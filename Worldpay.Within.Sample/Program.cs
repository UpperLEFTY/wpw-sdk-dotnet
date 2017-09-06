using Worldpay.Within.Sample.Commands;

using System;
using System.Runtime.InteropServices;

namespace Worldpay.Within.Sample
{
    internal class Program
    {
        private static CommandMenu menu;


        private static void Main(string[] args)
        {
            new Program().Run(args);
        }

        private void Run(string[] args)
        {
            // managing process shutdown
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            menu = new CommandMenu();
            CommandResult result = CommandResult.NoOp;
            while (result != CommandResult.CriticalError && result != CommandResult.Exit)
            {
                result = menu.ReadEvalPrint(args);
            }
        }

#region Managing Process shutdown gracefully (Ctrl+C instead of kill)

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);


        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                Console.WriteLine("Console window closing.");
                if (menu != null)
                {
                    menu.TerminateChilds();
                }
            }
            return false;
        }

#endregion
    }
}