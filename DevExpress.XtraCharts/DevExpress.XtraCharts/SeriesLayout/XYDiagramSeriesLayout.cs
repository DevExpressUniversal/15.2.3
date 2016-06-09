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
	public class XYDiagramSeriesLayout : SeriesLayout {
		readonly XYDiagramMappingBase diagramMapping;
		readonly bool singleLayout;
		WholeSeriesLayout wholeLayout;
		Rectangle MappingBounds { get { return diagramMapping.InflatedBounds; } }
		public XYDiagramMappingBase DiagramMapping { get { return diagramMapping; } }
		public bool SingleLayout { get { return singleLayout; } }
		public new XYDiagram2DSeriesViewBase View { get { return (XYDiagram2DSeriesViewBase)base.View; } }
		public override WholeSeriesLayout WholeLayout { get { return wholeLayout; } }
		public XYDiagramSeriesLayout(RefinedSeriesData seriesData, XYDiagramMappingBase diagramMapping, bool singleLayout) : base(seriesData) {
			this.diagramMapping = diagramMapping;
			this.singleLayout = singleLayout;
		}
		void RenderWholeSeries(IRenderer renderer, HitRegionContainer seriesHitRegion) {
			if (WholeLayout == null)
				return;
			View.RenderWholeSeries(renderer, MappingBounds, WholeLayout);
			HitTestController hitTestController = HitTestController;
			bool toolTipsEnabled = HitTestController.SeriesToolTipEnabled(SeriesData.Series);
			if (hitTestController.Enabled && View.HitTestingSupported || toolTipsEnabled) {
				using (HitRegionContainer wholeSeriesHitRegion = WholeLayout.CalculateHitRegion(SeriesData.DrawOptions)) {
					if (seriesHitRegion != null) {
						HitRegionContainer hitRegion = seriesHitRegion;
						wholeSeriesHitRegion.Intersect(hitRegion);
					}
					if (hitTestController.Enabled && View.HitTestingSupported)
						renderer.ProcessHitTestRegion(hitTestController, View.Series, null, (IHitRegion)wholeSeriesHitRegion.Underlying.Clone());
					if (toolTipsEnabled)
						renderer.ProcessHitTestRegion(hitTestController, View.Series, new ChartFocusedArea(View.Series), (IHitRegion)wholeSeriesHitRegion.Underlying.Clone(), true);
				}
			}
		}
		public void Calculate(TextMeasurer textMeasurer) {
			foreach (RefinedPointData pointData in SeriesData)
				if (!pointData.RefinedPoint.IsEmpty) {
					SeriesPointLayout pointLayout = View.CalculateSeriesPointLayout(diagramMapping, pointData);
					if (pointLayout != null) {
						pointData.ToolTipRelativePosition = View.CalculateRelativeToolTipPosition(diagramMapping, pointLayout.PointData);
						Add(pointLayout);
					}
				}
			wholeLayout = View.CalculateWholeSeriesLayout(diagramMapping, this);
		}
		public override void Render(IRenderer renderer) {
		}
		public void Render(IRenderer renderer, HitRegionContainer seriesHitRegion) {
			RenderWholeSeries(renderer, seriesHitRegion);
			foreach (SeriesPointLayout pointLayout in this)
				View.Render(renderer, MappingBounds, pointLayout, pointLayout.PointData.DrawOptions);
			HitTestController hitTestController = HitTestController;
			bool toolTipsEnabled = HitTestController.PointToolTipEnabled(SeriesData.Series);
			if (hitTestController.Enabled && View.HitTestingSupported || toolTipsEnabled) {
				foreach (SeriesPointLayout pointLayout in this) {
					HitRegionContainer pointHitRegion = pointLayout.CalculateHitRegion();
					if (pointHitRegion != null) {
						using (pointHitRegion) {
							if (seriesHitRegion != null)
								pointHitRegion.Intersect(seriesHitRegion);
							ISeriesPoint seriesPoint = pointLayout.PointData.SeriesPoint;
							if (hitTestController.Enabled && View.HitTestingSupported)
								renderer.ProcessHitTestRegion(hitTestController, View.Series, pointLayout.PointData.RefinedPoint, (IHitRegion)pointHitRegion.Underlying.Clone());
							if (toolTipsEnabled)
								renderer.ProcessHitTestRegion(hitTestController, View.Series, new ChartFocusedArea(seriesPoint, pointLayout.PointData.ToolTipRelativePosition), (IHitRegion)pointHitRegion.Underlying.Clone(), true);
						}
					}
				}
			}
		}
		public void RenderShadow(IRenderer renderer) {
			base.RenderShadow(renderer, MappingBounds);
		}
	}
	public class XYDiagramSeriesLayoutList : List<XYDiagramSeriesLayout> {
		readonly XYDiagramMappingList mappingList;
		public XYDiagramMappingList MappingList { get { return mappingList; } }
		public XYDiagramSeriesLayoutList(XYDiagramMappingList mappingList) {
			this.mappingList = mappingList;
		}
		public void Initialize(RefinedSeriesData seriesData) {
			XYDiagramMappingContainer mappingContainer = mappingList.GetMappingContainer(seriesData);
			if (mappingContainer != null)
				foreach (XYDiagramMappingBase mapping in mappingContainer)
					Add(new XYDiagramSeriesLayout(seriesData, mapping, mappingContainer.Count == 1));
		}
	}
}
