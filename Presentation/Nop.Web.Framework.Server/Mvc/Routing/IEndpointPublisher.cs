using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Web.Framework.Server.Mvc.Routing
{
    //TODO added
    /// <summary>
    /// Represents route publisher
    /// </summary>
    public interface IEndpointPublisher
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        void RegisterRoutes(IEndpointRouteBuilder routeBuilder);
    }
}
