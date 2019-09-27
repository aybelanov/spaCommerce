using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Server.Localization;
using Nop.Web.Framework.Server.Mvc.Routing;
using Nop.Web.Framework.Server.Seo;

namespace spaCommerce.Server.Infrastructure
{
    // TODO added
    /// <summary>
    /// Represents provider that provided generic routes
    /// </summary>
    public partial class GenericUrlEndpointProvider : IEndpointProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            //and default one
            //routeBuilder.MapControllerRoute("Default", "{controller}/{action}/{id?}");

            //generic URLs
            routeBuilder.MapGenericPathEndpoint("GenericUrl", "{GenericSeName}",
                new { controller = "Common", action = "GenericUrl" });

            //define this routes to use in UI views (in case if you want to customize some of them later)
            routeBuilder.MapLocalizedEndpoint("Product", "{SeName}", 
                new { controller = "Product", action = "ProductDetails" });

            routeBuilder.MapLocalizedEndpoint("Category", "{SeName}", 
                new { controller = "Catalog", action = "Category" });

            routeBuilder.MapLocalizedEndpoint("Manufacturer", "{SeName}", 
                new { controller = "Catalog", action = "Manufacturer" });

            routeBuilder.MapLocalizedEndpoint("Vendor", "{SeName}", 
                new { controller = "Catalog", action = "Vendor" });
            
            routeBuilder.MapLocalizedEndpoint("NewsItem", "{SeName}", 
                new { controller = "News", action = "NewsItem" });

            routeBuilder.MapLocalizedEndpoint("BlogPost", "{SeName}", 
                new { controller = "Blog", action = "BlogPost" });

            routeBuilder.MapLocalizedEndpoint("Topic", "{SeName}",
                new { controller = "Topic", action = "TopicDetails" });

            //product tags
            routeBuilder.MapLocalizedEndpoint("ProductsByTag", "{SeName}",
                new { controller = "Catalog", action = "ProductsByTag" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            //it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
            get { return -1000000; }
        }

        #endregion
    }
}
