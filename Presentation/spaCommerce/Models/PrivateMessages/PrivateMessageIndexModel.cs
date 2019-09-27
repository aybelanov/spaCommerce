using Nop.Web.Framework.Models;

namespace spaCommerce.Models.PrivateMessages
{
    public partial class PrivateMessageIndexModel : BaseNopModel
    {
        public int InboxPage { get; set; }
        public int SentItemsPage { get; set; }
        public bool SentItemsTabSelected { get; set; }
    }
}