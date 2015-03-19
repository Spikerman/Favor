using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Favor.DataModel
{
    /// <summary>
    /// 用户关系，表示用户与其好友对，在UsersRelation表中的一行
    /// </summary>
    public class UsersRelation
    {
        public string Id;

        [JsonProperty(PropertyName = "userId")]
        public string UserId{ get; set; }

        [JsonProperty(PropertyName = "friendId")]
        public string FriendId{ get; set; }
    }
}
