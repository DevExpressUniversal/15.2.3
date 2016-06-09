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

using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class BarSeries3DData : Series3DData {
		BarSeries3D BarSeries { get { return (BarSeries3D)base.Series; } }
		public BarSeries3DData(BarSeries3D series) : base(series) {
		}
		Point3D CalcSeriesLabelPoint(Rect3D modelBounds, bool invertByY) {
			Point3D center = MathUtils.CalcCenter(modelBounds);
			double offset = invertByY ? -modelBounds.SizeY / 2 : modelBounds.SizeY / 2;
			center.Offset(0, offset, 0);
			return center;
		}
		Point3D GetLowPoint(RefinedPoint pointInfo, XYDiagram3DDomain domain) {
			return domain.GetDiagramPoint(Series, pointInfo.Argument, 0);
		}
		Point3D GetHighPoint(ISideBySidePoint refinedPoint, XYDiagram3DDomain domain) {
			return domain.GetDiagramPoint(Series, refinedPoint.Argument, refinedPoint.Value);
		}
		protected Model3DInfo CreateModelInfo(Point3D lowPoint, Point3D highPoint, double barWidth, RefinedPoint pointInfo) {
			lowPoint.Offset(-barWidth / 2, 0, -barWidth / 2);
			bool invertByY = highPoint.Y < lowPoint.Y;
			SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(pointInfo.SeriesPoint);
			if (seriesPoint == null)
				return null;
			double height = highPoint.Y - lowPoint.Y;
			height = height == 0 ? double.Epsilon : height;
			Box box = new Box(lowPoint, barWidth, height, barWidth);
			SolidColorBrush brush = Series.MixColor(Series.GetSeriesPointColor(pointInfo));
			Model3D model = Bar3DCalculator.CalculateBar(BarSeries.ActualModel.SectionsData, box.Bounds, invertByY, brush);
			XYSeriesLabel3DLayout labelLayout = BarSeries.LabelsVisibility ? new XYSeriesLabel3DLayout(Series.ActualLabel, CalcSeriesLabelPoint(model.Bounds, invertByY), new Vector3D(0, invertByY ? -1 : 1, 0)) : null;
			return new XYSeriesModel3DInfo(seriesPoint, model, labelLayout);
		}
		protected override Model3DInfo CreateSeriesPointModelInfo(RefinedPoint refinedPoint, Diagram3DDomain domain) {
			XYDiagram3DDomain xyDomain = (XYDiagram3DDomain)domain;
			return CreateModelInfo(GetLowPoint(refinedPoint, xyDomain), GetHighPoint(refinedPoint, xyDomain), xyDomain.GetValueByAxisX(BarSeries.BarWidth), refinedPoint);
		}
		protected internal override Point3D CalculateToolTipPoint(SeriesPointCache pointCache, Diagram3DDomain domain) {
			RefinedPoint refinedPoint = pointCache.RefinedPoint;
			XYDiagram3DDomain xyDomain = domain as XYDiagram3DDomain;
			Point3D toolTipPoint = new Point3D();
			Series3DPointGeometry geometry = pointCache.PointGeometry as Series3DPointGeometry;
			if (geometry != null)
				toolTipPoint = CalcSeriesLabelPoint(geometry.ModelInfo.Model.Bounds, GetHighPoint(refinedPoint, xyDomain).Y < GetLowPoint(refinedPoint, xyDomain).Y);
			return toolTipPoint;
		}
	}
}
