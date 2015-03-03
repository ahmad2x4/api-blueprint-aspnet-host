using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace Blueprint.Aspnet.Host
{
    public class RequestWrapper : IRequestWrapper
    {
        private string _body;

        public RequestWrapper(HttpRequest request)
        {
            this.HttpMethod = request.HttpMethod;
            this.ContentType = request.ContentType;
            this._body = ReadBodyToEnd(request);
            //headers
        }

        private string ReadBodyToEnd(HttpRequest request)
        {
            var result = string.Empty;

            if (request == null)
                return result;

            using (var memStream = new MemoryStream())
            {
                if (request.InputStream.CanSeek)
                {
                    request.InputStream.Seek(0, SeekOrigin.Begin);
                }
                request.InputStream.CopyTo(memStream);

                memStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(memStream))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;            
        }

        public string  ContentType { get; set; }

        public NameValueCollection Headers { get; set; }

        public string HttpMethod { get; set; }

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }
    }
}