using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using System.Data;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Timers;

namespace redis_cache.Controllers
{

     

    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        public readonly IConfiguration _Configuration;

        //private readonly RequestDelegate _next;

        private readonly IDistributedCache _distributedCache;

        List<RedisClass> myTodos = new List<RedisClass>();

        List<RedisClass> redis = new List<RedisClass>();
        List<cmsclass> cms = new List<cmsclass>();
        List<TblSubPreferance> aa = new List<TblSubPreferance>();

        public RedisController(IDistributedCache distributedCache,IConfiguration configuration)
        {
            _Configuration=configuration;
            _distributedCache = distributedCache;
        }

        /*private var startTimeSpan = TimeSpan.Zero;

        private var periodTimeSpan = TimeSpan.FromMinutes(1);

        var Timer = new System.Threading.Timer((e) => {
            trans();

        },null, startTimeSpan,;*/

        [HttpPost]
        [Route("all")]
        public async Task<IActionResult> insert(RedisClass redisClass)
        {
            

            List<RedisClass> myTodos = new List<RedisClass>();
            HttpClient client = new HttpClient();

            long a = 0;

            bool j = true;
            bool IsCached = false;
            string cachedTodosString = string.Empty;
            cachedTodosString = await _distributedCache.GetStringAsync("_todos4");
            if (j = true)
            {
                // loaded data from the redis cache.
                myTodos = redis;
                // todos.Include(x => x.a);

                myTodos = JsonSerializer.Deserialize<List<RedisClass>>(cachedTodosString);
                myTodos.Add(redisClass);
                cachedTodosString = JsonSerializer.Serialize<List<RedisClass>>(myTodos);
                await _distributedCache.SetStringAsync($"_todos4 ", cachedTodosString);



                /* string sqlDataSource = _Configuration.GetConnectionString("conn");
                 NpgsqlConnection conn = new NpgsqlConnection(sqlDataSource);
                 conn.Open();
                 NpgsqlCommand command = new NpgsqlCommand();
                 command.Connection = conn;
                 command.CommandType = CommandType.Text;
                 command.CommandText = $"select count(id) from redismaster";
                 a = Convert.ToInt64(command.ExecuteScalar());



                 for (int i = (int)a; i<=(myTodos.Count()-1); i++)
                 {
                     Task.Delay(5000).Wait();
                     command.CommandType = CommandType.Text;
                     command.CommandText = $"insert into redismaster (name,description,mobilenumber) values ('{myTodos[i].name}','{myTodos[i].description}','{myTodos[i].phoneno}')";
                     command.ExecuteNonQuery();
                 }*/
            }

            /*using HttpResponseMessage response = await client.GetAsync("https://localhost:7299/api/Redis/transfardata");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();   */

            return Ok(new { success = "Inserted" });
        }


        [HttpGet]
        [Route("transfardata")]

        public async Task<ActionResult> transfar()
        {
            static void TimerMethod(object? state) { }
            long a = 0;

            await Task.Delay(20000);
            string cachedTodosString = string.Empty;
            cachedTodosString = await _distributedCache.GetStringAsync("_todos4");

            myTodos = JsonSerializer.Deserialize<List<RedisClass>>(cachedTodosString);

            string sqlDataSource = _Configuration.GetConnectionString("conn");
            NpgsqlConnection conn = new NpgsqlConnection(sqlDataSource);
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = conn;
            command.CommandType = CommandType.Text;
            command.CommandText = $"select count(id) from redismaster";
            a = Convert.ToInt64(command.ExecuteScalar());


            //await Task.Delay(10000);
            for (int i = (int)a; i <= (myTodos.Count() - 1); i++)
            {

                command.CommandType = CommandType.Text;
                command.CommandText = $"insert into redismaster (name,description,mobilenumber) values ('{myTodos[i].name}','{myTodos[i].description}','{myTodos[i].phoneno}')";
                command.ExecuteNonQuery();
            }
            return Ok(new { success = "Inserted" });
        }

        
        [HttpGet]
        [Route("getid")]
        public async Task<ActionResult> Get()
        {
            string sqlDataSource = _Configuration.GetConnectionString("conn");
            NpgsqlConnection conn = new NpgsqlConnection(sqlDataSource);
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = conn;
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from redismaster";
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var list = new RedisClass();
                list.name = reader.GetString("name");
                list.description = reader.GetString("description");
                list.phoneno = reader.GetInt64("mobilenumber");
                /*list.subId = reader.GetInt16("subId");*/
                redis.Add(list);
            }
            conn.Close();

            
            bool IsCached = false;
            string cachedTodosString = string.Empty;
            cachedTodosString = await _distributedCache.GetStringAsync($"_todos4");

            if (!string.IsNullOrEmpty(cachedTodosString))
            {
                // loaded data from the redis cache.
                myTodos = JsonSerializer.Deserialize<List<RedisClass>>(cachedTodosString);
                IsCached = true;
            }
            else
            {
                // loading from code (in real-time from database)
                // then saving to the redis cache 
                myTodos = redis;
                IsCached = false;
                cachedTodosString = JsonSerializer.Serialize<List<RedisClass>>(myTodos);
                await _distributedCache.SetStringAsync($"_todos4", cachedTodosString);
            }
            return Ok(new { IsCached, myTodos });
        }


        [HttpGet]
        [Route("getinsindedatadetalils")]

        public async Task<ActionResult> GetdetailsAsync(int id, int id1)
        {
            string sqlDataSource = _Configuration.GetConnectionString("conn2");
            NpgsqlConnection conn = new NpgsqlConnection(sqlDataSource);
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = conn;
            command.CommandType = CommandType.Text;
            command.CommandText = $"select * from cms_get_detaildata({id},{id1})";
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var list = new cmsclass();
                
                list.title = reader.GetString("title");
                list.description = reader.GetString("description");
                list.image = reader.GetString("image");
                list.subPreferenceId = reader.GetInt32("subId");
               
                cms.Add(list);
            }
            conn.Close();


            List<cmsclass> myTodos = new List<cmsclass>();
            bool IsCached = false;
            string cachedTodosString = string.Empty;
            cachedTodosString = await _distributedCache.GetStringAsync($"todos={id}and{id1}");

            if (!string.IsNullOrEmpty(cachedTodosString))
            {
                // loaded data from the redis cache.
                myTodos = JsonSerializer.Deserialize<List<cmsclass>>(cachedTodosString);
                IsCached = true;
            }
            else
            {
                // loading from code (in real-time from database)
                // then saving to the redis cache 
                myTodos = cms;
                IsCached = false;
                cachedTodosString = JsonSerializer.Serialize<List<cmsclass>>(cms);
                await _distributedCache.SetStringAsync($"todos={id}and{id1}", cachedTodosString);
            }


            return Ok(new { IsCached, myTodos });
           /* return Ok(cmsclass);*/
        }


        [HttpGet]
        [Route("transfardatas")]

        public async Task<ActionResult> trans()
        {


            long a = 0;
            
            List<cmsclass> myTodos = new List<cmsclass>();
            string cachedTodosString = string.Empty;
            cachedTodosString = await _distributedCache.GetStringAsync("_todos");

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
            }
            a = Convert.ToInt64(command.ExecuteScalar());



            //await Task.Delay(10000);
            for (int i = (int)a; i <= (myTodos.Count() - 1); i++)
            {

                command.CommandType = CommandType.Text;
                command.CommandText = $"Select insert_data($${myTodos[i].title}$$,$${myTodos[i].description}$$,'{myTodos[i].image}',{myTodos[i].prefId},{myTodos[i].subPreferenceId})";
                command.ExecuteNonQuery();
            }
            return Ok(new { success = "Inserted" });
        }

        [HttpPost]
        public async Task<ActionResult> postAsync(cmsclass data)
        {
            

            List<cmsclass> myTodos = new List<cmsclass>();
            HttpClient client = new HttpClient();

            long a = 0;

            bool j = true;
            bool IsCached = false;
            string cachedTodosString = string.Empty;
            cachedTodosString = await _distributedCache.GetStringAsync($"todos={data.prefId}and{data.subPreferenceId}");
            if (j = true)
            {
                // loaded data from the redis cache.
                myTodos = cms;
                IsCached = false;
                // todos.Include(x => x.a);

                myTodos = JsonSerializer.Deserialize<List<cmsclass>>(cachedTodosString);
                myTodos.Add(data);
                cachedTodosString = JsonSerializer.Serialize<List<cmsclass>>(myTodos);
                await _distributedCache.SetStringAsync($"todos={data.prefId}and{data.subPreferenceId}", cachedTodosString);
                }
            return Ok(new { success = "Inserted" });

            /*            string sqlConnection = _Configuration.GetConnectionString("conn2");
                        NpgsqlConnection con = new NpgsqlConnection(sqlConnection);
                        con.Open();
                        NpgsqlCommand cmd = new NpgsqlCommand($"Select insert_data('{data.title}',$${data.description}$$,'{data.image}',{data.prefId},{data.subPreferenceId})", con);
                        cmd.ExecuteNonQuery();
                        //con.Close();

                        await _distributedCache.RemoveAsync($"todos={data.prefId}and{data.subPreferenceId}");
                        await _distributedCache.RemoveAsync($"todos=1and0");

                        return Ok(new { success = "Inserted" });*/
        }


        [HttpGet]
        [Route("getsubs")]

        public ActionResult Get_sub_data(int id)
        {
            string sqlDataSource = _Configuration.GetConnectionString("conn2");
            NpgsqlConnection conn = new NpgsqlConnection(sqlDataSource);
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = conn;
            command.CommandType = CommandType.Text;
            command.CommandText = $"select * from cms_get_subdatatypes({id})";
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var list = new TblSubPreferance();
                list.id = reader.GetInt32("a_id");
                list.name = reader.GetString("title");
                list.pref_id = reader.GetInt16("p_id");

                aa.Add(list);
            }
            conn.Close();
            return Ok(aa);
        }

    }
}
