using System;
using System.Collections.Generic;
using System.Text;
using static SQLiteAdapter.SQLiteCacheSettings;

namespace SQLiteAdapter
{
    static internal class DefaultQueries
    {
        public static Dictionary<CommandType, string>  GetDefaultQueries(string tableName)
        {
            Dictionary<CommandType, string> result = new Dictionary<CommandType, string>();

            result[CommandType.CreateIfNotExists] = $"create table if not exists {tableName}" +
                $"(id integer not null primary key, JsonData text not null, Timestamp DateTime);";

            result[CommandType.Truncate] = $"delete from {tableName}";
            result[CommandType.Delete] = $"delete from {tableName} where (id = " + "{0} and id > 0) or (Timestamp < '{1}' and {2}=1)"; // delete by id or delete all before Timestamp
            result[CommandType.Select] = $"select * from {tableName}";
            result[CommandType.Insert] = $"insert into {tableName} values(null, " + "'{0}', '{1}')";

            return result;
        }
    }
}
