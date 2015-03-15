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
using Transparent.Data.Queries;
using Transparent.Data.Services;

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
                Component.For<ITickets>().ImplementedBy<Tickets>().LifeStyle.Transient,
                Component.For<IUser>().ImplementedBy<User>().LifeStyle.Transient,
                Component.For<IGeneral>().ImplementedBy<General>().LifeStyle.Singleton
            );

            Func<IUsersContext> usersContextFactory = () => container.Resolve<IUsersContext>();
            container.Register(Component.For<Func<IUsersContext>>().Instance(usersContextFactory));

            Func<ITickets> ticketsFactory = () => container.Resolve<ITickets>();
            container.Register(Component.For<Func<ITickets>>().Instance(ticketsFactory));
        }
    }
}
