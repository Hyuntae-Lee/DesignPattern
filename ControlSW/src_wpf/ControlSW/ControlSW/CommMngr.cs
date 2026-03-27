using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ControlSW
{
    public class CommMngr : IDisposable
    {
        private TcpClient _tcp;
        private NetworkStream _stream;
        private readonly object _lock = new object();
        private readonly Queue<Command> _commandQueue = new Queue<Command>();
        private bool _waitingResponse = false;
        private Command _currentCommand;
        private CancellationTokenSource _readCts;

        public Action<string> OnRespReceive;
        public Action<string> OnCommandReceived;
        public Action<string> OnCommentReceived;
        public Action<bool> OnConnectionStatusChanged; // true = connected

        public bool OpenConnection(string ip, int port)
        {
            try
            {
                CloseConnection();

                _tcp = new TcpClient();
                var connectTask = _tcp.ConnectAsync(ip, port);
                connectTask.Wait(3000);
                if (!_tcp.Connected)
                {
                    CloseConnection();
                    return false;
                }

                _stream = _tcp.GetStream();
                _readCts = new CancellationTokenSource();
                Task.Run(() => ReaderLoopAsync(_readCts.Token));

                OnConnectionStatusChanged?.Invoke(true);
                return true;
            }
            catch
            {
                CloseConnection();
                return false;
            }
        }

        public void CloseConnection()
        {
            try
            {
                _readCts?.Cancel();
            }
            catch { }

            try
            {
                _stream?.Close();
                _tcp?.Close();
            }
            catch { }

            lock (_lock)
            {
                _waitingResponse = false;
                _currentCommand = null;
                _commandQueue.Clear();
            }

            OnConnectionStatusChanged?.Invoke(false);
        }

        public bool IsConnected()
        {
            return _tcp != null && _tcp.Connected;
        }

        public void SendCommand(string msg, bool bWaitForResp)
        {
            if (!IsConnected()) throw new InvalidOperationException("Not connected");

            var cmd = CommandParser.Parse(msg, bWaitForResp);

            lock (_lock)
            {
                // If system is currently waiting for a response, queue all incoming client requests.
                if (_waitingResponse)
                {
                    _commandQueue.Enqueue(cmd);
                    return;
                }

                // Not waiting: if this command requires waiting, send and become waiting.
                if (cmd.WaitForResp)
                {
                    _currentCommand = cmd;
                    _waitingResponse = true;
                    _ = SendRawAsync(cmd.RawMsg);
                }
                else
                {
                    // send immediately and remain not waiting
                    _ = SendRawAsync(cmd.RawMsg);
                }
            }
        }

        private async Task SendRawAsync(string raw)
        {
            try
            {
                if (!IsConnected()) return;
                // ensure newline terminated when sending
                var outMsg = raw.EndsWith("\n") ? raw : raw + "\n";
                var bytes = Encoding.UTF8.GetBytes(outMsg);
                await _stream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch
            {
                // on send error, close connection
                CloseConnection();
            }
        }

        private async Task ReaderLoopAsync(CancellationToken ct)
        {
            var buffer = new byte[4096];
            var sb = new StringBuilder();

            try
            {
                while (!ct.IsCancellationRequested && IsConnected())
                {
                    if (!_stream.CanRead) break;
                    int read = 0;
                    try
                    {
                        var t = _stream.ReadAsync(buffer, 0, buffer.Length, ct);
                        read = await t;
                    }
                    catch (OperationCanceledException) { break; }
                    catch
                    {
                        // remote closed or network error
                        break;
                    }

                    if (read == 0)
                    {
                        // connection closed
                        break;
                    }

                    var chunk = Encoding.UTF8.GetString(buffer, 0, read);
                    sb.Append(chunk);

                    // process full lines delimited by \n
                    string all = sb.ToString();
                    int idx;
                    while ((idx = all.IndexOf('\n')) >= 0)
                    {
                        var line = all.Substring(0, idx);
                        all = all.Substring(idx + 1);
                        ProcessReceivedLine(line.TrimEnd('\r'));
                    }

                    // keep remainder (possibly final line not terminated)
                    sb.Clear();
                    sb.Append(all);

                    // If no more data available and stream read returned < buffer, still process final line (per spec)
                    if (_stream.DataAvailable == false && sb.Length > 0)
                    {
                        // process final non-terminated line
                        var finalLine = sb.ToString();
                        sb.Clear();
                        ProcessReceivedLine(finalLine.TrimEnd('\r'));
                    }
                }
            }
            finally
            {
                // cleanup on reader end
                CloseConnection();
            }
        }

        private void ProcessReceivedLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return;

            if (CommandParser.IsCommand(line))
            {
                var parsed = CommandParser.Parse(line, waitForResp: false);
                // if it's a response (head[0] == 'E')
                if (parsed.Head.Length >= 1 && parsed.Head[0] == 'E')
                {
                    bool matched = false;
                    lock (_lock)
                    {
                        if (_currentCommand != null)
                        {
                            // match by BC (chars 1-2 of head) and DDD (command)
                            var curBC = (_currentCommand.Head.Length >= 3) ? _currentCommand.Head.Substring(1, 2) : null;
                            var curDDD = _currentCommand.Cmd;
                            var respBC = (parsed.Head.Length >= 3) ? parsed.Head.Substring(1, 2) : null;
                            var respDDD = parsed.Cmd;

                            if (curBC != null && respBC != null && curBC == respBC && curDDD == respDDD)
                            {
                                matched = true;
                            }
                        }

                        if (matched)
                        {
                            // response to current command
                            var msg = line;
                            try { OnRespReceive?.Invoke(msg); } catch { }
                            // free waiting and send next queued
                            _currentCommand = null;
                            _waitingResponse = false;

                            if (_commandQueue.Count > 0)
                            {
                                var next = _commandQueue.Dequeue();
                                if (next.WaitForResp)
                                {
                                    _currentCommand = next;
                                    _waitingResponse = true;
                                    _ = SendRawAsync(next.RawMsg);
                                }
                                else
                                {
                                    _ = SendRawAsync(next.RawMsg);
                                }
                            }
                        }
                        else
                        {
                            // response that doesn't match current request -> treat as command received (not a response)
                            try { OnCommandReceived?.Invoke(line); } catch { }
                        }
                    }
                }
                else
                {
                    // it's a request/command from server side
                    try { OnCommandReceived?.Invoke(line); } catch { }
                }
            }
            else
            {
                // comment
                try { OnCommentReceived?.Invoke(line); } catch { }
            }
        }

        public void Dispose()
        {
            CloseConnection();
        }

        // Internal command representation and parser
        private class Command
        {
            public string RawMsg { get; set; }
            public bool WaitForResp { get; set; }
            public string Head { get; set; } = "";
            public string Cmd { get; set; } = "";
            public string Value { get; set; } = "";
        }

        private static class CommandParser
        {
            public static bool IsCommand(string msg)
            {
                if (string.IsNullOrEmpty(msg)) return false;
                if (!(msg.StartsWith("[") && msg.EndsWith("]"))) return false;
                var inner = msg.Substring(1, msg.Length - 2);
                var parts = inner.Split(new[] { '_' }, 3);
                if (parts.Length < 2) return false;
                if (parts[0].Length < 3 || parts[1].Length < 3) return false;
                return true;
            }

            public static Command Parse(string msg, bool waitForResp)
            {
                var cmd = new Command { RawMsg = msg, WaitForResp = waitForResp };
                if (!IsCommand(msg)) return cmd;

                var inner = msg.Substring(1, msg.Length - 2);
                var parts = inner.Split(new[] { '_' }, 3);
                cmd.Head = parts[0];
                cmd.Cmd = parts.Length > 1 ? parts[1] : "";
                cmd.Value = parts.Length > 2 ? parts[2] : "";
                return cmd;
            }
        }
    }
}