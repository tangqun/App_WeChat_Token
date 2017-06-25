using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class AuthorizationInfoModel
    {
        [JsonIgnore]
        public int ID { get; set; }
        [JsonIgnore]
        public string AuthorizerAppID { get; set; }
        public string AuthorizerAccessTokenOld { get; set; }
        public string AuthorizerAccessToken { get; set; }
        [JsonIgnore]
        public int ExpiresIn { get; set; }
        [JsonIgnore]
        public string AuthorizerRefreshToken { get; set; }
        public string JSAPITicket { get; set; }
        public string APITicket { get; set; }
        [JsonIgnore]
        public DateTime AuthTime { get; set; }
        [JsonIgnore]
        public DateTime RefreshTime { get; set; }
        [JsonIgnore]
        public DateTime CreateTime { get; set; }
        [JsonIgnore]
        public DateTime UpdateTime { get; set; }
    }
}
