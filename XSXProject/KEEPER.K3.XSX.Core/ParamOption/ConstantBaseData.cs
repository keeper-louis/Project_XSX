using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEEPER.K3.XSX.Core.ParamOption
{
    /// <summary>
    /// 常量基础资料信息描述
    /// </summary>
    public class ConstantBaseData
    {
        //客户类别
        public static string YQYMDNO = "MD01";//有区域门店
        public static string WQYMDNO = "MD03";//无区域门店
        public static string JXMDNO = "MD02";//吉祥门店
        public static string QYMDNO = "QY";//区域代理商门店
        public static string QTMDNO = "QT";//其他门店

        //组织类型
        public static string BrandOrgNo = "PP";//品牌公司编码
        public static string QYOrgNo = "QY";//区域公司编码
        public static string SCOrgNo = "SC";//生产公司编码
        public static long SCOrgId = 113493;//生产公司ID
        public static string YQYOrgNo = "MD01";//有区域加盟门店编码
        public static string WQYOrgNo = "MD03";//无区域加盟门店编码
        public static string JXOrgNo = "MD02";//吉祥门店编码

        //账号用途
        public static string PPGSQYJMF = "01"; //品牌公司区域加盟费
        public static string PPGSQYBZJ = "02";//品牌公司区域加盟保证金
        public static string PPGSMDJMF = "03";//品牌公司门店加盟费
        public static string PPGSMDJMBZJ = "04";//品牌公司门店加盟保证金
        public static string SCGSQYDHBZJ = "05";//生产公司区域订货保证金
        public static string SCGSMDDHHK = "06";//生产公司门店订货货款
        public static string SCGSMDYJF = "07";//生产公司门店营建费

        //收付款用途 
        public static long MDJMFID = 113486;//门店加盟费
        public static long MDJMBZJID = 113487;//门店加盟保证金
        public static long YJFID = 113488;//营建费
        public static long QYJMFID = 107586;//区域加盟费
        public static long QYJMBZJID = 107587;//区域加盟保证金
        public static long QYDHBZJID = 107608;//区域订货保证金
        public static long MDDHHKID = 107610;//门店订货货款

        //业务类型-账套1114版
        //public static long QYDHID = 120076;//区域订货
        //public static long YQYMDDHID = 120077; //门店订货(有区域)
        //public static long WQYMDDHID = 120078;//门店订货(无区域)
        //public static long MDYJID = 120079;//门店营建
        //业务类型-正式环境版
        public static long QYDHID = 124355;//区域订货
        public static long YQYMDDHID = 124356; //门店订货(有区域)
        public static long WQYMDDHID = 124357;//门店订货(无区域)
        public static long MDYJID = 124358;//门店营建




    }
}
