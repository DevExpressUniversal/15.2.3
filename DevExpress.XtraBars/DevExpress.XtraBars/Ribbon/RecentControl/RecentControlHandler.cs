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

using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RecentControlHandler {
		static Point InvalidPoint = new Point(int.MinValue, int.MinValue);
		RecentItemControl owner;
		public RecentControlHandler(RecentItemControl owner) {
			this.owner = owner;
		}
		RecentItemControl RecentControl { get { return owner; } }
		RecentControlViewInfo ViewInfo { get { return RecentControl.GetViewInfo(); } }
		RecentControlHitInfo hotInfo, pressedInfo;
		int prevPos;
		RecentPanelViewInfoBase DragPanelInfo { get; set; }
		internal void OnMouseMove(DevExpress.Utils.DXMouseEventArgs ee) {
			hotInfo = ViewInfo.CalcHitInfo(ee.Location);
			if(RecentControl.IsDesignMode) {
				if(pressedInfo != null && pressedInfo.InItem && RecentControl.IsDesignMode && ShouldStartDrag(ee.Location, pressedInfo.HitPoint) && ViewInfo.DragItem != null) {
					RecentDragControl c = new RecentDragControl(this);
					DragDropEffects effects = DragDropEffects.None;
					try {
						effects = c.DoDragDrop(ViewInfo.DragItem.Item, DragDropEffects.Move | DragDropEffects.Copy);
					}
					catch(Exception) {
						effects = DragDropEffects.None;
					}
					StopDragging(effects);
					c.Dispose();
				}
				return;
			}
			if(pressedInfo != null && pressedInfo.HitTest == RecentControlHitTest.Splitter) {
				ViewInfo.UpdateSplitterPosition(hotInfo.HitPoint.X);
				prevPos = hotInfo.HitPoint.X;
				return;
			}
			if(!hotInfo.InItem) {
				RecentControl.MainPanel.ViewInfo.HotItem = null;
				RecentControl.ContentPanel.ViewInfo.HotItem = null;
			}
			else {
				hotInfo.Panel.ViewInfo.HotItem = hotInfo.Item.ViewInfo;
				hotInfo.Item.Handler.OnMouseMove(ee);
			}
			if(hotInfo.HitTest == RecentControlHitTest.Splitter && RecentControl.Cursor != Cursors.SizeWE) {
				RecentControl.Cursor = Cursors.SizeWE;
			}
			else if(RecentControl.Cursor == Cursors.SizeWE)
				RecentControl.Cursor = Cursors.Default;
		}
		bool ShouldStartDrag(Point pt1, Point pt2) {
			return Math.Abs(pt1.X - pt2.X) > SystemInformation.DragSize.Width || Math.Abs(pt1.Y - pt2.Y) > SystemInformation.DragSize.Height;
		}
		internal void OnMouseDown(DevExpress.Utils.DXMouseEventArgs ee) {
			pressedInfo = ViewInfo.CalcHitInfo(ee.Location);
			if(RecentControl.IsDesignMode) {
				if(pressedInfo.InItem) {
					DragPanelInfo = pressedInfo.Panel.ViewInfo;
					ViewInfo.SetDragItem(pressedInfo.Item.ViewInfo);
					ViewInfo.DesignTimeManager.SelectComponent(pressedInfo.Item);
				}
				else if(pressedInfo.InPanel)
					ViewInfo.DesignTimeManager.SelectComponent(pressedInfo.Panel);
				RecentControl.Invalidate();
				return;
			}
			if(pressedInfo.HitTest == RecentControlHitTest.Splitter) {
				prevPos = pressedInfo.HitPoint.X;
			}
			if(pressedInfo.InItem) {
				if(!pressedInfo.Item.Handler.OnMouseDown(ee))
					pressedInfo.Panel.ViewInfo.PressedItem = pressedInfo.Item.ViewInfo;
				pressedInfo.Item.RaiseItemPressed();
			}
			if(pressedInfo.HitTest == RecentControlHitTest.MainPanel || pressedInfo.HitTest == RecentControlHitTest.ContentPanel) {
				pressedInfo.Panel.ViewInfo.SelectedItem = null;
			}
		}
		internal void OnMouseUp(DevExpress.Utils.DXMouseEventArgs ee) {
			RecentControlHitInfo info = ViewInfo.CalcHitInfo(ee.Location);
			if(info.InItem && pressedInfo.Item == info.Item) {
				if(RecentControl.IsDesignMode) {
					if(info.Item is RecentTabItem)
						RecentControl.SelectedTab = (RecentTabItem)info.Item;
				}
				else {
					if(!info.Item.Handler.OnMouseUp(ee)) {
						info.Item.OwnerPanel.ViewInfo.SelectedItem = info.Item.ViewInfo;
						RecentControl.RaiseItemClick(info.Item);
					}
					info.Item.RaiseItemClick();
				}
			}
			RecentControl.MainPanel.ViewInfo.PressedItem = null;
			RecentControl.ContentPanel.ViewInfo.PressedItem = null;
			pressedInfo = null;
			ViewInfo.SetDragItem(null);
			DragPanelInfo = null;
		}
		internal void OnMouseLeave(EventArgs e) {
			hotInfo = null;
		}
		internal void OnMouseWheel(MouseEventArgs e) {
			if(!ViewInfo.IsVScrollVisible) return;
			int wheelChange = ViewInfo.CalcVSmallChange();
			RecentControl.ScrollerPosition += (e.Delta > 0) ? -wheelChange : wheelChange;
		}
		#region Drag&Drop
		int dragCursorIndex = 0;
		protected int DragCursorIndex { get { return dragCursorIndex; } }
		protected virtual RecentItemViewInfoBase GetDropItem(Point pt) {
			RecentItemViewInfoBase res = RecentControl.CalcHitInfo(pt).Item.ViewInfo;
			if(res != null)
				return res;
			return null;
		}
		protected virtual void DoDragging(MouseEventArgs e) {
			if(ViewInfo.DragItem == null || !RecentControl.IsDesignMode) {
				this.dragCursorIndex = BarManager.NoDropCursor;
				return;
			}
			Point pt = RecentControl.PointToClient(new Point(e.X, e.Y));
			ViewInfo.SetDropItem(GetDropItem(pt));
			if(ViewInfo.DropItem == null) {
				RecentControlHitInfo hitInfo = RecentControl.CalcHitInfo(pt);
				if(hitInfo.Panel != DragPanelInfo.Panel){
					this.dragCursorIndex = BarManager.NoDropCursor;
					RecentControl.Invalidate(ViewInfo.DragItem.Item.OwnerPanel.ViewInfo.DropBounds);
					return;
				}
				if(pt.Y <= DragPanelInfo.PanelContentRect.Top) {
					ViewInfo.SetDropItem(DragPanelInfo.Panel.Items[0].ViewInfo);
				}
				else if(pt.Y >= DragPanelInfo.PanelContentRect.Bottom)
					ViewInfo.SetDropItem(DragPanelInfo.Panel.Items[DragPanelInfo.Panel.Items.Count - 1].ViewInfo);
			}
			if(ViewInfo.DropItem == null) {
				this.dragCursorIndex = BarManager.NoDropCursor;
			}
			else {
				if(pt.Y < (ViewInfo.DropItem.Bounds.Y + ViewInfo.DropItem.Bounds.Height / 2))
					ViewInfo.SetDropIndicatorLocation(ItemLocation.Top);
				else
					ViewInfo.SetDropIndicatorLocation(ItemLocation.Bottom);
				this.dragCursorIndex = Control.ModifierKeys == Keys.Control ? BarManager.CopyCursor : BarManager.DragCursor;
			}
			RecentControl.Invalidate(DragPanelInfo.PanelContentRect);
		}
		public virtual void OnDragOver(DragEventArgs e) {
			Point pt = Control.MousePosition;
			DoDragging(new MouseEventArgs(Control.MouseButtons, 0, pt.X, pt.Y, 0));
			if(DragCursorIndex == BarManager.NoDropCursor)
				e.Effect = DragDropEffects.None;
			else if(DragCursorIndex == BarManager.CopyCursor)
				e.Effect = DragDropEffects.Copy;
			else if(DragCursorIndex == BarManager.DragCursor)
				e.Effect = DragDropEffects.Move;
		}
		int GetInsertPosition() {
			if(ViewInfo.DropItem == null)
				return DragPanelInfo.Panel.Items.Count;
			int dropItemIndex = DragPanelInfo.Panel.Items.IndexOf(ViewInfo.DropItem.Item);
			if(ViewInfo.DropIndicatorLocation == ItemLocation.Bottom)
				dropItemIndex++;
			return dropItemIndex;
		}
		public virtual void OnDragDrop(DragEventArgs e) {
			StopDragging(e.Effect);
		}
		protected virtual void StopDragging(DragDropEffects effects) {
			RecentItemViewInfoBase item = ViewInfo.DragItem;
			if(item == null || !RecentControl.IsDesignMode) {
				return;
			}
			if(effects == DragDropEffects.None || DragCursorIndex == BarManager.NoDropCursor) {
				if(DragPanelInfo.Panel.Items.Contains(ViewInfo.DragItem.Item))
					DragPanelInfo.Panel.Items.Remove(ViewInfo.DragItem.Item);
				if(RecentControl.Container != null)
					RecentControl.Container.Remove(ViewInfo.DragItem.Item);
			}
			else if(effects == DragDropEffects.Move) {
				if(ViewInfo.DropItem == null || item.Item != ViewInfo.DropItem.Item) {
					DragPanelInfo.Panel.Items.Remove(item.Item);
					DragPanelInfo.Panel.Items.Insert(GetInsertPosition(), item.Item);
				}
			}
			RecentControl.FireRecentControlChanged(RecentControl);
			ViewInfo.SetDragItem(null);
			ViewInfo.SetDropItem(null);
			RecentControl.Invalidate(DragPanelInfo.PanelContentRect);
		}
		public virtual void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			if(RecentControl.IsDesignMode) {
				if(e.EscapePressed) {
					e.Action = DragAction.Cancel;
					ViewInfo.SetDragItem(null);
					RecentControl.Invalidate(DragPanelInfo.PanelContentRect);
					return;
				}
				if(FireDoDragging) {
					DoDragging(new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
				}
			}
		}
		public virtual void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if(FireDoDragging) {
				e.UseDefaultCursors = false;
				DoDragging(new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
			}
		}
		bool FireDoDragging = false;
		public virtual void OnDragEnter(DragEventArgs e) {
			const int CtrlKeyPressed = 8;
			this.FireDoDragging = false;
			if(ViewInfo.DragItem == null || !RecentControl.IsDesignMode)
				e.Effect = DragDropEffects.None;
			else
				e.Effect = (e.KeyState & CtrlKeyPressed) != 0 ? DragDropEffects.Copy : DragDropEffects.Move;
		}
		public virtual void OnDragLeave(EventArgs e) {
			ViewInfo.SetDropItem(null);
			this.FireDoDragging = true;
			RecentControl.Invalidate(DragPanelInfo.PanelContentRect);
		}
		#endregion
	}
	internal class RecentDragControl : Control {
		RecentControlHandler handler;
		public RecentDragControl(RecentControlHandler handler) {
			this.handler = handler;
		}
		public RecentControlHandler Handler { get { return this.handler; } }
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			Handler.OnQueryContinueDrag(e);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			Handler.OnGiveFeedback(e);
		}
	}
}
