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

using DevExpress.XtraExport;
using DevExpress.XtraExport.Helpers;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Export.Xl;
namespace DevExpress.Printing.ExportHelpers {
	internal class LookUpValuesExporter<TCol, TRow> : ExportHelperBase<TCol, TRow> 
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		public LookUpValuesExporter(ExporterInfo<TCol, TRow> exportInfo) : base(exportInfo){
			AuxiliarySheetRowsCount = AuxiliarySheetRows(exportInfo.ColumnsInfoColl);
		}
		internal static int AuxiliarySheetRowsCount { get; private set; }
		int AuxiliarySheetRows(ExportColumnsCollection<TCol> gridColumns) {
			int max = 0;
			foreach(var gridColumn in gridColumns) {
				List<object> columnItemsColl = gridColumns.ColumnDvItemsByFieldName(gridColumn.FieldName);
				if(columnItemsColl!=null) {
					int count = columnItemsColl.Count();
					if(count > max) max = count;
				}
			}
			return max;
		}
		public void ProcessDataValidation(string auxiliarySheetName) {
			if(ExportInfo.GroupsList.Count != 0) {
				foreach(TCol gridColumn in ExportInfo.GridColumns) {
					List<object> columnItems = ExportInfo.ColumnsInfoColl.ColumnDvItemsByFieldName(gridColumn.FieldName); ;
					XlCellRange sourcerangeitem = null;
					if(columnItems != null && columnItems.Any())
						sourcerangeitem = GetSourceRange(auxiliarySheetName, gridColumn, columnItems.Count - 1);
					SetRanges(sourcerangeitem, gridColumn);
				}
			}
		}
		void SetRanges(XlCellRange sourcerangeitem, IColumn gridColumn){
			foreach(var rangeitem in ExportInfo.GroupsList){
				if(sourcerangeitem != null && rangeitem.End != 0){
					int endGroup = rangeitem.End;
					if(Equals(rangeitem, ExportInfo.GroupsList.Last())) endGroup--; 
					SetDataValidationOnCellsGroup(sourcerangeitem, gridColumn.LogicalPosition, rangeitem.Start, endGroup);
				}
			}
		}
		void SetDataValidationOnCellsGroup(XlCellRange rangeItem, int column, int startrow, int endrow){
			var validation = new XlDataValidation{
				Type = XlDataValidationType.List,
				AllowBlank = true,
				Operator = XlDataValidationOperator.Between,
				ListRange = rangeItem
			};
			validation.Ranges.Add(new XlCellRange(new XlCellPosition(column, startrow), new XlCellPosition(column, endrow)));
			ExportInfo.Sheet.DataValidations.Add(validation);
		}
		static XlCellRange GetSourceRange(string auxiliarySheetName, IColumn gridColumn,int endRange){
			var sourcerangeitem = new XlCellRange(
				new XlCellPosition(gridColumn.LogicalPosition, 0, XlPositionType.Absolute, XlPositionType.Absolute),
				new XlCellPosition(gridColumn.LogicalPosition, endRange, XlPositionType.Absolute,
					XlPositionType.Absolute)){ SheetName = auxiliarySheetName };
			return sourcerangeitem;
		}
		public void CompleteDataValidation(int prEvRaiseCount, int exporterRowsIndex) {
			int columnValuesMaxCount = AuxiliarySheetRowsCount;
			if(columnValuesMaxCount != 0) 
				ExportDataValidationItems(columnValuesMaxCount, prEvRaiseCount,exporterRowsIndex);
			else ExportInfo.Exporter.EndSheet();
		}
		void ExportDataValidationItems(int columnValuesMaxCount, int prEvRaiseCount, int exporterRowsIndex) {
			BeginAuxiliarySheet(ExportInfo.Exporter);
			for(int dvItemRow = 0; dvItemRow < columnValuesMaxCount; dvItemRow++) {
				ExportInfo.Exporter.BeginRow();
				ExportLookupValues(dvItemRow);
				ExportInfo.Exporter.EndRow();
				exporterRowsIndex++;
				ExportInfo.ReportProgress(exporterRowsIndex, ref prEvRaiseCount);
			}
			ExportInfo.Exporter.EndSheet();
		}
		static void BeginAuxiliarySheet(IXlExport exporter) {
			exporter.EndSheet();
			IXlSheet auxiliarySheet = exporter.BeginSheet();
			auxiliarySheet.SplitPosition = new XlCellPosition(0, 0);
			auxiliarySheet.VisibleState = XlSheetVisibleState.Hidden;
		}
		void ExportLookupValues(int dvItemRow) {
			foreach(var gridColumn in ExportInfo.ColumnsInfoColl) {
				IXlCell cell = ExportInfo.Exporter.BeginCell();
				List<object> columnItems = ExportInfo.ColumnsInfoColl.ColumnDvItemsByFieldName(gridColumn.FieldName);
				var item = GetItemByKey(columnItems, dvItemRow);
				cell.Value = XlVariantValue.FromObject(item);
				ExportInfo.Exporter.EndCell();
			}
		}
		static object GetItemByKey(IList<object> itemsList, int index){
			if(itemsList != null && itemsList.Count > index) return itemsList[index];
			return null;
		}
	}
}
