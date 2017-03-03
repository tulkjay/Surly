using System;
using System.Collections.Generic;
using Surly.Core;
using static System.ConsoleColor;

namespace Surly.Helpers
{
    public static class ConsoleInterface
    {
        private static readonly SurlyDatabase Database = SurlyDatabase.GetInstance();

        public static void Set(ConsoleColor selection)
        {
            Console.ForegroundColor = selection;
        }

        public static void WelcomeOurGuests()
        {
            Set(Magenta);
            Console.WriteLine("\t\tWelcome, welcome! \n\n" +
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

        public static void WriteRow(List<string> dataList, string startString, ConsoleColor textColor = Magenta, ConsoleColor postTextColor = Cyan)
        {
            Set(textColor);

            Console.Write(startString);

            dataList.ForEach(x => Console.Write($"{x} "));

            Console.WriteLine();

            Set(postTextColor);
        }

        public static void PrintMenu()
        {
            Set(Cyan);
            Console.WriteLine("\n\tMain Menu");
            Set(Yellow);
            Console.WriteLine(string.Empty.PadRight(35, '='));
            Set(Cyan);
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
                    LoadFile();
                    break;

                case "4":
                    Database.PrintAll();
                    break;

                case "5":
                    PrintHelp();
                    break;

                case "6":
                    return false;

                default:
                    Console.WriteLine($"\n{input} is not an option. Please try again.\n");
                    return true;
            }
            return true;
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

        private static void LoadFile()
        {
            var reader = new SurlyFileReader();

            Console.Write("Enter the path to the file or press enter for default.(default): ");
            var path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path) || path == "default")
                path = Constants.SurlyInputFile;

            reader.ParseFile(path);
        }

        private static void PrintHelp()
        {
            string[] actions =
            {
                "RELATION <table-name> (<attribute-name> <attribute-type> <attribute-maximum-length>, ...);",
                "INSERT <table-name> <attribute-value-n> <attribute-value-n + 1> ... ;",
                "PRINT <table-name>, ... ;"
            };

            WriteLine("\nCurrently available query request actions are: \n", Yellow);
            foreach (var action in actions)
            {
                Console.WriteLine($"\t{action}\n");
            }
            Set(Yellow);
            Console.WriteLine("\nTo execute scripts from a file, you can either use the default Surly file or import one.\n");
        }

        private static void NewQuery(string query)
        {
            Database.ExecuteQuery(query);
        }
    }
}