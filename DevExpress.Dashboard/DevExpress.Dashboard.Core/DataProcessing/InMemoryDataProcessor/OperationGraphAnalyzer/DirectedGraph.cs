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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public class DirectedGraph<T> {
		List<T> nodes;
		HashSet<T> removedNodes;
		List<List<int>> edges;
		List<List<int>> reverseEdges;
		Dictionary<T, int> nodesIndexes;
		public int NodesCount { get { return nodes.Count - removedNodes.Count; } }
		public IList<T> Nodes {
			get {
				List<T> list = nodes.FindAll(x => !removedNodes.Contains(x));
				return list.AsReadOnly();
			}
		}
		public DirectedGraph() : this(new List<T>(), new List<List<int>>(), new List<List<int>>()) { }
		DirectedGraph(List<T> nodes, List<List<int>> edges, List<List<int>> reverseEdges) {
			this.nodes = nodes;
			this.edges = edges;
			this.reverseEdges = reverseEdges;
			this.removedNodes = new HashSet<T>();
			this.nodesIndexes = new Dictionary<T, int>();
			for(int i = 0; i < nodes.Count; i++)
				this.nodesIndexes.Add(nodes[i], i);
		}
		void AddEdge(int first, int second) {
			edges[first].Add(second);
			reverseEdges[second].Add(first);
		}
		void RemoveEdge(int first, int second) {
			edges[first].Remove(second);
			reverseEdges[second].Remove(first);
		}
		public void AddNode(T node) {
			if(nodesIndexes.ContainsKey(node))
				return;
			int index = nodes.Count;
			nodes.Add(node);
			nodesIndexes.Add(node, index);
			edges.Add(new List<int>());
			reverseEdges.Add(new List<int>());
		}
		public void RemoveNode(T node) {
			int index = nodesIndexes[node];
			List<int> edgesList = new List<int>();
			edgesList.AddRange(this.edges[index]);
			foreach(int indexS in edgesList)
				RemoveEdge(index, indexS);
			if(!removedNodes.Contains(node))
				removedNodes.Add(node);
		}
		public void AddEdge(T first, T second) {
			if(removedNodes.Contains(first) || removedNodes.Contains(second))
				return;
			int indexF = nodesIndexes[first];
			int indexS = nodesIndexes[second];
			AddEdge(indexF, indexS);
		}
		public void RemoveEdge(T first, T second) {
			int indexF = nodesIndexes[first];
			int indexS = nodesIndexes[second];
			RemoveEdge(indexF, indexS);
		}
		public bool IsConnected(T first, T second) {
			int indexF = nodesIndexes[first];
			int indexS = nodesIndexes[second];
			return edges[indexF].Contains(indexS);
		}
		public bool HasSameParents(T first, T second) {
			int indexF = nodesIndexes[first];
			int indexS = nodesIndexes[second];
			return reverseEdges[indexF].SequenceEqualsAsSet(reverseEdges[indexS]);
		}
		public List<T> Connected(T node) {
			List<T> list = new List<T>();
			int index = nodesIndexes[node];
			foreach(int i in edges[index])
				list.Add(nodes[i]);
			return list;
		}
		public List<T> Incoming(T node) {
			List<T> list = new List<T>();
			int index = nodesIndexes[node];
			foreach(int i in reverseEdges[index])
				list.Add(nodes[i]);
			return list;
		}
		public void ForConnected(T node, Action<T> action) {
			List<T> list = Connected(node);
			foreach(T n in list)
				action(n);
		}
		public List<T> BreadthFirstSearch(Func<T, T, bool> edgeFilter, Func<T, bool> selector, T start) {
			List<T> selected = new List<T>();
			Queue<int> queue = new Queue<int>();
			queue.Enqueue(nodesIndexes[start]);
			while(queue.Count > 0) {
				int index = queue.Dequeue();
				if(selector(nodes[index]))
					selected.Add(nodes[index]);
				foreach(int cIndex in edges[index])
					if(edgeFilter(nodes[index], nodes[cIndex]))
						queue.Enqueue(cIndex);
			}
			return selected;
		}
		public List<T> Select(Func<T, bool> predicate) {
			List<T> list = new List<T>();
			foreach(T node in nodes)
				if(predicate(node))
					list.Add(node);
			return list;
		}
		public List<T> FindTerminalNodes() {
			List<T> list = new List<T>();
			for(int i = 0; i < edges.Count; i++) {
				if(edges[i].Count == 0)
					list.Add(nodes[i]);
			}
			return list;
		}
		public List<T> FindStartNodes() {
			List<T> list = new List<T>();
			for(int i = 0; i < edges.Count; i++) {
				if(reverseEdges[i].Count == 0)
					list.Add(nodes[i]);
			}
			return list;
		}
		public bool Contains(T node) {
			return nodesIndexes.ContainsKey(node);
		}
		public bool HasPath(T start, T goal) {
			return FindPath(start, goal, (x, y) => true) != null;
		}
		public bool HasPath(T start, T goal, Func<T, T, bool> filter) {
			return FindPath(start, goal, filter) != null;
		}
		public List<T> FindPath(T start, T goal) {
			return FindPath(start, goal, (x, y) => true);
		}
		public List<T> FindPath(T start, T goal, Func<T, T, bool> filter) {
			int[] paths = new int[nodes.Count];
			for(int i = 0; i < paths.Length; i++)
				paths[i] = -1;
			Queue<int> queue = new Queue<int>();
			int startIndex = nodesIndexes[start];
			int goalIndex = nodesIndexes[goal];
			queue.Enqueue(startIndex);
			while(queue.Count > 0) {
				int index = queue.Dequeue();
				if(index == goalIndex)
					break;
				List<int> connected = edges[index];
				foreach(int i in connected) {
					if(filter(nodes[index], nodes[i])) {
						paths[i] = index;
						queue.Enqueue(i);
					}
				}
			}
			if(paths[goalIndex] == -1)
				return null;
			List<T> path = new List<T>();
			{
				int index = goalIndex;
				while(startIndex != index) {
					path.Add(nodes[index]);
					index = paths[index];
				}
				path.Add(nodes[startIndex]);
			}
			path.Reverse();
			return path;
		}
		public DirectedGraph<T> GetReversedGraph() {
			return new DirectedGraph<T>(new List<T>(nodes), new List<List<int>>(reverseEdges), new List<List<int>>(edges));
		}
	}
}
