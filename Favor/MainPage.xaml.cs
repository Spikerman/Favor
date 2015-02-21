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
using Newtonsoft.Json;
using Windows.UI.Core;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace Favor
{

    public class Mission
    {
        public string id { get; set; }
        
        [JsonProperty(PropertyName="information")]
        public string information { get; set; }
        [JsonProperty(PropertyName="completed")]
        public bool completed { get; set; }

        //public int receivednum;


    }

    public sealed partial class MainPage : Page
    {

        private MobileServiceCollection<Mission, Mission> items;//两个参数
        private IMobileServiceTable<Mission> mission = App.MobileService.GetTable<Mission>();
        public MainPage()
        {
            this.InitializeComponent();
           
            //this.Loaded += MainPage_Loaded;
           // this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async Task InsertMissionTable(Mission entryItem)
        {
            await mission.InsertAsync(entryItem);
            items.Add(entryItem);
        }

        private async Task RefreshMissionTable()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                items = await mission
                    .Where(missionTable => missionTable.completed == false)
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }
            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loding").ShowAsync();
            }
            else
            {
                ListItems.ItemsSource = items;
                this.SaveButton.IsEnabled = true;
            }


        }

        private async Task UpdateChenkedMissionTable(Mission entry)
        {
            await mission.UpdateAsync(entry);
            items.Remove(entry);
            ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);
        }
        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            
            //await Authenticate();
            await RefreshMissionTable();
        }
       
        //private MobileServiceUser user;
        //private async System.Threading.Tasks.Task Authenticate()
        //{
        //    while (user == null)
        //    {
        //        string message;
        //        try
        //        {
        //            user = await App.MobileService
        //                .LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
        //            message =
        //                string.Format("You are now logged in - {0}", user.UserId);
        //        }
        //        catch (InvalidOperationException)
        //        {
        //            message = "You must log in. Login Required";
        //        }
        //        var dialog = new MessageDialog(message, "Login Status");
        //        await dialog.ShowAsync();
        //    }
        //}



        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var missionItem = new Mission { information = TextInput.Text };
            await InsertMissionTable(missionItem);
        }

        private async void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Mission x = cb.DataContext as Mission;
            await UpdateChenkedMissionTable(x);
        }


    }
}
