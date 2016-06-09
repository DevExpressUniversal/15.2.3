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
	#region FunctionBesselY
	public class FunctionBesselY : FunctionBesselJ {
		#region BesselSeriesSumCoeffs
		static readonly double[,] besselSeriesSumCoeffsOrder0 = CreateBesselSeriesSumCoeffsOrder0();
		static readonly double[,] besselSeriesSumCoeffsOrder1 = CreateBesselSeriesSumCoeffsOrder1();
		static double[,] CreateBesselSeriesSumCoeffsOrder0() {
			double[,] result = new double[,] {{-41370.35497933148554125235152,	59152134.65686889654273830069,   -34363712229.79040378171030138,
												10255208596863.94284509167421,   -1648605817185729.473122082537,	137562431639934407.8571335453,
											   -5247065581112764941.297350814,	65874732757195549259.99402049,   -27502866786291095837.01933175 },
											  { 1.0,							  1282.452772478993804176329391,	1001702.641288906265666651753,
												579512264.0700729537480087915,	261306575504.1081249568482092,	91620380340751.85262489147968,
												23928830434997818.57439356652,	4192417043410839973.904769661,	372645883898616588198.9980	}};
			return result;
		}
		static double[,] CreateBesselSeriesSumCoeffsOrder1() {
			double[,] result = new double[,] {{-2108847.540133123652824139923,	 3639488548.124002058278999428,   -2580681702194.450950541426399,
												956993023992168.3481121552788,	-196588746272214065.8820322248,	21931073399177975921.11427556,
												-1212297555414509577913.561535,	26554738314348543268942.48968,   -99637534243069222259967.44354},
											  { 1.0,							   1612.361029677000859332072312,	1563282.754899580604737366452,
												1128686837.169442121732366891,	 646534088126.5275571961681500,	297663212564727.6729292742282,
												108225825940881955.2553850180,	 29549879358971486742.90758119,	5435310377188854170800.653097},
											  { 508206736694124324531442.4152,	 0,0,0,0,0,0,0,0}};
			return result;
		}
		static double[,] BesselSeriesSumCoeffsOrder0 { get { return besselSeriesSumCoeffsOrder0; } }
		static double[,] BesselSeriesSumCoeffsOrder1 { get { return besselSeriesSumCoeffsOrder1; } }
		#endregion
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "BESSELY"; } }
		public override int Code { get { return 0x01AB; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override double MaxArgument { get { return 134217728; } }
		protected override double MinArgument { get { return 0; } }
		protected override VariantValue EvaluateArguments(double value, int order) {
			return BesselYn(value, order); 
		}
		double BesselYn(double value, int order) {
			if (order == 0)
				return BesselY01(value, 0, BesselAproximationCoeffOrder0, BesselSeriesSumCoeffsOrder0);
			if (order == 1)
				return BesselY01(value, 1, BesselAproximationCoeffOrder1, BesselSeriesSumCoeffsOrder1);
			double coeff1 = 1, coeff0 = 0;
			for (int i = order - 1; i > 0; --i) {
				double temp = coeff1;
				coeff1 = coeff1 * 2 * i / value + coeff0;
				coeff0 = -temp;
			}
			return coeff1 * BesselY01(value, 1, BesselAproximationCoeffOrder1, BesselSeriesSumCoeffsOrder1) + coeff0 * BesselY01(value, 0, BesselAproximationCoeffOrder0, BesselSeriesSumCoeffsOrder0);
		}
		double BesselY01(double value, int order, double[,] aproxCoeff, double[,] seriesCoeff) {
			if (value > 8.0) {
				BesselApproximationResult approximationResult = BesselApproximation(value, aproxCoeff);
				double arg = value - (1 + 2 * order) * Math.PI / 4;
				return Math.Sqrt(2 / Math.PI / value) * (approximationResult.Pzero * Math.Sin(arg) + approximationResult.Qzero * Math.Cos(arg));
			}
			return Math.Pow(value, order) * SeriesSum(value, seriesCoeff) + 2 / Math.PI * (BesselJn(value, order) * Math.Log(value) - order / value);
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
