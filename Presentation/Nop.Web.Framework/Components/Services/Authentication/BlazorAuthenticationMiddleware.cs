using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace spaCommerce
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class BlazorAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public BlazorAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BlazorAuthenticationMiddleware>();
        }
    }
}
