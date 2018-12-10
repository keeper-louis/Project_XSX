using Kingdee.BOS.Contracts.Report;
using Kingdee.BOS.Core.Permission.Objects;
using Kingdee.BOS.Core.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KEEPER.K3.CAPITALPOOL.BRAND
{
    [Description("品牌公司资金池报表取数服务端插件")]
    public class BrandCompanyCapitalPoolReport:SysReportBaseService
    {
        /// <summary>
        /// 初始化事件：在此事件中设置报表的基本属性
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            //账表类型【REPORTTYPE_NORMAL】= 简单账表,【REPORTTYPE_MOVE】= 分页账表,【REPORTTYPE_TREE】= 树形账表
            this.ReportProperty.ReportType = Kingdee.BOS.Core.Report.ReportType.REPORTTYPE_NORMAL;
            //是否分组求和
            this.ReportProperty.IsGroupSummary = true;
            //联查单据的FFORMID字段，需要在报表数据构建该列
            this.ReportProperty.FormIdFieldName = "FFORMID";
            //联查单据的单据内码字段，需要在报表数据构建该列
            this.ReportProperty.BillKeyFieldName = "FBILLID";
            //联查单据的单据类型字段，需要在报表数据构建该列
            this.ReportProperty.BillTypeFieldName = "FBILLTYPE";
            //报表名称,this.Context.UserLocale.LCID，语言设定
            this.ReportProperty.ReportName = new Kingdee.BOS.LocaleValue("品牌公司资金池", this.Context.UserLocale.LCID);
            //区别是否为同一单据的标识,一般使用【FID】OR【FBILLNO】,报表中两行记录的【FID】OR【FBILLNO】相同则代表是同一张单据的两行分录，单据编号或者单据ID可以不重复显示，需要在报表数据构建该列
            this.ReportProperty.PrimaryKeyFieldName = "FBILLNO";
            //账表列头是否是通过BOSIDE设计
            this.ReportProperty.IsUIDesignerColumns = false;
            //是否锁定账表表格列
            this.ReportProperty.SimpleAllCols = false;
            //列表格式化列，指示Key列被Value列内容替代
            this.SetDspInsteadColumns();
            //设置精度控制
            this.SetDecimalControl();
        }

        public override void BuilderReportSqlAndTempTable(IRptParams filter, string tableName)
        {
            //base.BuilderReportSqlAndTempTable(filter, tableName);
            //获取报表业务对象数据规则权限，需测试
            //List<BaseDataTempTable> baseDateTempTable = filter.BaseDataTempTable;

        }

       

        private void SetDspInsteadColumns()
        {
            //this.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns = new Dictionary<string, string>();
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FMATERIALID", "FMATERIALNUMBER");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FMATERIALTYPEID", "FMATERIALTYPENAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FAUXPROPID", "FAUXPROP");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FSTOCKID", "FSTOCKNAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FDEPARTMENTID", "FDEPARTMENTNAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FSTOCKLOCID", "FSTOCKLOC");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FSTOCKSTATUSID", "FSTOCKSTATUSNAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FBOMID", "FBOMNO");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FBILLTYPE", "FBILLTYPENAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FBASEUNITID", "FBASEUNITNAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FSTOCKUNITID", "FSTOCKUNITNAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FSECUNITID", "FSECUNITNAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FOWNERTYPEID", "FOWNERTYPENAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FOWNERID", "FOWNERNAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FKEEPERID", "FKEEPERNAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FKEEPERTYPEID", "FKEEPERTYPENAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FCUSTID", "FCUSTRNAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FSUPPLIERID", "FSUPPLIERNAME");
            //base.ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FSTOCKORGID", "FSTOCKORGNAME");
        }

        private void SetDecimalControl()
        {
            //List<DecimalControlField> list = new List<DecimalControlField>();
            //// 数量
            //list.Add(new DecimalControlField
            //{
            //    ByDecimalControlFieldName = "FQty",
            //    DecimalControlFieldName = "FUnitPrecision"
            //});
            //// 单价
            //list.Add(new DecimalControlField
            //{
            //    ByDecimalControlFieldName = "FTAXPRICE",
            //    DecimalControlFieldName = "FPRICEDIGITS"
            //});
            //// 金额
            //list.Add(new DecimalControlField
            //{
            //    ByDecimalControlFieldName = "FALLAMOUNT",
            //    DecimalControlFieldName = "FAMOUNTDIGITS"
            //});
            //this.ReportProperty.DecimalControlFieldList = list;
        }
    }
}
