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
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
#if SILVERLIGHT
using System.Windows.Threading;
using DevExpress.Data;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using System.Windows.Controls;
#elif DXPORTABLE
using PlatformIndependentMouseEventArgs = DevExpress.Compatibility.System.Windows.Forms.MouseEventArgs;
#else
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
#endif
namespace DevExpress.Utils {
	public interface IOfficeScroller : IDisposable {
#if !DXPORTABLE
		void Start(Control control);
		void Start(Control control, Point screenPoint);
#endif
	}
	public abstract class MouseHandler : IDisposable {
		#region Fields
		public static readonly PlatformIndependentMouseEventArgs EmptyMouseEventArgs = new PlatformIndependentMouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
		bool isDisposed;
		bool suspended;
		int clickCount;
		Point clickScreenPoint;
		MouseHandlerState state;
		Timer clickTimer;
		IOfficeScroller officeScroller;
		AutoScroller autoScroller;
		#endregion
		#region Properties
		public bool IsDisposed { get { return isDisposed; } }
		protected virtual bool SupportsTripleClick { get { return false; } }
		protected internal bool Suspended { get { return suspended; } }
		protected internal int ClickCount { get { return clickCount; } set { clickCount = value; } }
		public virtual bool IsControlPressed { get { return DevExpress.Utils.KeyboardHandler.KeyboardHandler.IsControlPressed; } }
		public MouseHandlerState State { get { return state; } }
		public Timer ClickTimer { get { return clickTimer; } }
		public bool IsClickTimerActive { get { return clickTimer != null ? clickTimer.Enabled : false; } }
		public IOfficeScroller OfficeScroller { get { return officeScroller; } }
		public AutoScroller AutoScroller { get { return autoScroller; } }
		#endregion
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(autoScroller != null) {
					autoScroller.Dispose();
					autoScroller = null;
				}
				if(officeScroller != null) {
					officeScroller.Dispose();
					officeScroller = null;
				}
				if(clickTimer != null) {
					clickTimer.Dispose();
					clickTimer = null;
				}
				if(state != null) {
					state.Finish();
					state = null;
				}
			}
			isDisposed = true;
		}
		#endregion
		public virtual void Initialize() {
			this.officeScroller = CreateOfficeScroller();
			this.clickTimer = CreateTimer();
			this.autoScroller = CreateAutoScroller();
			SwitchToDefaultState();
		}
		protected internal virtual Timer CreateTimer() {
			return new Timer();
		}
		#region RunClickTimer
		public virtual void RunClickTimer() {
			ClickTimer.Interval = SystemInformation.DoubleClickTime;
			this.clickTimer.Tick += OnClickTimerTick;
			ClickTimer.Start();
		}
		#endregion
		#region StopClickTimer
		public virtual void StopClickTimer() {
			if(ClickTimer == null)
				return;
			ClickTimer.Stop();
			ClickTimer.Tick -= OnClickTimerTick;
		}
		#endregion
		public virtual void OnMouseDown(PlatformIndependentMouseEventArgs e) {
			if((e.Button & MouseButtons.Middle) != 0) {
				StartOfficeScroller(new Point(e.X, e.Y));
				return;
			}
			CalculateAndSaveHitInfo(e);
			ClickCount++;
			if(IsTripleClick(e)) {
				StopClickTimer();
				HandleMouseTripleClick(ConvertMouseEventArgs(e));
			} else if(IsDoubleClick(e)) {
				StopClickTimer();
				if(SupportsTripleClick) {
					RunClickTimer();
					ClickCount = 2;
				}
				HandleMouseDoubleClick(ConvertMouseEventArgs(e));
			} else {
				StopClickTimer();
				RunClickTimer();
				ClickCount = 1;
				clickScreenPoint = new Point(e.X, e.Y);
				HandleMouseDown(ConvertMouseEventArgs(e));
			}
		}
		public virtual void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			HandleMouseMove(ConvertMouseEventArgs(e));
		}
		public virtual void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			HandleMouseUp(ConvertMouseEventArgs(e));
		}
		public virtual void OnMouseWheel(PlatformIndependentMouseEventArgs e) {
			HandleMouseWheel(ConvertMouseEventArgs(e));
		}
		public virtual void OnClickTimerTick(object sender, EventArgs e) {
			ClickCount = 0;
			if(IsClickTimerActive)
				HandleClickTimerTick();
			StopClickTimer();
		}
		public virtual bool OnPopupMenu(PlatformIndependentMouseEventArgs e) {
			return HandlePopupMenu(ConvertMouseEventArgs(e));
		}
		protected virtual PlatformIndependentMouseEventArgs ConvertMouseEventArgs(PlatformIndependentMouseEventArgs screenMouseEventArgs) {
			return screenMouseEventArgs;
		}
		protected virtual bool IsDoubleClick(PlatformIndependentMouseEventArgs e) {
			return IsMultipleClickCore(e) && ClickCount == 2;
		}
		protected internal virtual bool IsTripleClick(PlatformIndependentMouseEventArgs e) {
			return IsMultipleClickCore(e) && SupportsTripleClick && ClickCount == 3;
		}
		protected internal virtual bool IsMultipleClickCore(PlatformIndependentMouseEventArgs e) {
			if(IsClickTimerActive && e.Button == MouseButtons.Left) {
				return Math.Abs(e.X - clickScreenPoint.X) <= GetDoubleClickSize().Width &&
					Math.Abs(e.Y - clickScreenPoint.Y) <= GetDoubleClickSize().Height;
			} else
				return false;
		}
		protected internal Size GetDoubleClickSize() {
			return SystemInformation.DoubleClickSize;
		}
		public Size GetDragSize() {
			return SystemInformation.DragSize;
		}
		public void Suspend() {
			suspended = true;
		}
		public void Resume() {
			suspended = false;
		}
		#region SwitchStateCore
		public virtual void SwitchStateCore(MouseHandlerState newState, Point mousePosition) {
			if(newState == null)
				return;
			System.Diagnostics.Debug.Assert(!Suspended);
			AutoScroller.Deactivate();
			if(state != null)
				state.Finish();
			state = newState;
			if(state.AutoScrollEnabled)
				AutoScroller.Activate(mousePosition);
			state.Start();
		}
		#endregion
		protected virtual void HandleMouseDoubleClick(PlatformIndependentMouseEventArgs e) {
			State.OnMouseDoubleClick(e);
		}
		protected virtual void HandleMouseTripleClick(PlatformIndependentMouseEventArgs e) {
			State.OnMouseTripleClick(e);
		}
		protected virtual void HandleMouseDown(PlatformIndependentMouseEventArgs e) {
			State.OnMouseDown(e);
		}
		protected virtual void HandleMouseMove(PlatformIndependentMouseEventArgs e) {
			Point mousePosition = new Point(e.X, e.Y);
			CalculateAndSaveHitInfo(e);
			State.OnMouseMove(e);
			AutoScroller.OnMouseMove(mousePosition);
		}
		protected virtual void HandleMouseUp(PlatformIndependentMouseEventArgs e) {
			CalculateAndSaveHitInfo(e);
			State.OnMouseUp(e);
		}
		protected virtual bool HandlePopupMenu(PlatformIndependentMouseEventArgs e) {
			return State.OnPopupMenu(e);
		}
		protected abstract void HandleMouseWheel(PlatformIndependentMouseEventArgs e);
		protected abstract void HandleClickTimerTick();
		protected abstract void CalculateAndSaveHitInfo(PlatformIndependentMouseEventArgs e);
		protected abstract IOfficeScroller CreateOfficeScroller();
		protected abstract AutoScroller CreateAutoScroller();
		protected abstract void StartOfficeScroller(Point clientPoint);
		public abstract void SwitchToDefaultState();
	}
	public abstract class MouseHandlerState {
		#region Fields
		bool isFinished;
		MouseHandler mouseHandler;
		#endregion
		protected MouseHandlerState(MouseHandler mouseHandler) {
			this.mouseHandler = mouseHandler;
		}
		#region Properties
		public MouseHandler MouseHandler { get { return mouseHandler; } }
		public virtual bool AutoScrollEnabled { get { return true; } }
		public bool IsFinished { get { return isFinished; } }
		public virtual bool CanShowToolTip { get { return false; } }
		public virtual bool StopClickTimerOnStart { get { return true; } }
		#endregion
		public virtual void Finish() {
			isFinished = true;
		}
		public virtual void OnCancelState() {
		}
		public virtual void OnMouseMove(PlatformIndependentMouseEventArgs e) {
		}
		public virtual void OnMouseDown(PlatformIndependentMouseEventArgs e) {
		}
		public virtual void OnMouseUp(PlatformIndependentMouseEventArgs e) {
		}
		public virtual void OnMouseDoubleClick(PlatformIndependentMouseEventArgs e) {
		}
		public virtual void OnMouseTripleClick(PlatformIndependentMouseEventArgs e) {
		}
		public virtual void OnMouseWheel(PlatformIndependentMouseEventArgs e) {
		}
		public virtual bool OnPopupMenu(PlatformIndependentMouseEventArgs e) {
			return false;
		}
		public virtual void OnLongMouseDown() {
		}
		public virtual void OnDragEnter(DragEventArgs e) {
		}
		public virtual void OnDragOver(DragEventArgs e) {
		}
		public virtual void OnDragDrop(DragEventArgs e) {
		}
		public virtual void OnDragLeave() {
		}
		public virtual void OnGiveFeedback(GiveFeedbackEventArgs e) {
		}
		public virtual void Start() {
			if(StopClickTimerOnStart)
				MouseHandler.StopClickTimer();
		}
		public virtual void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
		}
		public virtual void OnKeyStateChanged(KeyState keyState) {
		}
		public virtual bool OnPopupMenuShowing() {
			return true;
		}
	}
	public class BeginMouseDragHelperState : MouseHandlerState {
		readonly Point initialPoint;
		readonly MouseHandlerState dragState;
		public BeginMouseDragHelperState(MouseHandler mouseHandler, MouseHandlerState dragState, Point point)
			: base(mouseHandler) {
			if(dragState == null)
				throw new ArgumentNullException();
			this.dragState = dragState;
			this.initialPoint = point;
		}
		public MouseHandlerState DragState { get { return dragState; } }
		public bool CancelOnPopupMenu { get; set; }
		public bool CancelOnRightMouseUp { get; set; }
		public override bool StopClickTimerOnStart { get { return false; } }
		public override void OnMouseWheel(PlatformIndependentMouseEventArgs e) {
			MouseHandler.SwitchStateCore(DragState, Point.Empty);
			DragState.OnMouseWheel(e);
		}
		bool IsDragStarted(PlatformIndependentMouseEventArgs e) {
			Size dragSize = MouseHandler.GetDragSize();
			return Math.Abs(initialPoint.X - e.X) > dragSize.Width || Math.Abs(initialPoint.Y - e.Y) > dragSize.Height;
		}
		public override void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			if(IsDragStarted(e)) {
				MouseHandler.SwitchStateCore(DragState, new Point(e.X, e.Y));
				DragState.OnMouseMove(e);
			}
		}
		public override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			MouseButtons button = e.Button;
			if(button == MouseButtons.Left || (button == MouseButtons.Right && CancelOnRightMouseUp))
				MouseHandler.SwitchToDefaultState();
		}
		public override bool OnPopupMenu(PlatformIndependentMouseEventArgs e) {
			if(CancelOnPopupMenu) {
				MouseHandler.SwitchToDefaultState();
				return false;
			} else
				return base.OnPopupMenu(e);
		}
	}
	public class OfficeMouseWheelEventArgs : PlatformIndependentMouseEventArgs {
		public OfficeMouseWheelEventArgs(MouseButtons buttons, int clicks, int x, int y, int delta)
			: base(buttons, clicks, x, y, delta) {
		}
		public bool IsHorizontal { get; set; }
	}
}
