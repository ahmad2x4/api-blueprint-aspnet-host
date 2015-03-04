using System.Web;

namespace Blueprint.Aspnet.Host
{
    class ResponseWrapper : IResponseWrapper
    {
        private readonly HttpResponse _response;

        public ResponseWrapper(HttpResponse response)
        {
            _response = response;
        }

        public void End()
        {
            _response.End();
        }


        public void Clear()
        {
            _response.Clear();
        }

        public int StatusCode
        {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }

        public void AppendHeader(string name, string value)
        {
            _response.AppendHeader(name, value);
        }

        public void Write(string p)
        {
            _response.Write(p);
        }
    }
}