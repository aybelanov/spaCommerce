using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Core.Http;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Components.Services
{
    public class CompareProductsComponentService : ICompareProductsComponentService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly IJSService _jsService;

        #endregion

        #region Ctor

        public CompareProductsComponentService(CatalogSettings catalogSettings,
            IHttpContextAccessor httpContextAccessor, IProductService productService,
            IJSService jsService)
        {
            this._catalogSettings = catalogSettings;
            this._httpContextAccessor = httpContextAccessor;
            this._productService = productService;
            this._jsService = jsService;
        }

        #endregion

        #region Utilities
        /// <summary>
        /// Get a list of identifier of compared products
        /// </summary>
        /// <returns>List of identifier</returns>
        protected async Task<List<int>> GetComparedProductIds()
        {
            //var httpContext = _httpContextAccessor.HttpContext;
            //if (httpContext?.Request == null)
            //    return new List<int>();

            //try to get cookie
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ComparedProductsCookie}";
            var productIdsCookie = await _jsService.GetCookie(cookieName);

            if (string.IsNullOrEmpty(productIdsCookie))
                return new List<int>();

            //if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var productIdsCookie) || string.IsNullOrEmpty(productIdsCookie))
            //    return new List<int>();

            //get array of string product identifiers from cookie
            var productIds = productIdsCookie.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //return list of int product identifiers
            return productIds.Select(int.Parse).Distinct().ToList();
        }


        /// <summary>
        /// Add cookie value for the compared products
        /// </summary>
        /// <param name="comparedProductIds">Collection of compared products identifiers</param>
        protected async Task AddCompareProductsCookie(IEnumerable<int> comparedProductIds)
        {
            //delete current cookie if exists
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ComparedProductsCookie}";
            await _jsService.EraseCookie(cookieName);

            //create cookie value
            var comparedProductIdsCookie = string.Join(",", comparedProductIds);

            //create cookie options 
            var cookieExpires = 24 * 10; //TODO make configurable
            await _jsService.SetCookie(cookieName, comparedProductIdsCookie, cookieExpires);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        public async Task ClearCompareProducts()
        {
            //sets an expired cookie
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ComparedProductsCookie}";
            await _jsService.EraseCookie(cookieName);
        }

        /// <summary>
        /// Gets a "compare products" list
        /// </summary>
        /// <returns>"Compare products" list</returns>
        public async Task<IList<Product>> GetComparedProducts()
        {
            //get list of compared product identifiers
            var productIds = await GetComparedProductIds();

            //return list of product
            return _productService.GetProductsByIds(productIds.ToArray())
                .Where(product => product.Published && !product.Deleted).ToList();
        }

        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public async Task RemoveProductFromCompareList(int productId)
        {
            //get list of compared product identifiers
            var comparedProductIds = await GetComparedProductIds();

            //whether product identifier to remove exists
            if (!comparedProductIds.Contains(productId))
                return;

            //it exists, so remove it from list
            comparedProductIds.Remove(productId);

            //set cookie
            await AddCompareProductsCookie(comparedProductIds);
        }

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public async Task AddProductToCompareList(int productId)
        {
            //get list of compared product identifiers
            var comparedProductIds = await GetComparedProductIds();

            //whether product identifier to add already exist
            if (!comparedProductIds.Contains(productId))
                comparedProductIds.Insert(0, productId);

            //limit list based on the allowed number of products to be compared
            comparedProductIds = comparedProductIds.Take(_catalogSettings.CompareProductsNumber).ToList();

            //set cookie
            await AddCompareProductsCookie(comparedProductIds);
        }

        #endregion
    }
}
