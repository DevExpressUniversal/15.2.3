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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(Spline3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Spline3DSeriesView : Line3DSeriesView, ISplineSeriesView {
		static void CalcNormals(Line topLine, Line bottomLine, out DiagramVector topNormal, out DiagramVector bottomNormal) {
			DiagramVector direction = new DiagramVector(topLine.V2.X - topLine.V1.X, topLine.V2.Y - topLine.V1.Y, 0);
			topNormal = MathUtils.CalcNormal(direction, new DiagramVector(0, 0, -1));
			direction = new DiagramVector(bottomLine.V2.X - bottomLine.V1.X, bottomLine.V2.Y - bottomLine.V1.Y, 0);
			bottomNormal = MathUtils.CalcNormal(direction, new DiagramVector(0, 0, 1));
		}
		static List<PlanePolygon> GetPolygons(XYDiagram3DCoordsCalculator coordsCalculator, Line topLine, Line bottomLine, DiagramVector topLeftNormal, DiagramVector topRightNormal, DiagramVector bottomLeftNormal, DiagramVector bottomRightNormal, bool leftVisible, bool rightVisible, double lineWidth, int seriesWeight, Color color) {
			Prizm prizm = new Prizm(new PlanePolygon(new DiagramPoint[] { topLine.V2, topLine.V1, bottomLine.V1, bottomLine.V2 }), lineWidth);
			prizm.Laterals[0].SameNormals = false;
			prizm.Laterals[0].Vertices[0].Normal = topRightNormal;
			prizm.Laterals[0].Vertices[1].Normal = topLeftNormal;
			prizm.Laterals[0].Vertices[2].Normal = topLeftNormal;
			prizm.Laterals[0].Vertices[3].Normal = topRightNormal;
			prizm.Laterals[1].Visible = leftVisible;
			prizm.Laterals[2].SameNormals = false;
			prizm.Laterals[2].Vertices[0].Normal = bottomLeftNormal;
			prizm.Laterals[2].Vertices[1].Normal = bottomRightNormal;
			prizm.Laterals[2].Vertices[2].Normal = bottomRightNormal;
			prizm.Laterals[2].Vertices[3].Normal = bottomLeftNormal;
			prizm.Laterals[3].Visible = rightVisible;
			return SeriesView3DHelper.GetPolygons(coordsCalculator, prizm, seriesWeight, color);
		}
		const int DefaultLineTensionPercent = 80;
		int lineTensionPercent = DefaultLineTensionPercent;
		double LineTension { get { return (double)lineTensionPercent / 100; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnSpline3D); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Spline3DSeriesViewLineTensionPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Spline3DSeriesView.LineTensionPercent"),
		XtraSerializableProperty
		]
		public int LineTensionPercent {
			get { return lineTensionPercent; }
			set {
				if (value > 100 || value < 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLineTensionPercent));
				if (lineTensionPercent != value) {
					SendNotification(new ElementWillChangeNotification(this));
					lineTensionPercent = value;
					RaiseControlChanged();
				}
			}
		}
		public Spline3DSeriesView() {
		}
		#region ISplineSeriesView
		bool ISplineSeriesView.ShouldCorrectRanges { get { return true; } }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "LineTensionPercent" ? ShouldSerializeLineTensionPercent() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLineTensionPercent() {
			return lineTensionPercent != DefaultLineTensionPercent;
		}
		void ResetLineTensionPercent() {
			LineTensionPercent = DefaultLineTensionPercent;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeLineTensionPercent();
		}
		#endregion
		List<PlanePolygon> CreateSplineSegmentPolygons(XYDiagram3DCoordsCalculator coordsCalculator, LineStrip strip, List<SeriesPointLayout> pointsLayout, LineStrip pointsStrip, double lineWidth, int seriesWeight, int lineThickness, Color color) {
			if (strip.Count == 0)
				return new List<PlanePolygon>();
			double z = coordsCalculator.CalcSeriesOffset(Series);
			if (strip.IsEmpty)
				return CreatePolygonsForSinglePoint(coordsCalculator, strip[0], z, pointsLayout[0], lineWidth, seriesWeight, lineThickness, color);
			SeriesPointLayout layout;
			Line3DDrawOptions drawOptions = null;
			View3DSeriesPointLayout pointLayout = null;
			float halfThickness = (float)(lineThickness / 2.0);
			List<PlanePolygon> polygons = new List<PlanePolygon>();
			DiagramPoint previousPoint = new DiagramPoint(strip[0].X, strip[0].Y, z);
			Line previousTopLine = null;
			Line previousBottomLine = null;
			DiagramVector previousTopNormal = new DiagramVector(0, 1, 0);
			DiagramVector previousBottomNormal = new DiagramVector(0, -1, 0);
			DiagramVector previousTopPolygonNormal = new DiagramVector(0, 1, 0);
			DiagramVector previousBottomPolygonNormal = new DiagramVector(0, -1, 0);
			bool leftVisible = true;
			int pointsStripLength = pointsStrip.Count;
			int pointIndex = 0;
			bool breakNormals = false;
			DiagramPoint pointsStripPoint = new DiagramPoint(pointsStrip[pointIndex].X, pointsStrip[pointIndex].Y, z);
			for (int i = 1; i < strip.Count; i++) {
				DiagramPoint point = new DiagramPoint(strip[i].X, strip[i].Y, z);
				if (pointsStripPoint == previousPoint) {
					layout = pointsLayout[pointIndex];
					if(layout != null) {
						drawOptions = layout.DrawOptions as Line3DDrawOptions;
						pointLayout = layout as View3DSeriesPointLayout;
					}
					if (++pointIndex < pointsStripLength)
						pointsStripPoint = new DiagramPoint(pointsStrip[pointIndex].X, pointsStrip[pointIndex].Y, z);
				}
				if (MathUtils.ArePointsEquals2D(point, previousPoint, Diagram3D.Epsilon)) {
					breakNormals = true;
				}
				else {
					Line topLine, bottomLine;
					CalcLines(previousPoint, point, halfThickness, out topLine, out bottomLine);
					if (previousTopLine == null || previousBottomLine == null) {
						CalcNormals(topLine, bottomLine, out previousTopPolygonNormal, out previousBottomPolygonNormal);
						previousTopNormal = previousTopPolygonNormal;
						previousBottomNormal = previousBottomPolygonNormal;
					}
					else {
						DiagramVector topPolygonNormal;
						DiagramVector bottomPolygonNormal;
						CalcNormals(topLine, bottomLine, out topPolygonNormal, out bottomPolygonNormal);
						DiagramVector topNormal, bottomNormal;
						if (breakNormals) {
							topNormal = previousTopPolygonNormal;
							bottomNormal = previousBottomPolygonNormal;
						}
						else {
							topNormal = previousTopPolygonNormal + topPolygonNormal;
							topNormal.Normalize();
							bottomNormal = previousBottomPolygonNormal + bottomPolygonNormal;
							bottomNormal.Normalize();
						}
						DiagramPoint topJointPoint, bottomJointPoint;
						Line topJointLine, bottomJointLine;
						CalcJoints(ref previousTopLine, ref topLine, out topJointPoint, out topJointLine, true);
						CalcJoints(ref previousBottomLine, ref bottomLine, out bottomJointPoint, out bottomJointLine, false);
						bool revertNormals = breakNormals;
						if (topJointLine != null && bottomJointLine != null) {
							double angle = MathUtils.CalcAngleBetweenLines2D(previousTopLine.V1, previousTopLine.V2, 
								topLine.V1, topLine.V2, Diagram3D.Epsilon);
							if (Math.Abs(angle) > Math.PI / 2.0) {
								List<PlanePolygon> jointPolygons = SeriesView3DHelper.GetPolygons(coordsCalculator, 
									topJointLine, bottomJointLine, false, false, lineWidth, seriesWeight, color);
								polygons.AddRange(jointPolygons);
								topNormal = previousTopPolygonNormal;
								bottomNormal = previousBottomPolygonNormal;
								revertNormals = true;
							}
						}
						else if (topJointLine != null) {
							List<PlanePolygon> jointPolygons = SeriesView3DHelper.GetPolygons(coordsCalculator, 
								bottomJointPoint, topJointLine, true, false, lineWidth, seriesWeight, color);
							polygons.AddRange(jointPolygons);
						}
						else if (bottomJointLine != null) {
							List<PlanePolygon> jointPolygons = SeriesView3DHelper.GetPolygons(coordsCalculator, 
								topJointPoint, bottomJointLine, true, false, lineWidth, seriesWeight, color);
							polygons.AddRange(jointPolygons);
						}
						List<PlanePolygon> polygonsToAdd = GetPolygons(coordsCalculator, previousTopLine, previousBottomLine, 
							previousTopNormal, topNormal, previousBottomNormal, bottomNormal, leftVisible, false, lineWidth, seriesWeight, color);
						polygons.AddRange(polygonsToAdd);
						leftVisible = false;
						previousTopPolygonNormal = topPolygonNormal;
						previousBottomPolygonNormal = bottomPolygonNormal;
						previousTopNormal = topNormal;
						previousBottomNormal = bottomNormal;
						if (revertNormals) {
							previousTopNormal = topPolygonNormal;
							previousBottomNormal = bottomPolygonNormal;
						}
						breakNormals = false;
					}
					previousTopLine = topLine;
					previousBottomLine = bottomLine;
				}
				previousPoint = point;
			}
			layout = pointsLayout[pointsLayout.Count - 1];
			if (previousBottomLine != null && previousTopLine != null) {
				List<PlanePolygon> polygonsToAdd = GetPolygons(coordsCalculator, previousTopLine, previousBottomLine, previousTopNormal, 
					previousTopNormal, previousBottomNormal, previousBottomNormal, leftVisible, true, lineWidth, seriesWeight, color);
				polygons.AddRange(polygonsToAdd);
			}
			return polygons;
		}
		protected override ChartElement CreateObjectForClone() {
			return new Spline3DSeriesView();
		}
		protected override GeometryStripCreator CreateStripCreator() {
			XYDiagram3D diagram = (XYDiagram3D)Series.Chart.Diagram;
			if (diagram != null)
				return new SplineGeometryStripCreator(LineTension, diagram.AxisX.ScaleTypeMap.Transformation, diagram.AxisY.ScaleTypeMap.Transformation);
			return new SplineGeometryStripCreator(LineTension, null, null);
		}
		protected override List<PlanePolygon> CalculatePolygons(XYDiagram3DCoordsCalculator coordsCalculator, XYDiagram3DSeriesLayout seriesLayout, IList<IGeometryStrip> strips) {
			double zoomingFactor = coordsCalculator.Diagram.ZoomPercent / 100.0;
			double lineWidth = coordsCalculator.CalcSeriesWidth(this);
			int seriesWeight = coordsCalculator.GetSeriesWeight(Series);
			Color color = seriesLayout.SeriesData.DrawOptions.Color;
			int pointIndex = 0;
			List<PlanePolygon> result = new List<PlanePolygon>();
			foreach (LineStrip strip in strips) {
				LineStrip mappedStrip = StripsUtils.MapStrip(coordsCalculator, strip);
				List<PlanePolygon> polygons = CreateSplineSegmentPolygons(coordsCalculator,
					BezierUtils.CalcBezier(mappedStrip as BezierStrip, zoomingFactor), ExtractSeriesPointViewInfo(seriesLayout, ref pointIndex), 
					mappedStrip, lineWidth, seriesWeight, LineThickness, color);
				result.AddRange(polygons);
			}
			return result;
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			Line3DDrawOptions drawOptions = seriesDrawOptions as Line3DDrawOptions;
			if (drawOptions == null)
				return;
			LineStyle lineStyle = LegendLineStyle;
			renderer.EnableAntialiasing(lineStyle.AntiAlias);
			renderer.DrawBezier(SplineSeriesViewPainter.CreateStripForLegendMarker(bounds), drawOptions.Color, 1, lineStyle);
			renderer.RestoreAntialiasing();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ISplineSeriesView view = obj as ISplineSeriesView;
			if (view != null)
				lineTensionPercent = view.LineTensionPercent;
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return lineTensionPercent == ((Spline3DSeriesView)obj).lineTensionPercent;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
