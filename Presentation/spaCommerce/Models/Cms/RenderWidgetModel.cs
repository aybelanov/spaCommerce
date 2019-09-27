using Nop.Web.Framework.Models;
using System;

namespace spaCommerce.Models.Cms
{
    public partial class RenderWidgetModel : BaseNopModel
    {
        // changed
        public Type WidgetViewComponentType { get; set; }
        public object[] WidgetViewComponentArguments { get; set; }
    }
}