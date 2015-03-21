using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using System.Text.RegularExpressions;
using Windows.Storage;
using Favor;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Windows.UI.Xaml.Controls;
using Favor.DataModel;
using Favor.Common;


namespace Favor.Controller
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


        public StorageFile userImageStorageFile;//存储用户头像文件


        /// <summary>
        /// 将任务插入MissionTable
        /// </summary>
        /// <param name="entryItem">需要插入的任务</param>
        /// <returns></returns>
        public async Task InsertMissionTable(Mission entryItem)
        {
            await MobileServiceTable.instance.missionItem.InsertAsync(entryItem);
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
                missionCollection = await MobileServiceTable.instance.missionItem
                    .Where(missionTable => missionTable.completed == false
                        & missionTable.userId == this.account.AuthenId
                        & missionTable.__createdAt > DateTime.Now.AddHours(Mission.ACTIVETIME))
                    .ToCollectionAsync();//导入自己发布的任务

                //导入朋友所发任务
                await RefreshUserAllFriends();
                if (AllFriendsCollection != null)
                {
                    foreach (Account friendAccount in AllFriendsCollection)
                    {
                        await ExtractUserMissions(friendAccount.AuthenId);
                    }
                }

                //导入自己和朋友转发的任务

                //读取所有和用户有关的转发记录
                List<Repost> repostList= await MobileServiceTable.instance.RepostItem
                                                .Where(repostTable => repostTable.ReposterId == this.account.AuthenId)
                                                .ToListAsync();
                if (AllFriendsCollection != null)
                {
                    foreach (Account friendAccount in AllFriendsCollection)
                    {
                        List<Repost> tempRepost =await MobileServiceTable.instance.RepostItem
                                                        .Where(repostTable => repostTable.ReposterId == friendAccount.AuthenId)
                                                        .ToListAsync();
                        foreach(Repost repost in tempRepost)
                        {
                            repostList.Add(repost);
                        }
                    }
                }

                //根据转发记录获得动态
                foreach (Repost repost in repostList)
                {
                    Mission mission = new Mission();
                    List<Mission> tempMission = await MobileServiceTable.instance.missionItem
                                                        .Where(missonTable => missonTable.id == repost.MissionId)
                                                        .ToListAsync();
                    mission = tempMission.First();
                    List<Account> tempAccount = await MobileServiceTable.instance.accountItem
                                                        .Where(accountTable => accountTable.AuthenId == repost.ReposterId)
                                                        .ToListAsync();
                    mission.Reposter = tempAccount.First().UserName;
                    missionCollection.Add(mission);
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
            else
            {
                //try
                //{
                //    await MobileServiceTable.instance.accountItem.UpdateAsync(account);
                //}
                //catch (MobileServiceInvalidOperationException e)
                //{
                //    exception = e;
                //}
                //if (exception != null)
                //{
                //    await new MessageDialog(exception.Message, "Error loding").ShowAsync();
                //}
            }
        }





        /// <summary>
        /// 选中之后更新MssionTable
        /// </summary>
        /// <param name="checkedMission">被选中的Mission</param>
        /// <returns></returns>
        public async Task UpdateChenkedMissionTable(Mission checkedMission)
        {
            checkedMission.receiverId = account.AuthenId;
            checkedMission.received = true;
            await MobileServiceTable.instance.missionItem.UpdateAsync(checkedMission);
        }

        /// <summary>
        /// 使用microsoft账号验证用户
        /// </summary>
        /// <returns></returns>
        public async Task Authenticate()
        {
            MobileServiceInvalidOperationException exceptionS = null;
            InvalidOperationException exception = null;
            while (mobileServiceUser == null)
            {
                string message;
                try
                {
                    try
                    {
                        mobileServiceUser = await App.MobileService
                            .LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
                    }

                    catch (MobileServiceInvalidOperationException e)
                    {
                        exceptionS = e;
                    }

                    if (exceptionS != null)
                    {
                        await App.statusBar.ProgressIndicator.HideAsync();
                        await new MessageDialog(exception.Message, "登陆状态").ShowAsync();
                    }
                    else
                    {
                        message = string.Format("You are now logged in");
                        var dialog = new MessageDialog(message, "Login Status");
                        await dialog.ShowAsync();

                    }
                }
                catch (InvalidOperationException e)
                {
                    exception = e;

                }
                if (exception != null)
                {
                    var dialog = new MessageDialog("You Must Login");
                    await dialog.ShowAsync();
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
                    await App.statusBar.ProgressIndicator.HideAsync();
                    message = "请输入正确的邮箱格式";
                    var dialog = new MessageDialog(message);
                    await dialog.ShowAsync();
                }
                else if (LoginAccount.Password.Length < 8)
                {
                    await App.statusBar.ProgressIndicator.HideAsync();
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
                        List<Account> accountList = await MobileServiceTable.instance.accountItem.Where(accountTable => accountTable.Email == LoginAccount.Email
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
                        await App.statusBar.ProgressIndicator.HideAsync();
                        await new MessageDialog(exception.Message, "登陆状态").ShowAsync();
                    }
                    else
                    {
                        //如果用户不存在
                        if (account == null)
                        {
                            await App.statusBar.ProgressIndicator.HideAsync();
                            await new MessageDialog("账号或用户名错误").ShowAsync();
                        }
                        else
                        {
                            //存储用户信息
                            AccountLocalStorage.instance.SaveAccount(account);
                            //message = "登陆成功!";
                            //var dialog = new MessageDialog(message);
                            //await dialog.ShowAsync();
                        }
                    }
                }
            }

            else
            {
                await new MessageDialog("您已登陆").ShowAsync();
            }


        }

        public async Task Login()
        {
            MobileServiceInvalidOperationException exception = null;

            try
            {
                List<Account> accountList = await MobileServiceTable.instance.accountItem.Where(accountTable => accountTable.AuthenId == mobileServiceUser.UserId).ToListAsync();

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
                await App.statusBar.ProgressIndicator.HideAsync();
                await new MessageDialog(exception.Message, "登陆状态").ShowAsync();
            }
            else
            {
                //如果用户不存在->插入该用户
                if (account.UserName != null)
                {
                    AccountLocalStorage.instance.SaveAccount(account);
                }
               
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

            await App.statusBar.ProgressIndicator.ShowAsync();

            if (!Regex.IsMatch(SigningUpAccount.Email, pattern))
            {
                await App.statusBar.ProgressIndicator.HideAsync();
                message = "请输入正确的邮箱格式";
                var dialog = new MessageDialog(message);
                await dialog.ShowAsync();
            }
            else if (SigningUpAccount.Password.Length < 8)
            {
                await App.statusBar.ProgressIndicator.HideAsync();
                message = "密码长度必须大于8位";
                var dialog = new MessageDialog(message);
                await dialog.ShowAsync();
            }
            else
            {
                MobileServiceInvalidOperationException exception = null;
                try
                {
                    await MobileServiceTable.instance.accountItem.InsertAsync(SigningUpAccount);
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
                    await App.statusBar.ProgressIndicator.HideAsync();
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
                searchFriendResultList = await MobileServiceTable.instance.accountItem
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
            mobileServiceUser = null;
            await new MessageDialog("注销成功").ShowAsync();

        }

        /// <summary>
        /// 通过查找用户账号增加朋友
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task AddingFriend(string name)
        {

            await App.statusBar.ProgressIndicator.ShowAsync();

            MobileServiceInvalidOperationException exception = null;
            List<Account> searchFriendResultList = new List<Account>();
            try
            {
                searchFriendResultList = await MobileServiceTable.instance.accountItem
                        .Where(accountTable => accountTable.UserName == name).ToListAsync();
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
                    string friendId = searchFriendResultList[0].AuthenId;//获取希望添加为好友的用户id
                    string accountDetail = searchFriendResultList[0].Password;//此处访问取回用户密码信息作为查询验证<之后需要修改>

                    List<UsersRelation> searchDuplicatedUserIdList = new List<UsersRelation>();//用户搜索好友关系表中是否已经存在该好友，避免重复添加

                    try
                    {
                        searchDuplicatedUserIdList = await (from userRelationPair in MobileServiceTable.instance.usersRelationItem
                                                            where (account.AuthenId == userRelationPair.UserId & userRelationPair.FriendId == friendId) 
                                                            select userRelationPair).ToListAsync();
                    }



                    catch (MobileServiceInvalidOperationException e)
                    {
                        exception = e;
                    }

                    if (exception != null)
                    {
                        await App.statusBar.ProgressIndicator.HideAsync();
                        await new MessageDialog(exception.Message, "登陆状态").ShowAsync();
                    }
                    else
                    {
                        if (searchDuplicatedUserIdList.Count != 0)//用户关系表中已有记录
                        {
                            await App.statusBar.ProgressIndicator.HideAsync();
                            var dialog = new MessageDialog("已是好友，无需添加");
                            await dialog.ShowAsync();
                        }
                        else
                        {
                            UsersRelation userRelation = new UsersRelation { UserId = account.AuthenId, FriendId = friendId };
                            UsersRelation userRelationX = new UsersRelation { UserId = friendId, FriendId = account.AuthenId };
                            try
                            {
                                await MobileServiceTable.instance.usersRelationItem.InsertAsync(userRelation);//若为新好友，则向用户关系表中插入数据
                                await MobileServiceTable.instance.usersRelationItem.InsertAsync(userRelationX);
                            }
                            catch (MobileServiceInvalidOperationException e)
                            {
                                exception = e;
                            }
                            if (exception != null)
                            {
                                await App.statusBar.ProgressIndicator.HideAsync();
                                await new MessageDialog(exception.Message, "登陆状态").ShowAsync();
                            }
                            else
                            {
                                await App.statusBar.ProgressIndicator.HideAsync();
                                var dialog = new MessageDialog("成功");//若插入成功，则返回密码作为验证
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
                    = await (from relation in MobileServiceTable.instance.usersRelationItem
                             where relation.UserId == this.account.AuthenId 
                             select relation).ToCollectionAsync();

                //根据所有记录查找用户的信息
                this.AllFriendsCollection = null;                        //清空好友列表
                
                foreach (UsersRelation FriendRelation in userAllFriendsRelationItem)
                {
                    if (this.AllFriendsCollection != null)
                    {
                        List<Account> friend = await (from account in MobileServiceTable.instance.accountItem
                                                      where account.AuthenId == FriendRelation.FriendId
                                                      where account.AuthenId != this.account.AuthenId
                                                      select account).ToListAsync();
                        AllFriendsCollection.Add(friend.First());
                    }
                    else
                    {
                        MobileServiceCollection<Account, Account> friend = await (from account in MobileServiceTable.instance.accountItem
                                                                                  where account.AuthenId == FriendRelation.FriendId
                                                                                  where account.AuthenId != this.account.AuthenId
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
            List<Mission> userMissionList = new List<Mission>();
            try
            {
                userMissionList = await MobileServiceTable.instance.missionItem
                    .Where(missionTable => missionTable.completed == false
                            & missionTable.userId == userId
                            & missionTable.__createdAt > DateTime.Now.AddHours(Mission.ACTIVETIME))
                    .ToListAsync();

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
                foreach (Mission mission in userMissionList)
                {
                    missionCollection.Add(mission);
                }
            }
        }

        /// <summary>
        /// 为Account表增加user在注册后填入的用户名
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task AddUserName(string userName)
        {

            account.UserName = userName;
            MobileServiceInvalidOperationException exception = null;
            try
            {
                await MobileServiceTable.instance.accountItem.UpdateAsync(account);
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
                await App.statusBar.ProgressIndicator.HideAsync();
            }
        }

        public async Task UploadUserImage()
        {
            UserImage userImage = new UserImage();//新建存储用户头像表类

            MobileServiceInvalidOperationException exception = null;

            string errorString = string.Empty;

            if (userImageStorageFile != null)
            {

                // Set blob properties
                userImage.ContainerName = "userimages";
                userImage.ResourceName = userImageStorageFile.Name;
                userImage.userId = account.AuthenId;
            }

            // Send the item to be inserted. When blob properties are set this
            // generates an SAS in the response.
            try
            {
                await MobileServiceTable.instance.userImageItem.InsertAsync(userImage);
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
                // If we have a returned SAS, then upload the blob.
                if (!string.IsNullOrEmpty(userImage.SasQueryString))
                {
                    // Get the URI generated that contains the SAS 
                    // and extract the storage credentials.
                    StorageCredentials cred = new StorageCredentials(userImage.SasQueryString);
                    var imageUri = new Uri(userImage.ImageUri);

                    // Instantiate a Blob store container based on the info in the returned item.
                    CloudBlobContainer container = new CloudBlobContainer(new Uri(string.Format("https://{0}/{1}", imageUri.Host, userImage.ContainerName)), cred);

                    //Get the new image as a stream.
                    using (var fileStream = await userImageStorageFile.OpenReadAsync())
                    {
                        // Upload the new image as a BLOB from the stream.
                        CloudBlockBlob blobFromSASCredential = container.GetBlockBlobReference(userImage.ResourceName);
                        await blobFromSASCredential.UploadFromStreamAsync(fileStream);

                    }

                    account.UserImageUri = userImage.ImageUri;

                    var dialog = new MessageDialog("upload success!");
                    await dialog.ShowAsync();

                }
                else
                {
                    var dialog = new MessageDialog("sasquerystring is NULL");
                    await dialog.ShowAsync();
                }
            }

        }

        /// <summary>
        /// 转发动态函数
        /// </summary>
        /// <param name="mission">用户需要转发的动态</param>
        /// <returns></returns>
        public async Task RepostMission(Mission mission)
        {
            Repost repost = new Repost();
            repost.MissionId = mission.id;
            repost.ReposterId = FavorUser.instance.account.AuthenId;
            await MobileServiceTable.instance.RepostItem.InsertAsync(repost);
            await new MessageDialog("Repost successful!").ShowAsync();
        }
    }
}



























