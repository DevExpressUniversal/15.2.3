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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Popups;
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public abstract class SimplePopupBaseForm : XtraForm, IPopupForm {
		BaseEdit _ownerEdit;
		object _savedEditValue;
		int lockUpdate;
		ShadowManager shadows;
		PopupBaseFormPainter fPainter;
		PopupBaseFormViewInfo _viewInfo;
		public event EventHandler ResultValueChanged;
		public static bool ForceRemotingCompatibilityMode = false;
		public static bool ShowPopupWithinParentForm = false;
		public static event QueryAvailablePopupBoundsEventHandler QueryAvailablePopupBounds;
		internal static void RaiseGetPopupParentBounds(Control control, BoundsEventArgs e) {
			if(QueryAvailablePopupBounds != null)
				QueryAvailablePopupBounds(control, e);
		}
		public SimplePopupBaseForm(BaseEdit ownerEdit) {
			this._ownerEdit = ownerEdit;
			this.fPainter = CreatePainter();
			this._viewInfo = CreateViewInfo();
			this._savedEditValue = null;
			this.lockUpdate = 0;
			this.shadows = new ShadowManager(this);
			this.KeyPreview = true;
			this.LookAndFeel.Assign(OwnerEdit.LookAndFeel);
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			this.StartPosition = FormStartPosition.Manual;
			this.FormBorderStyle = FormBorderStyle.None;
			this.ShowInTaskbar = false;
			this.ControlBox = false;
		}
		protected virtual PopupBaseFormPainter CreatePainter() {
			return new PopupBaseFormPainter();
		}
		protected virtual PopupBaseFormPainter Painter { get { return fPainter; } }
		protected virtual PopupBaseFormViewInfo CreateViewInfo() {
			return new PopupBaseFormViewInfo(this);
		}
		protected virtual PopupBaseFormViewInfo ViewInfo { get { return _viewInfo; } }
#if DXWhidbey
		protected override bool ShowWithoutActivation {
			get {
				return true;
			}
		}
#endif
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(DXAccessible == null) return base.CreateAccessibilityInstance();
			return DXAccessible.Accessible;
		}
		DevExpress.Accessibility.BaseAccessible dxAccessible;
		protected internal virtual DevExpress.Accessibility.BaseAccessible DXAccessible {
			get {
				if(dxAccessible == null) dxAccessible = CreateAccessibleInstance();
				return dxAccessible;
			}
		}
		protected virtual DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.Accessibility.ContainerBaseAccessible(this);
		}
		public virtual bool AllowMouseClick(Control control, Point mousePosition) {
			return AllowMouseClickCore(control, mousePosition);
		}
		protected override void OnPaint(PaintEventArgs e) {
			DevExpress.Utils.Mdi.ControlState.CheckPaintError(this);
			if(!ViewInfo.IsReady) UpdateViewInfo(e.Graphics);
			GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(e));
			PopupFormGraphicsInfoArgs args = new PopupFormGraphicsInfoArgs(ViewInfo, cache, ClientRectangle);
			if(WindowsFormsSettings.GetIsRightToLeftLayout(this)) {
				UpdateClipBoundsInRTL(cache);
			}
			Painter.Draw(args);
			cache.Dispose();
		}
		private void UpdateClipBoundsInRTL(GraphicsCache cache) {
			cache.Graphics.SetClip(new Rectangle(ClientRectangle.X - 1, ClientRectangle.Y, ClientRectangle.Width + 2, ClientRectangle.Height));
		}
		[DXCategory(CategoryName.Focus)]
		public virtual bool FormContainsFocus { get { return ContainsFocus; } }
		public virtual void BeginUpdate() {
			++lockUpdate;
		}
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				LayoutChanged();
			}
		}
		public virtual void CancelUpdate() {
			--lockUpdate;
		}
		protected virtual Control EmbeddedControl { get { return null; } }
		protected virtual Control SeparatorControl { get { return null; } }
		internal int SeparatorControlHeight {
			get {
				if(SeparatorControl == null) return 0;
				return SeparatorControl.Height;
			}
		}
		[Browsable(false)]
		public virtual BaseEdit OwnerEdit { get { return _ownerEdit; } }
		Control toFocus;
		protected internal virtual void FocusFormControl(Control control) {
			if(control == null) control = toFocus;
			this.toFocus = null;
			if(control == null) return;
			if(OwnerEdit != null && IsImeInProgress) {
				toFocus = control;
				return;
			}
			Form form = OwnerEdit.FindForm();
			XtraForm xform = form as XtraForm;
			if(xform != null) xform.SuspendRedraw();
			try {
				SuppressDeactivation = true;
				XtraForm.DeactivatedForm = OwnerEdit.FindForm();
				control.Focus();
				SuppressDeactivation = false;
				XtraForm.DeactivatedForm = null;
				if(form == null || !form.IsHandleCreated) return;
				ServiceObject.EmulateFormFocus(form.Handle);
				if(form.MdiParent != null && form.MdiParent.IsHandleCreated) ServiceObject.EmulateFormFocus(form.MdiParent.Handle);
			}
			finally {
				if(xform != null) xform.ResumeRedraw();
			}
		}
		public void ClosePopup() {
			ClosePopup(PopupCloseMode.Normal);
		}
		protected virtual IPopupServiceControl ServiceObject { get { return OwnerEdit.ServiceObject; } }
		protected virtual bool UseSimpleVisible { get { return true; } }
		protected override void SetVisibleCore(bool newVisible) {
			Form form = OwnerEdit.FindForm();
			if(ServiceObject != null) {
#if DXWhidbey //Related to WinUtils
				if(UseSimpleVisible)
					ServiceObject.SetSimpleVisibleCore(this, form == null ? IntPtr.Zero : form.Handle, newVisible);
				else
					ServiceObject.SetVisibleCore(this, newVisible);
				base.SetVisibleCore(newVisible);
				return;
#else
				CreateControl();
				bool res = false;
				if(UseSimpleVisible) {
					res = ServiceObject.SetSimpleVisibleCore(this, form == null ? IntPtr.Zero : form.Handle, newVisible);
				}
				else {
					res = ServiceObject.SetVisibleCore(this, newVisible);
				}
				if(res) {
					MethodInfo mi = typeof(Control).GetMethod("SetState", BindingFlags.NonPublic | BindingFlags.Instance);
					if(mi != null) mi.Invoke(this, new Object[] { 2, true});
					OnVisibleChanged(EventArgs.Empty);
					OnLoad(EventArgs.Empty);
					return;
				}
#endif
			}
			base.SetVisibleCore(newVisible);
		}
		const int SC_CLOSE = 0x0F060;
		const int SC_MASK = 0x0FFF0;
		const int WM_SYSCOMMAND = 0x0112;
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if(ServiceObject != null && ServiceObject.WndProc(ref m))
				return;
			if(m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt32() & SC_MASK) == SC_CLOSE)
				return;
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
#if OLDSHADOWS
			if(m.Msg == 0x0047 || m.Msg == 0x18 || m.Msg == 0x000A) Shadows.Update();
#endif
		}
		protected virtual bool AllowLayoutChanged { get { return IsHandleCreated; } }
		protected virtual void LayoutChanged() {
			if(lockUpdate != 0) return;
			if(!AllowLayoutChanged) return;
			UpdateViewInfo();
			Invalidate();
		}
		int lockUpdatePositions = 0;
		protected void LockUpdateControlPositions() { lockUpdatePositions++; }
		protected void UnlockUpdateControlPositions() { lockUpdatePositions--; }
		protected void UpdateControlPositions() {
			if(this.lockUpdatePositions != 0) return;
			UpdateControlPositionsCore();
		}
		protected virtual void UpdateControlPositionsCore() {
			if(EmbeddedControl != null) {
				EmbeddedControl.Bounds = ViewInfo.ContentRect;
				EmbeddedControl.Visible = true;
			}
			if(SeparatorControl != null) {
				ViewInfo.UpdateEmbeddedControlPosition(EmbeddedControl, SeparatorControl.Height);
				SeparatorControl.Location = ViewInfo.GetSeparatorLineLocation(SeparatorControl.Height);
				SeparatorControl.Width = ViewInfo.Bounds.Width - 2;
				SeparatorControl.Visible = true;
			}
		}
		#region Remoting compatibility tricks
		Timer timer;
		protected void ForceUpdateRemoting() {
			if(ForceRemotingCompatibilityMode) {
				ClearTimer();
				InitTimer();
			}
		}
		private void InitTimer() {
			timer = new Timer();
			timer.Interval = 1;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Enabled = true;
		}
		void ClearTimer() {
			if(this.timer != null) {
				this.timer.Stop();
				this.timer.Dispose();
				this.timer = null;
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				ClearTimer();   
			}
		}
		void timer_Tick(object sender, EventArgs e) {
			if(timer == null) return;
			try {
				timer.Enabled = false;
				timer.Dispose();
				timer = null;
				if(!IsHandleCreated || !Visible) return;
				this.Left++;
				this.Left--;
				XUpdateZOrder();
			}
			catch { }
		}
		void XUpdateZOrder() {
			if(!IsHandleCreated || Owner == null || !Owner.IsHandleCreated) return;
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SetWindowPos(Handle, new IntPtr(-1), 0, 0, 0, 0,
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOACTIVATE |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOMOVE |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOSIZE |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOOWNERZORDER |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOSENDCHANGING);
		}
		#endregion
		public Size CalcFormSize() {
			Size res = CalcFormSizeCore();
			res.Width = Math.Max(res.Width, MinFormSize.Width);
			res.Height = Math.Max(res.Height, MinFormSize.Height);
			return res;
		}
		public virtual Size CalcFormSize(Size contentSize) {
			LockUpdateControlPositions();
			try {
				if(!ViewInfo.IsReady) UpdateViewInfo();
				return ViewInfo.CalcSizeByContentSize(contentSize);
			}
			finally {
				UnlockUpdateControlPositions();
			}
		}
		protected override void OnLocationChanged(EventArgs e) {
			base.OnLocationChanged(e);
		}
		protected internal virtual void UpdateViewInfo(Graphics g) {
			ViewInfo.CalcViewInfo(g, ClientRectangle);
			UpdateControlPositions();
		}
		protected internal virtual void UpdateViewInfo() {
			UpdateViewInfo(null);
		}
		protected virtual Size DefaultMinFormSize { get { return new Size(10, 10); } }
		protected virtual Size MinFormSize {
			get {
				Size res = DefaultMinFormSize;
				if(PopupFormMinSize.Width > 0) res.Width = PopupFormMinSize.Width;
				if(PopupFormMinSize.Height > 0) res.Height = PopupFormMinSize.Height;
				return res;
			}
		}
		public virtual void ProcessKeyPress(KeyPressEventArgs e) {
		}
		protected override bool IsInputKey(Keys keyData) {
			bool res = base.IsInputKey(keyData);
			if(res || IsNeededKey(keyData)) return true;
			return false;
		}
		protected internal void OnKeyDownCore(KeyEventArgs e) {
			OnKeyDown(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(!e.Handled) {
				ProcessKeyDown(e);
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(!e.Handled) {
				ProcessKeyUp(e);
			}
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(!Visible) return;
			if(!e.Handled) {
				ProcessKeyPress(e);
			}
		}
		public virtual void ProcessKeyUp(KeyEventArgs e) {
		}
		public virtual void ProcessKeyDown(KeyEventArgs e) {
			if(e.Handled) return;
			if(e.KeyCode == Keys.Tab && AllowPopupTabOut) {
				e.Handled = true;
				ProcessPopupTabKey(e);
				return;
			}
			if(e.KeyData == Keys.Escape) {
				if(ProcessEscapeKeyDown(e))
					return;
			}
			if((e.KeyData == CloseUpKey.Key && CloseUpKey.IsExist) || e.KeyData == (Keys.Down | Keys.Alt)) {
				e.Handled = true;
				ClosePopup(PopupCloseMode.CloseUpKey);
				return;
			}
		}
		protected virtual bool ProcessEscapeKeyDown(KeyEventArgs e) {
			if(AllowCloseByEscape) {
				e.Handled = true;
				CancelPopup();
				return true;
			}
			return false;
		}
		protected virtual bool IgnoreOldEditValue { get { return false; } }
		protected internal virtual object QueryOldEditValue() {
			return OldEditValue;
		}
		protected internal virtual object QueryResultValue() {
			return ResultValue;
		}
		[DXCategory(CategoryName.Appearance)]
		public virtual object OldEditValue {
			get {
				if(IgnoreOldEditValue) return ResultValue;
				return _savedEditValue;
			}
			set { _savedEditValue = value; }
		}
		protected virtual void RaiseResultValueChanged(EventArgs e) {
			if(ResultValueChanged != null) ResultValueChanged(this, e);
		}
		public virtual void ShowPopupForm() {
			this.isPopupFormClosing = true;
			this._savedEditValue = OwnerEdit.EditValue;
			this.Visible = true;
			SubscribeFormEvents();
			this.Update();
			this.isPopupFormClosing = false;
			ForceUpdateRemoting();
		}
		bool isPopupFormClosing = false;
		protected bool IsPopupFormClosing { get { return isPopupFormClosing; } }
		public virtual void HidePopupForm() {
			this.isPopupFormClosing = true;
			UnsubscribeFormEvents();
			if(ContainsFocus) {
				Form form = OwnerEdit.FindForm();
				if(form != null && Form.ActiveForm == form) form.Activate();
				OwnerEdit.Focus();
			}
			this.Visible = false;
		}
		protected virtual void SubscribeFormEvents() {
			Form form = OwnerEdit.FindForm();
			if(form == null) return;
			form.Resize += new EventHandler(OnForm_Resize);
			form.Move += new EventHandler(OnForm_Move);
			if(PopupBaseForm.ForceRemotingCompatibilityMode) {
				form.VisibleChanged -= new EventHandler(OnForm_VisibleChanged);
				form.VisibleChanged += new EventHandler(OnForm_VisibleChanged);
			}
		}
		protected virtual void UnsubscribeFormEvents() {
			if(OwnerEdit == null) return;
			Form form = OwnerEdit.FindForm();
			if(form == null) return;
			form.Resize -= new EventHandler(OnForm_Resize);
			form.Move -= new EventHandler(OnForm_Move);
		}
		protected void OnForm_VisibleChanged(object sender, EventArgs e) {
			Form frm = OwnerEdit.FindForm();
			if(frm != null && !frm.Visible) {
				frm.VisibleChanged -= new EventHandler(OnForm_VisibleChanged);
				DestroyPopupForm();
			}
		}
		protected void OnForm_Resize(object sender, EventArgs e) {
			if(Visible && OwnerEdit != null) ClosePopup(PopupCloseMode.Immediate);
		}
		protected void OnForm_Move(object sender, EventArgs e) {
			if(Visible && OwnerEdit != null) ClosePopup(PopupCloseMode.Immediate);
		}
		public virtual void ForceCreateHandle() {
			if(IsHandleCreated) return;
			BeginUpdate();
			try {
				Size clientSize = ClientSize;
				CreateHandle();
				ClientSize = clientSize;
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual bool AllowMouseClickCore(Control control, Point mousePosition) {
			if(PopupHelper.IsBelowModalForm(this, true)) return true;
			return control == this || Contains(control);
		}
		#region Shadows
#if OLDSHADOWS
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			DestroyShadowTimer();
			if(disposing)
				Shadows.Dispose();
			base.Dispose(disposing);
		}
		Timer shadowTimer = null;
		protected virtual void StartShadowTimer() {
			if(shadowTimer == null) {
				shadowTimer = new Timer();
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
		protected void CreateShadowsCore() {
			if(!Visible || !CanShowShadow) return;
			Rectangle r = OwnerEdit.RectangleToScreen(OwnerEdit.ClientRectangle);
			if(!OwnerEdit.AltBounds.IsEmpty) r = OwnerEdit.RectangleToScreen(new Rectangle(Point.Empty, OwnerEdit.AltBounds.Size));
			Shadows.Show(r);
		}
		[DXCategory(CategoryName.Appearance)]
		public virtual ShadowManager Shadows { get { return shadows; } }
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			LayoutChanged();
			Shadows.Move();
		}
		protected override void OnMove(EventArgs e) {
			base.OnMove(e);
			Shadows.Move();
		}
		public void HideShadows() {
			Shadows.Hide();
		}
		bool lockUpdateShadows = false;
		public void ShowHideShadows() {
			if(this.lockUpdateShadows) return;
			try {
				this.lockUpdateShadows = true;
				bool visible = Visible && Bounds.X != -10000 && CanShowShadow;
				if(visible) {
					StartShadowTimer();
				}
				else {
					DestroyShadowTimer();
					HideShadows();
				}
			}
			catch {}
			finally {
				this.lockUpdateShadows = false;
			}
		}
#else
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			LayoutChanged();
		}
		public void ShowHideShadows() { }
		public void HideShadows() { }
#endif
		#endregion Shadows
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				if(CanShowShadow && (System.Environment.OSVersion.Version.Major > 5 || (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor > 0)))
					cp.ClassStyle |= 0x20000; 
				return cp;
			}
		}
		protected virtual bool CanShowShadow {
			get { return true; }
		}
		public virtual void CheckClosePopup(Control activeControl) { }
		protected override bool GetAllowSkin() { return false; }
		protected internal virtual void ResetResultValue() { }
		protected virtual bool IsNeededKey(Keys keyData) {
			return false;
		}
		protected virtual void ProcessPopupTabKey(KeyEventArgs e) {
			ClosePopup(PopupCloseMode.Normal);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			LayoutChanged();
			ShowHideShadows();
		}
		[DXCategory(CategoryName.Properties)]
		public virtual RepositoryItem Properties { get { return OwnerEdit == null ? null : OwnerEdit.Properties; } }
		protected virtual bool AllowPopupTabOut { get { return false; } }
		protected virtual KeyShortcut CloseUpKey { get { return KeyShortcut.Empty; } }
		protected virtual bool AllowCloseByEscape { get { return true; } }
		protected abstract Size PopupFormMinSize { get; }
		protected abstract void CancelPopup();
		protected abstract void ClosePopup(PopupCloseMode closeMode);
		protected abstract void DestroyPopupForm();
		[DXCategory(CategoryName.Appearance)]
		public abstract object ResultValue { get; }
		protected abstract Size CalcFormSizeCore();
		protected abstract bool IsImeInProgress { get; }
		public abstract AppearanceObject AppearanceDropDown { get; }
		public abstract PopupBorderStyles PopupBorderStyle { get; }
	}
	public interface ISupportTopMost {
		bool IsTopMost { get; }
	}
	[ToolboxItem(false)]
	public abstract class PopupBaseForm : SimplePopupBaseForm, ISupportTopMost {
		public PopupBaseForm(PopupBaseEdit ownerEdit)
			: base(ownerEdit) {
		}
		public override bool AllowMouseClick(Control control, Point mousePosition) {
			bool allow = AllowMouseClickCore(control, mousePosition);
			PopupAllowClickEventArgs e = new PopupAllowClickEventArgs(mousePosition, control, allow);
			OwnerEdit.RaisePopupAllowClick(e);
			return e.Allow;
		}
		public override AppearanceObject AppearanceDropDown { get { return Properties.AppearanceDropDown; } }
		public override PopupBorderStyles PopupBorderStyle { get { return Properties.PopupBorderStyle; } }
		[DXCategory(CategoryName.Properties)]
		new public virtual RepositoryItemPopupBase Properties { get { return OwnerEdit == null ? null : OwnerEdit.Properties; } }
		[Browsable(false)]
		public new virtual PopupBaseEdit OwnerEdit { get { return base.OwnerEdit as PopupBaseEdit; } }
		protected override void OnPaintBackground(PaintEventArgs e) { }
		protected internal virtual void OnBeforeShowPopup() { }
		protected override void DestroyPopupForm() {
			if(OwnerEdit != null)
				OwnerEdit.DestroyPopupForm();
		}
		protected override void ClosePopup(PopupCloseMode closeMode) {
			if(OwnerEdit == null)
				return;
			OwnerEdit.ClosePopup(closeMode);
		}
		protected override void ProcessPopupTabKey(KeyEventArgs e) {
			if(OwnerEdit != null)
				OwnerEdit.ProcessPopupTabKey(e);
		}
		protected override void CancelPopup() {
			if(OwnerEdit != null)
				OwnerEdit.CancelPopup();
		}
		protected override bool IsNeededKey(Keys keyData) {
			return Properties.IsNeededKey(keyData);
		}
		protected override KeyShortcut CloseUpKey { get { return Properties.CloseUpKey; } }
		protected override bool AllowCloseByEscape { get { return Properties.AllowCloseByEscape; } }
		protected override bool AllowPopupTabOut { get { return OwnerEdit.AllowPopupTabOut; } }
		protected override Size PopupFormMinSize { get { return Properties.PopupFormMinSize; } }
		protected override bool CanShowShadow { get { return Properties == null || Properties.ShowPopupShadow; } }
		protected override bool IsImeInProgress { get { return OwnerEdit.MaskBox.IsImeInProgress; } }
		bool ISupportTopMost.IsTopMost {
			get { return true; }
		}
	}
	public interface IPopupSizeableForm {
		Form Form { get; }
		Control OwnerEdit { get; }
		Size MinFormSize { get; }
		Size UpdateMinFormSize(Size minSize);
		int UpdateWidthWhenRightGrip(int width);
		SizeGripPosition GripPosition { get; }
		bool IsGripPoint(Point pt);
		Cursor CalcGripCursor(SizeGripPosition gripPos);
		ResizeMode CurrentPopupResizeMode { get; }
		void DrawSizingRect(bool show);
		SizeGripPosition CurrentSizeGripPosition { get; set; }
		bool IsTopSizeBar { get; set; }
	}
	public class SizablePopupHelper : IDisposable {
		bool formResized, sizing;
		Rectangle sizingBounds;
		IPopupSizeableForm owner;
		Timer reversibleFrameTimer;
		public SizablePopupHelper(IPopupSizeableForm owner) {
			this.owner = owner;
			this.formResized = false;
			this.sizing = false;
			this.sizingBounds = Rectangle.Empty;
			this.reversibleFrameTimer = null;
		}
		public virtual void OnShowPopupForm() {
			Point location = Owner.OwnerEdit.PointToScreen(Point.Empty);
			Owner.IsTopSizeBar = (location.Y > Owner.Form.Location.Y);
			this.formResized = false;
		}
		public virtual void OnHidePopupForm() {
			if(Sizing) CancelSizing();
		}
		public virtual DXMouseEventArgs OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			bool changeCursor = false;
			if(!ee.Handled) {
				if(Sizing) {
					DoSizing(new Point(e.X, e.Y));
					changeCursor = true;
					ee.Handled = true;
				}
				else {
					if(e.Button == MouseButtons.None) {
						if(Owner.IsGripPoint(new Point(e.X, e.Y))) {
							changeCursor = true;
							ee.Handled = true;
						}
					}
				}
				UpdateCursor(changeCursor);
			}
			return ee;
		}
		public virtual DXMouseEventArgs OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			if(!ee.Handled) {
				if(ee.Button == MouseButtons.Left && e.Clicks == 1 && Owner.IsGripPoint(new Point(e.X, e.Y))) {
					ee.Handled = true;
					StartSizing();
				}
			}
			return ee;
		}
		public virtual DXMouseEventArgs OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			if(Sizing) {
				ee.Handled = true;
				EndSizing();
			}
			return ee;
		}
		Point pointOffset = Point.Empty;
		Rectangle newRect = Rectangle.Empty;
		public virtual void DoSizing(Point p) {
			SetFormResized();
			p = PointToScreen(p);
			Size minSize = Owner.MinFormSize, currentSize = Size.Empty;
			minSize = Owner.UpdateMinFormSize(minSize);
			SizeGripPosition gp = Owner.GripPosition;
			if(Owner.Form.RightToLeftLayout)
				gp = InvertSizeGrip(gp);
			if(gp == SizeGripPosition.LeftBottom || gp == SizeGripPosition.RightBottom) {
				currentSize.Height = p.Y - SizingBounds.Top;
			}
			else {
				currentSize.Height = SizingBounds.Bottom - p.Y;
			}
			if(gp == SizeGripPosition.LeftBottom || gp == SizeGripPosition.LeftTop) {
				currentSize.Width = Math.Max(sizingBounds.Right - p.X, sizingBounds.Right - Owner.OwnerEdit.PointToScreen(Point.Empty).X);
			}
			else {
				currentSize.Width = p.X - SizingBounds.X;
				currentSize.Width = Owner.UpdateWidthWhenRightGrip(currentSize.Width);
			}
			currentSize.Width = Math.Max(currentSize.Width, minSize.Width);
			currentSize.Height = Math.Max(currentSize.Height, minSize.Height);
			Rectangle newBounds = SizingBounds;
			newBounds.Size = currentSize;
			if(gp == SizeGripPosition.LeftTop || gp == SizeGripPosition.RightTop) {
				newBounds.Y = SizingBounds.Bottom - newBounds.Height;
			}
			if(gp == SizeGripPosition.LeftBottom || gp == SizeGripPosition.LeftTop) {
				newBounds.X = SizingBounds.Right - newBounds.Width;
			}
			if(SizingBounds != newBounds) {
				newRect = newBounds;
				reversibleFrameTimer.Start();
			}
		}
		private Point PointToScreen(Point p) {
			if(!WindowsFormsSettings.GetIsRightToLeftLayout(Owner.Form)) {
				p = Owner.Form.PointToScreen(p);
				p.Offset(pointOffset);  
				return p;
			}
			p.X = SizingBounds.Width - p.X;
			p.X += SizingBounds.X;
			p.Y += SizingBounds.Y;
			return p;
		}
		private SizeGripPosition InvertSizeGrip(SizeGripPosition gp) {
			return PopupBaseSizeableForm.InvertGripPosition(gp);
		}
		public bool FormResized { get { return formResized; } }
		public virtual Rectangle SizingBounds {
			get { return sizingBounds; }
			set { sizingBounds = value; }
		}
		protected void SetFormResized() { this.formResized = true; }
		protected virtual void StartSizing() {
			this.sizing = true;
			SizingBounds = Owner.Form.Bounds;
			Owner.CurrentSizeGripPosition = Owner.GripPosition;
			ShowSizingRect();
			CreateReversibleTimer();
			UpdateCursor(true);
			CalcPointOffset(); 
		}
		protected virtual void EndSizing() {
			if(!Sizing) return;
			CancelSizing();
			Owner.Form.Bounds = SizingBounds;
		}
		protected virtual void CancelSizing() {
			if(!Sizing) return;
			this.sizing = false;
			HideSizingRect();
			DisposeReversibleTimer();
			UpdateCursor(false);
		}
		protected void CalcPointOffset() {
			Point SizingCornerLocation = GetSizingCornerLocation();
			pointOffset = SizingCornerLocation;
			pointOffset.Offset(-Control.MousePosition.X, -Control.MousePosition.Y);
		}
		protected Point GetSizingCornerLocation() {
			Point SizingCornerLocation = SizingBounds.Location;
			if(Owner.GripPosition == SizeGripPosition.RightBottom || Owner.GripPosition == SizeGripPosition.RightTop)
				SizingCornerLocation.X = SizingCornerLocation.X + SizingBounds.Width;
			if(Owner.GripPosition == SizeGripPosition.RightBottom || Owner.GripPosition == SizeGripPosition.LeftBottom)
				SizingCornerLocation.Y = SizingCornerLocation.Y + SizingBounds.Height;
			return SizingCornerLocation;
		}
		protected void CreateReversibleTimer() {
			if(reversibleFrameTimer != null) return;
			reversibleFrameTimer = new Timer();
			reversibleFrameTimer.Interval = 40;
			if(DevExpress.Utils.Drawing.Helpers.NativeVista.IsVista && Owner.CurrentPopupResizeMode == ResizeMode.FrameResize)
				reversibleFrameTimer.Interval = 70;
			reversibleFrameTimer.Tick += new EventHandler(ReversibleFrameTimer_Tick);
		}
		protected void DisposeReversibleTimer() {
			if(reversibleFrameTimer == null) return;
			reversibleFrameTimer.Tick -= new EventHandler(ReversibleFrameTimer_Tick);
			reversibleFrameTimer.Dispose();
			reversibleFrameTimer = null;
		}
		protected void ReversibleFrameTimer_Tick(object sender, EventArgs e) {
			reversibleFrameTimer.Stop();
			HideSizingRect();
			SizingBounds = newRect;
			ShowSizingRect();
			if(Owner.CurrentPopupResizeMode == ResizeMode.LiveResize) Owner.Form.Bounds = SizingBounds;
		}
		protected virtual void ShowSizingRect() {
			Owner.DrawSizingRect(true);
		}
		protected virtual void HideSizingRect() {
			Owner.DrawSizingRect(false);
		}
		public bool Sizing { get { return sizing; } }
		protected virtual void UpdateCursor(bool setSizing) {
			if(setSizing) {
				Cursor.Current = Owner.CalcGripCursor(Owner.GripPosition);
			}
			else {
				Cursor.Current = Owner.Form.Cursor;
			}
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			this.owner = null;
		}
		public IPopupSizeableForm Owner { get { return owner; } }
	}
	public class PopupBaseSizeableForm : PopupBaseForm, IPopupSizeableForm {
		bool allowSizing;
		SizablePopupHelper sizablePopupHelper;
		public PopupBaseSizeableForm(PopupBaseEdit ownerEdit) : base(ownerEdit) {
			this.allowSizing = true;
			this.sizablePopupHelper = CreateSizablePopupHelper();
		}
		protected virtual SizablePopupHelper CreateSizablePopupHelper() {
			return new SizablePopupHelper(this);
		}
		public virtual bool AllowSizing { 
			get { return allowSizing; }
			set { 
				if(AllowSizing == value) return;
				allowSizing = value;
			}
		}
		public override object ResultValue { get { return null; } }
		protected override Size CalcFormSizeCore() {
			return CalcFormSize(new Size(200, 200));
		}
		protected override Size DefaultMinFormSize { get { return new Size(100, 50); } }
		protected override PopupBaseFormPainter CreatePainter() {
			return new PopupBaseSizeableFormPainter();
		}
		protected override PopupBaseFormViewInfo CreateViewInfo() {
			return new PopupBaseSizeableFormViewInfo(this);
		}
		protected new PopupBaseSizeableFormViewInfo ViewInfo { get { return base.ViewInfo as PopupBaseSizeableFormViewInfo; } }
		public override void HidePopupForm() {
			SizablePopupHelper.OnHidePopupForm();
			base.HidePopupForm();
		}
		public override void ShowPopupForm() {
			SizablePopupHelper.OnShowPopupForm();
			base.ShowPopupForm();
		}
		public bool FormResized { get { return SizablePopupHelper.FormResized; } }
		protected virtual bool Sizing { get { return SizablePopupHelper.Sizing; } }
		#region B132275
		internal bool CurrentSizing { get { return SizablePopupHelper.Sizing; } }
		internal SizeGripPosition CurrentSizeGripPosition = SizeGripPosition.RightBottom;
		#endregion
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = SizablePopupHelper.OnMouseMove(e);
			base.OnMouseMove(ee);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = SizablePopupHelper.OnMouseDown(e);
			base.OnMouseDown(ee);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = SizablePopupHelper.OnMouseUp(e);
			base.OnMouseUp(ee);
		}
		protected internal virtual int UpdateWidthWhenRightGrip(int width) { return width; }
		protected virtual Size UpdateMinFormSize(Size minSize) { return minSize; }
		protected SizablePopupHelper SizablePopupHelper { get { return sizablePopupHelper; } }
		bool frameVisible = false;
		protected virtual void DrawSizingRect(bool show) {
			if(OwnerEdit.CurrentPopupResizeMode == ResizeMode.LiveResize) return;
			if(show == frameVisible) return;
			ControlPaint.DrawReversibleFrame(SizablePopupHelper.SizingBounds, SystemColors.Control, FrameStyle.Thick);
			frameVisible = show;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(SizablePopupHelper != null) {
					SizablePopupHelper.Dispose();
				}
			}
			this.sizablePopupHelper = null;
			base.Dispose(disposing);
		}
		protected virtual Rectangle SizingBounds {
			get { return SizablePopupHelper.SizingBounds; }
			set { SizablePopupHelper.SizingBounds = value; }
		}
		#region IPopupSizeableForm
		int IPopupSizeableForm.UpdateWidthWhenRightGrip(int width) {
			return UpdateWidthWhenRightGrip(width);
		}
		Form IPopupSizeableForm.Form { get { return this; } }
		Control IPopupSizeableForm.OwnerEdit { get { return OwnerEdit; } }
		bool IPopupSizeableForm.IsTopSizeBar {
			get { return ViewInfo.IsTopSizeBar; }
			set { ViewInfo.IsTopSizeBar = value; }
		}
		Size IPopupSizeableForm.MinFormSize { get { return MinFormSize; } }
		Size IPopupSizeableForm.UpdateMinFormSize(Size minSize) {
			return UpdateMinFormSize(minSize);
		}
		bool IPopupSizeableForm.IsGripPoint(Point pt) {
			return ViewInfo.IsGripPoint(pt);
		}
		SizeGripPosition IPopupSizeableForm.CurrentSizeGripPosition {
			get { return CurrentSizeGripPosition; }
			set { CurrentSizeGripPosition = value; }
		}
		SizeGripPosition IPopupSizeableForm.GripPosition {
			get { return ViewInfo.GripObjectInfo.GripPosition; }
		}
		Cursor IPopupSizeableForm.CalcGripCursor(SizeGripPosition gripPos) {
			if(RightToLeftLayout)
				gripPos = InvertGripPosition(gripPos);
			return ViewInfo.GripPainter.CalcGripCursor(gripPos);
		}
		internal static SizeGripPosition InvertGripPosition(SizeGripPosition gripPos) {
			switch(gripPos) { 
				case SizeGripPosition.LeftBottom:
					return SizeGripPosition.RightBottom;
				case SizeGripPosition.RightTop:
					return SizeGripPosition.LeftTop;
				case SizeGripPosition.LeftTop:
					return SizeGripPosition.RightTop;
				case SizeGripPosition.RightBottom:
					return SizeGripPosition.LeftBottom;
			}
			return gripPos;
		}
		void IPopupSizeableForm.DrawSizingRect(bool show) {
			DrawSizingRect(show);
		}
		ResizeMode IPopupSizeableForm.CurrentPopupResizeMode { get { return OwnerEdit.CurrentPopupResizeMode; } }
		#endregion
	}
	public class PopupBaseFormViewInfo {
		GraphicsInfo gInfo;
		bool fIsReady;
		protected BorderPainter fBorderPainter;
		protected Rectangle fBounds, fClientRect, fContentRect;
		SimplePopupBaseForm form;
		AppearanceObject paintAppearance;
		public PopupBaseFormViewInfo(SimplePopupBaseForm form) {
			this.form = form;
			this.paintAppearance = new AppearanceObject();
			this.gInfo = new GraphicsInfo();
			UpdatePainters();
		}
		protected virtual AppearanceDefault DefaultAppearance { 
			get { 
				AppearanceDefault res = new AppearanceDefault(GetSystemColor(SystemColors.ControlText), Form.BackColor);
				if(Form != null && Form.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003) {
					res.BackColor = Office2003Colors.Default[Office2003Color.TabPageClient];
				}
				return res;
			}
		}
		protected internal bool IsRightToLeft { get { return Form != null && Form.OwnerEdit != null && Form.OwnerEdit.IsRightToLeft && !WindowsFormsSettings.GetIsRightToLeftLayout(Form); } }
		public virtual Color GetSystemColor(Color color) {
			return LookAndFeelHelper.GetSystemColor(Form.LookAndFeel, color);
		}
		public virtual AppearanceObject PaintAppearance { get { return paintAppearance; } }
		public virtual AppearanceObject Appearance { get { return Form.AppearanceDropDown; } }
		public virtual SimplePopupBaseForm Form { get { return form; } }
		public virtual bool IsReady { get { return fIsReady; } }
		protected virtual GraphicsInfo GInfo { get { return gInfo; } }
		protected virtual void UpdatePainters() {
			switch(Form.PopupBorderStyle) {
				case PopupBorderStyles.NoBorder: this.fBorderPainter = new EmptyBorderPainter(); break;
				case PopupBorderStyles.Flat : this.fBorderPainter = new HotFlatBorderPainter(); break;
				case PopupBorderStyles.Style3D : this.fBorderPainter = new Border3DRaisedPainter(); break;
				case PopupBorderStyles.Simple : this.fBorderPainter = new SimpleBorderPainter(); break;
				default:
					if(Form.Properties.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
						this.fBorderPainter = new SkinPopupFormBorderPainter(Form.Properties.LookAndFeel.ActiveLookAndFeel) { RightToLeftLayout = WindowsFormsSettings.GetIsRightToLeftLayout(Form) };
					else
						this.fBorderPainter = new HotFlatBorderPainter();
					break;
			}
		}
		protected virtual IStyleController StyleController { get { return Form != null && Form.OwnerEdit != null ? Form.OwnerEdit.StyleController : null; } }
		public virtual void UpdatePaintAppearance() { 
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Appearance, StyleController == null ? null : StyleController.AppearanceDropDown}, DefaultAppearance);
		}
		protected virtual void Clear() {
			this.fIsReady = false;
			this.fBounds = this.fClientRect = this.fContentRect = Rectangle.Empty;
		}
		public virtual void CalcViewInfo(Graphics g, Rectangle bounds) {
			Clear();
			UpdateFromForm();
			UpdatePainters();
			UpdatePaintAppearance();
			GInfo.AddGraphics(g);
			try {
				this.fBounds = bounds;
				CalcRects();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			this.fIsReady = true;
		}
		public virtual Rectangle Bounds { get { return fBounds; } }
		public virtual Rectangle ContentRect { get { return fContentRect; } }
		public virtual Rectangle ClientRect { get { return fClientRect; } }
		protected virtual void UpdateFromForm() {
		}
		protected virtual void CalcRects() {
			CalcClientRect(Bounds);
			CalcContentRect(ClientRect);
		}
		protected virtual void CalcClientRect(Rectangle bounds) {
			this.fClientRect = BorderPainter.GetObjectClientRectangle(new BorderObjectInfoArgs(GInfo.Cache, bounds, null));
		}
		protected virtual void CalcContentRect(Rectangle bounds) {
			this.fContentRect = bounds;
		}
		public virtual Size CalcBorderSize() {
			Size res = Size.Empty;
			GInfo.AddGraphics(null);
			try {
				res = BorderPainter.CalcBoundsByClientRectangle(new BorderObjectInfoArgs(GInfo.Cache, Rectangle.Empty, null)).Size;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		public virtual Size CalcSizeByContentSize(Size contentSize) {
			Size res = contentSize;
			GInfo.AddGraphics(null);
			try {
				UpdatePaintAppearance();
				res = BorderPainter.CalcBoundsByClientRectangle(new BorderObjectInfoArgs(GInfo.Cache, new Rectangle(Point.Empty, contentSize), null)).Size;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		public virtual BorderPainter BorderPainter { get { return fBorderPainter; } }
		public virtual void BeginPaint() { 
		}
		public virtual void EndPaint() {
		}
		public virtual ObjectState State { get { return ObjectState.Selected; } }
		public virtual Point GetSeparatorLineLocation(int height) {
			return new Point(Bounds.Left + 1, ContentRect.Bottom - height);
		}
		public virtual void UpdateEmbeddedControlPosition(Control embeddedControl, int height) {
			if(embeddedControl == null) return;
			embeddedControl.Height -= height;
		}
	}
	public abstract class SimplePopupBaseSizeableFormViewInfo : PopupBaseFormViewInfo {
		Rectangle _sizeBarRect, _sizeGripRect;
		SizeGripObjectPainter gripPainter;
		SizeGripObjectInfoArgs gripObjectInfo;
		bool isTopSizeBar, showSizeBar;
		public SimplePopupBaseSizeableFormViewInfo(SimplePopupBaseForm form) : base(form) {
			this.gripObjectInfo = new SizeGripObjectInfoArgs();
			this.isTopSizeBar = false;
			this.showSizeBar = true;
		}
		protected override void UpdatePainters() {
			base.UpdatePainters();
			this.gripPainter = SizeGripHelper.GetPainter(Form.OwnerEdit.LookAndFeel);
		}
		public virtual bool IsGripPoint(Point p) {
			return ShowSizeGrip && SizeGripRect.Contains(p);
		}
		public new SimplePopupBaseForm Form { get { return base.Form as SimplePopupBaseForm; } }
		public virtual SizeGripObjectPainter GripPainter { get { return gripPainter; } }
		public virtual SizeGripObjectInfoArgs GripObjectInfo { get { return gripObjectInfo; } }
		public virtual bool ShowSizeGrip { get { return AllowSizing; } }
		public virtual bool ShowSizeBar { get { return showSizeBar; } set { showSizeBar = value; } }
		public virtual bool IsTopSizeBar { get { return isTopSizeBar; } set { isTopSizeBar = value; } }
		Screen lastScreen = null;
		Rectangle lastScreenBounds = Rectangle.Empty;
		protected virtual bool IsLeftSizeGrip { 
			get { 
				if(Bounds.IsEmpty || !Form.IsHandleCreated || Form.Location.IsEmpty) return false;
				if(CurrentSizing) return (CurrentSizeGripPosition == SizeGripPosition.LeftBottom || CurrentSizeGripPosition == SizeGripPosition.LeftTop);
				Rectangle bounds = Form.RectangleToScreen(Bounds);
				Screen scr = null;
				if(bounds == lastScreenBounds) scr = lastScreen;
				if(scr == null) {
					lastScreen = scr = Screen.FromPoint(bounds.Location);
					lastScreenBounds = bounds;
				}
				int deltaLeft = bounds.Left - scr.WorkingArea.Left,
					deltaRight = scr.WorkingArea.Right - bounds.Right;
				if(!IsRightToLeftLayout && (scr.WorkingArea.Width > 0 && deltaLeft > scr.WorkingArea.Width / 3)) {
					if(scr.WorkingArea.Right - bounds.Right < 50) return true;
				}
				else if(IsRightToLeftLayout && (scr.WorkingArea.Width > 0 && deltaLeft < scr.WorkingArea.Width / 3)) {
					if(bounds.Left - scr.WorkingArea.Left < 50) return true;
				}
				if(IsRightToLeft) {
					if(scr.WorkingArea.Width > 0 && deltaRight > scr.WorkingArea.Width / 5) {
						return true;
					}
					return false;
				}
				return false;
			} 
		}
		protected override void Clear() {
			base.Clear();
			this._sizeGripRect = this._sizeBarRect = Rectangle.Empty;
		}
		public virtual Rectangle SizeGripRect { get { return _sizeGripRect; } set { _sizeGripRect = value; } }
		public virtual Rectangle SizeBarRect { get { return _sizeBarRect; } set { _sizeBarRect = value; } }
		protected virtual void UpdateSizeGripInfo() {
			gripObjectInfo.SetAppearance(PaintAppearance);
			if(IsTopSizeBar) {
				gripObjectInfo.GripPosition = IsLeftSizeGrip ? SizeGripPosition.LeftTop : SizeGripPosition.RightTop;
			} else {
				gripObjectInfo.GripPosition = IsLeftSizeGrip ? SizeGripPosition.LeftBottom : SizeGripPosition.RightBottom;
			}
			gripObjectInfo.Bounds = SizeGripRect;
		}
		protected virtual Size CalcGripSize() {
			UpdateSizeGripInfo();
			Size size = new Size(10, 10);
			Graphics g = GInfo.AddGraphics(null);
			try {
				GripObjectInfo.Cache = GInfo.Cache;
				size = GripPainter.CalcObjectMinBounds(GripObjectInfo).Size;
				GripObjectInfo.Cache = null;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return size;
		}
		protected virtual int CalcSizeBarHeight(Size gripSize) {
			if(!ShowSizeBar) return 0;
			int height = Math.Max(gripSize.Height, CalcSizeBarContentHeight());
			height += 4;
			return height;
		}
		protected virtual int CalcSizeBarContentHeight() {
			return 0;
		}
		public override Size CalcSizeByContentSize(Size contentSize) {
			Size res = contentSize;
			GInfo.AddGraphics(null);
			try {
				contentSize.Height += CalcSizeBarHeight(CalcGripSize());
				res = base.CalcSizeByContentSize(contentSize);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		protected override void CalcContentRect(Rectangle bounds) {
			this._sizeBarRect = Rectangle.Empty;
			if(ShowSizeBar) {
				Size gripSize = CalcGripSize();
				int height = CalcSizeBarHeight(gripSize);
				this._sizeBarRect = bounds;
				this._sizeBarRect.Height = height;
				if(IsTopSizeBar) {
					bounds.Y += height;
				} else {
					this._sizeBarRect.Y = bounds.Bottom - height;
				}
				bounds.Height -= height;
				if(ShowSizeGrip) {
					this._sizeGripRect = this._sizeBarRect;
					this._sizeGripRect.Size = gripSize;
					if(!IsTopSizeBar) {
						this._sizeGripRect.Y = this._sizeBarRect.Bottom - gripSize.Height;
					}
					if(!IsLeftSizeGrip) {
						this._sizeGripRect.X = this._sizeBarRect.Right - gripSize.Width;
					}
					this._sizeGripRect.Offset(IsLeftSizeGrip ? 1 : -1, IsTopSizeBar ? 1 : -1);
				}
			}
			UpdateSizeGripInfo();
			if(Form != null)
				bounds.Height += Form.SeparatorControlHeight;
			base.CalcContentRect(bounds);
		}
		public override Point GetSeparatorLineLocation(int height) {
			if(isTopSizeBar)
				return new Point(Bounds.Left + 1, ContentRect.Top);
			else
				return base.GetSeparatorLineLocation(height);
		}
		public override void UpdateEmbeddedControlPosition(Control embeddedControl, int height) {
			if(embeddedControl == null) return;
			if(isTopSizeBar)
				embeddedControl.Top += height;
			base.UpdateEmbeddedControlPosition(embeddedControl, height);
		}
		protected abstract bool AllowSizing { get; }
		protected abstract bool CurrentSizing { get; }
		protected abstract SizeGripPosition CurrentSizeGripPosition { get; }
		public bool IsRightToLeftLayout { get { return WindowsFormsSettings.GetIsRightToLeftLayout(Form); } }
	}
	public class PopupBaseSizeableFormViewInfo : SimplePopupBaseSizeableFormViewInfo {
		public PopupBaseSizeableFormViewInfo(PopupBaseForm form)
			: base(form) {
		}
		protected override bool AllowSizing { get { return Form.AllowSizing; } }
		protected override bool CurrentSizing { get { return Form.CurrentSizing; } }
		protected override SizeGripPosition CurrentSizeGripPosition { get { return Form.CurrentSizeGripPosition; } }
		public new PopupBaseSizeableForm Form { get { return base.Form as PopupBaseSizeableForm; } }
	}
	public class PopupBaseFormPainter {
		public virtual void Draw(PopupFormGraphicsInfoArgs info) {
			info.ViewInfo.BeginPaint();
			try {
				DrawBorder(info);
				DrawContent(info);
			}
			finally {
				info.ViewInfo.EndPaint();
			}
		}
		protected virtual void DrawBorder(PopupFormGraphicsInfoArgs info) {
			info.ViewInfo.BorderPainter.DrawObject(new BorderObjectInfoArgs(info.Cache, info.Bounds, info.ViewInfo.Appearance, info.ViewInfo.State));
		}
		protected virtual void DrawContent(PopupFormGraphicsInfoArgs info) {
			PopupBaseFormViewInfo vi = info.ViewInfo;
			Rectangle rect = vi.ClientRect;
			if(WindowsFormsSettings.GetIsRightToLeftLayout(vi.Form)) {
				rect.X--;
			}
			vi.PaintAppearance.DrawBackground(info.Graphics, info.Cache, rect);
		}
	}
	public class PopupBaseSizeableFormPainter : PopupBaseFormPainter {
		protected override void DrawContent(PopupFormGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawSizeBar(info);			
		}
		protected virtual Rectangle GetSizeBarRect(PopupFormGraphicsInfoArgs info) {
			return Rectangle.Empty;
		}
		protected virtual void DrawSizeBar(PopupFormGraphicsInfoArgs info) {
			SimplePopupBaseSizeableFormViewInfo vi = info.ViewInfo as SimplePopupBaseSizeableFormViewInfo;
			if(!vi.ShowSizeBar) return;
			if(GetSizeBarRect(info) != Rectangle.Empty)
				vi.PaintAppearance.DrawBackground(info.Graphics, info.Cache, GetSizeBarRect(info));
			if(vi.ShowSizeGrip) {
				vi.GripObjectInfo.Cache = info.Cache;
				vi.GripPainter.DrawObject(vi.GripObjectInfo);
				vi.GripObjectInfo.Cache = null;
			}
		}
	}
	public class PopupFormGraphicsInfoArgs : GraphicsInfoArgs {
		PopupBaseFormViewInfo viewInfo;
		public PopupFormGraphicsInfoArgs(PopupBaseFormViewInfo viewInfo, GraphicsCache cache, Rectangle bounds) : base(cache, bounds) {
			this.viewInfo = viewInfo;
		}
		public PopupFormGraphicsInfoArgs(PopupBaseFormViewInfo viewInfo, GraphicsInfoArgs info, Rectangle bounds) : base(info, bounds) {
			this.viewInfo = viewInfo;
		}
		public PopupBaseFormViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class PopupAllowClickEventArgs : EventArgs {
		Point mousePosition;
		Control control;
		bool allow;
		public PopupAllowClickEventArgs(Point mousePosition, Control control, bool allow) {
			this.mousePosition = mousePosition;
			this.control = control;
			this.allow = allow;
		}
		public Point MousePosition { get { return mousePosition; } }
		public Control Control { get { return control; } }
		public bool Allow {
			get { return allow; }
			set { allow = value; }
		}
	}
	public delegate void PopupAllowClickEventHandler(object sender, PopupAllowClickEventArgs e);
	public class SkinPopupFormBorderPainter : SkinBorderPainter {
		public SkinPopupFormBorderPainter(ISkinProvider provider) : base(provider) { }
		public bool RightToLeftLayout { get; set; }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinPopupFormBorder]);
			info.CorrectImageFormRTL = RightToLeftLayout;
			return info;
		}
	}
	public delegate void QueryAvailablePopupBoundsEventHandler(object sender, BoundsEventArgs e);
	public class BoundsEventArgs {
		public BoundsEventArgs() { Bounds = Rectangle.Empty; }
		public Rectangle Bounds { get; set; }
	}
}
