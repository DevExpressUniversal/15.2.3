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
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraTreeList.Columns;
using conditionEnum = DevExpress.XtraGrid;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Utils.Serializing;
using TreeListData = DevExpress.XtraTreeList.Data;
using System.Threading;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraTreeList.Data;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraTreeList.Nodes.Operations;
namespace DevExpress.XtraTreeList.StyleFormatConditions {
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class StyleFormatCondition : StyleFormatConditionBase {
		private bool applyToRow;
		public StyleFormatCondition() : this(conditionEnum.FormatConditionEnum.None, null, null, null, null) {
			base.SetLoaded(false); 
		} 
		public StyleFormatCondition(conditionEnum.FormatConditionEnum condition) : this(condition, null, null, null) {
			base.SetLoaded(true);
		}
		public StyleFormatCondition(conditionEnum.FormatConditionEnum condition, TreeListColumn column, object tag, object val1) : this(condition, column, tag, val1, null) {
		}
		public StyleFormatCondition(conditionEnum.FormatConditionEnum condition, TreeListColumn column, object tag, object val1, object val2) : this(condition, column, tag, val1, val2, false) {
		}
		public StyleFormatCondition(conditionEnum.FormatConditionEnum condition, TreeListColumn column, object tag, object val1, object val2, bool applyToRow) : this(condition, tag, null, val1, val2, column, applyToRow) {
			base.SetLoaded(true);
		}
		[DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), 
		TypeConverter("DevExpress.XtraTreeList.Design.ColumnReferenceConverter, " + AssemblyInfo.SRAssemblyTreeListDesign) ]
		public new TreeListColumn Column	{
			get {
				return (base.Column as TreeListColumn);
			}
			set {
				base.Column = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public string ColumnName {
			get {
				if(Column != null) return Column.Name;
				return string.Empty;
			}
			set {
				if(Collection == null) return;
				TreeListColumn column = TreeList.Columns.ColumnByName(value);
				Column = column;
			}
		}
		public StyleFormatCondition(conditionEnum.FormatConditionEnum condition, object tag, AppearanceObject appearance, object val1, object val2, TreeListColumn column, bool applyToRow) : base(condition, tag, appearance, val1, val2, column) {
			this.applyToRow = applyToRow;
		}
		public override void Assign(StyleFormatConditionBase source) {
			StyleFormatCondition condition1 = (StyleFormatCondition) source;
			this.applyToRow = condition1.ApplyToRow;
			base.Assign(source);
		}
		protected override void AssignColumn(object colObject) {
			base.AssignColumn(null);
			TreeListColumn column1 = (TreeListColumn) colObject;
			if ((column1 != null) && (column1.AbsoluteIndex != -1)) {
				base.AssignColumn(column1);
			}
		}
		[ DefaultValue(false),XtraSerializableProperty()]
		public virtual bool ApplyToRow {
			get {
				return this.applyToRow;
			}
			set {
				if (this.ApplyToRow != value) {
					this.applyToRow = value;
					this.ItemChanged();
				}
			}
		}
		protected override ExpressionEvaluator CreateEvaluator() {
			if(TreeList == null) return null;
			try {
				ExpressionEvaluator evaluator = new ExpressionEvaluator(CreateDescriptors(), Expression, false);
				evaluator.DataAccess = TreeList;
				return evaluator;
			}
			catch {
			}
			return null;
		}
		PropertyDescriptorCollection CreateDescriptors() {
			List<TreeListData.DataColumnInfo> columns = new List<TreeListData.DataColumnInfo>();
			for(int i = 0; i < TreeList.Data.Columns.Count; i++) {
				TreeListData.DataColumnInfo col = TreeList.Data.Columns[i];
				if(!string.IsNullOrEmpty(col.ColumnName))
					columns.Add(col);
			}
			PropertyDescriptor[] properties = new PropertyDescriptor[columns.Count];
			for(int n = 0; n < properties.Length; n++) properties[n] = new TreePropertyDescriptor(columns[n]);
			return new PropertyDescriptorCollection(properties);
		}
		protected override bool IsFitCore(ExpressionEvaluator evaluator, object val, object row) {
			try {
				object res = evaluator.Evaluate(row); 
				if(res is bool) return (bool)res;
				return Convert.ToBoolean(res);
			}
			catch {
			}
			return false;
		}
		protected TreeList TreeList {
			get {
				StyleFormatConditionCollection coll = Collection as StyleFormatConditionCollection;
				return coll == null ? null : coll.TreeListControl;
			}
		}
		protected override List<DevExpress.Data.IDataColumnInfo> GetColumns() {
			if(TreeList == null) return new List<DevExpress.Data.IDataColumnInfo>();
			return new List<DevExpress.Data.IDataColumnInfo>(new ArrayList(TreeList.Columns).ToArray(typeof(DevExpress.Data.IDataColumnInfo)) as DevExpress.Data.IDataColumnInfo[]);
		}
		protected internal virtual void OnColumnRemoved(TreeListColumn column) {
			if(column == Column) 
				Column = null;
		}
	}
	internal class TreePropertyDescriptor : PropertyDescriptor {
		TreeListData.DataColumnInfo column;
		public TreePropertyDescriptor(TreeListData.DataColumnInfo column)
			: base(column.ColumnName, null) {
			this.column = column;
		}
		protected TreePropertyDescriptor(TreeListData.DataColumnInfo column, string name) : base(name, null) {
			this.column = column;
		}
		public override string DisplayName {
			get {
				return column.Caption;
			}
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override Type ComponentType { get { return typeof(TreeList); } }
		public override object GetValue(object component) { return null; }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType { get { return column.Type; } }
		public override void ResetValue(object component) { }
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	internal class TreeDisplayPropertyDescriptor : TreePropertyDescriptor {
		public TreeDisplayPropertyDescriptor(TreeListData.DataColumnInfo column)
			: base(column) {
		}
	}
	internal class TreeFindFilterDisplayPropertyDescriptor : TreePropertyDescriptor {
		string originalName;
		public TreeFindFilterDisplayPropertyDescriptor(TreeListData.DataColumnInfo column) : base(column, AddPrefix(column.ColumnName)) {
			this.originalName = column.ColumnName;
		}
		public string OriginalName { get { return originalName; }}
		static string AddPrefix(string name) {
			return string.Concat(DevExpress.Data.Filtering.DxFtsContainsHelper.DxFtsPropertyPrefix, name);
		}
	}
	public class TreeListUnboundPropertyDescriptor : PropertyDescriptor {
		DataColumnInfo unboundInfo;
		ExpressionEvaluator evaluator;
		Type dataType;
		Exception evaluatorCreateException = null;
		TreeListDataHelper dataHelper;
		protected internal TreeListUnboundPropertyDescriptor(TreeListDataHelper dataHelper, DataColumnInfo unboundInfo)
			: base(unboundInfo.ColumnName, null) {
			this.evaluator = null;
			this.unboundInfo = unboundInfo;
			this.dataType = UnboundInfo.Type;
			this.dataHelper = dataHelper;
		}
		protected virtual ExpressionEvaluator CreateEvaluator() {
			return dataHelper.CreateExpressionEvaluator(CriteriaOperator.TryParse(UnboundInfo.UnboundExpression), out evaluatorCreateException);
		}
		protected ExpressionEvaluator Evaluator {
			get {
				if(evaluator == null)
					evaluator = CreateEvaluator();
				return evaluator;
			}
		}
		protected DataColumnInfo UnboundInfo { get { return unboundInfo; } }
		public override bool IsReadOnly { get { return UnboundInfo.ReadOnly; } }
		public override string Category { get { return string.Empty; } }
		public override Type PropertyType { get { return UnboundInfo.Type; } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override bool ShouldSerializeValue(object component) { return false; }
		public override object GetValue(object component) {
			object value = null;
			if(component is int) {
				if(Evaluator != null) {
					value = GetEvaluatorValue((int)component);
				}
				else {
					if(this.evaluatorCreateException != null) value = DevExpress.Data.UnboundErrorObject.Value;
				}
				return dataHelper.GetUnboundData((int)component, Name, value);
			}
			return null;
		}
		public override void SetValue(object component, object value) {
			if(component is int)
				dataHelper.SetUnboundData((int)component, Name, value);
		}
		protected virtual object GetEvaluatorValue(int nodeId) {
			object res = null;
			try {
				res = Convert(Evaluator.Evaluate(nodeId));
			}
			catch {
				return DevExpress.Data.UnboundErrorObject.Value;
			}
			return res;
		}
		protected bool RequireValueConversion { get { return !string.IsNullOrEmpty(UnboundInfo.UnboundExpression) && !dataType.Equals(typeof(object)); } }
		protected object Convert(object value) {
			if(!RequireValueConversion || value == null) return value;
			if(IsErrorValue(value)) return value;
			try {
				Type type = value.GetType();
				if(type.Equals(dataType)) return value;
				return System.Convert.ChangeType(value, dataType, Thread.CurrentThread.CurrentCulture);
			}
			catch {
			}
			return null;
		}
		public static bool IsErrorValue(object value) {
			return object.ReferenceEquals(value, DevExpress.Data.UnboundErrorObject.Value);
		}
		public static bool IsUnbound(PropertyDescriptor descriptor) {
			return descriptor is TreeListUnboundPropertyDescriptor;
		}
	}
	public class TreeListFormatRule : FormatRuleBase {
		TreeListFormatRuleValueProvider valueProvider;
		TreeListColumn column, columnApplyTo;
		bool applyToRow;
		public TreeListFormatRule() : base() {  }
		[DefaultValue(null)]
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraTreeList.Design.ColumnReferenceConverter, " + AssemblyInfo.SRAssemblyTreeListDesign)]
		public TreeListColumn Column {
			get { return column; }
			set {
				if(Column == value) return;
				column = value;
				OnColumnChanged();
			}
		}
		protected override DevExpress.Data.IDataColumnInfo GetColumnInfo() {
			if(TreeList == null) return null;
			return new DevExpress.XtraEditors.Helpers.RuleDataColumnInfoWrapper(null, GetColumns());
		}
		List<DevExpress.Data.IDataColumnInfo> GetColumns() {
			var res = new List<DevExpress.Data.IDataColumnInfo>();
			if(TreeList == null) return res;
			foreach(TreeListColumn col in TreeList.Columns) {
				if(!col.OptionsColumn.ShowInExpressionEditor) continue;
				res.Add(new TreeListUnboundColumnWrapper(col));
			}
			return res;
		}
		[DefaultValue(null)]
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraTreeList.Design.ColumnReferenceConverter, " + AssemblyInfo.SRAssemblyTreeListDesign)]
		public TreeListColumn ColumnApplyTo {
			get { return columnApplyTo; }
			set {
				if(ColumnApplyTo == value) return;
				columnApplyTo = value;
				OnColumnChanged();
			}
		}
		[ DefaultValue(false), XtraSerializableProperty]
		public bool ApplyToRow {
			get { return applyToRow; }
			set {
				if(ApplyToRow == value) return;
				applyToRow = value;
				OnApplyToRowChanged();
			}
		}
		bool XtraShouldSerializeColumnApplyToName() { return ColumnApplyToName != ""; }
		bool XtraShouldSerializeColumnName() { return ColumnName != ""; }
		[DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty, EditorBrowsable(EditorBrowsableState.Never)]
		public string ColumnName {
			get { return Column == null ? "" : Column.Name; }
			set {
				if(TreeList == null) return;
				Column = TreeList.Columns.ColumnByName(value);
			}
		}
		[DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty, EditorBrowsable(EditorBrowsableState.Never)]
		public string ColumnApplyToName {
			get { return ColumnApplyTo == null ? "" : ColumnApplyTo.Name; }
			set {
				if(TreeList == null) return;
				ColumnApplyTo = TreeList.Columns.ColumnByName(value);
			}
		}
		[Browsable(false)]
		public TreeList TreeList { get { return FormatRuleCollection != null ? FormatRuleCollection.TreeList : null; } }
		public override bool IsValid { get { return base.IsValid && Column != null; } }
		protected internal TreeListColumn ActualColumn { get { return ColumnApplyTo == null ? Column : ColumnApplyTo; } }
		protected internal string ActualFieldName { get { return Column == null ? string.Empty : Column.FieldName; } }
		protected override string FieldNameCore { get { return ActualFieldName; } }
		protected internal TreeListFormatRuleValueProvider ValueProvider {
			get {
				if(valueProvider == null) 
					valueProvider = CreateValueProvider();
				return valueProvider;
			}
		}
		protected virtual TreeListFormatRuleValueProvider CreateValueProvider() { 
			return new TreeListFormatRuleValueProvider(this);
		}
		protected virtual TreeListFormatRuleCollection FormatRuleCollection { get { return base.Collection as TreeListFormatRuleCollection; } }
		protected virtual void OnColumnChanged() {
			InvalidateRuleState();
			OnCollectionChanged(CollectionChangedAction.Changed, FormatConditionRuleChangeType.All);
		}
		protected virtual void OnApplyToRowChanged() {
			OnCollectionChanged(CollectionChangedAction.Changed, FormatConditionRuleChangeType.UI);
		}
		protected virtual void OnCollectionChanged(CollectionChangedAction action, FormatConditionRuleChangeType changeType) {
			if(Collection != null)
				Collection.OnCollectionChanged(new FormatConditionCollectionChangedEventArgs(action, this, changeType));
		}
		protected override FormatRuleBase CreateInstance() {
			return FormatRuleCollection != null ? FormatRuleCollection.CreateItemInstance() : null;
		}
		public override void Assign(FormatRuleBase source) {
			base.Assign(source);
			TreeListFormatRule rule = source as TreeListFormatRule;
			if(rule == null) return;
			this.ApplyToRow = rule.ApplyToRow;
			this.ColumnApplyTo = GetActualColumn(rule.ColumnApplyTo);
			this.Column = GetActualColumn(rule.Column);
		}
		TreeListColumn GetActualColumn(TreeListColumn column) {
			if(column == null || column.AbsoluteIndex < 0) return null;
			return column;
		}
		protected override ExpressionEvaluator CreateExpressionEvaluator(CriteriaOperator criteriaOperator, out bool readyToCreate) {
			readyToCreate = false;
			if(TreeList == null || TreeList.Data.Columns.Count == 0) return null;
			try {
				readyToCreate = true;
				ExpressionEvaluator evaluator = new ExpressionEvaluator(GetPropertyDescriptors(), criteriaOperator, false);
				evaluator.DataAccess = TreeList;
				return evaluator;
			}
			catch {
			}
			return null;
		}
		protected virtual PropertyDescriptorCollection GetPropertyDescriptors() {
			List<TreeListData.DataColumnInfo> columns = new List<TreeListData.DataColumnInfo>();
			for(int i = 0; i < TreeList.Data.Columns.Count; i++) {
				TreeListData.DataColumnInfo col = TreeList.Data.Columns[i];
				if(!string.IsNullOrEmpty(col.ColumnName))
					columns.Add(col);
			}
			PropertyDescriptor[] properties = new PropertyDescriptor[columns.Count];
			for(int n = 0; n < properties.Length; n++) properties[n] = new TreePropertyDescriptor(columns[n]);
			return new PropertyDescriptorCollection(properties);
		}
		protected internal virtual void OnColumnRemoved(TreeListColumn column) {
			bool changed = false;
			if(column == this.columnApplyTo) {
				this.columnApplyTo = null;
				changed = true;
			}
			if(column == this.column) {
				this.column = null;
				changed = true;
			}
			if(changed) OnColumnChanged();
		}
	}
	public class TreeListFormatRuleCollection : FormatRuleCollection<TreeListFormatRule, TreeListColumn> {
		FormatRuleSummaryInfoCollection summaryInfo;
		public TreeListFormatRuleCollection(TreeList treeList) {
			TreeList = treeList;
		}
		public TreeList TreeList { get; private set; }
		protected FormatRuleSummaryInfoCollection SummaryInfo {
			get {
				if(summaryInfo == null)
					summaryInfo = CreateSummaryInfo();
				return summaryInfo;
			}
		}
		protected override void AssignColumn(TreeListFormatRule format, TreeListColumn column) {
			format.Column = column;
		}
		protected override LookAndFeel.UserLookAndFeel ElementsLookAndFeel {
			get { return TreeList.ElementsLookAndFeel; }
		}
		protected override FormatConditionRuleState GetRuleState(FormatRuleBase format) {
			TreeListFormatRule rule = format as TreeListFormatRule;
			if(rule != null )
				return rule.ValueProvider.RuleState;
			return FormatConditionRuleState.NullState;
		}
		protected internal virtual bool UpdateStateValues() {
			if(CheckAllValuesReady()) return false;
			List<DevExpress.Data.SummaryItem> summaryItems = GetSummaryItems();
			foreach(var formatRule in this) 
				summaryInfo.UpdateValues(summaryItems, formatRule.ActualFieldName, GetRuleState(formatRule));
			return CheckAllValuesReady();
		}
		protected virtual List<DevExpress.Data.SummaryItem> GetSummaryItems() {
			List<DevExpress.Data.SummaryItem> summaryItems = new List<DevExpress.Data.SummaryItem>();
			foreach(FormatRuleSummaryInfo info in SummaryInfo) {
				DevExpress.Data.SummaryItem item = new DevExpress.Data.SummaryItem() { SummaryTypeEx = info.SummaryType, FieldName = info.Column, SummaryArgument = info.SummaryArgument, Tag = info };
				item.SummaryValue = GetSummaryValue(item, TreeList.Nodes, TreeList.Columns[info.Column], info.SummaryType);
				summaryItems.Add(item);
			}
			return summaryItems;
		}
		protected virtual object GetSummaryValue(DevExpress.Data.SummaryItem item, TreeListNodes nodes, TreeListColumn column, DevExpress.Data.SummaryItemTypeEx summaryType) {
			if(summaryType == DevExpress.Data.SummaryItemTypeEx.Duplicate || summaryType == DevExpress.Data.SummaryItemTypeEx.Unique) {
				TreeListOperationCalcUniqueAndDuplicateValues op = new TreeListOperationCalcUniqueAndDuplicateValues(column, summaryType == DevExpress.Data.SummaryItemTypeEx.Duplicate, true);
				TreeList.NodesIterator.DoOperation(op);
				return op.Result;
			}
			bool isBottom = summaryType == DevExpress.Data.SummaryItemTypeEx.Bottom || summaryType == DevExpress.Data.SummaryItemTypeEx.BottomPercent;
			if(summaryType == DevExpress.Data.SummaryItemTypeEx.Top || summaryType == DevExpress.Data.SummaryItemTypeEx.TopPercent || isBottom) { 
				bool isPercent = summaryType == DevExpress.Data.SummaryItemTypeEx.TopPercent || summaryType == DevExpress.Data.SummaryItemTypeEx.BottomPercent;
				TreeListOperationCalcTopAndBottomValues op = new TreeListOperationCalcTopAndBottomValues(column, isBottom, isPercent, (int)item.SummaryArgument, true);
				TreeList.NodesIterator.DoOperation(op);
				return op.Result;
			}
			return TreeList.GetSummaryValueCore(TreeList.Nodes, column, ConvertSummaryItemType(summaryType), true, true);
		} 
		protected SummaryItemType ConvertSummaryItemType(DevExpress.Data.SummaryItemTypeEx type) {
			switch(type) {
				case DevExpress.Data.SummaryItemTypeEx.Max: return SummaryItemType.Max;
				case DevExpress.Data.SummaryItemTypeEx.Min: return SummaryItemType.Min;
				case DevExpress.Data.SummaryItemTypeEx.Sum: return SummaryItemType.Sum;
				case DevExpress.Data.SummaryItemTypeEx.Count: return SummaryItemType.Count;
				case DevExpress.Data.SummaryItemTypeEx.Average: return SummaryItemType.Average;
				case DevExpress.Data.SummaryItemTypeEx.Custom: return SummaryItemType.Custom;
			}
			return SummaryItemType.None;
		}
		protected internal new bool Changed { get { return base.Changed; } }
		protected internal new FormatRuleBase CreateItemInstance() { return base.CreateItemInstance(); }
		protected internal new FormatRuleBase AddInstance() { return base.AddInstance(); }
		protected override void OnCollectionChanged(FormatConditionCollectionChangedEventArgs e) {
			this.summaryInfo = null;
			base.OnCollectionChanged(e);
		}
		protected internal virtual void OnColumnRemoved(TreeListColumn column) {
			if(IsLoading) return;
			for(int n = Count - 1; n >= 0; n--) 
				this[n].OnColumnRemoved(column);
		}
	}
	public class TreeListFormatRuleValueProvider : FormatConditionRuleValueProviderBase {
		public TreeListFormatRuleValueProvider(TreeListFormatRule format) : base(format) { }
		public TreeListNode Node { get; set; }
		public object Value { get; set; }
		public void Set(TreeListNode node, object value) {
			Node = node;
			Value = value;
		}
		protected override object GetValueCore(FormatConditionRuleBase rule) {
			return Value;
		}
		protected override object GetValueExpressionCore(FormatConditionRuleBase rule) {
			return Node;
		}
	}
}
