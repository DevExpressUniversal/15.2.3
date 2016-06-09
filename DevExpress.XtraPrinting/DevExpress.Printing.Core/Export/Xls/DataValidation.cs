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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	public partial class XlsDataAwareExporter {
		readonly Dictionary<XlCellRange, int> dataValidationListRanges = new Dictionary<XlCellRange, int>();
		protected void WriteDataValidations() {
			IList<XlDataValidation> dataValidations = currentSheet.DataValidations;
			int recordCount = GetDataValidationsRecordCount(dataValidations);
			if(recordCount == 0)
				return;
			XlsContentDVal content = new XlsContentDVal();
			content.RecordCount = recordCount;
			WriteContent(XlsRecordType.DVal, content);
			int count = 0;
			foreach(XlDataValidation item in dataValidations) {
				if(WriteDataValidation(item)) {
					count++;
					if(count > XlsDefs.MaxDataValidationRecordCount)
						break;
				}
			}
		}
		int GetDataValidationsRecordCount(IList<XlDataValidation> collection) {
			int result = 0;
			int count = collection.Count;
			for(int i = 0; i < count; i++) {
				XlDataValidation item = collection[i];
				if(!IsXlsCompatibleRange(item.Ranges))
					continue;
				result++;
				if(result == XlsDefs.MaxDataValidationRecordCount)
					break;
			}
			return result;
		}
		bool IsXlsCompatibleRange(IList<XlCellRange> ranges) {
			int count = ranges.Count;
			if(count == 0)
				return false;
			for(int i = 0; i < count; i++)
				if(!IsXlsCompatibleRange(ranges[i]))
					return false;
			return true;
		}
		bool IsXlsCompatibleRange(XlCellRange cellRange) {
			return cellRange.TopLeft.Column < XlsDefs.MaxColumnCount && cellRange.TopLeft.Row < XlsDefs.MaxRowCount;
		}
		string CheckLength(string value, int maxLength) {
			if(string.IsNullOrEmpty(value) || value.Length <= maxLength)
				return value;
			return value.Substring(0, maxLength);
		}
		XlCellPosition GetTopLeftCell(IList<XlCellRange> ranges) {
			if(ranges.Count == 0)
				return XlCellPosition.InvalidValue;
			XlCellRange range = ranges[0];
			int column = range.TopLeft.Column;
			int row = range.TopLeft.Row;
			for(int i = 1; i < ranges.Count; i++) {
				range = ranges[i];
				column = Math.Min(column, range.TopLeft.Column);
				row = Math.Min(row, range.TopLeft.Row);
			}
			return new XlCellPosition(column, row);
		}
		bool WriteDataValidation(XlDataValidation validation) {
			XlsContentDv content = new XlsContentDv();
			foreach(XlCellRange range in validation.Ranges) {
				XlsRef8 item = XlsRef8.FromRange(range);
				if(item != null)
					content.Ranges.Add(item);
			}
			if(content.Ranges.Count == 0)
				return false;
			content.ValidationType = validation.Type;
			content.ErrorStyle = validation.ErrorStyle;
			content.AllowBlank = validation.AllowBlank;
			content.SuppressCombo = (validation.Type != XlDataValidationType.List) || !validation.ShowDropDown;
			content.ImeMode = validation.ImeMode;
			content.ShowInputMessage = validation.ShowInputMessage;
			content.ShowErrorMessage = validation.ShowErrorMessage;
			content.ValidationOperator = validation.Operator;
			content.PromptTitle = CheckLength(validation.PromptTitle, XlsDefs.MaxDataValidationTitleLength);
			content.ErrorTitle = CheckLength(validation.ErrorTitle, XlsDefs.MaxDataValidationTitleLength);
			content.Prompt = CheckLength(validation.InputPrompt, XlsDefs.MaxDataValidationPromptLength);
			content.Error = CheckLength(validation.ErrorMessage, XlsDefs.MaxDataValidationErrorLength);
			if(validation.Type == XlDataValidationType.List) {
				if(validation.ListRange != null)
					content.Formula1Bytes = GetListRangeFormulaBytes(validation.ListRange);
				else {
					content.Formula1Bytes = GetFormulaBytes(validation.ListValues);
					content.StringLookup = true;
				}
			}
			else {
				expressionContext.CurrentCell = GetTopLeftCell(validation.Ranges);
				if(!validation.Criteria1.IsEmpty)
					content.Formula1Bytes = GetFormulaBytes(validation.Criteria1);
				if(!validation.Criteria2.IsEmpty)
					content.Formula2Bytes = GetFormulaBytes(validation.Criteria2);
			}
			WriteContent(XlsRecordType.Dv, content);
			return true;
		}
		bool RegisterDataValidation(XlDataValidation validation) {
			if(!IsXlsCompatibleRange(validation.Ranges))
				return false;
			if(validation.Type == XlDataValidationType.List && validation.ListRange != null && !string.IsNullOrEmpty(validation.ListRange.SheetName))
				RegisterDataValidationListRange(validation.ListRange);
			return true;
		}
		void RegisterDataValidationListRange(XlCellRange range) {
			if(dataValidationListRanges.ContainsKey(range))
				return;
			XlsContentDefinedName content = new XlsContentDefinedName();
			content.Name = string.Format("DvListSource{0}", dataValidationListRanges.Count + 1);
			content.SheetIndex = 0;
			content.IsHidden = false;
			content.FormulaBytes = GetFormulaBytes(range);
			definedNames.Add(content);
			dataValidationListRanges.Add(range, definedNames.Count);
		}
		int GetListSourceNameIndex(XlCellRange range) {
			if(dataValidationListRanges.ContainsKey(range))
				return dataValidationListRanges[range];
			return 0;
		}
		byte[] GetListRangeFormulaBytes(XlCellRange range) {
			if(string.IsNullOrEmpty(range.SheetName))
				return GetFormulaBytes(range);
			int nameIndex = GetListSourceNameIndex(range);
			XlExpression formula = new XlExpression();
			formula.Add(new XlPtgName(nameIndex, XlPtgDataType.Reference));
			return formula.GetBytes(this);
		}
		byte[] GetFormulaBytes(XlCellRange range) {
			XlExpression formula = new XlExpression();
			formula.Add(CreatePtg(range));
			return formula.GetBytes(this);
		}
		byte[] GetFormulaBytes(IList<XlVariantValue> values) {
			XlExpression formula = new XlExpression();
			StringBuilder sb = new StringBuilder();
			foreach(XlVariantValue value in values) {
				if(sb.Length > 0)
					sb.Append("\0");
				sb.Append(value.ToText().TextValue);
			}
			formula.Add(new XlPtgStr(sb.ToString())); 
			return formula.GetBytes(this);
		}
		byte[] GetFormulaBytes(XlVariantValue value) {
			XlExpression formula = new XlExpression();
			formula.Add(CreatePtg(value));
			return formula.GetBytes(this);
		}
	}
}
