using System.Collections.Generic;

namespace TestClient
{
    public class TestCase
    {
        public string Description { get; set; }
        public string TokenEndpoint { get; set; }

        public ICollection<Api> Apis { get; set; } = new HashSet<Api>();
    }
}
