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
	#region FunctionIndex
	public class FunctionIndex : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "INDEX"; } }
		public override int Code { get { return 0x001D; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Reference; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0];
			if (value.IsError)
				return value;
			VariantValue row = arguments[1].ToNumeric(context);
			if (row.IsError)
				return row;
			VariantValue column = GetColumnIndex(arguments, context);
			if (column.IsError)
				return column;
			return IndexCore(arguments, context, value, (int)row.NumericValue, (int)column.NumericValue);
		}
		VariantValue IndexCore(IList<VariantValue> arguments, WorkbookDataContext context, VariantValue value, int row, int column) {
			VariantValue result;
			if (value.IsArray)
				result = GetValueFromArray(arguments, context, value, row, column);
			else if (value.IsCellRange)
				result = GetValueFromCellRange(arguments, context, value, row, column);
			else if (value.IsNumeric)
				result = GetNumericValue(arguments, context, value, row, column);
			else
				result = VariantValue.ErrorInvalidValueInFunction;
			return !result.IsEmpty ? result : 0;
		}
		VariantValue GetNumericValue(IList<VariantValue> arguments, WorkbookDataContext context, VariantValue value, int row, int column) {
			if (row < 0 || row > 1 || column < 0 || column > 1)
				return VariantValue.ErrorReference;
			if (arguments.Count == 4) {
				VariantValue area = GetAreaCore(arguments, context, 1);
				if (area.IsError)
					return area;
			}
			return value;
		}
		VariantValue GetArea(IList<VariantValue> arguments, WorkbookDataContext context, CellRangeBase range, CellRange singleRange) {
			VariantValue result = new VariantValue();
			result.CellRangeValue = singleRange;
			int areaCount = 1;
			CellUnion cellUnion = range as CellUnion;
			if (range.RangeType == CellRangeType.UnionRange)
				areaCount = cellUnion.InnerCellRanges.Count;
			VariantValue area = GetAreaCore(arguments, context, areaCount);
			if (area.IsError)
				return area;
			if (range.RangeType == CellRangeType.UnionRange)
				result.CellRangeValue = cellUnion.InnerCellRanges[(int)area.NumericValue - 1].GetFirstInnerCellRange();
			return result;
		}
		VariantValue GetAreaCore(IList<VariantValue> arguments, WorkbookDataContext context, int maxValue) {
			VariantValue area = arguments[3].ToNumeric(context).ToNumeric(context);
			if (area.IsError) 
				return area;
			if ((int)area.NumericValue < 1)
				return VariantValue.ErrorInvalidValueInFunction;
			if ((int)area.NumericValue > maxValue)
				return VariantValue.ErrorReference;
			return area;
		}
		VariantValue GetValueFromCellRange(IList<VariantValue> arguments, WorkbookDataContext context, VariantValue value, int row, int column) {
			CellRange singleRange = value.CellRangeValue.GetFirstInnerCellRange();
			int operandCount = arguments.Count;
			VariantValue result = new VariantValue();
			if (operandCount == 4) {
				VariantValue area = GetArea(arguments, context, value.CellRangeValue, singleRange);
				if (area.IsError)
					return area;
				else
					singleRange = (CellRange)area.CellRangeValue;
			}
			if (row == 0 && column == 0) {
				result.CellRangeValue = singleRange;
				return result;
			}
			if ((singleRange.Width > 1) && (singleRange.Height > 1))
				result = GetResultFromTwoDimensionalRange(singleRange, row, column, operandCount);
			else
				result = GetResultFromOneDimensionalRange(singleRange, row, column, operandCount);
			if (!result.IsError) {
				VariantValue resultValue = result.CellRangeValue.GetCellValueRelative(0, 0);
				if (result.CellRangeValue.CellCount == 1 && resultValue.IsError)
					result = resultValue;
			}
			return result;
		}
		VariantValue GetResultFromOneDimensionalRange(CellRange range, int row, int column, int operandCount) {
			int topLeftColumn;
			int topLeftRow;
			int bottomRightColumn;
			int bottomRightRow;
			if ((row == 0 && (operandCount == 2|| range.Width == 1) || ( range.Height == 1 && column == 0))) {
				VariantValue result = new VariantValue();
				result.CellRangeValue = range;
				return result;
			}
			if (operandCount == 2) {
				if (range.Width == 1) {
					if (row > range.Height || column > range.Width)
						return VariantValue.ErrorReference;
					topLeftColumn = range.TopLeft.Column;
					topLeftRow = range.TopLeft.Row + (row != 0 ? row - 1 : row);
					bottomRightColumn = range.BottomRight.Column;
					bottomRightRow = range.TopLeft.Row + (row != 0 ? row - 1 : row);
				}
				else {
					if ((operandCount == 2 && row > range.Width) || (column > range.Width))
						return VariantValue.ErrorReference;
					topLeftColumn = range.TopLeft.Column + (row != 0 ? row - 1 : row);
					topLeftRow = range.TopLeft.Row;
					bottomRightColumn = range.TopLeft.Column + (row != 0 ? row - 1 : row);
					bottomRightRow = range.BottomRight.Row;
				}
			}
			else {
				if(row > range.Height || column>range.Width)
					return VariantValue.ErrorReference;
				topLeftColumn = range.TopLeft.Column + (column != 0 ? column - 1 : column);
				topLeftRow = range.TopLeft.Row + (row != 0 ? row - 1 : row);
				bottomRightColumn = range.TopLeft.Column + (column != 0 ? column - 1 : column);
				bottomRightRow = range.TopLeft.Row + (row != 0 ? row - 1 : row);
			}
			return GetValueFromCellRangeCore(topLeftColumn, topLeftRow, bottomRightColumn, bottomRightRow, range.Worksheet);
		}
		VariantValue GetResultFromTwoDimensionalRange(CellRange range, int row, int column, int operandCount) {
			int topLeftColumn;
			int topLeftRow;
			int bottomRightColumn;
			int bottomRightRow;
			if (row > range.Height || column > range.Width || operandCount == 2)
				return VariantValue.ErrorReference;
			if (row == 0) {
				topLeftRow = range.TopLeft.Row;
				topLeftColumn = range.TopLeft.Column + column - 1;
				bottomRightRow = range.BottomRight.Row;
				bottomRightColumn = range.TopLeft.Column + column - 1;
			}
			else if (column == 0) {
				topLeftRow = range.TopLeft.Row + row - 1;
				topLeftColumn = range.TopLeft.Column;
				bottomRightRow = range.TopLeft.Row + row - 1;
				bottomRightColumn = range.BottomRight.Column;
			}
			else {
				topLeftRow = range.TopLeft.Row + row - 1;
				topLeftColumn = range.TopLeft.Column + column - 1;
				bottomRightRow = range.TopLeft.Row + row - 1;
				bottomRightColumn = range.TopLeft.Column + column - 1;
			}
			return GetValueFromCellRangeCore(topLeftColumn, topLeftRow, bottomRightColumn, bottomRightRow, range.Worksheet);
		}
		VariantValue GetValueFromCellRangeCore(int topLeftColumn, int topLeftRow, int bottomRightColumn, int bottomRightRow, ICellTable worksheet) {
			CellPosition topLeft = new CellPosition(topLeftColumn, topLeftRow);
			CellPosition bottomRight = new CellPosition(bottomRightColumn, bottomRightRow);
			CellRange resultRange = new CellRange(worksheet, topLeft, bottomRight);
			VariantValue result = new VariantValue();
			result.CellRangeValue = resultRange;
			return result;
		}
		VariantValue GetValueFromArray(IList<VariantValue> arguments, WorkbookDataContext context, VariantValue value, int row, int column) {
			int operandCount = arguments.Count;
			VariantValue result;
			if (operandCount == 4) {
				VariantValue area = GetAreaCore(arguments, context, 1);
				if (area.IsError)
					return area;
			}
			if (row == 0 && column == 0)
				return value;
			IVariantArray array = value.ArrayValue;
			if ((array.Width > 1) && (array.Height > 1))
				result = GetResultFromTwoDimensionalArray(array, row, column, operandCount);
			else
				result = GetResultFromOneDimensionalArray(array, row, column, operandCount);
			return result;
		}
		VariantValue GetResultFromTwoDimensionalArray(IVariantArray array, int row, int column, int operandCount) {
			VariantValue result = new VariantValue();
			VariantArray resultArray = new VariantArray();
			if (row > array.Height || column > array.Width)
				return VariantValue.ErrorReference;
			if (row == 0) {
				resultArray.Values = new VariantValue[array.Height];
				List<VariantValue> list = new List<VariantValue>();
				for (int i = 0; i < array.Height; i++)
					list.Add(array.GetValue(i, column - 1));
				resultArray.SetValues(list, 1, (short)array.Height);
				result.ArrayValue = resultArray;
				return result;
			}
			if (column == 0 || operandCount == 2) {
				resultArray.Values = new VariantValue[array.Width];
				List<VariantValue> list = new List<VariantValue>();
				for (int i = 0; i < array.Width; i++)
					list.Add(array.GetValue(row - 1, i));
				resultArray.SetValues(list, (short)array.Width, 1);
				result.ArrayValue = resultArray;
				return result;
			}
			return array.GetValue(row - 1, column - 1);
		}
		VariantValue GetResultFromOneDimensionalArray(IVariantArray array, int row, int column, int operandCount) {
			if ((row == 0 && (operandCount == 2 || array.Width == 1) || (array.Height == 1 && column == 0))) {
				VariantValue result = new VariantValue();
				result.ArrayValue = array;
				return result;
			}
			if (operandCount == 2) {
				if (array.Width == 1) {
					if (row > array.Height || column > array.Width)
						return VariantValue.ErrorReference;
					return array.GetValue(row - 1, 0);
				}
				else {
					if ((operandCount == 2 && row > array.Width) || (column > array.Width))
						return VariantValue.ErrorReference;
					return array.GetValue(0, row - 1);
				}
			}
			else {
				if (row > array.Height || column > array.Width)
					return VariantValue.ErrorReference;
				return array.GetValue(row - 1, column - 1);
			}
		}
		VariantValue GetColumnIndex(IList<VariantValue> arguments, WorkbookDataContext context) {
			return (arguments.Count > 2 ? arguments[2].ToNumeric(context) : 1);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
