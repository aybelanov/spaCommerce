using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Plugins;
using Nop.Data;
using Nop.Services.Affiliates;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Forums;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Services.Tax;
using Nop.Services.Themes;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Components.Services;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Server.Components;
using Nop.Web.Framework.Server.Infrastructure.Extensions;
using Nop.Web.Framework.Server.Mvc.Routing;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;


namespace Nop.Web.Framework.Server.Infrastructure
{
    public class ServiceRegistar : IServiceRegistrar
    {
        public void Register(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder, NopConfig nopConfig)
        {
            //file provider
            services.AddScoped<INopFileProvider, NopFileProvider>();

            //web helper
            services.AddScoped<IWebHelper, WebHelper>();

            //user agent helper
            services.AddScoped<IUserAgentHelper, UserAgentHelper>();

            //data layer
            services.AddTransient<IDataProviderManager, EfDataProviderManager>();
            //services.Register(context => context.Resolve<IDataProviderManager>().DataProvider).As<IDataProvider>().InstancePerDependency();
            services.AddTransient(provider => provider.GetRequiredService<IDataProviderManager>().DataProvider);
            services.AddScoped<IDbContext>(provider => new NopObjectContext(provider.GetRequiredService<DbContextOptions<NopObjectContext>>()));

            //repositories
            services.AddTransient(typeof(IRepository<>), typeof(EfRepository<>));

            //plugins
            services.AddScoped<IPluginFinder, PluginFinder>();
            services.AddScoped<IOfficialFeedManager, OfficialFeedManager>();

            //cache manager
            //services.AddScoped<ICacheManager, PerRequestCacheManager>();
            //services.AddScoped<ICacheManager, MemoryCacheManager>();
            services.AddScoped<ICacheManager, NopNullCache>();

            //static cache manager
            if (nopConfig.RedisCachingEnabled)
            {
                services.AddSingleton<ILocker, RedisConnectionWrapper>();
                services.AddSingleton<IRedisConnectionWrapper, RedisConnectionWrapper>();
                services.AddScoped<IStaticCacheManager, RedisCacheManager>();
            }
            else
            {
                services.AddSingleton<ILocker, MemoryCacheManager>();
                services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();

                // PROBLEM Choice of CacheManager implementation
                // ADDED because PerRequestCacheManager doesn't work properly. Blazor doesn't support classical requests and 
                // PerRequestCacheManager causes many errors including DBContext's and Thread's ones.
                //services.AddSingleton<ICacheManager, MemoryCacheManager>();
            }

            //work context
            services.AddScoped<IWorkContext, WebWorkContext>();

            //store context
            services.AddScoped<IStoreContext, WebStoreContext>();

            //services
            services.AddScoped<IBackInStockSubscriptionService, BackInStockSubscriptionService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICompareProductsService, CompareProductsService>();
            services.AddScoped<IRecentlyViewedProductsService, RecentlyViewedProductsService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IPriceFormatter, PriceFormatter>();
            services.AddScoped<IProductAttributeFormatter, ProductAttributeFormatter>();
            services.AddScoped<IProductAttributeParser, ProductAttributeParser>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICopyProductService, CopyProductService>();
            services.AddScoped<ISpecificationAttributeService, SpecificationAttributeService>();
            services.AddScoped<IProductTemplateService, ProductTemplateService>();
            services.AddScoped<ICategoryTemplateService, CategoryTemplateService>();
            services.AddScoped<IManufacturerTemplateService, ManufacturerTemplateService>();
            services.AddScoped<ITopicTemplateService, TopicTemplateService>();
            services.AddScoped<IProductTagService, ProductTagService>();
            services.AddScoped<IAddressAttributeFormatter, AddressAttributeFormatter>();
            services.AddScoped<IAddressAttributeParser, AddressAttributeParser>();
            services.AddScoped<IAddressAttributeService, AddressAttributeService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAffiliateService, AffiliateService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<IVendorAttributeFormatter, VendorAttributeFormatter>();
            services.AddScoped<IVendorAttributeParser, VendorAttributeParser>();
            services.AddScoped<IVendorAttributeService, VendorAttributeService>();
            services.AddScoped<ISearchTermService, SearchTermService>();
            services.AddScoped<IGenericAttributeService, GenericAttributeService>();
            services.AddScoped<IFulltextService, FulltextService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<ICustomerAttributeFormatter, CustomerAttributeFormatter>();
            services.AddScoped<ICustomerAttributeParser, CustomerAttributeParser>();
            services.AddScoped<ICustomerAttributeService, CustomerAttributeService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerRegistrationService, CustomerRegistrationService>();
            services.AddScoped<ICustomerReportService, CustomerReportService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IAclService, AclService>();
            services.AddScoped<IPriceCalculationService, PriceCalculationService>();
            services.AddScoped<IGeoLookupService, GeoLookupService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IMeasureService, MeasureService>();
            services.AddScoped<IStateProvinceService, StateProvinceService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IStoreMappingService, StoreMappingService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ILocalizedEntityService, LocalizedEntityService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IDownloadService, DownloadService>();
            services.AddScoped<IMessageTemplateService, MessageTemplateService>();
            services.AddScoped<IQueuedEmailService, QueuedEmailService>();
            services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IWorkflowMessageService, WorkflowMessageService>();
            services.AddScoped<IMessageTokenProvider, MessageTokenProvider>();
            services.AddScoped<ITokenizer, Tokenizer>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ICheckoutAttributeFormatter, CheckoutAttributeFormatter>();
            services.AddScoped<ICheckoutAttributeParser, CheckoutAttributeParser>();
            services.AddScoped<ICheckoutAttributeService, CheckoutAttributeService>();
            services.AddScoped<IGiftCardService, GiftCardService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderReportService, OrderReportService>();
            services.AddScoped<IOrderProcessingService, OrderProcessingService>();
            services.AddScoped<IOrderTotalCalculationService, OrderTotalCalculationService>();
            services.AddScoped<IReturnRequestService, ReturnRequestService>();
            services.AddScoped<IRewardPointService, RewardPointService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<ICustomNumberFormatter, CustomNumberFormatter>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IAuthenticationService, CookieAuthenticationService>();
            services.AddScoped<IUrlRecordService, UrlRecordService>();
            services.AddScoped<IShipmentService, ShipmentService>();
            services.AddScoped<IShippingService, ShippingService>();
            services.AddScoped<IDateRangeService, DateRangeService>();
            services.AddScoped<ITaxCategoryService, TaxCategoryService>();
            services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<ILogger, DefaultLogger>();
            services.AddScoped<ICustomerActivityService, CustomerActivityService>();
            services.AddScoped<IForumService, ForumService>();
            services.AddScoped<IGdprService, GdprService>();
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IWidgetService, WidgetService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IDateTimeHelper, DateTimeHelper>();
            services.AddScoped<ISitemapGenerator, SitemapGenerator>();
            services.AddScoped<IPageHeadBuilder, PageHeadBuilder>();
            services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
            services.AddScoped<IExportManager, ExportManager>();
            services.AddScoped<IImportManager, ImportManager>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IThemeProvider, ThemeProvider>();
            services.AddScoped<IThemeContext, ThemeContext>();
            services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();
            services.AddSingleton<IRoutePublisher, RoutePublisher>();
            services.AddSingleton<IReviewTypeService, ReviewTypeService>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddSingleton<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ISettingService, SettingService>();

            #region added
            //TODO added
            services.AddScoped<IEndpointPublisher, EndpointPublisher>();
            services.AddScoped<IJSService, JSService>();
            services.AddScoped<IRecentlyViewedProductsComponentService, RecentlyViewedProductsComponentService>();
            services.AddScoped<ICompareProductsComponentService, CompareProductsComponentService>();
            services.AddScoped<AuthenticationStateProvider, ComponentAuthStateProvider>();
            #endregion

            services.AddScoped<IActionContextAccessor, ActionContextAccessor>();

            //register all settings
            //services.RegisterSource(new SettingsSource());
            var settings = typeFinder.FindClassesOfType(typeof(ISettings)).ToList();
            foreach (var setting in settings)
            {
                services.AddScoped(setting,
                    provider => provider.GetRequiredService<ISettingService>().LoadSetting(setting, provider.GetRequiredService<IStoreContext>().CurrentStore.Id));
            }

            //services.AddSingleton<IActionResultExecutor<RedirectResult>, NopRedirectResultExecutor>();

            //picture service
            if (!string.IsNullOrEmpty(nopConfig.AzureBlobStorageConnectionString))
                services.AddScoped<IPictureService, AzurePictureService>();
            else
                services.AddScoped<IPictureService, PictureService>();

            //installation service
            if (!DataSettingsManager.DatabaseIsInstalled)
            {
                if (nopConfig.UseFastInstallationService)
                    services.AddScoped<IInstallationService, SqlFileInstallationService>();
                else
                    services.AddScoped<IInstallationService, CodeFirstInstallationService>();
            }

            //event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                var consumersInterfaces = consumer.FindInterfaces((type, criteria) =>
                {
                    var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                    return isMatch;

                }, typeof(IConsumer<>));

                foreach (var @interface in consumersInterfaces)
                {
                    services.AddScoped(@interface, consumer);
                }
            }
        }

        public void Configure(IApplicationBuilder application)
        {

        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //MVC should be loaded last
            get { return 0; }
        }
    }
}
