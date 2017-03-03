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
                WriteLine($"  {idHeader.PadRight(5)}{nameHeader.PadRight(20)}{typeHeader.PadRight(20)}{maxHeader}",
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

        public void PrintAll()
        {
            const string id = "Id";

            WriteLine("\n\n\t*** FULL DATABASE ***\n");
            if (Tables.Count == 0) WriteLine("\t<--EMPTY-->\n\n", Red);

            foreach (var table in Tables)
            {
                WriteLine($"\n\tTable: {table.Name}");
                Console.WriteLine();

                Console.Write($"  {id.PadRight(8)}");

                foreach (var schema in table.Schema)
                    Console.Write($"{schema.Name.PadRight(12)}");

                var count = 1;

                WriteLine("\n" + string.Empty.PadRight(100, '='), Green);

                Set(Yellow);

                foreach (var tableTuple in table.Tuples)
                {
                    Console.Write($"  {count.ToString().PadRight(8)}");

                    foreach (var attribute in tableTuple)
                        Console.Write($"{attribute.Value.ToString().PadRight(12)}");

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
            switch (steps[0].ToLower())
            {
                case "relation":
                    Console.WriteLine("Creating table: " + steps[1] + "\n");

                    Set(Cyan);

                    CreateTable(steps[1], line);

                    Set(Magenta);
                    break;

                case "insert":
                    Console.WriteLine("\nAdding tuple(s) to " + steps[1]);

                    Set(Cyan);

                    AddTuples(steps[1], line);

                    Set(Magenta);
                    break;

                case "print":
                    Set(Cyan);

                    Print(line.Replace(",", "").Split(' ').ToList());

                    Set(Magenta);
                    break;

                default:
                    Set(Red);

                    Console.WriteLine("Not sure about this command: " + steps[0]);

                    Set(Cyan);
                    break;
            }
        }

        private void Print(IList<string> steps)
        {
            const string id = "Id";

            steps.RemoveAt(0);

            var tables = steps
                .Select(step => Tables
                    .SingleOrDefault(x => x.Name == step))
                .ToList();

            foreach (var table in tables)
            {
                if (table == null) continue;

                WriteLine($"\n\tTable: {table.Name}");
                Console.WriteLine();

                Console.Write($"  {id.PadRight(8)}");

                foreach (var schema in table.Schema)
                    Console.Write($"{schema.Name.PadRight(12)}");

                var count = 1;

                WriteLine("\n" + string.Empty.PadRight(100, '='), Green);

                Set(Yellow);

                foreach (var tableTuple in table.Tuples)
                {
                    Console.Write($"  {count.ToString().PadRight(8)}");

                    foreach (var attribute in tableTuple)
                        Console.Write($"{attribute.Value.ToString().PadRight(12)}");

                    Console.WriteLine();

                    count++;
                }
                Console.WriteLine();
            }
        }

        private void CreateTable(string tableName, string line)
        {
            Set(Yellow);

            Tables.AddLast(new SurlyTable(tableName));

            var tuples = line
                .Substring(line.IndexOf("(", StringComparison.Ordinal) + 1,
                    line.IndexOf(")", StringComparison.Ordinal) - line.IndexOf("(", StringComparison.Ordinal) - 1);

            var test = tuples.Split(',');

            Set(Green);

            foreach (var tuple in test)
            {
                var parts = tuple.Trim().Split(' ');
                int numMax;

                Tables.Last.Value.Schema.AddLast(new SurlyAttributeSchema
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
            var table = Tables.Single(x => x.Name == tableName);
            var schema = table.Schema.ToArray();

            WriteRow(tuples.ToList(), "\tParsed: ", Blue);

            var newTuple = new LinkedList<SurlyAttribute>();

            for (var i = 0; i < schema.Length; i++)
                newTuple.AddLast(new SurlyAttribute { Value = tuples[i].To(schema[i].Type, schema[i].Maximum) });

            if (newTuple.Count > 0) table.Tuples.AddLast(newTuple);
        }
    }
}