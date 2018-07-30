using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCGDirect.WebApi.ViewModels
{
    public class PolicyQuoteViewModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("number")]
        public int Number { get; set; }
        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }
}
