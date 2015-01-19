using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Data
{
    public class UsersContext : DbContext, IUsersContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public IDbSet<UserProfile> UserProfiles { get; set; }
        public IDbSet<UserPoint> UserPoints { get; set; }
        public IDbSet<Tag> Tags { get; set; }
        public IDbSet<Ticket> Tickets { get; set; }
        public IDbSet<TicketUserRank> TicketUserRanks { get; set; }
        public IDbSet<TicketTag> TicketTags { get; set; }
        public IDbSet<UserTag> UserTags { get; set; }
        public IDbSet<Test> Tests { get; set; }
        public IDbSet<Question> Questions { get; set; }
        public IDbSet<Suggestion> Suggestions { get; set; }
        public IDbSet<TestMarking> TestMarkings { get; set; }

        /// <summary>
        /// UserProfiles with eagerly loaded information.
        /// </summary>
        public IQueryable<UserProfile> FullUserProfiles
        {
            get
            {
                // Eagerly load tag information associated with profiles
                return ((DbSet<UserProfile>)UserProfiles).Include(profile => profile.Tags.Select(tag => tag.Tag));
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
            .Map<Ticket>(t => t.Requires("TicketType").HasValue(-1))
            .Map<Question>(q => q.Requires("TicketType").HasValue((int)TicketType.Question))
            .Map<Suggestion>(s => s.Requires("TicketType").HasValue((int)TicketType.Suggestion))
            .Map<Test>(t => t.Requires("TicketType").HasValue((int)TicketType.Test))
            .ToTable("dbo.Tickets");

            modelBuilder.Entity<Tag>().
              HasMany(c => c.Parents).
              WithMany(p => p.Children).
              Map(
               m =>
               {
                   m.MapLeftKey("FkParentId");
                   m.MapRightKey("FkChildId");
                   m.ToTable("TagRelationships");
               });
        }
    }
}
