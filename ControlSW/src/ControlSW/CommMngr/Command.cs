using System;

namespace CommMngrLib
{
    /// <summary>
    /// Represents a command message with parsed parts.
    /// Format: [ABC_DDD_VALUE...]
    /// </summary>
    public class Command
    {
        public string RawMsg { get; set; }
        public bool WaitForResp { get; set; }

        // Head (first 3 characters in the head part) e.g., "SXY" or "EXY"
        public string Head { get; set; }

        // Command ID (DDD)
        public string Cmd { get; set; }

        // The remaining value part (may contain underscores)
        public string Value { get; set; }

        public bool IsRequest => !string.IsNullOrEmpty(Head) && Head.Length >= 1 && Head[0] == 'S';
        public bool IsResponse => !string.IsNullOrEmpty(Head) && Head.Length >= 1 && Head[0] == 'E';
    }
}