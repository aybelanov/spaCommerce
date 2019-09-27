using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Web.Framework.Server.Localization
{
    // TODO added (likely problem ClearSeoFriendlyUrlsCachedValueForRoutes)
    /// <summary>
    /// Represents extensions of LocalizedEndpoint
    /// </summary>
    public static class LocalizedEndpointExtensions
    {
        /// <summary>
        /// Adds a route to the route builder with the specified name and template
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <returns>Route builder</returns>
        public static IEndpointRouteBuilder MapLocalizedEndpoint(this IEndpointRouteBuilder endpointBuilder, string name, string template)
        {
            return MapLocalizedEndpoint(endpointBuilder, name, template, defaults: null);
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
        public static IEndpointRouteBuilder MapLocalizedEndpoint(this IEndpointRouteBuilder endpointBuilder, string name, string template, object defaults)
        {
            return MapLocalizedEndpoint(endpointBuilder, name, template, defaults, constraints: null);
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
        public static IEndpointRouteBuilder MapLocalizedEndpoint(this IEndpointRouteBuilder endpointBuilder,
            string name, string template, object defaults, object constraints)
        {
            return MapLocalizedEndpoint(endpointBuilder, name, template, defaults, constraints, dataTokens: null);
        }

        /// <summary>
        /// Adds a route to the route builder with the specified name, template, default values, constraints anddata tokens.
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
        public static IEndpointRouteBuilder MapLocalizedEndpoint(this IEndpointRouteBuilder endpointBuilder,
            string name, string template, object defaults, object constraints, object dataTokens)
        {
            //get registered InlineConstraintResolver
            var inlineConstraintResolver = endpointBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();  

            //create new generic route
            endpointBuilder.MapControllerRoute(name, template,
                new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new RouteValueDictionary(dataTokens));


            return endpointBuilder;
        }
    }
}