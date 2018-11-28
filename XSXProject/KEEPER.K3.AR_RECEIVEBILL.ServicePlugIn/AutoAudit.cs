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
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.Bill;
using Kingdee.BOS;

namespace KEEPER.K3.AR_RECEIVEBILL.ServicePlugIn
{
    [Description("收款单审核自动生成费用申请单，收付款用途为xx加盟费，客户类型：门店")]
    public class AutoAudit:AbstractOperationServicePlugIn
    {
        private string orgNumber = string.Empty;
        private string belongCustNumbaer = string.Empty;
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            base.OnPreparePropertys(e);
            e.FieldKeys.Add("FBelongCust");
        }
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            if (e.DataEntitys!=null&&e.DataEntitys.Count()>0)
            {
                foreach (DynamicObject DataEntity in e.DataEntitys)
                {
                    if (Convert.ToString(DataEntity["CONTACTUNITTYPE"]).Equals("BD_Customer"))
                    {
                        if (!XSXServiceHelper.XSXServiceHelper.IsJXCust(this.Context, Convert.ToInt64(DataEntity["CONTACTUNIT_Id"])))
                        {
                            orgNumber = Convert.ToString(((DynamicObject)DataEntity["FPAYORGID"])["Number"]);
                            belongCustNumbaer = Convert.ToString(((DynamicObject)DataEntity["FBelongCust"])["Number"]);
                            long BillID = Convert.ToInt64(DataEntity["Id"]);
                            //收款用途，门店加盟费的ID：113486,******根据实际编码进行修改******
                            string strSql = string.Format(@"/*dialect*/SELECT COUNT(*) NUM FROM T_AR_RECEIVEBILL AR INNER JOIN T_AR_RECEIVEBILLENTRY ARY ON AR.FID = ARY.FID WHERE ARY.FPURPOSEID = 113486 AND AR.FID = {0}", BillID);
                            int num = DBUtils.ExecuteScalar<int>(this.Context, strSql, -1, null);
                            //收付款用途：xx加盟费，客户类型=门店
                            if (num >= 1)//生成费用申请单
                            {
                                Action<IDynamicFormViewService> fillBillPropertys = new Action<IDynamicFormViewService>(fillPropertys);
                                DynamicObject billModel = XSXServiceHelper.XSXServiceHelper.CreateBillMode(this.Context, "ER_ExpenseRequest", fillBillPropertys);
                                IOperationResult saveResult = XSXServiceHelper.XSXServiceHelper.Save(this.Context, "ER_ExpenseRequest", billModel);
                                XSXServiceHelper.XSXServiceHelper.Log(this.Context, "Save", saveResult);
                                if (!saveResult.IsSuccess)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine("自动创建费用申请单失败原因：");
                                    foreach (var operateResult in saveResult.OperateResult)
                                    {
                                        sb.AppendLine(operateResult.Message);
                                    }
                                    throw new KDBusinessException("AutoOperate", sb.ToString());
                                }
                                if (saveResult.IsSuccess)
                                {
                                    //反写"已生成费用申请单"，并反写单据编号
                                    string costRequestNum = saveResult.OperateResult[0].Number;
                                    string updateSql = string.Format(@"/*dialect*/Update T_AR_RECEIVEBILL set FISCREATEAPPLY = 1,FCOSTAPPLYNO = '{0}' where FID = {1}", costRequestNum, BillID);
                                    DBUtils.Execute(this.Context, updateSql);
                                }
                            }
                        }
                    }
                    
                }
            }
        }

        private void fillPropertys(IDynamicFormViewService dynamicFormView)
        {
            dynamicFormView.SetItemValueByNumber("FStaffID", "Proxy", 0);//申请人，默认设置为固定的一个人
            //((IDynamicFormView)dynamicFormView).InvokeFieldUpdateService("FSTAFFNUMBER", 0);//SetItemValueByNumber不会触发值更新事件，需要继续调用该函数
            //FReason : "加盟奖励申请"
            dynamicFormView.UpdateValue("FReason", 0, "加盟奖励申请");
            //申请组织：收款单的收款组织
            dynamicFormView.SetItemValueByNumber("FOrgID", orgNumber, 0);
            //费用承担组织：收款单的收款组织
            dynamicFormView.SetItemValueByNumber("FCostOrgID", orgNumber, 0);
            //付款组织：收款单的收款组织
            dynamicFormView.SetItemValueByNumber("FPayOrgID", orgNumber, 0);
            //FTOCONTACTUNITTYPE : "BD_Customer"，往来单位类型
            dynamicFormView.UpdateValue("FTOCONTACTUNITTYPE", 0, "BD_Customer");
            //往来单位：收款单的所属区域
            dynamicFormView.SetItemValueByNumber("FTOCONTACTUNIT", belongCustNumbaer, 0);
            //往来单位：收款单的所属区域
            dynamicFormView.SetItemValueByNumber("FBelongCust", belongCustNumbaer, 0);
            //费用承担部门：固定值
            dynamicFormView.SetItemValueByNumber("FCostDeptID", "BM000017", 0);
            //分录
            //费用项目：固定值
            dynamicFormView.SetItemValueByNumber("FExpenseItemID", "100", 0);
            //申请金额：固定值：10000
            dynamicFormView.UpdateValue("FOrgAmount", 0, 10000);
            //新增分录
            //((IBillView)dynamicFormView).Model.CreateNewEntryRow("FEntity");
            //如果预知有多条分录，可以使用这个方法进行批量新增
            //((IBillView)dynamicFormView).Model.BatchCreateNewEntryRow("FEntity",100);
            //dynamicFormView.SetItemValueByNumber("FExpenseItemID", "CI001", 1);
            //申请金额：固定值：10000
            //dynamicFormView.UpdateValue("FOrgAmount", 1, 20000);

        }
    }
}
