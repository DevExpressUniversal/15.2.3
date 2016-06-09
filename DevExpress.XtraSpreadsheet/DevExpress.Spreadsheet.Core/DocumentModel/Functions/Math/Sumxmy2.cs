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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionSumxmy2
	public class FunctionSumXMY2 : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "SUMXMY2"; } }
		public override int Code { get { return 0x012F; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue array1 = arguments[0];
			if (array1.IsError)
				return array1;
			VariantValue array2 = arguments[1];
			if (array2.IsError)
				return array2;
			return GetNumericResult(array1, array2);
		}
		VariantValue GetNumericResult(VariantValue array1, VariantValue array2) {
			if (array1.IsNumeric && array2.IsNumeric)
				return VariantValue.ErrorDivisionByZero;
			if (!(array1.IsArray || array1.IsCellRange) || !(array2.IsArray || array2.IsCellRange))
				return VariantValue.ErrorInvalidValueInFunction;
			IVector<VariantValue> firstArray, secondArray;
			if (array1.IsArray)
				firstArray = new ArrayZVector(array1.ArrayValue);
			else {
				if (array1.CellRangeValue.RangeType == CellRangeType.UnionRange)
					return VariantValue.ErrorInvalidValueInFunction;
				firstArray = new RangeZVector(array1.CellRangeValue.GetFirstInnerCellRange());
			}
			if (array2.IsArray)
				secondArray = new ArrayZVector(array2.ArrayValue);
			else {
				if (array2.CellRangeValue.RangeType == CellRangeType.UnionRange)
					return VariantValue.ErrorInvalidValueInFunction;
				secondArray = new RangeZVector(array2.CellRangeValue.GetFirstInnerCellRange());
			}
			if (firstArray.Count != secondArray.Count)
				return VariantValue.ErrorValueNotAvailable;
			return GetSumxmy2(firstArray, secondArray);
		}
		VariantValue GetSumxmy2(IVector<VariantValue> firstArray, IVector<VariantValue> secondArray) {
			double result = 0;
			int count = 0;
			for (int i = 0; i < firstArray.Count; i++) {
				VariantValue value1 = firstArray[i];
				VariantValue value2 = secondArray[i];
				if (value1.IsError)
					return value1;
				if (value2.IsError)
					return value2;
				if (value1.IsNumeric && value2.IsNumeric) {
					result += (value1.NumericValue - value2.NumericValue) * (value1.NumericValue - value2.NumericValue);
					count++;
				}
			}
			if (count == 0)
				return VariantValue.ErrorDivisionByZero;
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Array));
			return collection;
		}
	#endregion
	}
}
