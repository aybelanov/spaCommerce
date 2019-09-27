using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Components.Infrastructure
{
    public delegate Task PaymentInfoOnValidSubmit(ProcessPaymentRequest paymentInfo);
}
