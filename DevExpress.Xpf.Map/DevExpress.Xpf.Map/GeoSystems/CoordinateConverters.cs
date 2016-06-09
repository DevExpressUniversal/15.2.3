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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Map {
	public abstract class CoordinateConverterBase : MapDependencyObject, ICoordPointConverter {
		protected internal abstract CoordPointType DesinationPointType { get; }
		public abstract CoordPoint Convert(CoordPoint sourcePoint);
	}
	public abstract class CoordinateConverterCoreWrrapper : CoordinateConverterBase {
		CoordinateConverterCore converterCore;
		protected CoordinateConverterCore ConverterCore {
			get {
				if (converterCore == null)
					this.converterCore = CreateCoreConverter();
				return converterCore;
			}
		}
		protected CoordinateConverterCoreWrrapper() {
		}
		protected abstract CoordinateConverterCore CreateCoreConverter();
		public override CoordPoint Convert(CoordPoint sourcePoint) {
			if (ConverterCore != null)
				return ConverterCore.Convert(sourcePoint);
			return sourcePoint;
		}
	}
	public abstract class EllipsoidBasedCartesianToGeoConverter : CoordinateConverterCoreWrrapper {
		static readonly Ellipsoid DefaultEllipsoid = Ellipsoid.WGS84;
		const double DefaultFalseEasting = 0.0;
		const double DefaultFalseNorthing = 0.0;
		static GeoPoint DefaultProjectionCenter { get { return new GeoPoint(0.0, 0.0); } }
		protected internal override CoordPointType DesinationPointType { get { return CoordPointType.Geo; } }
		[Category(Categories.Data)]
		public Ellipsoid Ellipsoid { get; set; }
		[Category(Categories.Data)]
		public double FalseEasting { get; set; }
		[Category(Categories.Data)]
		public double FalseNorthing { get; set; }
		[Category(Categories.Data)]
		public GeoPoint ProjectionCenter { get; set; }
		protected EllipsoidBasedCartesianToGeoConverter()
			: this(DefaultEllipsoid, DefaultFalseEasting, DefaultFalseNorthing, DefaultProjectionCenter) {
		}
		protected EllipsoidBasedCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter) {
			Ellipsoid = ellipsoid;
			FalseEasting = falseEasting;
			FalseNorthing = falseNorthing;
			ProjectionCenter = projectionCenter;
		}
	}
	public class TransverseMercatorCartesianToGeoConverter : EllipsoidBasedCartesianToGeoConverter {
		const double DefaultScaleFactor = 1.0;
		[Category(Categories.Data)]
		public double ScaleFactor { get; set; }
		public TransverseMercatorCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter, double scaleFactor)
			: base(ellipsoid, falseEasting, falseNorthing, projectionCenter) {
			ScaleFactor = scaleFactor;
		}
		public TransverseMercatorCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter)
			: this(ellipsoid, falseEasting, falseNorthing, projectionCenter, DefaultScaleFactor) {
		}
		public TransverseMercatorCartesianToGeoConverter()
			: base() {
			ScaleFactor = DefaultScaleFactor;
		}
		protected override CoordinateConverterCore CreateCoreConverter() {
			return new TransverseMercatorCartesianToGeoConverterCore(GeoPointFactory.Instance, Ellipsoid.EllipsoidCore, FalseEasting, FalseNorthing, ProjectionCenter, ScaleFactor);
		}
		protected override MapDependencyObject CreateObject() {
			return new TransverseMercatorCartesianToGeoConverter();
		}
	}
	public class UTMCartesianToGeoConverter : CoordinateConverterCoreWrrapper {
		const int DefaultUtmZone = 1;
		const Hemisphere DefaultHemisphere = Hemisphere.Northern;
		int utmZone;
		Hemisphere hemisphere;
		protected internal override CoordPointType DesinationPointType { get { return CoordPointType.Geo; } }
		[Category(Categories.Data)]
		public double ScaleFactor { get; private set; }
		[Category(Categories.Data)]
		public Ellipsoid Ellipsoid { get; private set; }
		[Category(Categories.Data)]
		public double FalseEasting { get; private set; }
		[Category(Categories.Data)]
		public double FalseNorthing { get; private set; }
		[Category(Categories.Data)]
		public GeoPoint ProjectionCenter { get; private set; }
		[Category(Categories.Data)]
		public int UtmZone {
			get { return utmZone; }
			set {
				if (value < 1 || value > 60)
					throw new Exception(DXMapStrings.MsgIncorrectUtmZone);
				utmZone = value;
				RecalculateTransverceMercatorParameters();
			}
		}
		[Category(Categories.Data)]
		public Hemisphere Hemisphere {
			get { return hemisphere; }
			set {
				hemisphere = value;
				RecalculateTransverceMercatorParameters();
			}
		}
		public UTMCartesianToGeoConverter(int utmZone, Hemisphere hemisphere) {
			UtmZone = utmZone;
			Hemisphere = hemisphere;
			Ellipsoid = Ellipsoid.WGS84;
			FalseEasting = UTMCartesianToGeoConverterCore.UtmFalseEasting;
			ScaleFactor = UTMCartesianToGeoConverterCore.UtmScaleFactor;
			RecalculateTransverceMercatorParameters();
		}
		public UTMCartesianToGeoConverter()
			: this(DefaultUtmZone, DefaultHemisphere) { }
		void RecalculateTransverceMercatorParameters() {
			FalseNorthing = UTMCartesianToGeoConverterCore.GetFalseNorthing((HemisphereCore)Hemisphere);
			ProjectionCenter = (GeoPoint)UTMCartesianToGeoConverterCore.GetProjectionCenter(UtmZone, GeoPointFactory.Instance);
		}
		protected override CoordinateConverterCore CreateCoreConverter() {
			return new TransverseMercatorCartesianToGeoConverterCore(GeoPointFactory.Instance, Ellipsoid.EllipsoidCore, FalseEasting, FalseNorthing, ProjectionCenter, ScaleFactor);
		}
		protected override MapDependencyObject CreateObject() {
			return new UTMCartesianToGeoConverter();
		}
	}
	public abstract class ConicCartesianToGeoConverter : EllipsoidBasedCartesianToGeoConverter {
		const double DefaultStandardParallelN = 10.0;
		const double DefaultStandardParallelS = 0.0;
		[Category(Categories.Data)]
		public double StandardParallelN { get; set; }
		[Category(Categories.Data)]
		public double StandardParallelS { get; set; }
		protected ConicCartesianToGeoConverter()
			: base() {
			StandardParallelN = DefaultStandardParallelN;
			StandardParallelS = DefaultStandardParallelS;
		}
		protected ConicCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter)
			: this(ellipsoid, falseEasting, falseNorthing, projectionCenter, DefaultStandardParallelN, DefaultStandardParallelS) {
		}
		protected ConicCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter, double standardParallelN, double standardParallelS)
			: base(ellipsoid, falseEasting, falseNorthing, projectionCenter) {
			StandardParallelN = standardParallelN;
			StandardParallelS = standardParallelS;
		}
	}
	public class LambertConformalConicCartesianToGeoConverter : ConicCartesianToGeoConverter {
		public LambertConformalConicCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter, double standardParallelN, double standardParallelS)
			: base(ellipsoid, falseEasting, falseNorthing, projectionCenter, standardParallelN, standardParallelS) {
		}
		public LambertConformalConicCartesianToGeoConverter()
			: base() { }
		protected override CoordinateConverterCore CreateCoreConverter() {
			return new LambertConformalConicCartesianToGeoConverterCore(GeoPointFactory.Instance, Ellipsoid.EllipsoidCore, FalseEasting, FalseNorthing, ProjectionCenter, StandardParallelN, StandardParallelS);
		}
		protected override MapDependencyObject CreateObject() {
			return new LambertConformalConicCartesianToGeoConverter();
		}
	}
	public class AlbersEqualAreaConicCartesianToGeoConverter : ConicCartesianToGeoConverter {
		public AlbersEqualAreaConicCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter, double standardParallelN, double standardParallelS)
			: base(ellipsoid, falseEasting, falseNorthing, projectionCenter, standardParallelN, standardParallelS) {
		}
		public AlbersEqualAreaConicCartesianToGeoConverter()
			: base() { }
		protected override CoordinateConverterCore CreateCoreConverter() {
			return new AlbersCartesianToGeoConverterCore(GeoPointFactory.Instance, Ellipsoid.EllipsoidCore, FalseEasting, FalseNorthing, ProjectionCenter, StandardParallelN, StandardParallelS);
		}
		protected override MapDependencyObject CreateObject() {
			return new AlbersEqualAreaConicCartesianToGeoConverter();
		}
	}
	public class MercatorAuxiliarySphereCartesianToGeoConverter : EllipsoidBasedCartesianToGeoConverter {
		const int DefaultSphereType = 0;
		[Category(Categories.Data)]
		public int SphereType { get; set; }
		public MercatorAuxiliarySphereCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter, int sphereType)
			: base(ellipsoid, falseEasting, falseNorthing, projectionCenter) {
			SphereType = sphereType;
		}
		public MercatorAuxiliarySphereCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter)
			: this(ellipsoid, falseEasting, falseNorthing, projectionCenter, DefaultSphereType) {
		}
		public MercatorAuxiliarySphereCartesianToGeoConverter()
			: base() {
			SphereType = DefaultSphereType;
		}
		protected override CoordinateConverterCore CreateCoreConverter() {
			return new MercatorAuxiliarySphereCartesianToGeoConverterCore(GeoPointFactory.Instance, Ellipsoid.EllipsoidCore, FalseEasting, FalseNorthing, ProjectionCenter, SphereType);
		}
		protected override MapDependencyObject CreateObject() {
			return new MercatorAuxiliarySphereCartesianToGeoConverter();
		}
	}
	public class IdentityCartesianToGeoConverter : CoordinateConverterBase {
		protected internal override CoordPointType DesinationPointType { get { return CoordPointType.Cartesian; } }
		public override CoordPoint Convert(CoordPoint sourcePoint) {
			return new GeoPoint(Math.Max(-90, Math.Min(90, sourcePoint.GetY())), sourcePoint.GetX());
		}
		protected override MapDependencyObject CreateObject() {
			return new IdentityCartesianToGeoConverter();
		}
	}
	public enum Hemisphere {
		Northern = 0,
		Southern = 1
	}
}
