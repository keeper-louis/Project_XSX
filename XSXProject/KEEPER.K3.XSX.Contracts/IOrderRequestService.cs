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
    /// 要货控制契约
    /// </summary>
    [RpcServiceError]
    [ServiceContract]
    public interface IOrderRequestService
    {
        /// <summary>
        /// 获取区域客户要货申请可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Double GetQYAmount(Context ctx, string custNo);

        /// <summary>
        /// 获取门店客户要货申请可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Double GetMDAmount(Context ctx, string custNo, long custID);

        /// <summary>
        /// 获取营建门店客户要货申请可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Double GetYJAmount(Context ctx, string custNo, long custID);

        /// <summary>
        /// 获取库存可发量
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="stockOrgId">库存组织ID</param>
        /// <param name="masterId">物料MASTERID</param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Double GetKFQty(Context ctx, long stockOrgId, long masterId, long custID);
    }
}
