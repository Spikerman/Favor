using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favor.DataModel
{
    public class MobileServiceTable
    {
        private MobileServiceTable() { }
        public static readonly MobileServiceTable instance = new MobileServiceTable();

        /// <summary>
        /// 对应Mission表中的一条记录
        /// </summary>
        public IMobileServiceTable<Mission> missionItem = App.MobileService.GetTable<Mission>();

        /// <summary>
        /// 对应Account表中的 一条记录
        /// </summary>
        public IMobileServiceTable<Account> accountItem = App.MobileService.GetTable<Account>();

        /// <summary>
        /// 对应UsersRelation表中的一条记录
        /// </summary>
        public IMobileServiceTable<UsersRelation> usersRelationItem = App.MobileService.GetTable<UsersRelation>();

        public IMobileServiceTable<UserImage> userImageItem = App.MobileService.GetTable<UserImage>();

        public IMobileServiceTable<Mission> MissionOperator
        {
            get { return missionItem; }
            set { missionItem = value; }
        }

        public IMobileServiceTable<Repost> RepostItem = App.MobileService.GetTable<Repost>();
    }
}
