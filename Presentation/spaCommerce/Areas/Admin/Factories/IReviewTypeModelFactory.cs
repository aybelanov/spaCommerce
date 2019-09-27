using Nop.Core.Domain.Catalog;
using spaCommerce.Areas.Admin.Models.Catalog;

namespace spaCommerce.Areas.Admin.Factories
{
    /// <summary>
    /// Represents a review type model factory
    /// </summary>
    public partial interface IReviewTypeModelFactory
    {
        /// <summary>
        /// Prepare review type search model
        /// </summary>
        /// <param name="searchModel">Review type search model</param>
        /// <returns>Review type search model</returns>
        ReviewTypeSearchModel PrepareReviewTypeSearchModel(ReviewTypeSearchModel searchModel);

        /// <summary>
        /// Prepare paged review type list model
        /// </summary>
        /// <param name="searchModel">Review type search model</param>
        /// <returns>Review type list model</returns>
        ReviewTypeListModel PrepareReviewTypeListModel(ReviewTypeSearchModel searchModel);

        /// <summary>
        /// Prepare review type model
        /// </summary>
        /// <param name="model">Review type model</param>
        /// <param name="reviewType">Review type</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Review type model</returns>
        ReviewTypeModel PrepareReviewTypeModel(ReviewTypeModel model,
            ReviewType reviewType, bool excludeProperties = false);
    }
}
