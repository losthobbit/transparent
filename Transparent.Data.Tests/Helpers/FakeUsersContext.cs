using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Tests.Common;

namespace Transparent.Data.Tests.Helpers
{
    public class FakeUsersContext : IUsersContext
    {
        public event Action<FakeUsersContext> SavedChanges;

        public FakeUsersContext()
        {
            UserProfiles = new FakeDbSet<UserProfile>();
            UserPoints = new FakeDbSet<UserPoint>();
            Tags = new FakeDbSet<Tag>();
            Tickets = new FakeTicketDbSet();
            TicketUserRanks = new FakeDbSet<TicketUserRank>();
            TicketTags = new FakeDbSet<TicketTag>();
            UserTags = new FakeDbSet<UserTag>();
            Tests = new FakeDbSet<Test>();
            Questions = new FakeDbSet<Question>();
            Suggestions = new FakeDbSet<Suggestion>();
            TestMarkings = new FakeDbSet<TestMarking>();
            Subscriptions = new FakeDbSet<Subscription>();
            Arguments = new FakeDbSet<Argument>();
            TicketUserVotes = new FakeDbSet<TicketUserVote>();
            UsersInRoles = new FakeDbSet<UserInRole>();
            Roles = new FakeDbSet<Role>();
            TicketHistory = new FakeDbSet<TicketHistory>();
        }

        public IDbSet<UserProfile> UserProfiles { get; private set; }
        public IDbSet<UserPoint> UserPoints { get; private set; }
        public IDbSet<Tag> Tags { get; private set; }
        public IDbSet<Ticket> Tickets { get; private set; }
        public IDbSet<TicketUserRank> TicketUserRanks { get; private set; }
        public IDbSet<TicketTag> TicketTags { get; private set; }
        public IDbSet<UserTag> UserTags { get; private set; }
        public IDbSet<Test> Tests { get; set; }
        public IDbSet<Question> Questions { get; set; }
        public IDbSet<Suggestion> Suggestions { get; set; }
        public IDbSet<TestMarking> TestMarkings { get; set; }
        public IDbSet<Subscription> Subscriptions { get; set; }
        public IDbSet<Argument> Arguments { get; set; }
        public IDbSet<TicketUserVote> TicketUserVotes { get; private set; }
        public IDbSet<UserInRole> UsersInRoles { get; private set; }
        public IDbSet<Role> Roles { get; private set; }
        public IDbSet<TicketHistory> TicketHistory { get; private set; }

        public IQueryable<UserProfile> FullUserProfiles
        {
            get
            {
                return UserProfiles;
            }
        }

        public int SaveChanges()
        {
            if (SavedChanges != null)
                SavedChanges(this);
            return 0;
        }

        public DbEntityEntry Entry(object entity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
