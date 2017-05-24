using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class AuthorizerAccessTokenGetResp
    {
        [JsonProperty("authorizer_access_token")]
        public string AuthorizerAccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("authorizer_refresh_token")]
        public string AuthorizerRefreshToken { get; set; }
    }
}
