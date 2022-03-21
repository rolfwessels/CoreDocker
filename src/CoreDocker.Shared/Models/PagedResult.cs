using System.Collections.Generic;

namespace CoreDocker.Shared.Models
{
    public record PagedResult<T>(long Count, List<T> Items);

}