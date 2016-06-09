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
using System.Windows;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class Axis2DElementLayout : ILayout {
		readonly Rect bounds;
		public bool Visible { get { return true; } }
		public Rect Bounds { get { return bounds; } }
		public Rect ClipBounds { get { return Rect.Empty; } }
		#region ILayout implementation
		Point ILayout.Location { get { return new Point(bounds.Left, bounds.Top); } }
		Size ILayout.Size { get { return new Size(bounds.Width, bounds.Height); } }
		double ILayout.Angle { get { return 0.0; } }
		#endregion
		public Axis2DElementLayout(Rect bounds) {
			this.bounds = bounds;
		}
	}
	public class AxisLabelBounds {
		double maxLeft = 0.0;
		double maxRight = 0.0;
		double maxTop = 0.0;
		double maxBottom = 0.0;
		public double MaxHeight { get { return maxTop + maxBottom; } }
		public double MaxWidth { get { return maxLeft + maxRight; } }
		public double MaxLeft { get { return maxLeft; } }
		public double MaxTop { get { return maxTop; } }
		public void Reset() {
			maxLeft = 0.0;
			maxRight = 0.0;
			maxTop = 0.0;
			maxBottom = 0.0;
		}
		public void SelectMax(double left, double top, double right, double bottom) {
			maxLeft = Math.Max(maxLeft, left);
			maxTop = Math.Max(maxTop, top);
			maxRight = Math.Max(maxRight, right);
			maxBottom = Math.Max(maxBottom, bottom);
		}
	}
	public abstract class Axis2DElementsLayoutCalculator {
		public static void CalculateLayout(Axis2DItem axisItem, Rect viewport, Rect diagramBounds, bool isNavigationEnabled, ref Thickness offsets, bool firstAxisOnPosition, ref AxisLabelResolveOverlappingCache axisLabelsCache) {
			Axis2DElementsLayoutCalculator calculator;
			Axis2D axis = axisItem.Axis as Axis2D;
			switch (axis.Position) {
				case AxisPosition.Left:
					calculator = new LeftAxis2DElementsLayoutCalculator(axisItem, viewport);
					break;
				case AxisPosition.Right:
					calculator = new RightAxis2DElementsLayoutCalculator(axisItem, viewport);
					break;
				case AxisPosition.Top:
					calculator = new TopAxis2DElementsLayoutCalculator(axisItem, viewport);
					break;
				default:
					calculator = new BottomAxis2DElementsLayoutCalculator(axisItem, viewport);
					break;
			}
			offsets = calculator.CreateLayout( diagramBounds, firstAxisOnPosition, isNavigationEnabled, offsets);
			axisLabelsCache = calculator.axisLabelResolveOverlappingCache;
		}
		Axis2DItem axisItem;
		Rect viewport;
		IAxisMapping axisMapping;
		AxisLabelBounds maxPrimaryBounds = new AxisLabelBounds();
		AxisLabelBounds maxStaggeredBounds = new AxisLabelBounds();
		AxisLabelResolveOverlappingCache axisLabelResolveOverlappingCache = null;
		protected Rect Viewport { get { return viewport; } }
		protected double AxisThickness { get { return axisItem.TotalThickness; } }
		protected AxisLabelBounds MaxPrimaryBounds { get { return maxPrimaryBounds; } }
		protected AxisLabelBounds MaxStaggeredBounds { get { return maxStaggeredBounds; } }
		protected double MaxLabelItemsWidth { get { return Math.Max(MaxPrimaryBounds.MaxWidth, MaxStaggeredBounds.MaxWidth); } }
		protected double MaxLabelItemsWidthLeft { get { return Math.Max(MaxPrimaryBounds.MaxLeft, MaxStaggeredBounds.MaxLeft); } }
		protected double MaxLabelItemsHeight { get { return Math.Max(MaxPrimaryBounds.MaxHeight, MaxStaggeredBounds.MaxHeight); } }
		protected double MaxLabelItemsHeightTop { get { return Math.Max(MaxPrimaryBounds.MaxTop, MaxStaggeredBounds.MaxTop); } }
		protected Size TitleSize { get { return axisItem.TitleItem.Size; } }
		protected double HorizontalTitleOffset {
			get {
				AxisTitleItem titleItem = axisItem.TitleItem;
				switch (((AxisTitle)titleItem.Title).ActualAlignment) {
					case TitleAlignment.Center:
						return (viewport.Left + viewport.Right - titleItem.Size.Width) / 2;
					case TitleAlignment.Far:
						return viewport.Right - titleItem.Size.Width;
					default:
						return viewport.Left;
				}
			}
		}
		protected double VerticalTitleOffset {
			get {
				AxisTitleItem titleItem = axisItem.TitleItem;
				switch (((AxisTitle)titleItem.Title).ActualAlignment) {
					case TitleAlignment.Center:
						return (viewport.Top + viewport.Bottom - titleItem.Size.Height) / 2;
					case TitleAlignment.Far:
						return viewport.Top;
					default:
						return viewport.Bottom - titleItem.Size.Height;
				}
			}
		}
		protected Axis2DElementsLayoutCalculator(Axis2DItem axisItem, Rect viewport) {
			this.axisItem = axisItem;
			this.viewport = viewport;
			axisMapping = axisItem.Axis.CreateMapping(viewport);
		}
		protected double GetAxisValue(AxisLabelItem labelItem) {
			return axisMapping.GetRoundedAxisValue(labelItem.Value) + 1.0;
		}
		protected abstract Thickness CorrectOffsetsBySize(double correctionSize, Thickness offsets);
		protected abstract Rect CalculateAxisItemRect(Thickness offsets);
		protected abstract Rect CalculateLabelRectWithScrolling(Thickness offsets);
		protected abstract Rect CalculateTitleRect(Thickness offsets);
		protected abstract Axis2DLabelItemLayout CalculateLabelItemLayout(AxisLabelItem labelItem, Thickness offsets);
		Rect CalculateLabelRect(IList<AxisLabelItem> items, Axis2D axis, Thickness offsets, bool isNavigationEnabled, Rect diagramBounds) {
			Rect labelRect = Rect.Empty;
			List<IAxisLabelLayout> layoutItems = new List<IAxisLabelLayout>();
			AxisLabel label = axis.ActualLabel;
			for (int i = 0; i < items.Count; i++) {
				Axis2DLabelItemLayout layout = CalculateLabelItemLayout(items[i], offsets);
				layout.Angle = label.Angle;
				layout.Visible = true;
				layout.IsOutOfRange = items[i].IsOutOfRange;
				layoutItems.Add(layout);
			}
			AxisLabelOverlappingResolver overlappingResolver = AxisLabelsResolveOverlappingHelper.CreateOverlappingResolver(layoutItems, axis, axis.Alignment, false);
			if (isNavigationEnabled && axis.OverlappingCache != null && axis.OverlappingCache.Count == layoutItems.Count)
				axis.OverlappingCache.Apply(layoutItems);
			else {
				overlappingResolver.Process();
				if (axisLabelResolveOverlappingCache == null)
					axisLabelResolveOverlappingCache = new AxisLabelResolveOverlappingCache(axis, layoutItems);
			}
			if (!axis.IsVertical && (isNavigationEnabled || !AreLabelsRotated(layoutItems)))
				overlappingResolver.ProcessLabelsLimits(diagramBounds.ToGRealRect2D());
			overlappingResolver.ProcessCustomLabels();
			for (int i = 0; i < layoutItems.Count; i++) {
				items[i].AssignLayout(layoutItems[i]);
			}
			if (isNavigationEnabled) {
				maxStaggeredBounds.Reset();
				maxPrimaryBounds.Reset();
				for (int i = 0; i < items.Count; i++) {
					if (layoutItems[i].Visible) {
						Axis2DLabelItemLayout layoutItem = (Axis2DLabelItemLayout)layoutItems[i];
						double leftWidth = 0, rightWidth = 0, topHeight = 0, bottomHeight = 0;
						Rect itemRect = layoutItem.Bounds;
						Point basePoint = layoutItem.BasePoint;
						leftWidth = items[i].Staggered ? Math.Abs(basePoint.X - itemRect.Left) - Math.Abs(layoutItem.Offset.X) : Math.Abs(basePoint.X - itemRect.Left);
						rightWidth = Math.Abs(itemRect.Right - basePoint.X);
						topHeight = items[i].Staggered ? Math.Abs(basePoint.Y - itemRect.Top) - Math.Abs(layoutItem.Offset.Y) : Math.Abs(basePoint.Y - itemRect.Top);
						bottomHeight = Math.Abs(itemRect.Bottom - basePoint.Y);
						AxisLabelBounds affectedBounds = items[i].Staggered ? maxStaggeredBounds : maxPrimaryBounds;
						affectedBounds.SelectMax(leftWidth, topHeight, rightWidth, bottomHeight);
					}
				}
				labelRect = CalculateLabelRectWithScrolling(offsets);
			}
			else {
				foreach (AxisLabelItem labelItem in items)
					if (labelItem.Visible) {
						Rect labelItemRect = labelItem.Layout.Bounds;
						if (labelRect.IsEmpty)
							labelRect = labelItemRect;
						else
							labelRect.Union(labelItemRect);
					}
			}
			return labelRect;
		}
		Thickness CreateLayout(Rect diagramBounds, bool firstAxisOnPosition, bool isNavigationEnabled, Thickness inputOffsets) {
			Axis2D axis = axisItem.Axis as Axis2D;
			Thickness offsets = new Thickness(inputOffsets.Left, inputOffsets.Top, inputOffsets.Right, inputOffsets.Bottom);
			offsets = CorrectOffsetsBySize(firstAxisOnPosition ? -(axisItem.ActualTickmarksCrossLength + 1) : 9, offsets);
			Axis2DElementLayout axisLayout = new Axis2DElementLayout(CalculateAxisItemRect(offsets));
			offsets = CorrectOffsetsBySize(AxisThickness, offsets);
			Rect labelRect = CalculateLabelRectangle(isNavigationEnabled, axis, ref offsets, axis.IsVertical, diagramBounds);
			offsets = CorrectOffsetsBySize(axisItem.TitleIndent, offsets);
			Axis2DElementLayout axisTitleLayout = CalculateTitleLayout(diagramBounds, ref offsets, axis.IsVertical, axis.IsTitleVisible);
			bool axisVisible = axis.ActualVisible;
			SetLabelVisible(axisVisible);
			axisItem.Layout = axisLayout;
			axisItem.LabelRect = labelRect;
			if (axisItem.TitleItem != null) {
				if (axis.IsTitleVisible && axisVisible)
					axisItem.TitleItem.Layout = axisTitleLayout;
				else
					axisItem.TitleItem.Layout = null;
				axis.Title.Bounds = axisTitleLayout.Bounds;
			}
			if (axisVisible)
				return offsets;
			return inputOffsets;
		}
		void SetLabelVisible(bool axisVisible) {
			if (axisItem.LabelItems != null)
				foreach (AxisLabelItem labelItem in axisItem.LabelItems)
					if (labelItem.Layout != null) {
						labelItem.Layout.Visible = axisVisible;
						labelItem.Visible = axisVisible;
					}
		}
		Rect CalculateLabelRectangle(bool isNavigationEnabled, Axis2D axis, ref Thickness offsets, bool isVertical, Rect diagramBounds) {
			Rect labelRect;
			IList<AxisLabelItem> items = axisItem.LabelItems;
			if (items == null || items.Count == 0)
				labelRect = RectExtensions.Zero;
			else if (axis.ActualLabel.Visible) {
				labelRect = CalculateLabelRect(items, axis, offsets, isNavigationEnabled, diagramBounds);
				offsets = CorrectOffsetsBySize(isVertical ? labelRect.Width : labelRect.Height, offsets);
			}
			else {
				foreach (AxisLabelItem labelItem in items)
					labelItem.Layout = null;
				labelRect = RectExtensions.Zero;
			}
			return labelRect;
		}
		Axis2DElementLayout CalculateTitleLayout(Rect diagramBounds, ref Thickness offsets, bool isVertical, bool isTitleVisible) {
			if (axisItem.TitleItem == null)
				return null;
			Thickness titleOffsets = new Thickness(offsets.Left, offsets.Top, offsets.Right, offsets.Bottom);
			Axis2DElementLayout axisTitleLayout;
			Rect titleRect = CalculateTitleRect(titleOffsets);
			double titleWidth = Math.Min(titleRect.Width, diagramBounds.Width);
			double titleHeight = Math.Min(titleRect.Height, diagramBounds.Height);
			if (isVertical) {
				titleRect.Y += Math.Min(0, diagramBounds.Bottom - titleRect.Bottom) + Math.Max(0, diagramBounds.Top - titleRect.Top);
				titleOffsets = CorrectOffsetsBySize(titleWidth, titleOffsets);
			}
			else {
				titleRect.X += Math.Min(0, diagramBounds.Right - titleRect.Right) + Math.Max(0, diagramBounds.Left - titleRect.Left);
				titleOffsets = CorrectOffsetsBySize(titleHeight, titleOffsets);
			}
			titleRect = new Rect(titleRect.X, titleRect.Y, titleWidth, titleHeight);
			axisTitleLayout = new Axis2DElementLayout(titleRect);
			if (isTitleVisible)
				offsets = titleOffsets;
			return axisTitleLayout;
		}
		bool AreLabelsRotated(List<IAxisLabelLayout> labels) {
			bool rotated = false;
			foreach (IAxisLabelLayout label in labels) {
				if ((label.Angle % 90.0) != 0.0) {
					rotated = true;
					break;
				}
			}
			return rotated;
		}
	}
	public class LeftAxis2DElementsLayoutCalculator : Axis2DElementsLayoutCalculator {
		internal LeftAxis2DElementsLayoutCalculator(Axis2DItem axisItem, Rect viewport)
			: base(axisItem, viewport) {
		}
		protected override Thickness CorrectOffsetsBySize(double correctionSize, Thickness offsets) {
			offsets.Left -= correctionSize;
			return offsets;
		}
		protected override Rect CalculateAxisItemRect(Thickness offsets) {
			double axisThickness = AxisThickness;
			Rect viewport = Viewport;
			return new Rect(new Point(viewport.Left + offsets.Left - axisThickness, viewport.Top), new Size(axisThickness, viewport.Height));
		}
		protected override Axis2DLabelItemLayout CalculateLabelItemLayout(AxisLabelItem labelItem, Thickness offsets) {
			Size labelSize = labelItem.Size;
			Rect viewport = Viewport;
			double x = viewport.Left + offsets.Left;
			return new LeftAxis2DLabelItemLayout(new Rect(new Point(x, viewport.Bottom - GetAxisValue(labelItem)), labelSize), labelItem);
		}
		protected override Rect CalculateLabelRectWithScrolling(Thickness offsets) {
			Rect viewport = Viewport;
			double aggregateLabelItemsWidth = MaxLabelItemsWidth;
			return new Rect(new Point(viewport.Left + offsets.Left - aggregateLabelItemsWidth, viewport.Top - MaxLabelItemsHeightTop),
				new Size(aggregateLabelItemsWidth, viewport.Height + MaxLabelItemsHeight));
		}
		protected override Rect CalculateTitleRect(Thickness offsets) {
			Size titleSize = TitleSize;
			return new Rect(new Point(Viewport.Left + offsets.Left - titleSize.Width, VerticalTitleOffset), titleSize);
		}
	}
	public class RightAxis2DElementsLayoutCalculator : Axis2DElementsLayoutCalculator {
		internal RightAxis2DElementsLayoutCalculator(Axis2DItem axisItem, Rect viewport)
			: base(axisItem, viewport) {
		}
		protected override Thickness CorrectOffsetsBySize(double correctionSize, Thickness offsets) {
			offsets.Right += correctionSize;
			return offsets;
		}
		protected override Rect CalculateAxisItemRect(Thickness offsets) {
			Rect viewport = Viewport;
			return new Rect(new Point(viewport.Right + offsets.Right, viewport.Top), new Size(AxisThickness, viewport.Height));
		}
		protected override Axis2DLabelItemLayout CalculateLabelItemLayout(AxisLabelItem labelItem, Thickness offsets) {
			Size labelSize = labelItem.Size;
			Rect viewport = Viewport;
			double x = viewport.Right + offsets.Right;
			return new RightAxis2DLabelItemLayout(new Rect(new Point(x, viewport.Bottom - GetAxisValue(labelItem)), labelSize), labelItem);
		}
		protected override Rect CalculateLabelRectWithScrolling(Thickness offsets) {
			Rect viewport = Viewport;
			return new Rect(new Point(viewport.Right + offsets.Right, viewport.Top - MaxLabelItemsHeightTop), new Size(MaxLabelItemsWidth, viewport.Height + MaxLabelItemsHeight));
		}
		protected override Rect CalculateTitleRect(Thickness offsets) {
			return new Rect(new Point(Viewport.Right + offsets.Right, VerticalTitleOffset), TitleSize);
		}
	}
	public class TopAxis2DElementsLayoutCalculator : Axis2DElementsLayoutCalculator {
		internal TopAxis2DElementsLayoutCalculator(Axis2DItem axisItem, Rect viewport)
			: base(axisItem, viewport) {
		}
		protected override Thickness CorrectOffsetsBySize(double correctionSize, Thickness offsets) {
			offsets.Top -= correctionSize;
			return offsets;
		}
		protected override Rect CalculateAxisItemRect(Thickness offsets) {
			double axisThickness = AxisThickness;
			Rect viewport = Viewport;
			return new Rect(new Point(viewport.Left, viewport.Top + offsets.Top - axisThickness), new Size(viewport.Width, axisThickness));
		}
		protected override Axis2DLabelItemLayout CalculateLabelItemLayout(AxisLabelItem labelItem, Thickness offsets) {
			Size labelSize = labelItem.Size;
			Rect viewport = Viewport;
			double y = viewport.Top + offsets.Top;
			return new TopAxis2DLabelItemLayout(new Rect(new Point(viewport.Left + GetAxisValue(labelItem), y), labelSize), labelItem);
		}
		protected override Rect CalculateLabelRectWithScrolling(Thickness offsets) {
			Rect viewport = Viewport;
			double aggregateLabelItemsHeight = MaxLabelItemsHeight;
			return new Rect(new Point(viewport.Left, viewport.Top + offsets.Top - aggregateLabelItemsHeight), new Size(viewport.Width, aggregateLabelItemsHeight));
		}
		protected override Rect CalculateTitleRect(Thickness offsets) {
			Size titleSize = TitleSize;
			return new Rect(new Point(HorizontalTitleOffset, Viewport.Top + offsets.Top - titleSize.Height), titleSize);
		}
	}
	public class BottomAxis2DElementsLayoutCalculator : Axis2DElementsLayoutCalculator {
		internal BottomAxis2DElementsLayoutCalculator(Axis2DItem axisItem, Rect viewport)
			: base(axisItem, viewport) {
		}
		protected override Thickness CorrectOffsetsBySize(double correctionSize, Thickness offsets) {
			offsets.Bottom += correctionSize;
			return offsets;
		}
		protected override Rect CalculateAxisItemRect(Thickness offsets) {
			Rect viewport = Viewport;
			return new Rect(new Point(viewport.Left, viewport.Bottom + offsets.Bottom), new Size(viewport.Width, AxisThickness));
		}
		protected override Axis2DLabelItemLayout CalculateLabelItemLayout(AxisLabelItem labelItem, Thickness offsets) {
			Size labelSize = labelItem.Size;
			Rect viewport = Viewport;
			double y = viewport.Bottom + offsets.Bottom;
			return new BottomAxis2DLabelItemLayout(new Rect(new Point(viewport.Left + GetAxisValue(labelItem), y), new Size(labelSize.Width, labelSize.Height)), labelItem);
		}
		protected override Rect CalculateLabelRectWithScrolling(Thickness offsets) {
			Rect viewport = Viewport;
			return new Rect(new Point(viewport.Left, viewport.Bottom + offsets.Bottom), new Size(viewport.Width, MaxLabelItemsHeight));
		}
		protected override Rect CalculateTitleRect(Thickness offsets) {
			return new Rect(new Point(HorizontalTitleOffset, Viewport.Bottom + offsets.Bottom), TitleSize);
		}
	}
}
