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

using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraBars {
	public enum ShowTabsInTitleBar { True, False }
	[Designer("DevExpress.XtraBars.Design.TabFormControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.ComponentModel.Design.IDesigner)),
	ToolboxTabName(AssemblyInfo.DXTabNavigation), ToolboxItem(false)]
	public class TabFormControl : TabFormControlBase {
		TabForm tabForm;
		[Browsable(false)]
		public TabForm TabForm { get { return tabForm; } set { tabForm = value; } }
		bool ShouldProcessFormMouseEvent(System.Windows.Forms.MouseEventArgs e) {
			return ViewInfo.CalcHitInfo(e.Location).HitTest == TabFormControlHitTest.Caption && TabForm != null;
		}
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			if(ShouldProcessFormMouseEvent(e)) {
				MouseEventArgs args = new MouseEventArgs(e.Button, e.Clicks, e.X - TabForm.GetPainter().Margins.Left, e.Y, e.Delta);
				TabForm.GetPainter().OnMouseDown(args);
			}
			base.OnMouseDown(e);
		}
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			if(ShouldProcessFormMouseEvent(e)) {
				MouseEventArgs args = new MouseEventArgs(e.Button, e.Clicks, e.X - TabForm.GetPainter().Margins.Left, e.Y, e.Delta);
				TabForm.GetPainter().OnMouseMove(args);
			}
			base.OnMouseMove(e);
		}
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
			if(ShouldProcessFormMouseEvent(e)) {
				MouseEventArgs args = new MouseEventArgs(e.Button, e.Clicks, e.X - TabForm.GetPainter().Margins.Left, e.Y, e.Delta);
				TabForm.GetPainter().OnMouseUp(args);
			}
			base.OnMouseUp(e);
		}
		protected override void WndProc(ref Message m) {
			if(ProcessMessage(ref m)) return;
			base.WndProc(ref m);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = value; }
		}
		protected internal virtual bool ProcessMessage(ref Message msg) {
			if(Bounds.IsEmpty) return false;
			if(GetFormPainter() != null && msg.Msg == MSG.WM_NCHITTEST)
				return WMNCHitTest(ref msg);
			return false;
		}
		protected bool WMNCHitTest(ref Message msg) {
			Point screen = GetFormPainter().PointToFormBounds(msg.LParam);
			Point p = PointToClient(WinAPIHelper.GetPoint(msg.LParam));
			if(!IsDesignMode && ViewInfo.CalcHitInfo(p).HitTest == TabFormControlHitTest.Caption) {
				msg.Result = new IntPtr(NativeMethods.HT.HTTRANSPARENT);
				return true;
			}
			return false;
		}
		internal TabFormPainter GetFormPainter() {
			if(TabForm == null)
				return null;
			return TabForm.GetPainter();
		}
		[ DefaultValue(null)]
		public new BarManager Manager {
			get { return base.Manager; }
			set { base.Manager = value; }
		}
		protected override void OnSelectedPageChanged(TabFormPage prev, TabFormPage next) {
			base.OnSelectedPageChanged(prev, next);
			if(TabForm == null)
				return;
			TabForm.SuspendLayout();
			try {
				if(SelectedContainer != null) {
					TabForm.Controls.Remove(SelectedContainer);
				}
				if(next != null) {
					XtraScrollableControl nextContainer = next.ContentContainer;
					if(nextContainer != null && nextContainer.IsDisposed)
						SelectedContainer = null;
					else SelectedContainer = nextContainer;
					if(SelectedContainer != null) {
						TabForm.Controls.Add(SelectedContainer);
						SelectedContainer.Enabled = next.GetEnabled();
						SelectedContainer.LookAndFeel.ParentLookAndFeel = LookAndFeel.ActiveLookAndFeel;
					}
				}
				SendToBack();
			}
			finally { TabForm.ResumeLayout(); }
		}
		protected override void DefaultManagerEndInit() {
			Manager.Form = TabForm;
			if(TabForm != null && TabForm.Container != null)
				TabForm.Container.Add(Manager);
			Manager.EndInit();
		}
	}
	[ToolboxItem(false)]
	public class TabFormControlBase : CustomLinksControl, ISupportInitialize {
		private static readonly object pageCreated = new object();
		private static readonly object selectedPageChanged = new object();
		private static readonly object selectedPageChanging = new object();
		private static readonly object pageCollectionChanged = new object();
		private static readonly object outerFormCreated = new object();
		private static readonly object outerFormCreating = new object();
		RibbonControl ribbon;
		TabFormLinkProvider linkProvider;
		TabFormControlHandler handler;
		TabFormControlDesignTimeManager designManager;
		TabFormPageCollection pages;
		TabFormPage selectedPage;
		bool isInAnimation, showAddPageButton, allowMoveTabsToOuterForm, allowTabAnimation, allowMoveTabs, showTabCloseButtons;
		int leftTabIndent, rightTabIndent, maxTabWidth, titleTabVerticalOffset;
		DefaultBoolean allowGlyphSkinning;
		ShowTabsInTitleBar showTabsInTitleBar;
		object images;
		public TabFormControlBase() : base(null, null) {
			this.designManager = CreateDesignManager();
			this.handler = CreateHandler();
			this.linkProvider = new TabFormLinkProvider(this);
			this.leftTabIndent = this.rightTabIndent = this.titleTabVerticalOffset = 0;
			this.maxTabWidth = 200;
			this.isInAnimation = false;
			this.showAddPageButton = this.allowTabAnimation = this.allowMoveTabsToOuterForm = this.allowMoveTabs = this.showTabCloseButtons = true;
			this.allowGlyphSkinning = DefaultBoolean.Default;
			this.showTabsInTitleBar = ShowTabsInTitleBar.False;
			this.appearance = new TabFormControlAppearances(this);
			Dock = DockStyle.Top;
		}
		[Browsable(false)]
		public new TabFormControlViewInfoBase ViewInfo { get { return (TabFormControlViewInfoBase)base.ViewInfo; } }
		[Browsable(false)]
		public TabFormControlHandler Handler { get { return handler; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TabFormControlDesignTimeManager DesignManager { get { return designManager; } }
		[Browsable(false), DefaultValue(null)]
		public XtraScrollableControl SelectedContainer { get; set; }
		protected virtual TabFormControlDesignTimeManager CreateDesignManager() {
			return new TabFormControlDesignTimeManager(this);
		}
		protected virtual TabFormControlHandler CreateHandler() {
			return new TabFormControlHandler(this);
		}
		internal UserLookAndFeel LookAndFeel {
			get {
				if(Manager == null || Manager.GetController() == null)
					return null;
				return Manager.GetController().LookAndFeel;
			}
		}
		[ Category(CategoryName.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabFormPageCollection Pages {
			get {
				if(pages == null) {
					pages = new TabFormPageCollection(this);
					pages.ListChanged += OnPageCollectionChanged;
				}
				return pages;
			}
		}
		void OnPageCollectionChanged(object sender, ListChangedEventArgs e) {
			ForceRefreshLayout();
			CheckHeight();
			RaisePageCollectionChanged(EventArgs.Empty);
		}
		public void AddNewPage() {
			TabFormPage page = new TabFormPage() { Text = "Page " + Pages.Count.ToString() };
			Pages.Add(page);
			if(DesignMode)
				DesignManager.AddPage(page);
			RaisePageCreated(new PageCreatedEventArgs(page));
			SelectedPage = page;
		}
		protected virtual void OnSelectedPageChanged(TabFormPage prev, TabFormPage next) {
			ForceRefreshLayout();
			TabFormSelectedPageChangedEventArgs args = new TabFormSelectedPageChangedEventArgs(next, prev);
			RaiseSelectedPageChanged(args);
		}
		protected virtual bool OnSelectedPageChanging(TabFormPage prev, TabFormPage next) {
			TabFormSelectedPageChangingEventArgs args = new TabFormSelectedPageChangingEventArgs(next, prev);
			RaiseSelectedPageChanging(args);
			return args.Cancel;
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			ForceRefreshLayout();
		}
		void ForceRefreshLayout() {
			if(ViewInfo == null) return;
			ViewInfo.ClearPageInfos();
			LayoutChanged();
		}
		protected bool CanSelectPage(TabFormPage page) {
			if(page == null) return true;
			return page.CanSelect();
		}
		[ Category(CategoryName.Appearance), DefaultValue(null)]
		public TabFormPage SelectedPage {
			get { return selectedPage; }
			set {
				if(selectedPage == value || !CanSelectPage(value))
					return;
				TabFormPage prev = selectedPage;
				if(OnSelectedPageChanging(prev, value))
					return;
				selectedPage = value;
				OnSelectedPageChanged(prev, selectedPage);
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(0)]
		public int LeftTabIndent {
			get { return leftTabIndent; }
			set {
				if(LeftTabIndent == value)
					return;
				leftTabIndent = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(0)]
		public int RightTabIndent {
			get { return rightTabIndent; }
			set {
				if(RightTabIndent == value)
					return;
				rightTabIndent = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(true)]
		public bool ShowAddPageButton {
			get { return showAddPageButton; }
			set {
				if(ShowAddPageButton == value)
					return;
				showAddPageButton = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(true)]
		public bool ShowTabCloseButtons {
			get { return showTabCloseButtons; }
			set {
				if(ShowTabCloseButtons == value)
					return;
				showTabCloseButtons = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(true)]
		public bool AllowTabAnimation {
			get { return allowTabAnimation; }
			set {
				if(AllowTabAnimation == value)
					return;
				allowTabAnimation = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(true)]
		public bool AllowMoveTabsToOuterForm {
			get { return allowMoveTabsToOuterForm; }
			set {
				if(AllowMoveTabsToOuterForm == value)
					return;
				allowMoveTabsToOuterForm = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(true)]
		public bool AllowMoveTabs {
			get { return allowMoveTabs; }
			set {
				if(AllowMoveTabs == value)
					return;
				allowMoveTabs = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(200)]
		public int MaxTabWidth {
			get { return maxTabWidth; }
			set {
				if(MaxTabWidth == value)
					return;
				maxTabWidth = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(ShowTabsInTitleBar.False)]
		public ShowTabsInTitleBar ShowTabsInTitleBar {
			get { return showTabsInTitleBar; }
			set {
				if(ShowTabsInTitleBar == value)
					return;
				showTabsInTitleBar = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(0)]
		public int TitleTabVerticalOffset {
			get { return titleTabVerticalOffset; }
			set {
				if(TitleTabVerticalOffset == value)
					return;
				titleTabVerticalOffset = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter)), DefaultValue(null)]
		public virtual object Images {
			get { return images; }
			set {
				if(images == value)
					return;
				images = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				OnPropertiesChanged();
			}
		}
		protected virtual void OnPropertiesChanged() {
			ForceRefreshLayout();
		}
		TabFormControlAppearances appearance;
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[ Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TabFormControlAppearances Appearance {
			get { return appearance; }
		}
		internal void OnAppearanceChanged() { }
		[ DefaultValue(null)]
		public RibbonControl Ribbon {
			get { return ribbon; }
			set {
				if(Ribbon == value)
					return;
				ribbon = value;
				OnRibbonChanged();
			}
		}
		void OnRibbonChanged() {
			if(Ribbon != null) Manager = Ribbon.Manager;
		}
		bool IsDefaultBarManager { get { return base.Manager is TabFormDefaultManager; } }
		public override BarManager Manager {
			get {
				return GetManager();
			}
			protected set {
				SetManagerCore(value);
			}
		}
		BarManager GetManager() {
			if(base.Manager == null) {
				SetDefaultManager();
			}
			return base.Manager;
		}
		void SetDefaultManager() {
			SetManagerCore(CreateDefaultManager());
		}
		protected BarManager CreateDefaultManager() { return new TabFormDefaultManager(IsDesignMode); }
		internal override void SetManagerCore(BarManager manager) {
			if(manager == base.Manager) return;
			BarManager prevManager = base.Manager;
			if(prevManager != null) prevManager.Disposed -= OnManagerDisposed;
			if(manager == null) manager = CreateDefaultManager();
			manager.AllowDisposeItems = false;
			manager.Disposed += OnManagerDisposed;
			base.SetManagerCore(manager);
			OnManagerChanged(prevManager, manager);
		}
		void OnManagerDisposed(object sender, EventArgs e) {
			if(!IsDesignMode) return;
			SetDefaultManager();
			Manager.BeginInit();
			DefaultManagerEndInit();
		}
		void OnManagerChanged(BarManager prevManager, BarManager curManager) {
			if(prevManager != null) prevManager.ControllerChanged -= OnControllerChanged;
			if(curManager != null) {
				curManager.ControllerChanged += OnControllerChanged;
				MoveItems(prevManager, curManager);
			}
			if(prevManager is TabFormDefaultManager) prevManager.Dispose();
		}
		void MoveItems(BarManager prevManager, BarManager curManager) {
			if(!(prevManager is TabFormDefaultManager))
				return;
			while(prevManager.Items.Count > 0) {
				curManager.Items.Add(prevManager.Items[0]);
			}
			if(prevManager.Site != null) {
				prevManager.Site.Container.Remove(prevManager);
				prevManager.Dispose();
			}
		}
		protected virtual void OnControllerChanged(object sender, EventArgs e) {
			if(ViewInfo != null) {
				ViewInfo.DefaultAppearances.Update();
				Handler.UpdateHitInfoPattern();
				UpdateViewInfo();
				Invalidate();
			}
		}
		protected override LinksNavigation CreateLinksNavigator() {
			return new LinksControlNavigation(this);
		}
		public override BarItemLink GetLinkByPoint(Point screenPoint, bool includeSeparator) {
			BarItemLink link = ViewInfo.LinkInfoProvider.GetItemLinkByPoint(PointToClient(screenPoint));
			if(link != null) return link;
			return base.GetLinkByPoint(screenPoint, includeSeparator);
		}
		void ResetLinkProvider() { LinkProvider.Reset(); }
		bool ShouldSerializeLinkProvider() { return LinkProvider.ShouldSerialize(); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabFormLinkProvider LinkProvider { get { return linkProvider; } }
		[ Category(CategoryName.Data)]
		public TabFormLinkCollection TitleItemLinks { get { return LinkProvider.TitleItemLinks; } }
		[ Category(CategoryName.Data)]
		public TabFormLinkCollection TabLeftItemLinks { get { return LinkProvider.TabLeftItemLinks; } }
		[ Category(CategoryName.Data)]
		public TabFormLinkCollection TabRightItemLinks { get { return LinkProvider.TabRightItemLinks; } }
		bool ShouldSerializeItems() { return IsDefaultBarManager; }
		[ Category(CategoryName.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
		public BarItems Items { get { return (BarItems)Manager.Items; } }
		internal bool IsDesignMode { get { return DesignMode; } }
		bool isInInit = false;
		internal bool IsInInit { get { return isInInit; } }
		public void BeginInit() {
			this.isInInit = true;
			if(IsDefaultBarManager) Manager.BeginInit();
		}
		public void EndInit() {
			this.isInInit = false;
			if(IsDefaultBarManager) {
				DefaultManagerEndInit();
			}
			EndInitCore();
		}
		protected virtual void DefaultManagerEndInit() {
			Manager.EndInit();
		}
		protected virtual void EndInitCore() {
			InitImageUri();
			Init();
			Handler.UpdateHitInfoPattern();
		}
		protected void InitImageUri() {
			foreach(TabFormPage page in Pages) {
				page.ImageUri.SetClient(page);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LockLayout {
			get { return base.LockLayout; }
			set { base.LockLayout = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsVertical {
			get { return base.IsVertical; }
			set { base.IsVertical = value; }
		}
		protected internal bool IsInAnimation {
			get { return isInAnimation; }
			set { isInAnimation = value; }
		}
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			TabFormControlHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(hitInfo.HitTest == TabFormControlHitTest.AddPage || hitInfo.HitTest == TabFormControlHitTest.Page)
				Select();
			if(ShouldProcessMouseEvents(e)) {
				Handler.OnMouseDown(e);
				base.OnMouseDown(e);
			}
		}
		protected internal void ForceUpdateLinkInfo(BarItemLink sender){
			UpdateViewInfo();
			Invalidate();
		}
		protected bool ShouldProcessMouseEvents(System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) return false;
			return IsDesignMode || !ControlBase.GetValidationCanceled(this);
		}
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			Handler.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
			if(ShouldProcessMouseEvents(e)) {
				Handler.OnMouseUp(e);
				base.OnMouseUp(e);
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			Handler.OnMouseLeave();
			if(Manager == null || Manager.IsCustomizing) return;
			if(LinkProvider.Contains(Manager.SelectionInfo.HighlightedLink))
				Manager.SelectionInfo.HighlightedLink = null;
			base.OnMouseLeave(e);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(ViewInfo != null) height = ViewInfo.CalcControlBestHeight();
			base.SetBoundsCore(x, y, width, height, specified);
		}
		bool lockUpdateHeight = false;
		protected internal void LockUpdateHeight() {
			this.lockUpdateHeight = true;
		}
		protected internal void UnlockUpdateHeight() {
			this.lockUpdateHeight = false;
		}
		protected void CheckHeight() {
			if(!IsHandleCreated || this.lockUpdateHeight) return;
			Height = ViewInfo.CalcControlBestHeight();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Pages != null) {
					while(Pages.Count > 0) {
						TabFormPage page = Pages[0];
						Pages.Remove(page);
						page.Dispose();
					}
				}
				if(base.Manager != null)
					base.Manager.ControllerChanged -= OnControllerChanged;
			}
			base.Dispose(disposing);
		}
		bool isRightToLeft = false;
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			CheckRightToLeft();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			CheckRightToLeft();
		}
		protected void CheckRightToLeft() {
			bool newRightToLeft = WindowsFormsSettings.GetIsRightToLeft(this);
			if(newRightToLeft == this.isRightToLeft) return;
			this.isRightToLeft = newRightToLeft;
			OnRightToLeftChanged();
		}
		protected virtual void OnRightToLeftChanged() { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsRightToLeft { get { return isRightToLeft; } }
		[ Category(CategoryName.Events)]
		public event PageCreatedEventHandler PageCreated {
			add { Events.AddHandler(pageCreated, value); }
			remove { Events.RemoveHandler(pageCreated, value); }
		}
		[ Category(CategoryName.Events)]
		public event TabFormSelectedPageChangedEventHandler SelectedPageChanged {
			add { Events.AddHandler(selectedPageChanged, value); }
			remove { Events.RemoveHandler(selectedPageChanged, value); }
		}
		[ Category(CategoryName.Events)]
		public event TabFormSelectedPageChangingEventHandler SelectedPageChanging {
			add { Events.AddHandler(selectedPageChanging, value); }
			remove { Events.RemoveHandler(selectedPageChanging, value); }
		}
		[ Category(CategoryName.Events)]
		public event EventHandler PageCollectionChanged {
			add { Events.AddHandler(pageCollectionChanged, value); }
			remove { Events.RemoveHandler(pageCollectionChanged, value); }
		}
		[ Category(CategoryName.Events)]
		public event OuterFormCreatedEventHandler OuterFormCreated {
			add { Events.AddHandler(outerFormCreated, value); }
			remove { Events.RemoveHandler(outerFormCreated, value); }
		}
		[ Category(CategoryName.Events)]
		public event OuterFormCreatingEventHandler OuterFormCreating {
			add { Events.AddHandler(outerFormCreating, value); }
			remove { Events.RemoveHandler(outerFormCreating, value); }
		}
		protected internal virtual void RaiseOuterFormCreated(OuterFormCreatedEventArgs e) {
			OuterFormCreatedEventHandler handler = Events[outerFormCreated] as OuterFormCreatedEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseOuterFormCreating(OuterFormCreatingEventArgs e) {
			OuterFormCreatingEventHandler handler = Events[outerFormCreating] as OuterFormCreatingEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaisePageCollectionChanged(EventArgs e) {
			EventHandler handler = Events[pageCollectionChanged] as EventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseSelectedPageChanged(TabFormSelectedPageChangedEventArgs e) {
			TabFormSelectedPageChangedEventHandler handler = Events[selectedPageChanged] as TabFormSelectedPageChangedEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseSelectedPageChanging(TabFormSelectedPageChangingEventArgs e) {
			TabFormSelectedPageChangingEventHandler handler = Events[selectedPageChanging] as TabFormSelectedPageChangingEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaisePageCreated(PageCreatedEventArgs e) {
			PageCreatedEventHandler handler = Events[pageCreated] as PageCreatedEventHandler;
			if(handler != null) handler(this, e);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanDispose {
			get { return base.CanDispose; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Destroying {
			get { return base.Destroying; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool IsCanSpringLinks {
			get { return base.IsCanSpringLinks; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool IsMultiLine {
			get { return base.IsMultiLine; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool IsSubMenu {
			get { return base.IsSubMenu; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ContainerKeyTipManager KeyTipManager {
			get { return base.KeyTipManager; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override LinksNavigation Navigator {
			get { return base.Navigator; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool RotateWhenVertical {
			get { return base.RotateWhenVertical; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override BarItemLink ThisActiveLink {
			get { return base.ThisActiveLink; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override BarItemLinkReadOnlyCollection VisibleLinks {
			get { return base.VisibleLinks; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new InternalControlLinks ControlLinks {
			get { return base.ControlLinks; }
		}
	}
	[DesignTimeVisible(false), DXToolboxItem(false)]
	public class TabFormDefaultManager : BarManager {
		public TabFormDefaultManager() : base() { }
		public TabFormDefaultManager(bool designTimeManager) : base(designTimeManager) { }
	}
	public class TabFormPageEventArgs {
		public TabFormPageEventArgs(TabFormPage page) {
			this.page = page;
		}
		TabFormPage page;
		public TabFormPage Page {
			get { return page; }
		}
	}
	public class PageCreatedEventArgs : TabFormPageEventArgs {
		public PageCreatedEventArgs(TabFormPage page) : base(page) { }
	}
	public delegate void PageCreatedEventHandler(object sender, PageCreatedEventArgs e);
	public class TabFormSelectedPageChangedEventArgs : TabFormPageEventArgs {
		public TabFormSelectedPageChangedEventArgs(TabFormPage page, TabFormPage prevPage) : base(page) {
			this.prevPage = prevPage;
		}
		TabFormPage prevPage;
		public TabFormPage PrevPage {
			get { return prevPage; }
		}
	}
	public delegate void TabFormSelectedPageChangedEventHandler(object sender, TabFormSelectedPageChangedEventArgs e);
	public class TabFormSelectedPageChangingEventArgs : TabFormSelectedPageChangedEventArgs {
		public TabFormSelectedPageChangingEventArgs(TabFormPage page, TabFormPage prevPage)
			: base(page, prevPage) {
			this.cancel = false;
		}
		bool cancel;
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public delegate void TabFormSelectedPageChangingEventHandler(object sender, TabFormSelectedPageChangingEventArgs e);
	public class OuterFormCreatedEventArgs {
		public OuterFormCreatedEventArgs(TabForm form) {
			this.form = form;
		}
		TabForm form;
		public TabForm Form {
			get { return form; }
		}
	}
	public delegate void OuterFormCreatedEventHandler(object sender, OuterFormCreatedEventArgs e);
	public class OuterFormCreatingEventArgs {
		public OuterFormCreatingEventArgs() {
			this.form = null;
		}
		TabForm form;
		public TabForm Form {
			get { return form; }
			set { form = value; }
		}
	}
	public delegate void OuterFormCreatingEventHandler(object sender, OuterFormCreatingEventArgs e);
}
