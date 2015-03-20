using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Favor.DataModel;

namespace Favor.Controller
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
            if (ApplicationData.Current.LocalSettings.Values["Phone"] != null&&
                ApplicationData.Current.LocalSettings.Values["UserImageUri"] != null
                && ApplicationData.Current.LocalSettings.Values["UserName"] != null
                )
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
            ApplicationData.Current.LocalSettings.Values["Phone"] = account.Phone;
            ApplicationData.Current.LocalSettings.Values["UserImageUri"] = account.UserImageUri;
            ApplicationData.Current.LocalSettings.Values["UserName"] = account.UserName;
            
            
        }

        public void LoadAccount(Account account)
        {
            account.Phone = (string)ApplicationData.Current.LocalSettings.Values["Phone"];
            
            account.UserImageUri = (string)ApplicationData.Current.LocalSettings.Values["UserImageUri"];
            account.UserName = (string)ApplicationData.Current.LocalSettings.Values["UserName"];
            
        }

        public void ClearStorage()
        {
            
            ApplicationData.Current.LocalSettings.Values.Remove("Phone");
            ApplicationData.Current.LocalSettings.Values.Remove("UserImageUri");
            ApplicationData.Current.LocalSettings.Values.Remove("UserName");
           
            
        }
    }
}
