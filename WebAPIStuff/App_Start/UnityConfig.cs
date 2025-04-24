using WebAPIStuff.Services;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace WebAPIStuff
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<IPdfToTextServices, PdfToTextService>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}