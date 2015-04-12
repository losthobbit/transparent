using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Interfaces;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Business.Services
{
    /// <summary>
    /// Contains ticket related events / scheduled tasks.
    /// </summary>
    /// <remarks>
    /// This is designed to be used as a singleton.
    /// </remarks>
    public class ProgressTickets : IProgressTickets
    {
        private readonly Func<IUsersContext> getUsersContext;
        private readonly IConfiguration configuration;
        private readonly IDataService dataService;
        private readonly ITags tags;

        public ProgressTickets(Func<IUsersContext> getUsersContext, IDataService dataService, IConfiguration configuration,
            ITags tags)
        {
            this.getUsersContext = getUsersContext;
            this.dataService = dataService;
            this.configuration = configuration;
            this.tags = tags;
        }

        /// <summary>
        /// Progresses tickets which are in the Verification state, and were last modified
        /// the specified amount of time ago.
        /// </summary>
        public void ProgressTicketsWithVerifiedTags()
        {
            using (var db = getUsersContext())
            {
                var lastModified = DateTime.UtcNow - configuration.DelayAfterValidatingTags;

                var highestRankedVerificationTickets = (from ticket in db.Tickets
                                                        where ticket.State == TicketState.Verification
                                                        orderby ticket.Rank descending
                                                        select ticket).Take(configuration.MaxPositionToAdvanceState);

                var verifiedTickets = from ticket in highestRankedVerificationTickets
                                      where ticket.ModifiedDate <= lastModified &&
                                      ticket.TicketTags.Any() &&
                                      ticket.TicketTags.All(tag => tag.Verified)
                                      select ticket;

                foreach (var ticket in verifiedTickets)
                {
                    dataService.SetNextState(ticket);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Progresses tickets which are in the Discussion state, and were last modified
        /// the specified amount of time ago.
        /// </summary>
        public void ProgressTicketsWithArguments()
        {
            using (var db = getUsersContext())
            {
                var lastModified = DateTime.UtcNow - configuration.DelayAfterDiscussion;
                var minNumberOfArguments = configuration.MinimumNumberOfArgumentsToAdvanceState;

                var highestRankedDiscussionTickets = (from ticket in db.Tickets
                                                      where ticket.State == TicketState.Discussion
                                                      orderby ticket.Rank descending
                                                      select ticket).Take(configuration.MaxPositionToAdvanceState);

                var discussedTickets = from ticket in highestRankedDiscussionTickets
                                       where ticket.ModifiedDate <= lastModified &&
                                       ticket.Arguments.Count >= minNumberOfArguments
                                       select ticket;

                foreach (var ticket in discussedTickets)
                {
                    dataService.SetNextState(ticket);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Progresses tickets which are in the Voting state, and (were last modified
        /// the specified amount of time ago or have a large enough gap between for and against votes).
        /// </summary>
        public void ProgressTicketsWithVotes()
        {
            using (var db = getUsersContext())
            {
                var lastModified = DateTime.UtcNow - configuration.DelayForVoting;

                var highestRankedVotingTickets = (from ticket in db.Tickets
                                                      where ticket.State == TicketState.Voting
                                                      orderby ticket.Rank descending
                                                      select ticket).Take(configuration.MaxPositionToAdvanceState);

                var votingTickets = (from ticket in highestRankedVotingTickets
                                    where ticket.ModifiedDate <= lastModified
                                    select ticket).ToList();

                foreach (var ticket in votingTickets)
                {
                    var totalVotes = ticket.VotesFor + ticket.VotesAgainst;
                    var accepted = totalVotes > 0 && ((double)ticket.VotesFor / (double)totalVotes >= (double)configuration.PercentOfVotesRequiredToAccept / 100d);
                    dataService.SetNextState(ticket, accepted ? TicketState.Accepted : TicketState.Rejected);
                    if (accepted)
                        dataService.AddPoints(db, ticket.FkUserId, tags.ApplicationTag.Id, configuration.DiPointsForAcceptedTicket,
                            PointReason.TicketAccepted, ticketId:ticket.Id);
                }
                db.SaveChanges();
            }
        }
    }
}
