using Surly.Core.Structure;
using System.Linq;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;


namespace Surly.Core.Functions
{
    public static class DestroyRequest
    {
        public static bool DestroyTable(this SurlyDatabase database, string tableName, string line)
        {
            var table = database.Tables.SingleOrDefault(x => x.Name == tableName);

            if (table == null)
            {
                WriteLine($"\n\t{tableName} was not found.", Red);
                return false;
            }            

            table.Tuples.Clear();

            return true;
        }
    }
}


