using System;
using System.Collections.Generic;
using System.Net;
using FluentValidation.Results;
using Newtonsoft.Json;
using NLog;
using NzbDrone.Common.Extensions;
using NzbDrone.Common.Http;

namespace NzbDrone.Core.ImportLists.Lunarr
{
    public interface ILunarrV3Proxy
    {
        List<LunarrMovie> GetMovies(LunarrSettings settings);
        List<LunarrProfile> GetProfiles(LunarrSettings settings);
        List<LunarrTag> GetTags(LunarrSettings settings);
        ValidationFailure Test(LunarrSettings settings);
    }

    public class LunarrV3Proxy : ILunarrV3Proxy
    {
        private readonly IHttpClient _httpClient;
        private readonly Logger _logger;

        public LunarrV3Proxy(IHttpClient httpClient, Logger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public List<LunarrMovie> GetMovies(LunarrSettings settings)
        {
            return Execute<LunarrMovie>("/api/v3/movie", settings);
        }

        public List<LunarrProfile> GetProfiles(LunarrSettings settings)
        {
            return Execute<LunarrProfile>("/api/v3/qualityprofile", settings);
        }

        public List<LunarrTag> GetTags(LunarrSettings settings)
        {
            return Execute<LunarrTag>("/api/v3/tag", settings);
        }

        public ValidationFailure Test(LunarrSettings settings)
        {
            try
            {
                GetMovies(settings);
            }
            catch (HttpException ex)
            {
                if (ex.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.Error(ex, "API Key is invalid");
                    return new ValidationFailure("ApiKey", "API Key is invalid");
                }

                _logger.Error(ex, "Unable to send test message");
                return new ValidationFailure("ApiKey", "Unable to send test message");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to send test message");
                return new ValidationFailure("", "Unable to send test message");
            }

            return null;
        }

        private List<TResource> Execute<TResource>(string resource, LunarrSettings settings)
        {
            if (settings.BaseUrl.IsNullOrWhiteSpace() || settings.ApiKey.IsNullOrWhiteSpace())
            {
                return new List<TResource>();
            }

            var baseUrl = settings.BaseUrl.TrimEnd('/');

            var request = new HttpRequestBuilder(baseUrl).Resource(resource).Accept(HttpAccept.Json)
                .SetHeader("X-Api-Key", settings.ApiKey).Build();

            var response = _httpClient.Get(request);

            var results = JsonConvert.DeserializeObject<List<TResource>>(response.Content);

            return results;
        }
    }
}
