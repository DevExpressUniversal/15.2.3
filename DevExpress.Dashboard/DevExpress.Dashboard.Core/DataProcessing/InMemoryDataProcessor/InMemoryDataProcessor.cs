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
using System.Threading;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public static class DataProcessor {
		public static IDictionary<SingleBlockOperation, DataVectorBase> ExecuteOperationGraph(IEnumerable<SingleBlockOperation> resultOperations, CancellationToken cancellationToken) {
			return ExecuteOperationGraph(cancellationToken, resultOperations.ToArray());
		}
		public static IDictionary<SingleBlockOperation, DataVectorBase> ExecuteOperationGraph(IEnumerable<SingleBlockOperation> resultOperations) {
			return ExecuteOperationGraph(CancellationToken.None, resultOperations.ToArray());
		}
		public static IDictionary<SingleBlockOperation, DataVectorBase> ExecuteOperationGraph( params SingleBlockOperation[] resultOperations) {
			return ExecuteOperationGraph(CancellationToken.None, resultOperations);
		}
		public static IDictionary<SingleBlockOperation, DataVectorBase> ExecuteOperationGraph(CancellationToken cancellationToken, params SingleBlockOperation[] resultOperations) {
			if (resultOperations.Length == 0)
				return new Dictionary<SingleBlockOperation, DataVectorBase>();
			List<CustomOperation> roots = resultOperations.ToList<CustomOperation>();
			Dictionary<CustomOperation, CustomOperation> oldToNewMapping = PlanOptimizerHelper.Clone(roots);
			List<CustomOperation> optimizedOperations = new List<CustomOperation>(oldToNewMapping.Values);
			QueryOptimizer.Optimize(optimizedOperations);
			OperationGraphViz.SaveInitial(resultOperations);
			OperationGraphViz.SaveOptimized(optimizedOperations);
			OperationGraphViz.SaveQueues(optimizedOperations);
			IEnumerable<IQueueNode> queues = QueryPlanAnalyzer.Analyze(optimizedOperations);
			QueryExecutor executor = new QueryExecutor(queues, resultOperations.Select(o => (SingleBlockOperation)oldToNewMapping[o]).ToArray(), cancellationToken);
			var executorResults = executor.Execute();
			Dictionary<SingleBlockOperation, DataVectorBase> res = new Dictionary<SingleBlockOperation, DataVectorBase>();
			foreach (SingleBlockOperation op in resultOperations)
				if (!res.ContainsKey(op))
					res.Add(op, executorResults[(SingleBlockOperation)oldToNewMapping[op]]);
			return res;
		}
	}
}
