using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCGDirect.WebApi.ViewModels;
using FCGDirect.Policy.Commons.Classes;
using FCGDirect.Policy.Commons.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace FCGDirect.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PolicyArtifactController : Controller
    {
        private readonly IPolicyArtifactService _service;

        public PolicyArtifactController()
        {
            var proxyFactory = new ServiceProxyFactory((c) => new FabricTransportServiceRemotingClientFactory(
               serializationProvider: new GenericDataProvider(new List<Type> { typeof(bool), typeof(List<PolicyArtifact>), typeof(PolicyArtifact) })));

            _service = proxyFactory.CreateServiceProxy<IPolicyArtifactService>(new Uri("fabric:/FCGDirect/FCGDirect.Policy"), new ServicePartitionKey(0));

        }
   
        [HttpGet]
        public async Task<IEnumerable<PolicyQuoteViewModel>> Get()
        {
            return (await _service.GellAll()).Select(x => new PolicyQuoteViewModel { Id = x.Id, CreationDate = x.CreationDate, Number = x.Number, IsActive = x.IsActive });
        }

        [HttpGet("{id}")]
        public async Task<PolicyQuoteViewModel> Get(Guid id)
        {
            var result = await _service.GetPolicyQuoteById(id);

            return new PolicyQuoteViewModel { Id = result.Id, CreationDate = result.CreationDate, IsActive = result.IsActive, Number = result.Number };
        }

        [HttpGet("number/{id}")]
        public async Task<PolicyQuoteViewModel> GetByNumber(int id)
        {
            var result = await _service.GetPolicyQuoteByNumber(id);

            return new PolicyQuoteViewModel { Id = result.Id, CreationDate = result.CreationDate, IsActive = result.IsActive, Number = result.Number };
        }

        // POST api/values
        [HttpPost]
        public async Task<bool> Post([FromBody]PolicyQuoteViewModel policyquote)
        {
            try
            {
                var policy =  new PolicyArtifact
                {
                    Number = policyquote.Number,
                    CreationDate = DateTime.Now,
                    IsActive = policyquote.IsActive,
                    Id = Guid.NewGuid()
                };

                return await _service.AddPolicyQuote(policy);


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
