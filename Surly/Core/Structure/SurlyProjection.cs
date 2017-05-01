using System.Collections.Generic;

namespace Surly.Core.Structure
{
    public class SurlyProjection
    {
        public string ProjectionName { get; set; }
        public LinkedList<SurlyAttributeSchema> AttributeNames { get; set; }
        public string TableName { get; set; }
        public LinkedList<LinkedList<SurlyAttribute>> Tuples { get; set; }

        public bool HideIndex { get; set; }
    }
}