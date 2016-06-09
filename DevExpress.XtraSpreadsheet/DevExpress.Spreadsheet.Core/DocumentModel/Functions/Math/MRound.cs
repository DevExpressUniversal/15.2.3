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
	#region FunctionMRound
	public class FunctionMRound : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "MROUND"; } }
		public override int Code { get { return 0x01A6; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue number = GetNumericValue(arguments[0], context);
			if (number.IsError)
				return number;
			VariantValue multiple = GetNumericValue(arguments[1], context);
			if (multiple.IsError)
				return multiple;
			return GetNumericResult(number.NumericValue, multiple.NumericValue);
		}
		protected override VariantValue GetNumericValue(VariantValue value, WorkbookDataContext context) {
			if (value.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			if ((value.IsArray && value.ArrayValue[0].IsBoolean) ||
				(value.IsCellRange && value.CellRangeValue.GetFirstCellValue().IsBoolean) ||
				 value.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			return value.ToNumeric(context);
		} 
		VariantValue GetNumericResult(double number, double multiple) {
			if (multiple == 0 || number == 0)
				return 0;
			if (Math.Sign(number) * Math.Sign(multiple) < 0)
				return VariantValue.ErrorNumber;
			double result = number / multiple;
			double integerPartResult = Math.Floor(result);
			if (result == integerPartResult)
				result = number;
			else {
				double resultDown = integerPartResult * multiple;
				double resultUp = resultDown + multiple;
				result = (Math.Abs(number - resultUp) <= Math.Abs(number - resultDown)) ? resultUp : resultDown;
			}
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			return collection;
		}
	}
	#endregion
}
