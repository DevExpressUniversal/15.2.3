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
	public static class Marker3DCalculator {
		static void SetTransform(Model3D model, Rect3D bounds, Point3D point) {
			Transform3DGroup group = new Transform3DGroup();
			Point3D center = MathUtils.CalcCenter(model.Bounds);
			Vector3D scaleVector = new Vector3D(bounds.SizeX / model.Bounds.SizeX, bounds.SizeY / model.Bounds.SizeY,
				bounds.SizeZ / model.Bounds.SizeZ);
			group.Children.Add(new ScaleTransform3D(scaleVector, center));
			Vector3D location = new Vector3D(point.X, point.Y, point.Z);
			group.Children.Add(new TranslateTransform3D(location));
			model.Transform = group;
		}
		public static Model3D CalculateMarker(Model3D model, Rect3D bounds, Point3D point, SolidColorBrush brush) {
			Model3D marker = model.Clone();
			SetTransform(marker, bounds, point);
			ColorUtils.MixBrushes(marker, brush);
			return marker;
		}
	}
	public abstract class MarkerSeries3DData : Series3DData {
		MarkerSeries3D MarkerSeries { get { return (MarkerSeries3D)base.Series; } }
		public MarkerSeries3DData(MarkerSeries3D series) : base(series) {
		}
		XYSeriesLabel3DLayout CreateLabelLayout(Model3D model) {
			if (!MarkerSeries.LabelsVisibility)
				return null;
			Point3D position = MathUtils.CalcCenter(model.Bounds);
			switch (PointSeries3D.GetLabelPosition(MarkerSeries.ActualLabel)) {
				case Marker3DLabelPosition.Top:
					Point3D topPoint = new Point3D(position.X, position.Y + model.Bounds.SizeY / 2, position.Z);
					return new Marker3DLabelLayout(Series.ActualLabel, topPoint, position, new Vector3D(0, 1, 0), model);
				case Marker3DLabelPosition.Center:
					return new Marker3DLabelLayout(Series.ActualLabel, position, position, new Vector3D(), model);
				default:
					ChartDebug.Fail("Unknown Marker3DLabelPosition value.");
					return null;
			}
		}
		protected abstract double CalculateMarkerSize(XYDiagram3DDomain domain, RefinedPoint refinedPoint);
		protected override Model3DInfo CreateSeriesPointModelInfo(RefinedPoint refinedPoint, Diagram3DDomain domain) {
			XYDiagram3DDomain xyDomain = (XYDiagram3DDomain)domain;
			Point3D point = xyDomain.GetDiagramPoint(Series, refinedPoint.Argument, ((IXYPoint)refinedPoint).Value);
			double size = CalculateMarkerSize(xyDomain, refinedPoint);
			Rect3D bounds = new Rect3D(new Point3D(), new Size3D(size, size, size));
			SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint);
			if (seriesPoint == null)
				return null;
			Model3D model = Marker3DCalculator.CalculateMarker(MarkerSeries.ActualModel.GetModel(), bounds, point,
				Series.MixColor(Series.GetSeriesPointColor(refinedPoint)));
			return new XYSeriesModel3DInfo(seriesPoint, model, CreateLabelLayout(model));
		}
		protected internal override Point3D CalculateToolTipPoint(SeriesPointCache pointCache, Diagram3DDomain domain) {
			Point3D toolTipPoint = new Point3D();
			Series3DPointGeometry geometry = pointCache.PointGeometry as Series3DPointGeometry;
			if (geometry != null)
				toolTipPoint = MathUtils.CalcCenter(geometry.ModelInfo.Model.Bounds);
			return toolTipPoint;
		}
	}
}
