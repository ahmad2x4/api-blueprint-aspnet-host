using Blueprint.Aspnet.Host.Modules;

namespace Blueprint.Aspnet.Host
{
    public interface IRequestHandler
    {
        IRoute Route { get; }

        void Handle(IRequestWrapper request, IResponseWrapper response); 
    }
}
