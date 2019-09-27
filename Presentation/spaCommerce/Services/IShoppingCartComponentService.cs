using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Components.Infrastructure;
using Nop.Web.Framework.Components.Services;
using spaCommerce.Models;
using spaCommerce.Models.Catalog;
using spaCommerce.Models.ShoppingCart;

namespace spaCommerce.Services
{
    public interface IShoppingCartComponentService
    {
        //WishlistModel AddItemsToCartFromWishlist(Guid? customerGuid, IFormCollection form);
        AddToCartSummary AddProductToComparerList(int productId);
        AddToCartSummary AddProductToCart_Catalog(int productId, int shoppingCartTypeId, int quantity, bool forceredirection = false);
        AddToCartSummary AddProductToCart_Details(int productId, int shoppingCartTypeId, IFormCollection form);
        ShoppingCartModel ApplyDiscountCoupon(string discountcouponcode, IFormCollection form);
        ShoppingCartModel ApplyGiftCard(string giftcardcouponcode, IFormCollection form);
        //ShoppingCartModel Cart();
        CheckoutAttributeList CheckoutAttributeChange(CheckoutAttributeList attributeList, bool isEditable);
        //void ContinueShopping();
        //WishlistEmailAFriendModel EmailWishlist();
        //WishlistEmailAFriendModel EmailWishlistSend(WishlistEmailAFriendModel model, bool captchaValid);
        EstimateShippingResultModel GetEstimateShipping(int? countryId, int? stateProvinceId, string zipPostalCode, IFormCollection form);

        //void ProductDetails_AttributeChange(ProductAttributeList<ProductDetailsModel.ProductAttributeModel> productAttributes
        //    , bool validateAttributeConditions = true, bool loadPicture = true, IDictionary<string, string> dic = null);

        ShoppingCartModel RemoveDiscountCoupon(IFormCollection form);
        ShoppingCartModel RemoveGiftCardCode(IFormCollection form);
        //ShoppingCartModel StartCheckout(IFormCollection form);
        //ShoppingCartModel UpdateCart(IFormCollection form);
        //WishlistModel UpdateWishlist(IFormCollection form);
        object UploadFileCheckoutAttribute(int attributeId);
        object UploadFileProductAttribute(int attributeId);
        //WishlistModel Wishlist(Guid? customerGuid);

        string ParseProductAttributes(Product product, IFormCollection form, List<string> errors);
        void ParseRentalDates(Product product, IFormCollection form, out DateTime? startDate, out DateTime? endDate);
        void ParseAndSaveCheckoutAttributes(List<ShoppingCartItem> cart, IFormCollection form);

        // added
        //ShoppingCartModel PrepareShoppingCartModel(ShoppingCartModel model, IList<ShoppingCartItem> cart, bool isEditable = true,
        //    bool validateCheckoutAttributes = false, bool prepareAndDisplayOrderReviewData = false);
    }
}