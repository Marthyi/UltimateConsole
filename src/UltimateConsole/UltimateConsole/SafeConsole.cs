using System;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace UltimateConsole
{

    public static class SafeConsole
    {
        private static readonly Channel<Action> _operations;
        private static readonly Task _writerTask;

        static SafeConsole()
        {
            _operations = Channel.CreateUnbounded<Action>();

            _writerTask = Task.Run(async () =>
           {
               while (await _operations.Reader.WaitToReadAsync())
               {
                   (await _operations.Reader.ReadAsync())();
               }
           });
        }

        public static void WriteLine(string line) => AddAsync(() => Console.WriteLine(line));
        public static void Write(string line) => AddAsync(() => Console.Write(line));
        public static void WriteLine(string line, ConsoleOptions options) => AddAsync(() => InternalWrite(line + Environment.NewLine, options));
        public static void Write(string line, ConsoleOptions options) => AddAsync(() => InternalWrite(line, options));

        private static ValueTask AddAsync(Action action) => _operations.Writer.WriteAsync(action);

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
