using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using son.Net_rpi.Configuration;
using System.Device.Gpio;
using System.Threading;

namespace son.Net_rpi
{
    class SonNet
    {
        private readonly AppConfiguration _config;

        public SonNet(IOptions<AppConfiguration> config)
        {
            this._config = config.Value;
        }

        public void Run()
        {         
            using var controller = new GpioController();

            controller.OpenPin(_config.ledPin, PinMode.Output);
            controller.OpenPin(_config.boutonPin, PinMode.InputPullUp);
            
            try
            {
                while (true)
                {                 
                    if (controller.Read(_config.boutonPin) == false)
                    {
                        Console.WriteLine("Making API Call");
                        if (CallAPI())
                        {
                            controller.Write(_config.ledPin, PinValue.High);
                            Thread.Sleep(1000);
                            controller.Write(_config.ledPin, PinValue.Low);
                            Thread.Sleep(1000);
                            controller.Write(_config.ledPin, PinValue.High);
                            Thread.Sleep(1000);
                            controller.Write(_config.ledPin, PinValue.Low);
                            Thread.Sleep(3000);

                        }
                        else
                        {
                            controller.Write(_config.ledPin, PinValue.High);
                            Thread.Sleep(5000);
                            controller.Write(_config.ledPin, PinValue.Low);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //Allow to delete state of the led
                controller.ClosePin(_config.ledPin);
                throw;
            }
        }

        private bool CallAPI()
        {
          
            using var http = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://" + _config.UrlServer + "/sound"),
            };
            
            Task<HttpResponseMessage> task = http.SendAsync(request);
            HttpResponseMessage response = task.Result;

            if (response.IsSuccessStatusCode)
            {
                Console.Write("Request succeed");
                return true;
            }
            else
            {
                Console.Write("Request failed");
                return false;
            }
        }

    }
}
