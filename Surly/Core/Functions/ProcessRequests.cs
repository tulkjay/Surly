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
                var resultTable = database.HandleSelect(line);

                if (resultTable)
                    WriteLine("Success", Green);                
                return;
            }
            if (line.ToUpper().Contains("JOIN"))
            {                                
                var success = database.Join(line);

                if (success)
                    WriteLine("Success", Green);                
                return;
            }

            var steps = line.Split(' ').ToList();

            switch (steps[0].ToUpper())
            {
                case "RELATION":
                    database.CreateTable(steps[1].ToUpper(), line);
                    break;

                case "INSERT":
                    var rowAdded = database.AddTuples(steps[1].ToUpper(), line);

                    if (rowAdded)
                        WriteLine($"\tRow added to {steps[1].ToUpper()}", Green);
                    break;

                case "PRINT":
                    Set(Cyan);

                    if (line.ToUpper().Contains("PRINT CATALOG"))
                    {
                        database.PrintCatalog();
                        return;
                    }

                    database.PrintTables(line.Replace(",", "").Replace(";", "").Split(' ').ToList());

                    Set(Magenta);
                    break;

                case "DELETE":
                    var tableDeleted = database.DeleteTable(steps[1].Replace(";", "").ToUpper(), line);

                    if (tableDeleted)
                        WriteLine($"\n\tDeleted {steps[1].ToUpper()}", Green);
                    break;
                
                case "DESTROY":
                    var tableDestroyed = database.DestroyTable(steps[1].Replace(";", "").ToUpper(), line);

                    if (tableDestroyed)
                        WriteLine($"\n\tDestroyed {steps[1].ToUpper()}", Green);
                    break;

                default:
                    WriteLine($"\n\tNot sure about this command: {steps[0].ToUpper()}", Red);
                    break;
            }
        }
    }
}