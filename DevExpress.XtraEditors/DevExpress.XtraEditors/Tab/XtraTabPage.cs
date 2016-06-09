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
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.Accessibility.Tab;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
using System.Drawing.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraTab {
	public interface IXtraTabPage {
		IXtraTab TabControl { get; }
		void Invalidate();
		Image Image { get; }
		int ImageIndex { get; }
		Padding ImagePadding { get; }
		int TabPageWidth { get; }
		string Text { get; }
		string Tooltip { get; }
		string TooltipTitle { get; }
		ToolTipIconType TooltipIconType { get; }
		SuperToolTip SuperTip { get; }
		bool PageEnabled { get; }
		bool PageVisible { get; }
		PageAppearance Appearance { get; }
		DefaultBoolean ShowCloseButton { get; }
		DefaultBoolean AllowGlyphSkinning { get; }
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IXtraTabPageExt : IXtraTabPage {
		System.Drawing.Text.HotkeyPrefix HotkeyPrefixOverride { get; }
		int MaxTabPageWidth { get; }
		bool Pinned { get; set; }
		bool UsePinnedTab { get; }
		DefaultBoolean ShowPinButton { get; }
	}
	[Docking(DockingBehavior.Never)]
	[Designer("DevExpress.XtraTab.Design.XtraTabPageDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign), ToolboxItem(false)]
	[SmartTagSupport(typeof(XtraTabPageBounds), SmartTagSupportAttribute.SmartTagCreationMode.Auto), SmartTagFilter(typeof(XtraTabPageFilter))]
	public class XtraTabPage : XtraPanel, IXtraTabPageExt, IToolTipControlClient, IAnimatedItem, ISupportXtraAnimation {
		int imageIndex, tabPageWidth;
		bool pageEnabled, pageVisible;
		int maxTabPageWidthCore;
		string tooltip;
		string tooltipTitle;
		ToolTipIconType toolTipIconType = ToolTipIconType.None;
		SuperToolTip superTip;
		Image image;
		PageAppearance appearance;
		DefaultBoolean fShowCloseButtonCore;
		DefaultBoolean fAllowGlyphSkinning;
		Color lastBackColor = Color.Empty;
		protected XtraTabControl fTabControl;
		public XtraTabPage() {
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			this.tooltip = string.Empty;
			tooltipTitle = string.Empty;
			this.Size = Size.Empty;
			this.pageEnabled = this.pageVisible = true;
			this.tabPageWidth = 0;
			this.image = null;
			this.imageIndex = -1;
			this.fTabControl = null;
			this.appearance = new PageAppearance();
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.fShowCloseButtonCore = DefaultBoolean.Default;
			this.fAllowGlyphSkinning = DefaultBoolean.Default;
			this.imagePaddingCore = new Padding(0);
			base.Visible = false;
			this.maxTabPageWidthCore = 0;
			CheckFont();
		}
		Font GetFont() {
			AppearanceObject app = Appearance.PageClient.GetAppearanceByFont();
			if(app.Options.UseFont || DefaultAppearance.Font == null) return app.Font;
			return DefaultAppearance.Font;
		}
		void CheckFont() {
			if(!base.Font.Equals(GetFont())) {
				useBaseFont = true;
				base.Font = GetFont();
				useBaseFont = false;
			}
		}
		bool useBaseFont = false;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font {
			get {
				if(useBaseFont) return base.Font;
				return GetFont();
			}
			set { base.Font = value; }
		}
		Padding imagePaddingCore;
		bool ShouldSerializeImagePadding() { return ImagePadding != new Padding(0); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageImagePadding"),
#endif
 Category(CategoryName.Appearance)]
		public virtual Padding ImagePadding {
			get { return imagePaddingCore; }
			set {
				if(ImagePadding == value) return;
				imagePaddingCore = value;
				OnPageChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageTabPageWidth"),
#endif
 Category(CategoryName.Appearance)]
		[Localizable(true), DefaultValue(0), SmartTagProperty("Tab Page Width", "")]
		public virtual int TabPageWidth {
			get { return tabPageWidth; }
			set {
				if(value < 1) value = 0;
				if(TabPageWidth == value) return;
				tabPageWidth = value;
				OnPageChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageShowCloseButton"),
#endif
 Category(CategoryName.Behavior)]
		[DefaultValue(DefaultBoolean.Default), SmartTagProperty("Show Close Button", "")]
		public virtual DefaultBoolean ShowCloseButton {
			get { return fShowCloseButtonCore; }
			set {
				if(ShowCloseButton == value) return;
				fShowCloseButtonCore = value;
				OnPageChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageAllowGlyphSkinning"),
#endif
 Category(CategoryName.Behavior)]
		[DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return fAllowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value) return;
				fAllowGlyphSkinning = value;
				OnPageChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BorderStyle BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		bool IToolTipControlClient.ShowToolTips { get { return false; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point pt) { return null; }
		protected internal void SetCompatibleMode(bool useCompatibleDrawingMode) {
			SetStyle(ControlStyles.AllPaintingInWmPaint, !useCompatibleDrawingMode);
		}
		DevExpress.Accessibility.BaseAccessible dxAccessible;
		protected override DevExpress.Accessibility.BaseAccessible DXAccessible {
			get {
				if(dxAccessible == null) dxAccessible = CreateAccessibleInstanceTabPage();
				return dxAccessible;
			}
		}
		protected internal DevExpress.Accessibility.BaseAccessible DXAccessibleInternal { get { return DXAccessible; } }
		protected virtual DevExpress.Accessibility.BaseAccessible CreateAccessibleInstanceTabPage() {
			return new TabPageAccessible(this);
		}
		public virtual void AccessibleNotifyClients(AccessibleEvents accEvent, int childId) {
			AccessibilityNotifyClients(accEvent, childId);
		}
#if DXWhidbey
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Localizable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public override AutoSizeMode AutoSizeMode {
			get { return AutoSizeMode.GrowOnly; }
			set { }
		}
		protected override AccessibleObject GetAccessibilityObjectById(int childId) {
			if(childId == -1) return DXAccessible.Accessible;
			else if(childId >= 0 && childId < DXAccessible.ChildCount) return DXAccessible.Children[childId].Accessible;
			return base.GetAccessibilityObjectById(childId);
		}
#endif
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(TabControl == null) return new Control.ControlAccessibleObject(this);
			return DXAccessible.Accessible;
		}
		void ResetAppearance() { Appearance.Reset(); }
		protected override bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageAppearance"),
#endif
 Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PageAppearance Appearance { get { return appearance; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageBackColor"),
#endif
 Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return GetBackColor(Appearance.PageClient.BackColor, View == null || ViewInfo == null ? Color.Empty : ViewInfo.GetDefaultAppearance(TabPageAppearance.PageClient).BackColor); }
			set { Appearance.PageClient.BackColor = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageTooltip"),
#endif
 Category(CategoryName.ToolTip), DefaultValue(""), Localizable(true), Editor(ControlConstants.MultilineStringEditor, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string Tooltip {
			get { return tooltip; }
			set { tooltip = value; }
		}
		bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		void ResetSuperTip() { SuperTip = null; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageSuperTip"),
#endif
 Category(CategoryName.ToolTip), Localizable(true), Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageTooltipTitle"),
#endif
 Category(CategoryName.ToolTip), Localizable(true), DefaultValue("")]
		public string TooltipTitle {
			get { return tooltipTitle; }
			set { tooltipTitle = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageTooltipIconType"),
#endif
 Category(CategoryName.ToolTip), Localizable(true), DefaultValue(ToolTipIconType.None)]
		public ToolTipIconType TooltipIconType {
			get { return toolTipIconType; }
			set { toolTipIconType = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageForeColor"),
#endif
 Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return GetColor(Appearance.PageClient.ForeColor, View == null || ViewInfo == null ? Color.Empty : ViewInfo.GetDefaultAppearance(TabPageAppearance.PageClient).ForeColor); }
			set { Appearance.PageClient.ForeColor = value; }
		}
		Color GetBackColor(Color color, Color defColor) {
			if(ViewInfo != null && ViewInfo.IsAllowPageCustomBackColor) {
				if(color == Color.Empty) return defColor;
				return color;
			}
			return defColor == Color.Empty ? SystemColors.Control : defColor;
		}
		Color GetColor(Color color, Color defColor) {
			return color == Color.Empty ? defColor : color;
		}
		protected override void OnVisibleChanged(EventArgs e) {
			if(lastBackColor != BackColor) {
				this.lastBackColor = BackColor;
				OnBackColorChanged(EventArgs.Empty);
			}
			if(Visible && ViewInfo != null) {
				ViewInfo.SelectedTabPage = this;
			}
			base.OnVisibleChanged(e);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(TabControl != null) TabControl.TabPages.Remove(this);
				if(this.appearance != null) {
					this.appearance.Changed -= new EventHandler(OnAppearanceChanged);
					this.appearance.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			OnPageChanged();
			OnBackColorChanged(EventArgs.Empty);
		}
		[Browsable(false)]
		public virtual XtraTabControl TabControl { get { return fTabControl; } }
		internal void SetTabControl(XtraTabControl newOwner) {
			if(TabControl != null && newOwner != null) return;
			this.fTabControl = newOwner;
			UserLookAndFeel containerLookAndFeel = (TabControl != null) ? TabControl.LookAndFeel : null;
			if(HScrollBar != null)
				this.HScrollBar.LookAndFeel.ParentLookAndFeel = containerLookAndFeel;
			if(VScrollBar != null)
				this.VScrollBar.LookAndFeel.ParentLookAndFeel = containerLookAndFeel;
			UpdateImageHelper();
		}
		IXtraTab IXtraTabPage.TabControl { get { return TabControl; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageImage"),
#endif
 Category(CategoryName.Appearance), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)), SmartTagProperty("Image", "")]
		public virtual Image Image {
			get { return image; }
			set {
				if(Image == value) return;
				image = value;
				OnImageChanged();
			}
		}
		protected virtual void OnImageChanged() {
			UpdateImageHelper();
			OnPageChanged();
		}
		public virtual void StopAnimation() {
			XtraAnimator.RemoveObject(this);
		}
		public virtual void StartAnimation() {
			IAnimatedItem animItem = this;
			if(TabControl == null || DesignMode || animItem.FramesCount < 2) return;
			XtraAnimator.Current.AddEditorAnimation(null, this, animItem, new CustomAnimationInvoker(OnImageAnimation));
		}
		protected virtual void OnImageAnimation(BaseAnimationInfo animInfo) {
			IAnimatedItem animItem = this;
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			if(GetImage() == null || TabControl == null || info == null) return;
			if(!info.IsFinalFrame) {
				GetImage().SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
				TabControl.Invalidate(animItem.AnimationBounds);
			}
			else {
				StopAnimation();
				StartAnimation();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPagePageEnabled"),
#endif
 Category(CategoryName.Behavior), DefaultValue(true)]
		public virtual bool PageEnabled {
			get { return pageEnabled; }
			set {
				if(PageEnabled == value) return;
				pageEnabled = value;
				OnPageChanged();
			}
		}
		System.Drawing.Text.HotkeyPrefix IXtraTabPageExt.HotkeyPrefixOverride { get { return System.Drawing.Text.HotkeyPrefix.Show; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageMaxTabPageWidth"),
#endif
 Category(CategoryName.Appearance), DefaultValue(0)]
		public virtual int MaxTabPageWidth {
			get { return maxTabPageWidthCore; }
			set {
				if(MaxTabPageWidth == value) return;
				maxTabPageWidthCore = value;
				OnPageChanged();
			}
		}
		int IXtraTabPageExt.MaxTabPageWidth {
			get {
				if(MaxTabPageWidth == 0 && TabControl != null)
					return TabControl.MaxTabPageWidth;
				return MaxTabPageWidth;
			}
		}
		bool isPInnedCore;
		bool IXtraTabPageExt.Pinned {
			get { return isPInnedCore; }
			set {
				isPInnedCore = value;
				TabControl.LayoutChanged();
			}
		}
		bool IXtraTabPageExt.UsePinnedTab { get { return true; } }
		DefaultBoolean IXtraTabPageExt.ShowPinButton {
			get { return DefaultBoolean.Default; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPagePageVisible"),
#endif
 Category(CategoryName.Behavior), DefaultValue(true)]
		public virtual bool PageVisible {
			get { return pageVisible; }
			set {
				if(PageVisible == value) return;
				pageVisible = value;
				OnPageChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageImageIndex"),
#endif
 Category(CategoryName.Appearance),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		ImageList("Images"),
		DefaultValue(-1)]
		public virtual int ImageIndex {
			get { return imageIndex; }
			set {
				if(value < 0) value = -1;
				if(ImageIndex == value) return;
				imageIndex = value;
				OnImageIndexChanged();
			}
		}
		protected internal virtual void UpdateImageHelper() {
			StopAnimation();
			ImageHelper.Image = GetImage();
			StartAnimation();
		}
		protected virtual Image GetImage() {
			if(Image != null) return Image;
			return ImageCollection.GetImageListImage(Images, ImageIndex);
		}
		protected virtual void OnImageIndexChanged() {
			UpdateImageHelper();
			OnPageChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Images {
			get { return TabControl == null ? null : TabControl.Images; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool Visible {
			get { return base.Visible; }
			set {
				base.Visible = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = DockStyle.None; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new int TabIndex {
			get { return base.TabIndex; }
			set { base.TabIndex = 0; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = true; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override AnchorStyles Anchor {
			get { return base.Anchor; }
			set { base.Anchor = AnchorStyles.None; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Point Location {
			get { return base.Location; }
			set { base.Location = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabPageText"),
#endif
 Category(CategoryName.Appearance), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), SmartTagProperty("Text", "")]
		public override string Text {
			get { return base.Text; }
			set {
				if(Text == value) return;
				base.Text = value;
				OnPageChanged();
			}
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
		}
		protected virtual void OnPageChanged() {
			if(TabControl != null)
				(TabControl as IXtraTab).OnPageChanged(this);
		}
		protected BaseTabPageViewInfo PageViewInfo {
			get {
				if(ViewInfo == null) return null;
				return ViewInfo.HeaderInfo.AllPages[this];
			}
		}
		protected BaseViewInfoRegistrator View { get { return TabControl == null ? null : TabControl.View; } }
		protected BaseTabControlViewInfo ViewInfo { get { return TabControl == null ? null : TabControl.ViewInfo; } }
		protected override void OnPaint(PaintEventArgs e) {
			BaseTabPageViewInfo pageInfo = PageViewInfo;
			if(pageInfo != null) {
				using(GraphicsCache cache = new GraphicsCache(e)) {
					TabDrawArgs args = new TabDrawArgs(cache, ViewInfo,
#if DXWhidbey
 Inflate(DisplayRectangle, Padding));
#else
						Inflate(DisplayRectangle, DockPadding));
#endif
					TabControl.Painter.DrawPageClientControl(args, pageInfo);
				}
			}
			else {
				using(Brush brush = new SolidBrush(BackColor)) {
					e.Graphics.FillRectangle(brush, ClientRectangle);
				}
			}
			if(TabControl != null && (!TabControl.Enabled && TabControl.UseDisabledStatePainter)) {
				UserLookAndFeel lf = View is SkinViewInfoRegistrator ? TabControlLookAndFeel : null;
				BackgroundPaintHelper.PaintDisabledControl(lf, e, ClientRectangle);
			}
			RaisePaintEvent(this, e);
			e.Graphics.ExcludeClip(ClientRectangle);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override UserLookAndFeel LookAndFeel { get { return base.LookAndFeel; } }
		protected UserLookAndFeel TabControlLookAndFeel { get { return ViewInfo != null && ViewInfo.TabControl != null ? ViewInfo.TabControl.LookAndFeel : null; } }
#if DXWhidbey
		Rectangle Inflate(Rectangle rect, Padding padding) {
			rect.X -= padding.Left;
			rect.Y -= padding.Top;
			rect.Width += padding.Horizontal;
			rect.Height += padding.Vertical;
			return rect;
		}
#else
		Rectangle Inflate(Rectangle rect, DockPaddingEdges padding) {
			rect.X -= padding.Left;
			rect.Y -= padding.Top;
			rect.Width += padding.Left + padding.Right;
			rect.Height += padding.Top + padding.Bottom;
			return rect;
		}
#endif
		protected override void OnPaintBackground(PaintEventArgs e) {
			if(!GetStyle(ControlStyles.AllPaintingInWmPaint)) OnPaint(e);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(TabControl != null && TabControl.IsHandleCreated) {
				Rectangle r = TabControl.DisplayRectangle;
				base.SetBoundsCore(r.X, r.Y, r.Width, r.Height, BoundsSpecified.All);
			}
			else {
				base.SetBoundsCore(x, y, width, height, specified);
			}
		}
		internal bool DoValidate() {
			if(!CausesValidation) return true;
			bool res = true;
			try {
				CancelEventArgs e = new CancelEventArgs(false);
				OnValidating(e);
				if(e.Cancel) return false;
				ContainerControl container = GetContainerControl() as ContainerControl;
				if(container == null) return true;
				if(ContainsFocus) {
					if(container.ActiveControl != this) {
						res = container.Validate();
					}
				}
			}
			catch {
				return false;
			}
			if(!res) return false;
			OnValidated(EventArgs.Empty);
			return true;
		}
		AnimatedImageHelper imageHelper;
		protected AnimatedImageHelper ImageHelper {
			get {
				if(imageHelper == null)
					imageHelper = new AnimatedImageHelper(Image);
				return imageHelper;
			}
		}
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);
			XtraEditors.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		#region IAnimatedItem Members
		Rectangle IAnimatedItem.AnimationBounds {
			get { return ViewInfo.HeaderInfo.Bounds; }
		}
		int IAnimatedItem.AnimationInterval { get { return ImageHelper.AnimationInterval; } }
		int[] IAnimatedItem.AnimationIntervals { get { return ImageHelper.AnimationIntervals; } }
		AnimationType IAnimatedItem.AnimationType { get { return ImageHelper.AnimationType; } }
		int IAnimatedItem.FramesCount { get { return ImageHelper.FramesCount; } }
		int IAnimatedItem.GetAnimationInterval(int frameIndex) {
			return ImageHelper.GetAnimationInterval(frameIndex);
		}
		bool IAnimatedItem.IsAnimated {
			get { return ImageHelper.IsAnimated; }
		}
		void IAnimatedItem.OnStart() { }
		void IAnimatedItem.OnStop() { }
		object IAnimatedItem.Owner {
			get { return TabControl; }
		}
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) {
			ImageHelper.UpdateAnimation(info);
		}
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return ImageHelper.FramesCount > 1; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return TabControl; }
		}
		#endregion
	}
}
