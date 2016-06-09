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
	#region FunctionChisqDotTest
	public class FunctionChisqDotTest : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Array | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Array | OperandDataType.Reference));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "CHISQ.TEST"; } }
		public override int Code { get { return 0x4069; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue actualRange = arguments[0];
			if (actualRange.IsError)
				return actualRange;
			if (IsInvalid(actualRange))
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue expectedRange = arguments[1];
			if (expectedRange.IsError)
				return expectedRange;
			if (IsInvalid(expectedRange))
				return VariantValue.ErrorInvalidValueInFunction;
			if (actualRange.IsNumeric || expectedRange.IsNumeric)
				return VariantValue.ErrorValueNotAvailable;
			return GetResult(actualRange, expectedRange, context);
		}
		bool IsInvalid(VariantValue value) {
			bool isUnionRange = value.IsCellRange && value.CellRangeValue.RangeType == CellRangeType.UnionRange;
			return !(value.IsArray || value.IsCellRange || value.IsNumeric) || isUnionRange;
		}
		VariantValue GetResult(VariantValue actualRange, VariantValue expectedRange, WorkbookDataContext context) {
			IVector<VariantValue> actualVector = PrepareVector(actualRange);
			IVector<VariantValue> expectedVector = PrepareVector(expectedRange);
			int actualVectorCount = actualVector.Count;
			int expectedVectorCount = expectedVector.Count;
			if (actualVectorCount < 2 || expectedVectorCount < 2 || actualVectorCount != expectedVectorCount)
				return VariantValue.ErrorValueNotAvailable;
			double chi = CalculateChi(actualVector, expectedVector, actualVectorCount);
			if (chi == Double.MinValue)
				return VariantValue.ErrorDivisionByZero;
			if (chi == double.MaxValue)
				return VariantValue.ErrorGettingData;
			if (chi < 0)
				return VariantValue.ErrorNumber; 
			int degreeFreedom = actualRange.IsArray ? actualVectorCount - 1 : GetDegreeFreedom(actualRange.CellRangeValue);
			return chi == 0 ? 1 : 1 - FunctionChisqDotDist.GetResult(chi, degreeFreedom, true);
		}
		int GetDegreeFreedom(CellRangeBase range) {
			int width = range.Width;
			int height = range.Height;
			if (width == 1)
				return height - 1;
			if (height == 1)
				return width - 1;
			return (width - 1) * (height - 1);
		}
		IVector<VariantValue> PrepareVector(VariantValue value) {
			if (value.IsArray)
				return new ArrayZVector(value.ArrayValue);
			return new RangeZVector(value.CellRangeValue.GetFirstInnerCellRange());
		}
		double CalculateChi(IVector<VariantValue> actualRange, IVector<VariantValue> expectedRange, int count) {
			int errorValuesCount = 0;
			double result = 0;
			for (int i = 0; i < count; i++) {
				VariantValue actualRangeValue = actualRange[i];
				VariantValue expectedRangeValue = expectedRange[i];
				if (actualRangeValue == VariantValue.ErrorGettingData || expectedRangeValue == VariantValue.ErrorGettingData)
					return double.MaxValue;
				if (actualRangeValue.IsNumeric && expectedRangeValue.IsNumeric) {
					double actualRangeNumber = actualRangeValue.NumericValue;
					double expectedRangeNumber = expectedRangeValue.NumericValue;
					if (expectedRangeNumber == 0)
						return Double.MinValue;
					result += (actualRangeNumber - expectedRangeNumber) * (actualRangeNumber - expectedRangeNumber) / expectedRangeNumber;
				}
				else
					errorValuesCount++;
			}
			return errorValuesCount == count ? Double.MinValue : result;
		}
	}
	#endregion
}
