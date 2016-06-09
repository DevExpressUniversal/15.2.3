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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.ButtonsPanelControl;
using BaseHeaderButtonPreferredConstructor = DevExpress.XtraEditors.Controls.EditorButtonPreferredConstructorAttribute;
namespace DevExpress.Utils {
	[TypeConverter("DevExpress.Utils.Design.FlyoutPanelTypeConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public interface IFlyoutPanel {
		void ShowBeakForm(Point loc, bool immediate, Control ownerControl);
		void ShowBeakForm(Point loc, bool immediate, Control ownerControl, Point offset, IFlyoutPanelPopupController controller);
		void HideBeakForm(bool immediate);
		bool IsPopupOpen { get; }
		FlyoutPanelOptions Options { get; }
		BeakPanelOptions OptionsBeakPanel { get; }
	}
	public interface IFlyoutPanelPopupController {
		bool AllowCloseOnMouseMove(Rectangle formBounds, Point pt);
	}
	public enum FlyoutPanelButtonPanelLocation {
		Default,
		Top,
		Bottom
	}
	public enum BeakPanelBeakLocation {
		Default,
		Top,
		Bottom
	}
	public class PeekFormButton : ButtonControl, Utils.MVVM.ISupportCommandBinding {
		public PeekFormButton()
			: base() {
		}
		[BaseHeaderButtonPreferredConstructor]
		public PeekFormButton(string caption, Image image, bool useCaption, bool enabled, string toolTip)
			: base(caption, image, useCaption, enabled, toolTip) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public PeekFormButton(string caption, Image image, string imageUri, bool useCaption, bool enabled, string toolTip)
			: base(caption, image, imageUri, useCaption, enabled, toolTip) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public PeekFormButton(string caption, Image image, int imageIndex, ButtonStyle style, int groupIndex)
			: base(caption, image, imageIndex, style, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public PeekFormButton(string caption, string imageUri, ButtonStyle style, int groupIndex)
			: base(caption, imageUri, style, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public PeekFormButton(string caption, Image image)
			: base(caption, image) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public PeekFormButton(string caption, Image image, string imageUri)
			: base(caption, image, imageUri) {
		}
		public PeekFormButton(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, int groupIndex)
			: base(caption, image, imageIndex, imageLocation, style, groupIndex) {
		}
		public PeekFormButton(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, int groupIndex)
			: base(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, groupIndex) {
		}
		public PeekFormButton(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, int groupIndex)
			: base(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, groupIndex) {
		}
		public PeekFormButton(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, int groupIndex)
			: base(caption, imageUri, imageLocation, style, groupIndex) {
		}
		public PeekFormButton(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, int groupIndex)
			: base(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, groupIndex) {
		}
		public PeekFormButton(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, int groupIndex)
			: base(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public PeekFormButton(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex, bool isLeft)
			: base(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, glyphs, tag, groupIndex, isLeft) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public PeekFormButton(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex, bool isLeft)
			: base(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, glyphs, tag, groupIndex, isLeft) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public PeekFormButton(string caption, Image image, string imageUri, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex, bool isLeft)
			: base(caption, image, imageUri, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, glyphs, tag, groupIndex, isLeft) {
		}
		public event EventHandler Click;
		protected internal void RaiseClick() {
			EventHandler handler = Click;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		#region Commands
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				(button, canExecute) => button.Enabled = canExecute(),
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				(button, canExecute) => button.Enabled = canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				(button, canExecute) => button.Enabled = canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
	}
	public class FlyoutPanelPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			FlyoutPanelObjectInfoArgs args = (FlyoutPanelObjectInfoArgs)e;
			base.DrawObject(e);
			if(args.ViewInfo.ShouldDrawDesignerRect) {
				DrawDesignerRect(args);
				if(args.ViewInfo.ShouldDrawDTHint) DrawDesignTimeHint(args);
				if(args.ViewInfo.AllowButtonPanelDTRect) DrawButtonPanelDesignerRect(args);
			}
			if(args.ViewInfo.AllowButtonPanel) DrawButtonPanel(args);
		}
		protected virtual void DrawButtonPanelDesignerRect(FlyoutPanelObjectInfoArgs args) {
			using(Pen dashPen = new Pen(Color.Red)) {
				dashPen.DashStyle = DashStyle.DashDot;
				dashPen.DashPattern = new float[] { 4.0f, 4.0f };
				args.Cache.DrawRectangle(dashPen, args.ViewInfo.ButtonPanelDesignerRect);
			}
		}
		protected virtual void DrawDesignerRect(FlyoutPanelObjectInfoArgs args) {
			DXControlPaint.DrawDashedBorder(args.Cache.Graphics, args.ViewInfo.Owner, args.ViewInfo.PaintAppearance.BorderColor);
		}
		protected internal virtual void DrawDTHintIfNeeded(FlyoutPanelObjectInfoArgs args) {
			if(args.ViewInfo.ShouldDrawDTHint) DrawDesignTimeHint(args);
		}
		protected virtual void DrawDesignTimeHint(FlyoutPanelObjectInfoArgs args) {
			using(StringFormat format = new StringFormat()) {
				format.Alignment = format.LineAlignment = StringAlignment.Center;
				args.ViewInfo.PaintAppearance.DrawString(args.Cache, Properties.Resources.FlyoutPanelDesignTimeWarning, args.ViewInfo.DesignTimeHintBounds, format);
			}
		}
		protected virtual void DrawButtonPanel(FlyoutPanelObjectInfoArgs args) {
			ButtonsPanelControl buttonPanel = args.ViewInfo.ButtonPanel;
			if(buttonPanel == null || buttonPanel.ViewInfo == null) return;
			ObjectPainter.DrawObject(args.Cache, ((IButtonsPanelOwner)args.ViewInfo.Owner).GetPainter(), (ObjectInfoArgs)buttonPanel.ViewInfo);
		}
	}
	public class FlyoutPanelViewInfo {
		FlyoutPanel owner;
		Rectangle bounds, captionBounds, contentBounds;
		public FlyoutPanelViewInfo(FlyoutPanel owner) {
			this.owner = owner;
			this.bounds = this.captionBounds = this.contentBounds = Rectangle.Empty;
		}
		public virtual void CalcViewInfo(Graphics graphics, Rectangle bounds) {
			this.bounds = bounds;
			this.captionBounds = CalcCaptionBounds();
			this.contentBounds = CalcContentBounds(CaptionBounds);
			if(AllowButtonPanel) ButtonPanel.ViewInfo.Calc(graphics, ButtonPanelContentBounds);
		}
		public ButtonsPanelControl ButtonPanel { get { return Owner.ButtonPanel; } }
		protected virtual Rectangle CalcContentBounds(Rectangle captionBounds) {
			if(!Owner.OptionsButtonPanel.ShowButtonPanel) return Bounds;
			if(Owner.GetButtonPanelLocation() == FlyoutPanelButtonPanelLocation.Top) {
				return new Rectangle(Bounds.X, captionBounds.Bottom, Bounds.Width, Bounds.Height - captionBounds.Bottom);
			}
			return new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - captionBounds.Height);
		}
		protected virtual Rectangle CalcCaptionBounds() {
			Rectangle bounds = CalcCaptionBoundsCore();
			Padding padding = Owner.OptionsButtonPanel.Padding;
			bounds.X += padding.Left;
			bounds.Width -= (padding.Left + padding.Right);
			bounds.Y += padding.Top;
			bounds.Height -= (padding.Top + padding.Bottom);
			return bounds;
		}
		protected virtual Rectangle CalcCaptionBoundsCore() {
			if(!Owner.OptionsButtonPanel.ShowButtonPanel) return Rectangle.Empty;
			if(Owner.GetButtonPanelLocation() == FlyoutPanelButtonPanelLocation.Top) {
				return new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Owner.OptionsButtonPanel.ButtonPanelHeight);
			}
			int y = Math.Max(0, Bounds.Bottom - Owner.OptionsButtonPanel.ButtonPanelHeight);
			return new Rectangle(bounds.X, y, Bounds.Width, Owner.OptionsButtonPanel.ButtonPanelHeight);
		}
		public virtual Padding GetBestContentPaddings() {
			Padding paddings = Owner.Padding;
			if(Owner.GetButtonPanelLocation() == FlyoutPanelButtonPanelLocation.Top)
				paddings.Top = ContentBounds.Top;
			else paddings.Top = 0;
			if(Owner.GetButtonPanelLocation() == FlyoutPanelButtonPanelLocation.Bottom)
				paddings.Bottom = Math.Max(0, Bounds.Bottom - ContentBounds.Bottom);
			else paddings.Bottom = 0;
			return paddings;
		}
		public virtual void EnsureViewInfoReady() {
			if(!Owner.IsHandleCreated) return;
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				CalcViewInfo(ginfo.Graphics, Owner.ClientRectangle);
			}
			finally {
				ginfo.ReleaseGraphics();
			}
		}
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle CaptionBounds { get { return captionBounds; } }
		public Rectangle ContentBounds { get { return contentBounds; } }
		public Rectangle ButtonPanelContentBounds {
			get { return Rectangle.Inflate(CaptionBounds, -2, -2); }
		}
		public bool ShouldDrawDesignerRect {
			get { return Owner.DesignModeCore; }
		}
		public Rectangle DesignTimeHintBounds {
			get { return Rectangle.Inflate(ContentBounds, -2, -2); }
		}
		public Rectangle ButtonPanelDesignerRect {
			get { return Rectangle.Inflate(CaptionBounds, -2, -2); }
		}
		public AppearanceObject PaintAppearance { get { return Owner.Appearance; } }
		public bool ShouldDrawDTHint {
			get {
				if(!Owner.DesignModeCore) return false;
				return Owner.OwnerControl == null;
			}
		}
		public bool AllowButtonPanelDTRect {
			get {
				if(!Owner.DesignModeCore) return false;
				return AllowButtonPanel;
			}
		}
		public bool AllowButtonPanel {
			get {
				if(ButtonPanel == null) return false;
				return Owner.OptionsButtonPanel.ShowButtonPanel;
			}
		}
		public FlyoutPanel Owner { get { return owner; } }
	}
	public class FlyoutPanelObjectInfoArgs : ObjectInfoArgs {
		FlyoutPanelViewInfo viewInfo;
		public FlyoutPanelObjectInfoArgs(FlyoutPanelViewInfo viewInfo, GraphicsCache cache, Rectangle bounds)
			: base(cache) {
			this.viewInfo = viewInfo;
			this.Bounds = bounds;
		}
		public FlyoutPanelViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class FlyoutButtonPanelControl : ButtonsPanelControl {
		public FlyoutButtonPanelControl(IButtonsPanelOwner owner)
			: base(owner) {
		}
	}
	public class FlyoutButtonPanelControlSkinPainter : ButtonsPanelControlSkinPainter {
		public FlyoutButtonPanelControlSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new FlyoutButtonControlSkinPainter(Provider);
		}
	}
	public class FlyoutButtonControlSkinPainter : ButtonControlSkinPainter {
		public FlyoutButtonControlSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault appearance = base.DefaultAppearance;
				appearance.ForeColor = GetDefaultForeColor();
				return appearance;
			}
		}
		protected virtual Color GetDefaultForeColor() {
			SkinElement barElement = BarSkins.GetSkin(SkinProvider)[BarSkins.SkinBar];
			if(Info == null) return barElement.Color.GetForeColor();
			ObjectState state = Info.State;
			if(state == ObjectState.Normal) {
				Skin editorsSkin = EditorsSkins.GetSkin(SkinProvider);
				if(editorsSkin.Colors.Contains(EditorsSkins.OptButtonPanelNormalForeColor))
					return editorsSkin.Colors.GetColor(EditorsSkins.OptButtonPanelNormalForeColor);
				if(barElement.Properties.ContainsProperty(BarSkins.OptBarNoBorderForeColor))
					return barElement.Properties.GetColor(BarSkins.OptBarNoBorderForeColor);
			}
			if(state == ObjectState.Hot && barElement.Properties.ContainsProperty(BarSkins.OptBarNoBorderForeColorHot)) {
				return barElement.Properties.GetColor(BarSkins.OptBarNoBorderForeColorHot);
			}
			if(state == (ObjectState.Hot | ObjectState.Pressed) && barElement.Properties.ContainsProperty(BarSkins.OptBarNoBorderForeColorPressed)) {
				return barElement.Properties.GetColor(BarSkins.OptBarNoBorderForeColorPressed);
			}
			return barElement.Color.GetForeColor();
		}
		protected override SkinElement GetBackground() {
			SkinElement buttonPanelElement = EditorsSkins.GetSkin(SkinProvider)[EditorsSkins.SkinButtonPanel];
			return buttonPanelElement ?? BarSkins.GetSkin(SkinProvider)[BarSkins.SkinLinkSelected];
		}
		protected override void DrawStandartBackground(GraphicsCache cache, BaseButtonInfo info) {
			if(info.State == ObjectState.Normal) return;
			SkinElementInfo elementInfo = new SkinElementInfo(GetBackground(), info.Bounds);
			if(info.State.HasFlag(ObjectState.Pressed)) {
				elementInfo.ImageIndex = 1;
			}
			else if(info.State.HasFlag(ObjectState.Hot)) {
				elementInfo.ImageIndex = 0;
			}
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elementInfo);
		}
	}
	[
	Designer("DevExpress.Utils.Design.FlyoutPanelDesigner, " + AssemblyInfo.SRAssemblyDesignFull, typeof(IDesigner)),
	Description("A panel that is displayed and hidden using an animation effect."),
	ToolboxTabName(AssemblyInfo.DXTabCommon), DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxBitmap(typeof(ToolBoxIcons.ToolboxIconsRootNS), "FlyoutPanel")
	]
	public class FlyoutPanel : XtraUserControl, ISupportInitialize, IFlyoutPanel, IToolTipControlClient, IButtonsPanelOwnerEx, IButtonPanelControlAppearanceOwner, IButtonsPanelGlyphSkinningOwner {
		static readonly object showing = new object();
		static readonly object shown = new object();
		static readonly object hiding = new object();
		static readonly object hidden = new object();
		static readonly object buttonClick = new object();
		Control ownerControl;
		Form parentForm;
		FlyoutPanelOptions options;
		BeakPanelOptions optionsBeakPanel;
		ButtonsPanelControl buttonPanel;
		FlyoutPanelButtonOptions optionsButtonPanel;
		ToolTipController toolTipController;
		public FlyoutPanel() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			base.Visible = false;
			this.options = CreateFlyoutPanelOptions();
			this.optionsBeakPanel = CreateBeakPanelOptions();
			this.optionsButtonPanel = CreateButtonPanelOptions();
			this.buttonPanel = CreateButtonsPanel();
			this.toolTipController = null;
			LookAndFeel.StyleChanged += OnLookAndFeelStyleChanged;
			SubscribeButtonsPanel();
			ToolTipController.DefaultController.AddClientControl(this);
		}
		protected override Color GetBackColor(Color color, Color defColor) {
			if(color == Color.Empty)
				return defColor;
			return color;
		}
		void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			ButtonPanel.UpdateStyle();
		}
		protected virtual ButtonsPanelControl CreateButtonsPanel() {
			return new FlyoutButtonPanelControl(this);
		}
		protected virtual FlyoutPanelButtonOptions CreateButtonPanelOptions() {
			return new FlyoutPanelButtonOptions(this);
		}
		protected void SubscribeButtonsPanel() {
			ButtonPanel.ButtonClick += RaiseButtonClickInternal;
			ButtonPanel.Buttons.CollectionChanged += OnButtonPanelButtonsCollectionChanged;
		}
		protected void UnsubscribeButtonsPanel() {
			ButtonPanel.ButtonClick -= RaiseButtonClickInternal;
			ButtonPanel.Buttons.CollectionChanged -= OnButtonPanelButtonsCollectionChanged;
		}
		FlyoutPanelPainter panelPainter = null;
		protected FlyoutPanelPainter PanelPainter {
			get {
				if(panelPainter == null) panelPainter = CreatePanelPainter();
				return panelPainter;
			}
		}
		protected virtual FlyoutPanelPainter CreatePanelPainter() {
			return new FlyoutPanelPainter();
		}
		FlyoutPanelViewInfo panelViewInfo = null;
		protected FlyoutPanelViewInfo PanelViewInfo {
			get {
				if(panelViewInfo == null) panelViewInfo = CreatePanelViewInfo();
				return panelViewInfo;
			}
		}
		protected virtual FlyoutPanelViewInfo CreatePanelViewInfo() {
			return new FlyoutPanelViewInfo(this);
		}
		protected internal FlyoutPanelViewInfo GetViewInfo() { return PanelViewInfo; }
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault appDefault = base.CreateDefaultAppearance();
			appDefault.BackColor = Color.Empty;
			return appDefault;
		}
		protected virtual FlyoutPanelOptions CreateFlyoutPanelOptions() {
			return new FlyoutPanelOptions();
		}
		protected virtual BeakPanelOptions CreateBeakPanelOptions() {
			return new BeakPanelOptions(this);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		new public void Show() { }
		[EditorBrowsable(EditorBrowsableState.Never)]
		new public void Hide() { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AnchorStyles Anchor {
			get { return base.Anchor; }
			set { base.Anchor = value; }
		}
		[Localizable(true)]
		public override Size MinimumSize {
			get { return base.MinimumSize; }
			set { base.MinimumSize = value; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			UpdateViewInfo(e.Graphics);
			using(GraphicsCache cache = new GraphicsCache(e)) {
				FlyoutPanelObjectInfoArgs info = new FlyoutPanelObjectInfoArgs(PanelViewInfo, cache, PanelViewInfo.Bounds);
				PanelPainter.DrawObject(info);
			}
		}
		protected virtual void UpdateViewInfo(Graphics graphics) {
			if(!IsHandleCreated || IsDisposed) return;
			PanelViewInfo.CalcViewInfo(graphics, ClientRectangle);
		}
		[DefaultValue(null), 
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelOwnerControl"),
#endif
 TypeConverter("DevExpress.Utils.Design.FlyoutPanelOwnerControlPropertyConverter, " + AssemblyInfo.SRAssemblyDesignFull), DXCategory(CategoryName.Options), RefreshProperties(RefreshProperties.All)]
		public Control OwnerControl {
			get { return ownerControl; }
			set {
				if(OwnerControl == value)
					return;
				ownerControl = value;
				OnOwnerControlChanged();
			}
		}
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null), 
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelParentForm"),
#endif
 DXCategory(CategoryName.Options)]
		public new Form ParentForm {
			get { return parentForm; }
			set {
				if(ParentForm == value)
					return;
				parentForm = value;
				OnParentFormChanged();
			}
		}
		[Category("Appearance"), 
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelToolTipController"),
#endif
 DefaultValue(null)]
		public virtual ToolTipController ToolTipController {
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
		protected virtual void OnParentFormChanged() {
			OnPropertyChanged();
		}
		protected virtual void OnOwnerControlChanged() {
			if(DesignMode) UpdateContentPanel();
			OnPropertyChanged();
		}
		protected virtual void UpdateContentPanel() {
			FlyoutPanelControl panel = GetContentPanel();
			if(panel != null) panel.Invalidate();
		}
		protected FlyoutPanelControl GetContentPanel() {
			foreach(Control directChild in Controls) {
				if(directChild is FlyoutPanelControl) return (FlyoutPanelControl)directChild;
			}
			return null;
		}
		protected internal FlyoutPanelButtonPanelLocation GetButtonPanelLocation() {
			if(OptionsButtonPanel.ButtonPanelLocation == FlyoutPanelButtonPanelLocation.Default) return FlyoutPanelButtonPanelLocation.Top;
			return OptionsButtonPanel.ButtonPanelLocation;
		}
		protected internal BeakPanelBeakLocation GetBeakLocation() {
			if(OptionsBeakPanel.BeakLocation == BeakPanelBeakLocation.Default) return BeakPanelBeakLocation.Bottom;
			return OptionsBeakPanel.BeakLocation;
		}
		protected internal virtual void OnPropertyChanged() {
			Invalidate();
		}
		protected virtual bool AllowAdjustContentPaddings { get { return true; } }
		protected virtual void UpdateContentPaddings() {
			if(!IsHandleCreated || !AllowAdjustContentPaddings) return;
			PanelViewInfo.EnsureViewInfoReady();
			Padding padding = PanelViewInfo.GetBestContentPaddings();
			if(Padding != padding) Padding = padding;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Visible {
			get { return FlyoutPanelState.IsActive; }
			set { base.Visible = value; }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelOptions"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Options)]
		public FlyoutPanelOptions Options {
			get { return options; }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelOptionsBeakPanel"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BeakPanelOptions OptionsBeakPanel {
			get { return optionsBeakPanel; }
		}
		bool ShouldSerializeOptionsBeakPanel() { return OptionsBeakPanel.ShouldSerialize(); }
		void ResetOptionsBeakPanel() { OptionsBeakPanel.Reset(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelOptionsButtonPanel"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FlyoutPanelButtonOptions OptionsButtonPanel {
			get { return optionsButtonPanel; }
		}
		bool ShouldSerializeOptionsButtonPanel() { return OptionsButtonPanel.ShouldSerialize(); }
		void ResetOptionsButtonPanel() { OptionsButtonPanel.Reset(); }
		protected virtual void SubscribeToolFormEvents(FlyoutPanelToolForm toolForm) {
			toolForm.StartAnimation += OnToolFormStartAnimationRaised;
			toolForm.EndAnimation += OnToolFormEndAnimationRaised;
		}
		protected virtual void UnsubscribeToolFormEvents(FlyoutPanelToolForm toolForm) {
			toolForm.StartAnimation -= OnToolFormStartAnimationRaised;
			toolForm.EndAnimation -= OnToolFormEndAnimationRaised;
		}
		protected virtual void OnToolFormStartAnimationRaised(object sender, PopupToolWindowAnimationEventArgs e) {
			FlyoutPanelEventArgs ee = CreateFlyoutPanelEventArgs();
			if(e.IsShowing) {
				OnShowing(ee);
			}
			else {
				OnHiding(ee);
			}
		}
		protected virtual void OnToolFormEndAnimationRaised(object sender, PopupToolWindowAnimationEventArgs e) {
			FlyoutPanelEventArgs ee = CreateFlyoutPanelEventArgs();
			FlyoutPanelToolForm toolForm = (FlyoutPanelToolForm)sender;
			if(e.IsShowing) {
				OnShown(ee);
			}
			else {
				OnHidden(ee);
				UnsubscribeToolFormEvents(toolForm);
				ResetCurrentObjectInfo();
			}
		}
		protected virtual FlyoutPanelEventArgs CreateFlyoutPanelEventArgs() {
			return new FlyoutPanelEventArgs();
		}
		protected virtual FlyoutPanelButtonClickEventArgs CreateFlyoutPanelButtonClickEventArgs(PeekFormButton button) {
			return new FlyoutPanelButtonClickEventArgs(button);
		}
		#region Handlers
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!DesignMode) ButtonPanel.Handler.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!DesignMode) ButtonPanel.Handler.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!DesignMode) ButtonPanel.Handler.OnMouseUp(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(!DesignMode) ButtonPanel.Handler.OnMouseLeave();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			UpdateButtonPanel();
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			UpdateButtonPanel();
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			UpdateButtonPanel();
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			UpdateButtonPanel();
		}
		#endregion
		#region Events
		[
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelShowing"),
#endif
 DXCategory(CategoryName.Events)]
		public event FlyoutPanelEventHandler Showing {
			add { Events.AddHandler(showing, value); }
			remove { Events.RemoveHandler(showing, value); }
		}
		protected virtual void OnShowing(FlyoutPanelEventArgs e) {
			FlyoutPanelEventHandler handler = (FlyoutPanelEventHandler)Events[showing];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelShown"),
#endif
 DXCategory(CategoryName.Events)]
		public event FlyoutPanelEventHandler Shown {
			add { Events.AddHandler(shown, value); }
			remove { Events.RemoveHandler(shown, value); }
		}
		protected virtual void OnShown(FlyoutPanelEventArgs e) {
			FlyoutPanelEventHandler handler = (FlyoutPanelEventHandler)Events[shown];
			if(handler != null) handler(this, e);
		}
		[DXCategory(CategoryName.Events), 
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelHiding")
#else
	Description("")
#endif
]
		public event FlyoutPanelEventHandler Hiding {
			add { Events.AddHandler(hiding, value); }
			remove { Events.RemoveHandler(hiding, value); }
		}
		protected virtual void OnHiding(FlyoutPanelEventArgs e) {
			FlyoutPanelEventHandler handler = (FlyoutPanelEventHandler)Events[hiding];
			if(handler != null) handler(this, e);
		}
		[DXCategory(CategoryName.Events), 
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelHidden")
#else
	Description("")
#endif
]
		public event FlyoutPanelEventHandler Hidden {
			add { Events.AddHandler(hidden, value); }
			remove { Events.RemoveHandler(hidden, value); }
		}
		protected virtual void OnHidden(FlyoutPanelEventArgs e) {
			FlyoutPanelEventHandler handler = (FlyoutPanelEventHandler)Events[hidden];
			if(handler != null) handler(this, e);
		}
		[DXCategory(CategoryName.Events), 
#if !SL
	DevExpressUtilsLocalizedDescription("FlyoutPanelButtonClick")
#else
	Description("")
#endif
]
		public event FlyoutPanelButtonClickEventHandler ButtonClick {
			add { Events.AddHandler(buttonClick, value); }
			remove { Events.RemoveHandler(buttonClick, value); }
		}
		protected virtual void OnButtonClick(FlyoutPanelButtonClickEventArgs e) {
			FlyoutPanelButtonClickEventHandler handler = (FlyoutPanelButtonClickEventHandler)Events[buttonClick];
			if(handler != null) handler(this, e);
		}
		#endregion
		protected virtual bool ShouldSerializeOptions() {
			return Options.ShouldSerialize();
		}
		protected virtual void ResetOptions() {
			Options.Reset();
		}
		ToolFormObjectInfo currentObjectInfo;
		public void ShowPopup() {
			ShowPopup(false);
		}
		public void ShowPopup(bool immediate) {
			if(!FlyoutPanelState.IsNull) return;
			CheckShowState();
			ToolFormObjectInfo formInfo = new ToolFormObjectInfo(CreateToolFormCore(OwnerControl, this, Options), this, immediate);
			AssignCurrentObjectInfo(formInfo);
			ShowToolFormCore(formInfo, immediate);
		}
		public void HidePopup() {
			HidePopup(false);
		}
		public void HidePopup(bool immediate) {
			ToolFormObjectInfo si = FlyoutPanelState;
			if(si.IsNull) return;
			if(si.IsHiding) {
				if(immediate && !si.Immediate) {
					si.Form.Visible = false;
					HideToolFormCore(immediate, si);
				}
				return;
			}
			si.SetHidingFlag();
			si.SetMode(immediate);
			HideToolFormCore(immediate, si);
		}
		[Browsable(false)]
		public bool IsPopupOpen { get { return FlyoutPanelState.IsActive; } }
		[Browsable(false)]
		public ToolFormObjectInfo FlyoutPanelState {
			get { return GetCurrentObjectInfo(); }
		}
		static ToolFormObjectInfo EmptyFlyoutPanelState = new NullToolFormObjectInfo();
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(!DesignMode && FlyoutPanelState.IsActive) FlyoutPanelState.Form.OnContentSizeChanged();
		}
		#region BeakForm
		protected virtual Control GetBeakOwnerControl() {
			if(OwnerControl != null) return OwnerControl;
			return OwnerControl = FindForm();
		}
		public void ShowBeakForm(Point loc) {
			ShowBeakForm(loc, false);
		}
		public void ShowBeakForm(Point loc, bool immediate) {
			ShowBeakForm(loc, immediate, GetBeakOwnerControl());
		}
		public void ShowBeakForm(Point loc, bool immediate, Control ownerControl) {
			ShowBeakForm(loc, immediate, ownerControl, Point.Empty);
		}
		public void ShowBeakForm() { ShowBeakForm(false); }
		public void ShowBeakForm(bool immediate) {
			Control ownerCore = GetBeakOwnerControl();
			bool isTopControl = object.ReferenceEquals(ownerCore, ownerCore.TopLevelControl);
			Point loc = Point.Empty;
			if(isTopControl)
				loc = new Point(ownerCore.Location.X + ownerCore.Width / 2, ownerCore.Location.Y + ownerCore.Height / 2);
			else {
				Point ownerStartPt = ownerCore.PointToScreen(Point.Empty);
				loc = new Point(ownerStartPt.X + ownerCore.Width / 2, ownerStartPt.Y + ownerCore.Height / 2);
			}
			ShowBeakForm(loc, immediate, ownerCore, new Point(0, ownerCore.Height / 2));
		}
		public void ShowBeakForm(Point loc, bool immediate, Control ownerControl, Point offset) {
			ShowBeakForm(loc, immediate, ownerControl, offset, null);
		}
		public void ShowBeakForm(Point loc, bool immediate, Control ownerControl, Point offset, IFlyoutPanelPopupController controller) {
			if(!FlyoutPanelState.IsNull) return;
			ToolFormObjectInfo formInfo = new ToolFormObjectInfo(CreateBeakFormCore(ownerControl, this, loc, offset), this, immediate, controller);
			AssignCurrentObjectInfo(formInfo);
			ShowToolFormCore(formInfo, immediate);
		}
		public void HideBeakForm() {
			HideBeakForm(false);
		}
		public void HideBeakForm(bool immediate) {
			HidePopup(immediate);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ToolFormObjectInfo GetCurrentObjectInfo() {
			ToolFormObjectInfo inf = this.currentObjectInfo;
			if(inf == null) {
				inf = EmptyFlyoutPanelState;
			}
			return inf;
		}
		protected virtual void CheckShowState() {
			if(OwnerControl == null) {
				throw new InvalidOperationException("OwnerControl is not specified");
			}
		}
		protected virtual FlyoutPanelToolForm CreateToolFormCore(Control owner, FlyoutPanel content, FlyoutPanelOptions options) {
			return new FlyoutPanelToolForm(owner, content, options);
		}
		protected virtual FlyoutPanelToolForm CreateBeakFormCore(Control owner, FlyoutPanel content, Point loc, Point offset) {
			BeakFlyoutPanelOptions beakOptions = new BeakFlyoutPanelOptions(loc);
			beakOptions.AnchorType = PopupToolWindowAnchor.Manual;
			beakOptions.AnimationType = PopupToolWindowAnimation.Fade;
			beakOptions.CloseOnOuterClick = OptionsBeakPanel.CloseOnOuterClick;
			beakOptions.Offset = offset;
			return CreateFlyoutPanelBeakFormInstance(owner, content, beakOptions);
		}
		protected virtual FlyoutPanelToolForm CreateFlyoutPanelBeakFormInstance(Control owner, FlyoutPanel content, BeakFlyoutPanelOptions beakOptions) {
			return new FlyoutPanelBeakForm(owner, content, beakOptions);
		}
		protected virtual void AssignCurrentObjectInfo(ToolFormObjectInfo formInfo) {
			this.currentObjectInfo = formInfo;
		}
		protected virtual void ResetCurrentObjectInfo() {
			this.currentObjectInfo = null;
		}
		protected virtual void ShowToolFormCore(ToolFormObjectInfo formInfo, bool immediate) {
			formInfo.Form.ApplyOptions();
			SubscribeToolFormEvents(formInfo.Form);
			formInfo.Form.ShowToolWindow(immediate);
		}
		protected virtual void HideToolFormCore(bool immediate, ToolFormObjectInfo formInfo) {
			formInfo.Form.HideToolWindow(immediate);
		}
		protected internal virtual void OnCancelShowCore() {
			ResetCurrentObjectInfo();
		}
		protected internal virtual void OnContainerDisposing() {
			if(FlyoutPanelState.IsHiding) HidePopup(true);
		}
		protected internal virtual bool AllowCloseOnMouseMoveCore(Rectangle formBounds, Point pt) {
			if(!IsPopupOpen) return false;
			IFlyoutPanelPopupController controller = FlyoutPanelState.Controller;
			return controller != null ? controller.AllowCloseOnMouseMove(formBounds, pt) : false;
		}
		protected internal void UpdateButtonPanel() {
			if(ButtonPanel == null) return;
			ButtonPanel.ViewInfo.SetDirty();
			UpdateContentPaddings();
			Invalidate();
			Update();
		}
		protected virtual void OnBeginInitCore() { }
		protected virtual void OnEndInitCore() {
			EnsureCreateHandle();
			UpdateContentPaddings();
		}
		protected virtual void EnsureCreateHandle() {
			IntPtr handle = Handle;
		}
		protected internal ButtonsPanelControl ButtonPanel { get { return buttonPanel; } }
		#region ButtonPanel Handlers
		protected virtual void RaiseButtonClickInternal(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e) {
			PeekFormButton button = e.Button as PeekFormButton;
			OnButtonClick(CreateFlyoutPanelButtonClickEventArgs(button));
			if(button != null) button.RaiseClick();
		}
		void OnButtonPanelButtonsCollectionChanged(object sender, CollectionChangeEventArgs e) {
			OnButtonPanelButtonsCollectionChangedCore();
		}
		protected virtual void OnButtonPanelButtonsCollectionChangedCore() {
			UpdateButtonPanel();
			OnPropertyChanged();
		}
		#endregion
		#region IButtonsPanelOwnerEx
		public Color GetGlyphSkinningColor(BaseButtonInfo info) { return ForeColor; }
		Padding IButtonsPanelOwnerEx.ButtonBackgroundImageMargin {
			get { return Padding.Empty; }
			set { }
		}
		bool IButtonsPanelOwnerEx.CanPerformClick(IBaseButton button) {
			return true;
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return false; }
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return OptionsButtonPanel.AllowGlyphSkinning; }
		}
		void IButtonsPanelOwner.Invalidate() {
			if(ButtonPanel != null && ButtonPanel.ViewInfo != null)
				Invalidate(ButtonPanel.ViewInfo.Bounds);
		}
		ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return OptionsButtonPanel.AppearanceButton; }
		}
		object IButtonsPanelOwner.Images { get { return OptionsButtonPanel.Images; } }
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new FlyoutButtonPanelControlSkinPainter(LookAndFeel);
			return new ButtonsPanelControlPainter();
		}
		bool IButtonsPanelOwner.IsSelected { get { return false; } }
		#endregion
		#region IButtonPanelControlAppearanceOwner
		IButtonsPanelControlAppearanceProvider IButtonPanelControlAppearanceOwner.CreateAppearanceProvider() {
			return new ButtonsPanelControlAppearanceProvider();
		}
		bool IAppearanceOwner.IsLoading { get { return false; } }
		#endregion
		#region ISupportInitialize
		void ISupportInitialize.BeginInit() {
			OnBeginInitCore();
		}
		void ISupportInitialize.EndInit() {
			OnEndInitCore();
		}
		#endregion
		#region IToolTipControlClient
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return ButtonPanel.GetObjectInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return OptionsButtonPanel.ShowButtonPanel; } }
		#endregion
		protected internal bool DesignModeCore { get { return DesignMode; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(LookAndFeel != null) LookAndFeel.StyleChanged -= OnLookAndFeelStyleChanged;
				UnsubscribeButtonsPanel();
				ToolTipController = null;
				ToolTipController.DefaultController.RemoveClientControl(this);
			}
			base.Dispose(disposing);
		}
	}
	[ToolboxItem(false)]
	public class FlyoutPanelControl : PanelControl {
		FlyoutPanel flyoutPanel;
		public FlyoutPanelControl()
			: this(null) {
		}
		public FlyoutPanelControl(FlyoutPanel flyoutPanel) {
			this.FlyoutPanel = flyoutPanel;
		}
		protected override void DrawPanel(GraphicsCache cache, GroupObjectInfoArgs info) {
			base.DrawPanel(cache, info);
			DrawDTHintIfNeeded(cache, info);
		}
		protected virtual void DrawDTHintIfNeeded(GraphicsCache cache, GroupObjectInfoArgs info) {
			if(FlyoutPanel == null || !DesignMode) return;
			new FlyoutPanelPainter().DrawDTHintIfNeeded(new FlyoutPanelObjectInfoArgs(FlyoutPanel.GetViewInfo(), cache, ClientRectangle));
		}
		[DXCategory(CategoryName.Options)]
		public FlyoutPanel FlyoutPanel {
			get { return flyoutPanel; }
			set {
				if(FlyoutPanel == value)
					return;
				flyoutPanel = value;
				OnFlyoutPanelChanged();
			}
		}
		protected virtual void OnFlyoutPanelChanged() {
			Invalidate();
		}
		protected override void Dispose(bool disposing) {
			FlyoutPanel = null;
			if(disposing) {
			}
			base.Dispose(disposing);
		}
	}
	public delegate void FlyoutPanelEventHandler(object sender, FlyoutPanelEventArgs e);
	public class FlyoutPanelEventArgs : EventArgs {
		public FlyoutPanelEventArgs() {
		}
	}
	public delegate void FlyoutPanelButtonClickEventHandler(object sender, FlyoutPanelButtonClickEventArgs e);
	public class FlyoutPanelButtonClickEventArgs : EventArgs {
		PeekFormButton button;
		public FlyoutPanelButtonClickEventArgs(PeekFormButton button) {
			this.button = button;
		}
		public PeekFormButton Button { get { return button; } }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FlyoutPanelOptions {
		public FlyoutPanelOptions() {
			Reset();
		}
		public int HorzIndent {
			get;
			set;
		}
		bool ShouldSerializeHorzIndent() { return HorzIndent != DefaultHorzIndent; }
		void ResetHorzIndent() { HorzIndent = DefaultHorzIndent; }
		public int VertIndent {
			get;
			set;
		}
		bool ShouldSerializeVertIndent() { return VertIndent != DefaultVertIndent; }
		void ResetVertIndent() { VertIndent = DefaultVertIndent; }
		public PopupToolWindowAnchor AnchorType {
			get;
			set;
		}
		bool ShouldSerializeAnchorType() { return AnchorType != DefaultAnchorType; }
		void ResetAnchorType() { AnchorType = DefaultAnchorType; }
		public PopupToolWindowAnimation AnimationType {
			get;
			set;
		}
		bool ShouldSerializeAnimationType() { return AnimationType != DefaultAnimationType; }
		void ResetAnimationType() { AnimationType = DefaultAnimationType; }
		public bool CloseOnOuterClick {
			get;
			set;
		}
		bool ShouldSerializeCloseOnOuterClick() { return CloseOnOuterClick != DefaultCloseOnOuterClick; }
		void ResetCloseOnOuterClick() { CloseOnOuterClick = DefaultCloseOnOuterClick; }
		public bool CloseOnHidingOwner {
			get;
			set;
		}
		bool ShouldSerializeCloseOnHidingOwner() { return CloseOnHidingOwner != DefaultCloseOnHidingOwner; }
		void ResetCloseOnHidingOwner() { CloseOnHidingOwner = DefaultCloseOnHidingOwner; }
		protected internal virtual bool ShouldSerialize() {
			if(HorzIndent != DefaultHorzIndent)
				return true;
			if(VertIndent != DefaultVertIndent)
				return true;
			if(AnchorType != DefaultAnchorType)
				return true;
			if(AnimationType != DefaultAnimationType)
				return true;
			if(Location != DefaultLocation)
				return true;
			if(CloseOnOuterClick != DefaultCloseOnOuterClick)
				return true;
			if(CloseOnHidingOwner != DefaultCloseOnHidingOwner)
				return true;
			return false;
		}
		public Point Location {
			get;
			set;
		}
		bool ShouldSerializeLocation() { return Location != DefaultLocation; }
		void ResetLocation() { Location = DefaultLocation; }
		protected internal bool IsResizeSuppressed {
			get { return AnchorType == PopupToolWindowAnchor.Center || AnchorType == PopupToolWindowAnchor.TopLeft || AnchorType == PopupToolWindowAnchor.TopRight || AnchorType == PopupToolWindowAnchor.Manual; }
		}
		protected internal virtual void Reset() {
			HorzIndent = DefaultHorzIndent;
			VertIndent = DefaultVertIndent;
			AnchorType = DefaultAnchorType;
			AnimationType = DefaultAnimationType;
			Location = DefaultLocation;
			CloseOnOuterClick = DefaultCloseOnOuterClick;
			CloseOnHidingOwner = DefaultCloseOnHidingOwner;
		}
		public static readonly int DefaultHorzIndent = 0;
		public static readonly int DefaultVertIndent = 0;
		public static readonly Point DefaultLocation = Point.Empty;
		public static readonly bool DefaultCloseOnOuterClick = false;
		public static readonly bool DefaultCloseOnHidingOwner = true;
		public static readonly PopupToolWindowAnchor DefaultAnchorType = PopupToolWindowAnchor.Top;
		public static readonly PopupToolWindowAnimation DefaultAnimationType = PopupToolWindowAnimation.Slide;
	}
	public class BeakPanelOptions : BaseOptions {
		FlyoutPanel owner;
		Color borderColor;
		bool closeOnOuterClick;
		BeakPanelBeakLocation beakLocation;
		double opacity;
		public BeakPanelOptions(FlyoutPanel owner) {
			this.owner = owner;
			this.borderColor = Color.Empty;
			this.opacity = 1d;
			this.closeOnOuterClick = true;
			this.beakLocation = BeakPanelBeakLocation.Default;
		}
		public virtual Color BorderColor {
			get { return borderColor; }
			set {
				if(BorderColor == value)
					return;
				Color prev = BorderColor;
				borderColor = value;
				OnChanged("BorderColor", prev, BorderColor);
			}
		}
		bool ShouldSerializeBorderColor() { return BorderColor != Color.Empty; }
		void ResetBorderColor() { BorderColor = Color.Empty; }
		public virtual Color BackColor {
			get { return Owner.BackColor; }
			set {
				if(Owner.BackColor == value)
					return;
				Color prev = Owner.BackColor;
				Owner.BackColor = value;
				OnChanged("BackColor", prev, Owner.BackColor);
			}
		}
		bool ShouldSerializeBackColor() { return BackColor != Color.Empty; }
		void ResetBackColor() { BackColor = Color.Empty; }
		[ DefaultValue(true)]
		public virtual bool CloseOnOuterClick {
			get { return closeOnOuterClick; }
			set {
				if(CloseOnOuterClick == value)
					return;
				bool prev = CloseOnOuterClick;
				closeOnOuterClick = value;
				OnChanged("CloseOnOuterClick", prev, CloseOnOuterClick);
			}
		}
		[ DefaultValue(BeakPanelBeakLocation.Default)]
		public virtual BeakPanelBeakLocation BeakLocation {
			get { return beakLocation; }
			set {
				if(BeakLocation == value)
					return;
				BeakPanelBeakLocation prev = BeakLocation;
				beakLocation = value;
				OnChanged("BeakLocation", prev, BeakLocation);
			}
		}
		[ DefaultValue((double)1.0), TypeConverter(typeof(OpacityConverter))]
		public virtual double Opacity {
			get { return opacity; }
			set {
				if(IsEquals(Opacity, value))
					return;
				double prev = Opacity;
				opacity = value;
				OnChanged("Opacity", prev, Opacity);
			}
		}
		protected virtual bool ShouldSerializeOpacity() {
			return !IsEquals(1, Opacity);
		}
		protected virtual void ResetOpacity() {
			Opacity = 1;
		}
		protected bool IsEquals(double x, double y) {
			return Math.Abs(x - y) < 0.0001;
		}
		protected override void RaiseOnChanged(BaseOptionChangedEventArgs e) {
			if(Owner != null) Owner.OnPropertyChanged();
			base.RaiseOnChanged(e);
		}
		protected FlyoutPanel Owner { get { return owner; } }
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			BeakPanelOptions opt = options as BeakPanelOptions;
			if(opt == null) return;
			BeginUpdate();
			try {
				BorderColor = opt.BorderColor;
				BackColor = opt.BackColor;
				CloseOnOuterClick = opt.CloseOnOuterClick;
				BeakLocation = opt.BeakLocation;
				Opacity = opt.Opacity;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal new bool ShouldSerialize() {
			if(BorderColor != Color.Empty || BackColor != Color.Empty) return true;
			if(!CloseOnOuterClick) return true;
			if(BeakLocation != BeakPanelBeakLocation.Default) return true;
			if(ShouldSerializeOpacity()) return true;
			return false;
		}
		public override void Reset() {
			BorderColor = BackColor = Color.Empty;
			CloseOnOuterClick = true;
			BeakLocation = BeakPanelBeakLocation.Default;
			ResetOpacity();
		}
	}
	public class FlyoutPanelButtonOptions : BaseOptions {
		FlyoutPanel owner;
		bool allowGlyphSkinning, showButtonPanel;
		int buttonPanelHeight;
		object imagesCore;
		Padding padding;
		FlyoutPanelButtonPanelLocation buttonPanelLocation;
		ButtonsPanelControlAppearance buttonAppearanceCore;
		const int ButtonPanelHeightDefault = 30;
		public FlyoutPanelButtonOptions(FlyoutPanel owner) {
			this.owner = owner;
			this.allowGlyphSkinning = this.showButtonPanel = false;
			this.buttonAppearanceCore = null;
			this.imagesCore = null;
			this.padding = Padding.Empty;
			this.buttonPanelHeight = ButtonPanelHeightDefault;
			this.buttonPanelLocation = FlyoutPanelButtonPanelLocation.Default;
		}
		[ DefaultValue(false), DXCategory(CategoryName.Appearance)]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				bool prev = AllowGlyphSkinning;
				allowGlyphSkinning = value;
				OnChanged("AllowGlyphSkinning", prev, AllowGlyphSkinning);
			}
		}
		[ DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ButtonsPanelControlAppearance AppearanceButton {
			get {
				if(buttonAppearanceCore == null) {
					buttonAppearanceCore = new ButtonsPanelControlAppearance(Owner);
					buttonAppearanceCore.Changed += OnButtonAppearanceChanged;
				}
				return buttonAppearanceCore;
			}
		}
		protected virtual void OnButtonAppearanceChanged(object sender, EventArgs e) {
			Owner.UpdateButtonPanel();
		}
		[ DefaultValue(ContentAlignment.MiddleRight), DXCategory(CategoryName.Appearance)]
		public ContentAlignment ButtonPanelContentAlignment {
			get { return Owner.ButtonPanel.ContentAlignment; }
			set {
				if(Owner.ButtonPanel.ContentAlignment == value)
					return;
				ContentAlignment prev = ButtonPanelContentAlignment;
				Owner.ButtonPanel.ContentAlignment = value;
				OnChanged("ButtonPanelContentAlignment", prev, ButtonPanelContentAlignment);
			}
		}
		[ DefaultValue(ButtonPanelHeightDefault), DXCategory(CategoryName.Appearance)]
		public int ButtonPanelHeight {
			get { return buttonPanelHeight; }
			set {
				if(ButtonPanelHeight == value)
					return;
				int prev = ButtonPanelHeight;
				buttonPanelHeight = value;
				Owner.UpdateButtonPanel();
				OnChanged("ButtonPanelHeight", prev, ButtonPanelHeight);
			}
		}
		[ DefaultValue(FlyoutPanelButtonPanelLocation.Default), DXCategory(CategoryName.Appearance)]
		public FlyoutPanelButtonPanelLocation ButtonPanelLocation {
			get { return buttonPanelLocation; }
			set {
				if(ButtonPanelLocation == value)
					return;
				FlyoutPanelButtonPanelLocation prev = ButtonPanelLocation;
				buttonPanelLocation = value;
				Owner.UpdateButtonPanel();
				OnChanged("ButtonPanelLocation", prev, ButtonPanelLocation);
			}
		}
		[ DefaultValue(false), DXCategory(CategoryName.Appearance)]
		public bool ShowButtonPanel {
			get { return showButtonPanel; }
			set {
				if(ShowButtonPanel == value)
					return;
				bool prev = ShowButtonPanel;
				showButtonPanel = value;
				Owner.UpdateButtonPanel();
				OnChanged("ShowButtonPanel", prev, ShowButtonPanel);
			}
		}
		[ DXCategory(CategoryName.Appearance), Localizable(true)]
		public Padding Padding {
			get { return padding; }
			set {
				if(Padding == value)
					return;
				Padding prev = Padding;
				padding = value;
				Owner.UpdateButtonPanel();
				OnChanged("Padding", prev, Padding);
			}
		}
		bool ShouldSerializePadding() { return Padding != Padding.Empty; }
		void ResetPadding() { Padding = Padding.Empty; }
		[ DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ListBindable(false), TypeConverter(typeof(UniversalCollectionTypeConverter)), Editor("DevExpress.XtraEditors.Design.FlyoutButtonsPanelControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), Localizable(true)]
		public BaseButtonCollection Buttons {
			get { return Owner.ButtonPanel.Buttons; }
		}
		[ DefaultValue(null), DXCategory(CategoryName.Appearance), TypeConverter(typeof(ImageCollectionImagesConverter))]
		public object Images {
			get { return imagesCore; }
			set {
				if(Images == value) return;
				object prev = Images;
				imagesCore = value;
				Owner.UpdateButtonPanel();
				OnChanged("Images", prev, Images);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShouldSerialize() { return base.ShouldSerialize(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void Reset() { base.Reset(); }
		protected override void RaiseOnChanged(BaseOptionChangedEventArgs e) {
			if(Owner != null) Owner.OnPropertyChanged();
			base.RaiseOnChanged(e);
		}
		protected internal FlyoutPanel Owner { get { return owner; } }
	}
	public class ToolFormObjectInfo {
		bool hiding, immediate;
		Control contentControl;
		Control realOwnerControl;
		FlyoutPanelToolForm form;
		IFlyoutPanelPopupController controller;
		public ToolFormObjectInfo(FlyoutPanelToolForm form, Control contentControl, bool immediate)
			: this(form, contentControl, immediate, null) {
		}
		public ToolFormObjectInfo(FlyoutPanelToolForm form, Control contentControl, bool immediate, IFlyoutPanelPopupController controller) {
			this.hiding = false;
			this.immediate = immediate;
			this.form = form;
			this.contentControl = contentControl;
			this.controller = controller;
			this.realOwnerControl = (form == null) ? null : form.OwnerControl;
		}
		protected internal virtual bool IsNull { get { return false; } }
		protected internal bool Immediate { get { return immediate; } }
		protected internal void SetHidingFlag() { this.hiding = true; }
		protected internal void ResetHidingFlag() { this.hiding = false; }
		protected internal void SetMode(bool immediate) {
			this.immediate = immediate;
		}
		protected internal bool IsHiding { get { return this.hiding; } }
		protected internal IFlyoutPanelPopupController Controller { get { return controller; } }
		public virtual bool IsActive {
			get { return Form != null && Form.Visible; }
		}
		public FlyoutPanelToolForm Form { get { return form; } }
		public Control ContentControl { get { return contentControl; } }
		public Control RealOwnerControl { get { return realOwnerControl; } }
	}
	public class NullToolFormObjectInfo : ToolFormObjectInfo {
		public NullToolFormObjectInfo()
			: base(null, null, false) {
		}
		public override bool IsActive {
			get { return false; }
		}
		protected internal override bool IsNull { get { return true; } }
	}
	[ToolboxItem(false)]
	public class FlyoutPanelToolForm : BasePopupToolWindow {
		Control owner;
		FlyoutPanel flyoutPanel;
		FlyoutPanelOptions options;
		public FlyoutPanelToolForm(Control owner, FlyoutPanel flyoutPanel, FlyoutPanelOptions options) {
			this.owner = owner;
			this.flyoutPanel = flyoutPanel;
			this.options = options;
		}
		protected override BasePopupToolWindowHandler CreateHandler() {
			return new FlyoutPanelToolFormHandler(this);
		}
		protected override void ForceClosingCore(bool immediate) {
			if(FlyoutPanel == null)
				return;
			FlyoutPanel.HidePopup(immediate);
		}
		DockStyle contentDockStyle = DockStyle.None;
		protected override void OnAddContentControl() {
			if(this.contentDockStyle != Content.Dock) {
				this.contentDockStyle = Content.Dock;
			}
			base.OnAddContentControl();
		}
		protected override bool ShouldDockFillContent {
			get {
				if(FlyoutPanel == null || !FlyoutPanel.AutoSize) return true;
				return !Options.IsResizeSuppressed;
			}
		}
		protected override bool ShouldCheckContentBounds {
			get {
				return !ShouldDockFillContent;
			}
		}
		protected internal override void OnEndAnimation(PopupToolWindowAnimationEventArgs e) {
			base.OnEndAnimation(e);
			if(!e.IsShowing && IsHandleCreated) BeginInvoke(new MethodInvoker(DisposeFormCore));
		}
		protected override void SubscribeOnParentEvents() {
			base.SubscribeOnParentEvents();
			if(FlyoutPanel == null || FlyoutPanel.FlyoutPanelState == null) return;
			if(FlyoutPanel.FlyoutPanelState.RealOwnerControl != null) FlyoutPanel.FlyoutPanelState.RealOwnerControl.VisibleChanged += OnOwnerControlVisibleChanged;
		}
		protected override void UnSubscribeOnParentEvents() {
			base.UnSubscribeOnParentEvents();
			if(FlyoutPanel == null || FlyoutPanel.FlyoutPanelState == null) return;
			if(FlyoutPanel.FlyoutPanelState.RealOwnerControl != null) FlyoutPanel.FlyoutPanelState.RealOwnerControl.VisibleChanged -= OnOwnerControlVisibleChanged;
		}
		void OnOwnerControlVisibleChanged(object sender, EventArgs e) {
			OnOwnerControlVisibleChangedCore(sender as Control);
		}
		protected virtual void OnOwnerControlVisibleChangedCore(Control ownerControl) {
			if(ownerControl == null || FlyoutPanel == null) return;
			SyncProperties(ownerControl);
		}
		protected virtual void SyncProperties(Control ownerControl) {
			Visible = ownerControl.Visible;
		}
		protected virtual void DisposeFormCore() {
			EnsureRemoveAnimation();
			Dispose();
		}
		protected virtual void EnsureRemoveAnimation() {
			if(Handler.AnimationProvider != null) XtraAnimator.RemoveObject(Handler.AnimationProvider);
		}
		Size contentSize;
		protected override void OnSaveContentState() {
			base.OnSaveContentState();
			this.contentSize = Content.Size;
		}
		protected override void OnRestoreContentState() {
			base.OnRestoreContentState();
			if(Content.Dock != this.contentDockStyle)
				Content.Dock = this.contentDockStyle;
			if(Content.Size != this.contentSize)
				Content.Size = this.contentSize;
			if(Controls.Count > 0) {
				Controls.Remove(Content);
			}
		}
		protected internal virtual void OnContentSizeChanged() {
		}
		protected override void OnOwnerFormChanging() {
			if(FlyoutPanel != null)
				FlyoutPanel.HidePopup(true);
		}
		protected override void OnOwnerFormChanged() {
			if(GetOwnerForm() == null)
				return;
			if(FlyoutPanel != null)
				FlyoutPanel.ShowPopup(true);
		}
		protected override void OnCancelShowCore() {
			base.OnCancelShowCore();
			if(FlyoutPanel != null)
				FlyoutPanel.OnCancelShowCore();
		}
		protected override Form GetOwnerForm() {
			if(FlyoutPanel != null && FlyoutPanel.ParentForm != null) {
				return FlyoutPanel.ParentForm;
			}
			return base.GetOwnerForm();
		}
		protected override bool AllowCloseOnMouseMoveCore(Rectangle formBounds, Point pt) {
			if(FlyoutPanel == null) base.AllowCloseOnMouseMoveCore(formBounds, pt);
			return FlyoutPanel.AllowCloseOnMouseMoveCore(formBounds, pt);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		protected override bool CloseOnHidingOwner { get { return Options.CloseOnHidingOwner; } }
		protected override bool AutoInitialization { get { return false; } }
		protected internal override bool ShouldUseSkinPadding { get { return false; } }
		protected internal override PopupToolWindowAnimation AnimationType { get { return Options.AnimationType; } }
		protected internal override PopupToolWindowAnchor AnchorType { get { return Options.AnchorType; } }
		protected internal override int HorzIndent { get { return Options.HorzIndent; } }
		protected internal override int VertIndent { get { return Options.VertIndent; } }
		protected internal override Control OwnerControl { get { return owner; } }
		protected internal override Size FormSize { get { return flyoutPanel.Size; } }
		protected internal override bool SyncLocationWithOwner { get { return true; } }
		protected internal override Control CreateContentControl() { return flyoutPanel; }
		protected internal override Point FormLocation { get { return Options.Location; } }
		protected internal override bool KeepParentFormActive { get { return true; } }
		protected internal override Control MessageRoutingTarget { get { return null; } }
		protected internal override ISupportLookAndFeel LookAndFeelProvider { get { return FlyoutPanel; } }
		protected internal override bool CloseOnOuterClick { get { return Options.CloseOnOuterClick; } }
		protected internal FlyoutPanel FlyoutPanel { get { return Content as FlyoutPanel; } }
		public FlyoutPanelOptions Options { get { return options; } }
	}
	public class FlyoutPanelToolFormHandler : BasePopupToolWindowHandler {
		public FlyoutPanelToolFormHandler(FlyoutPanelToolForm toolForm)
			: base(toolForm) {
		}
		protected internal override void OnFormOwnerLocationChanged() {
			base.OnFormOwnerLocationChanged();
		}
	}
	[ToolboxItem(false)]
	public class FlyoutPanelBeakForm : FlyoutPanelToolForm {
		BeakFormViewInfo beakFormViewInfo;
		public FlyoutPanelBeakForm(Control owner, FlyoutPanel flyoutPanel, FlyoutPanelOptions options)
			: base(owner, flyoutPanel, options) {
			this.Opacity = flyoutPanel.OptionsBeakPanel.Opacity;
			this.beakFormViewInfo = CreateBeakFormViewInfo();
		}
		public override void ShowToolWindow(bool immediate) {
			EnsureRegion();
			base.ShowToolWindow(immediate);
		}
		protected internal override void DoShow() {
			Show(GetOwner());
		}
		protected virtual Form GetOwner() {
			Form owner = OwnerForm;
			if(owner != null && owner.IsMdiChild) {
				return null;
			}
			return owner;
		}
		protected internal override Size CalcFormSize() {
			Size size = base.CalcFormSize();
			size.Height += ViewInfo.BeakSize.Height;
			return size;
		}
		protected virtual void EnsureRegion() {
			if(FlyoutPanel == null) return;
			Region = ViewInfo.GetRegion();
		}
		protected override void OnSaveContentState() {
			base.OnSaveContentState();
			SetContentBoundsCore();
		}
		protected internal override void OnContentSizeChanged() {
			SetSizeCore();
		}
		protected override void SetSizeCore() {
			base.SetSizeCore();
			EnsureRegion();
		}
		protected virtual void SetContentBoundsCore() {
			if(DesignMode) return;
			EnsureContentFillNone();
			Rectangle rect = ViewInfo.GetClientRect();
			FlyoutPanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
		}
		protected virtual void EnsureContentFillNone() {
			if(FlyoutPanel.Dock != DockStyle.None) FlyoutPanel.Dock = DockStyle.None;
		}
		protected override bool ShouldDockFillContent { get { return false; } }
		protected override bool ShouldCheckContentBounds { get { return false; } }
		protected override BasePopupToolWindowHandler CreateHandler() {
			return new FlyoutPanelBeakFormHandler(this);
		}
		protected override ToolWindowPainterBase CreatePainter() {
			return new FlyoutPanelBeakFormPainter(this, this);
		}
		protected internal override Point FormLocation {
			get { return ViewInfo.GetFormLocation(GetFormScreenLocation()); }
		}
		protected internal BeakFormViewInfo ViewInfo { get { return beakFormViewInfo; } }
		protected virtual BeakFormViewInfo CreateBeakFormViewInfo() { return new BeakFormViewInfo(this); }
		internal static readonly int BorderThickness = 1;
		protected internal override Size FormSize {
			get {
				Size size = base.FormSize;
				return new Size(size.Width + 2 * BorderThickness, size.Height + 2 * BorderThickness);
			}
		}
		protected internal Point GetFormScreenLocation() {
			Point pt = Options.ScreenFormLocation;
			if(!Options.Offset.IsEmpty) {
				int yOffset = Options.Offset.Y * (ViewInfo.ShouldShowBottomBeak ? -1 : 1);
				pt.Offset(Options.Offset.X, yOffset);
			}
			return pt;
		}
		protected virtual void DoDisposing() {
			if(FlyoutPanel != null) FlyoutPanel.OnContainerDisposing();
		}
		public new BeakFlyoutPanelOptions Options { get { return base.Options as BeakFlyoutPanelOptions; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DoDisposing();
				if(Region != null) {
					Region = null;
				}
			}
			base.Dispose(disposing);
		}
	}
	public class BeakFlyoutPanelOptions : FlyoutPanelOptions {
		public BeakFlyoutPanelOptions(Point loc) {
			this.ScreenFormLocation = loc;
			this.Offset = Point.Empty;
		}
		public Point ScreenFormLocation { get; protected internal set; }
		public Point Offset { get; protected internal set; }
	}
	public class FlyoutPanelBeakFormHandler : BasePopupToolWindowHandler {
		public FlyoutPanelBeakFormHandler(BasePopupToolWindow toolWindow)
			: base(toolWindow) {
		}
		protected override PopupToolWindowAnimationProviderBase CreateAnimationProvider() {
			return new FlyoutPanelBeakFormFadeAnimationProvider(this);
		}
	}
	public class FlyoutPanelBeakFormFadeAnimationProvider : PopupToolWindowFadeAnimationProvider {
		public FlyoutPanelBeakFormFadeAnimationProvider(IPopupToolWindowAnimationSupports info)
			: base(info) {
		}
		public new FlyoutPanelBeakForm ToolWindow { get { return base.ToolWindow as FlyoutPanelBeakForm; } }
	}
	public class FlyoutPanelBeakFormPainter : ToolWindowPainter {
		public FlyoutPanelBeakFormPainter(BasePopupToolWindow topWindow, ISupportLookAndFeel lookAndFeel)
			: base(topWindow, lookAndFeel) {
		}
		protected override void DrawBackground(GraphicsCache cache) {
			cache.FillRectangle(cache.GetSolidBrush(GetBackColor()), Bounds);
		}
		protected override void DrawBorders(GraphicsCache cache) {
			Point[] points = ToolWindow.ViewInfo.GetFormShapePoints();
			cache.Graphics.DrawLines(cache.GetPen(GetBorderColor()), points);
		}
		protected virtual Color GetBackColor() {
			FlyoutPanel flyoutPanel = ToolWindow.FlyoutPanel;
			if(flyoutPanel != null) {
				Color color = flyoutPanel.OptionsBeakPanel.BackColor;
				if(color != Color.Transparent && color != Color.Empty) return color;
			}
			return LookAndFeelHelper.GetSystemColor(GetActiveLookAndFeel(), SystemColors.Control);
		}
		protected virtual Color GetBorderColor() {
			FlyoutPanel flyoutPanel = ToolWindow.FlyoutPanel;
			if(flyoutPanel != null) {
				Color color = flyoutPanel.OptionsBeakPanel.BorderColor;
				if(color != Color.Transparent && color != Color.Empty) return color;
			}
			return EditorsSkins.GetSkin(GetActiveLookAndFeel()).Colors[EditorsSkins.SkinBeakFormBorderColor];
		}
		protected UserLookAndFeel GetActiveLookAndFeel() {
			if(ToolWindow == null) return null;
			return ToolWindow.LookAndFeelProvider.LookAndFeel.ActiveLookAndFeel;
		}
		public new FlyoutPanelBeakForm ToolWindow { get { return base.ToolWindow as FlyoutPanelBeakForm; } }
	}
	public class BeakFormViewInfo {
		FlyoutPanelBeakForm beakForm;
		public BeakFormViewInfo(FlyoutPanelBeakForm beakForm) {
			this.beakForm = beakForm;
		}
		public Region GetRegion() {
			if(ShouldShowBottomBeak) {
				return CreateBottomBeakRegion();
			}
			return CreateTopBeakRegion();
		}
		public virtual bool ShouldShowBottomBeak {
			get {
				BeakPanelBeakLocation beakLoc = BeakForm.FlyoutPanel.GetBeakLocation();
				if(beakLoc == BeakPanelBeakLocation.Bottom) {
					return BeakForm.Options.ScreenFormLocation.Y - BeakForm.Options.Offset.Y - BeakForm.FormSize.Height >= 0;
				}
				Rectangle workingArea = Screen.GetWorkingArea(BeakForm);
				return BeakForm.Options.ScreenFormLocation.Y + BeakForm.Options.Offset.Y + BeakForm.FormSize.Height > workingArea.Bottom;
			}
		}
		protected int BorderThickness { get { return FlyoutPanelBeakForm.BorderThickness; } }
		public virtual Rectangle GetClientRect() {
			Rectangle rect = BeakForm.ClientRectangle;
			rect.Inflate(-BorderThickness, 0);
			if(ShouldShowBottomBeak) {
				rect.Y += BorderThickness;
				rect.Height -= (BeakSize.Height + 2 * BorderThickness);
			}
			else {
				rect.Y += BeakSize.Height + BorderThickness;
				rect.Height -= (BeakSize.Height + 2 * BorderThickness);
			}
			return rect;
		}
		public static readonly Size DefaultBeakSize = new Size(19, 10);
		public virtual Size BeakSize {
			get { return DefaultBeakSize; }
		}
		public Point GetFormLocation(Point loc) { 
			int dx, dy;
			CalcHotPointOffsets(loc, out dx, out dy);
			loc.Offset(dx, dy);
			return BeakForm.OwnerControl.PointToClient(loc);
		}
		protected virtual void CalcHotPointOffsets(Point pt, out int dx, out int dy) {
			dx = dy = 0;
			if(ShouldShowBottomBeak) {
				dy = -BeakForm.Height;
			}
			dx = -BeakForm.Width / 2;
			Rectangle displayBounds = Screen.GetBounds(pt);
			if(pt.X + dx < displayBounds.Left) {
				dx += displayBounds.Left - (pt.X + dx);
			}
			if(pt.X + dx + BeakForm.Width > displayBounds.Right) {
				dx -= pt.X + dx + BeakForm.Width - displayBounds.Right;
			}
		}
		protected int CalcHotPointHorzBoundsRestrictionOffset(Point pt) {
			int dx = 0, dy = 0;
			CalcHotPointOffsets(pt, out dx, out dy);
			return dx + BeakForm.Width / 2;
		}
		protected virtual Region CreateTopBeakRegion() {
			int xc = GetHotPoint();
			int baseLine = GetTopBaseLine();
			Point[] beakPoints = GetTopBeakPoints();
			GraphicsPath gp = new GraphicsPath();
			gp.AddLine(0, baseLine, beakPoints[0].X, beakPoints[0].Y);
			gp.AddLine(beakPoints[0].X, beakPoints[0].Y, beakPoints[1].X, beakPoints[1].Y);
			gp.AddLine(beakPoints[1].X, beakPoints[1].Y, beakPoints[2].X, beakPoints[2].Y);
			gp.AddLine(xc - BeakSize.Width / 2 + BeakSize.Width, baseLine, BeakForm.Width, baseLine);
			gp.AddLine(BeakForm.Width, baseLine, BeakForm.Width, BeakForm.Height);
			gp.AddLine(BeakForm.Width, BeakForm.Height, 0, BeakForm.Height);
			gp.AddLine(0, BeakForm.Height, 0, 0);
			return new Region(gp);
		}
		protected virtual Region CreateBottomBeakRegion() {
			int xc = GetHotPoint();
			int baseLine = GetBottomBaseLine();
			Point[] beakPoints = GetBottomBeakPoints();
			GraphicsPath gp = new GraphicsPath();
			gp.AddLine(0, 0, BeakForm.Width, 0);
			gp.AddLine(BeakForm.Width, 0, BeakForm.Width, baseLine);
			gp.AddLine(BeakForm.Width, baseLine, beakPoints[2].X, beakPoints[2].Y);
			gp.AddLine(beakPoints[2].X, beakPoints[2].Y, beakPoints[1].X, beakPoints[1].Y);
			gp.AddLine(beakPoints[1].X, beakPoints[1].Y, beakPoints[0].X, beakPoints[0].Y);
			gp.AddLine(xc - BeakSize.Width / 2, baseLine, 0, baseLine);
			gp.AddLine(0, baseLine, 0, 0);
			return new Region(gp);
		}
		protected Point[] GetTopBeakPoints() {
			int xc = GetHotPoint();
			int baseLine = GetTopBaseLine();
			Point[] points = new Point[3];
			points[0] = new Point(xc - BeakSize.Width / 2 - 1, baseLine);
			points[1] = new Point(xc, -1);
			points[2] = new Point(xc - BeakSize.Width / 2 + BeakSize.Width, baseLine);
			return points;
		}
		protected Point[] GetBottomBeakPoints() {
			int xc = GetHotPoint();
			int baseLine = GetBottomBaseLine();
			Point[] points = new Point[3];
			points[0] = new Point(xc - BeakSize.Width / 2, baseLine);
			points[1] = new Point(xc, BeakForm.Height);
			points[2] = new Point(xc - BeakSize.Width / 2 + BeakSize.Width, baseLine);
			return points;
		}
		protected int GetTopBaseLine() {
			return BeakSize.Height;
		}
		protected int GetBottomBaseLine() {
			return BeakForm.Height - BeakSize.Height;
		}
		protected int GetHotPoint() {
			int dx = CalcHotPointHorzBoundsRestrictionOffset(BeakForm.GetFormScreenLocation());
			return BeakForm.Width / 2 - dx;
		}
		public Point[] GetFormShapePoints() {
			if(BeakForm.ViewInfo.ShouldShowBottomBeak) {
				return GetBottomBeakShapePoints();
			}
			return GetTopBeakShapePoints();
		}
		protected virtual Point[] GetTopBeakShapePoints() {
			Point[] res = new Point[8];
			int baseLine = GetTopBaseLine();
			Point[] beakPoints = GetTopBeakPoints();
			res[0] = new Point(0, baseLine);
			res[1] = new Point(0, BeakForm.Height - 1);
			res[2] = new Point(BeakForm.Width - 1, BeakForm.Height - 1);
			res[3] = new Point(BeakForm.Width - 1, baseLine);
			res[4] = new Point(beakPoints[2].X, baseLine);
			res[5] = new Point(beakPoints[1].X, 0);
			res[6] = new Point(beakPoints[0].X, baseLine);
			res[7] = new Point(0, baseLine);
			return res;
		}
		protected virtual Point[] GetBottomBeakShapePoints() {
			Point[] res = new Point[8];
			int baseLine = GetBottomBaseLine();
			Point[] beakPoints = GetBottomBeakPoints();
			res[0] = new Point(0, 0);
			res[1] = new Point(BeakForm.Width - 1, 0);
			res[2] = new Point(BeakForm.Width - 1, baseLine - 1);
			res[3] = new Point(beakPoints[2].X, baseLine - 1);
			res[4] = new Point(beakPoints[1].X, BeakForm.Height - 1);
			res[5] = new Point(beakPoints[0].X - 1, baseLine - 1);
			res[6] = new Point(0, baseLine - 1);
			res[7] = new Point(0, 0);
			return res;
		}
		public FlyoutPanelBeakForm BeakForm { get { return beakForm; } }
	}
}
