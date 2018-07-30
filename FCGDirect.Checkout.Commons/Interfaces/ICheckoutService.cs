using FCGDirect.Checkout.Commons.Classes;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FCGDirect.Checkout.Commons.Interfaces
{
    public interface ICheckoutService : IService
    {
        Task<CheckoutSummary> CheckoutQuote(string userId);
        Task<List<CheckoutSummary>> GetOrderHistory(string userId);
    }
}
