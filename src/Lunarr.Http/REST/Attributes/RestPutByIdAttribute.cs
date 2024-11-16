using System;
using Microsoft.AspNetCore.Mvc;

namespace Lunarr.Http.REST.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RestPutByIdAttribute : HttpPutAttribute
    {
        public RestPutByIdAttribute()
            : base("{id:int?}")
        {
        }
    }
}
