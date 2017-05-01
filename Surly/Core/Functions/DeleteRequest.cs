using System;
using Surly.Core.Structure;
using System.Linq;
using System.Text.RegularExpressions;
using Surly.Helpers;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core.Functions
{
    public static class DeleteRequest
    {
        public static void Delete(this SurlyDatabase database, string tableName, string line)
        {
            if (line.ToUpper().Contains("WHERE"))
            {
                database.DeleteTableWithConditions(tableName, line);
                return;
            }

            database.DeleteTable(tableName.Replace(";", "").ToUpper(), line);
        }

        public static void DeleteTable(this SurlyDatabase database, string tableName, string line)
        {
            var tableResponse = database.GetTable(tableName);

            if (tableResponse.Table == null) return;

            tableResponse.Table.Tuples.Clear();

            WriteLine($"\n\tDeleted {tableName.ToUpper()}", Green);
        }

        public static void DeleteTableWithConditions(this SurlyDatabase database, string tableName, string line)
        {
            var tableResponse = database.GetTable(tableName);

            if (tableResponse.Table == null) return;

            string[] conditions = null;
            try
            {
                conditions = new Regex("where (.+);", RegexOptions.IgnoreCase)
                    .Match(line)
                    .Groups[1]
                    .Captures[0]
                    .ToString()
                    .ToUpper()
                    .Split(' ');
            }
            catch (Exception)
            {
                WriteLine("Invalid syntax, please see help.", Red);
            }

            if (conditions == null) return;
            var success = false;

            tableResponse.Table.Tuples.ToList().ForEach(tableRow =>
            {
                var match = OperatorHelper.Chain(tableRow, true, conditions, 0);

                if (match)
                {
                    tableResponse.Table.Tuples.Remove(tableRow);
                    success = true;
                }
            });

            if (success)
                WriteLine("\n\tSuccess", Green);
            else
                WriteLine("No rows affected.");
        }
    }
}


