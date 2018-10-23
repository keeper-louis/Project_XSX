using Kingdee.BOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEEPER.K3.XSX.Contracts
{
    /// <summary>
    /// 服务工厂
    /// </summary>
    public class XSXServiceFactory
    {
        public static ServicesContainer _mapServer = new ServicesContainer();
        static bool noRegistered = true;

        static XSXServiceFactory()
        {
            RegisterService();

        }

        /// <summary>
        /// 获及服务实例
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <param name="ctx">上下文</param>
        /// <returns>服务实例</returns>
        public static T GetService<T>(Context ctx)
        {
            return GetService<T>(ctx, ctx.ServerUrl);
        }

        static T GetService<T>(Context ctx, string url)
        {
            if (ctx == null)
            {
                throw new Exception("{ctx == null}");
            }
            if (noRegistered)
            {

                RegisterService();
            }
            var instance = _mapServer.GetService<T>(typeof(T), url);
            if (instance == null)
            {
                throw new Exception("获取服务对象失败，请检查己映射该对象!");
            }
            return instance;
        }

        public static void RegisterService()
        {
            if (!noRegistered) return;


            ////增加对应的接口与实现类的对应关系

            _mapServer.Add(typeof(ICommonService), "LM.K3.App.CommonService,LM.K3.App");

            //这句话放到最后
            noRegistered = false;
        }
        public static void CloseService(object service)
        {
            IDisposable disposable = service as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public static ICommonService GetCommonService(Context ctx)
        {
            return GetService<ICommonService>(ctx);
        }
        
    }
}
