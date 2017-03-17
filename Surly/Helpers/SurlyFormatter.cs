using System;
using System.Linq;

namespace Surly.Helpers
{
    public class SurlyFormatter : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var text = arg.ToString();

            if (format == "insert")
            {
                var temp = text.Split(' ').Skip(2);
                text = string.Join(" ", temp);
            }

            //Additional custom string formatting goes here

            return text.Replace("&#39;", "'");
        }

        public object GetFormat(Type formatType) => formatType == typeof(ICustomFormatter) ? this : null;
    }
}