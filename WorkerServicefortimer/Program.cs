using WorkerServicefortimer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        services.AddDistributedRedisCache(
                        options =>
                        {
                            options.Configuration = "localhost:6379";
                        });
    })
    .Build();


await host.RunAsync();
