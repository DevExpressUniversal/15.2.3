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
using System.Text;
using System.Globalization;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI;
using System.ComponentModel;
using DevExpress.Web.FilterControl;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using System.Diagnostics;
using System.Linq;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Utils;
namespace DevExpress.Web.FilterControl {
	public interface IFilterControlOwner {
		string FilterExpression { get; set; }
		bool IsRightToLeft { get; }
		bool TryGetSpecialValueDisplayText(IFilterColumn column, object value, bool encodeValue, out string displayText);
		bool IsOperationHiddenByUser(IFilterablePropertyInfo propertyInfo, ClauseType operation);
		bool TryConvertValue(IFilterablePropertyInfo propertyInfo, string text, out object value);
		FilterControlViewMode ViewMode { get; }
		bool ShowOperandTypeButton { get; }
		FilterControlGroupOperationsVisibility GroupOperationsVisibility { get; }
		FilterControlColumnCollection GetFilterColumns();
		void RaiseCustomValueDisplayText(FilterControlCustomValueDisplayTextEventArgs e);
		void RaiseCriteriaValueEditorInitialize(FilterControlCriteriaValueEditorInitializeEventArgs e);
		void RaiseCriteriaValueEditorCreate(FilterControlCriteriaValueEditorCreateEventArgs e);
	}
	public interface IFilterColumn : IFilterablePropertyInfo {
		int Index { get; }
		FilterColumnClauseClass ClauseClass { get; }
		EditPropertiesBase PropertiesEdit { get; }
	}
	public class WebFilterOperations : WebFilterOperationsBase {
		IFilterControlOwner filterOwner;
		public WebFilterOperations(IFilterControlOwner filterOwner, string criteria, WebFilterTreeModel model)
			: base(criteria, model) {
			this.filterOwner = filterOwner;
		}
		protected IFilterControlOwner FilterOwner { get { return filterOwner; } }
		protected override string GetDefaultPropertyName() {
			return FilterOwner.GetFilterColumns().Count > 0 ? FilterOwner.GetFilterColumns()[0].PropertyName : string.Empty;
		}
		protected override Type GetTypeByPropertyName(string propertyName) {
			IFilterColumn column = GetColumnByPropertyName(propertyName);
			Type type = column != null && column.PropertyType != null ? column.PropertyType: typeof(string);			
			return ReflectionUtils.StripNullableType(type);
		}
		protected override FilterColumnClauseClass GetClauseClassByPropertyName(string propertyName) {
			IFilterColumn column = GetColumnByPropertyName(propertyName);
			return column != null ? column.ClauseClass : FilterColumnClauseClass.String; 
		}
		protected IFilterColumn GetColumnByPropertyName(string propertyName) {
			return FilterOwner.GetFilterColumns()[propertyName];
		}
		protected override void CorrectOperation(ClauseNode clauseNode) {
			if(clauseNode.Property == null) return;
			IFilterColumn column = GetColumnByPropertyName(clauseNode.Property.GetFullNameWithLists());
			clauseNode.Operation = Model.CorrectOperation(column, clauseNode.Operation);
			clauseNode.ValidateAdditionalOperands();
		}
		protected override bool TryConverValueCustom(string propertyName, string text, out object value) {
			return FilterOwner.TryConvertValue(GetColumnByPropertyName(propertyName), text, out value);
		}
	}
	public class FilterControlColumnBuilder {
		public const char ComplexPropertySeparator = '.';
		List<string> VisibleColumnNames { get; set; }
		public FilterControlColumnBuilder() { 
			Model = CreateFilterTreeModel();
		}
		public FilterControlColumnCollection GenerateColumns(object sourceType, bool allowHierarchicalColumns, int maxHierarchyDepth, bool showAllDataSourceColumns, IEnumerable<IFilterColumn> externalColumns) {
			Init(sourceType, allowHierarchicalColumns, maxHierarchyDepth, externalColumns, showAllDataSourceColumns);
			Model.SourceControl = SourceType;
			if(AllowHierarchicalColumns && !ShowAllDataSourceColumns)
				SetVisibleColumnNames();
			CreateColumnHierarchy(Columns, Properties.OfType<IBoundProperty>());
			if(!AllowHierarchicalColumns)
				CreateMissingExternalColumns();
			SortColumns(Columns);
			return Columns;
		}
		void Init(object sourceType, bool allowHierarchicalColumns, int maxHierarchyDepth, IEnumerable<IFilterColumn> externalColumns, bool showAllDataSourceColumns) {
			Columns = new FilterControlColumnCollection(null);
			SourceType = sourceType;
			AllowHierarchicalColumns = allowHierarchicalColumns;
			VisibleColumnNames = new List<string>();
			ExternalColumns = externalColumns.Distinct(new IFilterColumnComparer());
			ShowAllDataSourceColumns = showAllDataSourceColumns;
			MaxHierarchyDepth = maxHierarchyDepth;
		}
		protected object SourceType { get; private set; }
		protected bool AllowHierarchicalColumns { get; private set; }
		protected bool ShowAllDataSourceColumns { get; private set; }
		protected int MaxHierarchyDepth { get; private set; }
		protected IEnumerable<IFilterColumn> ExternalColumns { get; private set; }
		protected FilterControlColumnCollection Columns { get; private set; }
		protected WebFilterTreeModel Model { get; private set; }
		protected IBoundPropertyCollection Properties { get { return Model.FilterProperties; } }
		protected virtual bool DesignMode { get { return System.Web.HttpContext.Current == null; } }
		protected virtual WebFilterTreeModel CreateFilterTreeModel() {
			return new WebFilterTreeModel(new ASPxFilterControl());
		}
		void CreateColumnHierarchy(FilterControlColumnCollection columns, IEnumerable<IBoundProperty> properties) {
			foreach(var prop in properties) {
				if(prop.GetLevel() > MaxHierarchyDepth) return;
				var column = CreateFilterControlColumn(prop);
				if(column == null)
					continue;
				columns.Add(column);
				if(AllowHierarchicalColumns && prop.HasChildren)
					CreateColumnHierarchy(((FilterControlComplexTypeColumn)column).Columns, prop.Children);
			}
		}
		protected virtual void CreateMissingExternalColumns() {
			var missingExternalColumns = ExternalColumns.Where(c => Columns[c.PropertyName] == null);
			foreach(var externalColumn in missingExternalColumns) {
				var column = CreateFilterControlColumn(externalColumn);
				if(column != null)
					Columns.Add(column);
			}
		}
		protected virtual void SetVisibleColumnNames() {
			var complexColumns = ExternalColumns.Where(c => c.PropertyName.Contains(ComplexPropertySeparator));
			foreach(var column in complexColumns) {
				var prop = FindPropertyByName(column.PropertyName);
				VisibleColumnNames.AddRange(GetTreeLineNodePaths(prop)); 
			}
		}
		IBoundProperty FindPropertyByName(string propertyName) {
			IBoundProperty res = null;
			var properties = Properties.Cast<IBoundProperty>();
			foreach(var prop in propertyName.Split(FilterControlColumnBuilder.ComplexPropertySeparator)) {
				res = properties.SingleOrDefault(p => p.Name == prop);
				if(res != null)
					properties = res.Children;
			}
			return res;
		}
		List<string> GetTreeLineNodePaths(IBoundProperty prop) { 
			var paths = new List<string>();
			while(prop != null) {
				paths.Add(prop.GetFullName());
				prop = prop.Parent;
			}
			return paths;
		}
		protected virtual FilterControlColumn CreateFilterControlColumn(IBoundProperty prop) {
			var externalColumn = ExternalColumns.SingleOrDefault(c => c.PropertyName == prop.GetFullName());
			if(externalColumn != null && !prop.HasChildren) { 
				var column = CreateFilterControlColumn(externalColumn);
				column.PropertyName = prop.Name;
				return column;
			}
			if(ShowAllDataSourceColumns || VisibleColumnNames.Contains(prop.GetFullName()))
				return CreateFilterControlColumnByProperty(prop);
			return null;
		}
		protected virtual void SortColumns(FilterControlColumnCollection columns) {
			var sortedColumns = columns.Cast<FilterControlColumn>().OrderBy(col => col.DisplayName).ToList();
			columns.Clear();
			foreach(var column in sortedColumns) {
				columns.Add(column);
				var complexColumn = column as FilterControlComplexTypeColumn;
				if(complexColumn != null)
					SortColumns(complexColumn.Columns);
			}
		}
		FilterControlColumn CreateFilterControlColumnByProperty(IBoundProperty prop) { 
			var column = CreateFilterControlColumnCore(prop);
			column.PropertyName = prop.Name;
			column.DisplayName = prop.DisplayName;
			column.InternalColumnType = prop.Type;
			return column;
		}
		FilterControlColumn CreateFilterControlColumnCore(IBoundProperty prop) {
			if(prop.HasChildren || prop.IsList || prop.Type == typeof(byte[]))
				return new FilterControlComplexTypeColumn() { DataType = prop.IsList ? FilterControlDataType.List : FilterControlDataType.Object };
			if(prop.Type.IsEnum)
				return CreateComboboxColumnByEnumType(prop.Type);
			return CreateFilterControlColumn(prop.Type);
		}
		FilterControlColumn CreateFilterControlColumn(IFilterColumn externalColumn) {
			var column = CreateFilterControlColumn(externalColumn.PropertiesEdit);
			if(column == null) return null;
			column.CorrespondingExternalColumn = externalColumn;
			column.PropertyName = externalColumn.PropertyName;
			column.DisplayName = externalColumn.DisplayName;
			SetPropertiesEdit(column, externalColumn.PropertiesEdit);
			column.InternalColumnType = externalColumn.PropertyType;
			return column;
		}
		FilterControlColumn CreateFilterControlColumn(Type type) {
			var columnType = FilterControlColumn.GetColumnTypeByType(type);
			switch(columnType) {
				case FilterControlColumnType.String:
					return new FilterControlTextColumn();
				case FilterControlColumnType.Boolean: 
					return new FilterControlCheckColumn();
				case FilterControlColumnType.DateTime: 
					return new FilterControlDateColumn();
				case FilterControlColumnType.Integer: 
				case FilterControlColumnType.Decimal:
				case FilterControlColumnType.Double: 
					var column = new FilterControlSpinEditColumn();
					column.PropertiesSpinEdit.NumberType = columnType == FilterControlColumnType.Integer ? SpinEditNumberType.Integer : SpinEditNumberType.Float;
					return column;
			}
			return new FilterControlColumn();
		}
		FilterControlColumn CreateFilterControlColumn(EditPropertiesBase editProp) {
			if(editProp == null) return new FilterControlColumn();
			var type = editProp.GetType();
			if(type == typeof(SpinEditProperties)) return new FilterControlSpinEditColumn();
			if(type == typeof(ComboBoxProperties)) return new FilterControlComboBoxColumn();
			if(type == typeof(DateEditProperties)) return new FilterControlDateColumn();
			if(type == typeof(ButtonEditProperties)) return new FilterControlButtonEditColumn();
			if(type == typeof(HyperLinkProperties)) return new FilterControlHyperLinkColumn();
			if(type == typeof(MemoProperties)) return new FilterControlMemoColumn();
			if(type == typeof(CheckBoxProperties)) return new FilterControlCheckColumn();
			if(type == typeof(BinaryImageEditProperties)) return new FilterControlComplexTypeColumn();
			return new FilterControlTextColumn();
		}
		FilterControlColumn CreateComboboxColumnByEnumType(Type type) {
			var column = new FilterControlComboBoxColumn();
			var valueType = DesignMode ? typeof(int) : type;
			column.PropertiesComboBox.ValueType = valueType;
			foreach(var value in Enum.GetValues(type))
				column.PropertiesComboBox.Items.Add(CommonUtils.SplitPascalCaseString(Enum.GetName(type, value)), value);
			return column;
		}
		void SetPropertiesEdit(FilterControlColumn column, EditPropertiesBase propertiesEdit) {
			var propertiesToAssign = propertiesEdit;
			if(propertiesEdit is TokenBoxProperties) {
				var textBoxProperties = new TextBoxProperties();
				textBoxProperties.Assign(propertiesEdit);
				propertiesToAssign = textBoxProperties;
			}
			column.PropertiesEdit = propertiesToAssign;
		}
	}
	public class FilterControlDummyColumn : FilterControlColumn, IBoundProperty {
		List<IBoundProperty> children;
		string displayName, name;
		bool isList, isAggregate;
		Type type;
		IBoundProperty parent;
		public FilterControlDummyColumn(WebFilterTreeModel model, string displayName, string name, bool isList, bool isAggregate, Type type) {
			Model = model;
			this.displayName = displayName;
			this.name = name;
			this.isList = isList;
			this.isAggregate = isAggregate;
			this.type = type;
		}
		public WebFilterTreeModel Model { get; private set; }
		public void SetParent(IBoundProperty parent) { this.parent = parent; }
		protected override IBoundProperty GetParent() { return this.parent; }
		#region IBoundProperty Members
		List<IBoundProperty> IBoundProperty.Children {
			get {
				if(children == null)
					children = Model.GetChildrenProperties(this);
				return children;
			}
		}
		bool IBoundProperty.HasChildren { get { return ((IBoundProperty)this).Children.Count > 0; } }
		string IBoundProperty.DisplayName { get { return CommonUtils.SplitPascalCaseString(this.displayName); } }
		bool IBoundProperty.IsAggregate { get { return this.isAggregate; } }
		bool IBoundProperty.IsList { get { return this.isList; } }
		string IBoundProperty.Name { get { return this.name; } }
		Type IBoundProperty.Type { get { return this.type; } }
		#endregion
	}
	class IFilterColumnComparer : IEqualityComparer<IFilterColumn> {
		public bool Equals(IFilterColumn x, IFilterColumn y) {
			return x.PropertyName == y.PropertyName;
		}
		public int GetHashCode(IFilterColumn obj) {
			return obj.PropertyName.GetHashCode();
		}
	}
}
namespace DevExpress.Web {
	public interface IFilterablePropertyInfo {
		string PropertyName { get; }
		string DisplayName { get; }
		Type PropertyType { get; }
	}
	public enum FilterControlColumnType { Default, String, Integer, DateTime, Boolean, Double, Decimal }
	public class FilterControlColumn : CollectionItem, IBoundProperty, IFilterColumn {
		EditPropertiesBase propertiesEdit;
		protected FilterControlColumnCollection ColumnCollection { get { return Collection as FilterControlColumnCollection; } }
		protected internal Type InternalColumnType {
			get { return (Type)GetObjectProperty("InternalColumnType", null); }
			set { 
				if(value == InternalColumnType) return;
				SetObjectProperty("InternalColumnType", null, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlColumnPropertyName"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), RefreshProperties(RefreshProperties.Repaint),
		NotifyParentProperty(true)]
		public virtual string PropertyName {
			get { return GetStringProperty("PropertyName", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(value == PropertyName) return;
				SetStringProperty("PropertyName", string.Empty, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlColumnDisplayName"),
#endif
		DefaultValue(""), Localizable(false), RefreshProperties(RefreshProperties.Repaint),
		NotifyParentProperty(true)]
		public virtual string DisplayName {
			get { return GetStringProperty("DisplayName", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(value == DisplayName) return;
				SetStringProperty("DisplayName", string.Empty, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlColumnColumnType"),
#endif
		NotifyParentProperty(true), DefaultValue(FilterControlColumnType.Default)]
		public FilterControlColumnType ColumnType {
			get { return (FilterControlColumnType)GetEnumProperty("ColumnType", FilterControlColumnType.Default); }
			set {
				if(value == ColumnType) return;
				SetEnumProperty("ColumnType", FilterControlColumnType.Default, value);
				OnColumnChanged();
			}
		}
		[Category("Behavior"),
		Browsable(false), 
		PersistenceMode(PersistenceMode.InnerProperty),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)]
		public virtual EditPropertiesBase PropertiesEdit {
			get {  return propertiesEdit; }
			set {
				if(propertiesEdit == value) return;
				propertiesEdit = value;
				SetPropertiesEditTypeCore(value == null ? string.Empty : EditRegistrationInfo.GetEditName(value));
				OnColumnChanged();
			}
		}
		[DefaultValue(""), Localizable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), NotifyParentProperty(true)]
		public virtual string PropertiesEditType {
			get { return GetStringProperty("PropertiesEditType", string.Empty); }
			set {
				SetPropertiesEditTypeCore(value);
				PropertiesEdit = UpdateColumnEdit();
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("FilterControlColumnPropertyFullName")]
#endif
		public string PropertyFullName { get { return this.GetFullNameWithLists(); } }
		protected internal IFilterColumn CorrespondingExternalColumn { get; set; }
		protected virtual Type GetTypeByColumnType(FilterControlColumnType columnType) {
			switch(columnType) {
				case FilterControlColumnType.Boolean: return typeof(bool);
				case FilterControlColumnType.DateTime: return typeof(DateTime);
				case FilterControlColumnType.Decimal: return typeof(decimal);
				case FilterControlColumnType.Double: return typeof(double);
				case FilterControlColumnType.Integer: return typeof(int);
				case FilterControlColumnType.String: return typeof(string);
			}
			return null;
		}
		protected internal static FilterControlColumnType GetColumnTypeByType(Type type) {
			if (type == typeof(bool)) {
				return FilterControlColumnType.Boolean;
			}
			if (type == typeof(DateTime)) {
				return FilterControlColumnType.DateTime;
			}
			if (type == typeof(decimal)) {
				return FilterControlColumnType.Decimal;
			}
			if (type == typeof(double) ||
				type == typeof(float)) {
				return FilterControlColumnType.Double;
			}
			if (type == typeof(sbyte)  ||
				type == typeof(byte)   ||
				type == typeof(char)   ||
				type == typeof(short)  ||
				type == typeof(ushort) ||
				type == typeof(int)	||
				type == typeof(uint)   ||
				type == typeof(long)   ||
				type == typeof(ulong)) {
				return FilterControlColumnType.Integer;
			}
			return FilterControlColumnType.String;
		}
		protected Type GetInternalPropertyType() {
			if(InternalColumnType != null)
				return InternalColumnType;
			return GetPropertyType();
		}
		protected virtual Type GetPropertyType() {
			if(ColumnType != FilterControlColumnType.Default) return GetTypeByColumnType(ColumnType);
			if(string.IsNullOrEmpty(PropertiesEditType)) return typeof(string);
			return EditRegistrationInfo.GetEditType(PropertiesEditType);
		}
		protected virtual void OnColumnChanged() {
			if(ColumnCollection != null) {
				ColumnCollection.OnColumnCollectionChanged();
			}
		}
		void SetPropertiesEditTypeCore(string value) {
			if(value == null) value = string.Empty;
			SetStringProperty("PropertiesEditType", string.Empty, value);
		}
		EditPropertiesBase UpdateColumnEdit() {
			return EditRegistrationInfo.CreateProperties(PropertiesEditType);
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			FilterControlColumn column = source as FilterControlColumn;
			if(column != null) {
				PropertyName = column.PropertyName;
				DisplayName = column.DisplayName;
				ColumnType = column.ColumnType;
				PropertiesEditType = column.PropertiesEditType;
				if(PropertiesEdit != null)
					PropertiesEdit.Assign(column.PropertiesEdit);
			}
		}
		#region IFilterColumn Members
		string IFilterablePropertyInfo.PropertyName { get { return PropertyName; }  }
		string IFilterablePropertyInfo.DisplayName {  get { return GetDisplayName(); } }
		Type IFilterablePropertyInfo.PropertyType { get { return GetInternalPropertyType(); } }
		FilterColumnClauseClass IFilterColumn.ClauseClass { get { return GetClauseClass(); } }
		EditPropertiesBase IFilterColumn.PropertiesEdit { get { return PropertiesEdit; } }
		#endregion
		protected virtual FilterColumnClauseClass GetClauseClass() {
			if(PropertiesEdit != null && PropertiesEdit.GetEditorType() != EditorType.Generic) {
				if(PropertiesEdit.GetEditorType() == EditorType.Blob)
					return FilterColumnClauseClass.Blob;
				return FilterColumnClauseClass.Lookup;
			}
			var filterColumn = this as IFilterColumn;
			if(filterColumn.PropertyType == typeof(string))
				return FilterColumnClauseClass.String;
			if(filterColumn.PropertyType == typeof(DateTime))
				return FilterColumnClauseClass.DateTime;
			return FilterColumnClauseClass.Generic;
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(PropertyName))
				return PropertyName;
			return base.ToString();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(PropertiesEdit);
			return list.ToArray();
		}
		protected virtual string GetDisplayName() {
			return string.IsNullOrEmpty(DisplayName) ?  CommonUtils.SplitPascalCaseString(PropertyName) : DisplayName;
		}
		#region IBoundProperty Members
		List<IBoundProperty> children = new List<IBoundProperty>();
		List<IBoundProperty> IBoundProperty.Children { get { return GetChildren(); } }
		protected virtual List<IBoundProperty> GetChildren() {
			return children;
		}
		bool IBoundProperty.HasChildren { get { return (this as IBoundProperty).Children.Count > 0; } }
		bool IBoundProperty.IsAggregate { get { return false; } }
		bool IBoundProperty.IsList { get { return GetIsList(); } }
		string IBoundProperty.Name { get { return this.PropertyName; } }
		string IBoundProperty.DisplayName { get { return GetDisplayName(); } }
		IBoundProperty IBoundProperty.Parent { get { return GetParent(); } }
		Type IBoundProperty.Type {
			get { return GetInternalPropertyType(); }
		}
		internal void SetType(Type type) {
			ColumnType = GetColumnTypeByType(type);
		}
		protected virtual bool GetIsList() { return false; }
		protected virtual IBoundProperty GetParent() {
			return ColumnCollection != null ? ColumnCollection.Owner as FilterControlComplexTypeColumn : null;
		}
		#endregion
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class FilterControlColumnCollection : Collection, IBoundPropertyCollection {
		public FilterControlColumnCollection(IWebControlObject webControlObject)  : base(webControlObject) {
		}
		[Browsable(false)]
		public ASPxFilterControl FilterControl { get { return Owner as ASPxFilterControl; } }
		public override string ToString() { return string.Empty; }
		[Browsable(false)]
		public FilterControlColumn this[int index] {
			get { return GetItem(index) as FilterControlColumn; }
		}
		[Browsable(false)]
		public FilterControlColumn this[string propertyName] { 
			get { return FindByName(propertyName); }
		}
		protected override Type GetKnownType() {
			return typeof(FilterControlColumn);
		}
		public void Insert(int index, FilterControlColumn column) {
			base.Insert(index, column);
		}
		public void Remove(FilterControlColumn column) {
			base.Remove(column);
		}
		public void Remove(string columnName) {
			var column = FindByName(columnName);
			if(column == null)
				return;
			column.Collection.Remove(column);
		}
		public int IndexOf(FilterControlColumn column) {
			return base.IndexOf(column);
		}
		protected override void OnChanged() {
			base.OnChanged();
			OnColumnCollectionChanged();
		}
		protected internal void OnColumnCollectionChanged() {
			if(FilterControl != null) {
				FilterControl.OnColumnCollectionChanged();
			}
		}
		public void Add(FilterControlColumn column) {
			base.Add(column);
		}
		public void AddRange(params FilterControlColumn[] columns) {
			BeginUpdate();
			try {
				foreach(FilterControlColumn column in columns)
					Add(column);
			} finally {
				EndUpdate();
			}
		}
		protected FilterControlColumn FindByName(string columnName) {
			var parentColumn = Owner as FilterControlComplexTypeColumn;
			var prefix = parentColumn != null ? parentColumn.GetFullNameWithLists() : string.Empty;
			FilterControlColumn column = null;
			IterateFilterColumnsHierarchycally(this, col => {
				if(GetColumnFullName(col, prefix) == columnName)
					column = col;
				return column == col;
			});
			return column;
		}
		string GetColumnFullName(FilterControlColumn col, string prefix) {
			string columnFullName = col.GetFullNameWithLists();
			if(!string.IsNullOrEmpty(prefix) && columnFullName.Contains(prefix))
				return columnFullName.Remove(0, prefix.Length + 1);
			return columnFullName;
		}
		protected internal void IterateFilterColumnsHierarchycally(Func<FilterControlColumn, bool> action) {
			IterateFilterColumnsHierarchycally(this, action);
		}
		bool IterateFilterColumnsHierarchycally(FilterControlColumnCollection columns, Func<FilterControlColumn, bool> action) {
			foreach(FilterControlColumn col in columns) {
				if(action(col))
					return true;
				var complexTypeColumn = col as FilterControlComplexTypeColumn;
				if(complexTypeColumn != null && complexTypeColumn.Columns.Count > 0)
					if(IterateFilterColumnsHierarchycally((col as FilterControlComplexTypeColumn).Columns, action))
						return true;
			}
			return false;
		}
		#region IBoundPropertyCollection Members
		IBoundPropertyCollection IBoundPropertyCollection.CreateChildrenProperties(IBoundProperty listProperty) {
			return ((FilterControlComplexTypeColumn)listProperty).Columns;
		}
		string IBoundPropertyCollection.GetDisplayPropertyName(OperandProperty property, string fullPath) {
			Debug.Assert(!ReferenceEquals(property, null));
			if (this.GetProperty(property) != null) {
				return this.GetProperty(property).ToString();
			}
			return property.PropertyName;
		}
		string IBoundPropertyCollection.GetValueScreenText(OperandProperty operandProperty, object value) {
			if (value == null)
				return "emptyGVST";
			FilterControlColumn col = (FilterControlColumn)this.GetProperty(operandProperty);
			if (col != null)
				return "editorGVST";
			else
				return value.ToString();
		}
		IBoundProperty IBoundPropertyCollection.this[string fieldName] { get { return FindByName(fieldName); } }
		IBoundProperty IBoundPropertyCollection.this[int index] {
			get { return this[index]; }
		}
		void IBoundPropertyCollection.Add(IBoundProperty column) {
			base.Add((CollectionItem)column);
		}
		void IBoundPropertyCollection.Clear() {
			base.Clear();
		}
		int IBoundPropertyCollection.Count {
			get { return base.Count; }
		}
		#endregion
	}
}
