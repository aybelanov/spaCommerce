using Nop.Web.Framework.Models;
using System;

namespace spaCommerce.Areas.Admin.Models.Cms
{
    public partial class RenderWidgetModel : BaseNopModel
    {
        // chaged
        public Type WidgetViewComponentType { get; set; }
        public object WidgetViewComponentArguments { get; set; }
    }
}