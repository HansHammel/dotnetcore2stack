using System.Collections.Generic;

namespace RefArc.Api.HATEOAS.Lib
{
    public class LinksWrapperList<T>
    {
        public List<LinksWrapper<T>> Values { get; set; }
        public List<LinkInfo> Links { get; set; }
    }
}
