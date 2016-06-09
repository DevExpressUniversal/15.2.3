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
	#region FunctionGamma
	public class FunctionGamma : FunctionGammaLn {
		public override string Name { get { return "GAMMA"; } }
		public override int Code { get { return 0x4026; } }
		protected override VariantValue EvaluateArgument(VariantValue argument, WorkbookDataContext context) {
			VariantValue value = argument.ToNumeric(context);
			if (value.IsError)
				return value;
			double doubleValue = value.NumericValue;
			double result;
			if (doubleValue > 0)
				result = GetGamma(doubleValue);
			else {
				int integerValue = Math.Abs((int)doubleValue);
				double fracValue = doubleValue + integerValue;
				if (fracValue == 0)
					return VariantValue.ErrorNumber;
				result = Math.Pow(-1, integerValue - 1) * GetGamma(-fracValue) * GetGamma(1 + fracValue) / GetGamma(integerValue + 1 - fracValue);
			}
			return double.IsInfinity(result) ? VariantValue.ErrorNumber : result;
		}
		double GetGamma(double numericValue) {
			return Math.Exp(FunctionGammaLn.GetResult(numericValue));
		}
	}
	#endregion
}
