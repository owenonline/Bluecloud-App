using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Netcloud_App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Main_Menu();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
