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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionIndirect
	public class FunctionIndirect : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "INDIRECT"; } }
		public override int Code { get { return 0x0094; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Reference; } }
		public override bool IsVolatile { get { return true; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			bool isA1ReferenceStyle = true;
			if (arguments.Count == 2) {
				VariantValue isA1ReferenceStyleValue = arguments[1].ToBoolean(context);
				if (isA1ReferenceStyleValue.IsError)
					return isA1ReferenceStyleValue;
				if (!isA1ReferenceStyleValue.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
				isA1ReferenceStyle = isA1ReferenceStyleValue.BooleanValue;
			}
			return GetCellRange(arguments[0], isA1ReferenceStyle, context);
		}
		VariantValue GetCellRange(VariantValue cellRange, bool isA1ReferenceStyle, WorkbookDataContext context) {
			if (cellRange.IsError)
				return cellRange;
			if (!cellRange.IsCellRange)
				return Core(isA1ReferenceStyle, context, cellRange);
			if (cellRange.CellRangeValue.RangeType == CellRangeType.UnionRange || cellRange.CellRangeValue.CellCount != 1)
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue result = cellRange.CellRangeValue.GetFirstCellValue();
			if (!result.IsCellRange) {
				if (result.IsError)
					return result;
				return Core(isA1ReferenceStyle, context, result);
			}
			else
				return VariantValue.ErrorReference;
		}
		VariantValue Core(bool isA1ReferenceStyle, WorkbookDataContext context, VariantValue cellRange) {
			VariantValue strCellRange = cellRange.ToText(context);
			try {
				context.PushUseR1C1(!isA1ReferenceStyle);
				context.PushRelativeToCurrentCell(false);
				context.PushArrayFormulaProcessing(false);
				context.PushDefinedNameProcessing(null);
				ParsedExpression expression = context.ParseExpression(strCellRange.GetTextValue(context.StringTable), OperandDataType.None, false);
				if (expression == null)
					return VariantValue.ErrorReference;
				VariantValue result = expression.Evaluate(context);
				if (result.IsError || !result.IsCellRange)
					return VariantValue.ErrorReference;
				if (result.CellRangeValue.Worksheet == null)
					return VariantValue.ErrorReference;
				return result;
			}
			finally {
				context.PopRelativeToCurrentCell();
				context.PopArrayFormulaProcessing();
				context.PopDefinedNameProcessing();
				context.PopUseR1C1();
			}
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
