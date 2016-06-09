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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	public enum TileNavPaneHandlerState { Normal, DragMode }
	public class TileNavPaneHandler {
		public TileNavPaneHandler(TileNavPane control) {
			this.Control = control;
			this.HitPoint = InvalidHitPoint;
		}
		static Point invalidHitPoint = new Point(-10000, -10000);
		public static Point InvalidHitPoint { get { return invalidHitPoint; } }
		public TileNavPane Control { get; private set; }
		public Point HitPoint { get; private set; }
		public TileNavPaneHandlerState State { get; set; }
		public virtual void OnMouseEnter(EventArgs e) { }
		public virtual void OnMouseLeave(EventArgs e) {
			Control.ViewInfo.HoverInfo = null;
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			if(e.Button == MouseButtons.None) {
				Control.ViewInfo.HoverInfo = Control.ViewInfo.CalcHitInfo(e.Location);
				return;
			}
			if(e.Button != MouseButtons.Left)
				return;
			if(State == TileNavPaneHandlerState.Normal) {
				OnMouseMoveLeftPressed(e);
			}
			if(State == TileNavPaneHandlerState.DragMode) {
				OnButtonDrag(e.Location);
			}
		}
		protected virtual void OnMouseMoveLeftPressed(MouseEventArgs e) {
			if(!Control.IsDesignMode) return;
			Point distance = GetDistance(e.Location, HitPoint);
			if(!Control.ViewInfo.PressedInfo.InButton || !Control.ViewInfo.PressedInfo.ButtonInfo.IsInteractive)
				return;
			if(HitPoint != InvalidHitPoint && (distance.X > SystemInformation.DragSize.Width || distance.Y > SystemInformation.DragSize.Height) && !Control.DebuggingState) {
				StartDragging(Control.ViewInfo.PressedInfo.ButtonInfo);
			}
		}
		void StartDragging(TileNavButtonViewInfo buttonInfo) {
			Control.Capture = true;
			Control.ViewInfo.DragButtonInfo = buttonInfo;
			State = TileNavPaneHandlerState.DragMode;
			Cursor.Current = (Cursor)DevExpress.XtraBars.BarAndDockingController.Default.DragCursors[DevExpress.XtraBars.BarManager.DragCursor];
		}
		protected Point GetDistance(Point pt1, Point pt2) {
			return new Point(Math.Abs(pt1.X - pt2.X), Math.Abs(pt1.Y - pt2.Y));
		}
		protected virtual void OnButtonDrag(Point pt) {
			Control.ViewInfo.UpdateDropInfo(pt);
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			Control.Focus();
			if(Control.IsDesignMode) {
				OnMouseDownDesignMode(e);
				if(e.Button == MouseButtons.Right)
					return;
			}
			if(e.Button != MouseButtons.Left) return;
			TileNavPaneHitInfo pressedInfo = Control.ViewInfo.CalcHitInfo(e.Location);
			Control.ViewInfo.PressedInfo = pressedInfo;
			HitPoint = e.Location;
		}
		protected virtual void OnMouseDownDesignMode(MouseEventArgs e) {
			TileNavPaneHitInfo hitInfo = Control.ViewInfo.CalcHitInfo(e.Location);
			if(hitInfo.InButton) {
				Control.ViewInfo.DesignTimeManager.SelectComponent(hitInfo.ButtonInfo.Element);
			}
			Control.Invalidate(Control.ClientRectangle);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			Control.Navigator.ClearFocus();
			if(e.Button != MouseButtons.Left) return;
			TileNavPaneHitInfo upInfo = Control.ViewInfo.CalcHitInfo(e.Location);
			if(State == TileNavPaneHandlerState.Normal && upInfo.InButton &&
				Control.ViewInfo.PressedInfo.InButton && Control.ViewInfo.PressedInfo.ButtonInfo == upInfo.ButtonInfo)
				ProceedButtonClick(e);
			else {
				Control.ViewInfo.HideActiveDropDown();
				Control.ViewInfo.ResetPressedInfo();
			}
			if(State == TileNavPaneHandlerState.DragMode)
				OnButtonDrop();
			Control.Capture = false;
			State = TileNavPaneHandlerState.Normal;
			Control.ViewInfo.DropInfo = null;
		}
		private void OnButtonDrop() {
			Cursor.Current = Cursors.Default;
			if(Control.ViewInfo.DropInfo == null) return;
			Control.ViewInfo.OnButtonDrop();
		}
		protected virtual void ProceedButtonClick(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) return;
			Control.ViewInfo.PressedInfo.ButtonInfo.OnElementClick();
			if(Control.ViewInfo.PressedInfo.InButton && !Control.ViewInfo.PressedInfo.ButtonInfo.IsToggle)
				Control.ViewInfo.ResetPressedInfo();
		}
		public virtual void OnMouseWheel(MouseEventArgs e) { }
		public virtual void OnKeyDown(KeyEventArgs e) { OnKeyDownCore(e.KeyCode); }
		public virtual bool ProcessCmdKey(Keys key) { return OnKeyDownCore(key); }
		protected virtual bool OnKeyDownCore(Keys keyData) {
			if(keyData == Keys.Escape) {
				OnEscKeyDown();
				return true;
			}
			switch(keyData) {
				case Keys.Up:
					Control.Navigator.MoveUp();
					return true;
				case Keys.Down:
					Control.Navigator.MoveDown();
					return true;
				case Keys.Left:
					Control.Navigator.MoveLeft();
					return true;
				case Keys.Right:
					Control.Navigator.MoveRight();
					return true;
				case Keys.Enter:
				case Keys.Space:
					Control.Navigator.OnKeyClick();
					return true;
			}
			return false;
		}
		private void OnEscKeyDown() {
			Control.HideDropDownWindow();
		}
	}
	public class TileNavPaneNavigator {
		TileNavPane controlCore;
		public TileNavPane Control { get { return controlCore; } }
		public NavElement FocusedElement { get { return Control.ViewInfo.FocusedElement; } }
		public List<TileNavButtonViewInfo> Buttons { get { return Control.ViewInfo.Buttons; } }
		public TileNavPaneNavigator(TileNavPane control) {
			this.controlCore = control;
		}
		internal void ClearFocus() {
			SetFocused(null);
		}
		void SetFocused(NavElement element) {
			Control.ViewInfo.FocusedElement = element;
		}
		public void MoveLeft() {
			if(FocusedElement == null) {
				SetFocused(Buttons[0].Element);
				return;
			}
			SetFocused(GetFromLeft(FocusedElement.ViewInfo));
		}
		public void MoveRight() {
			if(FocusedElement == null) {
				SetFocused(Buttons[0].Element);
				return;
			}
			SetFocused(GetFromRight(FocusedElement.ViewInfo));
		}
		private NavElement GetFromLeft(TileNavButtonViewInfo focusedInfo) {
			int index = Math.Max(0, Buttons.IndexOf(focusedInfo) - 1);
			TileNavButtonViewInfo resultInfo = Buttons[index];
			if((resultInfo.Element == null || !resultInfo.IsVisible) && Buttons.IndexOf(focusedInfo) > 0)
				return GetFromLeft(resultInfo);
			return resultInfo.Element;
		}
		private NavElement GetFromRight(TileNavButtonViewInfo focusedInfo) {
			int index = Math.Min(Buttons.Count - 1, Buttons.IndexOf(focusedInfo) + 1);
			TileNavButtonViewInfo resultInfo = Buttons[index];
			if((resultInfo.Element == null || !resultInfo.IsVisible) && Buttons.Count - 1 > Buttons.IndexOf(focusedInfo))
				return GetFromRight(resultInfo);
			return resultInfo.Element;
		}
		public void OnKeyClick() {
			if(FocusedElement != null && FocusedElement.ViewInfo != null)
				FocusedElement.ViewInfo.OnElementClick();
		}
		internal void MoveUp() {
			if(FocusedElement == null) {
				SetFocused(Buttons[0].Element);
				return;
			}
		}
		internal void MoveDown() {
			if(FocusedElement == null) {
				SetFocused(Buttons[0].Element);
				return;
			}
		}
	}
}
