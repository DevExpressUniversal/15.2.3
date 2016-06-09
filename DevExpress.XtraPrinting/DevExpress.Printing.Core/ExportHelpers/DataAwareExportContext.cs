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
using System.Drawing;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Printing.ExportHelpers {
	public class CellObject {
		public object Value { get; set; }
		public XlFormattingObject Formatting { get; set; }
		public string Hyperlink { get; set; }
	}
	public class ExportSummaryItem {
		public XlSummary SummaryType { get; set; }
		public string FieldName { get; set; }
		public string DisplayFormat { get; set; }
	}
	public interface IExportContext{
		void AddRow();
		void AddRow(object[] values);
		void AddRow(CellObject[] values);
		void MergeCells(XlCellRange range);
		void InsertImage(Image image, XlCellRange range);
	}
	public interface ISheetHeaderFooterExportContext : IExportContext {
		void InsertImage(Image image, Size s);
	}
	public interface ISheetCustomizationContext {
		void SetFixedHeader(int documentRow);
		void AddAutoFilter(XlCellRange range);
	}
	internal class ClipboardDataAwareExportContext<TCol, TRow> : DataAwareExportContext<TCol, TRow>
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		public ClipboardDataAwareExportContext(ExporterInfo<TCol, TRow> exportInfo) :
			base(exportInfo) { }
		protected override void CombineFormatSettings(TRow gridRow, TCol col, IXlCell cell) {
			if(exportInfo.View is IClipboardGridView<TCol, TRow>) {
				XlCellFormatting formatting = (exportInfo.View as IClipboardGridView<TCol, TRow>).GetCellAppearance(gridRow, col);
				cell.Formatting = formatting;
			}
			base.CombineFormatSettings(gridRow, col, cell);
		}
	}
	public partial class DataAwareExportContext<TCol, TRow> 
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		internal ExporterInfo<TCol, TRow> exportInfo;
		const int MaxGroupingLevel = 8;
		const int DefaultRowHeight = -1;
		public DataAwareExportContext(ExporterInfo<TCol, TRow> exportInfo) {
			this.exportInfo = exportInfo;
		}
		internal int DataCellsRangeTop { get; private set; }
		bool CanExportCurrentColumn(TCol gridColumn) {
			if(exportInfo.View.ShowGroupedColumns) return true;
			return gridColumn.GroupIndex == -1;
		}
		public void AddAutoFilter(){
			if(!exportInfo.Options.CanRaiseCustomizeSheetSettingsEvent)
				AddAutoFilter(0, DataCellsRangeTop - 1, exportInfo.Exporter.CurrentColumnIndex - 1, exportInfo.Exporter.CurrentRowIndex - 1);
		}
		public void PrintTitles() {
			exportInfo.Sheet.PrintTitles.SetRows(0, 0);
		}
		#region Row
		public void AddRow(){
			CreateRowCore(null, null, DefaultRowHeight);
		}
		public void AddRow(Func<TCol, object> value, TRow gridRow){
			CreateRow(value, null, false, gridRow, DefaultRowHeight);
			if(exportInfo.Options.CanRaiseAfterAddRow) exportInfo.RaiseAfterAddRowEvent(gridRow, this);
		}
		void AddRowCore(ICollection<object> values, Action<IXlCell, int> value) {
			if(values == null) return;
			exportInfo.Exporter.BeginRow();
			for(int i = 0; i < values.Count && exportInfo.ComplyWithFormatLimits(i); i++) {
				IXlCell cell = exportInfo.Exporter.BeginCell();
				value(cell, i);
				exportInfo.Exporter.EndCell();
			}
			exportInfo.Exporter.EndRow();
			exportInfo.ExportRowIndex++;
		}
		void CreateRow(Func<TCol, object> value, Action<IXlRow> rowStyleAction, bool emptyRow, TRow gridRow,int height) {
			CreateRowCore(rowStyleAction, gridColumn => {
				if(CanExportCurrentColumn(gridColumn) && !emptyRow)
					CreateCell(value, gridRow, gridColumn);
			},height);
		}
		protected virtual void CreateCell(Func<TCol, object> value, TRow gridRow, TCol gridColumn){
			IXlCell cell = exportInfo.Exporter.BeginCell();
			if (GroupColumnHeaderCell(gridColumn)){
				cell.Value = XlVariantValue.FromObject(gridColumn.GetGroupColumnHeader());
				SetFormatting(cell, null, true);
				RaiseCustomizeDataCell(SheetAreaType.DataArea, cell, gridColumn.GetGroupColumnHeader(), gridColumn, gridRow);
			} else{
				CombineFormatSettings(gridRow, gridColumn, cell);
				SetCellValue(gridColumn, gridRow, cell, value(gridColumn));
			}
			exportInfo.Exporter.EndCell();
		}
		bool GroupColumnHeaderCell(TCol gridColumn){
			return gridColumn.IsGroupColumn && exportInfo.ExportRowIndex == 0;
		}
		void CreateRowCore(Action<IXlRow> rowStyleAction, Action<TCol> action, int height){
			IXlRow row = exportInfo.Exporter.BeginRow();
			row.HeightInPixels = height;
			foreach(TCol gridColumn in exportInfo.GridColumns)
				if(action != null && exportInfo.ComplyWithFormatLimits(gridColumn.LogicalPosition)) action(gridColumn);
			exportInfo.Exporter.EndRow();
			if(rowStyleAction != null) rowStyleAction(row);
			exportInfo.ExportRowIndex++;
		}
		#endregion
		#region Column
		public void CreateColumn(TCol gridColumn) {
			if (!exportInfo.ComplyWithFormatLimits(gridColumn.LogicalPosition)) return;
			IXlColumn column = exportInfo.Exporter.BeginColumn();
			column.Formatting = FormatExport.GetColumnAppearanceFormGridColumn(gridColumn, !exportInfo.RawDataMode);
			FormatExport.PrimaryFormatColumn(gridColumn, column.Formatting);
			FormatExport.SetBorder(column.Formatting, exportInfo.Options);
			SetColumnWidth(gridColumn, column);
			if(exportInfo.Options.AllowFixedColumns == DefaultBoolean.True)
				SetColumnAsFixed(exportInfo.Sheet, gridColumn);
			SetColumnVisibilityState(gridColumn, column);
			exportInfo.Exporter.EndColumn();
		}
		static void SetColumnWidth(TCol col, IXlColumn column) {
			column.WidthInPixels = col.Width;
		}
		static void SetColumnVisibilityState(TCol col, IXlColumn column) {
			column.IsHidden = !col.IsVisible;
		}
		static void SetColumnAsFixed(IXlSheet sheet, TCol column) {
			if(column.IsFixedLeft) sheet.SplitPosition = new XlCellPosition(column.LogicalPosition + 1, sheet.SplitPosition.Row);
		}
		#endregion
		#region Header
		public void CreateHeader() {
			CreateRowCore(null, gridColumn => {
				IXlCell cell = exportInfo.Exporter.BeginCell();
				object value = gridColumn.Header;
				SetFormatting(cell, gridColumn.AppearanceHeader,false);
				RaiseCustomizeDataCell(SheetAreaType.Header, cell, value, gridColumn, null);
				exportInfo.Exporter.EndCell();
			}, DefaultRowHeight);
			if(exportInfo.Options.CanRaiseAfterAddRow) exportInfo.RaiseAfterAddRowEvent(null,this);
			DataCellsRangeTop += exportInfo.ExportRowIndex;
		}
		void SetFormatting(IXlCell cell, XlCellFormatting format,bool setBoldFont){
			if(!exportInfo.RawDataMode) {
				if(format!=null) cell.Formatting = format;
				if(setBoldFont) FormatExport.SetBoldFont(cell.Formatting);
				FormatExport.SetBorder(cell.Formatting, exportInfo.Options);
			}
		}
		public virtual void PrintGroupRowHeader(TRow groupRow){
			if(exportInfo.GroupsStack.Count >= MaxGroupingLevel-1) return;
			IGroupRow<TRow> groupRowCore = groupRow as IGroupRow<TRow>;
			CreateRowCore(row => row.IsCollapsed = groupRowCore.IsCollapsed, gridColumn => {
				IXlCell cell = exportInfo.Exporter.BeginCell();
				string grRowHeader = gridColumn.LogicalPosition == 0 ? groupRowCore.GetGroupRowHeader() : null;
				SetFormatting(cell, exportInfo.View.AppearanceGroupRow, true);
				RaiseCustomizeDataCell(SheetAreaType.GroupHeader, cell, grRowHeader, gridColumn, groupRow);
				exportInfo.Exporter.EndCell();
			}, DefaultRowHeight);
			if(exportInfo.Options.CanRaiseAfterAddRow) exportInfo.RaiseAfterAddRowEvent(groupRow,this);
		}
		public void CreateExportDataGroup(int exportEssenceLevel, int groupId, bool isCollapsed){
			CreateExportDataGroup(exportEssenceLevel, exportInfo.ExportRowIndex,groupId, isCollapsed);
		}
		public void CreateExportDataGroup(int exportEssenceLevel, int startGroup, int groupId, bool isCollapsed){
			if(exportInfo.GroupsStack.Count < MaxGroupingLevel-1){
				XlGroup newExportGroup = new XlGroup{ Group = exportInfo.Exporter.BeginGroup() };
				newExportGroup.Group.IsCollapsed = isCollapsed;
				newExportGroup.Group.OutlineLevel = exportEssenceLevel + 1;
				newExportGroup.StartGroup = startGroup;
				newExportGroup.GroupId = groupId;
				exportInfo.GroupsStack.Push(newExportGroup);
			}
		}
		public void SetExportSheetSettings(){
			exportInfo.Sheet.ViewOptions.ShowGridLines = exportInfo.ShowGridLines;
			exportInfo.Sheet.HeaderFooter.ScaleWithDoc = true;
			string viewCaption = exportInfo.View.GetViewCaption;
			exportInfo.Sheet.HeaderFooter.EvenFooter = viewCaption;
			exportInfo.Sheet.HeaderFooter.FirstHeader = viewCaption;
			exportInfo.Sheet.HeaderFooter.AlignWithMargins = true;
			if(!exportInfo.Sheet.HeaderFooter.DifferentOddEven)
				exportInfo.Sheet.HeaderFooter.OddHeader = viewCaption;
			if (exportInfo.Options.CanRaiseCustomizeSheetSettingsEvent)
				exportInfo.RaiseCustomizeSheetSettingsEvent(this);
			if (exportInfo.Options.CanRaiseCustomizeHeaderEvent)
				exportInfo.OnContextCustomizationEvent(EventType.Header, this);
		}
		public void CustomizeSheetSettings(){
			if(!exportInfo.Options.CanRaiseCustomizeSheetSettingsEvent){
				int offset = 0;
				if(exportInfo.View.FixedRowsCount > 1 && exportInfo.Options.ShowColumnHeaders == DefaultBoolean.True)
					offset = 1;
				if(!exportInfo.Options.CanRaiseCustomizeHeaderEvent)
					SetFixedHeader((exportInfo.View.FixedRowsCount + offset) - 1);
			}
		}
		#endregion
		#region Footer
		public void CreateFooter() {
			if(exportInfo.Options.ShowTotalSummaries == DefaultBoolean.True)
				exportInfo.Helper.SummaryExporter.ExportTotalSummary(exportInfo.ExportRowIndex);
			if(exportInfo.Options.CanRaiseCustomizeFooterEvent)
				exportInfo.OnContextCustomizationEvent(EventType.Footer, this);
		}
		public void SetSummary(int startGroup, int endExcelGroupIndex, int groupId) {
			if(exportInfo.Options.ShowGroupSummaries == DefaultBoolean.True){
				exportInfo.Helper.SummaryExporter.ExportGroupSummary(startGroup, endExcelGroupIndex, groupId);
			}
		}
		#endregion
		#region CellProcessing
		protected virtual void CombineFormatSettings(TRow gridRow, TCol col, IXlCell cell) {
			var cellFmtSettings = exportInfo.View.GetRowCellFormatting(gridRow, col);
			bool hasRowSetings = CheckSettings(gridRow.FormatSettings);
			bool hasColSettings = CheckSettings(col.FormatSettings);
			bool hasCellSettings = CheckSettings(cellFmtSettings);
			if (hasCellSettings) { cell.Formatting.GetActual(cellFmtSettings); return; }
			if (hasRowSetings) { cell.Formatting.GetActual(gridRow.FormatSettings); return; }
			cell.Formatting.GetActual(col.FormatSettings);
		}
		bool CheckSettings(FormatSettings settings){
			return settings != null && (settings.ActualDataType != null || !string.IsNullOrEmpty(settings.FormatString));
		}
		void RaiseCustomizeDataCell(SheetAreaType areaType, IXlCell cell, object cellValue, TCol gridColumn, TRow gridRow) {
			ColumnEditTypes columnEditType = gridColumn.ColEditType;
			string hyperlink = GetActualHyperlinkValue(cellValue, gridRow, gridColumn);
			bool isHandled = false;
			if(exportInfo.Options.CanRaiseCustomizeCellEvent)
				cellValue = exportInfo.RaiseCustomizeCellEvent(areaType, cell, cellValue, gridColumn, gridRow, ref hyperlink, ref isHandled);
			cell.Value = XlVariantValue.FromObject(cellValue);
			if(AllowExportCutomTypes(cell, cellValue, columnEditType))
				cell.Value = XlVariantValue.FromObject(cellValue.ToString());
			if(CheckOptions(areaType, isHandled, hyperlink, columnEditType)){
				string format = gridColumn.HyperlinkTextFormatString;
				HyperlinkExporter.SetHyperlink(hyperlink, format, cell, exportInfo.Sheet);
			}
		}
		bool CheckOptions(SheetAreaType areaType, bool isHandled, string hyperlink, ColumnEditTypes columnEditType){
			return HyperlinkExporter.CanExportHyperlink(isHandled, hyperlink, columnEditType,
				   exportInfo.Options.AllowHyperLinks) && !exportInfo.RawDataMode && areaType == SheetAreaType.DataArea;
		}
		protected void SetCellValue(TCol gridColumn, TRow gridRow, IXlCell cell, object callBackValue) {
			RaiseCustomizeDataCell(SheetAreaType.DataArea, cell, callBackValue, gridColumn, gridRow);
			if(exportInfo.Options.AllowCellMerge == DefaultBoolean.True)
				exportInfo.Helper.CellMerger.ProcessVerticalMerging(exportInfo.ExportRowIndex, gridColumn, exportInfo.Exporter.CurrentColumnIndex, cell);
		}
		string GetActualHyperlinkValue(object cellValue, TRow row,TCol col) {
			string hyperlink = string.Empty;
			if(col.ColEditType == ColumnEditTypes.Hyperlink){
				if(string.IsNullOrEmpty(col.HyperlinkEditorCaption)){
					string cellLink = exportInfo.View.GetRowCellHyperlink(row, col);
					if(string.IsNullOrEmpty(cellLink) && cellValue != null){
						cellLink = cellValue.ToString();
					}
					hyperlink = cellLink;
				} else hyperlink = col.HyperlinkEditorCaption;
			}
			return hyperlink;
		}
		static bool AllowExportCutomTypes(IXlCell cell, object cellValue, ColumnEditTypes columnEditType){
			return cell.Value == XlVariantValue.Empty && cellValue != null && columnEditType != ColumnEditTypes.Image && columnEditType != ColumnEditTypes.Sparkline;
		}
		#endregion
	}
	public partial class DataAwareExportContext<TCol, TRow> : ISheetHeaderFooterExportContext, ISheetCustomizationContext {
		public void MergeCells(XlCellRange range){
			exportInfo.Helper.CellMerger.Merge(range);
		}
		#region Picture
		public void InsertImage(Image image, Size s){
			int widthInPixels = s.Width;
			int heightInPixels = s.Height;
			CreateRow(gridColumn => null, xlrow => SetPictureCore(image, 0, exportInfo.ExportRowIndex, widthInPixels, heightInPixels, XlAnchorType.OneCell), true, null, heightInPixels);
		}
		public void InsertImage(Image image, XlCellRange range){
			SetPictureCore(image, range.TopLeft.Column, range.TopLeft.Row, range.BottomRight.Column + 1, range.BottomRight.Row + 1, XlAnchorType.TwoCell);
		}
		void SetPictureCore(Image picture, int column, int row, int widthInPixels, int heightInPixels, XlAnchorType type){
			if(picture == null) return;
			PictureExporter<TCol, TRow> pExporter = new PictureExporter<TCol, TRow>(exportInfo);
			pExporter.SetPicture(picture, column, row, widthInPixels, heightInPixels, type);
		}
		#endregion
		public void SetFixedHeader(int bottomRowIndex){
			exportInfo.Sheet.SplitPosition = new XlCellPosition(exportInfo.Sheet.SplitPosition.Column, bottomRowIndex + 1);
		}
		public void AddAutoFilter(XlCellRange range){
			AddAutoFilter(range.TopLeft.Column, range.TopLeft.Row, range.BottomRight.Column, range.BottomRight.Row);
		}
		void AddAutoFilter(int left, int top, int right, int bottom){
			if(exportInfo.Exporter.CurrentColumnIndex != 0)
				exportInfo.Sheet.AutoFilterRange = XlCellRange.FromLTRB(left, top, right, bottom);
		}
		public void AddRow(object[] values){
			AddRowCore(values, (cell, i) =>{
				var customValue = i < values.Length ? values[i] : null;
				cell.Value = XlVariantValue.FromObject(customValue);
				cell.Formatting = (new XlFormattingObject()).ConvertWith(exportInfo.Options);
			});
		}
		public void AddRow(CellObject[] values){
			AddRowCore(values, (cell, i) =>{
				var customValue = i < values.Length ? values[i] : null;
				if(customValue != null){
					cell.Value = XlVariantValue.FromObject(customValue.Value);
					cell.Formatting = customValue.Formatting.ConvertWith(exportInfo.Options);
					if(!string.IsNullOrEmpty(customValue.Hyperlink))
						HyperlinkExporter.SetHyperlink(customValue.Hyperlink, string.Empty, cell, exportInfo.Sheet);
				}
			});
		}
	}
}
