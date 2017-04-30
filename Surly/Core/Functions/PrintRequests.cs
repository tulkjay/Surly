using System;
using System.Collections.Generic;
using System.Linq;
using Surly.Core.Structure;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core.Functions
{
    public static class PrintRequests
    {
        public static SurlyTable GetTable(this SurlyDatabase database, string tableName)
        {
            var table = database.Tables.SingleOrDefault(x => x.Name == tableName.ToUpper());

            if (table != null) return table;

            var projection = SurlyProjections.GetInstance().Projections
                .SingleOrDefault(x => x.ProjectionName == tableName.ToUpper());

            if (projection != null)
            {
                return new SurlyTable
                {
                    Name = projection.ProjectionName,
                    Schema = projection.AttributeNames,
                    Tuples = projection.Tuples
                };
            }
            
            WriteLine($"\n\t{tableName.ToUpper()} was not found.", Red);

            return null;
        }
        public static void PrintTables(this SurlyDatabase database, IList<string> query)
        {
            const string id = "Id";

            query.RemoveAt(0);

            var tables = query                
                .Select(database.GetTable)
                .ToList();                

            foreach (var table in tables)
            {
                if (table == null) continue;

                WriteLine($"\n\t{table.Name}");
                Console.WriteLine();

                Console.Write($"  {id.PadRight(8)}");

                var widthReferences = new List<int>();

                foreach (var schema in table.Schema)
                {
                    var tableWidth = Math.Max(schema.Maximum + 2, schema.Name.Length + 2);

                    Console.Write($"{schema.Name.PadRight(tableWidth)}");
                    widthReferences.Add(tableWidth);
                }

                var count = 1;

                WriteLine("\n" + string.Empty.PadRight(100, '='), Green);

                Set(Yellow);

                foreach (var tableTuple in table.Tuples)
                {
                    var index = 0;
                    Console.Write($"  {count.ToString().PadRight(8)}");

                    foreach (var attribute in tableTuple)
                    {
                        Console.Write($"{attribute.Value.ToString().PadRight(widthReferences[index])}");
                        index++;
                    }

                    Console.WriteLine();

                    count++;
                }
                Console.WriteLine();
            }
        }

        public static void PrintCatalog(this SurlyDatabase database)
        {
            const string idHeader = "Id";
            const string nameHeader = "Name";
            const string typeHeader = "Type";
            const string maxHeader = "Maximum";

            WriteLine("\n\n\t*** CURRENT CATALOG ***\n");

            if (database.Tables.Count == 0) WriteLine("\t<--EMPTY-->\n\n", Red);

            foreach (var table in database.Tables)
            {
                var count = 1;

                WriteLine($"\n\tTable: {table.Name}", Blue);

                Console.WriteLine();

                WriteLine(
                    $"  {idHeader.PadRight(5)}{nameHeader.PadRight(20)}{typeHeader.PadRight(20)}{maxHeader}",
                    Yellow);

                WriteLine($"{string.Empty.PadRight(100, '=')}");

                foreach (var schema in table.Schema)
                {
                    WriteLine(
                        $"  {count.ToString().PadRight(5)}{schema.Name.PadRight(20)}{schema.Type.Name.PadRight(20)}{schema.Maximum}",
                        Green);
                    count++;
                }
            }
        }

        public static void PrintDatabase(this SurlyDatabase database)
        {
            const string id = "Id";

            WriteLine("\n\n\t*** FULL DATABASE ***\n");

            if (database.Tables.Count == 0) WriteLine("\t<--EMPTY-->\n\n", Red);

            foreach (var table in database.Tables)
            {
                WriteLine($"\n\tTable: {table.Name}\n");

                Console.Write($"  {id.PadRight(8)}");

                var widthReferences = new List<int>();

                foreach (var schema in table.Schema)
                {
                    var tableWidth = Math.Max(schema.Maximum + 2, schema.Name.Length + 2);

                    Console.Write($"{schema.Name.PadRight(tableWidth)}");
                    widthReferences.Add(tableWidth);
                }

                var count = 1;

                WriteLine("\n" + string.Empty.PadRight(100, '='), Green);

                Set(Yellow);

                foreach (var tableTuple in table.Tuples)
                {
                    var index = 0;
                    Console.Write($"  {count.ToString().PadRight(8)}");

                    foreach (var attribute in tableTuple)
                    {
                        Console.Write($"{attribute.Value.ToString().PadRight(widthReferences[index])}");
                        index++;
                    }

                    Console.WriteLine();

                    count++;
                }
                Console.WriteLine();
            }
        }
    }
}