using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Infrastructure;
using System;

namespace Nop.Web.Framework.Components.Extensions
{
    // ADDED WORKAROUND it's a replace for methods IUrlHelper for non MVC app like Blazor 
    /// <summary>
    /// Analog for IUrlHelper
    /// </summary>
    public static partial class NopLinkGeneratorExtension
    {
        /// <summary>
        /// Equally to IUrlHelper.Content
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string Content(this LinkGenerator generator, string contentPath)
        {
            if (string.IsNullOrEmpty(contentPath))
            {
                return null;
            }
            else if (contentPath[0] == '~')
            {
                var segment = new PathString(contentPath.Substring(1));
                var _httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();
                var applicationPath = new PathString(""); //_httpContextAccessor.HttpContext.Request.PathBase;
                return applicationPath.Add(segment).Value;
            }

            return contentPath;
        }

        public static string RouteUrl(this LinkGenerator generator, string routeName, object values = null,
            PathString pathBase = default, FragmentString fragment = default, LinkOptions options = null)
        {        
            //var context = EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext;
            return generator.GetPathByRouteValues(routeName, values, pathBase, fragment, options);
        }

        public static string RouteUrl(this LinkGenerator generator, HttpContext httpContext, string routeName, object values = null,
            PathString pathBase = default, FragmentString fragment = default, LinkOptions options = null)
        {
            
            var resultRoute = generator.GetPathByRouteValues(routeName, values, pathBase, fragment, options);
            if (resultRoute == null)
            {
                if (values is string)
                {
                    resultRoute = httpContext.Request.PathBase.Value + values;
                }
                else if (values != null)
                {
                    resultRoute = httpContext.Request.PathBase.Value;
                    var routeValues = values.GetType().GetProperties();
                    foreach (var prop in routeValues)
                        resultRoute += prop.GetValue(values).ToString();
                }
            }
            return resultRoute;
        }
    }
}

