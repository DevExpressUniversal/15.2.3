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

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using System.Drawing;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(RadarPointSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RadarPointSeriesLabel : PointSeriesLabel {
		public RadarPointSeriesLabel()
			: base() {
		}
		XYDiagramSeriesLabelLayout CalculateLayoutForCenter(DiagramPoint anchorPoint, TextMeasurer textMeasurer, RefinedPointData pointData, SeriesLabelViewData labelViewData, int markerSize) {
			TextPainter painter = labelViewData.CreateTextPainterForCenterDrawing(this, textMeasurer, anchorPoint);
			RectangleF validRectangle = SeriesLabelHelper.CalculateValidRectangleForCenterPosition(anchorPoint, markerSize);
			return XYDiagramSeriesLabelLayout.CreateWithValidRectangle(pointData, pointData.DrawOptions.Color, painter, null, ResolveOverlappingMode, anchorPoint, validRectangle);
		}
		XYDiagramSeriesLabelLayout CalculateLayoutForOutside(DiagramPoint startPoint, TextMeasurer textMeasurer, RefinedPointData pointData, SeriesLabelViewData labelViewData) {
			double angle = MathUtils.Degree2Radian(MathUtils.NormalizeDegree(Angle));
			DiagramPoint finishPoint = CalculateFinishPoint(angle, startPoint);
			TextPainter painter = labelViewData.CreateTextPainterForFlankDrawing(this, textMeasurer, finishPoint, angle);
			return XYDiagramSeriesLabelLayout.CreateWithExcludedRectangle(pointData, pointData.DrawOptions.Color, painter,
				ActualLineVisible ? new LineConnectorPainter(startPoint, finishPoint, angle, (ZPlaneRectangle)painter.BoundsWithBorder, true) : null,
				ResolveOverlappingMode, startPoint, SeriesLabelHelper.CalcAnchorHoleForPoint(startPoint, LineLength));
		}
		protected internal override void CalculateLayout(SeriesLabelLayoutList labelLayoutList, RefinedPointData pointData, TextMeasurer textMeasurer) {
			RadarDiagramSeriesLabelLayoutList radarLabelLayoutList = labelLayoutList as RadarDiagramSeriesLabelLayoutList;
			if (radarLabelLayoutList == null) {
				ChartDebug.Fail("RadarDiagramSeriesLabelsLayout expected.");
				return;
			}
			RadarPointSeriesView view = Series.View as RadarPointSeriesView;
			if (view == null) {
				ChartDebug.Fail("RadarPointSeriesView expected.");
				return;
			}
			RadarDiagramMapping mapping = radarLabelLayoutList.DiagramMapping;
			if (mapping == null)
				return;
			IXYPoint xyPoint = pointData.RefinedPoint;
			SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
			DiagramPoint anchorPoint = mapping.GetScreenPoint(xyPoint.Argument, xyPoint.Value, true);
			if (anchorPoint.IsZero)
				return;
			XYDiagramSeriesLabelLayout layout = Position == PointLabelPosition.Outside ? 
				CalculateLayoutForOutside(anchorPoint, textMeasurer, pointData, labelViewData) :
				CalculateLayoutForCenter(anchorPoint, textMeasurer, pointData, labelViewData, view.PointMarkerOptions.Size);
			if (layout != null)
				radarLabelLayoutList.Add(layout);
		}
		protected override ChartElement CreateObjectForClone() {
			return new RadarPointSeriesLabel();
		}
	}
}
