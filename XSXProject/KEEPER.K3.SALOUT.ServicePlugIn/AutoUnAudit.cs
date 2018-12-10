using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;

namespace KEEPER.K3.SALOUT.ServicePlugIn
{
    [Description("配送出库单反审核")]
    public class AutoUnAudit:AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            base.OnPreparePropertys(e);
            e.FieldKeys.Add("FIsCreateApply");//是否生成佣金申请
            e.FieldKeys.Add("FCostApplyNo");//佣金申请单号
            e.FieldKeys.Add("FISCREATEOTHERAP");//是否生成其他应付单
            e.FieldKeys.Add("FOTHERAPNO");//其他应付单号
            e.FieldKeys.Add("FISCREATEBOUNS");//是否生成提点申请单
            e.FieldKeys.Add("FBOUNSAPPLYBILLNO");//提点申请单号
            e.FieldKeys.Add("FISCREATELOGISTICS");//是否生成管销申请单
            e.FieldKeys.Add("FLOGISTICSAPPLYBILLNO");//管销申请单号
        }
        public override void OnAddValidators(AddValidatorsEventArgs e)
        {
            base.OnAddValidators(e);
            AutoUnAuditValidator unAuditValidator = new AutoUnAuditValidator();
            unAuditValidator.EntityKey = "FBillHead";
            e.Validators.Add(unAuditValidator);
        }
    }
}
