using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Caches;
using Transparent.Data.Interfaces;
using Transparent.Data.Services;
using Common.Windsor;
using Castle.Facilities.TypedFactory;

namespace Transparent.Data.Windsor
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IUsersContext>().ImplementedBy<UsersContext>().LifeStyle.Transient,
                Component.For<ITags>().ImplementedBy<Tags>().LifeStyle.Singleton,
                Component.For<IConfiguration>().ImplementedBy<Configuration>().LifeStyle.Singleton,
                Component.For<IDataService>().ImplementedBy<DataService>().LifeStyle.Singleton,
                Component.For<IUsersContextFactory>().AsFactory()
            );
        }
    }
}
