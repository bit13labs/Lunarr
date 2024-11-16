using System;
using Microsoft.AspNetCore.Mvc;

namespace Lunarr.Http.REST.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RestPostByIdAttribute : HttpPostAttribute
    {
    }
}
