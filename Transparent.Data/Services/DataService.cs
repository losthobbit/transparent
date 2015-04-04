﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Data.Services
{
    /// <summary>
    /// For high level data operations.
    /// </summary>
    /// <remarks>
    /// Designed to be used as a singleton.
    /// Methods do not call DbContext.SaveChanges.
    /// </remarks>
    public class DataService : IDataService
    {
        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        public void SetNextState(Ticket ticket, TicketState? specificState = null)
        {
            var state = specificState ?? ticket.NextState;
            if (state == null)
                throw new NotSupportedException("Ticket does not have a next state");
            ticket.TrySetState(state.Value);
            ticket.ModifiedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        public void AddPoints(UserPoint userPoint, UserTag userTag, int points)
        {
            userPoint.Quantity += points;
            userTag.TotalPoints += points;
        }

        /// <summary>
        /// Adds points to the UserPoints and UserTags.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        public void AddPoints(IUsersContext db, IEnumerable<int> userId, int tagId, int testId, int points, PointReason reason)
        {
            foreach (var user in userId)
            {
                AddPoints(db, user, tagId, testId, points, reason);
            }
        }

        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        private void AddPoints(IUsersContext db, int userId, int tagId, int testId, int points, PointReason reason)
        {
            var userPoint = db.UserPoints.SingleOrDefault(point => point.FkUserId == userId && point.FkTagId == tagId && point.FkTestId == testId);
            if (userPoint == null)
            {
                userPoint = new UserPoint { FkUserId = userId, FkTagId = tagId, FkTestId = testId, Reason = reason };
                db.UserPoints.Add(userPoint);
            }

            var userTag = db.UserTags.SingleOrDefault(tag => tag.FkUserId == userId && tag.FkTagId == tagId);
            if (userTag == null)
            {
                userTag = new UserTag { FkTagId = tagId, FkUserId = userId };
                db.UserTags.Add(userTag);
            }
            AddPoints(userPoint, userTag, points);
        }

        /// <summary>
        /// Adjusts the votes of the ticket based on the user's stance.
        /// </summary>
        /// <remarks>
        /// Ensures that ticket votes (for and against) and ticket user vote are adjusted together.
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        public void SetVote(Ticket ticket, Stance vote, int userId)
        {
            var voteRecord = ticket.UserVotes.SingleOrDefault(userVote => userVote.FkUserId == userId);
            if (voteRecord == null)
            {
                if (vote != Stance.Neutral)
                {
                    ticket.UserVotes.Add
                    (
                        new TicketUserVote
                        {
                            For = vote == Stance.For,
                            FkUserId = userId
                        }
                    );
                    if (vote == Stance.For)
                        ticket.VotesFor++;
                    else
                        ticket.VotesAgainst++;
                }
            }
            else
            {
                if (vote == Stance.Neutral)
                {
                    ticket.UserVotes.Remove(voteRecord);
                    if (voteRecord.For)
                        ticket.VotesFor--;
                    else
                        ticket.VotesAgainst--;
                }
                else
                {
                    if (voteRecord.For && vote == Stance.Against || !voteRecord.For && vote == Stance.For)
                    {
                        voteRecord.For = !voteRecord.For;
                        ticket.VotesFor += (int)vote;
                        ticket.VotesAgainst -= (int)vote;
                    }
                }
            }
        }

        /// <summary>
        /// Adjusts the rank of the ticket based on the user's stance.
        /// </summary>
        /// <remarks>
        /// Ensures ticket rank and ticket user rank are adjusted together.
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        public int SetRank(IUsersContext db, int ticketId, Stance ticketRank, int userId)
        {
            var ticket = db.Tickets.Single(t => t.Id == ticketId);
            var rankRecord = ticket.UserRanks.SingleOrDefault(rank => rank.FkUserId == userId);
            if (rankRecord == null)
            {
                if (ticketRank != Stance.Neutral)
                {
                    ticket.UserRanks.Add
                    (
                        new TicketUserRank
                        {
                            Up = ticketRank == Stance.For,
                            FkUserId = userId
                        }
                    );
                    ticket.Rank += (int)ticketRank;
                }
            }
            else
            {
                if (ticketRank == Stance.Neutral)
                {
                    ticket.UserRanks.Remove(rankRecord);
                    ticket.Rank += rankRecord.Up ? -1 : 1;
                }
                else
                {
                    if (rankRecord.Up && ticketRank == Stance.Against || !rankRecord.Up && ticketRank == Stance.For)
                    {
                        rankRecord.Up = !rankRecord.Up;
                        ticket.Rank += (int)ticketRank * 2;
                    }
                }
            }
            return ticket.Rank;
        }
    }
}