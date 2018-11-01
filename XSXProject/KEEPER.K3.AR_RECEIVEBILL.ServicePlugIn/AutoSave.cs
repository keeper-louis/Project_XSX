﻿using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using KEEPER.K3.XSX.Core.Entity;
using Kingdee.BOS.Util;
using Kingdee.BOS.App;
using Kingdee.BOS.Contracts;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.ServiceHelper;
using Kingdee.BOS.Core.DynamicForm;

namespace KEEPER.K3.AR_RECEIVEBILL.ServicePlugIn
{
    [Description("网银交易记录自动生成收款单")]
    [Kingdee.BOS.Util.HotUpdate]
    public class AutoSave: AbstractOperationServicePlugIn
    {
        /// <summary>
        /// 在此事件中给DataEntity本次操作数据包复制，可通过保存操作直接赋值到界面
        /// </summary>
        /// <param name="e"></param>
        public override void BeginOperationTransaction(BeginOperationTransactionArgs e)
        {
            base.BeginOperationTransaction(e);
            if (e.DataEntitys != null && e.DataEntitys.Count() > 0)
            {
                foreach (DynamicObject DataEntity in e.DataEntitys)
                {
                    string WLUnitsType = Convert.ToString(DataEntity["CONTACTUNITTYPE"]);//往来单位类型
                    if (WLUnitsType.EqualsIgnoreCase("BD_Customer"))
                    {
                        Customer cust = XSXServiceHelper.XSXServiceHelper.GetCustomerProperty(this.Context, Convert.ToInt64(DataEntity["CONTACTUNIT_Id"]));
                        DynamicObject BelongCust = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "BD_Customer",cust.BelongCustID);//服务端获取所属区域对象
                        DynamicObject CustType = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "PAEZ_CUSTTYPE", cust.CustTypeID);//服务端获取所属区域对象//客户类别
                        DynamicObject OrgType = ((DynamicObject)DataEntity["FPAYORGID"])["FORGTYPE"] as DynamicObject;//组织类型
                        DataEntity["FBelongCust"] = BelongCust;
                        DataEntity["FBelongCust_Id"] = cust.BelongCustID;
                        DataEntity["FCUSTTYPE"] = CustType;
                        DataEntity["FCUSTTYPE_Id"] = cust.CustTypeID;
                        DataEntity["FORGTYPE"] = OrgType;
                        DataEntity["FORGTYPE_Id"] = Convert.ToInt64(OrgType["Id"]);
                        //组织类型是品牌,******根据实际编码进行修改******
                        if (Convert.ToString(OrgType["Number"]).EqualsIgnoreCase("PP"))
                        {
                            //客户类型是门店，门店加盟费******根据实际编码进行修改******
                            if (Convert.ToString(CustType["Number"]).EqualsIgnoreCase("MD"))
                            {
                                //门店加盟费 ID:108872	No:SFKYT001,******根据实际编码进行修改******
                                DynamicObject ReceivePurposeObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "CN_RECPAYPURPOSE", 108872);//服务端获取收付款用途对象
                                ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID"] = ReceivePurposeObject;
                                ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID_Id"] = 108872;
                            }
                            //客户类型是区域，区域加盟费******根据实际编码进行修改******
                            if (Convert.ToString(CustType["Number"]).EqualsIgnoreCase("QY"))
                            {
                                //区域加盟费ID:108873	No:SFKYT002 ,******根据实际编码进行修改******
                                DynamicObject ReceivePurposeObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "CN_RECPAYPURPOSE", 108873);//服务端获取收付款用途对象
                                ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID"] = ReceivePurposeObject;
                                ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID_Id"] = 108873;
                            }
                        }

                        //组织类型是生产公司******根据实际编码进行修改******
                        if (Convert.ToString(OrgType["Number"]).EqualsIgnoreCase("SC"))
                        {
                            //客户类型是门店，订货货款******根据实际编码进行修改******
                            if (Convert.ToString(CustType["Number"]).EqualsIgnoreCase("MD"))
                            {
                                //订货货款ID:108874	No:SFKYT003 ,******根据实际编码进行修改******
                                DynamicObject ReceivePurposeObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "CN_RECPAYPURPOSE", 108874);//服务端获取收付款用途对象
                                ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID"] = ReceivePurposeObject;
                                ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID_Id"] = 108874;
                            }
                            //客户类型是区域，订货保证金******根据实际编码进行修改******
                            if (Convert.ToString(CustType["Number"]).EqualsIgnoreCase("QY"))
                            {
                                //订货保证金ID:108875	No:SFKYT004 ,******根据实际编码进行修改******
                                DynamicObject ReceivePurposeObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "CN_RECPAYPURPOSE", 108875);//服务端获取收付款用途对象
                                ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID"] = ReceivePurposeObject;
                                ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID_Id"] = 108875;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 保存结束，除收付款用途包含"加盟费"，其余单据自动进行提交审核。
        /// </summary>
        /// <param name="e"></param>
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);

            object[] ids = (from p in e.DataEntitys.Where(p=> !(((DynamicObject)(((DynamicObjectCollection)p["RECEIVEBILLENTRY"])[0])["PURPOSEID"])["Number"].Equals("SFKYT001")||((DynamicObject)(((DynamicObjectCollection)p["RECEIVEBILLENTRY"])[0])["PURPOSEID"])["Number"].Equals("SFKYT002")))
                                select p[0]).ToArray();//获取收付款用途不等于加盟费的单据ID
            IOperationResult submitResult = XSXServiceHelper.XSXServiceHelper.Submit(this.Context, "AR_RECEIVEBILL",ids);
            XSXServiceHelper.XSXServiceHelper.Log(this.Context, "Submit", submitResult);
            if (submitResult.IsSuccess)
            {
                object[] ips = (from c in submitResult.SuccessDataEnity
                                select c[0]).ToArray();
                IOperationResult auditResult = XSXServiceHelper.XSXServiceHelper.Audit(this.Context, "AR_RECEIVEBILL", ips);
                XSXServiceHelper.XSXServiceHelper.Log(this.Context, "Audit", auditResult);
            }         
                        
        }
       

    }
}
