using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Web.Framework.Components.Services
{
    /// <summary>
    /// (Porperties in the lower case because they are exploited as the JS-object properties)
    /// </summary>
    public interface IJSNotificationMessage
    {
        /// <summary>
        /// Result of the notificationing porcess
        /// </summary>
        bool success { get; set; }
        /// <summary>
        /// Message
        /// </summary>
        string[] message { get; set; }
    }
}
