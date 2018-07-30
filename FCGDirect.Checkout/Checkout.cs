using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FCGDirect.Checkout.Commons.Classes;
using FCGDirect.Checkout.Commons.Interfaces;
using FCGDirect.Policy.Commons.Classes;
using FCGDirect.Policy.Commons.Interfaces;
using FCGDirect.Profile.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace FCGDirect.Checkout
{
    internal sealed class Checkout : StatefulService, ICheckoutService
    {
        public Checkout(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<CheckoutSummary> CheckoutQuote(string userId)
        {
            var result = new CheckoutSummary();
            result.Date = DateTime.UtcNow;
            result.Artifacts = new List<CheckoutArtifact>();

            //call user actor to get the basket
            IProfile userActor = GetUserActor(userId);
            Dictionary<Guid, int> basket = await userActor.GetQuote();

            //get catalog client
            IPolicyArtifactService catalogService = GetPolicyArtifactService();

            //constuct CheckoutProduct items by calling to the catalog
            foreach (KeyValuePair<Guid, int> basketLine in basket)
            {
                PolicyArtifact artifact = await catalogService.GetPolicyQuoteById(basketLine.Key);
                var checkoutProduct = new CheckoutArtifact
                {
                     Artifact = artifact,
                    Price = artifact.Price,
                    Quantity = basketLine.Value
                };
                result.Artifacts.Add(checkoutProduct);
            }

            //generate total price
            result.TotalPrice = result.Artifacts.Sum(p => p.Price);

            //clear user basket
            await userActor.ClearQuote();

            await AddToHistory(result);

            return result;
        }

        public async Task<List<CheckoutSummary>> GetOrderHistory(string userId)
        {
            var result = new List<CheckoutSummary>();
            var history = await StateManager.GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>("history");

            using (var tx = StateManager.CreateTransaction())
            {
                var allProducts = await history.CreateEnumerableAsync(tx, EnumerationMode.Unordered);
                using (var enumerator = allProducts.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<DateTime, CheckoutSummary> current = enumerator.Current;

                        result.Add(current.Value);
                    }
                }
            }

            return result;
        }

        private async Task AddToHistory(CheckoutSummary checkout)
        {
            var history = await StateManager.GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>("history");

            using (var tx = StateManager.CreateTransaction())
            {
                await history.AddAsync(tx, checkout.Date, checkout);

                await tx.CommitAsync();
            }
        }

        private IProfile GetUserActor(string userId)
        {
            return ActorProxy.Create<IProfile>(new ActorId(userId), new Uri("fabric:/FCGDirect/ProfileActorService"));
        }

        private IPolicyArtifactService GetPolicyArtifactService()
        {

            var proxyFactory = new ServiceProxyFactory((c) => new FabricTransportServiceRemotingClientFactory(
               serializationProvider: new GenericDataProvider(new List<Type> { typeof(PolicyArtifact), typeof(List<PolicyArtifact>) })));

            var artifactservice = proxyFactory.CreateServiceProxy<IPolicyArtifactService>(new Uri("fabric:/FCGDirect/FCGDirect.Policy"), new ServicePartitionKey(0));


            return (artifactservice);
        }


        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener((context) =>
                {
                    return new FabricTransportServiceRemotingListener(context, this, serializationProvider: new GenericDataProvider(new List<Type>{ typeof(CheckoutSummary), typeof(List<CheckoutSummary>)}));
                })
            };
        }

        
    }
}
