using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEEPER.K3.XSX.Core.Entity
{
    /// <summary>
    /// 客户
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        public long custID { get; set; }
        /// <summary>
        /// 所属区域ID
        /// </summary>
        public long BelongCustID { get; set; }
        /// <summary>
        /// 所属区域编码
        /// </summary>
        public string BelongCustNo { get; set; }
        /// <summary>
        /// 客户类型ID
        /// </summary>
        public long CustTypeID { get; set; }
        /// <summary>
        /// 客户类型编码
        /// </summary>
        public string FCustTypeNo { get; set; }
        /// <summary>
        /// 额外提点
        /// </summary>
        public double Proportions { get; set; }
    }
}
