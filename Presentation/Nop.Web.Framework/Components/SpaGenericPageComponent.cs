using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Seo;
using Nop.Core.Domain.Seo;
using System.Net;
using Nop.Web.Framework.Components.Extensions;

namespace Nop.Web.Framework.Components
{
    public class SpaGenericPageComponent : LazyLoadingSpaComponent
    {
        [Inject] private NavigationManager uriHelper { get; set; }
        EventHandler<LocationChangedEventArgs> handlerOnLocationChanged;

        protected override async Task OnInitializedAsync()
        {
            handlerOnLocationChanged = async (s, e) => await base.OnLocationChanged(s, e);
            uriHelper.LocationChanged += handlerOnLocationChanged;
            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            uriHelper.LocationChanged -= handlerOnLocationChanged;
            base.Dispose();
        }
    }
}
