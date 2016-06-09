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
using System.ComponentModel;
using DevExpress.Map;
using DevExpress.Map.Localization;
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
namespace DevExpress.Xpf.Map {
	[TypeConverter(typeof(GeoPointConverter))]
	public class GeoPoint : CoordPoint {
		public static GeoPoint Empty { 
			get { return new GeoPoint(double.NegativeInfinity, double.NegativeInfinity); }
		}
		static public GeoPoint Normalize(GeoPoint geoPoint) {
			return new GeoPoint(Math.Max(-90, Math.Min(geoPoint.Latitude, 90)), MathUtils.NormalizeDegree(geoPoint.Longitude));
		}
		static public GeoPoint Parse(string source) {
			double[] doubles = PointParser.Parse(source, DXMapStrings.MsgIncorrectGeoPointStringFormat);
			return new GeoPoint(doubles[0], doubles[1]);
		}
		[Category(Categories.Common)]
		public double Latitude { get { return YCoord; } set { YCoord = value; } }
		[Category(Categories.Common)]
		public double Longitude { get { return XCoord; } set { XCoord = value; } }
		internal GeoPoint(double latitude, double longitude, bool checkLatitude) :
			base(x: longitude, y: latitude) {
			if (checkLatitude && !IsLatitudeCorrect(latitude))
				throw new ArgumentException("Incorrent latitude", "value");
		}
		public GeoPoint(double latitudeDegree, double longitudeDegree)
			: this(latitudeDegree, longitudeDegree, checkLatitude: true) { }
		public GeoPoint()
			: this(0.0, 0.0, checkLatitude: true) { }
		bool IsLatitudeCorrect(double value) {
			return Double.IsNegativeInfinity(value) || !(value > 90.0 || value < -90.0);
		}
		protected override CoordPoint CreateNormalized() {
			return Normalize(this);
		}
		public override CoordPoint Offset(double offsetX, double offsetY) {
			return new GeoPoint(Latitude + offsetY, Longitude + offsetX);
		}
		public override string ToString(IFormatProvider provider) {
			return string.Format("{0}{1} {2}", Latitude.ToString(provider), GetCoordinateSeparator(provider), Longitude.ToString(provider));
		}
	}
	public class CartesianPoint : CoordPoint {
		static public CartesianPoint Parse(string source) {
			double[] doubles = PointParser.Parse(source, DXMapStrings.MsgIncorrectGeoPointStringFormat);
			return new CartesianPoint(doubles[0], doubles[1]);
		}
		[Category(Categories.Common)]
		public double X {
			get { return XCoord; }
			set { XCoord = value; }
		}
		[Category(Categories.Common)]
		public double Y {
			get { return YCoord; }
			set { YCoord = value; }
		}
		public CartesianPoint()
			: base(0, 0) {
		}
		public CartesianPoint(double x, double y)
			: base(x, y) {
		}
		protected override CoordPoint CreateNormalized() {
			return new CartesianPoint(X, Y);
		}
		public override CoordPoint Offset(double offsetX, double offsetY) {
			return new CartesianPoint(X + offsetX, Y + offsetY);
		}
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class GeoPointFactory : CoordObjectFactory {
		readonly static GeoPointFactory instance = new GeoPointFactory();
		public static GeoPointFactory Instance { get { return instance; } }
		public override CoordPoint CreatePoint(double x, double y) {
			return new GeoPoint(y, x, false);
		}
	}
	public class CartesianPointFactory : CoordObjectFactory {
		readonly static CartesianPointFactory instance = new CartesianPointFactory();
		public static CartesianPointFactory Instance { get { return instance; } }
		public override CoordPoint CreatePoint(double x, double y) {
			return new CartesianPoint(x, y);
		}
	}
}
