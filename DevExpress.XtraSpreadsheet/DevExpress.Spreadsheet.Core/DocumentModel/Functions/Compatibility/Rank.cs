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
	#region FunctionRank
	public class FunctionRank : WorksheetGenericFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "RANK"; } }
		public override int Code { get { return 0x00D8; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue number = arguments[0].ToNumeric(context);
			if (number.IsError)
				return number;
			VariantValue referenceRange = arguments[1];
			if (referenceRange.IsError)
				return referenceRange;
			CellRangeBase referenceCellRange = referenceRange.CellRangeValue;
			VariantValue orderSort = 0;
			if (arguments.Count == 3) {
				if (arguments[2].IsInlineText)
					return VariantValue.ErrorInvalidValueInFunction;
				orderSort = arguments[2].ToNumeric(context);
				if (orderSort.IsError)
					return orderSort;
			}
			FunctionRankResult result = CreateInitialFunctionResult(context) as FunctionRankResult;
			result.Number = number.NumericValue;
			result.IsAscendingSortOrder = orderSort.NumericValue == 0;
			return EvaluateCellRange(referenceCellRange, result);
		}
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionRankResult(context);
		}
	}
	#endregion
	#region FunctionRankResult
	public class FunctionRankResult : FunctionSumResultBase {
		int numbersCount;
		int result = 1;
		bool isContainsNumber;
		bool isAscendingSortOrder;
		double number;
		public FunctionRankResult(WorkbookDataContext context)
			: base(context) {
		}
		internal double Number { get { return number; } set { number = value; } }
		internal bool IsAscendingSortOrder { get { return isAscendingSortOrder; } set { isAscendingSortOrder = value; } }
		public override bool ProcessConvertedValue(VariantValue value) {
			numbersCount++;
			double doubleValue = value.NumericValue;
			if (!isContainsNumber && doubleValue == Number)
				isContainsNumber = true;
			if (IsAscendingSortOrder && doubleValue > Number)
				result++;
			if (!IsAscendingSortOrder && doubleValue < Number)
				result++;
			return true;
		}
		public override VariantValue GetFinalValue() {
			if (!isContainsNumber || numbersCount == 0)
				return VariantValue.ErrorValueNotAvailable;
			return result;
		}
	}
	#endregion
}
