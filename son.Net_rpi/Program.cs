using System;
using son.Net_rpi.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Device.Gpio;
using System.Net.Http;
using System.Threading;
using System.IO;

namespace son.Net_rpi
{
    class Program
    {
        private static ServiceCollection Services { get; set; }

        static void Main(string[] args)
        {
            Services = new ServiceCollection();
            ConfigureServices(Services);

            IServiceProvider serviceProvider = Services.BuildServiceProvider();

            try
            {
                serviceProvider.GetService<SonNet>().Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            services.AddSingleton(configuration);

            services.Configure<AppConfiguration>(configuration.GetSection("AppConfiguration"));

            // Add app
            services.AddTransient<SonNet>();
        }



    }
}