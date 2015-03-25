using Favor.Controller;
using Favor.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Favor.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Starting : Page
    {
        public Starting()
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
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void Starting_Button_Click(object sender, RoutedEventArgs e)
        {

            await FavorUser.instance.Authenticate();

            FavorUser.instance.account = new Account();

            FavorUser.instance.account.AuthenId = FavorUser.instance.mobileServiceUser.UserId;


            await FavorUser.instance.Login();

            if (FavorUser.instance.account.UserName == null)
            {
                Frame.Navigate(typeof(AfterLogin));
            }
            else
            {
                Frame.Navigate(typeof(MissionsWall));
            }


        }
    }
}
