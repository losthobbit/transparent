using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;

namespace Transparent.Data.Services
{
    /// <summary>
    /// For administering the database I need a way to 
    /// communicate directly with the database.
    /// </summary>
    /// <remarks>
    /// Can be a singleton.
    /// </remarks>
    public class DatabaseDirectService: IDatabaseDirectService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <param name="csvData"></param>
        /// <remarks>
        /// This method is UNSAFE!  Do not set the tableName from user data!
        /// </remarks>
        public void InsertData(IUsersContext db, string tableName, string csvData)
        {
            var columnQuery = "SELECT COLUMN_NAME, DATA_TYPE " +
            "FROM INFORMATION_SCHEMA.COLUMNS " +
            "WHERE TABLE_NAME = N'" + tableName + "'";
            var columns = db.Database.SqlQuery<Column>(columnQuery).ToList();
            var dataRows = csvData.Split('\n', '\r')
                .Where(row => !String.IsNullOrEmpty(row))
                .Select(row => row.Split('|'))
                .ToList();

            var commandSql = new StringBuilder();
            commandSql.AppendLine("INSERT INTO [" + tableName + "]");
            commandSql.AppendLine("(" + String.Join(",", columns.Select(column => "[" + column.COLUMN_NAME + "]")) + ")");
            commandSql.AppendLine("VALUES");
            for (var i = 0; i < dataRows.Count; i++)
            {
                var row = dataRows[i];
                commandSql.Append("(" + RowToSql(row, columns.Select(col => col.DATA_TYPE).ToArray()) + ")");
                commandSql.AppendLine(i < dataRows.Count - 1 ? "," : "");
            }
            var enableIdentityInsertQuery = "set identity_insert [" + tableName + "] ON";
            var hasIdentityColumn = true;
            try
            {
                db.Database.ExecuteSqlCommand(enableIdentityInsertQuery);
            }
            catch (SqlException)
            {
                // If there isn't an identity column, it doesn't matter
                hasIdentityColumn = false;
            }
            try
            {
                if(hasIdentityColumn)
                {
                    // No idea why I have to do this again as part of the query
                    // but if I don't then it doesn't always work.
                    commandSql.Insert(0, enableIdentityInsertQuery + Environment.NewLine);
                }
                db.Database.ExecuteSqlCommand(commandSql.ToString());
            }
            finally
            {
                if(hasIdentityColumn)
                    db.Database.ExecuteSqlCommand("set identity_insert [" + tableName + "] OFF");
            }
        }

        private string RowToSql(string[] row, string[] columnTypes)
        {
            var sql = new StringBuilder();
            for (var i = 0; i < columnTypes.Length; i++)
            {
                var type = columnTypes[i];
                var value = i < row.Length ? row[i] : GetDefaultValue(type);
                var isNull = value == "NULL";
                var isDate = type.Contains("date");
                var isText = type.Contains("char");
                var isBit = type == "bit";
                var requiresQuotes = 
                    (
                        isText || 
                        isDate || 
                        type.Contains("time") ||
                        isBit
                    )
                    && !isNull;

                if (!isNull)
                {
                    if (isDate)
                        value = SqlDate(value);

                    if (isText)
                        value = SqlText(value);
                    
                }

                if (requiresQuotes)
                    sql.Append("'");
                sql.Append(value);
                if (requiresQuotes)
                    sql.Append("'");
                sql.Append(i < columnTypes.Length - 1 ? ", " : "");
            }
            return sql.ToString();
        }

        private string GetDefaultValue(string type)
        {
            var isDate = type.Contains("date");
            var isText = type.Contains("char");
            var isBit = type == "bit";

            if (isText) return "NULL";
            if (isDate) throw new NotImplementedException("No default value defined for date");
            if (isBit) return "FALSE";
            return "0";
        }

        private string SqlText(string text)
        {
            var sqlText = new StringBuilder();
            var wholeTextInDoubleQuotes = false;
            var inDoubleQuotes = false;
            for(var i = 0; i < text.Length; i++)
            {
                var character = text[i];
                if (wholeTextInDoubleQuotes)
                {
                    if (character == '\"')
                    {
                        if (i == text.Length - 1)
                        {
                            // ignore the last double quote
                            continue;
                        }
                        if (text[i + 1] == '\"')
                        {
                            if (inDoubleQuotes)
                            {
                                inDoubleQuotes = false;
                                i++;
                            }
                            else // !inDoubleQuotes
                            {
                                inDoubleQuotes = true;
                                i++;
                            }
                        }
                    }
                    if (character == '\'')
                    {
                        if (inDoubleQuotes && text[i + 1] == '\'')
                        {
                            inDoubleQuotes = false;
                            sqlText.Append("\"");
                            i++;
                            continue;
                        }
                        sqlText.Append("\'");
                    }
                }
                else
                {
                    // not isWholeTextInDoubleQuotes
                    if (character == '\"')
                    {
                        if (i == 0)
                        {
                            wholeTextInDoubleQuotes = true;
                            // ignore the first double quote
                            continue;
                        }
                    }
                    if (character == '\'')
                    {
                        sqlText.Append("\'");
                    }
                }
                sqlText.Append(character);
            }

            return sqlText.ToString()
                .Replace("\r", "' + CHAR(13) + '")
                .Replace("\n", "' + CHAR(10) + '");
        }

        private string SqlDate(string date)
        {
            var day = date.Substring(0, 2);
            var month = date.Substring(3, 2);
            var year = "20" + date.Substring(6, 2);
            var time = date.Substring(9, 5);
            return $"{year}/{month}/{day} {time}";
        }

        public class Column
        {
            public string COLUMN_NAME { get; set; }
            public string DATA_TYPE { get; set; }
        }
    }
}
