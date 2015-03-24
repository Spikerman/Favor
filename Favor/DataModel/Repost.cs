using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favor.DataModel
{
    public class Repost
    {
        public string Id{ get; set; }

        [JsonProperty(PropertyName = "missionid")]
        public string MissionId { get; set; }

        [JsonProperty(PropertyName = "reposterid")]
        public string ReposterId { get; set; }
    }
}
