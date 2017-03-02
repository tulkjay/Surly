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
            const string idHeader = "Id";
            const string nameHeader = "Name";
            const string typeHeader = "Type";
            const string maxHeader = "Maximum";

            WriteLine("\n\n\t*** CURRENT CATALOG ***");

            foreach (var table in Tables)
            {
                var count = 1;
                WriteLine($"\n\tTable: {table.Name}", Color.Blue);
                Console.WriteLine();
                WriteLine($"  {idHeader.PadRight(5)}{nameHeader.PadRight(20)}{typeHeader.PadRight(20)}{maxHeader}", Color.Yellow);
                WriteLine($"{string.Empty.PadRight(100, '=')}");

                foreach (var schema in table.Schema)
                {
                    WriteLine($"  {count.ToString().PadRight(5)}{schema.Name.PadRight(20)}{schema.Type.Name.PadRight(20)}{schema.Maximum}", Color.Green);
                    count++;
                }
            }
        }

        public void PrintAll()
        {
            const string id = "Id";
            WriteLine("\n***FULL DATABASE***");

            foreach (var table in Tables)
            {
                WriteLine($"\n\tTable: {table.Name}");
                Console.WriteLine();

                Console.Write($"  {id.PadRight(8)}");

                foreach (var schema in table.Schema)
                {
                    Console.Write($"{schema.Name.PadRight(12)}");
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