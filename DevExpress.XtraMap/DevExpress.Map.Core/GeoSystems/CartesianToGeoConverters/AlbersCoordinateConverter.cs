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
	public class AlbersCartesianToGeoConverterCore : CoordinateConverterCore {
		const double Epsilon = 10e-10;
		const int MaxIterationsCount = 15;
		double stdParallelN;
		double stdParallelS;
		double n;
		double c;
		double rho0;
		public double StdParallelN { get { return stdParallelN; } }
		public double StdParallelS { get { return stdParallelS; } }
		public AlbersCartesianToGeoConverterCore(CoordObjectFactory factory, EllipsoidCore ellipsoid,
															double falseEasting, double falseNorthing, CoordPoint projectionCenter,
															double standardParallelN, double standardParallelS)
			: base(factory, ellipsoid, falseEasting, falseNorthing, projectionCenter) {
				this.stdParallelN = standardParallelN;
				this.stdParallelS = standardParallelS;
				if(stdParallelN <= stdParallelS)
					throw new InvalidCoordinateConverterParameterException("Invalid coordinate converter parameter: StandardParallelN must be greater than StandardParallelS");
				CalculateNC();
				CalculateRho0();
		}
		void CalculateNC() {
			double latSRad = MathUtils.Degree2Radian(stdParallelS);
			double latNRad = MathUtils.Degree2Radian(stdParallelN);
			double temp1 = Math.Sin(latSRad);
			double temp2 = Math.Sin(latNRad);
			double temp3 = 1 - Ellipsoid.SquareOfEccentricity * temp1 * temp1;
			double temp4 = 1 - Ellipsoid.SquareOfEccentricity * temp2 * temp2;
			double m1 = Math.Cos(latSRad) / Math.Sqrt(temp3);
			double m2 = Math.Cos(latNRad) / Math.Sqrt(temp4);
			double q1 = (1 - Ellipsoid.SquareOfEccentricity) * Math.Abs(temp1 / temp3 - (0.5 / Ellipsoid.Eccentricity) * Math.Log((1 - Ellipsoid.Eccentricity * temp1) / (1 + Ellipsoid.Eccentricity * temp1)));
			double q2 = (1 - Ellipsoid.SquareOfEccentricity) * Math.Abs(temp2 / temp4 - (0.5 / Ellipsoid.Eccentricity) * Math.Log((1 - Ellipsoid.Eccentricity * temp2) / (1 + Ellipsoid.Eccentricity * temp2)));
			this.n = (m1 * m1 - m2 * m2) / (q2 - q1);
			this.c = m1 * m1 + n * q1;
		}
		void CalculateRho0() {
			double temp5 = Math.Sin(MathUtils.Degree2Radian(ProjectionCenter.YCoord));
			double temp6 = 1 - Ellipsoid.SquareOfEccentricity * temp5 * temp5;
			double q0 = (1 - Ellipsoid.SquareOfEccentricity) * Math.Abs(temp5 / temp6 - (0.5 / Ellipsoid.Eccentricity) * Math.Log((1 - Ellipsoid.Eccentricity * temp5) / (1 + Ellipsoid.Eccentricity * temp5)));
			this.rho0 = Ellipsoid.SemimajorAxis * Math.Sqrt(c - n * q0) / n;
		}
		double CalculateLatitude(double q) {
			double lat0;
			q = Math.Max(-2, Math.Min(2, q));
			double lat = Math.Asin(q / 2);
			int iterationsCount = 0;
			do {
				lat0 = lat;
				double temp1 = Math.Sin(lat0);
				double temp2 = Ellipsoid.Eccentricity * temp1;
				double temp3 = 1 - temp2 * temp2;
				lat = lat0 + temp3 * temp3 / (2.0 * Math.Cos(lat0)) * (q / (1 - Ellipsoid.SquareOfEccentricity) - temp1 / temp3 + 0.5 / Ellipsoid.Eccentricity * Math.Log((1- temp2) / (1 + temp2)));
			} while(Math.Abs(lat0 - lat) >= Epsilon && (iterationsCount++ <= MaxIterationsCount));
			return MathUtils.Radian2Degree(lat);
		}
		public override CoordPoint Convert(CoordPoint point) {
			double xShift = point.XCoord - FalseEasting;
			double yShift = point.YCoord - FalseNorthing;
			double temp1 = rho0 - yShift;
			double rho = Math.Sqrt(xShift * xShift + (temp1 * temp1));
			double teta = Math.Atan(xShift / temp1);
			double q = (c - rho * rho * n * n / Ellipsoid.SemimajorAxis / Ellipsoid.SemimajorAxis) / n;
			double latitude = CalculateLatitude(q);
			double longitude = ProjectionCenter.XCoord + MathUtils.Radian2Degree(teta) / n;
			return PointFactory.CreatePoint(longitude, MathUtils.ValidateLatitude(latitude));
		}
	}
}
