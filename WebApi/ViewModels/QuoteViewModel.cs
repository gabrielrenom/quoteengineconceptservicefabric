using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
 

    public class QuoteViewModel
    {
        [JsonProperty("userId")]
        public string QuoteId { get; set; }

        [JsonProperty("item")]
        public QuoteItemViewModel[] Items { get; set; }
    }



    public class QuoteItemViewModel
    {
        [JsonProperty("policyArtifactid")]
        public string PolicyArtifactId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }

    public class QuoteItemRequestViewModel
    {
        [JsonProperty("policyArtifactid")]
        public Guid PolicyArtifactId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
