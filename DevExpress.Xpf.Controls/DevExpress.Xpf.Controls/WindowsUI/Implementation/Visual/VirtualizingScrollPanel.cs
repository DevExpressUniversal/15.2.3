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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
namespace DevExpress.Xpf.WindowsUI.Base {
	public abstract class VirtualizingScrollPanel : VirtualizingPanel, IScrollInfo {
		#region static
		static double ComputeScrollOffset(double topView, double bottomView, double topChild, double bottomChild) {
			bool fBottom = topChild < topView && bottomChild < bottomView;
			bool fTop = bottomChild > bottomView && topChild > topView;
			bool fSize = (bottomChild - topChild) > (bottomView - topView);
			if(!fBottom && !fTop)
				return topView;
			if((fBottom && !fSize) || (fTop && fSize))
				return topChild;
			return (bottomChild - (bottomView - topView));
		}
		#endregion
		protected virtual double LineSizeHorizontal { get { return 16d; } }
		const double WheelSize = 48d;
		ScrollInfo scrollInfo;
		protected VirtualizingScrollPanel() {
			scrollInfo = new ScrollInfo();
		}
		protected virtual bool CanUpdateScrollInfoOnMeasure(Size availableSize) {
			return !double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height);
		}
		protected override Size MeasureOverride(Size availableSize) {
			int firstItemIndex, lastItemIndex;
			if(CanUpdateScrollInfoOnMeasure(availableSize))
				UpdateScrollInfo(availableSize);
			GetVisibleRange(availableSize, out firstItemIndex, out lastItemIndex);
#if SILVERLIGHT
			UIElementCollection children = this.Children;
#else
			UIElementCollection children = this.InternalChildren;
#endif
			IItemContainerGenerator generator = this.ItemContainerGenerator;
			GeneratorPosition startPos = generator.GeneratorPositionFromIndex(firstItemIndex);
			int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;
			double maxWidth = 0;
			double maxHeight = 0;
			if(childIndex >= 0) {
				using(generator.StartAt(startPos, GeneratorDirection.Forward, true)) {
					for(int itemIndex = firstItemIndex; itemIndex <= lastItemIndex; ++itemIndex, ++childIndex) {
						bool isNewlyRealized;
						UIElement child = generator.GenerateNext(out isNewlyRealized) as UIElement;
						if(isNewlyRealized) {
							if(childIndex >= children.Count) {
								base.AddInternalChild(child);
							}
							else {
								base.InsertInternalChild(childIndex, child);
							}
							generator.PrepareItemContainer(child);
						}
						else {
							AssertionException.AreEqual(child, children[childIndex]);
						}
						child.Measure(availableSize);
						maxWidth = Math.Max(child.DesiredSize.Width, maxWidth);
						maxHeight = Math.Max(child.DesiredSize.Height, maxHeight);
					}
				}
				CleanUpItems(firstItemIndex, lastItemIndex);
			}
			maxHeight = double.IsInfinity(availableSize.Height) ? maxHeight : availableSize.Height;
			maxWidth = double.IsInfinity(availableSize.Width) ? maxWidth : availableSize.Width;
			Size measureSize = new Size(maxWidth, maxHeight);
			UpdateScrollInfo(measureSize);
			return measureSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			IItemContainerGenerator generator = this.ItemContainerGenerator;
			UpdateScrollInfo(finalSize);
			for(int i = 0; i < this.Children.Count; i++) {
				UIElement child = this.Children[i];
				int itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));
				ArrangeChild(itemIndex, child, finalSize);
			}
			AfterArrange();
			return finalSize;
		}
		protected virtual void AfterArrange() { }
		private void UpdateScrollInfo(Size availableSize) {
			ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
			int itemCount = itemsControl.Items.Count;
			Size extent = CalculateExtent(availableSize, itemCount);
			InvalidateScrollInfo(availableSize, extent);
		}
		protected virtual void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated) {
#if SILVERLIGHT
			UIElementCollection children = this.Children;
#else
			UIElementCollection children = this.InternalChildren;
#endif
			IItemContainerGenerator generator = this.ItemContainerGenerator;
			for(int i = children.Count - 1; i >= 0; i--) {
				GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
				int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
				if(itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated) {
					generator.Remove(childGeneratorPos, 1);
					RemoveInternalChildRange(i, 1);
				}
			}
		}
		protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args) {
			switch(args.Action) {
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
#if !SILVERLIGHT
				case NotifyCollectionChangedAction.Move:
#endif
					RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
					break;
			}
		}
		protected virtual Rect MakeVisibleCore(Rect rectangle) {
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
		protected void InvalidateScrollInfo(Size viewport, Size extent) {
			if(double.IsInfinity(viewport.Width))
				viewport.Width = extent.Width;
			if(double.IsInfinity(viewport.Height))
				viewport.Height = extent.Height;
			scrollInfo._extent = extent;
			scrollInfo._viewport = viewport;
			Point offset = new Point(
				Math.Max(0, Math.Min(scrollInfo._offset.X, ExtentWidth - ViewportWidth)),
				Math.Max(0, Math.Min(scrollInfo._offset.Y, ExtentHeight - ViewportHeight))
				);
			if(double.IsNaN(offset.X)) offset.X = 0;
			if(double.IsNaN(offset.Y)) offset.Y = 0;
			scrollInfo._offset.X = offset.X;
			scrollInfo._offset.Y = offset.Y;
			if(ScrollOwner != null)
				ScrollOwner.InvalidateScrollInfo();
		}
		protected virtual void OnHorizontalOffsetChanged(double offset) {
			InvalidateArrange();
		}
		protected virtual void OnVerticalOffsetChanged(double offset) {
			InvalidateArrange();
		}
		protected virtual double CalculateOffset(double extent, double viewport, double desiredOffset) {
			desiredOffset = Math.Max(0, Math.Min(desiredOffset, extent - viewport));
			return desiredOffset;
		}
		protected Point GetScrollOfset() {
			return scrollInfo._offset;
		}
		protected abstract void ArrangeChild(int childIndex, UIElement child, Size finalSize);
		protected abstract Size CalculateExtent(Size availableSize, int itemsCount);
		protected abstract void GetVisibleRange(Size size, out int firstItemIndex, out int lastItemIndex);
		#region IScrollInfo Members
		public bool CanHorizontallyScroll { get; set; }
		public bool CanVerticallyScroll { get; set; }
		public double ExtentHeight {
			get { return scrollInfo._extent.Height; }
		}
		public double ExtentWidth {
			get { return scrollInfo._extent.Width; }
		}
		public double HorizontalOffset { get { return scrollInfo._offset.X; } }
		public void LineDown() {
			SetVerticalOffset(VerticalOffset + LineSizeHorizontal);
		}
		public void LineLeft() {
			SetHorizontalOffset(HorizontalOffset - LineSizeHorizontal);
		}
		public void LineRight() {
			SetHorizontalOffset(HorizontalOffset + LineSizeHorizontal);
		}
		public void LineUp() {
			SetVerticalOffset(VerticalOffset - LineSizeHorizontal);
		}
#if !SILVERLIGHT
		public Rect MakeVisible(Visual visual, Rect rectangle) {
#else
		public Rect MakeVisible(UIElement visual, Rect rectangle) {
#endif
			if(rectangle.IsEmpty || visual == null) return Rect.Empty;
#if !SILVERLIGHT
			if(!IsAncestorOf(visual)) return Rect.Empty;
			rectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);
#else
			rectangle = visual.TransformToVisual(this).TransformBounds(rectangle);
#endif
			return MakeVisibleCore(rectangle);
		}
		public void MouseWheelDown() {
			SetVerticalOffset(VerticalOffset + WheelSize);
		}
		public void MouseWheelLeft() {
			SetVerticalOffset(VerticalOffset + WheelSize);
		}
		public void MouseWheelRight() {
			SetHorizontalOffset(HorizontalOffset + WheelSize);
		}
		public void MouseWheelUp() {
			SetVerticalOffset(VerticalOffset - WheelSize);
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
		public ScrollViewer ScrollOwner { get; set; }
		public void SetHorizontalOffset(double offset) {
			offset = CalculateOffset(scrollInfo._extent.Width, scrollInfo._viewport.Width, offset);
			if(offset != scrollInfo._offset.X) {
				scrollInfo._offset.X = offset;
				OnHorizontalOffsetChanged(offset);
			}
		}
		public void SetVerticalOffset(double offset) {
			offset = CalculateOffset(scrollInfo._extent.Height, scrollInfo._viewport.Height, offset);
			if(offset != scrollInfo._offset.Y) {
				scrollInfo._offset.Y = offset;
				OnVerticalOffsetChanged(offset);
			}
		}
		public double VerticalOffset { get { return scrollInfo._offset.Y; } }
		public double ViewportHeight { get { return scrollInfo._viewport.Height; } }
		public double ViewportWidth { get { return scrollInfo._viewport.Width; } }
		#endregion
		class ScrollInfo {
			internal Point _offset;
			internal Size _viewport;
			internal Size _extent;
		}
	}
}
