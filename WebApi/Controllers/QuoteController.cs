using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCGDirect.Profile.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class QuoteController : Controller
    {
        [HttpGet("{quoteId}")]
        public async Task<QuoteViewModel> Get(string quoteId)
        {
            IProfile actor = GetActor(quoteId);

            Dictionary<Guid, int> products = await actor.GetQuote();

            return new QuoteViewModel
            {
                QuoteId = quoteId,
                Items = products.Select(p => new QuoteItemViewModel {  PolicyArtifactId = p.Key.ToString(), Quantity = p.Value }).ToArray()
            };
        }

        [HttpPost("{quoteId}")]
        public async Task Add(string quoteId, [FromBody]QuoteItemRequestViewModel request)
        {
            IProfile actor = GetActor(quoteId);

            await actor.AddToQuote(request.PolicyArtifactId, request.Quantity);

        }

        [HttpDelete("{quoteId}")]
        public async Task Delete(string quoteId)
        {
            IProfile actor = GetActor(quoteId);

            await actor.ClearQuote();
        }

        private IProfile GetActor(string quoteId)
        {
            return ActorProxy.Create<IProfile>(new ActorId(quoteId), new Uri("fabric:/FCGDirect/ProfileActorService"));
        }
    }
}