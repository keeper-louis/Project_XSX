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

namespace KEEPER.K3.COST_COSTREQUESTBILL.ServicePlugIn
{
    [Description("费用申请单删除操作")]
    public class AutoDelete:AbstractOperationServicePlugIn
    {
        //在一个事务里进行操作，如果清除上游单据失败，则费用申请单删除操作回滚
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            if (e.DataEntitys!=null&&e.DataEntitys.Count()>0)
            {
                foreach (DynamicObject dataEntity in e.DataEntitys)
                {
                    string billNo = Convert.ToString(dataEntity["BillNo"]);
                    //配送出库单查询
                    string SALSql = string.Format(@"/*dialect*/SELECT COUNT(*) SALNUM FROM T_SAL_OUTSTOCK WHERE FCOSTAPPLYNO = '{0}' AND FISCREATEAPPLY = 1", billNo);
                    int SALNum = DBUtils.ExecuteScalar<int>(this.Context, SALSql, -1, null);
                    if (SALNum == 0)
                    {
                        //收款单查询
                        string ARSql = string.Format(@"/*dialect*/SELECT COUNT(*) NUM FROM T_AR_RECEIVEBILL WHERE FCOSTAPPLYNO = '{0}'AND FISCREATEAPPLY = 1", billNo);
                        int Num = DBUtils.ExecuteScalar<int>(this.Context, ARSql, -1, null);
                        if (Num!=0)
                        {
                            string updateARSql = string.Format(@"/*dialect*/UPDATE T_AR_RECEIVEBILL set FCOSTAPPLYNO = ' ',FISCREATEAPPLY = 0 WHERE FCOSTAPPLYNO = '{0}' AND FISCREATEAPPLY = 1", billNo);
                            DBUtils.Execute(this.Context, updateARSql);
                        }
                    }
                    else
                    {
                        string updateSALSql = string.Format(@"/*dialect*/UPDATE T_SAL_OUTSTOCK set FCOSTAPPLYNO = ' ',FISCREATEAPPLY = 0 WHERE FCOSTAPPLYNO = '{0}' AND FISCREATEAPPLY = 1", billNo);
                        DBUtils.Execute(this.Context, updateSALSql);
                    }
                }
            }

        }
    }
}
