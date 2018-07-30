using System;
using System.Collections.Generic;
using System.Text;

namespace FCGDirect.Checkout.Commons.Classes
{
    public class CheckoutSummary
    {

        public List<CheckoutArtifact> Artifacts { get; set; }
        public double TotalPrice { get; set; }
        public DateTime Date { get; set; }
    }
}
