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

        [JsonProperty(PropertyName="friendname")]
        public string FriendName { get; set; }


        [JsonProperty(PropertyName = "friendimageuri")]
        public string FriendImageUri { get; set; }

        [JsonProperty(PropertyName="isfocused")]
        public bool IsFocused { get; set; }

        [JsonProperty(PropertyName="isfocusingfriend")]
        public bool IsFocusingFriend { get; set; }


        
    }
}
