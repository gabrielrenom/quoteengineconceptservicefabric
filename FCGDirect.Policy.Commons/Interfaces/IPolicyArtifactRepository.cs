using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FCGDirect.Policy.Commons.Classes;

namespace FCGDirect.Policy.Commons.Interfaces
{
    public interface IPolicyArtifactRepository
    {
        Task<bool> AddPolicyQuote(PolicyArtifact policy);
        Task<List<PolicyArtifact>> GellAll();
        Task<PolicyArtifact> GetPolicyQuoteByNumber(int number);
        Task<PolicyArtifact> GetPolicyQuoteById(Guid id);
    }
}
