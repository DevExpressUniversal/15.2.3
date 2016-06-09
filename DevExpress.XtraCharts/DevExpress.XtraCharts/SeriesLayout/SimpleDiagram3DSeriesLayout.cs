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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class SimpleDiagram3DSeriesLayout : SimpleDiagramSeriesLayoutBase {
		readonly Rectangle bounds;
		readonly SimpleDiagramSeriesLabelLayoutList labelLayoutList;
		readonly List<AnnotationViewData> annotationsViewData;
		readonly ISimpleDiagramDomain domain;
		readonly float exclamationTextSize;
		public SimpleDiagramSeriesLabelLayoutList LabelLayoutList { get { return labelLayoutList; } }
		public ISimpleDiagramDomain Domain { get { return domain; } }
		void ProcessHitTesting(IRenderer renderer) {
			HitTestController hitTestController = HitTestController;
			bool toolTipsEnabled = HitTestController.PointToolTipEnabled(SeriesData.Series);
			if (!(hitTestController.Enabled || toolTipsEnabled))
				return;
			foreach (SeriesPointLayout pointLayout in this) {
				if (pointLayout == null)
					continue;
				HitRegionContainer pointHitRegion = pointLayout.CalculateHitRegion();
				if (pointHitRegion == null)
					continue;
				using (pointHitRegion) {
					renderer.ProcessHitTestRegion(hitTestController, SeriesData.Series, pointLayout.PointData.RefinedPoint, (IHitRegion)pointHitRegion.Underlying.Clone());
					if (toolTipsEnabled) {
						SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(pointLayout.PointData.SeriesPoint);
						ChartFocusedArea additionalObject = new ChartFocusedArea(seriesPoint, pointLayout.PointData.ToolTipRelativePosition);
						IHitRegion region = (IHitRegion)pointHitRegion.Underlying.Clone();
						renderer.ProcessHitTestRegion(hitTestController, SeriesData.Series, additionalObject, region, true);
					}
				}
			}
		}
		public SimpleDiagram3DSeriesLayout(RefinedSeriesData seriesData, TextMeasurer textMeasurer, GRect2D bounds)
			: base(seriesData) {
			Rectangle initialDomainBounds = bounds.ToRectangle();
			this.bounds = initialDomainBounds;
			if (!initialDomainBounds.AreWidthAndHeightPositive())
				return;
			Rectangle domainBounds = initialDomainBounds;
			TitlesViewData = CalculateTitlesViewDataAndCorrectDomainBounds(textMeasurer, ref domainBounds);
			Rectangle innerBounds, labelsBounds;
			if (View.CalculateBounds(SeriesData, domainBounds, out innerBounds, out labelsBounds)) {
				domainBounds = CorrectBoundsAndTitlesPosition(domainBounds, ref labelsBounds, ref innerBounds);
				if (innerBounds.AreWidthAndHeightPositive() && domainBounds.AreWidthAndHeightPositive())
					domain = CreateDiagramDomain(domainBounds, labelsBounds, innerBounds);
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
						if (!string.IsNullOrEmpty(labelViewData.Text)) {
							SeriesData.Series.Label.CalculateLayout(labelLayoutList, pointLayout, textMeasurer);
							break;
						}
				AnnotationsAnchorPointsLayout.AddRange(View.CalculateAnnotationsAchorPointsLayout(domain, pointLayout));
			}
			this.annotationsViewData = AnnotationHelper.CreateInnerAnnotationsViewData(AnnotationsAnchorPointsLayout, textMeasurer, domain.LabelsBounds);
		}
		GraphicsCommand CreatePointsGraphicsCommand() {
			GraphicsCommand command = new ContainerGraphicsCommand();
			foreach (SeriesPointLayout pointLayout in this) {
				command.AddChildCommand(View.CreateGraphicsCommand(Domain.Bounds, pointLayout, pointLayout.DrawOptions));
			}
			return command;
		}
		protected SimpleDiagram3DDomain CreateDiagramDomain(Rectangle domainBounds, Rectangle labelsBounds, Rectangle diagramBounds) {
			SimpleDiagram3D diagram3D = SeriesData.Series.Chart.Diagram as SimpleDiagram3D;
			ISimpleDiagram3DSeriesView simpleDiagram3DSeriesView = View as ISimpleDiagram3DSeriesView;
			return (diagram3D == null || simpleDiagram3DSeriesView == null) ? null :
				SimpleDiagram3DDomain.Create(diagram3D, domainBounds, labelsBounds, diagramBounds, simpleDiagram3DSeriesView);
		}
		public GraphicsCommand CreateGraphicsCommand() {
			SimpleDiagram3DDomain domain3D = Domain as SimpleDiagram3DDomain;
			if (domain3D == null)
				return null;
			GraphicsCommand command = new ViewPortGraphicsCommand(domain3D.Bounds);
			GraphicsCommand innerCommand, innerCommandForLabelConnector;
			command.AddChildCommand(domain3D.CreateGraphicsCommand(out innerCommand, out innerCommandForLabelConnector));
			innerCommand.AddChildCommand(CreatePointsGraphicsCommand());
			return command;
		}
		public override void Render(IRenderer renderer) {
			foreach (SeriesPointLayout pointLayout in this)
				if (pointLayout != null)
					View.Render(renderer, domain.Bounds, pointLayout, pointLayout.PointData.DrawOptions);
			ProcessHitTesting(renderer);
		}
		public void RenderExclamation(IRenderer renderer) {
			if (domain == null || exclamationTextSize < 0.4)
				return;
			renderer.DrawBoundedText(ExclamationText, new NativeFontDisposable(new Font(ExclamationFontFamily, exclamationTextSize)),
				View.ExclamationTextColor, View, domain.ElementBounds, ExclamationAlignment, ExclamationLineAlignment);
		}
		public void RenderAnnotations(IRenderer renderer) {
			if (annotationsViewData == null)
				return;
			renderer.SetClipping(Domain.Bounds);
			foreach (AnnotationViewData viewData in annotationsViewData)
				viewData.Render(renderer);
			renderer.RestoreClipping();
		}
		public void RenderTitles(IRenderer renderer) {
			if (TitlesViewData == null)
				return;
			ZPlaneRectangle clipRect = new ZPlaneRectangle(new DiagramPoint(bounds.Left, bounds.Top), bounds.Width, bounds.Height);
			renderer.SetClipping((RectangleF)clipRect, CombineMode.Intersect);
			foreach (DockableTitleViewData item in TitlesViewData)
				item.Render(renderer);
			renderer.RestoreClipping();
		}
	}
}
