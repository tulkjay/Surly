using System.Linq;
using Surly.Core.Structure;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core.Functions
{
    public static class DeleteRequests
    {
        public static bool DeleteTable(this SurlyDatabase database, string tableName, string line)
        {
            if (database.Tables.All(x => x.Name != tableName))
            {
                WriteLine($"Table {tableName} was not found.", Red);
                return false;
            }

            var table = database.Tables.SingleOrDefault(x => x.Name == tableName);

            return database.Tables.Remove(table);
        }
    }
}