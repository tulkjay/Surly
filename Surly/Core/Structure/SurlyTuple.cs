using System.Collections.Generic;

namespace Surly.Core.Structure
{
    public class SurlyTuple
    {
        public string Name { get; set; }

        public LinkedList<SurlyAttributeSchema> Attributes { get; set; } = new LinkedList<SurlyAttributeSchema>();
    }
}