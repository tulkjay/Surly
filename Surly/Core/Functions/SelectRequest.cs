using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Surly.Core.Structure;

namespace Surly.Core.Functions
{
    public static class SelectRequest
    {
        public static bool HandleSelect(this SurlyDatabase database, string query)
        {
            var steps = query.ToUpper().Split(' ').ToList();
            var selectNameRegex = new Regex("(\\w+) =").Match(query);

            var selectName = selectNameRegex
                .Groups[1]
                .Captures[0]
                .ToString()
                .ToUpper()
                .Trim();

            var attributeNamesRegex = new Regex("select (.+) from", RegexOptions.IgnoreCase)
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

            return false;
        }

        private static SurlyTable Handle(this SurlyTable resultSet, IEnumerable<string> query)
        {
            return null;
        }
    }
}
