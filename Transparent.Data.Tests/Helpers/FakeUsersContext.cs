using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Transparent.Data.Tests.Generic;

namespace Transparent.Data.Tests.Helpers
{
    public class FakeUsersContext : IUsersContext
    {
        public FakeUsersContext()
        {
            UserProfiles = new FakeDbSet<UserProfile>();
            UserPoints = new FakeDbSet<UserPoint>();
            Tags = new FakeDbSet<Tag>();
            Tickets = new FakeDbSet<Ticket>();
            TicketUserRanks = new FakeDbSet<TicketUserRank>();
            TicketTags = new FakeDbSet<TicketTag>();
            UserTags = new FakeDbSet<UserTag>();
        }

        public IDbSet<UserProfile> UserProfiles { get; private set; }
        public IDbSet<UserPoint> UserPoints { get; private set; }
        public IDbSet<Tag> Tags { get; private set; }
        public IDbSet<Ticket> Tickets { get; private set; }
        public IDbSet<TicketUserRank> TicketUserRanks { get; private set; }
        public IDbSet<TicketTag> TicketTags { get; private set; }
        public IDbSet<UserTag> UserTags { get; private set; }

        public IQueryable<UserProfile> FullUserProfiles
        {
            get
            {
                return UserProfiles;
            }
        }

        public int SaveChanges()
        {
            return 0;
        }

        public DbEntityEntry Entry(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
