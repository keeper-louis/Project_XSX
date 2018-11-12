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

namespace KEEPER.K3.ORDER_REQ.ServicePlugIn
{
    [Description("要货申请单保存,计算可订货余额的逻辑需要优化,随着数据量增加会越来越慢")]
    public class AutoSave:AbstractOperationServicePlugIn
    {
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
                    DynamicObject BelongCust = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "BD_Customer", cust.BelongCustID);//服务端获取所属区域对象
                    DynamicObject CustType = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "PAEZ_CUSTTYPE", cust.CustTypeID);//服务端获取所属区域对象//客户类别
                    DynamicObject OrgType = ((DynamicObject)dataEntity["ApplicationOrgId"])["FORGTYPE"] as DynamicObject;//组织类型
                    dataEntity["FApplyCust"] = custObject;
                    dataEntity["FApplyCust_Id"] = cust.custID;
                    dataEntity["FBelongCust"] = BelongCust;
                    dataEntity["FBelongCust_Id"] = cust.BelongCustID;
                    dataEntity["FCUSTTYPE"] = CustType;
                    dataEntity["FCUSTTYPE_Id"] = cust.CustTypeID;
                    dataEntity["FORGTYPE"] = OrgType;
                    dataEntity["FORGTYPE_Id"] = Convert.ToInt64(OrgType["Id"]);
                    //客户类型：区域客户QY，组织类型：区域QY
                    if (Convert.ToString(CustType["Number"]).Equals("QY")&&Convert.ToString(OrgType["Number"]).Equals("QY"))
                    {
                        //收款用途：订货保证金：No,SFKYT004 Id:108875 *************按照实际情况进行修改***********
                        //组织类型：生产：No，SC，ID：108867 *************按照实际情况进行修改***********
                        dataEntity["FOrderAmount"] = XSXServiceHelper.XSXServiceHelper.GetQYAmount(this.Context, cust.custID); ;
                    }
                    //客户类型：门店客户MD，组织类型：门店MD
                    if (Convert.ToString(CustType["Number"]).Equals("MD") && Convert.ToString(OrgType["Number"]).Equals("MD"))
                    {                        
                        dataEntity["FOrderAmount"] = XSXServiceHelper.XSXServiceHelper.GetMDAmount(this.Context, cust.custID);
                    }
                }
            }
        }
    }
}
