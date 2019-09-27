using spaCommerce.Models.Newsletter;

namespace spaCommerce.Factories
{
    /// <summary>
    /// Represents the interface of the newsletter model factory
    /// </summary>
    public partial interface INewsletterModelFactory
    {
        /// <summary>
        /// Prepare the newsletter box model
        /// </summary>
        /// <returns>Newsletter box model</returns>
        NewsletterBoxModel PrepareNewsletterBoxModel();

        /// <summary>
        /// Prepare the subscription activation model
        /// </summary>
        /// <param name="active">Whether the subscription has been activated</param>
        /// <returns>Subscription activation model</returns>
        SubscriptionActivationModel PrepareSubscriptionActivationModel(bool active);
    }
}
