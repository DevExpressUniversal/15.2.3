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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors;
using System.Threading;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public class ExecutorQueue {
		readonly List<CustomOperation> operations;
		readonly IExecutorQueueContext context;
		readonly List<ExecutorQueueRecord> executorsQueue;
		CancellationToken cancellationToken;
		public ExecutorQueue(IEnumerable<CustomOperation> operations, QueueResultEntry[] contextEntries, CancellationToken cancellationToken) {
			if(operations.Count() == 0)
				throw new InvalidOperationException();
			this.context = new ExecutorContext(contextEntries);
			this.operations = operations.ToList();
			this.executorsQueue = new List<ExecutorQueueRecord>();
			this.cancellationToken = cancellationToken;
			IDictionary<CustomOperation, ExecutorQueueRecord> mapping = new Dictionary<CustomOperation, ExecutorQueueRecord>();
			foreach(CustomOperation op in operations) {
				List<ExecutorQueueRecord> dependence = op.Operands.Select(o => mapping.ContainsKey(o) ? mapping[o] : null).NotNull().ToList();
				ExecutorQueueRecord record = new ExecutorQueueRecord(ExecutorFactory.Create(op, context), GetPreviuosRecords(mapping, op));
				mapping.Add(op, record);
				context.AddExecutorResult(op, record.ResultEntry.Result);
				executorsQueue.Add(record);
			}
		}
		public ExecutorQueue(IEnumerable<CustomOperation> operations, QueueResultEntry[] contextEntries) 
			: this(operations, contextEntries, CancellationToken.None) {		 
		}
		public QueueResultEntry[] Execute() {
			bool isEof;
			do {
				isEof = true;
				for (int i = 0; i < operations.Count; i++) {
					if (cancellationToken.IsCancellationRequested)
						break;
					if (executorsQueue[i].Operation.CanReturnEndOfFlow)
						isEof &= executorsQueue[i].Execute();
					else
						executorsQueue[i].Execute();
				}
			} while (!isEof);
			List<QueueResultEntry> result = new List<QueueResultEntry>();
			foreach (ExecutorQueueRecord record in executorsQueue)
				if (record.Operation is BlockOperation) {
					record.FinalizeExecutor();
					result.Add(record.ResultEntry);
				}
			return result.ToArray();
		}
		List<ExecutorQueueRecord> GetPreviuosRecords(IDictionary<CustomOperation, ExecutorQueueRecord> mapping, CustomOperation op) {
			List<ExecutorQueueRecord> result = new List<ExecutorQueueRecord>();
			foreach(var operand in op.Operands) {
				if(operand is Extract || operand is BlockExtract)
					foreach(var parentOperand in operand.Operands)
						result.AddRange(GetPreviuosRecords(mapping, parentOperand));
				if(mapping.ContainsKey(operand))
					result.Add(mapping[operand]);
			}
			return result;
		}
	}
}
