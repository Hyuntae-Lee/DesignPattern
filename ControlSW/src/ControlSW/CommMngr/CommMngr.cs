using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommMngrLib
{
    /// <summary>
    /// TCP client manager that sends bracketed commands, queues requests while waiting for responses,
    /// and raises events when responses, commands, or comments are received.
    /// - OnRespReceive(msg): called when a response is received for the sent command.
    /// - OnCommandReceived(msg): called when a bracketed command is received that is NOT the response for the sent command.
    /// - OnCommentReceived(msg): called when a line of text is received that is not in command format.
    ///
    /// Note: It parses incoming text line by line. Each complete line is processed as a single received message.
    /// The final line is included even if it is not terminated by a newline character.
    /// </summary>
    public class CommMngr : IDisposable
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private readonly Queue<Command> _commandQueue = new Queue<Command>();
        private readonly object _queueLock = new object();
        private volatile bool _waitingResponse;
        private Command _currentCommand;
        private CancellationTokenSource _cts;
        private Task _receiveTask;
        private readonly StringBuilder _recvBuffer = new StringBuilder();
        private readonly object _recvLock = new object();

        /// <summary>
        /// Raised when a response is received for a previously sent command (the raw message).
        /// </summary>
        public event EventHandler<string> RespReceived;

        /// <summary>
        /// Raised when any bracketed command is received except the response for the sent command.
        /// </summary>
        public event EventHandler<string> CommandReceived;

        /// <summary>
        /// Raised when any non-command text is received.
        /// </summary>
        public event EventHandler<string> CommentReceived;

        /// <summary>
        /// Open TCP connection to remote server as client. Returns true if connection established.
        /// </summary>
        public bool OpenConnection(string ip, int port)
        {
            CloseConnection();

            try
            {
                _tcpClient = new TcpClient();
                var connectTask = _tcpClient.ConnectAsync(ip, port);
                // Wait for up to 5 seconds to connect
                if (!connectTask.Wait(TimeSpan.FromSeconds(5)))
                {
                    _tcpClient.Close();
                    _tcpClient = null;
                    return false;
                }

                if (!_tcpClient.Connected)
                {
                    _tcpClient.Close();
                    _tcpClient = null;
                    return false;
                }

                _stream = _tcpClient.GetStream();
                _cts = new CancellationTokenSource();
                _receiveTask = Task.Run(() => ReceiveLoop(_cts.Token));
                return true;
            }
            catch
            {
                _tcpClient?.Close();
                _tcpClient = null;
                _stream = null;
                return false;
            }
        }

        /// <summary>
        /// Close active connection and stop receiving.
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                _cts?.Cancel();
            }
            catch { }

            try
            {
                _receiveTask?.Wait(500);
            }
            catch { }

            _stream?.Close();
            _tcpClient?.Close();
            _stream = null;
            _tcpClient = null;

            lock (_queueLock)
            {
                _commandQueue.Clear();
                _waitingResponse = false;
                _currentCommand = null;
            }
        }

        public bool IsConnected()
        {
            return _tcpClient != null && _tcpClient.Connected;
        }

        /// <summary>
        /// Send a textual command. If bWaitForResp is true, CommMngr will wait for matching response before sending queued requests.
        /// </summary>
        public void SendCommand(string msg, bool bWaitForResp)
        {
            if (!IsConnected())
                throw new InvalidOperationException("Not connected");

            // Normalize line - keep message as-is; the protocol is bracketed message.
            var cmd = CommandParser.IsCommand(msg) ? CommandParser.Parse(msg) : new Command { RawMsg = msg };
            cmd.WaitForResp = bWaitForResp;

            lock (_queueLock)
            {
                if (bWaitForResp)
                {
                    if (!_waitingResponse)
                    {
                        // send immediately and mark waiting
                        SendRaw(msg);
                        _waitingResponse = true;
                        _currentCommand = cmd;
                    }
                    else
                    {
                        // queue request
                        _commandQueue.Enqueue(cmd);
                    }
                }
                else
                {
                    // send immediately without waiting for response
                    SendRaw(msg);
                }
            }
        }

        private void SendRaw(string msg)
        {
            if (!IsConnected()) return;

            try
            {
                var data = Encoding.UTF8.GetBytes(msg);
                _stream.Write(data, 0, data.Length);
                _stream.Flush();
            }
            catch (Exception)
            {
                // In production code log or raise error; for now, simply close connection to reset state.
                CloseConnection();
            }
        }

        private async Task ReceiveLoop(CancellationToken ct)
        {
            var buffer = new byte[1024];
            try
            {
                while (!ct.IsCancellationRequested && IsConnected())
                {
                    int read = 0;
                    try
                    {
                        var t = _stream.ReadAsync(buffer, 0, buffer.Length, ct);
                        read = await t.ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) { break; }
                    catch
                    {
                        // Connection error -> break
                        break;
                    }

                    if (read == 0)
                    {
                        // remote closed
                        break;
                    }

                    var chunk = Encoding.UTF8.GetString(buffer, 0, read);
                    ProcessIncoming(chunk);
                }
            }
            finally
            {
                // Ensure we include any final line even if it wasn't newline-terminated.
                FlushReceiveBuffer();
                CloseConnection();
            }
        }

        private void ProcessIncoming(string chunk)
        {
            // Append new data and parse by lines: '\n' is the token; trailing '\r' is trimmed.
            lock (_recvLock)
            {
                _recvBuffer.Append(chunk);

                while (true)
                {
                    var s = _recvBuffer.ToString();
                    var nlIndex = s.IndexOf('\n');
                    if (nlIndex >= 0)
                    {
                        // Extract a complete line (including '\n'), remove it from the buffer
                        var lineWithNewline = s.Substring(0, nlIndex + 1);
                        _recvBuffer.Remove(0, nlIndex + 1);

                        // Trim trailing newline and optional carriage return
                        var line = lineWithNewline.TrimEnd('\r', '\n');

                        // If the line is empty, skip
                        if (string.IsNullOrEmpty(line))
                            continue;

                        // Each complete line is considered one "message" to handle
                        HandleReceivedMessage(line);
                        // continue loop to process further complete lines
                    }
                    else
                    {
                        // No complete line available yet; protect against runaway buffer size
                        if (_recvBuffer.Length > 32 * 1024)
                        {
                            var leftover = _recvBuffer.ToString();
                            _recvBuffer.Clear();
                            // Treat oversized incomplete content as a comment
                            OnCommentReceived(leftover);
                        }
                        break;
                    }
                }
            }
        }

        // Flush any remaining text in buffer as final line(s), even if no trailing newline.
        private void FlushReceiveBuffer()
        {
            string leftover = null;
            lock (_recvLock)
            {
                if (_recvBuffer.Length > 0)
                {
                    leftover = _recvBuffer.ToString();
                    _recvBuffer.Clear();
                }
            }

            if (string.IsNullOrEmpty(leftover))
                return;

            // The leftover may contain multiple lines without final newline; split and process each.
            var parts = leftover.Split(new[] { '\n' }, StringSplitOptions.None);
            foreach (var part in parts)
            {
                var line = part.TrimEnd('\r');
                if (string.IsNullOrEmpty(line))
                    continue;
                HandleReceivedMessage(line);
            }
        }

        private void HandleReceivedMessage(string raw)
        {
            if (CommandParser.IsCommand(raw))
            {
                var parsed = CommandParser.Parse(raw);
                // If this is a response and we are waitingResponse and it matches current command -> response
                bool isResp = parsed.IsResponse;
                bool matched = false;
                lock (_queueLock)
                {
                    if (isResp && _waitingResponse && _currentCommand != null && !string.IsNullOrEmpty(_currentCommand.Head))
                    {
                        // To match request and response, they have same 'BC' and 'DDD':
                        // _currentCommand.Head is like "SXY" (S + BC)
                        // parsed.Head is like "EXY" (E + BC)
                        if (_currentCommand.Head.Length >= 3 && parsed.Head.Length >= 3 &&
                            _currentCommand.Head.Substring(1, 2) == parsed.Head.Substring(1, 2) &&
                            _currentCommand.Cmd == parsed.Cmd)
                        {
                            matched = true;
                            // consume response
                            _waitingResponse = false;
                            _currentCommand = null;
                            // After matched, if queue not empty send next queued command(s) appropriately
                            if (_commandQueue.Count > 0)
                            {
                                var next = _commandQueue.Dequeue();
                                SendRaw(next.RawMsg);
                                if (next.WaitForResp)
                                {
                                    _waitingResponse = true;
                                    _currentCommand = next;
                                }
                                else
                                {
                                    // If additional queued commands exist, continue sending until a wait-for-resp arrives
                                    while (!_waitingResponse && _commandQueue.Count > 0)
                                    {
                                        var n = _commandQueue.Dequeue();
                                        SendRaw(n.RawMsg);
                                        if (n.WaitForResp)
                                        {
                                            _waitingResponse = true;
                                            _currentCommand = n;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (matched)
                {
                    OnRespReceive(raw);
                }
                else
                {
                    // Bracketed command but not matching current request => treat as command (not a comment)
                    OnCommandReceived(raw);
                }
            }
            else
            {
                // Not a bracketed command -> comment
                OnCommentReceived(raw);
            }
        }

        protected virtual void OnRespReceive(string msg)
        {
            try
            {
                RespReceived?.Invoke(this, msg);
            }
            catch { }
        }

        protected virtual void OnCommandReceived(string msg)
        {
            try
            {
                CommandReceived?.Invoke(this, msg);
            }
            catch { }
        }

        protected virtual void OnCommentReceived(string msg)
        {
            try
            {
                CommentReceived?.Invoke(this, msg);
            }
            catch { }
        }

        public void Dispose()
        {
            CloseConnection();
            _cts?.Dispose();
        }
    }
}