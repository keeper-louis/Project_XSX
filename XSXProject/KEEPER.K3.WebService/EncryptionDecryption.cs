using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEEPER.K3.WebService
{
    /// <summary>
    /// 加解密
    /// </summary>
    public class EncryptionDecryption
    {
        private EncryptionDecryption()
        {

        }

        private static EncryptionDecryption _EncryptionDecryption;

        public static EncryptionDecryption Instance
        {
            get
            {
                if (_EncryptionDecryption == null)
                {
                    _EncryptionDecryption = new EncryptionDecryption();
                }
                return _EncryptionDecryption;
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string EnCode(string str)
        {
            string htext = "";

            for (int i = 0; i < str.Length; i++)
            {
                htext = htext + (char)(str[i] + 10 - 1 * 2);
            }
            return htext;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string DeCode(string str)
        {
            string dtext = "";

            for (int i = 0; i < str.Length; i++)
            {
                dtext = dtext + (char)(str[i] - 10 + 1 * 2);
            }
            return dtext;
        }

        public string BOSEnCode(string str)
        {
            return Convert.ToString(Kingdee.BOS.Util.EncryptDecryptUtil.Encode(str));
        }
        public string BOSDeCode(string str)
        {
            return Convert.ToString(Kingdee.BOS.Util.EncryptDecryptUtil.Decode(str));
        }
    }
}