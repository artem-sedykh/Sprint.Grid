using Autofac;
using Sprint.Grid.Examples.Services;
using Sprint.Grid.Examples.Services.Impl;

namespace Sprint.Grid.Examples
{
    public class AutofacConfig : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //var type = typeof(CustomerService);
            //builder.RegisterAssemblyTypes(type.Assembly).AsImplementedInterfaces();

            builder.RegisterType<CustomerService>().As<ICustomerService>();
        }
    }
}