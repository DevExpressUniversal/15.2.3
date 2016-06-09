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
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Utils.Trees {
	public enum RTreeSeedPickMethod {
		Linear,
		Quadratic
	}
	public class RTree<T> : IEnumerable<T> where T : class {
		static readonly Random random = new Random();
		readonly int maxEntries;
		readonly int minEntries;
		readonly int dimensionCount;
		readonly IRTreeCoordinateProvider<float> coordinateProvider;
		readonly RTreeSeedPickMethod seedPicker;
		Node root;
		volatile int size;
		public RTree(int maxEntries, int minEntries, int dimensionCount, RTreeSeedPickMethod seedPicker) {
			Debug.Assert(minEntries <= (maxEntries / 2));
			this.coordinateProvider = new RTreeFloatCoordinateProvider();
			this.dimensionCount = dimensionCount;
			this.maxEntries = maxEntries;
			this.minEntries = minEntries;
			this.seedPicker = seedPicker;
			root = BuildRoot(true);
		}
		public RTree(int maxEntries, int minEntries, int dimensionCount)
			: this(maxEntries, minEntries, dimensionCount, RTreeSeedPickMethod.Linear) {
		}
		public RTree()
			: this(16, 2, 2, RTreeSeedPickMethod.Linear) {
		}
		public int Count { get { return size; } }
		Node BuildRoot(bool asLeaf) {
			float defaultCoord = coordinateProvider.Sqrt(coordinateProvider.MaxValue);
			float defaultDimension = coordinateProvider.Multiply(defaultCoord, -2.0f);
			float[] coords = new float[dimensionCount];
			float[] dimensions = new float[dimensionCount];
			for (int i = 0; i < this.dimensionCount; i++) {
				coords[i] = defaultCoord;
				dimensions[i] = defaultDimension;
			}
			return new Node(coords, dimensions, asLeaf);
		}
		#region Search
		public List<T> Search(float[] coords, float[] dimensions) {
			return Search(coords, dimensions, true);
		}
		public List<T> Search(float[] coords, float[] dimensions, bool allowIntersection) {
			Debug.Assert(coords.Length == dimensionCount);
			Debug.Assert(dimensions.Length == dimensionCount);
			List<T> results = new List<T>();
			Search(coords, dimensions, root, results, allowIntersection);
			return results;
		}
		void Search(float[] coords, float[] dimensions, Node node, IList<T> results, bool allowIntersection) {
			if (node.IsLeaf) {
				foreach (Node child in node.Children)
					if (IsOverlap(coords, dimensions, child.Coords, child.Dimensions, allowIntersection))
						results.Add(((Entry)child).Value);
			}
			else {
				foreach (Node child in node.Children)
					if (IsOverlap(coords, dimensions, child.Coords, child.Dimensions, allowIntersection))
						Search(coords, dimensions, child, results, allowIntersection);
			}
		}
		#endregion
		public bool Delete(float[] coords, float[] dimensions, T value) {
			Debug.Assert(coords.Length == dimensionCount);
			Debug.Assert(dimensions.Length == dimensionCount);
			Node leaf = FindLeaf(root, coords, dimensions, value);
			if (leaf == null) {
				Debug.WriteLine("somthing goes wrong!");
				FindLeaf(root, coords, dimensions, value);
			}
			Debug.Assert(leaf != null, "Could not find leaf for entry to delete", "");
			Debug.Assert(leaf.IsLeaf, "Entry is not found at leaf", "");
			IEnumerator<Node> enumerator = leaf.Children.GetEnumerator();
			T removed = null;
			while (enumerator.MoveNext()) {
				Entry entry = (Entry)enumerator.Current;
				if (entry.Value.Equals(value)) {
					removed = entry.Value;
					leaf.Children.Remove(entry);
					break;
				}
			}
			if (removed != null) {
				CondenseTree(leaf);
				size--;
			}
			if (size == 0)
				root = BuildRoot(true);
			return removed != null;
		}
		Node FindLeaf(Node node, float[] coords, float[] dimensions, T entry) {
			if (node.IsLeaf) {
				foreach (Entry child in node.Children)
					if (child.Value.Equals(entry))
						return node;
				return null;
			}
			else {
				foreach (Node child in node.Children) {
					if (IsOverlap(child.Coords, child.Dimensions, coords, dimensions, true)) {
						Node result = FindLeaf(child, coords, dimensions, entry);
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
					q.addAll(n.Children);
					n.Parent.Children.Remove(n);
				}
				else if (!n.IsLeaf && (n.Children.Count < minEntries)) {
					LinkedList<Node> toVisit = new LinkedList<Node>(n.Children);
					while (toVisit.Count > 0) {
						Node c = toVisit.First.Value;
						toVisit.RemoveFirst();
						if (c.IsLeaf)
							q.addAll(c.Children);
						else
							toVisit.addAll(c.Children);
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
				root = root.Children.First.Value;
				root.Parent = null;
			}
			else
				Tighten(root);
			foreach (Entry e in q)
				Insert(e.Coords, e.Dimensions, e.Value);
			size -= q.Count;
		}
		public void Clear() {
			root = BuildRoot(true);
			size = 0;
		}
		public void Insert(float[] coords, float[] dimensions, T value) {
			Debug.Assert(coords.Length == dimensionCount);
			Debug.Assert(dimensions.Length == dimensionCount);
			Entry entry = new Entry(coords, dimensions, value);
			Node leaf = ChooseLeaf(root, entry);
			leaf.Children.AddLast(entry);
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
					root.Children.AddLast(node);
					node.Parent = root;
					root.Children.AddLast(newNode);
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
			Node[] nodes = new Node[] { node, new Node(node.Coords, node.Dimensions, node.IsLeaf) };
			nodes[1].Parent = node.Parent;
			if (nodes[1].Parent != null) {
				nodes[1].Parent.Children.AddLast(nodes[1]);
			}
			LinkedList<Node> children = new LinkedList<Node>(node.Children);
			node.Children.Clear();
			Node[] seeds = seedPicker == RTreeSeedPickMethod.Linear ? LinearPickSeeds(children) : QuadraticPickSeeds(children);
			nodes[0].Children.AddLast(seeds[0]);
			nodes[1].Children.AddLast(seeds[1]);
			Tighten(nodes);
			while (children.Count > 0) {
				if ((nodes[0].Children.Count >= minEntries) && (nodes[1].Children.Count + children.Count == minEntries)) {
					nodes[1].Children.addAll(children);
					children.Clear();
					Tighten(nodes); 
					return nodes;
				}
				else if ((nodes[1].Children.Count >= minEntries) && (nodes[0].Children.Count + children.Count == minEntries)) {
					nodes[0].Children.addAll(children);
					children.Clear();
					Tighten(nodes); 
					return nodes;
				}
				Node c = seedPicker == RTreeSeedPickMethod.Linear ? LinearPickNext(children) : QuadraticPickNext(children, nodes);
				Node preferredNode = PickPreferredNode(nodes[0], nodes[1], c);
				preferredNode.Children.AddLast(c);
				Tighten(preferredNode);
			}
			return nodes;
		}
		Node PickPreferredNode(Node nodeA, Node nodeB, Node newNode) {
			float areaA = GetArea(nodeA.Dimensions);
			float areaB = GetArea(nodeB.Dimensions);
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
		Node[] QuadraticPickSeeds(LinkedList<Node> nn) {
			Node[] bestPair = new Node[2];
			float maxWaste = coordinateProvider.Multiply(coordinateProvider.MaxValue, -1.0f);
			foreach (Node nodeA in nn) {
				foreach (Node nodeB in nn) {
					if (nodeA == nodeB)
						continue;
					float areaA = GetArea(nodeA.Dimensions);
					float areaB = GetArea(nodeB.Dimensions);
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
		Node QuadraticPickNext(LinkedList<Node> cc, Node[] nn) {
			float maxDiff = coordinateProvider.Multiply(coordinateProvider.MaxValue, -1.0f);
			;
			Node nextC = null;
			foreach (Node c in cc) {
				float n0Exp = CalculateUnionArea(nn[0], c) - GetArea(nn[0].Dimensions);
				float n1Exp = CalculateUnionArea(nn[1], c) - GetArea(nn[1].Dimensions);
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
		Node[] LinearPickSeeds(LinkedList<Node> nn) {
			Node[] bestPair = new Node[2];
			float bestSep = 0.0f;
			for (int i = 0; i < dimensionCount; i++) {
				float dimLb = coordinateProvider.MaxValue;
				float dimMinUb = coordinateProvider.MaxValue;
				float dimUb = coordinateProvider.Multiply(coordinateProvider.MaxValue, -1.0f);
				;
				float dimMaxLb = coordinateProvider.Multiply(coordinateProvider.MaxValue, -1.0f);
				;
				Node nMaxLb = null, nMinUb = null;
				foreach (Node n in nn) {
					if (n.Coords[i] < dimLb) {
						dimLb = n.Coords[i];
					}
					if (n.Dimensions[i] + n.Coords[i] > dimUb) {
						dimUb = n.Dimensions[i] + n.Coords[i];
					}
					if (n.Coords[i] > dimMaxLb) {
						dimMaxLb = n.Coords[i];
						nMaxLb = n;
					}
					if (n.Dimensions[i] + n.Coords[i] < dimMinUb) {
						dimMinUb = n.Dimensions[i] + n.Coords[i];
						nMinUb = n;
					}
				}
				float sep = (nMaxLb == nMinUb) ? -1.0f : Math.Abs((dimMinUb - dimMaxLb) / (dimUb - dimLb));
				if (sep >= bestSep) {
					bestPair[0] = nMaxLb;
					bestPair[1] = nMinUb;
					bestSep = sep;
				}
			}
			if (bestPair[0] == null)
				bestPair = new Node[] { nn.First.Value, nn.First.Next.Value };
			nn.Remove(bestPair[0]);
			nn.Remove(bestPair[1]);
			return bestPair;
		}
		Node LinearPickNext(LinkedList<Node> cc) {
			Node result = cc.First.Value;
			cc.RemoveFirst();
			return result;
		}
		void Tighten(Node n) {
			Debug.Assert(n.Children.Count > 0, "tighten() called on empty node!", "");
			float[] minCoords = new float[dimensionCount];
			float[] maxCoords = new float[dimensionCount];
			for (int i = 0; i < dimensionCount; i++) {
				minCoords[i] = coordinateProvider.MaxValue;
				maxCoords[i] = coordinateProvider.MinValue;
				foreach (Node c in n.Children) {
					c.Parent = n;
					minCoords[i] = Math.Min(minCoords[i], c.Coords[i]);
					maxCoords[i] = Math.Max(maxCoords[i], c.Coords[i] + c.Dimensions[i]);
				}
			}
			for (int i = 0; i < dimensionCount; i++) {
				maxCoords[i] -= minCoords[i];
			}
			Array.Copy(minCoords, 0, n.Coords, 0, dimensionCount);
			Array.Copy(maxCoords, 0, n.Dimensions, 0, dimensionCount);
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
				float area = GetArea(child.Dimensions);
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
		float CalculateUnionArea(Node node, Node newNode) {
			float[] coords = node.Coords;
			float[] dimensions = node.Dimensions;
			float expanded = 1.0f;
			for (int i = 0; i < dimensions.Length; i++) {
				float near = Math.Min(coords[i], newNode.Coords[i]);
				float far = Math.Max(coords[i] + dimensions[i], newNode.Coords[i] + newNode.Dimensions[i]);
				expanded *= (far - near);
			}
			return expanded;
		}
		float GetArea(float[] dimensions) {
			float area = 1.0f;
			int count = dimensions.Length;
			for (int i = 0; i < count; i++)
				area *= dimensions[i];
			return area;
		}
		bool IsOverlap(float[] coordsA, float[] dimensionsA, float[] coordsB, float[] dimensionsB, bool allowIntersection) {
			for (int i = 0; i < coordsA.Length; i++)
				if (!IsOverlap(coordsA[i], dimensionsA[i], coordsB[i], dimensionsB[i], allowIntersection))
					return false;
			return true;
		}
		bool IsOverlap(float coordA, float extentA, float coordB, float extentB, bool allowIntersection) {
			if (allowIntersection) {
				float minFar = Math.Min(coordA + extentA, coordB + extentB);
				float maxNear = Math.Max(coordA, coordB);
				return maxNear < minFar;
			}
			else {
				return coordA + extentA >= coordB + extentB - 0.41 && coordA - 0.51 <= coordB;
			}
		}
		public void ToString(StringBuilder where) {
			this.root.ToString(where, String.Empty);
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder();
			ToString(result);
			return result.ToString();
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
		#region Node
		class Node {
			readonly float[] coords;
			readonly float[] dimensions;
			readonly LinkedList<Node> children;
			readonly bool isLeaf;
			Node parent;
			internal Node(float[] coords, float[] dimensions, bool isLeaf) {
				this.coords = new float[coords.Length];
				this.dimensions = new float[dimensions.Length];
				Array.Copy(coords, 0, this.coords, 0, coords.Length);
				Array.Copy(dimensions, 0, this.dimensions, 0, dimensions.Length);
				this.isLeaf = isLeaf;
				this.children = new LinkedList<Node>();
			}
			public bool IsLeaf { get { return isLeaf; } }
			public float[] Coords { get { return coords; } }
			public float[] Dimensions { get { return dimensions; } }
			public LinkedList<Node> Children { get { return children; } }
			public Node Parent { get { return parent; } set { parent = value; } }
			public void ToString(StringBuilder where, string indent) {
				where.Append(indent);
				where.AppendLine("{");
				string newIndent = indent + "  ";
				where.Append(newIndent);
				where.Append("coordinates { ");
				int count = coords.Length;
				for (int i = 0; i < count; i++) {
					if (i > 0)
						where.Append(", ");
					where.Append(coords[i]);
				}
				where.AppendLine(" }");
				where.Append(newIndent);
				where.Append("dimensions: { ");
				for (int i = 0; i < count; i++) {
					if (i > 0)
						where.Append(", ");
					where.Append(dimensions[i]);
				}
				where.AppendLine(" }");
				ContentToString(where, newIndent);
				where.Append(indent);
				where.AppendLine("}");
			}
			public virtual void ContentToString(StringBuilder where, string indent) {
				where.Append(indent);
				where.AppendLine(String.Format("children ({0})", children.Count));
				foreach (Node node in Children)
					node.ToString(where, indent);
			}
		}
		#endregion
		#region Entry
		class Entry : Node {
			readonly T value;
			public Entry(float[] coords, float[] dimensions, T value)
				: base(coords, dimensions, true) {
				this.value = value;
			}
			public T Value { get { return value; } }
			public override void ContentToString(StringBuilder where, string indent) {
				where.Append(indent);
				where.Append("value:      { ");
				where.Append(value.ToString());
				where.AppendLine(" }");
			}
		}
		#endregion
		class RTreeEnumerator : IEnumerator<T> {
			readonly Stack<IEnumerator<RTree<T>.Node>> nodesEnumerators = new Stack<IEnumerator<RTree<T>.Node>>();
			readonly RTree<T>.Node root;
			T current;
			public RTreeEnumerator(RTree<T>.Node root) {
				Guard.ArgumentNotNull(root, "root");
				this.root = root;
				Reset();
			}
			T IEnumerator<T>.Current { get { return GetCurrent(); } }
			object IEnumerator.Current { get { return GetCurrent(); } }
			void IDisposable.Dispose() {
			}
			public bool MoveNext() {
				if (nodesEnumerators.Count <= 0)
					return false;
				IEnumerator<RTree<T>.Node> enumerator = nodesEnumerators.Peek();
				for (; ; ) {
					if (!enumerator.MoveNext()) {
						nodesEnumerators.Pop();
						return false;
					}
					RTree<T>.Node currentNode = enumerator.Current;
					RTree<T>.Entry entry = currentNode as RTree<T>.Entry;
					if (entry != null) {
						this.current = entry.Value;
						return true;
					}
					else {
						nodesEnumerators.Push(currentNode.Children.GetEnumerator());
						if (MoveNext())
							return true;
					}
				}
			}
			public void Reset() {
				this.nodesEnumerators.Clear();
				this.nodesEnumerators.Push(root.Children.GetEnumerator());
			}
			T GetCurrent() {
				return current;
			}
		}
	}
}
