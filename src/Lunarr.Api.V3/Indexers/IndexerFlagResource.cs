using Newtonsoft.Json;
using Lunarr.Http.REST;

namespace Lunarr.Api.V3.Indexers
{
    public class IndexerFlagResource : RestResource
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public new int Id { get; set; }
        public string Name { get; set; }
        public string NameLower => Name.ToLowerInvariant();
    }
}
