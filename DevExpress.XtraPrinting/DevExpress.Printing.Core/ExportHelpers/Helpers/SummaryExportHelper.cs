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
using DevExpress.Data;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Export;
using DevExpress.Utils;
namespace DevExpress.Printing.ExportHelpers {
	public class SummaryExportHelper<TCol, TRow> : ExportHelperBase<TCol, TRow>
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		IEnumerable<ISummaryItemEx> groupSummaryItems;
		int groupFooterRowsCnt;
		bool allowExportGroupSummary;
		public SummaryExportHelper(ExporterInfo<TCol, TRow> exportInfo)
			: base(exportInfo) {
			IgnoreHiddenGroupValues = false;
			this.groupSummaryItems = exportInfo.View.GridGroupSummaryItemCollection;
			this.groupFooterRowsCnt = FooterRowsCount(groupSummaryItems);
			this.allowExportGroupSummary = CheckFooterForValues();
		}
		[DefaultValue(false)]
		public bool IgnoreHiddenGroupValues { get; set; }
		bool AllowExportGroupSummary { get { return allowExportGroupSummary; } }
		bool CheckFooterForValues(){
			foreach(var gridColumn in ExportInfo.GridColumns){
				var item = GetItemByKey(groupSummaryItems, gridColumn.FieldName);
				if(CanExportCurrentColumn(gridColumn) && item != null){
					return CheckGroupSummaryOptions();
				}
			}
			return false;
		}
		bool AllowExportTotalSummary {
			get { return CheckTotalSummaryOptions(); }
		}
		#region Ranges
		List<XlCellRange> GetRangeList(int colind) {
			List<XlCellRange> result = new List<XlCellRange>();
			foreach(var group in ExportInfo.GroupsList) {
				int endRange = group.End;
				if(object.Equals(group, ExportInfo.GroupsList.Last())) endRange--;
				result.Add(new XlCellRange(new XlCellPosition(colind, group.Start), new XlCellPosition(colind, endRange)));
			}
			return result;
		}
		List<XlCellRange> GetFullSheetRange(int columnPosition, int endRangeRow){
			var ranges = new List<XlCellRange>();
			ranges.Add(new XlCellRange(new XlCellPosition(columnPosition, GetStartRangePosition()), new XlCellPosition(columnPosition, endRangeRow - 1)));
			return ranges;
		}
		int GetStartRangePosition(){
			return ExportInfo.Options.ShowColumnHeaders != DefaultBoolean.True ? 0 : 1;
		}
		List<XlCellRange> GetGroupRange(int startRow, int endRow, int columnPosition) {
			var ranges = new List<XlCellRange>{
				new XlCellRange(new XlCellPosition(columnPosition, startRow),
				new XlCellPosition(columnPosition, endRow-1))
			};
			return ranges;
		}
		#endregion
		#region Checkers
		static bool CheckItemFormatString(FormatStringParser formatStringEx) {
			return !string.IsNullOrEmpty(formatStringEx.Prefix) || !string.IsNullOrEmpty(formatStringEx.Postfix);
		}
		bool CheckGroupSummaryOptions() {
			return ExportInfo.View.GridGroupSummaryItemCollection != null && ExportInfo.View.ShowGroupFooter;
		}
		bool CheckTotalSummaryOptions() {
			return ExportInfo.View.GridTotalSummaryItemCollection != null && ExportInfo.View.ShowFooter;
		}
		#endregion
		#region HelperMethods
		int FooterRowsCount(IEnumerable<ISummaryItemEx> collection) {
			var summaryItems = collection.ToList();
			int count = 0, maxcount = 0;
			foreach(var gridColumn in ExportInfo.GridColumns) {
				while(GetItemByKey(summaryItems, gridColumn.FieldName) != null) {
					count++;
					summaryItems.Remove(GetItemByKey(summaryItems, gridColumn.FieldName));
				}
				if(count > maxcount) { maxcount = count; count = 0; } else count = 0;
			}
			return maxcount;
		}
		ISummaryItemEx GetItemByKey(IEnumerable<ISummaryItemEx> items, string fieldName) {
			return string.IsNullOrEmpty(fieldName) ? null : items.FirstOrDefault(item => item.ShowInColumnFooterName == fieldName);
		}
		static XlSummary ConvertSummaryItemTypeToExcel(SummaryItemType sit) {
			switch(sit) {
				case SummaryItemType.Average: return XlSummary.Average;
				case SummaryItemType.Count: return XlSummary.CountA;
				case SummaryItemType.Max: return XlSummary.Max;
				case SummaryItemType.Min: return XlSummary.Min;
				case SummaryItemType.Sum: return XlSummary.Sum;
				default: return 0;
			}
		}
		#endregion
		void SummaryExportCore(IEnumerable<ISummaryItemEx> summaryCollection, int numOfRows, Action<ISummaryItemEx, TCol, IXlCell> action) {
			var tempCollection = summaryCollection.ToList();
			for(int ind = 0; ind < numOfRows; ind++) {
				ExportInfo.Exporter.BeginRow();
				foreach(var gridColumn in ExportInfo.GridColumns) {
					if(CanExportCurrentColumn(gridColumn)) {
						IXlCell cell = ExportInfo.Exporter.BeginCell();
						var item = GetItemByKey(tempCollection, gridColumn.FieldName);
						if(item != null) action(item, gridColumn, cell);
						tempCollection.Remove(item);
						ExportInfo.Exporter.EndCell();
					}
				}
				ExportInfo.Exporter.EndRow();
			}
		}
		public void ExportGroupSummary(int startRow, int endRow, int groupId) {
			if(AllowExportGroupSummary) {
				ExportInfo.ExportRowIndex += groupFooterRowsCnt;
				SummaryExportCore(groupSummaryItems, groupFooterRowsCnt, (item, gridColumn, cell) => {
					object summaryValue = item.GetSummaryValueByGroupId(groupId);
					var summaryType = ConvertSummaryItemTypeToExcel(item.SummaryType);
					if(ExportInfo.Options.CanRaiseCustomizeCellEvent)
						OnCustomizeFooterCell(SheetAreaType.GroupFooter, startRow, endRow, item, summaryValue, gridColumn, cell);
					else
						ExportItem(SheetAreaType.GroupFooter, startRow, endRow, summaryValue, item.FieldName, item.DisplayFormat, summaryType, gridColumn, cell, false);
				});
			}
		}
		public void ExportTotalSummary(int endExportIndex) {
			if(AllowExportTotalSummary && endExportIndex != GetStartRangePosition()) {
				var totalSummaryItems = ExportInfo.View.GridTotalSummaryItemCollection;
				int totalFooterRowsCnt = FooterRowsCount(totalSummaryItems);
				SummaryExportCore(totalSummaryItems, totalFooterRowsCnt, (item, gridColumn, cell) =>{
					object summaryValue = item.SummaryValue;
					var summaryType = ConvertSummaryItemTypeToExcel(item.SummaryType);
					if(ExportInfo.Options.CanRaiseCustomizeCellEvent)
						OnCustomizeFooterCell(SheetAreaType.TotalFooter, 0, endExportIndex, item, summaryValue, gridColumn, cell);
					else
						ExportItem(SheetAreaType.TotalFooter, 0, endExportIndex, summaryValue, item.FieldName, item.DisplayFormat, summaryType, gridColumn, cell, false);
				});
			}
		}
		void ExportCustomSummaryValue(IXlCell cell, object displayValue, string itemDisplayFormat, string itemFieldName) {
			FormatStringParser formatStringEx = new FormatStringParser(itemDisplayFormat, itemFieldName);
			object value = string.IsNullOrEmpty(formatStringEx.Prefix) ? displayValue : formatStringEx.Prefix + displayValue;
			cell.Value = XlVariantValue.FromObject(value);
			FormatExport.SetCellFormatting(cell.Formatting, ExportInfo.Options, itemDisplayFormat);
		}
		void ExportItem(SheetAreaType areaType, int startRow, int endRow, object summaryValue,string itemFieldName, string itemDisplayFormat,XlSummary summaryType, TCol gridColumn, IXlCell cell,bool eventHandled) {
			var ranges = new List<XlCellRange>();
			int columnIndex = GetColumnPosition(itemFieldName);
			if(columnIndex == -3) columnIndex = gridColumn.LogicalPosition;
			var sourceColumn = ExportInfo.ColumnsInfoColl[itemFieldName] ?? gridColumn;
			ranges = CalcSummaryRange(areaType, startRow, endRow, columnIndex, summaryType);
			if(SetLiveSummaries(summaryType, columnIndex, sourceColumn)){
				ExportSummaryItem(itemFieldName, itemDisplayFormat, summaryType, cell, sourceColumn, ranges, eventHandled);
			} else ExportCustomSummaryValue(cell, summaryValue, itemDisplayFormat, itemFieldName);
		}
		static bool SetLiveSummaries(XlSummary summaryType, int columnIndex, IColumn sorceColumn) {
			bool textSummaryCondition = sorceColumn != null &&
				 !(sorceColumn.FormatSettings.ActualDataType == typeof(string) && summaryType != XlSummary.CountA);
			return summaryType != 0 && columnIndex != -1 && textSummaryCondition;
		}
		int GetColumnPosition(string fieldName){
			if(string.IsNullOrEmpty(fieldName)) return -3;
			TCol col = ExportInfo.ColumnsInfoColl[fieldName];
			var position = col != null ? col.LogicalPosition : -1;
			return position;
		}
		void OnCustomizeFooterCell(SheetAreaType areaType, int startRow, int endRow, ISummaryItemEx item,object summaryValue, TCol gridColumn, IXlCell cell){
			CellObject exportedCell = GetExportedCellObject(gridColumn);
			ExportSummaryItem exportitem = GetExportedItem(item);
			var ea = ExportInfo.CreateEventArgs(areaType, exportedCell, gridColumn, null, exportitem);
			ExportInfo.Options.RaiseCustomizeCellEvent(ea);
			bool handled = ea.Handled && ea.AreaType == areaType;
			if(handled){
				cell.Value = XlVariantValue.FromObject(ea.Value);
				cell.Formatting = ea.Formatting.ConvertWith(ExportInfo.Options);
			}
			if(cell.Value.IsEmpty){
				ExportItem(areaType, startRow, endRow, summaryValue,exportitem.FieldName,exportitem.DisplayFormat, exportitem.SummaryType, gridColumn, cell,handled);
			}
		}
		static CellObject GetExportedCellObject(TCol gridColumn){
			var format = new XlFormattingObject();
			format.CopyFrom(gridColumn.Appearance, FormatType.Custom);
			return new CellObject{ Formatting = format };
		}
		ExportSummaryItem GetExportedItem(ISummaryItemEx item) {
			ExportSummaryItem exportitem = new ExportSummaryItem();
			exportitem.DisplayFormat = item.DisplayFormat;
			exportitem.FieldName = item.FieldName;
			exportitem.SummaryType = ConvertSummaryItemTypeToExcel(item.SummaryType);
			return exportitem;
		}
		List<XlCellRange> CalcSummaryRange(SheetAreaType areaType, int startRow, int endRow,int colPosition,XlSummary summaryType){
			var ranges = new List<XlCellRange>();
			if(areaType == SheetAreaType.GroupFooter)
				ranges = GetGroupRange(startRow, endRow, colPosition);
			if(areaType == SheetAreaType.TotalFooter){
				if(summaryType == XlSummary.CountA && colPosition == 0) ranges = GetRangeList(colPosition);
				else ranges = GetFullSheetRange(colPosition, endRow);
			}
			return ranges;
		}
		void ExportSummaryItem(string itemFieldName,string itemDisplayFormat, XlSummary summaryType, IXlCell cell, TCol gridColumn, List<XlCellRange> ranges,bool eventHandled){
			FormatStringParser formatStringEx = new FormatStringParser(itemDisplayFormat, itemFieldName);
			TCol appearanceColumn = ExportInfo.ColumnsInfoColl[itemFieldName];
			if(!eventHandled){
				FormatExport.SetCellFormatting(cell.Formatting, appearanceColumn, ExportInfo.Options, !ExportInfo.RawDataMode);
			}
			IXlFormulaEngine formulaEngine = ExportInfo.Exporter.FormulaEngine;
			if(CheckItemFormatString(formatStringEx)) {
				bool isDateTime = cell.Formatting != null && cell.Formatting.IsDateTimeFormatString;
				var paramPrefix = formulaEngine.Param(formatStringEx.Prefix);
				var paramText = formulaEngine.Text(formulaEngine.Subtotal(ranges, summaryType, IgnoreHiddenGroupValues), formatStringEx.ValueFormat, isDateTime);
				var paramPostfix = formulaEngine.Param(formatStringEx.Postfix);
				var concatenateParam = formulaEngine.Concatenate(paramPrefix, paramText, paramPostfix);
				cell.SetFormula(concatenateParam);
			} else {
				cell.SetFormula(formulaEngine.Subtotal(ranges, summaryType, IgnoreHiddenGroupValues));
				if(!eventHandled){
					string columnFormatString = gridColumn.FormatSettings.FormatString;
					string formatstring = !string.IsNullOrEmpty(formatStringEx.UnionString) ? formatStringEx.UnionString : columnFormatString;
					FormatExport.SetCellFormatting(cell.Formatting, ExportInfo.Options, formatstring, gridColumn, !ExportInfo.RawDataMode, summaryType != XlSummary.CountA);
				}
			}
		}
	}
}
