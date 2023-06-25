using HTTP;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HTTPClient
{
    // https://www.codeproject.com/Articles/7859/Building-COM-Objects-in-C

    [Guid("A658E340-5B37-4590-9E7D-75603124368E")]
    public interface HttpRequest_Interface
    {
        [DispId(1)]
        HttpRequest BasicAuthentication(string user, string pass);
        [DispId(2)]
        HttpRequest TokenAuthentication(string token);


    }

    [Guid("31F9098A-300C-4FE7-BE95-0F90E78C1BAA"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface HttpRequest_Events
    {

    }
    [Guid("4201F529-558E-4412-A526-FD0779695D6A"),
    ClassInterface(ClassInterfaceType.None),
    ComSourceInterfaces(typeof(HttpRequest_Events))]
    public class HttpRequest : HttpRequest_Interface
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private HttpResponse response = new HttpResponse();
        private string method = "get";
        private string url;
        private HttpContent content;

        public HttpRequest BasicAuthentication(string user, string pass)
        {
            var authToken = Encoding.ASCII.GetBytes($"{user}:{pass}");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            return this;
        }

        public HttpRequest TokenAuthentication(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return this;
        }

        public HttpRequest Get(string url)
        {
            this.url = url;

            return this;
        }
        public HttpRequest Post(string url)
        {
            this.method = "post";
            this.url = url;

            return this;
        }
        public HttpRequest Put(string url)
        {
            this.method = "put";
            this.url = url;

            return this;
        }

        public HttpResponse send()
        {
            this.Send().Wait();

            return response;
        }

        public HttpRequest BodyText( string text )
        {
            this.content = new StringContent(text);

            return this;
        }

        public async Task Send()
        {
            if(this.method == "get")
            {
                response.OriginalResponse = await httpClient.GetAsync($"{url}");
            }
            else if(this.method == "post")
            {
                response.OriginalResponse = await httpClient.PostAsync($"{url}", content);
            }
            else if(this.method == "put")
            {
                response.OriginalResponse = await httpClient.PutAsync($"{url}", content);
            }
            return;
        }

        public static void Main(string[] args)
        {
            HttpResponse response = new HttpRequest().Get("www.google.com").send();

            if(response.statusCode() == 200)
            {
                Console.Out.Write("ok");
            }
        }
    }
}
