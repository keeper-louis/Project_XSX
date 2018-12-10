using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.App.Data;

namespace KEEPER.K3.OTHERAPBILL.ServicePlugIn
{
    [Description("其他应付单删除操作，反写上游配送出库单标识")]
    public class AutoDelete:AbstractOperationServicePlugIn
    {
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            if (e.DataEntitys != null && e.DataEntitys.Count() > 0)
            {
                foreach (DynamicObject dataEntity in e.DataEntitys)
                {
                    string billNo = Convert.ToString(dataEntity["BillNo"]);
                    //配送出库单查询
                    string SALSql = string.Format(@"/*dialect*/SELECT COUNT(*) SALNUM FROM T_SAL_OUTSTOCK WHERE FOTHERAPNO = '{0}'", billNo);
                    int SALNum = DBUtils.ExecuteScalar<int>(this.Context, SALSql, -1, null);
                    if (SALNum > 0)
                    {
                        string updateSALSql1 = string.Format(@"/*dialect*/UPDATE T_SAL_OUTSTOCK set FOTHERAPNO = ' ',FISCREATEOTHERAP = 0 WHERE FOTHERAPNO = '{0}' AND FISCREATEOTHERAP = 1", billNo);
                        List<string> sql = new List<string>();
                        sql.Add(updateSALSql1);
                        DBUtils.ExecuteBatch(this.Context, sql, 1);
                    }
                }
            }
        }
    }
}
