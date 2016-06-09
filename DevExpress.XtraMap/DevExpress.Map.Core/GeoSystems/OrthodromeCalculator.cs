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
using System.Linq;
using System.Text;
namespace DevExpress.Map.Native {
	public class OrthodromeCalculator {
		CoordObjectFactory pointFactory;
		double lat1; 
		double lat2;
		double lon1;
		double lon2;
		double sinLat1;
		double sinLat2;
		double cosLat1;
		double cosLat2;
		double dist;
		double sinDist;
		public OrthodromeCalculator(CoordObjectFactory pointFactory) {
			this.pointFactory = pointFactory;
		}
		int CalculatePointsCount(CoordPoint pt1, CoordPoint pt2) {
			double dy = pt2.GetY() - pt1.GetY();
			double dx = Math.Abs(pt2.GetX() - pt1.GetX());
			if(dx > 180)
				dx = 360 - dx;
			return (int)Math.Sqrt(dy * dy + dx * dx);
		}
		CoordPoint CalcIntermediatePoint(double t) {
			double a = Math.Sin((1 - t) * dist) / sinDist;
			double b = Math.Sin(t * dist) / sinDist;
			double x = a * cosLat1 * Math.Cos(lon1) + b * cosLat2 * Math.Cos(lon2);
			double y = a * cosLat1 * Math.Sin(lon1) + b * cosLat2 * Math.Sin(lon2);
			double z = a * sinLat1 + b * sinLat2;
			double lat = Math.Atan2(z, Math.Sqrt(x * x + y * y));
			double lon = Math.Atan2(y, x);
			lat = MathUtils.Radian2Degree(lat);
			lon = MathUtils.Radian2Degree(lon);
			return pointFactory.CreatePoint(lon, lat);
		}
		bool IsHopOverMap(int lastSegmentIndex, CoordPoint point, IList<CoordPoint> points) {
			return lastSegmentIndex == 0 && points.Count > 0 && point.GetX() * points[points.Count - 1].GetX() < 0 && Math.Abs(point.GetX()) > 90.0;
		}
		double CalculateEdgeLatitude(IList<CoordPoint> list, CoordPoint point) {
			return (point.GetY() + list[list.Count - 1].GetY()) / 2.0;
		}
		double CalculateEdgeLongitude(IList<CoordPoint> list) {
			CoordPoint lastPoint = list[list.Count - 1];
			return lastPoint.GetX() < 0 ? -180.0 : 180.0;
		}
		public IList<IList<CoordPoint>> CalculateLine(CoordPoint point1, CoordPoint point2) {
			int intermediatePointsCount = CalculatePointsCount(point1, point2);
			if(intermediatePointsCount == 0)
				return new CoordPoint[][] { new CoordPoint[] { point1, point2 } };
			List<IList<CoordPoint>> line = new List<IList<CoordPoint>>();
			line.Add(new List<CoordPoint>());
			int lastSegmentIndex = 0;
			lat1 = MathUtils.Degree2Radian(point1.GetY());
			lat2 = MathUtils.Degree2Radian(point2.GetY());
			lon1 = MathUtils.Degree2Radian(point1.GetX());
			lon2 = MathUtils.Degree2Radian(point2.GetX());
			sinLat1 = Math.Sin(lat1);
			sinLat2 = Math.Sin(lat2);
			cosLat1 = Math.Cos(lat1);
			cosLat2 = Math.Cos(lat2);
			dist = Math.Acos(sinLat1 * sinLat2 + cosLat1 * cosLat2 * Math.Cos(lon1 - lon2));
			sinDist = Math.Sin(dist);
			for(int i = 0; i < intermediatePointsCount; i++) {
				double t = 1.0 / intermediatePointsCount * i;
				CoordPoint point = CalcIntermediatePoint(t);
				if(IsHopOverMap(lastSegmentIndex, point, line[lastSegmentIndex])) {
					double latOnEdge = CalculateEdgeLatitude(line[0], point);
					double lonOnEdge = CalculateEdgeLongitude(line[0]);
					line[0].Add(pointFactory.CreatePoint(lonOnEdge, latOnEdge));
					lastSegmentIndex = 1;
					line.Add(new List<CoordPoint>());
					line[1].Add(pointFactory.CreatePoint(-lonOnEdge, latOnEdge));
				}
				line[lastSegmentIndex].Add(point);
			}
			line[lastSegmentIndex].Add(point2);
			return line;
		}
	}
}
