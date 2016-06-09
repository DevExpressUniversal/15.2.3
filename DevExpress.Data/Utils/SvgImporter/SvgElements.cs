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

using DevExpress.Compatibility.System.Drawing;
using System;
using System.Drawing;
using System.Globalization;
namespace DevExpress.Data.Svg {
	public abstract class SvgItem {
		public abstract void FillData(SvgElementDataAgent dataAgent);
	}
	public class SvgShapeStyle : SvgItem {
		public Color Fill { get; set; }
		public Color Stroke { get; set; }
		public double StrokeWidth { get; set; }
		public override void FillData(SvgElementDataAgent dataAgent) {
			Fill = dataAgent.GetColor(SvgTokens.Fill);
			Stroke = dataAgent.GetColor(SvgTokens.Stroke);
			StrokeWidth = dataAgent.GetDouble(SvgTokens.StrokeWidth, 0.5, CultureInfo.InvariantCulture);
		}
	}
	public abstract class SvgElement : SvgItem {
		public SvgShapeStyle ElementStyle { get; set; }
		public void FillStyle(SvgElementDataAgent dataAgent) {
			ElementStyle = new SvgShapeStyle();
			ElementStyle.FillData(dataAgent);
		}
		public abstract SvgRect GetBoundaryPoints();
	}
	public static class SvgElementHelper {
		public static Func<SvgElement, T> GetMatcher<T>(Func<SvgLine, T> line, Func<SvgEllipse, T> ellipse, Func<SvgCircle, T> circle, Func<SvgRectangle, T> rectangle, 
			Func<SvgPath, T> path, Func<SvgPolyline, T> polyline, Func<SvgPolygon, T> polygon) where T : class {
			return elem => {
				if(elem is SvgLine)
					return line((SvgLine)elem);
				if(elem is SvgEllipse)
					return ellipse((SvgEllipse)elem);
				if(elem is SvgCircle)
					return circle((SvgCircle)elem);
				if(elem is SvgRectangle)
					return rectangle((SvgRectangle)elem);
				if(elem is SvgPath)
					return path((SvgPath)elem);
				if(elem is SvgPolyline)
					return polyline((SvgPolyline)elem);
				if(elem is SvgPolygon)
					return polygon((SvgPolygon)elem);
				return null;
			};
		}
		public static Func<SvgCommandBase, T> GetSvgCommandMatcher<T>(Func<SvgCommandMove, T> move, Func<SvgCommandBase, T> line, Func<SvgCommandArc, T> arc, Func<SvgCommandShortCubicBezier, T> shortCubicBezier,
			Func<SvgCommandCubicBezier, T> cubicBezier, Func<SvgCommandShortQuadraticBezier, T> shortQuadraticBezier, Func<SvgCommandQuadraticBezier, T> quadraticBezier) where T : class {
			return svgCommand => {
				if(svgCommand is SvgCommandMove)
					return move((SvgCommandMove)svgCommand);
				if(IsLineToCommand(svgCommand))
					return line(svgCommand);
				if(svgCommand is SvgCommandArc)
					return arc((SvgCommandArc)svgCommand);
				if(svgCommand is SvgCommandShortCubicBezier)
					return shortCubicBezier((SvgCommandShortCubicBezier)svgCommand);
				if(svgCommand is SvgCommandCubicBezier)
					return cubicBezier((SvgCommandCubicBezier)svgCommand);
				if(svgCommand is SvgCommandShortQuadraticBezier)
					return shortQuadraticBezier((SvgCommandShortQuadraticBezier)svgCommand);
				if(svgCommand is SvgCommandQuadraticBezier)
					return quadraticBezier((SvgCommandQuadraticBezier)svgCommand);
				return null;
			};
		}
		static bool IsLineToCommand(SvgCommandBase svgCommand) {
			return svgCommand is SvgCommandLine || svgCommand is SvgCommandHorizontal || svgCommand is SvgCommandVertical;
		}
	}
	[FormatElement(SvgTokens.Line)]
	public class SvgLine : SvgElement {
		public SvgPoint Point1 { get; set; }
		public SvgPoint Point2 { get; set; }
		public override void FillData(SvgElementDataAgent dataAgent) {
			Point1 = SvgPoint.Parse(dataAgent.GetString(SvgTokens.X1), dataAgent.GetString(SvgTokens.Y1), CultureInfo.InvariantCulture);
			Point2 = SvgPoint.Parse(dataAgent.GetString(SvgTokens.X2), dataAgent.GetString(SvgTokens.Y2), CultureInfo.InvariantCulture);
		}
		public override SvgRect GetBoundaryPoints() {
			return new SvgRect(Point1.X, Point1.Y, Point2.X, Point2.Y);
		}
	}
	public class SvgPointContainer : SvgElement {
		public SvgPointCollection PointCollection { get; set; }
		public override void FillData(SvgElementDataAgent dataAgent) {
			string pointString = dataAgent.GetString(SvgTokens.Points);
			PointCollection = SvgPointCollection.Parse(pointString);
		}
		public override SvgRect GetBoundaryPoints() {
			return PointCollection.GetBoundaryPoints();
		}
	}
	[FormatElement(SvgTokens.Polyline)]
	public class SvgPolyline : SvgPointContainer {
	}
	[FormatElement(SvgTokens.Polygon)]
	public class SvgPolygon : SvgPointContainer {
	}
	[FormatElement(SvgTokens.Path)]
	public class SvgPath : SvgElement {
		public SvgCommandCollection CommandCollection { get; set; }
		public override void FillData(SvgElementDataAgent dataAgent) {
			string commandsString = dataAgent.GetString(SvgTokens.Commands);
			CommandCollection = SvgCommandCollection.Parse(commandsString);
		}
		public override SvgRect GetBoundaryPoints() {
			return CommandCollection.GetBoundaryPoints();
		}
	}
	public abstract class SvgSupportRectangle : SvgElement {
		public double Width { get; set; }
		public double Height { get; set; }
		public SvgPoint Location { get; set; }
	}
	[FormatElement(SvgTokens.Rectangle)]
	public class SvgRectangle : SvgSupportRectangle {
		public override void FillData(SvgElementDataAgent dataAgent) {
			Location = SvgPoint.Parse(dataAgent.GetString(SvgTokens.X), dataAgent.GetString(SvgTokens.Y), CultureInfo.InvariantCulture);
			Width = dataAgent.GetDouble(SvgTokens.Width, CultureInfo.InvariantCulture);
			Height = dataAgent.GetDouble(SvgTokens.Height, CultureInfo.InvariantCulture);
		}
		public override SvgRect GetBoundaryPoints() {
			return new SvgRect(Location.X, Location.Y,Location.X + Width, Location.Y + Height);
		}
	}
	[FormatElement(SvgTokens.Ellipse)]
	public class SvgEllipse : SvgSupportRectangle {
		public override void FillData(SvgElementDataAgent dataAgent) {
			Location = SvgPoint.Parse(dataAgent.GetString(SvgTokens.Cx), dataAgent.GetString(SvgTokens.Cy), CultureInfo.InvariantCulture);
			Width = dataAgent.GetDouble(SvgTokens.Rx, CultureInfo.InvariantCulture);
			Height = dataAgent.GetDouble(SvgTokens.Ry, CultureInfo.InvariantCulture);
		}
		public override SvgRect GetBoundaryPoints() {
			return new SvgRect(Location.X - Width, Location.Y - Height, Location.X + Width, Location.Y + Height);
		}
	}
	[FormatElement(SvgTokens.Circle)]
	public class SvgCircle : SvgElement {
		public double Radius { get; set; }
		public SvgPoint Location { get; set; }
		public override void FillData(SvgElementDataAgent dataAgent) {
			Location = SvgPoint.Parse(dataAgent.GetString(SvgTokens.Cx), dataAgent.GetString(SvgTokens.Cy), CultureInfo.InvariantCulture);
			Radius = dataAgent.GetDouble(SvgTokens.R, CultureInfo.InvariantCulture);
		}
		public override SvgRect GetBoundaryPoints() {
			return new SvgRect(Location.X - Radius, Location.Y - Radius, Location.X + Radius, Location.Y + Radius);
		}
	}
}
