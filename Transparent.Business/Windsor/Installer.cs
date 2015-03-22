using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Common.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Events;
using Transparent.Business.Interfaces;
using Transparent.Business.Services;

namespace Transparent.Business.Windsor
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IEvent>().ImplementedBy<CompleteTagValidationEvent>().LifeStyle.Singleton,
                Component.For<ITickets>().ImplementedBy<Tickets>().LifeStyle.Transient,
                Component.For<IUser>().ImplementedBy<User>().LifeStyle.Transient,
                Component.For<IGeneral>().ImplementedBy<General>().LifeStyle.Singleton
            );

            Func<ITickets> ticketsFactory = () => container.Resolve<ITickets>();
            container.Register(Component.For<Func<ITickets>>().Instance(ticketsFactory));
        }
    }
}
