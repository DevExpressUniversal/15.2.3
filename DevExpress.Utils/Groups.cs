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
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Controls;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.ButtonsPanelControl;
using DevExpress.XtraBars.Docking2010;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraEditors { 
	[Designer("DevExpress.XtraEditors.Design.GroupDesigner, " + AssemblyInfo.SRAssemblyDesignFull), DXToolboxItem(DXToolboxItemKind.Free), Docking(DockingBehavior.Ask),
	 Description("Groups a collection of controls."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	  ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "PanelControl"),
	  SmartTagAction(typeof(PanelControlActions), "ContentImage", "Edit Content Image", SmartTagActionType.CloseAfterExecute),
	  SmartTagFilter(typeof(PanelControlFilter))
	]
	public class PanelControl : PanelBase, ISupportInitialize, ISupportLookAndFeel, IPanelControlOwner {
		GroupObjectInfoArgs viewInfo;
		GroupObjectPainter painter;
		UserLookAndFeel lookAndFeel;
		BorderStyles borderStyle;
		AppearanceObject appearance;
		Image contentImage;
		ContentAlignment contentImageAlignment;
		bool useCompatibleDrawingMode = false;
		public PanelControl() {
			SetStyle(ControlStyles.ResizeRedraw | ControlConstants.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.borderStyle = DefaultBorderStyle;
			this.lookAndFeel = new ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
			this.appearance = CreateAppearance();
			this.contentImage = null;
			this.contentImageAlignment = ContentAlignment.MiddleCenter;
			CreateInfo();
			InitElements();
			CheckFont();
		}
		protected override void InitElements() {
			if(LookAndFeel != null)
				base.InitElements();
		}
		protected override void Dispose(bool disposing) {
			if(disposing && this.lookAndFeel != null) {
				lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
				LookAndFeel.Dispose();
				DestroyAppearance(Appearance);
			}
			base.Dispose(disposing);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public bool UseCompatibleDrawingMode {
			get { return useCompatibleDrawingMode; }
			set {
				if(UseCompatibleDrawingMode == value) return;
				useCompatibleDrawingMode = value;
				OnCompatibleDrawingModeChanged();
			}
		}
		protected virtual void OnCompatibleDrawingModeChanged() {
			SetStyle(ControlStyles.AllPaintingInWmPaint, !UseCompatibleDrawingMode);
			if(IsHandleCreated) Refresh();
		}
		protected override void OnForeColorChanged(EventArgs e) {
			ClearDefaultAppearances();
			base.OnForeColorChanged(e);
		}
		protected override void OnBackColorChanged(EventArgs e) {
			ClearDefaultAppearances();
			base.OnBackColorChanged(e);
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		protected override void OnParentChanged(EventArgs e) {
			ClearDefaultAppearances();
			base.OnParentChanged(e);
			CheckParentColors();
		}
		protected void CheckParentColors() {
			if(ViewInfo != null && ViewInfo.LastBackColor != BackColor) {
				OnChanged();
				OnBackColorChanged(EventArgs.Empty);
			}
		}
		protected virtual int GetPaddingTop() {
			if(BorderStyle == BorderStyles.NoBorder) return Padding.Top;
			return (ViewInfo.ControlClientBounds.Y - ViewInfo.Bounds.Y) + Padding.Top;
		}
		protected virtual int GetPaddingLeft() {
			if(BorderStyle == BorderStyles.NoBorder) return Padding.Left;
			return (ViewInfo.ControlClientBounds.X - ViewInfo.Bounds.X) + Padding.Left;
		}
		protected virtual int GetPaddingRight() {
			if(BorderStyle == BorderStyles.NoBorder) return Padding.Right;
			return (ViewInfo.Bounds.Right - ViewInfo.ControlClientBounds.Right) + Padding.Right;
		}
		protected virtual int GetPaddingBottom() {
			if(BorderStyle == BorderStyles.NoBorder) return Padding.Bottom;
			return (ViewInfo.Bounds.Bottom - ViewInfo.ControlClientBounds.Bottom) + Padding.Bottom;
		}
		protected virtual int GetPaddingHorizontal() { return GetPaddingLeft() + GetPaddingRight(); }
		protected virtual int GetPaddingVertical() { return GetPaddingTop() + GetPaddingBottom(); }
		bool IsSideDock {
			get { return Dock != DockStyle.None && Dock != DockStyle.Fill; }
		}
		protected virtual bool IsChildDock {
			get {
				foreach(Control ctrl in Controls) {
					if(ctrl.Dock == DockStyle.None) return false;
				}
				return true;
			}
		}
		protected virtual int GetCaptionHorizontal() { return 0; }
		protected virtual int GetCaptionVertical() { return 0; }
		protected virtual Size UpdateByCaption(Size sz) { return sz; }
		protected override Size SizeFromClientSize(Size clientSize) {
			Size sz = base.SizeFromClientSize(clientSize);
			sz.Width -= Padding.Horizontal;
			sz.Height -= Padding.Vertical;
			UpdateViewInfoProperties(ViewInfo);
			Painter.CalcObjectBounds(ViewInfo);
			sz.Width = sz.Width + GetPaddingHorizontal();
			sz.Height = sz.Height + GetPaddingVertical();
			if(!(Parent is FlowLayoutPanel && AutoSize)) { 
				sz.Width += GetCaptionVertical();
				sz.Height += GetCaptionHorizontal();
			}
			return UpdateByCaption(sz);
		}
		protected bool useViewInfoBoundsAsDisplayRectangle = true;
#if !SL
	[DevExpressUtilsLocalizedDescription("PanelControlDisplayRectangle")]
#endif
		public override Rectangle DisplayRectangle {
			get {
				if(!useViewInfoBoundsAsDisplayRectangle) return base.DisplayRectangle;
				CheckInfo(false);
				Rectangle rect = ViewInfo.Bounds;
				rect.Offset(GetPaddingLeft(), GetPaddingTop());
				rect.Width -= GetPaddingHorizontal();
				rect.Height -= GetPaddingVertical();
				return rect;
			}
		}
		protected virtual void ClearDefaultAppearances() {
			this.defaultAppearance = null;
		}
		protected override void OnParentBackColorChanged(EventArgs e) {
			base.OnParentBackColorChanged(e);
			ClearDefaultAppearances();
			CheckParentColors();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(IsLoading) return;
			if(this.loaded) return;
			OnLoaded();
		}
		bool loaded = false;
		int lockInitialize = 0;
		protected bool IsLoaded { get { return this.loaded; } }
		protected internal virtual bool IsLoading { get { return lockInitialize != 0; } }
		public virtual void BeginInit() {
			this.lockInitialize++;
		}
		public virtual void EndInit() {
			if(--this.lockInitialize == 0) {
				OnLoaded();
			}
		}
		protected virtual void OnLoaded() {
			this.loaded = true;
			OnPropertiesChanged();
		}
		protected AppearanceObject CreateAppearance() {
			AppearanceObject res = new AppearanceObject();
			res.Changed += new EventHandler(OnAppearanceChanged);
			return res;
		}
		protected void DestroyAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			appearance.Changed -= new EventHandler(OnAppearanceChanged);
			appearance.Dispose();
		}
		void CheckFont() {
			if(!base.Font.Equals(GetFont())) {
				useBaseFont = true;
				base.Font = GetFont();
				useBaseFont = false;
			}
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnChanged();
			if(!IsLoading) OnBackColorChanged(e);
			CheckFont();
			PerformLayout();
		}
		[DefaultValue((string)null), Category("Appearance"), 
#if !SL
	DevExpressUtilsLocalizedDescription("PanelControlContentImage"),
#endif
 Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesignFull, typeof(UITypeEditor))]
		public Image ContentImage {
			get { return contentImage; }
			set { contentImage = value; OnChanged(); }
		}
		[Obsolete("Use the ContentImageAlignment property instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ContentAlignment ContentImageAlignement {
			get { return contentImageAlignment; }
			set { contentImageAlignment = value; OnChanged(); }
		}
		[DefaultValue(ContentAlignment.MiddleCenter), Category("Appearance"), 
#if !SL
	DevExpressUtilsLocalizedDescription("PanelControlContentImageAlignment")
#else
	Description("")
#endif
]
		public ContentAlignment ContentImageAlignment {
			get { return contentImageAlignment; }
			set { contentImageAlignment = value; OnChanged(); }
		}
		protected virtual BorderStyles DefaultBorderStyle { get { return BorderStyles.Default; } }
		protected bool ShouldSerializeBorderStyle() { return BorderStyle != DefaultBorderStyle; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("PanelControlBorderStyle"),
#endif
 Category("Appearance")]
		public new BorderStyles BorderStyle {
			get {
				return borderStyle;
			}
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				OnPropertiesChanged();
			}
		}
		void ResetAppearance() { Appearance.Reset(); }
		protected override bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("PanelControlAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override AppearanceObject Appearance { get { return appearance; } }
		protected override bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("PanelControlLookAndFeel"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		public Rectangle CalcBoundsByClient(Graphics graphics, Rectangle bounds) {
			Rectangle res = Painter.CalcBoundsByClientRectangle(ViewInfo, bounds);
			res.X -= DockPadding.Left; res.Width += DockPadding.Right + DockPadding.Left;
			res.Y -= DockPadding.Top; res.Height += DockPadding.Bottom + DockPadding.Top;
			return res;
		}
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
			OnBackColorChanged(EventArgs.Empty);
			if(!Size.IsEmpty) {
				InitLayout();
				UpdateChildrenLayout();
			}
		}
		void UpdateChildrenLayout() {
			for(int n = 0; n < Controls.Count; n++) Controls[n].LayoutEngine.InitLayout(Controls[n], BoundsSpecified.All);
		}
		protected virtual GroupObjectInfoArgs CreateViewInfoInstance() {
			return new GroupObjectInfoArgs();
		}
		protected virtual GroupObjectInfoArgs CreateViewInfo() {
			GroupObjectInfoArgs res = CreateViewInfoInstance();
			res.SetAppearance(new AppearanceObject());
			res.AppearanceCaption = new AppearanceObject();
			return res;
		}
		protected override ObjectPainter CreatePainter() {
			switch(LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Office2003: return new GroupObjectPainter(this);
				case ActiveLookAndFeelStyle.WindowsXP: return new WindowsXPGroupObjectPainter(this);
				case ActiveLookAndFeelStyle.Skin: return new SkinGroupObjectPainter(this, LookAndFeel.ActiveLookAndFeel);
			}
			return new FlatGroupObjectPainter(this);
		}
		protected virtual void CreateInfo() {
			this.viewInfo = CreateViewInfo();
			this.painter = CreatePainter() as GroupObjectPainter;
		}
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			OnChanged();
		}
		protected bool Is64BitOs { get { return IntPtr.Size == 8; } }
		protected virtual void CheckInfoOnResize() {
			CheckInfo(false);
		}
		protected override void OnResize(EventArgs e) {
			if(!IsLoading) {
				CheckInfoOnResize();
			}
			base.OnResize(e);
		}
		protected virtual void OnPropertiesChanged() {
			ClearDefaultAppearances();
			if(IsLoading) return;
			CreateInfo();
			OnChanged();
		}
		protected virtual void OnChanged() {
			if(IsLoading) return;
			ViewInfo.Bounds = Rectangle.Empty;
			CheckInfo(true);
			Invalidate();
		}
		protected GroupObjectInfoArgs ViewInfo { get { return viewInfo; } }
		protected internal new GroupObjectPainter Painter { get { return painter; } }
		protected virtual void CheckInfo(bool performLayout) {
			if(ViewInfo.Bounds == GetClientBoundsCore() && ViewInfo.IsReady) return;
			CheckInfoCore(ViewInfo, performLayout);
		}
		protected virtual bool CanShowCaption { get { return false; } }
		protected virtual void CheckInfoCore(GroupObjectInfoArgs info, bool performLayout) {
			Rectangle prevDisplay = GetDisplayRectangleCore();
			UpdateViewInfoProperties(info);
			CheckUpdateAppearance(info);
			info.DrawUserBackground = Appearance.Options.UseBackColor;
			info.ContentImage = ContentImage;
			info.ContentImageAlignment = ContentImageAlignment;
			info.LastBackColor = BackColor;
			Painter.CalcObjectBounds(info);
			if(prevDisplay != GetDisplayRectangleCore() && performLayout) {
				OnGroupLayout();
			}
		}
		protected virtual void CheckUpdateAppearance(GroupObjectInfoArgs info) {
			AppearanceHelper.Combine(info.Appearance, new AppearanceObject[] { Appearance }, DefaultAppearance);
			if(!GetAllowTransparency()) {
				info.Appearance.BackColor = AppearanceHelper.RemoveTransparency(info.Appearance.BackColor, AppearanceHelper.RemoveTransparency(DefaultAppearance.BackColor, SystemColors.Control));
				info.Appearance.BackColor2 = AppearanceHelper.RemoveTransparency(info.Appearance.BackColor2, AppearanceHelper.RemoveTransparency(DefaultAppearance.BackColor2, SystemColors.Control));
			}
		}
		protected virtual void OnGroupLayout() {
		}
		bool prevRightToLeft = false;
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			CheckRightToLeft();
		}
		void CheckRightToLeft() {
			if(prevRightToLeft == IsRightToLeft) return;
			prevRightToLeft = this.IsRightToLeft;
			OnRightToLeftChanged();
		}
		protected virtual void OnRightToLeftChanged() {
			ViewInfo.IsReady = false;
			CheckInfo(false);
		}
		protected bool IsRightToLeft { get { return WindowsFormsSettings.GetIsRightToLeft(this); } }
		protected virtual void UpdateViewInfoProperties(GroupObjectInfoArgs info) {
			info.BorderStyle = BorderStyle;
			info.RightToLeft = IsRightToLeft;
			info.ShowCaption = CanShowCaption;
			info.Bounds = GetClientBoundsCore();
			info.Caption = Text;
			info.AutoSize = AutoSize;
		}
		protected Rectangle GetClientBoundsCore() { return new Rectangle(Point.Empty, Bounds.Size); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("PanelControlText"),
#endif
 Category("Appearance"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		Rectangle GetDisplayRectangleCore() {
			Rectangle res = ViewInfo.ControlClientBounds; ;
			res.X += DockPadding.Left; res.Width -= DockPadding.Right + DockPadding.Left;
			res.Y += DockPadding.Top; res.Height -= DockPadding.Bottom + DockPadding.Top;
			return res;
		}
		void UpdateSupportTransparency() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
		internal bool GetAllowTransparency() {
			return AllowTotalTransparency && BackColor.A < 255 &&
				(BorderStyle == BorderStyles.NoBorder || Painter is WindowsXPGroupObjectPainter);
		}
		bool useDisabledStatePainterCore = true;
		[DefaultValue(true), 
#if !SL
	DevExpressUtilsLocalizedDescription("PanelControlUseDisabledStatePainter"),
#endif
 Category("Appearance")]
		public bool UseDisabledStatePainter {
			get { return useDisabledStatePainterCore; }
			set {
				if(useDisabledStatePainterCore != value) {
					useDisabledStatePainterCore = value;
					Invalidate();
				}
			}
		}
		bool firstPaint = true;
		protected virtual bool AllowTotalTransparency { get { return true; } }
		protected override void OnPaint(PaintEventArgs e) {
			if(this.firstPaint) {
				this.firstPaint = false;
				CheckParentColors();
			}
			CheckInfo(false);
			UpdateSupportTransparency();
			using(GraphicsCache cache = new GraphicsCache(e)) {
				if(!AllowTotalTransparency || BorderStyle != BorderStyles.NoBorder || BackColor != Color.Transparent) {
					DrawPanel(cache, ViewInfo);
					if(!Enabled && UseDisabledStatePainter) BackgroundPaintHelper.PaintDisabledControl(LookAndFeel, cache, ClientRectangle);
				}
				DrawPanelElements(cache, ViewInfo);
			}
			RaisePaintEvent(null, e);
		}
		protected virtual void DrawPanel(GraphicsCache cache, GroupObjectInfoArgs info) {
			ObjectPainter.DrawObject(cache, Painter, info);
		}
		protected virtual void DrawPanelElements(GraphicsCache cache, GroupObjectInfoArgs info) { }
		protected override void OnPaintBackground(PaintEventArgs e) {
			UpdateSupportTransparency();
			if(!GetStyle(ControlStyles.AllPaintingInWmPaint)) OnPaint(e);
			base.OnPaintBackground(e);
		}
		#region Appearance wrappers
		public override void ResetBackColor() { BackColor = Color.Empty; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("PanelControlBackColor"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get {
				return GetColor(Appearance.BackColor, DefaultAppearance.BackColor);
			}
			set { Appearance.BackColor = value; }
		}
		public override void ResetForeColor() { ForeColor = Color.Empty; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("PanelControlForeColor"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public override Color ForeColor {
			get { return LookAndFeelHelper.CheckTransparentForeColor(LookAndFeel, GetColor(Appearance.ForeColor, DefaultAppearance.ForeColor), this); }
			set { Appearance.ForeColor = value; }
		}
		Font GetFont() {
			AppearanceObject app = Appearance.GetAppearanceByOption(AppearanceObject.optUseFont);
			if(app.Options.UseFont || DefaultAppearance.Font == null) return app.Font;
			return DefaultAppearance.Font;
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
		protected Color GetColor(Color color, Color defaultColor) {
			return color == Color.Empty ? defaultColor : color;
		}
		AppearanceDefault defaultAppearance = null;
		protected override AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault res;
			if(Painter != null)
				res = Painter.DefaultAppearance.Clone() as AppearanceDefault;
			else
				res = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = CommonSkins.GetSkin(LookAndFeel)[IsDrawNoBorder ? CommonSkins.SkinGroupPanelNoBorder : CommonSkins.SkinGroupPanel];
				element.Apply(res, LookAndFeel);
			}
			res = LookAndFeelHelper.CheckColors(LookAndFeel, res, this);
			return res;
		}
		protected virtual bool IsDrawNoBorder { get { return BorderStyle == BorderStyles.NoBorder && !CanShowCaption; } }
		#endregion
		void IPanelControlOwner.OnCustomDrawCaption(GroupCaptionCustomDrawEventArgs e) {
			RaiseCustomDrawCaption(e);
		}
		Color IPanelControlOwner.GetForeColor() { return ForeColor; }
		protected virtual void RaiseCustomDrawCaption(GroupCaptionCustomDrawEventArgs e) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AutoScroll {
			get { return base.AutoScroll; }
			set { base.AutoScroll = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size AutoScrollMargin {
			get { return base.AutoScrollMargin; }
			set { base.AutoScrollMargin = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size AutoScrollMinSize {
			get { return base.AutoScrollMinSize; }
			set { base.AutoScrollMinSize = value; }
		}
	}
	[Designer("DevExpress.XtraEditors.Design.GroupDesigner, " + AssemblyInfo.SRAssemblyDesignFull),
	 Description("Combines controls into a group and allows a caption to be displayed along any group edge."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	 ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "GroupControl"),
	 SmartTagAction(typeof(GroupControlActions), "CaptionImage", "Edit Caption Image", SmartTagActionType.CloseAfterExecute),
	 SmartTagAction(typeof(GroupControlActions), "CaptionImageUri", "Edit Caption ImageUri", SmartTagActionType.CloseAfterExecute),
	 SmartTagAction(typeof(GroupControlActions), "AddCustomHeaderButtons", "Add Custom Header Buttons", SmartTagActionType.CloseAfterExecute),
	 DXToolboxItem(DXToolboxItemKind.Free)
	]
	public class GroupControl : PanelControl, IGroupBoxButtonsPanelOwner, IButtonPanelControlAppearanceOwner, IToolTipControlClient {
		public event GroupCaptionCustomDrawEventHandler CustomDrawCaption;
		static Padding DefaultCaptionImagePadding = new Padding(0);
		ToolTipController toolTipControllerCore;
		Locations captionLocation = Locations.Default;
		bool showCaption;
		AppearanceObject appearanceCaption;
		Image captionImage;
		Padding captionImagePadding = DefaultCaptionImagePadding;
		GroupElementLocation captionImageLocation = GroupElementLocation.Default;
		BaseButtonCollection customHeaderButtonsCore;
		public GroupControl() {
			this.appearanceCaption = CreateAppearance();
			this.showCaption = DefaultShowCaption;
			this.customHeaderButtonsCore = CreateButtonCollection();
			this.CustomHeaderButtons.CollectionChanged += OnButtonCollectionChanged;
			useViewInfoBoundsAsDisplayRectangle = true;
		}
		protected virtual BaseButtonsPanel ButtonsPanel {
			get { return ViewInfo != null ? ViewInfo.ButtonsPanel : null; }
		}
		protected virtual bool HorizontalDocking { get { return Dock == DockStyle.Left || Dock == DockStyle.Right || Dock == DockStyle.Fill; } }
		protected virtual bool VerticalDocking { get { return Dock == DockStyle.Bottom || Dock == DockStyle.Top || Dock == DockStyle.Fill; } }
		protected virtual bool HorizontalCaptionLocation { get { return CaptionLocation == Locations.Default || CaptionLocation == Locations.Top || CaptionLocation == Locations.Bottom; } }
		protected override bool ShouldAddBorderSize(Size proposedSize) {
			if(proposedSize.Width != int.MaxValue || proposedSize.Height != int.MaxValue)
				return true;
			for(int i = 0; i < Controls.Count; i++) {
				if(Controls[i].Dock == DockStyle.None)
					return false;
			}
			return true;
		}
		protected override GroupObjectInfoArgs CreateViewInfo() {
			if(ViewInfo != null)
				ViewInfo.Dispose();
			GroupObjectInfoArgs viewInfo = base.CreateViewInfo();
			viewInfo.SetButtonsPanelOwner(this);
			return viewInfo;
		}
		protected virtual BaseButtonCollection CreateButtonCollection() {
			return new BaseButtonCollection(null);
		}
		public virtual void LayoutChanged() {
			OnChanged();
		}
		protected override void ClearDefaultAppearances() {
			base.ClearDefaultAppearances();
			this.defaultAppearanceCaption = null;
		}
		protected override void OnRightToLeftChanged() {
			if(ButtonsPanel != null)
				ButtonsPanel.RightToLeft = IsRightToLeft;
			base.OnRightToLeftChanged();
		}
		AppearanceDefault defaultAppearanceCaption = null;
		protected virtual AppearanceDefault DefaultAppearanceCaption {
			get {
				if(defaultAppearanceCaption == null)
					defaultAppearanceCaption = CreateDefaultAppearanceCaption();
				return defaultAppearanceCaption;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceCaption() {
			AppearanceDefault res;
			if(Painter != null)
				res = Painter.DefaultAppearanceCaption.Clone() as AppearanceDefault;
			else
				res = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			res = LookAndFeelHelper.CheckColors(LookAndFeel, res, this);
			return res;
		}
		protected override void CheckUpdateAppearance(GroupObjectInfoArgs info) {
			base.CheckUpdateAppearance(info);
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				AppearanceHelper.Combine(info.AppearanceCaption, new AppearanceObject[] { AppearanceCaption }, DefaultAppearanceCaption);
			if(info.AppearanceCaption.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				info.AppearanceCaption.TextOptions.HotkeyPrefix = HKeyPrefix.Show;
		}
		bool trackingMouseLeave = false;
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(ButtonsPanel != null) {
				ButtonsPanel.Handler.OnMouseMove(e);
			}
			if(!trackingMouseLeave) {
				NativeMethods.TRACKMOUSEEVENTStruct msevnt = new NativeMethods.TRACKMOUSEEVENTStruct();
				msevnt.cbSize = Marshal.SizeOf(msevnt);
				msevnt.dwFlags = MouseEventFlag.TME_NONCLIENT | MouseEventFlag.TME_LEAVE;
				msevnt.hwndTrack = Handle;
				msevnt.dwHoverTime = 0;
				if(NativeMethods.TrackMouseEvent(msevnt)) {
					trackingMouseLeave = true;
				}
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(ButtonsPanel != null && ButtonsPanel.Bounds.Contains(e.Location)) {
				ButtonsPanel.Handler.OnMouseDown(e);
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(ButtonsPanel != null) {
				ButtonsPanel.Handler.OnMouseUp(e);
			}
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			ToolTipController.AddClientControl(this);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ToolTipController.RemoveClientControl(this);
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseLeave();
			trackingMouseLeave = false;
		}
		protected override bool ProcessMnemonic(char charCode) {
			if(!ShowCaption || !Control.IsMnemonic(charCode, Text) || !ControlBase.GetCanProcessMnemonic(this)) {
				return false;
			}
			SelectNextControl(null, true, true, true, false);
			return true;
		}
		protected override int GetCaptionHorizontal() {
			if(!ShowCaption) return 0;
			if(CaptionLocation == Locations.Top || CaptionLocation == Locations.Default || CaptionLocation == Locations.Bottom) return ViewInfo.CaptionBounds.Height;
			return 0;
		}
		protected override int GetCaptionVertical() {
			if(!ShowCaption) return 0;
			if(CaptionLocation == Locations.Left || CaptionLocation == Locations.Right) return ViewInfo.CaptionBounds.Width;
			return 0;
		}
		protected override int GetPaddingTop() {
			int padding = 0;
			if(CaptionLocation == Locations.Top || CaptionLocation == Locations.Default && !ViewInfo.CaptionBounds.IsEmpty) {
				padding = ViewInfo.ControlClientBounds.Y - ViewInfo.CaptionBounds.Bottom;
			}
			else padding = ViewInfo.ControlClientBounds.Y - ViewInfo.Bounds.Y;
			return padding + Padding.Top;
		}
		protected override int GetPaddingLeft() {
			int padding = 0;
			if(CaptionLocation == Locations.Left && !ViewInfo.CaptionBounds.IsEmpty) {
				padding = ViewInfo.ControlClientBounds.X - ViewInfo.CaptionBounds.Right;
			}
			else padding = ViewInfo.ControlClientBounds.X - ViewInfo.Bounds.X;
			return padding + Padding.Left;
		}
		protected override int GetPaddingRight() {
			int padding = 0;
			if(CaptionLocation == Locations.Right && !ViewInfo.CaptionBounds.IsEmpty) {
				padding = ViewInfo.CaptionBounds.X - ViewInfo.ControlClientBounds.Right;
			}
			else padding = ViewInfo.Bounds.Right - ViewInfo.ControlClientBounds.Right;
			return padding + Padding.Right;
		}
		protected override int GetPaddingBottom() {
			int padding = 0;
			if(CaptionLocation == Locations.Bottom && !ViewInfo.CaptionBounds.IsEmpty) {
				padding = ViewInfo.CaptionBounds.Y - ViewInfo.ControlClientBounds.Bottom;
			}
			else padding = ViewInfo.Bounds.Bottom - ViewInfo.ControlClientBounds.Bottom;
			return padding + Padding.Bottom;
		}
		[Browsable(false)]
		public override Rectangle DisplayRectangle {
			get {
				if(!useViewInfoBoundsAsDisplayRectangle) return base.DisplayRectangle;
				CheckInfo(false);
				Rectangle rect = ViewInfo.Bounds;
				if(HorizontalCaptionLocation) {
					rect.Height -= GetCaptionHorizontal();
					if(CaptionLocation == Locations.Top || CaptionLocation == Locations.Default) {
						rect.Y += GetCaptionHorizontal();
					}
				}
				else {
					rect.Width -= GetCaptionVertical();
					if(CaptionLocation == Locations.Left) rect.X += GetCaptionVertical();
				}
				rect.Offset(GetPaddingLeft(), GetPaddingTop());
				rect.Width -= GetPaddingHorizontal();
				rect.Height -= GetPaddingVertical();
				return rect;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				DestroyAppearance(AppearanceCaption);
			base.Dispose(disposing);
		}
		void OnButtonCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(ButtonsPanel == null) return;
			ButtonsPanel.Buttons.Merge(CustomHeaderButtons);
			OnChanged();
		}
		#region ButtonsPanelEvents
		static readonly object customButtonClick = new object();
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlCustomButtonClick"),
#endif
 Category("Behavior")]
		public event BaseButtonEventHandler CustomButtonClick {
			add { this.Events.AddHandler(customButtonClick, value); }
			remove { this.Events.RemoveHandler(customButtonClick, value); }
		}
		static readonly object customButtonUnchecked = new object();
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlCustomButtonUnchecked"),
#endif
 Category("Behavior")]
		public event BaseButtonEventHandler CustomButtonUnchecked {
			add { this.Events.AddHandler(customButtonUnchecked, value); }
			remove { this.Events.RemoveHandler(customButtonUnchecked, value); }
		}
		static readonly object customButtonChecked = new object();
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlCustomButtonChecked"),
#endif
 Category("Behavior")]
		public event BaseButtonEventHandler CustomButtonChecked {
			add { this.Events.AddHandler(customButtonChecked, value); }
			remove { this.Events.RemoveHandler(customButtonChecked, value); }
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonClick(BaseButtonEventArgs ea) {
			if(ea.Button is IDefaultButton) return;
			BaseButtonEventHandler handler = (BaseButtonEventHandler)Events[customButtonClick];
			if(handler != null)
				handler(this, ea);
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonChecked(BaseButtonEventArgs ea) {
			if(ea.Button is IDefaultButton) return;
			BaseButtonEventHandler handler = (BaseButtonEventHandler)Events[customButtonChecked];
			if(handler != null)
				handler(this, ea);
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonUnchecked(BaseButtonEventArgs ea) {
			if(ea.Button is IDefaultButton) return;
			BaseButtonEventHandler handler = (BaseButtonEventHandler)Events[customButtonUnchecked];
			if(handler != null)
				handler(this, ea);
		}
		void IGroupBoxButtonsPanelOwner.LayoutChanged() {
			if(ViewInfo != null)
				Painter.CalcObjectBounds(ViewInfo);
		}
		#endregion
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlToolTipController"),
#endif
 DefaultValue(null), Category("Appearance")]
		public virtual ToolTipController ToolTipController {
			get { return toolTipControllerCore ?? ToolTipController.DefaultController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null) ToolTipController.Disposed -= ToolTipControllerDisposed;
				toolTipControllerCore = value;
				if(ToolTipController != null) ToolTipController.Disposed += ToolTipControllerDisposed;
			}
		}
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlCustomHeaderButtons"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraEditors.Design.GroupBoxButtonCollectionEditor, " + AssemblyInfo.SRAssemblyDesign,
			typeof(System.Drawing.Design.UITypeEditor)), Category("Custom Header Buttons"), Localizable(true)]
		public BaseButtonCollection CustomHeaderButtons {
			get { return customHeaderButtonsCore; }
		}
		GroupElementLocation buttonsPanelLocationCore;
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlCustomHeaderButtonsLocation"),
#endif
 Category("Appearance"), DefaultValue(GroupElementLocation.Default)]
		public GroupElementLocation CustomHeaderButtonsLocation {
			get { return buttonsPanelLocationCore; }
			set {
				if(buttonsPanelLocationCore == value) return;
				buttonsPanelLocationCore = value;
				OnChanged();
			}
		}
		bool allowBorderColorBlendingCore = false;
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlAllowBorderColorBlending"),
#endif
 Category("Appearance"), DefaultValue(false)]
		public bool AllowBorderColorBlending {
			get { return allowBorderColorBlendingCore; }
			set {
				if(allowBorderColorBlendingCore == value) return;
				allowBorderColorBlendingCore = value;
				OnChanged();
			}
		}
		bool allowHtmlText;
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlAllowHtmlText"),
#endif
 Category("Appearance"), DefaultValue(false)]
		public bool AllowHtmlText {
			get { return allowHtmlText; }
			set {
				allowHtmlText = value;
				OnChanged();
			}
		}
		bool allowGlyphSkinning;
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlAllowGlyphSkinning"),
#endif
 Category("Appearance"), DefaultValue(false)]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				allowGlyphSkinning = value;
				OnChanged();
			}
		}
		ImageCollection htmlImages;
		[DefaultValue(null), 
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlHtmlImages")
#else
	Description("")
#endif
]
		public ImageCollection HtmlImages {
			get { return htmlImages; }
			set {
				if(htmlImages == value) return;
				if(htmlImages != null) htmlImages.Changed -= new EventHandler(OnHtmlImagesChanged);
				htmlImages = value;
				if(htmlImages != null) htmlImages.Changed += new EventHandler(OnHtmlImagesChanged);
				OnChanged();
			}
		}
		void OnHtmlImagesChanged(object sender, EventArgs e) {
			if(AllowHtmlText) {
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlCaptionLocation"),
#endif
 Category("Appearance"), DefaultValue(Locations.Default), SmartTagProperty("Caption Location", "")]
		public Locations CaptionLocation {
			get { return captionLocation; }
			set {
				if(CaptionLocation == value) return;
				captionLocation = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlCaptionImageLocation"),
#endif
 Category("Appearance"), DefaultValue(GroupElementLocation.Default)]
		public GroupElementLocation CaptionImageLocation {
			get { return captionImageLocation; }
			set {
				if(CaptionImageLocation == value) return;
				captionImageLocation = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlCaptionImage"),
#endif
 Category("Appearance"), DefaultValue(null)]
		[Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesignFull, typeof(UITypeEditor))]
		public Image CaptionImage {
			get { return captionImage; }
			set {
				if(CaptionImage == value) return;
				captionImage = value;
				OnChanged();
			}
		}
		DxImageUri captionImageUriCore = new DxImageUri();
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlCaptionImageUri"),
#endif
 Category("Appearance"), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DxImageUri CaptionImageUri {
			get { return captionImageUriCore; }
			set {
				if(CaptionImageUri == value) return;
				captionImageUriCore = value;
				OnChanged();
			}
		}
		bool ShouldSerializeCaptionImagePadding() { return CaptionImagePadding != DefaultCaptionImagePadding; }
		void ResetCaptionImagePadding() { CaptionImagePadding = DefaultCaptionImagePadding; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlCaptionImagePadding"),
#endif
 Category("Appearance")]
		public Padding CaptionImagePadding {
			get { return captionImagePadding; }
			set {
				if(CaptionImagePadding == value) return;
				captionImagePadding = value;
				OnChanged();
			}
		}
		protected virtual bool DefaultShowCaption { get { return true; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlShowCaption"),
#endif
 Category("Appearance"), DefaultValue(true), SmartTagProperty("Show Caption", "", 2, SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual bool ShowCaption {
			get { return showCaption; }
			set {
				if(ShowCaption == value) return;
				showCaption = value;
				OnChanged();
				PerformLayout();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlText"),
#endif
 Category("Appearance"), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		void ResetAppearanceCaption() { AppearanceCaption.Reset(); }
		bool ShouldSerializeAppearanceCaption() { return AppearanceCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("GroupControlAppearanceCaption"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceCaption { get { return appearanceCaption; } }
		protected override bool CanShowCaption { get { return ShowCaption && BorderStyle != BorderStyles.NoBorder; } }
		protected virtual Image GetActualCaptionImage() {
			if(CaptionImageUri != null && CaptionImageUri.HasImage)
				return CaptionImageUri.GetImage();
			return CaptionImage;
		}
		protected override void UpdateViewInfoProperties(GroupObjectInfoArgs info) {
			AppearanceDefault appearance = Painter.DefaultAppearanceCaption.Clone() as AppearanceDefault;
			LookAndFeelHelper.CheckColors(LookAndFeel, appearance, this);
			AppearanceHelper.Combine(info.AppearanceCaption, new AppearanceObject[] { AppearanceCaption }, appearance);
			Painter.UpdateAppearance(info);
			info.CaptionLocation = CaptionLocation;
			info.ShowCaptionImage = ShowCaption;
			info.CaptionImagePadding = CaptionImagePadding;
			info.CaptionImage = GetActualCaptionImage();
			info.CaptionImageLocation = CaptionImageLocation;
			info.AllowHtmlText = AllowHtmlText;
			info.AllowGlyphSkinning = AllowGlyphSkinning;
			info.HtmlImages = HtmlImages;
			info.ButtonLocation = CustomHeaderButtonsLocation;
			info.AllowBorderColorBlending = AllowBorderColorBlending;
			base.UpdateViewInfoProperties(info);
		}
		protected override void RaiseCustomDrawCaption(GroupCaptionCustomDrawEventArgs e) {
			if(CustomDrawCaption != null) CustomDrawCaption(this, e);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int GetTextTopLine() { return ViewInfo.TextBounds.Top + 1; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int GetTextLeftLine() { return ViewInfo.TextBounds.Left + 1; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int GetTextRightLine() { return ViewInfo.TextBounds.Right - 1; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int GetTextBottomLine() { return ViewInfo.TextBounds.Bottom - 1; }
		#region IControlBoxButtonsPanelOwner Members
		bool IGroupBoxButtonsPanelOwner.IsRightToLeft { get { return IsRightToLeft; } }
		#endregion
		#region IButtonsPanelOwner Members
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return new ButtonsPanelControlAppearance(this); }
		}
		bool IButtonsPanelOwner.Enabled {
			get { return Enabled; }
		}
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return AllowHtmlText; }
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return AllowGlyphSkinning; }
		}
		object IButtonsPanelOwner.Images {
			get { return null; }
		}
		bool IButtonsPanelOwner.IsSelected {
			get { return false; }
		}
		void IButtonsPanelOwner.Invalidate() {
			if(ViewInfo != null) {
				Invalidate(ViewInfo.ButtonsPanelBounds);
			}
		}
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new GroupBoxButtonsPanelSkinPainter(LookAndFeel);
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP)
				return new GroupBoxButtonsPanelWindowsXpPainter();
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003)
				return new BaseButtonsPanelOffice2003Painter();
			return new GroupBoxButtonsPanelPainter();
		}
		#endregion
		#region IButtonPanelControlAppearanceOwner Members
		IButtonsPanelControlAppearanceProvider IButtonPanelControlAppearanceOwner.CreateAppearanceProvider() {
			var provider = new ButtonsPanelControlAppearanceProvider();
			provider.Normal.ForeColor = ViewInfo.AppearanceCaption.ForeColor;
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				provider.Hovered.ForeColor = ViewInfo.AppearanceCaption.ForeColor;
				provider.Pressed.ForeColor = ViewInfo.AppearanceCaption.ForeColor;
			}
			else if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Office2003) {
				provider.Hovered.ForeColor = Color.White;
				provider.Pressed.ForeColor = Color.White;
			}
			return provider;
		}
		#endregion
		#region IToolTipControlClient Members
		void ToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		public ToolTipControlInfo GetObjectInfo(Point point) {
			if(ButtonsPanel == null) return null;
			return ButtonsPanel.GetObjectInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return ButtonsPanel != null && ButtonsPanel.ShowToolTips; } }
		#endregion
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class GroupAutoHeightControl : GroupControl {
		bool autoHeight = true;
		int minHeight, maxHeight;
		IAutoHeightControl autoHeightControl = null;
		public GroupAutoHeightControl() {
			this.minHeight = 0;
			this.maxHeight = 0;
		}
		[Category("Behavior"), DefaultValue(0)]
		public int MinHeight {
			get { return minHeight; }
			set {
				if(value < 0) value = 0;
				if(MinHeight == value) return;
				minHeight = value;
			}
		}
		[Category("Behavior"), DefaultValue(0)]
		public int MaxHeight {
			get { return maxHeight; }
			set {
				if(value < 0) value = 0;
				if(MaxHeight == value) return;
				maxHeight = value;
			}
		}
		[Category("Behavior"), DefaultValue(true)]
		public bool AutoHeight {
			get { return autoHeight; }
			set {
				if(AutoHeight == value) return;
				autoHeight = value;
				if(AutoHeight) CheckUpdateHeight();
			}
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			IAutoHeightControl hc = e.Control as IAutoHeightControl;
			if(this.autoHeightControl == null && hc != null) {
				this.autoHeightControl = hc;
				this.autoHeightControl.HeightChanged += new EventHandler(OnHeightControlChanged);
			}
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved(e);
			if(e.Control == this.autoHeightControl && e.Control != null) {
				this.autoHeightControl.HeightChanged -= new EventHandler(OnHeightControlChanged);
				this.autoHeightControl = null;
			}
		}
		bool lockChangeHeight = false;
		void OnHeightControlChanged(object sender, EventArgs e) {
			if(this.autoHeightControl == null || !this.autoHeightControl.SupportsAutoHeight || this.lockChangeHeight) return;
			if(!IsHandleCreated) return;
			this.lockChangeHeight = true;
			try {
				int height = Height;
				GraphicsInfo info = new GraphicsInfo();
				info.AddGraphics(null);
				try {
					height = this.autoHeightControl.CalcHeight(info.Cache);
					height = CalcBoundsByClient(info.Graphics, new Rectangle(0, 0, 100, height)).Height;
				}
				finally {
					info.ReleaseGraphics();
				}
				if(MinHeight != 0) height = Math.Max(MinHeight, height);
				if(MaxHeight != 0) height = Math.Min(MaxHeight, height);
				if(Height != height) {
					SetBoundsCore(Left, Top, Width, height, BoundsSpecified.Height);
				}
			}
			finally {
				this.lockChangeHeight = false;
			}
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			CheckUpdateHeight();
			CheckInfo(false);
		}
		void CheckUpdateHeight() {
			OnHeightControlChanged(this, EventArgs.Empty);
		}
	}
	public enum SplitPanelVisibility { Both, Panel1, Panel2 };
	public enum SplitFixedPanel { None, Panel1, Panel2 };
	public enum SplitCollapsePanel { None, Panel1, Panel2 };
	[Designer("DevExpress.XtraEditors.Design.SplitContainerControlDesigner, " + AssemblyInfo.SRAssemblyDesignFull),
	 Description("Provides two resizable panels that can contain other controls."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	 ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "SplitContainerControl"), SmartTagFilter(typeof(SplitContainerControlFilter)),
	 DXToolboxItem(DXToolboxItemKind.Free)
	]
	public class SplitContainerControl : GroupControl, IXtraSerializable {
		internal static int MagicMinSize = 0;
		SplitGroupPanel panel1, panel2;
		bool horizontal;
		SplitFixedPanel fixedPanel;
		SplitCollapsePanel collapsePanelCore;
		SplitContainerViewInfo containerInfo;
		int splitterPosition, lockUpdate;
		SplitPanelVisibility panelVisibility;
		private static object splitterMoved = new object();
		private static object beginSplitterMoving = new object();
		private static object splitterMoving = new object();
		private static object splitterPositionChanged = new object();
		private static object panelCollapsing = new object();
		private static object panelCollapsed = new object();
		public SplitContainerControl() {
			this.panelVisibility = SplitPanelVisibility.Both;
			this.collapsePanelCore = SplitCollapsePanel.None;
			this.lockUpdate = 0;
			this.splitterPosition = 100;
			this.fixedPanel = SplitFixedPanel.Panel1;
			this.horizontal = true;
			this.panel1 = CreatePanel();
			this.panel2 = CreatePanel();
			this.panel1.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			this.panel2.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			this.containerInfo = CreateContainerInfo();
			Controls.AddRange(new Control[] { Panel1, Panel2 });
			UpdateDockPosition();
		}
		protected override void OnCompatibleDrawingModeChanged() {
			Panel1.UseCompatibleDrawingMode = Panel2.UseCompatibleDrawingMode = UseCompatibleDrawingMode;
			base.OnCompatibleDrawingModeChanged();
		}
		public event EventHandler SplitterMoved {
			add { Events.AddHandler(splitterMoved, value); }
			remove { Events.RemoveHandler(splitterMoved, value); }
		}
		public event BeginSplitMovingEventHandler BeginSplitterMoving {
			add { Events.AddHandler(beginSplitterMoving, value); }
			remove { Events.RemoveHandler(beginSplitterMoving, value); }
		}
		public event SplitMovingEventHandler SplitterMoving {
			add { Events.AddHandler(splitterMoving, value); }
			remove { Events.RemoveHandler(splitterMoving, value); }
		}
		public event EventHandler SplitterPositionChanged { 
			add { Events.AddHandler(splitterPositionChanged, value); }
			remove { Events.RemoveHandler(splitterPositionChanged, value); }
		}
		public event SplitGroupPanelCollapsingEventHandler SplitGroupPanelCollapsing {
			add { Events.AddHandler(panelCollapsing, value); }
			remove { Events.RemoveHandler(panelCollapsing, value); }
		}
		public event SplitGroupPanelCollapsedEventHandler SplitGroupPanelCollapsed {
			add { Events.AddHandler(panelCollapsed, value); }
			remove { Events.RemoveHandler(panelCollapsed, value); }
		}
		protected virtual void RaiseSplitterMoved() {
			EventHandler handler = (EventHandler)Events[splitterMoved];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseBeginSplitterMoving(BeginSplitMovingEventArgs e) {
			BeginSplitMovingEventHandler handler = (BeginSplitMovingEventHandler)Events[beginSplitterMoving];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseSplitterMoving(SplitMovingEventArgs e) {
			SplitMovingEventHandler handler = (SplitMovingEventHandler)Events[splitterMoving];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseSplitterPositionChanged() {
			EventHandler handler = (EventHandler)Events[splitterPositionChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal void RaiseSplitGroupPanelCollapsing(SplitGroupPanelCollapsingEventArgs e) {
			SplitGroupPanelCollapsingEventHandler handler = (SplitGroupPanelCollapsingEventHandler)Events[panelCollapsing];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseSplitGroupPanelCollapsed(SplitGroupPanelCollapsedEventArgs e) {
			SplitGroupPanelCollapsedEventHandler handler = (SplitGroupPanelCollapsedEventHandler)Events[panelCollapsed];
			if(handler != null) handler(this, e);
		}
		public override void BeginInit() {
			base.BeginInit();
			Panel1.SuspendLayout();
			Panel2.SuspendLayout();
		}
		public override void EndInit() {
			base.EndInit();
			Panel2.ResumeLayout(false);
			Panel1.ResumeLayout(false);
		}
		protected override void OnLayout(LayoutEventArgs levent) {
			UpdateDockPosition();
			base.OnLayout(levent);
		}
		protected override bool DefaultShowCaption { get { return false; } }
		protected SplitContainerViewInfo ContainerInfo { get { return containerInfo; } }
		protected virtual SplitContainerViewInfo CreateContainerInfo() {
			return new SplitContainerViewInfo(this);
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitContainerControlPanelVisibility"),
#endif
 Category("Layout"), DefaultValue(SplitPanelVisibility.Both), SmartTagProperty("Panel Visibility", "", SmartTagActionType.RefreshAfterExecute)]
		public virtual SplitPanelVisibility PanelVisibility {
			get { return panelVisibility; }
			set {
				if(PanelVisibility == value) return;
				panelVisibility = value;
				UpdateDockPosition();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitContainerControlCollapsePanel"),
#endif
 Category("Layout"), DefaultValue(SplitCollapsePanel.None), XtraSerializableProperty]
		public virtual SplitCollapsePanel CollapsePanel {
			get { return collapsePanelCore; }
			set {
				if(CollapsePanel == value) return;
				collapsePanelCore = value;
				UpdateDockPosition();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitContainerControlFixedPanel"),
#endif
 Category("Layout"), DefaultValue(SplitFixedPanel.Panel1)]
		public virtual SplitFixedPanel FixedPanel {
			get { return fixedPanel; }
			set {
				if(FixedPanel == value) return;
				fixedPanel = value;
				UpdateDockPosition();
			}
		}
		public override Cursor Cursor {
			get {
				if(ContainerInfo != null && ContainerInfo.IsMouseOverSplitter) return Cursor.Current;
				return base.Cursor;
			}
			set {
				base.Cursor = value;
			}
		}
		protected void BeginUpdate() {
			this.lockUpdate++;
		}
		protected void EndUpdate() {
			if(--this.lockUpdate == 0)
				UpdateDockPosition();
		}
		SizeF scaleFactor = new SizeF(1f, 1f);
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SizeF ScaleFactor { get { return scaleFactor; } }
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			scaleFactor = new SizeF(scaleFactor.Width * factor.Width, scaleFactor.Height * factor.Height);
			base.ScaleControl(factor, specified);
		}
		double ratio = -1;
		int Scale(int position) {
			if(!AllowScaleSplitter) return position;
			if(Horizontal) return RectangleHelper.ScaleHorizontal(position, ScaleFactor.Width);
			return RectangleHelper.ScaleVertical(position, ScaleFactor.Width);
		}
		int DeScale(int position) {
			if(!AllowScaleSplitter) return position;
			if(Horizontal) return RectangleHelper.DeScaleHorizontal(position, ScaleFactor.Width);
			return RectangleHelper.DeScaleVertical(position, ScaleFactor.Width);
		}
		protected virtual bool AllowScaleSplitter {
			get { return FixedPanel != SplitFixedPanel.None || this.ratio == -1 || this.ratio == 0; }
		}
		protected internal int ScaledSplitterPosition {
			get { return Scale(SplitterPosition); }
			set {
				SplitterPosition = DeScale(value);
			}
		}
		protected internal int GetSplitterPosition() {
			if(AllowScaleSplitter)
				return Scale(SplitterPosition);
			return (int)Math.Ceiling(ContainerInfo.Helper.GetSize(Size) / ratio);
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitContainerControlSplitterPosition"),
#endif
 Category("Layout"), DefaultValue(100), SmartTagProperty("Splitter Position", ""), XtraSerializableProperty]
		public virtual int SplitterPosition {
			get { return splitterPosition; }
			set {
				if(!IsLoaded) {
					splitterPosition = value;
					return;
				}
				else {
					value = GetSplitRange().CheckPosition(value);
				}
				if(SplitterPosition == value) return;
				this.ratio = -1;
				splitterPosition = value;
				UpdateDockPosition();
				OnSplitterPositionChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitContainerControlHorizontal"),
#endif
 Category("Layout"), DefaultValue(true), SmartTagProperty("Horizontal", "", 0, SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual bool Horizontal {
			get { return horizontal; }
			set {
				if(Horizontal == value) return;
				horizontal = value;
				UpdateDockPosition();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitContainerControlPanel1"),
#endif
 Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagProperty("Panel1", "")]
		public SplitGroupPanel Panel1 { get { return panel1; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitContainerControlPanel2"),
#endif
 Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagProperty("Panel2", "")]
		public SplitGroupPanel Panel2 { get { return panel2; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitContainerControlShowCaption"),
#endif
 DefaultValue(false)]
		public override bool ShowCaption {
			get { return base.ShowCaption; }
			set { base.ShowCaption = value; }
		}
		bool isSplitterFixedCore;
		[ Category("Layout"), DefaultValue(false)]
		public virtual bool IsSplitterFixed {
			get { return isSplitterFixedCore; }
			set { isSplitterFixedCore = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle SplitterBounds {
			get { return ContainerInfo.Splitter.Bounds; }
		}
		public void SetPanelBorderStyle(BorderStyles border) {
			if(Panel1.BorderStyle == border && Panel2.BorderStyle == border) return;
			BeginUpdate();
			try {
				Panel1.BorderStyle = border;
				Panel2.BorderStyle = border;
			}
			finally {
				EndUpdate();
			}
		}
		public void SwapPanels() {
			BeginUpdate();
			SplitGroupPanel tmp = panel1;
			panel1 = panel2;
			panel2 = tmp;
			EndUpdate();
		}
		bool collapsedCore;
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitContainerControlCollapsed"),
#endif
		Category("Layout"), DefaultValue(false), XtraSerializableProperty]
		public virtual bool Collapsed {
			get { return (Site != null || IsLoading) ? collapsedCore : IsPanelCollapsed; }
			set {
				collapsedCore = value;
				if(IsLoading) return;
				SetPanelCollapsed(value);
			}
		}
		[Browsable(false)]
		public bool IsPanelCollapsed {
			get { return ContainerInfo.Splitter.IsCollapsed; }
		}
		public void SetPanelCollapsed(bool collapsed) {
			if(!CanProcessCollapsing()) return;
			if(ContainerInfo.Splitter.IsCollapsed != collapsed) {
				SplitGroupPanel panel = GetCollapsePanel();
				ContainerInfo.Splitter.ChangeCollapsedState(panel);
			}
		}
		internal SplitGroupPanel GetCollapsePanel() {
			return (CollapsePanel == SplitCollapsePanel.Panel1) ? Panel1 : Panel2;
		}
		protected internal virtual bool CanProcessCollapsing() {
			return (PanelVisibility == SplitPanelVisibility.Both) &&
				(CollapsePanel != SplitCollapsePanel.None);
		}
		protected override BorderStyles DefaultBorderStyle { get { return BorderStyles.NoBorder; } }
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			ContainerInfo.UpdatePainters();
			base.OnLookAndFeelChanged(sender, e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			ContainerInfo.Splitter.CheckMouse(PointToClient(MousePosition));
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ContainerInfo.Splitter.CheckMouse(new Point(-10000, -10000));
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			ContainerInfo.Splitter.CheckMouse(e.Location);
		}
		protected override void OnLostCapture() {
			ContainerInfo.Splitter.StopSplit(false);
		}
		protected virtual void OnSplitterPositionChanged() {
			RaiseSplitterMoved();
			RaiseSplitterPositionChanged();
			FireChanged();
		}
		protected void FireChanged() {
			if(!DesignMode || IsLoading) return;
			System.ComponentModel.Design.IComponentChangeService srv = GetService(typeof(System.ComponentModel.Design.IComponentChangeService)) as System.ComponentModel.Design.IComponentChangeService;
			if(srv == null) return;
			srv.OnComponentChanged(this, null, null, null);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Left && e.Clicks == 1)
				if(DesignMode) {
					ISelectionService srv = GetService(typeof(ISelectionService)) as ISelectionService;
					if(srv != null && !srv.GetComponentSelected(this)) srv.SetSelectedComponents(new object[] { this });
				}
			ContainerInfo.Splitter.CheckMouseDown(e.Location);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button == MouseButtons.Left)
				ContainerInfo.Splitter.CheckMouseUp(e.Location);
		}
		protected override void CheckInfoOnResize() {
			Form frm = FindForm();
			CheckInfo(frm == null || frm.WindowState != FormWindowState.Minimized);
		}
		bool allowSuspendRedraw = false;
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowSuspendRedraw { get { return allowSuspendRedraw; } set { allowSuspendRedraw = value; UpdateDockPosition(); } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowTouchScroll { get { return base.AllowTouchScroll; } set { base.AllowTouchScroll = value; } }
		protected virtual void UpdateDockPosition() {
			if(!IsLoaded || this.lockUpdate != 0) return;
			if(AllowSuspendRedraw) ControlUtils.SuspendRedraw(this);
			try {
				Panel1.SuspendLayout();
				Panel2.SuspendLayout();
				try {
					ContainerInfo.Calc();
					Panel1.Visible = (PanelVisibility != SplitPanelVisibility.Panel2);
					Panel2.Visible = (PanelVisibility != SplitPanelVisibility.Panel1);
					Panel1.UpdateBounds(ContainerInfo.Panel1Client);
					Panel2.UpdateBounds(ContainerInfo.Panel2Client);
					UpdateSplitterPosition();
				} finally {
					Panel1.ResumeLayout();
					Panel2.ResumeLayout();
				}
			}
			finally {
				if(AllowSuspendRedraw) ControlUtils.ResumeRedraw(this);
			}
			Invalidate();
		}
		void UpdateSplitterPosition() {
					if(this.ratio == -1 && PanelVisibility == SplitPanelVisibility.Both)
						this.ratio = ContainerInfo.CalcRatio();
					int realPosition = DeScale(GetSplitterPosition());
					int threshold = 1;
					if(Math.Abs(realPosition - SplitterPosition) > threshold) {
						this.splitterPosition = realPosition;
						RaiseSplitterPositionChanged();
					}
				}
		protected internal void OnMinSizeChanged(SplitGroupPanel panel) {
			if(!IsLoaded) return;
			UpdateDockPosition();
		}
		protected override void OnLoaded() {
			if(IsLoaded) {
				CheckInfo(true);
				return;
			}
			SuspendLayout();
			base.OnLoaded();
			try {
				if(collapsedCore)
					SetPanelCollapsed(true);
				UpdateDockPosition();
			}
			finally {
				ResumeLayout();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoSize { get { return false; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AutoSizeMode AutoSizeMode { get { return base.AutoSizeMode; } set { } }
		protected internal SplitRange GetSplitRange() { return ContainerInfo.GetSplitRange(); }
		protected internal Rectangle GetSplitterBounds(int position) { return ContainerInfo.GetSplitterBounds(position); }
		protected override void DrawPanelElements(GraphicsCache cache, GroupObjectInfoArgs info) {
			if(!ContainerInfo.Panel1Info.Bounds.IsEmpty)
				ObjectPainter.DrawObject(cache, Panel1.Painter, ContainerInfo.Panel1Info);
			if(!ContainerInfo.Panel2Info.Bounds.IsEmpty)
				ObjectPainter.DrawObject(cache, Panel2.Painter, ContainerInfo.Panel2Info);
			ContainerInfo.Splitter.Draw(cache);
		}
		protected override void OnGroupLayout() {
		}
		protected internal void OnPanelChanged(SplitGroupPanel panel) {
			UpdateDockPosition();
		}
		protected virtual SplitGroupPanel CreatePanel() {
			return new SplitGroupPanel(this);
		}
		protected virtual SplitterControl CreateSplitter() { return new SplitterControl(); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ControlCollection Controls { get { return base.Controls; } }
		protected override Control.ControlCollection CreateControlsInstance() {
			return new SplitControlCollection(this);
		}
		protected class SplitControlCollection : Control.ControlCollection {
			public SplitControlCollection(SplitContainerControl owner) : base(owner) { }
			public override void Add(Control value) {
				if(!(value is SplitterControl) && !(value is SplitGroupPanel)) throw new NotSupportedException();
				base.Add(value);
			}
			public override void Clear() {
				throw new NotSupportedException();
			}
			public override void Remove(Control value) {
				if(!(value is SplitterControl) && !(value is SplitGroupPanel)) throw new NotSupportedException();
				base.Remove(value);
			}
		}
		internal void ApplySplitterPosition(int position) {
			if(WindowsFormsSettings.GetIsRightToLeft(this) && Horizontal) {
				SplitRange range = GetSplitRange();
				position = range.Min + (range.Max - range.Min) - position;
			}
			ScaledSplitterPosition = position;
		}
		public void OnEndDeserializing(string restoredVersion) { }
		public void OnEndSerializing() { }
		public void OnStartDeserializing(LayoutAllowEventArgs e) { }
		public void OnStartSerializing() { }
	}
	[ToolboxItem(false),
	Designer("DevExpress.XtraEditors.Design.SplitGroupPanelDesigner, " + AssemblyInfo.SRAssemblyDesignFull)
   , System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraEditors.Design.SplitGroupPanelSerializer, " + AssemblyInfo.SRAssemblyDesignFull,
		"System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")
	]
	public class SplitGroupPanel : GroupControl {
		SplitContainerControl owner;
		int minSize;
		public SplitGroupPanel(SplitContainerControl owner) {
			this.owner = owner;
			this.ShowCaption = false;
			this.BorderStyle = DefaultBorderStyle;
			this.minSize = 0;
			this.autoScrollCore = false;
			useViewInfoBoundsAsDisplayRectangle = false;
		}
		protected override void OnCompatibleDrawingModeChanged() {
			SetStyle(ControlStyles.AllPaintingInWmPaint, !UseCompatibleDrawingMode);
		}
		protected override void OnResize(EventArgs e) {
			if(IsLoading) return;
			base.OnResize(e);
		}
		protected internal void UpdateBounds(Rectangle bounds) {
			if(Size == bounds.Size) {
				InitLayout();
			}
			Bounds = bounds;
		}
		protected override Size DefaultSize { get { return Size.Empty; } }
		protected internal override bool IsLoading { get { return Owner == null || Owner.IsLoading || base.IsLoading; } }
		[DefaultValue(false)]
		public override bool ShowCaption {
			get { return base.ShowCaption; }
			set { base.ShowCaption = value; }
		}
		protected override BorderStyles DefaultBorderStyle { get { return BorderStyles.NoBorder; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlAutoScroll"),
#endif
		RefreshProperties(System.ComponentModel.RefreshProperties.All), EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool AutoScroll {
			get { return autoScrollCore; }
			set { autoScrollCore = value; PerformLayout(); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public SplitContainerControl Owner { get { return owner; } }
		[DefaultValue(0), Category("Behavior")]
		public int MinSize {
			get { return minSize; }
			set {
				if(value < SplitContainerControl.MagicMinSize) value = SplitContainerControl.MagicMinSize;
				if(MinSize == value) return;
				minSize = value;
				if(Owner != null) Owner.OnMinSizeChanged(this);
			}
		}
		protected override void UpdateViewInfoProperties(GroupObjectInfoArgs info) {
			info.BorderStyle = BorderStyles.NoBorder; ;
			info.ShowCaption = false;
			info.Bounds = GetClientBoundsCore();
		}
		protected internal GroupObjectInfoArgs CreatePaintInfo() {
			GroupObjectInfoArgs info = CreateViewInfo();
			base.UpdateViewInfoProperties(info);
			return info;
		}
		protected override void OnChanged() {
			Owner.OnPanelChanged(this);
			base.OnChanged();
		}
	}
	public class ObjectCustomDrawEventArgs : EventArgs {
		GraphicsCache cache;
		ObjectPainter painter;
		ObjectInfoArgs info;
		bool handled;
		public ObjectCustomDrawEventArgs(GraphicsCache cache, ObjectPainter painter, ObjectInfoArgs info) {
			this.cache = cache;
			this.painter = painter;
			this.info = info;
			this.handled = false;
		}
		public bool Handled { get { return handled; } set { handled = value; } }
		public GraphicsCache Cache { get { return cache; } }
		public ObjectPainter Painter { get { return painter; } }
		public ObjectInfoArgs Info { get { return info; } }
		public Graphics Graphics { get { return Cache.Graphics; } }
	}
	public class GroupCaptionCustomDrawEventArgs : ObjectCustomDrawEventArgs {
		public GroupCaptionCustomDrawEventArgs(GraphicsCache cache, ObjectPainter painter, ObjectInfoArgs info)
			:
			base(cache, painter, info) {
		}
		public new GroupObjectInfoArgs Info { get { return base.Info as GroupObjectInfoArgs; } }
		public Rectangle CaptionBounds { get { return Info.CaptionBounds; } }
	}
	public delegate void GroupCaptionCustomDrawEventHandler(object sender, GroupCaptionCustomDrawEventArgs e);
	public delegate void SplitGroupPanelCollapsingEventHandler(object sender, SplitGroupPanelCollapsingEventArgs e);
	public delegate void SplitGroupPanelCollapsedEventHandler(object sender, SplitGroupPanelCollapsedEventArgs e);
	public delegate void SplitMovingEventHandler(object sender, SplitMovingEventArgs e);
	public delegate void BeginSplitMovingEventHandler(object sender, BeginSplitMovingEventArgs e);
	public class BeginSplitMovingEventArgs : CancelEventArgs {
		Point currentPointCore;
		public BeginSplitMovingEventArgs(Point currentPoint, bool cancel) {
			this.currentPointCore = currentPoint;
			this.Cancel = cancel;
		}
		public Point CurrentPoint {
			get { return currentPointCore; }
		}
	}
	public class SplitMovingEventArgs : BeginSplitMovingEventArgs {
		int currentPositionCore;
		public SplitMovingEventArgs(int currentPosition, Point currentPoint, bool cancel)
			: base(currentPoint, cancel) {
			this.currentPositionCore = currentPosition;
		}
		public int CurrentPosition {
			get { return currentPositionCore; }
		}
	}
	public class SplitGroupPanelCollapsingEventArgs : CancelEventArgs {
		SplitGroupPanel panelCore;
		bool collapsingCore;
		public SplitGroupPanelCollapsingEventArgs(SplitGroupPanel panel, bool collapsing) {
			this.panelCore = panel;
			this.collapsingCore = collapsing;
		}
		public SplitGroupPanel Panel {
			get { return panelCore; }
		}
		public bool Collapsing {
			get { return collapsingCore; }
		}
	}
	public class SplitGroupPanelCollapsedEventArgs : EventArgs {
		SplitGroupPanel panelCore;
		bool collapsedCore;
		public SplitGroupPanelCollapsedEventArgs(SplitGroupPanel panel, bool collapsed) {
			this.panelCore = panel;
			this.collapsedCore = collapsed;
		}
		public SplitGroupPanel Panel {
			get { return panelCore; }
		}
		public bool Collapsed {
			get { return collapsedCore; }
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class SplitContainerSplitter : IMessageFilter {
		SplitterInfoArgs info;
		ObjectPainter painter;
		SplitContainerControl container;
		SplitContainerViewInfo.SplitHelper helper;
		const int EmptySplitterPosition = -10000;
		int currentSplitPosition = EmptySplitterPosition, delta = 0;
		bool mouseIn = false;
		bool mouseInCollapseButton = false;
		public SplitContainerSplitter(SplitContainerControl container) {
			this.container = container;
			this.helper = new SplitContainerViewInfo.SplitHelper(container);
			this.info = new SplitterInfoArgs(true);
			UpdatePainter();
		}
		protected SplitContainerViewInfo.SplitHelper Helper { get { return helper; } }
		protected SplitContainerControl Container { get { return container; } }
		public void Update() {
			Info.IsCollapsable = IsCollapsible;
			Info.Inverted = (!IsPanel1Collapse) ^ IsCollapsed;
			Info.IsVertical = Container.Horizontal;
		}
		public void UpdatePainter() {
			this.painter = SplitterHelper.GetPainter(Container.LookAndFeel);
		}
		public void Draw(GraphicsCache cache) {
			if(Bounds.IsEmpty) return;
			ObjectPainter.DrawObject(cache, Painter, info);
		}
		public int CalcSize(Graphics g) {
			Size size = ObjectPainter.CalcObjectMinBounds(g, Painter, Info).Size;
			return Math.Max(3, Helper.GetSize(size));
		}
		protected void Capture() {
			Container.Capture = true;
		}
		bool fWaitDragging = false;
		Point startPoint = Point.Empty;
		public void CheckMouseDown(Point point) {
			CheckMouse(point);
			SplitRange range = Container.GetSplitRange();
			if(!MouseIn || range.IsEmpty) return;
			if(MouseIn && IsSplitterFixedAndCollapsable) {
				fWaitDragging = true;
				return;
			}
			BeginSplitterMoving(point);
		}
		protected void StartSplit(Point point, SplitRange range) {
			Application.AddMessageFilter(this);
			Capture();
			HideResizeSplitter();
			delta = GetDelta(point, Container.GetSplitterBounds(-1));
			MoveResizeSplitter(point, true);
			SetCursorCore(GetSplitCursor());
		}
		int GetDelta(Point point, Rectangle currentSplitter) {
			if(Container.FixedPanel != SplitFixedPanel.Panel2)
				return Helper.GetNear(point) - Helper.GetNear(currentSplitter);
			return Helper.GetNear(point) - Helper.GetFar(currentSplitter);
		}
		public void CheckMouseUp(Point point) {
			if(fWaitDragging) {
				CancelSplitterMoving();
				if(Bounds.Contains(point))
					DoExpandCollapseOnClick();
			}
			StopSplit(true);
		}
		bool collapsedStateCore = false;
		[Obsolete("Use the IsCollasible property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsCollapsable {
			get { return IsCollapsible; }
		}
		public bool IsCollapsible {
			get { return Container.CanProcessCollapsing(); }
		}
		public bool IsSplitterFixedAndCollapsable {
			get { return Container.IsSplitterFixed && IsCollapsible; }
		}
		public bool IsCollapsed {
			get { return collapsedStateCore; }
		}
		protected internal bool IsPanel1Collapse {
			get { return Container.CollapsePanel == SplitCollapsePanel.Panel1; }
		}
		public void DoExpandCollapseOnClick() {
			if(Container.Site != null || !Container.CanProcessCollapsing()) return;
			SplitGroupPanel panel = Container.GetCollapsePanel();
			ChangeCollapsedState(panel);
		}
		internal void ChangeCollapsedState(SplitGroupPanel panel) {
			bool cancelCollapsing = false;
			ContainerRaiseCollapsing(panel, out cancelCollapsing);
			if(!cancelCollapsing) {
				this.collapsedStateCore = !collapsedStateCore;
				Container.OnMinSizeChanged(panel);
				ContainerRaiseCollapsed(panel);
			}
		}
		void ContainerRaiseCollapsing(SplitGroupPanel panel, out bool cancelCollapsing) {
			SplitGroupPanelCollapsingEventArgs eCollapsing = new SplitGroupPanelCollapsingEventArgs(panel, !collapsedStateCore);
			Container.RaiseSplitGroupPanelCollapsing(eCollapsing);
			cancelCollapsing = eCollapsing.Cancel;
		}
		void ContainerRaiseCollapsed(SplitGroupPanel panel) {
			SplitGroupPanelCollapsedEventArgs eCollapsed = new SplitGroupPanelCollapsedEventArgs(panel, collapsedStateCore);
			Container.RaiseSplitGroupPanelCollapsed(eCollapsed);
		}
		public void StopSplit(bool resize) {
			CancelSplitterMoving();
			SetCursorCore(Cursors.Default);
			Application.RemoveMessageFilter(this);
			int position = CurrentSplitPosition;
			HideResizeSplitter();
			if(position != EmptySplitterPosition && resize)
				Container.ApplySplitterPosition(position);
		}
		void HideResizeSplitter() {
			CurrentSplitPosition = EmptySplitterPosition;
		}
		int CurrentSplitPosition {
			get { return currentSplitPosition; }
			set {
				if(currentSplitPosition == value) return;
				if(currentSplitPosition != EmptySplitterPosition) DrawResizeSplitter();
				this.currentSplitPosition = value;
				if(currentSplitPosition != EmptySplitterPosition) DrawResizeSplitter();
			}
		}
		bool IsResizing { get { return this.currentSplitPosition != EmptySplitterPosition; } }
		void MoveResizeSplitter(Point point, bool force) {
			if(!IsResizing && !force) return;
			SplitRange range = Container.GetSplitRange();
			int pos = GetPosition(point, Container.DisplayRectangle);
			SplitMovingEventArgs args = new SplitMovingEventArgs(range.CheckPosition(pos), point, false);
			Container.RaiseSplitterMoving(args);
			if(!args.Cancel)
				CurrentSplitPosition = range.CheckPosition(pos);
		}
		int GetPosition(Point point, Rectangle disp) {
			if(Container.FixedPanel != SplitFixedPanel.Panel2)
				return Helper.GetNear(point) - (Helper.GetNear(disp) + delta);
			return (Helper.GetFar(disp) - Helper.GetNear(point)) + delta;
		}
		public ObjectPainter Painter { get { return painter; } }
		public SplitterInfoArgs Info { get { return info; } set { info = value; } }
		public Rectangle Bounds { get { return Info.Bounds; } set { Info.Bounds = value; } }
		public Rectangle Button { get { return Info.Button; } set { Info.Button = value; } }
		public void CheckMouse(Point point) {
			if(fWaitDragging && PointIsOutOfStartDragArea(point) && !Container.IsSplitterFixed) {
				fWaitDragging = false;
				if(IsCollapsed) return;
				HideResizeSplitter();
				StartSplit(point, Container.GetSplitRange());
				return;
			}
			if(IsResizing) {
				SetCursorCore(GetSplitCursor());
				MoveResizeSplitter(point, false);
				return;
			}
			MouseIn = (!Bounds.IsEmpty && (!Container.IsSplitterFixed || IsCollapsible)) ? Bounds.Contains(point) : false;
			MouseInButton = (MouseIn && !Button.IsEmpty) ? Button.Contains(point) : false;
			SetCursorCore((MouseIn && !IsCollapsed && !Container.IsSplitterFixed) ? GetSplitCursor() : Cursors.Default);
		}
		void SetCursorCore(Cursor cur) {
			if(Cursor.Current != cur)
				Cursor.Current = cur;
		}
		void BeginSplitterMoving(Point point) {
			BeginSplitMovingEventArgs args = new BeginSplitMovingEventArgs(point, false);
			Container.RaiseBeginSplitterMoving(args);
			if(!args.Cancel) {
				fWaitDragging = true;
				startPoint = point;
			}
		}
		void CancelSplitterMoving() {
			fWaitDragging = false;
			startPoint = Point.Empty;
		}
		bool PointIsOutOfStartDragArea(Point pt) {
			return (Math.Abs(pt.X - startPoint.X) > 1) || (Math.Abs(pt.Y - startPoint.Y) > 1);
		}
		Cursor GetSplitCursor() {
			if(MouseInButton) return Cursors.Default;
			return Info.IsVertical ? Cursors.VSplit : Cursors.HSplit;
		}
		public bool MouseIn {
			get { return mouseIn; }
			set {
				if(MouseIn == value) return;
				mouseIn = value;
				Info.State = MouseIn ? ObjectState.Hot : ObjectState.Pressed;
				Invalidate();
			}
		}
		public bool MouseInButton {
			get { return mouseInCollapseButton; }
			set {
				if(MouseInButton == value) return;
				mouseInCollapseButton = value;
			}
		}
		public void Invalidate() {
			if(Container != null && !Bounds.IsEmpty) Container.Invalidate(Bounds);
		}
		protected void DrawResizeSplitter() {
			if(!IsResizing) return;
			Rectangle r = Container.GetSplitterBounds(CurrentSplitPosition);
			if(Helper.IsContainerHorizontalRtl) { r = Helper.GetRtlBounds(r); }
			SplitterLineHelper.Default.DrawReversibleSplitter(Container.Handle, r);
		}
		bool IMessageFilter.PreFilterMessage(ref Message m) {
			if(m.Msg == WM_KEYDOWN && (int)m.WParam == (int)Keys.Escape) {
				StopSplit(false);
				return true;
			}
			return false;
		}
		const int WM_KEYDOWN = 0x0100;
	}
	public class SplitRange {
		public int Min, Max;
		public SplitRange(int min, int max) {
			this.Max = max;
			this.Min = min;
		}
		public SplitRange() : this(0, 0) { }
		public bool IsEmpty { get { return Min >= Max; } }
		public int CheckPosition(int position) {
			return Math.Min(Math.Max(position, Min), Max);
		}
	}
	public class SplitContainerViewInfo : IDisposable {
		GraphicsInfo ginfo;
		SplitContainerControl container;
		SplitContainerSplitter splitter;
		GroupObjectInfoArgs panel1Info, panel2Info;
		SplitHelper helper;
		public SplitContainerViewInfo(SplitContainerControl container) {
			this.container = container;
			this.ginfo = new GraphicsInfo();
			this.helper = new SplitHelper(Container);
			this.panel1Info = new GroupObjectInfoArgs();
			this.panel2Info = new GroupObjectInfoArgs();
			this.splitter = CreateSplitter();
		}
		protected internal virtual SplitContainerSplitter CreateSplitter() {
			return new SplitContainerSplitter(Container);
		}
		public virtual void Dispose() {
		}
		public void UpdatePainters() {
			Splitter.UpdatePainter();
		}
		protected internal SplitHelper Helper { get { return helper; } }
		public SplitContainerControl Container { get { return container; } }
		public GraphicsInfo GInfo { get { return ginfo; } }
		public bool IsHorizontal {
			get { return Container.Horizontal; }
		}
		public virtual void Calc() {
			GInfo.AddGraphics(null);
			try {
				CreateInfo();
				UpdatePanelBounds();
				CalcInfo();
			} finally {
				GInfo.ReleaseGraphics();
			}
		}
		public SplitRange GetSplitRange() {
			SplitRange range = new SplitRange();
			if(ClientRectangle.IsEmpty) return range;
			Rectangle res = ClientRectangle;
			int m1 = Container.Panel1.MinSize, m2 = Container.Panel2.MinSize;
			if(IsSplitFixedPanel2) {
				m1 = Container.Panel2.MinSize;
				m2 = Container.Panel1.MinSize;
			}
			range.Min = m1;
			range.Max = Helper.GetSize(res) - ( +m2 + GetSplitterSize());
			range.Max = Math.Max(range.Max, range.Min);
			if(range.Min < 0) range.Max = range.Min = 0;
			return range;
		}
		public virtual bool IsMouseOverSplitter { get { return Splitter.MouseIn; } }
		protected internal double CalcRatio() {
			double ratio = -1;
			int size = Helper.GetSize(Container.Size);
			int panel1 = GetSplitRange().CheckPosition(Container.SplitterPosition);
			if(size == 0 || panel1 == 0) return ratio;
			ratio = (double)size / (double)panel1;
			return ratio;
		}
		protected virtual int GetSplitterSize() {
			int splitterSize = 0;
			GInfo.AddGraphics(null);
			try {
				splitterSize = Splitter.CalcSize(GInfo.Graphics);
			} finally {
				GInfo.ReleaseGraphics();
			}
			return splitterSize;
		}
		protected virtual void CreateInfo() {
			this.panel1Info = Container.Panel1.CreatePaintInfo();
			this.panel2Info = Container.Panel2.CreatePaintInfo();
		}
		protected virtual void CalcInfo() {
			if(!Panel1Info.Bounds.IsEmpty)
				ObjectPainter.CalcObjectBounds(GInfo.Graphics, Container.Panel1.Painter, Panel1Info);
			if(!Panel2Info.Bounds.IsEmpty)
				ObjectPainter.CalcObjectBounds(GInfo.Graphics, Container.Panel2.Painter, Panel2Info);
		}
		public Rectangle ClientRectangle { get { return Container.DisplayRectangle; } }
		protected virtual void UpdatePanelBounds() {
			if(!IsSplitPanelVisibilityBoth) {
				SetBoundsForVisibilityPanel();
				return;
			}
			Splitter.Update();
			int splitterSize = GetSplitterSize();
			int splitPosition = GetSplitPosition(splitterSize);
			PanelInfoHelper info = new PanelInfoHelper(this, splitterSize, splitPosition);
			Panel1Info.Bounds = info.GetPanel1Bounds();
			Panel2Info.Bounds = info.GetPanel2Bounds();
			Splitter.Bounds = info.GetSplitterBounds(info.SplitPosition);
			Splitter.Button = CalcCollapseButton(Splitter.Bounds);
		}
		bool IsSplitPanelVisibilityBoth {
			get { return Container.PanelVisibility == SplitPanelVisibility.Both; }
		}
		bool IsSplitPanelVisibilityPanel1 {
			get { return Container.PanelVisibility == SplitPanelVisibility.Panel1; }
		}
		void SetBoundsForVisibilityPanel() {
			Splitter.Bounds = Rectangle.Empty;
			Panel1Info.Bounds = Panel2Info.Bounds = Rectangle.Empty;
			if(IsSplitPanelVisibilityPanel1)
				Panel1Info.Bounds = ClientRectangle;
			else
				Panel2Info.Bounds = ClientRectangle;
		}
		public Rectangle GetSplitterBounds(int splitterPosition) {
			Rectangle splitter = ClientRectangle;
			if(splitterPosition == -1)
				splitterPosition = GetSplitPosition();
			int splitterSize = GetSplitterSize();
			splitter = Helper.SetNear(splitter, Helper.GetNear(ClientRectangle) + GetOffset(splitterPosition, splitterSize));
			splitter = Helper.SetSize(splitter, splitterSize);
			if(Helper.IsContainerHorizontalRtl)
				splitter = Helper.GetRtlBounds(splitter);
			return splitter;
		}
		int GetOffset(int splitterPosition, int splitterSize) {
			if(Splitter.IsCollapsible && Splitter.IsCollapsed)
				return Splitter.IsPanel1Collapse ? 0 : Helper.GetSize(ClientRectangle) - splitterSize;
			return (!IsSplitFixedPanel2) ? splitterPosition : Helper.GetSize(ClientRectangle) - (splitterPosition + splitterSize);
		}
		bool IsSplitFixedPanel2 {
			get { return Container.FixedPanel == SplitFixedPanel.Panel2; }
		}
		int GetSplitPosition(int splitterSize) {
			var splitPosition = GetSplitPosition();
			if(Splitter.IsCollapsible && Splitter.IsCollapsed)
				splitPosition = CalcCollapsedPosition(splitterSize);
			return splitPosition;
		}
		int GetSplitPosition() {
			return GetSplitRange().CheckPosition(Container.GetSplitterPosition());
		}
		int CalcCollapsedPosition(int splitterSize) {
			return (Container.CollapsePanel == SplitCollapsePanel.Panel1) ?
				0 : Helper.GetSize(ClientRectangle) - splitterSize;
		}
		Rectangle CalcCollapseButton(Rectangle splitter) {
			if(!Splitter.IsCollapsible) return Rectangle.Empty;
			Size btnSize = new Size(
					IsHorizontal ? splitter.Width * 2 : splitter.Height,
					IsHorizontal ? splitter.Width : splitter.Height * 2
				);
			if((IsHorizontal && btnSize.Height < splitter.Height) || !IsHorizontal && btnSize.Width < splitter.Width) {
				return new Rectangle(
						splitter.Left + (splitter.Width - btnSize.Width) / 2,
						splitter.Top + (splitter.Height - btnSize.Height) / 2,
						btnSize.Width, btnSize.Height
					);
			}
			return Rectangle.Empty;
		}
		public GroupObjectInfoArgs Panel1Info { get { return panel1Info; } set { panel1Info = value; } }
		public GroupObjectInfoArgs Panel2Info { get { return panel2Info; } set { panel2Info = value; } }
		public SplitContainerSplitter Splitter { get { return splitter; } }
		public Rectangle Panel1Client { get { return Panel1Info.Bounds.IsEmpty ? Rectangle.Empty : Panel1Info.ControlClientBounds; } }
		public Rectangle Panel2Client { get { return Panel2Info.Bounds.IsEmpty ? Rectangle.Empty : Panel2Info.ControlClientBounds; } }
		public class SplitHelper {
			SplitContainerControl container;
			public SplitHelper(SplitContainerControl container) {
				this.container = container;
			}
			bool IsHorizontal { get { return container.Horizontal; } }
			public int GetSize(Size size) { return IsHorizontal ? size.Width : size.Height; }
			public int GetSize(Rectangle rect) { return GetSize(rect.Size); }
			public int GetNear(Point point) { return IsHorizontal ? point.X : point.Y; }
			public int GetNear(Rectangle rect) { return GetNear(rect.Location); }
			public int GetFar(Rectangle rect) { return GetNear(rect.Location) + GetSize(rect); }
			public Rectangle SetNear(Rectangle rect, int position) {
				if(IsHorizontal) rect.X = position;
				else rect.Y = position;
				return rect;
			}
			public Rectangle SetSize(Rectangle rect, int position) {
				if(IsHorizontal) rect.Width = position;
				else rect.Height = position;
				return rect;
			}
			internal bool IsContainerHorizontalRtl {
				get { return WindowsFormsSettings.GetRightToLeft(container) == RightToLeft.Yes && IsHorizontal; }
			}
			internal Rectangle GetRtlBounds(Rectangle rect) {
				Rectangle bounds = container.ClientRectangle;
				return new Rectangle(bounds.Right - (rect.Left - bounds.Left) - rect.Width, rect.Y, rect.Width, rect.Height);
			}
		}
		class PanelInfoHelper {
			int panel1Start, panel1Size, panel2Start, panel2Size;
			SplitContainerViewInfo viewInfo;
			int splitterSize, splitPosition;
			public int SplitPosition { get { return splitPosition; } }
			SplitContainerViewInfo ViewInfo { get { return viewInfo; } }
			SplitContainerControl Container { get { return ViewInfo.Container; } }
			SplitContainerSplitter Splitter { get { return ViewInfo.Splitter; } }
			bool IsCollapsibleAndCollapsed { get { return Splitter.IsCollapsible && Splitter.IsCollapsed; } }
			SplitHelper Helper { get { return ViewInfo.Helper; } }
			Rectangle ClientRectangle { get { return Container.ClientRectangle; } }
			public PanelInfoHelper(SplitContainerViewInfo viewInfo, int splitterSize, int splitterPosition) {
				this.viewInfo = viewInfo;
				this.splitterSize = splitterSize;
				this.splitPosition = splitterPosition;
				InitPanelInfo(splitterSize, splitterPosition);
			}
			void InitPanelInfo(int splitterSize, int splitPosition) {
				panel1Start = 0;
				if(IsCollapsibleAndCollapsed)
					InitPanelInfoForCollapsible(splitterSize);
				else
					InitPanelInfoForFixedPanel(splitterSize, splitPosition);
			}
			void InitPanelInfoForCollapsible(int splitterSize) {
				if(Splitter.IsPanel1Collapse) {
					panel1Size = 0;
					panel2Start = splitterSize;
					panel2Size = Math.Max(0, Helper.GetSize(ClientRectangle) - splitterSize);
				} else {
					panel1Size = Math.Max(0, Helper.GetSize(ClientRectangle) - splitterSize);
					panel2Start = Helper.GetSize(ClientRectangle);
					panel2Size = 0;
				}
			}
			void InitPanelInfoForFixedPanel(int splitterSize, int splitPosition) {
				if(Container.FixedPanel != SplitFixedPanel.Panel2) {
					panel1Size = splitPosition;
					panel2Start = splitPosition + splitterSize;
					panel2Size = Math.Max(0, Helper.GetSize(ClientRectangle) - panel2Start);
				} else {
					panel2Start = Math.Max(0, Helper.GetSize(ClientRectangle) - splitPosition);
					panel2Size = Helper.GetSize(ClientRectangle) - panel2Start;
					panel1Size = Math.Max(0, panel2Start - splitterSize);
				}
			}
			public Rectangle GetPanel1Bounds() {
				return GetPanelBounds(panel1Start, panel1Size);
			}
			public Rectangle GetPanel2Bounds() {
				return GetPanelBounds(panel2Start, panel2Size);
			}
			Rectangle GetPanelBounds(int start, int size) {
				Rectangle bounds = ClientRectangle;
				bounds = Helper.SetNear(bounds, start + Helper.GetNear(bounds));
				bounds = Helper.SetSize(bounds, size);
				if(Helper.GetSize(bounds) <= 0)
					bounds = Rectangle.Empty;
				if(Helper.IsContainerHorizontalRtl)
					bounds = Helper.GetRtlBounds(bounds);
				return bounds;
			}
			public Rectangle GetSplitterBounds(int splitterPosition) {
				return viewInfo.GetSplitterBounds(splitterPosition);
			}
		}
	}
}
