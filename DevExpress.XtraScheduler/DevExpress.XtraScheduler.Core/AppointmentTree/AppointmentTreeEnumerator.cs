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
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentTreeNodeEnumerableFactoryBase
	public interface IAppointmentTreeNodeEnumeratorFactory {
		AppointmentTreeNodeEnumeratorBase CreateTreeEnumerator(AppointmentTree tree);
		AppointmentTreeNodeEnumeratorBase CreateCompositeNodeEnumerator(AppointmentTreeCompositeNode node);
	}
	public class AppointmentTreeNodeEnumerableFactory : IAppointmentTreeNodeEnumeratorFactory {
		public AppointmentTreeNodeEnumeratorBase CreateTreeEnumerator(AppointmentTree tree) {
			return new AppointmentTreeEnumerator(tree);
		}
		public AppointmentTreeNodeEnumeratorBase CreateCompositeNodeEnumerator(AppointmentTreeCompositeNode node) {
			return new AppointmentTreeCompositeNodeEnumerator(node);
		}
	}
	public class AppointmentTreeNodeNullEnumerableFactory : IAppointmentTreeNodeEnumeratorFactory {
		public AppointmentTreeNodeEnumeratorBase CreateTreeEnumerator(AppointmentTree tree) {
			return AppointmentTreeNodeNullEnumerator.Instance;
		}
		public AppointmentTreeNodeEnumeratorBase CreateCompositeNodeEnumerator(AppointmentTreeCompositeNode node) {
			return AppointmentTreeNodeNullEnumerator.Instance;
		}
	}
	public class PrevAppointmentTreeNodeEnumerableFactory : IAppointmentTreeNodeEnumeratorFactory {
		public AppointmentTreeNodeEnumeratorBase CreateTreeEnumerator(AppointmentTree tree) {
			return new AppointmentPrevTreeEnumerator(tree);
		}
		public AppointmentTreeNodeEnumeratorBase CreateCompositeNodeEnumerator(AppointmentTreeCompositeNode node) {
			return new AppointmentPrevTreeCompositeNodeEnumerator(node);
		}
	}
	public class NextAppointmentTreeNodeEnumerableFactory : IAppointmentTreeNodeEnumeratorFactory {
		public AppointmentTreeNodeEnumeratorBase CreateTreeEnumerator(AppointmentTree tree) {
			return new AppointmentNextTreeEnumerator(tree);
		}
		public AppointmentTreeNodeEnumeratorBase CreateCompositeNodeEnumerator(AppointmentTreeCompositeNode node) {
			return new AppointmentNextTreeCompositeNodeEnumerator(node);
		}
	}
	#endregion
	#region AppointmentTreeNodeEnumeratorBase
	public abstract class AppointmentTreeNodeEnumeratorBase {
		public abstract AppointmentTreeNode Current { get; }
		public abstract void Enumerate(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection);
		protected void VisitCurrentNode(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
			if (Current != null) {
				Current.Accept(visitor, parentIntersection);
			}
		}
	}
	#endregion
	#region AppointmentTreeNodeNullEnumerator
	public class AppointmentTreeNodeNullEnumerator : AppointmentTreeNodeEnumeratorBase {
		#region static
		static AppointmentTreeNodeNullEnumerator instance;
		static AppointmentTreeNodeNullEnumerator() {
			instance = new AppointmentTreeNodeNullEnumerator();
		}
		public static AppointmentTreeNodeNullEnumerator Instance { get { return instance; } }
		#endregion
		public AppointmentTreeNodeNullEnumerator() {
		}
		public override void Enumerate(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
		}
		public override AppointmentTreeNode Current { get { return null; } }
	}
	#endregion
	#region AppointmentTreeNodeEnumerator
	public abstract class AppointmentTreeNodeEnumerator : AppointmentTreeNodeEnumeratorBase {
		readonly AppointmentTreeNode node;
		int searchRangeStartIndex = -1;
		int searchRangeEndIndex = -1;
		protected AppointmentTreeNodeEnumerator(AppointmentTreeNode node) {
			Guard.ArgumentNotNull(node, "node");
			this.node = node;
		}
		protected AppointmentTreeNode Node { get { return node; } }
		protected int SearchRangeStartIndex { get { return searchRangeStartIndex; } set { searchRangeStartIndex = value; } }
		protected int SearchRangeEndIndex { get { return searchRangeEndIndex; } set { searchRangeEndIndex = value; } }
		public override void Enumerate(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
			ResetSearchRange();
			AppointmentTreeNodeIntersectionType nodeIntersection = CalculateCurrentNodeIntersection(parentIntersection, visitor.SearchInterval);
			TimeInterval actualTimeInterval = AppointmentTreeHelper.CalculateActualSearchInterval(visitor.SearchInterval);
			InitSearchRange(actualTimeInterval, nodeIntersection);
			EnumerateChildren(visitor, nodeIntersection);
		}
		protected virtual AppointmentTreeNodeIntersectionType CalculateCurrentNodeIntersection(AppointmentTreeNodeIntersectionType parentIntersection, TimeInterval searchInterval) {
			AppointmentTreeNodeIntersectionType result = AppointmentTreeNodeIntersectionType.Full;
			if (parentIntersection != AppointmentTreeNodeIntersectionType.Full) {
				result = AppointmentTreeHelper.CalculateNodeIntersectionType(searchInterval, Node);
			}
			return result;
		}
		protected virtual bool CanVisitNode(AppointmentTreeNodeIntersectionType intersection) {
			return !AppointmentTreeHelper.IsNoneIntersection(intersection);
		}
		protected abstract void EnumerateChildren(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType intersection);
		protected abstract void ResetSearchRange();
		protected abstract void InitSearchRange(TimeInterval interval, AppointmentTreeNodeIntersectionType intersectionType);
	}
	#endregion
	#region AppointmentTreeCompositeNodeEnumerator
	public class AppointmentTreeCompositeNodeEnumerator : AppointmentTreeNodeEnumerator {
		int currentIndex = -1;
		public AppointmentTreeCompositeNodeEnumerator(AppointmentTreeCompositeNode node)
			: base(node) {
		}
		protected AppointmentTreeCompositeNode CompositeNode { get { return (AppointmentTreeCompositeNode)Node; } }
		protected int CurrentIndex { get { return currentIndex; } }
		protected int ChildCount { get { return CompositeNode.Children.Length; } }
		protected void SetCurrentIndex(int index) {
			this.currentIndex = index;
		}
		public AppointmentTreeNode GetChild(int index) {
			return CompositeNode.Children[index];
		}
		public override AppointmentTreeNode Current {
			get {
				return CompositeNode.Children[CurrentIndex];
			}
		}
		internal bool MoveForward(int startIndex, ref int currentIndex) {
			currentIndex = startIndex;
			for (; ; ) {
				currentIndex++;
				if (IsEndOfRange(currentIndex) || GetChild(currentIndex) != null)
					break;
			}
			return !IsEndOfRange(currentIndex);
		}
		internal bool MoveBackward(int startIndex, ref int currentIndex) {
			currentIndex = startIndex;
			for (; ; ) {
				currentIndex--;
				if (IsStartOfRange(currentIndex) || GetChild(currentIndex) != null)
					break;
			}
			return !IsStartOfRange(currentIndex);
		}
		protected bool IsStartOfRange(int index) {
			return index <= 0 || index <= SearchRangeStartIndex;
		}
		protected bool IsEndOfRange(int index) {
			return index >= ChildCount || index > SearchRangeEndIndex;
		}
		protected override void ResetSearchRange() {
			this.currentIndex = -1;
			SearchRangeStartIndex = 0;
			SearchRangeEndIndex = ChildCount - 1;
		}
		protected override void InitSearchRange(TimeInterval interval, AppointmentTreeNodeIntersectionType intersectionType) {
			if (intersectionType == AppointmentTreeNodeIntersectionType.Partial) {
				SearchRangeStartIndex = Node.CalculateSearchRangeStartIndex(interval.Start);
				SearchRangeEndIndex = Node.CalculateSearchRangeEndIndex(interval.End);
			}
		}
		protected override void EnumerateChildren(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
			if (CanFastEnumerate(parentIntersection))
				FastEnumerateAllNodes(visitor);
			else
				EnumerateInSearchMode(visitor, parentIntersection);
		}
		protected virtual bool CanFastEnumerate(AppointmentTreeNodeIntersectionType parentIntersection) {
			return parentIntersection == AppointmentTreeNodeIntersectionType.Full;
		}
		protected virtual void EnumerateInSearchMode(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
			int startIndex = CalculateFirstChildNodeIndex();
			if (startIndex < 0)
				return;
			this.currentIndex = startIndex;
			AppointmentTreeNodeIntersectionType intersection = AppointmentTreeHelper.CalculateNodeIntersectionType(visitor.SearchInterval, Current);
			if (!CanVisitNode(intersection))
				return;
			VisitCurrentNode(visitor, intersection);
			int endIndex = CalculateLastChildNodeIndex();
			if (endIndex < 0 || endIndex > SearchRangeEndIndex || startIndex == endIndex)
				return;
			bool hasMiddle = endIndex - startIndex > 1;
			if (hasMiddle)
				EnumerateNodeRangeExcludeBounds(visitor, startIndex, endIndex);
			this.currentIndex = endIndex;
			intersection = AppointmentTreeHelper.CalculateNodeIntersectionType(visitor.SearchInterval, Current);
			if (!CanVisitNode(intersection))
				return;
			VisitCurrentNode(visitor, intersection);
		}
		protected int CalculateLastChildNodeIndex() {
			if (GetChild(SearchRangeEndIndex) != null)
				return SearchRangeEndIndex;
			int index = -1;
			return MoveBackward(SearchRangeEndIndex, ref index) ? index : -1;
		}
		protected int CalculateFirstChildNodeIndex() {
			if (GetChild(SearchRangeStartIndex) != null)
				return SearchRangeStartIndex;
			int index = -1;
			return MoveForward(SearchRangeStartIndex, ref index) ? index : -1;
		}
		protected virtual void FastEnumerateAllNodes(IAppointmentTreeNodeVisitor visitor) {
			int index = -1;
			int startIndex = SearchRangeStartIndex - 1;
			while (MoveForward(startIndex, ref index)) {
				startIndex = index;
				this.currentIndex = index;
				VisitCurrentNode(visitor, AppointmentTreeNodeIntersectionType.Full);
			}
		}
		protected void EnumerateNodeRangeExcludeBounds(IAppointmentTreeNodeVisitor visitor, int startIndex, int endIndex) {
			int index = -1;
			while (MoveForward(startIndex, ref index)) {
				if (index >= endIndex)
					break;
				startIndex = index;
				this.currentIndex = index;
				VisitCurrentNode(visitor, AppointmentTreeNodeIntersectionType.Full);
			}
		}
	}
	#endregion
	#region AppointmentPrevNextTreeCompositeNodeEnumerator
	public class AppointmentPrevNextTreeCompositeNodeEnumerator : AppointmentTreeNodeEnumerator {
		int currentIndex = -1;
		public AppointmentPrevNextTreeCompositeNodeEnumerator(AppointmentTreeNode node)
			: base(node) {
		}
		protected AppointmentTreeCompositeNode CompositeNode { get { return (AppointmentTreeCompositeNode)Node; } }
		protected int CurrentIndex { get { return currentIndex; } set { currentIndex = value; } }
		protected int ChildCount { get { return CompositeNode.Children.Length; } }
		public override AppointmentTreeNode Current { get { return CompositeNode.Children[CurrentIndex]; } }
		protected void SetCurrentIndex(int index) {
			this.currentIndex = index;
		}
		public AppointmentTreeNode GetChild(int index) {
			return CompositeNode.Children[index];
		}
		protected bool IsStartOfRange(int index) {
			return (index <= 0 || index <= SearchRangeStartIndex) && GetChild(index) == null;
		}
		protected bool IsEndOfRange(int index) {
			return index >= ChildCount || index > SearchRangeEndIndex && GetChild(index) == null;
		}
		protected override void ResetSearchRange() {
			this.currentIndex = -1;
			SearchRangeStartIndex = 0;
		}
		protected override void InitSearchRange(TimeInterval interval, AppointmentTreeNodeIntersectionType intersectionType) {
			SearchRangeStartIndex = Node.CalculateSearchRangeStartIndex(interval.Start);
			SearchRangeEndIndex = Node.CalculateSearchRangeEndIndex(interval.End);
		}
		protected bool EnumerateChildrenBase(IAppointmentTreeNodeVisitor visitor, int index) {
			CurrentIndex = index;
			VisitCurrentNode(visitor, AppointmentTreeNodeIntersectionType.Full);
			PrevNextAppointmentTreeNodeVisitor currentVisitor = visitor as PrevNextAppointmentTreeNodeVisitor;
			if (currentVisitor == null)
				return false;
			foreach (object key in currentVisitor.Result.Keys) {
				if (currentVisitor.Result[key] == null)
					return false;
			}
			return true;
		}
		protected override void EnumerateChildren(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) { }
	}
	#endregion
	#region AppointmentPrevTreeCompositeNodeEnumerator
	public class AppointmentPrevTreeCompositeNodeEnumerator : AppointmentPrevNextTreeCompositeNodeEnumerator {
		public AppointmentPrevTreeCompositeNodeEnumerator(AppointmentTreeNode node)
			: base(node) {
		}
		protected override void EnumerateChildren(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
			for (int i = SearchRangeEndIndex; i >= SearchRangeStartIndex; i--) {
				if (EnumerateChildrenBase(visitor, i))
					return;
			}
		}
	}
	#endregion
	#region AppointmentNextTreeCompositeNodeEnumerator
	public class AppointmentNextTreeCompositeNodeEnumerator : AppointmentPrevNextTreeCompositeNodeEnumerator {
		public AppointmentNextTreeCompositeNodeEnumerator(AppointmentTreeNode node)
			: base(node) {
		}
		protected override void EnumerateChildren(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
			for (int i = SearchRangeStartIndex; i <= SearchRangeEndIndex; i++) {
				if (EnumerateChildrenBase(visitor, i))
					return;
			}
		}
	}
	#endregion
	#region AppointmentTreeEnumerator
	public class AppointmentTreeEnumerator : AppointmentTreeNodeEnumerator {
		int currentKey = -1;
		public AppointmentTreeEnumerator(AppointmentTree tree)
			: base(tree) {
		}
		protected AppointmentTree Tree { get { return (AppointmentTree)Node; } }
		protected int CurrentKey { get { return currentKey; } }
		public override AppointmentTreeNode Current {
			get {
				return Tree.Children[CurrentKey];
			}
		}
		protected override void ResetSearchRange() {
			currentKey = -1;
			SearchRangeStartIndex = Int32.MinValue;
			SearchRangeEndIndex = Int32.MaxValue;
		}
		protected override void InitSearchRange(TimeInterval interval, AppointmentTreeNodeIntersectionType intersectionType) {
			SearchRangeStartIndex = Tree.CalculateSearchRangeStartIndex(interval.Start);
			SearchRangeEndIndex = Tree.CalculateSearchRangeEndIndex(interval.End);
		}
		protected bool EnumerateCurrentChildren(IAppointmentTreeNodeVisitor visitor, int key) {
			this.currentKey = key;
			if (currentKey < SearchRangeStartIndex || currentKey > SearchRangeEndIndex)
				return true;
			XtraSchedulerDebug.Assert(SearchRangeStartIndex <= currentKey && currentKey <= SearchRangeEndIndex);
			XtraSchedulerDebug.Assert(Current != null);
			AppointmentTreeNodeIntersectionType intersection = AppointmentTreeHelper.CalculateNodeIntersectionType(visitor.SearchInterval, Current);
			if (intersection == AppointmentTreeNodeIntersectionType.BeyondAtRight) 
				return false;
			if (CanVisitNode(intersection))
				Current.Accept(visitor, intersection);
			return true;
		}
		protected override void EnumerateChildren(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
			int[] keys = Tree.GetSortedChildrenKeys();
			int count = keys.Length;
			for (int i = 0; i < count; i++) {
				if (!EnumerateCurrentChildren(visitor, keys[i]))
					return;
			}
		}
	}
	#endregion
	#region AppointmentPrevTreeEnumerator
	public class AppointmentPrevTreeEnumerator : AppointmentTreeEnumerator {
		public AppointmentPrevTreeEnumerator(AppointmentTree tree)
			: base(tree) {
		}
		protected override void EnumerateChildren(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
			int[] keys = Tree.GetSortedChildrenKeys();
			int count = keys.Length;
			for (int i = count - 1; i >= 0; i--) {
				if (!EnumerateCurrentChildren(visitor, keys[i]))
					return;
				PrevNextAppointmentTreeNodeVisitor prevNextVisitor = (PrevNextAppointmentTreeNodeVisitor)visitor;
				bool isFindAllResults = true;
				foreach (object key in prevNextVisitor.Result.Keys) {
					if (prevNextVisitor.Result[key] == null) {
						isFindAllResults = false;
						break;
					}
				}
				if (isFindAllResults)
					break;
			}
		}
	}
	#endregion
	#region AppointmentNextTreeEnumerator
	public class AppointmentNextTreeEnumerator : AppointmentTreeEnumerator {
		public AppointmentNextTreeEnumerator(AppointmentTree tree)
			: base(tree) {
		}
		protected override void EnumerateChildren(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
			int[] keys = Tree.GetSortedChildrenKeys();
			int count = keys.Length;
			for (int i = 0; i < count; i++) {
				if (!EnumerateCurrentChildren(visitor, keys[i]))
					return;
				bool isFindAllResults = true;
				PrevNextAppointmentTreeNodeVisitor prevNextVisitor = (PrevNextAppointmentTreeNodeVisitor)visitor;
				foreach (object key in prevNextVisitor.Result.Keys) {
					if (prevNextVisitor.Result[key] == null) {
						isFindAllResults = false;
						break;
					}
				}
				if (isFindAllResults)
					break;
			}
		}
	}
	#endregion
}
