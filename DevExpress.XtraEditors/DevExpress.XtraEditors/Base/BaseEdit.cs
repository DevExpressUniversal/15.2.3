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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Win;
using DevExpress.Utils.Design;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.Skins;
namespace DevExpress.XtraEditors.Persistent {
	[Obsolete("Obsolete class.")]
	public class PersistentBaseEdit {
	}
}
namespace DevExpress.XtraEditors {
	[ToolboxItem(false), SmartTagSupport(typeof(BaseControlBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto), 
	SmartTagFilter(typeof(ControlFilter)),
	SmartTagAction(typeof(ControlActions), "DockInParentContainer", "Dock in parent container", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(ControlActions), "UndockFromParentContainer", "Undock from parent container", SmartTagActionType.CloseAfterExecute)]
	public abstract class BaseControl : ControlBase, IDXFocusController, IToolTipControlClient, ISupportLookAndFeel, ISupportStyleController, IXtraResizableControl,
						ISupportXtraAnimation {
		UserLookAndFeel lookAndFeel;
		BorderStyles borderStyle;
		bool _focusOnMouseDown;
		protected bool fShowToolTips;
		IStyleController fStyleController;
		string _toolTip, _toolTipTitle;
		DefaultBoolean allowHtmlText;
		SuperToolTip superTip;
		ToolTipIconType _toolTipIconType;
		ToolTipController toolTipController;
		protected bool fDisposing;
		static BaseControl() {
			DevExpress.Utils.Design.DXAssemblyResolver.Init();
		}
		public BaseControl() {
			this.SetStyle(ControlStyles.UserMouse, true);
			ToolTipController.DefaultController.AddClientControl(this);
			this._toolTipTitle = this._toolTip = "";
			this._toolTipIconType = ToolTipIconType.None;
			this.allowHtmlText = DefaultBoolean.Default;
			this.fShowToolTips = true;
			this.borderStyle = BorderStyles.Default;
			this.fStyleController = null;
			this.lookAndFeel = null;
			this._focusOnMouseDown = true;
		}
		SizeF scaleFactor = new SizeF(1f, 1f);
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SizeF ScaleFactor { get { return scaleFactor; } }
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			this.scaleFactor = factor;
			base.ScaleControl(factor, specified);
		}
		internal void UpdateScaleFactor(SizeF factor) {
			this.scaleFactor = factor;
		}
		protected internal Size ScaleSize(Size size) { return RectangleHelper.ScaleSize(size, ScaleFactor); }
		protected internal int ScaleHorizontal(int width) { return RectangleHelper.ScaleHorizontal(width, ScaleFactor.Width); }
		protected internal int ScaleVertical(int height) { return RectangleHelper.ScaleVertical(height, ScaleFactor.Height); }
		protected internal int DeScaleHorizontal(int width) { return RectangleHelper.DeScaleHorizontal(width, ScaleFactor.Width); }
		protected internal int DeScaleVertical(int height) { return RectangleHelper.DeScaleVertical(height, ScaleFactor.Height); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DevExpress.Accessibility.BaseAccessible GetAccessible() { return DXAccessible; }
		DevExpress.Accessibility.BaseAccessible dxAccessible;
		protected internal virtual DevExpress.Accessibility.BaseAccessible DXAccessible {
			get {
				if(dxAccessible == null) dxAccessible = CreateAccessibleInstance();
				return dxAccessible;
			}
		}
		protected virtual bool IsDisposing { get { return fDisposing; } }
		protected virtual DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return null;
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(DXAccessible == null) return base.CreateAccessibilityInstance();
			return DXAccessible.Accessible;
		}
		internal const int CS_VREDRAW = 0x0001, CS_HREDRAW = 0x0002;
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		protected override CreateParams CreateParams {
			get {
				System.Windows.Forms.CreateParams res = base.CreateParams;
				res.ClassStyle |= CS_VREDRAW | CS_HREDRAW;
				return res;
			}
		}
		public virtual Size CalcBestSize() {
			Size res = DefaultSize;
			if(ViewInfo == null) return res;
			if(!ViewInfo.IsReady) {
				ViewInfo.UpdateAppearances();
				ViewInfo.UpdatePaintAppearance();
			}
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				res = ViewInfo.CalcBestFit(ginfo.Graphics);
			}
			finally {
				ginfo.ReleaseGraphics();
			}
			return res;
		}
		protected void SetBackColor(Color color) {
			base.BackColor = color;
		}
		protected internal bool ShowKeyboardCuesCore { get { return ShowKeyboardCues; } }
		protected override Size DefaultSize { get { return new Size(75, 14); } }
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			if(disposing) {
				StyleController = null;
				if(this.lookAndFeel != null) {
					lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
				}
				if(lookAndFeel != null) {
					lookAndFeel.Dispose();
				}
				if(disposing) {
					ToolTipController = null;
					ToolTipController.DefaultController.RemoveClientControl(this);
				}
				XtraAnimator.RemoveObject(this);
			}
			base.Dispose(disposing);
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlToolTipController"),
#endif
 DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null)
					ToolTipController.RemoveClientControl(this);
				toolTipController = value;
				if(ToolTipController != null) {
					ToolTipController.DefaultController.RemoveClientControl(this);
					ToolTipController.AddClientControl(this);
				}
				else ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		[DXCategory(CategoryName.ToolTip), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlShowToolTips"),
#endif
 DefaultValue(true)]
		public virtual bool ShowToolTips { get { return fShowToolTips; } set { fShowToolTips = value; } }
		[DXCategory(CategoryName.ToolTip), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlToolTip"),
#endif
 DefaultValue(""), Localizable(true), Editor(ControlConstants.MultilineStringEditor, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string ToolTip {
			get { return _toolTip; }
			set {
				if(value == null) value = string.Empty;
				if(ToolTip == value) return;
				_toolTip = value;
			}
		}
		[DXCategory(CategoryName.ToolTip), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlToolTipTitle"),
#endif
 DefaultValue(""), Localizable(true)]
		public virtual string ToolTipTitle {
			get { return _toolTipTitle; }
			set {
				if(value == null) value = string.Empty;
				if(ToolTipTitle == value) return;
				_toolTipTitle = value;
			}
		}
		[DXCategory(CategoryName.ToolTip), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlAllowHtmlTextInToolTip"),
#endif
 DefaultValue(DefaultBoolean.Default), Localizable(true)]
		public virtual DefaultBoolean AllowHtmlTextInToolTip {
			get { return allowHtmlText; }
			set {
				if (AllowHtmlTextInToolTip == value) return;
				allowHtmlText = value;
			}
		}
		internal bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		[Localizable(true), DXCategory(CategoryName.ToolTip), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlSuperTip"),
#endif
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))
		]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		public void ResetSuperTip() { SuperTip = null; }
		[DXCategory(CategoryName.ToolTip), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlToolTipIconType"),
#endif
 DefaultValue(ToolTipIconType.None), Localizable(true)]
		public virtual ToolTipIconType ToolTipIconType {
			get { return _toolTipIconType; }
			set {
				_toolTipIconType = value;
			}
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return GetToolTipInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return ShowToolTipsCore; } }
		protected virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			if(!IsDesignMode && ShowToolTips && (ToolTip.Length > 0 || ShouldSerializeSuperTip())) {
				ToolTipControlInfo res = new ToolTipControlInfo(this, ToolTip, ToolTipTitle, ToolTipIconType);
				res.AllowHtmlText = AllowHtmlTextInToolTip;
				res.SuperTip = SuperTip;
				return res;
			}
			return null;
		}
		protected virtual bool ShowToolTipsCore { get { return ShowToolTips; } }
		protected internal bool FocusOnMouseDown { get { return ((IDXFocusController)this).FocusOnMouseDown; } set { ((IDXFocusController)this).FocusOnMouseDown = value; } }
		bool IDXFocusController.FocusOnMouseDown { get { return _focusOnMouseDown; } set { _focusOnMouseDown = value; } }
		[Browsable(false)]
		public virtual bool IsDesignMode { get { return DesignMode; } }
		bool ShouldSerializeLookAndFeel() { return StyleController == null && LookAndFeel != null && LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlLookAndFeel"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel {
			get {
				if(StyleController != null && StyleController.LookAndFeel != null) return StyleController.LookAndFeel;
				if(lookAndFeel == null)
					CreateLookAndFeel();
				return lookAndFeel;
			}
		}
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			if(ViewInfo != null) {
				ViewInfo.ResetAppearanceDefault();
				ViewInfo.UpdatePaintersCore();
			}
			CheckRightToLeft();
			OnPropertiesChanged(true);
		}
		protected override void OnRightToLeftChanged() {
			base.OnRightToLeftChanged();
			LayoutChanged();
		}
		protected virtual void CreateLookAndFeel() {
			lookAndFeel = new ControlUserLookAndFeel(this);
			lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
		}
		public override void Refresh() {
			LayoutChanged();
			base.Refresh();
		}
		protected internal virtual void RemoveXtraAnimator(){
			XtraAnimator.RemoveObject(this);
		}
		protected internal virtual void LayoutChangedCore(bool invalidateVisual) {
			if(IsDisposing)
				return;
			RemoveXtraAnimator();
			if(IsLayoutLocked || !IsHandleCreated) {
				ViewInfo.IsReady = false;
				return;
			}
			ViewInfo.CalcViewInfo(null, Control.MouseButtons, PointToClient(Control.MousePosition), ClientRectangle);
			if(invalidateVisual)
				Invalidate();
		}
		protected internal virtual void LayoutChanged() {
			LayoutChangedCore(true);
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlStyleController"),
#endif
 DefaultValue(null)]
		public virtual IStyleController StyleController {
			get { return fStyleController; }
			set {
				if(StyleController == value) return;
				if(StyleController != null) {
					StyleController.PropertiesChanged -= new EventHandler(OnStyleController_PropertiesChanged);
					StyleController.Disposed -= new EventHandler(OnStyleController_Disposed);
				}
				this.fStyleController = value;
				if(StyleController != null) {
					StyleController.PropertiesChanged += new EventHandler(OnStyleController_PropertiesChanged);
					StyleController.Disposed += new EventHandler(OnStyleController_Disposed);
				}
				OnStyleControllerChanged();
			}
		}
		protected BorderStyles BaseBorderStyle { get { return borderStyle; } }
		bool ShouldSerializeBorderStyle() { return StyleController == null && BorderStyle != BorderStyles.Default; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlBorderStyle"),
#endif
 DXCategory(CategoryName.Appearance)]
		public virtual BorderStyles BorderStyle {
			get {
				if(StyleController != null) return StyleController.BorderStyle;
				return borderStyle;
			}
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				OnPropertiesChanged(true);
			}
		}
		internal void OnPropertiesChanged(bool shrpc) {
			shouldRaiseSizeableChanged = shrpc;
			OnPropertiesChanged();
		}
		protected internal bool shouldRaiseSizeableChanged = false;
		protected internal virtual void OnPropertiesChanged() {
			if(IsDisposing) return;
			if(shouldRaiseSizeableChanged) {
				ViewInfo.IsReady = false;
				RaiseSizeableChanged(); shouldRaiseSizeableChanged = false; 
			}
			FireChanged();
			LayoutChanged();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BaseControlViewInfo GetViewInfo() { return ViewInfo; }
		protected internal abstract BaseControlPainter Painter { get; }
		protected internal abstract BaseControlViewInfo ViewInfo { get; }
		protected virtual void OnStyleControllerChanged() {
			OnPropertiesChanged(true);
		}
		protected Color GetColor(Color color, Color defaultColor) { return GetColor(color, defaultColor, true); }
		protected Color GetColor(Color color, Color defaultColor, bool useColor) {
			return color == Color.Empty || !useColor? defaultColor : color;
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(ViewInfo.Bounds.Size == ClientSize) return;
			LayoutChanged();
		}
		protected virtual void UpdateViewInfo(Graphics g) {
			if(ViewInfo.IsReady) return;
			if(!IsHandleCreated || IsDisposed) return;
			OnBeforeUpdateViewInfo();
			ViewInfo.CalcViewInfo(g, MouseButtons, PointToClient(MousePosition), ClientRectangle);
			OnAfterUpdateViewInfo();
		}
		protected virtual void OnBeforeUpdateViewInfo() { }
		protected virtual void OnAfterUpdateViewInfo() { }
		protected override void OnPaint(PaintEventArgs e) {
			DevExpress.Utils.Mdi.ControlState.CheckPaintError(this);
			UpdateViewInfo(e.Graphics);
			GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(e));
			try {
				ControlGraphicsInfoArgs info = new ControlGraphicsInfoArgs(ViewInfo, cache, ViewInfo.Bounds);
				ISupportGlassRegions reg = Parent as ISupportGlassRegions;
				if(reg != null) {
					info.IsDrawOnGlass = reg.IsOnGlass(Parent.RectangleToClient(RectangleToScreen(ViewInfo.Bounds)));
				}
				Painter.Draw(info);
				RaisePaintEvent(this, e);
			}
			finally {
				cache.Dispose();
			}
		}
		protected void UpdateViewInfoState() { UpdateViewInfoState(true); }
		protected virtual void UpdateViewInfoState(bool useValidMouse, Point actualMousePoint) {
			if(!IsHandleCreated) return;
			if(ViewInfo.UpdateObjectState(useValidMouse ? Control.MouseButtons : MouseButtons.None, useValidMouse ? actualMousePoint : new Point(-10000, -10000))) Invalidate();
		}
		protected virtual void UpdateViewInfoState(bool useValidMouse) {
			if(!IsHandleCreated) return;
			if(ViewInfo.UpdateObjectState(useValidMouse ? Control.MouseButtons : MouseButtons.None, useValidMouse ? PointToClient(Control.MousePosition) : new Point(-10000, -10000))) Invalidate();
		}
		protected override void OnMouseLeave(EventArgs e) {
			UpdateViewInfoState(false);
			base.OnMouseLeave(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			UpdateViewInfoState();
			base.OnMouseEnter(e);
		}
		protected virtual bool IsLayoutLocked { get { return false; } }
		[Browsable(false)]
		public virtual bool IsLoading { get { return false; } }
		int lockFireChanged = 0;
		internal void LockFireChanged() { lockFireChanged++; }
		internal void UnlockFireChanged() { lockFireChanged--; }
		protected internal virtual void FireChanged() {
			if(Site == null || IsLoading || this.lockFireChanged != 0) return;
			IDesignerHost dh = Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(dh != null && dh.Loading)
				return;
			IComponentChangeService srv = Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(this, null, null, null);
		}
		protected virtual void OnStyleController_PropertiesChanged(object sender, EventArgs e) {
			OnStyleControllerChanged();
		}
		protected virtual void OnStyleController_Disposed(object sender, EventArgs e) {
			StyleController = null;
		}
		Size IXtraResizableControl.MinSize { get { return SizeableMinSize; } }
		Size IXtraResizableControl.MaxSize { get { return SizeableMaxSize; } }
		private static readonly object sizeableChanged = new object();
		event EventHandler IXtraResizableControl.Changed {
			add { Events.AddHandler(sizeableChanged, value); }
			remove { Events.RemoveHandler(sizeableChanged, value); }
		}
		bool IXtraResizableControl.IsCaptionVisible { get { return SizeableIsCaptionVisible; } }
		bool autoSizeInLayoutControl = false;
		[ DXCategory(CategoryName.Properties), RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual bool AutoSizeInLayoutControl {
			get { return autoSizeInLayoutControl; }
			set { autoSizeInLayoutControl = value; }
		}
		protected virtual Size CalcSizeableMinSize() { return AutoSizeInLayoutControl ? CalcBestSize() : new Size(50, CalcBestSize().Height); }
		protected virtual Size CalcSizeableMaxSize() {
			return AutoSizeInLayoutControl ? CalcBestSize() : new Size(0, CalcBestSize().Height);
		}
		protected Size SizeableMinSize {
			get {
				this.prevSizeableMinSize = CalcSizeableMinSize();
				return this.prevSizeableMinSize;
			}
		}
		protected Size SizeableMaxSize {
			get {
				this.prevSizeableMaxSize = CalcSizeableMaxSize();
				return this.prevSizeableMaxSize;
			}
		}
		protected virtual void RaiseSizeableChanged() {
			EventHandler changed = (EventHandler)Events[sizeableChanged];
			if(changed == null) return;
			Size minSize = this.prevSizeableMinSize, maxSize = this.prevSizeableMaxSize;
			if(minSize == SizeableMinSize && maxSize == SizeableMaxSize) return;
			changed(this, EventArgs.Empty);
		}
		Size prevSizeableMinSize = Size.Empty, prevSizeableMaxSize = Size.Empty;
		protected virtual bool SizeableIsCaptionVisible { get { return false; } }
		Control ISupportXtraAnimation.OwnerControl { get { return this; } }
		bool ISupportXtraAnimation.CanAnimate {
			get { return CanAnimateCore; } 
		}
		protected virtual bool CanAnimateCore { 
			get {
				if(!ViewInfo.AllowAnimation) return false;
				return XtraAnimator.Current.CanAnimate(LookAndFeel); 
			} 
		}
		protected override void WndProc(ref Message msg)
		{
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
	}
	public abstract class BaseStyleControl : BaseControl {
		BaseStyleControlViewInfo viewInfo;
		BaseControlPainter painter;
		AppearanceObject appearance;
		public BaseStyleControl() {
			this.appearance = CreateAppearance();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer, true);
			this.painter = CreatePainter();
			this.viewInfo = CreateViewInfo();
		}
		protected virtual AppearanceObject CreateAppearance() {
			AppearanceObject res = new AppearanceObject();
			res.Changed += new EventHandler(OnStyleChanged);
			return res;
		}
		protected virtual void DestroyAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			appearance.Changed -= new EventHandler(OnStyleChanged);
			appearance.Dispose();
		}
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			if(disposing) {
				DestroyAppearance(Appearance);
			}
			base.Dispose(disposing);
		}
		protected abstract BaseControlPainter CreatePainter();
		protected abstract BaseStyleControlViewInfo CreateViewInfo();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetViewInfo() { this.viewInfo = null; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetPainter() { this.painter = null; }
		protected internal override BaseControlPainter Painter { 
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter; 
			} 
		}
		protected internal override BaseControlViewInfo ViewInfo { 
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo; 
			} 
		}
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseStyleControlAppearance"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Appearance { get { return appearance; } }
		protected virtual void OnStyleChanged(object sender, EventArgs e) {
			OnPropertiesChanged(true);
		}
		public override void ResetBackColor() { BackColor = Color.Empty; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseStyleControlBackColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return GetColor(Appearance.BackColor, ViewInfo == null ? Color.Empty : ViewInfo.DefaultAppearance.BackColor, Appearance.Options.UseBackColor); }
			set { Appearance.BackColor = value; }
		}
		public override void ResetForeColor() { ForeColor = Color.Empty; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseStyleControlForeColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return GetColor(Appearance.ForeColor, ViewInfo == null ? Color.Empty : ViewInfo.DefaultAppearance.ForeColor, Appearance.Options.UseForeColor); }
			set { Appearance.ForeColor = value; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseStyleControlFont"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font {
			get { return Appearance.Font; }
			set { Appearance.Font = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler ForeColorChanged {
			add { base.ForeColorChanged += value; }
			remove { base.ForeColorChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler BackColorChanged {
			add { base.BackColorChanged += value; }
			remove { base.BackColorChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler FontChanged {
			add { base.FontChanged += value; }
			remove { base.FontChanged -= value; }
		}
	}
	[DefaultEvent("EditValueChanged"), ToolboxItem(false),
	 DefaultBindingProperty("EditValue"),
	 Designer("DevExpress.XtraEditors.Design.BaseEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign)
	]
	public class BaseEdit : BaseControl {
		public static ErrorIconAlignment DefaultErrorIconAlignment = ErrorIconAlignment.MiddleLeft;
		int lockEditValueChangedEvent = 0;
		IEditorContainer fEditorContainer = null;
		IPopupServiceControl _serviceObject;
		static IPopupServiceControl popupServiceControl = new DevExpress.XtraEditors.Controls.HookPopupController();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static IPopupServiceControl PopupServiceControl { get { return popupServiceControl; } }
		int _lockAutoHeight = 0, _lockUpdate = 0, acceptEditValue = 0;
		bool _isModified;
		InplaceType _inplaceType;
		protected internal RepositoryItem fProperties;
		BaseEditViewInfo _viewInfo;
		protected object fEditValue, fOldEditValue;
		Image errorIcon = null;
		string errorText;
		ErrorIconAlignment errorIconAlignment;
		bool _allowValidate;
		DevExpress.XtraEditors.Senders.BaseSender _eventSender;
		[ThreadStatic]
		static Image defaultErrorIcon;
		IDXMenuManager menuManager = null;
		[ThreadStatic]
		static bool errorIconChanged = false;
		bool enterMoveNextControl;
		internal static bool ErrorIconChanged { get { return errorIconChanged; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditDefaultErrorIcon")
#else
	Description("")
#endif
]
		public static Image DefaultErrorIcon {
			get {
				if(defaultErrorIcon == null)
					defaultErrorIcon = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Images.Error.png", typeof(BaseEdit).Assembly);
				return defaultErrorIcon;
			}
			set {
				errorIconChanged = (value != null);
				defaultErrorIcon = value;
			}
		}
		private static readonly object invalidValue = new object();
		static BaseEdit() {
		}
		public static bool IsNotLoadedValue(object value) {
			return DevExpress.Data.AsyncServerModeDataController.IsNoValue(value);
		}
		public static void About() {
		}
		public BaseEdit() {
			Permissions.Request();
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.errorText = "";
			this.errorIconAlignment = DefaultErrorIconAlignment;
			this._serviceObject = popupServiceControl;
			this._lockAutoHeight = 0;
			this._inplaceType = InplaceType.Standalone;
			this._eventSender = CreateEventSender();
			this.enterMoveNextControl = false;
			CreateRepositoryItem();
			BeginUpdate();
			try {
				InitializeDefault();
			}
			finally {
				CancelUpdate();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Padding Padding {
			get { return base.Padding; }
			set { base.Padding = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditMenuManager"),
#endif
 DefaultValue(null), DXCategory(CategoryName.BarManager)]
		public IDXMenuManager MenuManager {
			get { return menuManager; }
			set { menuManager = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ErrorText {
			get { return errorText; }
			set {
				if(value == null) value = "";
				if(ErrorText == value) return;
				errorText = value;
				RefreshVisualLayout();
				RaiseSizeableChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ErrorIconAlignment ErrorIconAlignment {
			get { return errorIconAlignment; }
			set {
				if(ErrorIconAlignment == value) return;
				errorIconAlignment = value;
				RefreshVisualLayout();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Image ErrorIcon {
			get { return errorIcon; }
			set {
				if(ErrorIcon == value) return;
				errorIcon = value;
				RefreshVisualLayout();
			}
		}
		internal bool IsDataBindingActive { get { return DataBindings.Count > 0; } }
		internal bool IsDataBindingProperty(string name) { return DataBindings[name] != null; }
		internal bool IsBoundToEditValue { get { return IsDataBindingProperty("EditValue"); } }
		internal bool IsBoundUpdatingProperty(string name) { return IsInPropertySetting(DataBindings[name]); }
		internal bool IsBoundUpdatingEditValue { get { return IsBoundUpdatingProperty("EditValue"); } }
		internal bool IsBoundPropertyUpdating { get { return IsDataBindingActive && IsBoundUpdatingProperty(DataBindings[0].PropertyName); } }
		static FieldInfo inSetPropValueFieldInfo = null;
		internal bool IsInPropertySetting(Binding binding) {
			if(binding == null) return false;
			try {
				if(inSetPropValueFieldInfo == null) inSetPropValueFieldInfo = typeof(Binding).GetField("inSetPropValue", BindingFlags.NonPublic | BindingFlags.Instance);
				if(inSetPropValueFieldInfo != null) return (bool)inSetPropValueFieldInfo.GetValue(binding);
			}
			catch {
			}
			return false;
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			if(ViewInfo == null || IsDesignMode) return null;
			return ((BaseEditViewInfo)ViewInfo).GetToolTipInfo(point);
		}
		protected override bool ShowToolTipsCore {
			get { return base.ShowToolTipsCore || ErrorText.Length > 0; }
		}
		protected virtual void LockEditValueChanged() {
			this.lockEditValueChangedEvent++;
		}
		protected virtual void UnLockEditValueChanged() {
			this.lockEditValueChangedEvent--;
		}
		protected virtual void UpdateViewInfoEditValue() {
			if(IsLayoutLocked) return;
			((BaseEditViewInfo)ViewInfo).UpdateEditValue();
		}
		protected virtual bool AllowFireEditValueChanged { get { return lockEditValueChangedEvent == 0; } }
		protected internal virtual Control InnerControl { get { return null; } }
		protected internal virtual IEditorContainer EditorContainer { get { return fEditorContainer; } }
		internal void SetEditorContainer(IEditorContainer editorContainer) {
			this.fEditorContainer = editorContainer;
			UpdateScaleFactor(editorContainer.ScaleFactor);
		}
		protected override void Dispose(bool disposing) {
			this.fDisposing = true;
			if(disposing) {
				if(Properties != null) {
					this.fProperties.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected virtual void LayoutChangedCore() {
			LayoutChanged();
		}
		protected internal override void LayoutChanged() {
			if(IsDisposing)
				return;
			if(!IsLayoutLocked) {
				int prevHeight = this.preferredHeight;
				this.preferredHeight = -1;
				base.LayoutChanged();
				if(prevHeight != PreferredHeight) BaseControl.ClearPreferredSizeCache(this);
			}
			else {
				base.LayoutChanged();
			}
		}
		internal int visualLayoutUpdate = 0;
		internal void IncVisualLayoutUpdate() { this.visualLayoutUpdate++; }
		internal void DecVisualLayoutUpdate() { this.visualLayoutUpdate--; }
		protected internal virtual void RefreshVisualLayout() {
			RefreshVisualLayoutCore(false);
		}
		protected internal virtual void RefreshVisualLayoutCore(bool focusLost) {
			this.visualLayoutUpdate++;
			try {
				if(focusLost) LayoutChangedCore();
				else LayoutChanged();
			}
			finally {
				this.visualLayoutUpdate--;
			}
		}
		protected bool IsVisualLayoutUpdate { get { return this.visualLayoutUpdate != 0; } }
		protected virtual void BeginAcceptEditValue() {
			this.acceptEditValue++;
		}
		protected virtual void EndAcceptEditValue() {
			this.acceptEditValue--;
		}
		protected bool IsAcceptingEditValue { get { return this.acceptEditValue != 0; } }
		protected virtual void BeginUpdate() {
			++this._lockUpdate;
		}
		protected virtual void EndUpdate() {
			if(--this._lockUpdate == 0) LayoutChanged();
		}
		protected virtual void CancelUpdate() {
			--this._lockUpdate;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IPopupServiceControl ServiceObject {
			get { return _serviceObject; }
			set {
				if(value == null) return;
				_serviceObject = value;
			}
		}
		protected virtual internal bool PaintErrorIconBackground(Graphics g, Rectangle bounds) {
			return BackgroundPaintHelper.PaintTransparentBackground(this, new PaintEventArgs(g, bounds), bounds,
				new BackgroundPaintHelper.PaintInvoke(InvokePaint), new BackgroundPaintHelper.PaintInvoke(InvokePaintBackground));
		}
		protected virtual DevExpress.XtraEditors.Senders.BaseSender CreateEventSender() {
			return new DevExpress.XtraEditors.Senders.NativeSender();
		}
		protected virtual DevExpress.XtraEditors.Senders.BaseSender EventSender { get { return _eventSender; } }
		public virtual void SelectAll() {
		}
		public virtual void DeselectAll() {
		}
		public virtual void Reset() {
			IsModified = false;
		}
		internal void DoPreviewKeyDown(PreviewKeyDownEventArgs e) {
			OnPreviewKeyDown(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			OnEditorKeyDown(e);
		}
		protected virtual void OnEditorKeyDown(KeyEventArgs e) { if(EditorContainer != null) EditorContainer.OnEditorKeyDown(e); }
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.Handled) return;
			OnEditorKeyUp(e);
		}
		protected virtual void OnEditorKeyUp(KeyEventArgs e) { if(EditorContainer != null) EditorContainer.OnEditorKeyUp(e); }
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.Handled) return;
			OnEditorKeyPress(e);
		}
		protected virtual void OnEditorKeyPress(KeyPressEventArgs e) { if(EditorContainer != null) EditorContainer.OnEditorKeyPress(e); }
		public virtual void SendKey(KeyEventArgs e) {
			if(!EventSender.SendKeyDown(e))
				OnKeyDown(e);
		}
		public virtual void SendKeyUp(KeyEventArgs e) {
			OnKeyUp(e);
		}
		public virtual void SendKey(object message, KeyPressEventArgs e) {
			if(!EventSender.SendKeyChar(this, e, message))
				OnKeyPress(e);
		}
		object lastMouseDownClick = null;
		public virtual void SendMouse(Point position, MouseButtons button) {
			SendMouse(new MouseEventArgs(button, 1, position.X, position.Y, 0));
		}
		public virtual void SendMouseUp(Point position, MouseButtons button) {
			SendMouseUp(new MouseEventArgs(button, 1, position.X, position.Y, 0));
		}
		public virtual void SendMouse(MouseEventArgs e) {
			object ticks = DateTime.Now.Ticks;
			if(e.Button != MouseButtons.Left) ticks = null;
			if(!EventSender.SendMouseDown(this, e)) {
				OnMouseDown(e);
			}
			this.lastMouseDownClick = ticks;
		}
		internal bool IsDoubleClick() {
			if(this.lastMouseDownClick == null) return false;
			long timeStamp = DateTime.Now.Ticks;
			int delta = (int)((timeStamp - (long)this.lastMouseDownClick) / 10000); 
			this.lastMouseDownClick = null;
			return delta < SystemInformation.DoubleClickTime;
		}
		protected virtual void SendMouseUp(MouseEventArgs e) {
			this.lastMouseDownClick = null;
			OnMouseUp(e);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			this.lastMouseDownClick = null;
		}
		const int WM_LBUTTONDOWN = 0x0201, WM_LBUTTONDBLCLK = 0x0203;
		protected override void WndProc(ref Message m) {
			CheckDoubleClick(ref m);
			base.WndProc(ref m);
		}
		protected virtual void CheckDoubleClick(ref Message m) {
			if(m.Msg == WM_LBUTTONDBLCLK) this.lastMouseDownClick = null;
			if(m.Msg == WM_LBUTTONDOWN) {
				if(IsDoubleClick()) {
					m.Msg = WM_LBUTTONDBLCLK;
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual InplaceType InplaceType {
			get { return _inplaceType; }
			set {
				if(InplaceType == value) return;
				_inplaceType = value;
				this._allowValidate = InplaceType == InplaceType.Standalone;
			}
		}
		[Browsable(false)]
		public override bool IsLoading { get { return Properties.IsLoading; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsModified {
			get { return _isModified; }
			set {
				if(value)
					Properties.TriggerDelayedEditValueChanged();
				if(IsModified == value) return;
				_isModified = value;
				if(IsModified) {
					Properties.LockFormatParse = true;
					RaiseModified();
				}
				else {
					this.fOldEditValue = EditValue;
				}
			}
		}
		protected internal virtual void ResetEvents() {
			Events.Dispose();
		}
		[Browsable(false)]
		public virtual bool IsNeedFocus { get { return false; } }
		protected virtual void InitializeDefault() {
			this.fOldEditValue = null;
			this._allowValidate = true;
			this._isModified = false;
			this.fEditValue = null;
			this._viewInfo = Properties.CreateViewInfo();
			InitializeDefaultProperties();
			SetStyle(ControlStyles.FixedHeight, Properties.AutoHeight);
#if DXWhidbey
			SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
#endif
		}
		protected virtual void InitializeDefaultProperties() {
		}
		[Browsable(false)]
		public virtual object OldEditValue { get { return fOldEditValue; } }
		internal void SetOldEditValue(object value) { this.fOldEditValue = value; }
		protected internal override BaseControlPainter Painter { get { return Properties.Painter; } }
		protected internal override BaseControlViewInfo ViewInfo { get { return _viewInfo; } }
		protected internal virtual BaseEditViewInfo EditViewInfo { get { return ViewInfo as BaseEditViewInfo; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RepositoryItem Properties { get { return fProperties; } }
		[DefaultValue(false), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditReadOnly"),
#endif
 DXCategory(CategoryName.Behavior), Bindable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ReadOnly { get { return Properties.ReadOnly; } set { Properties.ReadOnly = value; } }
		protected virtual void CreateRepositoryItem() {
			this.fProperties = EditorClassInfo.CreateRepositoryItem();
			this.fProperties.SetOwnerEdit(this);
		}
		protected virtual void OnProperties_PropertiesChanged(object sender, EventArgs e) {
			if(IsDisposing) return;
			OnPropertiesChanged(true);
		}
		protected override void OnStyleControllerChanged() {
			if(InplaceType != InplaceType.Standalone) return;
			OnPropertiesChanged(true);
		}
		protected internal virtual void UpdateFixedHeight() {
			SetStyle(ControlStyles.FixedHeight, Properties.AutoHeight);
		}
		protected internal override void OnPropertiesChanged() {
			if(IsDisposing) return;
			if(ViewInfo != null) ((BaseEditViewInfo)ViewInfo).RefreshDisplayText = true;
			UpdateFixedHeight();
			base.OnPropertiesChanged();
			if(CheckAutoHeight()) LayoutChanged();
		}
		public new virtual bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		protected internal virtual void OnLoaded() {
			this.fOldEditValue = this.fEditValue;
			if(IsHandleCreated) {
				CheckEnabled();
				EnsureAnnotationAttributes();
				LayoutChanged();
				if(CheckAutoHeight()) LayoutChanged();
			}
		}
		protected virtual void ClearHotPressed() {
			ClearHotPressed(EditHitInfo.Empty);
		}
		protected virtual void ClearHotPressed(EditHitInfo newHotInfo) {
			ClearHotPressedCore(newHotInfo, false);
		}
		protected virtual void ClearHotPressedCore(EditHitInfo newHotInfo, bool focusLost) {
			EditViewInfo.PressedInfo = EditHitInfo.Empty;
			if(newHotInfo.HitTest != EditHitTest.Button) {
				EditViewInfo.HotInfo = EditHitInfo.Empty;
			}
			RefreshVisualLayoutCore(focusLost);
		}
		protected override bool IsLayoutLocked { get { return IsDisposing || _lockUpdate != 0 || IsLoading || _lockAutoHeight != 0; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditEditValue"),
#endif
		Bindable(true), Localizable(true), DXCategory(CategoryName.Data),
		RefreshProperties(RefreshProperties.All), DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public virtual object EditValue {
			get { return fEditValue; }
			set {
				value = Properties.DoParseEditValue(value).Value;
				FlushPendingEditActions();
				if(!IsBoundUpdatingEditValue) {
					if(CompareEditValue(value)) return;
				}
				OnEditValueChanging(new ChangingEventArgs(GetChangingOldEditValue(fEditValue), value));
			}
		}
		protected virtual object GetChangingOldEditValue(object value) { return value; } 
		bool CompareEditValue(object value) {
			if(EditValue == value) return true;
			return CompareEditValue(EditValue, value, false);
		}
		protected virtual bool CompareEditValue(object val1, object val2, bool parse) {
			if(parse) val2 = Properties.DoParseEditValue(val2).Value;
			if(val1 != null && val2 != null && val1.Equals(val2)) return true;
			if(val1 == val2) return true;
			return false;
		}
		[Browsable(false)]
		public virtual bool EditorContainsFocus { get { return ContainsFocus; } }
		[Browsable(false)]
		public virtual bool IsEditorActive { 
			get {
				if(!this.Enabled)
					return false;
				IContainerControl container = GetContainerControl();
				if(container == null) return EditorContainsFocus;
				return container.ActiveControl == this;
			} 
		}
		[Browsable(false)]
		public virtual string EditorTypeName { get { return "BaseEdit"; } }
		protected virtual EditorClassInfo EditorClassInfo { get { return EditorRegistrationInfo.Default.Editors[EditorTypeName]; } }
		protected virtual void OnEditValueChanging(ChangingEventArgs e) {
			if(IsLoading || IsBoundUpdatingEditValue) {
				bool valueEquals = CompareEditValue(e.NewValue);
				this.fOldEditValue = this.fEditValue = e.NewValue;
				IsModified = false;
				if(!valueEquals)
				OnEditValueChanged();
				return;
			}
			if(!IsMaskBoxUpdate) {
				Properties.RaiseEditValueChanging(e);
			}
			if(e.Cancel) {
				OnCancelEditValueChanging();
				return;
			}
			if(CompareEditValue(EditValue, e.NewValue, false)) return;
			if(!IsModified) this.fOldEditValue = GetChangingOldEditValue(EditValue);
			this.fEditValue = e.NewValue;
			if(IsEditorActive || IsAcceptingEditValue) IsModified = true;
			OnEditValueChanged();
			if(AutoSizeInLayoutControl) this.RaiseSizeableChanged();
		}
		protected virtual void OnCancelEditValueChanging() { }
		protected virtual bool IsMaskBoxUpdate { get { return false; } }
		protected virtual void OnEditValueChanged() {
			UpdateViewInfoEditValue();
			if(!IsMaskBoxUpdate) LayoutChanged();
			RaiseEditValueChanged();
		}
		protected internal void DestroyHandleCore() {
			DestroyHandle();
		}
		protected virtual void RaiseModified() {
			Properties.RaiseModified(EventArgs.Empty);
		}
		protected object lastChangedEditValue = null; 
		protected virtual void RaiseEditValueChanged() {
			if(!AllowFireEditValueChanged) return;
			this.lastChangedEditValue = EditValue;
			Properties.RaiseEditValueChanged(EventArgs.Empty);
		}
		public virtual bool IsNeededKey(KeyEventArgs e) {
			return Properties.IsNeededKey(e.KeyData);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingManagerBase BindingManager {
			get {
				if(DataBindings["EditValue"] == null) return null;
				return DataBindings["EditValue"].BindingManagerBase;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override UserLookAndFeel LookAndFeel { get { return Properties.LookAndFeel; } }
		bool ShouldSerializeBorderStyle() { return false; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditBorderStyle"),
#endif
 RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderStyles BorderStyle {
			get { return Properties.BorderStyle; }
			set { Properties.BorderStyle = value; }
		}
		protected internal virtual bool IsAllowValidate { get { return _allowValidate; } }
		public virtual bool DoValidate(PopupCloseMode closeMode) {
			return DoValidate();
		}
		public virtual bool DoValidate() {
			if(IsModified) {
				CancelEventArgs e = new CancelEventArgs();
				bool prevAllow = this.IsAllowValidate;
				try {
					this._allowValidate = true;
					OnValidating(e);
				}
				finally {
					this._allowValidate = prevAllow;
				}
				if(e.Cancel) return false;
				OnValidated(DoValidateEventArgs.Empty);
				return true;
			}
			return true;
		}
		protected internal virtual void OnAutoHeightChanged() {
			CheckAutoHeight();
		}
		protected virtual bool CheckAutoHeight() {
			if(IsDisposing || _lockUpdate != 0 || IsLoading || _lockAutoHeight != 0 || !Properties.AutoHeight) return false;
			if(PreferredHeight != Height) {
				this._lockAutoHeight++;
				try {
					Height = PreferredHeight;
				}
				finally {
					this._lockAutoHeight--;
				}
			}
			return false; 
		}
#if DXWhidbey
		public override Size GetPreferredSize(Size proposedSize) {
			Size res = base.GetPreferredSize(proposedSize);
			if(!IsDisposing && Properties.AutoHeight && !IsLoading) {
				res.Height = PreferredHeight;
			}
			return res;
		}
#endif
		int preferredHeight = -1;
		protected int PreferredHeight {
			get {
				if(preferredHeight == -1) {
					if(!Properties.AutoHeight) return -1;
					if(IsDisposing || _lockUpdate != 0 || IsLoading || _lockAutoHeight != 0 || !Properties.AutoHeight) return -1;
					preferredHeight = CalcPreferredHeight();
				}
				return preferredHeight;
			}
		}
		protected virtual int CalcPreferredHeight() {
			return EditViewInfo.CalcMinHeight(null);
		}
		protected virtual void DoBaseSetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			base.SetBoundsCore(x, y, width, height, specified);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(Properties.AutoHeight) {
				if(PreferredHeight != -1)
					height = PreferredHeight;
			}
			DoBaseSetBoundsCore(x, y, width, height, specified);
		}
		protected override void CreateLookAndFeel() {
		}
		public virtual int CalcMinHeight() {
			int res = DefaultSize.Height;
			if(ViewInfo == null) return res;
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				res = EditViewInfo.CalcMinHeight(ginfo.Graphics);
			}
			finally {
				ginfo.ReleaseGraphics();
			}
			return res;
		}
		public virtual bool AllowMouseClick(Control control, Point p) {
			if(control == this || this.Contains(control)) return true;
			return false;
		}
		protected override void OnEnabledChanged(EventArgs e) {
			RefreshEnabledState();
			base.OnEnabledChanged(e);
		}
		protected internal virtual void CheckEnabled() {
		}
		protected virtual void RefreshEnabledState() {
			LayoutChanged();
		}
		protected override bool GetValidationCanceled() {
			if(!base.GetValidationCanceled()) return false;
			return !IsUnvalidatedControlIsParent(this) || InplaceType == InplaceType.Standalone;
		}
		public override void ResetBackColor() { BackColor = Color.Empty; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditBackColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return GetColor(Properties.Appearance.BackColor, ViewInfo == null ? Color.Empty : ViewInfo.DefaultAppearance.BackColor, Properties.Appearance.Options.UseBackColor); }
			set { Properties.Appearance.BackColor = value; }
		}
		public override void ResetForeColor() { ForeColor = Color.Empty; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditForeColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return GetColor(Properties.Appearance.ForeColor, ViewInfo == null ? Color.Empty : ViewInfo.DefaultAppearance.ForeColor, Properties.Appearance.Options.UseForeColor); }
			set { Properties.Appearance.ForeColor = value; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditFont"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font {
			get { return Properties.Appearance.Font; }
			set { Properties.Appearance.Font = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ContextMenu ContextMenu {
			get { return base.ContextMenu; }
			set { base.ContextMenu = value; }
		}
#if DXWhidbey
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ContextMenuStrip ContextMenuStrip {
			get { return base.ContextMenuStrip; }
			set { base.ContextMenuStrip = value; }
		}
#endif
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditText"),
#endif
 RefreshProperties(RefreshProperties.All), Bindable(false), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return Properties.GetDisplayText(EditValue); }
			set { EditValue = value; }
		}
		protected override void OnValidating(CancelEventArgs e) {
			Properties.LockFormatParse = false;
			ParseEditorValue();
			Exception inner = null;
			if(InplaceType == InplaceType.Standalone)
				ErrorText = string.Empty;
			BaseEditValidatingEventArgs vArgs = new BaseEditValidatingEventArgs(this, e.Cancel);
			try {
				vArgs.TryValidateViaAnnotationAttributes(EditValue, Properties.AnnotationAttributes);
				OnValidatingCore(vArgs);
			}
			catch(Exception ex) {
				e.Cancel = true;
				ErrorText = ex.Message;
				inner = ex;
			}
			finally {
				e.Cancel = vArgs.Cancel;
				if(vArgs.Cancel && !string.IsNullOrEmpty(vArgs.ErrorText))
					ErrorText = vArgs.ErrorText;
			}
			if(e.Cancel && InplaceType == InplaceType.Standalone) {
				string exceptionText;
				if(ErrorText != null && ErrorText.Length > 0)
					exceptionText = ErrorText;
				else
					exceptionText = Localizer.Active.GetLocalizedString(StringId.InvalidValueText);
				e.Cancel = !OnInvalidValue(new WarningException(exceptionText, inner));
				if(e.Cancel && NeedLayoutUpdateValidating()) LayoutChanged(); 
			}
		}
		protected virtual bool CanValidateViaAnnotationAttributesCore() {
			return (InplaceType == InplaceType.Standalone) && IsDataBindingActive && IsBoundToEditValue && !Properties.Cloned;
		}
		protected internal virtual bool CanValidateValueViaAnnotationAttributes() {
			return CanValidateViaAnnotationAttributesCore();
		}
		protected internal virtual bool CanValidateObjectViaAnnotationAttributes() {
			return CanValidateViaAnnotationAttributesCore() && IsBindingDataAccessible(DataBindings["EditValue"]);
		}
		protected bool IsBindingDataAccessible(Binding binding) {
			return (binding.BindingManagerBase != null) && IsBindingManagerPositionValid(binding.BindingManagerBase);
		}
		protected bool IsBindingManagerPositionValid(BindingManagerBase bindingManager) {
			return bindingManager.Position >= 0 && bindingManager.Position < bindingManager.Count;
		}
		protected internal virtual object GetObjectToValidate() { 
			return DataBindings["EditValue"].BindingManagerBase.Current; 
		}
		protected virtual bool NeedLayoutUpdateValidating() { return false; }
		protected virtual void OnValidatingCore(CancelEventArgs e) {
			base.OnValidating(e);
		}
		protected internal virtual void FlushPendingEditActions() { }	
		protected virtual internal void CompleteChanges() { Properties.CompleteChanges(); }	
		protected virtual void ParseEditorValue() {
			if(!IsModified)
				return;
			CompleteChanges();
			ConvertEditValueEventArgs parsed = Properties.DoParseEditValue(EditValue);
			object val = ExtractParsedValue(parsed);
			bool prevLock = Properties.LockFormatParse;
			Properties.LockFormatParse = true;
			try {
				EditValue = val;
			}
			finally {
				Properties.LockFormatParse = prevLock;
			}
			CompleteChanges();
		}
		protected virtual object ExtractParsedValue(ConvertEditValueEventArgs e) {
			return e.Value;
		}
		protected override void OnValidated(EventArgs e) {
			base.OnValidated(e);
			if(IsAllowValidate) {
				this.IsModified = false;
			}
			OnEditorLeave(e);
		}
		protected override void OnLeave(EventArgs e) {
			base.OnLeave(e);
			if(!CausesValidation && InplaceType == InplaceType.Standalone) OnEditorLeave(e);
		}
		bool onEnterProcessing = false;
		protected override void OnEnter(EventArgs e) {
			if(this.onEnterProcessing) return;
			this.onEnterProcessing = true;
			try {
				base.OnEnter(e);
				if(InplaceType == InplaceType.Standalone)
				OnEditorEnter(e);
			}
			finally {
				this.onEnterProcessing = false;
			}
		}
		protected virtual void OnEditorLeave(EventArgs e) {
			Properties.LockFormatParse = false;
		}
		protected virtual void OnEditorEnter(EventArgs e) {
			Properties.LockFormatParse = false;
		}
		protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) {
			int x = bounds.X;
			Rectangle rect = base.GetScaledBounds(bounds, factor, specified);
			bool hasScaled = x != rect.X;
			if(GetStyle(ControlStyles.FixedHeight) && Properties.AutoHeight && (specified & BoundsSpecified.Height) == BoundsSpecified.Height && hasScaled)
				rect.Height = (int)Math.Round((double)bounds.Height * factor.Height);
			return rect;
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			CheckEnabled();
#if DXWhidbey
			if(Properties.AutoHeight && ((Anchor & (AnchorStyles.Bottom | AnchorStyles.Right)) == 0)) {
				BaseControl.ClearPreferredSizeCache(this);
				ClearPreferredHeight();
				CheckAutoHeight();
			}
#else
			CheckAutoHeight();
#endif
			EnsureAnnotationAttributes();
			LayoutChanged();
		}
		protected void EnsureAnnotationAttributes() {
			if(Properties.AnnotationAttributes != null || !IsDataBindingActive) return;
			if(CanValidateViaAnnotationAttributesCore()) {
				var binding = DataBindings["EditValue"];
				if(binding.BindingMemberInfo != null) {
					PropertyDescriptorCollection properties = null;
					if(binding.BindingManagerBase != null)
						properties = binding.BindingManagerBase.GetItemProperties();
					else {
						if(binding.DataSource != null) {
							BindingSource bs = binding.DataSource as BindingSource;
							if(bs != null && bs.CurrencyManager != null)
								properties = bs.CurrencyManager.GetItemProperties();
							else {
								Type dataSourceType = binding.DataSource.GetType();
								bool ignore = false;
								var elementType = DevExpress.Data.Helpers.ListDataControllerHelper.GetRowType(dataSourceType, out ignore);
								if(elementType != null)
									properties = TypeDescriptor.GetProperties(elementType);
							}
						}
					}
					if(properties != null) {
						var pd = properties[binding.BindingMemberInfo.BindingField];
						if(pd != null)
							Properties.AnnotationAttributes = new Data.Utils.AnnotationAttributes(pd, binding.DataSourceNullValue);
					}
				}
			}
		}
		protected virtual void ClearPreferredHeight() {
			this.preferredHeight = -1;
		}
		protected override void OnContextMenuChanged(EventArgs e) {
			Properties.ContextMenu = ContextMenu;
			base.OnContextMenuChanged(e);
		}
#if DXWhidbey
		protected override void OnContextMenuStripChanged(EventArgs e) {
			Properties.ContextMenuStrip = ContextMenuStrip;
			base.OnContextMenuStripChanged(e);
		}
#endif
		protected internal bool IsInputKeyCore(Keys keyData) { return IsInputKey(keyData); }
		protected override bool IsInputKey(Keys keyData) {
			bool res = base.IsInputKey(keyData) || Properties.NeededKeysContains(keyData) || (keyData == Keys.Enter && this.EnterMoveNextControl);
			return res;
		}
		protected override bool IsInputChar(char ch) {
			bool res = base.IsInputChar(ch) || Properties.IsNeededChar(ch);
			return res;
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			UpdateViewInfoState(true, e.Location);
			base.OnMouseMove(e);
		}
		protected virtual bool AllowPaintBackground {
			get {
				if(BackColor.A != 255) return true;
				return false;
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			if(!AllowPaintBackground) return;
			base.OnPaintBackground(e);
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			if(!EditorContainsFocus) {
				ClearHotPressedOnLostFocus();
			}
			RaiseSizeableChanged();
		}
		protected virtual void ClearHotPressedOnLostFocus() {
			ClearHotPressedCore(EditHitInfo.Empty, true);
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			RefreshVisualLayout();
			RaiseSizeableChanged();
		}
		protected override void CreateHandle() {
			try {
				base.CreateHandle();
			}
			catch {
				if(InplaceType != InplaceType.Grid) throw;
			}
		}
		protected virtual bool OnInvalidValue(Exception sourceException) {
			string errorText = sourceException.Message;
			InvalidValueExceptionEventArgs e = new InvalidValueExceptionEventArgs(errorText, sourceException, EditValue);
			RaiseInvalidValue(e);
			if(e.ExceptionMode == ExceptionMode.ThrowException) {
				throw e.Exception;
			}
			else if(e.ExceptionMode == ExceptionMode.DisplayError) {
				ErrorText = e.ErrorText;
			}
			else if(e.ExceptionMode == ExceptionMode.Ignore) {
				EditValue = OldEditValue;
				ErrorText = string.Empty;
				return true;
			}
			return false;
		}
		protected internal virtual void RaiseInvalidValue(InvalidValueExceptionEventArgs e) {
			InvalidValueExceptionEventHandler handler = (InvalidValueExceptionEventHandler)this.Events[invalidValue];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditInvalidValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event InvalidValueExceptionEventHandler InvalidValue {
			add { this.Events.AddHandler(invalidValue, value); }
			remove { this.Events.RemoveHandler(invalidValue, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditPropertiesChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler PropertiesChanged {
			add { Properties.PropertiesChanged += value; }
			remove { Properties.PropertiesChanged -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditEditValueChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler EditValueChanged {
			add { Properties.EditValueChanged += value; }
			remove { Properties.EditValueChanged -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditModified"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler Modified {
			add { Properties.Modified += value; }
			remove { Properties.Modified -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditEditValueChanging"),
#endif
 DXCategory(CategoryName.Events)]
		public event ChangingEventHandler EditValueChanging {
			add { Properties.EditValueChanging += value; }
			remove { Properties.EditValueChanging -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditParseEditValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event ConvertEditValueEventHandler ParseEditValue {
			add { Properties.ParseEditValue += value; }
			remove { Properties.ParseEditValue -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditFormatEditValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event ConvertEditValueEventHandler FormatEditValue {
			add { Properties.FormatEditValue += value; }
			remove { Properties.FormatEditValue -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditCustomDisplayText"),
#endif
 DXCategory(CategoryName.Events)]
		public event CustomDisplayTextEventHandler CustomDisplayText {
			add { Properties.CustomDisplayText += value; }
			remove { Properties.CustomDisplayText -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseEditQueryProcessKey"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryProcessKeyEventHandler QueryProcessKey {
			add { Properties.QueryProcessKey += value; }
			remove { Properties.QueryProcessKey -= value; }
		}
		#region Accessible patch
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return Properties.CreateAccessibleInstance();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp {
			add { }
			remove { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string AccessibleName {
			get { return Properties.AccessibleName; }
			set { Properties.AccessibleName = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new AccessibleRole AccessibleRole {
			get { return Properties.AccessibleRole; }
			set { Properties.AccessibleRole = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string AccessibleDefaultActionDescription {
			get { return Properties.AccessibleDefaultActionDescription; }
			set { Properties.AccessibleDefaultActionDescription = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string AccessibleDescription {
			get { return Properties.AccessibleDescription; }
			set { Properties.AccessibleDescription = value; }
		}
		internal void SetAccessibleName(string value) { base.AccessibleName = value; }
		internal void SetAccessibleRole(AccessibleRole value) { base.AccessibleRole = value; }
		internal void SetAccessibleDefaultActionDescription(string value) { base.AccessibleDefaultActionDescription = value; }
		internal void SetAccessibleDescription(string value) { base.AccessibleDescription = value; }
		#endregion
		[ DXCategory(CategoryName.Behavior), DefaultValue(false)]
		public virtual bool EnterMoveNextControl {
			get { return this.enterMoveNextControl; }
			set { this.enterMoveNextControl = value; }
		}
		public static bool StringStartsWidth(string source, string part) {
			return StringStartsWidth(source, part, false);
		}
		public static bool StringStartsWidth(string source, string part, bool ignoreCase) {
			if(part.Length > source.Length) return false;
			for(int n = 0; n < part.Length; n++) {
				char c1 = source[n], c2 = part[n];
				if(ignoreCase) {
					if(string.Equals(source.Substring(n, 1), part.Substring(n, 1), StringComparison.CurrentCultureIgnoreCase)) continue;
				}
				else {
					if(c1 == c2) continue;
				}
				if(Char.IsWhiteSpace(c1) && Char.IsWhiteSpace(c2)) continue;
				return false;
			}
			return true;
		}
		protected override bool SizeableIsCaptionVisible { get { return true; } }
		protected override Size CalcSizeableMaxSize() {
			if(Properties.AutoHeight) return new Size(0, CalcMinHeight());
			return Size.Empty;
		}
		protected internal bool IsNullValue(object editValue) {
			return Properties.IsNullValue(editValue);
		}
		protected internal virtual void OnAppearancePaintChanged() {
			ViewInfo.UpdateAppearances();
			Invalidate();
		}
		protected internal virtual void SetEmptyEditValue(object emptyEditValue) {
			this.fEditValue = emptyEditValue;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanShowDialog { get { return false; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler ForeColorChanged {
			add { base.ForeColorChanged += value; }
			remove { base.ForeColorChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler BackColorChanged {
			add { base.BackColorChanged += value; }
			remove { base.BackColorChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler FontChanged {
			add { base.FontChanged += value; }
			remove { base.FontChanged -= value; }
		}
		protected internal virtual bool SuppressMouseWheel(MouseEventArgs e) {
			return false;
		}
	}
}
namespace DevExpress.XtraEditors.Accessibility {
}
