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

        public static void Project(this SurlyDatabase database, string query)
        {
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
                    AttributeNames = new LinkedList<SurlyAttributeSchema>(),
                    Tuples = new LinkedList<LinkedList<SurlyAttribute>>()
                };

                projection = Validate(database, projection);

                //Clone selected data to new projection
                var schemaDefinition = new LinkedList<SurlyAttributeSchema>();
                var castedList = new LinkedList<LinkedList<SurlyAttribute>>();

                var table = database.GetTable(tableName);

                foreach (var attributeName in attributeNames)
                {
                    var selectedTuplesSchemata = table.Schema.Single(x => x.Name == attributeName);

                    schemaDefinition.AddLast(new SurlyAttributeSchema
                    {
                        Maximum = selectedTuplesSchemata.Maximum,
                        Name = selectedTuplesSchemata.Name
                    });
                }

                var selectedTuples = table.Tuples
                    .Select(x => x
                        .Where(y => attributeNames
                            .Any(a => a == y.Name)));

                foreach (var tupleList in selectedTuples)
                {
                    var list = new LinkedList<SurlyAttribute>();

                    foreach (var attribute in tupleList)
                    {
                        list.AddLast(attribute);
                    }
                    castedList.AddLast(list);
                }

                projection.AttributeNames = schemaDefinition;
                projection.Tuples = castedList;

                //Add projection
                Projections.Projections.AddLast(projection);

                WriteLine($"\n\tNew projection added: {projection.ProjectionName}", Green);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid syntax for PROJECT, see help.");
            }
        }

        public static SurlyProjection Validate(SurlyDatabase database, SurlyProjection projection)
        {
            var table = database.GetTable(projection.TableName);
            if (table == null) return null;

            var validAttributes =
                projection.AttributeNames.All(attributeName => table.Schema.Any(x => x.Name == attributeName.Name));

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

                Write(
                    $"\nProjection {projection.ProjectionName.ToUpper()} already exists, enter new projection name: ",
                    Yellow);

                string newProjectionName;
                do
                {
                    newProjectionName = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(newProjectionName))
                        Write("Please enter a valid projection name: ", Red);
                } while (string.IsNullOrWhiteSpace(newProjectionName));

                projection.ProjectionName = newProjectionName.ToUpper();

                existingProjection = Projections.Projections.Any(x => x.ProjectionName == projection.ProjectionName);
            } while (existingProjection);

            return projection;
        }
    }
}