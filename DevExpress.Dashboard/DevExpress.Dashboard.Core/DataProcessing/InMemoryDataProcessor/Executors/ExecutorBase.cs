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
	public enum ExecutorProcessResult { ResultReady, ResultNotReady, EndOfFlow }
	abstract class ExecutorBase {
		bool isFinished = false;
		readonly DataVectorBase[][] cardinalityGroupedVectors;
		public bool IsFinished { get { return isFinished; } }
		public abstract CustomOperation CustomOperation { get; }
		public ExecutorResultBase Result { get; protected set; }
		protected ExecutorBase(CustomOperation operation, IExecutorQueueContext context) {
			this.cardinalityGroupedVectors = operation.GetOperandsCardinalityGroups().Select(group => group.SelectMany(o => GetOperationVectors(operation, o, context)).ToArray()).ToArray();
		}
		IEnumerable<DataVectorBase> GetOperationVectors(CustomOperation thisOperation, CustomOperation operand, IExecutorQueueContext context) {
			ExecutorResultBase resultBase = context.GetExecutorResult(operand);
			SingleFlowExecutorResult singleFlow = resultBase as SingleFlowExecutorResult;
			MultiFlowExecutorResult multiFlow = resultBase as MultiFlowExecutorResult;
			if(thisOperation is GroupAggregate && operand is Group)
				return new[] { ((GroupExecutorResult)resultBase).GroupIndexes };
			if(singleFlow != null)
				return new[] { singleFlow.ResultVector };
			if(multiFlow != null)
				return multiFlow.ResultVectors;
			throw new NotSupportedException();
		}
		public ExecutorProcessResult ProcessVector() {
			AssertInputVectorsDataCount();
			if(isFinished) {
				Result.Clear();
				return ExecutorProcessResult.EndOfFlow;
			}
			Result.ProcessResult = Process();
			isFinished = Result.ProcessResult == ExecutorProcessResult.EndOfFlow;
			return Result.ProcessResult;
		}
		void AssertInputVectorsDataCount() {
			foreach(DataVectorBase[] group in cardinalityGroupedVectors) {
				if(group.Length == 0)
					continue;
				int count = group[0].Count;
				for(int i = 1; i < group.Length; i++)
					if(count != group[i].Count)
						throw new Exception(String.Format("{0}: Input DataVectors are imbalanced", GetType().Name));
			}
		}
		protected abstract ExecutorProcessResult Process();
		public virtual void FinalizeExecutor() { }
	}
	abstract class ExecutorBase<T> : ExecutorBase where T : CustomOperation {
		protected T Operation { get; private set; }
		protected IExecutorQueueContext Context { get; private set; }
		public override CustomOperation CustomOperation { get { return Operation; } }
		protected ExecutorBase(T operation, IExecutorQueueContext context) :base(operation, context) {
			this.Operation = operation;
			this.Context = context;
		}
	}
}
