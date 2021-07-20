using MediaManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Netcloud_App.content_pages
{
    public class Alert_Generator : ContentPage
    {
        public Alert_Generator()
        {
            CrossMediaManager.Current.Init();
        }

        public List<Frame> GenAlerts(StackLayout parent, string type)
        {
            using (WebClient wc = new WebClient())
            {
                List<Frame> return_list = new List<Frame>();

                var jsonstr = wc.DownloadString("http://172.30.211.33:5000/get_all?db=" + type);

                var list = JsonConvert.DeserializeObject<List<Alert>>(jsonstr);

                foreach (var obj in list)
                {
                    Color color = Color.Blue;
                    string banner = obj.Status + " alert";
                    Boolean visible = false;
                    if (type == "events" || type == "incontrol_offline")
                    {
                        if (obj.Status == "online")
                        {
                            color = Color.Green;
                        }
                        else
                        {
                            color = Color.Red;
                        }
                    }
                    if (type == "incontrol_bandwidth")
                    {
                        banner = obj.Name + ": " + obj.Status + " usage";
                        visible = true;
                    }
                    if (type == "incontrol_offline")
                    {
                        banner = obj.Name + " is " + obj.Status;
                        visible = true;
                    }
                    Frame alert_frame = new Frame
                    {
                        BorderColor = Color.Black,
                        BackgroundColor = color,
                        StyleId = obj.EventKey,
                        Content = new StackLayout
                        {
                            Orientation = Xamarin.Forms.StackOrientation.Vertical,
                            Padding = 0,
                            Margin = 0,
                            Children =
                            {
                                new Label
                                {
                                    Text=banner,
                                    TextColor=Color.White,
                                    HorizontalTextAlignment=Xamarin.Forms.TextAlignment.Center,
                                    FontSize=30,
                                    FontAttributes = FontAttributes.Bold,
                                    IsVisible=visible
                                },
                                new StackLayout
                                {
                                    Orientation=Xamarin.Forms.StackOrientation.Horizontal,
                                    Padding=0,
                                    Margin=0,
                                    Children =
                                    {
                                        GetChildren(obj,color,wc,type,parent)[0],
                                        GetChildren(obj,color,wc,type,parent)[1]
                                    }
                                }
                            }
                        }
                    };
                    return_list.Add(alert_frame);
                }

                return return_list;
            }
        }
        public List<View> GetChildren(Alert obj, Color color, WebClient wc, string type,StackLayout parent)
        {
            //creates info button with popup for the event readout
            Button info = new Button()
            {
                BackgroundColor = color,
                BorderColor=Color.White,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                FontSize = 20,
                Text = "Info"
            };
            info.Clicked += async (sender, args) => App.Current.MainPage.DisplayAlert("Event Info", obj.Change, "Okay");

            //creates delete button that deletes the parent alert object from the stack layout and sends the command to the api to remove it from the SQL table
            Button delete = new Button()
            {
                BackgroundColor = color,
                BorderColor= Color.White,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                FontSize = 20,
                Text = "Delete"
            };
            delete.Clicked += async (sender, args) => await Remove(obj, wc, type,parent);

            List<View> alert_frames = new List<View>();

            alert_frames.Add(info);
            alert_frames.Add(delete);

            return alert_frames;
        }

        async Task Remove(Alert obj, WebClient wc, string type, StackLayout parent)
        {
            //finds the id of the parent alert object and removes it from the alert stack
            View frame = new Frame();
            foreach (Frame element in parent.Children)
            {
                if (element.StyleId == obj.EventKey)
                {
                    frame = element;
                }
            }

            parent.Children.Remove(frame);

            //sends the remove command for the event's id to the api
            var jsonstr = wc.DownloadString("http://172.30.211.33:5000/remove?db="+type+"&id=" + obj.EventKey);
        }
    }
}
