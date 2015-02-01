using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatrix.WebData;

namespace Transparent.Data
{
    public static class Security
    {
        public static void InitializeDatabase()
        {
            if(!WebSecurity.Initialized)
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
        }
    }
}
