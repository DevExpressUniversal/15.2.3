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
	#region WorksheetNotationFunctionBase
	public abstract class WorksheetBesselFunctionBase : WorksheetFunctionBase {
		protected const double Eps = 1E-16;
		#region Static
		static readonly double[,] besselAproximationCoeffOrder0 = {{ 2485.271928957404011288128951,   153982.6532623911470917825993,  2016135.283049983642487182349,
																	 8413041.456550439208464315611,   12332384.76817638145232406055,  5393485.083869438325262122897 },
																   { 2615.700736920839685159081813,   156001.7276940030940592769933,  2025066.801570134013891035236,
																	 8426449.050629797331554404810,   12338310.22786324960844856182,  5393485.083869438325560444960 },
																   {-4.887199395841261531199129300,  -226.2630641933704113967255053, -2365.956170779108192723612816,
																	-8239.066313485606568803548860,  -10381.41698748464093880530341, -3984.617357595222463506790588 },
																   { 408.7714673983499223402830260,   15704.89191515395519392882766,  156021.3206679291652539287109,
																	 533291.3634216897168722255057,   666745.4239319826986004038103,  255015.5108860942382983170882 }};
		static readonly double[,] besselAproximationCoeffOrder1 = {{-1611.616644324610116477412898,  -109824.0554345934672737413139, -1523529.351181137383255105722,
																	-6603373.248364939109255245434,  -9942246.505077641195658377899, -4435757.816794127857114720794 },
																   {-1455.009440190496182453565068,  -107263.8599110382011903063867, -1511809.506634160881644546358,
																	-6585339.479723087072826915069,  -9934124.389934585658967556309, -4435757.816794127856828016962 },
																   { 35.26513384663603218592175580,   1706.375429020768002061283546,  18494.26287322386679652009819,
																	 66178.83658127083517939992166,   85145.16067533570196555001171,  33220.91340985722351859704442 },
																   { 863.8367769604990967475517183,   37890.22974577220264142952256,  400294.4358226697511708610813,
																	 1419460.669603720892855755253,   1819458.042243997298924553839,  708712.8194102874357377502472 }};
		static protected double[,] BesselAproximationCoeffOrder0 { get { return besselAproximationCoeffOrder0; } }
		static protected double[,] BesselAproximationCoeffOrder1 { get { return besselAproximationCoeffOrder1; } }
		#endregion
		protected BesselApproximationResult BesselApproximation(double value, double[,] coeff) {
			double partialSumPNominator = 0;
			double partialSumPDenominator = 1.0;
			double partialSumQNominator = 0;
			double partialSumQDenominator = 1.0;
			double commonDifference = 64.0 / (value * value);
			for (int i = 0; i < 6; ++i) {
				partialSumPNominator = coeff[0, i] + commonDifference * partialSumPNominator;
				partialSumPDenominator = coeff[1, i] + commonDifference * partialSumPDenominator;
				partialSumQNominator = coeff[2, i] + commonDifference * partialSumQNominator;
				partialSumQDenominator = coeff[3, i] + commonDifference * partialSumQDenominator;
			}
			return new BesselApproximationResult(partialSumPNominator / partialSumPDenominator, 8 * partialSumQNominator / partialSumQDenominator / value);
		}
		protected double SeriesSum(double value, double[,] coeff) {
			double commonDifference = value * value;
			double nominator = coeff[0, 0];
			double denominator = coeff[1, 0];
			for (int i = 0; i < 9; ++i) {
				nominator = coeff[0, i] + commonDifference * nominator;
				denominator = coeff[1, i] + commonDifference * denominator;
			}
			if (coeff.Length > 18)
				denominator = coeff[2, 0] + commonDifference * denominator;
			return nominator / denominator;
		}
		protected double ChebPolyExp(double value, double[] coeff) {
			double b0 = coeff[0], b1 = 0, b2 = 0;
			for (int i = 1; i < coeff.Length; ++i) {
				b2 = b1;
				b1 = b0;
				b0 = value * b1 - b2 + coeff[i];
			}
			return 0.5 * (b0 - b2);
		}
	}
	#endregion
	#region WorksheetBesselArgumentsFunctionBase
	public abstract class WorksheetBesselArgumentsFunctionBase : WorksheetBesselFunctionBase {
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			if (arguments[0].IsEmpty || arguments[1].IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			VariantValue x = context.DereferenceWithoutCrossing(arguments[0]).ToNumeric(context);
			if (x.IsError)
				return x;
			VariantValue n = context.DereferenceWithoutCrossing(arguments[1]).ToNumeric(context);
			if (n.IsError)
				return n;
			if (!(x.IsNumeric && n.IsNumeric))
				return VariantValue.ErrorInvalidValueInFunction;
			double doubleX = (double)x.NumericValue;
			if (doubleX <= MinArgument || doubleX >= MaxArgument)
				return VariantValue.ErrorNumber;
			int intN = (int)n.NumericValue;
			if (intN < 0 || intN > 10000000)
				return VariantValue.ErrorNumber;
			return EvaluateArguments(doubleX, intN);
		}
		protected abstract double MaxArgument { get; }
		protected abstract double MinArgument { get; }
		protected abstract VariantValue EvaluateArguments(double value, int order);
	}
	#endregion
	#region BesselApproximationResult
	public struct BesselApproximationResult {
		double pzero;
		double qzero;
		public BesselApproximationResult(double pzero, double qzero) {
			this.pzero = pzero;
			this.qzero = qzero;
		}
		public double Pzero { get { return pzero; } }
		public double Qzero { get { return qzero; } }
	}
	#endregion
}
