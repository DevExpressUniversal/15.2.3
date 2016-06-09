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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Registration;
using DevExpress.XtraBars.Ribbon.Accessible;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraBars.Ribbon {
	public enum RibbonQuickAccessToolbarLocation { Default, Above, Below, Hidden }
	public enum ShowPageHeadersMode { Default, Hide, Show, ShowOnMultiplePages }
	public enum RibbonMdiMergeStyle { Default, Always, Never, OnlyWhenMaximized } 
	public class RibbonMergeEventArgs : EventArgs {
		public RibbonMergeEventArgs(RibbonControl mergedChild) {
			MergedChild = mergedChild;
		}
		public RibbonMergeEventArgs(RibbonControl mergedChild, RibbonControl mergeOwner) {
			MergedChild = mergedChild;
			MergeOwner = mergeOwner;
		}
		public RibbonControl MergedChild { get; internal set; }
		public RibbonControl MergeOwner { get; internal set; }
	}
	public class MinimizedRibbonEventArgs : EventArgs {
		public RibbonControl MinimizedRibbon { get; set; }
	}
	public class RibbonPageChangingEventArgs : CancelEventArgs {
		RibbonPage page = null;
		public RibbonPageChangingEventArgs(RibbonPage page, bool cancel) : base(cancel) {
			this.page = page;
		}
		public RibbonPageChangingEventArgs(bool cancel) : this(null, cancel) { }
		public RibbonPageChangingEventArgs(RibbonPage page) : base() {
			this.page = page;
		}
		public RibbonPage Page { get { return page; } set { page = value; } }
	}
	public class ScreenModeChangedEventArgs : EventArgs {
		bool fullScreen;
		public ScreenModeChangedEventArgs(bool fullScreen) {
			this.fullScreen = fullScreen;
		}
		public bool FullScreen { get { return fullScreen; } }
	}
	public class InvalidLayoutExceptionEventArgs : EventArgs {
		public InvalidLayoutExceptionEventArgs(Exception exception) {
			this.Handled = false;
			this.Exception = exception;
		}
		public bool Handled { get; set; }
		public Exception Exception { get; private set; }
	}
	public class ResetLayoutEventArgs : EventArgs {
		public ResetLayoutEventArgs(RibbonControl ribbon) {
			this.Ribbon = ribbon;
		}
		public RibbonControl Ribbon { get; private set; }
	}
	public class CustomizeQatMenuEventArgs : EventArgs {
		BarItemLinkCollection itemLinks;
		public CustomizeQatMenuEventArgs(BarItemLinkCollection itemLinks) {
			this.itemLinks = itemLinks;
		}
		public BarItemLinkCollection ItemLinks { get { return itemLinks; } }
	}
	public delegate void CustomizeQatMenuEventHandler(object sender, CustomizeQatMenuEventArgs e);
	public delegate void RibbonPageChangingEventHandler(object sender, RibbonPageChangingEventArgs e);
	public class RibbonPageHeaderItemLinkCollection : BaseRibbonItemLinkCollection { 
		RibbonControl ribbon;
		public RibbonPageHeaderItemLinkCollection(RibbonControl ribbon) : base() {
			this.ribbon = ribbon;
		}
		public override RibbonControl Ribbon { get { return ribbon; } }
		protected override void OnCollectionChanged(CollectionChangeEventArgs e) {
			base.OnCollectionChanged(e);
			if(Ribbon != null) Ribbon.Refresh();
		}
		protected override ISupportAdornerUIManager GetVisualEffectsOwner() { return Ribbon; }
		protected override bool GetVisulEffectsVisible() { return Ribbon != null && Ribbon.Visible; }
	}
	public enum RibbonControlStyle { 
		Default,
		Office2007,
		Office2010,
		Office2013,
		MacOffice,
		TabletOffice,
		[Obsolete("Use RibbonControlStyle.OfficeUniversal"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		TabletOfficeEx, 
		OfficeUniversal
	}
	public enum RibbonControlColorScheme { Yellow = 0, Blue = 1, Green = 2, Orange = 3, Purple = 4 }
	public class RibbonDesignTimeBoundsProviderAttribute : DesignTimeBoundsProviderAttribute {
		public override Control GetOwnerControl(object obj) {
			return (RibbonControl)obj;
		}
		public override bool ShouldDrawSelection { get { return false; } }
		public override Rectangle GetBounds(object obj) {
			return ((RibbonControl)obj).RectangleToScreen(((RibbonControl)obj).ClientRectangle);
		}
	}
	[DXToolboxItem(true), Designer("DevExpress.XtraBars.Ribbon.Design.RibbonControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	 ProvideProperty("PopupContextMenu",typeof(Control)),
	 Description("Displays various commands (buttons, editors, galleries), by categorizing them into tab pages and page groups. "),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "RibbonControl")
]
	public class RibbonControl : ControlBase, IBarObject, IXtraSerializable, ICustomBarControl, IExtenderProvider, IDXMenuManager, IDXDropDownMenuManager, ISupportXtraAnimation, IToolTipControlClientEx, IEditorBackgroundProvider, ISupportInitialize, ISupportGlassRegions, IGestureClient, IBackstageViewAnimationListener, ISupportAdornerUIManager {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool AllowMergeInvisibleItems { get; set; }
		internal static Color DefaultColor = Color.FromArgb(255, 255, 175, 3);
		private static readonly object pageGroupCaptionButtonClick = new object();
		private static readonly object selectedPageChanging = new object();
		private static readonly object selectedPageChanged = new object();
		private static readonly object screenModeChanged = new object();
		private static readonly object applicationButtonClick = new object();
		private static readonly object applicationButtonDoubleClick = new object();
		private static readonly object showCustomizationMenu = new object();
		private static readonly object merge = new object();
		private static readonly object beforeUnmerge = new object();
		private static readonly object unMerge = new object();
		private static readonly object minimizedChanged = new object();
		private static readonly object toolbarLocationChanged = new object();
		private static readonly object afterApplicationButtonContentControlHidden = new object();
		private static readonly object beforeApplicationButtonContentControlShow = new object();
		private static readonly object colorSchemeChanged = new object();
		private static readonly object ribbonStyleChanged = new object();
		private static readonly object invalidSaveRestoreLayoutException = new object();
		private static readonly object resetLayout = new object();
		private static readonly object customizeQatMenu = new object();
		private static readonly object minimizedRibbonShow = new object();
		private static readonly object minimizedRibbonHide = new object();
		public static bool AllowSystemShortcuts = false;
		bool needCheckHeight = false;
		DevExpress.XtraBars.Ribbon.Handler.BaseHandler handler;
		RibbonPageCategoryCollection pageCategories;
		RibbonPageCategoryCollection mergedCategories;
		RibbonMiniToolbarCollection miniToolbars;
		RibbonViewInfo viewInfo;
		int pageHeaderMinWidth;
		RibbonPage selectedPage;
		RibbonBarManager manager;
		MinimizedRibbonPopupForm minimizedRibbonPopupForm;
		RibbonMinimizedGroupPopupForm popupGroupForm;
		RibbonQuickToolbarPopupForm popupToolbar;
		RibbonQuickAccessToolbar toolbar;
		RibbonQuickAccessToolbarLocation toolbarLocation;
		RibbonCustomizationPopupMenu customizationPopupMenu;
		RibbonPageHeaderItemLinkCollection pageHeaderItemLinks;
		RibbonStatusBar statusBar;
		Bitmap applicationIcon;
		string applicationButtonText = string.Empty;
		string applicationCaption = string.Empty, applicationDocumentCaption = string.Empty;
		object applicationButtonDropDownControl;
		bool destroying;
		bool minimized = false;
		bool managerInitialized = false;
		bool autoSizeItems = false;
		VertAlignment itemsVertAlign = VertAlignment.Default;
		VertAlignment buttonGroupsVertAlign = VertAlignment.Default;
		string applicationButtonAccessibleName = string.Empty;
		string applicationButtonAccessibleDescription = string.Empty;
		string applicationButtonKeyTip = string.Empty;
		AccessibleRibbon accessibleRibbon = null;
		ShowPageHeadersMode showPageHeadersMode = ShowPageHeadersMode.Default;
		RibbonPageCategoryAlignment pageCategoryAlignment = RibbonPageCategoryAlignment.Default;
		bool transparentEditors;
		BarMdiButtonItem mdiMinimizeItem = null;
		BarMdiButtonItem mdiRestoreItem = null;
		BarMdiButtonItem mdiCloseItem = null;
		BarButtonItem expandCollapseItem = null;
		BarButtonItem autoHiddenPagesMenuItem = null;
		BarMdiButtonItemLink mdiMinimizeItemLink = null;
		BarMdiButtonItemLink mdiRestoreItemLink = null;
		BarMdiButtonItemLink mdiCloseItemLink = null;
		BarButtonItemLink expandCollapseItemLink = null;
		BarButtonItemLink autoHiddenPagesMenuItemLink = null;
		RibbonControl mergedRibbon = null;
		int itemAnimationLength = -1;
		int groupAnimationLength = -1;
		int pageAnimationLength = -1;
		int applicationButtonAnimationLength = -1;
		int galleryAnimationLength = -1;
		RibbonMdiMergeStyle mergeStyle = RibbonMdiMergeStyle.Default;
		bool showCategoryInCaption = true;
		bool autoHideEmptyItems = false;
		bool allowKeyTips = true;
		SuperToolTip applicationButtonSuperTip;
		bool allowMinimizeRibbon = true;
		RibbonControlStyle ribbonStyle;
		DefaultBoolean showExpandCollapseButton;
		bool allowTrimPageText = true;
		RibbonControlColorScheme colorScheme;
		DefaultBoolean showApplicationButton;
		DefaultBoolean showFullScreenButton;
		bool allowMdiChildButtons = true;
		Image renderImage;
		CachedColoredImage fullScreenBarGlyph;
		bool drawGroupsBorder = true;
		DefaultBoolean drawGroupCaptions = DefaultBoolean.Default;
		bool showItemCaptionInQuickAccessToolbar;
		bool showItemCaptionInPageHeader;
		public static void About() {
		}
		static RibbonControl() {
			AllowMergeInvisibleItems = true;
			BarEditorsRepositoryItemRegistrator.Register();
		}
		public RibbonControl() {
#if DXWhidbey            
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
#else
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
#endif
			SetStyle(ControlStyles.FixedHeight, true);
			SetStyle(ControlStyles.UserMouse, false);
			SetStyle(ControlStyles.Selectable, false);
			base.AutoSize = IsAutoSizeRibbon;
			this.minimized = false;
			this.fullScreenBarGlyph = new CachedColoredImage(DefaultFullScreenBarGlyphIcon);
			this.manager = CreateBarManager();
			this.toolbar = CreateToolbar();
			this.pageHeaderMinWidth = 0;
			this.toolbarLocation = RibbonQuickAccessToolbarLocation.Default;
			if(ShouldInitOrDestroyBarManager) Manager.BeginInit();
			this.Dock = DockStyle.Top;
			base.TabStop = false;
			this.statusBar = null;
			this.ribbonStyle = RibbonControlStyle.Default;
			ToolTipController.DefaultController.AddClientControl(this);
			this.showExpandCollapseButton = DefaultBoolean.Default;
			this.colorScheme = RibbonControlColorScheme.Yellow;
			this.showApplicationButton = DefaultBoolean.Default;
			this.showFullScreenButton = DefaultBoolean.Default;
			this.ShowQatLocationSelector = true;
			this.OptionsCustomizationForm = CreateRibbonCustomizationFormOptions();
			this.allowHtmlText = false;
			this.renderImage = null;
			UpdateRegistrationInfo();
		}
		[DefaultValue(PopupMenuAlignment.Default), XtraSerializableProperty]
		public PopupMenuAlignment PopupMenuAlignment { get { return Manager.PopupMenuAlignment; } set { Manager.PopupMenuAlignment = value; } }
		ButtonGroupsLayout buttonGroupsLayout = ButtonGroupsLayout.Default;
		[DefaultValue(ButtonGroupsLayout.Default), XtraSerializableProperty]
		public ButtonGroupsLayout ButtonGroupsLayout {
			get { return buttonGroupsLayout; }
			set {
				if(ButtonGroupsLayout == value)
					return;
				buttonGroupsLayout = value;
				Refresh();
			}
		}
		internal ButtonGroupsLayout GetButtonGroupsLayout() {
			if(ButtonGroupsLayout == XtraBars.ButtonGroupsLayout.Default)
				return XtraBars.ButtonGroupsLayout.Auto;
			return ButtonGroupsLayout;
		}
		List<RibbonRegistrationInfo> registrationInfo;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<RibbonRegistrationInfo> RegistrationInfo {
			get {
				if(registrationInfo == null) {
					registrationInfo = new List<RibbonRegistrationInfo>();
					AddDefaultRegistrationInfo();
				}
				return registrationInfo; 
			}
		}
		internal RibbonRegistrationInfo DefaultRegistrationInfo {
			get {
				if(RegistrationInfo.Count > 0)
					return RegistrationInfo[0];
				return new RibbonDefaultRegistrationInfo(this);
			}
		}
		internal Type DefaultPageCategoryType {
			get {
				if(RegistrationInfo.Count > 0 && RegistrationInfo[0].PageCategoryType != null)
					return RegistrationInfo[0].PageCategoryType;
				return new RibbonDefaultRegistrationInfo(this).PageCategoryType;
			}
		}
		internal Type DefaultPageType {
			get {
				if(RegistrationInfo.Count > 0 && RegistrationInfo[0].PageType != null)
					return RegistrationInfo[0].PageType;
				return new RibbonDefaultRegistrationInfo(this).PageType;
			}
		}
		internal Type DefaultPageGroupType {
			get {
				if(RegistrationInfo.Count > 0 && RegistrationInfo[0].PageGroupType != null)
					return RegistrationInfo[0].PageGroupType;
				return new RibbonDefaultRegistrationInfo(this).PageGroupType;
			}
		}
		protected virtual void AddDefaultRegistrationInfo() {
			RegistrationInfo.Add(new RibbonDefaultRegistrationInfo(this));
		}
		protected virtual void UpdateRegistrationInfo() { 
		}
		protected virtual bool IsAutoSizeRibbon { get { return true; } }
		protected virtual RibbonQuickAccessToolbar CreateToolbar() {
			return new RibbonQuickAccessToolbar(this);
		}
		protected override void Dispose(bool disposing) {
			this.destroying = true;
			if(disposing) {
				if(fullScreenBarGlyph != null) fullScreenBarGlyph.Dispose();
				fullScreenBarGlyph = null;
				if(renderImage != null) renderImage.Dispose();
				renderImage = null;
				RibbonSourceStateInfo.Remove(this);
				handler = new EmptyHandler();
				DestroyPopupForms();
				MinimizedRibbonPopupForm = null;
				if(ShouldInitOrDestroyBarManager && Manager != null) Manager.Dispose();
				DestroyPages();
				DestroyPageCategories();
				Toolbar.Dispose();
				if(StatusBar != null)StatusBar.Ribbon = null;
				CustomizationPopupMenu = null;
				ToolTipController = null;
				ToolTipController.DefaultController.RemoveClientControl(this);
				if(KeyTipManager.Show) KeyTipManager.HideKeyTips();
				KeyTipManager.ClearItems();
				UnsubscribeParentEvents();
				if(this.applicationButtonContentControl != null)
					this.applicationButtonContentControl.Dispose();
				this.applicationButtonContentControl = null;
				parentForm = null;
			}
			base.Dispose(disposing);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set { }
		}
		protected override void SetVisibleCore(bool value) {
			if(!this.patchVisible)
				this.userVisible = value;
			base.SetVisibleCore(value);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			ClosePopupForms();
			RefreshHeight();
		}
		protected override void OnLocationChanged(EventArgs e) {
			UpdateVisualEffects(UpdateAction.BeginUpdate);
			base.OnLocationChanged(e);
			needCheckHeight = true;
			CheckHeight();
			UpdateVisualEffects(UpdateAction.EndUpdate);
		}
		protected override void OnParentVisibleChanged(EventArgs e) {
			base.OnParentVisibleChanged(e);
			ClosePopupForms();
		}
		internal void AssignToMinimized(RibbonMinimizedControl ribbon) { }
		#region MouseEvents
		static object[] mouseEvents = null;
		protected static object[] MouseEvents {
			get {
				if(mouseEvents == null) mouseEvents = LoadMouseEvents();
				return mouseEvents;
			}
		}
		static object[] LoadMouseEvents() {
			return new object[] { GetEventKeyObject("EventMouseMove"), GetEventKeyObject("EventMouseDown") };
		}
		protected internal virtual void SubscribeMouseEvents(RibbonControl targetRibbon) {
			for(int i = 0; i < MouseEvents.Length; i++)
				targetRibbon.Events.AddHandler(MouseEvents[i], Events[MouseEvents[i]]);
		}
		protected internal virtual void UnsubscribeMouseEvents(RibbonControl targetRibbon) {
			for(int i = 0; i < MouseEvents.Length; i++)
				targetRibbon.Events.RemoveHandler(MouseEvents[i], Events[MouseEvents[i]]);
		}
		#endregion
		protected static object GetEventKeyObject(string name) {
			FieldInfo fi = typeof(Control).GetField(name, BindingFlags.Static | BindingFlags.NonPublic);
			return fi.GetValue(null);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Padding Padding {
			get { return Padding.Empty; }
			set { }
		}
		[ThreadStatic]
		static Bitmap defaultApplicationIcon, defaultApplicationIcon2010;
		internal static Bitmap DefaultApplicationIcon {
			get {
				if(defaultApplicationIcon == null) {
					defaultApplicationIcon = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars.Images.RibbonApplication.png", typeof(RibbonControl).Assembly) as Bitmap;
				}
				return defaultApplicationIcon;
			}
		}
		internal static Bitmap DefaultApplicationIcon2010 {
			get {
				if(defaultApplicationIcon2010 == null) {
					string resName = DpiProvider.Default.DpiScaleFactor == 2.0f ? "DevExpress.XtraBars.Images.RibbonApplication2010_200dpi.png" : "DevExpress.XtraBars.Images.RibbonApplication2010.png";
					defaultApplicationIcon2010 = ResourceImageHelper.CreateImageFromResources(resName, typeof(RibbonControl).Assembly) as Bitmap;
				}
				return defaultApplicationIcon2010;
			}
		} 
		internal static Bitmap GetDefaultApplicationIcon(RibbonControl ribbon) {
			if(ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007)
				return DefaultApplicationIcon;
			return DefaultApplicationIcon2010;
		}
		[ThreadStatic]
		static Bitmap defaultFullScreenBarGlyphIcon;
		internal static Bitmap DefaultFullScreenBarGlyphIcon {
			get {
				if(defaultFullScreenBarGlyphIcon == null) {
					defaultFullScreenBarGlyphIcon = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars.Images.FullScreenBarGlyph.png", typeof(RibbonControl).Assembly) as Bitmap;
				}
				return defaultFullScreenBarGlyphIcon;
			}
		}
		protected internal CachedColoredImage FullScreenBarGlyph {
			get { return this.fullScreenBarGlyph; }
		}
		protected internal RibbonPageCategory[] GetPageCategories() {
			return PageCategories.GetPageCategories(MergedCategories);
		}
		public RibbonPageGroup GetGroupByName(string name) {
			foreach(RibbonPage page in TotalPageCategory.Pages) {
				RibbonPageGroup group = page.GetGroupByName(name);
				if(group != null) return group;
			}
			return null;
		}
		protected void DestroyPageCategories() {
			PageCategories.Destroy();
		}
		protected void DestroyPages() {
			TotalPageCategory.Pages.Destroy();
		}
		protected internal virtual object InternalGetService(Type type) {
			return GetService(type);
		}
		internal bool IsForceGraphicsInitialize { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ForceGraphicsInitialize() {
			IsForceGraphicsInitialize = true;
			try {
				using(Graphics g = Graphics.FromImage(new Bitmap(10, 10))) {
					OnPaint(new PaintEventArgs(g, new Rectangle(0, 0, 2000, 200)));
				}
			}
			finally {
				IsForceGraphicsInitialize = false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public BaseRibbonDesignTimeManager GetDesignTimeManager() {
			return ViewInfo.DesignTimeManager;
		}
		public virtual void ForceInitialize() {
			ForceInitialize(false);
		}
		public virtual void ForceInitialize(bool refreshLinks) {
			if(refreshLinks && (Manager != null && Manager.Helper.LoadHelper.Loaded)) {
				Manager.Helper.LoadHelper.Loaded = false;
			}
			if(!ShouldInitOrDestroyBarManager) return;
			if(!managerInitialized) {
				managerInitialized = true;
				Manager.EndInit();
			}
			Manager.FillPostponedRepositories();
			Manager.ForceInitialize();
		}
		protected override void WndProc(ref Message m) {
			if(ViewInfo.Caption.ProcessMessage(ref m)) return;
			if(m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEACTIVATE) {
				Form topLevelForm = Manager.GetForm();
				if(topLevelForm != null && m.WParam == topLevelForm.Handle) {
					var controller = Manager.GetController();
					if(controller != null) {
						var hitInfo = ViewInfo.CalcHitInfo(PointToClient(Control.MousePosition));
						if(hitInfo.HitTest == RibbonHitTest.ApplicationButton) {
							if(!ViewInfo.ApplicationButtonPopupActive) {
								var applicationMenu = ApplicationButtonDropDownControl as PopupMenu;
								if(applicationMenu != null && applicationMenu.CanShowPopup) {
									if(controller.NotifyBarMouseActivateClients(Manager, ref m))
										return;
								}
							}
						}
						if(hitInfo.InItem) {
							if(hitInfo.Item != null && hitInfo.Item.Enabled) {
								if(controller.NotifyBarMouseActivateClients(Manager, ref m))
									return;
							}
						}
						if(hitInfo.InPageGroup) {
							if(hitInfo.PageGroup != null && hitInfo.PageGroup.Enabled) {
								if(controller.NotifyBarMouseActivateClients(Manager, ref m))
									return;
							}
						}
						if(hitInfo.InPageCategory) {
							if(controller.NotifyBarMouseActivateClients(Manager, ref m))
								return;
						}
					}
				}
			}
			if(GestureHelper.WndProc(ref m))
				return;
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		public ReduceOperationCollection ReduceOperations { 
			get {
				return SelectedPage == null ? null : SelectedPage.ReduceOperations;
			} 
		}
		void ResetColorScheme() { ColorScheme = RibbonControlColorScheme.Yellow; }
		bool ShouldSerializeColorScheme() { return ColorScheme != RibbonControlColorScheme.Yellow; }
		[Category("Appearance"), XtraSerializableProperty]
		public RibbonControlColorScheme ColorScheme {
			get { return colorScheme; }
			set {
				if(ColorScheme == value)
					return;
				colorScheme = value;
				OnColorSchemeChanged();
			}
		}
		protected virtual void OnColorSchemeChanged() {
			Refresh();
			BackstageViewControl backstageView = ApplicationButtonDropDownControl as BackstageViewControl;
			if(backstageView != null)
				backstageView.Refresh();
		}
		ToolTipController toolTipController;
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlToolTipController"),
#endif
 DefaultValue((string)null)]
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
				} else ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		void IToolTipControlClientEx.OnBeforeShow(ToolTipControllerShowEventArgs e) {
			if(e.SelectedObject is BarItemLink) {
				((BarItemLink)e.SelectedObject).OnBeforeShowHint(e);
			}
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			if(!ViewInfo.IsRibbonFormActive) {
				if(!(Form.ActiveForm is RibbonBasePopupForm))
					return null;
			}
			ToolTipControlInfo res = GetToolTipInfo(point);
			if(res != null) res.ToolTipType = ToolTipType.SuperTip;
			return res;
		}
		bool IToolTipControlClient.ShowToolTips { get { return !DesignMode; } }
		protected virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			if(Manager.SelectionInfo.OpenedPopups.Count > 0) return null;
			ToolTipControlInfo res = ViewInfo.GetToolTipInfo(point);
			EditorToolTipControlInfo editorToolTipInfo = res as EditorToolTipControlInfo;
			BarItemLink link = null;
			if(editorToolTipInfo != null) {
				link = editorToolTipInfo.EditLink;
				res = editorToolTipInfo.ToolTipInfo;
			}
			else if(res != null) {
				link = res.Object as BarItemLink;
			}
			if(res == null || res.ToolTipPosition.X != -10000) return res;
			Point location = new Point(Cursor.Position.X, PointToScreen(new Point(0, Height)).Y);
			if(link != null) {
				if(LinkInToolbarsCore(link)) return res;
				if(!link.Bounds.IsEmpty) location.X = PointToScreen(link.Bounds.Location).X;
			}
			res.ToolTipPosition = location;
			res.ToolTipLocation = ToolTipLocation.Fixed;
			return res;
		}
		protected internal bool LinkInToolbarsCore(BarItemLink link) {
			object linkedObject = link.LinkedObject;
			if(linkedObject is BarButtonGroup) {
				BarButtonGroup bg = (BarButtonGroup)linkedObject;
				foreach(BarItemLink headerLink in PageHeaderItemLinks) {
					BarButtonGroupLink gl = headerLink as BarButtonGroupLink;
					if(gl != null && gl.Item == bg)
						return true;
				}
			}
			return linkedObject == Toolbar.CustomizeItem || linkedObject == Toolbar.DropDownItem || linkedObject == Toolbar.ItemLinks || linkedObject == PageHeaderItemLinks;
		}
		bool IExtenderProvider.CanExtend(object target) { return ((IExtenderProvider)Manager).CanExtend(target); }
		[DefaultValue(null), Category("Ribbon")]
		public PopupMenuBase GetPopupContextMenu(Control control) { return Manager.GetPopupContextMenu(control); }
		public void SetPopupContextMenu(Control control, PopupMenuBase menu) { Manager.SetPopupContextMenu(control, menu); }
		[Browsable(false), DefaultValue(null)]
		public RibbonStatusBar StatusBar { get { return statusBar; } set { statusBar = value; } }
		[DefaultValue(PopupShowMode.Default), XtraSerializableProperty]
		public PopupShowMode PopupShowMode {
			get;
			set;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlDock"),
#endif
 DefaultValue(DockStyle.Top), XtraSerializableProperty]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = value; }
		}
		[DefaultValue(false), Browsable(false), SkipRuntimeSerialization]
		public virtual bool Minimized {
			get { return minimized; }
			set {
				if(Minimized == value || !CanMinimize(value)) return;
				minimized = value;
				MinimizedRibbonPopupForm = null;
				Manager.SelectionInfo.CloseAllPopups();
				RefreshHeight();
				RaiseMinimizedChanged();
			}
		}
		protected internal bool InFullScreen { get { return ViewInfo.IsFullScreenModeActive || ViewInfo.IsPopupFullScreenModeActive; } }
		protected virtual bool CanMinimize(bool minimized) {
			if(!minimized) return true;
			if(InFullScreen)
				return false;
			return AllowMinimizeRibbon;
		}
		protected virtual void ResetAccessible() {
			AccessibleRibbon.Recreate();
		}
		protected internal static int AccessibleObjectRibbonPageList = 1;
		protected internal static int AccessibleObjectRibbonApplicationButton = 2;
		protected internal static int AccessibleObjectRibbonToolbarItem = 3;
		protected internal static int AccessibleGroupsBeginIndex = 10;
		protected internal static int AccessibleItemsBeginIndex = 100;
		protected internal static int AccessibleGalleryItemsBeginIndex = 10000;
#if DXWhidbey        
		protected override AccessibleObject GetAccessibilityObjectById(int objectId) {
			int index = objectId & 0xffff;
			if(objectId == RibbonControl.AccessibleObjectRibbonPageList)
				return AccessibleRibbon.Children[2].Accessible;
			else if(objectId == RibbonControl.AccessibleObjectRibbonApplicationButton)
				return AccessibleRibbon.Children[0].Accessible;
			else if(objectId == AccessibleObjectRibbonToolbarItem)
				return AccessibleRibbon.Children[1].Accessible;
			else if(objectId >= AccessibleGroupsBeginIndex && objectId < AccessibleItemsBeginIndex) {
				int childItemIndex = objectId - AccessibleGroupsBeginIndex;
				if(childItemIndex < AccessibleRibbon.Children[3].Children[0].Children.Count)
					return AccessibleRibbon.Children[3].Children[0].Children[childItemIndex].Accessible;
			}
			else if(objectId >= AccessibleItemsBeginIndex && objectId < AccessibleGalleryItemsBeginIndex)
				return AccessibleRibbon.Children[3].Children[0].Children[objectId - AccessibleItemsBeginIndex].Accessible;
			else if(objectId >= AccessibleGalleryItemsBeginIndex) {
				int groupIndex = (objectId - AccessibleGalleryItemsBeginIndex) / 1000;
				int itemIndex = objectId - AccessibleGalleryItemsBeginIndex - groupIndex * 1000;
				BaseAccessible acc = AccessibleRibbon.Children[3].Children[0].Children[groupIndex].Children[itemIndex];
				if(acc != null) return acc.Children[acc.GetChildCount() - 1].Accessible;
			}
			return AccessibleRibbon.Accessible;
		}
		public void AccessibleNotifyClients(AccessibleEvents accEvent, int objectId, int childId) {
			AccessibilityNotifyClients(accEvent, objectId, childId);
		}
#endif
		[Browsable(false)]
		public AccessibleRibbon AccessibleRibbon {
			get {
				if(accessibleRibbon == null) accessibleRibbon = CreateAccessibleRibbon();
				return accessibleRibbon;
			}
		}
		protected virtual AccessibleRibbon CreateAccessibleRibbon() { return new AccessibleRibbon(this); }
		protected override AccessibleObject CreateAccessibilityInstance() { return AccessibleRibbon.Accessible; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlAllowTrimPageText"),
#endif
 DefaultValue(true), Category("Behavior"), XtraSerializableProperty]
		public bool AllowTrimPageText {
			get { return allowTrimPageText; }
			set {
				if(AllowTrimPageText == value)
					return;
				allowTrimPageText = value;
				Refresh();
			}
		}
		[DefaultValue(false), XtraSerializableProperty]
		public bool ShowItemCaptionsInQAT {
			get { return showItemCaptionInQuickAccessToolbar; }
			set {
				if(ShowItemCaptionsInQAT == value)
					return;
				this.showItemCaptionInQuickAccessToolbar = value;
				Refresh();
			}
		}
		[DefaultValue(false), XtraSerializableProperty]
		public bool ShowItemCaptionsInPageHeader {
			get { return showItemCaptionInPageHeader; }
			set {
				if(ShowItemCaptionsInPageHeader == value)
					return;
				this.showItemCaptionInPageHeader = value;
				Refresh();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlAllowMdiChildButtons"),
#endif
 DefaultValue(true), Category("Behavior"), XtraSerializableProperty]
		public bool AllowMdiChildButtons {
			get { return allowMdiChildButtons; }
			set {
				if(AllowMdiChildButtons == value)
					return;
				allowMdiChildButtons = value;
				Refresh();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationButtonKeyTip"),
#endif
 DefaultValue(""), Category("Behavior"), XtraSerializableProperty]
		public string ApplicationButtonKeyTip {
			get { return applicationButtonKeyTip; }
			set {
				if(value != null) applicationButtonKeyTip = value.ToUpper();
				else applicationButtonKeyTip = string.Empty;
			}
		}
		bool applicationButtonDropDownControlVisible;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationButtonDropDownControl"),
#endif
 DefaultValue(null), Category("Behavior"), TypeConverter("DevExpress.XtraBars.TypeConverters.ApplicationButtonDropDownControlTypeConverter, " + AssemblyInfo.SRAssemblyBarsDesign)]
		public virtual object ApplicationButtonDropDownControl {
			get { return applicationButtonDropDownControl; }
			set {
				if(value != null && !(value is PopupControl) && !(value is Control))
					throw new Exception("Only PopupControl or Control can be assigned to ApplicationButtonDropDownControl");
				if(ApplicationButtonDropDownControl == value) return;
				OnApplicationButtonDropDownControlChanging();
				applicationButtonDropDownControl = value;
				OnApplicationButtonDropDownControlChanged();
			}
		}
		protected internal PopupControl ApplicationButtonPopupControl { get { return ApplicationButtonDropDownControl as PopupControl; } }
		protected internal Control ApplicationButtonControl { get { return ApplicationButtonDropDownControl as Control; } }
		protected virtual void OnApplicationButtonDropDownControlChanging() {
			Control appDropDownControl = ApplicationButtonDropDownControl as Control;
			if(appDropDownControl != null) {
				appDropDownControl.Visible = applicationButtonDropDownControlVisible;
			}
			if(ApplicationButtonPopupControl != null) {
				ApplicationButtonPopupControl.Popup -= new EventHandler(OnApplicationButtonDropDownControlPopup);
				ApplicationButtonPopupControl.CloseUp -= new EventHandler(OnApplicationButtonDropDownControlCloseUp);
			}
			BackstageViewControl bv = ApplicationButtonDropDownControl as BackstageViewControl;
			if(bv != null)
				bv.Ribbon = null;
		}
		protected virtual void OnApplicationButtonDropDownControlChanged() {
			ResetApplicationButtonContentControlCache();
			Control appDropDownControl = ApplicationButtonDropDownControl as Control;
			if(appDropDownControl != null) {
				applicationButtonDropDownControlVisible = appDropDownControl.Visible;
				if(!DesignMode)
					appDropDownControl.Visible = false;
			}
			if(ApplicationButtonPopupControl != null) {
				ApplicationButtonPopupControl.Popup += new EventHandler(OnApplicationButtonDropDownControlPopup);
				ApplicationButtonPopupControl.CloseUp += new EventHandler(OnApplicationButtonDropDownControlCloseUp);
			}
			BackstageViewControl bv = appDropDownControl as BackstageViewControl;
			if(bv != null)
				bv.Ribbon = this;
		}
		protected internal virtual void OnFullScreenButtonClicked(MouseEventArgs e) {
			Handler.OnFullScreenButtonClicked(e);
		}
		protected internal bool IsFullScreenModeActiveCore { get { return ViewInfo != null && ViewInfo.IsFullScreenModeActive; } }
		protected internal virtual void OnFullScreenModeChangeCore() {
			if(!ViewInfo.IsFullScreenModeActive) {
				ActivateFullScreenMode();
				CaptureRenderImageCore();
			}
			else {
				RestoreFromFullScreenMode();
			}
			ViewInfo.IsFullScreenModeActive = !ViewInfo.IsFullScreenModeActive;
			OnScreenModeChanged();
		}
		RibbonFormStateObject formStateObj;
		protected virtual void ActivateFullScreenMode() {
			Form form = ViewInfo.Form;
			if(form == null) return;
			this.formStateObj = RibbonFormHelper.CreateFormStateObject(form);
			if(Minimized) {
				Minimized = false;
				Refresh();
			}
			DisableExpandCollapseButton();
			form.WindowState = FormWindowState.Maximized;
		}
		protected virtual void RestoreFromFullScreenMode() {
			Form form = ViewInfo.Form;
			if(form == null) return;
			RibbonFormHelper.ApplyFormStateObject(form, this.formStateObj);
		}
		protected internal RibbonFormStateObject FormStateObj { get { return formStateObj; } }
		protected internal virtual void CaptureRenderImageCore() {
			if(ShouldCaptureRibbon) CaptureRibbonSourceState();
		}
		protected internal virtual void OnScreenModeChanged() {
			RefreshHeight();
			RaiseScreenModeChanged(new ScreenModeChangedEventArgs(ViewInfo.IsFullScreenModeActive));
		}
		protected override void OnClientSizeChanged(EventArgs e) {
			base.OnClientSizeChanged(e);
			if(ViewInfo.Form == null) return;
			CheckFullScreen();
		}
		protected virtual void CheckFullScreen() {
			FormWindowState state = ViewInfo.Form.WindowState;
			if(state == FormWindowState.Maximized)
				return;
			if(ViewInfo.IsFullScreenModeActive && state != FormWindowState.Minimized) {
				OnFullScreenModeChangeCore();
				return;
			}
			if(ViewInfo.IsPopupFullScreenModeActive) {
				if(state != FormWindowState.Maximized) {
					RestoreFromFullScreenMode();
					ViewInfo.IsPopupFullScreenModeActive = false;
					RaiseScreenModeChanged(new ScreenModeChangedEventArgs(false));
				}
				CheckHeight();
			}
		}
		protected bool ShouldCaptureRibbon {
			get {
				if(restoreFullScreenAfterBackstageClosing) return false;
				return true;
			}
		}
		protected internal virtual void CaptureRibbonSourceState() {
			if(renderImage != null) renderImage.Dispose();
			renderImage = BarUtilites.RenderToBitmap(this, ViewInfo.RenderImageOffset);
		}
		protected internal virtual void UpdateRenderImage(Image image) {
			if(renderImage != null) renderImage.Dispose();
			renderImage = image;
		}
		protected virtual void OnApplicationButtonDropDownControlPopup(object sender, EventArgs e) {
			Refresh();
		}
		protected virtual void OnApplicationButtonDropDownControlCloseUp(object sender, EventArgs e) {
			Refresh();
		}
		[RefreshProperties(RefreshProperties.All), Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlMdiMergeStyle"),
#endif
 DefaultValue(RibbonMdiMergeStyle.Default), XtraSerializableProperty]
		public RibbonMdiMergeStyle MdiMergeStyle {
			get { return mergeStyle; }
			set {
				if(MdiMergeStyle == value) return;
				mergeStyle = value;
				OnMdiMergeStyleChanged();
			}
		}
		private void OnMdiMergeStyleChanged() {
			UpdateIsMdiChildRibbon();
			if(ShouldMergeRibbon()) {
				Form frm = FindForm();
				MergeRibbon(FindMDIRibbon(frm.ActiveMdiChild));
			}
			else UnMergeRibbon();
			RefreshHeight();
		}
		protected virtual bool ShouldMergeRibbon() {
			Form frm = FindForm();
			if(frm == null || frm.ActiveMdiChild == null) return false;
			return ShouldMergeActivate(frm.ActiveMdiChild) || ShouldMergeMaximized(frm.ActiveMdiChild);
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlShowExpandCollapseButton"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty]
		public DefaultBoolean ShowExpandCollapseButton {
			get { return showExpandCollapseButton; }
			set {
				if(ShowExpandCollapseButton == value) return;
				showExpandCollapseButton = value;
				Refresh();
			}
		}
		protected internal bool GetShowExpandCollapseButton() {
			if(ShowExpandCollapseButton != DefaultBoolean.Default)
				return ShowExpandCollapseButton == DefaultBoolean.True? true: false;
			return GetRibbonStyle() != RibbonControlStyle.Office2007;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlDrawGroupsBorder"),
#endif
 DefaultValue(true), Category("Appearance"), XtraSerializableProperty]
		public bool DrawGroupsBorder {
			get { return drawGroupsBorder; }
			set {
				if(DrawGroupsBorder == value) return;
				drawGroupsBorder = value;
				RefreshHeight();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlDrawGroupCaptions"),
#endif
 DefaultValue(typeof(DefaultBoolean), "Default"), Category("Appearance"), XtraSerializableProperty]
		public DefaultBoolean DrawGroupCaptions {
			get { return drawGroupCaptions; }
			set {
				if(DrawGroupCaptions == value) return;
				drawGroupCaptions = value;
				RefreshHeight();
			}
		}
		[RefreshProperties(RefreshProperties.All), Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlShowPageHeadersMode"),
#endif
 DefaultValue(ShowPageHeadersMode.Default), XtraSerializableProperty]
		public ShowPageHeadersMode ShowPageHeadersMode {
			get { return showPageHeadersMode; }
			set {
				if(ShowPageHeadersMode == value) return;
				showPageHeadersMode = value;
				RefreshHeight();
			}
		}
		[RefreshProperties(RefreshProperties.All), Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlShowApplicationButton"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty]
		public DefaultBoolean ShowApplicationButton {
			get { return showApplicationButton; }
			set {
				if(ShowApplicationButton == value) return;
				showApplicationButton = value;
				Refresh();
			}
		}
		[RefreshProperties(RefreshProperties.All), Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlShowFullScreenButton"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty]
		public DefaultBoolean ShowFullScreenButton {
			get { return showFullScreenButton; }
			set {
				if(ShowFullScreenButton == value) return;
				showFullScreenButton = value;
				Refresh();
			}
		}
		[System.Obsolete("Use ShowPageHeadersMode instead of ShowPageHeaders"), Browsable(false), RefreshProperties(RefreshProperties.All), System.ComponentModel.Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlShowPageHeaders"),
#endif
 DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowPageHeaders {
			get {
				if(ShowPageHeadersMode == ShowPageHeadersMode.Default || ShowPageHeadersMode == ShowPageHeadersMode.Show) return true;
				if(ShowPageHeadersMode == ShowPageHeadersMode.Hide) return false;
				if(TotalPageCategory.GetVisiblePages().Count > 1) return true;
				return false;
			}
			set {
				if(value == true && (ShowPageHeadersMode == ShowPageHeadersMode.Default || ShowPageHeadersMode == ShowPageHeadersMode.Show)) return;
				if(value == true) ShowPageHeadersMode = ShowPageHeadersMode.Default;
				if(value == false) ShowPageHeadersMode = ShowPageHeadersMode.Hide;
				RefreshHeight();
			}
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlItemsVertAlign"),
#endif
 DefaultValue(VertAlignment.Default), XtraSerializableProperty]
		public virtual VertAlignment ItemsVertAlign {
			get { return itemsVertAlign; }
			set {
				if(ItemsVertAlign == value) return;
				itemsVertAlign = value;
				Refresh();
			}
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlButtonGroupsVertAlign"),
#endif
 DefaultValue(VertAlignment.Default), XtraSerializableProperty]
		public virtual VertAlignment ButtonGroupsVertAlign {
			get { return buttonGroupsVertAlign; }
			set {
				if(ButtonGroupsVertAlign == value) return;
				buttonGroupsVertAlign = value;
				Refresh();
			}
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlPageCategoryAlignment"),
#endif
 DefaultValue(RibbonPageCategoryAlignment.Default), XtraSerializableProperty]
		public RibbonPageCategoryAlignment PageCategoryAlignment {
			get { return pageCategoryAlignment; }
			set {
				if(PageCategoryAlignment == value) return;
				pageCategoryAlignment = value;
				Refresh();
			}
		}
		bool allowHtmlText;
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlAllowHtmlText"),
#endif
 DefaultValue(false), XtraSerializableProperty]
		public virtual bool AllowHtmlText{
			get { return allowHtmlText; }
			set {
				if(allowHtmlText == value) return;
				allowHtmlText = value;
				Refresh();
			}
		}
		Form parentForm = null;
		Form mdiChildForm = null;
		FormBorderStyle parentFormBorderStyle;
		void UnsubscribeParentEvents() {
			if(parentForm != null) {
				parentForm.MdiChildActivate -= new EventHandler(OnMdiChildActivate);
				parentForm.ParentChanged -= new EventHandler(OnFormParentChanged);
				parentForm.StyleChanged -= new EventHandler(OnParentFormStyleChanged);
				parentForm.Closing -= new CancelEventHandler(OnParentFormClosing);
			}
			else {
				if(Parent != null && Parent is UserControl)
					Parent.ParentChanged -= new EventHandler(Parent_ParentChanged);
			}
		}
		void SubscribeParentEvents() {
			if(parentForm != null) {
				parentForm.MdiChildActivate += new EventHandler(OnMdiChildActivate);
				parentForm.ParentChanged += new EventHandler(OnFormParentChanged);
				parentForm.StyleChanged += new EventHandler(OnParentFormStyleChanged);
				parentForm.Closing += new CancelEventHandler(OnParentFormClosing);
			}
			else {
				if(Parent != null && Parent is UserControl)
					Parent.ParentChanged += new EventHandler(Parent_ParentChanged);
			}
		}
		void Parent_ParentChanged(object sender, EventArgs e) {
			UpdateIsMdiChildRibbon();
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			UnsubscribeParentEvents();
			parentForm = FindForm();
			if(parentForm != null)
				parentFormBorderStyle = parentForm.FormBorderStyle;
			SubscribeParentEvents();
			UpdateIsMdiChildRibbon();
			OnMdiChildActivate(this, EventArgs.Empty);
		}
		void OnParentFormStyleChanged(object sender, EventArgs e) {
			if(parentForm != null) {
				if(parentFormBorderStyle == parentForm.FormBorderStyle)
					return;
				if(parentFormBorderStyle != FormBorderStyle.None && parentForm.FormBorderStyle != FormBorderStyle.None) {
					parentFormBorderStyle = parentForm.FormBorderStyle;
					return;
				}
				parentFormBorderStyle = parentForm.FormBorderStyle;
			}
			OnIsMdiChildRibbonChanged();
		}
		bool isMdiChildRibbon = false;
		protected internal bool IsMdiChildRibbon { 
			get { return isMdiChildRibbon; } 
			set {
				if(IsMdiChildRibbon == value)
					return;
				isMdiChildRibbon = value;
				OnIsMdiChildRibbonChanged();
			} 
		}
		void OnParentFormClosing(object sender, CancelEventArgs e) {
			AutoSaveLayoutToXmlCore();
		}
		internal RibbonControl GetMdiParentRibbon() {
			if(MergeOwner != null)
				return MergeOwner;
			if(parentForm == null) {
				if(manager != null) {
					var document = manager.GetDocument();
					if(document != null && document.Manager != null) {
						Form form = Docking2010.Views.DocumentsHostContext.GetParentForm(document.Manager);
						Control parentControl = document.Manager.GetContainer();
						return GetRibbonControl(form) ?? GetRibbonControlFromControl(parentControl);
					}
				}
				return null;
			}
			else {
				if(parentForm.MdiParent == null && parentForm.Owner != null) {
					var manager = Docking2010.DocumentManager.FromControl(parentForm.Owner);
					if(manager != null && manager.CanMergeOnDocumentActivate()) {
						if(manager.GetDocument(parentForm) != null) {
							return GetRibbonControl(parentForm.Owner);
						}
					}
				}
			}
			return GetRibbonControl(parentForm.MdiParent);
		}
		static RibbonControl GetRibbonControl(Form form) {
			return GetRibbonControlFromControl(form);
		}
		static RibbonControl GetRibbonControlFromControl(Control parentControl) {
			if(parentControl == null)
				return null;
			foreach(Control ctrl in parentControl.Controls) {
				if(ctrl is RibbonControl) return (RibbonControl)ctrl;
			}
			return null;
		}
		internal RibbonMdiMergeStyle GetMdiMergeStyle() {
			if(MdiMergeStyle == RibbonMdiMergeStyle.Default)
				return RibbonMdiMergeStyle.OnlyWhenMaximized;
			return MdiMergeStyle;
		}
		bool patchVisible = false;
		bool userVisible = true;
		bool shouldPatchVisibleOnHandleCreated = false;
		protected virtual void OnIsMdiChildRibbonChanged() {
			ResetApplicationButtonContentControlCache();
			this.patchVisible = true;
			try {
				if(parentForm == null) {
					if(manager == null || manager.GetDocument() == null)
						return;
				}
				RibbonControl mdiParentRibbon = GetMdiParentRibbon();
				if(mdiParentRibbon == null && IsLoading) return;
				if(parentForm != null) {
					if((mdiParentRibbon == null) ||
						(mdiParentRibbon.GetMdiMergeStyle() != RibbonMdiMergeStyle.Always && !(mdiParentRibbon.GetMdiMergeStyle() == RibbonMdiMergeStyle.OnlyWhenMaximized && parentForm.WindowState == FormWindowState.Maximized))) {
						Visible = true;
						return;
					}
				}
				if(parentForm is RibbonForm) {
					if(parentForm.FormBorderStyle == FormBorderStyle.None)
						Visible = !IsMdiChildRibbon;
					else
						Visible = true;
				}
				else {
					if(IsHandleCreated)
						Visible = !IsMdiChildRibbon;
					else {
						this.shouldPatchVisibleOnHandleCreated = true;
					}
				}
			}
			finally {
				this.patchVisible = false;
			}
		}
		protected virtual void OnFormParentChanged(object sender, EventArgs e) {
			UpdateIsMdiChildRibbon();
		}
		internal void UpdateIsMdiChildRibbon() {
			IsMdiChildRibbon = (parentForm != null && parentForm.IsMdiChild) || ((manager != null) && manager.GetIsMdiChildManager());
		}
		protected virtual bool ShouldMergeActivate(Form mdiChild) {
			return MdiMergeStyle == RibbonMdiMergeStyle.Always;
		}
		protected virtual bool ShouldMergeMaximized(Form mdiChild) {
			if(mdiChild == null) return false;
			return (GetMdiMergeStyle() == RibbonMdiMergeStyle.OnlyWhenMaximized) && mdiChild.WindowState == FormWindowState.Maximized;
		}
		protected virtual void OnMdiChildActivate(object sender, EventArgs e) {
			if(parentForm == null) return;
			var documentManager = Docking2010.DocumentManager.FromControl(parentForm);
			if(documentManager != null && documentManager.CanMergeOnDocumentActivate()) {
				if(GetMdiMergeStyle() == RibbonMdiMergeStyle.Always)
					return;
			}
			using(var mergingContext = CreateRibbonMergingContext()) {
				if(MdiMergeStyle != RibbonMdiMergeStyle.Never)
					UnMergeRibbon();
				if(mdiChildForm != null) {
					mdiChildForm.StyleChanged -= new EventHandler(OnMdiChildStyleChanged);
					mdiChildForm.LocationChanged -= new EventHandler(OnMdiLocationChanged);
				}
				MDIMinimizeItem.ChildForm = parentForm.ActiveMdiChild;
				MDIRestoreItem.ChildForm = parentForm.ActiveMdiChild;
				MDICloseItem.ChildForm = parentForm.ActiveMdiChild;
				mdiChildForm = parentForm.ActiveMdiChild;
				if(mdiChildForm != null) {
					mdiChildForm.StyleChanged += new EventHandler(OnMdiChildStyleChanged);
					mdiChildForm.LocationChanged += new EventHandler(OnMdiLocationChanged);
				}
				OnMdiChildStyleChanged(this, EventArgs.Empty);
				if(ShouldMergeActivate(mdiChildForm))
					MergeRibbon(FindMDIRibbon(mdiChildForm));
			}
		}
		protected virtual void OnMdiLocationChanged(object sender, EventArgs e) {
			OnMdiChildStyleChanged(sender, e);
		}
		protected internal virtual void OnMdiChildStyleChanged(object sender, EventArgs e) {
			if(ShouldMergeMaximized(mdiChildForm)) {
				MergeRibbon(FindMDIRibbon(mdiChildForm));
			}
			else if(!ShouldMergeActivate(mdiChildForm) && MdiMergeStyle != RibbonMdiMergeStyle.Never)
				UnMergeRibbon();
			Refresh();
		}
		protected virtual RibbonControl FindMDIRibbon(Form mdiChild) {
			if(mdiChild == null) return null;
			var floatDocumentForm = mdiChild as Docking2010.FloatDocumentForm;
			if(floatDocumentForm != null && floatDocumentForm.Controls.Count == 1) {
				if(floatDocumentForm.Manager != null && floatDocumentForm.Manager.CanMergeOnDocumentActivate())
					return FindRibbon(floatDocumentForm.Controls[0]);
			}
			return FindRibbon(mdiChild);
		}
		RibbonControl FindRibbon(Control container) {
			foreach(Control ctrl in container.Controls) {
				if(ctrl is RibbonControl) return ctrl as RibbonControl;
			}
			return null;
		}
		internal BarMdiButtonItemLink MDIMinimizeItemLink {
			get {
				if(mdiMinimizeItemLink == null) mdiMinimizeItemLink = MDIMinimizeItem.CreateLink(null, MDIMinimizeItem) as BarMdiButtonItemLink;
				mdiMinimizeItemLink.Caption = string.Empty;
				return mdiMinimizeItemLink;
			}
		}
		internal BarMdiButtonItemLink MDIRestoreItemLink {
			get {
				if(mdiRestoreItemLink == null) mdiRestoreItemLink = MDIRestoreItem.CreateLink(null, MDIRestoreItem) as BarMdiButtonItemLink;
				mdiRestoreItemLink.Caption = string.Empty;
				return mdiRestoreItemLink;
			}
		}
		internal BarMdiButtonItemLink MDICloseItemLink {
			get {
				if(mdiCloseItemLink == null) mdiCloseItemLink = MDICloseItem.CreateLink(null, MDICloseItem) as BarMdiButtonItemLink;
				mdiCloseItemLink.Caption = string.Empty;
				return mdiCloseItemLink;
			}
		}
		internal BarButtonItemLink ExpandCollapseItemLink {
			get {
				if(expandCollapseItemLink == null) {
					expandCollapseItemLink = ExpandCollapseItem.CreateLink(null, ExpandCollapseItem) as BarButtonItemLink;
					expandCollapseItem.Links.Add(expandCollapseItemLink);
				}
				expandCollapseItemLink.Caption = string.Empty;
				return expandCollapseItemLink;
			}
		}
		internal BarButtonItemLink AutoHiddenPagesMenuItemLink {
			get {
				if(autoHiddenPagesMenuItemLink == null) {
					autoHiddenPagesMenuItemLink = AutoHiddenPagesMenuItem.CreateLink(null, AutoHiddenPagesMenuItem) as BarButtonItemLink;
					autoHiddenPagesMenuItem.Links.Add(autoHiddenPagesMenuItemLink);
				}
				autoHiddenPagesMenuItemLink.Caption = string.Empty;
				return autoHiddenPagesMenuItemLink;
			}
		}
		protected internal BarMdiButtonItem MDIMinimizeItem {
			get {
				if(mdiMinimizeItem == null) mdiMinimizeItem = new BarMdiButtonItem(Manager, BarMdiButtonItem.SystemItemType.Minimize);
				mdiMinimizeItem.RibbonStyle = RibbonItemStyles.SmallWithoutText;
				return mdiMinimizeItem;
			}
		}
		protected internal BarMdiButtonItem MDIRestoreItem {
			get {
				if(mdiRestoreItem == null) mdiRestoreItem = new BarMdiButtonItem(Manager, BarMdiButtonItem.SystemItemType.Restore);
				mdiRestoreItem.RibbonStyle = RibbonItemStyles.SmallWithoutText;
				return mdiRestoreItem;
			}
		}
		protected internal BarMdiButtonItem MDICloseItem {
			get {
				if(mdiCloseItem == null) mdiCloseItem = new BarMdiButtonItem(Manager, BarMdiButtonItem.SystemItemType.Close);
				mdiCloseItem.RibbonStyle = RibbonItemStyles.SmallWithoutText;
				return mdiCloseItem;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarButtonItem ExpandCollapseItem {
			get {
				if(expandCollapseItem == null) {
					expandCollapseItem = new RibbonExpandCollapseItem(this);
					expandCollapseItem.Id = Manager.MaxItemId;
					expandCollapseItem.ShowInCustomizationForm = false;
					Manager.MaxItemId++;
				}
				expandCollapseItem.RibbonStyle = RibbonItemStyles.SmallWithoutText;
				return expandCollapseItem;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarButtonItem AutoHiddenPagesMenuItem {
			get {
				if(autoHiddenPagesMenuItem == null) {
					autoHiddenPagesMenuItem = new AutoHiddenPagesMenuItem(this);
					autoHiddenPagesMenuItem.Id = -1000;
					autoHiddenPagesMenuItem.ShowInCustomizationForm = false;
				}
				autoHiddenPagesMenuItem.RibbonStyle = RibbonItemStyles.SmallWithoutText;
				return autoHiddenPagesMenuItem;
			}
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlItemAnimationLength"),
#endif
 DefaultValue(-1), XtraSerializableProperty]
		public int ItemAnimationLength {
			get { return itemAnimationLength; }
			set { itemAnimationLength = value; }
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlGroupAnimationLength"),
#endif
 DefaultValue(-1), XtraSerializableProperty]
		public int GroupAnimationLength {
			get { return groupAnimationLength; }
			set { groupAnimationLength = value; }
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlPageAnimationLength"),
#endif
 DefaultValue(-1), XtraSerializableProperty]
		public int PageAnimationLength {
			get { return pageAnimationLength; }
			set { pageAnimationLength = value; }
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationButtonAnimationLength"),
#endif
 DefaultValue(-1), XtraSerializableProperty]
		public int ApplicationButtonAnimationLength {
			get { return applicationButtonAnimationLength; }
			set { applicationButtonAnimationLength = value; }
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlGalleryAnimationLength"),
#endif
 DefaultValue(-1), XtraSerializableProperty]
		public int GalleryAnimationLength {
			get { return galleryAnimationLength; }
			set { galleryAnimationLength = value; }
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlTransparentEditors"),
#endif
 DefaultValue(false), XtraSerializableProperty]
		public bool TransparentEditors {
			get { return transparentEditors; }
			set {
				if(TransparentEditors == value) return;
				transparentEditors = value;
				Refresh();
				if(StatusBar != null) StatusBar.Refresh();
			}
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlShowCategoryInCaption"),
#endif
 DefaultValue(true), XtraSerializableProperty]
		public bool ShowCategoryInCaption {
			get { return showCategoryInCaption; }
			set {
				if(ShowCategoryInCaption == value) return;
				showCategoryInCaption = value;
				Refresh();
			}
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlAutoSizeItems"),
#endif
 DefaultValue(false), XtraSerializableProperty]
		public bool AutoSizeItems {
			get { return autoSizeItems; }
			set {
				if(AutoSizeItems == value) return;
				autoSizeItems = value;
				Refresh();
			}
		}
		protected internal virtual bool AllowChangeToolbarLocationMenuItem { 
			get { 
				return ShowToolbarCustomizeItem && ToolbarLocation != RibbonQuickAccessToolbarLocation.Hidden && 
				ShowQatLocationSelector && ViewInfo.Toolbar.SupportForRibbonStyle(GetRibbonStyle()) && GetRibbonStyle() != RibbonControlStyle.TabletOffice; 
			} 
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlShowToolbarCustomizeItem"),
#endif
 DefaultValue(true), XtraSerializableProperty]
		public bool ShowToolbarCustomizeItem {
			get { return Toolbar.ShowCustomizeItem; }
			set { Toolbar.ShowCustomizeItem = value; }
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlController"),
#endif
 DefaultValue(null)]
		public virtual BarAndDockingController Controller {
			get { return Manager.Controller; }
			set { Manager.Controller = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationButtonSuperTip"),
#endif
 Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), Category("Appearance"), Localizable(true), SkipRuntimeSerialization]
		public virtual SuperToolTip ApplicationButtonSuperTip {
			get { return applicationButtonSuperTip; }
			set { applicationButtonSuperTip = value; }
		}
		protected virtual bool ShouldSerializeApplicationButtonSuperTip() { return ApplicationButtonSuperTip != null && !ApplicationButtonSuperTip.IsEmpty; }
		public virtual void ResetApplicationButtonSuperTip() { ApplicationButtonSuperTip = null; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationCaption"),
#endif
 DefaultValue(""), Category("Appearance"), Localizable(true), XtraSerializableProperty]
		public string ApplicationCaption {
			get { return applicationCaption; }
			set {
				if(value == null) value = string.Empty;
				if(ApplicationCaption == value) return;
				applicationCaption = value;
				OnApplicationCaptionChanged();
				Refresh();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationIcon"),
#endif
 DefaultValue(null), Category("Appearance"), Localizable(true), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Bitmap ApplicationIcon {
			get { return applicationIcon; }
			set {
				if(ApplicationIcon == value) return;
				applicationIcon = value;
				Refresh();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationButtonText"),
#endif
 DefaultValue(""), Category("Appearance"), Localizable(true), XtraSerializableProperty]
		public string ApplicationButtonText {
			get { return applicationButtonText; }
			set {
				if(ApplicationButtonText == value) return;
				applicationButtonText = value;
				Refresh();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationDocumentCaption"),
#endif
 DefaultValue(""), Category("Appearance"), Localizable(true), XtraSerializableProperty]
		public string ApplicationDocumentCaption {
			get { return applicationDocumentCaption; }
			set {
				if(value == null) value = string.Empty;
				if(ApplicationDocumentCaption == value) return;
				applicationDocumentCaption = value;
				OnApplicationCaptionChanged();
				Refresh();
			}
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlRibbonStyle"),
#endif
 DefaultValue(RibbonControlStyle.Default), XtraSerializableProperty]
		public RibbonControlStyle RibbonStyle {
			get { return ribbonStyle; }
			set {
				if(RibbonStyle == value) return;
				RibbonControlStyle prev = RibbonStyle;
#pragma warning disable 612, 618 // obsolete TabbletOfficeEx
				ribbonStyle = value == RibbonControlStyle.TabletOfficeEx ? RibbonControlStyle.OfficeUniversal : value;
#pragma warning restore 612, 618 // obsolete TabbletOfficeEx
				OnRibbonStyleChanged(prev);
			}
		}
		bool allowCustomization = false;
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlAllowCustomization"),
#endif
 DefaultValue(false), XtraSerializableProperty]
		public bool AllowCustomization {
			get { return allowCustomization; }
			set {
				if(AllowCustomization == value)
					return;
				allowCustomization = value;
				OnAllowCustomizationChanged();
			}
		}
		protected virtual void OnAllowCustomizationChanged() {
			DestroyCustomizationPopupMenu();
		}
		protected internal virtual bool ShowCustomizationOption {
			get { return AllowCustomization && !IsRibbonMerged; }	
		}
		internal bool IsRibbonMerged { get { return MergedRibbon != null; } }
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlShowQatLocationSelector"),
#endif
 DefaultValue(true), XtraSerializableProperty]
		public bool ShowQatLocationSelector {
			get;
			set;
		}
		bool autoSaveLayoutToXmlCore = false;
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlAutoSaveLayoutToXml"),
#endif
 DefaultValue(false), XtraSerializableProperty]
		public bool AutoSaveLayoutToXml {
			get { return this.autoSaveLayoutToXmlCore; }
			set {
				if(this.autoSaveLayoutToXmlCore == value) return;
				this.autoSaveLayoutToXmlCore = value;
			}
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlOptionsCustomizationForm"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public RibbonCustomizationFormOptions OptionsCustomizationForm {
			get;
			set;
		}
		bool ShouldSerializeOptionsTouch() { return OptionsTouch.ShouldSerializeCore(this); }
		void ResetOptionsTouch() { OptionsTouch.Reset(); }
		RibbonOptionsTouch optionsTouch;
		[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public RibbonOptionsTouch OptionsTouch {
			get { 
				if(optionsTouch == null)
					optionsTouch = CreateOptionsTouch();
				return optionsTouch;
			}
		}
		protected virtual RibbonOptionsTouch CreateOptionsTouch() {
			return new RibbonOptionsTouch(this);
		}
		const string defaultRibbonSettingsFileName = "RibbonSettings.xml";
		string autoSaveLayoutToXmlPathCore = defaultRibbonSettingsFileName;
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlAutoSaveLayoutToXmlPath"),
#endif
 DefaultValue(defaultRibbonSettingsFileName), XtraSerializableProperty]
		public string AutoSaveLayoutToXmlPath {
			get { return this.autoSaveLayoutToXmlPathCore; }
			set {
				if(this.autoSaveLayoutToXmlPathCore == value) return;
				this.autoSaveLayoutToXmlPathCore = value;
			}
		}
		public virtual void SaveLayoutToXml(string xmlFile) {
			RibbonSaveLoadLayoutHelper.SaveLayout(this, xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			RibbonSaveLoadLayoutHelper.LoadLayout(this, xmlFile);
		}
		protected internal virtual void AutoSaveLayoutToXmlCore() {
			if(!ShouldProcessAutoSaveRestoreLayoutCommand)
				return;
			SaveLayoutToXml(AutoSaveRestoreLayoutXmlFilePathCore);
		}
		protected internal virtual void AutoRestoreLayoutFromXmlCore() {
			if(!ShouldProcessAutoSaveRestoreLayoutCommand)
				return;
			if(!File.Exists(AutoSaveRestoreLayoutXmlFilePathCore))
				return;
			RestoreLayoutFromXml(AutoSaveRestoreLayoutXmlFilePathCore);
		}
		protected internal virtual bool ShouldProcessAutoSaveRestoreLayoutCommand {
			get {
				if(DesignMode)
					return false;
				return AutoSaveLayoutToXml;
			}
		}
		protected internal virtual string AutoSaveRestoreLayoutXmlFilePathCore {
			get { return RibbonSaveLoadLayoutHelper.GetAutoSaveRestoreLayoutXmlFilePath(this); }
		}
		protected internal virtual RibbonCustomizationFormOptions CreateRibbonCustomizationFormOptions() {
			return new RibbonCustomizationFormOptions(this);
		}
		protected virtual void OnRibbonStyleChanged(RibbonControlStyle prev) {
			if(RibbonStyle != RibbonControlStyle.Office2013 && ViewInfo != null && ViewInfo.IsPopupFullScreenModeActive) {
				CloseFullScreenModeRibbonPopup();
				OnFullScreenModeChangeCore();
				ClosePopupForms();
			}
			ApplicationButtonContentControl.HideContent();
			ResetApplicationButtonContentControlCache();
			if(prev == RibbonControlStyle.MacOffice || RibbonStyle == RibbonControlStyle.MacOffice || 
				prev == RibbonControlStyle.TabletOffice || RibbonStyle == RibbonControlStyle.TabletOffice || 
				prev == RibbonControlStyle.OfficeUniversal || RibbonStyle == RibbonControlStyle.OfficeUniversal) {
				ViewInfo.AllowCachedItemInfo = false;
				if(ViewInfo.Form != null) {
					RibbonForm.CheckUpdateSkinPainterCore();
					ViewInfo.Form.ForceStyleChanged();
				}
			}
			if (ViewInfo != null) {
				ViewInfo.SetAppearanceDirty();
			}
			if(RibbonForm != null) {
				RibbonForm.CheckUpdateSkinPainterCore();
				RibbonForm.UpdateFormMargins();
			}
			OnControllerChanged();
			RefreshHeight();
			RaiseRibbonStyleChanged(new RibbonStyleChangedEventArgs(prev, RibbonStyle));
			CheckRibbonStatusBar();
		}
		protected internal virtual void OnSkinPainterChanged() {
			CheckHeight();
			Refresh();
			CheckRibbonStatusBar();
		}
		protected internal void CheckRibbonStatusBar() {
			if(ApplicationButtonContentControl != null) ApplicationButtonContentControl.CheckRibbonStatusBar();
		}
		protected internal bool AllowGlassTabHeader {
			get {
				object prop = RibbonSkins.GetSkin(ViewInfo.Provider).Properties[RibbonSkins.SkinSupportGlassTabHeader];
				return prop != null && ((bool)prop) == true;
			}
		}
		protected internal RibbonControlStyle GetRibbonStyle() {
			return RibbonStyle == RibbonControlStyle.Default ? RibbonControlStyle.Office2007 : RibbonStyle; 
		}
		protected internal virtual bool IsOfficeTablet {
			get { 
				return RibbonStyle == RibbonControlStyle.TabletOffice || 
					RibbonStyle == RibbonControlStyle.OfficeUniversal; 
			}
		}
		protected internal bool IsOffice2010LikeStyle {
			get {
				RibbonControlStyle style = GetRibbonStyle();
				return style == RibbonControlStyle.Office2010 || style == RibbonControlStyle.Office2013;
			}
		}
		protected internal virtual bool IsExpandButtonInPanel {
			get {
				RibbonControlStyle style = GetRibbonStyle();
				return style == RibbonControlStyle.Office2013 && GetShowExpandCollapseButton();
			}
		}
		protected internal bool IsExpandCollapseItemInPageHeaderItemLinks {
			get {
				foreach(BarItemLink link in PageHeaderItemLinks) {
					if(link.Item == ExpandCollapseItem)
						return true;
				}
				return false;
			}
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			ViewInfo.SetAppearanceDirty();
			if(ApplicationButtonDropDownControl is BackstageViewControl) ((BackstageViewControl)ApplicationButtonDropDownControl).CheckRightToLeft();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationButtonAccessibleName"),
#endif
Category("Accessibility"), DefaultValue(""), XtraSerializableProperty]
		public string ApplicationButtonAccessibleName { get { return applicationButtonAccessibleName; } set { applicationButtonAccessibleName = value; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationButtonAccessibleDescription"),
#endif
Category("Accessibility"), DefaultValue(""), XtraSerializableProperty]
		public string ApplicationButtonAccessibleDescription { get { return applicationButtonAccessibleDescription; } set { applicationButtonAccessibleDescription = value; } }
		protected virtual bool ShouldInitOrDestroyBarManager { get { return true; } }
		protected internal bool IsDestroying { get { return destroying; } }
		protected virtual RibbonBarManager CreateBarManager() { return new RibbonBarManager(this); }
		public virtual BarAndDockingController GetController() {
			return Manager.GetController();
		}
		protected internal virtual void ShowMinimizedRibbon(bool activateKeyboardNavigation) {
			ShowMinimizedRibbon(activateKeyboardNavigation, new DefaultRibbonPopupFactory(this));
		}
		protected internal virtual void ShowMinimizedRibbon(bool activateKeyboardNavigation, RibbonPopupFactory factory) {
			XtraForm.SuppressDeactivation = true;
			try {
				if(MinimizedRibbonPopupForm != null) {
					MinimizedRibbonPopupForm.FocusForm();
					if(MinimizedRibbonPopupForm != null) {
						MinimizedRibbonPopupForm.UpdateRibbon();
						return;
					}
				}
				MinimizedRibbonPopupForm form = factory.CreatePopup();
				form.UpdateRibbon();
				MinimizedRibbonPopupForm = form;
				RaiseMinimizedRibbonShowing(new MinimizedRibbonEventArgs() { MinimizedRibbon = form.Control});
				form.ShowPopup();
				if(IsKeyboardActive || activateKeyboardNavigation)
					form.Control.ActivateKeyboardNavigation();
			}
			finally {
				XtraForm.SuppressDeactivation = false;
			}
		}
		protected internal virtual void ShowMinimizedRibbon() {
			ShowMinimizedRibbon(false);
		}
		#region Ribbon merging
		[Browsable(false)]
		public RibbonControl MergedRibbon { get { return mergedRibbon; } }
		protected internal virtual IRibbonMergingContext GetRibbonMergingContext() {
			return RibbonMergingContext.Get(this);
		}
		protected internal virtual IRibbonMergingContext CreateRibbonMergingContext() {
			return RibbonMergingContext.Create(this);
		}
		class RibbonMergingContext : IRibbonMergingContext {
			#region static
			[ThreadStatic]
			static System.Collections.Generic.IDictionary<RibbonControl, IRibbonMergingContext> contexts;
			static System.Collections.Generic.IDictionary<RibbonControl, IRibbonMergingContext> Contexts {
				get {
					if(contexts == null) 
						contexts = new System.Collections.Generic.Dictionary<RibbonControl, IRibbonMergingContext>();
					return contexts;
				}
			}
			internal static IRibbonMergingContext Get(RibbonControl ribbon) {
				IRibbonMergingContext context;
				return Contexts.TryGetValue(ribbon, out context) ? context : null;
			}
			internal static IRibbonMergingContext Create(RibbonControl ribbon) {
				IRibbonMergingContext context;
				if(!Contexts.TryGetValue(ribbon, out context)) {
					context = new RibbonMergingContext(ribbon);
				}
				else ((RibbonMergingContext)context).refCounter++;
				return context;
			}
			#endregion static
			int refCounter;
			RibbonControl prevMergedRibbon;
			RibbonControl ribbon;
			string selectedPageText;
			string selectedPageCategoryText;
			RibbonMergingContext(RibbonControl ribbon) {
				this.ribbon = ribbon;
				if(Ribbon != null)
					this.prevMergedRibbon = Ribbon.MergedRibbon;
				if(Ribbon != null && Ribbon.SelectedPage != null) {
					selectedPageText = Ribbon.SelectedPage.Text;
					if(Ribbon.SelectedPage.Category != null)
						selectedPageCategoryText = Ribbon.SelectedPage.Category.Text;
				}
				if(0 == refCounter++)
					Contexts.Add(ribbon, this);
			}
			public RibbonControl Ribbon {
				get { return ribbon; }
			}
			void IDisposable.Dispose() {
				if(--refCounter == 0) {
					Contexts.Remove(ribbon);
					if(Ribbon != null && Ribbon.MergedRibbon != prevMergedRibbon)
						CheckSelectedPage(Ribbon);
					this.ribbon = null;
					this.prevMergedRibbon = null;
				}
			}
			public void CheckSelectedPage(RibbonControl ribbon) {
				if(ribbon == null) return;
				if(!string.IsNullOrEmpty(selectedPageText)) {
					RibbonPage page = ribbon.TotalPageCategory.GetPageByText(selectedPageText, selectedPageCategoryText);
					if(page != null && page.Visible) {
						using(LockSelectedPageChanging()) {
							ribbon.SelectedPage = page;
						}
					}
				}
			}
			int selectedPageChanging = 0;
			public IDisposable LockSelectedPageChanging() {
				return new Locker(this);
			}
			public void SelectedPageChanged(RibbonPage page) {
				if(selectedPageChanging > 0) return;
				selectedPageText = (page != null) ? page.Text : null;
				selectedPageCategoryText = (page != null && page.Category != null) ? page.Category.Text : null;
			}
			class Locker : IDisposable {
				RibbonMergingContext context;
				public Locker(RibbonMergingContext context) {
					this.context = context;
					this.context.selectedPageChanging++;
				}
				void IDisposable.Dispose() {
					if(context != null)
						context.selectedPageChanging--;
					context = null;
				}
			}
		}
		bool mergedVisible = false;
		public virtual void MergeRibbon(RibbonControl ribbon) {
			if(MergedRibbon == ribbon) return;
			using(var mergingContext = CreateRibbonMergingContext()) {
				RibbonControl prevMergedRibbon = MergedRibbon;
				UnMergeRibbon();
				mergingContext.CheckSelectedPage(prevMergedRibbon);
				this.mergedRibbon = ribbon;
				if(MergedRibbon == null) return;
				DestroyCustomizationPopupMenu();
				MergePageCategories();
				MergeToolbarItemLinks();
				MergePageHeaderItemLinks();
				RaiseMerge(new RibbonMergeEventArgs(MergedRibbon, this));
				MergedRibbon.RaiseMerge(new RibbonMergeEventArgs(MergedRibbon, this));
				this.mergedVisible = MergedRibbon.userVisible;
				MergedRibbon.MergeOwner = this;
				MergedRibbon.AutoHideEmptyItems = this.AutoHideEmptyItems;
				try {
					this.patchVisible = true;
					if(MergedRibbon.ViewInfo.Form == null || MdiMergeStyle != RibbonMdiMergeStyle.Always || MergedRibbon.ViewInfo.Form.FormBorderStyle == FormBorderStyle.None)
						MergedRibbon.Visible = false;
				}
				finally {
					this.patchVisible = false;
				}
				UpdateSelectedPage();
				UpdateViewInfo();
				UpdateMinimizedRibbonPopupForm();
				Invalidate();
			}
		}
		protected virtual void UpdateSelectedPage() {
			if(SelectedPage == null)
				SelectedPage = TotalPageCategory.GetVisiblePages().Count > 0 ? (RibbonPage)TotalPageCategory.GetVisiblePages()[0] : null;
		}
		protected virtual void UpdateMinimizedRibbonPopupForm() {
			if(!IsMinimizedRibbonOpened)
				return;
			MinimizedRibbonPopupForm.UpdateRibbon();
			MinimizedRibbonPopupForm.FocusForm();
		}
		protected virtual void MergeToolbarItemLinks() {
			Toolbar.ItemLinks.Merge(MergedRibbon.Toolbar.ItemLinks);
		}
		protected virtual void MergePageHeaderItemLinks() {
			PageHeaderItemLinks.Merge(MergedRibbon.PageHeaderItemLinks);
		}
		protected virtual void UnMergeToolbarItemLinks() {
			Toolbar.ItemLinks.UnMerge();
		}
		protected virtual void UnMergePageHeaderItemLinks() {
			PageHeaderItemLinks.UnMerge();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlMergedCategories"),
#endif
 Browsable(false)]
		public virtual RibbonPageCategoryCollection MergedCategories {
			get {
				if(mergedCategories == null) mergedCategories = new RibbonPageCategoryCollection(this);
				return mergedCategories;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlMergedPages"),
#endif
 Browsable(false)]
		public virtual RibbonPageCollection MergedPages { get { return DefaultPageCategory.MergedPages; } }
		RibbonPageCategory FindVisibleCategory(string text) {
			foreach(RibbonPageCategory cat in PageCategories) {
				if(cat.Text == text && cat.Visible)
					return cat;
			}
			return null;
		}
		protected virtual void MergePageCategory(RibbonPageCategory mergedCategory) {
			RibbonPageCategory category = RibbonControl.AllowMergeInvisibleItems? PageCategories[mergedCategory.Text] : FindVisibleCategory(mergedCategory.Text);
			if(category == null) {
				MergedCategories.Add((RibbonPageCategory)mergedCategory.Clone(true));
			}
			else category.MergeCategory(mergedCategory);
		}
		protected virtual void MergePageCategoryCollection(RibbonPageCategoryCollection coll) {
			if(coll == null) return;
			foreach(RibbonPageCategory mergedCategory in coll) {
				MergePageCategory(mergedCategory);
			}
		}
		protected virtual void MergePageCategories() { 
			if(MergedRibbon == null) return;
			MergedCategories.Clear();
			DefaultPageCategory.MergeCategory(MergedRibbon.DefaultPageCategory);
			MergePageCategoryCollection(MergedRibbon.PageCategories);
			MergePageCategoryCollection(MergedRibbon.MergedCategories);
		}
		protected virtual void UnMergePageCategories() {
			foreach(RibbonPageCategory category in PageCategories) {
				category.UnMergeCategory();
			}
			DefaultPageCategory.UnMergeCategory();
			foreach(RibbonPageCategory cat in MergedCategories) {
				cat.ClearReference();
			}
			MergedCategories.Clear();
		}
		public virtual void UnMergeRibbon() {
			if(MergedRibbon == null) return;
			RaiseBeforeUnMerge(new RibbonMergeEventArgs(MergedRibbon));
			using(var mergingContext = CreateRibbonMergingContext()) {
				Manager.SelectionInfo.Clear();
				if(!BarManager.IsFormHierarchyDisposing(MergedRibbon))
					MergedRibbon.Visible = this.mergedVisible;
				MergedRibbon.AutoHideEmptyItems = this.autoHideEmptyItems;
				RibbonControl merged = MergedRibbon;
				MergedRibbon.MergeOwner = null;
				this.mergedRibbon = null;
				DestroyCustomizationPopupMenu();
				UnMergePageCategories();
				UnMergeToolbarItemLinks();
				UnMergePageHeaderItemLinks();
				UpdateViewInfo();
				RaiseUnMerge(new RibbonMergeEventArgs(merged, this));
				merged.RaiseUnMerge(new RibbonMergeEventArgs(merged, this));
				mergingContext.CheckSelectedPage(merged);
			}
		}
		#endregion
		protected internal void SwitchMinimized() {
			if(!AllowMinimizeRibbon || !ViewInfo.IsAllowDisplayRibbon || IsDesignMode) return;
			Minimized = !Minimized;
		}
		protected virtual void OnApplicationCaptionChanged() {
			if(!IsHandleCreated || RibbonForm == null) return;
			if(ApplicationDocumentCaption == string.Empty && ApplicationCaption == string.Empty) return;
			string text = string.Empty;
			if(ApplicationCaption == string.Empty || ApplicationCaption == " ") text = ApplicationDocumentCaption;
			if(ApplicationDocumentCaption == string.Empty) text = ApplicationCaption;
			if(text == string.Empty) text = string.Format("{0} - {1}", ApplicationDocumentCaption, ApplicationCaption);
			try {
				lockUpdateFormOriginalText = true;
				RibbonForm.Text = text;
			}
			finally {
				lockUpdateFormOriginalText = false;
			}			
		}
		bool IEditorBackgroundProvider.DrawBackground(Control ctrl, GraphicsCache cache) {
			return new RibbonPainter().DrawControlBackground(ctrl, cache, ViewInfo);
		}
		protected override void OnLayout(LayoutEventArgs levent) {
			DeactivateKeyTips();
			if(!TotalPageCategory.Pages.Contains(SelectedPage)) {
				SelectedPage = TotalPageCategory.GetFirstVisiblePage();
			}
			base.OnLayout(levent);
		}
		public virtual void LayoutChanged() { Refresh(); }
		protected internal void RefreshHeight() {
			needCheckHeight = true;
			Refresh();
			if(!IsDestroying && Height == 0 && IsHandleCreated) {
				BeginInvoke(new MethodInvoker(CheckViewInfo));
			}
		}
		public override void Refresh() {
			if(IsDestroying) return;
			UpdateVisualEffects(UpdateAction.BeginUpdate);
			ViewInfo.IsReady = false;
			Invalidate();
			UpdateVisualEffects(UpdateAction.EndUpdate);
		}
		protected internal bool IsMergeOwner { get { return ViewInfo.IsRibbonMerged; } }
		protected internal bool IsMerged {
			get { return MergeOwner != null; }
		}
		RibbonControl mergedOwner;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonControl MergeOwner {
			get { return mergedOwner; }
			set {
				if(MergeOwner == value)
					return;
				mergedOwner = value;
				RefreshHeight();
			}
		}
		bool allowGlyphSkinning = false;
		[DefaultValue(false), XtraSerializableProperty]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				Refresh();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable( EditorBrowsableState.Never)]
		public override string Text {
			get { return string.Empty; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new int TabIndex {
			get { return base.TabIndex; }
			set { }
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlToolbarLocation"),
#endif
 DefaultValue(RibbonQuickAccessToolbarLocation.Default), XtraSerializableProperty]
		public virtual RibbonQuickAccessToolbarLocation ToolbarLocation {
			get { return toolbarLocation; }
			set {
				if(ToolbarLocation == value) return;
				toolbarLocation = value;
				OnToolbarLocationChanged();
			}
		}
		protected virtual void OnToolbarLocationChanged() {
			DeactivateKeyboardNavigation();
			RefreshHeight();
			RaiseToolbarLocationChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(0)]
		public int MaxItemId {
			get { return Manager.MaxItemId; }
			set { Manager.MaxItemId = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SkipRuntimeSerialization]
		public virtual RepositoryItemCollection RepositoryItems { get { return Manager.RepositoryItems; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlExternalRepository"),
#endif
 Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null)]
		public PersistentRepository ExternalRepository {
			get { return Manager.ExternalRepository; }
			set { Manager.ExternalRepository = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public RibbonQuickAccessToolbar Toolbar { get { return toolbar; } }
		protected internal virtual RibbonQuickAccessToolbar SourceToolbar { get { return Toolbar; } }
		protected internal bool IsPopupFormOpened { get { return IsPopupGroupOpened || IsPopupToolbarOpened; } }
		protected internal bool IsPopupToolbarOpened { get { return PopupToolbar != null && PopupToolbar.Visible; } }
		protected internal bool IsPopupGroupOpened { get { return PopupGroupForm != null && PopupGroupForm.Visible; } }
		protected internal bool IsMinimizedRibbonOpened { get { return MinimizedRibbonPopupForm != null && MinimizedRibbonPopupForm.Visible; } }
		protected internal virtual RibbonMinimizedGroupPopupForm PopupGroupForm {
			get { return popupGroupForm; }
			set {
				RibbonBasePopupForm prev = PopupGroupForm;
				popupGroupForm = null;
				DestroyPopupForm(prev);
				popupGroupForm = value;
			}
		}
		protected internal virtual MinimizedRibbonPopupForm MinimizedRibbonPopupForm {
			get { return minimizedRibbonPopupForm; }
			set {
				if(MinimizedRibbonPopupForm == value) return;
				RibbonBasePopupForm prev = MinimizedRibbonPopupForm;
				minimizedRibbonPopupForm = null;
				DestroyPopupForm(prev);
				if(prev != null)
					RaiseMinimizedRibbonHiding(new MinimizedRibbonEventArgs() { MinimizedRibbon = prev.Control });
				minimizedRibbonPopupForm = value;
				ViewInfo.Invalidate(ViewInfo.Header.Bounds);
			}
		}
		protected internal virtual RibbonQuickToolbarPopupForm PopupToolbar {
			get { return popupToolbar; }
			set {
				RibbonBasePopupForm prev = PopupToolbar;
				popupToolbar = null;
				DestroyPopupForm(prev);
				popupToolbar = value;
			}
		}
		protected internal void CloseDXRibbonMiniToolbars() {
			for(int i = 0; i < MiniToolbars.Count; ) {
				DXRibbonMiniToolbar tb = MiniToolbars[i].Tag as DXRibbonMiniToolbar;
				if(tb == null) {
					i++;
					continue;
				}
				tb.Dispose();
			}
		}
		protected internal void HideDXRibbonMiniToolbars() {
			for(int i = 0; i < MiniToolbars.Count; i++) {
				DXRibbonMiniToolbar tb = MiniToolbars[i].Tag as DXRibbonMiniToolbar;
				if(tb == null) {
					continue;
				}
				MiniToolbars[i].Hide();
			}
		}
		protected internal virtual void ClosePopupForms(bool closeMinimizedRibbon, bool closeMiniToolbars, bool closeDXToolbars) {
			if(PopupGroupForm != null)
				PopupGroupForm.Hide();
			if(PopupToolbar != null)
				PopupToolbar.Hide();
			if(closeMinimizedRibbon)
				MinimizedRibbonPopupForm = null;
			else {
				if(MinimizedRibbonPopupForm != null && MinimizedRibbonPopupForm.Visible)
					MinimizedRibbonPopupForm.Control.ClosePopupForms(true);
			}
			CloseHoverForms();
			if(closeMiniToolbars)
				HideMiniToolbarForms();
			if(closeDXToolbars)
				CloseDXRibbonMiniToolbars();
			else
				HideDXRibbonMiniToolbars();
		}
		protected internal virtual void ClosePopupForms(bool closeMinimizedRibbon) {
			ClosePopupForms(closeMinimizedRibbon, true, true);
		}
		protected virtual void HideMiniToolbarForms() {
			foreach(RibbonMiniToolbar tb in MiniToolbars) {
				tb.Hide();
			}
		}
		protected internal virtual void DestroyPopupForms() {
			PopupGroupForm = null;
			PopupToolbar = null;
			MinimizedRibbonPopupForm = null;
		}
		protected internal void ClosePopupForms() {
			ClosePopupForms(true);
		}
		protected internal virtual void CloseHoverForms() {
			DevExpress.XtraBars.Ribbon.Gallery.BaseGallery.HideHoverForms();
		}
		protected virtual void DestroyPopupForm(RibbonBasePopupForm form) {
			if(form == null) return;
			if(Manager != null && Manager.SelectionInfo != null) Manager.SelectionInfo.CancelEditor();
			form.OnPrepareDisposing();
			form.Dispose();
			if(Manager != null) Manager.SelectionInfo.CloseAllPopups();
			if(IsKeyboardActive && Manager != null) Manager.SelectionInfo.ActiveBarControl = this;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(true), Browsable(false), MergableProperty(false), SkipRuntimeSerialization]
		public BarManagerCategoryCollection Categories { get { return Manager.Categories; } }
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlLargeImages"),
#endif
 DefaultValue(null), TypeConverter(typeof(ImageCollectionImagesConverter))]
		public object LargeImages {
			get { return Manager.LargeImages; }
			set { Manager.LargeImages = value; }
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlImages"),
#endif
 DefaultValue(null), TypeConverter(typeof(ImageCollectionImagesConverter))]
		public object Images {
			get { return Manager.Images; }
			set { Manager.Images = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual RibbonBarManager Manager { get { return manager; } }
		protected override void  OnResize(EventArgs e) {
			UpdateVisualEffects(UpdateAction.BeginUpdate);
 			base.OnResize(e);
			ViewInfo.IsReady = false;
			Invalidate();
			UpdateVisualEffects(UpdateAction.EndUpdate);
		}
		protected internal virtual string OriginalFormText {
			get;
			private set;
		}
		bool lockUpdateFormOriginalText = false;
		protected internal void OnFormOriginalTextChanged(string newText) {
			if(!lockUpdateFormOriginalText)
				OriginalFormText = newText;
		}
		protected internal void OnFormTextChanged() {
			if(!ViewInfo.Caption.Bounds.IsEmpty) {
				if(ViewInfo.IsReady) {
					ViewInfo.Caption.RecalcTextBounds();
					ViewInfo.Invalidate(ViewInfo.Caption.Bounds);
				}
			}
		}
		object prevFit = null;
		protected internal virtual void CheckViewInfo() {
			if(Manager == null || IsDestroying) return;
			if(!ViewInfo.IsReady || needCheckHeight) {
				ViewInfo.CalcViewInfo(RibbonClientRectangle);
				UpdateApplicationButtonContentControlBounds();
			}
			if(needCheckHeight) {
				this.needCheckHeight = false;
				CheckHeight();
			}
		}
		protected virtual Rectangle RibbonClientRectangle {
			get {
				Rectangle rect = ClientRectangle;
				if(Parent != null && IsRibbonTopControl)
					rect.Width = Parent.DisplayRectangle.Width;
				return rect;
			}
		}
		protected internal bool IsRibbonTopControl {
			get {
				if(RibbonForm == null) return true;
				return RibbonForm.IsRibbonTopControl;
			}
		}
		private void UpdateApplicationButtonContentControlBounds() {
			if(this.applicationButtonContentControl != null && this.applicationButtonContentControl.ContentVisible)
				ApplicationButtonContentControl.UpdateContentBounds();
		}
		protected internal virtual bool IsAllowDisplayRibbon { get { return ViewInfo.IsAllowDisplayRibbon; } }
		internal bool _isKeyboardActive = false;
		protected internal virtual bool IsKeyboardActive { get { return _isKeyboardActive; } }
		protected internal virtual void ActivateKeyboardNavigation() {
			ActivateKeyboardNavigation(false);
		}
		internal void ActivateKeyboardNavigationForEditors() {
			if(IsKeyboardActive || !Visible || !IsAllowDisplayRibbon) return;
			this._isKeyboardActive = true;
			NavigatableObjects = null;
			NavigatableObjectList = null;
		}
		protected internal virtual void ActivateKeyboardNavigation(bool byShortcut) {
			if(IsKeyboardActive || !Visible || !IsAllowDisplayRibbon ) return;
			BackstageViewControl backstage = ApplicationButtonDropDownControl as BackstageViewControl;
			if(backstage != null && backstage.Visible && backstage.Parent.Visible) {
				backstage.Focus();
				backstage.Handler.InitSelectedItem();
				return;
			}
			ViewInfo.KeyboardActiveObject = null;
			NavigatableObjects = null;
			NavigatableObjectList = null;
			if(!byShortcut)
				Manager.SelectionInfo.CloseAllPopups();
			Manager.SelectionInfo.SaveFocusedControl();
			SetStyle(ControlStyles.Selectable, true);
			Focus();
			if(!Focused) {
				DeactivateKeyboardNavigation();
				return;
			}
			this._isKeyboardActive = true;
			KeyboardSelectFirstObject();
			if(ViewInfo.KeyboardActiveObject != null && (!byShortcut || Manager.SelectionInfo.ActiveBarControl == null)) {
				Manager.SelectionInfo.ActiveBarControl = this;
			}
			if(ShouldActivateKeyTipsWithNavigation && !byShortcut) 
				ActivateKeyTips();
		}
		protected virtual bool ShouldActivateKeyTipsWithNavigation { get { return true; } }
		RibbonKeyTipManager keyTipManager = null;
		protected virtual RibbonKeyTipManager CreateKeyTipManager() { return new RibbonKeyTipManager(this); }
		RibbonBaseKeyTipManager activeKeyTipManager = null;
		protected internal RibbonBaseKeyTipManager ActiveKeyTipManager {
			get { return activeKeyTipManager; }
			set { activeKeyTipManager = value; }
		}
		protected internal virtual bool IsShowKeyTip { 
			get {
				if(MinimizedRibbonPopupForm != null) return MinimizedRibbonPopupForm.Control.IsShowKeyTip;
				return ActiveKeyTipManager != null && ActiveKeyTipManager.Show; 
			} 
		}
		[Browsable(false), SkipRuntimeSerialization]
		public virtual RibbonKeyTipManager KeyTipManager { 
			get {
				if(keyTipManager == null) keyTipManager = CreateKeyTipManager();
				return keyTipManager; 
			} 
		}
		protected internal virtual void ActivateKeyTips() {
			if(!AllowKeyTips)
				return;
			KeyTipManager.ActivatePageKeyTips();
			ActiveKeyTipManager = KeyTipManager;
		}
		protected internal virtual void DeactivateKeyTips() {
			if(ActiveKeyTipManager == null) return;
			ActiveKeyTipManager.HideKeyTips();
			ActiveKeyTipManager = null;
		}
		protected internal virtual bool ShouldActivateControl(Control ctrl) {
			if(ctrl == null || !ctrl.Visible || !ctrl.Enabled) return false;
			if(ctrl is Form) return true;
			return ctrl.FindForm() != null;
		}
		protected internal Point GetLastHitPoint() {
			return ((BaseRibbonHandler)Handler).LastHitPoint;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void DeactivateKeyboardNavigation() {
			DeactivateKeyboardNavigation(true, true);
		}
		protected void DeactivateNavigationCore() {
			NavigatableObjects = null;
			NavigatableObjectList = null;
			this._isKeyboardActive = false;
			ViewInfo.KeyboardActiveObject = null;
		}
		protected internal virtual void DeactivateKeyboardNavigation(bool clearSelectionInfo, bool closePopupForms) {
			if(!IsKeyboardActive) return;
			DeactivateNavigationCore();
			SetStyle(ControlStyles.Selectable, false);
			Manager.SelectionInfo.RestoreFocus(false);
			if(IsMinimizedRibbonOpened) ClosePopupForms();
			DeactivateKeyTips();
			if(clearSelectionInfo)
			Manager.SelectionInfo.Clear();
		}
		NavigationObjectRowCollection navigatableObjects;
		protected internal NavigationObjectRowCollection NavigatableObjects {
			get {
				if(navigatableObjects == null) {
					navigatableObjects = Handler.GetNavObjectGrid();
				}
				return navigatableObjects;
			}
			set {
				navigatableObjects = value;
			}
		}
		NavigationObjectRow navigatableObjectList;
		protected internal NavigationObjectRow NavigatableObjectList {
			get {
				if(navigatableObjectList == null)
					navigatableObjectList = Handler.GetNavObjectList();
				return navigatableObjectList;
			}
			set {
				navigatableObjectList = value;
			}
		}
		protected internal void CheckFit() {
			bool checkHeight = false;
			if(prevFit == null)
				checkHeight = true;
			else
				checkHeight |= ((bool)prevFit) != ViewInfo.IsAllowDisplayRibbon;
			prevFit = ViewInfo.IsAllowDisplayRibbon;
			if(checkHeight) {
				needCheckHeight = true;
				ViewInfo.IsReady = false;
			}
			CheckViewInfo();
		}
		protected internal void RibbonPaint(PaintEventArgs e){
			using(GraphicsCache cache = new GraphicsCache(e) { AllowDrawInvisibleRect = IsForceGraphicsInitialize }) {
				new RibbonPainter().Draw(cache, ViewInfo);
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			DevExpress.Utils.Mdi.ControlState.CheckPaintError(this);
			e.Graphics.Clear(Color.Transparent);
			CheckViewInfo();
			DrawGlassBackground(e);
			RibbonPaint(e);
			RaisePaintEvent(this, e);
		}
		private void DrawGlassBackground(PaintEventArgs e) {
			if(RibbonForm == null || !RibbonForm.IsGlassForm || ViewInfo.Caption.Bounds.IsEmpty) return;
			Rectangle bounds = ClientRectangle ;
			bounds.Y = GetRibbonStyle() != RibbonControlStyle.Office2007 && ViewInfo.IsAllowDisplayRibbon? ViewInfo.Header.Bounds.Bottom: ViewInfo.Caption.Bounds.Bottom;
			bounds.Height = Height - bounds.Top;
			using(Brush brush = new SolidBrush(RibbonForm.BackColor)) {
				e.Graphics.FillRectangle(brush, bounds);
			}
		}
		protected internal virtual void OnPageVisibleChanged(RibbonPage page) {
			if(IsDesignMode) return;
			if((page == SelectedPage && !page.Visible) || (page.Visible && SelectedPage == null)) SelectedPage = TotalPageCategory.GetFirstVisiblePage();;
			RefreshHeight();
		}
		protected internal virtual void OnPageChanged(RibbonPage page) {
			Refresh();
		}
		protected internal virtual void OnGroupChanged(RibbonPageGroup pageGroup) {
			Refresh();
			FireRibbonChanged();
		}
		protected internal virtual void OnPageAdded(RibbonPage page) {
			if(SelectedPage == null) SelectedPage = TotalPageCategory.GetFirstVisiblePage();
			RefreshHeight();
			FireRibbonChanged(page.Category);
		}
		protected internal virtual void OnPageRemoved(RibbonPage page) {
			if(SelectedPage == page) {
				IDisposable pageChangingContext = null;
				IRibbonMergingContext mergingContext = GetRibbonMergingContext();
				if(mergingContext != null)
					pageChangingContext = mergingContext.LockSelectedPageChanging();
				using(pageChangingContext) {
					SelectedPage = TotalPageCategory.GetFirstVisiblePage();
				}
			}
			RefreshHeight();
			FireRibbonChanged();
		}
		protected internal virtual void OnGroupRemoved(RibbonPageGroup group) {
			FireRibbonChanged(group.Page);
		}
		protected internal virtual void OnGroupAdded(RibbonPageGroup group) {
			FireRibbonChanged(group.Page);
		}
		protected internal virtual void OnPageCategoryAdded(RibbonPageCategory category) {
			RefreshHeight();
			FireRibbonChanged();
		}
		protected internal virtual void OnPageCategoryRemoved(RibbonPageCategory category) {
			foreach(RibbonPage page in category.Pages) {
				category.Pages.SetRibbon(this);
			}
			if(!TotalPageCategory.Pages.Contains(SelectedPage)) {
				IDisposable pageChangingContext = null;
				IRibbonMergingContext mergingContext = GetRibbonMergingContext();
				if(mergingContext != null)
					pageChangingContext = mergingContext.LockSelectedPageChanging();
				using(pageChangingContext) {
					SelectedPage = TotalPageCategory.GetFirstVisiblePage();
				}
			}
			RefreshHeight();
			FireRibbonChanged();
		}
		protected internal virtual void OnPageHeaderMinWidthChanged() { 
			Refresh();
		}
		protected virtual RibbonViewInfo CreateViewInfo() {
			return new RibbonViewInfo(this); 
		}
		public virtual RibbonHitInfo CalcHitInfo(Point point) { return ViewInfo.CalcHitInfo(point); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual RibbonViewInfo ViewInfo { 
			get {
				if(viewInfo == null) viewInfo = CreateViewInfo();
				return viewInfo; 
			} 
		}
		[Category("Layout"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlPageHeaderMinWidth"),
#endif
 DefaultValue(0), XtraSerializableProperty]
		public int PageHeaderMinWidth {
			get { return pageHeaderMinWidth; }
			set {
				if(value < 0) value = 0;
				if(pageHeaderMinWidth == value) return;
				pageHeaderMinWidth = value;
				OnPageHeaderMinWidthChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), XtraSerializableProperty(false, true, false)]
		public RibbonBarItems Items { get { return (RibbonBarItems)Manager.Items; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), XtraSerializableProperty(false, true, false)]
		public RibbonPageCollection Pages { get { return PageCategories.DefaultCategory.Pages; } }
		protected virtual RibbonPageHeaderItemLinkCollection CreatePageHeaderItemLinks() { return new RibbonPageHeaderItemLinkCollection(this); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DevExpress.Utils.Design.HiddenInheritableCollection]
		public RibbonQuickToolbarItemLinkCollection QuickToolbarItemLinks { get { return Toolbar.ItemLinks; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), DevExpress.Utils.Design.InheritableCollection, SkipRuntimeSerialization]
		public RibbonPageHeaderItemLinkCollection PageHeaderItemLinks { 
			get {
				if(pageHeaderItemLinks == null) pageHeaderItemLinks = CreatePageHeaderItemLinks();
				return pageHeaderItemLinks;
			} 
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlDefaultPageCategory")]
#endif
public RibbonPageCategory DefaultPageCategory { get { return PageCategories.DefaultCategory; } }
		[Browsable(false)]
		public RibbonTotalPageCategory TotalPageCategory { get { return PageCategories.TotalCategory; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(true), Browsable(false), MergableProperty(false), XtraSerializableProperty(false, true, false)]
		public RibbonPageCategoryCollection PageCategories { 
			get {
				if(pageCategories == null) pageCategories = new RibbonPageCategoryCollection(this);
				return pageCategories; 
			} 
		}
		protected virtual RibbonMiniToolbarCollection CreateMiniToolbars() { return new RibbonMiniToolbarCollection(this); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(true), MergableProperty(false), SkipRuntimeSerialization]
		public RibbonMiniToolbarCollection MiniToolbars {
			get {
				if(miniToolbars == null) miniToolbars = CreateMiniToolbars();
				return miniToolbars;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual bool IsDesignMode { get { return DesignMode; } }
		[System.ComponentModel.Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlSelectedPage"),
#endif
 DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public RibbonPage SelectedPage {
			get { return selectedPage; }
			set {
				value = UpdateSelectedPage(value);
				if(value == null) value = TotalPageCategory.GetFirstVisiblePage();
				if(value == null && IsDesignMode && TotalPageCategory.Pages.Count > 0) value = TotalPageCategory.Pages[0];
				if(SelectedPage == value) return;
				RibbonPageChangingEventArgs e = new RibbonPageChangingEventArgs(value);
				RaiseSelectedPageChanging(e);
				if(e.Cancel) return;
				IRibbonMergingContext mergingContext = GetRibbonMergingContext();
				if(mergingContext != null)
					mergingContext.SelectedPageChanged(e.Page);
				selectedPage = e.Page;
				OnSelectedPageChanged();
			}
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlAutoHideEmptyItems"),
#endif
 DefaultValue(false), XtraSerializableProperty]
		public bool AutoHideEmptyItems {
			get { return autoHideEmptyItems; }
			set {
				if(AutoHideEmptyItems == value)
					return;
				autoHideEmptyItems = value;
				if(MergedRibbon != null) MergedRibbon.AutoHideEmptyItems = value;
				UpdateViewInfo();
				Invalidate();
			}
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlAllowKeyTips"),
#endif
 DefaultValue(true), XtraSerializableProperty]
		public bool AllowKeyTips {
			get { return allowKeyTips; }
			set {
				if(AllowKeyTips == value)
					return;
				allowKeyTips = value;
				DeactivateKeyboardNavigation();
			}
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlAllowMinimizeRibbon"),
#endif
 DefaultValue(true), XtraSerializableProperty]
		public bool AllowMinimizeRibbon {
			get { return allowMinimizeRibbon; }
			set {
				if(AllowMinimizeRibbon == value)
					return;
				allowMinimizeRibbon = value;
				OnAllowMinimizeRibbonChanged();
			}
		}
		protected virtual void OnAllowMinimizeRibbonChanged() {
			if(Minimized && !AllowMinimizeRibbon)
				Minimized = false;
		}
		internal RibbonPage UpdateSelectedPage(RibbonPage page) {
			if(page == null || MergedRibbon == null || page.Ribbon != MergedRibbon) return page;
			return TotalPageCategory.GetPageByText(page.Text);
		} 
		public virtual void UpdateViewInfo() {
			ViewInfo.IsReady = false;
			CheckViewInfo();
		}
		protected internal virtual void OnSelectedPageChangedCore() {
			Manager.SelectionInfo.CloseAllPopups();
			HideApplicationButtonContentControl();
			CloseHoverForms();
			ViewInfo.Panel.SetPanelScrollOffset(0);
			ViewInfo.AllowCachedItemInfo = false;
			UpdateViewInfo();
			Invalidate();
			if(IsMinimizedRibbonOpened) {
				ShowMinimizedRibbon();
			}
			RaiseSelectedPageChanged();
			if(IsKeyboardActive) {
				KeyboardSelectFirstObject();
			}
			if(KeyTipManager.Show && KeyTipManager.KeyTipMode == RibbonKeyTipMode.PanelKeyTips)
				KeyTipManager.HideKeyTips();
			ResetAccessible();
#if DXWhidbey
			AccessibleNotifyClients(AccessibleEvents.Selection, RibbonControl.AccessibleObjectRibbonPageList, TotalPageCategory.Pages.IndexOf(SelectedPage));
			AccessibleNotifyClients(AccessibleEvents.Focus, RibbonControl.AccessibleObjectRibbonPageList, TotalPageCategory.Pages.IndexOf(SelectedPage));
#endif
		}
		protected virtual void OnSelectedPageChanged() {
			UpdateVisualEffects(UpdateAction.BeginUpdate);
			OnSelectedPageChangedCore();
			UpdateVisualEffects(UpdateAction.EndUpdate);
		}
		protected virtual RibbonItemViewInfo GetPanelItem() {
			foreach(RibbonPageGroupViewInfo pageGroupInfo in ViewInfo.Panel.Groups) {
				foreach(RibbonItemViewInfo itemInfo in pageGroupInfo.Items) {
					if(itemInfo != null && !(itemInfo is RibbonSeparatorItemViewInfo)) return itemInfo;
				}
			}
			return null;	
		}
		protected virtual RibbonItemViewInfo GetToolbarItem() {
			if(ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Hidden) return null;
			foreach(RibbonItemViewInfo itemInfo in ViewInfo.Toolbar.Items) {
				if(itemInfo != null && !(itemInfo is RibbonSeparatorItemViewInfo)) return itemInfo;
			}
			return null;
		}
		protected virtual void KeyboardSelectFirstObject() {
			NavigatableObjects = null;
			NavigatableObjectList = null;
			CheckViewInfo();
			RibbonPageViewInfo page = ViewInfo.Header.Pages[SelectedPage];
			RibbonItemViewInfo panelItem = GetPanelItem();
			RibbonItemViewInfo toolbarItem = GetToolbarItem();
			if(page != null) {
				ViewInfo.KeyboardActiveObject = new NavigationObjectPage(this, page);	
			}
			else if(panelItem != null) {
				ViewInfo.KeyboardActiveObject = new NavigationObjectRibbonPageGroupItem(this, panelItem);
			}
			else if(toolbarItem != null) {
				ViewInfo.KeyboardActiveObject = new NavigationObjectRibbonToolbarItem(this, toolbarItem);
			}
			else if(ViewInfo.IsAllowApplicationButton) {
				ViewInfo.KeyboardActiveObject = new NavigationObjectApplicationButton(this, ViewInfo.ApplicationButton);
			}
			else {
				DeactivateKeyboardNavigation();
			}
		}
		protected internal virtual void CheckHeight() {
			if(IsDestroying || !IsAllowAutoHeight) return;
			int newHeight = GetMinHeight();
			if(RibbonForm != null && RibbonForm.LayoutSuspendCountCore > 1)
				return;
			SetBounds(Left, Top, Width, newHeight, BoundsSpecified.Height);
			this.needCheckHeight = false;
			if(RibbonForm != null) RibbonForm.UpdateFormMargins();
		}
		public virtual int GetMinHeight() {
			CheckViewInfo();
			if(Manager == null || !ViewInfo.IsReady) return Height;
			return ViewInfo.CalcMinHeight();
		}
		protected virtual bool IsAllowAutoHeight { get { return true; } } 
		public override Size GetPreferredSize(Size proposedSize) {
			if(ViewInfo.SuppressInvalidate)
				return Size;
			ViewInfo.IsReady = false;
			return new Size(Width, GetMinHeight());
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if((specified & BoundsSpecified.Height) != 0) {
				if(IsAllowAutoHeight) height = GetMinHeight();
			}
			base.SetBoundsCore(x, y, width, height, specified);
			if(RibbonForm != null) RibbonForm.UpdateFormMargins();
		}
		protected internal RibbonForm RibbonForm { get { return FindForm() as RibbonForm; } }
		void DelayedPatchVisible() {
			if(this.shouldPatchVisibleOnHandleCreated) {
				this.shouldPatchVisibleOnHandleCreated = false;
				this.patchVisible = true;
				Visible = !IsMdiChildRibbon;
				this.patchVisible = false;
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			RibbonForm form = RibbonForm;
			if(form != null && form.Ribbon == null && Parent == form)
				form.Ribbon = this;
			DelayedPatchVisible();
			ForceInitialize();
			if(IsAllowAutoHeight) {
				ViewInfo.IsReady = false;
				Height = GetMinHeight();
			}
			OnApplicationCaptionChanged();
		}
		protected internal virtual void OnControllerChanged(bool setNewController) {
			OnControllerChanged();
			UpdateTouchUIMode();
		}
		protected internal virtual void OnControllerChanged() {
			this.needCheckHeight = true;
			ColoredRibbonElementsCache.ClearRibbonItems();
			ViewInfo.SetAppearanceDirty();
			ViewInfo.AllowCachedItemInfo = false;
			Refresh(); 
			if(StatusBar != null) {
				StatusBar.ViewInfo.IsReady = false;
				StatusBar.Refresh();
			}
			if(RibbonForm != null) {
				RibbonForm.OnControllerChanged();
			}
			ClosePopupForms(ViewInfo.CanCloseMinimizedRibbon(this), true, true);
			foreach(RibbonMiniToolbar miniToolbar in MiniToolbars)
				miniToolbar.OnControllerChanged();
			if(MinimizedRibbonPopupForm != null)
				MinimizedRibbonPopupForm.OnControllerChanged();
		}
		private void UpdateTouchUIMode() {
			if(OptionsTouch.TouchUI == DefaultBoolean.Default)
				return;
			if(OptionsTouch.TouchUI == DefaultBoolean.True)
				WindowsFormsSettings.TouchUIMode = TouchUIMode.True;
			else
				WindowsFormsSettings.TouchUIMode = TouchUIMode.False;
		}
		protected internal virtual void OnAddToToolbar(BarItemLink link) {
			if(link == null) return;
			Toolbar.ItemLinks.Add(link.Item);
		}
		protected internal virtual void OnRemoveFromToolbar(BarItemLink link) {
			if(SourceToolbar.Contains(link.Item)) SourceToolbar.ItemLinks.Remove(link.Item);
		}
		protected internal virtual void OnCustomizeRibbon() {
			DialogResult res;
			RibbonCustomizationModel info;
			using(RibbonCustomizationForm frm = new RibbonCustomizationForm(this)) {
				frm.StartPosition = FormStartPosition.CenterScreen;
				res = frm.ShowDialog();
				if(res != DialogResult.OK)
					return;
				info = frm.GetResult();
			}
			ApplyCustomizationSettings(info);
		}
		protected internal virtual void ApplyCustomizationSettings(RibbonCustomizationModel model) {
			RibbonProcessor rp = new RibbonProcessor(this);
			rp.Process(model);
		}
		protected internal virtual void OnRemovePageGroupLinkFromToolbar(RibbonToolbarPopupItemLink popupLink) {
			if(SourceToolbar.ItemLinks.Count == 0) return;
			if(popupLink.PageGroup.Ribbon.PopupGroupForm != null) popupLink.PageGroup.Ribbon.PopupGroupForm.UnpressGroupToolbarItem();
			foreach(BarItemLink link in SourceToolbar.ItemLinks) {
				RibbonToolbarPopupItemLink plink = link as RibbonToolbarPopupItemLink;
				if(plink == null || plink.PageGroup == null || plink.PageGroup.GetOriginalPageGroup() != popupLink.PageGroup.GetOriginalPageGroup()) continue;
				SourceToolbar.ItemLinks.Remove(link);
				break;
			}
		}
		protected internal virtual void OnChangeQuickToolbarPosition() {
			if(ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Below)
				ToolbarLocation = RibbonQuickAccessToolbarLocation.Above;
			else
				ToolbarLocation = RibbonQuickAccessToolbarLocation.Below;
		}
		protected internal RibbonCustomizationPopupMenu CustomizationPopupMenu {
			get {
				if(customizationPopupMenu == null) customizationPopupMenu = new RibbonCustomizationPopupMenu(this);
				return customizationPopupMenu;
			}
			set {
				if(customizationPopupMenu == value) return;
				if(customizationPopupMenu != null) customizationPopupMenu.Dispose();
				customizationPopupMenu = value;
			}
		}
		protected void DestroyCustomizationPopupMenu() {
			if(this.customizationPopupMenu != null) {
				this.customizationPopupMenu.Dispose();
				this.customizationPopupMenu = null;
			}
		}
		#region Handler
		protected override void OnLostCapture() {
			base.OnLostCapture();
			Handler.OnLostCapture();
			DeactivateKeyTips();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			Handler.OnKeyDown(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.Handled) return;
			Handler.OnKeyPress(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.Handled) return;
			Handler.OnKeyUp(e);
		}
		[Browsable(false)]
		public override ContextMenu ContextMenu {
			get { return base.ContextMenu; }
			set { base.ContextMenu = value; }
		}
#if DXWhidbey        
		[Browsable(false)]
		public override ContextMenuStrip ContextMenuStrip {
			get { return base.ContextMenuStrip; }
			set { base.ContextMenuStrip = value; }
		}
#endif        
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseDown(ee);
				if(ee.Handled) return;
				Handler.OnMouseDown(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseWheel(ee);
				if(ee.Handled) return;
				Handler.OnMouseWheel(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseUp(ee);
				if(ee.Handled) return;
				Handler.OnMouseUp(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			CheckViewInfo();
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseMove(ee);
				if(ee.Handled) return;
				Handler.OnMouseMove(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseEnter(EventArgs e) {
			CheckViewInfo();
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(this, e);
			try {
				base.OnMouseEnter(ee);
				if(ee.Handled) return;
				Handler.OnMouseEnter(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			CloseHoverForms();
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(this, e);
			try {
				base.OnMouseLeave(ee);
				if(ee.Handled) return;
				Handler.OnMouseLeave(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected internal DevExpress.XtraBars.Ribbon.Handler.BaseHandler Handler {
			get {
				if(handler == null) handler = CreateHandler();
				return handler;
			}
		}
		protected virtual DevExpress.XtraBars.Ribbon.Handler.BaseHandler CreateHandler() { return new RibbonHandler(this); }
		#endregion Handler
		#region IBarObject Members
		bool IBarObject.IsBarObject { get { return true; } }
		BarManager IBarObject.Manager { get { return Manager; } }
		BarMenuCloseType IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) { return RibbonShouldCloseMenuOnClick(e); }
		protected virtual BarMenuCloseType RibbonShouldCloseMenuOnClick(MouseInfoArgs e) {
			DeactivateKeyTips();
			if(IsGalleryDropDownInDesignerClicked(e)) return BarMenuCloseType.None;
			Point p = PointToClient(new Point(e.X, e.Y));
			RibbonHitInfo hitInfo = ViewInfo.CalcHitInfo(p);
			if(!e.MouseUp && IsMinimizedRibbonOpened) {
				if(ShouldCloseMinimizedRibbonOpened(hitInfo)) 
					return BarMenuCloseType.All;
			}
			if(hitInfo.InPageGroup && hitInfo.PageGroupInfo.Minimized && PopupGroupForm != null) return BarMenuCloseType.None;
			if(IsPopupToolbarOpened && hitInfo.ItemInfo is DevExpress.XtraBars.Ribbon.Internal.RibbonQuickToolbarDropItemViewInfo) return BarMenuCloseType.None;
			if(IsMinimizedRibbonOpened) return BarMenuCloseType.None;
			if(IsPopupFormOpened) return BarMenuCloseType.All;
			if(hitInfo.InItem) {
				if(hitInfo.Item != null && !hitInfo.Item.Enabled && ShouldClosePopupsOnDisabledItemClick(hitInfo.Item))
					return BarMenuCloseType.All;
				return BarMenuCloseType.None;
			}
			if(hitInfo.InGallery) {
				if(e.MouseUp || Manager.SelectionInfo.OpenedPopups.Count == 0) 
					return BarMenuCloseType.None;
				return BarMenuCloseType.All;
			}
			if(hitInfo.HitTest == RibbonHitTest.PageGroupCaptionButton) return BarMenuCloseType.All;
			if(hitInfo.HitTest == RibbonHitTest.ApplicationButton && ViewInfo.ApplicationButtonPopupActive) return BarMenuCloseType.None;
			if(IsGalleryDropDownInDesignerClicked(e)) return BarMenuCloseType.None;
			return BarMenuCloseType.All;
		}
		protected virtual bool ShouldClosePopupsOnDisabledItemClick(BarItemLink barItemLink) {
			return true;
		}
		protected virtual bool ShouldCloseMinimizedRibbonOpened(RibbonHitInfo hitInfo) {
			bool baseCheck = hitInfo.HitTest != RibbonHitTest.PageHeader || hitInfo.Page == null;
			if(!ViewInfo.IsPopupFullScreenModeActive)
				return baseCheck;
			return baseCheck && !hitInfo.InToolbar && !hitInfo.InItem;
		}
		internal virtual bool IsGalleryDropDownInDesignerClicked(MouseInfoArgs e) {
			if(!IsDesignMode || e.Button != MouseButtons.Right) return false;
			if(Manager.SelectionInfo.OpenedPopups.Count == 0) return false;
			GalleryDropDownBarControl dropDownControl = Manager.SelectionInfo.OpenedPopups[0] as GalleryDropDownBarControl;
			if(dropDownControl == null) return false;
			return dropDownControl.FindForm().Bounds.Contains(e.Location);
		}
		bool HasActiveEditorWithNoPopupOpen(RibbonControl ribbon) {
			if(ribbon.Manager.SelectionInfo.ActiveEditor != null) {
				if(ribbon.Manager.SelectionInfo.OpenedPopups.Count == 0) return true;
			}
			return false;
		}
		bool IBarObject.ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) {
			if(Manager == null) return true;
			DeactivateKeyTips();
			DeactivateKeyboardNavigation();
			if(PopupHelper.IsBelowModalForm(MinimizedRibbonPopupForm, BarManager.ClosePopupOnModalFormShow) ||
				PopupHelper.IsBelowModalForm(PopupGroupForm, BarManager.ClosePopupOnModalFormShow) ||
				PopupHelper.IsBelowModalForm(PopupToolbar, BarManager.ClosePopupOnModalFormShow))
				return false;
			CustomBaseRibbonControl customRibbon = this as CustomBaseRibbonControl;
			if(customRibbon != null && PopupHelper.IsBelowModalForm(customRibbon, BarManager.ClosePopupOnModalFormShow))
				return false;
			if(Manager == null) return true;
			if(HasActiveEditorWithNoPopupOpen(this))
				return true;
			if(MergedRibbon != null && HasActiveEditorWithNoPopupOpen(MergedRibbon))
				return true;
			RibbonMinimizedControl minimizedRibbon = this as RibbonMinimizedControl;
			if(minimizedRibbon != null) {
				IBarObject rbo = minimizedRibbon.SourceRibbon as IBarObject;
				if(rbo != null && rbo.ShouldCloseOnOuterClick(control, e))
					return true;
			}
			return false; 
		}
		bool IBarObject.ShouldCloseOnLostFocus(Control newFocus) {
			DeactivateKeyTips();
			DeactivateKeyboardNavigation();
			return true; 
		}
		#endregion
		#region ICustomBarControl Members
		void ICustomBarControl.ProcessKeyDown(KeyEventArgs e) {
			Handler.OnKeyDown(e);
		}
		bool ICustomBarControl.IsNeededKey(KeyEventArgs e) {
			return Handler.IsNeededKey(e);
		}
		bool ICustomBarControl.IsInterceptKey(KeyEventArgs e) {
			return Handler.IsInterceptKey(e);
		}
		void ICustomBarControl.ProcessKeyUp(KeyEventArgs e) {
			Handler.OnKeyUp(e);
		}
		bool ICustomBarControl.ProcessKeyPress(KeyPressEventArgs e) {
			if(IsShowKeyTip) {
				OnKeyPress(e);
				return true;
			}
			return false; 
		}
		Control ICustomBarControl.Control { get { return this; } }
		#endregion
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlBeforeApplicationButtonContentControlShow")]
#endif
		public event EventHandler BeforeApplicationButtonContentControlShow {
			add { Events.AddHandler(beforeApplicationButtonContentControlShow, value); }
			remove { Events.RemoveHandler(beforeApplicationButtonContentControlShow, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlAfterApplicationButtonContentControlHidden")]
#endif
		public event EventHandler AfterApplicationButtonContentControlHidden {
			add { Events.AddHandler(afterApplicationButtonContentControlHidden, value); }
			remove { Events.RemoveHandler(afterApplicationButtonContentControlHidden, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlHyperlinkClick")]
#endif
		public event HyperlinkClickEventHandler HyperlinkClick {
			add { Manager.HyperlinkClick += value; }
			remove { Manager.HyperlinkClick -= value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlItemClick")]
#endif
		public event ItemClickEventHandler ItemClick {
			add { Manager.ItemClick += value; }
			remove { Manager.ItemClick -= value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlItemPress")]
#endif
		public event ItemClickEventHandler ItemPress {
			add { Manager.ItemPress += value; }
			remove { Manager.ItemPress -= value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlPageGroupCaptionButtonClick")]
#endif
		public event RibbonPageGroupEventHandler PageGroupCaptionButtonClick {
			add { Events.AddHandler(pageGroupCaptionButtonClick, value); }
			remove { Events.RemoveHandler(pageGroupCaptionButtonClick, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationButtonClick")]
#endif
		public event EventHandler ApplicationButtonClick {
			add { Events.AddHandler(applicationButtonClick, value); }
			remove { Events.RemoveHandler(applicationButtonClick, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlApplicationButtonDoubleClick")]
#endif
		public event EventHandler ApplicationButtonDoubleClick {
			add { Events.AddHandler(applicationButtonDoubleClick, value); }
			remove { Events.RemoveHandler(applicationButtonDoubleClick, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlShowCustomizationMenu")]
#endif
		public event RibbonCustomizationMenuEventHandler ShowCustomizationMenu {
			add { Events.AddHandler(showCustomizationMenu, value); }
			remove { Events.RemoveHandler(showCustomizationMenu, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlSelectedPageChanging")]
#endif
		public event RibbonPageChangingEventHandler SelectedPageChanging {
			add { Events.AddHandler(selectedPageChanging, value); }
			remove { Events.RemoveHandler(selectedPageChanging, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlSelectedPageChanged")]
#endif
		public event EventHandler SelectedPageChanged {
			add { Events.AddHandler(selectedPageChanged, value); }
			remove { Events.RemoveHandler(selectedPageChanged, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlScreenModeChanged")]
#endif
		public event RibbonScreenModeChangedEventHandler ScreenModeChanged {
			add { Events.AddHandler(screenModeChanged, value); }
			remove { Events.RemoveHandler(screenModeChanged, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlMerge")]
#endif
		public event RibbonMergeEventHandler Merge {
			add { Events.AddHandler(merge, value); }
			remove { Events.RemoveHandler(merge, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlUnMerge")]
#endif
		public event RibbonMergeEventHandler UnMerge {
			add { Events.AddHandler(unMerge, value); }
			remove { Events.RemoveHandler(unMerge, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlUnMerge")]
#endif
		public event RibbonMergeEventHandler BeforeUnMerge {
			add { Events.AddHandler(beforeUnmerge, value); }
			remove { Events.RemoveHandler(beforeUnmerge, value); }
		}
		public event MinimizedRibbonEventHandler MinimizedRibbonShowing {
			add { Events.AddHandler(minimizedRibbonShow, value); }
			remove { Events.RemoveHandler(minimizedRibbonShow, value); }
		}
		public event MinimizedRibbonEventHandler MinimizedRibbonHiding {
			add { Events.AddHandler(minimizedRibbonHide, value); }
			remove { Events.RemoveHandler(minimizedRibbonHide, value); }
		}
		protected internal bool HasMergeEventSubscription {
			get { return (Events[merge] != null) && (Events[unMerge] != null); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlMinimizedChanged")]
#endif
		public event EventHandler MinimizedChanged {
			add { Events.AddHandler(minimizedChanged, value); }
			remove { Events.RemoveHandler(minimizedChanged, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlToolbarLocationChanged")]
#endif
		public event EventHandler ToolbarLocationChanged {
			add { Events.AddHandler(toolbarLocationChanged, value); }
			remove { Events.RemoveHandler(toolbarLocationChanged, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlColorSchemeChanged")]
#endif
		public event EventHandler ColorSchemeChanged {
			add { Events.AddHandler(colorSchemeChanged, value); }
			remove { Events.RemoveHandler(colorSchemeChanged, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlInvalidSaveRestoreLayoutException")]
#endif
		public event EventHandler<InvalidLayoutExceptionEventArgs> InvalidSaveRestoreLayoutException {
			add { Events.AddHandler(invalidSaveRestoreLayoutException, value); }
			remove { Events.RemoveHandler(invalidSaveRestoreLayoutException, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlResetLayout")]
#endif
		public event EventHandler<ResetLayoutEventArgs> ResetLayout {
			add { Events.AddHandler(resetLayout, value); }
			remove { Events.RemoveHandler(resetLayout, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlRibbonStyleChanged")]
#endif
		public event EventHandler<RibbonStyleChangedEventArgs> RibbonStyleChanged {
			add { Events.AddHandler(ribbonStyleChanged, value); }
			remove { Events.RemoveHandler(ribbonStyleChanged, value); }
		}
		public event BarItemCustomDrawEventHandler CustomDrawItem {
			add { Manager.CustomDrawItem += value; }
			remove { Manager.CustomDrawItem -= value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonControlCustomizeQatMenu")]
#endif
		public event CustomizeQatMenuEventHandler CustomizeQatMenu {
			add { Events.AddHandler(customizeQatMenu, value); }
			remove { Events.RemoveHandler(customizeQatMenu, value); }
		}
		protected virtual internal void RaiseCustomizeQatMenu(CustomizeQatMenuEventArgs e) {
			CustomizeQatMenuEventHandler handler = (CustomizeQatMenuEventHandler)Events[customizeQatMenu];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseRibbonStyleChanged(RibbonStyleChangedEventArgs e) {
			EventHandler<RibbonStyleChangedEventArgs> handler = Events[ribbonStyleChanged] as EventHandler<RibbonStyleChangedEventArgs>;
			if(handler != null) handler(this, e);
		}
		protected virtual internal void RaiseResetLayout(ResetLayoutEventArgs e) {
			EventHandler<ResetLayoutEventArgs> handler = (EventHandler<ResetLayoutEventArgs>)Events[resetLayout];
			if(handler != null) handler(this, e);
		}
		protected virtual internal void RaiseShowCustomizationMenu(RibbonCustomizationMenuEventArgs args) {
			RibbonCustomizationMenuEventHandler handler = (RibbonCustomizationMenuEventHandler)Events[showCustomizationMenu];
			if(handler != null) handler(this, args);
		}
		protected virtual void RaiseMinimizedChanged() {
			EventHandler handler = Events[minimizedChanged] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseToolbarLocationChanged() {
			EventHandler handler = Events[toolbarLocationChanged] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseColorSchemeChanged() {
			EventHandler handler = Events[colorSchemeChanged] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseInvalidSaveRestoreLayoutException(InvalidLayoutExceptionEventArgs e) {
			EventHandler<InvalidLayoutExceptionEventArgs> handler = Events[invalidSaveRestoreLayoutException] as EventHandler<InvalidLayoutExceptionEventArgs>;
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseMerge(RibbonMergeEventArgs e) {
			RibbonMergeEventHandler handler = Events[merge] as RibbonMergeEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseBeforeUnMerge(RibbonMergeEventArgs e) {
			RibbonMergeEventHandler handler = Events[beforeUnmerge] as RibbonMergeEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseUnMerge(RibbonMergeEventArgs e) {
			RibbonMergeEventHandler handler = Events[unMerge] as RibbonMergeEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseMinimizedRibbonShowing(MinimizedRibbonEventArgs e) {
			MinimizedRibbonEventHandler handler = Events[minimizedRibbonShow] as MinimizedRibbonEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseMinimizedRibbonHiding(MinimizedRibbonEventArgs e) {
			MinimizedRibbonEventHandler handler = Events[minimizedRibbonHide] as MinimizedRibbonEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseSelectedPageChanging(RibbonPageChangingEventArgs e) {
			RibbonPageChangingEventHandler handler = (RibbonPageChangingEventHandler)Events[selectedPageChanging];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseSelectedPageChanged() {
			EventHandler handler = (EventHandler)Events[selectedPageChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseScreenModeChanged(ScreenModeChangedEventArgs e) {
			RibbonScreenModeChangedEventHandler handler = (RibbonScreenModeChangedEventHandler)Events[screenModeChanged];
			if(handler != null) handler(this, e);
		}
		protected virtual internal void RaisePageGroupCaptionButtonClick(RibbonPageGroupEventArgs e) {
			RibbonPageGroupEventHandler handler = (RibbonPageGroupEventHandler)Events[pageGroupCaptionButtonClick];
			if(handler != null) handler(this, e);
			e.PageGroup.RaiseCaptionButtonClick(e);
		}
		protected virtual internal void RaiseApplicationButtonClick(EventArgs e) {
			EventHandler handler = (EventHandler)Events[applicationButtonClick];
			if(handler != null) handler(this, e);
		}
		protected virtual internal void RaiseApplicationButtonDoubleClick(EventArgs e) {
			EventHandler handler = (EventHandler)Events[applicationButtonDoubleClick];
			if(handler != null) handler(this, e);
		}
		#region IDXMenuManager Members
		IDXMenuManager IDXMenuManager.Clone(Form newForm) {
			return ((IDXMenuManager)Manager).Clone(newForm);
		}
		void IDXMenuManager.DisposeManager() {
			Dispose();
		}
		void IDXMenuManager.ShowPopupMenu(DXPopupMenu menu, Control control, Point pos) {
			((IDXMenuManager)Manager).ShowPopupMenu(menu, control, pos);
		}
		object IDXDropDownMenuManager.ShowDropDownMenu(DXPopupMenu menu, Control control, Point pos) {
			return ((IDXDropDownMenuManager)Manager).ShowDropDownMenu(menu, control, pos);
		}
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate { get { return !DesignMode; } }
		Control ISupportXtraAnimation.OwnerControl { get { return this; } }
		#endregion
		protected internal virtual void OnAnimation(BaseAnimationInfo info) { 
			if(IsDesignMode) return;
			BarAnimatedItemsHelper.Animate(this, Manager, info, ViewInfo.AnimationInvoker);
		}
		#region DragEvents
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			if(!IsDesignMode) {
				base.OnQueryContinueDrag(e);
				return;
			}
			Manager.Helper.DragManager.ItemOnQueryContinueDrag(e, this);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if(!IsDesignMode) {
				base.OnGiveFeedback(e);
				return;
			}
			Manager.Helper.DragManager.ItemOnGiveFeedback(e, this);
		}
		protected override void OnDragEnter(DragEventArgs e) {
			if(!IsDesignMode) {
				base.OnDragEnter(e);
				return;
			}
			Manager.Helper.DragManager.FireDoDragging = false;
			if(!Manager.Helper.DragManager.CanAcceptDragObject(e.Data))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
		}
		protected override void OnDragLeave(EventArgs e) {
			if(!IsDesignMode) {
				base.OnDragLeave(e);
				return;
			}
			Manager.Helper.DragManager.FireDoDragging = true;
		}
		protected override void OnDragDrop(DragEventArgs e) {
			if(!IsDesignMode) {
				base.OnDragDrop(e);
				return;
			}
			Manager.Helper.DragManager.StopDragging(this, e.Effect, false);
		}
		protected override void OnDragOver(DragEventArgs e) {
			if(!IsDesignMode) {
				base.OnDragOver(e);
				return;
			}
			if(!Manager.Helper.DragManager.CanAcceptDragObject(e.Data))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
			Manager.Helper.DragManager.DoDragging(this, new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
		}		
		#endregion
		protected internal bool InDesignerRect(Point p) {
			return ViewInfo.Header.DesignerRect.Contains(p) || ViewInfo.Toolbar.DesignerRect.Contains(p);
		}
		internal virtual void FireRibbonChanged() {
			FireRibbonChanged(this);
		}
		internal virtual void FireRibbonChanged(Component component) {
			if(!IsDesignMode) return;
			OnFireRibbonChanged(component);
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			ViewInfo.SetAppearanceDirty();
			Refresh();
		}
		protected virtual void OnFireRibbonChanged(Component component) {
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				srv.OnComponentChanged(component, null, null, null);
			}
		}
		protected internal virtual void OnBeforeKeyTipClick() {
			DeactivateKeyboardNavigation(false, true);
		}
		bool isLoading = false;
		protected internal bool IsLoading {
			get { return isLoading; }
			set { isLoading = value; }
		}
		#region ISupportInitialize Members
		public virtual void BeginInit() {
			IsLoading = true;
		}
		public virtual void EndInit() {
			IsLoading = false;
			Manager.Helper.LoadHelper.Loaded = false;
			ForceInitialize();
			RibbonSourceStateInfo.Create(this);
			AutoRestoreLayoutFromXmlCore();
			InitializeSkinRibbonGalleriesIfExists();
		}
		#endregion
		#region IXtraSerializable
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
		}
		#endregion
		protected virtual void InitializeSkinRibbonGalleriesIfExists() {
			if(DesignMode) return;
			for(int i = 0; i < Items.Count; i++) {
				SkinRibbonGalleryBarItem srg = Items[i] as SkinRibbonGalleryBarItem;
				if(srg != null)
					srg.OnInitialize();
			}
		}
		public void SetCurrentLayoutAsDefault() {
			RibbonSourceStateInfo.ReCreate(this);
		}
		#region ISupportGlassRegions Members
		bool ISupportGlassRegions.IsOnGlass(Rectangle rect) {
			if(RibbonForm == null || !RibbonForm.IsGlassForm)
				return false;
			return (ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above && ViewInfo.Toolbar.Bounds.Contains(rect)) || (AllowGlassTabHeader && ViewInfo.Header.PageHeaderItemsBounds.Contains(rect));
		}
		#endregion
		RibbonApplicationButtonContainerControl applicationButtonContentControl;
		protected internal RibbonApplicationButtonContainerControl ApplicationButtonContentControl {
			get {
				if(applicationButtonContentControl == null)
					applicationButtonContentControl = CreateApplicationButtonContentControl();
				return applicationButtonContentControl; 
			}
			set { applicationButtonContentControl = value; }
		}
		protected internal void ResetApplicationButtonContentControlCache() {
			applicationButtonContentControl = null;
		}
		protected virtual RibbonApplicationButtonContainerControl CreateApplicationButtonContentControl() {
			if(ShouldCreateOffice2013BackstageViewContainerControl)
				return new RibbonOffice2013BackstageViewContainerControl(this);
			return new RibbonApplicationButtonContainerControl(this);
		}
		protected internal virtual bool ShouldCreateOffice2013BackstageViewContainerControl {
			get {
				BackstageViewControl bsvc = ApplicationButtonDropDownControl as BackstageViewControl;
				if(bsvc == null) return false;
				return bsvc.ShouldUseOffice2013ControlStyle;
			}
		}
		void RaiseBeforeApplicationButtonContentControlShow() {
			EventHandler handler = Events[beforeApplicationButtonContentControlShow] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		internal void RaiseAfterApplicationButtonContentControlHidden() {
			EventHandler handler = Events[afterApplicationButtonContentControlHidden] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		bool restoreFullScreenAfterBackstageClosing = false;
		void OnApplicationButtonPopupClosed(object sender, EventArgs e) {
			PopupControl pc = (PopupControl)sender;
			pc.CloseUp -= new EventHandler(OnApplicationButtonPopupClosed);
			ViewInfo.ApplicationButtonPopupActive = false;
		}
		protected internal virtual Point GetApplicationMenuLocation() {
			Form frm = FindForm();
			bool isFormMaximized = frm != null && frm.WindowState == FormWindowState.Maximized;
			Point res = new Point(0, 0);
			if(!(ApplicationButtonDropDownControl is ApplicationMenu)) {
				res = new Point(ViewInfo.ApplicationButton.Bounds.X, ViewInfo.Header.Bounds.Bottom);
			}
			else {
				int x = ViewInfo.IsRightToLeft ? ViewInfo.ContentBounds.Right + 1 : ViewInfo.ContentBounds.Left - 1;
				res = new Point(x, ViewInfo.Header.Bounds.Y - 1);
			}
			if(isFormMaximized && res.X < 0)
				res.X = 0;
			return res;
		}
		public virtual void ShowApplicationButtonContentControl() {
			if(ApplicationButtonPopupControl != null) {
				ApplicationButtonPopupControl.CloseUp += new EventHandler(OnApplicationButtonPopupClosed);
				ApplicationButtonPopupControl.ShowPopup(Manager, PointToScreen(GetApplicationMenuLocation()));
				ViewInfo.ApplicationButtonPopupActive = true;
				if(!ApplicationButtonPopupControl.Visible)
					OnApplicationButtonPopupClosed(ApplicationButtonPopupControl, EventArgs.Empty);	
				return;
			}
			Control appDropDownControl = (Control)ApplicationButtonDropDownControl;
			if(appDropDownControl == null)
				return;
			DisableExpandCollapseButton();
			if(ApplicationButtonContentControl.ContentVisible)
				return;
			ApplicationButtonContentControl.Content = appDropDownControl;
			RaiseBeforeApplicationButtonContentControlShow();
			ApplicationButtonContentControl.ShowContent();
			appDropDownControl.Visible = true;
			if(ViewInfo.IsFullScreenModeActive) {
				restoreFullScreenAfterBackstageClosing = true;
				OnFullScreenModeChangeCore();
			}
		}
		protected internal void EnableExpandCollapseButton() {
			SetExpandCollapseButtonState(true);
		}
		protected internal void DisableExpandCollapseButton() {
			SetExpandCollapseButtonState(false);
		}
		protected internal void SetExpandCollapseButtonState(bool enabled) {
			if(enabled && ViewInfo != null & ViewInfo.IsPopupFullScreenModeActive)
				return;
			ExpandCollapseItem.Enabled = enabled;
		}
		public virtual void HideApplicationButtonContentControl() {
			EnableExpandCollapseButton();
			if(ApplicationButtonPopupControl != null) {
				ApplicationButtonPopupControl.HidePopup();
				return;
			}
			if(this.applicationButtonContentControl == null || !ApplicationButtonContentControl.ContentVisible)
				return;
			if(restoreFullScreenAfterBackstageClosing) {
				OnFullScreenModeChangeCore();
				restoreFullScreenAfterBackstageClosing = false;
			}
			ApplicationButtonContentControl.HideContent();
			Refresh();
		}
		protected internal virtual void ToggleApplicationButtonContentControlVisibility() {
			if(IsDesignMode)
				return;
			if(!ApplicationButtonContentControl.ContentVisible)
				ShowApplicationButtonContentControl();
			else
				HideApplicationButtonContentControl();
			Invalidate();
		}
		protected internal virtual bool IsSystemLink(BarItemLink link) {
			return link == MDICloseItemLink || link == MDIMinimizeItemLink || link == MDIRestoreItemLink || link == ExpandCollapseItemLink || link == AutoHiddenPagesMenuItemLink;
		}
		protected internal virtual void UpdateSystemLinkGlyph(BarItemLink link, ObjectState state) {
			if(IsExpandButtonInPanel) ViewInfo.Panel.PanelItems.UpdateSystemLinkGlyph(link, state);
			ViewInfo.Header.UpdateSystemLinkGlyph(link, state);
		}
		internal void ResetMiniToolbars() {
			foreach(RibbonMiniToolbar tb in MiniToolbars) {
				tb.ResetForm();
			}
		}
		protected internal virtual void UpdateMiniToolbarsVisibility(Point point, Control wnd) {
			foreach(RibbonMiniToolbar toolbar in MiniToolbars) {
				toolbar.UpdateVisibility(point, wnd);
			}
		}
		protected internal virtual void UpdateInRibbonGalleryCommandButtons(Point point) {
			if(SelectedPage == null)
				return;
			foreach(RibbonPageGroup pageGroup in SelectedPage.Groups) {
				foreach(BarItemLink link in pageGroup.ItemLinks) {
					RibbonGalleryBarItemLink galleryLink = link as RibbonGalleryBarItemLink;
					if(galleryLink == null)
						continue;
					galleryLink.UpdateMacStyleCommandButtonVisibility(point);
				}
			}
		}
		MouseClickInfo clickInfo = new MouseClickInfo();
		internal void OnMouseDownCore(MouseInfoArgs e) {
			clickInfo.MouseDown();
		}
		internal void OnMouseUpCore(MouseInfoArgs e) {
			if(clickInfo.IsClick()) {
				EmulateClickEvent(e, true);
			}
			clickInfo.Reset();
		}
		internal void EmulateClickEvent(MouseInfoArgs e, bool resetCapture) {
			OnMouseDown(e);
			OnMouseUp(e);
			if(Capture && resetCapture) {
				Capture = false;
			}
		}
		internal void OnMouseMoveCore(MouseInfoArgs e) {
			OnMouseMove(e);
		}
		internal void OnMouseEnterCore(EventArgs e) {
			OnMouseEnter(e);
		}
		internal void OnMouseLeaveCore(EventArgs e) {
			OnMouseLeave(e);
		}
		GestureHelper gestureHelper;
		GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) gestureHelper = new GestureHelper(this);
				return gestureHelper;
			}
		}
		#region IGestureClient Members
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			RibbonHitInfo hitInfo = ViewInfo.CalcHitInfo(point);
			if(hitInfo.InGallery) {
				if(GetRibbonStyle() == RibbonControlStyle.MacOffice)
					return new GestureAllowArgs[] { GestureAllowArgs.Pan };
				else 
					return new GestureAllowArgs[] { GestureAllowArgs.PanVertical };
			}
			else if(hitInfo.InPanel && ViewInfo.Panel.ShowScrollButtons)
				return new GestureAllowArgs[] { GestureAllowArgs.Pan };
			return new GestureAllowArgs[] { };
		}
		IntPtr IGestureClient.Handle {
			get { return IsHandleCreated ? Handle : IntPtr.Zero; }
		}
		RibbonGalleryBarItemLink TouchGalleryLink { get { return !TouchInfo.InGallery ? null : (RibbonGalleryBarItemLink)TouchInfo.GalleryBarItemInfo.Item; } }
		RibbonHitInfo TouchInfo { get; set; }
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
			TouchInfo = ViewInfo.CalcHitInfo(info.Start.X, info.Start.Y);
		}
		int yOverPan;
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(TouchGalleryLink != null)
				ScrollGallery(info, delta, ref overPan);
			if(TouchInfo.InPanel && ViewInfo.Panel.ShowScrollButtons)
				ScrollPanel(info, delta, ref overPan);
		}
		void ScrollPanel(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin) {
				yOverPan = 0;
				return;
			}
			if(delta.X == 0) return;
			int prevTopY = ViewInfo.Panel.PanelScrollOffset;
			ViewInfo.Panel.SetDeltaPanelScroll(delta.X);
			if(prevTopY == ViewInfo.Panel.PanelScrollOffset) {
				yOverPan += delta.X;
			}
			else { yOverPan = 0; }
			overPan.X = yOverPan;
		}
		void ScrollGallery(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin) {
				yOverPan = 0;
				return;
			}
			int dt = GetRibbonStyle() == RibbonControlStyle.MacOffice ? delta.X : delta.Y;
			if(dt == 0) return;
			int prevTopY = TouchGalleryLink.ScrollYPositionCore;
			TouchGalleryLink.ScrollYPositionCore -= dt;
			if(prevTopY == TouchGalleryLink.ScrollYPositionCore) {
				yOverPan += dt;
			}
			else { yOverPan = 0; }
			if(GetRibbonStyle() == RibbonControlStyle.MacOffice)
				overPan.X = yOverPan;
			else
				overPan.Y = yOverPan;
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
		}
		IntPtr IGestureClient.OverPanWindowHandle {
			get { return GestureHelper.FindOverpanWindow(this); }
		}
		Point IGestureClient.PointToClient(Point p) {
			return PointToClient(p);
		}
		#endregion
		protected internal virtual void OnReduceOperationChanged() {
			Refresh();
		}
		#region FullScreenMode
		protected internal virtual void OnFullScreenModeBarClicked() {
			ViewInfo.EnterToPopupFullScreenMode();
			ShowMinimizedRibbon(false, new FullScreenRibbonPopupFactory(this, renderImage));
		}
		protected internal virtual void OnMinimizedRibbonPopupFadeInAnimationFinished() {
			CheckHeight();
		}
		protected internal virtual void OnMinimizedRibbonPopupFormDisposed() {
			CloseFullScreenModeRibbonPopup();
		}
		protected internal virtual void CloseFullScreenModeRibbonPopup() {
			if(!ViewInfo.IsPopupFullScreenModeActive) return;
			ViewInfo.RestoreFromPopupFullScreenMode();
			CheckHeight();
			ViewInfo.UpdateHotObject(false);
		}
		#endregion
		#region RibbonPopup Factory
		protected internal abstract class RibbonPopupFactory {
			RibbonControl ribbon;
			public RibbonPopupFactory(RibbonControl ribbon) {
				this.ribbon = ribbon;
			}
			public MinimizedRibbonPopupForm CreatePopup() {
				return CreatePopupCore();
			}
			protected RibbonControl Ribbon { get { return ribbon; } }
			protected abstract MinimizedRibbonPopupForm CreatePopupCore();
		}
		protected internal class DefaultRibbonPopupFactory : RibbonPopupFactory {
			public DefaultRibbonPopupFactory(RibbonControl ribbon)
				: base(ribbon) {
			}
			protected override MinimizedRibbonPopupForm CreatePopupCore() {
				return new MinimizedRibbonPopupForm(Ribbon);
			}
		}
		protected internal class FullScreenRibbonPopupFactory : RibbonPopupFactory {
			Image image;
			public FullScreenRibbonPopupFactory(RibbonControl ribbon, Image image) : base(ribbon) {
				this.image = image;
			}
			protected override MinimizedRibbonPopupForm CreatePopupCore() {
				return new FullScreenMinimizedRibbonPopupForm(Ribbon, image);
			}
		}
		#endregion
		#region IBackstageViewAnimationListener Members
		void IBackstageViewAnimationListener.OnAnimationStarted() {
		}
		void IBackstageViewAnimationListener.OnAnimationFinished() {
			ResetPressedObject();
		}
		#endregion
		protected void ResetPressedObject() {
			if(ViewInfo == null) return;
			ViewInfo.ResetPressedObject();
		}
		internal void UpdateSelectedPageWithoutViewInfo() {
			RibbonPage firstPage = TotalPageCategory.GetFirstVisiblePage();
			RibbonPageChangingEventArgs e = new RibbonPageChangingEventArgs(firstPage);
			RaiseSelectedPageChanging(e);
			if(!e.Cancel)
				this.selectedPage = e.Page;
		}
		void UpdateTouchUI() {
			if(!OptionsTouch.AffectOnlyRibbon)
				WindowsFormsSettings.TouchUIMode = OptionsTouch.TouchUI == DefaultBoolean.True ? TouchUIMode.True : TouchUIMode.False;
			OnControllerChanged();
			RibbonForm form = FindForm() as RibbonForm;
			if(form != null && form.IsHandleCreated) {
				form.PerformLayout();
				form.ForceStyleChanged();
			}
		}
		TouchUIMode? PrevTouchMode { get; set; }
		protected internal virtual void OnPropertiesChanged(RibbonOptionsBase ribbonOptionsBase, string propName) {
			if(propName == "TouchUI") {
				if(!PrevTouchMode.HasValue) {
					PrevTouchMode = WindowsFormsSettings.TouchUIMode;
				}
				UpdateTouchUI();   
			}
			else if(propName == "AffectOnlyRibbon") { 
				RibbonOptionsTouch opt = (RibbonOptionsTouch)ribbonOptionsBase;
				if(opt.AffectOnlyRibbon && PrevTouchMode.HasValue) {
					WindowsFormsSettings.TouchUIMode = PrevTouchMode.Value;
				}
				if(!opt.AffectOnlyRibbon && PrevTouchMode.HasValue) {
					PrevTouchMode = null;
					UpdateTouchUI();
				}
			}
			else
				Refresh();
		}
		bool allowInplaceLinks;
		[DefaultValue(false), XtraSerializableProperty]
		public bool AllowInplaceLinks {
			get { return allowInplaceLinks; }
			set {
				if(AllowInplaceLinks == value)
					return;
				allowInplaceLinks = value;
				Refresh();
			}
		}
		#region ISupportAdornerUIManager Members
		readonly static object updateVisualEffects = new object();
		event UpdateActionEventHandler ISupportAdornerUIManager.Changed {
			add { Events.AddHandler(updateVisualEffects, value); }
			remove { Events.RemoveHandler(updateVisualEffects, value); }
		}
		void ISupportAdornerUIManager.UpdateVisualEffects(UpdateAction action) { UpdateVisualEffects(action); }
		protected void UpdateVisualEffects(UpdateAction action) {
			UpdateActionEventHandler handler = Events[updateVisualEffects] as UpdateActionEventHandler;
			if(handler == null) return;
			handler(this, new UpdateActionEvendArgs(action));
		}
		#endregion
	}
	[Flags]
	public enum RibbonItemStyles { Default = 0, Large = 1, SmallWithText = 2, SmallWithoutText = 4, All = Large | SmallWithText | SmallWithoutText }
	public interface IRibbonItem {
		DefaultBoolean AllowHtmlText { get; }
		bool IsAllowHtmlText { get; }
		bool Enabled { get; }
		string Text { get; }
		bool IsButtonGroup { get; }
		bool IsLargeButton { get; }
		RibbonItemStyles AllowedStyles { get; }
		bool BeginGroup { get; set; } 
		IRibbonItem[] GetChildren();
		bool IsChecked { get; }
		bool IsDroppedDown { get; }
		RepositoryItem Edit { get; }
		RibbonItemViewInfo GetCachedViewInfo();
		void OnViewInfoCreated(RibbonItemViewInfo viewInfo);
	}
	public class RibbonCommandContext : BarCommandContextBase {
		public RibbonCommandContext(RibbonControl ribbonControl)
			: base(ribbonControl) {
		}
		protected override void SetOwner(BarItem barItem, string category) {
			RibbonPageGroup group = GetOwner(category);
			if(group == null) {
				group = CreateOwner(category);
			}
			group.ItemLinks.Add(barItem);
			barItem.Manager = Ribbon.Manager;
		}
		protected RibbonPageGroup GetOwner(string category) {
			foreach(RibbonPage page in Ribbon.DefaultPageCategory.Pages) {
				RibbonPageGroup group = page.Groups.GetGroupByText(category);
				if(group != null) return group;
			}
			return null;
		}
		protected RibbonPageGroup CreateOwner(string category) {
			RibbonPageGroup group = (RibbonPageGroup)DesignerHost.CreateComponent(typeof(RibbonPageGroup));
			group.Text = category;
			Ribbon.Container.Add(group);
			RibbonPage page = Ribbon.DefaultPageCategory.Pages.FirstVisiblePage;
			if(page == null) {
				page = (RibbonPage)DesignerHost.CreateComponent(typeof(RibbonPage));
				page.Text = category;
				Ribbon.Container.Add(group);
				Ribbon.DefaultPageCategory.Pages.Add(page);
			}
			page.Groups.Add(group);
			return group;
		}
		public RibbonControl Ribbon { get { return Component as RibbonControl; } }
	}
	[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraBars.Design.Serialization.RibbonItemLinksSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class RibbonPageGroupItemLinkCollection : BaseRibbonItemLinkCollection, IHasRibbonKeyTipManager, BarLinksHolder {
		RibbonPageGroup pageGroup;
		public RibbonPageGroupItemLinkCollection(RibbonPageGroup pageGroup) {
			this.pageGroup = pageGroup;
		}
		protected internal override object Owner { get { return this; } }
		public RibbonPageGroup PageGroup { get { return pageGroup; } }
		public override RibbonControl Ribbon { get { return PageGroup.Ribbon; } }
		protected override void OnCollectionChanged(CollectionChangeEventArgs e) {
			base.OnCollectionChanged(e);
			PageGroup.OnLinksChanged(e);
		}
		RibbonBaseKeyTipManager IHasRibbonKeyTipManager.KeyTipManager { get { return Ribbon.KeyTipManager; } }
		protected internal override bool IsMergedState {
			get {
				if(Ribbon != null && (Ribbon.IsMerged || Ribbon.MergedRibbon != null) && PageGroup.Page != null && PageGroup.Page.MergedGroups.Contains(PageGroup))
					return true;
				return base.IsMergedState;
			}
		}
		bool BarLinksHolder.Enabled { get { return PageGroup.Enabled; } }
		protected override ISupportAdornerUIManager GetVisualEffectsOwner() {
			ISupportAdornerElement el = PageGroup as ISupportAdornerElement;
			if(el == null) return null;
			return el.Owner;
		}
		protected override bool GetVisulEffectsVisible() {
			ISupportAdornerElement el = PageGroup as ISupportAdornerElement;
			if(el == null) return false;
			return el.IsVisible;
		}
	}
	public class RibbonMiniToolbarCollection : CollectionBase {
		RibbonControl ribbon;
		public RibbonMiniToolbarCollection(RibbonControl ribbon) {
			this.ribbon = ribbon;
		}
		public RibbonControl Ribbon { get { return ribbon; } }
		public int Add(RibbonMiniToolbar miniToolbar) {
			if(Contains(miniToolbar)) return IndexOf(miniToolbar);
			return List.Add(miniToolbar); 
		}
		public bool Contains(RibbonMiniToolbar miniToolbar) { return List.Contains(miniToolbar); }
		public void Insert(int index, RibbonMiniToolbar miniToolbar) {
			if(!Contains(miniToolbar))
				List.Insert(index, miniToolbar); 
		}
		public int IndexOf(RibbonMiniToolbar miniToolbar) { return List.IndexOf(miniToolbar); }
		public void Remove(RibbonMiniToolbar miniToolbar) {
			if(Contains(miniToolbar))
				List.Remove(miniToolbar); 
		}
		public RibbonMiniToolbar this[int index] { get { return (RibbonMiniToolbar)List[index]; } set { List[index] = value; } }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			RibbonMiniToolbar toolbar = value as RibbonMiniToolbar;
			if(toolbar != null)
				toolbar.Collection = this;
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			RibbonMiniToolbar toolbar = value as RibbonMiniToolbar;
			if(toolbar != null)
				toolbar.Collection = null;
		}
	}
	public enum RibbonPageGroupItemsLayout { Default, TwoRows, ThreeRows }
	[
	DXToolboxItem(false), DesignTimeVisible(false),
	Designer("DevExpress.XtraBars.Ribbon.Design.RibbonPageGroupDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	SmartTagSupport(typeof(RibbonPageGroupDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	]
	public class RibbonPageGroup : BaseRibbonComponent, ISupportRibbonKeyTip, ISupportMergeOrder, ICloneable {
		private static readonly object captionButtonClick = new object();
		Image glyph;
		int imageIndex;
		RibbonPageGroupItemLinkCollection itemLinks;
		RibbonPageGroup mergedGroup;
		RibbonPage page;
		string text;
		object tag;
		bool visible;
		bool showCaptionButton = true;
		bool destroying = false;
		bool allowMinimize = true;
		bool allowTextClipping = true;
		SuperToolTip superTip;
		string userKeyTip = string.Empty;
		string itemKeyTip = string.Empty;
		int firstIndex = 0;
		int mergeOrder;
		RibbonToolbarPopupItem toolbarContentButton = null;
		RibbonToolbarPopupItemLink toolbarContentButtonLink = null;
		BarButtonItem contentButton;
		BarButtonItemLink contentButtonLink;
		RibbonPageGroupItemsLayout itemsLayout = RibbonPageGroupItemsLayout.Default;
		public RibbonPageGroup(string text) : this() {
			this.text = text;
			this.userKeyTip = string.Empty;
			this.itemKeyTip = string.Empty;
			this.firstIndex = 0;
		}
		public RibbonPageGroup() {
			this.imageIndex = -1;
			this.visible = true;
			this.text = string.Empty;
			this.itemLinks = new RibbonPageGroupItemLinkCollection(this);
			this.mergeOrder = -1;
		}
		public override string ToString() {
			return string.IsNullOrEmpty(Text) ? Name : Text;
		}
		protected internal BarButtonItem ContentButton {
			get {
				if(contentButton == null) contentButton = CreateContentButton();
				return contentButton;
			}
		}
		protected internal BarButtonItemLink ContentButtonLink {
			get {
				if(contentButtonLink == null) contentButtonLink = (BarButtonItemLink)ContentButton.CreateLink(null, ContentButton);
				return contentButtonLink;
			}
		}
		protected internal RibbonGroupItem CreateContentButton() {
			RibbonGroupItem item = new RibbonGroupItem(this);
			return item;
		}
		[DefaultValue(RibbonPageGroupItemsLayout.Default), XtraSerializableProperty]
		public RibbonPageGroupItemsLayout ItemsLayout {
			get { return itemsLayout; }
			set {
				if(ItemsLayout == value)
					return;
				itemsLayout = value;
				OnChanged();
			}
		}
		#region Merging
		protected internal virtual RibbonPageGroup MergedGroup { get { return mergedGroup; } }
		protected internal virtual void MergeGroup(RibbonPageGroup group) {
			UnMergeGroup();
			this.mergedGroup = group;
			ItemLinks.Merge(MergedGroup.ItemLinks);
		}
		protected internal virtual void UnMergeGroup() {
			this.mergedGroup = null;
			ItemLinks.UnMerge();
		}
		#endregion
		RibbonPageGroup originalPageGroup = null;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonPageGroup OriginalPageGroup { 
			get {
				if(originalPageGroup == null || originalPageGroup.OriginalPageGroup == null) return originalPageGroup;
				return originalPageGroup.OriginalPageGroup;
			}
			set { originalPageGroup = value; }
		}
		internal RibbonPageGroup GetOriginalPageGroup() {
			if(OriginalPageGroup == null) return this;
			return OriginalPageGroup;
		}
		protected internal RibbonPageGroup Reference { get; set; }
		public virtual void Assign(RibbonPageGroup group) {
			this.text = group.Text;
			if(Ribbon == null || !Ribbon.IsDesignMode)
				this.name = group.Name;
			this.imageIndex = group.ImageIndex;
			this.glyph = group.Glyph;
			this.superTip = group.superTip;
			this.allowTextClipping = group.AllowTextClipping;
			this.showCaptionButton = group.ShowCaptionButton;
			this.Events.AddHandler(captionButtonClick, group.Events[captionButtonClick]);
			this.OriginalPageGroup = group;
			this.Visible = group.Visible;
			this.Enabled = group.Enabled;
			this.KeyTip = group.KeyTip;
			this.AllowMinimize = group.AllowMinimize;
			this.Tag = group.Tag;
			this.ItemLinks.SetMergeSource(group.ItemLinks.MergeSource);
			this.ItemLinks.SetIsMergedState(group.ItemLinks.IsMergedState);
			this.mergeOrder = group.mergeOrder;
			this.itemsLayout = group.ItemsLayout;
			foreach(BarItemLink link in group.ItemLinks) {
				BarItemLink clonedLink = this.ItemLinks.Add(link.Item, link.BeginGroup);
				clonedLink.UserDefine = link.UserDefine;
				clonedLink.UserCaption = link.UserCaption;
				clonedLink.UserRibbonStyle = link.UserRibbonStyle;
				clonedLink.UserAlignment = link.UserAlignment;
				clonedLink.UserGlyph = link.UserGlyph;
				clonedLink.UserPaintStyle = link.UserPaintStyle;
				clonedLink.UserWidth = link.UserWidth;
				clonedLink.AssignKeyTip(link);
				clonedLink.ActAsButtonGroup = link.ActAsButtonGroup;
				clonedLink.Visible = link.Visible;
				RibbonGalleryBarItemLink galleryLink = clonedLink as RibbonGalleryBarItemLink;
				if(galleryLink != null) { galleryLink.OriginalLink = link as RibbonGalleryBarItemLink; }
			}
			this.mergedGroup = group.MergedGroup;
		}
		protected virtual RibbonPageGroup CreateGroup() { return new RibbonPageGroup(); }
		public object Clone() {
			return Clone(false);
		}
		protected internal object Clone(bool addReference) {
			RibbonPageGroup res = CreateGroup();
			res.Assign(this);
			if(addReference) {
				this.Reference = res;
				res.Reference = this;
			}
			return res;
		}
		internal bool Destroing { get { return destroying; } }
		protected override void Dispose(bool disposing) {
			if(destroying) return;
			destroying = true;
			if(disposing) {
				UpdateVisualEffects(UpdateAction.Dispose);
				DestroyLinks();
				if(Page != null) Page.Groups.Remove(this);
				if(this.contentButtonLink != null)
					this.contentButtonLink.Dispose();
				if(this.contentButton != null)
					this.contentButton.Dispose();
			}
			base.Dispose(disposing);
		}
		void DestroyLinks() {
			ItemLinks.Clear();
		}
		protected internal RibbonToolbarPopupItem ToolbarContentButton {
			get {
				if(toolbarContentButton == null) toolbarContentButton = CreateToolbarContentButton();
				return toolbarContentButton;
			}
		}
		bool enabled = true;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupEnabled"),
#endif
 DefaultValue(true), XtraSerializableProperty]
		public virtual bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value) return;
				enabled = value;
				OnEnabledChanged();
			}
		}
		protected virtual void OnEnabledChanged() {
			if(Reference != null)
				Reference.Enabled = Enabled;
			OnChanged();
		}
	  	[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupMergeOrder"),
#endif
 DefaultValue(-1), Category("Behavior"), XtraSerializableProperty]
		public virtual int MergeOrder {
			get { return mergeOrder; }
			set {
				if(MergeOrder == value) return;
				mergeOrder = value;
				OnMergeOrderChanged();
			}
		}
		protected virtual void OnMergeOrderChanged() {
			if(Reference != null)
				Reference.MergeOrder = MergeOrder;
			OnChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonToolbarPopupItemLink ToolbarContentButtonLink {
			get {
				if(toolbarContentButtonLink == null) toolbarContentButtonLink = CreateToolbarContentButtonLink();
				return toolbarContentButtonLink;
			}
		}
		protected virtual RibbonToolbarPopupItem CreateToolbarContentButton() {
			RibbonToolbarPopupItem item = new RibbonToolbarPopupItem(Manager, true);
			item.Tag = this;
			item.PageGroup = this;
			item.ItemClick += new ItemClickEventHandler(OnToolbarContentButtonClick);
			return item;
		}
		protected virtual RibbonToolbarPopupItemLink CreateToolbarContentButtonLink() {
			RibbonToolbarPopupItemLink link = ToolbarContentButton.CreateLink(null, ToolbarContentButton) as RibbonToolbarPopupItemLink;
			link.PageGroup = this;
			return link;
		}
		public virtual void AddGroupToToolbar() {
			if(Ribbon.Toolbar.ItemLinks.Contains(ToolbarContentButtonLink)) return;
			Ribbon.Toolbar.ItemLinks.Add(ToolbarContentButtonLink);
		}
		public virtual void RemoveGroupFromToolbar() {
			Ribbon.Toolbar.ItemLinks.Remove(ToolbarContentButtonLink);
		}
		protected virtual void OnToolbarContentButtonClick(object sender, ItemClickEventArgs e) {
			ShowContentDropDownCore(PopupGroupInfo, e.Link as RibbonToolbarPopupItemLink);
		}
		RibbonPageGroupViewInfo popupGroupInfo;
		protected internal RibbonPageGroupViewInfo PopupGroupInfo {
			get {
				if(popupGroupInfo == null) {
					if(Ribbon == null || Ribbon.ViewInfo == null) return null;
					popupGroupInfo = new RibbonPageGroupViewInfo(Ribbon.ViewInfo, this);
				}
				return popupGroupInfo;
			}
		}
		internal virtual RibbonControl GetToolbarLinkRibbonControl(RibbonToolbarPopupItemLink toolbarLink) { 
			if(toolbarLink != null && toolbarLink.RibbonItemInfo != null) 
				return toolbarLink.RibbonItemInfo.ViewInfo.OwnerControl as RibbonControl;
			return Ribbon;
		}
		internal virtual RibbonMinimizedGroupPopupForm CreateGroupPopupForm(RibbonPageGroupViewInfo groupInfo, RibbonToolbarPopupItemLink toolbarLink) {
			RibbonControl r = GetToolbarLinkRibbonControl(toolbarLink);
			RibbonMinimizedGroupPopupForm form = new RibbonMinimizedGroupPopupForm(r);
			form.GroupInfo = toolbarLink != null ? null : groupInfo;
			form.GroupToolbarLink = toolbarLink;
			return form;
		}
		protected internal virtual void HideContentDropDown() {
			if(Ribbon != null && Ribbon.PopupGroupForm != null) {
				Ribbon.PopupGroupForm.Hide();
				if(Ribbon.PopupGroupForm.Control.IsKeyboardActive) Ribbon.ActivateKeyboardNavigation();
				Ribbon.ViewInfo.Invalidate(this);
			}
		}
		protected internal virtual void ShowContentDropDownCore(RibbonPageGroupViewInfo groupInfo, RibbonToolbarPopupItemLink toolbarLink) {
			if(Ribbon.PopupGroupForm != null && Ribbon.PopupGroupForm.Visible) {
				Ribbon.PopupGroupForm = null;
				return;
			}
			Ribbon.PopupGroupForm = CreateGroupPopupForm(groupInfo, toolbarLink);
			XtraForm.SuppressDeactivation = true;
			try {
				Ribbon.PopupGroupForm.ShowPopup();
				if(Ribbon.IsKeyboardActive) {
					Ribbon.PopupGroupForm.Control.ActivateKeyboardNavigation();
				}
				Ribbon.ViewInfo.Invalidate(this);
			}
			finally {
				XtraForm.SuppressDeactivation = false;
			}
			Ribbon.ViewInfo.Invalidate(this);
		}
		internal virtual bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupSuperTip"),
#endif
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)),
		Localizable(true),
		SmartTagProperty("Super Tip", "Appearance", 10),
		SkipRuntimeSerialization
		]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set {
				if(SuperTip == value)
					return;
				superTip = value;
				OnSuperTipChanged();
			}
		}
		protected virtual void OnSuperTipChanged() {
			if(Reference != null)
				Reference.SuperTip = SuperTip;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupAllowMinimize"),
#endif
 DefaultValue(true), Category("Behavior"), XtraSerializableProperty]
		public virtual bool AllowMinimize {
			get { return allowMinimize; }
			set {
				if(AllowMinimize == value) return;
				allowMinimize = value;
				OnAllowMinimizeChanged();
			}
		}
		protected virtual void OnAllowMinimizeChanged() {
			if(Reference != null)
				Reference.AllowMinimize = AllowMinimize;
			OnChanged();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupAllowTextClipping"),
#endif
 DefaultValue(true), Category("Behavior"), SmartTagProperty("Allow Text Clipping", "Appearance", 15, SmartTagActionType.RefreshAfterExecute), XtraSerializableProperty]
		public virtual bool AllowTextClipping {
			get { return allowTextClipping; }
			set {
				if(AllowTextClipping == value) return;
				allowTextClipping = value;
				OnAllowTextClippingChanged();
			}
		}
		protected virtual void OnAllowTextClippingChanged() {
			if(Reference != null)
				Reference.AllowTextClipping = AllowTextClipping;
			OnChanged();
		}
		protected internal RibbonBarManager Manager {
			get {
				if(Page != null && Page.Ribbon != null) return Page.Ribbon.Manager;
				return null;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupImageIndex"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("Images"), SmartTagProperty("Image Index", "Image", 10, SmartTagActionType.RefreshAfterExecute), XtraSerializableProperty]
		public virtual int ImageIndex {
			get { return imageIndex; }
			set {
				if(value < 0) value = -1;
				if(value == ImageIndex) return;
				imageIndex = value;
				OnImageIndexChanged();
			}
		}
		protected virtual void OnImageIndexChanged() {
			if(Reference != null)
				Reference.ImageIndex = ImageIndex;
			OnChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Images { get { return Manager == null ? null : Manager.Images; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupGlyph"),
#endif
 DefaultValue(null), Category("Appearance"), SmartTagProperty("Glyph", "Image", 0, SmartTagActionType.RefreshAfterExecute), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Glyph {
			get { return glyph; }
			set {
				if(value == Glyph) return;
				glyph = value;
				OnGlyphChanged();
			}
		}
		protected virtual void OnGlyphChanged() {
			if(Reference != null)
				Reference.Glyph = Glyph;
			OnChanged();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupVisible"),
#endif
 DefaultValue(true), Category("Behavior"), XtraSerializableProperty]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnVisibleChanged();
			}
		}
		protected virtual void OnVisibleChanged() {
			if(Reference != null)
				Reference.Visible = Visible;
			if(Ribbon != null)
				Ribbon.Manager.SelectionInfo.CloseEditor();
			OnChanged();
		}
		internal bool InternalVisible { get; set; }
		internal bool ActualVisible {
			get {
				if(Ribbon != null && !Ribbon.IsDesignMode)
					return Ribbon.AutoHideEmptyItems ? InternalVisible && Visible : Visible;
				return Visible;
			}
		}
		#region Name property
		string name = string.Empty;
		[Browsable(false), DefaultValue(""), SkipRuntimeSerialization]
		public virtual string Name {
			get {
				if(this.Site != null) name = this.Site.Name;
				return name;
			}
			set {
				if(value == null) value = string.Empty;
				name = value;
				if(Site != null) Site.Name = name;
			}
		}
		#endregion
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupShowCaptionButton"),
#endif
 DefaultValue(true), Category("Appearance"), SmartTagProperty("Show Caption Button", "Appearance", 20, SmartTagActionType.RefreshAfterExecute), XtraSerializableProperty]
		public virtual bool ShowCaptionButton {
			get { return showCaptionButton; }
			set {
				if(ShowCaptionButton == value) return;
				showCaptionButton = value;
				OnShowCaptionButtonChanged();
			}
		}
		protected virtual void OnShowCaptionButtonChanged() {
			if(Reference != null)
				Reference.ShowCaptionButton = ShowCaptionButton;
			OnChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RibbonControl Ribbon { get { return Page == null ? null : Page.Ribbon; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DevExpress.Utils.Design.InheritableCollection, XtraSerializableProperty(false, true, false)]
		public RibbonPageGroupItemLinkCollection ItemLinks { get { return itemLinks; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set {
				if(Tag == value)
					return;
				tag = value;
				OnTagChanged();
			}
		}
		protected virtual void OnTagChanged() {
			if(Reference != null)
				Reference.Tag = Tag;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupText"),
#endif
 Category("Appearance"), Localizable(true), SmartTagProperty("Text", "Appearance", 0, SmartTagActionType.RefreshAfterExecute), XtraSerializableProperty]
		public virtual string Text {
			get { return text; }
			set {
				if(value == null) value = string.Empty;
				if(Text == value) return;
				text = value;
				OnTextChanged();
			}
		}
		protected virtual void OnTextChanged() {
			if(Reference != null)
				Reference.Text = Text;
			OnChanged();
		}
		protected internal virtual bool ShouldSerializeText() {
			return !String.IsNullOrEmpty(Text);
		}
		protected internal virtual void ResetText() {
			Text = String.Empty;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RibbonPage Page { get { return page; } }
		protected internal virtual void SetPage(RibbonPage page) { this.page = page; }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonPageGroupCaptionButtonClick")]
#endif
		public virtual event RibbonPageGroupEventHandler CaptionButtonClick {
			add { Events.AddHandler(captionButtonClick, value); }
			remove { Events.RemoveHandler(captionButtonClick, value); }
		}
		protected internal void RaiseCaptionButtonClick(RibbonPageGroupEventArgs e) {
			RibbonPageGroupEventHandler handler = (RibbonPageGroupEventHandler)Events[captionButtonClick];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnChanged() {
			if(destroying) return;
			if(Ribbon != null) Ribbon.OnGroupChanged(this);
		}
		static RibbonPageGroup nullGroup;
		internal static RibbonPageGroup NullGroup {
			get {
				if(nullGroup == null) nullGroup = new RibbonPageGroup();
				return nullGroup;
			}
		}
		protected internal virtual void OnLinksChanged(CollectionChangeEventArgs e) {
			OnChanged();
		}
		string ISupportRibbonKeyTip.ItemCaption { get { return Text; } }
		string ISupportRibbonKeyTip.ItemKeyTip { get { return itemKeyTip; } set { itemKeyTip = value; } }
		string ISupportRibbonKeyTip.ItemUserKeyTip { 
			get { return userKeyTip; } 
			set {
				if(value != null) 
					value = value.ToUpper();
				else value = string.Empty;
				if(((ISupportRibbonKeyTip)this).ItemUserKeyTip == value)
					return;
				userKeyTip = value;
				OnUserKeyTipChanged();
			} 
		}
		protected virtual void OnUserKeyTipChanged() {
			if(Reference != null)
				Reference.KeyTip = KeyTip;
		}
		int ISupportRibbonKeyTip.FirstIndex { get { return firstIndex; } set { firstIndex = value; } }
		void ISupportRibbonKeyTip.Click() {
			Ribbon.DeactivateKeyboardNavigation();
			Ribbon.RaisePageGroupCaptionButtonClick(new RibbonPageGroupEventArgs(this));
		}
		bool ISupportRibbonKeyTip.KeyTipEnabled { get { return true; } }
		bool ISupportRibbonKeyTip.KeyTipVisible { get { return GroupInfo.ViewInfo.Bounds.Contains(GroupInfo.Bounds); } }
		protected internal RibbonPageGroupViewInfo GroupInfo {
			get {
				for(int i = 0; i < Ribbon.ViewInfo.Panel.Groups.Count; i++)
					if(this == Ribbon.ViewInfo.Panel.Groups[i].PageGroup) return Ribbon.ViewInfo.Panel.Groups[i];
				return null;
			}
		}
		bool ISupportRibbonKeyTip.HasDropDownButton { get { return false; } }
		Point ISupportRibbonKeyTip.ShowPoint { 
			get {
				RibbonPageGroupViewInfo groupInfo = GroupInfo;
				if(groupInfo == null) return Point.Empty;
				return Ribbon.PointToScreen(new Point(groupInfo.ButtonBounds.X + groupInfo.ButtonBounds.Width / 2, groupInfo.Bounds.Bottom));
			}
		}
		bool ISupportRibbonKeyTip.IsCommandItem { get { return false; } }
		ContentAlignment ISupportRibbonKeyTip.Alignment { get { return ContentAlignment.TopCenter; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageGroupKeyTip"),
#endif
 Category("Appearance"), DefaultValue(""), XtraSerializableProperty]
		public string KeyTip { 
			get { return (this as ISupportRibbonKeyTip).ItemUserKeyTip; } 
			set { (this as ISupportRibbonKeyTip).ItemUserKeyTip = value; } 
		}
		internal void ClearReference() {
			if(Reference != null) {
				Reference.Reference = null;
				Reference = null;
			}
		}
		#region ISupportAdornerElement Members
		protected override Rectangle GetVisualEffectBounds() { return GroupInfo != null ? GroupInfo.Bounds : Rectangle.Empty; }
		protected override bool GetVisualEffectsVisible() {
			if(!base.GetVisualEffectsVisible()) return false;
			if(Page == null) return false;
			if(Page.Category == null) return false;
			if(Page != Ribbon.SelectedPage) return false;
			return ActualVisible && Page.ActualVisible && Page.Category.ActualVisible;
		}				
		#endregion
	}
	[ListBindable(false)]
	public class RibbonPageCategoryCollection : CollectionBase, IList {
		RibbonPageCategory defaultCategory;
		RibbonTotalPageCategory totalCategory;
		RibbonControl ribbon;
		public RibbonPageCategoryCollection(RibbonControl ribbon) {
			this.ribbon = ribbon;
			this.defaultCategory = new RibbonPageCategory(BarLocalizer.Active.GetLocalizedString(BarString.RibbonUnassignedPages), Color.Empty);
			this.defaultCategory.SetCollection(this);
			this.totalCategory = new RibbonTotalPageCategory(this);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryCollectionRibbon")]
#endif
public RibbonControl Ribbon { get { return ribbon; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryCollectionItem")]
#endif
		public virtual RibbonPageCategory this[int index] { get { return List[index] as RibbonPageCategory; } }
		public RibbonPageCategory this[string text] { get { return GetCategoryByText(text); } }
		public RibbonPageCategory GetCategoryByName(string name) {
			for(int i = 0; i < Count; i++) {
				if(name == this[i].Name) return this[i];
			}
			return null;
		}
		public RibbonPageCategory GetCategoryByText(string text) {
			for(int i = 0; i < Count; i++) {
				if(text == this[i].Text) return this[i];
			}
			return null;
		}
		public virtual int Add(RibbonPageCategory category) {
			return List.Add(category);
		}
		public virtual void Remove(RibbonPageCategory category) {
			if(List.Contains(category)) List.Remove(category);
		}
		public virtual void AddRange(RibbonPageCategory[] categories) {
			foreach(RibbonPageCategory category in categories) { Add(category); }
		}
		protected override void OnInsert(int index, object value) {
			base.OnInsert(index, value);
			RibbonPageCategory category = value as RibbonPageCategory;
			if(category == null) return;
			if(category.Collection != null) 
				category.Collection.Remove(category);
			category.SetCollection(this);
		}
		public virtual void Insert(int index, RibbonPageCategory category) {
			List.Insert(index, category);
		}
		public virtual bool Contains(RibbonPageCategory category) { return List.Contains(category); }
		public virtual int IndexOf(RibbonPageCategory category) { return List.IndexOf(category); }
		protected override void OnInsertComplete(int index, object value) {
			RibbonPageCategory category = (RibbonPageCategory)value;
			base.OnInsertComplete(index, value);
			category.Disposed += new EventHandler(category_Disposed);
			if(Ribbon != null && ShouldRaiseRibbonEvents) Ribbon.OnPageCategoryAdded(category);
		}
		void category_Disposed(object sender, EventArgs e) {
			if(List.Contains(sender)) List.Remove(sender);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			RibbonPageCategory category = (RibbonPageCategory)value;
			category.SetCollection(null);
			category.Disposed -= new EventHandler(category_Disposed);
			if(Ribbon != null && ShouldRaiseRibbonEvents) Ribbon.OnPageCategoryRemoved(category);
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
			if(Ribbon != null && ShouldRaiseRibbonEvents) Ribbon.FireRibbonChanged();
		}
		internal void Destroy() {
			ArrayList list = new ArrayList(List);
			InnerList.Clear();
			foreach(RibbonPageCategory category in list) category.Dispose();
		}
		protected internal RibbonPageCategory[] GetPageCategories() {
			return GetPageCategories(null);
		}
		protected internal RibbonPageCategory[] GetPageCategories(RibbonPageCategoryCollection merged) {
			int mergedCount = merged != null ? merged.Count : 0;
			RibbonPageCategory[] res = new RibbonPageCategory[Count + 1 + mergedCount];
			res[0] = DefaultCategory;
			for(int i = 0; i < Count; i++) res[i + 1] = this[i];
			if(merged == null) return res;
			for(int i = 0; i < merged.Count; i++) res[i + 1 + Count] = merged[i];
			return res;
		}
		protected internal void FillPageCategoriesList(RibbonPageCategory[] list, int cnt) {
			list[0] = DefaultCategory;
			cnt = cnt > Count + 1 ? Count + 1: cnt;
			for(int i = 0; i < cnt - 1; i++) list[i + 1] = this[i];
		}
		[Browsable(false)]
		public RibbonPageCategory DefaultCategory { get { return defaultCategory; } }
		[Browsable(false)]
		public RibbonTotalPageCategory TotalCategory { get { return totalCategory; } }
		#region IList
		int IList.Add(object value) {
			if(InnerList.Contains(value)) 
				return InnerList.IndexOf(value);
			this.OnValidate(value);
			this.OnInsert(this.InnerList.Count, value);
			int index = this.InnerList.Add(value);
			try {
				this.OnInsertComplete(index, value);
			}
			catch {
				this.InnerList.RemoveAt(index);
				throw;
			}
			return index;
		}
		void IList.Insert(int index, object value) {
			if(InnerList.Contains(value)) return;
			index = Math.Max(0, Math.Min(Count, index));
			this.OnValidate(value);
			this.OnInsert(index, value);
			this.InnerList.Insert(index, value);
			try {
				this.OnInsertComplete(index, value);
			}
			catch {
				this.InnerList.RemoveAt(index);
				throw;
			}
		}
		#endregion
		protected virtual bool ShouldRaiseRibbonEvents { get { return true; } }
	}
	public enum RibbonPageCategoryAlignment { Default, Left, Right }
	public interface ISupportMergeOrder {
		int MergeOrder { get; }
	}
	public interface IBackstageViewAnimationListener {
		void OnAnimationStarted();
		void OnAnimationFinished();
	}
	[
	ToolboxItem(false), DesignTimeVisible(false),
	Designer("DevExpress.XtraBars.Ribbon.Design.RibbonPageCategoryDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	SmartTagSupport(typeof(RibbonPageCategoryDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	SmartTagAction(typeof(RibbonPageCategoryDesignTimeActionsProvider), "AddPage", "Add Page", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(RibbonPageCategoryDesignTimeActionsProvider), "AddPageCategory", "Add PageCategory", SmartTagActionType.CloseAfterExecute),
	]
	public class RibbonPageCategory : BaseRibbonComponent, ISupportMergeOrder, ICloneable, IXtraObjectWithBounds {
		bool visible;
		string text;
		Color color;
		RibbonPageCategoryCollection collection;
		RibbonPageCollection pages;
		RibbonPageCollection mergedPages;
		RibbonPageCategory mergedCategory;
		object tag;
		int mergeOrder;
		bool expanded;
		bool autoStretchPageHeaders;
		public RibbonPageCategory() {
			this.color = Color.FromArgb(255, 231, 193, 253);
			this.text = string.Empty;
			this.pages = new RibbonPageCollection(null, this);
			this.visible = true;
			this.collection = null;
			this.mergeOrder = -1;
			this.expanded = true;
			this.autoStretchPageHeaders = false;
		}
		public RibbonPageCategory(string text, Color color) : this() {
			this.text = text;
			this.color = color;
		}
		public RibbonPageCategory(string text, Color color, bool visible)
			: this(text, color) {
			this.visible = visible;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				UpdateVisualEffects(UpdateAction.Dispose);
				DestroyPages();
				ColoredRibbonElementsCache.RemoveColoredImages(Color);
				if(Ribbon != null) Ribbon.PageCategories.Remove(this);
			}
		}
		protected void DestroyPages() {
			Pages.Destroy();
		}
		protected virtual RibbonPageCategory CreateRibbonPageCategory() {
			return new RibbonPageCategory();
		}
		public object Clone() {
			return Clone(false);
		}
		protected internal object Clone(bool addReference) {
			RibbonPageCategory res = CreateRibbonPageCategory();
			res.Name = Name;
			res.visible = visible;
			res.text = text;
			res.color = color;
			res.collection = collection;
			res.mergedCategory = mergedCategory;
			res.mergeOrder = mergeOrder;
			res.autoStretchPageHeaders = autoStretchPageHeaders;
			foreach(RibbonPage page in Pages) {
				res.Pages.Add((RibbonPage)page.Clone(addReference));
			}
			if(MergedPages == null) return res;
			foreach(RibbonPage page in MergedPages) {
				res.MergedPages.Add((RibbonPage)page.Clone(addReference));
			}
			if(addReference) {
				this.Reference = res;
				res.Reference = this;
			}
			return res;
		}
		protected internal RibbonPageCategory Reference { get; set; }
		#region Merging
		protected internal RibbonPageCategory MergedCategory { get { return mergedCategory; } }
		RibbonPage FindVisiblePage(string text) {
			foreach(RibbonPage page in Pages) {
				if(page.Text == text && page.Visible)
					return page;
			}
			return null;
		}
		protected internal virtual void MergePage(RibbonPage mergedPage) {
			RibbonPage page = RibbonControl.AllowMergeInvisibleItems? Pages[mergedPage.Text] : FindVisiblePage(mergedPage.Text);
			if(page == null) {
				MergedPages.Add((RibbonPage)mergedPage.Clone(true));
			}
			else page.MergePage(mergedPage);
		}
		protected virtual void MergePageCollection(RibbonPageCollection coll) {
			if(coll == null) return ;
			foreach(RibbonPage mergedPage in coll) {
				MergePage(mergedPage);
			}
		}
		protected internal virtual void MergeCategory(RibbonPageCategory category) {
			UnMergeCategory();
			this.mergedCategory = category;
			MergedPages.Clear();
			MergePageCollection(MergedCategory.Pages);
			MergePageCollection(MergedCategory.MergedPages);
		}
		protected internal virtual void UnMergeCategory() {
			if(MergedCategory == null) return;
			foreach(RibbonPage page in Pages){
				page.UnMergePage();
			}
			for(int i = MergedPages.Count - 1; i >= 0; i--) {
				RibbonPage mergedPage = MergedPages[i];
				if(mergedPage.Reference != null)
					mergedPage.Reference.Reference = null;
				mergedPage.Reference = null;
				for(int j = mergedPage.Groups.Count - 1; j >= 0; j--) {
					RibbonPageGroup mergedGroup = mergedPage.Groups[j];
					if(mergedGroup.Reference != null)
						mergedGroup.Reference.Reference = null;
					mergedGroup.Reference = null;
				}
				MergedPages[i].Dispose();
			}
			MergedPages.Clear();
			mergedCategory = null;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryMergedPages"),
#endif
 Browsable(false)]
		public virtual RibbonPageCollection MergedPages {
			get {
				if(mergedPages == null) mergedPages = new RibbonPageCollection(Ribbon, this);
				return mergedPages;
			}
		}
		#endregion
		[Browsable(false)]
		public RibbonPageCategoryCollection Collection { get { return collection; } }
		internal void SetCollection(RibbonPageCategoryCollection coll) { 
			this.collection = coll;
			SetRibbon();
		}
		protected virtual void SetRibbon() { Pages.SetRibbon(Ribbon); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryExpanded"),
#endif
 DefaultValue(true), Category("Behavior"), XtraSerializableProperty]
		public bool Expanded {
			get { return expanded; }
			set {
				if(Expanded == value)
					return;
				expanded = value;
				OnExpandedChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryAutoStretchPageHeaders"),
#endif
 DefaultValue(false), Category("Behavior"), XtraSerializableProperty]
		public bool AutoStretchPageHeaders {
			get { return autoStretchPageHeaders; }
			set {
				if(AutoStretchPageHeaders == value)
					return;
				autoStretchPageHeaders = value;
				OnAutoStretchPageHeadersChanged();
			}
		}
		protected virtual void OnExpandedChanged() {
			if(Reference != null)
				Reference.Expanded = Expanded;
			if(Ribbon == null)
				return;
			if(Ribbon.GetRibbonStyle() != RibbonControlStyle.MacOffice || !Ribbon.ViewInfo.GetShowPageHeaders())
				return;
			Ribbon.CheckViewInfo();
			BoundsAnimationInfo info = XtraAnimator.Current.Get(Ribbon, this) as BoundsAnimationInfo;
			XtraAnimator.Current.Animations.Remove(Ribbon, this);
			int delta = info != null? (int)(info.CurrentTick - info.BeginTick) / 10000: 0;
			if(Expanded)
				XtraAnimator.Current.AddBoundsAnimation(Ribbon, this, this, true, info!= null? info.CurrentBounds: CategoryInfo.CollapsedBounds, CategoryInfo.LowerBounds, 200 - delta);
			else
				XtraAnimator.Current.AddBoundsAnimation(Ribbon, this, this, false, info != null? info.CurrentBounds: CategoryInfo.LowerBounds, CategoryInfo.CollapsedBounds, 200 - delta);
		}
		protected virtual void OnAutoStretchPageHeadersChanged() {
			OnPageCategoryChanged();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryMergeOrder"),
#endif
 DefaultValue(-1), Category("Behavior"), XtraSerializableProperty]
		public int MergeOrder {
			get { return mergeOrder; }
			set {
				if(MergeOrder == value) return;
				mergeOrder = value;
				OnMergeOrderChanged();
			}
		}
		protected virtual void OnMergeOrderChanged() {
			if(Reference != null)
				Reference.MergeOrder = MergeOrder;
			OnPageCategoryChanged();
		}
		[Browsable(false)]
		public override RibbonControl Ribbon {
			get {
				if(Collection == null) return null;
				return Collection.Ribbon;
			}
		}
		[Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(false, true, false)]
		public RibbonPageCollection Pages { get { return pages; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set {
				if(Tag == value)
					return;
				tag = value;
				OnTagChanged();
			}
		}
		protected virtual void OnTagChanged() {
			if(Reference != null)
				Reference.Tag = Tag;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryVisible"),
#endif
 DefaultValue(true), Category("Behavior"), Localizable(true), SmartTagProperty("Visible", "Appearance", 30), XtraSerializableProperty]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnPageCategoryVisibleChanged();
			}
		}
		internal bool InternalVisible { get; set; }
		internal virtual bool ActualVisible {
			get {
				if(Ribbon != null && !Ribbon.IsDesignMode)
					return Ribbon.AutoHideEmptyItems ? InternalVisible && Visible : Visible;
				return Visible;
			}
		}
		protected virtual bool ShouldSerializeColor() { return color != Color.FromArgb(255, 231, 193, 253); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryColor"),
#endif
 Category("Appearance"), Localizable(true), SmartTagProperty("Color", "Appearance", 10), XtraSerializableProperty]
		public Color Color {
			get { return color; }
			set {
				if(Color == value) return;
				Color oldValue = color;
				color = value;
				OnColorChanged(oldValue, Color);
			}
		}
		protected virtual void OnColorChanged(Color prevColor, Color newColor) {
			ColoredRibbonElementsCache.RemoveColoredImages(prevColor);
			if(Reference != null)
				Reference.Color = newColor;
			OnPageCategoryChanged();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryText"),
#endif
 Category("Appearance"), Localizable(true), SmartTagProperty("Text", "Appearance", 0, SmartTagActionType.RefreshAfterExecute), XtraSerializableProperty]
		public string Text {
			get { return text; }
			set {
				if(text == value) return;
				text = value;
				OnPageCategoryTextChanged();
			}
		}
		private void OnPageCategoryTextChanged() {
			if(Reference != null)
				Reference.Text = Text;
			OnPageCategoryChanged();
		}
		protected internal virtual bool ShouldSerializeText() {
			return !String.IsNullOrEmpty(Text);
		}
		protected internal virtual void ResetText() {
			Text = String.Empty;
		}
		protected virtual void OnPageCategoryChanged() {
			if(Ribbon == null) return;
			Ribbon.Refresh();
		}
		protected virtual void OnPageCategoryVisibleChanged() {
			if(Reference != null)
				Reference.Visible = Visible;
			if(Pages == null) return;
			foreach(RibbonPage page in Pages) {
				page.OnPageVisibleChanged();
			}		   
		}
		#region Name property
		string name = string.Empty;
		[Browsable(false), DefaultValue(""), SkipRuntimeSerialization]
		public virtual string Name {
			get {
				if(this.Site != null) name = this.Site.Name;
				return name;
			}
			set {
				if(value == null) value = string.Empty;
				name = value;
				if(Site != null) Site.Name = name;
			}
		}
		#endregion
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageCategoryCategoryInfo"),
#endif
 Browsable(false), SkipRuntimeSerialization]
		public virtual RibbonPageCategoryViewInfo CategoryInfo {
			get {
				if(Ribbon == null) return null;
				for(int i = 0; i < Ribbon.ViewInfo.Header.PageCategories.Count; i++) {
					if(this == Ribbon.ViewInfo.Header.PageCategories[i].Category) return Ribbon.ViewInfo.Header.PageCategories[i];
				}
				return null;
			}
		}
		internal bool IsDefaultColor { get { return Color == Color.Transparent || Color.IsEmpty; } }
		public virtual RibbonPage GetPageByText(string pageText) {
			foreach(RibbonPage page in Pages) {
				if(page.Text == pageText) return page;
			}
			return null;
		}
		#region IXtraObjectWithBounds Members
		Rectangle IXtraObjectWithBounds.AnimatedBounds {
			get { return CategoryInfo.AnimatedBounds; }
			set { 
				CategoryInfo.AnimatedBounds = value;
				Ribbon.ViewInfo.Header.UpdateHeaderLayout();
				Ribbon.Invalidate(Ribbon.ViewInfo.Header.Bounds);
			}
		}
		void IXtraObjectWithBounds.OnEndBoundAnimation(BoundsAnimationInfo anim) {
			Ribbon.Refresh();
		}
		#endregion
		protected internal void ClearReference() {
			if(Reference != null) {
				Reference.Reference = null;
				Reference = null;
			}
			foreach(RibbonPage page in Pages) {
				page.ClearReference();
			}
		}
		protected override Rectangle GetVisualEffectBounds() { return CategoryInfo != null ? CategoryInfo.Bounds : Rectangle.Empty; }
		protected override bool GetVisualEffectsVisible() { return base.GetVisualEffectsVisible() && ActualVisible; }
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public abstract class BaseRibbonComponent : BaseBarComponent {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract RibbonControl Ribbon { get; }
		protected override ISupportAdornerUIManager GetVisualEffectsOwner() { return Ribbon; }
		protected override bool GetVisualEffectsVisible() { return Ribbon != null && Ribbon.Visible; }
	}
	[
	DXToolboxItem(false), DesignTimeVisible(false),
	Designer("DevExpress.XtraBars.Ribbon.Design.RibbonPageDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)), 
	SmartTagSupport(typeof(RibbonPageDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	SmartTagAction(typeof(RibbonPageDesignTimeActionsProvider), "AddPageGroup", "Add PageGroup", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(RibbonPageDesignTimeActionsProvider), "AddPage", "Add Page", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(RibbonPageDesignTimeActionsProvider), "AddPageCategory", "Add PageCategory", SmartTagActionType.CloseAfterExecute),
	]
	public class RibbonPage : BaseRibbonComponent, ISupportRibbonKeyTip, ISupportMergeOrder, ICloneable {
		object tag;
		string text;
		string userKeyTip;
		string itemKeyTip;
		int firstIndex;
		RibbonPageGroupCollection groups;
		RibbonPageGroupCollection mergedGroups;
		RibbonPage mergedPage;
		RibbonPageCollection collection;
		bool visible, destroying = false;
		Image image;
		int imageIndex;
		int imageToTextIndent;
		HorzAlignment imageAlign;
		int mergeOrder;
		ReduceOperationCollection reduceOperations;
		public RibbonPage(string text) {
			this.visible = true;
			this.groups = new RibbonPageGroupCollection(this);
			this.text = text;
			this.userKeyTip = string.Empty;
			this.itemKeyTip = string.Empty;
			this.collection = null;
			this.firstIndex = 0;
			this.imageIndex = -1;
			this.image = null;
			this.imageToTextIndent = -1;
			this.imageAlign = HorzAlignment.Default;
			this.mergeOrder = -1;
			this.appearance = CreateAppearance();
		}
		public RibbonPage() : this(string.Empty) { }
		protected override void Dispose(bool disposing) {
			if(destroying) return;
			this.destroying = true;
			if(disposing) {
				UpdateVisualEffects(UpdateAction.Dispose);
				DestroyGroups();
				if(Ribbon != null) Ribbon.Pages.Remove(this);
			}
			base.Dispose(disposing);
		}
		public override string ToString() {
			return string.IsNullOrEmpty(Text) ? Name : Text;
		}
		[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SkipRuntimeSerialization]
		public ReduceOperationCollection ReduceOperations {
			get {
				if(reduceOperations == null)
					reduceOperations = new ReduceOperationCollection(this);
				return reduceOperations;
			}
		}
		[Browsable(false), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageCollection")
#else
	Description("")
#endif
]
		public RibbonPageCollection Collection { get { return collection; } }
		internal void SetCollection(RibbonPageCollection collection) { this.collection = collection; }
		protected virtual RibbonPage CreatePage() { return new RibbonPage(); }
		public object Clone() {
			return Clone(false);
		}
		protected internal object Clone(bool addReference) {
			RibbonPage res = CreatePage();
			res.Name = Name;
			res.tag = tag;
			res.text = text;
			res.userKeyTip = userKeyTip;
			res.itemKeyTip = itemKeyTip;
			res.firstIndex = firstIndex;
			res.mergedPage = mergedPage;
			res.collection = collection;
			res.visible = visible;
			res.Image = Image;
			res.ImageIndex = ImageIndex;
			res.ImageAlign = ImageAlign;
			res.mergeOrder = mergeOrder;
			res.Appearance.Assign(Appearance);
			foreach(RibbonPageGroup group in Groups) {
				res.Groups.Add((RibbonPageGroup)group.Clone(addReference));
			}
			if(MergedGroups == null) return res;
			foreach(RibbonPageGroup group in MergedGroups) {
				res.MergedGroups.Add((RibbonPageGroup)group.Clone(addReference));
			}
			foreach(ReduceOperation op in ReduceOperations) {
				ReduceOperation op2 = (ReduceOperation)op.Clone();
				op.Reference = op2;
				res.ReduceOperations.Add(op2);
				op2.Assign(op);
			}
			if(addReference) {
				this.Reference = res;
				res.Reference = this;
			}
			return res;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageMergeOrder"),
#endif
 DefaultValue(-1), Category("Behavior"), XtraSerializableProperty]
		public int MergeOrder {
			get { return mergeOrder; }
			set {
				if(MergeOrder == value) return;
				mergeOrder = value;
				OnMergeOrderChanged();
			}
		}
		protected virtual void OnMergeOrderChanged() {
			if(Reference != null)
				Reference.MergeOrder = MergeOrder;
			OnPageChanged();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageImageAlign"),
#endif
 Category("Appearance"), DefaultValue(HorzAlignment.Default), SmartTagProperty("Image Alignment", "Image", 20), XtraSerializableProperty]
		public HorzAlignment ImageAlign {
			get { return imageAlign; }
			set {
				if(ImageAlign == value) return;
				imageAlign = value;
				OnImageAlignChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageImage"),
#endif
 Category("Appearance"), DefaultValue(null), SmartTagProperty("Image", "Image", 0, SmartTagActionType.RefreshAfterExecute), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(Image == value) return;
				image = value;
				OnImageChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageImageIndex"),
#endif
 Category("Appearance"), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("Images"), DefaultValue(-1), SmartTagProperty("Image Index", "Image", 10, SmartTagActionType.RefreshAfterExecute), XtraSerializableProperty]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value) return;
				imageIndex = value;
				OnImageIndexChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageImageToTextIndent"),
#endif
 System.ComponentModel.Category("Appearance"), DefaultValue(-1), XtraSerializableProperty]
		public int ImageToTextIndent {
			get { return imageToTextIndent; }
			set {
				if(ImageToTextIndent == value) return;
				imageToTextIndent = value;
				OnImageToTextIndentChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Images {
			get {
				if(Ribbon == null) return null;
				return Ribbon.Images;
			}
		}
		[Browsable(false)]
		public int PageIndex { 
			get {
				if(Category == null) return -1;
				return Category.Pages.IndexOf(this);
			} 
		}
		public Image GetImage() {
			if(Image != null) return Image;
			if(Ribbon == null) return null;
			return ImageCollection.GetImageListImage(Images, ImageIndex);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonPageCategory")]
#endif
public RibbonPageCategory Category { 
			get {
				if(Collection == null || Collection.Category == null) return null;
				return Collection.Category;
			}
		}
		protected virtual void DestroyGroups() {
			Groups.Destroy();
		}
		#region Merging
		protected internal RibbonPage MergedPage { get { return mergedPage; } }
		protected virtual void MergeGroupCollection(RibbonPageGroupCollection coll) {
			foreach(RibbonPageGroup mergedGroup in coll) {
				MergeGroup(mergedGroup);
			}
		}
		RibbonPageGroup FindVisibleGroup(string text) {
			foreach(RibbonPageGroup group in Groups) {
				if(group.Text == text && group.Visible)
					return group;
			}
			return null;
		}
		protected internal virtual void MergeGroup(RibbonPageGroup mergedGroup) {
			RibbonPageGroup group = RibbonControl.AllowMergeInvisibleItems? Groups.GetGroupByText(mergedGroup.Text): FindVisibleGroup(mergedGroup.Text);
			if(group == null) {
				MergedGroups.Add((RibbonPageGroup)mergedGroup.Clone(true));
			}
			else group.MergeGroup(mergedGroup);
		}
		protected internal virtual void MergePage(RibbonPage page) {
			UnMergePage();
			this.mergedPage = page;
			MergeGroupCollection(MergedPage.Groups);
			MergeGroupCollection(MergedPage.MergedGroups);
		}
		protected internal virtual void UnMergePage() {
			if(MergedPage == null) return;
			mergedPage = null;
			for(int i = MergedGroups.Count - 1; i >= 0; i--) {
				if(MergedGroups[i].Reference != null)
					MergedGroups[i].Reference.Reference = null;
				MergedGroups[i].Reference = null;
				MergedGroups[i].Dispose();
			} 
			MergedGroups.Clear();
			foreach(RibbonPageGroup group in Groups) {
				group.UnMergeGroup();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageMergedGroups"),
#endif
 Browsable(false)]
		public virtual RibbonPageGroupCollection MergedGroups {
			get {
				if(mergedGroups == null) mergedGroups = new RibbonPageGroupCollection(this);
				return mergedGroups;
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RibbonControl Ribbon { 
			get {
				if(Collection == null) return null;
				return Collection.Ribbon;
			} 
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), XtraSerializableProperty(false, true, false)]
		public virtual RibbonPageGroupCollection Groups { get { return groups; } }
		public RibbonPageGroup GetGroupByName(string name) { return Groups.GetGroupByName(name); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageVisible"),
#endif
 DefaultValue(true), Category("Behavior"), XtraSerializableProperty]
		public bool Visible {
			get { return Category != null ? Category.Visible && visible : visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnPageVisibleChanged();
			}
		}
		internal bool InternalVisible { get; set; }
		internal bool ActualVisible {
			get {
				if(Ribbon != null && !Ribbon.IsDesignMode)
					return Ribbon.AutoHideEmptyItems ? InternalVisible && Visible : Visible;
				return Visible;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set {
				if(Tag == value)
					return;
				tag = value;
				OnTagChanged();
			}
		}
		protected virtual void OnTagChanged() {
			if(Reference != null)
				Reference.Tag = Tag;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageText"),
#endif
 Category("Appearance"), Localizable(true), SmartTagProperty("Text", "Appearance", 0, SmartTagActionType.RefreshAfterExecute), XtraSerializableProperty]
		public string Text {
			get { return text; }
			set {
				if(text == value) return;
				text = value;
				OnTextChanged();
			}
		}
		protected virtual void OnTextChanged() {
			if(Reference != null)
				Reference.Text = Text;
			OnPageChanged();
		}
		AppearanceObject appearance;
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageAppearance"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SkipRuntimeSerialization]
		public virtual AppearanceObject Appearance { get { return appearance; } }
		protected virtual AppearanceObject CreateAppearance() {
			AppearanceObject res = new AppearanceObject();
			res.Changed += delegate { OnAppearanceChanged(); };
			return res;
		}
		protected internal RibbonPage Reference { get; set; }
		bool SyncAppearances { get; set; }
		protected virtual void OnAppearanceChanged() {
			if(Reference != null) {
				if(!SyncAppearances) {
					SyncAppearances = true;
					try {
						Reference.Appearance.Assign(Appearance);
					}
					finally {
						SyncAppearances = false;
					}
				}
			}
			OnPageChanged();
		}
		protected internal virtual bool ShouldSerializeText() {
			return !String.IsNullOrEmpty(Text);
		}
		protected internal virtual void ResetText() {
			Text = String.Empty;
		}
		protected internal virtual void OnPageVisibleChanged() {
			if(Reference != null)
				Reference.Visible = Visible;
			if(destroying) return;
			if(Ribbon != null) Ribbon.OnPageVisibleChanged(this);
		}
		protected virtual void OnPageChanged() {
			if(destroying) return;
			if(Ribbon != null) Ribbon.OnPageChanged(this);
		}
		protected internal virtual void OnGroupCollectionChanged(CollectionChangeEventArgs e) {
			OnPageChanged();
		}
		#region Name property
		string name = string.Empty;
		[Browsable(false), DefaultValue(""), SkipRuntimeSerialization]
		public virtual string Name {
			get {
				if(this.Site != null) name = this.Site.Name;
				return name;
			}
			set {
				if(value == null) value = string.Empty;
				name = value;
				if(Site != null) Site.Name = name;
			}
		}
		#endregion
		[Browsable(false), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPagePageInfo"),
#endif
 SkipRuntimeSerialization]
		public virtual RibbonPageViewInfo PageInfo { 
			get {
				if(Ribbon == null) return null;
				for(int i = 0; i < Ribbon.ViewInfo.Header.Pages.Count; i++) {
					if(this == Ribbon.ViewInfo.Header.Pages[i].Page) return Ribbon.ViewInfo.Header.Pages[i];
				}
				return null;
			} 
		}
		string ISupportRibbonKeyTip.ItemCaption { get { return Text; } }
		string ISupportRibbonKeyTip.ItemKeyTip { get { return itemKeyTip; } set { itemKeyTip = value; } }
		string ISupportRibbonKeyTip.ItemUserKeyTip { 
			get { return userKeyTip; } 
			set {
				if(value == null)
					value = string.Empty;
				else 
					value = value.ToUpper();
				if(((ISupportRibbonKeyTip)this).ItemUserKeyTip == value)
					return;
				userKeyTip = value;
				OnUserKeyTipChanged();
			} 
		}
		protected virtual void OnUserKeyTipChanged() {
			if(Reference != null)
				Reference.KeyTip = KeyTip;
		}
		int ISupportRibbonKeyTip.FirstIndex { get { return firstIndex; } set { firstIndex = value; } }
		void ISupportRibbonKeyTip.Click() { 
			Ribbon.SelectedPage = this;
			Ribbon.HideApplicationButtonContentControl();
			Ribbon.ViewInfo.IsReady = false;
			Ribbon.CheckViewInfo();
			Ribbon.Invalidate();
			if(Ribbon.Minimized) {
				Ribbon.KeyTipManager.HideKeyTips();
				Ribbon.ShowMinimizedRibbon();
				Ribbon.MinimizedRibbonPopupForm.Control.KeyTipManager.ActivatePanelKeyTips();
			}
			else {
				Ribbon.KeyTipManager.ActivatePanelKeyTips();
			}
		}
		bool ISupportRibbonKeyTip.KeyTipEnabled { get { return true; } }
		bool ISupportRibbonKeyTip.KeyTipVisible { 
			get {
				if(PageInfo == null)
					return true;
				return Ribbon.ClientRectangle.Contains(PageInfo.Bounds); 
			} 
		}
		bool ISupportRibbonKeyTip.HasDropDownButton { get { return false; } }
		internal bool ShowPageHeaders {
			get {
				if(Ribbon.ShowPageHeadersMode == ShowPageHeadersMode.Hide) return false;
				if(Ribbon.ShowPageHeadersMode == ShowPageHeadersMode.ShowOnMultiplePages && Ribbon.Pages.VisiblePages.Count <= 1) return false;
				return true;
			}
		}
		Point ISupportRibbonKeyTip.ShowPoint { 
			get {
				if(!ShowPageHeaders) {
					if(this != Ribbon.SelectedPage) return Point.Empty;
					Point pt = Point.Empty;
					if(Ribbon.ViewInfo.Panel.Groups.Count == 0) pt = new Point(50, Ribbon.ViewInfo.Panel.Bounds.Y);
					else {
						pt = new Point(Ribbon.ViewInfo.Panel.Groups[0].Bounds.X, Ribbon.ViewInfo.Panel.Groups[0].Bounds.Y);
						pt.Offset(Ribbon.ViewInfo.Panel.Groups[0].Bounds.Width / 2, 0);
					}
					return Ribbon.PointToScreen(pt);
				}
				RibbonPageViewInfo pageInfo = PageInfo;
				if(pageInfo == null) return Point.Empty;
				int textHeight = pageInfo.GetTextSize().Height;
				return Ribbon.PointToScreen(new Point(pageInfo.Bounds.X + pageInfo.Bounds.Width / 2, pageInfo.Bounds.Top + (pageInfo.Bounds.Height - textHeight) / 2 + pageInfo.GetTextAscentHeight()));
			} 
		}
		bool ISupportRibbonKeyTip.IsCommandItem { get { return false; } }
		ContentAlignment ISupportRibbonKeyTip.Alignment { 
			get {
				if(!ShowPageHeaders) return ContentAlignment.MiddleCenter;
				return ContentAlignment.TopCenter; 
			} 
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageKeyTip"),
#endif
 Category("Appearance"), DefaultValue(""), XtraSerializableProperty]
		public string KeyTip { 
			get { return (this as ISupportRibbonKeyTip).ItemUserKeyTip; } 
			set { (this as ISupportRibbonKeyTip).ItemUserKeyTip = value; } }
		[Browsable(false), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPageIsInDefaultCategory")
#else
	Description("")
#endif
]
		public virtual bool IsInDefaultCategory {
			get { return Ribbon == null || Category == Ribbon.DefaultPageCategory; }
		}
		protected virtual void OnImageToTextIndentChanged() {
			if(Reference != null)
				Reference.ImageToTextIndent = ImageToTextIndent;
			if(Ribbon != null)
				Ribbon.Refresh();
		}
		protected virtual void OnImageChanged() {
			if(Reference != null)
				Reference.Image = Image;
			if(Ribbon != null)
				Ribbon.RefreshHeight();
		}
		protected virtual void OnImageIndexChanged() {
			if(Reference != null)
				Reference.ImageIndex = ImageIndex;
			if(Ribbon != null)
				Ribbon.RefreshHeight();
		}
		protected virtual void OnImageAlignChanged() {
			if(Reference != null)
				Reference.ImageAlign = ImageAlign;
			if(Ribbon != null)
				Ribbon.Refresh();
		}
		protected internal void ClearReference() {
			if(Reference != null) {
				Reference.Reference = null;
				Reference = null;
			}
			foreach(RibbonPageGroup group in Groups) {
				group.ClearReference();
			}
		}
		protected override bool GetVisualEffectsVisible() {
			return base.GetVisualEffectsVisible() && Category != null && Category.Visible && ActualVisible;
		}
		protected override Rectangle GetVisualEffectBounds() {
			return PageInfo != null ? PageInfo.Bounds : Rectangle.Empty;
		}
	}
	[ListBindable(false)]
	public class RibbonTotalPageCategory : RibbonPageCategory {
		RibbonPageTotalCollection totalCollection;
		public RibbonTotalPageCategory(RibbonPageCategoryCollection coll)
			: base(BarLocalizer.Active.GetLocalizedString(BarString.RibbonAllPages), Color.Empty) {
			SetCollection(coll);
			this.totalCollection = new RibbonPageTotalCollection(this);
		}
		public new RibbonPageTotalCollection Pages { get { return totalCollection; } }
		public virtual ArrayList GetVisiblePages() {
			ArrayList res = new ArrayList();
			if(!ActualVisible) return res;
			foreach(RibbonPage page in Pages) {
				if(page.ActualVisible) res.Add(page);
			}
			return res;
		}
		public virtual RibbonPage GetFirstVisiblePage() {
			ArrayList res = GetVisiblePages();
			if(res.Count == 0) return null;
			return res[0] as RibbonPage;
		}
		public override RibbonPage GetPageByText(string pageText) {
			IList list = Pages;
			foreach(RibbonPage page in list) {
				if(page.Text == pageText) return page;
			}
			return null;
		}
		public RibbonPage GetPageByText(string pageText, string category) {
			IList list = Pages;
			foreach(RibbonPage page in list) {
				if(page.Text == pageText && ((page.Category != null) && page.Category.Text == category))
					return page;
			}
			return null;
		}
		internal override bool ActualVisible { get { return Visible; } }
	}
	[ListBindable(false)]
	public class RibbonPageTotalCollection : IList {
		RibbonTotalPageCategory totalCategory;
		public RibbonPageTotalCollection(RibbonTotalPageCategory totalCategory) {
			this.totalCategory = totalCategory;
		}
		int IList.Add(object value) { return -1; }
		void IList.Clear() { }
		bool IList.Contains(object value) { return GetItems().Contains(value); }
		int IList.IndexOf(object value) { return GetItems().IndexOf(value); }
		void IList.Insert(int index, object value) { }
		void IList.Remove(object value) { }
		void IList.RemoveAt(int index) { }
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return true; } }
		void ICollection.CopyTo(Array array, int index) { }
		int ICollection.Count { get { return GetItems().Count; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		public int Count { get { return (this as ICollection).Count; } }
		object IList.this[int index] {
			get {
				return GetItems()[index];
			}
			set { }
		}
		public IEnumerator GetEnumerator() {
			return GetItems().GetEnumerator();
		}
		public RibbonPage this[int index] {
			get {
				return ((IList)this)[index] as RibbonPage;
			}
		}
		RibbonControl Ribbon { 
			get { 
				if(totalCategory == null) return null;
				return totalCategory.Ribbon;
			} 
		}
		protected IList GetItems() {
			ArrayList result = new ArrayList();
			if(Ribbon == null) return result;
			RibbonPageCategory[] pc = totalCategory.Ribbon.GetPageCategories();
			foreach(RibbonPageCategory category in pc) {
				foreach(RibbonPage page in category.Pages) {
					result.Add(page);
				}
				foreach(RibbonPage page in category.MergedPages){
					result.Add(page);
				}
			}
			return result;
		}
		public void Destroy() {
			foreach (RibbonPageCategory category in totalCategory.Ribbon.GetPageCategories()) {
				category.Pages.Destroy();
			}
		}
		public bool Contains(RibbonPage page) {
			foreach (RibbonPageCategory category in totalCategory.Ribbon.GetPageCategories()) {
				if(category.Pages.Contains(page)) return true;
				if(category.MergedPages != null && category.MergedPages.Contains(page)) return true; 
			}
			return false;
		}
		public int IndexOf(RibbonPage page) {
			int cnt = 0;
			foreach (RibbonPageCategory category in totalCategory.Ribbon.GetPageCategories()) {
				if (category.Pages.Contains(page)) return category.Pages.IndexOf(page) + cnt;
				else cnt += category.Pages.Count;
			}
			return -1;
		}
		protected internal bool HasVisibleItems {
			get {
				bool hasVisibleItems = false;
				foreach(RibbonPage page in this) {
					if(page.ActualVisible == true) {
						hasVisibleItems = true;
						break;
					}
				}
				return hasVisibleItems;
			}
		}
	}
	[ListBindable(false)]
	public class RibbonPageCollection : CollectionBase, IList {
		RibbonControl ribbon;
		RibbonPageCategory category;
		public RibbonPageCollection(RibbonControl ribbon, RibbonPageCategory category) : this(ribbon) {
			this.category = category;
		}
		public RibbonPageCollection(RibbonControl ribbon) {
			this.ribbon = ribbon;
			this.category = null;
		}
		[Browsable(false)]
		public RibbonPageCategory Category { get { return category; } }
		protected internal RibbonControl Ribbon { 
			get {
				if(Category != null && Category.Ribbon != null) return Category.Ribbon;
				return ribbon; 
			} 
		}
		internal void SetRibbon(RibbonControl ribbon) { this.ribbon = ribbon; }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonPageCollectionItem")]
#endif
		public virtual RibbonPage this[int index] { get { return List[index] as RibbonPage; } }
		public RibbonPage this[string text] { get { return GetPageByText(text); } }
		public RibbonPage GetPageByName(string name) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Name == name) return this[i];
			}
			return null;
		}
		public RibbonPage GetPageByText(string text) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Text == text) return this[i];
			}
			return null;
		}
		public virtual int Add(RibbonPage page) {
			return List.Add(page);
		}
		public virtual void Remove(RibbonPage page) {
			if(List.Contains(page)) List.Remove(page);
		}
		public virtual void AddRange(RibbonPage[] pages) {
			foreach(RibbonPage page in pages) { Add(page); }
		}
		public virtual void Insert(int index, RibbonPage page) {
			List.Insert(index, page);
		}
		protected override void OnInsert(int index, object value) {
			base.OnInsert(index, value);
			RibbonPage page = (RibbonPage)value;
			if(page.Collection != null) page.Collection.Remove(page);
			page.SetCollection(this);
		}
		public virtual bool Contains(RibbonPage page) { return List.Contains(page); }
		public virtual int IndexOf(RibbonPage page) { return List.IndexOf(page); }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			RibbonPage page = (RibbonPage)value;
			page.Disposed += new EventHandler(page_Disposed);
			if(Ribbon != null) Ribbon.OnPageAdded(page);
		}
		void page_Disposed(object sender, EventArgs e) {
			if(List.Contains(sender)) List.Remove(sender);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			RibbonPage page = (RibbonPage)value;
			if(Ribbon!=null) Ribbon.OnPageRemoved(page);
			page.SetCollection(null);
			page.Disposed -= new EventHandler(page_Disposed);
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		protected internal RibbonPage FirstVisiblePage {
			get {
				if(Count == 0) return null;
				if(Ribbon.IsDesignMode) return this[0];
				for(int n = 0; n < Count; n++) {
					if(this[n].Visible) return this[n];
				}
				return null;
			}
		}
		internal void Destroy() {
			ArrayList list = new ArrayList(List);
			InnerList.Clear();
			foreach(RibbonPage page in list) page.Dispose();
		}
		protected internal ArrayList VisiblePages {
			get {
				ArrayList list = new ArrayList();
				for(int n = 0; n < Count; n++) {
					if(this[n].Visible) list.Add(this[n]);
				}
				return list;
			}
		}
		protected internal bool HasVisibleItems {
			get {
				bool hasVisibleItems = false;
				foreach(RibbonPage page in this) {
					if(page.ActualVisible) {
						hasVisibleItems = true;
						break;
					}
				}
				return hasVisibleItems;
			}
		}
		#region IList
		int IList.Add(object value) {
			if(InnerList.Contains(value)) 
				return InnerList.IndexOf(value);
			OnValidate(value);
			OnInsert(this.InnerList.Count, value);
			int index = this.InnerList.Add(value);
			try {
				this.OnInsertComplete(index, value);
			}
			catch {
				this.InnerList.RemoveAt(index);
				throw;
			}
			return index;
		}
		void IList.Insert(int index, object value) {
			if(InnerList.Contains(value)) return;
			index = Math.Max(0, Math.Min(Count, index));
			this.OnValidate(value);
			this.OnInsert(index, value);
			this.InnerList.Insert(index, value);
			try {
				this.OnInsertComplete(index, value);
			}
			catch {
				this.InnerList.RemoveAt(index);
				throw;
			}
		}
		#endregion
	}
	[ListBindable(false)]
	public class RibbonPageGroupCollection : CollectionBase, IList {
		RibbonPage page;
		public event CollectionChangeEventHandler CollectionChanged;
		public RibbonPageGroupCollection(RibbonPage page) {
			this.page = page;
		}
		public void Remove(RibbonPageGroup group) {
			if(List.Contains(group)) {
				group.HideContentDropDown();
				List.Remove(group);
			}
		}
		protected RibbonPage Page { get { return page; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonPageGroupCollectionItem")]
#endif
		public virtual RibbonPageGroup this[int index] { get { return List[index] as RibbonPageGroup; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonPageGroupCollectionItem")]
#endif
		public RibbonPageGroup this[string name] { get { return GetGroupByName(name); } }
		public RibbonPageGroup GetGroupByName(string name) {
			foreach(RibbonPageGroup group in this) {
				if(group.Name == name) return group;
			}
			return null;
		}
		public RibbonPageGroup GetGroupByText(string text) {
			foreach(RibbonPageGroup group in this) {
				if(group.Text == text) return group;
			}
			return null;
		}
		public virtual int Add(RibbonPageGroup group) {
			return List.Add(group);
		}
		public void Insert(int index, RibbonPageGroup group) { 
			List.Insert(index, group);
		}
		public bool Contains(RibbonPageGroup group) { return List.Contains(group); }
		public int IndexOf(RibbonPageGroup group) { return List.IndexOf(group); }
		public void AddRange(RibbonPageGroup[] groups) {
			foreach(RibbonPageGroup group in groups) { Add(group); }
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			RibbonPageGroup group = (RibbonPageGroup)value;
			group.Disposed -= new EventHandler(OnGroupDisposed);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
			if(Page != null && Page.Ribbon != null)
				Page.Ribbon.OnGroupRemoved(group);
		}
		protected override void OnClear() {
			InnerList.Clear();
			base.OnClear();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected override void OnInsertComplete(int index, object value) {
			RibbonPageGroup group = (RibbonPageGroup)value;
			group.Disposed += new EventHandler(OnGroupDisposed);
			group.SetPage(page);
			base.OnInsertComplete(index, value);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
			if(Page != null && Page.Ribbon != null)
				Page.Ribbon.OnGroupAdded(group);
		}
		void OnGroupDisposed(object sender, EventArgs e) {
			if(List.Contains(sender)) List.Remove(sender);
		}
		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null)
				CollectionChanged(this, e);
			if(Page != null) Page.OnGroupCollectionChanged(e);
		}
		internal void Destroy() {
			ArrayList list = new ArrayList(List);
			InnerList.Clear();
			foreach(RibbonPageGroup pageGroup in list) pageGroup.Dispose();
		}
		protected internal ArrayList VisibleGroups {
			get {
				ArrayList list = new ArrayList();
				for(int n = 0; n < Count; n++) {
					if(this[n].Visible) list.Add(this[n]);
				}
				return list;
			}
		}
		protected internal bool HasVisibleItems {
			get {
				bool hasVisibleItems = false;
				foreach(RibbonPageGroup group in this) {
					if(group.ActualVisible) {
						hasVisibleItems = true;
						break;
					}
				}
				return hasVisibleItems;
			}
		}
		#region IList 
		int IList.Add(object value) {
			if(InnerList.Contains(value)) 
				return InnerList.IndexOf(value);
			this.OnValidate(value);
			this.OnInsert(this.InnerList.Count, value);
			int index = this.InnerList.Add(value);
			try {
				this.OnInsertComplete(index, value);
			}
			catch {
				this.InnerList.RemoveAt(index);
				throw;
			}
			return index;
		}
		void IList.Insert(int index, object value) {
			if(InnerList.Contains(value)) return;
			index = Math.Max(0, Math.Min(Count, index));
			this.OnValidate(value);
			this.OnInsert(index, value);
			this.InnerList.Insert(index, value);
			try {
				this.OnInsertComplete(index, value);
			}
			catch {
				this.InnerList.RemoveAt(index);
				throw;
			}
		}
		#endregion 
	}
	public interface IRibbonMergingContext : IDisposable {
		RibbonControl Ribbon { get; }
		void CheckSelectedPage(RibbonControl ribbon);
		void SelectedPageChanged(RibbonPage ribbonPage);
		IDisposable LockSelectedPageChanging();
	}
	public class RibbonPageGroupEventArgs : EventArgs {
		RibbonPageGroup pageGroup;
		public RibbonPageGroupEventArgs(RibbonPageGroup pageGroup) {
			this.pageGroup = pageGroup;
		}
		public RibbonPageGroup PageGroup { get { return pageGroup; } }
	}
	public delegate void RibbonMergeEventHandler(object sender, RibbonMergeEventArgs e);
	public delegate void MinimizedRibbonEventHandler(object sender, MinimizedRibbonEventArgs e);
	public delegate void RibbonPageGroupEventHandler(object sender, RibbonPageGroupEventArgs e);
	public delegate void RibbonCustomizationMenuEventHandler(object sender, RibbonCustomizationMenuEventArgs e);
	public delegate void RibbonScreenModeChangedEventHandler(object sender, ScreenModeChangedEventArgs e);
	public class RibbonCustomizationMenuEventArgs : EventArgs {
		bool? showCustomizationMenu = true;
		RibbonHitInfo hitInfo = null;
		RibbonControl ribbon;
		BarItemLink link;
		public RibbonCustomizationMenuEventArgs(BarItemLink link, RibbonControl ribbon) {
			this.ribbon = ribbon;
			this.link = link;
		}
		public RibbonCustomizationMenuEventArgs(RibbonHitInfo hitInfo, RibbonControl ribbon) {
			this.ribbon = ribbon;
			this.hitInfo = hitInfo;
		}
		public RibbonCustomizationMenuEventArgs() : base() { }
		public bool? ShowCustomizationMenu { get { return showCustomizationMenu; } set { showCustomizationMenu = value; } }
		public RibbonHitInfo HitInfo { get { return hitInfo; } set { hitInfo = value; } }
		public RibbonControl Ribbon { get { return ribbon; } }
		public RibbonCustomizationPopupMenu CustomizationMenu { get { return Ribbon.CustomizationPopupMenu; } }
		public BarItemLink Link { get { return link; } set { link = value; } }
	}
	public class RibbonStyleChangedEventArgs : EventArgs {
		RibbonControlStyle oldStyle, newStyle;
		public RibbonStyleChangedEventArgs(RibbonControlStyle oldStyle, RibbonControlStyle newStyle) {
			this.oldStyle = oldStyle;
			this.newStyle = newStyle;
		}
		public RibbonControlStyle OldStyle { get { return oldStyle; } }
		public RibbonControlStyle NewStyle { get { return newStyle; } }
	}
	public class RibbonBarItems : BarItems {
		public RibbonBarItems(RibbonBarManager manager) : base(manager) { }
		public BarButtonGroup CreateButtonGroup(params BarBaseButtonItem[] items) {
			BarButtonGroup item = new BarButtonGroup(Manager, items);
			return item;
		}
	}
	public enum SupportedByRibbonKind { Supported, NonSupported, SupportedInMenu }
	public enum SupportedByBarManagerKind { Supported, NonSupported }
	public class SupportedByRibbon : Attribute {
		SupportedByRibbonKind support;
		public SupportedByRibbon(SupportedByRibbonKind support) {
			this.support = support;
		}
		public SupportedByRibbonKind Support { get { return support; } set { support = value; } }
	}
	public class SupportedByBarManager : Attribute {
		SupportedByBarManagerKind support;
		public SupportedByBarManager(SupportedByBarManagerKind support) { 
			this.support = support;
		}
		public SupportedByBarManagerKind Support { get { return support; } set { support = value; } }
	}
	public class SkipRuntimeSerialization : Attribute {
	}
	public enum ReduceOperationType { Gallery, ButtonGroups, LargeButtons, SmallButtonsWithText, CollapseGroup, CollapseItem }
	public enum ReduceOperationBehavior { Single, UntilAvailable }
	public class ReduceOperation : ICloneable {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonPage OwnerPage { get; set; }
		ReduceOperationType operation;
		[XtraSerializableProperty]
		public ReduceOperationType Operation {
			get { return operation; }
			set {
				if(Operation == value)
					return;
				operation = value;
				OnPropertyChanged();
			}
		}
		protected virtual void OnPropertyChanged() {
			if(Reference != null)
				Reference.Assign(this);
		}
		public void Assign(ReduceOperation src) {
			this.operation = src.Operation;
			this.Behavior = src.Behavior;
			this.ItemLinkIndex = src.ItemLinkIndex;
			this.ItemLinksCount = src.ItemLinksCount;
			if(OwnerPage != null && src.OwnerPage != null && src.Group != null)
				this.Group = OwnerPage.Groups[src.OwnerPage.Groups.IndexOf(src.Group)];
		}
		ReduceOperationBehavior behavior;
		[XtraSerializableProperty]
		public ReduceOperationBehavior Behavior {
			get { return behavior; }
			set {
				if(Behavior == value)
					return;
				behavior = value;
				OnPropertyChanged();
			}
		}
		RibbonPageGroup group;
		public RibbonPageGroup Group {
			get { return group; }
			set {
				if(Group == value)
					return;
				group = value;
				OnPropertyChanged();
			}
		}
		int itemLinkIndex;
		[XtraSerializableProperty]
		public int ItemLinkIndex {
			get { return itemLinkIndex; }
			set {
				if(ItemLinkIndex == value)
					return;
				itemLinkIndex = value;
				OnPropertyChanged();
			}
		}
		int itemLinksCount;
		[XtraSerializableProperty]
		public int ItemLinksCount {
			get { return itemLinksCount; }
			set {
				if(ItemLinksCount == value)
					return;
				itemLinksCount = value;
				OnPropertyChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReduceOperation Reference { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Clone() {
			ReduceOperation op = new ReduceOperation();
			op.Operation = Operation;
			op.Behavior = Behavior;
			op.Group = Group;
			op.ItemLinkIndex = ItemLinkIndex;
			op.ItemLinksCount = ItemLinksCount;
			return op;
		}
		public override string ToString() {
			string groupText = "";
			if(Group != null) {
				if(Operation == ReduceOperationType.CollapseGroup)
					groupText = " for " + Group.Text;
				else if(Operation == ReduceOperationType.Gallery)
					groupText = " in " + Group.Text.ToString() + " " + Behavior.ToString();
				else
					groupText = " in " + Group.Text.ToString();
			}
			return Operation.ToString() + groupText;
		}
	}
	public class ReduceOperationCollection : CollectionBase {
		public ReduceOperationCollection(RibbonPage page) {
			Page = page;
		}
		public RibbonPage Page { get; private set; }
		public RibbonControl Ribbon { get { return Page == null ? null : Page.Ribbon; } }
		public int Add(ReduceOperation op) { return List.Add(op); }
		public void Insert(int index, ReduceOperation op) { List.Insert(index, op); }
		public void Remove(ReduceOperation op) { List.Remove(op); }
		public int IndexOf(ReduceOperation op) { return List.IndexOf(op); }
		public ReduceOperation this[int index] { get { return (ReduceOperation)List[index]; } set { List[index] = value; } }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			((ReduceOperation)value).OwnerPage = Page;
			if(!IsUpdating && Ribbon != null)
				Ribbon.OnReduceOperationChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			((ReduceOperation)value).OwnerPage = null;
			if(!IsUpdating)
				Ribbon.OnReduceOperationChanged();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if(!IsUpdating)
				Ribbon.OnReduceOperationChanged();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			((ReduceOperation)oldValue).OwnerPage = null;
			((ReduceOperation)newValue).OwnerPage = Page;
			if(!IsUpdating)
				Ribbon.OnReduceOperationChanged();
		}
		public bool IsUpdating { get { return UpdateCount > 0; } }
		int UpdateCount { get; set; }
		public void BeginUpdate() {
			UpdateCount++;
		}
		public void EndUpdage() {
			if(UpdateCount > 0) {
				UpdateCount--;
				if(UpdateCount == 0 && Ribbon != null)
					Ribbon.OnReduceOperationChanged();
			}
		}
		public void Assign(ReduceOperationCollection src) {
			Clear();
			foreach(ReduceOperation op in src) {
				Add((ReduceOperation)op.Clone());
			}
		}
	}
	public abstract class RibbonRegistrationInfo {
		public abstract string PageCategoryName { get; }
		public abstract string PageName { get; }
		public abstract string PageGroupName { get; }
		public abstract Type PageCategoryType { get; }
		public abstract Type PageType { get; }
		public abstract Type PageGroupType { get; }
		public abstract BarItemInfoCollection ItemInfoCollection { get; }
		internal string GetPageName() {
			if(PageName != null)
				return PageName;
			return PageType != null ? PageType.Name : null;
		}
		internal string GetPageCategoryName() {
			if(PageCategoryName != null)
				return PageCategoryName;
			return PageCategoryType != null ? PageCategoryType.Name : null;
		}
		internal string GetPageGroupName() {
			if(PageGroupName != null)
				return PageGroupName;
			return PageGroupType != null ? PageGroupType.Name : null;
		}
	}
	public class RibbonDefaultRegistrationInfo : RibbonRegistrationInfo {
		public RibbonDefaultRegistrationInfo(RibbonControl ribbon) {
			Ribbon = ribbon;
		}
		public RibbonControl Ribbon { get; private set; }
		public override string PageCategoryName {
			get { return "PageCategory"; }
		}
		public override string PageName {
			get { return "Page"; }
		}
		public override string PageGroupName {
			get { return "PageGroup"; }
		}
		public override Type PageCategoryType {
			get { return typeof(RibbonPageCategory); }
		}
		public override Type PageGroupType {
			get { return typeof(RibbonPageGroup); }
		}
		public override Type PageType {
			get { return typeof(RibbonPage); }
		}
		public override BarItemInfoCollection ItemInfoCollection {
			get { return Ribbon.GetController().PaintStyle.ItemInfoCollection; }
		}
	}
} 
