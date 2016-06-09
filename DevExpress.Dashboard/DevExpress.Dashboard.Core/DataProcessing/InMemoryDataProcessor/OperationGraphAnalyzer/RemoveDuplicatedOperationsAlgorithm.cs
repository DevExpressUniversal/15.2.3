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
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public class RemoveDuplicatedOperationsAlgorithm : IOptimazerAlgorithm {
		class OperationNodeWithCounter : OperationNode {
			int counter = 0;
			int incoming = 0;
			public bool Ready { get { return counter == incoming; } }
			public OperationNodeWithCounter(CustomOperation x) : base(x) { }
			public void SetIncoming(int count) {
				incoming = count;
			}
			public void Increment() {
				counter++;
			}
		}
		List<CustomOperation> roots;
		public RemoveDuplicatedOperationsAlgorithm(List<CustomOperation> roots) {
			this.roots = roots;
		}
		void Merge(DirectedGraph<OperationNodeWithCounter> graph, List<List<OperationNodeWithCounter>> forMerge) {
			foreach (List<OperationNodeWithCounter> nodesForMerge in forMerge) {
				OperationNodeWithCounter node = nodesForMerge[0];
				for (int i = 1; i < nodesForMerge.Count; i++)
					graph.ForConnected(nodesForMerge[i], x => PlanOptimizerHelper.ReconnectAndReplace(node, nodesForMerge[i], x, graph));
			}
		}
		List<List<OperationNodeWithCounter>> FindNodesForMerge(List<OperationNodeWithCounter> nodes, DirectedGraph<OperationNodeWithCounter> graph) {
			List<List<OperationNodeWithCounter>> forMerge = new List<List<OperationNodeWithCounter>>();
			HashSet<OperationNodeWithCounter> visited = new HashSet<OperationNodeWithCounter>();
			for (int i = 0; i < nodes.Count; i++) {
				List<OperationNodeWithCounter> nodesList = new List<OperationNodeWithCounter>() { nodes[i] };
				for (int j = i + 1; j < nodes.Count; j++) {
					OperationNodeWithCounter node = nodes[j];
					if (visited.Contains(node))
						continue;
					bool isEquals = nodes[i].Wrapped.SimpleEquals(node.Wrapped)
						&& graph.HasSameParents(nodes[i], node)
						&& nodes[i].Wrapped.Operands.Zip(node.Wrapped.Operands, (x, y) => x.SimpleEquals(y)).All(v => v);
					if (isEquals) {
						nodesList.Add(node);
						visited.Add(node);
						continue;
					}
				}
				if (nodesList.Count > 1)
					forMerge.Add(nodesList);
			}
			return forMerge;
		}
		public void Optimize() {
			DirectedGraph<OperationNodeWithCounter> graph = PlanOptimizerHelper.BuildWrapperGraph<OperationNodeWithCounter>(roots, x => new OperationNodeWithCounter(x), true);
			foreach (OperationNodeWithCounter n in graph.Nodes)
				n.SetIncoming(graph.Incoming(n).Count);
			Queue<OperationNodeWithCounter> queue = new Queue<OperationNodeWithCounter>();
			List<List<OperationNodeWithCounter>> forMerge = FindNodesForMerge(graph.FindStartNodes(), graph);
			Merge(graph, forMerge);
			graph.FindStartNodes().ForEach(x => queue.Enqueue(x));
			while (queue.Count > 0) {
				OperationNodeWithCounter node = queue.Dequeue();
				forMerge = FindNodesForMerge(graph.Connected(node), graph);
				Merge(graph, forMerge);
				graph.Connected(node).ForEach(x => AddToQueue(x, queue, graph));
			}
		}
		void AddToQueue(OperationNodeWithCounter node, Queue<OperationNodeWithCounter> queue, DirectedGraph<OperationNodeWithCounter> graph) {
			node.Increment();
			if (node.Ready)
				queue.Enqueue(node);
		}
	}
}
