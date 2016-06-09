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
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	abstract class AggExecutor : ExecutorBase<AggregateBase> {
		const int defaultBufferCapacity = 100;
		readonly AggCalculatorBase calculator;
		public AggExecutor(AggregateBase operation, IExecutorQueueContext context)
			: base(operation, context) {
			this.calculator = NewCalculator(operation.ValuesFlow.OperationType, operation.SummaryType);
			DataVectorBase valuesVector = context.GetSingleFlowExecutorResultVector(operation.ValuesFlow);
			InitializeCalculator(calculator, operation, valuesVector);
			this.Result = new SingleFlowExecutorResult(calculator.ResultVector);
		}
		internal abstract void InitializeCalculator(AggCalculatorBase calculator, AggregateBase operation, DataVectorBase valuesVector);
		internal AggCalculatorBase NewCalculator(Type dataType, SummaryType summaryType) {
			switch (summaryType) {
				case SummaryType.Sum: return CreateSumCalculator(dataType);
				case SummaryType.Min: return GenericActivator.New<AggCalculatorBase>(typeof(MinCalc<>), dataType);
				case SummaryType.Max: return GenericActivator.New<AggCalculatorBase>(typeof(MaxCalc<>), dataType);
				case SummaryType.Average: return CreateAverageCalculator(dataType);
				case SummaryType.Count: return GenericActivator.New<AggCalculatorBase>(typeof(CountCalc<>), dataType);
				case SummaryType.CountDistinct: return GenericActivator.New<AggCalculatorBase>(typeof(CountDistinctCalc<>), dataType);
				case SummaryType.StdDev: return GenericActivator.New<AggCalculatorBase>(typeof(StdDevCalc<>), dataType);
				case SummaryType.StdDevp: return GenericActivator.New<AggCalculatorBase>(typeof(StdDevpCalc<>), dataType);
				case SummaryType.Var: return GenericActivator.New<AggCalculatorBase>(typeof(VarCalc<>), dataType);
				case SummaryType.Varp: return GenericActivator.New<AggCalculatorBase>(typeof(VarpCalc<>), dataType);
				default:
					throw new NotSupportedException();
			}
		}
		AggCalculatorBase CreateSumCalculator(Type operationType) {
			AggCalculatorBase calculator = null;
			if (operationType == typeof(UInt16) || operationType == typeof(UInt32) || operationType == typeof(UInt64))
				calculator = GenericActivator.New<AggCalculatorBase>(typeof(UInt64SumCalc<>), operationType);
			else if (operationType == typeof(float))
				calculator = GenericActivator.New<AggCalculatorBase>(typeof(FloatSumCalc<>), operationType);
			else if (operationType == typeof(double))
				calculator = GenericActivator.New<AggCalculatorBase>(typeof(DoubleSumCalc<>), operationType);
			else if (operationType == typeof(decimal))
				calculator = GenericActivator.New<AggCalculatorBase>(typeof(DecimalSumCalc<>), operationType);
			else
				calculator = GenericActivator.New<AggCalculatorBase>(typeof(Int64SumCalc<>), operationType);
			return calculator;
		}
		AggCalculatorBase CreateAverageCalculator(Type operationType) {
			AggCalculatorBase calculator = null;
			if (operationType == typeof(float))
				calculator = GenericActivator.New<AggCalculatorBase>(typeof(FloatAverageCalc<>), operationType);
			else if (operationType == typeof(decimal))
				calculator = GenericActivator.New<AggCalculatorBase>(typeof(DecimalAverageCalc<>), operationType);
			else
				calculator = GenericActivator.New<AggCalculatorBase>(typeof(DoubleAverageCalc<>), operationType);
			return calculator;
		}
		protected override ExecutorProcessResult Process() {
			calculator.Process();
			return ExecutorProcessResult.ResultReady;
		}
		public override void FinalizeExecutor() {
			calculator.FinalizeCalculator();
		}
	}
	class GrandTotalAggregateExecutor : AggExecutor {
		public GrandTotalAggregateExecutor(GrandTotalAggregate operation, IExecutorQueueContext context) : base(operation, context) { }
		internal override void InitializeCalculator(AggCalculatorBase calculator, AggregateBase operation, DataVectorBase valuesVector) {
			calculator.InitializeCalculator(1, null, valuesVector);
		}
	}
	class GroupAggregateExecutor : AggExecutor {
		public GroupAggregateExecutor(GroupAggregate operation, IExecutorQueueContext context) : base(operation, context) { }
		internal override void InitializeCalculator(AggCalculatorBase calculator, AggregateBase operation, DataVectorBase valuesVector) {
			SingleFlowExecutorResult indexesResult = null;
			GroupAggregate groupAgg = operation as GroupAggregate;
			if (groupAgg != null)
				indexesResult = (SingleFlowExecutorResult)Context.GetExecutorResult(groupAgg.Indexes);
			DataVector<int> indexVector = (DataVector<int>)indexesResult.ResultVector;
			calculator.InitializeCalculator(0, indexVector, valuesVector);
		}
	}
}
