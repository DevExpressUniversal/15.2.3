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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraBars.Controls;
using DevExpress.Utils.FormShadow;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Utils;
namespace DevExpress.XtraBars.Ribbon {
	public enum RibbonFormStyle { Ribbon, Standard }
	public enum RibbonVisibility { Auto, Visible, Hidden }
	public class RibbonForm : XtraForm, ISupportGlassRegions, IBarObjectContainer, ISupportFormShadow {
		public static bool UseAdvancedDisplayRectangle = false;
		RibbonControl ribbon;
		RibbonStatusBar statusBar;
		bool ribbonAlwaysAtBack = true;
		public RibbonForm() {
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlConstants.DoubleBuffer, true);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Padding Padding {
			get {
				if(RibbonFormStyle == XtraBars.Ribbon.RibbonFormStyle.Standard)
					return base.Padding;
				return Padding.Empty;
			}
			set { base.Padding = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowMdiChildCaptionInParentTitle {
			get { return true; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string MdiChildCaptionFormatString {
			get {
				if(IsRibbonStyle)
					return "";
				return base.MdiChildCaptionFormatString;
			}
			set {
				base.MdiChildCaptionFormatString = value;
			}
		}
		protected internal new byte LayoutSuspendCountCore { get { return base.LayoutSuspendCountCore; } }
		protected override FormShadow CreateFormShadow() {
			return new RibbonFormShadow();
		}
		protected internal virtual RibbonFormStyle RibbonFormStyle { get { return RibbonFormStyle.Ribbon; } }
		protected bool IsRibbonStyle { get { return RibbonFormStyle == RibbonFormStyle.Ribbon; } }
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(Ribbon != null && Ribbon.ViewInfo.CanProcessFullScreenActions(e)) Ribbon.OnFullScreenModeBarClicked();
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			if(Ribbon != null && Ribbon.ViewInfo.CanProcessFullScreenActions()) Ribbon.OnMouseEnterCore(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(Ribbon != null && Ribbon.ViewInfo.CanProcessFullScreenActions()) Ribbon.OnMouseLeaveCore(e);
		}
		protected internal bool IsGlassForm {
			get { return CheckGlassForm(); }
		}
		protected internal virtual bool IsRightToLeft { 
			get { return WindowsFormsSettings.GetIsRightToLeftLayout(this); } 
		}
		internal bool IsWindowActive {
			get {
				if(RibbonFormStyle == RibbonFormStyle.Ribbon && FormPainter != null)
					return FormPainter.IsWindowActiveCore;
				return Form.ActiveForm == this;
			}
		}
		protected override void OnLayout(LayoutEventArgs levent) {
			base.OnLayout(levent);
			CheckFormLayoutCore();
		}
		protected virtual void CheckFormLayoutCore() {
			if(!ShouldUpdateRibbonZOrder)
				return;
			Ribbon.SendToBack();
			if(IsHandleCreated) BeginInvoke(new MethodInvoker(PerformLayout));
		}
		protected virtual bool ShouldUpdateRibbonZOrder {
			get {
				if(!RibbonAlwaysAtBack) return false;
				if(Ribbon == null || ContainsBarDockControls)
					return false;
				if(Controls.Count < 2)
					return false;
				return !IsRibbonTopControl;
			}
		}
		protected internal bool IsRibbonTopControl {
			get {
				if(Ribbon == null) return false;
				return Controls.IndexOf(Ribbon) == Controls.Count - 1;
			}
		}
		protected bool ContainsBarDockControls {
			get {
				foreach(Control control in Controls) {
					if(control is BarDockControl) return true;
				}
				return false;
			}
		}
		protected override Size CalcPreferredSizeCore(Size size) {
			Size sz = base.CalcPreferredSizeCore(size);
			if(AutoSize && StatusBar != null && StatusBar.Visible)
				sz.Height += StatusBar.Height;
			return sz;
		}
		public override Rectangle DisplayRectangle {
			get {
				Rectangle res = base.DisplayRectangle;
				if(RibbonForm.UseAdvancedDisplayRectangle && WindowState == FormWindowState.Maximized && Ribbon != null && MdiParent == null) {
					res.Y += Ribbon.ViewInfo.Caption.CaptionYOffset;
					res.Height -= Ribbon.ViewInfo.Caption.CaptionYOffset;
				}
				return res;
			}
		}
		protected override void UpdateWindowThemeCore() {
			if(IsGlassForm)
				return;
			base.UpdateWindowThemeCore();
		}
		void RefreshRibbon() {
			if(Ribbon != null)
				Ribbon.Refresh();
			if(StatusBar != null)
				StatusBar.Refresh();
		}
		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);
			RefreshRibbon();
		}
		protected override void OnDeactivate(EventArgs e) {
			base.OnDeactivate(e);
			RefreshRibbon();
		}
		protected internal bool IsShouldUseGlassForm() {
			if(DesignMode) return false;
			if(AllowFormGlass == DefaultBoolean.Default) {
				if(WindowsFormsSettings.AllowRibbonFormGlass == DefaultBoolean.False)
					return false;
			}
			if(AllowFormGlass == DefaultBoolean.False || !NativeVista.IsVista || !NativeVista.IsCompositionEnabled() || MdiParent != null || Parent != null)
				return false;
			if(!NativeVista.IsWindows7 && BarUtilites.GetScreenDPI() > 96.0)
				return false;
			if(Ribbon != null &&
				(Ribbon.RibbonStyle == RibbonControlStyle.Office2013 ||
				Ribbon.RibbonStyle == RibbonControlStyle.TabletOffice ||
				Ribbon.RibbonStyle == RibbonControlStyle.OfficeUniversal))
				return false;
			if(Ribbon != null && !Ribbon.ViewInfo.Caption.AllowGlassCaption)
				return false;
			return true;
		}
		protected override object GetSkinPainterType() {
			if(!IsRibbonStyle) return base.GetSkinPainterType();
			if(FormBorderStyle == FormBorderStyle.None) return typeof(RibbonBorderlessFormPainter);
#if DXWhidbey
			if(IsShouldUseGlassForm()) return typeof(RibbonGlassFormPainter);
#endif
			return typeof(RibbonFormPainter);
		}
		protected override void ForceRefresh() {
			if(FormPainter == null && IsInitializing) return;
			base.ForceRefresh();
		}
		internal void ForceStyleChanged() {
			OnStyleChanged(EventArgs.Empty);
		}
		protected internal bool IsWindowMinimized {
			get {
				RibbonFormPainter p = FormPainter as RibbonFormPainter;
				if(p == null) return WindowState == FormWindowState.Minimized;
				return p.IsRibbonFormMinimized;
			}
		}
		protected override FormPainter CreateFormBorderPainter() {
			if(!IsRibbonStyle) return base.CreateFormBorderPainter();
			if(FormBorderStyle == FormBorderStyle.None) return new RibbonBorderlessFormPainter(this, LookAndFeel);
			if(IsShouldUseGlassForm())
				return new RibbonGlassFormPainter(this, LookAndFeel);
			return new RibbonFormPainter(this, LookAndFeel);
		}
		protected internal new RibbonFormPainter FormPainter { get { return base.FormPainter as RibbonFormPainter; } }
		protected override bool GetAllowSkin() {
			if(!IsRibbonStyle)
				return base.GetAllowSkin();
			return true;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonFormRibbonAlwaysAtBack"),
#endif
 DefaultValue(true)]
		public bool RibbonAlwaysAtBack {
			get { return ribbonAlwaysAtBack; }
			set {
				if(RibbonAlwaysAtBack == value) return;
				ribbonAlwaysAtBack = value;
				OnRibbonAlwaysAtBackChanged();
			}
		}
		protected virtual void OnRibbonAlwaysAtBackChanged() {
			if(IsDesignMode && RibbonAlwaysAtBack) {
				CheckFormLayoutCore();
			}
		}
		RibbonVisibility ribbonVisibility = RibbonVisibility.Auto;
		[DefaultValue(RibbonVisibility.Auto)]
		public RibbonVisibility RibbonVisibility {
			get { return ribbonVisibility; }
			set {
				if(RibbonVisibility == value)
					return;
				ribbonVisibility = value;
				if(Ribbon != null)
					Ribbon.CheckFit();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonFormAllowDisplayRibbon"),
#endif
 DefaultValue(true),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is obsolete. Use the RibbonVisibility property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowDisplayRibbon {
			get { return RibbonVisibility == RibbonVisibility.Visible || RibbonVisibility == RibbonVisibility.Auto; }
			set {
				if(value && RibbonVisibility != RibbonVisibility.Auto) {
					RibbonVisibility = RibbonVisibility.Visible;
				}
				else if(!value) {
					RibbonVisibility = RibbonVisibility.Hidden;
				}
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonFormAutoHideRibbon"),
#endif
 DefaultValue(true),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is obsolete. Use the RibbonVisibility property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AutoHideRibbon {
			get { return RibbonVisibility == RibbonVisibility.Auto; }
			set {
				if(value == false && RibbonVisibility == RibbonVisibility.Auto)
					RibbonVisibility = RibbonVisibility.Visible;
			}
		}
		protected virtual bool ShouldDrawBackgroundImage {
			get {
				if(BackgroundImage == null) return false;
				return BackgroundImage != GetSkinBackgroundImage();
			}
		}
		DefaultBoolean allowFormGlass = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonFormAllowFormGlass"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowFormGlass {
			get { return allowFormGlass; }
			set {
				if(AllowFormGlass == value) return;
				allowFormGlass = value;
				UpdateFormMargins();
				CheckUpdateSkinPainter();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowFormSkin {
			get {
				if(!IsRibbonStyle)
					return base.AllowFormSkin;
				return true;
			}
			set {
				if(!IsRibbonStyle)
					base.AllowFormSkin = value;
			}
		}
		DefaultBoolean allowDraggingByPageCategory = DefaultBoolean.Default;
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonFormAllowDraggingByPageCategory"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowDraggingByPageCategory {
			get { return allowDraggingByPageCategory; }
			set {
				if(AllowDraggingByPageCategory == value) return;
				allowDraggingByPageCategory = value;
			}
		}
		protected internal virtual bool ShouldDraggingByPageCategory() {
			if(DesignMode) return false;
			if(AllowDraggingByPageCategory == DefaultBoolean.Default) {
				if(NativeVista.IsWindows8) return true;
				return false;
			}
			return AllowDraggingByPageCategory == DefaultBoolean.True;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new UserLookAndFeel LookAndFeel { get { return base.LookAndFeel; } }
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(!IsRibbonStyle) return;
			UpdateSkinPainter(true);
			if(IsGlassForm) {
				UpdateFormMargins();
			}
			UpdateRegion();
		}
		public override bool RightToLeftLayout {
			get {
				return base.RightToLeftLayout;
			}
			set {
				base.RightToLeftLayout = value;
				if(FormPainter != null)
					FormPainter.UpdateFormRegionCore();
			}
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams res = base.CreateParams;
				if(WindowsFormsSettings.GetIsRightToLeftLayout(this) && IsGlassForm) { 
					const int WS_EX_LAYOUTRTL = 0x00400000;
					const int WS_EX_NOINHERITLAYOUT = 0x00100000;
					res.ExStyle |= WS_EX_LAYOUTRTL | WS_EX_NOINHERITLAYOUT;
				}
				return res;
			}
		}
		protected internal void CheckUpdateSkinPainterCore() {
			UpdateFormMargins();
			CheckUpdateSkinPainter();
		}
		internal int GlassCaptionHeight {
			get { return Ribbon == null ? 0 : Ribbon.ViewInfo.Caption.Bounds.Height; }
		}
		protected override void OnSkinPainterChanged() {
			if(needActivateForm == null) needActivateForm = true;
			base.OnSkinPainterChanged();
			if(Ribbon == null) return;
			Ribbon.ViewInfo.SetAppearanceDirty();
			ResetGlassMargins();
			Ribbon.OnSkinPainterChanged();
			if(IsGlassForm) UpdateFormMargins();
			UpdateRegion();
		}
		Size formSize;
		bool inCreateHandle;
		protected override void CreateHandle() {
			formSize = Size;
			inCreateHandle = IsGlassForm;
			base.CreateHandle();
			inCreateHandle = false;
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(inCreateHandle) {
				width = formSize.Width;
				height = formSize.Height;
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}
		protected internal int CalcGlassFormTopMargin() {
			if(FormBorderStyle == FormBorderStyle.None || Ribbon == null)
				return 0;
			Ribbon.CheckViewInfo();
			int res = Ribbon.ViewInfo.Header.AllowGlassHeader ? Ribbon.ViewInfo.Header.Bounds.Height : 0;
			return res + Ribbon.ViewInfo.Caption.Bounds.Height;
		}
		internal NativeMethods.Margins GetGlassMargins() {
			NativeMethods.Margins mrg = new NativeMethods.Margins();
			mrg.Top = IsShouldUseGlassForm() ? CalcGlassFormTopMargin() : 0;
			return mrg;
		}
		internal virtual void UpdateFormMargins() {
			if(!IsGlassForm) return;
			if(Ribbon == null || Ribbon.ViewInfo.Form == null || !IsHandleCreated) return;
			NativeMethods.Margins mrg = GetGlassMargins();
			NativeVista.DwmExtendFrameIntoClientArea(Handle, ref mrg);
		}
		internal virtual void ResetGlassMargins() {
			if(Ribbon == null || (Ribbon.ViewInfo.Caption.AllowGlassCaption && IsGlassForm))
				return;
			NativeMethods.Margins mrg = new NativeMethods.Margins();
			NativeVista.DwmExtendFrameIntoClientArea(Handle, ref mrg);
		}
		void ExcludeGlass(PaintEventArgs e) {
			int height = GlassCaptionHeight;
			if(height == 0) height = Ribbon == null ? 0 : Ribbon.ViewInfo.Caption.CalcCaptionHeight();
			if(height == 0) return;
			e.Graphics.SetClip(new Rectangle(0, 0, Width, height), System.Drawing.Drawing2D.CombineMode.Exclude);
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(!IsRibbonStyle) {
				base.OnPaint(e);
				return;
			}
#if DXWhidbey
			e.Graphics.Clear(Color.Transparent);
			if(IsGlassForm) ExcludeGlass(e);
#endif
			DrawBackground(e);
			RaisePaintEvent(this, e);
		}
		void DrawBackground(PaintEventArgs e) {
			if(FormPainter == null) return;
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(FormPainter.Provider)[CommonSkins.SkinForm], GetClientRect());
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				CorrectClipBoundsByRightToLeft(cache);
				info.Bounds = new Rectangle(info.Bounds.X - 1, info.Bounds.Y, info.Bounds.Width, info.Bounds.Height);
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			}
			if(ShouldDrawBackgroundImage) {
				DrawBackgroundImage(e);
			}
		}
		private void CorrectClipBoundsByRightToLeft(GraphicsCache cache) {
			if(!RightToLeftLayout)
				return;
			RectangleF rf = cache.Graphics.VisibleClipBounds;
			Rectangle rect = new Rectangle((int)rf.X, (int)rf.Y, (int)rf.Width, (int)rf.Height);
			if(rect.X == 0) {
				rect.Width++;
				rect.X--;
				cache.ClipInfo.RequireAPIClipping = true;
				cache.ClipInfo.SetClip(rect);
				cache.Graphics.SetClip(rect);
			}
		}
		protected virtual Rectangle GetClientRect() {
			if(FormPainter == null || !IsHandleCreated)
				return ClientRectangle;
			NativeMethods.RECT rect = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(Handle, ref rect);
			rect.Bottom -= rect.Top;
			rect.Right -= rect.Left;
			rect.Top = IsGlassForm ? CalcCaptionHeight() : 0;
			rect.Left = 0;
			return new Rectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}
		protected internal virtual int CalcCaptionHeight() {
			if(FormPainter == null || Ribbon == null || (!Ribbon.Visible && IsMdiChild) || !Ribbon.ViewInfo.IsAllowCaption)
				return 0;
			return Ribbon.ViewInfo.Caption.CalcCaptionHeight();
		}
		protected virtual void DrawBackgroundImage(PaintEventArgs e) {
			if(BackgroundImageLayout == ImageLayout.Tile || BackgroundImageLayout == ImageLayout.Stretch) {
				base.OnPaintBackground(e);
				return;
			}
			MethodInfo mi = typeof(ControlPaint).GetMethod("CalculateBackgroundImageRectangle", BindingFlags.NonPublic | BindingFlags.Static);
			Rectangle rect = (Rectangle)mi.Invoke(null, new object[] { e.ClipRectangle, BackgroundImage, BackgroundImageLayout });
			e.Graphics.DrawImage(BackgroundImage, rect);
		}
		protected override void SetVisibleCore(bool value) {
			if(FormPainter == null && value)
				CheckUpdateSkinPainter();
			base.SetVisibleCore(value);
			if(value && Ribbon != null && !Ribbon.IsHandleCreated && !Ribbon.IsDisposed) {
				IntPtr ignore = Ribbon.Handle;
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			if(!IsRibbonStyle) {
				base.OnPaintBackground(e);
			}
			else {
				if(IsGlassForm) ExcludeGlass(e);
				DrawBackground(e);
			}
		}
		bool prevMaximized = false;
		internal Size PreviousSize { get; set; }
		bool lockSizingCore;
		protected internal void LockSizingCore() {
			this.lockSizingCore = true;
		}
		protected internal void UnlockSizingCore() {
			this.lockSizingCore = false;
		}
		protected internal bool LockSizing { get { return lockSizingCore; } }
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(LockSizing) return;
			PreviousSize = Size;
			if(!IsRibbonStyle) return;
			if(prevMaximized != (WindowState == FormWindowState.Maximized)) {
				prevMaximized = WindowState == FormWindowState.Maximized;
				if(Ribbon != null) {
					Ribbon.CheckHeight();
				}
			}
			if(Ribbon != null && !Disposing && !IsDisposed) Ribbon.CheckFit();
			CalcFormBounds();
		}
		protected override bool IsRibbonForm { get { return true; } }
		protected override void OnStyleChanged(EventArgs e) {
			base.OnStyleChanged(e);
			if(IsRibbonStyle && IsGlassForm) Size = SizeFromClientSize(ClientSize);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(IsMdiChild)
				UpdateRegion();
		}
		protected Skin GetSkin() {
			return RibbonSkins.GetSkin(base.FormPainter.Provider);
		}
		protected int DefaultFormCornerRadius {
			get {
				int res = 9;
				if(WindowsFormsSettings.AllowDpiScale)
					return (int)(res * DpiProvider.Default.DpiScaleFactor);
				return res;
			}
		}
		protected internal int GetCornerRadius() {
			if(base.FormPainter == null) return 0;
			object res = GetSkin().Properties[RibbonSkins.OptRibbonFormCornerRadius];
			return (res == null) ? DefaultFormCornerRadius : (int)res;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			Focus();
		}
		protected internal int GetRegionDelta() {
			return FormPainter.GetRegionDelta();
		}
		protected virtual void UpdateRegion() {
			if(!IsHandleCreated) return;
			if(IsGlassForm || RibbonFormStyle == RibbonFormStyle.Standard ||
				NativeMethods.IsZoomed(Handle) || FormBorderStyle == FormBorderStyle.None) {
				Region = null;
				return;
			}
			Rectangle region = new Rectangle(Point.Empty, Size);
			if(RightToLeftLayout) {
				region.X -= GetRegionDelta();
				region.Width -= GetRegionDelta();
			}
			Region = NativeMethods.CreateRoundRegion(region, GetCornerRadius());
		}
		[Browsable(false), DefaultValue(null)]
		public virtual RibbonControl Ribbon {
			get { return ribbon; }
			set {
				if(Ribbon == value) return;
				ribbon = value;
				OnRibbonChanged();
			}
		}
		protected virtual void OnRibbonChanged() {
			if(Ribbon != null) {
				LookAndFeel.ParentLookAndFeel = Ribbon.GetController().LookAndFeel;
				ribbon.OnFormOriginalTextChanged(Text);
			}
			else
				LookAndFeel.ParentLookAndFeel = null;
			CheckUpdateSkinPainter();
		}
		int MinHeight {
			get {
				int w = 0;
				if(StatusBar != null) w += StatusBar.Height;
				if(Ribbon != null) w += Ribbon.ViewInfo.Caption.Bounds.Height;
				return w;
			}
		}
		int MinWidth { get { return 175; } }
		bool InStatusBarSetting { get; set; }
		[Browsable(false), DefaultValue(null)]
		public virtual RibbonStatusBar StatusBar {
			get { return statusBar; }
			set {
				if(StatusBar == value)
					return;
				OnStatusBarChanging();
				statusBar = value;
				OnStatusBarChanged();
			}
		}
		protected virtual void OnStatusBarChanging() {
			UnsubscribeStatusBarEvents();
		}
		protected override bool AllowSkinForEmptyText {
			get { return true; }
		}
		protected virtual void SubscribeStatusBarEvents() {
			if(StatusBar != null)
				StatusBar.VisibleChanged += new EventHandler(OnStatusBarVisibleChanged);
		}
		protected virtual void UnsubscribeStatusBarEvents() {
			if(StatusBar != null)
				StatusBar.VisibleChanged -= new EventHandler(OnStatusBarVisibleChanged);
		}
		void OnStatusBarVisibleChanged(object sender, EventArgs e) {
			if(FormPainter != null)
				FormPainter.SetDirty();
			if(WindowState == FormWindowState.Normal)
				ClientSize = ClientSizeFromSize(Size);
			Refresh();
		}
		protected virtual void OnStatusBarChanged() {
			SubscribeStatusBarEvents();
			UpdateByStatusBar();
		}
		private void UpdateByStatusBar() {
			if(WindowState == FormWindowState.Maximized)
				return;
			if(FormPainter != null)
				FormPainter.SetDirty();
			Size prevClientSize = ClientSize;
			Size = SizeFromClientSize(ClientSize);
			if(prevClientSize != ClientSize) {
				ClientSize = ClientSizeFromSize(Size);
			}
		}
		bool IsDesignMode { get { return Site != null && Site.DesignMode; } }
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			if(!IsRibbonStyle) return;
			RibbonControl rc = e.Control as RibbonControl;
			if(rc != null) {
				if(Ribbon == null) {
					if(IsDesignMode)
						TypeDescriptor.GetProperties(this)["Ribbon"].SetValue(this, e.Control);
					else
						Ribbon = rc;
				}
			}
			RibbonStatusBar sb = e.Control as RibbonStatusBar;
			if(sb != null) {
				if(IsDesignMode) {
					if(StatusBar == null)
						TypeDescriptor.GetProperties(this)["StatusBar"].SetValue(this, e.Control);
				}
				else {
					if(!InStatusBarSetting)
						StatusBar = sb;
				}
			}
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved(e);
			if(!IsRibbonStyle) return;
			if(e.Control == Ribbon) {
				if(IsDesignMode)
					TypeDescriptor.GetProperties(this)["Ribbon"].SetValue(this, null);
				else
					Ribbon = null;
			}
			if(e.Control == StatusBar) {
				if(IsDesignMode)
					TypeDescriptor.GetProperties(this)["StatusBar"].SetValue(this, null);
				else if(!InStatusBarSetting)
					StatusBar = null;
			}
		}
		protected internal virtual void OnFullScreenButtonClicked(MouseEventArgs e) {
			if(Ribbon != null) Ribbon.OnFullScreenButtonClicked(e);
		}
		[RefreshProperties(RefreshProperties.All)]
		public override string Text {
			get { return base.Text; }
			set {
				if(Ribbon != null) Ribbon.OnFormOriginalTextChanged(value);
				base.Text = value;
			}
		}
		bool ShouldSerializeText() { return string.IsNullOrEmpty(HtmlText) && !string.IsNullOrEmpty(Text); }
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			if(Ribbon != null) Ribbon.OnFormTextChanged();
			if(WindowState == FormWindowState.Normal)
				Size = SizeFromClientSize(ClientSize);
		}
		protected override void WndProc(ref Message msg) {
			const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
			if(msg.Msg == WM_DWMCOMPOSITIONCHANGED) {
				OnSkinPainterChanged();
			}
			base.WndProc(ref msg);
		}
		protected override int ClientBackgroundTop {
			get {
				return IsGlassForm ? CalcCaptionHeight() : 0;
			}
		}
		protected override bool ShouldDrawClientBackground(int msg) {
			if(IsShouldUseGlassForm() || FormBorderStyle == FormBorderStyle.None) return false;
			return base.ShouldDrawClientBackground(msg);
		}
		private const Int32 MF_BYPOSITION = 0x400;
		private const Int32 MF_REMOVE = 0x1000;
		internal void DestroyMenu() {
			IntPtr menu = BarNativeMethods.GetMenu(Handle);
			int itemCount = BarNativeMethods.GetMenuItemCount(menu);
			for(int i = 0; i < itemCount; i++) {
				bool res = BarNativeMethods.RemoveMenu(menu, itemCount - 1 - i, MF_BYPOSITION | MF_REMOVE);
			}
			RestoreMdiChildCaptionButtons();
		}
		void RestoreMdiChildCaptionButtons() {
			RestoreMdiChildCaptionButtons(ActiveMdiChild);
		}
		internal void RestoreMdiChildCaptionButtons(Form frm) {
			if(frm == null)
				return;
			MethodInfo mi = typeof(Form).GetMethod("UpdateStylesCore", BindingFlags.NonPublic | BindingFlags.Instance);
			mi.Invoke(frm, null);
		}
		#region ISupportGlassRegions Members
		bool ISupportGlassRegions.IsOnGlass(Rectangle rect) {
			if(!IsGlassForm)
				return false;
			return (new Rectangle(0, 0, ClientSize.Width, CalcGlassFormTopMargin())).IntersectsWith(rect);
		}
		#endregion
		#region IBarObjectContainer Members
		bool IBarObjectContainer.ContainsObject {
			get { return GetBarObjectCore() != null; }
		}
		IBarObject IBarObjectContainer.BarObject {
			get { return GetBarObjectCore(); }
		}
		#endregion
		protected virtual IBarObject GetBarObjectCore() {
			if(Ribbon == null || Ribbon.ViewInfo == null)
				return null;
			if(!Ribbon.ViewInfo.IsPopupFullScreenModeActive)
				return null;
			if(Ribbon.MinimizedRibbonPopupForm == null)
				return null;
			return Ribbon.MinimizedRibbonPopupForm.Control;
		}
		protected internal virtual void OnControllerChanged() {
			DevExpress.Skins.XtraForm.FormClientPainter.InvalidateNC(this);
			CheckUpdateSkinPainter();
		}
		bool ISupportFormShadow.IsActive(IntPtr handle) {
			if(XtraForm.SuppressDeactivation)
				return true;
			return false;
		}
		bool ISupportFormShadow.IsChildForm(IntPtr handle) {
			Control control = Control.FromHandle(handle);
			if(control == null)
				return false;
			DevExpress.XtraBars.Ribbon.RibbonOffice2013BackstageViewContainerControl.ContentPopupForm backStageForm = control as DevExpress.XtraBars.Ribbon.RibbonOffice2013BackstageViewContainerControl.ContentPopupForm;
			if(backStageForm != null)
				return true;
			return false;
		}
		protected override bool CheckGlassForm() { return IsShouldUseGlassForm() && IsRibbonStyle; }
		protected override bool CheckCustomDrawNonClientArea() { return true; }
		bool? needActivateForm = null;
		protected override bool NeedActivateForm {
			get {
				return needActivateForm == true;
			}
		}
		protected override void ForceActivateWindow(Message msg) {
			needActivateForm = false;
			base.ForceActivateWindow(msg);
		}
		protected internal Size CalcMinimizedClientSize() {
			if(FormPainter == null) return Size.Empty;
			int width = Math.Max(0, Width - FormPainter.Margins.Width);
			return new Size(width, Height);
		}
	}
}
namespace DevExpress.Skins.XtraForm {
	public class RibbonGlassFormPainter : RibbonFormPainter {
		public RibbonGlassFormPainter(RibbonForm form, ISkinProvider provider) : base(form, provider) { }
		protected override void DrawFrameCore(GraphicsCache cache, SkinElementInfo info, FrameKind kind) { }
		protected override void RedrawButtons() { }
		protected internal override Rectangle CalcButtons(Rectangle contentBounds) {
			Buttons.Clear();
			contentBounds.Width -= SystemInformation.CaptionButtonSize.Width * 3;
			return contentBounds;
		}
		protected override bool WMSize(ref Message msg) {
			return false;
		}
		protected override void NCCalcSize(ref Message m) {
			if(m.WParam == new IntPtr(1)) {
				if(IsZoomedBorder) {
					ResetZoomedMargins();
				}
				NativeMethods.NCCALCSIZE_PARAMS csp = NativeMethods.NCCALCSIZE_PARAMS.GetFrom(m.LParam);
				NativeMethods.RECT winRect = csp.rgrc0;
				DoBaseWndProc(ref m);
				csp = NativeMethods.NCCALCSIZE_PARAMS.GetFrom(m.LParam);
				csp.rgrc0.Top = winRect.Top + Margins.Top;
				if(IsZoomed) Taskbar.CorrectRECTByAutoHide(ref csp.rgrc0);
				ForceUpdateMdiChild();
				BarNativeMethods.StructureToPtr(csp, m.LParam, false);
				m.Result = IntPtr.Zero;
			}
			else
				DoBaseWndProc(ref m);
		}
		public override bool DoWndProc(ref Message msg) {
			switch(msg.Msg) {
				case MSG.WM_NCCALCSIZE:
					NCCalcSize(ref msg);
					return true;
				case MSG.WM_NCHITTEST:
					NCHitTest(ref msg);
					return true;
				case MSG.WM_NCMOUSELEAVE:
					NativeVista.CallDwmBase(ref msg);
					return false;
			}
			return base.DoWndProc(ref msg);
		}
		int GetInt(IntPtr ptr) {
			return (IntPtr.Size == 8) ? (int)(ptr.ToInt64() & 0xFFFFFFFFL) : ptr.ToInt32();
		}
		bool IsCaptionButtonHitTest(ref Message msg) {
			int res = GetInt(msg.Result);
			return res == NativeMethods.HT.HTCLOSE || res == NativeMethods.HT.HTMAXBUTTON || res == NativeMethods.HT.HTMINBUTTON;
		}
		protected virtual void NCHitTest(ref Message msg) {
			NativeVista.CallDwmBase(ref msg);
			if(GetInt(msg.Result) == NativeMethods.HT.HTNOWHERE)
				DoBaseWndProc(ref msg);
			Point p = PointToFormBoundsCore(new Point(GetInt(msg.LParam)));
			if(IsCaptionButtonHitTest(ref msg)) {
				if(!NativeVista.IsWindows8 && NativeVista.IsWindows7) {
					int buttonBottom = SystemInformation.CaptionButtonSize.Height;
					if(RibbonForm.WindowState == FormWindowState.Maximized && RibbonForm.Bounds.Top < 0)
						buttonBottom -= RibbonForm.Bounds.Top;
					if(p.Y >= buttonBottom) {
						msg.Result = new IntPtr(NativeMethods.HT.HTCAPTION);
					}
				}
			}
			else if(CheckOffice2010HeaderPanel(ref msg))
				return;
			switch(GetInt(msg.Result)) {
				case NativeMethods.HT.HTCAPTION:
				case NativeMethods.HT.HTSYSMENU:
					if(!IsAllowDisplayRibbon)
						return;
					break;
				case NativeMethods.HT.HTCLIENT:
					break;
				default: return;
			}
			msg.Result = new IntPtr(WMNCHitTestResize(p, GetInt(msg.Result)));
			if(GetInt(msg.Result) == NativeMethods.HT.HTCLIENT) {
				p = PointToFormBounds(msg.LParam);
				if(ViewInfo == null) return;
				Rectangle rect = ViewInfo.Caption.ClientBounds;
				rect.Y = ViewInfo.Caption.Bounds.Y;
				rect.Height = ViewInfo.Caption.ClientBounds.Bottom - rect.Y;
				if(rect.Contains(p)) {
					if(IconBounds.Contains(p))
						msg.Result = new IntPtr(NativeMethods.HT.HTMENU);
					else
						msg.Result = new IntPtr(NativeMethods.HT.HTCAPTION);
				}
			}
		}
		public override SkinPaddingEdges Margins {
			get {
				Size borderSize = Size.Empty;
				if(RibbonForm.FormBorderStyle == FormBorderStyle.None)
					return new SkinPaddingEdges();
				if(NativeVista.IsWindows7 && RibbonForm.IsHandleCreated) {
					CreateParams pars = new CreateParams();
					FillInCreateParamsBorderStyles(pars, RibbonForm.FormBorderStyle);
					NativeMethods.RECT wr = new NativeMethods.RECT(Rectangle.Empty);
					NativeMethods.AdjustWindowRectEx(ref wr, pars.Style, false, pars.ExStyle);
					int borderWidth = wr.Right;
					if(IsZoomedBorder && IsZoomedMarginsReady) {
						borderWidth -= GetZoomedMargins().Width;
					}
					return new SkinPaddingEdges() { Top = 0, Left = borderWidth, Right = borderWidth, Bottom = borderWidth };
				}
				if(RibbonForm.FormBorderStyle == FormBorderStyle.FixedDialog)
					borderSize = SystemInformation.FixedFrameBorderSize;
				else if(RibbonForm.FormBorderStyle == FormBorderStyle.Fixed3D)
					borderSize = SystemInformation.FixedFrameBorderSize + SystemInformation.Border3DSize;
				else if(RibbonForm.FormBorderStyle == FormBorderStyle.SizableToolWindow)
					borderSize = SystemInformation.FrameBorderSize;
				else if(RibbonForm.FormBorderStyle == FormBorderStyle.FixedToolWindow)
					borderSize = SystemInformation.FixedFrameBorderSize;
				else if(RibbonForm.FormBorderStyle == FormBorderStyle.Sizable)
					borderSize = SystemInformation.FrameBorderSize;
				else if(RibbonForm.FormBorderStyle == FormBorderStyle.FixedSingle)
					borderSize = SystemInformation.FixedFrameBorderSize;
				SkinPaddingEdges edges = new SkinPaddingEdges();
				edges.Left = borderSize.Width;
				edges.Right = borderSize.Width;
				edges.Bottom = borderSize.Height;
				return edges;
			}
		}
		protected override bool IsAllowNCDraw { get { return false; } }
	}
	public class RibbonFormPainter : FormPainter {
		public RibbonFormPainter(RibbonForm form, ISkinProvider provider) : base(form, provider) { }
		protected internal bool IsRibbonFormMinimized { get { return IsWindowMinimized; } }
		protected RibbonForm RibbonForm { get { return (RibbonForm)Owner; } }
		protected RibbonControl Ribbon { get { return RibbonForm == null ? null : RibbonForm.Ribbon; } }
		protected RibbonViewInfo ViewInfo {
			get {
				if(RibbonForm == null) return null;
				return RibbonForm.Ribbon == null ? null : RibbonForm.Ribbon.ViewInfo;
			}
		}
		protected override bool GetCorrectButtonImagesInRTLMode() {
			return false;
		}
		internal void UpdateFormRegionCore() { 
			if(ShouldUpdateFormRegion && !RibbonForm.IsGlassForm)
				UpdateFormRegion(); 
		}
		public bool AllowHtmlText { get { return AllowHtmlDraw; } }
		internal Rectangle GetIconBounds() { return IconBounds; }
		protected bool IsAllowDisplayRibbon { get { return RibbonForm.Ribbon != null && RibbonForm.Ribbon.ViewInfo.IsAllowDisplayRibbon; } }
		protected Point PointToFormBoundsCore(Point pt) { return PointToFormBounds(pt); }
		protected override bool ShouldShowMdiBar { get { return false; } }
		protected override Rectangle FrameLeft { get { return new Rectangle(0, Margins.Top, Margins.Left, FormBounds.Height - Margins.Top); } }
		protected override Rectangle FrameRight { get { return new Rectangle(FormBounds.Width - Margins.Right, Margins.Top, Margins.Right, FormBounds.Height - Margins.Top); } }
		protected override Rectangle FrameBottom { get { return new Rectangle(FrameLeft.Width, FormBounds.Height - Margins.Bottom, FormBounds.Width - FrameLeft.Width - FrameRight.Width, Margins.Bottom); } }
		protected internal Rectangle IconBoundsCore { get { return IconBounds; } }
		protected override Point PointToFormBounds(Point pt) {
			Point res = base.PointToFormBounds(pt);
			if(RibbonForm.WindowState == FormWindowState.Maximized && RibbonForm.Ribbon != null) {
				return RibbonForm.Ribbon.PointToClient(pt);
			}
			if(RibbonForm.WindowState != FormWindowState.Minimized && RibbonForm.Ribbon != null && Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007 && RibbonForm.Ribbon.ViewInfo.Caption.Bounds.Contains(res)) {
				return RibbonForm.Ribbon.PointToClient(pt);
			}
			if(Ribbon != null) {
				Point res2 = Ribbon.PointToClient(pt);
				if(!RibbonForm.IsGlassForm && Ribbon.ViewInfo.Caption.Bounds.Contains(res2))
					return Ribbon.PointToClient(pt);
			}
			return res;
		}
		protected override SkinElement GetSkinCaption() {
			if(IsWindowMinimized) return RibbonSkins.GetSkin(RibbonForm.Ribbon.ViewInfo.Provider)[RibbonSkins.SkinFormCaptionMinimized];
			if(ViewInfo != null && (!ViewInfo.IsAllowDisplayRibbon || ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice)) {
				SkinElement elem = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinFormCaptionNoRibbon];
				if(elem != null)
					return elem;
			}
			return RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinFormCaption];
		}
		protected override Region GetDefaultFormRegion(ref Rectangle rect) {
			if(RibbonForm == null) return null;
			if(!RibbonForm.IsMdiChild && RibbonForm.WindowState == FormWindowState.Minimized) return null;
			if(RibbonForm.FormBorderStyle == FormBorderStyle.None) return null;
			rect = new Rectangle(Point.Empty, FormBounds.Size);
			if(RibbonForm.RightToLeftLayout) {
				rect.X -= GetRegionDelta();
				rect.Width -= GetRegionDelta();
			}
			return NativeMethods.CreateRoundRegion(rect, RibbonForm.GetCornerRadius());
		}
		RibbonStatusBarViewInfo StatusBarViewInfo { get { return RibbonForm.StatusBar == null ? null : RibbonForm.StatusBar.ViewInfo; } }
		public override bool IsToolWindow { get { return false; } }
		protected override bool IsAllowButtonMessages {
			get {
				if(IsWindowMinimized) return true;
				RibbonControl ribbon = RibbonForm.Ribbon;
				if(ribbon == null)
					return false;
				if(!RibbonForm.IsShouldUseGlassForm()) {
					if(ribbon.RibbonStyle != RibbonControlStyle.Office2013 || !ribbon.IsFullScreenModeActiveCore)
						return true;
				}
				return !ribbon.Enabled;
			}
		}
		protected override bool IsDrawCaption {
			get {
				if(!RibbonForm.IsHandleCreated)
					return false;
				if(!IsOwnerWindowTopMost) {
					if(!RibbonForm.Visible) return false;
					if(BarNativeMethods.IsIconic(RibbonForm.Handle) && !RibbonForm.ShowInTaskbar) return true;
					if(RibbonForm.Parent == null) return false;
				}
				return BarNativeMethods.IsIconic(RibbonForm.Handle);
			}
		}
		protected internal bool IsDrawCaptionCore { get { return IsDrawCaption; } }
		protected override bool IsDrawFrameTop { get { return false; } }
		protected virtual bool IsOwnerWindowTopMost {
			get {
				Form frm = RibbonForm;
				while(frm.Owner != null) {
					if(frm.Owner.TopMost)
						return true;
					frm = frm.Owner;
				}
				return false;
			}
		}
		protected Skin RibbonSkin { get { return RibbonSkins.GetSkin(Provider); } }
		protected override SkinElement GetSkinFrameLeft() {
			if(ViewInfo == null)
				return RibbonSkin[RibbonSkins.SkinFormFrameLeft];
			if(!ViewInfo.IsAllowDisplayRibbon || !ViewInfo.GetShowPageHeaders() ||
				ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice || ViewInfo.IsOfficeTablet) {
				SkinElement elem = RibbonSkin[RibbonSkins.SkinFormFrameLeftNoRibbon];
				if(elem != null)
					return elem;
			}
			return RibbonSkin[RibbonSkins.SkinFormFrameLeft];
		}
		protected override SkinElement GetSkinFrameRight() {
			if(ViewInfo == null)
				return RibbonSkin[RibbonSkins.SkinFormFrameLeft];
			if(!ViewInfo.IsAllowDisplayRibbon || !ViewInfo.GetShowPageHeaders() ||
				ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice || ViewInfo.IsOfficeTablet) {
				SkinElement elem = RibbonSkin[RibbonSkins.SkinFormFrameRightNoRibbon];
				if(elem != null)
					return elem;
			}
			return RibbonSkin[RibbonSkins.SkinFormFrameRight];
		}
		protected override SkinElement GetSkinFrameBottom() {
			string skinName = RibbonForm.StatusBar != null && RibbonForm.StatusBar.Visible == RibbonForm.Visible ? RibbonSkins.SkinFormFrameBottom : RibbonSkins.SkinFormFrameBottomWithoutStatusBar;
			return RibbonSkin[skinName];
		}
		protected override void DrawText(GraphicsCache cache) { }
		protected void CalcRibbonCaption() {
			if(RibbonForm != null && RibbonForm.IsMdiChild && RibbonForm.WindowState == FormWindowState.Maximized)
				return;
			bool releaseGraphics = false;
			if(ViewInfo != null && ViewInfo.IsReady) {
				if(ViewInfo.GInfo.Graphics == null) {
					releaseGraphics = true;
					ViewInfo.GInfo.AddGraphics(null);
				}
				ViewInfo.Caption.CalcCaption();
				if(releaseGraphics) {
					ViewInfo.GInfo.ReleaseGraphics();
				}
			}
		}
		protected override void CheckReady() {
			if(IsReady) return;
			base.CheckReady();
			CalcRibbonCaption();
		}
		protected override void DrawBackground(GraphicsCache cache) {
			Rectangle formBounds = FormBounds;
			DrawCaptionBackground(cache, formBounds);
			RibbonCaptionPainter captionPainter = RibbonForm.Ribbon.ViewInfo.Caption.CaptionPainter as RibbonCaptionPainter;
			if(captionPainter != null && !ViewInfo.Caption.Bounds.IsEmpty) {
				RibbonDrawInfo info = new RibbonDrawInfo(RibbonForm.Ribbon.ViewInfo);
				info.Cache = cache;
				captionPainter.DrawCaption(info);
			}
		}
		protected virtual void DrawCaptionBackground(GraphicsCache cache, Rectangle formBounds) {
			if(RibbonForm.Ribbon.IsDisposed)
				return;
			RibbonForm.Ribbon.CheckViewInfo();
			if(!ViewInfo.Caption.Bounds.IsEmpty) {
				SkinElementInfo siCaption = ViewInfo.Caption.GetCaptionInfo();
				siCaption.Bounds = new Rectangle(0, 0, formBounds.Width, siCaption.Bounds.Height);
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, siCaption);
			}
		}
		protected virtual SkinElementInfo GetFrameLeftTopInfo(SkinElementInfo frameInfo) {
			SkinElement element = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinFormFrameLeftTop];
			if(element == null)
				return null;
			SkinElementInfo info = new SkinElementInfo(element, new Rectangle(frameInfo.Bounds.X, frameInfo.Bounds.Y, frameInfo.Bounds.Width, ViewInfo.Header.Bounds.Bottom - 1));
			if(!IsWindowActive)
				info.ImageIndex = 1;
			return info;
		}
		protected virtual SkinElementInfo GetFrameRightTopInfo(SkinElementInfo frameInfo) {
			SkinElement element = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinFormFrameRightTop];
			if(element == null)
				return null;
			SkinElementInfo info = new SkinElementInfo(element, new Rectangle(frameInfo.Bounds.X, frameInfo.Bounds.Y, frameInfo.Bounds.Width, ViewInfo.Header.Bounds.Bottom - 1));
			if(!IsWindowActive)
				info.ImageIndex = 1;
			return info;
		}
		protected override void DrawFrameCore(GraphicsCache cache, SkinElementInfo info, FrameKind kind) {
			base.DrawFrameCore(cache, info, kind);
			if(kind == FrameKind.Bottom || RibbonForm.Ribbon == null) return;
			SkinElementInfo topInfo = null;
			if(kind == FrameKind.Left)
				topInfo = GetFrameLeftTopInfo(info);
			else if(kind == FrameKind.Right)
				topInfo = GetFrameRightTopInfo(info);
			if(topInfo != null)
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, topInfo);
			Rectangle formBounds = FormBounds;
			DrawCaptionBackground(cache, formBounds);
			if(StatusBarViewInfo == null || !RibbonForm.StatusBar.Visible) return;
			RibbonForm.StatusBar.CheckViewInfo();
			if(StatusBarViewInfo.Bounds.IsEmpty) return;
			SkinElementInfo siStatus = StatusBarViewInfo.GetStatusBarDrawInfo();
			siStatus.CorrectImageFormRTL = RibbonForm.RightToLeftLayout;
			int x = siStatus.RightToLeft ? formBounds.Width - siStatus.Bounds.Width : 0;
			siStatus.Bounds = new Rectangle(0, formBounds.Height - Margins.Bottom - siStatus.Bounds.Height, formBounds.Width, siStatus.Bounds.Height);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, siStatus);
			siStatus = StatusBarViewInfo.GetRightStatusBarDrawInfo();
			if(siStatus.Bounds.IsEmpty) return;
			x = siStatus.RightToLeft ? 0 : formBounds.Width - siStatus.Bounds.Width;
			siStatus.Bounds = new Rectangle(x, formBounds.Height - Margins.Bottom - siStatus.Bounds.Height, siStatus.Bounds.Width, siStatus.Bounds.Height);
			siStatus.CorrectImageFormRTL = RibbonForm.RightToLeftLayout;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, siStatus);
		}
		internal new FormCaptionButtonCollection Buttons { get { return base.Buttons; } }
		protected internal void ResetIconBounds() {
			IconBounds = Rectangle.Empty;
		}
		protected override void OnNCLButtonDown(Message msg) {
			InvalidateCaption();
		}
		int ApplicationIconIndent {
			get {
				if(Ribbon == null || Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007)
					return 4;
				return 0;
			}
		}
		Image IconImage = null;
		public override void DrawIcon(GraphicsCache cache) {
			Icon icon = GetIcon();
			if(IconBounds.IsEmpty || icon == null) return;
			Rectangle rect = IconBounds;
			if(!IsAllowDisplayRibbon || Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007) {
				rect.X -= FrameLeft.Width;
			}
			if(RibbonForm.IsGlassForm && icon != RibbonForm.Icon && RibbonForm.Icon != null) {
				if(IconImage == null)
					IconImage = new Bitmap(IconBounds.Width, IconBounds.Height);
				using(Graphics g = Graphics.FromImage(IconImage)) {
					g.Clear(Color.Transparent);
					g.DrawIcon(icon, new Rectangle(Point.Empty, IconBounds.Size));
					cache.Graphics.DrawImage(IconImage, rect);
				}
			}
			else
				cache.Graphics.DrawIcon(icon, rect);
		}
		public override void Dispose() {
			base.Dispose();
			if(IconImage != null)
				IconImage.Dispose();
		}
		protected internal Rectangle GetContentBoundsForIcon(Rectangle contentBounds) {
			if(Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007) {
				SkinElementInfo info = Ribbon.ViewInfo.Toolbar.GetToolbarInfoInCaption();
				contentBounds.Y += info.Element.Offset.Offset.Y;
				info.Bounds = contentBounds;
				Rectangle client = ObjectPainter.GetObjectClientRectangle(null, SkinElementPainter.Default, info);
				client.X = contentBounds.X;
				client.Width = contentBounds.Width;
				return client;
			}
			return contentBounds;
		}
		protected internal Rectangle CalcIcon(Rectangle contentBounds) {
			IconBounds = Rectangle.Empty;
			Size icon = GetIconSize();
			if(icon.IsEmpty || icon.Height > contentBounds.Height || icon.Width > contentBounds.Width) return contentBounds;
			Rectangle iconBounds = GetContentBoundsForIcon(contentBounds);
			iconBounds.Width = icon.Width;
			iconBounds = RectangleHelper.GetCenterBounds(iconBounds, icon);
			if(RibbonForm.IsRightToLeft)
				iconBounds = BarUtilites.ConvertBoundsToRTL(iconBounds, ViewInfo.Ribbon.Bounds);
			if(!IsAllowDisplayRibbon || Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007) {
				this.IconBounds = new Rectangle(iconBounds.X + FrameLeft.Width, iconBounds.Y, iconBounds.Width, iconBounds.Height);
			}
			contentBounds.Width -= (icon.Width + ApplicationIconIndent);
			contentBounds.X += (icon.Width + ApplicationIconIndent);
			if(RibbonForm.IsRightToLeft)
				contentBounds = BarUtilites.ConvertBoundsToRTL(contentBounds, ViewInfo.Ribbon.Bounds);
			return contentBounds;
		}
		protected override bool AllowShowMdiChildMenu(Point pt) {
			return false;
		}
		protected internal virtual Rectangle CalcButtons(Rectangle contentBounds) {
			Rectangle res = contentBounds;
			GInfo.AddGraphics(null);
			try {
				Buttons.Clear();
				Buttons.CreateButtons(GetVisibleButtons());
				Size buttons = Buttons.CalcSize(GInfo.Graphics, ButtonPainter);
				if(IsWindowMinimized) buttons.Height = contentBounds.Height;
				Buttons.IsRightToLeft = RibbonForm.RightToLeftLayout;
				res = Buttons.CalcBounds(null, ButtonPainter, new Rectangle(contentBounds.X, contentBounds.Y + (contentBounds.Height - buttons.Height) / 2, contentBounds.Width, buttons.Height));
				res = new Rectangle(res.X, contentBounds.Y, res.Width, contentBounds.Height);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		protected override void RedrawButtons() {
			if(ViewInfo != null) ViewInfo.Invalidate(Buttons.ButtonsBounds);
		}
		protected override Rectangle CaptionBounds {
			get {
				if((RibbonForm != null && RibbonForm.FormBorderStyle == FormBorderStyle.None) || ViewInfo == null) return Rectangle.Empty;
				if(IsWindowMinimized) return new Rectangle(0, 0, FormBounds.Width, Math.Min(ViewInfo.Caption.Bounds.Height, FormBounds.Height));
				return new Rectangle(0, 0, FormBounds.Width, ViewInfo.Caption.Bounds.Height);
			}
		}
		protected override void OnWindowActiveChanged() {
			base.OnWindowActiveChanged();
			InvalidateCaption();
		}
		internal bool IsWindowActiveCore { get { return IsWindowActive; } }
		protected virtual void InvalidateCaption() {
			if(ViewInfo != null) ViewInfo.Invalidate(ViewInfo.Caption.Bounds);
		}
		SkinPaddingEdges emptyEdges = new SkinPaddingEdges();
		public override SkinPaddingEdges Margins {
			get {
				if((RibbonForm != null && RibbonForm.FormBorderStyle == FormBorderStyle.None)) return emptyEdges;
				SkinPaddingEdges m = base.Margins;
				if(IsWindowMinimized && RibbonForm != null && RibbonForm.Parent != null)
					m.Bottom = FormBounds.Height - m.Top;
				return m;
			}
		}
		protected override SkinPaddingEdges GetZoomedMargins() {
			if(RibbonForm.FormBorderStyle == FormBorderStyle.None)
				return new SkinPaddingEdges();
			return base.GetZoomedMargins();
		}
		protected virtual bool WmSizing(ref Message msg) {
			int height = 0;
			if(RibbonForm.Ribbon != null) height += RibbonForm.Ribbon.ViewInfo.Caption.Bounds.Height;
			if(RibbonForm.StatusBar != null) height += RibbonForm.StatusBar.Height;
			NativeMethods.RECT rect = (NativeMethods.RECT)BarNativeMethods.PtrToStructure(msg.LParam, typeof(NativeMethods.RECT));
			int flags = msg.WParam.ToInt32();
			if(rect.Bottom - rect.Top >= height) return false;
			if(flags == NativeMethods.WMSZ.WMSZ_TOP || flags == NativeMethods.WMSZ.WMSZ_TOPLEFT || flags == NativeMethods.WMSZ.WMSZ_TOPRIGHT)
				rect.Top = rect.Bottom - height;
			else
				rect.Bottom = rect.Top + height;
			BarNativeMethods.StructureToPtr(rect, msg.LParam, false);
			return true;
		}
		protected virtual bool CheckOffice2010HeaderPanel(ref Message msg) {
			if(Ribbon != null && Ribbon.IsOffice2010LikeStyle) {
				Point p = PointToFormBounds(msg.LParam);
				if(RibbonForm.WindowState != FormWindowState.Maximized)
					p = new Point(p.X - FrameLeft.Width, p.Y - FrameTop.Height);
				RibbonHitInfo hi = ViewInfo.CalcHitInfo(p);
				if(hi.HitTest == RibbonHitTest.FullScreenModeBar) {
					msg.Result = new IntPtr(NativeMethods.HT.HTCLIENT);
					return true;
				}
				else if(hi.HitTest == RibbonHitTest.HeaderPanel ||
					(hi.HitTest == RibbonHitTest.Toolbar && ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above) ||
					(hi.HitTest == RibbonHitTest.FormCaption && ViewInfo.Caption.ContentBounds.Contains(p))) {
					msg.Result = new IntPtr(NativeMethods.HT.HTCAPTION);
					return true;
				}
				else if(hi.HitTest == RibbonHitTest.PageHeaderCategory) {
					if(RibbonForm.ShouldDraggingByPageCategory())
						msg.Result = new IntPtr(NativeMethods.HT.HTCAPTION);
					else
						msg.Result = new IntPtr(NativeMethods.HT.HTCLIENT);
					return true;
				}
			}
			return false;
		}
		protected override FormCaptionButtonCollection CreateFormCaptionButtonCollection() {
			return new RibbonFormCaptionButtonCollection(this);
		}
		protected override FormCaptionButtonSkinPainter CreateFormCaptionButtonSkinPainter() {
			return new RibbonFormCaptionButtonSkinPainter(this);
		}
		protected override FormCaptionButtonKind GetVisibleButtons() {
			FormCaptionButtonKind kind = base.GetVisibleButtons();
			if(ShouldAddFullScreenButton) kind |= FormCaptionButtonKind.FullScreen;
			if(RibbonForm.HelpButton && Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2013)
				kind |= FormCaptionButtonKind.Help;
			return kind;
		}
		protected virtual bool ShouldAddFullScreenButton {
			get {
				if(Ribbon == null || Ribbon.ViewInfo == null) return false;
				return Ribbon.ViewInfo.CanUseFullScreenMode;
			}
		}
		protected override void OnClick(MouseEventArgs e, FormCaptionButtonAction kind) {
			base.OnClick(e, kind);
			if(Ribbon != null && Ribbon.ViewInfo.CanTranslateToFullScreenBarClick(e, kind)) {
				OnFullScreenButtonClicked(e);
			}
		}
		protected virtual void OnFullScreenButtonClicked(MouseEventArgs e) {
			if(RibbonForm != null) RibbonForm.OnFullScreenButtonClicked(e);
		}
		protected override void WMNCHitTest(ref Message msg) {
			if(CheckOffice2010HeaderPanel(ref msg))
				return;
			base.WMNCHitTest(ref msg);
		}
		public override bool DoWndProc(ref Message msg) {
			switch(msg.Msg) {
				case MSG.WM_SIZING:
					WmSizing(ref msg);
					break;
			}
			bool res = base.DoWndProc(ref msg);
			switch(msg.Msg) {
				case MSG.WM_NCMOUSEMOVE:
				case MSG.WM_NCMOUSELEAVE:
				case MSG.WM_NCLBUTTONDOWN:
				case MSG.WM_NCLBUTTONUP:
					if(IsWindowMinimized) DrawFrameNC(msg);
					break;
			}
			return res;
		}
		internal int GetRegionDelta() {
			return FrameLeft.Width + FrameRight.Width + 1;
		}
	}
	public class RibbonBorderlessFormPainter : RibbonFormPainter {
		public RibbonBorderlessFormPainter(RibbonForm form, ISkinProvider provider)
			: base(form, provider) {
		}
		protected override bool WMSize(ref Message msg) {
			return false;
		}
	}
	public class RibbonFormCaptionButtonCollection : FormCaptionButtonCollection {
		public RibbonFormCaptionButtonCollection(FormPainter owner)
			: base(owner) {
		}
		public override void CreateButtons(FormCaptionButtonKind visibleButtons) {
			base.CreateButtons(visibleButtons);
			int insertIndex = 0;
			int helpButtonIndex = GetHelpButtonIndex();
			if(helpButtonIndex >= 0)
				insertIndex = helpButtonIndex + 1;
			InsertButton(insertIndex, FormCaptionButtonKind.FullScreen, visibleButtons);
		}
		int GetHelpButtonIndex() {
			for(int i = 0; i < Count; i++) {
				if(this[i].Kind == FormCaptionButtonKind.Help) {
					return i;
				}
			}
			return -1;
		}
		protected override FormCaptionButton CreateFormCaptionButton(FormCaptionButtonKind kind) {
			return new RibbonFormCaptionButton(kind);
		}
		protected override Rectangle CalcBoundsCore(Graphics g, ObjectPainter painter, Rectangle bounds, bool isMdi) {
			try {
				if(Owner != null && Owner.IsDrawCaptionCore) {
					bounds.X -= Owner.Margins.Right;
				}
				return base.CalcBoundsCore(g, painter, bounds, isMdi);
			}
			finally {
				ButtonsBounds = CalcButtonsBounds(g, painter, bounds);
			}
		}
		protected virtual Rectangle CalcButtonsBounds(Graphics g, ObjectPainter painter, Rectangle bounds) {
			Size size = CalcSize(g, painter);
			int x = IsRightToLeft ? bounds.X : bounds.Right - size.Width;
			return new Rectangle(x, bounds.Y, size.Width, size.Height);
		}
		public new RibbonFormPainter Owner { get { return base.Owner as RibbonFormPainter; } }
	}
	public class RibbonFormCaptionButtonSkinPainter : FormCaptionButtonSkinPainter {
		public RibbonFormCaptionButtonSkinPainter(ISkinProvider owner)
			: base(owner) {
		}
		protected override SkinElement GetSkinElement(ObjectInfoArgs e) {
			FormCaptionButton button = e as FormCaptionButton;
			if(button.Action == FormCaptionButtonAction.FullScreen) {
				SkinElement element = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinFormButtonFullScreen];
				return element ?? FormSkins.GetSkin(Provider)[FormSkins.SkinFormButtonFullScreen];
			}
			string elementName = string.Empty;
			switch(button.Action) {
				case FormCaptionButtonAction.Maximize:
					elementName = RibbonSkins.SkinFormButtonMaximize; break;
				case FormCaptionButtonAction.Minimize:
					elementName = RibbonSkins.SkinFormButtonMinimize; break;
				case FormCaptionButtonAction.Restore:
					elementName = RibbonSkins.SkinFormButtonRestore; break;
				case FormCaptionButtonAction.Close:
					elementName = RibbonSkins.SkinFormButtonClose; break;
			}
			return GetSkinElementCore(elementName) ?? base.GetSkinElement(e);
		}
		protected virtual SkinElement GetSkinElementCore(string elementName) {
			return RibbonSkins.GetSkin(Provider)[elementName];
		}
	}
	public class RibbonFormCaptionButton : FormCaptionButton {
		public RibbonFormCaptionButton(FormCaptionButtonKind kind)
			: base(kind) {
		}
		protected override FormCaptionButtonAction GetActionCore(IntPtr handle, FormCaptionButtonKind kind) {
			if(kind == FormCaptionButtonKind.FullScreen) return FormCaptionButtonAction.FullScreen;
			return base.GetActionCore(handle, kind);
		}
	}
}
namespace DevExpress.XtraBars.MVVM.Services {
	using DevExpress.Utils.MVVM.Services;
	class ServiceFormBase : RibbonForm {
		protected RibbonControl ribbonControl;
		protected RibbonPage ribbonPage;
		protected RibbonPageGroup ribbonPageGroup;
		protected ServiceFormBase() {
			InitializeComponent();
		}
		protected virtual void InitializeComponent() {
			this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).BeginInit();
			this.SuspendLayout();
			this.ribbonControl.ExpandCollapseItem.Id = 0;
			this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.ribbonControl.ExpandCollapseItem,
			this.ribbonControl.AutoHiddenPagesMenuItem});
			this.ribbonControl.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl.MaxItemId = 1;
			this.ribbonControl.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always;
			this.ribbonControl.Name = "ribbonControl";
			this.ribbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;
			this.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl.Size = new System.Drawing.Size(678, 144);
			InitializePagesAndItems();
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(678, 408);
			this.Controls.Add(this.ribbonControl);
			this.Name = "RibbonForm";
			this.Ribbon = this.ribbonControl;
			this.ShowIcon = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "RibbonForm";
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		protected virtual void InitializePagesAndItems() {
			this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.ribbonPage});
			this.ribbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.ribbonPageGroup});
			this.ribbonPage.Name = "ribbonPage";
			this.ribbonPage.Text = "ribbonPage";
			this.ribbonPageGroup.AllowTextClipping = false;
			this.ribbonPageGroup.Name = "ribbonPageGroup";
			this.ribbonPageGroup.ShowCaptionButton = false;
			this.ribbonPageGroup.Text = "ribbonPageGroup";
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			var childRibbon = FindRibbon(e.Control);
			if(childRibbon != null)
				Ribbon.MergeRibbon(childRibbon);
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			Ribbon.UnMergeRibbon();
			base.OnControlRemoved(e);
		}
		static RibbonControl FindRibbon(Control container) {
			foreach(Control ctrl in container.Controls) {
				if(ctrl is RibbonControl) 
					return ctrl as RibbonControl;
			}
			return null;
		}
	}
	public sealed class RibbonDialogFormFactory : IDialogFormFactory {
		IDialogForm IDialogFormFactory.Create() {
			return new DialogForm();
		}
		class DialogForm : ServiceFormBase, IDialogForm {
			BarButtonItem[] buttonItems;
			protected override void InitializePagesAndItems() {
				base.InitializePagesAndItems();
				ribbonPage.Text = "DIALOG";
				ribbonPageGroup.Text = "Dialog";
				var bbiOK = new BarButtonItem();
				bbiOK.Caption = "OK";
				bbiOK.Id = 1;
				bbiOK.Name = "bbiOK";
				bbiOK.Tag = DialogResult.OK;
				bbiOK.ImageUri.Uri = "Apply";
				bbiOK.ItemClick += Ribbon_ItemClick;
				var bbiCancel = new BarButtonItem();
				bbiCancel.Caption = "Cancel";
				bbiCancel.Id = 2;
				bbiCancel.Name = "bbiCancel";
				bbiCancel.Tag = DialogResult.Cancel;
				bbiCancel.ImageUri.Uri = "Cancel";
				bbiCancel.ItemClick += Ribbon_ItemClick;
				var bbiYes = new BarButtonItem();
				bbiYes.Caption = "Yes";
				bbiYes.Id = 1;
				bbiYes.Name = "bbiYes";
				bbiYes.Tag = DialogResult.Yes;
				bbiYes.ImageUri.Uri = "Apply";
				bbiYes.ItemClick += Ribbon_ItemClick;
				var bbiNo = new BarButtonItem();
				bbiNo.Caption = "No";
				bbiNo.Id = 1;
				bbiNo.Name = "bbiNo";
				bbiNo.Tag = DialogResult.No;
				bbiNo.ImageUri.Uri = "Cancel";
				bbiNo.ItemClick += Ribbon_ItemClick;
				ribbonPageGroup.ItemLinks.Add(bbiOK);
				ribbonPageGroup.ItemLinks.Add(bbiCancel);
				ribbonPageGroup.ItemLinks.Add(bbiYes);
				ribbonPageGroup.ItemLinks.Add(bbiNo);
				buttonItems = new BarButtonItem[] { 
					bbiOK,
					bbiCancel,
					bbiYes,
					bbiNo
				};
			}
			void Ribbon_ItemClick(object sender, ItemClickEventArgs e) {
				if(e.Item.Tag is DialogResult)
					DialogResult = (DialogResult)e.Item.Tag;
			}
			Control content;
			Control IDialogForm.Content {
				get { return content; }
			}
			DialogResult IDialogForm.ShowDialog(IWin32Window owner, Control content, string caption, DialogResult[] buttons) {
				this.ClientSize = new Size(content.Width, content.Height + ribbonControl.Height);
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
				this.Owner = owner as Form ?? GuessOwner();
				if(Owner != null) {
					this.RightToLeft = Owner.RightToLeft;
					this.RightToLeftLayout = Owner.RightToLeftLayout;
				}
				this.content = content;
				foreach(BarButtonItem item in buttonItems) {
					if(item.Tag is DialogResult)
						item.Visibility = (Array.IndexOf(buttons, item.Tag) != -1) ? BarItemVisibility.Always : BarItemVisibility.Never;
				}
				content.Dock = DockStyle.Fill;
				content.Parent = this;
				Ribbon.ApplicationCaption = caption;
				return ShowDialog();
			}
			internal static Form GuessOwner() {
				Form frm = Form.ActiveForm;
				if(frm == null || frm.InvokeRequired)
					return null;
				while(frm != null && frm.Owner != null && !frm.ShowInTaskbar && !frm.TopMost)
					frm = frm.Owner;
				return frm;
			}
		}
	}
	public sealed class RibbonDocumentFormFactory : IDocumentFormFactory {
		Form IDocumentFormFactory.Create() {
			return new DocumentForm();
		}
		class DocumentForm : ServiceFormBase {
			protected override void InitializePagesAndItems() {
				base.InitializePagesAndItems();
				ribbonPage.Visible = false;
				ribbonPageGroup.Visible = false;
			}
		}
	}
}
