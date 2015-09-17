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
using Common.Windsor;
using Transparent.Business.Maps;
using Castle.Facilities.TypedFactory;

namespace Transparent.Business.Windsor
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ITimedEvent>().ImplementedBy<CompleteTagValidationEvent>().LifeStyle.Singleton,
                Component.For<ITimedEvent>().ImplementedBy<CompleteDiscussionEvent>().LifeStyle.Singleton,
                Component.For<ITimedEvent>().ImplementedBy<CompleteVotingEvent>().LifeStyle.Singleton,
                Component.For<ITimedEvent>().ImplementedBy<UpdateTagsEvent>().LifeStyle.Singleton,
                Component.For<IUserActionEvent>().ImplementedBy<SetLastActionDateEvent>().LifeStyle.Singleton,
                Component.For<ITickets>().ImplementedBy<Tickets>().LifeStyle.Transient,
                Component.For<IUser>().ImplementedBy<User>().LifeStyle.Transient,
                Component.For<IVolunteers>().ImplementedBy<Volunteers>().LifeStyle.Transient,
                Component.For<IGeneral>().ImplementedBy<General>().LifeStyle.Singleton,
                Component.For<IProgressTickets>().ImplementedBy<ProgressTickets>().LifeStyle.Singleton,
                Component.For<Dependencies>().LifeStyle.Singleton,
                Component.For<ITicketsFactory>().AsFactory(),
                Component.For<IUserFactory>().AsFactory()
            );
            DataToViewMappingExtensions.Dependencies = container.Resolve<Dependencies>();
        }
    }
}
