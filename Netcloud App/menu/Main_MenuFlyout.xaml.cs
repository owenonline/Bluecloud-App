using Netcloud_App.content_pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Netcloud_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Main_MenuFlyout : ContentPage
    {
        public ListView ListView;

        public Main_MenuFlyout()
        {
            InitializeComponent();

            BindingContext = new Main_MenuFlyoutViewModel();
            ListView = MenuItemsListView;
        }

        class Main_MenuFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<Main_MenuFlyoutMenuItem> MenuItems { get; set; }

            public Main_MenuFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<Main_MenuFlyoutMenuItem>(new[]
                {
                    new Main_MenuFlyoutMenuItem { Id = 0, Title = "Netcloud Alerts", TargetType=typeof(Netcloud_page) },
                    new Main_MenuFlyoutMenuItem { Id = 1, Title = "InControl Alerts", TargetType=typeof(Incontrol_page) }
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}