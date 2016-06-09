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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors.ASPx.GridListEditor.FilterControl;
using DevExpress.ExpressApp.Web.Localization;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Web;
using DevExpress.Web.Data;
using DevExpress.Web.Rendering;
using DevExpress.XtraPrinting;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxGridListEditor : ComplexWebListEditor, IControlOrderProvider, ITestable, ITestableContainer, IDetailFramesContainer, ISupportModelSaving, ISupportNewItemRowPosition, ISupportCallbackStartupScriptRegistering, IDataItemTemplateInfoProvider, ISupportSelectionOperations, IExportable, ICustomRenderUpdatePanel, ISupportFooter, ISupportFilter, ISupportPager, ISupportAppearanceCustomization, ISupportEnabledCustomization, IDataAwareExportableCsv, IDataAwareExportableXls, IDataAwareExportableXlsx, IGridBatchOperationProvider {
		public const string GroupSummary = "GroupSummary";
		public const string EndCallbackHandlers = "cpEndCallbackHandlers";
		private const string InlineEditColumnPropertyName = "InlineEditCommandColumn";
		private const bool UseASPxGridViewDataSpecificColumnsDefault = true;
		private static bool useASPxGridViewDataSpecificColumns = UseASPxGridViewDataSpecificColumnsDefault;
		private const bool AllowFilterControlHierarchyDefault = true;
		private ASPxGridViewSupportCallbackStartupScriptRegisteringImpl startupScriptRegisteringImpl;
		private ASPxGridView grid;
		private ASPxGridViewExporter aspxGridViewExporter;
		private ASPxGridViewContextMenu contextMenu;
		private ASPxGridViewFilterControlColumnsHelper filterControlHelper;
		private object newObject;
		private object editingObject;
		private NewItemRowPosition newItemRowPosition;
		private bool needRebindData;
		private bool isSelectable;
		private string errorText;
		private bool enableGroupTemplate = false;
		private bool needGetUpdatePanelsRenderResults = false;
		private Dictionary<WebColumnBase, IColumnInfo> columnsInfoCache = new Dictionary<WebColumnBase, IColumnInfo>();
		private ASPxGridBatchModeHelper batchHelper;
		private ASPxGridDetailFramesManager detailFramesManager;
		private bool displayActionColumns = true;
		private bool? isAdaptive = null;
		[DefaultValue(false)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool UseProtectedContentColumn { get; set; }
		public bool IsAdaptive {
			get {
				if(isAdaptive == null) {
					isAdaptive = WebApplicationStyleManager.IsNewStyle;
				}
				return isAdaptive ?? false;
			}
			set {
				isAdaptive = value;
			}
		}
		static ASPxGridListEditor() {
			AllowFilterControlHierarchy = AllowFilterControlHierarchyDefault;
		}
#if DebugTest
		public ASPxGridListEditor() : base(null) { }
#endif
		public ASPxGridListEditor(IModelListView info)
			: base(info) {
			CanManageFilterExpressionDisplayText = true;
			contextMenu = new ASPxGridViewContextMenu(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual ModelLayoutElementCollection GetLayoutElementCollection(IModelListView listViewModel) {
			if(!listViewModel.BandsLayout.Enable) {
				return new ModelLayoutElementCollection(listViewModel.Columns);
			}
			else {
				return new ModelLayoutElementCollection(new ModelBandLayoutItemCollection(listViewModel));
			}
		}
		private bool IsKeyFieldNameAllowed() {
			return (CollectionSource != null) &&
			CollectionSource.IsLoaded &&
			(CollectionSource.Collection != null) &&
			(
				!CollectionSource.List.IsReadOnly
				||
				(CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.Server)
				||
				(CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView)
			) &&
			(KeyMembersInfo.Count > 0);
		}
		private void UpdateIsSelectable() {
			isSelectable = IsKeyFieldNameAllowed() && CanConvertKeyMembersInfoFromToString();
		}
		private void UpdateKeyFieldName() {
			if(Grid != null) {
				if(IsKeyFieldNameAllowed()) {
					String keyFieldName = "";
					foreach(IMemberInfo keyMemberInfo in KeyMembersInfo) {
						keyFieldName = keyFieldName + keyMemberInfo.Name + ";";
					}
					Grid.KeyFieldName = keyFieldName.TrimEnd(';');
				}
				else {
					Grid.KeyFieldName = "";
				}
			}
		}
		private void CancelEdit() {
			if(Grid != null && Grid.IsEditing) {
				Grid.CancelEdit();
			}
		}
		public virtual void RemoveColumn(GridViewDataColumn column) {
			if(Grid != null) {
				Grid.Columns.Remove(column);
			}
			columnsInfoCache.Remove(column);
		}
		public override void RemoveColumn(ColumnWrapper columnWrapper) {
			base.RemoveColumn(columnWrapper);
			if(columnWrapper is ASPxGridViewColumnWrapper) {
				RemoveColumn(((ASPxGridViewColumnWrapper)columnWrapper).Column);
			}
		}
		private void SubscribeColumnFactoryEvents(ASPxGridViewColumnFactoryBase columnFactory) {
			if(CreateCustomGridViewDataColumn != null) {
				columnFactory.CreateCustomGridViewDataColumn += columnFactory_CreateCustomGridViewDataColumn;
			}
			if(CreateCustomDataItemTemplate != null) {
				columnFactory.CreateCustomDataItemTemplate += columnFactory_CreateCustomDataItemTemplate;
			}
			if(CreateCustomEditItemTemplate != null) {
				columnFactory.CreateCustomEditItemTemplate += columnFactory_CreateCustomEditItemTemplate;
			}
			if(CustomizeGridViewDataColumn != null) {
				columnFactory.CustomizeGridViewDataColumn += columnFactory_CustomizeGridViewDataColumn;
			}
			columnFactory.CreateCustomGridViewDataColumnCore += columnFactory_CreateCustomGridViewDataColumnCore;
		}
		private void UnsubscribeColumnFactoryEvents(ASPxGridViewColumnFactoryBase columnFactory) {
			columnFactory.CreateCustomGridViewDataColumn -= columnFactory_CreateCustomGridViewDataColumn;
			columnFactory.CreateCustomDataItemTemplate -= columnFactory_CreateCustomDataItemTemplate;
			columnFactory.CreateCustomEditItemTemplate -= columnFactory_CreateCustomEditItemTemplate;
			columnFactory.CustomizeGridViewDataColumn -= columnFactory_CustomizeGridViewDataColumn;
			columnFactory.CreateCustomGridViewDataColumnCore -= columnFactory_CreateCustomGridViewDataColumnCore;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		internal void PopulateColumns() {
			ASPxGridViewColumnFactoryBase columnFactory = CreateASPxGridViewColumnFactory();
			try {
				SubscribeColumnFactoryEvents(columnFactory);
				DataItemTemplateFactoryWrapper templateFactory = new DataItemTemplateFactoryWrapper(DataItemTemplateFactory, Application, ObjectSpace, ObjectTypeInfo, this);
				Dictionary<GridViewColumn, IColumnInfo> result = columnFactory.PopulateColumns(GetLayoutElementCollection(Model), Grid, Model.DataAccessMode, templateFactory, NeedCreateEditItemTemplate);
				foreach(KeyValuePair<GridViewColumn, IColumnInfo> columnInfo in result) {
					columnsInfoCache.Add(columnInfo.Key, columnInfo.Value);
				}
			}
			finally {
				UnsubscribeColumnFactoryEvents(columnFactory);
			}
			if(contextMenu != null) {
				contextMenu.CreateControls();
			}
		}
		protected virtual ASPxGridViewColumnFactoryBase CreateASPxGridViewColumnFactory() {
			if(Model != null && !Model.BandsLayout.Enable) {
				return new ASPxGridViewColumnFactory();
			}
			else {
				return new ASPxGridViewBandsColumnFactory();
			}
		}
		protected override ColumnWrapper AddColumnCore(IModelColumn columnInfo) {
			if(UseASPxGridViewDataSpecificColumns) {
				ASPxGridViewColumnFactoryBase columnFactory = CreateASPxGridViewColumnFactory();
				try {
					SubscribeColumnFactoryEvents(columnFactory);
					DataItemTemplateFactoryWrapper templateFactory = new DataItemTemplateFactoryWrapper(DataItemTemplateFactory, Application, ObjectSpace, ObjectTypeInfo, this);
					ColumnInfoWrapper columnInfoWrapper = columnFactory.AddColumn(columnInfo, Grid, Model.DataAccessMode, templateFactory, NeedCreateEditItemTemplate);
					columnsInfoCache.Add(columnInfoWrapper.Column, columnInfoWrapper.GridViewColumnInfo);
					return columnInfoWrapper.GridViewColumnInfo.CreateColumnWrapper(columnInfoWrapper.Column);
				}
				finally {
					UnsubscribeColumnFactoryEvents(columnFactory);
				}
			}
			else {
				GridViewDataColumn column = CreateColumn(columnInfo);
				GridViewDataColumnInfo gridViewDataColumnInfo = CreateGridViewDataColumnInfo(columnInfo, Model.DataAccessMode, HasProtectedContent(columnInfo.PropertyName));
				gridViewDataColumnInfo.SetupColumn(column);
				columnsInfoCache.Add(column, gridViewDataColumnInfo);
				Grid.Columns.Add(column); 
				ColumnWrapper columnWrapper = gridViewDataColumnInfo.CreateColumnWrapper(column);
				if(columnInfo.ModelMember != null && columnInfo.ModelMember.Type == typeof(Type)) {
					column.Settings.AllowSort = DevExpress.Utils.DefaultBoolean.True;
					column.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
					column.Settings.AllowGroup = DevExpress.Utils.DefaultBoolean.True;
					column.Settings.FilterMode = ColumnFilterMode.DisplayText;
				}
				columnWrapper.ApplyModel(columnInfo);
				return columnWrapper;
			}
		}
		protected virtual GridViewDataColumnInfo CreateGridViewDataColumnInfo(IModelColumn modelColumn, CollectionSourceDataAccessMode dataAccessMode, bool isProtectedColumn) {
			return new GridViewDataColumnInfo(modelColumn, Grid, Model.DataAccessMode, isProtectedColumn);
		}
		protected internal Boolean CanConvertKeyMembersInfoFromToString() {
			Boolean canConvertFromToString = true;
			foreach(IMemberInfo keyMemberInfo in KeyMembersInfo) {
				canConvertFromToString = canConvertFromToString && CanConvertFromToString(keyMemberInfo.MemberType);
			}
			return canConvertFromToString;
		}
		private void DataBind() {
			if(Grid != null && Grid.Page != null) {  
				UpdateGridDataSource();
				contextMenu.CreateControls();
				Grid.DataBind();
				if(needResetSelection) {
					needResetSelection = false;
					Grid.Selection.UnselectAll();
				}
				if(Grid.SettingsBehavior.AutoExpandAllGroups) {
					if(!Grid.Page.IsCallback && !Grid.Page.IsPostBack) {
						Grid.ExpandAll();
					}
				}
			}
		}
		private void UpdateGridDataSource() {
			if(Grid != null && ((Grid.DataSource == null) || (((WebDataSource)Grid.DataSource).Collection != CollectionSource.Collection))) {
				Grid.DataSource = CreateWebDataSource(CollectionSource.Collection);
			}
		}
		private void InitLocalization() {
			DevExpress.Web.Localization.ASPxGridViewLocalizer.SetActiveLocalizerProvider(new ASPxGridViewResourceLocalizerProvider());
		}
		private void grid_SelectionChanged(object sender, EventArgs e) {
			OnSelectionChanged();
			needGetUpdatePanelsRenderResults = true;
		}
		private void grid_RowUpdated(object sender, DevExpress.Web.Data.ASPxDataUpdatedEventArgs e) {
			EditingObject = null;
			Refresh();
		}
		private void grid_ClientLayout(object sender, ASPxClientLayoutArgs e) {
			switch(e.LayoutMode) {
				case ClientLayoutMode.Loading:
					break;
				case ClientLayoutMode.Saving:
					OnISupportModelSaving();
					break;
			}
		}
		private void grid_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e) {
			if(e.EditForm != null) {
				e.EditForm.Load += new EventHandler(EditForm_Load);
				ApplyAppearance(e.EditForm, e);
			}
		}
		private void EditForm_Load(object sender, EventArgs e) {
			Control editForm = ((Control)sender);
			editForm.Load -= new EventHandler(EditForm_Load);
			ApplyAppearance(editForm, e);
		}
		private void ApplyAppearance(Control editForm, EventArgs e) {
			foreach(Control formControl in editForm.Controls[0].Controls) {
				foreach(Control control in formControl.Controls) {
					GridViewTableEditFormEditorCell editCell = control as GridViewTableEditFormEditorCell;
					if(editCell != null) {
						GridViewDataColumn editCellColumn = editCell.Column as GridViewDataColumn;
						if(editCellColumn != null) {
							IDataColumnInfo info = GetColumnInfo(editCellColumn) as IDataColumnInfo;
							if(info != null) {
								object rowObj = GetRowObject(editCell.VisibleIndex);
								ApplyAppearance(e, rowObj, editCell, info.Model.PropertyName);
							}
						}
					}
				}
			}
		}
		private void ApplyAppearance(object e, object rowObj, GridViewTableEditorCellBase editCell, string propertyName) {
			ASPxGridInplaceEditAppearanceAdapter adapter = new ASPxGridInplaceEditAppearanceAdapter(editCell, e);
			OnCustomizeEnabled(new CustomizeEnabledEventArgs(propertyName, adapter, rowObj, CollectionSource, ObjectSpace));
			if(adapter.Enabled) {
				OnCustomizeAppearance(new CustomizeAppearanceEventArgs(propertyName, new ASPxGridInplaceEditAppearanceAdapter(editCell, e), rowObj));
			}
		}
		private void grid_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e) {
			if(e.RowType == GridViewRowType.InlineEdit) {
				object rowObj = GetRowObject(e.VisibleIndex);
				foreach(TableCell cell in e.Row.Cells) {
					GridViewTableEditorCellBase editCell = cell as GridViewTableEditorCellBase;
					if(editCell != null) {
						IDataColumnInfo info = GetColumnInfo(editCell.Column) as IDataColumnInfo;
						if(info != null) {
							ApplyAppearance(e, rowObj, editCell, info.Model.PropertyName);
						}
					}
				}
			}
		}
		private void grid_StartRowEditing(object sender, ASPxStartRowEditingEventArgs e) {
			ASPxGridView gridView = sender as ASPxGridView;
			editingObject = gridView.GetRow(gridView.FindVisibleIndexByKeyValue(e.EditingKeyValue));
		}
		private void grid_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e) {
			IDataColumnInfo column = GetColumnInfo(e.DataColumn) as IDataColumnInfo;
			if(column != null) {
				object rowObj = GetRowObject(e.VisibleIndex);
				OnCustomizeAppearance(new CustomizeAppearanceEventArgs(column.Model.PropertyName, new TableCellAppearanceAdapter(e.Cell, e), rowObj));
			}
		}
		private void grid_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e) {
			NewObject = OnNewObjectAdding();
			OnNewObjectCreated();
			DataBind();
		}
		private void grid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e) {
			NewObject = null;
			CancelEdit();
			e.Cancel = true;
		}
		private void grid_AutoFilterCellEditorCreate(object sender, ASPxGridViewEditorCreateEventArgs e) {
			SetupAutoFilterCellEditor(e);
		}
		private void grid_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e) {
			if(UseASPxGridViewDataSpecificColumns) {
			}
		}
		public bool IsBatchMode {
			get { return Grid.SettingsEditing.Mode == GridViewEditingMode.Batch; }
		}
		private bool NeedCreateEditItemTemplate {
			get { return AllowEdit && ((CollectionSource == null) || (CollectionSource.DataAccessMode != CollectionSourceDataAccessMode.DataView)) && !IsBatchMode; }
		}
		private void columnFactory_CreateCustomEditItemTemplate(object sender, CustomizeDataItemTemplateEventArgs e) {
			if(CreateCustomEditItemTemplate != null) {
				CreateCustomEditItemTemplateEventArgs args = new CreateCustomEditItemTemplateEventArgs(e.ModelColumn, this);
				CreateCustomEditItemTemplate(this, args);
				e.Column.EditItemTemplate = args.Template;
				e.Handled = args.Handled;
			}
		}
		private void columnFactory_CreateCustomDataItemTemplate(object sender, CustomizeDataItemTemplateEventArgs e) {
			if(CreateCustomDataItemTemplate != null) {
				CreateCustomDataItemTemplateEventArgs args = new CreateCustomDataItemTemplateEventArgs(e.ModelColumn, this);
				CreateCustomDataItemTemplate(this, args);
				e.Column.DataItemTemplate = args.Template;
				e.Handled = args.Handled;
				e.CreateDefaultDataItemTemplate = args.CreateDefaultDataItemTemplate;
			}
		}
		private void columnFactory_CustomizeGridViewDataColumn(object sender, CustomizeGridViewDataColumnEventArgs e) {
			if(CustomizeGridViewDataColumn != null) {
				CustomizeGridViewDataColumn(this, e);
			}
		}
		private void columnFactory_CreateCustomGridViewDataColumn(object sender, CreateCustomGridViewDataColumnEventArgs e) {
			if(CreateCustomGridViewDataColumn != null) {
				CreateCustomGridViewDataColumn(this, e);
			}
		}
		private void columnFactory_CreateCustomGridViewDataColumnCore(object sender, CreateCustomGridViewDataColumnEventArgs e) {
			if(!UseProtectedContentColumn) {
				return;
			}
			if(HasProtectedContent(e.ModelColumn.PropertyName)) {
				DataColumnCreatorBase columnCreator = new ASPxProtectedContentColumnCreator(e.ModelColumn);
				e.Column = columnCreator.CreateGridViewColumn();
				e.GridViewDataColumnInfo = CreateGridViewDataColumnInfo(e.ModelColumn, Model.DataAccessMode, true);
			}
		}
		private List<string> GetTestColumnsInitializationScript(string gridTestColumnsName, bool viewModeOnly) {
			List<string> result = new List<string>();
			foreach(GridViewColumn column in Grid.VisibleColumns) {
				string clientId = String.Empty;
				string scriptClassNameEditMode = String.Empty;
				string scriptClassNameViewMode = String.Empty;
				GridViewDataColumn dataColumn = column as GridViewDataColumn;
				if(dataColumn != null) {
					EditModeDataItemTemplate editItemTemplate = dataColumn.EditItemTemplate as EditModeDataItemTemplate;
					ViewModeDataItemTemplate viewItemTemplate = dataColumn.DataItemTemplate as ViewModeDataItemTemplate;
					if(editItemTemplate != null && viewItemTemplate != null || UseASPxGridViewDataSpecificColumns) {
						IDataColumnInfo columnInfo = GetColumnInfo(column) as IDataColumnInfo;
						bool oldMode = false;
						if(UseASPxGridViewDataSpecificColumns) {
							clientId = string.Format(Grid.ClientID + "_{0}{1}_{2}_{3}_{4}", "cell", 0, dataColumn.Index, "xaf", dataColumn.FieldName.Replace(".", "_"));
						}
						string fieldName = columnInfo != null ? columnInfo.MemberInfo.Name : "";
						if(editItemTemplate != null && !viewModeOnly) {
							WebPropertyEditor editItemTemplatePropertyEditor = editItemTemplate.PropertyEditor;
							scriptClassNameEditMode = GetTestEditorJSClassName(editItemTemplatePropertyEditor);
							if(!viewModeOnly) {
								oldMode = true;
								clientId = editItemTemplatePropertyEditor.ClientId;
							}
						}
						else {
							scriptClassNameEditMode = JSASPxDefaultGridViewColumnTestControl.ClassName;
						}
						if(viewItemTemplate != null) {
							WebPropertyEditor viewItemTemplatePropertyEditor = viewItemTemplate.PropertyEditor;
							scriptClassNameViewMode = GetTestEditorJSClassName(viewItemTemplatePropertyEditor);
							if(viewModeOnly) {
								oldMode = true;
								clientId = viewItemTemplatePropertyEditor.ClientId;
							}
						}
						else {
							scriptClassNameViewMode = JSASPxDefaultGridViewColumnTestControl.ClassName;
						}
						Dictionary<int, string> rowIndexToEditorIdMap = CreateRowIndexToEditorIdMap(column, clientId, viewModeOnly, oldMode);
						Grid.JSProperties.Add("cp" + fieldName.Replace(".", "_") + column.VisibleIndex + "_RowIndexToEditorIdMap", rowIndexToEditorIdMap);
						Grid.JSProperties["cpInlineEditMode"] = Grid.SettingsEditing.Mode.ToString();
						result.Add(string.Format("{0}.AddColumn('{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",
							gridTestColumnsName, fieldName, scriptClassNameEditMode, scriptClassNameViewMode, clientId, column.VisibleIndex, column.Caption));
					}
				}
			}
			return result;
		}
		private string GetTestEditorJSClassName(WebPropertyEditor propertyEditor) {
			string result = "";
			ITestable testable = propertyEditor as ITestable;
			if(testable != null) {
				if(propertyEditor.ViewEditMode == ViewEditMode.View) {
					IJScriptTestControl scriptTestControl = propertyEditor.GetInplaceViewModeEditorTestControlImpl();
					if(scriptTestControl != null) {
						result = scriptTestControl.JScriptClassName;
					}
				}
				else {
					IJScriptTestControl scriptTestControl = propertyEditor.GetEditorTestControlImpl();
					if(scriptTestControl != null) {
						result = scriptTestControl.JScriptClassName;
					}
				}
			}
			return result;
		}
		private string InitTestColumns() {
			string result = string.Empty;
			if(TestScriptsManager.EasyTestEnabled) {
				string gridTestColumnsName = Grid.ClientID + ".cpTestColumns";
				List<string> gridTestColumnsScript = new List<string>();
				gridTestColumnsScript.Add(string.Format("{0} = new TestColumns('{1}');", gridTestColumnsName, Grid.ClientID));
				if(Grid.IsEditing) {
					gridTestColumnsScript.AddRange(GetTestColumnsInitializationScript(gridTestColumnsName, false));
				}
				else {
					gridTestColumnsScript.AddRange(GetTestColumnsInitializationScript(gridTestColumnsName, true));
				}
				result = string.Join("\r\n", gridTestColumnsScript.ToArray());
			}
			return result;
		}
		private Dictionary<int, string> CreateRowIndexToEditorIdMap(GridViewColumn column, string editorClientId, bool viewModeOnly, bool oldMode) {
			Dictionary<int, string> result = new Dictionary<int, string>();
			if(!string.IsNullOrEmpty(editorClientId)) {
				string idPattern = Grid.ClientID + "_{0}{1}_{2}_{3}_{4}";
				IDataColumnInfo columnInfo = GetColumnInfo(column) as IDataColumnInfo;
				string propertyName = null;
				if(columnInfo != null && !editorClientId.Contains(ASPxProtectedContentColumnCreator.ColumnFieldPartName)) {
					if(!oldMode) {
						propertyName = columnInfo.Model.FieldName.Replace(".", "_");
					}
					else {
						propertyName = columnInfo.Model.PropertyName.Replace(".", "_");
					}
				}
				else {
					propertyName = ASPxProtectedContentColumnCreator.ColumnFieldPartName;
				}
				string id = editorClientId.Substring(editorClientId.LastIndexOf("xaf_" + propertyName));
				string viewEditPostfixMatchValue = System.Text.RegularExpressions.Regex.Match(id, "(Edit|View)[0-9]*(_[0-9]+)?$").Value;
				string hashCodePostfix = "";
				string rowPostfix = "";
				if(!string.IsNullOrEmpty(viewEditPostfixMatchValue)) {
					id = id.Remove(id.LastIndexOf('_'));
					hashCodePostfix = viewEditPostfixMatchValue.Replace("View", "").Replace("Edit", "");
					if(hashCodePostfix.Contains("_")) {
						id = id.Remove(id.LastIndexOf('_'));
						rowPostfix = hashCodePostfix.Substring(hashCodePostfix.IndexOf('_'));
						hashCodePostfix = hashCodePostfix.Substring(0, hashCodePostfix.IndexOf('_'));
					}
				}
				id = id.Replace("_" + EditModeDataItemTemplate.ViewEditModeString + "_View", "");
				id = id.Replace("_" + EditModeDataItemTemplate.ViewEditModeString + "_Edit", "");
				for(int i = Grid.VisibleStartIndex; i < Grid.VisibleStartIndex + Grid.SettingsPager.PageSize; i++) {
					string cliendId = string.Format(idPattern, "cell", i, GetColumnIndexFromId(editorClientId), id, "View" + (!string.IsNullOrEmpty(rowPostfix) ? "_" + i : ""));
					cliendId += ";ViewMode";
					result.Add(i, cliendId);
				}
				if(!viewModeOnly) {
					string cliendId = string.Format(idPattern, "edit", Grid.IsNewRowEditing ? "new" : Grid.EditingRowVisibleIndex.ToString(), GetColumnIndexFromId(editorClientId), id, "Edit" + hashCodePostfix + rowPostfix);
					cliendId += ";EditMode";
					result[Grid.EditingRowVisibleIndex] = cliendId;
				}
			}
			return result;
		}
		private string GetColumnIndexFromId(string editorClientId) {
			string pattern = Grid.ClientID + @"_(cell|edit)(new|\d*)_(?<columnIndex>\d*)_xaf.*";
			return System.Text.RegularExpressions.Regex.Match(editorClientId, pattern).Groups["columnIndex"].Value.ToString();
		}
		private void grid_CustomErrorText(object sender, ASPxGridViewCustomErrorTextEventArgs e) {
			if(e.Exception is ValidationException) {
				e.ErrorText = ((ValidationException)e.Exception).Message.Replace("\r\n", "<br>");
			}
			errorText = e.ErrorText;
		}
		private void grid_CancelRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e) {
			if(NewObject != null) {
				OnNewObjectCanceled();
				NewObject = null;
			}
			else {
				if(EditingObject != null && !ObjectSpace.IsNewObject(EditingObject)) {
					ObjectSpace.ReloadObject(EditingObject);
				}
			}
			EditingObject = null;
		}
		private void ObjectSpace_Refreshing(object sender, CancelEventArgs e) {
			CancelEdit();
		}
		private void grid_HeaderFilterFillItems(object sender, ASPxGridViewHeaderFilterEventArgs e) {
			IDataColumnInfo columnInfoCore = GetColumnInfo(e.Column) as IDataColumnInfo;
			if(columnInfoCore == null) {
				return;
			}
			IMemberInfo memberDescriptor = columnInfoCore.MemberInfo;
			if(memberDescriptor.MemberType.IsEnum) {
				EnumDescriptor enumDescriptor = new EnumDescriptor(memberDescriptor.MemberType);
				foreach(FilterValue filterValue in e.Values) {
					if(Enum.IsDefined(memberDescriptor.MemberType, filterValue.Value)) {
						object value = Enum.Parse(memberDescriptor.MemberType, filterValue.Value);
						filterValue.DisplayText = enumDescriptor.GetCaption(value);
					}
				}
			}
			if(memberDescriptor.MemberType == typeof(bool)) {
				IModelColumn columnInfo = columnInfoCore.Model;
				foreach(FilterValue filterValue in e.Values) {
					bool isDefinedBoolCaption = !string.IsNullOrEmpty(columnInfo.CaptionForTrue) && !string.IsNullOrEmpty(columnInfo.CaptionForFalse);
					if(filterValue.Value == bool.TrueString && isDefinedBoolCaption) {
						filterValue.DisplayText = columnInfo.CaptionForTrue;
					}
					if(filterValue.Value == bool.FalseString && isDefinedBoolCaption) {
						filterValue.DisplayText = columnInfo.CaptionForFalse;
					}
				}
			}
			if(memberDescriptor.MemberType == typeof(decimal)) {
				IModelColumn columnInfo = columnInfoCore.Model;
				foreach(FilterValue filterValue in e.Values) {
					try {
						decimal value = Decimal.Parse(filterValue.DisplayText);
						if(string.IsNullOrEmpty(columnInfo.DisplayFormat)) {
							filterValue.DisplayText = string.Format(FormattingProvider.GetDisplayFormat(typeof(decimal)), value);
						}
						else {
							filterValue.DisplayText = string.Format(columnInfo.DisplayFormat, value);
						}
					}
					catch {
					}
				}
			}
		}
		protected override void UpdateCollectionDependentProperties() {
			base.UpdateCollectionDependentProperties();
			UpdateIsSelectable();
			UpdateKeyFieldName();
		}
		public override IList<object> GetControlSelectedObjects() {
			List<object> result = new List<object>();
			if(Grid != null) {
				List<String> keyMemberNames = new List<String>();
				foreach(IMemberInfo keyMemberInfo in KeyMembersInfo) {
					keyMemberNames.Add(keyMemberInfo.Name);
				}
				foreach(Object key in Grid.GetSelectedFieldValues(keyMemberNames.ToArray())) {
					object objectToSelect = GetObjectByKey(key);
					if(objectToSelect != null) {
						result.Add(objectToSelect);
					}
				}
			}
			return result;
		}
		public override void SetControlSelectedObjects(IList<object> objects) {
			Grid.Selection.UnselectAll();
			foreach(Object obj in objects) {
				Object keyValue = ((WebDataSource)Grid.DataSource).View.GetKeyValue(obj);
				if((KeyMembersInfo.Count > 1) && (keyValue is List<Object>)) {
					keyValue = ((List<Object>)keyValue).ToArray();
				}
				Grid.Selection.SetSelection(Grid.FindVisibleIndexByKeyValue(keyValue), true);
			}
		}
		protected internal void DoOnProcessSelectedItem() {
			base.OnProcessSelectedItem();
		}
		protected virtual ASPxGridView CreateGridControl() {
			ASPxGridView gridControl = new ASPxGridView();
			RenderHelper.SetupASPxWebControl(gridControl);
			return gridControl;
		}
		protected virtual ITemplate CreateDetailRowTemplate() {
			IModelListViewWeb model = Model as IModelListViewWeb;
			if(detailFramesManager == null) {
				detailFramesManager = new ASPxGridDetailFramesManager(Application, model.DetailRowView);
			}
			return new ASPxGridDetailRowTemplate(detailFramesManager, model.DetailRowMode == DetailRowMode.DetailView);
		}
		private void grid_Init(object sender, EventArgs e) {
			ASPxGridView grid = (ASPxGridView)sender;
			grid.Page.LoadComplete += new EventHandler(Page_LoadComplete);
			aspxGridViewExporter.Page = grid.Page;
			DataBind();
			needRebindData = false;
		}
		[Browsable(false)]
		protected internal void TryRebindData(System.Web.UI.Page page) {
			if(page.IsPostBack && needRebindData) {
				DataBind();
			}
		}
		private void Page_LoadComplete(object sender, EventArgs e) {
			System.Web.UI.Page page = (System.Web.UI.Page)sender;
			TryRebindData(page);
		}
		private void gridControl_Load(object sender, EventArgs e) {
			if(Grid.SettingsPager.Mode == GridViewPagerMode.EndlessPaging) {
				Grid.Width = Unit.Percentage(100);
			}
		}
		protected void SetupAutoFilterCellEditor(ASPxGridViewEditorCreateEventArgs e) {
			Type memberType = null;
			IModelColumn columnModel = null;
			IDataColumnInfo columnInfo = GetColumnInfo(e.Column) as IDataColumnInfo;
			if(columnInfo != null) {
				columnModel = columnInfo.Model;
				if(columnInfo.MemberInfo != null) {
					memberType = columnInfo.MemberInfo.MemberType;
				}
			}
			SetupAutoFilterCellEditor(e, e.Column, columnModel, memberType);
		}
		protected void SetupAutoFilterCellEditor(ASPxGridViewEditorCreateEventArgs sourceEventArgs, GridViewDataColumn column, IModelColumn columnModel, Type memberType) {
			CustomHandleAutoFilterCellEditorEventArgs args = new CustomHandleAutoFilterCellEditorEventArgs(sourceEventArgs, column, columnModel, memberType);
			OnCustomHandleAutoFilterCellEditor(args);
			if(!args.Handled && (columnModel != null) && (memberType != null) && (column != null) && (!UseASPxGridViewDataSpecificColumns || column.DataItemTemplate != null)) {
				EnumDescriptor enumDescriptor = (memberType.IsEnum) ? new EnumDescriptor(memberType) : null;
				EditPropertiesBase newEditProperties = GetAutoFilterCellEditor(sourceEventArgs.EditorProperties, memberType,
					columnModel.CaptionForTrue, columnModel.ImageForTrue, columnModel.CaptionForFalse, columnModel.ImageForFalse, enumDescriptor);
				if(newEditProperties != null) {
					sourceEventArgs.EditorProperties = newEditProperties;
				}
			}
			OnCustomizeAutoFilterCellEditor(args);
		}
		protected virtual EditPropertiesBase GetAutoFilterCellEditor(EditPropertiesBase sourceEditorProperties, Type memberType, string captionForTrue, string imageForTrue, string captionForFalse, string imageForFalse, EnumDescriptor enumDescriptor) {
			return ASPxGridViewHelper.GetAutoFilterCellEditor(sourceEditorProperties, memberType, captionForTrue, imageForTrue, captionForFalse, imageForFalse, enumDescriptor);
		}
		protected virtual GridViewDataColumn CreateColumn(IModelColumn columnInfo) {
			GridViewDataColumn column = new GridViewDataColumn();
			if((CollectionSource == null) || (CollectionSource.DataAccessMode != CollectionSourceDataAccessMode.DataView)) {
				column.DataItemTemplate = CreateDataItemTemplate(columnInfo);
				column.EditItemTemplate = CreateEditItemTemplate(columnInfo);
			}
			column.VisibleIndex = -1;
			return column;
		}
		protected virtual ITemplate CreateEditItemTemplate(IModelColumn columnInfo) {
			return CreateDefaultColumnTemplate(columnInfo, this, ViewEditMode.Edit);
		}
		protected virtual ITemplate CreateDataItemTemplate(IModelColumn columnInfo) {
			return CreateDefaultColumnTemplate(columnInfo, this, ViewEditMode.View);
		}
		protected override void OnAllowEditChanged() {
			CancelEdit();
			Refresh();
			base.OnAllowEditChanged();
		}
		protected virtual void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(Printable));
			}
		}
		protected virtual void OnCustomHandleAutoFilterCellEditor(CustomHandleAutoFilterCellEditorEventArgs e) {
			if(CustomHandleAutoFilterCellEditor != null) {
				CustomHandleAutoFilterCellEditor(this, e);
			}
		}
		protected virtual void OnCustomizeAutoFilterCellEditor(CustomHandleAutoFilterCellEditorEventArgs e) {
			if(CustomizeAutoFilterCellEditor != null) {
				CustomizeAutoFilterCellEditor(this, e);
			}
		}
		protected override void CreatePropertyEditors() {
			if(!UseASPxGridViewDataSpecificColumns) {
				base.CreatePropertyEditors();
			}
		}
		internal object GetRowObject(int visibleIndex) {
			return GetRowObject(Grid, visibleIndex);
		}
		private object GetRowObject(ASPxGridView gridView, int visibleIndex) {
			return visibleIndex == WebDataProxy.NewItemRow ? NewObject : gridView.GetRow(visibleIndex);
		}
		public ASPxGridView Grid {
			get { return grid; }
		}
		public override bool CanSelectRows {
			get { return isSelectable && base.CanSelectRows; }
			set { base.CanSelectRows = value; }
		}
		public override Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return
				(dataAccessMode == CollectionSourceDataAccessMode.Client)
				||
				(dataAccessMode == CollectionSourceDataAccessMode.Server)
				||
				(dataAccessMode == CollectionSourceDataAccessMode.DataView);
		}
		[DefaultValue(UseASPxGridViewDataSpecificColumnsDefault)]
		public static bool UseASPxGridViewDataSpecificColumns {
			get { return useASPxGridViewDataSpecificColumns; }
			set { useASPxGridViewDataSpecificColumns = value; }
		}
		[DefaultValue(AllowFilterControlHierarchyDefault)]
		public static bool AllowFilterControlHierarchy { get; set; }
		public ASPxGridViewExporter ASPxGridViewExporter { get { return aspxGridViewExporter; } }
		public object NewObject {
			get { return newObject; }
			set {
				if(NewObject != value) {
					newObject = value;
					if(Grid != null && Grid.DataSource != null) {
						(((WebDataSource)Grid.DataSource).View).EditingObject = NewObject;
					}
				}
			}
		}
		public bool EnableGroupTemplate {
			get { return enableGroupTemplate; }
			set { enableGroupTemplate = value; }
		}
		public ASPxGridBatchModeHelper BatchEditModeHelper {
			get {
				if(batchHelper == null) {
					batchHelper = new ASPxGridBatchModeHelper(this, ObjectTypeInfo);
					batchHelper.ProtectedContentText = Application.Model.ProtectedContentText;
				}
				return batchHelper;
			}
			set { batchHelper = value; }
		}
		internal bool DisplayActionColumns {
			get { return displayActionColumns; }
			set { displayActionColumns = value; }
		}
		protected object EditingObject {
			get {
				if(editingObject == null) {
					return NewObject;
				}
				return editingObject;
			}
			set { editingObject = value; }
		}
		internal string GetErrorText() {
			return errorText;
		}
		public event EventHandler<CustomHandleAutoFilterCellEditorEventArgs> CustomHandleAutoFilterCellEditor;
		public event EventHandler<CustomHandleAutoFilterCellEditorEventArgs> CustomizeAutoFilterCellEditor;
		#region ListEditor Members
		protected override object CreateControlsCore() {
			InitLocalization();
				grid = CreateGridControl();
			if(WebApplicationStyleManager.IsNewStyle) {
				grid.SettingsPager.Summary.Visible = false;
				string pageSizeCaption = grid.SettingsPager.PageSizeItemSettings.Caption;
				if(WebApplicationStyleManager.EnableUpperCase && pageSizeCaption != null) {
					pageSizeCaption = pageSizeCaption.Replace(":", "");
					grid.SettingsPager.PageSizeItemSettings.Caption = pageSizeCaption.ToUpper();
				}
			}
			if(IsAdaptive) {
				grid.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.Disabled;
				grid.Width = Unit.Percentage(100);
				grid.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
				grid.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
			}
			grid.AutoGenerateColumns = false;
			startupScriptRegisteringImpl = new ASPxGridViewSupportCallbackStartupScriptRegisteringImpl(grid);
			startupScriptRegisteringImpl.RegisterCallbackStartupScript += new EventHandler<RegisterCallbackStartupScriptEventArgs>(startupScriptRegisteringImpl_RegisterCallbackStartupScript);
				grid.ID = "Grid";
			grid.SettingsCookies.Enabled = false;
			grid.Settings.ShowFilterBar = GridViewStatusBarMode.Auto;
			grid.SettingsBehavior.EnableCustomizationWindow = true;
			grid.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.Disabled;
			grid.Settings.ShowHeaderFilterButton = true;
			grid.SettingsEditing.UseFormLayout = false;
			grid.CssClass = "GridView";
			grid.Styles.Header.Wrap = DevExpress.Utils.DefaultBoolean.True;
			grid.EnableRowsCache = false;
			if(EnableGroupTemplate) {
				grid.Templates.GroupRowContent = new GroupContentTemplate(this);
			}
			ApplyModel();
			SubscribeGridViewEvents();
			if(NewItemRowPosition != NewItemRowPosition.None) {
				grid.SettingsEditing.NewItemRowPosition = (NewItemRowPosition == NewItemRowPosition.Top) ? GridViewNewItemRowPosition.Top : GridViewNewItemRowPosition.Bottom;
			}
			if(grid.SettingsDetail.ShowDetailRow) {
				grid.Templates.DetailRow = CreateDetailRowTemplate();
			}
			if(AllowFilterControlHierarchy && CollectionSource != null && CollectionSource.ObjectTypeInfo != null && Application != null && Application.Model != null && Application.Model.BOModel != null) {
				grid.SettingsFilterControl.AllowHierarchicalColumns = true;
				grid.SettingsFilterControl.ShowAllDataSourceColumns = true;
				grid.SettingsFilterControl.ViewMode = FilterControlViewMode.VisualAndText;
				grid.SettingsFilterControl.MaxHierarchyDepth = 3;
				filterControlHelper = new ASPxGridViewFilterControlColumnsHelper(grid);
				XafASPxGridViewFilterControlCustomizer filterControlCustomizer = new XafASPxGridViewFilterControlCustomizer(CollectionSource.ObjectTypeInfo, Application.Model.BOModel, filterControlHelper);
			}
			aspxGridViewExporter = new ASPxGridViewExporter();
			aspxGridViewExporter.GridViewID = grid.ID;
			aspxGridViewExporter.RenderBrick += new ASPxGridViewExportRenderingEventHandler(exporter_RenderBrick);
			OnPrintableChanged();
			return grid;
		}
		void exporter_RenderBrick(object sender, ASPxGridViewExportRenderingEventArgs e) {
			if(e.RowType == GridViewRowType.Data) {
				GridViewDataColumn column = e.Column as GridViewDataColumn;
				if(column != null) {
					ViewModeDataItemTemplate template = column.DataItemTemplate as ViewModeDataItemTemplate;
					if(template != null && template.PropertyEditor != null && template.PropertyEditor is ISupportExportCustomValue) {
						template.PropertyEditor.CurrentObject = Grid.GetRow(e.VisibleIndex);
						string localizedValue = ((ISupportExportCustomValue)template.PropertyEditor).GetExportedValue();
						if(localizedValue != null) {
							e.Text = localizedValue;
						}
					}
				}
			}
		}
		private void Control_Unload(object sender, EventArgs e) {
			OnControlInitialized(sender as Control);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
			if(TestableControlsCreated != null) {
				TestableControlsCreated(this, EventArgs.Empty);
			}
			ITestable[] testableControl = GetTestableControls();
			if(testableControl.Length > 0) {
				((TestableContolWrapper)testableControl[0]).OnControlInitialized();
			}
		}
		private void SubscribeGridViewEvents() {
			Grid.Init += new EventHandler(grid_Init);
			Grid.SelectionChanged += new EventHandler(grid_SelectionChanged);
			Grid.CustomErrorText += new ASPxGridViewCustomErrorTextEventHandler(grid_CustomErrorText);
			Grid.HtmlDataCellPrepared += new ASPxGridViewTableDataCellEventHandler(grid_HtmlDataCellPrepared);
			Grid.HtmlEditFormCreated += new ASPxGridViewEditFormEventHandler(grid_HtmlEditFormCreated);
			Grid.HtmlRowPrepared += new ASPxGridViewTableRowEventHandler(grid_HtmlRowPrepared);
			Grid.ClientLayout += new ASPxClientLayoutHandler(grid_ClientLayout);
			Grid.HeaderFilterFillItems += new ASPxGridViewHeaderFilterEventHandler(grid_HeaderFilterFillItems);
			Grid.AutoFilterCellEditorCreate += new ASPxGridViewEditorCreateEventHandler(grid_AutoFilterCellEditorCreate);
			Grid.AutoFilterCellEditorInitialize += grid_AutoFilterCellEditorInitialize;
			Grid.CustomJSProperties += new ASPxGridViewClientJSPropertiesEventHandler(grid_CustomJSProperties);
			Grid.Load += new EventHandler(gridControl_Load);
			Grid.Unload += new EventHandler(Control_Unload);
			if(CanManageFilterExpressionDisplayText) {
				Grid.CustomFilterExpressionDisplayText += new DevExpress.Web.CustomFilterExpressionDisplayTextEventHandler(grid_CustomFilterExpressionDisplayText);
			}
			Grid.BeforeGetCallbackResult += new EventHandler(grid_BeforeGetCallbackResult);
			if(IsBatchMode) {
				if(CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) {
					throw new Exception("The ASPxGridListEditor doesn't support the Batch edit mode together with the DataView data access mode. " +
						string.Format("To fix this issue, change the InlineEditMode or the DataAccessMode property of the '{0}' node in the Model Editor", Model.Id));
				}
				if(grid.SettingsPager.Mode == GridViewPagerMode.EndlessPaging) {
					throw new Exception("The ASPxGridListEditor doesn't support the Batch edit mode together with the Endless Paging mode. " +
						string.Format("To fix this issue, change the InlineEditMode or the AllowEndlessPaging property of the '{0}' node in the Model Editor.", Model.Id));
				}
				if(ASPxGridBatchModeHelper.CheckEnabled(Grid, AllowEdit)) {
					BatchEditModeHelper.Attach(Grid);
				}
			}
			else {
				if(NewItemRowPosition != NewItemRowPosition.None) {
					Grid.InitNewRow += new DevExpress.Web.Data.ASPxDataInitNewRowEventHandler(grid_InitNewRow);
					Grid.RowInserting += new DevExpress.Web.Data.ASPxDataInsertingEventHandler(grid_RowInserting);
				}
				Grid.RowValidating += new ASPxDataValidationEventHandler(grid_RowValidating);
				Grid.StartRowEditing += new ASPxStartRowEditingEventHandler(grid_StartRowEditing);
				Grid.CancelRowEditing += new DevExpress.Web.Data.ASPxStartRowEditingEventHandler(grid_CancelRowEditing);
				Grid.RowUpdated += new DevExpress.Web.Data.ASPxDataUpdatedEventHandler(grid_RowUpdated);
			}
		}
		private void grid_RowValidating(object sender, ASPxDataValidationEventArgs e) {
			ValidateObjectEventArgs args = new ValidateObjectEventArgs(EditingObject, true);
			try {
				OnValidateObject(args);
				if(args.Valid) {
					OnCommitChanges();
				}
			}
			catch(Exception exception) {
				args.Valid = false;
				if(!string.IsNullOrEmpty(exception.Message)) {
					args.ErrorText = exception.Message.Replace("\r\n", "<br>");
				}
			}
			if(!args.Valid) {
				e.RowError = args.ErrorText;
			}
		}
		private void grid_CustomJSProperties(object sender, ASPxGridViewClientJSPropertiesEventArgs e) {
			string cpInitTestColumnsPropertyName = "cpInitTestColumns";
			if(e.Properties.ContainsKey(cpInitTestColumnsPropertyName)) {
				e.Properties.Remove(cpInitTestColumnsPropertyName);
			}
			e.Properties.Add(cpInitTestColumnsPropertyName, InitTestColumns());
		}
		private void UnsubscribeGridViewEvents() {
			Grid.Init -= new EventHandler(grid_Init);
			Grid.SelectionChanged -= new EventHandler(grid_SelectionChanged);
			Grid.CustomErrorText -= new ASPxGridViewCustomErrorTextEventHandler(grid_CustomErrorText);
			Grid.RowValidating -= new ASPxDataValidationEventHandler(grid_RowValidating);
			Grid.HtmlDataCellPrepared -= new ASPxGridViewTableDataCellEventHandler(grid_HtmlDataCellPrepared);
			Grid.HtmlEditFormCreated -= new ASPxGridViewEditFormEventHandler(grid_HtmlEditFormCreated);
			Grid.HtmlRowPrepared -= new ASPxGridViewTableRowEventHandler(grid_HtmlRowPrepared);
			Grid.StartRowEditing -= new ASPxStartRowEditingEventHandler(grid_StartRowEditing);
			Grid.ClientLayout -= new ASPxClientLayoutHandler(grid_ClientLayout);
			Grid.RowUpdated -= new DevExpress.Web.Data.ASPxDataUpdatedEventHandler(grid_RowUpdated);
			Grid.HeaderFilterFillItems -= new ASPxGridViewHeaderFilterEventHandler(grid_HeaderFilterFillItems);
			Grid.AutoFilterCellEditorCreate -= new ASPxGridViewEditorCreateEventHandler(grid_AutoFilterCellEditorCreate);
			Grid.AutoFilterCellEditorInitialize -= grid_AutoFilterCellEditorInitialize;
			Grid.CancelRowEditing -= new DevExpress.Web.Data.ASPxStartRowEditingEventHandler(grid_CancelRowEditing);
			Grid.RowInserting -= new DevExpress.Web.Data.ASPxDataInsertingEventHandler(grid_RowInserting);
			Grid.InitNewRow -= new DevExpress.Web.Data.ASPxDataInitNewRowEventHandler(grid_InitNewRow);
			Grid.CustomJSProperties -= new ASPxGridViewClientJSPropertiesEventHandler(grid_CustomJSProperties);
			Grid.CustomFilterExpressionDisplayText -= new DevExpress.Web.CustomFilterExpressionDisplayTextEventHandler(grid_CustomFilterExpressionDisplayText);
			Grid.BeforeGetCallbackResult -= new EventHandler(grid_BeforeGetCallbackResult);
			if(batchHelper != null) {
				batchHelper.Detach();
			}
		}
		protected override void AssignDataSourceToControl(Object dataSource) {
			UpdateKeyFieldName();
			DataBind();
			needRebindData = false;
		}
		protected override WebDataSource CreateWebDataSource(object collection) {
			WebDataSource result = base.CreateWebDataSource(collection);
			result.View.EditingObject = NewObject;
			return result;
		}
		public override void Refresh() {
			if(Grid != null && !Grid.PreRendered && !Grid.IsEditing) {
				needRebindData = true;
			}
		}
		public override void BreakLinksToControls() {
			if(contextMenu != null) {
				contextMenu.BreakLinksToControls();
			}
			if(Grid != null) {
				UnsubscribeGridViewEvents();
				DisposeContainer disposeContainer = new DisposeContainer();
				foreach(GridViewColumn column in Grid.Columns) {
					if(column is GridViewDataColumn) {
						disposeContainer.TryAdd(((GridViewDataColumn)column).DataItemTemplate);
						disposeContainer.TryAdd(((GridViewDataColumn)column).EditItemTemplate);
						((GridViewDataColumn)column).DataItemTemplate = null;
						((GridViewDataColumn)column).EditItemTemplate = null;
					}
				}
				disposeContainer.Dispose();
				grid = null;
				ClearCache();
				if(aspxGridViewExporter != null) {
					aspxGridViewExporter.RenderBrick -= new ASPxGridViewExportRenderingEventHandler(exporter_RenderBrick);
					aspxGridViewExporter = null;
					OnPrintableChanged();
				}
				if(filterControlHelper != null) {
					filterControlHelper.Dispose();
					filterControlHelper = null;
				}
			}
			if(startupScriptRegisteringImpl != null) {
				startupScriptRegisteringImpl.RegisterCallbackStartupScript -= new EventHandler<RegisterCallbackStartupScriptEventArgs>(startupScriptRegisteringImpl_RegisterCallbackStartupScript);
				startupScriptRegisteringImpl.Dispose();
				startupScriptRegisteringImpl = null;
			}
			DataSource = null;
			base.BreakLinksToControls();
		}
		private void startupScriptRegisteringImpl_RegisterCallbackStartupScript(object sender, RegisterCallbackStartupScriptEventArgs e) {
			OnRegisterCallbackStartupScript(e);
		}
		private void grid_CustomFilterExpressionDisplayText(object sender, DevExpress.Web.CustomFilterExpressionDisplayTextEventArgs e) {
			if(CanManageFilterExpressionDisplayText) {
				CriteriaOperator criteria = CriteriaOperator.Clone((CriteriaOperator)e.Criteria);
				EnumLocalizationCriteriaProcessor localizer = new EnumLocalizationCriteriaProcessor(CollectionSource.ObjectTypeInfo, Model.Columns);
				localizer.Process(criteria);
				e.DisplayText = criteria.ToString();
			}
		}
		protected virtual void OnRegisterCallbackStartupScript(RegisterCallbackStartupScriptEventArgs e) {
			if(RegisterCallbackStartupScript != null) {
				RegisterCallbackStartupScript(this, e);
			}
		}
		protected virtual void OnISupportModelSaving() {
			if(supportModelSavingEvent != null) {
				supportModelSavingEvent(this, EventArgs.Empty);
			}
		}
		public override void Dispose() {
			BreakLinksToControls();
			if(ObjectSpace != null) {
				ObjectSpace.Refreshing -= new EventHandler<CancelEventArgs>(ObjectSpace_Refreshing);
			}
			if(contextMenu != null) {
				contextMenu.Dispose();
				contextMenu = null;
			}
			if(detailFramesManager != null) {
				detailFramesManager.Dispose();
				detailFramesManager = null;
			}
			CustomHandleAutoFilterCellEditor = null;
			CustomizeAutoFilterCellEditor = null;
			ControlInitialized = null;
			TestableControlsCreated = null;
			RegisterCallbackStartupScript = null;
			PrintableChanged = null;
			supportModelSavingEvent = null;
			CustomizeAppearance = null;
			CustomizeEnabled = null;
			CreateCustomGridViewDataColumn = null;
			CustomizeGridViewDataColumn = null;
			CreateCustomDataItemTemplate = null;
			CreateCustomEditItemTemplate = null;
			base.Dispose();
		}
		public override void ApplyModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelApplying(args);
				if(!args.Cancel) {
					if(Grid != null) {
						ASPxGridViewModelSynchronizer.ApplyModel(Model, this);
					}
					base.ApplyModel();
					OnModelApplied();
				}
			}
		}
		public override void SaveModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelSaving(args);
				if(!args.Cancel) {
					if(Grid != null) {
						ASPxGridViewModelSynchronizer.SaveModel(Model, this);
					}
					base.SaveModel();
					OnModelSaved();
				}
			}
		}
		public override SelectionType SelectionType {
			get { return CanSelectRows ? SelectionType.MultipleSelection : SelectionType.TemporarySelection; }
		}
		public override DevExpress.ExpressApp.Templates.IContextMenuTemplate ContextMenuTemplate {
			get { return contextMenu; }
		}
		public override object FocusedObject {
			get {
				if(NewObject != null) {
					return NewObject;
				}
				return base.FocusedObject;
			}
			set { base.FocusedObject = value; }
		}
		#endregion
		#region ISupportFooter Members
		public bool IsFooterVisible {
			get { return Grid.Settings.ShowFooter; }
			set { Grid.Settings.ShowFooter = value; }
		}
		#endregion
		#region IComplexListEditor Members
		public override void Setup(CollectionSourceBase collectionSource, XafApplication application) {
			base.Setup(collectionSource, application);
			ObjectSpace.Refreshing += new EventHandler<CancelEventArgs>(ObjectSpace_Refreshing);
		}
		#endregion
		#region ITestable Members
		string ITestable.TestCaption {
			get { return Name; }
		}
		string ITestable.ClientId {
			get { return Grid == null ? "" : Grid.ClientID; }
		}
		IJScriptTestControl ITestable.TestControl {
			get { return new JSASPxGridListEditorTestControl(); }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get { return TestControlType.Table; }
		}
		#endregion
		#region IDetailFramesContainer Members
		public IEnumerable<DetailFrameInfo> GetDetailFramesInfo() {
			IEnumerable<DetailFrameInfo> result = new List<DetailFrameInfo>();
			if(detailFramesManager != null) {
				foreach(DetailFrameInfo frameInfo in detailFramesManager.GetDetailsFramesInfo()) {
					int frameIndex = frameInfo.FrameIndex;
					if(Grid.DetailRows.IsVisible(frameIndex))
						(result as IList<DetailFrameInfo>).Add(frameInfo);
				}
			}
			return result;
		}
		#endregion
		#region ITestableContainer Members
		public ITestable[] GetTestableControls() {
			if(!string.IsNullOrEmpty(errorText)) {
				ErrorInfoTestableContolWrapper errorInfoTestableControl = new ErrorInfoTestableContolWrapper(Grid == null ? "" : Grid.ClientID, this.GetErrorText());
				return new ITestable[] { errorInfoTestableControl };
			}
			return new ITestable[] { };
		}
		public event EventHandler TestableControlsCreated;
		#endregion
		#region IControlOrderProvider Members
		public int GetIndexByObject(object obj) {
			if(Grid != null && obj != null) {
				return Grid.FindVisibleIndexByKeyValue(((WebDataSource)Grid.DataSource).View.GetKeyValue(obj));
			}
			return -1;
		}
		public object GetObjectByIndex(int index) {
			if(Grid != null) {
				return Grid.GetRow(index);
			}
			return null;
		}
		public IList GetOrderedObjects() {
			List<Object> list = null;
			if((Grid != null) && (CollectionSource != null)) {
				if(CollectionSource.IsServerMode) {
					list = new List<Object>(Grid.SettingsPager.PageSize);
					Int32 topRowIndex = Grid.VisibleStartIndex;
					Int32 bottomRowIndex = Grid.VisibleStartIndex + Grid.SettingsPager.PageSize - 1;
					if(bottomRowIndex > Grid.VisibleRowCount - 1) {
						bottomRowIndex = Grid.VisibleRowCount - 1;
					}
					for(Int32 i = topRowIndex; i <= bottomRowIndex; i++) {
						if(!Grid.IsGroupRow(i)) {
							object obj = Grid.GetRow(i);
							if(obj != null) {
								list.Add(obj);
							}
						}
					}
				}
				else {
					list = new List<Object>(Grid.VisibleRowCount);
					for(int i = 0; i < Grid.VisibleRowCount; i++) {
						if(!Grid.IsGroupRow(i)) {
							object obj = Grid.GetRow(i);
							if(obj != null) {
								list.Add(obj);
							}
						}
					}
				}
			}
			else {
				list = new List<Object>();
			}
			return list;
		}
		#endregion
		#region ISupportNewItemRowPosition Members
		public NewItemRowPosition NewItemRowPosition {
			get { return newItemRowPosition; }
			set {
				if(NewItemRowPosition != value) {
					newItemRowPosition = value;
					if(contextMenu != null) {
						contextMenu.UpdateNewItemRowPosition();
					}
				}
			}
		}
		#endregion
		#region IDataItemTemplateInfoProvider Members
		private event EventHandler<CustomGetDataColumnInfoEventArgs> CustomGetDataColumnInfo;
		event EventHandler<CustomGetDataColumnInfoEventArgs> IDataItemTemplateInfoProvider.CustomGetDataColumnInfo {
			add { CustomGetDataColumnInfo += value; }
			remove { CustomGetDataColumnInfo -= value; }
		}
		protected virtual IGridViewDataColumnInfo OnCustomGetDataColumnInfo(WebColumnBase column) {
			if(CustomGetDataColumnInfo != null) {
				CustomGetDataColumnInfoEventArgs args = new CustomGetDataColumnInfoEventArgs(column);
				CustomGetDataColumnInfo(this, args);
				return (IGridViewDataColumnInfo)args.ColumnInfo;
			}
			return null;
		}
		protected internal IColumnInfo GetColumnInfo(WebColumnBase column) {
			IColumnInfo result = OnCustomGetDataColumnInfo(column);
			if(result == null) {
				columnsInfoCache.TryGetValue(column, out result);
			}
			return result;
		}
		IColumnInfo IDataItemTemplateInfoProvider.GetColumnInfo(WebColumnBase column) {
			return GetColumnInfo(column);
		}
		WebColumnBase IDataItemTemplateInfoProvider.GetColumn(Control container) {
			return ((GridViewDataItemTemplateContainer)container).Column;
		}
		public object GetObject(Control container) {
			GridViewDataItemTemplateContainer holder = (GridViewDataItemTemplateContainer)container;
			return GetRowObject(holder.Grid, holder.VisibleIndex);
		}
		public string GetContainerId(Control container, string propertyName) {
			return "cntr_" + propertyName + "_" + ((TemplateContainerBase)container).ItemIndex;
		}
		private void ClearCache() {
			columnsInfoCache.Clear();
		}
		#endregion
		public override IList<ColumnWrapper> Columns {
			get {
				List<ColumnWrapper> result = new List<ColumnWrapper>();
				if(Grid != null) {
					if(!Model.BandsLayout.Enable) {
						foreach(GridViewColumn column in Grid.Columns) {
							ColumnWrapper columnWrapper = null;
							if(column is GridViewDataColumn) {
								IColumnInfo columnInfo = GetColumnInfo(column);
								if(columnInfo != null) {
									columnWrapper = columnInfo.CreateColumnWrapper(column);
								}
							}
							if(columnWrapper == null) {
								columnWrapper = new WebColumnBaseColumnWrapper(column);
							}
							result.Add(columnWrapper);
						}
					}
					else {
						result.AddRange(GetBandedCollumns(Grid.Columns));
					}
				}
				return result;
			}
		}
		private IList<ColumnWrapper> GetBandedCollumns(GridViewColumnCollection columns) {
			List<ColumnWrapper> result = new List<ColumnWrapper>();
			foreach(GridViewColumn column in columns) {
				ColumnWrapper columnWrapper = null;
				IColumnInfo columnInfo = GetColumnInfo(column);
				if(columnInfo != null) {
					columnWrapper = columnInfo.CreateColumnWrapper(column);
					result.Add(columnWrapper);
					if(column is GridViewBandColumn) {
						result.AddRange(GetBandedCollumns(((GridViewBandColumn)column).Columns));
					}
				}
				if(columnWrapper == null) {
					columnWrapper = new WebColumnBaseColumnWrapper(column);
					result.Add(columnWrapper);
				}
			}
			return result;
		}
		public bool CanManageFilterExpressionDisplayText { get; set; }
		#region ISupportCallbackStartupScriptRegistering Members
		public event EventHandler<RegisterCallbackStartupScriptEventArgs> RegisterCallbackStartupScript;
		#endregion
		#region IProcessCallbackComplete Members
		public override void ProcessCallbackComplete() {
			if(needRebindData) {
				DataBind();
			}
			if(contextMenu != null) {
				contextMenu.CreateControls();
			}
		}
		#endregion
		#region IExportable Members
		public List<ExportTarget> SupportedExportFormats {
			get {
				if(Printable == null) {
					return new List<ExportTarget>();
				}
				else {
					return new List<ExportTarget>() {
						ExportTarget.Csv,
						ExportTarget.Html,
						ExportTarget.Image,
						ExportTarget.Mht,
						ExportTarget.Text,
						ExportTarget.Pdf,
						ExportTarget.Rtf,
						ExportTarget.Xls,
						ExportTarget.Xlsx
					};
				}
			}
		}
		public IPrintable Printable {
			get { return aspxGridViewExporter; }
		}
		public void OnExporting() { }
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
		#endregion
		#region IDataAwareExportable Members
		void IDataAwareExportableCsv.Export(System.IO.Stream stream, CsvExportOptionsEx options) {
			if(aspxGridViewExporter == null) {
				throw new InvalidOperationException("'aspxGridViewExporter' is null");
			}
			aspxGridViewExporter.WriteCsv(stream, options);
		}
		void IDataAwareExportableXls.Export(System.IO.Stream stream, XlsExportOptionsEx options) {
			if(aspxGridViewExporter == null) {
				throw new InvalidOperationException("'aspxGridViewExporter' is null");
			}
			aspxGridViewExporter.WriteXls(stream, options);
		}
		void IDataAwareExportableXlsx.Export(System.IO.Stream stream, XlsxExportOptionsEx options) {
			if(aspxGridViewExporter == null) {
				throw new InvalidOperationException("'aspxGridViewExporter' is null");
			}
			aspxGridViewExporter.WriteXlsx(stream, options);
		}
		#endregion
		#region ISupportSelectionOperations Members
		bool needResetSelection = false;
		public void UnselectAll() {
			if(Grid != null) {
				Grid.Selection.UnselectAll();
			}
			else {
				needResetSelection = true;
			}
		}
		public void UnselectRowByKey(object key) {
			if(Grid != null) {
				Grid.Selection.UnselectRowByKey(key);
			}
		}
		public void BeginUpdateSelection() {
			Grid.Selection.BeginSelection();
		}
		public void EndUpdateSelection() {
			Grid.Selection.EndSelection();
		}
		#endregion
		#region ICustomRenderUpdatePanel Members
		event EventHandler<GetCustomRenderResultsArgs> ICustomRenderUpdatePanel.GetRenderResults {
			add { GetUpdatePanelsRenderResults += value; }
			remove { GetUpdatePanelsRenderResults -= value; }
		}
		private event EventHandler<GetCustomRenderResultsArgs> GetUpdatePanelsRenderResults;
		private void grid_BeforeGetCallbackResult(object sender, EventArgs e) {
			if(needGetUpdatePanelsRenderResults && GetUpdatePanelsRenderResults != null) {
				GetCustomRenderResultsArgs eventArgs = new GetCustomRenderResultsArgs();
				GetUpdatePanelsRenderResults(this, eventArgs);
				if(eventArgs.RenderResults.Count > 0) {
					if(!Grid.JSProperties.ContainsKey(EndCallbackHandlers)) {
						Grid.JSProperties[EndCallbackHandlers] = "";
					}
					Grid.JSProperties[EndCallbackHandlers] = "cpProcessCustomRenderResult;" + Grid.JSProperties[EndCallbackHandlers];
					Grid.JSProperties[XafCallbackManager.ControlsToUpdate] = "";
					Grid.JSProperties["cpProcessCustomRenderResult"] = @"ProcessMarkup(s, true);";
					foreach(string panelId in eventArgs.RenderResults.Keys) {
						Grid.JSProperties["cp" + panelId] = eventArgs.RenderResults[panelId];
						Grid.JSProperties[XafCallbackManager.ControlsToUpdate] += panelId + ";";
					}
				}
				needGetUpdatePanelsRenderResults = false;
			}
		}
		#endregion
		#region ISupportModelSaving Members
		private EventHandler<EventArgs> supportModelSavingEvent;
		event EventHandler<EventArgs> ISupportModelSaving.ModelSaving { add { supportModelSavingEvent += value; } remove { supportModelSavingEvent -= value; } }
		#endregion
		#region ISupportFilter Members
		bool ISupportFilter.FilterEnabled {
			get { return Grid.FilterEnabled; }
			set { Grid.FilterEnabled = value; }
		}
		string ISupportFilter.Filter {
			get { return Grid.FilterExpression; }
			set { Grid.FilterExpression = value; }
		}
		#endregion
		#region ISupportPager Members
		int ISupportPager.PageIndex {
			get { return Grid.PageIndex; }
			set {
				if(value < Grid.PageCount) {
					Grid.PageIndex = value;
				}
			}
		}
		int ISupportPager.PageSize {
			get { return Grid.SettingsPager.PageSize; }
			set { Grid.SettingsPager.PageSize = value; }
		}
		#endregion
		#region ISupportAppearanceCustomization Members
		public event EventHandler<CustomizeAppearanceEventArgs> CustomizeAppearance;
		public event EventHandler<CustomizeEnabledEventArgs> CustomizeEnabled;
		protected virtual void OnCustomizeAppearance(CustomizeAppearanceEventArgs args) {
			if(CustomizeAppearance != null) {
				CustomizeAppearance(this, args);
			}
		}
		protected virtual void OnCustomizeEnabled(CustomizeEnabledEventArgs args) {
			if(CustomizeEnabled != null) {
				CustomizeEnabled(this, args);
			}
		}
		#endregion
#if DebugTest
		public bool TestNeedResetSelection {
			get { return needResetSelection; }
		}
		public bool DebugTest_NeedGetUpdatePanelsRenderResults {
			get { return needGetUpdatePanelsRenderResults; }
			set { needGetUpdatePanelsRenderResults = value; }
		}
		public void SetGridView_ForTests(ASPxGridView gridView) {
			grid = gridView;
		}
		public Dictionary<WebColumnBase, IColumnInfo> ColumnsInfoCache_ForTests {
			get { return columnsInfoCache; }
		}
		public ModelLayoutElementCollection GetNotSortedModelColumns_ForTests(IModelListView listViewModel) {
			return GetLayoutElementCollection(listViewModel);
		}
#endif
		public event EventHandler<CreateCustomGridViewDataColumnEventArgs> CreateCustomGridViewDataColumn;
		public event EventHandler<CustomizeGridViewDataColumnEventArgs> CustomizeGridViewDataColumn;
		public event EventHandler<CreateCustomDataItemTemplateEventArgs> CreateCustomDataItemTemplate;
		public event EventHandler<CreateCustomEditItemTemplateEventArgs> CreateCustomEditItemTemplate;
		#region IGridBatchOperationProvider Members
		IObjectSpace IGridObjectSpaceCreator.CreateObjectSpace(Type type) {
			return Application.CreateObjectSpace(type);
		}
		object IGridBatchOperationProvider.GetObject(object key) {
			object obj = GetObjectByKey(key);
			if(obj is XafDataViewRecord) {
				obj = ObjectSpace.GetObject(obj);
			}
			return obj;
		}
		object IGridBatchOperationProvider.CreateNewObject() {
			return OnNewObjectAdding();
		}
		IModelColumn IGridBatchOperationProvider.GetModelColumn(GridViewDataColumn column) {
			IColumnInfo сolumnInfo = GetColumnInfo(column);
			return сolumnInfo == null ? null : сolumnInfo.Model as IModelColumn;
		}
		void IGridBatchOperationProvider.CommitChanges() {
			OnCommitChanges();
			if(CollectionSource.Collection is XafDataView) {
				((XafDataView)CollectionSource.Collection).Reload();
			}
		}
		void IGridBatchOperationProvider.Validate(ClientValidateObjectEventArgs args) {
			if(ClientValidateObject != null) {
				ClientValidateObject(this, args);
				if(!args.Valid) {
					errorText = args.ErrorText.Replace("\r\n", "<br>"); ;
				}
			}
		}
		void IGridBatchOperationProvider.CustomizeAppearance(CustomizeAppearanceEventArgs args) {
			OnCustomizeAppearance(args);
		}
		public event EventHandler<ClientValidateObjectEventArgs> ClientValidateObject;
		#endregion
	}
}
