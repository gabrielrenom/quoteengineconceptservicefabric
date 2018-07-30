using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FCGDirect.Policy.Commons.Classes;
using FCGDirect.Policy.Commons.Interfaces;
using FCGDirect.Policy.DAL;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace FCGDirect.Policy
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Policy : StatefulService, IPolicyArtifactService
    {
        private readonly IPolicyArtifactRepository _repository;

        public Policy(StatefulServiceContext context)
            : base(context)
        {
            _repository = new ServiceFabricQuotePolicyRepository(this.StateManager);
        }

        public async Task<bool> AddPolicyQuote(PolicyArtifact policy)=> await _repository.AddPolicyQuote(policy);

        public async Task<List<PolicyArtifact>> GellAll() => await _repository.GellAll();

        public async Task<PolicyArtifact> GetPolicyQuoteById(Guid id) => await _repository.GetPolicyQuoteById(id);

        public async Task<PolicyArtifact> GetPolicyQuoteByNumber(int number)=> await _repository.GetPolicyQuoteByNumber(number);

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener((context) =>
                {
                    return new FabricTransportServiceRemotingListener(context, this, serializationProvider: new GenericDataProvider(new List<Type>{ typeof(bool), typeof(List<PolicyArtifact>), typeof(PolicyArtifact) }));
                })
            };
        }

    }
}
