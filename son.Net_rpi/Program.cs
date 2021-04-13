using System;
using System.Device.Gpio;
using System.Net.Http;

namespace son.Net_rpi
{
    class Program
    {
        static void Main(string[] args)
        {
            using var controller = new GpioController();
            using var http = new HttpClient();
            var ledpin = 18;
            var buttonpin = 1;
            
            
            controller.OpenPin(ledpin, PinMode.Output);
            controller.OpenPin(buttonpin, PinMode.InputPullUp);

            try
            {
                while (true)
                {
                    if (controller.Read(buttonpin) == false)
                    {
                        controller.Write(ledpin, PinValue.High);
                        
                        Console.WriteLine("Making API Call");
                        
                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri("https://apiSonnete"),
                        };
                        
                        HttpResponseMessage response = http.Send(request);
                        Console.Write(response.EnsureSuccessStatusCode());
                    }
                    
                    else
                    {
                        controller.Write(ledpin, PinValue.Low); 
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //Allow to delete state of the led
                controller.ClosePin(ledpin);
                throw;
            }
        }
    }
}