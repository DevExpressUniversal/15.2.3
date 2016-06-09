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
	#region WorksheetStatisticalFunctionBase (abstract class)
	public abstract class WorksheetStatisticalFunctionBase : WorksheetFunctionBase {
		#region FillNumberArgumentsList
		protected VariantValue FillNumberArgumentsList(List<double> arguments, VariantValue set, VariantValue index, WorkbookDataContext context) {
			int maxCount = GetMaxCount(index);
			if (set.IsCellRange) {
				CellRangeBase cellRange = set.CellRangeValue;
				CalculationChain chain = cellRange.GetFirstInnerCellRange().Worksheet.Workbook.CalculationChain;
				foreach (CellBase cell in cellRange.GetExistingCellsEnumerable()) {
					VariantValue cellValue;
					if (cell == null)
						cellValue = VariantValue.Empty;
					else
						chain.TryGetCalculatedValue(cell, out cellValue);
					if (cellValue.IsError)
						return cellValue;
					if (cellValue.IsNumeric)
						AddValue(arguments, cellValue.NumericValue, maxCount);
				}
			}
			else if (set.IsArray)
				AddArrayValues(arguments, set, maxCount);
			else {
				VariantValue number = set.ToNumeric(context);
				if (number.IsError)
					return number;
				arguments = AddValue(arguments, number.NumericValue, maxCount);
			}
			if (!CheckIndex((int)index.NumericValue, arguments.Count))
				return VariantValue.ErrorNumber;
			return VariantValue.Empty;
		}
		#endregion
		protected virtual bool CheckIndex(int index, int argumentsCount) {
			return argumentsCount >= index;
		}
		protected virtual int GetMaxCount(VariantValue index) {
			return (int)index.NumericValue;
		}
		#region NthOrderNumber
		protected virtual double NthOrderNumber(List<double> arguments, VariantValue index) {
			return arguments[(int)index.NumericValue - 1];
		}
		#endregion
		#region AddValue
		List<double> AddValue(List<double> arguments, double value, int maxCount) {
			if (arguments.Count == 0) {
				arguments.Add(value);
				return arguments;
			}
			IComparer<double> comparer = GetComparer();
			int index = arguments.BinarySearch(value, comparer);
			if (index < 0)
				index = ~index;
			return AddValueCore(arguments, value, index, maxCount);
		}
		#endregion
		#region AddValueCore
		List<double> AddValueCore(List<double> arguments, double value, int index, int maxCount) {
			arguments.Insert(index, value);
			if (arguments.Count > maxCount)
				arguments.RemoveAt(arguments.Count - 1);
			return arguments;
		}
		#endregion
		#region AddArrayValues
		List<double> AddArrayValues(List<double> arguments, VariantValue value, int maxCount) {
			IVariantArray array = value.ArrayValue;
			for (int column = 0; column < array.Width; column++) {
				for (int row = 0; row < array.Height; row++) {
					VariantValue number = array.GetValue(row, column);
					if (number.IsNumeric)
						arguments = AddValue(arguments, number.NumericValue, maxCount);
				}
			}
			return arguments;
		}
		#endregion
		protected abstract IComparer<double> GetComparer();
		#region GetNthOrderResult
		protected VariantValue GetNthOrderResult(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue set = arguments[0];
			if (set.IsError)
				return set;
			VariantValue indexValue = arguments[1];
			if (indexValue.IsError)
				return indexValue;
			if (!indexValue.IsArray)
				return GetResultNthOrderCore(context, set, indexValue);
			else {
				int width = indexValue.ArrayValue.Width;
				int height = indexValue.ArrayValue.Height;
				VariantArray resultArray = VariantArray.Create(width, height);
				for (int i = 0; i < width; i++)
					for (int j = 0; j < height; j++)
						resultArray.SetValue(j, i, GetResultNthOrderCore(context, set, indexValue.ArrayValue.GetValue(j, i)));
				return VariantValue.FromArray(resultArray);
			}
		}
		#endregion
		#region GetResultNthOrderCore
		protected VariantValue GetResultNthOrderCore(WorkbookDataContext context, VariantValue set, VariantValue indexValue) {
			VariantValue index = GetNumericValue(indexValue, context);
			if (index.IsError)
				return index;
			if (index.NumericValue <= 0)
				return VariantValue.ErrorNumber;
			List<double> arguments = new List<double>();
			VariantValue operationStatus = FillNumberArgumentsList(arguments, set, index, context);
			if (operationStatus.IsError)
				return operationStatus;
			return NthOrderNumber(arguments, index);
		}
		#endregion
	}
	#endregion
	#region PivotTop10SumFilter
	public abstract class PivotTop10FilterBase : WorksheetStatisticalFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			return GetNthOrderResult(arguments, context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
		#region NthOrderNumber
		protected override double NthOrderNumber(List<double> arguments, VariantValue index) {
			double sum = 0;
			int i = 0;
			double threshold = index.NumericValue;
			int valuesCount = arguments.Count;
			while (sum < threshold && i < valuesCount) {
				sum += arguments[i];
				i++;
			}
			if (i == 0)
				return arguments[0];
			return arguments[i - 1];
		}
		protected override int GetMaxCount(VariantValue index) {
			return int.MaxValue;
		}
		protected override bool CheckIndex(int index, int argumentsCount) {
			return true;
		}
		#endregion
	}
	public class PivotTop10SumFilterLarge : PivotTop10FilterBase {
		public override string Name { get { return "PIVOTTOP10SUMFILTER_LARGE"; } }
		public override int Code { get { return 0x4104; } }
		protected override IComparer<double> GetComparer() {
			return new LargeNumberComparer();
		}
	}
	public class PivotTop10SumFilterSmall : PivotTop10FilterBase {
		public override string Name { get { return "PIVOTTOP10SUMFILTER_SMALL"; } }
		public override int Code { get { return 0x4105; } }
		protected override IComparer<double> GetComparer() {
			return new SmallNumberComparer();
		}
	}
	public abstract class PivotTop10PercentFilter : PivotTop10FilterBase {
		protected override double NthOrderNumber(List<double> arguments, VariantValue index) {
			double currentSum = 0;
			int i = 0;
			double threshold = index.NumericValue;
			int valuesCount = arguments.Count;
			double totalSum = GetTotalSum(arguments);
			while (currentSum < threshold && i < valuesCount) {
				currentSum += arguments[i] / totalSum;
				i++;
			}
			if (i == 0)
				return arguments[0];
			return arguments[i - 1];
		}
		double GetTotalSum(List<double> arguments) {
			double result = 0;
			for (int i = 0; i < arguments.Count; i++)
				result += arguments[i];
			return result;
		}
	}
	public class PivotTop10PercentFilterLarge : PivotTop10PercentFilter {
		public override string Name { get { return "PIVOTTOP10PERCENTFILTER_LARGE"; } }
		public override int Code { get { return 0x4106; } }
		protected override IComparer<double> GetComparer() {
			return new LargeNumberComparer();
		}
	}
	public class PivotTop10PercentFilterSmall : PivotTop10PercentFilter {
		public override string Name { get { return "PIVOTTOP10PERCENTFILTER_SMALL"; } }
		public override int Code { get { return 0x4107; } }
		protected override IComparer<double> GetComparer() {
			return new SmallNumberComparer();
		}
	}
	#endregion
}
