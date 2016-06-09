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
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Views.Base.Handler;
namespace DevExpress.XtraGrid.Views.Card.Handler {
	public class CardHandler : BaseViewHandler {
		CardHitInfo downPointHitInfo;
		internal GridCell prevFocusedCell;
		public CardHandler(CardView cv) : base(cv) {
			this.downPointHitInfo = new CardHitInfo();
			this.prevFocusedCell = new GridCell(0, null);
		}
		public new CardViewInfo ViewInfo { get { return base.ViewInfo as CardViewInfo; } }
		public new CardView View { 	get { return base.View as CardView; } }
		private Keys[] navKeys = { Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp,
									 Keys.End, Keys.Home, Keys.Tab};
		protected virtual void DoNavigation(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Left: 
				case Keys.Right:
					View.DoMoveFocusedRowHorz(e.KeyCode == Keys.Left ? -1 : 1, e.KeyCode);
					break;
				case Keys.Tab:
					View.DoMoveFocusedColumn((e.Shift ? -1 : 1), Keys.Tab);
					break;
				case Keys.Up:
					if(e.Modifiers != Keys.None) 
						View.DoMoveFocusedRow(-1, e);
					else
						View.DoMoveFocusedColumn(-1, e.KeyCode);
					break;
				case Keys.Down:
					if(e.Modifiers != Keys.None) 
						View.DoMoveFocusedRow(1, e);
					else
						View.DoMoveFocusedColumn(1, e.KeyCode);
					break;
				case Keys.PageUp:
					View.DoMoveFocusedRow(-ViewInfo.Cards.Count, new KeyEventArgs(Keys.None));
					break;
				case Keys.PageDown:
					View.DoMoveFocusedRow(ViewInfo.Cards.Count, new KeyEventArgs(Keys.None));
					break;
				case Keys.Home:
				case Keys.End:
					View.DoMoveFocusedRow(0, e);
					break;
			}
		}
		public override bool RequireMouse(MouseEventArgs e) {
			return false;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			bool prevEditorOpen = false;
			if(View.State == CardState.Normal && e.KeyCode == Keys.Escape) {
				View.CancelUpdateCurrentRow();
			}
			if(View.IsDefaultState) {
				if(e.KeyCode == Keys.F5) {
					View.RefreshData();
					return;
				}
				if(View.CheckShowFindPanelKey(e)) return;
			}
			if(View.State == CardState.Editing) {
				if(e.KeyCode == Keys.Escape)
					View.HideEditor();
				else {
					prevEditorOpen = true;
					View.PostEditor();
					View.HideEditor();
					if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape) return;
				}
			} else {
				if(View.IsDefaultState && (e.KeyData == (Keys.C | Keys.Control) || e.KeyData == (Keys.Insert | Keys.Control))) {
					View.CopyToClipboard();
					return;
				}
			}
			if(View.State == CardState.Normal) {
				if(View.FocusedColumn == null || View.GetCardCollapsed(View.FocusedRowHandle)) {
					switch(e.KeyData) {
						case Keys.Add :
							View.SetCardCollapsed(View.FocusedRowHandle, false);
							return;
						case Keys.Subtract:
							View.SetCardCollapsed(View.FocusedRowHandle, true);
							return;
					}
				}
				if(e.KeyCode == Keys.Space && e.Control && View.IsMultiSelect) {
					View.InvertFocusedRowSelectionCore(null);
					return;
				} 
				RepositoryItem ritem = View.GetRowCellRepositoryItem(View.FocusedRowHandle, View.FocusedColumn);
				if(ritem != null && ritem.IsActivateKey(e.KeyData)) {
					View.ShowEditorByKey(e);
					return;
				}
				if(Array.IndexOf(navKeys, e.KeyCode) != -1) {
					DoNavigation(e);
					if(prevEditorOpen && View.IsKeyboardFocused) {
						View.ShowEditor(); 
					}
					return;
				}
				if(e.KeyCode == Keys.Enter) {
					View.ShowEditor();
				}
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
		}
		static char[] keyPress = new char[] { (char)27, (char)9, (char)8, (char)3 };
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.KeyChar == 6 && View.FindPanel != null) {
				e.Handled = true;
			}
			if(e.Handled) return;
			if(!View.IsEditing && Array.IndexOf(keyPress, e.KeyChar) == -1) {
				if(e.KeyChar == '+' || e.KeyChar == '-' && View.FocusedColumn == null) return;
				if(e.KeyChar != 27)
					View.ShowEditorByKeyPress(e);
			}
		}
		protected virtual void DoStartCardsSizing(Point p) {
			View.StartCardsSizing(p);
		}
		protected override bool OnMouseDown(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseDown(e);
			if(e.Handled) return true;
			this.prevFocusedCell = new GridCell(View.FocusedRowHandle, View.FocusedColumn);
			View.GridControl.FocusViewByMouse(View);
			if(View == null) return true;
			this.downPointHitInfo = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
			if(this.downPointHitInfo.InCard) {
				int prevFocusedRow = View.FocusedRowHandle;
				GridColumn prevFocusedColumn = View.FocusedColumn;
				try {
					View.FocusedRowHandle = downPointHitInfo.RowHandle;
				}
				finally {
					View.DoAfterMoveFocusedRow(FromMouseEventArgs(e), prevFocusedRow, prevFocusedColumn, null);
				}
			}
			if((e.Button & MouseButtons.Left) != 0) { 
				View.GridControl.MouseCaptureOwner = View;
				if(ViewInfo.SelectionInfo.CheckMouseDown(this.downPointHitInfo)) return true;
				if(this.downPointHitInfo.InCard) {
					if(this.downPointHitInfo.InCardButtons || View.GetCardCollapsed(this.downPointHitInfo.RowHandle)) return true;
					if(View.IsDesignMode) {
						if(downPointHitInfo.Column != null) {
							downPointHitInfo.Column.SelectInDesigner();
							return true;
						}
					}
					View.FocusedColumn = downPointHitInfo.Column;
					GridControl ctrl = View.GridControl;
					bool isSameCellFocused = View.FocusedColumn == prevFocusedCell.Column && View.FocusedRowHandle == prevFocusedCell.RowHandle;
					if(this.downPointHitInfo.InCardContent && 
					  (View.GetShowEditorMode() == EditorShowMode.MouseDown || 
					  (View.GetShowEditorMode() == EditorShowMode.MouseDownFocused && isSameCellFocused))) {
						View.ShowEditorByMouse();
						ctrl.MouseCaptureOwner = null;
					}
					if((View.SelectedRowsCount > 1 || !View.IsRowSelected(View.FocusedRowHandle)) && this.downPointHitInfo.InCard && View.IsMultiSelect && Control.ModifierKeys == Keys.None) {
						View.ClearSelectionCore();
						View.SelectFocusedRowCore();
					}
					return true;
				}
				if(downPointHitInfo.HitTest == CardHitTest.Separator) {
 					DoStartCardsSizing(new Point(e.X, e.Y));
					return true;
				}
				if(!View.IsDefaultState) View.SetDefaultState();
			} else {
				if(e.Button == MouseButtons.Middle && e.Clicks == 1) {
					if(ViewInfo.ViewRects.Cards.Contains(e.X, e.Y)) {
						View.SetDefaultState();
						if(View.IsDefaultState) {
							View.GridControl.Focus();
							View.StartScrolling();
							return true;
						}
					}
				}
				return true;
			}
			return false;
		}
		protected override bool OnMouseUp(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseUp(e);
			if(e.Handled) return true;
			Point p = new Point(e.X, e.Y);
			CardHitInfo hitInfo = ViewInfo.CalcHitInfo(p);
			if((e.Button & MouseButtons.Left) != 0) {
				if(View.State == CardState.Sizing) {
					View.EndCardsSizing();
					View.SetState(CardState.Normal);
					return true;
				}
				ViewInfo.SelectionInfo.CheckMouseUp(hitInfo);
				if(View == null) return true; 
				if(View.IsMultiSelect) {
					if(CanCancelMultiSelectOnMouseUp(hitInfo)) {
						if(View.SelectedRowsCount != 1 || !View.IsRowSelected(View.FocusedRowHandle)) {
							View.ClearSelectionCore();
							View.SelectFocusedRowCore();
						}
					}
				}
				if(View.IsDefaultState && Control.ModifierKeys == Keys.None) {
					if(View.GetShowEditorMode() != EditorShowMode.MouseDown) {
						if(hitInfo.InCardButtons) return true;
						if(hitInfo.InCardContent && this.downPointHitInfo.RowHandle == hitInfo.RowHandle && View.FocusedRowHandle == hitInfo.RowHandle && this.downPointHitInfo.Column == hitInfo.Column) {
							if(View.GetShowEditorMode() == EditorShowMode.Click && !this.prevFocusedCell.Equals(new GridCell(View.FocusedRowHandle, View.FocusedColumn))) return true;
							View.ShowEditor();
							return true;
						}
					}
				}
				if(View != null && !View.IsEditing) View.SetDefaultState();
			}
			return false;
		}
		protected virtual bool CanCancelMultiSelectOnMouseUp(CardHitInfo hitInfo) {
			bool fCheckFieldUp = downPointHitInfo.InField && View.Editable && downPointHitInfo.Column.OptionsColumn.AllowEdit;
			bool fCheckColumnUp = downPointHitInfo.Column == hitInfo.Column;
			bool fCheckRowUp = (downPointHitInfo.RowHandle == hitInfo.RowHandle) && (View.FocusedRowHandle == hitInfo.RowHandle);
			return fCheckFieldUp && fCheckColumnUp && fCheckRowUp;
		}
		protected override bool OnMouseMove(MouseEventArgs ev) {
			if(View != null) View.OnActionScroll(XtraEditors.ScrollNotifyAction.MouseMove);
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseMove(e);
			if(e.Handled) return true;
			Point p = new Point(e.X, e.Y);
			CardHitInfo hi = ViewInfo.CalcHitInfo(p);
			if(View.State == CardState.Sizing) {
				View.SetCursor(Cursors.SizeWE);
				View.DoCardsSizing(p);
				return true;
			}
			bool resetCursor = true;
			if(hi.HitTest == CardHitTest.Separator && View.CanResizeCards) {
				View.SetCursor(Cursors.SizeWE);
				resetCursor = false;
			}
			resetCursor &= ViewInfo.SelectionInfo.CheckHotTrackMouseMove(hi);
			if(resetCursor)
				View.ResetCursor();
			return false;
		}
		protected override bool OnMouseWheel(MouseWheelScrollClientArgs e) {
 			base.OnMouseWheel(e);
			if(e.Handled) return true;
			e.Handled = true;
			if(View.IsEditing) return false;
			View.TopLeftCardIndex += View.CalcHSmallChange() * e.Distance;
			return true;
		}
		public override void DoClickAction(BaseHitInfo hitInfo) {
			CardHitInfo hit = hitInfo as CardHitInfo;
			if(hit.HitTest == CardHitTest.CardDownButton) {
				View.FocusedCardTopFieldIndex += 1;
				return;
			}
			if(hit.HitTest == CardHitTest.FilterPanelActiveButton) {
				View.ActiveFilterEnabled = !View.ActiveFilterEnabled;
				return;
			}
			if(hit.HitTest == CardHitTest.FilterPanelCustomizeButton) {
				View.OnFilterCustomizeClick();
				return;
			}
			if(hit.HitTest == CardHitTest.FilterPanelCloseButton) {
				View.ClearColumnsFilter();
				return;
			}
			if(hit.HitTest == CardHitTest.FilterPanelMRUButton || hit.HitTest == CardHitTest.FilterPanelText) {
				View.ShowMRUFilterPopup();
				return;
			}
			if(hit.HitTest == CardHitTest.CardUpButton) {
				View.FocusedCardTopFieldIndex -= 1;
				return;
			}
			if(hit.HitTest == CardHitTest.CloseZoomButton) {
				if(View.IsZoomedView)
					View.NormalView();
				else
					View.ZoomView();
				return;
			}
			if(hit.HitTest == CardHitTest.QuickCustomizeButton) {
				if(View.CustomizationForm != null)
					View.DestroyCustomizationForm();
				else
					View.ShowQuickCustomization();
				View.SetDefaultState();
				return;
			}
			if(hit.HitTest == CardHitTest.CardExpandButton) {
				View.SetCardCollapsed(hit.RowHandle, !View.GetCardCollapsed(hit.RowHandle));
			}
			base.DoClickAction(hitInfo);
		}
		protected override void OnResize(Rectangle clientRect) {
			View.OnActionScroll(XtraEditors.ScrollNotifyAction.Resize);
			View.InternalSetViewRectCore(clientRect);
		}
		protected override void OnMouseEnter(System.EventArgs e) {
			base.OnMouseEnter(e);
			CardHitInfo hi = View.CalcHitInfo(View.GridControl.PointToClient(Control.MousePosition));
			ViewInfo.SelectionInfo.OnHotTrackEnter(hi);
		}
		protected override void OnMouseLeave(System.EventArgs e) {
			base.OnMouseLeave(e);
			ViewInfo.SelectionInfo.OnHotTrackLeave();
		}
		public override bool ProcessKey(KeyEventArgs e) {
			if(View.ActiveEditor != null) {
				DevExpress.XtraEditors.BaseEdit be = View.ActiveEditor as DevExpress.XtraEditors.BaseEdit;
				if(be != null && be.IsNeededKey(e)) return false;
			}
			if(e.KeyData == (Keys.Tab | Keys.Control)) return false;
			if((e.KeyCode != Keys.Left && e.KeyCode != Keys.Right) && 
				Array.IndexOf(navKeys, e.KeyCode) != -1 || e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter) {
				if((Control.ModifierKeys & Keys.Alt) != 0)
					return false;
				OnKeyDown(e);
				return true;
			}
			return false;
		}
		public override bool NeedKey(NeedKeyType keyType) { 
			switch(keyType) {
				case NeedKeyType.Tab : return View.OptionsBehavior.UseTabKey;
				case NeedKeyType.Escape : return View.IsEditing || View.FocusedRowModified;
				case NeedKeyType.Enter : return View.IsEditing;
				case NeedKeyType.Dialog: return View.IsEditing;
			}
			return false; 
		}
	}
}
