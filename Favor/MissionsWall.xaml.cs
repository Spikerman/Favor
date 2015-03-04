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
using Favor.DataModel;
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace Favor
{

    public sealed partial class MissionsWall : Page
    {
        public MissionsWall()
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
            await FavorUser.instance.RefreshMissionsWall();
            ListItems.ItemsSource = FavorUser.instance.missionCollection;
        }

        /// <summary>
        /// 同步后台任务数据和前台任务列表
        /// </summary>
        private async void RefreshListItems()
        {
            await FavorUser.instance.RefreshMissionsWall();
            ListItems.ItemsSource = FavorUser.instance.missionCollection;
        }

        private void WishBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MissionWrite));
        }

        private void WallBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MissionsWall));
        }

        private async void Cancel_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //暂时作为注销按钮
            await FavorUser.instance.LoginOut();
            Frame.Navigate(typeof(Login));

        }

        private void Back_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //回退按钮
            this.Frame.GoBack();

        }

        private void AddingFriendsBtn_Click(object sender, RoutedEventArgs e)
        {
            //暂时转到加好友页面
            Frame.Navigate(typeof(AddingFriends));
        }

        private async void Accept_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            Mission x = item.DataContext as Mission;
            await FavorUser.instance.UpdateChenkedMissionTable(x);
            ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);
        }


    }
}