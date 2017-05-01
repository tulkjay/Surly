using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Surly.Core.Structure;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core.Functions
{
    public static class ViewRequests
    {
        private static readonly SurlyProjections Projections = SurlyProjections.GetInstance();

        //In Development...
        public static void CreateView(this SurlyDatabase database, string query)
        {
            WriteLine("The VIEW command is still in development, please try again later.", Yellow);
            return;

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

                var attributeNamesRegex = new Regex("view (.+) from", RegexOptions.IgnoreCase)
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
                    // AttributeNames = attributeNames
                };

                projection = Validate(database, projection);

                if (projection == null) return;

                //Add projection definition
                Projections.Projections.AddLast(projection);

                WriteLine($"\n\tNew view added: {projection.ProjectionName}", Green);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid syntax for VIEW, see help.");
            }
        }
        private static SurlyProjection Validate(SurlyDatabase database, SurlyProjection projection)
        {
            var tableResponse = database.GetTable(projection.TableName);
            if (tableResponse == null) return null;

            var validAttributes = projection.AttributeNames.All(attributeName => tableResponse.Table.Schema.Any(x => x.Name == attributeName.Name));

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

                Write($"\nProjection {projection.ProjectionName.ToUpper()} already exists, enter new projection name: ", Yellow);

                string newProjectionName;
                do
                {
                    newProjectionName = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(newProjectionName)) Write("Please enter a valid projection name: ", Red);

                } while (string.IsNullOrWhiteSpace(newProjectionName));

                projection.ProjectionName = newProjectionName.ToUpper();

                existingProjection = Projections.Projections.Any(x => x.ProjectionName == projection.ProjectionName);
            } while (existingProjection);

            return projection;
        }
    }
}
