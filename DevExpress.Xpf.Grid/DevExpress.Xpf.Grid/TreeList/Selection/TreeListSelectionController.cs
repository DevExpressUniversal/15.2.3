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

using DevExpress.Xpf.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Collections;
using System.Linq;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using DevExpress.Data;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListSelectionController : ISelectionController {
		int selectionLockCount = 0;
		bool actuallyChanged = false;
		TreeListDataProvider dataProvider;
		public TreeListSelectionController(TreeListDataProvider dataProvider) {
			this.dataProvider = dataProvider;
			this.Selection = new Dictionary<TreeListNode, object>();
		}
		public bool IsSelectionLocked { get { return this.selectionLockCount != 0; } }
		public int Count { get { return Selection.Count; } }
		protected Dictionary<TreeListNode, object> Selection { get; private set; }
		protected TreeListDataProvider DataProvider { get { return dataProvider; } }
		public void BeginSelection() {
			if(this.selectionLockCount++ == 0) actuallyChanged = false;
		}
		public void EndSelection() {
			if(--this.selectionLockCount == 0 && actuallyChanged) {
				OnSelectionChanged(new DevExpress.Data.SelectionChangedEventArgs());
			}
		}
		public void CancelSelection() {
			this.selectionLockCount--;
		}
		public bool GetSelected(int rowHandle) {
			return GetSelected(DataProvider.GetNodeByRowHandle(rowHandle));
		}
		public bool GetSelected(TreeListNode node) {
			if(node == null) return false;
			return Selection.ContainsKey(node);
		}
		public void SetSelected(TreeListNode node, bool selected) {
			SetSelected(node, selected, null);
		}
		public void SetSelected(TreeListNode node, bool selected, object selectedObject) {
			if(node == null) return;
			if(GetSelected(node) == selected) {
				if(selected && selectedObject != null) {
					if(Object.Equals(GetSelectedObject(node), selectedObject)) return;
				}
				else
					return;
			}
			if(!selected)
				Selection.Remove(node);
			else
				Selection[node] = selectedObject;
			OnSelectionChanged(new DevExpress.Data.SelectionChangedEventArgs(selected ? CollectionChangeAction.Add : CollectionChangeAction.Remove, node.RowHandle));
		}
		public void SetSelected(int rowHandle, bool selected) {
			SetSelected(DataProvider.GetNodeByRowHandle(rowHandle), selected);
		}
		public void SetSelected(int rowHandle, bool selected, object selectedObject) {
			SetSelected(DataProvider.GetNodeByRowHandle(rowHandle), selected, selectedObject);
		}
		public void Clear() {
			if(Count == 0) return;
			Selection.Clear();
			OnSelectionChanged(new DevExpress.Data.SelectionChangedEventArgs());
		}
		public void SelectAll() {
			BeginSelection();
			try {
				Clear();
				foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes))
					if(node.IsVisible)
						SetSelected(node, true);
			} finally {
				EndSelection();
			}
		}
		public int[] GetSelectedRows() {
			if(Count == 0)
				return new int[0];
			int[] list = (from TreeListNode node in Selection.Keys
						  select node.RowHandle).ToArray<int>();
			Array.Sort<int>(list);
			return list;
		}
		public TreeListNode[] GetSelectedNodes() {
			if(Count == 0)
				return new TreeListNode[0];
			return Selection.Keys.OrderBy<TreeListNode, int>(n => n.RowHandle).ToArray<TreeListNode>();
		}
		public void SetActuallyChanged() {
			if(IsSelectionLocked) actuallyChanged = true;
		}
		protected void OnSelectionChanged(DevExpress.Data.SelectionChangedEventArgs e) {
			this.actuallyChanged = true;
			if(IsSelectionLocked) return;
			DataProvider.OnSelectionChanged(e);
		}
		protected object GetSelectedObject(TreeListNode node) {
			if(node != null && Selection.ContainsKey(node))
				return Selection[node];
			return null;
		}
		#region ISelectionController Members
		object ISelectionController.GetSelectedObject(int rowHandle) {
			return GetSelectedObject(DataProvider.GetNodeByRowHandle(rowHandle));
		}
		void ISelectionController.SetSelected(int rowHandle, bool selected, object selectionObject) {
			SetSelected(rowHandle, selected, selectionObject);
		}
		#endregion
		#region comparer
		class RowHandleComparer : IComparer<TreeListNode> {
			static RowHandleComparer defaultComparer = new RowHandleComparer();
			public static RowHandleComparer Default { get { return defaultComparer; } }
			public int Compare(TreeListNode x, TreeListNode y) {
				return Comparer<int>.Default.Compare(x.RowHandle, y.RowHandle);
			}
		}
		#endregion
	}
}
