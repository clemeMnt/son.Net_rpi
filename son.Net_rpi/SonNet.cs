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

        //Constructor 
        //Allow to retrieve configurations variables
        public SonNet(IOptions<AppConfiguration> config)
        {
            this._config = config.Value;
        }

        //Function automatically runner when the class is run
        public void Run()
        {         
            //Define object controller to process Raspberry
            using var controller = new GpioController();
            
            controller.OpenPin(_config.ledPin, PinMode.Output);
            controller.OpenPin(_config.boutonPin, PinMode.InputPullUp);
            
            //Loop to avoid runnable error
            try
            {
                //while function allow to create a infinite loop 
                while (true)
                {          
                    //Process the code if the button is push
                    if (controller.Read(_config.boutonPin) == false)
                    {
                        Console.WriteLine("Making API Call");
                        if (CallAPI())
                        {
                            //Blinding led when the request 'ring' is succeed
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
                            //Blinding led when the request 'ring' is not succeed
                            //Error can due to internet connection or bad response 
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

        //Function to request the server
        //return bool if the request is succeed
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
