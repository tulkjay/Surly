using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Surly.Core;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Helpers
{
    public class SurlyFileReader
    {
        public SurlyDatabase Database = SurlyDatabase.GetInstance();

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
                Set(Color.Red);

                Console.WriteLine($"Error: {ex.Message}");

                Set(Color.Cyan);
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

            Set(Color.Cyan);
            switch (steps[0].ToLower())
            {
                case "relation":
                    Console.WriteLine("Creating table: " + steps[1] + "\n");
                    Set(Color.Cyan);
                    CreateTable(steps[1], line);
                    Set(Color.Magenta);
                    break;

                case "insert":
                    Console.WriteLine("Adding tuple to " + steps[1]);
                    Set(Color.Cyan);

                    AddTuples(steps[1], line);
                    Set(Color.Magenta);
                    break;

                case "print":
                    Set(Color.Cyan);
                    Database.PrintCatalog();
                    Set(Color.Magenta);
                    break;

                default:
                    Set(Color.Red);

                    Console.WriteLine("Not sure about this one: " + steps[0]);

                    Set(Color.Cyan);
                    break;
            }
        }

        private void CreateTable(string tableName, string line)
        {
            Set(Color.Yellow);

            Console.WriteLine($"About to add {tableName} using {line}\n");

            Database.Tables.AddLast(new SurlyTable(tableName));

            var tuples = line
                .Substring(line.IndexOf("(", StringComparison.Ordinal) + 1,
                    (line.IndexOf(")", StringComparison.Ordinal) - line.IndexOf("(", StringComparison.Ordinal) - 1));

            Console.WriteLine($"Tuples: {tuples}\n");

            var test = tuples.Split(',');

            Set(Color.Green);

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

        //TODO - Get this data saved in the table
        private void AddTuples(string tableName, string line)
        {
            var tuples = string.Format(new SurlyFormatter(), "{0:insert}", line).SplitValues();
            var table = Database.Tables.Single(x => x.Name == tableName);
            var schema = table.Schema;

            Set(Color.Blue);
            Console.Write("Parsed: ");
            tuples.ToList().ForEach(x => Console.Write($"{x} "));
            Console.WriteLine();

            var test = schema.Zip(tuples, (schemaItem, tuple) =>
            {
                Console.WriteLine($"Trying to save {tuple} in {schemaItem.Name} as {schemaItem.Type}, which has a max of {schemaItem.Maximum}.");
                var item = new SurlyAttribute { Value = tuple };
                return item;
            });

            //table.Tuples = SurlyZip(schema, tuples);    //May not be necessary to create a recursive aggregator.
        }

        public static LinkedList<LinkedList<SurlyTuple>> SurlyZip(LinkedList<SurlyAttributeSchema> schema, params object[] items)
        {
            Set(Color.Yellow);

            var test = schema.ToArray().Zip(items.ToArray(), (schemaItem, item) =>
            {
                Console.WriteLine($"Pretending {item} is {schemaItem.Type}.");
                return new SurlyAttribute { Value = item };
            });

            Set(Color.Cyan);
            return null;
        }
    }
}