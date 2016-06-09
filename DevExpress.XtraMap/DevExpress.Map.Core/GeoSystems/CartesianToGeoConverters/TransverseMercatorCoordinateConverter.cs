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
namespace DevExpress.Map.Native {
	public class TransverseMercatorCartesianToGeoConverterCore : CoordinateConverterCore {
		const double C00 = 1.0;
		const double C02 = 0.25;
		const double C04 = 0.046875;
		const double C06 = 0.01953125;
		const double C08 = 0.01068115234375;
		const double C22 = 0.75;
		const double C44 = 0.46875;
		const double C46 = 0.01302083333333333333;
		const double C48 = 0.00712076822916666666;
		const double C66 = 0.36458333333333333333;
		const double C68 = 0.00569661458333333333;
		const double C88 = 0.3076171875;
		const double FC1 = 1.0;
		const double FC2 = 0.5;
		const double FC3 = 0.16666666666666666666;
		const double FC4 = 0.08333333333333333333;
		const double FC5 = 0.05;
		const double FC6 = 0.03333333333333333333;
		const double FC7 = 0.02380952380952380952;
		const double FC8 = 0.01785714285714285714;
		const int MaxIterationCount = 10;
		const double CircleLenRad = 2 * Math.PI;
		readonly double equatorLength;
		readonly double ml0;
		readonly double[] e_n;
		double scaleFactor;
		internal double ScaleFactor { get { return scaleFactor; } }
		public TransverseMercatorCartesianToGeoConverterCore(CoordObjectFactory geoPointFactory, EllipsoidCore ellipsoid, 
															 double falseEasting, double falseNorthing, 
															 CoordPoint projectionCenter, double scaleFactor)
			: base(geoPointFactory, ellipsoid, falseEasting, falseNorthing, projectionCenter) {
			this.scaleFactor = scaleFactor;
			this.equatorLength = 2 * Math.PI * Ellipsoid.SemimajorAxis;
			this.e_n = CalculateEnParametersArray(Ellipsoid.SquareOfEccentricity);
			double projectionLatitude = MathUtils.Degree2Radian(ProjectionCenter.YCoord);
			this.ml0 = CalculateMl(projectionLatitude, Math.Sin(projectionLatitude), Math.Cos(projectionLatitude), e_n);
		}
		double[] CalculateEnParametersArray(double squareOfEllipseEccentricity) {
			double e2 = squareOfEllipseEccentricity;
			double e4 = e2 * e2;
			double e6 = e4 * e2;
			double[] en = new double[5];
			en[0] = C00 - e2 * (C02 + e2 * (C04 + e2 * (C06 + e2 * C08)));
			en[1] = e2 * (C22 - e2 * (C04 + e2 * (C06 + e2 * C08)));
			en[2] = e4 * (C44 - e2 * (C46 + e2 * C48));
			en[3] = e6 * (C66 - e2 * C68);
			en[4] = e6 * e2 * C88;
			return en;
		}
		double CalculateMl(double phi, double sphi, double cphi, double[] e_n) {
			double sphi2 = sphi * sphi;
			return e_n[0] * phi - cphi * sphi * (e_n[1] + sphi2 * (e_n[2] + sphi2 * (e_n[3] + sphi2 * e_n[4])));
		}
		double CalculateInvMl(double arg, double squareOfEllipseEccentricity, double[] en) {
			double phi;
			double k = 1.0 / (1.0 - squareOfEllipseEccentricity);
			phi = arg;
			for (int i = MaxIterationCount; i != 0; i--) {
				double sinPhi = Math.Sin(phi);
				double tmp = 1.0 - squareOfEllipseEccentricity * sinPhi * sinPhi;
				tmp = (CalculateMl(phi, sinPhi, Math.Cos(phi), en) - arg) * (tmp * Math.Sqrt(tmp)) * k;
				phi -= tmp;
				if (Math.Abs(tmp) < 1e-11)
					return phi;
			}
			return phi;
		}
		public override CoordPoint Convert(CoordPoint cartesianPoint) {
			var eastingRad = CircleLenRad * (cartesianPoint.XCoord - FalseEasting) / this.equatorLength;
			var northingRad = CircleLenRad * (cartesianPoint.YCoord - FalseNorthing) / this.equatorLength;
			double longitudeRad;
			double latitudeRad = CalculateInvMl(this.ml0 + northingRad / ScaleFactor, Ellipsoid.SquareOfEccentricity, e_n);
			if (Math.Abs(northingRad) >= Math.PI / 2.0) {
				latitudeRad = northingRad < 0.0 ? -Math.PI / 2.0 : Math.PI / 2.0;
				longitudeRad = 0.0;
			}
			else {
				double sinPhi = Math.Sin(latitudeRad);
				double cosPhi = Math.Cos(latitudeRad);
				double tmp = Math.Abs(cosPhi) > 1e-10 ? sinPhi / cosPhi : 0.0;
				double n = Ellipsoid.SquareOfEccentricity / (1.0 - Ellipsoid.SquareOfEccentricity) * cosPhi * cosPhi;
				double con = 1.0 - Ellipsoid.SquareOfEccentricity * sinPhi * sinPhi;
				double d = eastingRad * Math.Sqrt(con) / ScaleFactor;
				con *= tmp;
				tmp *= tmp;
				latitudeRad -= (con * d * d / (1.0 - Ellipsoid.SquareOfEccentricity)) * FC2 *
					(1.0 - d * d * FC4 * (5.0 + tmp * (3.0 - 9.0 * n) + n * (1.0 - 4 * n) - d * d * FC6 *
					(61.0 + tmp * (90.0 - 252.0 * n + 45.0 * tmp) + 46.0 * n - d * d * FC8 * (1385.0 + tmp *
					(3633.0 + tmp * (4095.0 + 1574.0 * tmp))))));
				longitudeRad = d * (FC1 - d * d * FC3 * (1.0 + 2.0 * tmp + n - d * d * FC5 *
					(5.0 + tmp * (28.0 + 24.0 * tmp + 8.0 * n) + 6.0 * n - d * d * FC7 *
					(61.0 + tmp * (662.0 + tmp * (1320.0 + 720.0 * tmp)))))) / cosPhi;
			}
			double lon = MathUtils.Radian2Degree(longitudeRad) + ProjectionCenter.XCoord;
			double lat = MathUtils.Radian2Degree(latitudeRad);
			return PointFactory.CreatePoint(lon, MathUtils.ValidateLatitude(lat));
		}
	}
	public class UTMCartesianToGeoConverterCore : TransverseMercatorCartesianToGeoConverterCore {
		public const double UtmFalseEasting = 500000; 
		public const double UtmScaleFactor = 0.9996;
		public const double UtmCenterLatitude = 0.0;
		public const double UtmZoneWidthDegree = 6.0; 
		public const double MinLongitudeDegree = -180.0; 
		public const double UtmFalseNorthingOnNorthenHemisphere = 0.0; 
		public const double UtmFalseNorthingOnSouthernHemisphere = 10000000; 
		public static CoordPoint GetProjectionCenter(int utmZone, CoordObjectFactory factory) {
			double longitude = MinLongitudeDegree + (utmZone - 1) * UtmZoneWidthDegree + UtmZoneWidthDegree / 2.0;
			return factory.CreatePoint(longitude, UtmCenterLatitude);
		}
		public static double GetFalseNorthing(HemisphereCore hemisphere) {
			return hemisphere == HemisphereCore.Northen ? UtmFalseNorthingOnNorthenHemisphere : UtmFalseNorthingOnSouthernHemisphere;
		}
		public UTMCartesianToGeoConverterCore(CoordObjectFactory factory, int utmZone, HemisphereCore hemisphere)
			: base(factory, EllipsoidCore.WGS84, UtmFalseEasting, GetFalseNorthing(hemisphere), GetProjectionCenter(utmZone, factory), UtmScaleFactor) {
		}
	}
	public enum HemisphereCore{
		Northen = 0,
		Southern = 1
	}
}
