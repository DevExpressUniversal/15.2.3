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
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionStdeva
	public class FunctionStdeva : WorksheetGenericFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "STDEVA"; } }
		public override int Code { get { return 0x016E; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionStdevaResult(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited));
			return collection;
		}
	}
	#endregion
	#region FunctionStdevaResult
	public class FunctionStdevaResult : FunctionResult {
		double total;
		List<double> values = new List<double>();
		protected virtual byte ErrorValue { get { return 1; } }
		public FunctionStdevaResult(WorkbookDataContext context)
			: base(context) {
		}
		bool ValidateArrayValue(IVariantArray variantArray) {
			for (int i = 0; i < variantArray.Count; i++)
				if (variantArray[i].IsNumeric || variantArray[i].IsEmpty)
					return true;
			return false;
		}
		public override bool ShouldProcessValueCore(VariantValue value) {
			if (value.IsArray)
				return ValidateArrayValue(value.ArrayValue);
			return true;
		}
		public override VariantValue ConvertValue(VariantValue value) {
			VariantValue result = value.ToNumeric(Context);
			if (!result.IsNumeric && value.IsText) {
				if (!ArrayOrRangeProcessing)
					return VariantValue.ErrorInvalidValueInFunction;
				return 0;
			}
			return result;
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			total += (double)value.NumericValue;
			values.Add((double)value.NumericValue);
			return true;
		}
		public override VariantValue GetFinalValue() {
			int count = values.Count;
			if (count <= ErrorValue)
				return VariantValue.ErrorDivisionByZero;
			double middle = total / count;
			double result = 0;
			foreach (double current in values) {
				double sum = current - middle;
				result += sum * sum;
			}
			result = Math.Sqrt(result / (count - ErrorValue));
			return result;
		}
	}
	#endregion
}
