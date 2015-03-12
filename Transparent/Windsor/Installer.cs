using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Common.Interfaces;
using Transparent.Services;
using System.Web.Http.Controllers;
using Transparent.Interfaces;

namespace Transparent.Windsor
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient(),
                Classes.FromThisAssembly().BasedOn<IHttpController>().LifestyleTransient(),
                Component.For<IWindsorContainer>().Instance(container),
                Component.For<IActionInvoker>().ImplementedBy<WindsorActionInvoker>().LifeStyle.Singleton,
                Component.For<IConfiguration>().ImplementedBy<Configuration>().LifeStyle.Singleton,
                Component.For<ITechnicalConfiguration>().ImplementedBy<TechnicalConfiguration>().LifeStyle.Singleton
            );
        }
    }
}