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
namespace DevExpress.Utils.Svg {
	static class SVGPathParser {
		public static void Parse(string data, SvgPathSegmentCollection segments) {
			if(string.IsNullOrEmpty(data)) return;
			try {
				foreach(var commandSet in GetCommands(data.TrimEnd(null))) {
					CreatePathSegment(commandSet, segments);
				}
			}
			catch { }
		}
		static void CreatePathSegment(string commandSet, SvgPathSegmentCollection segments) {
			char command = char.ToLower(commandSet[0]);
			bool isRelative = char.IsLower(commandSet[0]);
			PointF[] points;
			float[] numbers;
			switch(command) {
				case 'm':
					points = CoordinateParser.GetPoints(commandSet);
					for(int i = 0; i < points.Length; i++) {
						if(i == 0)
							segments.Add(new SvgPathMoveToSegment(ToAbsolute(points[i], PointF.Empty, isRelative)));
						else
							segments.Add(new SvgPathLineToSegment(segments.LastPoint, ToAbsolute(points[i], segments.LastPoint, isRelative)));
					}
					break;
				case 'a':
					break;
				case 'l':
					points = CoordinateParser.GetPoints(commandSet);
					for(int i = 0; i < points.Length; i++) {
						segments.Add(new SvgPathLineToSegment(segments.LastPoint, ToAbsolute(points[i], segments.LastPoint, isRelative)));
					}
					break;
				case 'h':
					numbers = CoordinateParser.GetNumbers(commandSet);
					foreach(var item in numbers) {
						segments.Add(new SvgPathLineToSegment(segments.LastPoint, ToAbsolute(item, segments.LastPoint.Y, segments.LastPoint, isRelative, false)));
					}
					break;
				case 'v':
					numbers = CoordinateParser.GetNumbers(commandSet);
					foreach(var item in numbers) {
						segments.Add(new SvgPathLineToSegment(segments.LastPoint, ToAbsolute(segments.LastPoint.X, item, segments.LastPoint, false, isRelative)));
					}
					break;
				case 'q':
					points = CoordinateParser.GetPoints(commandSet);
					for(int i = 0; i < points.Length; i += 2) {
						segments.Add(new SvgPathCurveToQuadraticSegment(segments.LastPoint,
							ToAbsolute(points[i], segments.LastPoint, isRelative),
							ToAbsolute(points[i + 1], segments.LastPoint, isRelative)));
					}
					break;
				case 't':
					points = CoordinateParser.GetPoints(commandSet);
					for(int i = 0; i < points.Length; i++) {
						PointF pt1 = segments.LastPoint;
						var quadraticSegment = segments.Last as SvgPathCurveToQuadraticSegment;
						if(quadraticSegment != null) {
							pt1 = ConvertPoint(quadraticSegment.AdditionalPoint, segments.LastPoint);
						}
						segments.Add(new SvgPathCurveToQuadraticSegment(segments.LastPoint, pt1, ToAbsolute(points[i], segments.LastPoint, isRelative)));
					}
					break;
				case 'c':
					points = CoordinateParser.GetPoints(commandSet);
					for(int i = 0; i < points.Length; i += 3) {
						segments.Add(new SvgPathCurveToCubicSegment(segments.LastPoint, ToAbsolute(points[i], segments.LastPoint, isRelative),
							ToAbsolute(points[i + 1], segments.LastPoint, isRelative), ToAbsolute(points[i + 2], segments.LastPoint, isRelative)));
					}
					break;
				case 's':
					points = CoordinateParser.GetPoints(commandSet);
					var curveToCubicSegment = segments.Last as SvgPathCurveToCubicSegment;
					PointF controlPoint = curveToCubicSegment != null ? ConvertPoint(curveToCubicSegment.SecondAdditionalPoint, segments.LastPoint) : segments.LastPoint;
					for(int i = 0; i < points.Length; i += 2) {
						PointF pt2 = ToAbsolute(points[i], segments.LastPoint, isRelative);
						PointF pt3 = ToAbsolute(points[i + 1], segments.LastPoint, isRelative);
						segments.Add(new SvgPathCurveToCubicSegment(segments.LastPoint, controlPoint, pt2, pt3));
					}
					break;
				case 'z':
					segments.Add(new SvgPathCloseSegment());
					break;
			}
		}
		static PointF ConvertPoint(PointF point, PointF mirrow) {
			float x = point.X + (mirrow.X - point.X) * 2f;
			float y = point.Y + (mirrow.Y - point.Y) * 2f;
			return new PointF(x, y);
		}
		static PointF ToAbsolute(PointF point, PointF current, bool isRelativeBoth) {
			return ToAbsolute(point.X, point.Y, current, isRelativeBoth, isRelativeBoth);
		}
		static PointF ToAbsolute(float x, float y, PointF current, bool isRelativeX, bool isRelativeY) {
			var point = new PointF(x, y);
			if(isRelativeX)
				point.X += current.X;
			if(isRelativeY)
				point.Y += current.Y;
			return point;
		}
		static IEnumerable<string> GetCommands(string path) {
			int start = 0;
			for(var i = 0; i < path.Length; i++) {
				string command;
				if(char.IsLetter(path[i]) && path[i] != 'e') {
					command = path.Substring(start, i - start).Trim();
					start = i;
					if(!string.IsNullOrEmpty(command))
						yield return command;
					if(path.Length == i + 1)
						yield return path[i].ToString();
				}
				if(path.Length == i + 1) {
					command = path.Substring(start, i - start + 1).Trim();
					if(!string.IsNullOrEmpty(command))
						yield return command;
				}
			}
		}
	}
}
