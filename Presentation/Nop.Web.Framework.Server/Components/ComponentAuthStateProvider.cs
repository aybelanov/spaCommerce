using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Server.Components
{
    public class ComponentAuthStateProvider : RevalidatingServerAuthenticationStateProvider
    {
        private readonly IWorkContext _workContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Nop.Services.Logging.ILogger _logger;

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        public ComponentAuthStateProvider(IWorkContext workContext, IServiceScopeFactory scopeFactory,
            Nop.Services.Logging.ILogger logger, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            this._workContext = workContext;
            this._scopeFactory = scopeFactory;
            this._logger = logger;
        }

        protected override async Task<bool> ValidateAuthenticationStateAsync(
             AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            var scope = _scopeFactory.CreateScope();
            try
            {
                return await CheckIfAuthenticationStateIsValidAsync(authenticationState.User, scope);
            }
            finally
            {
                if (scope is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    scope.Dispose();
                }
            }
        }

        private Task<bool> CheckIfAuthenticationStateIsValidAsync(ClaimsPrincipal principal, IServiceScope scope)
        {
            try
            {
                var attributeService = scope.ServiceProvider.GetRequiredService<IGenericAttributeService>();
                var customerSettings = scope.ServiceProvider.GetRequiredService<CustomerSettings>();
                var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();

                Customer customer = null;
                if (customerSettings.UsernamesEnabled)
                {
                    //try to get customer by username
                    var usernameClaim = principal.FindFirst(claim => claim.Type == ClaimTypes.Name
                        && claim.Issuer.Equals(NopAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                    if (usernameClaim != null)
                        customer = customerService.GetCustomerByUsername(usernameClaim.Value);
                }
                else
                {
                    //try to get customer by email
                    var emailClaim = principal.FindFirst(claim => claim.Type == ClaimTypes.Email
                        && claim.Issuer.Equals(NopAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                    if (emailClaim != null)
                        customer = customerService.GetCustomerByEmail(emailClaim.Value);
                }

                //whether the found customer is available
                if (customer == null || !customer.Active || customer.RequireReLogin || customer.Deleted || !customer.IsRegistered())
                    return Task.FromResult(false);

                var currentAuthState = attributeService.GetAttribute<bool?>(customer, NopCustomerDefaults.AuthenticationStateAttribute);

                return Task.FromResult(currentAuthState.GetValueOrDefault());
            }
            catch (Exception ex)
            {
                _logger.Warning("An error occurred while revalidating authentication state", ex);
                return Task.FromResult(false);
            }
        }
    }
}