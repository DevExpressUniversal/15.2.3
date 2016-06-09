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
using System.Text;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	public interface IHasRelativeIndex<TItem> where TItem : class, IHasRelativeIndex<TItem> {
		void ShiftRelativeIndex(int delta);
		void SetRelativeIndex(int relativeIndex);
		void ShiftRelativePosition(int deltaLength, int deltaFirstRunIndex, int deltaLastRunIndex);
		IndexedTreeNodeLeafLevel<TItem> Parent { get; set; }
		int GetRelativeFirstRunIndex();
		int GetRelativeLastRunIndex();
		int GetRelativeLogPosition();
	}
#if !SL
	[System.Diagnostics.DebuggerDisplay("{GetDebugString()}")]
#endif
	public class IndexedTree<TItem, TIndex> : IEnumerable<TItem>
		where TIndex : struct, IConvertToInt<TIndex>
		where TItem : class, IHasRelativeIndex<TItem> {
		#region static & const
		static protected TIndex IndexConverter = default(TIndex);
		const int defaultMaxChildrenCount = 16;
		#endregion
		#region fields
		readonly int maxChildrenCount;
		IndexedTreeNodeBase<TItem> root;
		int count;
		#endregion
		public IndexedTree()
			: this(defaultMaxChildrenCount) {
		}
		public IndexedTree(int maxChildrenCount) {
			this.maxChildrenCount = maxChildrenCount;
			Clear();
		}
		public TItem this[TIndex index] { get { return FindItemByIndex(index); } }
		public TItem First { get { return root.First; } }
		public TItem Last { get { return root.Last; } }
		public int Count { get { return count; } }
		protected internal int MaxChildrenCount { get { return maxChildrenCount; } }
		protected internal IndexedTreeNodeBase<TItem> Root { get { return root; } }
#if !SL && !DXPORTABLE
		protected internal string GetDebugString() {
			using (System.IO.StringWriter sw = new System.IO.StringWriter()) {
				using (System.CodeDom.Compiler.IndentedTextWriter tw = new System.CodeDom.Compiler.IndentedTextWriter(sw)) {
					if(Root != null)
						WriteDebugString(tw, Root);
					tw.Flush();
				}
				sw.Flush();
				return sw.GetStringBuilder().ToString();
			}
		}
		protected internal void WriteDebugString(System.CodeDom.Compiler.IndentedTextWriter tw, IndexedTreeNodeBase<TItem> item) {
			IndexedTreeNodeMiddleLevel<TItem> middleLevel = item as IndexedTreeNodeMiddleLevel<TItem>;
			if (middleLevel != null)
				WriteDebugString(tw, middleLevel);
			else
				WriteDebugString(tw, (IndexedTreeNodeLeafLevel<TItem>)item);
		}
		protected internal void WriteDebugString(System.CodeDom.Compiler.IndentedTextWriter tw, IndexedTreeNodeMiddleLevel<TItem> item) {
			tw.WriteLine(String.Format("{0},{1},{2}:(", item.FirstRelativeIndex, item.RelativeFirstRunIndex, item.RelativeLogPosition));
			tw.Indent++;
			List<IndexedTreeNodeBase<TItem>> children= item.Children;
			int count = children.Count;
			for (int i = 0; i < count; i++) {
				WriteDebugString(tw, children[i]);
			}
			tw.Indent--;
			tw.WriteLine("),");
		}
		protected internal void WriteDebugString(System.CodeDom.Compiler.IndentedTextWriter tw, IndexedTreeNodeLeafLevel<TItem> item) {
			tw.Write(String.Format("{0},{1},{2}:(", item.FirstRelativeIndex, item.RelativeFirstRunIndex, item.RelativeLogPosition));						
			List<TItem> items = item.Items;
			int count = items.Count;
			List<String> itemStrings = new List<string>(count);
			for (int i = 0; i < count; i++) {
				itemStrings.Add(items[i].GetRelativeLogPosition().ToString());
			}
			tw.Write(String.Join(",", itemStrings.ToArray()));
			tw.WriteLine("),");
		}
#endif
		protected IndexedTreeNodeLeafLevel<TItem> CreateLeafLevel() {
			return new IndexedTreeNodeLeafLevel<TItem>(MaxChildrenCount);
		}
		protected IndexedTreeNodeMiddleLevel<TItem> CreateMiddleLevel() {
			return new IndexedTreeNodeMiddleLevel<TItem>(MaxChildrenCount);
		}
		TItem FindItemByIndex(TIndex index) {
			int intIndex = index.ToInt();
			if (intIndex < 0)
				throw new ArgumentOutOfRangeException("index");			
			return root.FindItemByIndex(intIndex);
		}
		public void Add(TItem item) {
			InsertCore(Count, item);
		}
		public void Insert(TIndex index, TItem item) {
			InsertCore(index.ToInt(), item);			
		}
		protected void InsertCore(int index, TItem item) {
			CheckTree(null);
			count++;
			IndexedTreeNodeBase<TItem> newChild = root.Insert(index, item);
			if (newChild == null)
				return;
			IndexedTreeNodeMiddleLevel<TItem> newRoot = CreateMiddleLevel();
			newRoot.AddChild(root);
			newRoot.AddChild(newChild);
			root = newRoot;			
		}
		[System.Diagnostics.Conditional("DEBUGTEST")]
		protected internal virtual void CheckTree(TItem ignoreItem) {
			CheckNode(root);
		}
		[System.Diagnostics.Conditional("DEBUGTEST")]
		protected virtual void CheckNode(IndexedTreeNodeBase<TItem> node) {
			if(Count == 0)
				return;
			IndexedTreeNodeMiddleLevel<TItem> middleLevel = node as IndexedTreeNodeMiddleLevel<TItem>;
			if (middleLevel != null) {
				Debug.Assert(middleLevel.ChildrenCount > 0);
				Debug.Assert(middleLevel.Children[0].RelativeFirstRunIndex == 0);
				Debug.Assert(middleLevel.Children[0].RelativeLogPosition == 0);
#if DEBUGTEST
				foreach (IndexedTreeNodeBase<TItem> child in middleLevel.Children)
					CheckNode(child);
#endif
			}
			else {
				IndexedTreeNodeLeafLevel<TItem> leafLevel = (IndexedTreeNodeLeafLevel<TItem>)node;
				Debug.Assert(leafLevel.ChildrenCount > 0);
				Debug.Assert(leafLevel.Items[0].GetRelativeFirstRunIndex() == 0);
				Debug.Assert(leafLevel.Items[0].GetRelativeLogPosition() == 0);
			}
		}
		public void RemoveAt(TIndex relativeIndex) {
			count--;
			bool shouldClear = root.RemoveAt(relativeIndex.ToInt());
			if (!shouldClear) {
				CheckTree(null);
				return;
			}
			root = CreateLeafLevel();
			CheckTree(null);
		}
		public void Clear() {
			root = CreateLeafLevel();
			count = 0;
		}
		public IEnumerator<TItem> GetEnumerator() {
			return new IndexedTreeEnumerator<TItem>(root);
		}
		public IEnumerable<TItem> Range(TIndex from, TIndex to) {
			return new IndexedTreeRangeEnumerable<TItem, TIndex>(this, from, to);
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public void ForEach(Action<TItem> action) {
			foreach (TItem item in this)
				action(item);
		}
		public void AddRange(IEnumerable<TItem> collection) {
			foreach (TItem item in collection)
				Add(item);
		}
		public List<TItem> GetRange(TIndex index, int count) {
			List<TItem> result = new List<TItem>();
			int startIndex = index.ToInt();
			int endIndex = startIndex + count - 1;
			for (int i = startIndex; i <= endIndex; i++) {
				result.Add(root.FindItemByIndex(i));
			}
			return result;
		}
		public TIndex IndexOf(TItem item) {
			int index = 0;
			foreach (TItem current in this) {
				if (Object.Equals(item, current))
					return IndexConverter.FromInt(index);
				index++;
			}
			return IndexConverter.FromInt(index);
		}
	}
	public class IndexedTreeRangeEnumerable<TItem, TIndex> : IEnumerable<TItem>
		where TIndex : struct, IConvertToInt<TIndex>
		where TItem : class, IHasRelativeIndex<TItem> {
		readonly TIndex from;
		readonly TIndex to;
		readonly IndexedTree<TItem, TIndex> tree;
		public IndexedTreeRangeEnumerable(IndexedTree<TItem, TIndex> tree, TIndex from, TIndex to) {
			this.from = from;
			this.to = to;
			this.tree = tree;
		}
		public IEnumerator<TItem> GetEnumerator() {
			return new IndexedTreeRangeEnumerator<TItem, TIndex>(tree.Root, from, to);
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
	public class IndexedTreeEnumerator<TItem> : IEnumerator<TItem> where TItem : class, IHasRelativeIndex<TItem> {
		IndexedTreeNodeBase<TItem> root;
		List<IndexedTreeNodeMiddleLevel<TItem>> stack;
		List<int> indicies;
		IndexedTreeNodeLeafLevel<TItem> currentLevel;
		int currentIndex;
		TItem current;
		int height;
		readonly int maxCount;
		int count;
		public IndexedTreeEnumerator(IndexedTreeNodeBase<TItem> root) : this(root, Int32.MaxValue) {
		}
		protected IndexedTreeEnumerator(IndexedTreeNodeBase<TItem> root, int maxCount) {
			this.root = root;
			this.maxCount = maxCount;
			Reset();
		}
		public TItem Current { get { return current; } }
		public void Dispose() {
			root = null;
			stack = null;
			indicies = null;
			current = null;
		}
		object System.Collections.IEnumerator.Current { get { return current; } }
		protected void InitCore(int startIndex) {
			stack = new List<IndexedTreeNodeMiddleLevel<TItem>>(32);
			indicies = new List<int>(32);
			IndexedTreeNodeBase<TItem> currentNode = root;
			for (; ; ) {
				IndexedTreeNodeMiddleLevel<TItem> middleLevel = currentNode as IndexedTreeNodeMiddleLevel<TItem>;
				if (middleLevel != null) {
					stack.Add(middleLevel);
					if (startIndex <= 0) {
						indicies.Add(0);
						currentNode = middleLevel.Children[0];
					}
					else {
						int index = middleLevel.ChildrenCount - 1;
						while (index > 0 && middleLevel.Children[index].FirstRelativeIndex > startIndex)
							index--;
						currentNode = middleLevel.Children[index];
						indicies.Add(index);
					}
					startIndex -= currentNode.FirstRelativeIndex;
				}
				else {
					currentLevel = (IndexedTreeNodeLeafLevel<TItem>)currentNode;
					currentIndex = startIndex;
					break;
				}
			}
			height = stack.Count;
		}
		protected virtual void Init() {
			InitCore(-1);
		}
		public bool MoveNext() {
			if (stack == null)
				Init();
			if (count >= maxCount)
				return false;			
			if (currentIndex >= currentLevel.ChildrenCount - 1) {
				int upCount = 0;
				while (stack.Count - upCount > 0) {
					int lastIndex = height - upCount - 1;
					IndexedTreeNodeMiddleLevel<TItem> parent = stack[lastIndex];
					int currentIndexInParent = indicies[lastIndex];
					currentIndexInParent++;
					if (currentIndexInParent < parent.ChildrenCount) {
						indicies[lastIndex] = currentIndexInParent;
						break;
					}
					upCount++;
				}
				if (height - upCount == 0)
					return false;
				while (upCount > 0) {
					int lastIndex = height - upCount - 1;
					IndexedTreeNodeMiddleLevel<TItem> level = stack[lastIndex];
					stack[stack.Count - upCount] = (IndexedTreeNodeMiddleLevel<TItem>)level.Children[indicies[lastIndex]];
					indicies[lastIndex + 1] = 0;
					upCount--;
				}
				currentIndex = -1;
				currentLevel = (IndexedTreeNodeLeafLevel<TItem>)stack[height - 1].Children[indicies[height - 1]];
			}
			currentIndex++;
			current = currentLevel.Items[currentIndex];
			count++;
			return true;
		}
		public void Reset() {
			stack = null;
			indicies = null;
			count = 0;
			currentIndex = -1;
		}
	}
	public class IndexedTreeRangeEnumerator<TItem, TIndex> : IndexedTreeEnumerator<TItem>
		where TIndex : struct, IConvertToInt<TIndex>
		where TItem : class, IHasRelativeIndex<TItem> {
		readonly TIndex from;
		public IndexedTreeRangeEnumerator(IndexedTreeNodeBase<TItem> root, TIndex from, TIndex to)
			: base(root, to.ToInt() - from.ToInt() + 1) {
			this.from = from;
		}
		protected override void Init() {
			int fromIndex = from.ToInt();
			if (fromIndex <= 0)
				InitCore(-1);
			else
				InitCore(fromIndex - 1);
		}
	}
	public abstract class IndexedTreeNodeBase<TItem> where TItem : class, IHasRelativeIndex<TItem> {
		readonly int maxChildrenCount;		
		protected IndexedTreeNodeBase(int maxChildrenCount) {
			this.maxChildrenCount = maxChildrenCount;
		}
		public int MaxChildrenCount {
			[System.Diagnostics.DebuggerStepThrough]
			get { return maxChildrenCount; }
		}
		public int FirstRelativeIndex { get; set; }
		public int RelativeFirstRunIndex { get; set; }
		public int RelativeLastRunIndex { get; set; }
		public int RelativeLogPosition { get; set; }
		public int RunIndexCount { get; set; }
		public IndexedTreeNodeMiddleLevel<TItem> Parent { get; set; }
		public abstract IndexedTreeNodeBase<TItem> Insert(int relativeIndex, TItem item);
		public abstract bool RemoveAt(int relativeIndex);
		public abstract TItem FindItemByIndex(int relativeIndex);
		public abstract TItem First { get; }
		public abstract TItem Last { get; }
		public abstract void RecalcParagraphsPositionsCore(int relativeFrom, int relativeTo, int totalDescendantCount, int deltaLength, int deltaRunIndex);
		public abstract void OnEndMultipleRunSplit();
	}
	public class IndexedTreeNodeMiddleLevel<TItem> : IndexedTreeNodeBase<TItem> where TItem : class, IHasRelativeIndex<TItem> {
		List<IndexedTreeNodeBase<TItem>> children;
		public IndexedTreeNodeMiddleLevel(int maxChildrenCount)
			: base(maxChildrenCount) {
			this.children = new List<IndexedTreeNodeBase<TItem>>(MaxChildrenCount);
		}
		public List<IndexedTreeNodeBase<TItem>> Children {
			[System.Diagnostics.DebuggerStepThrough]
			get { return children; }
		}
		public int ChildrenCount {
			[System.Diagnostics.DebuggerStepThrough]
			get { return children.Count; }
		}
		public override TItem First {
			get {
				Debug.Assert(ChildrenCount > 0);
				return Children[0].First;
			}
		}
		public override TItem Last {
			get {
				Debug.Assert(ChildrenCount > 0);
				return Children[ChildrenCount - 1].Last;
			}
		}
		public override IndexedTreeNodeBase<TItem> Insert(int relativeIndex, TItem item) {
			Debug.Assert(ChildrenCount > 0);
			int childIndex = ChildrenCount - 1;
			while (relativeIndex < Children[childIndex].FirstRelativeIndex) {
				Children[childIndex].FirstRelativeIndex++;
				childIndex--;
			}
			IndexedTreeNodeBase<TItem> newChildNode = Children[childIndex].Insert(relativeIndex - Children[childIndex].FirstRelativeIndex, item);
			if (newChildNode == null)
				return null;
			if (ChildrenCount < MaxChildrenCount) {
				Children.Insert(childIndex + 1, newChildNode);
				newChildNode.Parent = this;
				return null;
			}
			int newChildIndex = childIndex + 1;
			int deltaIndex = Children[MaxChildrenCount / 2].FirstRelativeIndex;
			IndexedTreeNodeMiddleLevel<TItem> rightNode = SplitNode(deltaIndex);
			if (newChildIndex > MaxChildrenCount / 2) {
				newChildNode.FirstRelativeIndex -= deltaIndex;
				rightNode.Children.Insert(newChildIndex - MaxChildrenCount / 2, newChildNode);				
				newChildNode.Parent = rightNode;
				newChildNode.RelativeFirstRunIndex -= rightNode.RelativeFirstRunIndex - RelativeFirstRunIndex;
				newChildNode.RelativeLastRunIndex -= rightNode.RelativeLastRunIndex - RelativeLastRunIndex;
				newChildNode.RelativeLogPosition -= rightNode.RelativeLogPosition - RelativeLogPosition;
			}
			else {
				Children.Insert(newChildIndex, newChildNode);
				newChildNode.Parent = this;
			}
			return rightNode;
		}
		public override bool RemoveAt(int relativeIndex) {
			Debug.Assert(ChildrenCount > 0);
			int childIndex = ChildrenCount - 1;
			while (relativeIndex < Children[childIndex].FirstRelativeIndex) {
				Children[childIndex].FirstRelativeIndex--;
				childIndex--;
			}
			bool shouldRemoveChild = Children[childIndex].RemoveAt(relativeIndex - Children[childIndex].FirstRelativeIndex);
			if (shouldRemoveChild) {				
				Children.RemoveAt(childIndex);
				if (childIndex == 0 && Children.Count > 0) {
					IndexedTreeNodeBase<TItem> child = Children[childIndex];
					int firstRunIndex = child.RelativeFirstRunIndex;
					if (firstRunIndex > 0)
						ShiftRelativeFirstRunIndex(firstRunIndex);
					int firstLogPosition = child.RelativeLogPosition;
					if (firstLogPosition > 0)
						ShiftRelativeLogPosition(firstLogPosition);
				}
			}
			return Children.Count == 0;
		}
		protected IndexedTreeNodeMiddleLevel<TItem> CreateNew() {
			return new IndexedTreeNodeMiddleLevel<TItem>(MaxChildrenCount);
		}
		IndexedTreeNodeMiddleLevel<TItem> SplitNode(int deltaIndex) {
			IndexedTreeNodeMiddleLevel<TItem> newNode = CreateNew();
			newNode.FirstRelativeIndex = deltaIndex + FirstRelativeIndex;
			int deltaFirstRunIndex = Children[MaxChildrenCount / 2].RelativeFirstRunIndex;
			int deltaLastRunIndex = Children[MaxChildrenCount / 2].RelativeLastRunIndex;
			int deltaLogPosition = Children[MaxChildrenCount / 2].RelativeLogPosition;
			newNode.RelativeFirstRunIndex = RelativeFirstRunIndex + deltaFirstRunIndex;
			newNode.RelativeLastRunIndex = RelativeLastRunIndex + deltaLastRunIndex;
			newNode.RelativeLogPosition = RelativeLogPosition+ deltaLogPosition;
			for (int i = MaxChildrenCount / 2; i < MaxChildrenCount; i++) {
				IndexedTreeNodeBase<TItem> child = Children[i];
				newNode.AddChild(child);
				child.FirstRelativeIndex -= deltaIndex;
				child.Parent = newNode;
				child.RelativeFirstRunIndex -= deltaFirstRunIndex;
				child.RelativeLastRunIndex -= deltaLastRunIndex;
				child.RelativeLogPosition -= deltaLogPosition;
			}
			Children.RemoveRange(MaxChildrenCount / 2, MaxChildrenCount / 2);
			return newNode;
		}
		public override TItem FindItemByIndex(int relativeIndex) {
			int i = 1;
			for (; i < children.Count; i++) {
				if (children[i].FirstRelativeIndex > relativeIndex)
					break;
			}
			IndexedTreeNodeBase<TItem> prevChildren = children[i - 1];
			return prevChildren.FindItemByIndex(relativeIndex - prevChildren.FirstRelativeIndex);
		}
		public override string ToString() {
			List<String> str = new List<string>();
			for (int i = 0; i < ChildrenCount; i++) {
				str.Add(Children[i].ToString());
			}
			return String.Format("+{0},{1},{2}:({3})", FirstRelativeIndex, RelativeFirstRunIndex, RelativeLogPosition, String.Join(",", str.ToArray()));
		}
		public void AddChild(IndexedTreeNodeBase<TItem> child) {
			Children.Add(child);
			child.Parent = this;
		}
		public override void RecalcParagraphsPositionsCore(int relativeFrom, int relativeTo, int totalDescendantCount, int deltaLength, int deltaRunIndex) {
			int count = ChildrenCount;
			Debug.Assert(count > 0);
			if (relativeFrom <= 0 && relativeTo >= totalDescendantCount - 1) {
				RelativeFirstRunIndex += deltaRunIndex;
				RelativeLastRunIndex += deltaRunIndex;
				RelativeLogPosition += deltaLength;
#if DEBUGTEST || DEBUG
				if (RelativeFirstRunIndex < 0)
					Exceptions.ThrowInternalException();
#endif
				return;
			}
			relativeFrom = Math.Max(0, relativeFrom);
			relativeTo = Math.Min(relativeTo, totalDescendantCount - 1);
			int nextChildFirstRelativeIndex = totalDescendantCount;
			for(int i = count - 1; i >= 0; nextChildFirstRelativeIndex=Children[i].FirstRelativeIndex, i--) {
				IndexedTreeNodeBase<TItem> child = Children[i];
				if(child.FirstRelativeIndex > relativeTo)
					continue;
				if (nextChildFirstRelativeIndex <= relativeFrom)
					break;
				child.RecalcParagraphsPositionsCore(relativeFrom - child.FirstRelativeIndex, relativeTo - child.FirstRelativeIndex, nextChildFirstRelativeIndex - Children[i].FirstRelativeIndex, deltaLength, deltaRunIndex);
			}		   
		}
		public void ShiftRelativeFirstRunIndex(int deltaRunIndex) {
			RelativeFirstRunIndex += deltaRunIndex;
			int count = children.Count;
			for (int i = 0; i < count; i++)
				children[i].RelativeFirstRunIndex -= deltaRunIndex;
			if (Parent != null && (RelativeFirstRunIndex < 0 || (Parent.Children[0] == this && RelativeFirstRunIndex > 0)))
				Parent.ShiftRelativeFirstRunIndex(RelativeFirstRunIndex);
		}
		public void ShiftRelativeLogPosition(int deltaLogPosition) {
			RelativeLogPosition += deltaLogPosition;
			int count = children.Count;
			for (int i = 0; i < count; i++)
				children[i].RelativeLogPosition -= deltaLogPosition;
			if (Parent != null && (RelativeLogPosition < 0 || (Parent.Children[0] == this && RelativeLogPosition > 0)))
				Parent.ShiftRelativeLogPosition(RelativeLogPosition);
		}
		public override void OnEndMultipleRunSplit() {
			int count = children.Count;
			for (int i = 0; i < count; i++)
				Children[i].OnEndMultipleRunSplit();
			int relativeFirstRunIndex = Children[0].RelativeFirstRunIndex;
			if (relativeFirstRunIndex !=  0)
				ShiftRelativeFirstRunIndex(relativeFirstRunIndex);
			int relativeLogPosition = Children[0].RelativeLogPosition;
			if (relativeLogPosition != 0)
				ShiftRelativeFirstRunIndex(relativeLogPosition);
		}
	}
	public class IndexedTreeNodeLeafLevel<TItem> : IndexedTreeNodeBase<TItem> where TItem : class, IHasRelativeIndex<TItem> {
		List<TItem> items;
		public IndexedTreeNodeLeafLevel(int maxChildrenCount)
			: base(maxChildrenCount) {
			this.items = new List<TItem>(MaxChildrenCount / 2);
		}
		public int ChildrenCount {
			[System.Diagnostics.DebuggerStepThrough]
			get { return items.Count; }
		}
		public override TItem First {
			get {
				if (ChildrenCount <= 0)
					return null;
				else
					return items[0];
			}
		}
		public override TItem Last {
			get {
				int count = items.Count;
				if (count <= 0)
					return null;
				else
					return items[count - 1];
			}
		}
		protected internal List<TItem> Items {
			[System.Diagnostics.DebuggerStepThrough]
			get { return items; }
		}
		public override IndexedTreeNodeBase<TItem> Insert(int relativeIndex, TItem item) {
			int childrenCount = ChildrenCount;
			if (childrenCount < MaxChildrenCount) {
				InsertWithShiftIndex(relativeIndex, item);
				return null;
			}
			int halfCount = MaxChildrenCount / 2;
			IndexedTreeNodeLeafLevel<TItem> rightNode = SplitNode();
			if (relativeIndex > halfCount) {
				int targetIndex = relativeIndex - halfCount;
				rightNode.InsertWithShiftIndex(targetIndex, item);
			}
			else {
				InsertWithShiftIndex(relativeIndex, item);
				rightNode.FirstRelativeIndex++;
			}
			return rightNode;
		}
		protected void InsertWithShiftIndex(int targetIndex, TItem item) {
			Items.Add(null);
			for (int i = ChildrenCount - 2; i >= targetIndex; i--) {
				TItem currentItem = Items[i];
				Items[i + 1] = currentItem;
				currentItem.ShiftRelativeIndex(1);
			}
			Items[targetIndex] = item;
			item.Parent = this;
			item.SetRelativeIndex(targetIndex);
		}
		IndexedTreeNodeLeafLevel<TItem> SplitNode() {
			IndexedTreeNodeLeafLevel<TItem> newNode = CreateNew();			
			int halfCount = MaxChildrenCount / 2;
			int deltaFirstRunIndex = Items[halfCount].GetRelativeFirstRunIndex();
			int deltaLastRunIndex = Items[halfCount].GetRelativeLastRunIndex();
			int deltaLogPosition = Items[halfCount].GetRelativeLogPosition();
			newNode.RelativeFirstRunIndex = RelativeFirstRunIndex + deltaFirstRunIndex;
			newNode.RelativeLastRunIndex = RelativeLastRunIndex + deltaLastRunIndex;
			newNode.RelativeLogPosition = RelativeLogPosition + deltaLogPosition;
			for (int i = halfCount; i < MaxChildrenCount; i++) {
				TItem item = Items[i];
				newNode.Items.Add(item);
				item.SetRelativeIndex(i - halfCount);				
				item.Parent = newNode;
				item.ShiftRelativePosition(-deltaLogPosition, -deltaFirstRunIndex, -deltaLastRunIndex);
			}
			items.RemoveRange(halfCount, halfCount);
			newNode.FirstRelativeIndex = FirstRelativeIndex + halfCount;			
			return newNode;
		}
		protected IndexedTreeNodeLeafLevel<TItem> CreateNew() {
			return new IndexedTreeNodeLeafLevel<TItem>(MaxChildrenCount);
		}
		public override bool RemoveAt(int relativeIndex) {
			int count = items.Count;
			items[relativeIndex].Parent = null;
			if (count == 1) {
				Debug.Assert(relativeIndex == 0);
				return true;
			}
			for (int i = relativeIndex + 1; i < count; i++)
				items[i].ShiftRelativeIndex(-1);			
			items.RemoveAt(relativeIndex);
			if (relativeIndex == 0) {
				TItem firstItem = items[0];
				int firstRunIndex = firstItem.GetRelativeFirstRunIndex();
				if(firstRunIndex > 0)
					ShiftRelativeFirstRunIndex(firstRunIndex);
				int firstLogPosition = firstItem.GetRelativeLogPosition();
				if (firstLogPosition > 0)
					ShiftRelativeLogPosition(firstLogPosition);
			}
			return false;
		}
		public override TItem FindItemByIndex(int relativeIndex) {
#if DEBUGTEST || DEBUG
			if (relativeIndex < 0 || relativeIndex >= ChildrenCount)
				Exceptions.ThrowInternalException();
#endif
			return items[relativeIndex];
		}
		public override string ToString() {
			List<String> str = new List<string>();
			for (int i = 0; i < ChildrenCount; i++) {
				str.Add(String.Format("{0},{1}", items[i].GetRelativeFirstRunIndex(), items[i].GetRelativeLogPosition().ToString()));
			}
			return String.Format("+{0},{1},{2}:({3})", FirstRelativeIndex, RelativeFirstRunIndex, RelativeLogPosition, String.Join(",", str.ToArray()));
		}
		public override void RecalcParagraphsPositionsCore(int from, int to, int totalDescendantCount, int deltaLength, int deltaRunIndex) {
			int count = items.Count;
			if (from <= 0 && to >= totalDescendantCount - 1 && items[0].GetRelativeFirstRunIndex() == 0) {
				RelativeFirstRunIndex += deltaRunIndex;
				if (RelativeFirstRunIndex < 0) {
					ShiftRelativeFirstRunIndex(RelativeFirstRunIndex);
				}
				RelativeLogPosition += deltaLength;
				if (RelativeLogPosition < 0)
					ShiftRelativeLogPosition(RelativeLogPosition);
				RelativeLastRunIndex += deltaRunIndex;
#if DEBUGTEST || DEBUG
				if (RelativeFirstRunIndex < 0)
					Exceptions.ThrowInternalException();
#endif
				return;
			}
			from = Math.Max(from, 0);
			to = Math.Min(to, count - 1);
			for (int i = from; i <= to; i++) {				
				items[i].ShiftRelativePosition(deltaLength, deltaRunIndex, deltaRunIndex);
			}
		}
		public void ShiftRelativeFirstRunIndex(int deltaRunIndex) {
			RelativeFirstRunIndex += deltaRunIndex;
			int count = items.Count;
			for (int i = 0; i < count; i++)
				items[i].ShiftRelativePosition(0, -deltaRunIndex, 0);
			if (Parent != null && (RelativeFirstRunIndex < 0 || (Parent.Children[0] == this && RelativeFirstRunIndex > 0)))
				Parent.ShiftRelativeFirstRunIndex(RelativeFirstRunIndex);
		}
		public void ShiftRelativeLogPosition(int deltaLogPosition) {
			RelativeLogPosition += deltaLogPosition;
			int count = items.Count;
			for (int i = 0; i < count; i++)
				items[i].ShiftRelativePosition(-deltaLogPosition, 0, 0);
			if (Parent != null && (RelativeLogPosition < 0 || (Parent.Children[0] == this && RelativeLogPosition > 0)))
				Parent.ShiftRelativeLogPosition(RelativeLogPosition);
		}
		public override void OnEndMultipleRunSplit() {
			int relativeFirstRunIndex = items[0].GetRelativeFirstRunIndex();
			if (relativeFirstRunIndex != 0)
				ShiftRelativeFirstRunIndex(relativeFirstRunIndex);
			int relativeLogPosition = items[0].GetRelativeLogPosition();
			if (relativeLogPosition != 0)
				ShiftRelativeLogPosition(relativeLogPosition);
		}
		public void EnsureValidRelativeFirstRunIndex(TItem item, int relativeFirstRunIndex) {
			if (relativeFirstRunIndex < 0 || (item == items[0] && relativeFirstRunIndex > 0))
				ShiftRelativeFirstRunIndex(relativeFirstRunIndex);
		}
		public void EnsureValidRelativeLogPosition(TItem item, int relativeLogPosition) {
			if (relativeLogPosition < 0 || item == items[0] && relativeLogPosition > 0)
				ShiftRelativeLogPosition(relativeLogPosition);
		}
	}
	public class ParagraphIndexedTree : IndexedTree<Paragraph, ParagraphIndex> {
		protected internal class InnerListWrapper {
			readonly ParagraphIndexedTree tree;
			public InnerListWrapper(ParagraphIndexedTree tree) {
				this.tree = tree;
			}
			public Paragraph this[int index] { get { return tree[new ParagraphIndex(index)]; } }
			public int Count { get { return tree.Count; } }
		}
		readonly InnerListWrapper innerList;
		public ParagraphIndexedTree() {
			this.innerList = new InnerListWrapper(this);
		}
		protected internal InnerListWrapper InnerList { get { return innerList; } }
		protected internal override void CheckTree(Paragraph ignoreItem) {
			if (First != null && First.DocumentModel.DisableCheckDocumentModelIntegrity)
				return;
			base.CheckTree(ignoreItem);
			foreach (Paragraph current in this) {
				if (Object.ReferenceEquals(current, ignoreItem))
					continue;
				if (current.FirstRunIndex != current.GetFirstRunIndex())
					Exceptions.ThrowInternalException();
				if (current.LastRunIndex != current.GetLastRunIndex())
					Exceptions.ThrowInternalException();
				if (current.LogPosition != current.GetLogPosition())
					Exceptions.ThrowInternalException();
			}
		}
		public ParagraphIndex SearchByLogPosition(DocumentLogPosition logPosition) {			
			if (logPosition < DocumentLogPosition.Zero)
				return new ParagraphIndex(-1);
			int position = ((IConvertToInt<DocumentLogPosition>)logPosition).ToInt();
			IndexedTreeNodeBase<Paragraph> current = Root;
			bool isLastItem = true;
			while (true) {
				IndexedTreeNodeMiddleLevel<Paragraph> currentMiddleLevel = current as IndexedTreeNodeMiddleLevel<Paragraph>;
				if (currentMiddleLevel != null) {
					int i = currentMiddleLevel.ChildrenCount - 1;
					for (; i >= 0; i--) {
						IndexedTreeNodeBase<Paragraph> child = currentMiddleLevel.Children[i];
						if (position >= child.RelativeLogPosition) {
							position -= child.RelativeLogPosition;
							current = child;
							break;
						}
						isLastItem = false;
					}
					Debug.Assert(i >= 0);
				}
				else {
					IndexedTreeNodeLeafLevel<Paragraph> currentLeafLevel = current as IndexedTreeNodeLeafLevel<Paragraph>;
					List<Paragraph> items = currentLeafLevel.Items;
					int i = items.Count - 1;
					for (; i >= 0; i--) {
						Paragraph paragraph = items[i];
						IHasRelativeIndex<Paragraph> relativeIndexParagraph = (IHasRelativeIndex<Paragraph>)paragraph;
						if (position >= relativeIndexParagraph.GetRelativeLogPosition()) {
							if (isLastItem &&  logPosition > paragraph.EndLogPosition)
								return new ParagraphIndex(~Count);
							return paragraph.Index;
						}
						isLastItem = false;
					}
					Debug.Assert(false);
				}
			}
		}
		internal void RecalcParagraphsPositionsCore(ParagraphIndex from, ParagraphIndex to, int deltaLength, int deltaRunIndex) {
			if (to < from)
				return;
#if UseOldIndicies
			foreach (Paragraph paragraph in this) {
				if (paragraph.FirstRunIndex != paragraph.GetFirstRunIndex())
					Exceptions.ThrowInternalException();
				if (paragraph.LastRunIndex != paragraph.GetLastRunIndex())
					Exceptions.ThrowInternalException();
				if (paragraph.LogPosition != paragraph.GetLogPosition())
					Exceptions.ThrowInternalException();
			}
#endif
			Root.RecalcParagraphsPositionsCore(((IConvertToInt<ParagraphIndex>)from).ToInt(), ((IConvertToInt<ParagraphIndex>)to).ToInt(), Count, deltaLength, deltaRunIndex);
#if UseOldIndicies
			ParagraphIndex index = ParagraphIndex.Zero;
			foreach (Paragraph paragraph in this) {				
				if (index > to)
					break;
				if (index >= from) {
					paragraph.LogPosition += deltaLength;
					paragraph.ShiftRunIndex(deltaRunIndex);
					if (paragraph.FirstRunIndex != paragraph.GetFirstRunIndex())
						Exceptions.ThrowInternalException();
					if (paragraph.LastRunIndex != paragraph.GetLastRunIndex())
						Exceptions.ThrowInternalException();
					if (paragraph.LogPosition != paragraph.GetLogPosition())
						Exceptions.ThrowInternalException();
				}
				index++;
			}
#endif
		}
	}
}
