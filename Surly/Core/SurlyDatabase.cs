using System;
using System.Collections.Generic;
using System.Linq;
using Surly.Helpers;
using static System.ConsoleColor;
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

            WriteLine("\n\n\t*** CURRENT CATALOG ***\n");

            if (Tables.Count == 0) WriteLine("\t<--EMPTY-->\n\n", Red);

            foreach (var table in Tables)
            {
                var count = 1;

                WriteLine($"\n\tTable: {table.Name}", Blue);

                Console.WriteLine();

                WriteLine($"  {idHeader.PadRight(5)}{nameHeader.PadRight(20)}{typeHeader.PadRight(20)}{maxHeader}", Yellow);

                WriteLine($"{string.Empty.PadRight(100, '=')}");

                foreach (var schema in table.Schema)
                {
                    WriteLine(
                        $"  {count.ToString().PadRight(5)}{schema.Name.PadRight(20)}{schema.Type.Name.PadRight(20)}{schema.Maximum}", Green);
                    count++;
                }
            }
        }

        public void PrintDatabase()
        {
            const string id = "Id";

            WriteLine("\n\n\t*** FULL DATABASE ***\n");

            if (Tables.Count == 0) WriteLine("\t<--EMPTY-->\n\n", Red);

            foreach (var table in Tables)
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

        public void ExecuteQuery(string line)
        {
            var steps = line.Split(' ').ToList();

            Set(Cyan);
            switch (steps[0].ToUpper())
            {
                case "RELATION":
                    WriteLine($"Creating table: {steps[1]}", Cyan);
                    CreateTable(steps[1].ToUpper(), line);
                    break;

                case "INSERT":
                    var rowAdded = AddTuples(steps[1].ToUpper(), line);

                    if (rowAdded)
                        WriteLine($"Row added to {steps[1].ToUpper()}", Green);
                    break;

                case "PRINT":
                    Set(Cyan);

                    Print(line.Replace(",", "").Split(' ').ToList());

                    Set(Magenta);
                    break;

                case "DELETE":
                    var TableDelted = DeleteTables(steps[1].ToUpper(), line);
                    if (TableDelted)
                        WriteLine($"Deleted Table Named {steps[1].ToUpper()}",Green);
                    break;
                default:
                    WriteLine($"Not sure about this command: {steps[0].ToUpper()}", Red);
                    break;
            }
        }

        private void Print(IList<string> steps)
        {
            const string id = "Id";

            steps.RemoveAt(0);

            var tables = steps
                .Select(step => Tables
                    .SingleOrDefault(x => x.Name == step.ToUpper()))
                .ToList();

            foreach (var table in tables)
            {
                if (table == null) continue;

                WriteLine($"\n\tTable: {table.Name}");
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

        private void CreateTable(string tableName, string line)
        {
            Set(Yellow);

            if (Tables.Any(x => x.Name == tableName))
            {
                WriteLine($"Table {tableName} already exists. Please select a different table name.", Red);
                return;
            }

            var tuples = string.Empty;
            try
            {
                tuples = line
                    .Substring(line.IndexOf("(", StringComparison.Ordinal) + 1,
                        line.IndexOf(")", StringComparison.Ordinal) - line.IndexOf("(", StringComparison.Ordinal) - 1);
            }
            catch (Exception)
            {
                WriteLine("Syntax error", Red);
            }

            Set(Green);
            var tableCreated = false;

            foreach (var tuple in tuples.Split(','))
            {
                var parts = tuple.Trim().Split(' ');
                int numMax;

                if (!ValidTuple(tableName, parts)) return;

                if (!tableCreated)
                {
                    Tables.AddLast(new SurlyTable(tableName));
                    tableCreated = true;
                }

                Tables.Last.Value.Schema.AddLast(new SurlyAttributeSchema
                {
                    Name = parts[0],
                    Type = parts[1].ToSurlyType(),
                    Maximum = int.TryParse(parts[2], out numMax) ? numMax : 0
                });
            }

            Console.WriteLine();
        }

        private bool ValidTuple(string tableName, string[] parts)
        {
            if (parts.Any(string.IsNullOrWhiteSpace))
            {
                WriteLine("Invalid syntax in schema definition", Red);
                return false;
            }

            var table = Tables
                .SingleOrDefault(x => x.Name == tableName);

            if (table != null && table.Schema
                    .Any(x => x.Name == parts?[0]))
            {
                WriteLine($"{parts[0]} already exists. Please select a different attribute name.", Red);
                return false;
            }

            try
            {
                if (parts[1].ToSurlyType() == null)
                {
                    WriteLine($"{parts[1]} is not a recognized type.", Red);
                    return false;
                }
            }
            catch (Exception)
            {
                WriteLine($"{parts[1]} is not a recognized type.", Red);
                return false;
            }
            return true;
        }

        private bool AddTuples(string tableName, string line)
        {
            var rowsAdded = false;
            if (Tables.All(x => x.Name != tableName))
            {
                WriteLine($"Table {tableName} was not found.", Red);
                return false;
            }

            var tuples = string.Format(new SurlyFormatter(), "{0:insert}", line).SplitValues();
            var table = Tables.Single(x => x.Name == tableName);
            var schema = table.Schema.ToArray();

            var newTuple = new LinkedList<SurlyAttribute>();

            for (var i = 0; i < schema.Length; i++)
                newTuple.AddLast(new SurlyAttribute { Value = tuples[i].To(schema[i].Type, schema[i].Maximum) });

            if (newTuple.Count > 0)
            {
                table.Tuples.AddLast(newTuple);
                rowsAdded = true;
            }
            return rowsAdded;
        }

        private bool DeleteTables(string tableName, string line)
        {
            var DeleteSucess = false;
            if (Tables.All(x => x.Name != tableName))
            {
                WriteLine($"Table {tableName} was not found.", Red);
                return false;
            }
            var table = Tables.SingleOrDefault(x => x.Name == tableName);
            DeleteSucess = Tables.Remove(table);
            return DeleteSucess;

        }

        private bool DeleteTouple(string tableName, string line)
        {
            {
                var rowsAdded = false;
                if (Tables.All(x => x.Name != tableName))
                {
                    WriteLine($"Table {tableName} was not found.", Red);
                    return false;
                }
                return rowsAdded;
            }
        }
    }
}