using System;
using System.Diagnostics;
using Surly.Core;
using Surly.Helpers;
using static Surly.Helpers.ConsoleInterface;

namespace Surly
{
    public static class Program
    {
        public static SurlyDatabase Database = SurlyDatabase.GetInstance();

        public static void Main(string[] args)
        {
            WelcomeOurGuests();

            var reader = new SurlyFileReader();

            reader.ParseFile(Constants.SurlyInputFile);

            Database.PrintAll();

            if (Debugger.IsAttached)
                Console.ReadLine();
        }
    }
}