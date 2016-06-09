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
using System.Text;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList.Columns;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using System.Collections;
using DevExpress.Utils.Editors;
using System.Drawing.Design;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.Data;
using System.Collections.Generic;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraTreeList.FilterEditor;
using DevExpress.XtraEditors.Helpers;
namespace DevExpress.XtraTreeList {
	public enum FilterConditionEnum {
		None,
		Equals,
		NotEquals,
		Between,
		NotBetween,
		Less,
		Greater,
		GreaterOrEqual,
		LessOrEqual,
		BeginsWith,
		EndsWith,
		Contains,
		NotContains,
		Like,
		NotLike,
		IsBlank,
		IsNotBlank,
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class FilterConditionBase {
		object value1;
		object value2;
		object column;
		FilterConditionEnum condition;
		public FilterConditionBase() {
			this.value1 = null;
			this.value2 = null;
			this.condition = FilterConditionEnum.None;
			this.column = null;
		}
		public FilterConditionBase(FilterConditionEnum condition, object column, object value1, object value2) {
			this.value1 = value1;
			this.value2 = value2;
			this.condition = condition;
			this.column = column;
		}
		[DefaultValue(FilterConditionEnum.None), XtraSerializableProperty]
		public FilterConditionEnum Condition {
			get { return condition; }
			set {
				if(Condition == value) return;
				condition = value;
				ItemChanged();
			}
		}
		[DefaultValue((string)null), TypeConverter(typeof(ObjectEditorTypeConverter)), XtraSerializableProperty, Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(UITypeEditor))]
		public object Value1 {
			get { return value1; }
			set {
				if(Value1 == value) return;
				value1 = value;
				ItemChanged();
			}
		}
		[TypeConverter(typeof(ObjectEditorTypeConverter)), XtraSerializableProperty, DefaultValue((string)null), Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(UITypeEditor))]
		public object Value2 {
			get { return value2; }
			set {
				if(Value2 == value) return;
				value2 = value;
				ItemChanged();
			}
		}
		protected object Column {
			get { return column; }
			set {
				if(Column == value) return;
				column = value;
				ItemChanged();
			}
		}
		protected virtual void AssignColumn(object column) {
			this.column = column;
		}
		public virtual void Assign(FilterConditionBase source) {
			condition = source.Condition;
			value1 = source.Value1;
			value2 = source.Value2;
			AssignColumn(source.Column);
			ItemChanged();
		}
		[Browsable(false)]
		public FilterConditionCollectionBase Collection { get { return fCollection; } }
		protected internal FilterConditionCollectionBase fCollection;
		protected virtual void ItemChanged() {
			if(Collection != null) {
				Collection.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
			}
		}
		public virtual bool CheckValue(object val) { return false; }
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class FilterCondition : FilterConditionBase {
		bool visible = false;
		ExpressionEvaluator expressionEvaluatorCore = null;
		FilterEvaluatorContextDescriptor contextDescriptorCore = null;
		CriteriaOperator criteriaCore = null;
		protected static FilterConditionEnum ConvertConditionType(FormatConditionEnum type) {
			switch(type) {
				case FormatConditionEnum.None:
					return FilterConditionEnum.None;
				case FormatConditionEnum.Equal:
					return FilterConditionEnum.Equals;
				case FormatConditionEnum.NotEqual:
					return FilterConditionEnum.NotEquals;
				case FormatConditionEnum.Between:
					return FilterConditionEnum.Between;
				case FormatConditionEnum.NotBetween:
					return FilterConditionEnum.NotBetween;
				case FormatConditionEnum.Less:
					return FilterConditionEnum.Less;
				case FormatConditionEnum.Greater:
					return FilterConditionEnum.Greater;
				case FormatConditionEnum.GreaterOrEqual:
					return FilterConditionEnum.GreaterOrEqual;
				case FormatConditionEnum.LessOrEqual:
					return FilterConditionEnum.LessOrEqual;
				default:
					throw new NotImplementedException();
			}
		}
		public FilterCondition() : base() { }
		[Obsolete("The FormatConditionEnum is obsolete. Use the FilterConditionEnum instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public FilterCondition(FormatConditionEnum condition) : this(ConvertConditionType(condition), null, null, null) { }
		[Obsolete("The FormatConditionEnum is obsolete. Use the FilterConditionEnum instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public FilterCondition(FormatConditionEnum condition, TreeListColumn column, object val1) : this(ConvertConditionType(condition), column, val1, null) { }
		[Obsolete("The FormatConditionEnum is obsolete. Use the FilterConditionEnum instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public FilterCondition(FormatConditionEnum condition, TreeListColumn column, object val1, object val2) : this(ConvertConditionType(condition), column, val1, val2, false) { }
		[Obsolete("The FormatConditionEnum is obsolete. Use the FilterConditionEnum instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public FilterCondition(FormatConditionEnum condition, TreeListColumn column, object val1, object val2, bool visible) : this(ConvertConditionType(condition), column, val1, val2) { }
		public FilterCondition(FilterConditionEnum condition) : this(condition, null, null, null) { }
		public FilterCondition(FilterConditionEnum condition, TreeListColumn column, object val1) : this(condition, column, val1, null) { }
		public FilterCondition(FilterConditionEnum condition, TreeListColumn column, object val1, object val2) : this(condition, column, val1, val2, false) { }
		public FilterCondition(FilterConditionEnum condition, TreeListColumn column, object val1, object val2, bool visible)
			: base(condition, column, val1, val2) {
			this.visible = visible;
		}
		[TypeConverter("DevExpress.XtraTreeList.Design.ColumnReferenceConverter, " + AssemblyInfo.SRAssemblyTreeListDesign), DefaultValue((string)null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new TreeListColumn Column {
			get { return (base.Column as TreeListColumn); }
			set { base.Column = value; }
		}
		[DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				ItemChanged();
			}
		}
		public override bool CheckValue(object val) {
			if(Column == null) return false;
			ContextDescriptor = new FilterEvaluatorContextDescriptor(val);
			string s = Value1 as string;
			if(s == null && IsStringOperandRequired(Condition)) return false;
			try {
				switch(Condition) {
					case FilterConditionEnum.Equals:
						return EvaluateBinaryOperator(Value1, BinaryOperatorType.Equal);
					case FilterConditionEnum.NotEquals:
						return EvaluateBinaryOperator(Value1, BinaryOperatorType.NotEqual);
					case FilterConditionEnum.Between:
						return EvaluateBetweenOperator(Value1, Value2);
					case FilterConditionEnum.NotBetween:
						return !EvaluateBetweenOperator(Value1, Value2);
					case FilterConditionEnum.Less:
						return EvaluateBinaryOperator(Value1, BinaryOperatorType.Less);
					case FilterConditionEnum.Greater:
						return EvaluateBinaryOperator(Value1, BinaryOperatorType.Greater);
					case FilterConditionEnum.GreaterOrEqual:
						return EvaluateBinaryOperator(Value1, BinaryOperatorType.GreaterOrEqual);
					case FilterConditionEnum.LessOrEqual:
						return EvaluateBinaryOperator(Value1, BinaryOperatorType.LessOrEqual);
#pragma warning disable 618
					case FilterConditionEnum.BeginsWith:
						return EvaluateBinaryOperator(s + "%", BinaryOperatorType.Like);
					case FilterConditionEnum.EndsWith:
						return EvaluateBinaryOperator("%" + s, BinaryOperatorType.Like);
					case FilterConditionEnum.Contains:
						return EvaluateBinaryOperator("%" + s + "%", BinaryOperatorType.Like);
					case FilterConditionEnum.NotContains:
						return !EvaluateBinaryOperator("%" + s + "%", BinaryOperatorType.Like);
					case FilterConditionEnum.Like:
						return EvaluateBinaryOperator(s, BinaryOperatorType.Like);
					case FilterConditionEnum.NotLike:
						return !EvaluateBinaryOperator(s, BinaryOperatorType.Like);
#pragma warning disable 618
					case FilterConditionEnum.IsBlank:
						return EvaluateUnaryOperator(UnaryOperatorType.IsNull);
					case FilterConditionEnum.IsNotBlank:
						return !EvaluateUnaryOperator(UnaryOperatorType.IsNull);
					default:
						throw new NotImplementedException();
				}
			}
			catch { return false; }
		}
		protected virtual bool IsStringOperandRequired(FilterConditionEnum condition) {
			if(condition == FilterConditionEnum.BeginsWith ||
				condition == FilterConditionEnum.EndsWith ||
				condition == FilterConditionEnum.Contains ||
				condition == FilterConditionEnum.NotContains ||
				condition == FilterConditionEnum.Like ||
				condition == FilterConditionEnum.NotLike)
				return true;
			return false;
		}
		protected virtual FilterEvaluatorContextDescriptor ContextDescriptor {
			get { return contextDescriptorCore; }
			set { contextDescriptorCore = value; }
		}
		protected virtual CriteriaOperator Criteria {
			get { return criteriaCore; }
			set { criteriaCore = value; }
		}
		protected virtual ExpressionEvaluator ExpressionEvaluator {
			get {
				if(expressionEvaluatorCore == null) return new ExpressionEvaluator(ContextDescriptor, Criteria, false);
				return expressionEvaluatorCore;
			}
		}
		protected virtual bool EvaluateBinaryOperator(object value, BinaryOperatorType type) {
			Criteria = new BinaryOperator("", value, type);
			return ExpressionEvaluator.Fit(new object());
		}
		protected virtual bool EvaluateBetweenOperator(object value1, object value2) {
			Criteria = new BetweenOperator("", value1, value2);
			return ExpressionEvaluator.Fit(new object());
		}
		protected virtual bool EvaluateUnaryOperator(UnaryOperatorType type) {
			Criteria = new UnaryOperator(type, "");
			return ExpressionEvaluator.Fit(new object());
		}
		protected class FilterEvaluatorContextDescriptor : EvaluatorContextDescriptor {
			object valueCore = null;
			public FilterEvaluatorContextDescriptor() { }
			public FilterEvaluatorContextDescriptor(object value) { this.valueCore = value; }
			public object Value { get { return valueCore; } set { valueCore = value; } }
			public override IEnumerable GetCollectionContexts(object source, string collectionName) { return null; }
			public override EvaluatorContext GetNestedContext(object source, string propertyPath) { return null; }
			public override IEnumerable GetQueryContexts(object source, string queryTypeName, CriteriaOperator condition, int top) { return null; }
			public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) { return valueCore; }
		}
	}
	[ListBindable(false)]
	public class FilterConditionCollectionBase : CollectionBase {
		int lockUpdate;
		public FilterConditionCollectionBase() {
			this.lockUpdate = 0;
		}
		public virtual void Add(FilterConditionBase condition) {
			if(!this.Contains(condition)) {
				List.Add(condition);
			}
		}
		public virtual void Assign(FilterConditionCollectionBase source) {
			BeginUpdate();
			try {
				base.Clear();
				foreach(object obj1 in source) {
					object obj2 = CreateItem();
					Add(obj2 as FilterConditionBase);
					AssignItem(obj2, obj1);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual object CreateItem() {
			return new FilterConditionBase();
		}
		protected virtual void AssignItem(object item, object source) {
			(item as FilterConditionBase).Assign(source as FilterConditionBase);
		}
		protected virtual void AddRange(FilterConditionBase[] conditions) {
			BeginUpdate();
			try {
				foreach(FilterConditionBase condition in conditions) {
					Add(condition);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual int IndexOf(FilterConditionBase condition) {
			return List.IndexOf(condition);
		}
		public virtual bool Contains(FilterConditionBase condition) {
			return List.Contains(condition);
		}
		public FilterConditionBase this[int index] { get { return (List[index] as FilterConditionBase); } }
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
			}
		}
		protected internal virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if((lockUpdate == 0) && (CollectionChanged != null)) {
				CollectionChanged(this, e);
			}
		}
		protected override void OnInsertComplete(int index, object item) {
			base.OnInsertComplete(index, item);
			(item as FilterConditionBase).fCollection = this;
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int index, object item) {
			base.OnRemoveComplete(index, item);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		public virtual void Remove(FilterConditionBase condition) {
			if(Contains(condition)) {
				List.Remove(condition);
			}
		}
		protected override void OnClear() {
			InnerList.Clear();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public event CollectionChangeEventHandler CollectionChanged;
	}
	[ListBindable(false)]
	public class FilterConditionCollection : FilterConditionCollectionBase {
		TreeList treeList;
		public FilterConditionCollection(TreeList treeList)
			: base() {
			this.treeList = treeList;
		}
		public void Add(FilterCondition condition) {
			base.Add(condition);
		}
		public void AddRange(FilterCondition[] conditions) {
			base.AddRange(conditions);
		}
		public new FilterCondition this[int index] { get { return (base.List[index] as FilterCondition); } }
		public TreeList TreeListControl { get { return ((treeList != null) ? treeList : null); } }
	}
	public class FilterItem {
		string displayText;
		object val;
		public FilterItem(string displayText, object val) {
			this.val = val;
			this.displayText = displayText;
		}
		public object Value { get { return val; } }
		public string DisplayText { get { return displayText; } }
		public override string ToString() { return DisplayText; }
	}
	public class FilterItemComparer : IComparer<FilterItem> {
		int IComparer<FilterItem>.Compare(FilterItem item1, FilterItem item2) {
			if(item1 == item2) return 0;
			if(item1 == null) return -1;
			if(item2 == null) return 1;
			return Comparer.Default.Compare(item1.DisplayText, item2.DisplayText);
		}
	}
	public class TreeListFilterInfo {
		CriteriaOperator filter;
		string displayText, filterString;
		public TreeListFilterInfo(string filterString) { 
			this.filterString = filterString;
			this.displayText = null;
			this.filter = CriteriaOperator.TryParse(FilterString);
		}
		public TreeListFilterInfo(CriteriaOperator filter) : this(filter, null) { }
		internal TreeListFilterInfo(CriteriaOperator filter, string displayText) {
			this.filter = filter;
			if(!ReferenceEquals(this.filter, null))
				this.filterString = CriteriaOperator.ToString(FilterCriteria);
			this.displayText = displayText;
		}
		[XtraSerializableProperty]
		public string FilterString { get { return filterString; } }
		public CriteriaOperator FilterCriteria { get { return filter; } }
		public override bool Equals(object obj) {
			TreeListFilterInfo filterInfo = obj as TreeListFilterInfo;
			if(filterInfo == null) return false;
			if(ReferenceEquals(obj, this)) return true;
			return object.Equals(FilterCriteria, filterInfo.FilterCriteria);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			if(DisplayText == null) return string.Empty;
			return DisplayText;
		}
		internal string DisplayText { get { return displayText; } }
		internal void SetDisplayText(string displayText) {
			this.displayText = displayText;
		}
	}
	public class TreeListFilterInfoCollection : CollectionBase {
		public TreeListFilterInfo this[int index] { get { return List[index] as TreeListFilterInfo; } }
		public int Add(TreeListFilterInfo info) {
			if(!CanAdd(info)) return -1;
			return List.Add(info);
		}
		public void Insert(int index, TreeListFilterInfo info) {
			if(!CanAdd(info)) return;
			List.Insert(index, info);
		}
		public void Remove(TreeListFilterInfo info) { List.Remove(info); }
		public bool Contains(TreeListFilterInfo info) {
			return List.Contains(info); 
		}
		protected virtual bool CanAdd(TreeListFilterInfo info) {
			return true;
		}
		internal void AddMRUFilter(TreeListFilterInfo info, int maxCount) {
			if(info == null) return;
			if(List.Contains(info)) 
				List.Remove(info);
			List.Insert(0, info);
			CheckCount(maxCount);
		}
		protected internal void CheckCount(int maxCount) {
			maxCount = Math.Max(0, maxCount);
			while(Count > maxCount) RemoveAt(Count - 1);
		}
		internal void RemoveMRUFilter(TreeListFilterInfo info) {
			if(List.Contains(info))
				Remove(info);
		}
	}
	public class TreeListProvider : DevExpress.Utils.FilteredComponentColumnsProviderBase {
		public TreeListProvider(DevExpress.Utils.IFilteredComponentColumns component) : base(component, (column) => ((FilterColumn)column).FieldName) { }
	}
}
namespace DevExpress.XtraTreeList.Columns {
	public class TreeListColumnFilterInfo {
		CriteriaOperator filter;
		object value;
		public TreeListColumnFilterInfo() {
			Clear();
		}
		public CriteriaOperator FilterCriteria { get { return filter; } }
		public object AutoFilterRowValue { get { return value; } }
		public bool IsEmpty { get { return object.ReferenceEquals(FilterCriteria, null); } }
		protected internal void Clear() {
			Set(null, null);
		}
		protected internal void Set(CriteriaOperator filter, object value) {
			this.filter = filter;
			this.value = value;
		}
	}
}
namespace DevExpress.XtraTreeList.Internal {
	public class TreeListFilterHelper {
		TreeList treeList;
		ExpressionEvaluator expressionEvaluator;
		public TreeListFilterHelper(TreeList treeList) {
			this.treeList = treeList;
			Reset();
		}
		public CriteriaOperator FilterCriteria { get { return treeList.ActiveFilterCriteria & treeList.FindFilterCriteria & treeList.ExtraFilterCriteria; } }
		public ExpressionEvaluator ExpressionEvaluator {
			get {
				if(expressionEvaluator == null) {
					if(!ReferenceEquals(FilterCriteria, null)) {
						Exception e;
						this.expressionEvaluator = treeList.Data.DataHelper.CreateExpressionEvaluator(FilterCriteria, out e);
					}
				}
				return expressionEvaluator;
			}
		}
		public bool IsReady { get { return ExpressionEvaluator != null; } }
		public bool Fit(TreeListNode node) {
			if(node == null) return false;
			if(ExpressionEvaluator != null)
				return ExpressionEvaluator.Fit(node);
			return false;
		}
		public void Reset() {
			this.expressionEvaluator = null;
		}
		internal CriteriaOperator CreateColumnFilterCriteriaByValue(DataColumnInfo column, object value, bool displayText, bool roundDateTime, IFormatProvider provider) {
			if(column == null) return null;
			Type columnType = displayText ? typeof(string) : column.Type;
			string columnName = column.ColumnName;
			OperandProperty property = new OperandProperty(columnName);
			if(value == null || value is DBNull)
				return property.IsNull();
			Type underlyingType = Nullable.GetUnderlyingType(columnType);
			if(underlyingType != null)
				columnType = underlyingType;
			if(roundDateTime && columnType == typeof(DateTime)) {
				DateTime min = ConvertToDate(value, provider);
				min = new DateTime(min.Year, min.Month, min.Day);
				try {
					DateTime next = min.AddDays(1);
					return (property >= min) & (property < next);
				}
				catch {
					return property >= min;
				}
			}
			value = CorrectFilterValueType(columnType, value, provider);
			return property == new OperandValue(value);
		}
		internal object[] GetFilterPopupValues(TreeListColumn column, bool showAll, bool displayText) {
			TreeListGetFilterPopupValuesOperation op = new TreeListGetFilterPopupValuesOperation(treeList, column, showAll, treeList.IsRoundDateTime(column), displayText, column.IsCheckedListFilterPopupMode || column.IsListFilterPopupMode);
			treeList.NodesIterator.DoOperation(op);
			return op.Values; 
		}
		internal object GetValueFromFilterCriteria(CriteriaOperator filterCriteria, AutoFilterCondition condition) {
			BinaryOperator bop = filterCriteria as BinaryOperator;
			if(!ReferenceEquals(null, bop)) {
				if(condition != AutoFilterCondition.Equals || bop.OperatorType != BinaryOperatorType.Equal)
					return null;
				OperandValue ov = bop.RightOperand as OperandValue;
				if(ReferenceEquals(null, ov))
					return null;
				return ov.Value;
			}
			FunctionOperator fop = filterCriteria as FunctionOperator;
			if(!ReferenceEquals(null, fop)) {
				if(fop.Operands.Count != 2)
					return null;
				OperandValue ov = fop.Operands[1] as OperandValue;
				if(ReferenceEquals(null, ov))
					return null;
				string val = ov.Value as string;
				if(string.IsNullOrEmpty(val))
					return null;
				if(condition == AutoFilterCondition.Contains && fop.OperatorType == FunctionOperatorType.Contains) 
					return val;
				else if(condition == AutoFilterCondition.Like && fop.OperatorType == FunctionOperatorType.Contains) 
					return '%' + val;
				else if(condition == AutoFilterCondition.Like && fop.OperatorType == FunctionOperatorType.StartsWith) 
					return val;
			}
			return null;
		}
		DateTime ConvertToDate(object val, IFormatProvider provider) {
			DateTime res = DateTime.MinValue;
			if(val == null) return res;
			try {
				if(!(val is DateTime)) {
					if(provider != null)
						res = DateTime.Parse(val.ToString(), provider);
					else
						res = DateTime.Parse(val.ToString());
				}
				else
					res = (DateTime)val;
			}
			catch { }
			return res;
		}
		object CorrectFilterValueType(Type columnFilteredType, object filteredValue, IFormatProvider provider) {
			if(filteredValue == null)
				return filteredValue;
			if(columnFilteredType == null)
				return filteredValue;
			Type underlyingFilteredType = Nullable.GetUnderlyingType(columnFilteredType);
			if(underlyingFilteredType != null)
				columnFilteredType = underlyingFilteredType;
			Type currentType = filteredValue.GetType();
			if(columnFilteredType.IsAssignableFrom(currentType))
				return filteredValue;
			try {
				return Convert.ChangeType(filteredValue, columnFilteredType, provider);
			}
			catch { }
			return filteredValue;
		}
		#region static
		public static void UpdateChildNodesVisibility(TreeListNode parentNode) {
			if(parentNode.HasChildren) {
				foreach(TreeListNode node in parentNode.Nodes) {
					node._visible = parentNode.Visible;
					if(!node._visible && parentNode.TreeList.Selection.Contains(node)) {
						parentNode.TreeList.Selection.InternalRemove(node);
					}
					if(node.HasChildren) UpdateChildNodesVisibility(node);
				}
			}
		}
		public static bool IsNodeVisible(TreeListNode node) {
			return node == null ? false : node.Visible;
		}
		public static int GetVisibleNodeCount(TreeListNodes nodes) {
			int count = 0;
			foreach(TreeListNode node in nodes) {
				if(IsNodeVisible(node)) count++;
			}
			return count;
		}
		public static int HasVisibleChildrenCheckDepth = 3;
		public static bool HasVisibleChildren(TreeListNode node, bool recursive = false) {
			return HasVisibleChildrenCore(node, recursive, 0, HasVisibleChildrenCheckDepth);
		}
		static bool HasVisibleChildrenCore(TreeListNode node, bool recursive, int depth, int maxDepth) {
			if(node == null) return false;
			bool result = false;
			foreach(TreeListNode currentNode in node.Nodes) {
				if(IsNodeVisible(currentNode)) {
					result = true;
					return result;
				};
				if(recursive && depth < maxDepth) {
					result = HasVisibleChildrenCore(currentNode, recursive, ++depth, maxDepth);
					if(result)
						return result;
				}
			}
			return result;
		}
		public static TreeListNode GetNewFocusedNode(TreeListNode node) {
			TreeListNode candidateNode = TreeListNodesIterator.GetNextVisible(node);
			if(candidateNode == null) candidateNode = TreeListNodesIterator.GetPrevVisible(node);
			return candidateNode;
		}
		public static bool IsFirstVisible(TreeListNode node) {
			if(node == null || node.TreeList == null) return true;
			if(node.TreeList.GetActualTreeListFilterMode() == FilterMode.Smart) {
				TreeListNode newParent = GetVisibleParent(node);
				TreeListNode currentVisibleNode = node;
				while((currentVisibleNode = TreeListNodesIterator.GetPrevVisible(currentVisibleNode)) != null) {
					if(newParent == GetVisibleParent(currentVisibleNode)) return false;
				}
			}
			return node.IsFirstVisible;
		}
		public static bool IsLastVisible(TreeListNode node) {
			if(node == null || node.TreeList == null) return true;
			if(node.TreeList.GetActualTreeListFilterMode() == FilterMode.Smart) {
				TreeListNode newParent = GetVisibleParent(node);
				TreeListNode currentVisibleNode = node;
				while((currentVisibleNode = TreeListNodesIterator.GetNextVisible(currentVisibleNode)) != null) {
					if(newParent == GetVisibleParent(currentVisibleNode)) return false;
				}
			}
			return node.IsLastVisible;
		}
		public static int CalcVisibleNodeLevel(TreeListNode node) {
			int level = 0;
			TreeListNode parentNode = node.ParentNode;
			while(parentNode != null) {
				if(IsNodeVisible(parentNode)) level++;
				parentNode = parentNode.ParentNode;
			}
			return level;
		}
		public static TreeListNode GetVisibleParent(TreeListNode node) {
			if(node == null) return null;
			TreeListNode parent = node.ParentNode;
			while(parent != null) {
				if(IsNodeVisible(parent))
					break;
				parent = parent.ParentNode;
			}
			return parent;
		}
		#endregion
	}
	public class TreeListDateFilterInfoCache {
		public DateFilterInfoCache Cache { get; set; }
		public DateFilterResult LastFilterResult { get; set; }
	}
}
