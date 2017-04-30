using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Surly.Core.Structure;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Helpers
{
    public static class OperatorHelper
    {
        public static bool RunCondition(LinkedList<SurlyAttribute> leftTuples, LinkedList<SurlyAttribute> rightTuples, string condition)
        {
            var parts = condition.Split(' ').ToList();
            if (parts.Count != 3)
            {
                Write($"{condition.ToUpper()} is not a recognized comparison, see help section for comparisons", Red);
                return false;
            }
            var leftValue = leftTuples.Single(x => x.Name == parts[0]).Value;
            var rightValue = rightTuples.Single(x => x.Name == parts[0]).Value;
            switch (parts[1])
            {
                case "=":
                    return parts[0] == parts[2];

                case "!=":
                    return parts[0] != parts[2];

                case ">":
                    int greaterThanLeft;
                    var validGtLeft = int.TryParse(parts[0], out greaterThanLeft);
                    int greaterThanRight;
                    var validGtRight = int.TryParse(parts[2], out greaterThanRight);

                    if (!validGtLeft || !validGtRight)
                    {
                        WriteLine($"Invalid type comparison", Red);
                        return false;
                    }
                                        
                    return greaterThanLeft > greaterThanRight;

                case "<":
                    int lessThanLeft;
                    var validLtLeft = int.TryParse(parts[0], out lessThanLeft);
                    int lessThanRight;
                    var validLtRight = int.TryParse(parts[2], out lessThanRight);

                    if (!validLtLeft || !validLtRight)
                    {
                        WriteLine($"Invalid type comparison", Red);
                        return false;
                    }

                    return lessThanLeft < lessThanRight;

                default:
                    return false;
            }
        }
    }
}
