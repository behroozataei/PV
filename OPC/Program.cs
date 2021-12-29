using Irisa.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace OPC
{
    class Program
    {
        private static IHost _host;
        private static ILogger _logger;

        static void Main(string[] args)
        {
            _host = CreateHostBuilder(args).Build();
            _logger = _host.Services.GetService<ILogger>();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            try
            {
                _host.Run();
            }
            catch (Exception ex)
            {
                _logger.WriteEnteryInFile($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(serviceCollection =>
                {
                    var logger = new Logger("OPC");
                    serviceCollection.AddSingleton<ILogger>(logger);
                    serviceCollection.AddSingleton(serviceCollection);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<WorkerService>();

                }).UseWindowsService();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            _logger.WriteEnteryInFile($"{exception.Message}\n{exception.StackTrace}");
        }
    }
}
