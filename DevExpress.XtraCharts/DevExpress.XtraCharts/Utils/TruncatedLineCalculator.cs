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
	public enum LineKind {
		Line,
		Segment,
		Ray
	}
	public class TruncatedLineCalculator {
		static GRealLine2D? Truncate(GRealLine2D line, Rectangle bounds, LineKind lineKind) {
			double dx = line.End.X - line.Start.X;
			double dy = line.End.Y - line.Start.Y;
			double param1;
			double param2;
			GRealPoint2D point1;
			GRealPoint2D point2;
			if (dx == 0.0 && dy == 0.0)
				return null;
			if (Math.Abs(dx) > Math.Abs(dy)) {
				param1 = (bounds.Left - line.Start.X) / dx;
				param2 = (bounds.Right - line.Start.X) / dx;
				double leftY = line.Start.Y + dy * param1;
				double rightY = line.Start.Y + dy * param2;
				if ((leftY < bounds.Top && rightY < bounds.Top) || (leftY > bounds.Bottom && rightY > bounds.Bottom))
					return null;
				point1 = new GRealPoint2D(bounds.Left, leftY);
				point2 = new GRealPoint2D(bounds.Right, rightY);
			}
			else {
				param1 = (bounds.Top - line.Start.Y) / dy;
				param2 = (bounds.Bottom - line.Start.Y) / dy;
				double topX = line.Start.X + dx * param1;
				double bottomX = line.Start.X + dx * param2;
				if ((topX < bounds.Left && bottomX < bounds.Left) || (topX > bounds.Right && bottomX > bounds.Right))
					return null;
				point1 = new GRealPoint2D(topX, bounds.Top);
				point2 = new GRealPoint2D(bottomX, bounds.Bottom);
			}
			switch (lineKind) {
				case LineKind.Line:
					return new GRealLine2D(point1, point2);
				case LineKind.Ray:
					if (param1 < 0 && param2 < 0) 
						return null;
					 return new GRealLine2D(param1 >= 0 ? point1 : line.Start, param2 >= 0 ? point2 : line.Start);
				case LineKind.Segment:
					if ((param1 < 0 && param2 < 0) || (param1 > 1 && param2 > 1)) 
						return null;
					GRealPoint2D start;
					GRealPoint2D end;
					if (param1 < 0)
						start = line.Start;
					else if (param1 <= 1)
						start = point1;
					else
						start = line.End;
					if (param2 < 0)
						end = line.Start;
					else if (param2 <= 1)
						end = point2;
					else
						end = line.End;
					return new GRealLine2D(start, end);
				default:
					ChartDebug.Fail("Unknown line kind.");
					return null;
			}
		}
		public static GRealLine2D? Truncate(DiagramPoint start, DiagramPoint end, Rectangle bounds, LineKind lineKind) {
			return Truncate(new GRealLine2D(new GRealPoint2D(start.X, start.Y), new GRealPoint2D(end.X, end.Y)), bounds, lineKind);
		}
		public static GRealLine2D? Truncate(GRealPoint2D start, GRealPoint2D end, Rectangle bounds, LineKind lineKind) {
			return Truncate(new GRealLine2D(start, end), bounds, lineKind);
		}
		public static List<LineStrip> SplitLineStrip(LineStrip strip, Rectangle bounds) {
			List<LineStrip> strips = new List<LineStrip>();
			LineStrip currentStrip = new LineStrip();
			bounds.Width++;
			bounds.Height++;
			GRealPoint2D start;
			GRealPoint2D end = strip[0];
			int count = strip.Count;
			for (int i = 1; i < count; i++) {
				start = end;
				end = strip[i];
				Point roundedStart = new Point((int)Math.Round(start.X), (int)Math.Round(start.Y));
				Point roundedEnd = new Point((int)Math.Round(end.X), (int)Math.Round(end.Y));
				if (bounds.Contains(roundedStart) && bounds.Contains(roundedEnd)) {
					currentStrip.AddUniquePoint(start);
					currentStrip.AddUniquePoint(end);
				}
				else {
					GRealLine2D? truncation = Truncate(new GRealLine2D(start, end), bounds, LineKind.Segment);
					if (truncation.HasValue) {
						GRealLine2D truncated = truncation.Value;
						if (bounds.Contains(roundedStart)) {
							currentStrip.AddUniquePoint(start);
							currentStrip.AddUniquePoint(start == truncated.Start ? truncated.End : truncated.Start);
						}
						else if (bounds.Contains(roundedEnd)) {
							currentStrip.AddUniquePoint(end == truncated.Start ? truncated.End : truncated.Start);
							currentStrip.AddUniquePoint(end);
						}
						else {
							double dx1 = truncated.Start.X - start.X;
							double dy1 = truncated.Start.Y - start.Y;
							double dx2 = truncated.End.X - start.X;
							double dy2 = truncated.End.Y - start.Y;
							if (dx1 * dx1 + dy1 * dy1 < dx2 * dx2 + dy2 * dy2) {
								currentStrip.AddUniquePoint(truncated.Start);
								currentStrip.AddUniquePoint(truncated.End);
							}
							else {
								currentStrip.AddUniquePoint(truncated.End);
								currentStrip.AddUniquePoint(truncated.Start);
							}						
						}
					}
					else {
						if (currentStrip.Count > 1)
							strips.Add(currentStrip);
						currentStrip = new LineStrip();
					}
				}
			}
			if (currentStrip.Count > 1)
				strips.Add(currentStrip);
			return strips;
		}
	}  
}
