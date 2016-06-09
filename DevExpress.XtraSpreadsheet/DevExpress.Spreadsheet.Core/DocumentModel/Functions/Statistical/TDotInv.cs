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
	#region FunctionTDotInv
	public class FunctionTDotInv : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "T.INV"; } }
		public override int Code { get { return 0x4052; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0].ToNumeric(context);
			if (value.IsError)
				return value;
			double probability = value.NumericValue;
			value = arguments[1].ToNumeric(context);
			if (value.IsError)
				return value;
			double degFreedom = Math.Floor(value.NumericValue);
			if (probability == 0.0 && degFreedom == 1.0)
				return VariantValue.ErrorDivisionByZero;
			if (CheckInvalidProbability(probability) || degFreedom < 1 || degFreedom > 10000000000)
				return VariantValue.ErrorNumber;
			return GetResult(probability, degFreedom);
		}
		protected virtual bool CheckInvalidProbability(double probability) {
			return probability <= 0 || probability >= 1;
		}
		protected virtual double GetResult(double probability, double degFreedom) {
			if (probability == 0.5)
				return 0.0;
			if (probability > 0.25 && probability < 0.75) {
				double betaInv1 = FunctionBetaDotInv.GetResult(0.5, 0.5 * degFreedom, Math.Abs(1.0 - 2.0 * probability));
				return (probability < 0.5 ? -1 : 1) * Math.Sqrt(degFreedom * betaInv1 / (1.0 - betaInv1));
			}
			double sign = -1;
			if (probability >= 0.75) {
				probability = 1.0 - probability;
				sign = 1;
			}
			double betaInv2 = FunctionBetaDotInv.GetResult(0.5 * degFreedom, 0.5, 2.0 * probability);
			return sign * (double.MaxValue * betaInv2 < degFreedom ? double.MaxValue : Math.Sqrt(degFreedom * (1 - betaInv2) / betaInv2));
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
