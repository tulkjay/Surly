using System;
using System.Collections.Generic;
using Surly.Core.Functions;
using Surly.Core.Structure;
using static System.ConsoleColor;

namespace Surly.Helpers
{
    public static class ConsoleInterface
    {
        private static readonly SurlyDatabase Database = SurlyDatabase.GetInstance();

        public static List<ConsoleColor> ConsoleColors = new List<ConsoleColor>
        {
            Red,
            Cyan,
            DarkCyan,
            DarkGreen,
            DarkMagenta,
            DarkRed,
            DarkYellow,
            Gray,
            Green,
            Magenta,
            Yellow,
            DarkYellow,
            White
        };

        public static void Set(ConsoleColor selection)
        {
            Console.ForegroundColor = selection;
        }

        public static void WelcomeOurGuests()
        {
            Set(Magenta);
            MakeEpic("\n\t\tWelcome, welcome! \n\n" +
                     "\t\tYou are thusly a guest to the surliest database under the sun! Begin..." +
                     "\n\n");
            Set(Cyan);
        }

        public static void WriteLine(string text, ConsoleColor textColor = Magenta, ConsoleColor postTextColor = Cyan)
        {
            Set(textColor);
            Console.WriteLine(text);
            Set(postTextColor);
        }

        public static void Write(string text, ConsoleColor textColor = Magenta, ConsoleColor postTextColor = Cyan)
        {
            Set(textColor);
            Console.Write(text);
            Set(postTextColor);
        }



        public static void WriteRow(List<string> dataList, string startString, ConsoleColor textColor = Magenta,
            ConsoleColor postTextColor = Cyan)
        {
            Set(textColor);

            Console.Write(startString);

            dataList.ForEach(x => Console.Write($"{x} "));

            Console.WriteLine();

            Set(postTextColor);
        }

        public static void PrintMenu()
        {
            WriteLine("\n\tMain Menu");
            WriteLine(string.Empty.PadRight(35, '='), Yellow);

            Console.WriteLine("\t1. New Query");
            Console.WriteLine("\t2. Print Catalog");
            Console.WriteLine("\t3. Execute Script File");
            Console.WriteLine("\t4. Print Full Database");
            Console.WriteLine("\t5. Help");
            Console.WriteLine("\t6. Exit");

            Set(Blue);
            Console.Write("\nPlease make a selection: ");
            Set(Magenta);
        }

        public static bool HandleSelection(string input)
        {
            switch (input)
            {
                case "1":
                    NewQueryHandler();
                    break;

                case "2":
                    Database.PrintCatalog();
                    break;

                case "3":
                    SurlyFileReader.LoadFile();
                    break;

                case "4":
                    Database.PrintDatabase();
                    break;

                case "5":
                    PrintHelp();
                    break;

                case "6":
                    MakeEpic("\n\tCome again soon!");
                    return false;

                default:
                    Console.WriteLine($"\n{input} is not an option. Please enter a valid selection.");
                    return true;
            }
            return true;
        }

        public static void MakeEpic(string message)
        {
            var messageArray = message.ToCharArray();

            var rand = new Random();

            foreach (var c in messageArray)
            {
                var colorIndex = rand.Next() % ConsoleColors.Count;
                Console.ForegroundColor = ConsoleColors[colorIndex];
                Console.Write(c);
            }

            Set(Cyan);
            Console.WriteLine();
        }

        private static void NewQueryHandler()
        {
            var repeat = true;

            PrintNewQuerySection();

            while (repeat)
            {
                var query = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(query)) continue;

                switch (query.ToLower())
                {
                    case "help":
                        PrintHelp();
                        PrintNewQuerySection();
                        break;

                    case "exit":
                        repeat = false;
                        break;

                    default:
                        NewQuery(query);
                        PrintNewQuerySection();
                        break;
                }
            }
        }

        private static void PrintNewQuerySection()
        {
            WriteLine("\n('help' to display help, 'exit' to go to menu)", Yellow, Magenta);

            Console.Write("\nEnter a query: \n\t> ");

            Set(Cyan);
        }

        private static void PrintHelp()
        {
            string[] actions =
            {
                "RELATION <table-name> (<attribute-name> <attribute-type> <attribute-maximum-length>, ...);",
                "INSERT <table-name> <attribute-value-n> <attribute-value-n + 1> ... ;",
                "PRINT <table-name>, ... ;", 
                "DELETE <table-name>;",
                "<projection-name> = PROJECT <attribute-name>, <attribute-name>, ... FROM <table-name>;"
            };

            WriteLine("\n" + string.Empty.PadRight(110, '='), Green);
            WriteLine("Currently available query request actions are: \n", Yellow);

            foreach (var action in actions)
                Console.WriteLine($"\t{action}\n");

            WriteLine(
                "\nTo execute scripts from a file, you can either use the default Surly file or enter the file path name.",
                Yellow);
            WriteLine(string.Empty.PadRight(110, '='), Green);
        }

        private static void NewQuery(string query)
        {
            Database.ExecuteQuery(query);
        }
    }
}