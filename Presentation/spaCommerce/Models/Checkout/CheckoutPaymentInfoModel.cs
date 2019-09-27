using Nop.Web.Framework.Models;
using System;

namespace spaCommerce.Models.Checkout
{
    public partial class CheckoutPaymentInfoModel : BaseNopModel
    {
        public Type PaymentViewComponentType { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool DisplayOrderTotals { get; set; }
    }
}