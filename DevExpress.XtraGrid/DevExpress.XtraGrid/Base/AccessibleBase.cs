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
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using DevExpress.Accessibility;
using DevExpress.XtraEditors;
using DevExpress.Data;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Tab;
namespace DevExpress.XtraGrid.Accessibility {
	public abstract class ColumnViewAccessibleObject : BaseGridAccessibleObject {
		ColumnView view;
		public ColumnViewAccessibleObject(ColumnView view) {
			this.view = view;
		}
		public override bool IsTopControl { get { return View.GridControl != null && View.GridControl.DefaultView == View; } }
		protected internal ColumnView View { get { return view; } }
		public override Control GetOwnerControl() { return View.GridControl; } 
		public override Rectangle ClientBounds { get { return View.ViewRect; } }
		protected override string GetName() {
			return View.ViewCaption == "" ? null : View.ViewCaption;
		}
		public override AccessibleObject GetAccessibleObjectById(int objectId, int childId) {
			return null;
		}
		internal void UpdateHeaderAccessible() {
			if(HeaderPanel != null)
				HeaderPanel.ForceUpdateColumns();
		}
	}
	public class CardViewAccessibleObject : ColumnViewAccessibleObject {
		public CardViewAccessibleObject(CardView view) : base(view) { }
		protected override IAccessibleGrid Grid { get { return View; } }
		protected new CardView View { get { return (CardView)base.View; } }
		protected class CardViewDataPanel : BaseGridDataPanelAccessibleObject {
			public CardViewDataPanel(CardView view) : base(view) { }
			protected override BaseAccessible CreateChild(int index) {
				IAccessibleGridRow row = Grid.GetRow(index);
				if(row == null) return null;
				return new DevExpress.Accessibility.BaseGridAccessibleObject.BaseGridRowAccessibleObject(row);
			}
			protected CardView View { get { return (CardView)Grid; } }
			public override Rectangle ClientBounds { get { return View.ViewInfo.ViewRects.Cards; } }
		}
		protected override BaseGridHeaderPanelAccessibleObject CreateHeader() { return null; }
		protected override BaseGridDataPanelAccessibleObject CreateRows() { return new CardViewDataPanel(View); }
	}
	internal interface IAccessibleRowWithUpdate {
		void UpdateChildren();
	}
	public class GridViewAccessibleObject : ColumnViewAccessibleObject {
		public GridViewAccessibleObject(GridView view) : base(view) { }
		protected override IAccessibleGrid Grid { get { return View; } }
		protected internal new GridView View { get { return (GridView)base.View; } }
		protected class GridViewDataPanel : BaseGridDataPanelAccessibleObject {
			public GridViewDataPanel(GridView view) : base(view) { }
			protected override BaseAccessible CreateChild(int index) {
				IAccessibleGridRow row = Grid.GetRow(index);
				if(row == null) return null;
				return new GridRowAccessibleObject(row as GridAccessibleRow);
			}
			protected GridView View { get { return (GridView)Grid; } }
			public override Rectangle ClientBounds { get { return View.ViewInfo.ViewRects.Rows; } }
			protected class GridRowAccessibleObject : BaseGridRowAccessibleObject, IAccessibleRowWithUpdate {
				public GridRowAccessibleObject(GridAccessibleRow row) : base(row) { }
				protected internal GridAccessibleRow GridRow { get { return Row as GridAccessibleRow; } }
				protected override ChildrenInfo GetChildrenInfo() {
					ChildrenInfo res = base.GetChildrenInfo();
					if(GridRow.GetChildTable() != null) {
						res["Table " + GridRow.GetChildTableIndex()] = GridRow.GetChildTable() != null ? 1 : 0;
					}
					res["DetailPages"] = GridRow.IsMasterRow? 1 : 0;
					return res;
				}
				protected override void OnChildrenCountChanged() {
					base.OnChildrenCountChanged();
					if(GridRow.GetChildTable() != null) AddChild(GridRow.GetChildTable());
					if(GridRow.IsMasterRow) AddChild(new DetailViewCollectionAccessible(GridRow));
				}
				public override string Value {
					get {
						return Row.GetValue();
					}
				}
				#region IUpdateAccessibleChildren Members
				void IAccessibleRowWithUpdate.UpdateChildren() {
					ForceUpdateChildren();
				}
				#endregion
			}
			protected override AccessibleRole GetRole() {
				return AccessibleRole.Client;
			}
		}
		protected class GridViewHeaderPanel : BaseGridHeaderPanelAccessibleObject {
			public GridViewHeaderPanel(GridView view) : base(view) { }
			protected GridView View { get { return (GridView)Grid; } }
			protected override void OnChildrenCountChanged() {
				foreach(GridColumn column in View.Columns) {
					if(!column.Visible || (column.GroupIndex != -1 && !View.OptionsView.ShowGroupedColumns)) continue;
					AddChild(CreateHeader(column));
				}
			}
			public override Rectangle ClientBounds { get { return View.ViewInfo.ViewRects.ColumnPanel; } }
			protected virtual BaseGridHeaderAccessibleObject CreateHeader(GridColumn column) {
				return new BaseGridHeaderAccessibleObject(new GridAccessibleHeader(View, column));
			}
			protected override AccessibleRole GetRole() {
				return AccessibleRole.Client;
			}
		}
		protected override BaseGridHeaderPanelAccessibleObject CreateHeader() { 
			return new GridViewHeaderPanel(View); 
		}
		protected override BaseGridDataPanelAccessibleObject CreateRows() { 
			return new GridViewDataPanel(View);
		}
		public override AccessibleObject GetAccessibleObjectById(int objectId, int childId) {
			if(DataPanel != null && View != null) {
				int accIndex = View.RowHandle2AccessibleIndex(View.FocusedRowHandle);
				if(accIndex < DataPanel.ChildCount) {
					BaseGridRowAccessibleObject rowObject = DataPanel.Children[accIndex] as BaseGridRowAccessibleObject;
					if(rowObject != null && View.FocusedColumn != null && View.FocusedColumn.VisibleIndex >= 0 && View.FocusedColumn.VisibleIndex < rowObject.ChildCount) {
						BaseGridCellAccessibleObject cellObject = rowObject.Children[View.FocusedColumn.VisibleIndex] as BaseGridCellAccessibleObject;
					if(cellObject != null) return cellObject.Accessible;
				}
			}
			}
			return null;
		}
	}
	public class GridAccessibleHeader : IAccessibleGridHeaderCell {
		GridView view;
		GridColumn column;
		public GridAccessibleHeader(GridView view, GridColumn column) {
			this.view = view;
			this.column = column;
		}
		public string GetString(AccStringId id) { return AccLocalizer.Active.GetLocalizedString(id); }
		public GridView View { get { return view; } }
		public GridColumn Column { get { return column; } }
		public virtual Rectangle Bounds { 
			get {
				GridColumnInfoArgs info = View.ViewInfo.ColumnsInfo[Column];
				if(info != null) return info.Bounds;
				return Rectangle.Empty;
			}
		}
		public virtual string GetDefaultAction() {
			if(View.CanSortColumn(column)) {
				if(column.SortOrder == ColumnSortOrder.None)
					return GetString(AccStringId.GridColumnSortAscending);
				if(column.SortOrder == ColumnSortOrder.Ascending)
					return GetString(AccStringId.GridColumnSortDescending);
				return GetString(AccStringId.GridColumnSortNone);
			}
			return null;
		}
		public virtual void DoDefaultAction() {
			if(View.CanSortColumn(column)) {
				ColumnSortOrder res = ColumnSortOrder.Ascending;
				if(column.SortOrder == ColumnSortOrder.Ascending)
					res = ColumnSortOrder.Descending;
				if(column.SortOrder == ColumnSortOrder.Descending)
					res = ColumnSortOrder.None;
				Column.SortOrder = res;
			}
		}
		public virtual string GetName() {
			return Column.GetTextCaption();
		}
		public virtual AccessibleStates GetState() {
			if(!Column.Visible) return AccessibleStates.Invisible;
			return AccessibleStates.Default;
		}
	}
	public abstract class ColumnAccessibleRow : IAccessibleGridRow {
		int rowHandle;
		ColumnView view;
		public ColumnAccessibleRow(ColumnView view, int rowHandle) {
			this.view = view;
			this.rowHandle = rowHandle;
		}
		public string GetString(AccStringId id) { return AccLocalizer.Active.GetLocalizedString(id); }
		public int RowHandle { get { return rowHandle; } set { rowHandle = value; } }
		public ColumnView View { get { return view; } }
		public abstract Rectangle Bounds { get; }
		public int AccessibleIndex { get { return View.RowHandle2AccessibleIndex(RowHandle); } }
		int IAccessibleGridRow.Index { get { return AccessibleIndex; } }
		AccessibleStates IAccessibleGridRow.GetState() { return GetAccessibleState(); } 
		string IAccessibleGridRow.GetName() { return GetAccessibleRowName(); }
		protected virtual AccessibleStates GetAccessibleState() {
			AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable | AccessibleStates.ReadOnly;
			state |= ((IAccessibleGridRow)this).Bounds.IsEmpty ? AccessibleStates.Invisible : AccessibleStates.None;
			if(View.FocusedRowHandle == RowHandle) state |= AccessibleStates.Focused;
			if(View.IsRowSelected(RowHandle)) state |= AccessibleStates.Selected;
			return state;
		}
		protected virtual string GetAccessibleRowName() {
			if(View.IsNewItemRow(RowHandle)) return GetString(AccStringId.GridNewItemRow);
			int vindex = View.GetVisibleIndex(RowHandle);
			if(vindex < 0) return null;
			return string.Format(GetString(AccStringId.GridRow), vindex + 1);
		}
		public virtual string GetDefaultAction() { 
			if(RowHandle == View.FocusedRowHandle) return null;
			return GetString(AccStringId.GridRowActivate);
		}
		public virtual void DoDefaultAction() { 
			View.FocusedRowHandle = RowHandle;
		}
		public virtual string GetValue() { return null; }
		public virtual int CellCount { get { return 0; } } 
		public virtual IAccessibleGridRowCell GetCell(int index) { return null; } 
		public virtual BaseAccessible GetChildTable() { return null; }
		public virtual int GetChildTableIndex() { return 0; }
	}
	public class CardAccessibleRow : ColumnAccessibleRow {
		public CardAccessibleRow(CardView view, int rowHandle) : base(view, rowHandle) { }
		public new CardView View { get { return base.View as CardView; } }
		public override Rectangle Bounds {
			get {
				CardInfo ci = View.ViewInfo.Cards.CardInfoByRow(RowHandle);
				if(ci != null) return ci.Bounds;
				return Rectangle.Empty;
			}
		}
		public override string GetValue() { return View.GetCardCaption(RowHandle); }
		public override string GetDefaultAction() { 
			if(!View.OptionsBehavior.AllowExpandCollapse) return base.GetDefaultAction();
			return View.GetCardCollapsed(RowHandle) ? GetString(AccStringId.GridCardExpand) : GetString(AccStringId.GridCardCollapse);
		}
		public override void DoDefaultAction() { 
			if(!View.OptionsBehavior.AllowExpandCollapse)
				base.DoDefaultAction();
			else {
				View.SetCardCollapsed(RowHandle, !View.GetCardCollapsed(RowHandle));
			}
		}
		protected override AccessibleStates GetAccessibleState() {
			AccessibleStates state = base.GetAccessibleState();
			if(View.OptionsBehavior.AllowExpandCollapse) {
				state |=  !View.GetCardCollapsed(RowHandle) ? AccessibleStates.Expanded : AccessibleStates.Collapsed;
			}
			return state;
		}
		public override IAccessibleGridRowCell GetCell(int index) { 
			if(index >= View.VisibleColumns.Count) return null;
			return new CardAccessibleRowCell(View, RowHandle, View.VisibleColumns[index]);
		} 
		public override int CellCount { get { return View.VisibleColumns.Count; } }
	}
	public class GridAccessibleRow : ColumnAccessibleRow {
		public GridAccessibleRow(GridView view, int rowHandle) : base(view, rowHandle) { }
		public new GridView View { get { return base.View as GridView; } }
		protected override string GetAccessibleRowName() {
			if(View.IsFilterRow(RowHandle)) return GetString(AccStringId.GridFilterRow);
			return base.GetAccessibleRowName();
		}
		public override Rectangle Bounds {
			get {
				GridRowInfo row = View.ViewInfo.RowsInfo.GetInfoByHandle(RowHandle);
				return row != null ? row.Bounds : Rectangle.Empty;
			}
		}
		public virtual bool IsMasterRow { get { return false; } }
	}
	public class GridAccessibleDataRow : GridAccessibleRow {
		public GridAccessibleDataRow(GridView view, int rowHandle) : base(view, rowHandle) { }
		public override string GetDefaultAction() { 
			if(!IsMasterRow) return base.GetDefaultAction();
			if(View.GetMasterRowExpanded(RowHandle)) 
				return GetString(AccStringId.GridDataRowCollapse);
			if(View.IsMasterRowEmpty(RowHandle)) return null;
			return GetString(AccStringId.GridDataRowExpand);
		}
		public override void DoDefaultAction() { 
			if(!IsMasterRow)
				base.DoDefaultAction();
			else {
				if(View.GetMasterRowExpanded(RowHandle)) 
					View.CollapseMasterRow(RowHandle);
				else 
					View.ExpandMasterRow(RowHandle);
			}
		}
		public override string GetValue() {
			string val = string.Empty;
			for(int i = 0; i < CellCount; i++) {
				val += GetCell(i).GetValue() + ";";
			}
			return val;
		}
		protected override AccessibleStates GetAccessibleState() {
			AccessibleStates state = base.GetAccessibleState();
			if(IsMasterRow) {
				state |=  View.GetMasterRowExpanded(RowHandle) ? AccessibleStates.Expanded : AccessibleStates.Collapsed;
			}
			return state;
		}
		public override bool IsMasterRow { get { return View.IsMasterRow(RowHandle); } }
		public override IAccessibleGridRowCell GetCell(int index) { 
			if(index >= View.VisibleColumns.Count) return null;
			return new GridAccessibleRowCell(View, RowHandle, View.VisibleColumns[index]);
		} 
		public override int CellCount { get { return View.VisibleColumns.Count; } }
		public override BaseAccessible GetChildTable() { 
			if(IsMasterRow) {
				BaseView view = View.GetVisibleDetailView(RowHandle);
				if(view != null) return view.DXAccessible;
			}
			return null; 
		}
		public override int GetChildTableIndex() {
			BaseView view = View.GetVisibleDetailView(RowHandle);
			if(view == null)
				return 0;
			if(view.ParentInfo != null) return view.ParentInfo.RelationIndex;
			GridDetailInfo[] info = View.GetDetailInfo(RowHandle, false);
			for(int i = 0; i < info.Length; i++) {
				if(info[i].Caption == view.ViewCaption)
					return i;
			}
			return 0;
		}
	}
	public class DetailViewCollectionAccessible : BaseAccessible {
		public DetailViewCollectionAccessible(GridAccessibleRow owner) : base() {
			GridRow = owner;
		}
		protected internal GridAccessibleRow GridRow { get; set; }
		protected override AccessibleRole GetRole() {
			return AccessibleRole.PageTabList;
		}
		public virtual int GetDetailViewCount() {
			if(GridRow.IsMasterRow)
				return GridRow.View.GetDetailInfo(GridRow.RowHandle, true).Length;
			return 0;
		}
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo res = base.GetChildrenInfo();
			if(res == null)
				res = new ChildrenInfo(); 
			res["DetailViewCollection"] = GetDetailViewCount();
			return res;
		}
		protected GridDetailInfo GetInfoByIndex(int detailIndex) {
			GridDetailInfo[] info = GridRow.View.GetDetailInfo(GridRow.RowHandle, false);
			for(int i = 0; i < info.Length; i++) {
				if(info[i].RelationIndex == detailIndex)
					return info[i];
			}
			return null;
		}
		public virtual BaseAccessible GetDetailTable(int detailIndex) {
			if(GridRow.IsMasterRow) {
				return new DetailViewInfoAccessible(GetInfoByIndex(detailIndex), this);
			}
			return null;
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			int detailViewCount = GetDetailViewCount();
			for(int i = 0; i < detailViewCount; i++) {
				AddChild(GetDetailTable(i));
			}
		}
		public override Rectangle ClientBounds {
			get {
				ColumnViewAccessibleObject view = (ColumnViewAccessibleObject)GridRow.GetChildTable();
				if(view == null)
					return base.ClientBounds;
				return view.View.TabControl.ViewInfo.HeaderInfo.Bounds;
			}
		}
	}
	public class DetailViewInfoAccessible : BaseAccessible {
		public DetailViewInfoAccessible(GridDetailInfo info, DetailViewCollectionAccessible owner)
			: base() { 
			DetailInfo = info;
			Owner = owner;
		}
		protected DetailViewCollectionAccessible Owner { get; set; }
		protected GridDetailInfo DetailInfo { get; set; }
		protected override string GetName() {
			return DetailInfo.Caption;
		}
		public override string Value {
			get {
				return GetName();
			}
		}
		protected override AccessibleRole GetRole() {
			return AccessibleRole.PageTab;
		}
		protected override AccessibleStates GetState() {
			return AccessibleStates.Selectable;
		}
		protected override void DoDefaultAction() {
			Owner.GridRow.View.VisualSetMasterRowExpandedEx(Owner.GridRow.RowHandle, DetailInfo.RelationIndex, true);
			IAccessibleRowWithUpdate updateChildren = ParentCore.ParentCore as IAccessibleRowWithUpdate;
			if(updateChildren != null)
				updateChildren.UpdateChildren();
		}
		ViewTabPage GetPageByRelationIndex(int relationIndex) { 
			ColumnViewAccessibleObject view = (ColumnViewAccessibleObject)Owner.GridRow.GetChildTable();
			if(view == null)
				return null;
			foreach(ViewTabPage page in view.View.TabControl.Pages) {
				if(page.DetailInfo.RelationIndex == relationIndex)
					return page;
			}
			return null;
		}
		public override Rectangle ClientBounds {
			get {
				ColumnViewAccessibleObject view = (ColumnViewAccessibleObject)Owner.GridRow.GetChildTable();
				ViewTabPage page = GetPageByRelationIndex(DetailInfo.RelationIndex);
				if(view == null || page == null)
					return base.ClientBounds;
				return view.View.TabControl.ViewInfo.HeaderInfo.AllPages[page].Bounds;
			}
		}
	}
	public class CardAccessibleRowCell : ColumnViewAccessibleRowCell {
		public CardAccessibleRowCell(CardView view, int rowHandle, GridColumn column) : base(view, rowHandle, column) { }
		public new CardView View { get { return base.View as CardView; } }
		public override Rectangle Bounds {
			get {
				CardFieldInfo fi = View.ViewInfo.Cards.CardFieldInfoBy(RowHandle, Column);
				return fi != null ? fi.Bounds : Rectangle.Empty;
			}
		}
	}
	public class GridAccessibleRowCell : ColumnViewAccessibleRowCell {
		public GridAccessibleRowCell(GridView view, int rowHandle, GridColumn column) : base(view, rowHandle, column) { }
		public new GridView View { get { return base.View as GridView; } }
		public override Rectangle Bounds {
			get {
				GridDataRowInfo row = View.ViewInfo.RowsInfo.GetInfoByHandle(RowHandle) as GridDataRowInfo;
				if(row == null) return Rectangle.Empty;
				GridCellInfo cell = row.Cells[Column];
				return cell == null ? Rectangle.Empty : cell.Bounds;
			}
		}
	}
	public abstract class ColumnViewAccessibleRowCell : IAccessibleGridRowCell {
		ColumnView view;
		int rowHandle;
		GridColumn column;
		public ColumnViewAccessibleRowCell(ColumnView view, int rowHandle, GridColumn column) {
			this.view = view;
			this.rowHandle = rowHandle;
			this.column = column;
		}
		public string GetString(AccStringId id) { return AccLocalizer.Active.GetLocalizedString(id); }
		public int RowHandle { get { return rowHandle; } }
		public GridColumn Column { get { return column; } }
		public ColumnView View { get { return view; } }
		string IAccessibleGridRowCell.GetDefaultAction() { 
			if(IsReadOnly)
				return Column.OptionsColumn.AllowFocus ? GetString(AccStringId.GridCellFocus) : null;
			return GetString(AccStringId.GridCellEdit); 
		}
		public bool IsReadOnly {
			get {
				return !View.OptionsBehavior.Editable || !Column.OptionsColumn.AllowEdit || Column.OptionsColumn.ReadOnly;
			}
		}
		public abstract Rectangle Bounds { get; }
		public bool IsFocused { get { return View.FocusedRowHandle == RowHandle && View.FocusedColumn == Column; } }
		BaseAccessible IAccessibleGridRowCell.GetEdit() {
			if(View.IsEditing && View.ActiveEditor != null && IsFocused) return View.ActiveEditor.GetAccessible();
			return null;
		}
		void IAccessibleGridRowCell.DoDefaultAction() { 
			if(!Column.OptionsColumn.AllowFocus) return;
			View.FocusedRowHandle = RowHandle;
			if(View.FocusedRowHandle != RowHandle) return;
			View.FocusedColumn = Column;
			if(View.FocusedColumn != Column) return;
			View.ShowEditor();
		}
		string IAccessibleGridRowCell.GetName() {
			string name = Column.GetTextCaption() == "" ? Column.ToolTip : Column.GetTextCaption();
			if(rowHandle == GridControl.NewItemRowHandle)
				return name + " new item row";
			name += " row " + rowHandle.ToString();
			return name;
		}
		string IAccessibleGridRowCell.GetValue() {
			return View.GetRowCellDisplayText(RowHandle, Column);
		}
		AccessibleStates IAccessibleGridRowCell.GetState() { 
			AccessibleStates state = IsReadOnly ? AccessibleStates.ReadOnly : AccessibleStates.None;;
			state |= AccessibleStates.Selectable;
			if(Bounds.IsEmpty) {
				state |= AccessibleStates.Invisible;
			} else {
				if(Column.OptionsColumn.AllowFocus) state |= AccessibleStates.Focusable;
				if(IsFocused) state |= AccessibleStates.Focused;
			}
			return state;
		}
	}
	public class GridAccessibleGroupRow : GridAccessibleRow {
		public GridAccessibleGroupRow(GridView view, int rowHandle) : base(view, rowHandle) { }
		public bool Expanded { get { return View.GetRowExpanded(RowHandle); } }
		public override string GetDefaultAction() { 
			return Expanded ? GetString(AccStringId.GridRowCollapse) : GetString(AccStringId.GridRowExpand);
		}
		public override void DoDefaultAction() { 
			View.VisualExpandGroup(RowHandle, !Expanded);
		}
		protected override AccessibleStates GetAccessibleState() {
			AccessibleStates state = base.GetAccessibleState();
			state |=  View.GetRowExpanded(RowHandle) ? AccessibleStates.Expanded : AccessibleStates.Collapsed;
			return state;
		}
		public override string GetValue() { return View.GetGroupRowDisplayText(RowHandle); }
		public override int CellCount { get { return 0; } }
	}
}
