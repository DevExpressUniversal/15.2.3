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
	#region FunctionSumx2py2
	public class FunctionSumX2PY2 : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "SUMX2PY2"; } }
		public override int Code { get { return 0x0131; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			return GetResult(arguments, context, 1);
		}
		protected VariantValue GetResult(IList<VariantValue> arguments, WorkbookDataContext context, int sign) {
			VariantValue arg1 = ToArray(arguments[0], context);
			if (arg1.IsError)
				return arg1;
			VariantValue arg2 = ToArray(arguments[1], context);
			if (arg2.IsError)
				return arg2;
			IVariantArray xArray = arg1.ArrayValue;
			IVariantArray yArray = arg2.ArrayValue;
			if (xArray.Count != yArray.Count)
				return VariantValue.ErrorValueNotAvailable;
			bool oneOfArrayIsEmpty = true;
			for (int i = 0; i < xArray.Count; ++i) {
				VariantValue x = xArray[i];
				if (x.IsError)
					return x;
				if (x.IsNumeric)
					oneOfArrayIsEmpty = false;
			}
			bool isEmpty = true;
			for (int i = 0; i < xArray.Count; ++i) {
				VariantValue y = yArray[i];
				if (y.IsError)
					return y;
				if (y.IsNumeric)
					isEmpty = false;
			}
			oneOfArrayIsEmpty |= isEmpty;
			double result = 0;
			for (int i = 0; i < xArray.Count; ++i) {
				VariantValue x = xArray[i];
				VariantValue y = yArray[i];
				if (!x.IsNumeric || !y.IsNumeric)
					continue;
				result += Math.Pow(x.NumericValue, 2) + sign * Math.Pow(y.NumericValue, 2);
			}
			return oneOfArrayIsEmpty ? VariantValue.ErrorDivisionByZero : result;
		}
		VariantValue ToArray(VariantValue value, WorkbookDataContext context) {
			if (value.IsError || value.IsArray)
				return value;
			if (value.IsNumeric) {
				VariantArray array = VariantArray.Create(1, 1);
				array.SetValue(0, 0, value);
				return VariantValue.FromArray(array);
			}
			return VariantValue.ErrorInvalidValueInFunction;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Array));
			return collection;
		}
	}
	#endregion
}
