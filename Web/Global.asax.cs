using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using AutofacContrib.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using Web.App_Start;
using Web.Controllers;

namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
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

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(Container));

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
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