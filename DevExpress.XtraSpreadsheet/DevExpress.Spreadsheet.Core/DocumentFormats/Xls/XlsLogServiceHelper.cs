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
using DevExpress.Office.Services;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Internal {
	public static class XlsLogServiceHelper {
		const string formulaRangeFormatString = "{0} ({1} {2})";
		const string definedNameFormatString = "{0} \"{1}\"({2})";
		public static string GetDescription(this ICell cell) {
			CellPosition position = new CellPosition(cell.ColumnIndex, cell.RowIndex);
			return string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_CellOnSheet), position.ToString(cell.Context), cell.Sheet.Name);
		}
		public static string GetDescription(this ICell cell, ArrayFormula arrayFormula) {
			return string.Format(formulaRangeFormatString, cell.GetDescription(), 
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ArrayFormulaRange), 
				arrayFormula.Range.ToString(cell.Context));
		}
		public static string GetDescription(this ICell cell, SharedFormula sharedFormula) {
			return string.Format(formulaRangeFormatString, cell.GetDescription(), 
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_SharedFormulaRange), 
				sharedFormula.Range.ToString(cell.Context));
		}
		public static string GetDescription(this ICell cell, CellRange range) {
			return string.Format(formulaRangeFormatString, cell.GetDescription(),
				XtraSpreadsheetLocalizer.GetString(cell.Context.ArrayFormulaProcessing ? XtraSpreadsheetStringId.Msg_ArrayFormulaRange : XtraSpreadsheetStringId.Msg_SharedFormulaRange),
				range.ToString(cell.Context));
		}
		public static string GetDescription(this DefinedNameBase definedName) {
			IWorksheet sheet = definedName.DataContext.Workbook.GetSheetById(definedName.ScopedSheetId);
			string scopeName = sheet == null ? XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_Workbook) : sheet.Name;
			return string.Format(definedNameFormatString, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_DefinedName), definedName.Name, scopeName);
		}
	}
}
