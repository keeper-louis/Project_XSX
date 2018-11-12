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

namespace KEEPER.K3.AR_RECEIVEBILL.ServicePlugIn
{
    class AutoUnAuditValidator : AbstractValidator
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
                bool isCreate = Convert.ToBoolean(requestDynamic["FIsCreateApply"]);//是否生成费用申请单
                string billNo = Convert.ToString(requestDynamic["BillNo"]);//收款单单号
                string costReqBillNo = Convert.ToString(requestDynamic["FCostApplyNo"]);//费用申请单单号
                DynamicObjectCollection Entry = requestDynamic["RECEIVEBILLENTRY"] as DynamicObjectCollection;
                string PurPoseNum = Convert.ToString(((DynamicObject)Entry[0]["PURPOSEID"])["Number"]);//门店加盟费：SFKYT001
                if (PurPoseNum.Equals("SFKYT001")&&isCreate)
                {
                    string msg = string.Format("单据：{0}收付款用途包含门店加盟费,已生成费用申请单：{1},不允许反审核",billNo,costReqBillNo);
                    var errInfo = new ValidationErrorInfo(
                                    item.BillNo,
                                    item.DataEntity["Id"].ToString(),
                                    item.DataEntityIndex,
                                    item.RowIndex,
                                    "ARValid019",
                                    msg,
                                    " ",
                                    Kingdee.BOS.Core.Validation.ErrorLevel.Error);
                    validateContext.AddError(item.DataEntity, errInfo);

                }
            }
        }
    }
}
