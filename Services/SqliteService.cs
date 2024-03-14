using System.Data;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using SQLitePCL;

namespace KitchenBuddyAPI.Services;

public interface ISqliteProvider
{
    string[] Read(string query);
}

public class SqliteProvider : ISqliteProvider
{
    private IConfiguration _configuration;
    private SqliteConnection _conn;
    public SqliteProvider(IConfiguration config)
    {
        _configuration = config;
        _conn = new SqliteConnection(_configuration.GetConnectionString("SqliteConn"));
        _conn.Open();
    }

    public string[] Read(string query)
    {
        var resultBuilder = new StringBuilder();
        var command = _conn.CreateCommand();
        command.CommandText = query;

        var results = new List<string>();

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var split = "|";
                    if(i == reader.FieldCount - 1)
                        split = "";

                    resultBuilder.Append(reader.GetString(i) + split);
                }

                results.Add(resultBuilder.ToString());        
            }
        }
        
        return results.ToArray();
    }
}