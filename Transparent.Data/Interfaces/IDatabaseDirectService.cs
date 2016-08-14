using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Interfaces
{
    /// <summary>
    /// For administering the database I need a way to 
    /// communicate directly with the database.
    /// </summary>
    public interface IDatabaseDirectService
    {
        void InsertData(IUsersContext db, string tableName, string csvData);
    }
}
