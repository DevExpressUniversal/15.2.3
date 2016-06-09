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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.ButtonsPanelControl;
namespace DevExpress.XtraBars.Docking2010 {
	public interface IWindowsUIButtonPanelOwner : IButtonsPanelOwner {
		bool EnableImageTransparency { get; }
		bool UseButtonBackgroundImages { get; }
	}
	[DXToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "Docking2010.WindowsUIButtonPanel"),
	ToolboxTabName(AssemblyInfo.DXTabNavigation),
	Designer("DevExpress.XtraBars.Design.WindowsUIButtonPanelControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	public class WindowsUIButtonPanel : Control, IWindowsUIButtonPanelOwner, ISupportLookAndFeel, IToolTipControlClient, IButtonPanelControlAppearanceOwner, IButtonsPanelGlyphSkinningOwner,
		DevExpress.Utils.Controls.IXtraResizableControl {
		object imagesCore;
		WindowsUIButtonsPanel buttonsPanelCore;
		ToolTipController toolTipControllerCore;
		UserLookAndFeel lookAndFeelCore;
		ButtonsPanelControlAppearance appearanceButtonCore;
		object buttonBackgroundImagesCore;
		bool enableImageTransparencyCore;
		bool allowHtmlDrawCore;
		bool allowGlyphSkinningCore = true;
		bool useButtonBackgroundImagesCore = true;
		FlyoutPanel flyoutCore;
		public WindowsUIButtonPanel() {
			buttonsPanelCore = CreateButtonsPanel();
			SubscribeButtonPanel();
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
			this.lookAndFeelCore = new UserLookAndFeel(this);
			lookAndFeelCore.StyleChanged += OnLookAndFeelChanged;
			ButtonsPanel.ButtonInterval = 10;
			PeekFormShowDelay = defaultPeekFormShowDelay;
			CausesValidation = false;
			ColoredElementsCache.Reset();
		}
		protected override void Dispose(bool disposing) {
			ToolTipController = null;
			if(lookAndFeelCore != null)
				lookAndFeelCore.StyleChanged -= OnLookAndFeelChanged;
			if(flyoutCore != null)
				flyoutCore.Hidden -= OnFlyoutHidden;
			if(appearanceButtonCore != null)
				appearanceButtonCore.Changed -= OnAppearanceChanged;
			if(ButtonsPanel != null) 
				UnsubscribeButtonPanel();
			Ref.Dispose(ref buttonsPanelCore);
			Ref.Dispose(ref flyoutCore);
			Ref.Dispose(ref appearanceButtonCore);
			Ref.Dispose(ref DesignBorderPen);
			this.buttonBackgroundImagesCore = null;
			this.imagesCore = null;
			ColoredElementsCache.Reset();
			base.Dispose(disposing);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			ColoredElementsCache.Reset();
		}
		protected WindowsUIButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelButtons"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraBars.Design.WindowsUIButtonCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesign,
		 typeof(System.Drawing.Design.UITypeEditor)), Category("Buttons"), Localizable(true)]
		public BaseButtonCollection Buttons {
			get { return ButtonsPanel.Buttons; }
		}
		bool ShouldSerializeCustomHeaderButtons() {
			return (ButtonsPanel != null) && ButtonsPanel.Buttons.Count > 0;
		}
		[DefaultValue(false), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelCausesValidation")
#else
	Description("")
#endif
]
		public new bool CausesValidation {
			get { return base.CausesValidation; }
			set { base.CausesValidation = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelContentAlignment"),
#endif
 DefaultValue(ContentAlignment.MiddleCenter), Category(DockConsts.LayoutCategory)]
		public ContentAlignment ContentAlignment {
			get { return ButtonsPanel.ContentAlignment; }
			set { ButtonsPanel.ContentAlignment = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelImages"),
#endif
		DefaultValue(null), Category(DockConsts.AppearanceCategory), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object Images {
			get { return imagesCore; }
			set {
				if(Images == value) return;
				imagesCore = value;
				UpdateButtonPanel();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelOrientation"),
#endif
 DefaultValue(Orientation.Horizontal), Category(DockConsts.LayoutCategory)]
		public Orientation Orientation {
			get { return ButtonsPanel.Orientation; }
			set { ButtonsPanel.Orientation = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelButtonInterval"),
#endif
 DefaultValue(10), Category(DockConsts.LayoutCategory)]
		public int ButtonInterval {
			get { return ButtonsPanel.ButtonInterval; }
			set { ButtonsPanel.ButtonInterval = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelToolTipController"),
#endif
 DefaultValue(null), Category(DockConsts.AppearanceCategory)]
		public virtual ToolTipController ToolTipController {
			get { return toolTipControllerCore ?? ToolTipController.DefaultController; }
			set {
				if(ToolTipController == value) return;
				if(toolTipControllerCore != null) ToolTipController.Disposed -= ToolTipControllerDisposed;
				toolTipControllerCore = value;
				if(toolTipControllerCore != null) ToolTipController.Disposed += ToolTipControllerDisposed;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelAppearanceButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(DockConsts.AppearanceCategory)]
		public virtual ButtonsPanelControlAppearance AppearanceButton {
			get {
				if(appearanceButtonCore == null) {
					appearanceButtonCore = new ButtonsPanelControlAppearance(this);
					appearanceButtonCore.Changed += OnAppearanceChanged;
				}
				return appearanceButtonCore;
			}
		}
		void ResetAppearanceButton() { AppearanceButton.Reset(); }
		bool ShouldSerializeAppearanceButton() { return AppearanceButton.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelButtonBackgroundImages"),
#endif
		DefaultValue(null), Category(DockConsts.AppearanceCategory), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ButtonBackgroundImages {
			get { return buttonBackgroundImagesCore; }
			set {
				if(ButtonBackgroundImages == value) return;
				buttonBackgroundImagesCore = value;
				ColoredElementsCache.Reset();
				UpdateButtonPanel();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelWrapButtons"),
#endif
		DefaultValue(false), Category(DockConsts.LayoutCategory)]
		public bool WrapButtons {
			get { return ButtonsPanel.WrapButtons; }
			set { ButtonsPanel.WrapButtons = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonEnableImageTransparency"),
#endif
		DefaultValue(false), Category(DockConsts.AppearanceCategory)]
		public bool EnableImageTransparency {
			get { return enableImageTransparencyCore; }
			set {
				if(EnableImageTransparency == value) return;
				enableImageTransparencyCore = value;
				ColoredElementsCache.Reset();
				UpdateButtonPanel();
			}
		}
		[
		DefaultValue(false), Category(DockConsts.AppearanceCategory)]
		public bool AllowHtmlDraw {
			get { return allowHtmlDrawCore; }
			set {
				if(allowHtmlDrawCore == value) return;
				allowHtmlDrawCore = value;
				UpdateButtonPanel();
			}
		}
		[
		DefaultValue(true), Category(DockConsts.AppearanceCategory)]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinningCore; }
			set {
				if(allowGlyphSkinningCore == value) return;
				allowGlyphSkinningCore = value;
				UpdateButtonPanel();
			}
		}
		[
		DefaultValue(true), Category(DockConsts.AppearanceCategory)]
		public bool UseButtonBackgroundImages {
			get { return useButtonBackgroundImagesCore; }
			set {
				if(useButtonBackgroundImagesCore == value) return;
				useButtonBackgroundImagesCore = value;
				UpdateButtonPanel();
			}
		}
		[
		DefaultValue(false), Category(DockConsts.BehaviorCategory)]
		public bool ShowPeekFormOnItemHover { get; set; }
		const int defaultPeekFormShowDelay = 1500;
		[
		DefaultValue(defaultPeekFormShowDelay), Category(DockConsts.BehaviorCategory)]
		public int PeekFormShowDelay { get; set; }
		[
		Category(DockConsts.BehaviorCategory)]
		public Size PeekFormSize { get; set; }
		bool ShouldSerializePeekFormSize() {
			return PeekFormSize != Size.Empty;
		}
		void ResetPeekFormSize() {
			PeekFormSize = Size.Empty;
		}
		#region Events
		static readonly object buttonClick = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelButtonClick"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event ButtonEventHandler ButtonClick {
			add { this.Events.AddHandler(buttonClick, value); }
			remove { this.Events.RemoveHandler(buttonClick, value); }
		}
		static readonly object buttonUnchecked = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelButtonUnchecked"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event ButtonEventHandler ButtonUnchecked {
			add { this.Events.AddHandler(buttonUnchecked, value); }
			remove { this.Events.RemoveHandler(buttonUnchecked, value); }
		}
		static readonly object buttonChecked = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelButtonChecked"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event ButtonEventHandler ButtonChecked {
			add { this.Events.AddHandler(buttonChecked, value); }
			remove { this.Events.RemoveHandler(buttonChecked, value); }
		}
		static readonly object queryPeekFormContent = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelQueryPeekFormContent"),
#endif
 Category(DockConsts.LayoutCategory)]
		public event QueryPeekFormContentEventHandler QueryPeekFormContent {
			add { Events.AddHandler(queryPeekFormContent, value); }
			remove { Events.RemoveHandler(queryPeekFormContent, value); }
		}
		protected virtual void OnButtonChecked(object sender, ButtonEventArgs e) {
			RaiseButtonChecked(e);
		}
		protected virtual void OnButtonUnchecked(object sender, ButtonEventArgs e) {
			RaiseButtonUnchecked(e);
		}
		protected virtual void OnButtonClick(object sender, ButtonEventArgs e) {
			RaiseButtonClick(e);
		}
		protected void RaiseButtonChecked(ButtonEventArgs e) {
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[buttonChecked];
			if(handler != null) handler(this, e);
		}
		protected void RaiseButtonUnchecked(ButtonEventArgs e) {
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[buttonUnchecked];
			if(handler != null) handler(this, e);
		}
		protected void RaiseButtonClick(ButtonEventArgs e) {
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[buttonClick];
			if(handler != null) handler(this, e);
			WindowsUIButton button = e.Button as WindowsUIButton;
			if(button != null) button.RaiseClick();
		}
		protected internal Control RaiseQueryPeekFormContent(IBaseButton button) {
			QueryPeekFormContentEventHandler handler = (QueryPeekFormContentEventHandler)Events[queryPeekFormContent];
			QueryPeekFormContentEventArgs e = new QueryPeekFormContentEventArgs(button);
			if(handler != null)
				handler(this, e);
			return e.Control;
		}
		#endregion
		FlyoutPanel Flyout {
			get {
				if(flyoutCore == null || flyoutCore.IsDisposed) {
					flyoutCore = new FlyoutPanel();
					flyoutCore.Hidden += OnFlyoutHidden;
				}
				return flyoutCore;
			}
		}
		static void OnFlyoutHidden(object sender, FlyoutPanelEventArgs e) {
			Control flyout = sender as Control;
			if(flyout != null) flyout.Tag = null;
		}
		public void Merge(WindowsUIButtonPanel buttonsPanel){
			buttonsPanelCore.Merge(buttonsPanel);
		}
		public void Unmerge() {
			buttonsPanelCore.Unmerge();
		}
		public void HidePeekForm() {
			Flyout.HideBeakForm(true);
		}
		public void ShowPeekForm(IBaseButton button) {
			if(button != null)
				ShowPeekForm(button, GetButtonBounds(button));
		}
		internal void ShowPeekForm(IBaseButton button, Rectangle itemRect) {
			if(Flyout.Tag == button) return;
			Control content = RaiseQueryPeekFormContent(button);
			if(content == null || itemRect.IsEmpty) return;
			Flyout.HideBeakForm(true);
			Flyout.Controls.Clear();
			Flyout.OwnerControl = this;
			Flyout.ParentForm = this.FindForm();
			Flyout.Tag = button;
			if(!PeekFormSize.IsEmpty)
				Flyout.ClientSize = PeekFormSize;
			else
				Flyout.ClientSize = content.Size;
			content.Dock = DockStyle.Fill;
			content.Parent = Flyout;
			Flyout.ShowBeakForm(GetPeekLocation(RectangleToScreen(itemRect), Orientation, Dock, content.Bounds));
		}
		Point GetPeekLocation(Rectangle itemRect, Orientation orientation, DockStyle dockStyle, Rectangle contentBounds) {
			int centerX = itemRect.X + (itemRect.Width / 2);
			int centerY = itemRect.Y + (itemRect.Height / 2);
			switch(dockStyle) {
				case DockStyle.None:
				case DockStyle.Left:
				case DockStyle.Bottom:
				case DockStyle.Fill:
				case DockStyle.Right:
					Flyout.OptionsBeakPanel.BeakLocation = BeakPanelBeakLocation.Default;
					if(orientation == Orientation.Horizontal) {
						if(itemRect.Y - contentBounds.Height <= 0) {
							Flyout.OptionsBeakPanel.BeakLocation = BeakPanelBeakLocation.Top;
							return new Point(centerX, itemRect.Y + itemRect.Height);
						}
						return new Point(centerX, itemRect.Y);
					}
					return dockStyle == DockStyle.Right ? new Point(itemRect.Left, centerY) : new Point(itemRect.Right, centerY);
				case DockStyle.Top:
					Screen screen = Screen.FromControl(this);
					Flyout.OptionsBeakPanel.BeakLocation = BeakPanelBeakLocation.Top;
					if(screen != null && itemRect.Y + itemRect.Height + contentBounds.Height > screen.Bounds.Bottom) {
						Flyout.OptionsBeakPanel.BeakLocation = BeakPanelBeakLocation.Default;
						return new Point(centerX, itemRect.Y);
					}
					return new Point(centerX, itemRect.Y + itemRect.Height);
				default:
					return Point.Empty;
			}
		}
		Rectangle GetButtonBounds(IBaseButton button) {
			foreach(var item in ButtonsPanel.ViewInfo.Buttons) {
				if(item.Button == button)
					return item.Bounds;
			}
			return Rectangle.Empty;
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(Disposing) return;
			CalcButtonsPanel(e.Graphics);
			if(DesignMode)
				DrawDesignTimeBorder(e.Graphics);
			using(GraphicsCache cache = new GraphicsCache(e)) {
				if(ButtonsPanel != null && ButtonsPanel.ViewInfo != null) {
					ObjectPainter.DrawObject(cache, ((IButtonsPanelOwner)this).GetPainter(), (ObjectInfoArgs)ButtonsPanel.ViewInfo);
				}
			}
		}
		Pen DesignBorderPen = new Pen(Color.Red) { DashPattern = new float[] { 5.0f, 5.0f } };
		protected void DrawDesignTimeBorder(Graphics g) {
			g.DrawRectangle(DesignBorderPen, Rectangle.Inflate(ClientRectangle, -1, -1));
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
		protected virtual void CalcButtonsPanel(Graphics g) {
			if(ButtonsPanel != null)
				ButtonsPanel.ViewInfo.Calc(g, CalcClientRectangle(ClientRectangle));
		}
		public IBaseButton CalcHitInfo(Point location) {
			BaseButtonInfo result = ButtonsPanel.ViewInfo.CalcHitInfo(location);
			return (result != null) ? result.Button : null;
		}
		protected Rectangle CalcClientRectangle(Rectangle bounds) {
			if(Padding == null) return bounds;
			return new Rectangle(
				bounds.Left + Padding.Left,
				bounds.Top + Padding.Top,
				bounds.Width - Padding.Horizontal,
				bounds.Height - Padding.Vertical);
		}
		protected virtual WindowsUIButtonsPanel CreateButtonsPanel() {
			return new WindowsUIButtonsPanel(this);
		}
		protected void SubscribeButtonPanel() {
			ButtonsPanel.Changed += OnButtonsPanelChanged;
			ButtonsPanel.ButtonClick += OnButtonClick;
			ButtonsPanel.ButtonChecked += OnButtonChecked;
			ButtonsPanel.ButtonUnchecked += OnButtonUnchecked;
		}
		protected void UnsubscribeButtonPanel() {
			ButtonsPanel.Changed -= OnButtonsPanelChanged;
			ButtonsPanel.ButtonClick -= OnButtonClick;
			ButtonsPanel.ButtonChecked -= OnButtonChecked;
			ButtonsPanel.ButtonUnchecked -= OnButtonUnchecked;
		}
		void OnButtonsPanelChanged(object sender, EventArgs e) {
			UpdateButtonPanel();
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			UpdateButtonPanel();
		}
		protected virtual void UpdateButtonPanel() {
			ButtonsPanel.ViewInfo.SetDirty();
			RaiseSizeableChanged();
			Invalidate();
			Update();
		}
		#region MouseEvents
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!CheckCursorVisibility()) return;
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseMove(e);
		}
		const int CURSOR_SHOWING = 0x01;
		bool CheckCursorVisibility() {
			BarNativeMethods.CURSORINFO info = new BarNativeMethods.CURSORINFO();
			info.cbSize = BarNativeMethods.SizeOf(info);
			if(BarNativeMethods.GetCursorInfo(out info)) {
				if(info.flags != CURSOR_SHOWING)
					return false;
			}
			return true;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseUp(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ToolTipController.RemoveClientControl(this);
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseLeave();
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			ToolTipController.AddClientControl(this);
		}
		#endregion
		#region IButtonsPanelOwner Members
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			if(IsSkinPaintStyle)
				return new WindowsUIButtonsPanelSkinPainter(LookAndFeel);
			return new WindowsUIButtonsPanelPainter();
		}
		protected virtual internal bool IsSkinPaintStyle {
			get { return LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin; }
		}
		bool IButtonsPanelOwner.IsSelected {
			get { return false; }
		}
		#endregion
		#region IToolTipControlClient Members
		void ToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		public ToolTipControlInfo GetObjectInfo(Point point) {
			return ButtonsPanel.GetObjectInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return ButtonsPanel.ShowToolTips; } }
		#endregion
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren { get { return true; } }
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel { get { return LookAndFeel; } }
		protected UserLookAndFeel LookAndFeel { get { return lookAndFeelCore; } }
		#endregion
		void OnLookAndFeelChanged(object sender, EventArgs e) {
			ButtonsPanel.UpdateStyle();
			ColoredElementsCache.Reset();
			Invalidate();
			Update();
		}
		#region IWindowsUIButtonPanelAppearanceOwner Members
		public IButtonsPanelControlAppearanceProvider CreateAppearanceProvider() {
			return new ButtonsPanelControlAppearanceProvider();
		}
		#endregion
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
		#region IButtonsPanelGlyphSkinningOwner Members
		public Color GetGlyphSkinningColor(BaseButtonInfo info) {
			return ForeColor;
		}
		#endregion
		#region IXtraResizableControl
		bool autoSizeInLayoutControl = true;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonPanelAutoSizeInLayoutControl"),
#endif
 DefaultValue(true)]
		[Category(DevExpress.XtraEditors.CategoryName.Properties), RefreshProperties(RefreshProperties.All)]
		public virtual bool AutoSizeInLayoutControl {
			get { return autoSizeInLayoutControl; }
			set {
				if(autoSizeInLayoutControl == value) return;
				autoSizeInLayoutControl = value;
				RaiseSizeableChanged();
			}
		}
		bool DevExpress.Utils.Controls.IXtraResizableControl.IsCaptionVisible {
			get { return false; }
		}
		Size DevExpress.Utils.Controls.IXtraResizableControl.MinSize {
			get { return GetResizableSize(); }
		}
		Size DevExpress.Utils.Controls.IXtraResizableControl.MaxSize {
			get { return GetResizableSize(); }
		}
		Size GetResizableSize() {
			return AutoSizeInLayoutControl ? GetLayoutMinSize() : Size.Empty;
		}
		GraphicsInfo GInfo = new GraphicsInfo();
		Size GetLayoutMinSize() {
			Graphics g = GInfo.AddGraphics(null);
			try {
				Size minSize = ButtonsPanel.ViewInfo.CalcMinSize(g);
				if(Orientation == Orientation.Horizontal)
					return new Size(0, minSize.Height);
				return new Size(minSize.Width, 0);
			}
			finally { GInfo.ReleaseGraphics(); }
		}
		readonly static object sizableChanged = new object();
		event EventHandler DevExpress.Utils.Controls.IXtraResizableControl.Changed {
			add { Events.AddHandler(sizableChanged, value); }
			remove { Events.RemoveHandler(sizableChanged, value); }
		}
		void RaiseSizeableChanged() {
			EventHandler handler = Events[sizableChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion IXtraResizableControl
	}
	public delegate void QueryPeekFormContentEventHandler(
			object sender, QueryPeekFormContentEventArgs e);
	public class QueryPeekFormContentEventArgs : EventArgs {
		public QueryPeekFormContentEventArgs(IBaseButton button) {
			Button = button;
		}
		public IBaseButton Button { get; private set; }
		public Control Control { get; set; }
	}
}
