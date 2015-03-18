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
    public interface IUsersContext: IDisposable
    {
        IDbSet<UserProfile> UserProfiles { get; }
        IDbSet<UserPoint> UserPoints { get; }
        IDbSet<Tag> Tags { get; }
        IDbSet<Ticket> Tickets { get; }
        IDbSet<TicketUserRank> TicketUserRanks { get; }
        IDbSet<TicketTag> TicketTags { get; }
        IDbSet<UserTag> UserTags { get; }
        IDbSet<Test> Tests { get; }
        IDbSet<Question> Questions { get; }
        IDbSet<Suggestion> Suggestions { get; }
        IDbSet<TestMarking> TestMarkings { get; }
        IDbSet<Subscription> Subscriptions { get; }
        IDbSet<Argument> Arguments { get; set; }

        int SaveChanges();
        DbEntityEntry Entry(object entity);

        IQueryable<UserProfile> FullUserProfiles { get; }
    }
}
