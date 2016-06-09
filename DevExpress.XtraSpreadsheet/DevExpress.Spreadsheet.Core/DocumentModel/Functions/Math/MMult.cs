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
	#region FunctionMMult
	public class FunctionMMult : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "MMULT"; } }
		public override int Code { get { return 0x00A5; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Array; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue array1 = CalculateNumericValueArray(arguments[0], context);
			if (array1.IsError)
				return array1;
			VariantValue array2 = CalculateNumericValueArray(arguments[1], context);
			if (array2.IsError)
				return array2;
			if (array1.ArrayValue.Width != array2.ArrayValue.Height)
				return VariantValue.ErrorInvalidValueInFunction;
			return GetArrayResult(array1.ArrayValue, array2.ArrayValue);
		}
		VariantValue CalculateNumericValueArray(VariantValue value, WorkbookDataContext context) {
			if (value.IsError)
				return value;
			if (value.IsNumeric)
				return CalculateNumericValueArray(value.NumericValue);
			if (value.IsArray)
				return CalculateNumericValueArray(value.ArrayValue);
			if (value.IsCellRange)
				return CalculateNumericValueArray(value.CellRangeValue);
			return VariantValue.ErrorInvalidValueInFunction;
		}
		VariantValue CalculateNumericValueArray(double number) {
			VariantArray array = new VariantArray();
			array.Values = new VariantValue[] { number };
			array.Width = 1;
			array.Height = 1;
			VariantValue result = new VariantValue();
			result.ArrayValue = array;
			return result;
		}
		VariantValue CalculateNumericValueArray(IVariantArray array) {
			for (int i = 0; i < array.Height; i++)
				for (int j = 0; j < array.Width; j++) {
					VariantValue number = ToNumber(array.GetValue(i, j));
					if (number.IsError)
						return number;
				}
			VariantValue result = new VariantValue();
			result.ArrayValue = array;
			return result;
		}
		VariantValue CalculateNumericValueArray(CellRangeBase range) {
			VariantArray array = new VariantArray();
			if (range.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			CellRange cellRange = range.GetFirstInnerCellRange();
			int height = cellRange.Height;
			int width = cellRange.Width;
			array.Values = new VariantValue[height * width];
			array.Height = height;
			array.Width = width;
			for (int i = 0; i < height; i++)
				for (int j = 0; j < width; j++) {
					VariantValue number = ToNumber(cellRange.GetCellValueRelative(j, i));
					if (number.IsError)
						return number;
					array.SetValue(i, j, number);
				}
			VariantValue result = new VariantValue();
			result.ArrayValue = array;
			return result;
		}
		VariantValue ToNumber(VariantValue value) {
			if (value.IsNumeric || value.IsError)
				return value;
			return VariantValue.ErrorInvalidValueInFunction;
		}
		VariantValue GetArrayResult(IVariantArray array1, IVariantArray array2) {
			int height = array1.Height;
			int width = array2.Width;
			VariantArray arrayResult = new VariantArray();
			arrayResult.Values = new VariantValue[width * height];
			arrayResult.Height = height;
			arrayResult.Width = width;
			for (int i = 0; i < height; i++) 
				for (int j = 0; j < width; j++) { 
					double total = 0;
					for (int k = 0; k < array1.Width; k++) 
						total += array1.GetValue(i, k).NumericValue * array2.GetValue(k, j).NumericValue;
					arrayResult.SetValue(i, j, total);
				}
			VariantValue result = new VariantValue();
			result.ArrayValue = arrayResult;
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Array));
			return collection;
		}
	}
	#endregion
}
