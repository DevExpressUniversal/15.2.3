#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Controls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
namespace DevExpress.ExpressApp.TreeListEditors.Web {
	public class ASPxTreeListEditor : ComplexWebListEditor, INodeObjectAdapterProvider, ITestable, ISupportCallbackStartupScriptRegistering, IDataItemTemplateInfoProvider, ISupportSelectionOperations, IExportable, ISupportFooter, ISupportPager, ISupportAppearanceCustomization, ICustomRenderUpdatePanel {
		private ASPxTreeListDataBinderBase dataBinder;
		private Locker updateLocker = new Locker();
		private ASPxTreeListContextMenu contextMenu;
		private WebImageCache imageCache = new WebImageCache();
		public const string RowObjectColumnName = "__RowObjectColumn";
		private ASPxTreeList treeList;
		private ASPxTreeListExporter exporter;
		private object rootValue;
		private bool holdRootValue = false;
		private ASPxTreeListSupportCallbackStartupScriptRegisteringImpl startupScriptRegisteringImpl;
		private DataBindingMode dataBindingMode = DataBindingMode.Virtual;
		private List<string> visiblePropertyList = new List<string>();
		private Dictionary<WebColumnBase, IColumnInfo> columnsInfoCache = new Dictionary<WebColumnBase, IColumnInfo>();
		private void treeList_SelectionChanged(object sender, EventArgs e) {
			needRenderUpdatePanels = true;
			OnSelectionChanged();
		}
		private void treeList_Load(object sender, EventArgs e) {
			System.Web.UI.Page page = TreeList.Page;
			exporter.Page = TreeList.Page;
			if(!TreeList.IsCallback) {
				UpdateTemplateImageDecorator();
			}
			DataBinder.DataBind();
			if(page.IsPostBack) {
				RenderUtils.LoadPostDataRecursive(TreeList, page.Request.Params);
			}
		}
		protected void DataBinder_DataBound(object sender, EventArgs e) {
			ApplyPagerModelSettingsToControl();
		}
		private void treeList_BeforeGetCallbackResult(object sender, EventArgs e) {
			UpdateTemplateImageDecorator();
			RenderUpdatePanels();
		}
		private string GetDataFieldName(IModelColumn columnInfo) {
			return new ObjectEditorHelperBase(XafTypesInfo.Instance.FindTypeInfo(columnInfo.ModelMember.Type), columnInfo).GetFullDisplayMemberName(columnInfo.PropertyName);
		}
		protected virtual ITemplate CreateEditItemTemplate(IModelColumn columnInfo) {
			return CreateDefaultColumnTemplate(columnInfo, this, ViewEditMode.Edit);
		}
		protected virtual ITemplate CreateDataItemTemplate(IModelColumn columnInfo) {
			return CreateDefaultColumnTemplate(columnInfo, this, ViewEditMode.View);
		}
		private void treeList_HtmlDataCellPrepared(object sender, TreeListHtmlDataCellEventArgs e) {
			object rowObj = TreeList.FindNodeByKeyValue(e.NodeKey)[ASPxTreeListEditor.RowObjectColumnName];
			IDataColumnInfo info = GetGridDataColumnInfo(e.Column) as IDataColumnInfo;
			if(info != null) {
				OnCustomizeAppearance(new CustomizeAppearanceEventArgs(info.Model.PropertyName, new TableCellAppearanceAdapter(e.Cell), rowObj));
			}
			e.Cell.Style["cursor"] = "pointer";
		}
		private void ApplySettings() {
			treeList.Settings.GridLines = GridLines.None;
			treeList.Settings.ShowTreeLines = true;
			treeList.Settings.ShowColumnHeaders = true;
			treeList.SettingsBehavior.AutoExpandAllNodes = false;
			treeList.SettingsBehavior.AllowFocusedNode = false;
			treeList.SettingsBehavior.FocusNodeOnExpandButtonClick = false;
			treeList.SettingsCustomizationWindow.Enabled = true;
			treeList.SettingsPager.Mode = TreeListPagerMode.ShowPager;
			treeList.SettingsSelection.Recursive = false;
			treeList.SettingsSelection.AllowSelectAll = true;
			treeList.SettingsSelection.Enabled = true;
		}
		private void PrepareDataBinder() {
			NodeObjectAdapter adapter = CreateDataAdapter(ObjectSpace);
			dataBinder = CreateDataBinder(adapter);
			dataBinder.RootValue = RootValue;
			dataBinder.HoldRootValue = HoldRootValue;
			DataBinder.DataBound += DataBinder_DataBound;
		}
		protected internal void DoOnProcessSelectedItem() {
			base.OnProcessSelectedItem();
		}
		protected virtual NodeObjectAdapter CreateDataAdapter(IObjectSpace objectSpace) {
			TreeNodeInterfaceAdapter adapter = new TreeNodeInterfaceAdapter();
			adapter.ObjectSpace = objectSpace;
			return adapter;
		}
		protected virtual ASPxTreeListDataBinderBase CreateDataBinder(NodeObjectAdapter adapter) {
			ASPxTreeListDataBinderBase result;
			List<string> displayableProperties = new List<string>();
			foreach(IModelColumn column in Model.Columns) {
				displayableProperties.Add(GetDataFieldName(column));
			}
			switch(DataBindingMode) {
				case DataBindingMode.Unbound:
					result = new ASPxTreeListDataBinderUnboundMode(treeList, adapter, ObjectTypeInfo, displayableProperties);
					break;
				default:
					result = new ASPxTreeListDataBinderVirtualMode(treeList, adapter, ObjectTypeInfo, displayableProperties);
					break;
			}
			return result;
		}
		private void SubscribeToASPxTreeListEvents() {
			treeList.SelectionChanged += new EventHandler(treeList_SelectionChanged);
			treeList.HtmlDataCellPrepared += new TreeListHtmlDataCellEventHandler(treeList_HtmlDataCellPrepared);
			treeList.Load += new EventHandler(treeList_Load);
			treeList.Unload += new EventHandler(treeList_Unload);
			treeList.BeforeGetCallbackResult += new EventHandler(treeList_BeforeGetCallbackResult);
			treeList.PageIndexChanged += TreeList_PageIndexChanged;
			treeList.DataBound += TreeList_DataBound;
		}
		private void UnsubscribeFromASPxTreeListEvents() {
			treeList.HtmlDataCellPrepared -= new TreeListHtmlDataCellEventHandler(treeList_HtmlDataCellPrepared);
			treeList.SelectionChanged -= new EventHandler(treeList_SelectionChanged);
			treeList.Load -= new EventHandler(treeList_Load);
			treeList.Unload -= new EventHandler(treeList_Unload);
			treeList.BeforeGetCallbackResult -= new EventHandler(treeList_BeforeGetCallbackResult);
			treeList.PageIndexChanged -= TreeList_PageIndexChanged;
		}
		private void startupScriptRegisteringImpl_RegisterCallbackStartupScript(object sender, RegisterCallbackStartupScriptEventArgs e) {
			OnRegisterCallbackStartupScript(e);
		}
		private void UpdateTemplateImageDecorator() {
			foreach(TreeListColumn treeListColumn in treeList.Columns) {
				TreeListDataColumn dataColumn = treeListColumn as TreeListDataColumn;
				if(dataColumn != null && dataColumn.DataCellTemplate != null) {
					if(dataColumn.DataCellTemplate is ASPxTreeListDataCellTemplateImageDecorator) {
						dataColumn.DataCellTemplate = ((ASPxTreeListDataCellTemplateImageDecorator)dataColumn.DataCellTemplate).DecoratedTemplate;
					}
					if(dataColumn.VisibleIndex == 0) {
						dataColumn.DataCellTemplate = new ASPxTreeListDataCellTemplateImageDecorator(dataColumn.DataCellTemplate, imageCache);
					}
				}
			}
		}
		private void ClearCache() {
			columnsInfoCache.Clear();
		}
		protected override object CreateControlsCore() {
			treeList = CreateTreeListControl();
			startupScriptRegisteringImpl = new ASPxTreeListSupportCallbackStartupScriptRegisteringImpl(treeList);
			startupScriptRegisteringImpl.RegisterCallbackStartupScript += new EventHandler<RegisterCallbackStartupScriptEventArgs>(startupScriptRegisteringImpl_RegisterCallbackStartupScript);
			DevExpress.ExpressApp.Web.RenderHelper.SetupASPxWebControl(treeList);
			SubscribeToASPxTreeListEvents();
			treeList.ID = "TreeList";
			treeList.AutoGenerateColumns = false;
			treeList.Width = Unit.Percentage(100);
			treeList.BackColor = Color.Transparent;
			treeList.DataCacheMode = TreeListDataCacheMode.Disabled;
			ApplySettings();
			PrepareDataBinder();
			ApplyModel();
			TreeListDataColumn column = new TreeListDataColumn();
			column.VisibleIndex = -1;
			column.Visible = false;
			column.FieldName = RowObjectColumnName;
			treeList.Columns.Add(column);
			contextMenu.CreateControls();
			exporter = new ASPxTreeListExporter();
			exporter.TreeListID = treeList.ID;
			exporter.RenderBrick += new ASPxTreeListRenderBrickEventHandler(exporter_RenderBrick);
			OnPrintableChanged();
			return treeList;
		}
		void exporter_RenderBrick(object sender, ASPxTreeListExportRenderBrickEventArgs e) {
			ViewModeDataItemTemplate template;
			TreeListDataColumn column = e.Column as TreeListDataColumn;
			if (column != null && e.RowKind == TreeListRowKind.Data && column.DataCellTemplate != null) {
				if (column.DataCellTemplate is ASPxTreeListDataCellTemplateImageDecorator) {
					template = (ViewModeDataItemTemplate)((ASPxTreeListDataCellTemplateImageDecorator)column.DataCellTemplate).DecoratedTemplate;
				} else {
					template = column.DataCellTemplate as ViewModeDataItemTemplate;
				}
				if (template != null && template.PropertyEditor != null && template.PropertyEditor is ISupportExportCustomValue) {
					template.PropertyEditor.CurrentObject = TreeList.FindNodeByKeyValue(e.NodeKey)[ASPxTreeListEditor.RowObjectColumnName];
					string localizedValue = ((ISupportExportCustomValue)template.PropertyEditor).GetExportedValue();
					if (localizedValue != null) {
						e.Text = localizedValue;
					}
				}
			}
		}
		protected void TreeList_DataBound(object sender, EventArgs e) {
			ApplyPagerModelSettingsToControl();
		}
		private void ApplyPagerModelSettingsToControl() {
			if(Model != null && TreeList != null) {
				ASPxTreeListModelSynchronizer.ApplyPagerModel(Model, TreeList);
			}
		}
		private void TreeList_PageIndexChanged(object sender, EventArgs e) {
			SavePagerSettingsToModel();
		}
		private void SavePagerSettingsToModel() {
			if(TreeList != null && Model != null) {
				ASPxTreeListModelSynchronizer.SavePagerModel(Model, TreeList);
			}
		}
		protected void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(Printable));
			}
		}
		private void treeList_Unload(object sender, EventArgs e) {
			OnControlInitialized(sender as Control);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		protected virtual ASPxTreeList CreateTreeListControl() {
			return new ASPxTreeList();
		}
		protected override void OnControlsCreated() {
			base.OnControlsCreated();
			UpdateControlDataSource();
		}
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			UpdateControlDataSource();
		}
		protected void UpdateControlDataSource() {
			if(treeList != null) {
				dataBinder.DataSource = ListHelper.GetList(DataSource);
				if(ControlDataSourceChanged != null) {
					ControlDataSourceChanged(this, EventArgs.Empty);
				}
			}
		}
		protected override void AssignDataSourceToControl(Object dataSource) {
		}
		public ASPxTreeListEditor(IModelListView model)
			: base(model) {
			contextMenu = new ASPxTreeListContextMenu(this);
		}
		public override void ApplyModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelApplying(args);
				if(!args.Cancel) {
					if(TreeList != null) {
						ASPxTreeListModelSynchronizer.ApplyTreeListModel(Model, TreeList);
						new ColumnsListEditorModelSynchronizer(this, Model).ApplyModel();
					}
					base.ApplyModel();
					ApplyPagerModelSettingsToControl();
					OnModelApplied();
				}
			}
		}
		public override void SaveModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelSaving(args);
				if(!args.Cancel) {
					if(TreeList != null) {
						ASPxTreeListModelSynchronizer.SaveTreeListModel(Model, TreeList);
						new ColumnsListEditorModelSynchronizer(this, Model).SynchronizeModel();
					}
					base.SaveModel();
					SavePagerSettingsToModel();
					OnModelSaved();
				}
			}
		}
		public override IList<object> GetControlSelectedObjects() {
			List<object> result = new List<object>();
			if(TreeList != null) {
				List<TreeListNode> selectedNodes = TreeList.GetSelectedNodes();
				if(selectedNodes.Count != 0) {
					foreach(TreeListNode node in selectedNodes) {
						result.Add(node[RowObjectColumnName]);
					}
				}
			}
			return result;
		}
		public override void SetControlSelectedObjects(IList<object> objects) {
			foreach(TreeListNode node in TreeList.GetAllNodes()) {
				node.Selected = objects.Contains(node[ASPxTreeListEditor.RowObjectColumnName]);
			}
			OnSelectionChanged();
		}
		public override void Refresh() {
			UpdateControlDataSource();
			base.Refresh();
		}
		public override void BreakLinksToControls() {
			visiblePropertyList.Clear();
			if(dataBinder != null) {
				dataBinder.DataBound -= DataBinder_DataBound;
				dataBinder.Dispose();
				dataBinder = null;
			}
			if(contextMenu != null) {
				contextMenu.BreakLinksToControls();
			}
			if(treeList != null) {
				TreeList.PageIndexChanged -= TreeList_PageIndexChanged;
				UnsubscribeFromASPxTreeListEvents();
				foreach(TreeListColumn column in treeList.Columns) {
					if(column is TreeListDataColumn) {
						TreeListDataColumn treeListColumn = (TreeListDataColumn)column;
						treeListColumn.DataCellTemplate = null;
					}
				}
				ClearCache();
				treeList = null;
				if (exporter != null) {
					exporter.RenderBrick -= new ASPxTreeListRenderBrickEventHandler(exporter_RenderBrick);
					exporter = null;
					OnPrintableChanged();
				}
			}
			if(startupScriptRegisteringImpl != null) {
				startupScriptRegisteringImpl.RegisterCallbackStartupScript -= new EventHandler<RegisterCallbackStartupScriptEventArgs>(startupScriptRegisteringImpl_RegisterCallbackStartupScript);
				startupScriptRegisteringImpl.Dispose();
				startupScriptRegisteringImpl = null;
			}
			base.BreakLinksToControls();
		}
		public override void Dispose() {
			try {
				getRenderResults = null;
				BreakLinksToControls();
				if(contextMenu != null) {
					contextMenu.Dispose();
					contextMenu = null;
				}
				if(imageCache != null) {
					imageCache.Dispose();
					imageCache = null;
				}
				updateLocker = null;
			}
			finally {
				base.Dispose();
			}
		}
		public override Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return (dataAccessMode == CollectionSourceDataAccessMode.Client);
		}
		public override DevExpress.ExpressApp.Templates.IContextMenuTemplate ContextMenuTemplate {
			get { return contextMenu; }
		}
		public override SelectionType SelectionType {
			get { return SelectionType.MultipleSelection; }
		}
		public override string[] RequiredProperties {
			get {
				List<string> result = new List<string>();
				foreach(IModelColumn node in Model.Columns.GetVisibleColumns()) {
					string dataTextField = GetDataFieldName(node);
					if(!string.IsNullOrEmpty(dataTextField)) {
						result.Add(GetDisplayablePropertyName(dataTextField.TrimEnd('!')));
					}
				}
				return result.ToArray();
			}
		}
		public ASPxTreeList TreeList {
			get { return treeList; }
		}
		public ASPxTreeListDataBinderBase DataBinder {
			get { return dataBinder; }
		}
		public object RootValue {
			get { return rootValue; }
			set {
				if(value != rootValue) {
					rootValue = value;
					if(DataBinder != null) {
						DataBinder.RootValue = rootValue;
						UpdateControlDataSource();
					}
				}
			}
		}
		public bool HoldRootValue {
			get { return holdRootValue; }
			set {
				if(value != holdRootValue) {
					holdRootValue = value;
					if(DataBinder != null) {
						DataBinder.HoldRootValue = holdRootValue;
						UpdateControlDataSource();
					}
				}
			}
		}
		public DataBindingMode DataBindingMode {
			get { return dataBindingMode; }
			set { dataBindingMode = value; }
		}
		#region ISupportFooter Members
		public bool IsFooterVisible {
			get { return TreeList.Settings.ShowFooter; }
			set { TreeList.Settings.ShowFooter = value; }
		}
		#endregion
		#region IDataItemTemplateInfoProvider Members
		private event EventHandler<CustomGetDataColumnInfoEventArgs> customGetViewDataColumnInfo;
		protected virtual void OnCustomGetDataColumnInfo(WebColumnBase column, CustomGetDataColumnInfoEventArgs args) {
			if(customGetViewDataColumnInfo != null) {
				customGetViewDataColumnInfo(this, args);
			}
		}
		event EventHandler<CustomGetDataColumnInfoEventArgs> IDataItemTemplateInfoProvider.CustomGetDataColumnInfo {
			add { customGetViewDataColumnInfo += value; }
			remove { customGetViewDataColumnInfo -= value; }
		}
		private IColumnInfo GetGridDataColumnInfo(WebColumnBase column) {
			CustomGetDataColumnInfoEventArgs args = new CustomGetDataColumnInfoEventArgs(column);
			OnCustomGetDataColumnInfo(column, args);
			IColumnInfo result = args.ColumnInfo;
			if(result == null) {
				columnsInfoCache.TryGetValue(column, out result);
			}
			return result;
		}
		IColumnInfo IDataItemTemplateInfoProvider.GetColumnInfo(WebColumnBase column) {
			return GetGridDataColumnInfo(column);
		}
		WebColumnBase IDataItemTemplateInfoProvider.GetColumn(Control container) {
			return ((TreeListDataCellTemplateContainer)container).Column;
		}
		public object GetObject(Control container) {
			TreeListDataCellTemplateContainer holder = (TreeListDataCellTemplateContainer)container;
			return holder.GetValue(RowObjectColumnName);
		}
		public string GetContainerId(Control container, string propertyName) {
			return "cntr_" + propertyName + "_" + ((TreeListDataCellTemplateContainer)container).NodeKey;
		}
		#endregion
		#region ITestable Members
		public string TestCaption {
			get { return Name; }
		}
		public string ClientId {
			get { return TreeList != null ? TreeList.ClientID : null; }
		}
		public IJScriptTestControl TestControl {
			get { return new JSASPxTreeListEditorTestControl(); }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public TestControlType TestControlType {
			get { return TestControlType.Table; }
		}
		#endregion
		#region INodeObjectAdapterProvider Members
		public NodeObjectAdapter Adapter {
			get { return (dataBinder is ASPxTreeListDataBinderUnboundBase) ? ((ASPxTreeListDataBinderUnboundBase)dataBinder).Adapter : null; }
		}
		#endregion
		#region ISupportCallbackStartupScriptRegistering Members
		protected virtual void OnRegisterCallbackStartupScript(RegisterCallbackStartupScriptEventArgs e) {
			if(RegisterCallbackStartupScript != null) {
				RegisterCallbackStartupScript(this, e);
			}
		}
		public event EventHandler<RegisterCallbackStartupScriptEventArgs> RegisterCallbackStartupScript;
		#endregion
		public override IList<ColumnWrapper> Columns {
			get {
				List<ColumnWrapper> result = new List<ColumnWrapper>();
				if(TreeList != null) {
					foreach(TreeListColumn column in TreeList.Columns) {
						if(column is TreeListDataColumn) {
							IDataColumnInfo dataColumnInfo = GetGridDataColumnInfo(column) as IDataColumnInfo;
							if(dataColumnInfo != null) {
								result.Add(new XafTreeListColumnWrapper((TreeListDataColumn)column, dataColumnInfo));
							}
						}
					}
				}
				return result;
			}
		}
		public override void RemoveColumn(ColumnWrapper column) {
			base.RemoveColumn(column);
			if(column is XafTreeListColumnWrapper) {
				columnsInfoCache.Remove(((XafTreeListColumnWrapper)column).Column);
			}
		}
		protected override ColumnWrapper AddColumnCore(IModelColumn columnInfo) {
			CollectionSourceDataAccessMode viewDataAccessMode = Model.DataAccessMode;
			bool isProtectedContentColumn = HasProtectedContent(columnInfo.PropertyName);
			TreeListDataColumnInfo treeListDataColumnInfo = new TreeListDataColumnInfo(columnInfo, ObjectTypeInfo.FindMember(columnInfo.PropertyName), viewDataAccessMode, isProtectedContentColumn);
			TreeListDataColumn column = new TreeListDataColumn();
			columnsInfoCache.Add(column, treeListDataColumnInfo);
			string dataTextField = GetDataFieldName(columnInfo);
			if(!string.IsNullOrEmpty(dataTextField)) {
				if(column.Visible) {
					visiblePropertyList.Add(GetDisplayablePropertyName(dataTextField.TrimEnd('!')));
				}
			}
			column.FieldName = dataTextField;
			column.Name = columnInfo.Caption;
			column.DataCellTemplate = CreateDataItemTemplate(columnInfo);
			column.EditCellTemplate = CreateEditItemTemplate(columnInfo);			
			treeList.Columns.Add(column);
			XafTreeListColumnWrapper columnWrapper = new XafTreeListColumnWrapper(column, treeListDataColumnInfo);
			columnWrapper.ApplyModel(columnInfo);
			return columnWrapper;
		}
		#region ISupportSelectionOperations Members
		public void UnselectAll() {
			if(TreeList != null) {
				TreeList.UnselectAll();
			}
		}
		public void UnselectRowByKey(object key) {
			if(!updateLocker.Locked) {
				OnSelectionChanged();
			}
		}
		public void BeginUpdateSelection() {
			updateLocker.Lock();
		}
		public void EndUpdateSelection() {
			updateLocker.Unlock();
			if(!updateLocker.Locked) {
				OnSelectionChanged();
			}
		}
		#endregion
		public List<ExportTarget> SupportedExportFormats {
			get {
				if(Printable == null) {
					return new List<ExportTarget>();
				}
				else {
					return new List<ExportTarget>(){
				ExportTarget.Csv,
				ExportTarget.Html,
				ExportTarget.Image,
				ExportTarget.Mht,
				ExportTarget.Pdf,
				ExportTarget.Rtf,
				ExportTarget.Text,
				ExportTarget.Xls,
				ExportTarget.Xlsx
				};
				}
			}
		}
		public IPrintable Printable {
			get { return exporter; }
		}
		public void OnExporting() {
			if(treeList !=null && exporter != null && exporter.Settings.ExpandAllNodes) {
				treeList.ExpandAll();
			}
		}
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
		public event EventHandler<EventArgs> ControlDataSourceChanged;
		int ISupportPager.PageIndex {
			get { return TreeList.PageIndex; }
			set { TreeList.PageIndex = value; }
		}
		int ISupportPager.PageSize {
			get { return TreeList.SettingsPager.PageSize; }
			set { TreeList.SettingsPager.PageSize = value; }
		}
		#region ISupportAppearanceCustomization Members
		protected virtual void OnCustomizeAppearance(CustomizeAppearanceEventArgs e) {
			if(CustomizeAppearance != null) {
				CustomizeAppearance(this, e);
			}
		}
		public event EventHandler<CustomizeAppearanceEventArgs> CustomizeAppearance;
		#endregion
		#region ICustomRenderUpdatePanel Members
		private bool needRenderUpdatePanels;
		private event EventHandler<GetCustomRenderResultsArgs> getRenderResults;
		event EventHandler<GetCustomRenderResultsArgs> ICustomRenderUpdatePanel.GetRenderResults {
			add { getRenderResults += value; }
			remove { getRenderResults -= value; }
		}
		private void RenderUpdatePanels() {
			if(needRenderUpdatePanels && getRenderResults != null) {
				GetCustomRenderResultsArgs eventArgs = new GetCustomRenderResultsArgs();
				getRenderResults(this, eventArgs);
				TreeList.JSProperties[XafCallbackManager.ControlsToUpdate] = "";
				if(eventArgs.RenderResults.Count > 0) {
					foreach(string panelId in eventArgs.RenderResults.Keys) {
						TreeList.JSProperties[XafCallbackManager.ControlsToUpdate] += panelId + ";";
						TreeList.JSProperties["cp" + panelId] = eventArgs.RenderResults[panelId];
					}
				}
				needRenderUpdatePanels = false;
			}
		}
		#endregion
#if DebugTest
		public Dictionary<WebColumnBase, IColumnInfo> ColumnsInfoCache_ForTests {
			get { return columnsInfoCache; }
		}
#endif
	}
}
