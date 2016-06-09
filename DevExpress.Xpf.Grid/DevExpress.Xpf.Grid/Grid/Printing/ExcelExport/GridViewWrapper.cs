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
using System.Linq;
using DevExpress.XtraExport.Helpers;
using DevExpress.Data.Summary;
using DevExpress.Data;
using DevExpress.Xpf.Data;
using DevExpress.XtraPrinting;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting.Printing;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using System.ComponentModel;
using DevExpress.Data.Async;
using DevExpress.Xpf.Core;
using System.Threading;
using DevExpress.Export.Xl;
using System.Reflection;
using DevExpress.Data.Access;
namespace DevExpress.Xpf.Grid.Printing {
	public abstract class GridViewExportHelperBase<TCol, TRow> : DataViewExportHelperBase<TCol, TRow> where TRow : RowBaseWrapper where TCol : ColumnWrapper {
		readonly List<ISummaryItemEx> groupFooterSummary;
		readonly List<ISummaryItemEx> groupHeaderSummary;
		readonly List<TCol> groupedColumns;
		public GridViewExportHelperBase(TableView view, ExportTarget target)
			: base(view, target) {
				this.groupFooterSummary = GetGroupFooterSummary().ToList();
				this.groupHeaderSummary = GetGroupHeaderSummary().ToList();
				this.groupedColumns = GetPrintableColumns(view.GroupedColumns);
		}
		protected GridControl Grid { get { return View.Grid as GridControl; } }
		protected new TableView View { get { return base.View as TableView; } }
		public override bool ShowGroupedColumns { get { return View.ShowGroupedColumns; }  }
		public override IEnumerable<ISummaryItemEx> GridGroupSummaryItemCollection { get { return groupFooterSummary; } }
		public override IEnumerable<ISummaryItemEx> GroupHeaderSummaryItems { get { return groupHeaderSummary; } }
		protected bool PrintGroupFooters { get { return View.ShowGroupFooters && View.PrintGroupFooters; } }
		protected override long RowCountCore { get { return Grid.DataController.ListSourceRowCount + Grid.DataController.GroupRowCount; } }
		protected override FormatConditionCollection FormatConditionsCore { get { return View.FormatConditions; } }
		#region Columns
		public override IEnumerable<TCol> GetGroupedColumns() {
			return groupedColumns;
		}
		#endregion
		#region Summary
		protected abstract IEnumerable<ISummaryItemEx> GetGroupHeaderSummary();
		protected abstract IEnumerable<ISummaryItemEx> GetGroupFooterSummary();
		protected IEnumerable<ISummaryItemEx> GetGroupHeaderSummaryCore() {
			return GetPrintableGroupSummary(item => !IsGroupFooterSummary(item));
		}
		protected IEnumerable<ISummaryItemEx> GetGroupFooterSummaryCore() {
			return GetPrintableGroupSummary(item => IsGroupFooterSummary(item));
		}
		protected IEnumerable<ISummaryItemEx> GetPrintableGroupSummary(Func<SummaryItemBase, bool> canPrintSummaryItem) {
			return GetPrintableSummary(Grid.GroupSummary, canPrintSummaryItem, CreateGroupSummaryItemWrapper);
		}
		protected bool IsGroupFooterSummary(SummaryItemBase item) {
			return !string.IsNullOrEmpty(((GridSummaryItem)item).ShowInGroupColumnFooter);
		}
		SummaryItemExportWrapper CreateGroupSummaryItemWrapper(SummaryItemBase item) {
			string fieldName = GetSummaryFieldName(item, SummaryItemBase.ColumnSummaryType.Group);
			string showInColumn = GetExportToColumn(fieldName, item.ActualShowInColumn);
			string displayFormat = GetDisplayFormat(item, SummaryItemBase.ColumnSummaryType.Group);
			return new SummaryItemExportWrapper(fieldName, showInColumn, item.SummaryType, displayFormat, null, rowHandle => Grid.GetGroupSummaryValue(rowHandle, (GridSummaryItem)item));
		}
		#endregion
		#region CellMerge
		public override int RaiseMergeEvent(int row1, int row2, TCol col) {
			bool? result = View.RaiseCellMerge(((ColumnWrapper)col).Column, row1, row2, false);
			if(result.HasValue)
				return result.Value ? 0 : -1;
			return 3;
		}
		public override bool GetAllowMerge(TCol col) {
			ColumnWrapper columnWrapper = (ColumnWrapper)col;
			return columnWrapper.Column.AllowCellMerge.HasValue ? columnWrapper.Column.AllowCellMerge.Value : View.AllowCellMerge;
		}
		#endregion
		#region Rows
		public override IEnumerable<TRow> GetAllRows() {
			for(int visibleIndex = 0; visibleIndex < DataProvider.VisibleCount; visibleIndex++) {
				int rowHandle = Grid.GetRowHandleByVisibleIndex(visibleIndex);
				if(Grid.GetRowLevelByRowHandle(rowHandle) > 0)
					continue;
				yield return GetRowByRowHandle(rowHandle);
			}
		}
		internal TRow GetRowByRowHandle(int rowHandle) {
			if(Grid.IsGroupRowHandle(rowHandle))
				return (TRow)CreateGroupRow(rowHandle);
			else
				return CreateDataRow(rowHandle);
		}
		IGroupRow<RowBaseWrapper> CreateGroupRow(int rowHandle) {
			int rowLevel = Grid.GetRowLevelByRowHandle(rowHandle);
			string displayText = View.GetGroupRowDisplayText(rowHandle);
			bool isExpanded = Grid.IsGroupRowExpanded(rowHandle);
			IEnumerable<TRow> childRows = GetChildRows(rowHandle);
			return new GroupRowWrapper(rowHandle, rowLevel, -1, displayText, isExpanded, childRows, View);
		}
		IEnumerable<TRow> GetChildRows(int groupRowHandle) {
			if(!Grid.IsGroupRowHandle(groupRowHandle))
				yield break;
			int childCount = Grid.GetChildRowCount(groupRowHandle);
			for(int childIndex = 0; childIndex < childCount; childIndex++) {
				int childRowHandle = Grid.GetChildRowHandle(groupRowHandle, childIndex);
				yield return GetRowByRowHandle(childRowHandle);
			}
		}
		TRow CreateDataRow(int rowHandle) {
			return new DataRowWrapper(rowHandle, Grid.GetRowLevelByRowHandle(rowHandle), Grid.GetListIndexByRowHandle(rowHandle)) as TRow;
		}
		#endregion
	}
	public abstract class DataViewExportHelperBase<TCol, TRow> : IGridView<TCol, TRow> where TRow : IRowBase where TCol : ColumnWrapper {
		#region IGridView Properties
		public IEnumerable<IFormatRuleBase> FormatRulesCollection { get { return formatRulesCore; } }
		public IEnumerable<FormatConditionObject> FormatConditionsCollection { get { yield break; } }
		public abstract bool ShowGroupedColumns { get; }
		public IAdditionalSheetInfo AdditionalSheetInfo { get { return new AdditionalSheetInfoWrapper(); } }
		public abstract IEnumerable<ISummaryItemEx> GridGroupSummaryItemCollection { get; }
		public abstract IEnumerable<ISummaryItemEx> GroupHeaderSummaryItems{ get; }
		public abstract IEnumerable<TCol> GetGroupedColumns();
		public abstract bool GetAllowMerge(TCol col);
		public IEnumerable<ISummaryItemEx> GridTotalSummaryItemCollection { get { return totalSummary; } }
		public IEnumerable<ISummaryItemEx> FixedSummaryItems{ get { return GetFixedTotalSummary(); } }
		public string FilterString { get { return ReferenceEquals(View.DataControl.DataProviderBase.FilterCriteria, null) ? null : View.DataControl.DataProviderBase.FilterCriteria.ToString().Replace(DisplayTextPropertyDescriptor.DxFtsPropertyPrefix, ""); } }
		public virtual bool ShowFooter { get { return PrintTotalSummary || PrintFixedTotalSummary; } }
		public virtual bool ShowGroupFooter { get { return true; } }
		public bool ReadOnly { get { return !View.AllowEditing; } }
		bool PrintTotalSummary { get { return View.ShouldPrintTotalSummary; } }
		bool PrintFixedTotalSummary { get { return View.ShouldPrintFixedTotalSummary; } }
		long IGridView<TCol, TRow>.RowCount { get { return RowCountCore; } }
		public FormatSettings GetRowCellFormatting(TRow row, TCol col) { return null; }
		public string GetRowCellHyperlink(TRow row, TCol col) { return null; }
		public string GetViewCaption { get { return string.Empty; } }
		public DevExpress.Export.Xl.XlCellFormatting AppearanceGroupRow { get { return null; } }
		public XlCellFormatting AppearanceEvenRow { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceOddRow { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceGroupFooter { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceHeader { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceFooter { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceRow { get { return new XlCellFormatting(); } }
		public IGridViewAppearancePrint AppearancePrint { get { return null; } }
		public int RowHeight { get { return -1; } }
		public int FixedRowsCount { get { return 1; } }
		public bool IsCancelPending { get { return false; } }
		#endregion
		#region Fields
		protected readonly List<TCol> visibleColumns;
		readonly List<ISummaryItemEx> totalSummary;
		readonly IEnumerable<IFormatRuleBase> formatRulesCore;
		public DataViewBase View { get; private set; }
		#endregion
		#region Properties
		protected DataControlBase DataControl { get { return View.DataControl; } }
		protected DataProviderBase DataProvider { get { return View.DataProviderBase; } }
		protected abstract long RowCountCore { get; }
		protected abstract FormatConditionCollection FormatConditionsCore { get; }
		#endregion
		public DataViewExportHelperBase(DataViewBase view, ExportTarget target) {
			View = view;
			visibleColumns = GetPrintableColumns(view.PrintableColumns);
			totalSummary = GetFooterSummary().ToList();
			formatRulesCore = GetFormatRules();
		}
		#region Summary
		protected IEnumerable<ISummaryItemEx> GetPrintableSummary(ISummaryItemOwner items, Func<SummaryItemBase, bool> canPrintSummaryItem, Func<SummaryItemBase, ISummaryItemEx> createSummaryItemWrapper) {
			foreach(var item in items) {
				if(item.Visible && canPrintSummaryItem(item))
					yield return createSummaryItemWrapper(item);
			}
		}
		protected string GetDisplayFormat(SummaryItemBase item, SummaryItemBase.ColumnSummaryType summaryType) {
			if(item.ActualShowInColumn == item.FieldName)
				return item.GetFooterDisplayFormatSameColumn(GetDisplayFormat(item.FieldName), summaryType);
			return item.GetFooterDisplayFormat(GetDisplayFormat(item.FieldName), summaryType);
		}
		protected string GetDisplayFormat(string fieldName) {
			ColumnBase column = DataControl.ColumnsCore[fieldName];
			if(column != null)
				return column.DisplayFormat;
			return string.Empty;
		}
		protected string GetExportToColumn(string fieldName, string showInColumn) {
			return string.IsNullOrEmpty(showInColumn) ? fieldName : showInColumn;
		}
		protected string GetSummaryFieldName(SummaryItemBase item, SummaryItemBase.ColumnSummaryType summaryType) {
			if(string.IsNullOrEmpty(item.FieldName) && item.SummaryType == SummaryItemType.Count) {
				if(string.IsNullOrEmpty(item.ActualShowInColumn))
					return GetDefaultFieldName(item, summaryType);
				return item.ActualShowInColumn;
			}
			return item.FieldName;
		}
		string GetDefaultFieldName(SummaryItemBase item, SummaryItemBase.ColumnSummaryType summaryType) {
			if(summaryType == SummaryItemBase.ColumnSummaryType.Total)
				return GetFixedTotalSummaryItemFieldName(item.Alignment);
			return GetLastVisibleColumnName();
		}
		string GetLastVisibleColumnName() {
			return visibleColumns.Last().Return(column => column.FieldName, () => string.Empty);
		}
		#endregion
		#region TotalSummary
		IEnumerable<ISummaryItemEx> GetFooterSummary() {
			var totalSummary = PrintTotalSummary ? GetTotalSummary() : Enumerable.Empty<ISummaryItemEx>();
			var fixedTotalSummary = PrintFixedTotalSummary ? GetFixedTotalSummary() : Enumerable.Empty<ISummaryItemEx>();
			return totalSummary.Concat(fixedTotalSummary);
		}
		SummaryItemExportWrapper CreateTotalSummaryItemWrapper(SummaryItemBase item, string fieldName, string showInColumn) {
			string displayFormat = GetDisplayFormat(item, SummaryItemBase.ColumnSummaryType.Total);
			object totalSummaryValue = GetCustomTotalSummaryValue(item);
			return new SummaryItemExportWrapper(fieldName, showInColumn, item.SummaryType, displayFormat, totalSummaryValue, row => null);
		}
		IEnumerable<ISummaryItemEx> GetPrintableTotalSummary(Func<SummaryItemBase, bool> canPrintSummaryItem, Func<SummaryItemBase, ISummaryItemEx> createSummaryItemWrapper) {
			return GetPrintableSummary(DataControl.TotalSummaryCore, canPrintSummaryItem, createSummaryItemWrapper);
		}
		IEnumerable<ISummaryItemEx> GetTotalSummary() {
			return GetPrintableTotalSummary(CanPrintTotalSummaryItem, CreateTotalSummaryItemWrapper);
		}
		bool CanPrintTotalSummaryItem(SummaryItemBase item) {
			return item.Alignment == GridSummaryItemAlignment.Default;
		}
		SummaryItemExportWrapper CreateTotalSummaryItemWrapper(SummaryItemBase item) {
			return CreateTotalSummaryItemWrapper(item, item.FieldName, item.ActualShowInColumn);
		}
		IEnumerable<ISummaryItemEx> GetFixedTotalSummary() {
			return GetPrintableTotalSummary(CanPrintFixedTotalSummaryItem, CreateFixedTotalSummaryItemWrapper);
		}
		bool CanPrintFixedTotalSummaryItem(SummaryItemBase item) {
			if(string.IsNullOrEmpty(item.FieldName) && item.SummaryType != SummaryItemType.Count)
				return false;
			return item.Alignment != GridSummaryItemAlignment.Default;
		}
		object GetCustomTotalSummaryValue(SummaryItemBase item) {
			if(item.SummaryType == SummaryItemType.Custom)
				return DataControl.GetTotalSummaryValue(item);
			return null;
		}
		SummaryItemExportWrapper CreateFixedTotalSummaryItemWrapper(SummaryItemBase item) {
			string fieldName = GetSummaryFieldName(item, SummaryItemBase.ColumnSummaryType.Total);
			return CreateTotalSummaryItemWrapper(item, fieldName, GetExportToColumn(fieldName, item.ActualShowInColumn));
		}
		string GetFixedTotalSummaryItemFieldName(GridSummaryItemAlignment alignment) {
			if(visibleColumns.Count == 0)
				return string.Empty;
			switch(alignment) {
				case GridSummaryItemAlignment.Left:
					return visibleColumns.First().FieldName;
				case GridSummaryItemAlignment.Right:
					return visibleColumns.Last().FieldName;
				default:
					return string.Empty;
			}
		}
		#endregion
		#region Columns
		public IEnumerable<TCol> GetAllColumns() {
			return visibleColumns;
		}
		internal static TCol CreateColumn(ColumnBase column, int logicalPosition) {
			return (TCol)new ColumnWrapper(column, logicalPosition);
		}
		public TCol GetColumn(string fieldName) {
			return visibleColumns.FirstOrDefault(column => column.FieldName == fieldName);
		}
		protected virtual List<TCol> GetPrintableColumns(IEnumerable<ColumnBase> processedColumns) {
			IEnumerable<ColumnBase> columns = processedColumns.Cast<ColumnBase>();
			if(DataControl.BandsLayoutCore != null)
				columns = GetPrintableColumns(DataControl.BandsLayoutCore.PrintableBands);
			return columns.Select((column, index) => CreateColumn(column, index)).ToList();
		}
		IEnumerable<ColumnBase> GetPrintableColumns(IEnumerable<BandBase> bands) {
			IEnumerable<ColumnBase> columns = Enumerable.Empty<ColumnBase>();
			foreach(var band in bands) {
				columns = columns.Concat(GetPrintableColumns(band.PrintableBands));
				columns = columns.Concat(band.ActualRows.SelectMany(row => row.Columns).Where(column => column.AllowPrinting).Cast<ColumnBase>());
			}
			return columns;
		}
		#endregion
		#region Rows
		public abstract IEnumerable<TRow> GetAllRows();
		#endregion
		#region GetRowCellValue
		public object GetRowCellValue(TRow row, TCol col) {
			object value = GetRowCellValue(row.LogicalPosition, col.LogicalPosition);
			if (col.ColEditType != ColumnEditTypes.Sparkline)
				return value;
			return GetRowCellValueSparkline(col, value);
		}
		public virtual object GetRowCellValue(int rowHandle, int visibleIndex) {
			if(visibleIndex < 0 || visibleIndex >= visibleColumns.Count)
				return null;
			ColumnBase column = visibleColumns[visibleIndex].Column;
			return View.GetExportValue(rowHandle, column);
		}
		 public virtual object GetRowCellValueSparkline(TCol col, object val) {
			 try {
				 DevExpress.Xpf.Editors.Settings.SparklineEditSettings editSettings = (DevExpress.Xpf.Editors.Settings.SparklineEditSettings)col.Column.EditSettings;
				 if(string.IsNullOrEmpty(editSettings.PointValueMember))
					 return val;
				 var collection = (System.Collections.IEnumerable)val;
				 List<object> result = new List<object>();
				 PropertyInfo pi = null;				 
				 foreach(var element in collection) {
					 if(pi == null)
						 pi = element.GetType().GetProperty(editSettings.PointValueMember);
					 result.Add(pi.GetValue(element, null));
				 }
				 return result;
			 } catch {
				 return val;
			 }
		}
		#endregion
		#region Merge
		public virtual int RaiseMergeEvent(int row1, int row2, TCol col) { return -1; }
		#endregion
		#region FormatRules
		IEnumerable<IFormatRuleBase> GetFormatRules() {
			foreach(var formatCondition in FormatConditionsCore) {
				var column = GetColumn(formatCondition.FieldName);
				if(column == null)
					continue;
				var rule = GetFormatRule(formatCondition, column);
				if(rule.IsValid)
					yield return rule;
			}
		}
		public static FormatRuleExportWrapper GetFormatRule(FormatConditionBase formatCondition, TCol column) {
			return new FormatRuleExportWrapper(formatCondition, column);
		}
		#endregion
	}
	public class GridViewExportHelper<TCol, TRow> : GridViewExportHelperBase<TCol, TRow>, IDisposable
		where TRow : RowBaseWrapper
		where TCol : ColumnWrapper {
		readonly ItemsGenerationStrategyBase itemsGenerationStrategy;
		public GridViewExportHelper(TableView view, ExportTarget target) 
			: base(view, target) {
			itemsGenerationStrategy = view.CreateItemsGenerationStrategy();
			GenerateAll();
		}
		bool allowPartialGrouping = false;
		#region ItemsGeneration
		void GenerateAll() {
			Grid.LockUpdateLayout = true;
			if(View.AllowPartialGrouping) {
				Grid.DisableAllowPartialGrouping();
				allowPartialGrouping = true;
			}
			itemsGenerationStrategy.GenerateAll();
		}
		void ClearAll() {
			if(allowPartialGrouping) {
				allowPartialGrouping = false;
				Grid.UpdateAllowPartialGrouping();
			}
			itemsGenerationStrategy.ClearAll();
			Grid.LockUpdateLayout = false;
		}
		#endregion
		public void Dispose() {
			ClearAll();
		}
		protected override IEnumerable<ISummaryItemEx> GetGroupHeaderSummary() {
			return Enumerable.Empty<ISummaryItemEx>();
		}
		protected override IEnumerable<ISummaryItemEx> GetGroupFooterSummary() {
			var groupRowSummary = GetGroupHeaderSummaryCore();
			var groupFooterSummary = PrintGroupFooters ? GetGroupFooterSummaryCore() : Enumerable.Empty<ISummaryItemEx>();
			return groupRowSummary.Concat(groupFooterSummary);
		}
	}
	public class GridViewReportHelper<TCol, TRow> : GridViewExportHelperBase<TCol, TRow>
		where TRow : RowBaseWrapper
		where TCol : ColumnWrapper {
		public GridViewReportHelper(TableView view, ExportTarget target) : base(view, target) {
		}
		protected override IEnumerable<ISummaryItemEx> GetGroupFooterSummary() {
			return PrintGroupFooters ? GetGroupFooterSummaryCore() : Enumerable.Empty<ISummaryItemEx>();
		}
		protected override IEnumerable<ISummaryItemEx> GetGroupHeaderSummary() {
			return GetGroupHeaderSummaryCore();
		}
		protected override List<TCol> GetPrintableColumns(IEnumerable<ColumnBase> processedColumns) {
			IEnumerable<ColumnBase> columns = processedColumns.Cast<ColumnBase>();
			return columns.Select((column, index) => CreateColumn(column, index)).ToList();
		}
	}
	class SummaryItemExportWrapper : ISummaryItemEx {
		public string DisplayFormat { get; set; }
		public string FieldName { get; private set; }
		public SummaryItemType SummaryType { get; private set; }
		public object SummaryValue { get; private set; }
		public string ShowInColumnFooterName { get; private set; }
		readonly Func<int, object> getSummaryValueByGroupRowHandle;
		public object GetSummaryValueByGroupId(int groupId) {
			return getSummaryValueByGroupRowHandle(groupId);
		}
		public SummaryItemExportWrapper(string fieldName, string shownInColumn, SummaryItemType summaryType, string displayFormat, object summaryValue, Func<int, object> getSummaryValueByGroupRowHandle) {
			FieldName = fieldName;
			SummaryType = summaryType;
			DisplayFormat = displayFormat;
			SummaryValue = summaryValue;
			ShowInColumnFooterName = shownInColumn;
			this.getSummaryValueByGroupRowHandle = getSummaryValueByGroupRowHandle;
		}
	}
	class AdditionalSheetInfoWrapper:IAdditionalSheetInfo {
		public string Name { get { return "Additional Data"; } }
		public XlSheetVisibleState VisibleState { get { return XlSheetVisibleState.Hidden; } }
	}
	public class FormatRuleExportWrapper : IFormatRuleBase {
		public FormatConditionBase FormatCondition { get; private set; }
		public bool ApplyToRow {
			get { return FormatCondition.GetApplyToFieldName() == null; }
		}
		public IColumn Column { get; private set; }
		public IColumn ColumnApplyTo { get; private set; }
		public string ColumnName {
			get { return Column.Name; }
		}
		public bool Enabled {
			get { return true; }
		}
		public string Name {
			get { return GetHashCode().ToString(); }
		}
		readonly FormatConditionRuleBase ruleCore;
		public IFormatConditionRuleBase Rule {
			get { return ruleCore; }
		}
		public bool StopIfTrue {
			get { return false; }
		}
		public object Tag { get; set; }
		public bool IsValid { get { return ruleCore.IsValid; } }
		public FormatRuleExportWrapper(FormatConditionBase formatCondition, ColumnWrapper column) {
			FormatCondition = formatCondition;
			ruleCore = FormatCondition.CreateExportWrapper();
			if(column != null)
				ruleCore.ColumnType = column.ColumnType;
			Column = column;
			ColumnApplyTo = null;
		}
	}
	public static class GridReportHelper {
		public static object GetSource(TableView view) {
			if(view.DataProviderBase == null)
				return null;
			if(view.DataProviderBase.IsAsyncServerMode)
				return new AsyncServerModeItemsSourceWrapper(view);
			if(view.DataProviderBase.IsServerMode) {
				if(view.DataProviderBase.IsICollectionView)
					return new CollectionViewItemsSourceWrapper(view);
				return new ServerModeItemsSourceWrapper(view);
			}
			if(view.DataProviderBase.DataSource is System.Data.DataTable)
				return view.DataProviderBase.DataSource;
			if(view.DataProviderBase.DataController.DataSource is ITypedList)
				return view.DataProviderBase.DataController.DataSource;
			return view.DataProviderBase.DataSource;
		}
	}
	public abstract class ServerModeItemsSourceWrapperBase : List<object> {
		bool isFilled;
		public bool IsFilled {
			get { return isFilled; }
			set { isFilled = value; }
		}
		protected CriteriaOperator GetReportFilter(IServiceProvider servProvider) {
			IFilterStringContainer filterStringContainer = (IFilterStringContainer)servProvider;
			string filter = filterStringContainer.FilterString;
			if(filter == null)
				return null;
			CriteriaOperator result = null;
			try {
				result = CriteriaOperator.Parse(filter);
			}
			catch { }
			return result;
		}
	}
	public class CollectionViewItemsSourceWrapper : ServerModeItemsSourceWrapperBase, IListAdapter {
		readonly ICollectionViewHelper Server;
		public CollectionViewItemsSourceWrapper(TableView view) {
			Server = view.DataProviderBase.DataController.DataSource as ICollectionViewHelper;
		}
		public void FillList(IServiceProvider servProvider) {
			IsFilled = false;
			Clear();
			AddRange(Server.GetAllFilteredAndSortedRows().Cast<object>());
			IsFilled = true;
		}
	}
	public class ServerModeItemsSourceWrapper : ServerModeItemsSourceWrapperBase, IListAdapter {
		readonly IListServer Server;
		public ServerModeItemsSourceWrapper(TableView view) {
			IDXCloneable dataSource = view.DataProviderBase.DataController.DataSource as IDXCloneable;
			Server = dataSource.DXClone() as IListServer;
		}
		public void FillList(IServiceProvider servProvider) {
			IsFilled = false;
			Clear();
			CriteriaOperator criteria = GetReportFilter(servProvider);
			Server.Apply(criteria, null, 0, null, null);
			AddRange(Server.GetAllFilteredAndSortedRows().Cast<object>());
			IsFilled = true;
		}
		bool IListAdapter.IsFilled {
			get { return IsFilled; }
		}
	}
	public class AsyncServerModeItemsSourceWrapper : ServerModeItemsSourceWrapperBase, IListAdapterAsync, ITypedList {
		readonly IAsyncListServer Server;
		ITypedList TypedList { get { return Server as ITypedList; } }
		readonly DataViewBase View;
		ReportsAsyncResult Result;
		public AsyncServerModeItemsSourceWrapper(TableView view) {
			View = view;
			IDXCloneable dataSource = view.DataProviderBase.DataController.DataSource as IDXCloneable;
			Server = dataSource.DXClone() as IAsyncListServer;
			Server.SetReceiver(new AsyncListWrapper(view.DataProviderBase.DataController as AsyncServerModeDataController, Server));
		}
		public IAsyncResult BeginFillList(IServiceProvider servProvider, CancellationToken token) {
			IsFilled = false;
			Clear();
			CriteriaOperator criteria = GetReportFilter(servProvider);
			CommandApply applyFilter = Server.Apply(criteria, null, 0, null, null);
			CommandGetAllFilteredAndSortedRows commandGetRows = Server.GetAllFilteredAndSortedRows();
			Result = new ReportsAsyncResult(View, Server, applyFilter, commandGetRows);
			Result.Start();
			return Result;
		}
		public void EndFillList(IAsyncResult result) {
			AddRange(Result.Rows);
			IsFilled = true;
		}
		public void FillList(IServiceProvider servProvider) {
			var result = BeginFillList(servProvider, default(CancellationToken));
			EndFillList(result);
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return TypedList.GetItemProperties(listAccessors);
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return TypedList.GetListName(listAccessors);
		}
	}
	public class ReportsAsyncResult : IAsyncResult {
		readonly CommandApply CommandApplyFilter;
		readonly CommandGetAllFilteredAndSortedRows CommandGetRows;
		readonly IAsyncListServer Server;
		readonly DataViewBase View;
		bool completed = false;
		public IList<object> Rows { get; private set; }
		public ReportsAsyncResult(DataViewBase view, IAsyncListServer server, CommandApply commandApplyFilter, CommandGetAllFilteredAndSortedRows commandGetRows) {
			View = view;
			Server = server;
			CommandApplyFilter = commandApplyFilter;
			CommandGetRows = commandGetRows;
		}
		public void Start() {
			Rows = ApplyAsyncCommand();
			completed = true;
		}
		IList<object> ApplyAsyncCommand() {
			string progressWindowTitle = View.GetLocalizedString(GridControlStringId.ProgressWindowTitle);
			string cancelButtonCaption = View.GetLocalizedString(GridControlStringId.ProgressWindowCancel);
			using(System.Threading.ManualResetEvent stopEvent = new System.Threading.ManualResetEvent(false)) {
				System.Threading.Thread progressThread = new System.Threading.Thread(delegate() {
					DXWindow progressWindow = ProgressControl.CreateProgressWindow(stopEvent, true, progressWindowTitle, cancelButtonCaption);
					progressWindow.Show();
					System.Windows.Threading.Dispatcher.Run();
				});
				progressThread.SetApartmentState(System.Threading.ApartmentState.STA);
				progressThread.Start();
				while(!Server.WaitFor(CommandApplyFilter)) {
					if(progressThread.Join(100)) {
						Server.Cancel(CommandApplyFilter);
						break;
					}
				}
				while(!Server.WaitFor(CommandGetRows)) {
					if(progressThread.Join(100)) {
						Server.Cancel(CommandGetRows);
						break;
					}
				}
				stopEvent.Set();
				progressThread.Join();
			}
			return CommandGetRows.IsCanceled ? new List<object>() : CommandGetRows.Rows.Cast<object>().ToList();
		}
		public object AsyncState {
			get { throw new NotImplementedException(); }
		}
		public System.Threading.WaitHandle AsyncWaitHandle {
			get { throw new NotImplementedException(); }
		}
		public bool CompletedSynchronously {
			get { throw new NotImplementedException(); }
		}
		public bool IsCompleted {
			get { return completed; }
		}
	}
}
