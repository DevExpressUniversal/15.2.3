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
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting {
	public class ObjectEnumeratorBase : IEnumerator {
		private IList objs;
		private IEnumerator en;
		public ObjectEnumeratorBase(IList objs) {
			this.objs = objs;
			en = objs.GetEnumerator();
		}
		public object Current { get { return en.Current; }
		}
		public virtual bool MoveNext() {
			return en.MoveNext();
		}
		public virtual void Reset() {
			en.Reset();
		}
	}
	public abstract class NestedObjectEnumeratorBase : IEnumerator {
		#region inner class
		protected class EnumStack : StackBase {
			public SimpleEnumerator Enumerator { get { return (IsEmpty == false) ? (SimpleEnumerator)Peek() : null; } }
			public SimpleEnumerator this[int index] { get { return (SimpleEnumerator)list[index]; } }
			public EnumStack() {
			}
		}
		#endregion
		protected EnumStack stack;
		private IEnumerator enumerator;
		protected NestedObjectEnumeratorBase(IEnumerator enumerator) {
			this.enumerator = enumerator;
			stack = new EnumStack();
		}
		object IEnumerator.Current { get { return stack.Enumerator.Current; }
		}
		public virtual bool MoveNext() {
			if(stack.IsEmpty) {
				stack.Push( GetEnumerator(this.enumerator) );
				return stack.Enumerator.MoveNext();
			} 
			IEnumerator enumerator = GetNestedObjects();
			if(enumerator.MoveNext()) {
				enumerator.Reset();
				stack.Push( GetEnumerator(enumerator) );
				return stack.Enumerator.MoveNext();
			}
			while(stack.Enumerator.MoveNext() == false) {
				stack.Pop();
				if(stack.IsEmpty) return false; 
			}
			return true;
		}
		protected abstract IEnumerator GetNestedObjects();
		protected virtual IEnumerator GetEnumerator(IEnumerator source) {
			return new SimpleEnumerator(source);
		}
		public virtual void Reset() {
			stack.Clear();
		}
		public int[] GetStackIndices() {
			int[] indices = new int[stack.Count - 1];
			for(int i = 1; i < stack.Count; i++) {
				indices[i - 1] = stack[i].CurrentIndex;
			}
			return indices;
		}
	}
	public class SimpleEnumerator : IEnumerator {
		private IEnumerator en;
		int index = -1;
		public object Current { get { return (en != null) ? en.Current : null; } }
		internal int CurrentIndex { get { return index; } }
		public SimpleEnumerator(IEnumerator en) {
			this.en = en;
		}
		public virtual bool MoveNext() {
			index++;
			return en.MoveNext();
		}
		public virtual void Reset() {
			en.Reset();
			index = -1;
		}
	}
	public class BrickBaseEnumerator : IEnumerator 
	{
		#region inner class
		private class CompositeBrickEnumerator : SimpleEnumerator 
		{
			private BrickBase brickContainer;
			public new BrickBase Current { get { return ((IEnumerator)this).Current as BrickBase; } }
			public CompositeBrickEnumerator(BrickBase brickContainer, IEnumerator en)
				: base(en) {
				this.brickContainer = brickContainer;
			}
			public RectangleF GetBrickRectangle() {
				return Current != null ?
					GetBrickRectangle(Current, brickContainer.InnerBrickListOffset) : 
					RectangleF.Empty;
			}
			static RectangleF GetBrickRectangle(BrickBase brick, PointF offset) {
				return RectF.Offset(brick.GetViewRectangle(), offset.X, offset.Y);
			}
		}
		private class EnumStack : StackBase 
		{
			public CompositeBrickEnumerator Top { get { return  (CompositeBrickEnumerator)Peek(); }
			}
			public CompositeBrickEnumerator this[int index] { get { return (CompositeBrickEnumerator)list[index]; }
			}
			public EnumStack() {
			}
		}
		#endregion
		private IEnumerator en;
		private EnumStack stack;
		public virtual object Current { get { return CurrentBrick; } }
		public BrickBase CurrentBrick { get { return stack.Top.Current; } }
		internal BrickBaseEnumerator(BrickBase brick) {
			en = CreateEnumerator(brick);
			stack = new EnumStack();
		}
		private CompositeBrickEnumerator CreateEnumerator(BrickBase brick) {
			return new CompositeBrickEnumerator(brick, GetEnumerator(brick.InnerBrickList));
		}
		protected virtual IEnumerator GetEnumerator(IList bricks) {
			return bricks.GetEnumerator();
		}
		public virtual bool MoveNext() {
			if(stack.IsEmpty) {
				stack.Push(en);
				return stack.Top.MoveNext();
			} 
			if(CurrentBrick is CompositeBrick) {
				stack.Push( CreateEnumerator((CompositeBrick)CurrentBrick) );
				return stack.Top.MoveNext();
			}
			while(stack.Top.MoveNext() == false) {
				stack.Pop();
				if(stack.IsEmpty) return false; 
			}
			return true;
		}
		public virtual void Reset() {
			stack.Clear();
		}
		public RectangleF GetCurrentBrickBounds() {
			RectangleF bounds = RectangleF.Empty;
			for(int i = 0; i < stack.Count; i++) {
				CompositeBrickEnumerator item = stack[i];
				RectangleF rect = item.GetBrickRectangle();
				bounds = RectF.Offset(rect, bounds.X, bounds.Y);
			}
			return bounds;
		}
		public int[] GetStackIndices() {
			int[] indices = new int[stack.Count];
			for(int i = 0; i < stack.Count; i++) {
				indices[i] = stack[i].CurrentIndex;
			}
			return indices;
		}
	}
	public class NestedEnumerator : IEnumerator {
		IEnumerable enumerable;
		Stack<IEnumerable> enumerableStack;
		protected Stack<IEnumerator> enumeratorStack;
		public object Current {
			get { return CurrentEnumerator.Current; } 
		}
		IEnumerator CurrentEnumerator { 
			get { return enumeratorStack.Peek(); } 
		}
		IEnumerable CurrentEnumerable { 
			get { return Current as IEnumerable; }
		}
		public NestedEnumerator(IEnumerable enumerable) {
			this.enumerable = enumerable;
			enumerableStack = new Stack<IEnumerable>();
			enumeratorStack = new Stack<IEnumerator>();
		}
		public virtual bool MoveNext() {
			if(enumerableStack.Count == 0) {
				Push(enumerable);
				return CurrentEnumerator.MoveNext();
			}
			if(CurrentEnumerable != null) {
				Push(CurrentEnumerable);
				if(!CurrentEnumerator.MoveNext()) {				
					Pop();
					return enumerableStack.Count > 0;
				}
			}
			return true;
		}
		void Push(IEnumerable enumerable) {
			enumerableStack.Push(enumerable);
			enumeratorStack.Push(enumerable.GetEnumerator());
		}
		void Pop() {
			if(enumeratorStack.Count > 0) {
				enumerableStack.Pop();
				enumeratorStack.Pop();
			}
			if(enumeratorStack.Count == 0)
				return;
			if(!CurrentEnumerator.MoveNext())
				Pop();
		}
		public void Reset() {
			enumerableStack.Clear();
			enumeratorStack.Clear();
		}
	}
	public class PageBrickEnumerator : BrickBaseEnumerator {
		public new Brick CurrentBrick { get { return (Brick)Current; } }
		public PageBrickEnumerator(BrickBase brick)
			: base(brick) {
		}
		public override bool MoveNext() {
			while(base.MoveNext()) {
				if(base.CurrentBrick is Brick)
					return true;
			}
			return false;
		}
	}
	public class BrickEnumerator : PageBrickEnumerator {
		public override object Current { get { return ((Brick)base.Current).GetRealBrick(); } }
		public BrickEnumerator(BrickBase brick) : base(brick) {
		}
	}
}
namespace DevExpress.XtraPrinting.Native.Enumerators {
	public class IndexedEnumerator : IIndexedEnumerator {
		protected IList items;
		public int RealIndex { get; protected set; }
		public object Current { get; protected set; }
		public IndexedEnumerator(IList items) {
			this.items = items;
			ResetCore();
		}
		public virtual bool MoveNext() {
			if(RealIndex < items.Count - 1) {
				RealIndex++;
				UpdateCurrent();
				return true;
			}
			return false;
		}
		protected void UpdateCurrent() {
			Current = items[RealIndex];
		}
		public void Reset() {
			ResetCore();
		}
		protected virtual void ResetCore() {
			RealIndex = -1;
			Current = null;
		}
	}
	public class ReversedEnumerator : IndexedEnumerator {
		public ReversedEnumerator(IList items)
			: base(items) {
		}
		public override bool MoveNext() {
			if(RealIndex > 0) {
				RealIndex--;
				UpdateCurrent();
				return true;
			}
			return false;
		}
		protected override void ResetCore() {
			base.ResetCore();
			RealIndex = items.Count;
		}
	}
	public class MappedIndexedEnumerator : IndexedEnumerator {
		protected IList<MapItem> map;
		protected int mapIndex;
		public RectangleF ClipBounds { get; set; }
		public PointF ViewOrigin { get; set; }
		public MappedIndexedEnumerator(IList<MapItem> map, IList items)
			: base(items) {
			this.map = map;
			ResetMap();
		}
		public override bool MoveNext() {
			if(mapIndex >= 0 && mapIndex < map.Count && RealIndex < map[mapIndex].Index2) {
				RealIndex++;
				UpdateCurrent();
				return true;
			}
			return MoveNextMap();
		}
		protected virtual bool MoveNextMap() {
			while(++mapIndex < map.Count) {
				RectangleF rect = map[mapIndex].Bounds;
				rect.Offset(ViewOrigin);
				RealIndex = map[mapIndex].Index1;
				if(ClipBounds.IntersectsWith(rect)) {
					UpdateCurrent();
					return true;
				}
			}
			Current = null;
			return false;
		}
		protected override void ResetCore() {
			base.ResetCore();
			if(map != null) ResetMap();
		}
		protected virtual void ResetMap() {
			mapIndex = -1;
		}
	}
	public class ReversedMappedEnumerator : MappedIndexedEnumerator {
		public ReversedMappedEnumerator(IList<MapItem> map, IList items)
			: base(map, items) {
		}
		public override bool MoveNext() {
			if(mapIndex >= 0 && mapIndex < map.Count && RealIndex > map[mapIndex].Index1) {
				RealIndex--;
				UpdateCurrent();
				return true;
			}
			return MoveNextMap();
		}
		protected override bool MoveNextMap() {
			while(--mapIndex >= 0) {
				RectangleF rect = map[mapIndex].Bounds;
				rect.Offset(ViewOrigin);
				RealIndex = map[mapIndex].Index2;
				if(ClipBounds.IntersectsWith(rect)) {
					UpdateCurrent();
					return true;
				}
			}
			Current = null;
			return false;
		}
		protected override void ResetMap() {
			mapIndex = map.Count;
		}
	}
}
namespace DevExpress.XtraPrinting.Native {
	using DevExpress.XtraPrinting.Native.Enumerators;
	public abstract class MapBuilderBase<T>
		where T : IMapNode<T> {
		public T BuildMap(BrickBase brick) {
			SizeF size = brick is Page ? ((Page)brick).PageSize : brick.Size;
			T brickNode = CreateNode(new RectangleF(Point.Empty, size), brick, "", new RectangleF(Point.Empty, size));
			CollectMapNodes(brick, brickNode, brick.Rect);
			EnumerateBricks();
			return brickNode;
		}
		protected abstract T CreateNode(RectangleF rect, BrickBase brick, string indexes, RectangleF absoluteRect);
		protected virtual void EnumerateBricks() { }
		void CollectMapNodes(BrickBase brick, T brickNode, RectangleF absoluteRect) {
			int i = 0;
			foreach(BrickBase item in brick.InnerBrickList) {
				RectangleF rect = RectF.Offset(item.Rect, brick.InnerBrickListOffset.X, brick.InnerBrickListOffset.Y);
				T itemNode = CreateNode(rect, item, brickNode.Indexes.Length != 0 ? brickNode.Indexes + "," + i : i.ToString(), absoluteRect);
				i++;
				brickNode.Nodes.Add(itemNode);
				CollectMapNodes(item, itemNode, RectF.Offset(item.Rect, absoluteRect.X, absoluteRect.Y));
			}
		}
	}
	public interface IMapNode {
		RectangleF Bounds { get; }
		Dictionary<string, object> Content { get; }
		string Indexes { get; }
	}
	public interface IMapNode<T> : IMapNode where T : IMapNode {
		IList<T> Nodes { get; }
	}
	public class MapItem {
		public RectangleF Bounds { get; private set; }
		public int Index1 { get; private set; }
		public int Index2 { get; private set; }
		public MapItem(RectangleF bounds, int index1, int index2) {
			Bounds = bounds;
			Index1 = index1;
			Index2 = index2;
		}
	}
	public interface IIndexedEnumerator : IEnumerator {
		int RealIndex { get; }
	}
	public class ReversedBrickEnumerator : PageBrickEnumerator {
		public ReversedBrickEnumerator(CompositeBrick brick) : base(brick) {
		}
		protected override IEnumerator GetEnumerator(IList bricks) {
			return new ReversedEnumerator(bricks);
		}
	}
	public abstract class PageElementsEnumerator : IEnumerator {
		IEnumerator pageEnumerator;
		IEnumerator elementsEnumerator;
		public Page Page { get { return (Page)pageEnumerator.Current; } }
		public object Current { get { return elementsEnumerator.Current; } }
		protected PageElementsEnumerator(PageList pages) {
			pageEnumerator = pages.GetEnumerator();
			MoveNextPage();
		}
		public bool MoveNext() {
			if(elementsEnumerator != null && elementsEnumerator.MoveNext())
				return true;
			while(MoveNextPage())
				if(elementsEnumerator.MoveNext())
					return true;
			return false;
		}
		public void Reset() {
			pageEnumerator.Reset();
			MoveNextPage();
		}
		bool MoveNextPage() {
			if(pageEnumerator.MoveNext()) {
				elementsEnumerator = GetPageElementsEnumerator();
				return true;
			}
			return false;
		}
		protected abstract IEnumerator GetPageElementsEnumerator();
	}
	public class DocumentBrickEnumerator : PageElementsEnumerator, IEnumerable
	{
		public Brick Brick { get { return (Brick)Current; }
		}
		public DocumentBrickEnumerator(Document doc)
			: base(doc.Pages) {
		}
		protected override IEnumerator GetPageElementsEnumerator() {
			return new BrickEnumerator(Page);
		}
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return this;
		}
		#endregion
	}
	public class NestedBricksEnumerator : NestedObjectEnumeratorBase {
		public Brick Current { get { return (Brick)((IEnumerator)this).Current; } }
		public NestedBricksEnumerator(Brick brick)
			: base(new object[] { brick }.GetEnumerator()) {
		}
		protected override IEnumerator GetNestedObjects() {
			return Current.Bricks.GetEnumerator();
		}
	}
	public class BrickIterator : IEnumerator {
		IIndexedEnumerator bricks;
		public int Index { get { return bricks.RealIndex; } }
		public PointF Offset { get; private set; }
		public BrickBase CurrentBrick {
			get { return (BrickBase)bricks.Current; }
		}
		public RectangleF CurrentBrickRectangle {
			get {
				RectangleF brickRect = CurrentBrick.Rect;
				brickRect.Offset(Offset);
				return brickRect;
			}
		}
		public RectangleF CurrentClipRectangle {
			get;
			private set;
		}
		public BrickIterator(IList bricks, PointF offset, RectangleF clipRect) : this(new IndexedEnumerator(bricks), offset, clipRect) {
		}
		public BrickIterator(IIndexedEnumerator bricks, PointF offset, RectangleF clipRect) {
			this.bricks = bricks;
			Offset = offset;
			CurrentClipRectangle = clipRect;
			Reset();
		}
		#region IEnumerator Members
		public object Current {
			get { return bricks.Current; }
		}
		public bool MoveNext() {
			return bricks.MoveNext();
		}
		public void Reset() {
			bricks.Reset();
		}
		#endregion
	}
	public class NestedBrickIterator : IEnumerator {
		List<BrickIterator> iterators = new List<BrickIterator>();
		Func<BrickBase, PointF, IIndexedEnumerator> callback;
		IIndexedEnumerator enumerator;
		public PointF Offset { get; set; }
		public RectangleF ClipRect { get; set; }
		public NestedBrickIterator(IList bricks)
			: this(new IndexedEnumerator(bricks), (brick, offset) => new IndexedEnumerator(brick.InnerBrickList)) {
		}
		public NestedBrickIterator(IIndexedEnumerator enumerator, Func<BrickBase, PointF, IIndexedEnumerator> callback) {
			this.enumerator = enumerator;
			this.callback = callback;
			Offset = PointF.Empty;
			ClipRect = RectangleF.Empty;
		}
		public BrickBase CurrentBrick {
			get { return CurrentIterator.CurrentBrick; }
		}
		public BrickBase CurrentBrickOwner {
			get {
				return iterators.Count > 1 ? iterators[iterators.Count - 2].CurrentBrick : null;
			}
		}
		public RectangleF CurrentBrickRectangle {
			get {
				return CurrentIterator.CurrentBrickRectangle;
			}
		}
		public RectangleF CurrentClipRectangle {
			get {
				return CurrentIterator.CurrentClipRectangle;
			}
		}
		BrickIterator CurrentIterator {
			get { return iterators[iterators.Count - 1]; }
		}
		object IEnumerator.Current {
			get { return CurrentIterator.CurrentBrick; }
		}
		public bool MoveNext() {
			if(iterators.Count == 0) {
				iterators.Add(new BrickIterator(enumerator, Offset, ClipRect));
				return MoveNext();
			}
			if(!CurrentIterator.MoveNext()) {
				if(iterators.Count == 0)
					return false;
				iterators.RemoveAt(iterators.Count - 1);
				return iterators.Count != 0;
			}
			BrickBase currentBrick = CurrentBrick;
			PointF newOffset = new PointF(CurrentIterator.Offset.X + currentBrick.X, CurrentIterator.Offset.Y + currentBrick.Y);
			RectangleF clipRect = CurrentIterator.CurrentBrickRectangle;
			if(!CurrentIterator.CurrentClipRectangle.IsEmpty)
				clipRect.Intersect(CurrentIterator.CurrentClipRectangle);
			BrickIterator brickIterator = new BrickIterator(callback(currentBrick, newOffset),
				new PointF(newOffset.X + currentBrick.InnerBrickListOffset.X, newOffset.Y + currentBrick.InnerBrickListOffset.Y), 
				clipRect);
			iterators.Add(brickIterator);
			return MoveNext();
		}
		public void Reset() {
			enumerator.Reset();
			iterators.Clear();
		}
		public int[] GetCurrentBrickIndices() {
			int[] result = new int[iterators.Count];
			for(int i = 0; i < iterators.Count; i++)
				result[i] = iterators[i].Index;
			return result;
		}
	}
}
