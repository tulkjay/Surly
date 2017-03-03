using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.Remoting.Messaging;
using Surly.Core;
using Surly.Helpers;
using static System.ConsoleColor;
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

            Database.PrintAll();

            if (Debugger.IsAttached)
                Console.ReadLine();
        }

        private static void StartApplication()
        {
            var repeat = true;
            while (repeat)
            {
                ConsoleInterface.PrintMenu();
                repeat = ConsoleInterface.HandleSelection(Console.ReadLine());
            }
        }

        /*
        Help.
            list commands
        Exit.
        execute action
            new query
        */
    }
}