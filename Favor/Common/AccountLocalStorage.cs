﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

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
                    && ApplicationData.Current.LocalSettings.Values["Password"] != null)
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
        }

        public void LoadAccount(Account account)
        {
            account.Id = (string)ApplicationData.Current.LocalSettings.Values["Id"];
            account.Email = (string)ApplicationData.Current.LocalSettings.Values["Email"];
            account.Password = (string)ApplicationData.Current.LocalSettings.Values["Password"];
        }

        public void ClearStorage()
        {
            ApplicationData.Current.LocalSettings.Values.Remove("Id");
            ApplicationData.Current.LocalSettings.Values.Remove("Email");
            ApplicationData.Current.LocalSettings.Values.Remove("Password");
        }
    }
}