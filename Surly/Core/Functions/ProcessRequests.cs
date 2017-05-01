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

            if (line.ToUpper().Contains("PROJECT"))
            {
                database.Project(line);
                return;
            }
            if (line.ToUpper().Contains("VIEW"))
            {
                database.CreateView(line);
                return;
            }
            if (line.ToUpper().Contains("SELECT"))
            {
                database.Select(line);
                return;
            }
            if (line.ToUpper().Contains("JOIN"))
            {
                database.Join(line);
                return;
            }
            if (line.ToUpper().Contains("PRINT CATALOG"))
            {
                database.PrintCatalog();
                return;
            }

            var steps = line.Split(' ').ToList();

            switch (steps[0].ToUpper())
            {
                case "RELATION":
                    database.CreateTable(steps[1].ToUpper(), line);
                    break;

                case "INSERT":
                    database.AddTuples(steps[1].ToUpper(), line);                                            
                    break;

                case "PRINT":
                    database.Print(line);
                    break;

                case "DELETE":
                    database.Delete(steps[1], line);
                    break;

                case "DESTROY":
                    database.DestroyTable(steps[1].Replace(";", "").ToUpper(), line);    
                    break;

                default:
                    WriteLine($"\n\tUnknown command: {steps[0].ToUpper()}, please see help for recognized commands", Red);
                    break;
            }
        }
    }
}