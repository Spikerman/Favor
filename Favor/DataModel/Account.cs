﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Favor.DataModel
{
    public class Account
    {
        [JsonProperty(PropertyName="id")]
        public string Id { set; get; }
        
        [JsonProperty(PropertyName="email")]
        public string Email { get; set; }
        
        [JsonProperty(PropertyName="password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "userimageuri")]
        public string UserImageUri { get; set; }

        [JsonProperty(PropertyName ="authenid")]
        public string AuthenId { get; set; }

        [JsonProperty(PropertyName="phone")]
        public string Phone { get; set; }
    }
}
