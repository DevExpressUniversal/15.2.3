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
	#region FunctionBesselK
	public class FunctionBesselK : FunctionBesselI {
		#region BesselChebPolyExpCoeffs
		static readonly double[][] besselChebPolyExpCoeffsOrder0 = CreateChebPolyExpCoeffsOrder0();
		static readonly double[][] besselChebPolyExpCoeffsOrder1 = CreateChebPolyExpCoeffsOrder1();
		static double[][] CreateChebPolyExpCoeffsOrder0() {
			double[][] result = new double[][] {
				new double[] { 1.37446543561352307156E-16,	4.25981614279661018399E-14,	1.03496952576338420167E-11,   1.90451637722020886025E-9,
							   2.53479107902614945675E-7,	 2.28621210311945178607E-5,	 1.26461541144692592338E-3,	3.59799365153615016266E-2,
							   3.44289899924628486886E-1,	-5.35327393233902768720E-1 },
				new double[] { 5.30043377268626276149E-18,	-1.64758043015242134646E-17,   5.21039150503902756861E-17,  -1.67823109680541210385E-16,
							   5.51205597852431940784E-16,	-1.84859337734377901440E-15,   6.34007647740507060557E-15,  -2.22751332699166985548E-14,
							   8.03289077536357521100E-14,	-2.98009692317273043925E-13,   1.14034058820847496303E-12,  -4.51459788337394416547E-12,
							   1.85594911495471785253E-11,	-7.95748924447710747776E-11,   3.57739728140030116597E-10,  -1.69753450938905987466E-9,
							   8.57403401741422608519E-9,	 -4.66048989768794782956E-8,	2.76681363944501510342E-7,   -1.83175552271911948767E-6,
							   1.39498137188764993662E-5,	 -1.28495495816278026384E-4,	1.56988388573005337491E-3,   -3.14481013119645005427E-2,
							   2.44030308206595545468E0 }
			};
			return result;
		}
		static double[][] CreateChebPolyExpCoeffsOrder1() {
			double[][] result = new double[][] {
				new double[] {-7.02386347938628759343E-18,	-2.42744985051936593393E-15,   -6.66690169419932900609E-13, -1.41148839263352776110E-10,
							  -2.21338763073472585583E-8,	 -2.43340614156596823496E-6,	-1.73028895751305206302E-4,  -6.97572385963986435018E-3,
							  -1.22611180822657148235E-1,	 -3.53155960776544875667E-1,	 1.52530022733894777053E0, },
				new double[] {-5.75674448366501715755E-18,	 1.79405087314755922667E-17,   -5.68946255844285935196E-17,  1.83809354436663880070E-16, 
							  -6.05704724837331885336E-16,	 2.03870316562433424052E-15,   -7.01983709041831346144E-15,  2.47715442448130437068E-14, 
							  -8.97670518232499435011E-14,	 3.34841966607842919884E-13,   -1.28917396095102890680E-12,  5.13963967348173025100E-12, 
							  -2.12996783842756842877E-11,	 9.21831518760500529508E-11,   -4.19035475934189648750E-10,  2.01504975519703286596E-9,
							  -1.03457624656780970260E-8,	  5.74108412545004946722E-8,	-3.50196060308781257119E-7,   2.40648494783721712015E-6,
							  -1.93619797416608296024E-5,	  1.95215518471351631108E-4,	-2.85781685962277938680E-3,   1.03923736576817238437E-1, 
							   2.72062619048444266945E0 }
			};
			return result;
		}
		static double[][] BesselChebPolyExpCoeffsOrder0 { get { return besselChebPolyExpCoeffsOrder0; } }
		static double[][] BesselChebPolyExpCoeffsOrder1 { get { return besselChebPolyExpCoeffsOrder1; } }
		#endregion
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "BESSELK"; } }
		public override int Code { get { return 0x01AA; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override double MaxArgument { get { return double.MaxValue - 1; } }
		protected override double MinArgument { get { return 0; } }
		protected override VariantValue EvaluateArguments(double value, int order) {
			return BesselKn(value, order); 
		}
		double BesselKn(double value, int order) {
			if (order == 0)
				return BesselK01(value, 0, BesselChebPolyExpCoeffsOrder0);
			if (order == 1)
				return BesselK01(value, 1, BesselChebPolyExpCoeffsOrder1);
			double coeff1 = 1, coeff0 = 0;
			for (int i = order - 1; i > 0; --i) {
				double temp = coeff1;
				coeff1 = coeff1 * 2 * i / value + coeff0;
				coeff0 = temp;
			}
			return coeff1 * BesselK01(value, 1, BesselChebPolyExpCoeffsOrder1) + coeff0 * BesselK01(value, 0, BesselChebPolyExpCoeffsOrder0);
		}
		double BesselK01(double value, int order, double[][] coeff) {
			double tmp = 0;
			if (value <= 2) {
				tmp = value * value - 2.0;
				return ChebPolyExp(tmp, coeff[0]) / Math.Pow(value, order) - Math.Pow(-1, order) * Math.Log(0.5 * value) * BesselIn(value, order);
			}
			else {
				tmp = 8.0 / value - 2.0;
				return ChebPolyExp(tmp, coeff[1]) * Math.Exp(-value) / Math.Sqrt(value);
			}
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
