using Worldpay.Innovation.WPWithin.Sample.Commands;

namespace Worldpay.Innovation.WPWithin.Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new Program().Run(args);
        }

        private void Run(string[] args)
        {
            CommandMenu menu = new CommandMenu();
            CommandResult result = CommandResult.NoOp;
            while (result != CommandResult.CriticalError && result != CommandResult.Exit)
            {
                result = menu.ReadEvalPrint(args);
            }
        }
    }
}