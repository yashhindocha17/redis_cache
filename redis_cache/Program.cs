using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using StackExchange.Redis;
using System.Configuration;
using Timer = System.Timers.Timer;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        /*builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

        builder.Services.AddScoped<NpgsqlConnection>(_ => new NpgsqlConnection(builder.Configuration.GetConnectionString("PostgreSQL")));
        */



        builder.Services.AddCors(c =>
        {
            c.AddPolicy("AllowOrigin", Options => Options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });


        builder.Services.AddDistributedRedisCache(
                        options =>
                        {
                            options.Configuration = "localhost:6379";
                        });

        var app = builder.Build();

        static void TIMER()
        {
            Timer x = new Timer(5000);
            x.AutoReset = true;
            x.Elapsed += new System.Timers.ElapsedEventHandler(MyTimer);
            x.Start();
            Console.WriteLine("hi");
            Console.Read();
        }
        static void MyTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("5 seccond");
        }
        

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        };


        app.UseHttpsRedirection();

        app.UseCors("AllowOrigin");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}