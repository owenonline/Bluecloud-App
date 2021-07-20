using MediaManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Netcloud_App
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            Boolean initialrun = true;
            //initializes page
            InitializeComponent();
            CrossMediaManager.Current.Init();
            //begin running main loop in background thread
            Task.Run(() =>
            {
                while (true)
                {
                    //async invoke ui changes on main thread from background thread
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        using (WebClient wc = new WebClient())
                        {
                            //download the list of active events from the api
                            var jsonstr = wc.DownloadString("http://172.30.211.33:5000/get_all");

                            var list = JsonConvert.DeserializeObject<List<Alert>>(jsonstr);

                            List<Frame> alert_list = new List<Frame>();
                            List<String> active_events = new List<string>();
                            List<Alert> new_events = new List<Alert>();

                            //make a list of all the events displayed in the app
                            foreach (Frame element in alert_stack.Children)
                            {
                                active_events.Add(element.StyleId);
                            }

                            //subtracts the list of active events from the list of displayed events to get the list of new events
                            foreach (var obj in list)
                            {
                                if (!active_events.Contains(obj.EventKey))
                                {
                                    new_events.Add(obj);
                                }
                            }
                            //creates an alert object for each of the new events
                            foreach (var obj in new_events)
                            {
                                //make the screen flash a color and play a sound
                                Color color = Color.Default;
                                //sets color to green if online, red if offline
                                if (obj.Status == "online")
                                {
                                    color = Color.Green;
                                }
                                else
                                {
                                    color = Color.Red;
                                }
                                
                                Frame alert_frame = new Frame
                                {
                                    BorderColor = Color.Black,
                                    BackgroundColor = color,
                                    StyleId = obj.EventKey,
                                    Content = new StackLayout
                                    {
                                        Orientation = Xamarin.Forms.StackOrientation.Horizontal,
                                        Padding = 0,
                                        Margin = 0,
                                        Children =
                                            {
                                                GetChildren(obj,color,wc)[0],
                                                GetChildren(obj,color,wc)[1]
                                            }
                                    }

                                };
                                //add alert object to stack layout
                                alert_stack.Children.Add(alert_frame);
                                //play alert sound on subsequent runs
                                if (initialrun == false)
                                {
                                    if (color == Color.Green)
                                    {
                                        CrossMediaManager.Current.PlayFromAssembly("device_on.mp3", typeof(Netcloud_App.MainPage).Assembly);
                                    }
                                    else
                                    {
                                        CrossMediaManager.Current.PlayFromAssembly("device_off.mp3", typeof(Netcloud_App.MainPage).Assembly);
                                    }
                                }
                                //ensures that audio doesn't play on app startup
                                initialrun = false;
                            }
                        }
                    });
                    //wait 20 seconds for refresh
                    Thread.Sleep(20000);
                }
            });
        }
        public List<View> GetChildren(Alert obj, Color color, WebClient wc)
        {
            //creates info button with popup for the event readout
            Button info = new Button()
            {
                BackgroundColor = color,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                FontSize = 30,
                FontAttributes = FontAttributes.Bold,
                Text = "Info"
            };
            info.Clicked += async (sender, args) => await DisplayAlert("Event Info", obj.Change, "Okay");

            //creates delete button that deletes the parent alert object from the stack layout and sends the command to the api to remove it from the SQL table
            Button delete = new Button()
            {
                BackgroundColor = color,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                FontSize = 30,
                FontAttributes = FontAttributes.Bold,
                Text = "Delete"
            };
            delete.Clicked += async (sender, args) => await Remove(obj, wc);

            List<View> alert_frames = new List<View>();

            alert_frames.Add(info);
            alert_frames.Add(delete);

            return alert_frames;
        }
        async Task Remove(Alert obj, WebClient wc)
        {
            //finds the id of the parent alert object and removes it from the alert stack
            View frame = new Frame();
            foreach(Frame element in alert_stack.Children)
            {
                if (element.StyleId == obj.EventKey)
                {
                    frame = element;
                }
            }

            alert_stack.Children.Remove(frame);

            //sends the remove command for the event's id to the api
            var jsonstr = wc.DownloadString("http://172.30.211.33:5000/remove?id="+ obj.EventKey);
        }
    }
    public class Alert
    {
        //public class for alert objects, with the same three parameters as the SQL table's columns
        public string EventKey { get; set; }
        public string Status { get; set; }
        public string Change { get; set; }
        public Alert(string eventkey, string status, string change)
        {
            EventKey = eventkey;
            Status = status;
            Change = change;
        }
    }
}
