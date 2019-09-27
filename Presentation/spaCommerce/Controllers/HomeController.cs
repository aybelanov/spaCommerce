using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace spaCommerce.Controllers
{
    public partial class HomeController : BasePublicController
    {
        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult Index()
        {
            return View();
            //return View("IndexDebug");
        }
    }
}