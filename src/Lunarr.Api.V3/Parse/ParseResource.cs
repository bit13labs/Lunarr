using NzbDrone.Core.Parser.Model;
using Lunarr.Api.V3.Movies;
using Lunarr.Http.REST;

namespace Lunarr.Api.V3.Parse
{
    public class ParseResource : RestResource
    {
        public string Title { get; set; }
        public ParsedMovieInfo ParsedMovieInfo { get; set; }
        public MovieResource Movie { get; set; }
    }
}
