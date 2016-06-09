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
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
namespace DevExpress.XtraToolbox {
	[
	DXToolboxItem(true),
	ToolboxBitmap(typeof(XtraBars.ToolboxIcons.ToolboxIconsRootNS), "ToolboxControl"),
	Designer("DevExpress.XtraToolbox.Design.ToolboxControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	ToolboxTabName(AssemblyInfo.DXTabNavigation), 
	Description("Displays groups of items and provides drag-and-drop functionality of these items to external controls.")
	]
	public class ToolboxControl : BaseStyleControl, IToolboxControl, IMouseWheelSupport, IGestureClient, IDisposable {
		static readonly object itemClick = new object();
		static readonly object itemDoubleClick = new object();
		static readonly object dragItemDrop = new object();
		static readonly object dragItemStart = new object();
		static readonly object dragItemMove = new object();
		static readonly object getItemImage = new object();
		static readonly object selectedGroupChanged = new object();
		static readonly object initializeMenu = new object();
		static readonly object searchTextChanged = new object();
		static readonly object stateChanged = new object();
		bool shouldDrawOnlyItems;
		ToolboxMenuButton menuButton;
		ToolboxExpandButton expandButton;
		ToolboxMoreItemsButton moreItemsButton;
		ToolboxScrollButtonUp scrollButtonUp;
		ToolboxScrollButtonDown scrollButtonDown;
		ToolboxGroupCollection groups;
		ToolboxHandler handler;
		ToolboxScrollController scrollController;
		ToolboxAppearance appearance;
		ToolboxDesignTimeManager designManager;
		ToolboxOptionsMinimizing optionsMinimizing;
		GestureHelper gesture;
		public ToolboxControl() {
			this.gesture = new GestureHelper(this);
			this.searchControl = CreateSearchControl();
			this.menuButton = CreateMenuButton();
			this.expandButton = CreateExpandButton();
			this.moreItemsButton = CreateMoreItemsButton();
			this.scrollButtonUp = CreateScrollButtonUp();
			this.scrollButtonDown = CreateScrollButtonDown();
			this.groups = CreateGroupCollection();
			this.groups.ListChanged += OnGroupCollectionChanged;
			this.handler = CreateHandler();
			this.appearance = CreateToolboxAppearance();
			this.designManager = CreateDesignManager();
			this.scrollController = CreateScrollController();
			this.scrollController.GroupScroll.VScrollValueChanged += GroupScrollBarValueChanged;
			this.scrollController.ItemScroll.VScrollValueChanged += ItemScrollBarValueChanged;
			this.shouldDrawOnlyItems = false;
			this.menuManager = null;
			this.caption = "Toolbox";
			SubscribeOptionsEvents();
		}
		protected virtual void SubscribeOptionsEvents() {
			OptionsMinimizing.Changing += OnOptionMinimizingChanging;
			OptionsMinimizing.Changed += OnOptionMinimizingChanged;
			OptionsView.Changing += OnOptionViewChanging;
			OptionsView.Changed += OnOptionViewChanged;
			OptionsBehavior.Changing += OnOptionBehaviorChanging;
			OptionsBehavior.Changed += OnOptionBehaviorChanged;
		}
		protected virtual void UnsubscribeOptionsEvents() {
			OptionsMinimizing.Changing -= OnOptionMinimizingChanging;
			OptionsMinimizing.Changed -= OnOptionMinimizingChanged;
			OptionsView.Changing -= OnOptionViewChanging;
			OptionsView.Changed -= OnOptionViewChanged;
			OptionsBehavior.Changing -= OnOptionBehaviorChanging;
			OptionsBehavior.Changed -= OnOptionBehaviorChanged;
		}
		#region public properties
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ToolboxOptionsMinimizing OptionsMinimizing {
			get { return optionsMinimizing ?? (optionsMinimizing = CreateOptionsMinimizing()); }
		}
		protected virtual ToolboxOptionsMinimizing CreateOptionsMinimizing() {
			return new ToolboxOptionsMinimizing();
		}
		protected virtual void OnOptionMinimizingChanging(object sender, ToolboxOptionChangingEventArgs e) { }
		protected virtual void OnOptionMinimizingChanged(object sender, BaseOptionChangedEventArgs e) {
			OnPropertiesChanged();
		}
		ToolboxOptionsView optionsView;
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ToolboxOptionsView OptionsView {
			get { return optionsView ?? (optionsView = CreateOptionsView()); }
		}
		protected virtual ToolboxOptionsView CreateOptionsView() {
			return new ToolboxOptionsView();
		}
		protected virtual void OnOptionViewChanging(object sender, ToolboxOptionChangingEventArgs e) { }
		protected virtual void OnOptionViewChanged(object sender, BaseOptionChangedEventArgs e) {
			OnPropertiesChanged();
		}
		ToolboxOptionsBehavior optionsBehavior;
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ToolboxOptionsBehavior OptionsBehavior {
			get { return optionsBehavior ?? (optionsBehavior = CreateOptionsBehavior()); }
		}
		protected virtual ToolboxOptionsBehavior CreateOptionsBehavior() {
			return new ToolboxOptionsBehavior();
		}
		protected virtual void OnOptionBehaviorChanging(object sender, ToolboxOptionChangingEventArgs e) { }
		protected virtual void OnOptionBehaviorChanged(object sender, BaseOptionChangedEventArgs e) {
			OnPropertiesChanged();
		}
		IToolboxGroup selectedGroup;
		[ DefaultValue(null)]
		public IToolboxGroup SelectedGroup {
			get { return selectedGroup; }
			set {
				if(SelectedGroup == value)
					return;
				OnSelectedGroupChanging();
				ToolboxSelectedGroupChangedEventArgs args = new ToolboxSelectedGroupChangedEventArgs(value);
				RaiseSelectedGroupChanged(args);
				selectedGroup = value;
				OnSelectedGroupChanged();
			}
		}
		[ DefaultValue(-1), RefreshProperties(RefreshProperties.Repaint)]
		public int SelectedGroupIndex {
			get {
				if(SelectedGroup == null) return -1;
				return Groups.IndexOf(SelectedGroup as ToolboxGroup);
			}
			set {
				if(Groups.Count <= value || value < -1) return;
				SelectedGroup = value == -1 ? null : Groups[value];
			}
		}
		string caption;
		[ DefaultValue("Toolbox")]
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value)
					return;
				caption = value;
				OnPropertiesChanged();
			}
		}
		IDXMenuManager menuManager;
		[DefaultValue(null)]
		public IDXMenuManager MenuManager {
			get { return menuManager; }
			set {
				if(MenuManager == value)
					return;
				menuManager = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override void OnPropertiesChanged() {
			base.OnPropertiesChanged();
			Refresh();
		}
		#endregion
		#region Events
		[DXCategory(CategoryName.Events)]
		public event ToolboxItemClickEventHandler ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ToolboxItemDoubleClickEventHandler ItemDoubleClick {
			add { Events.AddHandler(itemDoubleClick, value); }
			remove { Events.RemoveHandler(itemDoubleClick, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ToolboxDragItemDropEventHandler DragItemDrop {
			add { Events.AddHandler(dragItemDrop, value); }
			remove { Events.RemoveHandler(dragItemDrop, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ToolboxDragItemStartEventHandler DragItemStart {
			add { Events.AddHandler(dragItemStart, value); }
			remove { Events.RemoveHandler(dragItemStart, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ToolboxDragItemMoveEventHandler DragItemMove {
			add { Events.AddHandler(dragItemMove, value); }
			remove { Events.RemoveHandler(dragItemMove, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ToolboxGetItemImageEventHandler GetItemImage {
			add { Events.AddHandler(getItemImage, value); }
			remove { Events.RemoveHandler(getItemImage, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ToolboxSelectedGroupChangedEventHandler SelectedGroupChanged {
			add { Events.AddHandler(selectedGroupChanged, value); }
			remove { Events.RemoveHandler(selectedGroupChanged, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ToolboxInitializeMenuEventHandler InitializeMenu {
			add { Events.AddHandler(initializeMenu, value); }
			remove { Events.RemoveHandler(initializeMenu, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ToolboxSearchTextChangedEventHandler SearchTextChanged {
			add { Events.AddHandler(searchTextChanged, value); }
			remove { Events.RemoveHandler(searchTextChanged, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ToolboxStateChangedEventHandler StateChanged {
			add { Events.AddHandler(stateChanged, value); }
			remove { Events.RemoveHandler(stateChanged, value); }
		}
		protected internal virtual void RaiseDragItemDrop(ToolboxDragItemDropEventArgs e) {
			ToolboxDragItemDropEventHandler handler = (ToolboxDragItemDropEventHandler)Events[dragItemDrop];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseItemClick(ToolboxItemClickEventArgs e) {
			ToolboxItemClickEventHandler handler = (ToolboxItemClickEventHandler)Events[itemClick];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseItemDoubleClick(ToolboxItemDoubleClickEventArgs e) {
			ToolboxItemDoubleClickEventHandler handler = (ToolboxItemDoubleClickEventHandler)Events[itemDoubleClick];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDragItemStart(ToolboxDragItemStartEventArgs e) {
			ToolboxDragItemStartEventHandler handler = (ToolboxDragItemStartEventHandler)Events[dragItemStart];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDragItemMove(ToolboxDragItemMoveEventArgs e) {
			ToolboxDragItemMoveEventHandler handler = (ToolboxDragItemMoveEventHandler)Events[dragItemMove];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseGetItemImage(ToolboxGetItemImageEventArgs e) {
			ToolboxGetItemImageEventHandler handler = (ToolboxGetItemImageEventHandler)Events[getItemImage];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseSelectedGroupChanged(ToolboxSelectedGroupChangedEventArgs e) {
			ToolboxSelectedGroupChangedEventHandler handler = (ToolboxSelectedGroupChangedEventHandler)Events[selectedGroupChanged];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseInitializeMenu(ToolboxInitializeMenuEventArgs e) {
			ToolboxInitializeMenuEventHandler handler = (ToolboxInitializeMenuEventHandler)Events[initializeMenu];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseSearchTextChanged(ToolboxSearchTextChangedEventArgs e) {
			ToolboxSearchTextChangedEventHandler handler = (ToolboxSearchTextChangedEventHandler)Events[searchTextChanged];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseStateChanged(ToolboxStateChangedEventArgs e) {
			ToolboxStateChangedEventHandler handler = (ToolboxStateChangedEventHandler)Events[stateChanged];
			if(handler != null) handler(this, e);
		}
		#endregion
		protected virtual ToolboxAppearance CreateToolboxAppearance() {
			return new ToolboxAppearance(this);
		}
		protected virtual ToolboxMenuButton CreateMenuButton() {
			return new ToolboxMenuButton(this);
		}
		protected virtual ToolboxExpandButton CreateExpandButton() {
			return new ToolboxExpandButton(this);
		}
		protected virtual ToolboxScrollButtonUp CreateScrollButtonUp() {
			return new ToolboxScrollButtonUp(this);
		}
		protected virtual ToolboxScrollButtonDown CreateScrollButtonDown() {
			return new ToolboxScrollButtonDown(this);
		}
		protected virtual ToolboxMoreItemsButton CreateMoreItemsButton() {
			return new ToolboxMoreItemsButton(this);
		}
		protected virtual ToolboxGroupCollection CreateGroupCollection() {
			return new ToolboxGroupCollection(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new ToolboxPainter();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new ToolboxViewInfo(this);
		}
		protected virtual ToolboxHandler CreateHandler() {
			return new ToolboxHandler(this);
		}
		protected virtual ToolboxScrollController CreateScrollController() {
			return new ToolboxScrollController(this);
		}
		protected virtual ToolboxDesignTimeManager CreateDesignManager() {
			return new ToolboxDesignTimeManager(this);
		}
		internal ToolboxMenuButton MenuButton {
			get { return menuButton; }
		}
		internal ToolboxExpandButton ExpandButton {
			get { return expandButton; }
		}
		internal ToolboxMoreItemsButton MoreItemsButton {
			get { return moreItemsButton; }
		}
		internal ToolboxScrollButtonUp ScrollButtonUp {
			get { return scrollButtonUp; }
		}
		internal ToolboxScrollButtonDown ScrollButtonDown {
			get { return scrollButtonDown; }
		}
		protected internal ToolboxHandler Handler {
			get { return handler; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ToolboxDesignTimeManager DesignManager {
			get { return designManager; }
		}
		[DXCategory(CategoryName.Data), RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("DevExpress.Utils.Design.DXCollectionEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)), InheritableCollection]
		public ToolboxGroupCollection Groups {
			get { return groups; }
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			ViewInfo.UpdateDefaultAppearances();
			ViewInfo.UpdateButtonsAppearance();
		}
		protected internal new virtual ToolboxViewInfo ViewInfo {
			get { return base.ViewInfo as ToolboxViewInfo; }
		}
		protected internal new bool IsRightToLeft {
			get { return base.IsRightToLeft; }
		}
		protected override Size DefaultSize {
			get { return new Size(260, 300); }
		}
		internal bool ShouldDrawOnlyItems {
			get { return shouldDrawOnlyItems; }
			set { shouldDrawOnlyItems = value; }
		}
		void ResetAppearance() {
			Appearance.Reset();
		}
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual ToolboxAppearance Appearance {
			get { return appearance; }
		}
		internal void SetAppearance(ToolboxAppearance appearance) {
			this.appearance = appearance;
		}
		#region SearchControl
		SearchControl searchControl;
		internal SearchControl SearchControl {
			get { return searchControl; }
		}
		protected virtual SearchControl CreateSearchControl() {
			SearchControl c = new SearchControl();
			Controls.Add(c);
			c.Visible = true;
			SubscribeSearchControlEvents(c);
			return c;
		}
		protected virtual void SubscribeSearchControlEvents(SearchControl c) {
			c.EditValueChanged += OnSearchControlTextChanged;
		}
		protected virtual void UnsubscribeSearchControlEvents(SearchControl c) {
			c.EditValueChanged -= OnSearchControlTextChanged;
		}
		protected virtual void OnSearchControlTextChanged(object sender, EventArgs e) {
			ToolboxSearchTextChangedEventArgs args = new ToolboxSearchTextChangedEventArgs(SearchControl.Text);
			RaiseSearchTextChanged(args);
			if(!ViewInfo.IsSearching) {
				ViewInfo.ResetSearchResult();
				return;
			}
			if(args.Handled)
				ViewInfo.ShowFoundItems(args.Result);
			else
				ViewInfo.Search(SearchControl.Text);
		}
		int isLockUpdate;
		protected internal bool IsLockUpdate {
			get { return isLockUpdate != 0; }
		}
		public void BeginUpdate() {
			isLockUpdate++;
		}
		public void EndUpdate() {
			isLockUpdate--;
			Refresh();
		}
		public override void Refresh() {
			if(IsLockUpdate) return;
			ViewInfo.Reset();
			ViewInfo.CalcViewInfo(ViewInfo.GInfo.Graphics);
			Invalidate();
		}
		#endregion
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			ViewInfo.CalcViewInfo(ViewInfo.GInfo.Graphics);
			OnPropertiesChanged();
		}
		protected virtual void OnSelectedGroupChanging() {
			ViewInfo.CreatePrevItemsBitmap();
		}
		protected virtual void OnSelectedGroupChanged() {
			ScrollController.ItemScroll.SmoothVScroll.Value = 0;
			ViewInfo.Items.Clear();
			ViewInfo.VisibleItems.Clear();
			ViewInfo.IsReady = false;
			ViewInfo.RunFadeAnimation();
			ViewInfo.Reset();
		}
		public void InvertExpanded() {
			ViewInfo.InvertMinimizedState();
			ViewInfo.RunMinimizingAnimation();
		}
		void OnGroupCollectionChanged(object sender, ListChangedEventArgs e) {
			OnLayoutChanged();
		}
		void IToolboxControl.LayoutChanged() {
			OnLayoutChanged();
		}
		protected internal virtual void OnAppearanceChanged() {
			OnLayoutChanged();
		}
		protected internal virtual void OnLayoutChanged() {
			LayoutChanged();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			Handler.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			Handler.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Handler.OnMouseMove(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			Handler.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Handler.OnMouseLeave(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			if(XtraForm.ProcessSmartMouseWheel(this, e)) return;
			Handler.OnMouseWheel(e);
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			ViewInfo.ResetParent();
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			base.OnMouseClick(e);
			Handler.OnMouseClick(e);
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			Handler.OnMouseDoubleClick(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			Handler.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			Handler.OnKeyUp(e);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			Handler.OnMouseWheel(e);
		}
		protected internal ToolboxScrollController ScrollController {
			get { return scrollController; }
		}
		protected virtual void GroupScrollBarValueChanged(object sender, EventArgs e) {
			ViewInfo.TopGroupIndent = ScrollController.GroupScroll.VScrollPosition;
		}
		protected virtual void ItemScrollBarValueChanged(object sender, EventArgs e) {
			ViewInfo.TopItemIndent = ScrollController.ItemScroll.VScrollPosition;
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			Refresh();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeOptionsEvents();
				if(this.groups != null) {
					this.groups.ListChanged -= OnGroupCollectionChanged;
					while(groups.Count > 0) {
						groups[0].Dispose();
						groups.RemoveAt(0);
					}
					groups = null;
				}
				if(ScrollController != null) {
					ScrollController.GroupScroll.VScrollValueChanged -= GroupScrollBarValueChanged;
					ScrollController.ItemScroll.VScrollValueChanged -= ItemScrollBarValueChanged;
					ScrollController.Dispose();
				}
				this.scrollController = null;
				if(searchControl != null) {
					UnsubscribeSearchControlEvents(this.searchControl);
					this.searchControl.Dispose();
				}
				searchControl = null;
			}
			base.Dispose(disposing);
		}
		protected override void WndProc(ref Message msg) {
			if(gesture.WndProc(ref msg)) return;
			base.WndProc(ref msg);
		}
		#region IGestureClient
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			ScrollController.ItemScroll.SmoothVScroll.SetValue(delta.Y);
		}
		IntPtr IGestureClient.OverPanWindowHandle {
			get { return GestureHelper.FindOverpanWindow(this); }
		}
		IntPtr IGestureClient.Handle {
			get { return Handle; }
		}
		Point IGestureClient.PointToClient(Point p) {
			return PointToClient(p);
		}
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			GestureAllowArgs p = new GestureAllowArgs() { GID = GID.PAN, AllowID = GestureHelper.GC_PAN_WITH_GUTTER | GestureHelper.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY | GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY, BlockID = GestureHelper.GC_PAN_WITH_INERTIA };
			return new GestureAllowArgs[] { p, GestureAllowArgs.PressAndTap };
		}
		void IGestureClient.OnBegin(GestureArgs info) { }
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnPressAndTap(GestureArgs info) { }
		#endregion
	}
}
