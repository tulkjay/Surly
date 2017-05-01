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
    public static class SelectRequest
    {
        private static LinkedList<LinkedList<SurlyAttribute>> _resultSet;

        public static void Select(this SurlyDatabase database, string query)
        {
            _resultSet = new LinkedList<LinkedList<SurlyAttribute>>();
            string tableName, conditions, projectionName = null;
            var printProjection = false;
            try
            {
                try
                {
                    projectionName = new Regex("(\\w+) = select", RegexOptions.IgnoreCase)
                        .Match(query)
                        .Groups[1]
                        .Captures[0]
                        .ToString()
                        .ToUpper();
                }
                catch (Exception)
                {
                    printProjection = true;
                }

                tableName = new Regex("select (\\w+) where", RegexOptions.IgnoreCase)
                    .Match(query)
                    .Groups[1]
                    .Captures[0]
                    .ToString()
                    .ToUpper();

                conditions = new Regex("where (.+);", RegexOptions.IgnoreCase)
                    .Match(query)
                    .Groups[1]
                    .Captures[0]
                    .ToString()
                    .ToUpper();

            }
            catch (Exception)
            {
                WriteLine("Invalid SELECT syntax, please see help", Red);
                return;
            }

            var tableResponse = database.GetTable(tableName);

            if (tableResponse.Table == null)
            {
                WriteLine($"{tableName.ToUpper()} not found.", Red);
                return;
            }

            if (printProjection
                && SurlyProjections.GetInstance().Projections.Any(x => x.ProjectionName == projectionName))
            {
                WriteLine($"\n\t{projectionName?.ToUpper()} already exists, please choose a different name", Red);
                return;
            }

            var conditionSteps = conditions.Split(' ').ToList();

            tableResponse.Table.Tuples.ToList().ForEach(tableRow =>
            {
                var valid = Chain(tableRow, true, conditionSteps.ToArray(), 0);

                if (valid)
                {
                    var trimmedRow = new LinkedList<SurlyAttribute>(tableRow);

                    var rowId = trimmedRow.Single(x => x.Name == "Id");
                    trimmedRow.Remove(rowId);

                    _resultSet.AddLast(trimmedRow);
                }
            });

            if (_resultSet.Count == 0)
            {
                WriteLine("\n\tQuery yielded no results.", Yellow);
                return;
            }

            var schema = new LinkedList<SurlyAttributeSchema>(tableResponse.Table.Schema);
            var id = schema.Single(x => x.Name == "Id");
            schema.Remove(id);

            if (printProjection)
            {
                var response = new SurlyTableResponse
                {
                    Table = new SurlyTable
                    {
                        Schema = schema,
                        Name = "Results",
                        Tuples = _resultSet
                    },
                    HideIndexes = true
                };

                database.PrintTables(new List<SurlyTableResponse> {response});

                return;
            }

            SurlyProjections.GetInstance().Projections.AddLast(new SurlyProjection
            {
                AttributeNames = schema,
                HideIndex = true,
                ProjectionName = projectionName,
                TableName = tableResponse.Table.Name,
                Tuples = _resultSet
            });

            WriteLine($"\n\t{projectionName.ToUpper()} build successful.", Green);
        }

        public static bool Chain(LinkedList<SurlyAttribute> row, bool previousValid, string[] conditionSet, int index)
        {
            string attribute;
            var result = false;
            if (conditionSet.Length > index + 4)
            {
                result = Chain(row, previousValid, conditionSet, index + 4);
            }
            else
            {
                attribute = row.Single(x => x.Name.ToUpper() == conditionSet[index]).Value.ToString();

                return OperatorHelper.ApplyCondition(attribute, conditionSet[index + 1], conditionSet[index + 2]);
            }
            attribute = row.Single(x => x.Name.ToUpper() == conditionSet[index]).Value.ToString();

            var valid = OperatorHelper.ApplyCondition(attribute, conditionSet[index + 1], conditionSet[index + 2]);

            switch (conditionSet[index + 3].ToUpper())
            {
                case "AND":
                    valid = result && valid;
                    break;
                case "OR":
                    valid = result || valid;
                    break;
                default:
                    WriteLine("Invalid operation, please see help.", Red);
                    return false;
            }
            return valid;              
        }
    }
}
