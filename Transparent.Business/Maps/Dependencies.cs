using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Interfaces;
using Transparent.Data.Interfaces;

namespace Transparent.Business.Maps
{
    /// <summary>
    /// Dependencies for static classes.
    /// </summary>
    /// <remarks>
    /// All of these dependencies will be singletons.
    /// </remarks>
    public class Dependencies
    {
        public Dependencies(IConfiguration configuration, ITicketsFactory ticketsFactory, IUserFactory userFactory, ITags tags)
        {
            Configuration = configuration;
            TicketsFactory = ticketsFactory;
            UserFactory = userFactory;
            Tags = tags;
        }

        public IConfiguration Configuration { get; set; }
        public ITicketsFactory TicketsFactory { get; set; }
        public IUserFactory UserFactory { get; set; }
        public ITags Tags { get; set; }
    }
}
