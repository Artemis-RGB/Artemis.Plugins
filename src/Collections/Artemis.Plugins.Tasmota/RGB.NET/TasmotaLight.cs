using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Plugins.Tasmota.RGB.NET
{
    public class TasmotaLight
    {
        public bool AutoRefreshEnabled { get; set; }
        public bool Connected { get; set; }
        public IPAddress Ip { get; private set; }

        public void ConnectAsync(IPAddress p)
        {
            Console.WriteLine("ConnectAsync", p);
            this.Connected = true;
            this.Ip = p;
        }

        public void TurnOnAsync()
        {
            Console.WriteLine("TurnOnAsync");
        }

        public async void SetColorAsync(Color color)
        {
            using (var httpClient = new HttpClient())
            {
                var name = color.Name.Substring(2);
                var url = "http://" + this.Ip + "/cm?cmnd=Color%20%23" + name;
                httpClient.BaseAddress = new Uri("http://" + this.Ip + "/");
                var result = httpClient.GetAsync("cm?cmnd=Color%20%23" + name);
                result.Wait();
            }
        }
    }
}
