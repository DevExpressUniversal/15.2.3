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
	#region FunctionGammaDotInv
	public class FunctionGammaDotInv : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "GAMMA.INV"; } }
		public override int Code { get { return 0x4064; } }
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
			double alpha = value.NumericValue;
			value = arguments[2].ToNumeric(context);
			if (value.IsError)
				return value;
			double beta = value.NumericValue;
			if (probability < 0 || probability >= 1 || alpha <= 0 || beta <= 0)
				return VariantValue.ErrorNumber;
			if (probability == 0)
				return 0;
			return GetResult(alpha, beta, probability);
		}
		internal static double GetResult(double alpha, double beta, double probability) {
			return GammaDotInvSolver.Solve(alpha, 1 - probability) * beta;
		}
		struct GammaDotInvSolver {
			enum GammaDotInvSolverState {
				NewtonIterationsFirstStage = 0,
				BigNumberCheckStage = 1,
				NewtonIterationsSecondStage = 2,
				Result = 3
			}
			#region Fields
			const double Threshhold = 5 * FunctionGammaDist.IncompleteGammaEpsilon;
			double result;
			double normSInv;
			double incompleteGamma;
			double beginY;
			double endY;
			double beginX;
			double endX;
			double dihotomy;
			int dihotomyRegularity;
			int iterationsCount;
			GammaDotInvSolverState state;
			double alpha;
			double probability;
			#endregion
			public static double Solve(double alpha, double probability) {
				GammaDotInvSolver solver = new GammaDotInvSolver();
				solver.Initialize(alpha, probability);
				return solver.Solve();
			}
			void Initialize(double alpha, double probability) {
				this.alpha = alpha;
				this.probability = probability;
				beginX = 0;
				endX = FunctionGammaDist.IncompleteGammaBigNumber;
				beginY = 0;
				endY = 1;
				dihotomy = 1 / (9 * alpha);
				normSInv = 1 - dihotomy - FunctionNormSInvCompatibility.GetResult(probability) * Math.Sqrt(dihotomy);
				result = alpha * normSInv * normSInv * normSInv;
				iterationsCount = 0;
				state = GammaDotInvSolverState.NewtonIterationsFirstStage;
			}
			double Solve() {
				while (state != GammaDotInvSolverState.Result)
					state = Change(state);
				return result;
			}
			GammaDotInvSolverState Change(GammaDotInvSolverState state) {
				if (state == GammaDotInvSolverState.NewtonIterationsFirstStage)
					return NewtonIterationsFirstStage();
				if (state == GammaDotInvSolverState.BigNumberCheckStage)
					return BigNumberCheckStage();
				if (state == GammaDotInvSolverState.NewtonIterationsSecondStage)
					return NewtonIterationsSecondStage();
				return state;
			}
			GammaDotInvSolverState NewtonIterationsFirstStage() {
				while (iterationsCount < 10) {
					if (result > endX || result < beginX) {
						dihotomy = 0.0625;
						break;
					}
					incompleteGamma = UpperIncompleteGamma(alpha, result);
					if (incompleteGamma < beginY || incompleteGamma > endY) {
						dihotomy = 0.0625;
						break;
					}
					if (incompleteGamma < probability) {
						endX = result;
						beginY = incompleteGamma;
					}
					else {
						beginX = result;
						endY = incompleteGamma;
					}
					dihotomy = (alpha - 1) * Math.Log(result) - result - FunctionGammaLn.GetResult(alpha);
					if (dihotomy < -709.78271289338399) {
						dihotomy = 0.0625;
						break;
					}
					dihotomy = (incompleteGamma - probability) / -Math.Exp(dihotomy);
					if (Math.Abs(dihotomy / result) < FunctionGammaDist.IncompleteGammaEpsilon)
						return GammaDotInvSolverState.Result;
					result -= dihotomy;
					iterationsCount++;
				}
				return GammaDotInvSolverState.BigNumberCheckStage;
			}
			GammaDotInvSolverState BigNumberCheckStage() {
				if (endX == FunctionGammaDist.IncompleteGammaBigNumber) {
					if (result <= 0)
						result = 1;
					while (endX == FunctionGammaDist.IncompleteGammaBigNumber) {
						result = (1 + dihotomy) * result;
						incompleteGamma = UpperIncompleteGamma(alpha, result);
						if (incompleteGamma < probability) {
							endX = result;
							beginY = incompleteGamma;
							break;
						}
						dihotomy *= 2;
					}
				}
				return GammaDotInvSolverState.NewtonIterationsSecondStage;
			}
			GammaDotInvSolverState NewtonIterationsSecondStage() {
				InitDihotomy();
				iterationsCount = 0;
				while (iterationsCount < 400) {
					result = beginX + dihotomy * (endX - beginX);
					incompleteGamma = UpperIncompleteGamma(alpha, result);
					if (Math.Abs((endX - beginX) / (beginX + endX)) < Threshhold || Math.Abs((incompleteGamma - probability) / probability) < Threshhold || result <= 0)
						break;
					if (incompleteGamma >= probability) {
						beginX = result;
						endY = incompleteGamma;
						if (dihotomyRegularity < 0)
							InitDihotomy();
						else
							dihotomy = dihotomyRegularity > 1 ? 0.5 * dihotomy + 0.5 : (probability - beginY) / (endY - beginY);
						dihotomyRegularity++;
					}
					else {
						endX = result;
						beginY = incompleteGamma;
						if (dihotomyRegularity > 0)
							InitDihotomy();
						else
							dihotomy = dihotomyRegularity < -1 ? 0.5 * dihotomy : (probability - beginY) / (endY - beginY);
						dihotomyRegularity--;
					}
					iterationsCount++;
				}
				return GammaDotInvSolverState.Result;
			}
			void InitDihotomy() {
				dihotomyRegularity = 0;
				dihotomy = 0.5;
			}
			double UpperIncompleteGamma(double alpha, double x) {
				double value = Math.Exp(alpha * Math.Log(x) - x - FunctionGammaLn.GetResult(alpha));
				if (x < 1 || x < alpha)
					return 1 - value * FunctionGammaDotDist.LowerIncompleteGamma(alpha, x);
				return value * FunctionGammaDotDist.UpperIncompleteGamma(alpha, x);
			}
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
