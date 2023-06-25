using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HTTP
{
    public class HttpResponse
    {
        private HttpResponseMessage originalResponse = null;

        public HttpResponse()
        {
        }
        
        public HttpResponseMessage OriginalResponse
        {
            get { return this.originalResponse; }
            set { this.originalResponse = value; }
        }

        public int statusCode()
        {
            return (int)this.originalResponse.StatusCode;
        }

        public HttpStatusCode StatusCode()
        {
            return this.originalResponse.StatusCode;
        }
    }
}
