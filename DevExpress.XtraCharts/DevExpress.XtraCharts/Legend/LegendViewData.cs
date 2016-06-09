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
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using DevExpress.Utils;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class LegendViewData {
		public static LegendViewData Create(Legend legend, IList<ILegendItemData> legendItems, TextMeasurer textMeasurer, Rectangle mappingBounds) {
			if (legend.Visibility == DefaultBoolean.False || legendItems.Count == 0) {
				legend.Bounds = new GRealRect2D();
				return null;
			}
			RectangleIndents margins = legend.Margins;
			int doubleBorderThickness = legend.Border.ActualThickness * 2;
			int width = (int)(mappingBounds.Width * legend.MaxHorizontalPercentage / 100) - doubleBorderThickness - margins.Left - margins.Right;
			int height = (int)(mappingBounds.Height * legend.MaxVerticalPercentage / 100) - doubleBorderThickness - margins.Top - margins.Bottom;
			Size maxLegendSize = new Size(width, height);
			if (!GraphicUtils.CheckIsSizePositive(maxLegendSize))
				return null;
			List<LegendItemViewData> itemsData = new List<LegendItemViewData>(legendItems.Count);
			foreach (ILegendItemData item in legendItems) 
				itemsData.Add(new LegendItemViewData(legend, item, textMeasurer));
			Shadow shadow = legend.Shadow;
			LegendItemGroup[] groups = LegendItemGroupingAlgorithm.Calculate(legend, 
				itemsData, shadow.Visible ? shadow.DecreaseSize(maxLegendSize) : maxLegendSize);
			LegendItemsOffsetCalculator.Calculate(legend, groups);
			Size innerSize = LegendSizeCalculator.Calculate(legend, groups);
			LegendViewData viewData = null;
			if(GraphicUtils.CheckIsSizePositive(innerSize))
				viewData = new LegendViewData(legend, groups, mappingBounds, maxLegendSize, innerSize);
			if (viewData != null)
				legend.Bounds = new GRealRect2D(viewData.Bounds.Left, viewData.Bounds.Top, viewData.Bounds.Width, viewData.Bounds.Height);
			legend.SetItemsCount(itemsData.Count);
			return legend.ActualVisibility ? viewData : null;
		}
		readonly Legend legend;
		readonly LegendItemGroup[] itemGroups;
		readonly Rectangle bounds;
		readonly Rectangle outerBounds;
		public LegendItemGroup[] ItemGroups { get { return itemGroups; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle OuterBounds { get { return outerBounds; } }
		LegendViewData(Legend legend, LegendItemGroup[] itemGroups, Rectangle mappingBounds, Size maxLegendSize, Size innerSize) {
			this.legend = legend;
			this.itemGroups = itemGroups;
			RectangularBorder border = legend.Border;
			int doubleBorderThickness = border.ActualThickness * 2;
			Size outerSize = border.CalcBorderedSize(innerSize);
			Size totalSize = outerSize;
			Shadow shadow = legend.Shadow;
			if (shadow.Visible) {
				totalSize = shadow.IncreaseSize(outerSize);
				totalSize = new Size(Math.Min(maxLegendSize.Width + doubleBorderThickness, totalSize.Width),
									 Math.Min(maxLegendSize.Height + doubleBorderThickness, totalSize.Height));
				outerSize = shadow.DecreaseSize(totalSize);
			}
			else {
				totalSize = new Size(Math.Min(maxLegendSize.Width + doubleBorderThickness, totalSize.Width),
									 Math.Min(maxLegendSize.Height + doubleBorderThickness, totalSize.Height));
				outerSize = totalSize;
			}
			totalSize = legend.Margins.IncreaseSize(totalSize);
			int left = AlignRectangleX(mappingBounds, totalSize, legend.AlignmentHorizontal);
			int top = AlignRectangleY(mappingBounds, totalSize, legend.AlignmentVertical);
			Point locaton = new Point(left + legend.Margins.Left, top + legend.Margins.Top);
			bounds = new Rectangle(locaton, outerSize);
			outerBounds = new Rectangle(locaton, totalSize);
		}
		int AlignRectangleX(Rectangle outerRect, Size innerRectSize, LegendAlignmentHorizontal alignment) {
			switch (alignment) {
				case LegendAlignmentHorizontal.Left:
				case LegendAlignmentHorizontal.LeftOutside:
					return outerRect.Left;
				case LegendAlignmentHorizontal.Center:
					return outerRect.Left + (outerRect.Width - innerRectSize.Width) / 2;
				default:
					return outerRect.Right - innerRectSize.Width;
			}
		}
		int AlignRectangleY(Rectangle outerRect, Size innerRectSize, LegendAlignmentVertical alignment) {
			switch (alignment) {
				case LegendAlignmentVertical.Center:
					return outerRect.Top + (outerRect.Height - innerRectSize.Height) / 2;
				case LegendAlignmentVertical.Bottom:
				case LegendAlignmentVertical.BottomOutside:
					return outerRect.Bottom - innerRectSize.Height;
				default:
					return outerRect.Top;
			}
		}
		public Rectangle ModifyBounds(Rectangle bounds) {
			LegendAlignmentHorizontal alignmentHorizontal = legend.AlignmentHorizontal;
			if (alignmentHorizontal == LegendAlignmentHorizontal.LeftOutside)
				bounds.X += outerBounds.Width;
			if (alignmentHorizontal == LegendAlignmentHorizontal.LeftOutside || alignmentHorizontal == LegendAlignmentHorizontal.RightOutside)
				bounds.Width -= outerBounds.Width;
			LegendAlignmentVertical alignmentVertical = legend.AlignmentVertical;
			if (alignmentVertical == LegendAlignmentVertical.TopOutside)
				bounds.Y += outerBounds.Height;
			if (alignmentVertical == LegendAlignmentVertical.TopOutside || alignmentVertical == LegendAlignmentVertical.BottomOutside)
				bounds.Height -= outerBounds.Height;
			return bounds;
		}
		public void Render(IRenderer renderer, Rectangle mappingBounds) {
			RectangularBorder border = legend.Border;
			int borderThickness = border.ActualThickness;
			Rectangle innerBounds = Rectangle.Inflate(bounds, -borderThickness, -borderThickness);
			if (!innerBounds.AreWidthAndHeightPositive())
				return;
			if (!legend.IsOutside)
				renderer.SetClipping(mappingBounds);
			renderer.ProcessHitTestRegion(legend.Chart.HitTestController, legend, null, new HitRegion(bounds)); 
			legend.Shadow.Render(renderer, bounds);
			LegendAppearance appearance = legend.Appearance;
			if ((legend.BackImage == null) || (!legend.BackImage.Render(renderer, innerBounds)) ) {
				BackgroundImage defaultBackImage = appearance.BackImage;
				if ((defaultBackImage == null) || (defaultBackImage.Render(renderer, innerBounds)))
					renderer.FillRectangle(innerBounds, legend.ActualBackColor, legend.ActualFillStyle);
			}
			HitTestState hitState = ((IHitTest)legend).State;
			Color borderColor = BorderHelper.CalculateBorderColor(border, appearance.BorderColor, hitState);
			BorderHelper.RenderBorder(renderer, border, innerBounds, hitState, borderThickness, borderColor);
			renderer.SetClipping(innerBounds);
			RectangleIndents padding = legend.ActualPadding;
			Point location = innerBounds.Location;
			location.Offset(padding.Left, padding.Top);
			foreach (LegendItemGroup group in itemGroups)
				group.Render(renderer, location);
			renderer.RestoreClipping();
			if (!legend.IsOutside)
				renderer.RestoreClipping();
		}
	}
}
