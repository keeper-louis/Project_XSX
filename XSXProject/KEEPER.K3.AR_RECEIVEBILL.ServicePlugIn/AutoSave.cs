using Kingdee.BOS.Core.DynamicForm.PlugIn;
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
using KEEPER.K3.XSX.Core.ParamOption;

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
                        /*
                         * 1.只要不是吉祥的门店，我们的逻辑都可以处理
                         * 2.如果是有区域的门店，需要调用GetCustomerProperty()方法获取所属区域并赋值
                         * 3.如果是没有区域的门店，需要调用GetWQYCustomerProperty()方法获取客户属性，部队所属区域赋值
                         * 4.根据账号用途进行收付款用途的判断赋值
                         * 5.根据不同收付款用途，进行业务类型的赋值
                         * 6.所有收款单自动进行提交审核。
                         * 7.只有有区域的门店加盟才进行费用申请的计提
                         */
                        //判断是否是吉祥门店，非吉祥门店进行逻辑处理
                        if (!XSXServiceHelper.XSXServiceHelper.IsJXCust(this.Context, Convert.ToInt64(DataEntity["CONTACTUNIT_Id"])))
                        {
                            if (XSXServiceHelper.XSXServiceHelper.IsYQYCust(this.Context, Convert.ToInt64(DataEntity["CONTACTUNIT_Id"]))|| XSXServiceHelper.XSXServiceHelper.IsWQYCust(this.Context, Convert.ToInt64(DataEntity["CONTACTUNIT_Id"])))
                            {
                                DynamicObjectCollection RECEIVEBILLENTRY = DataEntity["RECEIVEBILLENTRY"] as DynamicObjectCollection;
                                DynamicObject AccountUsage = ((DynamicObject)RECEIVEBILLENTRY[0]["ACCOUNTID"])["FACCOUNTUSAGE"] as DynamicObject;//账户用途
                                Customer cust = XSXServiceHelper.XSXServiceHelper.IsYQYCust(this.Context, Convert.ToInt64(DataEntity["CONTACTUNIT_Id"])) ? XSXServiceHelper.XSXServiceHelper.GetCustomerProperty(this.Context, Convert.ToInt64(DataEntity["CONTACTUNIT_Id"])) : XSXServiceHelper.XSXServiceHelper.GetWQYCustomerProperty(this.Context, Convert.ToInt64(DataEntity["CONTACTUNIT_Id"]));
                                // Customer cust = XSXServiceHelper.XSXServiceHelper.GetCustomerProperty(this.Context, Convert.ToInt64(DataEntity["CONTACTUNIT_Id"]));
                                DynamicObject BelongCust = XSXServiceHelper.XSXServiceHelper.IsYQYCust(this.Context, Convert.ToInt64(DataEntity["CONTACTUNIT_Id"])) ? XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "BD_Customer", cust.BelongCustID) : null;
                                //DynamicObject BelongCust = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "BD_Customer", cust.BelongCustID);//服务端获取所属区域对象
                                DynamicObject CustType = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "PAEZ_CUSTTYPE", cust.CustTypeID);//服务端获取客户类别对象
                                DynamicObject Brand = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "ORG_Organizations", cust.BrandID);//服务端获取所属品牌对象,组织基础资料
                                DynamicObject Region = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "PAEZ_REGION", cust.RegionID);//服务端获取所属大区对象
                                DynamicObject OrgType = ((DynamicObject)DataEntity["FPAYORGID"])["FORGTYPE"] as DynamicObject;//组织类型
                                if (BelongCust!=null)
                                {
                                    DataEntity["FBelongCust"] = BelongCust;
                                    DataEntity["FBelongCust_Id"] = cust.BelongCustID;
                                }
                                DataEntity["FCUSTTYPE"] = CustType;
                                DataEntity["FCUSTTYPE_Id"] = cust.CustTypeID;
                                DataEntity["FORGTYPE"] = OrgType;
                                DataEntity["FORGTYPE_Id"] = Convert.ToInt64(OrgType["Id"]);
                                DataEntity["FSHBRAND"] = Brand;
                                DataEntity["FSHBRAND_Id"] = cust.BrandID;
                                DataEntity["FSHREGION"] = Region;
                                DataEntity["FSHREGION_Id"] = cust.RegionID;
                                RECEIVEBILLENTRY[0]["FACCOUNTUSAGE"] = AccountUsage;
                                RECEIVEBILLENTRY[0]["FACCOUNTUSAGE_Id"] = Convert.ToInt64(AccountUsage["Id"]);
                                //根据账号用途进行收付款用途对应，业务类型对应
                                if (Convert.ToString(AccountUsage["Number"])!=null&& !Convert.ToString(AccountUsage["Number"]).EqualsIgnoreCase(" "))
                                {
                                    if (XSXServiceHelper.XSXServiceHelper.GetSFKYT(this.Context, Convert.ToString(AccountUsage["Number"]))!=0)
                                    {
                                        DynamicObject ReceivePurposeObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "CN_RECPAYPURPOSE", XSXServiceHelper.XSXServiceHelper.GetSFKYT(this.Context, Convert.ToString(AccountUsage["Number"])));
                                        ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID"] = ReceivePurposeObject;
                                        ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID_Id"] = XSXServiceHelper.XSXServiceHelper.GetSFKYT(this.Context, Convert.ToString(AccountUsage["Number"]));
                                    }
                                    if (XSXServiceHelper.XSXServiceHelper.GetYWTYPE(this.Context, Convert.ToString(AccountUsage["Number"]), Convert.ToInt64(DataEntity["CONTACTUNIT_Id"]))!=0)
                                    {
                                        DynamicObject YwTypeObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "PAEZ_SALSERVICE", XSXServiceHelper.XSXServiceHelper.GetYWTYPE(this.Context, Convert.ToString(AccountUsage["Number"]), Convert.ToInt64(DataEntity["CONTACTUNIT_Id"])));
                                        DataEntity["FYWTYPE"] = YwTypeObject;
                                        DataEntity["FYWTYPE_Id"] = XSXServiceHelper.XSXServiceHelper.GetYWTYPE(this.Context, Convert.ToString(AccountUsage["Number"]), Convert.ToInt64(DataEntity["CONTACTUNIT_Id"]));
                                    }
                                }
                                //组织类型是品牌,******根据实际编码进行修改******
                                //if (Convert.ToString(OrgType["Number"]).EqualsIgnoreCase(ConstantBaseData.BrandOrgNo))
                                // {
                                /*
                                 * 客户类别是有区域或者无区域门店
                                 * ConstantBaseDate增加账号用途,业务类型，收付款用途ID
                                 * 银行账号维护账户用途
                                 * 收款单增加业务类型、账号用途两个字段，账号用途根据我方银行账号进行属性获取携带，当业务类型为门店加盟、保证金时为空，订货货款，区域订货保证金、营建费用时为对应业务类型
                                 * 账号用途是门店加盟费则收付款用途为门店加盟费，账号用途为门店加盟保证金则收付款用途为门店加盟保证金
                                 * 同理使用于区域加盟
                                 */
                                //客户类型是门店，门店加盟费******根据实际编码进行修改******
                                //if (Convert.ToString(CustType["Number"]).EqualsIgnoreCase(ConstantBaseData.YQYMDNO) || Convert.ToString(CustType["Number"]).EqualsIgnoreCase(ConstantBaseData.WQYMDNO))
                                //{
                                //    //门店加盟费 ID:113486	No:005,******根据实际编码进行修改******
                                //    DynamicObject ReceivePurposeObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "CN_RECPAYPURPOSE", 113486);//服务端获取收付款用途对象
                                //    ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID"] = ReceivePurposeObject;
                                //    ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID_Id"] = 113486;
                                //}
                                ////客户类型是区域，区域加盟费******根据实际编码进行修改******
                                //if (Convert.ToString(CustType["Number"]).EqualsIgnoreCase("QY"))
                                //{
                                //    //区域加盟费ID:107586	No:100 ,******根据实际编码进行修改******
                                //    DynamicObject ReceivePurposeObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "CN_RECPAYPURPOSE", 107586);//服务端获取收付款用途对象
                                //    ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID"] = ReceivePurposeObject;
                                //    ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID_Id"] = 107586;
                                //}
                                ////}

                                ////组织类型是生产公司******根据实际编码进行修改******
                                //// if (Convert.ToString(OrgType["Number"]).EqualsIgnoreCase("SC"))
                                ////{
                                ////客户类型是门店，订货货款******根据实际编码进行修改******
                                //if (Convert.ToString(CustType["Number"]).EqualsIgnoreCase("MD01"))
                                //{
                                //    //订货货款ID:107610	No:400 ,******根据实际编码进行修改******
                                //    DynamicObject ReceivePurposeObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "CN_RECPAYPURPOSE", 107610);//服务端获取收付款用途对象
                                //    ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID"] = ReceivePurposeObject;
                                //    ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID_Id"] = 107610;
                                //}
                                ////客户类型是区域，订货保证金******根据实际编码进行修改******
                                //if (Convert.ToString(CustType["Number"]).EqualsIgnoreCase("QY"))
                                //{
                                //    //订货保证金ID:107608	No:300 ,******根据实际编码进行修改******
                                //    DynamicObject ReceivePurposeObject = XSXServiceHelper.XSXServiceHelper.GetBasicObject(this.Context, "CN_RECPAYPURPOSE", 107608);//服务端获取收付款用途对象
                                //    ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID"] = ReceivePurposeObject;
                                //    ((DynamicObjectCollection)DataEntity["RECEIVEBILLENTRY"])[0]["PURPOSEID_Id"] = 107608;
                                //}
                                //}
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

            /*只取第一行判断，会造成现象，有的单据能自动提交审核，有的单据不会*/
            //object[] ids = (from p in e.DataEntitys.Where(p=> !(((DynamicObject)(((DynamicObjectCollection)p["RECEIVEBILLENTRY"])[0])["PURPOSEID"])["Number"].Equals("005")||((DynamicObject)(((DynamicObjectCollection)p["RECEIVEBILLENTRY"])[0])["PURPOSEID"])["Number"].Equals("100")||Convert.ToInt64(p["FCUSTTYPE_Id"])==0))
            //                    select p[0]).ToArray();//获取收付款用途不等于加盟费的单据ID
            object[] ids = (from p in e.DataEntitys.Where(p => (Convert.ToString(p["DOCUMENTSTATUS"]).EqualsIgnoreCase("A")))
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
