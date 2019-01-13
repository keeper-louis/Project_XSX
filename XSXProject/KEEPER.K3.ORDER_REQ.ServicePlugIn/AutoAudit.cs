using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;

namespace KEEPER.K3.ORDER_REQ.ServicePlugIn
{
    [Description("要货申请单审核")]
    public class AutoAudit:AbstractOperationServicePlugIn
    {
        //public override void OnPreparePropertys(PreparePropertysEventArgs e)
        //{
        //    base.OnPreparePropertys(e);
        //    e.FieldKeys.Add("FCUSTTYPE");
        //    e.FieldKeys.Add("FAPPLYCUST");
        //    e.FieldKeys.Add("FORGTYPE");
        //    e.FieldKeys.Add("FTOTALAMOUNT");
        //}
        //public override void OnAddValidators(AddValidatorsEventArgs e)
        //{
        //    AutoAuditValidator auditValidator = new AutoAuditValidator();
        //    auditValidator.EntityKey = "FBillHead";
        //    e.Validators.Add(auditValidator);
        //}
    }
}
