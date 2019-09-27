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
    public class LazyLoadingSpaComponent : SpaComponent
    {
        public LazyLoadingSpaComponent()
        {
            IsDataLazyLoading = true;
            needStub = true;
        }
    }
}
