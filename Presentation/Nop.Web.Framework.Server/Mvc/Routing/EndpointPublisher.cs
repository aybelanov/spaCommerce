using Microsoft.AspNetCore.Routing;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Web.Framework.Server.Mvc.Routing
{
    // TODO added
    /// <summary>
    /// Represents implementation of route publisher
    /// </summary>
    public class EndpointPublisher : IEndpointPublisher
    {
        #region Fields

        /// <summary>
        /// Type finder
        /// </summary>
        protected readonly ITypeFinder typeFinder;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="typeFinder">Type finder</param>
        public EndpointPublisher(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointBuilder">Route builder</param>
        public virtual void RegisterRoutes(IEndpointRouteBuilder endpointBuilder)
        {
            //find route providers provided by other assemblies
            var routeProviders = typeFinder.FindClassesOfType<IEndpointProvider>();

            //create and sort instances of route providers
            var instances = routeProviders
                .Where(routeProvider => PluginManager.FindPlugin(routeProvider)?.Installed ?? true) //ignore not installed plugins
                .Select(routeProvider => (IEndpointProvider)Activator.CreateInstance(routeProvider))
                .OrderByDescending(endointProvider => endointProvider.Priority);

            //register all provided routes
            foreach (var routeProvider in instances)
                routeProvider.RegisterRoutes(endpointBuilder);
        }

        #endregion
    }
}
