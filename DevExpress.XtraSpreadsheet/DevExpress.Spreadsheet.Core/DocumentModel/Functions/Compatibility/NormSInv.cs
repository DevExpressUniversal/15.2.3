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
	#region FunctionNormSInvCompatibility
	public class FunctionNormSInvCompatibility : WorksheetFunctionSingleDoubleArgumentBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "NORMSINV"; } }
		public override int Code { get { return 0x0128; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue GetNumericResult(double argument) {
			if (argument <= 0 || argument >= 1)
				return VariantValue.ErrorNumber;
			return GetResult(argument);
		}
		protected internal static double GetResult(double argument) {
			double aspect = -1;
			if (argument > 1.0 - Math.Exp(-2)) {
				aspect = 1;
				argument = 1.0 - argument;
			}
			double const0, const1;
			int typeConstPolynomial;
			if (argument > Math.Exp(-2)) {
				aspect = Math.Sqrt(2 * Math.PI);
				argument -= 0.5;
				const0 = const1 = argument;
				argument *= argument; 
				typeConstPolynomial = 0;
			}
			else {
				double value = Math.Sqrt(-(2 * Math.Log(argument)));
				argument = 1 / value;
				const0 = value - Math.Log(value) / value;
				const1 = -1;
				typeConstPolynomial = (value < 8) ? 1 : 2;
			}
			return aspect * GetResultApproximation(argument, const0, const1, typeConstPolynomial);
		}
		static double GetResultApproximation(double argument, double const0, double const1, int type){
			return const0 + const1 * argument * GetResultPolynomial(argument, GetConstA(type)) / GetResultPolynomial(argument, GetConstB(type));
		}
		static double GetResultPolynomial(double argument, double[] constCoeff) { 
			int lengthCoeff = constCoeff.Length;
			double result = constCoeff[0];
			for (int i = 1; i < lengthCoeff; i++)
				result = constCoeff[i] + argument * result;
			return result;
		}
		static double[] GetConstA(int type) {
			if (type == 0)
				return new double[] { -5.99633501014107895267E+1, 9.80010754185999661536E+1, -5.66762857469070293439E+1, 
									   1.39312609387279679503E+1, -1.23916583867381258016 };
			if (type == 1)
				return new double[] { 4.05544892305962419923, 3.15251094599893866154E+1, 5.71628192246421288162E+1, 
									  4.408050738932008347E+1, 1.46849561928858024014E+1, 2.18663306850790267539, 
									  -1.40256079171354495875E-1, -3.50424626827848203418E-2, -8.57456785154685413611E-4 };
			return new double[] { 3.2377489177694603597, 6.91522889068984211695, 3.93881025292474443415, 
								  1.33303460815807542389, 2.01485389549179081538E-1, 1.23716634817820021358E-2, 
								  3.01581553508235416007E-4, 2.65806974686737550832E-6, 6.2397453918498329373E-9 };
		}
		static double[] GetConstB(int type) {
			if (type == 0)
				return new double[] { 1, 1.95448858338141759834, 4.67627912898881538453, 8.63602421390890590575E+1, 
									 -2.25462687854119370527E+2, 2.00260212380060660359E+2, -8.20372256168333339912E+1, 
									  1.59056225126211695515E+1, -1.18331621121330003142 };
			if (type == 1)
				return new double[] { 1, 1.57799883256466749731E+1, 4.53907635128879210584E+1, 4.1317203825467203044E+1, 
									  1.50425385692907503408E+1, 2.50464946208309415979, -1.42182922854787788574E-1, 
									  -3.80806407691578277194E-2, -9.33259480895457427372E-4 };
			return new double[] { 1, 6.02427039364742014255, 3.67983563856160859403, 1.37702099489081330271, 
								  2.1623699359449663589E-1, 1.34204006088543189037E-2, 3.28014464682127739104E-4, 
								  2.89247864745380683936E-6, 6.79019408009981274425E-9 };
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
