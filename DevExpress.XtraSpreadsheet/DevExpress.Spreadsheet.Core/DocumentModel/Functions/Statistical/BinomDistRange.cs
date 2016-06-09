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
	#region FunctionBinomDistRange
	public class FunctionBinomDistRange : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
		#endregion
		public const int MaxNumber = 2147483646;
		#region Properties
		public override string Name { get { return "BINOM.DIST.RANGE"; } }
		public override int Code { get { return 0x4025; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
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
			int lowerBound = (int)value.NumericValue;
			int upperBound = lowerBound;
			if (arguments.Count > 3) {
				value = arguments[3].ToNumeric(context);
				if (value.IsError)
					return value;
				upperBound = (int)value.NumericValue;
			}
			if (trials < 0 || trials > MaxNumber || probability < 0 || probability > 1 || lowerBound < 0 || lowerBound > trials || upperBound < lowerBound || upperBound > trials)
				return VariantValue.ErrorNumber;
			return GetResult(trials, probability, lowerBound, upperBound);
		}
		protected VariantValue GetResult(int trials, double probability, int lowerBound, int upperBound) {
			double normalIndicator = trials * probability * (1 - probability);
			if (normalIndicator > 100) {
				double normalResultForUpperBound = FunctionNormSDist.GetResult((upperBound - trials * probability + 1.0 / 2.0) / Math.Sqrt(normalIndicator), true);
				double normalResultForLowerBound = FunctionNormSDist.GetResult((lowerBound - 1 - trials * probability + 1.0 / 2.0) / Math.Sqrt(normalIndicator), true);
				return normalResultForUpperBound - normalResultForLowerBound;
			}
			return GetBinom(trials, probability, lowerBound, upperBound);
		}
		double GetBinom(double trials, double probability, int lowerBound, int upperBound) {
			double begin = FunctionCombin.GetResult(trials, lowerBound) * Math.Pow(probability, lowerBound) * Math.Pow(1 - probability, trials - lowerBound);
			if (begin == 0)
				return 0;
			double result = begin;
			double current = begin;
			for (double k = lowerBound; k < upperBound; k += 1.0) {
				current *= ((trials - k) / (k + 1)) * (probability / (1 - probability));
				result += current;
			}
			return result > 1 ? 1 : result;
		}
	}
	#endregion
}
