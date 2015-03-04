using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blueprint.Aspnet.Host.RequestHandlers
{
    public class DiscoveryHandler : IRequestHandler
    {
        private readonly IEnumerable<snowcrashCLR.Blueprint> _bluprints;

        public DiscoveryHandler(IEnumerable<snowcrashCLR.Blueprint> bluprints, IRoute route)
        {
            _bluprints = bluprints;
            Route = route;
        }

        public IRoute Route { get; private set; }

        public void Handle(IRequestWrapper request, IResponseWrapper response)
        {
            response.Write(JsonConvert.SerializeObject(_bluprints));
        }
    }
}