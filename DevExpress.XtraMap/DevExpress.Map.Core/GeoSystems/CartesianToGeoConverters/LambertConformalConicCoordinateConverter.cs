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
	public class LambertConformalConicCartesianToGeoConverterCore : CoordinateConverterCore {
		double stdParallelN;
		double stdParallelS;
		double e2;
		double phi0;
		double phi1;
		double phi2;
		double lambda0;
		double phi0t;
		double phi1m;
		double phi1t;
		double phi2m;
		double phi2t;
		double n;
		double f;
		double r0;
		public double StdParallelN { get { return stdParallelN; } }
		public double StdParallelS { get { return stdParallelS; } }
		public LambertConformalConicCartesianToGeoConverterCore(CoordObjectFactory factory, EllipsoidCore ellipsoid,
															double falseEasting, double falseNorthing, CoordPoint projectionCenter,
															double standardParallelN, double standardParallelS)
			: base(factory, ellipsoid, falseEasting, falseNorthing, projectionCenter) {
				this.stdParallelN = standardParallelN;
				this.stdParallelS = standardParallelS;
				if(stdParallelN <= stdParallelS)
					throw new InvalidCoordinateConverterParameterException("Invalid coordinate converter parameter: StandardParallelN must be greater than StandardParallelS");
				this.e2 = Ellipsoid.SquareOfEccentricity;
				this.phi0 = MathUtils.Degree2Radian(Math.Abs(projectionCenter.YCoord));
				this.phi1 = MathUtils.Degree2Radian(Math.Abs(stdParallelS));
				this.phi2 = MathUtils.Degree2Radian(Math.Abs(stdParallelN));
				this.lambda0 = MathUtils.Degree2Radian(Math.Abs(projectionCenter.XCoord));
				double e = Math.Sqrt(Ellipsoid.SquareOfEccentricity);
				this.phi1m = Math.Cos(phi1) / Math.Sqrt(1 - Ellipsoid.SquareOfEccentricity * Math.Pow(Math.Sin(phi1), 2.0));
				this.phi2m = Math.Cos(phi2) / Math.Sqrt(1 - Ellipsoid.SquareOfEccentricity * Math.Pow(Math.Sin(phi2), 2.0));
				this.phi0t = Math.Tan(Math.PI / 4.0 - phi0 / 2.0) / Math.Pow((1 - e * Math.Sin(phi0)) / (1 + e * Math.Sin(phi0)), e / 2.0);
				this.phi1t = Math.Tan(Math.PI / 4.0 - phi1 / 2.0) / Math.Pow((1 - e * Math.Sin(phi1)) / (1 + e * Math.Sin(phi1)), e / 2.0);
				this.phi2t = Math.Tan(Math.PI / 4.0 - phi2 / 2.0) / Math.Pow((1 - e * Math.Sin(phi2)) / (1 + e * Math.Sin(phi2)), e / 2.0);
				this.n = (Math.Log(phi1m) - Math.Log(phi2m)) / (Math.Log(phi1t) - Math.Log(phi2t));
				this.f = phi1m / (n * Math.Pow(phi1t, n));
				this.r0 = Ellipsoid.SemimajorAxis * f * Math.Pow(phi0t, n);
		}
		public override CoordPoint Convert(CoordPoint point) {
			double xShift = point.XCoord - FalseEasting;
			double yShift = point.YCoord - FalseNorthing;
			double r = Math.Sqrt(xShift * xShift + (r0 - yShift) * (r0 - yShift));
			double t = Math.Pow(r / (f * Ellipsoid.SemimajorAxis), 1.0 / n);
			double chi1 = Math.PI / 2.0 - 2.0 * Math.Atan(t);
			double chi2 = Math.Sin(2.0 * chi1) * (e2 / 2.0 + 5.0 / 24.0 * e2 * e2 + 1.0 / 12.0 * Math.Pow(e2, 3) + 13.0 / 360.0 * Math.Pow(e2, 4));
			double chi3 = Math.Sin(4.0 * chi1) * (7.0 / 48.0 * e2 * e2 + 29.0 / 240.0 * Math.Pow(e2, 3) + 811.0 / 11520.0 * Math.Pow(e2, 4));
			double chi4 = Math.Sin(6.0 * chi1) * (7.0 / 120.0 * Math.Pow(e2, 3) + 81.0 / 1120.0 * Math.Pow(e2, 4));
			double chi5 = Math.Sin(8.0 * chi1) * 4279.0 / 161280.0 * Math.Pow(e2, 4);			
			double latitude = chi1 + chi2 + chi3 + chi4 + chi5;
			double teta = Math.Atan(xShift / (r0 - yShift));
			double longitude = teta / n - lambda0;
			return PointFactory.CreatePoint(MathUtils.Radian2Degree(longitude), MathUtils.ValidateLatitude(MathUtils.Radian2Degree(latitude)));
		}
	}
}
