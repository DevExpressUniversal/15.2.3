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

using DevExpress.DashboardCommon.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	public class OthersAggregationException : ArgumentException {
		public OthersAggregationException() : base("Input vector has 'Others' special data") { }
	}
	abstract class AggCalculatorBase {
		public abstract DataVectorBase ResultVector { get; }
		public abstract void Process();
		public abstract void InitializeCalculator(int groupCount, DataVector<int> groupIndexes, DataVectorBase values);
		public abstract void FinalizeCalculator();
	}
	class AggCalculator<TContainer, TCompiler, TInput, TOutput> : AggCalculatorBase
		where TContainer : AggDataContainer<TInput, TOutput>, new()
		where TCompiler : AggWorkerCompiler<TContainer, TInput, TOutput>, new() {
		class PrecompiledAggCacheKey : IEquatable<PrecompiledAggCacheKey> {
			Type dataType;
			AggCalculatorWorkerType workerType;
			bool stopProcessingOnError;
			public PrecompiledAggCacheKey(Type dataType, AggCalculatorWorkerType workerType, bool stopProcessingOnError) {
				this.dataType = dataType;
				this.workerType = workerType;
				this.stopProcessingOnError = stopProcessingOnError;
			}
			public bool Equals(PrecompiledAggCacheKey other) {
				return dataType == other.dataType && 
					   workerType == other.workerType && 
					   stopProcessingOnError == other.stopProcessingOnError;
			}
			public override int GetHashCode() {
				return HashcodeHelper.GetCompositeHashCode<object>(dataType, workerType, stopProcessingOnError);
			}			
		}
		static LRUCache<PrecompiledAggCacheKey, Action<TContainer>> precompiledCache = new LRUCache<PrecompiledAggCacheKey, Action<TContainer>>(DataProcessingOptions.AggregateWorkerCacheSize);
		TContainer container;
		Action<TContainer> worker;
		Action<TContainer> finalizeWorker;
		enum AggCalculatorWorkerType { Worker, Finalizer }
		public override DataVectorBase ResultVector { get { return container.ResultVector; } }
		public override void InitializeCalculator(int groupCount, DataVector<int> groupIndexes, DataVectorBase values) {
			container = new TContainer();
			container.InitializeContainer(groupCount, groupIndexes, values);
			PrecompiledAggCacheKey keyWorker = new PrecompiledAggCacheKey(typeof(TContainer), AggCalculatorWorkerType.Worker, DataProcessingOptions.StopProcessingOnError);
			PrecompiledAggCacheKey keyFinalize = new PrecompiledAggCacheKey(typeof(TContainer), AggCalculatorWorkerType.Finalizer, DataProcessingOptions.StopProcessingOnError);
			worker = precompiledCache.GetOrAdd(keyWorker, () => new TCompiler().CompileWorker());
			finalizeWorker = precompiledCache.GetOrAdd(keyFinalize, () => new TCompiler().CompileFinalizer());
		}
		public override void Process() {
			if (container.GroupIndexes != null) {
				int groupCount = GetGroupCount();
				if (groupCount > container.GroupCount)
					container.Extend(groupCount);
			}
			worker(container);
		}
		int GetGroupCount() {
			if (container.GroupIndexes.Count == 0)
				return 0;
			int max = 0;
			for (int i = 0; i < container.GroupIndexes.Count; i++)
				if (max < container.GroupIndexes.Data[i])
					max = container.GroupIndexes.Data[i];
			return max + 1;
		}
		public override void FinalizeCalculator() {
			if (finalizeWorker != null)
				finalizeWorker(container);
		}
	}
	class MinCalc<T> : AggCalculator<MinMaxAggDataCointainer<T>, MinAggWorkerCompilerBase<T>, T, T> { }
	class MaxCalc<T> : AggCalculator<MinMaxAggDataCointainer<T>, MaxAggWorkerCompilerBase<T>, T, T> { }
	class CountCalc<T> : AggCalculator<AggDataContainer<T, int>, CountAggWorkerCompiler<T>, T, int> { }
	class CountDistinctCalc<T> : AggCalculator<CountDAggDataCointainer<T>, CountDAggWorkerCompiler<T>, T, int> { }
	class AverageCalc<TInput, TOutput> : AggCalculator<AverageAggDataCointainer<TInput, TOutput>, AverageAggWorkerCompiler<TInput, TOutput>, TInput, TOutput> { }
	class FloatAverageCalc<T> : AverageCalc<T, float> { }
	class DoubleAverageCalc<T> : AverageCalc<T, double> { }
	class DecimalAverageCalc<T> : AverageCalc<T, decimal> { }
	class StdDevCalc<T> : AggCalculator<StdDevCointainer<T>, StdDevAggWorkerCompiler<T>, T, double> { }
	class StdDevpCalc<T> : AggCalculator<StdDevpCointainer<T>, StdDevpAggWorkerCompiler<T>, T, double> { }
	class VarCalc<T> : AggCalculator<VarCointainer<T>, VarAggWorkerCompiler<T>, T, double> { }
	class VarpCalc<T> : AggCalculator<VarpCointainer<T>, VarpAggWorkerCompiler<T>, T, double> { }
	class SumCalc<TInput, TOutput> : AggCalculator<AggDataContainer<TInput, TOutput>, SumAggWorkerCompiler<TInput, TOutput>, TInput, TOutput> { }
	class UInt64SumCalc<TInput> : SumCalc<TInput, UInt64> { }
	class FloatSumCalc<TInput> : SumCalc<TInput, float> { }
	class DoubleSumCalc<TInput> : SumCalc<TInput, double> { }
	class DecimalSumCalc<TInput> : SumCalc<TInput, decimal> { }
	class Int64SumCalc<TInput> : SumCalc<TInput, Int64> { }
}
