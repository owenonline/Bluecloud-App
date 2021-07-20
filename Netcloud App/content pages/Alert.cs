using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Netcloud_App.content_pages
{
    public class Alert : ContentPage
    {
        public string EventKey { get; set; }
        public string Status { get; set; }
        public string Change { get; set; }
        public string Name { get; set; }
        public Alert(string eventkey, string status, string change, string name="")
        {
            EventKey = eventkey;
            Status = status;
            Change = change;
            Name = name;
        }
    }
}
