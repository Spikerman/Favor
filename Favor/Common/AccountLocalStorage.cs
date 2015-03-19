using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Favor.DataModel;

namespace Favor.Common
{
    public class AccountLocalStorage
    {
        private AccountLocalStorage() { }
        public static readonly AccountLocalStorage instance = new AccountLocalStorage();

        /// <summary>
        /// 判断本地存储是否有效
        /// </summary>
        /// <returns>
        ///     true:本地存储有效
        ///     false:本地存储无效
        /// </returns>
        public bool isvaild()
        {
            if (ApplicationData.Current.LocalSettings.Values["Id"] != null
                     && ApplicationData.Current.LocalSettings.Values["Email"] != null
                     && ApplicationData.Current.LocalSettings.Values["Password"] != null
                && ApplicationData.Current.LocalSettings.Values["UserImageUri"] != null
                && ApplicationData.Current.LocalSettings.Values["UserName"] != null
                && ApplicationData.Current.LocalSettings.Values["ChannelUri"] != null)

            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SaveAccount(Account account)
        {
            ApplicationData.Current.LocalSettings.Values["Id"] = account.Id;
            ApplicationData.Current.LocalSettings.Values["Email"] = account.Email;
            ApplicationData.Current.LocalSettings.Values["Password"] = account.Password;
            ApplicationData.Current.LocalSettings.Values["UserImageUri"] = account.UserImageUri;
            ApplicationData.Current.LocalSettings.Values["UserName"] = account.UserName;
            ApplicationData.Current.LocalSettings.Values["ChannelUri"] = account.ChannelUri;
        }

        public void LoadAccount(Account account)
        {
            account.Id = (string)ApplicationData.Current.LocalSettings.Values["Id"];
            account.Email = (string)ApplicationData.Current.LocalSettings.Values["Email"];
            account.Password = (string)ApplicationData.Current.LocalSettings.Values["Password"];
            account.UserImageUri = (string)ApplicationData.Current.LocalSettings.Values["UserImageUri"];
            account.UserName = (string)ApplicationData.Current.LocalSettings.Values["UserName"];
            account.ChannelUri = (string)ApplicationData.Current.LocalSettings.Values["ChannelUri"];
        }

        public void ClearStorage()
        {
            ApplicationData.Current.LocalSettings.Values.Remove("Id");
            ApplicationData.Current.LocalSettings.Values.Remove("Email");
            ApplicationData.Current.LocalSettings.Values.Remove("Password");
            ApplicationData.Current.LocalSettings.Values.Remove("UserImageUri");
            ApplicationData.Current.LocalSettings.Values.Remove("UserName");
            ApplicationData.Current.LocalSettings.Values.Remove("ChannelUri");
        }
    }
}
