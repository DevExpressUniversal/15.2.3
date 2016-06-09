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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Tile.Drawing;
using DevExpress.XtraGrid.Views.Tile.Handler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraGrid.Views.Tile.ViewInfo {
	public class TileViewInfo : ColumnViewInfo, ITileControl {
		public TileViewInfo(BaseView view) : base(view) {
			EnsureDefaultGroup();
		}
		void EnsureDefaultGroup() {
			DefaultGroup = new TileViewGroup() { IsDefault = true };
			Groups.Add(DefaultGroup);
		}
		TileView TileView {
			get { return View as TileView; }
		}
		public override ObjectPainter FilterPanelPainter {
			get { return null; }
		}
		public override ObjectPainter ViewCaptionPainter {
			get { return null; }
		}
		public TileViewGroup DefaultGroup { get; set; }
		public override void Calc(Graphics g, Rectangle bounds) {
			if(!View.IsDisposing && !ViewInfoCore.IsReady) {
				this.bounds = bounds;
				ViewInfoCore.CalcViewInfo(bounds);
			}
		}
		public override Rectangle ViewCaptionBounds {
			get { return Rectangle.Empty; }
		}
		protected internal void UpdateFindControl() {
			this.UpdateFindControlVisibility();
		}
		protected internal Rectangle UpdateFindControl(Rectangle clientRect, bool setPosition) {
			return this.UpdateFindControlVisibility(clientRect, setPosition);
		}
		Rectangle bounds;
		public override Rectangle Bounds {
			get { return bounds; }
		}
		public override Rectangle ClientBounds {
			get { return ViewInfoCore.ClientBounds; }
		}
		protected override BaseAppearanceCollection CreatePaintAppearances() {
			return new DevExpress.XtraGrid.Views.WinExplorer.WinExplorerViewAppearances(View);
		}
		protected override BaseSelectionInfo CreateSelectionInfo() {
			return new DevExpress.XtraGrid.Views.WinExplorer.ViewInfo.WinExplorerViewSelectionInfo(View);
		}
		protected internal bool SuppressOnPropertiesChanged { get; set; }
		protected internal void OnPropertiesChanged() {
			if(View.IsLockUpdate || SuppressOnPropertiesChanged)
				return;
			ViewInfoCore.SetDirty();
			(this as ITileControl).Invalidate(ClientBounds);
		}
		public override void SetPaintAppearanceDirty() {
			base.SetPaintAppearanceDirty();
			this.appearanceItem = null;
			ViewInfoCore.ResetDefaultAppearances();
		}
		TileGroupCollection groupsCore;
		internal TileGroupCollection Groups {
			get {
				if(groupsCore == null) groupsCore = new TileGroupCollection(this);
				return groupsCore;
			}
		}
		TileViewPainterCore painterCore;
		TileViewPainterCore Painter {
			get {
				if(painterCore == null) painterCore = CreatePainter();
				return painterCore;
			}
		}
		TileControlNavigator navigatorCore;
		TileControlNavigator Navigator {
			get {
				if(navigatorCore == null) navigatorCore = CreateTileViewNavigator();
				return navigatorCore;
			}
		}
		TileViewHandlerCore handlerCore;
		TileViewHandlerCore Handler {
			get {
				if(handlerCore == null) handlerCore = CreateTileViewHandlerCore();
				return handlerCore;
			}
		}
		TileViewInfoCore vinfoCore;
		TileViewInfoCore ViewInfoCore {
			get {
				if(vinfoCore == null) vinfoCore = CreateViewInfoCore();
				return vinfoCore;
			}
		}
		protected virtual TileViewInfoCore CreateViewInfoCore() {
			return new TileViewInfoCore(this, TileView);
		}
		protected virtual TileViewHandlerCore CreateTileViewHandlerCore() {
			return new TileViewHandlerCore(this);
		}
		protected virtual TileViewNavigator CreateTileViewNavigator() {
			return new TileViewNavigator(this);
		}
		protected virtual TileViewPainterCore CreatePainter() {
			if(View.IsDesignMode || TileView.IsTemplateEditingMode)
				return new TileViewDesignTimePainter();
			return new TileViewPainterCore();
		}
		#region ITileControl Members
		ContextItemCollection ITileControl.ContextButtons { get { return TileView.ContextButtons; } }
		ContextItemCollectionOptions ITileControl.ContextButtonOptions { get { return TileView.ContextButtonOptions; } }
		void ITileControl.RaiseContextItemClick(ContextItemClickEventArgs e) { TileView.RaiseContextItemClick(e); }
		void ITileControl.RaiseContextButtonCustomize(ITileItem tileItem, ContextItem contextItem) { TileView.RaiseContextItemCustomize(tileItem, contextItem); }
		void ITileControl.RaiseCustomContextButtonToolTip(ITileItem tileItem, ContextButtonToolTipEventArgs e) { }
		TileControlHandler ITileControl.Handler {
			get { return Handler; }
		}
		TileControlNavigator ITileControl.Navigator {
			get { return Navigator; }
		}
		TileControlViewInfo ITileControl.ViewInfo {
			get { return ViewInfoCore; }
		}
		TileControlPainter ITileControl.SourcePainter {
			get { return Painter; }
		}
		void ITileControl.AddControl(System.Windows.Forms.Control control) {
			this.GridControl.Controls.Add(control);
		}
		bool ITileControl.AllowDisabledStateIndication {
			get;
			set;
		}
		bool ITileControl.AllowDrag {
			get { return false; }
		}
		bool ITileControl.AllowDragTilesBetweenGroups {
			get;
			set;
		}
		bool ITileControl.AllowGlyphSkinning {
			get;
			set;
		}
		bool ITileControl.AllowItemContentAnimation {
			get { return true; ; }
		}
		bool ITileControl.AllowSelectedItem {
			get { return false; }
		}
		bool ITileControl.AnimateArrival {
			get { return TileView.AnimateArrival; }
			set { TileView.AnimateArrival = value; }
		}
		ISupportXtraAnimation ITileControl.AnimationHost {
			get { return View as TileView; }
		}
		AppearanceObject ITileControl.AppearanceGroupText {
			get { return TileView.Appearance.GroupText; }
		}
		TileViewItemAppearances appearanceItem;
		TileItemAppearances ITileControl.AppearanceItem {
			get {
				if(appearanceItem == null)
					appearanceItem = GetAppearanceItem();
				return appearanceItem;
			}
		}
		protected virtual TileViewItemAppearances GetAppearanceItem() {
			TileViewItemAppearances result = new TileViewItemAppearances();
			result.Normal.Assign(TileView.Appearance.ItemNormal);
			result.Pressed.Assign(TileView.Appearance.ItemPressed);
			result.Selected.Assign(TileView.Appearance.ItemSelected);
			result.Hovered.Assign(TileView.Appearance.ItemHovered);
			result.Focused.Assign(TileView.Appearance.ItemFocused);
			return result;
		}
		AppearanceObject ITileControl.AppearanceText {
			get { return TileView.Appearance.ViewCaption; }
		}
		bool ITileControl.AutoSelectFocusedItem {
			get;
			set;
		}
		Color ITileControl.BackColor {
			get { return View.BackColor; }
		}
		Image ITileControl.BackgroundImage {
			get { return GridControl.BackgroundImage; }
			set { }
		}
		void ITileControl.BeginUpdate() {
			View.BeginUpdate();
		}
		XtraEditors.Controls.BorderStyles ITileControl.BorderStyle {
			get;
			set;
		}
		Rectangle ITileControl.Bounds {
			get { return Bounds; }
		}
		TileControlHitInfo ITileControl.CalcHitInfo(Point pt) {
			return ViewInfoCore.CalcHitInfo(pt);
		}
		bool ITileControl.Capture {
			get;
			set;
		}
		Rectangle ITileControl.ClientRectangle {
			get { return ClientBounds; }
		}
		System.ComponentModel.IContainer ITileControl.Container {
			get { return View.Container; }
		}
		bool ITileControl.ContainsControl(Control control) {
			return this.GridControl.Controls.Contains(control);
		}
		Control ITileControl.Control {
			get { return GridControl; }
		}
		bool ITileControl.DebuggingState {
			get { return false; }
		}
		Size ITileControl.DragSize {
			get { return new Size(1, 1); }
			set { }
		}
		bool ITileControl.EnableItemDoubleClickEvent {
			get { return true; }
			set { }
		}
		bool ITileControl.Enabled {
			get;
			set;
		}
		event TileItemDragEventHandler ITileControl.EndItemDragging {
			add { }
			remove { }
		}
		void ITileControl.EndUpdate() { View.EndUpdate(); }
		bool ITileControl.Focus() {
			return false;
		}
		TileGroupCollection ITileControl.Groups {
			get { return Groups; }
		}
		IntPtr ITileControl.Handle {
			get { return GridControl.Handle; }
		}
		object ITileControl.Images {
			get { return View.Images; }
			set { View.Images = value; }
		}
		void ITileControl.Invalidate(Rectangle rect) {
			View.InvalidateRect(rect);
		}
		bool ITileControl.IsAnimationSuspended {
			get { return false; }
		}
		bool ITileControl.IsDesignMode {
			get { return View.IsDesignMode; }
		}
		bool ITileControl.IsHandleCreated {
			get { return GridControl.IsHandleCreated; }
		}
		bool ITileControl.IsLockUpdate {
			get { return View.IsLockUpdate; }
		}
		event TileItemClickEventHandler ITileControl.ItemCheckedChanged {
			add { }
			remove { }
		}
		event TileItemClickEventHandler ITileControl.ItemClick {
			add { }
			remove { }
		}
		event TileItemClickEventHandler ITileControl.ItemDoubleClick {
			add { }
			remove { }
		}
		event TileItemClickEventHandler ITileControl.ItemPress {
			add { }
			remove { }
		}
		LookAndFeel.UserLookAndFeel ITileControl.LookAndFeel {
			get { return View.LookAndFeel; }
		}
		void ITileControl.OnItemCheckedChanged(TileItem tileItem) {
			if(TileView != null)
				TileView.OnItemCheckedChanged(tileItem);
		}
		void ITileControl.OnItemClick(TileItem tileItem) {
			if(TileView != null)
				TileView.OnItemClick(tileItem);
		}
		void ITileControl.OnItemDoubleClick(TileItem tileItem) {
			if(TileView != null)
				TileView.OnItemDoubleClick(tileItem);
		}
		void ITileControl.OnItemPress(TileItem tileItem) {
			if(TileView != null)
				TileView.OnItemPress(tileItem);
		}
		void ITileControl.OnPropertiesChanged() {
			OnPropertiesChanged();
		}
		void ITileControl.OnRightItemClick(TileItem tileItem) {
			if(TileView != null)
				TileView.OnRightItemClick(tileItem);
		}
		Point ITileControl.PointToClient(Point pt) {
			return View.PointToClient(pt);
		}
		Point ITileControl.PointToScreen(Point pt) {
			return View.PointToScreen(pt);
		}
		int ITileControl.Position {
			get { return (View as TileView).Position; }
			set { (View as TileView).Position = value; }
		}
		ITileControlProperties ITileControl.Properties {
			get { return TileView.OptionsTiles; }
		}
		TileItemDragEventArgs ITileControl.RaiseEndItemDragging(TileItem item, TileGroup targetGroup) {
			throw new NotImplementedException();
		}
		TileItemDragEventArgs ITileControl.RaiseStartItemDragging(TileItem item) {
			throw new NotImplementedException();
		}
		void ITileControl.ResumeAnimation() { }
		event TileItemClickEventHandler ITileControl.RightItemClick {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		SizeF ITileControl.ScaleFactor {
			get { 
				if(GridControl == null)
					return new SizeF(1f, 1f);
				return GridControl.ScaleFactor;
			}
		}
		ScrollBarBase ITileControl.ScrollBar {
			get { return null; }
			set { }
		}
		protected internal void OnScroll(int value) {
			Handler.OnScroll(value);
		}
		int ITileControl.ScrollButtonFadeAnimationTime {
			get { return 600; }
			set { }
		}
		TileControlScrollMode ITileControl.ScrollMode {
			get { return TileView.OptionsTiles.ScrollMode; }
			set { TileView.OptionsTiles.ScrollMode = value; }
		}
		TileItem ITileControl.SelectedItem {
			get;
			set;
		}
		event TileItemClickEventHandler ITileControl.SelectedItemChanged {
			add { }
			remove { }
		}
		Color ITileControl.SelectionColor {
			get {
				if(TileView.FocusBorderColor != Color.Empty)
					return TileView.FocusBorderColor;
				return GridSkins.GetSkin(GridControl.LookAndFeel.ActiveLookAndFeel)[GridSkins.SkinBorder].Border.All;
			}
		}
		System.ComponentModel.ISite ITileControl.Site {
			get { return View.Site; }
		}
		event TileItemDragEventHandler ITileControl.StartItemDragging {
			add { }
			remove { }
		}
		void ITileControl.SuspendAnimation() { }
		string ITileControl.Text {
			get { return View.ViewCaption; }
			set { }
		}
		void ITileControl.UpdateSmartTag() { }
		#endregion ITileControl Members
	}
	public class TileViewInfoCore : TileControlViewInfo, IAsyncImageLoaderClient {
		public TileViewInfoCore(ITileControl owner, TileView view) : base(owner) {
			this.groupsOffsetCache = new TileViewGroupsOffsetCache(this);
			this.virtInfo = new TileViewVirtualizationInfo();
			this.viewCore = view;
			this.NeedClearVisibleItems = false;
		}
		TileView viewCore;
		public TileView View { get { return viewCore; } }
		public TileViewInfo ViewInfo { get { return View.ViewInfo as TileViewInfo; } }
		TileViewNavigator Navigator { get { return Owner.Navigator as TileViewNavigator; } }
		public Dictionary<int, TileViewItem> VisibleItems = new Dictionary<int, TileViewItem>();
		public bool NeedClearVisibleItems { get; set; }
		internal int MaxRowCount { get; set; }
		bool VirtualizationInfoIsChanged { get; set; }
		bool NavigatorIsVisible { get { return View.GridControl != null && View.GridControl.UseEmbeddedNavigator; } }
		public virtual bool AllowGroups { get { return View.IsValidRowHandle(-1); } }
		protected virtual bool AllowVirtualisation { get { return true; } }
		public Color GetBackgroundColor() { return GridSkins.GetSkin(View).GetSystemColor(SystemColors.Control); }
		public void DoAnimateArrival() { this.AnimateAppearance(); }
		protected override void CreateScrollBar() { }
		protected override int ClipBoundsTopIndent { get { return 0; } }
		public override bool IsRightToLeft { get { return View.IsRightToLeft; } }
		protected override AppearanceDefault DefaultAppearanceGroupText {
			get { 
				var res = base.DefaultAppearanceGroupText;
				res.ForeColor = GridSkins.GetSkin(View).GetSystemColor(SystemColors.ControlText);
				return res;
			}
		}
		protected override TileControlLayoutCalculator GetNewLayoutCalculator(TileControlViewInfo viewInfo) {
			if(View.IsDesignMode || View.IsTemplateEditingMode)
				return base.GetNewLayoutCalculator(viewInfo);
			return new TileViewLayoutCalculator(viewInfo);
		}
		public override void CalcViewInfo(Rectangle bounds) {
			ClearVisibleItems();
			ClearItemsCache();
			GroupsOffsetCache.IsDirty = true;
			GroupsOffsetCache.CheckCache();
			base.CalcViewInfo(bounds);
			ViewInfo.UpdateFindControl();
		}
		protected virtual void ClearVisibleItems() {
			if(NeedClearVisibleItems)
				VisibleItems.Clear();
			NeedClearVisibleItems = false;
		}
		protected override Rectangle CalcClientBounds(Rectangle rect) {
			Rectangle clientBounds = base.CalcClientBounds(rect);
			UpdateBoundsByFindPanel(clientBounds);					  
			return UpdateBoundsByFindPanel(clientBounds);
		}
		Rectangle UpdateBoundsByFindPanel(Rectangle clientBounds) {
			int minTop = ViewInfo.UpdateFindControl(clientBounds, false).Y;
			clientBounds.Height -= Math.Max(0, minTop - clientBounds.Y);
			clientBounds.Y = minTop;
			return clientBounds;
		}
		protected override Rectangle ConstraintByScrollBar(Rectangle rect) {
			bool horzAffected = false;
			if(ScrollMode == TileControlScrollMode.ScrollBar && !View.ScrollInfo.IsOverlapScrollBar) {
				if(IsHorizontal) {
					if(View.ScrollInfo.HScrollVisible || NavigatorIsVisible) {
						rect.Height -= View.ScrollInfo.HScrollSize;
						horzAffected = true;
					}
				}
				else {
					if(View.ScrollInfo.VScrollVisible)
						rect.Width -= View.ScrollInfo.VScrollSize;
				}
			}
			if(NavigatorIsVisible && !horzAffected)
				rect.Height -= View.ScrollInfo.HScrollSize;
			return rect;
		}
		protected override void CalcGroupsLayoutCore() {
			MaxRowCount = GetRowCount();
			VirtualizationInfo = CalcVirtualization();
			base.CalcGroupsLayoutCore();
		}
		public virtual void UpdateScrollBar() {
			UpdateScrollParams();
		}
		protected override void RemoveScrollBar() {
			View.ScrollInfo.HScrollVisible = NavigatorIsVisible;
			View.ScrollInfo.VScrollVisible = false;
		}
		protected override void UpdateScrollParams() {
			base.UpdateScrollParams();
			if(NavigatorIsVisible) {
				View.ScrollInfo.HScrollVisible = true;
				View.ScrollInfo.HScroll.Enabled = IsHorizontal && ScrollMode == TileControlScrollMode.ScrollBar;
				View.ScrollInfo.ClientRect = ClientBounds;
				View.ScrollInfo.UpdateLookAndFeel(View.LookAndFeel);
				View.ScrollInfo.UpdateScrollRects();
			}
		}
		protected override void UpdateScrollParamsScrollBar() {
			bool prevVisibleHrz = View.ScrollInfo.HScrollVisible;
			bool prevVisibleVert = View.ScrollInfo.VScrollVisible;
			UpdateScrollInfo();
			if(prevVisibleHrz != View.ScrollInfo.HScrollVisible ||
				prevVisibleVert != View.ScrollInfo.VScrollVisible)
				CalcViewInfo(Bounds);
		}
		void UpdateScrollInfo() {
			ScrollArgs args = new ScrollArgs();
			if(IsHorizontal) {
				View.ScrollInfo.HScrollVisible = ContentBestWidth >= GroupsContentBounds.Width;
				View.ScrollInfo.VScrollVisible = false;
				args.Maximum = ContentBestWidth;
				args.LargeChange = Math.Min(GroupsContentBounds.Width, args.Maximum);
			}
			else {
				View.ScrollInfo.VScrollVisible = ContentBestHeight >= GroupsContentBounds.Height;
				View.ScrollInfo.HScrollVisible = NavigatorIsVisible;
				args.Maximum = ContentBestHeight;
				args.LargeChange = Math.Min(GroupsContentBounds.Height, args.Maximum);
			}
			args.Minimum = 0;
			args.SmallChange = Owner.Properties.ItemSize + Owner.Properties.IndentBetweenItems;
			args.Value = View.Position;
			if(IsHorizontal)
				View.ScrollInfo.HScrollArgs = args;
			else
				View.ScrollInfo.VScrollArgs = args;
			View.ScrollInfo.ClientRect = ClientBounds;
			View.ScrollInfo.UpdateLookAndFeel(View.LookAndFeel);
			View.ScrollInfo.UpdateScrollRects();
		}
		public virtual int CalcBestScreenItemsCount() {
			var itemSize = GetItemSize(GetItemType());
			if(IsHorizontal) {
				int cols = ClientBounds.Width / (itemSize.Width + IndentBetweenItems) + 2;
				return cols * MaxRowCount;
			}
			else {
				int rows = ClientBounds.Height / (itemSize.Height + IndentBetweenItems) + 2;
				return rows * GetColumnCount();
			}
		}
		public virtual int GetTopLeftItemIndex() {
			if(VirtualizationInfo.IsEmpty) return 0;
			if(AllowGroups) {
				int groupIndex = View.DataController.GetVisibleIndex(-(VirtualizationInfo.FirstVisibleGroupIndex + 1));
				return groupIndex + VirtualizationInfo.FirstVisibleItemIndex + 1;
			}
			return VirtualizationInfo.FirstVisibleItemIndex;
		}
		protected override int CalcContentBestWidth() {
			if(AllowVirtualisation) {
				if(AllowGroups) {
					EnsureGroupsOffsetCache();
					int width = GroupsOffsetCache[GroupsOffsetCache.Length - 1];
					width += CalcGroupWidth(-(GroupsOffsetCache.Length));
					return width;
				}
				else {
					int colcount = GetMaxColumnCountHorz(View.RowCount, MaxRowCount);
					return (colcount * (GetItemSize(GetItemType()).Width + IndentBetweenItems)) - IndentBetweenItems;
				}
			}
			return base.CalcContentBestWidth();
		}
		protected override int CalcContentBestHeight() {
			if(AllowVirtualisation) {
				if(AllowGroups) {
					int height = GroupsOffsetCache[GroupsOffsetCache.Length - 1];
					height += CalcGroupHeight(-(GroupsOffsetCache.Length));
					height -= Owner.Properties.IndentBetweenGroups;
					return height;
				}
				else {
					int colCount = GetColumnCount();
					int rowCount = GetMaxRowCountVert(View.RowCount, colCount);
					int height = (rowCount * (ItemSize + IndentBetweenItems)) - IndentBetweenItems;
					height += GroupTextHeight + GroupTextToItemsIndent;
					return height;
				}
			}
			return base.CalcContentBestHeight();
		}
		protected override Point GetStartPoint() {
			var result = base.GetStartPoint();
			if(IsRightToLeft) {
				result.X -= CalcFirstVisibleItemLocation().X - GroupsContentBounds.X;
			}
			return result;
		}
		protected override void OnOffsetChanged(int prevOffset) {
			int delta = Offset - prevOffset;
			if(ShouldReverseScrollDueRTL) delta = -delta;
			int xoffset = 0;
			int yoffset = 0;
			if(IsHorizontal) {
				xoffset = -delta;
				ContentLocation = new Point(ContentLocation.X - delta, ContentLocation.Y);
			}
			else {
				yoffset = -delta;
				ContentLocation = new Point(ContentLocation.X, ContentLocation.Y - delta);
			}
			VirtualizationInfo = CalcVirtualization();
			if(VirtualizationInfoIsChanged || prevOffset == 0) {
				Point firstVisibleItemLocation = VirtualizationInfoIsChanged ? VirtualizationInfo.FirstVisibleItemLocation : CalcFirstVisibleItemLocation();
				if(IsHorizontal) {
					xoffset = firstVisibleItemLocation.X - GroupsContentBounds.X;
					yoffset = 0;
				}
				else {
					yoffset = firstVisibleItemLocation.Y - GroupsContentBounds.Y;
					xoffset = 0;
				}
			}
			MakeGroupsOffset(xoffset, yoffset);
			VirtualizationInfoIsChanged = false;
			DoWindowScroll(-delta);
			XtraAnimator.Current.FrameStep(this, EventArgs.Empty);
		}
		Point CalcFirstVisibleItemLocation() {
			Point startPoint = GroupsContentBounds.Location;
			return VirtCalculator.CalcFirstVisibleItemLocation(startPoint);
		}
		protected override bool UseOptimizedScrolling {
			get {
				return false;
			}
		}
		void EnsureGroupsOffsetCache() {
			if(GroupsOffsetCache.IsEmpty)
				return;
			if(GroupsOffsetCache.RowCount != MaxRowCount) {
				GroupsOffsetCache.IsDirty = true;
				GroupsOffsetCache.CheckCache();
			}
		}
		protected virtual TileViewVirtualizationInfo CalcVirtualization() {
			EnsureGroupsOffsetCache();
			var result = new TileViewVirtualizationInfo();
			VirtCalculator.Calc(result);
			return result;
		}
		TileViewVirtCalculatorBase virtCalculator;
		TileViewVirtCalculatorBase VirtCalculator {
			get {
				if(virtCalculator == null || !CalculatorIsValid(virtCalculator))
					virtCalculator = CreateVirtCalculator();
				return virtCalculator;
			}
		}
		bool CalculatorIsValid(TileViewVirtCalculatorBase virtCalculator) {
			if(virtCalculator == null) return false;
			return AllowGroups == virtCalculator.IsGrouped;
		}
		protected internal void ResetVirtCalculator() {
			this.virtCalculator = null;
		}
		protected virtual TileViewVirtCalculatorBase CreateVirtCalculator() {
			if(IsHorizontal) {
				if(AllowGroups)
					return new TileViewVirtCalculatorHorzGrouped(this);
				return new TileViewVirtCalculatorHorz(this);
			}
			else {
				if(AllowGroups)
					return new TileViewVirtCalculatorVertGrouped(this);
				return new TileViewVirtCalculatorVert(this);
			}
		}
		protected internal int GetColumnCount() {
			int bestColumnCount = (GroupsContentBounds.Width + IndentBetweenItems) / (GetItemSize(GetItemType()).Width + IndentBetweenItems);
			int userColumnCount = Owner.Properties.ColumnCount;
			if(userColumnCount > 0) {
				var itemType = GetItemType();
				if(itemType == TileItemSize.Large || itemType == TileItemSize.Wide)
					userColumnCount = userColumnCount / 2;
				bestColumnCount = Math.Min(userColumnCount, bestColumnCount);
			}
			return Math.Max(bestColumnCount, 1);
		}
		int GetRowCount() {
			int result = AvailableRowCount;
			int userRowCount = Owner.Properties.RowCount;
			if(userRowCount > 0)
				result = Math.Min(result, userRowCount);
			return Math.Max(result, 1);
		}
		protected internal int GetMaxRowCountVert(int itemCount, int colCount) {
			switch(GetItemType()) {
				case TileItemSize.Medium: return GetMaxMediumRowCountVert(itemCount, colCount);
				case TileItemSize.Wide: return GetMaxWideRowCountVert(itemCount, colCount);
			}
			return 0;
		}
		int GetMaxWideRowCountVert(int itemCount, int colCount) {
			return GetMaxMediumRowCountVert(itemCount, colCount);
		}
		int GetMaxMediumRowCountVert(int itemCount, int colCount) {
			if(colCount == 0) return 0;
			int rowsCount = itemCount / colCount;
			if(itemCount % colCount != 0)
				rowsCount++;
			return rowsCount;
		}
		protected internal int GetMaxColumnCountHorz(int itemCount, int rowCount) {
			switch(GetItemType()) {
				case TileItemSize.Medium: return GetMaxMediumColumnCountHorz(itemCount, rowCount);
				case TileItemSize.Wide: return GetMaxWideColumnCountHorz(itemCount, rowCount);
				default: return 0;
			}
		}
		int GetMaxWideColumnCountHorz(int itemsCount, int rowCount) {
			if(rowCount == 0) return 0;
			int result = itemsCount / rowCount;
			if(itemsCount % rowCount != 0)
				result++;
			return result;
		}
		int GetMaxMediumColumnCountHorz(int itemCount, int rowCount) {
			if(rowCount == 0) return 0;
			int colCount = 0;
			int pairsCount = itemCount / 2;
			int singleItem = itemCount % 2;
			if(pairsCount <= rowCount)
				colCount = pairsCount > 0 ? 2 : 1;
			else
				colCount = (pairsCount / rowCount) * 2 + (pairsCount % rowCount == 0 ? 0 : 2);
			if(singleItem > 0 && pairsCount > 0 && pairsCount % rowCount == 0)
				colCount += singleItem;
			return colCount;
		}
		TileViewVirtualizationInfo virtInfo;
		public TileViewVirtualizationInfo VirtualizationInfo {
			get { return virtInfo; }
			set {
				if(!virtInfo.Equals(value)) {
					virtInfo = value;
					OnVirtualizationInfoChanged();
				}
			}
		}
		protected virtual void OnVirtualizationInfoChanged() {
			VirtualizationInfoIsChanged = true;
			GroupsOffsetCache.IsDirty = true;
			base.CalcGroupsLayoutCore();
		}
		protected override void LayoutGroups(bool calcFullLayout) {
			if(Calculator == null || Calculator.Groups.Count == 0) {
				Calculator = GetNewLayoutCalculator(this);
			}
			Calculator.CalcGroupsLayout(DragItem, DropTargetInfo);
			if(calcFullLayout) {
				foreach(TileControlLayoutGroup group in Calculator.Groups) {
					group.GroupInfo.LayoutGroup(Calculator, group);
				}
			}
		}
		public TileGroup DefaultGroup { 
			get { return GetDefaultGroup(); }
		}
		protected virtual TileGroup GetDefaultGroup() {
			return (Owner as TileViewInfo).DefaultGroup;
		}
		public int CalcGroupDimension(int groupRowHandle) {
			if(IsHorizontal)
				return CalcGroupWidth(groupRowHandle);
			return CalcGroupHeight(groupRowHandle);
		}
		protected virtual Size GetItemSize(TileItemSize size) {
			TileViewItem item = new TileViewItem() { ItemSize = size };
			TileViewItemInfo itemInfo = new TileViewItemInfo(item);
			return GetItemSize(itemInfo);
		}
		protected internal virtual TileItemSize GetItemType() {
			return TileItemSize.Wide;
		}
		int CalcGroupWidth(int groupRowHandle) {
			int childCount = View.DataController.GroupInfo.GetChildCount(groupRowHandle);
			int maxColumnsCount = GetMaxColumnCountHorz(childCount, MaxRowCount);
			int w = maxColumnsCount * (GetItemSize(GetItemType()).Width + IndentBetweenItems);
			w -= IndentBetweenItems;
			if(childCount > 0 && groupRowHandle < -1) w += Owner.Properties.IndentBetweenGroups;
			return w;
		}
		int CalcGroupHeight(int groupRowHandle) {
			int colCount = GetColumnCount();
			int childCount = View.DataController.GroupInfo.GetChildCount(groupRowHandle);
			int rowCount = GetMaxRowCountVert(childCount, colCount);
			int h = rowCount * (GetItemSize(GetItemType()).Height + IndentBetweenItems);
			h += GroupTextHeight + GroupTextToItemsIndent;
			h -= IndentBetweenItems;
			if(childCount > 0) h += Owner.Properties.IndentBetweenGroups;
			return h;
		}
		TileViewGroupsOffsetCache groupsOffsetCache;
		public TileViewGroupsOffsetCache GroupsOffsetCache { get { return groupsOffsetCache; } }
		protected override TileControlScrollMode DefaultScrollMode {
			get { return TileControlScrollMode.ScrollBar; }
		}
		protected internal TileControlScrollMode ScrollModeInternal {
			get { return this.ScrollMode; }
		}
		protected internal void DestroyItems(Dictionary<int, TileViewItem> dict) {
			if(dict == null) return;
			foreach(KeyValuePair<int, TileViewItem> pair in dict) {
				TileItem item = pair.Value;
				ItemViewInfoCache.Remove(item);
				foreach(TileItemElement element in item.Elements) {
					element.Image = null;
					element.Appearance.Dispose();
				}
				item.Elements.Clear();
				item.AppearanceItem.Dispose();
				item.Dispose();
			}
			dict.Clear();
		}
		protected internal void SetFocusedRowHandleCore(int rowHandle) {
			this.focusedRowHandle = rowHandle;
		}
		int focusedRowHandle = GridControl.InvalidRowHandle;
		protected internal int FocusedRowHandle {
			get { return focusedRowHandle; }
			set {
				ResetFocusedHighlightState(focusedRowHandle == value); 
				if(focusedRowHandle == value)
					return;
				int prevFocusedRowHandle = FocusedRowHandle;
				focusedRowHandle = value;
				OnFocusedRowHandleChanged(prevFocusedRowHandle, FocusedRowHandle);
			}
		}
		protected internal void ResetFocusedHighlightState(bool itemNotChanged) {
			bool highlightChanged = !View.CanHighlightFocusedItem;
			View.CanHighlightFocusedItem = true;
			bool invalidate = itemNotChanged && highlightChanged;
			if(invalidate) {
				var focusedItem = GetFocusedItem();
				UpdateItemAppearance(focusedItem);
				InvalidateItemOuterBounds(focusedItem);
			}
		}
		protected virtual void OnFocusedRowHandleChanged(int prevHandle, int newHandle) {
			View.FocusedRowHandle = FocusedRowHandle;
			var focusedItem = GetFocusedItem();
			var prevFocusedItem = GetVisibleItemByHandle(prevHandle);
			View.OnItemCustomize(focusedItem);
			UpdateItemAppearance(focusedItem);
			UpdateItemAppearance(prevFocusedItem);
			InvalidateItemOuterBounds(prevFocusedItem);
			InvalidateItemOuterBounds(focusedItem);
			if(focusedItem != null && focusedItem.ItemInfo != null) {
				_cantPressItemInfo = null;
				if(!IsBoundsContainsCore(GroupsClipBounds, focusedItem.ItemInfo)) {
					_cantPressItemInfo = focusedItem.ItemInfo;
					MakeVisible(focusedItem.ItemInfo);
				}
			}
			else {
				ScrollToItem(FocusedRowHandle);
			}
			Navigator.UpdateCurrentFocusedColumn(UseOptimizedCurrentFocusColumnUpdate);
			UseOptimizedCurrentFocusColumnUpdate = false;
		}
		void InvalidateItemOuterBounds(TileViewItem item){
			if(item == null || item.ItemInfo == null) return;
			Rectangle rect = item.ItemInfo.SelectionBounds;
			rect.Inflate(1, 1);
			Owner.Invalidate(rect);
		}
		TileItemViewInfo _cantPressItemInfo;
		protected override void AddPressItemAnimation(TileControlHitInfo hitInfo) {
			if(hitInfo.InItem && _cantPressItemInfo == hitInfo.ItemInfo)
				return;
			base.AddPressItemAnimation(hitInfo);
		}
		protected override void AddUnPressItemAnimation(TileControlHitInfo hitInfo) {
			bool canAdd = true;
			if(hitInfo.InItem && _cantPressItemInfo == hitInfo.ItemInfo) {
				canAdd = false;
			}
			_cantPressItemInfo = null;
			if(canAdd)
				base.AddUnPressItemAnimation(hitInfo);
		}
		protected internal bool UseOptimizedCurrentFocusColumnUpdate { get; set; }
		protected internal TileViewItem GetFocusedItem() {
			return GetVisibleItemByHandle(FocusedRowHandle);
		}
		TileViewItem GetVisibleItemByHandle(int handle) {
			if(VisibleItems.ContainsKey(handle))
				return VisibleItems[handle];
			return null;
		}
		void UpdateItemAppearance(TileViewItem item) {
			if(item == null || item.ItemInfo == null) return;
			item.ItemInfo.ForceUpdateAppearanceColors();
		}
		protected internal virtual void ScrollToItem(int rowHandle) {
			int endOffset = Offset;
			endOffset = CalcColumnLocation(rowHandle);
			if(endOffset > Offset) {
				endOffset = ConstrainByItemRight(endOffset);
			}
			if(Offset != endOffset)
				StartOffsetAnimation(endOffset);
		}
		protected internal void ScrollPage(int rowHandle, bool left) {
			int endOffset = Offset;
			endOffset = CalcColumnLocation(rowHandle);
			if(left) {
				endOffset = ConstrainByItemRight(endOffset);
			}
			if(Offset != endOffset)
				StartPageOffsetAnimation(endOffset);
		}
		protected internal void StartOffsetAnimation(int endOffset) {
			XtraAnimator.Current.Animations.Remove(Owner.AnimationHost, this);
			XtraAnimator.Current.AddAnimation(new TileOffsetAnimationInfo(this, OffsetAnimationLength, Offset, endOffset));
		}
		protected internal void StartPageOffsetAnimation(int endOffset) {
			XtraAnimator.Current.Animations.Remove(Owner.AnimationHost, this);
			XtraAnimator.Current.AddAnimation(new TileViewOffsetAnimationInfo(this, OffsetAnimationLength, Offset, endOffset));
		}
		protected internal void OnScrollToItemFinish() {
			if(Navigator != null)
				Navigator.OnScrollFinish();
		}
		int ConstrainByItemRight(int itemLocation) {
			if(IsHorizontal)
				itemLocation += GetItemSize(GetItemType()).Width - GroupsContentBounds.Width;
			else
				itemLocation += GetItemSize(GetItemType()).Height - GroupsContentBounds.Height;
			return itemLocation;
		}
		protected internal int CalcColumnLocation(int rowHandle) {
			if(AllowGroups)
				return CalcColumnLocationGrouped(rowHandle);
			return CalcColumnLocationNoGroup(rowHandle);
		}
		int CalcColumnLocationGrouped(int rowHandle) {
			var groupInfo = Navigator.GetGroupInfoByHandle(rowHandle);
			if(groupInfo == null || GroupsOffsetCache.IsEmpty)
				return 0;
			int indexInGroup = rowHandle - groupInfo.ChildControllerRow;
			int groupOffset = GroupsOffsetCache[groupInfo.Index];
			groupOffset += CalcColumnLocationNoGroup(indexInGroup);
			if(IsHorizontal)
				groupOffset += groupInfo.Index > 0 ? Owner.Properties.IndentBetweenGroups : 0;
			return groupOffset;
		}
		int CalcColumnLocationNoGroup(int visibleIndex) {
			if(IsHorizontal) {
				int row = visibleIndex / MaxRowCount;
				return row * (GetItemSize(GetItemType()).Width + IndentBetweenItems);
			}
			else {
				int col = visibleIndex / GetColumnCount();
				int location = col * (GetItemSize(GetItemType()).Height + IndentBetweenItems);
				location += View.OptionsTiles.ShowGroupText ? GroupTextHeight + GroupTextToItemsIndent : 0;
				return location;
			}
		}
		int GetRowHandle(TileItemViewInfo itemInfo) {
			var item = itemInfo.Item as TileViewItem;
			if(item == null) 
				return GridControl.InvalidRowHandle;
			return item.RowHandle;
		}
		protected override void PressItem(TileControlHitInfo newInfo) {
			if(newInfo.InItem) {
				FocusedRowHandle = GetRowHandle(newInfo.ItemInfo);
			}
			base.PressItem(newInfo);
		}
		protected override void UpdateNavigationGrid() {
			Navigator.UpdateNavigationGridCore();
		}
		protected override BaseAnimationInfo CreateButtonsOpacityAnimationInfo(float start, float end) {
			return new TileViewFloatAnimationInfo(XtraAnimationObject, AnimationId, Owner.ScrollButtonFadeAnimationTime, start, end);
		}
		AppearanceObject GetAppearanceEmptySpace() {
			var app = new AppearanceObject(AppearanceObject.EmptyAppearance);
			app.BackColor = GetBackgroundColor();
			if(View.Appearance.EmptySpace.ShouldSerialize())
				app.Combine(View.Appearance.EmptySpace);
			return app;
		}
		AppearanceObject appearanceEmptySpace = null;
		public AppearanceObject AppearanceEmptySpace {
			get {
				if(appearanceEmptySpace == null)
					appearanceEmptySpace = GetAppearanceEmptySpace();
				return appearanceEmptySpace;
			}
		}
		public void ResetDefaultAppearances() {
			appearanceEmptySpace = null;
			ResetItemsDefaultAppearances();
		}
		void ResetItemsDefaultAppearances() {
			foreach(TileGroupViewInfo grInfo in Groups)
				foreach(TileItemViewInfo itemInfo in grInfo.Items)
					itemInfo.ResetDefaultAppearance();
		}
		#region AsyncImageLoading
		void IAsyncImageLoaderClient.AddAnimation(ImageLoadInfo info) {
			if(Owner.Control == null || !Owner.Control.IsHandleCreated) return;
			Owner.Control.Invoke(new Action<ImageLoadInfo>(OnRunAnimation), info);
		}
		void IAsyncImageLoaderClient.ForceItemRefresh(ImageLoadInfo info) {
			if(Owner.Control == null || !Owner.Control.IsHandleCreated || View == null) return;
			int rowHandle = View.GetRowHandle(info.DataSourceIndex);
			if(VisibleItems.ContainsKey(rowHandle))
				Owner.Control.Invoke(new Action<int>(RefreshItem), rowHandle);
		}
		Image IAsyncImageLoaderClient.RaiseGetLoadingImage(GetLoadingImageEventArgs e) {
			if(View == null) return null;
			return View.RaiseGetLoadingImage(e);
		}
		ThumbnailImageEventArgs IAsyncImageLoaderClient.RaiseGetThumbnailImage(ThumbnailImageEventArgs e) {
			if(View == null) return null;
			return View.RaiseGetThumbnailImage(e);
		}
		void OnRunAnimation(ImageLoadInfo info) {
			RemoveInvisibleAnimations(info.DataSourceIndex);
			int delay = View == null || !View.OptionsImageLoad.RandomShow ? 0 : rand.Next() % 300;
			if(info.RenderImageInfo == null) return;
			XtraAnimator.Current.AddAnimation(new TileViewImageShowingAnimationInfo(this.Owner.AnimationHost, info.InfoId, info.RenderImageInfo, 1000, delay));
		}
		Random rand = new Random();
		void RemoveInvisibleAnimations(int newAnimDataSourceIndex) {
			if(View == null || !View.OptionsImageLoad.AsyncLoad) return;
			for(int i = 0; i < XtraAnimator.Current.Animations.Count; i++) {
				if(XtraAnimator.Current.Animations[i] is ImageShowingAnimationInfo) {
					var ai = XtraAnimator.Current.Animations[i] as ImageShowingAnimationInfo;
					if(ai.AnimatedObject == Owner.AnimationHost)
						RemoveInvisibleAnimationCore(ai, newAnimDataSourceIndex);
				}
			}
		}
		protected internal void RemoveInvisibleAnimationCore(ImageShowingAnimationInfo ai, int newAnimDataSourceIndex) {
			if(ai.Item == null || ai.Item.LoadInfo == null) return;
			int rowHandle = View.GetRowHandle(ai.Item.LoadInfo.DataSourceIndex);
			if(!VisibleItems.ContainsKey(rowHandle)) {
				ai.Item.LoadInfo.IsInAnimation = false;
				XtraAnimator.Current.Animations.Remove(ai);
			}
			if(ai.Item.LoadInfo.DataSourceIndex == newAnimDataSourceIndex) 
				XtraAnimator.Current.Animations.Remove(ai);
		}
		protected void RefreshItem(int rowHandle) {
			TileViewItem item;
			if(VisibleItems.TryGetValue(rowHandle, out item)) {
				if(item == null || item.ItemInfo == null) return;
				if(!item.ImageInfo.LoadingStarted) 
					(item.ItemInfo as TileViewItemInfo).ForceUpdateImageContentBounds();
				Owner.Invalidate(item.ItemInfo.Bounds);
			}
		}
		#endregion AsyncImageLoading
	}
	class TileViewOffsetAnimationInfo : TileOffsetAnimationInfo {
		public TileViewOffsetAnimationInfo(TileViewInfoCore viewInfo, int milliseconds, int start, int end)
			: base(viewInfo, milliseconds, start, end) { }
		public override void FrameStep() {
			base.FrameStep();
			if(this.IsFinalFrame)
				OnAnimationComplete();
		}
		void OnAnimationComplete() {
			var vInfo = ViewInfo as TileViewInfoCore;
			if(vInfo != null) {
				vInfo.OnScrollToItemFinish();
			}
		}
	}
	public class TileViewHitInfo : BaseHitInfo {
		public TileViewHitInfo() : base() { }
		public TileViewHitInfo(TileControlHitInfo hitInfo)
			: this() {
			this.actualHitInfo = hitInfo;
			this.ItemInfo = hitInfo.ItemInfo as TileViewItemInfo;
			this.GroupInfo = hitInfo.GroupInfo as TileViewGroupInfo;
			this.HitTest = hitInfo.HitTest;
			this.HitPoint = hitInfo.HitPoint;
		}
		TileControlHitInfo actualHitInfo;
		public TileViewItemInfo ItemInfo { get; private set; }
		public TileViewGroupInfo GroupInfo { get; private set; }
		public TileViewItem Item { get { return ItemInfo == null ? null : ItemInfo.Item as TileViewItem; } }
		public int RowHandle { get { return Item == null ? GridControl.InvalidRowHandle : Item.RowHandle; } }
		public TileControlHitTest HitTest { get; private set; }
		public bool InItem { get { return HitTest == TileControlHitTest.Item && ItemInfo != null; } }
		public bool InGroup { get { return HitTest == TileControlHitTest.Group && GroupInfo != null; } }
		public bool InBackArrow { get { return HitTest == TileControlHitTest.BackArrow; } }
		public bool InForwardArrow { get { return HitTest == TileControlHitTest.ForwardArrow; } }
		public override bool Equals(object obj) {
			TileViewHitInfo hi = obj as TileViewHitInfo;
			if(hi != null) {
				if(actualHitInfo != null) {
					return actualHitInfo.Equals(hi.actualHitInfo);
				}
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	class TileViewImageShowingAnimationInfo : ImageShowingAnimationInfo {
		public TileViewImageShowingAnimationInfo(ISupportXtraAnimation anim, object animationId, RenderImageViewInfo imageInfo, int ms, int delay)
			: base(anim, animationId, imageInfo, ms, delay) {
		}
		TileView GetView() {
			return AnimatedObject as TileView;
		}
		TileViewInfoCore GetViewInfo() {
			TileView view = GetView();
			if(view == null) return null;
			return (view.ViewInfo as ITileControl).ViewInfo as TileViewInfoCore;
		}
		TileViewItem GetItem(TileView view, TileViewInfoCore viewInfo) {
			TileViewItem item = null;
			if(viewInfo == null || view == null) return null;
			int rowHandle = view.GetRowHandle(Item.LoadInfo.DataSourceIndex);
			viewInfo.VisibleItems.TryGetValue(rowHandle, out item);
			return item;
		}
		protected override void Invalidate() {
			TileView view = GetView();
			TileViewItem item = GetItem(view, GetViewInfo());
			if(item == null || view == null)
				return;
			(view.ViewInfo as ITileControl).Invalidate(item.ItemInfo.Bounds);
		}
		protected override void OnAnimationComplete() {
			base.OnAnimationComplete();
			SetItemBackgroundImage();
		}
		void SetItemBackgroundImage() {
			TileView view = GetView();
			TileViewInfo viewInfo = view.ViewInfo as TileViewInfo;
			bool oldValue = viewInfo.SuppressOnPropertiesChanged;
			try {
				viewInfo.SuppressOnPropertiesChanged = true;
				TileViewItem item = GetItem(view, (viewInfo as ITileControl).ViewInfo as TileViewInfoCore);
				if(item == null) return;
				item.BackgroundImage = Item.LoadInfo.ThumbImage;
				item.ItemInfo.LayoutItem(item.ItemInfo.Bounds);
			}
			finally {
				viewInfo.SuppressOnPropertiesChanged = oldValue;
			}
		}
	}
	class TileViewFloatAnimationInfo : FloatAnimationInfo {
		public TileViewFloatAnimationInfo(ISupportXtraAnimation obj, object animationId, int ms, float start, float end) :
			base(obj, animationId, ms, start, end) { }
		protected override void Invalidate() {
			TileViewInfo viewInfo = this.AnimatedObject as TileViewInfo;
			if(viewInfo != null)
				(viewInfo as ITileControl).Invalidate(viewInfo.ClientBounds);
		}
	}
	public class TileViewGroupsOffsetCache {
		int[] cachedPositions;
		TileViewInfoCore viewInfo;
		public TileViewGroupsOffsetCache(TileViewInfoCore viewInfo) {
			IsDirty = true;
			this.viewInfo = viewInfo;
			this.cachedPositions = null;
		}
		public bool IsEmpty { get { return CachedPositions.Length == 0; } }
		public int Length { get { return CachedPositions.Length; } }
		public int RowCount { get; set; }
		public void CheckCache() {
			if(this.cachedPositions != null && IsDataSourceChanged()) {
				ReCreateCache();
			}
			if(IsDirty)
				InitializeCache();
		}
		public int this[int index] {
			get { return CachedPositions[index]; }
		}
		public int CalcScrollableAreaHeightGroup() {
			return CachedPositions[CachedPositions.Length - 1] + ViewInfo.CalcGroupDimension(-CachedPositions.Length);
		}
		public int CalcGroupLocationByHandle(int groupRowHandle) {
			return CachedPositions[-groupRowHandle - 1];
		}
		protected bool IsDataSourceChanged() {
			int rowsCount = ViewInfo.View.DataController.GroupRowCount;
			return this.cachedPositions.Length != rowsCount;
		}
		protected virtual void InitializeCache() {
			IsDirty = false;
			int groupRowHandle = -1, index = 1;
			int offset = 0;
			this.RowCount = ViewInfo.MaxRowCount;
			while(ViewInfo.View.IsValidRowHandle(groupRowHandle)) {
				offset += ViewInfo.CalcGroupDimension(groupRowHandle);
				if(index < CachedPositions.Length) {
					CachedPositions[index] = offset;
					index++;
				}
				groupRowHandle--;
			}
		}
		protected virtual void ReCreateCache() {
			this.cachedPositions = new int[ViewInfo.View.DataController.GroupRowCount];
			InitializeCache();
		}
		protected int[] CachedPositions {
			get {
				if(this.cachedPositions == null || IsDirty) {
					ReCreateCache();
				}
				return this.cachedPositions;
			}
		}
		public bool IsDirty { get; protected internal set; }
		public TileViewInfoCore ViewInfo { get { return viewInfo; } }
		protected internal virtual void UpdateCacheFromHandle(int groupRowHandle) {
			if(IsDirty)
				return;
			int cacheIndex = -groupRowHandle - 1;
			if(cacheIndex == CachedPositions.Length - 1)
				return;
			int groupHeight = ViewInfo.CalcGroupDimension(groupRowHandle);
			int delta = groupHeight - (CachedPositions[cacheIndex + 1] - CachedPositions[cacheIndex]);
			for(int i = cacheIndex + 1; i < CachedPositions.Length; i++) {
				CachedPositions[i] += delta;
			}
		}
	}
	public class TileViewAsyncImageLoader : AsyncImageLoader {
		protected IAsyncImageLoaderClient TileViewInfo {
			get { return ViewInfo; }
		}
		protected TileView View {
			get { return (ViewInfo as TileViewInfoCore).View; }
		}
		public TileViewAsyncImageLoader(IAsyncImageLoaderClient viewInfo)
			: base(viewInfo) {
		}
		protected override bool ShouldCacheThumbnails() {
			if(View == null) return false;
			return View.OptionsImageLoad.CacheThumbnails;
		}
		protected override bool ShouldLoadThumbnailImagesFromDataSource() {
			if(View == null) return false;
			return View.OptionsImageLoad.LoadThumbnailImagesFromDataSource;
		}
		protected override Image GetImageCore(int rowHandle) {
			if(TileViewInfo == null || View == null) return null;
			return TileViewImageLoadHelper.GetImage(View, rowHandle, View.ColumnSet.BackgroundImageColumn);
		}
		public override bool IsRowLoaded(int rowHandle) {
			if(View == null || View.DataController == null)
				return true;
			return View.DataController.IsRowLoaded(rowHandle);
		}
	}
	public class TileViewSyncImageLoader : SyncImageLoader {
		public TileViewSyncImageLoader(IAsyncImageLoaderClient viewInfo) : base(viewInfo) { }
		protected IAsyncImageLoaderClient TileViewInfo {
			get { return ViewInfo; }
		}
		protected TileView View {
			get { return (ViewInfo as TileViewInfoCore).View; } 
		}
		protected override Image GetImageCore(int rowHandle) {
			if(TileViewInfo == null || View == null) return null;
			return TileViewImageLoadHelper.GetImage(View, rowHandle, View.ColumnSet.BackgroundImageColumn);
		}
	}
	public class TileViewItemInfo : TileItemViewInfo, IAsyncImageItemViewInfo {
		public TileViewItemInfo(TileItem item) : base(item) { }
		TileView View { get { return ((TileViewInfoCore)ControlInfo).View; } }
		TileViewItem ViewItem { get { return Item as TileViewItem; } }
		protected override TileItemElementViewInfo CreateElementInfo(TileItemViewInfo itemInfo, TileItemElement elem) {
			return new TileViewElementInfo(itemInfo, elem);
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault(GetDefaultForeColor(), GetDefaultBackColor(), GetDefaultBorderColor(), Color.Empty, AppearanceObject.DefaultFont);
		}
		Color GetDefaultBorderColor() {
			return GridSkins.GetSkin(ControlInfo.Owner.LookAndFeel.ActiveLookAndFeel)[GridSkins.SkinBorder].Border.All;
		}
		Color GetDefaultBackColor() {
			return CommonSkins.GetSkin(ControlInfo.Owner.LookAndFeel.ActiveLookAndFeel).GetSystemColor(SystemColors.Window);
		}
		Color GetDefaultForeColor() {
			return CommonSkins.GetSkin(ControlInfo.Owner.LookAndFeel.ActiveLookAndFeel).GetSystemColor(SystemColors.WindowText);
		}
		public override bool AllowSelectAnimation {
			get {
				return View.OptionsTiles.AllowPressAnimation && base.AllowSelectAnimation;
			}
		}
		public bool IsFullyVisibleOnScreen { get { return GroupInfo.ControlInfo.GroupsClipBounds.Contains(Bounds); } }
		public bool IsFocused { 
			get {
				if(View == null || ViewItem == null || !View.CanHighlightFocusedItem) 
					return false;
				return View.FocusedRowHandle == ViewItem.RowHandle; 
			}
		}
		protected override AppearanceObject GetPressedAppearanceExt() { return GetFocusedAppearance(); }
		protected override AppearanceObject GetHoveredAppearanceExt() { return GetFocusedAppearance(); }
		protected override AppearanceObject GetSelectedAppearanceExt() { return GetFocusedAppearance(); }
		protected override AppearanceObject GetNormalAppearanceExt() { return GetFocusedAppearance(); }
		AppearanceObject GetFocusedAppearance() {
			if(View != null && Item != null) {
				if(IsFocused) {
					var tvItemAppearances = Item.AppearanceItem as TileViewItemAppearances;
					if(tvItemAppearances != null) {
						AppearanceObject result = new AppearanceObject();
						AppearanceObject focusedItem = tvItemAppearances.Focused;
						AppearanceHelper.Combine(result, new AppearanceObject[] { focusedItem, View.Appearance.ItemFocused });
						return result;
					}
				}
			}
			return AppearanceObject.EmptyAppearance;
		}
		Rectangle IAsyncImageItemViewInfo.ImageContentBounds {
			get {
				if(ViewItem.ImageInfo.ThumbImage != null) {
					var size = ViewItem.ImageInfo.AnimatedRegion;
					return TileItemCalculator.LayoutContent(Bounds, size, BackgroundImageAlignment, Point.Empty);
				}
				return Bounds;
			}
		}
		ImageLoadInfo IAsyncImageItemViewInfo.ImageInfo {
			get { return ViewItem == null ? null : ViewItem.ImageInfo; }
			set { }
		}
		protected internal void ForceUpdateImageContentBounds() { }
		protected virtual ImageLayoutMode GetImageLayoutMode() {
			if(!ViewItem.ImageInfo.IsLoaded && View.ImageLoader is AsyncImageLoader)
				return ImageLayoutMode.Squeeze;
			return ViewItem.GetImageLayoutMode();
		}
	}
	public class TileViewElementInfo : TileItemElementViewInfo {
		public TileViewElementInfo(TileItemViewInfo itemInfo, TileItemElement elem) : base(itemInfo, elem) { }
		static Image imgBindingField;
		static Image imgBindingImage;
		protected internal static Image ImgBindingField {
			get { 
				if(imgBindingField == null)
					imgBindingField = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraGrid.TileView.Images.DesignBindingField.png", typeof(GridControl).Assembly);
				return imgBindingField;
			}
		}
		protected internal static Image ImgBindingImage {
			get {
				if(imgBindingImage == null)
					imgBindingImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraGrid.TileView.Images.DesignBindingImage.png", typeof(GridControl).Assembly);
				return imgBindingImage;
			}
		}
		public Rectangle GetBindingIconRectangle() {
			Rectangle result = new Rectangle(Point.Empty, ImgBindingField.Size);
			result.Location = new Point(TextBounds.Right, TextBounds.Top);
			result.Y -= (int)ImgBindingField.Size.Height / 4;
			return result;
		}
		protected override TileItemContentAlignment GetImageAlignment() {
			TileItemContentAlignment res = Element.ImageAlignment != TileItemContentAlignment.Default ? Element.ImageAlignment : Item.ImageAlignment;
			if(res == TileItemContentAlignment.Default) res = Item.Control.Properties.ItemImageAlignment;
			return res == TileItemContentAlignment.Default ? GetAlignment(Item.Elements.IndexOf(Element), res) : res;
		}
		public override bool ShouldDrawImageBorder {
			get {
				bool baseRes = base.ShouldDrawImageBorder;
				if(!baseRes && Element is TileViewItemElement && (Element as TileViewItemElement).DrawAsImageField)
					return true;
				return baseRes;
			}
		}
		public override bool UseAppearanceForImageBorder {
			get {
				if(Item is TileViewTemplateItem && !base.ShouldDrawImageBorder)
					return true;
				return base.UseAppearanceForImageBorder;
			}
		}
	}
	public class TileViewGroupInfo : TileGroupViewInfo {
		public TileViewGroupInfo(TileGroup group) : base(group) { }
		TileViewVirtualizationInfo GetVirtualizationInfo() {
			var controlInfo = ControlInfo as TileViewInfoCore;
			return controlInfo == null ? null : controlInfo.VirtualizationInfo;
		}
		TileViewInfoCore ControlInfoCore { get { return ControlInfo as TileViewInfoCore; } }
		protected override void MakeTextBoundsOffset(int deltaX, int deltaY) {
			UpdateTextWidth();
			if(ControlInfoCore.IsRightToLeft)
				MakeTextBoundsOffsetCoreRTL(deltaX, deltaY);
			else
				MakeTextBoundsOffsetCore(deltaX, deltaY);
		}
		void MakeTextBoundsOffsetCore(int deltaX, int deltaY) {
			int dx = TextBounds.X + deltaX;
			int dy = TextBounds.Y + deltaY;
			int x = Math.Max(Math.Min(dx, Bounds.Left), ControlInfo.ClientBounds.Left + ControlInfo.Owner.Properties.Padding.Left);
			int textRight = x + TextBounds.Width;
			if(ControlInfo.IsHorizontal && textRight > Bounds.Right && ConstraintTextByGroupBounds)
				x += Bounds.Right - (textRight);
			TextBounds = new Rectangle(x, dy, TextBounds.Width, TextBounds.Height);
		}
		void MakeTextBoundsOffsetCoreRTL(int deltaX, int deltaY) {
			int x = Bounds.Right - TextBounds.Width;
			int y = TextBounds.Y + deltaY;
			x = Math.Min(x, ControlInfo.ClientBounds.Right - ControlInfo.Owner.Properties.Padding.Right - TextBounds.Width);
			if(x < Bounds.X)
				x = Bounds.X;
			TextBounds = new Rectangle(x, y, TextBounds.Width, TextBounds.Height);
		}
		bool ConstraintTextByGroupBounds {
			get { return ControlInfo.Groups.Count > 1; }
		}
		protected override void UpdateTextWidth() {
			if(TextBounds.Size.IsEmpty || ControlInfo.IsVertical || ConstraintTextByGroupBounds) {
				base.UpdateTextWidth();
				return;
			}
			int constraint = TextBounds.Width;
			if(ControlInfo.IsRightToLeft)
				constraint = TextBounds.Right - ControlInfo.ContentBounds.Width;
			else
				constraint = ControlInfo.ContentBounds.Width - TextBounds.X;
			TextBounds = new Rectangle(TextBounds.X, TextBounds.Y, Math.Min(TextBounds.Width, constraint), TextBounds.Height);
		}
	}
	class TileViewLayoutCalculator : TileControlLayoutCalculator {
		public TileViewLayoutCalculator(TileControlViewInfo viewInfo) : base(viewInfo) {
			ViewInfoCore.GroupsOffsetCache.CheckCache();
		}
		public TileView View { get { return (ViewInfo as TileViewInfoCore).View; } }
		public TileViewInfoCore ViewInfoCore { get { return ViewInfo as TileViewInfoCore; } }
		protected virtual bool AllowGroups { get { return View.SortInfo.GroupCount > 0; } }
		TileViewVirtualizationInfo VirtInfo { get { return ViewInfoCore.VirtualizationInfo; } }
		DevExpress.Data.BaseGridController DataController { get { return View.DataController; } }
		DevExpress.Data.GroupRowInfoCollection GroupInfo { get { return DataController.GroupInfo; } }
		Dictionary<int, TileViewItem> PrevVisibleItems;
		Dictionary<int, TileViewItem> VisibleItems {
			get { return ViewInfoCore.VisibleItems; }
		}
		protected override List<TileControlLayoutGroup> CreateLayoutInfoCore(TileItemViewInfo dragItem, TileControlDropItemInfo dropInfo) {
			List<TileControlLayoutGroup> res = new List<TileControlLayoutGroup>();
			(ViewInfoCore.Owner as TileViewInfo).SuppressOnPropertiesChanged = true;
			PrevVisibleItems = new Dictionary<int,TileViewItem>(VisibleItems);
			VisibleItems.Clear();
			if(AllowGroups) {
				if(!VirtInfo.IsEmpty)
					CreateLayoutInfoByGroups(res);
			}
			else {
				if(ViewInfoCore.DefaultGroup != null)
					CreateLayoutInfoInDefaultGroup(res);
			}
			var unusedItems = PrevVisibleItems.Except(VisibleItems).ToDictionary(x => x.Key, x => x.Value);
			ViewInfoCore.DestroyItems(unusedItems);
			(ViewInfoCore.Owner as TileViewInfo).SuppressOnPropertiesChanged = false;
			return res;
		}
		protected virtual void CreateLayoutInfoByGroups(List<TileControlLayoutGroup> layoutInfo) {
			if(ViewInfoCore.GroupsOffsetCache.Length == 0) 
				return;
			int firstVisibleGroupIndex = VirtInfo.FirstVisibleGroupIndex;
			int groupRowHandle = -(firstVisibleGroupIndex + 1);
			if(!View.IsValidRowHandle(groupRowHandle))
				return;
			TileViewGroup group = GetGroupByRowHandle(groupRowHandle);
			int itemsCount = 0;
			if(firstVisibleGroupIndex == VirtInfo.LastVisibleGroupIndex)
				itemsCount = VirtInfo.VisibleItemsCount;
			else
				itemsCount = GroupInfo.GetChildCount(groupRowHandle) - VirtInfo.FirstVisibleItemIndex;
			int firstItemIndex = DataController.GetVisibleIndexes().IndexOf(GroupInfo.GetChildRow(groupRowHandle, 0));
			firstItemIndex += VirtInfo.FirstVisibleItemIndex;
			layoutInfo.Add(CreateLayoutGroupInfo(group, itemsCount, firstItemIndex));
			groupRowHandle--;
			int groupCounter = 1;
			while(View.IsValidRowHandle(groupRowHandle) && groupCounter < VirtInfo.VisibleGroupsCount) {
				itemsCount = 0;
				group = GetGroupByRowHandle(groupRowHandle);
				if(groupCounter == VirtInfo.LastVisibleGroupIndex - VirtInfo.FirstVisibleGroupIndex) 
					itemsCount = VirtInfo.LastVisibleItemIndex + 1;
				else
					itemsCount = GroupInfo.GetChildCount(groupRowHandle);
				firstItemIndex = DataController.GetVisibleIndexes().IndexOf(GroupInfo.GetChildRow(groupRowHandle, 0));
				if(itemsCount > 0) {
					layoutInfo.Add(CreateLayoutGroupInfo(group, itemsCount, firstItemIndex));
					groupCounter++;
				}
				groupRowHandle--;
			}
		}
		protected virtual void CreateLayoutInfoInDefaultGroup(List<TileControlLayoutGroup> layoutInfo) {
			int itemsCount = VirtInfo.VisibleItemsCount;
			int firstItemIndex = VirtInfo.FirstVisibleItemIndex;
			layoutInfo.Add(CreateLayoutGroupInfo(ViewInfoCore.DefaultGroup, itemsCount, firstItemIndex));
		}
		protected virtual TileControlLayoutGroup CreateLayoutGroupInfo(TileGroup group, int itemsCount, int firstVisibleItemIndex) {
			TileControlLayoutGroup layoutInfo = new TileControlLayoutGroup() { Group = group, GroupInfo = group.GroupInfo };
			TileViewGroup tvgroup = group as TileViewGroup;
			if(tvgroup == null)
				return layoutInfo;
			int rowIndex = firstVisibleItemIndex;
			if(View.GetVisibleRowHandle(firstVisibleItemIndex) == DevExpress.Data.DataController.InvalidRow)
				rowIndex = DevExpress.Data.DataController.InvalidRow;
			tvgroup.Items.Clear();
			int itemCounter = 0;
			while(rowIndex != DevExpress.Data.DataController.InvalidRow && itemCounter < itemsCount) {
				int rowHandle = View.GetVisibleRowHandle(rowIndex);
				if(!DataController.IsGroupRowHandle(rowHandle)) {
					TileViewItem item = GetItemByRowHandle(rowHandle, tvgroup);
					VisibleItems.Add(rowHandle, item);
					layoutInfo.Items.Add(GetNewLayoutItem(item, item.ItemInfo));
					itemCounter++;
				}
				rowIndex = View.GetNextVisibleRow(rowIndex);
			}
			return layoutInfo;
		}
		protected virtual TileViewGroup GetGroupByRowHandle(int groupRowHandle) {
			TileViewGroup result = null;
			foreach(TileViewGroup group in ViewInfoCore.Owner.Groups) {
				if(group.RowHandle == groupRowHandle) {
					result = group;
					break;
				}
			}
			if(result == null) {
				result = new TileViewGroup();
				result.RowHandle = groupRowHandle;
				ViewInfoCore.Owner.Groups.Add(result);
			}
			UpdateGroup(result, groupRowHandle);
			return result;
		}
		void UpdateGroup(TileViewGroup group, int groupRowHandle) {
			string groupCaption = string.Empty;
			if(View.GroupColumn != null)
				groupCaption = View.GroupColumn.OptionsColumn.ShowCaption ? View.GroupColumn.GetCaption() + ": " : string.Empty;
			groupCaption += View.GetGroupRowDisplayText(groupRowHandle);
			group.Text = groupCaption;
		}
		protected virtual TileViewItem GetItemByRowHandle(int rowHandle, TileViewGroup group) {
			TileViewItem item;
			if(PrevVisibleItems.TryGetValue(rowHandle, out item))
				return item;
			TileViewItem newItem = View.CreateItem();
			View.UpdateItem(newItem, rowHandle);								
			group.Items.Add(newItem);
			return newItem;
		}
		protected override TileControlLayoutItem GetNewLayoutItemCore(Rectangle bounds, TileItem item, TileItemViewInfo itemInfo, TileItemPosition position) {
			return new TileViewLayoutItem() { Bounds = bounds, Item = item, ItemInfo = itemInfo, Position = position };
		}
	}
	class TileViewLayoutItem : TileControlLayoutItem {
		protected override Point CalcLocation(TileGroupLayoutInfo info) {
			if(!ControlInfo.IsRightToLeft) {
				if(ItemInfo.IsLarge) return info.Location;
				return new Point(info.Location.X + info.ItemPosition.Column * (ControlInfo.ItemSize + ControlInfo.IndentBetweenItems), info.Location.Y);
			}
			else {
				if(ItemInfo.IsLarge) return new Point(info.Location.X - (ControlInfo.GetItemSize(ItemInfo).Width), info.Location.Y);
				int xpos = info.Location.X - ControlInfo.ItemSize;
				return new Point(xpos - info.ItemPosition.Column * (ControlInfo.ItemSize + ControlInfo.IndentBetweenItems), info.Location.Y);
			}
		}
	}
	public abstract class TileViewVirtCalculatorBase {
		public TileViewVirtCalculatorBase(TileViewInfoCore viewInfoCore) {
			this.ViewInfoCore = viewInfoCore;
		}
		public abstract bool IsGrouped { get; }
		protected TileViewInfoCore ViewInfoCore { get; private set; }
		protected TileView View { get { return ViewInfoCore.View; } }
		protected Rectangle ClientBounds { get { return ViewInfoCore.ClientBounds; } }
		protected Point Start { get { return ViewInfoCore.GroupsContentBounds.Location; } }
		protected int Offset { get { return ViewInfoCore.Offset; } }
		protected int MaxRowCount { get { return ViewInfoCore.MaxRowCount; } }
		protected int IndentBetweenItems { get { return ViewInfoCore.IndentBetweenItems; } }
		protected int IndentBetweenGroups { get { return ViewInfoCore.Owner.Properties.IndentBetweenGroups; } }
		protected int ItemSize { get { return ViewInfoCore.ItemSize; } }
		protected TileViewGroupsOffsetCache GroupsOffsetCache { get { return ViewInfoCore.GroupsOffsetCache; } }
		[DebuggerStepThrough]
		protected Size GetItemSize(TileItemViewInfo itemInfo) { return ViewInfoCore.GetItemSize(itemInfo); }
		[DebuggerStepThrough]
		protected TileItemSize GetItemType() { return ViewInfoCore.GetItemType(); }
		public abstract void Calc(TileViewVirtualizationInfo vInfo);
		protected virtual Size GetItemSize(TileItemSize size) {
			TileViewItem item = new TileViewItem() { ItemSize = size };
			TileViewItemInfo itemInfo = new TileViewItemInfo(item);
			return GetItemSize(itemInfo);
		}
		protected internal virtual int CalcFirstVisibleGroupIndex(int position) {
			var cache = GroupsOffsetCache;
			if(cache.IsEmpty)
				return 0;
			int start = 0, end = cache.Length - 1;
			while(end - start > 4) {
				int middle = start + (end - start) / 2;
				if(cache[middle] > position)
					end = middle;
				else
					start = middle;
			}
			for(int i = start; i <= end && i < cache.Length - 1; i++) {
				if(cache[i] <= position && cache[i + 1] > position)
					return i;
			}
			if(position >= cache[cache.Length - 1])
				return cache.Length - 1;
			return 0;
		}
		public virtual Point CalcFirstVisibleItemLocation(Point startPoint) { return startPoint; }
	}
	public class TileViewVirtCalculatorHorz : TileViewVirtCalculatorBase {
		public TileViewVirtCalculatorHorz(TileViewInfoCore viewInfoCore) : base(viewInfoCore) { }
		public override bool IsGrouped { get { return false; } }
		public override void Calc(TileViewVirtualizationInfo vInfo) {
			int itemCount = View.RowCount;
			if(itemCount == 0) return;
			int maxColumnCount = ViewInfoCore.GetMaxColumnCountHorz(itemCount, MaxRowCount);
			int firstVisibleColumnIndex = GetFirstVisibleColumnIndex(Offset - Start.X);
			int lastVisibleColumnIndex = GetLastMaxVisibleColumnIndex(Offset + ClientBounds.Width - Start.X);
			lastVisibleColumnIndex = Math.Min(maxColumnCount - 1, lastVisibleColumnIndex);
			vInfo.FirstVisibleItemIndex = GetItemIndexByColumnIndex(firstVisibleColumnIndex, itemCount, true);
			vInfo.LastVisibleItemIndex = GetItemIndexByColumnIndex(lastVisibleColumnIndex, itemCount, false);
			vInfo.FirstVisibleItemLocation = GetItemLocation(firstVisibleColumnIndex, Start, Offset, GetItemType());
		}
		int GetLastMaxVisibleColumnIndex(int offset) {
			return offset / (GetItemSize(GetItemType()).Width + IndentBetweenItems);
		}
		protected internal int GetFirstVisibleColumnIndex(int offset) {
			switch(GetItemType()) {
				case TileItemSize.Medium: return GetFirstVisibleMediumColumnIndex(offset);
				case TileItemSize.Wide: return GetFirstVisibleWideColumnIndex(offset);
			}
			return 0;
		}
		int GetFirstVisibleWideColumnIndex(int offset) {
			return offset / (GetItemSize(TileItemSize.Wide).Width + IndentBetweenItems);
		}
		int GetFirstVisibleMediumColumnIndex(int offset) {
			int result = offset / (GetItemSize(TileItemSize.Medium).Width + IndentBetweenItems);
			if((result + 1) % 2 == 0)
				result = Math.Max(0, result - 1);
			return result;
		}
		protected int GetItemIndexByColumnIndex(int columnIndex, int itemsCount, bool top) {
			switch(GetItemType()) {
				case TileItemSize.Medium: return GetMedulmItemIndexByColumnIndex(columnIndex, itemsCount, top);
				case TileItemSize.Wide: return GetWideItemIndexByColumnIndex(columnIndex, itemsCount, top);
			}
			return 0;
		}
		int GetWideItemIndexByColumnIndex(int columnIndex, int itemsCount, bool top) {
			int itemIndex = 0;
			if(columnIndex > 0)
				itemIndex = MaxRowCount * columnIndex;
			if(!top) {
				itemIndex += MaxRowCount - 1;
				while(itemIndex > itemsCount - 1)
					itemIndex--;
			}
			return itemIndex;
		}
		int GetMedulmItemIndexByColumnIndex(int columnIndex, int itemsCount, bool top) {
			int itemIndex = 0;
			bool evenColumn = (columnIndex + 1) % 2 == 0;
			if(columnIndex > 0) {
				if(evenColumn) {
					itemIndex = ((columnIndex - 1) * MaxRowCount) + 1;
				}
				else {
					itemIndex = columnIndex * MaxRowCount;
				}
			}
			if(!top) {
				itemIndex += (MaxRowCount - 1) * 2;
				while(itemIndex > itemsCount - 1) {
					itemIndex--;
					if(!evenColumn && (itemIndex + 1) % 2 == 0)
						itemIndex--;
				}
			}
			return itemIndex;
		}
		protected Point GetItemLocation(int columnIndex, Point start, int offset, TileItemSize itemType) {
			if(columnIndex < 0)
				return start;
			Point result = start;
			result.X += columnIndex * (GetItemSize(itemType).Width + IndentBetweenItems);
			result.X -= offset;
			return result;
		}
		public override Point CalcFirstVisibleItemLocation(Point startPoint) {
			int leftColumnIndex = GetFirstVisibleColumnIndex(Offset - startPoint.X);
			return GetItemLocation(leftColumnIndex, startPoint, Offset, GetItemType());
		}
	}
	public class TileViewVirtCalculatorHorzGrouped : TileViewVirtCalculatorHorz {
		public TileViewVirtCalculatorHorzGrouped(TileViewInfoCore viewInfoCore) : base(viewInfoCore) { }
		public override bool IsGrouped { get { return true; } }
		public override void Calc(TileViewVirtualizationInfo vInfo) {
			if(GroupsOffsetCache.IsEmpty) return;
			int firstVisibleGroupIndex = CalcFirstVisibleGroupIndex(Offset - Start.X);
			int itemCount = View.DataController.GroupInfo.GetChildCount(-(firstVisibleGroupIndex + 1));
			int groupOffset = GroupsOffsetCache[firstVisibleGroupIndex];
			if(firstVisibleGroupIndex > 0) {
				groupOffset += IndentBetweenGroups;
			}
			int firstVisibleColumnIndex = GetFirstVisibleColumnIndex(Offset - groupOffset - Start.X);
			vInfo.FirstVisibleGroupIndex = firstVisibleGroupIndex;
			vInfo.FirstVisibleItemIndex = GetItemIndexByColumnIndex(firstVisibleColumnIndex, itemCount, true);
			vInfo.FirstVisibleItemLocation = GetItemLocation(firstVisibleColumnIndex, Start, Offset - groupOffset, GetItemType());
			int lastItemOffset = GetItemSize(GetItemType()).Width + IndentBetweenItems;
			int lastVisibleGroupIndex = CalcFirstVisibleGroupIndex(Offset + ClientBounds.Width + lastItemOffset - Start.X);
			lastVisibleGroupIndex = Math.Min(View.DataController.GroupInfo.GetTotalGroupsCountByLevel(0) - 1, lastVisibleGroupIndex);
			itemCount = View.DataController.GroupInfo.GetChildCount(-(lastVisibleGroupIndex + 1));
			groupOffset = GroupsOffsetCache[lastVisibleGroupIndex];
			if(lastVisibleGroupIndex > 0)
				groupOffset += IndentBetweenGroups;
			int lastVisibleColumnIndex = GetFirstVisibleColumnIndex(Offset + ClientBounds.Width + lastItemOffset - groupOffset - Start.X);
			int lastVisibleItemIndex = GetItemIndexByColumnIndex(lastVisibleColumnIndex, itemCount, false);
			vInfo.LastVisibleGroupIndex = lastVisibleGroupIndex;
			vInfo.LastVisibleItemIndex = lastVisibleItemIndex;
		}
		public override Point CalcFirstVisibleItemLocation(Point startPoint) {
			if(GroupsOffsetCache.IsEmpty) return Point.Empty;
			int leftGroupIndex = CalcFirstVisibleGroupIndex(Offset - startPoint.X);
			int groupOffset = GroupsOffsetCache[leftGroupIndex];
			if(leftGroupIndex > 0) groupOffset += IndentBetweenGroups;
			int leftColumnIndex = GetFirstVisibleColumnIndex(Offset - groupOffset - startPoint.X);
			return GetItemLocation(leftColumnIndex, startPoint, Offset - groupOffset, GetItemType());
		}
	}
	public class TileViewVirtCalculatorVert : TileViewVirtCalculatorBase {
		public TileViewVirtCalculatorVert(TileViewInfoCore viewInfoCore) : base(viewInfoCore) { }
		public override bool IsGrouped { get { return false; } }
		protected int GroupTextHeight { get { return ViewInfoCore.GroupTextHeight; } }
		protected int GroupTextToItemsIndent { get { return ViewInfoCore.GroupTextToItemsIndent; } }
		public override void Calc(TileViewVirtualizationInfo vInfo) {
			int columnCount = ViewInfoCore.GetColumnCount();
			int itemCount = View.RowCount;
			int maxRowCount = ViewInfoCore.GetMaxRowCountVert(itemCount, columnCount);
			int groupTextOffset = GroupTextHeight + GroupTextToItemsIndent;
			int firstVisibleRowIndex = GetRowIndexByOffset(Offset - Start.Y - groupTextOffset);
			int lastVisibleRowIndex = GetRowIndexByOffset(Offset + ClientBounds.Bottom - Start.Y - groupTextOffset);
			lastVisibleRowIndex = Math.Min(maxRowCount - 1, lastVisibleRowIndex);
			vInfo.FirstVisibleItemIndex = GetItemIndexByRowIndex(firstVisibleRowIndex, itemCount, columnCount, true);
			vInfo.LastVisibleItemIndex = GetItemIndexByRowIndex(lastVisibleRowIndex, itemCount, columnCount, false);
			vInfo.FirstVisibleItemLocation = GetItemLocation(firstVisibleRowIndex, Start, Offset, GetItemType());
		}
		protected int GetRowIndexByOffset(int offset) {
			return offset / (GetItemSize(GetItemType()).Height + IndentBetweenItems);
		}
		protected int GetItemIndexByRowIndex(int rowIndex, int itemsCount, int columnCount, bool first) {
			int itemIndex = 0;
			if(rowIndex > 0) {
				itemIndex = rowIndex * columnCount;
				itemIndex = Math.Min(itemsCount - 1, itemIndex);
			}
			if(!first) {
				itemIndex += columnCount - 1;
				while(itemIndex > itemsCount - 1)
					itemIndex--;
			}
			return itemIndex;
		}
		protected Point GetItemLocation(int rowIndex, Point start, int offset, TileItemSize itemType) {
			if(rowIndex < 0)
				return start;
			Point result = start;
			result.Y += rowIndex * (GetItemSize(itemType).Height + IndentBetweenItems);
			result.Y -= offset;
			return result;
		}
		public override Point CalcFirstVisibleItemLocation(Point startPoint) {
			int groupTextOffset = GroupTextHeight + GroupTextToItemsIndent;
			int topRowIndex = GetRowIndexByOffset(Offset - startPoint.Y - groupTextOffset);
			return GetItemLocation(topRowIndex, startPoint, Offset, GetItemType());
		}
	}
	public class TileViewVirtCalculatorVertGrouped : TileViewVirtCalculatorVert {
		public TileViewVirtCalculatorVertGrouped(TileViewInfoCore tvic) : base(tvic) { }
		public override bool IsGrouped { get { return true; } }
		public override void Calc(TileViewVirtualizationInfo vInfo) {
			if(GroupsOffsetCache.IsEmpty) return;
			int columnCount = ViewInfoCore.GetColumnCount();
			int firstVisibleGroupIndex = CalcFirstVisibleGroupIndex(Offset);
			int itemsCount = View.DataController.GroupInfo.GetChildCount(-(firstVisibleGroupIndex + 1));
			int groupOffset = GroupsOffsetCache[firstVisibleGroupIndex];
			int groupTextOffset = GroupTextHeight + GroupTextToItemsIndent;
			vInfo.FirstVisibleGroupIndex = firstVisibleGroupIndex;
			int firstVisibleRowIndex = (Offset - groupOffset - groupTextOffset) / (ItemSize + IndentBetweenItems);
			int maxRowCount = Math.Max(1, itemsCount / columnCount);
			firstVisibleRowIndex = Math.Min(firstVisibleRowIndex, maxRowCount - 1);
			vInfo.FirstVisibleItemIndex = GetItemIndexByRowIndex(firstVisibleRowIndex, itemsCount, columnCount, true);
			vInfo.FirstVisibleItemLocation = GetItemLocation(firstVisibleRowIndex, Start, Offset - groupOffset, GetItemType());
			int lastItemOffset = GetItemSize(GetItemType()).Height + IndentBetweenItems;
			int lastVisibleGroupIndex = CalcFirstVisibleGroupIndex(Offset + ClientBounds.Bottom + lastItemOffset - Start.Y);
			lastVisibleGroupIndex = Math.Min(View.DataController.GroupInfo.GetTotalGroupsCountByLevel(0) - 1, lastVisibleGroupIndex);
			itemsCount = View.DataController.GroupInfo.GetChildCount(-(lastVisibleGroupIndex + 1));
			groupOffset = GroupsOffsetCache[lastVisibleGroupIndex];
			if(lastVisibleGroupIndex > 0)
				groupOffset += IndentBetweenGroups;
			int lastVisibleRowIndex = GetRowIndexByOffset(Offset + ClientBounds.Bottom + lastItemOffset - groupOffset - Start.Y);
			int lastVisibleItemIndex = GetItemIndexByRowIndex(lastVisibleRowIndex, itemsCount, columnCount, false);
			vInfo.LastVisibleGroupIndex = lastVisibleGroupIndex;
			vInfo.LastVisibleItemIndex = lastVisibleItemIndex;
		}
		public override Point CalcFirstVisibleItemLocation(Point startPoint) {
			if(GroupsOffsetCache.IsEmpty) return Point.Empty;
			int topGroupIndex = CalcFirstVisibleGroupIndex(Offset);
			int itemsCount = View.DataController.GroupInfo.GetChildCount(-(topGroupIndex + 1));
			int groupOffset = GroupsOffsetCache[topGroupIndex];
			int groupTextOffset = GroupTextHeight + GroupTextToItemsIndent;
			int topRowIndex = (Offset - groupOffset - groupTextOffset) / (ItemSize + IndentBetweenItems);
			int maxRowCount = Math.Max(1, itemsCount / ViewInfoCore.GetColumnCount());
			topRowIndex = Math.Min(topRowIndex, maxRowCount - 1);
			return GetItemLocation(topRowIndex, startPoint, Offset - groupOffset, GetItemType());
		}
	}
	public class TileViewVirtualizationInfo {
		public TileViewVirtualizationInfo() {
			FirstVisibleItemIndex = -1; LastVisibleItemIndex = -1;
			FirstVisibleGroupIndex = -1; LastVisibleGroupIndex = -1;
			FirstVisibleItemLocation = Point.Empty;
		}
		public int FirstVisibleGroupIndex { get; set; }
		public int LastVisibleGroupIndex { get; set; }
		public int FirstVisibleItemIndex { get; set; }
		public int LastVisibleItemIndex { get; set; }
		public Point FirstVisibleItemLocation { get; set; }
		public int VisibleItemsCount {
			get { return Math.Max(0, LastVisibleItemIndex - FirstVisibleItemIndex + 1); }
		}
		public int VisibleGroupsCount {
			get { return Math.Max(0, LastVisibleGroupIndex - FirstVisibleGroupIndex + 1); }
		}
		public bool IsEmpty {
			get {
				return FirstVisibleItemIndex == -1 &&
					LastVisibleItemIndex == -1 &&
					FirstVisibleGroupIndex == -1 &&
					LastVisibleGroupIndex == -1 &&
					FirstVisibleItemLocation.IsEmpty;
			}
		}
		public override bool Equals(object obj) {
			TileViewVirtualizationInfo vi = obj as TileViewVirtualizationInfo;
			if(vi == null) return false;
			return
				this.FirstVisibleItemIndex == vi.FirstVisibleItemIndex &&
				this.LastVisibleItemIndex == vi.LastVisibleItemIndex &&
				this.FirstVisibleGroupIndex == vi.FirstVisibleGroupIndex &&
				this.LastVisibleGroupIndex == vi.LastVisibleGroupIndex;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
