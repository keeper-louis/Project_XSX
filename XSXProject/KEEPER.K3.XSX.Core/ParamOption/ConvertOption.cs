using Kingdee.BOS.Core.List;
using Kingdee.BOS.Orm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEEPER.K3.XSX.Core.ParamOption
{
    /// <summary>
    /// 后台调用单据转换参数
    /// </summary>
    public class ConvertOption
    {
        /// <summary>
        /// 源单标识
        /// </summary>
        public string SourceFormId { get; set; }

        /// <summary>
        /// 目标单标识
        /// </summary>
        public string TargetFormId { get; set; }

        /// <summary>
        /// 单据转换规则KEY
        /// </summary>
        public string ConvertRuleKey { get; set; }

        /// <summary>
        /// 选中的源单行
        /// </summary>
        public ListSelectedRow[] BizSelectRows { get; set; }

        /// <summary>
        /// 目标单单据类型
        /// </summary>
        public string TargetBillTypeId { get; set; }

        /// <summary>
        /// 转换参数
        /// </summary>
        public OperateOption Option { get; set; }
    }
}
