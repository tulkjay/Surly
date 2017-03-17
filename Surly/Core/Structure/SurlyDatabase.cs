using System.Collections.Generic;

namespace Surly.Core.Structure
{
    //This database is a singleton
    public class SurlyDatabase
    {
        private static SurlyDatabase _instance;

        private SurlyDatabase() {}

        public LinkedList<SurlyTable> Tables { get; set; } = new LinkedList<SurlyTable>();

        public static SurlyDatabase GetInstance() => _instance ?? (_instance = new SurlyDatabase());
    }
}