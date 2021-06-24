using System.Collections.Generic;

namespace Api.Models.Responses
{
    public record ListResponse<TResource>(long Total, IEnumerable<TResource> Data);
}
