using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using WebMatrix.WebData;
using System.Data.Entity;

namespace Transparent.Data
{
    using Models;

    //public class InitDatabase : DropCreateDatabaseIfModelChanges<UsersContext>
    //{
    //    protected override void Seed(UsersContext context)
    //    {
    //        WebSecurity.InitializeDatabaseConnection("DefaultConnection",
    //           "UserProfile", "UserId", "UserName", autoCreateTables: true);
    //    }
    //}

    public class InitDatabase<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        public void InitializeDatabase(TContext context)
        {
            WebSecurity.InitializeDatabaseConnection("DefaultConnection",
               "UserProfile", "UserId", "UserName", autoCreateTables: true);
        }
    }
}
