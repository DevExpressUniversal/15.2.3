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
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public static class RowExtensions {
		public static bool IsDefault(this Row row) {
			bool isCustom = row.ApplyStyle || row.IsCustomHeight || row.IsCollapsed || 
				row.IsHidden || row.IsThickBottomBorder || row.IsThickTopBorder || 
				row.OutlineLevel > 0 || row.CellsCount > 0;
			return !isCustom;
		}
		public static bool OutOfXlsLimits(this Row row) {
			if(row.Index >= XlsDefs.MaxRowCount) return true;
			int firstColumnIndex = row.FirstColumnIndex;
			if(firstColumnIndex != -1 && firstColumnIndex >= XlsDefs.MaxColumnCount)
				return true;
			return false;
		}
	}
	public static class WorksheetExtensions {
		public static CellRange GetDimensions(this Worksheet sheet) {
			return GetCustomCellsRangeCore(sheet, true);
		}
		public static CellRange GetCustomCellsRange(this Worksheet sheet) {
			return GetCustomCellsRangeCore(sheet, false);
		}
		public static CellRange GetCustomRowsRange(this Worksheet sheet) {
			return GetCustomRowsRangeCore(sheet);
		}
		static CellRange GetCustomCellsRangeCore(Worksheet sheet, bool withComments) {
			int minRowIndex = XlsDefs.MaxRowCount;
			int maxRowIndex = 0;
			int minColIndex = XlsDefs.MaxColumnCount;
			int maxColIndex = 0;
			foreach(Row row in sheet.Rows.GetExistingRows(0, XlsDefs.MaxRowCount, false)) {
				if(SkipRow(row))
					continue;
				if(row.Index < minRowIndex)
					minRowIndex = row.Index;
				if(row.Index > maxRowIndex)
					maxRowIndex = row.Index;
				int firstColumnIndex = row.FirstColumnIndex;
				if(firstColumnIndex != -1 && firstColumnIndex < minColIndex)
					minColIndex = firstColumnIndex;
				int lastColumnIndex = row.LastColumnIndex;
				if(lastColumnIndex != -1) {
					if(lastColumnIndex >= XlsDefs.MaxColumnCount)
						lastColumnIndex = XlsDefs.MaxColumnCount - 1;
					if(lastColumnIndex > maxColIndex)
						maxColIndex = lastColumnIndex;
				}
			}
			if(withComments) {
				foreach(Comment comment in sheet.Comments) {
					if(comment.Reference.OutOfLimits()) continue;
					if(comment.Reference.Row < minRowIndex)
						minRowIndex = comment.Reference.Row;
					if(comment.Reference.Row > maxRowIndex)
						maxRowIndex = comment.Reference.Row;
					if(comment.Reference.Column < minColIndex)
						minColIndex = comment.Reference.Column;
					if(comment.Reference.Column > maxColIndex)
						maxColIndex = comment.Reference.Column;
				}
			}
			if(maxRowIndex < minRowIndex)
				return null;
			if(maxColIndex < minColIndex)
				minColIndex = 0;
			return new CellRange(sheet, new CellPosition(minColIndex, minRowIndex), new CellPosition(maxColIndex, maxRowIndex));
		}
		static CellRange GetCustomRowsRangeCore(Worksheet sheet) {
			int minRowIndex = XlsDefs.MaxRowCount;
			int maxRowIndex = 0;
			foreach(Row row in sheet.Rows.GetExistingRows(0, XlsDefs.MaxRowCount, false)) {
				if(SkipRow(row))
					continue;
				if(row.Index < minRowIndex)
					minRowIndex = row.Index;
				if(row.Index > maxRowIndex)
					maxRowIndex = row.Index;
			}
			if(maxRowIndex < minRowIndex)
				return null;
			return new CellRange(sheet, new CellPosition(0, minRowIndex), new CellPosition(0, maxRowIndex));
		}
		static bool SkipRow(Row row) {
			return row.IsDefault() || row.OutOfXlsLimits();
		}
	}
}
