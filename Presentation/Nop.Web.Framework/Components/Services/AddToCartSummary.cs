using Nop.Web.Framework.Components.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Components.Services
{
    public class AddToCartSummary : IJSNotificationMessage
    {
        public bool success { get; set; }
        public string[] message { get; set; }
        public string RedirectUrl { get; set; }
        //public string updatetopwishlistsectionhtml { get; set; }
        //public string updatetopcartsectionhtml { get; set; }
        //public string updateflyoutcartsectionhtml { get; set; }
    }
}
