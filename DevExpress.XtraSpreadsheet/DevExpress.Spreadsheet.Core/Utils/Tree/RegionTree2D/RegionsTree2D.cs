#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Utils.Trees {
	public partial class RTree2D<T> : IEnumerable<T> {
		static readonly Random random = new Random();
		readonly int maxEntries;
		readonly int minEntries;
		readonly IRTreeCoordinateProvider<int> coordinateProvider;
		readonly RTreeSeedPickMethod seedPicker;
		Node root;
		volatile int size;
		public RTree2D(int maxEntries, int minEntries, RTreeSeedPickMethod seedPicker) {
			Debug.Assert(minEntries <= (maxEntries / 2));
			this.coordinateProvider = new RTreeIntCoordinateProvider();
			this.maxEntries = maxEntries;
			this.minEntries = minEntries;
			this.seedPicker = seedPicker;
			root = BuildRoot(true);
		}
		public RTree2D(int maxEntries, int minEntries)
			: this(maxEntries, minEntries, RTreeSeedPickMethod.Linear) {
		}
		public RTree2D()
			: this(16, 2, RTreeSeedPickMethod.Linear) {
		}
		public int Count { get { return size; } }
		Node BuildRoot(bool asLeaf) {
			int defaultCoord = coordinateProvider.Sqrt(coordinateProvider.MaxValue);
			int defaultDimension = coordinateProvider.Multiply(defaultCoord, -2);
			return new Node(defaultCoord, defaultCoord, defaultDimension, defaultDimension, asLeaf);
		}
		#region Search
		public List<T> Search(NodeBase pattern) {
			return Search(pattern, true);
		}
		public List<T> Search(NodeBase pattern, bool allowIntersection) {
			List<T> results = new List<T>();
			Search(pattern, root, results, allowIntersection);
			return results;
		}
		public T SearchItemOrNull(NodeBase pattern, bool allowIntersection) {
			return SearchItemOrNull(pattern, root, allowIntersection);
		}
		T SearchItemOrNull(NodeBase pattern, Node node, bool allowIntersection) {
			if (node.IsLeaf) {
				foreach (Node child in node.Children)
					if (IsOverlap(pattern, child, allowIntersection))
						return ((Entry)child).Value;
			}
			else {
				foreach (Node child in node.Children)
					if (IsOverlap(pattern, child, allowIntersection)) {
						T result = SearchItemOrNull(pattern, child, allowIntersection);
						if (result != null)
							return result;
					}
			}
			return default(T);
		}
		void Search(NodeBase pattern, Node node, IList<T> results, bool allowIntersection) {
			if (node.IsLeaf) {
				foreach (Node child in node.Children)
					if (IsOverlap(pattern, child, allowIntersection))
						results.Add(((Entry)child).Value);
			}
			else {
				foreach (Node child in node.Children)
					if (IsOverlap(pattern, child, allowIntersection))
						Search(pattern, child, results, allowIntersection);
			}
		}
		#endregion
		public bool Delete(NodeBase pattern, T value, bool exactMatch) {
			Tuple<Node, int> searchResult = FindLeaf(root, pattern, value, exactMatch);
			if (searchResult == null) {
				Debug.WriteLine("somthing goes wrong!");
				FindLeaf(root, pattern, value, exactMatch);
			}
			Debug.Assert(searchResult != null, "Could not find leaf for entry to delete", "");
			Node leaf = searchResult.Item1;
			Debug.Assert(leaf.IsLeaf, "Entry is not found at leaf", "");
					leaf.Children.RemoveAt(searchResult.Item2);
				CondenseTree(leaf);
				size--;
			if (size == 0)
				root = BuildRoot(true);
			return true;
		}
		Tuple<Node, int> FindLeaf(Node node, NodeBase pattern, T entry, bool exactMatch) {
			if (node.IsLeaf) {
				for (int i = 0; i < node.Children.Count; i++) {
					Entry child = node.Children[i] as Entry;
					if (child != null && child.Value.Equals(entry) && (!exactMatch || child.IsRangeEqual(pattern)))
						return new Tuple<Node, int>(node, i);
				}
				return null;
			}
			else {
				foreach (Node child in node.Children) {
					if (IsOverlap(child, pattern, true)) {
						Tuple<Node, int> result = FindLeaf(child, pattern, entry, exactMatch);
						if (result != null)
							return result;
					}
				}
				return null;
			}
		}
		void CondenseTree(Node n) {
			HashSet<Node> q = new HashSet<Node>();
			while (n != root) {
				if (n.IsLeaf && (n.Children.Count < minEntries)) {
					q.AddRange(n.Children);
					n.Parent.Children.Remove(n);
				}
				else if (!n.IsLeaf && (n.Children.Count < minEntries)) {
					List<Node> toVisit = new List<Node>(n.Children);
					while (toVisit.Count > 0) {
						Node c = toVisit[0];
						toVisit.RemoveAt(0);
						if (c.IsLeaf)
							q.AddRange(c.Children);
						else
							toVisit.AddRange(c.Children);
					}
					n.Parent.Children.Remove(n);
				}
				else
					Tighten(n);
				n = n.Parent;
			}
			if (root.Children.Count == 0)
				root = BuildRoot(true);
			else if ((root.Children.Count == 1) && (!root.IsLeaf)) {
				root = root.Children[0];
				root.Parent = null;
			}
			else
				Tighten(root);
			foreach (Entry e in q)
				Insert(e.ColumnIndex, e.RowIndex, e.Width, e.Height, e.Value);
			size -= q.Count;
		}
		public void Clear() {
			root = BuildRoot(true);
			size = 0;
		}
		public void Insert(int columnIndex, int rowIndex, int width, int height, T value) {
			Entry entry = new Entry(columnIndex, rowIndex, width, height, value);
			Node leaf = ChooseLeaf(root, entry);
			leaf.Children.Add(entry);
			size++;
			entry.Parent = leaf;
			if (leaf.Children.Count > maxEntries) {
				Node[] splits = SplitNode(leaf);
				AdjustTree(splits[0], splits[1]);
			}
			else
				AdjustTree(leaf, null);
		}
		void AdjustTree(Node node, Node newNode) {
			if (node == root) {
				if (newNode != null) {
					root = BuildRoot(false);
					root.Children.Add(node);
					node.Parent = root;
					root.Children.Add(newNode);
					newNode.Parent = root;
				}
				Tighten(root);
				return;
			}
			Tighten(node);
			if (newNode != null) {
				Tighten(newNode);
				if (node.Parent.Children.Count > maxEntries) {
					Node[] splits = SplitNode(node.Parent);
					AdjustTree(splits[0], splits[1]);
				}
			}
			if (node.Parent != null)
				AdjustTree(node.Parent, null);
		}
		Node[] SplitNode(Node node) {
			Node[] nodes = new Node[] { node, new Node(node.ColumnIndex, node.RowIndex, node.Width, node.Height, node.IsLeaf) };
			nodes[1].Parent = node.Parent;
			if (nodes[1].Parent != null) {
				nodes[1].Parent.Children.Add(nodes[1]);
			}
			List<Node> children = new List<Node>(node.Children);
			node.Children.Clear();
			Node[] seeds = seedPicker == RTreeSeedPickMethod.Linear ? LinearPickSeeds(children) : QuadraticPickSeeds(children);
			nodes[0].Children.Add(seeds[0]);
			nodes[1].Children.Add(seeds[1]);
			Tighten(nodes);
			while (children.Count > 0) {
				if ((nodes[0].Children.Count >= minEntries) && (nodes[1].Children.Count + children.Count == minEntries)) {
					nodes[1].Children.AddRange(children);
					children.Clear();
					Tighten(nodes); 
					return nodes;
				}
				else if ((nodes[1].Children.Count >= minEntries) && (nodes[0].Children.Count + children.Count == minEntries)) {
					nodes[0].Children.AddRange(children);
					children.Clear();
					Tighten(nodes); 
					return nodes;
				}
				Node c = seedPicker == RTreeSeedPickMethod.Linear ? LinearPickNext(children) : QuadraticPickNext(children, nodes);
				Node preferredNode = PickPreferredNode(nodes[0], nodes[1], c);
				preferredNode.Children.Add(c);
				Tighten(preferredNode);
			}
			return nodes;
		}
		Node PickPreferredNode(Node nodeA, Node nodeB, Node newNode) {
			float areaA = nodeA.Area;
			float areaB = nodeB.Area;
			float areaDeltaA = CalculateUnionArea(nodeA, newNode) - areaA;
			float areaDeltaB = CalculateUnionArea(nodeB, newNode) - areaB;
			if (areaDeltaA < areaDeltaB)
				return nodeA;
			else if (areaDeltaA > areaDeltaB)
				return nodeB;
			else {
				if (areaA < areaB)
					return nodeA;
				else if (areaA > areaB)
					return nodeB;
				else {
					if (nodeA.Children.Count < nodeB.Children.Count)
						return nodeA;
					else if (nodeA.Children.Count > nodeB.Children.Count)
						return nodeB;
					else {
						if ((int)Math.Round(random.NextDouble()) == 0)
							return nodeA;
						else
							return nodeB;
					}
				}
			}
		}
		Node[] QuadraticPickSeeds(List<Node> nn) {
			Node[] bestPair = new Node[2];
			float maxWaste = coordinateProvider.Multiply(coordinateProvider.MaxValue, -1.0f);
			foreach (Node nodeA in nn) {
				foreach (Node nodeB in nn) {
					if (nodeA == nodeB)
						continue;
					float areaA = nodeA.Area;
					float areaB = nodeB.Area;
					float unionArea = CalculateUnionArea(nodeA, nodeB);
					float waste = unionArea - areaA - areaB;
					if (waste > maxWaste) {
						maxWaste = waste;
						bestPair[0] = nodeA;
						bestPair[1] = nodeB;
					}
				}
			}
			nn.Remove(bestPair[0]);
			nn.Remove(bestPair[1]);
			return bestPair;
		}
		Node QuadraticPickNext(List<Node> cc, Node[] nn) {
			float maxDiff = coordinateProvider.Multiply(coordinateProvider.MaxValue, -1.0f);
			Node nextC = null;
			foreach (Node c in cc) {
				float n0Exp = CalculateUnionArea(nn[0], c) - nn[0].Area;
				float n1Exp = CalculateUnionArea(nn[1], c) - nn[1].Area;
				float diff = Math.Abs(n1Exp - n0Exp);
				if (diff > maxDiff) {
					maxDiff = diff;
					nextC = c;
				}
			}
			Debug.Assert(nextC != null, "No node selected from qPickNext", "");
			cc.Remove(nextC);
			return nextC;
		}
		Node[] LinearPickSeeds(List<Node> nn) {
			Node[] bestPair = new Node[2];
			float bestSep = 0.0f;
			float dimLb = coordinateProvider.MaxValue;
			float dimMinUb = coordinateProvider.MaxValue;
			float dimUb = coordinateProvider.Multiply(coordinateProvider.MaxValue, -1.0f);
			float dimMaxLb = coordinateProvider.Multiply(coordinateProvider.MaxValue, -1.0f);
			Node nMaxLb = null, nMinUb = null;
			foreach (Node n in nn) {
				if (n.ColumnIndex < dimLb) {
					dimLb = n.ColumnIndex;
				}
				if (n.Width + n.ColumnIndex > dimUb) {
					dimUb = n.Width + n.ColumnIndex;
				}
				if (n.ColumnIndex > dimMaxLb) {
					dimMaxLb = n.ColumnIndex;
					nMaxLb = n;
				}
				if (n.Width + n.ColumnIndex < dimMinUb) {
					dimMinUb = n.Width + n.ColumnIndex;
					nMinUb = n;
				}
			}
			float sep = (nMaxLb == nMinUb) ? -1.0f : Math.Abs((dimMinUb - dimMaxLb) / (dimUb - dimLb));
			if (sep >= bestSep) {
				bestPair[0] = nMaxLb;
				bestPair[1] = nMinUb;
				bestSep = sep;
			}
			dimLb = coordinateProvider.MaxValue;
			dimMinUb = coordinateProvider.MaxValue;
			dimUb = coordinateProvider.Multiply(coordinateProvider.MaxValue, -1.0f);
			dimMaxLb = coordinateProvider.Multiply(coordinateProvider.MaxValue, -1.0f);
			nMaxLb = null;
			nMinUb = null;
			foreach (Node n in nn) {
				if (n.RowIndex < dimLb) {
					dimLb = n.RowIndex;
				}
				if (n.Height + n.RowIndex > dimUb) {
					dimUb = n.Height + n.RowIndex;
				}
				if (n.RowIndex > dimMaxLb) {
					dimMaxLb = n.RowIndex;
					nMaxLb = n;
				}
				if (n.Height + n.RowIndex < dimMinUb) {
					dimMinUb = n.Height + n.RowIndex;
					nMinUb = n;
				}
			}
			sep = (nMaxLb == nMinUb) ? -1.0f : Math.Abs((dimMinUb - dimMaxLb) / (dimUb - dimLb));
			if (sep >= bestSep) {
				bestPair[0] = nMaxLb;
				bestPair[1] = nMinUb;
				bestSep = sep;
			}
			if (bestPair[0] == null)
				bestPair = new Node[] { nn[0], nn[1] };
			nn.Remove(bestPair[0]);
			nn.Remove(bestPair[1]);
			return bestPair;
		}
		Node LinearPickNext(List<Node> cc) {
			Node result = cc[0];
			cc.RemoveAt(0);
			return result;
		}
		void Tighten(Node n) {
			Debug.Assert(n.Children.Count > 0, "tighten() called on empty node!", "");
			int columnIndex = coordinateProvider.MaxValue;
			int width = coordinateProvider.MinValue;
			int rowIndex = coordinateProvider.MaxValue;
			int height = coordinateProvider.MinValue;
			foreach (Node c in n.Children) {
				c.Parent = n;
				columnIndex = Math.Min(columnIndex, c.ColumnIndex);
				width = Math.Max(width, c.ColumnIndex + c.Width);
				rowIndex = Math.Min(rowIndex, c.RowIndex);
				height = Math.Max(height, c.RowIndex + c.Height);
			}
			width -= columnIndex;
			height -= rowIndex;
			n.ColumnIndex = columnIndex;
			n.RowIndex = rowIndex;
			n.Width = width;
			n.Height = height;
		}
		void Tighten(Node[] nodes) {
			Debug.Assert(nodes.Length >= 1);
			foreach (Node n in nodes)
				Tighten(n);
		}
		Node ChooseLeaf(Node node, Entry entry) {
			if (node.IsLeaf)
				return node;
			Debug.Assert(node.Children.Count > 0);
			float minDelta = coordinateProvider.MaxValue;
			Node next = null;
			float nextArea = 0;
			foreach (Node child in node.Children) {
				float expandedArea = CalculateUnionArea(child, entry);
				float area = child.Area;
				float delta = expandedArea - area;
				if (delta < minDelta) {
					minDelta = delta;
					next = child;
					nextArea = area;
				}
				else if (delta == minDelta) {
					if (area < nextArea)
						next = child;
				}
			}
			return ChooseLeaf(next, entry);
		}
		int CalculateUnionArea(Node node, Node newNode) {
			int near = Math.Min(node.ColumnIndex, newNode.ColumnIndex);
			int far = Math.Max(node.LastColumnIndex, newNode.LastColumnIndex);
			int expanded = far - near;
			near = Math.Min(node.RowIndex, newNode.RowIndex);
			far = Math.Max(node.LastRowIndex, newNode.LastRowIndex);
			expanded *= (far - near);
			return expanded;
		}
		bool IsOverlap(NodeBase nodeA, NodeBase nodeB, bool allowIntersection) {
			if (!IsOverlap(nodeA.ColumnIndex, nodeA.Width, nodeB.ColumnIndex, nodeB.Width, allowIntersection))
				return false;
			return IsOverlap(nodeA.RowIndex, nodeA.Height, nodeB.RowIndex, nodeB.Height, allowIntersection);
		}
		bool IsOverlap(int coordA, int extentA, int coordB, int extentB, bool allowIntersection) {
			if (allowIntersection) {
				float minFar = Math.Min(coordA + extentA, coordB + extentB);
				float maxNear = Math.Max(coordA, coordB);
				return maxNear < minFar;
			}
			else
				return coordA + extentA >= coordB + extentB && coordA <= coordB;
		}
		#region IEnumerable<T> implementation
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public IEnumerator<T> GetEnumerator() {
			return new RTreeEnumerator(root);
		}
		#endregion
	}
}
