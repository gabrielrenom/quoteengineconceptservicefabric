using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCGDirect.Checkout.Commons.Classes;
using FCGDirect.Checkout.Commons.Interfaces;
using FCGDirect.Policy.Commons.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CheckoutController : Controller
    {
        private static readonly Random rnd = new Random(DateTime.UtcNow.Second);

        [Route("{quoteId}")]
        public async Task<CheckoutSummaryViewModel> Checkout(string quoteId)
        {

            var service = GetCheckoutService();

            CheckoutSummary summary = await service.CheckoutQuote(quoteId);

            return ToApiCheckoutSummary(summary);
        }

        [Route("history/{quoteId}")]
        public async Task<IEnumerable<CheckoutSummaryViewModel>> GetHistory(string quoteId)
        {
            IEnumerable<CheckoutSummary> history = await GetCheckoutService().GetOrderHistory(quoteId);

            return history.Select(ToApiCheckoutSummary);
        }


        private CheckoutSummaryViewModel ToApiCheckoutSummary(CheckoutSummary model)
        {
            return new CheckoutSummaryViewModel
            {
                Artifacts = model.Artifacts.Select(p => new CheckoutArtifactViewModel
                {
                     ArtifactId = p.Artifact.Id,
                     ArtifactName = p.Artifact.Type,
                    Price = p.Price,
                    Quantity = p.Quantity
                }).ToList(),
                Date = model.Date,
                TotalPrice = model.TotalPrice
            };
        }

        private ICheckoutService GetCheckoutService()
        {
            long key = LongRandom();

            var proxyFactory = new ServiceProxyFactory((c) => new FabricTransportServiceRemotingClientFactory(
               serializationProvider: new GenericDataProvider(new List<Type> { typeof(CheckoutSummary), typeof(List<CheckoutSummary>) })));

            var checkoutService = proxyFactory.CreateServiceProxy<ICheckoutService>(new Uri("fabric:/FCGDirect/FCGDirect.Checkout"), new ServicePartitionKey(key));

            return checkoutService;
        }

        private long LongRandom()
        {
            byte[] buf = new byte[8];
            rnd.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return longRand;
        }
    }
}