using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Surly.Core.Structure;
using Surly.Helpers;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core.Functions
{
    public static class CreateRequests
    {
        public static bool AddTuples(this SurlyDatabase database, string tableName, string line)
        {
            if (database.Tables.All(x => x.Name != tableName))
            {
                WriteLine($"Table {tableName.ToUpper()} was not found.", Red);
                return false;
            }

            var tuples = string.Format(new SurlyFormatter(), "{0:insert}", line).SplitValues();
            var table = database.Tables.Single(x => x.Name == tableName);
            var schema = table.Schema.ToArray();

            var newTuple = new LinkedList<SurlyAttribute>();

            for (var i = 0; i < schema.Length; i++)
                newTuple.AddLast(new SurlyAttribute {Value = tuples[i].To(schema[i].Type, schema[i].Maximum)});

            if (newTuple.Count <= 0) return false;

            table.Tuples.AddLast(newTuple);

            return true;
        }


        public static void CreateTable(this SurlyDatabase database, string tableName, string line)
        {
            Set(Yellow);

            if (database.Tables.Any(x => x.Name == tableName))
            {
                WriteLine($"Table {tableName} already exists. Please select a different table name.", Red);
                return;
            }

            var tuples = string.Empty;

            try
            {
                tuples = line.Substring(line.IndexOf("(", StringComparison.Ordinal) + 1,
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

                if (!database.ValidTuple(tableName, parts)) return;

                if (!tableCreated)
                {
                    database.Tables.AddLast(new SurlyTable(tableName));
                    tableCreated = true;
                }

                database.Tables.Last.Value.Schema.AddLast(new SurlyAttributeSchema
                {
                    Name = parts[0],
                    Type = parts[1].ToSurlyType(),
                    Maximum = int.TryParse(parts[2], out numMax) ? numMax : 0
                });
            }

            Console.WriteLine();
        }
    }
}