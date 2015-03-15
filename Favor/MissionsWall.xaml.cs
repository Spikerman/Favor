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
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace Favor
{

    public sealed partial class MissionsWall : Page
    {
        public MissionsWall()
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
                //无上一页弹窗提示关闭APP【与最小化后台运行并不同】 
                else
                {
                    this.Frame.GoBack();
                }
            };
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await FavorUser.instance.RefreshMissionsWall();
            await FavorUser.instance.RefreshUserAllFriends();
            MisssionListItems.ItemsSource = FavorUser.instance.missionCollection;
            FriendListItems.ItemsSource = FavorUser.instance.AllFriendsCollection;
        }

        /// <summary>
        /// 同步后台任务数据和前台任务列表
        /// </summary>
        /*旧的按钮
private async void RefreshListItems()
{
    await FavorUser.instance.RefreshMissionsWall();
    MisssionListItems.ItemsSource = FavorUser.instance.missionCollection;

}

private void WishBtn_Click(object sender, RoutedEventArgs e)
{
    this.Frame.Navigate(typeof(MissionWrite));
}

private void WallBtn_Click(object sender, RoutedEventArgs e)
{
    RefreshListItems();
}

private void AddressBookBtn_Click(object sender, RoutedEventArgs e)
{
    //暂时转到加好友页面
    //Frame.Navigate(typeof(AddressBook));
}
*/
        private async void Cancel_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //注销按钮
            await FavorUser.instance.LoginOut();
            Frame.Navigate(typeof(MainPage));

        }

        private void Back_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //回退按钮
            this.Frame.GoBack();

        }

        private async void Accept_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //接收任务
            //注意SelectionMode="Single"
            Mission x = (Mission)MisssionListItems.SelectedItem;
            await FavorUser.instance.UpdateChenkedMissionTable(x);
        }

        private void Write_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //发布任务
            Frame.Navigate(typeof(MissionWrite));
        }

        private void AddFriend_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //搜寻好友
            Frame.Navigate(typeof(AddingFriends));
        }

    }
}