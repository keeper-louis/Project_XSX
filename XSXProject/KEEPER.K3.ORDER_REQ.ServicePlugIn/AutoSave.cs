using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using KEEPER.K3.XSXServiceHelper;
using KEEPER.K3.XSX.Core.Entity;
using Kingdee.BOS.App.Data;
using Kingdee.K3.SCM.Contracts;
using Kingdee.BOS.Core.DynamicForm;

namespace KEEPER.K3.ORDER_REQ.ServicePlugIn
{
    [Description("要货申请单保存,计算可订货余额的逻辑需要优化,随着数据量增加会越来越慢")]
    public class AutoSave:AbstractOperationServicePlugIn
    {
        /// <summary>
        /// 保存校验
        /// </summary>
        /// <param name="e"></param>
        public override void OnAddValidators(AddValidatorsEventArgs e)
        {
            AutoAuditValidator auditValidator = new AutoAuditValidator();
            auditValidator.EntityKey = "FBillHead";
            e.Validators.Add(auditValidator);
        }

        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            base.OnPreparePropertys(e);
            e.FieldKeys.Add("FCUSTTYPE");
            e.FieldKeys.Add("FAPPLYCUST");
            e.FieldKeys.Add("FORGTYPE");
            e.FieldKeys.Add("FTOTALAMOUNT");
            e.FieldKeys.Add("FYWTYPE");
            e.FieldKeys.Add("FDispatchOrgIdDetail");
            e.FieldKeys.Add("FMATERIALID");
            e.FieldKeys.Add("FREQQTY");
        }
        /// <summary>
        /// 区域订货
        /// 有区域门店订货
        /// 无区域门店订货
        /// 营建订货
        /// </summary>
        /// <param name="e"></param>
        public override void BeginOperationTransaction(BeginOperationTransactionArgs e)
        {
            base.BeginOperationTransaction(e);
            if (e.DataEntitys!=null&&e.DataEntitys.Count()>0)
            {
                foreach (DynamicObject dataEntity in e.DataEntitys)
                {
                    long ApplicationOrgId = Convert.ToInt64(dataEntity["ApplicationOrgId_Id"]);//申请组织ID
                    Customer cust = XSXServiceHelper.XSXServiceHelper.GetIntCustomerProperty(this.Context, ApplicationOrgId);
                    DynamicObject custObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "BD_Customer", cust.custID);//申请客户
                    DynamicObject BelongCust = cust.BelongCustID > 0 ? XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "BD_Customer", cust.BelongCustID) : null;
                    //DynamicObject BelongCust = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "BD_Customer", cust.BelongCustID);//服务端获取所属区域对象
                    DynamicObject CustType = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "PAEZ_CUSTTYPE", cust.CustTypeID);//服务端获取客户类别
                    DynamicObject Brand = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "ORG_Organizations", cust.BrandID);//服务端获取所属品牌对象,组织基础资料
                    DynamicObject Region = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "PAEZ_REGION", cust.RegionID);//服务端获取所属大区对象
                    DynamicObject OrgType = ((DynamicObject)dataEntity["ApplicationOrgId"])["FORGTYPE"] as DynamicObject;//组织类型
                    dataEntity["FApplyCust"] = custObject;
                    dataEntity["FApplyCust_Id"] = cust.custID;
                    if (BelongCust != null)
                    {
                        dataEntity["FBelongCust"] = BelongCust;
                        dataEntity["FBelongCust_Id"] = cust.BelongCustID;
                    }
                    dataEntity["FCUSTTYPE"] = CustType;
                    dataEntity["FCUSTTYPE_Id"] = cust.CustTypeID;
                    dataEntity["FORGTYPE"] = OrgType;
                    dataEntity["FORGTYPE_Id"] = Convert.ToInt64(OrgType["Id"]);
                    dataEntity["FSHBRAND"] = Brand;
                    dataEntity["FSHBRAND_Id"] = cust.BrandID;
                    dataEntity["FSHREGION"] = Region;
                    dataEntity["FSHREGION_Id"] = cust.RegionID;
                    ////客户类型：区域客户QY，组织类型：区域QY
                    //if (Convert.ToString(CustType["Number"]).Equals("QY")&&Convert.ToString(OrgType["Number"]).Equals("QY"))
                    //{
                    //    //收款用途：订货保证金：No,SFKYT004 Id:108875 *************按照实际情况进行修改***********
                    //    //组织类型：生产：No，SC，ID：108867 *************按照实际情况进行修改***********
                    //    dataEntity["FOrderAmount"] = XSXServiceHelper.XSXServiceHelper.GetQYAmount(this.Context, cust.custNo); 
                    //}
                    ////客户类型：门店客户MD，组织类型：门店MD
                    //if (Convert.ToString(CustType["Number"]).Equals("MD01") && Convert.ToString(OrgType["Number"]).Equals("MD01"))
                    //{                        
                    //    dataEntity["FOrderAmount"] = XSXServiceHelper.XSXServiceHelper.GetMDAmount(this.Context,cust.custNo, cust.custID);
                    //}
                }
            }
        }

        /// <summary>
        /// 保存成功后，自动提交审核
        /// </summary>
        /// <param name="e"></param>
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            object[] ids = (from p in e.DataEntitys.Where(p => (Convert.ToString(p["DOCUMENTSTATUS"]).Equals("A")))
                            select p[0]).ToArray();//获取收付款用途不等于加盟费的单据ID
            IOperationResult submitResult = XSXServiceHelper.XSXServiceHelper.Submit(this.Context, "DE_SCMS_ApplyGools", ids);
            XSXServiceHelper.XSXServiceHelper.Log(this.Context, "Submit", submitResult);
            if (submitResult.IsSuccess)
            {
                object[] ips = (from c in submitResult.SuccessDataEnity
                                select c[0]).ToArray();
                IOperationResult auditResult = XSXServiceHelper.XSXServiceHelper.Audit(this.Context, "DE_SCMS_ApplyGools", ips);
                XSXServiceHelper.XSXServiceHelper.Log(this.Context, "Audit", auditResult);
            }
        }
    }
}
