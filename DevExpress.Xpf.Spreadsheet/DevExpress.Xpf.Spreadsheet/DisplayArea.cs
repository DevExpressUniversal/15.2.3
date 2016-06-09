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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
#if SL || WPF
namespace DevExpress.Xpf.Spreadsheet {
#else
namespace DevExpress.XtraSpreadsheet {
#endif
	#region WorksheetDisplayArea
	public class WorksheetDisplayArea {
		#region static
		const string invalidLimitMessage = "The number of visible columns or rows cannot be less or equal to 0.";
		static void SetSheetDisplayLimits(int maxColumnCount, int maxRowCount, Worksheet sheet) {
			if (maxColumnCount <= 0)
				throw new ArgumentOutOfRangeException(invalidLimitMessage);
			if (maxRowCount <= 0)
				throw new ArgumentOutOfRangeException(invalidLimitMessage);
			sheet.MaxColumnCount = maxColumnCount;
			sheet.MaxRowCount = maxRowCount;
		}
		#endregion
		#region Fields
		readonly DocumentModel workbook;
		#endregion
		public WorksheetDisplayArea(DocumentModel workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
		}
		#region Properties
		internal DocumentModel Workbook { get { return workbook; } }
		#endregion
		Worksheet GetSheet(string name) {
			Worksheet sheet = workbook.GetSheetByName(name) as Worksheet;
			if (sheet == null)
				throw new ArgumentException();
			return sheet;
		}
		Worksheet GetSheet(int index) {
			Worksheet sheet = workbook.GetSheetByIndex(index) as Worksheet;
			if (sheet == null)
				throw new ArgumentException();
			return sheet;
		}
		public int GetColumnCount(string worksheetName) {
			return GetSheet(worksheetName).MaxColumnCount;
		}
		public int GetColumnCount(int worksheetIndex) {
			return GetSheet(worksheetIndex).MaxColumnCount;
		}
		public int GetRowCount(string worksheetName) {
			return GetSheet(worksheetName).MaxRowCount;
		}
		public int GetRowCount(int worksheetIndex) {
			return GetSheet(worksheetIndex).MaxRowCount;
		}
		public void SetSize(string worksheetName, int maxColumnCount, int maxRowCount) {
			SetSheetDisplayLimits(maxColumnCount, maxRowCount, GetSheet(worksheetName));
		}
		public void SetSize(int worksheetIndex, int maxColumnCount, int maxRowCount) {
			SetSheetDisplayLimits(maxColumnCount, maxRowCount, GetSheet(worksheetIndex));
		}
		public void Reset(string worksheetName) {
			SetSize(worksheetName, DevExpress.XtraSpreadsheet.Utils.IndicesChecker.MaxColumnCount, DevExpress.XtraSpreadsheet.Utils.IndicesChecker.MaxRowCount);
		}
		public void Reset(int worksheetIndex) {
			SetSize(worksheetIndex, DevExpress.XtraSpreadsheet.Utils.IndicesChecker.MaxColumnCount, DevExpress.XtraSpreadsheet.Utils.IndicesChecker.MaxRowCount);
		}
	}
	#endregion
}
