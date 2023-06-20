using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

using Npgsql;
using System.Data;
using System.Text.Json;

namespace WorkerServicefortimer
{
    internal class WorkflowProcessing
    {
        public IConfiguration _Configuration;

        private IDistributedCache _distributedCache;


        List<cmsclass> cms = new List<cmsclass>();

        //public WorkflowProcessing(IDistributedCache distributedCache, IConfiguration configuration)
        //{
        //    _Configuration = configuration;
        //    _distributedCache = distributedCache;
        //}

        public  async  Task<int> trans()
        {
            
            long a = 0;

            List<cmsclass> myTodos = new List<cmsclass>();
            string cachedTodosString ;
            cachedTodosString = await _distributedCache.GetStringAsync("todos_u=1and0");

            myTodos = JsonSerializer.Deserialize<List<cmsclass>>(cachedTodosString);

            string sqlDataSource = _Configuration.GetConnectionString("conn2");
            NpgsqlConnection conn = new NpgsqlConnection(sqlDataSource);
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = conn;
            command.CommandType = CommandType.Text;
            for (int id = 1; id <= 3; id++)
            {
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
            }
            return 0;

        }
        
    }
}