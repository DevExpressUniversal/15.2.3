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
using DevExpress.Utils;
using DevExpress.DashboardCommon.Native;
using System.IO;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	static class QueryPlanAnalyzer {
		public static IEnumerable<CustomOperation> TopologicalSort(IEnumerable<CustomOperation> operations) {
			return Algorithms.TopologicalSort<CustomOperation>(operations.ToList(), new CustomOperationTopologicalComparer(operations));
		}
		public static IEnumerable<IQueueNode> TopologicalSort(IEnumerable<IQueueNode> queues) {
			return Algorithms.TopologicalSort<IQueueNode>(queues.ToList(), new QueueNodeTopologicalComparer());
		}
		public static IEnumerable<IQueueNode> Analyze(IEnumerable<CustomOperation> queryResultOperations) {
			IList<BlockOperation> blockOperations = GetAllQueryNodes(queryResultOperations).OfType<BlockOperation>().ToList();
			Dictionary<BlockOperation, QueueNode> queueNodes = InitializeQueueNodes(blockOperations);
			List<QueueNode> finishQueues = MergeNodes(queueNodes);
			foreach (var node in finishQueues)
				node.QueueSort();
			return finishQueues;
		}
		static List<QueueNode> MergeNodes(Dictionary<BlockOperation, QueueNode> queueNodes) {
			RankCalculator rankCalculator = new DataProcessing.InMemoryDataProcessor.RankCalculator();
			Dictionary<QueueNode, int> ranks = rankCalculator.Calculate(queueNodes.Values.ToList());
			List<QueueNode> result = new List<QueueNode>();
			int maxRank = ranks.Values.Max();
			for (int rank = 0; rank <= maxRank; rank++) {
				List<QueueNode> sameRankNodes = ranks.Where(pair => pair.Value == rank).Select(pair => pair.Key).ToList();
				List<QueueNode> notMergedNodes = new List<QueueNode>();
				while (sameRankNodes.Count > 0) {
					QueueNode firstNode = sameRankNodes[0];
					for (int i = 1; i < sameRankNodes.Count; i++) {
						QueueNode tryMergeNode = sameRankNodes[i];
						var sameOperations = firstNode.QueueOperations.Intersect(tryMergeNode.QueueOperations).ToList();
						if (NeedMergeNodes(sameOperations))
							firstNode.MergeWith(tryMergeNode);
						else
							notMergedNodes.Add(tryMergeNode);
					}
					sameRankNodes = notMergedNodes.ToList();
					notMergedNodes.Clear();
					result.Add(firstNode);
				}
			}
			return result;
		}
		static bool NeedMergeNodes(List<CustomOperation> sameOperations) {
			return sameOperations.Count > 0;
		}
		static Dictionary<BlockOperation, QueueNode> InitializeQueueNodes(IList<BlockOperation> blockOperations) {
			Dictionary<BlockOperation, QueueNode> queueNodes = blockOperations.ToDictionary(o => o, o => new QueueNode());
			foreach (var pair in queueNodes) {
				BlockOperation block = pair.Key;
				QueueNode queue = pair.Value;
				queue.AddQueueOperation(block);
				BreadthWalk(block, operation => {
					BlockOperation blockOperation = operation as BlockOperation;
					if(blockOperation != null) {
						queue.RegisterPreviousNode(queueNodes[blockOperation]);
						return false;
					} else {
						queue.AddQueueOperation(operation);
						return true;
					}
				});
			}
			return queueNodes;
		}
		static void BreadthWalk(CustomOperation root, Func<CustomOperation, bool> walkFunc) {
			HashSet<CustomOperation> inQueueSet = new HashSet<CustomOperation>();
			Queue<CustomOperation> visitQueue = new Queue<CustomOperation>();
			visitQueue.Enqueue(root);
			while (visitQueue.Count > 0) {
				CustomOperation curr = visitQueue.Dequeue();
				if (curr == root || walkFunc(curr))
					foreach (CustomOperation operand in curr.Operands)
						if (!inQueueSet.Contains(operand)) {
							visitQueue.Enqueue(operand);
							inQueueSet.Add(curr);
						}
			}
		}
		public static IEnumerable<CustomOperation> GetAllQueryNodes(IEnumerable<CustomOperation> queryResultOperations) {
			HashSet<CustomOperation> result = new HashSet<CustomOperation>(queryResultOperations);
			foreach (CustomOperation operation in queryResultOperations)
				BreadthWalk(operation, (op) => result.Add(op));
			return result;
		}
	}
	class RankCalculator {
		class RankedQueueNode {
			int counter = 0;
			int incoming = 0;
			int rank = 0;
			QueueNode wrapped;
			public QueueNode Wrapped { get { return wrapped; } }
			public bool Ready { get { return counter == incoming; } }
			public int Rank { get { return rank; } }
			public RankedQueueNode(QueueNode x) {
				this.wrapped = x;
			}
			public void SetIncoming(int count) {
				incoming = count;
			}
			public void Increment() {
				counter++;
			}
			public void SetRank(int rank) {
				if (rank > this.rank)
					this.rank = rank;
			}
			public override bool Equals(object obj) {
				RankedQueueNode other = (RankedQueueNode)obj;
				return other.wrapped.Equals(wrapped);
			}
			public override int GetHashCode() {
				return wrapped.GetHashCode();
			}
			public override string ToString() {
				return "wrap: " + wrapped.ToString();
			}
		}
		public Dictionary<QueueNode, int> Calculate(IList<QueueNode> nodes) {
			DirectedGraph<RankedQueueNode> graph = PlanOptimizerHelper.BuildWrapperGraph<RankedQueueNode, QueueNode>(nodes, x => x.PreviousNodes.Cast<QueueNode>(), x => new RankedQueueNode(x), true);
			foreach (RankedQueueNode node in graph.Nodes)
				node.SetIncoming(graph.Incoming(node).Count);
			Queue<RankedQueueNode> queue = new Queue<RankedQueueNode>();
			graph.FindStartNodes().ForEach(x => queue.Enqueue(x));
			while (queue.Count > 0) {
				RankedQueueNode node = queue.Dequeue();
				foreach (RankedQueueNode child in graph.Connected(node)) {
					child.SetRank(node.Rank + 1);
					child.Increment();
					if (child.Ready)
						queue.Enqueue(child);
				}
			}
			Dictionary<QueueNode, int> dictionary = new Dictionary<QueueNode, int>();
			foreach (RankedQueueNode node in graph.Nodes)
				dictionary.Add(node.Wrapped, node.Rank);
			return dictionary;
		}
	}
}
