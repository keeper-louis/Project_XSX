using KEEPER.K3.XSX.Contracts;
using KEEPER.K3.XSX.Core.Entity;
using Kingdee.BOS;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEEPER.K3.XSXServiceHelper
{
    public class XSXServiceHelper
    {
        /// <summary>
        /// 获取客户属性
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public static Customer GetCustomerProperty(Context ctx,long custID)
        {
            ICustomerService service = CustomerServiceFactory.GetService<ICustomerService>(ctx);
            Customer cust = service.GetCustomerProperty(ctx, custID);
            return cust;
        }
        /// <summary>
        /// 封装基础资料对象
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID">基础资料标识</param>
        /// <param name="ObjectID">基础资料ID</param>
        /// <returns></returns>
        public static DynamicObject GetBasicObject(Context ctx, string FormID, long ObjectID)
        {
            ICommonService service = XSXServiceFactory.GetService<ICommonService>(ctx);
            DynamicObject basicObject = service.GetBasicObject(ctx, FormID, ObjectID);
            return basicObject;
        }

        /// <summary>
        /// 封装基础资料对象
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID">基础资料标识</param>
        /// <param name="ObjectID">基础资料ID</param>
        /// <returns></returns>
        public static IOperationResult Submit(Context ctx, string formID, Object[] ids)
        {
            ICommonService service = XSXServiceFactory.GetService<ICommonService>(ctx);
            IOperationResult submitResult = service.SubmitBill(ctx, formID, ids);
            return submitResult;
        }

        /// <summary>
        /// 封装基础资料对象
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID">基础资料标识</param>
        /// <param name="ObjectID">基础资料ID</param>
        /// <returns></returns>
        public static IOperationResult Audit(Context ctx, string formID, Object[] ids)
        {
            ICommonService service = XSXServiceFactory.GetService<ICommonService>(ctx);
            IOperationResult auditResult = service.AuditBill(ctx, formID, ids);
            return auditResult;
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="Operation">操作标识</param>
        /// <param name="returnResult">操作结果</param>
        /// <returns></returns>
        public static void Log(Context ctx, string Operation, IOperationResult returnResult)
        {
            ICommonService service = XSXServiceFactory.GetService<ICommonService>(ctx);
            service.Log(ctx, Operation, returnResult);
            
        }
    }
}
