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
	#region FunctionRate
	public class FunctionRate : FunctionPmt {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "RATE"; } }
		public override int Code { get { return 0x003C; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override void SetArgument(FinancialArguments arguments, int argumentIndex, double value) {
			switch (argumentIndex) {
				case 0:
					arguments.CountPeriods = value;
					break;
				case 1:
					arguments.Pmt = value;
					break;
				case 5:
					arguments.Rate = value;
					break;
				default:
					base.SetArgument(arguments, argumentIndex, value);
					break;
			}
		}
		protected override void SetDefaultArguments(FinancialArguments arguments) {
			arguments.Rate = 0.1;
		}
		protected override VariantValue CalculateResult(FinancialArguments arguments) {
			double precision = 1e-7;
			double countPeriods = arguments.CountPeriods;
			double pmt = arguments.Pmt;
			double presentValue = arguments.PresentValue;
			double futureValue = arguments.FutureValue;
			if (countPeriods < 1 || arguments.Rate + 1 <= precision ||
				(pmt >= 0 && presentValue >= 0 && futureValue >= 0) ||
				(pmt <= 0 && presentValue <= 0 && futureValue <= 0))
				return VariantValue.ErrorNumber;
			int maxIteration = 100;
			int numberIteration = 1;
			if (arguments.Rate == 0)
				arguments.Rate += precision;
			while (Math.Abs(GetFunctionImplicitEquation(arguments)) >= precision) {
				arguments.Rate = GetResultNewtonIteration(arguments);
				if (numberIteration > maxIteration || arguments.Rate + 1 <= precision)
					return VariantValue.ErrorNumber;
				numberIteration++;
			}
			return arguments.Rate;
		}
		double GetResultNewtonIteration(FinancialArguments arguments) {
			double rate = arguments.Rate;
			if (rate == 0)
				return 0;
			double functionImplicitEquation = GetFunctionImplicitEquation(arguments);
			if (functionImplicitEquation == 0)
				return rate;
			return rate - functionImplicitEquation / GetDerivativeFunctionImplicitEquation(arguments);
		}
		double GetFunctionImplicitEquation(FinancialArguments arguments) {
			double rate = arguments.Rate;
			double value = Math.Pow(1 + rate, arguments.CountPeriods);
			return arguments.PresentValue * value + arguments.Pmt * (1 + rate * arguments.Type) * (value - 1) / rate + arguments.FutureValue;
		}
		double GetDerivativeFunctionImplicitEquation(FinancialArguments arguments) {
			double rate = arguments.Rate;
			double countPeriods = arguments.CountPeriods;
			double pmt = arguments.Pmt;
			double presentValue = arguments.PresentValue;
			double futureValue = arguments.FutureValue;
			double type = arguments.Type;
			if (rate == 0)
				return presentValue + futureValue + pmt * countPeriods;
			double value = Math.Pow(1 + rate, countPeriods);
			double derivativeValue = countPeriods * value / (1 + rate);
			return presentValue * derivativeValue + pmt * type * (value - 1) / rate +
				   pmt * (1 + rate * type) * (derivativeValue * rate - value + 1) / rate / rate;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
