using FCGDirect.Policy.Commons.Classes;

namespace FCGDirect.Checkout.Commons.Classes
{
    public class CheckoutArtifact
    {
        public PolicyArtifact Artifact { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}