using System.Collections.Generic;

namespace CoreDocker.Shared.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int Count { get; set; }
    }
}