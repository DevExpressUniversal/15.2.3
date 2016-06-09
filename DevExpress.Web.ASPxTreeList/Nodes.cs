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
using System.Text;
using DevExpress.Data;
using DevExpress.Web.ASPxTreeList.Internal;
using System.ComponentModel;
using DevExpress.Web;
namespace DevExpress.Web.ASPxTreeList {
	public class TreeListNode {
		const string KeyArgumentName = "keyObject";
		string key;
		IWebTreeListData treeListData;
		TreeListNode parentNode;
		TreeListNodeCollection childNodes;
		TreeListNodeDataItem dataItem;
		int originalIndex;	
		bool allowSelect;
		internal TreeListNode(object keyObject, IWebTreeListData treeListData) {
			SetKeyValue(keyObject);
			this.treeListData = treeListData;
			this.childNodes = new TreeListNodeCollection();
			this.originalIndex = -1;
			this.allowSelect = true;
		}
		protected TreeListNode() {
		}
		protected internal TreeListNode(object keyObject)
			: this(keyObject, null) {
		}
		protected internal void SetKeyValue(object keyObject) {
			if(keyObject == TreeListNewNode.NewNodeMarker) {
				this.key = TreeListRenderHelper.NewNodeKey;
			} else if(keyObject == TreeListRootNode.RootNodeMarker) {
				this.key = TreeListRenderHelper.RootNodeKey;
			} else {
				this.key = PrepareKey(keyObject);
			}
		}
		string PrepareKey(object keyObject) {
			if(keyObject == null || keyObject == DBNull.Value)
				throw new ArgumentNullException(KeyArgumentName);
			if(!(keyObject is ValueType || keyObject is String))
				throw new ArgumentException("Must be a string or an instance of the ValueType", KeyArgumentName);
			string key = keyObject.ToString();
			if(keyObject is Guid)
				key = key.Replace("-", String.Empty); 
			if(key.Length == 0)
				throw new ArgumentException("Must not be empty", KeyArgumentName);
			return key;
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeItem")]
#endif
public object this[string fieldName] {
			get { return GetValue(fieldName); }
			set { SetValue(fieldName, value); }
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeKey")]
#endif
		public string Key { get { return key; } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeParentNode")]
#endif
		public TreeListNode ParentNode { get { return parentNode; } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeChildNodes")]
#endif
		public TreeListNodeCollection ChildNodes { get { return childNodes; } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeHasChildren")]
#endif
		public bool HasChildren { get { return ChildNodes.Count > 0; } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeLevel")]
#endif
		public int Level { get { return CalcLevel(); } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeExpanded")]
#endif
		public virtual bool Expanded {
			get { return IsExpanded(); }
			set { SetExpanded(value); }
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeSelected")]
#endif
		public virtual bool Selected {
			get { return GetCheckState() == CheckState.Checked; }
			set { SetSelected(value); }
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeAllowSelect")]
#endif
		public virtual bool AllowSelect {
			get { return allowSelect; }
			set {
				if(AllowSelect == value)
					return;
				allowSelect = value;
				TreeListData.ResetVisibleData();
			}
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeDataItem")]
#endif
		public object DataItem { get { return DataItemInternal.GetDataObject(); } }
		protected internal IWebTreeListData TreeListData { get { return treeListData; } }
		protected internal int OriginalIndex {
			get { return originalIndex; }
			set { originalIndex = value; }
		}
		protected internal TreeListNodeDataItem DataItemInternal {
			get { return dataItem; }
			set { dataItem = value; }
		}
		public virtual object GetValue(string fieldName) {
			if(DataItemInternal == null)
				return null;
			return DataItemInternal.GetValue(fieldName);
		}
		public virtual void SetValue(string fieldName, object value) {
			if(DataItemInternal == null)
				return;
			DataItemInternal.SetValue(fieldName, value);
		}
		public void Focus() {
			MakeVisible();
			TreeListData.FocusedNodeKey = Key;
		}
		public void MakeVisible() {
			TreeListNode parent = ParentNode;
			while(parent != null) {
				parent.Expanded = true;
				parent = parent.ParentNode;
			}
			TreeListData.PageToNode(Key);
		}
		protected internal bool AppendChild(TreeListNode node) {
			if(!CanAppend(node))
				return false;
			if(node.ParentNode != null)
				node.Unlink();
			ChildNodes.Add(node);
			node.SetParentNode(this);
			return true;
		}
		protected internal void SetParentNode(TreeListNode node) {
			this.parentNode = node;
		}
		protected int CalcLevel() {
			int level = 0;
			TreeListNode parent = ParentNode;
			while(parent != null) {
				level++;
				parent = parent.ParentNode;
			}
			return level;
		}
		protected void Unlink() {
			ParentNode.ChildNodes.Remove(this);
			SetParentNode(null);
		}
		protected bool CanAppend(TreeListNode node) {
			if(TreeListData != null && node.TreeListData != null && TreeListData != node.TreeListData)
				return false;
			return !TreeListUtils.TestParentChildRelationship(node, this);
		}
		protected CheckState GetCheckState() {
			return TreeListData == null ? CheckState.Unchecked : this.TreeListData.GetNodeCheckState(Key);
		}
		protected bool IsExpanded() {
			if(TreeListData == null)
				return false;
			return TreeListData.IsNodeExpanded(Key);
		}
		protected void SetSelected(bool value) {
			if(!AllowSelect)
				value = false;
			if(TreeListData != null)
				TreeListData.SetNodeSelected(Key, value);
		}
		protected void SetExpanded(bool value) {
			if(TreeListData != null)
				TreeListData.SetNodeExpanded(Key, value);
		}
		protected internal bool IsFirst() {
			if(ParentNode == null)
				return true;
			return ParentNode.ChildNodes[0] == this;
		}
		protected internal bool IsLast() {
			if(ParentNode == null)
				return true;
			TreeListNodeCollection list = ParentNode.childNodes;
			return list[list.Count - 1] == this;
		}
#if DEBUG
		public override string ToString() {
			return String.Format("Key={0}", Key);
		}
#endif
	}
	public class TreeListNodeCollection : IEnumerable {
		List<TreeListNode> innerList;
		internal TreeListNodeCollection() {
			this.innerList = new List<TreeListNode>();
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeCollectionCount")]
#endif
		public int Count { get { return InnerList.Count; } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeCollectionItem")]
#endif
public TreeListNode this[int index] { get { return InnerList[index]; } }
		protected List<TreeListNode> InnerList { get { return innerList; } }
		protected internal void Clear() {
			InnerList.Clear();
		}
		protected internal void Add(TreeListNode node) {
			InnerList.Add(node);
		}
		protected internal void Remove(TreeListNode node) {
			InnerList.Remove(node);
		}
		protected internal void Sort(IComparer<TreeListNode> comparer) {
			InnerList.Sort(comparer);
		}
		IEnumerator IEnumerable.GetEnumerator() { return InnerList.GetEnumerator(); }
	}
	public class TreeListNodeIterator {
		TreeListNode startNode;
		TreeListNode currentNode;
		Stack<int> indexStack;
		public TreeListNodeIterator(TreeListNode startNode) {
			this.indexStack = new Stack<int>();
			this.startNode = this.currentNode = startNode;
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListNodeIteratorCurrent")]
#endif
		public TreeListNode Current { get { return currentNode; } }
		protected TreeListNode StartNode { get { return startNode; } }
		protected Stack<int> IndexStack { get { return indexStack; } }
		public void Reset() {
			this.currentNode = StartNode;
			IndexStack.Clear();
		}
		public TreeListNode GetNext() {
			return GetNextCore(false);
		}
		protected internal TreeListNode GetNextVisible() {
			return GetNextCore(true);
		}
		TreeListNode GetNextCore(bool onlyVisible) {
			if(Current == null)
				return null;
			if(Current.HasChildren && (!onlyVisible || Current.Expanded)) {
				IndexStack.Push(0);
				return UpdateCurrent(Current.ChildNodes[0]);
			}
			TreeListNode node = Current;
			while(IndexStack.Count > 0 && node != StartNode) {
				TreeListNodeCollection siblings = node.ParentNode.ChildNodes;
				int index = IndexStack.Pop();
				if(index < siblings.Count - 1) {
					index++;
					IndexStack.Push(index);
					return UpdateCurrent(siblings[index]);
				}
				node = node.ParentNode;
			}
			return UpdateCurrent(null);
		}
		TreeListNode UpdateCurrent(TreeListNode node) {
			this.currentNode = node;
			return node;
		}
	}
}
namespace DevExpress.Web.ASPxTreeList.Internal {
	public class TreeListRootNode : TreeListNode {
		internal static readonly object RootNodeMarker = new object();
		internal TreeListRootNode(IWebTreeListData treeListData)
			: base(RootNodeMarker, treeListData) {
		}
		public override bool Expanded { get { return true; } set { } }
		public override object GetValue(string fieldName) {
			return null;
		}
#if DEBUG
		public override string ToString() {
			return "Root";
		}
#endif
	}
	public class TreeListVirtualNode : TreeListNode {
		object nodeObject;
		bool isLeaf;
		public TreeListVirtualNode(object keyObject, IWebTreeListData treeListData, object nodeObject, bool isLeaf)
			: base(keyObject, treeListData) {
			this.nodeObject = nodeObject;
			this.isLeaf = isLeaf;
		}
		protected internal object NodeObject { get { return nodeObject; } set { nodeObject = value; } }
		protected internal bool IsLeaf { get { return isLeaf; } }
	}
	public class TreeListNewNode : TreeListNode {
		internal static readonly object NewNodeMarker = new object();
		public TreeListNewNode(IWebTreeListData data) 
			: base(NewNodeMarker, data) {
			DataItemInternal = new TreeListUnboundNodeDataItem(data);
		}
		public override bool AllowSelect { get { return false; } set { } }
	}
	public class TreeListNodeComparer : Comparer<TreeListNode> {
		ASPxTreeList treeList;
		public TreeListNodeComparer(ASPxTreeList treeList) {
			this.treeList = treeList;
		}
		protected ASPxTreeList TreeList { get { return treeList; } }
		public override int Compare(TreeListNode x, TreeListNode y) {
			if(x == y) return 0;
			if(x.Key == TreeListRenderHelper.NewNodeKey) return -1;
			if(y.Key == TreeListRenderHelper.NewNodeKey) return 1;
			int result = 0;
			TreeListCustomNodeSortEventArgs e = null;
			ColumnSortOrder lastOrder = ColumnSortOrder.None;
			foreach(TreeListDataColumn column in treeList.SortedColumns) {
				bool handled = false;
				if(TreeList.IsCustomNodeSortEventAssigned()) {
					if(e == null)
						e = new TreeListCustomNodeSortEventArgs(column, x, y);
					else
						e.SetColumn(column);
					TreeList.RaiseCustomNodeSort(e);
					if(e.Handled) {
						result = e.Result;
						handled = true;
					}
				}
				if(!handled)
					result = CompareInternal(x, y, column.FieldName);				
				if(column.SortOrder == ColumnSortOrder.Descending)
					result = -result;
				lastOrder = column.SortOrder;
				if(result != 0) break;
			}
			if(result == 0) {
				result = Comparer.Default.Compare(x.OriginalIndex, y.OriginalIndex);
				if(lastOrder == ColumnSortOrder.Descending)
					result = -result;
			}
			return result;
		}
		protected int CompareInternal(TreeListNode x, TreeListNode y, string fieldName) {
			object xValue = x.GetValue(fieldName);
			object yValue = y.GetValue(fieldName);
			if(xValue == DBNull.Value || xValue == null)
				return -1;
			if(yValue == DBNull.Value || yValue == null)
				return 1;
			return Comparer.Default.Compare(xValue, yValue);
		}
	}
}
