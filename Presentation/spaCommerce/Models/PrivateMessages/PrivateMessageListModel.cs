using System.Collections.Generic;
using Nop.Web.Framework.Models;
using spaCommerce.Models.Common;

namespace spaCommerce.Models.PrivateMessages
{
    public partial class PrivateMessageListModel : BaseNopModel
    {
        public IList<PrivateMessageModel> Messages { get; set; }
        public PagerModel PagerModel { get; set; }
    }
}