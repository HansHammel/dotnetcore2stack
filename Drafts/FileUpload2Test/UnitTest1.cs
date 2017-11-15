using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using FileUpload2;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using RestSharp;
using Xunit;

namespace FileUpload2Test
{
    /*
    public class TestFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly TestServer _testServer;
        public HttpClient HttpClient { get; }

        public TestFixture()
        {
            var webHostBuilder = new WebHostBuilder().UseStartup<TStartup>();
            _testServer = new TestServer(webHostBuilder);

            HttpClient = _testServer.CreateClient();
            HttpClient.BaseAddress = new Uri("http://localhost:58834");
        }

        public void Dispose()
        {
            HttpClient.Dispose();
            _testServer.Dispose();
        }
    }
    */

    public class CreateUploadFilesFixture : IDisposable
    {

        public string[] Files { get; private set; }

        // only be run once
        public CreateUploadFilesFixture()
        {
            //cleanup
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (File.Exists(Path.Combine(path, "test1.bak"))) File.Delete(Path.Combine(path, "test1.bak"));
            if (File.Exists(Path.Combine(path, "test1.bak"))) File.Delete(Path.Combine(path, "test2.bak"));

            Files = new string[] { Path.GetTempFileName(), Path.GetTempFileName() };
        }

        public string[] GetTestUploadFiles()
        {
            foreach (string file in Files)
            {
                // Create Files
                using (var fs = new FileStream(file, FileMode.Truncate, FileAccess.ReadWrite, FileShare.None))
                {
                    var sizeInMB = 20;
                    fs.SetLength(sizeInMB * 1024 * 1024);
                }
            }
            return Files;
        }

        public void Dispose()
        {
            // Delete Files
            foreach (string file in Files)
            {
                if (File.Exists(file)) File.Delete(file); 
            }
        }
    }

    public class UnitTest1: IClassFixture<CreateUploadFilesFixture>, IDisposable
    {
        //private readonly TestServer _server;
        //private readonly HttpClient _client;

        CreateUploadFilesFixture fixture;

        public UnitTest1(CreateUploadFilesFixture fixture)
        {
            //_server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            //_client = _server.CreateClient();
            this.fixture = fixture;
        }


        public void Dispose()
        {
            //throw new NotImplementedException();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (File.Exists(Path.Combine(path, "test1.bak"))) File.Delete(Path.Combine(path, "test1.bak"));
            if (File.Exists(Path.Combine(path, "test1.bak"))) File.Delete(Path.Combine(path, "test2.bak"));
        }

        [Fact]
        public async void Test1()
        {
            var webHostBuilder = new WebHostBuilder().UseStartup<Startup>()
               .UseKestrel()
           //.UseContentRoot(Directory.GetCurrentDirectory())
           .UseIISIntegration()
           .UseStartup<Startup>()
           .UseUrls("http://localhost:5001/");

            using (var host = new TestServer(webHostBuilder))
            {
                
                using (var _client = host.CreateClient())
                {
                    var _response = await _client.GetAsync("http://localhost:5001/api/v1/download");
                    Assert.Equal(HttpStatusCode.OK, _response.StatusCode);

                    
                    using (var multipartFormDataContent = new MultipartFormDataContent())
                    {
                        /*
                        var values = new[]
                        {
                            new KeyValuePair<string, string>("name", "somename")
                        };

                        foreach (var keyValuePair in values)
                        {
                            multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                                String.Format("\"{0}\"", keyValuePair.Key));
                        }
                        */

                        multipartFormDataContent.Add(new ByteArrayContent(File.ReadAllBytes(fixture.GetTestUploadFiles()[0])),
                            "files",
                            "test1.bak");
                        multipartFormDataContent.Add(new ByteArrayContent(File.ReadAllBytes(fixture.GetTestUploadFiles()[1])),
                            "files",
                            "test2.bak");

                        var requestUri = "http://localhost:5001/api/v1/fileuploads";
                        var result = _client.PostAsync(requestUri, multipartFormDataContent).Result;
                        Assert.Equal(HttpStatusCode.Found, result.StatusCode);
                        var path = Path.Combine(
            Directory.GetCurrentDirectory(), "wwwroot");
                        Assert.True(File.Exists(Path.Combine(path, "test1.bak")));
                        Assert.True(File.Exists(Path.Combine(path, "test2.bak")));
                    }

                    /*
                    var imageContent = new ByteArrayContent(ImageData);
                    imageContent.Headers.ContentType =
                        MediaTypeHeaderValue.Parse("image/jpeg");

                    requestContent.Add(imageContent, "image", "image.jpg");

                    return await _client.PostAsync("http://localhost:5001/api/v1/fileuploads", requestContent);
                    */
                }

                /*
                 * //TODO: RestSharp not working?
                RestClient restClient = new RestClient("http://localhost:5001/");
                // restClient.Authenticator = new HttpBasicAuthenticator(username, password);
                RestRequest restRequest = new RestRequest("/api/v1/fileuploads");
                restRequest.RequestFormat = DataFormat.Json;
                restRequest.Method = Method.POST;
                //restRequest.AddHeader("Authorization", "Authorization");
                restRequest.AddHeader("Content-Type", "multipart/form-data");
                restRequest.AddFile("files", fixture.GetTestUploadFiles()[0]);
                restRequest.AddFile("files", fixture.GetTestUploadFiles()[1]);
                var response = restClient.Execute(restRequest);
                Assert.Equal(RestSharp.ResponseStatus.Completed, response.ResponseStatus);
                var content = response.Content;
                Assert.NotEmpty(content);
                */

                //request.AddParameter("request", tokenRequest.ToJson());
                //request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
                //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource
                // add parameters for all properties on an object
                //request.AddObject(object);
                // or just whitelisted properties
                //request.AddObject(object, "PersonId", "Name", ...);
                // easily add HTTP Headers
                //request.AddHeader("header", "value");
                // execute the request
                //IRestResponse response = client.Execute(request);
                //Assert.Equal(RestSharp.ResponseStatus.Completed, response.ResponseStatus);
                //var content = response.Content; // raw content as string
                //Assert.NotEmpty(content);
                //Console.WriteLine(content);



                // or automatically deserialize result
                // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
                //IRestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // or download and save file to disk
                //client.DownloadData(request).SaveAs(path);

                // easy async support
                //await client.ExecuteAsync(request);

                // async with deserialization
                //var asyncHandle = client.ExecuteAsync<Person>(request, response => {
                //    Console.WriteLine(response.Data.Name);
                //});

                // abort the request on demand
                //asyncHandle.Abort();
            }

        }
    }
}
