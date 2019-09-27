using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Components.Extensions;
using Nop.Web.Framework.Security.Captcha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Components.UI
{
    public class Recaptcha : SpaComponent
    {
        [Inject] private IHtmlHelper htmlHelper { get; set; }
        [Inject] private IJSRuntime js { get; set; }
        [Inject] CaptchaSettings captchaSettings { get; set; }
        [Inject] IHttpContextAccessor httpContextAccessor { get; set; }
        public string Response { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddMarkupContent(0, "<div id=\"recaptcha\"></div>");
        }

        // TODO refactoring code after solving https://github.com/aspnet/AspNetCore/issues/11595
        protected override async Task OnceOnAfterRenderAsync()
        {
            try
            {
                var config = htmlHelper.GenerateScriptSourceCaptcha();
                await js.InvokeAsync<object>("GoogleCaptcha.Set", config);//, DotNetObjectReference.Create(this));
            }
            catch { }
        }

        [JSInvokable]
        public void SetCaptchaResponse(string response)
        {
            Response = response;
        }
        public async Task<bool> IsCaptchaValid()
        {
            try
            {
                var response = await js.InvokeAsync<string>("GoogleCaptcha.GetState");

                if (!string.IsNullOrWhiteSpace(response))
                {
                    var captchaValidtor = new GReCaptchaValidator()
                    {
                        SecretKey = captchaSettings.ReCaptchaPrivateKey,
                        RemoteIp = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Response = response
                    };

                    var captchaResponse = captchaValidtor.Validate();
                    return captchaResponse.IsValid;
                }
                else return false;
            }
            catch { return false; }
        }
    }
}