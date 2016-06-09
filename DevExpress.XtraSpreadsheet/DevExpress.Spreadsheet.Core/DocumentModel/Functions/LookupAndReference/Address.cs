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
using DevExpress.Utils;
using System.Text.RegularExpressions;
using DevExpress.XtraSpreadsheet.Utils;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionAddress
	public class FunctionAddress : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "ADDRESS"; } }
		public override int Code { get { return 0x00DB; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue rowNumber = arguments[0].ToNumeric(context);
			if (rowNumber.IsError)
				return rowNumber;
			VariantValue columnNumber = arguments[1].ToNumeric(context);
			if (columnNumber.IsError)
				return columnNumber;
			int referenceType = 1;
			bool useR1C1Style = false;
			string sheetName = String.Empty;
			int operandCount = arguments.Count;
			if (operandCount >= 3) {
				VariantValue typeValue = arguments[2];
				VariantValue type;
				if (typeValue.IsEmpty)
					type = 1;
				else
					type = typeValue.ToNumeric(context);
				if (type.IsError)
					return type;
				referenceType = (int)type.NumericValue;
				if (referenceType <= 0 || referenceType > 4)
					return VariantValue.ErrorInvalidValueInFunction;
			}
			if (operandCount >= 4) {
				VariantValue style = arguments[3].ToBoolean(context);
				if (style.IsError)
					return style;
				useR1C1Style = !style.BooleanValue;
			}
			if (operandCount >= 5) {
				VariantValue name = arguments[4].ToText(context);
				if (name.IsError)
					return name;
				sheetName = name.GetTextValue(context.StringTable);
			}
			PositionType columnPositionType = CalculateColumnPositionType(referenceType);
			PositionType rowPositionType = CalculateRowPositionType(referenceType);
			int rowIndex = (int)rowNumber.NumericValue;
			if (rowPositionType == PositionType.Absolute || !useR1C1Style) {
				rowIndex--;
				if (rowIndex < 0)
					return VariantValue.ErrorInvalidValueInFunction;
			}
			int columnIndex = (int)columnNumber.NumericValue;
			if (columnPositionType == PositionType.Absolute || !useR1C1Style) {
				columnIndex--;
				if (columnIndex < 0)
					return VariantValue.ErrorInvalidValueInFunction;
			}
			return CreateReferenceText(context, rowIndex, columnIndex, columnPositionType, rowPositionType, useR1C1Style, sheetName);
		}
		internal string CreateReferenceText(WorkbookDataContext context, int rowIndex, int columnIndex, PositionType columnPositionType, PositionType rowPositionType, bool useR1C1Style, string sheetName) {
			StringBuilder result = new StringBuilder();
			if (!String.IsNullOrEmpty(sheetName)) {
				SheetDefinition sheetDefinition = new SheetDefinition();
				sheetDefinition.SheetNameStart = sheetName;
				sheetDefinition.SheetNameEnd = sheetName;
				sheetDefinition.BuildExpressionString(result, context);
			}
			if (useR1C1Style) {
				result.Append("R");
				CellReferenceParser.BuildRCStylePart(result, rowIndex, rowPositionType, 0, IndicesChecker.MaxRowCount);
				result.Append("C");
				CellReferenceParser.BuildRCStylePart(result, columnIndex, columnPositionType, 0, IndicesChecker.MaxColumnCount);
			}
			else {
				CellPosition position = new CellPosition(columnIndex, rowIndex, columnPositionType, rowPositionType);
				CellReferenceParser.GetA1StringRepresentation(position, result);
			}
			return result.ToString();
		}
		PositionType CalculateRowPositionType(int type) {
			if (type == 1 || type == 2)
				return PositionType.Absolute;
			else
				return PositionType.Relative;
		}
		PositionType CalculateColumnPositionType(int type) {
			if (type == 1 || type == 3)
				return PositionType.Absolute;
			else
				return PositionType.Relative;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
