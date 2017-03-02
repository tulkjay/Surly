using System;
using System.Collections.Generic;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core
{
    //This database is a singleton
    public class SurlyDatabase
    {
        private static SurlyDatabase _instance;
        public LinkedList<SurlyTable> Tables { get; set; } = new LinkedList<SurlyTable>();

        private SurlyDatabase()
        {
        }

        public static SurlyDatabase GetInstance()
        {
            return _instance ?? (_instance = new SurlyDatabase());
        }

        public void PrintCatalog()
        {
            const string id = "Id";
            Console.WriteLine("\nPrinting tables");

            foreach (var table in Tables)
            {
                WriteLine($"\n\tTable: {table.Name}");
                Console.WriteLine();

                Console.Write($"  {id.PadRight(8)}");

                foreach (var tuple in table.Schema)
                {
                    Console.Write($"{tuple.Name.PadRight(12)}");
                }

                var count = 1;

                WriteLine("\n" + string.Empty.PadRight(100, '='), Color.Green);

                Set(Color.Yellow);

                foreach (var tableTuple in table.Tuples)
                {
                    Console.Write($"  {count.ToString().PadRight(8)}");

                    foreach (var attribute in tableTuple)
                    {
                        Console.Write($"{attribute.Value.ToString().PadRight(12)}");
                    }

                    Console.WriteLine();

                    count++;
                }
                Console.WriteLine();
            }
        }
    }
}