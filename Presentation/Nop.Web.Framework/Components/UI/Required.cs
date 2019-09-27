using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Web.Framework.Components.UI
{
    public class Required : ComponentBase
    {
        [Parameter] public string Sighn { get; set; } = "*";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", "required");
            builder.AddContent(2, Sighn);
            builder.CloseElement();
        }
    }
}
