using System;
using spaCommerce.Models.Newsletter;

namespace spaCommerce.Services
{
    public interface INewsletterComponentService
    {
        (bool Success, string Result) SubscribeNewsletter(string email, bool subscribe);
        SubscriptionActivationModel SubscriptionActivation(Guid token, bool active);
    }
}