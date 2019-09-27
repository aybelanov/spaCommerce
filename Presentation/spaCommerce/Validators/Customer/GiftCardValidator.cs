using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using spaCommerce.Models.Customer;

namespace spaCommerce.Validators.Customer
{
    public partial class GiftCardValidator : BaseNopValidator<CheckGiftCardBalanceModel>
    {
        public GiftCardValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.GiftCardCode).NotEmpty().WithMessage(localizationService.GetResource("CheckGiftCardBalance.GiftCardCouponCode.Empty"));            
        }
    }
}
