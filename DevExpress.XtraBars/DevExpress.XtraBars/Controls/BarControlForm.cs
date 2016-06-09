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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DevExpress.XtraBars;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Objects;
using DevExpress.XtraBars.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors.Popup;
namespace DevExpress.XtraBars.Forms {
	public interface IFormContainedControl {
		Size CalcSize(int width, int maxHeight);
		void CalcViewInfo();
		void SetParentForm(ControlForm form);
		Size FormMimimumSize { get;}
	}
	public class ControlForm : CustomFloatingForm, IBarObject { 
		[Flags]
		protected enum SizingModeEnum { None = 0, Left = 1, Top = 2, Right = 4, Bottom = 8};
		static Cursor[] SizingCursors = new Cursor[] { 
					Cursors.Default, Cursors.SizeWE, Cursors.SizeNS, Cursors.SizeNWSE,
				Cursors.SizeWE, null, Cursors.SizeNESW, null, Cursors.SizeNS, Cursors.SizeNESW, null, Cursors.SizeNS, Cursors.SizeNWSE};
		Control containedControl;
		protected TitleBarEl fTitleBar;
		protected ControlFormViewInfo fViewInfo;
		protected Point fLastCursorPos;
		protected SizingModeEnum fSizingMode;
		bool creatingHandle, internalSizing;
		bool canDispose, destroying, animating;
		MouseButtons lastMouseButtons;
		Animator animator = null;
		BarManager manager;
		int lockUpdate = 0;
		internal LocationInfo locationInfo;
		bool hiddenControl = false;
		ShadowManager shadows;
		internal ControlForm(BarManager manager, Control containedControl) : this(manager, containedControl, FormBehavior.Custom) { }
		internal ControlForm(BarManager manager, Control containedControl, FormBehavior behavior) {
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.Opaque, false);
			this.manager = manager;
			this.shadows = new ShadowManager(this);
			Reset();
			this.containedControl = containedControl;
			Controls.Add(ContainedControl);
			destroying = false;
			canDispose = true;
			internalSizing = creatingHandle = false;
			if(IContainedControl != null) IContainedControl.SetParentForm(this);
			this.fViewInfo = CreateViewInfo();
			InitializeBehavior(behavior);
			this.fSizingMode = SizingModeEnum.None;
			this.fTitleBar = null;
			Init();
			CreateTitleBar();
			this.SizeGripStyle = SizeGripStyle.Show;
			this.StartPosition = FormStartPosition.Manual;
			this.MaximizedBounds = Rectangle.Empty;
			this.MaximizeBox = false;
			this.Visible = false;
			if(AllowInitialize) Initialize();
		}
		protected virtual void InitializeBehavior(FormBehavior behavior) {
		}
		internal void SetStyleCore(ControlStyles flag, bool value) {
			SetStyle(flag, value);
		}
		protected internal void BaseSetVisibleCore(bool newVisible) {
			base.SetVisibleCore(newVisible);
		}
		protected internal virtual bool BaseAllowMouseActivate { get { return base.AllowMouseActivate; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(CanDispose) {
					this.shadows.Dispose();
					DestroyShadowTimer();
					destroying = true;
					DestroyTitleBar();
					base.Dispose(disposing);
				}
				else {
					Location = BarManager.zeroPoint;
					Size = new Size(0, 0);
				}
			} else base.Dispose(disposing);
		}
		public override Color BackColor {
			get {
				if(manager.GetController().LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
					return BarSkins.GetSkin(manager.GetController().LookAndFeel).GetSystemColor(base.BackColor);
				return base.BackColor;
			}
			set {
				base.BackColor = value;
			}
		}
		public override Color ForeColor {
			get {
				if(manager.GetController().LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
					return BarSkins.GetSkin(manager.GetController().LookAndFeel).GetSystemColor(base.ForeColor);
				return base.ForeColor;
			}
			set {
				base.ForeColor = value;
			}
		}
		protected internal virtual ShadowManager Shadows { get { return shadows; } }
		protected virtual void Reset() {
			this.lastMouseButtons = MouseButtons.None;
			this.animating = false;
			Animator = null;
			this.locationInfo = null;
			this.fLastCursorPos = Point.Empty;
		}
		protected internal virtual void HideControl() {
			this.hiddenControl = true;
			this.locationInfo = null;
			ClientSize = Size.Empty;
			DestroyContainedControl();
		}
		protected virtual void DestroyContainedControl() {
			if(this.containedControl != null) {
				if(IContainedControl != null) IContainedControl.SetParentForm(null);
				Controls.Remove(this.containedControl);
				this.containedControl.Dispose();
				this.containedControl = null;
			}
		}
		protected virtual bool AllowInitialize { get { return true; } }
		protected internal virtual void Initialize() {
			this.creatingHandle = true;
			try {
			} finally {
				creatingHandle = false;
			}
			UpdateSizeT();
		}
		protected internal void UpdateSizeT() {
			this.internalSizing = true;
			try {
				this.ClientSize = CalcSizeByWidth(-1, MaxFormHeight);
				UpdateWidth();
			}
			finally {
				this.internalSizing = false;
			}
		}
		protected virtual void UpdateWidth() {
		}
		protected override void WndProc(ref Message msg) {
			if(msg.Msg == 0x000A && msg.WParam.ToInt32() == 0) { 
				if(Manager != null && Manager.Form != null) {
					Form form = Manager.Form.FindForm();
					if (form != null && BarNativeMethods.IsWindowEnabled(form.Handle)) {
						BarNativeMethods.EnableWindow(Handle, true);
						return;
					}
				}
			}
			base.WndProc(ref msg);
		}
		protected virtual int MaxFormHeight { get { return -1; } }
		protected virtual void Init() {
		}
		protected internal virtual void UpdateScheme() {
			this.fViewInfo = CreateViewInfo();
			DestroyTitleBar();
			CreateTitleBar();
			LayoutChanged();
		}
		public virtual Animator Animator { 
			get { return animator; } 
			set { 
				if(Animator == value) return;
				animator = value; 
				OnAnimatorChanged();
			} 
		}
		void OnAnimatorChanged() {
			if(Animator == null) {
				AllowTransparency = false;
			}
		}
		public bool Animating { 
			get { return animating; } 
			set { 
				if(Animating == value) return;
				animating = value; 
				if(!value) 
					OnAnimationEnd();
				else
					OnAnimationStart();
			} 
		}
		protected virtual void OnAnimationStart() { }
		protected virtual void OnAnimationEnd() {
			CreateShadowsCore();
			Update();
		}
		public virtual BarManager Manager { get { return manager; } }
		public virtual Control ContainedControl { 
			get { return containedControl; } 
			internal set {
				Control prev = ContainedControl;
				containedControl = value;
				OnContainedControlChanged(prev);
			} 
		}
		protected virtual void OnContainedControlChanged(Control prev) {
			if(prev != null)
				Controls.Remove(prev);
			if(ContainedControl != null)
				Controls.Add(ContainedControl);
		}
		public virtual IFormContainedControl IContainedControl { get { return ContainedControl as IFormContainedControl; } }
		protected void OnCloseItemClick(object sender, EventArgs e) {
			HideDockableObject();
		}
		protected virtual bool IsNeedTitleBar { get { return false; } }
		protected virtual void DestroyTitleBar() {
			if(TitleBar != null) {
				this.fTitleBar.CloseItemClick -= new EventHandler(OnCloseItemClick);
				this.fTitleBar.Dispose();
				this.fTitleBar = null;
			}
		}
		protected virtual void CreateTitleBar() {
			if(!IsNeedTitleBar) return;
			this.fTitleBar = ViewInfo.CreateTitleBarInstance();
			this.fTitleBar.CloseItemClick += new EventHandler(OnCloseItemClick);
			this.fTitleBar.Selected = true;
			UpdateTitleBar();
			this.fTitleBar.Control = this;
		}
		protected internal virtual void UpdateTitleBar() {
		}
		protected virtual void HideDockableObject() {
		}
		public virtual TitleBarEl TitleBar { get { return fTitleBar; } }
		public virtual bool ShowTitleBar { get { return false; } }
		internal bool Destroying { get { return destroying; } }
		internal bool CanDispose { get { return canDispose; } set { canDispose = value; } }
		bool IBarObject.ShouldCloseOnLostFocus(Control newFocus) { return true; }
		bool IBarObject.IsBarObject { get { return true; } }
		BarManager IBarObject.Manager { get { return Manager;} }
		BarMenuCloseType IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			return BarMenuCloseType.None;
		}
		public virtual bool ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) { return true; }
		public virtual ControlFormViewInfo ViewInfo { get { return fViewInfo; } }
		protected SizingModeEnum SizingMode {
			get { return fSizingMode; }
			set { 
				if(SizingMode == value) return;
				if(SizingMode == SizingModeEnum.None) OnStartSizing();
				fSizingMode = value;
				if(SizingMode != SizingModeEnum.None) Capture = true;
				Cursor.Current = SizingCursors[(int)fSizingMode];
				if(SizingMode == SizingModeEnum.None) OnEndSizing();
			}
		}
		protected virtual void OnStartSizing() {
			Manager.SelectionInfo.CloseAllPopups();
		}
		protected virtual void OnEndSizing() {
		}
		protected internal void UpdateZOrderCore(IntPtr after) {
			UpdateZOrder(after);
		}
		protected override void UpdateZOrder(IntPtr after) {
			base.UpdateZOrder(after);
			Shadows.Update();
		}
		internal void CreateShadowsInternal() {
			this.Shadows.Move(ShadowOwnerBounds);
		}
		protected internal Rectangle ShadowOwnerBounds { 
			get {
				if(!Manager.DrawParameters.Constants.AllowLinkShadows) return Rectangle.Empty;
				IPopup popup = ContainedControl as IPopup;
				return popup != null ? popup.PopupOwnerRectangle : Rectangle.Empty;
			}
		}
		Timer shadowTimer = null;
		protected virtual void StartShadowTimer() {
			if(shadowTimer == null) {
				shadowTimer = new Timer( );
				shadowTimer.Tick += new EventHandler(OnTimerTick);
			}
			shadowTimer.Interval = 70;
			shadowTimer.Enabled = true;
		}
		protected virtual void DestroyShadowTimer() {
			if(this.shadowTimer != null) {
				this.shadowTimer.Tick -= new EventHandler(OnTimerTick);
				this.shadowTimer.Dispose();
				this.shadowTimer = null;
			}
		}
		protected void OnTimerTick(object sender, EventArgs e) {
			DestroyShadowTimer();
			CreateShadowsCore();
		}
		protected virtual void CreateShadowsCore() {
			DestroyShadowTimer();
			if(!HasShadow || AllowSystemShadow) return;
			Shadows.ShadowSize = Manager.DrawParameters.Constants.FloatingShadowSize;
			Shadows.Show(ShadowOwnerBounds);
		}
		protected virtual void CreateShadows() {
			if(AllowSystemShadow) return;
			if(!HasShadow || !Visible || ClientSize.IsEmpty || Manager.IsCustomizing) {
				DestroyShadowTimer();
				Shadows.Hide();
				return;
			}
			StartShadowTimer();
		}
		protected internal virtual void CalcFormSize(ref Size res, bool bAdd) {
			Size formSize = ViewInfo.CalcMinSize();
			int m = bAdd ? 1 : -1;
			res.Width += m * formSize.Width;
			res.Height += m * formSize.Height;
			IFormContainedControl fc = ContainedControl as IFormContainedControl;
			if(fc == null) return;
			res.Width = Math.Max(res.Width, fc.FormMimimumSize.Width);
			res.Height = Math.Max(res.Height, fc.FormMimimumSize.Height);
		}
		protected virtual Size CalcSizeByHeight(int width, int heightDelta) {
			return Size.Empty;
		}
		protected internal virtual Size CalcSizeByWidthCore(int width, int maxFormHeight) {
			if(LocationInfo != null && !LocationInfo.ForceFormBounds.IsEmpty) return LocationInfo.ForceFormBounds.Size;
			if(IContainedControl != null) {
				Size res = IContainedControl.CalcSize(width, maxFormHeight);
				CalcFormSize(ref res, true);
				return res;
			}
			return Size.Empty;
		}
		protected virtual Size CalcSizeByWidth(int width, int maxFormHeight) {
			return CalcSizeByWidthCore(width, maxFormHeight);
		}
		public virtual bool HasShadow { get { return false; } } 
		protected override bool HasSystemShadow { get { return AllowSystemShadow; } }
		protected virtual bool AllowSystemShadow {
			get { return Manager != null && Manager.PaintStyle is DevExpress.XtraBars.Styles.SkinBarManagerPaintStyle; }
		}
		protected virtual ControlFormViewInfo CreateViewInfo() {
			return Manager.Helper.CreateControlViewInfo(this) as ControlFormViewInfo;
		}
		protected virtual bool CanCornerSizing { get { return false; } }
		protected virtual bool CanSized { get { return false; } }
		internal void OnPaintCore(PaintEventArgs e) {
			e.Graphics.FillRectangle(SystemBrushes.Control, ClientRectangle);
			if(!ViewInfo.IsReady)
				UpdateViewInfo();
			bool needHide = BarManager.NeedHideCursor(this);
			if(!Animating && needHide) Cursor.Hide();
			GraphicsInfoArgs dre = new GraphicsInfoArgs(new DevExpress.Utils.Drawing.GraphicsCache(e), ClientRectangle);
			if(Animator != null && ContainedControl != null && Animator.RequireFormPaint) {
				PaintContainedControl(e);
			}
			ViewInfo.Painter.Draw(dre, ViewInfo, null);
			dre.Cache.Dispose();
			if(!Animating && needHide) Cursor.Show();
		}
		internal void OnPaintBackgroundCore(PaintEventArgs e) {
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			OnPaintBackgroundCore(e);
		}
		protected override void OnPaint(PaintEventArgs e) {
			OnPaintCore(e);   
		}
		protected virtual void PaintContainedControl(PaintEventArgs e) {
			Point offset = ViewInfo.ContentRect.Location;
			e.Graphics.FillRectangle(Brushes.White, ViewInfo.ContentRect);
			GraphicsState state = e.Graphics.Save();
			e.Graphics.TranslateTransform((Animator.ChildPosition.X - offset.X) + offset.X,
				(Animator.ChildPosition.Y - offset.Y) + offset.Y);
			PaintEventArgs np = new PaintEventArgs(e.Graphics, Rectangle.Empty);
			InvokePaint(ContainedControl, np);
			e.Graphics.Restore(state);
		}
		protected override void OnMove(EventArgs e) {
			if(Animating) return;
			UpdateZOrder(IntPtr.Zero);
			base.OnMove(e);
		}
		public virtual void UpdateZ() {
			UpdateZOrder(IntPtr.Zero);
		}
		protected internal virtual void OnVisibleChangedCore(EventArgs e) {
			base.OnVisibleChanged(e);
			if(Visible) CreateShadows();
		}
		protected override void OnVisibleChanged(EventArgs e) {
			OnVisibleChangedCore(e);
		}
		public bool IsOpenedDown {
			get {
				if(locationInfo == null) return false;
				if(locationInfo.VerticalOpen) {
					if(locationInfo.ShowPoint.Y == Location.Y) return true;
				}
				return false;
			}
		}
		protected internal virtual LocationInfo LocationInfo { get { return locationInfo; } }
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			OnResizeCore(e);
		}
		public bool BoundsAnimating { get; set; }
		protected virtual void OnResizeCore(EventArgs e) {
			if(this.creatingHandle || Animating || BoundsAnimating) return;
			if(!internalSizing) {
				LayoutChanged();
				if(Visible) UpdateLocationInfo();
				return;
			}
			if(Visible) {
				UpdateLocationInfo();
			}
			Shadows.Move(ShadowOwnerBounds);
		}
		bool suppressUpdateLocationInfo;
		protected internal bool SuppressUpdateLocationInfo { get { return suppressUpdateLocationInfo; } set { suppressUpdateLocationInfo = value; } }
		internal void UpdateLocationInfoCore() {
			if(locationInfo == null || SuppressUpdateLocationInfo) return;
			locationInfo.WindowSize = Size;
			Point location = locationInfo.ShowPoint;
			Location = location;
		}
		protected virtual void UpdateLocationInfo() {
			UpdateLocationInfoCore();
		}
		protected internal virtual void UpdateViewInfo() {
			ViewInfo.CalcViewInfo(null, this, ClientRectangle);
			if(ContainedControl != null) {
				ContainedControl.Bounds = ViewInfo.ControlRect;
			}
		}
		protected virtual void UpdateSize() {
			if(!LocationInfo.ForceFormBounds.IsEmpty)
				ClientSize = LocationInfo.ForceFormBounds.Size;
			else
				ClientSize = CalcSizeByWidth(-2, MaxFormHeight);
			UpdateViewInfo();
			Invalidate();
		}
		protected virtual Size ClientFormSize {
			get { return ClientSize; }
		}
		public void BeginUpdate() {
			this.lockUpdate ++;
		}
		public void EndUpdate() {
			if(--this.lockUpdate == 0) LayoutChanged();
		}
		public void CancelUpdate() {
			--this.lockUpdate;
		}
		public bool IsLockUpdate { get { return lockUpdate != 0; } }
		public virtual void LayoutChanged() {
			if(IsLockUpdate || this.hiddenControl) return;
			if(Destroying) return;
			LayoutChangedCore();
		}
		protected internal Size CalcBestSize() {
			Size size = ClientFormSize;
			CalcFormSize(ref size, false);
			return CalcSizeByWidth(size.Width, MaxFormHeight == -1 ? -1 : MaxFormHeight - (ClientSize.Height - size.Height));
		}
		protected internal void BaseLayoutChangedCore() {
			SuspendLayout();
			try {
				UpdateTitleBar();
				try {
					internalSizing = true;
					ClientSize = CalcBestSize();
				}
				finally {
					internalSizing = false;
				}
				UpdateViewInfo();
			}
			finally {
				ResumeLayout();
			}
			Invalidate();
			Shadows.Move(ShadowOwnerBounds);
		}
		protected virtual void LayoutChangedCore() {
			BaseLayoutChangedCore();
		}
		protected SizingModeEnum CheckCanSize(Point p) {
			const int cx = 4;
			SizingModeEnum result = SizingModeEnum.None;
			if(!CanSized) return result;
			if(Math.Abs(p.Y - Bounds.Y) < cx) 
				result |= SizingModeEnum.Top;
			else
				if(Math.Abs(p.Y - Bounds.Bottom) < cx) result |= SizingModeEnum.Bottom;
			if(!CanCornerSizing && result != SizingModeEnum.None) return result;
			if(Math.Abs(p.X - Bounds.X) < cx) result |= SizingModeEnum.Left;
			else
				if(Math.Abs(p.X - Bounds.Right) < cx) result |= SizingModeEnum.Right;
			return result;
		}
		protected virtual void SetFormBounds(Rectangle newBounds, SizingModeEnum smode) {
			if(((smode & SizingModeEnum.Top) != 0) && newBounds.Bottom != Bounds.Bottom) return;
			if(((smode & SizingModeEnum.Left) != 0) && newBounds.Right != Bounds.Right) return;
			if(Bounds == newBounds) return;
			Bounds = newBounds;
			Update();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if(Animating) return;
			lastMouseButtons = e.Button;
			base.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if(Animating) return;
			if(SizingMode != SizingModeEnum.None) {
				if(this.fLastCursorPos != Control.MousePosition) {
					Rectangle newBounds = Bounds;
					int direction = Control.MousePosition.Y - this.fLastCursorPos.Y;
					if((SizingMode & SizingModeEnum.Right) != 0)
						newBounds.Width = Control.MousePosition.X - newBounds.X;
					if((SizingMode & SizingModeEnum.Left) != 0)
						newBounds.Width = (newBounds.Right - Control.MousePosition.X);
					if((SizingMode & SizingModeEnum.Top) != 0) {
						if((direction < 0 && Control.MousePosition.Y > Bounds.Top) ||
						   (direction > 0 && Control.MousePosition.Y < Bounds.Top)) {
							this.fLastCursorPos = Control.MousePosition;
							return;
						}
						newBounds.Height = newBounds.Bottom - Control.MousePosition.Y;
					}
					if((SizingMode & SizingModeEnum.Bottom) != 0) {
						if((direction < 0 && Control.MousePosition.Y > Bounds.Bottom) ||
							(direction > 0 && Control.MousePosition.Y < Bounds.Bottom)) {
							this.fLastCursorPos = Control.MousePosition;
							return;
						}
						newBounds.Height = Control.MousePosition.Y - newBounds.Y;
					}
					if(newBounds.Width < 8) return;
					if(newBounds.Height < 5) return;
					this.fLastCursorPos = Control.MousePosition;
					Size newSize;
					Size size = newBounds.Size;
					CalcFormSize(ref size, false);
					newSize = size;
					if((SizingMode & (SizingModeEnum.Right | SizingModeEnum.Left)) != 0) 
						newSize = CalcSizeByWidth(size.Width, -1);
					if((SizingMode & (SizingModeEnum.Top | SizingModeEnum.Bottom)) != 0) 
						newSize = CalcSizeByHeight(newSize.Width, newBounds.Height - Bounds.Height);
					newBounds.Size = newSize;
					if(newSize != Bounds.Size) {
						if((SizingMode & SizingModeEnum.Left) != 0)
							newBounds.Offset(Bounds.Right - newBounds.Right, 0);
						if((SizingMode & SizingModeEnum.Top) != 0) {
							newBounds.Offset(0, Bounds.Bottom - newBounds.Bottom);
						}
						SetFormBounds(newBounds, SizingMode);
					}
				}
				return;
			}
			SizingModeEnum result = CheckCanSize(Control.MousePosition);
			if(result != SizingModeEnum.None && !IsFormDragging) {
				Cursor.Current = SizingCursors[(int)result];
				return;
			}
			base.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if(Animating) return;
			if(Destroying) return;
			lastMouseButtons = e.Button;
			if(SizingMode != SizingModeEnum.None) {
				SizingMode = SizingModeEnum.None;
				return;
			}
			base.OnMouseUp(e);
		}
		protected override void OnLostCapture() { 
			SizingMode = SizingModeEnum.None;
			base.OnLostCapture();
		}
		protected internal virtual void BaseOnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
		}
		protected internal void BaseOnClosing(CancelEventArgs e) {
			base.OnClosing(e);
		}
		protected internal bool BaseProcessDialogKey(Keys keyData) {
			return base.ProcessDialogKey(keyData);
		}
	}
	public class ControlFormBehavior {
		public ControlFormBehavior(ControlForm form, BarManager manager, Control containedControl) {
			Form = form;
			if(Form is ISupportFormBehavior) {
				((ISupportFormBehavior)Form).Behavior = this;
			}
			Manager = manager;
			ContainedControl = containedControl;
		}
		protected ControlForm Form {
			get;
			set;
		}
		protected virtual void OnFormChanged() {
			throw new NotImplementedException();
		}
		protected BarManager Manager { get; set; }
		protected Control ContainedControl { get; set; }
		public virtual void SetVisibleCore(bool newVisible) {
			Form.BaseSetVisibleCore(newVisible);
		}
		public virtual void UpdateWidth() { }
		public virtual Size CalcSizeByWidth(int width, int maxFormHeight) {
			return Form.CalcSizeByWidthCore(width, maxFormHeight);
		}
		public virtual bool HasShadow { get { return false; } }
		public virtual void OnKeyDown(KeyEventArgs e) { }
		public virtual bool IsTopMost { get { return true; } }
		public virtual int MaxFormHeight { get { return -1; } }
		public virtual void OnClosing(CancelEventArgs e) { }
		public virtual bool ProcessDialogKey(Keys keyData) { return false; }
		public virtual void CallBaseSetVisibleCore(bool newVisible) {
			Form.BaseSetVisibleCore(newVisible);
		}
		public virtual void OnVisibleChanged() {
			Form.OnVisibleChangedCore(EventArgs.Empty);
		}
		public virtual bool AllowMouseActivate { get { return Form.BaseAllowMouseActivate; } }
		public virtual bool AllowInitialize { get { return true; } }
		public virtual void UpdateLocationInfo() {
			Form.UpdateLocationInfoCore();
		}
		public virtual void LayoutChangedCore() {
			Form.BaseLayoutChangedCore();
		}
		public virtual void OnPaint(PaintEventArgs e) {
			Form.OnPaintCore(e);
		}
		public virtual void OnPaintBackground(PaintEventArgs e) {
			Form.OnPaintBackgroundCore(e);
		}
	}
	public class SubMenuControlFormBehavior : ControlFormBehavior {
		public SubMenuControlFormBehavior(ControlForm form, BarManager manager, Control containedControl) : base(form, manager, containedControl) {
			Form.AllowTransparency = DevExpress.XtraBars.Controls.Animator.AllowFadeAnimation;
		}
		public override void SetVisibleCore(bool newVisible) {
			base.SetVisibleCore(newVisible);
			if(newVisible && !IsTopMost) {
				Form.UpdateZOrderCore(new IntPtr(-1));
			}
		}
		public override void UpdateWidth() {
			Form.Width = Math.Max(Form.Width, GetUpdatedMinWidth());
		}
		private int GetUpdatedMinWidth() {
			PopupMenuBarControl pc = ContainedControl as PopupMenuBarControl;
			if(pc != null && pc.Menu != null)
				return pc.Menu.MinWidth;
			SubMenuBarControl sb = ContainedControl as SubMenuBarControl;
			BarSubItemLink link = sb == null ? null : sb.ContainerLink as BarSubItemLink;
			if(link != null)
				return ((BarSubItem)link.Item).PopupMinWidth;
			return 0;
		}
		public override Size CalcSizeByWidth(int width, int maxFormHeight) {
			Size res = Form.CalcSizeByWidthCore(width, maxFormHeight);
			return new Size(Math.Max(res.Width, GetUpdatedMinWidth()), res.Height);
		}
		public override bool HasShadow { get { return Manager != null && Manager.GetController().PropertiesBar.SubmenuHasShadow && !Form.Animating; } }
		public override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.KeyData == (Keys.Alt | Keys.F4)) {
				Manager.SelectionInfo.ClosePopup(ContainedControl as IPopup);
				return;
			}
			Form.BaseOnKeyDown(e);
		}
		public override bool IsTopMost {
			get { return Manager.IsDesignMode || Manager.IsOnTopMostForm || GetActivatorForm() != null; }
		}
		Form GetActivatorForm() {
			PopupMenuBarControl pc = ContainedControl as PopupMenuBarControl;
			if(pc != null && pc.Menu != null && pc.Menu.Activator is Form) {
				return pc.Menu.Activator as Form;
			}
			return null;
		}
		bool closing = false;
		public override void OnClosing(CancelEventArgs e) {
			if(!closing) {
				e.Cancel = true;
				closing = true;
				Manager.SelectionInfo.ClosePopup(ContainedControl as IPopup);
				return;
			}
			Form.BaseOnClosing(e);
		}
		public override int MaxFormHeight {
			get {
				if(Form.LocationInfo == null) return -1;
				return Form.LocationInfo.CalcMaxHeight();
			}
		}
	}
	public enum FormBehavior { Custom, SubMenu, PopupContainer, Gallery }
	public class PopupContainerFormBehavior : ControlFormBehavior {
		public PopupContainerFormBehavior(ControlForm form, BarManager manager, Control containedControl) : base(form, manager, containedControl) {
			Form.TabStop = false;
		}
		public override bool HasShadow { get { return Manager.GetController().PropertiesBar.SubmenuHasShadow; } }
		public override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape || keyData == (Keys.Alt | Keys.F4)) {
				Manager.SelectionInfo.ClosePopup(ContainedControl as IPopup);
				return true;
			}
			return Form.BaseProcessDialogKey(keyData);
		}
		public override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyData == Keys.Escape || e.KeyData == (Keys.Alt | Keys.F4)) {
				Manager.SelectionInfo.ClosePopup(ContainedControl as IPopup);
				return;
			}
			Form.BaseOnKeyDown(e);
		}
		bool closing = false;
		public override void OnClosing(CancelEventArgs e) {
			if(!closing) {
				e.Cancel = true;
				closing = true;
				Manager.SelectionInfo.ClosePopup(ContainedControl as IPopup);
				return;
			}
			Form.BaseOnClosing(e);
		}
	}
	public interface ISupportFormBehavior {
		ControlFormBehavior Behavior { get; set; }
	}
	public class SubMenuControlForm : ControlForm, IDXPopupMenuForm, ISupportXtraAnimation, ISupportFormBehavior, ISupportTopMost, ISupportToolTipsForm {
		public ControlFormBehavior Behavior { get; set; }
		public SubMenuControlForm(BarManager manager, Control containedControl, FormBehavior behavior) : base(manager, containedControl, behavior) {
		}
		protected override void InitializeBehavior(FormBehavior behavior) {
			switch(behavior) { 
				case FormBehavior.Gallery:
					Behavior = new GalleryDropDownFormBehavior(((GalleryDropDownBarControl)ContainedControl).GalleryMenu, this, Manager, ContainedControl);
					break;
				case FormBehavior.PopupContainer:
					Behavior = new PopupContainerFormBehavior(this, Manager, ContainedControl);
					break;
				case FormBehavior.SubMenu:
					Behavior = new SubMenuControlFormBehavior(this, Manager, ContainedControl);
					break;
			}
		}
		protected override bool AllowMouseActivate {
			get {
				return Behavior.AllowMouseActivate;
			}
		}
		protected override bool AllowInitialize {
			get {
				return Behavior.AllowInitialize;
			}
		}
		protected override void UpdateLocationInfo() {
			Behavior.UpdateLocationInfo();
		}
		protected override void LayoutChangedCore() {
			Behavior.LayoutChangedCore();
		}
		Form GetActivatorForm() {
			PopupMenuBarControl pc = ContainedControl as PopupMenuBarControl;
			if(pc != null && pc.Menu != null && pc.Menu.Activator is Form) {
				return pc.Menu.Activator as Form;
			}
			return null;
		}
		protected override void SetVisibleCore(bool newVisible) {
			if(Behavior == null)
				base.SetVisibleCore(newVisible);
			else 
				Behavior.SetVisibleCore(newVisible);
		}
		protected override void UpdateWidth() {
			Behavior.UpdateWidth();
		}
		protected virtual int GetUpdatedMinWidth() { 
			PopupMenuBarControl pc = ContainedControl as PopupMenuBarControl;
			if(pc != null && pc.Menu != null)
				return pc.Menu.MinWidth;
			SubMenuBarControl sb = ContainedControl as SubMenuBarControl;
			BarSubItemLink link = sb == null? null: sb.ContainerLink as BarSubItemLink;
			if(link != null)
				return ((BarSubItem)link.Item).PopupMinWidth;
			return 0;
		}
		protected override Size CalcSizeByWidth(int width, int maxFormHeight) {
			return Behavior.CalcSizeByWidth(width, maxFormHeight);
		}
		public override bool HasShadow { get { return Behavior.HasShadow; } }
		protected override void OnKeyDown(KeyEventArgs e) {
			Behavior.OnKeyDown(e);
		}
		protected override bool IsTopMost { get { return Behavior.IsTopMost; } }
		protected override int MaxFormHeight { 
			get {
				return Behavior.MaxFormHeight;
			} 
		}
		protected override void OnVisibleChanged(EventArgs e) {
			if(Behavior == null)
				base.OnVisibleChanged(e);
			else 
				Behavior.OnVisibleChanged();
		}
		protected override void OnClosing(CancelEventArgs e) {
			Behavior.OnClosing(e);
		}
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
		}
		bool ISupportTopMost.IsTopMost {
			get { return true; }
		}
		bool ISupportToolTipsForm.ShowToolTipsFor(Form form) {
			return false;
		}
		bool ISupportToolTipsForm.ShowToolTipsWhenInactive {
			get { return true; }
		}
	}
	public class PopupContainerForm : ControlForm { 
		public PopupContainerForm(BarManager manager, Control containedControl) : base(manager, containedControl) {
			this.TabStop = false;
		}
		public override bool HasShadow { get { return Manager.GetController().PropertiesBar.SubmenuHasShadow;} }
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape || keyData == (Keys.Alt | Keys.F4)) {
				Manager.SelectionInfo.ClosePopup(ContainedControl as IPopup);
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyData == Keys.Escape || e.KeyData == (Keys.Alt | Keys.F4)) {
				Manager.SelectionInfo.ClosePopup(ContainedControl as IPopup);
				return;
			}
			base.OnKeyDown(e);
		}
		bool closing = false;
		protected override void OnClosing(CancelEventArgs e) {
			if(!closing) {
				e.Cancel = true;
				closing = true;
				Manager.SelectionInfo.ClosePopup(ContainedControl as IPopup);
				return;
			}
			base.OnClosing(e);
		}
	}
	public class FloatingBarControlForm : ControlForm {
		BarQMenuAddRemoveButtonsItem addRemoveButtons;
		BarItemLink addRemoveButtonsLink;
		PopupMenu menu;
		public FloatingBarControlForm(BarManager manager, Control containedControl) : base(manager, containedControl) {
			Size = new Size(10, 10);
			this.SetStyle(ControlStyles.StandardDoubleClick, true);
			this.addRemoveButtons = new BarQMenuAddRemoveButtonsItem(Manager);
			this.addRemoveButtonsLink = addRemoveButtons.CreateLink(null, this) as BarSubItemLink;
			this.addRemoveButtonsLink.BeginGroup = true;
			this.addRemoveButtonsLink.ownerControl = BarControl;
			this.menu = new PopupMenu();
			this.menu.Popup += new EventHandler(OnQuickMenuPopup);
			this.menu.ItemLinks.Add(addRemoveButtonsLink);
			this.menu.Manager = Manager;
			UpdateTitleBar();
			this.BarControl.ClearHash();
			this.Location = BarControl.FloatLocation;
			Initialize();
		}
		protected override bool AllowInitialize { get { return false; } }
		public override void LayoutChanged() {
			base.LayoutChanged();
			if(this.menu != null && this.menu.SubControl != null) {
				this.menu.SubControl.Form.CreateShadowsInternal();
				this.menu.SubControl.LayoutChanged();
			}
		}
		void OnQuickMenuPopup(object sender, EventArgs e) {
			Bar.CreateQuickCustomizationLinks(this.addRemoveButtons, true);
		}
		public virtual FloatingBarControl BarControl { get { return base.ContainedControl as FloatingBarControl; } }
		public virtual Bar Bar { get { return BarControl != null ? BarControl.Bar : null; } } 
		protected override void Dispose(bool disposing) {
			if(disposing && !CanDispose) {
				Visible = false;
			}
			if(disposing && CanDispose) {
				if(this.menu != null)
					this.menu.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override IntPtr InsertAfterWindow {
			get { 
				if(Manager.SelectionInfo.ModalDialogActive) {
					Form form = Manager.OForm; 
					if(form != null) return form.Handle;
				}
				return base.InsertAfterWindow;
			}
		}
		protected override void UpdateZOrder(IntPtr after) {
			Form form = Manager.OForm;
			if(Manager.SelectionInfo.ModalDialogActive) {
				if(form != null) {
					form.AddOwnedForm(this);
					return;
				}
			} else {
				if(form != null) form.RemoveOwnedForm(this);
			}
			base.UpdateZOrder(after);
		}
		public override void UpdateZ() {
			UpdateZOrder(IntPtr.Zero);
			SetVisibleCore(true);
			Enabled = !Manager.SelectionInfo.ModalDialogActive;
		}
		protected internal override void UpdateTitleBar() {
			if(TitleBar != null && Bar != null) {
				TitleBar.Menu = this.menu;
				TitleBar.ShowCloseButton = !Bar.OptionsBar.DisableClose && !Bar.IsMainMenu;
				TitleBar.ShowMenuButton = Bar.OptionsBar.AllowQuickCustomization;
			}
		}
		protected override void DraggingChanged() { 
			if(Bar != null) {
				((IDockableObject)Bar).IsDragging = IsFormDragging;
			}
		}
		protected override void HideDockableObject() {
			if(Bar != null)
				Bar.Visible = false;
		}
		public override bool HasShadow { get { return false;} }
		protected override bool IsNeedTitleBar { get { return Bar.OptionsBar.AllowCaptionWhenFloating; } }
		protected override void OnMove(EventArgs e) {
			base.OnMove(e);
			if(Location.X != BarManager.zeroPoint.X) {
				SetDockObjectFloatLocation(Location);
			}
		}
		protected virtual void SetDockObjectFloatLocation(Point p) {
			if(Bar != null) {
				Bar.floatLocation = p;
			}
		}
		ArrayList widthes = null;
		protected virtual void CreateWidthesCache() {
			if(widthes != null) return;
			widthes = new ArrayList();
			Size res, ores = Size.Empty;
			int mw = 10;
			while(mw < 1000) {
				res = IContainedControl.CalcSize(mw, -1);
				if(res != ores) {
					ores = res;
					widthes.Add(res);
					if(mw < ores.Width) mw = ores.Width + 1;
				} else {
					mw += 10;
				}
			}
		}
		protected override Size CalcSizeByHeight(int width, int heightDelta) {
			CreateWidthesCache();
			Size res = Size.Empty;
			res = IContainedControl.CalcSize(Bar.GetFloatWidht(), -1);
			CalcFormSize(ref res, true);
			Point p = PointToClient(Control.MousePosition);
			int clientDelta = (SizingMode & SizingModeEnum.Bottom) != 0 ?  ClientSize.Height - p.Y : p.Y;
			if(Math.Abs(clientDelta) < 16) return res;
			int curIndex = widthes.IndexOf(Bar.FloatSize);
			if(curIndex < 0) return res;
			if(clientDelta < 0) {
				res = (Size)(curIndex > 0 ? widthes[curIndex - 1] : widthes[curIndex]);
			} else {
				res = (Size)(curIndex < widthes.Count - 1 ? widthes[curIndex + 1] : widthes[widthes.Count - 1]);
			}
			if(Bar != null) Bar.SetFloatSize(res);
			CalcFormSize(ref res, true);
			return res;
		}
		protected override void OnEndSizing() {
			base.OnEndSizing();
			BarControl.Bar.SetFloatSize(BarControl.ClientSize);
		}
		protected override Size ClientFormSize {
			get { 
				if(SizingMode != SizingModeEnum.None) return ClientSize;
				return CalcSizeByWidth(Bar.GetFloatWidht(), -1); 
			}
		}
		protected override void UpdateLocationInfo() {
		}
		protected override bool CanSized { get { return true; } }
		protected override void OnDoubleClick(EventArgs e) {
			Bar.SwitchDockStyle();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			bool md = BarControl.IsRequireMouseDown(e);
			if(!md) {
				if(e.Clicks == 1) {
					if(e.Button == MouseButtons.Left) {
						SizingModeEnum result = CheckCanSize(Control.MousePosition);
						if(result != SizingModeEnum.None) {
							this.fLastCursorPos = Control.MousePosition;
							widthes = null;
							SizingMode = result;
							return;
						}
						if(Bar != null)
							Manager.SelectionInfo.CloseAllPopups();
						if(Bar != null && Bar.CanMove) {
							Bar.StartMoving(this);
							return;
						}
					}
				}
			}
			base.OnMouseDown(e);
		}
	}
}
