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
using System.Numerics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionGcd
	public class FunctionGcd : WorksheetGenericFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FakeParameterType.Number));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited, FakeParameterType.Number));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "GCD"; } }
		public override int Code { get { return 0x01D9; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionGcdResult(context);
		}
	}
	#endregion
	#region FunctionGcdResult
	public class FunctionGcdResult : FunctionResult {
		#region Fields
		double result;
		bool hasOnlyEmptyCells = true;
		bool isFirstValue = true;
		#endregion
		public FunctionGcdResult(WorkbookDataContext context)
			: base(context) {
		}
		#region Properties
		protected double Result { get { return result; } set { result = value; } }
		protected bool HasOnlyEmptyCells { get { return hasOnlyEmptyCells; } set { hasOnlyEmptyCells = value; } }
		#endregion
		public override bool ShouldProcessValueCore(VariantValue value) {
			if (value.IsCellRange && value.CellRangeValue.CellCount > 1)
				FixEmptyCellsError();
			if (CellRangeProcessing && value.IsEmpty) {
				if (hasOnlyEmptyCells)
					Error = VariantValue.ErrorInvalidValueInFunction;
				return false;
			}
			return true;
		}
		protected void FixEmptyCellsError() {
			if (hasOnlyEmptyCells) {
				hasOnlyEmptyCells = false;
				if (Error != VariantValue.ErrorNumber)
					Error = VariantValue.Empty;
			}
		}
		public override VariantValue ConvertValue(VariantValue value) {
			if (value.IsBoolean || (CellRangeProcessing && value.IsText))
				return VariantValue.ErrorInvalidValueInFunction;
			if (!ArrayOrRangeProcessing && isFirstValue && value.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			return value.ToNumeric(Context);
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			double current = Math.Floor(value.NumericValue);
			isFirstValue = false;
			FixEmptyCellsError();
			if (Error == VariantValue.ErrorNumber)
				return true;
			if (current < 0)
				Error = VariantValue.ErrorNumber;
			else
				result = GetNextValue(current);
			return true;
		}
		protected virtual double GetNextValue(double value) {
			if (result == 0 && value == 0)
				return 0;
			if (result == value)
				return 1;
			if (result <= 1)
				return value;
			if (value <= 1)
				return result;
			return GetGreatestCommonDivisor(value);
		}
		protected double GetGreatestCommonDivisor(double value) {
			BigInteger first = (BigInteger)result;
			BigInteger second = (BigInteger)value;
			return (double)BigInteger.GreatestCommonDivisor(first, second);
		}
		public override VariantValue GetFinalValue() {
			return result;
		}
	}
	#endregion
}
