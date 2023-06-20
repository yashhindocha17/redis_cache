using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace redis_cache
{

    public class RedisClass
    {
        static private readonly IDistributedCache _distributedCache;

        public readonly IConfiguration _Configuration;


        public string name { get; set; }

        public string description { get; set; }

        public long phoneno { get; set; }

       // List<cmsclass> myTodos = new List<cmsclass>();

       /* public async Task<JsonResult> trans(int id)
        {
            long a = 0;

            
            string cachedTodosString = string.Empty;
            cachedTodosString = await _distributedCache.GetStringAsync($"todos_u={id}and0");

            myTodos = JsonSerializer.Deserialize<List<cmsclass>>(cachedTodosString);

            string sqlDataSource = _Configuration.GetConnectionString("conn2");
            NpgsqlConnection conn = new NpgsqlConnection(sqlDataSource);
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = conn;
            command.CommandType = CommandType.Text;
            
            if (id == 1)
            {
                command.CommandText = $"select count(sport_id) from tblsportmaster where sport_status = true";
            }
            else if (id == 2)
            {
                command.CommandText = $"select count(politics_id) from tblpoliticsmaster where politics_status=true";
            }
            else
            {
                command.CommandText = $"select count(technology_id) from tbltechnologymaster where technology_status=true";
            }
            
            a = Convert.ToInt64(command.ExecuteScalar());

            //await Task.Delay(10000);

            for (int i = (int)a; i <= (myTodos.Count() - 1); i++)
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"Select insert_data($${myTodos[i].title}$$,$${myTodos[i].description}$$,'{myTodos[i].image}',{myTodos[i].prefId},{myTodos[i].subPreferenceId})";
                command.ExecuteNonQuery();
            }
            var obj = new { success = "Inserted" };
            return new JsonResult(obj);
        }*/
    }

    public class TblSubPreferance
    {
        public int id { get; set; }

        public string name { get; set; }

        public int pref_id { get; set; }

    }
}
