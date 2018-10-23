using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEEPER.K3.Web.Core.Model
{
    /// <summary>
    /// 返回结果
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    [Serializable]
    public class ReturnInfo<T>
    {
        //输入ctor+2次tab可快速构建构造方法
        public ReturnInfo()
        {

        }
        public ReturnInfo(T t)
        {
            this.ReturnValue = t;
        }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public T ReturnValue { get; set; }
    }
}
