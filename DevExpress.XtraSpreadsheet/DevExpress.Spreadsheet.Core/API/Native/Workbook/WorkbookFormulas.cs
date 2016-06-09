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
namespace DevExpress.Spreadsheet.Formulas {
	#region WorkbookFormulas
	public interface FormulaEngine {
		ParameterValue Evaluate(string formula);
		ParameterValue Evaluate(string formula, ExpressionContext context);
		ParsedExpression Parse(string formula);
		ParsedExpression Parse(string formula, ExpressionContext context);
	}
	#endregion
}
#region WorkbookFormulas implementation
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet.Formulas;
	using DevExpress.XtraSpreadsheet;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Functions;
	using DevExpress.XtraSpreadsheet.Localization;
	using System.Collections.Generic;
	using DevExpress.Export.Xl;
	#region NativeFormulaEngine
	partial class NativeFormulaEngine : FormulaEngine {
		readonly NativeWorkbook nativeWorkbook;
		public NativeFormulaEngine(NativeWorkbook nativeWorkbook) {
			Guard.ArgumentNotNull(nativeWorkbook, "workbook");
			this.nativeWorkbook = nativeWorkbook;
		}
		public Model.DocumentModel ModelWorkbook { get { return nativeWorkbook.DocumentModel; } }
		#region WorkbookFormulas Members
		public ParameterValue Evaluate(string formula) {
			return Evaluate(formula, null);
		}
		public ParameterValue Evaluate(string formula, ExpressionContext context) {
			Model.WorkbookDataContext modelContext = ModelWorkbook.DataContext;
			IExpressionContext commonContext = context;
			if (commonContext == null)
				commonContext = new ActiveSettingsBasedExpressionContext(nativeWorkbook);
			PushSettingsToModelContext(nativeWorkbook, commonContext, modelContext);
			try {
				DevExpress.XtraSpreadsheet.Model.OperandDataType dataType = commonContext.ExpressionStyle == ExpressionStyle.DefinedName ? XtraSpreadsheet.Model.OperandDataType.Default : XtraSpreadsheet.Model.OperandDataType.Value;
				Model.ParsedExpression expression = GetExpression(formula, modelContext, dataType);
				Model.VariantValue value = expression.Evaluate(modelContext);
				return new ParameterValue(value, nativeWorkbook);
			}
			finally {
				PopSettingsFromModelContext(modelContext);
			}
		}
		public ParsedExpression Parse(string formula) {
			return Parse(formula, null);
		}
		public ParsedExpression Parse(string formula, ExpressionContext context) {
			Model.WorkbookDataContext modelContext = ModelWorkbook.DataContext;
			IExpressionContext commonContext = context;
			if (commonContext == null)
				commonContext = new ActiveSettingsBasedExpressionContext(nativeWorkbook);
			PushSettingsToModelContext(nativeWorkbook, commonContext, modelContext);
			try {
				DevExpress.XtraSpreadsheet.Model.OperandDataType dataType = commonContext.ExpressionStyle == ExpressionStyle.DefinedName ? XtraSpreadsheet.Model.OperandDataType.Default : XtraSpreadsheet.Model.OperandDataType.Value;
				Model.ParsedExpression expression = GetExpression(formula, modelContext, dataType);
				return ParsedExpression.FromModelExporession(expression, nativeWorkbook);
			}
			finally {
				PopSettingsFromModelContext(modelContext);
			}
		}
		#endregion
		internal static void PushSettingsToModelContext(IWorkbook workbook, IExpressionContext context, Model.WorkbookDataContext modelContext) {
			if (context == null)
				context = new ActiveSettingsBasedExpressionContext(workbook);
			Model.Worksheet sheet = modelContext.Workbook.ActiveSheet;
			Worksheet contextSheet = context.Sheet;
			if (contextSheet != null) {
				if (!object.ReferenceEquals(contextSheet.Workbook, workbook) || !workbook.Worksheets.Contains(contextSheet))
					Exceptions.ThrowInvalidOperationException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidSheetSpecified));
				sheet = ((NativeWorksheet)contextSheet).ModelWorksheet;
			}
			bool useR1C1 = false;
			if (context.ReferenceStyle == ReferenceStyle.UseDocumentSettings)
				useR1C1 = modelContext.Workbook.Properties.UseR1C1ReferenceStyle;
			else
				useR1C1 = context.ReferenceStyle == ReferenceStyle.R1C1;
			modelContext.PushCurrentWorksheet(sheet);
			modelContext.PushCurrentCell(context.Column, context.Row);
			modelContext.PushArrayFormulaProcessing(context.ExpressionStyle == ExpressionStyle.Array);
			modelContext.PushUseR1C1(useR1C1);
			if (context.Culture != null)
				modelContext.PushCulture(context.Culture);
			else
				modelContext.PushCulture(modelContext.Culture);
		}
		internal static void PopSettingsFromModelContext(Model.WorkbookDataContext modelContext) {
			modelContext.PopCulture();
			modelContext.PopArrayFormulaProcessing();
			modelContext.PopCurrentCell();
			modelContext.PopCurrentWorksheet();
			modelContext.PopUseR1C1();
		}
		Model.ParsedExpression GetExpression(string formula, Model.WorkbookDataContext modelContext, XtraSpreadsheet.Model.OperandDataType dataType) {
			Model.ParsedExpression expression = modelContext.ParseExpression(formula, dataType, false);
			if (expression == null)
				ThrowErrorFormula(formula, modelContext.Culture);
			return expression;
		}
		void ThrowErrorFormula(string formula, CultureInfo culture) {
			string cultureName = culture == System.Globalization.CultureInfo.InvariantCulture ? "Invariant" : culture.Name;
			string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorFormula), formula, cultureName);
			throw new ArgumentException(message);
		}
	}
	#endregion
}
#endregion
