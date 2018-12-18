using Kingdee.BOS.Core.Bill.PlugIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using System.ComponentModel;
using KEEPER.K3.XSX.Core.Entity;
using KEEPER.K3.XSXServiceHelper;
using Kingdee.BOS.Orm.DataEntity;
using KEEPER.K3.XSX.Core.ParamOption;

namespace KEEPER.K3.ORDER_REQ.BusinessPlugIn
{
    [Description("门店要货申请单-表单插件")]
    public class OrderReqBill:AbstractBillPlugIn
    {
        public override void DataChanged(DataChangedEventArgs e)
        {
            if (e.Field.Key.ToUpperInvariant().Equals("FYWTYPE"))
            {
                //业务类型=区域订货，客户类型=区域门店
                if (Convert.ToInt64(e.NewValue) == ConstantBaseData.QYDHID&&Convert.ToString(((DynamicObject)this.Model.GetValue("FCUSTTYPE"))["Number"]).Equals(ConstantBaseData.QYMDNO))
                {
                    this.Model.SetValue("FOrderAmount", XSXServiceHelper.XSXServiceHelper.GetQYAmount(this.Context, Convert.ToString(((DynamicObject)this.Model.GetValue("FApplyCust"))["Number"])));
                }
                //业务类型=有区域门店订货，客户类型=有区域门店
                if ((Convert.ToInt64(e.NewValue) == ConstantBaseData.YQYMDDHID && Convert.ToString(((DynamicObject)this.Model.GetValue("FCUSTTYPE"))["Number"]).Equals(ConstantBaseData.YQYMDNO))||(Convert.ToInt64(e.NewValue) == ConstantBaseData.WQYMDDHID && Convert.ToString(((DynamicObject)this.Model.GetValue("FCUSTTYPE"))["Number"]).Equals(ConstantBaseData.WQYMDNO)))
                {
                    this.Model.SetValue("FOrderAmount", XSXServiceHelper.XSXServiceHelper.GetMDAmount(this.Context, Convert.ToString(((DynamicObject)this.Model.GetValue("FApplyCust"))["Number"]),Convert.ToInt64(((DynamicObject)this.Model.GetValue("FApplyCust"))["Id"])));
                }
                //业务类型=营建，客户类型=有区域门店，无区域门店
                if (Convert.ToInt64(e.NewValue) == ConstantBaseData.MDYJID&&(Convert.ToString(((DynamicObject)this.Model.GetValue("FCUSTTYPE"))["Number"]).Equals(ConstantBaseData.YQYMDNO)|| Convert.ToString(((DynamicObject)this.Model.GetValue("FCUSTTYPE"))["Number"]).Equals(ConstantBaseData.WQYMDNO)))
                {
                    this.Model.SetValue("FOrderAmount",XSXServiceHelper.XSXServiceHelper.GetYJAmount(this.Context, Convert.ToString(((DynamicObject)this.Model.GetValue("FApplyCust"))["Number"]), Convert.ToInt64(((DynamicObject)this.Model.GetValue("FApplyCust"))["Id"])));
                }
                    
            }
            //明细分录申请数量
            if (e.Field.Key.ToUpperInvariant().Equals("FREQQTY"))
            {
                //业务类型有区域门店订货，获取库存可发量 = 即时库存数量-锁库数量-（申请数量-已配送数量）
                if (Convert.ToInt64(((DynamicObject)this.Model.GetValue("FYWTYPE"))["Id"]) == ConstantBaseData.YQYMDDHID && Convert.ToString(((DynamicObject)this.Model.GetValue("FCUSTTYPE"))["Number"]).Equals(ConstantBaseData.YQYMDNO) && Convert.ToInt64(e.NewValue) > 0)
                {
                    double kfqty = 0;
                    long stockOrgID = Convert.ToInt64(((DynamicObject)this.Model.GetValue("FDispatchOrgIdDetail", e.Row))["Id"]);
                    long masterID = Convert.ToInt64(((DynamicObject)this.Model.GetValue("FMaterialId", e.Row))["msterID"]);
                    long custID = Convert.ToInt64(((DynamicObject)this.Model.GetValue("FApplyCust"))["Id"]);
                    if (stockOrgID>0&&masterID>0&&custID>0)
                    {
                        kfqty = XSXServiceHelper.XSXServiceHelper.GetKFQty(this.Context, stockOrgID, masterID, custID);
                        this.Model.SetValue("FINVENTORYQTY",kfqty,e.Row);
                    }
                }
            }
            

        }
        public override void AfterCreateNewData(EventArgs e)
        {
            base.AfterCreateNewData(e);
            long ApplicationOrgId = Convert.ToInt64(((DynamicObject)this.Model.GetValue("FApplicationOrgId"))["Id"]);//申请组织ID
            Customer cust = XSXServiceHelper.XSXServiceHelper.GetIntCustomerProperty(this.Context, ApplicationOrgId);
            DynamicObject custObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "BD_Customer", cust.custID);//申请客户
            DynamicObject BelongCust = cust.BelongCustID > 0 ? XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "BD_Customer", cust.BelongCustID) : null;
            //DynamicObject BelongCust = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "BD_Customer", cust.BelongCustID);//服务端获取所属区域对象
            DynamicObject CustType = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "PAEZ_CUSTTYPE", cust.CustTypeID);//服务端获取客户类别
            DynamicObject Brand = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "ORG_Organizations", cust.BrandID);//服务端获取所属品牌对象,组织基础资料
            DynamicObject Region = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "PAEZ_REGION", cust.RegionID);//服务端获取所属大区对象
            DynamicObject OrgType = ((DynamicObject)this.Model.GetValue("FApplicationOrgId"))["FORGTYPE"] as DynamicObject;//组织类型
            this.Model.SetValue("FApplyCust", custObject);
            this.Model.SetValue("FApplyCust_Id", cust.custID);
            if (BelongCust != null)
            {
                this.Model.SetValue("FBelongCust", BelongCust);
                this.Model.SetValue("FBelongCust_Id", cust.BelongCustID);
            }
            this.Model.SetValue("FCUSTTYPE", CustType);
            this.Model.SetValue("FCUSTTYPE_Id", cust.CustTypeID);
            this.Model.SetValue("FORGTYPE", OrgType);
            this.Model.SetValue("FORGTYPE_Id", Convert.ToInt64(OrgType["Id"]));
            this.Model.SetValue("FSHBRAND", Brand);
            this.Model.SetValue("FSHBRAND_Id", cust.BrandID);
            this.Model.SetValue("FSHREGION", Region);
            this.Model.SetValue("FSHREGION_Id", cust.RegionID);
        }
    }
}
