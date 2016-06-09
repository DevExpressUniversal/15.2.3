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
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public abstract class SimpleDiagramSeriesLayoutBase : SeriesLayout {
		protected const StringAlignment ExclamationAlignment = StringAlignment.Center;
		protected const StringAlignment ExclamationLineAlignment = StringAlignment.Center;
		protected static readonly FontFamily ExclamationFontFamily = FontFamily.GenericSansSerif;
		protected static readonly string ExclamationText = ChartLocalizer.GetString(ChartStringId.PieIncorrectValuesText);
		readonly RefinedSeriesData seriesData;
		IList<DockableTitleViewData> titlesViewData;
		List<AnnotationLayout> annotationsAnchorPointsLayout;
		protected override HitTestController HitTestController { get { return View.HitTestController; } }
		public new RefinedSeriesData SeriesData { get { return seriesData; } }
		protected IList<DockableTitleViewData> TitlesViewData {
			get { return titlesViewData; }
			set { titlesViewData = value; }
		}
		public new SimpleDiagramSeriesViewBase View {
			get { return (SimpleDiagramSeriesViewBase)seriesData.Series.View; }
		}
		public List<AnnotationLayout> AnnotationsAnchorPointsLayout {
			get { return annotationsAnchorPointsLayout; }
			protected set { annotationsAnchorPointsLayout = value; }
		}
		public SimpleDiagramSeriesLayoutBase(RefinedSeriesData seriesData) : base(null) {
			this.seriesData = seriesData;
		}
		void CalculateBoundsOffsets(out int xOffset, out int yOffset) {
			xOffset = yOffset = 0;
			bool isVerticalLastTitle = this.titlesViewData[this.titlesViewData.Count - 1].IsVerticalTitle;
			double currentXOffset = 0;
			double currentYOffset = 0;
			for (int i = this.titlesViewData.Count - 1; i > -1; i--) {
				if (isVerticalLastTitle != this.titlesViewData[i].IsVerticalTitle)
					break;
				double currentOffset = this.titlesViewData[i].Size / 2.0;
				switch (this.titlesViewData[i].Title.Dock) {
					case ChartTitleDockStyle.Left:
						currentXOffset -= currentOffset;
						break;
					case ChartTitleDockStyle.Right:
						currentXOffset += currentOffset;
						break;
					case ChartTitleDockStyle.Top:
						currentYOffset -= currentOffset;
						break;
					case ChartTitleDockStyle.Bottom:
						currentYOffset += currentOffset;
						break;
				}
			}
			xOffset = (int)currentXOffset;
			yOffset = (int)currentYOffset;
		}
		void CalculateTitleOffsets(Rectangle domainBounds, Rectangle labelsBounds, out int leftOffset, out int topOffset, out int rightOffset, out int bottomOffset) {
			topOffset = labelsBounds.Top - domainBounds.Top;
			leftOffset = labelsBounds.Left - domainBounds.Left;
			bottomOffset = domainBounds.Bottom - labelsBounds.Bottom;
			rightOffset = domainBounds.Right - labelsBounds.Right;
		}
		void CorrectBoundsOffsets(int topOffset, int leftOffset, int bottomOffset, int rightOffset, ref int xOffset, ref int yOffset) {
			if (xOffset != 0) {
				if (xOffset > 0) {
					if (xOffset > rightOffset)
						xOffset = rightOffset;
				}
				else {
					if (Math.Abs(xOffset) > leftOffset)
						xOffset = -leftOffset;
				}
			}
			if (yOffset != 0) {
				if (yOffset > 0) {
					if (yOffset > bottomOffset)
						yOffset = bottomOffset;
				}
				else {
					if (Math.Abs(yOffset) > topOffset)
						yOffset = -topOffset;
				}
			}
		}
		void CorrectTitlesPosition(int topOffset, int leftOffset, int bottomOffset, int rightOffset, out bool isLeftOffset, out bool isTopOffset) {
			isLeftOffset = isTopOffset = false;
			bool isVerticalLastTitle = titlesViewData[titlesViewData.Count - 1].IsVerticalTitle;
			for (int i = titlesViewData.Count - 1; i > -1; i--) {
				if (isVerticalLastTitle != titlesViewData[i].IsVerticalTitle)
					break;
				switch (titlesViewData[i].Title.Dock) {
					case ChartTitleDockStyle.Left:
						if (leftOffset > 0) {
							titlesViewData[i].Offset(leftOffset, 0);
							isLeftOffset = true;
						}
						break;
					case ChartTitleDockStyle.Right:
						if (rightOffset > 0)
							titlesViewData[i].Offset(-rightOffset, 0);
						break;
					case ChartTitleDockStyle.Top:
						if (topOffset > 0) {
							titlesViewData[i].Offset(0, topOffset);
							isTopOffset = true;
						}
						break;
					case ChartTitleDockStyle.Bottom:
						if (bottomOffset > 0)
							titlesViewData[i].Offset(0, -bottomOffset);
						break;
				}
			}
		}
		Rectangle CorrectDomainBounds(Rectangle domainBounds, Rectangle enlargedLabelsBounds, bool isLeftOffset, bool isTopOffset, int xBoundsOffset, int yBoundsOffset) {
			int xOffset, yOffset;
			if (xBoundsOffset > 0)
				xOffset = xBoundsOffset + (domainBounds.Width - enlargedLabelsBounds.Width) / 2;
			else
				xOffset = isLeftOffset ? enlargedLabelsBounds.Left - domainBounds.Left : 0;
			if (yBoundsOffset > 0)
				yOffset = yBoundsOffset + (domainBounds.Height - enlargedLabelsBounds.Height) / 2;
			else
				yOffset = isTopOffset ? (enlargedLabelsBounds.Top - domainBounds.Top) : 0;
			if (xBoundsOffset != 0)
				domainBounds.Width = enlargedLabelsBounds.Width;
			if (yBoundsOffset != 0)
				domainBounds.Height = enlargedLabelsBounds.Height;
			domainBounds.Offset(xOffset, yOffset);
			return domainBounds;
		}
		protected IList<DockableTitleViewData> CalculateTitlesViewDataAndCorrectDomainBounds(TextMeasurer textMeasurer, ref Rectangle domainBounds) {
			if (View.Titles.Count == 0)
				return null;
			return View.Titles.CalculateViewDataAndBoundsWithoutTitle(textMeasurer, ref domainBounds);
		}
		protected Rectangle CorrectBoundsAndTitlesPosition(Rectangle domainBounds, ref Rectangle labelsBounds, ref Rectangle diagramBounds) {
			if (this.titlesViewData == null || titlesViewData.Count < 1)
				return domainBounds;
			int xOffset = 0, yOffset = 0;
			CalculateBoundsOffsets(out xOffset, out yOffset);
			int leftOffset, topOffset, rightOffset, bottomOffset;
			CalculateTitleOffsets(domainBounds, labelsBounds, out leftOffset, out topOffset, out rightOffset, out bottomOffset);
			CorrectBoundsOffsets(topOffset, leftOffset, bottomOffset, rightOffset, ref xOffset, ref yOffset);
			labelsBounds.Offset(xOffset, yOffset);
			diagramBounds.Offset(xOffset, yOffset);
			CalculateTitleOffsets(domainBounds, labelsBounds, out leftOffset, out topOffset, out rightOffset, out bottomOffset);
			bool isLeftOffset, isTopOffset;
			CorrectTitlesPosition(topOffset, leftOffset, bottomOffset, rightOffset, out isLeftOffset, out isTopOffset);
			return CorrectDomainBounds(domainBounds, labelsBounds, isLeftOffset, isTopOffset, xOffset, yOffset);
		}
	}
}
