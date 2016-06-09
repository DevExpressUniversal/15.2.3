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

using System.Collections.Generic;
using DevExpress.Xpf.Grid.TreeList;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Grid {
	public enum NodeChangeType { Add, Remove, Expand, Content, ExpandButtonVisibility, Image, CheckBox, IsCheckBoxEnabled }
	public class TreeListNodeCollection : Collection<TreeListNode> {
		public TreeListNodeCollection(TreeListNode owner) {
			this.Owner = owner;
		}
		protected internal TreeListNode Owner { get; set; }
		protected override void InsertItem(int index, TreeListNode item) {
			if(CanBeAddedAsChild(item)) {
				OnChanging(item, NodeChangeType.Add);
				base.InsertItem(index, item);
				LinkNode(item);
				OnChanged(item, NodeChangeType.Add);
			}
		}
		bool CanBeAddedAsChild(TreeListNode item) {
			return !(ReferenceEquals(this.Owner, item) || this.Owner.IsDescendantOf(item));
		}
		protected override void ClearItems() {
			List<TreeListNode> nodes = new List<TreeListNode>(Items);
			foreach(TreeListNode node in nodes) {
				OnChanging(node, NodeChangeType.Remove);
				UnlinkNode(node);
			}
			base.ClearItems();
			foreach(TreeListNode node in nodes) 
				OnChanged(node, NodeChangeType.Remove);
			nodes.Clear();
		}
		protected virtual void LinkNode(TreeListNode node) {
			if(node.ParentNode != null)
				node.ParentNode.Nodes.Remove(node);
			node.ParentNode = Owner;
			if(Owner.DataProvider != null && Owner.DataProvider.IsRecursiveNodesUpdateLocked)
				return;
			AssignFromOwner(node, Owner);
		}
		protected virtual void UnlinkNode(TreeListNode node) {
			node.ParentNode = null;
			if(Owner.DataProvider != null && Owner.DataProvider.IsRecursiveNodesUpdateLocked)
				return;
			AssignFromOwner(node, null);
		}
		protected static void AssignFromOwner(TreeListNode node, TreeListNode owner) {
			node.ProcessNodeAndDescendantsAction((n) => {
				n.DataProvider = owner != null ? owner.DataProvider : null;
				UpdateNodeId(n);
				if(n.DataProvider != null && n.DataProvider.IsUnboundMode)
					n.InitIsChecked();
				if(owner != null && owner.ItemTemplate != null) {
					n.Template = owner.ItemTemplate;
					n.ItemTemplate = owner.ItemTemplate;
				}
				return true;
			});
		}
		private static void UpdateNodeId(TreeListNode n) {
			if(n.DataProvider == null) {
				n.Id = -1;
				n.visibleIndex = -1;
				n.rowHandle = GridControl.InvalidRowHandle;
			} else
				n.UpdateId();
		}
		protected virtual void OnChanged(TreeListNode node, NodeChangeType changeType) {
			if(Owner != null && Owner.DataProvider != null)
				Owner.DataProvider.OnNodeCollectionChanged(node, changeType);
		}
		protected virtual void OnChanging(TreeListNode node, NodeChangeType changeType) {
			if(Owner != null && Owner.DataProvider != null)
				Owner.DataProvider.OnNodeCollectionChanging(node, changeType);
		}
		protected override void RemoveItem(int index) {
			TreeListNode node = this[index];
			EnsureNotEditing(node);
			OnChanging(node, NodeChangeType.Remove);
			UnlinkNode(node);
			base.RemoveItem(index);
			OnChanged(node, NodeChangeType.Remove);
		}
		private void EnsureNotEditing(TreeListNode node) {
			if(Owner != null && Owner.DataProvider != null) {
				TreeListDataProvider provider = Owner.DataProvider;
				if(provider.CurrentControllerRow == node.RowHandle) {
					provider.CloseActiveEditor();
					provider.EndCurrentRowEdit();
				}
			}
		}
		protected override void SetItem(int index, TreeListNode item) {
			base.SetItem(index, item);
			OnChanged(item, NodeChangeType.Add);
		}
		protected internal virtual void DoSort(IComparer<TreeListNode> comparer) {
			((List<TreeListNode>)Items).Sort(comparer);
		}
		protected internal TreeListNode GetFirstVisible() {
			for(int i = 0; i < Count; i++) {
				TreeListNode node = null;
				if(this[i].IsVisible) return this[i];
				else if(this[i].IsExpanded) node = this[i].Nodes.GetFirstVisible();
				if(node != null) return node;
			}
			return null;
		}
		protected internal TreeListNode GetLastVisible() {
			for(int i = this.Count - 1; i >= 0; i--) {
				TreeListNode node = null;
				if(this[i].IsVisible) return this[i];
				else if(this[i].IsExpanded) node = this[i].Nodes.GetLastVisible();
				if(node != null) return node;
			}
			return null;
		}
	}
}
