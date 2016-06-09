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
	#region FinancialArguments
	public class FinancialArguments {
		double rate;
		double period;
		double countPeriods;
		double pmt;
		double presentValue;
		double futureValue;
		double type;
		double startPeriod;
		double endPeriod;
		protected internal double Rate { get { return rate; } set { rate = value; } }
		protected internal double Period { get { return period; } set { period = value; } }
		protected internal double CountPeriods { get { return countPeriods; } set { countPeriods = value; } }
		protected internal double Pmt { get { return pmt; } set { pmt = value; } }
		protected internal double PresentValue { get { return presentValue; } set { presentValue = value; } }
		protected internal double FutureValue { get { return futureValue; } set { futureValue = value; } }
		protected internal double Type { get { return type; } set { type = value; } }
		protected internal double StartPeriod { get { return startPeriod; } set { startPeriod = value; } }
		protected internal double EndPeriod { get { return endPeriod; } set { endPeriod = value; } }
	}
	#endregion
	#region FunctionAnnuityBase (abstract class)
	public abstract class FunctionAnnuityBase : WorksheetFunctionBase {
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			FinancialArguments financialArguments = new FinancialArguments();
			SetDefaultArguments(financialArguments);
			for (int i = 0; i < arguments.Count; i++) {
				VariantValue value = CalculateArgument(arguments[i], context);
				if (value.IsError)
					return value;
				SetArgument(financialArguments, i, value.NumericValue);
			}
			return CalculateResult(financialArguments);
		}
		protected virtual VariantValue CalculateArgument(VariantValue value, WorkbookDataContext context) {
			return value.ToNumeric(context);
		}
		protected virtual void SetDefaultArguments(FinancialArguments arguments) {
		}
		protected abstract void SetArgument(FinancialArguments arguments, int argumentIndex, double value);
		protected abstract VariantValue CalculateResult(FinancialArguments arguments);
	}
	#endregion
	#region FunctionAnnuityPrincipalPaymentBase (abstract class)
	public abstract class FunctionAnnuityPrincipalPaymentBase : FunctionAnnuityBase {
		#region Properties
		protected abstract bool IsPrincipalPayment { get; }
		#endregion
		protected override void SetArgument(FinancialArguments arguments, int argumentIndex, double value) {
			switch (argumentIndex) {
				case 0:
					arguments.Rate = value;
					break;
				case 1:
					arguments.Period = value;
					break;
				case 2:
					arguments.CountPeriods = value;
					break;
				case 3:
					arguments.PresentValue = value;
					break;
				case 4:
					arguments.FutureValue = value;
					break;
				case 5:
					arguments.Type = value == 0 ? 0 : 1;
					break;
			}
		}
		protected double CalculatePayment(FinancialArguments arguments) {
			double rate = arguments.Rate;
			double period = arguments.Period;
			double countPeriods = arguments.CountPeriods;
			double presentValue = arguments.PresentValue;
			double futureValue = arguments.FutureValue;
			double type = arguments.Type;
			if (((period == 1 && type == 1) || rate == 0) && !IsPrincipalPayment)
				return 0;
			double sumPresentFutureValue = presentValue + futureValue;
			if (rate == 0)
				return -sumPresentFutureValue / countPeriods;
			double value = rate / (1 + type * rate);
			double value1 = 1 - Math.Pow(1 + rate, countPeriods);
			if (period == 1 && type == 1)
				return value * (sumPresentFutureValue / value1 - presentValue);
			double value2 = Math.Pow(1 + rate, period - 1);
			if (IsPrincipalPayment)
				return value * sumPresentFutureValue * value2 / value1;
			return value * (sumPresentFutureValue * (1 - value2) / value1 - presentValue);
		}
		protected override VariantValue CalculateResult(FinancialArguments arguments) {
			double period = arguments.Period;
			double countPeriods = arguments.CountPeriods;
			if (period < 1 || period - countPeriods >= 1)
				return VariantValue.ErrorNumber;
			if (countPeriods > 0 && countPeriods < 1)
				arguments.CountPeriods = 1;
			return CalculatePayment(arguments);
		}
	}
	#endregion
	#region FunctionCumulativeAnnuityPrincipalPaymentBase (abstract class)
	public abstract class FunctionCumulativeAnnuityPrincipalPaymentBase : FunctionAnnuityPrincipalPaymentBase {
		protected override VariantValue CalculateArgument(VariantValue value, WorkbookDataContext context) {
			if (value.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			if (value.IsBoolean || (value.IsArray && value.ArrayValue[0].IsBoolean) ||
				(value.IsCellRange && value.CellRangeValue.GetFirstCellValue().IsBoolean))
				return VariantValue.ErrorInvalidValueInFunction;
			return value.ToNumeric(context);
		}
		protected override void SetArgument(FinancialArguments arguments, int argumentIndex, double value) {
			switch (argumentIndex) {
				case 0:
					arguments.Rate = value;
					break;
				case 1:
					arguments.CountPeriods = value;
					break;
				case 2:
					arguments.PresentValue = value;
					break;
				case 3:
					arguments.StartPeriod = value;
					break;
				case 4:
					arguments.EndPeriod = value;
					break;
				case 5:
					arguments.Type = value;
					break;
			}
		}
		protected override VariantValue CalculateResult(FinancialArguments arguments) {
			double rate = arguments.Rate;
			double countPeriods = arguments.CountPeriods;
			double presentValue = arguments.PresentValue;
			double startPeriod = arguments.StartPeriod;
			double endPeriod = arguments.EndPeriod;
			double type = arguments.Type;
			if (rate <= 0 || countPeriods <= 0 || presentValue <= 0 ||
					startPeriod < 1 || endPeriod < 1 || startPeriod > endPeriod ||
					startPeriod > countPeriods || endPeriod > countPeriods ||
					(type != 0 && type != 1))
				return VariantValue.ErrorNumber;
			type = (type == 0) ? 0 : 1;
			arguments.FutureValue = 0;
			double result = 0;
			arguments.Period = Math.Ceiling(startPeriod);
			while (arguments.Period <= endPeriod) {
				result += CalculatePayment(arguments);
				arguments.Period += 1;
			}
			return result;
		}
	}
	#endregion
}
