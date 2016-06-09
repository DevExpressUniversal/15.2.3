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
	#region FunctionBinomDotInv
	public class FunctionBinomDotInv : FunctionBinomDistRange {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "BINOM.INV"; } }
		public override int Code { get { return 0x4070; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0].ToNumeric(context);
			if (value.IsError)
				return value;
			int trials = (int)value.NumericValue;
			value = arguments[1].ToNumeric(context);
			if (value.IsError)
				return value;
			double probability = value.NumericValue;
			value = arguments[2].ToNumeric(context);
			if (value.IsError)
				return value;
			double alpha = value.NumericValue;
			if (trials < 0 || trials > MaxNumber || probability <= 0 || probability >= 1 || alpha <= 0 || alpha >= 1)
				return VariantValue.ErrorNumber;
			return GetResult(trials, probability, alpha);
		}
		VariantValue GetResult(int trials, double probability, double alpha) {
			return probability <= 0.5 ? ComputeSumFromLeft(trials, probability, alpha) : ComputeSumFromRight(trials, probability, alpha);
		}
		int ComputeSumFromLeft(int trials, double probability, double alpha) {
			double probabilitySum = 0;
			int current = 0;
			while (probabilitySum < alpha) {
				probabilitySum += GetBinom(trials, probability, current);
				current++;
			}
			return IsMiddle(alpha, probabilitySum) ? current : current - 1;
		}
		int ComputeSumFromRight(int trials, double probability, double alpha) {
			double probabilitySum = 1;
			int current = trials;
			while (probabilitySum >= alpha) {
				probabilitySum -= GetBinom(trials, probability, current);
				current--;
			}
			return IsMiddle(alpha, probability) ? current : current + 1;
		}
		bool IsMiddle(double alpha, double probabilitySum) {
			return alpha == 0.5 && Math.Abs(probabilitySum - 0.5) < 1e-14;
		}
		double GetBinom(int trials, double probability, int current) {
			if (trials == current)
				return base.GetResult(trials, probability, current, current).NumericValue;
			double upperBoundSum = FunctionBetaDist.GetResult(trials - current, current + 1, 1 - probability);
			double lowerBoundSum = FunctionBetaDist.GetResult(trials - current + 1, current, 1 - probability);
			return upperBoundSum - lowerBoundSum;
		}
	}
	#endregion
}
