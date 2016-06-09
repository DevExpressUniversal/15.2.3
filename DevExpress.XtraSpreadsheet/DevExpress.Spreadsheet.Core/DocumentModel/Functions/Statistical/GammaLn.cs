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
	#region FunctionGammaLn
	public class FunctionGammaLn : WorksheetFunctionSingleArgumentBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "GAMMALN"; } }
		public override int Code { get { return 0x010F; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		public static double GetResult(double value){
			double[] stirlingCoef = new double[20] {
				1.0 / 12.0, -1.0 / 360.0, 1.0 / 1260.0, -1.0 / 1680.0, 1.0 / 1188.0,
				-691.0 / 360360.0, 1.0 / 156.0, -3617.0 / 122400.0, 43867.0 / 244188.0,
				-174611.0 / 125400.0, 77683.0 / 5796.0, -236364091.0 / 1506960.0,
				657931.0 / 300.0, -3392780147.0 / 93960.0, 1723168255201.0 / 2492028.0,
				-7709321041217.0 / 505920.0, 151628697551.0 / 396.0,
				-26315271553053477373.0 / 2418179400.0, 154210205991661.0 / 444.0,
				- 261082718496449122051.0 / 21106800.0
			};
			double sqrtLogPi = 0.9189385332046727417803297364;
			double approx;
			for (approx = 0; value < 7; value += 1)
				approx += Math.Log(value);
			double denominator = value;
			double sum = 0;
			double power2Value = value * value;
			double presum = (value - 0.5) * Math.Log(value) - value + sqrtLogPi;
			for (int i = 0; i < 20; i++) {
				sum = presum + stirlingCoef[i] / denominator;
				if (sum == presum) break;
				denominator = denominator * power2Value;
				presum = sum;
			}
			return sum - approx;
		}
		protected override VariantValue EvaluateArgument(VariantValue argument, WorkbookDataContext context) {
			VariantValue numberValue = argument.ToNumeric(context);
			if (numberValue.IsError)
				return numberValue;
			if (numberValue.NumericValue <= 0)
				return VariantValue.ErrorNumber;
			return GetResult(numberValue.NumericValue);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
