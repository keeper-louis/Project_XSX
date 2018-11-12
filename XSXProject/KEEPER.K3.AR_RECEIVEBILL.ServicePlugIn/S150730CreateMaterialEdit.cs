using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Kingdee.BOS;
using Kingdee.BOS.Util;
using Kingdee.BOS.Orm;
using Kingdee.BOS.Core;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.FormElement;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Bill;
using Kingdee.BOS.Core.Interaction;
using Kingdee.BOS.ServiceHelper;
using Kingdee.BOS.Orm.DataEntity;

namespace KEEPER.K3.AR_RECEIVEBILL.ServicePlugIn
{
    public class S150730CreateMaterialEdit: AbstractDynamicFormPlugIn
    {
        public override void AfterBarItemClick(AfterBarItemClickEventArgs e)
        {
            if (e.BarItemKey.EqualsIgnoreCase("tbImportMaterial"))
            {
                this.ImportMaterial();
            }
        }
        /// <summary>
        /// 读取第三方物料信息，存入K/3 Cloud物料
        /// </summary>
        private void ImportMaterial()
        {
            // 构建一个IBillView实例，通过此实例，可以方便的填写物料各属性
            IBillView billView = this.CreateMaterialView();
            // 新建一个空白物料
            // billView.CreateNewModelData();
            ((IBillViewService)billView).LoadData();


            // 触发插件的OnLoad事件：
            // 组织控制基类插件，在OnLoad事件中，对主业务组织改变是否提示选项进行初始化。
            // 如果不触发OnLoad事件，会导致主业务组织赋值不成功
            DynamicFormViewPlugInProxy eventProxy = billView.GetService<DynamicFormViewPlugInProxy>();
            eventProxy.FireOnLoad();


            // 记载指定的单据进行修改
            // this.ModifyBill(billView, "100001"); // 本代码演示新建物料，把调用修改的代码，注释掉

            // 填写物料各属性
            this.FillMaterialPropertys(billView);
            // 保存物料
            OperateOption saveOption = OperateOption.Create();
            this.SaveMaterial(billView, saveOption);
        }
        /// <summary>
        /// 把物料的各属性，填写到IBillView当前所管理的物料中
        /// </summary>
        /// <param name="billView"></param>
        private void FillMaterialPropertys(IBillView billView)
        {
            // 把billView转换为IDynamicFormViewService接口：
            // 调用IDynamicFormViewService.UpdateValue: 会执行字段的值更新事件
            // 调用 dynamicFormView.SetItemValueByNumber ：不会执行值更新事件，需要继续调用：
            // ((IDynamicFormView)dynamicFormView).InvokeFieldUpdateService(key, rowIndex);
            IDynamicFormViewService dynamicFormView = billView as IDynamicFormViewService;
            /********************物料页签上的字段******************/
            // 创建组织、使用组织 : 
            // 基础资料字段，用编码录入 (SetItemValueByNumber)
            // 特别说明：基础资料字段，也可以使用SetValue函数赋值，填写基础资料内码
            // 本示例，模拟引入数据，并不清楚这些组织的内码是多少，只知道编码，所以选用SetItemValueByNumber
            // 函数参数 : 基础资料字段Key，组织编码，行号
            dynamicFormView.SetItemValueByNumber("FUseOrgId", "101.1", 0);
            dynamicFormView.SetItemValueByNumber("FCreateOrgId", "101.1", 0);
            // 物料编码、名称 :
            // 文本(简单值类型)，直接使用SetValue赋值
            dynamicFormView.UpdateValue("FNumber", 0, "物料编码(JD-001)");
            dynamicFormView.UpdateValue("FName", 0, "物料名称(JD-001)");
            // 规格型号 (文本）
            dynamicFormView.UpdateValue("FSpecification", 0, "规格型号(JD-001)");
            // 助记码 （文本）
            dynamicFormView.UpdateValue("FMnemonicCode", 0, "助记码(JD-001)");
            // 描述 （文本）
            dynamicFormView.UpdateValue("FDescription", 0, "描述(JD-001)");
            // 来源 （下拉列表）：填写枚举值
            dynamicFormView.UpdateValue("FMaterialSRC", 0, "B");
            // 创建人、创建日期、修改人、修改日期、审核人、审核日期、
            // 禁用人、禁用日期、禁用状态、数据状态等由系统自动赋值
            /********************基本页签上的字段******************/
            // 条码 (文本）
            dynamicFormView.UpdateValue("FBARCODE", 0, "TiaoMa(JD-001)");
            // 物料属性（下拉列表，'1' = 外购，'2' = 自制...）
            dynamicFormView.UpdateValue("FErpClsID", 0, "1");
            // 存货类别（基础资料，按编码赋值 'CHLB01_SYS' = 原材料）
            dynamicFormView.SetItemValueByNumber("FCategoryID", "CHLB01_SYS", 0);
            // 税分类（单选辅助资料-物料的税分类，赋值同基础资料赋值 'WLDSFL01_SYS' = 标准税率）
            dynamicFormView.SetItemValueByNumber("FTaxType", "WLDSFL01_SYS", 0);
            // 允许采购（复选框，简单值类型，值填写true\false）
            dynamicFormView.UpdateValue("FIsPurchase", 0, true);
            // 基本单位（基础资料）：非常重要的字段，必须慎重设置
            // 基本单位仅能选择基准计量单位，如kg, m, pcs
            dynamicFormView.SetItemValueByNumber("FBaseUnitId", "kg", 0);
            // 允许库存、允许委外、即时核算、允许销售、允许生产、
            // 允许资产等字段同允许采购字段，已有默认值，无需设置
            // 默认税率已有默认值，无需设置
            // 物料分类(FTypeID，维度关联字段，赋值比较特别，需要针对各仓位维度赋值)
            // 如果没有定义仓位维度，则无需赋值
            // 另外，需要根据仓位维度的值类型，选用适当的赋值函数
            // dynamicFormView.SetItemValueByNumber("$$FTypeID__FF100001", "仓位维度1的编码(JD-001)", 0);
            // 重量单位，默认为kg;
            // 长度单位，默认为m;
            // 毛重（数量，简单值类型，直接填写数值）
            dynamicFormView.UpdateValue("FGROSSWEIGHT", 0, 0);
            // 净重、长、宽、高、体积，非重要属性，忽略
            /********************其他页签上的字段****************************/
            // 演示代码，仅设置必录字段
            // 库存单位（基础资料）
            ((IDynamicFormViewService)billView).SetItemValueByNumber("FStoreUnitID", "kg", 0);
            dynamicFormView.SetItemValueByNumber("FStoreUnitID", "kg", 0);
            // 辅助单位
            dynamicFormView.SetItemValueByNumber("FAuxUnitID", "Pcs", 0);
            // 销售单位
            dynamicFormView.SetItemValueByNumber("FSaleUnitId", "kg", 0);
            // 销售计价单位
            dynamicFormView.SetItemValueByNumber("FSalePriceUnitId", "kg", 0);
            // 采购单位
            dynamicFormView.SetItemValueByNumber("FPurchaseUnitId", "kg", 0);
            // 采购计价单位
            dynamicFormView.SetItemValueByNumber("FPurchasePriceUnitId", "kg", 0);
            // 换算方向（下拉列表, '1' = 库存单位 -> 辅助单位）
            dynamicFormView.UpdateValue("FUnitConvertDir", 0, "1");
            // 配额方式、计划策略、订货策略、
            // 固定提前期单位、变动提前期单位、检验提前期单位、订货间隔期单位、
            // 预留类型、时间单位、发料方式、超发控制方式
            // 上述字段为下拉列表，必录字段，但已经设置了默认值，演示代码忽略
            // 其他非必录字段均采用默认值
            // 推荐：
            // 可以在BOS设计器，打开物料，查看需要填写的字段类型，按照类型填字段值
            /********************库存属性单据体字段******************/
            // 需确定本物料，是否区分如下各库存维度：
            // 仓库、仓位、BOM版本、批号、计划跟踪号；
            // 默认情况下，仅区分仓库维度，仓位等不区分
            for (int rowIndex = 0; rowIndex < billView.Model.GetEntryRowCount("FEntityInvPty"); rowIndex++)
            {
                // 库存属性，基础资料类型：
                // 读取出当前库存属性信息，以便判断本物料是否启用此维度
                DynamicObject invPty = billView.Model.GetValue("FInvPtyId", rowIndex) as DynamicObject;
                if (invPty == null) continue;
                // 读出库存属性编码备用(库存属性的编码 = T_BD_INVPROPERTY.FNumber)
                string invPtyNumber = Convert.ToString(invPty["number"]);
                // 演示代码，增加仓位维度(number = '02')
                if (invPtyNumber.EqualsIgnoreCase("02"))
                {
                    // 库存属性 - 启用（复选框）
                    dynamicFormView.UpdateValue("FIsEnable", rowIndex, true);
                    // 是否影响价格等选项，采用默认值
                }
            }
            /********************辅助属性单据体字段******************/
            // 需逐行判断本物料是否启用各辅助属性
            for (int rowIndex = 0; rowIndex < billView.Model.GetEntryRowCount("FEntityAuxPty"); rowIndex++)
            {
                // 辅助属性、基础资料类型：
                // 读取出当前辅助属性信息，以便判断本物料是否启用
                DynamicObject auxProp = billView.Model.GetValue("FAuxPropertyId", rowIndex) as DynamicObject;
                if (auxProp == null) continue;
                // 读出辅助属性编码备用。
                string auxPropNumber = Convert.ToString(auxProp["number"]);
                // 判断此辅助属性是否启用(辅助属性编码 = T_BD_FLEXAUXPROPERTY.FNumber)
                // 演示代码，仅启用辅助属性 - 等级(Class)
                if (auxPropNumber.EqualsIgnoreCase("Class"))
                {
                    // 辅助属性是否启用（复选框）
                    dynamicFormView.UpdateValue("FIsEnable1", rowIndex, true);
                    // 是否影响价格等选项，采用默认值
                }
            }
            // 扩展的字段：
            dynamicFormView.UpdateValue("FTPBarCode", 0, "自定义编码");
        }
        /// <summary>
        /// 保存物料，并显示保存结果
        /// </summary>
        /// <param name="billView"></param>
        /// <returns></returns>
        private void SaveMaterial(IBillView billView, OperateOption saveOption)
        {
            // 设置FormId
            Form form = billView.BillBusinessInfo.GetForm();
            if (form.FormIdDynamicProperty != null)
            {
                form.FormIdDynamicProperty.SetValue(billView.Model.DataObject, form.Id);
            }

            // 调用保存操作
            IOperationResult saveResult = BusinessDataServiceHelper.Save(
                        this.Context,
                        billView.BillBusinessInfo,
                        billView.Model.DataObject,
                        saveOption,
                        "Save");
            // 显示处理结果
            if (saveResult == null)
            {
                this.View.ShowErrMessage("未知原因导致保存物料失败！");
                return;
            }
            else if (saveResult.IsSuccess == true)
            {// 保存成功，直接显示
                this.View.ShowOperateResult(saveResult.OperateResult);
                return;
            }
            else if (saveResult.InteractionContext != null
                    && saveResult.InteractionContext.Option.GetInteractionFlag().Count > 0)
            {// 保存失败，需要用户确认问题
                InteractionUtil.DoInteraction(this.View, saveResult, saveOption,
                    new Action<FormResult, IInteractionResult, OperateOption>((
                        formResult, opResult, option) =>
                    {
                        // 用户确认完毕，重新调用保存处理
                        this.SaveMaterial(billView, option);
                    }));
            }
            // 保存失败，显示错误信息
            if (saveResult.IsShowMessage)
            {
                saveResult.MergeValidateErrors();
                this.View.ShowOperateResult(saveResult.OperateResult);
                return;
            }
        }
        /// <summary>
        /// 创建一个单据视图，后续将利用此视图的各种方法，设置物料字段值
        /// </summary>
        /// <remarks>
        /// 理论上，也可以直接修改物料的数据包达成修改数据的目的
        /// 但是，利用单据视图更具有优势：
        /// 1. 视图会自动触发插件，这样逻辑更加完整；
        /// 2. 视图会自动利用单据元数据，填写字段默认值，不用担心字段值不符合逻辑；
        /// 3. 字段改动，会触发实体服务规则；
        /// 
        /// 而手工修改数据包的方式，所有的字段值均需要自行填写，非常麻烦
        /// </remarks>
        private IBillView CreateMaterialView()
        {
            // 读取物料的元数据
            FormMetadata meta = MetaDataServiceHelper.Load(this.Context, "BD_MATERIAL") as FormMetadata;
            Form form = meta.BusinessInfo.GetForm();
            // 创建用于引入数据的单据view
            Type type = Type.GetType("Kingdee.BOS.Web.Import.ImportBillView,Kingdee.BOS.Web");
            var billView = (IDynamicFormViewService)Activator.CreateInstance(type);
            // 开始初始化billView：
            // 创建视图加载参数对象，指定各种参数，如FormId, 视图(LayoutId)等
            BillOpenParameter openParam = CreateOpenParameter(meta);
            // 动态领域模型服务提供类，通过此类，构建MVC实例
            var provider = form.GetFormServiceProvider();
            billView.Initialize(openParam, provider);
            return billView as IBillView;
        }
        /// <summary>
        /// 创建视图加载参数对象，指定各种初始化视图时，需要指定的属性
        /// </summary>
        /// <param name="meta">元数据</param>
        /// <returns>视图加载参数对象</returns>
        private BillOpenParameter CreateOpenParameter(FormMetadata meta)
        {
            Form form = meta.BusinessInfo.GetForm();
            // 指定FormId, LayoutId
            BillOpenParameter openParam = new BillOpenParameter(form.Id, meta.GetLayoutInfo().Id);
            // 数据库上下文
            openParam.Context = this.Context;
            // 本单据模型使用的MVC框架
            openParam.ServiceName = form.FormServiceName;
            // 随机产生一个不重复的PageId，作为视图的标识
            openParam.PageId = Guid.NewGuid().ToString();
            // 元数据
            openParam.FormMetaData = meta;
            // 界面状态：新增 (修改、查看)
            openParam.Status = OperationStatus.ADDNEW;
            // 单据主键：本案例演示新建物料，不需要设置主键
            openParam.PkValue = null;
            // 界面创建目的：普通无特殊目的 （为工作流、为下推、为复制等）
            openParam.CreateFrom = CreateFrom.Default;
            // 基础资料分组维度：基础资料允许添加多个分组字段，每个分组字段会有一个分组维度
            // 具体分组维度Id，请参阅 form.FormGroups 属性
            openParam.GroupId = "";
            // 基础资料分组：如果需要为新建的基础资料指定所在分组，请设置此属性
            openParam.ParentId = 0;
            // 单据类型
            openParam.DefaultBillTypeId = "";
            // 业务流程
            openParam.DefaultBusinessFlowId = "";
            // 主业务组织改变时，不用弹出提示界面
            openParam.SetCustomParameter("ShowConfirmDialogWhenChangeOrg", false);
            // 插件
            List<AbstractDynamicFormPlugIn> plugs = form.CreateFormPlugIns();
            openParam.SetCustomParameter(FormConst.PlugIns, plugs);
            PreOpenFormEventArgs args = new PreOpenFormEventArgs(this.Context, openParam);
            foreach (var plug in plugs)
            {// 触发插件PreOpenForm事件，供插件确认是否允许打开界面
                plug.PreOpenForm(args);
            }
            if (args.Cancel == true)
            {// 插件不允许打开界面
                // 本案例不理会插件的诉求，继续....
            }
            // 返回
            return openParam;
        }
        /// <summary>
        /// 加载指定的单据进行修改
        /// </summary>
        /// <param name="billView"></param>
        /// <param name="pkValue"></param>
        private void ModifyBill(IBillView billView, string pkValue)
        {
            billView.OpenParameter.Status = OperationStatus.EDIT;
            billView.OpenParameter.CreateFrom = CreateFrom.Default;
            billView.OpenParameter.PkValue = pkValue;
            billView.OpenParameter.DefaultBillTypeId = string.Empty;
            ((IDynamicFormViewService)billView).LoadData();
        }
    }
}
