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
	#region FunctionChisqDotDist
	public class FunctionChisqDotDist : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "CHISQ.DIST"; } }
		public override int Code { get { return 0x4065; } }
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
			if (x < 0 || degreeFreedom < 1 || degreeFreedom > 10000000000 || (!cumulative && x == 0 && degreeFreedom < 2))
				return VariantValue.ErrorNumber;
			return GetResult(x, degreeFreedom, cumulative);
		}
		internal static double GetResult(double x, double df, bool cumulative) {
			if (cumulative)
				return FunctionGammaDotDist.GetResult(df / 2.0, 1.0, x / 2.0, cumulative);
			return FunctionGammaDotDist.GetResult(df / 2.0, 1.0, x / 2.0, cumulative) / 2;
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
