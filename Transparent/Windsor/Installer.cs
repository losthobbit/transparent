using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Common.Interfaces;
using Transparent.Services;

namespace Transparent.Windsor
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient(),
                Component.For<IConfiguration>().ImplementedBy<Configuration>().LifeStyle.Singleton
            );
        }
    }
}