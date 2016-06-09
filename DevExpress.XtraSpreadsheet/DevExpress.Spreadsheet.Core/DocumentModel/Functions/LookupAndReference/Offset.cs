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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionOffset
	public class FunctionOffset : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "OFFSET"; } }
		public override int Code { get { return 0x004E; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Reference; } }
		public override bool IsVolatile { get { return true; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			int operandCount = arguments.Count;
			VariantValue reference = arguments[0];
			if(reference.IsError)
				return reference;
			if (reference.CellRangeValue.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue rows = arguments[1].ToNumeric(context);
			if (rows.IsError)
				return rows;
			VariantValue cols = arguments[2].ToNumeric(context);
			if (cols.IsError)
				return cols;
			VariantValue height = reference.CellRangeValue.Height;
			if (operandCount > 3) {
				height = GetSizeArgument(arguments[3], context, height);
				if (height.IsError)
					return height;
			}
			VariantValue width = reference.CellRangeValue.Width;
			if (operandCount == 5) {
				width = GetSizeArgument(arguments[4], context, width);
				if (width.IsError)
					return width;
			}
			return GetRangeResult(reference, (int)rows.NumericValue, (int)cols.NumericValue, (int)height.NumericValue, (int)width.NumericValue);
		}
		VariantValue GetSizeArgument(VariantValue argument, WorkbookDataContext context, VariantValue referenceSize) {
			if (argument.IsEmpty)
				return referenceSize;
			argument = argument.ToNumeric(context);
			if (argument.IsError)
				return argument;
			if (argument.NumericValue == 0)
				return VariantValue.ErrorReference;
			if (Math.Abs(argument.NumericValue) < 1)
				return -2 * Math.Sign(argument.NumericValue);
			return argument;
		}
		VariantValue GetRangeResult(VariantValue reference, int rows, int cols, int height, int width) {
			CellRange range = reference.CellRangeValue.GetFirstInnerCellRange();
			int topLeftRow = range.TopLeft.Row + rows;
			int topLeftColumn = range.TopLeft.Column + cols;
			int bottomRightRow = topLeftRow + height - Math.Sign(height);
			int bottomRightColumn = topLeftColumn + width - Math.Sign(width);
			if (topLeftRow > bottomRightRow) {
				int row = topLeftRow;
				topLeftRow = bottomRightRow;
				bottomRightRow = row;
			}
			if (topLeftColumn > bottomRightColumn) {
				int column = topLeftColumn;
				topLeftColumn = bottomRightColumn;
				bottomRightColumn = column;
			}
			if (!IsValidateCellRange(topLeftRow, topLeftColumn, bottomRightRow, bottomRightColumn))
				return VariantValue.ErrorReference;
			VariantValue result = new VariantValue();
			result.CellRangeValue = new CellRange(reference.CellRangeValue.Worksheet, new CellPosition(topLeftColumn, topLeftRow), new CellPosition(bottomRightColumn, bottomRightRow));
			return result;
		}
		bool IsValidateCellRange(int topLeftRow, int topLeftColumn, int bottomRightRow, int bottomRightColumn) {
			return IndicesChecker.CheckIsRowIndexValid(topLeftRow) && 
				   IndicesChecker.CheckIsColumnIndexValid(topLeftColumn) &&
				   IndicesChecker.CheckIsRowIndexValid(bottomRightRow) && 
				   IndicesChecker.CheckIsColumnIndexValid(bottomRightColumn) &&
				   topLeftRow <= bottomRightRow && topLeftColumn <= bottomRightColumn;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
