using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Common.Interfaces;
using Common.Interfaces.Events;
using Common.Services;
using Common.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Windsor
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ITimedEventRunner>().ImplementedBy<TimedEventRunner>().LifeStyle.Singleton,
                Component.For<IUserActionEventRunner>().ImplementedBy<UserActionEventRunner>().LifeStyle.Singleton,
                Component.For<ISecurity>().ImplementedBy<Security>().LifeStyle.Singleton
            );
        }
    }
}
