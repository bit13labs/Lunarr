using NLog;
using NzbDrone.Common.Cloud;
using NzbDrone.Common.Http;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Parser;

namespace NzbDrone.Core.ImportLists.LunarrList2.StevenLu
{
    public class StevenLu2Import : HttpImportListBase<StevenLu2Settings>
    {
        private readonly IHttpRequestBuilderFactory _lunarrMetadata;

        public override string Name => "StevenLu List";

        public override ImportListType ListType => ImportListType.Other;
        public override bool Enabled => true;
        public override bool EnableAuto => false;

        public StevenLu2Import(ILunarrCloudRequestBuilder requestBuilder,
                              IHttpClient httpClient,
                              IImportListStatusService importListStatusService,
                              IConfigService configService,
                              IParsingService parsingService,
                              Logger logger)
        : base(httpClient, importListStatusService, configService, parsingService, logger)
        {
            _lunarrMetadata = requestBuilder.LunarrMetadata;
        }

        public override IImportListRequestGenerator GetRequestGenerator()
        {
            return new StevenLu2RequestGenerator()
            {
                Settings = Settings,
                Logger = _logger,
                HttpClient = _httpClient,
                RequestBuilder = _lunarrMetadata
            };
        }

        public override IParseImportListResponse GetParser()
        {
            return new LunarrList2Parser();
        }
    }
}
