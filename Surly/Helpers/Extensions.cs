using System;
using System.Collections.Generic;
using System.Linq;
using Surly.Core.Structure;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Helpers
{
    public static class SurlyExtensions
    {
        public static bool ValidTuple(this SurlyDatabase database, string tableName, string[] parts)
        {
            if (parts.Any(string.IsNullOrWhiteSpace))
            {
                WriteLine("Invalid syntax in schema definition", Red);
                return false;
            }

            var table = database.Tables.SingleOrDefault(x => x.Name == tableName);

            if (table != null && table.Schema
                    .Any(x => x.Name == parts?[0]))
            {
                WriteLine($"{parts[0]} already exists. Please select a different attribute name.", Red);
                return false;
            }

            try
            {
                if (parts[1].ToSurlyType() == null)
                {
                    WriteLine($"{parts[1]} is not a recognized type.", Red);
                    return false;
                }
            }
            catch (Exception)
            {
                if (parts.Length < 2)
                {
                    WriteLine("Invalid syntax, please reference Help section for correct syntax", Red);
                    return false;
                }

                WriteLine($"{parts[1]} is not a recognized type.", Red);
                return false;
            }
            return true;
        }
    }

    public static class Extensions
    {
        public static Type ToSurlyType(this string typeName)
        {
            switch (typeName.ToLower())
            {
                case "char":
                    return typeof(string);

                case "num":
                    return typeof(int);

                default:
                    return null;
            }
        }

        public static string[] SplitValues(this string text, char delimiter = ' ')
        {
            //Parse out strings wrapped with single quotes
            if (text.Contains("'")) return SplitWithStrings(text, delimiter);

            var result = new List<string>();

            text.Split(delimiter)
                .ToList()
                .ForEach(x => result.Add(x.Replace(";", "")));

            var test = result
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            return test;
        }

        private static string[] SplitWithStrings(string text, char delimiter)
        {
            var result = new List<string>();

            while (text.Contains("'"))
            {
                var substringBegin = text.IndexOf("'", StringComparison.Ordinal);
                var substringEnd = text.IndexOf("'", substringBegin + 1, StringComparison.Ordinal);

                var temp = text.Substring(0, substringBegin);

                var quotedString = text.Substring(substringBegin, substringEnd - substringBegin + 1);

                text = text.Replace(quotedString, "");

                quotedString = quotedString.Replace("'", "");

                temp.Split(delimiter).ToList().ForEach(x => result.Add(x));

                result.Add(quotedString);

                text = text.Replace(temp, "");

                text.Split(delimiter)
                    .ToList()
                    .ForEach(x => result.Add(x.Replace(";", "")));
            }

            return result
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }

        public static dynamic To<T>(this T source, Type destination, int max)
        {
            if (destination == typeof(string))
                return source.ToString().Length > max
                    ? StringLengthError(source, max)
                    : Convert.ChangeType(source, destination);

            if (destination == typeof(int))
                return int.Parse(source.ToString()) > max
                    ? IntMaximumError(source, max)
                    : Convert.ChangeType(source, destination);

            return source;
        }

        private static object IntMaximumError(object source, int max)
        {
            int value;
            var valid = int.TryParse(source.ToString(), out value);

            if (valid)
            {
                if (!(value > Math.Pow(10, max))) return value;

                WriteLine($"Truncation warning: Value was {source}, maximum allowed is {max}. Truncating...",
                    Red);
                return Math.Pow(10, max) - 1;
            }

            WriteLine($"{source} is not a valid NUM value.");
            return null;
        }

        private static object StringLengthError(object source, int max)
        {
            WriteLine(
                $"Truncation warning: {source} length was {source.ToString().Length}, maximum allowed is {max}. Truncating...",
                Red);
            return source.ToString().Substring(0, max);
        }
    }
}