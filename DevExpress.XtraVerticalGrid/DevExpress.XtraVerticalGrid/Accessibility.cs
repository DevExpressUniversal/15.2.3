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

using DevExpress.Accessibility;
using DevExpress.XtraVerticalGrid.Rows;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraVerticalGrid.ViewInfo;
namespace DevExpress.XtraVerticalGrid.Accessibility {
	public class GridAccessibleObject : BaseGridAccessibleObject {
		VGridControlBase grid;
		AccessibleGrid accessibleGrid;
		protected VGridControlBase Control { get { return grid; } }
		public GridAccessibleObject(VGridControlBase grid) {
			this.grid = grid;
			this.accessibleGrid = new AccessibleGrid(grid);
		}
		protected override BaseGridAccessibleObject.BaseGridHeaderPanelAccessibleObject CreateHeader() {
			return new HeaderPanelAccessibleObject(Grid);
		}
		protected override BaseGridAccessibleObject.BaseGridDataPanelAccessibleObject CreateRows() {
			return new DataPanelAccessibleObject(Grid);
		}
		protected override IAccessibleGrid Grid { get { return this.accessibleGrid; } }
		public override Control GetOwnerControl() { return Control; }
		public override Rectangle ClientBounds { get { return Control.ViewInfo.ViewRects.Client; } }
		#region protected classes
		protected class HeaderPanelAccessibleObject : BaseGridHeaderPanelAccessibleObject {
			public HeaderPanelAccessibleObject(IAccessibleGrid grid) : base(grid) { }
			public override Rectangle ClientBounds { get { return Control.ViewInfo.ViewRects.Window; } }
			protected VGridControlBase Control { get { return ((AccessibleGrid)Grid).Grid; } }
			protected override void OnChildrenCountChanged() {
				int rowPropertiesCount = AccessibleHelper.GetVisibleRowPropertiesCount(Control);
				for(int i = 0; i < rowPropertiesCount; i++) {
					AddChild(CreateHeader(AccessibleHelper.GetVisibleRowProperties(Control, i)));
				}
			}
			protected virtual BaseGridHeaderAccessibleObject CreateHeader(RowProperties rowProperties) {
				return new BaseGridHeaderAccessibleObject(new AccessibleHeader(Control, rowProperties));
			}
			protected override AccessibleRole GetRole() {
				return AccessibleRole.Client;
			}
		}
		protected class DataPanelAccessibleObject : BaseGridDataPanelAccessibleObject {
			public DataPanelAccessibleObject(IAccessibleGrid grid) : base(grid) { }
			protected override BaseAccessible CreateChild(int index) {
				IAccessibleGridRow row = Grid.GetRow(index);
				if(row == null)
					return null;
				return new RecordAccessibleObject((AccessibleRecord)row);
			}
			public override Rectangle ClientBounds { get { return Control.ViewInfo.ViewRects.Window; } }
			protected VGridControlBase Control { get { return ((AccessibleGrid)Grid).Grid; } }
			protected override AccessibleRole GetRole() {
				return AccessibleRole.Client;
			}
			protected class RecordAccessibleObject : BaseGridRowAccessibleObject {
				public RecordAccessibleObject(AccessibleRecord record) : base(record) { }
				AccessibleRecord GridRow { get { return Row as AccessibleRecord; } }
			}
		}
		#endregion protected classes
	}
	public class AccessibleHeader : IAccessibleGridHeaderCell {
		VGridControlBase grid;
		RowProperties rowProperties;
		public AccessibleHeader(VGridControlBase view, RowProperties rowProperties) {
			this.grid = view;
			this.rowProperties = rowProperties;
		}
		public VGridControlBase Grid { get { return grid; } }
		public RowProperties RowProperties { get { return rowProperties; } }
		public virtual Rectangle Bounds {
			get {
				BaseRowViewInfo info = Grid.ViewInfo[RowProperties.Row];
				if(info == null)
					return Rectangle.Empty;
				return ((RowCaptionInfo)info.HeaderInfo.CaptionsInfo[RowProperties.CellIndex]).CaptionRect;
			}
		}
		public virtual string GetDefaultAction() { return null; }
		public virtual void DoDefaultAction() { }
		public virtual string GetName() {
			return RowProperties.GetTextCaption();
		}
		public virtual AccessibleStates GetState() {
			if(!RowProperties.Row.Visible)
				return AccessibleStates.Invisible;
			return AccessibleStates.Default;
		}
	}
	public class AccessibleGrid : IAccessibleGrid {
		VGridControlBase grid;
		public AccessibleGrid(VGridControlBase grid) {
			this.grid = grid;
		}
		public VGridControlBase Grid { get { return grid; } }
		#region IAccessibleGrid Members
		public DevExpress.XtraEditors.ScrollBarBase HScroll {
			get { return Grid.Scroller.ScrollInfo.HScrollVisible ? grid.Scroller.ScrollInfo.HScroll : null; }
		}
		public int HeaderCount { get { return AccessibleHelper.GetVisibleRowPropertiesCount(Grid); } }
		public int RowCount { get { return Grid.ViewInfo.VisibleValuesCount; } }
		public int SelectedRow { get { return Grid.FocusedRecord; } }
		public DevExpress.XtraEditors.ScrollBarBase VScroll {
			get { return Grid.Scroller.ScrollInfo.VScrollVisible ? grid.Scroller.ScrollInfo.VScroll : null; }
		}
		public int FindRow(int x, int y) {
			throw new System.Exception("The method or operation is not implemented.");
		}
		public IAccessibleGridRow GetRow(int index) {
			return new AccessibleRecord(Grid, AccessibleHelper.ToRecordIndex(Grid, index));
		}
		#endregion
	}
	public class AccessibleRecord : IAccessibleGridRow {
		int rowHandle;
		VGridControlBase grid;
		public AccessibleRecord(VGridControlBase grid, int rowHandle) {
			this.rowHandle = rowHandle;
			this.grid = grid;
		}
		public VGridControlBase Grid { get { return grid; } }
		public int RowHandle { get { return rowHandle; } set { rowHandle = value; } }
		#region IAccessibleGridRow Members
		public virtual Rectangle Bounds { get { return Grid.ViewInfo.ViewRects.Window; } }
		public virtual int CellCount { get { return AccessibleHelper.GetVisibleRowPropertiesCount(Grid); } }
		public virtual void DoDefaultAction() {
			Grid.FocusedRecord = RowHandle;
		}
		public virtual IAccessibleGridRowCell GetCell(int index) {
			return new AccessibleRecordCell(Grid, RowHandle, AccessibleHelper.GetVisibleRowProperties(Grid, index));
		}
		public virtual string GetDefaultAction() {
			if(RowHandle == Grid.FocusedRecord)
				return null;
			return AccessibleHelper.GetString(AccStringId.GridRowActivate);
		}
		public virtual string GetName() {
			if(RowHandle < 0)
				return null;
			return string.Format("Record {0}", RowHandle);
		}
		public virtual AccessibleStates GetState() {
			return AccessibleStates.Default;
		}
		public virtual string GetValue() {
			string val = string.Empty;
			for(int i = 0; i < CellCount; i++) {
				val += GetCell(i).GetValue() + ";";
			}
			return val;
		}
		public virtual int Index { get { return RowHandle; } }
		#endregion
	}
	public class AccessibleRecordCell : IAccessibleGridRowCell {
		VGridControlBase grid;
		int rowHandle;
		RowProperties rowProperties;
		public AccessibleRecordCell(VGridControlBase grid, int rowHandle, RowProperties rowProperties) {
			this.grid = grid;
			this.rowHandle = rowHandle;
			this.rowProperties = rowProperties;
		}
		public int RowHandle { get { return rowHandle; } }
		public RowProperties RowProperties { get { return rowProperties; } }
		public VGridControlBase Grid { get { return grid; } }
		string IAccessibleGridRowCell.GetDefaultAction() {
			if(IsReadOnly)
				return RowProperties.Row.OptionsRow.AllowFocus ? AccessibleHelper.GetString(AccStringId.GridCellFocus) : null;
			return AccessibleHelper.GetString(AccStringId.GridCellEdit);
		}
		public bool IsReadOnly {
			get {
				return !Grid.OptionsBehavior.Editable || !RowProperties.GetReadOnly();
			}
		}
		public virtual Rectangle Bounds { get { return Rectangle.Empty; } }
		public bool IsFocused { get { return Grid.FocusedRecord == RowHandle && Grid.FocusedRow == RowProperties.Row && Grid.FocusedRecordCellIndex == RowProperties.CellIndex; } }
		BaseAccessible IAccessibleGridRowCell.GetEdit() {
			if(Grid.ActiveEditor != null && IsFocused)
				return Grid.ActiveEditor.GetAccessible();
			return null;
		}
		void IAccessibleGridRowCell.DoDefaultAction() {
			if(!RowProperties.Row.OptionsRow.AllowFocus)
				return;
			Grid.FocusedRecord = RowHandle;
			if(Grid.FocusedRecord != RowHandle)
				return;
			Grid.FocusedRow = RowProperties.Row;
			if(Grid.FocusedRow != RowProperties.Row)
				return;
			Grid.FocusedRecordCellIndex = RowProperties.CellIndex;
			if(Grid.FocusedRecordCellIndex != RowProperties.CellIndex)
				return;
			Grid.ShowEditor();
		}
		string IAccessibleGridRowCell.GetName() {
			return RowProperties.GetTextCaption() + " Record " + (rowHandle).ToString();;
		}
		string IAccessibleGridRowCell.GetValue() {
			return Grid.GetCellDisplayText(RowProperties, RowHandle);
		}
		AccessibleStates IAccessibleGridRowCell.GetState() {
			AccessibleStates state = IsReadOnly ? AccessibleStates.ReadOnly : AccessibleStates.None;
			state |= AccessibleStates.Selectable;
			if(Bounds.IsEmpty) {
			} else {
				if(RowProperties.Row.OptionsRow.AllowFocus)
					state |= AccessibleStates.Focusable;
				if(IsFocused)
					state |= AccessibleStates.Focused;
			}
			return state;
		}
	}
	class AccessibleHelper {
		public static string GetString(AccStringId id) { return AccLocalizer.Active.GetLocalizedString(id); }
		public static RowProperties GetVisibleRowProperties(VGridControlBase vGrid, int index) {
			int count = 0;
			foreach(BaseRow row in vGrid.VisibleRows) {
				int start = count;
				if(count <= index) {
					count += row.RowPropertiesCount;
				}
				if(index < count) {
					return row.GetRowProperties(index - start);
				}
			}
			return null;
		}
		public static int GetVisibleRowPropertiesCount(VGridControlBase vGrid) {
			int count = 0;
			foreach(BaseRow row in vGrid.VisibleRows) {
				count += row.RowPropertiesCount;
			}
			return count;
		}
		public static int ToRecordIndex(VGridControlBase vGrid, int index) {
			return vGrid.LeftVisibleRecord + index;
		}
	}
}
