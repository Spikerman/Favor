using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Windows.UI.Core;
using Favor.DataModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Favor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MissionWrite : Page
    {
        public MissionWrite()
        {
            this.InitializeComponent();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await FavorUser.instance.RefreshMissionsWall();
            //ListItems.ItemsSource = FavorUser.instance.missionCollection;
            this.SaveButton.IsEnabled = true;
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Frame.IsEnabled = false;
            var missionItem = new Mission { information = TextInput.Text, userId = FavorUser.instance.account.Id,publisher=FavorUser.instance.account.UserName,publisherImageUri=FavorUser.instance.account.UserImageUri };
            await FavorUser.instance.InsertMissionTable(missionItem);
            //RefreshListItems();
            Frame.IsEnabled = true;
            this.Frame.Navigate(typeof(MissionsWall));


        }

        private void PushingButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PushingList));
        }
    }
}
