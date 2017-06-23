using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class SaveAuthModel
    {
        [JsonProperty("authorizerAppID")]
        public string AuthorizerAppID { get; set; }
        [JsonProperty("authorizerAccessToken")]
        public string AuthorizerAccessToken { get; set; }
        [JsonProperty("expiresIn")]
        public int ExpiresIn { get; set; }
        [JsonProperty("authorizerRefreshToken")]
        public string AuthorizerRefreshToken { get; set; }
        [JsonProperty("authTime")]
        public DateTime AuthTime { get; set; }
    }
}
