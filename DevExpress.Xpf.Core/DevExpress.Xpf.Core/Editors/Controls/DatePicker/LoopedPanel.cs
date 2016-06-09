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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors {
	public class IndexCalculator {
		const double Epsilon = 0.000000001;
		protected const double MagicNumber = 100.0;
		public int GetIndex(int index, int count, bool isLooped) {
			if (count == 0)
				return 0;
			if (!isLooped)
				return index;
			int absIndex = Math.Abs(index);
			int loopedIndex = absIndex % count;
			return Math.Sign(index) >= 0 ? loopedIndex : count - loopedIndex;
		}
		public double IndexToLogicalOffset(int index) {
			return IndexToLogicalOffset((double)index);
		}
		public int LogicalOffsetToIndex(double offset, int count, bool isLooped) {
			int index = Convert.ToInt32(Floor(offset / MagicNumber));
			return GetIndex(index, count, isLooped);
		}
		public double LogicalToNormalizedOffset(double offset) {
			return offset / MagicNumber;
		}
		public double IndexToLogicalOffset(double index) {
			return index * MagicNumber;
		}
		public double OffsetToLogicalOffset(double offset, double viewport) {
			double center = CalcStart(viewport);
			return offset - center;
		}
		public virtual double CalcStart(double extent) {
			return 0d;
		}
		public double CalcRelativePosition(double offset) {
			double normalizedOffset = LogicalToNormalizedOffset(offset);
			return Floor(normalizedOffset) - normalizedOffset;
		}
		public double CalcIndexOffset(double offset, double logicalViewport, double viewport, double itemOffset, double tapPointOffset) {
			double center = logicalViewport / 2d;
			double logicalToPixel = logicalViewport / viewport;
			itemOffset = itemOffset * logicalToPixel - MagicNumber / 2d;
			return Floor(offset + tapPointOffset * logicalToPixel - center - itemOffset);
		}
		public double Floor(double value) {
			double rounded = Math.Round(value);
			if (AreClose(rounded, value))
				return rounded;
			return Math.Floor(value);
		}
		public bool AreClose(double value1, double value2) {
			return Math.Abs(value1 - value2) < Epsilon;
		}
		public bool AreLessOrClose(double value1, double value2) {
			return AreClose(value1, value2) || value1 < value2;
		}
	}
	public class LoopedPanel : Panel, IScrollInfo, IScrollSnapPointsInfo {
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty IsLoopedProperty;
		static LoopedPanel() {
			Type ownerType = typeof(LoopedPanel);
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), ownerType,
				new PropertyMetadata(Orientation.Vertical));
			IsLoopedProperty = DependencyPropertyManager.Register("IsLooped", typeof(bool), ownerType,
				new PropertyMetadata(true, (d, e) => ((LoopedPanel)d).LoopedChanged((bool)e.NewValue)));
		}
		protected virtual void LoopedChanged(bool newValue) {
		}
		protected virtual void ItemSizeChanged(Size newValue) {
		}
		public IDXItemContainerGenerator ItemsContainerGenerator { get; set; }
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public bool IsLooped {
			get { return (bool)GetValue(IsLoopedProperty); }
			set { SetValue(IsLoopedProperty, value); }
		}
		Size extentSize;
		Size viewportSize;
		double horizontalOffset;
		double verticalOffset;
		public IndexCalculator IndexCalculator { get; private set; }
		public LoopedPanel() {
			IndexCalculator = CreateIndexCalculator();
		}
		protected virtual IndexCalculator CreateIndexCalculator() {
			return new IndexCalculator();
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (ItemsContainerGenerator == null)
				return finalSize;
			double viewport = GetItemSize(finalSize);
			UpdateScrollData(viewport);
			double logicalViewport = GetLogicalViewport();
			double offset = GetOffset(logicalViewport);
			double calculatedLogicalOffset = EnsureLogicalOffset(offset, GetLogicalExtent(), logicalViewport);
			int index = CalcIndex(calculatedLogicalOffset);
			double relativeStartPosition = IndexCalculator.CalcRelativePosition(calculatedLogicalOffset);
			UIElement container = ItemsContainerGenerator.GetContainer(index);
			if (container == null)
				return finalSize;
			double startPosition;
			int arrangedItems = 0;
			double nextStartPosition = GetItemSize(container.DesiredSize) * relativeStartPosition;
			double itemsLength = 0d;
			do {
				startPosition = nextStartPosition;
				container = ItemsContainerGenerator.GetContainer(index);
				if (container == null)
					break;
				Size arrangeSize = new Size(
					Orientation == Orientation.Vertical ? finalSize.Width : container.DesiredSize.Width,
					Orientation == Orientation.Vertical ? container.DesiredSize.Height : finalSize.Height);
				Rect arrangeRect = Orientation == Orientation.Vertical
					? new Rect(new Point(0d, startPosition), arrangeSize)
					: new Rect(new Point(startPosition, 0d), arrangeSize);
				container.Arrange(arrangeRect);
				double itemSize = GetItemSize(arrangeSize);
				GetRelativeViewport(itemsLength, viewport, itemSize);
				nextStartPosition = startPosition + itemSize;
				itemsLength += itemSize;
				index = IncrementIndex(index, true);
				arrangedItems++;
			}
			while (CalcStopCriteria(arrangedItems, startPosition, viewport));
			return finalSize;
		}
		public void InvalidatePanel() {
			InvalidateMeasure();
		}
		static double GetRelativeViewport(double nextStartPosition, double viewport, double itemSize) {
			double delta = (nextStartPosition - viewport);
			if (DoubleExtensions.GreaterThan(delta, itemSize))
				return 0d;
			return nextStartPosition > viewport ? (itemSize - delta) / itemSize : 1d;
		}
		double GetLogicalViewport() {
			return Orientation == Orientation.Vertical ? ViewportHeight : ViewportWidth;
		}
		double GetLogicalExtent() {
			return Orientation == Orientation.Vertical ? ExtentHeight : ExtentWidth;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (ItemsContainerGenerator == null)
				return availableSize;
			double logicalViewport = GetLogicalViewport();
			double offset = GetOffset(logicalViewport);
			double viewport = GetItemSize(availableSize);
			double calculatedLogicalOffset = EnsureLogicalOffset(offset, GetLogicalExtent(), logicalViewport);
			int index = CalcIndex(calculatedLogicalOffset);
			double width = Orientation == Orientation.Vertical ? GetSize(availableSize, Orientation.Horizontal) : 0d;
			double height = Orientation == Orientation.Vertical ? 0d : GetSize(availableSize, Orientation.Vertical);
			double current = 0;
			int measuredItems = 0;
			double nextStartPosition = 0d;
			ItemsContainerGenerator.StartAt(index);
			do {
				current = nextStartPosition;
				bool isNew = false;
				UIElement container = ItemsContainerGenerator.Generate(index, out isNew);
				if (container == null)
					break;
				if (isNew)
					InternalChildren.Add(container);
				ItemsContainerGenerator.PrepareItemContainer(index, container);
				container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				double itemSize = GetItemSize(container.DesiredSize);
				if (Orientation == Orientation.Vertical) {
					width = Math.Max(container.DesiredSize.Width, width);
					height += container.DesiredSize.Height;
				}
				else {
					width += container.DesiredSize.Width;
					height = Math.Max(container.DesiredSize.Height, height);
				}
				nextStartPosition = current + itemSize;
				index = IncrementIndex(index, true);
				measuredItems++;
			}
			while (CalcStopCriteria(measuredItems, current, viewport));
			ItemsContainerGenerator.Stop();
			ItemsContainerGenerator.RemoveItems();
			return Orientation == Orientation.Vertical
				? new Size(width, double.IsInfinity(availableSize.Height) ? height : availableSize.Height)
				: new Size(double.IsInfinity(availableSize.Width) ? width : availableSize.Width, height);
		}
		double GetSize(Size size, Orientation orientation) {
			double result = orientation == Orientation.Vertical ? size.Height : size.Width;
			return double.IsInfinity(result) ? 0d : result;
		}
		double GetOffset(double logicalViewport) {
			return IndexCalculator.OffsetToLogicalOffset(Orientation == Orientation.Vertical ? VerticalOffset : HorizontalOffset, logicalViewport);
		}
		bool CalcStopCriteria(int items, double offset, double viewport) {
			bool continueGenerate = IndexCalculator.AreLessOrClose(offset, viewport);
			if (IsLooped)
				return continueGenerate && items < ItemsContainerGenerator.GetItemsCount();
			return continueGenerate;
		}
		int CalcIndex(double offset) {
			return IndexCalculator.LogicalOffsetToIndex(offset, ItemsContainerGenerator.GetItemsCount(), IsLooped);
		}
		int IncrementIndex(int index, bool direction) {
			index += direction ? 1 : -1;
			return IsLooped ? IndexToRange(index) : index;
		}
		int IndexToRange(int index) {
			int itemsCount = ItemsContainerGenerator.GetItemsCount();
			if (itemsCount == 0)
				return 0;
			if (index >= itemsCount)
				index = index % itemsCount;
			else if (index < 0) {
				int tempIndex = Math.Abs(index);
				tempIndex = tempIndex % itemsCount;
				index = itemsCount - tempIndex;
			}
			return index;
		}
		double GetItemSize(Size itemSize) {
			double result = Orientation == Orientation.Vertical ? itemSize.Height : itemSize.Width;
			if (double.IsInfinity(result) && ScrollOwner != null)
				result = Orientation == Orientation.Vertical ? ScrollOwner.ActualHeight : ScrollOwner.ActualWidth;
			return result;
		}
		double GetSize(Size size) {
			double itemSize = GetItemSize(size);
			return double.IsInfinity(itemSize) ? 0d : itemSize;
		}
		#region IScrollInfo
		ScrollViewer scrollOwner;
		public bool CanHorizontallyScroll { get; set; }
		public bool CanVerticallyScroll { get; set; }
		public ScrollViewer ScrollOwner {
			get { return scrollOwner; }
			set {
				if (scrollOwner != null) {
					scrollOwner.ManipulationStarted -= OnManipulationStarted;
					scrollOwner.ManipulationCompleted -= OnManipulationCompleted;
				}
				scrollOwner = value;
				if (scrollOwner != null) {
					scrollOwner.ManipulationStarted += OnManipulationStarted;
					scrollOwner.ManipulationCompleted += OnManipulationCompleted;
				}
			}
		}
		void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {
			ItemsContainerGenerator.Do(x => x.StopManipulation());
		}
		void OnManipulationStarted(object sender, ManipulationStartedEventArgs e) {
			ItemsContainerGenerator.Do(x => x.StartManipulation());
		}
		public double Extent {
			get { return Orientation == Orientation.Vertical ? ExtentHeight : ExtentWidth; }
		}
		public double ExtentHeight {
			get { return extentSize.Height; }
		}
		public double ExtentWidth {
			get { return extentSize.Width; }
		}
		public double Offset {
			get { return Orientation == Orientation.Vertical ? VerticalOffset : HorizontalOffset; }
		}
		public double HorizontalOffset {
			get { return horizontalOffset; }
		}
		public double VerticalOffset {
			get { return verticalOffset; }
		}
		public double Viewport {
			get { return Orientation == Orientation.Vertical ? ViewportHeight : ViewportWidth; }
		}
		public double ViewportHeight {
			get { return viewportSize.Height; }
		}
		public double ViewportWidth {
			get { return viewportSize.Width; }
		}
		protected void UpdateScrollData(double viewportSize) {
			if (ItemsContainerGenerator == null) {
				InvalidateScrollInfo(Size.Empty, Size.Empty);
				return;
			}
			int itemsCount = ItemsContainerGenerator.GetItemsCount();
			double viewport = CalcViewport(viewportSize);
			double logicalViewport = IndexCalculator.IndexToLogicalOffset(viewport);
			Size newViewportSize = Orientation == Orientation.Vertical
				? new Size(1, logicalViewport)
				: new Size(logicalViewport, 1);
			double logicalExtent = IsLooped ? IndexCalculator.IndexToLogicalOffset(itemsCount) : IndexCalculator.IndexToLogicalOffset(2 * itemsCount - 1);
			if (!IsLooped && DoubleExtensions.LessThanOrClose(logicalExtent, logicalViewport))
				logicalExtent = logicalViewport + IndexCalculator.IndexToLogicalOffset(1);
			Size newExtentSize = Orientation == Orientation.Vertical
				? new Size(1, logicalExtent)
				: new Size(logicalExtent, 1);
			InvalidateScrollInfo(newViewportSize, newExtentSize);
		}
		double CalcViewport(double viewport) {
			double logicalViewport = GetLogicalViewport();
			double offset = GetOffset(logicalViewport);
			double calculatedLogicalOffset = EnsureLogicalOffset(offset, GetLogicalExtent(), logicalViewport);
			int index = CalcIndex(calculatedLogicalOffset);
			double relativeStartPosition = IndexCalculator.CalcRelativePosition(calculatedLogicalOffset);
			double relativeViewport = 0d;
			UIElement container = ItemsContainerGenerator.GetContainer(index);
			if (container == null)
				return 0d;
			double nextStartPosition = GetItemSize(container.DesiredSize) * relativeStartPosition;
			double itemsLength = 0d;
			int arrangedItems = 0;
			do {
				container = ItemsContainerGenerator.GetContainer(index);
				if (container == null)
					break;
				double itemSize = GetItemSize(container.DesiredSize);
				itemsLength += itemSize;
				relativeViewport += GetRelativeViewport(itemsLength, viewport, itemSize);
				nextStartPosition = nextStartPosition + itemSize;
				index = IncrementIndex(index, true);
				arrangedItems++;
			}
			while (CalcStopCriteria(arrangedItems, nextStartPosition, viewport));
			return relativeViewport;
		}
		void InvalidateScrollInfo(Size newViewportSize, Size newExtentSize) {
			if (viewportSize != newViewportSize || extentSize != newExtentSize) {
				viewportSize = newViewportSize;
				extentSize = newExtentSize;
				ScrollOwner.Do(x => x.InvalidateScrollInfo());
				InvalidateMeasure();
			}
		}
		public void LineDown() {
			SetVerticalOffset(VerticalOffset + IndexCalculator.IndexToLogicalOffset(1));
		}
		public void LineLeft() {
			SetHorizontalOffset(HorizontalOffset - IndexCalculator.IndexToLogicalOffset(1));
		}
		public void LineRight() {
			SetHorizontalOffset(HorizontalOffset + IndexCalculator.IndexToLogicalOffset(1));
		}
		public void LineUp() {
			SetVerticalOffset(VerticalOffset - IndexCalculator.IndexToLogicalOffset(1));
		}
		public Rect MakeVisible(UIElement visual, Rect rectangle) {
			if (rectangle.IsEmpty || visual == null || visual == this)
				return Rect.Empty;
			Rect viewRect = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);
			rectangle.X += viewRect.X;
			rectangle.Y += viewRect.Y;
			viewRect.X = ComputeScrollOffset(viewRect.Left, viewRect.Right, rectangle.Left, rectangle.Right);
			viewRect.Y = ComputeScrollOffset(viewRect.Top, viewRect.Bottom, rectangle.Top, rectangle.Bottom);
			SetHorizontalOffset(viewRect.X);
			SetVerticalOffset(viewRect.Y);
			rectangle.Intersect(viewRect);
			rectangle.X -= viewRect.X;
			rectangle.Y -= viewRect.Y;
			return rectangle;
		}
		static double ComputeScrollOffset(double viewTop, double viewBottom, double childTop, double childBottom) {
			bool shiftedDown = childTop < viewTop && childBottom < viewBottom;
			bool shiftedUp = childBottom > viewBottom && childTop > viewTop;
			bool childLarger = (childBottom - childTop) > (viewBottom - viewTop);
			if (!shiftedDown && !shiftedUp)
				return viewTop;
			if ((shiftedDown && !childLarger) || (shiftedUp && childLarger))
				return childTop;
			return (childBottom - (viewBottom - viewTop));
		}
		public void MouseWheelDown() {
			SetVerticalOffset(VerticalOffset - IndexCalculator.IndexToLogicalOffset(1d));
			(ScrollOwner as DXScrollViewer).Do(x => x.IsIntermediate = true);
			ScrollOwner.Do(x => x.ScrollToVerticalOffset(verticalOffset));
		}
		public void MouseWheelLeft() {
			SetHorizontalOffset(HorizontalOffset + ((SystemParameters.WheelScrollLines * 3) / 9) * 10);
			(ScrollOwner as DXScrollViewer).Do(x => x.IsIntermediate = true);
			ScrollOwner.Do(x => x.ScrollToHorizontalOffset(horizontalOffset));
		}
		public void MouseWheelRight() {
			SetHorizontalOffset(HorizontalOffset - ((SystemParameters.WheelScrollLines * 3) / 9) * 10);
			(ScrollOwner as DXScrollViewer).Do(x => x.IsIntermediate = true);
			ScrollOwner.Do(x => x.ScrollToHorizontalOffset(horizontalOffset));
		}
		public void MouseWheelUp() {
			SetVerticalOffset(VerticalOffset + IndexCalculator.IndexToLogicalOffset(1d));
			(ScrollOwner as DXScrollViewer).Do(x => x.IsIntermediate = true);
			ScrollOwner.Do(x => x.ScrollToVerticalOffset(verticalOffset));
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
			if (offset != HorizontalOffset) {
				horizontalOffset = EnsureOffset(offset, ExtentWidth, ViewportWidth);
				InvalidatePanel();
			}
		}
		public void SetVerticalOffset(double offset) {
			double restrictedOffset = EnsureOffset(offset, ExtentHeight, ViewportHeight);
			if (!restrictedOffset.AreClose(VerticalOffset)) {
				verticalOffset = restrictedOffset;
				InvalidatePanel();
			}
		}
		double EnsureLogicalOffset(double offset, double extent, double viewport) {
			if (!IsLooped || extent.AreClose(0d) || viewport.AreClose(0d))
				return offset;
			if (Math.Sign(offset) > 0 && offset < extent)
				return offset;
			return Math.Sign(offset) > 0
				? offset % extent
				: extent - Math.Abs(offset) % extent;
		}
		double EnsureOffset(double offset, double extent, double viewport) {
			return IsLooped ? EnsureLoopedOffset(offset, extent, viewport) : EnsureNormalOffset(offset, extent, viewport);
		}
		double EnsureLoopedOffset(double offset, double extent, double viewport) {
			if (DoubleExtensions.GreaterThanOrClose(offset, 0d) && DoubleExtensions.LessThan(offset, extent))
				return offset;
			return DoubleExtensions.GreaterThan(offset, 0d)
				? offset % extent
				: extent - Math.Abs(offset) % extent;
		}
		double EnsureNormalOffset(double offset, double extent, double viewport) {
			int itemsCount = ItemsContainerGenerator.Return(x => x.GetItemsCount(), () => 0);
			return Math.Min(Math.Max(offset, 0), IndexCalculator.IndexToLogicalOffset(itemsCount - 1));
		}
		public Rect MakeVisible(Visual visual, Rect rectangle) {
			if (rectangle.IsEmpty || visual == null || visual == this)
				return Rect.Empty;
			Rect viewRect = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);
			rectangle.X += viewRect.X;
			rectangle.Y += viewRect.Y;
			viewRect.X = ComputeScrollOffset(viewRect.Left, viewRect.Right, rectangle.Left, rectangle.Right);
			viewRect.Y = ComputeScrollOffset(viewRect.Top, viewRect.Bottom, rectangle.Top, rectangle.Bottom);
			SetHorizontalOffset(viewRect.X);
			SetVerticalOffset(viewRect.Y);
			rectangle.Intersect(viewRect);
			rectangle.X -= viewRect.X;
			rectangle.Y -= viewRect.Y;
			return rectangle;
		}
		#endregion
		#region IScrollSnapPointsInfo
		public bool AreHorizontalSnapPointsRegular {
			get { return true; }
		}
		public bool AreVerticalSnapPointsRegular {
			get { return true; }
		}
		public IEnumerable<float> GetIrregularSnapPoints(Orientation orientation, SnapPointsAlignment alignment) {
			throw new NotImplementedException();
		}
		public virtual float GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment alignment, out float offset) {
			offset = 0f;
			double itemSize = IndexCalculator.IndexToLogicalOffset(1);
			switch (alignment) {
				case SnapPointsAlignment.Center:
					offset = (float)itemSize / 2f;
					break;
				case SnapPointsAlignment.Far:
					offset = (float)((orientation == Orientation.Vertical ? ViewportHeight : ViewportWidth) - itemSize / 2d);
					break;
				case SnapPointsAlignment.Near:
					offset = 0;
					break;
			}
			return (float)itemSize;
		}
		void RaiseHorizontalSnapPointsChanged() {
			if (HorizontalSnapPointsChanged != null)
				HorizontalSnapPointsChanged(this, EventArgs.Empty);
		}
		void RaiseVerticalSnapPointsChanged() {
			if (VerticalSnapPointsChanged != null)
				VerticalSnapPointsChanged(this, EventArgs.Empty);
		}
		public event EventHandler HorizontalSnapPointsChanged;
		public event EventHandler VerticalSnapPointsChanged;
		#endregion
	}
}
