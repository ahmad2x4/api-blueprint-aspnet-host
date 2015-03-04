using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Blueprint.Aspnet.Host.RequestHandlers;
using snowcrashCLR;
using Blueprint.Aspnet.Host.Extensions;

namespace Blueprint.Aspnet.Host.Modules
{
    public class BlueprintModule: IHttpModule 
    {
        IEnumerable<snowcrashCLR.Blueprint> _blueprints;
        IEnumerable<IRequestHandler> _reqestHandlers;
        private IRequestRouter _requestRouter = new RequestRouter();


        public String ModuleName
        {
            get { return "BlueprintModule"; }
        }

        public void Init(HttpApplication application)
        {
            LoadBlueprints();
            ConfigureRoutes();
            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
        }

        private void ConfigureRoutes()
        {
            var result = (from b in _blueprints
                from g in b.GetResourceGroupsCs()
                from r in g.GetResourcesCs()
                select r).ToDictionary(resource => (IRoute) new Route(resource.uriTemplate),
                    resource => (IRequestHandler) new MockHandler(resource, new Route(resource.uriTemplate)), new Route.UrlTemplateEqualityComparer());

            _requestRouter
                .Configure()
                .WithRoutes(result)
                .WithRoute(new Route("/"), new DiscoveryHandler(_blueprints, new Route("/")));

        }

        private void LoadBlueprints()
        {
            var blueprintPath = ConfigurationManager.AppSettings["BlueprintPath"];
            if (_blueprints == null)
            {
                _blueprints = GetBlueprints(blueprintPath).ToList();
            }
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            // Create HttpApplication and HttpContext objects to access
            // request and response properties.
            var application = (HttpApplication)source;
            var context = application.Context;
            ProcessRequest(context);
        }

        private IEnumerable<snowcrashCLR.Blueprint> GetBlueprints(string path)
        {
            if (!Directory.Exists(path))
                yield break;

            var files = Directory.EnumerateFiles(path, "*.md", SearchOption.AllDirectories).ToList();

            foreach (var file in files)
            {
                using (var reader = new StreamReader(file))
                {
                    var contents = reader.ReadToEnd();
                    snowcrashCLR.Blueprint blueprint;
                    Result result;
                    SnowCrashCLR.parse(contents, out blueprint, out result);
                    if (blueprint != null)
                    {
                        yield return blueprint;
                    }
                }
            }
        }

        private void Application_EndRequest(Object source, EventArgs e)
        {
            var application = (HttpApplication)source;
            var context = application.Context;
            context.Request.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public void Dispose() { }

        private void ProcessRequest(HttpContext context)
        {
            LoadBlueprints();
            var request = new RequestWrapper(context.Request);
            var response = new ResponseWrapper(context.Response);

            _requestRouter.Route(request)
                .Handle(request,response);
        }
    }

    public interface IRouterConfiguration
    {
        IDictionary<IRoute, IRequestHandler> Routes { get; }
    }

    class RouterConfiguration : IRouterConfiguration
    {
        public RouterConfiguration()
        {
            Routes = new Dictionary<IRoute, IRequestHandler>();
        }

        public IDictionary<IRoute, IRequestHandler> Routes { get; private set; }
    }

    public interface IRequestRouter
    {
        IRouterConfiguration Routes { get; }

        IRouterConfiguration Configure();

        IRequestHandler Route(IRequestWrapper request);

    }


    class RequestRouter : IRequestRouter
    {
        private IRouterConfiguration _routerConfiguration;
        public IRouterConfiguration Routes { get; private set; }

        public RequestRouter()
        {
            _routerConfiguration = new RouterConfiguration();
        }

        public IRouterConfiguration Configure()
        {
            return _routerConfiguration;
        }
        public IRequestHandler Route(IRequestWrapper request)
        {
            return _routerConfiguration.Routes[new Route(request.Url.PathAndQuery)];
        }
    }
}