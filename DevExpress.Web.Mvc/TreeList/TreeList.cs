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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Mvc {
	using System.Collections.Specialized;
	using System.Web;
	using DevExpress.Web.ASPxTreeList;
	using DevExpress.Web.ASPxTreeList.Internal;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	[ToolboxItem(false)]
	public class MVCxTreeList : ASPxTreeList {
		ViewContext viewContext;
		public MVCxTreeList()
			: this(null) {
		}
		protected internal MVCxTreeList(ViewContext viewContext)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			EditErrorText = string.Empty;
		}
		public new TreeListImages Images {
			get { return base.Images; }
		}
		public new TreeListStyles Styles {
			get { return base.Styles; }
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public object CustomDataActionRouteValues { get; set; }
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		public new MVCxTreeListSettings Settings {
			get { return (MVCxTreeListSettings)base.Settings; }
		}
		public new MVCxTreeListSettingsBehavior SettingsBehavior {
			get { return (MVCxTreeListSettingsBehavior)base.SettingsBehavior; }
		}
		public new MVCxTreeListSettingsCookies SettingsCookies {
			get { return (MVCxTreeListSettingsCookies)base.SettingsCookies; }
		}
		public new MVCxTreeListSettingsCustomizationWindow SettingsCustomizationWindow {
			get { return (MVCxTreeListSettingsCustomizationWindow)base.SettingsCustomizationWindow; }
		}
		public new MVCxTreeListSettingsEditing SettingsEditing {
			get { return (MVCxTreeListSettingsEditing)base.SettingsEditing; }
		}
		public new MVCxTreeListSettingsPopupEditForm SettingsPopupEditForm {
			get { return (MVCxTreeListSettingsPopupEditForm)base.SettingsPopupEditForm; }
		}
		public new MVCxTreeListSettingsDataSecurity SettingsDataSecurity {
			get { return (MVCxTreeListSettingsDataSecurity)base.SettingsDataSecurity; }
		}
		public new MVCxTreeListSettingsSelection SettingsSelection {
			get { return (MVCxTreeListSettingsSelection)base.SettingsSelection; }
		}
		public new MVCxTreeListSettingsText SettingsText {
			get { return (MVCxTreeListSettingsText)base.SettingsText; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new TreeListNode AppendNode(object keyObject) {
			return base.AppendNode(keyObject);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new TreeListNode AppendNode(object keyObject, TreeListNode parentNode) {
			return base.AppendNode(keyObject, parentNode);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void DeleteNode(string nodeKey) {
			base.DeleteNode(nodeKey);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void MoveNode(string nodeKey, string parentNodeKey) {
			base.MoveNode(nodeKey, parentNodeKey);
		}
		protected internal string EditErrorText { get; set; }
		protected internal bool IsFirstLoad { get { return !IsCallback; } }
		protected internal new int InitialPageSize { get { return base.InitialPageSize; } set { base.InitialPageSize = value; } }
		protected internal ViewContext ViewContext { get { return viewContext; } }
		protected internal ControllerBase Controller { get { return (ViewContext != null) ? ViewContext.Controller : null; } }
		protected internal ModelStateDictionary ModelState {
			get {
				Controller controller = Controller as Controller;
				return (controller != null) ? controller.ModelState : null;
			}
		}
		protected new internal MVCxTreeListDataHelper TreeDataHelper {
			get { return (MVCxTreeListDataHelper)base.TreeDataHelper; }
		}
		protected new System.Web.Mvc.IValueProvider PostDataCollection { get { return base.PostDataCollection as System.Web.Mvc.IValueProvider; } }
		protected internal bool IsPartialUpdatePossible() {
			return IsPartialCallbackPossible() && CommandToExecute.PartialUpdatePossible;
		}
		protected internal new void LayoutChanged() {
			base.LayoutChanged();
		}
		protected internal void PerformOnLoad() {
			LoadColumnData();
			FinalizeLoading(false);
		}
		void LoadColumnData() {
			foreach(TreeListColumn column in Columns) {
				TreeListDataColumn dataColumn = column as TreeListDataColumn;
				if(dataColumn == null)
					continue;
				DevExpress.Web.ComboBoxProperties prop = dataColumn.PropertiesEdit as DevExpress.Web.ComboBoxProperties;
				if(prop != null && prop.DataSource != null)
					EditorsIntegrationHelper.CheckInplaceBound(prop, prop.ValueType, this);
				DateEditProperties dateEditProperties = dataColumn.PropertiesEdit as DateEditProperties;
				if (dateEditProperties != null && !string.IsNullOrEmpty(dateEditProperties.DateRangeSettings.StartDateEditID)) {
					TreeListDataColumn startDateColumn = Columns[dateEditProperties.DateRangeSettings.StartDateEditID] as TreeListDataColumn;
					if(startDateColumn != null && startDateColumn.PropertiesEdit is DateEditProperties) {
						int requiredCalendarColumnCount = dateEditProperties.DateRangeSettings.CalendarColumnCount;
						((DateEditProperties)startDateColumn.PropertiesEdit).CalendarProperties.Columns = requiredCalendarColumnCount;
					}
				}
			}
		}
		protected internal string GetEditorID(TreeListDataColumn column) {
			return RenderHelper.GetEditorId(column);
		}
		protected internal new void EnsureClientStateLoaded() {
			base.EnsureClientStateLoaded();
		}
		protected override bool NeedLoadClientState() {
			return IsFirstLoad;
		}
		protected internal new void DoSort() {
			base.DoSort();
		}
		protected internal void SetEditingValues() {
			foreach(var fieldName in TreeDataHelper.Fields) {
				if(IsServiceFieldName(fieldName)) continue;
				var value = GetEditValueForField(fieldName);
				TreeDataHelper.SetEditingValue(fieldName, value);
			}
		}
		object GetEditValueForField(string fieldName){
			object value;
			ModelBindingContext bindingContext = CreateBindingContext(fieldName);
			if(ExtensionValueProvidersFactory.TryGetValue(bindingContext, out value))
				return value;
			ValueProviderResult valueProviderResult = PostDataCollection.GetValue(fieldName);
			return valueProviderResult != null ? valueProviderResult.AttemptedValue : null;
		}
		ModelBindingContext CreateBindingContext(string fieldName) {
			var bindingContext = new ModelBindingContext();
			bindingContext.ModelName = fieldName;
			bindingContext.ModelMetadata = GetMetadataForField(fieldName);
			bindingContext.ValueProvider = PostDataCollection;
			return bindingContext;
		}
		ModelMetadata GetMetadataForField(string fieldName) {
			if(DataSource == null || DataSource.GetType().GetGenericArguments().Length == 0)
				return ExtensionsHelper.GetMetadataByEditorName(fieldName, ViewContext.ViewData);
			var containerType = DataSource.GetType().GetGenericArguments().SingleOrDefault();
			return ModelMetadataProviders.Current.GetMetadataForProperty(() => null, containerType, fieldName);
		}
		protected override void RecreateNodes() {
			if(CommandToExecute is TreeListUpdateEditCommand && IsEditing) {
				TreeDataHelper.ResetEditing();
			}
			base.RecreateNodes();
		}
		protected override void DeleteNodeInternal(string nodeKey) {
			LayoutChanged();
		}
		protected override TreeListRenderHelper CreateRenderHelper() {
			return new MVCxTreeListRenderHelper(this);
		}
		protected override TreeListDataHelper CreateDataHelper() {
			return new MVCxTreeListDataHelper(this);
		}
		protected override Web.ASPxTreeList.TreeListSettings CreateSettings() {
			return new MVCxTreeListSettings(this);
		}
		protected override TreeListSettingsBehavior CreateBehaviorSettings() {
			return new MVCxTreeListSettingsBehavior(this);
		}
		protected override TreeListSettingsCustomizationWindow CreateCustomizationWindowSettings() {
			return new MVCxTreeListSettingsCustomizationWindow(this);
		}
		protected override TreeListSettingsSelection CreateSelectionSettings() {
			return new MVCxTreeListSettingsSelection(this);
		}
		protected override TreeListSettingsCookies CreateCookiesSettings() {
			return new MVCxTreeListSettingsCookies(this);
		}
		protected override TreeListSettingsEditing CreateEditingSettings() {
			return new MVCxTreeListSettingsEditing(this);
		}
		protected override TreeListSettingsPopupEditForm CreatePopupEditFormSettings() {
			return new MVCxTreeListSettingsPopupEditForm(this);
		}
		protected override TreeListSettingsDataSecurity CreateDataSecuritySettings() {
			return new MVCxTreeListSettingsDataSecurity(this);
		}
		protected override TreeListSettingsText CreateTextSettings() {
			return new MVCxTreeListSettingsText(this);
		}
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null;
		}
		protected internal bool IsDataCallback() {
			return CommandToExecute is TreeListCustomDataCallbackCommand || CommandToExecute is TreeListGetNodeValuesCommand;
		}
		protected override object GetCallbackResult() {
			Hashtable callbackResult = (Hashtable)base.GetCallbackResult();
			if(IsEditing) {
				callbackResult["editingParentKey"] = GetEditingNodeParentKey(); 
				if(!string.IsNullOrEmpty(EditErrorText))
					callbackResult["errorText"] = EditErrorText;
			}
			return callbackResult;
		}
		protected internal List<TableRow> GetPartialCallbackResult() {
			return ((MVCxTreeListRenderHelper)RenderHelper).GetRowsForPartialUpdateNodeRender(CommandToExecute.GetPartialUpdateNodeKey());
		}
		protected internal Control GetCallbackResultControl() {
			if(IsDataCallback())
				return null;
			EnsureChildControls();
			return GetUpdateableContainer();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if(CustomActionRouteValues != null)
				stb.Append(localVarName + ".customActionUrl=\"" + Utils.GetUrl(CustomActionRouteValues) + "\";\n");
			if(CustomDataActionRouteValues != null)
				stb.Append(localVarName + ".customDataActionUrl=\"" + Utils.GetUrl(CustomDataActionRouteValues) + "\";\n");
			if(SettingsEditing.AddNewNodeRouteValues != null)
				stb.Append(localVarName + ".addNewNodeUrl=\"" + Utils.GetUrl(SettingsEditing.AddNewNodeRouteValues) + "\";\n");
			if(SettingsEditing.UpdateNodeRouteValues != null)
				stb.Append(localVarName + ".updateNodeUrl=\"" + Utils.GetUrl(SettingsEditing.UpdateNodeRouteValues) + "\";\n");
			if(SettingsEditing.NodeDragDropRouteValues != null)
				stb.Append(localVarName + ".moveNodeUrl=\"" + Utils.GetUrl(SettingsEditing.NodeDragDropRouteValues) + "\";\n");
			if(SettingsEditing.DeleteNodeRouteValues != null)
				stb.Append(localVarName + ".deleteNodeUrl=\"" + Utils.GetUrl(SettingsEditing.DeleteNodeRouteValues) + "\";\n");
			if(!string.IsNullOrEmpty(KeyFieldName))
				stb.Append(localVarName + ".keyName=\"" + KeyFieldName + "\";\n");
			if(!string.IsNullOrEmpty(ParentFieldName)) {
				stb.Append(localVarName + ".parentKeyName=\"" + ParentFieldName + "\";\n");
				if(IsEditing)
					stb.Append(localVarName + ".editingParentKey=\"" + GetEditingNodeParentKey() + "\";\n");
			}
			var validationRules = GetColumnUnobtrusiveValidationRules();
			if(validationRules.Count > 0)
				stb.Append(localVarName + ".unobtrusiveValidationRules=" + HtmlConvertor.ToJSON(validationRules) + ";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientTreeList";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxTreeList), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxTreeList), Utils.TreeListScriptResourceName);
		}
		protected string GetEditingNodeParentKey() {
			if(TreeDataHelper.EditingKey == null)
				return TreeDataHelper.NewNodeParentKey;
			var editingNode = FindNodeByKeyValue(TreeDataHelper.EditingKey);
			return editingNode != null ? editingNode.ParentNode.Key : null;
		}
		protected override NameValueCollection GetDefaultPostDataCollection() {
			var valueProvider = Controller != null ? Controller.ValueProvider : null;
			return valueProvider != null ? new MvcPostDataCollection(valueProvider) : Request.Params;
		}
		Dictionary<string, object> GetColumnUnobtrusiveValidationRules() {
			Dictionary<string, object> rules = new Dictionary<string, object>();
			foreach(TreeListColumn column in Columns) {
				TreeListDataColumn dataColumn = column as TreeListDataColumn;
				if(!column.Visible || dataColumn == null) continue;
				IDictionary<string, object> editorRules = ExtensionsHelper.GetUnobtrusiveValidationRulesForColumnEditor(dataColumn.FieldName, ViewContext, GetEditorIdByFieldName);
				if(editorRules != null && editorRules.Count > 0)
					rules[dataColumn.FieldName] = editorRules;
			}
			return rules;
		}
		string GetEditorIdByFieldName(string fieldName) {
			TreeListDataColumn dataColumn = Columns[fieldName] as TreeListDataColumn;
			return GetEditorClientIDByColumn(dataColumn);
		}
		string GetEditorClientIDByColumn(TreeListDataColumn column) {
			if(column == null)
				return null;
			string editorPrefix = ClientID;
			if(SettingsEditing.IsPopupEditForm)
				editorPrefix += "_" + TreeListRenderHelper.PopupEditFormID;
			return string.Format("{0}_{1}", editorPrefix, RenderHelper.GetEditorId(column));
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web;
	using DevExpress.Web.ASPxTreeList;
	using DevExpress.Web.ASPxTreeList.Internal;
	public class MVCxTreeListDataHelper : TreeListDataHelper {
		public MVCxTreeListDataHelper(MVCxTreeList treeList)
			: base(treeList) {
		}
		public override string DefaultEditingNodeError { get { return ((MVCxTreeList)TreeList).EditErrorText; } }
		public IEnumerable<string> Fields { get { return FieldTypes.Keys; } }
		public ModelStateDictionary ModelState { get { return ((MVCxTreeList)TreeList).ModelState; } }
		protected internal new Dictionary<string, Type> FieldTypes { get { return base.FieldTypes; } }		
		public override bool MoveNode(string key, string parentKey) {
			return false;
		}
		public override bool DoNodeValidation() {
			return base.DoNodeValidation() && string.IsNullOrEmpty(DefaultEditingNodeError);
		}
		public new void ResetEditing() {
			base.ResetEditing();
		}
		protected override void DoInsertNodeData() {
		}
		protected override void DoUpdateNodeData() {
		}
		protected override void SetColumnErrors(Dictionary<string, string> errors) {
			if(((MVCxTreeList)TreeList).SettingsEditing.ShowModelErrorsForEditors && HasModelErrors) {
				foreach(string key in EditingValues.Keys) {
					if(!errors.ContainsKey(key) && !ModelState.IsValidField(key)) {
						string error = ModelState[key].GetErrorMessage();
						if(!string.IsNullOrEmpty(error))
							errors.Add(key, error);
					}
				}
			}
			base.SetColumnErrors(errors);
		}
		bool HasModelErrors { get { return ModelState != null && !ModelState.IsValid; } }
	}
	public class MVCxTreeListRenderHelper : TreeListRenderHelper {
		public MVCxTreeListRenderHelper(MVCxTreeList treeList)
			: base(treeList) {
		}
		public new MVCxTreeList TreeList { get { return (MVCxTreeList)base.TreeList; } }
		public override bool IsNodeDragDropEnabled { 
			get { return TreeList.SettingsEditing.NodeDragDropRouteValues != null; } 
		}
		protected override bool CanRenderStyleTable { get { return true; } }
		public override string GetFullRenderResult() {
			return Utils.CallbackHtmlContentPlaceholder;
		}
		public override string GetPartialRenderResult(string nodeKey) {
			return Utils.CallbackHtmlContentPlaceholder;
		}
		public new List<TableRow> GetRowsForPartialUpdateNodeRender(string nodeKey) {
			return base.GetRowsForPartialUpdateNodeRender(nodeKey);
		}
		protected override void PrepareEditor(ASPxEditBase baseEdit, TreeListDataColumn column, int columnSpan) {
			base.PrepareEditor(baseEdit, column, columnSpan);
			var editor = baseEdit as ASPxEdit;
			if(editor != null)
				ExtensionsHelper.AssignValidationSettingsToColumnEditor(editor, column.FieldName, TreeList.ViewContext, TreeList.SettingsEditing.ShowModelErrorsForEditors);
		}
	}
	public class MVCxTreeListCustomDataCallbackCommand : TreeListCustomDataCallbackCommand {
		public MVCxTreeListCustomDataCallbackCommand(string args)
			: base(null, args) {
		}
		public IDictionary GetCustomDataCallbackResult(object data) {
			return CreateCustomDataCallbackResult(-1, EventArgs.Argument, data);
		}
	}
}
