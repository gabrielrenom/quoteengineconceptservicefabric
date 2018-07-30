using FCGDirect.Policy.Commons.Classes;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FCGDirect.Policy.Commons.Interfaces
{
    public interface IPolicyArtifactService:IService
    {
        Task<bool> AddPolicyQuote(PolicyArtifact policy);
        Task<List<PolicyArtifact>> GellAll();
        Task<PolicyArtifact> GetPolicyQuoteByNumber(int number);
        Task<PolicyArtifact> GetPolicyQuoteById(Guid id);
    }
}
