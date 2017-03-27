using System.Collections.Generic;

namespace Surly.Core
{
    public class SurlyTable
    {
        public string Name { get; set; }

        public LinkedList<SurlyAttributeSchema> Schema { get; set; } = new LinkedList<SurlyAttributeSchema>();

        public LinkedList<LinkedList<SurlyAttribute>> Tuples { get; set; } = new LinkedList<LinkedList<SurlyAttribute>>();

        public SurlyTable(string name = null, LinkedList<LinkedList<SurlyAttribute>> tuples = null)
        {
            if (name != null) Name = name;
            if (tuples != null) Tuples = tuples;
        }
    }
}