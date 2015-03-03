using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using snowcrashCLR;

namespace Blueprint.Aspnet.Host.Modules
{
    public class BlueprintModule: IHttpModule 
    {
        IEnumerable<snowcrashCLR.Blueprint> _blueprints;
        IEnumerable<IRequestHandler> _reqestHandlers;

        public String ModuleName
        {
            get { return "BlueprintModule"; }
        }

        public void Init(HttpApplication application)
        {
            LoadBlueprints();
            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
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
                    snowcrashCLR.Result result;
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
            var request = context.Request;
            var response = context.Response;

            var matchingResources = from b in _blueprints
                from g in b.GetResourceGroupsCs()
                from r in g.GetResourcesCs()
                where r.uriTemplate == request.Path
                select r;

            // we will just take the first resource that matches the path
            var resource = matchingResources.FirstOrDefault();

            if (resource != null)
            {
                //HandleRequest(request, response, resource);
            }
        }
    }

    /*
     * rh.configure.with(reuqest)
     * rh.configure.with(reuqest[]).unhandled
     * */

    public interface IRoute
    {
        string UrlTemplate { get; }
    }

    public interface IRouterConfiguration
    {
        IDictionary<IRoute, IRequestHandler> Routes { get; }
    }

    public static class RouteExtensions
    {
        public static IRouterConfiguration WithRoute(this IRouterConfiguration config, IRoute route, IRequestHandler handler)
        {
            return config;
        }

        public static IRouterConfiguration WithRoutes(this IRouterConfiguration config, IDictionary<IRoute, IRequestHandler> handlerPairs)
        {
            return config;
        }
}

    public interface IRequestRouter
    {
        IRouterConfiguration Routes { get; }

        IRouterConfiguration Configure();
        
    }
}