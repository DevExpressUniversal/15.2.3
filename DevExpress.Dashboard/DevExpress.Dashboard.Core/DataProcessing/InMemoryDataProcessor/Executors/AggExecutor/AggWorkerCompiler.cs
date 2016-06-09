#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	abstract class AggWorkerCompiler<TContainer, TInput, TOutput> where TContainer : AggDataContainer<TInput, TOutput> {
		#region static
		static Type[] nativeEqutableTypes = new Type[] { 
			typeof(byte), typeof(sbyte), 
			typeof(Int16), typeof(Int32), typeof(Int64),
			typeof(UInt16), typeof(UInt32), typeof(UInt64),
			typeof(float), typeof(double), typeof(decimal), 
			typeof(Guid), typeof(DateTime), typeof(TimeSpan), 
			typeof(bool), typeof(char), typeof(string)
		};
		static Type[] nativeComparableTypes = new Type[] { 
			typeof(byte), typeof(sbyte), 
			typeof(Int16), typeof(Int32), typeof(Int64),
			typeof(UInt16), typeof(UInt32), typeof(UInt64),
			typeof(float), typeof(double),typeof(decimal), 
			typeof(DateTime),typeof(TimeSpan), typeof(char)
		};
		protected static bool IsNativeEquableType<TTestType>() {
			return nativeEqutableTypes.Contains(typeof(TTestType));
		}
		protected static bool IsNativeComparableType<TTestType>() {
			return nativeComparableTypes.Contains(typeof(TTestType));
		}
		#endregion
		bool initialized = false;
		Expression workerBody;
		ParameterExpression inputVectorCount;
		ParameterExpression inputVectorData;
		ParameterExpression inputVectorSpecialData;
		ParameterExpression resultVectorData;
		ParameterExpression resultVectorSpecialData;
		ParameterExpression groupIndexesData;
		ParameterExpression resultIndex;
		ParameterExpression dataIndex;
		IndexExpression inputVectorDataAccess;
		IndexExpression inputVectorSpecialDataAccess;
		IndexExpression resultVectorDataAccess;
		IndexExpression resultVectorSpecialDataAccess;
		Expression assignResultError;
		LabelTarget switchBreakPoint;
		protected ParameterExpression Container { get; set; }
		public Action<TContainer> CompileWorker() {
			Initialize();
			ConstructWorker();
			return Expression.Lambda<Action<TContainer>>(workerBody, Container).Compile();
		}
		public Action<TContainer> CompileFinalizer() {
			Initialize();
			return GetFinalizerInternal();
		}
		protected virtual Action<TContainer> GetFinalizerInternal() {
			return null;
		}
		protected abstract Expression GetAggregateExpression(IndexExpression inputVectorDataAccess, IndexExpression resultVectorDataAccess, Expression dataIndex, Expression resultIndex);
		protected virtual void Initialize() {
			if (initialized)
				return;
			initialized = true;
			Container = Expression.Parameter(typeof(TContainer), "container");
			inputVectorCount = Expression.Variable(typeof(int), "inputVectorCount");
			inputVectorData = Expression.Variable(typeof(TInput[]), "inputVectorData");
			inputVectorSpecialData = Expression.Variable(typeof(SpecialDataValue[]), "inputVectorSpecialData");
			resultVectorData = Expression.Variable(typeof(TOutput[]), "resultVectorData");
			resultVectorSpecialData = Expression.Variable(typeof(SpecialDataValue[]), "resultVectorSpecialData");
			groupIndexesData = Expression.Variable(typeof(int[]), "groupIndexesData");
			dataIndex = Expression.Variable(typeof(int), "dataIndex");
			resultIndex = Expression.Variable(typeof(int), "resultIndex");
			inputVectorDataAccess = Expression.ArrayAccess(inputVectorData, dataIndex);
			inputVectorSpecialDataAccess = Expression.ArrayAccess(inputVectorSpecialData, dataIndex);
			resultVectorDataAccess = Expression.ArrayAccess(resultVectorData, resultIndex);
			resultVectorSpecialDataAccess = Expression.ArrayAccess(resultVectorSpecialData, resultIndex);
			assignResultError = Expression.Block(typeof(void), Expression.Assign(resultVectorSpecialDataAccess, Expression.Constant(SpecialDataValue.Error)));
			switchBreakPoint = Expression.Label("switchBreakPoint");
		}
		void ConstructWorker() {
			Expression containerInputVector = Expression.Property(Container, "InputVector");
			Expression containerResultVector = Expression.Property(Container, "ResultVector");
			Expression containerGroupIndexes = Expression.Property(Container, "GroupIndexes");
			var caseNone = GetCaseNone();
			var caseError = GetCaseError();
			var caseOthers = GetCaseOthers();
			var dataSwitch = GetSwitch(caseNone, caseError, caseOthers);
			var processLoop = GetLoop(dataIndex, inputVectorCount, GetLoopBody(dataSwitch, dataIndex));
			workerBody = Expression.Block(
				new ParameterExpression[] { 
					inputVectorCount,
					inputVectorData,
					inputVectorSpecialData,
					resultVectorData,
					resultVectorSpecialData,
					groupIndexesData,
					resultIndex,
					dataIndex 
				},
				Assign(inputVectorCount, Expression.Property(containerInputVector, "Count")),
				Assign(inputVectorData, Expression.Property(containerInputVector, "Data")),
				Assign(inputVectorSpecialData, Expression.Property(containerInputVector, "SpecialData")),
				Assign(resultVectorData, Expression.Property(containerResultVector, "Data")),
				Assign(resultVectorSpecialData, Expression.Property(containerResultVector, "SpecialData")),
				Assign(groupIndexesData,
					Expression.Condition(Expression.Equal(containerGroupIndexes, Expression.Constant(null)),
						Expression.Constant(null, typeof(int[])),
						Expression.Property(containerGroupIndexes, "Data"))),
				Assign(dataIndex, Expression.Constant(0)),
				processLoop
				);
		}
		BlockExpression GetLoopBody(SwitchExpression dataSwitch, ParameterExpression index) {
			return Expression.Block(new[] { resultIndex },
									Assign(resultIndex, GetGroupIndex(index)),
									dataSwitch,
									Expression.Label(switchBreakPoint),
									Expression.AddAssign(index, Expression.Constant(1)));
		}
		ConditionalExpression GetGroupIndex(ParameterExpression index) {
			return Expression.Condition(Expression.Equal(groupIndexesData, Expression.Constant(null, typeof(int[]))),
											Expression.Constant(0),
											Expression.ArrayAccess(groupIndexesData, index));
		}
		SwitchExpression GetSwitch(BlockExpression caseNone, BlockExpression caseError, UnaryExpression caseOthers) {
			var dataSwitch = Expression.Switch(inputVectorSpecialDataAccess,
				Expression.SwitchCase(caseNone, Expression.Constant(SpecialDataValue.None)),
				Expression.SwitchCase(caseError, Expression.Constant(SpecialDataValue.Error)),
				Expression.SwitchCase(caseOthers, Expression.Constant(SpecialDataValue.Others)));
			return dataSwitch;
		}
		UnaryExpression GetCaseOthers() {
			var caseOthers = Expression.Throw(Expression.New(typeof(DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors.OthersAggregationException)));
			return caseOthers;
		}
		BlockExpression GetCaseError() {
			var caseError = Expression.Block(assignResultError, Expression.Break(switchBreakPoint));
			return caseError;
		}
		BlockExpression GetCaseNone() {
			if (DataProcessingOptions.StopProcessingOnError) 
				return Expression.Block(GetAggregateExpression(inputVectorDataAccess, resultVectorDataAccess, dataIndex, resultIndex));
			else {
				var caseNone = Expression.Block(
				Expression.TryCatch(
					Expression.Block(typeof(void), GetAggregateExpression(inputVectorDataAccess, resultVectorDataAccess, dataIndex, resultIndex)),
					Expression.Catch(typeof(Exception), assignResultError)),
				Expression.Break(switchBreakPoint));
				return caseNone;			
			}
		}
		LoopExpression GetLoop(ParameterExpression index, ParameterExpression maxIndex, BlockExpression body) {
			var loopBreakProint = Expression.Label("loopBreakProint");
			var loop =
			Expression.Loop(
				Expression.IfThenElse(
					Expression.LessThan(index, maxIndex),
						body,
						Expression.Break(loopBreakProint)
				)
				, loopBreakProint
			);
			return loop;
		}
		BinaryExpression Assign(Expression left, Expression rigth) {
			return Expression.Assign(left, rigth);
		}
	}
	abstract class MinMaxAggWorkerCompilerBase<T> : AggWorkerCompiler<MinMaxAggDataCointainer<T>, T, T> {
		Expression comparer;
		Expression isAssign;
		protected override void Initialize() {
			base.Initialize();
			comparer = Expression.Property(Container, "Comparer");
			isAssign = Expression.Property(Container, "IsAssign");
		}
		protected override Expression GetAggregateExpression(IndexExpression inputVectorDataAccess, IndexExpression resultVectorDataAccess, Expression dataIndex, Expression resultIndex) {
			var trueExpr = Expression.Constant(true);
			var isAssignAccess = Expression.ArrayAccess(isAssign, resultIndex);
			var replace = Expression.Variable(typeof(bool));
			var selectMinMaxBlock = Expression.Block(new[] { replace },
				Expression.Assign(replace, GetCompareExpression(resultVectorDataAccess, inputVectorDataAccess)),
				Expression.IfThen(replace,
					Expression.Assign(resultVectorDataAccess, inputVectorDataAccess)));
			var ifExpression = Expression.IfThenElse(isAssignAccess,
				selectMinMaxBlock,
				Expression.Block(
					Expression.Assign(resultVectorDataAccess, inputVectorDataAccess),
					Expression.Assign(isAssignAccess, Expression.Constant(true)))
				);
			return Expression.Block(ifExpression);
		}
		Expression GetCompareExpression(IndexExpression resultVectorDataAccess, IndexExpression inputVectorDataAccess) {
			if (IsNativeComparableType<T>())
				return MakeCompare(inputVectorDataAccess, resultVectorDataAccess);
			else {
				MethodInfo cmpMethod = typeof(Comparer<T>).GetMethod("Compare", BindingFlags.Instance | BindingFlags.Public);
				Expression cmpResult = Expression.Call(comparer, cmpMethod, new[] { inputVectorDataAccess, resultVectorDataAccess });
				return MakeCompare(cmpResult, Expression.Constant(0));
			}
		}
		protected abstract Expression MakeCompare(Expression left, Expression right);
	}
	class MinAggWorkerCompilerBase<T> : MinMaxAggWorkerCompilerBase<T> {
		protected override Expression MakeCompare(Expression left, Expression right) {
			return Expression.MakeBinary(ExpressionType.LessThan, left, right);
		}
	}
	class MaxAggWorkerCompilerBase<T> : MinMaxAggWorkerCompilerBase<T> {
		protected override Expression MakeCompare(Expression left, Expression right) {
			return Expression.MakeBinary(ExpressionType.GreaterThan, left, right);
		}
	}
	class CountAggWorkerCompiler<T> : AggWorkerCompiler<AggDataContainer<T, int>, T, int> {
		protected override Expression GetAggregateExpression(IndexExpression inputVectorDataAccess, IndexExpression resultVectorDataAccess, Expression dataIndex, Expression resultIndex) {
			return Expression.AddAssign(resultVectorDataAccess, Expression.Constant(1));
		}
	}
	class CountDAggWorkerCompiler<T> : AggWorkerCompiler<CountDAggDataCointainer<T>, T, int> {
		protected override Expression GetAggregateExpression(IndexExpression inputVectorDataAccess, IndexExpression resultVectorDataAccess, Expression dataIndex, Expression resultIndex) {
			MethodInfo addMethod = typeof(CountDAggDataCointainer<T>).GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
			return Expression.IfThen(Expression.Call(Container, addMethod, resultIndex, inputVectorDataAccess),
				Expression.AddAssign(resultVectorDataAccess, Expression.Constant(1)));
		}
	}
	class AverageAggWorkerCompiler<TInput, TOutput> : AggWorkerCompiler<AverageAggDataCointainer<TInput, TOutput>, TInput, TOutput> {
		Expression counts;
		protected override void Initialize() {
			base.Initialize();
			counts = Expression.Property(Container, "Counts");
		}
		protected override Expression GetAggregateExpression(IndexExpression inputVectorDataAccess, IndexExpression resultVectorDataAccess, Expression dataIndex, Expression resultIndex) {
			var countsAccess = Expression.ArrayAccess(counts, resultIndex);
			return Expression.Block(
				Expression.AddAssign(resultVectorDataAccess, Expression.Convert(inputVectorDataAccess, typeof(TOutput))),
				Expression.AddAssign(countsAccess, Expression.Constant(1)));
		}
		protected override Action<AverageAggDataCointainer<TInput, TOutput>> GetFinalizerInternal() {
			var resultVector = Expression.Property(Container, "ResultVector");
			var groupCount = Expression.Property(Container, "GroupCount");
			var resultIndex = Expression.Variable(typeof(int), "resultIndex");
			var assign_resultIndex = Expression.Assign(resultIndex, Expression.Constant(0));
			var countsAccess = Expression.ArrayAccess(counts, resultIndex);
			var resultVectorDataAccess = Expression.ArrayAccess(Expression.Property(resultVector, "Data"), resultIndex);
			var resultVectorSpecialDataAccess = Expression.ArrayAccess(Expression.Property(resultVector, "SpecialData"), resultIndex);
			var loopBreakTarget = Expression.Label(":loopBreakTarget");
			var loopBody = Expression.IfThen(Expression.NotEqual(resultVectorSpecialDataAccess, Expression.Constant(SpecialDataValue.Error)),
				Expression.IfThenElse(Expression.NotEqual(countsAccess, Expression.Constant(0)),
					Expression.DivideAssign(resultVectorDataAccess, Expression.Convert(countsAccess, typeof(TOutput))),
					Expression.Assign(resultVectorSpecialDataAccess, Expression.Constant(SpecialDataValue.Error))));
			var loop = Expression.Loop(Expression.Block(
				Expression.IfThenElse(Expression.LessThan(resultIndex, groupCount),
					Expression.Block(
						loopBody,
						Expression.AddAssign(resultIndex, Expression.Constant(1))),
					Expression.Break(loopBreakTarget))), loopBreakTarget);
			return Expression
				.Lambda<Action<AverageAggDataCointainer<TInput, TOutput>>>(Expression.Block(new[] { resultIndex }, assign_resultIndex, loop), new[] { Container })
				.Compile();
		}
	}
	abstract class StdDevAndVarAggWorkerCompiler<TContainer, T> : AggWorkerCompiler<TContainer, T, double> where TContainer : StdDevAndVarAggDataCointainer<T> {
		Expression sum;
		Expression sqrSum;
		Expression counts;
		protected override void Initialize() {
			base.Initialize();
			sum = Expression.Property(Container, "Sum");
			sqrSum = Expression.Property(Container, "SqrSum");
			counts = Expression.Property(Container, "Counts");
		}
		protected override Expression GetAggregateExpression(IndexExpression inputVectorDataAccess, IndexExpression resultVectorDataAccess, Expression dataIndex, Expression resultIndex) {
			var sumAccess = Expression.ArrayAccess(sum, resultIndex);
			var sqrSumAccess = Expression.ArrayAccess(sqrSum, resultIndex);
			var countsAccess = Expression.ArrayAccess(counts, resultIndex);
			var doubleInputValue = Expression.Variable(typeof(double));
			return Expression.Block(new[] { doubleInputValue },
				Expression.AddAssign(sumAccess, Expression.Convert(inputVectorDataAccess, typeof(decimal))),
				Expression.Assign(doubleInputValue, Expression.Convert(inputVectorDataAccess, typeof(double))),
				Expression.AddAssign(sqrSumAccess, Expression.Multiply(doubleInputValue, doubleInputValue)),
				Expression.AddAssign(countsAccess, Expression.Constant(1)));
		}
		protected override Action<TContainer> GetFinalizerInternal() {
			return FinalizeWorker;
		}
		static void FinalizeWorker(StdDevAndVarAggDataCointainer<T> container) {
			int subK = 0;
			if (container.Type == SummaryType.StdDev || container.Type == SummaryType.Var)
				subK = 1;
			DataVector<double> resultVector = container.ResultVector;
			decimal[] sum = container.Sum;
			double[] sqrSum = container.SqrSum;
			int[] counts = container.Counts;
			for (int i = 0; i < container.GroupCount; i++)
				if (resultVector.SpecialData[i] != SpecialDataValue.Error) {
					if (IsDecimalConversionAllowed(sqrSum[i]))
						resultVector.Data[i] = (double)(((decimal)sqrSum[i] - (sum[i] * sum[i]) / counts[i]) / (counts[i] - subK));
					else
						resultVector.Data[i] = ((sqrSum[i] - ((double)sum[i] * (double)sum[i]) / counts[i]) / (counts[i] - subK));
				}
			if (container.Type == SummaryType.StdDev || container.Type == SummaryType.StdDevp)
				for (int i = 0; i < container.GroupCount; i++)
					if (resultVector.SpecialData[i] != SpecialDataValue.Error)
						resultVector.Data[i] = Math.Sqrt(resultVector.Data[i]);
		}
		static bool IsDecimalConversionAllowed(double value) {
			double decimalValue = (double)((decimal)value);
			return (value <= (double)decimal.MaxValue) &&
				(value >= (double)decimal.MinValue) &&
				((value > 0 && (value - decimalValue) < 0) || (value < 0 && (value - decimalValue) > 0));
		}
	}
	class StdDevAggWorkerCompiler<T> : StdDevAndVarAggWorkerCompiler<StdDevCointainer<T>, T> { }
	class StdDevpAggWorkerCompiler<T> : StdDevAndVarAggWorkerCompiler<StdDevpCointainer<T>, T> { }
	class VarAggWorkerCompiler<T> : StdDevAndVarAggWorkerCompiler<VarCointainer<T>, T> { }
	class VarpAggWorkerCompiler<T> : StdDevAndVarAggWorkerCompiler<VarpCointainer<T>, T> { }
	class SumAggWorkerCompiler<TInput, TOutput> : AggWorkerCompiler<AggDataContainer<TInput, TOutput>, TInput, TOutput> {
		protected override Expression GetAggregateExpression(IndexExpression inputVectorDataAccess, IndexExpression resultVectorDataAccess, Expression dataIndex, Expression resultIndex) {
			return Expression.AddAssign(resultVectorDataAccess, Expression.Convert(inputVectorDataAccess, typeof(TOutput)));
		}
	}
}
