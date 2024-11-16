using System.Net;
using Lunarr.Http.Exceptions;

namespace Lunarr.Http.REST
{
    public class UnsupportedMediaTypeException : ApiException
    {
        public UnsupportedMediaTypeException(object content = null)
            : base(HttpStatusCode.UnsupportedMediaType, content)
        {
        }
    }
}
