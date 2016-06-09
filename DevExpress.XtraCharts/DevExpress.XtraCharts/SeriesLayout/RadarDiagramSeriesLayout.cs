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

using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class RadarDiagramSeriesLayout : SeriesLayout {
		readonly RadarDiagramMapping diagramMapping;
		WholeSeriesLayout wholeLayout;
		public RadarDiagramMapping DiagramMapping { get { return diagramMapping; } }
		Rectangle MappingBounds { get { return diagramMapping.MappingBounds; } }
		public new RadarSeriesViewBase View { get { return (RadarSeriesViewBase)SeriesData.Series.View; } }
		public override WholeSeriesLayout WholeLayout { get { return wholeLayout; } }
		protected override HitTestController HitTestController { get { return View.HitTestController; } }
		public override WholeSeriesViewData WholeViewData { get { return SeriesData.WholeViewData; } }
		public override DrawOptions DrawOptions { get { return SeriesData.DrawOptions; } }
		public RadarDiagramSeriesLayout(RefinedSeriesData seriesData, RadarDiagramMapping diagramMapping)
			: base(seriesData) {
			this.diagramMapping = diagramMapping;
		}
		public void Calculate() {
			foreach (RefinedPointData pointData in SeriesData)
				if (!pointData.RefinedPoint.IsEmpty) {
					SeriesPointLayout pointLayout = View.CalculateSeriesPointLayout(DiagramMapping, pointData);
					if (pointLayout != null) {
						pointData.ToolTipRelativePosition = View.CalculateRelativeToolTipPosition(diagramMapping, pointData);
						Add(pointLayout);
					}
				}
			wholeLayout = View.CalculateWholeSeriesLayout(DiagramMapping, this);
		}
		void RenderWholeSeries(IRenderer renderer) {
			if (WholeLayout == null)
				return;
			View.RenderWholeSeries(renderer, MappingBounds, WholeLayout);
			HitTestController hitTestController = HitTestController;
			bool toolTipsEnabled = HitTestController.SeriesToolTipEnabled(SeriesData.Series);
			if (hitTestController.Enabled || toolTipsEnabled) {
				using (HitRegionContainer wholeSeriesHitRegion = WholeLayout.CalculateHitRegion(SeriesData.DrawOptions)) {
					wholeSeriesHitRegion.Intersect(new HitRegion(MappingBounds));
					IHitRegion underlyingHitRegion = wholeSeriesHitRegion.Underlying;
					renderer.ProcessHitTestRegion(hitTestController, View.Series, null, (IHitRegion)underlyingHitRegion.Clone());
					if (toolTipsEnabled)
						renderer.ProcessHitTestRegion(hitTestController, View.Series, new ChartFocusedArea(View.Series), (IHitRegion)underlyingHitRegion.Clone(), true);
				}
			}			
		}
		public override void Render(IRenderer renderer) {
			RenderWholeSeries(renderer);
			foreach (SeriesPointLayout pointLayout in this)
				View.Render(renderer, MappingBounds, pointLayout, pointLayout.DrawOptions);
			HitTestController hitTestController = HitTestController;
			bool toolTipsEnabled = HitTestController.PointToolTipEnabled(SeriesData.Series);
			if (hitTestController.Enabled || toolTipsEnabled) {
				foreach (SeriesPointLayout pointLayout in this) {
					HitRegionContainer pointHitRegion = pointLayout.CalculateHitRegion();
					if (pointHitRegion != null) {
						using (pointHitRegion) {
							pointHitRegion.Intersect(new HitRegion(MappingBounds));
							ISeriesPoint seriesPoint = pointLayout.SeriesPoint;
							IHitRegion underlyingHitRegion = pointHitRegion.Underlying;
							renderer.ProcessHitTestRegion(hitTestController, View.Series, pointLayout.PointData.RefinedPoint, (IHitRegion)underlyingHitRegion.Clone());
							if (toolTipsEnabled)
								renderer.ProcessHitTestRegion(hitTestController, View.Series,
									new ChartFocusedArea(seriesPoint, pointLayout.ToolTipRelativePosition), (IHitRegion)underlyingHitRegion.Clone(), true);
						}
					}
				}
			}
		}
		public GraphicsCommand CreateShadowGraphicsCommand() {
			return null;
		}
		public void RenderShadow(IRenderer renderer) {
			if (WholeLayout != null)
				View.RenderWholeSeriesShadow(renderer, MappingBounds, WholeLayout);
			foreach (SeriesPointLayout pointLayout in this)
				View.RenderShadow(renderer, MappingBounds, pointLayout, pointLayout.DrawOptions);
		}		
	}
}
