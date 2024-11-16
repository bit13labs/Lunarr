using System;
using Lunarr.Http.REST;

namespace Lunarr.Api.V3.Logs
{
    public class LogFileResource : RestResource
    {
        public string Filename { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string ContentsUrl { get; set; }
        public string DownloadUrl { get; set; }
    }
}
