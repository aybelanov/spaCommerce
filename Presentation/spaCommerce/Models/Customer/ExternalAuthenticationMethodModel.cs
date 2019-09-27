using Nop.Web.Framework.Models;
using System;

namespace spaCommerce.Models.Customer
{
    public partial class ExternalAuthenticationMethodModel : BaseNopModel
    {
        public string ViewComponentName { get; set; }
        public Type ViewComponentType { get; set; }
    }
}