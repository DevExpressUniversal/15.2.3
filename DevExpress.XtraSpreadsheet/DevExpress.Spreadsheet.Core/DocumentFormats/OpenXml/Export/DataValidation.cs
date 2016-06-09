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
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Translation tables
		internal static Dictionary<DataValidationType, string> DataValidationTypeTable = CreateDataValidationTypeTable();
		internal static Dictionary<DataValidationErrorStyle, string> DataValidationErrorStyleTable = CreateDataValidationErrorStyleTable();
		internal static Dictionary<DataValidationImeMode, string> DataValidationImeModeTable = CreateDataValidationImeModeTable();
		internal static Dictionary<DataValidationOperator, string> DataValidationOperatorTable = CreateDataValidationOperatorTable();
		static Dictionary<DataValidationType, string> CreateDataValidationTypeTable() {
			Dictionary<DataValidationType, string> result = new Dictionary<DataValidationType, string>();
			result.Add(DataValidationType.Custom, "custom");
			result.Add(DataValidationType.Date, "date");
			result.Add(DataValidationType.Decimal, "decimal");
			result.Add(DataValidationType.List, "list");
			result.Add(DataValidationType.None, "none");
			result.Add(DataValidationType.TextLength, "textLength");
			result.Add(DataValidationType.Time, "time");
			result.Add(DataValidationType.Whole, "whole");
			return result;
		}
		static Dictionary<DataValidationErrorStyle, string> CreateDataValidationErrorStyleTable() {
			Dictionary<DataValidationErrorStyle, string> result = new Dictionary<DataValidationErrorStyle, string>();
			result.Add(DataValidationErrorStyle.Information, "information");
			result.Add(DataValidationErrorStyle.Stop, "stop");
			result.Add(DataValidationErrorStyle.Warning, "warning");
			return result;
		}
		static Dictionary<DataValidationImeMode, string> CreateDataValidationImeModeTable() {
			Dictionary<DataValidationImeMode, string> result = new Dictionary<DataValidationImeMode, string>();
			result.Add(DataValidationImeMode.Disabled, "disabled");
			result.Add(DataValidationImeMode.FullAlpha, "fullAlpha");
			result.Add(DataValidationImeMode.FullHangul, "fullHangul");
			result.Add(DataValidationImeMode.FullKatakana, "fullKatakana");
			result.Add(DataValidationImeMode.HalfAlpha, "halfAlpha");
			result.Add(DataValidationImeMode.HalfHangul, "halfHangul");
			result.Add(DataValidationImeMode.HalfKatakana, "halfKatakana");
			result.Add(DataValidationImeMode.Hiragana, "hiragana");
			result.Add(DataValidationImeMode.NoControl, "noControl");
			result.Add(DataValidationImeMode.Off, "off");
			result.Add(DataValidationImeMode.On, "on");
			return result;
		}
		static Dictionary<DataValidationOperator, string> CreateDataValidationOperatorTable() {
			Dictionary<DataValidationOperator, string> result = new Dictionary<DataValidationOperator, string>();
			result.Add(DataValidationOperator.Between, "between");
			result.Add(DataValidationOperator.Equal, "equal");
			result.Add(DataValidationOperator.GreaterThan, "greaterThan");
			result.Add(DataValidationOperator.GreaterThanOrEqual, "greaterThanOrEqual");
			result.Add(DataValidationOperator.LessThan, "lessThan");
			result.Add(DataValidationOperator.LessThanOrEqual, "lessThanOrEqual");
			result.Add(DataValidationOperator.NotBetween, "notBetween");
			result.Add(DataValidationOperator.NotEqual, "notEqual");
			return result;
		}
		#endregion
		#region DataValidations
		protected internal virtual void GenerateDataValidations() {
			DataValidationCollection validations = ActiveSheet.DataValidations;
			int count = CountItems(validations, false);
			if (count <= 0)
				return;
			WriteShStartElement("dataValidations");
			try {
				WriteIntValue("count", count);
				validations.ForEach(GenerateDataValidation);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateDataValidation(DataValidation validation) {
			if (IsDataValidationExt(validation))
				return;
			WriteShStartElement("dataValidation");
			try {
				WriteDataValidationAttributes(validation);
				WriteStSqref(validation.CellRange, "sqref");
				if (!ParsedExpression.IsNullOrEmpty(validation.Expression1))
					WriteString("formula1", null, PrepareDataValidationFormula(validation.CellRange.TopLeft, validation.Expression1));
				if (!ParsedExpression.IsNullOrEmpty(validation.Expression2))
					WriteString("formula2", null, PrepareDataValidationFormula(validation.CellRange.TopLeft, validation.Expression2));
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteDataValidationAttributes(DataValidation validation) {
			if (validation.Type != DataValidationType.None)
				WriteShStringValue("type", DataValidationTypeTable[validation.Type]);
			if (validation.AllowBlank)
				WriteBoolValue("allowBlank", validation.AllowBlank);
			if (validation.ErrorStyle != DataValidationErrorStyle.Stop)
				WriteShStringValue("errorStyle", DataValidationErrorStyleTable[validation.ErrorStyle]);
			if (validation.ImeMode != DataValidationImeMode.NoControl)
				WriteShStringValue("imeMode", DataValidationImeModeTable[validation.ImeMode]);
			if (validation.ValidationOperator != DataValidationOperator.Between)
				WriteShStringValue("operator", DataValidationOperatorTable[validation.ValidationOperator]);
			if (validation.SuppressDropDown)
				WriteBoolValue("showDropDown", validation.SuppressDropDown);
			if (validation.ShowInputMessage)
				WriteBoolValue("showInputMessage", validation.ShowInputMessage);
			if (validation.ShowErrorMessage)
				WriteBoolValue("showErrorMessage", validation.ShowErrorMessage);
			if (!string.IsNullOrEmpty(validation.ErrorTitle))
				WriteStringAttr(null, "errorTitle", null, EncodeXmlChars(validation.ErrorTitle));
			if (!string.IsNullOrEmpty(validation.Error))
				WriteStringAttr(null, "error", null, EncodeXmlChars(validation.Error));
			if (!string.IsNullOrEmpty(validation.PromptTitle))
				WriteStringAttr(null, "promptTitle", null, EncodeXmlChars(validation.PromptTitle));
			if (!string.IsNullOrEmpty(validation.Prompt))
				WriteStringAttr(null, "prompt", null, EncodeXmlChars(validation.Prompt));
		}
		protected internal void GenerateDataValidationsExt() {
			DataValidationCollection validations = ActiveSheet.DataValidations;
			int count = CountItems(validations, true);
			if (count <= 0)
				return;
			WriteShStartElement("ext");
			try {
				WriteStringAttr("xmlns", "x14", null, x14NamespaceReference);
				WriteStringValue("uri", DataValidationExtUri);
				WriteStartElement("dataValidations", x14NamespaceReference);
				try {
					WriteStringAttr("xmlns", "xm", null, xmNamespaceReference);
					WriteIntValue("count", count);
					validations.ForEach(GenerateDataValidationExt);
				}
				finally {
					WriteShEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateDataValidationExt(DataValidation validation) {
			if (!IsDataValidationExt(validation))
				return;
			WriteStartElement("dataValidation", x14NamespaceReference);
			try {
				WriteDataValidationAttributes(validation);
				CellRangeBase range = validation.CellRange;
				if (!ParsedExpression.IsNullOrEmpty(validation.Expression1))
					WriteDataValidationFormulaExt("formula1", PrepareDataValidationFormula(range.TopLeft, validation.Expression1));
				if (!ParsedExpression.IsNullOrEmpty(validation.Expression2))
					WriteDataValidationFormulaExt("formula2", PrepareDataValidationFormula(range.TopLeft, validation.Expression2));
				WriteString("sqref", xmNamespaceReference, GetStSqrefValue(range));
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteDataValidationFormulaExt(string tag, string formulaBody) {
			WriteStartElement(tag, x14NamespaceReference);
			try {
				WriteString("f", xmNamespaceReference, formulaBody);
			}
			finally {
				WriteShEndElement();
			}
		}
		string PrepareDataValidationFormula(CellPosition topLeft, ParsedExpression expression) {
			WorkbookDataContext dataContext = ActiveSheet.DataContext;
			dataContext.PushRelativeToCurrentCell(true);
			dataContext.PushCurrentCell(topLeft);
			try {
				return expression.BuildExpressionString(dataContext);
			}
			finally {
				dataContext.PopRelativeToCurrentCell();
				dataContext.PopCurrentCell();
			}
		}
		bool ShouldGenerateDataValidationsExt() {
			if (ActiveSheet == null)
				return false;
			return CountItems(ActiveSheet.DataValidations, true) > 0;
		}
		int CountItems(DataValidationCollection validations, bool countExtItems) {
			int count = 0;
			int extCount = 0;
			foreach (DataValidation validation in validations) {
				if (IsDataValidationExt(validation))
					extCount++;
				else
					count++;
			}
			return countExtItems ? extCount : count;
		}
		bool IsDataValidationExt(DataValidation validation) {
			return Has3dItems(validation.Expression1) || Has3dItems(validation.Expression2);
		}
		bool Has3dItems(ParsedExpression expression) {
			if (ParsedExpression.IsNullOrEmpty(expression))
				return false;
			foreach (IParsedThing ptg in expression)
				if (ptg is IParsedThingWithSheetDefinition)
					return true;
			return false;
		}
		#endregion
	}
}
