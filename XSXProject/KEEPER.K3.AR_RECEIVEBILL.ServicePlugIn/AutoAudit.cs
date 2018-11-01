using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;

namespace KEEPER.K3.AR_RECEIVEBILL.ServicePlugIn
{
    [Description("收款单审核自动生成费用申请单，收付款用途为xx加盟费，客户类型：门店")]
    public class AutoAudit:AbstractOperationServicePlugIn
    {
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            if (e.DataEntitys!=null&&e.DataEntitys.Count()>0)
            {
                foreach (DynamicObject DataEntity in e.DataEntitys)
                {
                    //收付款用途：xx加盟费
                    if (DataEntity)
                    {

                    }
                }
            }
        }
    }
}
