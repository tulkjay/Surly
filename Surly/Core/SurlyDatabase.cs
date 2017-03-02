using System;
using System.Collections.Generic;
using System.Linq;

namespace Surly.Core
{
    //The database is a singleton
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
            Console.WriteLine("\nPrinting tables");
            foreach (var table in Tables)
            {
                Console.WriteLine($"\nTable: {table.Name}");

                foreach (var tuple in table.Schema)
                {
                    Console.Write($"\t{tuple.Name}");
                }

                foreach (var tableTuple in table.Tuples)
                {
                    foreach (var attribute in tableTuple)
                    {
                        Console.WriteLine($"val: {attribute.Value}");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}