using System.Collections.Generic;

namespace FilterExtensions.Filters
{
    public class Filter
    {
        public string Property { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public string Logic { get; set; }
        public IEnumerable<Filter> Filters { get; set; }
    }
}