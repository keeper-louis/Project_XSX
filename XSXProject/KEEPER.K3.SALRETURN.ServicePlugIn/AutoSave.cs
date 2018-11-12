using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using KEEPER.K3.XSXServiceHelper;
using KEEPER.K3.XSX.Core.Entity;

namespace KEEPER.K3.SALRETURN.ServicePlugIn
{
    public class AutoSave:AbstractOperationServicePlugIn
    {
        public override void BeginOperationTransaction(BeginOperationTransactionArgs e)
        {
            base.BeginOperationTransaction(e);
            if (e.DataEntitys!=null&&e.DataEntitys.Count()>0)
            {
                foreach (DynamicObject dataEntity in e.DataEntitys)
                {
                    long StockOrgId = Convert.ToInt64(dataEntity["StockOrgId_Id"]);//退货组织ID
                    Customer cust = XSXServiceHelper.XSXServiceHelper.GetIntCustomerProperty(this.Context, StockOrgId);
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
                }
            }
        }
    }
}
