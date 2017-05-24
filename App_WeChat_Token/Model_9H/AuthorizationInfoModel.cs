using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class AuthorizationInfoModel
    {
        public int ID { get; set; }
        public string AuthorizerAppID { get; set; }
        public string AuthorizerAccessTokenOld { get; set; }
        public string AuthorizerAccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string AuthorizerRefreshToken { get; set; }
        public DateTime AuthTime { get; set; }
        public DateTime RefreshTime { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
