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
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	class ProjectExecutor : ExecutorBase<Project> {
		ProjectCalculatorBase calculator;
		public ProjectExecutor(Project operation, IExecutorQueueContext context)
			: base(operation, context) {
			DataVectorBase resultVector = DataVectorBase.New(operation.OperationType, context.VectorSize);
			Result = new SingleFlowExecutorResult(resultVector);
			DataVectorBase[] inputVectors = operation.Arguments.Select(o => ((SingleFlowExecutorResult)context.GetExecutorResult(o)).ResultVector).ToArray();
			this.calculator = ProjectCalculatorBase.New(operation, inputVectors, resultVector);
		}
		protected override ExecutorProcessResult Process() {
			calculator.Process();
			return ExecutorProcessResult.ResultReady;
		}
	}
	abstract class ProjectCalculatorBase {
		public static ProjectCalculatorBase New(Project operation, DataVectorBase[] inputVectors, DataVectorBase resultVector) {
			Type resultType = operation.OperationType;
			LambdaExpression workerExpression = ProjectExpressionConstructor.GetWorkerExpression(
				CriteriaOperator.Parse(operation.ExpressionString),
				operation.Operands.OfType<SingleFlowOperation>().Select(o => o.OperationType));
			Delegate compiledWorker = ProjectExpressionConstructor.GetCompiledExpression(operation, workerExpression);
			if(resultType.IsValueType()) {
				if(Nullable.GetUnderlyingType(workerExpression.ReturnType) == null) 
					return GenericActivator.New<ProjectCalculatorBase>(typeof(ProjectCalculatorNonNullable<>), resultType, inputVectors, resultVector, compiledWorker);
				else
					return GenericActivator.New<ProjectCalculatorBase>(typeof(ProjectCalculatorNullable<>), resultType, inputVectors, resultVector, compiledWorker);
			} else
				return GenericActivator.New<ProjectCalculatorBase>(typeof(ProjectCalculatorClass<>), resultType, inputVectors, resultVector, compiledWorker);
		}
		public abstract void Process();
	}
	abstract class ProjectCalculatorBase<T> : ProjectCalculatorBase {
		protected DataVector<T> ResultVector { get; set; }
		protected DataVectorBase[] InputVectors { get; set; }
		public ProjectCalculatorBase(DataVectorBase[] inputVectors, DataVectorBase resultVector) {
			this.InputVectors = inputVectors;
			this.ResultVector = (DataVector<T>)resultVector;
		}
		public override void Process() {
			int count = InputVectors[0].Count;
			ProjectDataVectorContainer container = new ProjectDataVectorContainer(0, InputVectors);
			for(int i = 0; i < count; i++) {
				try {
					container.Index = i;
					ProcessLine(container);
				} catch(Exception e) {
					if (DataProcessingOptions.StopProcessingOnError)
						throw e;
					else
						ResultVector.SpecialData[i] = SpecialDataValue.Error;
				}
			}
			ResultVector.Count = count;
		}
		protected abstract void ProcessLine(ProjectDataVectorContainer container);
	}
	class ProjectCalculatorNullable<T> : ProjectCalculatorBase<T> where T : struct {
		Func<ProjectDataVectorContainer, T?> worker;
		public ProjectCalculatorNullable(DataVectorBase[] inputVectors, DataVectorBase resultVector, Delegate worker)
			: base(inputVectors, resultVector) {
			this.worker = (Func<ProjectDataVectorContainer, T?>)worker;
		}
		protected override void ProcessLine(ProjectDataVectorContainer container) {
			T? result = worker(container);
			if(result.HasValue)
				ResultVector.Data[container.Index] = result.Value;
			else
				ResultVector.SpecialData[container.Index] = SpecialDataValue.Null;
		}
	}
	class ProjectCalculatorNonNullable<T> : ProjectCalculatorBase<T> {
		Func<ProjectDataVectorContainer, T> worker;
		public ProjectCalculatorNonNullable(DataVectorBase[] inputVectors, DataVectorBase resultVector, Delegate worker)
			: base(inputVectors, resultVector) {
			this.worker = (Func<ProjectDataVectorContainer, T>)worker;
		}
		protected override void ProcessLine(ProjectDataVectorContainer container) {
			ResultVector.Data[container.Index] = worker(container);
		}
	}
	class ProjectCalculatorClass<T> : ProjectCalculatorBase<T> where T : class {
		Func<ProjectDataVectorContainer, T> worker;
		public ProjectCalculatorClass(DataVectorBase[] inputVectors, DataVectorBase resultVector, Delegate worker)
			: base(inputVectors, resultVector) {
			this.worker = (Func<ProjectDataVectorContainer, T>)worker;
		}
		protected override void ProcessLine(ProjectDataVectorContainer container) {
			T result = worker(container);
			if(result == null)
				ResultVector.SpecialData[container.Index] = SpecialDataValue.Null;
			else
				ResultVector.Data[container.Index] = result;
		}
	}
}
