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
using DevExpress.XtraSpreadsheet.Utils;
using System.Text.RegularExpressions;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionLookup
	public class FunctionLookup : FunctionLookupBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "LOOKUP"; } }
		public override int Code { get { return 0x001C; } }
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
				if (lookupRange.IsNumeric)
					return VariantValue.ErrorValueNotAvailable;
				else
					return VariantValue.ErrorInvalidValueInFunction;
			}
			if (lookupRange.IsCellRange && lookupRange.CellRangeValue.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorValueNotAvailable;
			if (arguments.Count == 3) {
				VariantValue resultRange = arguments[2];
				if (resultRange.IsError)
					return resultRange;
				if (!resultRange.IsCellRange && !resultRange.IsArray) {
					if (resultRange.IsNumeric)
						return VariantValue.ErrorValueNotAvailable;
					else
						return VariantValue.ErrorInvalidValueInFunction;
				}
				if (resultRange.IsCellRange && resultRange.CellRangeValue.RangeType == CellRangeType.UnionRange)
					return VariantValue.ErrorValueNotAvailable;
				return PerformLookup(lookupValue, lookupRange, resultRange, context);
			}
			else
				return PerformLookup(lookupValue, lookupRange, context);
		}
		VariantValue PerformLookup(VariantValue lookupValue, VariantValue lookupRange, WorkbookDataContext context) {
			bool isVerticalDirection = IsVerticalDirection(lookupRange);
			IVector<VariantValue> lookupVector = GetLookupVector(lookupRange, isVerticalDirection);
			IVector<VariantValue> resultVector = GetResultVector(lookupRange, isVerticalDirection);
			return PerformLookup(lookupValue, lookupVector, resultVector, VariantValue.Empty, true, context);
		}
		VariantValue PerformLookup(VariantValue lookupValue, VariantValue lookupRange, VariantValue resultRange, WorkbookDataContext context) {
			if (resultRange.IsCellRange) {
				if (resultRange.CellRangeValue.Width != 1 && resultRange.CellRangeValue.Height != 1)
					return VariantValue.ErrorValueNotAvailable;
			}
			else { 
				if (resultRange.ArrayValue.Width != 1 && resultRange.ArrayValue.Height != 1)
					return VariantValue.ErrorValueNotAvailable;
			}
			IVector<VariantValue> lookupVector = GetLookupVector(lookupRange, IsVerticalDirection(lookupRange));
			IVector<VariantValue> resultVector = GetResultVector(resultRange, IsVerticalDirection(resultRange));
			return PerformLookup(lookupValue, lookupVector, resultVector, VariantValue.Empty, true, context);
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
		IVector<VariantValue> GetResultVector(VariantValue resultRange, bool isVerticalDirection) {
			if (isVerticalDirection) {
				if (resultRange.IsCellRange)
					return new RangeVerticalVector(resultRange.CellRangeValue.GetFirstInnerCellRange(), resultRange.CellRangeValue.Width - 1);
				else
					return new ArrayVerticalVector(resultRange.ArrayValue, resultRange.ArrayValue.Width - 1);
			}
			else {
				if (resultRange.IsCellRange)
					return new RangeHorizontalVector(resultRange.CellRangeValue.GetFirstInnerCellRange(), resultRange.CellRangeValue.Height - 1);
				else
					return new ArrayHorizontalVector(resultRange.ArrayValue, resultRange.ArrayValue.Height - 1);
			}
		}
		bool IsVerticalDirection(VariantValue lookupRange) {
			if (lookupRange.IsCellRange)
				return lookupRange.CellRangeValue.Height >= lookupRange.CellRangeValue.Width;
			else
				return lookupRange.ArrayValue.Height >= lookupRange.ArrayValue.Width;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Array, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
