using System.Net;
using System.Reflection;

namespace IS4ProtectedAPI
{
    public class ServiceDisvoveryOptions
    {
        public string ServiceName { get; set; }

        public ConsulOptions Consul { get; set; }
    }

    public class ConsulOptions
    {
        public string HttpEndpoint { get; set; }

        public DnsEndpoint DnsEndpoint { get; set; }
    }

    public class DnsEndpoint
    {
        public string Address { get; set; }

        public int Port { get; set; }

        public IPEndPoint ToIPEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(Address), Port);
        }
    }


    public class ServiceConfiguartion
    {
        private string _transport;
        private int _port;
        private string _host;
        private string _id;

        public int HttpPort { get; }
        public int HttpsPort { get; }
        public string Transport { get; }
        public string Host { get; }
        public string URL { get { return _transport + "://" + _host + ":" + _port; } }
        public string ListeningURL { get { return _transport + "://*:" + _port; } }
        public string ServiceID { get { return _id; } }
        public string Name
        {
            get
            {
                var a = Assembly.GetEntryAssembly();
                return a.GetName().Name + " v" + a.GetName().Version;
            }
        }

        public ServiceConfiguartion(int port) : this("http", "localhost", port) { }
        public ServiceConfiguartion(string transport = "http", string host = "localhost", int port = 5000)
        {
            HttpPort = _port = port;
            Host = _host = host;
            Transport = _transport = transport;
            _id = System.Guid.NewGuid().ToString();
        }
    }

    /*
    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            //.UseConfiguration(config)
            //.UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((builderContext, config) =>
            {
                IHostingEnvironment env = builderContext.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            })
            //.UseIISIntegration()
            .CaptureStartupErrors(true)
            // bind to all nics using specific port
            .UseUrls(serviceConfiguration.ListeningURL)
            .UseDefaultServiceProvider((context, options) =>
            {
                options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
            })
            .UseStartup<Startup>()
            .Build();
    */
}

