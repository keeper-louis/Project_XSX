using KEEPER.K3.XSX.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using KEEPER.K3.XSX.Core.ParamOption;

namespace KEEPER.K3.App
{
    public class OrderRequestService : IOrderRequestService
    {
        /// <summary>
        /// 获取库存可发量
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="stockOrgId">库存组织ID</param>
        /// <param name="masterId">物料MASTERID</param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        public double GetKFQty(Context ctx, long stockOrgId, long masterId, long custID)
        {
            double kcQty = 0;
            //根据物料MASTERID，库存组织ID 库存量基本单位-锁库量基本单位
            string strSql = string.Format(@"/*dialect*/select SUM(FBaseQty)-SUM(FBaseLockQty) STOCKKFQTY from T_STK_INVENTORY WHERE FSTOCKORGID = {0} and FMATERIALID  = {1}", stockOrgId, masterId);
            //根据MASTERID，获取单据换算率
            string CONVERTRATESql = string.Format(@"/*dialect*/select FCONVERTNUMERATOR from T_BD_UNITCONVERTRATE where FMATERIALID = {0}",masterId);
            //根据客户ID获取已审核要货申请单的待发量
            string reqBillQtySql = string.Format(@"select SUM(T_SCMS_ApplyGoolsEntry.FREQQTY) -
       SUM(T_SCMS_ApplyGoolsEntry.FSALQTY) FQTY
  from T_SCMS_ApplyGools
 inner join T_SCMS_ApplyGoolsEntry
    ON T_SCMS_ApplyGools.FID = T_SCMS_ApplyGoolsEntry.FID
 where T_SCMS_ApplyGools.FDOCUMENTSTATUS = 'C'
   AND T_SCMS_ApplyGools.FAPPLYCUST = {0}
   AND T_SCMS_ApplyGools.FYWTYPE = {1}", custID, ConstantBaseData.YQYMDDHID);
            kcQty = Convert.ToDouble(DBUtils.ExecuteScalar<double>(ctx, strSql, 0, null));
            if (kcQty > 0)
            {
               double ConvertRate = DBUtils.ExecuteScalar<double>(ctx, CONVERTRATESql, 1, null);
               kcQty = kcQty / ConvertRate;
               double reqBillQty =  DBUtils.ExecuteScalar<double>(ctx, reqBillQtySql, 0, null);
               kcQty = kcQty - reqBillQty;
               return kcQty;
            }
            else
            {
                return 0; 
            }
            
        }

        /// <summary>
        /// 获取门店客户要货可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID">门店客户Id
        /// </param>
        /// <returns></returns>
        public double GetMDAmount(Context ctx, string custNo,long custID)
        {
            string actualAmountSql = string.Format(@"/*dialect*/select sum(arentry.FREALRECAMOUNTFOR) FREALRECAMOUNTFOR
  from T_AR_RECEIVEBILL ar
 inner join T_AR_RECEIVEBILLENTRY arentry
    on ar.FID = arentry.FID
   and ar.FDOCUMENTSTATUS = 'C'
 where arentry.FPURPOSEID = {0}
   and ar.FCONTACTUNIT IN (SELECT FCUSTID FROM T_BD_CUSTOMER WHERE FNUMBER ='{1}')
   and ar.FORGTYPE = {2}", ConstantBaseData.MDDHHKID, custNo, ConstantBaseData.SCOrgId);
            //要货申请金额-已配送金额
            //已审核的要货申请单
            string actualSalAmountSql = string.Format(@"/*dialect*/select SUM(T_SCMS_ApplyGoolsEntry.FREQAMOUNT) -
       SUM(T_SCMS_ApplyGoolsEntry.FALLAMOUNT_LC) FBillAllAmount
  from T_SCMS_ApplyGools
 inner join T_SCMS_ApplyGoolsEntry
    ON T_SCMS_ApplyGools.FID = T_SCMS_ApplyGoolsEntry.FID
 where T_SCMS_ApplyGools.FDOCUMENTSTATUS = 'C'
   AND T_SCMS_ApplyGools.FAPPLYCUST = {0}
   AND T_SCMS_ApplyGools.FYWTYPE IN ({1}, {2})
", custID, ConstantBaseData.YQYMDDHID, ConstantBaseData.WQYMDDHID);
            double actualAmount = DBUtils.ExecuteScalar<double>(ctx, actualAmountSql, 0, null);
            double actualSalAmount = DBUtils.ExecuteScalar<double>(ctx, actualSalAmountSql, 0, null);
            return actualAmount - actualSalAmount;
        }

        /// <summary>
        /// 获取区域客户要货申请可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custNo">区域申请客户编码</param>
        /// <returns></returns>
        public double GetQYAmount(Context ctx, string custNo)
        {
            string noHxSql = string.Format(@"/*dialect*/select sum(arentry.FREALRECAMOUNTFOR) - sum(FWRITTENOFFAMOUNTFOR) FORDERAMOUNT
  from T_AR_RECEIVEBILL ar
 inner join T_AR_RECEIVEBILLENTRY arentry
    on ar.FID = arentry.FID
   and ar.FWRITTENOFFSTATUS <> 'C'
   and ar.FDOCUMENTSTATUS = 'C'
   and ar.FYWTYPE = {3}
 where arentry.FPURPOSEID = {0}
   and ar.FCONTACTUNIT IN (SELECT FCUSTID FROM T_BD_CUSTOMER WHERE FNUMBER ='{1}')
   and ar.FORGTYPE = {2}", ConstantBaseData.QYDHBZJID, custNo, ConstantBaseData.SCOrgId, ConstantBaseData.QYDHID);
            double OrderAmount = DBUtils.ExecuteScalar<double>(ctx, noHxSql, 0, null);
            return OrderAmount;
        }

        /// <summary>
        /// 获取营建门店客户要货可用额度
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custNo"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public double GetYJAmount(Context ctx, string custNo, long custID)
        {
            string actualAmountSql = string.Format(@"/*dialect*/select sum(arentry.FREALRECAMOUNTFOR) FREALRECAMOUNTFOR
  from T_AR_RECEIVEBILL ar
 inner join T_AR_RECEIVEBILLENTRY arentry
    on ar.FID = arentry.FID
   and ar.FDOCUMENTSTATUS = 'C'
 where arentry.FPURPOSEID = {0}
   and ar.FCONTACTUNIT IN (SELECT FCUSTID FROM T_BD_CUSTOMER WHERE FNUMBER ='{1}')
   and ar.FORGTYPE = {2}", ConstantBaseData.YJFID, custNo, ConstantBaseData.SCOrgId);
            //要货申请金额-已配送金额
            //已审核的要货申请单
            string actualSalAmountSql = string.Format(@"/*dialect*/select SUM(T_SCMS_ApplyGoolsEntry.FREQAMOUNT) -
       SUM(T_SCMS_ApplyGoolsEntry.FALLAMOUNT_LC) FBillAllAmount
  from T_SCMS_ApplyGools
 inner join T_SCMS_ApplyGoolsEntry
    ON T_SCMS_ApplyGools.FID = T_SCMS_ApplyGoolsEntry.FID
 where T_SCMS_ApplyGools.FDOCUMENTSTATUS = 'C'
   AND T_SCMS_ApplyGools.FAPPLYCUST = {0}
   AND T_SCMS_ApplyGools.FYWTYPE = {1}", custID,ConstantBaseData.MDYJID);
            double actualAmount = DBUtils.ExecuteScalar<double>(ctx, actualAmountSql, 0, null);
            double actualSalAmount = DBUtils.ExecuteScalar<double>(ctx, actualSalAmountSql, 0, null);
            return actualAmount - actualSalAmount;
        }
    }
}
