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

using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionMode
	public class FunctionMode : WorksheetGenericFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "MODE"; } }
		public override int Code { get { return 0x014A; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionModeResult(context);
		}
	}
	#endregion
	#region FunctionModeResult
	public class FunctionModeResult : FunctionSumResultBase {
		readonly Dictionary<double, int> numbers;
		bool containsNumbers;
		int currentArraySeveralNumbers;
		public FunctionModeResult(WorkbookDataContext context)
			: base(context) {
			this.numbers = new Dictionary<double, int>();
			ProcessErrorValues = true;
		}
		protected Dictionary<double, int> Numbers { get { return numbers; } }
		public override void BeginArrayProcessingCore(long itemsCount) {
			containsNumbers = false;
			currentArraySeveralNumbers = 0;
		}
		public override bool EndArrayProcessingCore() {
			if (Error.IsError)
				return false;
			if (!containsNumbers && currentArraySeveralNumbers <= 1) {
				Error = VariantValue.ErrorInvalidValueInFunction;
				return false;
			}
			return true;
		}
		public override bool ShouldProcessValueCore(VariantValue value) {
			if (!Error.IsEmpty)
				return false;
			if (value.IsError) {
				Error = value;
				return ArrayOrRangeProcessing;
			}
			if (value.IsCellRange && value.CellRangeValue.RangeType == CellRangeType.UnionRange) {
				Error = VariantValue.ErrorInvalidValueInFunction;
				return false;
			}
			if (ArrayOrRangeProcessing) {
				if (currentArraySeveralNumbers <= 1)
					currentArraySeveralNumbers++;
				return value.IsNumeric;
			}
			return !value.IsError;
		}
		public override VariantValue ConvertValue(VariantValue value) {
			if (value.IsBoolean) 
				return VariantValue.ErrorInvalidValueInFunction;
			return base.ConvertValue(value);
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			if (ArrayOrRangeProcessing)
				containsNumbers = true;
			AddValue(value.NumericValue);
			return true;
		}
		void AddValue(double value) {
			if (numbers.ContainsKey(value))
				numbers[value]++;
			else
				numbers.Add(value, 1);
		}
		public override VariantValue GetFinalValue() {
			int maxOccurenceCount = 0;
			double maxOccurenceValue = 0;
			foreach (KeyValuePair<double, int> pair in numbers) {
				if (pair.Value > maxOccurenceCount) {
					maxOccurenceCount = pair.Value;
					maxOccurenceValue = pair.Key;
				}
			}
			if (maxOccurenceCount < 2)
				return VariantValue.ErrorValueNotAvailable;
			return ModeCore(maxOccurenceCount, maxOccurenceValue);
		}
		protected virtual VariantValue ModeCore(int maxOccurenceCount, double maxOccurenceValue) {
			return maxOccurenceValue;
		}
	}
	#endregion
}
