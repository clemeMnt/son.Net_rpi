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
          
            //Define object to declare a new service
            Services = new ServiceCollection();
            ConfigureServices(Services);
            IServiceProvider serviceProvider = Services.BuildServiceProvider();

            try
            {
                Console.WriteLine("lets try yo use service");
                //Define service SonNet 
                serviceProvider.GetService<SonNet>().Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //Allow to configure configuration variable of external json
        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            services.AddSingleton(configuration);

            //Add configuration file in the SonNet application
            services.Configure<AppConfiguration>(configuration.GetSection("AppConfiguration"));
            services.AddTransient<SonNet>();
        }



    }
}