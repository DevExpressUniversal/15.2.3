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
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using ModelCellRange = DevExpress.XtraSpreadsheet.Model.CellRange;
using ModelCellRangeBase = DevExpress.XtraSpreadsheet.Model.CellRangeBase;
using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
#if !SL
using System.Drawing;
using DevExpress.XtraSpreadsheet.Localization;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	public static class RangeExtensions {
		static void CheckUnionRange(Range range){
			if (range.Areas.Count > 1)
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUnionRange));
		}
		public static void Clear(this Range range) {
			range.Worksheet.Clear(range);
		}
		public static void ClearContents(this Range range) {
			range.Worksheet.ClearContents(range);
		}
		public static void ClearFormats(this Range range) {
			range.Worksheet.ClearFormats(range);
		}
		public static void ClearHyperlinks(this Range range) {
			range.Worksheet.ClearHyperlinks(range);
		}
		public static void ClearComments(this Range range) {
			range.Worksheet.ClearComments(range);
		}
		public static void GroupRows(this Range range, bool collapse) {
			CheckUnionRange(range);
			range.Worksheet.Rows.Group(range.TopRowIndex, range.BottomRowIndex, collapse);
		}
		public static void GroupColumns(this Range range, bool collapse) {
			CheckUnionRange(range);
			range.Worksheet.Columns.Group(range.LeftColumnIndex, range.RightColumnIndex, collapse);
		}
		public static void UnGroupColumns(this Range range, bool unhideCollapsed) {
			CheckUnionRange(range);
			range.Worksheet.Columns.UnGroup(range.LeftColumnIndex, range.RightColumnIndex, unhideCollapsed);
		}
		public static void UnGroupRows(this Range range, bool unhideCollapsed) {
			CheckUnionRange(range);
			range.Worksheet.Rows.UnGroup(range.TopRowIndex, range.BottomRowIndex, unhideCollapsed);
		}
		public static void ClearOutline(this Range range, bool rows, bool columns) {
			if (rows)
				range.Worksheet.Rows.ClearOutline(range.TopRowIndex, range.BottomRowIndex);
			if (columns)
				range.Worksheet.Columns.ClearOutline(range.LeftColumnIndex, range.RightColumnIndex);
		}
		public static void Subtotal(this Range range, int groupByColumn, System.Collections.Generic.List<int> subtotalColumnList, int functionCode, string functionText) {
			range.Worksheet.Subtotal(range, groupByColumn, subtotalColumnList, functionCode, functionText);
		}
		public static void RemoveSubtotal(this Range range) {
			range.Worksheet.RemoveSubtotal(range);
		}
		public static void Merge(this Range range) {
			range.Worksheet.MergeCells(range);
		}
		public static void Merge(this Range range, MergeCellsMode mode) {
			range.Worksheet.MergeCells(range, mode);
		}
		public static void UnMerge(this Range range) {
			range.Worksheet.UnMergeCells(range);
		}
		public static void Insert(this Range range) {
			range.Worksheet.InsertCells(range);
		}
		public static void Delete(this Range range) {
			range.Worksheet.DeleteCells(range);
		}
		public static void Insert(this Range range, InsertCellsMode mode) {
			range.Worksheet.InsertCells(range, mode);
		}
		public static void Delete(this Range range, DeleteMode mode) {
			range.Worksheet.DeleteCells(range, mode);
		}
		public static void Select(this Range range) {
			range.Worksheet.Selection = range;
		}
		public static void AutoFitColumns(this Range range) {
			NativeWorksheet nativeSheet = range.Worksheet as NativeWorksheet;
			if (nativeSheet == null)
				return;
			ModelCellRangeBase modelRange = nativeSheet.GetModelRange(range);
			nativeSheet.ModelWorksheet.TryBestFitColumn(modelRange, ColumnBestFitMode.None);
		}
		public static void AutoFitRows(this Range range) {
			NativeWorksheet nativeSheet = range.Worksheet as NativeWorksheet;
			if (nativeSheet == null)
				return;
			ModelCellRangeBase modelRange = nativeSheet.GetModelRange(range);
			nativeSheet.ModelWorksheet.TryBestFitRow(modelRange);
		}
	}
}
