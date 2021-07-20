using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Netcloud_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Main_MenuDetail : ContentPage
    {
        public Main_MenuDetail()
        {
            InitializeComponent();
            Image image = new Image
            {
                Source = ImageSource.FromResource("Netcloud_App.assets.bluecloud.png", typeof(Main_MenuDetail).GetTypeInfo().Assembly)
            };
            var assembly = typeof(Main_MenuDetail).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                System.Diagnostics.Debug.WriteLine("found resource: " + res);
            }
            menu_stack.Children.Add(image);
        }
    }
}