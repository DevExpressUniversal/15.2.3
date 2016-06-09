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
	#region FunctionFDotDist
	public class FunctionFDotDist : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "F.DIST"; } }
		public override int Code { get { return 0x4054; } }
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
			double degreeFreedom1 = Math.Floor(value.NumericValue);
			value = arguments[2].ToNumeric(context);
			if (value.IsError)
				return value;
			double degreeFreedom2 = Math.Floor(value.NumericValue);
			value = arguments[3].ToBoolean(context);
			if (value.IsError)
				return value;
			bool cumulative = value.BooleanValue;
			if (x < 0 || degreeFreedom1 < 1 || degreeFreedom2 < 1 || degreeFreedom1 > 10000000000 || degreeFreedom2 > 10000000000 || 
			   (x == 0 && !cumulative && degreeFreedom1 == 1 && degreeFreedom2 == 1))
				return VariantValue.ErrorNumber;
			return GetResult(degreeFreedom1, degreeFreedom2, x, cumulative);
		}
		internal static double GetResult(double df1, double df2, double x, bool cumulative) {
			if (cumulative)
				return GetResult(df1, df2, x);
			double lnBeta = FunctionBetaDist.GetLnBeta(df1 / 2.0, df2 / 2.0);
			if (x == 0)
				return FunctionBetaDist.GetBeta(df1 / 2.0, df2 / 2.0) * Math.Pow(df1 / df2, df1 / 2.0) * Math.Pow(x, df1 / 2.0 - 1.0) * Math.Pow(1.0 + df1 * x / df2, -(df1 + df2) / 2.0);
			return Math.Exp(lnBeta + df1 * Math.Log(df1 / df2) / 2.0 + (df1 / 2.0 - 1.0) * Math.Log(x) - (df1 + df2) * Math.Log(1.0 + df1 * x / df2) / 2.0);
		}
		internal static double GetResult(double df1, double df2, double x) {
			if (x == 1 && df1 == df2)
				return 0.5;
			return FunctionBetaDist.GetResult(0.5 * df1, 0.5 * df2, df1 * x / (df1 * x + df2));
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
