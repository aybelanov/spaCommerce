using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Themes;
using Nop.Web.Framework.Components.Extensions;
using Nop.Web.Framework.Components.Infrastructure;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Themes;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Components
{
    // ADDED
    /// <summary>
    /// 
    /// </summary>
    public class SpaComponent : LayoutComponentBase, IDisposable
    {
        #region Properties, Fields, Parameters

        private Localizer _localizer;

        [Inject] private ILocalizationService _localizationService { get; set; }
        [Inject] public IHtmlHelper Html { get; set; }
        //private IHtmlHelper _htmlHelper;
        [Inject] public LinkGenerator Url { get; set; }
        //private LinkGenerator _linkGenerator;
        [Inject] IUrlRecordService urlRecordService { get; set; }
        [Inject] private NavigationManager uriHelper { get; set; }

        /// <summary>
        /// Globas applications parameters for passing into hierarchy of components
        /// </summary>
        [CascadingParameter] protected CommonParameters CommonParams { get; set; }
        /// <summary>
        /// Points if data loads (requests) before rendering of a page or after.
        /// </summary>
        [Parameter] public bool IsDataLazyLoading { get; set; }

        /// <summary>
        /// Points if a stub needs to show during data loading
        /// </summary>
        protected bool needStub;

        #endregion

        #region Ctor
        public SpaComponent()
        {
            IsDataLazyLoading = false;
            needStub = false;
        }
        #endregion

        #region Lifecycle
        protected override async Task OnInitializedAsync()
        {
            if (!IsDataLazyLoading)
            {
                await DataRequest();
                needStub = false;
            }
        }

        /// <summary>
        /// Loading data if a page path has been changed for the same (generic/template) page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual async Task OnLocationChanged(object sender, LocationChangedEventArgs args)
        {
            if (CommonParams.Router.IsPathChangedOnCurrentPage())
            {
                needStub = true;
                StateHasChanged();

                await DataRequest();
                needStub = false;
                StateHasChanged();
            }
        }

        protected virtual Task DataRequest()
        {
            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstTime)
        {
            if (firstTime)
            {
                if (IsDataLazyLoading)
                {
                    await DataRequest();
                    needStub = false;
                    StateHasChanged();
                }

                await OnceOnAfterRenderAsync();
            }
        }

        /// <summary>
        /// Exexutes one time after loading page even if it is a template page.
        /// It ensures once loading scripts for example.
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnceOnAfterRenderAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void Dispose()
        {

        }

        //// TODO
        //protected virtual void Dispose(bool disposing)
        //{

        //}
        #endregion

        #region Utils

        public Localizer T
        {
            get
            {
                //if (_localizationService == null)
                //    _localizationService = EngineContext.Current.Resolve<ILocalizationService>();

                if (_localizer == null)
                {
                    _localizer = (format, args) =>
                    {
                        var resFormat = _localizationService.GetResource(format);
                        if (string.IsNullOrEmpty(resFormat))
                        {
                            return new LocalizedString(format);
                        }
                        return new LocalizedString((args == null || args.Length == 0)
                            ? resFormat
                            : string.Format(resFormat, args));
                    };
                }
                return _localizer;
            }
        }

        /// <summary>
        /// Return a value indicating whether the working language and theme support RTL (right-to-left)
        /// </summary>
        /// <returns></returns>
        public bool ShouldUseRtlTheme()
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var supportRtl = workContext.WorkingLanguage.Rtl;
            if (supportRtl)
            {
                //ensure that the active theme also supports it
                var themeProvider = EngineContext.Current.Resolve<IThemeProvider>();
                var themeContext = EngineContext.Current.Resolve<IThemeContext>();
                supportRtl = themeProvider.GetThemeBySystemName(themeContext.WorkingThemeName)?.SupportRtl ?? false;
            }
            return supportRtl;
        }

        /// <summary>
        /// RenderSection implementation like in MVC
        /// </summary>
        /// <param name="name"></param>
        protected RenderFragment RenderSection(string name)
        {
            if (Body != null && name != null)
            {
                var pageType = (Body.Target as RouteView)?.RouteData?.PageType;
                if (pageType != null)
                {
                    var renderFragment = pageType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                        .Where(x => x.PropertyType == typeof(RenderFragment))
                        .FirstOrDefault(x => x.Name.Equals(name + "section", StringComparison.InvariantCultureIgnoreCase));

                    if (renderFragment != null)
                    {
                        var renderingSection = renderFragment.GetValue(Activator.CreateInstance(pageType));
                        return renderingSection as RenderFragment;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Render the component via the code
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vs"></param>
        /// <returns></returns>
        protected RenderFragment RenderComponent<T>(params object[] vs) => RenderComponent(typeof(T), vs);


        /// <summary>
        /// Render the component via the code
        /// </summary>
        /// <param name="T"></typeparam>
        /// <param name="vs"></param>
        /// <returns></returns>
        protected RenderFragment RenderComponent(Type type, params object[] vs) => builder =>
        {
            builder.OpenComponent(0, type);
            int i = 1;
            foreach (var obj in vs)
            {
                var props = obj.GetType().GetProperties();
                foreach (var prop in props)
                {
                    builder.AddAttribute(i++, prop.Name, prop.GetValue(obj, null));
                }
            }
            builder.CloseComponent();
        };

        protected int GetEntityId(string entityName) =>
            CurrentUrlRecord?.EntityName?.Equals(entityName, StringComparison.InvariantCultureIgnoreCase) ?? false
                 && CurrentUrlRecord.IsActive ? CurrentUrlRecord.EntityId : 0;

        /// <summary>
        /// Gets current urlreord of the slug service
        /// </summary>
        protected UrlRecord CurrentUrlRecord
        {
            get
            {
                var template = uriHelper.GetRelativePath();
                template = WebUtility.UrlDecode(template);
                template = template.TrimStart('/').TrimStart('{').TrimEnd('}');
                var cashRecord = urlRecordService.GetBySlugCached(template);
                if (cashRecord != null && cashRecord.IsActive)
                    return new UrlRecord()
                    {
                        EntityId = cashRecord.EntityId,
                        EntityName = cashRecord.EntityName,
                        Id = cashRecord.Id,
                        IsActive = cashRecord.IsActive,
                        LanguageId = cashRecord.LanguageId,
                        Slug = cashRecord.Slug
                    };
                else return null;
            }
        }

        #endregion
    }
}