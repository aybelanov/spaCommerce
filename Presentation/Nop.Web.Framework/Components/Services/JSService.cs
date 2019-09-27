using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Components.Services
{

    /// <summary>
    /// Implementation of the calls to public.common.js and public.ajaxcart.js for manipulating the DOM elements
    /// </summary>
    public class JSService : IJSService
    {
        private readonly IJSRuntime _js;

        public JSService(IJSRuntime js)
        {
            this._js = js;
        }

        /// <summary>
        /// Shows a native load waiting stub
        /// </summary>
        /// <returns></returns>
        public async Task ShowLoadWaiting()
        {
            try
            {
                await _js.InvokeAsync<object>("AjaxCart.setLoadWaiting", true);
            }
            catch { }
        }

        /// <summary>
        /// Hides a native load waiting stub
        /// </summary>
        /// <returns></returns>
        public async Task ResetLoadWaiting()
        {
            try
            {
                await _js.InvokeAsync<object>("AjaxCart.setLoadWaiting", false);
            }
            catch { }
        }

        /// <summary>
        /// Shows messages according the buying process results
        /// </summary>
        /// <param name="message">Messages</param>
        /// <returns></returns>
        public async Task ShowNotifications(IJSNotificationMessage message)
        {
            try
            {
                await _js.InvokeAsync<object>("AjaxCart.success_process", message);
            }
            catch { }
        }

        /// <summary>
        /// Open a modal window wicth content from the query
        /// </summary>
        /// <param name="query">url/query for window content</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="scroll">enable/disable scrolling</param>
        /// <returns></returns>
        public async Task OpenWindow(string query, int width, int height, bool scroll)
        {
            try
            {
                await _js.InvokeAsync<object>("OpenWindow", query, width, height, scroll);
            }
            catch { }
        }

        /// <summary>
        /// Display a popup content from the pointed url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="modal">if the window should be shown as modal one</param>
        /// <param name="width"></param>
        /// <returns></returns>
        public async Task DisplayPopupContentFromUrl(string url, string title = null, bool? modal = null, int? width= null)
        {
            try
            {
                await _js.InvokeAsync<object>("displayPopupContentFromUrl", url, title, modal, width);
            }
            catch { }
        }
        /// <summary>
        /// Display a popup content with the pointed content 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="modal">if the window should be shown as modal one</param>
        /// <param name="width"></param>
        /// <returns></returns>
        public async Task DisplayPopupContent(string content, string title = null, bool? modal = null, int? width = null)
        {
            try
            {
                await _js.InvokeAsync<object>("displayPopupContent", content, title, modal, width);
            }
            catch { }
        }
        /// <summary>
        /// Get the alert windiw with the pointed message
        /// </summary>
        /// <param name="message">message</param>
        /// <returns></returns>
        public async Task Alert(string message)
        {
            try
            {
                await _js.InvokeAsync<object>("alert", message);
            }
            catch { }
        }

        /// <summary>
        /// Display/hide a load waiting stub
        /// </summary>
        /// <param name="display">true - display; false - hide;</param>
        public async Task DisplayAjaxLoading(bool display)
        {
            try
            {
                await _js.InvokeAsync<object>("displayAjaxLoading", display);
            }
            catch { }
        }

        /// <summary>
        /// Display a popup notification
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="modal"></param>
        /// <returns></returns>
        public async Task DisplayPopupNotification(string[] message, JSMessageType messageType, bool modal)
        {
            try
            {
                await _js.InvokeAsync<object>("displayPopupNotification", message, messageType.ToString().ToLower(), modal);
            }
            catch { }
        }

        /// <summary>
        /// Display a bar notification
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="timeout">timeout delay the disappearance, seconds</param>
        /// <returns></returns>
        public async Task DisplayBarNotification(string[] message, JSMessageType messageType = JSMessageType.Succes, int timeout = 0)
        {
            try
            {
                await _js.InvokeAsync<object>("displayBarNotification", message, messageType.ToString().ToLower(), timeout);
            }
            catch { }
        }

        /// <summary>
        /// Get a cookie by a name
        /// </summary>
        /// <param name="name">name of a cookie</param>
        /// <returns></returns>
        public async Task<string> GetCookie(string cookieName)
        {
            try
            {
                return await _js.InvokeAsync<string>("CookiesService.Get", cookieName);
            }
            catch 
            {
                return null;
            }
        }

        /// <summary>
        /// Get a cookie by a name
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        /// <param name="experienceDays"></param>
        /// <returns></returns>
        public async Task SetCookie(string cookieName, string cookieValue, int experienceDays)
        {
            try
            {
                await _js.InvokeAsync<string>("CookiesService.Set", cookieName, cookieValue, experienceDays);
            }
            catch { }
        }

        /// <summary>
        /// Get a cookie by a name
        /// </summary>
        /// <param name="name">name of the erasing cookie</param>
        /// <returns></returns>
        public async Task EraseCookie(string cookieName)
        {
            try
            {
                await _js.InvokeAsync<string>("CookiesService.Erase", cookieName);
            }
            catch { }
        }
    }
}
