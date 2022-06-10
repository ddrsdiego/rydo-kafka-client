namespace Rydo.Kafka.Client.Sample.Consumers
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;
    using Serilog.Exceptions;
    using Serilog.Exceptions.Core;
    using Serilog.Formatting.Elasticsearch;
    using Serilog.Sinks.SystemConsole.Themes;

    public class Program
    {
        private const string EnvironmentProduction = "Development";
        private const string AspnetcoreEnvironment = "ASPNETCORE_ENVIRONMENT";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseSerilog((_, loggerConfiguration) =>
                            CreateLoggerConfiguration(Configuration, loggerConfiguration));
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