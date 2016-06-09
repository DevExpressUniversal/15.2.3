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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Data.Summary;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Selection;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using WarningException = System.Exception;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using Visual = System.Windows.UIElement;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using FrameworkContentElement = System.Windows.DependencyObject;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
using ContentPresenter = DevExpress.Xpf.Core.XPFContentPresenter;
#else
using System.Timers;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class ScrollableCellsAreaPresenter : CellsAreaPresenter, IScrollInfo, IScrollingStrategyOwner {
#if !SL
		const int defaultScrollUpdateDelay = 9;
#else
		const int defaultScrollUpdateDelay = 50;
#endif
		ScrollViewer scrollOwner;
		IHorizontalScrollingStrategy horizontalScrollStrategy;
		Collection<ScrollableAreaPresenter> relatedScrollableAreas = new Collection<ScrollableAreaPresenter>();
		ScrollUpdateDelayer scrollDelayer;
		ScrollUpdateDelegate scrollUpdate;
		CellsSelectionScroller selectionScroller;
		ScrollableAreaCell mouseDownItem;
		bool isPixelScrolling = true;
		protected ScrollViewer ScrollOwner {
			get { return scrollOwner; }
		}
		public IHorizontalScrollingStrategy HorizontalScrollStrategy {
			get {
				if(horizontalScrollStrategy == null)
					CreateHorizontalScrollingStrategy();
				return horizontalScrollStrategy;
			}
		}
		public CellsSelectionScroller SelectionScroller {
			get {
				if(selectionScroller == null)
					selectionScroller = new CellsSelectionScroller(this);
				return selectionScroller;
			}
		}
		public ScrollableCellsAreaPresenter() : base() {
			scrollDelayer = new ScrollUpdateDelayer(UpdateScrolling, defaultScrollUpdateDelay);
			CreateScrollingManager(false);
			MouseRightButtonDown += OnMouseRightButtonDown;
			MouseRightButtonUp += OnMouseRightButtonUp;
			LostMouseCapture += OnLostMouseCapture;
			UseTimedScrolling(true);
		}
		protected override void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if(DevExpress.Xpf.Core.LayoutUpdatedHelper.GlobalLocker.IsLocked)
				return;
			EnsureTopLeft(e.NewSize.Width, e.NewSize.Height);
			base.OnSizeChanged(sender, e);
			if(e.PreviousSize.IsZero())
				OnItemsChanged();
			else
				InvalidateScrollOwner();
		}
		protected override ScrollableAreaCell CreateCell() {
			return new CellElement() { Template = ItemTemplate };
		}
		protected override void OnItemsChanged() {
			base.OnItemsChanged();
			Top = (int)OnTopChanging(Top);
			Left = (int)OnLeftChanging(Left);
			InvalidateScrollOwner();
		}
		protected override void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			StopScrollTimer();
			scrollDelayer.Stop();
			base.OnUnloaded(sender, e);
		}
		protected override void OnLeftTopChanged() {
			this.scrollUpdate();
			if(Data != null)
				Data.PivotGrid.SetLeftTopCoord(new System.Drawing.Point(Left, Top), new Point(LeftOffset, TopOffset));
		}
		public void UseTimedScrolling(bool use) {
			if(use)
				this.scrollUpdate = this.scrollDelayer.Invoke;
			else
				this.scrollUpdate = UpdateScrolling;
		}
		public void SynchronizeScrollingWith(ScrollableAreaPresenter scrollableAreaPresenter) {
			if(this.relatedScrollableAreas.Contains(scrollableAreaPresenter))
				return;
			this.relatedScrollableAreas.Add(scrollableAreaPresenter);
		}
		void CreateScrollingManager(bool value) {
			isPixelScrolling = value;
		}
		protected internal void EnsureScrollingMode() {
			bool isPixelScrolling = PivotGrid == null || PivotGrid.ScrollingMode == ScrollingMode.Pixel;
			CreateScrollingManager(isPixelScrolling);
			this.CoerceValue(TopOffsetProperty);
		}
		void CreateHorizontalScrollingStrategy() {
			EnsureScrollingMode();
			if(PivotGrid == null || PivotGrid.FixedRowHeaders)
				horizontalScrollStrategy = new FixedHorizontalScrollingStrategy(this);
			else
				horizontalScrollStrategy = new UnfixedHorizontalScrollingStrategy(this);
			Left = Left + RowsLeft;
			RowsLeft = 0;
		}
		public int GetMinVisibleLeft() {
			return GetMinVisibleLeft(MaxIndex);
		}
		int GetMinVisibleLeft(int index) {
			if(ActualWidth == 0 || IsChild)
				return int.MaxValue;
			double unusedWidth;
			bool fixedRowHeaders = PivotGrid.FixedRowHeaders;
			if(fixedRowHeaders) {
				unusedWidth = ActualWidth;
			} else {
				unusedWidth = ActualWidth + PivotGrid.PivotGridScroller.RowValues.ActualWidth;
			}
			int res = 0;
			for(int i = index; i >= 0 && unusedWidth > 0; i--) {
				unusedWidth -= GetWidth(i);
				res++;
			}
			if(!isPixelScrolling && unusedWidth < 0)
				res--;
			return Math.Max(0, index - Math.Max(res, 1) + 1);
		}
		public int GetMinVisibleTop() {
			return GetMinVisibleTop(MaxLevel);
		}
		int GetMinVisibleTop(int index) {
			if(ActualHeight == 0 || IsChild)
				return int.MaxValue;
			double unusedHeight = ActualHeight;
			int res = 0;
			for(int i = index; i >= 0 && unusedHeight > 0; i--) {
				unusedHeight -= GetHeight(i);
				res++;
			}
			if(!isPixelScrolling && unusedHeight < 0)
				res--;
			return Math.Max(0, index - Math.Max(res, 1) + 1);
		}
		protected override object OnLeftChanging(object baseValue) {
			int newValue = (int)baseValue;
			bool fixedRowHeaders = PivotGrid.FixedRowHeaders;
			if(newValue < 0) {
				int newRowsLeft = RowsLeft + newValue;
				if(newRowsLeft < 0) {
					newRowsLeft = 0;
					RowsLeftOffset = 0;
				}
				RowsLeft = newRowsLeft;
				LeftOffset = 0;
			}
			if(newValue > GetMinVisibleLeft(MaxIndex)) {
				newValue = ScrollToRightBorder(MaxIndex);
			}
			if((int)baseValue < 0) {
				newValue = 0;
			}
			if(RowsLeft > 0 || RowsLeftOffset > 0 || LeftOffset > 0) {
				double diff = ActualWidth - VisualItems.GetWidthDifference(newValue, MaxIndex + 1, true) + LeftOffset;
				if(diff > 0 && !PivotGrid.FixedRowHeaders)
					diff += PivotGrid.PivotGridScroller.RowValues.ActualWidth - VisualItems.GetWidthDifference(RowsLeft, VisualItems.GetLevelCount(false), false) + RowsLeftOffset;
				if(diff > 0)
					newValue = ScrollToRightBorder(MaxIndex);
			}
			return newValue;
		}
		protected override object OnTopChanging(object baseValue) {
			int newValue = (int)baseValue;
			if(newValue >= GetMinVisibleTop(MaxLevel))
				newValue = ScrollToBottomBorder(MaxLevel);
			if(newValue < 0) {
				newValue = 0;
				TopOffset = 0d;
			}
			return newValue;
		}
		int ScrollToBottomBorder(int index) {
			int newTop = GetMinVisibleTop(index);
			if(isPixelScrolling) {
				if(newTop == 0 && index == 0) {
					TopOffset = 0;
				} else {
					double diff = VisualItems.GetHeightDifference(newTop, index + 1, false) - ActualHeight;
					while(diff < 0 && newTop > 0)
						diff = VisualItems.GetHeightDifference(--newTop, index + 1, false) - ActualHeight;
					TopOffset = diff;
				}
			}
			return newTop;
		}
		int ScrollToRightBorder(int index) {
			return GetLeftHorizontalPosition(GetRightBorderOffset(index));
		}
		int GetRightBorderOffset(int level) {
			double fullWidth = ActualWidth;
			bool fixedRowHeaders = PivotGrid.FixedRowHeaders;
			if(!fixedRowHeaders)
				fullWidth += PivotGrid.PivotGridScroller.RowValues.ActualWidth;
			if(isPixelScrolling) {
				int widthToRightBorder = VisualItems.GetWidthDifference(0, level + 1, true);
				if(!fixedRowHeaders)
					widthToRightBorder += VisualItems.GetWidthDifference(0, VisualItems.GetLevelCount(false), false);
				return Math.Max(0, Convert.ToInt32(widthToRightBorder - fullWidth));
			} else {
				int newRowsLeft = VisualItems.GetLevelCount(false);
				int newLeft = level;
				fullWidth -= GetWidth(newLeft);
				int diff = GetWidth(Math.Max(0, newLeft - 1));
				while(fullWidth > diff && newLeft != 0) {
					fullWidth -= diff;
					newLeft--;
					if(newLeft > 0)
						diff = GetWidth(newLeft - 1);
				}
				if(!fixedRowHeaders) {
					diff = VisualItems.GetWidthDifference(newRowsLeft - 1, newRowsLeft, false);
					while(fullWidth > diff && newRowsLeft != 0) {
						fullWidth -= diff;
						newRowsLeft--;
						if(newRowsLeft > 0)
							diff = VisualItems.GetWidthDifference(newRowsLeft - 1, newRowsLeft, false);
					}
					RowsLeft = newRowsLeft;
				}
				return newLeft + RowsLeft;
			}
		}
		protected override object OnLeftTopOffsetChanging(object baseValue) {
			return isPixelScrolling ? Math.Max(0d, (double)baseValue) : 0d;
		}
		protected internal void EnsureHorizontalScrollingMode() {
			CreateHorizontalScrollingStrategy();
		}
		protected void InvalidateScrollOwner() {
			if(ScrollOwner != null) {
				ScrollOwner.InvalidateScrollInfo();
#if SL
				IScrollInfo info = this;
				Visibility needH = info.ExtentHeight > info.ViewportHeight ? Visibility.Visible : Visibility.Collapsed;
				Visibility needW = info.ExtentWidth > info.ViewportWidth ? Visibility.Visible : Visibility.Collapsed;
				if(needH != scrollOwner.ComputedVerticalScrollBarVisibility || needW != scrollOwner.ComputedHorizontalScrollBarVisibility)
					ScrollOwner.InvalidateMeasure();
#endif
			}
		}
		protected override bool OnReceiveWeakEvent(Type managerType, EventArgs e) {
			bool res = base.OnReceiveWeakEvent(managerType, e);
			if(res)
				return res;
			if(managerType == typeof(GridDataCellMadeVisibleEventManager)) {
				MakeCellVisible(((CellMadeVisibleEventArgs)e).Cell);
				return true;
			}
			return false;
		}
		internal void MakeCellVisible(System.Drawing.Point cell) {
			if(!VisualItems.IsCellValid(cell))
				return;
			EnsureItems();
			MakeCellLeftTopVisible(false, cell);
			MakeCellLeftTopVisible(true, cell);
		}
		void MakeCellLeftTopVisible(bool isTop, System.Drawing.Point cell) {
			int curLeftTop, result, newLeftTop, visibleCount;
			double offset;
			if(isTop) {
				result = curLeftTop = Top;
				newLeftTop = cell.Y;
				offset = TopOffset;
				visibleCount = GetViewPortHeight(ActualHeight, false);
			} else {
				result = curLeftTop = Left;
				newLeftTop = cell.X;
				offset = LeftOffset;
				visibleCount = GetViewPortWidthCore(ActualWidth, false);
			}
			if(newLeftTop <= curLeftTop)
				result = Math.Max(0, newLeftTop + (offset > 0 ? -1 : 0));
			if(visibleCount == 0)
				visibleCount = 1;
			if(newLeftTop >= curLeftTop + visibleCount) {
				if(isTop)
					result = ScrollToBottomBorder(newLeftTop);
				else
					result = ScrollToRightBorder(newLeftTop);
			}
			if(isTop) {
				Top = result;
				if(cell.Y == 0)
					TopOffset = 0;
			} else {
				Left = result;
				if(cell.X == 0)
					LeftOffset = 0;
			}
		}
		void UpdateScrolling() {
			OnLeftTopChangedCore();
			foreach(ScrollableAreaPresenter item in relatedScrollableAreas) {
				if(item.ScrollingType == ScrollingType.Vertical) {
					bool invalidateMeasure = item.Left != RowsLeft || item.LeftOffset != RowsLeftOffset;
					item.Top = Top;
					item.TopOffset = TopOffset;
					item.Left = RowsLeft;
					item.LeftOffset = RowsLeftOffset;
					if(invalidateMeasure)
						item.InvalidateMeasure();
				}
				if(item.ScrollingType == ScrollingType.Horizontal) {
					item.Left = Left;
					item.LeftOffset = LeftOffset;
				}
				item.OnLeftTopChangedCore();
			}
			this.RaiseEvent(new RoutedEventArgs(LeftTopChangedEvent));
			InvalidateScrollOwner();
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			if(VisualItems != null) {
				VisualItemsFocusedCellChangedEventManager.AddListener(VisualItems, this);
				VisualItemsSelectionChangedEventManager.AddListener(VisualItems, this);
			}
			if(Data != null)
				GridDataCellMadeVisibleEventManager.AddListener(Data, this);
			if(PivotGrid != null) {
				PivotGrid.MouseDoubleClick += OnPivotGridDoubleClick;
			}
		}
		protected override void OnPivotBrushesChanged(object sender, PivotBrushChangedEventArgs e) {
			if(e.BrushType != PivotBrushType.CellBrush)
				return;
			base.OnPivotBrushesChanged(sender, e);
		}
		protected override void UnsubscribeEvents() {
			if(VisualItems != null) {
				VisualItemsFocusedCellChangedEventManager.RemoveListener(VisualItems, this);
				VisualItemsSelectionChangedEventManager.RemoveListener(VisualItems, this);
			}
			if(Data != null)
				GridDataCellMadeVisibleEventManager.RemoveListener(Data, this);
			if(PivotGrid != null)
				PivotGrid.MouseDoubleClick -= OnPivotGridDoubleClick;
			base.UnsubscribeEvents();
		}
		#region IScrollInfo Members
		bool IScrollInfo.CanHorizontallyScroll { get { return true; } set { } }
		bool IScrollInfo.CanVerticallyScroll { get { return true; } set { } }
		double IScrollInfo.ExtentHeight {
			get { return isPixelScrolling ? VisualItems.GetHeightDifference(0, MaxLevel + 1, false) : MaxLevel + 1; }
		}
		double IScrollInfo.ExtentWidth {
			get {
				if(isPixelScrolling) {
					if(PivotGrid.FixedRowHeaders)
						return VisualItems.GetWidthDifference(0, VisualItems.ColumnCount, true);
					else
						return VisualItems.GetWidthDifference(0, VisualItems.ColumnCount, true) + VisualItems.GetWidthDifference(0, VisualItems.GetLevelCount(false), false);
				} else {
					return HorizontalScrollStrategy.ExtendWidth;
				}
			}
		}
		double IScrollInfo.HorizontalOffset {
			get {
				if(isPixelScrolling)
					return VisualItems.GetWidthDifference(0, Left, true) + VisualItems.GetWidthDifference(0, RowsLeft, false) + PivotGrid.PivotGridScroller.RowValues.LeftOffset + LeftOffset;
				else
					return Left + RowsLeft;
			}
		}
		void IScrollInfo.LineDown() {
			LinesDown(1);
		}
		void IScrollInfo.LineLeft() {
			LinesRight(-1);
		}
		void IScrollInfo.LineRight() {
			LinesRight(1);
		}
		void IScrollInfo.LineUp() {
			LinesDown(-1);
		}
#if !SL
		Rect IScrollInfo.MakeVisible(System.Windows.Media.Visual visual, Rect rectangle) {
#else
		Rect IScrollInfo.MakeVisible(UIElement visual, Rect rectangle) {
#endif
			return rectangle;
		}
		void IScrollInfo.MouseWheelDown() {
			LinesDown(System.Windows.SystemParameters.WheelScrollLines);
		}
		void IScrollInfo.MouseWheelLeft() {
			LinesRight(-System.Windows.SystemParameters.WheelScrollLines);
		}
		void IScrollInfo.MouseWheelRight() {
			LinesRight(System.Windows.SystemParameters.WheelScrollLines);
		}
		void IScrollInfo.MouseWheelUp() {
			LinesDown(-System.Windows.SystemParameters.WheelScrollLines);
		}
		void IScrollInfo.PageDown() {
			LinesDown(GetViewPortHeight(ActualHeight, false));
		}
		void IScrollInfo.PageLeft() {
			LinesRight(-GetViewPortWidth(ActualWidth, false));
		}
		void IScrollInfo.PageRight() {
			LinesRight(GetViewPortWidth(ActualWidth, false));
		}
		void IScrollInfo.PageUp() {
			LinesDown(-GetViewPortHeight(ActualHeight, false));
		}
		ScrollViewer IScrollInfo.ScrollOwner {
			get { return scrollOwner; }
			set { scrollOwner = value; }
		}
		void IScrollInfo.SetHorizontalOffset(double offset) {
			Left = GetLeftHorizontalPosition(offset);
		}
		int GetLeftHorizontalPosition(double offset) {
			offset = Math.Min(((IScrollInfo)this).ExtentWidth, Math.Max(0, offset));
			int newLeft;
			if(isPixelScrolling) {
				double toScrollOffset = offset;
				bool decrement = true;
				bool scroll = true;
				if(!PivotGrid.FixedRowHeaders) {
					int rowLevelsCount = VisualItems.GetLevelCount(false);
					int rowValuesScroll = 0;
					double rowWidth = VisualItems.GetWidthDifference(rowValuesScroll, rowValuesScroll + 1, false);
					while(rowValuesScroll != rowLevelsCount && decrement) {
						if(rowWidth > toScrollOffset) {
							decrement = false;
						} else {
							toScrollOffset -= rowWidth;
							rowValuesScroll++;
						}
						rowWidth = VisualItems.GetWidthDifference(rowValuesScroll, rowValuesScroll + 1, false);
					}
					scroll = rowValuesScroll == rowLevelsCount;
					RowsLeftOffset = !scroll ? toScrollOffset : 0;
					RowsLeft = rowValuesScroll;
				}
				int fullRowsToScroll = 0;
				if(scroll && decrement) {
					int rowWidth = VisualItems.GetWidthDifference(fullRowsToScroll, fullRowsToScroll + 1, true);
					while(decrement) {
						if(rowWidth > toScrollOffset) {
							decrement = false;
						} else {
							toScrollOffset -= rowWidth;
							fullRowsToScroll++;
						}
						rowWidth = VisualItems.GetWidthDifference(fullRowsToScroll, fullRowsToScroll + 1, true);
					}
				}
				LeftOffset = scroll ? toScrollOffset : 0;
				newLeft = fullRowsToScroll;
			} else {
				if(PivotGrid.FixedRowHeaders) {
					newLeft = (int)offset;
				} else {
					RowsLeft = Math.Min((int)offset, VisualItems.GetLevelCount(false));
					newLeft = Math.Max(0, (int)offset - RowsLeft);
				}
			}
			return newLeft;
		}
		void IScrollInfo.SetVerticalOffset(double offset) {
			offset = Math.Min(((IScrollInfo)this).ExtentHeight, Math.Max(0, offset));
			if(isPixelScrolling) {
				int fullRowsToScroll = 0;
				double toScrollOffset = offset;
				double rowHeight = VisualItems.GetHeightDifference(fullRowsToScroll, fullRowsToScroll + 1, false);
				while(toScrollOffset >= rowHeight) {
					fullRowsToScroll++;
					toScrollOffset -= rowHeight;
					rowHeight = VisualItems.GetHeightDifference(fullRowsToScroll, fullRowsToScroll + 1, false);
				}
				TopOffset = toScrollOffset;
				Top = fullRowsToScroll;
			} else
				Top = (int)offset;
		}
		double IScrollInfo.VerticalOffset {
			get { return isPixelScrolling ? VisualItems.GetHeightDifference(0, Top, false) + TopOffset : Top; }
		}
		double IScrollInfo.ViewportHeight {
			get { return isPixelScrolling ? ActualHeight : GetViewPortHeight(ActualHeight, false); }
		}
		double IScrollInfo.ViewportWidth {
			get {
				if(!isPixelScrolling) {
					double ww = GetViewPortWidth(ActualWidth, false);
					if(IsInLayoutCicle(this, ww))
						ww--;
					return ww;
				} else {
					if(PivotGrid.FixedRowHeaders)
						return ActualWidth;
					else
						return PivotGrid.PivotGridScroller.RowValues.ActualWidth + ActualWidth;
				}
			}
		}
		bool IsInLayoutCicle(IScrollInfo info, double ww) {
			if(ScrollOwner == null || ww != info.ExtentWidth || ScrollOwner.ComputedVerticalScrollBarVisibility != System.Windows.Visibility.Visible)
				return false;
			ScrollBar scroll = VerticalScrollBar;
			Border parent = ParentContainerBorder;
			if(scroll == null || parent == null || GetViewPortWidth(parent.ActualWidth - scroll.ActualWidth, false) + 1 != info.ExtentWidth)
				return false;
			if(GetViewPortHeight(ActualHeight + scroll.ActualWidth, false) == info.ExtentHeight)
				return true;
			return false;
		}
		Border ParentContainerBorder {
			get { return LayoutHelper.FindParentObject<Border>(this); }
		}
		ScrollBar VerticalScrollBar {
			get {
				if(PivotGrid == null)
					return null;
				if(PivotGrid.PivotGridScroller == null)
					return null;
				return PivotGrid.PivotGridScroller.GetVerticalScrollBar();
			}
		}
		protected void LinesDown(int count) {
			Top += count;
		}
		protected void LinesRight(int count) {
			int newOffset = Left + RowsLeft + count;
			int newLeft;
			if(!PivotGrid.FixedRowHeaders) {
				if(newOffset < 0) {
					newOffset = 0;
					RowsLeftOffset = 0;
					LeftOffset = 0;
				}
				int maxRowsLeft = VisualItems.GetLevelCount(false);
				RowsLeft = Math.Min(VisualItems.GetLevelCount(false), newOffset);
				newLeft = newOffset - RowsLeft;
				if(newLeft < 0 || RowsLeft != maxRowsLeft) {
					newLeft = 0;
					LeftOffset = 0;
				}
			} else
				newLeft = newOffset;
			Left = newLeft;
		}
		#endregion
		#region IScrollStrategyOwner Members
		int IScrollingStrategyOwner.LeftLevelCount {
			get { return Data.VisualItems.GetLevelCount(false); }
		}
		int IScrollingStrategyOwner.CellsExtendWidth {
			get { return MaxIndex + 1; }
		}
		int IScrollingStrategyOwner.LeftVisibleCount {
			get { return PivotGrid.PivotGridScroller.RowValues.GetViewPortWidthCore(PivotGrid.PivotGridScroller.RowValues.ActualWidth, false); }
		}
		bool IScrollingStrategyOwner.IsLastCellFullyVisible {
			get { return GetMinVisibleLeft() == 0 && MaxIndex == 0 && VisualItems.GetWidthDifference(0, 2, true) <= ActualWidth; }
		}
		protected internal override int GetViewPortWidth(double actualWidth, bool allowPartiallyVisible) {
			int width = base.GetViewPortWidth(actualWidth, allowPartiallyVisible);
			return HorizontalScrollStrategy.GetViewPortWidth(width);
		}
		protected override int LeftCoerce(int newValue) {
			int oldRowsLeft = RowsLeft;
			int value = HorizontalScrollStrategy.LeftCoerce(newValue);
			if(value < 0)
				value = 0;
			if(Left == value && oldRowsLeft != RowsLeft)
				OnLeftTopChanged();
			return value;
		}
		#endregion
		#region mouse iteraction
		void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			mouseDownItem = GetItemBySourceOrCoord(e.OriginalSource, e.GetPosition(this), false, false, true);
		}
		void RaiseCellClick(object source, MouseButton button, Point position) {
			ScrollableAreaCell mouseUpItem = GetItemBySourceOrCoord(source, position, false, false, true);
			if(mouseDownItem == null || mouseUpItem != mouseDownItem || PivotGrid == null)
				return;
			CellsAreaItem item = (CellsAreaItem)mouseUpItem.ValueItem;
			if(item == null)
				return;
			PivotGrid.RaiseCellClick(mouseUpItem, item.Item, button);
		}
		protected internal override void OnMouseLeftButtonDown(object originalSource, Point position) {
			base.OnMouseLeftButtonDown(originalSource, position);
			mouseDownItem = GetItemBySourceOrCoord(originalSource, position, false, false, true);
			ScrollableAreaCell cell = mouseDownItem;
			if(cell == null)
				cell = GetItemBySourceOrCoord(originalSource, position, true, false, true);
			CellsAreaItem item = cell != null ? (CellsAreaItem)cell.ValueItem : null;
			if(item != null)
				VisualItems.OnCellMouseDown(new System.Drawing.Point(item.ColumnIndex, item.RowIndex));
		}
#if SL
		CellsAreaItem lastClickedItem = null, clickedItem = null;
#endif
		protected internal override void OnMouseLeftButtonUp(object originalSource, Point position) {
			base.OnMouseLeftButtonUp(originalSource, position);
			VisualItems.OnCellMouseUp();
			SelectionScroller.StopScrollTimer();
			RaiseCellClick(originalSource, MouseButton.Left, position);
#if SL
			lastClickedItem = clickedItem;
			clickedItem = GetValueItemBySourceOrCoord(originalSource, position, false, false, true) as CellsAreaItem;
#endif
		}
		protected virtual void OnLostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e) {
			SelectionScroller.StopScrollTimer();
			mouseDownItem = null;
		}
		protected internal override void OnMouseMove(object originalSource, Point position) {
			base.OnMouseMove(originalSource, position);
			if(!VisualItems.IsCellMouseDown)
				return;
			CellsAreaItem item = GetValueItemBySourceOrCoord(originalSource, position, true, true, false) as CellsAreaItem;
			if(item != null)
				VisualItems.OnCellMouseMove(new System.Drawing.Point(item.ColumnIndex, item.RowIndex));
			if(IsMouseCaptured)
				SelectionScroller.StartScrollTimer(new System.Drawing.Point((int)(position.X + .5), (int)(position.Y + .5)));
		}
		void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			RaiseCellClick(sender, MouseButton.Right, e.GetPosition(this));
		}
		void StopScrollTimer() {
			if(this.selectionScroller != null)
				this.selectionScroller.StopScrollTimer();
		}
		protected virtual void OnPivotGridDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			object source;
#if SL
			source = e.OriginalSource;
#else
			source = GetSourceFromEventArgs(e);
#endif
			ScrollableAreaCell cell = GetItemBySourceOrCoord(source, e.GetPosition(this), false, false, true);
			if(cell == null)
				return;
			CellsAreaItem item = cell.ValueItem as CellsAreaItem;
			if(item == null
#if SL
				 || item != lastClickedItem
#endif
)
				return;
			e.Handled = PivotGrid.RaiseCellDblClick(cell, item.Item,
#if !SL
 e.ChangedButton
#else
				MouseButton.Left
#endif
);
		}
		#endregion
	}
}
