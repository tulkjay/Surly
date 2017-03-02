using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace Surly.Helpers
{
    public static class Extensions
    {
        public static Type ToSurlyType(this string typeName)
        {
            switch (typeName.ToLower())
            {
                case "char":
                    return typeof(char);

                case "num":
                    return typeof(int);
            }
            return null;
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

                var testLength = substringEnd - substringBegin;

                var quotedString = text.Substring(substringBegin, testLength + 1);

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
    }
}