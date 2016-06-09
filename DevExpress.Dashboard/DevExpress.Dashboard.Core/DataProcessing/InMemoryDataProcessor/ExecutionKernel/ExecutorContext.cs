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
using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public interface IExecutorQueueContext {
		int VectorSize { get; }
		ExecutorResultBase GetExecutorResult(CustomOperation operation);
		void AddExecutorResult(CustomOperation operation, ExecutorResultBase result);
	}
	static class IExecutorQueueContextExtensions {
		public static SingleFlowExecutorResult GetSingleFlowExecutorResult(this IExecutorQueueContext context, CustomOperation operation) {
			return (SingleFlowExecutorResult)context.GetExecutorResult(operation);
		}
		public static MultiFlowExecutorResult GetMultiFlowExecutorResult(this IExecutorQueueContext context, CustomOperation operation) {
			return (MultiFlowExecutorResult)context.GetExecutorResult(operation);
		}
		public static DataVectorBase GetSingleFlowExecutorResultVector(this IExecutorQueueContext context, CustomOperation operation) {
			return ((SingleFlowExecutorResult)context.GetExecutorResult(operation)).ResultVector;
		}
		public static DataVectorBase[] GetMultiFlowExecutorResultVectors(this IExecutorQueueContext context, CustomOperation operation) {
			return ((MultiFlowExecutorResult)context.GetExecutorResult(operation)).ResultVectors;
		}
	}
	public class ExecutorContext : IExecutorQueueContext {
		readonly Dictionary<CustomOperation, ExecutorResultBase> operationResults;
		public int VectorSize { get { return DataProcessingOptions.DataVectorSize; } }
		public ExecutorContext(params QueueResultEntry[] context) {
			this.operationResults = context.ToDictionary(e => e.Operation, e => e.Result);
		}
		public ExecutorResultBase GetExecutorResult(CustomOperation operation) {
			return operationResults[operation];
		}
		public void AddExecutorResult(CustomOperation operation, ExecutorResultBase result) {
			operationResults.Add(operation, result);
		}
	}
}
