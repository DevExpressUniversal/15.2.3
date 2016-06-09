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

using DevExpress.Web.Rendering;
using DevExpress.XtraExport.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Summary;
using DevExpress.Web.Data;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using System.Collections;
using System.Drawing;
using DevExpress.Export.Xl;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public abstract class GridPrinterBase : IWebControlPageSettings, IDisposable {
		public GridPrinterBase(ASPxGridExporterBase exporter, ASPxGridBase grid) {
			Exporter = exporter;
			Grid = grid;
			BeginExport();
		}
		public ASPxGridExporterBase Exporter { get; set; }
		public ASPxGridBase Grid { get; private set; }
		public WebDataProxy DataProxy { get { return Grid.DataBoundProxy; } }
		public GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		public string SavedFilterExpression { get; private set; }
		public ColumnFilterMode? SavedKeyColumnFilterMode { get; private set; }
		protected virtual IWebGridDataColumn KeyFieldColumn { get { return ColumnHelper.FindColumnByString(Grid.KeyFieldName) as IWebGridDataColumn; } }
		protected bool ChooseSelectedRowsByFilter { get { return Exporter.ExportSelectedRowsOnly && !Grid.DataSourceForceStandardPaging; } }
		protected bool ChooseSelectedRowsByChecking { get { return Exporter.ExportSelectedRowsOnly && Grid.DataSourceForceStandardPaging; } }
		protected virtual bool RequireExpandAllGroups { get { return false; } }
		public virtual void BeginExport() {
			PrepareGridForExport();
			Grid.BeginExport(); 
		}
		public virtual void EndExport() {
			DataProxy.PrinterPageSettings = null;
			Grid.FilterExpression = SavedFilterExpression;
			if(SavedKeyColumnFilterMode.HasValue && KeyFieldColumn != null)
				KeyFieldColumn.Adapter.Settings.FilterMode = SavedKeyColumnFilterMode.Value;
			Grid.EndExport();
		}
		public IEnumerable<int> EnumerateExportedVisibleRows() {
			bool checkSelection = ChooseSelectedRowsByChecking;
			for(int i = 0; i < DataProxy.VisibleRowCountOnPage; i++) {
				if(checkSelection && !Grid.Selection.IsRowSelected(i))
					continue;
				yield return i;
			}
		}
		protected virtual void PrepareGridForExport() {
			DataProxy.PrinterPageSettings = this;
			if(NeedRebindGrid())
				Grid.DataBind();
			SavedFilterExpression = Grid.FilterExpression;
			if(KeyFieldColumn != null) {
				SavedKeyColumnFilterMode = KeyFieldColumn.Adapter.Settings.FilterMode;
				KeyFieldColumn.Adapter.Settings.FilterMode = ColumnFilterMode.Value;
			}
			if(RequireExpandAllGroups)
				DataProxy.ExpandAll();
			FilterOutNonSelectedRows();
		}
		protected virtual bool NeedRebindGrid() {
			if(Grid.DataSourceForceStandardPaging && Grid.PageIndex > -1)
				return true;
			return false;
		}
		protected virtual void FilterOutNonSelectedRows() { 
			if(!ChooseSelectedRowsByFilter) return;
			CriteriaOperator gridCriteria = CriteriaOperator.Parse(SavedFilterExpression);
			CriteriaOperator selectionCriteria = GetSelectionFilterCriteria();
			string newFilterExpression = !ReferenceEquals(selectionCriteria, null) ? selectionCriteria.ToString() : string.Empty;
			if(!ReferenceEquals(gridCriteria, null) && Grid.FilterEnabled)
				newFilterExpression = GroupOperator.Combine(GroupOperatorType.And, gridCriteria, selectionCriteria).ToString();
			Grid.FilterExpression = newFilterExpression;
		}
		protected CriteriaOperator GetSelectionFilterCriteria() {
			if(Grid.Selection.Count < 1)
				return CriteriaOperator.Parse("False");
			bool filterSelected = DataProxy.ListSourceRowCount / 2 > Grid.Selection.Count || Grid.SettingsBehavior.SelectionStoringMode == GridViewSelectionStoringMode.DataIntegrityOptimized; 
			List<object> filteredKeys = GetKeysForFiltration(filterSelected);
			if(!DataProxy.IsMultipleKeyFields && filteredKeys.Count > 0) {
				CriteriaOperator op = new InOperator(Grid.KeyFieldName, filteredKeys);
				if(!filterSelected)
					op = !op;
				return op;
			}
			CriteriaOperator criteria = null;
			string[] keyFieldNames = DataProxy.KeyFieldNames.ToArray();
			foreach(object multipleKey in filteredKeys) {
				CriteriaOperator summand = null;
				object[] keys = (object[])multipleKey;
				for(int i = 0; i < keyFieldNames.Length; i++) {
					CriteriaOperator op = new BinaryOperator(keyFieldNames[i], keys[i]);
					if(!filterSelected)
						op = !op;
					if(Object.ReferenceEquals(summand, null))
						summand = op;
					else
						summand = GroupOperator.Combine(GroupOperatorType.And, summand, op);
				}
				if(Object.ReferenceEquals(criteria, null))
					criteria = summand;
				else
					criteria = GroupOperator.Combine(GroupOperatorType.Or, criteria, summand);
			}
			return criteria;
		}
		protected List<object> GetKeysForFiltration(bool filterSelected) {
			if(filterSelected && !DataProxy.IsMultipleKeyFields)
				return GetSelectedKeys();
			List<object> result = new List<object>();
			for(int i = 0; i < DataProxy.VisibleRowCount; i++) {
				if(DataProxy.GetRowType(i) == WebRowType.Group)
					continue;
				if(Grid.Selection.IsRowSelected(i) ^ filterSelected)
					continue;
				result.Add(GetKeyValue(i));
			}
			return result;
		}
		protected List<object> GetSelectedKeys() { 
			return DataProxy.GetSelectedValues(new string [] { Grid.KeyFieldName }); 
		}
		protected object GetKeyValue(int visibleIndex) {
			if(DataProxy.IsMultipleKeyFields)
				return DataProxy.KeyFieldNames.Select(k => DataProxy.GetRowValue(visibleIndex, k)).ToArray();
			return DataProxy.GetRowValue(visibleIndex, Grid.KeyFieldName);
		}
		bool disposed = false;
		public void Dispose() {
			if(this.disposed) return;
			EndExport();
			this.disposed = true;
		}
		void IDisposable.Dispose() { Dispose(); }
		int IWebControlPageSettings.PageSize { get { return 1; } }
		int IWebControlPageSettings.PageIndex { get { return -1; } set { } }
		GridViewPagerMode IWebControlPageSettings.PagerMode { get { return GridViewPagerMode.ShowAllRecords; } }
	}
	public class GridExcelDataPrinter : GridPrinterBase, IGridView<GridXlsExportColumn, GridXlsExportRowBase> {
		public GridExcelDataPrinter(ASPxGridExporterBase exporter)
			: base(exporter, exporter.GridBase) {
			TextBuilder = CreateTextBuilder();
		}
		public GridXlsExportTextBuilder TextBuilder { get; private set; }
		protected virtual GridXlsExportRowBase CreateDataRow(int handle, int dataSourceRowIndex, int level) {
			return new GridXlsExportDataRow(this, handle, dataSourceRowIndex, level);
		}
		protected virtual GridXlsExportColumn CreateColumn(IWebGridDataColumn dataColumn, int index) {
			return new GridXlsExportColumn(this, dataColumn, index);
		}
		protected virtual GridXlsExportSummaryItem CreateSummaryItem(ASPxSummaryItemBase item) {
			return new GridXlsExportSummaryItem(Grid, item);
		}
		protected virtual bool HasGrouping { get { return false; } }
		protected virtual bool ShowFooter { get { return false; } }
		protected virtual bool ShowGroupFooter { get { return false; } }
		protected virtual bool ShowGroupedColumns { get { return false; } }
		protected virtual GridXlsExportTextBuilder CreateTextBuilder() { return new GridXlsExportTextBuilder(Grid, this); }
		public DataController DataController { get { return !IsForceDataSourcePaging ? DataProxy.GetDataController() : null; } }
		public bool IsForceDataSourcePaging { get { return Grid.IsAllowDataSourcePaging(); } }
		public List<GridXlsExportRowBase> Rows { get; private set; }
		public List<GridXlsExportColumn> Columns { get; private set; }
		public override void BeginExport() {
			base.BeginExport();
			Columns = new List<GridXlsExportColumn>();
			Rows = new List<GridXlsExportRowBase>();
			LoadColumns();
			LoadAllRows();
			DataProxy.ExpandAll();
			if(!Columns.OfType<IColumn>().Any(c => c.GroupIndex == -1 && c.IsVisible))
				throw new NotSupportedException("To export grid data, a visible ungrouped DataColumn is required.");
		}
		public override void EndExport() {
			Rows = null;
			Columns = null;
			base.EndExport();
		}
		public int GetVisibleIndex(int rowHandle) {
			if(IsForceDataSourcePaging)
				return rowHandle;
			return DataController.GetVisibleIndex(rowHandle);
		}
		protected virtual void LoadColumns() {
			Columns.AddRange(
				ColumnHelper.Leafs.Select(l => l.Column).OfType<IWebGridDataColumn>().Select((c, i) => CreateColumn(c, i))
			);
		}
		protected virtual void LoadAllRows() {
			Rows.Capacity = DataProxy.VisibleRowCount;
			for(int i = 0; i < DataProxy.VisibleRowCount; i++)
				Rows.Add(CreateDataRow(i, i, 0));
		}
		protected virtual long TotalRowCount { get { return HasGrouping ? DataController.ListSourceRowCount + DataController.GroupRowCount : DataProxy.VisibleRowCount; } }
		long IGridView<GridXlsExportColumn, GridXlsExportRowBase>.RowCount { get { return TotalRowCount; } }
		IEnumerable<GridXlsExportColumn> IGridView<GridXlsExportColumn, GridXlsExportRowBase>.GetAllColumns() { return Columns; }
		IEnumerable<GridXlsExportRowBase> IGridView<GridXlsExportColumn, GridXlsExportRowBase>.GetAllRows() { return Rows; }
		IEnumerable<GridXlsExportColumn> IGridView<GridXlsExportColumn, GridXlsExportRowBase>.GetGroupedColumns() { return new List<GridXlsExportColumn>(); }
		object IGridViewBase<GridXlsExportColumn, GridXlsExportRowBase, GridXlsExportColumn, GridXlsExportRowBase>.GetRowCellValue(GridXlsExportRowBase row, GridXlsExportColumn col) { 
			return TextBuilder.GetCellValue(col as GridXlsExportColumn, GetVisibleIndex(row.LogicalPosition)); 
		}
		FormatSettings IGridViewBase<GridXlsExportColumn, GridXlsExportRowBase, GridXlsExportColumn, GridXlsExportRowBase>.GetRowCellFormatting(GridXlsExportRowBase row, GridXlsExportColumn col) {
			return null;
		}
		string IGridViewBase<GridXlsExportColumn, GridXlsExportRowBase, GridXlsExportColumn, GridXlsExportRowBase>.GetRowCellHyperlink(GridXlsExportRowBase row, GridXlsExportColumn col) {
			if(row != null)
				return TextBuilder.GetNavigateUrl(col as GridXlsExportColumn, GetVisibleIndex(row.LogicalPosition));
			return null;
		}
		IEnumerable<FormatConditionObject> IGridView<GridXlsExportColumn, GridXlsExportRowBase>.FormatConditionsCollection { get { return new FormatConditionObject[0]; } }
		IEnumerable<IFormatRuleBase> IGridView<GridXlsExportColumn, GridXlsExportRowBase>.FormatRulesCollection {
			get { return Grid.FormatConditions.Select(c => new GridExportCondition(this, c)); }
		}
		bool IGridView<GridXlsExportColumn, GridXlsExportRowBase>.ShowFooter { get { return ShowFooter; } }
		bool IGridView<GridXlsExportColumn, GridXlsExportRowBase>.ShowGroupFooter { get { return ShowGroupFooter; } }
		bool IGridView<GridXlsExportColumn, GridXlsExportRowBase>.ShowGroupedColumns { get { return ShowGroupedColumns; } }
		IAdditionalSheetInfo IGridView<GridXlsExportColumn, GridXlsExportRowBase>.AdditionalSheetInfo { get { return new AdditionalSheetInfoWrapper(); } }
		string IGridView<GridXlsExportColumn, GridXlsExportRowBase>.GetViewCaption { get { return !string.IsNullOrEmpty(Grid.SettingsText.Title) ? Grid.SettingsText.Title : Grid.Caption; } }
		string IGridView<GridXlsExportColumn, GridXlsExportRowBase>.FilterString { get { return string.Empty; } }
		XlCellFormatting IGridView<GridXlsExportColumn, GridXlsExportRowBase>.AppearanceGroupRow { get { return null; } }
		XlCellFormatting IGridView<GridXlsExportColumn, GridXlsExportRowBase>.AppearanceEvenRow { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<GridXlsExportColumn, GridXlsExportRowBase>.AppearanceOddRow { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceGroupFooter { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceHeader { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceFooter { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceRow { get { return new XlCellFormatting(); } }
		public IGridViewAppearancePrint AppearancePrint { get { return null; } }
		bool IGridView<GridXlsExportColumn, GridXlsExportRowBase>.ReadOnly { get { return false; } } 
		int IGridView<GridXlsExportColumn, GridXlsExportRowBase>.RowHeight { get { return 20; } } 
		int IGridView<GridXlsExportColumn, GridXlsExportRowBase>.FixedRowsCount { get { return 1; } }
		bool IGridView<GridXlsExportColumn, GridXlsExportRowBase>.IsCancelPending { get { return false; } }
		IEnumerable<ISummaryItemEx> IGridView<GridXlsExportColumn, GridXlsExportRowBase>.GridGroupSummaryItemCollection { get { return CreateSummaryItems(DataProxy.GroupSummary); } }
		IEnumerable<ISummaryItemEx> IGridView<GridXlsExportColumn, GridXlsExportRowBase>.GridTotalSummaryItemCollection { get { return CreateSummaryItems(DataProxy.TotalSummary); } }
		IEnumerable<ISummaryItemEx> IGridView<GridXlsExportColumn, GridXlsExportRowBase>.GroupHeaderSummaryItems { get { return new List<ISummaryItemEx>(); } }
		IEnumerable<ISummaryItemEx> IGridView<GridXlsExportColumn, GridXlsExportRowBase>.FixedSummaryItems { get { return new List<ISummaryItemEx>(); } }
		IEnumerable<ISummaryItemEx> CreateSummaryItems(IList items) {
			return items.OfType<ASPxSummaryItemBase>().Select(i => CreateSummaryItem(i)).ToList();
		}
		bool IGridViewBase<GridXlsExportColumn, GridXlsExportRowBase, GridXlsExportColumn, GridXlsExportRowBase>.GetAllowMerge(GridXlsExportColumn col) { return false; }
		int IGridViewBase<GridXlsExportColumn, GridXlsExportRowBase, GridXlsExportColumn, GridXlsExportRowBase>.RaiseMergeEvent(int startRow, int rowLogicalPosition, GridXlsExportColumn col) { return -1; }
		public static XlCellFormatting ConvertStyle(GridExportAppearanceBase style) {
			var result = new XlCellFormatting();
			GetFill(style, result);
			result.Alignment = new XlCellAlignment {
				HorizontalAlignment = Convert(style.HorizontalAlign),
				VerticalAlignment = Convert(style.VerticalAlign)
			};
			GetBorders(style, result);
			result.Font = new XlFont {
				Bold = style.Font.Bold,
				Italic = style.Font.Italic,
				StrikeThrough = style.Font.Strikeout,
				Underline = style.Font.Underline ? XlUnderlineType.Single : XlUnderlineType.None,
				Name = string.IsNullOrEmpty(style.Font.Name) ? "Calibri" : style.Font.Name
			};
			return result;
		}
		static void GetFill(GridExportAppearanceBase appearance, XlCellFormatting result) {
			var backColor = appearance.BackColor;
			if(!Equals(backColor, Color.Empty)) {
				result.Fill = new XlFill {
					BackColor = appearance.BackColor,
					ForeColor = appearance.BackColor,
					PatternType = XlPatternType.Solid
				};
			}
		}
		static void GetBorders(GridExportAppearanceBase appearance, XlCellFormatting result) {
			Color borderColor = appearance.BorderColor;
			if(!Equals(borderColor, Color.Empty)) {
				result.Border = new XlBorder {
					BottomLineStyle = XlBorderLineStyle.Thin,
					BottomColor = borderColor,
					LeftLineStyle = XlBorderLineStyle.Thin,
					LeftColor = borderColor,
					RightLineStyle = XlBorderLineStyle.Thin,
					RightColor = borderColor,
				};
			}
		}
		static XlHorizontalAlignment Convert(HorizontalAlign align) {
			switch(align) {
				case HorizontalAlign.Center:
					return XlHorizontalAlignment.Center;
				case HorizontalAlign.Justify:
					return XlHorizontalAlignment.Justify;
				case HorizontalAlign.Left:
					return XlHorizontalAlignment.Left;
				case HorizontalAlign.Right:
					return XlHorizontalAlignment.Right;
			}
			return XlHorizontalAlignment.General;
		}
		static XlVerticalAlignment Convert(VerticalAlign align) {
			switch(align) {
				case VerticalAlign.Bottom:
					return XlVerticalAlignment.Bottom;
				case VerticalAlign.Middle:
					return XlVerticalAlignment.Center;
				case VerticalAlign.Top:
					return XlVerticalAlignment.Top;
			}
			return XlVerticalAlignment.Bottom;
		}
	}
	public class GridXlsExportColumn : IColumn {
		public GridXlsExportColumn(GridExcelDataPrinter printer, IWebGridDataColumn column, int index) {
			DataColumn = column;
			Index = index;
			LoadDataValidationItems();
			CellFormating = GridExcelDataPrinter.ConvertStyle(DataColumn.ExportCellStyle);
		}
		protected virtual void LoadDataValidationItems() {
			var prop = ColumnEdit as ComboBoxProperties;
			if(prop == null) return;
			DataValidationItems = new List<object>(prop.Items.Count);
			foreach(ListEditItem item in prop.Items)
				DataValidationItems.Add(item.Text);
		}
		public IWebGridDataColumn DataColumn { get; private set; }
		public int Index { get; private set; }
		protected ASPxGridBase Grid { get { return DataColumn.Adapter.Grid; } }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		public EditPropertiesBase ColumnEdit { get { return DataColumn.Adapter.ExportPropertiesEdit; } }
		FormatSettings IColumn.FormatSettings {
			get {
				return new FormatSettings { ActualDataType = DataColumn.Adapter.DataType, FormatString = GetFormatString(), FormatType = Utils.FormatType.None };
			}
		}
		protected List<object> DataValidationItems { get; private set; }
		protected XlCellFormatting CellFormating { get; private set; }
		string IColumn.Name { get { return DataColumn.Name; } }
		string IColumn.FieldName { get { return DataColumn.FieldName; } }
		string IColumn.Header { get { return DataColumn.ToString(); } }
		string IColumn.HyperlinkEditorCaption { get { return string.Empty; } }
		string IColumn.HyperlinkTextFormatString { get { return GetFormatString(); } }
		string IColumn.GetGroupColumnHeader() {
			return null;
		}
		IUnboundInfo IColumn.UnboundInfo { get { return null; } }
		bool IColumn.IsVisible { get { return DataColumn.Visible; } }
		bool IColumn.IsFixedLeft { get { return IsFixedLeft(DataColumn); } }
		public ISparklineInfo SparklineInfo { get { return null; } }
		int IColumn.LogicalPosition { get { return Index; } }
		int IColumn.Width { get { return GetColumnWidth(); } }
		int IColumn.GroupIndex { get { return DataColumn.Adapter.GroupIndex; } }
		ColumnSortOrder IColumn.SortOrder {
			get { return ColumnSortOrder.None; }
		}
		int IColumn.VisibleIndex { get { return DataColumn.VisibleIndex; } }
		ColumnEditTypes IColumn.ColEditType { get { return ResolveColumnType(ColumnEdit); } }
		bool IColumn.IsGroupColumn { get { return false; } }
		IEnumerable<IColumn> IColumn.GetAllColumns() { return null; }
		bool IColumn.IsCollapsed { get { return false; } }
		int IColumn.GetColumnGroupLevel() { return 0; }
		IEnumerable<object> IColumn.DataValidationItems { get { return DataValidationItems; } }
		XlCellFormatting IColumn.Appearance { get { return CellFormating; } }
		XlCellFormatting IColumn.AppearanceHeader { get { return CellFormating; } }
		protected virtual int GetColumnWidth() { return 200; }
		protected virtual bool IsFixedLeft(IWebGridDataColumn column) { return false; }
		protected string GetFormatString() {
			var formatString = ColumnEdit != null ? ColumnEdit.DisplayFormatString : string.Empty;
			var hlProp = ColumnEdit as HyperLinkProperties;
			return hlProp != null ? hlProp.TextFormatString : formatString;
		}
		static ColumnEditTypes ResolveColumnType(EditPropertiesBase prop) {
			if(prop is ComboBoxProperties) return ColumnEditTypes.Lookup;
			if(prop is ProgressBarProperties) return ColumnEditTypes.ProgressBar;
			if(prop is BinaryImageEditProperties) return ColumnEditTypes.Image;
			if(prop is HyperLinkProperties) return ColumnEditTypes.Hyperlink;
			return ColumnEditTypes.Text;
		}
	}
	public abstract class GridXlsExportRowBase : IRowBase {
		public GridXlsExportRowBase(GridExcelDataPrinter printer, int handle, int dataSourceRowIndex, int level) {
			Printer = printer;
			Handle = handle;
			DataSourceRowIndex = dataSourceRowIndex;
			Level = level;
		}
		public int Handle { get; private set; }
		public int DataSourceRowIndex { get; private set; }
		public int Level { get; private set; }
		protected GridExcelDataPrinter Printer { get; private set; }
		protected abstract bool IsGroupRow { get; }
		public FormatSettings FormatSettings { get { return null; } }
		int IRowBase.GetRowLevel() { return Level; }
		public int LogicalPosition { get { return Handle; } }
		bool IRowBase.IsGroupRow { get { return IsGroupRow; } }
		bool IRowBase.IsDataAreaRow { get { return true; } }
		public string FormatString { get { return null; } }
	}
	public class GridXlsExportDataRow : GridXlsExportRowBase, IRowBase {
		public GridXlsExportDataRow(GridExcelDataPrinter exporter, int handle, int dataSourceRowIndex, int level)
			: base(exporter, handle, dataSourceRowIndex, level) {
		}
		protected override bool IsGroupRow { get { return false; } }
	}
	public class GridXlsExportSummaryItem : ISummaryItemEx {
		public GridXlsExportSummaryItem(ASPxGridBase grid, ASPxSummaryItemBase item) {
			Grid = grid;
			Item = item;
		}
		public ASPxGridBase Grid { get; private set; }
		public ASPxSummaryItemBase Item { get; private set; }
		protected virtual string ShowInColumnFooterName { get { return Item.FieldName; } }
		object ISummaryItemEx.GetSummaryValueByGroupId(int groupId) { return null; }
		string ISummaryItemEx.ShowInColumnFooterName { get { return ShowInColumnFooterName; } }
		object ISummaryItemEx.SummaryValue { get { return Grid.DataProxy.GetTotalSummaryValue(Item); } }
		string ISummaryItem.DisplayFormat { get { return Item.ResolveDisplayFormat(Item.DisplayFormat); } set { } }
		string ISummaryItem.FieldName { get { return Item.FieldName; } }
		SummaryItemType ISummaryItem.SummaryType { get { return Item.SummaryType; } }
	}
	public class GridXlsExportTextBuilder : GridTextBuilder {
		public GridXlsExportTextBuilder(ASPxGridBase grid, GridExcelDataPrinter printer)
			: base(grid) {
			Printer = printer;
		}
		public GridExcelDataPrinter Printer { get; private set; }
		public object GetCellValue(GridXlsExportColumn column, int visibleIndex) {
			var dbValue = DataProxy.GetRowValue(visibleIndex, column.DataColumn.FieldName);
			var exportValue = column.ColumnEdit.GetExportValue(GetDisplayConrolArgs(column.DataColumn, visibleIndex));
			if(column.ColumnEdit is AutoCompleteBoxPropertiesBase)
				return GetRowDisplayText(column.DataColumn, visibleIndex);
			if(column.ColumnEdit is CheckBoxProperties)
				return dbValue;
			var hyperlinkProp = column.ColumnEdit as HyperLinkProperties;
			if(hyperlinkProp != null) {
				if(!string.IsNullOrEmpty(hyperlinkProp.TextField))
					return DataProxy.GetRowValue(visibleIndex, hyperlinkProp.TextField);
				if(!string.IsNullOrEmpty(hyperlinkProp.Text))
					return hyperlinkProp.Text;
				return dbValue;
			}
			return exportValue;
		}
		public string GetNavigateUrl(GridXlsExportColumn column, int visibleIndex) {
			if((column as IColumn).ColEditType != ColumnEditTypes.Hyperlink)
				return null;
			return column.DataColumn.Adapter.ExportPropertiesEdit.GetExportNavigateUrl(GetDisplayConrolArgs(column.DataColumn, visibleIndex));
		}
		protected internal override CreateDisplayControlArgs GetDisplayControlArgsCore(IWebGridDataColumn column, int visibleIndex, IValueProvider provider, ASPxGridBase grid, object value) {
			CreateDisplayControlArgs controlArgs = base.GetDisplayControlArgsCore(column, visibleIndex, provider, grid, value);
			controlArgs.EncodeHtml = false;
			return controlArgs;
		}
		protected CreateDisplayControlArgs GetDisplayConrolArgs(IWebGridDataColumn column, int visibleIndex) {
			return GetDisplayControlArgsCore(column, visibleIndex, DataProxy.GetRowValue(visibleIndex, column.FieldName));
		}
		protected override string GetEditorDisplayTextCore(EditPropertiesBase editor, IWebGridDataColumn column, int visibleIndex, object value, bool encodeValue = false) {
			return editor.GetExportDisplayText(GetDisplayControlArgsCore(column, visibleIndex, value));
		}
		protected override ASPxGridSummaryDisplayTextEventArgs GetSummaryDisplayTextEventArgs(ASPxSummaryItemBase item, object value, string text) {
			return null;
		}
	}
	class AdditionalSheetInfoWrapper : IAdditionalSheetInfo {
		public string Name { get { return "Additional Data"; } }
		public XlSheetVisibleState VisibleState { get { return XlSheetVisibleState.Hidden; } }
	}
}
