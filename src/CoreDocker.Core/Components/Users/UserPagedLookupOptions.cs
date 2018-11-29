    using CoreDocker.Core.Framework.CommandQuery;

namespace CoreDocker.Core.Components.Users
{
    public class UserPagedLookupOptions : PagedLookupOptionsBase
    {
        public string Search { get; set; }
        public SortOptions? Sort { get; set; }

        public enum SortOptions
        {
            Name,
            Recent
        }
    }
}