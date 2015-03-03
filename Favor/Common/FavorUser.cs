using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using System.Text.RegularExpressions;
using Windows.Storage;

namespace Favor.Common
{
    public class FavorUser
    {
        //单实例模式，只允许有一个FavorUser对象，不允许使用构造函数。
        //请使用FavorUser.instance获取FavorUser对象。
        private FavorUser() {}
        public static readonly FavorUser instance = new FavorUser();

        public MobileServiceUser mobileServiceUser { get; set; }                          //For Authenticate()
        public MobileServiceCollection<Mission, Mission> missionCollection { get; set; }  //mission的集合
        public Account account { get; set; }                                              //用户账户信息

        private IMobileServiceTable<Mission> missionOperator = App.MobileService.GetTable<Mission>();   //用来操作Mission表
        private IMobileServiceTable<Account> accountOperator = App.MobileService.GetTable<Account>();   //用来操作Account表

        public IMobileServiceTable<Mission> MissionOperator
        {
            get { return missionOperator; }
            set { missionOperator = value; }
        }


        /// <summary>
        /// 将任务插入MissionTable
        /// </summary>
        /// <param name="entryItem">需要插入的任务</param>
        /// <returns></returns>
        public async Task InsertMissionTable(Mission entryItem)
        {
            await missionOperator.InsertAsync(entryItem);
        }

        /// <summary>
        /// 刷新missionCollection
        /// </summary>
        /// <returns></returns>
        public async Task RefreshMissionTable()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                missionCollection = await missionOperator
                    .Where(missionTable => missionTable.completed == false & missionTable.userId == this.account.Id)
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

        /// <summary>
        /// 选中之后更新MssionTable
        /// </summary>
        /// <param name="checkedMission">被选中的Mission</param>
        /// <returns></returns>
        public async Task UpdateChenkedMissionTable(Mission checkedMission)
        {
            await missionOperator.UpdateAsync(checkedMission);
        }

        /// <summary>
        /// 使用microsoft账号验证用户
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="LoginAccount">要登陆的用户信息</param>
        /// <returns></returns>
        public async Task Login(Account LoginAccount)
        {
            if (this.account == null)
            {
                string message;
                //验证账号格式
                string pattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

                if (!Regex.IsMatch(LoginAccount.Email, pattern))
                {
                    message = "请输入正确的邮箱格式";
                    var dialog = new MessageDialog(message);
                    await dialog.ShowAsync();
                }
                else if (LoginAccount.Password.Length < 8)
                {
                    message = "密码长度必须大于8位";
                    var dialog = new MessageDialog(message);
                    await dialog.ShowAsync();
                }
                else
                {
                    //执行登陆操作
                    MobileServiceInvalidOperationException exception = null;
                    try
                    {
                        List<Account> accountList = await accountOperator.Where(accountTable => accountTable.Email == LoginAccount.Email
                                                                                 & accountTable.Password == LoginAccount.Password)
                                                                         .ToListAsync();
                        if (accountList.Count != 0)
                        {
                            this.account = accountList.First();
                        }
                        else
                        {
                            this.account = null;
                        }
                    }
                    catch (MobileServiceInvalidOperationException e)
                    {
                        exception = e;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "通信错误").ShowAsync();
                    }
                    else
                    {
                        //如果用户不存在
                        if (account == null)
                        {
                            await new MessageDialog("账号或用户名错误").ShowAsync();
                        }
                        else
                        {
                            //存储用户信息
                            AccountLocalStorage.instance.SaveAccount(account);
                            message = "登陆成功!";
                            var dialog = new MessageDialog(message);
                            await dialog.ShowAsync();
                        }
                    }
                    
                }
            }
            else
            {
                await new MessageDialog("您已登陆").ShowAsync();
            }


        }

        /// <summary>
        /// 用户注册账户
        /// </summary>
        /// <param name="entry">需要注册的账户</param>
        /// <returns></returns>
        public async Task SignUp(Account SigningUpAccount)
        {
            //EventHandler OnSuccess;
            string message;
            string pattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            if (!Regex.IsMatch(SigningUpAccount.Email, pattern))
            {
                message = "请输入正确的邮箱格式";
                var dialog = new MessageDialog(message);
                await dialog.ShowAsync();
            }
            else if (SigningUpAccount.Password.Length < 8)
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
                    await accountOperator.InsertAsync(SigningUpAccount);
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

        
        /// <summary>
        /// 用户注销当前账户
        /// </summary>
        /// <returns></returns>
        public async Task LoginOut()
        {
            this.account = null;
            AccountLocalStorage.instance.ClearStorage();
            await new MessageDialog("注销成功").ShowAsync();

        }
    }
}







