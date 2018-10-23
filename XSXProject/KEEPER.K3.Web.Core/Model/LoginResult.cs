using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEEPER.K3.Web.Core.Model
{
    /// <summary>
    /// 登录结果存储
    /// </summary>
    public class LoginResult
    {
        public string AccessToken { get; set; }
        public string UserRealName { get; set; }
    }
}
