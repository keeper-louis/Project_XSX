using KEEPER.K3.XSX.Core.Entity;
using KEEPER.K3.XSX.Core.ParamOption;
using Kingdee.BOS;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Orm.DataEntity;
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
    /// 服务契约
    /// </summary>
    [RpcServiceError]
    [ServiceContract]
    public interface ICommonService
    {

        /// <summary>
        /// 获取基础资料对象契约
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="formID">基础资料标识</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        DynamicObject GetBasicObject(Context ctx, string formID,long ObjectID);


        /// <summary>
        /// 创建单据
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        IOperationResult SaveBill(Context ctx, string FormID,DynamicObject dyObject);


        /// <summary>
        /// 提交单据
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        IOperationResult SubmitBill(Context ctx, string FormID, object[] ids);

        /// <summary>
        /// 审核单据
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        IOperationResult AuditBill(Context ctx, string FormID, object[] ids);

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="businessInfo"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void Log(Context ctx, string Operation,IOperationResult returnResult);

        /// <summary>
        /// 组装费用申请单数据包
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        DynamicObject installCostRequestPackage(Context ctx, string FormID,Action<IDynamicFormViewService> fillBillPropertys,string BillTypeId);


        

        /// <summary>
        /// 获取Cloud系统时间
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //DateTime GetSysDate(Context ctx);

        /// <summary>
        /// 后台调用单据转换生成目标单
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //IEnumerable<DynamicObject> ConvertBills(Context ctx, ConvertOption option);

        /// <summary>
        /// 暂存单据
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="businessInfo"></param>
        /// <param name="billData"></param>
        /// <returns></returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //IOperationResult DraftBill(Context ctx, BusinessInfo businessInfo, IEnumerable<DynamicObject> billData);

        /// <summary>
        /// 保存单据
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="businessInfo"></param>
        /// <param name="billData"></param>
        /// <returns></returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //IOperationResult SaveBill(Context ctx, BusinessInfo businessInfo, IEnumerable<DynamicObject> billData);



        /// <summary>
        /// 审核单据
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="businessInfo"></param>
        /// <param name="ids"></param>
        /// <param name="formOperation"></param>
        /// <returns></returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //IOperationResult SetBillStatus(Context ctx, BusinessInfo businessInfo, object[] ids, FormOperationEnum formOperation = FormOperationEnum.Audit);

        /// <summary>
        ///  创建无源单的单据的数据包（填充默认值、触发实体服务规则）
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="formId">单据元数据标识</param>
        /// <param name="fillBillPropertys"> 1.调用IDynamicFormViewService.UpdateValue: 会执行字段的值更新事件；2.调用 dynamicFormView.SetItemValueByNumber ：不会执行值更新事件，需要继续调用： ((IDynamicFormView)dynamicFormView).InvokeFieldUpdateService(key, rowIndex);</param>
        /// <param name="billTypeId">单据类型</param>
        /// <returns>无源单的单据的数据包（未保存单据）</returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //DynamicObject CreateNoSourceBill(Context ctx, string formId, Action<IDynamicFormViewService> fillBillPropertys, string billTypeId = "");

    }
}
