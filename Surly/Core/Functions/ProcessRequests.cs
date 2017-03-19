using System.Linq;
using Surly.Core.Structure;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core.Functions
{
    public static class ProcessRequests
    {
        public static void ExecuteQuery(this SurlyDatabase database, string line)
        {
            Set(Cyan);
            if (line.Contains("="))
            {
                database.Project(line);
                return;
            }

            var steps = line.Split(' ').ToList();

            switch (steps[0].ToUpper())
            {
                case "RELATION":
                    WriteLine($"Creating table: {steps[1]}", Cyan);
                    database.CreateTable(steps[1].ToUpper(), line);
                    break;

                case "INSERT":
                    var rowAdded = database.AddTuples(steps[1].ToUpper(), line);

                    if (rowAdded)
                        WriteLine($"Row added to {steps[1].ToUpper()}", Green);
                    break;

                case "PRINT":
                    Set(Cyan);

                    database.PrintTables(line.Replace(",", "").Split(' ').ToList());

                    Set(Magenta);
                    break;

                case "DELETE":
                    var tableDeleted = database.DeleteTable(steps[1].ToUpper(), line);
                    if (tableDeleted)
                        WriteLine($"Deleted Table Named {steps[1].ToUpper()}", Green);
                    break;

                default:
                    WriteLine($"Not sure about this command: {steps[0].ToUpper()}", Red);
                    break;
            }
        }
    }
}