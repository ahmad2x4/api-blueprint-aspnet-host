using System.Collections.Generic;
using System.Linq;
using Blueprint.Aspnet.Host.Modules;

namespace Blueprint.Aspnet.Host.Extensions
{
    public static class RouteExtensions
    {
        public static IRouterConfiguration WithRoute(this IRouterConfiguration config, IRoute route, IRequestHandler handler)
        {
            config.Routes.Add(new KeyValuePair<IRoute, IRequestHandler>(route, handler));
            return config;
        }

        public static IRouterConfiguration WithRoutes(this IRouterConfiguration config, IDictionary<IRoute, IRequestHandler> handlerPairs)
        {
            handlerPairs.ForEach(p => config.Routes.Add(new KeyValuePair<IRoute, IRequestHandler>(p.Key, p.Value)));
            return config;
        }
    }
}