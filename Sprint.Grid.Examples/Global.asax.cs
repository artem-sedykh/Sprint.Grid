using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;

using Sprint.Grid.Examples.Controllers;

namespace Sprint.Grid.Examples
{
    public class MvcApplication : HttpApplication
    {
        public ContainerBuilder ContainerBuilder { get; set; }
        public IContainer Container { get; set; }      

        protected void Application_Start()
        {
            ContainerBuilder = new ContainerBuilder();
            ContainerBuilder.RegisterControllers(typeof(HomeController).Assembly);
            ContainerBuilder.RegisterModule(new AutofacConfig());

            Container = ContainerBuilder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));            

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            BundleTable.EnableOptimizations = true;

#if DEBUG
            BundleTable.EnableOptimizations = false;
#endif
        }
    }
}
