using System;
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
        public static bool Chain(LinkedList<SurlyAttribute> row, bool previousValid, string[] conditionSet, int index)
        {
            string attribute;
            bool result;

            if (conditionSet.Length > index + 4)
            {
                result = Chain(row, previousValid, conditionSet, index + 4);
            }
            else
            {
                attribute = row.Single(x => x.Name.ToUpper() == conditionSet[index]).Value.ToString();

                return OperatorHelper.ApplyCondition(attribute, conditionSet[index + 1], conditionSet[index + 2]);
            }

            attribute = row.Single(x => x.Name.ToUpper() == conditionSet[index]).Value.ToString();

            var valid = OperatorHelper.ApplyCondition(attribute, conditionSet[index + 1], conditionSet[index + 2]);

            switch (conditionSet[index + 3].ToUpper())
            {
                case "AND":
                    valid = result && valid;
                    break;
                case "OR":
                    valid = result || valid;
                    break;
                default:
                    WriteLine("Invalid operation, please see help.", Red);
                    return false;
            }

            return valid;
        }

        public static bool ApplyCondition(string leftOperand, string oprtr, string rightOperand)
        {
            switch (oprtr)
            {
                case "=":
                    return leftOperand == rightOperand;

                case "!=":
                    return leftOperand != rightOperand;

                case ">":
                    int greaterThanLeft;
                    var validGtLeft = Int32.TryParse(leftOperand, out greaterThanLeft);
                    int greaterThanRight;
                    var validGtRight = Int32.TryParse(rightOperand, out greaterThanRight);

                    if (!validGtLeft || !validGtRight)
                    {
                        WriteLine($"Invalid type comparison", Red);
                        return false;
                    }

                    return greaterThanLeft > greaterThanRight;

                case "<":
                    int lessThanLeft;
                    var validLtLeft = Int32.TryParse(leftOperand, out lessThanLeft);
                    int lessThanRight;
                    var validLtRight = Int32.TryParse(rightOperand, out lessThanRight);

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

            return ApplyCondition(parts[0], parts[1], parts[2]);
        }
    }
}
