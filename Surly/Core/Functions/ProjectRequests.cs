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
        private static readonly SurlyDatabase Database = SurlyDatabase.GetInstance();                        
        public static void PrintProjection(this SurlyProjection projection)
        {
            const string id = "Id";

            var table = Database.Tables.SingleOrDefault(x => x.Name == projection.TableName);

            if (table == null)
            {
                WriteLine($"\n\t{projection.ProjectionName} is no longer a valid projection, {projection.TableName} no longer exists.", Red);
                WriteLine($"\n\tRemoving {projection.ProjectionName}.", Green);

                var staleProjection = Projections.Projections.Single(x => x.ProjectionName == projection.ProjectionName);

                Projections.Projections.Remove(staleProjection);
                return;
            }            

            WriteLine($"\n\tProjection: {projection.ProjectionName}\n");

            Console.Write($"  {id.PadRight(8)}");

            var widthReferences = new List<int>();

            var place = 0;
            var attributePlaces = new List<int>();
            foreach (var schema in table.Schema)
            {
                if (!projection.AttributeNames.Contains(schema.Name))
                {
                    place++;
                    continue;
                }

                var tableWidth = Math.Max(schema.Maximum + 2, schema.Name.Length + 2);                

                Console.Write($"{schema.Name.PadRight(tableWidth)}");

                widthReferences.Add(tableWidth);
                attributePlaces.Add(place);
                place++;
            }

            var count = 1;

            WriteLine("\n" + string.Empty.PadRight(100, '='), Green);

            Set(Yellow);

            foreach (var tableTuple in table.Tuples)
            {                
                var index = 0;
                Console.Write($"  {count.ToString().PadRight(8)}");
                var tupleArray = tableTuple.ToArray();                 

                foreach (var location in attributePlaces)
                {                    
                    Console.Write($"{tupleArray[location].Value.ToString().PadRight(widthReferences[index])}");
                    index++;
                }

                Console.WriteLine();

                count++;
            }
            Console.WriteLine();            
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

                WriteLine($"\n\tNew projection added: {projectionName}", Green);
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