using Kingdee.BOS.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS;
using Kingdee.BOS.Core;
using Kingdee.BOS.Util;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.App.Data;
using System.Data;
using KEEPER.K3.XSX.Core.ParamOption;

namespace KEEPER.K3.ORDER_REQ.ServicePlugIn
{
    public class AutoAuditValidator : AbstractValidator
    {
        public override void Validate(ExtendedDataEntity[] dataEntities, ValidateContext validateContext, Context ctx)
        {
            if (dataEntities.IsNullOrEmpty() || dataEntities.Length == 0)
            {
                return;
            }

            foreach (ExtendedDataEntity item in dataEntities)
            {
                DynamicObject requestDynamic = item.DataEntity;
                //有区域门店订货校验起订量
                if ((Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals(ConstantBaseData.YQYMDNO) && Convert.ToInt64(((DynamicObject)requestDynamic["FYWTYPE"])["id"]) == ConstantBaseData.YQYMDDHID))
                {
                    double TotalAmount = Convert.ToDouble(requestDynamic["TotalAmount"]);
                    if (TotalAmount < 1000)
                    {
                        string msg = string.Format("单据：{0},订货金额:{1}小于门店起订金额1000", requestDynamic["BillNo"], TotalAmount);
                        var errInfo = new ValidationErrorInfo(
                                        item.BillNo,
                                        item.DataEntity["Id"].ToString(),
                                        item.DataEntityIndex,
                                        item.RowIndex,
                                        "Valid019",
                                        msg,
                                        " ",
                                        Kingdee.BOS.Core.Validation.ErrorLevel.Error);
                        validateContext.AddError(item.DataEntity, errInfo);
                    }
                }

                //有区域门店，订货可发量控制
                if ((Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals(ConstantBaseData.YQYMDNO) && Convert.ToInt64(((DynamicObject)requestDynamic["FYWTYPE"])["id"]) == ConstantBaseData.YQYMDDHID))
                {

                    long custId = Convert.ToInt64(((DynamicObject)requestDynamic["FAPPLYCUST"])["Id"]);
                    DynamicObjectCollection dyObjectCol = requestDynamic["FEntity"] as DynamicObjectCollection;
                    foreach (DynamicObject dyObject in dyObjectCol)
                    {
                        long stockOrgId = Convert.ToInt64(((DynamicObject)dyObject["FDispatchOrgIdDetail"])["Id"]);
                        long masterId = Convert.ToInt64(((DynamicObject)dyObject["MaterialId"])["msterID"]);
                        long baseUnitId = Convert.ToInt64(((DynamicObjectCollection)((DynamicObject)dyObject["MaterialId"])["MaterialBase"])[0]["BaseUnitId_Id"]);
                        long stockUnitId = Convert.ToInt64(((DynamicObjectCollection)((DynamicObject)dyObject["MaterialId"])["MaterialStock"])[0]["StoreUnitID_Id"]);

                        double kfQty = XSXServiceHelper.XSXServiceHelper.GetKFQty(this.Context, stockOrgId, masterId, custId, baseUnitId, stockUnitId);
                        double reqNum = Convert.ToDouble(dyObject["ReqQty"]);
                        if (reqNum > kfQty)
                        {
                            string msg = string.Format("单据：{0},第{1}行申请数量：{2}超出库存可发量：{3},不允许进行订货", requestDynamic["BillNo"], dyObject["Seq"], reqNum, reqNum - kfQty);
                            var errInfo = new ValidationErrorInfo(
                                            item.BillNo,
                                            item.DataEntity["Id"].ToString(),
                                            item.DataEntityIndex,
                                            item.RowIndex,
                                            "Valid019",
                                            msg,
                                            " ",
                                            Kingdee.BOS.Core.Validation.ErrorLevel.Error);
                            validateContext.AddError(item.DataEntity, errInfo);
                            continue;
                        }
                    }
                }

                //客户类别：区域：QY 
                if (Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals(ConstantBaseData.QYMDNO)&&Convert.ToInt64(((DynamicObject)requestDynamic["FYWTYPE"])["id"])== ConstantBaseData.QYDHID)
                {
                    string strSql = string.Format(@"/*dialect*/select a.FBILLNO, b.FSEQ
  from T_SCMS_ApplyGools a
 inner join T_SCMS_ApplyGoolsEntry b
    on a.FID = b.FID
 where a.FAPPLYCUST = {0}
   and b.FMRPCLOSESTATUS <> 'B'
   and a.FCUSTTYPE = {1}
   and a.FID <> {2} ", Convert.ToInt64(((DynamicObject)requestDynamic["FAPPLYCUST"])["Id"]), Convert.ToInt64(((DynamicObject)requestDynamic["FCUSTTYPE"])["Id"]), Convert.ToInt64(requestDynamic["Id"]));
                    using (IDataReader reader = DBUtils.ExecuteReader(this.Context, strSql))
                    {
                        while (reader.Read())
                        {
                            string msg = string.Format("历史单据：{0},第{1}行业务没有关闭,不允许审核", reader["FBILLNO"], reader["FSEQ"]);
                            var errInfo = new ValidationErrorInfo(
                                            item.BillNo,
                                            item.DataEntity["Id"].ToString(),
                                            item.DataEntityIndex,
                                            item.RowIndex,
                                            "Valid019",
                                            msg,
                                            " ",
                                            Kingdee.BOS.Core.Validation.ErrorLevel.Error);
                            validateContext.AddError(item.DataEntity, errInfo);
                            continue;
                        }
                        reader.Close();
                    }
                }
                //区域可订货余额校验
                if (Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals(ConstantBaseData.QYMDNO) && Convert.ToInt64(((DynamicObject)requestDynamic["FYWTYPE"])["id"]) == ConstantBaseData.QYDHID)
                {
                    double TotalAmount = Convert.ToDouble(requestDynamic["TotalAmount"]);
                    double QYAmount = XSXServiceHelper.XSXServiceHelper.GetQYAmount(this.Context, Convert.ToString(((DynamicObject)requestDynamic["FApplyCust"])["Number"]));
                    if (TotalAmount > QYAmount)
                    {
                        string msg = string.Format("单据：{0},订货金额:{1}超出可用额度:{2}", requestDynamic["BillNo"], TotalAmount, TotalAmount- QYAmount);
                        var errInfo = new ValidationErrorInfo(
                                        item.BillNo,
                                        item.DataEntity["Id"].ToString(),
                                        item.DataEntityIndex,
                                        item.RowIndex,
                                        "Valid019",
                                        msg,
                                        " ",
                                        Kingdee.BOS.Core.Validation.ErrorLevel.Error);
                        validateContext.AddError(item.DataEntity, errInfo);
                    }
                }
                //有区域门店，无区域门店可订货余额检验
                if ((Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals(ConstantBaseData.YQYMDNO) && Convert.ToInt64(((DynamicObject)requestDynamic["FYWTYPE"])["id"]) == ConstantBaseData.YQYMDDHID) || (Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals(ConstantBaseData.WQYMDNO) && Convert.ToInt64(((DynamicObject)requestDynamic["FYWTYPE"])["id"]) == ConstantBaseData.WQYMDDHID))
                {
                    double TotalAmount = Convert.ToDouble(requestDynamic["TotalAmount"]);
                    double MDAmount = XSXServiceHelper.XSXServiceHelper.GetMDAmount(this.Context, Convert.ToString(((DynamicObject)requestDynamic["FApplyCust"])["Number"]), Convert.ToInt64(((DynamicObject)requestDynamic["FApplyCust"])["Id"]));
                    if (TotalAmount > MDAmount)
                    {
                        string msg = string.Format("单据：{0},订货金额:{1}超出可用额度:{2}", requestDynamic["BillNo"], TotalAmount, TotalAmount- MDAmount);
                        var errInfo = new ValidationErrorInfo(
                                        item.BillNo,
                                        item.DataEntity["Id"].ToString(),
                                        item.DataEntityIndex,
                                        item.RowIndex,
                                        "Valid019",
                                        msg,
                                        " ",
                                        Kingdee.BOS.Core.Validation.ErrorLevel.Error);
                        validateContext.AddError(item.DataEntity, errInfo);
                    }
                }

                if (Convert.ToInt64(((DynamicObject)requestDynamic["FYWTYPE"])["id"]) == ConstantBaseData.MDYJID && (Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals(ConstantBaseData.YQYMDNO)|| Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals(ConstantBaseData.WQYMDNO)))
                {
                    double TotalAmount = Convert.ToDouble(requestDynamic["TotalAmount"]);
                    double YJAmount = XSXServiceHelper.XSXServiceHelper.GetYJAmount(this.Context, Convert.ToString(((DynamicObject)requestDynamic["FApplyCust"])["Number"]), Convert.ToInt64(((DynamicObject)requestDynamic["FApplyCust"])["Id"]));
                    if (TotalAmount > YJAmount)
                    {
                        string msg = string.Format("单据：{0},订货金额:{1}超出营建可用额度:{2}", requestDynamic["BillNo"], TotalAmount, TotalAmount - YJAmount);
                        var errInfo = new ValidationErrorInfo(
                                        item.BillNo,
                                        item.DataEntity["Id"].ToString(),
                                        item.DataEntityIndex,
                                        item.RowIndex,
                                        "Valid019",
                                        msg,
                                        " ",
                                        Kingdee.BOS.Core.Validation.ErrorLevel.Error);
                        validateContext.AddError(item.DataEntity, errInfo);
                    }
                }
                
            }
        }
    }
}
