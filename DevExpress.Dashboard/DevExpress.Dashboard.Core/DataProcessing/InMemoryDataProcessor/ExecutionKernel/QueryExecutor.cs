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

using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public class QueryExecutor {
		readonly IEnumerable<IQueueNode> query;
		readonly SingleBlockOperation[] resultOperations;
		readonly CancellationToken cancellationToken;
		public QueryExecutor(IEnumerable<IQueueNode> query, SingleBlockOperation[] resultOperations) {
			this.query = query;
			this.resultOperations = resultOperations;
		}
		public QueryExecutor(IEnumerable<IQueueNode> query, SingleBlockOperation[] resultOperations, CancellationToken cancellationToken)
			: this(query, resultOperations) {
			this.cancellationToken = cancellationToken;
		}
		public IDictionary<SingleBlockOperation, DataVectorBase> Execute() {
			IEnumerable<QueueResultEntry> results = DataProcessingOptions.UseParallelProcessing ? ExecuteAsync() : ExecuteSync();
			Dictionary<SingleBlockOperation, DataVectorBase> dictionary = new Dictionary<SingleBlockOperation, DataVectorBase>();
			foreach (QueueResultEntry result in results) {
				SingleBlockOperation op = result.Operation as SingleBlockOperation;
				if (op != null && resultOperations.Contains(op))
					dictionary.Add(op, ((SingleFlowExecutorResult)result.Result).ResultVector);
			}
			return dictionary;			
		}
		IEnumerable<QueueResultEntry> ExecuteAsync() {
			var sortedNodes = QueryPlanAnalyzer.TopologicalSort(query).ToList();
			Dictionary<IQueueNode, Task<QueueResultEntry[]>> queueTasks = new Dictionary<IQueueNode, Task<QueueResultEntry[]>>();
			foreach (var node in sortedNodes) {
				IQueueNode nodeCopy = node; 
				Func<QueueResultEntry[], QueueResultEntry[]> action = results => {
					ExecutorQueue q = new ExecutorQueue(nodeCopy.SortedOperations, results, cancellationToken);
					return q.Execute();
				};
				Task<QueueResultEntry[]>[] antecedents = node.PreviousNodes.Select(n => queueTasks[n]).ToArray();
				Task<QueueResultEntry[]> task = antecedents.Length == 0 ?
					task = Task.Factory.StartNew(() => action(new QueueResultEntry[0])) :
					task = Task.Factory.ContinueWhenAll(antecedents, (tasks) => action(tasks.SelectMany(t => t.Result).ToArray()));
				queueTasks.Add(node, task);
			}
			try {
				Task.WaitAll(queueTasks.Values.ToArray());
			} catch (AggregateException ae) {
				throw ae.InnerException;
			}
			return queueTasks.Values.SelectMany(t => t.Result);
		}
		IEnumerable<QueueResultEntry> ExecuteSync() {
			List<QueueResultEntry> results = new List<QueueResultEntry>();
			foreach (IQueueNode queue in query) {
				ExecutorQueue executorQueue = new ExecutorQueue(queue.SortedOperations, results.ToArray(), cancellationToken);
				results.AddRange(executorQueue.Execute());
			}
			return results;
		}
	}
}
