using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Favor.Common
{
    public class Mission
    {
        public string id { get; set; }

        [JsonProperty(PropertyName = "information")]
        public string information { get; set; }
        [JsonProperty(PropertyName = "completed")]
        public bool completed { get; set; }

        //public int receivednum;

    }
}
