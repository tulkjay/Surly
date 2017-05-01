using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Surly.Core.Structure;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core.Functions
{
    public static class JoinRequests
    {
        public static SurlyProjections ProjectionsContainer = SurlyProjections.GetInstance();

        public static void Join(this SurlyDatabase database, string query)
        {
            var projection = CreateJoinProjection(database, query);

            if (projection == null)
            {
                WriteLine("Error adding projection", Red);
                return;
            }

            if (ProjectionsContainer.Projections.Any(x => x.ProjectionName == projection.ProjectionName))
            {
                WriteLine(
                    $"Projection {projection.ProjectionName.ToUpper()} already exists, please try a different name.",
                    Red);
                return;
            }

            ProjectionsContainer.Projections.AddLast(projection);
            WriteLine($"{projection.ProjectionName.ToUpper()} build successful", Green);
        }

        public static SurlyProjection CreateJoinProjection(SurlyDatabase database, string query)
        {
            string projectionName;
            List<string> tableNamesRegex;
            string[] joinCondition;

            try
            {
                var projectionNameRegex = new Regex("(\\w+) =").Match(query);

                projectionName = projectionNameRegex
                    .Groups[1]
                    .Captures[0]
                    .ToString()
                    .ToUpper()
                    .Trim();

                tableNamesRegex = new Regex("join (.+) on", RegexOptions.IgnoreCase)
                    .Match(query)
                    .Groups[1]
                    .Captures[0]
                    .ToString()
                    .ToUpper()
                    .Split(',')
                    .ToList();

                joinCondition = new Regex("on (.+);", RegexOptions.IgnoreCase)
                    .Match(query)
                    .Groups[1]
                    .Captures[0]
                    .ToString()
                    .ToUpper()
                    .Trim()
                    .Split(' ');
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid syntax for JOIN, see help.");
                return null;
            }

            var tableNames = new LinkedList<string>();
            var tables = new List<SurlyTable>();
            var attributeNames = new LinkedList<SurlyAttributeSchema>();
            var resultSet = new LinkedList<LinkedList<SurlyAttribute>>();

            foreach (var tableName in tableNamesRegex)
            {
                tableNames.AddLast(tableName.Trim());

                var tempTableResponse = database.GetTable(tableName.Trim());

                if (tempTableResponse.Table == null)
                {
                    WriteLine($"{tableName.Trim()} not found", Red);
                    return null;
                }                

                attributeNames.Combine(tempTableResponse.Table.Schema);

                tables.Add(tempTableResponse.Table);
            }

            var leftTableRows = tables[0].Tuples.ToList();
            var rightTableRows = tables[1].Tuples;

            leftTableRows.ForEach(
                row => resultSet = resultSet.Combine(row.ApplyCondition(rightTableRows, joinCondition)));

            var projection = new SurlyProjection
            {
                ProjectionName = projectionName.ToUpper(),
                TableName = projectionName.ToUpper(),
                AttributeNames = attributeNames,
                Tuples = resultSet,
                HideIndex = true
            };

            return projection;
        }


        public static LinkedList<T> Combine<T>(this LinkedList<T> baseList, LinkedList<T> newList)
        {
            foreach (var surlyAttribute in newList)
            {
                if (typeof(T) == typeof(SurlyAttribute) || typeof(T) == typeof(SurlyAttributeSchema))
                {
                    if ((surlyAttribute as SurlyAttribute)?.Name == "Id")
                        continue;

                    if ((surlyAttribute as SurlyAttributeSchema)?.Name == "Id")
                        continue;
                }

                baseList.AddLast(surlyAttribute);
            }
            return baseList;
        }

        public static LinkedList<LinkedList<SurlyAttribute>> ApplyCondition( this LinkedList<SurlyAttribute> baseTableRow, 
            LinkedList<LinkedList<SurlyAttribute>> comparingTable,
            string[] condition)
        {
            if (condition.Length != 3)
            {
                WriteLine("Invalid syntax for JOIN condition, please see help", Red);
                return null;
            }

            var tempResult = new LinkedList<LinkedList<SurlyAttribute>>();

            comparingTable.ToList().ForEach(rightRow =>
            {
                if (baseTableRow.First(x => x.Name.ToUpper() == condition[0]).Value
                    .Equals(rightRow.First(x => x.Name.ToUpper() == condition[2]).Value))
                {                    
                    tempResult.AddLast(baseTableRow.Combine(rightRow));
                }                    
            });

            var resultSet = new LinkedList<LinkedList<SurlyAttribute>>();
            var resultAttribute = new LinkedList<SurlyAttribute>();

            foreach (var surlyAttributes in tempResult)
            {
                foreach (var surlyAttribute in surlyAttributes)
                {
                    if (surlyAttribute.Name != "Id")
                    {
                        resultAttribute.AddLast(surlyAttribute);
                    }
                }
                resultSet.AddLast(resultAttribute);
            }

            return resultSet;
        }
    }
}