using Favor.DataModel;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Favor
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AddressBook : Page
    {
        public AddressBook()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await FavorUser.instance.RefreshUserAllFriends();
            ListItems.ItemsSource = FavorUser.instance.AllFriendsCollection;
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
            Frame.Navigate(typeof(MainPage));

        }

        private void Back_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //回退按钮
            this.Frame.GoBack();

        }

        private async void AddressBookBtn_Click(object sender, RoutedEventArgs e)
        {
            //暂时转到加好友页面
            await FavorUser.instance.RefreshUserAllFriends();
        }


        private void Add_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //回退按钮
            Frame.Navigate(typeof(AddingFriends));

        }
    }
}
