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
	#region FunctionBetaDotInv
	public class FunctionBetaDotInv : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "BETA.INV"; } }
		public override int Code { get { return 0x4051; } }
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
			double A = 0;
			if (arguments.Count > 3) {
				value = arguments[3].ToNumeric(context);
				if (value.IsError)
					return value;
				A = value.NumericValue;
			}
			double B = 1;
			if (arguments.Count > 4) {
				value = arguments[4];
				if (!value.IsEmpty) {
					value = value.ToNumeric(context);
					if (value.IsError)
						return value;
					B = value.NumericValue;
				}
			}
			if (alpha <= 0 || beta <= 0 || probability <= 0 || probability >= 1 || A >= B)
				return VariantValue.ErrorNumber;
			return GetResult(probability, alpha, beta, A, B);
		}
		struct BetaFiniteStateMachine {
			enum BetaSolverState {
				Main = 0,
				IntervalHalvingInit = 1,
				IntervalHalvingCycle = 2,
				IntervalHalvingCycleBreak = 3,
				NewtonIterationsInit = 4,
				NewtonIterationsCycle = 5,
				NewtonIterationsCycleBreak = 6,
				Result = 7
			}
			#region Fields
			double result;
			double gammaLn;
			double betaDist;
			double currentAlpha;
			double currentBeta;
			double beginY;
			double endY;
			double currentY;
			double beginX;
			double endX;
			double currentX;
			double dihotomy;
			double dihotomyThreshhold;
			bool resultFlag;
			bool newtonFlag;
			int iterationsCount;
			int dihotomyRegularity;
			BetaSolverState state;
			double probability;
			double alpha;
			double beta;
			#endregion
			public static double Solve(double alpha, double beta, double probability) {
				BetaFiniteStateMachine machine = new BetaFiniteStateMachine();
				machine.Initialize(alpha, beta, probability);
				return machine.Solve();
			}
			void Initialize(double alpha, double beta, double probability) {
				endX = 1.0;
				currentY = 1.0;
				state = BetaSolverState.Main;
				this.alpha = alpha;
				this.beta = beta;
				this.probability = probability;
			}
			double Solve() {
				while (state != BetaSolverState.Result)
					state = Change(state);
				return UpdateResult();
			}
			BetaSolverState Change(BetaSolverState state) {
				if (state == BetaSolverState.Main)
					return MainChange();
				if (state == BetaSolverState.IntervalHalvingInit)
					return IntervalHalvingInitChange();
				if (state == BetaSolverState.IntervalHalvingCycle)
					return IntervalHalvingCycleChange();
				if (state == BetaSolverState.IntervalHalvingCycleBreak)
					return IntervalHalvingCycleBreakChange();
				if (state == BetaSolverState.NewtonIterationsInit)
					return NewtonIterationsInitChange();
				if (state == BetaSolverState.NewtonIterationsCycle)
					return NewtonIterationsCycleChange();
				if (state == BetaSolverState.NewtonIterationsCycleBreak)
					return NewtonIterationsCycleBreakChange();
				return state;
			}
			BetaSolverState MainChange() {
				if (alpha <= 1.0 || beta <= 1.0) {
					dihotomyThreshhold = 1.0e-6;
					resultFlag = false;
					SetCurrentAlphaBeta(alpha, beta);
					beginY = probability;
					result = currentAlpha / (currentAlpha + currentBeta);
					betaDist = FunctionBetaDist.GetResult(currentAlpha, currentBeta, result);
					return BetaSolverState.IntervalHalvingInit;
				} else
					dihotomyThreshhold = 1.0e-4;
				double normSInv = -FunctionNormSInvCompatibility.GetResult(probability);
				if (probability > 0.5) {
					resultFlag = true;
					SetCurrentAlphaBeta(beta, alpha);
					beginY = 1.0 - probability;
					normSInv = -normSInv;
				} else {
					resultFlag = false;
					SetCurrentAlphaBeta(alpha, beta);
					beginY = probability;
				}
				gammaLn = (normSInv * normSInv - 3.0) / 6.0;
				result = 2.0 / (1.0 / (2.0 * currentAlpha - 1.0) + 1.0 / (2.0 * currentBeta - 1.0));
				double delta = 2.0 * normSInv * Math.Sqrt(result + gammaLn) / result - (1.0 / (2.0 * currentBeta - 1.0) - 1.0 / (2.0 * currentAlpha - 1.0)) * (gammaLn + 5.0 / 6.0 - 2.0 / (3.0 * result));
				if (delta < FunctionBetaDist.MinLog) {
					result = 0;
					return BetaSolverState.Result;
				}
				result = currentAlpha / (currentAlpha + currentBeta * Math.Exp(delta));
				betaDist = FunctionBetaDist.GetResult(currentAlpha, currentBeta, result);
				return Math.Abs((betaDist - beginY) / beginY) < 0.2 ? BetaSolverState.NewtonIterationsInit : BetaSolverState.IntervalHalvingInit;
			}
			BetaSolverState IntervalHalvingInitChange() {
				InitDihotomy();
				iterationsCount = 0;
				return BetaSolverState.IntervalHalvingCycle;
			}
			BetaSolverState IntervalHalvingCycleChange() {
				if (iterationsCount <= 99) {
					if (iterationsCount != 0) {
						result = beginX + dihotomy * (endX - beginX);
						if (result == 1.0)
							result = 1.0 - FunctionBetaDist.MachineEpsilon;
						if (result == 0) {
							dihotomy = 0.5;
							result = beginX + dihotomy * (endX - beginX);
							if (result == 0)
								return BetaSolverState.Result;
						}
						betaDist = FunctionBetaDist.GetResult(currentAlpha, currentBeta, result);
						if (Math.Abs((endX - beginX) / (endX + beginX)) < dihotomyThreshhold || Math.Abs((betaDist - beginY) / beginY) < dihotomyThreshhold)
							return BetaSolverState.NewtonIterationsInit;
					}
					if (betaDist < beginY) {
						beginX = result;
						endY = betaDist;
						if (dihotomyRegularity < 0)
							InitDihotomy();
						else
							dihotomy = dihotomyRegularity > 3 ? 1.0 - (1.0 - dihotomy) * (1.0 - dihotomy) : dihotomyRegularity > 1 ? 0.5 * dihotomy + 0.5 : (beginY - betaDist) / (currentY - endY);
						dihotomyRegularity++;
						if (beginX > 0.75) {
							if (resultFlag) {
								SetCurrentAlphaBeta(alpha, beta);
								beginY = probability;
							} else {
								SetCurrentAlphaBeta(beta, alpha);
								beginY = 1.0 - probability;
							}
							resultFlag = !resultFlag;
							result = 1.0 - result;
							betaDist = FunctionBetaDist.GetResult(currentAlpha, currentBeta, result);
							beginX = 0.0;
							endX = 1.0;
							endY = 0.0;
							currentY = 1.0;
							return BetaSolverState.IntervalHalvingInit;
						}
					} else {
						endX = result;
						if (resultFlag & endX < FunctionBetaDist.MachineEpsilon) {
							result = 0.0;
							return BetaSolverState.Result;
						}
						currentY = betaDist;
						if (dihotomyRegularity > 0)
							InitDihotomy();
						else
							dihotomy = dihotomyRegularity < -3 ? dihotomy * dihotomy : dihotomyRegularity < -1 ? 0.5 * dihotomy : (betaDist - beginY) / (currentY - endY);
						dihotomyRegularity--;
					}
					iterationsCount++;
					return BetaSolverState.IntervalHalvingCycle;
				}
				return BetaSolverState.IntervalHalvingCycleBreak;
			}
			BetaSolverState IntervalHalvingCycleBreakChange() {
				if (beginX >= 1.0) {
					result = 1.0 - FunctionBetaDist.MachineEpsilon;
					return BetaSolverState.Result;
				}
				if (result <= 0.0) {
					result = 0.0;
					return BetaSolverState.Result;
				}
				return BetaSolverState.NewtonIterationsInit;
			}
			BetaSolverState NewtonIterationsInitChange() {
				if (newtonFlag)
					return BetaSolverState.Result;
				newtonFlag = true;
				gammaLn = FunctionBetaDist.GetLnBeta(currentAlpha, currentBeta);
				iterationsCount = 0;
				return BetaSolverState.NewtonIterationsCycle;
			}
			BetaSolverState NewtonIterationsCycleChange() {
				if (iterationsCount <= 7) {
					if (iterationsCount != 0)
						betaDist = FunctionBetaDist.GetResult(currentAlpha, currentBeta, result);
					if (betaDist < endY) {
						result = beginX;
						betaDist = endY;
					} else {
						if (betaDist > currentY) {
							result = endX;
							betaDist = currentY;
						} else {
							if (betaDist < beginY) {
								beginX = result;
								endY = betaDist;
							} else {
								endX = result;
								currentY = betaDist;
							}
						}
					}
					if (result == 1.0 || result == 0.0)
						return BetaSolverState.NewtonIterationsCycleBreak;
					double delta = (currentAlpha - 1.0) * Math.Log(result) + (currentBeta - 1.0) * Math.Log(1.0 - result) + gammaLn;
					if (delta < FunctionBetaDist.MinLog)
						return BetaSolverState.Result;
					if (delta > FunctionBetaDist.MaxLog)
						return BetaSolverState.NewtonIterationsCycleBreak;
					delta = (betaDist - beginY) / Math.Exp(delta);
					currentX = result - delta;
					if (currentX <= beginX) {
						betaDist = (result - beginX) / (endX - beginX);
						currentX = beginX + 0.5 * betaDist * (result - beginX);
						if (currentX <= 0.0)
							return BetaSolverState.NewtonIterationsCycleBreak;
					}
					if (currentX >= endX) {
						betaDist = (endX - result) / (endX - beginX);
						currentX = endX - 0.5 * betaDist * (endX - result);
						if (currentX >= 1.0)
							return BetaSolverState.NewtonIterationsCycleBreak;
					}
					result = currentX;
					if (Math.Abs(delta / result) < 128.0 * FunctionBetaDist.MachineEpsilon)
						return BetaSolverState.Result;
					iterationsCount++;
					return BetaSolverState.NewtonIterationsCycle;
				} else
					return BetaSolverState.NewtonIterationsCycleBreak;
			}
			BetaSolverState NewtonIterationsCycleBreakChange() {
				dihotomyThreshhold = 256.0 * FunctionBetaDist.MachineEpsilon;
				return BetaSolverState.IntervalHalvingInit;
			}
			double UpdateResult() {
				if (resultFlag)
					result = result <= FunctionBetaDist.MachineEpsilon ? 1.0 - FunctionBetaDist.MachineEpsilon : 1.0 - result;
				return result;
			}
			void SetCurrentAlphaBeta(double alpha, double beta) {
				currentAlpha = alpha;
				currentBeta = beta;
			}
			void InitDihotomy() {
				dihotomyRegularity = 0;
				dihotomy = 0.5;
			}
		} 
		static VariantValue GetResult(double probability, double alpha, double beta, double A, double B) {
			return (B - A) * GetResult(alpha, beta, probability) + A;
		}
		internal static double GetResult(double alpha, double beta, double probability) {
			return BetaFiniteStateMachine.Solve(alpha, beta, probability);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
