using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Surly.Core.Structure;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core.Functions
{
    public static class ProjectRequests
    {
        private static readonly SurlyProjections Projections = SurlyProjections.GetInstance();
                        
        public static SurlyTable BuildProjection(this SurlyProjection projection)
        {
            return null;
        }

        public static void Project(this SurlyDatabase database, string query)
        {
                        
            //If the syntax is wrong, the regex with throw an exception
            try
            {
                var projectionNameRegex = new Regex("(\\w+) =").Match(query);
                
                var projectionName = projectionNameRegex
                    .Groups[1]
                    .Captures[0]
                    .ToString()
                    .ToUpper()
                    .Trim();

                var attributeNamesRegex = new Regex("project (.+) from", RegexOptions.IgnoreCase)
                    .Match(query)
                    .Groups[1]
                    .Captures[0]
                    .ToString()
                    .ToUpper()
                    .Split(',')
                    .ToList();

                var attributeNames = new LinkedList<string>();
                foreach (var name in attributeNamesRegex)
                {
                    attributeNames.AddLast(name.Trim());
                }

                attributeNamesRegex.ForEach(x => WriteLine($"\t{x}"));

                var tableName = new Regex("(\\w+);", RegexOptions.IgnoreCase)
                    .Match(query)
                    .Groups[1]
                    .Captures[0]
                    .ToString()
                    .ToUpper()
                    .Trim();

                //Verify tables/attributes exist                
                var projection = new SurlyProjection
                {
                    ProjectionName = projectionName,
                    TableName = tableName,
                    AttributeNames = attributeNames
                };

                projection = Validate(database, projection);

                if (projection == null) return;

                //Add projection definition
                Projections.Projections.AddLast(projection);

                WriteLine($"\tNew projection added: {projectionName}", Green);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid syntax for Project, see help.");
            }
        }

        private static SurlyProjection Validate(SurlyDatabase database, SurlyProjection projection)
        {
            var table = database.GetTable(projection.TableName);
            if (table == null) return null;

            var validAttributes = projection.AttributeNames.All(attributeName => table.Schema.Any(x => x.Name == attributeName));

            if (!validAttributes)
            {
                WriteLine("\tColumn name(s) not found.\n", Red);
                return null;
            }

            bool existingProjection;
            do
            {
                existingProjection = Projections.Projections.Any(x => x.ProjectionName == projection.ProjectionName);

                //Rename projection
                if (!existingProjection) continue;

                WriteLine("Projection already exists, enter new: ");
                projection.ProjectionName = Console.ReadLine();
            } while (existingProjection);

            return projection;
        }
    }
}