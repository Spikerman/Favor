﻿using Microsoft.WindowsAzure.MobileServices;
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
using Favor.View;
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

            //控制pivot高亮

        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            
            if (MobileServiceTable.instance.usersRelationItem != null)
            {
                FavorUser.instance.AllUserFriendCollection = await (from userRelationPair in MobileServiceTable.instance.usersRelationItem
                                                                    where (FavorUser.instance.account.AuthenId == userRelationPair.UserId)
                                                                    select userRelationPair).ToCollectionAsync();
              
             

            }
            await FavorUser.instance.RefreshMissionsWall();
            await FavorUser.instance.RefreshUserAllFriends();
            MisssionListItems.ItemsSource = FavorUser.instance.missionCollection;
            FriendListItems.ItemsSource = FavorUser.instance.AllUserFriendCollection;

        }

        /// <summary>
        /// 同步后台任务数据和前台任务列表
        /// </summary>


        private async void Cancel_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //注销按钮
            await FavorUser.instance.LoginOut();
            Frame.Navigate(typeof(Starting));

        }

        private void Back_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //回退按钮
            this.Frame.GoBack();

        }

        /*private async void Accept_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //接收任务
            //注意SelectionMode="Single"
            Mission x = (Mission)MisssionListItems.SelectedItem;
            await FavorUser.instance.UpdateChenkedMissionTable(x);
        }*/

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

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             
            /*PivotItem currentItem = e.AddedItems[0] as PivotItem;
            (currentItem.Header as Image).Opacity = 1.0;*/
            //pivotitem.header的循环和高亮无法解决
            //已经解决APPbar的button轮流显示
            if (pivot.SelectedIndex == 0)
            {
                //个人中心：注销
                comBar.PrimaryCommands.Remove(AddFriend);
                comBar.PrimaryCommands.Remove(WriteMission);
                comBar.PrimaryCommands.Remove(LogOut);

                comBar.PrimaryCommands.Add(LogOut);
            }
            else if (pivot.SelectedIndex == 1)
            {
                //墙：写任务 
                comBar.PrimaryCommands.Remove(AddFriend);
                comBar.PrimaryCommands.Remove(WriteMission);
                comBar.PrimaryCommands.Remove(LogOut);

                comBar.PrimaryCommands.Add(WriteMission);
                
            }
            else if (pivot.SelectedIndex == 2)
            {
                //好友列表：加好友
                comBar.PrimaryCommands.Remove(AddFriend);
                comBar.PrimaryCommands.Remove(WriteMission);
                comBar.PrimaryCommands.Remove(LogOut);

                comBar.PrimaryCommands.Add(AddFriend);
            }
            
        }

        private async void Repost_Button_Click(object sender, RoutedEventArgs e)
        {
            Mission x = new Mission();
            await FavorUser.instance.RepostMission(x);
        }

        private async void ToggledHappen(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = (ToggleSwitch)sender;
            UsersRelation x=(UsersRelation)toggle.DataContext;
            if (toggle.IsOn)
            {
                x.IsFocusingFriend = true;
            }
            else
            {
                x.IsFocusingFriend = false;
            }

            MobileServiceInvalidOperationException exception = null;
            try
            {
                await MobileServiceTable.instance.usersRelationItem.UpdateAsync(x);

                List<UsersRelation> friend = await (from userRelationPair in MobileServiceTable.instance.usersRelationItem
                                                    where (x.FriendId == userRelationPair.UserId)
                                                    select userRelationPair).ToListAsync();

                if (toggle.IsOn)
                {
                    friend.First().IsFocused = true;
                }
                else
        {
                    friend.First().IsFocused = false;
                }

                await MobileServiceTable.instance.usersRelationItem.UpdateAsync(
                   friend.First());
            }
            catch (MobileServiceInvalidOperationException ee)
            {
                exception = ee;
            }
            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loding, please check the Internet ").ShowAsync();
            }
        }

    }
}
