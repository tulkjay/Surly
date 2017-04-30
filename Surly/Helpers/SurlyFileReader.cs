using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Surly.Core;
using Surly.Core.Functions;
using Surly.Core.Structure;

namespace Surly.Helpers
{
    public class SurlyFileReader
    {
        public SurlyDatabase Database = SurlyDatabase.GetInstance();

        public void ParseFile(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                        ParseLine(reader.ReadLine());
                }
            }
            catch (Exception ex)
            {
                ConsoleInterface.Set(ConsoleColor.Red);

                Console.WriteLine($"Error: {ex.Message}");

                ConsoleInterface.Set(ConsoleColor.Cyan);
            }
        }

        public void ParseLine(string line)
        {
            if (line.Length == 0 || line.Contains("/*")) return;
            Database.ExecuteQuery(line);
        }

        public static void LoadFile()
        {
            var reader = new SurlyFileReader();

            Console.Write("Enter the path to the file or press enter for default.(default): ");
            var path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path) || path == "default")
                path = Constants.Surly2InputFile;

            reader.ParseFile(path);
        }

        //It may not be necessary to create a recursive aggregator.
        public static LinkedList<LinkedList<SurlyTuple>> SurlyZip(LinkedList<SurlyAttributeSchema> schema,
            params object[] items)
        {
            ConsoleInterface.Set(ConsoleColor.Yellow);

            var test = schema.ToArray().Zip(items.ToArray(), (schemaItem, item) =>
            {
                Console.WriteLine($"Pretending {item} is {schemaItem.Type}.");
                return new SurlyAttribute {Value = item};
            });

            ConsoleInterface.Set(ConsoleColor.Cyan);
            return null;
        }
    }
}