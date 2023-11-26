using System.Collections.Generic;
using System.Linq;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public class PagedList<T>
    {
        public PagedList(IQueryable<T> queryable, PagedLookupOptionsBase optionsIncludeCount)
        {
            Items = optionsIncludeCount.First == 0
                ? new List<T>()
                : queryable.Take(optionsIncludeCount.First).ToList();
            Count = -1;
            if (optionsIncludeCount.IncludeCount)
            {
                Count = Items.Count == optionsIncludeCount.First ? queryable.LongCount() : Items.Count;
            }
        }

        public long Count { get; set; }

        public List<T> Items { get; set; }
    }
}