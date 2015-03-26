using Favor.Controller;
using Favor.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Favor.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReceivedMissions : Page
    {
        public ReceivedMissions()
        {
            this.InitializeComponent();
            //添加物理键返回前一页的响应
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += (sender, e) =>
            {
                //向系统表明我们对物理返回键按钮响应自行处理，必须放在一开始
                e.Handled = true;

                Frame.Navigate(typeof(MissionsWall));

            };
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            App.statusBar.ProgressIndicator.Text = "Loading information...";
            await App.statusBar.ProgressIndicator.ShowAsync();
            await FavorUser.instance.RefreshReceivedMission();
            await App.statusBar.ProgressIndicator.HideAsync();
            App.statusBar.ProgressIndicator.Text = "Loading...";
            MisssionListItems.ItemsSource = FavorUser.instance.receivedMissionCollection;
        }



        private void Back_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MissionsWall));
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Button cancelButton = (Button)sender;
            Mission cancelMission = (Mission)cancelButton.DataContext;
            await FavorUser.instance.CancelReceivedMission(cancelMission);

            App.statusBar.ProgressIndicator.Text = "Loading information...";
            await App.statusBar.ProgressIndicator.ShowAsync();
            await FavorUser.instance.RefreshReceivedMission();
            await App.statusBar.ProgressIndicator.HideAsync();
            App.statusBar.ProgressIndicator.Text = "Loading...";
            MisssionListItems.ItemsSource = FavorUser.instance.receivedMissionCollection;
        }
    }
}
