using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class Authorizer_Access_Token_Req
    {
        [JsonProperty("component_appid")]
        public string Component_AppId { get; set; }
        [JsonProperty("authorizer_appid")]
        public string Authorizer_AppId { get; set; }
        [JsonProperty("authorizer_refresh_token")]
        public string Authorizer_Refresh_Token { get; set; }
    }
}
