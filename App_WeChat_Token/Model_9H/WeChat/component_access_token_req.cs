using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class component_access_token_req
    {
        [JsonProperty("component_appid")]
        public string Component_AppId { get; set; }
        [JsonProperty("component_appsecret")]
        public string Component_AppSecret { get; set; }
        [JsonProperty("component_verify_ticket")]
        public string Component_Verify_Ticket { get; set; }
    }
}
