using System;

namespace UltimateConsole
{
    public record ConsoleOptions
    {
        public int? Left { get; set; }
        public int? Top { get; set; }

        public bool HasToReturnToPreviousPosition { get; set; }

        public ConsoleColor? ForegroundColor { get; set; }
        public ConsoleColor? Background { get; set; }
    }

}
