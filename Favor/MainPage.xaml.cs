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
using Favor.Common;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace Favor
{

    public sealed partial class MainPage : Page
    {
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

      
        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await FavorUser.instance.RefreshMissionTable();
            ListItems.ItemsSource = FavorUser.instance.missionCollection;
            this.SaveButton.IsEnabled = true;
        }

        /// <summary>
        /// 同步后台任务数据和前台任务列表
        /// </summary>
        private async void RefreshListItems()
        {
            await FavorUser.instance.RefreshMissionTable();
            ListItems.ItemsSource = FavorUser.instance.missionCollection;
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Frame.IsEnabled = false;
            var missionItem = new Mission { information = TextInput.Text, userId = FavorUser.instance.account.Id };
            await FavorUser.instance.InsertMissionTable(missionItem);
            RefreshListItems();
            Frame.IsEnabled = true;
        }

        private async void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Mission x = cb.DataContext as Mission;
            await FavorUser.instance.UpdateChenkedMissionTable(x);
            RefreshListItems();
            ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);
        }

        private async void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            await FavorUser.instance.LoginOut();
            Frame.Navigate(typeof(Login));
        }



    }
}