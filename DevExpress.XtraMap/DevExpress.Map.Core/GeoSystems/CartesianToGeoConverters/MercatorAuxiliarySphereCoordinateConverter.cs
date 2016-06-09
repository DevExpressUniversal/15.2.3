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
	public class MercatorAuxiliarySphereCartesianToGeoConverterCore : CoordinateConverterCore {
		int sphereType;
		double sphereRadius;
		bool needCalcAuthalicLat;
		double invSphereRadius;
		double poleQ;
		protected internal double SphereRadius { get { return sphereRadius; } }
		public int SphereType { get { return sphereType; } }
		public MercatorAuxiliarySphereCartesianToGeoConverterCore(CoordObjectFactory factory, EllipsoidCore ellipsoid,
															double falseEasting, double falseNorthing, CoordPoint projectionCenter,
															int sphereType)
			: base(factory, ellipsoid, falseEasting, falseNorthing, projectionCenter) {
				this.sphereType = sphereType;
				this.sphereRadius = CalculateSphereRadius(sphereType);
				this.needCalcAuthalicLat = sphereType == 3;
				if(needCalcAuthalicLat)
					this.poleQ = CalculateQ(Math.PI / 2, Ellipsoid);
				this.invSphereRadius = 1.0 / sphereRadius;
		}
		double CalculateSphereRadius(int sphereType) {
			switch(sphereType) {
				case 0: return Ellipsoid.SemimajorAxis;
				case 1: return Ellipsoid.SemiminorAxis;
				case 2: 
				case 3: return CalculateAuthalicRadius(Ellipsoid);
				default: throw new InvalidCoordinateConverterParameterException("Invalid coordinate converter parameter: SphereType must be either 0 or 1 or 2 or 3");
			}
		}
		double CalculateAuthalicRadius(EllipsoidCore ellipsoid) {
			double a = ellipsoid.SemimajorAxis;
			double b = ellipsoid.SemiminorAxis;
			double e = ellipsoid.Eccentricity;
			return Math.Sqrt(a * a / 2.0 + b * b / 2.0 * MathUtils.AtanH(e) / e);
		}
		double CalculateAuthalicLatitude(double lat, EllipsoidCore ellipsoid) {
			double numerator = CalculateQ(lat, ellipsoid);
			double denominator = poleQ;
			return Math.Asin(numerator / denominator);
		}
		double CalculateQ(double lat, EllipsoidCore ellipsoid) {
			double e = ellipsoid.Eccentricity;
			double temp1 = Math.Sin(lat);
			return (1.0 - e * e) * temp1 / (1 - e * e * temp1 * temp1) + (1 - e * e) / e * MathUtils.AtanH(e * temp1);
		}
		public override CoordPoint Convert(CoordPoint point) {
			double xShift = point.XCoord - FalseEasting;
			double yShift = point.YCoord - FalseNorthing;
			double lat = Math.PI / 2.0 - (2.0 * Math.Atan(Math.Exp(-invSphereRadius * yShift)));
			if(needCalcAuthalicLat)
				lat = CalculateAuthalicLatitude(lat, Ellipsoid);
			double lon = xShift / sphereRadius;
			return PointFactory.CreatePoint(MathUtils.Radian2Degree(lon), MathUtils.ValidateLatitude(MathUtils.Radian2Degree(lat) + ProjectionCenter.XCoord));
		}
	}
}
