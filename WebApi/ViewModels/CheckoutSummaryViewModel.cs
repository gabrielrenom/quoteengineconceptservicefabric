using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class CheckoutSummaryViewModel
    {
        [JsonProperty("artifacts")]
        public List<CheckoutArtifactViewModel> Artifacts { get; set; }

        [JsonProperty("totalPrice")]
        public double TotalPrice { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

    }

    public class CheckoutArtifactViewModel
    {
        [JsonProperty("artifactId")]
        public Guid ArtifactId { get; set; }

        [JsonProperty("artifactName")]
        public string ArtifactName { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }

}
