using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using spaCommerce.Models.Boards;

namespace spaCommerce.Validators.Boards
{
    public partial class EditForumPostValidator : BaseNopValidator<EditForumPostModel>
    {
        public EditForumPostValidator(ILocalizationService localizationService)
        {            
            RuleFor(x => x.Text).NotEmpty().WithMessage(localizationService.GetResource("Forum.TextCannotBeEmpty"));
        }
    }
}