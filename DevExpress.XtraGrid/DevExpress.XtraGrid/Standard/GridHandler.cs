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
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Menu;
using DevExpress.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Dragging;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraEditors;
namespace DevExpress.XtraGrid.Views.Grid.Handler {
	public class GridRowNavigator {
		GridHandler handler;
		public GridRowNavigator(GridHandler handler) {
			this.handler = handler;
		}
		protected virtual bool DoRowIndicatorClick(int rowHandle) {
			View.FocusedRowHandle = DownPointHitInfo.RowHandle;
			View.FocusedColumn = View.GetNearestCanFocusedColumn(DownPointHitInfo.Column, 0, false);
			if(!View.IsMultiSelect) return false;
			if(!View.OptionsSelection.UseIndicatorForSelection) return false;
			if((Control.ModifierKeys & (Keys.Control | Keys.Alt | Keys.Shift)) != 0) return false;
			View.StartAccessSelection();
			return true;
		}
		protected GridHitInfo DownPointHitInfo { get { return Handler.DownPointHitInfo; } }
		protected GridHandler Handler { get { return handler; } }
		protected GridView View { get { return Handler.View; } }
		public virtual bool OnProcessKey(KeyEventArgs e) { return false; }
		public virtual void OnKeyDown(KeyEventArgs e) { 
			if(View.IsDefaultState && (e.KeyData == (Keys.C | Keys.Control) || e.KeyData == (Keys.Insert | Keys.Control))) {
				View.CopyToClipboard();
			}
		}
		public virtual void OnKeyUp(KeyEventArgs e) { }
		public virtual bool OnMouseDown(GridHitInfo hitInfo, DXMouseEventArgs e) { return false; }
		public virtual bool OnMouseUp(GridHitInfo hitInfo, DXMouseEventArgs e) { return false; }
		public virtual void OnKeyPress(KeyPressEventArgs e) { }
		static Keys[] NavKeys = { Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp,
							 Keys.End, Keys.Home, Keys.Tab};
		internal static bool IsNavKey(Keys key) {
			return Array.IndexOf(NavKeys, key) != -1;
		}
		protected virtual bool IsNavigationKey(Keys key) {
			return IsNavKey(key);
		}
	}
	public class GridGroupRowNavigator : GridRowNavigator {
		public GridGroupRowNavigator(GridHandler handler) : base(handler) { }
		public override bool OnMouseUp(GridHitInfo hitInfo, DXMouseEventArgs e) { 
			if(View.IsDefaultState && Control.ModifierKeys == Keys.None) {
				if(View.IsMultiSelect && hitInfo.HitTest != GridHitTest.RowIndicator) {
					if(View.SelectedRowsCount != 1 || !View.IsRowSelected(View.FocusedRowHandle)) {
						if(!View.IsShowCheckboxSelectorInGroupRow) {
							if(View.CheckboxSelectorColumnFocusChangeSelection || View.CheckboxSelectorColumn == null) {
								View.ClearSelectFocusedRow();
							}
						}
					}
					return false;
				}
			}
			View.SetDefaultState();
			return false;
		}
		public override bool OnMouseDown(GridHitInfo hitInfo, DXMouseEventArgs e) { 
			if(e.Clicks > 1) { 
				View.VisualExpandGroup(DownPointHitInfo.RowHandle, !View.GetRowExpanded(DownPointHitInfo.RowHandle));
				return true;
			}
			int prevFocusedRowHandle = View.FocusedRowHandle;
			bool allowFireAfterMove = true;
			try {
				if(e.Button == MouseButtons.Left && DownPointHitInfo.HitTest == GridHitTest.RowIndicator) {
					if(DoRowIndicatorClick(DownPointHitInfo.RowHandle)) {
						allowFireAfterMove = false;
						return true;
					}
				}
				else {
					View.FocusedRowHandle = DownPointHitInfo.RowHandle;
				}
			}
			finally {
				if(allowFireAfterMove)
					View.DoAfterMoveFocusedRow(BaseViewHandler.FromMouseEventArgs(e), prevFocusedRowHandle, null, DownPointHitInfo, DownPointHitInfo.HitTest != GridHitTest.RowIndicator);
			}
			if(e.Button == MouseButtons.Left && View.IsDefaultState) {
				if(DownPointHitInfo.HitTest == GridHitTest.RowGroupButton) {
					View.VisualExpandGroup(DownPointHitInfo.RowHandle, !View.GetRowExpanded(DownPointHitInfo.RowHandle));
					return true;
				}
				if(DownPointHitInfo.HitTest == GridHitTest.RowGroupCheckSelector) {
					View.ChangeGroupRowSelection(DownPointHitInfo.RowHandle);
					return true;
				}
			}
			return false;
		}
		public override void OnKeyDown(KeyEventArgs e) { 
			base.OnKeyDown(e);
			if(View.IsDefaultState) {
				if(e.KeyCode == Keys.F5) {
					View.RefreshData();
					return;
				}
			}
			if(e.KeyCode == Keys.Escape) {
				View.CancelUpdateCurrentRow();
				return;
			}
			if(e.KeyCode == Keys.Space) {
				if(View.IsShowCheckboxSelectorInGroupRow) {
					View.ChangeGroupRowSelection(View.FocusedRowHandle);
					return;
				}
			}
			if(e.KeyData == (Keys.A | Keys.Control)) {
				View.SelectAll();
				return;
			}
			if(e.KeyCode == Keys.Add || e.KeyCode == Keys.Subtract ||
				(e.KeyCode == Keys.Oemplus && e.Control) || (e.KeyCode == Keys.OemMinus && e.Control)) {
				View.VisualExpandGroup(View.FocusedRowHandle, e.KeyCode == Keys.Add || (e.KeyCode == Keys.Oemplus && e.Control));
				return;
			}
			int delta = 0;
			switch(e.KeyCode) {
				case Keys.Tab:
					if(View.OptionsNavigation.AutoMoveRowFocus) {
						if(e.Shift)
							View.FocusedColumn = View.GetNearestCanFocusedColumn(View.GetVisibleColumn(View.VisibleColumns.Count - 1), 0, false, e); 
						else
							View.FocusedColumn = View.GetNearestCanFocusedColumn(View.GetVisibleColumn(0), 0, false, e); 
						delta = e.Shift ? -1 : 1;
					}
					break;
				case Keys.Left: 
				case Keys.Right:
					if(View.GetRowExpanded(View.FocusedRowHandle) != (e.KeyCode == Keys.Right)) {
						View.VisualExpandGroup(View.FocusedRowHandle, e.KeyCode == Keys.Right);
					} else {
						if(!View.IsFirstRow || e.KeyCode != Keys.Left) {
							delta = e.KeyCode == Keys.Left ? -1 : 1;
							View.FocusedColumn = View.GetNearestCanFocusedColumn(View.GetVisibleColumn(delta < 0 ? View.VisibleColumns.Count - 1 : 0), 0, false);
						}
					}
					break;
				case Keys.Up:
					if(View.OptionsView.ShowAutoFilterRow && e.Control)
						View.FocusedRowHandle = GridControl.AutoFilterRowHandle;
					else
						delta = -1;
					break;
				case Keys.Down:
					delta = 1; break;
				case Keys.PageUp:
					View.MovePrevPage();
					break;
				case Keys.PageDown:
					View.MoveNextPage();
					break;
				case Keys.Home:
					View.MoveFirst();
					break;
				case Keys.End:
					View.MoveLastVisible();
					break;
			}
			if(delta != 0 && View != null)
				View.DoMoveFocusedRow(delta, e);
		}
	}
	public class GridNewRowNavigator : GridRegularRowNavigator {
		public GridNewRowNavigator(GridHandler handler) : base(handler) { }
		protected override void SelectAll() { 	}
		protected override void DoNavigation(KeyEventArgs e) {
			if(e.KeyData == Keys.Down) {
				View.CloseEditor();
				bool IsModified = View.FocusedRowModified;
				if(!View.UpdateCurrentRow()) return;
				View.FocusedRowHandle = CurrencyDataController.NewItemRow;
				View.CheckViewInfo();
				View.MakeRowVisible(View.FocusedRowHandle, true);
				if(View.IsDetailView && View.OptionsView.NewItemRowPosition == NewItemRowPosition.Bottom && !IsModified) {
					base.DoNavigation(e);
				}
				return;
			}
			base.DoNavigation(e);
		}
		public override void OnKeyDown(KeyEventArgs e) {
			if((e.KeyData == Keys.Tab || e.KeyCode == Keys.Enter) && (View.IsDefaultState || View.IsEditing)) {
				if(View.IsEditing || e.KeyData == Keys.Tab 
					|| (View.FocusedColumn != null && !View.GetCanShowEditor(View.FocusedColumn))) {
					View.CloseEditor();
					if(e.KeyCode == Keys.Enter) e = new GridHandler.KeyEnterTabEventArgs();
					GridColumn oldFocused = View.FocusedColumn;
					if(View.FocusedColumn != null) {
						View.FocusedColumn = View.GetNearestCanFocusedColumn(View.FocusedColumn, 1, false, e);
						if(oldFocused != View.FocusedColumn) {
							View.ShowEditor();
						}
						else {
							if(View.UpdateCurrentRow()) {
								View.CheckViewInfo();
								View.FocusedRowHandle = CurrencyDataController.NewItemRow;
								View.FocusedColumn = Handler.GetFirstCanFocusedColumn(e);
								if(View.IsDetailView) {
									View.SetTopRowIndexDirty();
									View.LayoutChangedSynchronized();
								}
								View.MakeRowVisible(View.FocusedRowHandle, false);
								View.ShowEditor();
							}
						}
						return;
					} else {
						Handler.DoMoveFocusedColumn(1, false, e);
					}
				}
				return;
			}
			base.OnKeyDown(e);
		}
		protected override bool KeyboardAutoMoveRowFocus { get { return true; } }
	}
	public class GridTopNewRowNavigator : GridNewRowNavigator {
		public GridTopNewRowNavigator(GridHandler handler) : base(handler) { }
		protected override int DoNavigationUp(KeyEventArgs e) {
			if(View.OptionsView.ShowAutoFilterRow && e.Control) {
				View.CloseEditor();
				if(!View.UpdateCurrentRow()) return 0;
				View.FocusedRowHandle = GridControl.AutoFilterRowHandle;
				return 0;
			}
			return 0;
		}
		protected override bool KeyboardAutoMoveRowFocus { get { return false; } }
		protected override void DoNavigation(KeyEventArgs e) {
			if(e.KeyData == Keys.Down) {
				View.CloseEditor();
				if(!View.UpdateCurrentRow()) return;
				View.TopRowIndex = 0;
				View.FocusedRowHandle = View.GetVisibleRowHandle(0);
				if(View.AllowChangeSelectionOnNavigation) View.SelectFocusedRowCore();
				return;
			}
			base.DoNavigation(e);
		}
	}
	public class GridFilterRowNavigator : GridRegularRowNavigator {
		public GridFilterRowNavigator(GridHandler handler) : base(handler) { }
		protected override int DoNavigationUp(KeyEventArgs e) { return 0; }
		public override bool OnProcessKey(KeyEventArgs e) { 
			if(e.KeyData == (Keys.Control | Keys.Delete)) {
				ClearFocusedColumnFilter();
				return true;
			}
			if(e.KeyCode == Keys.Enter) {
				if(View.ActiveEditor != null && !View.ActiveEditor.IsNeededKey(e))
					View.PostEditor();
			}
			return false; 
		}
		protected virtual void ClearFocusedColumnFilter() {
			View.BeginLockFocusedRowChange();
			try {
				if(View.FocusedColumn != null) View.HideEditor();
				if(View.FocusedColumn != null)
					View.FocusedColumn.FilterInfo = ColumnFilterInfo.Empty;
			} finally {
				View.EndLockFocusedRowChange();
			}
		}
		public override void OnKeyDown(KeyEventArgs e) { 
			if((e.KeyData == (Keys.Control | Keys.Delete)) || (e.KeyData == Keys.Delete && !View.IsEditing)) {
				ClearFocusedColumnFilter();
				return;
			}
			base.OnKeyDown(e);
		}
		protected override void SelectAll() { }
		protected override bool DoIncrementalSearch(KeyPressEventArgs e) { return false; }
		protected override bool KeyboardAutoMoveRowFocus { get { return false; } }
	}
	public class GridRegularRowNavigator : GridRowNavigator {
		public GridRegularRowNavigator(GridHandler handler) : base(handler) { 
		}
		protected virtual void SelectAll() {
			View.SelectAll();
		}
		public override bool OnMouseDown(GridHitInfo hitInfo, DXMouseEventArgs e) { 
			if(e.Clicks > 1) { 
				if(DownPointHitInfo.HitTest == GridHitTest.RowIndicator && View.IsMasterRow(DownPointHitInfo.RowHandle)) {
					bool expand = !View.GetMasterRowExpanded(DownPointHitInfo.RowHandle);
					View.VisualSetMasterRowExpandedEx(DownPointHitInfo.RowHandle, -1, expand);
				}
				return true;
			}
			int prevFocusedRowHandle = View.FocusedRowHandle;
			GridColumn prevFocusedColumn = View.FocusedColumn;
			if(e.Button == MouseButtons.Left && DownPointHitInfo.HitTest == GridHitTest.RowIndicator) {
				if(DoRowIndicatorClick(DownPointHitInfo.RowHandle)) return true;
			}
			else {
				View.FocusedRowHandle = DownPointHitInfo.RowHandle;
				View.FocusedColumn = View.GetNearestCanFocusedColumn(DownPointHitInfo.Column, 0, false);
			}
			View.DoAfterMoveFocusedRow(BaseViewHandler.FromMouseEventArgs(e), prevFocusedRowHandle, prevFocusedColumn, DownPointHitInfo, DownPointHitInfo.HitTest != GridHitTest.RowIndicator);
				if(e.Button == MouseButtons.Left && (View.IsDefaultState || View.IsEditing)) {
					if(DownPointHitInfo.HitTest == GridHitTest.CellButton) {
						View.HideEditor();
						View.VisualSetMasterRowExpandedEx(DownPointHitInfo.RowHandle, -1, !View.GetMasterRowExpanded(DownPointHitInfo.RowHandle));
						return true;
					}
				}
			bool isSameFocusedCell = prevFocusedColumn == View.FocusedColumn && prevFocusedRowHandle == View.FocusedRowHandle;
			if(e.Button == MouseButtons.Left && View.IsDefaultState) {
				if(Control.ModifierKeys != Keys.None) return true; 
					if(View.FocusedColumn == DownPointHitInfo.Column) {
					if((View.GetShowEditorMode() != EditorShowMode.MouseDown && (View.GetShowEditorMode() != EditorShowMode.MouseDownFocused || !isSameFocusedCell)) || (View.IsMultiSelect && View.SelectedRowsCount > 1 && View.FocusedColumn != View.CheckboxSelectorColumn)) {
						return true;
					}
					GridControl ctrl = View.GridControl;
					GridView view = View;
					if(DownPointHitInfo.ListSourceRowIndex == view.FocusedListSourceIndex) {
						view.ShowEditorOnMouse(true);
						if(view.IsEditing) ctrl.MouseCaptureOwner = null;
					}
					return true;
				}
			}
			return true;
		}
		public override bool OnMouseUp(GridHitInfo hitInfo, DXMouseEventArgs e) { 
			if(View.IsDefaultState && Control.ModifierKeys == Keys.None) {
				if(View.GetShowEditorMode() != EditorShowMode.MouseDown || (View.IsMultiSelect && hitInfo.HitTest != GridHitTest.RowIndicator)) {
					if(DownPointHitInfo.InRowCell && DownPointHitInfo.Column == hitInfo.Column && DownPointHitInfo.RowHandle == hitInfo.RowHandle && View.FocusedRowHandle == hitInfo.RowHandle) {
						if(View.SelectedRowsCount != 1 || !View.IsRowSelected(View.FocusedRowHandle)) {
							if(!View.IsCheckboxSelectorHitInfo(DownPointHitInfo)) {
								if(View.CheckboxSelectorColumn == null || View.OptionsSelection.ResetSelectionClickOutsideCheckboxSelector) {
									View.ClearSelectFocusedRow();
								}
							}
						}
						if((View.GetShowEditorMode() == EditorShowMode.Click || View.GetShowEditorMode() == EditorShowMode.MouseDownFocused) && !Handler.prevFocusedCell.Equals(new GridCell(hitInfo.RowHandle, hitInfo.Column))) return false;
						if(View.FocusedColumn != hitInfo.Column) return false;
						if(hitInfo.HitTest == GridHitTest.CellButton) return false;
						if(View.GetShowEditorMode() != EditorShowMode.MouseDown) View.ShowEditorOnMouse(false);
						return false;
					}
				}
			}
			if(View.IsDefaultState && View.WorkAsLookup && DownPointHitInfo.InRow && View.IsDataRow(DownPointHitInfo.RowHandle)) {
				e.Handled = true;
				if(View.IsRowLoaded(DownPointHitInfo.RowHandle)) {
					View.LookUpOwner.DoClosePopup();
				}
				return true;
			}
			if(View.IsDefaultState || View.IsEditing) return false;
			View.SetDefaultState();
			return false;
		}
		public override void OnKeyUp(KeyEventArgs e) { 
			base.OnKeyUp(e);
			bool prevEditors = this.shouldOpenEditorOnKeyUp;
			this.shouldOpenEditorOnKeyUp = false;
			if(IsNavigationKey(e.KeyCode)) {
				if(prevEditors) View.ShowEditor();
			}
		}
		bool shouldOpenEditorOnKeyUp = false;
		public override void OnKeyDown(KeyEventArgs e) { 
			base.OnKeyDown(e);
			if(e.KeyData == Keys.LWin) return; 
			if(View.State == GridState.IncrementalSearch) {
				if(DoIncrementalSearchKeyDown(e)) return;
			}
			bool prevEditorOpen = false;
			if(e.KeyCode == Keys.Escape) {
				if(View.IsEditing)
					View.HideEditorByKey();
				else 
					if(View.IsDefaultState) 
						View.CancelUpdateCurrentRow();
				return;
			}
			if(View.IsInplaceEditFormVisible) {
				View.FocusedEditForm();
				return;
			}
			if(View.IsDefaultState) {
				if(e.KeyData == (Keys.A | Keys.Control)) {
					SelectAll();
					return;
				}
			}
			if(View.IsDefaultState) {
				if(View.IsShowEditorKey(e.KeyData)) {
					View.ShowEditorByKey(e);
					return;
				}
			}
			if(View.IsDefaultState) {
				if(e.KeyCode == Keys.F5) {
					View.RefreshData();
					return;
				}
			}
			if(View.IsEditing) {
				if(e.KeyCode == Keys.F2) {
					View.HideEditor();
					return;
				}
			}
			if(View.IsDefaultState || View.IsEditing) {
				if(View.IsEditing) {
					prevEditorOpen = true;
					if(e.KeyCode == Keys.Enter) View.CloseEditor();
				}
				if(e.KeyCode == Keys.Enter) {
					if((prevEditorOpen || (View.FocusedColumn != null && !View.FocusedColumn.OptionsColumn.AllowEdit)) && View.OptionsNavigation.EnterMoveNextColumn) {
						Handler.DoMoveFocusedColumn(1, KeyboardAutoMoveRowFocus, new DevExpress.XtraGrid.Views.Grid.Handler.GridHandler.KeyEnterTabEventArgs()); 
						View.ShowEditor();
						return;
					}
					if(prevEditorOpen) {
						return;
					}
				}
				if(IsNavigationKey(e.KeyCode)) {
					DoNavigation(e);
					if(View == null) return; 
					if((this.shouldOpenEditorOnKeyUp || prevEditorOpen) && View.IsKeyboardFocused && !View.IsEditing) {
						this.shouldOpenEditorOnKeyUp = true;
					}
					return;
				} 
				View.HideEditor();
				if(e.KeyCode == Keys.Space && e.Control && View.IsMultiSelect) {
					View.InvertFocusedRowSelectionCore(null);
					return;
				} 
				if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.F2) {
					View.ShowEditor();
				}
				if(e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Subtract ||
					e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.OemMinus)) {
					if(View.IsMasterRow(View.FocusedRowHandle)) {
						View.VisualSetMasterRowExpandedEx(View.FocusedRowHandle, -1,
							e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus);
					}
				}
			}
		}
		protected virtual bool DoIncrementalSearchKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Escape) {
				View.SetDefaultState();
				return true;
			}
			if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.F2) {
				View.SetDefaultState();
				View.ShowEditor();
				return true;
			}
			if(IsNavigationKey(e.KeyCode)) {
				if(DoIncrementalSearchNavigation(e)) return true;
				return false;
			} 
			return true;
		}
		protected virtual bool DoIncrementalSearchNavigation(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Up :
				case Keys.Down :
					if(e.Modifiers == Keys.Control) {
						int newRow = View.FindRow(new FindRowArgs(View.FocusedRowHandle, View.FocusedColumn, View.IncrementalText, true, false, e.KeyCode == Keys.Down),
							delegate(object completed) {
								int row = (int)completed;
								if(row != GridControl.InvalidRowHandle) View.FocusedRowHandle = row; 
							});
						if(newRow != GridControl.InvalidRowHandle && newRow != BaseGridController.OperationInProgress) {
							View.FocusedRowHandle = newRow;
						}
						return true;
					}
					break;
			}
			View.SetDefaultState();
			return false;
		}
		protected virtual bool KeyboardAutoMoveRowFocus { get { return View.OptionsNavigation.AutoMoveRowFocus; } }
		protected virtual int DoNavigationUp(KeyEventArgs e) {
			if(View.IsFirstRow) {
				if(View.OptionsView.NewItemRowPosition == NewItemRowPosition.Top) {
					View.FocusedRowHandle = CurrencyDataController.NewItemRow;
					return 0;
				}
				if(View.OptionsView.ShowAutoFilterRow && e.Control) {
					View.FocusedRowHandle = GridControl.AutoFilterRowHandle;
					return 0;
				}
			}
			return -1;
		}
		protected virtual void DoNavigation(KeyEventArgs e) {
			int delta = 0;
			switch(e.KeyCode) {
				case Keys.Tab:
					Handler.DoMoveFocusedColumn(e.Shift ? -1 : 1, KeyboardAutoMoveRowFocus, e); break;
				case Keys.Left: 
					Handler.DoMoveFocusedColumn(-1, KeyboardAutoMoveRowFocus, e); break;
				case Keys.Right:
					Handler.DoMoveFocusedColumn(1, KeyboardAutoMoveRowFocus, e); break;
				case Keys.Up:
					delta = DoNavigationUp(e);
					break;
				case Keys.Down:
					if(View.IsLastRow) {
						View.CloseEditor();
						if(!View.UpdateCurrentRow()) return;
					}
					delta = 1; break;
				case Keys.PageUp:
					View.MovePrevPage();
					break;
				case Keys.PageDown:
					View.MoveNextPage();
					break;
				case Keys.Home:
					View.FocusedColumn = View.GetNearestCanFocusedColumn(View.GetVisibleColumn(0), 0, false); 
					if(e.Control) View.KeyMoveFirst();
					break;
				case Keys.End:
					View.FocusedColumn = View.GetNearestCanFocusedColumn(View.GetVisibleColumn(View.VisibleColumns.Count - 1), 0, false); 
					if(e.Control) View.KeyMoveLastVisible();
					break;
			}
			if(delta != 0 && View != null)
				View.DoMoveFocusedRow(delta, e);
		}
		static char[] keyPress = new char[] { (char)27, (char)9, (char)8, (char)3};
		public override void OnKeyPress(KeyPressEventArgs e) { 
			if(View.IsDefaultState) {
				if(View.FocusedColumn != null) {
					if(View.FocusedColumn == View.CheckboxSelectorColumn) {
						if(e.KeyChar == ' ' && Control.ModifierKeys == Keys.None) {
							View.InvertRowSelection(View.FocusedRowHandle);
							e.Handled = true;
							return;
						}
					}
					if(View.CanIncrementalSearch(View.FocusedColumn)) {
						if(DoIncrementalSearch(e)) return;
					}
				}
			}
			if(View.State == GridState.IncrementalSearch) {
				DoIncrementalSearch(e);
				return;
			}
			if(!View.IsEditing && Array.IndexOf(keyPress, e.KeyChar) == -1) {
				if(e.KeyChar == ' ' && Control.ModifierKeys == Keys.Control) return;
				if(e.KeyChar == 1 && Control.ModifierKeys == Keys.Control) return;
				View.ShowEditorByKeyPress(e);
			}
		}
		protected virtual bool DoIncrementalSearch(KeyPressEventArgs e) {
			string text = View.IncrementalText;
			if(e.KeyChar == 8) {
				if(text.Length < 2) {
					View.SetDefaultState();
					return true;
				}
				text = text.Substring(0, text.Length - 1);
			} else {
				if(e.KeyChar > 31)
					text += e.KeyChar;
			}
			return View.DoIncrementalSearch(text);
		}
	}
	public class GridHandler : BaseViewHandler {
		protected const int DragDeltaStart = 5;
		protected Cursor fSizingCursor;
		protected GridHitInfo fDownPointHitInfo;
		GridDragManager _dragManager;
		internal GridCell prevFocusedCell;
		DevExpress.XtraEditors.ViewInfo.EditHitInfo cellEditDownPointHitInfo;
		GridRowNavigator topNewRowNavigator, rowNavigator, groupRowNavigator, newRowNavigator, filterRowNavigator;
		public new GridView View { get { return base.View as GridView; } }
		public new GridViewInfo ViewInfo { get { return base.ViewInfo as GridViewInfo; } }
		public GridPainter Painter { get { return View.Painter as GridPainter; } }
		public GridHandler(GridView gridView) : base(gridView) { 
			this.prevFocusedCell = new GridCell(0, null);
			this.fSizingCursor = Cursors.Default;
			this.fDownPointHitInfo = null;
			this._dragManager = CreateDragManager();
			this.rowNavigator = CreateRowNavigator();
			this.groupRowNavigator = CreateGroupRowNavigator();
			this.topNewRowNavigator = CreateTopNewRowNavigator();
			this.newRowNavigator = CreateNewRowNavigator();
			this.filterRowNavigator = CreateFilterRowNavigator();
		}
		public override void Dispose() {
			base.Dispose();
			if(DragManager != null) DragManager.Dispose();
			this._dragManager = null;
		}
		protected virtual GridRowNavigator CreateRowNavigator() { return new GridRegularRowNavigator(this); }
		protected virtual GridRowNavigator CreateGroupRowNavigator() { return new GridGroupRowNavigator(this); }
		protected virtual GridRowNavigator CreateNewRowNavigator() { return new GridNewRowNavigator(this); }
		protected virtual GridRowNavigator CreateTopNewRowNavigator() { return new GridTopNewRowNavigator(this); }
		protected virtual GridRowNavigator CreateFilterRowNavigator() { return new GridFilterRowNavigator(this); }
		protected virtual GridDragManager CreateDragManager() { return new GridDragManager(View); }
		protected virtual GridRowNavigator RowNavigator { get { return rowNavigator; } }
		protected virtual GridRowNavigator GroupRowNavigator { get { return groupRowNavigator; } }
		protected virtual GridRowNavigator NewRowNavigator { get { return newRowNavigator; } }
		protected virtual GridRowNavigator TopNewRowNavigator { get { return topNewRowNavigator; } }
		protected virtual GridRowNavigator FilterRowNavigator { get { return filterRowNavigator; } }
		protected virtual GridRowNavigator GetRowNavigator(int rowHandle) {
			if(View.IsNewItemRow(rowHandle)) {
				if(View.OptionsView.NewItemRowPosition == NewItemRowPosition.Top) return TopNewRowNavigator;
				return NewRowNavigator;
			}
			if(View.IsFilterRow(rowHandle)) return FilterRowNavigator;
			if(View.IsGroupRow(rowHandle)) return GroupRowNavigator;
			return RowNavigator;
		}
		protected virtual Cursor SizingCursor { get { return fSizingCursor; } }
		public virtual object DragObject { get { return DragManager.DragObject; } }
		public virtual GridDragManager DragManager { get { return _dragManager; } }
		public GridHitInfo DownPointHitInfo { 
			get { 
				if(fDownPointHitInfo == null) fDownPointHitInfo = ViewInfo.CreateHitInfo();
				return fDownPointHitInfo; 
			} 
			set { fDownPointHitInfo = value; } }
		private Point PointToScreen(Point p) {
			return View.GridControl.PointToScreen(p);
		}
		protected virtual void RemoveFromGrouping(GridColumn column) {
			View.BeginSort();
			try {
				column.GroupIndex = -1;
			}
			finally {
				View.EndSort();
			}
		}
		protected virtual void DoBestFit(GridColumn column) {
			if(!View.CanResizeColumn(column)) return;
			column.BestFit();
		}
		protected virtual void DoStartColumnSizing(GridColumn column, Point p) {
			if(!View.CanResizeColumn(column)) return;
			Painter.HideSizerLine();
			Painter.CurrentSizerPos = Painter.StartSizerPos = p.X;
			View.SetStateCore(GridState.ColumnSizing);
			Painter.ReSizingObject = column;
			Painter.ShowSizerLine();
			View.SetCursor(this.fSizingCursor = Cursors.SizeWE);
		}
		protected virtual void DoStartRowDetailSizing(int rowHandle, Point p) {
			if(!View.CanResizeDetailRow(rowHandle)) return;
			Painter.HideSizerLine();
			Painter.CurrentSizerPos = Painter.StartSizerPos = p.Y;
			View.SetStateCore(GridState.RowDetailSizing);
			Painter.ReSizingObject = rowHandle;
			Painter.ShowSizerLine();
			View.SetCursor(this.fSizingCursor = Cursors.SizeNS);
		}
		protected virtual void DoStartRowSizing(int rowHandle, Point p) {
			if(!View.CanResizeRow(rowHandle)) return;
			Painter.HideSizerLine();
			Painter.CurrentSizerPos = Painter.StartSizerPos = p.Y;
			View.SetStateCore(GridState.RowSizing);
			Painter.ReSizingObject = rowHandle;
			Painter.ShowSizerLine();
			View.SetCursor(this.fSizingCursor = Cursors.SizeNS);
		}
		protected virtual void DoStartResize(GridHitInfo hi) {
			if(hi.HitTest == GridHitTest.RowDetailEdge) {
				DoStartRowDetailSizing(hi.RowHandle, hi.HitPoint);
			}
			if(hi.HitTest == GridHitTest.RowEdge) {
				DoStartRowSizing(hi.RowHandle, hi.HitPoint);
			}
			if(DownPointHitInfo.HitTest == GridHitTest.ColumnEdge) {
				DoStartColumnSizing(hi.Column, hi.HitPoint);
			}
		}
		protected virtual void DoSizing(Point p) {
			int newPosition = -10000;
			if(View.State == GridState.ColumnSizing) {
				GridColumn column = Painter.ReSizingObject as GridColumn;
				GridColumnInfoArgs ci = ViewInfo.ColumnsInfo[column];
				newPosition = ValidateColumnSizingPosition(p.X, ViewInfo.ViewRects.ColumnPanel, ci, View.GetColumnMinWidth(column));
			}
			if(View.State == GridState.RowDetailSizing || View.State == GridState.RowSizing) {
				newPosition = p.Y;
			}
			if(newPosition != -10000 && newPosition != Painter.CurrentSizerPos) {
				Painter.HideSizerLine();
				Painter.CurrentSizerPos = newPosition;
				Painter.ShowSizerLine();
			}
		}
		internal int ValidateColumnSizingPosition(int x, Rectangle panelBounds, DevExpress.Utils.Drawing.HeaderObjectInfoArgs ci, int minWidth) {
			if(View.IsRightToLeft) {
				if(x > panelBounds.Right - ViewInfo.ViewRects.IndicatorWidth) {
					x = panelBounds.Right - ViewInfo.ViewRects.IndicatorWidth;
				}
				if(ci != null) {
					x = Math.Min(x, ci.Bounds.Right - minWidth);
				}
			}
			else {
				if(x < panelBounds.Left + ViewInfo.ViewRects.IndicatorWidth) {
					x = panelBounds.Left + ViewInfo.ViewRects.IndicatorWidth;
				}
				if(ci != null) {
					x = Math.Max(x, ci.Bounds.Left + minWidth);
				}
			}
			return x;
		}
		protected virtual void DoCheckShowMenu() {
			GridViewMenu menu = null;
			if(DownPointHitInfo.InRow && DownPointHitInfo.HitTest != GridHitTest.RowGroupCell) {
				View.DoShowGridMenu(GridMenuType.Row, new GridViewMenu(View), DownPointHitInfo, true);
				return;
			}
			if(!View.UpdateCurrentRow()) return;
			if(View.OptionsMenu.EnableFooterMenu) {
				if(DownPointHitInfo.Column != null && View.CanDataSummaryColumn(DownPointHitInfo.Column) && (DownPointHitInfo.HitTest == GridHitTest.Footer || DownPointHitInfo.HitTest == GridHitTest.RowFooter || DownPointHitInfo.HitTest == GridHitTest.RowGroupCell)) {
					menu = CreateGridViewFooterMenu(View);
					menu.Init(DownPointHitInfo);
				}
			}
			if(View.OptionsMenu.EnableColumnMenu) {
				if(DownPointHitInfo.InColumnPanel || DownPointHitInfo.InColumn || DownPointHitInfo.InGroupColumn) {
					menu = CreateGridViewColumnMenu(View);
					menu.Init(DownPointHitInfo.Column);
				}
			}
			if(View.OptionsMenu.EnableGroupPanelMenu) {
				if(DownPointHitInfo.HitTest == GridHitTest.GroupPanel || 
					(DownPointHitInfo.HitTest == GridHitTest.Row && View.IsGroupRow(DownPointHitInfo.RowHandle))) {
					menu = CreateGridViewGroupPanelMenu(View);
					menu.Init(null);
				}
			}
			menu = CreateMenuEx(menu);
			View.DoShowGridMenu(menu, DownPointHitInfo);
		}
		protected virtual GridViewFooterMenu CreateGridViewFooterMenu(GridView gridView) {
			return new GridViewFooterMenu(gridView);
		}
		protected virtual GridViewColumnMenu CreateGridViewColumnMenu(GridView gridView) {
			return new GridViewColumnMenu(gridView);
		}
		protected virtual GridViewGroupPanelMenu CreateGridViewGroupPanelMenu(GridView gridView) {
			return new GridViewGroupPanelMenu(gridView);
		}
		protected virtual GridViewMenu CreateMenuEx(GridViewMenu menu) { return menu; }
		protected override void OnClick(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnClick(e);
			if(e.Handled || View == null) return;
			ProcessClickEvent(e);
		}
		protected override void OnDoubleClick(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnDoubleClick(e);
			if(e.Handled || View == null) return;
			ProcessClickEvent(e);
		}
		protected virtual void ProcessClickEvent(DXMouseEventArgs e) {
			GridHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(View.CanRaiseRowCellUserEvents) {
				bool theSameRow = (DownPointHitInfo.RowHandle == hitInfo.RowHandle);
				bool theSameRowAndColumn = theSameRow && (DownPointHitInfo.Column == hitInfo.Column);
				if(hitInfo.InRow && (e.Clicks > 1 || theSameRow)) View.RaiseRowClick(new RowClickEventArgs(e, hitInfo));
				if(hitInfo.InRowCell && (e.Clicks > 1 || theSameRowAndColumn)) View.RaiseRowCellClick(new RowCellClickEventArgs(e, hitInfo));
				if(e.Handled) return;
				if(hitInfo.InRow && e.Clicks > 1 && View.IsEditFormMode && !View.IsInplaceEditFormVisible) {
					if(View.OptionsEditForm.ShowOnDoubleClick != DefaultBoolean.False) View.ShowEditForm();
				}
			}
		}
		protected internal DevExpress.XtraEditors.ViewInfo.EditHitInfo CellEditDownPointHitInfo { get { return cellEditDownPointHitInfo; } }
		protected override bool OnMouseDown(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseDown(e);
			if(e.Handled) return true;
			Point p = new Point(e.X, e.Y);
			this.cellEditDownPointHitInfo = null;
			this.prevFocusedCell = new GridCell(View.FocusedRowHandle, View.FocusedColumn);
			this.fDownPointHitInfo = ViewInfo.CalcHitInfo(p);
			if(fDownPointHitInfo.HitTest != GridHitTest.RowDetail) 
				View.GridControl.FocusViewByMouse(View);
			if(View == null) return true;
			if((e.Button & MouseButtons.Left) != 0) { 
				View.GridControl.MouseCaptureOwner = View;
				if(DownPointHitInfo.CellInfo != null && DownPointHitInfo.CellInfo.ViewInfo != null) {
					this.cellEditDownPointHitInfo = DownPointHitInfo.CellInfo.ViewInfo.CalcHitInfo(View.GetCellPoint(DownPointHitInfo.CellInfo, p));
				}
				if(ViewInfo.SelectionInfo.CheckMouseDown(DownPointHitInfo)) return true;
				if(e.Clicks == 1 && CanResizeObject(DownPointHitInfo)) {
					DoStartResize(DownPointHitInfo);
					return true;
				}
				if(DownPointHitInfo.InRow) {
					View.SuspendImmediateUpdateRowPosition(DownPointHitInfo.InRowCell ? DownPointHitInfo.Column : null);
					try {
						bool res = GetRowNavigator(DownPointHitInfo.RowHandle).OnMouseDown(DownPointHitInfo, e);
						this.fDownPointHitInfo = ViewInfo.CalcHitInfo(p);
						return res;
					} finally {
						View.ResumeImmediateUpdateRowPosition();
					}
				}
				if(DownPointHitInfo.HitTest == GridHitTest.ColumnEdge) {
					if(e.Clicks > 1) 
						DoBestFit(DownPointHitInfo.Column);
				}
				if(DownPointHitInfo.HitTest == GridHitTest.EmptyRow) {
					if(!View.IsInplaceEditFormVisible) {
						View.UpdateCurrentRow();
					}
					else {
						if(e.Clicks == 1 && View.GetAllowCancelEditFormOnRowChange() && View.AskModifiedCancelAndHide()) return true;
					}
				}
				return true;
			}
			else {
				if(!View.IsDefaultState) {
					View.SetDefaultState();
				}
				if(e.Button == MouseButtons.Right) {
					if(DownPointHitInfo.InRow) {
						GetRowNavigator(DownPointHitInfo.RowHandle).OnMouseDown(DownPointHitInfo, e);
					}
					DoCheckShowMenu();
					return true;
				}
			}
			if(e.Button == MouseButtons.Middle && e.Clicks == 1) {
				if(ViewInfo.ViewRects.Rows.Contains(p)) {
					View.SetDefaultState();
					if(View.IsDefaultState) {
						View.GridControl.Focus();
						View.StartScrolling();
						return true;
					}
				}
			}
			return false;
		}
		protected override bool OnMouseUp(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseUp(e);
			if(e.Handled) return true;
			GridHitInfo hi = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
			GridState prevState = View.State;
			if((e.Button & MouseButtons.Left) != 0) {
				ViewInfo.SelectionInfo.CheckMouseUp(hi);
				if(View == null) return true; 
				if(View.IsSizingState) {
					View.EndSizingCore();
					View.SetDefaultState();
					return true;
				} 
				if(hi.InRow) {
					return GetRowNavigator(hi.RowHandle).OnMouseUp(hi, e);
				}
				if(View.IsDefaultState || View.IsEditing) return false;
				View.SetDefaultState();
			}
			else if((e.Button & MouseButtons.Right) != 0) {
				ViewInfo.SelectionInfo.ClearPressedInfo();
			}
			return false;
		}
		protected override bool AllowMouseWheelPixelScrollingHorz { get { return true; } }
		protected override bool AllowMouseWheelPixelScrollingVert { get { return View.IsPixelScrolling; } }
		protected override bool OnMouseWheel(MouseWheelScrollClientArgs e) {
			try {
				if(View.IsEditing) return false;
				if(View.FilterPopup != null) return true;
				if(e.Horizontal) {
					int distance = e.InPixels ? e.Distance : MouseHWheelScrollDelta * (e.Distance < 0 ? -1 : 1);
					View.DoMouseHWheelScroll(distance);
				}
				else
					View.DoMouseWheelScroll(e);
				return true;
			}
			finally {
				if(View != null && View.ScrollInfo != null && View.ScrollInfo.VScroll != null && View.ScrollInfo.VScroll.Visible) e.Handled = true;
			}
		}
		public static int MouseHWheelScrollDelta = 40;
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			Point p = View.PointToClient(Control.MousePosition);
			if(View.ViewRect.Contains(p)) View.firstMouseEnter = false; 
			GridHitInfo hi = ViewInfo.CalcHitInfo(p);
			ViewInfo.SelectionInfo.OnHotTrackEnter(hi);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(View.detailTip.Visible) {
				if(!View.detailTip.ClientRectangle.Contains(View.detailTip.PointToClient(Control.MousePosition))) {
					View.HideDetailTip();
				}
			}
			ViewInfo.SelectionInfo.OnHotTrackLeave();
		}
		protected virtual void DragMasterStart(object drag, Point p, Bitmap bmp) {
			View.GridControl.DragController.StartDragging(DragManager, new GridDragStartArgs(DownPointHitInfo, drag, bmp, PointToScreen(p), DragDropEffects.Copy));
		}
		protected virtual GridView GetRedirectView(BaseView view) {
			GridView gv = view as GridView;
			if(gv == null) return null;
			if(gv.SynchronizeClones && gv.IsLevelDefault) return gv;
			return null;
		}
		protected virtual void SyncHandlers(GridHandler toHandler) {
			toHandler.fDownPointHitInfo = DownPointHitInfo;
			View.GridControl.MouseCaptureOwner = null;
			View.GridControl.Capture = true;
		}
		public virtual void DoStartDragObject(object drag, Size size) {
			GridColumn column = drag as GridColumn;
			if(column != null) {
				if(column.View != View) {
					GridView red = GetRedirectView(column.View);
					if(red != null) {
						GridHandler gh = red.Handler as GridHandler;
						SyncHandlers(gh);
						gh.DoStartDragObject(drag, size);
						View.SetDefaultState();
					}
					return;
				}
				if(!View.RaiseDragObjectStart(new DragObjectStartEventArgs(column))) return;
				View.SetStateCore(GridState.ColumnDragging);
				DragMasterStart(drag, DownPointHitInfo.HitPoint, ((GridPainter)View.Painter).GetColumnDragBitmap(ViewInfo, drag as GridColumn, size, false, false));
			}
		}
		protected virtual object CanStartDragObject(GridHitInfo hi, Point offs) {
			if((offs.X > DragDeltaStart || offs.Y > DragDeltaStart) && DownPointHitInfo.Column != null && View.State == GridState.ColumnDown) {
				if(View.CanDragColumn(DownPointHitInfo.Column)) return DownPointHitInfo.Column;
			}
			return null;
		}
		protected virtual bool CanResizeObject(GridHitInfo hi) {
			if(!View.IsDefaultState && !View.IsEditing) return false;
			if(hi.HitTest == GridHitTest.ColumnEdge) {
				if(View.CanResizeColumn(hi.Column)) {
					this.fSizingCursor = Cursors.SizeWE;
					return true;
				}
			}
			if(hi.HitTest == GridHitTest.RowDetailEdge) {
				if(View.CanResizeDetailRow(hi.RowHandle)) {
					this.fSizingCursor = Cursors.SizeNS;
					return true;
				}
			}
			if(hi.HitTest == GridHitTest.RowEdge) {
				if(View.CanResizeRow(hi.RowHandle)) {
					this.fSizingCursor = Cursors.SizeNS;
					return true;
				}
			}
			return false;
		}
		GridHitInfo mouseMoveInfo = null;
		protected override bool OnMouseMove(MouseEventArgs ev) {
			if(View != null) View.OnActionScroll(XtraEditors.ScrollNotifyAction.MouseMove);
			if(View != null) View.firstMouseEnter = false; 
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseMove(e);
			if(e.Handled) return true;
			Point p = new Point(e.X, e.Y);
			GridHitInfo hi = ViewInfo.CalcHitInfo(p);
			GridHitInfo prevInfo = this.mouseMoveInfo == null ? hi : this.mouseMoveInfo;
			if(prevInfo.HitTest != hi.HitTest || prevInfo.RowHandle != hi.RowHandle || prevInfo.Column != hi.Column || this.mouseMoveInfo == null) this.mouseMoveInfo = hi;
			Point offs = new Point(Math.Abs(p.X - DownPointHitInfo.HitPoint.X), Math.Abs(p.Y - DownPointHitInfo.HitPoint.Y));
			if(View.IsSizingState) {
				View.SetCursor(SizingCursor);
				DoSizing(p);
				return true;
			}
			if(View.State == GridState.Selection || View.State == GridState.CellSelection) {
				int row = ViewInfo.GetNearestRowHandle(p);
				GridColumn column = ViewInfo.GetNearestColumn(p);
				bool sameColumn = prevInfo.Column == column;
				if(View.State == GridState.Selection ||
					(prevInfo.HitTest == GridHitTest.RowIndicator && hi.HitTest == GridHitTest.RowIndicator)) {
					column = null;
					sameColumn = true;
				}
				if(prevInfo == null || prevInfo.RowHandle != row || !sameColumn) View.UpdateAccessSelection(row, column);
				return false;
			}
			bool resetCursor = ViewInfo.SelectionInfo.CheckHotTrackMouseMove(hi);
			if((e.Button & MouseButtons.Left) != 0) {
				object drag = CanStartDragObject(hi, offs);
				if(drag != null) {
					DoStartDragObject(drag, Size.Empty);
					return true;
				}
				if(View.IsDefaultState && View.IsCellSelect) {
					int row = ViewInfo.GetNearestRowHandle(p);
					GridColumn column = ViewInfo.GetNearestColumn(p);
					if(DownPointHitInfo.InRowCell && (DownPointHitInfo.RowHandle != row || DownPointHitInfo.Column != column)) {
						View.StartAccessSelectionCore(DownPointHitInfo.RowHandle, DownPointHitInfo.Column, false);
						View.UpdateAccessSelection(row, column);
						return true;
					}
				}
			} else {
				if(CanResizeObject(hi)) {
					View.SetCursor(SizingCursor);
					resetCursor = false;
				}
			}
			if(resetCursor)
				View.ResetCursor();
			if(View.IsDefaultState && hi.HitTest == GridHitTest.CellButton) {
				View.ShowDetailTipCore(hi);
			}
			return false;
		}
		public override void DoClickAction(BaseHitInfo hitInfo) {
			GridHitInfo hit = hitInfo as GridHitInfo;
			if(hit.HitTest == GridHitTest.FilterPanelCloseButton) {
				View.ClearColumnsFilter();
				return;
			}
			if(hit.HitTest == GridHitTest.FilterPanelMRUButton || hit.HitTest == GridHitTest.FilterPanelText) {
				View.ShowMRUFilterPopup();
				return;
			}
			if(hit.HitTest == GridHitTest.FilterPanelCustomizeButton) {
				View.OnFilterCustomizeClick();
				return;
			}
			if(hit.HitTest == GridHitTest.FilterPanelActiveButton) {
				View.ActiveFilterEnabled = !View.ActiveFilterEnabled;
				return;
			}
			if(hit.HitTest == GridHitTest.Column || hit.HitTest == GridHitTest.GroupPanelColumn) {
				if(!View.GridControl.IsDesignMode) {
					if(hit.Column == View.CheckboxSelectorColumn) {
						if(View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader != DefaultBoolean.False)
							View.InvertAllRowsSelection();
						return;
					}
					DoMouseSortColumn(hit.Column, GridControl.ModifierKeys);
				}
				else {
					hit.Column.SelectInDesigner();
				}
			}
			if(View.IsDesignMode) return;
			if(hit.HitTest == GridHitTest.ColumnFilterButton) {
				View.ShowFilterPopup(hit.ColumnInfo);
			}
			if(hit.HitTest == GridHitTest.GroupPanelColumnFilterButton) {
				GridColumnInfoArgs ci = ViewInfo.GroupPanel.Rows.GetColumnInfo(hit.Column);
				if(ci != null)
					View.ShowFilterPopup(ci); 
			}
			if(hit.HitTest == GridHitTest.ColumnButton) {
				if(View.IsDetailView) {
					if(View.IsZoomedView)
						View.NormalView();
					else
						View.ZoomView();
				}
			}
		}
		protected virtual void DoMouseSortColumn(GridColumn column, Keys key) {
			if(column.View != View) {
				GridHandler hnd = column.View.Handler as GridHandler;
				hnd.DoMouseSortColumn(column, key);
				return;
			}
			if(!View.CanSortColumn(column)) return;
			View.DoMouseSortColumn(column, key);
			View.MakeRowVisible(View.FocusedRowHandle, true);
		}
		internal class KeyEnterTabEventArgs : KeyEventArgs {
			public KeyEnterTabEventArgs() : base(Keys.Tab) { }
		}
		public GridColumn GetFirstCanFocusedColumn(KeyEventArgs e) {
			var res =GetFirstCanFocusedColumnCore(e);
			if(res == null && e is KeyEnterTabEventArgs) res = GetFirstCanFocusedColumn(new KeyEventArgs(Keys.Enter));
			return res;
		}
		public virtual GridColumn GetFirstCanFocusedColumnCore(KeyEventArgs e) {
			for(int n = 0; n < View.VisibleColumns.Count; n ++) {
				GridColumn col = View.GetVisibleColumn(n);
				if(!col.OptionsColumn.AllowFocus) continue;
				if(e.KeyCode == Keys.Tab && !col.OptionsColumn.TabStop) continue;
				return col;
			}
			return null;
		}
		protected virtual bool CheckLeaveFocusOnTab(KeyEventArgs e) {
			if(e.Control || (e.KeyCode != Keys.Tab)) return false;
			if(View.OptionsBehavior.FocusLeaveOnTab) {
				View.GridControl.ProcessControlTab(!e.Shift);
				return true;
			}
			return false;
		}
		public virtual void DoMoveFocusedColumn(int delta, bool autoMoveRowFocus, KeyEventArgs e) {
			if(View.FocusedRowHandle == GridControl.InvalidRowHandle) {
				View.DoMoveFocusedRow(1, e);
				return;
			}
			if(View.FocusedColumn == null) {
				GridColumn column = GetFirstCanFocusedColumn(e);
				if(column == null && CheckLeaveFocusOnTab(e)) return;
				View.FocusedColumn = column;
				View.SelectFocusedCellCore(e);
				return;
			}
			GridCell anchor = new GridCell(View.FocusedRowHandle, View.FocusedColumn);
			View.FocusedColumn = View.GetNearestCanFocusedColumn(View.FocusedColumn, delta, autoMoveRowFocus || 
				AutoMoveRowFocusForAutoFilterRow, e);
			View.SelectFocusedCellRangeCore(anchor, new GridCell(View.FocusedRowHandle, View.FocusedColumn), e);
		}
		bool AutoMoveRowFocusForAutoFilterRow { 
			get {
				return View.FocusedRowHandle == GridControl.AutoFilterRowHandle && View.RowCount == 0;
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			if(View.IsDraggingState || View.IsSizingState) {
				View.SetDefaultState();
				return;
			}
			if(View.CheckShowFindPanelKey(e)) return;
			KeyEventArgs ee = TranslateRTLKeys(e);
			GetRowNavigator(View.FocusedRowHandle).OnKeyDown(ee);
			e.Handled = ee.Handled;
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.Handled) return;
			GetRowNavigator(View.FocusedRowHandle).OnKeyUp(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.KeyChar == 6 && (View.FindPanel != null || Grid.IsAttachedToSearchControl)) {
				e.Handled = true;
			}
			if(e.Handled) return;
			GetRowNavigator(View.FocusedRowHandle).OnKeyPress(e);
		}
		protected override void OnResize(Rectangle clientRect) {
			View.OnActionScroll(XtraEditors.ScrollNotifyAction.Resize);
			View.SetDefaultState();
			View.InternalSetViewRectCore(clientRect);
			base.OnResize(clientRect);
		}
		public override bool RequireMouse(MouseEventArgs e) {
			GridHitInfo hitInfo = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
			if(hitInfo.HitTest == GridHitTest.RowDetailEdge) {
				return true;
			}
			if(View.IsDraggingState) return true;
			return base.RequireMouse(e);
		}
		public override bool ProcessKey(KeyEventArgs e) {
			if(View.FilterPopup != null) return false;
			if(GetRowNavigator(View.FocusedRowHandle).OnProcessKey(e)) return true;
			if(View.ActiveEditor != null) {
				DevExpress.XtraEditors.BaseEdit be = View.ActiveEditor as DevExpress.XtraEditors.BaseEdit;
				if(be.IsNeededKey(e)) return false;
			}
			if(e.KeyData == (Keys.Tab | Keys.Control) || (Control.ModifierKeys & Keys.Alt) != 0) return false;
			if(GridRowNavigator.IsNavKey(e.KeyCode) || e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter) {
				OnKeyDown(e);
				if(View != null && View.IsDefaultState && e.KeyData == Keys.Escape) return false;
				return true;
			}
			return false;
		}
		public override bool NeedKey(NeedKeyType keyType) { 
			switch(keyType) {
				case NeedKeyType.Tab : return View.OptionsNavigation.UseTabKey;
				case NeedKeyType.Escape : return View.IsEditing || View.FocusedRowModified; 
				case NeedKeyType.Enter : return View.IsEditing;
				case NeedKeyType.Dialog: return View.IsEditing;
			}
			return false; 
		}
	}
}
