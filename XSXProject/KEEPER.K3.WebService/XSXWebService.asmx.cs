using KEEPER.K3.Web.Core.Model;
using Kingdee.BOS.WebApi.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace KEEPER.K3.WebService
{
    /// <summary>
    /// XSXWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class XSXWebService : System.Web.Services.WebService
    {
        /// <summary>
        /// 字典(Session,ApiClient)
        /// </summary>
        /// <remarks>
        /// ApiClient需引用Kingdee.BOS.WebApi.Client.dll并using Kingdee.BOS.WebApi.Client;
        /// </remarks>
        private static ConcurrentDictionary<string, ApiClient> DctTokenApiClient = new ConcurrentDictionary<string, ApiClient>();

        /// <summary>
        /// K3Cloud站点地址
        /// </summary>
        string K3CloudURI = ConfigurationManager.AppSettings["K3CloudURI"].ToString();

        /// <summary>
        /// 数据中心ID
        /// </summary>
        string DBCenterId = ConfigurationManager.AppSettings["DBCenterId"].ToString();

        /// <summary>
        /// 用户
        /// </summary>
        string UserName = ConfigurationManager.AppSettings["UserName"].ToString();

        /// <summary>
        /// 密码
        /// </summary>
        string PassWord = ConfigurationManager.AppSettings["PassWord"].ToString();

        /// <summary>
        /// 语言环境ID：简体中文2052，英文1033，繁体中文
        /// </summary>
        int LocaleId = Convert.ToInt16(ConfigurationManager.AppSettings["LocaleId"]);

        [WebMethod(Description = "Test WebService")]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// 登录到K3Cloud系统
        /// </summary>
        /// <param name="loginInfo">登录信息</param>
        /// <returns></returns>
        [WebMethod(Description = "登录到K3Cloud系统")]
        public ReturnInfo<LoginResult> Login(string loginInfo)
        {
            LoginResult loginResult = new LoginResult();
            ReturnInfo<LoginResult> returnInfo = new ReturnInfo<LoginResult>(loginResult);
            ApiClient apiClient = new ApiClient(K3CloudURI);
            bool isSuccess = apiClient.Login(DBCenterId, EncryptionDecryption.Instance.BOSDeCode(UserName), EncryptionDecryption.Instance.BOSDeCode(PassWord), LocaleId); //此处借用BOS的WebApi登录接口登录到K3Cloud系统
            string accessToken = string.Empty;
            if (isSuccess)
            {
                returnInfo.IsSuccess = true;
                returnInfo.Message = "登录成功";

                //生成Token作为登录令牌
                accessToken = EncryptionDecryption.Instance.BOSEnCode(loginInfo);
                loginResult.AccessToken = accessToken;
                loginResult.UserRealName = loginInfo;
                ApiClient oldApiClient;
                if (DctTokenApiClient.TryGetValue(accessToken, out oldApiClient))
                {
                    DctTokenApiClient.TryRemove(accessToken, out oldApiClient);
                }

                DctTokenApiClient.AddOrUpdate(accessToken, apiClient, (k, v) =>
                {
                    return v;
                });
            }
            else
            {
                returnInfo.IsSuccess = false;
                returnInfo.Message = "登录失败，用户名或密码错误";
            }
            return returnInfo;
        }

        [WebMethod(Description = "某方法")]
        public string BindCustomerWeiXin(string parameterJson)
        {
            string accessToken = EncryptionDecryption.Instance.BOSEnCode(parameterJson);
            ApiClient apiClient;
            if (!DctTokenApiClient.TryGetValue(accessToken, out apiClient))
            {
                ReturnInfo<LoginResult> loginResult = Login(parameterJson);
                if (!loginResult.IsSuccess)
                {
                    return JsonConvert.SerializeObject(loginResult);
                }
            }
            DctTokenApiClient.TryGetValue(accessToken, out apiClient);
            object[] obj = new object[] { parameterJson };
            Kingdee.BOS.Log.Logger.Info("xx某某", string.Format("parameterJson:{0}", parameterJson));
            string result = apiClient.Execute<string>("自定义weiapi或者标准api", obj);
            return result;

        }
    }
}
