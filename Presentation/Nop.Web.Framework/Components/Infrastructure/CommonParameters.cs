using Microsoft.AspNetCore.Components;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Components.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Components.Infrastructure
{
    /// <summary>
    /// It's a class of global parameters being used in the app for passing between classes as a cascading value 
    /// </summary>
    public class CommonParameters
    {
        public delegate void CurrencyChangedHandler(IComponent sender);
        public delegate void ProductAttributeChangedHandler(IComponent sender);
        public delegate void ProductAddedToCartHandler(IComponent sender, ProductAddedToCartEventArgs args);
        public delegate void CheckoutAttributeChangedHandler(IComponent sender);


        /// <summary>
        /// Payment request for hand over through steps during chekout process
        /// </summary>
        public ProcessPaymentRequest ProcessPaymentRequest { get; set; }
        /// <summary>
        /// The current router
        /// </summary>
        public Router Router { get; set; }

        /// <summary>
        /// It is called after a product was added to a cart
        /// </summary>
        public event ProductAddedToCartHandler ProductAddedToCart;

        /// <summary>
        /// It is called while the current currency is being changed with CurrencySelector
        /// </summary>
        public event CurrencyChangedHandler CurrencyChanged;

        /// <summary>
        /// It is called while products attributes are being changed by a user.
        /// </summary>
        public event ProductAttributeChangedHandler ProductAttributeChanged;

        /// <summary>
        /// It is called while products attributes are being changed by a user.
        /// </summary>
        public event CheckoutAttributeChangedHandler CheckoutAttributeChanged;

        /// <summary>
        /// It is called inside sender's method which provides a currency changing method.
        /// </summary>
        /// <param name="sender">Component, which has changed a currency</param>
        /// <param name="currency">New currency</param>
        public void OnCurrencyChanged(IComponent sender)
        {
            CurrencyChanged?.Invoke(sender);
        }

        /// <summary>
        /// It is called inside sender's method which provides a attribute changing method.
        /// </summary>
        /// <param name="sender">Component, which has changed an attribute</param>
        /// <param name="currency">New currency</param>
        public void OnProductAttributeChanged(IComponent sender)
        {
            ProductAttributeChanged?.Invoke(sender);
        }

        /// <summary>
        /// IT is called inside carts (e.g. flyout cart and its attributes)
        /// </summary>
        /// <param name="sender">Component which has called the event</param>
        public void OnProductAddedToCart(IComponent sender, ProductAddedToCartEventArgs args)
        {
            ProductAddedToCart?.Invoke(sender, args);
        }

        /// <summary>
        /// IT is called when a checkout attribute is changed
        /// </summary>
        /// <param name="sender">Component which has called the event</param>
        public void OnCheckoutAttributeChanged(IComponent component)
        {
            CheckoutAttributeChanged?.Invoke(component);
        }
    }

    public class ProductAddedToCartEventArgs
    {
        public ShoppingCartType CartType { get; set; }
        //public Product Product { get; set; }
    }

}
