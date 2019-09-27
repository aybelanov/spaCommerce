using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Server.Seo
{
    // TODO added
    /// <summary>
    /// Represents extensions of GenericPathRoute
    /// </summary>
    public static class GenericPathEndpointExtensions
    {
        /// <summary>
        /// Adds a route to the route builder with the specified name and template
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <returns>Route builder</returns>
        public static IEndpointRouteBuilder MapGenericPathEndpoint(this IEndpointRouteBuilder routeBuilder, string name, string template)
        {
            return MapGenericPathEndpoint(routeBuilder, name, template, defaults: null);
        }

        /// <summary>
        /// Adds a route to the route builder with the specified name, template, and default values
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <param name="defaults">An object that contains default values for route parameters. 
        /// The object's properties represent the names and values of the default values</param>
        /// <returns>Route builder</returns>
        public static IEndpointRouteBuilder MapGenericPathEndpoint(this IEndpointRouteBuilder routeBuilder, string name, string template, object defaults)
        {
            return MapGenericPathEndpoint(routeBuilder, name, template, defaults, constraints: null);
        }

        /// <summary>
        /// Adds a route to the route builder with the specified name, template, default values, and constraints.
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <param name="defaults"> An object that contains default values for route parameters. 
        /// The object's properties represent the names and values of the default values</param>
        /// <param name="constraints">An object that contains constraints for the route. 
        /// The object's properties represent the names and values of the constraints</param>
        /// <returns>Route builder</returns>
        public static IEndpointRouteBuilder MapGenericPathEndpoint(this IEndpointRouteBuilder routeBuilder,
            string name, string template, object defaults, object constraints)
        {
            return MapGenericPathEndpoint(routeBuilder, name, template, defaults, constraints, dataTokens: null);
        }

        /// <summary>
        /// Adds a route to the route builder with the specified name, template, default values, constraints and data tokens.
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <param name="defaults"> An object that contains default values for route parameters. 
        /// The object's properties represent the names and values of the default values</param>
        /// <param name="constraints">An object that contains constraints for the route. 
        /// The object's properties represent the names and values of the constraints</param>
        /// <param name="dataTokens">An object that contains data tokens for the route. 
        /// The object's properties represent the names and values of the data tokens</param>
        /// <returns>Route builder</returns>
        public static IEndpointRouteBuilder MapGenericPathEndpoint(this IEndpointRouteBuilder endpointBuilder,
            string name, string template, object defaults, object constraints, object dataTokens)
        {
            //if (routeBuilder.DefaultHandler == null)
            //    throw new ArgumentNullException(nameof(routeBuilder));

            //get registered InlineConstraintResolver
            //var inlineConstraintResolver = endpointBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();
            
            
            // TODO added, need developmnet
            //create new generic route
            endpointBuilder.MapControllerRoute(name, template,
                new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new RouteValueDictionary(dataTokens));

            return endpointBuilder;
        }
    }
}