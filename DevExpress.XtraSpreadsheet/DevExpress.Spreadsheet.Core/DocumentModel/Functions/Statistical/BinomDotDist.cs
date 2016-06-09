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

using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionBinomDotDist
	public class FunctionBinomDotDist : FunctionBinomDistRange {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "BINOM.DIST"; } }
		public override int Code { get { return 0x406F; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0].ToNumeric(context);
			if (value.IsError)
				return value;
			int successes = (int)value.NumericValue;
			value = arguments[1].ToNumeric(context);
			if (value.IsError)
				return value;
			int trials = (int)value.NumericValue;
			value = arguments[2].ToNumeric(context);
			if (value.IsError)
				return value;
			double probability = value.NumericValue;
			value = arguments[3].ToBoolean(context);
			if (value.IsError)
				return value;
			bool cumulative = value.BooleanValue;
			if (successes < 0 || successes > trials || probability < 0 || probability > 1 || successes > MaxNumber || trials > MaxNumber)
				return VariantValue.ErrorNumber;
			return GetResult(successes, trials, probability, cumulative);
		}
		protected VariantValue GetResult(int successes, int trials, double probability, bool cumulative) {
			if (cumulative) {
				if (successes == trials)
					return 1;
				return FunctionBetaDist.GetResult(trials - successes, successes + 1, 1 - probability);
			}
			return base.GetResult(trials, probability, successes, successes);
		}
	}
	#endregion
}
