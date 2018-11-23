﻿using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Core.DynamicForm;
using KEEPER.K3.XSXServiceHelper;
using Kingdee.BOS;
using Kingdee.BOS.App.Data;

namespace KEEPER.K3.SALOUT.ServicePlugIn
{
    [Description("配送出库单审核")]
    public class AutoAudit:AbstractOperationServicePlugIn
    {
        private string orgNumber = "1004";//默认为生产公司，以后启用多生产公司的时候在说
        private string belongCustNumbaer = string.Empty;
        double YJSum = 0;
        double Amount = 0;
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            base.OnPreparePropertys(e);
            e.FieldKeys.Add("FCommissionSum");
            e.FieldKeys.Add("FCUSTTYPE");
            e.FieldKeys.Add("FBelongCust");
            e.FieldKeys.Add("FBILLAMOUNT");
        }

        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            if (e.DataEntitys!=null&&e.DataEntitys.Count()>0)
            {
                foreach (DynamicObject DataEntity in e.DataEntitys)
                {
                    //佣金合计大于0并且客户类型=门店，审核时需要自动生成费用申请单并且自动生成生产公司给区域的其他应付单
                    if (Convert.ToDouble(DataEntity["FCommissionSum"])>0&&Convert.ToString(((DynamicObject)DataEntity["FCUSTTYPE"])["Number"]).Equals("MD01"))
                    {
                        belongCustNumbaer = Convert.ToString(((DynamicObject)DataEntity["FBelongCust"])["Number"]);
                        YJSum = Convert.ToDouble(DataEntity["FCommissionSum"]);
                        DynamicObjectCollection DataEntityFIn = DataEntity["SAL_OUTSTOCKFIN"] as DynamicObjectCollection;
                        Amount = Convert.ToDouble(DataEntityFIn[0]["BillAmount"]);
                        long BillID = Convert.ToInt64(DataEntity["Id"]);
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
                            string updateSql = string.Format(@"/*dialect*/Update T_SAL_OUTSTOCK set FISCREATEAPPLY = 1,FCOSTAPPLYNO = '{0}' where FID = {1}", costRequestNum, BillID);
                            DBUtils.Execute(this.Context, updateSql);
                        }
                        //生成生产公司给区域的其他应付单
                        Action<IDynamicFormViewService> fillAPBillPropertys = new Action<IDynamicFormViewService>(fillAPPropertys);
                        DynamicObject apModel = XSXServiceHelper.XSXServiceHelper.CreateBillMode(this.Context, "AP_OtherPayable", fillAPBillPropertys);
                        IOperationResult apSaveResult = XSXServiceHelper.XSXServiceHelper.Save(this.Context, "AP_OtherPayable", apModel);
                        XSXServiceHelper.XSXServiceHelper.Log(this.Context, "Save", apSaveResult);
                        if (!apSaveResult.IsSuccess)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("自动创建生产公司-区域其他应付单失败原因：");
                            foreach (var operateResult in apSaveResult.OperateResult)
                            {
                                sb.AppendLine(operateResult.Message);
                            }
                            throw new KDBusinessException("AP_OtherPayable", sb.ToString());
                        }
                        if (apSaveResult.IsSuccess)
                        {
                            //反写"已生成其他应付单"，并反写单据编号
                            string OtherApBilltNum = apSaveResult.OperateResult[0].Number;
                            string updateSql = string.Format(@"/*dialect*/Update T_SAL_OUTSTOCK set FISCREATEOTHERAP = 1,FOTHERAPNo = '{0}' where FID = {1}", OtherApBilltNum, BillID);
                            DBUtils.Execute(this.Context, updateSql);
                        }
                    } 
                }
            }
        }

        private void fillAPPropertys(IDynamicFormViewService dynamicFormView)
        {
            //结算组织：默认为生产组织，后续多组织可以更改
            dynamicFormView.SetItemValueByNumber("FSETTLEORGID", orgNumber, 0);
            //费用承担组织：默认为生产组织，后续多组织可以更改
            //dynamicFormView.SetItemValueByNumber("FCostOrgID", orgNumber, 0);
            //付款组织：默认为生产组织，后续多组织可以更改
            //dynamicFormView.SetItemValueByNumber("FPayOrgID", orgNumber, 0);
            //dynamicFormView.SetItemValueByNumber("FStaffID", "Proxy", 0);//申请人，默认设置为固定的一个人
            //FReason : "配送佣金申请"
            //dynamicFormView.UpdateValue("FReason", 0, "配送佣金申请");
            //FTOCONTACTUNITTYPE : "BD_Customer"，往来单位类型
            dynamicFormView.UpdateValue("FCONTACTUNITTYPE", 0, "BD_Customer");
            //往来单位：收款单的所属区域
            dynamicFormView.SetItemValueByNumber("FCONTACTUNIT", belongCustNumbaer, 0);
            //所属区域：收款单的所属区域
            dynamicFormView.SetItemValueByNumber("FBelongCust", belongCustNumbaer, 0);
            //申请部门：固定值
            dynamicFormView.SetItemValueByNumber("FDEPARTMENTID", "BM000017", 0);
            //费用承担部门：固定值
            //dynamicFormView.SetItemValueByNumber("FCostDeptID", "BM000017", 0);
            //分录
            //费用项目：固定值，返还区域保证金
            dynamicFormView.SetItemValueByNumber("FCOSTID", "400", 0);
            //费用承担部门
            dynamicFormView.SetItemValueByNumber("FCOSTDEPARTMENTID", "BM000017", 0);
            //不含税金额 NOTAXAMOUNTFOR
            dynamicFormView.UpdateValue("FNOTAXAMOUNTFOR", 0, Amount);
            //申请金额：固定值，佣金合计
            //dynamicFormView.UpdateValue("FOrgAmount", 0, YJSum);
        }
        private void fillPropertys(IDynamicFormViewService dynamicFormView)
        {
            
            //((IDynamicFormView)dynamicFormView).InvokeFieldUpdateService("FSTAFFNUMBER", 0);//SetItemValueByNumber不会触发值更新事件，需要继续调用该函数
            
            //申请组织：默认为生产组织，后续多组织可以更改
            dynamicFormView.SetItemValueByNumber("FOrgID", orgNumber, 0);
            //费用承担组织：默认为生产组织，后续多组织可以更改
            dynamicFormView.SetItemValueByNumber("FCostOrgID", orgNumber, 0);
            //付款组织：默认为生产组织，后续多组织可以更改
            dynamicFormView.SetItemValueByNumber("FPayOrgID", orgNumber, 0);
            dynamicFormView.SetItemValueByNumber("FStaffID", "Proxy", 0);//申请人，默认设置为固定的一个人
            //FReason : "配送佣金申请"
            dynamicFormView.UpdateValue("FReason", 0, "配送佣金申请");
            //FTOCONTACTUNITTYPE : "BD_Customer"，往来单位类型
            dynamicFormView.UpdateValue("FTOCONTACTUNITTYPE", 0, "BD_Customer");
            //往来单位：收款单的所属区域
            dynamicFormView.SetItemValueByNumber("FTOCONTACTUNIT", belongCustNumbaer, 0);
            //往来单位：收款单的所属区域
            dynamicFormView.SetItemValueByNumber("FBelongCust", belongCustNumbaer, 0);
            //申请部门：固定值
            dynamicFormView.SetItemValueByNumber("DeptID", "BM000017", 0);
            //费用承担部门：固定值
            dynamicFormView.SetItemValueByNumber("FCostDeptID", "BM000017", 0);
            //分录
            //费用项目：固定值，佣金
            dynamicFormView.SetItemValueByNumber("FExpenseItemID", "200", 0);
            //申请金额：固定值，佣金合计
            dynamicFormView.UpdateValue("FOrgAmount", 0, YJSum);
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
