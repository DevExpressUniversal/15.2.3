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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using DevExpress.XtraGrid.EditForm.Helpers;
namespace DevExpress.XtraGrid.Views.Grid {
	public class PopupMenuShowingEventArgs : EventArgs {
		GridMenuType menuType;
		GridViewMenu menu;
		GridHitInfo hitInfo;
		Point point;
		bool allow;
		public PopupMenuShowingEventArgs(GridViewMenu menu, GridHitInfo hitInfo)
			:
			this(menu == null ? GridMenuType.User : menu.MenuType, menu, hitInfo, true) { }
		public PopupMenuShowingEventArgs(GridMenuType menuType, GridViewMenu menu, GridHitInfo hitInfo, bool allow)
			:
			this(menuType, menu, Point.Empty, allow) {
			this.hitInfo = hitInfo == null ? new GridHitInfo() : hitInfo;
			this.point = hitInfo.HitPoint;
		}
		public PopupMenuShowingEventArgs(GridMenuType menuType, GridViewMenu menu, Point point, bool allow) {
			this.menu = menu;
			this.menuType = menuType;
			this.point = point;
			this.allow = allow;
		}
		public GridHitInfo HitInfo { get { return hitInfo; } }
		public GridMenuType MenuType { get { return menuType; } }
		public bool Allow {
			get { return allow; }
			set { allow = value; }
		}
		public GridViewMenu Menu {
			get { return menu; }
			set { menu = value; }
		}
		public Point Point {
			get { return point; }
			set { point = value; }
		}
	}
	[Obsolete("You should use the 'PopupMenuShowingEventArgs' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class GridMenuEventArgs : PopupMenuShowingEventArgs {
		public GridMenuEventArgs(GridViewMenu menu, GridHitInfo hitInfo) :
			base(menu, hitInfo) { }
		public GridMenuEventArgs(GridMenuType menuType, GridViewMenu menu, GridHitInfo hitInfo, bool allow) :
			base(menuType, menu, hitInfo, allow) {
		}
		public GridMenuEventArgs(GridMenuType menuType, GridViewMenu menu, Point point, bool allow)
			: base(menuType, menu, point, allow) {
		}
	}
	public class RowHeightEventArgs : EventArgs {
		int rowHandle;
		int rowHeight;
		public RowHeightEventArgs(int rowHandle, int rowHeight) {
			this.rowHandle = rowHandle;
			this.rowHeight = rowHeight;
		}
		public int RowHandle { get { return rowHandle; } }
		public int RowHeight { 
			get { return rowHeight; } 
			set {
				if(value < -1) value = -1;
				rowHeight = value;
			}
		}
	}
	public delegate void RowHeightEventHandler(object sender, RowHeightEventArgs e);
	public class CustomMasterRowEventArgs : EventArgs {
		internal int rowHandle;
		internal int relationIndex;
		public CustomMasterRowEventArgs(int rowHandle, int relationIndex) {
			this.rowHandle = rowHandle;
			this.relationIndex = relationIndex;
		}
		public int RowHandle { get { return rowHandle; } }
		public int RelationIndex { get { return relationIndex; } }
	}
	public class MasterRowEmptyEventArgs : CustomMasterRowEventArgs {
		internal bool isEmpty;
		public MasterRowEmptyEventArgs(int rowHandle, int relationIndex, bool isEmpty) : base(rowHandle, relationIndex) {
			this.isEmpty = isEmpty;
		}
		public bool IsEmpty {
			get { return isEmpty; }
			set { isEmpty = value; }
		}
	}
	public class MasterRowGetRelationCountEventArgs : EventArgs {
		int relationCount;
		int rowHandle;
		public MasterRowGetRelationCountEventArgs(int rowHandle, int relationCount) {
			this.relationCount = relationCount;
			this.rowHandle = rowHandle;
		}
		public int RowHandle {
			get { return rowHandle; }
		}
		public int RelationCount {
			get { return relationCount; }
			set { relationCount = value; }
		}
	}
	public class MasterRowGetRelationNameEventArgs : CustomMasterRowEventArgs {
		string relationName;
		public MasterRowGetRelationNameEventArgs(int rowHandle, int relationIndex, string relationName) : base(rowHandle, relationIndex) {
			this.relationName = relationName;
		}
		public string RelationName {
			get { return relationName; }
			set { relationName = value; }
		}
	}
	public class MasterRowCanExpandEventArgs : CustomMasterRowEventArgs {
		internal bool allow;
		public MasterRowCanExpandEventArgs(int rowHandle, int relationIndex, bool allow) : base(rowHandle, relationIndex) {
			this.allow = allow;
		}
		public bool Allow {
			get { return allow; }
			set { allow = value; }
		}
	}
	public class MasterRowGetLevelDefaultViewEventArgs : CustomMasterRowEventArgs {
		BaseView defaultView;
		public MasterRowGetLevelDefaultViewEventArgs(int rowHandle, int relationIndex, BaseView defaultView) : base(rowHandle, relationIndex) {
			this.defaultView = defaultView;
		}
		public BaseView DefaultView {
			get { return defaultView; }
			set { defaultView = value; }
		}
	}
	public class MasterRowGetChildListEventArgs : CustomMasterRowEventArgs {
		IList childList;
		public MasterRowGetChildListEventArgs(int rowHandle, int relationIndex, IList childList) : base(rowHandle, relationIndex) {
			this.childList = childList;
		}
		public IList ChildList {
			get { return childList; }
			set { childList = value; }
		}
	}
	public class RowIndicatorCustomDrawEventArgs : RowObjectCustomDrawEventArgs {
		public RowIndicatorCustomDrawEventArgs(GraphicsCache cache, int rowHandle, ObjectPainter painter, IndicatorObjectInfoArgs info) : base(cache, rowHandle, painter, info, info.Appearance) {
		}
		public new IndicatorObjectInfoArgs Info { get { return base.Info as IndicatorObjectInfoArgs; } }
	}
	public class FooterCellCustomDrawEventArgs : RowCellObjectCustomDrawEventArgs {
		public FooterCellCustomDrawEventArgs(GraphicsCache cache, int rowHandle, GridColumn column, ObjectPainter painter, StyleObjectInfoArgs info) : base(cache, rowHandle, column, painter, info, info.Appearance) {
		}
		public new GridFooterCellInfoArgs Info { get { return base.Info as GridFooterCellInfoArgs; } }
	}
	public class ColumnHeaderCustomDrawEventArgs : CustomDrawObjectEventArgs {
		public ColumnHeaderCustomDrawEventArgs(GraphicsCache cache, HeaderObjectPainter painter, GridColumnInfoArgs info) : base(cache, painter, info, info.Appearance) {
		}
		public new GridColumnInfoArgs Info { get { return base.Info as GridColumnInfoArgs; } }
		public GridColumn Column { get { return Info.Column; } }
	}
	public delegate void FooterCellCustomDrawEventHandler(object sender, FooterCellCustomDrawEventArgs e);
	public delegate void ColumnHeaderCustomDrawEventHandler(object sender, ColumnHeaderCustomDrawEventArgs e);
	public delegate void RowIndicatorCustomDrawEventHandler(object sender, RowIndicatorCustomDrawEventArgs e);
	[Obsolete("You should use the 'PopupMenuShowingEventHandler' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void GridMenuEventHandler(object sender, GridMenuEventArgs e);
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	public delegate void MasterRowEmptyEventHandler(object sender, MasterRowEmptyEventArgs e);
	public delegate void MasterRowCanExpandEventHandler(object sender,  MasterRowCanExpandEventArgs e);
	public delegate void CustomMasterRowEventHandler(object sender,  CustomMasterRowEventArgs e);
	public delegate void MasterRowGetLevelDefaultViewEventHandler(object sender, MasterRowGetLevelDefaultViewEventArgs e);
	public delegate void MasterRowGetChildListEventHandler(object sender, MasterRowGetChildListEventArgs e);
	public delegate void MasterRowGetRelationNameEventHandler(object sender, MasterRowGetRelationNameEventArgs e);
	public delegate void MasterRowGetRelationCountEventHandler(object sender, MasterRowGetRelationCountEventArgs e);
	public class CalcPreviewTextEventArgs : EventArgs {
		int rowHandle, dataSourceRowIndex;
		object row;
		string previewText;
		[Obsolete]
		public CalcPreviewTextEventArgs(int rowHandle, string previewText) {
			this.rowHandle = rowHandle;
			this.previewText = previewText;
			this.dataSourceRowIndex = -1;
			this.row = null;
		}
		public CalcPreviewTextEventArgs(int dataSourceRowIndex, int rowHandle, object row, string previewText) {
			this.rowHandle = rowHandle;
			this.previewText = previewText;
			this.row = row;
			this.dataSourceRowIndex = dataSourceRowIndex;
		}
		public int DataSourceRowIndex { get { return dataSourceRowIndex; } }
		public int RowHandle { get { return rowHandle; } }
		public object Row { get { return row; } }
		public string PreviewText { 
			get { return previewText;}
			set { previewText = value; }
		}
	}
	public class FilterPopupListBoxEventArgs : EventArgs {
		GridColumn column;
		RepositoryItemComboBox comboBox;
		public FilterPopupListBoxEventArgs(GridColumn column, RepositoryItemComboBox comboBox) {
			this.column = column;
			this.comboBox = comboBox;
		}
		public GridColumn Column { get { return column; } }
		public RepositoryItemComboBox ComboBox { get { return comboBox; } }
	}
	public class FilterPopupCheckedListBoxEventArgs : EventArgs {
		GridColumn column;
		RepositoryItemCheckedComboBoxEdit comboBox;
		public FilterPopupCheckedListBoxEventArgs(GridColumn column, RepositoryItemCheckedComboBoxEdit comboBox) {
			this.column = column;
			this.comboBox = comboBox;
		}
		public GridColumn Column { get { return column; } }
		public RepositoryItemCheckedComboBoxEdit CheckedComboBox { get { return comboBox; } }
	}
	public class FilterPopupDateEventArgs : EventArgs {
		GridColumn column;
		List<FilterDateElement> list;
		public FilterPopupDateEventArgs(GridColumn column, List<FilterDateElement> list) {
			this.column = column;
			this.list = list;
		}
		public GridColumn Column { get { return column; } }
		public List<FilterDateElement> List { get { return list; } }
	}
	public class CustomFilterDialogEventArgs : EventArgs {
		GridColumn column;
		ColumnFilterInfo filterInfo;
		bool handled, useAsteriskAsWildcard;
		public CustomFilterDialogEventArgs(GridColumn column, ColumnFilterInfo filterInfo) {
			this.column = column;
			this.handled = false;
			this.FilterInfo = filterInfo;
			this.useAsteriskAsWildcard = true;
		}
		public ColumnFilterInfo FilterInfo {
			get { return filterInfo; }
			set { filterInfo = value; }
		}
		public bool Handled {
			get { return handled; }
			set { handled = value;}
		}
		public bool UseAsteriskAsWildcard {
			get { return useAsteriskAsWildcard; }
			set { useAsteriskAsWildcard = value; }
		}
		public GridColumn Column { get { return column; } }
	}
	public class GroupLevelStyleEventArgs : EventArgs {
		AppearanceObject levelAppearance;
		int level;
		public GroupLevelStyleEventArgs(int level, AppearanceObject levelAppearance) {
			this.levelAppearance = levelAppearance;
			this.level = level;
		}
		public AppearanceObject LevelAppearance {
			get { return levelAppearance; }
			set {
				if(value == null) return;
				levelAppearance = value;
			}
		}
		public int Level { get { return level; } }
	}
	public class RowStyleEventArgs : RowEventArgs {
		AppearanceObject appearance;
		GridRowCellState state;
		bool highPriority = false;
		public RowStyleEventArgs(int rowHandle, GridRowCellState state, AppearanceObject appearance) : base(rowHandle) {
			this.appearance = appearance;
			this.state = state;
		}
		public bool HighPriority { get { return highPriority; } set { highPriority = value; } }
		public GridRowCellState State { get { return state; } }
		public AppearanceObject Appearance { get { return appearance; } }
		internal void SetAppearance(AppearanceObject appearance) {
			this.appearance = appearance;
		}
		public void CombineAppearance(AppearanceObject appearance) {
			if(appearance == null || Appearance == appearance) return;
			AppearanceHelper.Combine(Appearance, appearance, Appearance);
		}
	}
	public class RowCellStyleEventArgs : CustomRowCellEventArgs {
		AppearanceObject appearance;
		internal GridRowCellState state;
		public RowCellStyleEventArgs(int rowHandle, GridColumn column, GridRowCellState state, AppearanceObject appearance) : base(rowHandle, column) {
			this.state = state;
			this.appearance = appearance;
		}
		public AppearanceObject Appearance { get { return appearance; } }
		internal void SetAppearance(AppearanceObject appearance) {
			this.appearance = appearance;
		}
		public void CombineAppearance(AppearanceObject appearance) {
			if(appearance == null || Appearance == appearance) return;
			AppearanceHelper.Combine(Appearance, appearance, Appearance);
		}
	}
	public class CustomRowCellEditEventArgs : CustomRowCellEventArgs {
		DevExpress.XtraEditors.Repository.RepositoryItem repositoryItem;
		public CustomRowCellEditEventArgs(int rowHandle, GridColumn column) : this(rowHandle, column, null) { }
		public CustomRowCellEditEventArgs(int rowHandle, GridColumn column, DevExpress.XtraEditors.Repository.RepositoryItem repositoryItem) : base(rowHandle, column) {
			this.repositoryItem = repositoryItem;
		}
		public DevExpress.XtraEditors.Repository.RepositoryItem RepositoryItem {
			get { return repositoryItem; }
			set { repositoryItem = value;
			}
		}
	}
	public class EditFormPreparedEventArgs : EventArgs {
		int rowHandle;
		Control panel;
		EditFormBindableControlsCollection bindableControls;
		public EditFormPreparedEventArgs(int rowHandle, Control panel, EditFormBindableControlsCollection bindableControls) {
			this.rowHandle = rowHandle;
			this.bindableControls = bindableControls;
			this.panel = panel;
		}
		public int RowHandle { get { return rowHandle; } }
		public EditFormBindableControlsCollection BindableControls { get { return bindableControls; } }
		public Control Panel { get { return panel; } }
		public bool FocusField(string fieldName) {
			return FocusInternal(BindableControls[fieldName]);
		}
		public bool FocusField(GridColumn column) {
			return FocusInternal(BindableControls[column]);
		}
		bool FocusInternal(Control control) {
			if(control == null) return false;
			control.Focus();
			((IContainerControl)Panel).ActivateControl(control);
			return true;
		}
	}
	public delegate void EditFormPreparedEventHandler(object sender, EditFormPreparedEventArgs e);
	public class EditFormShowingEventArgs : EventArgs {
		int rowHandle;
		bool allow = true;
		public EditFormShowingEventArgs(int rowHandle) {
			this.rowHandle = rowHandle;
		}
		public int RowHandle { get { return rowHandle; } }
		public bool Allow {
			get { return allow; }
			set { allow = value; }
		}
	}
	public delegate void EditFormShowingEventHandler(object sender, EditFormShowingEventArgs e);
	public class RowClickEventArgs : DXMouseEventArgs {
		int rowHandle;
		GridHitInfo hitInfo;
		internal RowClickEventArgs(DXMouseEventArgs e, GridHitInfo hitInfo)
			: this(e, hitInfo.RowHandle) {
			this.hitInfo = hitInfo;
		}
		public RowClickEventArgs(DXMouseEventArgs e, int rowHandle)
			: base(e) {
			this.rowHandle = rowHandle;
		}
		public int RowHandle { get { return rowHandle; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public GridHitInfo HitInfo { get { return hitInfo; } set { hitInfo = value; } }
	}
	public class RowCellClickEventArgs : RowClickEventArgs {
		GridColumn column;
		internal RowCellClickEventArgs(DXMouseEventArgs e, GridHitInfo hitInfo)
			: this(e, hitInfo.RowHandle, hitInfo.Column) {
		}
		public RowCellClickEventArgs(DXMouseEventArgs e, int rowHandle, GridColumn column)
			: base(e, rowHandle) {
			this.column = column;
		}
		public GridColumn Column { get { return column; } }
		public object CellValue { get { return ColumnView.GetRowCellValueCore(RowHandle, Column); } }
	}
	public class CellMergeEventArgs : EventArgs {
		int rowHandle1, rowHandle2;
		bool handled;
		bool merge;
		GridColumn column;
		public CellMergeEventArgs(int rowHandle1, int rowHandle2, GridColumn column) {
			Setup(rowHandle1, rowHandle2, column);
		}
		public int RowHandle1 { get { return rowHandle1; } }
		public int RowHandle2 { get { return rowHandle2; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public bool Merge {
			get { return merge; }
			set { merge = value; }
		}
		public GridColumn Column { get { return column; } }
		public object CellValue1 { get { return ColumnView.GetRowCellValueCore(RowHandle1, Column); } }
		public object CellValue2 { get { return ColumnView.GetRowCellValueCore(RowHandle2, Column); } }
		internal void Setup(int rowHandle1, int rowHandle2, GridColumn column) {
			this.rowHandle1 = rowHandle1;
			this.rowHandle2 = rowHandle2;
			this.column = column;
			this.handled = false;
			this.merge = false;
		}
	}
	public delegate void RowClickEventHandler(object sender, RowClickEventArgs e);
	public delegate void RowCellClickEventHandler(object sender, RowCellClickEventArgs e);
	public delegate void CellMergeEventHandler(object sender, CellMergeEventArgs e);
	public delegate void GroupLevelStyleEventHandler(object sender, GroupLevelStyleEventArgs e);
	public delegate void RowCellStyleEventHandler(object sender, RowCellStyleEventArgs e);
	public delegate void RowStyleEventHandler(object sender, RowStyleEventArgs e);
	public delegate void CustomRowCellEditEventHandler(object sender, CustomRowCellEditEventArgs e);
	public delegate void FilterPopupListBoxEventHandler(object sender, FilterPopupListBoxEventArgs e);
	public delegate void FilterPopupCheckedListBoxEventHandler(object sender, FilterPopupCheckedListBoxEventArgs e);
	public delegate void FilterPopupDateEventHandler(object sender, FilterPopupDateEventArgs e);
	public delegate void CalcPreviewTextEventHandler(object sender, CalcPreviewTextEventArgs e);
	public delegate void CustomFilterDialogEventHandler(object sender, CustomFilterDialogEventArgs e);
}
