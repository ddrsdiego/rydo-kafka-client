namespace Rydo.Kafka.Client.Sample
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Core.Consumers;
    using Core.Repositories;
    using Extensions;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using Serilog;
    using Serilog.Events;
    using Serilog.Exceptions;
    using Serilog.Exceptions.Core;
    using Serilog.Formatting.Elasticsearch;
    using Serilog.Sinks.SystemConsole.Themes;

    public static class Program
    {
        private const string EnvironmentProduction = "Development";
        private const string AspnetcoreEnvironment = "ASPNETCORE_ENVIRONMENT";
        
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseSerilog((_, loggerConfiguration) =>
                            CreateLoggerConfiguration(Configuration, loggerConfiguration));
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddControllers();
                    services.AddSingleton<IAccountRepository, AccountRepository>();
                    services.AddAsyncServices(context.Configuration, typeof(MessageHandlerConsumerHandler));
                    services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo {Title = Assembly.GetEntryAssembly()?.GetName().Name});
                    });
                });

        private static void CreateLoggerConfiguration(IConfiguration configuration,
            LoggerConfiguration loggerConfiguration)
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();

            var isDebug = configuration.GetSection("IsDebug").Get<bool>();

            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Destructure.ToMaximumCollectionCount(10)
                .Destructure.ToMaximumStringLength(1024)
                .Destructure.ToMaximumDepth(5)
                .Enrich.WithProperty("Jornada", "Foundations")
                .Enrich.WithProperty("Assembly", $"{assembly.Name}")
                .Enrich.WithProperty("Version", $"{assembly.Version}")
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers()
                    .WithRootName("Exception"));

            if (isDebug)
            {
                loggerConfiguration.WriteTo.Async(sinkConfigurations =>
                    sinkConfigurations.Console(
                        outputTemplate:
                        "{Timestamp:HH:mm:ss} {Level:u3} => {Message:lj}{Properties:j}{NewLine}{Exception}",
                        restrictedToMinimumLevel: LogEventLevel.Debug, theme: AnsiConsoleTheme.Code));
            }
            else
            {
                loggerConfiguration.WriteTo.Async(sinkConfigurations =>
                    sinkConfigurations.Console(new ElasticsearchJsonFormatter(inlineFields: true,
                        renderMessageTemplate: false)));
            }
        }
        
        private static string Env => Environment.GetEnvironmentVariable(AspnetcoreEnvironment) ?? EnvironmentProduction;

        private static IConfiguration Configuration
        {
            get
            {
                var appSettingsJson = $"appsettings.{Env}.json";
                return new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile(appSettingsJson, optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
            }
        }
    }
}