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
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {  
	[Serializable,
	TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)
	]
	public sealed class GeoPoint : CoordPoint {
		const double DefaultLatitude = 0.0;
		const double DefaultLongitude = 0.0;
		static public GeoPoint Parse(string source) {
			double[] parsedDoubles = PointParser.Parse(source, MapLocalizer.GetString(MapStringId.MsgIncorrectStringFormat));
			return new GeoPoint(parsedDoubles[0], parsedDoubles[1]);
		}
		public static GeoPoint Normalize(GeoPoint geoPoint) {
			return new GeoPoint(Math.Max(-90, Math.Min(geoPoint.Latitude, 90)), MathUtils.NormalizeDegree(geoPoint.Longitude));
		}
		[RefreshProperties(RefreshProperties.All), NotifyParentProperty(true),
#if !SL
	DevExpressXtraMapLocalizedDescription("GeoPointLatitude"),
#endif
		DefaultValue(DefaultLatitude)]
		public double Latitude { 
			get { return YCoord; } 
			set { YCoord = value; }
		}
		[RefreshProperties(RefreshProperties.All), NotifyParentProperty(true),
#if !SL
	DevExpressXtraMapLocalizedDescription("GeoPointLongitude"),
#endif
		DefaultValue(DefaultLongitude)]
		public double Longitude { 
			get { return XCoord; } 
			set { XCoord = value; } 
		}
		public GeoPoint() : base(DefaultLongitude, DefaultLatitude) { 
		}
		public GeoPoint(double latitude, double longitude) : base(longitude, latitude) {
			if(!IsLatitudeCorrect(latitude))
				throw new ArgumentException("Incorrect Latitude", "value");
		}
		internal GeoPoint(double latitude, double longitude, bool suppressLongitudeCheck)
			: base(longitude, latitude) {
				if (!IsLatitudeCorrect(latitude) && !suppressLongitudeCheck)
					throw new ArgumentException("Incorrect Latitude", "value");
		}
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
	[Serializable,
	TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)
	]
	public sealed class CartesianPoint : CoordPoint {
		const double DefaultX = 0.0;
		const double DefaultY = 0.0;
		static public CartesianPoint Parse(string source) {
			double[] parsedDoubles = PointParser.Parse(source, MapLocalizer.GetString(MapStringId.MsgIncorrectStringFormat));
			return new CartesianPoint(parsedDoubles[0], parsedDoubles[1]);
		}
		[RefreshProperties(RefreshProperties.All), NotifyParentProperty(true),
#if !SL
	DevExpressXtraMapLocalizedDescription("CartesianPointX"),
#endif
		DefaultValue(DefaultX)]
		public double X { 
			get { return XCoord; } 
			set { XCoord = value; } 
		}
		[RefreshProperties(RefreshProperties.All), NotifyParentProperty(true),
#if !SL
	DevExpressXtraMapLocalizedDescription("CartesianPointY"),
#endif
		DefaultValue(DefaultY)]
		public double Y { 
			get { return YCoord; } 
			set { YCoord = value; } 
		}
		public CartesianPoint() : base(DefaultX, DefaultY) { 
		}
		public CartesianPoint(double x, double y) : base(x, y) { 
		}
		protected override CoordPoint CreateNormalized() {
			return new CartesianPoint(X, Y);
		}
		public override CoordPoint Offset(double offsetX, double offsetY) {
			return new CartesianPoint(X + offsetX, Y + offsetY);
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class GeoPointFactory : CoordObjectFactory {
		readonly static GeoPointFactory instance = new GeoPointFactory();
		public static GeoPointFactory Instance { get { return instance; } }
		public override CoordPoint CreatePoint(double x, double y) {
			return new GeoPoint(y, x, true);
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
