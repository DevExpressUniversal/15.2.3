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
	#region FunctionHypGeomDotDist
	public class FunctionHypGeomDotDist : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "HYPGEOM.DIST"; } }
		public override int Code { get { return 0x406D; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0].ToNumeric(context);
			if (value.IsError)
				return value;
			int successesInSample = (int)value.NumericValue;
			value = arguments[1].ToNumeric(context);
			if (value.IsError)
				return value;
			int sampleSize = (int)value.NumericValue;
			value = arguments[2].ToNumeric(context);
			if (value.IsError)
				return value;
			int succesesInPopulation = (int)value.NumericValue;
			value = arguments[3].ToNumeric(context);
			if (value.IsError)
				return value;
			int populationSize = (int)value.NumericValue;
			value = arguments[4].ToBoolean(context);
			if (value.IsError)
				return value;
			bool cumulative = value.BooleanValue;
			if (successesInSample < 0 || sampleSize < 0 || sampleSize > populationSize || succesesInPopulation < 0 || succesesInPopulation > populationSize ||
				populationSize < 0 || successesInSample > FunctionBinomDistRange.MaxNumber || populationSize > FunctionBinomDistRange.MaxNumber)
				return VariantValue.ErrorNumber;
			return GetResult(successesInSample, sampleSize, succesesInPopulation, populationSize, cumulative);
		}
		protected VariantValue GetResult(int successesInSample, int sampleSize, int succesesInPopulation, int populationSize, bool cumulative) {
			if (successesInSample == sampleSize && successesInSample == succesesInPopulation && successesInSample == populationSize)
				return 1;
			int lowerBound = sampleSize + succesesInPopulation - populationSize;
			if (successesInSample < lowerBound)
				return 0;
			int upperBound = Math.Min(sampleSize, succesesInPopulation);
			if (cumulative && successesInSample >= upperBound)
				return 1;
			if (!cumulative && successesInSample > upperBound)
				return 0;
			if (cumulative)
				return CalculateSum(successesInSample, sampleSize, succesesInPopulation, populationSize, Math.Max(0, lowerBound));
			return GetProbabilityMassFunctionGamma(successesInSample, sampleSize, succesesInPopulation, populationSize);
		}
		double CalculateSum(int successesInSample, int sampleSize, int succesesInPopulation, int populationSize, int firstNonZero) {
			double sum = 0;
			for (int i = firstNonZero; i <= successesInSample; i++)
				sum += GetProbabilityMassFunctionGamma(i, sampleSize, succesesInPopulation, populationSize);
			return sum > 1 ? 1 : sum;
		}
		double GetProbabilityMassFunctionGamma(double successesInSample, double sampleSize, double succesesInPopulation, double populationSize) {
			double coeff1 = FunctionGammaLn.GetResult(succesesInPopulation + 1) -
							FunctionGammaLn.GetResult(successesInSample + 1) -
							FunctionGammaLn.GetResult(succesesInPopulation - successesInSample + 1);
			double coeff2 = FunctionGammaLn.GetResult(populationSize - succesesInPopulation + 1) -
							FunctionGammaLn.GetResult(sampleSize - successesInSample + 1) -
							FunctionGammaLn.GetResult(populationSize - succesesInPopulation - sampleSize + successesInSample + 1);
			double coeff3 = FunctionGammaLn.GetResult(sampleSize + 1) +
							FunctionGammaLn.GetResult(populationSize - sampleSize + 1) -
							FunctionGammaLn.GetResult(populationSize + 1);
			return Math.Exp(coeff1 + coeff2 + coeff3);
		}
	}
	#endregion
}
