using System;
using System.Collections.Generic;
using System.Text;

namespace FCGDirect.Policy.Commons.Classes
{
    public class PolicyArtifact
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public double Price { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
