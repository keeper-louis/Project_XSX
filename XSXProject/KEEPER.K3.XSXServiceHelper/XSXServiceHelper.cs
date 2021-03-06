﻿using KEEPER.K3.XSX.Contracts;
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

        public static double GetKFQty(Context ctx, long stockOrgId, long masterId, long custID, long baseUnitId, long stockUnitId)
        {
            IOrderRequestService service = OrderRequestServiceFactory.GetService<IOrderRequestService>(ctx);
            double kfQty = service.GetKFQty(ctx, stockOrgId, masterId, custID, baseUnitId, stockUnitId);
            return kfQty;
        }
        /// <summary>
        /// 通过账户用途获取收付款用途
        /// </summary>
        /// <param name="AccountUsage">账户用途</param>
        /// <returns></returns>
        public static long GetSFKYT(Context ctx,string AccountUsage)
        {
            ICustomerService service = CustomerServiceFactory.GetService<ICustomerService>(ctx);
            long SFKYTID = service.GetSFKYT(ctx, AccountUsage);
            return SFKYTID;
        }

        /// <summary>
        /// 获取业务类型
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AccountUsage"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public static long GetYWTYPE(Context ctx, string AccountUsage, long custID)
        {
            ICustomerService service = CustomerServiceFactory.GetService<ICustomerService>(ctx);
            long YWTYPEID = service.GetYWTYPE(ctx, AccountUsage, custID);
            return YWTYPEID;
        }

        /// <summary>
        /// 判断是否是吉祥门店
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public static bool IsJXCust(Context ctx, long custID)
        {
            ICustomerService service = CustomerServiceFactory.GetService<ICustomerService>(ctx);
            bool reuslt = service.IsJXCust(ctx, custID);
            return reuslt;
        }
        /// <summary>
        /// 判断是否是无区域门店
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public static bool IsWQYCust(Context ctx, long custID)
        {
            ICustomerService service = CustomerServiceFactory.GetService<ICustomerService>(ctx);
            bool reuslt = service.IsWQYCust(ctx, custID);
            return reuslt;
        }
        /// <summary>
        /// 判断是否是有区域门店
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public static bool IsYQYCust(Context ctx, long custID)
        {
            ICustomerService service = CustomerServiceFactory.GetService<ICustomerService>(ctx);
            bool reuslt = service.IsYQYCust(ctx, custID);
            return reuslt;
        }

        /// <summary>
        /// 获取有区域客户属性
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
        /// 获取无区域客户属性
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public static Customer GetWQYCustomerProperty(Context ctx, long custID)
        {
            ICustomerService service = CustomerServiceFactory.GetService<ICustomerService>(ctx);
            Customer cust = service.GetWQYCustomerProperty(ctx, custID);
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
        /// 业务对象提交
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID">业务对象标识</param>
        /// <param name="ids">业务对象ID集合</param>
        /// <returns></returns>
        public static IOperationResult Submit(Context ctx, string formID, Object[] ids)
        {
            ICommonService service = XSXServiceFactory.GetService<ICommonService>(ctx);
            IOperationResult submitResult = service.SubmitBill(ctx, formID, ids);
            return submitResult;
        }

        /// <summary>
        /// 审核业务对象
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="formID">业务对象标识</param>
        /// <param name="ids">业务对象ID集合</param>
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
        /// <summary>
        /// 保存业务对象
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID">对象标识</param>
        /// <param name="dyObject">业务对象数据包</param>
        /// <returns></returns>
        public static IOperationResult Save(Context ctx,string FormID,DynamicObject dyObject)
        {
            ICommonService service = XSXServiceFactory.GetService<ICommonService>(ctx);
            IOperationResult saveResult = service.SaveBill(ctx,FormID,dyObject);
            return saveResult;
        }
        /// <summary>
        /// 构建业务对象数据包
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID">对象标识</param>
        /// <param name="fillBillPropertys">填充业务对象属性委托对象</param>
        /// <returns></returns>
        public static DynamicObject CreateBillMode(Context ctx, string FormID, Action<IDynamicFormViewService> fillBillPropertys)
        {
            ICommonService service = XSXServiceFactory.GetService<ICommonService>(ctx);
            DynamicObject model = service.installCostRequestPackage(ctx, FormID, fillBillPropertys, "");
            return model;
        }
        /// <summary>
        /// 获取内部客户属性
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="orgID">对应组织ID</param>
        /// <returns></returns>
        public static Customer GetIntCustomerProperty(Context ctx, long orgID)
        {
            ICustomerService service = CustomerServiceFactory.GetService<ICustomerService>(ctx);
            Customer cust = service.GetIntCustomerProperty(ctx, orgID);
            return cust;
        }
        /// <summary>
        /// 获取区域客户要货可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public static double GetQYAmount(Context ctx,string custNo)
        {
            IOrderRequestService service = OrderRequestServiceFactory.GetService<IOrderRequestService>(ctx);
            double QYAmount = service.GetQYAmount(ctx, custNo);
            return QYAmount;
        }

        /// <summary>
        /// 获取门店客户可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public static double GetMDAmount(Context ctx, string custNo,long custID)
        {
            IOrderRequestService service = OrderRequestServiceFactory.GetService<IOrderRequestService>(ctx);
            double MDAmount = service.GetMDAmount(ctx, custNo,custID);
            return MDAmount;
        }
        /// <summary>
        /// 获取客户门店可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custNo"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public static double GetYJAmount(Context ctx, string custNo, long custID)
        {
            IOrderRequestService service = OrderRequestServiceFactory.GetService<IOrderRequestService>(ctx);
            double YJAmount = service.GetYJAmount(ctx, custNo, custID);
            return YJAmount;
        }
    }
}
