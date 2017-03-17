namespace Surly.Core.Structure
{
    public class SurlyAttribute<T>
    {
        public T Value { get; set; }
    }

    public class SurlyAttribute
    {
        public object Value { get; set; }
    }
}