using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Surly.Helpers;

namespace Surly.Core
{
    //TODO - Pull out functions that should be defined in the database definition.
    public class SurlyFileReader
    {
        public SurlyDatabase Database = SurlyDatabase.GetInstance();
        private string _repeatedTableName = string.Empty;

        public void ParseFile(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        ParseLine(reader.ReadLine());
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleInterface.Set(ConsoleInterface.Color.Red);

                Console.WriteLine($"Error: {ex.Message}");

                ConsoleInterface.Set(ConsoleInterface.Color.Cyan);
            }
        }

        public void ParseLine(string line)
        {
            if (line.Length == 0 || line.Contains("/*")) return;
            ExecuteCommand(line);
        }

        public void ExecuteCommand(string line)
        {
            var steps = line.Split(' ').ToList();

            var repeatName = _repeatedTableName == steps[1];
            _repeatedTableName = _repeatedTableName == steps[1]
                ? _repeatedTableName
                : steps[1];

            ConsoleInterface.Set(ConsoleInterface.Color.Cyan);
            switch (steps[0].ToLower())
            {
                case "relation":
                    Console.WriteLine("Creating table: " + steps[1] + "\n");

                    ConsoleInterface.Set(ConsoleInterface.Color.Cyan);

                    CreateTable(steps[1], line);

                    ConsoleInterface.Set(ConsoleInterface.Color.Magenta);
                    break;

                case "insert":
                    if (!repeatName) Console.WriteLine("\nAdding tuple(s) to " + steps[1]);

                    ConsoleInterface.Set(ConsoleInterface.Color.Cyan);

                    AddTuples(steps[1], line);

                    ConsoleInterface.Set(ConsoleInterface.Color.Magenta);
                    break;

                case "print":
                    ConsoleInterface.Set(ConsoleInterface.Color.Cyan);

                    Print(line.Replace(",", "").Split(' ').ToList());

                    ConsoleInterface.Set(ConsoleInterface.Color.Magenta);
                    break;

                default:
                    ConsoleInterface.Set(ConsoleInterface.Color.Red);

                    Console.WriteLine("Not sure about this command: " + steps[0]);

                    ConsoleInterface.Set(ConsoleInterface.Color.Cyan);
                    break;
            }
        }

        private void Print(IList<string> steps)
        {
            const string id = "Id";

            steps.RemoveAt(0);

            var tables = steps
                .Select(step => Database.Tables
                    .SingleOrDefault(x => x.Name == step))
                .ToList();

            foreach (var table in tables)
            {
                if (table == null) continue;

                ConsoleInterface.WriteLine($"\n\tTable: {table.Name}");
                Console.WriteLine();

                Console.Write($"  {id.PadRight(8)}");

                foreach (var schema in table.Schema)
                {
                    Console.Write($"{schema.Name.PadRight(12)}");
                }

                var count = 1;

                ConsoleInterface.WriteLine("\n" + string.Empty.PadRight(100, '='), ConsoleInterface.Color.Green);

                ConsoleInterface.Set(ConsoleInterface.Color.Yellow);

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

        private void CreateTable(string tableName, string line)
        {
            ConsoleInterface.Set(ConsoleInterface.Color.Yellow);

            Console.WriteLine($"About to add {tableName} using {line}\n");

            Database.Tables.AddLast(new SurlyTable(tableName));

            var tuples = line
                .Substring(line.IndexOf("(", StringComparison.Ordinal) + 1,
                    (line.IndexOf(")", StringComparison.Ordinal) - line.IndexOf("(", StringComparison.Ordinal) - 1));

            Console.WriteLine($"Tuples: {tuples}\n");

            var test = tuples.Split(',');

            ConsoleInterface.Set(ConsoleInterface.Color.Green);

            foreach (var tuple in test)
            {
                Console.WriteLine($"About to parse: {tuple}");

                var parts = tuple.Trim().Split(' ');
                int numMax;

                Database.Tables.Last.Value.Schema.AddLast(new SurlyAttributeSchema
                {
                    Name = parts[0],
                    Type = parts[1].ToSurlyType(),
                    Maximum = int.TryParse(parts[2], out numMax) ? numMax : 0
                });
            }

            Console.WriteLine();
        }

        private void AddTuples(string tableName, string line)
        {
            var tuples = string.Format(new SurlyFormatter(), "{0:insert}", line).SplitValues();
            var table = Database.Tables.Single(x => x.Name == tableName);
            var schema = table.Schema.ToArray();

            ConsoleInterface.WriteRow(tuples.ToList(), "\tParsed: ", ConsoleInterface.Color.Blue);

            var newTuple = new LinkedList<SurlyAttribute>();

            for (var i = 0; i < schema.Length; i++)
            {
                newTuple.AddLast(new SurlyAttribute { Value = tuples[i].To(schema[i].Type, schema[i].Maximum) });
            }

            if (newTuple.Count > 0) table.Tuples.AddLast(newTuple);
        }

        //It may not be necessary to create a recursive aggregator.
        public static LinkedList<LinkedList<SurlyTuple>> SurlyZip(LinkedList<SurlyAttributeSchema> schema, params object[] items)
        {
            ConsoleInterface.Set(ConsoleInterface.Color.Yellow);

            var test = schema.ToArray().Zip(items.ToArray(), (schemaItem, item) =>
            {
                Console.WriteLine($"Pretending {item} is {schemaItem.Type}.");
                return new SurlyAttribute { Value = item };
            });

            ConsoleInterface.Set(ConsoleInterface.Color.Cyan);
            return null;
        }
    }
}