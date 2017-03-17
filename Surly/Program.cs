using System;
using System.Diagnostics;
using Surly.Core.Structure;
using static Surly.Helpers.ConsoleInterface;

namespace Surly
{
    public static class Program
    {
        public static SurlyDatabase Database = SurlyDatabase.GetInstance();

        public static void Main(string[] args)
        {
            WelcomeOurGuests();

            StartApplication();

            if (Debugger.IsAttached)
            {
                Console.Write("\nPress Enter to close the window... ");
                Console.ReadLine();
            }
        }

        private static void StartApplication()
        {
            var repeat = true;
            while (repeat)
            {
                PrintMenu();
                repeat = HandleSelection(Console.ReadLine());
            }
        }
    }
}