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
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraBars.Accessible;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Helpers.Docking;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.MessageFilter;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Registration;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraBars {
	[Designer("DevExpress.XtraBars.Design.BarManagerDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	DesignerCategory("Component"),
 ProvideProperty("PopupContextMenu", typeof(Control)), DXToolboxItem(true),
	Description("Allows you to create bars on forms."),
	ToolboxTabName(AssemblyInfo.DXTabNavigation),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "BarManager")
	]
	public class BarManager : ComponentEditorContainer, System.ComponentModel.IExtenderProvider, IXtraSerializable, IXtraSerializableLayout, IBarManagerControl, IDXMenuManager, IDXDropDownMenuManager, IBarAndDockingControllerClient, ISupportXtraSerializer, ISupportLookAndFeel, IXtraSerializationIdProvider, IXtraCollectionDeserializationOptionsProvider {
		static bool allowFocusPopupForm = false;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool AllowFocusPopupForm { get { return allowFocusPopupForm; } set { allowFocusPopupForm = value; } }
		static bool updateMdiClientOnChildActivate = true;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool UpdateMdiClientOnChildActivate { get { return updateMdiClientOnChildActivate; } set { updateMdiClientOnChildActivate = value; } }
		static bool closeOnModalFormShow = true;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool ClosePopupOnModalFormShow { get { return closeOnModalFormShow; } set { closeOnModalFormShow = value; } }
		internal BarManager internalMenuManager;
		DockManager dockManager;
		BarAndDockingController controller;
		static internal Point zeroPoint = new Point(-6000, -6000);
		internal static ArrayList managers;
		internal static BarManagerMessageFilter messageFilter;
		Font tabFont;
		int maxItemId;
		BarMdiMenuMergeStyle mdiMenuMergeStyle;
		Bar mainMenu, statusBar;
		internal BarDockWindowCollection dockWindows;
		bool destroying;
		internal Point dragStartPoint;
		BarDockControls dockControls;
		Bars bars;
		BarItems items;
		BarManagerCategoryCollection categories;
		Control form;
		object largeImages, images;
		SharedImageCollectionImageSizeMode collectionSizeMode;
		BarManagerInternalItems internalItems;
		int inClearSelection, ignoreMouseUp, ignoreLeftMouseUp;
		bool dockingEnabled, isBarsActive, autoSaveInRegistry, storing, restoring, allowCustomization, allowShowToolbarsPopup, showScreenTipsInToolbars, showShortcutInScreenTips,
			showFullMenus, showFullMenusAfterDelay, allowMoveBarOnToolbar, allowQuickCustomization,
			useAltKeyForMenu,
			useF10KeyForMenu,
			closeButtonAffectAllTabs, showCloseButton, transparentEditors, processShortcutsWhenInvisible;
		DefaultBoolean rightToLeft = DefaultBoolean.Default;
		DevExpress.XtraBars.Customization.Helpers.ToolBarsPopup toolBarsPopup;
		int threadId;
		Form originalForm;
		internal int initCount = 0;
		BarSelectionInfo selectionInfo;
		Hashtable customizationProperties, popupContextMenus;
		ArrayList itemHolders;
		System.ComponentModel.Container components;
		BarManagerHelpers helper;
		string registryPath;
		ImageCollection barItemsImages;
		Hashtable cachedForms;
		bool allowDisposeObjects = true;
		bool allowDisposeItems = true;
		bool allowMergeInvisibleLinks = false;
		bool active = true;
		bool allowItemAnimatedHighlighting = true;
		BarManagerListenerCollection listeners;
		private static readonly object beforeLoadLayout = new object();
		private static readonly object layoutUpgrade = new object();
		private static readonly object showToolbarsContextMenu = new object();
		private static readonly object shortcutItemClick = new object();
		private static readonly object closeButtonClick = new object();
		private static readonly object unMerge = new object();
		private static readonly object merge = new object();
		private static readonly object itemClick = new object();
		private static readonly object itemDoubleClick = new object();
		private static readonly object hyperlinkClick = new object();
		private static readonly object itemPress = new object();
		private static readonly object createToolbar = new object();
		private static readonly object startCustomization = new object();
		private static readonly object endCustomization = new object();
		private static readonly object createCustomizationForm = new object();
		private static readonly object highlightedLinkChanged = new object();
		private static readonly object pressedLinkChanged = new object();
		private static readonly object queryShowPopupMenu = new object();
		private static readonly object customDrawItem = new object();
		private static readonly object controllerChanged = new object();
		internal static MyDebug Debug = new MyDebug();
		internal const int DragCursor = 0, CopyCursor = 1, NoDropCursor = 2, EditSizingCursor = 3;
		static BarManager() {
			DevExpress.Utils.Design.DXAssemblyResolver.Init();
			managers = new ArrayList();
			messageFilter = new BarManagerMessageFilter();
			BarEditorsRepositoryItemRegistrator.Register();
			MenuManagerHelper.RegisterFindRoutine(FindManager);
			ComponentLocator.RegisterFindRoutine(FindManager);
		}
		internal bool designTimeManager = false;
		internal BarManager(bool designTimeManager)
			: this() {
			this.designTimeManager = designTimeManager;
		}
		internal bool Active {
			get { return active; }
			set { active = value; }
		}
		public BarManager(IContainer container)
			: this() {
			container.Add(this);
		}
		public BarManager() {
			Initialize();
			Permissions.Request();
			this.components = new System.ComponentModel.Container();
			this.controller = null;
			this.dockManager = null;
			this.cachedForms = new Hashtable();
			this.threadId = DevExpress.Utils.Win.Hook.HookManager.GetCurrentThreadId();
			this.closeButtonAffectAllTabs = true;
			this.mainMenu = this.statusBar = null;
			this.useAltKeyForMenu = useF10KeyForMenu = true;
			this.showCloseButton = false;
			this.helper = CreateHelpers();
			this.tabFont = Control.DefaultFont;
			this.destroying = false;
			this.ignoreLeftMouseUp = this.ignoreMouseUp = 0;
			PrimaryShortcutProcessor = XtraBars.PrimaryShortcutProcessor.Editor;
			inClearSelection = 0;
			popupContextMenus = new Hashtable();
			customizationProperties = null;
			lock(managers.SyncRoot) {
				managers.Add(this);
				if(managers.Count == 1) {
					messageFilter.InstallHook();
				}
			}
			this.mdiMenuMergeStyle = BarMdiMenuMergeStyle.Always;
			this.showFullMenusAfterDelay = true;
			this.showFullMenus = false;
			this.processShortcutsWhenInvisible = true;
			this.showScreenTipsInToolbars = this.showShortcutInScreenTips = true;
			this.restoring = this.storing = this.autoSaveInRegistry = false;
			this.allowQuickCustomization = this.allowCustomization = true;
			this.allowMoveBarOnToolbar = true;
			this.allowShowToolbarsPopup = true;
			this.registryPath = "";
			this.maxItemId = 0;
			this.isBarsActive = true;
			this.dockingEnabled = true;
			this.toolBarsPopup = null;
			this.itemHolders = new ArrayList();
			this.dragStartPoint = Point.Empty;
			this.selectionInfo = CreateSelectionInfo();
			this.categories = new BarManagerCategoryCollection(this);
			this.categories.CollectionChange += new CollectionChangeEventHandler(OnCategoriesChange);
			this.items = CreateItems();
			this.bars = new Bars(this);
			this.dockWindows = new BarDockWindowCollection();
			this.dockControls = new BarDockControls(this);
			this.allowHtmlText = false;
			this.listeners = CreateListenersCollection();
			this.internalItems = new BarManagerInternalItems(this);
			this.form = null;
			this.largeImages = this.images = null;
			this.collectionSizeMode = SharedImageCollectionImageSizeMode.UseCollectionImageSize;
			Helper.LoadHelper.Loaded = true;
			HideBarsWhenMerging = true;
			GetController().AddClient(this);
			AddHighlightedLinkChangedEvent();
		}
		protected virtual void Initialize() {
		}
		bool lockMenu;
		internal bool LockMenu { get { return lockMenu; } set { lockMenu = value; } }
		protected internal virtual void FillAdditionalBarItemInfoCollection(BarItemInfoCollection coll) { }
		IDXMenuManager IDXMenuManager.Clone(Form newForm) {
			BarManager manager = new BarManager();
			manager.RightToLeft = IsRightToLeft ? DefaultBoolean.True : DefaultBoolean.False;
			manager.DockingEnabled = false;
			manager.Controller = GetController();
			manager.Form = newForm;
			return manager;
		}
		void IDXMenuManager.DisposeManager() {
			RemoveHighlightedLinkChanged();
			Dispose();
		}
		void ResetOptionsLayout() { OptionsLayout.Reset(); }
		bool ShouldSerializeOptionsLayout() { return OptionsLayout.ShouldSerializeCore(); }
		BarManagerOptionsLayout optionsLayout;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarManagerOptionsLayout OptionsLayout {
			get {
				if(optionsLayout == null)
					optionsLayout = new BarManagerOptionsLayout();
				return optionsLayout;
			}
		}
		protected virtual BarManagerHelpers CreateHelpers() { return new BarManagerHelpers(this); }
		protected void OnForm_WindowProc(object sender, ref Message msg) {
			if(msg.Msg != 0x86) return; 
			if(SelectionInfo.showingEditor) msg.WParam = new IntPtr(1);
		}
		protected virtual void OnMyPropertyChanged() { }
		private PopupMenuAlignment popupMenuAlighment = PopupMenuAlignment.Default;
		[DefaultValue(PopupMenuAlignment.Default), XtraSerializableProperty]
		public PopupMenuAlignment PopupMenuAlignment {
			get { return popupMenuAlighment; }
			set {
				if(PopupMenuAlignment == value)
					return;
				popupMenuAlighment = value;
				OnPopupMenuAlignmentChanged();
			}
		}
		protected virtual void OnPopupMenuAlignmentChanged() { }
		protected internal virtual LeftRightAlignment GetPopupMenuAlignment() {
			LeftRightAlignment res = SystemInformation.PopupMenuAlignment;
			if(PopupMenuAlignment == XtraBars.PopupMenuAlignment.Left)
				res = LeftRightAlignment.Left;
			if(PopupMenuAlignment == XtraBars.PopupMenuAlignment.Right)
				res = LeftRightAlignment.Right;
			if(IsRightToLeft) {
				if(res == LeftRightAlignment.Left) return LeftRightAlignment.Right;
				if(res == LeftRightAlignment.Right) return LeftRightAlignment.Left;
			}
			return res;
		}
		protected virtual BarSelectionInfo CreateSelectionInfo() { return new BarSelectionInfo(this); }
		protected virtual BarItems CreateItems() { return new BarItems(this); }
		protected virtual BarManagerListenerCollection CreateListenersCollection() { return new BarManagerListenerCollection(); }
		protected internal virtual void AddListener(IBarManagerListener listener) {
			if(Listeners != null)
				Listeners.Add(listener);
		}
		protected internal virtual void RemoveListener(IBarManagerListener listener) {
			if(Listeners != null)
				Listeners.Remove(listener);
		}
		protected internal BarManagerListenerCollection Listeners { get { return listeners; } }
		[Browsable(false)]
		public virtual Bar CreateDesignTimeToolbar() {
			return new Bar();
		}
		bool allowGlyphSkinning = false;
		[DefaultValue(false)]
		public virtual bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				OnControllerChanged();
			}
		}
		[DefaultValue(false)]
		public bool ShowScreenTipsInMenus { get; set; }
		protected virtual Hashtable CachedForms { get { return cachedForms; } }
		protected internal virtual ImageCollection BarItemsImages {
			get {
				if(barItemsImages == null) {
					System.IO.Stream str = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraBars.BarItems.bmp");
					this.barItemsImages = new ImageCollection();
					this.barItemsImages.TransparentColor = Color.Magenta;
					this.barItemsImages.Images.AddImageStrip(ImageTool.ImageFromStream(str));
				}
				return barItemsImages;
			}
		}
		ToolTipAnchor toolTipAnchor = ToolTipAnchor.Cursor;
		[DefaultValue(ToolTipAnchor.Cursor)]
		public ToolTipAnchor ToolTipAnchor {
			get { return toolTipAnchor; }
			set { toolTipAnchor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarManager MergedOwner { get; internal set; }
		protected internal virtual bool AllowHotCustomization { get { return true; } }
		bool IExtenderProvider.CanExtend(object target) {
			if(target is Control && !(target is PopupMenuBase)) {
				if(target is IDXManagerPopupMenu) return false;
				return true;
			}
			return false;
		}
		[DefaultValue(null), Category("BarManager")]
		public PopupMenuBase GetPopupContextMenu(Control control) {
			if(control == null) return null;
			if(control.Parent != null && (control.Parent is DevExpress.XtraEditors.BaseEdit))
				control = control.Parent;
			PopupMenuBase menu = (PopupMenuBase)PopupContextMenus[control];
			return menu;
		}
		public void SetPopupContextMenu(Control control, PopupMenuBase menu) {
			if(menu == null && control != null)
				PopupContextMenus.Remove(control);
			else
				PopupContextMenus[control] = menu;
		}
		internal Form OriginalForm {
			get {
				if(originalForm == null) {
					if(Form == null) return null;
					originalForm = GetForm();
				}
				return originalForm;
			}
		}
		internal int ThreadId { get { return threadId; } }
		internal void RemoveContextMenu(PopupMenuBase menu) {
			while(PopupContextMenus.ContainsValue(menu)) {
				object key = null;
				foreach(DictionaryEntry entry in PopupContextMenus) {
					if(entry.Value == menu) {
						key = entry.Key;
						break;
					}
				}
				if(key == null) break;
				PopupContextMenus[key] = null;
			}
		}
		#region public persistProperties
		[Browsable(false), DefaultValue(null)]
		public DockManager DockManager {
			get { return dockManager; }
			set {
				dockManager = value;
				if(dockManager != null)
					dockManager.MenuManager = this;
				if(DockManager != null && IsLoading ) DockManager.SuspendOnLoad();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string LayoutVersion {
			get { return OptionsLayout.LayoutVersion; }
			set { OptionsLayout.LayoutVersion = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete()]
		public virtual bool SubmenuHasShadow {
			get { return GetController().PropertiesBar.SubmenuHasShadow; }
			set { GetController().PropertiesBar.SubmenuHasShadow = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete()]
		public virtual bool AllowLinkLighting {
			get { return GetController().PropertiesBar.AllowLinkLighting; }
			set { GetController().PropertiesBar.AllowLinkLighting = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete()]
		public virtual string PaintStyleName {
			get { return GetController().PaintStyleName; }
			set { GetController().PaintStyleName = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerCloseButtonAffectAllTabs"),
#endif
 DefaultValue(true), XtraSerializableProperty(), Category("Behavior")]
		public virtual bool CloseButtonAffectAllTabs {
			get { return closeButtonAffectAllTabs; }
			set {
				closeButtonAffectAllTabs = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerRightToLeft"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), Category("Behavior")]
		public virtual DefaultBoolean RightToLeft {
			get { return rightToLeft; }
			set {
				if(value == RightToLeft) return;
				rightToLeft = value;
				CheckRightToLeft();
			}
		}
		bool isRightToLeft = false;
		protected internal void CheckRightToLeft() {
			if(UpdateRightToLeft()) LayoutChanged();
		}
		protected internal bool UpdateRightToLeft() {
			bool rightToLeft = false;
			if(RightToLeft == DefaultBoolean.True) {
				rightToLeft = true;
			}
			else {
				if(RightToLeft == DefaultBoolean.Default) {
					if(OForm != null) rightToLeft = WindowsFormsSettings.GetIsRightToLeft(OForm);
				}
			}
			if(rightToLeft == isRightToLeft) return false;
			this.isRightToLeft = rightToLeft;
			return true;
		}
		internal bool IsRightToLeft { get { return isRightToLeft; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerMdiMenuMergeStyle"),
#endif
 DefaultValue(BarMdiMenuMergeStyle.Always), Category("Behavior")]
		public virtual BarMdiMenuMergeStyle MdiMenuMergeStyle {
			get { return mdiMenuMergeStyle; }
			set {
				if(MdiMenuMergeStyle == value) return;
				mdiMenuMergeStyle = value;
			}
		}
		#region Obsolete properties
		[Obsolete(BarsObsoleteText.SRObsoleteManager), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image SubMenuBackgroundImage {
			get { return GetController().AppearancesBar.SubMenu.Menu.Image; }
			set { GetController().AppearancesBar.SubMenu.Menu.Image = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteManager), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public Color SubMenuBackColor {
			get { return GetController().AppearancesBar.SubMenu.Menu.BackColor; }
			set { GetController().AppearancesBar.SubMenu.Menu.BackColor = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteManager), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public Color SubMenuGlyphBackColor {
			get { return GetController().AppearancesBar.SubMenu.SideStrip.BackColor; }
			set { GetController().AppearancesBar.SubMenu.SideStrip.BackColor = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteManager), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public Color SubMenuNonRecentGlyphBackColor {
			get { return GetController().AppearancesBar.SubMenu.SideStripNonRecent.BackColor; }
			set { GetController().AppearancesBar.SubMenu.SideStripNonRecent.BackColor = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteManager), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public Color BarBackColor {
			get { return GetController().AppearancesBar.Bar.BackColor; }
			set { GetController().AppearancesBar.Bar.BackColor = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteManager), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public Color MainMenuBarBackColor {
			get { return GetController().AppearancesBar.MainMenu.BackColor; }
			set { GetController().AppearancesBar.MainMenu.BackColor = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteManager), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public Color StatusBarBackColor {
			get { return GetController().AppearancesBar.StatusBar.BackColor; }
			set { GetController().AppearancesBar.StatusBar.BackColor = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteManager), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int BarItemVertIndent {
			get { return GetController().PropertiesBar.BarItemVertIndent; }
			set { GetController().PropertiesBar.BarItemVertIndent = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteManager), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int BarItemHorzIndent {
			get { return GetController().PropertiesBar.BarItemHorzIndent; }
			set { GetController().PropertiesBar.BarItemHorzIndent = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteManager), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Font ItemsFont {
			get { return GetController().AppearancesBar.ItemsFont; }
			set { GetController().AppearancesBar.ItemsFont = value; }
		}
		#endregion  Obsolete properties
		#region Obsolete methods
		[Obsolete(), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetStyleDefaults() { }
		#endregion Obsolete methods
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public int MostRecentItemsPercent {
			get { return GetController().PropertiesBar.MostRecentItemsPercent; }
			set { GetController().PropertiesBar.MostRecentItemsPercent = value; }
		}
		internal bool ShouldSerializeDockWindowTabFont() { return DockWindowTabFont != Control.DefaultFont && !DockWindowTabFont.Equals(Control.DefaultFont); }
		internal void ResetDockWindowTabFont() { DockWindowTabFont = Control.DefaultFont; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerDockWindowTabFont"),
#endif
 Category("Appearance")]
		public virtual Font DockWindowTabFont {
			get { return tabFont; }
			set {
				if(value == null) return;
				if(tabFont == value) return;
				tabFont = value;
				LayoutChangedCore();
			}
		}
		[System.ComponentModel.Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerTransparentEditors"),
#endif
 DefaultValue(false)]
		public virtual bool TransparentEditors {
			get { return transparentEditors; }
			set {
				if(TransparentEditors == value) return;
				transparentEditors = value;
				LayoutChangedCore();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		System.ComponentModel.RefreshProperties(RefreshProperties.All)]
		public int MaxItemId {
			get { return maxItemId; }
			set {
				if(restoring) {
					if(value < maxItemId) value = maxItemId;
				}
				maxItemId = value;
				FireManagerChanged();
			}
		}
		[DefaultValue(true)]
		public bool HideBarsWhenMerging {
			get;
			set;
		}
		protected internal bool HasMergeEventSubscription {
			get { return (Events[merge] != null) && (Events[unMerge] != null); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerDockingEnabled"),
#endif
 DefaultValue(true), Category("Behavior")]
		public virtual bool DockingEnabled {
			get { return dockingEnabled; }
			set {
				if(DockingEnabled == value) return;
				dockingEnabled = value;
				if(DockingEnabled) {
					Helper.DockingHelper.CreateDefaultDockControls();
					Helper.DockingHelper.CheckDockControlsAdded();
				}
				else {
					Helper.DockingHelper.RemoveDefaultDockControls();
					ClearBars();
				}
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), ListBindable(false)]
		public BarDockControls DockControls { get { return dockControls; } }
		internal object XtraFindBarsItem(XtraItemEventArgs e) {
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo pi = e.Item.ChildProperties["BarName"];
			if(pi == null) pi = e.Item.ChildProperties["Name"];
			if(pi == null) return null;
			Bar bar = null;
			if(pi.Value != null) bar = Bars[pi.Value.ToString()]; else return null;
			if(bar == null) {
				return new Bar(this);
			}
			return bar;
		}
		void XtraIsOldItemBars(XtraOldItemEventArgs e) {
			e.OldItem = true;
		}
		BarItemLinkId XtraCreateItemLinkIdCollectionItem(XtraItemEventArgs e) {
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo pi = e.Item.ChildProperties["Id"];
			return new BarItemLinkId() { Id = pi.Value.ToString() };
		}
		void XtraSetIndexItemLinkIdCollectionItem(XtraSetItemIndexEventArgs e) {
			ItemLinkIdCollection.Add((BarItemLinkId)e.Item.Value);
		}
		BarItemLinkIdCollection itemLinkIdCollection;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 0, XtraSerializationFlags.None)]
		public BarItemLinkIdCollection ItemLinkIdCollection { 
			get { 
				if(itemLinkIdCollection == null)
					itemLinkIdCollection = new BarItemLinkIdCollection();
				return itemLinkIdCollection;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), XtraSerializableProperty(XtraSerializationVisibility.Collection, false, true, false, true, 1, XtraSerializationFlags.None)]
		public Bars Bars { get { return bars; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerDockWindows"),
#endif
 Obsolete(), EditorBrowsable(EditorBrowsableState.Never)]
		public BarDockWindowCollection DockWindows { get { return dockWindows; } }
		internal bool ShouldSerializeForm() { return form != null; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerForm"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public virtual Control Form {
			get { return form; }
			set {
				if(value == null) return;
				SetForm(value);
			}
		}
		protected internal virtual Control FilterForm {
			get { return Form; }
		}
		protected virtual bool IsRibbonManager { get { return false; } }
		protected virtual void SetForm(Control value) {
			if(Form == value) return;
			this.originalForm = null;
			Control prev = Form;
			Form prevForm = prev as Form;
			XtraForm xf = Form as XtraForm;
			if(Form != null) {
				Form.HandleDestroyed -= OnFormHandleDestroyed;
				if(!IsRibbonManager) {
					if(xf != null)
						xf.FormLayoutChanged -= OnFormLayoutChanged;
					else
						Form.Layout -= OnFormLayoutChanged;
					Form.Resize -= OnFormResize;
					Form.VisibleChanged -= OnFormVisibleChanged;
					Form.ParentChanged -= OnFormParentChanged;
					Form.RightToLeftChanged -= OnFormRightToLeftChanged;
					if(Form is Form) {
						formClosed = false;
						(Form as Form).Closed -= OnFormClosed;
						(Form as Form).MdiChildActivate -= OnFormMdiChildActivate;
					}
					Form.Disposed -= OnFormDisposed;
				}
				Form.BindingContextChanged -= OnFormBindingContextChanged;
				Form.HandleCreated -= OnFormHandlerCreated;
			}
			Form currentForm = Form as Form;
			if(currentForm != null) {
				currentForm.Shown += OnFormShown;
			}
			this.formHandleDestroyed = false;
			form = value;
			xf = Form as XtraForm;
			if(prev != null)
				DevExpress.Utils.Win.Hook.ControlWndHook.RemoveHook(prev, new DevExpress.Utils.Win.Hook.MsgEventHandler(OnForm_WindowProc), null);
			if(form != null)
				DevExpress.Utils.Win.Hook.ControlWndHook.AddHook(form, new DevExpress.Utils.Win.Hook.MsgEventHandler(OnForm_WindowProc), null);
			if(Form != null) {
				Form.HandleDestroyed += OnFormHandleDestroyed;
				messageFilter.AddHook(this);
				if(!IsRibbonManager) {
					if(xf != null)
						xf.FormLayoutChanged += OnFormLayoutChanged;
					else
						Form.Layout += OnFormLayoutChanged;
					Form.Resize += OnFormResize;
					Form.VisibleChanged += OnFormVisibleChanged;
					Form.ParentChanged += OnFormParentChanged;
					Form.RightToLeftChanged += OnFormRightToLeftChanged;
					if(!IsDesignMode && !IsPartialDesignMode)
						Form.Disposed += OnFormDisposed;
					if(Form is Form) {
						(Form as Form).Closed += OnFormClosed;
						(Form as Form).MdiChildActivate += OnFormMdiChildActivate;
					}
				}
				Form.HandleCreated += OnFormHandlerCreated;
				Form.BindingContextChanged += OnFormBindingContextChanged;
			}
			if(prevForm != null)
				prevForm.Shown -= OnFormShown;
			if(!IsLoading) {
				if(Form == null)
					Helper.DockingHelper.RemoveDefaultDockControls();
				else {
					Helper.DockingHelper.CreateDefaultDockControls();
					Helper.DockingHelper.CheckDockControlsAdded();
				}
			}
			UpdateRightToLeft();
		}
		bool formShown = false;
		protected internal bool IsFormShown { get { return !(Form is Form) || formShown; } }
		void OnFormShown(object sender, EventArgs e) {
			this.formShown = true;
		}
		void OnFormParentChanged(object sender, EventArgs e) {
			if(IsLoading)
				return;
			Helper.DockingHelper.CheckDockControlsAdded();
			if(Form is Form) UpdateMainMenuVisibility();
			UpdateBarsVisibilityByMergeType();
		}
		void OnFormRightToLeftChanged(object sender, EventArgs e) {
			UpdateRightToLeft();
		}
		void OnFormBindingContextChanged(object sender, EventArgs e) {
			foreach(BarItem item in Items)
				item.OnBindingContextChanged();
		}
		protected virtual void UpdateBarsVisibilityByMergeType() {
			foreach(Bar bar in Bars) {
				bar.UpdateIsMdiChildBar();
			}
		}
		internal static bool IsFormHierarchyDisposing(Control ctrl) {
			while(ctrl != null) {
				if(ctrl.Disposing)
					return true;
				if(ctrl is Form)
					break;
				ctrl = ctrl.Parent;
			}
			return false;
		}
		bool formHandleDestroyed = false;
		void OnFormHandleDestroyed(object sender, EventArgs e) {
			this.formHandleDestroyed = true;
		}
		internal Form OForm {
			get {
				Form frm = GetForm();
				if(frm != null) {
					if(frm.MdiParent != null) return frm.MdiParent;
				}
				return frm;
			}
		}
		List<CustomControl> barControls = new List<CustomControl>();
		protected internal List<CustomControl> BarControls {
			get { return barControls; }
		}
		protected override void OnChangeToolTipController(ToolTipController old, ToolTipController newController) {
			base.OnChangeToolTipController(old, newController);
			if(old == null) old = ToolTipController.DefaultController;
			if(old != null) {
				foreach(CustomControl control in BarControls) {
					old.RemoveClientControl(control);
				}
			}
			if(newController != null) {
				foreach(CustomControl control in BarControls) {
					newController.AddClientControl(control);
				}
			}
		}
		protected internal virtual Control GetTopMostControl() {
			Form res = GetForm();
			if(res != null) return res;
			if(FilterForm != null) {
				if(FilterForm.TopLevelControl != null) return FilterForm.TopLevelControl;
				if(FilterForm.Parent != null) return FilterForm.Parent;
				return FilterForm;
			}
			return null;
		}
		internal virtual UserControl GetUserControl() {
			return Form as UserControl;
		}
		internal virtual IBindableComponent GetParentBindableComponent() {
			return (Form != null && Form.IsHandleCreated) ? Form : null;
		}
		protected internal virtual Form GetForm() {
			if(Form == null) return null;
			if(Form is Form) return Form as Form;
			ContainerControl container = Form as ContainerControl;
			if(container != null) return container.ParentForm;
			return Form.FindForm();
		}
		class DocumentFloatingContext : IDocumentFloatingContext {
			#region static
			static readonly object syncObj = new object();
			static System.Collections.Generic.IDictionary<BarManager, IDocumentFloatingContext> contexts =
				new System.Collections.Generic.Dictionary<BarManager, IDocumentFloatingContext>();
			internal static IDocumentFloatingContext Create(BarManager manager) {
				lock(syncObj) {
					IDocumentFloatingContext context;
					if(!contexts.TryGetValue(manager, out context))
						context = new DocumentFloatingContext(manager);
					else ((DocumentFloatingContext)context).refCounter++;
					return context;
				}
			}
			#endregion static
			int refCounter;
			BarManager manager;
			Docking2010.Views.BaseDocument documentCore;
			DocumentFloatingContext(BarManager manager) {
				this.manager = manager;
				if(0 == refCounter++)
					contexts.Add(manager, this);
			}
			void IDisposable.Dispose() {
				lock(syncObj) {
					if(--refCounter == 0) {
						contexts.Remove(manager);
						this.manager = null;
						this.documentCore = null;
					}
				}
			}
			BarManager IDocumentFloatingContext.BarManager {
				get { return manager; }
			}
			Docking2010.Views.BaseDocument IDocumentFloatingContext.Document {
				get { return documentCore; }
			}
			void IDocumentFloatingContext.SetDocument(Docking2010.Views.BaseDocument document) {
				documentCore = document;
			}
		}
		protected internal virtual IDocumentFloatingContext CreateDocumentFloatingContext() {
			return DocumentFloatingContext.Create(this);
		}
		protected internal virtual bool GetIsMdiChildManager() {
			return GetIsMdiChild(GetDocument());
		}
		protected internal virtual Docking2010.Views.BaseDocument GetDocument() {
			if(Form != null) {
				var documentContainer = Docking2010.DocumentContainer.FromControl(Form);
				if(documentContainer != null)
					return documentContainer.Document;
				var documentForm = Form.Parent as Docking2010.FloatDocumentForm;
				if(documentForm != null)
					return documentForm.Document;
			}
			return null;
		}
		protected bool GetIsMdiChild(Docking2010.Views.BaseDocument document) {
			DocumentManager manager = document != null ? document.Manager : null;
			if(manager != null && manager.RibbonAndBarsMergeStyle == Docking2010.Views.RibbonAndBarsMergeStyle.WhenNotFloating) {
				return !document.IsFloating;
			}
			else if(document != null && updatingMainMenuVisibility == 0) {
				if(!document.IsInitializing && !document.IsControlLoading) return false;
			}
			return GetIsMdiChildDocumentCore(document);
		}
		protected bool GetIsMdiChildDocumentCore(Docking2010.Views.BaseDocument document) {
			using(IDocumentFloatingContext context = CreateDocumentFloatingContext()) {
				if(document == null)
					document = context.Document;
				return (document != null) && (document.Manager != null) && document.Manager.CanMergeOnDocumentActivate();
			}
		}
		protected internal virtual Docking2010.DocumentContainer GetDocumentContainer() {
			if(Form == null) return null;
			return Docking2010.DocumentContainer.FromControl(Form);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerShowFullMenus"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool ShowFullMenus {
			get { return showFullMenus; }
			set {
				showFullMenus = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerShowFullMenusAfterDelay"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowFullMenusAfterDelay {
			get { return showFullMenusAfterDelay; }
			set {
				showFullMenusAfterDelay = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerShowShortcutInScreenTips"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowShortcutInScreenTips {
			get { return showShortcutInScreenTips; }
			set {
				showShortcutInScreenTips = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerShowScreenTipsInToolbars"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowScreenTipsInToolbars {
			get { return showScreenTipsInToolbars; }
			set {
				showScreenTipsInToolbars = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAllowMoveBarOnToolbar"),
#endif
 DefaultValue(true)]
		public bool AllowMoveBarOnToolbar {
			get { return allowMoveBarOnToolbar; }
			set {
				if(value == AllowMoveBarOnToolbar) return;
				allowMoveBarOnToolbar = value;
				if(!IsLoading)
					Helper.DockingHelper.UpdateBarDocking();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAllowShowToolbarsPopup"),
#endif
 DefaultValue(true)]
		public bool AllowShowToolbarsPopup {
			get { return allowShowToolbarsPopup; }
			set {
				allowShowToolbarsPopup = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAllowCustomization"),
#endif
 DefaultValue(true)]
		public bool AllowCustomization {
			get { return allowCustomization; }
			set {
				allowCustomization = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerUseAltKeyForMenu"),
#endif
 DefaultValue(true)]
		public bool UseAltKeyForMenu {
			get { return useAltKeyForMenu; }
			set {
				useAltKeyForMenu = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerUseF10KeyForMenu"),
#endif
 DefaultValue(true)]
		public bool UseF10KeyForMenu {
			get { return useF10KeyForMenu; }
			set {
				useF10KeyForMenu = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowMergeInvisibleLinks {
			get { return allowMergeInvisibleLinks; }
			set { allowMergeInvisibleLinks = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAllowItemAnimatedHighlighting"),
#endif
 DefaultValue(true)]
		public virtual bool AllowItemAnimatedHighlighting {
			get { return allowItemAnimatedHighlighting; }
			set { allowItemAnimatedHighlighting = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerShowCloseButton"),
#endif
 DefaultValue(false)]
		public bool ShowCloseButton {
			get { return showCloseButton; }
			set {
				if(ShowCloseButton == value) return;
				showCloseButton = value;
				if(IsLoading) return;
				if(MainMenu != null) MainMenu.OnBarChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAllowQuickCustomization"),
#endif
 DefaultValue(true)]
		public bool AllowQuickCustomization {
			get { return allowQuickCustomization; }
			set {
				if(AllowQuickCustomization == value) return;
				allowQuickCustomization = value;
				LayoutChangedCore();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAutoSaveInRegistry"),
#endif
 DefaultValue(false)]
		public bool AutoSaveInRegistry {
			get { return autoSaveInRegistry; }
			set {
				autoSaveInRegistry = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerRegistryPath"),
#endif
 DefaultValue("")]
		public string RegistryPath {
			get { return registryPath; }
			set {
				if(value != null)
					registryPath = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerLargeImages"),
#endif
 DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object LargeImages {
			get { return largeImages; }
			set {
				largeImages = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerImages"),
#endif
 DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object Images {
			get { return images; }
			set {
				images = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerSharedImageCollectionImageSizeMode"),
#endif
 DefaultValue(SharedImageCollectionImageSizeMode.UseCollectionImageSize)]
		public SharedImageCollectionImageSizeMode SharedImageCollectionImageSizeMode {
			get { return collectionSizeMode; }
			set {
				if(collectionSizeMode == value) return;
				collectionSizeMode = value;
			}
		}
		[DefaultValue(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ProcessShortcutsWhenInvisible {
			get { return processShortcutsWhenInvisible; }
			set { processShortcutsWhenInvisible = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(true), Browsable(false), MergableProperty(false)]
		public BarManagerCategoryCollection Categories { get { return categories; } }
		internal bool XtraShouldSerializeCollectionItemsItem(XtraItemEventArgs e) {
			if(e.Item.Value is BarLinkContainerItem) return true;
			return false;
		}
		internal object XtraFindItemsItem(XtraItemEventArgs e) {
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo pi = e.Item.ChildProperties["Id"];
			if(pi != null) return Items.FindById(Convert.ToInt32(pi.Value));
			return null;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), XtraSerializableProperty(false, true, false, -2)]
		public BarItems Items { get { return items; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(BarsObsoleteText.SRObsoleteEditorsRepository)]
		public PersistentRepository EditorsRepository {
			get { return null; }
			set { }
		}
		#endregion
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
		public override bool IsLoading {
			get {
				if(base.IsLoading || initCount != 0) return true;
				IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null && host.Loading) return true;
				return false;
			}
		}
		protected internal int IgnoreMouseUp {
			get { return ignoreMouseUp; }
			set { ignoreMouseUp = value; }
		}
		protected internal int IgnoreLeftMouseUp {
			get { return ignoreLeftMouseUp; }
			set { ignoreLeftMouseUp = value; }
		}
		protected internal Hashtable PopupContextMenus { get { return popupContextMenus; } }
		protected internal Hashtable CustomizationProperties {
			get { return customizationProperties; }
			set {
				customizationProperties = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerController"),
#endif
 DefaultValue(null)]
		public virtual BarAndDockingController Controller {
			get { return controller; }
			set {
				if(Controller == value) return;
				GetControllerInternal().RemoveClient(this);
				this.controller = value;
				GetControllerInternal().AddClient(this);
				OnControllerChanged(true);
			}
		}
		protected virtual void OnControllerChanged(bool setNewController) {
			OnControllerChanged();
		}
		bool allowHtmlText;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAllowHtmlText"),
#endif
 DefaultValue(false)]
		public virtual bool AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(allowHtmlText == value) return;
				allowHtmlText = value;
				OnControllerChanged();
			}
		}
		BarAndDockingController GetControllerInternal() {
			return controller != null ? controller : BarAndDockingController.Default;
		}
		public virtual BarAndDockingController GetController() {
			return Controller == null || Controller.Disposing ? BarAndDockingController.Default : Controller;
		}
		protected internal virtual BarManagerProperties GetProperties() {
			return GetController().PropertiesBar;
		}
		protected internal virtual BarManagerPaintStyle PaintStyle { get { return GetController().PaintStyle; } }
		public int GetNewItemId() {
			int res = MaxItemId++;
			FireManagerChanged();
			return res;
		}
		#region public properties
		[Browsable(false)]
		public virtual DevExpress.XtraEditors.BaseEdit ActiveEditor { get { return EditorHelper.ActiveEditor; } }
		[Browsable(false)]
		public virtual BarEditItemLink ActiveEditItemLink { get { return SelectionInfo.EditingLink; } }
		[Browsable(false)]
		public virtual BarItemLink HighlightedLink { get { return SelectionInfo.HighlightedLink; } }
		[Browsable(false)]
		public virtual BarItemLink PressedLink { get { return SelectionInfo.PressedLink; } }
		[Browsable(false)]
		public virtual BarItemLink CustomizeSelectedLink { get { return SelectionInfo.CustomizeSelectedLink; } }
		[Browsable(false)]
		public virtual BarItemLink KeyboardHighlightedLink { get { return SelectionInfo.KeyboardHighlightedLink; } }
		[Browsable(false)]
		public virtual Form ActiveMdiChild {
			get {
				if(MdiParent != null) return MdiParent.ActiveMdiChild;
				return null;
			}
		}
		[Browsable(false)]
		public virtual bool CanShowNonRecentItems {
			get {
				return SelectionInfo.ShowNonRecentItems || IsCustomizing || ShowFullMenus;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public bool LargeIcons {
			get { return GetProperties().LargeIcons; }
			set {
				GetProperties().LargeIcons = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public AnimationType MenuAnimationType {
			get { return GetProperties().MenuAnimationType; }
			set {
				GetProperties().MenuAnimationType = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerMainMenu"),
#endif
 Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null)]
		public virtual Bar MainMenu {
			get { return mainMenu; }
			set {
				if(value != null && (value.Manager != this && !IsLoading)) value = null;
				if(MainMenu == value) return;
				mainMenu = value;
				UpdateMainMenuProperties();
				LayoutChangedCore();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerStatusBar"),
#endif
 Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null)]
		public virtual Bar StatusBar {
			get { return statusBar; }
			set {
				if(value != null && (value.Manager != this && !IsLoading)) value = null;
				if(statusBar == value) return;
				statusBar = value;
				UpdateStatusBarProperties();
				LayoutChangedCore();
			}
		}
		public virtual void PerformClick(BarItem item, BarItemLink link) {
			item.PerformClick(link);
		}
		public virtual void PerformClick(BarItem item) {
			item.PerformClick();
		}
		protected virtual void UpdateMainMenuProperties() {
			if(IsLoading || MainMenu == null) return;
			MainMenu.OptionsBar.BeginUpdate();
			try {
				MainMenu.OptionsBar.Hidden = false;
				MainMenu.OptionsBar.UseWholeRow = true;
				MainMenu.OptionsBar.MultiLine = true;
				MainMenu.Offset = 0;
			}
			finally {
				MainMenu.OptionsBar.EndUpdate();
			}
		}
		protected virtual void UpdateStatusBarProperties() {
			if(IsLoading || StatusBar == null) return;
			StatusBar.OptionsBar.BeginUpdate();
			try {
				StatusBar.OptionsBar.AllowQuickCustomization = false;
				StatusBar.OptionsBar.DrawDragBorder = false;
				StatusBar.OptionsBar.UseWholeRow = true;
				StatusBar.Offset = 0;
			}
			finally {
				StatusBar.OptionsBar.EndUpdate();
			}
			StatusBar.CanDockStyle = BarCanDockStyle.Bottom;
			StatusBar.DockStyle = BarDockStyle.Bottom;
		}
		[Browsable(false)]
		public virtual int SubMenuOpenCloseInterval { get { return 400; } }
		[Browsable(false)]
		public virtual bool IsDesignMode {
			get {
				if(DesignMode) return true;
				if(Form != null && Form.Site != null && Form.Site.DesignMode) {
					if(this.designTimeManager) return false;
					return true;
				}
				return false;
			}
		}
		protected internal bool IsOnTopMostForm {
			get { return Form != null && Form.FindForm() != null && Form.FindForm().TopMost; }
		}
		protected virtual bool IsPartialDesignMode {
			get {
				if(IsDesignMode && !DesignMode) return true;
				return false;
			}
		}
		[Browsable(false)]
		public virtual bool IsDocking {
			get { return SelectionInfo.DockManager != null && SelectionInfo.DockManager.IsDragging; }
		}
		[Browsable(false)]
		public virtual bool IsCustomizing {
			get { return IsDesignMode || (Helper != null && Helper.CustomizationManager.IsCustomizing); }
		}
		[Browsable(false)]
		public bool IsDragging { get { return Helper.DragManager.IsDragging; } }
		[Browsable(false)]
		public bool IsLinkSizing {
			get { return Helper.DragManager.SizingLink != null; }
		}
		[Browsable(false)]
		public bool IsStoring {
			get { return storing; }
		}
		#endregion
		public virtual void ResetUsageData() {
			foreach(BarItem item in Items) {
				item.ResetUsageData();
			}
		}
		public override void BeginInit() {
			if(Helper.LoadHelper.Loaded) Helper.LoadHelper.Loaded = false;
			base.BeginInit();
		}
		public virtual object InternalGetService(Type type) {
			if(type.Equals(typeof(BarSelectionInfo))) {
				return SelectionInfo;
			}
			if(type.Equals(typeof(IBarObject))) {
				InternalClearManager();
				return this;
			}
			if(type.Equals(typeof(DevExpress.XtraBars.Customization.CustomizationForm)) && Helper.CustomizationManager.CustomizationForm != null)
				return Helper.CustomizationManager.CustomizationForm;
			object service = GetService(type);
			if(service == null && Form != null && Form.Site != null) service = Form.Site.GetService(type);
			return service;
		}
		internal IDesignerHost GetDesignerHost() {
			return InternalGetService(typeof(IDesignerHost)) as IDesignerHost;
		}
		Form onLoadForm = null;
		UserControl onLoadUserControl = null;
		Control onParentChanged = null;
		protected void HookManagerOnLoaded() {
			this.allowDisposeObjects = !(IsDesignMode || IsPartialDesignMode);
			RemoveManagerOnLoadedHook();
			this.onLoadForm = GetForm();
			this.onLoadUserControl = GetUserControl();
			this.onParentChanged = null;
			if(onLoadForm == null && onLoadUserControl == null && Form != null) {
				Form.ParentChanged += OnParentChanged;
				Form.HandleCreated += OnHandleCreated;
				onParentChanged = Form;
				while(onParentChanged.Parent != null) {
					onParentChanged = onParentChanged.Parent;
				}
				if(onParentChanged != Form)
					onParentChanged.ParentChanged += OnParentChanged;
				else
					onParentChanged = null;
			}
			if(onLoadForm != null) {
				onLoadForm.RightToLeftChanged += onLoadForm_RightToLeftChanged;
#if DXWhidbey
				if(onLoadForm.IsHandleCreated && (onLoadForm.Site == null || !onLoadForm.Site.DesignMode || IsPartialDesignMode))
#else
				if(onLoadForm.IsHandleCreated && (onLoadForm.Site == null || !onLoadForm.Site.DesignMode || IsPartialDesignMode))  
#endif
					OnFormLoadComplete(this, EventArgs.Empty);
				else
					onLoadForm.Load += OnFormLoadComplete;
			}
			else if(onLoadUserControl != null) {
#if DXWhidbey
				if(onLoadUserControl.IsHandleCreated && (onLoadUserControl.Site == null || !onLoadUserControl.Site.DesignMode || IsPartialDesignMode))
#else
				if(onLoadUserControl.IsHandleCreated && (onLoadUserControl.Site == null || !onLoadUserControl.Site.DesignMode || IsPartialDesignMode))  
#endif
					OnFormLoadComplete(this, EventArgs.Empty);
				else
					onLoadUserControl.Load += OnFormLoadComplete;
			}
		}
		protected void RemoveManagerOnLoadedHook() {
			if(onLoadForm != null) {
				onLoadForm.Load -= OnFormLoadComplete;
				onLoadForm.RightToLeftChanged -= onLoadForm_RightToLeftChanged;
			}
			this.onLoadForm = null;
			if(onLoadUserControl != null)
				onLoadUserControl.Load -= OnFormLoadComplete;
			this.onLoadUserControl = null;
			if(onParentChanged != null)
				onParentChanged.ParentChanged -= OnParentChanged;
			this.onParentChanged = null;
			if(Form != null) {
				Form.ParentChanged -= OnParentChanged;
				Form.HandleCreated -= OnHandleCreated;
			}
		}
		void onLoadForm_RightToLeftChanged(object sender, EventArgs e) {
			CheckRightToLeft();
		}
		protected virtual void OnHandleCreated(object sender, EventArgs e) {
			if(onLoadForm == null && onLoadUserControl == null)
				HookManagerOnLoaded();
		}
		protected void OnParentChanged(object sender, EventArgs e) {
			if(onLoadForm == null && onLoadUserControl == null)
				HookManagerOnLoaded();
		}
		internal void ForceBaseOnLoaded() {
			base.OnLoaded();
		}
		protected void OnFormLoadComplete(object sender, EventArgs e) {
			if(destroying) return;
			UpdateRightToLeft();
			if(IsDesignMode && IsLoading) return;
			Helper.LoadHelper.Load();
			Helper.DockingHelper.CheckUpdateDockingOrder();
		}
		void IBarManagerControl.DesignerHostLoaded() {
			if(destroying) return;
			Helper.LoadHelper.Load();
			if(!IsDesignMode) Helper.DockingHelper.CheckUpdateDockingOrder();
			if(IsPartialDesignMode) {
				foreach(BarDockControl dock in DockControls) {
					dock.Enabled = false;
				}
				Deactivate();
			}
		}
		public void ForceInitialize() {
			ForceLinkCreate();
		}
		internal bool SuppressUpdateDockIndexes { get; set; }
		public virtual void ForceLinkCreate() {
			Helper.LoadHelper.Load();
		}
		protected override void OnEndInit() {
			bool nullForm = false;
			if(IsDesignMode && Form == null) {
				Form = GetFormFromContainer();
				nullForm = true;
			}
			DrawParameters.UpdateScheme(DesignMode);
			Helper.DockingHelper.CreateDefaultDockControls();
			HookManagerOnLoaded();
			if(nullForm && Form != null) {
				Helper.LoadHelper.Load();
			}
			InitializeSkinBarSubItemsIfExists();
		}
		protected virtual void InitializeSkinBarSubItemsIfExists() {
			if(DesignMode) return;
			for(int i = 0; i < Items.Count; i++) {
				SkinBarSubItem ssi = Items[i] as SkinBarSubItem;
				if(ssi != null)
					ssi.OnInitialize();
			}
		}
		internal Control GetFormFromContainer() {
			if(GetTypeFromContainer(typeof(Form)) != null)
				return GetTypeFromContainer(typeof(Form));
			return GetTypeFromContainer(typeof(UserControl));
		}
		Control GetTypeFromContainer(Type type) {
			foreach(IComponent comp in Container.Components) {
				if(type.IsInstanceOfType(comp))
					return comp as Control;
			}
			return null;
		}
		internal void LoadRepositories() {
			base.OnEndInit();
		}
		internal void FillPostponedRepositories() {
			EditorHelper.FillPostponedRepositories();
		}
		internal void LoadPostponedRepositories() {
			EditorHelper.InitializePostponedRepositories();
		}
		public void Customize() {
			Helper.CustomizationManager.StartCustomization();
		}
		internal Delegate GetEventItemClick() { return Events[itemClick]; }
		internal Delegate GetEventItemPress() { return Events[itemPress]; }
		void AddHighlightedLinkChangedEvent() {
			HighlightedLinkChanged += new HighlightedLinkChangedEventHandler(OnHighlightedLinkChanged);
		}
		void RemoveHighlightedLinkChanged() {
			HighlightedLinkChanged -= new HighlightedLinkChangedEventHandler(OnHighlightedLinkChanged);
		}
		protected virtual void OnHighlightedLinkChanged(object sender, HighlightedLinkChangedEventArgs e) {
			if(e.Link == null || e.Link.BarControl == null) return;
			e.Link.BarControl.AccessibleNotifyClients(AccessibleEvents.Focus, ((BaseLinkAccessible)e.Link.DXAccessible).ID);
		}
		protected internal virtual void RaiseCustomDrawItem(BarItemCustomDrawEventArgs e) {
			BarItemCustomDrawEventHandler handler = Events[customDrawItem] as BarItemCustomDrawEventHandler;
			if(handler != null) handler(this, e);
		}
		public event BarItemCustomDrawEventHandler CustomDrawItem {
			add { Events.AddHandler(customDrawItem, value); }
			remove { Events.RemoveHandler(customDrawItem, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerShortcutItemClick")]
#endif
		public event ShortcutItemClickEventHandler ShortcutItemClick {
			add { this.Events.AddHandler(shortcutItemClick, value); }
			remove { this.Events.RemoveHandler(shortcutItemClick, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerCloseButtonClick")]
#endif
		public event EventHandler CloseButtonClick {
			add { this.Events.AddHandler(closeButtonClick, value); }
			remove { this.Events.RemoveHandler(closeButtonClick, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerMerge")]
#endif
		public event BarManagerMergeEventHandler Merge {
			add { this.Events.AddHandler(merge, value); }
			remove { this.Events.RemoveHandler(merge, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerUnMerge")]
#endif
		public event BarManagerMergeEventHandler UnMerge {
			add { this.Events.AddHandler(unMerge, value); }
			remove { this.Events.RemoveHandler(unMerge, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerItemClick")]
#endif
		public event ItemClickEventHandler ItemClick {
			add { this.Events.AddHandler(itemClick, value); }
			remove { this.Events.RemoveHandler(itemClick, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerItemDoubleClick")]
#endif
		public event ItemClickEventHandler ItemDoubleClick {
			add { this.Events.AddHandler(itemDoubleClick, value); }
			remove { this.Events.RemoveHandler(itemDoubleClick, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerHyperlinkClick")]
#endif
		public event HyperlinkClickEventHandler HyperlinkClick {
			add { this.Events.AddHandler(hyperlinkClick, value); }
			remove { this.Events.RemoveHandler(hyperlinkClick, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerItemPress")]
#endif
		public event ItemClickEventHandler ItemPress {
			add { this.Events.AddHandler(itemPress, value); }
			remove { this.Events.RemoveHandler(itemPress, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerCreateToolbar")]
#endif
		public event CreateToolbarEventHandler CreateToolbar {
			add { this.Events.AddHandler(createToolbar, value); }
			remove { this.Events.RemoveHandler(createToolbar, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerStartCustomization")]
#endif
		public event EventHandler StartCustomization {
			add { this.Events.AddHandler(startCustomization, value); }
			remove { this.Events.RemoveHandler(startCustomization, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerEndCustomization")]
#endif
		public event EventHandler EndCustomization {
			add { this.Events.AddHandler(endCustomization, value); }
			remove { this.Events.RemoveHandler(endCustomization, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerCreateCustomizationForm")]
#endif
		public event CreateCustomizationFormEventHandler CreateCustomizationForm {
			add { this.Events.AddHandler(createCustomizationForm, value); }
			remove { this.Events.RemoveHandler(createCustomizationForm, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerHighlightedLinkChanged")]
#endif
		public event HighlightedLinkChangedEventHandler HighlightedLinkChanged {
			add { this.Events.AddHandler(highlightedLinkChanged, value); }
			remove { this.Events.RemoveHandler(highlightedLinkChanged, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerPressedLinkChanged")]
#endif
		public event HighlightedLinkChangedEventHandler PressedLinkChanged {
			add { this.Events.AddHandler(pressedLinkChanged, value); }
			remove { this.Events.RemoveHandler(pressedLinkChanged, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerQueryShowPopupMenu")]
#endif
		public event QueryShowPopupMenuEventHandler QueryShowPopupMenu {
			add { this.Events.AddHandler(queryShowPopupMenu, value); }
			remove { this.Events.RemoveHandler(queryShowPopupMenu, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerShowToolbarsContextMenu")]
#endif
		public event ShowToolbarsContextMenuEventHandler ShowToolbarsContextMenu {
			add { this.Events.AddHandler(showToolbarsContextMenu, value); }
			remove { this.Events.RemoveHandler(showToolbarsContextMenu, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerLayoutUpgrade")]
#endif
		public event LayoutUpgradeEventHandler LayoutUpgrade {
			add { this.Events.AddHandler(layoutUpgrade, value); }
			remove { this.Events.RemoveHandler(layoutUpgrade, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerBeforeLoadLayout"),
#endif
 Category("Data")]
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { this.Events.AddHandler(beforeLoadLayout, value); }
			remove { this.Events.RemoveHandler(beforeLoadLayout, value); }
		}
		protected internal event EventHandler ControllerChanged {
			add { this.Events.AddHandler(controllerChanged, value); }
			remove { this.Events.RemoveHandler(controllerChanged, value); }
		}
		protected internal virtual void RaiseControllerChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[controllerChanged];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[beforeLoadLayout];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseLayoutUpgrade(LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseShowToolbarsContextMenu(ShowToolbarsContextMenuEventArgs e) {
			ShowToolbarsContextMenuEventHandler handler = (ShowToolbarsContextMenuEventHandler)this.Events[showToolbarsContextMenu];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCreateCustomizationForm(CreateCustomizationFormEventArgs e) {
			CreateCustomizationFormEventHandler handler = (CreateCustomizationFormEventHandler)this.Events[createCustomizationForm];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHighlightedLinkChanged(HighlightedLinkChangedEventArgs e) {
			HighlightedLinkChangedEventHandler handler = (HighlightedLinkChangedEventHandler)this.Events[highlightedLinkChanged];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseItemPress(ItemClickEventArgs e) {
			SelectionInfo.HideToolTip();
			ItemClickEventHandler handler = (ItemClickEventHandler)this.Events[itemPress];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseItemClick(ItemClickEventArgs e) {
			SelectionInfo.HideToolTip();
			ItemClickEventHandler handler = (ItemClickEventHandler)this.Events[itemClick];
			if(handler != null) {
				if(e.Item.ItemClickFireMode != BarItemEventFireMode.Postponed || Form == null)
					handler(this, e);
				else
					Form.BeginInvoke(handler, new object[] { this, e });
			}
		}
		protected internal virtual void RaiseHyperlinkClick(HyperlinkClickEventArgs e) {
			HyperlinkClickEventHandler handler = (HyperlinkClickEventHandler)this.Events[hyperlinkClick];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseItemDoubleClick(ItemClickEventArgs e) {
			ItemClickEventHandler handler = (ItemClickEventHandler)this.Events[itemDoubleClick];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCreateToolbar(CreateToolbarEventArgs e) {
			CreateToolbarEventHandler handler = (CreateToolbarEventHandler)this.Events[createToolbar];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseShortcutItemClick(ShortcutItemClickEventArgs e) {
			ShortcutItemClickEventHandler handler = (ShortcutItemClickEventHandler)this.Events[shortcutItemClick];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseMerge(BarManagerMergeEventArgs e) {
			BarManagerMergeEventHandler handler = (BarManagerMergeEventHandler)this.Events[merge];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseUnMerge(BarManagerMergeEventArgs e) {
			BarManagerMergeEventHandler handler = (BarManagerMergeEventHandler)this.Events[unMerge];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseStartCustomization(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[startCustomization];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseEndCustomization(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[endCustomization];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCloseButtonClick(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[closeButtonClick];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaisePressedLinkChanged(HighlightedLinkChangedEventArgs e) {
			HighlightedLinkChangedEventHandler handler = (HighlightedLinkChangedEventHandler)this.Events[pressedLinkChanged];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseQueryShowPopupMenu(QueryShowPopupMenuEventArgs e) {
			QueryShowPopupMenuEventHandler handler = (QueryShowPopupMenuEventHandler)this.Events[queryShowPopupMenu];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void OnCloseButtonClick() {
			RaiseCloseButtonClick(EventArgs.Empty);
		}
		protected internal virtual DevExpress.XtraBars.Customization.CustomizationForm OnCreateCustomizationForm() {
			DevExpress.XtraBars.Customization.CustomizationForm cf = null;
			CreateCustomizationFormEventArgs e = new CreateCustomizationFormEventArgs(null);
			RaiseCreateCustomizationForm(e);
			if(e.CustomizationForm != null) {
				cf = e.CustomizationForm;
			}
			if(cf == null)
				cf = new DevExpress.XtraBars.Customization.CustomizationForm(BarLocalizer.Active.Customization.Clone(), PaintStyle.CustomizationLookAndFeel);
			cf.Init(this);
			if(GetForm() != null) GetForm().AddOwnedForm(cf);
			else {
				Form frm = Form.FindForm();
				if(frm != null)
					frm.AddOwnedForm(cf);
				else
					cf.TopMost = true;
			}
			return cf;
		}
		public virtual void ShowToolBarsPopup(BarItemLink link) {
			ShowToolBarsPopup();
		}
		public virtual void ShowToolBarsPopup() {
			if(IsCustomizing) return;
			if(!AllowShowToolbarsPopup) return;
			ToolBarsPopup = null;
			ToolBarsPopup = new DevExpress.XtraBars.Customization.Helpers.ToolBarsPopup(this);
			ToolBarsPopup.ShowPopup(Control.MousePosition);
			if(!ToolBarsPopup.Opened) ToolBarsPopup = null;
		}
		public virtual void HideToolBarsPopup() {
			ToolBarsPopup = null;
		}
		protected internal void OnItemRemoved(BarItem item) {
			if(AllowDisposeObjects) {
				item.Manager = null;
				item.Dispose();
			}
		}
		void IBarAndDockingControllerClient.OnDisposed(BarAndDockingController controller) {
			if(IsDestroying) return;
			Controller = null;
		}
		void IBarAndDockingControllerClient.OnControllerChanged(BarAndDockingController controller) {
			if(destroying) return;
			if(Form != null && Form.InvokeRequired) {
				Form.Invoke(new MethodInvoker(OnControllerChanged));
				return;
			}
			OnControllerChanged();
		}
		protected virtual void OnControllerChanged() {
			RaiseControllerChanged(EventArgs.Empty);
			if(Form != null && !Form.IsHandleCreated) {
				if(Helper.LoadHelper.Loaded && formHandleDestroyed) return;
			}
			EditorHelper.DestroyEditorsCache();
			Form frm = GetForm();
			if(frm == null || frm.WindowState != FormWindowState.Minimized)
				LayoutChangedCore();
			if(this.internalMenuManager != null) this.internalMenuManager.Controller = Controller;
		}
		protected internal bool IsDestroying {
			get {
				if(destroying) return true;
				return destroying;
			}
		}
		protected internal bool AllowDisposeItems { get { return allowDisposeItems; } set { allowDisposeItems = value; } }
		protected internal bool AllowDisposeObjects { get { return allowDisposeObjects; } }
		protected void ClearHooks(bool disposing) {
			messageFilter.RemoveHook(this);
			RemoveManagerOnLoadedHook();
			lock(managers.SyncRoot) {
				managers.Remove(this);
				if(managers.Count == 0) {
					messageFilter.RemoveHook(disposing);
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if(this.destroying) return;
			this.destroying = true;
			this.managerLockUpdate++;
			this.allowDisposeObjects = true;
			ClearHooks(disposing);
			PopupContextMenus.Clear();
			DestroyCheckMdiTimer();
			if(disposing && Helper != null) {
				GetController().RemoveClient(this);
				SelectionInfo.Clear();
				InternalClearManager();
				SetForm(null);
				if(Helper != null) Helper.Dispose();
				helper = null;
				if(Categories != null) {
					this.categories.CollectionChange -= new CollectionChangeEventHandler(OnCategoriesChange);
				}
				if(Listeners != null) {
					Listeners.Clear();
				}
				this.listeners = null;
				if(InternalItems != null) InternalItems.Dispose();
				this.internalItems = null;
				if(this.internalMenuManager != null) this.internalMenuManager.Dispose();
				this.internalMenuManager = null;
				if(DockManager != null)
					DockManager.MenuManager = null;
			}
			if(components != null)
				components.Dispose();
			base.Dispose(disposing);
		}
		protected internal void OnLinkDelete(BarItemLink link) {
			if(SelectionInfo == null) return;
			SelectionInfo.OnLinkDelete(link);
		}
		protected virtual void RemoveDockWindows() {
			dockWindows.Clear();
		}
		protected virtual void InternalClearManager() {
			Control form = Form;
			if(form != null) form.SuspendLayout();
			try {
				CloseMenus();
				RemoveDockWindows();
				Helper.DockingHelper.RemoveDefaultDockControls();
				ClearBars();
				Items.Clear();
				EditorHelper.InternalRepository.Items.Clear();
			}
			finally {
				if(form != null) form.ResumeLayout(false);
			}
		}
		protected internal IBarManagerDesigner Designer {
			get {
				IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null) return host.GetDesigner(this) as IBarManagerDesigner;
				return null;
			}
		}
		void IDXMenuManager.ShowPopupMenu(DXPopupMenu menu, Control control, Point pos) {
			(this as IDXDropDownMenuManager).ShowDropDownMenu(menu, control, pos);
		}
		object IDXDropDownMenuManager.ShowDropDownMenu(DXPopupMenu menu, Control control, Point pos) {
			return ShowDXDropDownMenu(menu, control, pos);
		}
		protected virtual object ShowDXDropDownMenu(DXPopupMenu menu, Control control, Point pos) {
			if(menu == null) return null;
			if(menu.MenuViewType == MenuViewType.Toolbar || 
				menu.MenuViewType == MenuViewType.RibbonMiniToolbar) {
				DXToolbar tb = new DXToolbar(this, menu);
				MouseButtons buttons = Control.MouseButtons;
				if((buttons & MouseButtons.Right) != 0) IgnoreMouseUp++;
				if((buttons & MouseButtons.Left) != 0) IgnoreLeftMouseUp++;
				tb.Show(control, pos);
				return tb.Bar;
			}
			DXPopupXtraMenu dxMenu = new DXPopupXtraMenu(this, menu);
			dxMenu.Show(control, pos);
			return dxMenu.BarsMenu;
		}
		void IBarManagerControl.Activate() {
			lockFireChanged++;
			try {
				Activate();
			}
			finally {
				lockFireChanged--;
			}
		}
		void IBarManagerControl.Deactivate() {
			lockFireChanged++;
			try {
				Deactivate();
			}
			finally {
				lockFireChanged--;
			}
		}
		bool IBarManagerControl.IsActive { get { return IsBarsActive; } }
		protected internal bool IsRealBarsActive { get { return isBarsActive; } }
		internal bool IsBarsActive {
			get { return isBarsActive || SelectionInfo.TemporaryActive; }
			set {
				if(!value)
					Deactivate();
				if(isBarsActive == value) return;
				isBarsActive = value;
				if(isBarsActive)
					Activate();
			}
		}
		protected internal virtual void Activate() {
			if(IsRealBarsActive) return;
			if(Form == null) return;
			if(IsPartialDesignMode || (Designer != null && Designer.DebuggingState)) return;
			if(IsDesignMode) {
				IDesignerEventService srv = GetService(typeof(IDesignerEventService)) as IDesignerEventService;
				if(srv != null) {
					if(srv.ActiveDesigner == null) return;
				}
			}
			Control form = Form;
			while(form != null && form.IsHandleCreated && form.Handle != IntPtr.Zero) {
				if(!BarNativeMethods.IsWindowEnabled(form.Handle) && !SelectionInfo.ModalDialogActive) return;
				if(form == Form.Parent) break;
				form = Form.Parent;
			}
			if(GetForm() != null && GetForm().WindowState == FormWindowState.Minimized) return;
			foreach(Bar bar in Bars) {
				ISupportWindowActivate act = bar as ISupportWindowActivate;
				if(act != null) act.Activate();
			}
			IsBarsActive = true;
		}
		protected internal virtual void Deactivate() {
			if(SelectionInfo == null) return;
			isBarsActive = false;
			SelectionInfo.DockManager = null;
			SelectionInfo.CloseAllPopups();
			if(!SelectionInfo.ModalDialogActive)
				ClearSelection();
			foreach(Bar bar in Bars) {
				ISupportWindowActivate act = bar as ISupportWindowActivate;
				if(act != null) act.Deactivate();
			}
		}
		public virtual void CloseMenus() {
			SelectionInfo.CloseAllPopups();
			SelectionInfo.Clear();
		}
		public virtual void SelectLink(BarItemLink link) {
			if(IsLoading || IsCustomizing) return;
			if(link != null && link.BarControl == null) return;
			SelectionInfo.KeyboardHighlightedLink = link;
			if(SelectionInfo.KeyboardHighlightedLink == link) {
				if(link == null)
					CloseMenus();
				else {
					SelectionInfo.ActiveBarControl = link.BarControl;
				}
			}
		}
		protected virtual void ClearSelection() {
			if(inClearSelection > 0) return;
			inClearSelection++;
			try {
				SelectionInfo.Clear();
				if(Helper != null && Helper.CustomizationManager != null)
					Helper.CustomizationManager.HideItemDesigner();
				SelectionInfo.RestoreFocus(true);
			}
			finally {
				inClearSelection--;
			}
		}
		protected internal void HideMenus() {
			ClearSelection();
			SelectionInfo.CloseAllPopups();
			Helper.CustomizationManager.HideItemDesigner();
		}
		internal static bool NeedHideCursor(Control control) {
			if(!control.Visible) return false;
			if(Cursor.Current == null) return false;
			Size size = Cursor.Current.Size;
			Point p = control.PointToClient(Cursor.Position);
			p.Offset(size.Width, size.Height);
			Rectangle r = control.ClientRectangle;
			r.Offset(size.Width, size.Height);
			r.Inflate(size);
			if(r.Contains(p)) return true;
			return false;
		}
		protected internal static BarManager FindManager(Control form, bool useFindForm) {
			lock(managers.SyncRoot) {
				var documentForm = form as Docking2010.FloatDocumentForm;
				if(documentForm != null && documentForm.Controls.Count == 1)
					form = documentForm.Controls[0];
				foreach(BarManager man in managers) {
					if(man.Form == form) return man;
					if(man.Form is Ribbon.RibbonControl) {
						if(form != null) {
							if(man.Form.Parent == form)
								return man;
						}
						if(useFindForm) {
							var ribbonForm = man.GetForm();
							if(ribbonForm == form)
								return man;
							documentForm = ribbonForm as Docking2010.FloatDocumentForm;
							if(documentForm != null && documentForm.Controls.Count == 1) {
								if(documentForm.Controls[0] == form)
									return man;
							}
						}
					}
				}
			}
			return null;
		}
		protected internal static BarManager FindManager(Control form) {
		   return FindManager(form, true);
		}
		protected internal DevExpress.XtraBars.Customization.Helpers.ToolBarsPopup ToolBarsPopup {
			get { return toolBarsPopup; }
			set {
				if(ToolBarsPopup == value) return;
				if(toolBarsPopup != null)
					toolBarsPopup.Dispose();
				toolBarsPopup = value;
			}
		}
		protected internal ArrayList ItemHolders { get { return itemHolders; } }
		protected internal BarManagerInternalItems InternalItems { get { return internalItems; } }
		protected internal BarSelectionInfo SelectionInfo { get { return selectionInfo; } }
		protected internal PrimitivesPainter PPainter { get { return PaintStyle.PrimitivesPainter; } }
		protected internal BarDrawParameters DrawParameters { get { return PaintStyle.DrawParameters; } }
		protected internal virtual BarManagerHelpers Helper { get { return helper; } }
		protected internal void SynchronizeLinksInfo(LinksInfo lInfo, LinksInfo originalInfo) {
			for(int n = 0; n < lInfo.Count && n < originalInfo.Count; n++) {
				LinkPersistInfo info = lInfo[n], oInfo = originalInfo[n];
				oInfo.SetLink(info.Link);
			}
		}
		protected internal static bool ShouldSerializeLinks(LinksInfo defaultLinks, LinksInfo currentInfo) {
			if(defaultLinks == null || currentInfo == null) return true;
			if(defaultLinks.Count != currentInfo.Count) return true;
			for(int n = 0; n < defaultLinks.Count; n++) {
				if(!defaultLinks[n].IsEquals(currentInfo[n])) return true;
			}
			return false;
		}
		protected internal void CreateLinks(BarLinksHolder holder, LinksInfo lInfo) {
			try {
				if (lInfo == null) return;
				holder.BeginUpdate();
				holder.ClearLinks();
				foreach (LinkPersistInfo info in lInfo) {
					IBeginGroupSupport beginGroupSupport = info.Item as IBeginGroupSupport;
					if (beginGroupSupport != null)
						info.BeginGroup = beginGroupSupport.BeginGroup;
					BarItemLink link = holder.AddItem(info.Item, info);
				}
			}
			finally {
				holder.EndUpdate();
			}
		}
		protected internal virtual void AddToContainer(BarItem item) {
			if(IsDesignMode && Container != null)
				Container.Add(item);
		}
		protected internal void AddBar(Bar bar) {
			bars.Add(bar);
			if(IsDesignMode && Container != null && bar.Site == null) Container.Add(bar);
			if(IsLoading) return;
			RaiseCreateToolbar(new CreateToolbarEventArgs(bar));
			FireManagerChanged();
		}
		protected internal void OnRemoveBar(Bar bar) {
			if(MainMenu == bar) MainMenu = null;
			if(StatusBar == bar) StatusBar = null;
			bar.OnRemove();
			if(AllowDisposeObjects) {
				if(Container != null) Container.Remove(bar);
				FireManagerChanged();
			}
		}
		protected internal void AddDockControl(BarDockControl dockControl) {
			dockControls.Add(dockControl);
		}
		protected internal void RemoveDockControl(BarDockControl dockControl) {
			dockControl.AllowDispose = true;
			dockControls.Remove(dockControl);
		}
		public string GetNewBarName() {
			int n = Bars.Count + 1;
			string res, prev;
			prev = string.Empty;
			for(; ; ) {
				res = string.Format(BarLocalizer.Active.GetLocalizedString(BarString.NewToolbarCustomNameFormat), n);
				if(prev == res) res += n.ToString();
				prev = res;
				foreach(Bar bar in Bars) {
					if(bar.BarName == res) { n++; res = null; break; }
				}
				if(res == null) continue;
				break;
			}
			return res;
		}
		protected virtual void OnCategoriesChange(object sender, CollectionChangeEventArgs e) {
			if(IsLoading) return;
			if(e.Action == CollectionChangeAction.Add) {
			}
			FireManagerChanged();
		}
		protected internal void LayoutChangedCore() {
			if(this.destroying || this.formClosed) return;
			DrawParameters.UpdateScheme(DesignMode);
			Helper.MdiHelper.UpdateSystemButtons();
			if(IsLoading) return;
			CloseMenus();
			this.selectionInfo = CreateSelectionInfo();
			try {
				foreach(BarDockControl dock in DockControls) {
					dock.BeginUpdate();
				}
				foreach(Bar bar in Bars) {
					bar.UpdateScheme();
				}
			}
			finally {
				foreach(BarDockControl dock in DockControls) {
					dock.CancelUpdate();
				}
			}
			foreach(BarDockControl dock in DockControls) {
				dock.UpdateScheme();
			}
		}
		public virtual void HideCustomization() {
			Helper.CustomizationManager.StopCustomization();
		}
		protected internal void ClearBars() {
			Bars.DisposeBars();
		}
		int lockPaint = 0;
		internal void LockPaint() { this.lockPaint++; }
		internal void UnlockPaint() {
			if(this.lockPaint > 0)
				this.lockPaint--;
		}
		internal bool IsPaintLocked { get { return this.lockPaint > 0; } }
		internal int lockFireChanged = 0;
		internal void FireManagerChanged() {
			if(!IsDesignMode) return;
			if(IsLoading) return;
			if(Helper.DockingHelper.InUpdateDocking || lockFireChanged != 0) return;
			OnFireManagerChanged();
		}
		protected virtual void OnFireManagerChanged() {
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				srv.OnComponentChanged(this, null, null, null);
			}
		}
		protected override void RaiseRequireHideEditor() {
			if(SelectionInfo != null) SelectionInfo.HideEditor();
		}
		protected override ComponentEditorContainerHelper CreateHelper() {
			return new BarEditorContainerHelper(this);
		}
		protected new internal BarEditorContainerHelper EditorHelper { get { return base.EditorHelper as BarEditorContainerHelper; } }
		protected internal BarManager InternalMenuManager {
			get {
				if(internalMenuManager == null) {
					this.internalMenuManager = CreateInternalManager();
				}
				return internalMenuManager;
			}
		}
		BarManager CreateInternalManager() {
			BarManager manager = new BarManager();
			manager.DockingEnabled = false;
			manager.Form = Form;
			manager.Controller = Controller;
			return manager;
		}
		protected virtual BarItemLink FindLinkByAccelerator(Keys keyCode) {
			if(keyCode == Keys.None || keyCode == Keys.Menu) return null;
			for(int n = -1; n < Bars.Count; n++) {
				Bar bar = (n == -1 ? MainMenu : Bars[n]);
				if(bar == null || !bar.Visible || bar.BarControl == null || !bar.BarControl.Visible) continue;
				foreach(BarItemLink link in bar.VisibleLinks) {
					if(link.Enabled && keyCode == link.Accelerator) return link;
				}
			}
			return null;
		}
		protected internal virtual bool ProcessLinkAccelerator(Keys keyCode) {
			BarItemLink link = FindLinkByAccelerator(keyCode);
			if(link != null) {
				SelectionInfo.PostEditor();
				link.OnLinkActionCore(BarLinkAction.KeyClick, null);
				return true;
			}
			return false;
		}
		protected void OnFormHandlerCreated(object sender, EventArgs e) {
			this.formHandleDestroyed = false;
			if(Form != null)
				messageFilter.CheckHook(this);
		}
		protected virtual void OnFormDisposed(object sender, EventArgs e) {
			this.Dispose();
		}
		protected virtual void OnFormMdiChildActivate(object sender, EventArgs e) {
			if(IsDesignMode) return;
			Form form = sender as Form, mdiParent = form;
			if(form.IsMdiContainer) {
				form = form.ActiveMdiChild;
			}
			else {
				mdiParent = form.MdiParent;
			}
			BarManager manager = BarManager.FindManager(mdiParent);
			if(manager != null) manager.Helper.MdiHelper.DoCheckMdi(form, true);
		}
		bool formClosed = false;
		bool dialogClosed = false;
		protected virtual void OnFormClosed(object sender, EventArgs e) {
			Form frm = GetForm();
			formClosed = frm == null || frm.DialogResult == DialogResult.None;
			dialogClosed = !formClosed;
			if(AutoSaveInRegistry && !IsDesignMode)
				SaveToRegistry();
		}
		Size lastFormSize = Size.Empty;
		bool IsSizeEmpty(Size size) {
			return size.Height < 1 || size.Width < 1;
		}
		protected virtual bool CheckAndLoad() {
			if(Helper.LoadHelper.Loaded) return true;
			if(Form != null && Form.IsHandleCreated) {
				if(!IsDesignMode) {
					Helper.LoadHelper.Load();
					return true;
				}
			}
			return false;
		}
		void OnFormResize(object sender, EventArgs e) {
			if(IsLoading || IsDestroying || Form == null || !Form.Visible) return;
			if(!CheckAndLoad()) return;
			if(!IsSizeEmpty(Form.ClientSize)) {
				if(IsSizeEmpty(lastFormSize)) {
					Helper.DockingHelper.ForceUpdateDockingOrder();
					Helper.DockingHelper.ForceUpdateDockingOrder();
				}
				if(ShouldUpdateDockControlsOnFormResize()) {
					Helper.DockingHelper.CheckForceUpdateDockControls();
				}
			}
			else {
				if(Form.Location.X < -2000) Deactivate();
			}
			lastFormSize = Form.ClientSize;
		}
		bool ShouldUpdateDockControlsOnFormResize() {
			foreach(BarDockControl control in DockControls) {
				if(control.DockStyle != BarDockStyle.Standalone && control.DockStyle != BarDockStyle.None) {
					if(control.Parent != Form)
						return true;
				}
			}
			return false;
		}
		Timer mdiCheckTimer = null;
		internal void DestroyCheckMdiTimer() {
			if(this.mdiCheckTimer != null) {
				this.mdiCheckTimer.Stop();
				this.mdiCheckTimer.Dispose();
				this.mdiCheckTimer = null;
			}
		}
		internal void CreateCheckMdiTimer() {
			if(mdiCheckTimer != null) return;
			mdiCheckTimer = new Timer();
			mdiCheckTimer.Interval = 1;
			mdiCheckTimer.Tick += new EventHandler(OnCheckMdiTimer);
			mdiCheckTimer.Start();
		}
		void OnCheckMdiTimer(object sender, EventArgs e) {
			if(this.mdiCheckTimer != null) {
				DestroyCheckMdiTimer();
				if(!IsDestroying && Form != null && MdiContainerManager != null) MdiContainerManager.Helper.MdiHelper.DoCheckMdi(null);
			}
		}
		void OnFormLayoutChanged(object sender, LayoutEventArgs e) {
			this.originalForm = null;
			if(this.IsLoading || IsDestroying || (DockManager != null && DockManager.IsDeserializing)) return;
			if(!CheckAndLoad()) return;
			if(OForm != null && OForm.WindowState == FormWindowState.Minimized) return;
			if(e.AffectedControl is BarDockControl) return;
			if(IsMdiChildManager) {
				CreateCheckMdiTimer();
			}
			if(Form.ClientSize.IsEmpty || (IsDesignMode && e.AffectedProperty == "Visible")) return; 
			Helper.DockingHelper.CheckForceUpdateDockControls();
		}
		void OnFormVisibleChanged(object sender, EventArgs e) {
			this.originalForm = null;
			if(IsLoading || IsDestroying || Form == null) return;
			if(!Form.Visible) {
				Deactivate();
				return;
			}
			if(Form.ClientSize.IsEmpty || (OForm != null && OForm.WindowState == FormWindowState.Minimized)) return;
			if(!CheckAndLoad()) return;
			if(MdiContainerManager != null) MdiContainerManager.Helper.MdiHelper.DoCheckMdi(null);
			Helper.DockingHelper.ForceUpdateDockingOrder();
			Helper.DockingHelper.ForceUpdateDockingOrder();
			lastFormSize = Form.ClientSize;
			if(this.dialogClosed) {
				this.dialogClosed = false;
				LayoutChangedCore();
			}
			if(IsOriginFormActive) 
				Activate();
		}
		protected virtual bool IsOriginFormActive {
			get {
				return OForm != null && OForm == System.Windows.Forms.Form.ActiveForm;
			}
		}
		protected BarManager MdiContainerManager {
			get {
				if(IsMdiContainerManager) return this;
				if(!IsMdiChildManager) return null;
				return FindManager(MdiParent);
			}
		}
		protected internal bool IsMdiContainerManager {
			get {
				System.Windows.Forms.Form frm = FilterForm as Form;
				return (frm != null && frm.IsMdiContainer);
			}
		}
		protected internal bool IsMdiChildManager {
			get {
				System.Windows.Forms.Form frm = FilterForm as Form;
				return (frm != null && frm.IsMdiChild);
			}
		}
		protected internal Form MdiParent {
			get {
				Form frm = FilterForm as Form;
				if(frm != null) {
					if(frm.IsMdiContainer) return frm;
					return frm.MdiParent;
				}
				return null;
			}
		}
		protected internal bool CanShowMainMenu {
			get {
				if(IsDesignMode) return true;
				if(IsMdiContainerManager) return true;
				if(!IsMdiChildManager) return true;
				if(MdiParent == null) return true;
				BarManager manager = FindManager(MdiParent);
				if(manager == null) return true;
				Form frm = GetForm();
				if(frm != null && !frm.Visible) return true;
				BarMdiMenuMergeStyle mergeStyle = manager.MdiMenuMergeStyle;
				if(mergeStyle == BarMdiMenuMergeStyle.Never) return true;
				if(mergeStyle == BarMdiMenuMergeStyle.Always) {
					return false;
				}
				bool isActive = ActiveMdiChild == Form;
				if(mergeStyle == BarMdiMenuMergeStyle.WhenChildActivated)
					return !isActive;
				if(Helper.MdiHelper.IsMaximized(FilterForm as Form) && (isActive)) return false;
				return true;
			}
		}
		int updatingMainMenuVisibility;
		protected internal void UpdateMainMenuVisibility() {
			if(MainMenu == null) return;
			updatingMainMenuVisibility++;
			if(!MainMenu.CheckIsMdiChildBar())
				MainMenu.OnIsMdiChildBarChanged();
			updatingMainMenuVisibility--;
			if(!MainMenu.Visible || IsDestroying) return;
			bool visible = MainMenu.BarControl != null && MainMenu.BarControl.Visible;
			if(CanShowMainMenu != visible) {
				((IDockableObject)MainMenu).VisibleChanged();
			}
		}
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinXtraBars));
		}
		public bool SaveLayoutToRegistry() {
			if(RegistryPath.Length == 0) return false;
			return SaveToRegistry(RegistryPath);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SaveToRegistry() {
			return SaveLayoutToRegistry();
		}
		public void RestoreLayoutFromRegistry() {
			if(RegistryPath.Length == 0) return;
			RestoreFromRegistry(RegistryPath);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RestoreFromRegistry() {
			RestoreLayoutFromRegistry();
		}
		public virtual string GetString(BarString str) {
			return BarLocalizer.Active.GetLocalizedString(str);
		}
		public virtual void SaveLayoutToXml(string xmlFile) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual bool SaveLayoutToRegistry(string path) {
			return SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void SaveLayoutToStream(System.IO.Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void RestoreLayoutFromStream(System.IO.Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SaveToXml(string xmlFile) {
			SaveLayoutToXml(xmlFile);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RestoreFromXml(string xmlFile) {
			RestoreLayoutFromXml(xmlFile);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool SaveToRegistry(string path) {
			return SaveLayoutToRegistry(path);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RestoreFromRegistry(string path) {
			RestoreLayoutFromRegistry(path);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SaveToStream(System.IO.Stream stream) {
			SaveLayoutToStream(stream);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RestoreFromStream(System.IO.Stream stream) {
			RestoreLayoutFromStream(stream);
		}
		protected virtual bool SaveLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				return serializer.SerializeObjects(GetXtraObjectInfo(), stream, this.GetType().Name);
			else
				return serializer.SerializeObjects(GetXtraObjectInfo(), path.ToString(), this.GetType().Name);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			ForceLinkCreate();
			System.IO.Stream stream = path as System.IO.Stream;
			if(Form != null) Form.SuspendLayout();
			try {
				if(stream != null)
					serializer.DeserializeObjects(GetXtraObjectInfo(), stream, this.GetType().Name);
				else
					serializer.DeserializeObjects(GetXtraObjectInfo(), path.ToString(), this.GetType().Name);
			}
			finally {
				if(Form != null) Form.ResumeLayout(true);
			}
		}
		protected XtraObjectInfo[] GetXtraObjectInfo() {
			ArrayList result = new ArrayList();
			result.Add(new XtraObjectInfo("BarManager", this));
			if(DockManager != null) {
				result.Add(new XtraObjectInfo("DockManager", DockManager));
				if(DockManager.DocumentManager != null && DockManager.DocumentManager.View!= null)
					result.Add(new XtraObjectInfo("DocumentManager.View", DockManager.DocumentManager.View));
			}
			return (XtraObjectInfo[])result.ToArray(typeof(XtraObjectInfo));
		}
		protected internal virtual BarItems MergedItems {
			get {
				if(Helper != null && Helper.MdiHelper != null && Helper.MdiHelper.MergedManager != null)
					return Helper.MdiHelper.MergedManager.Items;
				return null;
			}
		}
		BarManager prevMergedManager = null;
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			RaiseBeforeLoadLayout(e);
			if(!e.Allow) return;
			this.restoring = true;
			this.initCount++;
			this.prevMergedManager = Helper.MdiHelper.MergedManager;
			if(Helper.MdiHelper.MergedManager != null) {
				Helper.MdiHelper.UnMergeManager();
			}
			Helper.RecentHelper.ClearRecentItems();
		}
		internal bool IsRestoring { get { return restoring; } }
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			initCount--;
			Helper.RecentHelper.UpdateRecentItems();
			BeginUpdate();
			try {
				restoring = true;
				try {
					UpdateVisibleList();
					Helper.DockingHelper.UpdateBarDocking();
					Helper.MdiHelper.MergeManager(this.prevMergedManager);
				}
				finally {
					restoring = false;
				}
				if(restoredVersion != LayoutVersion) RaiseLayoutUpgrade(new LayoutUpgradeEventArgs(restoredVersion));
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void UpdateVisibleList() {
			for(int n = 0; n < Bars.Count; n++) {
				Bars[n].OnBarChanged();
			}
		}
		protected internal void LayoutChanged() {
			BeginUpdate();
			EndUpdate();
		}
		internal void DesignerLayoutChanged() {
			this.lockFireChanged++;
			try {
				LayoutChanged();
			}
			finally {
				this.lockFireChanged--;
			}
		}
		int managerLockUpdate = 0;
		public virtual void BeginUpdate() {
			if(this.managerLockUpdate++ == 0) {
				if(Form != null) Form.SuspendLayout();
				for(int n = 0; n < Bars.Count; n++) {
					Bars[n].BeginUpdate();
				}
				foreach(BarDockControl dock in DockControls) {
					dock.BeginUpdate();
				}
			}
		}
		protected internal virtual bool IsManagerLockUpdate { get { return managerLockUpdate != 0; } }
		public virtual void EndUpdate() {
			if(this.managerLockUpdate == 1) {
				try {
					ArrayList bars = Helper.DockingHelper.GetDockableObjects();
					for(int n = 0; n < bars.Count; n++) {
						Bar bar = bars[n] as Bar;
						bar.EndUpdate();
					}
					foreach(BarDockControl dock in DockControls) {
						dock.EndUpdate();
					}
					Helper.DockingHelper.UpdateDockableObjects(Helper.DockingHelper.GetDockableObjects());
				}
				finally {
					this.managerLockUpdate = 0;
					if(Form != null) Form.ResumeLayout(true);
				}
			}
			else
				this.managerLockUpdate--;
		}
		void IXtraSerializable.OnStartSerializing() {
			storing = true;
			BeginUpdate();
			this.prevMergedManager = Helper.MdiHelper.MergedManager;
			if(Helper.MdiHelper.MergedManager != null) {
				Helper.MdiHelper.UnMergeManager();
			}
			ItemLinkIdCollection.Clear();
			foreach(BarItem item in Items) {
				ItemLinkIdCollection.Add(new BarItemLinkId() { Id = item.Name });
			}
		}
		void IXtraSerializable.OnEndSerializing() {
			try {
				Helper.MdiHelper.MergeManager(this.prevMergedManager);
				storing = false;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual bool ProcessCommandKey(Keys key) {
			return false;
		}
		protected internal bool IsMdiParentControl(Control mdiParent, Control mdiChild) {
			Form frm = mdiParent as Form;
			Form child = mdiChild as Form;
			if(frm == null || child == null) return false;
			return frm.ActiveMdiChild == child;
		}
		protected internal virtual bool GetAllowHideEmptyBar() { return false; }
		protected internal virtual bool IsHideItemsFromMergedManager() { return false; }
		#region ISupportXtraSerializer Members
		void ISupportXtraSerializer.SaveLayoutToRegistry(string path) {
			SaveLayoutToRegistry(path);
		}
		#endregion
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return false; }
		}
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel {
			get { return GetController().LookAndFeel; }
		}
		#endregion
		protected internal virtual void UpdateRadialMenuHoverInfo(Point point) {
			foreach(BarLinksHolder holder in ItemHolders) {
				if(holder is RadialMenu) ((RadialMenu)holder).UpdateHoverInfo(point);
			}
		}
		protected internal virtual void RadialMenuAppActiveStatusChanging(bool status) {
			foreach(BarLinksHolder holder in ItemHolders) {
				if(holder is RadialMenu) ((RadialMenu)holder).AppActiveStatusChanging(status);
			}
		}
		[DefaultValue(PopupShowMode.Default)]
		public virtual PopupShowMode PopupShowMode {
			get;
			set;
		}
		protected internal virtual PopupShowMode GetPopupShowMode(IPopup popup) {
			return PopupShowMode == XtraBars.PopupShowMode.Default ? PopupShowMode.Classic : PopupShowMode;
		}
		protected internal virtual bool GetScaleEditors() {
			return GetController().PropertiesBar.ScaleEditors;
		}
		object IXtraSerializationIdProvider.GetSerializationId(XtraSerializableProperty property, object item) {
			return ((Bar)item).BarName;
		}
		bool IXtraCollectionDeserializationOptionsProvider.AddNewItems {
			get { return OptionsLayout.AllowAddNewItems; }
		}
		bool IXtraCollectionDeserializationOptionsProvider.RemoveOldItems {
			get { return OptionsLayout.AllowRemoveOldItems; }
		}
		void XtraRemoveBarsItem(XtraSetItemIndexEventArgs e) {
			Bars.Remove((Bar)e.Item.Value);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PrimaryShortcutProcessor PrimaryShortcutProcessor { get; set; }
	}
	public enum PopupShowMode { Default, Classic, Inplace }
	public interface IDocumentFloatingContext : IDisposable {
		BarManager BarManager { get; }
		Docking2010.Views.BaseDocument Document { get; }
		void SetDocument(Docking2010.Views.BaseDocument document);
	}
	public interface ISupportWindowActivate {
		void Activate();
		void Deactivate();
	}
	public interface IBarManagerDesigner {
		bool AllowDesignTimeEnhancements { get; }
		bool DebuggingState { get; }
	}
	public interface IBarManagerControl {
		void Activate();
		void Deactivate();
		bool IsActive { get; }
		void DesignerHostLoaded();
	}
	#region MyDebug
	public class MyDebug {
		int ticks;
		static int row = 1;
		public void StartProfile() {
			ticks = System.Environment.TickCount;
		}
		public void EndProfile(string title) {
			ticks = System.Environment.TickCount - ticks;
			WriteString("OP '" + title + "', time: " + ticks.ToString());
		}
		public void Write(string str, bool printRow) {
			if(printRow)
				BarNativeMethods.OutputDebugString(row++.ToString() + "." + str);
			else
				BarNativeMethods.OutputDebugString(str);
		}
		public void Write(string str) { Write(str, true); }
		public void WriteString(string str) {
			Write(str + "\n");
		}
		public void PrintStackTrace(int depth) {
			WriteString("***************************");
			System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
			for(int i = 0; i < st.FrameCount; i++) {
				if(depth != -1 && i > depth) break;
				System.Diagnostics.StackFrame sf = st.GetFrame(i);
				Write(string.Format("{0} Line: {1} ({2})", sf.GetMethod(), sf.GetFileLineNumber(), sf.GetFileName()), false);
				Write("\n", false);
			}
		}
		public string GetEnumString(object obj) {
			System.Type typ = obj.GetType();
			return typ.GetMembers()[1 + Convert.ToInt32(obj.ToString())].Name;
		}
	}
	#endregion MyDebug
	public class BarManagerCommandContext : BarCommandContextBase {
		public BarManagerCommandContext(BarManager barManager)
			: base(barManager) {
		}
		protected override void SetOwner(BarItem barItem, string category) {
			Bar mainMenu = Manager.MainMenu;
			if(mainMenu != null) {
				BarSubItem owner = GetOwner(category);
				if(owner == null) {
					owner = CreateOwner(category);
				}
				barItem.Manager = Manager;
				owner.AddItem(barItem);
			}
		}
		protected BarSubItem GetOwner(string category) {
			Bar mainMenu = Manager.MainMenu;
			if(mainMenu == null) return null;
			foreach(BarItemLink link in mainMenu.ItemLinks) {
				if(link.Item is BarSubItem && link.Caption.Equals(category))
					return link.Item as BarSubItem;
			}
			return null;
		}
		protected BarSubItem CreateOwner(string category) {
			Bar mainMenu = Manager.MainMenu;
			if(mainMenu == null) return null;
			BarSubItem owner = DesignerHost.CreateComponent(typeof(BarSubItem)) as BarSubItem;
			Manager.Items.Add(owner);
			try {
				Manager.AddToContainer(owner);
			}
			catch { }
			owner.Caption = category;
			owner.Manager = Manager;
			mainMenu.InsertItem(null, owner);
			return owner;
		}
		protected override void UpdateDesigner(BarItem barItem) {
			base.UpdateDesigner(barItem);
		}
		public BarManager Manager { get { return Component as BarManager; } }
	}
	public class BarEditorContainerHelper : ComponentEditorContainerHelper {
		public BarEditorContainerHelper(BarManager owner)
			: base(owner) {
		}
		protected new BarManager Owner { get { return base.Owner as BarManager; } }
		protected override void OnRepositoryItemRemoved(RepositoryItem item) {
			if(IsLoading) return;
			base.OnRepositoryItemRemoved(item);
		}
		public override InplaceType InplaceContainerType { get { return InplaceType.Bars; } }
		protected override void OnRepositoryItemChanged(RepositoryItem item) {
			if(!IsLoading) {
				for(int n = 0; n < Owner.Items.Count; n++) {
					BarEditItem be = Owner.Items[n] as BarEditItem;
					if(be == null || be.Edit != item) continue;
					be.OnItemChanged(false);
				}
			}
			base.OnRepositoryItemChanged(item);
		}
		protected override void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
		}
		protected override void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs va) {
		}
	}
	internal class BarsObsoleteText {
		internal const string SRObsoleteAppearance = "You should use the 'Appearance'";
		internal const string SRObsoleteShowDockWindows = "You should use the 'ShowDockPanels' property instead of property 'ShowDockWindows'";
		internal const string SRObsoleteOptions = "You should use the 'OptionsBar' property instead of property 'Options'";
		internal const string SRObsoleteMenuAppearance = "You should use the 'MenuAppearance'";
		internal const string SRObsoleteManager = "You should use the BarManagerProperties class";
		internal const string SRObsoleteEditorsRepository = "You should use 'RepositoryItems' or 'ExternalRepository' property";
		internal const string SRObsoleteItemCategoryIndex = "You should use 'CategoryGuid' or 'Category' instead of property 'CategoryIndex'";
		internal const string SRObsoleteCtorWithBarOptionFlagsParam = "The 'BarOptionFlags AOptions' parameter is ignored. Please remove this parameter from your constructor call";
	}
	public class BarManagerInternalItems : IDisposable {
		BarManager manager;
		BarDesignTimeItem designTimeItem;
		BarEmptyItem emptyItem;
		BarCloseItem closeItem;
		BarScrollItem scrollItem;
		BarQBarCustomizationItem quickCustomizationItem;
		public BarManagerInternalItems(BarManager manager) {
			this.manager = manager;
			this.quickCustomizationItem = new BarQBarCustomizationItem(Manager);
			this.scrollItem = new BarScrollItem(Manager);
			this.emptyItem = new BarEmptyItem(Manager);
			this.closeItem = new BarCloseItem(Manager);
			this.designTimeItem = new BarDesignTimeItem(Manager);
		}
		public virtual void Dispose() {
			if(Manager != null) {
				this.quickCustomizationItem.Dispose();
				this.scrollItem.Dispose();
				this.emptyItem.Dispose();
				this.closeItem.Dispose();
				this.designTimeItem.Dispose();
			}
			this.manager = null;
		}
		public BarManager Manager { get { return manager; } }
		public BarQBarCustomizationItem QuickCustomizationItem { get { return quickCustomizationItem; } }
		public BarScrollItem ScrollItem { get { return scrollItem; } }
		public BarEmptyItem EmptyItem { get { return emptyItem; } }
		public BarCloseItem CloseItem { get { return closeItem; } }
		public BarDesignTimeItem DesignTimeItem { get { return designTimeItem; } }
	}
	public enum ToolTipAnchor { Cursor, BarItemLink }
	public static class MenuManagerCreator {
		public static void CreateMenuManager(Control owner) {
			BarManager barManager = new BarManager();
			barManager.Form = owner;
			BarAndDockingController controller = new BarAndDockingController();
			ISupportLookAndFeel lookAndFeelProvider = owner as ISupportLookAndFeel;
			if(lookAndFeelProvider != null)
				controller.LookAndFeel.ParentLookAndFeel = lookAndFeelProvider.LookAndFeel.ActiveLookAndFeel;
			barManager.Controller = controller;
		}
	}
	public enum PrimaryShortcutProcessor { BarItem, Editor }
	public enum PopupMenuAlignment { Default, Left, Right }
}
