using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Text.Json;

namespace WorkerServicefortimer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        System.Timers.Timer timer;
        double processTimer = 3 * 60 * 1000; // 3 minutes interval in milli seconds
        private readonly IConfiguration _Configuration;

        private readonly IDistributedCache _distributedCache;

        //WorkflowProcessing objProcessing = new WorkflowProcessing();

        public Worker(ILogger<Worker> logger, IDistributedCache distributedCache, IConfiguration configuration)
        {
            _logger = logger;
            _Configuration = configuration;
            _distributedCache = distributedCache;
            InitializeTimer();
            trans();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public void InitializeTimer()
        {
            timer = new System.Timers.Timer(processTimer); // Interval for 5 minutes
            timer.Elapsed += (sender, e) => HandleTimer();
            timer.AutoReset = false;
            timer.Start();
            Console.Write("Press any key to exit... ");
            //Console.ReadKey();
        }

        private void HandleTimer()
        {
            trans();
            Console.WriteLine("\nTimer Call...");

            timer.Start();
        }


        public async Task<int> trans()
        {

            long a = 0;

            List<cmsclass> myTodos = new List<cmsclass>();
            
            

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

                string cachedTodosString;
                cachedTodosString = await _distributedCache.GetStringAsync($"todos_u={id}and0");
                myTodos = JsonSerializer.Deserialize<List<cmsclass>>(cachedTodosString);


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