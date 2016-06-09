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
	public class SimpleDiagramSeriesLayout : SimpleDiagramSeriesLayoutBase {
		readonly SimpleDiagramSeriesLabelLayoutList labelLayoutList;
		readonly List<AnnotationViewData> annotationsViewData;
		readonly SimpleDiagramDomain domain;
		readonly float exclamationTextSize;
		public SimpleDiagramSeriesLabelLayoutList LabelLayoutList { get { return labelLayoutList; } }
		public SimpleDiagramDomain Domain { get { return domain; } }
		public SimpleDiagramSeriesLayout(RefinedSeriesData seriesData, TextMeasurer textMeasurer, Rectangle diagramBounds, Rectangle initialDomainBounds)
			: base(seriesData) {
			if (!initialDomainBounds.AreWidthAndHeightPositive())
				return;
			Rectangle domainBounds = initialDomainBounds;
			TitlesViewData = CalculateTitlesViewDataAndCorrectDomainBounds(textMeasurer, ref domainBounds);
			Rectangle innerBounds, labelsBounds;
			if (View.CalculateBounds(SeriesData, domainBounds, out innerBounds, out labelsBounds)) {
				domainBounds = CorrectBoundsAndTitlesPosition(domainBounds, ref labelsBounds, ref innerBounds);
				if (innerBounds.AreWidthAndHeightPositive() && domainBounds.AreWidthAndHeightPositive())
					this.domain = new SimpleDiagramDomain(diagramBounds, initialDomainBounds, domainBounds, labelsBounds, innerBounds);
			}
			labelLayoutList = new SimpleDiagramSeriesLabelLayoutList(seriesData, domain);
			if (domain == null)
				return;
			SimpleDiagramWholeSeriesViewData simpleWholeViewData = SeriesData.WholeViewData as SimpleDiagramWholeSeriesViewData;
			if (simpleWholeViewData != null && simpleWholeViewData.NegativeValuePresents)
				exclamationTextSize = GraphicUtils.CalcFontEmSize(textMeasurer, domain.ElementBounds,
					ExclamationText, ExclamationFontFamily, ExclamationAlignment, ExclamationLineAlignment);
			AddRange(View.CalculatePointsLayout(domain, seriesData));
			AnnotationsAnchorPointsLayout = new List<AnnotationLayout>();
			foreach (SeriesPointLayout pointLayout in this) {
				if (!pointLayout.IsEmpty)
					foreach (SeriesLabelViewData labelViewData in pointLayout.LabelViewData)
						if (!String.IsNullOrEmpty(labelViewData.Text)) {
							SeriesData.Series.Label.CalculateLayout(labelLayoutList, pointLayout, textMeasurer);
							break;
						}
				AnnotationsAnchorPointsLayout.AddRange(View.CalculateAnnotationsAchorPointsLayout(domain, pointLayout));
			}
			this.annotationsViewData = AnnotationHelper.CreateInnerAnnotationsViewData(AnnotationsAnchorPointsLayout, textMeasurer, domain.LabelsBounds);
		}
		public SimpleDiagramSeriesLayout(RefinedSeriesData seriesData, TextMeasurer textMeasurer, IList<DockableTitleViewData> titlesViewData, Rectangle diagramBounds,
										 Rectangle bounds, Rectangle domainBounds, Rectangle elementWithLabelsBounds, Rectangle elementBounds)
			: base(seriesData) {
			TitlesViewData = titlesViewData;
			Rectangle correctedDomainBounds = CorrectBoundsAndTitlesPosition(domainBounds, ref elementWithLabelsBounds, ref elementBounds);
			this.domain = new SimpleDiagramDomain(diagramBounds, domainBounds, correctedDomainBounds, elementWithLabelsBounds, elementBounds);
			this.labelLayoutList = new SimpleDiagramSeriesLabelLayoutList(seriesData, domain);
			SimpleDiagramWholeSeriesViewData simpleWholeViewData = SeriesData.WholeViewData as SimpleDiagramWholeSeriesViewData;
			if (simpleWholeViewData != null && simpleWholeViewData.NegativeValuePresents)
				exclamationTextSize = GraphicUtils.CalcFontEmSize(textMeasurer, domain.ElementBounds,
					ExclamationText, ExclamationFontFamily, ExclamationAlignment, ExclamationLineAlignment);
			AddRange(View.CalculatePointsLayout(domain, seriesData));
			AnnotationsAnchorPointsLayout = new List<AnnotationLayout>();
			foreach (SeriesPointLayout pointLayout in this) {
				if (!pointLayout.IsEmpty)
					foreach (SeriesLabelViewData labelViewData in pointLayout.LabelViewData)
						if (!String.IsNullOrEmpty(labelViewData.Text)) {
							SeriesData.Series.Label.CalculateLayout(labelLayoutList, pointLayout, textMeasurer);
							break;
						}
				AnnotationsAnchorPointsLayout.AddRange(View.CalculateAnnotationsAchorPointsLayout(domain, pointLayout));
			}
			annotationsViewData = AnnotationHelper.CreateInnerAnnotationsViewData(AnnotationsAnchorPointsLayout, textMeasurer, domain.DiagramBounds);
		}
		void ProcessHitTesting(IRenderer renderer) {
			HitTestController hitTestController = HitTestController;
			bool toolTipsEnabled = HitTestController.PointToolTipEnabled(SeriesData.Series);
			if (!(hitTestController.Enabled || toolTipsEnabled))
				return;
			foreach (SeriesPointLayout pointLayout in this) {
				if (pointLayout == null)
					continue;
				HitRegionContainer pointHitRegion = pointLayout.CalculateHitRegion();
				pointHitRegion.Intersect(new HitRegion(this.domain.DiagramBounds));
				if (pointHitRegion == null)
					continue;
				using (pointHitRegion) {
					ISeriesPoint seriesPoint = pointLayout.SeriesPoint;
					renderer.ProcessHitTestRegion(hitTestController, SeriesData.Series, pointLayout.PointData.RefinedPoint, (IHitRegion)pointHitRegion.Underlying.Clone());
					if (toolTipsEnabled) {
						ChartFocusedArea additionalObject = new ChartFocusedArea(seriesPoint, pointLayout.ToolTipRelativePosition);
						IHitRegion region = (IHitRegion)pointHitRegion.Underlying.Clone();
						renderer.ProcessHitTestRegion(hitTestController, SeriesData.Series, additionalObject, region, true);
					}
				}
			}
		}
		public void RenderContent(IRenderer renderer) {
			Render(renderer);
			RenderTitles(renderer);
		}
		public override void Render(IRenderer renderer) {
			foreach (SeriesPointLayout pointLayout in this)
				if (pointLayout != null)
					View.Render(renderer, domain.Bounds, pointLayout, pointLayout.DrawOptions);
			ProcessHitTesting(renderer);
		}
		public void RenderExclamation(IRenderer renderer) {
			if (domain == null || exclamationTextSize < 0.4)
				return;
			renderer.DrawBoundedText(ExclamationText, new NativeFontDisposable(new Font(ExclamationFontFamily, this.exclamationTextSize)),
				View.ExclamationTextColor, View, domain.ElementBounds, ExclamationAlignment, ExclamationLineAlignment);
		}
		public void RenderAnnotations(IRenderer renderer) {
			if (this.annotationsViewData == null)
				return;
			foreach (AnnotationViewData viewData in annotationsViewData)
				viewData.Render(renderer);
		}
		public void RenderTitles(IRenderer renderer) {
			if (TitlesViewData == null)
				return;
			foreach (DockableTitleViewData item in TitlesViewData)
				item.Render(renderer);
		}
	}
}
