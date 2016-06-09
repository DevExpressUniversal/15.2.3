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

using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.Drawing;
namespace DevExpress.XtraBars.Ribbon {
	public delegate void RecentEventHandler(object sender, EventArgs e);
	public delegate void RecentItemEventHandler(object sender, RecentItemEventArgs e);
	public class RecentItemEventArgs : EventArgs {
		RecentItemBase item;
		public RecentItemEventArgs(RecentItemBase item) {
			this.item = item;
		}
		public RecentItemBase Item { get { return item; } }
	}
	[DXToolboxItem(true),
	Description("Allows you to build content of any complexity for BackstageView Control's tabs."),
	ToolboxTabName(AssemblyInfo.DXTabNavigation),
	Designer("DevExpress.XtraBars.Ribbon.Design.RecentControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)), ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "RecentItemControl")
	]
	public class RecentItemControl : BaseStyleControl, IXtraAnimationListener, ISupportXtraAnimation, ISupportInitialize
 {
		private static readonly object selectedTabChanged = new object();
		private static readonly object splitterPositionChanged = new object();
		private static readonly object itemClick = new object();
		static readonly Padding DefaultTitlePadding = Padding.Empty;
		static readonly Size DefaultControlSize = new Size(640, 450);
		internal readonly object AnimationId = new object();
		const int DefaultSplitterPosition = 320;
		string title;
		Padding titlePadding;
		Padding paddingRecentItem;
		Padding paddingLabelItem;
		Padding paddingTabItem;
		Padding paddingButtonItem;
		AppearanceObject appearanceTitle;
		AppearanceObject appearancePanelCaption;
		AppearanceObject appearanceRecentItemCaption;
		AppearanceObject appearanceLabelItemCaption;
		AppearanceObject appearanceTabItemCaption;
		AppearanceObject appearanceButtonItemCaption;
		RecentStackPanel mainPanel;
		RecentStackPanel defaultContentPanel;
		bool showTitle;
		bool showSplitter;
		RecentPanelBase contentPanel;
		RecentTabItem selectedTab;
		RecentScrollController scrollController;
		int scrollerPosition;
		int splitterPosition;
		RecentAppearanceCollection appearance;
		bool isInitialize;
		int mainPanelMinWidth;
		int contentPanelMinWidth;
		public RecentItemControl() {
			this.title = "Title";
			this.titlePadding = DefaultTitlePadding;
			this.paddingButtonItem = Padding.Empty;
			this.paddingLabelItem = Padding.Empty;
			this.paddingRecentItem = Padding.Empty;
			this.paddingTabItem = Padding.Empty;
			this.appearance = CreateAppearances();
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.appearanceTitle = CreateAppearance();
			this.appearancePanelCaption = CreateAppearance();
			this.appearanceButtonItemCaption = CreateAppearance();
			this.appearanceLabelItemCaption = CreateAppearance();
			this.appearanceRecentItemCaption = CreateAppearance();
			this.appearanceTabItemCaption = CreateAppearance();
			this.mainPanel = new RecentStackPanel();
			this.defaultContentPanel = new RecentStackPanel();
			this.contentPanel = defaultContentPanel;
			this.showTitle = true;
			this.showSplitter = true;
			this.splitterPosition = DefaultSplitterPosition;
			this.scrollController = CreateScrollController();
			this.scrollController.AddControls(this);
			this.scrollController.VScrollValueChanged += OnVScrollValueChanged;
			this.mainPanelMinWidth = 0;
			this.contentPanelMinWidth = 0;
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
		public override Color BackColor {
			get {
				if(Parent != null && Parent is BackstageViewClientControl) 
					return Color.Transparent;
				return base.BackColor;
			}
			set { base.BackColor = value;
			}
		}
		protected virtual RecentScrollController CreateScrollController() {
			return new RecentScrollController(this);
		}
		protected virtual void OnVScrollValueChanged(object sender, EventArgs e) {
			ScrollerPosition = scrollController.VScrollPosition;
		}
		protected internal RecentScrollController ScrollController { get { return scrollController; } }
		protected internal int ScrollerPosition {
			get { return scrollerPosition; }
			set {
				if(ScrollerPosition == value) return;
				scrollerPosition = ConstrainPosition(value);
				OnScrollerPositionChanged();
			}
		}
		protected int ConstrainPosition(int value) {
			if(value < 0)
				value = 0;
			int height = Math.Max(0, ScrollController.VScrollMaximum - ScrollController.VScrollLargeChange);
			if(value > height)
				value = height;
			return value;
		}
		protected virtual void OnScrollerPositionChanged() {
			Refresh();
		}
		protected override XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new RecentControlViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new RecentControlPainter();
		}
		protected override Size DefaultSize { get { return DefaultControlSize; } }
		public RecentTabItem SelectedTab {
			get { return selectedTab; }
			set {
				if(SelectedTab == value)
					return;
				RecentTabItem prev = selectedTab;
				selectedTab = value;
				OnSelectedTabChanged(prev);
			}
		}
		protected override Size DefaultMinimumSize
		{
			get { return new Size(SplitterPosition + 50, 100); }
		}
		private void OnSelectedTabChanged(RecentTabItem prev) {
			if(SelectedTab != null ) {
				if(SelectedTab.TabPanel.RecentControl == null) SelectedTab.TabPanel.SetOwnerControl(this);
				if(prev != null) {
					prev.TabPanel.HidePanel();
					Invalidate(prev.ViewInfo.Bounds);
				}
				SelectedTab.TabPanel.ShowPanel();
				if(!isInitialize && !IsDesignMode)
					GetViewInfo().AddTransitionAnimation();
				else OnPropertiesChanged();
			}
			RaiseSelectedTabChanged();
		}
		protected internal void UpdateSelectedTabToDefault() {
			SelectedTab = null;
		}
		RecentControlHandler handler;
		protected RecentControlHandler Handler {
			get {
				if(this.handler == null)
					this.handler = CreateHandler();
				return this.handler;
			}
		}
		protected RecentControlHandler CreateHandler() {
			return new RecentControlHandler(this);
		}
		[DefaultValue("")]
		public string Title {
			get { return title; }
			set {
				if(Title == value)
					return;
				title = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(DefaultSplitterPosition)]
		public int SplitterPosition {
			get { return CheckSpliterPosition(splitterPosition); }
			set {
				int newValue = CheckSpliterPosition(value);
				if(SplitterPosition == newValue) return;
				splitterPosition = newValue;
				OnPropertiesChanged();
			}
		}
		protected internal int CheckSpliterPosition(int value) {
			if(value < MainPanelMinWidth)  return MainPanelMinWidth;
			if(value > Bounds.Width - ContentPanelMinWidth)
				if(Bounds.Width - ContentPanelMinWidth < MainPanelMinWidth) return MainPanelMinWidth;
				else return Bounds.Right - ContentPanelMinWidth;
			return value;
		}
		[DefaultValue(0), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public int MainPanelMinWidth {
			get { return mainPanelMinWidth; }
			set {
				if(MainPanelMinWidth == value) return;
				if(value < 0) value = 0;
				if(value > Bounds.Width) value = Bounds.Width;
				mainPanelMinWidth = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(0), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public int ContentPanelMinWidth {
			get { return contentPanelMinWidth; }
			set {
				if(ContentPanelMinWidth == value) return;
				if(value < 0) value = 0;
				if(value > Bounds.Width) value = Bounds.Width;
				contentPanelMinWidth = value;
				OnPropertiesChanged();
			}
		}
		#region Paddings
		bool ShouldSerializeTitlePadding() { return TitlePadding != DefaultTitlePadding; }
		void ResetTitlePadding() { TitlePadding = DefaultTitlePadding; }
		public Padding TitlePadding {
			get { return titlePadding; }
			set {
				if(TitlePadding == value)
					return;
				titlePadding = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializePaddingButtonItem() { return PaddingButtonItem != Padding.Empty; }
		void ResetPaddingButtonItem() { PaddingButtonItem = Padding.Empty; }
		public Padding PaddingButtonItem {
			get { return paddingButtonItem; }
			set {
				if(PaddingButtonItem == value)
					return;
				paddingButtonItem = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializePaddingLabelItem() { return PaddingLabelItem != Padding.Empty; }
		void ResetPaddingLabelItem() { PaddingLabelItem = Padding.Empty; }
		public Padding PaddingLabelItem {
			get { return paddingLabelItem; }
			set {
				if(PaddingLabelItem == value)
					return;
				paddingLabelItem = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializePaddingRecentItem() { return PaddingRecentItem != Padding.Empty; }
		void ResetPaddingRecentItem() { PaddingRecentItem = Padding.Empty; }
		public Padding PaddingRecentItem {
			get { return paddingRecentItem; }
			set {
				if(PaddingRecentItem == value)
					return;
				paddingRecentItem = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializePaddingTabItem() { return PaddingTabItem != Padding.Empty; }
		void ResetPaddingTabItem() { PaddingTabItem = Padding.Empty; }
		public Padding PaddingTabItem {
			get { return paddingTabItem; }
			set {
				if(PaddingTabItem == value)
					return;
				paddingTabItem = value;
				OnPropertiesChanged();
			}
		}
		#endregion
		#region Appearances
		private RecentAppearanceCollection CreateAppearances() {
			return new RecentAppearanceCollection();
		}
		[
		DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RecentAppearanceCollection Appearances { get { return appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new AppearanceObject Appearance { get { return base.Appearance; } }
		protected void OnAppearanceChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		#endregion
		[DefaultValue(true)]
		public bool ShowTitle {
			get { return showTitle; }
			set {
				if(ShowTitle == value) return;
				showTitle = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(true)]
		public bool ShowSplitter {
			get { return showSplitter; }
			set {
				if(ShowSplitter == value) return;
				showSplitter = value;
				OnPropertiesChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RecentPanelBase MainPanel { get { return mainPanel; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RecentPanelBase DefaultContentPanel { get { return defaultContentPanel; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RecentPanelBase ContentPanel {
			get {
				if(SelectedTab != null) return SelectedTab.TabPanel;
				return DefaultContentPanel;
			}
		}
		public new RecentControlViewInfo GetViewInfo() { return ViewInfo as RecentControlViewInfo; }
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			OnPropertiesChanged();
		}
		void OnSecondPanelItemsListChanged(object sender, ListChangedEventArgs e) {
			OnPropertiesChanged();
		}
		void OnMainPanelItemsListChanged(object sender, ListChangedEventArgs e) {
			OnPropertiesChanged();
		}
		protected override void LayoutChanged() {
			base.LayoutChanged();
			UpdateScrollers();
			InvalidateControlContainers();
		}
		private void InvalidateControlContainers() {
			MainPanel.ViewInfo.InvalidateControlContainers();
			ContentPanel.ViewInfo.InvalidateControlContainers();
		}
		protected override void OnPropertiesChanged() {
			GetViewInfo().SetAppearanceDirty();
			GetViewInfo().ResetSplitterPos();
			base.OnPropertiesChanged();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			MainPanel.SetOwnerControl(this);
			DefaultContentPanel.SetOwnerControl(this);
		}
		protected override void OnAfterUpdateViewInfo() {
			base.OnAfterUpdateViewInfo();
			UpdateScrollers();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			OnPropertiesChanged();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseMove(ee);
			if(ee.Handled) return;
			Handler.OnMouseMove(ee);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseDown(e);
			if(ee.Handled) return;
			Handler.OnMouseDown(ee);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseUp(e);
			if(ee.Handled) return;
			Handler.OnMouseUp(ee);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Handler.OnMouseLeave(e);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			CheckScrollPos();
			Refresh();
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			Handler.OnMouseWheel(e);
		}
		protected override void OnDragEnter(DragEventArgs e) {
			if(!IsDesignMode)
				base.OnDragEnter(e);
			else
				Handler.OnDragEnter(e);
		}
		protected override void OnDragLeave(EventArgs e) {
			if(!IsDesignMode)
				base.OnDragLeave(e);
			else
				Handler.OnDragLeave(e);
		}
		protected override void OnDragOver(DragEventArgs e) {
			if(!IsDesignMode)
				base.OnDragOver(e);
			else
				Handler.OnDragOver(e);
		}
		protected override void OnDragDrop(DragEventArgs e) {
			if(!IsDesignMode)
				base.OnDragDrop(e);
			else
				Handler.OnDragDrop(e);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if(!IsDesignMode)
				base.OnGiveFeedback(e);
			else
				Handler.OnGiveFeedback(e);
		}
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			if(!IsDesignMode)
				base.OnQueryContinueDrag(e);
			else
				Handler.OnQueryContinueDrag(e);
		}
		protected bool IsVScrollVisible { get { return (ViewInfo as RecentControlViewInfo).IsVScrollVisible; } }
		protected virtual void CheckScrollPos() {
			if(ScrollerPosition != 0 && !IsVScrollVisible) ScrollerPosition = 0;
		}
		protected virtual void UpdateScrollers() {
			if(IsDisposing ) return;
			if(DesignMode) ScrollController.VScroll.Enabled = false;
			else ScrollController.VScroll.Enabled = true;
			ScrollController.IsRightToLeft = IsRightToLeft;
			ScrollController.VScrollVisible = IsVScrollVisible;
			ScrollController.ClientRect = GetViewInfo().PanelsAreaBounds; 
			ScrollController.VScroll.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			if(ScrollController.VScrollVisible) ScrollController.VScrollArgs = (ViewInfo as RecentControlViewInfo).CalcVScrollArgs();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.appearance.Changed -= new EventHandler(OnAppearanceChanged);
				if(ScrollController != null) {
					ScrollController.VScrollValueChanged -= OnVScrollValueChanged;
					ScrollController.RemoveControls(this);
					ScrollController.Dispose();
				}
				this.scrollController = null;
				MainPanel.Dispose();
				DefaultContentPanel.Dispose();
			}
			base.Dispose(disposing);
		}
		public RecentControlHitInfo CalcHitInfo(Point p) {
			return GetViewInfo().CalcHitInfo(p);
		}
		RecentControlDesignTimeManager designTimeManagerCore = null;
		protected internal RecentControlDesignTimeManager DesignTimeManager {
			get {
				if(designTimeManagerCore == null) {
					designTimeManagerCore = CreateDesignTimeManager();
				}
				return designTimeManagerCore;
			}
		}
		protected virtual RecentControlDesignTimeManager CreateDesignTimeManager() {
			return new RecentControlDesignTimeManager(this, Site);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public RecentControlDesignTimeManager GetDesignTimeManager() {
			return DesignTimeManager;
		}
		#region Animation Members
		void IXtraAnimationListener.OnAnimation(BaseAnimationInfo info) {
			GetViewInfo().OnAnimation();
		}
		void IXtraAnimationListener.OnEndAnimation(BaseAnimationInfo info) {
			GetViewInfo().OnEndAnimation();
		}
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		#endregion
		protected internal object InternalGetService(Type service) {
			return GetService(service);
		}
		internal void FireRecentControlChanged(Component component) {
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				srv.OnComponentChanged(component, null, null, null);
			}
		}
		#region Events
		public event RecentItemEventHandler SelectedTabChanged {
			add { Events.AddHandler(selectedTabChanged, value); }
			remove { Events.RemoveHandler(selectedTabChanged, value); }
		}
		public event RecentEventHandler SplitterPositionChanged {
			add { Events.AddHandler(splitterPositionChanged, value); }
			remove { Events.RemoveHandler(splitterPositionChanged, value); }
		}
		public event RecentItemEventHandler ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		protected internal void RaiseSelectedTabChanged() {
			RecentItemEventHandler handler = Events[selectedTabChanged] as RecentItemEventHandler;
			if(handler != null)
				handler(this, new RecentItemEventArgs(SelectedTab));
		}
		protected internal void RaiseSplitterPositionChanged() {
			RecentEventHandler handler = Events[splitterPositionChanged] as RecentEventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}		
		protected internal void RaiseItemClick(RecentItemBase item) {
			RecentItemEventHandler handler = Events[itemClick] as RecentItemEventHandler;
			if(handler != null)
				handler(this, new RecentItemEventArgs(item));
		}
		#endregion
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			ToolTipControlInfo info = base.GetToolTipInfo(point);
			return GetViewInfo().GetToolTipInfo(point);
		}
		void ISupportInitialize.BeginInit() {
			this.isInitialize = true;
		}
		void ISupportInitialize.EndInit() {
			this.isInitialize = false;
			GetViewInfo().SetAppearanceDirty();
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			base.SetBoundsCore(x, y, width, height, specified);
		}
	}
	public class RecentControlDesignTimeManager : BaseDesignTimeManager {
		public RecentControlDesignTimeManager(RecentItemControl owner, ISite site) : base(owner, site) { }
		public RecentItemControl RecentControl { get { return Owner as RecentItemControl; } }
		public virtual void OnAddLabelItem(RecentPanelBase panel) {
			RecentLabelItem labelItem = new RecentLabelItem();
			RecentControl.Container.Add(labelItem);
			panel.Items.Add(labelItem);
			labelItem.Caption = labelItem.Name;
		}
		public virtual void OnAddButtonItem(RecentPanelBase panel) {
			RecentButtonItem buttonItem = new RecentButtonItem();
			DesignerHost.Container.Add(buttonItem);
			RecentControl.Container.Add(buttonItem);
			panel.Items.Add(buttonItem);
			buttonItem.Caption = buttonItem.Name;
		}
		public virtual void OnAddRecentItem(RecentPanelBase panel) {
			RecentPinItem recentItem = new RecentPinItem();
			DesignerHost.Container.Add(recentItem);
			RecentControl.Container.Add(recentItem);
			panel.Items.Add(recentItem);
			recentItem.Caption = recentItem.Name;
		}
		public virtual void OnAddSeparatorItem(RecentPanelBase panel) {
			RecentSeparatorItem separatorItem = new RecentSeparatorItem();
			DesignerHost.Container.Add(separatorItem);
			RecentControl.Container.Add(separatorItem);
			panel.Items.Add(separatorItem);
		}
		public virtual void OnAddTabItem(RecentPanelBase panel) {
			if(panel == RecentControl.ContentPanel) {
				MessageBox.Show("Tab Items can be hosted only within the main panel.");
				return;
			}
			RecentTabItem tabItem = new RecentTabItem();
			DesignerHost.Container.Add(tabItem);
			RecentControl.Container.Add(tabItem);
			panel.Items.Add(tabItem);
			tabItem.Caption = tabItem.Name;
		}
		public virtual void OnAddContainerItem(RecentPanelBase panel) {
			RecentControlContainerItem containerItem = new RecentControlContainerItem();
			DesignerHost.Container.Add(containerItem);
			RecentControl.Container.Add(containerItem);
			panel.Items.Add(containerItem);
		}
		public virtual void OnAddHyperLinkItem(RecentPanelBase panel) {
			RecentHyperlinkItem hyperLinkItem = new RecentHyperlinkItem();
			DesignerHost.Container.Add(hyperLinkItem);
			RecentControl.Container.Add(hyperLinkItem);
			panel.Items.Add(hyperLinkItem);
			hyperLinkItem.Caption = hyperLinkItem.Name;
		}
	}
	public class RecentStackPanel : RecentPanelBase {
		public RecentStackPanel() : base() { }
		protected override RecentPanelViewInfoBase CreatePanelViewInfo() {
			return new RecentStrackPanelViewInfo(this);
		}
		protected override RecentPanelPainterBase CreatePanelPainter() {
			return new RecentStackPanelPainter();
		}
	}
	public class RecentTablePanel : RecentStackPanel {
		private readonly static int DefaultRowCount = 3;
		private readonly static int DefaultColCount = 2;
		int rowCount;
		int colCount;
		public RecentTablePanel()
			: base() {
			this.colCount = DefaultColCount;
			this.rowCount = DefaultRowCount;
		}
		public int RowCount {
			get { return rowCount; }
			set {
				if(RowCount == value) return;
				rowCount = value;
				OnPanelChanged();
			}
		}
		public int ColCount {
			get { return colCount; }
			set {
				if(ColCount == value) return;
				colCount = value;
				OnPanelChanged();
			}
		}
		protected override RecentPanelViewInfoBase CreatePanelViewInfo() {
			return new RecentTablePanelViewInfo(this);
		}
	}
	public class RecentScrollController : IDisposable {
		RecentItemControl owner;
		DevExpress.XtraEditors.VScrollBar vScroll;
		bool vScrollVisible;
		Rectangle clientRect, vscrollRect;
		bool isRightToLeft;
		public RecentScrollController(RecentItemControl owner) {
			this.owner = owner;
			this.clientRect = this.vscrollRect = Rectangle.Empty;
			this.vScroll = CreateVScroll();
			this.VScroll.Visible = false;
			this.VScroll.SmallChange = 1;
			this.VScroll.LookAndFeel.ParentLookAndFeel = owner.LookAndFeel;
			this.isRightToLeft = false;
			ScrollBarBase.ApplyUIMode(VScroll);
		}
		protected virtual DevExpress.XtraEditors.VScrollBar CreateVScroll() { return new DevExpress.XtraEditors.VScrollBar(); }
		public virtual void AddControls(Control container) {
			if(container == null) return;
			container.Controls.Add(VScroll);
		}
		public virtual void RemoveControls(Control container) {
			if(container == null) return;
			container.Controls.Remove(VScroll);
		}
		public virtual int VScrollWidth {
			get { return VScroll.GetDefaultVerticalScrollBarWidth(); }
		}
		public int VScrollPosition { get { return VScroll.Value; } }
		public DevExpress.XtraEditors.VScrollBar VScroll { get { return vScroll; } }
		public ScrollArgs VScrollArgs {
			get { return new ScrollArgs(VScroll); }
			set { value.AssignTo(VScroll); }
		}
		public Rectangle VScrollRect { get { return vscrollRect; } }
		public int VScrollMaximum { get { return VScroll.Maximum; } }
		public int VScrollLargeChange { get { return VScroll.LargeChange; } }
		public bool VScrollVisible {
			get { return vScrollVisible; }
			set {
				if(VScrollVisible == value) UpdateVisiblity();
				else {
					vScrollVisible = value;
					LayoutChanged();
				}
			}
		}
		public bool IsRightToLeft {
			get {
				return isRightToLeft;
			}
			set {
				if(isRightToLeft == value) return;
				isRightToLeft = value;
				LayoutChanged();
			}
		}
		public Rectangle ClientRect {
			get { return clientRect; }
			set {
				if(ClientRect == value) return;
				clientRect = value;
				LayoutChanged();
			}
		}
		protected virtual void CalcRects() {
			this.vscrollRect = Rectangle.Empty;
			Rectangle r = Rectangle.Empty;
			if(VScrollVisible) {
				if(!IsRightToLeft) {
					r.Location = new Point(ClientRect.Right - VScrollWidth, ClientRect.Y);
				}
				else {
					r.Location = new Point(ClientRect.X, ClientRect.Y);
				}
				r.Size = new Size(VScrollWidth, ClientRect.Height);
				vscrollRect = r;
			}
		}
		public void UpdateVisiblity() {
			VScroll.SetVisibility(vScrollVisible && !ClientRect.IsEmpty);
			VScroll.Bounds = VScrollRect;
		}
		int lockLayout = 0;
		public virtual void LayoutChanged() {
			if(lockLayout != 0) return;
			lockLayout++;
			try {
				CalcRects();
				UpdateVisiblity();
				if(ClientRect.IsEmpty) VScroll.SetVisibility(false);
			}
			finally {
				lockLayout--;
			}
		}
		public event EventHandler VScrollValueChanged {
			add { VScroll.ValueChanged += value; }
			remove { VScroll.ValueChanged -= value; }
		}
		internal void OnAction(ScrollNotifyAction action) {
			VScroll.OnAction(action);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(VScroll != null) VScroll.Dispose();
			}
		}
	}
	public class RecentAppearanceCollection : BaseAppearanceCollection {
		AppearanceObject panelCaption, title;
		BaseRecentItemAppearanceCollection buttonItem, tabItem;
		RecentLabelItemAppearances labelItem;
		RecentPinItemAppearances recentItem;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.panelCaption = CreateAppearance("PanelCaption");
			this.title = CreateAppearance("Title");
			this.buttonItem = CreateBaseAppearanceCollection();
			this.tabItem = CreateBaseAppearanceCollection();
			this.labelItem = CreateLabelAppearanceCollection();
			this.recentItem = CreateRecentAppearanceCollection();
			this.title.Changed += new EventHandler(OnAppearanceChanged);
			this.panelCaption.Changed += new EventHandler(OnAppearanceChanged);
			this.tabItem.Changed += new EventHandler(OnAppearanceChanged);
			this.buttonItem.Changed += new EventHandler(OnAppearanceChanged);
			this.labelItem.Changed += new EventHandler(OnAppearanceChanged);
			this.recentItem.Changed += new EventHandler(OnAppearanceChanged);
		}
		protected override void OnChanged() {
			base.OnChanged();
		}
		RecentPinItemAppearances CreateRecentAppearanceCollection() {
			return new RecentPinItemAppearances();
		}
		RecentLabelItemAppearances CreateLabelAppearanceCollection() {
			return new RecentLabelItemAppearances();
		}
		BaseRecentItemAppearanceCollection CreateBaseAppearanceCollection() {
			return new BaseRecentItemAppearanceCollection();
		}
		void ResetPanelCaption() { PanelCaption.Reset(); }
		bool ShouldSerializePanelCaption() { return PanelCaption.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject PanelCaption { get { return panelCaption; } }
		void ResetTitle() { Title.Reset(); }
		bool ShouldSerializeTitle() { return Title.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Title { get { return title; } }
		void ResetButtonItem() { ButtonItem.Reset(); }
		bool ShouldSerializeButtonItem() { return ButtonItem.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BaseRecentItemAppearanceCollection ButtonItem { get { return buttonItem; } }
		void ResetTabItem() { TabItem.Reset(); }
		bool ShouldSerializeTabItem() { return TabItem.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BaseRecentItemAppearanceCollection TabItem { get { return tabItem; } }
		void ResetLabelItem() { LabelItem.Reset(); }
		bool ShouldSerializeLabelItem() { return LabelItem.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RecentLabelItemAppearances LabelItem { get { return labelItem; } }
		void ResetRecentItem() { RecentItem.Reset(); }
		bool ShouldSerializeRecentItem() { return RecentItem.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RecentPinItemAppearances RecentItem { get { return recentItem; } }
		public override bool ShouldSerialize() {
			return base.ShouldSerialize() || PanelCaption.ShouldSerialize() || Title.ShouldSerialize() || ButtonItem.ShouldSerialize() || TabItem.ShouldSerialize() || LabelItem.ShouldSerialize() || RecentItem.ShouldSerialize();
		}
		public override string ToString() {
			return string.Empty;
		}
		public override void Dispose() {
			if(PanelCaption != null) PanelCaption.Dispose();
			if(Title != null) Title.Dispose();
			if(ButtonItem != null) ButtonItem.Dispose();
			if(RecentItem != null) RecentItem.Dispose();
			if(TabItem != null) TabItem.Dispose();
			if(LabelItem != null) LabelItem.Dispose();
			base.Dispose();
		}
	}
	public class BaseRecentItemAppearanceCollection : BaseAppearanceCollection {
		AppearanceObject itemNormal, itemHovered, itemPressed;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.itemNormal = CreateAppearance("ItemNormal");
			this.itemHovered = CreateAppearance("ItemHovered");
			this.itemPressed = CreateAppearance("ItemPressed");
		}
		void ResetItemNormal() { ItemNormal.Reset(); }
		bool ShouldSerializeItemNormal() { return ItemNormal.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemNormal { get { return itemNormal; } }
		void ResetItemHovered() { ItemHovered.Reset(); }
		bool ShouldSerializeItemHovered() { return ItemHovered.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemHovered { get { return itemHovered; } }
		void ResetItemPressed() { ItemPressed.Reset(); }
		bool ShouldSerializeItemPressed() { return ItemPressed.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemPressed { get { return itemPressed; } }
	}
}
