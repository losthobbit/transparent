using Common.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Interfaces;
using Transparent.Data.Interfaces;

namespace Transparent.Business.Events
{
    /// <summary>
    /// Used to update the competency levels in each tag and refresh the tag cache.
    /// </summary>
    /// <remarks>
    /// Can be a singleton
    /// </remarks>
    public class UpdateTagsEvent: TimedEvent
    {
        private readonly ITags tags;
        private readonly IUsersContextFactory usersContextFactory;
        private readonly IConfiguration configuration;

        public UpdateTagsEvent(Common.Interfaces.IConfiguration commonConfiguration, ITags tags, IUsersContextFactory usersContextFactory,
            IConfiguration configuration)
            : base(TimeSpan.Parse(commonConfiguration.GetValue("UpdateTagsEventInterval")))
        {
            this.tags = tags;
            this.usersContextFactory = usersContextFactory;
            this.configuration = configuration;
        }

        public override void Action()
        {
            UpdateCompetencyLevels();
            tags.Refresh();
        }

        /// <summary>
        /// Calculates the points required to be competent and an expert in a tag, and stores them in the tag.
        /// </summary>
        /// <remarks>
        /// Calculated as follows: a user is competent in a tag if any of the following are true:
        /// 1. Their score is in the top MinPercentCompetents % of all users.
        /// 2. Their score is in the top MinCompetents of all users.
        /// 3. Their score is in the top CompetentPercentOfHighestScore % of the highest score for that tag.
        /// 
        /// Expert level is calculated in a similar way.
        /// </remarks>
        public void UpdateCompetencyLevels()
        {
            using (var db = usersContextFactory.Create())
            {
                var userCount = db.UserProfiles.Count();
                var competentPosition = Math.Max(
                    configuration.MinCompetents,
                    (int)Math.Ceiling(((float)configuration.MinPercentCompetents / 100f) * userCount)
                );

                var expertPosition = Math.Max(
                    configuration.MinExperts,
                    (int)Math.Ceiling(((float)configuration.MinPercentExperts / 100f) * userCount)
                );

                var tags = db.Tags.ToList();

                foreach (var tag in tags)
                {
                    var orderedUserTags = db.UserTags
                        .Where(ut => ut.FkTagId == tag.Id)
                        .OrderByDescending(ut => ut.TotalPoints);

                    var competentUserTag = orderedUserTags.Skip(competentPosition - 1).FirstOrDefault();
                    var expertUserTag = orderedUserTags.Skip(expertPosition - 1).FirstOrDefault();
                    var highestUserTag = orderedUserTags.FirstOrDefault();

                    var competentPassMark = highestUserTag == null
                        ? 0
                        : (int)Math.Ceiling(((float)configuration.CompetentPercentOfHighestScore / 100f) * highestUserTag.TotalPoints);

                    var expertPassMark = highestUserTag == null
                        ? 0
                        : (int)Math.Ceiling(((float)configuration.ExpertPercentOfHighestScore / 100f) * highestUserTag.TotalPoints);

                    tag.CompetentPoints = Math.Min(competentUserTag == null ? 0 : competentUserTag.TotalPoints, competentPassMark);
                    tag.ExpertPoints = Math.Min(expertUserTag == null ? 0 : expertUserTag.TotalPoints, expertPassMark);
                }
                db.SaveChanges();
            }
        }
    }
}
