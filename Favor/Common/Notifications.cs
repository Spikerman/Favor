using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Windows.Foundation;
using Windows.Networking.PushNotifications;
using Favor.DataModel;
using Favor.Controller;

namespace Favor.Common
{
    public class Notifications
    {

       
        private Notifications() { }
        public static readonly Notifications instance = new Notifications();
        //以用户的ID作为Tag标签
        public List<string> userIdTags = new List<string>();

        public PushNotificationChannel channel ;
        
        public async Task RefreshChannel()
        {
            channel=await Windows.Networking.PushNotifications.PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
        }

        public async Task PushToFriends()
        {
            try
            {
                string message = FavorUser.instance.account.UserName+" need you help";
               

                await App.MobileService.GetPush().RegisterNativeAsync(channel.Uri);
                await App.MobileService.InvokeApiAsync("notifyAllUsers", new JObject(new JProperty("toast", message)));
                userIdTags.Clear();

            }
            catch (Exception exception)
            {
                HandleRegisterException(exception);
            }
        }
        private static void HandleRegisterException(Exception exception)
        {

        }

    }
}
