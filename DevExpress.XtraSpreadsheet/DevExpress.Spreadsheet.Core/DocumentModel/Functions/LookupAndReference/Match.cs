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
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionMatch
	public class FunctionMatch : FunctionLookupBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "MATCH"; } }
		public override int Code { get { return 0x0040; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue lookupValue = GetValidateLookupValue(arguments[0], context);
			if (lookupValue.IsError)
				return lookupValue;
			VariantValue lookupRange = arguments[1];
			if (lookupRange.IsError)
				return lookupRange;
			if (!lookupRange.IsCellRange && !lookupRange.IsArray) {
				if (lookupRange.IsNumeric || lookupRange.IsBoolean)
					return VariantValue.ErrorValueNotAvailable;
				else
					return VariantValue.ErrorInvalidValueInFunction;
			}
			if (lookupRange.IsCellRange && lookupRange.CellRangeValue.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorValueNotAvailable;
			int matchType = 1;
			if (arguments.Count == 3) {
				VariantValue value = GetValidateMatchType(arguments[2], context);
				if (value.IsError)
					return value;
				matchType = Math.Sign(value.NumericValue);
			}
			if (IsErrorRange(lookupRange))
				return VariantValue.ErrorValueNotAvailable;
			return PerformLookup(lookupValue, lookupRange, matchType, context);
		}
		bool IsErrorRange(VariantValue range) {
			if (range.IsCellRange)
				return (!IsVerticalDirection(range) && range.CellRangeValue.Height > 1) ||
					   (IsVerticalDirection(range) && range.CellRangeValue.Width > 1);
			if (range.IsArray)
				return (!IsVerticalDirection(range) && range.ArrayValue.Height > 1) ||
					   (IsVerticalDirection(range) && range.ArrayValue.Width > 1);
			return false;
		}
		VariantValue GetValidateMatchType(VariantValue matchType, WorkbookDataContext context) {
			if (matchType.IsError)
				return matchType;
			if (matchType.IsArray)
				if (matchType.ArrayValue.Count == 1)
					matchType = matchType.ArrayValue[0];
				else
					return VariantValue.ErrorReference;
			if (matchType.IsCellRange) {
				if (matchType.CellRangeValue.RangeType == CellRangeType.UnionRange)
					return VariantValue.ErrorReference;
				matchType = context.DereferenceValue(matchType, true);
			}
			return matchType.ToNumeric(context);
		}
		VariantValue PerformLookup(VariantValue lookupValue, VariantValue lookupRange, int matchType, WorkbookDataContext context) {
			bool isVerticalDirection = IsVerticalDirection(lookupRange);
			IVector<VariantValue> lookupVector = GetLookupVector(lookupRange, isVerticalDirection);
			return PerformLookup(lookupValue, lookupVector, matchType, context);
		}
		bool IsVerticalDirection(VariantValue lookupRange) {
			if (lookupRange.IsCellRange)
				return lookupRange.CellRangeValue.Height >= lookupRange.CellRangeValue.Width;
			else
				return lookupRange.ArrayValue.Height >= lookupRange.ArrayValue.Width;
		}
		IVector<VariantValue> GetLookupVector(VariantValue lookupRange, bool isVerticalDirection) {
			if (isVerticalDirection) {
				if (lookupRange.IsCellRange)
					return new RangeVerticalVector(lookupRange.CellRangeValue.GetFirstInnerCellRange(), 0);
				else
					return new ArrayVerticalVector(lookupRange.ArrayValue, 0);
			}
			else {
				if (lookupRange.IsCellRange)
					return new RangeHorizontalVector(lookupRange.CellRangeValue.GetFirstInnerCellRange(), 0);
				else
					return new ArrayHorizontalVector(lookupRange.ArrayValue, 0);
			}
		}
		VariantValue PerformLookup(VariantValue lookupValue, IVector<VariantValue> lookupVector, int matchType, WorkbookDataContext context) {
			bool useBinarySearch = (matchType != 0) ? true : false;
			int valueIndex;
			if (matchType == -1) 
				valueIndex = BinarySearchValue(lookupValue, lookupVector, context);
			else 
				valueIndex = LookupValue(lookupValue, lookupVector, useBinarySearch, context);
			if (valueIndex == ErrorGettingData)
				return VariantValue.ErrorGettingData;
			if (valueIndex < 0)
				return VariantValue.ErrorValueNotAvailable;
			if (valueIndex >= lookupVector.Count)
				return 0;
			if (useBinarySearch)
				if (!AreTypesMatch(lookupValue, lookupVector[valueIndex]))
					return VariantValue.ErrorValueNotAvailable;
			return valueIndex + 1;
		}
		int BinarySearchValue(VariantValue value, IVector<VariantValue> values, WorkbookDataContext context) {
			int index = Algorithms.BinarySearch(values, new InversedDefaultVariantValueComparable(value, context.StringTable));
			if (index < 0) {
				index = ~index;
				if (index >= values.Count)
					return values.Count - 1;
				index--;
			}
			return index;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
