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
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class AxisLabelViewData : AxisElementViewDataBase {
		const int staggeredIndent = 2;
		readonly AxisLabelItemList items;
		readonly int size;
		readonly Rectangle lastPrimaryLabelItemBounds;
		readonly Rectangle lastStaggeredLabelItemBounds;
		Rectangle bounds = Rectangle.Empty;
		public bool ShouldLimitLabels { get { return !Axis.IsVertical && (PaneAxesContainer.CanZoomOutAxis(Axis) || !items.IsRotated()); } }
		public int Size { get { return CalculateSize(items); } }
		public AxisLabelItemList Items { get { return items; } }
		public Rectangle LastPrimaryLabelItemBounds { get { return lastPrimaryLabelItemBounds; } }
		public Rectangle LastStaggeredLabelItemBounds { get { return lastStaggeredLabelItemBounds; } }
		public AxisLabelViewData(TextMeasurer textMeasurer, Axis2D axis, AxisIntervalMapping mapping, int axisOffset, int elementOffset, AxisTextDataEx textData)
			: base(axis, mapping, axisOffset, elementOffset, AxisElementLocation.Outside) {
			IList<AxisLabelItemBase> primaryItems = CalculateItems(textMeasurer, textData.PrimaryItems, 0);
			int primarySize = CalculateSize(primaryItems);
			if (textData.StaggeredItems.Count > 0)
				primarySize += staggeredIndent;
			IList<AxisLabelItemBase> staggeredItems = CalculateItems(textMeasurer, textData.StaggeredItems, primarySize);
			int staggeredSize = CalculateSize(staggeredItems);
			items = new AxisLabelItemList(primaryItems.Count + staggeredItems.Count);
			items.AddRange(primaryItems);
			items.AddRange(staggeredItems);
			lastPrimaryLabelItemBounds = primaryItems.Count > 0 ? primaryItems[primaryItems.Count - 1].RoundedBounds : Rectangle.Empty;
			lastStaggeredLabelItemBounds = staggeredItems.Count > 0 ? staggeredItems[staggeredItems.Count - 1].RoundedBounds : Rectangle.Empty;
			size = primarySize + staggeredSize;
		}
		AxisLabelItemList CalculateItems(TextMeasurer textMeasurer, IList<AxisTextItem> textItems, int offset) {
			Axis2D axis = Axis;
			AxisLabel label = axis.Label;
			AxisLabelItemList items = new AxisLabelItemList(textItems.Count);
			foreach (AxisTextItem textItem in textItems)
				items.Add(new AxisLabelItem(label, GetNotClampedScreenPoint(textItem.Value, 0, offset), textItem));
			items.AdjustPropertiesAndCreatePainters(axis, textMeasurer);
			return items;
		}
		int CalculateSize(IList<AxisLabelItemBase> items) {
			int size = 0;
			foreach (AxisLabelItemBase item in items) {
				int elementSize = Axis.GetTextSize(item.RoundedBounds);
				IAxisLabelLayout labelLayout = item as IAxisLabelLayout;
				if (labelLayout != null)
					if (Axis.IsVertical)
						elementSize += (int)Math.Abs(labelLayout.Offset.X);
					else
						elementSize += (int)Math.Abs(labelLayout.Offset.Y);
				if (elementSize > size)
					size = elementSize;
			}
			return size;
		}
		void UpdateCorrectionWithScrolling(RectangleCorrection correction) {
			Rectangle maxLabelRect = items.GetMaxLabelRect(Axis.IsVertical, Axis.Alignment);
			Rectangle cachedLabelRect = Axis.MaxLabelRectCache;
			Rectangle enlargedRect = new Rectangle(Math.Min(maxLabelRect.Left, cachedLabelRect.Left),
										 Math.Min(maxLabelRect.Top, cachedLabelRect.Top),
										 Math.Max(maxLabelRect.Width, cachedLabelRect.Width),
										 Math.Max(maxLabelRect.Height, cachedLabelRect.Height));
			Rectangle rectBuffer = enlargedRect;
			rectBuffer.Offset(GetScreenPoint(Double.NegativeInfinity, 0, 0));
			UpdateCorrection(correction, rectBuffer);
			rectBuffer = enlargedRect;
			rectBuffer.Offset(GetScreenPoint(Double.PositiveInfinity, 0, 0));
			UpdateCorrection(correction, rectBuffer);
			Axis.MaxLabelRectCache = enlargedRect;   
		}
		void UpdateCorrection(RectangleCorrection correction, Rectangle rect) {
			if (ShouldLimitLabels)
				correction.UpdateHeight(rect);
			else
				correction.Update(rect);
		}
		public Rectangle CalculateBounds() {
			if (bounds.IsEmpty) {
				foreach (AxisLabelItemBase item in items) {
					Rectangle itemBounds = GraphicUtils.RoundRectangle(item.Bounds);
					bounds = bounds.IsEmpty ? itemBounds : Rectangle.Union(bounds, itemBounds);
				}
			}
			return bounds;
		}
		public void Render(IRenderer renderer, Rectangle previousPrimaryLabelItemBounds, Rectangle previousStaggeredLabelItemBounds) {
			items.Render(renderer, HitTestController, previousPrimaryLabelItemBounds, previousStaggeredLabelItemBounds);
		}
		public override void CalculateDiagramBoundsCorrection(RectangleCorrection correction) {
			if (PaneAxesContainer.CanScrollAxis(Axis) && Axis.XYDiagram2D.IsScrollingEnabled)
				UpdateCorrectionWithScrolling(correction);
			else if (ShouldLimitLabels)
				items.UpdateHeight(correction);
			else
				items.UpdateCorrection(correction);
		}
		public override void Render(IRenderer renderer) {
			Render(renderer, Rectangle.Empty, Rectangle.Empty);
		}
	}
}
