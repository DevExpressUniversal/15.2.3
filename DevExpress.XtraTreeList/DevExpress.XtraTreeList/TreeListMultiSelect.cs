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
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraTreeList.Columns;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.XtraTreeList {
	public class TreeListMultiSelection : CollectionBase, IEnumerable<TreeListNode> {
		private TreeList treeList;
		Dictionary<TreeListNode, CellSelectionInfo> cellSelection;
		public TreeListMultiSelection(TreeList treeList) {
			this.treeList = treeList;
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListMultiSelectionItem")]
#endif
		public TreeListNode this[int index] {
			get {
				if(index > -1 && index < Count) 
					return (TreeListNode)InnerList[index];
				return null;
			}
		}
		public new void Clear() {
			if(Count == 0) return;
			base.Clear();
		}
		public void Add(TreeListNode node) {
			if(!IsMultiSelect && Count > 0) return;
			if(TreeListAutoFilterNode.IsAutoFilterNode(node)) return;
			if(!Contains(node)) 
				List.Add(node);
		}
		public void Add(IEnumerable nodes) {
			if(nodes == null) return;
			if(!IsMultiSelect && Count > 0) return;
			bool changed = false;
			foreach(TreeListNode node in nodes) {
				if(TreeListAutoFilterNode.IsAutoFilterNode(node)) continue;
				if(!Contains(node)) {
					InnerList.Add(node);
					AddCellSelectionInfo(node);
					changed = true;
				}
			}
			if(changed) Changed();
		}
		public void AddRemove(TreeListNode node) {
			if(Contains(node)) Remove(node);
			else Add(node);
		}
		public void Remove(TreeListNode node) {
			int index = IndexOf(node);
			if(index > -1)
				List.RemoveAt(index);
		}
		protected override void OnInsertComplete(int index, object value) {
			AddCellSelectionInfo((TreeListNode)value);
			Changed(); 
		}
		protected override void OnRemoveComplete(int index, object value) {
			RemoveCellSelectionInfo((TreeListNode)value);
			Changed(); 
		}
		protected override void OnClearComplete() {
			ClearCellSelectionInfo();
			Changed();
		}
		public void Set(TreeListNode node) {
			if(node == null || (Count == 1 && this[0] == node)) return;
			if(!IsMultiSelect && node != treeList.FocusedNode) throw new NotSupportedException(TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MultiSelectMethodNotSupported));
			if(TreeListAutoFilterNode.IsAutoFilterNode(node)) return;
			InternalClear();
			Add(node);
		}
		public void Set(IEnumerable nodes) {
			InternalClear();
			if(nodes != null) {
				IEnumerator en = nodes.GetEnumerator();
				en.Reset();
				if(!IsMultiSelect) {
					if(en.MoveNext()) {
						InnerList.Add((TreeListNode)en.Current);
						AddCellSelectionInfo((TreeListNode)en.Current);
					}
				}
				else
					while(en.MoveNext()) {
						if(TreeListAutoFilterNode.IsAutoFilterNode((TreeListNode)en.Current)) continue;
						InnerList.Add((TreeListNode)en.Current);
						AddCellSelectionInfo((TreeListNode)en.Current);
					}
			}
			Changed();
		}
		public int IndexOf(TreeListNode node) {
			return InnerList.IndexOf(node);
		}
		public bool Contains(TreeListNode node) {
			return InnerList.Contains(node);
		}
		protected bool IsMultiSelect { get { return treeList.OptionsSelection.MultiSelect; } }
		protected bool IsCellSelection { get { return treeList.IsCellSelect; } }
		protected void Changed() { treeList.OnSelectionChanged(); }
		IEnumerator<TreeListNode> IEnumerable<TreeListNode>.GetEnumerator() {
			foreach(TreeListNode node in InnerList) 
				yield return node;
		}
		internal CellSelectionInfo InternalAdd(TreeListNode node) {
			if(node == null) return null;
			if(!InnerList.Contains(node))
				InnerList.Add(node);
			return AddCellSelectionInfo(node);
		}
		internal void InternalSet(TreeListNode node) {
			InternalClear();
			if(node != null)
				InternalAdd(node);
		}
		internal void InternalRemove(TreeListNode node) {
			int index = IndexOf(node);
			if(index > -1) {
				InnerList.RemoveAt(index);
				RemoveCellSelectionInfo(node);
			}
		}
		internal void InternalClear() {
			InnerList.Clear();
			ClearCellSelectionInfo();
		}
		internal CellSelectionInfo GetCellSelectionInfo(TreeListNode node) {
			if(CellSelection == null) return null;
			CellSelectionInfo info = null;
			if(cellSelection.TryGetValue(node, out info))
				return info;
			return null;
		}
		protected CellSelectionInfo AddCellSelectionInfo(TreeListNode node) {
			if(CellSelection == null) return null;
			cellSelection.Add(node, new CellSelectionInfo());
			return cellSelection[node];
		}
		protected void RemoveCellSelectionInfo(TreeListNode node) {
			if(CellSelection == null) return;
			cellSelection.Remove(node);
		}
		protected void ClearCellSelectionInfo() {
			if(CellSelection == null) return;
			CellSelection.Clear();
			cellSelection = null;
		}
		protected Dictionary<TreeListNode, CellSelectionInfo> CellSelection {
			get {
				if(IsCellSelection && cellSelection == null)
					cellSelection = new Dictionary<TreeListNode, CellSelectionInfo>();
				return cellSelection;
			}
		}
	}
	public class CellSelectionInfo {
		Dictionary<TreeListColumn, int> cells;
		public CellSelectionInfo() { }
		protected Dictionary<TreeListColumn, int> Cells {
			get {
				if(cells == null)
					cells = new Dictionary<TreeListColumn, int>();
				return cells;
			}
		}
		public bool IsEmpty { get { return cells == null; } }
		public bool Contains(TreeListColumn column) {
			if(IsEmpty) return false;
			return Cells.ContainsKey(column);
		}
		public bool AddCell(TreeListColumn column, bool useSelectionCount) {
			if(Contains(column)) {
				if(useSelectionCount)
					Cells[column]++;
				return false;
			}
			Cells.Add(column, 0);
			return true;
		}
		public bool RemoveCell(TreeListColumn column, bool useSelectionCount) {
			if(!Contains(column)) 
				return false;
			if(useSelectionCount && Cells[column] != 0) {
				Cells[column]--;
				return false;
			}
			Cells.Remove(column); 
			if(Cells.Count == 0)
				this.cells = null;
			return true;
		}
		public List<TreeListColumn> GetCells() {
			if(IsEmpty) return null;
			return Cells.Keys.ToList<TreeListColumn>();
		}
	}
}
