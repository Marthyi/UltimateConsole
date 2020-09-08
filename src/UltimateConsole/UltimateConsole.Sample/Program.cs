using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace UltimateConsole.Sample
{
    public class Operation
    {
        public int Id { get; set; }
        public int sleepDuration { get; set; }
    }

    class Program
    {
        const int ConsoleSize = 20;

        static async Task Main(string[] args)
        {
            ActionBlock<Operation> action = new ActionBlock<Operation>(ActionOperationAsync, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 10 });

            var rnd = new Random();

            SafeConsole.WriteLine($"Start program", new ConsoleOptions() { Left = 0, Top = 20 });

            for (int i = 0; i < 100; i++)
            {
                DisplayOperation(i, ConsoleColor.Red);
                var operation = new Operation() { Id = i, sleepDuration = rnd.Next(5000) };
                SafeConsole.WriteLine($"sleep duration : {operation.sleepDuration}", new ConsoleOptions() { Left = 0, Top = 19 });
                action.Post(operation);
            }

            action.Complete();

            await action.Completion.ConfigureAwait(false);

            Console.ReadKey();
        }

        private static void DisplayOperation(int id, ConsoleColor color)
        {
            SafeConsole.Write(" ", new ConsoleOptions()
            {
                Background = color,
                Left = id % ConsoleSize,
                Top = id / ConsoleSize,
                HasToReturnToPreviousPosition = true                
            });
        }

        private static async Task ActionOperationAsync(Operation operation)
        {
            DisplayOperation(operation.Id, ConsoleColor.Yellow);
            await Task.Delay(operation.sleepDuration).ConfigureAwait(false);
            DisplayOperation(operation.Id, ConsoleColor.Green);
        }
    }
}
