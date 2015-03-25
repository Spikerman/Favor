using Favor.Controller;
using Favor.DataModel;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class SendedMissions : Page
    {
        public SendedMissions()
        {
            this.InitializeComponent();
            //添加物理键返回前一页的响应
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += (sender, e) =>
            {
                //向系统表明我们对物理返回键按钮响应自行处理，必须放在一开始
                e.Handled = true;

                //有上一页可回退时
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }

            };
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await FavorUser.instance.RefreshSendedMissions();
            SendedMisssionListItems.ItemsSource = FavorUser.instance.sendedMissionCollection;
        }

        private void Back_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MissionsWall));
        }

        private async void CancelMissionButton(object sender, RoutedEventArgs e)
        {
            MobileServiceInvalidOperationException exception = null;

            Frame.IsEnabled = false;
            App.statusBar.ProgressIndicator.Text = "Deleting...";


            try
            {
                
                await App.statusBar.ProgressIndicator.ShowAsync();
                Button clicked = (Button)sender;
                Mission x = (Mission)clicked.DataContext;
                await MobileServiceTable.instance.missionItem.DeleteAsync(x);
                FavorUser.instance.sendedMissionCollection.Remove(x);
                await App.statusBar.ProgressIndicator.HideAsync();
                
                Frame.IsEnabled = true;
            }
            catch (MobileServiceInvalidOperationException ee)
            {
                exception = ee;
            }
            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loding, please check the Internet ").ShowAsync();
            }
            else
            {
                await new MessageDialog("Deleting success!").ShowAsync();
            }

        }

        private async void CompleteMissionButton(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            Mission x = (Mission)clicked.DataContext;
            await FavorUser.instance.MissionCompleteCheck(x);
        }
    }
}
