using KEEPER.K3.XSX.Core.Entity;
using Kingdee.BOS;
using Kingdee.BOS.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KEEPER.K3.XSX.Contracts
{
    /// <summary>
    /// 客户服务契约
    /// </summary>
    [RpcServiceError]
    [ServiceContract]
    public interface ICustomerService
    {
        /// <summary>
        /// 通过客户内码获取客户属性契约
        /// </summary>
        /// <param name="ctx">上下文</param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Customer GetCustomerProperty(Context ctx, long custID);

        /// <summary>
        /// 通过客户内码获取无区域客户属性契约
        /// </summary>
        /// <param name="ctx">上下文</param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Customer GetWQYCustomerProperty(Context ctx, long custID);

        /// <summary>
        /// 通过对应组织内码获取客户属性契约
        /// </summary>
        /// <param name="ctx">上下文</param>
        /// <param name="custID">对应组织ID</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Customer GetIntCustomerProperty(Context ctx, long orgID);


        /// <summary>
        /// 判断是否是吉祥客户
        /// </summary>
        /// <param name="ctx">上下文</param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool IsJXCust(Context ctx, long custID);

        /// <summary>
        /// 判断是否是有区域门店
        /// </summary>
        /// <param name="ctx">上下文</param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool IsYQYCust(Context ctx, long custID);

        /// <summary>
        /// 判断是否是无区域门店
        /// </summary>
        /// <param name="ctx">上下文</param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool IsWQYCust(Context ctx, long custID);

       /// <summary>
       /// 通过账号用途获取收付款用途
       /// </summary>
       /// <param name="AccountUsage">账号用途编码</param>
       /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        long GetSFKYT(Context ctx,string AccountUsage);


        /// <summary>
        /// 通过账号用途获取业务类型
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AccountUsage">账号用途</param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        long GetYWTYPE(Context ctx, string AccountUsage,long custID);


    }
}
