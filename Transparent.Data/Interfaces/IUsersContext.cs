using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Data.Interfaces
{
    public interface IUsersContext
    {
        IDbSet<UserProfile> UserProfiles { get; }
        IDbSet<UserPoint> UserPoints { get; }
        IDbSet<Tag> Tags { get; }
        IDbSet<Ticket> Tickets { get; }
        IDbSet<TicketUserRank> TicketUserRanks { get; }
        IDbSet<TicketTag> TicketTags { get; }
        int SaveChanges();
        DbEntityEntry Entry(object entity);
    }
}
