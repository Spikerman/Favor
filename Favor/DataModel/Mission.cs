using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Favor.Controller;

namespace Favor.DataModel
{
    public class Mission
    {
        public string id { get; set; }
        
        [JsonProperty(PropertyName="userId")]
        public string userId { get; set; }

        [JsonProperty(PropertyName = "publisher")]
        public string publisher { get; set; }
        
        [JsonProperty(PropertyName = "information")]
        public string information { get; set; }
        
        [JsonProperty(PropertyName = "completed")]
        public bool completed { get; set; }

        [JsonProperty(PropertyName = "received")]
        public bool received { get; set; } //默认值为false

        [JsonProperty(PropertyName = "receiverId")]
        public string receiverId { get; set; }//默认值为 null

        [JsonProperty(PropertyName = "publisherimageuri")]
        public string publisherImageUri { get; set; }//任务发布者头像的URI

        [JsonProperty(PropertyName = "__createdAt")]
        public DateTime __createdAt { get; set; }  //任务的创建时间

        public static double ACTIVETIME = -144;    //任务的有效时间常量，必须为负数（表示从结束到开始的时间）

        private string restTime;                   //任务剩余时间
        public string RestTime
        {
            get
            {
                return restTime = (__createdAt.AddHours(-ACTIVETIME).Subtract(DateTime.Now).Days*24 
                                    + __createdAt.AddHours(-ACTIVETIME).Subtract(DateTime.Now).Hours) + "'"   //计算小时
                                    + __createdAt.AddHours(-ACTIVETIME).Subtract(DateTime.Now).Minutes.ToString() + "''";
            }
            set
            {
                restTime = value;
            }
        }

        private string reposter;
        public string Reposter
        {
            get
            {
                if (reposter == "" || reposter == null)
                    return reposter;
                else if (!reposter.Contains("'s friend"))
                    return reposter + "'s friend";
                else
                    return reposter;
                    
            }
            set
            {
                reposter = value;
            }
        }



        public string FrontColor
        {
            get
            {
                if (received == true || userId == FavorUser.instance.account.AuthenId)
                    return "#FFA9AEB6";
                else
                    return "#FFFF8F3D";
            }
        }

    }
}
