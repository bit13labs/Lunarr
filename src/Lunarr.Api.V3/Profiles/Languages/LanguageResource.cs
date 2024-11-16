using System.Text.Json.Serialization;
using Lunarr.Http.REST;

namespace Lunarr.Api.V3.Profiles.Languages
{
    public class LanguageResource : RestResource
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public new int Id { get; set; }
        public string Name { get; set; }
        public string NameLower => Name.ToLowerInvariant();
    }
}
