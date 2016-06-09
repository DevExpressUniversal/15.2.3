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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Localization;
namespace DevExpress.XtraEditors.Filtering {
	internal interface IWinFilterTreeNodeModel {
		void Rebuild();
		bool SortProperties { get; set; }		
		List<IBoundProperty> GetChildrenProperties(IBoundProperty property);
	}
	internal class FilterColumnSorters {
		private static int Compare(object x, object y) {
			FilterColumn col1 = (FilterColumn)x, col2 = (FilterColumn)y;
			return Comparer.Default.Compare(col1.ColumnCaption, col2.ColumnCaption);
		}
		internal class FilterColumnCollectionSorter : IComparer {
			int IComparer.Compare(object x, object y) {
				return FilterColumnSorters.Compare(x, y);
			}
		}
		internal class ITreeSelectableItemSorter : IComparer<ITreeSelectableItem> {
			public int Compare(ITreeSelectableItem x, ITreeSelectableItem y) {
				return FilterColumnSorters.Compare(x, y);
			}
		}
	}
	public class WinFilterTreeNodeModelBase : FilterTreeNodeModel, IWinFilterTreeNodeModel {
		public override Type GetFunctionType(string name) {
			return FilterControlViewInfo.GetFunctionType(name);
		}
		public override IBoundProperty CreateProperty(object dataSource, string dataMember, string displayName, bool isList, PropertyDescriptor property) {
			DataColumnInfoFilterColumn filterColumn = new DataColumnInfoFilterColumn(new DataColumnInfo(property), isList);
			filterColumn.SetColumnCaption(displayName);
			filterColumn.Model = this;
			filterColumn.SetIsAggregate(!isList && IsAggregate(property));
			filterColumn.CreateCustomRepositoryItem += delegate(object sender, CreateCustomRepositoryItemEventArgs args) {
				OnCreateCustomRepositoryItem(args);
			};
			return filterColumn;
		}
		protected virtual void OnCreateCustomRepositoryItem(CreateCustomRepositoryItemEventArgs args) { }
		protected override void SetFilterColumnsCollection(IBoundPropertyCollection propertyCollection) { }
		public override void OnVisualChange(FilterChangedActionInternal action, Node node) { }
		static bool IsAggregate(PropertyDescriptor property) {			
			foreach (var attr in property.Attributes) {
				if (attr.ToString() == "DevExpress.Xpo.AggregatedAttribute")
					return true;
			}
			return false;
		}
		public override void SetParent(IBoundProperty property, IBoundProperty parent) {
			((FilterColumn)property).Parent = parent;
		}
		public override bool DoesAllowItemCollectionEditor(IBoundProperty property) {
			return ((FilterColumn)property).AllowItemCollectionEditor;
		}
		protected override FilterColumnClauseClass GetClauseClass(IBoundProperty property) {
			return ((FilterColumn)property).ClauseClass;
		}
		public override string GetLocalizedStringForFilterEmptyParameter() {
			return Localizer.Active.GetLocalizedString(StringId.FilterEmptyParameter);
		}
		public override string GetLocalizedStringForFilterEmptyEnter() {
			return Localizer.Active.GetLocalizedString(StringId.FilterEmptyEnter);
		}
		public override string GetLocalizedStringForFilterClauseBetweenAnd() {
			return Localizer.Active.GetLocalizedString(StringId.FilterClauseBetweenAnd);
		}
		public override string GetLocalizedStringForFilterEmptyValue() {
			return Localizer.Active.GetLocalizedString(StringId.FilterEmptyValue);
		}
		public override string GetMenuStringByType(GroupType type) {
			return OperationHelper.GetMenuStringByType(type);
		}
		public override string GetMenuStringByType(Aggregate type) {
			return OperationHelper.GetMenuStringByType(type);
		}
		public override string GetMenuStringByType(ClauseType type) {
			return OperationHelper.GetMenuStringByType(type);
		}
		protected override IBoundPropertyCollection CreateIBoundPropertyCollection() {
			var filterColumns =  new FilterColumnCollection();
			filterColumns.Model = this;
			return filterColumns;
		}
		#region IWinFilterTreeNodeModel
		void IWinFilterTreeNodeModel.Rebuild() {
			RebuildElements();
		}
		List<IBoundProperty> IWinFilterTreeNodeModel.GetChildrenProperties(IBoundProperty property) {
			var children = PickManager.PickProperties(SourceControl, property.GetFullNameWithLists(), property.Type);
			foreach (var child in children) {
				((FilterColumn)child).Parent = property;
			}
			return children;
		}
		#endregion
	}
	public class WinFilterTreeNodeModel : WinFilterTreeNodeModelBase {
		FilterControl control;
		internal Dictionary<Node, FilterControlLabelInfo> labels;
		public WinFilterTreeNodeModel(FilterControl control) {
			this.control = control;
			this.labels = new Dictionary<Node, FilterControlLabelInfo>();			
		}
		public FilterControlLabelInfo this[Node node] { 
			get { 
				if(this.labels.ContainsKey(node))
					return this.labels[node];
				return null;
			} 
		}
		public bool IsLabelRegistered(Node node) {
			return this.labels.ContainsKey(node);
		}
		public FilterControl Control { get { return control; } }
		protected override void OnCreateCustomRepositoryItem(CreateCustomRepositoryItemEventArgs args) {
			if (control != null) {
				control.RaiseCreateCustomRepositoryItem(args);
			}
		}
		protected override void OnFocusInfoChanged() {
			Control.OnFocusedElementChanged();
		}
		void RaiseVisualChange(FilterChangedActionInternal action, Node node) {
			if(VisualChange != null)
				VisualChange(action, node);
		}
		public delegate void OnVisualChangeDelegate(FilterChangedActionInternal action, Node node);
		public event OnVisualChangeDelegate VisualChange;
		public override void OnVisualChange(FilterChangedActionInternal action, Node node) {
			Debug.Print("OnVisualChange:{0},{1},{2}", action, node, node != null ? node.GetHashCode().ToString() : "null");
			switch (action) {
				case FilterChangedActionInternal.NodeAdded:
					this.labels[node] = new FilterControlLabelInfo(node);		   
					break;
				case FilterChangedActionInternal.NodeParentChanged:
					CreateLabelInfoTexts(node);
					break;
				case FilterChangedActionInternal.NodeElementsRebuilt:
					if (node == null || !labels.ContainsKey(node))
						return;
					this[node].Clear(); 
					this[node].CreateLabelInfoTexts();
					break;
				case FilterChangedActionInternal.NodeRemoved:
					if (node == null || !labels.ContainsKey(node))
						return;
					this.labels.Remove(node);
					break;
				case FilterChangedActionInternal.RootNodeReplaced:
					labels.Clear();
					RecursiveVisitor(RootNode, child => {
						var info = new FilterControlLabelInfo(child);
						info.Clear();
						info.CreateLabelInfoTexts();
						labels[child] = info;
					});
					break;
				default:
					Debug.Assert(false);
					break;
			} 
			RaiseVisualChange(action, node);
		}
		public void ClearActiveItem() {
			foreach (FilterControlLabelInfo label in this.labels.Values) {
				label.ViewInfo.ActiveItem = null;
			}
		}		
		public FilterControlLabelInfo GetLabelInfoByCoordinates(int x, int y) {
			foreach (FilterControlLabelInfo label in this.labels.Values) {
				if (label.TextBounds.Contains(x, y)) return label;
			}
			return null;
		}
		public void CalcSizes(ControlGraphicsInfoArgs info) {
			this[RootNode].CalcTextBounds(info);
		}
		void CreateLabelInfoTexts(Node node) {
			OnVisualChange(FilterChangedActionInternal.NodeElementsRebuilt, node);			
			foreach (Node child in node.GetChildren()) {
				CreateLabelInfoTexts(child);
			}
		}
		protected override void SetFilterColumnsCollection(IBoundPropertyCollection propertyCollection) {
			control.SetFilterColumnsCollection((FilterColumnCollection)propertyCollection);
		}
	}
	public class OperationHelper {
		public static string GetMenuStringByType(GroupType type) {
			switch(type) {
				case GroupType.And: return Localizer.Active.GetLocalizedString(StringId.FilterGroupAnd);
				case GroupType.NotAnd: return Localizer.Active.GetLocalizedString(StringId.FilterGroupNotAnd);
				case GroupType.NotOr: return Localizer.Active.GetLocalizedString(StringId.FilterGroupNotOr);
				case GroupType.Or: return Localizer.Active.GetLocalizedString(StringId.FilterGroupOr);
			}
			return type.ToString();
		}
		public static string GetMenuStringByType(ClauseType type) {
			switch(type) {
				case ClauseType.AnyOf: return Localizer.Active.GetLocalizedString(StringId.FilterClauseAnyOf);
				case ClauseType.BeginsWith: return Localizer.Active.GetLocalizedString(StringId.FilterClauseBeginsWith);
				case ClauseType.Between: return Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween);
				case ClauseType.Contains: return Localizer.Active.GetLocalizedString(StringId.FilterClauseContains);
				case ClauseType.EndsWith: return Localizer.Active.GetLocalizedString(StringId.FilterClauseEndsWith);
				case ClauseType.Equals: return Localizer.Active.GetLocalizedString(StringId.FilterClauseEquals);
				case ClauseType.Greater: return Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater);
				case ClauseType.GreaterOrEqual: return Localizer.Active.GetLocalizedString(StringId.FilterClauseGreaterOrEqual);
				case ClauseType.IsNotNull: return Localizer.Active.GetLocalizedString(StringId.FilterClauseIsNotNull);
				case ClauseType.IsNull: return Localizer.Active.GetLocalizedString(StringId.FilterClauseIsNull);
				case ClauseType.IsNotNullOrEmpty: return Localizer.Active.GetLocalizedString(StringId.FilterClauseIsNotNullOrEmpty);
				case ClauseType.IsNullOrEmpty: return Localizer.Active.GetLocalizedString(StringId.FilterClauseIsNullOrEmpty);
				case ClauseType.Less: return Localizer.Active.GetLocalizedString(StringId.FilterClauseLess);
				case ClauseType.LessOrEqual: return Localizer.Active.GetLocalizedString(StringId.FilterClauseLessOrEqual);
				case ClauseType.Like: return Localizer.Active.GetLocalizedString(StringId.FilterClauseLike);
				case ClauseType.NoneOf: return Localizer.Active.GetLocalizedString(StringId.FilterClauseNoneOf);
				case ClauseType.NotBetween: return Localizer.Active.GetLocalizedString(StringId.FilterClauseNotBetween);
				case ClauseType.DoesNotContain: return Localizer.Active.GetLocalizedString(StringId.FilterClauseDoesNotContain);
				case ClauseType.DoesNotEqual: return Localizer.Active.GetLocalizedString(StringId.FilterClauseDoesNotEqual);
				case ClauseType.NotLike: return Localizer.Active.GetLocalizedString(StringId.FilterClauseNotLike);
				case ClauseType.IsBeyondThisYear: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalBeyondThisYear);
				case ClauseType.IsLaterThisYear: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisYear);
				case ClauseType.IsLaterThisMonth: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisMonth);
				case ClauseType.IsNextWeek: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalNextWeek);
				case ClauseType.IsLaterThisWeek: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisWeek);
				case ClauseType.IsTomorrow: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalTomorrow);
				case ClauseType.IsToday: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalToday);
				case ClauseType.IsYesterday: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalYesterday);
				case ClauseType.IsEarlierThisWeek: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisWeek);
				case ClauseType.IsLastWeek: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLastWeek);
				case ClauseType.IsEarlierThisMonth: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisMonth);
				case ClauseType.IsEarlierThisYear: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisYear);
				case ClauseType.IsPriorThisYear: return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalPriorThisYear);
			}
			return type.ToString();
		}
		public static string GetMenuStringByType(Aggregate type) {
			switch(type) {
				case Aggregate.Avg: return Localizer.Active.GetLocalizedString(StringId.FilterAggregateAvg);
				case Aggregate.Count: return Localizer.Active.GetLocalizedString(StringId.FilterAggregateCount);
				case Aggregate.Exists: return Localizer.Active.GetLocalizedString(StringId.FilterAggregateExists);
				case Aggregate.Max: return Localizer.Active.GetLocalizedString(StringId.FilterAggregateMax);
				case Aggregate.Min: return Localizer.Active.GetLocalizedString(StringId.FilterAggregateMin);
				case Aggregate.Sum: return Localizer.Active.GetLocalizedString(StringId.FilterAggregateSum);
			}
			return type.ToString();
		}
	}
	public abstract class FilterColumn : IDisposable, IBoundProperty, ITreeSelectableItem {
		IWinFilterTreeNodeModel model;
		internal static int CompareFilterColumn(IBoundProperty col1, IBoundProperty col2) {
			return Comparer.Default.Compare(col1.DisplayName, col2.DisplayName);
		}
		public virtual void SetParent(IBoundProperty parent) { }
		public virtual string DataMember { set { } }
		public abstract string ColumnCaption { get; }
		public abstract string FieldName { get; }
		public abstract Type ColumnType { get; }
		public virtual FilterColumnClauseClass ClauseClass { 
			get {
				return IBoundPropertyDefaults.GetDefaultClauseClass(this); 
			} 
		}
		public abstract RepositoryItem ColumnEditor { get; }
		public string FullName { get { return this.GetFullName(); } }
		protected virtual string ColumnName { get { return FieldName; } }
		public int Level { get { return this.GetLevel(); } }
		public abstract Image Image { get; }
		public virtual void SetColumnEditor(RepositoryItem item) {
		}
		public virtual void SetColumnCaption(string caption) {
		}
		public virtual void SetImage(Image image) {
		}
		public virtual void Dispose() { }
		public virtual bool HasChildren { get { return false; } }
		public virtual bool IsAggregate { get { return false; } }
		public virtual bool IsList { get { return false; } }
		public virtual IBoundProperty Parent { get { return null; } set { } }
		public virtual List<IBoundProperty> Children { get { return null; } }
		public virtual bool AllowItemCollectionEditor { get { return false; } }
		public virtual RepositoryItem CreateItemCollectionEditor() { return new RepositoryItemTextEdit(); }
		public override string ToString() {  return this.GetFullDisplayName(); }
		public FilterColumn this[string fieldName] {
			get {
				if(Children == null) return null;
				foreach(FilterColumn child in Children) {
					if(fieldName == child.FieldName) return child;
				}
				return null;
			}
		}
		public FilterColumn GetChildByCaption(string caption) {
			if(Children == null) return null;
			foreach(FilterColumn child in Children) {
				if(caption == child.ColumnCaption) return child;
			}
			return null;
		}
		internal IWinFilterTreeNodeModel Model {
			get {
				if(model != null) return model;
				FilterColumn parentColumn = Parent as FilterColumn;
				return parentColumn != null ? parentColumn.Model : null;
			}
			set { model = value; }
		}
		protected void RebuildModel() {
			if(Model != null) {
				Model.Rebuild();
			}
		}
		#region IBoundProperty Members
		List<IBoundProperty> IBoundProperty.Children { 
			get {
				return this.Children;
			} 
		}
		string IBoundProperty.DisplayName { get { return ColumnCaption; } }
		bool IBoundProperty.HasChildren { get { return this.HasChildren; } }
		bool IBoundProperty.IsAggregate { get { return this.IsAggregate; } }
		IBoundProperty IBoundProperty.Parent { get { return this.Parent; } }
		string IBoundProperty.Name { get { return FieldName; } }
		Type IBoundProperty.Type { get { return ColumnType; } }
		#endregion
		#region ITreeComboBoxItem Members
		ITreeSelectableItem ITreeSelectableItem.Parent { get { return Parent != null && !Parent.IsList ? (FilterColumn)Parent : null; } }
		List<ITreeSelectableItem> ITreeSelectableItem.Children {
			get {
				if(Children == null || Children.Count == 0 || IsList) return null;
				List<ITreeSelectableItem> list = new List<ITreeSelectableItem>();
				foreach(FilterColumn column in Children) {
					if(column.Parent == this) {
						list.Add(column);
					}
				}
				if (Model != null && Model.SortProperties) {
					list.Sort(new FilterColumnSorters.ITreeSelectableItemSorter());
				}
				return list;
			}
		}
		bool ITreeSelectableItem.AllowSelect { 
			get {
				return !HasChildren || !IsAggregate; 
			}
		}
		string ITreeSelectableItem.Text { get { return ColumnCaption; } }
		#endregion
	}
	public delegate bool FilterColumnResolver(FilterColumn column, string text);
	public class FilterColumnCollection : CollectionBase, IDisposable, IBoundPropertyCollection, IDisplayCriteriaGeneratorNamesSourcePathed {
		public FilterColumn this[int index] {
			get {
				return (FilterColumn)List[index];
			}
			set {
				List[index] = value;
			}
		}
		public FilterColumn this[OperandProperty operandProperty] { get { return (FilterColumn)this.GetProperty(operandProperty); } }
		public int Add(FilterColumn value) {
			if(Model != null) {
				value.Model = Model;
			}
			return List.Add(value);
		}
		public int IndexOf(FilterColumn value) {
			return List.IndexOf(value);
		}
		public void Insert(int index, FilterColumn value) {
			List.Insert(index, value);
		}
		public void Remove(FilterColumn value) {
			List.Remove(value);
		}
		public bool Contains(FilterColumn value) {
			return List.Contains(value);
		}
		public FilterColumn this[string fieldName] {
			get {
				return GetFilterColumnByFieldName(fieldName, this);
			}
		}
		public FilterColumn GetFilterColumnByCaption(string caption) {
			return GetFilterColumnByCaption(caption, this);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FilterColumn GetFilterColumnByCaption(string caption, IList list) {
			return GetFilterColumnByCaptionOrField(caption, list, ResolveByCaption);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FilterColumn GetFilterColumnByFieldName(string fieldName, IList list) {
			FilterColumn result = GetFilterColumnByCaptionOrField(fieldName, list, ResolveByFieldName);
			if (result == null && !string.IsNullOrEmpty(fieldName) && fieldName.EndsWith("!")) {
				return GetFilterColumnByFieldName(fieldName.Remove(fieldName.Length - 1), list);
			}
			return result;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FilterColumn GetFilterColumnByCaptionOrField(string captionOrField, IList list, FilterColumnResolver resolver) {
			FilterColumn res = ResolvePropertyInList(captionOrField, list, resolver);
			return res != null ? res : ResolveComplexProperty(captionOrField, list, resolver);
		}
		[Obsolete("Use FilterControl.CreateDefaultClauseNode instead", true)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ClauseNode CreateDefaultClauseNode(FilterColumn column) {
			throw new NotSupportedException();
		}
		[Obsolete("Use GetDisplayPropertyName(OperandProperty operandProperty, string fullPath). Use fullPath argument for Condition of aggregates")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string GetDisplayPropertyName(OperandProperty operandProperty) {
			IBoundProperty col = this.GetProperty(operandProperty);
			if(col != null)
				return col.ToString();
			else
				return operandProperty.PropertyName;
		}
		public virtual string GetDisplayPropertyName(OperandProperty operandProperty, string fullPath) {			
			IBoundProperty col = this[fullPath];
			if (col == null)
				col = this[operandProperty];
			if(col != null)
				return col.ToString();
			else
				return operandProperty.PropertyName;
		}
		public virtual string GetValueScreenText(OperandProperty operandProperty, object value) {
			if (value is FunctionOperator) {
				var functionOperator = (FunctionOperator)value;
				return FilterControlViewInfo.GetLocalizedFunctionName(functionOperator.OperatorType, value as FunctionOperator);
			}
			if(value == null)
				return Localizer.Active.GetLocalizedString(StringId.FilterEmptyValue);
			FilterColumn col = (FilterColumn)this.GetProperty(operandProperty);
			if (col != null && col.ColumnEditor != null) {
				return col.ColumnEditor.GetDisplayText(value);
			} else {
				return value != null ? value.ToString() : string.Empty;
			}
		}
		public virtual void Dispose() {
			foreach(FilterColumn column in this) {
				column.Dispose();
			}
		}
		public void Sort() {
			InnerList.Sort(new FilterColumnSorters.FilterColumnCollectionSorter());
		}
		protected FilterColumn ResolveComplexProperty(string name, IList list, FilterColumnResolver resolver) {
			if(string.IsNullOrEmpty(name)) return null;
			FilterColumn parent = GetParentProperty(ref name, list, resolver);
			while(parent != null && !string.IsNullOrEmpty(name)) {
				FilterColumn child = ResolvePropertyInList(name, parent.Children, resolver);
				if(child != null) return child;
				parent = GetParentProperty(ref name, parent.Children, resolver);
			}
			return null;
		}
		FilterColumn ResolvePropertyInList(string name, IList list, FilterColumnResolver resolver) {
			if(list == null || string.IsNullOrEmpty(name)) return null;
			foreach(FilterColumn column in list) {
				if(resolver(column, name)) return column;
			}
			return null;
		}
		FilterColumn GetParentProperty(ref string name, IList list, FilterColumnResolver resolver) {
			string prefix = GetParentFieldName(ref name);
			return ResolvePropertyInList(prefix, list, resolver);
		}
		protected virtual string GetParentFieldName(ref string name) {
			int pos = name.IndexOf('.');
			if(pos < 0) return null;
			string prefix = name.Substring(0, pos);
			name = name.Substring(pos + 1);
			return prefix;
		}
		protected virtual bool ResolveByCaption(FilterColumn column, string caption) {
			if (column == null || string.IsNullOrEmpty(caption)) return false;
			return caption.Equals(column.ColumnCaption,StringComparison.OrdinalIgnoreCase);
		}
		protected virtual bool ResolveByFieldName(FilterColumn column, string fieldName) {
			if (column == null || string.IsNullOrEmpty(fieldName)) return false;
			return column.FieldName == fieldName;
		}
		public FilterColumnCollection CreateChildrenColumns(FilterColumn listColumn) {
			FilterColumnCollection collection = CreateChildrenColumns();
			foreach(FilterColumn column in listColumn.Children) {
				collection.Add(column);
			}
			return collection;
		}
		protected virtual FilterColumnCollection CreateChildrenColumns() { return new FilterColumnCollection(); }
		#region IBoundPropertyCollection Members
		IBoundProperty IBoundPropertyCollection.this[int index] {
			get { return this[index]; }
		}
		IBoundProperty IBoundPropertyCollection.this[string fieldName] {
			get { return this[fieldName]; }
		}
		IBoundPropertyCollection IBoundPropertyCollection.CreateChildrenProperties(IBoundProperty listProperty) {
			return CreateChildrenColumns((FilterColumn)listProperty);
		}
		public void Add(IBoundProperty property) {
			Add((FilterColumn)property);
		}
		#endregion
		internal IWinFilterTreeNodeModel Model { get; set; }
	}   
	public class ColumnsContainedCollector : IClientCriteriaVisitor {
		protected readonly IDictionary Columns = new HybridDictionary();
		void IClientCriteriaVisitor.Visit(OperandProperty theOperand) {
			Columns[theOperand] = theOperand;
		}
		void IClientCriteriaVisitor.Visit(AggregateOperand theOperand) {
			Process(theOperand.CollectionProperty);
		}
		void IClientCriteriaVisitor.Visit(JoinOperand theOperand) {
		}
		void ICriteriaVisitor.Visit(FunctionOperator theOperator) {
			foreach(CriteriaOperator op in theOperator.Operands)
				Process(op);
		}
		void ICriteriaVisitor.Visit(OperandValue theOperand) {
		}
		void ICriteriaVisitor.Visit(GroupOperator theOperator) {
			foreach(CriteriaOperator op in theOperator.Operands)
				Process(op);
		}
		void ICriteriaVisitor.Visit(InOperator theOperator) {
			Process(theOperator.LeftOperand);
			foreach(CriteriaOperator op in theOperator.Operands)
				Process(op);
		}
		void ICriteriaVisitor.Visit(UnaryOperator theOperator) {
			Process(theOperator.Operand);
		}
		void ICriteriaVisitor.Visit(BinaryOperator theOperator) {
			Process(theOperator.LeftOperand);
			Process(theOperator.RightOperand);
		}
		void ICriteriaVisitor.Visit(BetweenOperator theOperator) {
			Process(theOperator.TestExpression);
			Process(theOperator.BeginExpression);
			Process(theOperator.EndExpression);
		}
		void Process(CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return;
			op.Accept(this);
		}
		public static ICollection CollectColumns(CriteriaOperator criteria) {
			ColumnsContainedCollector collector = new ColumnsContainedCollector();
			collector.Process(criteria);
			return collector.Columns.Keys;
		}
		public static bool IsContained(CriteriaOperator criteria, OperandProperty op) {
			ColumnsContainedCollector collector = new ColumnsContainedCollector();
			collector.Process(criteria);
			return collector.Columns.Contains(op);
		}
	}
	public static class LocalaizableCriteriaToStringProcessor {
		public static string Process(XtraLocalizer<StringId> localizer, CriteriaOperator op) {
			return LocalaizableCriteriaToStringProcessorCore.Process(new LocalaizableCriteriaToStringProcessorLocalizerWrapper(localizer), op);
		}
		public class LocalaizableCriteriaToStringProcessorLocalizerWrapper : ILocalaizableCriteriaToStringProcessorOpNamesSource {
			public readonly XtraLocalizer<StringId> Localizer;
			public LocalaizableCriteriaToStringProcessorLocalizerWrapper(XtraLocalizer<StringId> localizer) {
				this.Localizer = localizer;
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetBetweenString() {
				return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBetween);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetInString() {
				return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringIn);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetIsNotNullString() {
				return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringIsNotNull);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetIsNullString() {
				return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringUnaryOperatorIsNull);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetNotLikeString() {
				return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringNotLike);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(Aggregate opType) {
				return null;
			}
			Dictionary<FunctionOperatorType, StringId?> localizerMap;
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(FunctionOperatorType opType) {
				StringId? stringId;
				lock(this) {
					if(localizerMap == null)
						localizerMap = new Dictionary<FunctionOperatorType, StringId?>();
					if(!localizerMap.TryGetValue(opType, out stringId)) {
						string strIdName = "FilterCriteriaToStringFunction" + opType.ToString();
						try {
							stringId = (StringId?)Enum.Parse(typeof(StringId), strIdName);
						} catch {
							stringId = null;
						}
						localizerMap.Add(opType, stringId);
					}
				}
				if(!stringId.HasValue)
					return opType.ToString();
				return Localizer.GetLocalizedString(stringId.Value);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(BinaryOperatorType opType) {
				switch(opType) {
					case BinaryOperatorType.BitwiseAnd:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorBitwiseAnd);
					case BinaryOperatorType.BitwiseOr:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorBitwiseOr);
					case BinaryOperatorType.BitwiseXor:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorBitwiseXor);
					case BinaryOperatorType.Divide:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorDivide);
					case BinaryOperatorType.Equal:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorEqual);
					case BinaryOperatorType.Greater:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorGreater);
					case BinaryOperatorType.GreaterOrEqual:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorGreaterOrEqual);
					case BinaryOperatorType.Less:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorLess);
					case BinaryOperatorType.LessOrEqual:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorLessOrEqual);
#pragma warning disable 618
					case BinaryOperatorType.Like:
#pragma warning restore 618
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorLike);
					case BinaryOperatorType.Minus:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorMinus);
					case BinaryOperatorType.Modulo:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorModulo);
					case BinaryOperatorType.Multiply:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorMultiply);
					case BinaryOperatorType.NotEqual:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorNotEqual);
					case BinaryOperatorType.Plus:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringBinaryOperatorPlus);
					default:
						return opType.ToString();
				}
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(UnaryOperatorType opType) {
				switch(opType) {
					case UnaryOperatorType.BitwiseNot:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringUnaryOperatorBitwiseNot);
					case UnaryOperatorType.IsNull:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringUnaryOperatorIsNull);
					case UnaryOperatorType.Minus:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringUnaryOperatorMinus);
					case UnaryOperatorType.Not:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringUnaryOperatorNot);
					case UnaryOperatorType.Plus:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringUnaryOperatorPlus);
					default:
						return opType.ToString();
				}
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(GroupOperatorType opType) {
				switch(opType) {
					case GroupOperatorType.And:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringGroupOperatorAnd);
					case GroupOperatorType.Or:
						return Localizer.GetLocalizedString(StringId.FilterCriteriaToStringGroupOperatorOr);
					default:
						return opType.ToString();
				}
			}
		}
	}
}
