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
namespace DevExpress.Map.Native {
	public static class CoordinateSystemHelper {
		public static CoordPoint CreateNormalizedPoint(CoordPoint point) {
			return point.CreateNormalized();
		}
		public static bool IsNumericCoordinate(double coord) {
			return !Double.IsNaN(coord) && !Double.IsInfinity(coord);
		}
		public static bool IsNumericCoordPoint(CoordPoint point) {
			return point != null && IsNumericCoordinate(point.XCoord) && IsNumericCoordinate(point.YCoord);
		}
	}
	public static class MathUtils {
		public const float PI = 3.1415926f;
		public const double DoubleCompareEpsilon = 0.00001;
		public static int StrongRound(double value) {
			return Math.Sign(value) * (int)(Math.Abs(value) + 0.5);
		}
		public static double NormalizeRadian(double angleRadian) { 
			return Math.Atan2(Math.Sin(angleRadian), Math.Cos(angleRadian));
		}
		public static double NormalizeDegree(double angleDegree) {
			double angleRadian = NormalizeRadian(Degree2Radian(angleDegree));
			return Radian2Degree(angleRadian);
		}
		public static double Degree2Radian(double angleDegree) {
			return angleDegree * MathUtils.PI / 180.0;
		}
		public static double Radian2Degree(double angleRadian) {
			return angleRadian * 180.0 / MathUtils.PI;
		}
		public static double GetDegrees(double coordinate, int decimals) {
			return Truncate(coordinate, decimals);
		}
		public static double GetMinutes(double coordinate) {
			double partMinutes = coordinate - Truncate(coordinate, 0);
			return partMinutes * 60.0;
		}
		public static double GetMinutes(double coordinate, int decimals) {
			return Truncate(GetMinutes(coordinate), decimals);
		}
		public static double GetSeconds(double coordinate) {
			double partSeconds = GetMinutes(coordinate) - Truncate(GetMinutes(coordinate), 0);
			return partSeconds * 60.0;
		}
		public static double GetSeconds(double coordinate, int decimals) {
			return Truncate(GetSeconds(coordinate), decimals);
		}
		public static string GetLongituteString(double longitude) {
			string cardinalPoint = longitude > 0 ? "E" : "W";
			double lon = Math.Abs(longitude);
			string coord = string.Format("{0:F1}°{1:F1}'{2:F2}''", GetDegrees(lon, 0), GetMinutes(lon, 0), GetSeconds(lon, 2));
			return string.Format("{0}{1}", coord, cardinalPoint);
		}
		public static string GetLatitudeString(double latitude) {
			string cardinalPoint = latitude > 0 ? "N" : "S";
			double lat = Math.Abs(latitude);
			string coord = string.Format("{0:F1}°{1:F1}'{2:F2}''", GetDegrees(lat, 0), GetMinutes(lat, 0), GetSeconds(lat, 2));
			return string.Format("{0}{1}", coord, cardinalPoint);
		}
		public static double Truncate(double value, int decimals) {
			return Math.Sign(value) * (Math.Floor(Math.Abs(value) * Math.Pow(10, decimals)) / Math.Pow(10, decimals));
		}
		public static double MinMax(double value, double minLimit, double maxLimit) {
			return Math.Max(Math.Min(value, maxLimit), minLimit);
		}
		public static int MinMax(int value, int minLimit, int maxLimit) {
			return Math.Max(Math.Min(value, maxLimit), minLimit);
		}
		public static bool Compare(double v1, double v2) {
			return Math.Abs(v1 - v2) < DoubleCompareEpsilon;
		}
		public static bool IsNumeric(double d) {
			return !double.IsNaN(d) && !double.IsInfinity(d);
		}
		public static int GetRelativeCenter(int startCoord, int length1, int length2) {
			int delta = Math.Max(0, length2 - length1);
			return startCoord + delta / 2;
		}
		public static double AtanH(double x) {
			return Math.Log((1 + x) / (1 - x)) / 2;
		}
		public static double ValidateLatitude(double lat) {
			return MinMax(lat, -90, 90);
		}
		public static MapSizeCore CalcMapSizeInPixels(double zoomLevel, MapSizeCore initialMapSize) {
			if(zoomLevel < 1.0)
				return new MapSizeCore(zoomLevel * initialMapSize.Width, zoomLevel * initialMapSize.Height);
			double level = Math.Max(0.0, zoomLevel - 1.0);
			double coeff = Math.Pow(2.0, level);
			return new MapSizeCore(coeff * initialMapSize.Width, coeff * initialMapSize.Height);
		}
		public static bool ValueBetween(double value, double min, double max, bool over) {
			return over ? (value >= min && value <= max) : (value > min && value < max);
		}
		public static bool ValueBetween(double value, double min, double max) {
			return ValueBetween(value, min, max, true);
		}
	}
	public enum CardinalDirection {
		NorthSouth,
		WestEast
	}
	enum GeoCoordinateElement {
		Degree,
		Minute,
		Second,
		CardinalPoint,
		PrecisionDegree,
		PrecisionMinute,
		PrecisionSecond
	}
	enum CartesianCoordinateElement {
		Value,
		MeasureUnit,
		PrecisionValue
	}
}
