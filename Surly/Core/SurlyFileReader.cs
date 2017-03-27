using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core
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
                    {
                        ParseLine(reader.ReadLine());
                    }
                }
            }
            catch (Exception ex)
            {
                Set(Red);

                Console.WriteLine($"Error: {ex.Message}");

                Set(Cyan);
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

            if (String.IsNullOrWhiteSpace(path) || path == "default")
                path = Constants.SurlyInputFile;

            reader.ParseFile(path);
        }

        //It may not be necessary to create a recursive aggregator.
        public static LinkedList<LinkedList<SurlyTuple>> SurlyZip(LinkedList<SurlyAttributeSchema> schema, params object[] items)
        {
            Set(Yellow);

            var test = schema.ToArray().Zip(items.ToArray(), (schemaItem, item) =>
            {
                Console.WriteLine($"Pretending {item} is {schemaItem.Type}.");
                return new SurlyAttribute { Value = item };
            });

            Set(Cyan);
            return null;
        }
    }
}