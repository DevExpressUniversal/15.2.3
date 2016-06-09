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
	#region FunctionPower
	public class FunctionPower : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "POWER"; } }
		public override int Code { get { return 0x0151; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue number = arguments[0].ToNumeric(context);
			if (number.IsError)
				return number;
			VariantValue power = arguments[1].ToNumeric(context);
			if (power.IsError)
				return power;
			return GetNumericResult(number.NumericValue, power.NumericValue);
		}
		protected internal VariantValue GetNumericResult(double number, double power) {
			if (number == 0.0 && power < 0.0)
				return VariantValue.ErrorDivisionByZero;
			if (power == 0.0)
				return number == 0.0 ? VariantValue.ErrorNumber : 1.0;
			if (number >= 0 || (number < 0 && Math.Abs(power) >= 1)) {
				double result = (power > 0) ? Math.Pow(number, power) : 1 / Math.Pow(number, -power);
				if (!Double.IsNaN(result) && !Double.IsInfinity(result))
					return result;
			}
			if (number < 0 && Math.Abs(power) < 1)
				return GetNumericResultSpecialCase(number, power);
			return VariantValue.ErrorNumber;
		}
		VariantValue GetNumericResultSpecialCase(double number, double power) {
			double absPower = Math.Abs(power);
			double inversed = Math.Round(1 / absPower, 14);
			bool isIntegral = inversed - (int)inversed == 0;
			bool isEven = Math.Round((int)(inversed / 2) - inversed / 2, 14) == 0;
			if (!isIntegral || isEven) 
				return VariantValue.ErrorNumber;
			double result = - Math.Exp(Math.Log(Math.Abs(number)) * absPower);
			if (Double.IsNaN(result) || Double.IsInfinity(result))
				return VariantValue.ErrorNumber;
			if (power < 0)
				result = 1 / result;
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
