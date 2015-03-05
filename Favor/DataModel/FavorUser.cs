using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using System.Text.RegularExpressions;
using Windows.Storage;
using Favor.Common;

namespace Favor.DataModel
{
    public class FavorUser
    {
        //单实例模式，只允许有一个FavorUser对象，不允许使用构造函数。
        //请使用FavorUser.instance获取FavorUser对象。
        private FavorUser() { }
        public static readonly FavorUser instance = new FavorUser();

        public MobileServiceUser mobileServiceUser { get; set; }                          //For Authenticate()
        public MobileServiceCollection<Mission, Mission> missionCollection { get; set; }  //mission的集合

        public MobileServiceCollection<Account, Account> AllFriendsCollection { get; set; }         //用户的所有好友

        public Account account { get; set; }                                              //用户账户信息

        //public UsersRelation usersRelation { get; set; }

        /// <summary>
        /// 对应Mission表中的一条记录
        /// </summary>
        private IMobileServiceTable<Mission> missionItem = App.MobileService.GetTable<Mission>();

        /// <summary>
        /// 对应Account表中的 一条记录
        /// </summary>
        private IMobileServiceTable<Account> accountItem = App.MobileService.GetTable<Account>();

        /// <summary>
        /// 对应UsersRelation表中的一条记录
        /// </summary>
        private IMobileServiceTable<UsersRelation> usersRelationItem = App.MobileService.GetTable<UsersRelation>();
        public IMobileServiceTable<Mission> MissionOperator
        {
            get { return missionItem; }
            set { missionItem = value; }
        }

        /// <summary>
        /// 将任务插入MissionTable
        /// </summary>
        /// <param name="entryItem">需要插入的任务</param>
        /// <returns></returns>
        public async Task InsertMissionTable(Mission entryItem)
        {
            await missionItem.InsertAsync(entryItem);
        }

        /// <summary>
        /// 刷新missionCollection,显示中的心愿墙
        /// 原函数为RefreshMissionTable
        /// </summary>
        /// <returns></returns>
        public async Task RefreshMissionsWall()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                missionCollection = await missionItem
                    .Where(missionTable => missionTable.completed == false & missionTable.userId == this.account.Id)
                    .ToCollectionAsync();//导入自己发布的任务

                //导入朋友所发任务
                await RefreshUserAllFriends();
                foreach(Account friendAccount in AllFriendsCollection)
                {
                   await ExtractUserMissions(friendAccount.Id);
                }
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }
            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loding").ShowAsync();
            }
        }

        /// <summary>
        /// 选中之后更新MssionTable
        /// </summary>
        /// <param name="checkedMission">被选中的Mission</param>
        /// <returns></returns>
        public async Task UpdateChenkedMissionTable(Mission checkedMission)
        {
            checkedMission.receiverId = account.Id;
            await missionItem.UpdateAsync(checkedMission);
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
                        List<Account> accountList = await accountItem.Where(accountTable => accountTable.Email == LoginAccount.Email
                                                                                 & accountTable.Password == LoginAccount.Password)
                                                                         .ToListAsync();
                        if (accountList.Count != 0)
                        {
                            this.account = accountList.First();
                        }
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
                    await accountItem.InsertAsync(SigningUpAccount);
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
        /// 根据用户账号查找密码（暂时未用）
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task SearchFriend(string email)
        {
            MobileServiceInvalidOperationException exception = null;
            List<Account> searchFriendResultList = new List<Account>();
            try
            {
                searchFriendResultList = await accountItem
                        .Where(accountTable => accountTable.Email == email).ToListAsync();
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
                if (searchFriendResultList.Count == 0)
                {
                    var dialog = new MessageDialog("无此账号信息，请检查所输入账号是否正确");
                    await dialog.ShowAsync();
                }
                else
                {
                    string accountDetail = searchFriendResultList[0].Password;//此处访问取回用户密码信息作为查询验证

                    await new MessageDialog(accountDetail, "账号信息").ShowAsync();
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

        /// <summary>
        /// 通过查找用户账号增加朋友
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task AddingFriend(string email)
        {
            MobileServiceInvalidOperationException exception = null;
            List<Account> searchFriendResultList = new List<Account>();
            try
            {
                searchFriendResultList = await accountItem
                        .Where(accountTable => accountTable.Email == email).ToListAsync();
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
                if (searchFriendResultList.Count == 0)//Account表中并没有该用户记录
                {
                    var dialog = new MessageDialog("无此账号信息，请检查所输入账号是否正确");
                    await dialog.ShowAsync();
                }
                else
                {
                    string friendId = searchFriendResultList[0].Id;//获取希望添加为好友的用户id
                    string accountDetail = searchFriendResultList[0].Password;//此处访问取回用户密码信息作为查询验证<之后需要修改>

                    List<UsersRelation> searchDuplicatedUserIdList = new List<UsersRelation>();//用户搜索好友关系表中是否已经存在该好友，避免重复添加

                    try
                    {
                        searchDuplicatedUserIdList = await usersRelationItem
                            .Where(usersRelationTable => usersRelationTable.FriendId == friendId).ToListAsync();
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
                        if (searchDuplicatedUserIdList.Count != 0)//用户关系表中已有记录
                        {
                            var dialog = new MessageDialog("已是好友，无需添加");
                            await dialog.ShowAsync();
                        }
                        else
                        {
                            UsersRelation userRelation = new UsersRelation { UserId = account.Id, FriendId = friendId };
                            try
                            {
                                await usersRelationItem.InsertAsync(userRelation);//若为新好友，则向用户关系表中插入数据
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
                                var dialog = new MessageDialog("成功！密码: " + accountDetail);//若插入成功，则返回密码作为验证
                                await dialog.ShowAsync();
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 根据用户ID参数查找并收集该用户存储在userRelation表中的所有相关好友关系对
        /// </summary>
        /// <param name="userId">
        /// 传入参数为希望查找好友的自身用户ID
        /// </param>
        /// <returns></returns>
        public async Task RefreshUserAllFriends()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                //查询所有和该用户有关的关系记录
                MobileServiceCollection<UsersRelation, UsersRelation> userAllFriendsRelationItem
                    = await (from relation in usersRelationItem
                             where relation.UserId == this.account.Id || relation.FriendId == this.account.Id
                             select relation).ToCollectionAsync();

                //根据所有记录查找用户的信息
                this.AllFriendsCollection = null;                        //清空好友列表
                foreach (UsersRelation FriendRelation in userAllFriendsRelationItem)
                {
                    if (this.AllFriendsCollection != null)
                    {
                        List<Account> friend = await (from account in accountItem
                                                      where account.Id == FriendRelation.UserId || account.Id == FriendRelation.FriendId
                                                      where account.Id != this.account.Id
                                                      select account).ToListAsync();
                        AllFriendsCollection.Add(friend.First());
                    }
                    else
                    {
                        MobileServiceCollection<Account,Account> friend = await (from account in accountItem
                                                                                 where account.Id == FriendRelation.UserId || account.Id == FriendRelation.FriendId
                                                                                 where account.Id != this.account.Id
                                                                                 select account).ToCollectionAsync();
                        AllFriendsCollection = friend;
                    }
                    
                }
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error,try again").ShowAsync();
            }
            else
            {

            }
        }
        /// <summary>
        /// 根据输入ID查找对应用户的已发布但未完成的所有任务
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task ExtractUserMissions(string userId)
        {
            MobileServiceInvalidOperationException exception = null;
            List<Mission> userMissionList=new List<Mission>();
            try
            {
                userMissionList=await missionItem
                    .Where(missionTable => missionTable.completed == false & missionTable.userId == userId).ToListAsync();
                    
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error,try again").ShowAsync();
            }
            else
            {
                foreach(Mission mission in userMissionList)
                {
                    missionCollection.Add(mission);
                }
            }
        }
    }

}



















