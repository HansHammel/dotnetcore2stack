//namespace JSONUtils
//{

//    [DataContract]
//    public class DnsEndpoint
//    {

//        [DataMember(Name = "Address")]
//        [JsonProperty("Address")]
//        public string Address { get; set; }

//        [DataMember(Name = "Port")]
//        [JsonProperty("Port")]
//        public int Port { get; set; }
//    }

//    [DataContract]
//    public class Consul
//    {
//        [DataMember(Name = "HttpEndpoint")]
//        [JsonProperty("HttpEndpoint")]
//        public string HttpEndpoint { get; set; }

//        [DataMember(Name = "DnsEndpoint")]
//        [JsonProperty("DnsEndpoint")]
//        public DnsEndpoint DnsEndpoint { get; set; }
//    }

//    [DataContract]
//    public class LogLevel
//    {
//        [DataMember(Name = "Default")]
//        [JsonProperty("Default")]
//        public string Default { get; set; }
//    }

//    [DataContract]
//    public class Logging
//    {
//        [DataMember(Name = "IncludeScopes")]
//        [JsonProperty("IncludeScopes")]
//        public bool IncludeScopes { get; set; }

//        [DataMember(Name = "LogLevel")]
//        [JsonProperty("LogLevel")]
//        public LogLevel LogLevel { get; set; }
//    }

//    [DataContract]
//    public class Console
//    {
//        [DataMember(Name = "LogLevel")]
//        [JsonProperty("LogLevel")]
//        public LogLevel {get; set;} 
//}


//[DataContract]
//public class ServiceDiscovery
//{
//    [DataMember(Name = "ServiceName")]
//    [JsonProperty("ServiceName")]
//    public string ServiceName { get; set; }

//    [DataMember(Name = "Version")]
//    [JsonProperty("Version")]
//    public string Version { get; set; }

//    [DataMember(Name = "Description")]
//    [JsonProperty("Description")]
//    public string Description { get; set; }

//    [DataMember(Name = "HealthCheckTemplate")]
//    [JsonProperty("HealthCheckTemplate")]
//    public string HealthCheckTemplate { get; set; }

//    [DataMember(Name = "Endpoints")]
//    [JsonProperty("Endpoints")]
//    public IList<object> Endpoints { get; set; }

//    [DataMember(Name = "Logging")]
//    [JsonProperty("Logging")]
//    public Logging Logging { get; set; }

//    [DataMember(Name = "Console")]
//    [JsonProperty("Console")]
//    public Console Console { get; set; }
//}

//[DataContract]
//public class Example
//{
//    [DataMember(Name = "Consul")]
//    [JsonProperty("Consul")]
//    public Consul Consul { get; set; }

//    [DataMember(Name = "ServiceDiscovery")]
//    [JsonProperty("ServiceDiscovery")]
//    public ServiceDiscovery ServiceDiscovery { get; set; }
//}


