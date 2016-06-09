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
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils.Implementation;
using System.Threading;
using System.Reflection;
namespace DevExpress.Utils {
	public interface IVector<T> {
		int Count { get; }
		T this[int index] { get; set; }
	}
	public static class Algorithms {
		public static int BinarySearch<T>(IList<T> list, T value, IComparer<T> comparer) {
			return BinarySearch<T>(list, 0, list.Count, value, comparer);
		}
		public static int BinarySearch<T>(IList<T> list, int index, int length, T value, IComparer<T> comparer) {
			if(comparer == null)
				comparer = Comparer<T>.Default;
			int result = BinarySearchCore<T>(list, index, length, value, comparer);
			return result;
		}
		static int BinarySearchCore<T>(IList<T> list, int index, int length, T value, IComparer<T> comparer) {
			int i = index;
			int num = index + length - 1;
			while(i <= num) {
				int pos = i + (num - i >> 1);
				int result = comparer.Compare(list[pos], value);
				if(result == 0)
					return pos;
				if(result < 0)
					i = pos + 1;
				else
					num = pos - 1;
			}
			return ~i;
		}
		public static int BinarySearch<T>(IList<T> list, IComparable<T> predicate) {
			return BinarySearch(list, predicate, 0, list.Count - 1);
		}
		public static int BinarySearch<T>(IVector<T> list, IComparable<T> predicate) {
			return BinarySearch(list, predicate, 0, list.Count - 1);
		}
		public static int BinarySearch<T>(IList<T> list, IComparable<T> predicate, int startIndex, int endIndex) {
			int low = startIndex;
			int hi = endIndex;
			while (low <= hi) {
				int median = (low + ((hi - low) >> 1));
				int compareResult = predicate.CompareTo(list[median]);
				if (compareResult == 0)
					return median;
				if (compareResult < 0)
					low = median + 1;
				else
					hi = median - 1;
			}
			return ~low;
		}
		public static int BinarySearch<T>(IVector<T> list, IComparable<T> predicate, int startIndex, int endIndex) {
			int low = startIndex;
			int hi = endIndex;
			while (low <= hi) {
				int median = (low + ((hi - low) >> 1));
				int compareResult = predicate.CompareTo(list[median]);
				if (compareResult == 0)
					return median;
				if (compareResult < 0)
					low = median + 1;
				else
					hi = median - 1;
			}
			return ~low;
		}
		public static void InvertElementsOrder<T>(IList<T> list) {
			int maxIndex = list.Count - 1;
			if (maxIndex < 0)
				return;
			int maxIndex2 = maxIndex / 2;
			for (int i = 0; i <= maxIndex2; i++)
				SwapElements(list, i, maxIndex - i);
		}
		public static void InvertElementsOrder<T>(IVector<T> list) {
			int maxIndex = list.Count - 1;
			if (maxIndex < 0)
				return;
			int maxIndex2 = maxIndex / 2;
			for (int i = 0; i <= maxIndex2; i++)
				SwapElements(list, i, maxIndex - i);
		}
		public static void SwapElements<T>(IList<T> list, int index1, int index2) {
			T value = list[index1];
			list[index1] = list[index2];
			list[index2] = value;
		}
		public static void SwapElements<T>(IVector<T> list, int index1, int index2) {
			T value = list[index1];
			list[index1] = list[index2];
			list[index2] = value;
		}
		public static T Min<T>(T index1, T index2) where T : IComparable<T> {
			if (index1.CompareTo(index2) < 0)
				return index1;
			else
				return index2;
		}
		public static T Max<T>(T index1, T index2) where T : IComparable<T> {
			if (index1.CompareTo(index2) < 0)
				return index2;
			else
				return index1;
		}
		public static IList<T> TopologicalSort<T>(IList<T> sourceObjects, IComparer<T> comparer) {
			TopologicalSorter<T> sorter = new TopologicalSorter<T>();
			return sorter.Sort(sourceObjects, comparer);
		}
	}
}
namespace DevExpress.Utils.Implementation {
	public delegate bool IsDependOnDelegate(int y, int x);
#region TopologicalSorter
	public class TopologicalSorter<T> {
#region Node
		protected internal class Node {
			readonly int oneBasedSuccessorIndex;
			readonly Node next;
			public Node(int oneBasedSuccessorIndex, Node next) {
				this.oneBasedSuccessorIndex = oneBasedSuccessorIndex;
				this.next = next;
			}
			public int OneBasedSuccessorIndex { get { return oneBasedSuccessorIndex; } }
			public Node Next { get { return next; } }
		}
#endregion
		protected internal class DefaultDependencyCalculator {
			readonly IList<T> sourceObjects;
			readonly IComparer<T> comparer;
			public DefaultDependencyCalculator(IList<T> sourceObjects, IComparer<T> comparer) {
				this.sourceObjects = sourceObjects;
				this.comparer = comparer;
			}
			public bool IsDependOn(int x, int y) {
				return comparer.Compare(sourceObjects[x], sourceObjects[y]) > 0;
			}
		}
#region Fields
		int[] qLink;
		Node[] nodes;
		bool success;
#endregion
#region Properties
		protected internal Node[] Nodes { get { return nodes; } }
		protected internal int[] QLink { get { return qLink; } }
		public bool Success { get { return success; } }
#endregion
		public IList<T> Sort(IList<T> sourceObjects, IComparer<T> comparer) {
			DefaultDependencyCalculator dependencyCalculator = new DefaultDependencyCalculator(sourceObjects, comparer != null ? comparer : System.Collections.Generic.Comparer<T>.Default);
			return Sort(sourceObjects, dependencyCalculator.IsDependOn);
		}
		public IList<T> Sort(IList<T> sourceObjects) {
			DefaultDependencyCalculator dependencyCalculator = new DefaultDependencyCalculator(sourceObjects, System.Collections.Generic.Comparer<T>.Default);
			return Sort(sourceObjects, dependencyCalculator.IsDependOn);
		}
		public IList<T> Sort(IList<T> sourceObjects, IsDependOnDelegate isDependOn) {
			int n = sourceObjects.Count;
			if (n < 2) {
				this.success = true;
				return sourceObjects;
			}
			Initialize(n);
			CalculateRelations(sourceObjects, isDependOn);
			int lastNoPredecessorItemIndex = CreateVirtualNoPredecessorsItemList();
			IList<T> result = ProcessNodes(lastNoPredecessorItemIndex, sourceObjects);
			return result.Count > 0 ? result : sourceObjects;
		}
		protected internal void Initialize(int n) {
			int count = n + 1;
			this.qLink = new int[count];
			this.nodes = new Node[count];
		}
		protected internal virtual void AppendRelation(int successorIndex, int predecessorIndex) {
			int oneBasedPredecessorIndex = predecessorIndex + 1;
			int oneBasedSuccessorIndex = successorIndex + 1;
			QLink[oneBasedSuccessorIndex]++;
			Nodes[oneBasedPredecessorIndex] = new Node(oneBasedSuccessorIndex, Nodes[oneBasedPredecessorIndex]);
		}
		protected internal virtual void CalculateRelations(IList<T> sourceObjects, IsDependOnDelegate isDependOn) {
			int n = sourceObjects.Count;
			for (int y = n - 1; y >= 0; y--) {
				for (int x = n - 1; x >= 0; x--) {
					if (isDependOn(y, x))
						AppendRelation(y, x);
				}
			}
		}
		protected internal int CreateVirtualNoPredecessorsItemList() {
			int r = 0;
			int n = QLink.Length;
			for (int i = 1; i < n; i++) {
				if (QLink[i] == 0) {
					QLink[r] = i;
					r = i;
				}
			}
			return r;
		}
		protected virtual IList<T> ProcessNodes(int lastNoPredecessorItemIndex, IList<T> sourceObjects) {
			int n = sourceObjects.Count;
			int itemsLeft = n;
			int f = QLink[0]; 
			List<T> result = new List<T>(n);
			while (f > 0) {
				result.Add(sourceObjects[f - 1]);
				itemsLeft--;
				Node node = Nodes[f];
				while (node != null) {
					node = RemoveRelation(node, ref lastNoPredecessorItemIndex);
				}
				f = QLink[f];
			}
			this.success = (itemsLeft == 0);
			return result;
		}
		Node RemoveRelation(Node node, ref int lastNoPredecessorItemIndex) {
			int index = node.OneBasedSuccessorIndex;
			QLink[index]--; 
			if (QLink[index] == 0) { 
				QLink[lastNoPredecessorItemIndex] = index; 
				lastNoPredecessorItemIndex = index;
			}
			return node.Next;
		}
	}
#endregion
}
