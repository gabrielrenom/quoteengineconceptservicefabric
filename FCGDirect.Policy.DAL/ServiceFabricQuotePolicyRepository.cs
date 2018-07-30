using FCGDirect.Policy.Commons.Classes;
using FCGDirect.Policy.Commons.Interfaces;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace FCGDirect.Policy.DAL
{
    public class ServiceFabricQuotePolicyRepository : IPolicyArtifactRepository
    {
        private readonly IReliableStateManager _stateManager;

        public ServiceFabricQuotePolicyRepository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public async Task<bool> AddPolicyQuote(PolicyArtifact policy)
        {
            try
            {
                var policies = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, PolicyArtifact>>("policyquotes");

                using (var transaction = _stateManager.CreateTransaction())
                {
                    await policies.AddOrUpdateAsync(transaction, policy.Id, policy, (id, value) => policy);
                    await transaction.CommitAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<PolicyArtifact>> GellAll()
        {

            var policies = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, PolicyArtifact>>("policyquotes");
            var result = new List<PolicyArtifact>();

            using (var tx = _stateManager.CreateTransaction())
            {
                var Allpolicies =  (await policies.CreateEnumerableAsync(tx, EnumerationMode.Ordered)).GetAsyncEnumerator();

                using (var enumerator = Allpolicies)
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, PolicyArtifact> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }

            return result;
        }

        public async Task<PolicyArtifact> GetPolicyQuoteById(Guid id)
        {
            var policies = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, PolicyArtifact>>("policyquotes");

            using (var tx = _stateManager.CreateTransaction())
            {
                ConditionalValue<PolicyArtifact> product = await policies.TryGetValueAsync(tx, id);

                return product.HasValue ? product.Value : null;
            }
        }

        public async Task<PolicyArtifact> GetPolicyQuoteByNumber(int number)
        {
            var policies = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, PolicyArtifact>>("policyquotes");
            var result = new List<PolicyArtifact>();

            using (var tx = _stateManager.CreateTransaction())
            {
                var Allpolicies = (await policies.CreateEnumerableAsync(tx, EnumerationMode.Ordered)).GetAsyncEnumerator();

                using (var enumerator = Allpolicies)
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, PolicyArtifact> current = enumerator.Current;

                        if (current.Value.Number == number) return new PolicyArtifact { Id = current.Value.Id, CreationDate = current.Value.CreationDate, Number = current.Value.Number, IsActive = current.Value.IsActive };
                    }
                }
            }

            return null;
        }
    }
}
