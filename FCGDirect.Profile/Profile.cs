using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using FCGDirect.Profile.Interfaces;

namespace FCGDirect.Profile
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class Profile : Actor, IProfile
    {
        /// <summary>
        /// Initializes a new instance of Profile
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public Profile(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        public async Task AddToQuote(Guid policyId, int quantity)
        {
            await StateManager.AddOrUpdateStateAsync(
               policyId.ToString(),
               quantity,
               (id, oldQuantity) => oldQuantity + quantity);
        }

        public async Task ClearQuote()
        {
            IEnumerable<string> policyids = await StateManager.GetStateNamesAsync();

            foreach (string policyid in policyids)
            {
                await StateManager.RemoveStateAsync(policyid);
            }
        }

        public async Task<Dictionary<Guid, int>> GetQuote()
        {
            var result = new Dictionary<Guid, int>();

            IEnumerable<string> policyids = await StateManager.GetStateNamesAsync();

            foreach (string policyid in policyids)
            {
                int quantity = await StateManager.GetStateAsync<int>(policyid);
                result[new Guid(policyid)] = quantity;
            }

            return result;
        }
    }
}
