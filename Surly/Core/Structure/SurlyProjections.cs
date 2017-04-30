using System.Collections.Generic;

namespace Surly.Core.Structure
{
    //Projections are held in a singleton class
    public class SurlyProjections
    {
        private static SurlyProjections _instance;
        public LinkedList<SurlyProjection> Projections { get; set; } = new LinkedList<SurlyProjection>();

        private SurlyProjections() {}

        public static SurlyProjections GetInstance() => _instance ?? (_instance = new SurlyProjections());        
    }
}
