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
using System.Text;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xlsx {
	partial class XlsxDataAwareExporter {
		#region Translation tables
		static Dictionary<XlDataValidationType, string> dataValidationTypeTable = CreateDataValidationTypeTable();
		static Dictionary<XlDataValidationOperator, string> dataValidationOperatorTable = CreateDataValidationOperatorTable();
		static Dictionary<XlDataValidationErrorStyle, string> dataValidationErrorStyleTable = CreateDataValidationErrorStyleTable();
		static Dictionary<XlDataValidationImeMode, string> dataValidationImeModeTable = CreateDataValidationImeModeTable();
		static Dictionary<XlDataValidationType, string> CreateDataValidationTypeTable() {
			Dictionary<XlDataValidationType, string> result = new Dictionary<XlDataValidationType, string>();
			result.Add(XlDataValidationType.Custom, "custom");
			result.Add(XlDataValidationType.Date, "date");
			result.Add(XlDataValidationType.Decimal, "decimal");
			result.Add(XlDataValidationType.List, "list");
			result.Add(XlDataValidationType.None, "none");
			result.Add(XlDataValidationType.TextLength, "textLength");
			result.Add(XlDataValidationType.Time, "time");
			result.Add(XlDataValidationType.Whole, "whole");
			return result;
		}
		static Dictionary<XlDataValidationOperator, string> CreateDataValidationOperatorTable() {
			Dictionary<XlDataValidationOperator, string> result = new Dictionary<XlDataValidationOperator, string>();
			result.Add(XlDataValidationOperator.Between, "between");
			result.Add(XlDataValidationOperator.Equal, "equal");
			result.Add(XlDataValidationOperator.GreaterThan, "greaterThan");
			result.Add(XlDataValidationOperator.GreaterThanOrEqual, "greaterThanOrEqual");
			result.Add(XlDataValidationOperator.LessThan, "lessThan");
			result.Add(XlDataValidationOperator.LessThanOrEqual, "lessThanOrEqual");
			result.Add(XlDataValidationOperator.NotBetween, "notBetween");
			result.Add(XlDataValidationOperator.NotEqual, "notEqual");
			return result;
		}
		static Dictionary<XlDataValidationErrorStyle, string> CreateDataValidationErrorStyleTable() {
			Dictionary<XlDataValidationErrorStyle, string> result = new Dictionary<XlDataValidationErrorStyle, string>();
			result.Add(XlDataValidationErrorStyle.Information, "information");
			result.Add(XlDataValidationErrorStyle.Stop, "stop");
			result.Add(XlDataValidationErrorStyle.Warning, "warning");
			return result;
		}
		static Dictionary<XlDataValidationImeMode, string> CreateDataValidationImeModeTable() {
			Dictionary<XlDataValidationImeMode, string> result = new Dictionary<XlDataValidationImeMode, string>();
			result.Add(XlDataValidationImeMode.Disabled, "disabled");
			result.Add(XlDataValidationImeMode.FullAlpha, "fullAlpha");
			result.Add(XlDataValidationImeMode.FullHangul, "fullHangul");
			result.Add(XlDataValidationImeMode.FullKatakana, "fullKatakana");
			result.Add(XlDataValidationImeMode.HalfAlpha, "halfAlpha");
			result.Add(XlDataValidationImeMode.HalfHangul, "halfHangul");
			result.Add(XlDataValidationImeMode.HalfKatakana, "halfKatakana");
			result.Add(XlDataValidationImeMode.Hiragana, "hiragana");
			result.Add(XlDataValidationImeMode.NoControl, "noControl");
			result.Add(XlDataValidationImeMode.Off, "off");
			result.Add(XlDataValidationImeMode.On, "on");
			return result;
		}
		#endregion
		const string DataValidationExtUri = "{CCE6A557-97BC-4b89-ADB6-D9C93CAAB3DF}";
		int CountItems(IList<XlDataValidation> validations, bool countExtItems) {
			int count = 0;
			int extCount = 0;
			foreach(XlDataValidation validation in validations) {
				if(validation.IsExtended)
					extCount++;
				else
					count++;
			}
			return countExtItems ? extCount : count;
		}
		bool ShouldExportDataValidationsExt(IList<XlDataValidation> validations) {
			return CountItems(validations, true) > 0;
		}
		void GenerateDataValidations(IList<XlDataValidation> dataValidations) {
			int count = CountItems(dataValidations, false);
			if (count <= 0)
				return;
			WriteShStartElement("dataValidations");
			try {
				WriteIntValue("count", count);
				foreach(XlDataValidation validation in dataValidations)
					GenerateDataValidation(validation);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateDataValidation(XlDataValidation validation) {
			if(validation.IsExtended)
				return;
			WriteShStartElement("dataValidation");
			try {
				WriteDataValidationAttributes(validation);
				WriteStringValue("sqref", GetSqrefString(validation));
				if(validation.Type == XlDataValidationType.List) {
					if(validation.ListRange != null)
						WriteString("formula1", null, validation.ListRange.ToString());
					else
						WriteString("formula1", null, CreateListValues(validation.ListValues));
				}
				else {
					XlCellPosition topLeft = GetTopLeftCell(validation.Ranges);
					if(!validation.Criteria1.IsEmpty)
						WriteString("formula1", null, GetValueObjectString(validation.Criteria1, topLeft));
					if(!validation.Criteria2.IsEmpty)
						WriteString("formula2", null, GetValueObjectString(validation.Criteria2, topLeft));
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateDataValidationsExt(IList<XlDataValidation> dataValidations) {
			int count = CountItems(dataValidations, true);
			if(count <= 0)
				return;
			WriteShStartElement("ext");
			try {
				WriteStringAttr("xmlns", "x14", null, x14NamespaceReference);
				WriteStringValue("uri", DataValidationExtUri);
				WriteStartElement("dataValidations", x14NamespaceReference);
				try {
					WriteStringAttr("xmlns", "xm", null, xmNamespaceReference);
					WriteIntValue("count", count);
					foreach(XlDataValidation validation in dataValidations)
						GenerateDataValidationExt(validation);
				}
				finally {
					WriteShEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateDataValidationExt(XlDataValidation validation) {
			if(!validation.IsExtended)
				return;
			WriteStartElement("dataValidation", x14NamespaceReference);
			try {
				WriteDataValidationAttributes(validation);
				if(validation.Type == XlDataValidationType.List) {
					if(validation.ListRange != null)
						WriteDataValidationFormulaExt("formula1", validation.ListRange.ToString());
					else
						WriteDataValidationFormulaExt("formula1", CreateListValues(validation.ListValues));
				}
				else {
					XlCellPosition topLeft = GetTopLeftCell(validation.Ranges);
					if(!validation.Criteria1.IsEmpty)
						WriteDataValidationFormulaExt("formula1", GetValueObjectString(validation.Criteria1, topLeft));
					if(!validation.Criteria2.IsEmpty)
						WriteDataValidationFormulaExt("formula2", GetValueObjectString(validation.Criteria2, topLeft));
				}
				WriteString("sqref", xmNamespaceReference, GetSqrefString(validation));
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
		void WriteDataValidationAttributes(XlDataValidation validation) {
			if(validation.Type != XlDataValidationType.None)
				WriteShStringValue("type", dataValidationTypeTable[validation.Type]);
			if(validation.AllowBlank)
				WriteBoolValue("allowBlank", validation.AllowBlank);
			if(validation.ErrorStyle != XlDataValidationErrorStyle.Stop)
				WriteShStringValue("errorStyle", dataValidationErrorStyleTable[validation.ErrorStyle]);
			if(validation.ImeMode != XlDataValidationImeMode.NoControl)
				WriteShStringValue("imeMode", dataValidationImeModeTable[validation.ImeMode]);
			if(validation.Operator != XlDataValidationOperator.Between)
				WriteShStringValue("operator", dataValidationOperatorTable[validation.Operator]);
			if(!validation.ShowDropDown && validation.Type == XlDataValidationType.List)
				WriteBoolValue("showDropDown", true);
			if(validation.ShowInputMessage && validation.Type != XlDataValidationType.None)
				WriteBoolValue("showInputMessage", true);
			if(validation.ShowErrorMessage && validation.Type != XlDataValidationType.None)
				WriteBoolValue("showErrorMessage", true);
			if(!string.IsNullOrEmpty(validation.ErrorTitle))
				WriteStringAttr(null, "errorTitle", null, validation.ErrorTitle);
			if(!string.IsNullOrEmpty(validation.ErrorMessage))
				WriteStringAttr(null, "error", null, validation.ErrorMessage);
			if(!string.IsNullOrEmpty(validation.PromptTitle))
				WriteStringAttr(null, "promptTitle", null, validation.PromptTitle);
			if(!string.IsNullOrEmpty(validation.InputPrompt))
				WriteStringAttr(null, "prompt", null, validation.InputPrompt);
		}
		string GetSqrefString(XlDataValidation validation) {
			StringBuilder sb = new StringBuilder();
			foreach(XlCellRange range in validation.Ranges) {
				if(sb.Length > 0)
					sb.Append(" ");
				sb.Append(range.ToString());
			}
			return sb.ToString();
		}
		string CreateListValues(IList<XlVariantValue> values) {
			StringBuilder result = new StringBuilder();
			int count = values.Count;
			if (count > 0)
				result.Append('\"');
			for (int i = 0; i < count; i++) {
				if (i != 0)
					result.Append(',');
				result.Append(values[i].ToText().TextValue);
			}
			if (count > 0)
				result.Append('\"');
			return result.ToString();
		}
	}
}
