using System;
using System.Device.Gpio;
using System.Net.Http;
using System.Threading;

namespace son.Net_rpi
{
    class Program
    {
        static void Main(string[] args)
        {
            using var controller = new GpioController();
           
            var ledpin = 18;
            var buttonpin = 1;

            controller.OpenPin(ledpin, PinMode.Output);
            controller.OpenPin(buttonpin, PinMode.Input);

            try
            {
                while (true)
                {
                    var button = controller.Read(buttonpin);
                    if (button.Equals(true))
                    {
                        Console.WriteLine("Making API Call");
                        if (CallAPI())
                        {
                            controller.Write(ledpin, PinValue.High);
                            Thread.Sleep(1000);
                            controller.Write(ledpin, PinValue.Low);
                            Thread.Sleep(1000);
                            controller.Write(ledpin, PinValue.High);
                            Thread.Sleep(1000);
                            controller.Write(ledpin, PinValue.Low);

                        }
                        else
                        {
                            controller.Write(ledpin, PinValue.High);
                            Thread.Sleep(3000);
                            controller.Write(ledpin, PinValue.Low); 
                        }
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

        private static bool CallAPI()
        {
            using var http = new HttpClient();
            
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://apiSonnete"),
            };
                        
            HttpResponseMessage response = http.Send(request);

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