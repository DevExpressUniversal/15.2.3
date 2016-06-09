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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Helpers;
using DevExpress.XtraGrid.Internal;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.XtraGrid {
	public class GridFormatRule : FormatRuleBase {
		GridColumn column, columnApplyTo;
		GridFormatRuleValueProvider valueProvider;
		bool applyToRow;
		[DefaultValue(null)]
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridFormatRuleColumn"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraGrid.TypeConverters.ColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn Column {
			get { return column; }
			set {
				if(Column == value) return;
				column = value;
				OnColumnChanged();
				OnCollectionChangedCore(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Changed, this, FormatConditionRuleChangeType.All));
			}
		}
		protected override bool GetIsRightToLeft() {
			if(View != null) return View.IsRightToLeft;
			return false;
		}
		void OnColumnChanged() {
			InvalidateRuleState();
		}
		protected override DevExpress.Data.IDataColumnInfo GetColumnInfo() {
			if(View == null) return null;
			return new RuleDataColumnInfoWrapper(View.DataController, GetColumns());
		}
		List<IDataColumnInfo> GetColumns() {
			List<IDataColumnInfo> res = new List<IDataColumnInfo>();
			if(View == null) return res;
			foreach(GridColumn col in ((ColumnView)View).Columns) res.Add(new GridColumnIDataColumnInfoWrapper(col, GridColumnIDataColumnInfoWrapperEnum.General));
			return res;
		}
		[DefaultValue(null)]
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridFormatRuleColumnApplyTo"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraGrid.TypeConverters.ColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn ColumnApplyTo {
			get { return columnApplyTo; }
			set {
				if(ColumnApplyTo == value) return;
				columnApplyTo = value;
				OnColumnChanged();
				OnCollectionChangedCore(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Changed, this, FormatConditionRuleChangeType.All));
			}
		}
		bool XtraShouldSerializeColumnApplyToName() { return ColumnApplyToName != ""; }
		bool XtraShouldSerializeColumnName() { return ColumnName != ""; }
		[DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty, EditorBrowsable(EditorBrowsableState.Never)]
		public string ColumnName {
			get { return Column == null ? "" : Column.Name; }
			set {
				if(View == null) return;
				Column = View.Columns.ColumnByName(value);
			}
		}
		[DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty, EditorBrowsable(EditorBrowsableState.Never)]
		public string ColumnApplyToName {
			get { return ColumnApplyTo == null ? "" : ColumnApplyTo.Name; }
			set {
				if(View == null) return;
				ColumnApplyTo = View.Columns.ColumnByName(value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridFormatRuleCollection Collection { get { return base.Collection as GridFormatRuleCollection; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsValid { get { return base.IsValid && (Column != null || ApplyToRow); } }
		protected override FormatRuleBase CreateInstance() {
			if(Collection == null) return new GridFormatRule();
			else return Collection.CreateItemInstance();
		}
		[DefaultValue(false), XtraSerializableProperty]
		public bool ApplyToRow {
			get { return applyToRow; }
			set {
				if(ApplyToRow == value) return;
				applyToRow = value;
				OnCollectionChangedCore(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Changed, this, FormatConditionRuleChangeType.UI));
			}
		}
		public override void Assign(FormatRuleBase source) {
			base.Assign(source);
			GridFormatRule sourceRule = source as GridFormatRule;
			if(sourceRule == null) return;
			this.ApplyToRow = sourceRule.ApplyToRow;
			this.ColumnApplyTo = FindByColumn(sourceRule.ColumnApplyTo);
			this.Column = FindByColumn(sourceRule.Column);
		}
		GridColumn FindByColumn(GridColumn otherViewColumn) {
			if(otherViewColumn == null) return null;
			if(otherViewColumn.View == View) return otherViewColumn;
			if(View == null) return null;
			return View.Columns.FindLocalColumn(otherViewColumn);
		}
		protected internal GridColumn GetApplyToColumn() { return ColumnApplyTo == null ? Column : ColumnApplyTo; }
		protected ColumnView View { get { return Collection == null ? null : Collection.View; } }
		void OnCollectionChangedCore(FormatConditionCollectionChangedEventArgs e) {
			if(Collection != null) base.Collection.OnCollectionChanged(e);
		}
		public bool IsFit() { return Rule != null && ValueProvider != null && Rule.IsFit(ValueProvider); }
		protected internal virtual GridFormatRuleValueProvider CreateValueProvider() {
			if(View == null || Rule == null) return null;
			return new GridFormatRuleValueProvider(this, View);
		}
		protected internal GridFormatRuleValueProvider ValueProvider {
			get {
				if(valueProvider == null) valueProvider = CreateValueProvider();
				return valueProvider;
			}
		}
		protected internal void EnsureValueProvider() {
			if(!IsValid) {
				valueProvider = null;
				return;
			} 
			if(valueProvider == null) valueProvider = CreateValueProvider();
		}
		protected override string FieldNameCore {
			get {
				return Column == null ? "" : Column.FieldName;
			}
		}
		protected override DevExpress.Data.Filtering.Helpers.ExpressionEvaluator CreateExpressionEvaluator(DevExpress.Data.Filtering.CriteriaOperator criteriaOperator, out bool readyToCreate) {
			readyToCreate = false;
			if(View == null || !View.DataController.IsReady || View.DataController.Columns.Count == 0) return null;
			readyToCreate = true;
			Exception e;
			return View.DataController.CreateExpressionEvaluator(criteriaOperator, true, out e);
		}
		internal void OnColumnRemoved(GridColumn column) {
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
	public class GridFormatRuleCollection : FormatRuleCollection<GridFormatRule, GridColumn> {
		ColumnView view;
		FormatRuleSummaryInfoCollection summaryInfo;
		public GridFormatRuleCollection(ColumnView view) {
			this.view = view;
		}
		protected internal new bool Changed { get { return base.Changed; } }
		protected override void AssignColumn(GridFormatRule format, GridColumn column) {
			format.Column = column;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ColumnView View { get { return view; } }
		protected override void OnCollectionChanged(FormatConditionCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			this.summaryInfo = null;
			if(!IsLoading && View != null) View.OnRuleCollectionChanged(e);
		}
		protected internal FormatRuleSummaryInfoCollection SummaryInfo { 
			get { 
				if(summaryInfo == null) {
					summaryInfo = CreateSummaryInfo();
				}
				return summaryInfo; 
			} 
		}
		protected override LookAndFeel.UserLookAndFeel ElementsLookAndFeel {
			get { return View.ElementsLookAndFeel; }
		}
		protected override FormatConditionRuleState GetRuleState(FormatRuleBase format) {
			GridFormatRule gf = format as GridFormatRule;
			if(gf.ValueProvider == null) return FormatConditionRuleState.NullState;
			return gf.ValueProvider.RuleState;
		}
		public override bool IsLoading {
			get {
				return base.IsLoading || View == null || View.IsLoading;
			}
		}
		internal void EnsureValueProviders() {
			foreach(var gr in this) {
				if(gr.IsValid) gr.EnsureValueProvider();
			}
		}
		internal new bool CheckAllValuesReady() { return base.CheckAllValuesReady(); }
		internal bool TryUpdateStateValues() {
			return TryUpdateStateValues(View.DataController, SummaryInfo);
		}
		internal new void ResetValuesReady() {
			base.ResetValuesReady();
		}
		internal new FormatRuleBase CreateItemInstance() { return base.CreateItemInstance(); }
		internal new FormatRuleBase AddInstance() { return base.AddInstance(); }
		internal void Assign(GridFormatRuleCollection source) {
			base.Assign(source);
		}
		internal void ResetVisualCacheInternal() {
			ResetVisualCache();
		}
		internal void OnColumnRemoved(GridColumn column) {
			if(IsLoading) return;
			for(int n = Count - 1; n >= 0; n--) {
				this[n].OnColumnRemoved(column);
			}
		}
		internal void ResetEvaluatorsCore() {
			ResetDataSourceProperties();
		}
	}
}
namespace DevExpress.XtraGrid.Helpers {
	public class GridFormatRuleValueProvider : FormatConditionRuleValueProviderBase {
		BaseView view;
		int listSourceRow;
		object cell;
		public GridFormatRuleValueProvider(GridFormatRule format, BaseView view) : base(format) {
			this.view = view;
			this.listSourceRow = 0;
		}
		public new GridFormatRule Format { get { return (GridFormatRule)base.Format; } }
		public object Cell { get { return cell; } set { cell = value; } }
		public int ListSourceRow { get { return listSourceRow; } set { listSourceRow = value; } }
		public void SetData(object cellValue, int listSourceRow) {
			this.cell = cellValue;
			this.listSourceRow = listSourceRow;
		}
		protected override object GetValueCore(FormatConditionRuleBase rule) {
			return cell;
		}
		protected override object GetValueExpressionCore(FormatConditionRuleBase rule) {
			return listSourceRow;
		}
	}
}
