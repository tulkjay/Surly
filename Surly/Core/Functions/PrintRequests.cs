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
        const string Id = "Id";

        public static SurlyTableResponse GetTable(this SurlyDatabase database, string tableName)
        {
            var table = database.Tables.SingleOrDefault(x => x.Name == tableName.ToUpper());

            if (table != null) return new SurlyTableResponse {Table = table};

            var projection = SurlyProjections.GetInstance().Projections
                .SingleOrDefault(x => x.ProjectionName == tableName.ToUpper());

            if (projection != null)
            {
                return new SurlyTableResponse
                {
                    Table = new SurlyTable
                    {
                        Name = projection.ProjectionName,
                        Schema = projection.AttributeNames,
                        Tuples = projection.Tuples,
                    },
                    IsProjection = true,
                    HideIndexes = projection.HideIndex
                };
            }
            
            WriteLine($"\n\t{tableName.ToUpper()} was not found.", Red);

            return null;
        }

        public static void PrintTables(this SurlyDatabase database, IList<SurlyTableResponse> tables)
        {
            foreach (var response in tables)
            {
                if (response == null) continue;

                WriteLine($"\n\t{response.Table.Name}");
                Console.WriteLine();

                Console.Write($"  {(response.IsProjection ? Id.PadRight(8) : "")}");

                var widthReferences = new List<int>();

                foreach (var schema in response.Table.Schema)
                {
                    var tableWidth = Math.Max(schema.Maximum + 2, schema.Name.Length + 2);

                    Console.Write($"{schema.Name.PadRight(tableWidth)}");

                    widthReferences.Add(tableWidth);
                }

                var count = 1;

                WriteLine("\n" + string.Empty.PadRight(response.HideIndexes ? 165 : 100, '='), Green);

                Set(Yellow);

                foreach (var tableTuple in response.Table.Tuples)
                {
                    var index = 0;

                    Console.Write($"  {(response.IsProjection ? count.ToString().PadRight(8) : "")}");

                    foreach (var attribute in tableTuple)
                    {
                        try
                        {
                            Console.Write($"{attribute.Value.ToString().PadRight(widthReferences[index])}");
                            index++;
                        }
                        catch (Exception)
                        {
                            Console.Write($"{attribute.Value.ToString().PadRight(8)}");
                            return;
                        }
                    }

                    Console.WriteLine();

                    count++;
                }
                Console.WriteLine();
            }
        }
        public static void Print(this SurlyDatabase database, IList<string> query)
        {
            query.RemoveAt(0);

            var tablesQueryResponse = query                
                .Select(database.GetTable)
                .ToList();                

            database.PrintTables(tablesQueryResponse);
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

    public class SurlyTableResponse
    {
        public SurlyTable Table { get; set; }
        public bool IsProjection { get; set; }
        public bool HideIndexes { get; set; }
    }
}