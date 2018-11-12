using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;

namespace KEEPER.K3.AR_RECEIVEBILL.ServicePlugIn
{
    [Description("收款单反审核")]
    public class AutoUnAudit:AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            base.OnPreparePropertys(e);
            e.FieldKeys.Add("FIsCreateApply");
            e.FieldKeys.Add("FCostApplyNo");
        }
        public override void OnAddValidators(AddValidatorsEventArgs e)
        {
            AutoUnAuditValidator unAuditValidator = new AutoUnAuditValidator();
            unAuditValidator.EntityKey = "FBillHead";
            e.Validators.Add(unAuditValidator);
        }
        
    }
}
