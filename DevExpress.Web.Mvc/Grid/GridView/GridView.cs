#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using System.Collections;
	using System.Web;
	using DevExpress.Web;
	using DevExpress.Web.Data;
	using DevExpress.Web.FilterControl;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.Web.Rendering;
	[ToolboxItem(false)]
	public class MVCxGridView: ASPxGridView, IGridAdapterOwner, IViewContext {
		bool enableCustomOperation;
		ViewContext viewContext;
		NameValueCollection postDataCollection;
		IDictionary<string, string> callbackActionUrlCollection;
		GridViewModel customOperationViewModel;
		public MVCxGridView()
			: this(null) {
		}
		protected internal MVCxGridView(ViewContext viewContext)
			: base() {
			GridAdapter = new GridAdapter(this);
			this.callbackActionUrlCollection = new Dictionary<string, string>();
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		protected override GridFilterHelper CreateFilterHelper() {
			if (!EnableCustomOperations)
				return base.CreateFilterHelper();
			return new MVCxGridViewCustomBindingFilterHelper(this);
		}
		protected internal new void ResetFilterHelper() {
			base.ResetFilterHelper();
		}
		protected internal virtual GridViewModel CustomOperationViewModel {
			get {
				if(customOperationViewModel == null)
					customOperationViewModel = CreateCustomOperationViewModel();
				return customOperationViewModel;
			}
		}
		protected internal new GridCallbackState CallbackState { get { return base.CallbackState; } }
		protected internal new MVCxGridViewDataProxy DataProxy { get { return (MVCxGridViewDataProxy)base.DataProxy; } }
		protected internal new IList<GridViewDataColumn> DataColumns { get { return base.DataColumns; } }
		protected internal new GridViewRenderHelper RenderHelper { get { return base.RenderHelper; } }
		protected internal new GridViewEndlessPagingHelper EndlessPagingHelper { get { return base.EndlessPagingHelper; } }
		protected internal new MVCxGridViewBatchEditHelper BatchEditHelper { get { return (MVCxGridViewBatchEditHelper)base.BatchEditHelper; } }
		protected internal IDictionary<string, string> CallbackActionUrlCollection { get { return callbackActionUrlCollection; } }
		protected internal int TotalRowCount { get; set; }
		protected internal bool EnableCustomOperations {
			get { return enableCustomOperation; }
			set {
				if(EnableCustomOperations == value)
					return;
				enableCustomOperation = value;
				DataProxy.CustomOperationChanged();
			}
		}
		protected override GridFormLayoutProperties CreateEditFormLayoutProperties() {
			return new MVCxGridViewFormLayoutProperties(this);
		}
		protected override WebDataProxy CreateDataProxy() {
			return new MVCxGridViewDataProxy(this, this, this);
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new MVCxGridDataHelper(this, name);
		}
		protected virtual GridViewModel CreateCustomOperationViewModel() {
			return new GridViewModel(this);
		}
		public new GridViewImages Images { get { return base.Images; } }
		public new GridViewStyles Styles { get { return base.Styles; } }
		public new MVCxGridViewDetailSettings SettingsDetail {
			get { return (MVCxGridViewDetailSettings)base.SettingsDetail; }
		}
		public new MVCxGridViewEditingSettings SettingsEditing {
			get { return (MVCxGridViewEditingSettings)base.SettingsEditing; }
		}
		public new MVCxGridViewBehaviorSettings SettingsBehavior {
			get { return (MVCxGridViewBehaviorSettings)base.SettingsBehavior; }
		}
		protected internal new ASPxGridViewCustomizationWindowSettings SettingsCustomizationWindowInternal {
			get { return base.SettingsCustomizationWindowInternal; }
		}
		protected internal string ErrorText { get; set; }
		protected override bool IsFirstLoad { get { return !IsCallback; } } 
		protected internal new int PageSize { get { return base.PageSize; } set { base.PageSize = value; } }
		protected internal new int InitialPageSize { get { return base.InitialPageSize; } set { base.InitialPageSize = value; } }
		protected override NameValueCollection PostDataCollection { 
			get {
				if(postDataCollection == null && Controller != null && Controller.ValueProvider != null)
					postDataCollection = new MvcPostDataCollection(Controller.ValueProvider);
				return postDataCollection ?? base.PostDataCollection; 
			} 
		}
		protected internal GridAdapter GridAdapter { get; private set; }
		protected internal ViewContext ViewContext { get { return viewContext; } }
		protected internal ControllerBase Controller { get { return (ViewContext != null) ? ViewContext.Controller : null; } }
		protected internal ModelStateDictionary ModelState {
			get {
				Controller controller = Controller as Controller;
				return (controller != null) ? controller.ModelState : null;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void AddNewRow() {
			base.AddNewRow();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void DeleteRow(int visibleIndex) {
			base.DeleteRow(visibleIndex);
		}
		protected internal new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		protected internal new void LayoutChanged() {
			base.LayoutChanged();
		}
		protected override void OnFilterExpressionChanging(string value, bool isFilterEnabled) {
			if (EnableCustomOperations) {
				value = CustomOperationViewModel.FilterExpression;
				isFilterEnabled = CustomOperationViewModel.IsFilterApplied;
			}
			base.OnFilterExpressionChanging(value, isFilterEnabled);
		}
		protected internal override bool IsCallBacksEnabled() {
			return true;
		}
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ClientID; }
		}
		protected string FilterControlCallbackName {
			get {
				return string.Format("{0}_{1}_{2}", ClientID, ASPxPopupFilterControl.PopupFilterControlFormID,
					ASPxPopupFilterControl.PopupFilterControlID);
			}
		}
		protected override string GetCallbackResultHtml() {
			if(IsDataCallback() || RenderHelper.RequireEndlessPagingPartialLoad)
				return base.GetCallbackResultHtml();
			return Utils.CallbackHtmlContentPlaceholder;
		}
		protected internal Control GetCallbackResultControl() {
			if(IsDataCallback() || RenderHelper.RequireEndlessPagingPartialLoad)
				return null;
			EnsureChildControls();
			return ContainerControl;
		}
		protected internal override FormLayoutProperties GenerateDefaultLayout(bool fromControlDesigner) {
			var properties =  base.GenerateDefaultLayout(fromControlDesigner);
			FormLayoutItemHelper.ConfigureLayoutItemsByMetadata(properties);
			return properties;
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			if(MvcUtils.RenderMode != MvcRenderMode.RenderResources)
				RenderUtils.LoadPostDataRecursive(this, PostDataCollection, false, (control) => control as DropDownPopupControl == null);
		}
		protected override GridUpdatableContainer CreateContainerControl() {
			return new MVCxGridViewContainerControl(this);
		}
		protected internal new MVCxGridViewContainerControl ContainerControl {
			get { return (MVCxGridViewContainerControl)base.ContainerControl; }
		}
		protected internal MVCxWebFilterControlPopup PopupFilterControlForm {
			get { return ContainerControl.PopupFilterControlForm; }
		}
		protected internal MVCxPopupFilterControl FilterControl {
			get { return (PopupFilterControlForm != null) ? PopupFilterControlForm.FilterControl : null; }
		}
		protected internal Type GetColumnDataType(GridViewDataColumn column) {
			return (column as IWebGridDataColumn).Adapter.DataType;
		}
		protected internal GridColumnEditKind GetColumnEditKind(GridViewDataColumn column) {
			return FilterHelper.GetEditKind(column);
		}
		protected internal int GetColumnGlobalIndex(GridViewDataColumn column) {
			return ColumnHelper.GetColumnGlobalIndex(column);
		}
		protected internal bool IsDataCallback() {
			return InternalCallbackInfo != null;
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if(CustomActionRouteValues != null)
				stb.Append(localVarName + ".customActionUrl=\"" + Utils.GetUrl(CustomActionRouteValues) + "\";\n");
			if(SettingsEditing.AddNewRowRouteValues != null)
				stb.Append(localVarName + ".addNewItemUrl=\"" + Utils.GetUrl(SettingsEditing.AddNewRowRouteValues) + "\";\n");
			if(SettingsEditing.UpdateRowRouteValues != null)
				stb.Append(localVarName + ".updateItemUrl=\"" + Utils.GetUrl(SettingsEditing.UpdateRowRouteValues) + "\";\n");
			if(SettingsEditing.DeleteRowRouteValues != null)
				stb.Append(localVarName + ".deleteItemUrl=\"" + Utils.GetUrl(SettingsEditing.DeleteRowRouteValues) + "\";\n");
			if(SettingsEditing.BatchUpdateRouteValues != null)
				stb.Append(localVarName + ".batchUpdateUrl=\"" + Utils.GetUrl(SettingsEditing.BatchUpdateRouteValues) + "\";\n");
			if(!string.IsNullOrEmpty(KeyFieldName))
				stb.Append(localVarName + ".keyName=\"" + KeyFieldName + "\";\n");
			if(CallbackActionUrlCollection.Count > 0) {
				stb.Append(localVarName + ".callbackActionUrlCollection=eval(\"" + HtmlConvertor.ToJSON(CallbackActionUrlCollection) + "\");\n");
			}
		}
		protected override void InitializeClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientID) {
			base.InitializeClientObjectScript(stb, localVarName, clientID);
			int clientEditingRowVisibleIndex = RenderHelper.UseEndlessPaging ? EndlessPagingHelper.ClientEditingRowVisibleIndex : EditingRowVisibleIndex;
			stb.Append(localVarName + ".editingItemVisibleIndex=" + clientEditingRowVisibleIndex + ";\n");
			if(FilterControl == null && MvcUtils.CallbackName == FilterControlCallbackName)
				GetCustomJSPropertiesScript(stb, ShortClientLocalVariableName);
			if(IsEditing || SettingsEditing.Mode == GridViewEditingMode.Batch) {
				IEnumerable<string> editorNames = DataColumns.Where(c => c.GroupIndex < 0).Select(c => GetEditorClientIDByColumn(c));
				GridAdapter.AppendUnobtrusiveRules(stb, editorNames);
			}
		}
		protected internal string GetEditorIdByFieldName(string fieldName) {
			GridViewDataColumn dataColumn = Columns[fieldName] as GridViewDataColumn;
			return GetEditorClientIDByColumn(dataColumn);
		}
		string GetEditorClientIDByColumn(GridViewDataColumn column) {
			if(column == null)
				return null;
			string editorPrefix = ClientID;
			if(SettingsEditing.IsPopupEditForm)
				editorPrefix += "_" + GridViewRenderHelper.PopupEditFormID;
			if(IsEditFormLayoutExists())
				editorPrefix += "_" + GridViewRenderHelper.EditFormLayoutID;
			return string.Format("{0}_{1}", editorPrefix, RenderHelper.GetEditorId(column));
		}
		bool IsEditFormLayoutExists() {
			if(!SettingsEditing.UseFormLayout)
				return false;
			return SettingsEditing.IsEditForm || SettingsEditing.IsPopupEditForm;
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = base.GetClientObjectState();
			if(EnableCustomOperations || (CallbackActionUrlCollection != null && CallbackActionUrlCollection.Count() > 0)) {
				result.Add(MVCxGridViewRenderHelper.CustomOperationStateKey, CustomOperationViewModel.Save());
			}
			return result;
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientGridView";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxGridView), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxGridView), Utils.GridAdapterScriptResourceName);
			RegisterIncludeScript(typeof(MVCxGridView), Utils.GridViewScriptResourceName);
		}
		public override bool IsLoading() {
			return false;
		}
		protected internal void PerformOnLoad() {
			LoadColumnData();
			OnLoad(EventArgs.Empty);
		}
		void LoadColumnData() {
			foreach(GridViewDataColumn column in RenderHelper.ColumnHelper.AllDataColumns) {
				ComboBoxProperties prop = column.PropertiesEdit as ComboBoxProperties;
				if(prop != null && prop.DataSource != null)
					EditorsIntegrationHelper.CheckInplaceBound(prop, prop.ValueType, this);
				DateEditProperties dateEditProperties = column.PropertiesEdit as DateEditProperties;
				if (dateEditProperties != null && !string.IsNullOrEmpty(dateEditProperties.DateRangeSettings.StartDateEditID)) {
					GridViewDataColumn startDateColumn = Columns[dateEditProperties.DateRangeSettings.StartDateEditID] as GridViewDataColumn;
					if(startDateColumn != null && startDateColumn.PropertiesEdit is DateEditProperties) {
						int requiredCalendarColumnCount = dateEditProperties.DateRangeSettings.CalendarColumnCount;
						((DateEditProperties)startDateColumn.PropertiesEdit).CalendarProperties.Columns = requiredCalendarColumnCount;
					}
				}
			}
		}
		protected override bool NeedLoadClientState() {
			return IsFirstLoad;
		}
		protected override ASPxGridViewDetailSettings CreateSettingsDetail() {
			return new MVCxGridViewDetailSettings();
		}
		protected override ASPxGridEditingSettings CreateSettingsEditing() {
			return new MVCxGridViewEditingSettings();
		}
		protected override GridRenderHelper CreateRenderHelper() {
			return new MVCxGridViewRenderHelper(this);
		}
		protected override ASPxGridBehaviorSettings CreateBehaviorSettings() {
			return new MVCxGridViewBehaviorSettings(this);
		}
		protected override GridEndlessPagingHelper CreateEndlessPagingHelper() {
			return new MVCxGridViewEndlessPagingHelper(this);
		}
		protected override GridBatchEditHelper CreateBatchEditHelper() {
			return new MVCxGridViewBatchEditHelper(this);
		}
		protected override ASPxGridSearchPanelSettings CreateSearchPanelSettings() {
			return new MVCxGridViewSearchPanelSettings(this);
		}
		protected override void CBUpdateEdit(string[] args) {
			LayoutChanged();
			if(RenderHelper.AllowBatchEditing)
				BatchEditHelper.LoadValidationErrorsFromModelState();
			if(DataProxy.ValidateRow(false) && ModelState != null && ModelState.IsValid && string.IsNullOrEmpty(ErrorText)) {
				CancelEdit();
				DataProxy.DoOwnerDataBinding();
			}
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.UpdateEdit);
		}
		protected override void CBDeleteRow(string[] args) {
			LayoutChanged();
			DataProxy.DoOwnerDataBinding();
			object key = DataProxy.GetKeyValueFromScript(args[0]);
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.DeleteRow, key);
		}
		protected override void OnRowValidatingCore(ASPxGridDataValidationEventArgs e) {
			base.OnRowValidatingCore(e);
			RenderHelper.EditingErrorText = !string.IsNullOrEmpty(e.RowError) ? e.RowError : ErrorText;
		}
		protected override string OnCallbackException(Exception e) {
			if(!string.IsNullOrEmpty(ErrorText))
				e = new Exception(ErrorText, e);
			return OnCallbackExceptionInternal(e);
		}
		protected override void LoadGridControlState(Dictionary<string, object> clientState) {
			var loaded = IsGridStateLoaded;
			base.LoadGridControlState(clientState);
			if(!loaded)
				LoadCustomOperationState();
		}
		void LoadCustomOperationState() {
			if(!EnableCustomOperations)
				return;
			PageIndex = CustomOperationViewModel.Pager.PageIndex;
			PageSize = CustomOperationViewModel.Pager.PageSize;
			FilterExpression = CustomOperationViewModel.FilterExpression;
			FilterEnabled = CustomOperationViewModel.IsFilterApplied;
			foreach(var columnState in CustomOperationViewModel.SortedColumns) {
				var column = Columns[columnState.FieldName] as MVCxGridViewColumn;
				if(column != null)
					column.AssignState(columnState);
			}
			foreach(var columnState in CustomOperationViewModel.Columns) {
				var column = Columns[columnState.FieldName] as MVCxGridViewColumn;
				if(column != null && CustomOperationViewModel.SortedColumns.IndexOf(columnState) == -1) 
					column.AssignState(columnState);
			}
			BuildSortedColumns();
		}
		protected override bool IsTotalSummaryHasChanges() {
			if(EnableCustomOperations) return false;
			return base.IsTotalSummaryHasChanges();
		}
		protected override void DoCallBackPostBack(string eventArgument) {
			if(EnableCustomOperations && PageIndex != CustomOperationViewModel.Pager.PageIndex)
				PageIndex = CustomOperationViewModel.Pager.PageIndex;
			base.DoCallBackPostBack(eventArgument);
		}
		protected override void CBExpandRow(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBExpandRow(args);
		}
		protected override void CBCollapseRow(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBCollapseRow(args);
		}
		protected override void CBExpandAll(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBExpandAll(args);
		}
		protected override void CBCollapseAll(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBCollapseAll(args);
		}
		protected override void CBNextPage(string[] args) {
			if(EnableCustomOperations) {
				PageIndex = CustomOperationViewModel.Pager.PageIndex;
				return;
			}
			base.CBNextPage(args);
		}
		protected override void CBPrevPage(string[] args) {
			if(EnableCustomOperations) {
				PageIndex = CustomOperationViewModel.Pager.PageIndex;
				return;
			}
			base.CBPrevPage(args);
		}
		protected override void CBGotoPage(string[] args) {
			if(EnableCustomOperations) {
				PageIndex = CustomOperationViewModel.Pager.PageIndex;
				return;
			}
			base.CBGotoPage(args);
		}
		protected override void CBPagerOnClick(string[] args) {
			if(EnableCustomOperations) {
				PageIndex = CustomOperationViewModel.Pager.PageIndex;
				return;
			}
			base.CBPagerOnClick(args);
		}
		protected override void CBApplyFilter(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBApplyFilter(args);
		}
		protected override void CBApplyColumnFilter(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBApplyColumnFilter(args);
		}
		protected override void CBApplyHeaderColumnFilter(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBApplyHeaderColumnFilter(args);
		}
		protected override void CBFilterRowMenu(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBFilterRowMenu(args);
		}
		protected override void CBApplyMultiColumnAutoFilter(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBApplyMultiColumnAutoFilter(args);
		}
		protected override void CBSetFilterEnabled(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBSetFilterEnabled(args);
		}
		protected override void CBSort(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBSort(args);
		}
		protected override void CBGroup(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBGroup(args);
		}
		protected override void ColumnMoveCore(GridViewColumn column, GridViewColumn moveToColumn, GridViewDataColumn dataColumn, GridViewDataColumn moveToDataColumn,
			bool moveBefore, bool moveToGroup, bool moveFromGroup) {
			if(EnableCustomOperations && moveToGroup)
				return;
			if(EnableCustomOperations && moveFromGroup) {
				column.VisibleIndex = GetMovedColumnNewVisibleIndex(column, moveToColumn, moveBefore);
				return;
			}
			base.ColumnMoveCore(column, moveToColumn, dataColumn, moveToDataColumn, moveBefore, moveToGroup, moveFromGroup);
		}
		protected internal static Hashtable GetGridClientObjectBatchState(string name, Func<string, string> getRequestValueMethod) {
			Hashtable state = LoadClientObjectState(getRequestValueMethod(name));
			return GetClientObjectStateValue<Hashtable>(state, GridClientStateProperties.BatchEditClientModifiedValues);
		}
		protected internal static string GetCustomOperationState(string gridName) {
			Hashtable clientState = GetGridClientObjectState(gridName);
			return GetClientObjectStateValueString(clientState, MVCxGridViewRenderHelper.CustomOperationStateKey);
		}
		static Hashtable GetGridClientObjectState(string name) {
			HttpRequest request = HttpContext.Current.Request;
			string state = request.Cookies.AllKeys.Contains(name) ? Utils.GetFormCollectionValue(name) : HttpUtils.GetValueFromRequest(name);
			return LoadClientObjectState(state);
		}
		#region IViewContext Members
		ViewContext IViewContext.ViewContext { get { return ViewContext; } }
		#endregion
		#region IGridAdapterOwner Members
		Func<string, string> IGridAdapterOwner.GetEditorIdByFieldName { get { return GetEditorIdByFieldName; } }
		bool IGridAdapterOwner.ShowModelErrorsForEditors { get { return SettingsEditing.ShowModelErrorsForEditors; } }
		bool IGridAdapterOwner.EnableCustomOperations { get { return EnableCustomOperations; } }
		GridBaseViewModel IGridAdapterOwner.CustomOperationViewModel { get { return CustomOperationViewModel; } }
		IDictionary<string, string> IGridAdapterOwner.CallbackActionUrlCollection { get { return CallbackActionUrlCollection; } }
		GridAdapter IGridAdapterOwner.Adapter { get { return GridAdapter; } }
		ModelStateDictionary IGridAdapterOwner.ModelState { get { return ModelState; } }
		#endregion
	}
}
namespace DevExpress.Web.Mvc.Internal {
	using System.Collections;
	using System.Globalization;
	using System.IO;
	using System.Text.RegularExpressions;
	using DevExpress.Web;
	using DevExpress.Web.Data;
	using DevExpress.Web.FilterControl;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Rendering;
	public class MVCxGridViewRenderHelper: GridViewRenderHelper {
		public const string CustomOperationStateKey = "customOperationState";
		public MVCxGridViewRenderHelper(MVCxGridView grid)
			: base(grid) {
		}
		public new MVCxGridView Grid { get { return (MVCxGridView)base.Grid; } }
		protected ModelStateDictionary ModelState { get { return Grid.ModelState; } }
		public override string CustomSearchPanelEditorClientID {
			get { return Grid.SettingsSearchPanel.CustomEditorID; }
		}
		public override void ApplyEditorSettings(ASPxEditBase baseEditor, IWebGridDataColumn column) {
			base.ApplyEditorSettings(baseEditor, column);
			Grid.GridAdapter.ApplyEditorValidationSettings(baseEditor as ASPxEdit, column);
		}
	}
	public class MVCxGridViewDataProxy: WebDataProxy {
		public MVCxGridViewDataProxy(IWebDataOwner owner, IWebControlPageSettings pageSettings, IWebDataEvents events)
			: base(owner, pageSettings, events) {
		}
		protected internal new IWebDataOwner Owner { get { return base.Owner; } }
		protected internal IGridAdapterOwner Grid { get { return (IGridAdapterOwner)Owner; } }
		protected internal GridBaseCustomOperationHelper CustomOperationHelper { get { return CustomOperationState.CustomOperationHelper; } }
		protected GridBaseViewModel CustomOperationState { get { return Grid.CustomOperationViewModel; } }
		protected internal bool EnableCustomOperations { get { return Grid.EnableCustomOperations; } }
		protected internal Type DataRowType { get { return forcedDataRowType; } }
		protected internal new bool PageSizeShowAllItem { get { return base.PageSizeShowAllItem; } }
		protected internal new IWebDataEvents Events { get { return base.Events; } }
		public override List<object> GetSelectedValues(string[] fieldNames) {
			if(EnableCustomOperations && fieldNames.Any(f => !KeyFieldNames.Contains(f)))
				throw new Exception("Values of non key fields cannot be accessed and returned in custom data binding mode. Only values of the key field(s) are accessible. So, specify only key field name(s) in calls to methods returning values of selected rows.");
			return base.GetSelectedValues(fieldNames);
		}
		protected override void CreateWebDataControllerProvider() {
			if(!EnableCustomOperations) {
				base.CreateWebDataControllerProvider();
				return;
			}
			var provider = DataProvider as GridViewCustomOperationsProvider;
			if(provider == null)
				SetDataProvider(CreateCustomOperationsProvider(), false);
		}
		protected virtual GridViewCustomOperationsProvider CreateCustomOperationsProvider() {
			return new GridViewCustomOperationsProvider(this);
		}
		protected internal new void DoOwnerDataBinding() {
			base.DoOwnerDataBinding();
		}
		protected internal void CustomOperationChanged() {
			CreateWebDataControllerProvider();
		}
		internal string GetExpadedState() {
			if(!EnableCustomOperations)
				return string.Empty;
			return CustomOperationHelper.ExpandedState.Save();
		}
	}
	public class MVCxGridViewContainerControl : GridViewUpdatableContainer {
		public MVCxGridViewContainerControl(MVCxGridView grid)
			: base(grid) {
		}
		public new MVCxGridView Grid { get { return (MVCxGridView)base.Grid; } }
		protected override WebFilterControlPopup CreateFilterControlPopup() {
			return new MVCxWebFilterControlPopup(Grid);
		}
		protected internal new MVCxWebFilterControlPopup PopupFilterControlForm {
			get { return (MVCxWebFilterControlPopup)base.PopupFilterControlForm; }
		}
	}
	public class MVCxGridViewEndlessPagingHelper : GridViewEndlessPagingHelper {
		public MVCxGridViewEndlessPagingHelper(MVCxGridView grid)
			: base(grid) {
		}
		public new MVCxGridView Grid { get { return (MVCxGridView)base.Grid; } }
		protected override void RenderGridItems(IGridEndlessPagingItemsContainer itemsContainer, IEnumerable<int> excludedIndices, HtmlTextWriter writer) {
			TextWriter writerOfView = Grid.ViewContext.Writer;
			Grid.ViewContext.Writer = writer;
			base.RenderGridItems(itemsContainer, excludedIndices, writer);
			Grid.ViewContext.Writer = writerOfView;
		}
	}
	public class MVCxGridViewBatchEditHelper : GridViewBatchEditHelper {
		public MVCxGridViewBatchEditHelper(MVCxGridView grid)
			: base(grid) {
			BatchEditHelperAdapter = new MVCxGridBatchEditHelperAdapter(this);
		}
		public GridAdapter GridAdapter { get { return (Grid as IGridAdapterOwner).Adapter; } }
		protected MVCxGridBatchEditHelperAdapter BatchEditHelperAdapter { get; private set; }
		public void LoadValidationErrorsFromModelState() {
			BatchEditHelperAdapter.LoadValidationErrorsFromModelState();
		}
		protected override bool HasEditItemTemplate(IWebGridDataColumn column) {
			var mvcColumn = column as MVCxGridViewColumn;
			if(mvcColumn == null) return false;
			return mvcColumn.EditItemTemplateContentMethod != null || !string.IsNullOrEmpty(mvcColumn.EditItemTemplateContent);
		}
		protected override void ApplyEditorSettings(ASPxEditBase editBase, IWebGridDataColumn column) {
			base.ApplyEditorSettings(editBase, column);
			GridAdapter.ApplyEditorValidationSettings(editBase as ASPxEdit, column);
		}
	}
	public class MVCxGridBatchEditHelperAdapter {
		public const string
			MVCBatchEditingKeyFieldName = "DXMVCBatchEditingKeyFieldName",
			MVCBatchEditingValuesRequestKey = "DXMVCBatchEditingValuesRequestKey";
		const string
			InsertValueFormat = "Insert[{0}].{1}",
			UpdateValueFormat = "Update[{0}].{1}",
			DeleteKeyFormat = "DeleteKeys[{0}]",
			InsertEditorErrorSearchPattern = "^Insert\\[(\\d+)\\]\\.(.+)",
			UpdateEditorErrorSearchPattern = "^Update\\[(\\d+)\\]\\.(.+)",
			InsertRowErrorFormat = "InsertRE[{0}]",
			UpdateRowErrorFormat = "UpdateRE[{0}]",
			DeleteRowErrorFormat = "DeleteRE[{0}]",
			InsertRowErrorSearchPattern = "^InsertRE\\[(\\d+)\\]$",
			UpdateRowErrorSearchPattern = "^UpdateRE\\[(\\d+)\\]$",
			DeleteRowErrorSearchPattern = "^DeleteRE\\[(\\d+)\\]$";
		public MVCxGridBatchEditHelperAdapter(GridBatchEditHelper owner) {
			Owner = owner;
		}
		GridBatchEditHelper Owner { get; set; }
		ModelStateDictionary ModelState { get { return ((IGridAdapterOwner)Owner.Grid).ModelState; } }
		protected Dictionary<string, Dictionary<IWebGridDataColumn, string>> EditorValidationErrors { get { return Owner.EditorValidationErrors; } }
		protected Dictionary<string, string> RowValidationErrors { get { return Owner.RowValidationErrors; } }
		protected Dictionary<int, Dictionary<string, string>> ClientInsertState { get { return Owner.ClientInsertState; } }
		protected Dictionary<string, Dictionary<string, string>> ClientUpdateState { get { return Owner.ClientUpdateState; } }
		protected List<int> InsertedRowIndices { get { return Owner.InsertedRowIndices; } }
		protected List<string> ClientDeleteState { get { return Owner.ClientDeleteState; } }
		protected List<object> UpdatedRowKeys { get { return Owner.UpdatedRowKeys; } }
		protected List<object> DeletedRowKeys { get { return Owner.DeletedRowKeys; } }
		public void LoadValidationErrorsFromModelState() {
			LoadEditorErrorsFromModelState();
			LoadRowErrorsFromModelState();
			LoadUpdatedState();
		}
		protected virtual void LoadUpdatedState() {
			var invalidRowKeys = EditorValidationErrors.Keys.Concat(RowValidationErrors.Keys).Distinct().ToList();
			foreach(var key in ClientInsertState.Keys) {
				if(!invalidRowKeys.Contains(key.ToString()))
					InsertedRowIndices.Add(key);
			}
			foreach(var key in ClientUpdateState.Keys) {
				if(!invalidRowKeys.Contains(key))
					UpdatedRowKeys.Add(key);
			}
			foreach(var key in ClientDeleteState) {
				if(!invalidRowKeys.Contains(key))
					DeletedRowKeys.Add(key);
			}
		}
		protected virtual void LoadRowErrorsFromModelState() {
			LoadRowErrorStateCore(ClientInsertState.Keys.OrderBy(k => k).ToList(), GetInsertRowErrors(ModelState));
			LoadRowErrorStateCore(ClientUpdateState.Keys.OrderBy(k => k).ToList(), GetUpdateRowErrors(ModelState));
			LoadRowErrorStateCore(ClientDeleteState.OrderBy(k => k).ToList(), GetDeleteRowErrors(ModelState));
		}
		protected virtual void LoadEditorErrorsFromModelState() {
			LoadEditorErrorsCore(ClientInsertState.Keys.OrderBy(k => k).ToList(), InsertEditorErrorSearchPattern);
			LoadEditorErrorsCore(ClientUpdateState.Keys.OrderBy(k => k).ToList(), UpdateEditorErrorSearchPattern);
		}
		protected virtual void LoadRowErrorStateCore(IList sortedClientKeys, Dictionary<int, string> rowErrors) {
			if(sortedClientKeys.Count == 0) return;
			foreach(var pair in rowErrors) {
				var modelStateIndex = pair.Key;
				if(sortedClientKeys.Count <= modelStateIndex) continue;
				var key = sortedClientKeys[modelStateIndex].ToString();
				RowValidationErrors[key] = pair.Value;
			}
		}
		protected void LoadEditorErrorsCore(IList sortedClientKeys, string modelStateSearchPattern) {
			if(sortedClientKeys.Count == 0) return;
			var modelStateErrors = GetEditorErrorsCore(ModelState, modelStateSearchPattern);
			foreach(var pair in modelStateErrors) {
				var modelStateIndex = pair.Key;
				if(sortedClientKeys.Count <= modelStateIndex) continue;
				var editorErrors = GetEditorErrors(pair.Value);
				if(editorErrors.Count == 0) continue;
				var key = sortedClientKeys[modelStateIndex].ToString();
				EditorValidationErrors[key] = editorErrors;
			}
		}
		protected Dictionary<IWebGridDataColumn, string> GetEditorErrors(ModelStateDictionary rowState) {
			if(rowState.Count == 0) return null;
			var result = new Dictionary<IWebGridDataColumn, string>();
			foreach(var pair in rowState) {
				var fieldName = pair.Key;
				var column = Owner.ColumnHelper.FindColumnByKey(fieldName) as IWebGridDataColumn;
				if(column == null) continue;
				result[column] = pair.Value.GetErrorMessage();
			}
			return result;
		}
		public static void SetInsertRowErrorText(ModelStateDictionary modelState, int index, string errorText) {
			modelState.AddModelError(string.Format(InsertRowErrorFormat, index), errorText);
		}
		public static void SetUpdateRowErrorText(ModelStateDictionary modelState, int index, string errorText) {
			modelState.AddModelError(string.Format(UpdateRowErrorFormat, index), errorText);
		}
		public static void SetDeleteRowErrorText(ModelStateDictionary modelState, int index, string errorText) {
			modelState.AddModelError(string.Format(DeleteRowErrorFormat, index), errorText);
		}
		public static System.Web.Mvc.IValueProvider CreateValueProvider(ModelBindingContext bindingContext) {
			var stateRequestKey = bindingContext.ValueProvider.GetValue(MVCBatchEditingValuesRequestKey).AttemptedValue;
			var state = MVCxGridView.GetGridClientObjectBatchState(stateRequestKey, key => { return bindingContext.ValueProvider.GetValue(key).AttemptedValue; });
			var keyFieldName = bindingContext.ValueProvider.GetValue(MVCBatchEditingKeyFieldName).AttemptedValue;
			if(state == null || string.IsNullOrEmpty(keyFieldName))
				return null;
			return new NameValueCollectionValueProvider(CreateValueCollection(state, keyFieldName), CultureInfo.InvariantCulture);
		}
		public static NameValueCollection CreateValueCollection(Hashtable state, string keyFieldName) {
			var insertState = new Dictionary<int, Dictionary<string, string>>();
			var updateState = new Dictionary<string, Dictionary<string, string>>();
			var deleteState = new List<string>();
			GridBatchEditHelper.LoadClientState(state, null, insertState, updateState, deleteState);
			var collection = new NameValueCollection();
			PopulateInsertValues(collection, insertState);
			PopulateUpdateValues(collection, updateState, keyFieldName);
			PopulateDeleteKeys(collection, deleteState);
			return collection;
		}
		public static Dictionary<int, string> GetInsertRowErrors(ModelStateDictionary modelState) {
			return GetModelStateRowErrors(modelState, InsertRowErrorSearchPattern);
		}
		public static Dictionary<int, string> GetUpdateRowErrors(ModelStateDictionary modelState) {
			return GetModelStateRowErrors(modelState, UpdateRowErrorSearchPattern);
		}
		public static Dictionary<int, string> GetDeleteRowErrors(ModelStateDictionary modelState) {
			return GetModelStateRowErrors(modelState, DeleteRowErrorSearchPattern);
		}
		protected static Dictionary<int, string> GetModelStateRowErrors(ModelStateDictionary modelState, string searchPattern) {
			var result = new Dictionary<int, string>();
			foreach(var pair in modelState) {
				var state = pair.Value;
				if(state.Errors.Count == 0) continue;
				var match = Regex.Match(pair.Key, searchPattern);
				if(!match.Success || match.Groups.Count != 2) continue;
				var index = Convert.ToInt32(match.Groups[1].Value);
				result[index] = state.GetErrorMessage();
			}
			return result;
		}
		public static Dictionary<int, ModelStateDictionary> GetInsertEditorErrors(ModelStateDictionary modelState) {
			return GetEditorErrorsCore(modelState, InsertEditorErrorSearchPattern);
		}
		public static Dictionary<int, ModelStateDictionary> GetUpdateEditorErrors(ModelStateDictionary modelState) {
			return GetEditorErrorsCore(modelState, UpdateEditorErrorSearchPattern);
		}
		protected static Dictionary<int, ModelStateDictionary> GetEditorErrorsCore(ModelStateDictionary modelState, string searchPattern) {
			var result = new Dictionary<int, ModelStateDictionary>();
			foreach(var pair in modelState) {
				var state = pair.Value;
				if(state.Errors.Count == 0) continue;
				string fieldName;
				var index = GetItemIndex(pair.Key, searchPattern, out fieldName);
				if(index < 0) continue;
				if(!result.ContainsKey(index))
					result[index] = new ModelStateDictionary();
				var dict = result[index];
				result[index][fieldName] = state;
			}
			return result;
		}
		static int GetItemIndex(string stateKey, string regExPattern, out string fieldName) {
			fieldName = null;
			if(string.IsNullOrEmpty(stateKey)) 
				return -1;
			var match = Regex.Match(stateKey, regExPattern);
			if(!match.Success || match.Groups.Count != 3)
				return -1;
			fieldName = match.Groups[2].Value;
			return Convert.ToInt32(match.Groups[1].Value);
		}
		static void PopulateInsertValues(NameValueCollection collection, Dictionary<int, Dictionary<string, string>> insertState) {
			if(insertState.Count == 0) return;
			var sortedKeys = insertState.Keys.OrderBy(k => k).ToList();
			for(var i = 0; i < sortedKeys.Count; i++) {
				var rowValues = insertState[sortedKeys[i]];
				PopulateRowValues(collection, rowValues, i, InsertValueFormat);
			}
		}
		static void PopulateUpdateValues(NameValueCollection collection, Dictionary<string, Dictionary<string, string>> updateState, string keyFieldName) {
			if(updateState.Count == 0) return;
			var sortedKeys = updateState.Keys.OrderBy(k => k).ToList();
			for(var i = 0; i < sortedKeys.Count; i++) {
				var key = sortedKeys[i];
				var rowValues = updateState[key];
				PopulateKeyValues(rowValues, keyFieldName, key);
				PopulateRowValues(collection, rowValues, i, UpdateValueFormat);
			}
		}
		static void PopulateKeyValues(Dictionary<string, string> rowValues, string rawkeyFieldNames, string rawKeyValues) {
			if(string.IsNullOrEmpty(rawkeyFieldNames) || string.IsNullOrEmpty(rawKeyValues))
				return;
			string[] keyFielsNames = rawkeyFieldNames.Split(WebDataProxy.MultipleKeyFieldSeparator);
			string[] keyValues = rawKeyValues.Split(WebDataProxy.MultipleKeyValueSeparator);
			int count = Math.Min(keyFielsNames.Count(), keyValues.Count());
			for(int i = 0; i < count; i++) {
				string key = keyFielsNames[i];
				rowValues[key] = keyValues[i];
			}
		}
		static void PopulateRowValues(NameValueCollection collection, Dictionary<string, string> rowValues, int rowIndex, string keyFormat) {
			if(rowValues.Count == 0) return;
			foreach(var pair in rowValues)
				collection.Add(string.Format(keyFormat, rowIndex, pair.Key), pair.Value);
		}
		static void PopulateDeleteKeys(NameValueCollection collection, List<string> deleteState) {
			if(deleteState == null) return;
			var sortedKeys = deleteState.OrderBy(k => k).ToList();
			for(var i = 0; i < sortedKeys.Count; i++)
				collection.Add(string.Format(DeleteKeyFormat, i), sortedKeys[i]);
		}
	}
	public class MVCxGridDataHelper: GridDataHelper {
		public MVCxGridDataHelper(ASPxGridBase grid, string name)
			: base(grid, name) {
		}
		protected override bool CanBindOnEnsureDataBound { get { return RequiresDataBinding; } }
	}
}
