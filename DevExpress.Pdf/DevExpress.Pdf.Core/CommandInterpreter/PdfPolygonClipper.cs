#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfPolygonClipper {
		abstract class PdfValidatableEnumerator : IEnumerable<PdfPoint> {
			bool isValid = true;
			public bool IsValid {
				get { return isValid; }
				protected set { isValid = value; }
			}
			public abstract IEnumerator<PdfPoint> GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() {
				return ((IEnumerable<PdfPoint>)this).GetEnumerator();
			}
		}
		class PdfPointsEnumerator : PdfValidatableEnumerator {
			IEnumerable<PdfPoint> points;
			public PdfPointsEnumerator(IEnumerable<PdfPoint> points) {
				this.points = points;
			}
			public override IEnumerator<PdfPoint> GetEnumerator() {
				return points.GetEnumerator();
			}
		}
		class PdfPolygonEnumerator : PdfValidatableEnumerator {
			PdfGraphicsPath path;
			public PdfPolygonEnumerator(PdfGraphicsPath path) {
				this.path = path;
			}
			public override IEnumerator<PdfPoint> GetEnumerator() {
				yield return path.StartPoint;
				foreach (PdfGraphicsPathSegment segment in path.Segments) {
					if (!(segment is PdfLineGraphicsPathSegment))
						IsValid = false;
					yield return segment.EndPoint;
				}
			}
		}
		readonly Dictionary<PdfPoint, int> bitCodes;
		readonly double boundsLeft;
		readonly double boundsBottom;
		readonly double boundsRight;
		readonly double boundsTop;
		public PdfPolygonClipper(PdfRectangle bounds) {
			this.boundsLeft = bounds.Left;
			this.boundsBottom = bounds.Bottom;
			this.boundsRight = bounds.Right;
			this.boundsTop = bounds.Top;
			bitCodes = new Dictionary<PdfPoint, int>();
		}
		internal int GetBitCode(PdfPoint point) {
			int bitCode = 0;
			if (!bitCodes.TryGetValue(point, out bitCode)) {
				if (point.X < boundsLeft)
					bitCode |= 1;
				else if (point.X > boundsRight)
					bitCode |= 2;
				if (point.Y < boundsBottom)
					bitCode |= 4;
				else if (point.Y > boundsTop)
					bitCode |= 8;
				bitCodes[point] = bitCode;
			}
			return bitCode;
		}
		PdfPoint GetEdgeIntersection(PdfPoint firstPoint, PdfPoint secondPoint, int edge) {
			double ratio = (secondPoint.Y - firstPoint.Y) / (secondPoint.X - firstPoint.X);
			switch (edge) {
				case 1:
					return new PdfPoint(boundsLeft, firstPoint.Y + ratio * (boundsLeft - firstPoint.X));
				case 2:
					return new PdfPoint(boundsRight, firstPoint.Y + ratio * (boundsRight - firstPoint.X));
				case 4:
					return new PdfPoint(firstPoint.X + (boundsBottom - firstPoint.Y) / ratio, boundsBottom);
				default:
					return new PdfPoint(firstPoint.X + (boundsTop - firstPoint.Y) / ratio, boundsTop);
			}
		}
		public void Clip(PdfGraphicsPath path) {
			PdfValidatableEnumerator polygonEnumerator = new PdfPolygonEnumerator(path);
			using (IEnumerator<PdfPoint> enumerator = Clip(polygonEnumerator).GetEnumerator()) {
				if (!polygonEnumerator.IsValid)
					return;
				if (!enumerator.MoveNext())
					return;
				path.StartPoint = enumerator.Current;
				path.Segments.Clear();
				while (enumerator.MoveNext())
					path.Segments.Add(new PdfLineGraphicsPathSegment(enumerator.Current));
			}
		}
		IEnumerable<PdfPoint> Clip(PdfValidatableEnumerator points) {
			for (int edge = 1; edge <= 8; edge *= 2) {
				PdfPolygon clippedPoints = new PdfPolygon();
				using (IEnumerator<PdfPoint> enumerator = points.GetEnumerator()) {
					if (!enumerator.MoveNext())
						return points;
					PdfPoint previousPoint = enumerator.Current;
					PdfPoint firstPoint = previousPoint;
					while (enumerator.MoveNext()) {
						if (!points.IsValid)
							return points;
						PdfPoint currentPoint = enumerator.Current;
						ClipEdge(clippedPoints, previousPoint, currentPoint, edge);
						previousPoint = currentPoint;
					}
					ClipEdge(clippedPoints, previousPoint, firstPoint, edge);
				}
				points = new PdfPointsEnumerator(clippedPoints.Points);
			}
			return points;
		}
		void ClipEdge(PdfPolygon clippedPoints, PdfPoint previousPoint, PdfPoint currentPoint, int edge) {
			bool previousPointBitCode = (GetBitCode(previousPoint) & edge) > 0;
			if ((GetBitCode(currentPoint) & edge) == 0) {
				if (previousPointBitCode)
					clippedPoints.AddPoint(GetEdgeIntersection(previousPoint, currentPoint, edge));
				clippedPoints.AddPoint(currentPoint);
			}
			else if (!previousPointBitCode)
				clippedPoints.AddPoint(GetEdgeIntersection(previousPoint, currentPoint, edge));
		}
	}
}
