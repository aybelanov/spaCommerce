using Nop.Web.Framework.Models;

namespace spaCommerce.Areas.Admin.Models.Polls
{
    /// <summary>
    /// Represents a poll answer search model
    /// </summary>
    public partial class PollAnswerSearchModel : BaseSearchModel
    {
        #region Properties

        public int PollId { get; set; }

        #endregion
    }
}