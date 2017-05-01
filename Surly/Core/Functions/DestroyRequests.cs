using System.Linq;
using Surly.Core.Structure;
using static System.ConsoleColor;
using static Surly.Helpers.ConsoleInterface;

namespace Surly.Core.Functions
{
    public static class DestroyRequests
    {
        public static void DestroyTable(this SurlyDatabase database, string tableName, string line)
        {
            var tableResponse = database.GetTable(tableName);

            if (tableResponse.IsProjection)
            {
                var projection = SurlyProjections.GetInstance().Projections.Single(x => x.ProjectionName == tableName);
                SurlyProjections.GetInstance().Projections.Remove(projection);
            }
            else
            {
                database.Tables.Remove(tableResponse.Table);
            }

            WriteLine($"\n\tDestroyed {tableName.ToUpper()}", Green);
        }
    }
}