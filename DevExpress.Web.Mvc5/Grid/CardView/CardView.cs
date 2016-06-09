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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using DevExpress.Web.FilterControl;
using DevExpress.Web.Internal;
using DevExpress.Web.Rendering;
namespace DevExpress.Web.Mvc {
	using System;
	using System.Collections;
	using System.Text;
	using System.Web.Mvc;
	using System.Web.UI;
	using DevExpress.Web;
	using DevExpress.Web.Data;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.Web.Rendering;
	[ToolboxItem(false)]
	public class MVCxCardView: ASPxCardView, IGridAdapterOwner, IViewContext {
		IDictionary<string, string> callbackActionUrlCollection;
		CardViewModel customOperationViewModel;
		bool enableCustomOperation;
		ViewContext viewContext;
		public MVCxCardView()
			: this(null) {
		}
		protected internal MVCxCardView(ViewContext viewContext)
			: base() {
			GridAdapter = new GridAdapter(this);
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			callbackActionUrlCollection = new Dictionary<string, string>();
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public new CardViewImages Images { get { return base.Images; } }
		public new CardViewStyles Styles { get { return base.Styles; } }
		public new MVCxCardViewEditingSettings SettingsEditing { get { return (MVCxCardViewEditingSettings)base.SettingsEditing; } }
		protected internal new MVCxCardViewBatchEditHelper BatchEditHelper { get { return (GridBatchEditHelper)base.BatchEditHelper as MVCxCardViewBatchEditHelper; } }
		protected internal new MVCxGridViewDataProxy DataProxy { get { return (MVCxGridViewDataProxy)base.DataProxy; } }
		protected internal GridAdapter GridAdapter { get; private set; }
		protected internal ViewContext ViewContext { get { return viewContext; } }
		protected internal ControllerBase Controller { get { return (ViewContext != null) ? ViewContext.Controller : null; } }
		protected internal ModelStateDictionary ModelState {
			get {
				Controller controller = Controller as Controller;
				return (controller != null) ? controller.ModelState : null;
			}
		}
		protected internal bool EnableCustomOperations {
			get { return enableCustomOperation; }
			set {
				if(EnableCustomOperations == value)
					return;
				enableCustomOperation = value;
				DataProxy.CustomOperationChanged();
			}
		}
		protected internal virtual CardViewModel CustomOperationViewModel {
			get {
				if(customOperationViewModel == null)
					customOperationViewModel = new CardViewModel(this);
				return customOperationViewModel;
			}
		}
		protected internal IDictionary<string, string> CallbackActionUrlCollection { get { return callbackActionUrlCollection; } }
		protected internal string GetEditorIdByFieldName(string fieldName) {
			CardViewColumn cardViewColumn = Columns[fieldName] as CardViewColumn;
			return GetEditorClientIDByColumn(cardViewColumn);
		}
		string GetEditorClientIDByColumn(CardViewColumn column) {
			if(column == null)
				return null;
			string editorPrefix = ClientID;
			if(SettingsEditing.IsPopupEditForm)
				editorPrefix += "_" + CardViewRenderHelper.PopupEditFormID;
			if(!SettingsEditing.IsBatchEdit)
				editorPrefix += "_" + CardViewRenderHelper.CardLayoutID + EditingCardVisibleIndex;
			return string.Format("{0}_{1}", editorPrefix, RenderHelper.GetEditorId(column));
		}
		protected internal override bool IsCallBacksEnabled() {
			return true;
		}
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ClientID; }
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if(CustomActionRouteValues != null)
				stb.Append(localVarName + ".customActionUrl=\"" + Utils.GetUrl(CustomActionRouteValues) + "\";\n");
			if(SettingsEditing.AddNewCardRouteValues != null)
				stb.Append(localVarName + ".addNewItemUrl=\"" + Utils.GetUrl(SettingsEditing.AddNewCardRouteValues) + "\";\n");
			if(SettingsEditing.UpdateCardRouteValues != null)
				stb.Append(localVarName + ".updateItemUrl=\"" + Utils.GetUrl(SettingsEditing.UpdateCardRouteValues) + "\";\n");
			if(SettingsEditing.DeleteCardRouteValues != null)
				stb.Append(localVarName + ".deleteItemUrl=\"" + Utils.GetUrl(SettingsEditing.DeleteCardRouteValues) + "\";\n");
			if(SettingsEditing.BatchUpdateRouteValues != null)
				stb.Append(localVarName + ".batchUpdateUrl=\"" + Utils.GetUrl(SettingsEditing.BatchUpdateRouteValues) + "\";\n");
			if(!string.IsNullOrEmpty(KeyFieldName))
				stb.Append(localVarName + ".keyName=\"" + KeyFieldName + "\";\n");
			if(CallbackActionUrlCollection.Count > 0)
				stb.Append(localVarName + ".callbackActionUrlCollection=eval(\"" + HtmlConvertor.ToJSON(CallbackActionUrlCollection) + "\");\n");
		}
		protected override void InitializeClientObjectScript(StringBuilder stb, string localVarName, string clientID) {
			base.InitializeClientObjectScript(stb, localVarName, clientID);
			int clientEditingCardVisibleIndex = RenderHelper.UseEndlessPaging ? EndlessPagingHelper.ClientEditingRowVisibleIndex : EditingCardVisibleIndex;
			stb.Append(localVarName + ".editingItemVisibleIndex=" + clientEditingCardVisibleIndex + ";\n");
			if(IsEditing || SettingsEditing.Mode == CardViewEditingMode.Batch) {
				IEnumerable<string> editorNames = ColumnHelper.AllVisibleDataColumns.Select(c => GetEditorClientIDByColumn(c as CardViewColumn));
				GridAdapter.AppendUnobtrusiveRules(stb, editorNames);
			}
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientCardView";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxCardView), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxCardView), Utils.GridAdapterScriptResourceName);
			RegisterIncludeScript(typeof(MVCxCardView), Utils.CardViewScriptResourceName);
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new MVCxGridDataHelper(this, name);
		}
		public override bool IsLoading() {
			return false;
		}
		protected internal new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		protected override CardViewFormLayoutProperties CreateCardLayoutProperties() {
			return new MVCxCardViewFormLayoutProperties(this) { IsStandalone = false, DataOwner = this };
		}
		protected override GridFormLayoutProperties CreateEditFormLayoutProperties() {
			return new MVCxCardViewFormLayoutProperties(this);
		}
		protected override GridBatchEditHelper CreateBatchEditHelper() {
			return new MVCxCardViewBatchEditHelper(this);
		}
		protected override ASPxGridEditingSettings CreateSettingsEditing() {
			return new MVCxCardViewEditingSettings();
		}
		protected override ASPxGridSearchPanelSettings CreateSearchPanelSettings() {
			return new MVCxCardViewSearchPanelSettings(this);
		}
		protected override GridRenderHelper CreateRenderHelper() {
			return new MVCxCardViewRenderHelper(this);
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
			Settings.LayoutMode = CustomOperationViewModel.LayoutMode;
			SettingsPager.SettingsTableLayout.Assign(CustomOperationViewModel.Pager.SettingsTableLayout);
			SettingsPager.SettingsFlowLayout.Assign(CustomOperationViewModel.Pager.SettingsFlowLayout);
			FilterExpression = CustomOperationViewModel.FilterExpression;
			FilterEnabled = CustomOperationViewModel.IsFilterApplied;
			foreach(var columnState in CustomOperationViewModel.SortedColumns) {
				var column = Columns[columnState.FieldName] as MVCxCardViewColumn;
				if(column != null)
					column.AssignState(columnState);
			}
			foreach(var columnState in CustomOperationViewModel.Columns) {
				var column = Columns[columnState.FieldName] as MVCxCardViewColumn;
				if(column != null && CustomOperationViewModel.SortedColumns.IndexOf(columnState) == -1)
					column.AssignState(columnState);
			}
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = base.GetClientObjectState();
			if(EnableCustomOperations || (CallbackActionUrlCollection != null && CallbackActionUrlCollection.Count() > 0)) {
				result.Add(MVCxGridViewRenderHelper.CustomOperationStateKey, CustomOperationViewModel.Save());
			}
			return result;
		}
		protected internal override bool EnableRowsCache {
			get { return EnableCustomOperations ? false : base.EnableRowsCache; }
			set { base.EnableRowsCache = value; }
		}
		protected override void CBNextPage(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBNextPage(args);
		}
		protected override void CBPrevPage(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBPrevPage(args);
		}
		protected override void CBGotoPage(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBGotoPage(args);
		}
		protected override void CBPagerOnClick(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBPagerOnClick(args);
		}
		protected override void CBSort(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBSort(args);
		}
		protected override void CBApplyFilter(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBApplyFilter(args);
		}
		protected override void CBApplyHeaderColumnFilter(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBApplyHeaderColumnFilter(args);
		}
		protected override void CBSetFilterEnabled(string[] args) {
			if(EnableCustomOperations)
				return;
			base.CBSetFilterEnabled(args);
		}
		protected override void OnFilterExpressionChanging(string value, bool isFilterEnabled) {
			if(EnableCustomOperations) {
				value = CustomOperationViewModel.FilterExpression;
				isFilterEnabled = CustomOperationViewModel.IsFilterApplied;
			}
			base.OnFilterExpressionChanging(value, isFilterEnabled);
		}
		protected override GridUpdatableContainer CreateContainerControl() {
			return new MVCxCardViewContainerControl(this);
		}
		protected internal new MVCxCardViewContainerControl ContainerControl {
			get { return (MVCxCardViewContainerControl)base.ContainerControl; }
		}
		protected internal MVCxWebFilterControlPopup PopupFilterControlForm {
			get { return ContainerControl.PopupFilterControlForm; }
		}
		protected internal MVCxPopupFilterControl FilterControl {
			get { return (PopupFilterControlForm != null) ? PopupFilterControlForm.FilterControl : null; }
		}
		protected internal string ErrorText { get; set; }
		protected override void OnRowValidatingCore(ASPxGridDataValidationEventArgs e) {
			base.OnRowValidatingCore(e);
			RenderHelper.EditingErrorText = !string.IsNullOrEmpty(e.RowError) ? e.RowError : ErrorText;
		}
		protected override string OnCallbackException(Exception e) {
			if(!string.IsNullOrEmpty(ErrorText))
				e = new Exception(ErrorText, e);
			return OnCallbackExceptionInternal(e);
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
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.StartEdit);
		}
		protected override void CBDeleteRow(string[] args) {
			LayoutChanged();
			DataProxy.DoOwnerDataBinding();
			object key = DataProxy.GetKeyValueFromScript(args[0]);
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.DeleteRow, key);
		}
		protected internal bool IsDataCallback() {
			return InternalCallbackInfo != null;
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
		protected override WebDataProxy CreateDataProxy() {
			return new MVCxGridViewDataProxy(this, this, this);
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
	public class MVCxCardViewContainerControl : CardViewUpdatableContainer {
		public MVCxCardViewContainerControl(MVCxCardView grid)
			: base(grid) {
		}
		protected override WebFilterControlPopup CreateFilterControlPopup() {
			return new MVCxWebFilterControlPopup(Grid);
		}
		protected internal new MVCxWebFilterControlPopup PopupFilterControlForm {
			get { return (MVCxWebFilterControlPopup)base.PopupFilterControlForm; }
		}
	}
	public class MVCxCardViewRenderHelper : CardViewRenderHelper {
		public MVCxCardViewRenderHelper(MVCxCardView grid)
			: base(grid) {
		}
		public new MVCxCardView Grid { get { return (MVCxCardView)base.Grid; } }
		public override void ApplyEditorSettings(ASPxEditBase baseEditor, IWebGridDataColumn column) {
			base.ApplyEditorSettings(baseEditor, column);
			Grid.GridAdapter.ApplyEditorValidationSettings(baseEditor as ASPxEdit, column);
		}
		protected override void AddEditItemTemplateControlCore(int visibleIndex, CardViewColumn column, ITemplate template, CardViewColumnLayoutItem layoutItem) {
			base.AddEditItemTemplateControlCore(visibleIndex, column, template, layoutItem);
			RenderUtils.EnsureChildControlsRecursive(layoutItem.NestedControlContainer, true);
		}
		protected override void AddDataItemTemplateControlCore(CardViewColumnLayoutItem layoutItem, ITemplate template, CardViewDataItemTemplateContainer templateContainer) {
			base.AddDataItemTemplateControlCore(layoutItem, template, templateContainer);
			RenderUtils.EnsureChildControlsRecursive(layoutItem.NestedControlContainer, true);
		}
	}
	public class MVCxCardViewBatchEditHelper : CardViewBatchEditHelper {
		public MVCxCardViewBatchEditHelper(MVCxCardView grid)
			: base(grid) {
			BatchEditHelperAdapter = new MVCxGridBatchEditHelperAdapter(this);
		}
		public GridAdapter GridAdapter { get { return (Grid as IGridAdapterOwner).Adapter; } }
		protected MVCxGridBatchEditHelperAdapter BatchEditHelperAdapter { get; private set; }
		public void LoadValidationErrorsFromModelState() {
			BatchEditHelperAdapter.LoadValidationErrorsFromModelState();
		}
		protected override bool HasEditItemTemplate(IWebGridDataColumn column) {
			var mvcColumn = column as MVCxCardViewColumn;
			if(mvcColumn == null) return false;
			return mvcColumn.EditItemTemplateContentMethod != null || !string.IsNullOrEmpty(mvcColumn.EditItemTemplateContent);
		}
		protected override void ApplyEditorSettings(ASPxEditBase editBase, IWebGridDataColumn column) {
			base.ApplyEditorSettings(editBase, column);
			GridAdapter.ApplyEditorValidationSettings(editBase as ASPxEdit, column);
		}
	}
}
