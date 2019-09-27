﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Services.Vendors;
using Nop.Web.Framework.Localization;
using Microsoft.JSInterop;
using Nop.Web.Framework.Components.Services;
using Nop.Core.Infrastructure;
using System.Security.Claims;

namespace Nop.Web.Framework
{
    // TODO need a new implementation!!!
    /// <summary>
    /// Represents work _httpContextAccessor.HttpContext for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IVendorService _vendorService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly TaxSettings _taxSettings;

        private Customer _cachedCustomer;
        private Customer _originalCustomerIfImpersonated;
        private Vendor _cachedVendor;
        private Language _cachedLanguage;
        private Currency _cachedCurrency;
        private TaxDisplayType? _cachedTaxDisplayType;
        private HttpContext _cashedHttpContext;

        // ADDED locking object because of the get and set sections have records operations
        // (like SaveAttributes ) -  It is not thread safety
        object _lockCurency = new object();
        object _lockLanguage = new object();
        object _lockCustomer = new object();
        object _lockTax = new object();

        #endregion

        #region Ctor

        public WebWorkContext(CurrencySettings currencySettings,
            IAuthenticationService authenticationService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUserAgentHelper userAgentHelper,
            IVendorService vendorService,
            LocalizationSettings localizationSettings,
            TaxSettings taxSettings)
        {
            this._currencySettings = currencySettings;
            this._authenticationService = authenticationService;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._httpContextAccessor = httpContextAccessor;
            this._languageService = languageService;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._userAgentHelper = userAgentHelper;
            this._vendorService = vendorService;
            this._localizationSettings = localizationSettings;
            this._taxSettings = taxSettings;

        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get nop customer cookie
        /// </summary>
        /// <returns>String value of cookie</returns>
        protected virtual string GetCustomerCookie()
        {
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
            string result = _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];

            // workaround
            // considering that blazor server-side uses SignalR for getting data we get async requests during page rendering.
            // So those requests can come in before closing the render. Other version is the DI doesn't work properly.
            // Any case we have a new instance of WebWorkContext during one http request so this workaround has been done:
            // we get cookies from a response whiсh ones have been set before.
            if (string.IsNullOrEmpty(result))
            {
                var headerValues = _httpContextAccessor.HttpContext?.Response?.Headers?.Values;
                if (headerValues != null)
                    foreach (var headers in headerValues)
                        foreach (var header in headers)
                            if (header.StartsWith($"{cookieName}="))
                            {
                                var p1 = header.IndexOf('=');
                                var p2 = header.IndexOf(';');
                                result = header.Substring(p1 + 1, p2 - p1 - 1);
                            }
            }

            return result;
        }

        /// <summary>
        /// Set nop customer cookie
        /// </summary>
        /// <param name="customerGuid">Guid of the customer</param>
        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
           // changed
            if (_httpContextAccessor.HttpContext?.Response == null || _httpContextAccessor.HttpContext.Response.HasStarted)
                return;

            //delete current cookie value
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (customerGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate
            };
            
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, customerGuid.ToString(), options);
        }

        /// <summary>
        /// Get language from the requested page URL
        /// </summary>
        /// <returns>The found language</returns>
        protected virtual Language GetLanguageFromUrl()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;

            //whether the requsted URL is localized
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            if (!path.IsLocalizedUrl(_httpContextAccessor.HttpContext.Request.PathBase, false, out Language language))
                return null;

            //check language availability
            if (!_storeMappingService.Authorize(language))
                return null;

            return language;
        }

        /// <summary>
        /// Get language from the request
        /// </summary>
        /// <returns>The found language</returns>
        protected virtual Language GetLanguageFromRequest()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;

            //get request culture
            var requestCulture = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture;
            if (requestCulture == null)
                return null;

            //try to get language by culture name
            var requestLanguage = _languageService.GetAllLanguages().FirstOrDefault(language =>
                language.LanguageCulture.Equals(requestCulture.Culture.Name, StringComparison.InvariantCultureIgnoreCase));

            //check language availability
            if (requestLanguage == null || !requestLanguage.Published || !_storeMappingService.Authorize(requestLanguage))
                return null;

            return requestLanguage;
        }

        #endregion

        #region Properties
        //public virtual ClaimsPrincipal User
        //{
        //    get
        //    {
        //        if (_httpContextAccessor.HttpContext != null)
        //        {
        //            _cashedHttpContext = _httpContextAccessor.HttpContext;
        //            _cashedUser = _cashedHttpContext.User;
        //        }

        //        return _cashedUser;
        //    }
        //}


        public virtual HttpContext HttpContext
        {
            get
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    //_cashedHttpContext = _httpContextAccessor.HttpContext;
                    //_cashedUser = _cashedHttpContext.User;
                    _cashedHttpContext = new DefaultHttpContext(_httpContextAccessor.HttpContext.Features);
                }

                return _cashedHttpContext;
            }
        }


        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public virtual Customer CurrentCustomer
        {
            get
            {
                lock (_lockCustomer)
                {
                    //whether there is a cached value
                    if (_cachedCustomer != null)
                        return _cachedCustomer;

                    Customer customer = null;

                    //check whether request is made by a background (schedule) task
                    if (_httpContextAccessor.HttpContext == null ||
                        _httpContextAccessor.HttpContext.Request.Path.Equals(new PathString($"/{NopTaskDefaults.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //in this case return built-in customer record for background task
                        customer = _customerService.GetCustomerBySystemName(NopCustomerDefaults.BackgroundTaskCustomerName);
                    }

                    if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                    {
                        //check whether request is made by a search engine, in this case return built-in customer record for search engines
                        if (_userAgentHelper.IsSearchEngine())
                            customer = _customerService.GetCustomerBySystemName(NopCustomerDefaults.SearchEngineCustomerName);
                    }

                    if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                    {
                        //try to get registered user
                        //if (Context != null)
                            customer = _authenticationService.GetAuthenticatedCustomer();
                    }

                    if (customer != null && !customer.Deleted && customer.Active && !customer.RequireReLogin)
                    {
                        //get impersonate user if required
                        var impersonatedCustomerId = _genericAttributeService
                            .GetAttribute<int?>(customer, NopCustomerDefaults.ImpersonatedCustomerIdAttribute);

                        if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                        {
                            var impersonatedCustomer = _customerService.GetCustomerById(impersonatedCustomerId.Value);
                            if (impersonatedCustomer != null && !impersonatedCustomer.Deleted && impersonatedCustomer.Active && !impersonatedCustomer.RequireReLogin)
                            {
                                //set impersonated customer
                                _originalCustomerIfImpersonated = customer;
                                customer = impersonatedCustomer;
                            }
                        }
                    }

                    if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                    {
                        //get guest customer
                        var customerCookie = GetCustomerCookie();
                        if (!string.IsNullOrEmpty(customerCookie))
                        {
                            if (Guid.TryParse(customerCookie, out Guid customerGuid))
                            {
                                //get customer from cookie (should not be registered)
                                var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                                if (customerByCookie != null && !customerByCookie.IsRegistered())
                                    customer = customerByCookie;
                            }
                        }
                    }

                    if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                    {
                        //create guest if not exists
                        customer = _customerService.InsertGuestCustomer();
                    }

                    if (!customer.Deleted && customer.Active && !customer.RequireReLogin)
                    {
                        //set customer cookie
                        SetCustomerCookie(customer.CustomerGuid);

                        //cache the found customer
                        _cachedCustomer = customer;
                    }

                    return _cachedCustomer;
                }
            }
            set
            {
                lock (_lockCustomer)
                {
                    SetCustomerCookie(value.CustomerGuid);
                    _cachedCustomer = value;
                }
            }
        }

        /// <summary>
        /// Gets the original customer (in case the current one is impersonated)
        /// </summary>
        public virtual Customer OriginalCustomerIfImpersonated
        {
            get { return _originalCustomerIfImpersonated; }
        }

        /// <summary>
        /// Gets the current vendor (logged-in manager)
        /// </summary>
        public virtual Vendor CurrentVendor
        {
            get
            {
                //whether there is a cached value
                if (_cachedVendor != null)
                    return _cachedVendor;

                if (this.CurrentCustomer == null)
                    return null;

                //try to get vendor
                var vendor = _vendorService.GetVendorById(this.CurrentCustomer.VendorId);

                //check vendor availability
                if (vendor == null || vendor.Deleted || !vendor.Active)
                    return null;

                //cache the found vendor
                _cachedVendor = vendor;

                return _cachedVendor;
            }
        }

        /// <summary>
        /// Gets or sets current user working language
        /// </summary>
        public virtual Language WorkingLanguage
        {
            get
            {
                lock (_lockLanguage)
                {
                    //whether there is a cached value
                    if (_cachedLanguage != null)
                        return _cachedLanguage;

                    Language detectedLanguage = null;

                    //localized URLs are enabled, so try to get language from the requested page URL
                    if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                        detectedLanguage = GetLanguageFromUrl();

                    //whether we should detect the language from the request
                    if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
                    {
                        //whether language already detected by this way
                        var alreadyDetected = _genericAttributeService.GetAttribute<bool>(this.CurrentCustomer,
                            NopCustomerDefaults.LanguageAutomaticallyDetectedAttribute, _storeContext.CurrentStore.Id);

                        //if not, try to get language from the request
                        if (!alreadyDetected)
                        {
                            detectedLanguage = GetLanguageFromRequest();
                            if (detectedLanguage != null)
                            {
                                //language already detected
                                _genericAttributeService.SaveAttribute(this.CurrentCustomer,
                                    NopCustomerDefaults.LanguageAutomaticallyDetectedAttribute, true, _storeContext.CurrentStore.Id);
                            }
                        }
                    }

                    //if the language is detected we need to save it
                    if (detectedLanguage != null)
                    {
                        //get current saved language identifier
                        var currentLanguageId = _genericAttributeService.GetAttribute<int>(this.CurrentCustomer,
                            NopCustomerDefaults.LanguageIdAttribute, _storeContext.CurrentStore.Id);

                        //save the detected language identifier if it differs from the current one
                        if (detectedLanguage.Id != currentLanguageId)
                        {
                            _genericAttributeService.SaveAttribute(this.CurrentCustomer,
                                NopCustomerDefaults.LanguageIdAttribute, detectedLanguage.Id, _storeContext.CurrentStore.Id);
                        }
                    }

                    //get current customer language identifier
                    var customerLanguageId = _genericAttributeService.GetAttribute<int>(this.CurrentCustomer,
                        NopCustomerDefaults.LanguageIdAttribute, _storeContext.CurrentStore.Id);

                    var allStoreLanguages = _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id);

                    //check customer language availability
                    var customerLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == customerLanguageId);
                    if (customerLanguage == null)
                    {
                        //it not found, then try to get the default language for the current store (if specified)
                        customerLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == _storeContext.CurrentStore.DefaultLanguageId);
                    }

                    //if the default language for the current store not found, then try to get the first one
                    if (customerLanguage == null)
                        customerLanguage = allStoreLanguages.FirstOrDefault();

                    //if there are no languages for the current store try to get the first one regardless of the store
                    if (customerLanguage == null)
                        customerLanguage = _languageService.GetAllLanguages().FirstOrDefault();

                    //cache the found language
                    _cachedLanguage = customerLanguage;

                    return _cachedLanguage;
                }
            }
            set
            {
                lock (_lockLanguage)
                {
                    //get passed language identifier
                    var languageId = value?.Id ?? 0;

                    //and save it
                    _genericAttributeService.SaveAttribute(this.CurrentCustomer,
                        NopCustomerDefaults.LanguageIdAttribute, languageId, _storeContext.CurrentStore.Id);

                    //then reset the cached value
                    _cachedLanguage = null;
                }
            }
        }


        /// <summary>
        /// Gets or sets current user working currency
        /// </summary>
        public virtual Currency WorkingCurrency
        {
            get
            {
                lock (_lockCurency)
                {
                    //whether there is a cached value
                    if (_cachedCurrency != null)
                        return _cachedCurrency;

                    //return primary store currency when we're in admin area/mode
                    if (this.IsAdmin)
                    {
                        var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
                        if (primaryStoreCurrency != null)
                        {
                            _cachedCurrency = primaryStoreCurrency;
                            return primaryStoreCurrency;
                        }
                    }

                    //find a currency previously selected by a customer
                    var customerCurrencyId = _genericAttributeService.GetAttribute<int>(this.CurrentCustomer,
                        NopCustomerDefaults.CurrencyIdAttribute, _storeContext.CurrentStore.Id);

                    var allStoreCurrencies = _currencyService.GetAllCurrencies(storeId: _storeContext.CurrentStore.Id);

                    //check customer currency availability
                    var customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == customerCurrencyId);
                    if (customerCurrency == null)
                    {
                        //it not found, then try to get the default currency for the current language (if specified)
                        customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == this.WorkingLanguage.DefaultCurrencyId);
                    }

                    //if the default currency for the current store not found, then try to get the first one
                    if (customerCurrency == null)
                        customerCurrency = allStoreCurrencies.FirstOrDefault();

                    //if there are no currencies for the current store try to get the first one regardless of the store
                    if (customerCurrency == null)
                        customerCurrency = _currencyService.GetAllCurrencies().FirstOrDefault();

                    //cache the found currency
                    _cachedCurrency = customerCurrency;

                    return _cachedCurrency;
                }
            }
            set
            {
                lock (_lockCurency)
                {
                    //get passed currency identifier
                    var currencyId = value?.Id ?? 0;

                    //and save it
                    _genericAttributeService.SaveAttribute(this.CurrentCustomer,
                        NopCustomerDefaults.CurrencyIdAttribute, currencyId, _storeContext.CurrentStore.Id);

                    //then reset the cached value
                    _cachedCurrency = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        public virtual TaxDisplayType TaxDisplayType
        {
            get
            {
                lock (_lockTax)
                {
                    //whether there is a cached value
                    if (_cachedTaxDisplayType.HasValue)
                        return _cachedTaxDisplayType.Value;

                    var taxDisplayType = TaxDisplayType.IncludingTax;

                    //whether customers are allowed to select tax display type
                    if (_taxSettings.AllowCustomersToSelectTaxDisplayType && this.CurrentCustomer != null)
                    {
                        //try to get previously saved tax display type
                        var taxDisplayTypeId = _genericAttributeService.GetAttribute<int?>(this.CurrentCustomer,
                            NopCustomerDefaults.TaxDisplayTypeIdAttribute, _storeContext.CurrentStore.Id);
                        if (taxDisplayTypeId.HasValue)
                        {
                            taxDisplayType = (TaxDisplayType)taxDisplayTypeId.Value;
                        }
                        else
                        {
                            //default tax type by customer roles
                            var defaultRoleTaxDisplayType = _customerService.GetCustomerDefaultTaxDisplayType(this.CurrentCustomer);
                            if (defaultRoleTaxDisplayType != null)
                            {
                                taxDisplayType = defaultRoleTaxDisplayType.Value;
                            }
                        }
                    }
                    else
                    {
                        //default tax type by customer roles
                        var defaultRoleTaxDisplayType = _customerService.GetCustomerDefaultTaxDisplayType(this.CurrentCustomer);
                        if (defaultRoleTaxDisplayType != null)
                        {
                            taxDisplayType = defaultRoleTaxDisplayType.Value;
                        }
                        else
                        {
                            //or get the default tax display type
                            taxDisplayType = _taxSettings.TaxDisplayType;
                        }
                    }

                    //cache the value
                    _cachedTaxDisplayType = taxDisplayType;

                    return _cachedTaxDisplayType.Value;
                }
            }
            set
            {
                lock (_lockTax)
                {
                    //whether customers are allowed to select tax display type
                    if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
                        return;

                    //save passed value
                    _genericAttributeService.SaveAttribute(this.CurrentCustomer,
                        NopCustomerDefaults.TaxDisplayTypeIdAttribute, (int)value, _storeContext.CurrentStore.Id);

                    //then reset the cached value
                    _cachedTaxDisplayType = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}