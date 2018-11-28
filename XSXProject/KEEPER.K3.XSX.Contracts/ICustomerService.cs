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
        /// <param name="custID">对应组织ID</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool IsJXCust(Context ctx, long orgID);


    }
}
