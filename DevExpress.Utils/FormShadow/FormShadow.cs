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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Win.Hook;
using FormShadowHelpers = DevExpress.Utils.FormShadow.Helpers;
namespace DevExpress.Utils.FormShadow {
	public interface ISupportFormShadow {
		bool IsChildForm(IntPtr handle);
		bool IsActive(IntPtr handle);
	}
	public abstract class FormShadow : IDisposable {
		const int restoreAnimationDurationMs = 300;
		Control controlCore;
		FormShadowHookController hookController;
		byte opacity;
		protected List<ShadowWindow> windowsCore;
		public FormShadow() {
			windowsCore = new List<ShadowWindow>();
			ownerCore = IntPtr.Zero;
			FirstAppearanceAnimationDuration = 300;
			this.opacity = 255;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FirstAppearanceAnimationDuration {
			get;
			set;
		}
		public Control Form {
			get { return controlCore; }
			set {
				if(controlCore == value) return;
				Control oldValue = controlCore;
				controlCore = value;
				OnControlChanged(oldValue, controlCore);
			}
		}
		public byte Opacity {
			get { return opacity; }
			set {
				if(opacity == value) return;
				opacity = value;
				OnOpacityChanged();
			}
		}
		protected void SetZOrderCore(IntPtr bottom, IntPtr top) {
			NativeMethods.SetWindowPos(bottom, top, 0, 0, 0, 0, 0x0002 | 0x0001); 
		}
		public void UpdateZOrder(IntPtr ownerHandle) {
			foreach(var window in windowsCore)
				SetZOrderCore(ownerHandle, window.Handle);
		}
		protected virtual void OnOpacityChanged() {
			foreach(var window in windowsCore) window.Opacity = Opacity;
		}
		internal List<ShadowWindow> Windows {
			get { return windowsCore; }
		}
		IntPtr ownerCore;
		public void SetOwner(IntPtr owner) {
			if(Windows.Count == 0 || owner == IntPtr.Zero || ownerCore.Equals(owner)) return;
			this.ownerCore = owner;
			foreach(var window in Windows)
				NativeMethods.SetWindowLong(window.Handle, FormShadowHelpers.NativeHelper.GWL_HWNDPARENT, owner);
		}
		protected void OnControlChanged(Control old, Control newControl) {
			UnsubscribeControlEvents(old);
			SubscribeControlEvents(newControl);
			if(old != null && old.IsHandleCreated) DestroyShadowWindowHandles();
			if(newControl != null && newControl.IsHandleCreated) InitShadowWindowHandles();
		}
		void UnsubscribeControlEvents(Control control) {
			if(control != null) {
				control.HandleCreated -= controlCore_HandleCreated;
				control.HandleDestroyed -= controlCore_HandleDestroyed;
			}
		}
		void SubscribeControlEvents(Control control) {
			if(control != null) {
				control.HandleCreated += controlCore_HandleCreated;
				control.HandleDestroyed += controlCore_HandleDestroyed;
			}
		}
		void controlCore_HandleDestroyed(object sender, EventArgs e) {
			DestroyShadowWindowHandles();
		}
		void controlCore_HandleCreated(object sender, EventArgs e) {
			InitShadowWindowHandles();
		}
		protected void InitHook() {
			hookController = new FormShadowHookController(this);
		}
		protected abstract ShadowWindow CreateShadowWindow(ShadowWindowTypes hdt);
		protected void InitShadowWindowHandles() {
			InitHook();
			var shadowWindow = CreateShadowWindow(ShadowWindowTypes.Composite);
			shadowWindow.IsGlow = IsGlow;
			shadowWindow.ActiveGlowColor = ActiveGlowColor;
			shadowWindow.InactiveGlowColor = InactiveGlowColor;
			var parentForm = Form.FindForm();
			shadowWindow.IsActive = (parentForm != null) && parentForm == System.Windows.Forms.Form.ActiveForm;
			Windows.Add(shadowWindow);
			foreach(var window in Windows)
				window.EnsureHandle();
			UpdateShadowWindowsWithDelayIfNecessary();
		}
		bool isGlow;
		public virtual bool IsGlow {
			get { return isGlow; }
			set {
				if(isGlow == value) return;
				isGlow = value;
				OnIsGlowChangedCore();
			}
		}
		void OnIsGlowChangedCore() {
			if(IsLockUpdate) return;
				OnIsGlowChanged();
			}
		Color activeGlowColor;
		public virtual Color ActiveGlowColor {
			get { return activeGlowColor; }
			set {
				if(activeGlowColor == value) return;
				activeGlowColor = value;
				OnActiveGlowColorChangedCore();
			}
		}
		void OnActiveGlowColorChangedCore() {
			if(IsLockUpdate) return;
				OnActiveGlowColorChanged();
			}
		Color inactiveGlowColor;
		public virtual Color InactiveGlowColor {
			get { return inactiveGlowColor; }
			set {
				if(inactiveGlowColor == value) return;
				inactiveGlowColor = value;
				OnInactiveGlowColorChangedCore();
			}
		}
		void OnInactiveGlowColorChangedCore() {
			if(IsLockUpdate) return;
				OnInactiveGlowColorChanged();
			}
		protected virtual void OnIsGlowChanged() { }
		protected virtual void OnActiveGlowColorChanged() { }
		protected virtual void OnInactiveGlowColorChanged() { }
		protected void DestroyShadowWindowHandles() {
			StopTimer();
			isShadowWindowVisible = false;
			if(hookController != null)
				hookController.Dispose();
			if(!HasWindows)
				return;
			var windowsCopy = Windows.ToArray();
			for(int i = 0; i < windowsCopy.Length; i++)
				windowsCopy[i].Dispose();
			Windows.Clear();
		}
		bool IsWindowNormal(NativeMethods.ShowWindowCommands placement) {
			return placement == NativeMethods.ShowWindowCommands.Restore || placement == NativeMethods.ShowWindowCommands.Normal;
		}
		bool IsWindowMaximized(NativeMethods.ShowWindowCommands placement) {
			return placement == NativeMethods.ShowWindowCommands.Maximize || placement == NativeMethods.ShowWindowCommands.ShowMaximized;
		}
		internal IntPtr HwndSourceHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			ISupportFormShadow formShadow = controlCore as ISupportFormShadow;
			switch(msg) {
				case MSG.WM_ACTIVATE: {
						if(hWnd != controlCore.Handle && (formShadow == null || !formShadow.IsChildForm(hWnd)))
							return new IntPtr(1);
						int lowWord = FormShadowHelpers.NativeHelper.LOWORD(wParam);
						if(lowWord == FormShadowHelpers.NativeHelper.WA_ACTIVE || lowWord == FormShadowHelpers.NativeHelper.WA_CLICKACTIVE) {
							ActiveStateChanged(true);
						}
						else if(lowWord == MSG.WA_INACTIVE) {
							if(formShadow == null || !formShadow.IsActive(hWnd))
								ActiveStateChanged(false);
						}
						handled = true;
						return new IntPtr(1);
					}
				case MSG.WM_WINDOWPOSCHANGED:
					NativeMethods.ShowWindowCommands placement = GetWindowPlacement(hWnd);
					if(IsWindowMaximized(Form.Handle)) {
						HideFormShadow();
						StopTimer();
						restoreAnimation = false;
						handled = true;
						return IntPtr.Zero;
					}
					if(restoreAnimation) {
						handled = true;
						return IntPtr.Zero;
					}
					if(hWnd != controlCore.Handle || firstAppearance) {
						if(IsWindowNormal(placement) && IsWindowMaximized(prevWindowPlacement)) {
							UpdateShadowWindowPositions(true);
						}
						UpdateShadowWindowPos(GetFormRectangle());
						handled = true;
						return IntPtr.Zero;
					}
					WindowPosChanged();
					if(restoreAnimation)
						HideFormShadow();
					handled = true;
					return IntPtr.Zero;
				default:
					return IntPtr.Zero;
			}
		}
		void ActiveStateChanged(bool active) {
			if(!HasWindows) return;
			foreach(var window in Windows)
				window.IsActive = active;
			if(!active) {
				if(NativeMethods.IsWindow10)
					HideFormShadow();
			}
			else {
				UpdateShadowWindowPositions(false);
			}
		}
		bool firstAppearance = true;
		bool restoreAnimation = false;
		protected void InvalidateShadowWindows() {
			UpdateShadowWindowPositions(false);
			HideFormShadow();
			RefreshFormShadow();
		}
		void HideFormShadow() {
			foreach(var window in Windows)
				window.HideWnd();
			SetShadowWindowsVisibility(false);
		}
		void RefreshFormShadow() {
			foreach(var window in Windows)
				window.RefreshWindow();
		}
		void UpdateShadowWindows(bool delayIfNecessary, bool delayWhileMove) {
			if(delayIfNecessary) {
				if(Timer != null) {
					if(!firstAppearance)
						Timer.Stop();
				}
				else {
					this.timerCore = new Timer();
					Timer.Tick += Timer_Tick;
					if(firstAppearance)
						Timer.Interval = FirstAppearanceAnimationDuration;
					else
						Timer.Interval = restoreAnimationDurationMs;
				}
				if(!firstAppearance)
					restoreAnimation = true;
				Timer.Start();
				return;
			}
			if(firstAppearance)
				return;
			if(Timer != null)
				StopTimer();
			if(!HasWindows)
				return;
			UpdateShadowWindowPositions(delayWhileMove);
		}
		Rectangle prevControlRect;
		void UpdateShadowWindowsWithDelayIfNecessary() {
			var formRect = GetFormRectangle();
			UpdateShadowWindowPos(formRect);
			var shoulDelay = ShouldDelay(Form.Handle);
			var delayWhileMove = ShouldDelayWhileMove(formRect, prevControlRect);
			UpdateShadowWindows(shoulDelay, delayWhileMove);
			prevControlRect = formRect;
		}
		Rectangle GetFormRectangle() {
			NativeMethods.RECT rect = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(Form.Handle, ref rect);
			return rect.ToRectangle();
		}
		void UpdateShadowWindowPos(Rectangle formRect) {
			foreach(var window in Windows) {
				window.UpdateWindowPos(formRect);
			}
		}
		bool ShouldDelayWhileMove(Rectangle currentCtrlBounds, Rectangle previousCtrlBounds) {
			bool res = false;
			var shadowWindow = GetShadowWindowByType(ShadowWindowTypes.Composite);
			if(shadowWindow == null) shadowWindow = GetShadowWindowByType(ShadowWindowTypes.Left);
			var leftBorderBounds = ShadowWindowLayoutCalculator.Calculate(ShadowWindowTypes.Left, currentCtrlBounds, shadowWindow.GetPainter());
			var bordersWidth = leftBorderBounds.Width;
			var bordersOffset = shadowWindow.GetPainter().GetOffsetByWindowType(ShadowWindowTypes.Left);
			if(bordersOffset != 0 && (Math.Abs(previousCtrlBounds.Width - currentCtrlBounds.Width) > bordersWidth || Math.Abs(previousCtrlBounds.Height - currentCtrlBounds.Height) > bordersWidth)) res = true;
			return res;
		}
		ShadowWindow GetShadowWindowByType(ShadowWindowTypes type) {
			if(Windows.Count == 0) return null;
			foreach(var window in Windows) {
				if(window.WindowType == type)
					return window;
			}
			return null;
		}
		Timer timerCore;
		protected Timer Timer {
			get { return timerCore; }
		}
		void Timer_Tick(object sender, EventArgs e) {
			if(firstAppearance)
				firstAppearance = false;
			StopTimer();
			if(restoreAnimation)
				UpdateShadowWindowPos(GetFormRectangle());
			UpdateShadowWindows(false, false);
			restoreAnimation = false;
		}
		void StopTimer() {
			if(Timer != null) {
				Timer.Stop();
				Timer.Tick -= Timer_Tick;
				Timer.Dispose();
			}
			this.timerCore = null;
		}
		void WindowPosChanged() {
			UpdateShadowWindowsWithDelayIfNecessary();
		}
		bool ShouldShowShadowWindow {
			get {
				if(Form == null || Form.Handle == IntPtr.Zero)
					return false;
				if(Form.Width == 0 || Form.Height == 0)
					return false;
				if(IsDesignMode)
					return false;
				return NativeMethods.IsWindowVisible(Form.Handle) && !NativeMethods.IsIconic(Form.Handle) && !NativeMethods.IsZoomed(Form.Handle);
			}
		}
		bool? isDesignMode;
		bool IsDesignMode {
			get {
				if(!isDesignMode.HasValue) {
					using(var process = System.Diagnostics.Process.GetCurrentProcess()) {
						isDesignMode = (process.ProcessName == "devenv");
					}
				}
				return isDesignMode.Value;
			}
		}
		bool isShadowWindowVisible;
		protected bool HasWindows {
			get { return (Windows != null) && Windows.Count > 0; }
		}
		void UpdateShadowWindowPositions(bool delayWhileMove) {
			if(!HasWindows) return;
			SetShadowWindowsVisibility(ShouldShowShadowWindow);
			UpdateZOrder();
			foreach(var window in Windows)
				window.CommitChanges(restoreAnimation, delayWhileMove, AllowResizeViaShadows);
		}
		void UpdateZOrder() {
			if(!HasWindows) return;
			IntPtr handle = controlCore.Handle;
			foreach(var window in Windows) {
				SetZOrderOfWindows(window.Handle, handle);
				handle = window.Handle;
			}
		}
		void SetZOrderOfWindows(IntPtr hWndFirst, IntPtr hWndSecond) {
			if(hWndFirst == IntPtr.Zero)
				return;
			if(NativeMethods.GetWindow(hWndFirst, NativeMethods.GW_HWNDPREV) != hWndSecond)
				NativeMethods.SetWindowPos(hWndFirst, hWndSecond, 0, 0, 0, 0, NativeMethods.SWP.SWP_NOMOVE | NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOACTIVATE);
		}
		NativeMethods.ShowWindowCommands prevWindowPlacement;
		bool ShouldDelay(IntPtr hWnd) {
			NativeMethods.ShowWindowCommands showCmd = GetWindowPlacement(hWnd);
			bool res = false;
			if(showCmd == NativeMethods.ShowWindowCommands.Normal
				&& (prevWindowPlacement == NativeMethods.ShowWindowCommands.Maximize
					|| prevWindowPlacement == NativeMethods.ShowWindowCommands.ShowMaximized
					|| prevWindowPlacement == NativeMethods.ShowWindowCommands.Minimize
					|| prevWindowPlacement == NativeMethods.ShowWindowCommands.ShowMinimized
					|| prevWindowPlacement == NativeMethods.ShowWindowCommands.Hide)) {
				res = true;
			}
			if(showCmd == NativeMethods.ShowWindowCommands.Hide)
				firstAppearance = true;
			prevWindowPlacement = showCmd;
			return res;
		}
		bool IsWindowMaximized(IntPtr hWnd) {
			NativeMethods.ShowWindowCommands showCmd = GetWindowPlacement(hWnd);
			if(showCmd == NativeMethods.ShowWindowCommands.Maximize || showCmd == NativeMethods.ShowWindowCommands.ShowMaximized) {
				prevWindowPlacement = showCmd;
				return true;
			}
			return false;
		}
		NativeMethods.ShowWindowCommands GetWindowPlacement(IntPtr hWnd) {
			NativeMethods.WINDOWPLACEMENT windowPlacement;
			NativeMethods.GetWindowPlacement(hWnd, out windowPlacement);
			NativeMethods.ShowWindowCommands showCmd = windowPlacement.ShowCmd;
			return showCmd;
		}
		void SetShadowWindowsVisibility(bool isVisible) {
			if(isShadowWindowVisible == isVisible)
				return;
			if(!HasWindows)
				return;
			foreach(var window in Windows)
				window.IsVisible = isVisible;
			isShadowWindowVisible = isVisible;
		}
		#region IDisposable Members
		public void Dispose() {
			if(hookController != null)
				hookController.Dispose();
			DestroyShadowWindowHandles();
		}
		#endregion
		public static bool AllowResizeViaShadows { get; set; }
		int lockUpdateCount;
		protected internal bool IsLockUpdate {
			get { return lockUpdateCount > 0; }
		}
		protected internal virtual void BeginUpdate() {
			lockUpdateCount++;
		}
		protected internal virtual void EndUpdate() {
			if(--lockUpdateCount == 0)
				OnUpdate();
		}
		void OnUpdate() {
			if(HasWindows) {
				foreach(var window in Windows) {
					window.IsGlow = IsGlow;
					window.InactiveGlowColor = InactiveGlowColor;
					window.ActiveGlowColor = ActiveGlowColor;
				}
			}
			UpdateShadowWindowPositions(false);
		}
	}
	public class FormShadowHookController : IHookController, IDisposable {
		FormShadow ownerCore;
		public FormShadowHookController(FormShadow owner) {
			ownerCore = owner;
			HookManager.DefaultManager.AddController(this);
		}
		public bool InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return false;
		}
		public bool InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			bool handled = false;
			ownerCore.HwndSourceHook(HWnd, Msg, WParam, LParam, ref handled);
			return handled;
		}
		public IntPtr OwnerHandle {
			get {
				if(ownerCore == null || ownerCore.Form == null)
					return IntPtr.Zero;
				return ownerCore.Form.Handle;
			}
		}
		public void Dispose() {
			HookManager.DefaultManager.RemoveController(this);
			this.ownerCore = null;
		}
	}
	public class XtraFormShadow : FormShadow {
		protected override ShadowWindow CreateShadowWindow(ShadowWindowTypes hdt) {
			return new XtraFormShadowWindow(this, hdt);
		}
		protected override void OnActiveGlowColorChanged() {
			foreach(var window in Windows)
				window.ActiveGlowColor = ActiveGlowColor;
			InvalidateShadowWindows();
		}
		protected override void OnInactiveGlowColorChanged() {
			foreach(var window in Windows)
				window.InactiveGlowColor = InactiveGlowColor;
			InvalidateShadowWindows();
		}
		protected override void OnIsGlowChanged() {
			foreach(var window in Windows)
				window.IsGlow = IsGlow;
			InvalidateShadowWindows();
		}
	}
	public class ToolbarShadow : FormShadow {
		protected override ShadowWindow CreateShadowWindow(ShadowWindowTypes hdt) {
			return new ToolbarShadowWindow(this, hdt);
		}
	}
	public class RibbonFormShadow : FormShadow {
		protected override ShadowWindow CreateShadowWindow(ShadowWindowTypes hdt) {
			return new RibbonFormShadowWindow(this, hdt);
		}
		protected override void OnActiveGlowColorChanged() {
			foreach(var window in Windows)
				window.ActiveGlowColor = ActiveGlowColor;
			InvalidateShadowWindows();
		}
		protected override void OnInactiveGlowColorChanged() {
			foreach(var window in Windows)
				window.InactiveGlowColor = InactiveGlowColor;
			InvalidateShadowWindows();
		}
		protected override void OnIsGlowChanged() {
			foreach(var window in Windows)
				window.IsGlow = IsGlow;
			InvalidateShadowWindows();
		}
	}
	public class BeakFormShadow : FormShadow {
		protected override ShadowWindow CreateShadowWindow(ShadowWindowTypes hdt) {
			return new BeakFormShadowWindow(this, hdt);
		}
	}
}
