﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Interfaces;
using Transparent.Business.ViewModels;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Business.Services
{
    /// <summary>
    /// Contains methods for getting or setting general information.
    /// </summary>
    /// <remarks>
    /// This is designed to be used as a singleton.
    /// </remarks>
    public class General: IGeneral
    {
        private readonly IUsersContextFactory usersContextFactory;

        public General(IUsersContextFactory usersContextFactory)
        {
            this.usersContextFactory = usersContextFactory;
        }

        public StatsViewModel GetStats()
        {
            using (var dbContext = usersContextFactory.Create())
            {
                return new StatsViewModel
                {
                    RegisteredUsers = dbContext.UserProfiles.Count(),
                    SuggestionsCreated = dbContext.Suggestions.Count(),
                    SuggestionsImplemented = dbContext.Suggestions.Count(suggestion => suggestion.State == TicketState.Completed)
                };
            }
        }
    }
}
