using System.Collections.Generic;

namespace Api.ION
{
    public class PagedResults<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalSize { get; set; }
    }
}