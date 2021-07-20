using MediaManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Netcloud_App.content_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Netcloud_page : ContentPage
    {
        public Netcloud_page()
        {
            Boolean initialrun = true;
            //initializes page
            InitializeComponent();
            Alert_Generator generator = new Alert_Generator();
            CrossMediaManager.Current.Init();
            //begin running main loop in background thread
            Task.Run(() => 
            {
                while (true)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        List<Frame> alerts = generator.GenAlerts(alert_stack, "events");
                        //this if block just plays the sound for new events if the page is selected
                        if(App.Current.MainPage?.Navigation?.ModalStack?.LastOrDefault() == this && !initialrun)
                        {
                            List<string> old_list = new List<string>();
                            foreach (Frame frame in alert_stack.Children)
                            {
                                old_list.Add(frame.StyleId);
                            }
                            for(int i =0; i < alerts.Count && !old_list.Contains(alerts[i].StyleId); i++)
                            {
                                if (alerts[i].BackgroundColor.Equals(Color.Red))
                                {
                                    CrossMediaManager.Current.PlayFromAssembly("device_off.mp3", App.Current.MainPage.GetType().Assembly);
                                }
                                else if (alerts[i].BackgroundColor.Equals(Color.Green))
                                {
                                    CrossMediaManager.Current.PlayFromAssembly("device_on.mp3", App.Current.MainPage.GetType().Assembly);
                                }
                            }
                        }
                        alert_stack.Children.Clear();
                        foreach(Frame frame in alerts)
                        {
                            alert_stack.Children.Add(frame);
                        }
                    });
                    initialrun = false;
                    Thread.Sleep(10000);
                }
            });
        }
    }
}