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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraNavBar.Accessibility;
using DevExpress.XtraNavBar.Forms;
using DevExpress.XtraNavBar.ViewInfo;
using System.Drawing.Design;
using System.Collections.Generic;
using DevExpress.Utils.Navigation;
namespace DevExpress.XtraNavBar {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class NavBarLayoutOptions {
		public NavBarLayoutOptions() {
			StoreAppearance = false;
		}
		[DefaultValue(false), 
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarLayoutOptionsStoreAppearance")
#else
	Description("")
#endif
]
		public bool StoreAppearance { get; set; }
	}
	public class NavPaneStateConverter {
		public static NavPaneState Invert(NavPaneState state) {
			if(state == NavPaneState.Collapsed) return NavPaneState.Expanded;
			if(state == NavPaneState.Expanded) return NavPaneState.Collapsed;
			return state;
		}
	}
	public enum NavPaneState { Expanded, Collapsed }
	public enum NavBarScrollMode { Default, ScrollAlways, ScrollWhenFocused }
	[Designer("DevExpress.XtraNavBar.Design.NavBarControlDesigner, " + AssemblyInfo.SRAssemblyNavBarDesign, typeof(System.ComponentModel.Design.IDesigner)),
	 Description("Represents options as links combined into groups."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation)
]
	[DXToolboxItem(DXToolboxItemKind.Regular)]
	public class NavBarControl : Control, ISupportInitialize, IXtraSerializable, IXtraSerializableLayout, ISkinProvider, ISkinProviderEx, ISupportLookAndFeel, IToolTipControlClient, ISupportXtraAnimation, IXtraObjectWithBounds, IXtraResizableControl, IDXMenuSupport, IMouseWheelSupport, IGestureClient, IOfficeNavigationBarClient, ISearchControlClient {
		ToolTipController toolTipController;
		NavBarDesignTimeManager designManager;
		bool hideGroupCaptions, firstBeginInit, eachGroupHasSelectedLink, allowSelectedLink, showGroupHint, showLinkHint,
			 explorerBarShowGroupButtons, suspendFormLayoutInAnimation;
		Cursor hotTrackedGroupCursor, hotTrackedItemCursor, originalCursor;
		SkinExplorerBarViewScrollStyle skinExplorerBarViewScrollStyle;
		Brush groupTextureBackgroundBrush;
		Image groupBackgroundImage;
		BaseViewInfoRegistrator view;
		NavGroupCollection groups;
		NavItemCollection items;
		object smallImages, largeImages;
		NavBarDragDrop dragDropFlags;
		NavBarHook hook;
		IDXMenuManager menuManager;
		string contentButtonHint = string.Empty;
		int lockInit, lockLayout, showHintInterval, navigationPaneMaxVisibleGroups, navigationPaneGroupClientHeight,
			explorerBarGroupInterval, explorerBarGroupOuterIndent, linkInterval, deserializing;
		bool navigationPaneOverflowPanelUseSmallImages, storeDefaultPaintStyleName;
		bool explorerBarStretchLastGroup, allowHtmlString;
		ImageCollection htmlImages;
		NavBarGroup activeGroup;
		NavBarViewKind paintStyleKind;
		UserLookAndFeel lookAndFeel;
		BaseNavGroupPainter groupPainter;
		BaseNavLinkPainter linkPainter;
		ObjectPainter buttonPainter;
		BorderStyles borderStyle;
		NavBarViewInfo viewInfo;
		NavBarViewCollection availableNavBarViews;
		NavBarAppearances appearance;
		OptionsNavPane optionsNavPane;
		NavPaneForm navPaneForm;
		NavBarScrollMode scrollMode;
		string paintStyleName, layoutVersion;
		SharedImageCollectionImageSizeMode collectionSizeMode;
		NavBarLayoutOptions optionsLayout;
		bool allowGlyphSkinning;
		Dictionary<string, string> groupSelectedItems;
		LinkSelectionModeType linkSelectionMode;
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinNavBar));
		}
		public const string DefaultPaintStyleName = "Default";
		public NavBarControl() {
			Permissions.Request();
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw | ControlStyles.UserMouse, true);
			this.layoutVersion = "";
			this.paintStyleName = DefaultPaintStyleName;
			DevExpress.Utils.WXPaint.Painter.ThemeChanged(); 
			NavBarLocalizer.ActiveChanged += new EventHandler(OnLocalizerChanged);
			this.explorerBarShowGroupButtons = true;
			this.paintStyleKind = NavBarViewKind.Default;
			this.lookAndFeel = new ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
			this.appearance = new NavBarAppearances();
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.hotTrackedGroupCursor = Cursors.Default;
			this.hotTrackedItemCursor = Cursors.Hand;
			this.availableNavBarViews = new NavBarViewCollection();
			RegisterAvailableNavBarViews();
			this.borderStyle = BorderStyles.Default;
			this.skinExplorerBarViewScrollStyle = SkinExplorerBarViewScrollStyle.Default;
			this.view = null;
			this.linkInterval = this.explorerBarGroupOuterIndent = this.explorerBarGroupInterval = -1;
			this.navigationPaneOverflowPanelUseSmallImages = true;
			this.navigationPaneGroupClientHeight = 100;
			this.navigationPaneMaxVisibleGroups = -1;
			this.showHintInterval = 1000;
			this.showGroupHint = this.showLinkHint = true;
			this.dragDropFlags = NavBarDragDrop.AllowDrag | NavBarDragDrop.AllowDrop;
			this.allowSelectedLink = false;
			this.eachGroupHasSelectedLink = false;
			this.firstBeginInit = true;
			this.explorerBarStretchLastGroup = false;
			this.groupTextureBackgroundBrush = null;
			this.groupBackgroundImage = null;
			this.hideGroupCaptions = false;
			this.allowHtmlString = false;
			this.htmlImages = null;
			this.buttonPainter = null;
			this.linkPainter = null;
			this.groupPainter = null;
			this.lockLayout = 0;
			this.deserializing = 0;
			this.lockInit = 1;
			this.optionsNavPane = null;
			this.collectionSizeMode = SharedImageCollectionImageSizeMode.UseCollectionImageSize;
			this.StateInfo = new NavBarCustomizationInfo();
			this.optionsLayout = CreateOptionsLayout();
			this.allowGlyphSkinning = false;
			this.groupSelectedItems = new Dictionary<string, string>();
			this.linkSelectionMode = LinkSelectionModeType.None;
			this.suspendFormLayoutInAnimation = true;
			try {
				this.TabStop = false; 
				this.designManager = CreateDesignManager();
				UpdateView();
				this.paintStyleName = DefaultPaintStyleName;
				this.storeDefaultPaintStyleName = false;
				this.activeGroup = null;
				this.smallImages = this.largeImages = null;
				this.groups = CreateGroupCollection();
				this.groups.CollectionChanged += new CollectionChangeEventHandler(OnGroupCollectionChanged);
				this.items = CreateItemCollection();
				this.items.CollectionChanged += new CollectionChangeEventHandler(OnItemCollectionChanged);
			}
			finally {
				lockInit--;
			}
			ToolTipController.DefaultController.AddClientControl(this);
			this.AllowDrop = true;
		}
		protected virtual NavBarLayoutOptions CreateOptionsLayout() {
			return new NavBarLayoutOptions();
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual NavBarScrollMode ScrollMode { get { return scrollMode; } set { scrollMode = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual NavPaneForm NavPaneForm { get { return navPaneForm; } }
		protected internal virtual void SetNavPaneForm(NavPaneForm frm) { navPaneForm = frm; }
		public virtual void HideNavPaneForm() {
			if(NavPaneForm == null) return;
			NavPaneForm.Close();
			if(NavPaneForm == null) return;
			NavPaneForm.Dispose();
			SetNavPaneForm(null);
			NavigationPaneViewInfo navPane = ViewInfo as NavigationPaneViewInfo;
			Invalidate(navPane.ContentButton);
		}
		DefaultBoolean showIcons = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowIcons {
			get { return showIcons; }
			set {
				if(ShowIcons == value)
					return;
				showIcons = value;
				LayoutChanged();
			}
		}
		[Category("Behavior"), DefaultValue(false)]
		public bool AllowHtmlString {
			get { return allowHtmlString; }
			set {
				if(AllowHtmlString == value)
					return;
				allowHtmlString = value;
				LayoutChanged();
			}
		}
		[DefaultValue(null)]
		public ImageCollection HtmlImages {
			get { return htmlImages; }
			set {
				if(HtmlImages == value) return;
				ImageCollection prev = HtmlImages;
				htmlImages = value;
				OnHtmlImagesChangedCore(prev, HtmlImages);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true)]
		public bool SuspendFormLayoutInAnimation {
			get { return suspendFormLayoutInAnimation; }
			set {
				if(SuspendFormLayoutInAnimation == value) return;
				suspendFormLayoutInAnimation = value;
			}
		}
		bool isRightToLeft = false;
		protected void CheckRightToLeft() {
			bool newRightToLeft = WindowsFormsSettings.GetIsRightToLeft(this);
			if(newRightToLeft == this.isRightToLeft) return;
			this.isRightToLeft = newRightToLeft;
			OnRightToLeftChanged();
		}
		protected internal virtual bool IsRightToLeft { get { return isRightToLeft; } }
		protected virtual void OnRightToLeftChanged() {
			LayoutChanged();
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			CheckRightToLeft();
		}
		private void OnHtmlImagesChangedCore(ImageCollection prev, ImageCollection curr) {
			if(prev != null)
				prev.Changed -= new EventHandler(OnHtmlImagesChanged);
			if(curr != null)
				curr.Changed += new EventHandler(OnHtmlImagesChanged);
			LayoutChanged();
		}
		void OnHtmlImagesChanged(object sender, EventArgs e) {
			if(AllowHtmlString) {
				LayoutChanged();
			}
		}
		protected internal virtual void OnInitContentControl(Control control) {
			if(control == null || IsLoading) return;
			if(!IsDesignMode) {
				if(!Controls.Contains(control)) Controls.Add(control);
				control.Visible = false;
			}
		}
		protected internal virtual void OnDestroyContentControl(Control control) {
			if(control == null || IsLoading) return;
			if(!IsDesignMode) {
				if(Controls.Contains(control)) Controls.Remove(control);
			}
		}
		protected internal bool ContainsControl(Control control) {
			if(control == null) return false;
			return Controls.Contains(control);
		}
		protected internal IEnumerable<NavBarItemLink> GetAllLinks() {
			foreach(NavBarGroup group in Groups) {
				foreach(NavBarItemLink link in group.ItemLinks) {
					yield return link;
				}
			}
		}
		protected override void Dispose(bool disposing) {
			NavBarLocalizer.ActiveChanged -= new EventHandler(OnLocalizerChanged);
			ToolTipController = null;
			ToolTipController.DefaultController.RemoveClientControl(this);
			if(disposing) {
				if(StateInfo != null) {
					StateInfo.Dispose();
					StateInfo = null;
				}
				this.BeginUpdate();
				if(optionsNavPane != null) {
					optionsNavPane.Changed -= optionsNavPane_Changed;
					optionsNavPane.Dispose();
				}
				optionsNavPane = null;
				Groups.Clear();
				Items.Clear();
				ViewInfo.Dispose();
				if(LookAndFeel != null) {
					LookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
					LookAndFeel.Dispose();
				}
				if(this.appearance != null) {
					this.appearance.Changed -= new EventHandler(OnAppearanceChanged);
					this.appearance.Dispose();
				}
				HookManager.DefaultManager.RemoveController(hook);
				hook = null;
			}
			base.Dispose(disposing);
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			if(parentForm != null) parentForm.Deactivate -= new EventHandler(OnDeactivate);
			parentForm = FindForm();
			if(parentForm != null) parentForm.Deactivate += new EventHandler(OnDeactivate);
		}
		Form parentForm = null;
		internal bool skipDeactivate = false;
		protected virtual void OnDeactivate(object sender, EventArgs e) {
			if(skipDeactivate) {
				skipDeactivate = false;
				return;
			}
			ViewInfo.ClearPressedInfo();
			Form formCore = Form.ActiveForm;
			if(formCore != null && formCore.Parent == FindForm()) return;
			HideNavPaneForm();
		}
		protected virtual NavGroupCollection CreateGroupCollection() { return new NavGroupCollection(this); }
		protected virtual NavItemCollection CreateItemCollection() { return new NavItemCollection(this); }
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(ViewInfo == null) return base.CreateAccessibilityInstance();
			return ViewInfo.DXAccessible.Accessible;
		}
#if DXWhidbey
		protected internal virtual void AccessibleNotifyClients(AccessibleEvents accEvent, int objectId) {
			AccessibilityNotifyClients(accEvent, objectId, -1);
		}
		protected override AccessibleObject GetAccessibilityObjectById(int objectId) {
			BaseAccessible accessible = ViewInfo.DXAccessible;
			if(accessible == null || accessible.Children == null)
				return base.GetAccessibilityObjectById(objectId);
			objectId--;
			int groupIndex = objectId / 1000;
			int itemIndex = objectId % 1000;
			if(ViewInfo is NavigationPaneViewInfo)
				groupIndex = 0;
			else if(accessible.Children.Count > 0 && accessible.Children[0] is NavBarScrollButton)
				groupIndex++;
			if(groupIndex < 0 || groupIndex >= accessible.ChildCount)
				return base.GetAccessibilityObjectById(objectId);
			if(accessible.Children[groupIndex].Children != null && itemIndex < accessible.Children[groupIndex].Children.Count)
				return accessible.Children[groupIndex].Children[itemIndex].Accessible;
			return base.GetAccessibilityObjectById(objectId);
		}
#endif
		[Category("Appearance"), 
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlContentButtonHint"),
#endif
 DefaultValue("")]
		public string ContentButtonHint {
			get { return contentButtonHint; }
			set { contentButtonHint = value; }
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlToolTipController"),
#endif
 DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(value == ToolTipController.DefaultController) value = null;
				if(ToolTipController == value) return;
				if(ToolTipController != null) {
					ToolTipController.RemoveClientControl(this);
					if(ToolTipController != ToolTipController.DefaultController)
						ToolTipController.Disposed -= new EventHandler(OnToolTipControllerDisposed);
				}
				toolTipController = value;
				if(ToolTipController != null) {
					ToolTipController.AddClientControl(this);
					if(ToolTipController != ToolTipController.DefaultController)
						ToolTipController.Disposed += new EventHandler(OnToolTipControllerDisposed);
					ToolTipController.DefaultController.RemoveClientControl(this);
				}
				else
					ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		[DefaultValue(true)]
		public override bool AllowDrop {
			get { return base.AllowDrop; }
			set { base.AllowDrop = value; }
		}
		public ToolTipController GetToolTipController() {
			if(ToolTipController == null) return ToolTipController.DefaultController;
			return ToolTipController;
		}
		protected void OnToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return ViewInfo == null ? null : ViewInfo.GetTooltipObjectInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return ViewInfo != null && ViewInfo.AllowTooltipController; } }
		string ISkinProvider.SkinName {
			get {
				ISkinProvider provider = View as ISkinProvider;
				if(provider != null) return provider.SkinName;
				return ((ISkinProvider)LookAndFeel).SkinName;
			}
		}
		bool ISkinProviderEx.GetTouchUI() {
			return LookAndFeel.GetTouchUI();
		}
		float ISkinProviderEx.GetTouchScaleFactor() {
			return LookAndFeel.GetTouchScaleFactor();
		}
		Color ISkinProviderEx.GetMaskColor() {
			return LookAndFeel.GetMaskColor();
		}
		Color ISkinProviderEx.GetMaskColor2() {
			return LookAndFeel.GetMaskColor2();
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlTabStop"),
#endif
 DefaultValue(false)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Cursor Cursor {
			get { return base.Cursor; }
			set {
				base.Cursor = value;
				originalCursor = Cursor;
			}
		}
		public virtual void SetCursor(Cursor newCursor) {
			base.Cursor = newCursor;
		}
		public virtual void RestoreCursor() {
			base.Cursor = originalCursor;
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlMenuManager"),
#endif
 DefaultValue(null), Category("BarManager")]
		public IDXMenuManager MenuManager {
			get { return menuManager; }
			set { menuManager = value; }
		}
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavBarAppearances Appearance {
			get { return appearance; }
		}
		[DefaultValue(""), Category("Data")]
		public virtual string LayoutVersion {
			get { return layoutVersion; }
			set {
				if(value == null) value = "";
				layoutVersion = value;
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlOptionsLayout"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Layout)]
		public NavBarLayoutOptions OptionsLayout {
			get { return optionsLayout; }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlAllowGlyphSkinning"),
#endif
 Category("Appearance"), DefaultValue(false), XtraSerializableProperty()]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlAllowSelectedLink"),
#endif
 Category("Behavior"), DefaultValue(false),
			Obsolete("This property is obsolete. Use LinkSelectionMode instead.", false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool AllowSelectedLink {
			get { return AllowSelectedLinkCore; }
			set {
				AllowSelectedLinkCore = value;
			}
		}
		internal virtual bool AllowSelectedLinkCore {
			get { return allowSelectedLink; }
			set {
				if(AllowSelectedLinkCore == value) return;
				bool prevAllow = GetAllowSelectedLink();
				allowSelectedLink = value;
				if(prevAllow != GetAllowSelectedLink()) {
					if(NeedUpdateLinkSelectionMode(allowSelectedLink, true))
						UpdateLinkSelectionMode(value, EachGroupHasSelectedLinkCore);
					LayoutChanged();
				}
			}
		}
		bool NeedUpdateLinkSelectionMode(bool next, bool isAllow) {
			if(isAllow) {
				if(next && LinkSelectionMode != LinkSelectionModeType.None) return false;
				if(!next && LinkSelectionMode == LinkSelectionModeType.None) return false;
			}
			else {
				if(next && (LinkSelectionMode == LinkSelectionModeType.OneInGroup || LinkSelectionMode == LinkSelectionModeType.OneInGroupAndAllowAutoSelect)) return false;
				if(!next && (LinkSelectionMode == LinkSelectionModeType.OneInControl || LinkSelectionMode == LinkSelectionModeType.None)) return false;
			}
			return true;
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlEachGroupHasSelectedLink"),
#endif
 Category("Behavior"), DefaultValue(false),
			ObsoleteAttribute("This property is obsolete. Use LinkSelectionMode instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool EachGroupHasSelectedLink {
			get { return EachGroupHasSelectedLinkCore; }
			set { EachGroupHasSelectedLinkCore = value; }
		}
		internal bool EachGroupHasSelectedLinkCore {
			get { return eachGroupHasSelectedLink; }
			set {
				if(EachGroupHasSelectedLinkCore == value) return;
				eachGroupHasSelectedLink = value;
				if(NeedUpdateLinkSelectionMode(eachGroupHasSelectedLink, false))
					UpdateLinkSelectionMode(AllowSelectedLinkCore, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlExplorerBarShowGroupButtons"),
#endif
 DefaultValue(true), Category("Appearance"), XtraSerializableProperty()]
		public bool ExplorerBarShowGroupButtons {
			get { return explorerBarShowGroupButtons; }
			set {
				if(ExplorerBarShowGroupButtons == value) return;
				explorerBarShowGroupButtons = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlLinkSelectionMode"),
#endif
 Category("Behavior"), XtraSerializableProperty(), DefaultValue(LinkSelectionModeType.None)]
		public LinkSelectionModeType LinkSelectionMode {
			get { return linkSelectionMode; }
			set {
				if(value == LinkSelectionMode) return;
				linkSelectionMode = value;
				OnLinkSelectionModeChanged();
			}
		}
		void OnLinkSelectionModeChanged() {
			if(linkSelectionMode == LinkSelectionModeType.None) {
				AllowSelectedLinkCore = false;
				return;
			}
			AllowSelectedLinkCore = true;
			if(linkSelectionMode == LinkSelectionModeType.OneInControl) {
				EachGroupHasSelectedLinkCore = false;
			}
			if(linkSelectionMode == LinkSelectionModeType.OneInGroup || linkSelectionMode == LinkSelectionModeType.OneInGroupAndAllowAutoSelect) {
				EachGroupHasSelectedLinkCore = true;
				UpdateSelectedLink();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlNavigationPaneOverflowPanelUseSmallImages"),
#endif
 DefaultValue(true), Category("Appearance"), XtraSerializableProperty()]
		public bool NavigationPaneOverflowPanelUseSmallImages {
			get { return navigationPaneOverflowPanelUseSmallImages; }
			set {
				if(NavigationPaneOverflowPanelUseSmallImages == value) return;
				navigationPaneOverflowPanelUseSmallImages = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlExplorerBarStretchLastGroup"),
#endif
 Category("Appearance"), DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ExplorerBarStretchLastGroup {
			get { return explorerBarStretchLastGroup; }
			set {
				if(ExplorerBarStretchLastGroup == value) return;
				explorerBarStretchLastGroup = value;
				LayoutChanged();
			}
		}
		protected internal bool AllowStretchGroup(NavBarGroup group) {
			if(!ExplorerBarStretchLastGroup || !IsGroupLast(group) || ViewInfo.ScrollBarVisible) return false;
			return group.GroupStyle == NavBarGroupStyle.ControlContainer;
		}
		bool IsGroupLast(NavBarGroup group) {
			if(group == null || Groups.Count == 0) return false;
			return Groups.IndexOf(group) == Groups.Count - 1;
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlNavigationPaneMaxVisibleGroups"),
#endif
 DefaultValue(-1), Category("Appearance"), XtraSerializableProperty()]
		public int NavigationPaneMaxVisibleGroups {
			get { return navigationPaneMaxVisibleGroups; }
			set {
				if(value < 0) value = -1;
				if(NavigationPaneMaxVisibleGroups == value) return;
				navigationPaneMaxVisibleGroups = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlExplorerBarGroupInterval"),
#endif
 DefaultValue(-1), Category("Appearance"), XtraSerializableProperty()]
		public int ExplorerBarGroupInterval {
			get { return explorerBarGroupInterval; }
			set {
				if(value < 0) value = -1;
				if(ExplorerBarGroupInterval == value) return;
				explorerBarGroupInterval = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlExplorerBarGroupOuterIndent"),
#endif
 DefaultValue(-1), Category("Appearance"), XtraSerializableProperty()]
		public int ExplorerBarGroupOuterIndent {
			get { return explorerBarGroupOuterIndent; }
			set {
				if(value < 0) value = -1;
				if(ExplorerBarGroupOuterIndent == value) return;
				explorerBarGroupOuterIndent = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlSkinExplorerBarViewScrollStyle"),
#endif
 DefaultValue(SkinExplorerBarViewScrollStyle.Default), Category("Appearance"), XtraSerializableProperty()]
		public SkinExplorerBarViewScrollStyle SkinExplorerBarViewScrollStyle {
			get { return skinExplorerBarViewScrollStyle; }
			set {
				if(SkinExplorerBarViewScrollStyle == value) return;
				skinExplorerBarViewScrollStyle = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlLinkInterval"),
#endif
 DefaultValue(-1), Category("Appearance"), XtraSerializableProperty()]
		public int LinkInterval {
			get { return linkInterval; }
			set {
				if(value < 0) value = -1;
				if(LinkInterval == value) return;
				linkInterval = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlNavigationPaneGroupClientHeight"),
#endif
 DefaultValue(100), Category("Appearance"), XtraSerializableProperty()]
		public int NavigationPaneGroupClientHeight {
			get { return navigationPaneGroupClientHeight; }
			set {
				if(value < 10) value = 10;
				if(NavigationPaneGroupClientHeight == value) return;
				navigationPaneGroupClientHeight = value;
				LayoutChanged();
			}
		}
		bool selectLinkOnPress = true;
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlSelectLinkOnPress"),
#endif
 DefaultValue(true), Category("Behavior"), XtraSerializableProperty()]
		public bool SelectLinkOnPress {
			get { return selectLinkOnPress; }
			set { selectLinkOnPress = value; }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlBorderStyle"),
#endif
 DefaultValue(BorderStyles.Default), Category("Appearance"), XtraSerializableProperty()]
		public BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlShowGroupHint"),
#endif
 Category("Behavior"), DefaultValue(true), XtraSerializableProperty()]
		public bool ShowGroupHint {
			get { return showGroupHint; }
			set {
				if(ShowGroupHint == value) return;
				showGroupHint = value;
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlShowHintInterval"),
#endif
 Category("Behavior"), DefaultValue(1000)]
		public int ShowHintInterval {
			get { return showHintInterval; }
			set {
				if(value < 100) value = 100;
				if(ShowHintInterval == value) return;
				showHintInterval = value;
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlShowLinkHint"),
#endif
 Category("Behavior"), DefaultValue(true), XtraSerializableProperty()]
		public bool ShowLinkHint {
			get { return showLinkHint; }
			set {
				if(ShowLinkHint == value) return;
				showLinkHint = value;
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlDragDropFlags"),
#endif
 Category("Behavior"), DefaultValue(NavBarDragDrop.AllowDrag | NavBarDragDrop.AllowDrop),
		Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor)), XtraSerializableProperty()]
		public virtual NavBarDragDrop DragDropFlags {
			get { return dragDropFlags; }
			set {
				dragDropFlags = value;
			}
		}
		public virtual NavBarDragDrop GetDragDropFlags() {
			if(DragDropFlags == NavBarDragDrop.Default) return NavBarDragDrop.AllowDrag | NavBarDragDrop.AllowDrop;
			return DragDropFlags;
		}
		[Browsable(false)]
		public virtual NavBarViewCollection AvailableNavBarViews {
			get { return availableNavBarViews; }
		}
		protected virtual NavBarDesignTimeManager CreateDesignManager() {
			return new NavBarDesignTimeManager(this);
		}
		protected internal NavBarDesignTimeManager DesignManager { get { return designManager; } }
		protected virtual void RegisterAvailableNavBarViews() {
			AvailableNavBarViews.Clear();
			AvailableNavBarViews.Add(new BaseViewInfoRegistrator());
			AvailableNavBarViews.Add(new FlatViewInfoRegistrator());
			AvailableNavBarViews.Add(new Office1ViewInfoRegistrator());
			AvailableNavBarViews.Add(new Office2ViewInfoRegistrator());
			AvailableNavBarViews.Add(new Office3ViewInfoRegistrator());
			AvailableNavBarViews.Add(new VSToolBoxViewInfoRegistrator());
			AvailableNavBarViews.Add(new AdvExplorerBarViewInfoRegistrator());
			AvailableNavBarViews.Add(new ExplorerBarViewInfoRegistrator());
			AvailableNavBarViews.Add(new UltraFlatExplorerBarViewInfoRegistrator());
			AvailableNavBarViews.Add(new SkinExplorerBarViewInfoRegistrator());
			AvailableNavBarViews.Add(new XP1ViewInfoRegistrator());
			AvailableNavBarViews.Add(new XP2ViewInfoRegistrator());
			AvailableNavBarViews.Add(new XPExplorerBarViewInfoRegistrator());
			AvailableNavBarViews.Add(new NavigationPaneViewInfoRegistrator());
			AvailableNavBarViews.Add(new SkinNavigationPaneViewInfoRegistrator());
			RegisterSkinViews();
			RegisterXMLNavBarViews();
			RegisterDesignTimeViews();
		}
		protected virtual void RegisterSkinViews() {
			foreach(SkinContainer container in SkinManager.Default.GetRuntimeSkins()) {
				AvailableNavBarViews.Add(new StandardSkinExplorerBarViewInfoRegistrator(container.SkinName));
			}
			foreach(SkinContainer container in SkinManager.Default.GetRuntimeSkins()) {
				AvailableNavBarViews.Add(new StandardSkinNavigationPaneViewInfoRegistrator(container.SkinName));
			}
		}
		protected override void OnSystemColorsChanged(EventArgs e) {
			base.OnSystemColorsChanged(e);
			DevExpress.Utils.WXPaint.Painter.ThemeChanged();
			AvailableNavBarViews.UpdateThemeColors();
			if(IsLoading) return;
			UpdateView();
		}
		protected override Size DefaultSize { get { return new Size(140, 300); } }
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			if(ViewInfo != null) ViewInfo.SetPaintAppearanceDirty();
			LayoutChanged();
		}
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			if(IsLoading) return;
			AvailableNavBarViews.UpdateThemeColors();
			ViewInfo.SetPaintAppearanceDirty();
			UpdateView();
			CheckRightToLeft();
			LayoutChanged();
		}
		protected virtual void UpdateView() {
			try {
				if(PaintStyleName != DefaultPaintStyleName) {
					BaseViewInfoRegistrator view = AvailableNavBarViews[PaintStyleName];
					if(view == null) {
						PaintStyleName = DefaultPaintStyleName;
						return;
					}
					View = view;
					return;
				}
				NavBarViewKind kind = PaintStyleKind;
				ActiveLookAndFeelStyle activeStyle = LookAndFeel.ActiveStyle;
				switch(kind) {
					case NavBarViewKind.ExplorerBar:
					case NavBarViewKind.Default:
						SetViewCore(GetExplorerView(activeStyle));
						break;
					case NavBarViewKind.NavigationPane:
						SetViewCore(GetNavigationPaneView(activeStyle));
						break;
					case NavBarViewKind.SideBar:
						SetViewCore(GetSideBarView(activeStyle));
						break;
				}
			}
			finally {
				FireChanged();
			}
		}
		protected virtual BaseViewInfoRegistrator GetExplorerView(ActiveLookAndFeelStyle activeStyle) {
			string paintStyle = NavBarViewNames.AdvExporerBar;
			switch(activeStyle) {
				case ActiveLookAndFeelStyle.Skin:
					paintStyle = NavBarViewNames.SkinExplorerBar; break;
				case ActiveLookAndFeelStyle.UltraFlat:
				case ActiveLookAndFeelStyle.Style3D:
				case ActiveLookAndFeelStyle.Flat: paintStyle = NavBarViewNames.FlatExplorerBar; break;
			}
			return AvailableNavBarViews[paintStyle];
		}
		protected virtual BaseViewInfoRegistrator GetNavigationPaneView(ActiveLookAndFeelStyle activeStyle) {
			if(activeStyle == ActiveLookAndFeelStyle.Skin) return AvailableNavBarViews[NavBarViewNames.SkinNavigationPane];
			return AvailableNavBarViews[NavBarViewNames.NavigationPane];
		}
		protected virtual BaseViewInfoRegistrator GetSideBarView(ActiveLookAndFeelStyle activeStyle) {
			string paintStyle = NavBarViewNames.FlatSideBar;
			switch(activeStyle) {
				case ActiveLookAndFeelStyle.Style3D: paintStyle = NavBarViewNames.SideBar3D; break;
				case ActiveLookAndFeelStyle.WindowsXP: paintStyle = NavBarViewNames.XPSideBar; break;
			}
			return AvailableNavBarViews[paintStyle];
		}
		bool ShouldSerializeView() { return PaintStyleName != DefaultPaintStyleName; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public BaseViewInfoRegistrator View {
			get { return view; }
			set {
				SetViewCore(value, true);
			}
		}
		protected void SetViewCore(BaseViewInfoRegistrator value) {
			SetViewCore(value, false);
		}
		protected virtual void SetViewCore(BaseViewInfoRegistrator value, bool changePaintStyleName) {
			if(changePaintStyleName || PaintStyleName != DefaultPaintStyleName) {
				paintStyleName = value.ViewName;
			}
			view = value;
			OnViewChanged();
			if(IsDesignMode && !IsLoading) {
				ResetStyles();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ViewName {
			get { return View == null ? null : View.ViewName; }
			set {
				if(ViewName == value) return;
				BaseViewInfoRegistrator info = AvailableNavBarViews[value];
				if(info == null) return;
				View = info;
			}
		}
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlLookAndFeel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlPaintStyleKind"),
#endif
 DefaultValue(NavBarViewKind.Default), Category("Appearance")]
		public NavBarViewKind PaintStyleKind {
			get { return paintStyleKind; }
			set {
				if(PaintStyleKind == value) return;
				paintStyleKind = value;
				UpdateView();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlSharedImageCollectionImageSizeMode"),
#endif
 DefaultValue(SharedImageCollectionImageSizeMode.UseCollectionImageSize), Category("Appearance")]
		public SharedImageCollectionImageSizeMode SharedImageCollectionImageSizeMode {
			get { return collectionSizeMode; }
			set {
				if(collectionSizeMode == value) return;
				collectionSizeMode = value;
			}
		}
		protected virtual OptionsNavPane CreateOptionsNavPane() {
			return new OptionsNavPane(this);
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlOptionsNavPane"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public OptionsNavPane OptionsNavPane {
			get {
				if(optionsNavPane == null) {
					optionsNavPane = CreateOptionsNavPane();
					optionsNavPane.Changed += new BaseOptionChangedEventHandler(optionsNavPane_Changed);
				}
				return optionsNavPane;
			}
			set { optionsNavPane = value; }
		}
		void optionsNavPane_Changed(object sender, BaseOptionChangedEventArgs e) {
			if(e.Name == "NavPaneState") {
				if((NavPaneState)e.NewValue == NavPaneState.Collapsed && OptionsNavPane.ExpandedWidth < 0) OptionsNavPane.ExpandedWidth = Width;
				ProcessExpandChanged(OptionsNavPane.NavPaneState == NavPaneState.Expanded);
			}
			else if(e.Name == "ExpandedWidth") {
				if(!IsLoading && OptionsNavPane.NavPaneState == NavPaneState.Expanded && OptionsNavPane.ExpandedWidth != -1) {
					Width = OptionsNavPane.ExpandedWidth;
				}
			}
			else if(e.Name == "CollapsedWidth") {
				if(OptionsNavPane.NavPaneState == NavPaneState.Collapsed && !IsLoading) {
					UpdateCollapsedNavBarSize();
				}
			}
		}
		[DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool StoreDefaultPaintStyleName {
			get { return storeDefaultPaintStyleName; }
			set { storeDefaultPaintStyleName = value; }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlPaintStyleName"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DefaultValue(DefaultPaintStyleName),
		TypeConverter("DevExpress.XtraNavBar.Design.NavBarViewNamesConverter, " + AssemblyInfo.SRAssemblyNavBarDesign)]
		public string PaintStyleName {
			get { return paintStyleName; }
			set {
				if(value == null) paintStyleName = DefaultPaintStyleName;
				if(PaintStyleName == value) return;
				paintStyleName = value;
				StoreDefaultPaintStyleName = PaintStyleName == DefaultPaintStyleName;
				UpdateView();
			}
		}
		protected virtual void OnViewChanged() {
			NavBarViewInfo oldViewInfo = this.viewInfo;
			this.viewInfo = CreateViewInfo();
			if(oldViewInfo != null && oldViewInfo.dxAccessible != null) this.viewInfo.DXAccessible.Substitute(oldViewInfo.DXAccessible);
			CreatePainters();
			if(oldViewInfo != null) oldViewInfo.Dispose();
			ViewInfo.SetPaintAppearanceDirty();
			LayoutChanged();
			FireChanged();
		}
		protected internal void MakeGroupVisible(NavBarGroup group) {
			ViewInfo.MakeGroupVisible(group);
		}
		[Browsable(false)]
		public NavBarGroup PressedGroup {
			get { return ViewInfo.PressedGroup; }
		}
		[Browsable(false)]
		public NavBarState State { get { return ViewInfo.State; } }
		[Browsable(false)]
		public NavBarGroup HotTrackedGroup {
			get { return ViewInfo.HotTrackedGroup; }
		}
		[Browsable(false)]
		public NavBarItemLink HotTrackedLink {
			get { return ViewInfo.HotTrackedLink; }
		}
		[Browsable(false)]
		public NavBarItemLink PressedLink {
			get { return ViewInfo.PressedLink; }
		}
		string activeGroupName = string.Empty;
		[XtraSerializableProperty, 
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlActiveGroupName"),
#endif
 Category("Appearance"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ActiveGroupName {
			get {
				if(ActiveGroup != null)
					activeGroupName = ActiveGroup.Name;
				return activeGroupName;
			}
			set {
				if(IsDeserializing) {
					activeGroupName = value;
					return;
				}
				SetActiveGroup(value);
			}
		}
		void SetActiveGroup() { SetActiveGroup(activeGroupName); }
		void SetActiveGroup(string groupName) {
			foreach(NavBarGroup group in Groups) {
				if(group.Name != groupName) continue;
				ActiveGroup = group;
				activeGroupName = group.Name;
				break;
			}
		}
		protected internal NavBarGroup FindVisibleGroup() {
			foreach(NavBarGroup group in Groups) {
				if(group.IsVisible) return group;
			}
			return null;
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlActiveGroup"),
#endif
 Category("Appearance")]
		public NavBarGroup ActiveGroup {
			get {
				if(activeGroup == null) {
					activeGroup = FindVisibleGroup();
					if(activeGroup != null)
						RaiseActiveGroupChanged(activeGroup);
				}
				return activeGroup;
			}
			set {
				if(ActiveGroup == value) return;
				if(value != null && !value.IsVisible) return;
				activeGroup = value;
				if(LinkSelectionMode == LinkSelectionModeType.OneInGroupAndAllowAutoSelect &&
					(ViewInfo is NavigationPaneViewInfo || ViewInfo is XP2NavBarViewInfo))
					UpdateSelectedLink();
				ViewInfo.ResetAccessible();
				LayoutChanged();
				RaiseActiveGroupChanged(ActiveGroup);
			}
		}
		protected internal void UpdateLinkSelectionMode(bool allowSelection, bool eachgroup) {
			if(!allowSelection) {
				LinkSelectionMode = LinkSelectionModeType.None;
				return;
			}
			if(!eachgroup) {
				LinkSelectionMode = LinkSelectionModeType.OneInControl;
				return;
			}
			if(LinkSelectionMode == LinkSelectionModeType.OneInGroupAndAllowAutoSelect || LinkSelectionMode == LinkSelectionModeType.OneInGroup) return;
			LinkSelectionMode = LinkSelectionModeType.OneInGroup;
		}
		[Obsolete("This method is obsolete. Use ShowNavPaneForm instead.", false)]
		public void NavPaneShowCollapsedGroupContent() {
			ShowNavPaneForm();
		}
		public void ShowNavPaneForm() {
			if(PaintStyleKind != NavBarViewKind.NavigationPane || OptionsNavPane.NavPaneState != NavPaneState.Collapsed) return;
			ViewInfo.ActivateGroupContent();
		}
		public void UpdateSelectedLink() {
			if(activeGroup != null && activeGroup.VisibleItemLinks.Count != 0) {
				NavBarItemLink link = GetLinkByGroup(activeGroup);
				if(!link.AllowAutoSelect) {
					SelectedLink = null;
					return;
				}
				if(SelectedLink == link)
					RaiseSelectedLinkChanged(new NavBarSelectedLinkChangedEventArgs(activeGroup, SelectedLink));
				else SelectedLink = link;
			}
		}
		public virtual void BeginInit() {
			if(lockInit++ == 0) {
				if(firstBeginInit) {
					firstBeginInit = false;
				}
			}
		}
		public virtual void EndInit() {
			for(int i = 0; i < Groups.Count; i++) {
				if(Groups[i].ImageUri != null) Groups[i].ImageUri.SetClient(Groups[i]);
			}
			for(int i = 0; i < Items.Count; i++) {
				if(Items[i].ImageUri != null) Items[i].ImageUri.SetClient(Items[i]);
			}
			HookDesignerLoad();
			if(--lockInit == 0) {
				if(StoreDefaultPaintStyleName) {
					this.paintStyleName = DefaultPaintStyleName;
				}
				UpdateView();
				ViewInfo.EnsureGroupControls();
				if(OptionsNavPane.ExpandedWidth < 0) OptionsNavPane.ExpandedWidth = Size.Width;
			}
		}
		protected virtual bool AllowAutoFitGroupClientHeight { get { return true; } }
		protected virtual void DoAutoFitGroupClientHeight(SizeF scaleFactor) {
			if(!AllowAutoFitGroupClientHeight || DesignMode) return;
			foreach(NavBarGroup group in Groups) {
				if(group.GroupClientHeight != -1)
					group.GroupClientHeight = CalcScaleGroupClientHeight(scaleFactor, group);
			}
		}
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			if(factor.Height > 1)
				DoAutoFitGroupClientHeight(factor);
			base.ScaleControl(factor, specified);
		}
		int CalcScaleGroupClientHeight(SizeF scaleFactor, NavBarGroup group) {
			SkinExplorerBarViewInfoRegistrator reg = View as SkinExplorerBarViewInfoRegistrator;
			if(reg != null) {
				SkinElement elem = reg.GetSkin(this)[NavBarSkins.SkinGroupClient];
				if(elem != null) {
					int groupClientHeight = group.GroupClientHeight - elem.ContentMarginsCore.Top - elem.ContentMarginsCore.Bottom;
					return (int)(group.GroupClientHeight * scaleFactor.Height) + elem.ContentMargins.Top + elem.ContentMargins.Bottom;
				}
			}
			return (int)(group.GroupClientHeight * scaleFactor.Height);
		}
		bool ShouldScaleControlContainer { get { return View is ExplorerBarViewInfoRegistrator; } }
		IDesignerHost designerHost = null;
		protected void HookDesignerLoad() {
			if(this.designerHost != null) return;
			this.designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(this.designerHost == null && Parent != null && Parent.Site != null) this.designerHost = Parent.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(this.designerHost != null) designerHost.LoadComplete += new EventHandler(OnLoadComplete);
		}
		protected void RemoveOnLoadedHook() {
			if(this.designerHost != null)
				this.designerHost.LoadComplete -= new EventHandler(OnLoadComplete);
			this.designerHost = null;
		}
		protected void OnLoadComplete(object sender, EventArgs e) {
			bool prev = ControlUtils.EnableComponentNotifications(false, this);
			RemoveOnLoadedHook();
			foreach(NavBarGroup group in Groups) {
				group.OnLoaded();
			}
			LayoutChanged();
			ControlUtils.EnableComponentNotifications(prev, this);
		}
		protected bool IsAllowFireEvents { get { return !IsLoading; } }
		[Browsable(false)]
		public virtual bool IsLoading { get { return lockInit != 0; } }
		[Browsable(false)]
		public virtual bool IsDesignMode { get { return DesignMode; } }
		internal bool ShouldSerializeHotTrackedGroupCursor() { return HotTrackedGroupCursor != Cursors.Default; }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlHotTrackedGroupCursor"),
#endif
 Category("Appearance")]
		public virtual Cursor HotTrackedGroupCursor {
			get { return hotTrackedGroupCursor; }
			set {
				hotTrackedGroupCursor = value;
			}
		}
		internal bool ShouldSerializeHotTrackedItemCursor() { return HotTrackedItemCursor != Cursors.Hand; }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlHotTrackedItemCursor"),
#endif
 Category("Appearance")]
		public virtual Cursor HotTrackedItemCursor {
			get { return hotTrackedItemCursor; }
			set {
				hotTrackedItemCursor = value;
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlGroupBackgroundImage"),
#endif
 Category("Appearance"), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image GroupBackgroundImage {
			get { return groupBackgroundImage; }
			set {
				if(GroupBackgroundImage == value) return;
				groupBackgroundImage = value;
				UpdateGroupBrush();
				LayoutChanged();
			}
		}
		[Browsable(false)]
		public virtual Brush GroupTextureBackgroundBrush { get { return groupTextureBackgroundBrush; } }
		protected virtual void UpdateGroupBrush() {
			if(GroupBackgroundImage == null) groupTextureBackgroundBrush = null;
			else groupTextureBackgroundBrush = new TextureBrush(GroupBackgroundImage);
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlSmallImages"),
#endif
 Category("Appearance"), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object SmallImages {
			get { return smallImages; }
			set {
				if(SmallImages == value) return;
				smallImages = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlLargeImages"),
#endif
 Category("Appearance"), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object LargeImages {
			get { return largeImages; }
			set {
				if(LargeImages == value) return;
				largeImages = value;
				LayoutChanged();
			}
		}
		protected internal NavBarCustomizationInfo StateInfo {
			get;
			private set;
		}
		protected internal virtual void InitNavBarSourceStateInfo() {
			if(StateInfo.IsInitialized)
				return;
			StateInfo = PaneCustomizationHelper.Fill(this);
		}
		PaneCustomizationHelper customizationHelperCore = null;
		protected internal PaneCustomizationHelper PaneCustomizationHelper {
			get {
				if(customizationHelperCore == null) customizationHelperCore = CreatePaneCustomizationHelper();
				return customizationHelperCore;
			}
		}
		protected internal virtual PaneCustomizationHelper CreatePaneCustomizationHelper() {
			return new PaneCustomizationHelper();
		}
		bool fireDelayedSelectedLinkChanged = false;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(false)]
		public bool FireDelayedSelectedLinkChangedEvent {
			get { return fireDelayedSelectedLinkChanged; }
			set { fireDelayedSelectedLinkChanged = value; }
		}
		internal object XtraCreateGroupsItem(XtraItemEventArgs e) { return Groups.Add(); }
		internal object XtraFindGroupsItem(XtraItemEventArgs e) {
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo pi = e.Item.ChildProperties["Name"];
			if(pi == null) return null;
			groupsOrder.Add(pi.Value.ToString());
			return Groups[pi.Value.ToString()];
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlGroups"),
#endif
 Category("Data"), RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(true, true, false, 999),
		Editor("DevExpress.XtraNavBar.Design.NavBarControlGroupsCollectionEditor, " + AssemblyInfo.SRAssemblyNavBarDesign, typeof(System.Drawing.Design.UITypeEditor))
#if DXWhidbey
, DevExpress.Utils.Design.InheritableCollection
#endif
]
		public NavGroupCollection Groups {
			get { return groups; }
		}
		internal object XtraCreateItemsItem(XtraItemEventArgs e) {
			return Items.Add(); 
		}
		internal object XtraFindItemsItem(XtraItemEventArgs e) {
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo pi = e.Item.ChildProperties["Name"];
			if(pi == null) return null;
			return Items[pi.Value.ToString()];
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlItems"),
#endif
 Category("Data"), RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(true, true, false),
		Editor("DevExpress.XtraNavBar.Design.NavBarControlItemsCollectionEditor, " + AssemblyInfo.SRAssemblyNavBarDesign, typeof(System.Drawing.Design.UITypeEditor))
#if DXWhidbey
, DevExpress.Utils.Design.InheritableCollection
#endif
]
		public NavItemCollection Items {
			get { return items; }
		}
		protected virtual NavBarViewInfo CreateViewInfo() {
			return View.CreateViewInfo(this);
		}
		public NavBarViewInfo GetViewInfo() { return ViewInfo; }
		protected internal virtual NavBarViewInfo ViewInfo {
			get { return viewInfo; }
		}
		public virtual NavBarHitInfo CalcHitInfo(Point p) {
			CheckViewInfo();
			return ViewInfo.CalcHitInfo(p);
		}
		protected internal bool IsLockLayout { get { return lockLayout != 0; } }
		public virtual void BeginUpdate() {
			lockLayout++;
		}
		protected virtual void CancelUpdate() {
			lockLayout--;
		}
		public virtual void EndUpdate() {
			if(--lockLayout == 0)
				LayoutChanged();
		}
		protected virtual void LayoutChangedCore() {
			ViewInfo.Clear();
			CheckViewInfo();
			foreach(NavBarGroup group in Groups) {
				if(group.ControlContainer != null && group.ControlContainer.Visible) group.ControlContainer.Invalidate();
				if(group.DelayedSetSelectedLink && FireDelayedSelectedLinkChangedEvent) {
					group.DelayedSetSelectedLink = false;
					if(group.NewSelectedLink == null)
						group.SelectedLinkIndex = -1;
					else
						group.SelectedLink = group.NewSelectedLink;
				}
			}
			Invalidate();
		}
		public virtual void LayoutChanged() {
			if(IsLoading || IsLockLayout || !handleAlreadyCreated) return;
			LayoutChangedCore();
		}
		protected override void CreateHandle() {
			base.CreateHandle();
			handleAlreadyCreated = true;
		}
		bool handleAlreadyCreated = false;
		protected override void OnPaint(PaintEventArgs e) {
			if(IsLockLayout) return;
			CheckViewInfo();
			ViewInfo.Draw(e);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			ViewInfo.Clear();
			HideNavPaneForm();
			ViewInfo.TopY = ViewInfo.TopY;
		}
		protected internal virtual void CheckViewInfo() {
			if(ViewInfo.IsReady) return;
			ViewInfo.Calc(ClientRectangle);
		}
		protected internal void ForceLayoutChanged() {
			ViewInfo.Clear();
			CheckViewInfo();
		}
		protected virtual void OnGroupCollectionChanged(object sender, CollectionChangeEventArgs e) {
			NavBarGroup group = e.Element as NavBarGroup;
			BeginUpdate();
			try {
				switch(e.Action) {
					case CollectionChangeAction.Add:
						group.SetNavBarCore(this);
						group.ItemChanged += new EventHandler(OnGroupChanged);
						if(Site != null) {
							if(!IsLoading || IsDeserializing) {
								Site.Container.Add(group);
								if(!IsDeserializing && group.Caption == group.DefaultCaption && group.Name != string.Empty) group.Caption = group.Name;
							}
						}
						break;
					case CollectionChangeAction.Remove:
						group.ItemChanged -= new EventHandler(OnGroupChanged);
						if(!DesignMode)
						   group.Dispose();
						if(ActiveGroup == group) {
							activeGroup = Groups.Count == 0 ? null : Groups[0];
						}
						break;
				}
				FireChanged();
			}
			finally { EndUpdate(); }
			RaiseNavigationBarClientItemsSourceChanged(e);
		}
		protected virtual void OnItemCollectionChanged(object sender, CollectionChangeEventArgs e) {
			NavBarItem item = e.Element as NavBarItem;
			switch(e.Action) {
				case CollectionChangeAction.Add:
					item.SetNavBarCore(this);
					item.ItemChanged += new EventHandler(OnItemChanged);
					if(Site != null) {
						if(!IsLoading || IsDeserializing) {
							Site.Container.Add(item);
							if(!IsDeserializing && item.Caption == item.DefaultCaption && item.Name != string.Empty) item.Caption = item.Name;
						}
					}
					break;
				case CollectionChangeAction.Remove:
					item.ItemChanged -= new EventHandler(OnItemChanged);
					if(!DesignMode)
						item.Dispose();
					break;
			}
			ViewInfo.OnItemCollectionChanged();
			FireChanged();
		}
		protected virtual void OnGroupChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected virtual void OnItemChanged(object sender, EventArgs e) {
			NavBarItem item = sender as NavBarItem;
			if(item != null) {
				Hashtable hash = new Hashtable();
				foreach(NavBarItemLink link in item.Links) {
					hash[link.Group] = 1;
				}
				foreach(NavBarGroup group in hash.Keys) {
					group.RebuildVisibleLinksCore();
				}
			}
			if(NavPaneForm != null && NavPaneForm.Visible)
				NavPaneForm.OnItemChanged(sender, e);
			LayoutChanged();
		}
		internal bool ExpandGroupCore(NavBarGroup group, bool setExpanded) {
			if(group == null) return false;
			if(!AllowExpandCollapse) {
				bool prevExpanded = group.Expanded;
				if(setExpanded) {
					NavBarGroup prevActive = ActiveGroup;
					ActiveGroup = group;
					if(prevActive != null)
						prevActive.RaiseGroupExpandedChanged();
					return prevExpanded;
				}
			}
			return !group.Expanded;
		}
		protected internal bool IsGroupExpanded(NavBarGroup group) {
			if(AllowExpandCollapse) return group.ExpandedCore;
			return group == ActiveGroup;
		}
		protected virtual bool AllowExpandCollapse {
			get { return ViewInfo.AllowExpandCollapse; }
		}
		void OnLocalizerChanged(object sender, EventArgs e) {
			OnViewChanged();
		}
		public virtual bool GetAllowSelectedLink() {
			return ViewInfo.AllowSelectedLink & AllowSelectedLinkCore;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public NavBarAppearances PaintAppearance { get { return ViewInfo == null ? Appearance : ViewInfo.PaintAppearance; } }
		public virtual void ResetStyles() {
			LayoutChanged();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			this.hook = new NavBarHook(this);
			if(!DesignMode) {
				if(!IsLoading && ActiveGroup != null && ActiveGroup.ControlContainer != null)
					ActiveGroup.ControlContainer.PerformLayout();
			}
		}
		protected internal BaseNavGroupPainter GroupPainter { get { return groupPainter; } }
		protected internal BaseNavLinkPainter LinkPainter { get { return linkPainter; } }
		protected internal ObjectPainter ButtonPainter { get { return buttonPainter; } }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlHideGroupCaptions"),
#endif
 Category("Appearance"), DefaultValue(false)]
		public virtual bool HideGroupCaptions {
			get { return hideGroupCaptions; }
			set {
				if(HideGroupCaptions == value) return;
				hideGroupCaptions = value;
				LayoutChanged();
			}
		}
		public virtual bool GetHideGroupCaptions() {
			return HideGroupCaptions && ViewInfo.AllowHideGroupCaptions;
		}
		protected internal bool AllowOnlySmallImages {
			get { return ViewInfo.AllowOnlySmallImages; }
		}
		protected internal bool AllowShowAsIconsView {
			get { return ViewInfo.AllowListViewMode; }
		}
		protected internal bool AllowSelectDisabledLink {
			get { return ViewInfo.AllowSelectDisabledLink; }
		}
		protected virtual void CreatePainters() {
			groupPainter = View.CreateGroupPainter(this);
			linkPainter = View.CreateLinkPainter(this);
			buttonPainter = View.CreateButtonPainter(this);
		}
		void OnStyleChanged(object sender, EventArgs e) {
			FireChanged();
			LayoutChanged();
		}
		protected internal void FireChanged() {
			if(!DesignMode || IsLoading) return;
			System.ComponentModel.Design.IComponentChangeService srv = GetService(typeof(System.ComponentModel.Design.IComponentChangeService)) as System.ComponentModel.Design.IComponentChangeService;
			if(srv == null) return;
			srv.OnComponentChanged(this, null, null, null);
		}
		protected internal virtual int SelectItemLink(NavBarGroup group, int itemLinkIndex) {
			if(!GetAllowSelectedLink()) return -1;
			if(itemLinkIndex > group.VisibleItemLinks.Count - 1) return -1;
			if(itemLinkIndex < 0) return -1;
			if(EachGroupHasSelectedLinkCore) return itemLinkIndex;
			for(int n = 0; n < Groups.Count; n++) {
				NavBarGroup g = Groups[n];
				g.selectedLinkIndex = g == group ? itemLinkIndex : -1;
			}
			LayoutChanged();
			return itemLinkIndex;
		}
		protected internal virtual bool GetSelectLinkOnPress() {
			return ((DragDropFlags & NavBarDragDrop.AllowDrag) == NavBarDragDrop.AllowDrag) || SelectLinkOnPress;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual NavBarItemLink SelectedLink {
			get {
				if(!GetAllowSelectedLink()) return null;
				if(EachGroupHasSelectedLinkCore) {
					if(ActiveGroup == null) return null;
					return ActiveGroup.SelectedLink;
				}
				for(int n = 0; n < Groups.Count; n++) {
					NavBarGroup g = Groups[n];
					NavBarItemLink link = g.SelectedLink;
					if(link != null) return link;
				}
				return null;
			}
			set {
				NavBarItemLink prevSelected = SelectedLink;
				if(value == prevSelected) return;
				if(!GetAllowSelectedLink()) return;
				if(EachGroupHasSelectedLinkCore) {
					if(ActiveGroup == null) return;
					ActiveGroup.SelectedLink = value;
					return;
				}
				if(value != null) {
					value.Group.SelectedLink = value;
					return;
				}
				prevSelected.Group.SelectedLink = null;
			}
		}
		protected internal void AddToGroupSelectedItems(NavBarGroup group, NavBarItemLink link) {
			if(groupSelectedItems.ContainsKey(group.Caption)) {
				if(groupSelectedItems[group.Caption] == link.Caption) return;
				groupSelectedItems.Remove(group.Caption);
			}
			groupSelectedItems.Add(group.Caption, link.Caption);
		}
		protected internal NavBarItemLink GetLinkByGroup(NavBarGroup group) {
			if(groupSelectedItems.ContainsKey(group.Caption))
				return group.GetItemLinkByCaption(groupSelectedItems[group.Caption]);
			return group.GetFirstItemLink();
		}
		protected override void OnDragEnter(DragEventArgs e) {
			ViewInfo.DragDropInfo.OnDragEnter(e);
			base.OnDragEnter(e);
		}
		protected override void OnDragLeave(EventArgs e) {
			ViewInfo.DragDropInfo.OnDragLeave(e);
			base.OnDragLeave(e);
		}
		protected override void OnDragOver(DragEventArgs e) {
			ViewInfo.DragDropInfo.OnDragOver(e);
			base.OnDragOver(e);
		}
		protected override void OnDragDrop(DragEventArgs e) {
			ViewInfo.DragDropInfo.OnDragDrop(e);
			base.OnDragDrop(e);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			ViewInfo.DragDropInfo.OnGiveFeedback(e);
			base.OnGiveFeedback(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			CheckViewInfo();
			ViewInfo.OnMouseLeave(e);
			base.OnMouseLeave(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			CheckViewInfo();
			ViewInfo.OnMouseEnter(e);
			base.OnMouseEnter(e);
		}
		protected override void OnMouseMove(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseMove(e);
			if(e.Handled) return;
			CheckViewInfo();
			ViewInfo.OnMouseMove(e);
		}
		protected override void OnMouseDown(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseDown(e);
			if(e.Handled) return;
			CheckViewInfo();
			ViewInfo.OnMouseDown(e);
		}
		protected sealed override void OnMouseWheel(MouseEventArgs ev) {
			if(XtraForm.ProcessSmartMouseWheel(this, ev)) return;
			OnMouseWheelCore(ev);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseWheel(e);
			if(e.Handled) return;
			CheckViewInfo();
			ViewInfo.OnMouseWheel(e);
		}
		protected override void OnMouseUp(MouseEventArgs ev) {
			if(IsDisposed) return;
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseUp(e);
			if(e.Handled) return;
			CheckViewInfo();
			ViewInfo.OnMouseUp(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.F && e.Control && CheckAttachedToSearchControl()) {
				searchControl.Focus();
				return;
			}
			base.OnKeyDown(e);
		}
		protected internal object InternalGetService(Type service) {
			return GetService(service);
		}
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		Control ISupportXtraAnimation.OwnerControl { get { return this; } }
		protected Form Form {
			get {
				return this.FindForm();
			}
		}
		void IXtraObjectWithBounds.OnEndBoundAnimation(BoundsAnimationInfo anim) {
			if(Form != null && SuspendFormLayoutInAnimation) Form.ResumeLayout();
			OptionsNavPane.actualNavPaneState = OptionsNavPane.NavPaneState;
			OnNavPaneBoundsAnimationCompleted();
			animatedBoundsCore = Rectangle.Empty;
			RaiseSizeableChanged();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			ViewInfo.OnSizeChanged();
			if(IsDesignMode && !IsLoading && OptionsNavPane.ExpandedWidth != -1 && OptionsNavPane.NavPaneState == NavPaneState.Expanded) {
				OptionsNavPane.ExpandedWidth = Width;
			}
		}
		protected Rectangle animatedBoundsCore = Rectangle.Empty;
		Rectangle IXtraObjectWithBounds.AnimatedBounds {
			get { return Bounds; }
			set {
				int prevWidth = Bounds.Width;
				animatedBoundsCore = value;
				if(OptionsNavPane.ExpandButtonMode == ExpandButtonMode.Normal) {
					if(IsRightToLeft) Invalidate();
					if(IsRightToLeftDirection) {
						if(OptionsNavPane.NavPaneState == NavPaneState.Collapsed) {
							if(IsRightToLeft) Bounds = animatedBoundsCore;
							else Location = animatedBoundsCore.Location;
						}
						else {
							Size = animatedBoundsCore.Size;
							Location = animatedBoundsCore.Location;
						}
					}
					else
						Bounds = animatedBoundsCore;
				}
				else
					Location = animatedBoundsCore.Location;
				UpdateSplitContainerControl(prevWidth);
			}
		}
		protected internal bool IsRightToLeftDirection {
			get {
				if(IsRightToLeftLayout)
					return Dock != DockStyle.Right;
				return Dock == DockStyle.Right;
			}
		}
		protected internal bool IsRightToLeftLayout {
			get { return WindowsFormsSettings.GetIsRightToLeftLayout(FindForm()); }
		}
		void UpdateSplitContainerControl(int prevWidth) {
			SplitContainerControl control = GetParentSplitContainer();
			if(control != null) {
				if(control.Panel1 == Parent)
					control.SplitterPosition += animatedBoundsCore.Width - prevWidth;
				else
					control.SplitterPosition -= animatedBoundsCore.Width - prevWidth;
			}
		}
		public virtual int CalcCollapsedPaneWidth() {
			NavigationPaneViewInfo paneInfo = ViewInfo as NavigationPaneViewInfo;
			if(paneInfo == null) return 20;
			return paneInfo.GetCollapsedWidth();
		}
		protected virtual void OnNavPaneBoundsAnimationStarted() { }
		protected virtual void OnNavPaneBoundsAnimationCompleted() {
			OptionsNavPane.allowAnimation = false;
			RaiseSizeableChanged();
			if(OptionsNavPane.ExpandButtonMode == ExpandButtonMode.Inverted || Dock == DockStyle.Right) Bounds = animatedBoundsCore;
		}
		protected virtual void ProcessExpandChangedWithoutAnimation() {
			RaiseNavPaneStateChanged();
			RaiseSizeableChanged();
			OptionsNavPane.actualNavPaneState = OptionsNavPane.NavPaneState;
			ViewInfo.Clear();
			Invalidate();
			return;
		}
		protected SplitContainerControl GetParentSplitContainer() {
			if(Parent is SplitGroupPanel) {
				SplitContainerControl control = Parent.Parent as SplitContainerControl;
				if(control == null || !control.Horizontal)
					return null;
				return control;
			}
			return null;
		}
		protected virtual void ProcessExpandChanged(bool newValue) {
			if(Dock == DockStyle.Fill && GetParentSplitContainer() == null) {
				ProcessExpandChangedWithoutAnimation();
				return;
			}
			if(ViewInfo as NavigationPaneViewInfo == null) {
				OptionsNavPane.actualNavPaneState = OptionsNavPane.NavPaneState;
				return;
			}
			if(!ViewInfo.IsReady)
				ViewInfo.Calc(ClientRectangle);
			Size startSize, endSize;
			Point startLocation, endLocation;
			startLocation = endLocation = Location;
			if(!newValue) {
				OptionsNavPane.ExpandedWidth = Width;
				endSize = new Size(CalcCollapsedPaneWidth(), Height);
				startSize = new Size(OptionsNavPane.ExpandedWidth, Height);
				if(OptionsNavPane.ExpandButtonMode == ExpandButtonMode.Inverted)
					endLocation = new Point(endLocation.X + startSize.Width - endSize.Width, endLocation.Y);
			}
			else {
				startSize = new Size(Size.Width, Height);
				endSize = new Size(OptionsNavPane.ExpandedWidth, Height);
				if(OptionsNavPane.ExpandButtonMode == ExpandButtonMode.Inverted)
					endLocation = new Point(endLocation.X - (endSize.Width - startSize.Width), endLocation.Y);
			}
			if(Dock == DockStyle.Right && !IsRightToLeftLayout) {
				endLocation = new Point(Parent.ClientRectangle.Right - endSize.Width, Location.Y);
			}
			else if(Dock != DockStyle.Right && IsRightToLeftLayout) {
				endLocation = new Point(Parent.ClientRectangle.X, Location.Y);
			}
			else {
				if(IsRightToLeft) endLocation.X = endLocation.X + startSize.Width - endSize.Width;
			}
			if(!Visible || !IsHandleCreated || Parent == null || !OptionsNavPane.allowAnimation) {
				if(Dock == DockStyle.Fill) {
					CheckSplitterControl(endSize);
				}
				Size = endSize;
				Location = endLocation;
				OptionsNavPane.actualNavPaneState = OptionsNavPane.NavPaneState;
				LayoutChanged();
			}
			else {
				OnNavPaneBoundsAnimationStarted();
				if(Form != null && SuspendFormLayoutInAnimation) Form.SuspendLayout();
				XtraAnimator.Current.AddBoundsAnimation(this, this, this, newValue, new Rectangle(startLocation, startSize), new Rectangle(endLocation, endSize), Math.Max(1, OptionsNavPane.AnimationFramesCount / 6));
			}
			RaiseNavPaneStateChanged();
		}
		protected void UpdateCollapsedNavBarSize() {
			Size = new Size(CalcCollapsedPaneWidth(), Height);
		}
		protected void CheckSplitterControl(Size size) {
			SplitContainerControl splitContainer = GetParentSplitContainer();
			if(splitContainer != null && Dock == DockStyle.Fill) {
				splitContainer.SplitterPosition = size.Width;
			}
		}
		protected void RaiseNavPaneStateChanged() {
			EventHandler handler = this.Events[navPaneStateChanged] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		public virtual void SaveToXml(string xmlFile) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void RestoreFromXml(string xmlFile) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual bool SaveToRegistry(string path) {
			return SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RestoreFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void SaveToStream(System.IO.Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void RestoreFromStream(System.IO.Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream);
		}
		protected virtual bool SaveLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				return serializer.SerializeObject(this, stream, this.GetType().Name);
			else
				return serializer.SerializeObject(this, path.ToString(), this.GetType().Name);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				serializer.DeserializeObject(this, stream, this.GetType().Name);
			else
				serializer.DeserializeObject(this, path.ToString(), this.GetType().Name);
		}
		ArrayList groupsOrder = new ArrayList();
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			RaiseBeforeLoadLayout(e);
			if(!e.Allow) return;
			BeginUpdate();
			try {
				groupsOrder.Clear();
				foreach(NavBarGroup group in Groups) {
					group.Visible = false;
					group.ItemLinks.Clear();
				}
			}
			finally {
				CancelUpdate();
			}
			this.lockInit++;
			this.deserializing++;
		}
		bool IsDeserializing { get { return deserializing != 0; } }
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			try {
				Groups.RestoreGroupOrderCore(groupsOrder);
				if(restoredVersion != LayoutVersion) RaiseLayoutUpgrade(new LayoutUpgradeEventArgs(restoredVersion));
				SetActiveGroup();
			}
			finally {
				this.lockInit--;
				this.deserializing--;
				LayoutChanged();
			}
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		Size IXtraResizableControl.MaxSize {
			get {
				if(animatedBoundsCore != Rectangle.Empty) return animatedBoundsCore.Size;
				if(OptionsNavPane.NavPaneState == NavPaneState.Collapsed) return new Size(CalcCollapsedPaneWidth(), 0);
				else return new Size(0, 0);
			}
		}
		Size IXtraResizableControl.MinSize {
			get {
				if(animatedBoundsCore != Rectangle.Empty) return animatedBoundsCore.Size;
				if(OptionsNavPane.NavPaneState == NavPaneState.Collapsed) return new Size(CalcCollapsedPaneWidth(), 10);
				else return new Size(10, 10);
			}
		}
		bool IXtraResizableControl.IsCaptionVisible { get { return false; } }
		event EventHandler IXtraResizableControl.Changed {
			add { Events.AddHandler(sizeableChanged, value); }
			remove { Events.RemoveHandler(sizeableChanged, value); }
		}
		static readonly object sizeableChanged = new object();
		protected void RaiseSizeableChanged() {
			EventHandler changed = (EventHandler)Events[sizeableChanged];
			if(changed == null) return;
			changed(this, EventArgs.Empty);
		}
		private static readonly object navPaneStateChanged = new object();
		private static readonly object hotTrackedLinkChanged = new object();
		private static readonly object activeGroupChanged = new object();
		private static readonly object linkPressed = new object();
		private static readonly object linkClicked = new object();
		private static readonly object customDrawGroupCaption = new object();
		private static readonly object customDrawLink = new object();
		private static readonly object customDrawGroupClientBackground = new object();
		private static readonly object customDrawGroupClientForeground = new object();
		private static readonly object customDrawBackground = new object();
		private static readonly object navDragOver = new object();
		private static readonly object navDragDrop = new object();
		private static readonly object selectedLinkChanged = new object();
		private static readonly object customDrawHint = new object();
		private static readonly object getHint = new object();
		private static readonly object calcHintSize = new object();
		private static readonly object groupExpanded = new object();
		private static readonly object groupExpanding = new object();
		private static readonly object groupCollapsed = new object();
		private static readonly object groupCollapsing = new object();
		private static readonly object layoutUpgrade = new object();
		private static readonly object beforeLoadLayout = new object();
		private static readonly object navPaneMinimizedGroupFormShowing = new object();
		private static readonly object navPaneOptionsCanEditGroupFont = new object();
		private static readonly object navPaneOptionsApplyGroupFont = new object();
		private static readonly object navPaneOptionsReset = new object();
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlGroupExpanded"),
#endif
 Category("NavBar")]
		public event NavBarGroupEventHandler GroupExpanded {
			add { this.Events.AddHandler(groupExpanded, value); }
			remove { this.Events.RemoveHandler(groupExpanded, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlGroupExpanding"),
#endif
 Category("NavBar")]
		public event NavBarGroupCancelEventHandler GroupExpanding {
			add { this.Events.AddHandler(groupExpanding, value); }
			remove { this.Events.RemoveHandler(groupExpanding, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlLayoutUpgrade"),
#endif
 Category("NavBar")]
		public event LayoutUpgradeEventHandler LayoutUpgrade {
			add { this.Events.AddHandler(layoutUpgrade, value); }
			remove { this.Events.RemoveHandler(layoutUpgrade, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlBeforeLoadLayout"),
#endif
 Category("Data")]
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { this.Events.AddHandler(beforeLoadLayout, value); }
			remove { this.Events.RemoveHandler(beforeLoadLayout, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlGroupCollapsed"),
#endif
 Category("NavBar")]
		public event NavBarGroupEventHandler GroupCollapsed {
			add { this.Events.AddHandler(groupCollapsed, value); }
			remove { this.Events.RemoveHandler(groupCollapsed, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlGroupCollapsing"),
#endif
 Category("NavBar")]
		public event NavBarGroupCancelEventHandler GroupCollapsing {
			add { this.Events.AddHandler(groupCollapsing, value); }
			remove { this.Events.RemoveHandler(groupCollapsing, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlSelectedLinkChanged"),
#endif
 Category("NavBar")]
		public event NavBarSelectedLinkChangedEventHandler SelectedLinkChanged {
			add { Events.AddHandler(selectedLinkChanged, value); }
			remove { Events.RemoveHandler(selectedLinkChanged, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlNavDragOver"),
#endif
 Category("NavBar")]
		public event NavBarDragDropEventHandler NavDragOver {
			add { Events.AddHandler(navDragOver, value); }
			remove { Events.RemoveHandler(navDragOver, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlNavDragDrop"),
#endif
 Category("NavBar")]
		public event NavBarDragDropEventHandler NavDragDrop {
			add { Events.AddHandler(navDragDrop, value); }
			remove { Events.RemoveHandler(navDragDrop, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlCustomDrawGroupCaption"),
#endif
 Category("NavBar")]
		public event CustomDrawNavBarElementEventHandler CustomDrawGroupCaption {
			add { Events.AddHandler(customDrawGroupCaption, value); }
			remove { Events.RemoveHandler(customDrawGroupCaption, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlCustomDrawLink"),
#endif
 Category("NavBar")]
		public event CustomDrawNavBarElementEventHandler CustomDrawLink {
			add { Events.AddHandler(customDrawLink, value); }
			remove { Events.RemoveHandler(customDrawLink, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlCustomDrawGroupClientBackground"),
#endif
 Category("NavBar")]
		public event CustomDrawObjectEventHandler CustomDrawGroupClientBackground {
			add { Events.AddHandler(customDrawGroupClientBackground, value); }
			remove { Events.RemoveHandler(customDrawGroupClientBackground, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlCustomDrawGroupClientForeground"),
#endif
 Category("NavBar")]
		public event CustomDrawObjectEventHandler CustomDrawGroupClientForeground {
			add { Events.AddHandler(customDrawGroupClientForeground, value); }
			remove { Events.RemoveHandler(customDrawGroupClientForeground, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlCustomDrawBackground"),
#endif
 Category("NavBar")]
		public event CustomDrawObjectEventHandler CustomDrawBackground {
			add { Events.AddHandler(customDrawBackground, value); }
			remove { Events.RemoveHandler(customDrawBackground, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlHotTrackedLinkChanged"),
#endif
 Category("NavBar")]
		public event NavBarLinkEventHandler HotTrackedLinkChanged {
			add { Events.AddHandler(hotTrackedLinkChanged, value); }
			remove { Events.RemoveHandler(hotTrackedLinkChanged, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlNavPaneStateChanged"),
#endif
 Category("NavBar")]
		public event EventHandler NavPaneStateChanged {
			add { Events.AddHandler(navPaneStateChanged, value); }
			remove { Events.RemoveHandler(navPaneStateChanged, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlActiveGroupChanged"),
#endif
 Category("NavBar")]
		public event NavBarGroupEventHandler ActiveGroupChanged {
			add { Events.AddHandler(activeGroupChanged, value); }
			remove { Events.RemoveHandler(activeGroupChanged, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlLinkPressed"),
#endif
 Category("NavBar")]
		public event NavBarLinkEventHandler LinkPressed {
			add { Events.AddHandler(linkPressed, value); }
			remove { Events.RemoveHandler(linkPressed, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlLinkClicked"),
#endif
 Category("NavBar")]
		public event NavBarLinkEventHandler LinkClicked {
			add { Events.AddHandler(linkClicked, value); }
			remove { Events.RemoveHandler(linkClicked, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlCustomDrawHint"),
#endif
 Category("NavBar")]
		public event NavBarCustomDrawHintEventHandler CustomDrawHint {
			add { Events.AddHandler(customDrawHint, value); }
			remove { Events.RemoveHandler(customDrawHint, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlCalcHintSize"),
#endif
 Category("NavBar")]
		public event NavBarCalcHintSizeEventHandler CalcHintSize {
			add { Events.AddHandler(calcHintSize, value); }
			remove { Events.RemoveHandler(calcHintSize, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlGetHint"),
#endif
 Category("NavBar")]
		public event NavBarGetHintEventHandler GetHint {
			add { Events.AddHandler(getHint, value); }
			remove { Events.RemoveHandler(getHint, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlNavPaneMinimizedGroupFormShowing"),
#endif
 Category("NavBar")]
		public event EventHandler<NavPaneMinimizedGroupFormShowingEventArgs> NavPaneMinimizedGroupFormShowing {
			add { Events.AddHandler(navPaneMinimizedGroupFormShowing, value); }
			remove { Events.RemoveHandler(navPaneMinimizedGroupFormShowing, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlNavPaneOptionsCanEditGroupFont"),
#endif
 Category("NavBar")]
		public event EventHandler<NavPaneOptionsCanEditGroupFontEventArgs> NavPaneOptionsCanEditGroupFont {
			add { Events.AddHandler(navPaneOptionsCanEditGroupFont, value); }
			remove { Events.RemoveHandler(navPaneOptionsCanEditGroupFont, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlNavPaneOptionsApplyGroupFont"),
#endif
 Category("NavBar")]
		public event EventHandler<NavPaneOptionsApplyGroupFontEventArgs> NavPaneOptionsApplyGroupFont {
			add { Events.AddHandler(navPaneOptionsApplyGroupFont, value); }
			remove { Events.RemoveHandler(navPaneOptionsApplyGroupFont, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarControlNavPaneOptionsReset"),
#endif
 Category("NavBar")]
		public event EventHandler<NavPaneOptionsResetEventArgs> NavPaneOptionsReset {
			add { Events.AddHandler(navPaneOptionsReset, value); }
			remove { Events.RemoveHandler(navPaneOptionsReset, value); }
		}
		protected internal virtual void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[beforeLoadLayout];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseLayoutUpgrade(LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseGroupCollapsed(NavBarGroupEventArgs e) {
			if(!IsAllowFireEvents) return;
			NavBarGroupEventHandler handler = (NavBarGroupEventHandler)this.Events[groupCollapsed];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseGroupCollapsing(NavBarGroupCancelEventArgs e) {
			if(!IsAllowFireEvents) return;
			NavBarGroupCancelEventHandler handler = (NavBarGroupCancelEventHandler)this.Events[groupCollapsing];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseGroupExpanded(NavBarGroupEventArgs e) {
			if(!IsAllowFireEvents) return;
			NavBarGroupEventHandler handler = (NavBarGroupEventHandler)this.Events[groupExpanded];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseGroupExpanding(NavBarGroupCancelEventArgs e) {
			if(!IsAllowFireEvents) return;
			NavBarGroupCancelEventHandler handler = (NavBarGroupCancelEventHandler)this.Events[groupExpanding];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseActiveGroupChanged(NavBarGroup group) {
			RaiseGroupEvent(activeGroupChanged, group);
			RaiseNavigationBarClientSelectedItemChanged();
		}
		protected internal virtual void RaiseHotTrackedLinkChanged(NavBarItemLink link) {
			RaiseLinkEvent(hotTrackedLinkChanged, link);
		}
		protected internal virtual void RaiseLinkPressed(NavBarItemLink link) {
			RaiseLinkEvent(linkPressed, link);
			if(link != null) link.Item.RaiseLinkPressedCore(link);
		}
		protected internal virtual void RaiseLinkClicked(NavBarItemLink link) {
			RaiseLinkEvent(linkClicked, link);
			if(link != null) link.Item.RaiseLinkClickedCore(link);
		}
		protected internal virtual void RaiseSelectedLinkChanged(NavBarSelectedLinkChangedEventArgs e) {
			if(!IsAllowFireEvents) return;
			NavBarSelectedLinkChangedEventHandler handler = (NavBarSelectedLinkChangedEventHandler)this.Events[selectedLinkChanged];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseLinkEvent(object linkEvent, NavBarItemLink link) {
			if(!IsAllowFireEvents) return;
			NavBarLinkEventHandler handler = (NavBarLinkEventHandler)this.Events[linkEvent];
			if(handler != null) handler(this, new NavBarLinkEventArgs(link));
		}
		protected internal virtual void RaiseGroupEvent(object linkEvent, NavBarGroup group) {
			if(!IsAllowFireEvents) return;
			NavBarGroupEventHandler handler = (NavBarGroupEventHandler)this.Events[linkEvent];
			if(handler != null) handler(this, new NavBarGroupEventArgs(group));
		}
		protected internal virtual void RaiseCustomDrawEvent(object cevent, CustomDrawObjectEventArgs e) {
			CustomDrawObjectEventHandler handler = (CustomDrawObjectEventHandler)this.Events[cevent];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDragEvent(object cevent, NavBarDragDropEventArgs e) {
			NavBarDragDropEventHandler handler = (NavBarDragDropEventHandler)this.Events[cevent];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawElementEvent(object cevent, CustomDrawNavBarElementEventArgs e) {
			CustomDrawNavBarElementEventHandler handler = (CustomDrawNavBarElementEventHandler)this.Events[cevent];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseNavDragDrop(NavBarDragDropEventArgs e) {
			RaiseDragEvent(navDragDrop, e);
		}
		protected internal virtual void RaiseNavDragOver(NavBarDragDropEventArgs e) {
			RaiseDragEvent(navDragOver, e);
		}
		protected internal virtual void RaiseCustomDrawGroupClientBackground(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(customDrawGroupClientBackground, e);
		}
		protected internal virtual void RaiseCustomDrawGroupClientForeground(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(customDrawGroupClientForeground, e);
		}
		protected internal virtual void RaiseCustomDrawGroupCaption(CustomDrawNavBarElementEventArgs e) {
			RaiseCustomDrawElementEvent(customDrawGroupCaption, e);
		}
		protected internal virtual void RaiseCustomDrawLink(CustomDrawNavBarElementEventArgs e) {
			RaiseCustomDrawElementEvent(customDrawLink, e);
		}
		protected internal virtual void RaiseCustomDrawBackground(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(customDrawBackground, e);
		}
		protected internal virtual void RaiseCusomDrawHint(NavBarCustomDrawHintEventArgs e) {
			NavBarCustomDrawHintEventHandler handler = (NavBarCustomDrawHintEventHandler)this.Events[customDrawHint];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCalcHintSize(NavBarCalcHintSizeEventArgs e) {
			NavBarCalcHintSizeEventHandler handler = (NavBarCalcHintSizeEventHandler)this.Events[calcHintSize];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseGetHint(NavBarGetHintEventArgs e) {
			NavBarGetHintEventHandler handler = (NavBarGetHintEventHandler)this.Events[getHint];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseNavPaneMinimizedGroupFormShowing(NavPaneMinimizedGroupFormShowingEventArgs e) {
			EventHandler<NavPaneMinimizedGroupFormShowingEventArgs> handler = (EventHandler<NavPaneMinimizedGroupFormShowingEventArgs>)this.Events[navPaneMinimizedGroupFormShowing];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseNavPaneOptionsCanEditGroupFont(NavPaneOptionsCanEditGroupFontEventArgs e) {
			EventHandler<NavPaneOptionsCanEditGroupFontEventArgs> handler = (EventHandler<NavPaneOptionsCanEditGroupFontEventArgs>)this.Events[navPaneOptionsCanEditGroupFont];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseNavPaneOptionsApplyGroupFont(NavPaneOptionsApplyGroupFontEventArgs e) {
			EventHandler<NavPaneOptionsApplyGroupFontEventArgs> handler = (EventHandler<NavPaneOptionsApplyGroupFontEventArgs>)this.Events[navPaneOptionsApplyGroupFont];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseNavPaneOptionsReset(NavPaneOptionsResetEventArgs e) {
			EventHandler<NavPaneOptionsResetEventArgs> handler = (EventHandler<NavPaneOptionsResetEventArgs>)this.Events[navPaneOptionsReset];
			if(handler != null) handler(this, e);
		}
		static IList designTimeViews = null;
		static IList xmlViews = null;
		protected virtual void RegisterXMLNavBarViews() {
			if(xmlViews == null) {
				xmlViews = new ArrayList();
				IList namesList = NavXMLRead.ReadViewsFromXML();
				foreach(string[] s in namesList) {
					if(s.Length < 2) continue;
					BaseViewInfoRegistrator vr = null;
					try {
						System.Reflection.Assembly asm = DevExpress.Data.Utils.Helpers.LoadWithPartialName(s[0]);
						if(asm != null) {
							Type t = asm.GetType(s[1]);
							if(t != null)
								vr = Activator.CreateInstance(t) as BaseViewInfoRegistrator;
						}
					}
					catch { vr = null; }
					if(vr != null) xmlViews.Add(vr);
				}
			}
			RegisterViews(xmlViews);
		}
		protected virtual void OnRegisterType(Type type) {
			object[] attrs = type.GetCustomAttributes(typeof(UserNavBarView), false);
			if(attrs == null || attrs.Length != 1) return;
			UserNavBarView attr = attrs[0] as UserNavBarView;
			if(attr != null) {
				designTimeViews.Add(type);
			}
		}
		protected virtual void PopulateDesignTimeViews() {
			if(designTimeViews == null) {
				IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
				designTimeViews = new ArrayList();
				new DevExpress.Utils.Registrator.RegistratorHelper(new DevExpress.Utils.Registrator.CheckTypeHandler(OnRegisterType), typeof(BaseViewInfoRegistrator), host);
			}
			RegisterDesignTimeViews();
		}
		protected virtual void RegisterDesignTimeViews() {
			if(designTimeViews == null) return;
			RegisterViews(designTimeViews);
		}
		BaseViewInfoRegistrator GetRegistrator(object obj) {
			BaseViewInfoRegistrator vr = obj as BaseViewInfoRegistrator;
			if(vr != null) return vr;
			Type type = obj as Type;
			if(type == null) return null;
			try {
				vr = Activator.CreateInstance(type) as BaseViewInfoRegistrator;
			}
			catch {
			}
			return vr;
		}
		protected virtual void RegisterViews(IList views) {
			foreach(object obj in views) {
				BaseViewInfoRegistrator vr = GetRegistrator(obj);
				if(vr == null) continue;
				bool found = false;
				foreach(BaseViewInfoRegistrator r in AvailableNavBarViews) {
					if(r.ViewName == vr.ViewName) {
						found = true;
						break;
					}
				}
				if(!found) AvailableNavBarViews.Add(vr);
			}
		}
		GestureHelper gestureHelper;
		GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) gestureHelper = new GestureHelper(this);
				return gestureHelper;
			}
		}
		const int WM_CAPTURECHANGED = 0x215;
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if(wndProcHandler != null) wndProcHandler(this, ref m);
			if(m.Msg == WM_CAPTURECHANGED) {
				if(m.LParam == this.Handle)
					OnGotCapture();
				else
					OnLostCapture();
			}
			if(GestureHelper.WndProc(ref m)) return;
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected virtual void OnLostCapture() {
			if(ViewInfo != null) ViewInfo.ClearPressedInfo();
		}
		protected virtual void OnGotCapture() { }
		public class NavXMLRead {
			public static IList ReadViewsFromXML() { return ReadViewsFromXML(""); }
			public static IList ReadViewsFromXML(string path) {
				ArrayList ret = new ArrayList();
				try {
					if(path == "")
						path = System.Environment.SystemDirectory + "\\DevExpress.XtraNavBar.Views.Xml";
					if(System.IO.File.Exists(path)) {
						System.Xml.XmlTextReader xml = new System.Xml.XmlTextReader(path);
						while(xml.Read()) {
							switch(xml.NodeType) {
								case System.Xml.XmlNodeType.Element:
									if(xml.AttributeCount == 2 &&
										(xml.Name == "View"))
										ret.Add(new string[] { xml[0], xml[1] });
									break;
								default:
									break;
							}
						}
						xml.Close();
					}
				}
				catch {}
				return ret;
			}
		}
		#region IDXMenuSupport Members
		event DXMenuWndProcHandler IDXMenuSupport.WndProc {
			add {
				wndProcHandler += value;
			}
			remove {
				if(wndProcHandler != null)
					wndProcHandler -= value;
			}
		}
		DXPopupMenu IDXMenuSupport.Menu { get { return null; } }
		void IDXMenuSupport.ShowMenu(Point pos) { }
		event DXMenuWndProcHandler wndProcHandler;
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { base.BackgroundImageLayout = value; } }
		protected internal virtual bool AllowDrawLinkDropMark { get { return true; } }
		#region IGestureClient Members
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			if(ViewInfo == null || !ViewInfo.AllowScrollBar) return GestureAllowArgs.None;
			return new GestureAllowArgs[] { GestureAllowArgs.PanVertical };
		}
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		IntPtr IGestureClient.Handle { get { return IsHandleCreated ? Handle : IntPtr.Zero; } }
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) { }
		int yOverPan = 0;
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin) {
				yOverPan = 0;
				return;
			}
			if(delta.Y == 0) return;
			int prevTopY = ViewInfo.TopY;
			ViewInfo.TopY -= delta.Y;
			if(prevTopY == ViewInfo.TopY) {
				yOverPan += delta.Y;
			}
			else { yOverPan = 0; }
			overPan.Y = yOverPan;
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		Point IGestureClient.PointToClient(Point p) { return PointToClient(p); }
		#endregion
		protected internal virtual void UpdateNavBarOnLinksChanged() {
			if(NavPaneForm == null || !NavPaneForm.Visible)
				return;
			NavPaneForm.UpdateExpandedGroupInfo();
		}
		#region IOfficeNavigationBarClient Members
		bool compactCore;
		protected internal bool IsCompact {
			get { return compactCore; }
		}
		INavigationBar navigationBarCore;
		protected internal INavigationBar NavigationBar {
			get { return navigationBarCore; }
			private set {
				if(navigationBarCore == value) return;
				if(IsAttachedToOfficeNavigationBar)
					OnDetachedToOfficeNavigationBar();
				navigationBarCore = value;
				if(IsAttachedToOfficeNavigationBar)
					OnAttachedToOfficeNavigationBar();
			}
		}
		protected internal bool IsAttachedToOfficeNavigationBar {
			get { return navigationBarCore != null; }
		}
		bool IOfficeNavigationBarClient.Compact {
			get { return compactCore; }
			set {
				if(value == compactCore) return;
				this.compactCore = value;
				OnIsCompactChanged();
			}
		}
		int attachCounter = 0;
		void Attach(INavigationBar navigationBar) {
			if(0 == attachCounter++)
				NavigationBar = navigationBar;
		}
		void Dettach(INavigationBar navigationBar) {
			if(NavigationBar == navigationBar && (--attachCounter == 0))
				NavigationBar = null;
		}
		protected virtual void OnAttachedToOfficeNavigationBar() {
			ViewInfo.OnAttachToOfficeNavigationBar();
			LayoutChanged();
		}
		protected virtual void OnDetachedToOfficeNavigationBar() {
			ViewInfo.OnDetachToOfficeNavigationBar();
			LayoutChanged();
		}
		protected virtual void OnIsCompactChanged() {
			LayoutChanged();
		}
		#endregion
		#region INavigationBarClient Members
		IEnumerable<INavigationItem> INavigationBarClient.ItemsSource {
			get { return groups; }
		}
		object itemsSourceChanged = new object();
		event EventHandler INavigationBarClient.ItemsSourceChanged {
			add {
				Events.AddHandler(itemsSourceChanged, value);
				if(value != null) Attach(value.Target as INavigationBar);
			}
			remove {
				if(value != null) Dettach(value.Target as INavigationBar);
				Events.RemoveHandler(itemsSourceChanged, value);
			}
		}
		protected internal void RaiseNavigationBarClientItemsSourceChanged(CollectionChangeEventArgs e) {
			EventHandler handler = (EventHandler)Events[itemsSourceChanged];
			if(handler != null)
				handler(this, e);
		}
		INavigationItem INavigationBarClient.SelectedItem {
			get { return ActiveGroup; }
			set { ActiveGroup = value as NavBarGroup; }
		}
		object selectedItemChanged = new object();
		event EventHandler INavigationBarClient.SelectedItemChanged {
			add {
				Events.AddHandler(selectedItemChanged, value);
				if(value != null) Attach(value.Target as INavigationBar);
			}
			remove {
				if(value != null) Dettach(value.Target as INavigationBar);
				Events.RemoveHandler(selectedItemChanged, value);
			}
		}
		void RaiseNavigationBarClientSelectedItemChanged() {
			EventHandler handler = (EventHandler)Events[selectedItemChanged];
			if(handler != null)
				handler(this, new NavBarGroupEventArgs(ActiveGroup));
		}
		#endregion
		#region ISearchControlClient Members
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase searchInfo) {
			SearchCriteriaInfo info = searchInfo as SearchCriteriaInfo;
			ApplyFindFilterCore(info.CriteriaOperator);
		}
		void ApplyFindFilterCore(DevExpress.Data.Filtering.CriteriaOperator criteriaOperator) {
			foreach(NavBarGroup group in Groups)
				group.RebuildVisibleLinksCore(criteriaOperator);
			this.LayoutChanged();
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() {
			return new NavBarCriteriaProvider();
		}
		bool ISearchControlClient.IsAttachedToSearchControl { get { return CheckAttachedToSearchControl(); } }
		bool CheckAttachedToSearchControl() { return searchControl != null; }
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) {
			if(this.searchControl == searchControl) return;
			this.searchControl = searchControl;
			ApplyFindFilterCore(null);
		}
		ISearchControl searchControl;
		#endregion
	}
	public class OptionsNavPane : BaseOptions, IDisposable {
		NavBarControl navBar;
		bool showExpandButton;
		bool showOverflowPanel;
		bool showOverflowButton;
		bool showSplitter;
		bool showGroupImageInHeader;
		bool showHeaderText;
		ExpandButtonMode expandButtonMode;
		int expandedWidth, collapsedWidth;
		int animationFramesCount;
		NavPaneState navPaneState;
		internal NavPaneState actualNavPaneState;
		Size popupFormSize;
		int maxPopupFormWidth;
		bool allowOptionsMenuItem;
		GroupImageShowMode groupImageShowMode;
		Control collapsedNavPaneContentControl;
		public OptionsNavPane(OptionsNavPane source) {
			showExpandButton = source.ShowExpandButton;
			expandButtonMode = source.ExpandButtonMode;
			expandedWidth = source.ExpandedWidth;
			collapsedWidth = source.CollapsedWidth;
			navPaneState = source.NavPaneState;
			animationFramesCount = source.AnimationFramesCount;
			actualNavPaneState = source.actualNavPaneState;
			maxPopupFormWidth = source.MaxPopupFormWidth;
			popupFormSize = source.popupFormSize;
			showOverflowPanel = source.showOverflowPanel;
			showOverflowButton = source.showOverflowButton;
			showSplitter = source.showSplitter;
			showGroupImageInHeader = source.ShowGroupImageInHeader;
			allowOptionsMenuItem = source.AllowOptionsMenuItem;
			groupImageShowMode = source.GroupImageShowMode;
			collapsedNavPaneContentControl = source.CollapsedNavPaneContentControl;
			showHeaderText = source.ShowHeaderText;
		}
		public OptionsNavPane(NavBarControl navBar) {
			this.navBar = navBar;
			this.showOverflowButton = true;
			this.showExpandButton = true;
			this.showOverflowPanel = true;
			this.showSplitter = true;
			this.showHeaderText = true;
			this.expandButtonMode = ExpandButtonMode.Normal;
			this.expandedWidth = this.collapsedWidth = -1;
			this.navPaneState = NavPaneState.Expanded;
			this.animationFramesCount = 100;
			this.showGroupImageInHeader = false;
			this.allowOptionsMenuItem = false;
			this.groupImageShowMode = GroupImageShowMode.Always;
			this.collapsedNavPaneContentControl = null;
		}
		[Browsable(false)]
		public Size DefaultNavPaneHeaderImageSize {
			get { return new Size(16, 16); }
		}
		internal bool allowAnimation = false;
		internal event BaseOptionChangedEventHandler Changed {
			add { base.ChangedCore += value; }
			remove { base.ChangedCore -= value; }
		}
		protected virtual bool ShouldSerializePopupFormSize() {
			return PopupFormSize != Size.Empty;
		}
		[Browsable(false)]
		public NavBarControl NavBar { get { return navBar; } }
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("OptionsNavPanePopupFormSize")]
#endif
		public Size PopupFormSize {
			get { return popupFormSize; }
			set { popupFormSize = value; }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneMaxPopupFormWidth"),
#endif
 DefaultValue(0)]
		public int MaxPopupFormWidth {
			get { return maxPopupFormWidth; }
			set { maxPopupFormWidth = value; }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneAnimationFramesCount"),
#endif
 DefaultValue(100)]
		public virtual int AnimationFramesCount {
			get { return animationFramesCount; }
			set {
				if(AnimationFramesCount == value) return;
				if(value < 1) value = 1;
				int prevValue = AnimationFramesCount;
				animationFramesCount = value;
				OnChanged(new BaseOptionChangedEventArgs("AnimationFramesCount", prevValue, AnimationFramesCount));
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneShowExpandButton"),
#endif
 DefaultValue(true)]
		public virtual bool ShowExpandButton {
			get { return showExpandButton; }
			set {
				if(ShowExpandButton == value) return;
				bool prevValue = ShowExpandButton;
				showExpandButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowExpandButton", prevValue, ShowExpandButton));
				NavBar.LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneShowOverflowPanel"),
#endif
 DefaultValue(true)]
		public virtual bool ShowOverflowPanel {
			get { return showOverflowPanel; }
			set {
				if(ShowOverflowPanel == value) return;
				showOverflowPanel = value;
				NavBar.LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneShowOverflowButton"),
#endif
 DefaultValue(true)]
		public virtual bool ShowOverflowButton {
			get { return showOverflowButton; }
			set {
				if(ShowOverflowButton == value) return;
				showOverflowButton = value;
				NavBar.LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneShowSplitter"),
#endif
 DefaultValue(true)]
		public virtual bool ShowSplitter {
			get { return showSplitter; }
			set {
				if(ShowSplitter == value) return;
				showSplitter = value;
				NavBar.LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneShowHeaderText"),
#endif
 DefaultValue(true)]
		public virtual bool ShowHeaderText {
			get { return showHeaderText; }
			set {
				if(ShowHeaderText == value)
					return;
				showHeaderText = value;
				NavBar.LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneExpandButtonMode"),
#endif
 DefaultValue(ExpandButtonMode.Normal)]
		public virtual ExpandButtonMode ExpandButtonMode {
			get { return expandButtonMode; }
			set {
				if(ExpandButtonMode == value) return;
				ExpandButtonMode prevValue = ExpandButtonMode;
				expandButtonMode = value;
				OnChanged(new BaseOptionChangedEventArgs("ExpandButtonMode", prevValue, ExpandButtonMode));
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneExpandedWidth"),
#endif
 DefaultValue(-1), Localizable(true)]
		public virtual int ExpandedWidth {
			get { return expandedWidth; }
			set {
				if(ExpandedWidth == value) return;
				int prevValue = ExpandedWidth;
				expandedWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("ExpandedWidth", prevValue, ExpandedWidth));
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneCollapsedWidth"),
#endif
 DefaultValue(-1), Localizable(true)]
		public virtual int CollapsedWidth {
			get { return collapsedWidth; }
			set {
				if(CollapsedWidth == value) return;
				int prevValue = CollapsedWidth;
				collapsedWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("CollapsedWidth", prevValue, CollapsedWidth));
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneNavPaneState"),
#endif
 DefaultValue(NavPaneState.Expanded)]
		public virtual NavPaneState NavPaneState {
			get { return navPaneState; }
			set {
				if(NavPaneState == value) return;
				NavPaneState prevValue = NavPaneState;
				navPaneState = value;
				OnChanged(new BaseOptionChangedEventArgs("NavPaneState", prevValue, NavPaneState));
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneShowGroupImageInHeader"),
#endif
 DefaultValue(false)]
		public bool ShowGroupImageInHeader {
			get { return showGroupImageInHeader; }
			set {
				if(ShowGroupImageInHeader == value) return;
				bool prevValue = ShowGroupImageInHeader;
				showGroupImageInHeader = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupImageInHeader", prevValue, ShowGroupImageInHeader));
				NavBar.LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneAllowOptionsMenuItem"),
#endif
 DefaultValue(false)]
		public virtual bool AllowOptionsMenuItem {
			get { return allowOptionsMenuItem; }
			set {
				if(AllowOptionsMenuItem == value) return;
				bool prev = AllowOptionsMenuItem;
				allowOptionsMenuItem = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowOptionsMenuItem", prev, AllowOptionsMenuItem));
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual NavPaneState ActualNavPaneState {
			get { return actualNavPaneState; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsAnimationInProgress {
			get { return NavPaneState != ActualNavPaneState; }
		}
		[DefaultValue(GroupImageShowMode.Always)]
		public GroupImageShowMode GroupImageShowMode {
			get { return groupImageShowMode; }
			set {
				if(GroupImageShowMode == value) return;
				GroupImageShowMode prev = GroupImageShowMode;
				groupImageShowMode = value;
				OnChanged(new BaseOptionChangedEventArgs("GroupImageShowMode", prev, GroupImageShowMode));
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("OptionsNavPaneCollapsedNavPaneContentControl"),
#endif
 DefaultValue(null), TypeConverter("DevExpress.XtraNavBar.Design.CollapsedNavPaneContentControlTypeConverter, " + AssemblyInfo.SRAssemblyNavBarDesign)]
		public Control CollapsedNavPaneContentControl {
			get { return collapsedNavPaneContentControl; }
			set {
				if(CollapsedNavPaneContentControl == value) return;
				Control prev = CollapsedNavPaneContentControl;
				if(prev != null)
					if(NavBar != null) NavBar.OnDestroyContentControl(prev);
				collapsedNavPaneContentControl = value;
				if(value != null)
					if(NavBar != null) NavBar.OnInitContentControl(value);
				OnChanged(new BaseOptionChangedEventArgs("CollapsedNavPaneContentControl", prev, CollapsedNavPaneContentControl));
			}
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(CollapsedNavPaneContentControl != null && NavBar != null && NavBar.ContainsControl(CollapsedNavPaneContentControl))
					CollapsedNavPaneContentControl.Dispose();
			}
			this.collapsedNavPaneContentControl = null;
		}
	}
	public enum GroupImageShowMode { Always, InCollapsedState }
	public enum LinkSelectionModeType { None, OneInControl, OneInGroup, OneInGroupAndAllowAutoSelect }
	[ListBindable(false)]
	public class NavBarViewCollection : CollectionBase, IEnumerable<BaseViewInfoRegistrator> {
		public void Add(BaseViewInfoRegistrator viewInfo) {
			List.Add(viewInfo);
		}
		public BaseViewInfoRegistrator this[int index] {
			get { return List[index] as BaseViewInfoRegistrator; }
		}
		public BaseViewInfoRegistrator this[string name] {
			get {
				foreach(BaseViewInfoRegistrator item in List) {
					if(item.ViewName == name) return item;
				}
				return null;
			}
		}
		public void Remove(BaseViewInfoRegistrator viewInfo) {
			List.Remove(viewInfo);
		}
		protected internal virtual void UpdateThemeColors() {
			foreach(BaseViewInfoRegistrator reg in this) {
				reg.UpdateThemeColors();
			}
		}
		public Array ToArray(Type type) { return InnerList.ToArray(type); }
		public object[] ToArray() { return InnerList.ToArray(); }
		#region IEnumerator<BaseViewInfoRegistrator>
		IEnumerator<BaseViewInfoRegistrator> IEnumerable<BaseViewInfoRegistrator>.GetEnumerator() {
			foreach(BaseViewInfoRegistrator reg in InnerList)
				yield return reg;
		}
		#endregion
	}
}
