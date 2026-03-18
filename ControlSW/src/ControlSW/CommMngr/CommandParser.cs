using System;

namespace CommMngrLib
{
    /// <summary>
    /// Parses incoming raw strings into Command objects if they match the format.
    /// Format expected: [ABC_DDD_VALUE...]
    /// </summary>
    public static class CommandParser
    {
        public static bool IsCommand(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return false;
            msg = msg.Trim();
            if (!msg.StartsWith("[") || !msg.EndsWith("]")) return false;
            var inner = msg.Substring(1, msg.Length - 2);
            var parts = inner.Split(new[] { '_' }, 3); // head, cmd, value
            if (parts.Length < 2) return false;
            // head must be at least 3 characters, cmd at least 3
            if (parts[0].Length < 3) return false;
            if (parts[1].Length < 3) return false;
            return true;
        }

        public static Command Parse(string msg)
        {
            if (!IsCommand(msg)) return null;
            var inner = msg.Trim().Substring(1, msg.Length - 2);
            var parts = inner.Split(new[] { '_' }, 3);
            var cmd = new Command
            {
                RawMsg = msg,
                Head = parts[0],
                Cmd = parts[1],
                Value = parts.Length >= 3 ? parts[2] : string.Empty
            };
            return cmd;
        }
    }
}