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
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.ViewInfo;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraTab.Buttons;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraTab {
	[Flags]
	public enum TabButtons {
		None = 0,
		Prev = 1,
		Next = 2,
		Close = 4,
		Default = 0x800
	};
	public class TabPageEventArgs : EventArgs {
		XtraTabPage pageCore;
		int pageIndexCore;
		TabControlAction actionCore;
		public TabPageEventArgs(XtraTabPage page, int pageIndex, TabControlAction action) {
			this.pageCore = page;
			this.pageIndexCore = pageIndex;
			this.actionCore = action;
		}
		public XtraTabPage Page { get { return pageCore; } }
		public int PageIndex { get { return pageIndexCore; } }
		public TabControlAction Action { get { return actionCore; } }
	}
	public class TabPageCancelEventArgs : TabPageEventArgs {
		bool cancelCore = false;
		public TabPageCancelEventArgs(XtraTabPage page, int pageIndex, bool cancel, TabControlAction action)
			: base(page, pageIndex, action) {
			this.cancelCore = cancel;
		}
		public bool Cancel {
			get { return cancelCore; }
			set { cancelCore = value; }
		}
	}
	public class TabPageChangedEventArgs : EventArgs {
		XtraTabPage prevPage, page;
		public TabPageChangedEventArgs(XtraTabPage prevPage, XtraTabPage page) {
			this.prevPage = prevPage;
			this.page = page;
		}
		public XtraTabPage PrevPage { get { return prevPage; } }
		public XtraTabPage Page { get { return page; } }
	}
	public class TabPageChangingEventArgs : TabPageChangedEventArgs {
		bool cancel = false;
		public TabPageChangingEventArgs(XtraTabPage prevPage, XtraTabPage page) : base(prevPage, page) { }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public delegate void TabPageChangedEventHandler(object sender, TabPageChangedEventArgs e);
	public delegate void TabPageChangingEventHandler(object sender, TabPageChangingEventArgs e);
	public delegate void TabPageCancelEventHandler(object sender, TabPageCancelEventArgs e);
	public delegate void TabPageEventHandler(object sender, TabPageEventArgs e);
	public enum TabMiddleClickFiringMode {
		Default,
		MouseDown,
		MouseUp,
		None
	}
	public enum ClosePageButtonShowMode {
		Default,
		InTabControlHeader,
		InAllTabPageHeaders,
		InActiveTabPageHeader,
		InAllTabPagesAndTabControlHeader,
		InActiveTabPageAndTabControlHeader,
		InActiveTabPageHeaderAndOnMouseHover,
	}
	public enum PinPageButtonShowMode {
		Default,
		InAllTabPageHeaders,
		InActiveTabPageHeader,
		InActiveTabPageHeaderAndOnMouseHover,
	}
	public interface IXtraTabProperties {
		DefaultBoolean AllowHotTrack { get; }
		DefaultBoolean ShowTabHeader { get; }
		DefaultBoolean ShowToolTips { get; }
		DefaultBoolean MultiLine { get; }
		DefaultBoolean HeaderAutoFill { get; }
		DefaultBoolean ShowHeaderFocus { get; }
		TabPageImagePosition PageImagePosition { get; }
		AppearanceObject Appearance { get; }
		PageAppearance AppearancePage { get; }
		BorderStyles BorderStyle { get; }
		BorderStyles BorderStylePage { get; }
		TabButtonShowMode HeaderButtonsShowMode { get; }
		TabButtons HeaderButtons { get; }
		int TabPageWidth { get; }
		ClosePageButtonShowMode ClosePageButtonShowMode { get; }
		PinPageButtonShowMode PinPageButtonShowMode { get; }
		DefaultBoolean AllowGlyphSkinning { get; }
	}
	public interface IXtraTabPropertiesEx {
		CustomHeaderButtonCollection CustomHeaderButtons { get; }
		TabMiddleClickFiringMode TabMiddleClickFiringMode { get; }
	}
	public class DefaultTabProperties : IXtraTabProperties, IXtraTabPropertiesEx {
		AppearanceObject appearance;
		PageAppearance appearancePage;
		public DefaultTabProperties() {
			this.appearance = new AppearanceObject();
			this.appearancePage = new PageAppearance();
		}
		static DefaultTabProperties def;
		public static DefaultTabProperties Default {
			get {
				if(def == null) def = new DefaultTabProperties();
				return def;
			}
		}
		public int TabPageWidth { get { return 0; } }
		public DefaultBoolean AllowHotTrack { get { return DefaultBoolean.Default; } }
		public DefaultBoolean AllowGlyphSkinning { get { return DefaultBoolean.Default; } }
		public DefaultBoolean ShowTabHeader { get { return DefaultBoolean.Default; } }
		public DefaultBoolean ShowToolTips { get { return DefaultBoolean.Default; } }
		public DefaultBoolean MultiLine { get { return DefaultBoolean.Default; } }
		public DefaultBoolean HeaderAutoFill { get { return DefaultBoolean.Default; } }
		public DefaultBoolean ShowHeaderFocus { get { return DefaultBoolean.Default; } }
		public TabPageImagePosition PageImagePosition { get { return TabPageImagePosition.Near; } }
		public AppearanceObject Appearance { get { return appearance; } }
		public PageAppearance AppearancePage { get { return appearancePage; } }
		public BorderStyles BorderStyle { get { return BorderStyles.Default; } }
		public BorderStyles BorderStylePage { get { return BorderStyles.Default; } }
		public TabButtonShowMode HeaderButtonsShowMode { get { return TabButtonShowMode.Default; } }
		public TabButtons HeaderButtons { get { return TabButtons.Default; } }
		public ClosePageButtonShowMode ClosePageButtonShowMode { get { return ClosePageButtonShowMode.Default; } }
		public PinPageButtonShowMode PinPageButtonShowMode { get { return PinPageButtonShowMode.Default; } }
		public CustomHeaderButtonCollection CustomHeaderButtons { get { return CustomHeaderButtonCollection.Empty; } }
		public TabMiddleClickFiringMode TabMiddleClickFiringMode { get { return TabMiddleClickFiringMode.Default; } }
	}
	public interface IXtraTab {
		int PageCount { get; }
		IXtraTabPage GetTabPage(int index);
		BaseViewInfoRegistrator View { get; }
		Rectangle Bounds { get; }
		TabHeaderLocation HeaderLocation { get; }
		TabOrientation HeaderOrientation { get; }
		object Images { get; }
		BaseTabHitInfo CreateHitInfo();
		BaseTabControlViewInfo ViewInfo { get; }
		Control OwnerControl { get; }
		UserLookAndFeel LookAndFeel { get; }
		void OnPageChanged(IXtraTabPage page);
		void LayoutChanged();
		void Invalidate(Rectangle rect);
		Point ScreenPointToControl(Point point);
		bool RightToLeftLayout { get; }
	}
	[DesignerSerializer("", "")]
	public class NoSerializationControlCollection : System.Windows.Forms.Control.ControlCollection {
		public NoSerializationControlCollection(Control owner) : base(owner) { }
		public override void SetChildIndex(Control child, int newIndex) {
			XtraTabControl xTabControl = Owner as XtraTabControl;
			XtraTabPage xTabPage = child as XtraTabPage;
			if(xTabControl != null && xTabPage != null && !xTabControl.TabPages.Contains(xTabPage)) {
				xTabControl.TabPages.Add(xTabPage);
			}
			if(!this.Contains(child)) return;
			base.SetChildIndex(child, newIndex);
		}
		public override int GetChildIndex(Control child, bool throwException) {
			if(!this.Contains(child)) return 0;
			return base.GetChildIndex(child, throwException);
		}
	}
	[Designer("DevExpress.XtraTab.Design.XtraTabControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign), DXToolboxItem(DXToolboxItemKind.Free),
	 Description("Displays tab pages that can contain controls."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	 ToolboxBitmap(typeof(ToolboxIconsRootNS), "XtraTabControl"),
	 SmartTagSupport(typeof(XtraTabControlBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	 SmartTagFilter(typeof(XtraTabControlFilter)),
	 SmartTagAction(typeof(XtraTabControlActions), "TabPages", "Tab Pages", SmartTagActionType.CloseAfterExecute),
	 SmartTagAction(typeof(XtraTabControlActions), "AddTabPage", "Add Tab Page"),
	 SmartTagAction(typeof(XtraTabControlActions), "RemoveTabPage", "Remove Tab Page"),
	 SmartTagAction(typeof(XtraTabControlActions), "CustomHeaderButtons", "Custom Header Buttons", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(XtraTabControlActions), "DockInParentContainer", "Dock in parent container", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(XtraTabControlActions), "UndockFromParentContainer", "Undock from parent container", SmartTagActionType.CloseAfterExecute)
	]
	public class XtraTabControl : ControlBase, ISupportInitialize, IXtraTab, IXtraTabProperties, IXtraTabPropertiesEx, IToolTipControlClient, ISupportLookAndFeel,
		DevExpress.Utils.MVVM.Services.IDocumentAdapterFactory {
		CustomHeaderButtonCollection customHeaderButtonsCore;
		bool mouseDownFired;
		TabHeaderLocation headerLocation;
		TabOrientation headerOrientation;
		TabPageImagePosition pageImagePosition;
		XtraTabPageCollection tabPages;
		ClosePageButtonShowMode closePageButtonShowModeCore;
		TabMiddleClickFiringMode tabMiddleClickFiringModeCore;
		TabButtonShowMode headerButtonsShowMode;
		TabButtons headerButtons;
		BaseViewInfoRegistrator view;
		BaseTabControlViewInfo viewInfo;
		BaseTabPainter painter;
		BaseTabHandler handler;
		DefaultBoolean showHeaderFocus, headerAutoFill, showTabHeader, showToolTips, multiLine, allowGlyphSkinning, rightToLeftLayout;
		int loading, tabPageWidth;
		int maxTabPageWidthCore;
		object images;
		UserLookAndFeel lookAndFeel;
		string paintStyleName;
		PageAppearance appearancePage;
		AppearanceObject appearance;
		BorderStyles borderStyle = BorderStyles.Default, borderStylePage = BorderStyles.Default;
		ToolTipController toolTipController;
		bool useCompatibleDrawingMode = false;
		#region events
		readonly static object selectedPageChanged = new object();
		readonly static object selectedPageChanging = new object();
		readonly static object hotTrackedPageChanged = new object();
		readonly static object closeButtonClick = new object();
		readonly static object tabMiddleClick = new object();
		readonly static object customHeaderButtonClick = new object();
		readonly static object headerButtonClick = new object();
		readonly static object selecting = new object();
		readonly static object selected = new object();
		readonly static object deselecting = new object();
		readonly static object deselected = new object();
		readonly static object pageClosing = new object();
		readonly static object pageRemoved = new object();
		[DXCategory(CategoryName.Behavior)]
		public event TabPageChangedEventHandler SelectedPageChanged {
			add { Events.AddHandler(selectedPageChanged, value); }
			remove { Events.RemoveHandler(selectedPageChanged, value); }
		}
		[DXCategory(CategoryName.Behavior)]
		public event TabPageChangingEventHandler SelectedPageChanging {
			add { Events.AddHandler(selectedPageChanging, value); }
			remove { Events.RemoveHandler(selectedPageChanging, value); }
		}
		[DXCategory(CategoryName.Behavior)]
		public event TabPageChangedEventHandler HotTrackedPageChanged {
			add { Events.AddHandler(hotTrackedPageChanged, value); }
			remove { Events.RemoveHandler(hotTrackedPageChanged, value); }
		}
		[DXCategory(CategoryName.Behavior)]
		public event EventHandler CloseButtonClick {
			add { Events.AddHandler(closeButtonClick, value); }
			remove { Events.RemoveHandler(closeButtonClick, value); }
		}
		[DXCategory(CategoryName.Behavior)]
		public event PageEventHandler TabMiddleClick {
			add { Events.AddHandler(tabMiddleClick, value); }
			remove { Events.RemoveHandler(tabMiddleClick, value); }
		}
		[DXCategory(CategoryName.Behavior)]
		public event CustomHeaderButtonEventHandler CustomHeaderButtonClick {
			add { Events.AddHandler(customHeaderButtonClick, value); }
			remove { Events.RemoveHandler(customHeaderButtonClick, value); }
		}
		[DXCategory(CategoryName.Behavior)]
		public event HeaderButtonEventHandler HeaderButtonClick {
			add { Events.AddHandler(headerButtonClick, value); }
			remove { Events.RemoveHandler(headerButtonClick, value); }
		}
		[DXCategory(CategoryName.Action)]
		public event TabPageCancelEventHandler Selecting {
			add { Events.AddHandler(selecting, value); }
			remove { Events.RemoveHandler(selecting, value); }
		}
		[DXCategory(CategoryName.Action)]
		public event TabPageEventHandler Selected {
			add { Events.AddHandler(selected, value); }
			remove { Events.RemoveHandler(selected, value); }
		}
		[DXCategory(CategoryName.Action)]
		public event TabPageCancelEventHandler Deselecting {
			add { Events.AddHandler(deselecting, value); }
			remove { Events.RemoveHandler(deselecting, value); }
		}
		[DXCategory(CategoryName.Action)]
		public event TabPageEventHandler Deselected {
			add { Events.AddHandler(deselected, value); }
			remove { Events.RemoveHandler(deselected, value); }
		}
		protected internal event TabPageCancelEventHandler PageClosing {
			add { Events.AddHandler(pageClosing, value); }
			remove { Events.RemoveHandler(pageClosing, value); }
		}
		protected internal event TabPageEventHandler PageRemoved {
			add { Events.AddHandler(pageRemoved, value); }
			remove { Events.RemoveHandler(pageRemoved, value); }
		}
		#endregion events
		public XtraTabControl() {
			ToolTipController.DefaultController.AddClientControl(this);
			SetStyle(ControlStyles.SupportsTransparentBackColor | ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint
				| ControlStyles.UserMouse, true);
			SetStyle(ControlStyles.Selectable, AllowTabFocus);
			this.rightToLeftLayout = DefaultBoolean.Default;
			this.headerButtonsShowMode = TabButtonShowMode.Default;
			this.headerButtons = TabButtons.Default;
			this.appearance = new AppearanceObject("Appearance");
			this.appearance.Changed += OnAppearanceChanged;
			this.appearancePage = new PageAppearance();
			this.appearancePage.Changed += OnAppearanceChanged;
			this.paintStyleName = BaseViewInfoRegistrator.DefaultViewName;
			this.showTabHeader = DefaultBoolean.Default;
			this.showHeaderFocus = DefaultBoolean.Default;
			this.showToolTips = DefaultBoolean.Default;
			this.headerAutoFill = this.multiLine = DefaultBoolean.Default;
			this.allowGlyphSkinning = DefaultBoolean.Default;
			this.headerLocation = TabHeaderLocation.Top;
			this.headerOrientation = TabOrientation.Default;
			this.pageImagePosition = TabPageImagePosition.Near;
			this.tabPageWidth = 0;
			this.maxTabPageWidthCore = 0;
			this.lookAndFeel = new ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += OnLookAndFeel_StyleChanged;
			this.images = null;
			this.painter = null;
			this.loading = 0;
			this.tabPages = CreateTabCollection();
			this.tabPages.CollectionChanged += OnTagPagesCollectionChanged;
			this.closePageButtonShowModeCore = ClosePageButtonShowMode.Default;
			this.viewInfo = null;
			this.customHeaderButtonsCore = new CustomHeaderButtonCollection();
			CustomHeaderButtons.CollectionChanged += CustomHeaderButtonsCollectionChanged;
			CheckInfo();
			CheckFont();
			UseMnemonic = true;
		}
		void CustomHeaderButtonsCollectionChanged(object sender, CollectionChangeEventArgs e) {
			LayoutChanged();
			if(!DesignMode || Site == null) return;
			DevExpress.Utils.Design.EditorContextHelper.FireChanged(Site, this);
		}
		protected virtual XtraTabPageCollection CreateTabCollection() { return new XtraTabPageCollection(this); }
		protected override void Dispose(bool disposing) {
			if(Appearance != null)
				appearance.Changed -= OnAppearanceChanged;
			if(AppearancePage != null)
				appearancePage.Changed -= OnAppearanceChanged;
			if(CustomHeaderButtons != null)
				CustomHeaderButtons.CollectionChanged -= CustomHeaderButtonsCollectionChanged;
			if(disposing) {
				this.lookAndFeel.StyleChanged -= OnLookAndFeel_StyleChanged;
				this.LookAndFeel.Dispose();
				UnsubscribeViewInfo(viewInfo);
				if(ViewInfo != null)
					ViewInfo.Dispose();
				this.viewInfo = null;
				DestroyAppearances();
				ToolTipController = null;
				ToolTipController.DefaultController.RemoveClientControl(this);
			}
			base.Dispose(disposing);
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		void DestroyAppearances() {
			DestroyPageAppearance(this.appearancePage);
			DestroyAppearance(this.appearance);
		}
		void DestroyPageAppearance(PageAppearance appearance) {
			if(this.appearancePage != null) {
				this.appearancePage.Changed -= new EventHandler(OnAppearanceChanged);
				this.appearancePage.Dispose();
			}
		}
		void DestroyAppearance(AppearanceObject appearance) {
			if(this.appearance != null) {
				this.appearance.Changed -= new EventHandler(OnAppearanceChanged);
				this.appearance.Dispose();
			}
		}
		AppearanceDefault defaultAppearance = null;
		protected virtual AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault();
		}
		Font GetFont() {
			AppearanceObject app = Appearance.GetAppearanceByFont();
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
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			Handler.ProcessEvent(EventType.KeyDown, e);
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			Invalidate();
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			Invalidate();
		}
		protected virtual void OnLookAndFeel_StyleChanged(object sender, EventArgs e) {
			CheckInfo();
			LayoutChanged();
			OnBackColorChanged(EventArgs.Empty);
		}
		protected virtual void CheckInfo() {
			this.view = CreateViewInstance();
			if(ViewInfo != null) ViewInfo.ResetDefaultAppearances();
			CreateView();
		}
		protected virtual BaseViewInfoRegistrator CreateViewInstance() {
			return PaintStyleCollection.DefaultPaintStyles.GetView(LookAndFeel, PaintStyleName);
		}
		public void MakePageVisible(XtraTabPage page) {
			if(ViewInfo != null) ViewInfo.MakePageVisible(page);
		}
		protected virtual void CreateView() {
			IXtraTabPage prevSelected = null;
			int prevLockUpdate = 0;
			if(this.viewInfo != null) {
				prevLockUpdate = this.viewInfo.lockUpdate;
				UnsubscribeViewInfo(viewInfo);
				this.viewInfo.Dispose();
				prevSelected = this.ViewInfo.SelectedTabPage;
			}
			this.viewInfo = View.CreateViewInfo(this);
			this.painter = View.CreatePainter(this);
			this.handler = View.CreateHandler(this);
			ViewInfo.lockUpdate = prevLockUpdate;
			if(prevSelected != null)
				viewInfo.SetSelectedTabPageCore(prevSelected);
			SubscribeViewInfo(ViewInfo);
		}
		void SubscribeViewInfo(BaseTabControlViewInfo viewInfo) {
			if(viewInfo == null) return;
			viewInfo.CloseButtonClick += OnCloseButtonClick;
			viewInfo.TabMiddleClick += OnTabMiddleClick;
			viewInfo.HeaderButtonClick += OnHeaderButtonClick;
			viewInfo.CustomHeaderButtonClick += OnCustomHeaderButtonClick;
			viewInfo.HotTrackedPageChanged += OnHotTrackedPageChanged;
			viewInfo.PageClientBoundsChanged += OnPageClientBoundsChanged;
			viewInfo.SelectedPageChanged += OnSelectedPageChanged;
			viewInfo.SelectedPageChanging += OnSelectedPageChanging;
		}
		void UnsubscribeViewInfo(BaseTabControlViewInfo viewInfo) {
			if(viewInfo == null) return;
			viewInfo.SelectedPageChanged -= OnSelectedPageChanged;
			viewInfo.SelectedPageChanging -= OnSelectedPageChanging;
			viewInfo.HotTrackedPageChanged -= OnHotTrackedPageChanged;
			viewInfo.PageClientBoundsChanged -= OnPageClientBoundsChanged;
			viewInfo.CloseButtonClick -= OnCloseButtonClick;
			viewInfo.TabMiddleClick -= OnTabMiddleClick;
			viewInfo.HeaderButtonClick -= OnHeaderButtonClick;
			viewInfo.CustomHeaderButtonClick -= OnCustomHeaderButtonClick;
		}
		DevExpress.Accessibility.BaseAccessible dxAccessible;
		protected internal virtual DevExpress.Accessibility.BaseAccessible DXAccessible {
			get {
				if(dxAccessible == null) dxAccessible = CreateAccessibleInstance();
				return dxAccessible;
			}
		}
		protected virtual DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.Accessibility.Tab.TabControlAccessible(this);
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(DXAccessible == null) return base.CreateAccessibilityInstance();
			return DXAccessible.Accessible;
		}
		protected virtual void OnTabMiddleClick(object sender, PageEventArgs e) {
			TabPageCancelEventArgs ea = new TabPageCancelEventArgs(e.Page as XtraTabPage, -1, false, TabControlAction.Deselecting);
			if(RaisePageClosing(ea)) {
				PageEventHandler handler = Events[tabMiddleClick] as PageEventHandler;
				if(handler != null) handler(this, e);
			}
		}
		protected virtual void OnCloseButtonClick(object sender, EventArgs e) {
			ClosePageButtonEventArgs closeArgs = e as ClosePageButtonEventArgs;
			TabPageCancelEventArgs ea = new TabPageCancelEventArgs(
				closeArgs != null ? closeArgs.Page as XtraTabPage : SelectedTabPage, -1, false, TabControlAction.Deselecting);
			if(RaisePageClosing(ea)) {
				EventHandler handler = Events[closeButtonClick] as EventHandler;
				if(handler != null) handler(this, e);
			}
		}
		protected virtual void OnCustomHeaderButtonClick(object sender, CustomHeaderButtonEventArgs e) {
			CustomHeaderButtonEventHandler handler = Events[customHeaderButtonClick] as CustomHeaderButtonEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void OnHeaderButtonClick(object sender, HeaderButtonEventArgs e) {
			HeaderButtonEventHandler handler = Events[headerButtonClick] as HeaderButtonEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void OnPageClientBoundsChanged(object sender, EventArgs e) {
			UpdatePageSelection();
		}
		protected void UpdatePageSelection() {
			if(!IsHandleCreated) return;
			if(SelectedTabPage != null && ViewInfo != null) {
				SelectedTabPage.Bounds = ViewInfo.PageClientBounds;
				SelectedTabPage.Visible = true;
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			ControlContainerLookAndFeelHelper.UpdateChildrenLookAndFeel(this);
			UpdatePageSize();
			UpdatePageSelection();
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			ControlContainerLookAndFeelHelper.UpdateChildrenLookAndFeel(this);
		}
		protected virtual void UpdatePageSize() {
			for(int n = 0; n < TabPages.Count; n++) {
				TabPages[n].Bounds = DisplayRectangle;
			}
		}
		protected virtual void OnHotTrackedPageChanged(object sender, ViewInfoTabPageChangedEventArgs e) {
			TabPageChangedEventHandler handler = Events[hotTrackedPageChanged] as TabPageChangedEventHandler;
			if(handler != null) handler(this, ConvertArgs(e));
		}
		protected virtual void OnSelectedPageChanging(object sender, ViewInfoTabPageChangingEventArgs e) {
			if(e.PrevPage != null) {
				XtraTabPage deselectedPage = e.PrevPage as XtraTabPage;
				e.Cancel = !deselectedPage.DoValidate();
				if(e.Cancel) return;
				int pageIndex = this.TabPages.IndexOf(deselectedPage);
				TabPageCancelEventArgs cancelArgs = new TabPageCancelEventArgs(deselectedPage, pageIndex, false, TabControlAction.Deselecting);
				OnDeselecting(cancelArgs);
				if(!cancelArgs.Cancel) OnDeselected(new TabPageEventArgs(deselectedPage, pageIndex, TabControlAction.Deselected));
				e.Cancel = cancelArgs.Cancel;
			}
			if(e.Cancel) return;
			TabPageChangingEventArgs changingArgs = new TabPageChangingEventArgs(e.PrevPage as XtraTabPage, e.Page as XtraTabPage);
			RaiseSelectedPageChanging(changingArgs);
			e.Cancel = changingArgs.Cancel;
			if(e.Cancel) return;
			if(e.Page != null) {
				XtraTabPage selectedPage = e.Page as XtraTabPage;
				int pageIndex = this.TabPages.IndexOf(selectedPage);
				TabPageCancelEventArgs cancelArgs = new TabPageCancelEventArgs(selectedPage, pageIndex, false, TabControlAction.Selecting);
				OnSelecting(cancelArgs);
				if(!cancelArgs.Cancel)
					OnSelected(new TabPageEventArgs(selectedPage, pageIndex, TabControlAction.Deselected));
				e.Cancel = cancelArgs.Cancel;
			}
		}
		protected virtual void RaiseSelectedPageChanging(TabPageChangingEventArgs e) {
			TabPageChangingEventHandler handler = Events[selectedPageChanging] as TabPageChangingEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseSelectedPageChanged(TabPageChangedEventArgs e) {
			TabPageChangedEventHandler handler = Events[selectedPageChanged] as TabPageChangedEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void OnSelecting(TabPageCancelEventArgs e) {
			TabPageCancelEventHandler handler = Events[selecting] as TabPageCancelEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void OnSelected(TabPageEventArgs e) {
			TabPageEventHandler handler = Events[selected] as TabPageEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void OnDeselecting(TabPageCancelEventArgs e) {
			TabPageCancelEventHandler handler = Events[deselecting] as TabPageCancelEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void OnDeselected(TabPageEventArgs e) {
			TabPageEventHandler handler = Events[deselected] as TabPageEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual bool RaisePageClosing(TabPageCancelEventArgs e) {
			TabPageCancelEventHandler handler = Events[pageClosing] as TabPageCancelEventHandler;
			if(handler != null) handler(this, e);
			return !e.Cancel;
		}
		protected virtual void RaisePageRemoved(TabPageEventArgs e) {
			TabPageEventHandler handler = Events[pageRemoved] as TabPageEventHandler;
			if(handler != null) handler(this, e);
		}
		bool isSendingFocus = false;
		protected virtual void OnSelectedPageChanged(object sender, ViewInfoTabPageChangedEventArgs e) {
			TabPageChangedEventArgs res = ConvertArgs(e);
			if(res.PrevPage != null) res.PrevPage.Visible = false;
			if(res.Page != null) {
				if(IsHandleCreated) {
					res.Page.Bounds = ViewInfo.PageClientBounds;
					res.Page.Visible = true;
					XtraTabPage page = e.Page as XtraTabPage;
					page.AccessibleNotifyClients(AccessibleEvents.Selection, -1);
					page.AccessibleNotifyClients(AccessibleEvents.Focus, -1);
					try {
						if(ContainsFocus &&
							!Focused &&
							!res.Page.ContainsFocus &&
							!isSendingFocus
						   ) {
							isSendingFocus = true;
							res.Page.SelectNextControl(null, true, true, false, false);
						}
					}
					finally {
						isSendingFocus = false;
					}
				}
			}
			if(IsLoading) return;
			RaiseSelectedPageChanged(res);
		}
		protected virtual TabPageChangedEventArgs ConvertArgs(ViewInfoTabPageChangedEventArgs e) {
			return new TabPageChangedEventArgs(e.PrevPage as XtraTabPage, e.Page as XtraTabPage);
		}
		DefaultBoolean IXtraTabProperties.AllowHotTrack { get { return DefaultBoolean.True; } }
		PinPageButtonShowMode IXtraTabProperties.PinPageButtonShowMode { get { return PinPageButtonShowMode.Default; } }
		BaseTabControlViewInfo IXtraTab.ViewInfo { get { return ViewInfo; } }
		BaseTabHitInfo IXtraTab.CreateHitInfo() { return new XtraTabHitInfo(); }
		int IXtraTab.PageCount { get { return TabPages.Count; } }
		IXtraTabPage IXtraTab.GetTabPage(int index) {
			return TabPages[index];
		}
		Control IXtraTab.OwnerControl { get { return this; } }
		public virtual XtraTabHitInfo CalcHitInfo(Point point) {
			return ViewInfo.CalcHitInfo(point) as XtraTabHitInfo;
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
			if(IsLoading) return;
			foreach(XtraTabPage page in TabPages) page.SetCompatibleMode(UseCompatibleDrawingMode);
			if(IsHandleCreated) Refresh();
		}
		public virtual Size CalcSizeByPageClient(Size clientSize) {
			BaseTabControlViewInfo vi = View.CreateViewInfo(this);
			Rectangle constBounds = new Rectangle(0, 0, 300, 300);
			vi.CalcViewInfo(constBounds, null);
			Size delta = new Size(constBounds.Width - vi.PageClientBounds.Width, constBounds.Height - vi.PageClientBounds.Height);
			vi.Dispose();
			return new Size(delta.Width + clientSize.Width, delta.Height + clientSize.Height);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Rectangle PageClientBounds { get { return ViewInfo.PageClientBounds; } }
		Rectangle IXtraTab.Bounds { get { return ClientRectangle; } }
		protected virtual void OnTagPagesCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Add) {
				XtraTabPage page = e.Element as XtraTabPage;
				if(page != SelectedTabPage) page.Visible = false;
				Controls.Add(page);
			}
			if(IsLoading) return;
			if(ViewInfo != null) ViewInfo.BeginUpdate();
			try {
				switch(e.Action) {
					case CollectionChangeAction.Add:
						OnTabPageAdded(e.Element as XtraTabPage);
						break;
					case CollectionChangeAction.Remove:
						OnTabPageRemoved(e.Element as XtraTabPage);
						break;
				}
			}
			finally {
				if(ViewInfo != null) ViewInfo.EndUpdate();
			}
			if(ViewInfo != null) ViewInfo.CheckFirstPageIndex();
		}
		void NotifyPagesChanged() {
			if(!DesignMode || Site == null) return;
			DevExpress.Utils.Design.EditorContextHelper.FireChanged(Site, this);
		}
		protected virtual void OnTabPageAdded(XtraTabPage page) {
			ViewInfo.OnPageAdded(page);
			NotifyPagesChanged();
		}
		protected virtual void OnTabPageRemoved(XtraTabPage page) {
			if(ViewInfo != null) ViewInfo.OnPageRemoved(page);
			Controls.Remove(page);
			LayoutChanged();
			NotifyPagesChanged();
			RaisePageRemoved(new TabPageEventArgs(page, -1, TabControlAction.Deselected));
		}
		public virtual void BeginUpdate() {
			if(ViewInfo != null) ViewInfo.BeginUpdate();
		}
		public virtual void EndUpdate() {
			if(ViewInfo != null) ViewInfo.EndUpdate();
		}
		protected bool IsLockUpdate { get { return ViewInfo == null || ViewInfo.IsLockUpdate; } }
		public virtual void BeginInit() {
			loading++;
		}
		public virtual void EndInit() {
			if(--loading == 0) {
				CheckInfo();
				LayoutChanged();
				UpdatePageSelection();
				UpdatePageImageHelpers();
				OnPageClientBoundsChanged(this, EventArgs.Empty);
				if(UseCompatibleDrawingMode) OnCompatibleDrawingModeChanged();
			}
		}
		[Browsable(false)]
		public virtual bool IsLoading { get { return loading != 0; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlBackColor"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return Appearance.BackColor; }
			set { Appearance.BackColor = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlForeColor"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return Appearance.ForeColor; }
			set { Appearance.ForeColor = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlTabPageWidth"),
#endif
 DXCategory(CategoryName.Appearance)]
		[Localizable(true), DefaultValue(0)]
		public virtual int TabPageWidth {
			get { return tabPageWidth; }
			set {
				if(value < 1) value = 0;
				if(TabPageWidth == value) return;
				tabPageWidth = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlMaxTabPageWidth"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(0), SmartTagProperty("Max Tab Page Width", "")]
		public virtual int MaxTabPageWidth {
			get { return maxTabPageWidthCore; }
			set {
				if(MaxTabPageWidth == value) return;
				maxTabPageWidthCore = value;
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("XtraTabControlCustomHeaderButtons")]
#endif
		[Localizable(true), DXCategory(CategoryName.Behavior), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CustomHeaderButtonCollection CustomHeaderButtons {
			get { return customHeaderButtonsCore; }
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("XtraTabControlTabMiddleClickFiringMode")]
#endif
		[Localizable(true), DXCategory(CategoryName.Behavior), DefaultValue(TabMiddleClickFiringMode.Default)]
		public virtual TabMiddleClickFiringMode TabMiddleClickFiringMode {
			get { return tabMiddleClickFiringModeCore; }
			set {
				if(TabMiddleClickFiringMode == value) return;
				tabMiddleClickFiringModeCore = value;
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("XtraTabControlClosePageButtonShowMode")]
#endif
		[Localizable(true), DXCategory(CategoryName.Behavior), DefaultValue(ClosePageButtonShowMode.Default), SmartTagProperty("Close Page Button Show Mode", "")]
		public virtual ClosePageButtonShowMode ClosePageButtonShowMode {
			get { return closePageButtonShowModeCore; }
			set {
				if(ClosePageButtonShowMode == value) return;
				closePageButtonShowModeCore = value;
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("XtraTabControlAllowGlyphSkinning")]
#endif
		[Localizable(true), DXCategory(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value) return;
				allowGlyphSkinning = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlHeaderButtonsShowMode"),
#endif
 Localizable(true), DXCategory(CategoryName.Behavior), DefaultValue(TabButtonShowMode.Default)]
		public virtual TabButtonShowMode HeaderButtonsShowMode {
			get { return headerButtonsShowMode; }
			set {
				if(HeaderButtonsShowMode == value) return;
				headerButtonsShowMode = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlShowHeaderFocus"),
#endif
 Localizable(true), DXCategory(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean ShowHeaderFocus {
			get { return showHeaderFocus; }
			set {
				if(ShowHeaderFocus == value) return;
				showHeaderFocus = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlRightToLeftLayout"),
#endif
 Localizable(true), DXCategory(CategoryName.Behavior), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean RightToLeftLayout {
			get { return rightToLeftLayout; }
			set {
				if(RightToLeftLayout == value) return;
				rightToLeftLayout = value;
				LayoutChanged();
			}
		}
		protected override void OnRightToLeftChanged() {
			base.OnRightToLeftChanged();
			LayoutChanged();
		}
		bool IXtraTab.RightToLeftLayout { get { return IsRightToLeftLayout; } }
		protected internal bool IsRightToLeftLayout {
			get {
				if(RightToLeftLayout == DefaultBoolean.False) return false;
				if(RightToLeftLayout == DefaultBoolean.True) return true;
				return WindowsFormsSettings.GetIsRightToLeft(this);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlToolTipController"),
#endif
 DXCategory(CategoryName.ToolTip), DefaultValue(null)]
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
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlHeaderButtons"),
#endif
 Localizable(true), DXCategory(CategoryName.Behavior), DefaultValue(TabButtons.Default),
		Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual TabButtons HeaderButtons {
			get { return headerButtons; }
			set {
				if(HeaderButtons == value) return;
				headerButtons = value;
				LayoutChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public new NoSerializationControlCollection Controls {
			get { return base.Controls as NoSerializationControlCollection; }
		}
		protected override Control.ControlCollection CreateControlsInstance() {
			return new NoSerializationControlCollection(this);
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlAppearance"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Appearance {
			get { return appearance; }
		}
		void ResetAppearancePage() { AppearancePage.Reset(); }
		bool ShouldSerializeAppearancePage() { return AppearancePage.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlAppearancePage"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual PageAppearance AppearancePage {
			get { return appearancePage; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlShowToolTips"),
#endif
 Localizable(true), DXCategory(CategoryName.ToolTip), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean ShowToolTips {
			get { return showToolTips; }
			set { showToolTips = value; }
		}
		bool IToolTipControlClient.ShowToolTips { get { return ViewInfo == null ? false : ViewInfo.ShowToolTips; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlShowTabHeader"),
#endif
 Localizable(true), DXCategory(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default), SmartTagProperty("Show Tab Header", "")]
		public virtual DefaultBoolean ShowTabHeader {
			get { return showTabHeader; }
			set {
				if(ShowTabHeader == value) return;
				showTabHeader = value;
				LayoutChanged();
			}
		}
		[ Localizable(true), DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public virtual bool UseMnemonic { get; set; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlPaintStyleName"),
#endif
 Localizable(true), DXCategory(CategoryName.Appearance), DefaultValue(BaseViewInfoRegistrator.DefaultViewName),
		TypeConverter("DevExpress.XtraTab.Design.TabPaintStyleNameConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
		public virtual string PaintStyleName {
			get { return paintStyleName; }
			set {
				if(PaintStyleName == value) return;
				paintStyleName = value;
				if(IsLoading) return;
				CheckInfo();
				OnBackColorChanged(EventArgs.Empty);
				LayoutChanged();
			}
		}
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlLookAndFeel"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlHeaderLocation"),
#endif
 Localizable(true), DXCategory(CategoryName.Appearance), DefaultValue(TabHeaderLocation.Top), SmartTagProperty("Header Location", "")]
		public virtual TabHeaderLocation HeaderLocation {
			get { return headerLocation; }
			set {
				if(HeaderLocation == value) return;
				headerLocation = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlHeaderOrientation"),
#endif
 Localizable(true), DXCategory(CategoryName.Appearance), DefaultValue(TabOrientation.Default), SmartTagProperty("Header Orientation", "")]
		public virtual TabOrientation HeaderOrientation {
			get { return headerOrientation; }
			set {
				if(HeaderOrientation == value) return;
				headerOrientation = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlPageImagePosition"),
#endif
 Localizable(true), DXCategory(CategoryName.Appearance), DefaultValue(TabPageImagePosition.Near), SmartTagProperty("Page Image Position", "")]
		public virtual TabPageImagePosition PageImagePosition {
			get { return pageImagePosition; }
			set {
				if(PageImagePosition == value) return;
				pageImagePosition = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlMultiLine"),
#endif
 Localizable(true), DXCategory(CategoryName.Behavior), DefaultValue(DefaultBoolean.Default), SmartTagProperty("Multi Line", "")]
		public virtual DefaultBoolean MultiLine {
			get { return multiLine; }
			set {
				if(!IsLoading) {
					if(!ViewInfo.IsAllowMultiLine && value == DefaultBoolean.True) value = DefaultBoolean.Default;
				}
				if(MultiLine == value) return;
				multiLine = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlBorderStyle"),
#endif
 Localizable(true), DXCategory(CategoryName.Appearance), DefaultValue(BorderStyles.Default)]
		public virtual BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlBorderStylePage"),
#endif
 Localizable(true), DXCategory(CategoryName.Appearance), DefaultValue(BorderStyles.Default)]
		public virtual BorderStyles BorderStylePage {
			get { return borderStylePage; }
			set {
				if(BorderStylePage == value) return;
				borderStylePage = value;
				LayoutChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int FirstVisiblePageIndex {
			get { return ViewInfo.FirstVisiblePageIndex; }
			set { ViewInfo.FirstVisiblePageIndex = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlHeaderAutoFill"),
#endif
 Localizable(true), DXCategory(CategoryName.Behavior), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean HeaderAutoFill {
			get { return headerAutoFill; }
			set {
				if(HeaderAutoFill == value) return;
				headerAutoFill = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlImages"),
#endif
 Localizable(true), DXCategory(CategoryName.Appearance), DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object Images {
			get { return images; }
			set {
				if(Images == value) return;
				images = value;
				OnImagesChanged();
			}
		}
		protected virtual void UpdatePageImageHelpers() {
			foreach(XtraTabPage page in TabPages) {
				page.UpdateImageHelper();
			}
		}
		protected virtual void OnImagesChanged() {
			UpdatePageImageHelpers();
			LayoutChanged();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlSelectedTabPage"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(null), TypeConverter("DevExpress.XtraTab.Design.TabSelectedPageConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
		public virtual XtraTabPage SelectedTabPage {
			get { return ViewInfo == null ? null : ViewInfo.SelectedTabPage as XtraTabPage; }
			set { ViewInfo.SelectedTabPage = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual XtraTabPage HotTrackedTabPage {
			get { return ViewInfo.HotTrackedTabPage as XtraTabPage; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int SelectedTabPageIndex {
			get { return ViewInfo == null ? -1 : ViewInfo.SelectedTabPageIndex; }
			set { ViewInfo.SelectedTabPageIndex = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("XtraTabControlTabPages"),
#endif
 DXCategory(CategoryName.Behavior), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual XtraTabPageCollection TabPages { get { return tabPages; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[Browsable(false)]
		public override Rectangle DisplayRectangle {
			get {
				return ViewInfo == null ? base.DisplayRectangle : ViewInfo.PageClientBounds;
			}
		}
		bool useDisabledStatePainterCore = true;
		[DefaultValue(true),  DXCategory(CategoryName.Appearance)]
		public bool UseDisabledStatePainter {
			get { return useDisabledStatePainterCore; }
			set {
				if(useDisabledStatePainterCore != value) {
					useDisabledStatePainterCore = value;
					Invalidate();
				}
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			DevExpress.Utils.Mdi.ControlState.CheckPaintError(this);
			TabDrawArgs dr = new TabDrawArgs(new GraphicsCache(e), ViewInfo, ClientRectangle);
			Painter.Draw(dr);
			if(!Enabled && UseDisabledStatePainter) {
				DrawDisabled(dr);
			}
			dr.Cache.Dispose();
		}
		protected virtual void DrawDisabled(TabDrawArgs dr) {
			UserLookAndFeel lf = View is SkinViewInfoRegistrator ? LookAndFeel : null;
			foreach(BaseTabPageViewInfo pv in ViewInfo.HeaderInfo.AllPages) {
				BackgroundPaintHelper.PaintDisabledControl(lf, dr.Cache, pv.Bounds);
			}
			BackgroundPaintHelper.PaintDisabledControl(lf, dr.Cache, ViewInfo.PageBounds);
		}
		protected override Size DefaultSize { get { return new Size(300, 300); } }
		protected override void OnPaintBackground(PaintEventArgs e) {
			base.OnPaintBackground(e);
		}
		public virtual void LayoutChanged() {
			if(IsLoading || IsLockUpdate || ViewInfo == null) return;
			ViewInfo.LayoutChanged();
		}
		void IXtraTab.OnPageChanged(IXtraTabPage page) {
			if(ViewInfo != null) ViewInfo.OnPageChanged(page);
			LayoutChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual BaseViewInfoRegistrator View { get { return view; } }
		protected internal virtual BaseTabControlViewInfo ViewInfo { get { return viewInfo; } }
		protected internal virtual BaseTabPainter Painter { get { return painter; } }
		protected virtual BaseTabHandler Handler { get { return handler; } }
		protected override void OnLostCapture() {
			Handler.ProcessEvent(EventType.LostCapture, null);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			Handler.ProcessEvent(EventType.MouseEnter, DXMouseEventArgs.GetMouseArgs(this, e));
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Handler.ProcessEvent(EventType.MouseLeave, e);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			Handler.ProcessEvent(EventType.Resize, ClientRectangle);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			this.mouseDownFired = true;
			base.OnMouseDown(e);
			if(!GetValidationCanceled())
				Handler.ProcessEvent(EventType.MouseDown, e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(!GetValidationCanceled())
				Handler.ProcessEvent(EventType.MouseUp, e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Handler.ProcessEvent(EventType.MouseMove, e);
		}
		protected internal virtual void FireChanged() {
			if(!DesignMode) return;
			if(IsLoading) return;
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				srv.OnComponentChanged(this, null, null, null);
			}
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == MSG.WM_LBUTTONDOWN && IsHandleCreated) {
				Point position = PointToClient(MousePosition);
				this.mouseDownFired = false;
				base.WndProc(ref m);
				if(!mouseDownFired) {
					OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, position.X, position.Y, 0));
				}
				return;
			}
			base.WndProc(ref m);
			XtraEditors.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return ViewInfo == null ? null : ViewInfo.GetToolTipInfo(point);
		}
		protected override bool IsInputKey(Keys keyData) {
			switch(keyData) {
				case Keys.Left:
				case Keys.Right:
				case Keys.Home:
				case Keys.End:
					return true;
			}
			return base.IsInputKey(keyData);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			Keys keyCode = keyData & Keys.KeyCode;
			if(keyCode == Keys.Tab && (keyData & Keys.Control) != 0) {
				if(ContainsFocus && !Focused) {
					if(AllowTabFocus) Focus();
					if(Focused) {
						KeyEventArgs e = new KeyEventArgs(keyData);
						OnKeyDown(e);
						return true;
					}
				}
			}
			return base.ProcessDialogKey(keyData);
		}
		protected override bool ProcessMnemonic(char charCode) {
			if(!UseMnemonic) return false;
			IXtraTabPage page = ViewInfo.FindMnemonic(charCode);
			if(page != null && CanSelect) {
				if(AllowTabFocus) Focus();
				ViewInfo.SelectedTabPage = page;
				return true;
			}
			return false;
		}
		protected internal virtual bool AllowTabFocus { get { return true; } }
		Point IXtraTab.ScreenPointToControl(Point point) {
			return ((IXtraTab)this).OwnerControl.PointToClient(point);
		}
		#region MVVM
		Utils.MVVM.Services.IDocumentAdapter Utils.MVVM.Services.IDocumentAdapterFactory.Create() {
			return new XtraEditors.MVVM.Services.TabPageAdapter(this);
		}
		#endregion MVVM
	}
}
namespace DevExpress.XtraEditors.MVVM.Services {
	using DevExpress.XtraTab;
	using DevExpress.Utils.MVVM.Services;
	class TabPageAdapter : IDocumentAdapter {
		XtraTabPage tabPage;
		XtraTabControl tabControl;
		public TabPageAdapter(XtraTabControl tabControl) {
			this.tabControl = tabControl;
			tabControl.PageRemoved += tabControl_PageRemoved;
			tabControl.PageClosing += tabControl_PageClosing;
		}
		public void Dispose() {
			var control = GetControl(tabPage);
			if(control != null)
				control.TextChanged -= control_TextChanged;
			tabControl.PageClosing -= tabControl_PageClosing;
			tabControl.PageRemoved -= tabControl_PageRemoved;
			tabPage = null;
		}
		void tabControl_PageClosing(object sender, TabPageCancelEventArgs e) {
			if(e.Page == tabPage)
				RaiseClosing(e);
		}
		void tabControl_PageRemoved(object sender, TabPageEventArgs e) {
			if(e.Page == tabPage) {
				RaiseClosed(e);
				Dispose();
			}
		}
		void control_TextChanged(object sender, EventArgs e) {
			tabPage.Text = ((Control)sender).Text;
		}
		void RaiseClosed(TabPageEventArgs e) {
			if(Closed != null) Closed(tabControl, e);
		}
		void RaiseClosing(TabPageCancelEventArgs e) {
			var ea = new CancelEventArgs(e.Cancel);
			if(Closing != null) Closing(tabControl, ea);
			e.Cancel = ea.Cancel;
		}
		public void Show(Control control) {
			var page = System.Linq.Enumerable.FirstOrDefault(tabControl.TabPages, p => GetControl(p) == control);
			if(page == null) {
				tabPage = new XtraTabPage();
				tabPage.Text = control.Text;
				control.TextChanged += control_TextChanged;
			}
			if(tabPage != null) {
				control.Dock = DockStyle.Fill;
				tabPage.Controls.Add(control);
				if(!tabControl.TabPages.Contains(tabPage))
					tabControl.TabPages.Add(tabPage);
			}
			tabControl.SelectedTabPage = tabPage;
		}
		public void Close(Control control, bool force = true) {
			if(force)
				tabControl.PageClosing -= tabControl_PageClosing;
			if(control != null)
				control.TextChanged -= control_TextChanged;
			tabPage.Dispose();
		}
		public event EventHandler Closed;
		public event CancelEventHandler Closing;
		static Control GetControl(XtraTabPage page) {
			return page != null && page.Controls.Count > 0 ? page.Controls[0] : null;
		}
	}
}
