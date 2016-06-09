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

using DevExpress.Map.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Map.Native {
	public struct EllipsoidCore {
		#region Predefinde Ellipsoids
		static EllipsoidCore krassovsky = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378245.0, 298.3, "Krassovsky");
		static EllipsoidCore wgs66 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378145.0, 298.25, "WGS 66");
		static EllipsoidCore wgs72 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378135.0, 298.26, "WGS 72");
		static EllipsoidCore wgs84 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378137.0, 298.257223563, "WGS 84"); 
		static EllipsoidCore grs67 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378160.0, 298.247167427, "GRS 67");
		static EllipsoidCore grs80 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378137.0, 298.257222101, "GRS 80");
		static EllipsoidCore australianNational66 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378160.0, 298.25, "Australian National (1966)");
		static EllipsoidCore newInternational67 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378157.5, 298.24961539, "New International (1967)");
		static EllipsoidCore southAmerican69 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378160.0, 298.25, "South American (1969)");
		static EllipsoidCore iers89 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378136.0, 298.257, "IERS (1989)");
		static EllipsoidCore iers03 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378136.6, 298.25642, "IERS (2003)");
		static EllipsoidCore pz90 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378136.0, 298.25784, "ПЗ-90");	  
		static EllipsoidCore gsk11 = EllipsoidCore.CreateBySemimajorAndInverseFlattering(6378136.5, 298.2564151, "ГСК-2011"); 
		public static EllipsoidCore Krassovsky { get { return krassovsky; } }
		public static EllipsoidCore WGS66 { get { return wgs66; } }
		public static EllipsoidCore WGS72 { get { return wgs72; } }
		public static EllipsoidCore WGS84 { get { return wgs84; } }
		public static EllipsoidCore GRS67 { get { return grs67; } }
		public static EllipsoidCore GRS80 { get { return grs80; } } 
		public static EllipsoidCore AustralianNational66 { get { return australianNational66; } }
		public static EllipsoidCore NewInternational67 { get { return newInternational67; } }
		public static EllipsoidCore SouthAmerican69 {get { return southAmerican69; } }
		public static EllipsoidCore IERS89 { get { return iers89; } }
		public static EllipsoidCore IERS03 { get { return iers03; } }
		public static EllipsoidCore PZ90 { get { return pz90; } }
		public static EllipsoidCore GSK11 { get { return gsk11; } }
		#endregion
		public static EllipsoidCore CreateByTwoAxes(double semimajor, double semiminor, string name) {
			double flattering = (semimajor - semiminor) / semimajor;
			double squareOfFirstEccentrisity = (semimajor * semimajor - semiminor * semiminor) / (semimajor * semimajor);
			return new EllipsoidCore(semimajor, semiminor, flattering, squareOfFirstEccentrisity, name);
		}
		public static EllipsoidCore CreateBySemimajorAndInverseFlattering(double semimajor, double inverseFlattering, string name) {
			double flattering = 1.0 / inverseFlattering;
			double semiminor = semimajor - flattering * semimajor;
			double squareOfFirstEccentricity = flattering * (2.0 - flattering);
			return new EllipsoidCore(semimajor, semiminor, flattering, squareOfFirstEccentricity, name);
		}
		public double SemimajorAxis { get; private set; }		
		public double SemiminorAxis { get; private set; }		
		public double Flattening { get; private set; }		   
		public double Eccentricity { get; private set; }		 
		public double SquareOfEccentricity { get; private set; } 
		public string Name { get; private set; }
		EllipsoidCore(double semimajorAxis, double semiminorAxis, double flattening, double squareOfFirstEccentricity, string name)
			: this() {
			SemimajorAxis = semimajorAxis;
			SemiminorAxis = semiminorAxis;
			Flattening = flattening;
			SquareOfEccentricity = squareOfFirstEccentricity;
			Eccentricity = Math.Sqrt(squareOfFirstEccentricity);
			Name = name;
		}
	}
}
