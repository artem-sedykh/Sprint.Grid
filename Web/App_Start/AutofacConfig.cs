using Autofac;
using Autofac.Integration.Mvc;
using Domain.Model;

namespace Web.App_Start
{
    public class AutofacConfig : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //var type = typeof(AccountService);
            //builder.RegisterAssemblyTypes(type.Assembly).AsImplementedInterfaces();
            builder.Register(c => new NorthWindDataContext()).As<NorthWindDataContext>().InstancePerHttpRequest();
        }
    }
}