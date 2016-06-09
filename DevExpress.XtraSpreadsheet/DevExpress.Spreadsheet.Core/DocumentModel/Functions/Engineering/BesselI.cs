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
	#region FunctionBesselI
	public class FunctionBesselI : WorksheetBesselArgumentsFunctionBase {
		#region BesselChebPolyExpCoeffs
		static readonly double[][] besselChebPolyExpCoeffsOrder0 = CreateChebPolyExpCoeffsOrder0();
		static readonly double[][] besselChebPolyExpCoeffsOrder1 = CreateChebPolyExpCoeffsOrder1();
		static double[][] CreateChebPolyExpCoeffsOrder0() {
			double[][] result = new double[][] {
				new double[] { -4.41534164647933937950E-18,	 3.33079451882223809783E-17,	 -2.43127984654795469359E-16,	 1.71539128555513303061E-15,
							   -1.16853328779934516808E-14,	 7.67618549860493561688E-14,	 -4.85644678311192946090E-13,	 2.95505266312963983461E-12,
							   -1.72682629144155570723E-11,	 9.67580903537323691224E-11,	 -5.18979560163526290666E-10,	 2.65982372468238665035E-9,
							   -1.30002500998624804212E-8,	  6.04699502254191894932E-8,	  -2.67079385394061173391E-7,	  1.11738753912010371815E-6,
							   -4.41673835845875056359E-6,	  1.64484480707288970893E-5,	  -5.75419501008210370398E-5,	  1.88502885095841655729E-4,
							   -5.76375574538582365885E-4,	  1.63947561694133579842E-3,	  -4.32430999505057594430E-3,	  1.05464603945949983183E-2,
							   -2.37374148058994688156E-2,	  4.93052842396707084878E-2,	  -9.49010970480476444210E-2,	  1.71620901522208775349E-1,
							   -3.04682672343198398683E-1,	  6.76795274409476084995E-1 },
				new double[] { -7.23318048787475395456E-18,	-4.83050448594418207126E-18,	  4.46562142029675999901E-17,	 3.46122286769746109310E-17,
							   -2.82762398051658348494E-16,	-3.42548561967721913462E-16,	  1.77256013305652638360E-15,	 3.81168066935262242075E-15,
							   -9.55484669882830764870E-15,	-4.15056934728722208663E-14,	  1.54008621752140982691E-14,	 3.85277838274214270114E-13,
								7.18012445138366623367E-13,	-1.79417853150680611778E-12,	 -1.32158118404477131188E-11,	-3.14991652796324136454E-11,
								1.18891471078464383424E-11,	 4.94060238822496958910E-10,	  3.39623202570838634515E-9,	  2.26666899049817806459E-8,
								2.04891858946906374183E-7,	  2.89137052083475648297E-6,	   6.88975834691682398426E-5,	  3.36911647825569408990E-3,
								8.04490411014108831608E-1 }
			};
			return result;
		}
		static double[][] CreateChebPolyExpCoeffsOrder1() {
			double[][] result = new double[][] {
				new double[] { 2.77791411276104639959E-18,	-2.11142121435816608115E-17,	  1.55363195773620046921E-16,	-1.10559694773538630805E-15,
							   7.60068429473540693410E-15,	-5.04218550472791168711E-14,	  3.22379336594557470981E-13,	-1.98397439776494371520E-12,
							   1.17361862988909016308E-11,	-6.66348972350202774223E-11,	  3.62559028155211703701E-10,	-1.88724975172282928790E-9,
							   9.38153738649577178388E-9,	 -4.44505912879632808065E-8,	   2.00329475355213526229E-7,	 -8.56872026469545474066E-7,
							   3.47025130813767847674E-6,	 -1.32731636560394358279E-5,	   4.78156510755005422638E-5,	 -1.61760815825896745588E-4,
							   5.12285956168575772895E-4,	 -1.51357245063125314899E-3,	   4.15642294431288815669E-3,	 -1.05640848946261981558E-2,
							   2.47264490306265168283E-2,	 -5.29459812080949914269E-2,	   1.02643658689847095384E-1,	 -1.76416518357834055153E-1,
							   2.52587186443633654823E-1 },
				new double[] { 7.51729631084210481353E-18,	 4.41434832307170791151E-18,	 -4.65030536848935832153E-17,	-3.20952592199342395980E-17,
							   2.96262899764595013876E-16,	 3.30820231092092828324E-16,	 -1.88035477551078244854E-15,	-3.81440307243700780478E-15,
							   1.04202769841288027642E-14,	 4.27244001671195135429E-14,	 -2.10154184277266431302E-14,	-4.08355111109219731823E-13,
							  -7.19855177624590851209E-13,	 2.03562854414708950722E-12,	  1.41258074366137813316E-11,	 3.25260358301548823856E-11,
							  -1.89749581235054123450E-11,	-5.58974346219658380687E-10,	 -3.83538038596423702205E-9,	 -2.63146884688951950684E-8,
							  -2.51223623787020892529E-7,	 -3.88256480887769039346E-6,	  -1.10588938762623716291E-4,	 -9.76109749136146840777E-3,
							   7.78576235018280120474E-1 }
			};
			return result;
		}
		static double[][] BesselChebPolyExpCoeffsOrder0 { get { return besselChebPolyExpCoeffsOrder0; } }
		static double[][] BesselChebPolyExpCoeffsOrder1 { get { return besselChebPolyExpCoeffsOrder1; } }
		#endregion
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "BESSELI"; } }
		public override int Code { get { return 0x01AC; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override double MaxArgument { get { return 710; } }
		protected override double MinArgument { get { return -MaxArgument; } }
		protected override VariantValue EvaluateArguments(double value, int order) {
			return BesselIn(value, order);
		}
		protected double BesselIn(double value, int order) {
			if (order == 0)
				return BesselI01(value, 0, BesselChebPolyExpCoeffsOrder0);
			if (order == 1)
				return BesselI01(value, 1, BesselChebPolyExpCoeffsOrder1);
			double coeff1 = 1, coeff0 = 0;
			for (int i = order - 1; i > 0; --i) {
				double temp = coeff1;
				coeff1 = -coeff1 * 2 * i / value + coeff0;
				coeff0 = temp;
			}
			return coeff1 * BesselI01(value, 1, BesselChebPolyExpCoeffsOrder1) + coeff0 * BesselI01(value, 0, BesselChebPolyExpCoeffsOrder0);
		}
		double BesselI01(double value, int order, double[][] coeff) {
			double absValue = Math.Abs(value);
			double tmp = absValue;
			if (tmp <= 8.0) {
				tmp = tmp / 2.0 - 2.0;
				tmp = ChebPolyExp(tmp, coeff[0]) * Math.Pow(absValue, order) * Math.Exp(absValue);
			}
			else {
				tmp = 32.0 / tmp - 2.0;
				tmp = ChebPolyExp(tmp, coeff[1]) * Math.Exp(absValue) / Math.Sqrt(absValue);
			}
			return tmp * Math.Pow(Math.Sign(value), order);
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
