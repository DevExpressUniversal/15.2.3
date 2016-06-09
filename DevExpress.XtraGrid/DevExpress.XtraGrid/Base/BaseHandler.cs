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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Tab;
using DevExpress.Utils;
namespace DevExpress.XtraGrid.Views.Base.Handler {
	public class BaseViewHandler : BaseHandler, IMouseWheelScrollClient {
		BaseView view;
		MouseWheelScrollHelper mouseWheelHelper;
		public BaseViewHandler(BaseView view) {
			this.view = view;
			this.mouseWheelHelper = new MouseWheelScrollHelper(this);
		}
		public override void Dispose() {
			base.Dispose();
		}
		public Keys TranslateRTLKeyCode(Keys e) {
			if(View == null || !View.IsRightToLeft) return e;
			if(e == Keys.Right) return Keys.Left;
			if(e == Keys.Left) return Keys.Right;
			return e;
		}
		public Keys TranslateRTLKeyData(Keys e) {
			if(View == null || !View.IsRightToLeft) return e;
			Keys keyCode = e & (~Keys.Modifiers);
			Keys res = TranslateRTLKeyCode(keyCode);
			if(res != keyCode) return res | (e & Keys.Modifiers);
			return e;
		}
		public KeyEventArgs TranslateRTLKeys(KeyEventArgs e) {
			if(View == null || !View.IsRightToLeft) return e;
			Keys data = TranslateRTLKeyData(e.KeyData);
			if(data != e.KeyData) return new KeyEventArgs(data);
			return e;
		}
		public static KeyEventArgs FromMouseEventArgs(MouseEventArgs e) {
			Keys keyData = (e.Button == MouseButtons.Left) ? Keys.LButton : Keys.RButton;
			if(e.Button == MouseButtons.Middle) keyData = Keys.MButton;
			Keys modifiers = Control.ModifierKeys;
			if((modifiers & Keys.Shift) == Keys.Shift) keyData |= Keys.Shift;
			if((modifiers & Keys.Control) == Keys.Control) keyData |= Keys.Control;
			if((modifiers & Keys.Alt) == Keys.Alt) keyData |= Keys.Alt;
			return new KeyEventArgs(keyData | modifiers);
		}
		public virtual bool TabControlVisible { get { return ViewInfo.ShowTabControl && TabControl != null && !TabControl.Bounds.IsEmpty; } }
		public virtual ViewTab TabControl { get { return ViewInfo == null ? null : ViewInfo.TabControl; } }
		public virtual BaseViewInfo ViewInfo { get { return View == null ? null : View.ViewInfo; } }
		public BaseView View { get { return view; } }
		public GridControl Grid { get { return view == null ? null : view.GridControl; } }
		protected override Rectangle ClientBounds { get { return View.ViewRect; } }
		protected override void OnKeyDown(KeyEventArgs e) { 
			View.RaiseKeyDown(e); 
		}
		protected override void OnKeyUp(KeyEventArgs e) { View.RaiseKeyUp(e); }
		protected override void OnKeyPress(KeyPressEventArgs e) { 
			View.RaiseKeyPress(e);
		}
		protected override bool OnMouseDown(MouseEventArgs e) {
			DoubleClickChecker.CheckDoubleClick(View, e, false, View.RaiseDoubleClick);
			View.RaiseMouseDown(e);
			return false;
		}
		protected override bool OnMouseUp(MouseEventArgs e) { View.RaiseMouseUp(e); return false; }
		protected override bool OnMouseMove(MouseEventArgs e) { View.RaiseMouseMove(e); return false; }
		protected override bool OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			View.RaiseMouseWheel(e);
			if(ee.Handled) return false;
			mouseWheelHelper.OnMouseWheel(ee);
			return false;
		}
		protected override void OnClick(MouseEventArgs e) { View.RaiseClick(e); }
		protected override void OnDoubleClick(MouseEventArgs e) { View.RaiseDoubleClick(e); }
		protected override void OnMouseEnter(System.EventArgs e) { View.RaiseMouseEnter(e); }
		protected override void OnMouseLeave(System.EventArgs e) { View.RaiseMouseLeave(e); }
		protected override void OnResize(Rectangle clientRect) { }
		public override bool RequireMouse(MouseEventArgs e) {
			bool result = base.RequireMouse(e);
			if(!result && TabControlVisible) {
				result = TabControl.Handler.RequireMouse(e);
			}
			return result;
		}
		public override EventResult ProcessEvent(EventType etype, object args) {
			if(!TabControlVisible || View.IsSizingState || View.IsDraggingState) return base.ProcessEvent(etype, args);
			bool defaultProcess = true;
			EventResult res = EventResult.None;
			switch(etype) {
				case EventType.MouseDown : 
				case EventType.MouseUp : 
					if(TabControl.Handler.RequireMouse(args as MouseEventArgs)) {
						res = TabControl.Handler.ProcessEvent(etype, args);
						if(TabControl == null) return EventResult.Handled; 
						if(TabControl.SelectedPage != null && TabControl.SelectedPage == TabControl.Handler.DownHitInfo.Page) {
							if(Grid != null && Grid.FocusedView != View) {
								if(!Grid.FocusedView.CheckCanLeaveCurrentRow(true)) return EventResult.Handled;
							}
							View.Focus();
						}
						defaultProcess = false;
					}
					break;
				case EventType.MouseMove : 
					res = TabControl.Handler.ProcessEvent(etype, args);
					break;
			}
			if(defaultProcess) return base.ProcessEvent(etype, args);
			return res;
		}
		public override void UpdateMouseHere(Point p, bool val) {
			if(TabControlVisible) {
				TabControl.Handler.UpdateMouseHere(p, TabControl.Bounds.Contains(p));
			}
			base.UpdateMouseHere(p, val);
		}
		public virtual void DoClickAction(BaseHitInfo hitInfo) {
		}
		#region IMouseWheelScrollClient Members
		protected virtual bool OnMouseWheel(MouseWheelScrollClientArgs e) { return false; }
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			OnMouseWheel(e);
		}
		bool IMouseWheelScrollClient.PixelModeHorz {
			get { return AllowMouseWheelPixelScrollingHorz; }
		}
		bool IMouseWheelScrollClient.PixelModeVert {
			get { return AllowMouseWheelPixelScrollingVert; }
		}
		#endregion
		protected virtual bool AllowMouseWheelPixelScrollingVert { get { return false; } }
		protected virtual bool AllowMouseWheelPixelScrollingHorz { get { return false; } }
	}
	public class EmptyViewHandler : BaseViewHandler {
		public EmptyViewHandler() : base(null) { }
		static EmptyViewHandler _default;
		public static EmptyViewHandler Default {
			get {
				if(_default == null) _default = new EmptyViewHandler();
				return _default;
			}
		}
		public override bool TabControlVisible { get { return false; } }
		protected override Rectangle ClientBounds { get { return Rectangle.Empty; } }
		protected override void OnKeyDown(KeyEventArgs e) { }
		protected override void OnKeyUp(KeyEventArgs e) { }
		protected override void OnKeyPress(KeyPressEventArgs e) {  }
		protected override bool OnMouseDown(MouseEventArgs e) { return false; }
		protected override bool OnMouseUp(MouseEventArgs e) { return false; }
		protected override bool OnMouseMove(MouseEventArgs e) { return false; }
		protected override bool OnMouseWheel(MouseEventArgs e) { return false; }
		protected override void OnClick(MouseEventArgs e) { }
		protected override void OnDoubleClick(MouseEventArgs e) { }
		protected override void OnMouseEnter(System.EventArgs e) { }
		protected override void OnMouseLeave(System.EventArgs e) {  }
		protected override void OnResize(Rectangle clientRect) { }
		public override bool RequireMouse(MouseEventArgs e) { return false; }
		public override EventResult ProcessEvent(EventType etype, object args) { return EventResult.None; }
		public override void UpdateMouseHere(Point p, bool val) { }
		public override void DoClickAction(BaseHitInfo hitInfo) {}
	}
	public delegate void DoubleClickHandler(MouseEventArgs e);
	public class DoubleClickChecker{
		const long tickLength = 0x2710L;
		static Point lastMouseDowntPoint;
		static long lastMouseDownTime;
		static BaseView lastMouseDownView;
		static bool fWaitSecondClickMode = false;
		static bool lastSenderIsEditor = false;
		public static void CheckDoubleClick(BaseView view, MouseEventArgs e, bool fromEditor, DoubleClickHandler handler) {
			if(!view.ForceDoubleClick || IsLocked) return;
			if((e.Button & MouseButtons.Left) == 0) {
				fWaitSecondClickMode = false;
				return;
			}
			bool needSupressOnEditorShowing = !lastSenderIsEditor && fromEditor;
			lastSenderIsEditor = fromEditor;
			if(fWaitSecondClickMode && !needSupressOnEditorShowing) {
				if(CheckSameViewCondition(view) & CheckTimeCondition(DateTime.Now.Ticks) & CheckLocationCondition(e.Location)) {
					if(handler!=null) handler(e);
				}
				fWaitSecondClickMode = false;
			} else {
				SaveFirstClick(view, e.Location, DateTime.Now.Ticks);
				fWaitSecondClickMode = true;
				return;
			}
		}
		static int lockCounter = 0;
		protected static bool IsLocked { 
			get { return lockCounter>0; } 
		}
		public static void Lock() {
			lockCounter++;
		}
		public static void Unlock() {
			if(--lockCounter == 0) Reset();
		}
		public static void Reset() {
			lastMouseDownView = null;
			lastMouseDowntPoint = Point.Empty;
			lastMouseDownTime = 0;
			fWaitSecondClickMode = false;
		}
		private static void SaveFirstClick(BaseView view, Point point, long time) {
			lastMouseDownView = view;
			lastMouseDowntPoint = point;
			lastMouseDownTime = time;
		}
		private static bool CheckSameViewCondition(BaseView view) { 
			bool condition = lastMouseDownView == view;
			lastMouseDownView = view;
			return condition;
		}
		private static bool CheckTimeCondition(long currentMouseDownTime) {
			bool condition = (Math.Abs(lastMouseDownTime-currentMouseDownTime) / tickLength) < SystemInformation.DoubleClickTime;
			lastMouseDownTime = currentMouseDownTime;
			return condition;
		}
		private static bool CheckLocationCondition(Point currentMouseEventPoint) {
			bool conditionX = Math.Abs(currentMouseEventPoint.X-lastMouseDowntPoint.X) <= SystemInformation.DoubleClickSize.Width;
			bool conditionY = Math.Abs(currentMouseEventPoint.Y-lastMouseDowntPoint.Y) <= SystemInformation.DoubleClickSize.Height;
			lastMouseDowntPoint = currentMouseEventPoint;
			return conditionX && conditionY;
		}
	}
}
