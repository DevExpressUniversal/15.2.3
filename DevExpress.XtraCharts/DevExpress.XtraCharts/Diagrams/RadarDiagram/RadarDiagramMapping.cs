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
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class RadarDiagramMapping : IXYDiagramMapping, IEllipse {
		public struct Vertex {
			double argument;
			double x, y;
			public double Argument { get { return argument; } }
			public double X { get { return x; } }
			public double Y { get { return y; } }
			public Vertex(double argument, double x, double y) {
				this.argument = argument;
				this.x = x;
				this.y = y;
			}
		}
		const double PI = Math.PI;
		readonly Rectangle bounds;
		readonly bool clipArgument;
		readonly GRealPoint2D center;
		readonly double radius;
		readonly double startAngle;
		readonly bool revertAngle;
		readonly double minArgument;
		readonly double maxArgument;
		readonly double argumentDiapason;
		readonly double argumentDelta;
		readonly Vertex[] vertices;
		readonly double minValue;
		readonly double maxValue;
		readonly double valueScaleFactor;
		readonly RadarDiagramDrawingStyle drawingStyle;
		readonly double diagramStartAngle;
		readonly bool isQualitativeScale;
		readonly Transformation valueTransformation;
		readonly Transformation argumentTransformation;
		public Rectangle MappingBounds { get { return bounds; } }
		public bool ClipArgument { get { return clipArgument; } }
		public GRealPoint2D Center { get { return center; } }
		public double Radius { get { return radius; } }
		public double StartAngle { get { return startAngle; } }
		public bool RevertAngle { get { return revertAngle; } }
		public double MinArgument { get { return minArgument; } }
		public double MaxArgument { get { return maxArgument; } }
		public double MinValue { get { return minValue; } }
		public double MaxValue { get { return maxValue; } }
		public bool IsValid { get { return argumentDiapason > 0.0; } }
		public int VerticesCount { get { return vertices == null ? 0 : vertices.Length - 1; } }
		public double DiagramStartAngle { get { return diagramStartAngle; } }
		public double ValueScaleFactor { get { return valueScaleFactor; } }
		public double ArgumentDiapason { get { return argumentDiapason; } }
		public double ArgumentDelta { get { return argumentDelta; } }
		public Vertex[] Vertices { get { return vertices; } }
		public RadarDiagramMapping(RadarDiagram diagram, ZPlaneRectangle bounds, IList<double> values) {
			this.bounds = (Rectangle)bounds;
			drawingStyle = diagram.DrawingStyle;
			clipArgument = diagram.ClipArgument;
			diagramStartAngle = diagram.StartAngleInDegrees * PI / 180;
			startAngle = diagram.ActualStartAngle - PI / 2.0;
			revertAngle = diagram.RotationDirection == RadarDiagramRotationDirection.Counterclockwise;
			argumentTransformation = diagram.AxisX.ScaleTypeMap.Transformation;
			if (values.Count > 1) {
				int lastValueIndex = values.Count - 1;
				maxArgument = values[lastValueIndex];
				minArgument = values[0];
				argumentDiapason = maxArgument - minArgument;
				argumentDelta = argumentDiapason / lastValueIndex;
				if (diagram.DrawingStyle == RadarDiagramDrawingStyle.Polygon) {
					vertices = new Vertex[values.Count];
					for (int i = 0; i <= lastValueIndex; i++) {
						double argument = values[i];
						double angle = CalcAngle(argument);
						vertices[i] = new Vertex(argument, Math.Cos(angle), Math.Sin(angle));
					}
				}
			}
			else {
				minArgument = 0.0;
				maxArgument = 0.0;
				argumentDiapason = 0.0;
			}
			valueTransformation = diagram.AxisY.ScaleTypeMap.Transformation;
			minValue = diagram.AxisY.VisualRangeData.Min;
			maxValue = diagram.AxisY.VisualRangeData.Max;
			center = GraphicUtils.CalcCenter(bounds, true);
			radius = GraphicUtils.CalcRadius(Math.Min(bounds.Width, bounds.Height), true);
			valueScaleFactor = radius / (valueTransformation.TransformForward(maxValue) - valueTransformation.TransformForward(minValue));
			isQualitativeScale = diagram.AxisX.ScaleType == ActualScaleType.Qualitative;
		}
		double GetNormalDistance(double normalIndent) {
			return (double)(revertAngle ? -normalIndent : normalIndent);
		}
		double GetNormalAngle(double angle) {
			return angle + PI / 2;
		}
		double GetValue(double distance) {
			return distance / valueScaleFactor + valueTransformation.TransformForward(minValue);
		}
		double CorrectAngle(double angle) {
			if (revertAngle && angle != 0)
				angle = 2 * PI - angle;
			if (diagramStartAngle >= 0) {
				if (angle >= diagramStartAngle)
					angle -= diagramStartAngle;
				else
					angle = 2 * PI - diagramStartAngle + angle;
			}
			else {
				if (angle < 2 * PI + diagramStartAngle)
					angle -= diagramStartAngle;
				else
					angle = angle - 2 * PI - diagramStartAngle;
			}
			return angle;
		}
		bool CalcArgumentAndValueForPolygon(double angle, double dx, double dy, out double argumentInternal, out double valueInternal) {
			argumentInternal = 0;
			valueInternal = 0;
			if (vertices.Length <= 1)
				return false;
			double k = angle / (2 * PI / (vertices.Length - 1));
			int index = (int)Math.Floor(k);
			if (index < 0 || index >= vertices.Length - 1)
				return false;
			Vertex min = vertices[index];
			Vertex max = vertices[index + 1];
			double divisor1 = (min.X - max.X) * dy - (min.Y - max.Y) * dx;
			if (divisor1 == 0)
				return false;
			double maxFactor = (min.X * dy - min.Y * dx) / divisor1;
			double dist;
			double divisor2 = (min.X - max.X) * maxFactor - min.X;
			double divisor3 = (min.Y - max.Y) * maxFactor - min.Y;
			if (dx != 0 && divisor2 != 0)
				dist = dx / divisor2;
			else if (dy != 0 && divisor3 != 0)
				dist = dy / divisor3;
			else
				return false;
			if (dist > radius)
				return false;
			valueInternal = GetValue(dist);
			argumentInternal = (max.Argument - min.Argument) * maxFactor + min.Argument;
			return true;
		}
		public GRealPoint2D CalcEllipsePoint(double angle) {
			return new GRealPoint2D(Math.Cos(angle) * radius + center.X, Math.Sin(angle) * radius + center.Y);
		}
		public DiagramPoint GetScreenPoint(double argument, double value, bool clipValue, int indent, int normalIndent) {
			if (value == double.PositiveInfinity || value == double.MaxValue)
				value = maxValue;
			if ((clipArgument && (argument < minArgument || argument > maxArgument)) ||
				(clipValue && (value < minValue || value > maxValue)))
					return DiagramPoint.Zero;
			while (argument < minArgument)
				argument += argumentDiapason;
			while (argument > maxArgument)
				argument -= argumentDiapason;
			if (value < minValue)
				value = minValue;
			if (drawingStyle == RadarDiagramDrawingStyle.Circle) {
				double distance = GetDistance(value, indent);
				double angle = CalcAngle(argument);
				double normalDistance = GetNormalDistance(normalIndent);
				double normalAngle = GetNormalAngle(angle);
				return new DiagramPoint(
					Math.Cos(angle) * distance + Math.Cos(normalAngle) * normalDistance + center.X, 
					Math.Sin(angle) * distance + Math.Sin(normalAngle) * normalDistance + center.Y);
			}
			else {
				double distance = GetDistance(value, 0);
				double coord = (argument - minArgument) / argumentDelta;
				int index = (int)Math.Floor(coord);
				if (index < 0)
					index = 0;
				else if (index >= vertices.Length - 1)
					index = vertices.Length - 2;
				Vertex min = vertices[index];
				Vertex max = vertices[index + 1];
				double maxFactor = (argument - min.Argument) / (max.Argument - min.Argument);
				double minFactor = 1.0 - maxFactor;
				DiagramPoint result = new DiagramPoint((min.X * minFactor + max.X * maxFactor) * distance + center.X,
													   (min.Y * minFactor + max.Y * maxFactor) * distance + center.Y);
				if (indent != 0 || normalIndent != 0) {
					double angle = CalcAngle(argument);
					double normalAngle = GetNormalAngle(angle);
					double normalDistance = GetNormalDistance(normalIndent);
					result.X += Math.Cos(angle) * indent + Math.Cos(normalAngle) * normalDistance;
					result.Y += Math.Sin(angle) * indent + Math.Sin(normalAngle) * normalDistance;
				}
				return result;
			}
		}
		public DiagramPoint GetScreenPoint(double argument, double value, bool clipValue, int indent) {
			return GetScreenPoint(argument, value, clipValue, indent, 0);
		}
		public DiagramPoint GetScreenPoint(double argument, double value, bool clipValue) {
			return GetScreenPoint(argument, value, clipValue, 0);
		}
		public DiagramPoint GetScreenPoint(double argument, double value) {
			return GetScreenPoint(argument, value, false);
		}
		public double GetDistance(double value, int indent) {
			return (valueTransformation.TransformForward(value) - valueTransformation.TransformForward(minValue)) * valueScaleFactor + indent;
		}		
		public double GetDistance(double value) {
			return GetDistance(value, 0);
		}
		public double CalcAngle(double argument) {
			double angle = (argumentTransformation.TransformForward(argument) - argumentTransformation.TransformForward(minArgument)) / argumentTransformation.TransformForward(argumentDiapason) * PI * 2.0;
			if (revertAngle)
				angle = -angle;
			return angle + startAngle;
		}	 
		public bool CalcArgumentAndValue(Point p, out double argumentInternal, out double valueInternal) {
			argumentInternal = 0;
			valueInternal = 0;
			double dy = center.Y - p.Y;
			double dx = center.X - p.X;
			double distance = Math.Sqrt(dy * dy + dx * dx);
			if (radius == 0 || distance > radius)
				return false;
			if (distance == 0) {
				argumentInternal = minArgument;
				valueInternal = minValue;
				return true;
			}
			double angleCos = dy / distance;
			if (angleCos > 1 || angleCos < -1)
				return false;
			double angle = Math.Acos(angleCos);
			if (p.X < center.X)
				angle = PI * 2 - angle;
			angle = CorrectAngle(angle);
			if (drawingStyle == RadarDiagramDrawingStyle.Circle) {
				valueInternal = GetValue(distance);
				argumentInternal = angle / PI / 2.0 * argumentTransformation.TransformForward(argumentDiapason) + argumentTransformation.TransformForward(minArgument);
			}
			else if (!CalcArgumentAndValueForPolygon(angle, dx, dy, out argumentInternal, out valueInternal))
				return false;
			if (isQualitativeScale && argumentInternal > maxArgument - argumentDelta / 2)
				argumentInternal -= argumentDiapason;
			return true;
		}
	}
	public class RadarAxisXMapping {
		readonly RadarDiagramMapping diagramMapping;
		public RadarAxisXMapping(RadarDiagramMapping diagramMapping) {
			this.diagramMapping = diagramMapping;
		}
		public DiagramPoint GetNearScreenPoint(double axisValue) {
			return diagramMapping.GetScreenPoint(axisValue, Double.MinValue);
		}
		public DiagramPoint GetFarScreenPoint(double axisValue) {
			return diagramMapping.GetScreenPoint(axisValue, Double.MaxValue);
		}
	}
	public class RadarAxisYMapping {
		readonly RadarDiagramMapping diagramMapping;
		public RadarDiagramMapping DiagramMapping { get { return diagramMapping; } }
		public RadarAxisYMapping(RadarDiagramMapping diagramMapping) {
			this.diagramMapping = diagramMapping;
		}
		public DiagramPoint GetDiagramPoint(double axisValue, int selfOffset, int normalOffset) {
			return diagramMapping.GetScreenPoint(diagramMapping.MinArgument, axisValue, false, selfOffset, normalOffset);
		}
	}
}
