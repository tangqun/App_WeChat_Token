using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class Authorization_Info_Req
    {
        [JsonProperty("component_appid")]
        public string Component_AppId { get; set; }
        [JsonProperty("authorization_code")]
        public string Authorization_Code { get; set; }
    }
}
