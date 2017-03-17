using Surly.Core.Structure;

namespace Surly.Core.Functions
{
    public static class ProjectRequests
    {
        public static void Project(this SurlyDatabase database, string query)
        {
            var projections = SurlyProjections.GetInstance();
            
            //Parse query
            var projectionName = query;
            
            //Get projection name and verify tables exist
            
            
            projections.Projections.AddLast(new SurlyProjection
            {
                ProjectionName = projectionName                
            });
                        
            //Add Projection to projections
            //Print results
        }
    }
}