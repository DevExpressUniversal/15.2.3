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
namespace DevExpress.XtraSpreadsheet.Model
{
	#region FunctionBesselJ
	public class FunctionBesselJ : WorksheetBesselArgumentsFunctionBase
	{
		#region BesselSeriesSumCoeffs
		static readonly double[,] besselSeriesSumCoeffsOrder0 = CreateBesselSeriesSumCoeffsOrder0();
		static readonly double[,] besselSeriesSumCoeffsOrder1 = CreateBesselSeriesSumCoeffsOrder1();
		static double[,] CreateBesselSeriesSumCoeffsOrder0()
		{
			double[,] result = new double[,] {{ 26857.86856980014981415848441,	-40504123.71833132706360663322,	 25071582855.36881945555156435,
											   -8085222034853.793871199468171,	 1434354939140344.111664316553,	-136762035308817138.6865416609,
												6382059341072356562.289432465,	-117915762910761053603.8440800,	 493378725179413356181.6813446 },
											  { 1.0,							   1363.063652328970604442810507,	 1114636.098462985378182402543,
												669998767.2982239671814028660,	 312304311494.1213172572469442,	 112775673967979.8507056031594,
												30246356167094626.98627330784,	 5428918384092285160.200195092,	 493378725179413356211.3278438 }};
			return result;
		}
		static double[,] CreateBesselSeriesSumCoeffsOrder1()
		{
			double[,] result = new double[,] {{ 2701.122710892323414856790990,	 -4695753.530642995859767162166,	 3413234182.301700539091292655,
											   -1322983480332.126453125473247,	  290879526383477.5409737601689,	-35888175699101060.50743641413,
												2316433580634002297.931815435,	 -66721065689249162980.20941484,	 581199354001606143928.050809  },
											  { 1.0,								1606.931573481487801970916749,	 1501793.594998585505921097578,
												1013863514.358673989967045588,	  524371026216.7649715406728642,	 208166122130760.7351240184229,
												60920613989175217.46105196863,	  11857707121903209998.37113348,	 1162398708003212287858.529400 }};
			return result;
		}
		static double[,] BesselSeriesSumCoeffsOrder0 { get { return besselSeriesSumCoeffsOrder0; } }
		static double[,] BesselSeriesSumCoeffsOrder1 { get { return besselSeriesSumCoeffsOrder1; } }
		#endregion
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "BESSELJ"; } }
		public override int Code { get { return 0x01A9; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override double MaxArgument { get { return 134217728; } }
		protected override double MinArgument { get { return -MaxArgument; } }
		protected override VariantValue EvaluateArguments(double value, int order)
		{
			return BesselJn(value, order);
		}
		protected double BesselJn(double value, int order)
		{
			if (value == 0)
				return 0;
			int valueSign = 1;
			if (value < 0)
			{
				if (order % 2 != 0)
				{
					valueSign = -valueSign;
				}
				value = Math.Abs(value);
			}
			if (value < Eps)
				return 0;
			if (order == 0)
				return BesselJ01(value, 0, BesselAproximationCoeffOrder0, BesselSeriesSumCoeffsOrder0);
			if (order == 1)
				return BesselJ01(value, 1, BesselAproximationCoeffOrder1, BesselSeriesSumCoeffsOrder1);
			double coeff1 = 1, coeff0 = 0;
			for (int i = order - 1; i > 0; --i)
			{
				double temp = coeff1;
				coeff1 = coeff1 * 2 * i / value + coeff0;
				coeff0 = -temp;
			}
			return valueSign * (coeff1 * BesselJ01(value, 1, BesselAproximationCoeffOrder1, BesselSeriesSumCoeffsOrder1) + coeff0 * BesselJ01(value, 0, BesselAproximationCoeffOrder0, BesselSeriesSumCoeffsOrder0));
		}
		double BesselJ01(double value, int order, double[,] aproxCoeff, double[,] seriesCoeff)
		{
			if (value > 8.0)
			{
				BesselApproximationResult approximationResult = BesselApproximation(value, aproxCoeff);
				double arg = value - (1 + 2 * order) * Math.PI / 4;
				return Math.Sqrt(2 / Math.PI / value) * (approximationResult.Pzero * Math.Cos(arg) - approximationResult.Qzero * Math.Sin(arg));
			}
			return Math.Pow(value, order) * SeriesSum(value, seriesCoeff);
		}
		static FunctionParameterCollection PrepareParameters()
		{
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			return collection;
		}
	}
	#endregion
}
