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
	#region FunctionFrequency
	public class FunctionFrequency : WorksheetGenericFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "FREQUENCY"; } }
		public override int Code { get { return 0x00FC; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Array; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value1 = arguments[0];
			if (value1.IsError)
				return value1;
			if (value1.IsBoolean || value1.IsText)
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue value2 = arguments[1];
			if (value2.IsError)
				return value2;
			return GetResult(value1, value2, context);
		}
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionFrequencyResult(context);
		}
		VariantValue GetResult(VariantValue dataArray, VariantValue binsArray, WorkbookDataContext context) {
			FunctionFrequencyResult resultFunction = CreateInitialFunctionResult(context) as FunctionFrequencyResult;
			VariantValue error = resultFunction.Prepare(binsArray);
			if (error.IsError)
				return error;
			return GetResultCore(dataArray, resultFunction);
		}
		VariantValue GetResultCore(VariantValue dataArray, FunctionFrequencyResult function) {
			if (dataArray.IsNumeric)
				return EvaluateValue(dataArray, function);
			if (dataArray.IsArray)
				return EvaluateArray(dataArray.ArrayValue, function);
			if (dataArray.IsCellRange)
				return EvaluateCellRange(dataArray.CellRangeValue, function);
			return VariantValue.ErrorInvalidValueInFunction;
		}
	}
	#endregion 
	#region FunctionFrequencyResult
	public class FunctionFrequencyResult : FunctionSumResultBase {
		#region Fields
		readonly IList<double> binsArray = new List<double>();
		VariantArray resultArray;
		#endregion
		public FunctionFrequencyResult(WorkbookDataContext context)
			: base(context) {
		}
		internal VariantValue Prepare(VariantValue binsArray) {
			VariantValue error = VariantValue.Empty;
			if (binsArray.IsBoolean || binsArray.IsText || binsArray.IsEmpty)
				return VariantValue.ErrorInvalidValueInFunction;
			if (binsArray.IsNumeric)
				error = PrepareFunctionCore(binsArray);
			if (binsArray.IsArray)
				error = PrepareFunctionCore(binsArray.ArrayValue);
			if (binsArray.IsCellRange)
				error = PrepareFunctionCore(binsArray.CellRangeValue);
			if (!error.IsError)
				PrepareCore();
			return error;
		}
		VariantValue PrepareFunctionCore(VariantValue binsArray) {
			this.binsArray.Add(binsArray.NumericValue);
			return VariantValue.Empty;
		}
		VariantValue PrepareFunctionCore(IVariantArray binsArray) {
			int binsArrayCount = (int)binsArray.Count;
			for (int i = 0; i < binsArrayCount; i++) {
				VariantValue value = binsArray[i];
				if (value.IsError)
					return value;
				if (value.IsNumeric)
					this.binsArray.Add(value.NumericValue);
			}
			return VariantValue.Empty;
		}
		VariantValue PrepareFunctionCore(CellRangeBase binsArray) {
			IEnumerator<VariantValue> enumerator = binsArray.GetExistingValuesEnumerator();
			while (enumerator.MoveNext()) {
				VariantValue value = enumerator.Current;
				if (value.IsError)
					return value;
				if (value.IsNumeric)
					this.binsArray.Add(value.NumericValue);
			}
			return VariantValue.Empty;
		}
		void PrepareCore() {
			if (binsArray.Count == 0)
				binsArray.Add(0);
			int height = binsArray.Count + 1;
			resultArray = VariantArray.Create(1, height);
			for (int i = 0; i < height; i++)
				resultArray.SetValue(i, 0, 0);
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			double doubleValue = value.NumericValue;
			if (doubleValue <= binsArray[0])
				IncrementArrayValue(0);
			int binsArrayCount = binsArray.Count;
			for (int i = 1; i < binsArrayCount; i++) {
				double previosValue = binsArray[i - 1];
				double currentValue = binsArray[i];
				if (previosValue == currentValue)
					continue;
				if (doubleValue > previosValue && doubleValue <= currentValue)
					IncrementArrayValue(i);
			}
			if (doubleValue > binsArray[binsArrayCount - 1])
				IncrementArrayValue(binsArrayCount);
			return true;
		}
		void IncrementArrayValue(int index) {
			int previosValue = (int)resultArray.GetValue(index, 0).NumericValue;
			resultArray.SetValue(index, 0, previosValue + 1);
		}
		public override VariantValue GetFinalValue() {
			VariantValue result = new VariantValue();
			result.ArrayValue = resultArray;
			return result;
		}
	}
	#endregion
}
