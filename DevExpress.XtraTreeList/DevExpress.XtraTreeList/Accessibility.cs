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
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using DevExpress.Accessibility;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.XtraTreeList.Accessibility {
	public class TreeListControlAccessible : BaseGridAccessibleObject {
		TreeList control;
		public TreeListControlAccessible(TreeList owner) { 
			control = owner;
		}
		protected TreeList TreeList {  get { return control; } }
		public override Control GetOwnerControl() { return TreeList; } 
		protected override AccessibleRole GetRole(){
			return AccessibleRole.Outline; 
		} 
		protected override IAccessibleGrid Grid { 
			get {
				return TreeList as IAccessibleGrid;
			} 
		}
		protected class TreeListDataPanel : BaseGridDataPanelAccessibleObject {  
			public TreeListDataPanel(IAccessibleGrid owner) : base(owner) { }
			protected override BaseAccessible CreateChild(int index) {
				IAccessibleGridRow row = Grid.GetRow(index); 
				if(row == null) return null;
				return new TreeListRowAccessibleObject(row as TreeListAccessibleRow);
			}
			protected TreeList TreeList { get { return (TreeList)Grid; } } 
			public override Rectangle ClientBounds { get { return TreeList.ViewInfo.ViewRects.Rows; } }
			public class TreeListRowAccessibleObject : BaseGridRowAccessibleObject {
				public TreeListRowAccessibleObject(TreeListAccessibleRow row) : base(row) { }
				new TreeListAccessibleRow  Row { get { return base.Row as TreeListAccessibleRow; } }
				protected override AccessibleRole GetRole() {
					return AccessibleRole.OutlineItem;
				}
				public override AccessibleObject Navigate(AccessibleNavigation nav) {
					if(nav == AccessibleNavigation.Left) {
						TreeListNode node = Row.TreeList.GetNodeByVisibleIndex(Row.AccessibleIndex);
						if(node.ParentNode != null) 
							return ParentCore.GetChild(Row.TreeList.GetVisibleIndexByNode(node.ParentNode));
					}
					if(nav == AccessibleNavigation.Right) {
						TreeListNode node = Row.TreeList.GetNodeByVisibleIndex(Row.AccessibleIndex);
						if(node.HasChildren) 
							return ParentCore.GetChild(Row.TreeList.GetVisibleIndexByNode(node.Nodes[0]));
					}
					return base.Navigate(nav);
				}
			}
		}
		protected class TreeListHeaderPanel : BaseGridHeaderPanelAccessibleObject {
			public TreeListHeaderPanel(IAccessibleGrid owner) : base(owner) { }
			protected TreeList TreeList { get { return (TreeList)Grid; } }
			protected override void OnChildrenCountChanged() {
				foreach(TreeListColumn column in TreeList.Columns) {
					if(!(column.VisibleIndex > -1)) continue;
					AddChild(CreateHeader(column));
				}
			}
			public override Control GetOwnerControl() { return TreeList; } 
			public override Rectangle ClientBounds { get { return TreeList.ViewInfo.ViewRects.ColumnPanel; } }
			protected virtual BaseGridHeaderAccessibleObject CreateHeader(TreeListColumn column) {
				return new BaseGridHeaderAccessibleObject(new TreeListAccessibleHeader(TreeList, column));
			}
		}
		protected override BaseGridHeaderPanelAccessibleObject CreateHeader() { 
			return new TreeListHeaderPanel(TreeList); 
		}
		protected override BaseGridDataPanelAccessibleObject CreateRows() { 
			return new TreeListDataPanel(TreeList); 
		}
		public override AccessibleObject GetAccessibleObjectById(int objectId, int childId) {
			if(DataPanel != null) {
				TreeListDataPanel.TreeListRowAccessibleObject rowObj = DataPanel.Children[TreeList.FocusedRowIndex] as TreeListDataPanel.TreeListRowAccessibleObject;
				if(rowObj != null && rowObj.HasChildren && TreeList.FocusedCellIndex >= 0 && TreeList.FocusedCellIndex < rowObj.Children.Count) {
					BaseGridCellAccessibleObject cellObj = rowObj.Children[TreeList.FocusedCellIndex] as BaseGridCellAccessibleObject;
					if(cellObj != null) return cellObj.Accessible;
				}
			}
			return null;
		}
	}
	public class TreeListAccessibleRow : IAccessibleGridRow {
		TreeList control;
		int rowIndex;
		public TreeListAccessibleRow(TreeList owner, int rowIndex) { 
			control = owner;
			this.rowIndex = rowIndex;
		}
		public TreeList TreeList{ get { return control; } }
		public  Rectangle Bounds {
			get {
				RowInfo row = TreeList.ViewInfo.RowsInfo[TreeList.GetNodeByVisibleIndex(RowIndex)];   
				return row != null ? row.Bounds : Rectangle.Empty;
			}
		}
		public string GetString(AccStringId id) { return AccLocalizer.Active.GetLocalizedString(id); }
		public int RowIndex { 
			get {
				return rowIndex; 
			} 
			set { 
				rowIndex = value; 
			} 
		}
		public int AccessibleIndex { get  { return RowIndex; } }
		int IAccessibleGridRow.Index { get { return AccessibleIndex; } }
		AccessibleStates IAccessibleGridRow.GetState() { return GetAccessibleState(); } 
		string IAccessibleGridRow.GetName() { return GetAccessibleRowName(); }
		protected virtual AccessibleStates GetAccessibleState() {
			AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;
			if (((IAccessibleGridRow)this).Bounds.IsEmpty) { 
				state |= AccessibleStates.Invisible; 
				return state;
			}
			if (TreeList.FocusedNode == TreeList.GetNodeByVisibleIndex(AccessibleIndex)) state |= AccessibleStates.Focused;
			if (TreeList.Selection.Contains( TreeList.GetNodeByVisibleIndex(AccessibleIndex)))  state |= AccessibleStates.Selected;
			if (TreeList.GetNodeByVisibleIndex(AccessibleIndex).HasChildren &&
				(TreeList.GetNodeByVisibleIndex(AccessibleIndex).Expanded)) state |= AccessibleStates.Expanded; 
			else if (TreeList.GetNodeByVisibleIndex(AccessibleIndex).HasChildren &&
				!(TreeList.GetNodeByVisibleIndex(AccessibleIndex).Expanded)) state |= AccessibleStates.Collapsed;
			if(TreeList.GetNodeByVisibleIndex(AccessibleIndex).Checked) state |= AccessibleStates.Checked;
			return state;
		}
		protected virtual string GetAccessibleRowName() {
			return GetString(AccStringId.TreeListNode) + AccessibleIndex.ToString();
		}
		public virtual string GetDefaultAction() { 
			if(TreeList.FocusedNode == TreeList.GetNodeByVisibleIndex(AccessibleIndex)) return null;
			return GetString(AccStringId.TreelistRowActivate);
		}
		public virtual void DoDefaultAction() { 
			TreeList.FocusedNode = TreeList.GetNodeByVisibleIndex(AccessibleIndex);
		}
		public virtual string GetValue() { return null; }  
		public virtual int CellCount { get { return 0; } } 
		public virtual IAccessibleGridRowCell GetCell(int index) { return null; } 
		public virtual BaseAccessible GetChildTable() { return null; }
	}
	public class TreeListAccessibleHeader : IAccessibleGridHeaderCell {
		TreeList control;
		TreeListColumn column;
		public TreeListAccessibleHeader(TreeList owner, TreeListColumn column) {
			this.control = owner;
			this.column = column;
		}
		public string GetString(AccStringId id) { return AccLocalizer.Active.GetLocalizedString(id); }
		public TreeList TreeList { get { return control; } }
		public TreeListColumn Column { get { return column; } }
		public virtual Rectangle Bounds { 
			get {
				ColumnInfo info = TreeList.ViewInfo.ColumnsInfo[Column];
				if(info != null) return info.Bounds;
				return Rectangle.Empty;
			}
		}
		public virtual string GetDefaultAction() {
			if (Column.OptionsColumn.AllowSort){
				if (Column.SortOrder == SortOrder.None)
					return GetString(AccStringId.TreeListColumnSortAscending);
				if (Column.SortOrder == SortOrder.Ascending)
					return GetString(AccStringId.TreeListColumnSortDescending);
				return GetString(AccStringId.TreeListColumnSortNone);
			}
			return null;
		}
		public virtual void DoDefaultAction() {
			if (Column.OptionsColumn.AllowSort){
				SortOrder res = SortOrder.Ascending;
				if(column.SortOrder == SortOrder.Ascending)
					res = SortOrder.Descending;
				if(column.SortOrder == SortOrder.Descending)
					res = SortOrder.None;
				Column.SortOrder = res;
			}
		}
		public virtual string GetName() {
			return Column.GetTextCaption();
		}
		public virtual AccessibleStates GetState() {
			if(!(Column.VisibleIndex > -1)) return AccessibleStates.Invisible;
			return AccessibleStates.Default;
		}
	}
	public class TreeListAccessibleDataRow : TreeListAccessibleRow {
		public TreeListAccessibleDataRow(TreeList owner, int rowIndex) : base(owner, rowIndex) { }
		public override string GetDefaultAction() { 
			if(!IsMasterNode || TreeList.ViewInfo.RowsInfo[TreeList.GetNodeByVisibleIndex(AccessibleIndex)].Bounds.IsEmpty) return base.GetDefaultAction();
			if (TreeList.GetNodeByVisibleIndex(AccessibleIndex).Expanded) 
				return GetString(AccStringId.TreeListNodeCollapse);
			if (!TreeList.GetNodeByVisibleIndex(AccessibleIndex).Expanded) 
				return GetString(AccStringId.TreeListNodeExpand);
			return null;
		}
		public override void DoDefaultAction() { 
			if(!IsMasterNode)
				base.DoDefaultAction();
			else {
				if(TreeList.GetNodeByVisibleIndex(AccessibleIndex).Expanded) 
					TreeList.GetNodeByVisibleIndex(AccessibleIndex).Expanded = false;
				else 
					TreeList.GetNodeByVisibleIndex(AccessibleIndex).Expanded = true;
			}
		}
		protected override AccessibleStates GetAccessibleState() {
			return base.GetAccessibleState();
		}
		protected bool IsMasterNode { 
			get { 
				return TreeList.GetNodeByVisibleIndex(AccessibleIndex).HasChildren; 
			} 
		}
		public override IAccessibleGridRowCell GetCell(int index) { 
			if(index >= TreeList.VisibleColumns.Count) return null;
			return new TreeListAccessibleRowCell(TreeList, RowIndex, TreeList.VisibleColumns[index]);
		}
		public override string GetValue() {
			string val = string.Empty;
			for(int i = 0; i < CellCount; i++) {
				if(i > 0) val += ";";
				val += GetCell(i).GetValue();
			}
			return val;
		}
		public override int CellCount { get { return TreeList.VisibleColumns.Count; } }
	}
	public class TreeListAccessibleRowCell : IAccessibleGridRowCell {
		TreeList control;
		int rowIndex;
		TreeListColumn column;
		public TreeListAccessibleRowCell(TreeList owner, int rowIndex, TreeListColumn column){ 
			control = owner;
			this.rowIndex = rowIndex;
			this.column = column;
		}
		public TreeList TreeList { get { return control; } }
		public int RowIndex { get { return rowIndex; } }
		public Rectangle Bounds {
			get {
				RowInfo row = TreeList.ViewInfo.RowsInfo[TreeList.GetNodeByVisibleIndex(RowIndex)];
				if(row == null) return Rectangle.Empty;
				CellInfo cell = row[Column]; 
				return cell == null ? Rectangle.Empty : cell.Bounds;
			}
		}
		public string GetString(AccStringId id) { return AccLocalizer.Active.GetLocalizedString(id); }
		public TreeListColumn Column { get { return column; } }
		string IAccessibleGridRowCell.GetDefaultAction() { 
			if(IsReadOnly)
				return Column.OptionsColumn.AllowFocus ?  GetString(AccStringId.GridCellFocus) : null;
			return GetString(AccStringId.TreeListCellEdit); 
		}
		public bool IsReadOnly {
			get {
				return !TreeList.OptionsBehavior.Editable || !Column.OptionsColumn.AllowEdit || Column.OptionsColumn.ReadOnly;
			}
		}
		public bool IsFocused { get { return (TreeList.FocusedNode == TreeList.GetNodeByVisibleIndex(RowIndex) && (TreeList.FocusedColumn == Column)); } }
		BaseAccessible IAccessibleGridRowCell.GetEdit() {
			if(TreeList.OptionsBehavior.Editable && TreeList.ActiveEditor != null && IsFocused) return TreeList.ActiveEditor.GetAccessible();
			return null;
		}
		void IAccessibleGridRowCell.DoDefaultAction() { 
			if(!Column.OptionsColumn.AllowFocus) return;
			TreeList.FocusedNode = TreeList.GetNodeByVisibleIndex(RowIndex);
			if(TreeList.FocusedNode != TreeList.GetNodeByVisibleIndex(RowIndex)) return;
			TreeList.FocusedColumn = Column;
			if(TreeList.FocusedColumn != Column) return;
			TreeList.ShowEditor();
			return;
		}
		string IAccessibleGridRowCell.GetName() {
			return Column.GetTextCaption() + " row " + this.RowIndex;
		}
		string IAccessibleGridRowCell.GetValue() {
			RowInfo row = TreeList.ViewInfo.RowsInfo[TreeList.GetNodeByVisibleIndex(RowIndex)];
			return row.Node[Column].ToString();
		}
		AccessibleStates IAccessibleGridRowCell.GetState() { 
			AccessibleStates state = IsReadOnly ? AccessibleStates.ReadOnly : AccessibleStates.None;
			if(Bounds.IsEmpty) {
				state |= AccessibleStates.Invisible; 
			} else {
				if(Column.OptionsColumn.AllowFocus) state |= AccessibleStates.Focusable;
				if(IsFocused) state |= AccessibleStates.Focused;
			}
			return state;
		}
	}
}
