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

using System.Collections.ObjectModel;
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.WinExplorer.Handler;
using DevExpress.Utils.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using DevExpress.Utils.Text;
using System.Diagnostics;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using System.IO;
using System.Threading;
using DevExpress.XtraDataLayout;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.WinExplorer;
namespace DevExpress.XtraGrid.Views.WinExplorer.ViewInfo {
	public enum DescriptionLocation { BelowText, Right }
	public class WinExplorerItemAnimationInfo : CustomAnimationInfo {
		WinExplorerItemViewInfo itemInfo;
		public WinExplorerItemAnimationInfo(ISupportXtraAnimation obj, WinExplorerItemViewInfo item, CustomAnimationInvoker method)
			: base(obj, item, (item as IAnimatedItem).AnimationIntervals, (item as IAnimatedItem).FramesCount, method) {
			this.itemInfo = item;
			this.AnimationType = AnimationType.Cycle;
		}
		public WinExplorerItemViewInfo ItemInfo { get { return itemInfo; } }
		public GridRow Row { get { return ItemInfo.Row; } }
	}
	public abstract class WinExplorerViewInfoLayoutCalculatorBase {
		GridRowCollection visibleRows;
		WinExplorerItemInfoCollection visibleItems;
		WinExplorerViewScrollingCache scrollingCache;
		public WinExplorerViewInfoLayoutCalculatorBase(WinExplorerViewInfo viewInfo) {
			ViewInfo = viewInfo;
			this.scrollingCache = CreateScrollingCache();
		}
		protected internal WinExplorerViewScrollingCache ScrollingCache { get { return scrollingCache; } }
		public WinExplorerViewInfo ViewInfo { get; private set; }
		public WinExplorerView View { get { return ViewInfo.WinExplorerView; } }
		public abstract bool SupportItemType(WinExplorerViewStyle itemType);
		protected virtual WinExplorerViewScrollingCache CreateScrollingCache() {
			return new WinExplorerViewScrollingCache(ViewInfo);
		}
		#region Animation
		protected virtual void RemoveInvisibleAnimatedItems() {
			for(int i = 0; i < XtraAnimator.Current.Animations.Count; i++) {
				WinExplorerItemAnimationInfo info = XtraAnimator.Current.Animations[i] as WinExplorerItemAnimationInfo;
				if(info == null) continue;
				if(!info.ItemInfo.IsVisible || !ContainsItem(info.Row.RowHandle))
					RemoveAnimatedItem(info);
			}
		}
		protected virtual bool ContainsItem(int rowHandle) {
			foreach(WinExplorerItemViewInfo itemInfo in ViewInfo.VisibleItems) {
				if(itemInfo.Row.RowHandle == rowHandle)
					return true;
			}
			return false;
		}
		protected virtual void RemoveAnimatedItem(WinExplorerItemAnimationInfo info) {
			XtraAnimator.RemoveObject(info.AnimatedObject, info.AnimationId);
		}
		protected virtual bool ShouldAddAnimatedItem(WinExplorerItemViewInfo info) {
			if(!info.IsVisible) return false;
			IAnimatedItem item = info as IAnimatedItem;
			if(!item.IsAnimated || IsLoadingAnimation(info)) return false;
			return XtraAnimator.Current.Get(ViewInfo, info) == null;
		}
		protected bool IsLoadingAnimation(WinExplorerItemViewInfo info) {
			if(View == null || !View.OptionsImageLoad.AsyncLoad) return false;
			return (!info.ImageInfo.IsLoaded || info.ImageInfo.ImageSize == Size.Empty) && info.ImageInfo.LoadingStarted;
		}
		protected virtual void AddAnimatedItems() {
			foreach(WinExplorerItemViewInfo item in ViewInfo.VisibleItems) {
				if(!ShouldAddAnimatedItem(item)) continue;
				XtraAnimator.Current.AddAnimation(new WinExplorerItemAnimationInfo(ViewInfo, item, AnimationInvoker));
			}
		}
		CustomAnimationInvoker animationInvoker = null;
		internal CustomAnimationInvoker AnimationInvoker {
			get {
				if(animationInvoker == null) animationInvoker = new CustomAnimationInvoker(OnAnimation);
				return animationInvoker;
			}
		}
		protected internal virtual void ClearAnimatedItems() {
			XtraAnimator.RemoveObject(ViewInfo);
		}
		protected virtual void UpdateAimatedItems() {
			RemoveInvisibleAnimatedItems();
			AddAnimatedItems();
		}
		protected virtual void OnAnimation(BaseAnimationInfo info) {
			if(!View.OptionsImageLoad.AsyncLoad) return;
			WinExplorerItemAnimationInfo itemInfo = info as WinExplorerItemAnimationInfo;
			if(itemInfo == null || ViewInfo.GridControl == null) return;
			IAnimatedItem item = itemInfo.ItemInfo as IAnimatedItem;
			if(item == null) return;
			item.UpdateAnimation(info);
			ViewInfo.GridControl.Invalidate(item.AnimationBounds);
		}
		#endregion
		public virtual int CalcPositionDelta(int mouseWheelDelta) {
			return 0;
		}
		protected int GetRowCount(int itemsCount, int itemsInRow) {
			if(itemsInRow == 0)
				return 0;
			int rowCount = itemsCount / itemsInRow;
			if(itemsCount % itemsInRow > 0)
				rowCount++;
			return rowCount;
		}
		protected internal virtual int CalcTopVisibleGroupIndex(int position) {
			if(ScrollingCache.IsEmpty)
				return 0;
			int start = 0, end = ScrollingCache.Length - 1;
			while(end - start > 4) {
				int middle = start + (end - start) / 2;
				if(ScrollingCache[middle] > position)
					end = middle;
				else
					start = middle;
			}
			for(int i = start; i <= end && i < ScrollingCache.Length - 1; i++) {
				if(ScrollingCache[i] <= position && ScrollingCache[i + 1] > position)
					return i;
			}
			if(position >= ScrollingCache[ScrollingCache.Length - 1])
				return ScrollingCache.Length - 1;
			return 0;
		}
		protected abstract int CalcGroupPrimarySize(int rowHandle);
		protected virtual WinExplorerItemViewInfo GetItemInfo(WinExplorerItemInfoCollection cachedItems, GridRow row) {
			WinExplorerItemViewInfo itemInfo = GetItemInfoFromCache(cachedItems, row.RowHandle);
			if(itemInfo == null)
				itemInfo = CreateItemInfo(row);
			itemInfo.Row = row;
			return itemInfo;
		}
		protected internal virtual void InvalidateCache() {
			ScrollingCache.IsDirty = true;
		}
		protected internal virtual void CheckCache() {
			ScrollingCache.CheckCache();
		}
		public GridRowCollection VisibleRows {
			get {
				if(visibleRows == null)
					visibleRows = new GridRowCollection();
				return visibleRows;
			}
		}
		public WinExplorerItemInfoCollection VisibleItems {
			get {
				if(visibleItems == null)
					visibleItems = CreateItemInfoCollection();
				return visibleItems;
			}
		}
		protected internal virtual int CalcRowCountInSpace(int space) {
			return 0;
		}
		protected virtual WinExplorerItemInfoCollection CreateItemInfoCollection() {
			return new WinExplorerItemInfoCollection();
		}
		public virtual Rectangle SelectionVisualRect { get { return Rectangle.Empty; } }
		protected internal abstract int CalcTopLeftIconIndexByPosition(int position);
		public virtual void CalcIconDrawInfo(int position) {
			int topLeftIconIndex = CalcTopLeftIconIndexByPosition(position);
			if(ViewInfo.AvailableColumnCount == 0 || View.IsDisposing)
				return;
			WinExplorerItemInfoCollection cachedItems = VisibleItems;
			ClearVisibleItemsData();
			int rowIndex = topLeftIconIndex;
			if(View.GetVisibleRowHandle(topLeftIconIndex) == DevExpress.Data.DataController.InvalidRow)
				rowIndex = DevExpress.Data.DataController.InvalidRow;
			Point startLocation = CalcFirstItemLocation(position);
			CalcIconDrawInfo(cachedItems, rowIndex, startLocation);
			RemoveAnimations(cachedItems);
			AddAnimatedItems();
		}
		protected internal virtual void RemoveAnimations() {
			XtraAnimator.Current.Animations.Remove(ViewInfo);
		}
		protected internal virtual void RemoveAnimations(WinExplorerItemInfoCollection cachedItems) {
			foreach(WinExplorerItemViewInfo itemInfo in cachedItems) {
				XtraAnimator.Current.Animations.Remove(ViewInfo, itemInfo.Row);
			}
		}
		protected abstract Point CalcFirstItemLocation(int position);
		protected virtual Point GetTopLeftCoor() {
			return ViewInfo.ContentBounds.Location;
		}
		protected virtual int CalcTopVisibleRowScreenLocation(int position) {
			return ViewInfo.CalcTopVisibleRowScreenLocation(position);
		}
		protected virtual void ClearVisibleItemsData() {
			this.visibleRows = null;
			this.visibleItems = null;
			ViewInfo.BreakAsyncLoading();
		}
		protected internal virtual bool ClearItemsCache { get; set; }
		protected virtual WinExplorerItemViewInfo GetItemInfoFromCache(WinExplorerItemInfoCollection cachedItems, int rowHandle) {
			if(ClearItemsCache || cachedItems == null)
				return null;
			foreach(WinExplorerItemViewInfo itemInfo in cachedItems) {
				if(itemInfo.Row.RowHandle == rowHandle) {
					cachedItems.Remove(itemInfo);
					return itemInfo;
				}
			}
			return null;
		}
		protected virtual int PrimaryItemIndent { get { return ViewInfo.ItemVerticalIndent; } }
		protected virtual int GetIndentForRow(int rowVisibleIndex) {
			if(View.IsGroupRow(VisibleRows[rowVisibleIndex].RowHandle) ||
				rowVisibleIndex > 0 && View.IsGroupRow(VisibleRows[rowVisibleIndex - 1].RowHandle))
				return ViewInfo.GroupItemIndent;
			return PrimaryItemIndent;
		}
		protected abstract void CalcIconDrawInfo(WinExplorerItemInfoCollection cachedItems, int rowIndex, Point startLocation);
		protected internal virtual int ArrangeGroupItem(WinExplorerGroupViewInfo itemInfo, Point location) {
			Rectangle bounds = new Rectangle(location, new Size(ViewInfo.ContentBounds.Right - location.X, ViewInfo.GroupCaptionHeight));
			itemInfo.ArrangeGroup(bounds);
			return GetLocationAfterGroup(location, itemInfo.Row.RowHandle);
		}
		protected virtual WinExplorerItemViewInfo CreateItemInfo(GridRow gridRow) {
			if(View.IsGroupRow(gridRow.RowHandle))
				return new WinExplorerGroupViewInfo(ViewInfo, gridRow);
			return new WinExplorerItemViewInfo(ViewInfo, gridRow);
		}
		protected virtual int GetLocationAfterGroup(Point location, int groupRowHandle) {
			return 0;
		}
		protected internal virtual int ArrangeItem(WinExplorerItemViewInfo itemInfo, Point location, Size itemSize) {
			Rectangle content1Bounds = Rectangle.Empty, textBounds = Rectangle.Empty, imageBounds = Rectangle.Empty, checkBoxBounds = Rectangle.Empty;
			Rectangle itemBounds = new Rectangle(location, itemSize);
			if(WindowsFormsSettings.GetIsRightToLeft(ViewInfo.GridControl) && !itemInfo.SuppressRtl) {
				itemBounds = GetRtlBounds(itemBounds, ViewInfo.ContentBounds);
			}
			ViewInfo.TextImagePanel.ArrangeItems(itemBounds, ViewInfo.ItemContent1Size, ViewInfo.ItemContent2Size, ref content1Bounds, ref textBounds);
			ViewInfo.CheckBoxImagePanel.ArrangeItems(content1Bounds, ViewInfo.GetCheckBoxSize(), ViewInfo.GetImageSize(), ref checkBoxBounds, ref imageBounds);
			if(WindowsFormsSettings.GetIsRightToLeft(ViewInfo.GridControl)) {
				checkBoxBounds = GetRtlBounds(checkBoxBounds, itemBounds);
				imageBounds = GetRtlBounds(imageBounds, itemBounds);
				textBounds = GetRtlBounds(textBounds, itemBounds);
			}
			itemInfo.ArrangeItem(itemBounds, checkBoxBounds, imageBounds, textBounds);
			return GetLocationAfterItem(location, itemSize);
		}
		internal static Rectangle GetRtlBounds(Rectangle rect, Rectangle bounds) {
			return new Rectangle(bounds.Right - rect.Width - (rect.Left - bounds.Left), rect.Y, rect.Width, rect.Height);
		}
		protected virtual int GetLocationAfterItem(Point location, Size itemSize) {
			return 0;
		}
		protected internal Rectangle ScrollAreaBounds {
			get { return new Rectangle(ViewInfo.ClientBounds.X, ViewInfo.ContentBounds.Y, ViewInfo.ClientBounds.Width, ViewInfo.ClientBounds.Bottom - ViewInfo.ContentBounds.Y); }
		}
		protected internal virtual void UpdateScrollBar() {
			View.ScrollInfo.ClientRect = ScrollAreaBounds;
			View.ScrollInfo.UpdateLookAndFeel(View.LookAndFeel);
			UpdateScrollBarCore();
			View.ScrollInfo.UpdateScrollRects();
		}
		protected internal bool SuppressPositionUpdate { get; set; }
		protected abstract void UpdateScrollBarCore();
		protected internal abstract int AvailableColumnCount { get; }
		protected internal virtual int CalcTotalRowsCount() {
			int columnCount = AvailableColumnCount == 0 ? 1 : AvailableColumnCount;
			return (int)Math.Ceiling((double)View.DataRowCount / columnCount);
		}
		protected internal virtual Rectangle CalcContentBounds(Rectangle bounds) {
			return bounds;
		}
		protected internal abstract int CalcScreenRowsCount();
		protected internal abstract int CalcRowLocationByRowHandle(int rowHandle);
		protected internal abstract int CalcGroupLocationByHandle(int groupRowHandle);
		protected internal virtual Rectangle CalcItemBoundsbyContent(WinExplorerItemViewInfo itemInfo) {
			int startX = Math.Min(itemInfo.TextBounds.X, itemInfo.ImageBounds.X);
			int startY = Math.Min(itemInfo.TextBounds.Y, itemInfo.ImageBounds.Y);
			int endX = Math.Max(itemInfo.TextBounds.Right, itemInfo.ImageBounds.Right);
			int endY = Math.Max(itemInfo.TextBounds.Bottom, itemInfo.ImageBounds.Bottom);
			if(ViewInfo.WinExplorerView.OptionsView.ShowCheckBoxes) {
				startX = Math.Min(itemInfo.CheckBoxBounds.X, startX);
				startY = Math.Min(itemInfo.CheckBoxBounds.Y, startY);
				endX = Math.Max(itemInfo.CheckBoxBounds.Right, endX);
				endY = Math.Max(itemInfo.CheckBoxBounds.Bottom, endY);
			}
			return ViewInfo.CalcItemBoundsByContentBounds(itemInfo, new Rectangle(startX, startY, endX - startX, endY - startY));
		}
		public virtual int ConstrainPosition(int value) {
			return value;
		}
		protected internal virtual void UpdateCacheFromHandle(int groupRowHandle) {
		}
		public virtual void ScrollToItem(int newHandle) { }
		protected internal int CalcScrollableAreaSize() {
			if(View.DataController.GroupInfo.Count > 0)
				return CalcScrollableAreaSizeGroup();
			return CalcScrollableAreaSizeNoGroup();
		}
		protected internal abstract int CalcScrollableAreaSizeGroup();
		protected internal abstract int CalcScrollableAreaSizeNoGroup();
		public virtual int ItemsTopLocation { get { return ViewInfo.ContentBounds.Y; } }
		public virtual void OnTouchScroll(Utils.Gesture.GestureArgs info, Point delta, Point overPan) {
		}
		protected virtual int GetPrimarySize(Size sz) { return sz.Height; }
		protected internal virtual bool ShouldShowScrollBar() {
			return CalcBestSize() >= GetPrimarySize(ScrollViewAreaSize);
		}
		protected internal Size ScrollViewAreaSize {
			get { return new Size(ViewInfo.ClientBounds.Width, ViewInfo.ContentBounds.Height); }
		}
		protected internal virtual int CalcBestSize() {
			if(!ViewInfo.HasGroups)
				return CalcBestSizeNoGroup();
			return CalcBestSizeGroup();
		}
		protected virtual int CalcBestSizeNoGroup() {
			return 0;	
		}
		protected virtual int CalcBestSizeGroup() {
			return 0;
		}
		protected internal virtual void UpdatePosition(Rectangle contentRect) {
			if(View.Position > 0 && CalcBestSize() - View.Position < contentRect.Height) {
				View.InternalSetPosition(Math.Max(0, CalcBestSize() - contentRect.Height));
			}
		}
		protected internal virtual WinExplorerItemViewInfo GetLastVisibleItem() {
			return null;
		}
	}
	public class WinExplorerViewInfoLayoutCalculator : WinExplorerViewInfoLayoutCalculatorBase {
		public WinExplorerViewInfoLayoutCalculator(WinExplorerViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected internal override WinExplorerItemViewInfo GetLastVisibleItem() {
			WinExplorerItemViewInfo res = null;
			foreach(WinExplorerItemViewInfo itemInfo in VisibleItems) {
				if(!itemInfo.IsFullyVisible)
					continue;
				if(res == null || res.Bounds.Y < itemInfo.Bounds.Y || (res.Bounds.Y == itemInfo.Bounds.Y && res.Bounds.X <= itemInfo.Bounds.X))
					res = itemInfo;
			}
			return res;
		}
		protected override Point GetTopLeftCoor() {
			Rectangle content = ViewInfo.ContentBounds;
			int cc = AvailableColumnCount;
			int availableWidth = cc * ViewInfo.ItemSize.Width + (cc - 1) * ViewInfo.ItemHorizontalIndent;
			if(View.OptionsView.ContentHorizontalAlignment == HorzAlignment.Default ||
				View.OptionsView.ContentHorizontalAlignment == HorzAlignment.Near)
				return content.Location;
			if(View.OptionsView.ContentHorizontalAlignment == HorzAlignment.Far)
				return new Point(content.Right - availableWidth, content.Y);
			return new Point(content.X + (content.Width - availableWidth) / 2, content.Y);
		}
		protected override int CalcBestSizeNoGroup() {
			int rowCount = GetRowCount(View.DataRowCount, AvailableColumnCount);
			int height = rowCount * GetPrimarySize(ViewInfo.ItemSize);
			if(rowCount > 0)
				height += (rowCount - 1) * PrimaryItemIndent;
			return height;
		}
		public override int CalcPositionDelta(int mouseWheelDelta) {
			int deltaMultiplier = ViewInfo.AllowSmoothScrollOnMouseWheel ? 100 : 20;
			return View.CalcVSmallChange() * deltaMultiplier * (mouseWheelDelta < 0 ? 1 : -1);
		}
		protected override int CalcBestSizeGroup() {
			int offset = ScrollingCache[ScrollingCache.Length - 1];
			offset += ViewInfo.GroupIndent;
			offset += CalcGroupPrimarySize(-ScrollingCache.Length);
			return offset;
		}
		int overPanValue = 0;
		public override void OnTouchScroll(Utils.Gesture.GestureArgs info, Point delta, Point overPan) {
			if(info.IsBegin) {
				overPanValue = 0;
				return;
			}
			if(info.IsEnd) {
			}
			if(delta.Y == 0 || ViewInfo.ScrollBarPresence != ScrollBarPresence.Visible) return;
			int prevTop = View.Position;
			View.Position -= delta.Y;
			if(prevTop == View.Position)
				overPanValue += delta.Y;
			else
				overPanValue = 0;
			overPan.Y = overPanValue;
		}
		public override int ConstrainPosition(int value) {
			if(value < 0)
				value = 0;
			int height = Math.Max(0, CalcScrollableAreaSize() - ViewInfo.ContentBounds.Height);
			if(value > height)
				value = height;
			return value;
		}
		public override void ScrollToItem(int newHandle) {
			if(newHandle == GridControl.InvalidRowHandle) {
				View.RaiseDelayedFocusedRowChanged();
				return;
			}
			int location = CalcRowLocationByRowHandle(newHandle);
			WinExplorerItemViewInfo itemInfo = ViewInfo.GetItemByRow(newHandle);
			if(itemInfo != null && itemInfo.IsFullyVisible) {
				View.RaiseDelayedFocusedRowChanged();
				return;
			}
			if(location > View.Position) {
				location = location + ViewInfo.ItemSize.Height - ViewInfo.ContentBounds.Height;
			}
			if(View.OptionsBehavior.EnableSmoothScrolling) {
				if(View.Position != location) {
					ViewInfo.ScrollHelper.Scroll(View.Position, location, WinExplorerViewInfo.ScrollTime, true);
				}
				else {
					ViewInfo.WinExplorerView.Invalidate();
					View.RaiseDelayedFocusedRowChanged();
				}
			}
			else { 
				View.Position = location;
				if(ViewInfo.ShowEditorWhenItemBecomesVisible) {
					WinExplorerViewColumnType columnType = ModifierKeysHelper.IsShiftPressed ? WinExplorerViewColumnType.Description : WinExplorerViewColumnType.Text;
					View.ShowEditor(columnType);
				}
			}
		}
		protected internal override void UpdateCacheFromHandle(int groupRowHandle) {
			ScrollingCache.UpdateCacheFromHandle(groupRowHandle);
		}
		protected internal override int CalcGroupLocationByHandle(int groupRowHandle) {
			return ScrollingCache.CalcGroupLocationByHandle(groupRowHandle);
		}
		protected internal override int CalcScrollableAreaSizeGroup() {
			return ScrollingCache.CalcScrollableAreaHeightGroup();
		}
		protected internal override int CalcScrollableAreaSizeNoGroup() {
			int deltaRowsCount = CalcTotalRowsCount();
			int height = deltaRowsCount * ViewInfo.ItemSize.Height;
			if(deltaRowsCount > 0)
				height += (deltaRowsCount - 1) * ViewInfo.ItemVerticalIndent;
			return height;
		}
		protected internal override int CalcScreenRowsCount() {
			return CalcRowCountInSpace(ViewInfo.ContentBounds.Height);
		}
		protected internal override int CalcRowCountInSpace(int space) {
			return (space + ViewInfo.ItemVerticalIndent) / (ViewInfo.ItemSize.Height + ViewInfo.ItemVerticalIndent);
		}
		protected internal override int AvailableColumnCount {
			get {
				if(View.OptionsView.Style == WinExplorerViewStyle.Content)
					return 1;
				int totalItemWidth = ViewInfo.ItemSize.Width + ViewInfo.ItemHorizontalIndent;
				if(totalItemWidth == 0) return 0;
				return Math.Max(1, (ViewInfo.ContentBounds.Width + ViewInfo.ItemHorizontalIndent) / totalItemWidth);
			}
		}
		protected virtual int CalcRowLocationNoGroup(int visibleIndex) {
			int row = visibleIndex / AvailableColumnCount;
			return row * (ViewInfo.ItemSize.Height + ViewInfo.ItemVerticalIndent);
		}
		protected internal override int CalcRowLocationByRowHandle(int rowHandle) {
			if(View.SortInfo.GroupCount == 0)
				return CalcRowLocationNoGroup(rowHandle);
			int groupRowHandle = View.IsGroupRow(rowHandle) ? rowHandle : View.GetParentRowHandle(rowHandle);
			if(!View.IsValidRowHandle(groupRowHandle)) {
				return 0;
			}
			int index = -groupRowHandle - 1;
			if(index < 0 || index >= ScrollingCache.Length)
				return 0;
			int offset = ScrollingCache[index];
			if(rowHandle == groupRowHandle)
				return offset;
			offset += ViewInfo.GroupCaptionHeight + ViewInfo.GroupItemIndent;
			int firstRowHandle = View.DataController.GroupInfo.GetChildRow(groupRowHandle, 0);
			int deltaRow = (rowHandle - firstRowHandle) / AvailableColumnCount;
			offset += deltaRow * (ViewInfo.ItemSize.Height + ViewInfo.ItemVerticalIndent);
			return offset;
		}
		protected override Point CalcFirstItemLocation(int position) {
			Point topLeft = GetTopLeftCoor();
			int screenYLocation = CalcTopVisibleRowScreenLocation(position) + ViewInfo.VerticalContentOffset;
			return new Point(topLeft.X, screenYLocation);
		}
		protected override void UpdateScrollBarCore() {
			View.ScrollInfo.HScrollVisible = View.GridControl != null && View.GridControl.UseEmbeddedNavigator;
			View.ScrollInfo.VScrollVisible = View.IsVisible && ViewInfo.ScrollBarPresence == ScrollBarPresence.Visible;
			ScrollArgs args = new ScrollArgs();
			args.Enabled = ViewInfo.ShouldShowScrollBar();
			args.Minimum = 0;
			args.Maximum = ViewInfo.CalcScrollableAreaSize();
			args.SmallChange = 3;
			args.LargeChange = ViewInfo.ContentBounds.Height;
			args.Value = View.Position;
			try {
				SuppressPositionUpdate = true;
				View.ScrollInfo.VScrollArgs = args;
			}
			finally {
				SuppressPositionUpdate = false;
			}
		}
		public override bool SupportItemType(WinExplorerViewStyle itemType) {
			return itemType != WinExplorerViewStyle.List;
		}
		protected override int GetLocationAfterGroup(Point location, int groupRowHandle) {
			int res = location.Y + ViewInfo.GroupCaptionHeight;
			if(View.GetRowExpanded(groupRowHandle))
				res += ViewInfo.GroupItemIndent;
			else
				res += ViewInfo.CollapsedGroupItemIndent;
			return res;
		}
		protected override int GetLocationAfterItem(Point location, Size itemSize) {
			return location.X + itemSize.Width + ViewInfo.ItemHorizontalIndent;
		}
		public override Rectangle SelectionVisualRect {
			get {
				Rectangle rect = ViewInfo.SelectionRect;
				rect.Y -= View.Position;
				return rect;
			}
		}
		protected virtual bool ShouldStartNewRow(int visibleRowIndex, Point location, Size itemSize) {
			GridRow row = VisibleRows[visibleRowIndex];
			if(visibleRowIndex > 0 && View.IsGroupRow(row.RowHandle) && row.VisibleIndex > 0) return true;
			return VisibleItems.Count > 0 && location.X + itemSize.Width > ViewInfo.ContentBounds.Right;
		}
		protected internal override Rectangle CalcContentBounds(Rectangle bounds) {
			if(ViewInfo.ScrollBarPresence == ScrollBarPresence.Visible) {
				bounds.Width -= View.ScrollInfo.VScroll.GetWidth() + ViewInfo.ScrollBarIndent;
				if(ViewInfo.View.IsRightToLeft)
					bounds.X += View.ScrollInfo.VScroll.GetWidth() + ViewInfo.ScrollBarIndent;
			}
			if(View.GridControl != null && View.GridControl.UseEmbeddedNavigator)
				bounds.Height -= View.ScrollInfo.HScroll.GetHeight() + ViewInfo.ScrollBarIndent;
			bounds.Width -= ViewInfo.ContentAreaIndent;
			bounds.X += ViewInfo.ContentAreaIndent;
			return bounds;
		}
		protected override void CalcIconDrawInfo(WinExplorerItemInfoCollection cachedItems, int rowIndex, Point startLocation) {
			Rectangle itemBounds = Rectangle.Empty;
			Point location = startLocation;
			int itemIndex = 0;
			int prevRowHandle = GridControl.InvalidRowHandle;
			while(rowIndex != DevExpress.Data.DataController.InvalidRow) {
				int rowHandle = View.GetVisibleRowHandle(rowIndex);
				VisibleRows.Add(rowHandle, rowIndex, 0, 0, View.GetRowKeyCore(rowHandle), false);
				if(ShouldStartNewRow(itemIndex, location, ViewInfo.ItemSize)) {
					location.X = startLocation.X;
					if(View.IsGroupRow(rowHandle) && View.IsGroupRow(prevRowHandle)) {
					}
					else 
						location.Y += ViewInfo.ItemSize.Height + GetIndentForRow(itemIndex);
					if(location.Y >= ViewInfo.ContentBounds.Bottom)
						break;
				}
				WinExplorerItemViewInfo itemInfo = GetItemInfo(cachedItems, VisibleRows[itemIndex]);
				itemInfo.CalcFieldsInfo();
				if(itemInfo.IsGroupItem)
					location.Y = ArrangeGroupItem((WinExplorerGroupViewInfo)itemInfo, new Point(GroupStartX, location.Y));
				else
					location.X = ArrangeItem(itemInfo, location, ViewInfo.ItemSize);
				VisibleItems.Add(itemInfo);
				rowIndex = View.GetNextVisibleRow(rowIndex);
				itemIndex++;
				prevRowHandle = rowHandle;
			}
			ClearItemsCache = false;
		}
		protected virtual int GroupStartX { 
			get { 
				if(ViewInfo.WinExplorerView.ScrollInfo.VScrollVisible && ViewInfo.IsRightToLeft)
					return ViewInfo.ClientBounds.X + ViewInfo.WinExplorerView.ScrollInfo.VScrollSize + ViewInfo.ScrollBarIndent;
				return ViewInfo.ClientBounds.X;
			} 
		}
		protected override int CalcGroupPrimarySize(int rowHandle) {
			return ViewInfo.CalcGroupHeight(rowHandle);
		}
		protected virtual int CalcNoGroupRowIndexByPosition(int position, int itemHeight) {
			return (position + ViewInfo.ItemVerticalIndent) / (itemHeight + ViewInfo.ItemVerticalIndent);
		}
		protected internal override int CalcTopLeftIconIndexByPosition(int position) {
			int groupRowHandle = -1;
			if(!View.IsValidRowHandle(groupRowHandle)) {
				int rowCount = CalcNoGroupRowIndexByPosition(position, ViewInfo.ItemSize.Height);
				return rowCount * AvailableColumnCount;
			}
			int topVisibleGroupIndex = CalcTopVisibleGroupIndex(position);
			int offset = ScrollingCache[topVisibleGroupIndex];
			groupRowHandle = -(topVisibleGroupIndex + 1);
			if(offset + ViewInfo.GroupCaptionHeight >= position)
				return View.DataController.GetVisibleIndex(groupRowHandle);
			offset += ViewInfo.GroupCaptionHeight + ViewInfo.GroupItemIndent;
			int rowHandle = ViewInfo.GetFirstItemInGroup(groupRowHandle);
			int itemsCount = CalcRowCountInSpace(position - offset) * AvailableColumnCount;
			if(itemsCount >= ViewInfo.GetChildCount(groupRowHandle)) {
				groupRowHandle--;
				return View.DataController.GetVisibleIndex(groupRowHandle);
			}
			return View.DataController.GetVisibleIndex(rowHandle + itemsCount);
		}
	}
	public class WinExplorerViewInfoLayoutCalculatorList : WinExplorerViewInfoLayoutCalculatorBase {
		public WinExplorerViewInfoLayoutCalculatorList(WinExplorerViewInfo viewInfo) : base(viewInfo) { }
		protected override Point CalcFirstItemLocation(int position) {
			if(position > 0)
				return new Point(ViewInfo.ClientBounds.Left, ViewInfo.ContentBounds.Top);
			return GetTopLeftCoor();
		}
		protected internal override WinExplorerItemViewInfo GetLastVisibleItem() {
			WinExplorerItemViewInfo res = null;
			foreach(WinExplorerItemViewInfo itemInfo in VisibleItems) {
				if(!itemInfo.IsFullyVisible)
					continue;
				if(res == null || res.SelectionBounds.X < itemInfo.SelectionBounds.X || (res.SelectionBounds.X == itemInfo.SelectionBounds.X && res.SelectionBounds.Y <= itemInfo.SelectionBounds.Y))
					res = itemInfo;
			}
			return res;
		}
		protected internal override void UpdatePosition(Rectangle contentRect) {
		}
		public override int CalcPositionDelta(int mouseWheelDelta) {
			return mouseWheelDelta < 0 ? 1 : -1;
		}
		protected internal override int CalcTotalRowsCount() {
			if(!ViewInfo.HasGroups)
				return base.CalcTotalRowsCount();
			int offset = ScrollingCache[ScrollingCache.Length - 1];
			offset += GetRowCount(View.DataController.GroupInfo.GetChildCount(-ScrollingCache.Length), AvailableColumnCount);
			return offset;
		}
		protected internal override bool ShouldShowScrollBar() {
			if(View.Position > 0)
				return true;
			if(!ViewInfo.HasGroups && ViewInfo.VisibleItems.Count < View.DataRowCount)
				return true;
			if(ViewInfo.HasGroups && ViewInfo.VisibleItems.Count < View.DataRowCount + View.DataController.GroupInfo.Count)
				return true;
			foreach(WinExplorerItemViewInfo itemInfo in ViewInfo.VisibleItems) {
				if(itemInfo.SelectionBounds.Right > ViewInfo.ClientBounds.Right)
					return true;
			}
			return false;
		}
		protected override int CalcBestSizeNoGroup() {
			return GetRowCount(View.DataRowCount, AvailableColumnCount);
		}
		protected override int CalcBestSizeGroup() {
			if(ScrollingCache.Length == 0 || AvailableColumnCount == 0)
				return 0;
			int offset = ScrollingCache[ScrollingCache.Length - 1];
			int itemCount = View.DataController.GroupInfo.GetChildCount(-ScrollingCache.Length);
			int rowCount = itemCount / AvailableColumnCount;
			if(itemCount % AvailableColumnCount > 0) rowCount++;
			return offset + rowCount;
		}
		int overPanValue = 0;
		int horizontalOffset = 0;
		protected internal int HorizontalOffset {
			get { return horizontalOffset; }
			set {
				if(HorizontalOffset == value)
					return;
				int prev = HorizontalOffset;
				horizontalOffset = value;
				OnHorizontalOffsetChanged(prev);
			}
		}
		protected virtual void OnHorizontalOffsetChanged(int prev) {
			OffsetItems(HorizontalOffset - prev);
			GenerateAdditionalItems();
		}
		protected virtual void GenerateAdditionalItems() {
			bool shouldRemoveGraphics = false;
			if(ViewInfo.GInfo.Graphics == null) {
				shouldRemoveGraphics = true;
				ViewInfo.GInfo.AddGraphics(null);
			}
			try {
				if(HorizontalOffset < 0)
					GenerateRightItems();
				else
					GenerateLeftItems();
			}
			finally {
				UpdateGroupsLayout();
				if(shouldRemoveGraphics)
					ViewInfo.GInfo.ReleaseGraphics();
			}
		}
		public class WinExplorerLayoutInfo {
			public WinExplorerLayoutInfo() { }
			public WinExplorerLayoutInfo(WinExplorerViewInfoLayoutCalculatorList calculator, int rowIndex, int column, Point startLocation) {
				Calculator = calculator;
				RowIndex = rowIndex;
				Column = column;
				ItemStartLocation = StartLocation = startLocation;
				if(Calculator.ViewInfo.HasGroups)
					ItemStartLocation = new Point(ItemStartLocation.X, ItemStartLocation.Y + Calculator.ViewInfo.GroupCaptionHeight);
				Location = ItemStartLocation;
				GroupRowHandle = GridControl.InvalidRowHandle;
			}
			public WinExplorerViewInfoLayoutCalculatorList Calculator { get; set; }
			public int RowIndex { get; set; }
			public int RowHandle { get; set; }
			public int GroupRowHandle { get; set; }
			public int ColumnItemsCount { get; set; }
			public int MaxItemsCount { get; set; }
			public int ItemIndex { get; set; }
			public int Column { get; set; }
			public int MaxWidth { get; set; }
			public Point StartLocation { get; set; }
			public Point ItemStartLocation { get; set; }
			public Point Location { get; set; }
			public WinExplorerGroupViewInfo Group { get; set; }
			public void MoveColumnLeft() {
				Column--;
				int rowIndex = Calculator.View.GetNextVisibleRow(RowIndex);
				int rowHandle = Calculator.View.GetVisibleRowHandle(rowIndex);
				Location = new Point(Location.X - MaxWidth - Calculator.GetIndentForColumn(RowHandle, rowHandle), ItemStartLocation.Y);
				MaxWidth = 0;
				ColumnItemsCount = 0;
			}
			public void LayoutItem(WinExplorerItemViewInfo itemInfo) {
				itemInfo.Column = Column;
				Size itemSize = itemInfo.ViewInfo.ItemWidth > 0? itemInfo.ViewInfo.ItemSize: itemInfo.CalcBestSize();
				Location = new Point(Location.X, Calculator.ArrangeItem(itemInfo, Location, itemSize));
				MaxWidth = Math.Max(itemSize.Width, MaxWidth);
				ColumnItemsCount++;
			}
			public void MovePrevRow() {
				RowIndex = Calculator.View.GetPrevVisibleRow(RowIndex);
			}
			public void LayoutGroup() {
				if(Group == null)
					return;
				Calculator.ArrangeGroupItem(Group, StartLocation);
			}
			public void OffsetItemsLeft() {
				for(int i = 0; i < ColumnItemsCount; i++) {
					Calculator.ViewInfo.VisibleItems[i].Offset(new Point(-MaxWidth, 0));
				}
			}
			public bool ContinueGenerateFromLeft {
				get {
					return Column >= 0 && Location.X >= Calculator.ViewInfo.ClientBounds.Left;
				}
			}
			public bool ContinueGenerateFromRight {
				get {
					return Location.X < Calculator.ViewInfo.ClientBounds.Right;
				}
			}
			public void MoveColumnRight() {
				Column++;
				Location = new Point(Location.X + MaxWidth + Calculator.GetIndentForColumn(Column), ItemStartLocation.Y);
				MaxWidth = 0;
			}
			public void MoveNextRow() {
				RowIndex = Calculator.View.GetNextVisibleRow(RowIndex);
			}
		}
		protected internal int GetIndentForColumn(int rowHandle1, int rowHandle2) {
			int groupRowHandle1 = View.IsGroupRow(rowHandle1)? rowHandle1: View.GetParentRowHandle(rowHandle1);
			int groupRowHandle2 = View.IsGroupRow(rowHandle2)? rowHandle2 : View.GetParentRowHandle(rowHandle2);
			if(groupRowHandle1 != groupRowHandle2)
				return ViewInfo.GroupItemIndent;
			return PrimaryItemIndent;
		}
		protected virtual void ProcessGroup(WinExplorerItemInfoCollection cachedItems, int groupRowHandle, WinExplorerLayoutInfo info) {
			bool notAdded = VisibleRows.RowByHandle(groupRowHandle) == null;
			if(notAdded)
				VisibleRows.Add(groupRowHandle, info.RowIndex, 0, 0, View.GetRowKeyCore(groupRowHandle), false);
			info.Group = (WinExplorerGroupViewInfo)ViewInfo.GetItemByRow(groupRowHandle);
			if(info.Group == null)
				info.Group = (WinExplorerGroupViewInfo)GetItemInfo(cachedItems, VisibleRows.RowByHandle(groupRowHandle));
			info.Group.CalcFieldsInfo();
			if(notAdded)
				VisibleItems.Add(info.Group);
		}
		protected virtual int GetIndentForColumn(int column) {
			if(!ViewInfo.HasGroups)
				return PrimaryItemIndent;
			int topVisibleGroupIndex = CalcTopVisibleGroupIndex(column);
			if(ScrollingCache[topVisibleGroupIndex] == column) return ViewInfo.GroupIndent;
			return PrimaryItemIndent;
		}
		int GetGroupRowHandle(int rowHandle) {
			if(!ViewInfo.HasGroups)
				return GridControl.InvalidRowHandle;
			if(View.IsGroupRow(rowHandle))
				return rowHandle;
			return View.GetParentRowHandle(rowHandle);
		}
		protected virtual void GenerateLeftItems() {
			if(View.Position == 0 || VisibleItems.Count == 0)
				return;
			WinExplorerItemViewInfo firstItemInfo = GetFirstNonGroupItem();
			if(firstItemInfo == null)
				return;
			Point startPoint = new Point(firstItemInfo.Bounds.X - GetIndentForColumn(firstItemInfo.Column), ViewInfo.ContentBounds.Y);
			WinExplorerLayoutInfo info = new WinExplorerLayoutInfo(this, VisibleItems[0].Row.VisibleIndex - 1, firstItemInfo.Column - 1, startPoint);
			if(!info.ContinueGenerateFromLeft)
				return;
			info.RowIndex = CalcStartRowIndexForColumn(info.Column);
			info.MaxItemsCount = CalcItemsCountInColumn(info.Column);
			while(true) {
				if(ViewInfo.HasGroups) {
					info.RowHandle = View.GetVisibleRowHandle(info.RowIndex);
					int groupRowHandle = View.GetParentRowHandle(info.RowHandle);
					if(View.IsGroupRow(info.RowHandle)  || groupRowHandle != info.GroupRowHandle) { 
						if(ViewInfo.GetItemByRow(groupRowHandle) == null) 
							ProcessGroup(null, groupRowHandle, info);
						info.GroupRowHandle = groupRowHandle;
					}
				}
				for(int i = 0; i < info.MaxItemsCount; i++, info.RowIndex++) {
					info.RowHandle = View.GetVisibleRowHandle(info.RowIndex);
					VisibleRows.Add(info.RowHandle, info.RowIndex, 0, 0, View.GetRowKeyCore(info.RowHandle), false);
					WinExplorerItemViewInfo itemInfo = GetItemInfo(null, VisibleRows[VisibleRows.Count - 1]);
					itemInfo.CalcFieldsInfo();
					info.LayoutItem(itemInfo);
					VisibleItems.Insert(i, itemInfo);
				}
				info.OffsetItemsLeft();
				info.MoveColumnLeft();
				info.Column--;
				if(!info.ContinueGenerateFromLeft)
					break;
				info.RowIndex = CalcStartRowIndexForColumn(info.Column);
				info.MaxItemsCount = CalcItemsCountInColumn(info.Column);
				info.ColumnItemsCount = 0;
			}
		}
		private int CalcItemsCountInColumn(int column) {
			if(!ViewInfo.HasGroups)
				return Math.Min(ViewInfo.AvailableRowCountHorizontal, View.DataRowCount - (column * ViewInfo.AvailableRowCountHorizontal));
			int topVisibleGroupIndex = CalcTopVisibleGroupIndex(column);
			int startColumn = ScrollingCache[topVisibleGroupIndex];
			int childCount = View.DataController.GroupInfo.GetChildCount(-topVisibleGroupIndex - 1);
			return Math.Min(childCount - (column - startColumn) * ViewInfo.AvailableRowCountHorizontal, ViewInfo.AvailableRowCountHorizontal);
		}
		private int CalcStartRowIndexForColumn(int column) {
			if(!ViewInfo.HasGroups)
				return column * ViewInfo.AvailableRowCountHorizontal;
			int topVisibleGroupIndex = CalcTopVisibleGroupIndex(column);
			int startColumn = ScrollingCache[topVisibleGroupIndex];
			int startRowHandle = View.DataController.GroupInfo.GetChildRow(-topVisibleGroupIndex - 1, 0);
			int rowHandle = startRowHandle + (column - startColumn) * ViewInfo.AvailableRowCountHorizontal;
			return View.GetVisibleIndex(rowHandle);
		}
		protected virtual void UpdateGroupsLayout() {
			foreach(WinExplorerItemViewInfo itemInfo in ViewInfo.VisibleItems) {
				if(itemInfo.IsGroupItem)
					ArrangeGroupItem((WinExplorerGroupViewInfo)itemInfo, new Point(0, ViewInfo.ContentBounds.Top));
			}
		}
		protected virtual int GetLeftPosition() {
			WinExplorerItemViewInfo itemInfo = GetFirstNonGroupItem();
			return itemInfo != null ? itemInfo.Bounds.X - ViewInfo.ItemHorizontalIndent : 0;
		}
		protected virtual WinExplorerItemViewInfo GetFirstNonGroupItem() {
			for(int i = 0; i < VisibleItems.Count; i++) {
				if(!VisibleItems[i].IsGroupItem)
					return VisibleItems[i];
			}
			return null;
		}
		protected virtual void GenerateRightItems() {
			WinExplorerItemViewInfo lastItemInfo = VisibleItems[VisibleItems.Count - 1];
			WinExplorerLayoutInfo info = new WinExplorerLayoutInfo(this, lastItemInfo.Row.VisibleIndex + 1, lastItemInfo.Column + 1, new Point(GetRightPosition(), ViewInfo.ContentBounds.Y));
			while(info.RowIndex != DevExpress.Data.DataController.InvalidRow) {
				int rowHandle = View.GetVisibleRowHandle(info.RowIndex);
				if(rowHandle == GridControl.InvalidRowHandle)
					break;
				if(VisibleRows.RowByHandle(rowHandle) == null)
					VisibleRows.Add(rowHandle, info.RowIndex, 0, 0, View.GetRowKeyCore(rowHandle), false);
				if(ShouldStartNewColumn(rowHandle, info.Location, ViewInfo.ItemSize)) {
					info.MoveColumnRight();
					if(!info.ContinueGenerateFromRight)
						break;
				}
				WinExplorerItemViewInfo itemInfo = GetItemInfo(null, VisibleRows[VisibleRows.Count - 1]);
				itemInfo.CalcFieldsInfo();
				if(!itemInfo.IsGroupItem)
					info.LayoutItem(itemInfo);
				VisibleItems.Add(itemInfo);
				info.MoveNextRow();
			}
		}
		protected virtual WinExplorerItemViewInfo GetLastNonGroupItem() {
			for(int i = VisibleItems.Count - 1; i >= 0; i--) {
				if(!VisibleItems[i].IsGroupItem)
					return VisibleItems[i];
			}
			return null;
		}
		protected virtual int GetRightPosition() {
			WinExplorerItemViewInfo itemInfo = GetLastNonGroupItem();
			if(itemInfo == null)
				return 0;
			int columnWidth = ViewInfo.GetColumnWidth(itemInfo.Column);
			return itemInfo.Bounds.X + columnWidth + ViewInfo.ItemHorizontalIndent;
		}
		protected virtual int GetLastColumnIndex() {
			WinExplorerItemViewInfo itemInfo = GetLastNonGroupItem();
			return itemInfo != null ? itemInfo.Column : 0;
		}
		protected virtual void OffsetItems(int delta) {
			foreach(WinExplorerItemViewInfo itemInfo in ViewInfo.VisibleItems) {
				itemInfo.Offset(new Point(delta, 0));
			}
			View.Invalidate();
		}
		protected override void CalcIconDrawInfo(WinExplorerItemInfoCollection cachedItems, int rowIndex, Point startLocation) {
			Rectangle itemBounds = Rectangle.Empty;
			WinExplorerLayoutInfo info = new WinExplorerLayoutInfo(this, rowIndex, View.Position, startLocation);
			while(info.RowIndex != DevExpress.Data.DataController.InvalidRow) {
				int rowHandle = View.GetVisibleRowHandle(info.RowIndex);
				if(info.Group == null && !View.IsGroupRow(rowHandle) && ViewInfo.HasGroups) {
					ProcessGroup(cachedItems, View.GetParentRowHandle(rowHandle), info);
				}
				VisibleRows.Add(rowHandle, info.RowIndex, 0, 0, View.GetRowKeyCore(rowHandle), false);
				if(ShouldStartNewColumn(rowHandle, info.Location, ViewInfo.ItemSize)) {
					info.MoveColumnRight();
					if(!info.ContinueGenerateFromRight)
						break;
				}
				WinExplorerItemViewInfo itemInfo = GetItemInfo(cachedItems, VisibleRows[VisibleRows.Count - 1]);
				itemInfo.CalcFieldsInfo();
				if(itemInfo.IsGroupItem) {
					info.LayoutGroup();
					info.Group = (WinExplorerGroupViewInfo)itemInfo;
				}
				else
					info.LayoutItem(itemInfo);
				VisibleItems.Add(itemInfo);
				info.MoveNextRow();
			}
			info.LayoutGroup();
			ClearItemsCache = false;
		}
		public override void OnTouchScroll(Utils.Gesture.GestureArgs info, Point delta, Point overPan) {
			if(info.IsBegin) {
				overPanValue = 0;
				return;
			}
			if(ViewInfo.ScrollBarPresence != ScrollBarPresence.Visible) return;
			int prevTop = View.Position;
			if((delta.X < 0 && View.Position != View.ConstrainPosition(View.Position + 1)) || (delta.X >= 0 && View.Position > 0))
				HorizontalOffset += delta.X;
			if(HorizontalOffset < 0) {
				int columnWidth = ViewInfo.GetColumnWidth(View.Position);
				if(info.IsEnd) columnWidth /= 2;
				if(columnWidth > 0 && Math.Abs(HorizontalOffset) > columnWidth) {
					this.horizontalOffset = 0;
					View.Position++;
				}
			}
			else if(View.Position > 0){
				int columnWidth = ViewInfo.GetColumnWidth(Math.Max(0, View.Position - 1));
				if(info.IsEnd) columnWidth /= 2;
				if(columnWidth > 0 && HorizontalOffset > columnWidth) {
					this.horizontalOffset = 0;
					View.Position--;
				}
			}
			if(info.IsEnd) {
				HorizontalOffset = 0;
			}
			if(prevTop == View.Position)
				overPanValue += delta.X;
			else
				overPanValue = 0;
			overPan.X = overPanValue;
		}
		public override int ItemsTopLocation {
			get {
				return ViewInfo.HasGroups ? ViewInfo.ContentBounds.Y + ViewInfo.GroupCaptionHeight + ViewInfo.GroupIndent : ViewInfo.ContentBounds.Y;
			}
		}
		protected override WinExplorerViewScrollingCache CreateScrollingCache() {
			return new WinExplorerViewListScrollingCache(ViewInfo);
		}
		public new WinExplorerViewListScrollingCache ScrollingCache { get { return (WinExplorerViewListScrollingCache)base.ScrollingCache; } }
		protected internal override Rectangle CalcContentBounds(Rectangle bounds) {
			if(ViewInfo.ScrollBarPresence == ScrollBarPresence.Visible)
				bounds.Height -= View.ScrollInfo.HScroll.GetHeight() + ViewInfo.ScrollBarIndent;
			bounds.Width -= ViewInfo.ContentAreaIndent;
			bounds.X += ViewInfo.ContentAreaIndent;
			return bounds;
		}
		public override int ConstrainPosition(int value) {
			int maxColumnCount = CalcScrollableAreaSize();
			return Math.Max(0, Math.Min(maxColumnCount, value));
		}
		public override void ScrollToItem(int newHandle) {
			if(newHandle == GridControl.InvalidRowHandle) {
				View.RaiseDelayedFocusedRowChanged();
				return;
			}
			int location = CalcRowLocationByRowHandle(newHandle);
			WinExplorerItemViewInfo itemInfo = ViewInfo.GetItemByRow(newHandle);
			if(location < View.Position || itemInfo == null) {
				View.Position = location;
				View.RaiseDelayedFocusedRowChanged();
				return;
			}
			int deltaWidth = itemInfo.SelectionBounds.Right - ViewInfo.ClientBounds.Right;
			if(deltaWidth < 0) {
				View.RaiseDelayedFocusedRowChanged();
				return;
			}
			int columnIndex = -1;
			int width = 0, lastVisibleItem = -1;
			for(int i = 0; i < ViewInfo.VisibleItems.Count; i++) {
				if(ViewInfo.VisibleItems[i].IsGroupItem || columnIndex == ViewInfo.VisibleItems[i].Column)
					continue;
				columnIndex = ViewInfo.VisibleItems[i].Column;
				width += ViewInfo.VisibleItems[i].Bounds.Width;
				if(i > 0) {
					int indent = ViewInfo.ItemHorizontalIndent;
					if(lastVisibleItem >= 0 && View.GetParentRowHandle(ViewInfo.VisibleItems[lastVisibleItem].Row.RowHandle) != View.GetParentRowHandle(ViewInfo.VisibleItems[i].Row.RowHandle))
						indent = ViewInfo.GroupIndent;
					width += indent;
				}
				lastVisibleItem = i;
				if(width > deltaWidth)
					break;
			}
			View.Position = columnIndex + 1;
			View.RaiseDelayedFocusedRowChanged();
		}
		protected override int PrimaryItemIndent { get { return ViewInfo.ItemHorizontalIndent; } }
		protected internal override int CalcRowLocationByRowHandle(int rowHandle) {
			if(View.SortInfo.GroupCount == 0)
				return CalcRowLocationNoGroup(rowHandle);
			int groupRowHandle = View.IsGroupRow(rowHandle) ? rowHandle : View.GetParentRowHandle(rowHandle);
			if(!View.IsValidRowHandle(groupRowHandle)) {
				return 0;
			}
			int offset = ScrollingCache[-groupRowHandle - 1];
			if(rowHandle == groupRowHandle)
				return offset;
			int firstRowHandle = View.DataController.GroupInfo.GetChildRow(groupRowHandle, 0);
			offset += (rowHandle - firstRowHandle) / AvailableColumnCount;
			return offset;
		}
		protected internal override int CalcGroupLocationByHandle(int groupRowHandle) {
			return 0;
		}
		protected override int CalcGroupPrimarySize(int rowHandle) {
			return ViewInfo.CalcGroupColumnCount(rowHandle);	
		}
		protected virtual int CalcRowLocationNoGroup(int visibleIndex) {
			return AvailableColumnCount == 0 ? 0 : visibleIndex / AvailableColumnCount;
		}
		protected internal override int CalcScrollableAreaSizeNoGroup() {
			if(AvailableColumnCount == 0) return 0;
			int res = View.DataRowCount / AvailableColumnCount;
			if(View.DataRowCount % AvailableColumnCount > 0)
				res++;
			return res - 1;
		}
		protected internal override int CalcScrollableAreaSizeGroup() {
			return ScrollingCache.CalcScrollableAreaColumnCount() - 1;
		}
		protected internal override int CalcScreenRowsCount() {
			return CalcRowCountInSpace(ViewInfo.ContentBounds.Width);
		}
		protected internal override int CalcRowCountInSpace(int space) {
			return (space + ViewInfo.ItemHorizontalIndent) / (ViewInfo.ItemSize.Width + ViewInfo.ItemHorizontalIndent);
		}
		protected virtual int CalcNoGroupRowIndexByPosition(int position, int itemWidth) {
			return position;
		}
		protected internal override int CalcTopLeftIconIndexByPosition(int position) {
			int groupRowHandle = -1;
			if(!View.IsValidRowHandle(groupRowHandle)) {
				int rowCount = CalcNoGroupRowIndexByPosition(position, ViewInfo.ItemSize.Height);
				return rowCount * AvailableColumnCount;
			}
			int topVisibleGroupIndex = CalcTopVisibleGroupIndex(position);
			int offset = ScrollingCache[topVisibleGroupIndex];
			int deltaRow = position - offset;
			groupRowHandle = -topVisibleGroupIndex - 1;
			int rowHandle = ViewInfo.GetFirstItemInGroup(groupRowHandle);
			return View.DataController.GetVisibleIndex(rowHandle + deltaRow * AvailableColumnCount);
		}
		public override bool SupportItemType(WinExplorerViewStyle itemType) {
			return itemType == WinExplorerViewStyle.List;
		}
		protected internal override int AvailableColumnCount {
			get {
				int contentHeight = ViewInfo.ContentBounds.Height;
				if(ViewInfo.HasGroups)
					contentHeight -= ViewInfo.GroupCaptionHeight + ViewInfo.GroupItemIndent;
				int totalItemHeight = ViewInfo.ItemSize.Height + ViewInfo.ItemVerticalIndent;
				if(totalItemHeight == 0) return 0;
				return Math.Max(0, (contentHeight + ViewInfo.ItemVerticalIndent) / totalItemHeight); 
			}
		}
		protected override void UpdateScrollBarCore() {
			View.ScrollInfo.VScrollVisible = false;
			View.ScrollInfo.HScrollVisible = View.IsVisible && ViewInfo.ScrollBarPresence == ScrollBarPresence.Visible;
			View.ScrollInfo.HScrollVisible |= View.GridControl != null && View.GridControl.UseEmbeddedNavigator;
			ScrollArgs args = new ScrollArgs();
			args.Enabled = ViewInfo.ShouldShowScrollBar();
			args.Minimum = 0;
			args.Maximum = ViewInfo.CalcScrollableAreaSize();
			args.SmallChange = 1;
			args.LargeChange = 1;
			args.Value = View.Position;
			try {
				SuppressPositionUpdate = true;
				View.ScrollInfo.HScrollArgs = args;
			}
			finally {
				SuppressPositionUpdate = false;
			}
		}
		public override Rectangle SelectionVisualRect {
			get {
				return ViewInfo.SelectionRect;
			}
		}
		protected override int GetLocationAfterItem(Point location, Size itemSize) {
			return location.Y + itemSize.Height + ViewInfo.ItemVerticalIndent;
		}
		protected virtual bool ShouldStartNewColumn(int rowHandle, Point location, Size itemSize) {
			if(View.IsGroupRow(rowHandle) && VisibleItems.Count > 0)
				return true;
			return location.Y + itemSize.Height > ViewInfo.ContentBounds.Bottom;
		}
		protected virtual bool ShouldStartNewColumn(int prevGroupRowHandle, int rowHandle, Point location, Size itemSize) {
			if(prevGroupRowHandle != GetGroupRowHandle(rowHandle))
				return true;
			return ShouldStartNewColumn(rowHandle, location, itemSize);
		}
		protected virtual Rectangle CalcGroupItemBounds(WinExplorerGroupViewInfo groupInfo, int yPosition) {
			int startX = int.MaxValue;
			int endX = int.MinValue;
			int minRowHandle = int.MaxValue;
			int maxRowHandle = int.MinValue;
			foreach(WinExplorerItemViewInfo itemInfo in VisibleItems) {
				if(itemInfo.IsGroupItem)
					continue;
				if(View.GetParentRowHandle(itemInfo.Row.RowHandle) != groupInfo.Row.RowHandle)
					continue;
				minRowHandle = Math.Min(minRowHandle, itemInfo.Row.RowHandle);
				maxRowHandle = Math.Max(maxRowHandle, itemInfo.Row.RowHandle);
				startX = Math.Min(startX, itemInfo.Bounds.X);
				int right = ViewInfo.ItemWidth > 0 ? itemInfo.Bounds.X + ViewInfo.ItemWidth : itemInfo.SelectionBounds.Right;
				endX = Math.Max(endX, right);
			}
			if(minRowHandle > ViewInfo.GetFirstItemInGroup(groupInfo.Row.RowHandle))
				startX = ViewInfo.ClientBounds.Left;
			if(maxRowHandle < ViewInfo.GetLastItemInGroup(groupInfo.Row.RowHandle))
				endX = ViewInfo.ClientBounds.Right;
			startX = Math.Max(ViewInfo.ClientBounds.Left, startX);
			return new Rectangle(startX, yPosition, endX - startX, ViewInfo.GroupCaptionHeight);
		}
		protected internal override int ArrangeGroupItem(WinExplorerGroupViewInfo itemInfo, Point location) {
			itemInfo.ArrangeGroup(CalcGroupItemBounds(itemInfo, location.Y));
			return location.Y;
		}
	}
	public interface IWinExplorerAsyncImageLoaderClient : IAsyncImageLoaderClient {
		bool ShouldLoadThumbnailImagesFromDataSource();
		bool ShouldCacheThumbnails();
		bool IsRowLoaded(int rowHandle);
		object GetRowCellValue(int rowHandle, GridColumn column);
		object ExtraLargeImages { get; }
		object LargeImages { get; }
		object MediumImages { get; }
		object SmallImages { get; }
		bool AllowReplaceableImages { get; }
		WinExplorerViewStyle WinExplorerViewStyle { get; }
		WinExplorerViewColumns ColumnSet { get; }
		object Images { get; }
	}
	public class WinExplorerViewInfo : ColumnViewInfo, ISupportAnimatedScroll, IWinExplorerAsyncImageLoaderClient, ISupportXtraAnimation {
		internal const int InvalidRowHandle = DevExpress.Data.DataController.InvalidRow;
		internal const float ScrollTime = 0.3f;
		WinExplorerViewHitInfo hoverInfo, pressedInfo, activeEditorInfo;
		AnimatedScrollHelper scrollHelper;
		public WinExplorerViewInfo(WinExplorerView view)
			: base(view) {
				this.hoverInfo = new WinExplorerViewHitInfo();
				this.pressedInfo = new WinExplorerViewHitInfo();
				this.activeEditorInfo = new WinExplorerViewHitInfo();
			this.focusedRowHandle = GridControl.InvalidRowHandle;
			AllowDrawFocus = true;
			AllowFadeOutAnimationWhenFocusChanged = true;
			SelectionStartPoint = InvalidSelectionPoint;
			AllowMakeItemVisible = true;
		}
		public bool AllowReplaceableImages { get { return WinExplorerView.OptionsImageLoad.AllowReplaceableImages; } }
		protected internal bool GetShowDescription() {
			if(ItemViewOptions.ShowDescription == DefaultBoolean.Default)
				return WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Content || WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Tiles;
			return ItemViewOptions.ShowDescription == DefaultBoolean.True;
		}
		protected internal virtual ObjectState CalcCaptionButtonState(WinExplorerGroupViewInfo itemInfo) {
			if(PressedInfo.HitTest == WinExplorerViewHitTest.GroupCaptionButton && PressedInfo.GroupInfo.Row.RowHandle == itemInfo.Row.RowHandle)
				return ObjectState.Pressed;
			if(HoverInfo.HitTest == WinExplorerViewHitTest.GroupCaptionButton && HoverInfo.GroupInfo.Row.RowHandle == itemInfo.Row.RowHandle)
				return ObjectState.Hot;
			return ObjectState.Normal;
		}
		protected internal virtual int GetGroupCaptionLineHeight() { return 1; }
		WinExplorerViewInfoLayoutCalculatorBase calculator;
		public WinExplorerViewInfoLayoutCalculatorBase Calculator {
			get {
				if(calculator == null || !calculator.SupportItemType(WinExplorerView.OptionsView.Style))
					calculator = CreateCalculator();
				return calculator;
			}
		}
		protected virtual WinExplorerViewInfoLayoutCalculatorBase CreateCalculator() {
			if(WinExplorerView.OptionsView.Style == WinExplorerViewStyle.List)
				return new WinExplorerViewInfoLayoutCalculatorList(this);
			return new WinExplorerViewInfoLayoutCalculator(this);
		}
		protected internal bool NeedDrawFocusedRect { get; set; }
		protected internal virtual bool AllowOverlappedCheckBox { get { return WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Tiles && WinExplorerView.OptionsView.ShowCheckBoxes; } }
		protected internal bool ShouldFocusLastRowItemAfterScroll { get; set; }
		protected internal bool ShouldFocusFirstRowItemAfterScroll { get; set; }
		protected internal bool AllowDrawItemSeparator { get { return WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Content; } }
		protected internal AnimatedScrollHelper ScrollHelper {
			get {
				if(scrollHelper == null)
					scrollHelper = new AnimatedScrollHelper(this);
				return scrollHelper;
			}
		}
		protected internal int InternalFocusedRowHandle {
			get { return focusedRowHandle; }
			set { focusedRowHandle = value; }
		}
		int focusedRowHandle;
		protected internal int FocusedRowHandle {
			get { return focusedRowHandle; }
			set {
				if(focusedRowHandle == value)
					return;
				int prevFocusedRowHandle = FocusedRowHandle;
				focusedRowHandle = value;
				OnFocusedRowHandleChanged(prevFocusedRowHandle, FocusedRowHandle);
			}
		}
		BaseImageLoader imageLoader;
		protected internal BaseImageLoader ImageLoader {
			get {
				if(imageLoader == null) {
					if(IsAsyncImageLoader()) imageLoader = new WinExplorerAsyncImageLoader(this);
					else imageLoader = new WinExplorerSyncImageLoader(this);
				}
				return imageLoader;
			}
		}
		internal WinExplorerAsyncImageLoader AsyncImageLoader { get { return ImageLoader as WinExplorerAsyncImageLoader; } }
		protected internal void ResetImageLoader() {
			imageLoader = null;
		}
		protected bool IsAsyncImageLoader() {
			if(WinExplorerView == null) return false;
			return WinExplorerView.OptionsImageLoad.AsyncLoad;
		}
		public WinExplorerItemViewInfo GetItemInfo(int rowHandle) {
			for(int i = 0; i < VisibleItems.Count; i++) {
				if(VisibleItems[i].Row.RowHandle == rowHandle) return VisibleItems[i];
			}
			return null;
		}
		protected internal void ClearImageLoader(){
			if(this.imageLoader != null) ImageLoader.Clear();
			ClearLoadInfo();
		}
		protected internal void BreakAsyncLoading() {
			if(this.imageLoader is AsyncImageLoader)
				AsyncImageLoader.BreakLoading();
		}
		protected internal void ClearLoadInfo() {
			for(int i = 0; i < VisibleItems.Count; i++) {
				ClearItemLoadInfoCore(VisibleItems[i]);
			}
		}
		protected void ClearItemLoadInfoCore(WinExplorerItemViewInfo info) {
			XtraAnimator.Current.Animations.Remove(this, info.ImageInfo.InfoId);
			info.ImageInfo = null;
			info.ClearAnimatedImageHelper();
		}
		protected internal void ClearItemLoadInfo(int rowHandle, GridColumn column) {
			if(!IsImageColumn(column)) return;
			for(int i = 0; i < VisibleItems.Count; i++) {
				if(VisibleItems[i].Row != null && VisibleItems[i].Row.RowHandle == rowHandle) {
					ClearItemLoadInfoCore(VisibleItems[i]);
					return;
				}
			}
		}
		protected bool IsImageColumn(GridColumn column) {
			if(column == null) return false;
			WinExplorerViewColumns columns = WinExplorerView.ColumnSet;
			return columns.MediumImageColumn == column || columns.ExtraLargeImageColumn == column || columns.LargeImageColumn == column || columns.SmallImageColumn == column;
		}
		public ThumbnailImageEventArgs RaiseGetThumbnailImage(ThumbnailImageEventArgs e) {
			if(WinExplorerView == null) return null;
			return WinExplorerView.RaiseGetThumbnailImage(e);
		}
		public Image RaiseGetLoadingImage(GetLoadingImageEventArgs e) {
			if(WinExplorerView == null) return null;
			return WinExplorerView.RaiseGetLoadingImage(e);
		}
		protected internal virtual int GetLastColumn() {
			if(VisibleItems.Count > 0)
				return VisibleItems[VisibleItems.Count - 1].Column;
			return -1;
		}
		protected internal virtual bool AllowMakeItemVisible { get; set; }
		protected virtual void OnFocusedRowHandleChanged(int prevHandle, int newHandle) {
			if(WinExplorerView.OptionsBehavior.EnableSmoothScrolling)
				WinExplorerView.DelayRaiseFocusedRowChangedEvent = true;
			View.FocusedRowHandle = FocusedRowHandle;
			NeedDrawFocusedRect = false;
			ClearSelectionBounds();
			WinExplorerItemViewInfo itemInfo = GetItemByRow(prevHandle);
			if(itemInfo != null && !itemInfo.IsGroupItem && !itemInfo.IsSelected && (!itemInfo.IsChecked || !WinExplorerView.OptionsView.DrawCheckedItemsAsSelected)) {
				if(AllowDrawFocus && AllowFadeOutAnimationWhenFocusChanged) {
					AddFadeOutAnimation(itemInfo, itemInfo.SelectedImageIndex);
				}
			}
			if(newHandle == GridControl.InvalidRowHandle || IsItemVisible(newHandle)) {
				WinExplorerView.Invalidate();
				WinExplorerView.RaiseDelayedFocusedRowChanged();
				return;
			}
			if(AllowMakeItemVisible && WinExplorerView.OptionsBehavior.AutoScrollItemOnMouseClick)
				Calculator.ScrollToItem(newHandle);
		}
		protected virtual void ClearSelectionData() {
			WinExplorerView.DataController.Selection.Clear();
		}
		protected internal virtual bool IsItemVisible(int newHandle) {
			foreach(WinExplorerItemViewInfo itemInfo in VisibleItems) {
				if(itemInfo.Row.RowHandle == newHandle && itemInfo.IsVisible)
					return true;
			}
			return false;
		}
		public virtual WinExplorerViewHitInfo HoverInfo {
			get { return hoverInfo; }
			set {
				if(value == null)
					value = new WinExplorerViewHitInfo();
				if(HoverInfo.Equals(value))
					return;
				WinExplorerViewHitInfo prevInfo = HoverInfo;
				hoverInfo = value;
				OnHoverInfoChanged(prevInfo, hoverInfo);
			}
		}
		public virtual WinExplorerViewHitInfo PressedInfo {
			get { return pressedInfo; }
			set {
				if(value == null)
					value = new WinExplorerViewHitInfo();
				if(PressedInfo.Equals(value))
					return;
				WinExplorerViewHitInfo prevInfo = PressedInfo;
				pressedInfo = value;
				OnPressedInfoChanged(prevInfo, PressedInfo);
			}
		}
		protected internal virtual bool DrawMarqueeSelection {
			get { return WinExplorerView.AllowMarqueeSelection && SelectionStartPoint != InvalidSelectionPoint; }
		}
		protected internal void ClearSelectionBounds() {
			SelectionStartPoint = InvalidSelectionPoint;
			SelectionEndPoint = Point.Empty;
		}
		protected internal Point SelectionStartPoint { get; set; }
		protected internal Point SelectionEndPoint { get; set; }
		internal static Point InvalidSelectionPoint = new Point(-10000, -10000);
		protected internal Rectangle SelectionRect {
			get {
				Rectangle rect = new Rectangle();
				if(SelectionStartPoint == InvalidSelectionPoint)
					return new Rectangle(SelectionStartPoint, Size.Empty);
				rect.X = Math.Min(SelectionStartPoint.X, SelectionEndPoint.X);
				rect.Y = Math.Min(SelectionStartPoint.Y, SelectionEndPoint.Y);
				rect.Width = Math.Abs(SelectionStartPoint.X - SelectionEndPoint.X);
				rect.Height = Math.Abs(SelectionStartPoint.Y - SelectionEndPoint.Y);
				return rect;
			}
		}
		protected internal Rectangle SelectionVisualRect {
			get { return Calculator.SelectionVisualRect; }
		}
		public virtual WinExplorerViewHitInfo ActiveEditorInfo {
			get { return activeEditorInfo; }
			set {
				if(value == null)
					value = new WinExplorerViewHitInfo();
				activeEditorInfo = value;
			}
		}
		protected override XtraEditors.Controls.BorderStyles EditorDefaultBorderStyle {
			get {
				return XtraEditors.Controls.BorderStyles.Default;
			}
		}
		protected internal bool IsPressedInfoChanged { get; set; }
		protected virtual void OnPressedInfoChanged(WinExplorerViewHitInfo prevInfo, WinExplorerViewHitInfo newInfo) {
			IsPressedInfoChanged = true;
			try {
				if(prevInfo.HitTest == WinExplorerViewHitTest.GroupCaptionButton) {
					WinExplorerView.Invalidate(prevInfo.GroupInfo.CaptionButtonBounds);
				}
				else if(prevInfo.InItem || prevInfo.InGroup) {
					((WinExplorerViewHandler)WinExplorerView.Handler).Navigator.ClearPreferredColumnIndex();
					FocusedRowHandle = prevInfo.ItemInfo.Row.RowHandle;
				}
				if(newInfo.HitTest == WinExplorerViewHitTest.GroupCaptionButton) {
					WinExplorerView.Invalidate(newInfo.GroupInfo.CaptionButtonBounds);
				}
				else if(newInfo.InItem || newInfo.InGroup) {
					((WinExplorerViewHandler)WinExplorerView.Handler).Navigator.ClearPreferredColumnIndex();
					FocusedRowHandle = newInfo.ItemInfo.Row.RowHandle;
					XtraAnimator.Current.Animations.Remove(this, newInfo.ItemInfo.Row);
					WinExplorerView.GridControl.Invalidate(newInfo.ItemInfo.Bounds);
				}
			}
			finally {
				IsPressedInfoChanged = false;
			}
		}
		protected virtual void ProcessPrevHoverInfo(WinExplorerViewHitInfo prevInfo) {
			if(prevInfo == null)
				return;
			if(prevInfo.HitTest == WinExplorerViewHitTest.GroupCaptionButton)
				WinExplorerView.Invalidate(prevInfo.GroupInfo.CaptionButtonBounds);
			if(prevInfo.InItem && ItemHoverBordersShowMode == XtraGrid.WinExplorer.ItemHoverBordersShowMode.Never || (ItemHoverBordersShowMode == XtraGrid.WinExplorer.ItemHoverBordersShowMode.ContextButtons && HasAutoContextButtons))
				return;
			if(prevInfo.InItem && (prevInfo.ItemInfo.IsSelected || (prevInfo.ItemInfo.IsChecked && WinExplorerView.OptionsView.DrawCheckedItemsAsSelected)))
				return;
			if(prevInfo.InItem && !prevInfo.ItemInfo.IsSelected && !prevInfo.ItemInfo.IsFocused ||
				prevInfo.InGroup && (prevInfo.HitTest == WinExplorerViewHitTest.GroupCaption ||
				prevInfo.HitTest == WinExplorerViewHitTest.Group)) {
				AddFadeOutAnimation(prevInfo.ItemInfo, prevInfo.ItemInfo.HoveredImageIndex);
			}
		}
		protected virtual void ProcessNewHoverInfo(WinExplorerViewHitInfo newInfo) {
			if(newInfo == null)
				return;
			if(newInfo.HitTest == WinExplorerViewHitTest.GroupCaptionButton)
				WinExplorerView.Invalidate(newInfo.GroupInfo.CaptionButtonBounds);
			if(newInfo.InItem && ItemHoverBordersShowMode == XtraGrid.WinExplorer.ItemHoverBordersShowMode.Never || (ItemHoverBordersShowMode == XtraGrid.WinExplorer.ItemHoverBordersShowMode.ContextButtons && HasAutoContextButtons))
				return;
			if(newInfo.InItem && (newInfo.ItemInfo.IsSelected || (newInfo.ItemInfo.IsChecked && WinExplorerView.OptionsView.DrawCheckedItemsAsSelected)))
				return;
			if(newInfo.InGroup && (newInfo.HitTest == WinExplorerViewHitTest.GroupCaption || newInfo.HitTest == WinExplorerViewHitTest.Group)
				|| (newInfo.InItem && !newInfo.ItemInfo.IsFocused)) {
				AddFadeInAnimation(newInfo.ItemInfo);
			}
		}
		protected virtual void OnHoverInfoChanged(WinExplorerViewHitInfo prevInfo, WinExplorerViewHitInfo newInfo) {
			if(prevInfo != null && newInfo != null && prevInfo.InItem && newInfo.InItem && prevInfo.ItemInfo == newInfo.ItemInfo) 
				return;
			ProcessPrevHoverInfo(prevInfo);
			ProcessNewHoverInfo(newInfo);
		}
		protected virtual bool HasAutoContextButtons {
			get {
				foreach(ContextItem item in WinExplorerView.ContextButtons) {
					if(item.Visibility == ContextItemVisibility.Auto)
						return true;
				}
				return false;
			}
		}
		protected internal virtual ItemHoverBordersShowMode ItemHoverBordersShowMode {
			get { 
				return WinExplorerView.OptionsView.ItemHoverBordersShowMode == ItemHoverBordersShowMode.ContextButtons || 
					WinExplorerView.OptionsView.ItemHoverBordersShowMode == XtraGrid.WinExplorer.ItemHoverBordersShowMode.Default && HasAutoContextButtons ? 
					ItemHoverBordersShowMode.ContextButtons : 
					WinExplorerView.OptionsView.ItemHoverBordersShowMode; 
			}
		} 
		protected virtual bool AllowHoverAnimation {
			get {
				return WindowsFormsSettings.GetAllowHoverAnimation(View);
			}
		}
		protected virtual void AddFadeInAnimation(WinExplorerItemViewInfo itemInfo) {
			if(!AllowHoverAnimation) {
				View.GridControl.Invalidate(itemInfo.Bounds);
				return;
			}
			WinExplorerViewOpcaityAnimationInfo info = XtraAnimator.Current.Get(this, itemInfo.Row) as WinExplorerViewOpcaityAnimationInfo;
			double start = 0;
			if(info != null) {
				XtraAnimator.Current.Animations.Remove(info);
				start = info.Value;
			}
			XtraAnimator.Current.AddAnimation(new WinExplorerViewOpcaityAnimationInfo(itemInfo, start, 1));
		}
		protected virtual void AddFadeOutAnimation(WinExplorerItemViewInfo itemInfo, int startImageIndex) {
			if(!AllowHoverAnimation) {
				View.GridControl.Invalidate(itemInfo.Bounds);
				return;
			}
			WinExplorerViewOpcaityAnimationInfo info = XtraAnimator.Current.Get(this, itemInfo.Row) as WinExplorerViewOpcaityAnimationInfo;
			double start = 1;
			if(info != null) {
				XtraAnimator.Current.Animations.Remove(info);
				start = info.Value;
			}
			itemInfo.StartImageIndex = startImageIndex;
			XtraAnimator.Current.AddAnimation(new WinExplorerViewOpcaityAnimationInfo(itemInfo, start, 0));
		}
		public object Images { 
			get {
				switch(WinExplorerView.OptionsView.Style) {
					case WinExplorerViewStyle.ExtraLarge:
						return WinExplorerView.ExtraLargeImages;
					case WinExplorerViewStyle.Large:
						return WinExplorerView.LargeImages;
					case WinExplorerViewStyle.Medium:
					case WinExplorerViewStyle.Default:
						return WinExplorerView.MediumImages;
					case WinExplorerViewStyle.Small:
					case WinExplorerViewStyle.List:
						return WinExplorerView.SmallImages;
				}
				return WinExplorerView.MediumImages;
			}
		}
		public WinExplorerView WinExplorerView {
			get { return (WinExplorerView)base.View; }
		}
		public GridRowCollection VisibleRows { 
			get { return Calculator.VisibleRows; } 
		}
		public WinExplorerItemInfoCollection VisibleItems {
			get { return Calculator.VisibleItems; }
		}
		protected internal virtual WinExplorerViewStyleOptions ItemViewOptions {
			get {
				return GetItemViewOptions(WinExplorerView.OptionsViewStyles);
			}
		}
		protected virtual WinExplorerViewStyleOptions ItemViewOptionsDefault {
			get {
				return GetItemViewOptions(DefaultItemViewOptionsCollection);
			}
		}
		WinExplorerViewStyleOptions GetItemViewOptions(WinExplorerViewStyleOptionsCollection coll) {
			switch(WinExplorerView.OptionsView.Style) {
				case WinExplorerViewStyle.Default:
				case WinExplorerViewStyle.Medium:
					return coll.Medium;
				case WinExplorerViewStyle.ExtraLarge:
					return coll.ExtraLarge;
				case WinExplorerViewStyle.Large:
					return coll.Large;
				case WinExplorerViewStyle.Tiles:
					return coll.Tiles;
				case WinExplorerViewStyle.List:
					return coll.List;
				case WinExplorerViewStyle.Content:
					return coll.Content;
			}
			return coll.Small;
		}
		WinExplorerViewStyleOptionsCollection defaultItemViewOptionsCollection;
		protected virtual WinExplorerViewStyleOptionsCollection DefaultItemViewOptionsCollection {
			get {
				if(defaultItemViewOptionsCollection == null)
					defaultItemViewOptionsCollection = CreateDefaultItemViewOptionsCollection();
				return defaultItemViewOptionsCollection;
			}
		}
		protected virtual WinExplorerViewStyleOptionsCollection CreateDefaultItemViewOptionsCollection() {
			WinExplorerViewStyleOptionsCollection res = new WinExplorerViewStyleOptionsCollection(null);
			res.ExtraLarge.ContentMargins = new Padding(3);
			res.Large.ContentMargins = new Padding(3);
			res.Medium.ContentMargins = new Padding(3);
			res.Small.ContentMargins = new Padding(1);
			res.List.ContentMargins = new Padding(1);
			res.Tiles.ContentMargins = new Padding(3);
			res.Content.ContentMargins = new Padding(6);
			res.ExtraLarge.ImageMargins = new Padding(3);
			res.Large.ImageMargins = new Padding(3);
			res.Medium.ImageMargins = new Padding(3);
			res.Tiles.ImageMargins = new Padding(3);
			res.Small.ImageMargins = new Padding(1);
			res.List.ImageMargins = new Padding(1);
			res.Content.ImageMargins = new Padding(3);
			res.ExtraLarge.CheckBoxMargins = new Padding(3);
			res.Large.CheckBoxMargins = new Padding(3);
			res.Medium.CheckBoxMargins = new Padding(3);
			res.Tiles.CheckBoxMargins = new Padding(3);
			res.Small.CheckBoxMargins = new Padding(1);
			res.List.CheckBoxMargins = new Padding(1);
			res.Content.CheckBoxMargins = new Padding(0);
			res.ExtraLarge.ImageToTextIndent = 5;
			res.Large.ImageToTextIndent = 5;
			res.Medium.ImageToTextIndent = 5;
			res.Tiles.ImageToTextIndent = 5;
			res.Small.ImageToTextIndent = 2;
			res.List.ImageToTextIndent = 2;
			res.Content.ImageToTextIndent = 12;
			res.ExtraLarge.IndentBetweenGroupAndItem = 1;
			res.Large.IndentBetweenGroupAndItem = 1;
			res.Medium.IndentBetweenGroupAndItem = 1;
			res.Tiles.IndentBetweenGroupAndItem = 1;
			res.Small.IndentBetweenGroupAndItem = 1;
			res.List.IndentBetweenGroupAndItem = 1;
			res.Content.IndentBetweenGroupAndItem = 0;
			res.ExtraLarge.IndentBetweenGroups = 1;
			res.Large.IndentBetweenGroups = 1;
			res.Medium.IndentBetweenGroups = 1;
			res.Tiles.IndentBetweenGroups = 1;
			res.Small.IndentBetweenGroups = 1;
			res.List.IndentBetweenGroups = 1;
			res.Content.IndentBetweenGroups = 0;
			res.ExtraLarge.VerticalIndent = 20;
			res.Large.VerticalIndent = 20;
			res.Medium.VerticalIndent = 20;
			res.Tiles.VerticalIndent = 0;
			res.Small.VerticalIndent = 0;
			res.List.VerticalIndent = 0;
			res.Content.VerticalIndent = 0;
			res.ExtraLarge.HorizontalIndent = 20;
			res.Large.HorizontalIndent = 20;
			res.Medium.HorizontalIndent = 20;
			res.Tiles.HorizontalIndent = 0;
			res.Small.HorizontalIndent = 0;
			res.List.HorizontalIndent = 20;
			res.Content.HorizontalIndent = 0;
			res.Small.ItemWidth = 200;
			res.List.ItemWidth = 200;
			res.Tiles.ItemWidth = 200;
			res.Content.ItemWidth = 200;
			res.ExtraLarge.GroupCaptionButtonIndent =
				res.Large.GroupCaptionButtonIndent =
				res.Medium.GroupCaptionButtonIndent =
				res.Small.GroupCaptionButtonIndent =
				res.Tiles.GroupCaptionButtonIndent =
				res.List.GroupCaptionButtonIndent =
				res.Content.GroupCaptionButtonIndent = 3;
			res.ExtraLarge.GroupCheckBoxIndent =
				res.Large.GroupCheckBoxIndent =
				res.Medium.GroupCheckBoxIndent =
				res.Small.GroupCheckBoxIndent =
				res.Tiles.GroupCheckBoxIndent =
				res.List.GroupCheckBoxIndent =
				res.Content.GroupCheckBoxIndent = 3;
			return res;
		}
		public override void Calc(Graphics g, Rectangle bounds) {
			GInfo.AddGraphics(g);
				try {
					if(AsyncImageLoader != null) {
						AsyncImageLoader.Suspend();
					}
					if(!WinExplorerView.SkipInvalidatePositionCache) {
						Calculator.InvalidateCache();
					}
					this.bounds = bounds;
					this.clientBounds = CalcClientBounds(Bounds);
					CalcConstants();
					CalcViewInfo();
					CheckNavigator();
					ContentBounds = CalcContentBounds(ClientBounds);
					Calculator.CheckCache();
					UpdateScrollBarPresence();
					ContentBounds = CalcContentBounds(ClientBounds);
					ScrollBarPresence prev = ScrollBarPresence;
					UpdatePosition(ContentBounds);
					CalcIconDrawInfo();
					UpdateScrollBarPresence();
					if(prev != ScrollBarPresence) {
						ContentBounds = CalcContentBounds(ClientBounds);
						Calculator.InvalidateCache();
						Calculator.CheckCache();
						CalcIconDrawInfo();
					}
					UpdateFindControlVisibility();
				}
				finally {
					if(AsyncImageLoader != null)
						AsyncImageLoader.Resume();
					GInfo.ReleaseGraphics();
				}
		}
		void CheckNavigator() {
			if(View.GridControl == null || !View.GridControl.UseEmbeddedNavigator) return;
			((GridControlNavigator)View.GridControl.EmbeddedNavigator).UpdateLayout();
		}
		protected internal int VerticalContentOffset {
			get { 
				return Math.Max(0,  Math.Max(0, ContentBounds.Y - ClientBounds.Y + 1)); 
			}
		}
		protected virtual Rectangle CalcContentBounds(Rectangle bounds) {
			bounds = Calculator.CalcContentBounds(bounds);
			int minTop = UpdateFindControlVisibility(ClientBounds, false).Y;
			bounds.Height -= Math.Max(0, minTop - bounds.Y);
			bounds.Y = minTop;
			return bounds;
		}
		protected void UpdatePosition(Rectangle contentRect) {
			Calculator.UpdatePosition(contentRect);
		}
		protected override void UpdatePaintAppearanceDefaults() {
			base.UpdatePaintAppearanceDefaults();
			UpdatePaintAppearanceDefaults(PaintAppearance.GetAppearance("ItemNormal"));
			UpdatePaintAppearanceDefaults(PaintAppearance.GetAppearance("ItemHovered"));
			UpdatePaintAppearanceDefaults(PaintAppearance.GetAppearance("ItemPressed"));
			UpdatePaintAppearanceDefaults(PaintAppearance.GetAppearance("ItemDisabled"));
			UpdatePaintAppearanceDefaultsDescription(PaintAppearance.GetAppearance("ItemDescriptionNormal"));
			UpdatePaintAppearanceDefaultsDescription(PaintAppearance.GetAppearance("ItemDescriptionHovered"));
			UpdatePaintAppearanceDefaultsDescription(PaintAppearance.GetAppearance("ItemDescriptionPressed"));
			UpdatePaintAppearanceDefaultsDescription(PaintAppearance.GetAppearance("ItemDescriptionDisabled"));
			UpdatePaintAppearanceDefaults(PaintAppearance.GetAppearance("GroupNormal"));
			UpdatePaintAppearanceDefaults(PaintAppearance.GetAppearance("GroupHovered"));
			UpdatePaintAppearanceDefaults(PaintAppearance.GetAppearance("GroupPressed"));
		}
		protected virtual void UpdatePaintAppearanceDefaultsDescription(AppearanceObject obj) {
			if(!obj.Options.UseTextOptions || obj.TextOptions.WordWrap == WordWrap.Default) {
				obj.TextOptions.WordWrap = WordWrap.Wrap;
			}
			UpdatePaintAppearanceDefaultsCore(obj);
		}
		protected virtual void UpdatePaintAppearanceDefaults(AppearanceObject obj) {
			if(!obj.Options.UseTextOptions || obj.TextOptions.WordWrap == WordWrap.Default) {
				if(WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Small || WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Tiles || WinExplorerView.OptionsView.Style == WinExplorerViewStyle.List || WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Content)
					obj.TextOptions.WordWrap = WordWrap.NoWrap;
				else
					obj.TextOptions.WordWrap = WordWrap.Wrap;
			}
			UpdatePaintAppearanceDefaultsCore(obj);
		}
		protected virtual void UpdatePaintAppearanceDefaultsCore(AppearanceObject obj) {
			if(!obj.Options.UseTextOptions || obj.TextOptions.Trimming == Trimming.Default)
				obj.TextOptions.Trimming = Trimming.EllipsisCharacter;
			if(!obj.Options.UseTextOptions || obj.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				obj.TextOptions.HotkeyPrefix = HKeyPrefix.None;
		}
		protected virtual Rectangle CalcClientBounds(Rectangle bounds) {
			return CalcBorderRect(bounds);
		}
		protected internal WinExplorerViewStyleOptionsCollection ItemViewOptionsCollection { get { return WinExplorerView != null ? WinExplorerView.OptionsViewStyles : null; } }
		protected internal virtual int ItemVerticalIndent { 
			get { return ItemViewOptions.IsVerticalIndentSet ? ItemViewOptions.VerticalIndent : ItemViewOptionsDefault.VerticalIndent; } 
		}
		public virtual int ItemHorizontalIndent {
			get { return ItemViewOptions.IsHorizontalIndentSet ? ItemViewOptions.HorizontalIndent : ItemViewOptionsDefault.HorizontalIndent; }
		}
		protected internal virtual int GroupItemIndent { get { return ItemViewOptions.IsIndentBetweenGroupAndItemSet ? ItemViewOptions.IndentBetweenGroupAndItem : ItemViewOptionsDefault.IndentBetweenGroupAndItem; } }
		protected internal virtual int CollapsedGroupItemIndent { get { return GroupIndent; } }
		protected internal virtual int GroupIndent { get { return ItemViewOptions.IsIndentBetweenGroupsSet ? ItemViewOptions.IndentBetweenGroups : ItemViewOptionsDefault.IndentBetweenGroups; } }
		protected virtual void CalcIconDrawInfo() {
			CalcIconDrawInfoCore(WinExplorerView.Position);
		}
		protected internal WinExplorerItemViewInfo GetItemByRow(int rowHandle) {
			foreach(WinExplorerItemViewInfo item in VisibleItems) {
				if(item.Row.RowHandle == rowHandle) {
					return item;
				}
			}
			return null;
		}
		protected internal int CalcColumnLocation(int columnIndex) {
			return ContentBounds.X + columnIndex * (ItemSize.Width + ItemHorizontalIndent);
		}
		protected internal int CalcRowLocationHorizontal(int rowIndex) {
			return ContentBounds.Y + rowIndex * (ItemSize.Height + ItemVerticalIndent);
		}
		protected internal virtual bool ClearItemsCache { get { return Calculator.ClearItemsCache; } set { Calculator.ClearItemsCache = value; } }
		protected internal virtual void CalcIconDrawInfoCore(int position) {
			if(ItemViewOptionsCollection != null) {
				ItemViewOptionsCollection.CheckDefaults();
			}
			Calculator.CalcIconDrawInfo(position);
		}
		static Size DefaultExtraLargeImageSize = new Size(256, 256);
		static Size DefaultLargeImageSize = new Size(96, 96);
		static Size DefaultMediumImageSize = new Size(48, 48);
		static Size DefaultSmallImageSize = new Size(16, 16);
		protected internal virtual Size GetImageSize() {
			switch(WinExplorerView.OptionsView.Style) { 
				case WinExplorerViewStyle.ExtraLarge:
					return WinExplorerView.OptionsViewStyles.ExtraLarge.IsImageSizeSet?  WinExplorerView.OptionsViewStyles.ExtraLarge.ImageSize: DefaultExtraLargeImageSize;
				case WinExplorerViewStyle.Large:
					return WinExplorerView.OptionsViewStyles.Large.IsImageSizeSet? WinExplorerView.OptionsViewStyles.Large.ImageSize: DefaultLargeImageSize;
				case WinExplorerViewStyle.Small:
					return WinExplorerView.OptionsViewStyles.Small.IsImageSizeSet ? WinExplorerView.OptionsViewStyles.Small.ImageSize : DefaultSmallImageSize;
				case WinExplorerViewStyle.List:
					return WinExplorerView.OptionsViewStyles.List.IsImageSizeSet? WinExplorerView.OptionsViewStyles.List.ImageSize: DefaultSmallImageSize;
				case WinExplorerViewStyle.Tiles:
					return WinExplorerView.OptionsViewStyles.Tiles.IsImageSizeSet ? WinExplorerView.OptionsViewStyles.Tiles.ImageSize : DefaultMediumImageSize;
				case WinExplorerViewStyle.Content:
					return WinExplorerView.OptionsViewStyles.Content.IsImageSizeSet ? WinExplorerView.OptionsViewStyles.Content.ImageSize : DefaultMediumImageSize;
			}
			return WinExplorerView.OptionsViewStyles.Medium.IsImageSizeSet? WinExplorerView.OptionsViewStyles.Medium.ImageSize: DefaultMediumImageSize;
		}
		protected internal Size GetDesiredThumbnailSize() {
			if(WinExplorerView != null && WinExplorerView.OptionsImageLoad.DesiredThumbnailSize != Size.Empty)
				return WinExplorerView.OptionsImageLoad.DesiredThumbnailSize;
			return DefaultExtraLargeImageSize;
		}
		protected internal virtual Size CheckBoxSize {
			get {
				return new Size(18, 18);
			}
		}
		protected internal virtual Size GetCheckBoxSize() {
			if(!ShowCheckBoxes)
				return Size.Empty;
			return CheckBoxSize;
		}
		protected virtual int CalcGroupCaptionTextHeight() {
			return CalcTextHeight(PaintAppearance.GetAppearance("GroupNormal"), "Wg");
		}
		protected virtual int CalcTextHeight(AppearanceObject app, string text) {
			bool shouldReleaseGraphics = false;
			if(GInfo.Graphics == null) {
				shouldReleaseGraphics = true;
				GInfo.AddGraphics(null);
			}
			try {
				return (int)app.CalcTextSize(GInfo.Graphics, text, 20).Height;
			}
			finally {
				if(shouldReleaseGraphics)
					GInfo.ReleaseGraphics();
			}
		}
		protected virtual int CalcTextHeight() {
			if(WinExplorerView.ColumnSet.TextColumn == null)
				return 0;
			return CalcTextHeight(PaintAppearance.GetAppearance("ItemNormal"), "Wg");
		}
		Items2Panel textImagePanel;
		protected internal Items2Panel TextImagePanel {
			get {
				if(textImagePanel == null)
					textImagePanel = new Items2Panel();
				return textImagePanel;
			}
		}
		Items2Panel imageCheckBoxPanel;
		protected internal Items2Panel CheckBoxImagePanel {
			get {
				if(imageCheckBoxPanel == null)
					imageCheckBoxPanel = new Items2Panel();
				return imageCheckBoxPanel;
			}
		}
		protected Padding ImageMargins {
			get { return ItemViewOptions.IsImageMarginsSet ? ItemViewOptions.ImageMargins : ItemViewOptionsDefault.ImageMargins; }
		}
		protected internal Size ItemContent1Size { get; private set; }
		protected internal Size ItemContent2Size { get; private set; }
		protected Padding ItemContentMargins {
			get { return ItemViewOptions.IsContentMarginsSet ? ItemViewOptions.ContentMargins : ItemViewOptionsDefault.ContentMargins; }
		}
		protected int ImageToTextIndent { 
			get { return ItemViewOptions.IsImageToTextIndentSet ? ItemViewOptions.ImageToTextIndent : ItemViewOptionsDefault.ImageToTextIndent; } 
		}
		protected virtual void SetupTextImagePanel() {
			WinExplorerViewStyle itemType = WinExplorerView.OptionsView.Style;
			if(itemType == WinExplorerViewStyle.Small || itemType == WinExplorerViewStyle.Tiles || itemType == WinExplorerViewStyle.List || itemType == WinExplorerViewStyle.Content) {
				SetupTextImagePanelForSmallItem();
				return;
			}
			TextImagePanel.Content1Location = ItemLocation.Top;
			TextImagePanel.Content1HorizontalAlignment = ItemHorizontalAlignment.Center;
			TextImagePanel.Content1VerticalAlignment = ItemVerticalAlignment.Top;
			TextImagePanel.Content2HorizontalAlignment = ItemHorizontalAlignment.Stretch;
			TextImagePanel.Content2VerticalAlignment = ItemVerticalAlignment.Top;
			TextImagePanel.Content2Padding = new Padding(0);
			TextImagePanel.VerticalIndent = ImageToTextIndent;
			TextImagePanel.VerticalPadding = ItemContentMargins;
			TextImagePanel.Content1Padding = ItemContentMargins;
		}
		protected virtual void SetupTextImagePanelForSmallItem() {
			TextImagePanel.Content1Location = ItemLocation.Left;
			TextImagePanel.Content1HorizontalAlignment = ItemHorizontalAlignment.Left;
			TextImagePanel.Content1VerticalAlignment = ItemVerticalAlignment.Center;
			TextImagePanel.Content2HorizontalAlignment = ItemHorizontalAlignment.Stretch;
			TextImagePanel.Content2VerticalAlignment = ItemVerticalAlignment.Center;
			TextImagePanel.Content2Padding = new Padding(0);
			TextImagePanel.HorizontalIndent = ImageToTextIndent;
			TextImagePanel.HorizontalPadding = ItemContentMargins;
			TextImagePanel.Content1Padding = ItemContentMargins;
			TextImagePanel.StretchContent2SecondarySize = true;
			TextImagePanel.StretchContent2 = true;
		}
		protected virtual void SetupCheckBoxImagePanel() {
			CheckBoxImagePanel.Content1Location = ItemLocation.Left;
			CheckBoxImagePanel.Content1HorizontalAlignment = ItemHorizontalAlignment.Right;
			CheckBoxImagePanel.Content1Padding = new Padding(0);
			CheckBoxImagePanel.Content1VerticalAlignment = ItemVerticalAlignment.Center;
			CheckBoxImagePanel.Content2HorizontalAlignment = ItemHorizontalAlignment.Left;
			CheckBoxImagePanel.Content2VerticalAlignment = ItemVerticalAlignment.Center;
			CheckBoxImagePanel.Content2Padding = new Padding(0);
			CheckBoxImagePanel.HorizontalIndent = CheckBoxMargins.Right;
			CheckBoxImagePanel.StretchContent1 = false;
			CheckBoxImagePanel.StretchContent2 = false;
			if(ShowCheckBoxes) {
				CheckBoxImagePanel.HorizontalPadding = new Padding(CheckBoxMargins.Left, ImageMargins.Top, ImageMargins.Right, 0);
			}
			else {
				CheckBoxImagePanel.HorizontalPadding = new Padding(ImageMargins.Left, ImageMargins.Top, ImageMargins.Right, 0);
			}
		}
		protected internal virtual int GroupCaptionButton2TextIndent { 
			get { 
				return ItemViewOptions.IsGroupCaptionButtonIndentSet ? ItemViewOptions.GroupCaptionButtonIndent: ItemViewOptionsDefault.GroupCaptionButtonIndent;
			} 
		}
		protected internal virtual int GroupCheckBox2TextIndent {
			get {
				return ItemViewOptions.IsGroupCheckBoxIndentSet ? ItemViewOptions.GroupCheckBoxIndent : ItemViewOptionsDefault.GroupCheckBoxIndent;
			}
		}
		protected virtual Padding CheckBoxMargins {
			get { return ItemViewOptions.IsCheckBoxMarginsSet ? ItemViewOptions.CheckBoxMargins : ItemViewOptionsDefault.CheckBoxMargins; }
		}
		protected virtual int CalcGroupHeightByCaptionHeight(int captionHeight) { return captionHeight; }
		protected internal virtual int CalcGroupCaptionHeight() {
			int height = CalcGroupCaptionTextHeight();
			return CalcGroupHeightByCaptionHeight(height);
		}
		public virtual bool ShowCheckBoxes {
			get {
				if(WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Tiles)
					return false;
				return WinExplorerView.OptionsView.ShowCheckBoxes;
			}
		}
		protected internal virtual Size CalcItemSize() {
			return CalcItemSize(0);
		}
		protected internal Dictionary<int, CheckState> CheckedCache {
			get {
				return WinExplorerView.CheckedGroupCache;
			}
		}
		protected internal virtual Size CalcItemSize(int textWidth) {
			Size imageSize = GetImageSize();
			Size checkBoxSize = GetCheckBoxSize();
			int textAreaHeight = CalcTextAreaHeight(WinExplorerView.OptionsView.Style);
			SetupTextImagePanel();
			SetupCheckBoxImagePanel();
			if(ShowCheckBoxes) {
				ItemContent1Size = CheckBoxImagePanel.CalcBestSize(checkBoxSize, imageSize);
			}
			else {
				ItemContent1Size = CheckBoxImagePanel.CalcBestSize(Size.Empty, imageSize);
			}
			ItemContent2Size = new Size(textWidth, textAreaHeight);
			Size res = TextImagePanel.CalcBestSize(ItemContent1Size, ItemContent2Size);
			if(ItemWidth > 0)
				res.Width = ItemWidth;
			CheckItemSizeConstrains(ref res);
			return res;
		}
		public static readonly int MinimumContentItemTypeHeight = 60;
		protected virtual void CheckItemSizeConstrains(ref Size res) {
			if(WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Content) {
				res.Height = Math.Max(res.Height, MinimumContentItemTypeHeight);
			}
		}
		protected virtual int CalcTextAreaHeight(WinExplorerViewStyle itemType) {
			int res = TextHeight * 2;
			if(itemType == WinExplorerViewStyle.Small || itemType == WinExplorerViewStyle.List || itemType == WinExplorerViewStyle.Content)
				res = TextHeight;
			if(itemType == WinExplorerViewStyle.Tiles)
				res = TextHeight * 3;
			else if(GetShowDescription())
				res = TextHeight * 3;
			return res;
		}
		protected internal virtual int ItemWidth {
			get {
				return ItemViewOptions.IsItemWidthSet ? ItemViewOptions.ItemWidth : ItemViewOptionsDefault.ItemWidth;
			}
		}
		protected internal int GroupCaptionHeight { get; set; }
		protected internal Size ItemSize { get; set; }
		protected internal Size GroupCaptionButtonSize { get; set; }
		protected internal int TextHeight { get; set; }
		protected internal int AvailableColumnCount { get { return Calculator.AvailableColumnCount; } }
		protected internal int AvailableRowCountHorizontal { get { return Calculator.AvailableColumnCount; } }
		protected virtual void CalcConstants() {
			GroupCaptionButtonSize = CalcGroupCaptionButtonSize();
			TextHeight = CalcTextHeight();
			ItemSize = CalcItemSize();
			GroupCaptionHeight = CalcGroupCaptionHeight();
		}
		protected virtual Size CalcGroupCaptionButtonSize() {
			return new Size(16, 16);
		}
		protected internal virtual int GetFirstItemInGroup(int groupRowHandle) {
			return View.DataController.GroupInfo.GetChildRow(groupRowHandle, 0);
		}
		protected internal virtual int GetLastItemInGroup(int groupRowHandle) {
			return View.DataController.GroupInfo.GetChildRow(groupRowHandle, GetChildCount(groupRowHandle) - 1);
		}
		protected internal virtual int GetChildCount(int groupRowHandle) {
			return View.DataController.GroupInfo.GetChildCount(groupRowHandle);
		}
		protected internal virtual int CalcGroupColumnCount(int rowHandle) {
			int childCount = View.DataController.GroupInfo.GetChildCount(rowHandle);
			int rowCount = AvailableRowCountHorizontal > 0? AvailableRowCountHorizontal : 1;
			int columnCount = childCount / rowCount;
			if(childCount % rowCount > 0)
				columnCount++;
			return columnCount;
		}
		protected internal virtual int CalcGroupHeight(int rowHandle) {
			if(!WinExplorerView.GetRowExpanded(rowHandle))
				return GroupCaptionHeight;
			int childCount = View.DataController.GroupInfo.GetChildCount(rowHandle);
			int columnCount = AvailableColumnCount > 0? AvailableColumnCount: 1;
			int rowCount = childCount / columnCount;
			rowCount += childCount % columnCount > 0? 1: 0;
			int res = GroupCaptionHeight;
			if(rowCount > 0) res += GroupItemIndent * 2 + rowCount * ItemSize.Height + ItemVerticalIndent * (rowCount - 1);
			return res;
		}
		protected virtual int CalcTopRowIndex(int position) {
			return Calculator.CalcTopLeftIconIndexByPosition(position);
		}
		protected virtual int CalcTopVisibleRowLocation() {
			int rowIndex = View.GetVisibleRowHandle(CalcTopRowIndex(WinExplorerView.Position));
			return CalcRowLocation(rowIndex);
		}
		protected internal virtual int CalcTopVisibleRowScreenLocation(int position) {
			int rowIndex = View.GetVisibleRowHandle(CalcTopRowIndex(position));
			int rowLoc = CalcRowLocation(rowIndex);
			return rowLoc - position;
		}
		protected virtual int CalcTopVisibleRowScreenLocation() {
			return CalcTopVisibleRowScreenLocation(WinExplorerView.Position);
		}
		protected virtual int CalcTotalRowsCount() {
			return Calculator.CalcTotalRowsCount();
		}
		protected virtual int CalcScreenRowsCount() {
			return Calculator.CalcScreenRowsCount();
		}
		Rectangle bounds, clientBounds;
		public override Rectangle Bounds {
			get { return bounds; }
		}
		public override Rectangle ClientBounds {
			get { return clientBounds; }
		}
		public Rectangle ContentBounds { get; private set; }
		protected override DevExpress.Utils.BaseAppearanceCollection CreatePaintAppearances() {
			return new WinExplorerViewAppearances(View);
		}
		protected override BaseSelectionInfo CreateSelectionInfo() {
			return new WinExplorerViewSelectionInfo(View);
		}
		protected override DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo HasItem(CellId id) {
			return null;
		}
		public override ObjectPainter FilterPanelPainter {
			get { return null; }
		}
		public override Rectangle ViewCaptionBounds {
			get { return Rectangle.Empty; }
		}
		public override ObjectPainter ViewCaptionPainter {
			get { return null; }
		}
		protected internal virtual int ConstrainPosition(int value) {
			return Calculator.ConstrainPosition(value);
		}
		protected internal virtual bool ShouldShowScrollBar() {
			return Calculator.ShouldShowScrollBar();
		}
		ScrollBarPresence scrollBarPresence = ScrollBarPresence.Unknown;
		public virtual ScrollBarPresence ScrollBarPresence {
			get {
				if(scrollBarPresence == ScrollBarPresence.Unknown) UpdateScrollBarPresence();
				return scrollBarPresence;
			}
		}
		protected virtual void UpdateScrollBarPresence() {
			if(WinExplorerView.OptionsView.ScrollVisibility != ScrollVisibility.Auto) {
				this.scrollBarPresence = WinExplorerView.OptionsView.ScrollVisibility == ScrollVisibility.Always ? ScrollBarPresence.Visible : ScrollBarPresence.Hidden;
			}
			else {
				this.scrollBarPresence = ScrollBarPresence.Calculable;
				if(ShouldShowScrollBar())
					this.scrollBarPresence = ScrollBarPresence.Visible;
				else
					this.scrollBarPresence = ScrollBarPresence.Hidden;
			}
		}
		public virtual WinExplorerViewHitInfo CalcHitInfo(Point point) {
			WinExplorerViewHitInfo hitInfo = new WinExplorerViewHitInfo();
			hitInfo.HitPoint = point;
			return CalcHitInfo(hitInfo);
		}
		public virtual Point CalcSelectionPoint(Point mousePoint) {
			if(WinExplorerView.OptionsView.Style == WinExplorerViewStyle.List)
				return mousePoint;
			return new Point(mousePoint.X, mousePoint.Y + WinExplorerView.Position);
		}
		protected virtual WinExplorerViewHitInfo CalcHitInfo(WinExplorerViewHitInfo hitInfo) {
			foreach(WinExplorerItemViewInfo itemInfo in VisibleItems) {
				WinExplorerGroupViewInfo groupInfo = itemInfo as WinExplorerGroupViewInfo;
				if(groupInfo != null) {
					hitInfo.ItemInfo = groupInfo;
					if(ShowExpandCollapseButtons && hitInfo.ContainsSet(groupInfo.CaptionButtonBounds, WinExplorerViewHitTest.GroupCaptionButton))
						return hitInfo;
					else if(ShowCheckBoxInGroupCaption && hitInfo.ContainsSet(groupInfo.CheckBoxBounds, WinExplorerViewHitTest.GroupCaptionCheckBox))
						return hitInfo;
					else if(hitInfo.ContainsSet(groupInfo.Bounds, WinExplorerViewHitTest.GroupCaption))
						return hitInfo;
				}
				else if(hitInfo.ContainsSet(itemInfo.SelectionBounds, WinExplorerViewHitTest.Item)) {
					hitInfo.ItemInfo = itemInfo;
					if(hitInfo.ContainsSet(itemInfo.CheckBoxBounds, WinExplorerViewHitTest.ItemCheck))
						return hitInfo;
					if (hitInfo.ContainsSet(itemInfo.DescriptionBounds, WinExplorerViewHitTest.ItemDescription))
						return hitInfo;
					if(hitInfo.ContainsSet(itemInfo.TextBounds, WinExplorerViewHitTest.ItemText))
						return hitInfo;
					hitInfo.ContainsSet(itemInfo.ImageBounds, WinExplorerViewHitTest.ItemImage);
					return hitInfo;
				}
				else if(hitInfo.ContainsSet(itemInfo.CheckBoxBounds, WinExplorerViewHitTest.ItemCheck)) {
					hitInfo.ItemInfo = itemInfo;
				}
			}
			return hitInfo;
		}
		protected internal virtual Rectangle CalcGroupCaptionClient(Rectangle Bounds) {
			return bounds;
		}
		void ISupportAnimatedScroll.OnScroll(double currentScrollValue) {
			WinExplorerView.Position = (int)currentScrollValue;
		}
		void ISupportAnimatedScroll.OnScrollFinish() {
			if(GridControl == null)
				return;
			if(WinExplorerView.WinExplorerViewHandler != null) {
				if(ShouldFocusLastRowItemAfterScroll) {
					WinExplorerView.WinExplorerViewHandler.Navigator.FocusLastVisibleRowItem();
				}
				if(ShouldFocusFirstRowItemAfterScroll) {
					WinExplorerView.WinExplorerViewHandler.Navigator.FocusFirstVisibleRowItem();
				}
			}
			ShouldFocusLastRowItemAfterScroll = false;
			ShouldFocusFirstRowItemAfterScroll = false;
			WinExplorerView.Invalidate();
			if (ShowEditorWhenItemBecomesVisible) {
				ShowEditorWhenItemBecomesVisible = false;
				WinExplorerViewColumnType columnType = ModifierKeysHelper.IsShiftPressed ? WinExplorerViewColumnType.Description : WinExplorerViewColumnType.Text;
				WinExplorerView.ShowEditor(columnType);
			}
			WinExplorerView.RaiseDelayedFocusedRowChanged();
			InvalidateContextButtons();
			SuppressDrawContextButtons = false;
			SetContextButtonsEnabled(true);   
			UpdateContextButtons();
		}
		protected internal virtual void SetContextButtonsEnabled(bool enabled) {
			foreach(WinExplorerItemViewInfo item in VisibleItems) {
				item.SuppressRedrawContextButtons = true;
				item.ContextButtonsHandler.EnableInnerAnimation(enabled);
				item.SuppressRedrawContextButtons = false;
			}
		}
		private void UpdateContextButtons() {
			Point pt = GridControl.PointToClient(Control.MousePosition);
			((WinExplorerViewHandler)View.Handler).OnMouseMoveCore(new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0));
		}
		private void InvalidateContextButtons() {
			foreach(WinExplorerItemViewInfo itemInfo in VisibleItems) {
				if(itemInfo.ContextButtonsViewInfo != null) {
					itemInfo.ContextButtonsViewInfo.ShouldShowAutoItems = false;
					itemInfo.ContextButtonsViewInfo.InvalidateViewInfo();
					itemInfo.ContextButtonsHandler.UpdateItemsByMouse(new Point(-10000, -10000));
				}
			} 
		}
		public virtual int ItemsTopLocation {
			get { return Calculator.ItemsTopLocation; }
		}
		public virtual AppearanceObject GetTextAppearance(WinExplorerItemViewInfo itemInfo) {
			if(!itemInfo.IsEnabled)
				return PaintAppearance.GetAppearance("ItemDisabled");
			if(itemInfo.IsPressed || itemInfo.IsSelected)
				return PaintAppearance.GetAppearance("ItemPressed");
			if(itemInfo.IsHovered || (itemInfo.IsFocused && !AllowDrawFocus))
				return PaintAppearance.GetAppearance("ItemHovered");
			return itemInfo.PaintAppearanceTextNormal;
		}
		public virtual AppearanceObject GetDescriptionAppearance(WinExplorerItemViewInfo itemInfo) {
			if(!itemInfo.IsEnabled)
				return PaintAppearance.GetAppearance("ItemDescriptionDisabled");
			if(itemInfo.IsPressed || itemInfo.IsSelected)
				return PaintAppearance.GetAppearance("ItemDescriptionPressed");
			if(itemInfo.IsHovered || (itemInfo.IsFocused && !AllowDrawFocus))
				return PaintAppearance.GetAppearance("ItemDescriptionHovered");
			return itemInfo.PaintAppearanceDescriptionNormal;
		}
		public virtual AppearanceObject GetGroupCaptionAppearance(WinExplorerGroupViewInfo groupInfo) {
			if(groupInfo.IsPressed || groupInfo.IsSelected)
				return PaintAppearance.GetAppearance("GroupPressed");
			if(groupInfo.IsHovered || (groupInfo.IsFocused && !AllowDrawFocus))
				return PaintAppearance.GetAppearance("GroupHovered");
			return PaintAppearance.GetAppearance("GroupNormal");
		}
		protected internal int CalcTopVisibleGroupIndex(int position) {
			return Calculator.CalcTopVisibleGroupIndex(position);
		}
		protected internal virtual int CalcRowLocation(int rowHandle) {
			return Calculator.CalcRowLocationByRowHandle(rowHandle);
		}
		protected internal virtual int CalcScrollableAreaSize() {
			return Calculator.CalcScrollableAreaSize();
		}
		protected internal virtual int CalcGroupLocationByHandle(int groupRowHandle) {
			return Calculator.CalcGroupLocationByHandle(groupRowHandle);
		}
		protected internal virtual Rectangle CalcItemBoundsByContentBounds(WinExplorerItemViewInfo itemInfo, Rectangle bounds) {
			return bounds;
		}
		public virtual bool IsHorizontal { get { return WinExplorerView.OptionsView.Style == WinExplorerViewStyle.List; } }
		protected internal int GetColumnWidth(int columnIndex) {
			int maxWidth = 0;
			for(int i = 0; i < VisibleItems.Count; i++) {
				if(VisibleItems[i].IsGroupItem)
					continue;
				if(VisibleItems[i].Column > columnIndex)
					break;
				if(VisibleItems[i].Column < columnIndex)
					continue;
				maxWidth = Math.Max(VisibleItems[i].SelectionBounds.Width, maxWidth);
			}
			return maxWidth;
		}
		protected internal virtual void UpdateCacheFromHandle(int groupRowHandle) {
			Calculator.UpdateCacheFromHandle(groupRowHandle);
		}
		public bool AllowDrawFocus { get; set; }
		protected internal bool AllowFadeOutAnimationWhenFocusChanged { get; set; }
		public bool ShowExpandCollapseButtons { get { return WinExplorerView.OptionsView.ShowExpandCollapseButtons && WinExplorerView.OptionsView.Style != WinExplorerViewStyle.List; } }
		public bool ShowCheckBoxInGroupCaption { get { return WinExplorerView.OptionsView.ShowCheckBoxInGroupCaption; } }
		public int ScrollBarIndent { get { return 1; } }
		public int ContentAreaIndent { 
			get {
				if(WinExplorerView.OptionsView.Style == WinExplorer.WinExplorerViewStyle.Content)
					return 0;
				return 12; 
			} 
		}
		public bool HasGroups { get { return View.DataController.GroupInfo.Count > 0; } }
		public virtual int CalcPositionDelta(int mouseWheelDelta) {
			return Calculator.CalcPositionDelta(mouseWheelDelta);
		}
		protected internal virtual WinExplorerItemViewInfo GetLastVisibleItem() {
			return Calculator.GetLastVisibleItem();
		}
		public bool ShowEditorWhenItemBecomesVisible { get; set; }
		public override void PrepareCalcRealViewHeight(Rectangle viewRect, BaseViewInfo oldViewInfo) {
			WinExplorerViewInfo oldWViewInfo = oldViewInfo as WinExplorerViewInfo;
			if(oldWViewInfo != null) {
				this.IsReady = oldViewInfo.IsReady;
			}
		}
		public override int CalcRealViewHeight(Rectangle viewRect) {
			int result = viewRect.Height;
			StartRealHeightCalculate();
			try {
				Calc(null, viewRect);
				int realViewHeight = Calculator.CalcBestSize();
				result = realViewHeight + ContentAreaIndent;
			}
			finally {
				EndRealHeightCalculate();
			}
			return result;
		}
		public void AddAnimation(ImageLoadInfo info) {
			if(GridControl == null || !GridControl.IsHandleCreated) return;
			GridControl.BeginInvoke(new Action<ImageLoadInfo>(OnRunAnimation), info);
		}
		protected internal virtual void RemoveInvisibleAnimations(int addedAnimDSIndex) {
			if(WinExplorerView == null || !WinExplorerView.OptionsImageLoad.AsyncLoad) return;
			for(int i = 0; i < XtraAnimator.Current.Animations.Count; i++) {
				if(XtraAnimator.Current.Animations[i] is ImageShowingAnimationInfo) {
					ImageShowingAnimationInfo ai = (ImageShowingAnimationInfo)XtraAnimator.Current.Animations[i];
					if(ai.AnimatedObject == this)
						RemoveInvisibleAnimationCore(ai, addedAnimDSIndex);
				}
			}
		}
		protected internal void RemoveInvisibleAnimationCore(ImageShowingAnimationInfo ai, int addedAnimDSIndex) {
			if(ai.Item == null || ai.Item.LoadInfo == null)return;
			int rowHandle = View.GetRowHandle(ai.Item.LoadInfo.DataSourceIndex);
			if(VisibleRows.RowByHandle(rowHandle) == null) {
				ai.Item.LoadInfo.IsInAnimation = false;
				XtraAnimator.Current.Animations.Remove(ai);
			}
			if(ai.Item.LoadInfo.DataSourceIndex == addedAnimDSIndex) XtraAnimator.Current.Animations.Remove(ai);
		}
		public void ForceItemRefresh(ImageLoadInfo info) {
			if(GridControl == null || !GridControl.IsHandleCreated || View == null) return;
			int rowHandle = View.GetRowHandle(info.DataSourceIndex);
			GridRow row = VisibleRows.RowByHandle(rowHandle);
			if(row == null) return;
			GridControl.Invoke(new Action<int>(RefreshItem), row.RowHandle);
		}
		protected void RefreshItem(int rowHandle) {
			WinExplorerItemViewInfo info = GetItemInfo(rowHandle);
			if(info == null) return;
			RefreshItemCore(info, false);
		}
		protected internal void RefreshItemCore(WinExplorerItemViewInfo info, bool isFinalFrame) {
			if(WinExplorerView == null || !WinExplorerView.OptionsImageLoad.AsyncLoad) return;
			if(!info.ImageInfo.LoadingStarted) info.ForceUpdateImageContentBounds();
			Rectangle rect = info.GetInvalidateRectangle(isFinalFrame);
			GridControl.Invalidate(rect);
		}
		Random rand = new Random();
		void OnRunAnimation(ImageLoadInfo info) {
			RemoveInvisibleAnimations(info.DataSourceIndex);
			int delay = WinExplorerView == null || !WinExplorerView.OptionsImageLoad.RandomShow ? 0 : rand.Next() % 300;
			if(info.RenderImageInfo == null) return;
			WinExplorerItemViewInfo itemInfo = GetItemInfo(info.RowHandle);
			if(itemInfo != null)
				itemInfo.SelectionBounds = itemInfo.CalcSelectionBounds();
			XtraAnimator.Current.AddAnimation(new WinExplorerImageShowingAnimationInfo(this, info.InfoId, info.RenderImageInfo, 1000 + delay, delay));
		}
		protected internal ImageContentAnimationType GetAnimationType() { return WinExplorerView != null ? WinExplorerView.OptionsImageLoad.AnimationType : ImageContentAnimationType.Expand; }
		public bool ShouldLoadThumbnailImagesFromDataSource() { return WinExplorerView != null ? WinExplorerView.OptionsImageLoad.LoadThumbnailImagesFromDataSource : true; }
		public bool ShouldCacheThumbnails() { return WinExplorerView != null ? WinExplorerView.OptionsImageLoad.CacheThumbnails : true; }
		public bool CanAnimate { get { return true; } }
		public Control OwnerControl { get { return GridControl; } }
		public object GetRowCellValue(int rowHandle, GridColumn column) {
			if(View == null) return null;
			return View.GetRowCellValue(rowHandle, column);
		}
		bool IWinExplorerAsyncImageLoaderClient.IsRowLoaded(int rowHandle) {
			if(WinExplorerView == null || WinExplorerView.DataController == null)
				return true;
			return WinExplorerView.DataController.IsRowLoaded(rowHandle);
		}
		public object ExtraLargeImages { get { return WinExplorerView == null ? null : WinExplorerView.ExtraLargeImages; } }
		public object LargeImages { get { return WinExplorerView == null ? null : WinExplorerView.LargeImages; } }
		public object MediumImages { get { return WinExplorerView == null ? null : WinExplorerView.MediumImages; } }
		public object SmallImages { get { return WinExplorerView == null ? null : WinExplorerView.SmallImages; } }
		public WinExplorerViewStyle WinExplorerViewStyle { get { return WinExplorerView == null ? WinExplorerViewStyle.Default : WinExplorerView.OptionsView.Style; } }
		public WinExplorerViewColumns ColumnSet { get { return WinExplorerView == null ? null : WinExplorerView.ColumnSet; } }
		public virtual bool AllowSmoothScrollOnMouseWheel { 
			get {
				return WinExplorerView.OptionsView.Style != WinExplorer.WinExplorerViewStyle.List && WinExplorerView.OptionsBehavior.EnableSmoothScrolling;
			} 
		}
		internal bool SuppressDrawContextButtons { get; set; }
		internal virtual bool GetIsDataDirty() {
			foreach(WinExplorerItemViewInfo item in VisibleItems) {
				if(item.IsGroupItem)
					continue;
				if(item.Text != item.GetText() || item.Description != item.GetDescription())
					return true;
			}
			return false;
		}
		private WinExplorerItemViewInfo GetItemInfo(Point p) {
			foreach(WinExplorerItemViewInfo info in VisibleItems) {
				if(info.Bounds.Contains(p))
					return info;
			}
			return null;
		}
	}
	public class SkinWinExplorerViewInfo : WinExplorerViewInfo {
		public SkinWinExplorerViewInfo(WinExplorerView view) : base(view) { }
		protected internal override Size CheckBoxSize {
			get {
				SkinElement elem = EditorsSkins.GetSkin(WinExplorerView)[EditorsSkins.SkinCheckBox];
				return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(elem)).Size;
			}
		}
		protected override Size CalcGroupCaptionButtonSize() {
			SkinElementInfo info = GetGroupCaptionButtonInfo(null);
			return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, info).Size;
		}
		protected override WinExplorerViewStyleOptionsCollection CreateDefaultItemViewOptionsCollection() {
			WinExplorerViewStyleOptionsCollection res = base.CreateDefaultItemViewOptionsCollection();
			SkinElementInfo info = GetItemBackgroundInfo(null);
			Padding contentMargins = new Padding(info.Element.ContentMargins.Left, info.Element.ContentMargins.Top, info.Element.ContentMargins.Right, info.Element.ContentMargins.Bottom);
			res.ExtraLarge.ContentMargins = res.Large.ContentMargins = res.Medium.ContentMargins = res.Tiles.ContentMargins = res.Small.ContentMargins = res.List.ContentMargins = contentMargins;
			return res;
		}
		protected internal virtual SkinElement GetGroupHeaderElement() {
			SkinElement elem = GridSkins.GetSkin(WinExplorerView)[GridSkins.SkinWinExplorerViewGroupHeader];
			if(elem == null)
				elem = RibbonSkins.GetSkin(WinExplorerView)[RibbonSkins.SkinPopupGalleryGroupCaption];
			return elem;
		}
		protected internal virtual SkinElementInfo GetGroupHeaderInfo(WinExplorerGroupViewInfo groupInfo) {
			SkinElementInfo info = new SkinElementInfo(GetGroupHeaderElement(), groupInfo.Bounds);
			info.ImageIndex = -1;
			info.State = ObjectState.Normal;
			if(groupInfo.IsPressed)
				info.State = ObjectState.Pressed;
			else if(groupInfo.IsSelected)
				info.State = ObjectState.Selected;
			else if(groupInfo.IsHovered)
				info.State = ObjectState.Hot;
			return info;
		}
		protected override int CalcGroupHeightByCaptionHeight(int captionHeight) {
			SkinElementInfo info = new SkinElementInfo(GetGroupHeaderElement());
			return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info, new Rectangle(0, 0, 10, captionHeight)).Height;
		}
		protected internal override Rectangle CalcGroupCaptionClient(Rectangle bounds) {
			SkinElementInfo info = new SkinElementInfo(GetGroupHeaderElement(), bounds);
			return ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info);
		}
		public virtual SkinElementInfo GetItemBackgroundInfo() {
			SkinElement elem = GridSkins.GetSkin(View)[GridSkins.SkinWinExplorerViewItem];
			if(elem == null)
				elem = RibbonSkins.GetSkin(View)[RibbonSkins.SkinPopupGalleryPopupButton];
			return new SkinElementInfo(elem);
		}
		protected internal virtual SkinElementInfo GetItemBackgroundInfo(WinExplorerItemViewInfo itemInfo) {
			SkinElementInfo info = GetItemBackgroundInfo();
			if(info == null)
				return null;
			if(itemInfo != null)
				info.Bounds = itemInfo.SelectionBounds;
			if(itemInfo == null)
				return info;
			if(itemInfo.IsHovered && ItemHoverBordersShowMode != XtraGrid.WinExplorer.ItemHoverBordersShowMode.Never)
				info.ImageIndex = itemInfo.HoveredImageIndex;
			if(itemInfo.IsPressed)
				info.ImageIndex = itemInfo.PressedImageIndex;
			if((itemInfo.IsFocused && AllowDrawFocus) || itemInfo.IsSelected || (itemInfo.IsChecked && WinExplorerView.OptionsView.DrawCheckedItemsAsSelected))
				info.ImageIndex = itemInfo.SelectedImageIndex;
			return info;
		}
		protected internal override int GetGroupCaptionLineHeight() {
			SkinElement elem = GridSkins.GetSkin(WinExplorerView)[GridSkins.SkinWinExplorerViewGroupCaptionLine];
			if(elem == null)
				elem = CommonSkins.GetSkin(View)[CommonSkins.SkinLabelLine];
			SkinElementInfo info = new SkinElementInfo(elem, Rectangle.Empty);
			return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, info).Height;
		}
		protected internal virtual SkinElementInfo GetGroupCaptionLine(WinExplorerGroupViewInfo groupInfo) {
			SkinElement elem = GridSkins.GetSkin(WinExplorerView)[GridSkins.SkinWinExplorerViewGroupCaptionLine];
			if(elem == null)
				elem = CommonSkins.GetSkin(View)[CommonSkins.SkinLabelLine];
			SkinElementInfo info = new SkinElementInfo(elem, groupInfo.LineBounds);
			info.ImageIndex = -1;
			info.State = ObjectState.Normal;
			return info;
		}
		protected internal override Rectangle CalcItemBoundsByContentBounds(WinExplorerItemViewInfo itemInfo, Rectangle bounds) {
			SkinElementInfo info = GetItemBackgroundInfo(itemInfo);
			return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info, bounds);
		}
		protected internal virtual SkinElementInfo GetGroupCaptionButtonInfo(WinExplorerGroupViewInfo itemInfo) {
			bool isRowExpanded = itemInfo == null? true: WinExplorerView.GetRowExpanded(itemInfo.Row.RowHandle); 
			string skinName = isRowExpanded ? GridSkins.SkinWinExplorerViewGroupCloseButton : GridSkins.SkinWinExplorerViewGroupOpenButton;
			SkinElement elem = GridSkins.GetSkin(WinExplorerView)[skinName];
			if(elem == null) {
				skinName = isRowExpanded ? GridSkins.SkinCardCloseButton : GridSkins.SkinCardOpenButton;
				elem = GridSkins.GetSkin(WinExplorerView)[skinName];
			}
			SkinElementInfo info = new SkinElementInfo(elem, itemInfo != null? itemInfo.CaptionButtonBounds : Rectangle.Empty);
			info.ImageIndex = -1;
			if(itemInfo != null) 
				info.State = CalcCaptionButtonState(itemInfo);
			return info;
		}
		protected internal virtual SkinElementInfo GetWinExplorerViewItemSeparatorInfo(WinExplorerItemViewInfo itemInfo) {
			SkinElement elem = GridSkins.GetSkin(WinExplorerView)[GridSkins.SkinWinExplorerViewItemSeparator];
			if(elem == null)
				elem = CommonSkins.GetSkin(View)[CommonSkins.SkinLabelLine];
			SkinElementInfo info = new SkinElementInfo(elem, itemInfo.ItemSeparatorBounds);
			info.ImageIndex = -1;
			info.State = ObjectState.Normal;
			return info;
		}
	}
	public class WindowsXPWinExplorerViewInfo : WinExplorerViewInfo {
		public WindowsXPWinExplorerViewInfo(WinExplorerView view) : base(view) { }
		protected internal override int GroupItemIndent {
			get {
				return 3;
			}
		}
		protected internal override Rectangle CalcGroupCaptionClient(Rectangle Bounds) {
			int groupCaptionIndent = 5;
			Rectangle rect = new Rectangle(Bounds.X + groupCaptionIndent, Bounds.Y, Bounds.Width - groupCaptionIndent, Bounds.Height);
			return rect;
		}
		protected internal override int CalcGroupCaptionHeight() {
			int height = base.CalcGroupCaptionHeight();
			return height + 5;
		}	 
		public void CalcGroupLineBounds(WinExplorerGroupViewInfo groupInfo) {
			groupInfo.LineBounds = new Rectangle(groupInfo.Bounds.X, groupInfo.Bounds.Bottom - 2, groupInfo.Bounds.Width, 1);
		}
	}
	public class WindowsClassicWinExplorerViewInfo : WindowsXPWinExplorerViewInfo {
		public WindowsClassicWinExplorerViewInfo(WinExplorerView view) : base(view) { }
	}
	public class Windows8WinExplorerViewInfo : WinExplorerViewInfo {
		public Windows8WinExplorerViewInfo(WinExplorerView view) : base(view) { }
		protected internal override int GroupItemIndent {
			get {
				return base.GroupItemIndent + 3;
			}
		}
		protected internal override Rectangle CalcGroupCaptionClient(Rectangle Bounds) {
			int groupCaptionIndent = 10;
			Rectangle rect = new Rectangle(Bounds.X + groupCaptionIndent, Bounds.Y, Bounds.Width - groupCaptionIndent, Bounds.Height);
			return rect;
		}
		public PointF[] CalcCollapseButtonCoordinates(Rectangle captionButtonBounds, Size captionButtonSize) {
			PointF[] res = new PointF[]{
				new PointF(captionButtonBounds.X + (captionButtonBounds.Width - captionButtonSize.Width)/2, captionButtonBounds.Y + (captionButtonBounds.Height - captionButtonSize.Height)/2),
				new PointF(captionButtonBounds.X + (captionButtonBounds.Width - captionButtonSize.Width)/2, captionButtonBounds.Bottom - (captionButtonBounds.Height - captionButtonSize.Height)/2),
				new PointF(captionButtonBounds.Right - (captionButtonBounds.Width - captionButtonSize.Width)/2, captionButtonBounds.Y + captionButtonBounds.Height/2)
			};
			return res;
		}
		public PointF[] CalcExpandButtonCoordinates(Rectangle captionButtonBounds, Size captionButtonSize) {
			PointF[] res = new PointF[]{
				new PointF(captionButtonBounds.X + (captionButtonBounds.Width - captionButtonSize.Width)/2, captionButtonBounds.Bottom - (captionButtonBounds.Height - captionButtonSize.Height)/2),
				new PointF(captionButtonBounds.Right - (captionButtonBounds.Width - captionButtonSize.Width)/2, captionButtonBounds.Bottom - (captionButtonBounds.Height - captionButtonSize.Height)/2),
				new PointF(captionButtonBounds.Right - (captionButtonBounds.Width - captionButtonSize.Width)/2, captionButtonBounds.Y + (captionButtonBounds.Height - captionButtonSize.Height)/2)
			};
			return res;
		}
		protected internal override int CalcGroupCaptionHeight() {
			int height = base.CalcGroupCaptionHeight();
			return height + 10;
		}
	}
	public class WinExplorerViewOpcaityAnimationInfo : DoubleAnimationInfo {
		static int OpacityAnimationLength = 200;
		public WinExplorerViewOpcaityAnimationInfo(WinExplorerItemViewInfo itemInfo, double start, double end) : this(itemInfo, start, end, -1) { }
		public WinExplorerViewOpcaityAnimationInfo(WinExplorerItemViewInfo itemInfo, double start, double end, int prevImageIndex)
			: base(itemInfo.ViewInfo, itemInfo.Row, start, end, OpacityAnimationLength) {
			ItemInfo = itemInfo;
			PrevImageIndex = prevImageIndex;
		}
		public WinExplorerItemViewInfo ItemInfo { get; private set; }
		public int PrevImageIndex { get; private set; }
		public override void FrameStep() {
			base.FrameStep();
			ItemInfo.ViewInfo.View.GridControl.Invalidate(ItemInfo.Bounds);
		}
	}
	public class NullWinExplorerViewInfo : WinExplorerViewInfo {
		public NullWinExplorerViewInfo(WinExplorerView view) : base(view) { }
		public override bool IsNull { get { return true; } }
	}
	public enum WinExplorerViewHitTest { None, Group, GroupCaption, GroupCaptionButton, GroupCaptionCheckBox, Item, ItemCheck, ItemText, ItemImage, ItemDescription }
	public class WinExplorerViewHitInfo : BaseHitInfo {
		public int RowHandle { get { return ItemInfo == null ? GridControl.InvalidRowHandle : ItemInfo.Row.RowHandle; } }
		public bool IsMarqueeSelectionZone { get { return HitTest == WinExplorerViewHitTest.Group || HitTest == WinExplorerViewHitTest.None; } }
		public WinExplorerViewHitTest HitTest { get; internal set; }
		public WinExplorerItemViewInfo ItemInfo { get; internal set; }
		public WinExplorerGroupViewInfo GroupInfo { get { return ItemInfo as WinExplorerGroupViewInfo; } }
		public bool InGroup { get { return HitTest != WinExplorerViewHitTest.None; } }
		public bool InItem { get { return HitTest == WinExplorerViewHitTest.Item || HitTest == WinExplorerViewHitTest.ItemCheck || HitTest == WinExplorerViewHitTest.ItemImage || HitTest == WinExplorerViewHitTest.ItemText || HitTest == WinExplorerViewHitTest.ItemDescription; } }
		public bool ContainsSet(Rectangle bounds, WinExplorerViewHitTest hitTest) {
			if(bounds.Contains(HitPoint)) {
				HitTest = hitTest;
				return true;
			}
			return false;
		}
		public override bool Equals(object obj) {
			WinExplorerViewHitInfo info = obj as WinExplorerViewHitInfo;
			if(info == null) return false;
			if(info.HitTest != HitTest)
				return false;
			if(info.HitTest == WinExplorerViewHitTest.GroupCaption ||
				info.HitTest == WinExplorerViewHitTest.GroupCaptionButton ||
				info.HitTest == WinExplorerViewHitTest.GroupCaptionCheckBox) {
				return GroupInfo.Row.RowHandle == info.GroupInfo.Row.RowHandle;
			}
			else if(info.HitTest == WinExplorerViewHitTest.Item || 
				info.HitTest == WinExplorerViewHitTest.ItemCheck || 
				info.HitTest == WinExplorerViewHitTest.ItemImage || 
				info.HitTest == WinExplorerViewHitTest.ItemText) {
				return ItemInfo.Row.RowHandle == info.ItemInfo.Row.RowHandle;
			}
			return info.HitPoint == HitPoint;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class WinExplorerItemViewInfo : IAnimatedItem, IAsyncImageItemViewInfo, IRowConditionFormatProvider, ISupportContextItems {
		public WinExplorerItemViewInfo(WinExplorerViewInfo viewInfo, GridRow row) {
			Row = row;
			ViewInfo = viewInfo;
			StartImageIndex = -1;
			this.conditionInfo = new ConditionInfo();
			this.formatInfo = new RowFormatRuleInfo(viewInfo.View);
		}
		protected internal bool SuppressRtl { get; set; }
		protected internal virtual bool NeedDrawFocusedRect {
			get {
				if(ViewInfo.FocusedRowHandle != Row.RowHandle || IsSelected)
					return false;
				return ViewInfo.NeedDrawFocusedRect;
			}
		}
		protected internal int Column { get; set; }
		protected internal WinExplorerViewInfo ViewInfo { get; private set; }
		public GridRow Row { get; internal set; }
		AppearanceObject paintAppearanceTextNormal;
		AppearanceObject paintAppearanceDescriptionNormal;
		ConditionInfo conditionInfo;
		RowFormatRuleInfo formatInfo;
		public int StartImageIndex { get; protected internal set; }
		public Rectangle Bounds { get; protected internal set; }
		public Rectangle SelectionBounds { get; protected internal set; }
		public Rectangle TextBounds { get; protected internal set; }
		public Rectangle LineBounds { get; protected internal set; }
		public Rectangle DescriptionBounds { get; protected internal set; }
		public Rectangle ImageBounds { get; protected internal set; }
		public Rectangle ImageContentBounds { get; protected internal set; }
		public Rectangle CheckBoxBounds { get; protected internal set; }
		public Rectangle ItemSeparatorBounds { get; protected internal set; }
		protected internal bool ShowToolTip { get { return showToolTip; } private set { showToolTip = value; } }
		bool showToolTip = true;
		Rectangle OffsetRect(Rectangle rect, Point pt) {
			return new Rectangle(rect.X + pt.X, rect.Y + pt.Y, rect.Width, rect.Height);
		}
		protected internal void Offset(Point offset) {
			Bounds = OffsetRect(Bounds, offset);
			SelectionBounds = OffsetRect(SelectionBounds, offset);
			TextBounds = OffsetRect(TextBounds, offset);
			DescriptionBounds = OffsetRect(DescriptionBounds, offset);
			ImageBounds = OffsetRect(ImageBounds, offset);
			ImageContentBounds = OffsetRect(ImageContentBounds, offset);
			CheckBoxBounds = OffsetRect(CheckBoxBounds, offset);
			ItemSeparatorBounds = OffsetRect(ItemSeparatorBounds, offset);
		}
		public virtual int ImageIndex { get; protected internal set; }
		Image image;
		public virtual Image Image {
			get { return image; }
			protected internal set {
				if(image == value) return;
				image = value;
				ClearAnimatedImageHelper();
			}
		}
		protected bool ShouldCalculateTextInfo { get; set; }
		protected bool ShouldCalculateDescriptionInfo { get; set; }
		string text;
		public virtual string Text {
			get { return text; }
			set {
				if(Text == value)
					return;
				text = value;
				ShouldCalculateTextInfo = true;
			}
		}
		string description;
		public virtual string Description {
			get { return description; }
			set {
				if(Description == value)
					return;
				description = value;
				ShouldCalculateDescriptionInfo = true;
			}
		}
		public AppearanceObject PaintAppearanceTextNormal {
			get { return paintAppearanceTextNormal; }
			set { paintAppearanceTextNormal = value; }
		}
		public AppearanceObject PaintAppearanceDescriptionNormal {
			get { return paintAppearanceDescriptionNormal; }
			set { paintAppearanceDescriptionNormal = value; }
		}
		public virtual Image Icon { get { return null; } }
		public virtual bool IsChecked { get; set; }
		public virtual bool IsEnabled { get; set; }
		public virtual bool IsGroupItem { get { return false; } }
		public virtual StringInfo TextInfo { get; internal set; }
		public virtual StringInfo DescriptionInfo { get; internal set; }
		public virtual bool IsSelected { get { return ViewInfo.WinExplorerView.DataController.Selection.GetSelected(Row.RowHandle); } }
		public virtual void Arrange() {
			SuppressRtl = true;
			try {
				ViewInfo.Calculator.ArrangeItem(this, Bounds.Location, ViewInfo.ItemSize);
			}
			finally {
				SuppressRtl = false;
			}
		}
		public virtual void CalcFieldsInfo() {
			if(ViewInfo.WinExplorerView.Columns.Count == 0)
				return;
			UpdateAndApplyConditionToTextAndDescriptionAppearance();
			Text = GetText();
			Description = GetDescription();
			Image = GetImage();
			IsChecked = ViewInfo.WinExplorerView.HasCheckField ? GetBooleanCellValue(ViewInfo.WinExplorerView.ColumnSet.CheckBoxColumn, Row.RowHandle) : false;
			IsEnabled = ViewInfo.WinExplorerView.HasEnabledField ? GetBooleanCellValue(ViewInfo.WinExplorerView.ColumnSet.EnabledColumn, Row.RowHandle) : true;
		}
		protected internal string GetText() { 
			return ViewInfo.WinExplorerView.HasTextField ? ViewInfo.View.GetRowCellDisplayText(Row.RowHandle, ViewInfo.WinExplorerView.ColumnSet.TextColumn) : "";
		}
		protected internal string GetDescription() { 
			return AllowDescription ? ViewInfo.View.GetRowCellDisplayText(Row.RowHandle, ViewInfo.WinExplorerView.ColumnSet.DescriptionColumn) : "";
		}
		void UpdateAndApplyConditionToTextAndDescriptionAppearance() {
			paintAppearanceTextNormal = ViewInfo.PaintAppearance.GetAppearance("ItemNormal");
			paintAppearanceDescriptionNormal = ViewInfo.PaintAppearance.GetAppearance("ItemDescriptionNormal");
			ViewInfo.UpdateRowConditionAndFormat(this.Row.RowHandle, this);
			foreach(GridColumn column in ViewInfo.View.Columns) {
				AppearanceObjectEx conditionApp = GetAppearance(column);
				AppearanceObjectEx descConditionApp = GetDescriptionAppearance(column);
				if(conditionApp != null) {
					this.paintAppearanceTextNormal = (AppearanceObject)ViewInfo.PaintAppearance.GetAppearance("ItemNormal").Clone();
					AppearanceHelper.Combine(this.paintAppearanceTextNormal, new AppearanceObject[] { conditionApp });
				}
				if(descConditionApp != null) {
					this.paintAppearanceDescriptionNormal = (AppearanceObject)ViewInfo.PaintAppearance.GetAppearance("ItemDescriptionNormal").Clone();
					AppearanceHelper.Combine(this.paintAppearanceDescriptionNormal, new AppearanceObject[] { descConditionApp });
				}
			}
		}
		protected bool GetBooleanCellValue(GridColumn gridColumn, int rowHandle) {
			object currentValue = ViewInfo.View.GetRowCellValue(rowHandle, gridColumn);
			if(currentValue is bool)
				return (bool)currentValue;
			if(DBNull.Value == currentValue)
				return false;
			IConvertible cvt = currentValue as IConvertible;
			if(cvt != null) {
				try {
					bool res = cvt.ToBoolean(Thread.CurrentThread.CurrentUICulture);
					return res;
				}
				catch(FormatException) {
				}
			}
			return false;
		}
		public virtual bool AllowDrawCheckBox {
			get {
				if(!ViewInfo.ShowCheckBoxes && !ViewInfo.AllowOverlappedCheckBox)
					return false;
				if(!ViewInfo.AllowOverlappedCheckBox || IsChecked)
					return true;
				return (ViewInfo.HoverInfo.InItem && ViewInfo.HoverInfo.ItemInfo.Row.RowHandle == Row.RowHandle) || (ViewInfo.FocusedRowHandle == Row.RowHandle);
			}
		}
		public virtual bool ShouldDrawDisabled {
			get {
				return !IsEnabled;
			}
		}
		protected internal virtual bool AllowDescription {
			get {
				WinExplorerView view = ViewInfo.WinExplorerView;
				return view.HasDescriptionField && ViewInfo.GetShowDescription();
			}
		}
		protected internal virtual DescriptionLocation DescriptionLocation {
			get {
				WinExplorerView view = ViewInfo.WinExplorerView;
				return view.OptionsView.Style == WinExplorerViewStyle.Content ? DescriptionLocation.Right : DescriptionLocation.BelowText;
			}
		}
		ImageLoadInfo imageInfo;
		public ImageLoadInfo ImageInfo {
			get {
				if(imageInfo == null) {
					imageInfo = CreateImageLoadInfo();
				}
				return imageInfo;
			}
			set { imageInfo = value; }
		}
		protected ImageLoadInfo CreateImageLoadInfo() {
			int dataSourceIndex = ViewInfo.WinExplorerView.ViewRowHandleToDataSourceIndex(Row.RowHandle);
			ImageLayoutMode mode = ViewInfo.WinExplorerView.OptionsView.ImageLayoutMode;
			ImageContentAnimationType animationType = ViewInfo.GetAnimationType();
			Size desiredSize = ViewInfo.GetDesiredThumbnailSize();
			return new ImageLoadInfo(dataSourceIndex, Row.RowHandle, animationType, mode, ViewInfo.GetImageSize(), desiredSize);
		}
		protected virtual ImageLayoutMode GetImageLayoutMode() {
			if(!ImageInfo.IsLoaded && ViewInfo.ImageLoader is AsyncImageLoader)
				return ImageLayoutMode.Squeeze;
			if(ViewInfo.WinExplorerView.OptionsView.ImageLayoutMode == ImageLayoutMode.Default)
				return ImageLayoutMode.ZoomInside;
			return ViewInfo.WinExplorerView.OptionsView.ImageLayoutMode;
		}
		protected virtual Image GetImage() {
			return ViewInfo.ImageLoader.LoadImage(ImageInfo);
		}
		protected void CalcImageBounds(Rectangle imageBounds) {
			ImageBounds = imageBounds;
			Size imageSize;
			if(ViewInfo.ImageLoader is AsyncImageLoader && ImageInfo.ThumbImage != null)
				imageSize = ImageInfo.AnimatedRegion;
			else imageSize = Image != null ? Image.Size : Size.Empty;
			ImageContentBounds = ImageLayoutHelper.GetImageBounds(ImageBounds, imageSize, GetImageLayoutMode());
		}
		public void ForceUpdateImageContentBounds() {
			Size imageSize = ImageInfo.ThumbImage != null ? ImageInfo.AnimatedRegion : Size.Empty;
			ImageContentBounds = ImageLayoutHelper.GetImageBounds(ImageBounds, imageSize, GetImageLayoutMode());
		}
		protected internal Rectangle GetInvalidateRectangle(bool isFinalFrame) {
			if(isFinalFrame) return Bounds;
			if(ViewInfo.GetAnimationType() == ImageContentAnimationType.Slide) {
				Size size = new Size((int)(ImageBounds.Width + ImageShowingAnimationHelper.OutsideWidth), (int)(ImageBounds.Height + ImageShowingAnimationHelper.OutsideHeight));
				Rectangle rect = new Rectangle(ImageBounds.Location, size);
				return Rectangle.Intersect(ViewInfo.ContentBounds, rect);
			}
			if(ViewInfo.GetAnimationType() == ImageContentAnimationType.Push) {
				Point location = new Point(ImageBounds.X, ImageBounds.Y - ImageShowingAnimationHelper.EasyOutAnimationOdds);
				Size size = new Size(ImageBounds.Width, ImageBounds.Height + ImageShowingAnimationHelper.EasyOutAnimationOdds + ImageShowingAnimationHelper.EasyInAnimationOdds);
				return Rectangle.Intersect(ViewInfo.ContentBounds, new Rectangle(location, size));
			}
			return Rectangle.Intersect(ViewInfo.ContentBounds, ImageBounds);
		}
		public virtual void ArrangeItem(Rectangle bounds, Rectangle checkBoxBounds, Rectangle imageBounds, Rectangle textBounds) {
			Bounds = bounds;
			CalcImageBounds(imageBounds);
			CalcTextAndDescriptionBounds(textBounds);
			CalcCheckBoxBounds(checkBoxBounds);
			CalcItemSeparatorBounds(ViewInfo.GInfo.Graphics, bounds);
			SelectionBounds = CalcSelectionBounds();
			if(ViewInfo.WinExplorerView.OptionsView.Style == WinExplorerViewStyle.List) {
				UpdateTextBoundsBySelectionBounds();
			}
			CalcTextInfo(ViewInfo.GInfo.Graphics);
			CalcDescriptionInfo(ViewInfo.GInfo.Graphics);
			CalcShowToolTip(ViewInfo.GInfo.Graphics);
			InvalidateContextButtonsViewInfo();
		}
		Rectangle CalculatedContextButtonDisplayBounds { get; set; }
		protected internal virtual void CheckContextButtonsLayout() {
			if(CalculatedContextButtonDisplayBounds != ((ISupportContextItems)this).DisplayBounds)
				InvalidateContextButtonsViewInfo();
		}
		protected internal virtual void InvalidateContextButtonsViewInfo() {
			ContextButtonsViewInfo.InvalidateViewInfo();
			CalculatedContextButtonDisplayBounds = ((ISupportContextItems)this).DisplayBounds;
		}
		protected virtual void UpdateTextBoundsBySelectionBounds() {
			int deltaWidth = Bounds.Width - SelectionBounds.Width;
			TextBounds = new Rectangle(TextBounds.Location, new Size(TextBounds.Width - deltaWidth, TextBounds.Height));
			DescriptionBounds = new Rectangle(DescriptionBounds.Location, new Size(DescriptionBounds.Width - deltaWidth, DescriptionBounds.Height));
		}
		protected virtual AppearanceObject GetNormalAppearance() { return ViewInfo.PaintAppearance.GetAppearance("ItemNormal"); }
		protected virtual int CalcTextBestWidth(Graphics graphics) {
			int textWidth = 0, descriptionWidth = 0;
			if(ViewInfo.WinExplorerView.OptionsView.AllowHtmlText) {
				textWidth = TextInfo != null ? TextInfo.Bounds.Width : 0;
				descriptionWidth = DescriptionInfo != null ? DescriptionInfo.Bounds.Width : 0;
			}
			else {
				AppearanceObject app = GetNormalAppearance();
				textWidth = (int)app.CalcTextSize(graphics, Text, 0).Width;
				if(ViewInfo.WinExplorerView.HasDescriptionField)
					descriptionWidth = (int)app.CalcTextSize(graphics, Description, 0).Width;
			}
			return Math.Max(textWidth, descriptionWidth);
		}
		protected virtual void CalcCheckBoxBounds(Rectangle checkBoxBounds) {
			CheckBoxBounds = checkBoxBounds;
			if(ViewInfo.AllowOverlappedCheckBox) {
				CheckBoxBounds = new Rectangle(ImageBounds.Location, ViewInfo.CheckBoxSize);
			}
		}
		protected virtual int ContentTextAndDescriptionIndent { get { return 5; } }
		protected virtual int DescriptionMinOffset {
			get {
				if(ViewInfo.WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Content)
					return ViewInfo.WinExplorerView.OptionsViewStyles.Content.DescriptionMinOffset;
				return 0;
			}
		}
		protected virtual int DescriptionMaxOffset {
			get {
				if(ViewInfo.WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Content)
					return ViewInfo.WinExplorerView.OptionsViewStyles.Content.DescriptionMaxOffset;
				return 0;
			}
		}
		protected virtual int DescriptionMinWidth {
			get {
				if(ViewInfo.WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Content)
					return ViewInfo.WinExplorerView.OptionsViewStyles.Content.DescriptionMinWidth;
				return 0;
			}
		}
		protected virtual void CalcTextAndDescriptionBounds(Rectangle textBounds) {
			TextBounds = textBounds;
			if(AllowDescription) {
				Rectangle desc = TextBounds;
				if(DescriptionLocation == DescriptionLocation.BelowText) {
					desc.Y += ViewInfo.TextHeight;
					desc.Height -= ViewInfo.TextHeight;
				}
				else {
					int offset = textBounds.Width - DescriptionMinWidth;
					offset = Math.Min(offset, DescriptionMaxOffset);
					offset = Math.Max(offset, DescriptionMinOffset);
					desc.X += offset;
					desc.Width -= (offset + ContentStyleSelectionOffset);
					desc.Height = Bounds.Bottom - desc.Y - ContentTextAndDescriptionIndent;
					if(textBounds.Right + ContentTextAndDescriptionIndent > desc.X) {
						textBounds.Width -= textBounds.Right - desc.X + ContentTextAndDescriptionIndent;
					}
				}
				textBounds.Height = ViewInfo.TextHeight;
				DescriptionBounds = desc;
				TextBounds = textBounds;
			}
		}
		protected virtual void CalcItemSeparatorBounds(Graphics graphics, Rectangle bounds) {
			SkinElementInfo elementInfo = new SkinElementInfo(CommonSkins.GetSkin(ViewInfo.View)[CommonSkins.SkinLabelLine], bounds);
			int height = ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, elementInfo).Height;
			ItemSeparatorBounds = new Rectangle(0, bounds.Bottom - height, bounds.Width, height);
		}
		protected virtual int ContentStyleSelectionOffset { get { return Bounds.X; } }
		protected internal virtual Rectangle CalcSelectionBounds() {
			Rectangle res = Bounds;
			bool shouldReleaseGraphics = false;
			if(ViewInfo.GInfo.Graphics == null) {
				ViewInfo.GInfo.AddGraphics(null);
				shouldReleaseGraphics = true;
			}
			WinExplorerViewStyle viewStyle = ViewInfo.WinExplorerView.OptionsView.Style;
			if(viewStyle == WinExplorerViewStyle.List) {
				res.Width -= Math.Max(0, TextBounds.Width - CalcTextBestWidth(ViewInfo.GInfo.Graphics));
			}
			else if(viewStyle == WinExplorerViewStyle.Content) {
				res.Width -= ContentStyleSelectionOffset;
				res.Height--;
			}
			else {
				if(!ViewInfo.WinExplorerView.HasTextField && ViewInfo.ItemViewOptions.SelectionDrawMode == SelectionDrawMode.AroundImage) {
					res = new Rectangle(ImageBounds.X, ImageContentBounds.Y, ImageBounds.Width, ImageContentBounds.Height);
					res.X -= ViewInfo.TextImagePanel.Content1Padding.Left;
					res.Width += ViewInfo.TextImagePanel.Content1Padding.Horizontal;
					res.Y -= ViewInfo.TextImagePanel.Content1Padding.Top;
					res.Height += ViewInfo.TextImagePanel.Content1Padding.Vertical;
				}
			}
			if(shouldReleaseGraphics)
				ViewInfo.GInfo.ReleaseGraphics();
			return res;
		}
		protected internal virtual void CalcTextInfo(Graphics graphics) {
			if(ViewInfo.WinExplorerView.OptionsView.AllowHtmlText && graphics != null) {
				if(!ShouldCalculateTextInfo && TextInfo != null) {
					TextInfo.SetLocation(TextBounds.Location);
					return;
				}
				TextInfo = StringPainter.Default.Calculate(graphics, ViewInfo.PaintAppearance.GetAppearance("ItemNormal"), Text, TextBounds);
				ShouldCalculateTextInfo = false;
			}
		}
		protected internal virtual void CalcDescriptionInfo(Graphics graphics) {
			if(!AllowDescription)
				return;
			if(ViewInfo.WinExplorerView.OptionsView.AllowHtmlText && graphics != null) {
				if(!ShouldCalculateDescriptionInfo) {
					DescriptionInfo.SetLocation(DescriptionBounds.Location);
					return;
				}
				DescriptionInfo = StringPainter.Default.Calculate(graphics, ViewInfo.PaintAppearance.GetAppearance("ItemDescriptionNormal"), Description, DescriptionBounds);
				ShouldCalculateDescriptionInfo = false;
			}
		}
		public virtual bool IsHovered {
			get { return ViewInfo.HoverInfo.InItem && ViewInfo.HoverInfo.ItemInfo.Row == Row; }
		}
		public virtual bool IsPressed {
			get { return ViewInfo.PressedInfo.InItem && ViewInfo.PressedInfo.ItemInfo.Row == Row; }
		}
		public bool IsFocused {
			get { return ViewInfo.FocusedRowHandle == Row.RowHandle; }
		}
		protected internal virtual Rectangle GetEditorBounds(WinExplorerViewColumnType itemEdit) {
			return itemEdit == WinExplorerViewColumnType.Text ? TextBounds : DescriptionBounds;
		}
		public virtual bool IsFullyVisible { get { return ViewInfo.Bounds.Contains(SelectionBounds); } }
		public virtual bool IsVisible { 
			get { 
				return ViewInfo.ClientBounds.IntersectsWith(SelectionBounds) || ViewInfo.ClientBounds.IntersectsWith(Bounds); 
			} 
		}
		protected internal virtual WinExplorerItemViewInfo Clone() {
			WinExplorerItemViewInfo item = new WinExplorerItemViewInfo(ViewInfo, Row);
			item.Bounds = Bounds;
			item.SelectionBounds = SelectionBounds;
			item.Column = Column;
			return item;
		}
		public virtual int SelectedImageIndex { get { return 4; } }
		public virtual int HoveredImageIndex { get { return 1; } }
		public virtual int PressedImageIndex { get { return 2; } }
		protected internal void CalcShowToolTip(Graphics g) {
			ShowToolTip = CalcShowToolTipCore(g);
		}
		bool CalcShowToolTipCore(Graphics g) {
			if(Description != string.Empty) return true;
			bool isCropped = false;
			if(g == null) return false;
			if(ViewInfo.WinExplorerView.OptionsView.AllowHtmlText) {
				if(!TextBounds.Contains(TextInfo.Bounds)) {
					return true;
				}
				SizeF size = ViewInfo.PaintAppearance.GetAppearance("ItemNormal").CalcTextSize(g, ViewInfo.PaintAppearance.GetAppearance("ItemNormal").GetStringFormat(), Text, TextBounds.Width, TextBounds.Height, out isCropped);
			}
			else {
				SizeF size = ViewInfo.GetTextAppearance(this).CalcTextSize(g, ViewInfo.GetTextAppearance(this).GetStringFormat(), Text, TextBounds.Width, TextBounds.Height, out isCropped);
			}
			return isCropped;
		}
		AnimatedImageHelper helper;
		protected AnimatedImageHelper AnimatedImageHelper {
			get {
				if(helper == null)
					helper = new AnimatedImageHelper(GetImage());
				return helper;
			}
		}
		protected internal void ClearAnimatedImageHelper() {
			helper = null;
		}
		Rectangle IAnimatedItem.AnimationBounds {
			get { return ImageBounds; }
		}
		int IAnimatedItem.AnimationInterval {
			get { return AnimatedImageHelper.AnimationInterval; }
		}
		int[] IAnimatedItem.AnimationIntervals {
			get { return AnimatedImageHelper.AnimationIntervals; }
		}
		AnimationType IAnimatedItem.AnimationType {
			get { return AnimatedImageHelper.AnimationType; }
		}
		int IAnimatedItem.FramesCount {
			get { return AnimatedImageHelper.FramesCount; }
		}
		int IAnimatedItem.GetAnimationInterval(int frameIndex) {
			return AnimatedImageHelper.GetAnimationInterval(frameIndex);
		}
		bool IAnimatedItem.IsAnimated {
			get { return AnimatedImageHelper.IsAnimated; }
		}
		void IAnimatedItem.OnStart() {
			throw new NotImplementedException();
		}
		void IAnimatedItem.OnStop() {
			throw new NotImplementedException();
		}
		object IAnimatedItem.Owner { 
			get { return this; }
		}
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) {
			if(info == null || info.IsFinalFrame)
				return;
			if(!((IAnimatedItem)this).IsAnimated)
				return;
			Image img = GetImage();
			if(img != AnimatedImageHelper.Image) {
				AnimatedImageHelper.Image = img;
				Image = GetImage();
				CalcImageBounds(ImageBounds);
				XtraAnimator.Current.Animations.Remove(info);
				ViewInfo.GridControl.Invalidate(Bounds);
				return;
			}
			img.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
		}
		protected internal AppearanceObjectEx GetAppearance(GridColumn column) {
			var res = formatInfo.GetCellAppearance(column);
			if(res != null) return res;
			return conditionInfo.GetCellAppearance(column);
		}
		protected internal AppearanceObjectEx GetDescriptionAppearance(GridColumn column) {
			return conditionInfo.GetCellDescriptionAppearance(column);
		}
		ConditionInfo IRowConditionFormatProvider.ConditionInfo {
			get { return conditionInfo; }
		}
		RowFormatRuleInfo IRowConditionFormatProvider.FormatInfo {
			get { return formatInfo; }
		}
		public virtual Size CalcBestSize() {
			int textWidth = CalcTextBestWidth(ViewInfo.GInfo.Graphics);
			return ViewInfo.CalcItemSize(textWidth);
		}
		ContextItemCollectionHandler contextButtonsHandler;
		protected internal ContextItemCollectionHandler ContextButtonsHandler {
			get {
				if(contextButtonsHandler == null)
					contextButtonsHandler = CreateContextButtonsHandler();
				return contextButtonsHandler;
			}
		}
		protected virtual ContextItemCollectionHandler CreateContextButtonsHandler() {
			return new ContextItemCollectionHandler(ContextButtonsViewInfo);
		}
		ContextItemCollectionViewInfo contextButtonsViewInfo;
		protected internal ContextItemCollectionViewInfo ContextButtonsViewInfo {
			get {
				if(contextButtonsViewInfo == null)
					contextButtonsViewInfo = new ContextItemCollectionViewInfo(((ISupportContextItems)this).ContextItems, ContextButtonOptions, this);
				return contextButtonsViewInfo;
			}
		}
		bool ShouldUseImageContentBoundsForContextButtons {
			get {
				if(ViewInfo.ItemViewOptions.SelectionDrawMode == SelectionDrawMode.AroundImage)
					return true;
				return false; 
			}
		}
		Rectangle ISupportContextItems.DisplayBounds {
			get { 
				if(ShouldUseImageContentBoundsForContextButtons)
					return new Rectangle(ImageBounds.X, ImageContentBounds.Y, ImageBounds.Width, ImageContentBounds.Height);
				return ImageBounds;
			}
		}
		Rectangle ISupportContextItems.DrawBounds {
			get { return Bounds; }
		}
		Rectangle ISupportContextItems.ActivationBounds {
			get { return SelectionBounds; }
		}
		ContextItemCollection ISupportContextItems.ContextItems {
			get { return ViewInfo.WinExplorerView.ContextButtons; }
		}
		Control ISupportContextItems.Control {
			get { return ViewInfo.GridControl; }
		}
		bool ISupportContextItems.DesignMode { get { return ViewInfo.View.IsDesignMode; } }
		ContextItemCollectionOptions ContextButtonOptions { get { return ViewInfo.WinExplorerView.ContextButtonOptions; } }
		bool ISupportContextItems.CloneItems { get { return true; } }
		void ISupportContextItems.RaiseCustomizeContextItem(ContextItem item) {
			ViewInfo.WinExplorerView.RaiseContextButtonCustomize(new WinExplorerViewContextButtonCustomizeEventArgs(item, Row.RowHandle));
		}
		void ISupportContextItems.RaiseContextItemClick(ContextItemClickEventArgs e) {
			e.DataItem = Row.RowHandle;
			ViewInfo.WinExplorerView.RaiseContextItemClick(e);
		}
		void ISupportContextItems.RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
			ViewInfo.WinExplorerView.RaiseCustomContextButtonToolTip(new WinExplorerViewContextButtonToolTipEventArgs(Row.RowHandle, e));
		}
		ItemHorizontalAlignment ISupportContextItems.GetCaptionHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemVerticalAlignment ISupportContextItems.GetCaptionVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		ItemHorizontalAlignment ISupportContextItems.GetGlyphHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemLocation ISupportContextItems.GetGlyphLocation(ContextButton btn) {
			return ItemLocation.Left;
		}
		int ISupportContextItems.GetGlyphToCaptionIndent(ContextButton btn) {
			return 3;
		}
		ItemVerticalAlignment ISupportContextItems.GetGlyphVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		UserLookAndFeel ISupportContextItems.LookAndFeel {
			get { return ViewInfo.WinExplorerView.LookAndFeel.ActiveLookAndFeel; }
		}
		bool ISupportContextItems.ShowOutsideDisplayBounds { get { return ((WinExplorerContextItemCollectionOptions)ContextButtonOptions).ShowOutsideDisplayBounds; } }
		ContextItemCollectionOptions ISupportContextItems.Options {
			get { return ContextButtonOptions; }
		}
		void ISupportContextItems.Redraw() {
			if(ViewInfo.GridControl == null || SuppressRedrawContextButtons) return;
			ViewInfo.GridControl.Invalidate();
		}
		void ISupportContextItems.Redraw(Rectangle rect) {
			if(ViewInfo.GridControl == null || SuppressRedrawContextButtons) return;
			ViewInfo.GridControl.Invalidate(rect);
		}
		void ISupportContextItems.Update() {
			if(ViewInfo.GridControl == null || SuppressRedrawContextButtons) return;
			ViewInfo.GridControl.Update();
		}
		public bool SuppressRedrawContextButtons { get; set; }
	}
	public class WinExplorerGroupViewInfo : WinExplorerItemViewInfo {
		public WinExplorerGroupViewInfo(WinExplorerViewInfo viewInfo, GridRow row)
			: base(viewInfo, row) {
		}
		protected override int CalcTextBestWidth(Graphics graphics) {
			if(ViewInfo.WinExplorerView.OptionsView.AllowHtmlText) {
				TextInfo = StringPainter.Default.Calculate(graphics, GetNormalAppearance(), Text, TextClientRect);
				ShouldCalculateTextInfo = false;
				return TextInfo != null ? TextInfo.Bounds.Width : 0;
			}
			else {
				AppearanceObject app = GetNormalAppearance();
				return (int)app.CalcTextSize(graphics, Text, 0).Width;
			}
		}
		protected internal override WinExplorerItemViewInfo Clone() {
			WinExplorerGroupViewInfo item = new WinExplorerGroupViewInfo(ViewInfo, Row);
			item.Bounds = Bounds;
			item.SelectionBounds = SelectionBounds;
			item.Column = Column;
			return item;
		}
		public override bool IsPressed {
			get {
				return ViewInfo.PressedInfo.HitTest == WinExplorerViewHitTest.GroupCaption && ViewInfo.PressedInfo.RowHandle == Row.RowHandle;
			}
		}
		public override bool IsFullyVisible { get { return ViewInfo.ClientBounds.Contains(Bounds); } }
		public override bool IsVisible { get { return ViewInfo.ClientBounds.IntersectsWith(Bounds); } }
		protected override AppearanceObject GetNormalAppearance() {
			return ViewInfo.PaintAppearance.GetAppearance("GroupNormal");
		}
		protected internal Rectangle TextClientRect { get; set; }
		protected internal bool ShouldLayoutText { get; set; }
		public virtual void ArrangeGroup(Rectangle bounds) {
			Bounds = bounds;
			SelectionBounds = CalcSelectionBounds();
			Rectangle clientRect = ViewInfo.CalcGroupCaptionClient(Bounds);
			if(ViewInfo.ShowExpandCollapseButtons) {
				CaptionButtonBounds = new Rectangle(new Point(clientRect.X, clientRect.Y + (clientRect.Height - ViewInfo.GroupCaptionButtonSize.Height)/2), ViewInfo.GroupCaptionButtonSize);
				clientRect.X = CaptionButtonBounds.Right + ViewInfo.GroupCaptionButton2TextIndent;
				clientRect.Width -= CaptionButtonBounds.Right + ViewInfo.GroupCaptionButton2TextIndent;
			}
			if(ViewInfo.ShowCheckBoxInGroupCaption) {
				CheckBoxBounds = new Rectangle(new Point(clientRect.X, clientRect.Y + (clientRect.Height - ViewInfo.CheckBoxSize.Height) / 2), ViewInfo.CheckBoxSize);
				clientRect.X = CheckBoxBounds.Right + ViewInfo.GroupCheckBox2TextIndent;
				clientRect.Width -= CheckBoxBounds.Width + ViewInfo.GroupCheckBox2TextIndent;
			}
			TextClientRect = clientRect;
			if(WindowsFormsSettings.GetIsRightToLeft(ViewInfo.GridControl)) {
				CaptionButtonBounds = WinExplorerViewInfoLayoutCalculatorBase.GetRtlBounds(CaptionButtonBounds, Bounds);
				CheckBoxBounds = WinExplorerViewInfoLayoutCalculatorBase.GetRtlBounds(CheckBoxBounds, Bounds);
				TextClientRect = WinExplorerViewInfoLayoutCalculatorBase.GetRtlBounds(TextClientRect, Bounds);
			}
			if(ViewInfo.GInfo.Graphics != null) {
				LayoutText(TextClientRect, ViewInfo.GInfo.Graphics);
				CalcTextInfo(ViewInfo.GInfo.Graphics);
				ShouldLayoutText = false;
			}
			else {
				ShouldLayoutText = true;
			}
		}
		protected internal virtual void LayoutText(Rectangle bounds, Graphics graphics) {
			int width = Math.Min(CalcTextBestWidth(graphics), bounds.Width);
			AppearanceObject obj = GetNormalAppearance();
			int lineHeight = ViewInfo.GetGroupCaptionLineHeight();
			Rectangle lineRect = new Rectangle(0, bounds.Y + (bounds.Height - lineHeight) / 2, bounds.Width - TextBounds.Width - ViewInfo.GroupCaptionButton2TextIndent, lineHeight);
			if(ViewInfo.ShowExpandCollapseButtons)
				lineRect.Width -= ViewInfo.GroupCaptionButtonSize.Width + ViewInfo.GroupCaptionButton2TextIndent;
			if(obj.TextOptions.HAlignment == HorzAlignment.Near || obj.TextOptions.HAlignment == HorzAlignment.Default) {
				TextBounds = new Rectangle(bounds.X, bounds.Y, width, bounds.Height);
				lineRect.X = TextBounds.Right + ViewInfo.GroupCaptionButton2TextIndent;
				lineRect.Width = bounds.Right - lineRect.X - ViewInfo.GroupCaptionButton2TextIndent;
			}
			else if(obj.TextOptions.HAlignment == HorzAlignment.Far) {
				TextBounds = new Rectangle(bounds.Right - width, bounds.Y, width, bounds.Height);
				lineRect.X = ViewInfo.ShowExpandCollapseButtons? bounds.X + ViewInfo.GroupCaptionButtonSize.Width + ViewInfo.GroupCaptionButton2TextIndent : bounds.X;
				lineRect.Width = TextBounds.X - lineRect.X - ViewInfo.GroupCaptionButton2TextIndent;
			}
			else {
				TextBounds = new Rectangle(bounds.X + (bounds.Width - width) / 2, bounds.Y, width, bounds.Height);
				lineRect = Rectangle.Empty;
			}
			if(WindowsFormsSettings.GetIsRightToLeft(ViewInfo.GridControl)) {
				TextBounds = WinExplorerViewInfoLayoutCalculatorBase.GetRtlBounds(TextBounds, bounds);
			}
			LineBounds = lineRect;
		}
		protected internal override void CalcTextInfo(Graphics graphics) {
			if(ViewInfo.WinExplorerView.OptionsView.AllowHtmlText && graphics != null) {
				if(!ShouldCalculateTextInfo) {
					TextInfo.SetLocation(TextBounds.Location);
					return;
				}
				TextInfo = StringPainter.Default.Calculate(graphics, GetNormalAppearance(), Text, TextBounds);
				ShouldCalculateTextInfo = false;
			}
		}
		protected internal override Rectangle CalcSelectionBounds() {
			Rectangle rect = Bounds;
			if(rect.Right > ViewInfo.Bounds.Width - ViewInfo.WinExplorerView.ScrollInfo.VScrollRect.Width)
				if(ViewInfo.ScrollBarPresence == ScrollBarPresence.Visible) {
					rect.Width -= rect.Right - ViewInfo.WinExplorerView.ScrollInfo.VScrollRect.X;
				}
			return rect;
		}
		public override void CalcFieldsInfo() {
			Text = ViewInfo.HasGroups ? (string)ViewInfo.WinExplorerView.GetGroupRowDisplayText(Row.RowHandle) : "";
		}
		public Rectangle CaptionButtonBounds { get; internal set; }
		public override bool IsGroupItem {
			get {
				return true;
			}
		}
		public override bool IsHovered {
			get { 
				return ViewInfo.HoverInfo.HitTest == WinExplorerViewHitTest.GroupCaption && ViewInfo.HoverInfo.ItemInfo.Row == Row; }
		}
		public virtual CheckState CheckState {
			get {
				CheckState state = System.Windows.Forms.CheckState.Indeterminate;
				ViewInfo.CheckedCache.TryGetValue(Row.RowHandle, out state);
				return state;
			}
			set {
				if(ViewInfo.CheckedCache.ContainsKey(Row.RowHandle)) {
					ViewInfo.CheckedCache[Row.RowHandle] = value;
				}
			}
		}
		public virtual bool Clicked { get; set; }
		public override bool AllowDrawCheckBox {
			get {
				return true;
			}
		}
	}
	public class WinExplorerItemInfoCollection : Collection<WinExplorerItemViewInfo> {
		public WinExplorerItemInfoCollection() : base() { }
	}
	public class WinExplorerViewSelectionInfo : BaseSelectionInfo {
		public WinExplorerViewSelectionInfo(BaseView view) : base(view) { }
		protected override int GetState() {
			return 0;
		}
	}
	public static class ImageLoadHelper {
		public static Image GetImage(int rowHandle, IWinExplorerAsyncImageLoaderClient viewInfo) {
			Image img = GetImageOriginal(rowHandle, viewInfo);
			if(img != null)
				return img;
			object imageIndex = GetImageIndexOriginal(rowHandle, viewInfo);
			if(imageIndex != null && imageIndex is int && (int)imageIndex != -1)
				return ImageCollection.GetImageListImage(viewInfo.Images, (int)imageIndex);
			if(!viewInfo.AllowReplaceableImages)
				return null;
			img = GetImageCore(rowHandle, viewInfo);
			if(img != null)
				return img;
			imageIndex = GetImageIndex(rowHandle, viewInfo);
			if(imageIndex != null && imageIndex is int && (int)imageIndex != -1)
				return ImageCollection.GetImageListImage(GetImages(viewInfo), (int)imageIndex);
			return null;
		}
		static object GetImages(IWinExplorerAsyncImageLoaderClient viewInfo) {
			WinExplorerViewStyle style = viewInfo.WinExplorerViewStyle;
			if(style == WinExplorerViewStyle.Small || style == WinExplorerViewStyle.List) {
				if(viewInfo.SmallImages != null)
					return viewInfo.SmallImages;
				style = WinExplorerViewStyle.Medium;
			}
			if(style == WinExplorerViewStyle.Medium || style == WinExplorerViewStyle.Tiles) {
				if(viewInfo.MediumImages != null)
					return viewInfo.MediumImages;
				style = WinExplorerViewStyle.Large;
			}
			if(style == WinExplorerViewStyle.Large) {
				if(viewInfo.LargeImages != null)
					return viewInfo.LargeImages;
				style = WinExplorerViewStyle.ExtraLarge;
			}
			if(style == WinExplorerViewStyle.ExtraLarge) {
				if(viewInfo.ExtraLargeImages != null)
					return viewInfo.ExtraLargeImages;
			}
			style = viewInfo.WinExplorerViewStyle;
			if(style == WinExplorerViewStyle.ExtraLarge) {
				if(viewInfo.ExtraLargeImages != null)
					return viewInfo.ExtraLargeImages;
				style = WinExplorerViewStyle.Large;
			}
			if(style == WinExplorerViewStyle.Large) {
				if(viewInfo.LargeImages != null)
					return viewInfo.LargeImages;
				style = WinExplorerViewStyle.Medium;
			}
			if(style == WinExplorerViewStyle.Medium || style == WinExplorerViewStyle.Tiles) {
				if(viewInfo.MediumImages != null)
					return viewInfo.MediumImages;
			}
			return viewInfo.SmallImages;
		}
		static bool IsCorrectImageIndexValue(object imageIndex) {
			return imageIndex != null && imageIndex is int && (int)imageIndex != -1;
		}
		static object GetImageIndex(int rowHandle, IWinExplorerAsyncImageLoaderClient viewInfo) {
			object imageIndex = null;
			WinExplorerViewStyle style = viewInfo.WinExplorerViewStyle;
			if(style == WinExplorerViewStyle.Small || style == WinExplorerViewStyle.List) {
				imageIndex = viewInfo.ColumnSet.HasExtraLargeImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.ExtraLargeImageIndexColumn) : -1;
				if(IsCorrectImageIndexValue(imageIndex))
					return imageIndex;
				style = WinExplorerViewStyle.Medium;
			}
			if(style == WinExplorerViewStyle.Medium ||
				style == WinExplorerViewStyle.Tiles) {
				imageIndex = viewInfo.ColumnSet.HasMediumImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.MediumImageIndexColumn) : -1;
				if(IsCorrectImageIndexValue(imageIndex))
					return imageIndex;
				style = WinExplorerViewStyle.Large;
			}
			if(style == WinExplorerViewStyle.Large) {
				imageIndex = viewInfo.ColumnSet.HasLargeImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.LargeImageIndexColumn) : -1;
				if(IsCorrectImageIndexValue(imageIndex))
					return imageIndex;
				style = WinExplorerViewStyle.ExtraLarge;
			}
			if(style == WinExplorerViewStyle.ExtraLarge) {
				imageIndex = viewInfo.ColumnSet.HasLargeImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.LargeImageIndexColumn) : -1;
				if(IsCorrectImageIndexValue(imageIndex))
					return imageIndex;
			}
			style = viewInfo.WinExplorerViewStyle;
			if(style == WinExplorerViewStyle.ExtraLarge) {
				imageIndex = viewInfo.ColumnSet.HasLargeImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.LargeImageIndexColumn) : -1;
				if(IsCorrectImageIndexValue(imageIndex))
					return imageIndex;
				style = WinExplorerViewStyle.Large;
			}
			if(style == WinExplorerViewStyle.Large) {
				imageIndex = viewInfo.ColumnSet.HasLargeImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.LargeImageIndexColumn) : -1;
				if(IsCorrectImageIndexValue(imageIndex))
					return imageIndex;
				style = WinExplorerViewStyle.Medium;
			}
			if(style == WinExplorerViewStyle.Medium || style == WinExplorerViewStyle.Tiles) {
				imageIndex = viewInfo.ColumnSet.HasMediumImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.MediumImageIndexColumn) : -1;
				if(IsCorrectImageIndexValue(imageIndex))
					return imageIndex;
			}
			return viewInfo.ColumnSet.HasExtraLargeImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.ExtraLargeImageIndexColumn) : -1;
		}
		static Image GetImageCore(int rowHandle, IWinExplorerAsyncImageLoaderClient viewInfo) {
			Image image = null;
			WinExplorerViewStyle style = viewInfo.WinExplorerViewStyle;
			if(style == WinExplorerViewStyle.Small ||
				style == WinExplorerViewStyle.List) {
				image = viewInfo.ColumnSet.HasSmallImageField ? GetCellImageCore(viewInfo.ColumnSet.SmallImageColumn, rowHandle, viewInfo) : null;
				if(image != null)
					return image;
				style = WinExplorerViewStyle.Medium;
			}
			if(style == WinExplorerViewStyle.Medium ||
				style == WinExplorerViewStyle.Tiles) {
				image = viewInfo.ColumnSet.HasMediumImageField ? GetCellImageCore(viewInfo.ColumnSet.MediumImageColumn, rowHandle, viewInfo) : null;
				if(image != null)
					return image;
				style = WinExplorerViewStyle.Large;
			}
			if(style == WinExplorerViewStyle.Large) {
				image = viewInfo.ColumnSet.HasLargeImageField ? GetCellImageCore(viewInfo.ColumnSet.LargeImageColumn, rowHandle, viewInfo) : null;
				if(image != null)
					return image;
				style = WinExplorerViewStyle.ExtraLarge;
			}
			if(style == WinExplorerViewStyle.ExtraLarge) {
				image = viewInfo.ColumnSet.HasExtraLargeImageField ? GetCellImageCore(viewInfo.ColumnSet.ExtraLargeImageColumn, rowHandle, viewInfo) : null;
				if(image != null)
					return image;
			}
			style = viewInfo.WinExplorerViewStyle;
			if(style == WinExplorerViewStyle.ExtraLarge) {
				image = viewInfo.ColumnSet.HasExtraLargeImageField ? GetCellImageCore(viewInfo.ColumnSet.ExtraLargeImageColumn, rowHandle, viewInfo) : null;
				if(image != null)
					return image;
				style = WinExplorerViewStyle.Large;
			}
			if(style == WinExplorerViewStyle.Large) {
				image = viewInfo.ColumnSet.HasLargeImageField ? GetCellImageCore(viewInfo.ColumnSet.LargeImageColumn, rowHandle, viewInfo) : null;
				if(image != null)
					return image;
				style = WinExplorerViewStyle.Medium;
			}
			if(style == WinExplorerViewStyle.Medium || style == WinExplorerViewStyle.Tiles) {
				image = viewInfo.ColumnSet.HasMediumImageField ? GetCellImageCore(viewInfo.ColumnSet.MediumImageColumn, rowHandle, viewInfo) : null;
				if(image != null)
					return image;
			}
			return viewInfo.ColumnSet.HasSmallImageField ? GetCellImageCore(viewInfo.ColumnSet.SmallImageColumn, rowHandle, viewInfo) : null;
		}
		static object GetImageIndexOriginal(int rowHandle, IWinExplorerAsyncImageLoaderClient viewInfo) {
			switch(viewInfo.WinExplorerViewStyle) {
				case WinExplorerViewStyle.ExtraLarge:
					return viewInfo.ColumnSet.HasExtraLargeImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.ExtraLargeImageIndexColumn) : -1;
				case WinExplorerViewStyle.Large:
					return viewInfo.ColumnSet.HasLargeImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.LargeImageIndexColumn) : -1;
				case WinExplorerViewStyle.Small:
				case WinExplorerViewStyle.List:
					return viewInfo.ColumnSet.HasSmallImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.SmallImageIndexColumn) : -1;
			}
			return viewInfo.ColumnSet.HasMediumImageIndexField ? viewInfo.GetRowCellValue(rowHandle, viewInfo.ColumnSet.MediumImageIndexColumn) : -1;
		}
		static Image GetImageOriginal(int rowHandle, IWinExplorerAsyncImageLoaderClient viewInfo) {
			switch(viewInfo.WinExplorerViewStyle) {
				case WinExplorerViewStyle.ExtraLarge:
					return viewInfo.ColumnSet.HasExtraLargeImageField ? GetCellImageCore(viewInfo.ColumnSet.ExtraLargeImageColumn, rowHandle, viewInfo) : null;
				case WinExplorerViewStyle.Large:
					return viewInfo.ColumnSet.HasLargeImageField ? GetCellImageCore(viewInfo.ColumnSet.LargeImageColumn, rowHandle, viewInfo) : null;
				case WinExplorerViewStyle.Small:
				case WinExplorerViewStyle.List:
					return viewInfo.ColumnSet.HasSmallImageField ? GetCellImageCore(viewInfo.ColumnSet.SmallImageColumn, rowHandle, viewInfo) : null;
			}
			return viewInfo.ColumnSet.HasMediumImageField ? GetCellImageCore(viewInfo.ColumnSet.MediumImageColumn, rowHandle, viewInfo) : null;
		}
		static Image GetCellImageCore(GridColumn column, int rowHandle, IWinExplorerAsyncImageLoaderClient viewInfo) {
			object rawData = viewInfo.GetRowCellValue(rowHandle, column);
			if(rawData is Image) return (Image)rawData;
			return ByteImageConverter.FromByteArray(ByteImageConverter.ToByteArray(rawData));
		}
	}
	public class WinExplorerAsyncImageLoader : AsyncImageLoader {
		protected IWinExplorerAsyncImageLoaderClient WinExplorerViewInfo {
			get {
				return ViewInfo as IWinExplorerAsyncImageLoaderClient;
			}
		}
		public WinExplorerAsyncImageLoader(IWinExplorerAsyncImageLoaderClient viewInfo)
			: base(viewInfo) {
		}
		protected override bool ShouldCacheThumbnails() {
			if(WinExplorerViewInfo == null) return false;
			return WinExplorerViewInfo.ShouldCacheThumbnails();
		}
		protected override bool ShouldLoadThumbnailImagesFromDataSource() {
			if(WinExplorerViewInfo == null) return false;
			return WinExplorerViewInfo.ShouldLoadThumbnailImagesFromDataSource();
		}
		protected override Image GetImageCore(int rowHandle) {
			if(WinExplorerViewInfo == null) return null;
			return ImageLoadHelper.GetImage(rowHandle, WinExplorerViewInfo);
		}
		public override bool IsRowLoaded(int rowHandle) {
			if(WinExplorerViewInfo == null)
				return true;
			return WinExplorerViewInfo.IsRowLoaded(rowHandle);
		}
	}
	public class WinExplorerSyncImageLoader : SyncImageLoader {
		public WinExplorerSyncImageLoader(IAsyncImageLoaderClient viewInfo) : base(viewInfo) { }
		protected IWinExplorerAsyncImageLoaderClient WinExplorerViewInfo {
			get {
				return ViewInfo as IWinExplorerAsyncImageLoaderClient;
			}
		}
		protected override Image GetImageCore(int rowHandle) {
			if(WinExplorerViewInfo == null) return null;
			return ImageLoadHelper.GetImage(rowHandle, WinExplorerViewInfo);
		}
	}
	public class WinExplorerImageShowingAnimationInfo : ImageShowingAnimationInfo {
		public WinExplorerImageShowingAnimationInfo(ISupportXtraAnimation anim, object animationId, RenderImageViewInfo imageInfo, int ms, int delay)
			: base(anim, animationId, imageInfo, ms, delay) {
		}
		protected override void Invalidate() {
			if(!(AnimatedObject is WinExplorerViewInfo)) return;
			WinExplorerViewInfo vi = (WinExplorerViewInfo)AnimatedObject;
			int rowHandle = vi.View.GetRowHandle(Item.LoadInfo.DataSourceIndex);
			WinExplorerItemViewInfo info = vi.GetItemInfo(rowHandle);
			if(info == null || !Item.LoadInfo.IsLoaded) return;
			vi.RefreshItemCore(info, IsFinalFrame);
		}
		protected override void FrameStepCore() {
			base.FrameStepCore();
			if(IsFinalFrame) {
				WinExplorerViewInfo vi = (WinExplorerViewInfo)AnimatedObject;
				WinExplorerItemViewInfo info = vi.GetItemInfo(Item.LoadInfo.RowHandle);
				if(info != null) {
					info.SelectionBounds = info.CalcSelectionBounds();
					info.ImageInfo.IsLoaded = true;
					info.ImageInfo.IsInAnimation = false;
					info.ImageInfo.IsAnimationEnd = true;
				}
			}
		}
	}
}
