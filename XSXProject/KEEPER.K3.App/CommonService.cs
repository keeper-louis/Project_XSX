using KEEPER.K3.XSX.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KEEPER.K3.XSX.Core.ParamOption;
using Kingdee.BOS;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.App;
using Kingdee.BOS.Contracts;
using Kingdee.BOS.ServiceHelper;
using Kingdee.BOS.Orm;
using Kingdee.BOS.Log;
using Kingdee.BOS.App.Data;

namespace KEEPER.K3.App
{
    public class CommonService : ICommonService
    {
       

        public DynamicObject GetBasicObject(Context ctx, string formID,long ObjectID)
        {
            IMetaDataService metaService = ServiceHelper.GetService<IMetaDataService>();//元数据服务
            IViewService view = ServiceHelper.GetService<IViewService>();//界面服务
            FormMetadata Meta = metaService.Load(ctx, formID) as FormMetadata;//获取基础资料元数据
            DynamicObject BasicObject = view.LoadSingle(ctx, ObjectID, Meta.BusinessInfo.GetDynamicObjectType());
            return BasicObject;
        }

        public IOperationResult SubmitBill(Context ctx, string formID, object[] ids)
        {
            IMetaDataService metaService = ServiceHelper.GetService<IMetaDataService>();//元数据服务
            FormMetadata Meta = metaService.Load(ctx, formID) as FormMetadata;//获取元数据
            OperateOption submitOption = OperateOption.Create();
            IOperationResult submitResult = BusinessDataServiceHelper.Submit(ctx, Meta.BusinessInfo, ids, "Submit", submitOption);
            return submitResult;
        }

        public IOperationResult AuditBill(Context ctx, string FormID, object[] ids)
        {
            IMetaDataService metaService = ServiceHelper.GetService<IMetaDataService>();//元数据服务
            FormMetadata Meta = metaService.Load(ctx, FormID) as FormMetadata;//获取元数据
            OperateOption AuditOption = OperateOption.Create();
            IOperationResult AuditResult = BusinessDataServiceHelper.Audit(ctx, Meta.BusinessInfo, ids, AuditOption);
            return AuditResult;
        }
        public void Log(Context ctx,string Operation, IOperationResult returnResult)
        {
            string strSql = string.Empty;
            try
            {
                if (returnResult.IsSuccess)
                {                    
                    strSql = string.Format(@"/*dialect*/INSERT INTO KEEPER_LOG VALUES('{0}','{1}','{2}','{3}')",DateTime.Now,returnResult.OperateResult[0].Number,Operation,returnResult.OperateResult[0].Message);
                    DBUtils.Execute(ctx, strSql);
                }
            }
            catch (Exception e)
            {               
                Logger.Error(Operation, "日志记录失败"+ strSql, e);
            }
            
            

        }

        
    }
}
