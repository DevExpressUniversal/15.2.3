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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace DevExpress.XtraSpreadsheet.Model {
	public abstract class FunctionLookupBase : WorksheetFunctionBase {
		protected int ErrorGettingData { get { return -2147400000; } }
		protected VariantValue GetValidateLookupValue(VariantValue lookupValue, WorkbookDataContext context) {
			if (lookupValue.IsError)
				return lookupValue;
			lookupValue = context.DereferenceValue(lookupValue, true);
			return lookupValue;
		}
		protected VariantValue PerformLookup(VariantValue lookupValue, IVector<VariantValue> lookupVector, IVector<VariantValue> resultVector, VariantValue secondaryDimensionIndex, bool useBinarySearch, WorkbookDataContext context) {
			int valueIndex = LookupValue(lookupValue, lookupVector, useBinarySearch, context);
			if (valueIndex == ErrorGettingData)
				return VariantValue.ErrorGettingData;
			if (valueIndex < 0) {
				if (context.TransitionFormulaEvaluation)
					return VariantValue.ErrorInvalidValueInFunction;
				else
					return VariantValue.ErrorValueNotAvailable;
			}
			if (secondaryDimensionIndex.IsError) 
				return secondaryDimensionIndex;
			if (useBinarySearch) {
				if (!AreTypesMatch(lookupValue, lookupVector[valueIndex]))
					return VariantValue.ErrorValueNotAvailable;
			}
			return resultVector[valueIndex];
		}
		protected bool AreTypesMatch(VariantValue lookupValue, VariantValue result) {
			bool isNumericFirst = lookupValue.IsNumeric;
			bool isNumericSecond = result.IsNumeric;
			return (lookupValue.IsText && result.IsText) || (isNumericFirst && isNumericSecond) || (lookupValue.Type == result.Type);
		}
		protected int LookupValue(VariantValue value, IVector<VariantValue> values, bool useBinarySearch, WorkbookDataContext context) {
			if (useBinarySearch) {
				if (context.TransitionFormulaEvaluation)
					return LotusBinarySearchValue(value, values, context);
				else
					return BinarySearchValue(value, values, context);
			}
			else
				return SearchValue(value, values, context, true);
		}
		int LotusBinarySearchValue(VariantValue value, IVector<VariantValue> values, WorkbookDataContext context) {
			if (value.IsText)
				return SearchValue(value, values, context, false);
			int count = values.Count;
			for (int i = 0; i < count; i++) {
				VariantValue item = values[i];
				if (item == VariantValue.ErrorGettingData)
					return ErrorGettingData;
				int compareResult = context.Compare(value, item);
				if (compareResult == 0)
					return i;
				if (compareResult < 0)
					return i - 1;
			}
			return count - 1;
		}
		int BinarySearchValue(VariantValue value, IVector<VariantValue> values, WorkbookDataContext context) {
			DefaultVariantValueComparable predicate = new DefaultVariantValueComparable(value, context.StringTable);
			int index = BinarySearchNoTypeConversion(value, values, predicate);
			if (index == ErrorGettingData)
				return ErrorGettingData;
			if (index >= 0) {
				for (int i = index + 1; i < values.Count; i++) {
					VariantValue currentValue = values[i];
					if (currentValue == VariantValue.ErrorGettingData)
						return ErrorGettingData;
					if (CompareValuesStrictTypeMatch(value, currentValue, predicate) == 0)
						index = i;
				}
			}
			if (index < 0) {
				index = ~index;
				if (index >= values.Count)
					return values.Count - 1;
				index--;
			}
			return index;
		}
		int BinarySearchNoTypeConversion(VariantValue value, IVector<VariantValue> values, IComparable<VariantValue> predicate) {
			int low = 0;
			int hi = values.Count - 1;
			while (low <= hi) {
				int median = (low + hi) / 2;
				VariantValue medianValue = values[median];
				if (medianValue == VariantValue.ErrorGettingData)
					return ErrorGettingData;
				int compareResult = CompareValuesStrictTypeMatch(value, medianValue, predicate);
				if (compareResult == 0) {
					low = ~median;
					break;
				}
				if (compareResult == Int32.MinValue) { 
					int highMedian = median;
					int highCompareResult = compareResult;
					int i;
					for (i = median + 1; i <= hi; i++) {
						VariantValue currentValue = values[i];
						if (currentValue == VariantValue.ErrorGettingData)
							return ErrorGettingData;
						highCompareResult = CompareValuesStrictTypeMatch(value, currentValue, predicate);
						if (highCompareResult != Int32.MinValue) {
							highMedian = i;
							break;
						}
					}
					if (highCompareResult == 0) {
						low = ~highMedian;
						break;
					}
					if (i > hi) { 
						hi = median - 1;
						continue;
					}
					Debug.Assert(highCompareResult != Int32.MinValue);
					compareResult = highCompareResult;
					median = highMedian;
				}
				if (compareResult < 0)
					low = median + 1;
				else
					hi = median - 1;
			}
			return ~low;
		}
		int CompareValuesStrictTypeMatch(VariantValue value, VariantValue other, IComparable<VariantValue> predicate) {
			if (value.InversedSortType != other.InversedSortType)
				return Int32.MinValue;
			return predicate.CompareTo(other);
		}
		int SearchValue(VariantValue value, IVector<VariantValue> values, WorkbookDataContext context, bool allowWildcards) {
			if (value.IsText) {
				string textValue = value.GetTextValue(context.StringTable);
				if (allowWildcards && WildcardComparer.IsWildcard(textValue))
					return SearchValueWithWildcards(textValue, values, context);
				else
					return SearchValueCore(value, values, context);
			}
			else
				return SearchValueCore(value, values, context);
		}
		int GetVectorEffectiveCount(IVector<VariantValue> values) {
			RangeVerticalVector vector = values as RangeVerticalVector;
			if (vector == null)
				return values.Count;
			IRowCollection rows = vector.Range.Worksheet.Rows as IRowCollection;
			if (rows == null || rows.Count <= 0)
				return values.Count;
			return Math.Min(Math.Max(0, rows.Last.Index - vector.Range.TopLeft.Row + 1), values.Count);
		}
		int SearchValueWithWildcards(string textValue, IVector<VariantValue> values, WorkbookDataContext context) {
			Regex regex = WildcardComparer.CreateWildcardRegex(textValue);
			int count = GetVectorEffectiveCount(values); 
			for (int i = 0; i < count; i++) {
				VariantValue value = values[i];
				if (value == VariantValue.ErrorGettingData)
					return ErrorGettingData;
				if (value.IsText) {
					string text = value.GetTextValue(context.StringTable);
					if (!String.IsNullOrEmpty(text) && text.Length > 255)
						return -1;
					if (WildcardComparer.Match(regex, text))
						return i;
				}
			}
			return -1;
		}
		int SearchValueCore(VariantValue value, IVector<VariantValue> values, WorkbookDataContext context) {
			LookupCalculationInfo calculationInfo = context.Workbook.CalculationChain.CalculationHash.GetLookupCalculationInfo(values);
			return calculationInfo.SearchValue(value);
		}
	}
	public abstract class FunctionOrderedLookupBase : FunctionLookupBase {
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue lookupValue = GetValidateLookupValue(arguments[0], context);
			if (lookupValue.IsError)
				return lookupValue;
			VariantValue range = arguments[1];
			if (range.IsError)
				return range;
			if (!range.IsCellRange && !range.IsArray) {
				if (range.IsText)
					return VariantValue.ErrorInvalidValueInFunction;
				else
					return VariantValue.ErrorValueNotAvailable;
			}
			if (range.IsCellRange && range.CellRangeValue.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorValueNotAvailable;
			VariantValue secondaryDimensionIndex = arguments[2];
			if (secondaryDimensionIndex.IsError)
				return secondaryDimensionIndex;
			if (secondaryDimensionIndex.IsCellRange && secondaryDimensionIndex.CellRangeValue.RangeType == CellRangeType.UnionRange)
				secondaryDimensionIndex = VariantValue.ErrorReference;
			else {
				secondaryDimensionIndex = secondaryDimensionIndex.ToNumeric(context);
				if (secondaryDimensionIndex == VariantValue.ErrorGettingData)
					return secondaryDimensionIndex;
				if (secondaryDimensionIndex.IsError)
					secondaryDimensionIndex = VariantValue.ErrorReference;
				else
					if (secondaryDimensionIndex.NumericValue < 1)
						secondaryDimensionIndex = VariantValue.ErrorInvalidValueInFunction;
			}
			VariantValue rangeLookup;
			if (arguments.Count == 4)
				rangeLookup = arguments[3].ToBoolean(context);
			else
				rangeLookup = true;
			if (rangeLookup.IsError)
				return rangeLookup;
			if (rangeLookup.IsCellRange && rangeLookup.CellRangeValue.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorValueNotAvailable;
			return PerformLookup(lookupValue, range, secondaryDimensionIndex, rangeLookup.BooleanValue, context);
		}
		VariantValue PerformLookup(VariantValue lookupValue, VariantValue range, VariantValue secondaryDimensionIndex, bool useBinarySearch, WorkbookDataContext context) {
			int secondaryDimensionIndexNum = (int)secondaryDimensionIndex.NumericValue - 1;
			int dimension = CalculateSecondaryDimension(range);
			if (secondaryDimensionIndexNum >= dimension)
				secondaryDimensionIndex = VariantValue.ErrorReference;
			IVector<VariantValue> lookupVector = GetLookupVector(range);
			IVector<VariantValue> resultVector = GetResultVector(range, secondaryDimensionIndexNum);
			return PerformLookup(lookupValue, lookupVector, resultVector, secondaryDimensionIndex, useBinarySearch, context);
		}
		protected internal abstract int CalculateSecondaryDimension(VariantValue tableArray);
		protected internal abstract IVector<VariantValue> GetLookupVector(VariantValue tableArray);
		protected internal abstract IVector<VariantValue> GetResultVector(VariantValue tableArray, int secondaryDirectionIndex);
	}
	#region LookupCalculationInfo
	public class LookupCalculationInfo {
		const int ErrorGettingData = -2147400000;
		readonly IVector<VariantValue> values;
		readonly Dictionary<VariantValue, int> hash;
		readonly WorkbookDataContext context;
		readonly int effectiveCount;
		int lastIndex = -1;
		public LookupCalculationInfo(IVector<VariantValue> values, WorkbookDataContext context) {
			this.hash = new Dictionary<VariantValue, int>(new LookupValuesEqualityComparer(context));
			this.values = values;
			this.context = context;
			this.effectiveCount = GetVectorEffectiveCount(values);
		}
		public IVector<VariantValue> Values { get { return values; } }
		public int SearchValue(VariantValue value) {
			int index;
			if (hash.TryGetValue(value, out index))
				return index;
			if (lastIndex >= effectiveCount)
				return -1;
			return SearchValueCore(value);
		}
		public int SearchValueCore(VariantValue value) {
			int count = Values.Count;
			if (count == effectiveCount)
				return SearchValueCore(value, Values, lastIndex + 1, count - 1, context);
			else {
				int result = SearchValueCore(value, Values, lastIndex + 1, effectiveCount - 1, context);
				if (result == -1)
					result = SearchValueCore(value, Values, effectiveCount, effectiveCount, context); 
				return result;
			}
		}
		int GetVectorEffectiveCount(IVector<VariantValue> values) {
			RangeVerticalVector vector = values as RangeVerticalVector;
			if (vector == null)
				return values.Count;
			IRowCollection rows = vector.Range.Worksheet.Rows as IRowCollection;
			if (rows == null || rows.Count <= 0)
				return values.Count;
			return Math.Min(Math.Max(0, rows.Last.Index - vector.Range.TopLeft.Row + 1), values.Count);
		}
		int SearchValueCore(VariantValue value, IVector<VariantValue> values, int from, int to, WorkbookDataContext context) {
			SharedStringTable stringTable = context.StringTable;
			for (int i = from; i <= to; i++) {
				VariantValue currentValue = values[i];
				if (!hash.ContainsKey(currentValue))
					hash.Add(currentValue, i);
				if (currentValue == VariantValue.ErrorGettingData)
					return ErrorGettingData;
				if (currentValue.IsEqual(value, StringComparison.CurrentCultureIgnoreCase, stringTable)) {
					lastIndex = i;
					return i;
				}
			}
			lastIndex = to;
			return -1;
		}
	}
	#endregion
	#region LookupValuesEqualityComparer
	public class LookupValuesEqualityComparer : IEqualityComparer<VariantValue> {
		readonly WorkbookDataContext context;
		public LookupValuesEqualityComparer(WorkbookDataContext context) {
			this.context = context;
		}
		public bool Equals(VariantValue x, VariantValue y) {
			return x.IsEqual(y, StringComparison.CurrentCultureIgnoreCase, context.StringTable);
		}
		public int GetHashCode(VariantValue obj) {
			if (obj.IsText)
				obj = obj.GetTextValue(context.StringTable).ToLower(context.Culture);
			else
				if (obj.IsEmpty)
					obj = 0;
			return obj.GetHashCode();
		}
	}
	#endregion
}
