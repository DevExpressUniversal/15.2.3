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

using DevExpress.Office.Utils;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionLinest
	public class FunctionLinest : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "LINEST"; } }
		public override int Code { get { return 0x0031; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Array; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue knownY = arguments[0];
			if (arguments.Count == 1)
				return GetResult(knownY, VariantValue.Empty, true, false);
			VariantValue knownX = arguments[1];
			if (arguments.Count == 2)
				return GetResult(knownY, knownX, true, false);
			VariantValue constant = arguments[2];
			if (constant.IsError)
				return constant;
			if (constant.IsEmpty)
				return GetResult(knownY, knownX, true, false);
			constant = constant.ToBoolean(context);
			if (constant.IsError) {
				if (constant == VariantValue.ErrorGettingData)
					return constant;
				return VariantValue.ErrorInvalidValueInFunction;
			}
			if (arguments.Count == 3)
				return GetResult(knownY, knownX, constant.BooleanValue, false);
			VariantValue stats = arguments[3];
			if (stats.IsError)
				return stats;
			if (stats.IsEmpty)
				return GetResult(knownY, knownX, constant.BooleanValue, false);
			stats = stats.ToBoolean(context);
			if (stats.IsError) {
				if (stats == VariantValue.ErrorGettingData)
					return stats;
				return VariantValue.ErrorInvalidValueInFunction;
			}
			return GetResult(knownY, knownX, constant.BooleanValue, stats.BooleanValue);
		}
		protected virtual VariantValue GetResult(VariantValue knownY, VariantValue knownX, bool hasIntercept, bool hasAdditionalStatistics) {
			return RegressionMath.GetLinest(knownY, knownX, hasIntercept, hasAdditionalStatistics);
		}
	}
	#endregion
	#region ObservationsDirection (enum)
	public enum ObservationsDirection {
		HorizontalVector, 
		VerticalVector,
		ZVector
	}
	#endregion
	#region MatrixMath
	public static class MatrixMath {
		public static double[,] GetMult(double[,] left, double[,] right) {
			if (left == null || right == null)
				return null;
			int heightLeft = left.GetLength(0);
			int widthLeft = left.GetLength(1);
			int heightRight = right.GetLength(0);
			int widthRight = right.GetLength(1);
			if (widthLeft != heightRight)
				return null;
			double[,] result = new double[heightLeft, widthRight];
			for (int i = 0; i < heightLeft; i++)
				for (int j = 0; j < widthRight; j++) {
					double sum = 0;
					for (int k = 0; k < widthLeft; k++)
						sum += left[i, k] * right[k, j];
					result[i, j] = sum;
				}
			return result;
		}
		public static double[] GetMultVector(double[,] left, double[] right) {
			if (left == null || right == null)
				return null;
			int heightLeft = left.GetLength(0);
			int widthLeft = left.GetLength(1);
			int widthRight = right.Length;
			if (widthLeft != widthRight)
				return null;
			double[] result = new double[heightLeft];
			for (int i = 0; i < heightLeft; i++) {
				double sum = 0;
				for (int j = 0; j < widthLeft; j++)
					sum += left[i, j] * right[j];
				result[i] = sum;
			}
			return result;
		}
		public static double[,] GetTranspose(double[,] matrix) {
			if (matrix == null)
				return null;
			int height = matrix.GetLength(0);
			int width = matrix.GetLength(1);
			double[,] result = new double[width, height];
			for (int i = 0; i < height; i++)
				for (int j = 0; j < width; j++)
					result[j, i] = matrix[i, j];
			return result;
		}
	}
	#endregion
	#region RegressionFactorInfo
	public struct RegressionFactorInfo {
		#region Static Members
		public static RegressionFactorInfo CreateResultFactor(double[] observations) {
			return CreateCore(ResultIndex, observations);
		}
		public static RegressionFactorInfo Create(int index, double[] observations) {
			return CreateCore(index, observations);
		}
		static RegressionFactorInfo CreateCore(int index, double[] observations) {
			RegressionFactorInfo result = new RegressionFactorInfo();
			result.index = index;
			result.observations = observations;
			return result;
		}
		#endregion
		#region Fields
		internal const int ResultIndex = -1;
		int index;
		double[] observations;
		#endregion
		#region Properties
		public int Index { get { return index; } }
		public bool HasObservations { get { return observations != null; } }
		public double[] Observations { get { return observations; } }
		#endregion
		public bool GetMulticollinearity(RegressionFactorStatisticsInfo targetStatistics, List<RegressionFactorInfo> factors, List<RegressionFactorStatisticsInfo> factorsStatistics, bool hasIntercept) {
			int factorsCount = factors.Count;
			for (int i = 0; i < factorsCount; i++) {
				RegressionFactorInfo factor = factors[i];
				RegressionFactorStatisticsInfo statistics = factorsStatistics[i];
				if (index != factor.index && IsLinearCombination(targetStatistics, factor, statistics, hasIntercept))
					return true;
			}
			return false;
		}
		#region Internal
		bool IsLinearCombination(RegressionFactorStatisticsInfo targetStatistics, RegressionFactorInfo currentFactor, RegressionFactorStatisticsInfo currentStatistics, bool hasIntercept) {
			if (!HasObservations)
				return false;
			if (hasIntercept) {
				int count = observations.Length;
				if (count == 1)
					return true;
				double targetMean = targetStatistics.Mean;
				double targetVariance = targetStatistics.Variance;
				if (targetVariance == 0)
					return true;
				double currentMean = currentStatistics.Mean;
				double currentVariance = currentStatistics.Variance;
				double[] currentFactorObservations = currentFactor.Observations;
				double meanMultCurrentFactor = 0;
				for (int j = 0; j < count; j++) 
					meanMultCurrentFactor += (currentFactorObservations[j] * observations[j] - meanMultCurrentFactor) / (j + 1);
				double k = (meanMultCurrentFactor - targetMean * currentMean) / targetVariance;
				double b = currentMean - k * targetMean;
				for (int i = 0; i < count; i++)
					if (Math.Round(currentFactorObservations[i] - k * observations[i] - b, RegressionMath.Digits) != 0)
						return false;
			} else {
				double value = observations[0] / currentFactor.observations[0];
				int count = observations.Length;
				for (int i = 1; i < count; i++) 
					if (observations[i] / currentFactor.observations[i] != value)
						return false;
			}
			return true;
		}
		#endregion
	}
	#endregion
	#region RegressionFactorStatisticsInfo
	public struct RegressionFactorStatisticsInfo {
		#region Static Members
		public static RegressionFactorStatisticsInfo Create(double variance, double mean) {
			return Create(NoneCorrel, variance, mean);
		}
		public static RegressionFactorStatisticsInfo Create(double correlWithResult, double variance, double mean) {
			RegressionFactorStatisticsInfo result = new RegressionFactorStatisticsInfo();
			result.correlWithResult = Math.Round(correlWithResult, RegressionMath.Digits);
			result.variance = Math.Round(variance, RegressionMath.Digits);
			result.mean = Math.Round(mean, RegressionMath.Digits);
			return result;
		}
		#endregion
		#region Fields
		internal const double NoneCorrel = -2;
		double correlWithResult;
		double variance;
		double mean;
		#endregion
		#region Properties
		public double CorrelWithResult { get { return correlWithResult; } }
		public double Variance { get { return variance; } }
		public double Mean { get { return mean; } }
		#endregion
		public int Compare(RegressionFactorStatisticsInfo otherInfo) {
			int compare = CompareNoneCorrel(correlWithResult, otherInfo.correlWithResult);
			if (compare != 0)
				return compare;
			compare = GetCompareCore(correlWithResult, otherInfo.correlWithResult);
			if (compare != 0)
				return compare;
			compare = GetCompareCore(variance, otherInfo.variance);
			if (compare != 0)
				return compare;
			return GetCompareCore(mean, otherInfo.mean);
		}
		int CompareNoneCorrel(double left, double right) { 
			double noneCorrel = RegressionFactorStatisticsInfo.NoneCorrel;
			if (left == noneCorrel && right != noneCorrel)
				return -1;
			if (left != noneCorrel && right == noneCorrel)
				return 1;
			return 0;
		}
		int GetCompareCore(double left, double right) {
			double absLeft = Math.Abs(left);
			double absRight = Math.Abs(right);
			if (absLeft < absRight)
				return -1;
			return absLeft > absRight ? 1 : 0;
		}
	}
	#endregion
	#region RegressionExtendedFactorMatrixInfo
	public struct RegressionExtendedFactorMatrixInfo {
		#region Static Members
		public static RegressionExtendedFactorMatrixInfo Create(double[] resultObservations) {
			RegressionExtendedFactorMatrixInfo result = CreateCore(resultObservations);
			result.factorMatrix = RegressionFactorMatrixInfo.Create(resultObservations.Length);
			return result;
		}
		public static RegressionExtendedFactorMatrixInfo Create(double[] resultObservations, List<RegressionFactorInfo> factors) {
			Guard.ArgumentNotNull(factors, "factors");
			RegressionExtendedFactorMatrixInfo result = CreateCore(resultObservations);
			result.factorMatrix = RegressionFactorMatrixInfo.Create(factors);
			return result;
		}
		public static RegressionExtendedFactorMatrixInfo Create(double[] resultObservations, double[,] observations) {
			Guard.ArgumentNotNull(observations, "observations");
			RegressionExtendedFactorMatrixInfo result = CreateCore(resultObservations);
			result.factorMatrix = RegressionFactorMatrixInfo.Create(observations);
			return result;
		}
		static RegressionExtendedFactorMatrixInfo CreateCore(double[] resultObservations) {
			Guard.ArgumentNotNull(resultObservations, "resultObservations");
			RegressionExtendedFactorMatrixInfo result = new RegressionExtendedFactorMatrixInfo();
			result.resultFactor = RegressionFactorInfo.CreateResultFactor(resultObservations);
			return result;
		}
		#endregion
		#region Fields
		RegressionFactorInfo resultFactor;
		RegressionFactorMatrixInfo factorMatrix;
		#endregion
		#region Properties
		public RegressionFactorInfo ResultFactor { get { return resultFactor; } }
		public RegressionFactorMatrixInfo FactorMatrix { get { return factorMatrix; } }
		List<RegressionFactorInfo> Factors { get { return factorMatrix.Factors; } }
		#endregion
		public RegressionFactorStatisticsInfo GetBaseResultStatistics() {
			if (!resultFactor.HasObservations)
				return RegressionFactorStatisticsInfo.Create(0, 0);
			double[] resultObservations = resultFactor.Observations;
			double mean = RegressionMath.GetMean(resultObservations);
			double variance = RegressionMath.GetVariance(resultObservations, mean);
			return RegressionFactorStatisticsInfo.Create(variance, mean);
		}
		#region Additional Statistics
		public double GetTrendResultValue(int index, double[] coeffB) {
			if (CheckInvalidFactors(coeffB) || index < 0 || index >= resultFactor.Observations.Length)
				return 0;
			double result = coeffB[0];
			int factorsCount = Factors.Count;
			for (int i = 0; i < factorsCount; i++) {
				RegressionFactorInfo factor = Factors[i];
				int factorIndex = Factors[i].Index;
				result += coeffB[factor.Index + 1] * factor.Observations[index];
			}
			return result;
		}
		public double GetResidualSumSquares(double[] coeffB) {
			if (CheckInvalidFactors(coeffB))
				return 0;
			double result = 0;
			double[] observations = resultFactor.Observations;
			int count = observations.Length;
			for (int i = 0; i < count; i++) {
				double trendValue = GetTrendResultValue(i, coeffB);
				double different = observations[i] - trendValue;
				result += different * different;
			}
			return result;
		}
		public double GetRegressionSumSquares(double residualSumSquares, double totalSumSquares) {
			return totalSumSquares - residualSumSquares;
		}
		public double GetTotalSumSquares(RegressionFactorStatisticsInfo resultStatistics, bool hasIntercept) {
			if (!resultFactor.HasObservations)
				return 0;
			return GetTotalSumSquaresCore(resultStatistics.Mean, resultStatistics.Variance, hasIntercept);
		}
		double GetTotalSumSquaresCore(double resultMean, double resultVariance, bool hasIntercept) {
			double result = resultVariance;
			if (!hasIntercept)
				result += resultMean * resultMean;
			return result * (double)resultFactor.Observations.Length;
		}
		#endregion
		#region GetResult
		public VariantValue GetResult(bool hasIntercept, bool hasAdditionalStatistics) {
			if (Factors.Count == 1)
				return RegressionMath.GetPairLinest(resultFactor.Observations, Factors[0].Observations, hasIntercept, hasAdditionalStatistics);
			return GetMultipleResult(hasIntercept, hasAdditionalStatistics);
		}
		VariantValue GetMultipleResult(bool hasIntercept, bool hasAdditionalStatistics) {
			double[] resultObservations = resultFactor.Observations;
			RegressionFactorStatisticsInfo resultStatistics = GetBaseResultStatistics();
			List<RegressionFactorStatisticsInfo> factorsStatistics = factorMatrix.GetBaseStatistics(resultObservations, resultStatistics);
			factorMatrix.MultipleSelect(resultStatistics, factorsStatistics, hasIntercept);
			double[,] predicators = factorMatrix.GetPredicatorsMatrix(hasIntercept);
			double[,] transpose = MatrixMath.GetTranspose(predicators);
			double[,] inverse = factorMatrix.GetInverseLeftPartNormalMatrix(transpose, predicators);
			double[] coeffB = factorMatrix.GetCoeffB(resultObservations, inverse, transpose, hasIntercept);
			VariantArray array = CreateVariantArray(coeffB, hasAdditionalStatistics);
			if (hasAdditionalStatistics)
				CopyMultipleAdditionalStatisticsToArray(array, hasIntercept, resultStatistics, inverse, coeffB);
			return VariantValue.FromArray(array);
		}
		#endregion
		#region CreateVariantArray
		public VariantArray CreateVariantArray(double[] coeffB, bool hasAdditionalStatistics) {
			int countCoeffB = coeffB.Length;
			VariantArray result = VariantArray.Create(countCoeffB, hasAdditionalStatistics ? 5 : 1);
			for (int i = 0; i < countCoeffB; i++)
				result.SetValue(0, i, coeffB[countCoeffB - i - 1]);
			return result;
		}
		#endregion
		#region CopyMultipleAdditionalStatisticsToArray
		public void CopyMultipleAdditionalStatisticsToArray(VariantArray array, bool hasIntercept, RegressionFactorStatisticsInfo resultStatistics, double[,] inverse, double[] coeffB) {
			double resSS = GetResidualSumSquares(coeffB);
			int valuesCount = resultFactor.Observations.Length;
			int factorsCount = Factors.Count;
			int dF = RegressionMath.GetDegreeFreedom(valuesCount, factorsCount, hasIntercept);
			double stdErrorY = RegressionMath.GetStandardErrorY(resSS, dF);
			double totalSS = GetTotalSumSquares(resultStatistics, hasIntercept);
			double regSS = GetRegressionSumSquares(resSS, totalSS);
			double r2 = RegressionMath.GetRSquare(regSS, totalSS);
			double fStat = RegressionMath.GetFisherStatictic(valuesCount, r2, factorsCount, hasIntercept);
			double[] stdErrorsB = factorMatrix.GetMultipleStandartErrorsB(inverse, stdErrorY, hasIntercept);
			CopyAdditionalStatisticsToArrayCore(array, stdErrorsB, r2, stdErrorY, fStat, dF, regSS, resSS);
		}
		void CopyAdditionalStatisticsToArrayCore(VariantArray array, double[] stdErrorsB, double r2, double stdErrorY, double fStat, int dF, double regSS, double resSS) {
			int countStandartErrorB = stdErrorsB.Length;
			for (int i = 0; i < countStandartErrorB; i++)
				array.SetValue(1, i, stdErrorsB[countStandartErrorB - i - 1]);
			array.SetValue(2, 0, r2);
			array.SetValue(2, 1, stdErrorY);
			array.SetValue(3, 0, RegressionMath.GetResultValue(fStat));
			array.SetValue(3, 1, dF);
			array.SetValue(4, 0, regSS);
			array.SetValue(4, 1, resSS);
			int width = array.Width;
			if (countStandartErrorB == width - 1)
				array.SetValue(1, countStandartErrorB, VariantValue.ErrorValueNotAvailable);
			for (int i = 2; i < width; i++) {
				array.SetValue(2, i, VariantValue.ErrorValueNotAvailable);
				array.SetValue(3, i, VariantValue.ErrorValueNotAvailable);
				array.SetValue(4, i, VariantValue.ErrorValueNotAvailable);
			}
		}
		#endregion
		bool CheckInvalidFactors(double[] coeffB) {
			return !factorMatrix.HasFactors || coeffB == null || coeffB.Length == 0;
		}
	}
	#endregion
	#region RegressionFactorMatrixInfo
	public struct RegressionFactorMatrixInfo {
		#region Static Members
		public static RegressionFactorMatrixInfo Create(int observationsCount) {
			RegressionFactorMatrixInfo result = new RegressionFactorMatrixInfo();
			result.coeffsBCount = 2;
			result.PrepareDefaultFactors(observationsCount);
			return result;
		}
		public static RegressionFactorMatrixInfo Create(double[,] observations) {
			int factorsCount = observations.GetLength(0);
			int valuesCount = observations.GetLength(1);
			List<RegressionFactorInfo> factors = new List<RegressionFactorInfo>();
			for (int factorIndex = 0; factorIndex < factorsCount; factorIndex++) {
				double[] factorObservations = new double[valuesCount];
				for (int valueIndex = 0; valueIndex < valuesCount; valueIndex++)
					factorObservations[valueIndex] = observations[factorIndex, valueIndex];
				factors.Add(RegressionFactorInfo.Create(factorIndex, factorObservations));
			}
			return Create(factors);
		}
		public static RegressionFactorMatrixInfo Create(List<RegressionFactorInfo> factors) {
			Guard.ArgumentNotNull(factors, "factors");
			RegressionFactorMatrixInfo result = new RegressionFactorMatrixInfo();
			result.coeffsBCount = factors.Count + 1;
			result.factors = factors;
			return result;
		}
		#endregion
		#region Fields
		List<RegressionFactorInfo> factors;
		int coeffsBCount;
		#endregion
		#region Properies
		public List<RegressionFactorInfo> Factors { get { return factors; } }
		public int CoeffsBCount { get { return coeffsBCount; } }
		public bool HasFactors { get { return factors != null && factors.Count != 0; } }
		#endregion
		#region SelectFactors
		public void MultipleSelect(RegressionFactorStatisticsInfo resultInfo, List<RegressionFactorStatisticsInfo> factorsStatistics, bool hasIntercept) {
			if (coeffsBCount <= 1)
				return;
			Dictionary<RegressionFactorStatisticsInfo, List<int>> clusteredFactorIndexes = GetClusteredFactorIndexes(factorsStatistics);
			List<RegressionFactorInfo> selectedFactors = new List<RegressionFactorInfo>();
			List<RegressionFactorStatisticsInfo> selectedStatistics = new List<RegressionFactorStatisticsInfo>();
			while (clusteredFactorIndexes.Count > 0) {
				RegressionFactorStatisticsInfo statistics = GetMaxStatistics(clusteredFactorIndexes);
				int index = GetFactorIndex(clusteredFactorIndexes, statistics, hasIntercept);
				AddSelectedFactor(selectedFactors, selectedStatistics, factorsStatistics, index, hasIntercept);
				clusteredFactorIndexes.Remove(statistics);
			}
			if (selectedFactors[0].Observations.Length == 1) {
				int selectedFactorsCount = selectedFactors.Count;
				for (int i = 1; i < selectedFactorsCount; i++)
					selectedFactors.RemoveAt(selectedFactors.Count - 1);
			}
			this.factors = selectedFactors;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		public RegressionFactorStatisticsInfo GetMaxStatistics(Dictionary<RegressionFactorStatisticsInfo, List<int>> clusteredFactorIndexes) {
			RegressionFactorStatisticsInfo result = RegressionFactorStatisticsInfo.Create(0, 0);
			foreach (RegressionFactorStatisticsInfo currentIndex in clusteredFactorIndexes.Keys)
				if (currentIndex.Compare(result) == 1)
					result = currentIndex;
			return result;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		public Dictionary<RegressionFactorStatisticsInfo, List<int>> GetClusteredFactorIndexes(List<RegressionFactorStatisticsInfo> factorInfoes) {
			Dictionary<RegressionFactorStatisticsInfo, List<int>> result = new Dictionary<RegressionFactorStatisticsInfo, List<int>>();
			int factorInfoesCount = factorInfoes.Count;
			for (int i = 0; i < factorInfoesCount; i++) 
				AddStatisticsIndex(result, factorInfoes[i], i);
			return result;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		public int GetFactorIndex(Dictionary<RegressionFactorStatisticsInfo, List<int>> clusteredFactorIndexes, RegressionFactorStatisticsInfo index, bool hasIntercept) { 
			List<int> value = clusteredFactorIndexes[index];
			return value.Count == 1 ? value[0] : value[hasIntercept ? 1 : 0];
		}
		#endregion
		#region Calculate
		public List<RegressionFactorStatisticsInfo> GetBaseStatistics(double[] resultObservations, RegressionFactorStatisticsInfo resultStatistics) {
			List<RegressionFactorStatisticsInfo> result = new List<RegressionFactorStatisticsInfo>();
			if (coeffsBCount <= 1)
				return result;
			int factorsCount = coeffsBCount - 1;
			for (int i = 0; i < factorsCount; i++) {
				RegressionFactorStatisticsInfo info = CreateFactorStatisticsInfo(resultObservations, resultStatistics, factors[i].Observations);
				result.Add(info);
			}
			return result;
		}
		public double[,] GetPredicatorsMatrix(bool hasIntercept) {
			int factorsCount = factors.Count;
			if (factorsCount == 0)
				return null;
			int height = Factors[0].Observations.Length;
			int width = factorsCount + (hasIntercept && height > 1 ? 1 : 0);
			double[,] result = new double[height, width];
			for (int row = 0; row < height; row++)
				result[row, 0] = hasIntercept ? 1 : factors[0].Observations[row];
			if (height == 1)
				return result;
			int column = 1;
			for (int i = hasIntercept ? 0 : 1; i < factorsCount; i++) {
				double[] observations = factors[i].Observations;
				for (int row = 0; row < height; row++)
					result[row, column] = observations[row];
				column++;
			}
			return result;
		}
		public double[,] GetInverseLeftPartNormalMatrix(double[,] transposePredicators, double[,] predicators) {
			double[,] mult = MatrixMath.GetMult(transposePredicators, predicators);
			return SquareMatrix.TryGetInverse(mult);
		}
		public double[] GetCoeffB(double[] resultObservations, double[,] inverse, double[,] transpose, bool hasIntercept) {
			double[] result = new double[coeffsBCount];
			if (!HasFactors)
				return result;
			double[] selectedCoeffB = GetCoeffBCore(resultObservations, inverse, transpose);
			if (selectedCoeffB == null)
				return result;
			int selectedCoeffBCount = selectedCoeffB.Length;
			if (hasIntercept) {
				result[0] = selectedCoeffB[0];
				if (selectedCoeffBCount == 1)
					return result;
			}
			int intercept = hasIntercept ? 1 : 0;
			int factorsCount = Factors.Count;
			for (int i = 0; i < factorsCount; i++) {
				int factorIndex = Factors[i].Index;
				int index = i + intercept;
				if (index < selectedCoeffBCount)
					result[factorIndex + 1] = selectedCoeffB[index];
				else break;
			}
			return result;
		}
		public double[] GetMultipleStandartErrorsB(double[,] inverse, double standardErrorY, bool hasIntercept) {
			int intercept = hasIntercept ? 1 : 0;
			double[] result = new double[coeffsBCount - 1 + intercept];
			if (!HasFactors || inverse == null)
				return result;
			if (hasIntercept)
				result[0] = GetStandartErrorB(0, inverse, standardErrorY);
			int factorsCount = factors.Count;
			int inverseWidth = inverse.GetLength(0);
			for (int i = 0; i < factorsCount; i++) {
				int factorIndex = factors[i].Index;
				int index = i + intercept;
				if (index < inverseWidth)
					result[factorIndex + intercept] = GetStandartErrorB(index, inverse, standardErrorY);
				else break;
			}
			return result;
		}
		#endregion
		#region Internal
		void PrepareDefaultFactors(int observationsCount) {
			factors = new List<RegressionFactorInfo>();
			if (observationsCount == 0)
				return;
			double[] observations = new double[observationsCount];
			for (int i = 0; i < observationsCount; i++)
				observations[i] = i + 1;
			factors.Add(RegressionFactorInfo.Create(0, observations));
		}
		RegressionFactorStatisticsInfo CreateFactorStatisticsInfo(double[] resultObservations, RegressionFactorStatisticsInfo resultStatistics, double[] observations) {
			double mean = RegressionMath.GetMean(observations);
			double variance = RegressionMath.GetVariance(observations, mean);
			if (resultStatistics.Variance == 0 || variance == 0)
				return RegressionFactorStatisticsInfo.Create(variance, mean);
			double correlWithResult = RegressionMath.GetCorrel(resultObservations, observations, resultStatistics.Mean, resultStatistics.Variance, mean, variance);
			return RegressionFactorStatisticsInfo.Create(correlWithResult, variance, mean);
		}
		double GetStandartErrorB(int index, double[,] inverse, double standardErrorY) {
			return Math.Sqrt(inverse[index, index]) * standardErrorY;
		}
		bool IsZeroFactor(RegressionFactorStatisticsInfo statictics) {
			return RegressionMath.IsZeroFactor(statictics.Mean, statictics.Variance);
		}
		void AddSelectedFactor(List<RegressionFactorInfo> selectedFactors, List<RegressionFactorStatisticsInfo> selectedStatistics, List<RegressionFactorStatisticsInfo> factorsStatistics, int index, bool hasIntercept) {
			RegressionFactorInfo factor = Factors[index];
			RegressionFactorStatisticsInfo statistics = factorsStatistics[index];
			if (!IsZeroFactor(statistics) && !factor.GetMulticollinearity(statistics, selectedFactors, selectedStatistics, hasIntercept)) {
				selectedStatistics.Add(statistics);
				selectedFactors.Add(factor);
			}
		}
		void AddStatisticsIndex(Dictionary<RegressionFactorStatisticsInfo, List<int>> result, RegressionFactorStatisticsInfo info, int index) {
			if (result.ContainsKey(info))
				result[info].Add(index);
			else {
				List<int> newValue = new List<int>();
				newValue.Add(index);
				result.Add(info, newValue);
			}
		}
		double[] GetCoeffBCore(double[] resultObservations, double[,] inverse, double[,] transpose) {
			double[,] mult = MatrixMath.GetMult(inverse, transpose);
			return MatrixMath.GetMultVector(mult, resultObservations);
		}
		#endregion
	}
	#endregion
	#region CorrelationMath
	public static class CorrelationMath {
		#region GetSlope
		public static VariantValue GetSlope(VariantValue knownY, VariantValue knownX) {
			IVector<VariantValue> knownYVector;
			IVector<VariantValue> knownXVector;
			VariantValue error = CalculateZVectors(knownY, knownX, out knownYVector, out knownXVector);
			if (error.IsError)
				return error;
			return GetSlopeCore(knownYVector, knownXVector);
		}
		static VariantValue GetSlopeCore(IVector<VariantValue> knownY, IVector<VariantValue> knownX) {
			double[] means = new double[2];
			VariantValue error = CalculateMeans(knownY, knownX, means, true);
			if (error.IsError)
				return error;
			return GetSlopeCore(knownY, knownX, means[0], means[1], true);
		}
		static VariantValue GetSlopeCore(IVector<VariantValue> vectorY, IVector<VariantValue> vectorX, double meanY, double meanX, bool checkErrors) {
			int valuesCount = 0;
			double sumCenteredXY = 0;
			double sumCenteredXX = 0;
			int count = vectorY.Count;
			for (int i = 0; i < count; i++) {
				VariantValue valueY = vectorY[i];
				VariantValue valueX = vectorX[i];
				if (valueY.IsError)
					return valueY;
				if (valueX.IsError)
					return valueX;
				if (valueY.IsNumeric && valueX.IsNumeric) {
					double centeredY = valueY.NumericValue - meanY;
					double centeredX = valueX.NumericValue - meanX;
					sumCenteredXY += centeredY * centeredX;
					sumCenteredXX += centeredX * centeredX;
					valuesCount++;
				}
			}
			if (valuesCount == 0 || sumCenteredXX == 0)
				return GetErrorDivisionByZero(checkErrors);
			if (Double.IsInfinity(sumCenteredXY) || Double.IsInfinity(sumCenteredXX))
				return GetErrorNumber(checkErrors);
			return sumCenteredXY / sumCenteredXX;
		}
		static VariantValue GetErrorDivisionByZero(bool checkDivisionByZero) {
			return checkDivisionByZero ? VariantValue.ErrorDivisionByZero : VariantValue.Empty;
		}
		static VariantValue GetErrorNumber(bool checkErrorNumber) {
			return checkErrorNumber ? VariantValue.ErrorNumber : VariantValue.Empty;
		}
		#endregion
		#region GetIntercept
		public static VariantValue GetIntercept(VariantValue knownY, VariantValue knownX) {
			IVector<VariantValue> knownYVector;
			IVector<VariantValue> knownXVector;
			VariantValue error = CalculateZVectors(knownY, knownX, out knownYVector, out knownXVector);
			if (error.IsError)
				return error;
			return GetInterceptCore(knownYVector, knownXVector, true);
		}
		static VariantValue GetInterceptCore(IVector<VariantValue> knownY, IVector<VariantValue> knownX, bool checkErrors) {
			double[] means = new double[2];
			VariantValue error = CalculateMeans(knownY, knownX, means, checkErrors);
			if (error.IsError)
				return error;
			double meanY = means[0];
			double meanX = means[1];
			VariantValue slopeValue = GetSlopeCore(knownY, knownX, meanY, meanX, checkErrors);
			if (slopeValue.IsError)
				return slopeValue;
			return GetInterceptCore(slopeValue.NumericValue, meanY, meanX, checkErrors);
		}
		static VariantValue GetInterceptCore(double slope, double meanY, double meanX, bool checkErrors) {
			double slopeMultMeanX = slope * meanX;
			if (double.IsInfinity(slopeMultMeanX))
				return GetErrorNumber(checkErrors);
			return meanY - slopeMultMeanX;
		}
		#endregion
		#region GetForecast
		public static VariantValue GetForecast(double number, VariantValue knownY, VariantValue knownX) {
			IVector<VariantValue> knownYVector;
			IVector<VariantValue> knownXVector;
			VariantValue error = CalculateZVectors(knownY, knownX, out knownYVector, out knownXVector);
			if (error.IsError)
				return error;
			return GetForecastCore(number, knownYVector, knownXVector, true);
		}
		static VariantValue GetForecastCore(double number, IVector<VariantValue> knownY, IVector<VariantValue> knownX, bool checkErrors) {
			double[] means = new double[2];
			VariantValue error = CalculateMeans(knownY, knownX, means, checkErrors);
			if (error.IsError)
				return error;
			double meanY = means[0];
			double meanX = means[1];
			VariantValue slopeValue = GetSlopeCore(knownY, knownX, meanY, meanX, checkErrors);
			if (slopeValue.IsError)
				return slopeValue;
			double slope = slopeValue.NumericValue;
			VariantValue interceptValue = GetInterceptCore(slope, meanY, meanX, checkErrors);
			if (interceptValue.IsError)
				return interceptValue;
			double result = slope * number + interceptValue.NumericValue;
			if (double.IsInfinity(result))
				return GetErrorNumber(checkErrors);
			return result;
		}
		#endregion
		#region GetPearson
		public static VariantValue GetPearson(VariantValue knownY, VariantValue knownX) {
			return GetCorrelationStatistics(knownY, knownX, true, GetPearsonCore);
		}
		static VariantValue GetPearsonCore(int valuesCount, double sumCenteredXY, double sumCenteredYY, double sumCenteredXX, bool checkErrors) {
			double part = sumCenteredYY * sumCenteredXX;
			if (Double.IsInfinity(part))
				return GetErrorNumber(checkErrors);
			return sumCenteredXY / Math.Sqrt(part);
		}
		#endregion
		#region GetStandartErrorYX
		public static VariantValue GetStandartErrorYX(VariantValue knownY, VariantValue knownX) {
			return GetCorrelationStatistics(knownY, knownX, true, GetStandartErrorYXCore);
		}
		static VariantValue GetStandartErrorYXCore(int valuesCount, double sumCenteredXY, double sumCenteredYY, double sumCenteredXX, bool checkErrors) {
			if (valuesCount < 3)
				return GetErrorDivisionByZero(checkErrors);
			double part = sumCenteredXY * sumCenteredXY;
			if (Double.IsInfinity(part))
				return GetErrorNumber(checkErrors);
			part = (sumCenteredYY - part / sumCenteredXX) / (valuesCount - 2);
			if (part < 0)
				return GetErrorNumber(checkErrors);
			return Math.Sqrt(part);
		}
		#endregion
		#region CalculateZVectors
		static VariantValue CalculateZVectors(VariantValue firstValue, VariantValue secondValue, out IVector<VariantValue> firstVector, out IVector<VariantValue> secondVector) {
			firstVector = null;
			secondVector = null;
			if (firstValue.IsError)
				return firstValue;
			if (IsNotArrayCellRangeOrNumeric(firstValue))
				return VariantValue.ErrorInvalidValueInFunction;
			firstVector = CreateZVector(firstValue);
			int firstVectorCount = firstVector.Count;
			if (firstVectorCount == 1) {
				VariantValue error = CheckErrorByEmptyFirstValue(firstVector[0]);
				if (error.IsError)
					return error;
			}
			if (secondValue.IsError)
				return secondValue;
			if (IsNotArrayCellRangeOrNumeric(secondValue))
				return VariantValue.ErrorInvalidValueInFunction;
			secondVector = CreateZVector(secondValue);
			int secondVectorCount = secondVector.Count;
			if (secondVectorCount == 1) {
				VariantValue error = CheckErrorByEmptyFirstValue(secondVector[0]);
				if (error.IsError)
					return error;
			}
			if (firstVectorCount != secondVectorCount)
				return VariantValue.ErrorValueNotAvailable;
			return VariantValue.Empty;
		}
		static VariantValue CheckErrorByEmptyFirstValue(VariantValue firstValue) {
			if (firstValue.IsError)
				return firstValue;
			if (!firstValue.IsNumeric)
				return VariantValue.ErrorInvalidValueInFunction;
			return VariantValue.Empty;
		}
		static bool IsNotArrayCellRangeOrNumeric(VariantValue value) {
			if (value.IsCellRange)
				return value.CellRangeValue.RangeType == CellRangeType.UnionRange;
			return !value.IsArray && !value.IsNumeric;
		}
		static IVector<VariantValue> CreateZVector(VariantValue value) {
			if (value.IsNumeric) {
				VariantArray array = VariantArray.Create(1, 1);
				array.SetValue(0, 0, value);
				return new ArrayZVector(array);
			}
			if (value.IsArray)
				return new ArrayZVector(value.ArrayValue);
			return new RangeZVector(value.CellRangeValue.GetFirstInnerCellRange());
		}
		#endregion
		#region CalculateStatictics
		#region CalculateMeans
		static VariantValue CalculateMeans(IVector<VariantValue> vectorY, IVector<VariantValue> vectorX, double[] means, bool checkErrors) {
			int valuesCount = 0;
			double meanY = 0;
			double meanX = 0;
			int count = vectorY.Count;
			for (int i = 0; i < count; i++) {
				VariantValue valueY = vectorY[i];
				VariantValue valueX = vectorX[i];
				if (valueY.IsError)
					return valueY;
				if (valueX.IsError)
					return valueX;
				if (valueY.IsNumeric && valueX.IsNumeric) {
					meanY += valueY.NumericValue;
					meanX += valueX.NumericValue;
					valuesCount++;
				}
			}
			if (valuesCount == 0)
				return GetErrorDivisionByZero(checkErrors);
			meanY /= valuesCount;
			meanX /= valuesCount;
			means[0] = meanY;
			means[1] = meanX;
			return VariantValue.Empty;
		}
		#endregion
		#region GetCorrelationStatistics
		delegate VariantValue GetCorrelationStatisticsCore(int valuesCount, double sumCenteredXY, double sumCenteredYY, double sumCenteredXX, bool checkErrors);
		static VariantValue GetCorrelationStatistics(VariantValue knownY, VariantValue knownX, bool checkErrors, GetCorrelationStatisticsCore GetCorrelationStatisticsCore) {
			IVector<VariantValue> knownYVector;
			IVector<VariantValue> knownXVector;
			VariantValue error = CalculateZVectors(knownY, knownX, out knownYVector, out knownXVector);
			if (error.IsError)
				return error;
			double[] means = new double[2];
			error = CalculateMeans(knownYVector, knownXVector, means, true);
			if (error.IsError)
				return error;
			double sumCenteredXY = 0;
			double sumCenteredYY = 0;
			double sumCenteredXX = 0;
			int valuesCount = 0;
			int count = knownXVector.Count;
			for (int i = 0; i < count; i++) {
				VariantValue valueY = knownYVector[i];
				VariantValue valueX = knownXVector[i];
				if (valueY.IsError)
					return valueY;
				if (valueX.IsError)
					return valueX;
				if (valueY.IsNumeric && valueX.IsNumeric) {
					double centeredY = valueY.NumericValue - means[0];
					double centeredX = valueX.NumericValue - means[1];
					sumCenteredXY += centeredY * centeredX;
					sumCenteredYY += centeredY * centeredY;
					sumCenteredXX += centeredX * centeredX;
					valuesCount++;
				}
			}
			if (valuesCount == 0 || sumCenteredYY == 0 || sumCenteredXX == 0)
				return GetErrorDivisionByZero(checkErrors);
			if (Double.IsInfinity(sumCenteredXY) || Double.IsInfinity(sumCenteredYY) || Double.IsInfinity(sumCenteredXX))
				return GetErrorNumber(checkErrors);
			return GetCorrelationStatisticsCore(valuesCount, sumCenteredXY, sumCenteredYY, sumCenteredXX, checkErrors);
		}
		#endregion
		#endregion
	}
	#endregion
	#region RegressionMath
	public static class RegressionMath {
		internal static int Digits { get { return 15; } }
		#region ConvertToVariantArray
		public static IVariantArray ConvertToVariantArray(VariantValue value) {
			if (value.IsNumeric) {
				VariantArray result = VariantArray.Create(1, 1);
				result[0] = value;
				return result;
			}
			if (value.IsArray)
				return value.ArrayValue;
			CellRange range = value.CellRangeValue.GetFirstInnerCellRange();
			return new RangeVariantArray(range);
		}
		public static VariantArray ConvertToVariantArray(double[,] array) {
			int height = array.GetLength(0);
			int width = array.GetLength(1);
			VariantArray result = VariantArray.Create(width, height);
			for (int i = 0; i < height; i++)
				for (int j = 0; j < width; j++) {
					VariantValue value = GetResultValue(array[i, j]);
					result.SetValue(i, j, value);
				}
			return result;
		}
		internal static VariantValue GetResultValue(double value) {
			return Double.IsNaN(value) ? VariantValue.ErrorValueNotAvailable : Double.IsInfinity(value) ? VariantValue.ErrorNumber : value;
		}
		#endregion
		#region GetLinest
		public static VariantValue GetLinest(VariantValue knownY, VariantValue knownX, bool hasIntercept, bool hasAdditionalStatistics) {
			VariantValue error = CheckRequiredLinestFactors(knownY);
			if (error.IsError)
				return error;
			error = CheckOptionalLinestFactors(knownX);
			if (error.IsError)
				return error;
			IVariantArray knownYArray = ConvertToVariantArray(knownY);
			IVariantArray knownXArray = knownX.IsEmpty ? null : ConvertToVariantArray(knownX);
			return CalculateLinearCoeffsB(knownYArray, knownXArray, hasIntercept, hasAdditionalStatistics, TryGetLinearResultObservations);
		}
		#endregion
		#region GetLogest
		public static VariantValue GetLogest(VariantValue knownY, VariantValue knownX, bool hasIntercept, bool hasAdditionalStatistics) {
			VariantValue error = CheckRequiredLinestFactors(knownY);
			if (error.IsError)
				return error;
			error = CheckOptionalLinestFactors(knownX);
			if (error.IsError)
				return error;
			IVariantArray knownYArray = ConvertToVariantArray(knownY);
			IVariantArray knownXArray = knownX.IsEmpty ? null : ConvertToVariantArray(knownX);
			return GetLogestCore(knownYArray, knownXArray, hasIntercept, hasAdditionalStatistics);
		}
		static VariantValue GetLogestCore(IVariantArray knownY, IVariantArray knownX, bool hasIntercept, bool hasAdditionalStatistics) {
			VariantValue result = CalculateLinearCoeffsB(knownY, knownX, hasIntercept, hasAdditionalStatistics, TryGetLogResultObservations);
			if (result.IsError)
				return result;
			ReplaceCoeffsBByExp(result.ArrayValue);
			return result;
		}
		#endregion
		#region GetTrend
		public static VariantValue GetTrend(VariantValue knownY, VariantValue knownX, VariantValue newX, bool hasIntercept) {
			VariantValue error = CheckRequiredLinestFactors(knownY);
			if (error.IsError)
				return error;
			error = CheckOptionalLinestFactors(knownX);
			if (error.IsError)
				return error;
			error = CheckOptionalLinestFactors(newX);
			if (error.IsError)
				return error;
			IVariantArray knownYArray = ConvertToVariantArray(knownY);
			IVariantArray knownXArray = knownX.IsEmpty ? null : ConvertToVariantArray(knownX);
			IVariantArray newXArray = newX.IsEmpty ? null : ConvertToVariantArray(newX);
			return GetTrendCore(knownYArray, knownXArray, newXArray, hasIntercept);
		}
		#endregion
		#region GetGrowth
		public static VariantValue GetGrowth(VariantValue knownY, VariantValue knownX, VariantValue newX, bool hasIntercept) {
			VariantValue error = CheckRequiredLinestFactors(knownY);
			if (error.IsError)
				return error;
			error = CheckOptionalLinestFactors(knownX);
			if (error.IsError)
				return error;
			error = CheckOptionalLinestFactors(newX);
			if (error.IsError)
				return error;
			IVariantArray knownYArray = ConvertToVariantArray(knownY);
			IVariantArray knownXArray = knownX.IsEmpty ? null : ConvertToVariantArray(knownX);
			IVariantArray newXArray = newX.IsEmpty ? null : ConvertToVariantArray(newX);
			return GetGrowthCore(knownYArray, knownXArray, newXArray, hasIntercept);
		}
		#endregion
		#region PrepareFactors
		public static VariantValue PrepareFactors(IVariantArray knownX, List<RegressionFactorInfo> factors, ObservationsDirection direction, long observationsCount) {
			long factorsCount = GetFactorsCount(knownX, direction);
			long factorsObservationsCount = GetObservationsCount(knownX, direction);
			if (factorsObservationsCount != observationsCount)
				return VariantValue.ErrorReference;
			for (int i = 0; i < factorsCount; i++) {
				double[] observations = new double[observationsCount];
				VariantValue error = TryGetObservations(knownX, observations, i, direction);
				if (error.IsError)
					return error;
				factors.Add(RegressionFactorInfo.Create(i, observations));
			}
			return VariantValue.Empty;
		}
		#endregion
		#region TryGetObservations
		public static VariantValue TryGetLinearResultObservations(IVariantArray value, double[] result) {
			long count = value.Count;
			for (int i = 0; i < count; i++) {
				VariantValue number = value[i];
				if (number.IsNumeric)
					result[i] = number.NumericValue;
				else
					return number == VariantValue.ErrorGettingData ? number : VariantValue.ErrorInvalidValueInFunction;
			}
			return VariantValue.Empty;
		}
		public static VariantValue TryGetLogResultObservations(IVariantArray value, double[] result) {
			long count = value.Count;
			for (int i = 0; i < count; i++) {
				VariantValue numValue = value[i];
				if (numValue.IsNumeric) {
					double number = numValue.NumericValue;
					if (number < 1)
						return VariantValue.ErrorNumber;
					result[i] = Math.Log(number);
				}
				else
					return numValue == VariantValue.ErrorGettingData ? numValue : VariantValue.ErrorInvalidValueInFunction;
			}
			return VariantValue.Empty;
		}
		public static VariantValue TryGetObservations(IVariantArray value, double[] result, int vectorIndex, ObservationsDirection direction) {
			long observationsCount = result.Length;
			for (int i = 0; i < observationsCount; i++) {
				VariantValue number = GetObservation(value, vectorIndex, i, direction);
				if (number.IsNumeric)
					result[i] = number.NumericValue;
				else
					return number == VariantValue.ErrorGettingData ? number : VariantValue.ErrorInvalidValueInFunction;
			}
			return VariantValue.Empty;
		}
		#endregion
		#region GetMean
		public static double GetMean(double[] vector) {
			double result = 0;
			int count = vector.Length;
			for (int i = 0; i < count; i++)
				result += vector[i];
			return count == 0 ? 0 : result / count;
		}
		#endregion
		#region GetVariance
		public static double GetVariance(double[] vector, double mean) {
			double result = 0;
			int count = vector.Length;
			for (int i = 0; i < count; i++) {
				double centeredValue = vector[i] - mean;
				result += centeredValue * centeredValue;
			}
			return count == 0 ? 0 : result / count;
		}
		public static double[] GetVariances(double[] vectorY, double[] vectorX, double meanY, double meanX) {
			double varianceY = 0;
			double varianceX = 0;
			int count = vectorY.Length;
			for (int i = 0; i < count; i++) {
				double centeredValueY = vectorY[i] - meanY;
				double centeredValueX = vectorX[i] - meanX;
				varianceY += centeredValueY * centeredValueY;
				varianceX += centeredValueX * centeredValueX;
			}
			if (count > 0) {
				varianceY /= count;
				varianceX /= count;
			}
			return new double[2] { varianceY, varianceX };
		}
		#endregion
		#region GetCorrel
		public static double GetCorrel(double[] vectorY, double[] vectorX, double meanY, double varianceY, double meanX, double varianceX) {
			double result = 0;
			int count = vectorX.Length;
			for (int i = 0; i < count; i++) {
				double centeredValueX = vectorX[i] - meanX;
				double centeredValueY = vectorY[i] - meanY;
				result += centeredValueX * centeredValueY;
			}
			double denaminator = varianceX * varianceY;
			if (count > 0 && denaminator > 0)
				result /= count * Math.Sqrt(denaminator);
			return result;
		}
		#endregion
		#region GetPairLinest
		public static VariantValue GetPairLinest(double[] vectorY, double[] vectorX, bool hasIntercept, bool hasAdditionalStatistics) {
			double[,] linestResult = GetPairLinestCore(vectorY, vectorX, hasIntercept, hasAdditionalStatistics);
			VariantArray array = ConvertToVariantArray(linestResult);
			return VariantValue.FromArray(array);
		}
		public static double[,] GetPairLinestCore(double[] vectorY, double[] vectorX, bool hasIntercept, bool hasAdditionalStatistics) {
			double[] statistics = new double[5];
			if (!CheckEmptyVector(vectorX))
				CalculatePairStatistics(vectorY, vectorX, statistics);
			else
				CalculatePairStatistics(vectorY, statistics);
			double sumYY = statistics[0];
			double sumY = statistics[1];
			double sumXX = statistics[2];
			double sumX = statistics[3];
			double sumXY = statistics[4];
			int valuesCount = vectorY.Length;
			double sumCenteredYY = GetSumCentered(hasIntercept, sumYY, sumY, valuesCount);
			double sumCenteredXX = GetSumCentered(hasIntercept, sumXX, sumX, valuesCount);
			double sumCenteredXY = GetSumCenteredXY(hasIntercept, sumXY, sumY, sumX, valuesCount);
			double b1 = GetSlope(sumCenteredXY, sumCenteredXX);
			double b0 = hasIntercept ? GetIntercept(b1, sumY, sumX, valuesCount) : 0;
			if (hasAdditionalStatistics) {
				double regSS = sumCenteredXX == 0 ? 0 : sumCenteredXY * sumCenteredXY / sumCenteredXX;
				double resSS = sumCenteredYY - regSS;
				double r2 = sumCenteredYY == 0 ? 1 : regSS / sumCenteredYY;
				int factorsCount = GetPairFactorsCount(hasIntercept, sumCenteredYY, sumCenteredXX, sumY, sumX);
				int dF = GetDegreeFreedom(valuesCount, factorsCount, hasIntercept);
				double stdErrorY = GetStandardErrorY(resSS, dF);
				double fStat = GetFisherStatictic(valuesCount, r2, factorsCount, hasIntercept);
				double stdErrorB1 = 0;
				double stdErrorB0 = hasIntercept ? 0 : Double.NaN;
				if (valuesCount != 0 && sumCenteredXX != 0) {
					stdErrorB1 = stdErrorY / Math.Sqrt(sumCenteredXX);
					if (hasIntercept)
						stdErrorB0 = stdErrorB1 * Math.Sqrt(sumXX / (double)valuesCount);
				}
				return GetPairLinestCore(b1, b0, stdErrorB1, stdErrorB0, r2, stdErrorY, fStat, dF, regSS, resSS);
			}
			return GetPairLinestCore(b1, b0);
		}
		static double[,] GetPairLinestCore(double b1, double b0, double stdErrorB1, double stdErrorB0, double r2, double stdErrorY, double fStat, int dF, double regSS, double resSS) {
			return new double[5, 2] { { b1, b0 }, 
									  { stdErrorB1, stdErrorB0},  
									  { r2, stdErrorY},  
									  { fStat, dF},  
									  { regSS, resSS} };
		}
		static double[,] GetPairLinestCore(double b1, double b0) {
			return new double[1, 2] { { b1, b0 } };
		}
		static bool CheckEmptyVector(double[] vector) {
			return vector == null || vector.Length == 0;
		}
		#endregion
		#region CalculatePairStatistics
		static void CalculatePairStatistics(double[] vectorY, double[] vectorX, double[] statistics) {
			double sumYY = 0;
			double sumY = 0;
			double sumXX = 0;
			double sumX = 0;
			double sumXY = 0;
			int count = vectorY.Length;
			for (int i = 0; i < count; i++) {
				double valueY = vectorY[i];
				sumYY += valueY * valueY;
				sumY += valueY;
				double valueX = vectorX[i];
				sumXX += valueX * valueX;
				sumX += valueX;
				sumXY += valueX * valueY;
			}
			statistics[0] = sumYY;
			statistics[1] = sumY;
			statistics[2] = sumXX;
			statistics[3] = sumX;
			statistics[4] = sumXY;
		}
		static void CalculatePairStatistics(double[] vectorY, double[] statistics) {
			double sumYY = 0;
			double sumY = 0;
			int count = vectorY.Length;
			for (int i = 0; i < count; i++) {
				double valueY = vectorY[i];
				sumYY += valueY * valueY;
				sumY += valueY;
			}
			statistics[0] = sumYY;
			statistics[1] = sumY;
		}
		static double GetSlope(double sumCenteredXY, double sumCenteredXX) {
			return sumCenteredXX == 0 ? 0 : sumCenteredXY / sumCenteredXX;
		}
		static double GetIntercept(double slope, double sumY, double sumX, double count) {
			return count == 0 ? 0 : (sumY - slope * sumX) / count;
		}
		static double GetSumCentered(bool hasIntercept, double sumSquares, double sum, double count) {
			return hasIntercept ? (count == 0 ? 0 : sumSquares - sum * sum / count) : sumSquares;
		}
		static double GetSumCenteredXY(bool hasIntercept, double sumXY, double sumY, double sumX, double count) {
			return hasIntercept ? (count == 0 ? 0 : sumXY - sumY * sumX / count) : sumXY;
		}
		static int GetPairFactorsCount(bool hasIntercept, double sumCenteredYY, double sumCenteredXX, double sumY, double sumX) {
			return IsZeroFactor(sumY, sumCenteredYY) && IsZeroFactor(sumX, sumCenteredXX) && !hasIntercept ? 0 : 1;
		}
		#endregion
		#region Additional Statistics
		internal static bool IsZeroFactor(double mean, double variance) {
			return mean == 0 && variance == 0;
		}
		internal static int GetDegreeFreedom(int valuesCount, int factorsCount, bool hasIntercept) {
			if (valuesCount == 0)
				return 0;
			int different = valuesCount - factorsCount - (hasIntercept ? 1 : 0);
			return different < 0 ? 0 : different;
		}
		internal static double GetStandardErrorY(double residualSumSquares, int degreeFreedom) {
			return degreeFreedom == 0 ? 0 : Math.Sqrt(residualSumSquares / (double)degreeFreedom);
		}
		internal static double GetFisherStatictic(int valuesCount, double rSquare, int factorsCount, bool hasIntercept) {
			if (valuesCount == 0)
				return double.NegativeInfinity;
			if (rSquare == 1 || factorsCount == 0)
				return double.PositiveInfinity;
			double result = rSquare / (1 - rSquare);
			result *= ((double)valuesCount - (double)factorsCount - (hasIntercept ? 1 : 0)) / (double)factorsCount;
			return result;
		}
		internal static double GetRSquare(double regressionSumSquares, double totalSumSquares) {
			return totalSumSquares == 0 ? 1 : regressionSumSquares / totalSumSquares;
		}
		#endregion
		#region Internal
		#region Check
		static bool CheckFactorsCore(VariantValue value) {
			return !value.IsNumeric && !value.IsArray && !value.IsCellRange;
		}
		static bool HasUnionRange(VariantValue value) {
			return value.IsCellRange && value.CellRangeValue.RangeType == CellRangeType.UnionRange;
		}
		#region CheckRequiredLinestFactors
		static VariantValue CheckRequiredLinestFactors(VariantValue value) {
			if (value.IsError)
				return value;
			if (CheckFactorsCore(value))
				return VariantValue.ErrorInvalidValueInFunction;
			if (HasUnionRange(value))
				return VariantValue.ErrorReference;
			return VariantValue.Empty;
		}
		#endregion
		#region CheckOptionalLinestFactors
		static VariantValue CheckOptionalLinestFactors(VariantValue value) {
			if (value.IsEmpty)
				return VariantValue.Empty;
			return CheckRequiredLinestFactors(value);
		}
		#endregion
		#endregion
		#region CalculateLinearCoeffsB
		delegate VariantValue TryGetResultObservations(IVariantArray arrayValue, double[] observations);
		static VariantValue CalculateLinearCoeffsB(IVariantArray knownY, IVariantArray knownX, bool hasIntercept, bool hasAdditionalStatistics, TryGetResultObservations TryGetResultObservations) {
			long observationsCount = GetResultObservationsCount(knownY);
			ObservationsDirection direction = GetObservationsDirection(knownY);
			return CalculateLinearCoeffsBCore(knownY, knownX, direction, observationsCount, hasIntercept, hasAdditionalStatistics, TryGetResultObservations);
		}
		static VariantValue CalculateLinearCoeffsBCore(IVariantArray knownY, IVariantArray knownX, ObservationsDirection direction, long resultOnservationsCount, bool hasIntercept, bool hasAdditionalStatistics, TryGetResultObservations TryGetResultObservations) {
			double[] resultObservations = new double[resultOnservationsCount];
			VariantValue errorResultObservations = TryGetResultObservations(knownY, resultObservations);
			if (errorResultObservations.IsError)
				return errorResultObservations;
			if (knownX == null)
				return GetLinestCore(RegressionExtendedFactorMatrixInfo.Create(resultObservations), hasIntercept, hasAdditionalStatistics);
			List<RegressionFactorInfo> factors = new List<RegressionFactorInfo>();
			VariantValue errorFactors = PrepareFactors(knownX, factors, direction, resultOnservationsCount);
			if (errorFactors.IsError)
				return errorFactors;
			return GetLinestCore(RegressionExtendedFactorMatrixInfo.Create(resultObservations, factors), hasIntercept, hasAdditionalStatistics);
		}
		#endregion
		static VariantValue GetLinestCore(RegressionExtendedFactorMatrixInfo info, bool hasIntercept, bool hasAdditionalRegression) {
			return info.GetResult(hasIntercept, hasAdditionalRegression);
		}
		static ObservationsDirection GetObservationsDirection(IVariantArray value) {
			if (value.Width > 1 && value.Height == 1)
				return ObservationsDirection.HorizontalVector;
			if (value.Height > 1 && value.Width == 1)
				return ObservationsDirection.VerticalVector;
			return ObservationsDirection.ZVector;
		}
		static long GetFactorsCount(IVariantArray value, ObservationsDirection direction) {
			if (direction == ObservationsDirection.HorizontalVector)
				return value.Height;
			if (direction == ObservationsDirection.VerticalVector)
				return value.Width;
			return 1;
		}
		static long GetResultObservationsCount(IVariantArray value) {
			if (value.Width > 1 && value.Height == 1)
				return value.Width;
			if (value.Height > 1 && value.Width == 1)
				return value.Height;
			return value.Count;
		}
		static long GetObservationsCount(IVariantArray value, ObservationsDirection direction) {
			if (direction == ObservationsDirection.HorizontalVector)
				return value.Width;
			if (direction == ObservationsDirection.VerticalVector)
				return value.Height;
			return value.Count;
		}
		static VariantValue GetObservation(IVariantArray value, int factorIndex, int observationIndex, ObservationsDirection direction) {
			if (direction == ObservationsDirection.HorizontalVector)
				return value.GetValue(factorIndex, observationIndex);
			if (direction == ObservationsDirection.VerticalVector)
				return value.GetValue(observationIndex, factorIndex);
			return value[observationIndex];
		}
		static void ReplaceCoeffsBByExp(IVariantArray arrayValue) {
			VariantArray array = arrayValue as VariantArray;
			int width = array.Width;
			for (int i = 0; i < width; i++)
				array.SetValue(0, i, Math.Exp(array.GetValue(0, i).NumericValue));
		}
		#region GetTrendCore
		static VariantValue GetTrendCore(IVariantArray knownY, IVariantArray knownX, IVariantArray newX, bool hasIntercept) {
			long observationsCount = GetResultObservationsCount(knownY);
			ObservationsDirection direction = GetObservationsDirection(knownY);
			VariantValue coeffsB = CalculateLinearCoeffsBCore(knownY, knownX, direction, observationsCount, hasIntercept, false, TryGetLinearResultObservations);
			if (coeffsB.IsError)
				return coeffsB;
			return GetTrendValues(coeffsB.ArrayValue, direction, knownY, knownX, newX, hasIntercept, GetLinearTrendOperation);
		}
		static VariantValue GetGrowthCore(IVariantArray knownY, IVariantArray knownX, IVariantArray newX, bool hasIntercept) {
			ObservationsDirection direction = GetObservationsDirection(knownY);
			VariantValue coeffsB = GetLogestCore(knownY, knownX, hasIntercept, false);
			if (coeffsB.IsError)
				return coeffsB;
			return GetTrendValues(coeffsB.ArrayValue, direction, knownY, knownX, newX, hasIntercept, GetGrowthTrendOperation);
		}
		#region GetTrendOperation
		delegate double GetTrendOperation(double result, double coeff, double value);
		static double GetLinearTrendOperation(double result, double coeff, double value) {
			result += coeff * value;
			return result;
		}
		static double GetGrowthTrendOperation(double result, double coeff, double value) {
			result *= Math.Pow(coeff, value);
			return result;
		}
		#endregion
		static VariantValue GetTrendValues(IVariantArray coeffsB, ObservationsDirection direction, IVariantArray knownY, IVariantArray knownX, IVariantArray newX, bool hasIntercept, GetTrendOperation GetTrendOperation) {
			IVariantArray coeffsBArray = coeffsB;
			int valuesCount = (int)coeffsBArray.Count - 1;
			if (newX == null) 
				newX = knownX == null ? PrepareDefaultVector(knownY, direction) : knownX;
			if (valuesCount > 1) {
				int factorsCount = (int)GetFactorsCount(newX, direction);
				if (factorsCount != valuesCount)
					return VariantValue.ErrorReference;
			}
			int width;
			int height;
			if (valuesCount == 1) {
				width = newX.Width;
				height = newX.Height;
			} else {
				int observationsCount = (int)GetObservationsCount(newX, direction);
				width = direction == ObservationsDirection.HorizontalVector ? observationsCount : 1;
				height = direction == ObservationsDirection.HorizontalVector ? 1 : observationsCount;
			}
			VariantArray resultArray = VariantArray.Create(width, height);
			for (int row = 0; row < height; row++)
				for (int column = 0; column < width; column++)
					if (!SetTrendValue(resultArray, row, column, coeffsBArray, newX, valuesCount, direction, GetTrendOperation))
						return VariantValue.ErrorGettingData;
			return VariantValue.FromArray(resultArray);
		}
		static bool SetTrendValue(VariantArray resultArray, int row, int column, IVariantArray coeffsB, IVariantArray newX, int valuesCount, ObservationsDirection direction, GetTrendOperation GetTrendOperation) {
			double sum = coeffsB[valuesCount].NumericValue;
			for (int valueIndex = 0; valueIndex < valuesCount; valueIndex++) {
				int index = valuesCount - valueIndex - 1;
				VariantValue value = GetNewXValue(newX, valueIndex, row, column, direction);
				if (value == VariantValue.ErrorGettingData)
					return false;
				sum = GetTrendOperation(sum, coeffsB[index].NumericValue, value.NumericValue);
			}
			resultArray.SetValue(row, column, sum);
			return true;
		}
		static VariantValue GetNewXValue(IVariantArray newX, int index, int row, int column, ObservationsDirection direction) {
			if (direction == ObservationsDirection.HorizontalVector)
				return newX.GetValue(row + index, column);
			if (direction == ObservationsDirection.VerticalVector)
				return newX.GetValue(row, column + index);
			return newX.GetValue(row, column);
		}
		#endregion
		static VariantArray PrepareDefaultVector(IVariantArray array, ObservationsDirection direction) {
			VariantArray result = VariantArray.Create(array.Width, array.Height);
			long count = result.Count;
			for (int i = 0; i < count; i++)
				result.Values[i] = i + 1;
			return result;
		}
		#endregion
	}
	#endregion
}
