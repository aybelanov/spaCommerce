using Nop.Web.Framework.Models;
using spaCommerce.Models.Common;

namespace spaCommerce.Models.Customer
{
    public partial class CustomerAddressEditModel : BaseNopModel
    {
        public CustomerAddressEditModel()
        {
            this.Address = new AddressModel();
        }
        
        public AddressModel Address { get; set; }
    }
}