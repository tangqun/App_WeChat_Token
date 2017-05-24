using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class ComponentAccessTokenGetReq
    {
        [JsonProperty("component_appid")]
        public string ComponentAppId { get; set; }
        [JsonProperty("component_appsecret")]
        public string ComponentAppSecret { get; set; }
        [JsonProperty("component_verify_ticket")]
        public string ComponentVerifyTicket { get; set; }
    }
}
