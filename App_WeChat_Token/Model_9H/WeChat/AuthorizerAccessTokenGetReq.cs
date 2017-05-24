using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class AuthorizerAccessTokenGetReq
    {
        [JsonProperty("component_appid")]
        public string ComponentAppID { get; set; }
        [JsonProperty("authorizer_appid")]
        public string AuthorizerAppID { get; set; }
        [JsonProperty("authorizer_refresh_token")]
        public string AuthorizerRefreshToken { get; set; }
    }
}
