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
	#region FunctionNormDist
	public class FunctionNormDist : WorksheetFunctionBase {
		const double Infinum = 10;
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "NORM.DIST"; } }
		public override int Code { get { return 0x400D; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0].ToNumeric(context);
			if (value.IsError)
				return value;
			double x = value.NumericValue;
			value = arguments[1].ToNumeric(context);
			if (value.IsError)
				return value;
			double mean = value.NumericValue;
			value = arguments[2].ToNumeric(context);
			if (value.IsError)
				return value;
			if (value.NumericValue <= 0)
				return VariantValue.ErrorNumber;
			double standardDev = value.NumericValue;
			value = arguments[3].ToBoolean(context);
			if (value.IsError)
				return value;
			bool cumulative = value.BooleanValue;
			return GetResult(x, mean, standardDev, cumulative);
		}
		internal static double GetResult(double x, double mean, double standardDev, bool cumulative) {
			double result = GetResult((x - mean) / standardDev, cumulative);
			return cumulative ? result : result / standardDev; 
		}
		internal static double GetResult(double x, bool cumulative) {
			double result;
			if (cumulative)
				result = GetStandartNormalDistribution(x);
			else
				result = GetDensityStandartNormalDistribution(x);
			return result;
		}
		static double GetDensityStandartNormalDistribution(double x) {
			return 0.398942280401433 * Math.Exp(-x * x / 2);
		}
		static double GetStandartNormalDistribution(double x) {
			double absX = Math.Abs(x);
			if (absX > Infinum)
				return (x > 0) ? 1 : 0;
			double tableStepX = 1.0;
			double[] functionLaplaceTable = { 0.5, 0.841344746068543, 0.977249868051821, 0.998650101968370, 0.999968328758167, 
											  0.999999713348428, 0.999999999013412, 0.999999999998720, 0.999999999999999, 1.0 };
			int nearestTableArgumentNumber = (int)(absX / tableStepX + 0.5);
			if (nearestTableArgumentNumber > 9)
				nearestTableArgumentNumber = 9;
			double nearestTableArgument = nearestTableArgumentNumber * tableStepX;
			absX -= nearestTableArgument;
			double endInterval = absX;
			double nearestTableResult = functionLaplaceTable[nearestTableArgumentNumber];
			double beginValue = nearestTableResult;
			double endValue = GetDensityStandartNormalDistribution(nearestTableArgument);
			double partialSum = 0;
			double result = nearestTableResult + endValue * endInterval;
			for (int i = 2; Math.Abs(partialSum - nearestTableResult) > 1e-15 || Math.Abs(nearestTableResult - result) > 1e-15; i++) {
				partialSum = -nearestTableArgument * endValue - (i - 2) * beginValue;
				beginValue = endValue;
				endValue = partialSum;
				endInterval *= absX / i;
				partialSum = nearestTableResult;
				nearestTableResult = result;
				result += endValue * endInterval;
			}
			if (x < 0)
				result = 1 - result;
			return Math.Round(result, 15);
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
