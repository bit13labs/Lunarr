using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NzbDrone.Core.ImportLists.LunarrList
{
    public class LunarrListException : Exception
    {
        public LunarrErrors APIErrors;

        public LunarrListException(LunarrErrors apiError)
            : base(HumanReadable(apiError))
        {
            APIErrors = apiError;
        }

        private static string HumanReadable(LunarrErrors apiErrors)
        {
            var firstError = apiErrors.Errors.First();
            var details = string.Join("\n", apiErrors.Errors.Select(error =>
            {
                return $"{error.Title} ({error.Status}, RayId: {error.RayId}), Details: {error.Detail}";
            }));
            return $"Error while calling api: {firstError.Title}\nFull error(s): {details}";
        }
    }

    public class LunarrError
    {
        [JsonProperty("id")]
        public string RayId { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }
    }

    public class LunarrErrors
    {
        [JsonProperty("errors")]
        public IList<LunarrError> Errors { get; set; }
    }
}
