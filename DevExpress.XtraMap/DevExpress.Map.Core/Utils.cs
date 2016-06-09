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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using DevExpress.Utils;
using System.Globalization;
namespace DevExpress.Map.Native {
	public static class CoreUtils {
		static int GetIntBigEndian(int value) {
			return (int)(((
				(value >> 0x18) +
				(((byte)(value >> 0x10)) << 0x8)) +
				(((byte)(value >> 0x8)) << 0x10)) +
				(((byte)(value)) << 0x18));
		}
		public static IList<double> SortDoubleCollection(IList<double> collection) {
			if (collection == null) return new List<double>();
			List<double> list = new List<double>(collection);
			list.Sort(Comparer<double>.Default);
			return list;
		}
		public static IEnumerable<Type> GetTypeDescendants(Assembly assembly, Type type) {
			return assembly.GetTypes().Where(t => !t.IsAbstract() && t.IsPublic() && type.IsAssignableFrom(t) && !t.Equals(type));
		}
		[SecuritySafeCritical]
		public static object CreateEmptyObject(Type type) {
#if !DXPORTABLE
			return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
#else
			return Activator.CreateInstance(type);
#endif
		}
		public static string ColorToBGRHex(Color color, bool isPrintAlpha) {
			string codeAlpha = isPrintAlpha ? String.Format("{0:x2}", color.A) : String.Empty;
			return String.Format("{0}{1:x2}{2:x2}{3:x2}", codeAlpha, color.B, color.G, color.R);
		}
		public static string ColorToRGBHex(Color color, bool isPrintAlpha) {
			string codeAlpha = isPrintAlpha ? String.Format("{0:x2}", color.A) : String.Empty;
			return String.Format("{0}{1:x2}{2:x2}{3:x2}", codeAlpha, color.R, color.G, color.B);
		}
		public static string GetFilePath(Uri uri) {
			if(!uri.IsAbsoluteUri)
				uri = new Uri(Path.GetFullPath(uri.OriginalString), UriKind.Absolute);
			return uri.AbsolutePath;
		}
		public static Uri GetDbfUri(Uri uri) {
			return new Uri(GetDbfPath(uri.ToString()), UriKind.RelativeOrAbsolute);
		}
		public static string GetDbfPath(string filePath) {
			string text = filePath.ToLower();
			string ext = Path.GetExtension(filePath);
			return string.IsNullOrEmpty(ext) ? text + ".dbf" : text.Replace(ext, ".dbf");
		}
		public static void WriteInt(BinaryWriter writer, int value, bool useBigEndian) {
			if(useBigEndian)
				value = GetIntBigEndian(value);
			writer.Write(value);
		}
		public static void WriteDouble(BinaryWriter writer, double value) {
			writer.Write(value);
		}
		public static void WriteByte(BinaryWriter writer, byte value) {
			writer.Write(value);
		}
		[CLSCompliant(false)]
		public static void WriteUInt16(BinaryWriter writer, UInt16 value) {
			writer.Write(value);
		}
		public static void WriteString(BinaryWriter writer, string value) {
			byte[] bytes = Encoding.ASCII.GetBytes(value);
			writer.Write(bytes);
		}
		public static void WriteBool(BinaryWriter writer, bool value) {
			writer.Write(value);
		}
		public static object TryConvertValue(Type newType, object correctFieldValueByType) {
			object value;
			try {
				value = Convert.ChangeType(correctFieldValueByType, newType, CultureInfo.InvariantCulture);
			} catch {
				value = newType != typeof(DateTime) ? Activator.CreateInstance(newType) : new DateTime(1900, 0, 1);
			}
			return value;
		}
	}
	public class PathSegmentHelperCore {
		IPathCore path;
		CoordObjectFactory pointFactory;
		int maxSegmentIndex = -1;
		double maxSegmentArea = Double.NegativeInfinity;
		CoordBounds maxSegmentBounds = CoordBounds.Empty;
		bool isPathActual = false;
		public CoordBounds MaxSegmentBounds { get { return maxSegmentBounds; } }
		public int MaxSegmentIndex {
			get {
				UpdateMapPathActuality();
				return maxSegmentIndex; 
			} 
		}
		public IPathSegmentCore MaxSegment { get { return MaxSegmentIndex >= 0 ? path.GetSegment(maxSegmentIndex) : null; } }
		public PathSegmentHelperCore(IPathCore path, CoordObjectFactory pointFactory) {
			this.path = path;
			UpdatePointFactory(pointFactory);
		}
		void UpdateMaxSegment() {
			double maxBoundsArea = Double.NegativeInfinity;
			maxSegmentIndex = -1;
			for(int i = 0; i < path.SegmentCount; i++) {
				IPathSegmentCore segment = path.GetSegment(i);
				if(segment == null)
					continue;
				CoordBounds bounds = segment.GetBounds();
				double boundsArea = bounds.Width * bounds.Height;
				if(boundsArea > maxBoundsArea) {
					maxBoundsArea = boundsArea;
					maxSegmentIndex = i;
				}
			}
		}
		void UpdateMapPathActuality() {
			if(isPathActual)
				return;
			UpdateMaxSegment();
			if((maxSegmentIndex != -1) && (path.GetSegment(maxSegmentIndex).PointCount > 0))
				maxSegmentArea = CentroidHelper.CalculatePolygonArea(path.GetSegment(maxSegmentIndex));
			this.isPathActual = true;
		}
		public CoordPoint GetMaxSegmentCenter() {
			UpdateMapPathActuality();
			if(Math.Abs(maxSegmentArea) < 10e-10)
				return null;
			IPathSegmentCore maxSegment = maxSegmentIndex >= 0 ? path.GetSegment(maxSegmentIndex) : null;
			return CentroidHelper.CalculatePolygonCentroid(pointFactory, maxSegment, maxSegmentArea);
		}
		public void Reset() {
			this.isPathActual = false;
		}
		public void UpdatePointFactory(CoordObjectFactory pointFactory) {
			this.pointFactory = pointFactory;
		}
	}
	public static class CentroidHelper {
		static CoordPoint CalculatePolygonCentroidCore(CoordObjectFactory pointFactory, IPolygonCore polygon, double area) {
			double horizontalCenter = 0;
			double verticalCenter = 0;
			for(int i = 0; i < polygon.PointCount - 1; i++) {
				IMapUnit increment = GetCentroidIncrement(polygon, i, i + 1, area);
				horizontalCenter += increment.X;
				verticalCenter += increment.Y;
			}
			if(polygon.GetPoint(0) != polygon.GetPoint(polygon.PointCount - 1)) {
				IMapUnit increment = GetCentroidIncrement(polygon, polygon.PointCount - 1, 0, area);
				horizontalCenter += increment.X;
				verticalCenter += increment.Y;
			}
			return pointFactory.CreatePoint(horizontalCenter + polygon.GetPoint(0).GetX(), verticalCenter + polygon.GetPoint(0).GetY());
		}
		static IMapUnit GetCentroidIncrement(IPolygonCore polygon, int i1, int i2, double area) {
			double commonFactor = 1.0 / 6.0 / area * (polygon.GetPoint(i1).GetX() - polygon.GetPoint(0).GetX()) * (polygon.GetPoint(i2).GetY() - polygon.GetPoint(0).GetY()) - 1.0 / 6.0 / area * (polygon.GetPoint(i2).GetX() - polygon.GetPoint(0).GetX()) * (polygon.GetPoint(i1).GetY() - polygon.GetPoint(0).GetY());
			double horizontalIncrement = (polygon.GetPoint(i1).GetX() - polygon.GetPoint(0).GetX() + polygon.GetPoint(i2).GetX() - polygon.GetPoint(0).GetX()) * commonFactor;
			double vertiaclIncrement = (polygon.GetPoint(i1).GetY() - polygon.GetPoint(0).GetY() + polygon.GetPoint(i2).GetY() - polygon.GetPoint(0).GetY()) * commonFactor;
			return new MapUnitCore(horizontalIncrement, vertiaclIncrement);
		}
		static double GetSegmentLineIntersection(IPolygonCore segment, int i1, int i2, double axis) {
			return (axis - segment.GetPoint(i1).GetY()) * (segment.GetPoint(i2).GetX() - segment.GetPoint(i1).GetX()) / (segment.GetPoint(i2).GetY() - segment.GetPoint(i1).GetY()) + segment.GetPoint(i1).GetX();
		}
		static List<double> GetIntersectionsAtYLevel(double axis, IPolygonCore polygon) {
			List<double> result = new List<double>();
			for(int i = 0; i < polygon.PointCount - 1; i++) {
				if(polygon.GetPoint(i).GetY() > axis ^ polygon.GetPoint(i + 1).GetY() > axis)
					result.Add(GetSegmentLineIntersection(polygon, i, i + 1, axis));
			}
			if(polygon.GetPoint(0).GetY() > axis ^ polygon.GetPoint(polygon.PointCount - 1).GetY() > axis)
				result.Add(GetSegmentLineIntersection(polygon, 0, polygon.PointCount - 1, axis));
			result.Sort();
			return result;
		}
		static bool IsPointInisidePolygon(int intersectsCount) {
			return (intersectsCount % 2 == 1);
		}
		static CoordPoint GetClosestPoint(CoordObjectFactory pointFactory, CoordPoint inititalPoint, int intersectionsCount, List<double> intersections) {
			double rightCenter = Double.PositiveInfinity;
			double leftCenter = Double.PositiveInfinity;
			if(intersectionsCount > 0)
				leftCenter = intersections[intersectionsCount - 2] + (intersections[intersectionsCount - 1] - intersections[intersectionsCount - 2]) / 2;
			if(intersectionsCount < intersections.Count)
				rightCenter = intersections[intersectionsCount] + (intersections[intersectionsCount + 1] - intersections[intersectionsCount]) / 2;
			double newX = (Math.Abs(rightCenter - inititalPoint.GetX()) > Math.Abs(inititalPoint.GetX() - leftCenter)) ? leftCenter : rightCenter;
			return pointFactory.CreatePoint(Double.IsPositiveInfinity(newX) ? inititalPoint.GetX() : newX, inititalPoint.GetY());
		}
		static int GetIntersectsCount(double longitude, List<double> intersects) {
			int result = 0;
			for(int i = 0; i < intersects.Count; i++)
				if(longitude > intersects[i])
					result++;
			return result;
		}
		static CoordPoint MovePointInsidePolygonIfNeeded(CoordObjectFactory pointFactory, CoordPoint centroid, IPolygonCore polygon) {
			List<double> intersectionLongs = GetIntersectionsAtYLevel(centroid.GetY(), polygon);
			int intersectsCount = GetIntersectsCount(centroid.GetX(), intersectionLongs);
			return IsPointInisidePolygon(intersectsCount) ? centroid : GetClosestPoint(pointFactory, centroid, intersectsCount, intersectionLongs);
		}
		static double GetAreaIncrement(IPolygonCore segment, int i1, int i2) {
			return 1.0 / 2.0 * (segment.GetPoint(i1).GetX() - segment.GetPoint(0).GetX()) * (segment.GetPoint(i2).GetY() - segment.GetPoint(0).GetY()) - 1.0 / 2.0 * (segment.GetPoint(i2).GetX() - segment.GetPoint(0).GetX()) * (segment.GetPoint(i1).GetY() - segment.GetPoint(0).GetY());
		}
		public static CoordPoint CalculatePolygonCentroid(CoordObjectFactory pointFactory, IPolygonCore polygon, double area) {
			if(polygon == null || polygon.PointCount == 0)
				return pointFactory.CreatePoint(0, 0);
			CoordPoint centroid = CalculatePolygonCentroidCore(pointFactory, polygon, area);
			centroid = MovePointInsidePolygonIfNeeded(pointFactory, centroid, polygon);
			return centroid;
		}
		public static double CalculatePolygonArea(IPolygonCore polygon) {
			double result = 0;
			for(int i = 0; i < polygon.PointCount - 1; i++)
				result += GetAreaIncrement(polygon, i, i + 1);
			if(polygon.PointCount > 0)
				result += GetAreaIncrement(polygon, polygon.PointCount - 1, 0);
			return result;
		}
	}
	public static class BoundingRectItemHelper {
		public static CoordPoint CalculateLocation(MapCoordinateSystemCore coordSystem, CoordPoint center, double width, double height) {
			MapSizeCore topSize = coordSystem.KilometersToGeoSize(center, new MapSizeCore(0.0, height / 2.0));
			CoordPoint topPoint = coordSystem.PointFactory.CreatePoint(center.GetX(), center.GetY() + topSize.Height);
			MapSizeCore leftSize = coordSystem.KilometersToGeoSize(topPoint, new MapSizeCore(-width / 2.0, 0.0));
			return coordSystem.PointFactory.CreatePoint(center.GetX() + leftSize.Width, topPoint.GetY());
		}
	}
}
