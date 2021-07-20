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
    public partial class Incontrol_page : TabbedPage
    {
        public Incontrol_page()
        {
            Boolean initialrun = true;
            Alert_Generator generator = new Alert_Generator();
            CrossMediaManager.Current.Init();
            InitializeComponent();
            Task.Run(() => 
            {
                while (true)
                {
                    Device.BeginInvokeOnMainThread(() => 
                    {
                        List<Frame> offline_alerts = generator.GenAlerts(offline_stack,"incontrol_offline");
                        List<Frame> bandwidth_alerts = generator.GenAlerts(bandwidth_stack,"incontrol_bandwidth");
                        //this if block just plays the sound for new events if the page is selected
                        if (App.Current.MainPage?.Navigation?.ModalStack?.LastOrDefault() == this && !initialrun)
                        {
                            //list of displayed style ids
                            List<string> old_list = new List<string>();
                            foreach (Frame frame in offline_stack.Children)
                            {
                                old_list.Add(frame.StyleId);
                            }
                            //for each of the new frames not displayed, play a sound 
                            for (int i = 0; i < offline_alerts.Count && !old_list.Contains(offline_alerts[i].StyleId); i++)
                            {
                                if (offline_alerts[i].BackgroundColor.Equals(Color.Red))
                                {
                                    CrossMediaManager.Current.PlayFromAssembly("device_off.mp3", App.Current.MainPage.GetType().Assembly);
                                }
                                else if (offline_alerts[i].BackgroundColor.Equals(Color.Green))
                                {
                                    CrossMediaManager.Current.PlayFromAssembly("device_on.mp3", App.Current.MainPage.GetType().Assembly);
                                }
                            }
                        }
                        offline_stack.Children.Clear();
                        bandwidth_stack.Children.Clear();
                        foreach(Frame frame in offline_alerts)
                        {
                            offline_stack.Children.Add(frame);
                        }
                        foreach(Frame frame in bandwidth_alerts)
                        {
                            bandwidth_stack.Children.Add(frame);
                        }
                    });
                    initialrun = false;
                    Thread.Sleep(10000);
                }
            });
        }
        
    }
}