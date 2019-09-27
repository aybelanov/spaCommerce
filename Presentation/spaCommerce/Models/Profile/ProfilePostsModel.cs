using System.Collections.Generic;
using Nop.Web.Framework.Models;
using spaCommerce.Models.Common;

namespace spaCommerce.Models.Profile
{
    public partial class ProfilePostsModel : BaseNopModel
    {
        public IList<PostsModel> Posts { get; set; }
        public PagerModel PagerModel { get; set; }
    }
}