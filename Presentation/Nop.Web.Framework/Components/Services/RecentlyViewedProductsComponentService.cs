using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Nop.Core.Domain.Catalog;
using Nop.Core.Http;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Components.Services
{
    public class RecentlyViewedProductsComponentService : IRecentlyViewedProductsComponentService
    {
        #region Fields

        CatalogSettings _catalogSettings { get; set; }
        IHttpContextAccessor _httpContextAccessor { get; set; }
        IProductService _productService { get; set; }
        IJSRuntime _jsRuntime { get; set; }

        #endregion

        #region Ctor
        public RecentlyViewedProductsComponentService(CatalogSettings catalogSettings,
            IHttpContextAccessor httpContextAccessor,
            IProductService productService,
            IJSRuntime jsRuntime)
        {
            this._catalogSettings = catalogSettings;
            this._httpContextAccessor = httpContextAccessor;
            this._productService = productService;
            this._jsRuntime = jsRuntime;
        }

        #endregion

        /// <summary>
        /// Gets a list of identifier of recently viewed products
        /// </summary>
        /// <returns>List of identifier</returns>
        protected Task<List<int>> GetRecentlyViewedProductsIds()
        {
            return GetRecentlyViewedProductsIds(int.MaxValue);
        }


        /// <summary>
        /// Gets a list of identifier of recently viewed products
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>List of identifier</returns>
        protected async Task<List<int>> GetRecentlyViewedProductsIds(int number)
        {
            //try to get cookie
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.RecentlyViewedProductsCookie}";
            string productIdsCookie = string.Empty;
            try
            {
                productIdsCookie = await _jsRuntime.InvokeAsync<string>("CookiesService.Get", cookieName);
                if (string.IsNullOrEmpty(productIdsCookie))
                    throw new Exception();
            }
            catch
            {
                return new List<int>();
            }

            //get array of string product identifiers from cookie
            var productIds = productIdsCookie.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //return list of int product identifiers
            return productIds.Select(int.Parse).Distinct().Take(number).ToList();
        }


        /// <summary>
        /// Add cookie value for the recently viewed products
        /// </summary>
        /// <param name="recentlyViewedProductIds">Collection of the recently viewed products identifiers</param>
        protected async Task AddRecentlyViewedProductsCookie(IEnumerable<int> recentlyViewedProductIds)
        {
            //delete current cookie if exists
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.RecentlyViewedProductsCookie}";

            try
            {
                await _jsRuntime.InvokeAsync<object>("CookiesService.Erase");
            }
            catch { }

            //create cookie value
            var productIdsCookie = string.Join(",", recentlyViewedProductIds);

            //create cookie options 
            var cookieExpires = 24 * 10; //TODO make configurable
            
            try
            {
                await _jsRuntime.InvokeAsync<object>("CookiesService.Set", cookieName, productIdsCookie, cookieExpires);
            }
            catch { }
        }

        /// <summary>
        /// Gets a "recently viewed products" list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>"recently viewed products" list</returns>
        public async Task<IList<Product>> GetRecentlyViewedProducts(int number)
        {
            //get list of recently viewed product identifiers
            var productIds = await GetRecentlyViewedProductsIds(number);
            
            //return list of product
            return _productService.GetProductsByIds(productIds.ToArray())
                .Where(product => product.Published && !product.Deleted).ToList();
        }

        /// <summary>
        /// Adds a product to a recently viewed products list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public async Task AddProductToRecentlyViewedList(int productId)
        {
            //whether recently viewed products is enabled
            if (!_catalogSettings.RecentlyViewedProductsEnabled)
                return ;

            //get list of recently viewed product identifiers
            var productIds = await GetRecentlyViewedProductsIds();

            //whether product identifier to add already exist
            if (!productIds.Contains(productId))
                productIds.Insert(0, productId);

            //limit list based on the allowed number of the recently viewed products
            productIds = productIds.Take(_catalogSettings.RecentlyViewedProductsNumber).ToList();

            //set cookie
            await AddRecentlyViewedProductsCookie(productIds);
        }
    }
}
