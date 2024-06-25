using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ReadToCSV
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main()
        {

            try
            {
                var handler = new HttpClientHandler();
                handler.Credentials = new System.Net.NetworkCredential(@"thaiunion\service.webbase", "Tuna@40wb*");
                HttpClient client = new HttpClient(handler);
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Format("{0}:{1}", "admin", "admin"))));
                //  client.BaseAddress = new Uri(URL);
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("http://192.168.1.170:90/ServiceCS.asmx/GetUpdateTOSQL?Data=ddd");

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response received: {responseBody}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine($"Message :{e.Message} ");
            }
        }
    }
}
