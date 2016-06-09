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

using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap {
	[Editor("DevExpress.XtraMap.Design.EllipsoidEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))]
	public class Ellipsoid {
		#region Predefined Ellipsoids
		static Ellipsoid krassovsky = new Ellipsoid(EllipsoidCore.Krassovsky);
		static Ellipsoid wgs66 = new Ellipsoid(EllipsoidCore.WGS66);
		static Ellipsoid wgs72 = new Ellipsoid(EllipsoidCore.WGS72);
		static Ellipsoid wgs84 = new Ellipsoid(EllipsoidCore.WGS84); 
		static Ellipsoid grs67 = new Ellipsoid(EllipsoidCore.GRS67);
		static Ellipsoid grs80 = new Ellipsoid(EllipsoidCore.GRS80);
		static Ellipsoid australianNational66 = new Ellipsoid(EllipsoidCore.AustralianNational66);
		static Ellipsoid newInternational67 = new Ellipsoid(EllipsoidCore.NewInternational67);
		static Ellipsoid southAmerican69 = new Ellipsoid(EllipsoidCore.SouthAmerican69);
		static Ellipsoid iers89 = new Ellipsoid(EllipsoidCore.IERS89);
		static Ellipsoid iers03 = new Ellipsoid(EllipsoidCore.IERS03);
		static Ellipsoid pz90 = new Ellipsoid(EllipsoidCore.PZ90);	
		static Ellipsoid gsk11 = new Ellipsoid(EllipsoidCore.GSK11); 
		public static Ellipsoid Krassovsky { get { return krassovsky; } }
		public static Ellipsoid WGS66 { get { return wgs66; } }
		public static Ellipsoid WGS72 { get { return wgs72; } }
		public static Ellipsoid WGS84 { get { return wgs84; } }
		public static Ellipsoid GRS67 { get { return grs67; } }
		public static Ellipsoid GRS80 { get { return grs80; } }
		public static Ellipsoid AustralianNational66 { get { return australianNational66; } }
		public static Ellipsoid NewInternational67 { get { return newInternational67; } }
		public static Ellipsoid SouthAmerican69 { get { return southAmerican69; } }
		public static Ellipsoid IERS89 { get { return iers89; } }
		public static Ellipsoid IERS03 { get { return iers03; } }
		public static Ellipsoid PZ90 { get { return pz90; } }
		public static Ellipsoid GSK11 { get { return gsk11; } }
		public static Ellipsoid[] GetPredefinedEllipsoids() {
			Ellipsoid[] predefined = new Ellipsoid[13];
			predefined[0] = krassovsky;
			predefined[1] = wgs66;
			predefined[2] = wgs72;
			predefined[3] = wgs84;
			predefined[4] = grs67;
			predefined[5] = grs80;
			predefined[6] = australianNational66;
			predefined[7] = newInternational67;
			predefined[8] = southAmerican69;
			predefined[9] = iers89;
			predefined[10] = iers03;
			predefined[11] = pz90;
			predefined[12] = gsk11;
			return predefined;
		}
		#endregion
		public static Ellipsoid CreateByTwoAxes(double semimajor, double semiminor, string name) {
			Ellipsoid ellipsoid = new Ellipsoid();
			ellipsoid.EllipsoidCore = EllipsoidCore.CreateByTwoAxes(semimajor, semiminor, name);
			return ellipsoid;
		}
		public static Ellipsoid CreateByTwoAxes(double semimajor, double semiminor) {
			return CreateByTwoAxes(semimajor, semiminor, null);
		}
		public static Ellipsoid CreateBySemimajorAndInverseFlattering(double semimajor, double inverseFlattering, string name) {
			Ellipsoid ellipsoid = new Ellipsoid();
			ellipsoid.EllipsoidCore = EllipsoidCore.CreateBySemimajorAndInverseFlattering(semimajor, inverseFlattering, name);
			return ellipsoid;
		}
		public static Ellipsoid CreateBySemimajorAndInverseFlattering(double semimajor, double inverseFlattering) {
			return CreateBySemimajorAndInverseFlattering(semimajor, inverseFlattering, null);
		}
		protected internal EllipsoidCore EllipsoidCore { get; private set; }
		public double SemimajorAxis { get { return EllipsoidCore.SemimajorAxis; } }
		public double SemiminorAxis { get { return EllipsoidCore.SemiminorAxis; } }
		public double Flattening { get { return EllipsoidCore.Flattening; } }
		public double InverseFlattening { get { return 1.0 / EllipsoidCore.Flattening; } }
		public double SquareOfEccentricity { get { return EllipsoidCore.SquareOfEccentricity; } }
		public string Name { get { return EllipsoidCore.Name; } }
		Ellipsoid() { }
		internal Ellipsoid(EllipsoidCore ellipsoidCore) {
			EllipsoidCore = ellipsoidCore;
		}
		public override string ToString() {
			if (!string.IsNullOrWhiteSpace(Name))
				return Name;
			else
				return "(Ellipsoid: a=" + SemimajorAxis + ", b=" + SemiminorAxis + ")";
		}
	}
}
