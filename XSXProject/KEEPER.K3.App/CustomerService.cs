using KEEPER.K3.XSX.Contracts;
using KEEPER.K3.XSX.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using System.Data;
using KEEPER.K3.XSX.Core.ParamOption;
using Kingdee.BOS.Orm.DataEntity;

namespace KEEPER.K3.App
{
    public class CustomerService : ICustomerService
    {
        /// <summary>
        /// 获取有区域门店属性
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        public Customer GetCustomerProperty(Context ctx, long custID)
        {
            //FCUSTTYPEID = 81f12c96310b47b0b9e1b5f7298250b8 客户类别内部结算客户
            Customer cust = new Customer();
            string strSql = string.Format(@"SELECT cust.fcustid,
       cust.fnumber      custNo,
       cust.FBELONGCUST  FBELONGCUST,
       ccust.FNUMBER     ccustFNUMBER,
       cust.FCUSTTYPE    FCUSTTYPE,
       custtype.FNUMBER  custtypeFFNUMBER,
       cust.FPROPORTIONS FPROPORTIONS,
	   ORG.FORGID        FORGID,
	   ORG.FNUMBER       FORGNO,
	   REGION.FID        FREGIONID,
	   REGION.FNUMBER    FREGIONNO
  FROM T_BD_CUSTOMER cust
 INNER JOIN T_KEEPER_CUSTTYPE custtype
    ON cust.FCUSTTYPE = custtype.FID
 LEFT JOIN T_ORG_ORGANIZATIONS ORG
    ON cust.FSHBRAND = ORG.FORGID
 LEFT JOIN PAEZ_REGION REGION
   ON cust.FSHREGION = REGION.FID
 INNER JOIN T_BD_CUSTOMER ccust
    ON ccust.FCUSTID = cust.FBELONGCUST
 where cust.fcustid = {0}
   and cust.FCUSTTYPEID = '81f12c96310b47b0b9e1b5f7298250b8'", custID);
            using (IDataReader reader =  DBUtils.ExecuteReader(ctx,strSql))
            {
                while (reader.Read())
                {
                    cust.custID = Convert.ToInt64(reader["fcustid"]);//客户ID
                    cust.custNo = Convert.ToString(reader["custNo"]);//客户编码
                    cust.BelongCustID = Convert.ToInt64(reader["FBELONGCUST"]);//所属区域ID
                    cust.BelongCustNo = Convert.ToString(reader["ccustFNUMBER"]);//所属区域编码
                    cust.CustTypeID = Convert.ToInt64(reader["FCUSTTYPE"]);//客户类别ID
                    cust.FCustTypeNo = Convert.ToString(reader["custtypeFFNUMBER"]);//客户类别编码
                    cust.Proportions = Convert.ToDouble(reader["FPROPORTIONS"]);//额外提点
                    cust.BrandID = Convert.ToInt64(reader["FORGID"]);//所属品牌ID
                    cust.BrandNo = Convert.ToString(reader["FORGNO"]);//所属品牌编码
                    cust.RegionID = Convert.ToInt64(reader["FREGIONID"]);//所属大区ID
                    cust.RegionNo = Convert.ToString(reader["FREGIONNO"]);//所属大区编码
                }
                reader.Close();
            }
                return cust;
        }

        /// <summary>
        /// 通过组织内码和客户对应组织进行对应，查询客户属性
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="orgID">客户对应组织</param>
        /// <returns></returns>
        public Customer GetIntCustomerProperty(Context ctx, long orgID)
        {
            Customer cust = null;
            string strSql = string.Format(@"/*dialect*/SELECT FCUSTID CUSTID, CUSTTYPE.FNUMBER CUSTTYPENO
  FROM T_BD_CUSTOMER CUST
 INNER JOIN T_KEEPER_CUSTTYPE CUSTTYPE
    ON CUST.FCUSTTYPE = CUSTTYPE.FID
 WHERE FCORRESPONDORGID = {0}
   and FUSEORGID = {1}", orgID,orgID);
            DynamicObjectCollection reader = DBUtils.ExecuteDynamicObject(ctx, strSql);
            foreach (DynamicObject item in reader)
            {
                if (Convert.ToString(item["CUSTTYPENO"]).Equals(ConstantBaseData.YQYMDNO) || Convert.ToString(item["CUSTTYPENO"]).Equals(ConstantBaseData.QYMDNO))
                {
                    cust = GetCustomerProperty(ctx, Convert.ToInt64(item["CUSTID"]));
                }
                else if (Convert.ToString(item["CUSTTYPENO"]).Equals(ConstantBaseData.WQYMDNO))
                {
                    cust = GetWQYCustomerProperty(ctx, Convert.ToInt64(item["CUSTID"]));
                }
                else
                {
                    cust = null;
                }
            }
            return cust;
            #region 忽略
            //using (IDataReader reader = DBUtils.ExecuteReader(ctx, strSql))
            //{
            //    while (reader.Read())
            //    {
            //        if (Convert.ToString(reader["CUSTTYPENO"]).Equals(ConstantBaseData.YQYMDNO)|| Convert.ToString(reader["CUSTTYPENO"]).Equals(ConstantBaseData.QYMDNO))
            //        {
            //            cust = GetCustomerProperty(ctx, Convert.ToInt64(reader["CUSTID"]));
            //        }
            //        else if (Convert.ToString(reader["CUSTTYPENO"]).Equals(ConstantBaseData.WQYMDNO))
            //        {
            //            cust = GetWQYCustomerProperty(ctx, Convert.ToInt64(reader["CUSTID"]));
            //        }
            //        else
            //        {
            //            cust = null;
            //        }

            //    }
            //    reader.Close();
            //    return cust;
            //}
            #endregion
        }
        /// <summary>
        /// 通过账号用途获取收付款用途ID
        /// </summary>
        /// <param name="AccountUsage">账号用途编码</param>
        /// <returns></returns>
        public long GetSFKYT(Context ctx,string AccountUsage)
        {
            if (AccountUsage.Equals(ConstantBaseData.PPGSQYJMF))//品牌公司区域加盟费
            {
                return ConstantBaseData.QYJMFID;
            }
            else if (AccountUsage.Equals(ConstantBaseData.PPGSQYBZJ))//品牌公司区域加盟保证金
            {
                return ConstantBaseData.QYJMBZJID;
            }
            else if (AccountUsage.Equals(ConstantBaseData.PPGSMDJMF))//品牌公司门店加盟费
            {
                return ConstantBaseData.MDJMFID;
            }
            else if (AccountUsage.Equals(ConstantBaseData.PPGSMDJMBZJ))//品牌公司门店加盟保证金
            {
                return ConstantBaseData.MDJMBZJID;
            }
            else if (AccountUsage.Equals(ConstantBaseData.SCGSQYDHBZJ))//生产公司区域订货保证金
            {
                return ConstantBaseData.QYDHBZJID;
            }
            else if (AccountUsage.Equals(ConstantBaseData.SCGSMDDHHK))//生产公司门店订货货款
            {
                return ConstantBaseData.MDDHHKID;
            }
            else if (AccountUsage.Equals(ConstantBaseData.SCGSMDYJF))//生产公司门店营建费
            {
                return ConstantBaseData.YJFID;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 通过账号用途获取业务类型ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AccountUsage"></param>
        /// <returns></returns>
        public long GetYWTYPE(Context ctx, string AccountUsage, long custID)
        {
            if (IsWQYCust(ctx,custID))
            {
                if (AccountUsage.Equals(ConstantBaseData.PPGSQYJMF))//品牌公司区域加盟费
                {
                    return 0;
                }
                else if (AccountUsage.Equals(ConstantBaseData.PPGSQYBZJ))//品牌公司区域加盟保证金
                {
                    return 0;
                }
                else if (AccountUsage.Equals(ConstantBaseData.PPGSMDJMF))//品牌公司门店加盟费
                {
                    return 0;
                }
                else if (AccountUsage.Equals(ConstantBaseData.PPGSMDJMBZJ))//品牌公司门店加盟保证金
                {
                    return 0;
                }
                else if (AccountUsage.Equals(ConstantBaseData.SCGSQYDHBZJ))//生产公司区域订货保证金
                {
                    return ConstantBaseData.QYDHID;
                }
                else if (AccountUsage.Equals(ConstantBaseData.SCGSMDDHHK))//生产公司门店订货货款
                {
                    return ConstantBaseData.WQYMDDHID;
                }
                else if (AccountUsage.Equals(ConstantBaseData.SCGSMDYJF))//生产公司门店营建费
                {
                    return ConstantBaseData.MDYJID;
                }
                else
                {
                    return 0;
                }
            }
            else if (IsYQYCust(ctx,custID))
            {
                if (AccountUsage.Equals(ConstantBaseData.PPGSQYJMF))//品牌公司区域加盟费
                {
                    return 0;
                }
                else if (AccountUsage.Equals(ConstantBaseData.PPGSQYBZJ))//品牌公司区域加盟保证金
                {
                    return 0;
                }
                else if (AccountUsage.Equals(ConstantBaseData.PPGSMDJMF))//品牌公司门店加盟费
                {
                    return 0;
                }
                else if (AccountUsage.Equals(ConstantBaseData.PPGSMDJMBZJ))//品牌公司门店加盟保证金
                {
                    return 0;
                }
                else if (AccountUsage.Equals(ConstantBaseData.SCGSQYDHBZJ))//生产公司区域订货保证金
                {
                    return ConstantBaseData.QYDHID;
                }
                else if (AccountUsage.Equals(ConstantBaseData.SCGSMDDHHK))//生产公司门店订货货款
                {
                    return ConstantBaseData.YQYMDDHID;
                }
                else if (AccountUsage.Equals(ConstantBaseData.SCGSMDYJF))//生产公司门店营建费
                {
                    return ConstantBaseData.MDYJID;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
            
        }

        /// <summary>
        /// 获取无区域门店属性
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        public Customer GetWQYCustomerProperty(Context ctx, long custID)
        {
            Customer cust = new Customer();
            string strSql = string.Format(@"SELECT cust.fcustid,
       cust.fnumber      custNo,
       cust.FCUSTTYPE    FCUSTTYPE,
       custtype.FNUMBER  custtypeFFNUMBER,
       cust.FPROPORTIONS FPROPORTIONS,
       ORG.FORGID        FORGID,
       ORG.FNUMBER       FORGNO,
       REGION.FID        FREGIONID,
       REGION.FNUMBER    FREGIONNO
  FROM T_BD_CUSTOMER cust
 INNER JOIN T_KEEPER_CUSTTYPE custtype
    ON cust.FCUSTTYPE = custtype.FID
  LEFT JOIN T_ORG_ORGANIZATIONS ORG
    ON cust.FSHBRAND = ORG.FORGID
  LEFT JOIN PAEZ_REGION REGION
    ON cust.FSHREGION = REGION.FID
 where cust.fcustid = {0}
   and cust.FCUSTTYPEID = '81f12c96310b47b0b9e1b5f7298250b8'", custID);
            using (IDataReader reader = DBUtils.ExecuteReader(ctx, strSql))
            {
                while (reader.Read())
                {
                    cust.custID = Convert.ToInt64(reader["fcustid"]);//客户ID
                    cust.custNo = Convert.ToString(reader["custNo"]);//客户编码
                    cust.CustTypeID = Convert.ToInt64(reader["FCUSTTYPE"]);//客户类别ID
                    cust.FCustTypeNo = Convert.ToString(reader["custtypeFFNUMBER"]);//客户类别编码
                    cust.Proportions = Convert.ToDouble(reader["FPROPORTIONS"]);//额外提点
                    cust.BrandID = Convert.ToInt64(reader["FORGID"]);//所属品牌ID
                    cust.BrandNo = Convert.ToString(reader["FORGNO"]);//所属品牌编码
                    cust.RegionID = Convert.ToInt64(reader["FREGIONID"]);//所属大区ID
                    cust.RegionNo = Convert.ToString(reader["FREGIONNO"]);//所属大区编码
                }
                reader.Close();
            }
            return cust;
        }

        

        /// <summary>
        /// 判断是否是吉祥门店客户
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public bool IsJXCust(Context ctx, long custID)
        {
            string strSql = string.Format(@"/*dialect*/select custtype.FNUMBER from T_BD_CUSTOMER cust inner join T_KEEPER_CUSTTYPE custtype on cust.FCUSTTYPE = custtype.FID where cust.FCUSTID = {0}", custID);
            string custtype = DBUtils.ExecuteScalar<string>(ctx, strSql, "noResult", null);
            if (custtype.Equals("noResult")||custtype.Equals(ConstantBaseData.JXMDNO))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否是无区域门店
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public bool IsWQYCust(Context ctx, long custID)
        {
            string strSql = string.Format(@"/*dialect*/select custtype.FNUMBER from T_BD_CUSTOMER cust inner join T_KEEPER_CUSTTYPE custtype on cust.FCUSTTYPE = custtype.FID where cust.FCUSTID = {0}", custID);
            string custtype = DBUtils.ExecuteScalar<string>(ctx, strSql, "noResult", null);
            if (custtype.Equals("noResult") || custtype.Equals(ConstantBaseData.WQYMDNO))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否是有区域门店
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID"></param>
        /// <returns></returns>
        public bool IsYQYCust(Context ctx, long custID)
        {
            string strSql = string.Format(@"/*dialect*/select custtype.FNUMBER from T_BD_CUSTOMER cust inner join T_KEEPER_CUSTTYPE custtype on cust.FCUSTTYPE = custtype.FID where cust.FCUSTID = {0}", custID);
            string custtype = DBUtils.ExecuteScalar<string>(ctx, strSql, "noResult", null);
            if (custtype.Equals("noResult") || custtype.Equals(ConstantBaseData.YQYMDNO) || custtype.Equals(ConstantBaseData.QYMDNO))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
