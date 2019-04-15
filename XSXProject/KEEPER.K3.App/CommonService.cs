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
using Kingdee.BOS.Core.Bill;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.Metadata.FormElement;
using Kingdee.BOS.Core;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;

namespace KEEPER.K3.App
{
    public class CommonService : ICommonService
    {
       
        /// <summary>
        /// 通过业务对象标识和业务对象ID,构建基础资料对象
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="formID">业务对象标识</param>
        /// <param name="ObjectID">业务对象ID</param>
        /// <returns></returns>
        public DynamicObject GetBasicObject(Context ctx, string formID,long ObjectID)
        {
            IMetaDataService metaService = ServiceHelper.GetService<IMetaDataService>();//元数据服务
            IViewService view = ServiceHelper.GetService<IViewService>();//界面服务
            FormMetadata Meta = metaService.Load(ctx, formID) as FormMetadata;//获取基础资料元数据
            DynamicObject BasicObject = view.LoadSingle(ctx, ObjectID, Meta.BusinessInfo.GetDynamicObjectType());
            return BasicObject;
        }

        /// <summary>
        /// 通过业务对象标识和业务对象id数据，批量进行业务对象提交
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="formID">业务对象标识</param>
        /// <param name="ids">业务对象id数据</param>
        /// <returns></returns>
        public IOperationResult SubmitBill(Context ctx, string formID, object[] ids)
        {
            IMetaDataService metaService = ServiceHelper.GetService<IMetaDataService>();//元数据服务
            FormMetadata Meta = metaService.Load(ctx, formID) as FormMetadata;//获取元数据
            OperateOption submitOption = OperateOption.Create();
            IOperationResult submitResult = BusinessDataServiceHelper.Submit(ctx, Meta.BusinessInfo, ids, "Submit", submitOption);
            return submitResult;
        }

        /// <summary>
        /// 通过业务对象标识和业务对象id数据，进行业务单据批量审核
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID">业务对象标识</param>
        /// <param name="ids">业务对象id数组</param>
        /// <returns></returns>
        public IOperationResult AuditBill(Context ctx, string FormID, object[] ids)
        {
            IMetaDataService metaService = ServiceHelper.GetService<IMetaDataService>();//元数据服务
            FormMetadata Meta = metaService.Load(ctx, FormID) as FormMetadata;//获取元数据
            OperateOption AuditOption = OperateOption.Create();
            IOperationResult AuditResult = BusinessDataServiceHelper.Audit(ctx, Meta.BusinessInfo, ids, AuditOption);
            return AuditResult;
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="Operation"></param>
        /// <param name="returnResult"></param>
        public void Log(Context ctx,string Operation, IOperationResult returnResult)
        {
            string strSql = string.Empty;
            try
            {
                                   
                    strSql = string.Format(@"/*dialect*/INSERT INTO KEEPER_LOG VALUES('{0}','{1}','{2}','{3}')",DateTime.Now,returnResult.OperateResult[0].Number,Operation,returnResult.OperateResult[0].Message);
                    DBUtils.Execute(ctx, strSql);
                
            }
            catch (Exception e)
            {               
                Logger.Error(Operation, "日志记录失败"+ strSql, e);
            }
            
            

        }

        /// <summary>
        /// 通过业务对象标识和单据DynamicObject数据包对单据进行保存操作
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID"></param>
        /// <param name="dyObject"></param>
        /// <returns></returns>
        public IOperationResult SaveBill(Context ctx, string FormID,DynamicObject dyObject)
        {
            IMetaDataService metaService = ServiceHelper.GetService<IMetaDataService>();//元数据服务
            FormMetadata Meta = metaService.Load(ctx, FormID) as FormMetadata;//获取元数据
            OperateOption SaveOption = OperateOption.Create();
            IOperationResult SaveResult = BusinessDataServiceHelper.Save(ctx, Meta.BusinessInfo, dyObject, SaveOption, "Save");
            return SaveResult;
        }

        /// <summary>
        /// 打包单据数据结构，录入各自单据需要的字段，可组装成各业务对象单据的数据包
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="FormID"></param>
        /// <param name="fillBillPropertys"></param>
        /// <param name="BillTypeId"></param>
        /// <returns></returns>
        public DynamicObject installCostRequestPackage(Context ctx, string FormID, Action<IDynamicFormViewService> fillBillPropertys, string BillTypeId="")
        {
            //IMetaDataService metaService = ServiceHelper.GetService<IMetaDataService>();//元数据服务
            //FormMetadata Meta = metaService.Load(ctx, FormID) as FormMetadata;//获取元数据
            FormMetadata Meta = MetaDataServiceHelper.Load(ctx, FormID) as FormMetadata;//获取元数据
            Form form = Meta.BusinessInfo.GetForm();
            IDynamicFormViewService dynamicFormViewService = (IDynamicFormViewService)Activator.CreateInstance(Type.GetType("Kingdee.BOS.Web.Import.ImportBillView,Kingdee.BOS.Web"));
            // 创建视图加载参数对象，指定各种参数，如FormId, 视图(LayoutId)等
            BillOpenParameter openParam = new BillOpenParameter(form.Id, Meta.GetLayoutInfo().Id);
            openParam.Context = ctx;
            openParam.ServiceName = form.FormServiceName;
            openParam.PageId = Guid.NewGuid().ToString();
            openParam.FormMetaData = Meta;
            openParam.Status = OperationStatus.ADDNEW;
            openParam.CreateFrom = CreateFrom.Default;
            // 单据类型
            openParam.DefaultBillTypeId = BillTypeId;
            openParam.SetCustomParameter("ShowConfirmDialogWhenChangeOrg", false);
            // 插件
            List<AbstractDynamicFormPlugIn> plugs = form.CreateFormPlugIns();
            openParam.SetCustomParameter(FormConst.PlugIns, plugs);
            PreOpenFormEventArgs args = new PreOpenFormEventArgs(ctx, openParam);
            foreach (var plug in plugs)
            {
                plug.PreOpenForm(args);
            }
            // 动态领域模型服务提供类，通过此类，构建MVC实例
            IResourceServiceProvider provider = form.GetFormServiceProvider(false);

            dynamicFormViewService.Initialize(openParam, provider);
            IBillView billView = dynamicFormViewService as IBillView;
            ((IBillViewService)billView).LoadData();

            // 触发插件的OnLoad事件：
            // 组织控制基类插件，在OnLoad事件中，对主业务组织改变是否提示选项进行初始化。
            // 如果不触发OnLoad事件，会导致主业务组织赋值不成功
            DynamicFormViewPlugInProxy eventProxy = billView.GetService<DynamicFormViewPlugInProxy>();
            eventProxy.FireOnLoad();
            if (fillBillPropertys!=null)
            {
                fillBillPropertys(dynamicFormViewService);
            }
            // 设置FormId
            form = billView.BillBusinessInfo.GetForm();
            if (form.FormIdDynamicProperty != null)
            {
                form.FormIdDynamicProperty.SetValue(billView.Model.DataObject, form.Id);
            }
            return billView.Model.DataObject;
        }

        
    }
}
