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
using System.ComponentModel.Design;
using System.Linq;
using System.Web.UI;
using System.Web.UI.Design;
using System.Windows.Forms;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxTreeList.Design {	
	public class ASPxTreeListDesigner : ASPxDataWebControlDesigner {
		const string ColumnTemplateFormat = "Columns[{0}]: {1}";
		static readonly string[]
			RootTemplateNames = new string[] { "HeaderCaption", "DataCell", "Preview", "GroupFooterCell", "FooterCell", "EditForm" },
			ColumnTemplateNames = new string[] { "HeaderCaptionTemplate", "GroupFooterCellTemplate", "FooterCellTemplate" },
			DataColumnTemplateNames = new string[] { "DataCellTemplate", "EditCellTemplate" };
		public override void Initialize(IComponent component) {	
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
			RegisterTagPrefix(typeof(ASPxEditBase));
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences(AssemblyInfo.SRAssemblyTreeList);
		}
		protected internal ASPxTreeList TreeList {
			get { return Component as ASPxTreeList; }
		}
		protected DesignerHierarchicalDataSourceView DesignerHierarchicalView {
			get {
				IHierarchicalDataSourceDesigner dataSourceDesigner = GetHierarchicalDataSourceDesigner();
				if(dataSourceDesigner != null)
					return GetHierarchicalDesignerView(dataSourceDesigner);
				return null;
			}
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PropertyDescriptor autoGCDescriptor = (PropertyDescriptor)properties["AutoGenerateColumns"];
			properties["AutoGenerateColumns"] = TypeDescriptor.CreateProperty(typeof(ASPxTreeListDesigner), autoGCDescriptor);
			if(TreeList.AutoGenerateColumns) {
				PropertyDescriptor oldDescriptor = properties["Columns"] as PropertyDescriptor;
				if(oldDescriptor != null) {
					Attribute[] attrs = new Attribute[] { new BrowsableAttribute(false) };
					properties["Columns"] = TypeDescriptor.CreateProperty(typeof(ASPxTreeListDesigner), oldDescriptor, attrs);
				}
				TypeDescriptor.Refresh(this);
			}
		}
		[DefaultValue(true)]
		public bool AutoGenerateColumns {
			get { return TreeList.AutoGenerateColumns; }
			set {
				if(TreeList.AutoGenerateColumns == value)
					return;
				TreeList.AutoGenerateColumns = value;
				if(AutoGenerateColumns)
					ComponentChanged();
			}
		}
		protected override void OnSchemaRefreshed() {
			base.OnSchemaRefreshed();
			Cursor cursor = Cursor.Current;
			try {
				Cursor.Current = Cursors.WaitCursor;
				ControlDesigner.InvokeTransactedChange(Component, new TransactedChangeCallback(OnSchemaRefreshedCallback), null, TreeListDesignerSR.SchemaRefreshedTransactionName);
				UpdateDesignTimeHtml();
			} finally {
				Cursor.Current = cursor;
			}
		}
		protected override bool IsControlRequireHttpHandlerRegistration() {
			return TreeList != null && TreeList.Columns != null && TreeList.Columns.OfType<WebColumnBase>().Any(x => ColumnTypesRequireHttpHandlerRegistration.Any(t => x.GetType().IsAssignableFrom(t)));
		}
		bool OnSchemaRefreshedCallback(object context) {
			IDataSourceViewSchema schema = GetDataSourceSchema();
			bool dataSettingsExists = TreeList.Columns.Count > 0
				|| !String.IsNullOrEmpty(TreeList.KeyFieldName)
				|| !String.IsNullOrEmpty(TreeList.ParentFieldName);
			bool dataSourceExists = schema != null && !string.IsNullOrEmpty(DataSourceID);
			if(dataSourceExists) {
				if(dataSettingsExists) {
					string caption = string.Format(TreeListDesignerSR.RefreshCaptionFormat, TreeList.ID);
					string message = string.Format("{0}\n{1}", TreeListDesignerSR.RegenerateQuestion, TreeListDesignerSR.Warning);
					if(MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
						ResetDataSettings();
						CreateDataSettings(schema, true);
					}
				} else
					CreateDataSettings(schema, true);
				return true;
			}
			if(dataSettingsExists) {
				string caption = string.Format(TreeListDesignerSR.ClearCaptionFormat, TreeList.ID);
				string message = string.Format("{0}\n{1}", TreeListDesignerSR.ClearQuestion, TreeListDesignerSR.Warning);
				if(MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					ResetDataSettings();
			}
			return true;
		}	   
		protected internal void CreateDataSettings(IDataSourceViewSchema schema, bool createKeys) {
			IDataSourceFieldSchema[] fields = schema.GetFields();
			if(fields == null || fields.Length < 1)
				return;
			string primaryKeyName = null;
			List<IDataSourceFieldSchema> bindableFields = new List<IDataSourceFieldSchema>();
			foreach(IDataSourceFieldSchema field in fields) {
				if(!IsBindableType(field.DataType))
					continue;
				if(field.PrimaryKey && primaryKeyName == null)
					primaryKeyName = field.Name;
				bindableFields.Add(field);				
			}
			if(createKeys && !string.IsNullOrEmpty(primaryKeyName)) {
				TreeList.KeyFieldName = primaryKeyName;
				string parentKeyName = TryFindParentKeyFieldName(primaryKeyName, fields);
				if(!string.IsNullOrEmpty(parentKeyName))
					TreeList.ParentFieldName = parentKeyName;
			}
			TreeList.AutoGenerateColumns = false;
			foreach(IDataSourceFieldSchema field in bindableFields) {
				string fieldName = field.Name;
				if(TreeList.AutoGenerateServiceColumns || !TreeList.IsServiceFieldName(fieldName)) {
					TreeListDataColumn column = TreeListDataColumn.CreateInstance(field.DataType);
					column.FieldName = fieldName;
					column.ReadOnly = field.IsReadOnly || field.PrimaryKey;
					column.Visible = !field.Identity;
					TreeList.Columns.Add(column);					
				}
			}
		}
		void ResetDataSettings() {
			TreeList.Columns.Clear();
			TreeList.KeyFieldName = String.Empty;
			TreeList.ParentFieldName = String.Empty;
			TreeList.PreviewFieldName = String.Empty;
			TreeList.AutoGenerateColumns = true;
		}
		bool IsBindableType(Type type) {
			return type.IsPrimitive || type == typeof(string) || type == typeof(Decimal) || type == typeof(DateTime) || type == typeof(Guid);
		}
		string TryFindParentKeyFieldName(string keyFieldName, IDataSourceFieldSchema[] fields) {
			string keyToLower = keyFieldName.ToLower();
			foreach(IDataSourceFieldSchema field in fields) {
				string candidateToLower = field.Name.ToLower();
				if(candidateToLower == "parent" + keyToLower
					|| candidateToLower == "parent_" + keyToLower)
					return field.Name;
			}
			return null;
		}
		protected internal IDataSourceViewSchema GetDataSourceSchema() {
			DesignerDataSourceView designerView = DesignerView;
			if(designerView != null)
				return designerView.Schema;
			return null;
		}
		IHierarchicalDataSourceDesigner GetHierarchicalDataSourceDesigner() {
			IHierarchicalDataSourceDesigner designer = null;
			string dataSourceID = base.DataSourceID;
			if(!string.IsNullOrEmpty(dataSourceID)) {
				System.Web.UI.Control component = DataControlHelper.FindControl((System.Web.UI.Control)base.Component, dataSourceID);
				if((component != null) && (component.Site != null)) {
					IDesignerHost service = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
					if(service != null) {
						designer = service.GetDesigner(component) as IHierarchicalDataSourceDesigner;
					}
				}
			}
			return designer;
		}
		DesignerHierarchicalDataSourceView GetHierarchicalDesignerView(IHierarchicalDataSourceDesigner dataSourceDesigner) {
			DesignerHierarchicalDataSourceView view = null;
			if(dataSourceDesigner != null) {
				view = dataSourceDesigner.GetView(string.Empty);
			}
			return view;
		}
		protected override IEnumerable GetDesignTimeDataSource() {
			if(DesignerHierarchicalView == null)
				return base.GetDesignTimeDataSource();
			bool dummy;
			IHierarchicalEnumerable enumerable = DesignerHierarchicalView.GetDesignTimeData(out dummy);
			if(enumerable == null) {
				DataBinding binding = DataBindings["DataSource"];
				if(binding != null)
					enumerable = DesignTimeData.GetSelectedDataSource(Component, binding.Expression, null) as IHierarchicalEnumerable;
			}
			if(enumerable != null) {
				ICollection collection = enumerable as ICollection;
				if(collection == null || collection.Count > 0)
					return enumerable;
			}
			return null;
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection collection = base.CreateTemplateGroups();
			foreach(TreeListColumn column in TreeList.Columns) {
				string groupName = String.Format(ColumnTemplateFormat, column.Index, column.GetCaption());
				TemplateGroup group = new TemplateGroup(groupName);
				FillTemplateGroup(group, column, ColumnTemplateNames);
				if(column is TreeListDataColumn)
					FillTemplateGroup(group, column, DataColumnTemplateNames);
				collection.Add(group);
			}
			foreach(string name in RootTemplateNames) {
				TemplateGroup group = new TemplateGroup(name);
				TemplateDefinition definition = new TemplateDefinition(this, name, TreeList.Templates, name);
				definition.SupportsDataBinding = true;
				group.AddTemplateDefinition(definition);
				collection.Add(group);
			}
			return collection;
		}
		void FillTemplateGroup(TemplateGroup group, object obj, IEnumerable<string> names) {
			foreach(string name in names) {
				TemplateDefinition definition = new TemplateDefinition(this, name, obj, name);
				definition.SupportsDataBinding = true;
				group.AddTemplateDefinition(definition);
			}
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new TreeListDesignerActionList(this);
		}
		public override void ShowAbout() {
			TreeListAboutDialogHelper.ShowAbout(Component.Site);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new TreeListCommonFormDesigner(TreeList, DesignerHost)));
		}
	 }
	public class TreeListDesignerActionList : ASPxWebControlDesignerActionList {
		public const string SettingsCategoryName = "Settings";
		public const string EditingCategoryName = "Editing";
		public const string AllowModifyCategoryName = "AllowModify";
		private ASPxTreeListDesigner designer;
		public TreeListDesignerActionList(ASPxTreeListDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		protected ASPxTreeList TreeList {
			get { return designer.TreeList; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("ShowPager", StringResources.TreeListActionList_ShowPager, SettingsCategoryName));
			collection.Add(new DesignerActionPropertyItem("ShowColumnHeaders", StringResources.TreeListActionList_ShowColumnHeaders, SettingsCategoryName));
			collection.Add(new DesignerActionPropertyItem("ShowTreeLines", StringResources.TreeListActionList_ShowTreeLines, SettingsCategoryName));
			collection.Add(new DesignerActionPropertyItem("EnableFocusedNode", StringResources.TreeListActionList_EnableFocusedNode, SettingsCategoryName));
			collection.Add(new DesignerActionPropertyItem("ShowSelectCheckBox", StringResources.TreeListActionList_ShowSelectCheckBox, SettingsCategoryName));
			collection.Add(new DesignerActionPropertyItem("EnableAutoExpandAll", StringResources.TreeListActionList_EnableAutoExpandAll, SettingsCategoryName));
			collection.Add(new DesignerActionPropertyItem("ShowEditButton", StringResources.TreeListActionList_ShowEditButton, EditingCategoryName));
			collection.Add(new DesignerActionPropertyItem("ShowNewButton", StringResources.TreeListActionList_ShowNewButton, EditingCategoryName));
			collection.Add(new DesignerActionPropertyItem("ShowDeleteButton", StringResources.TreeListActionList_ShowDeleteButton, EditingCategoryName));
			collection.Add(new DesignerActionPropertyItem("AllowEdit", StringResources.DataEditingActionList_AllowEdit, AllowModifyCategoryName));
			collection.Add(new DesignerActionPropertyItem("AllowInsert", StringResources.DataEditingActionList_AllowInsert, AllowModifyCategoryName));
			collection.Add(new DesignerActionPropertyItem("AllowDelete", StringResources.DataEditingActionList_AllowDelete, AllowModifyCategoryName));
			collection.Add(new DesignerActionPropertyItem("EnableNodeDragging", StringResources.TreeListActionList_EnableNodeDragging, AllowModifyCategoryName));
			return collection;
		}
		public bool ShowPager {
			get { return TreeList.SettingsPager.Mode == TreeListPagerMode.ShowPager && TreeList.SettingsPager.Visible; }
			set {
				if(value) {
					TreeList.SettingsPager.Mode = TreeListPagerMode.ShowPager;
					TreeList.SettingsPager.Visible = value;
				}
				else {
					TreeList.SettingsPager.Mode = TreeListPagerMode.ShowAllNodes;
				}
				FireControlPropertyChanged("SettingsPager");
			}
		}
		public bool ShowColumnHeaders {
			get { return TreeList.Settings.ShowColumnHeaders; }
			set {
				TreeList.Settings.ShowColumnHeaders = value;
				FireControlPropertyChanged("Settings");
			}
		}
		public bool ShowTreeLines {
			get { return TreeList.Settings.ShowTreeLines; }
			set {
				TreeList.Settings.ShowTreeLines = value;
				FireControlPropertyChanged("Settings");
			}
		}
		public bool EnableFocusedNode {
			get { return TreeList.SettingsBehavior.AllowFocusedNode; }
			set {
				TreeList.SettingsBehavior.AllowFocusedNode = value;
				FireControlPropertyChanged("SettingsBehavior");
			}
		}
		public bool ShowSelectCheckBox {
			get { return TreeList.SettingsSelection.Enabled; }
			set {
				TreeList.SettingsSelection.Enabled = value;
				FireControlPropertyChanged("SettingsSelection");
			}
		}
		public bool EnableAutoExpandAll {
			get { return TreeList.SettingsBehavior.AutoExpandAllNodes; }
			set {
				TreeList.SettingsBehavior.AutoExpandAllNodes = value;
				FireControlPropertyChanged("SettingsBehavior");
			}
		}		
		public bool ShowEditButton {
			get {
				TreeListCommandColumn column = GetCommandColumn(false);
				return column != null && column.EditButton.Visible;
			}
			set {
				GetCommandColumn(true).EditButton.Visible = value;
				FireControlPropertyChanged("Columns");
			}
		}
		public bool ShowNewButton {
			get {
				TreeListCommandColumn column = GetCommandColumn(false);
				return column != null && column.NewButton.Visible;
			}
			set {
				GetCommandColumn(true).NewButton.Visible = value;
				FireControlPropertyChanged("Columns");
			}
		}
		public bool ShowDeleteButton {
			get {
				TreeListCommandColumn column = GetCommandColumn(false);
				return column != null && column.DeleteButton.Visible;
			}
			set {
				GetCommandColumn(true).DeleteButton.Visible = value;
				FireControlPropertyChanged("Columns");
			}
		}
		public bool AllowEdit {
			get { return TreeList.SettingsDataSecurity.AllowEdit; }
			set {
				TreeList.SettingsDataSecurity.AllowEdit = value;
				FireControlPropertyChanged("Columns");
			}
		}
		public bool AllowInsert {
			get { return TreeList.SettingsDataSecurity.AllowInsert; }
			set {
				TreeList.SettingsDataSecurity.AllowInsert = value;
				FireControlPropertyChanged("Columns");
			}
		}
		public bool AllowDelete {
			get { return TreeList.SettingsDataSecurity.AllowDelete; }
			set {
				TreeList.SettingsDataSecurity.AllowDelete = value;
				FireControlPropertyChanged("Columns");
			}
		}
		public bool EnableNodeDragging {
			get { return TreeList.SettingsEditing.AllowNodeDragDrop; }
			set {
				TreeList.SettingsEditing.AllowNodeDragDrop = value;
				FireControlPropertyChanged("SettingsEditing");
			}
		}
		void FireControlPropertyChanged(string propertyName) {
			System.Web.UI.Design.ControlDesigner.InvokeTransactedChange(Component, delegate(object arg) {				
				TypeDescriptor.Refresh(TreeList);
				EditorContextHelper.FireChanged(Designer, TreeList, propertyName);
				EditorContextHelper.RefreshSmartPanel(Component);
				return true;
			}, null, string.Format("{0} changed", propertyName));
		}
		List<TreeListCommandColumn> GetCommandColumns() {
			List<TreeListCommandColumn> list = new List<TreeListCommandColumn>();
			foreach(TreeListColumn column in TreeList.Columns) {
				TreeListCommandColumn cmdColumn = column as TreeListCommandColumn;
				if(cmdColumn != null) list.Add(cmdColumn);
			}
			return list;
		}
		TreeListCommandColumn GetCommandColumn(bool createIfNotFound) {
			foreach(TreeListColumn column in TreeList.Columns) {
				TreeListCommandColumn cmdColumn = column as TreeListCommandColumn;
				if(cmdColumn != null)
					return cmdColumn;
			}
			if(!createIfNotFound)
				return null;
			TreeListCommandColumn newColumn = new TreeListCommandColumn();
			TreeList.Columns.Add(newColumn);
			newColumn.VisibleIndex = TreeList.Columns.Count - 1;
			TreeList.AutoGenerateColumns = false;
			return newColumn;
		}
		void RemoveEmptyCommandColumns() {			
			List<TreeListCommandColumn> columnsToRemove = new List<TreeListCommandColumn>();
			foreach(TreeListCommandColumn column in GetCommandColumns()) {
				if(!column.EditButton.Visible && !column.NewButton.Visible && !column.DeleteButton.Visible)
					columnsToRemove.Add(column);
			}
			foreach(TreeListCommandColumn column in columnsToRemove)
				TreeList.Columns.Remove(column);
		}
	}
	internal static class TreeListDesignerSR {
		public const string
			RetrieveColumnsButtonText = "Retrieve columns";
		public const string
			ClearCaptionFormat = "Clear columns and keys for {0}",
			ClearQuestion = "Would you like to clear the ASPxTreeList columns and keys?",
			RefreshCaptionFormat = "Refresh columns and keys for {0}",
			RegenerateQuestion = "Would you like to regenerate the ASPxTreeList columns and keys using the selected data source schema?",			
			RefreshNoKeysCaptionFormat = "Refresh columns for {0}",
			RegenerateNoKeysQuestion = "Would you like to regenerate the ASPxTreeList columns?",			
			Warning = "Warning: this will delete all existing columns.",
			SchemaRefreshedTransactionName = "OnSchemaRefreshed";
	}
	public class TreeListCommonFormDesigner : CommonFormDesigner {
		public TreeListCommonFormDesigner(ASPxTreeList treeList, IServiceProvider provider)
			: base(new TreeListColumnsOwner(treeList, provider)) {
			ItemsImageIndex = ColumnsItemImageIndex;
		}
		ASPxTreeList TreeList { get { return (ASPxTreeList)Control; } }
		protected override void CreateGroups() {
			base.CreateGroups();
			CreateGroupSummaryItem();
		}
		protected void CreateGroupSummaryItem() {
			Groups.Add(SummaryGroupCaption, SummaryGroupCaption, GetDefaultGroupImage(SummaryGroupImageIndex), false);
			Groups[SummaryGroupCaption].Add(CreateDesignerItem(new TreeListSummaryItemsOwner(TreeList, Provider), typeof(ItemsEditorFrame), TotalSummaryItemImageIndex));
		}
	}
	public class TreeListSummaryItemsOwner : FlatCollectionItemsOwner<TreeListSummaryItem> {
		public TreeListSummaryItemsOwner(ASPxTreeList treeList, IServiceProvider provider)
			: base(treeList, provider, treeList.Summary, "Summary") {
		}
	}   
}
