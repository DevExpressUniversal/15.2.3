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
namespace DevExpress.Map.Native {
	public class DouglasPeuckerLineSimplificator {
		struct Point {
			public static bool AreEquals(Point a, Point b) {
				return (Math.Abs(a.X - b.X) < double.Epsilon) && (Math.Abs(a.Y - b.Y) < double.Epsilon);
			}
			public double X { get; set; }
			public double Y { get; set; }
		}
		readonly double epsilon;
		CoordObjectFactory coordFactory;
		static double Distance(Point point, Point a, Point b) {
			double A = point.X - a.X;
			double B = point.Y - a.Y;
			double C = b.X - a.X;
			double D = b.Y - a.Y;
			return Math.Abs(A * D - C * B) / Math.Sqrt(C * C + D * D);
		}
		public DouglasPeuckerLineSimplificator(CoordObjectFactory coordFactory, double epsilon) {
			this.epsilon = epsilon;
			this.coordFactory = coordFactory;
		}
		Point SegmentPoint(IPathSegmentCore segment, int index) {
			CoordPoint point = segment.GetPoint(index);
			return new Point() { X = point.XCoord, Y = point.YCoord };
		}
		void SimplifySegment(IPathSegmentCore segment, List<int> liveIndexes, int startIndex, int endIndex) {
			double maxDistance = 0.0d;
			int maxIndex = 0;
			Point first = SegmentPoint(segment, startIndex);
			Point last = SegmentPoint(segment, endIndex);
			for (int index = startIndex; index < endIndex; index++) {
				double distance = Distance(SegmentPoint(segment, index), first, last);
				if (distance > maxDistance) {
					maxDistance = distance;
					maxIndex = index;
				}
			}
			if ((maxDistance > epsilon) && (maxIndex != 0)) {
				liveIndexes.Add(maxIndex);
				SimplifySegment(segment, liveIndexes, startIndex, maxIndex);
				SimplifySegment(segment, liveIndexes, maxIndex, endIndex);
			}
		}
		void SimplifySegment(IPathSegmentCore segment, IPathSegmentCore destination) {
			int startIndex = 0;
			int endIndex = segment.PointCount - 1;
			List<int> liveIndexes = new List<int>();
			Point first = SegmentPoint(segment, startIndex);
			Point last = SegmentPoint(segment, endIndex);
			while (Point.AreEquals(first, last) && (endIndex != startIndex)) {
				endIndex--;
				last = SegmentPoint(segment, endIndex);
			}
			if (endIndex != startIndex) {
				liveIndexes.Add(startIndex);
				liveIndexes.Add(endIndex);
				SimplifySegment(segment, liveIndexes, startIndex, endIndex);
			} else {
				liveIndexes.Add(startIndex);
				liveIndexes.Add(startIndex + 1);
			}
			liveIndexes.Sort();
			foreach (int index in liveIndexes) {
				Point point = SegmentPoint(segment, index);
				destination.AddPoint(this.coordFactory.CreatePoint(point.X, point.Y));
			}
		}
		public void Simplify(IPathCore source, IPathCore destination) {
			for (int index = 0; index < source.SegmentCount; index++) {
				IPathSegmentCore sourceSegment = source.GetSegment(index);
				IPathSegmentCore destinationSegment = destination.CreateSegment();
				SimplifySegment(sourceSegment, destinationSegment);
			}
		}
	}
}
