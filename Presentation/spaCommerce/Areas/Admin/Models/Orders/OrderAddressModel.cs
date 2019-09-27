using spaCommerce.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;

namespace spaCommerce.Areas.Admin.Models.Orders
{
    public partial class OrderAddressModel : BaseNopModel
    {
        #region Ctor

        public OrderAddressModel()
        {
            Address = new AddressModel();
        }

        #endregion

        #region Properties

        public int OrderId { get; set; }

        public AddressModel Address { get; set; }

        #endregion
    }
}