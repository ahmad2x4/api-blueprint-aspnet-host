using System;
using Blueprint.Aspnet.Host.Modules;

namespace Blueprint.Aspnet.Host.RequestHandlers
{
    public class DiscoveryHandler : IRequestHandler
    {

        public IRoute Route { get; private set; }

        public void Handle(IRequestWrapper request, IResponseWrapper response)
        {
            throw new NotImplementedException();
        }
    }
}