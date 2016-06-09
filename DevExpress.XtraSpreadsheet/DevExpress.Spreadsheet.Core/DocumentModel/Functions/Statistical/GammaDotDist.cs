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
	#region FunctionGammaDotDist
	public class FunctionGammaDotDist : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "GAMMA.DIST"; } }
		public override int Code { get { return 0x4058; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		public const double IncompleteGammaEpsilon = 0.000000000000001;
		public const double IncompleteGammaBigNumber = 4503599627370496.0;
		const double IncompleteGammaBigNumberInv = 2.22044604925031308085E-16;
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
			if (x < 0 || alpha <= 0 || beta <= 0 || (!cumulative && x == 0 && alpha <= 1))
				return VariantValue.ErrorNumber;
			return GetResult(alpha, beta, x, cumulative);
		}
		internal static double GetResult(double alpha, double beta, double x, bool cumulative) {
			if (x == 0)
				return 0;
			if (cumulative)
				return GetResult(alpha, x / beta);
			double gammaLn = FunctionGammaLn.GetResult(alpha);
			return Math.Exp(-gammaLn - alpha * Math.Log(beta) + (alpha - 1) * Math.Log(x) - x / beta);
		}
		internal static double GetResult(double alpha, double x) {
			if (x == 0)
				return 0;
			double value = Math.Exp(alpha * Math.Log(x) - x - FunctionGammaLn.GetResult(alpha));
			if (x > 1.0 && x > alpha)
				return 1.0 - value * UpperIncompleteGamma(alpha, x);
			return value * LowerIncompleteGamma(alpha, x);
		}
		internal static double LowerIncompleteGamma(double alpha, double x) {
			double denominator = alpha;
			double current = 1.0;
			double result = 1.0;
			do {
				denominator += 1;
				current *= x / denominator;
				result += current;
			}
			while ((current / result) > IncompleteGammaEpsilon);
			return result / alpha;
		}
		internal static double UpperIncompleteGamma(double alpha, double x) {
			double currentY = 1.0 - alpha;
			double currentZ = x + currentY + 1.0;
			double coef = 0;
			double approxCoefPk1 = x + 1;
			double approxCoefPk2 = 1;
			double approxCoefQk1 = currentZ * x;
			double approxCoefQk2 = x;
			double result = approxCoefPk1 / approxCoefQk1;
			double precision = 1;
			do {
				coef += 1.0;
				currentY += 1.0;
				currentZ += 2.0;
				double current = currentY * coef;
				double approxPk = approxCoefPk1 * currentZ - approxCoefPk2 * current;
				double approxQk = approxCoefQk1 * currentZ - approxCoefQk2 * current;
				if (approxQk != 0) {
					precision = Math.Abs((approxQk * result - approxPk) / approxPk);
					result = approxPk / approxQk;
				}
				approxCoefPk2 = approxCoefPk1;
				approxCoefPk1 = approxPk;
				approxCoefQk2 = approxCoefQk1;
				approxCoefQk1 = approxQk;
				if (Math.Abs(approxPk) > IncompleteGammaBigNumber) {
					approxCoefPk2 *= IncompleteGammaBigNumberInv;
					approxCoefPk1 *= IncompleteGammaBigNumberInv;
					approxCoefQk2 *= IncompleteGammaBigNumberInv;
					approxCoefQk1 *= IncompleteGammaBigNumberInv;
				}
			}
			while (precision > IncompleteGammaEpsilon);
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
