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

namespace KEEPER.K3.App
{
    public class CustomerService : ICustomerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="custID">客户ID</param>
        /// <returns></returns>
        public Customer GetCustomerProperty(Context ctx, long custID)
        {
            //FCUSTTYPEID = 81f12c96310b47b0b9e1b5f7298250b8 客户类别内部结算客户
            Customer cust = new Customer();
            string strSql = string.Format(@"SELECT cust.fcustid,cust.fnumber custNo,cust.FBELONGCUST FBELONGCUST,
       ccust.FNUMBER ccustFNUMBER,
       cust.FCUSTTYPE FCUSTTYPE,
       custtype.FNUMBER custtypeFFNUMBER,
       cust.FPROPORTIONS FPROPORTIONS
  FROM T_BD_CUSTOMER cust
 INNER JOIN T_KEEPER_CUSTTYPE custtype
    ON cust.FCUSTTYPE = custtype.FID
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
                }
            }
                return cust;
        }

        public Customer GetIntCustomerProperty(Context ctx, long orgID)
        {
            string strSql = string.Format(@"/*dialect*/SELECT FCUSTID FROM T_BD_CUSTOMER WHERE FCORRESPONDORGID = {0} and FUSEORGID = {1}", orgID,orgID);
            long custId = DBUtils.ExecuteScalar<long>(ctx, strSql, -1, null);
            if (custId!=-1)
            {
                Customer cust = GetCustomerProperty(ctx, custId);
                return cust;
            }
            else
            {
                return null;
            }
        }
    }
}
