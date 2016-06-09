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

using DevExpress.Data.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Diagram.Core.Native {
	internal static class DiagramSvgParser {
		public static Tuple<ShapeGeometry, Size> Parse(Stream svgStream) {
			SvgImporter importer = new SvgImporter();
			importer.Import(svgStream);
			var bounds = importer.Bounds.ToRect();
			var geometry = ParseSvgItems(importer.SvgItems);
			return new Tuple<ShapeGeometry, Size>(geometry, bounds.Size);
		}
		internal static ShapeGeometry ParseSvgItems(IList<SvgItem> svgItems) {
			List<ShapeSegment> segments = new List<ShapeSegment>();
			var matcher = GetMatcher();
			foreach(SvgElement svgItem in svgItems) {
				var diagramItems = matcher(svgItem);
				diagramItems = ApplyStyle(diagramItems, svgItem.ElementStyle);
				segments.AddRange(diagramItems);
			}
			return new ShapeGeometry(segments.ToArray());
		}
		static Func<SvgElement, IEnumerable<ShapeSegment>> GetMatcher() {
			return SvgElementHelper.GetMatcher(
				line => CreateLine(line),
				ellipse => CreateEllipse(ellipse),
				circle => CreateEllipse(circle),
				rectangle => CreateRectangle(rectangle),
				path => CreatePath(path),
				polyline => CreatePolyline(polyline),
				polygon => CreatePolygon(polygon)
			);
		}
		static IEnumerable<ShapeSegment> ApplyStyle(IEnumerable<ShapeSegment> segments, SvgShapeStyle style) {
			return segments.Select(segment => {
				var startSegment = segment as StartSegment;
				if(startSegment != null && style != null) {
					var segmentStyle = new StartSegmentStyle(startSegment.Style.FillBrightness, style.Fill.ToColor(), style.Stroke.ToColor(), style.StrokeWidth);
					return new StartSegment(startSegment.Point, startSegment.Kind, startSegment.IsSmoothJoin, startSegment.IsNewShape, segmentStyle);
				}
				return segment;
			});
		}
		#region Svg Items Factory Methods
		static IEnumerable<ShapeSegment> CreateEllipse(SvgCircle svgCircle) {
			return CreateEllipse(svgCircle.Location.ToPoint(), new Size(svgCircle.Radius, svgCircle.Radius));
		}
		static IEnumerable<ShapeSegment> CreateEllipse(SvgEllipse svgEllipse) {
			return CreateEllipse(svgEllipse.Location.ToPoint(), new Size(svgEllipse.Width, svgEllipse.Height));
		}
		static IEnumerable<ShapeSegment> CreateEllipse(Point center, Size radius) {
			Point location = center.OffsetPoint(new Point(radius.Width, radius.Height).InvertPoint());
			return BasicShapes.GetEllipsePoints(radius.Width * 2, radius.Height * 2).Offset(location).Segments;
		}
		static IEnumerable<ShapeSegment> CreateRectangle(SvgRectangle svgRectangle) {
			return BasicShapes.GetRectanglePoints(svgRectangle.Location.X, svgRectangle.Location.Y, svgRectangle.Width, svgRectangle.Height).Segments;
		}
		static IEnumerable<ShapeSegment> CreatePolygon(SvgPolygon svgPolygon) {
			return CreatePolyline(svgPolygon.PointCollection, GeometryKind.ClosedFilled);
		}
		static IEnumerable<ShapeSegment> CreatePolyline(SvgPolyline svgPolyline) {
			return CreatePolyline(svgPolyline.PointCollection, GeometryKind.None);
		}
		static IEnumerable<ShapeSegment> CreatePolyline(SvgPointCollection svgPoints, GeometryKind kind) {
			if(svgPoints.Count < 1)
				yield break;
			yield return StartSegment.Create(svgPoints.Points.First().ToPoint(), kind: kind);
			foreach(var point in svgPoints.Points.Skip(1).Select(x => x.ToPoint())) {
				yield return LineSegment.Create(point);
			}
		}
		static IEnumerable<ShapeSegment> CreateLine(SvgLine svgLine) {
			yield return StartSegment.Create(svgLine.Point1.ToPoint());
			yield return LineSegment.Create(svgLine.Point2.ToPoint());
		}
		#endregion
		#region Svg Path
		static IEnumerable<ShapeSegment> CreatePath(SvgPath svgPath) {
			var openSegments = GetStartCommands(svgPath.CommandCollection.Commands);
			ShapeSegment prevSegment = null;
			var filteredCommands = svgPath.CommandCollection.Commands.Where(x => x.CommandAction != SvgCommandAction.Stop);
			Func<SvgCommandBase, bool> getIsClosed = command => !openSegments.Contains(command);
			var commandMatcher = GetCommandMatcher(() => prevSegment, getIsClosed);
			foreach(var svgCommand in filteredCommands) {
				prevSegment = commandMatcher(svgCommand);
				yield return prevSegment;
			}
		}
		static List<SvgCommandBase> GetStartCommands(IEnumerable<SvgCommandBase> commands) {
			List<SvgCommandBase> startCommandsList = new List<SvgCommandBase>();
			Stack<SvgCommandBase> startCommandsStack = new Stack<SvgCommandBase>();
			foreach(var command in commands) {
				if(command.CommandAction == SvgCommandAction.Start) {
					startCommandsStack.Push(command);
				}
				else if(command.CommandAction == SvgCommandAction.Stop) {
					if(startCommandsStack.Count > 0)
						startCommandsStack.Pop();
					startCommandsList.AddRange(startCommandsStack);
					startCommandsStack.Clear();
				}
			}
			startCommandsList.AddRange(startCommandsStack);
			return startCommandsList;
		}
		static Func<SvgCommandBase, ShapeSegment> GetCommandMatcher(Func<ShapeSegment> getPrevsegment, Func<SvgCommandBase, bool> getIsClosed) {
			return SvgElementHelper.GetSvgCommandMatcher<ShapeSegment>(
				move => CreateStartSegment(move, getIsClosed(move)),
				line => CreateLineSegment(line),
				arc => CreateArcSegment(arc),
				shortCubicBezier => CreateShortCubicBezierSegment(getPrevsegment(), shortCubicBezier),
				cubicBezier => CreateCubicBezierSegment(cubicBezier),
				shortQuadraticBezier => CreateShortQuadraticBezierSegment(getPrevsegment(), shortQuadraticBezier),
				quadraticBezier => CreateQuadraticBezierSegment(quadraticBezier)
			);
		}
		static StartSegment CreateStartSegment(SvgCommandBase command, bool isClosed) {
			return StartSegment.Create(command.GeneralPoint.ToPoint(), kind: isClosed ? GeometryKind.ClosedFilled : GeometryKind.Filled);
		}
		static LineSegment CreateLineSegment(SvgCommandBase command) {
			return LineSegment.Create(command.GeneralPoint.ToPoint());
		}
		static ArcSegment CreateArcSegment(SvgCommandArc command) {
			var sweepDirection = command.IsSwap ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
			var size = new Size(command.Radius.X, command.Radius.Y);
			return new ArcSegment(command.GeneralPoint.ToPoint(), sweepDirection, size);
		}
		static BezierSegment CreateCubicBezierSegment(SvgCommandBase command) {
			return new BezierSegment(command.Points[0].ToPoint(), command.Points[1].ToPoint(), command.Points[2].ToPoint());
		}
		static BezierSegment CreateShortCubicBezierSegment(ShapeSegment prevSegment, SvgCommandBase command) {
			Point point1 = prevSegment.Point;
			var bezierSegment = prevSegment as BezierSegment;
			if(bezierSegment != null)
				point1 = bezierSegment.Point + (bezierSegment.Point - bezierSegment.Point2);
			return new BezierSegment(point1, command.Points[0].ToPoint(), command.Points[1].ToPoint());
		}
		static QuadraticBezierSegment CreateQuadraticBezierSegment(SvgCommandBase command) {
			return new QuadraticBezierSegment(command.Points[0].ToPoint(), command.Points[1].ToPoint());
		}
		static QuadraticBezierSegment CreateShortQuadraticBezierSegment(ShapeSegment prevSegment, SvgCommandBase command) {
			Point point1 = prevSegment.Point;
			var quadraticBezierSegment = prevSegment as QuadraticBezierSegment;
			if(quadraticBezierSegment != null)
				point1 = quadraticBezierSegment.Point + (quadraticBezierSegment.Point - quadraticBezierSegment.Point1);
			return new QuadraticBezierSegment(point1, command.Points[0].ToPoint());
		}
		#endregion
	}
	static class SvgItemExtensions {
		public static Point ToPoint(this SvgPoint point) {
			return new Point(point.X, point.Y);
		}
		public static Rect ToRect(this SvgRect rect) {
			return new Rect(rect.X1, rect.Y1, rect.Width, rect.Height);
		}
		public static Color ToColor(this System.Drawing.Color color) {
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}
	}
}
