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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(Area3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Area3DSeriesView : Line3DSeriesView {
		static void AddSinglePolygon(XYDiagram3DCoordsCalculator coordsCalculator, List<PlanePolygon> polygons, DiagramPoint point1, DiagramPoint point2, double offset, int seriesWeight, Color color) {
			if (MathUtils.ArePointsEquals2D(point1, point2, Diagram3D.Epsilon))
				return;
			double zMin = point1.Z - offset;
			double zMax = point1.Z + offset;
			PlanePolygon rect = new PlanePolygon(new DiagramPoint[] {
				new DiagramPoint(point1.X, point1.Y, zMin), new DiagramPoint(point2.X, point2.Y, zMin),
				new DiagramPoint(point2.X, point2.Y, zMax), new DiagramPoint(point1.X, point1.Y, zMax) },
				true, true, new DiagramVector(0, 0, 1), color);
			rect.Normal = MathUtils.CalcNormal(rect);
			rect.Weight = seriesWeight;
			IList<PlanePolygon> clipped = coordsCalculator.ClipPolygons(new PlanePolygon[] { rect });
			if (clipped != null && clipped.Count > 0)
				polygons.Add(clipped[0]);
		}
		const double DefaultAreaWidth = 0.6;
		double areaWidth = DefaultAreaWidth;
		PolygonFillStyle legendFillStyle;
		protected override byte DefaultOpacity { get { return ConvertBetweenOpacityAndTransparency((byte)135); } }
		protected internal PolygonFillStyle LegendFillStyle { get { return legendFillStyle; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnArea3D); } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int LineThickness { get { return 0; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new double LineWidth { get { return 0; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Area3DSeriesViewAreaWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Area3DSeriesView.AreaWidth"),
		XtraSerializableProperty
		]
		public double AreaWidth {
			get { return areaWidth; }
			set {
				if (value != areaWidth) {
					if (value <= 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAreaWidth));
					SendNotification(new ElementWillChangeNotification(this));
					areaWidth = value;
					RaiseControlChanged();
				}
			}
		}
		public Area3DSeriesView() : base() {
			legendFillStyle = new PolygonFillStyle(null);
			legendFillStyle.FillMode = FillMode.Solid;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "AreaWidth" ? ShouldSerializeAreaWidth() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeAreaWidth() {
			return areaWidth != DefaultAreaWidth;
		}
		void ResetAreaWidth() {
			AreaWidth = DefaultAreaWidth;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeAreaWidth();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new Area3DSeriesView();
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new AreaGeometryStripCreator();
		}
		protected override List<PlanePolygon> CalculatePolygons(XYDiagram3DCoordsCalculator coordsCalculator, XYDiagram3DSeriesLayout seriesLayout, IList<IGeometryStrip> strips) {
			double epsilon = Math.Min(coordsCalculator.Diagram.AxisX.VisualRangeData.Delta / coordsCalculator.Bounds.Width,
									  coordsCalculator.Diagram.AxisY.VisualRangeData.Delta / coordsCalculator.Bounds.Height) * 0.01;
			double areaWidth = coordsCalculator.CalcSeriesWidth(this);
			double halfWidth = areaWidth / 2.0;
			int seriesWeight = coordsCalculator.GetSeriesWeight(Series);
			Color color = seriesLayout.SeriesData.DrawOptions.Color;
			List<PlanePolygon> result = new List<PlanePolygon>();
			foreach (RangeStrip strip in strips) {
				RangeStripTriangulationResult triangulationResult = RangeStripTriangulation.Triangulate(strip, epsilon);
				foreach (GPolygon2D polygon in triangulationResult.Polygons) {
					List<DiagramPoint> nearVertices = new List<DiagramPoint>(polygon.Vertices.Length);
					List<DiagramPoint> farVertices = new List<DiagramPoint>(polygon.Vertices.Length);
					foreach (GRealPoint2D point in polygon.Vertices) {
						DiagramPoint p = coordsCalculator.GetDiagramPoint(Series, point.X, point.Y, false);
						nearVertices.Add(DiagramPoint.Offset(p, 0, 0, halfWidth));
						farVertices.Add(DiagramPoint.Offset(p, 0, 0, -halfWidth));
					}
					PlanePolygon nearPolygon = new PlanePolygon(nearVertices.ToArray(), true, true, new DiagramVector(0, 0, 1), color);
					nearPolygon.Weight = seriesWeight;
					PlanePolygon farPolygon = new PlanePolygon(farVertices.ToArray(), true, true, new DiagramVector(0, 0, 1), color);
					farPolygon.Weight = seriesWeight;
					IList<PlanePolygon> clipped = coordsCalculator.ClipPolygons(new PlanePolygon[] { nearPolygon, farPolygon });
					if (clipped != null)
						result.AddRange(clipped);
				}
				LineStrip borderStrip = triangulationResult.BorderStrip;
				int borderLength = borderStrip.Count;
				if (borderLength > 1) {
					DiagramPoint firstPoint = coordsCalculator.GetDiagramPoint(Series, borderStrip[0].X, borderStrip[0].Y, false);
					DiagramPoint previousPoint = firstPoint;
					for (int i = 1; i < borderLength; i++) {
						DiagramPoint point = coordsCalculator.GetDiagramPoint(Series, borderStrip[i].X, borderStrip[i].Y, false);
						AddSinglePolygon(coordsCalculator, result, previousPoint, point, halfWidth, seriesWeight, color);
						previousPoint = point;
					}
					AddSinglePolygon(coordsCalculator, result, previousPoint, firstPoint, halfWidth, seriesWeight, color);
				}
			}
			return result;
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			Line3DDrawOptions lineDrawOptions = seriesPointDrawOptions as Line3DDrawOptions;
			if (lineDrawOptions == null)
				return;
			renderer.EnablePolygonAntialiasing(true);
			StripsUtils.Render(renderer, StripsUtils.CreateAreaMarkerStrip(this, bounds, false),
				legendFillStyle.Options, lineDrawOptions.Color, lineDrawOptions.ActualColor2, new SeriesHitTestState(), null, selectionState);
			renderer.RestorePolygonAntialiasing();
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new Area3DDrawOptions(this);
		}
		protected override IGeometryStrip CreateStripInternal() {
			return new RangeStrip();
		}
		protected internal override MinMaxValues GetSeriesPointValues(RefinedPoint refinedPoint) {
			return new MinMaxValues(0.0, ((IXYPoint)refinedPoint).Value);
		}
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData) {
			return null;
		}
		protected internal override double GetSeriesDepth() {
			return areaWidth;
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new Area3DSeriesLabel();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Area3DSeriesView view = obj as Area3DSeriesView;
			if (view != null)
				areaWidth = view.areaWidth;
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return areaWidth == ((Area3DSeriesView)obj).areaWidth;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
