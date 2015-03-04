using System;
using System.Collections.Specialized;

namespace Blueprint.Aspnet.Host
{
    public interface IRequestWrapper
    {
        string HttpMethod { get; set; }

        NameValueCollection Headers { get; set; }
        string ContentType { get; set; }

        string Body { get; set; }
        Uri Url { get; set; }
    }

}
