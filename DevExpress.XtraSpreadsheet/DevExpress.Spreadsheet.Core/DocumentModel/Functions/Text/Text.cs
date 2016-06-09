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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionText
	public class FunctionText : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "TEXT"; } }
		public override int Code { get { return 0x0030; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue number = CalculateNumericValue(arguments[0], context);
			if (number.IsError)
				return number;
			VariantValue formatText = CalculateFormatText(arguments[1], context);
			if (formatText.IsError)
				return formatText;
			return GetStringResult(number, formatText.GetTextValue(context.StringTable), context);
		}
		VariantValue CalculateNumericValue(VariantValue value, WorkbookDataContext context) {
			if (value.IsError)
				return value;
			if (value.IsEmpty)
				value = 0;
			if (value.IsArray)
				return value.ArrayValue[0];
			if (value.IsBoolean)
				return value.ToText(context).InlineTextValue;
			if (value.IsText) {
				if (context.TransitionFormulaEvaluation)
					return value;
				VariantValue numericValue = value.ToNumeric(context);
				return numericValue.IsError ? value : numericValue;
			}
			if (value.IsCellRange)
				if (value.CellRangeValue.CellCount > 1)
					return VariantValue.ErrorInvalidValueInFunction;
				else
					return CalculateNumericValue(value.CellRangeValue.GetFirstCellValue(), context);
			return value;
		}
		VariantValue CalculateFormatText(VariantValue value, WorkbookDataContext context) {
			if (value.IsError)
				return value;
			if (value.IsEmpty)
				return String.Empty;
			if (value.IsArray || value.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			if (value.IsCellRange)
				if (value.CellRangeValue.CellCount > 1)
					return VariantValue.ErrorInvalidValueInFunction;
				else
					return CalculateFormatText(value.CellRangeValue.GetFirstCellValue(), context); 
			return value.ToText(context);
		}
		VariantValue GetStringResult(VariantValue value, string formatText, WorkbookDataContext context) {
			if (String.IsNullOrEmpty(formatText)) {
				if (value.IsNumeric)
					return String.Empty;
				else
					return value.ToText(context);
			}
			NumberFormat numberFormat = NumberFormatParser.Parse(formatText, context.Culture);
			if (numberFormat == null)
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue result = numberFormat.Format(value, context).Text;
			if (result.GetTextValue(context.StringTable) == "#")
				return VariantValue.ErrorInvalidValueInFunction;
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			return collection;
		}
	}
	#endregion
}
