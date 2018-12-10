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

namespace KEEPER.K3.SALOUT.ServicePlugIn
{
    public class AutoUnAuditValidator : AbstractValidator
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
                bool isCreate = Convert.ToBoolean(requestDynamic["FIsCreateApply"]);//是否生成佣金费用申请单
                string costReqBillNo = Convert.ToString(requestDynamic["FCostApplyNo"]);//佣金费用申请单单号
                bool ISCREATEOTHERAP = Convert.ToBoolean(requestDynamic["FISCREATEOTHERAP"]);//是否生成其他应付单
                string OTHERAPNO = Convert.ToString(requestDynamic["FOTHERAPNO"]);//其他应付单号
                bool ISCREATEBOUNS = Convert.ToBoolean(requestDynamic["FISCREATEBOUNS"]);//是否生成提点申请单
                string BOUNSAPPLYBILLNO = Convert.ToString(requestDynamic["FBOUNSAPPLYBILLNO"]);//提点申请单号
                bool ISCREATELOGISTICS = Convert.ToBoolean(requestDynamic["FISCREATELOGISTICS"]);//是否生成管销申请单
                string LOGISTICSAPPLYBILLNO = Convert.ToString(requestDynamic["FLOGISTICSAPPLYBILLNO"]);//管销申请单号
                string billNo = Convert.ToString(requestDynamic["BillNo"]);
                if (isCreate||ISCREATEOTHERAP||ISCREATEBOUNS||ISCREATELOGISTICS)
                {
                    string msg = string.Format("单据：{0}配送出库单,已生成下游单据,不允许反审核,具体信息见单据资金池页签", billNo);
                    var errInfo = new ValidationErrorInfo(
                                    item.BillNo,
                                    item.DataEntity["Id"].ToString(),
                                    item.DataEntityIndex,
                                    item.RowIndex,
                                    "SALOUTValid019",
                                    msg,
                                    " ",
                                    Kingdee.BOS.Core.Validation.ErrorLevel.Error);
                    validateContext.AddError(item.DataEntity, errInfo);
                }
            }
        }
    }
}
