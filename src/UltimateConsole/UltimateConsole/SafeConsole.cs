using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace UltimateConsole
{
    public class ConsoleOptions
    {
        public int? Left { get; set; }
        public int? Top { get; set; }

        public bool HasToReturnToPreviousPosition { get; set; }

        public ConsoleColor? ForegroundColor { get; set; }
        public ConsoleColor? Background { get; set; }
    }

    public static class SafeConsole
    {
        private static readonly BlockingCollection<Action> _operations;
        private static readonly Task _writerTask;

        static SafeConsole()
        {
            _operations = new BlockingCollection<Action>();

            _writerTask = Task.Run(() =>
            {
                foreach (Action action in _operations.GetConsumingEnumerable())
                {
                    action();
                }
            });
        }

        public static void WriteLine(string line) => _operations.Add(() => Console.WriteLine(line));
        public static void Write(string line) => _operations.Add(() => Console.Write(line));

        public static void WriteLine(string line, ConsoleOptions options)
        {
            _operations.Add(() =>
            {
                InternalWrite(line + Environment.NewLine, options);
            });
        }

        public static void Write(string line, ConsoleOptions options)
        {
            _operations.Add((Action)(() =>
            {
                InternalWrite(line, options);
            }));
        }

        private static void InternalWrite(string line, ConsoleOptions options)
        {
            int top = Console.CursorTop;
            int left = Console.CursorLeft;

            Console.CursorLeft = options.Left ?? Console.CursorLeft;
            Console.CursorTop = options.Top ?? Console.CursorTop;

            Console.ForegroundColor = options?.ForegroundColor ?? Console.ForegroundColor;
            Console.BackgroundColor = options?.Background ?? Console.BackgroundColor;

            Console.Write(line);

            if (options.Background != null || options.ForegroundColor != null)
            {
                Console.ResetColor();
            }
            if (options.HasToReturnToPreviousPosition)
            {
                Console.SetCursorPosition(left, top);
            }
        }
    }

}
