using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using System.Text.RegularExpressions;

namespace Favor.Common
{
    public class FavorUser
    {
        //单实例模式，只允许有一个FavorUser对象，不允许使用构造函数。
        //请使用FavorUser.instance获取FavorUser对象。
        private FavorUser() { }
        public static readonly FavorUser instance = new FavorUser();

        public MobileServiceUser mobileServiceUser { get; set; }
        public MobileServiceCollection<Mission, Mission> items { get; set; }//两个参数

        private IMobileServiceTable<Mission> mission = App.MobileService.GetTable<Mission>();
        private IMobileServiceTable<Account> account = App.MobileService.GetTable<Account>();

        public IMobileServiceTable<Mission> Mission
        {
            get { return mission; }
            set { mission = value; }
        }

        public async Task InsertMissionTable(Mission entryItem)
        {
            await mission.InsertAsync(entryItem);
            items.Add(entryItem);
        }

        public async Task RefreshMissionTable()
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
            }


        }

        public async Task UpdateChenkedMissionTable(Mission entry)
        {
            await mission.UpdateAsync(entry);
            items.Remove(entry);
        }

        public async Task Authenticate()
        {
            while (mobileServiceUser == null)
            {
                string message;
                try
                {
                    mobileServiceUser = await App.MobileService
                        .LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
                    message =
                        string.Format("You are now logged in - {0}", mobileServiceUser.UserId);
                    var dialog = new MessageDialog(message, "Login Status");
                    await dialog.ShowAsync();
                }
                catch (InvalidOperationException)
                {
                    //message = "You must log in. Login Required";
                }

            }

        }

        public async Task SignUp(Account entry)
        {
            //EventHandler OnSuccess;
            string message;
            string pattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            if (!Regex.IsMatch(entry.Email, pattern))
            {
                message = "请输入正确的邮箱格式";
                var dialog = new MessageDialog(message);
                await dialog.ShowAsync();
            }
            else if (entry.Password.Length < 8)
            {
                message = "密码长度必须大于8位";
                var dialog = new MessageDialog(message);
                await dialog.ShowAsync();
            }
            else
            {
                MobileServiceInvalidOperationException exception = null;
                try
                {
                    await account.InsertAsync(entry);
                }
                catch (MobileServiceInvalidOperationException e)
                {
                    exception = e;
                }
                if (exception != null)
                {
                    await new MessageDialog(exception.Message, "登陆状态").ShowAsync();
                }
                else
                {
                    message = "注册成功!";
                    var dialog = new MessageDialog(message);
                    await dialog.ShowAsync();
                }
            }
        }
    }
}







