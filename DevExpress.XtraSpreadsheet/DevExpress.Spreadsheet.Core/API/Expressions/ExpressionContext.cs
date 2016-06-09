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
using System.Globalization;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet.Functions;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using Model = DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using System.ComponentModel;
using DevExpress.Export.Xl;
namespace DevExpress.Spreadsheet.Formulas {
	#region ExpressionStyle
	public enum ExpressionStyle {
		Normal,
		Array,
		DefinedName,
	}
	#endregion
	#region IExpressionContext
	public interface IExpressionContext : EvaluationContext {
		[Obsolete("Use the ExpressionStyle property instead.", false)]
		bool IsArrayFormula { get; }
		ReferenceStyle ReferenceStyle { get; }
		ExpressionStyle ExpressionStyle { get; }
	}
	#endregion
	#region ExpressionContext
	public class ExpressionContext : IExpressionContext {
		#region Fields
		CultureInfo culture;
		int column = 0;
		int row = 0;
		Worksheet sheet;
		ReferenceStyle referenceStyle = ReferenceStyle.UseDocumentSettings;
		ExpressionStyle expressionStyle = ExpressionStyle.Normal;
		#endregion
		public ExpressionContext() {
		}
		public ExpressionContext(int column, int row, Worksheet sheet)
			: this(column, row, sheet, null, ReferenceStyle.UseDocumentSettings, ExpressionStyle.Normal) {
		}
		[Obsolete("Use the ExpressionContext(int column, int row, Worksheet sheet, CultureInfo culture, ReferenceStyle referenceStyle, ExpressionStyle expressionStyle) constructor instead.", false)]
		public ExpressionContext(int column, int row, Worksheet sheet, CultureInfo culture, bool isArrayFormula, ReferenceStyle referenceStyle)
			: this(column, row, sheet, null, ReferenceStyle.UseDocumentSettings, isArrayFormula ? ExpressionStyle.Array : ExpressionStyle.Normal) {
		}
		public ExpressionContext(int column, int row, Worksheet sheet, CultureInfo culture, ReferenceStyle referenceStyle, ExpressionStyle expressionStyle) {
			CheckColumnIndex(column);
			CheckRowIndex(row);
			this.column = column;
			this.row = row;
			this.sheet = sheet;
			this.culture = culture;
			this.referenceStyle = referenceStyle;
			this.expressionStyle = expressionStyle;
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ExpressionContextCulture")]
#endif
		public CultureInfo Culture { get { return culture; } set { culture = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ExpressionContextColumn")]
#endif
		public int Column {
			get { return column; }
			set {
				CheckColumnIndex(column);
				column = value;
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ExpressionContextRow")]
#endif
		public int Row {
			get { return row; }
			set {
				CheckRowIndex(row);
				row = value;
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ExpressionContextSheet")]
#endif
		public Worksheet Sheet { get { return sheet; } set { sheet = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ExpressionContextIsArrayFormula")]
#endif
		[Obsolete("Use the ExpressionStyle property instead.", false)]
		public bool IsArrayFormula {
			get { return ExpressionStyle == ExpressionStyle.Array; }
			set { ExpressionStyle = value ? ExpressionStyle.Array : ExpressionStyle.Normal; }
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ExpressionContextReferenceStyle")]
#endif
		public ReferenceStyle ReferenceStyle { get { return referenceStyle; } set { referenceStyle = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ExpressionContextExpressionStyle")]
#endif
		public ExpressionStyle ExpressionStyle { get { return expressionStyle; } set { expressionStyle = value; } }
		#endregion
		void CheckColumnIndex(int value) {
			if (!IndicesChecker.CheckIsColumnIndexValid(value))
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnIndex, "column");
		}
		void CheckRowIndex(int value) {
			if (!IndicesChecker.CheckIsRowIndexValid(value))
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowIndex, "row");
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet.Formulas;
	using DevExpress.XtraSpreadsheet;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Functions;
	using DevExpress.XtraSpreadsheet.Localization;
	using System.Collections.Generic;
	#region ActiveSettingsBasedExpressionContext
	partial class ActiveSettingsBasedExpressionContext : IExpressionContext {
		readonly IWorkbook workbook;
		public ActiveSettingsBasedExpressionContext(IWorkbook workbook) {
			this.workbook = workbook;
		}
		#region IExpressionContext Members
		public CultureInfo Culture { get { return workbook.Options.Culture; } }
		public int Column { get { return Sheet.SelectedCell.LeftColumnIndex; } }
		public int Row { get { return Sheet.SelectedCell.TopRowIndex; } }
		public Worksheet Sheet { get { return workbook.Worksheets.ActiveWorksheet; } }
		public ReferenceStyle ReferenceStyle { get { return Spreadsheet.ReferenceStyle.UseDocumentSettings; } }
		public bool IsArrayFormula { get { return false; } }
		public ExpressionStyle ExpressionStyle { get { return ExpressionStyle.Normal; } }
		#endregion
	}
	#endregion
}
