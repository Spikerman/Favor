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
using Favor.Controller;
using Favor.Common;

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
            this.NavigationCacheMode=NavigationCacheMode.Enabled;//设置页面缓存
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
            //这里有个bug 就是在写任务按取消时返回会出现
            this.SaveButton.IsEnabled = true;
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            await App.statusBar.ProgressIndicator.ShowAsync();
            if (TextInput.Text.Length <= 6)
            {
                var dialog = new MessageDialog("请输入足够的信息（大于6个字）");
                await dialog.ShowAsync();
                await App.statusBar.ProgressIndicator.HideAsync();
            }
            else
            {
                Frame.IsEnabled = false;
                var missionItem = new Mission { information = TextInput.Text, userId = FavorUser.instance.account.AuthenId, publisher = FavorUser.instance.account.UserName, publisherImageUri = FavorUser.instance.account.UserImageUri };
                await FavorUser.instance.InsertMissionTable(missionItem);
                await Notifications.instance.PushToFriends();
                Frame.IsEnabled = true;
                this.Frame.Navigate(typeof(MissionsWall));
                await App.statusBar.ProgressIndicator.HideAsync();
            }

        }



        private void PushingButton_Click(object sender, RoutedEventArgs e)
        {
           Frame.Navigate(typeof(PushingList));
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MissionsWall));
        }

        private void PushingFriendButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PushingList));
        }
    }
}
