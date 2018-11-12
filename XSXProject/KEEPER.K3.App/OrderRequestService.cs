using KEEPER.K3.XSX.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS;
using Kingdee.BOS.App.Data;

namespace KEEPER.K3.App
{
    public class OrderRequestService : IOrderRequestService
    {
        /// <summary>
        /// 获取门店客户要货可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID">门店客户Id
        /// </param>
        /// <returns></returns>
        public double GetMDAmount(Context ctx, long custID)
        {
            //收款用途：订货货款：No,SFKYT003 Id:108874 *************按照实际情况进行修改***********
            //组织类型：生产：No，SC，ID：108867 *************按照实际情况进行修改***********
            string actualAmountSql = string.Format(@"/*dialect*/select sum(arentry.FREALRECAMOUNTFOR) FREALRECAMOUNTFOR
  from T_AR_RECEIVEBILL ar
 inner join T_AR_RECEIVEBILLENTRY arentry
    on ar.FID = arentry.FID
   and ar.FDOCUMENTSTATUS = 'C'
 where arentry.FPURPOSEID = {0}
   and ar.FCONTACTUNIT = {1}
   and ar.FORGTYPE = {2}", 108874, custID, 108867);
            string actualSalAmountSql = string.Format(@"/*dialect*/select SUM(SALFIN.FBillAllAmount) FBillAllAmount
  from T_SAL_OUTSTOCK SALOUT
 inner join T_SAL_OUTSTOCKFIN SALFIN
    on SALOUT.FID = SALFIN.FID
 WHERE SALOUT.FDOCUMENTSTATUS = 'C'
   AND SALOUT.FBILLTYPEID = 'ee5d5daffb2b424a9fa831a46fac2d5a'
   AND SALOUT.FCUSTOMERID = {0}", custID);
            double actualAmount = DBUtils.ExecuteScalar<double>(ctx, actualAmountSql, 0, null);
            double actualSalAmount = DBUtils.ExecuteScalar<double>(ctx, actualSalAmountSql, 0, null);
            return actualAmount - actualSalAmount;
        }

        /// <summary>
        /// 获取区域客户要货申请可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID">区域申请客户ID</param>
        /// <returns></returns>
        public double GetQYAmount(Context ctx, long custID)
        {
            //收款用途：订货保证金：No,SFKYT004 Id:108875 *************按照实际情况进行修改***********
            //组织类型：生产：No，SC，ID：108867 *************按照实际情况进行修改***********
            string noHxSql = string.Format(@"/*dialect*/select sum(arentry.FREALRECAMOUNTFOR) - sum(FWRITTENOFFAMOUNTFOR) FORDERAMOUNT
  from T_AR_RECEIVEBILL ar
 inner join T_AR_RECEIVEBILLENTRY arentry
    on ar.FID = arentry.FID
   and ar.FWRITTENOFFSTATUS <> 'C'
   and ar.FDOCUMENTSTATUS = 'C'
 where arentry.FPURPOSEID = {0}
   and ar.FCONTACTUNIT = {1}
   and ar.FORGTYPE = {2}", 108875, custID, 108867);
            double OrderAmount = DBUtils.ExecuteScalar<double>(ctx, noHxSql, 0, null);
            return OrderAmount;
        }
    }
}
