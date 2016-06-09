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
	#region FunctionTDotDist
	public class FunctionTDotDist : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "T.DIST"; } }
		public override int Code { get { return 0x4055; } }
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
			double degreeFreedom = Math.Floor(value.NumericValue);
			value = arguments[2].ToBoolean(context);
			if (value.IsError)
				return value;
			bool cumulative = value.BooleanValue;
			if (cumulative && (degreeFreedom < 1 || degreeFreedom > 10000000000))
				return VariantValue.ErrorNumber;
			if (!cumulative && degreeFreedom < 1)
				return VariantValue.ErrorDivisionByZero;
			return GetResult(degreeFreedom, x, cumulative);
		}
		internal static double GetResult(double degreeFreedom, double x, bool cumulative) {
			if (cumulative)
				return GetResult(degreeFreedom, x);
			double lnBeta = FunctionBetaDist.GetLnBeta(0.5 * degreeFreedom, 0.5);
			return Math.Exp(lnBeta - Math.Log(Math.Sqrt(degreeFreedom)) + (-0.5 * (degreeFreedom + 1.0)) * Math.Log(1.0 + x * x / degreeFreedom));
		}
		internal static double GetResult(double degreeFreedom, double x) {
			if (x == 0.0)
				return 0.5;
			if (x < -2.0)
				return 0.5 * FunctionBetaDist.GetResult(0.5 * degreeFreedom, 0.5, degreeFreedom / (degreeFreedom + x * x));
			double lowerLimit = 1.0 + x * x / degreeFreedom;
			double result = 0.0;
			if (degreeFreedom % 2 != 0) {
				double xsqk = Math.Abs(x) / Math.Sqrt(degreeFreedom);
				result = Math.Atan(xsqk);
				if (degreeFreedom > 1)
					result += GetDefiniteIntegral(lowerLimit, degreeFreedom, 3.0) * xsqk / lowerLimit;
				result *= 2.0 / Math.PI;
			}
			else
				result = GetDefiniteIntegral(lowerLimit, degreeFreedom, 2.0) * Math.Abs(x) / Math.Sqrt(lowerLimit * degreeFreedom);
			if (x < 0)
				result = -result;
			return 0.5 + 0.5 * result;
		}
		static double GetDefiniteIntegral(double lowerLimit, double degreeFreedom, double beginDegreeFreedom) {
			double result = 1.0;
			double current = 1.0;
			double currentDegreeFreedom = beginDegreeFreedom;
			while (currentDegreeFreedom <= degreeFreedom - 2 && (current / result) > FunctionBetaDist.MachineEpsilon) {
				current *= (currentDegreeFreedom - 1) / (lowerLimit * currentDegreeFreedom);
				result += current;
				currentDegreeFreedom += 2;
			}
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
