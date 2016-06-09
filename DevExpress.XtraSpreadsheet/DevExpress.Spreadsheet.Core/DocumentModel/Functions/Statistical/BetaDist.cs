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
	#region FunctionBetaDist
	public class FunctionBetaDist : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "BETA.DIST"; } }
		public override int Code { get { return 0x4014; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		internal const double MachineEpsilon = 1.11022302462515654042E-16;
		internal const double MaxLog = 7.09782712893383996732E2;
		internal const double MinLog = -7.451332191019412076235E2;
		internal const double MaxGamma = 171.624376956302725;
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0].ToNumeric(context);
			if (value.IsError)
				return value;
			double x = value.NumericValue;
			value = arguments[1].ToNumeric(context);
			if (value.IsError)
				return value;
			double alpha = value.NumericValue;
			value = arguments[2].ToNumeric(context);
			if (value.IsError)
				return value;
			double beta = value.NumericValue;
			value = arguments[3].ToBoolean(context);
			if (value.IsError)
				return value;
			bool cumulative = value.BooleanValue;
			double A = 0;
			if (arguments.Count > 4) {
				value = arguments[4].ToNumeric(context);
				if (value.IsError)
					return value;
				A = value.NumericValue;
			}
			double B = 1;
			if (arguments.Count > 5) {
				value = arguments[5];
				if (!value.IsEmpty) {
					value = value.ToNumeric(context);
					if (value.IsError)
						return value;
					B = value.NumericValue;
				}
			}
			if (alpha <= 0 || beta <= 0 || x < A || x > B || A == B)
				return VariantValue.ErrorNumber;
			if (!cumulative && ((x == 0 && alpha <= 1) || (x == 1 && beta <= 1)))
				return VariantValue.ErrorNumber;
			if (cumulative)
				return GetResult(alpha, beta, (x - A) / (B - A));
			return Math.Exp(GetLnBeta(alpha, beta) + (alpha - 1) * Math.Log(x - A) + (beta - 1) * Math.Log(B - x) - (alpha + beta - 1) * Math.Log(B - A));
		}
		internal static double GetBeta(double alpha, double beta) {
			return Math.Exp(FunctionGammaLn.GetResult(alpha + beta)) / (Math.Exp(FunctionGammaLn.GetResult(alpha)) * Math.Exp(FunctionGammaLn.GetResult(beta)));
		}
		internal static double GetLnBeta(double alpha, double beta) {
			return FunctionGammaLn.GetResult(alpha + beta) - FunctionGammaLn.GetResult(alpha) - FunctionGammaLn.GetResult(beta);
		}
		internal static double GetResult(double alpha, double beta, double x) {
			if ((beta * x) <= 1.0 && x <= 0.95)
				return Pseries(alpha, beta, x);
			if (x > (alpha / (alpha + beta)))
				return IbetaCore(beta, alpha, true, x, 1.0 - x);
			return IbetaCore(alpha, beta, false, 1.0 - x, x);
		}
		static double IbetaCore(double alpha, double beta, bool flag, double beginX, double endX) {
			if (flag && (beta * endX) <= 1.0 && endX <= 0.95)
				return IbetaFinalResult(true, Pseries(alpha, beta, endX));
			double result = endX * (alpha + beta - 2.0) - (alpha - 1.0);
			double fraction = result < 0.0 ? FractionExpansion(alpha, beta, endX, true) : FractionExpansion(alpha, beta, endX, false) / beginX;
			result = alpha * Math.Log(endX);
			if ((alpha + beta) < MaxGamma && Math.Abs(result) < MaxLog && Math.Abs(beta * Math.Log(beginX)) < MaxLog)
				return IbetaFinalResult(flag, Math.Pow(beginX, beta) * Math.Pow(endX, alpha) * fraction * GetBeta(alpha, beta) / alpha);
			result += beta * Math.Log(beginX) + GetLnBeta(alpha, beta) + Math.Log(fraction / alpha);
			if (result < MinLog)
				return IbetaFinalResult(flag, 0);
			return IbetaFinalResult(flag, Math.Exp(result));
		}
		static double IbetaFinalResult(bool flag, double result) {
			if (flag && result <= MachineEpsilon)
				return 1.0 - MachineEpsilon;
			if (flag && result > MachineEpsilon)
				return 1.0 - result;
			return result;
		}
		static double FractionExpansion(double alpha, double beta, double x, bool flag) {
			double big = 4.503599627370496e15;
			double bigInv = 2.22044604925031308085e-16;
			double k1, k2, current, approxPk, approxQk, precision;
			double approxCoefPk1 = 1.0;
			double approxCoefPk2 = 0.0;
			double approxCoefQk1 = 1.0;
			double approxCoefQk2 = 1.0;
			double result = 1.0;
			int n = 0;
			if (flag) {
				k1 = alpha + beta;
				k2 = beta - 1.0;
			} else {
				k1 = beta - 1.0;
				k2 = alpha + beta;
			}
			do {
				if (flag)
					current = -(x * (alpha + n) * (k1 + n)) / ((alpha + 2 * n) * (alpha + 1 + 2 * n));
				else
					current = -(x / (1.0 - x) * (alpha + n) * (k1 - n)) / ((alpha + 2 * n) * (alpha + 1 + 2 * n));
				approxPk = approxCoefPk1 + approxCoefPk2 * current;
				approxQk = approxCoefQk1 + approxCoefQk2 * current;
				approxCoefPk2 = approxCoefPk1;
				approxCoefPk1 = approxPk;
				approxCoefQk2 = approxCoefQk1;
				approxCoefQk1 = approxQk;
				if (flag)
					current = (x * (1 + n) * (k2 - n)) / ((alpha + 1 + 2 * n) * (alpha + 2 + 2 * n));
				else
					current = (x / (1.0 - x) * (1 + n) * (k2 + n)) / ((alpha + 1 + 2 * n) * (alpha + 2 + 2 * n));
				approxPk = approxCoefPk1 + approxCoefPk2 * current;
				approxQk = approxCoefQk1 + approxCoefQk2 * current;
				approxCoefPk2 = approxCoefPk1;
				approxCoefPk1 = approxPk;
				approxCoefQk2 = approxCoefQk1;
				approxCoefQk1 = approxQk;
				if (approxQk != 0 && approxPk / approxQk != 0) {
					result = approxPk / approxQk;
					precision = Math.Abs(1.0 / result - 1);
				} else
					precision = 1.0;
				if (precision < 3.0 * MachineEpsilon)
					return result;
				if ((Math.Abs(approxQk) + Math.Abs(approxPk)) > big) {
					approxCoefPk2 *= bigInv;
					approxCoefPk1 *= bigInv;
					approxCoefQk2 *= bigInv;
					approxCoefQk1 *= bigInv;
				}
				if ((Math.Abs(approxQk) < bigInv) || (Math.Abs(approxPk) < bigInv)) {
					approxCoefPk2 *= big;
					approxCoefPk1 *= big;
					approxCoefQk2 *= big;
					approxCoefQk1 *= big;
				}
			} while (++n < 300);
			return result;
		}
		static double Pseries(double alpha, double beta, double x) {
			double result = 0;
			double n = 2.0;
			double current = (1.0 - beta) * x / (alpha + 1.0);
			while (Math.Abs(current) > MachineEpsilon / alpha) {
				current *= (alpha + n - 1) * (n - beta) * x / (n * (alpha + n));
				result += current;
				n++;
			}
			result += (1.0 - beta) * x / (alpha + 1.0) + 1.0 / alpha;
			if ((alpha + beta) < MaxGamma && Math.Abs(alpha * Math.Log(x)) < MaxLog)
				result *= GetBeta(alpha, beta) * Math.Pow(x, alpha);
			else {
				double precision = GetLnBeta(alpha, beta) + alpha * Math.Log(x) + Math.Log(result);
				result = precision < MinLog ? 0.0 : Math.Exp(precision);
			}
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
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
