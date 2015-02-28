using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.ViewModels;

namespace Transparent.Data.Queries
{
    /// <summary>
    /// Contains methods for getting or setting general information.
    /// </summary>
    /// <remarks>
    /// There's no need to split this into a business and data service, unless a different data source is required.
    /// This is designed to be used as a singleton.
    /// </remarks>
    public class General: IGeneral
    {
        private readonly Func<IUsersContext> getUsersContext;

        public General(Func<IUsersContext> getUsersContext)
        {
            this.getUsersContext = getUsersContext;
        }

        public StatsViewModel GetStats()
        {
            using (var dbContext = getUsersContext())
            {
                return new StatsViewModel
                {
                    RegisteredUsers = dbContext.UserProfiles.Count(),
                    SuggestionsCreated = dbContext.Suggestions.Count(),
                    SuggestionsImplemented = dbContext.Suggestions.Count(suggestion => suggestion.State == Models.TicketState.Completed)
                };
            }
        }
    }
}
