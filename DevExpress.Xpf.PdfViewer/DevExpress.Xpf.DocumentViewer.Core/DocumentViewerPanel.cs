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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.DocumentViewer {
	public class DocumentViewerInfiniteHeightException : ApplicationException {
		public DocumentViewerInfiniteHeightException(string message)
			: base(message) {
		}
	}
	public enum ScrollIntoViewMode {
		Center,
		TopLeft,
		Edge,
		BringIntoView,
	}
	public class IndexCalculator {
		protected IDictionary<int, Size> ItemSizes { get; set; }
		readonly DocumentViewerPanel documentViewerPanel;
		protected internal DocumentViewerPanel Panel { get { return documentViewerPanel; } }
		public IndexCalculator(DocumentViewerPanel panel) {
			ItemSizes = new Dictionary<int, Size>();
			documentViewerPanel = panel;
		}
		public double IndexToVerticalOffset(int index, bool showSingleItem) {
			if (showSingleItem) {
				double k = Panel.ExtentHeight / (Panel.ExtentHeight - Panel.ViewportHeight);
				if (GetRealItemSize(index).Height.LessThanOrClose(Panel.ViewportHeight))
					return IndexToVerticalOffset(index, false);
				return IndexToVerticalOffset(index, false) / k;
			}
			double offset = 0d;
			for (int i = 0; i < index; ++i) {
				if (!HasItemSize(i))
					return double.PositiveInfinity;
				offset += GetItemSize(i).Height;
			}
			return offset;
		}
		public int VerticalOffsetToIndex(double offset) {
			double currentOffset = offset;
			int currentIndex = 0;
			while (HasItemSize(currentIndex) && currentOffset.GreaterThanOrClose(0d))
				currentOffset -= GetItemSize(currentIndex++).Height;
			if (currentOffset.AreClose(0d))
				return currentIndex;
			if (currentOffset.GreaterThan(0d))
				return -1;
			return currentIndex - 1;
		}
		public void SetItemSize(int index, Size size) {
			if (ItemSizes.ContainsKey(index))
				ItemSizes[index] = size;
			else
				ItemSizes.Add(index, size);
		}
		public Size GetRealItemSize(int index) {
			return HasItemSize(index) ? ItemSizes[index] : Size.Empty;
		}
		public Size GetItemSize(int index) {
			if (!HasItemSize(index))
				return Size.Empty;
			return Panel.ActualShowSingleItem && Panel.ViewportHeight.GreaterThanOrClose(ItemSizes[index].Height) ?
				new Size(ItemSizes[index].Width, Panel.ViewportHeight) :
				ItemSizes[index];
		}
		public bool HasItemSize(int index) {
			return ItemSizes.ContainsKey(index);
		}
	}
	public class DocumentViewerPanel : VirtualizingPanel, IScrollInfo {
		public static readonly DependencyProperty ShowSingleItemProperty;
		public IndexCalculator IndexCalculator { get; private set; }
		public new UIElementCollection InternalChildren { get { return base.InternalChildren; } }
		public DocumentViewerPanel() {
			IndexCalculator = CreateIndexCalculator();
		}
		static DocumentViewerPanel() {
			Type ownerType = typeof(DocumentViewerPanel);
			ShowSingleItemProperty = DependencyPropertyRegistrator.Register<DocumentViewerPanel, bool>(owner => owner.ShowSingleItem, false, (d, oldValue, newValue) => d.OnShowSingleItemChanged(newValue));
		}
		public bool ShowSingleItem {
			get { return (bool)GetValue(ShowSingleItemProperty); }
			set { SetValue(ShowSingleItemProperty, value); }
		}
		public bool ActualShowSingleItem { get; private set; }
		void UpdateScrollData(Size viewportSize) {
			if (viewportSize.Height.IsNotNumber())
				throw new DocumentViewerInfiniteHeightException(DocumentViewerLocalizer.GetString(DocumentViewerStringId.DocumentViewerInfiniteHeightExceptionMessage));
			Size newViewportSize = viewportSize;
			Size newExtentSize = CalcExtentSize();
			InvalidateScrollInfo(newViewportSize, newExtentSize);
		}
		Size CalcExtentSize() {
			int itemsCount = ItemsControl.GetItemsOwner(this).Items.Count;
			var items = ItemsControl.GetItemsOwner(this).Items;
			double width = 0d;
			double height = 0d;
			for (int i = 0; i < itemsCount; ++i) {
				Size itemSize = ((PageWrapper)items[i]).RenderSize;
				IndexCalculator.SetItemSize(i, itemSize);
				width = Math.Max(itemSize.Width, width);
				height += itemSize.Height;
			}
			if (ShowSingleItem)
				height = Math.Max(ViewportHeight * itemsCount, height);
			return new Size(width, height);
		}
		void CleanUpItems(int startIndex, int endIndex) {
			var children = InternalChildren;
			var generator = ItemContainerGenerator;
			int childrenCount = children.Count;
			for (int i = childrenCount - 1; i >= 0; --i) {
				GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
				int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
				if (itemIndex < startIndex || itemIndex > endIndex) {
					generator.Remove(childGeneratorPos, 1);
					RemoveInternalChildRange(i, 1);
				}
			}
		}
		internal double CalcStartPosition(bool showSingleItem) {
			int index = IndexCalculator.VerticalOffsetToIndex(showSingleItem ? GetVirtualVerticalOffset() : VerticalOffset);
			if (showSingleItem) {
				double itemHeight = IndexCalculator.GetRealItemSize(index).Height;
				if (itemHeight.LessThanOrClose(ViewportHeight))
					return (ViewportHeight - itemHeight) / 2d;
				double itemOffset = IndexCalculator.IndexToVerticalOffset(index, false);
				double k = (GetVirtualVerticalOffset() - itemOffset) / itemHeight;
				double itemDiff = itemHeight - ViewportHeight;
				return -itemDiff * Math.Abs(k);
			}
			double offset = VerticalOffset;
			for (int i = 0; i < index; ++i)
				offset -= IndexCalculator.GetRealItemSize(i).Height;
			return -offset;
		}
		void CalcVisibleItemsRange(double viewportHeight, out int firstIndex, out int lastIndex) {
			if (ShowSingleItem) {
				firstIndex = IndexCalculator.VerticalOffsetToIndex(GetVirtualVerticalOffset());
				lastIndex = firstIndex;
				return;
			}
			int itemsCount = ItemsControl.GetItemsOwner(this).Items.Count;
			if (itemsCount == 0) {
				firstIndex = 0;
				lastIndex = 0;
				return;
			}
			int offsetIndex = IndexCalculator.VerticalOffsetToIndex(VerticalOffset);
			viewportHeight += VerticalOffset - IndexCalculator.IndexToVerticalOffset(offsetIndex, ShowSingleItem);
			firstIndex = offsetIndex < 0 ? 0 : offsetIndex;
			lastIndex = itemsCount - 1;
			for (int i = firstIndex; i < lastIndex; ++i) {
				if (viewportHeight.LessThanOrClose(0d)) {
					lastIndex = i;
					return;
				}
				viewportHeight -= IndexCalculator.GetRealItemSize(i).Height;
			}
		}
		public double CalcPageHorizontalOffset(double itemWidth) {
			double sizeOffset = Math.Max(0d, (ViewportWidth - itemWidth) / 2d);
			if (ViewportWidth.LessThan(itemWidth))
				sizeOffset -= HorizontalOffset;
			return sizeOffset;
		}
		public void InvalidatePanel() {
			InvalidateMeasure();
		}
		public double GetVirtualVerticalOffset() {
			return GetVirtualOffset(VerticalOffset);
		}
		double GetVirtualOffset(double offset) {
			double k = ExtentHeight / (ExtentHeight - ViewportHeight);
			if (k.IsNotNumber())
				return ExtentHeight - 1d;
			return Math.Min(ExtentHeight - 1d, offset * k);
		}
		protected virtual IndexCalculator CreateIndexCalculator() {
			return new IndexCalculator(this);
		}
		protected virtual void OnShowSingleItemChanged(bool newValue) {
			FixSinglePageOffset(newValue);
			InvalidatePanel();
		}
		void FixSinglePageOffset(bool showSingleItem) {
			int pageIndex = IndexCalculator.VerticalOffsetToIndex(!showSingleItem ? GetVirtualVerticalOffset() : VerticalOffset);
			double relativePageOffset = GetPageRelativeVerticalOffset(!showSingleItem);
			ActualShowSingleItem = showSingleItem;
			double verticalOffset = IndexCalculator.IndexToVerticalOffset(pageIndex, showSingleItem) + RelativeOffsetToRealOffset(relativePageOffset, IndexCalculator.GetRealItemSize(pageIndex).Height, showSingleItem);
			SetVerticalOffset(verticalOffset);
		}
		double RelativeOffsetToRealOffset(double relativeOffset, double pageHeight, bool showSingleItem) {
			if (!showSingleItem)
				return relativeOffset * pageHeight;
			if (pageHeight.LessThanOrClose(ViewportHeight))
				return 0d;
			double pageDiff = pageHeight - ViewportHeight;
			double k = ExtentHeight / (ExtentHeight - ViewportHeight);
			double virtualPageHeight = pageHeight / k;
			if (relativeOffset.GreaterThanOrClose(pageDiff / pageHeight))
				return virtualPageHeight - 1;
			double k1 = pageHeight / pageDiff;
			double newRelativeOffset = relativeOffset * k1;
			return virtualPageHeight * newRelativeOffset;
		}
		protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args) {
			base.OnItemsChanged(sender, args);
			InvalidatePanel();
		}
		protected override Size MeasureOverride(Size availableSize) {
			UIElementCollection children = InternalChildren;
			UpdateScrollData(availableSize);
			int firstVisibleItemIndex;
			int lastVisibleItemIndex;
			CalcVisibleItemsRange(availableSize.Height, out firstVisibleItemIndex, out lastVisibleItemIndex);
			IItemContainerGenerator generator = ItemContainerGenerator;
			GeneratorPosition startPos = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);
			int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;
			using (generator.StartAt(startPos, GeneratorDirection.Forward, true)) {
				for (int itemIndex = firstVisibleItemIndex; itemIndex <= lastVisibleItemIndex; ++itemIndex, ++childIndex) {
					bool isNew;
					UIElement child = generator.GenerateNext(out isNew) as UIElement;
					if (child == null)
						break;
					if (isNew) {
						if (childIndex >= children.Count)
							AddInternalChild(child);
						else
							InsertInternalChild(childIndex, child);
						generator.PrepareItemContainer(child);
					}
					else {
						Debug.Assert(Equals(child, children[childIndex]), "Wrong child was generated");
						child.InvalidateMeasure();
					}
					child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				}
			}
			CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);
			return base.MeasureOverride(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double nextStartPosition = CalcStartPosition(ShowSingleItem);
			for (int i = 0; i < InternalChildren.Count; ++i) {
				double startPosition = nextStartPosition;
				UIElement container = InternalChildren[i];
				Size arrangeSize = container.DesiredSize;
				Rect arrangeRect = new Rect(new Point(CalcPageHorizontalOffset(arrangeSize.Width), startPosition), arrangeSize);
				nextStartPosition = startPosition + arrangeSize.Height;
				container.Arrange(arrangeRect);
			}
			return base.ArrangeOverride(finalSize);
		}
		public double GetPageRelativeVerticalOffset(bool showSingleItem) {
			int pageIndex = IndexCalculator.VerticalOffsetToIndex(showSingleItem ? GetVirtualVerticalOffset() : VerticalOffset);
			if (showSingleItem && IndexCalculator.GetRealItemSize(pageIndex).Height.LessThanOrClose(ViewportHeight))
				return 0;
			double offset = -CalcStartPosition(showSingleItem);
			return offset / IndexCalculator.GetRealItemSize(pageIndex).Height;
		}
		#region IScrollInfo
		ScrollData scrollData = new ScrollData();
		public bool CanHorizontallyScroll {
			get { return scrollData.CanHorizontallyScroll; }
			set { scrollData.CanHorizontallyScroll = value; }
		}
		public bool CanVerticallyScroll {
			get { return scrollData.CanVerticallyScroll; }
			set { scrollData.CanVerticallyScroll = value; }
		}
		public ScrollViewer ScrollOwner {
			get { return scrollData.ScrollOwner; }
			set { scrollData.ScrollOwner = value; }
		}
		public double VerticalOffset {
			get { return scrollData.VerticalOffset; }
		}
		public double HorizontalOffset {
			get { return scrollData.HorizontalOffset; }
		}
		public double ViewportHeight {
			get { return scrollData.ViewportSize.Height; }
		}
		public double ViewportWidth {
			get { return scrollData.ViewportSize.Width; }
		}
		public double ExtentHeight {
			get { return scrollData.ExtentSize.Height; }
		}
		public double ExtentWidth {
			get { return scrollData.ExtentSize.Width; }
		}
		public Rect MakeVisible(Visual visual, Rect rectangle) {
			ScrollToRect(rectangle, ScrollIntoViewMode.TopLeft);
			return rectangle;
		}
		public virtual void ScrollIntoView(int pageNumber, Rect rect) {
			ScrollIntoView(pageNumber, rect, ScrollIntoViewMode.TopLeft);
		}
		public virtual void ScrollIntoView(int pageNumber, Rect rect, ScrollIntoViewMode mode) {
			Rect rectangle;
			if (!rect.IsEmpty)
				rectangle = new Rect(rect.X, rect.Y + IndexCalculator.IndexToVerticalOffset(pageNumber, false), rect.Width, rect.Height);
			else
				rectangle = new Rect(0, IndexCalculator.IndexToVerticalOffset(pageNumber, false), IndexCalculator.GetRealItemSize(pageNumber).Width, IndexCalculator.GetRealItemSize(pageNumber).Height);
			ScrollToRect(rectangle, mode);
		}
		public void MouseWheelDown() {
			if (ShowSingleItem) {
				int pageIndex = IndexCalculator.VerticalOffsetToIndex(GetVirtualVerticalOffset());
				if (IndexCalculator.GetRealItemSize(pageIndex).Height.LessThanOrClose(ViewportHeight)) {
					SetVerticalOffset(IndexCalculator.IndexToVerticalOffset(pageIndex + 1, ShowSingleItem));
					return;
				}
			}
			SetVerticalOffset(scrollData.VerticalOffset + scrollData.WheelSize);
		}
		public void MouseWheelLeft() {
			SetHorizontalOffset(scrollData.HorizontalOffset - scrollData.WheelSize);
		}
		public void MouseWheelRight() {
			SetHorizontalOffset(scrollData.HorizontalOffset + scrollData.WheelSize);
		}
		public void MouseWheelUp() {
			if (ShowSingleItem) {
				int pageIndex = IndexCalculator.VerticalOffsetToIndex(GetVirtualVerticalOffset());
				if (IndexCalculator.GetRealItemSize(pageIndex).Height.LessThanOrClose(ViewportHeight)) {
					SetVerticalOffset(IndexCalculator.IndexToVerticalOffset(pageIndex - 1, ShowSingleItem));
					return;
				}
			}
			SetVerticalOffset(VerticalOffset - scrollData.WheelSize);
		}
		public void LineDown() {
			SetVerticalOffset(VerticalOffset + scrollData.VerticalLineSize);
		}
		public void LineLeft() {
			SetHorizontalOffset(HorizontalOffset - scrollData.HorizontalLineSize);
		}
		public void LineRight() {
			SetHorizontalOffset(HorizontalOffset + scrollData.HorizontalLineSize);
		}
		public void LineUp() {
			SetVerticalOffset(VerticalOffset - scrollData.VerticalLineSize);
		}
		public void PageDown() {
			SetVerticalOffset(VerticalOffset + ViewportHeight);
		}
		public void PageLeft() {
			SetHorizontalOffset(HorizontalOffset - ViewportWidth);
		}
		public void PageRight() {
			SetHorizontalOffset(HorizontalOffset + ViewportWidth);
		}
		public void PageUp() {
			SetVerticalOffset(VerticalOffset - ViewportHeight);
		}
		public void SetHorizontalOffset(double offset) {
			offset = Math.Max(0, Math.Min(offset, ExtentWidth - ViewportWidth));
			if (!offset.AreClose(HorizontalOffset)) {
				scrollData.HorizontalOffset = offset;
				InvalidateMeasure();
				ScrollOwner.Do(x => x.InvalidateScrollInfo());
			}
		}
		public void SetVerticalOffset(double offset) {
			offset = Math.Max(0, Math.Min(offset, ExtentHeight - ViewportHeight));
			if (!offset.AreClose(VerticalOffset)) {
				scrollData.VerticalOffset = offset;
				InvalidateMeasure();
				ScrollOwner.Do(x => x.InvalidateScrollInfo());
			}
		}
		void ScrollToRect(Rect rectangle, ScrollIntoViewMode mode) {
			switch (mode) {
				case ScrollIntoViewMode.Center:
					CenterRect(rectangle);
					break;
				case ScrollIntoViewMode.Edge:
					ScrollToEdge(rectangle);
					break;
				case ScrollIntoViewMode.TopLeft:
					ScrollToRect(rectangle);
					break;
			}
		}
		void CenterRect(Rect rectangle) {
			if (ShowSingleItem) {
				CenterRectSingleItemMode(rectangle);
				return;
			}
			if (IsRectangleVisible(rectangle))
				return;
			if (rectangle.Height.GreaterThanOrClose(ViewportHeight)) {
				SetVerticalOffset(rectangle.Top);
			}
			else {
				double offset = (ViewportHeight - rectangle.Height) / 2d;
				SetVerticalOffset(rectangle.Top - offset);
			}
			if (rectangle.Width.GreaterThanOrClose(ViewportWidth)) {
				SetHorizontalOffset(rectangle.Left);
			}
			else {
				double offset = (ViewportWidth - rectangle.Width) / 2d;
				SetHorizontalOffset(rectangle.Left - offset);
			}
		}
		void CenterRectSingleItemMode(Rect rectangle) {
			if (IsRectangleVisible(rectangle))
				return;
			int pageIndex = IndexCalculator.VerticalOffsetToIndex(rectangle.Top);
			if (IndexCalculator.GetRealItemSize(pageIndex).Height.LessThanOrClose(ViewportHeight)) {
				SetVerticalOffset(IndexCalculator.IndexToVerticalOffset(pageIndex, true));
				return;
			}
			double pageOffset = IndexCalculator.IndexToVerticalOffset(pageIndex, false);
			double relativeOffset = (rectangle.Top - pageOffset) / IndexCalculator.GetRealItemSize(pageIndex).Height;
			double verticalOffset = IndexCalculator.IndexToVerticalOffset(pageIndex, true) + RelativeOffsetToRealOffset(relativeOffset, IndexCalculator.GetRealItemSize(pageIndex).Height, true);
			SetVerticalOffset(verticalOffset);
			SetHorizontalOffset(rectangle.Left);
		}
		void ScrollToEdge(Rect rectangle) {
			if (ShowSingleItem) {
				ScrollToEdgeSingleItemMode(rectangle);
				return;
			}
			if (!IsRectangleVerticalVisible(rectangle)) {
				if (rectangle.Top.LessThanOrClose(VerticalOffset + 10d))
					SetVerticalOffset(rectangle.Top - 10d);
				if (rectangle.Bottom.GreaterThanOrClose(VerticalOffset + ViewportHeight - 10d))
					SetVerticalOffset(rectangle.Bottom - ViewportHeight + 10d);
			}
			if (!IsRectangleHorizontalVisible(rectangle)) {
				if (rectangle.Right.GreaterThanOrClose(HorizontalOffset + ViewportWidth - 10d))
					SetHorizontalOffset(rectangle.Right - ViewportWidth + 10d);
				if (rectangle.Left.LessThanOrClose(HorizontalOffset + 10d))
					SetHorizontalOffset(rectangle.Left - 10d);
			}
		}
		void ScrollToEdgeSingleItemMode(Rect rectangle) {
			if (!IsRectangleVerticalVisible(rectangle)) {
				int pageIndex = IndexCalculator.VerticalOffsetToIndex(GetVirtualVerticalOffset());
				double pageVerticalOffset = IndexCalculator.IndexToVerticalOffset(pageIndex, false);
				double pageSize = IndexCalculator.GetRealItemSize(pageIndex).Height;
				if (pageSize.GreaterThan(ViewportHeight)) {
					var relativeOffset = GetPageRelativeVerticalOffset(true);
					pageVerticalOffset += pageSize * relativeOffset;
				}
				if (rectangle.Top.LessThanOrClose(pageVerticalOffset + 10d))
					SetVerticalOffset(GetSinglePageModeVerticalOffset(rectangle.Top - 10d));
				if (rectangle.Bottom.GreaterThanOrClose(pageVerticalOffset + ViewportHeight - 10d)) {
					var rectanglePageIndex = IndexCalculator.VerticalOffsetToIndex(rectangle.Bottom);
					if (rectanglePageIndex != pageIndex) {
						SetVerticalOffset(IndexCalculator.IndexToVerticalOffset(rectanglePageIndex, true));
						ScrollToEdgeSingleItemMode(rectangle);
					}
					else
						SetVerticalOffset(GetSinglePageModeVerticalOffset(rectangle.Bottom - ViewportHeight + 10d));
				}
			}
			if (!IsRectangleHorizontalVisible(rectangle)) {
				if (rectangle.Right.GreaterThanOrClose(HorizontalOffset + ViewportWidth - 10d))
					SetHorizontalOffset(rectangle.Right - ViewportWidth + 10d);
				if (rectangle.Left.LessThanOrClose(HorizontalOffset + 10d))
					SetHorizontalOffset(rectangle.Left - 10d);
			}
		}
		void ScrollToRect(Rect rectangle) {
			SetVerticalOffset(ShowSingleItem ? GetSinglePageModeVerticalOffset(rectangle.Top) : rectangle.Top);
			SetHorizontalOffset(rectangle.Left);
		}
		double GetSinglePageModeVerticalOffset(double offset) {
			int pageIndex = IndexCalculator.VerticalOffsetToIndex(offset);
			double pageOffset = IndexCalculator.IndexToVerticalOffset(pageIndex, false);
			double relativeOffset = (offset - pageOffset) / IndexCalculator.GetRealItemSize(pageIndex).Height;
			return IndexCalculator.IndexToVerticalOffset(pageIndex, true) + RelativeOffsetToRealOffset(relativeOffset, IndexCalculator.GetRealItemSize(pageIndex).Height, true);
		}
		bool IsRectangleVisible(Rect rect) {
			return IsRectangleVerticalVisible(rect) && IsRectangleHorizontalVisible(rect);
		}
		bool IsRectangleVerticalVisible(Rect rect) {
			var viewport = Rect.Empty;
			if (ShowSingleItem) {
				int pageIndex = IndexCalculator.VerticalOffsetToIndex(GetVirtualVerticalOffset());
				double pageVerticalOffset = IndexCalculator.IndexToVerticalOffset(pageIndex, false);
				double pageSize = IndexCalculator.GetRealItemSize(pageIndex).Height;
				if (pageSize.GreaterThan(ViewportHeight)) {
					var relativeOffset = GetPageRelativeVerticalOffset(true);
					pageVerticalOffset += pageSize * relativeOffset;
				}
				double viewportHeight = pageSize.LessThanOrClose(ViewportHeight) ? pageSize : ViewportHeight;
				viewport = new Rect(HorizontalOffset, pageVerticalOffset, ViewportWidth, viewportHeight);
			}
			else
				viewport = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);
			var newViewport = Rect.Union(viewport, rect);
			return viewport.Height.Round().AreClose(newViewport.Height.Round());
		}
		bool IsRectangleHorizontalVisible(Rect rect) {
			var viewport = new Rect(HorizontalOffset, 0, ViewportWidth, ExtentHeight);
			var newViewport = Rect.Union(viewport, rect);
			return viewport.Width.Round().AreClose(newViewport.Width.Round());
		}
		void InvalidateScrollInfo(Size viewportSize, Size extentSize) {
			if ((scrollData.ViewportSize != viewportSize || scrollData.ExtentSize != extentSize)) {
				scrollData.ViewportSize = viewportSize;
				scrollData.ExtentSize = extentSize;
				scrollData.VerticalLineSize = extentSize.Height / (ItemsControl.GetItemsOwner(this).Items.Count * 10d);
				scrollData.HorizontalLineSize = extentSize.Width / 50d;
				scrollData.HorizontalOffset = Math.Max(0d, Math.Min(ExtentWidth - ViewportWidth, HorizontalOffset));
				scrollData.VerticalOffset = Math.Max(0d, Math.Min(ExtentHeight - ViewportHeight, VerticalOffset));
				ScrollOwner.Do(x => x.InvalidateScrollInfo());
				InvalidateMeasure();
			}
		}
		class ScrollData {
			public double HorizontalOffset { get; set; }
			public double VerticalOffset { get; set; }
			public Size ViewportSize { get; set; }
			public Size ExtentSize { get; set; }
			public double VerticalLineSize { get; set; }
			public double HorizontalLineSize { get; set; }
			public double WheelSize { get { return 50; } }
			public bool CanHorizontallyScroll { get; set; }
			public bool CanVerticallyScroll { get; set; }
			public ScrollViewer ScrollOwner { get; set; }
		}
		#endregion
	}
}
