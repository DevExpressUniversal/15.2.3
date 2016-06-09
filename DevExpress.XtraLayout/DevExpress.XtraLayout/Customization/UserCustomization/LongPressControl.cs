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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Internal;
using DevExpress.Utils.Win.Hook;
namespace DevExpress.XtraLayout.Customization.UserCustomization {
	class AdornerLayeredWindow : DXLayeredWindowEx {
		int percentCore;
		public int Percent { get { return percentCore; } set { percentCore = value; Invalidate(); } }
		protected override void DrawCore(GraphicsCache cache) {
			float progressStep = (float)Bounds.Height / 100 * percentCore;
			cache.FillRectangle(Brushes.Gray, new Rectangle(0, 0, Bounds.Width, Bounds.Height));
			cache.FillRectangle(Brushes.White, new Rectangle(1, 1, Bounds.Width - 2, Bounds.Height - 2));
			cache.FillRectangle(Brushes.Black, new Rectangle(2, 2, Bounds.Width - 4, Bounds.Height - 4));
			cache.FillRectangle(Brushes.White, new Rectangle(2, 2, Bounds.Width - 4, (Bounds.Height - 4) - (int)progressStep));
		}
	}
	class LongPressManagerVisaulizer : IDisposable {
		AdornerLayeredWindow form;
		public LongPressManagerVisaulizer() {
			form = new AdornerLayeredWindow();
		}
		public void Show(Point location, Control control, Size quickModeLoadIndicatorSize) {
			EnsureInitialized(control);
			form.Size = quickModeLoadIndicatorSize;
			form.Percent = 0;
			location.X += 16;
			location.Y -= 3;
			form.Show(location);
		}
		public void EnsureInitialized(Control control) {
			if(!form.IsCreated) form.Create(control);
		}
		public void Hide() {
			form.Hide();
		}
		public void Update(int percent) {
			form.Percent = percent;
		}
		public void Dispose() {
			if(form != null) form.Dispose();
		}
	}
	public class LongPressControlHookController : IDisposable, IHookController {
		const int WM_GESTURENOTIFY = 0x011A;
		const int WM_GESTURE = 0x0119;
		Control owner;
		LongPressControl longPressControl;
		public LongPressControlHookController(LongPressControl lpc, Control target) {
			owner = target;
			longPressControl = lpc;
			HookManager.DefaultManager.AddController(this);
		}
		public bool InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return false;
		}
		protected Point TransformPointToOwner(Control client, IntPtr lParam, int Msg) {
			if(Msg == WM_GESTURENOTIFY) return owner.PointToClient(GetGestureNotifyPoint(lParam));
			return owner.PointToClient(lParam.PointFromLParam());
		}
		[System.Security.SecuritySafeCritical]
		private Point GetGestureNotifyPoint(IntPtr lParam) {
			NativeMethods.GESTURENOTIFYSTRUCT gs = new NativeMethods.GESTURENOTIFYSTRUCT();
			Marshal.PtrToStructure(lParam, gs);
			return new Point(gs.ptsLocation.x, gs.ptsLocation.y);
		}
		bool AllowProcessMouseEvent(Point p, Control wnd) {
			Control current = wnd;
			while(current != owner && current != null)
				current = current.Parent;
			return current == owner && owner.Visible && owner.DisplayRectangle.Contains(p);
		}
		public bool InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr lParam) {
			if(owner != null) {
				if(Msg == MSG.WM_ACTIVATEAPP) longPressControl.ProcessEvent(EventType.ActivateApp, null, null, null);
				if(wnd != null) {
					Form form = wnd.FindForm();
					Form ownerform = owner.FindForm();
					if(form == ownerform) {
						if(Msg == MSG.WM_RBUTTONDOWN || Msg == MSG.WM_RBUTTONUP || Msg == MSG.WM_LBUTTONUP || Msg == MSG.WM_MOUSEMOVE || Msg == WM_GESTURENOTIFY || Msg == WM_GESTURE) {
							Point p = TransformPointToOwner(wnd, lParam, Msg);
							if(!AllowProcessMouseEvent(p, wnd) && Msg != WM_GESTURE) return false;
							if(Msg == MSG.WM_RBUTTONDOWN) longPressControl.ProcessEvent(EventType.MouseDown, null, new MouseEventArgs(Msg == MSG.WM_LBUTTONDOWN ? MouseButtons.Left : MouseButtons.Right, 0, p.X, p.Y, 0), null);
							if(Msg == MSG.WM_RBUTTONUP || Msg == MSG.WM_LBUTTONUP) longPressControl.ProcessEvent(EventType.MouseUp, null, new MouseEventArgs(Msg == MSG.WM_LBUTTONUP ? MouseButtons.Left : MouseButtons.Right, 0, p.X, p.Y, 0), null);
							if(Msg == MSG.WM_MOUSEMOVE) longPressControl.ProcessEvent(EventType.MouseMove, null, new MouseEventArgs(Control.MouseButtons, 0, p.X, p.Y, 0), null);
							if(Msg == WM_GESTURENOTIFY) longPressControl.ProcessEvent(EventType.GestureNotify, null, new MouseEventArgs(Control.MouseButtons, 0, p.X, p.Y, 0), null);
							if(Msg == WM_GESTURE) longPressControl.ProcessEvent(EventType.GestureNotify, null, null, null, true);
						}
					}
				}
			}
			return false;
		}
		public IntPtr OwnerHandle {
			get { return owner.IsHandleCreated ? owner.Handle : IntPtr.Zero; }
		}
		public void Dispose() {
			HookManager.DefaultManager.RemoveController(this);
		}
	}
	public class LongPressControl : IDisposable {
		bool enebledCore = false;
		public bool Enabled { get { return enebledCore; } set { enebledCore = value; } }
		Control targetCore;
		Timer holdTimer, updateTimer, gestureTimer;
		Point cursorPoint, handPoint;
		LongPressControlHookController hookController;
		LongPressManagerVisaulizer visualizer;
		bool touchMode = false;
		public const int animationStepsCount = 20;
		public const int mouseMoveTolerantDistance = 15;
		protected int progressCore = 0, gestureCore = 0;
		OptionsCustomizationForm optionsCustomizationForm;
		public LongPressControl(Control target, OptionsCustomizationForm optionsCustomizationForm) {
			targetCore = target;
			this.optionsCustomizationForm = optionsCustomizationForm;
			InitTimers();
			visualizer = new LongPressManagerVisaulizer();
			SubscribeEvents();
		}
		private void InitTimers() {
			holdTimer = new Timer() { Interval = this.optionsCustomizationForm.QuickModeInitDelay };
			updateTimer = new Timer() { Interval = this.optionsCustomizationForm.QuickModeLoadTime / animationStepsCount };
			gestureTimer = new Timer() { Interval = 700 };
		}
		public void Init() { }
		protected virtual void SubscribeEvents() {
			if(targetCore == null) return;
			targetCore.HandleCreated += targetCore_HandleCreated;
			RegisterHook();
			holdTimer.Tick += HoldTimerTick;
			updateTimer.Tick += UpdateTimerTick;
			gestureTimer.Tick += gestureTimerTick;
		}
		void gestureTimerTick(object sender, EventArgs e) {
			Reset();
			TargetMouseDown(sender, new MouseEventArgs(Control.MouseButtons, 0, handPoint.X, handPoint.Y, 0), true);
		}
		protected virtual void UnSubscribeEvents() {
			targetCore.HandleCreated -= targetCore_HandleCreated;
			if(holdTimer != null) holdTimer.Tick -= HoldTimerTick;
			if(updateTimer != null) updateTimer.Tick -= UpdateTimerTick;
			if(gestureTimer != null) gestureTimer.Tick -= gestureTimerTick;
		}
		void targetCore_HandleCreated(object sender, EventArgs e) {
			RegisterHook();
		}
		protected void RegisterHook() {
			if(targetCore != null && targetCore.IsHandleCreated && hookController == null) hookController = CreateHookController();
		}
		protected virtual LongPressControlHookController CreateHookController() {
			return new LongPressControlHookController(this, targetCore);
		}
		protected void UpdateTimerTick(object sender, EventArgs e) {
			progressCore += 100 / animationStepsCount;
			visualizer.Update(progressCore);
			if(progressCore >= 100) {
				Reset();
				RaiseLongPress();
			}
		}
		protected internal void RaiseLongPress() {
			if(LongPress != null) {
				Point point = touchMode ? handPoint : cursorPoint;
				LongPress(this, new MouseEventArgs(MouseButtons.Left, 0, point.X, point.Y, 0));
			}
		}
		public event MouseEventHandler LongPress;
		protected internal void ProcessEvent(EventType eventType, object sender, MouseEventArgs e, KeyEventArgs key, bool reset = false) {
#if DEBUGTEST
			if(eventType == EventType.MouseDown && Control.ModifierKeys.HasFlag(Keys.Shift) && Control.ModifierKeys.HasFlag(Keys.Control) && Control.ModifierKeys.HasFlag(Keys.Alt) && targetCore is LayoutControl) {
				(this.targetCore as LayoutControl).ShowPrintPreview();
			}
#endif
			if(!Enabled || CheckCursor()) return;
			switch(eventType) {
				case EventType.MouseDown:
					TargetMouseDown(sender, e, true);
					break;
				case EventType.ActivateApp:
					TargetMouseLeave(sender, null);
					break;
				case EventType.MouseUp:
					TargetMouseUp(sender, e);
					break;
				case EventType.MouseMove:
					TargetMouseMove(sender, e);
					break;
				case EventType.GestureNotify:
					TargetGestureNotify(sender, e, reset);
					break;
			}
		}
		private bool CheckCursor() {
			return !(Cursor.Current == Cursors.Arrow || Cursor.Current == Cursors.Default ||
					 Cursor.Current == Cursors.WaitCursor || Cursor.Current == Cursors.IBeam || Cursor.Current == Cursors.Hand);
		}
		private void TargetGestureNotify(object sender, MouseEventArgs e, bool reset) {
			if(!reset) {
				touchMode = true;
				cursorPoint = Cursor.Position;
				handPoint = targetCore.PointToScreen(e.Location);
				handPoint.X -= 50;
				gestureTimer.Start();
			} else {
				touchMode = false;
				cursorPoint = Point.Empty;
				handPoint = Point.Empty;
				Reset();
			}
		}
		protected void HoldTimerTick(object sender, EventArgs e) {
			holdTimer.Stop();
			updateTimer.Start();
			visualizer.Show(touchMode ? handPoint : cursorPoint, targetCore, optionsCustomizationForm.QuickModeLoadIndicatorSize);
		}
		protected void TargetMouseDown(object sender, MouseEventArgs e, bool pointToScreen) {
			if(pointToScreen) cursorPoint = Cursor.Position;
			holdTimer.Start();
		}
		protected void TargetMouseUp(object sender, MouseEventArgs e) {
			touchMode = false;
			cursorPoint = Point.Empty;
			handPoint = Point.Empty;
			Reset();
		}
		protected void TargetMouseMove(object sender, MouseEventArgs e) {
			Point currentPoint = Cursor.Position;
			int dx, dy;
			dx = Math.Abs(currentPoint.X - cursorPoint.X);
			dy = Math.Abs(currentPoint.Y - cursorPoint.Y);
			if((dx > mouseMoveTolerantDistance || dy > mouseMoveTolerantDistance)) Reset();
		}
		protected void TargetMouseLeave(object sender, EventArgs e) {
			Reset();
		}
		private void Reset() {
			progressCore = 0;
			gestureCore = 0;
			visualizer.Hide();
			holdTimer.Stop();
			updateTimer.Stop();
			gestureTimer.Stop();
		}
		public void Dispose() {
			visualizer.Dispose();
			if(hookController != null) hookController.Dispose();
			UnSubscribeEvents();
			if(updateTimer != null) { updateTimer.Dispose(); updateTimer = null; }
			if(holdTimer != null) { holdTimer.Dispose(); holdTimer = null; }
			if(gestureTimer != null) { gestureTimer.Dispose(); gestureTimer = null; }
		}
		internal void Update(OptionsCustomizationForm optionsCustomizationForm) {
			UnSubscribeEvents();
			this.optionsCustomizationForm = optionsCustomizationForm;
			InitTimers();
			SubscribeEvents();
		}
	}
}
