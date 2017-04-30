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

        public static bool Join(this SurlyDatabase database, string query)
        {
            var projection = CreateJoinProjection(database, query);

            if (projection == null)
            {
                WriteLine("Error adding projection", Red);
                return false;
            }

            if (ProjectionsContainer.Projections.Any(x => x.ProjectionName == projection.ProjectionName))
            {
                WriteLine($"Projection {projection.ProjectionName.ToUpper()} already exists, please try a different name.", Red);
                return false;
            }

            ProjectionsContainer.Projections.AddLast(projection);

            return true;
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
            catch (Exception )
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
                    
                if (!database.Tables.Select(x => x.Name).Contains(tableName.Trim()))
                {
                    WriteLine($"{tableName.Trim()} not found", Red);
                    return null;
                }
                var tempTable = database.GetTable(tableName.Trim());

                attributeNames.Combine(tempTable.Schema);
                    
                tables.Add(tempTable); 
            }

            var leftTableRows = tables[0].Tuples.ToList();
            var rightTableRows = tables[1].Tuples;

            leftTableRows.ForEach(row => resultSet = resultSet.Combine(row.ApplyCondition(rightTableRows, joinCondition)));                        
             
            var projection = new SurlyProjection
            {
                ProjectionName = projectionName.ToUpper(),
                TableName = projectionName.ToUpper(),
                AttributeNames = attributeNames,
                Tuples = resultSet
            };           

            return projection;
            
        }


        public static LinkedList<T> Combine<T>(this LinkedList<T> baseList, LinkedList<T> newList)
        {
            foreach (var surlyAttribute in newList)
            {
                baseList.AddLast(surlyAttribute);
            }
            return baseList;
        }

        public static LinkedList<LinkedList<SurlyAttribute>> ApplyCondition(this LinkedList<SurlyAttribute> baseTableRow, LinkedList<LinkedList<SurlyAttribute>> comparingTable, string[] condition)
        {
            if (condition.Length != 3)
            {
                WriteLine("Invalid syntax for JOIN condition, please see help", Red);
                return null;
            }

            var result = new LinkedList<LinkedList<SurlyAttribute>>();

            comparingTable.ToList().ForEach(rightRow =>
            {
                //WriteLine($"{baseTableRow.Single(x => x.Name == condition[0]).Value} vs {rightRow.Single(x => x.Name == condition[2]).Value}");
                //WriteLine($"equals {baseTableRow.Single(x => x.Name == condition[0]).Value.Equals(rightRow.Single(x => x.Name == condition[2]).Value)}");
                //WriteLine($"== {baseTableRow.Single(x => x.Name == condition[0]).Value.Equals(rightRow.Single(x => x.Name == condition[2]).Value)}");

                if (baseTableRow.First(x => x.Name == condition[0]).Value.Equals(rightRow.First(x => x.Name == condition[2]).Value))
                {
                    result.AddLast(baseTableRow.Combine(rightRow));
                }
            });            

            return result;
        }
    }
}