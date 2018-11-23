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
                //客户类别：区域：QY 
                if (Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals("QY"))
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
                    }
                }
                if (Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals("QY")&& Convert.ToString(((DynamicObject)requestDynamic["FORGTYPE"])["Number"]).Equals("QY"))
                {
                    double TotalAmount = Convert.ToDouble(requestDynamic["TotalAmount"]);
                    double QYAmount = XSXServiceHelper.XSXServiceHelper.GetQYAmount(this.Context, Convert.ToString(((DynamicObject)requestDynamic["FApplyCust"])["Number"]), Convert.ToInt64(((DynamicObject)requestDynamic["FApplyCust"])["Id"]));
                    if (TotalAmount > QYAmount)
                    {
                        string msg = string.Format("单据：{0},订货金额:{2}超出可用额度:{1}", requestDynamic["BillNo"], QYAmount,TotalAmount- QYAmount);
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
                 if (Convert.ToString(((DynamicObject)requestDynamic["FCUSTTYPE"])["Number"]).Equals("MD01") && Convert.ToString(((DynamicObject)requestDynamic["FORGTYPE"])["Number"]).Equals("MD01"))
                {
                    double TotalAmount = Convert.ToDouble(requestDynamic["TotalAmount"]);
                    double MDAmount = XSXServiceHelper.XSXServiceHelper.GetMDAmount(this.Context, Convert.ToString(((DynamicObject)requestDynamic["FApplyCust"])["Number"]), Convert.ToInt64(((DynamicObject)requestDynamic["FApplyCust"])["Id"]));
                    if (TotalAmount > MDAmount)
                    {
                        string msg = string.Format("单据：{0},订货金额:{2}超出可用额度:{1}", requestDynamic["BillNo"], MDAmount,TotalAmount- MDAmount);
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
