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
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Base;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Navigation;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Navigation {
	public interface INavigationPageBase : IBaseObject {
		string Caption { get; }
		bool PageVisible { get; }
		bool Visible { get; set; }
		Control Parent { get; set; }
		Rectangle Bounds { get; set; }
		void SetOwner(INavigationPane owner);
		Padding BackgroundPadding { get; }
		IBaseNavigationPageDefaultProperties Properties { get; }
		Image Image { get; }
		DxImageUri ImageUri { get; }
		ObjectPainter Painter { get; }
		NavigationPageViewInfo ViewInfo { get; }
		string PageText { get; }
	}
	public interface INavigationPage : INavigationPageBase {
		ButtonsPanel ButtonsPanel { get; }
		ButtonCollection CustomHeaderButtons { get; }
		new INavigationPageDefaultProperties Properties { get; }
	}
	public class NavigationPageBase : XtraPanel, INavigationPageBase, INavigationItem {
		IBaseNavigationPageDefaultProperties propertiesCore;
		NavigationPageViewInfo viewInfoCore;
		GraphicsInfo gInfo;
		bool pageVisibleCore;
		INavigationFrame ownerCore;
		public NavigationPageBase() {
			Visible = false;
			viewInfoCore = CreateViewInfo();
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.Selectable, false);
			base.SetStyle(ControlStyles.CacheText, true);
			gInfo = new GraphicsInfo();
			pageVisibleCore = true;
			InitProperties(null);
			ImageUri.Uri = "New";
			LookAndFeel.StyleChanged += OnStyleChanged;
		}
		void OnStyleChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		void INavigationPageBase.SetOwner(INavigationPane owner) {
			SetOwner(owner);
		}
		protected virtual void SetOwner(INavigationPane owner) {
			ownerCore = owner;
			if(Owner != null)
				LookAndFeel.ParentLookAndFeel = owner.LookAndFeel;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationPageProperties")]
#endif
		[Category(DevExpress.XtraEditors.CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public virtual IBaseNavigationPageDefaultProperties Properties {
			get { return propertiesCore; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public INavigationFrame Owner { get { return ownerCore; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsDisposing {
			get { return false; }
		}
		ObjectPainter INavigationPageBase.Painter { get { return Painter; } }
		protected virtual NavigationPageViewInfo CreateViewInfo() {
			return new NavigationPageViewInfo(this);
		}
		protected override ObjectPainter CreatePainter() {
			return new NavigationPageSkinPainter(LookAndFeel);
		}
		void InitProperties(IBaseNavigationPageProperties parentProperties) {
			propertiesCore = CreateDefaultProperties(parentProperties);
			Properties.Changed += OnPropertiesChanged;
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			LayoutChanged(true);
		}
		protected virtual IBaseNavigationPageDefaultProperties CreateDefaultProperties(IBaseNavigationPageProperties parentProperties) { return null; }
		protected internal virtual void LayoutChanged(bool updateOwner = false) { }
		string pageTextCore;
		[DefaultValue(null), Category("Appearance"), SmartTagProperty("Page Text", "")]
		public string PageText {
			get { return pageTextCore; }
			set {
				if(String.Equals(pageTextCore, value)) return;
				pageTextCore = value;
				LayoutChanged(true);
			}
		}
		Image imageCore;
		[Category("Appearance")]
		[Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)), DefaultValue(null), SmartTagProperty("Image", "")]
		public Image Image {
			get { return imageCore; }
			set {
				if(imageCore == value) return;
				imageCore = value;
				LayoutChanged(true);
			}
		}
		DxImageUri imageUriCore;
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DxImageUri ImageUri {
			get {
				if(imageUriCore == null)
					imageUriCore = new DxImageUri();
				return imageUriCore;
			}
			set {
				if(value == null || value.Equals(imageUriCore)) return;
				imageUriCore = value;
				LayoutChanged(true);
			}
		}
		void ResetImageUri() {
			ImageUri.Reset();
			LayoutChanged(true);
		}
		bool ShouldSerializeImageUri() {
			return ImageUri.ShouldSerialize();
		}
		Padding? backgroundPaddingCore;
		[ DXCategory(CategoryName.Appearance)]
		public Padding BackgroundPadding {
			get {
				if(backgroundPaddingCore.HasValue)
					return backgroundPaddingCore.Value;
				var navigationPagePainter = Painter as NavigationPagePainter;
				if(navigationPagePainter != null)
					return navigationPagePainter.Padding;
				return Padding.Empty;
			}
			set {
				if(backgroundPaddingCore == value) return;
				backgroundPaddingCore = value;
				LayoutChanged(true);
			}
		}
		bool ShouldSerializeBackgroundPadding() {
			return backgroundPaddingCore.HasValue;
		}
		void ResetBackgroundPadding() {
			backgroundPaddingCore = null;
			LayoutChanged(true);
		}
		[ DXCategory(CategoryName.Appearance)]
		[Bindable(true), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), SmartTagProperty("Caption", "")]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public string Caption {
			get { return (this as INavigationPageBase).Caption; }
			set { Text = value; }
		}
		[DefaultValue(true), Category("Behavior")]
		public bool PageVisible {
			get { return pageVisibleCore; }
			set {
				if(pageVisibleCore == value) return;
				pageVisibleCore = value;
				LayoutChanged(true);
			}
		}
		NavigationPageViewInfo INavigationPageBase.ViewInfo {
			get { return ViewInfo; }
		}
		protected internal virtual NavigationPageViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			Properties.Changed -= OnPropertiesChanged;
			Properties.Dispose();
		}
		bool INavigationItem.Visible { get { return true; } }
		string INavigationItem.Text { get { return string.IsNullOrEmpty(Caption) ? Name : Caption; } }
		protected void EnsureIsBoundToControl() {
			if(!IsControlLoadedByQueryControl.GetValueOrDefault(false)) {
				NavigationFrame frame = Parent as NavigationFrame;
				if(frame != null && frame.SelectedPageCore == this)
					EnsureIsBoundToControl(frame);
			}
		}
		#region Deferred Load
		bool isDeferredControlLoadCore;
		[Browsable(false)]
		public bool IsDeferredControlLoad {
			get { return isDeferredControlLoadCore || (!IsControlLoadedByQueryControl.GetValueOrDefault(false) && GetControl(this) == null); }
		}
		internal void MarkAsDeferredControlLoad() {
			isDeferredControlLoadCore = true;
		}
		[Browsable(false)]
		public bool IsControlLoaded {
			get { return Controls.Count > 0; }
		}
		string controlTypeNameCore;
		[Category("Deferred Control Load Properties"), DefaultValue(null)]
		public string ControlTypeName {
			get { return controlTypeNameCore; }
			set {
				if(!CanLoadControl()) return;
				controlTypeNameCore = value;
			}
		}
		string controlNameCore;
		[Category("Deferred Control Load Properties"), DefaultValue(null)]
		public string ControlName {
			get { return controlNameCore; }
			set {
				if(!CanLoadControl()) return;
				controlNameCore = value;
			}
		}
		protected internal static Control GetControl(NavigationPageBase page) {
			return (page != null) && (page.Controls.Count > 0) ? page.Controls[0] : null;
		}
		protected internal virtual bool EnsureIsBoundToControl(NavigationFrame frame) {
			if(CanLoadControl())
				EnsureDeferredLoadControl(frame);
			bool controlLoaded = GetControl(this) != null;
			var navigationFrame = Parent as INavigationFrame;
			return controlLoaded;
		}
		bool? IsControlLoadedByQueryControl;
		protected void EnsureDeferredLoadControl(NavigationFrame frame) {
			Control control = frame.RaiseQueryControl(this);
			if(control != null) {
				IsControlLoadedByQueryControl = true;
				control.Dock = DockStyle.Fill;
				Controls.Add(control);
				(frame as INavigationFrame).UpdateMergedBarsAndRibbon();
			}
		}
		protected internal void ReleaseDeferredLoadControl(NavigationFrame frame) {
			ReleaseDeferredLoadControl(frame, true, true);
		}
		protected internal void ReleaseDeferredLoadControl(NavigationFrame frame, bool keepControl, bool disposeControl) {
			if(frame == null || !IsControlLoadedByQueryControl.GetValueOrDefault(false)) return;
			ReleaseDeferredLoadControlCore(frame, keepControl, disposeControl);
		}
		int lockReleaseDeferredLoadControl = 0;
		protected void ReleaseDeferredLoadControlCore(NavigationFrame frame, bool keepControl, bool disposeControl) {
			if(lockReleaseDeferredLoadControl > 0) return;
			lockReleaseDeferredLoadControl++;
			try {
				bool disposeControlResult;
				if(!frame.RaiseControlReleasing(this, keepControl, disposeControl, out disposeControlResult)) return;
				using(BatchUpdate.Enter(frame, true))
					ReleaseDeferredLoadControlCore(frame, disposeControlResult);
			}
			finally { lockReleaseDeferredLoadControl--; }
		}
		protected void ReleaseDeferredLoadControlCore(NavigationFrame frame, bool disposeControl) {
			Control control = GetControl(this);
			if(control != null) {
				control.Parent = null;
				if(disposeControl) {
					if(!control.IsDisposed)
						Ref.Dispose(ref control);
				}
			}
			IsControlLoadedByQueryControl = false;
		}
		protected internal virtual bool CanLoadControl() {
			return IsDeferredControlLoad && !IsControlLoaded;
		}
		protected internal bool CanLoadControlByName(string controlName) {
			return CanLoadControl() &&
				(!string.IsNullOrEmpty(controlName)) && (ControlName == controlName);
		}
		protected internal bool CanLoadControlByType(Type controlType) {
			return CanLoadControl() && (controlType != null) &&
				(ControlTypeName == controlType.FullName || ControlTypeName == controlType.Name);
		}
		protected internal bool CanLoadControl(Control control) {
			return CanLoadControl() && (control != null) &&
				(string.IsNullOrEmpty(ControlName) && string.IsNullOrEmpty(ControlTypeName));
		}
		#endregion
		string INavigationPageBase.Caption {
			get { return string.IsNullOrEmpty(Text) ? Name : Text; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(DesignMode)
				DrawDesignTimeBorder(e.Graphics);
		}
		Pen borderPen = new Pen(Color.Red) { DashPattern = new float[] { 5.0f, 5.0f } };
		protected void DrawDesignTimeBorder(Graphics graphics) {
			Rectangle rect = ClientRectangle;
			rect.Inflate(-1, -1);
			graphics.DrawRectangle(borderPen, rect);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new BorderStyle BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
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
	}
	[Docking(DockingBehavior.Never)]
	[Designer("DevExpress.XtraBars.Design.NavigationPageDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	[SmartTagSupport(typeof(NavigationPageBounds), SmartTagSupportAttribute.SmartTagCreationMode.Auto), SmartTagFilter(typeof(NavigationPageFilter))]
	public class TabNavigationPage : NavigationPageBase {
		public TabNavigationPage() : base() { }
		protected override IBaseNavigationPageDefaultProperties CreateDefaultProperties(IBaseNavigationPageProperties parentProperties) {
			return new BaseNavigationPageDefaultProperties(parentProperties);
		}
		[Category("Appearance"), DefaultValue(ItemShowMode.Default)]
		public ItemShowMode ItemShowMode {
			get { return base.Properties.ShowMode; }
			set { base.Properties.ShowMode = value; }
		}
		protected internal override void LayoutChanged(bool updateOwner = false) {
			var owner = Owner as ITabPane;
			if(owner != null)
				owner.LayoutChanged();
		}
	}
	[Docking(DockingBehavior.Never)]
	[Designer("DevExpress.XtraBars.Design.NavigationPageDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	[SmartTagSupport(typeof(NavigationPageBounds), SmartTagSupportAttribute.SmartTagCreationMode.Auto), SmartTagFilter(typeof(NavigationPageFilter)),
	SmartTagAction(typeof(NavigationPageActions), "CustomHeaderButtons", "Custom Header Buttons", SmartTagActionType.CloseAfterExecute)]
	public class NavigationPage : NavigationPageBase, IButtonsPanelOwner, INavigationPage {
		ButtonsPanel buttonsPanelCore;
		public NavigationPage() : base() {
			buttonsPanelCore = CreateButtonsPanel();
			SubscribeButtonsPanel();
			customHeaderButtonsCore = new ButtonCollection(buttonsPanelCore);
			customHeaderButtonsCore.CollectionChanged += OnCustomHeaderButtonsCollectionChanged;
			buttonsPanelCore.Buttons.AddRange(new IBaseButton[] { new ExpandButton(), new CollapseButton() });
			buttonsPanelCore.ContentAlignment = ContentAlignment.MiddleRight;
		}
		void OnStyleChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected void SubscribeButtonsPanel() {
			ButtonsPanel.Changed += OnButtonsPanelChanged;
			ButtonsPanel.ButtonClick += OnButtonClick;
			ButtonsPanel.ButtonClick += OnEmbeddedButtonClick;
			ButtonsPanel.ButtonChecked += OnEmbeddedButtonChecked;
			ButtonsPanel.ButtonUnchecked += OnEmbeddedButtonUnchecked;
		}
		protected void UnsubscribeButtonsPanel() {
			ButtonsPanel.Changed -= OnButtonsPanelChanged;
			ButtonsPanel.ButtonClick -= OnButtonClick;
			ButtonsPanel.ButtonClick -= OnEmbeddedButtonClick;
			ButtonsPanel.ButtonChecked -= OnEmbeddedButtonChecked;
			ButtonsPanel.ButtonUnchecked -= OnEmbeddedButtonUnchecked;
		}
		protected ButtonsPanel ButtonsPanel { get { return buttonsPanelCore; } }
		protected override CreateParams CreateParams {
			get {
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 0x00100000;
				return createParams;
			}
		}
		protected override IBaseNavigationPageDefaultProperties CreateDefaultProperties(IBaseNavigationPageProperties parentProperties) {
			return new NavigationPageDefaultProperties(parentProperties);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AutoSizeMode AutoSizeMode {
			get { return base.AutoSizeMode; }
			set { base.AutoSizeMode = value; }
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			LayoutChanged(true);
		}
		ButtonCollection customHeaderButtonsCore;
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationPageCustomHeaderButtons")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.XtraBars.Docking.Design.CustomHeaderButtonCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesign,
			typeof(System.Drawing.Design.UITypeEditor)), Category("Custom Header Buttons"), Localizable(true)]
		public ButtonCollection CustomHeaderButtons {
			get { return customHeaderButtonsCore; }
		}
		[Category("Properties"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new INavigationPageDefaultProperties Properties {
			get { return base.Properties as NavigationPageDefaultProperties; }
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			UnsubscribeButtonsPanel();
		}
		void OnButtonClick(object sender, ButtonEventArgs e) {
			if(Owner is NavigationPane) {
				NavigationPane owner = Owner as NavigationPane;
				if(e.Button is CollapseButton)
					owner.State = NavigationPaneState.Collapsed;
				if(e.Button is ExpandButton) {
					if(owner.State != NavigationPaneState.Expanded)
						owner.State = NavigationPaneState.Expanded;
					else
						owner.State = NavigationPaneState.Default;
				}
			}
		}
		void OnCustomHeaderButtonsCollectionChanged(object sender, CollectionChangeEventArgs e) {
			LayoutChanged(true);
		}
		ButtonsPanel CreateButtonsPanel() {
			return new NavigationPageButtonsPanel(this);
		}
		protected override ObjectPainter CreatePainter() {
			return new NavigationPageSkinPainter(LookAndFeel);
		}
		void OnButtonsPanelChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		void OnEmbeddedButtonChecked(object sender, ButtonEventArgs e) {
			if(e.Button is DefaultButton) return;
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[customButtonChecked];
			if(handler != null) handler(this, e);
		}
		void OnEmbeddedButtonUnchecked(object sender, ButtonEventArgs e) {
			if(e.Button is DefaultButton) return;
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[customButtonUnchecked];
			if(handler != null) handler(this, e);
		}
		void OnEmbeddedButtonClick(object sender, ButtonEventArgs e) {
			if(e.Button is DefaultButton) return;
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[customButtonClick];
			if(handler != null) handler(this, e);
		}
		static readonly object customButtonClick = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("NavigationPageCustomButtonClick"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event ButtonEventHandler CustomButtonClick {
			add { this.Events.AddHandler(customButtonClick, value); }
			remove { this.Events.RemoveHandler(customButtonClick, value); }
		}
		static readonly object customButtonUnchecked = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("NavigationPageCustomButtonUnchecked"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event ButtonEventHandler CustomButtonUnchecked {
			add { this.Events.AddHandler(customButtonUnchecked, value); }
			remove { this.Events.RemoveHandler(customButtonUnchecked, value); }
		}
		static readonly object customButtonChecked = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("NavigationPageCustomButtonChecked"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event ButtonEventHandler CustomButtonChecked {
			add { this.Events.AddHandler(customButtonChecked, value); }
			remove { this.Events.RemoveHandler(customButtonChecked, value); }
		}
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			if(Owner != null && Owner is NavigationPane) {
				(Owner as NavigationPane).LayoutChanged();
			}
		}
		protected override void OnResize(EventArgs eventargs) {
			base.OnResize(eventargs);
			LayoutChanged();
		}
		protected override void SetOwner(INavigationPane owner) {
			base.SetOwner(owner);
			buttonsPanelCore.Buttons.ForEach((x) =>
			{
				if(x is ExpandButton)
					(x as ExpandButton).Owner = owner;
			});
		}
		protected internal override void LayoutChanged(bool updateOwner = false) {
			ButtonsPanel.ViewInfo.SetDirty();
			NavigationPane owner = Owner as NavigationPane;
			if(owner != null && updateOwner)
				owner.LayoutChanged();
		}
		NavigationPageViewInfo INavigationPageBase.ViewInfo { get { return ViewInfo; } }
		ButtonsPanel INavigationPage.ButtonsPanel {
			get { return ButtonsPanel; }
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_ERASEBKGND) {
				if(!DesignMode)
					EnsureIsBoundToControl();
				m.Result = new IntPtr(1);
			}
			base.WndProc(ref m);
		}
		void IButtonsPanelOwner.Invalidate() {
			var navigationPane = Owner as NavigationPane;
			if(navigationPane != null && navigationPane.IsHandleCreated)
				DevExpress.Utils.Drawing.Helpers.NativeMethods.RedrawWindow(navigationPane.Handle, IntPtr.Zero, IntPtr.Zero, 0x401);
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return false; }
		}
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return true; }
		}
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return null; }
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			return new NavigationPageButtonsPanelPainter(LookAndFeel);
		}
		object IButtonsPanelOwner.Images {
			get { return null; }
		}
		bool IButtonsPanelOwner.IsSelected {
			get { return false; }
		}
	}
	#region Buttons
	public class ExpandButton : BasePinButton, IButton {
		public INavigationPane Owner { get; set; }
		public ExpandButton() {
			Visible = true;
		}
		public override bool Checked {
			get {
				if(Owner != null)
					return Owner.State == NavigationPaneState.Expanded;
				return base.Checked;
			}
		}
	}
	public class CollapseButton : DefaultButton, IButton {
		public CollapseButton() {
			Visible = true;
		}
	}
	public class NavigationPageButtonsPanelPainter : BaseButtonsPanelSkinPainter {
		public NavigationPageButtonsPanelPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new NavigationPageButtonPainter(Provider);
		}
	}
	public class NavigationPageButtonPainter : BaseButtonSkinPainter {
		public NavigationPageButtonPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetBackground() {
			SkinElement result = GetNavigationPaneSkin()[NavigationPaneSkins.SkinPageButtons];
			if(result != null) return result;
			return base.GetBackground();
		}
		protected override int GetImageIndex(BaseButtonInfo info) {
			if(!(info.Button is DefaultButton))
				return base.GetImageIndex(info);
			int imageIndex = 0;
			int checkedIndex = 0;
			if(info.Button.Properties.Checked)
				checkedIndex = 3;
			imageIndex = checkedIndex;
			if(info.Selected)
				imageIndex = checkedIndex + 3;
			if(info.Hot)
				imageIndex = checkedIndex + 1;
			if(info.Pressed)
				imageIndex = checkedIndex + 2;
			return imageIndex;
		}
		protected override ImageCollection GetGlyphs(IBaseButton button) {
			if(!(button is IDefaultButton)) return null;
			using(ImageCollection images = GetSkinButtonImages()) {
				if(images == null) return null;
				ImageCollection result = new ImageCollection();
				result.ImageSize = images.ImageSize;
				if(button is CollapseButton) {
					result.Images.Add(images.Images[3]);
					result.Images.Add(images.Images[4]);
					result.Images.Add(images.Images[5]);
				}
				if(button is ExpandButton) {
					result.Images.Add(images.Images[0]);
					result.Images.Add(images.Images[1]);
					result.Images.Add(images.Images[2]);
					result.Images.Add(images.Images[6]);
					result.Images.Add(images.Images[7]);
					result.Images.Add(images.Images[8]);
				}
				return result;
			}
		}
		protected override ImageCollection GetSkinButtonImages() {
			SkinElement buttonElement = GetNavigationPaneSkin()[NavigationPaneSkins.SkinPageButtons];
			ImageCollection result = new ImageCollection();
			Image image = GetDefaultButtonImage();
			int imageCount = 9;
			bool verticalLayout = true;
			if(buttonElement != null && buttonElement.Glyph != null) {
				image = buttonElement.Glyph.Image;
				verticalLayout = (buttonElement.Glyph.Layout == SkinImageLayout.Vertical);
				imageCount = buttonElement.Glyph.ImageCount;
			}
			if(verticalLayout) {
				result.ImageSize = new Size(image.Width, image.Height / imageCount);
				result.Images.AddImageStripVertical(image);
			}
			else {
				result.ImageSize = new Size(image.Width / imageCount, image.Height);
				result.Images.AddImageStrip(image);
			}
			return result;
		}
		protected override void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			SkinElement buttonElement = GetNavigationPaneSkin()[NavigationPaneSkins.SkinPageButtons];
			if(!(info.Button is IDefaultButton)) { base.DrawImage(cache, info); return; }
			if(buttonElement != null && buttonElement.Glyph != null) { base.DrawImage(cache, info); return; }
			Color color = GetBackgroundColor(GetStateIndex(info));
			Image image = GetActualImage(info);
			Rectangle r = info.ImageBounds;
			RotateObjectPaintHelper rotateHelper = new RotateObjectPaintHelper();
			ImagePainterInfo painterInfo = new ImagePainterInfo(info.Cache, r, r.Size, image, AppearanceObject.EmptyAppearance);
			painterInfo.ImageAttributes = ImageColorizer.GetColoredAttributes(color);
			painterInfo.State = info.State;
			ObjectPainter imagePainter = (ObjectPainter)new ColoredImagePainter();
			RotateFlipType flipType = RotateFlipType.RotateNoneFlipNone;
			var navigationPage = info.ButtonPanelOwner as INavigationPage;
			if(navigationPage != null && navigationPage.Parent is INavigationFrame) {
				flipType = rotateHelper.RotateFlipTypeByRotationAngle(0, (navigationPage.Parent as INavigationFrame).IsRightToLeftLayout());
			}
			rotateHelper.DrawRotated(info.Cache, painterInfo, imagePainter, flipType);
		}
		protected virtual Image GetDefaultButtonImage() {
			return Docking2010.Resources.CommonResourceLoader.GetImage("NavigationPageButtonGlyphs");
		}
		protected override Color GetColor() {
			Color normalColor = GetNavigationPaneSkin().Colors.GetColor(NavigationPaneSkins.DefaultNavigationPageButtonNormalColor);
			if(!normalColor.IsEmpty) return normalColor;
			return base.GetColor();
		}
		protected override Color GetHotColor(Color defaultColor) {
			Color hotColor = GetNavigationPaneSkin().Colors.GetColor(NavigationPaneSkins.DefaultNavigationPageButtonHotColor);
			if(!hotColor.IsEmpty) return hotColor;
			return base.GetHotColor(defaultColor);
		}
		protected override Color GetPressedColor(Color defaultColor) {
			Color pressedColor = GetNavigationPaneSkin().Colors.GetColor(NavigationPaneSkins.DefaultNavigationPageButtonPressedColor);
			if(!pressedColor.IsEmpty) return pressedColor;
			return base.GetPressedColor(defaultColor);
		}
		protected virtual Skin GetNavigationPaneSkin() {
			return NavigationPaneSkins.GetSkin(SkinProvider);
		}
	}
	public class NavigationPageButtonsPanel : ButtonsPanel, IButtonsPanel {
		public NavigationPageButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
		}
		void IButtonsPanel.PerformClick(IBaseButton button) {
			if(Buttons.Contains(button)) {
				IButtonsPanelOwnerEx ownerEx = Owner as IButtonsPanelOwnerEx;
				if(ownerEx != null && !ownerEx.CanPerformClick(button)) return;
				if(button.Properties.Style == ButtonStyle.CheckButton) {
					CheckButtonGroupIndex(button);
					button.Properties.BeginUpdate();
					if(button is ExpandButton) {
						RaiseButtonClick(button);
						return;
					}
					button.Properties.Checked = CalcNewCheckedState(button);
					button.Properties.CancelUpdate();
				}
				else RaiseButtonClick(button);
			}
		}
	}
	#endregion Buttons
}
