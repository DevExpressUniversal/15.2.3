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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionPv
	public class FunctionPv : FunctionPmt {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "PV"; } }
		public override int Code { get { return 0x0038; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override void SetArgument(FinancialArguments arguments, int argumentIndex, double value) {
			if (argumentIndex == 2)
				arguments.Pmt = value;
			else
				base.SetArgument(arguments, argumentIndex, value);
		}
		protected override VariantValue CalculateResult(FinancialArguments arguments) {
			double rate = arguments.Rate;
			double countPeriods = arguments.CountPeriods;
			double pmt = arguments.Pmt;
			double futureValue = arguments.FutureValue;
			if (rate == 0)
				return -futureValue - countPeriods * pmt;
			VariantValue value;
			if (Math.Abs(countPeriods) < 1 && rate < -1) {
				value = GetNumericPowerSpecialCase(1 + rate, countPeriods);
				if (value.IsError)
					return value;
			} else
				value = Math.Pow(1 + rate, countPeriods);
			return -(futureValue + pmt * (1 + arguments.Type * rate) * (value.NumericValue - 1) / rate) / value.NumericValue;
		}
		VariantValue GetNumericPowerSpecialCase(double number, double power) {
			double absPower = Math.Abs(power);
			double inversed = Math.Round(1 / absPower, 14);
			bool isIntegral = inversed - (int)inversed == 0;
			bool isEven = Math.Round((int)(inversed / 2) - inversed / 2, 14) == 0;
			if (!isIntegral || isEven)
				return VariantValue.ErrorNumber;
			double result = -Math.Exp(Math.Log(Math.Abs(number)) * absPower);
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
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
