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
	public abstract class SeriesLabelLayoutList : List<SeriesLabelLayout> {
		readonly RefinedSeriesData seriesData;
		SeriesViewBase View { get { return Series.View; } }
		protected virtual Series Series { get { return seriesData.Series; } }
		public RefinedSeriesData SeriesData { get { return seriesData; } }
		public SeriesLabelBase Label { get { return Series.Label; } }
		public SeriesLabelLayoutList(RefinedSeriesData seriesData) {
			this.seriesData = seriesData;
		}
		protected virtual void CalculateLabelsLayout(TextMeasurer textMeasurer) {
			foreach (RefinedPointData pointData in seriesData)
				if (!pointData.RefinedPoint.IsEmpty)
					foreach (SeriesLabelViewData labelViewData in pointData.LabelViewData)
						if (!string.IsNullOrEmpty(labelViewData.Text)) {
							Label.CalculateLayout(this, pointData, textMeasurer);
							break;
						}
		}
		public void CalculateDiagramBoundsCorrection(RectangleCorrection correction) {
			foreach (SeriesLabelLayout labelLayout in this)
				if (labelLayout.Visible)
					labelLayout.CalculateDiagramBoundsCorrection(correction);
		}
		public void RenderSeriesLabels(IRenderer renderer) {
			HitTestController hitTestController = View.HitTestController;
			foreach (SeriesLabelLayout layout in this)
				layout.Render(renderer, hitTestController);
		}
		public void RenderSeriesLabels(IRenderer renderer, Rectangle clipBounds) {
			HitTestController hitTestController = View.HitTestController;
			foreach (SeriesLabelLayout layout in this)
				layout.Render(renderer, hitTestController, clipBounds);
		}
		public void RenderSeriesLabelConnectors(IRenderer renderer) {
			SeriesLabelBase label = Label;
			GraphicsCommand command = new ContainerGraphicsCommand();
			foreach (SeriesLabelLayout layout in this)
				layout.RenderConnector(renderer, label);
		}
	}
	public class XYDiagramSeriesLabelLayoutList : SeriesLabelLayoutList {
		readonly XYDiagramMappingContainer mappingContainer;
		public XYDiagramMappingContainer MappingContainer { get { return mappingContainer; } }
		public XYDiagram2DSeriesViewBase View { get { return (XYDiagram2DSeriesViewBase)Series.View; } }
		public XYDiagramSeriesLabelLayoutList(RefinedSeriesData seriesData, XYDiagramMappingContainer mappingContainer, TextMeasurer textMeasurer)
			: base(seriesData) {
			this.mappingContainer = mappingContainer;
			CalculateLabelsLayout(textMeasurer);
		}
		public XYDiagramMappingBase GetMapping(double argument, double value) {
			return View.IsScrollingEnabled ? mappingContainer.MappingForScrolling : mappingContainer.GetMapping(argument, value);
		}
		public void CalculateLabelsVisibility() {
			if (View.IsScrollingEnabled) {
				RectangleF mappingBounds = (RectangleF)mappingContainer.MappingBounds;
				foreach (SeriesLabelLayout layout in this)
					if (!mappingBounds.IntersectsWith(layout.Bounds) &&
						(layout.ConnectorPainter == null || !mappingBounds.Contains((PointF)layout.ConnectorPainter.StartPoint)))
						layout.Visible = false;
			}
		}
	}
	public class RadarDiagramSeriesLabelLayoutList : SeriesLabelLayoutList {
		readonly RadarDiagramMapping diagramMapping;
		public RadarDiagramMapping DiagramMapping { get { return diagramMapping; } }
		public RadarDiagramSeriesLabelLayoutList(RefinedSeriesData seriesData, RadarDiagramMapping diagramMapping, TextMeasurer textMeasurer)
			: base(seriesData) {			
			this.diagramMapping = diagramMapping;
			CalculateLabelsLayout(textMeasurer);
		}		
	}
	public class XYDiagram3DSeriesLabelLayoutList : SeriesLabelLayoutList {
		readonly XYDiagram3DCoordsCalculator coordsCalculator;
		readonly IAxisRangeData axisRangeY;
		public XYDiagram3DCoordsCalculator CoordsCalculator { get { return coordsCalculator; } }
		public IAxisRangeData AxisRangeY { get { return axisRangeY; } }
		public XYDiagram3DSeriesViewBase View { get { return (XYDiagram3DSeriesViewBase)Series.View; } }
		public XYDiagram3DSeriesLabelLayoutList(RefinedSeriesData seriesData, XYDiagram3DCoordsCalculator coordsCalculator, IAxisRangeData axisRangeY, TextMeasurer textMeasurer)
			: base(seriesData) {				
				this.coordsCalculator = coordsCalculator;
				this.axisRangeY = axisRangeY;
			CalculateLabelsLayout(textMeasurer);
		}
		public void AddLabelLayout(XYDiagramSeriesLabelLayout layout) {
			Rectangle mappingBounds = new Rectangle(0, 0, (int)Math.Round(coordsCalculator.Width), (int)Math.Round(coordsCalculator.Height));
			if (!XYDiagramMappingHelper.PointInMappingBounds(mappingBounds, layout.AnchorPoint))
				layout.Visible = false;
			Add(layout);
		}
		public void FillPrimitivesContainer(PrimitivesContainer container) {
			SeriesLabelBase label = Label;
			foreach (SeriesLabelLayout layout in this)
				layout.FillPrimitives(label, container);
		}
	}
	public class SimpleDiagramSeriesLabelLayoutList : SeriesLabelLayoutList {
		readonly ISimpleDiagramDomain domain;
		public ISimpleDiagramDomain Domain { get { return domain; } }
		public SimpleDiagramSeriesViewBase View { get { return (SimpleDiagramSeriesViewBase)Series.View; } }		
		public SimpleDiagramSeriesLabelLayoutList(RefinedSeriesData seriesData, ISimpleDiagramDomain domain) : base(seriesData) {
			this.domain = domain;			
		}
	}
}
