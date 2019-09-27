using System.Threading.Tasks;

namespace Nop.Web.Framework.Components.Services
{
    public interface IJSService
    {
        /// <summary>
        /// Get the alert windiw with the pointed message
        /// </summary>
        /// <param name="message">message</param>
        /// <returns></returns>
        Task Alert(string message);
        /// <summary>
        /// Display/hide a load waiting stub
        /// </summary>
        /// <param name="display">true - display; false - hide;</param>
        Task DisplayAjaxLoading(bool display);
        /// <summary>
        /// Display a bar notification
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="timeout">timeout delay the disappearance, seconds</param>
        /// <returns></returns>
        Task DisplayBarNotification(string[] message, JSMessageType messageType = JSMessageType.Succes, int timeout = 0);
        /// <summary>
        /// Display a popup content from the pointed url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="modal">if the window should be shown as modal one</param>
        /// <param name="width"></param>
        /// <returns></returns>
        Task DisplayPopupContentFromUrl(string url, string title = null, bool? modal = null, int? width = null);
        /// <summary>
        /// Display a popup content with the pointed content
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="modal">if the window should be shown as modal one</param>
        /// <param name="width"></param>
        /// <returns></returns>
        Task DisplayPopupContent(string content, string title = null, bool? modal = null, int? width = null);
        /// <summary>
        /// Display a popup notification
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="modal"></param>
        /// <returns></returns>
        Task DisplayPopupNotification(string[] message, JSMessageType messageType, bool modal);
        /// <summary>
        /// Get a cookie by a name
        /// </summary>
        /// <param name="name">name of the erasing cookie</param>
        /// <returns></returns>
        Task EraseCookie(string cookieName);
        /// <summary>
        /// Get a cookie by a name
        /// </summary>
        /// <param name="name">name of a cookie</param>
        /// <returns></returns>
        Task<string> GetCookie(string cookieName);
        /// <summary>
        /// Open a modal window wicth content from the query
        /// </summary>
        /// <param name="query">url/query for window content</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="scroll">enable/disable scrolling</param>
        Task OpenWindow(string query, int width, int height, bool scroll);
        /// <summary>
        /// Hides a native load waiting stub
        /// </summary>
        /// <returns></returns>
        Task ResetLoadWaiting();
        /// <summary>
        /// Get a cookie by a name
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        /// <param name="experienceDays"></param>
        /// <returns></returns>
        Task SetCookie(string cookieName, string cookieValue, int experienceDays);
        /// <summary>
        /// Shows a native load waiting stub
        /// </summary>
        /// <returns></returns>
        Task ShowLoadWaiting();
        /// <summary>
        /// Shows messages according the buying process results
        /// </summary>
        /// <param name="message">Messages</param>
        /// <returns></returns>
        Task ShowNotifications(IJSNotificationMessage message);
    }
}