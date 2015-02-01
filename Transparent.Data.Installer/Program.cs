using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using WebMatrix.WebData;

namespace Transparent.Data.Installer
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeDatabase();
        }

        private static void InitializeDatabase()
        {
            Database.SetInitializer<UsersContext>(new InitDatabase<UsersContext>());
            UsersContext context = new UsersContext();
            context.Database.Initialize(false);
        }
    }
}
