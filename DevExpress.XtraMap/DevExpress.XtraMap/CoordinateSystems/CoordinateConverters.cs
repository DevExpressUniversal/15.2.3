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
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public class IncorrectUtmZoneException : Exception { }
	public abstract class CoordinateConverterBase : ICoordPointConverter {
		protected internal abstract CoordPointType DesinationPointType { get; }
		public abstract CoordPoint Convert(CoordPoint sourcePoint);
	}
	public abstract class CoordinateConverterCoreWrapper : CoordinateConverterBase {
		CoordinateConverterCore converterCore;
		protected CoordinateConverterCore ConverterCore { 
			get { 
				if (converterCore == null)
					this.converterCore = CreateCoreConverter();			
				return converterCore; 
			} 
		}
		protected CoordinateConverterCoreWrapper() {
		}
		internal static ICoordPointConverter CreateConverterByInnerConverter(CoordinateConverterCore coreConverter) {
			TransverseMercatorCartesianToGeoConverterCore tmConverter = coreConverter as TransverseMercatorCartesianToGeoConverterCore;
			if (tmConverter != null)
				return new TransverseMercatorCartesianToGeoConverter(tmConverter);
			LambertConformalConicCartesianToGeoConverterCore lccConverter = coreConverter as LambertConformalConicCartesianToGeoConverterCore;
			if (lccConverter != null)
				return new LambertConformalConicCartesianToGeoConverter(lccConverter);
			AlbersCartesianToGeoConverterCore aConverter = coreConverter as AlbersCartesianToGeoConverterCore;
			if(aConverter != null)
				return new AlbersEqualAreaConticCartesianToGeoConverter(aConverter);
			MercatorAuxiliarySphereCartesianToGeoConverterCore masConverter = coreConverter as MercatorAuxiliarySphereCartesianToGeoConverterCore;
			if(masConverter != null)
				return new MercatorAuxiliarySphereCartesianToGeoConverter(masConverter);
			return coreConverter;
		}
		protected abstract CoordinateConverterCore CreateCoreConverter();
		public override CoordPoint Convert(CoordPoint sourcePoint) {
			if (ConverterCore != null)
				return ConverterCore.Convert(sourcePoint);
			return sourcePoint;
		}
	}
	public abstract class EllipsoidBasedCartesianToGeoConverter : CoordinateConverterCoreWrapper {
		static readonly Ellipsoid DefaultEllipsoid = Ellipsoid.WGS84;
		const double DefaultFalseEasting = 0.0;
		const double DefaultFalseNorthing = 0.0;
		static GeoPoint DefaultProjectionCenter { get { return new GeoPoint(0.0, 0.0); } }
		protected internal override CoordPointType DesinationPointType { get { return CoordPointType.Geo; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("EllipsoidBasedCartesianToGeoConverterEllipsoid")]
#endif
		public Ellipsoid Ellipsoid { get; set; }
		void ResetEllipsoid() { Ellipsoid = DefaultEllipsoid; }
		bool ShouldSerializeEllipsoid() { return Ellipsoid != DefaultEllipsoid; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("EllipsoidBasedCartesianToGeoConverterFalseEasting"),
#endif
		DefaultValue(DefaultFalseEasting)]
		public double FalseEasting { get; set; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("EllipsoidBasedCartesianToGeoConverterFalseNorthing"),
#endif
		DefaultValue(DefaultFalseNorthing)]
		public double FalseNorthing { get; set; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("EllipsoidBasedCartesianToGeoConverterProjectionCenter"),
#endif
		TypeConverter(typeof(ExpandableObjectConverter))]
		public GeoPoint ProjectionCenter { get; set; }
		void ResetProjectionCenter() { ProjectionCenter = DefaultProjectionCenter; }
		bool ShouldSerializeProjectionCenter() { return ProjectionCenter != DefaultProjectionCenter; }
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
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("TransverseMercatorCartesianToGeoConverterScaleFactor"),
#endif
		DefaultValue(DefaultScaleFactor)]
		public double ScaleFactor { get; set; }
		internal TransverseMercatorCartesianToGeoConverter(TransverseMercatorCartesianToGeoConverterCore converter)
			: this(new Ellipsoid(converter.Ellipsoid), converter.FalseEasting, converter.FalseNorthing, new GeoPoint(converter.ProjectionCenter.GetY(), converter.ProjectionCenter.GetX())) {
		}
		public TransverseMercatorCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter, double scaleFactor)
			: base(ellipsoid, falseEasting, falseNorthing, projectionCenter) {
			ScaleFactor = scaleFactor;
		}
		public TransverseMercatorCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter)
			: this(ellipsoid, falseEasting, falseNorthing, projectionCenter, DefaultScaleFactor) {
		}
		public TransverseMercatorCartesianToGeoConverter() {
			ScaleFactor = DefaultScaleFactor;
		}
		protected override CoordinateConverterCore CreateCoreConverter() {
			return new TransverseMercatorCartesianToGeoConverterCore(GeoPointFactory.Instance, Ellipsoid.EllipsoidCore, FalseEasting, FalseNorthing, ProjectionCenter, ScaleFactor);
		}
	}
	public class UTMCartesianToGeoConverter : CoordinateConverterCoreWrapper {
		const int DefaultUtmZone = 1;
		const Hemisphere DefaultHemisphere = Hemisphere.Northern;
		int utmZone;
		Hemisphere hemisphere;
		protected internal override CoordPointType DesinationPointType { get { return CoordPointType.Geo; } }
		public double ScaleFactor { get; private set; }
		public Ellipsoid Ellipsoid { get; private set; }
		public double FalseEasting { get; private set; }
		public double FalseNorthing { get; private set; }
		public GeoPoint ProjectionCenter { get; private set; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("UTMCartesianToGeoConverterUtmZone"),
#endif
		DefaultValue(DefaultUtmZone)]
		public int UtmZone {
			get { return utmZone; }
			set {
				if (value < 1 || value > 60)
					throw new IncorrectUtmZoneException();
				utmZone = value;
				RecalculateTransverceMercatorParameters();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("UTMCartesianToGeoConverterHemisphere"),
#endif
		DefaultValue(DefaultHemisphere)]
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
	}
	public abstract class ConicCartesianToGeoConverter : EllipsoidBasedCartesianToGeoConverter {
		const double DefaultStandardParallelN = 10.0;
		const double DefaultStandardParallelS = 0.0;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ConicCartesianToGeoConverterStandardParallelN"),
#endif
		DefaultValue(DefaultStandardParallelN)]
		public double StandardParallelN { get; set; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ConicCartesianToGeoConverterStandardParallelS"),
#endif
		DefaultValue(DefaultStandardParallelS)]
		public double StandardParallelS { get; set; }
		protected ConicCartesianToGeoConverter() {
			StandardParallelN = DefaultStandardParallelN;
			StandardParallelS = DefaultStandardParallelS;
		}
		protected ConicCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter, double standardParallelN, double standardParallelS)
			: base(ellipsoid, falseEasting, falseNorthing, projectionCenter) {
			StandardParallelN = standardParallelN;
			StandardParallelS = standardParallelS;
		}
	}
	public class LambertConformalConicCartesianToGeoConverter : ConicCartesianToGeoConverter {
		internal LambertConformalConicCartesianToGeoConverter(LambertConformalConicCartesianToGeoConverterCore converter)
			: this(new Ellipsoid(converter.Ellipsoid), converter.FalseEasting, converter.FalseNorthing, new GeoPoint(converter.ProjectionCenter.GetY(), converter.ProjectionCenter.GetX()), converter.StdParallelN, converter.StdParallelS) {
		}
		public LambertConformalConicCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter, double standardParallelN, double standardParallelS)
			: base(ellipsoid, falseEasting, falseNorthing, projectionCenter, standardParallelN, standardParallelS) {
		}
		public LambertConformalConicCartesianToGeoConverter() { }
		protected override CoordinateConverterCore CreateCoreConverter() {
			return new LambertConformalConicCartesianToGeoConverterCore(GeoPointFactory.Instance, Ellipsoid.EllipsoidCore, FalseEasting, FalseNorthing, ProjectionCenter, StandardParallelN, StandardParallelS);
		}
	}
	public class AlbersEqualAreaConticCartesianToGeoConverter : ConicCartesianToGeoConverter {
		internal AlbersEqualAreaConticCartesianToGeoConverter(AlbersCartesianToGeoConverterCore converter)
			: this(new Ellipsoid(converter.Ellipsoid), converter.FalseEasting, converter.FalseNorthing, new GeoPoint(converter.ProjectionCenter.GetY(), converter.ProjectionCenter.GetX()), converter.StdParallelN, converter.StdParallelS) {
		}
		public AlbersEqualAreaConticCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter, double standardParallelN, double standardParallelS)
			: base(ellipsoid, falseEasting, falseNorthing, projectionCenter, standardParallelN, standardParallelS) {
		}
		public AlbersEqualAreaConticCartesianToGeoConverter() { }
		protected override CoordinateConverterCore CreateCoreConverter() {
			return new AlbersCartesianToGeoConverterCore(GeoPointFactory.Instance, Ellipsoid.EllipsoidCore, FalseEasting, FalseNorthing, ProjectionCenter, StandardParallelN, StandardParallelS);
		}
	}
	public class MercatorAuxiliarySphereCartesianToGeoConverter : EllipsoidBasedCartesianToGeoConverter {
		const int DefaultSphereType = 0;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MercatorAuxiliarySphereCartesianToGeoConverterSphereType"),
#endif
		DefaultValue(DefaultSphereType)]
		public int SphereType { get; set; }
		internal MercatorAuxiliarySphereCartesianToGeoConverter(MercatorAuxiliarySphereCartesianToGeoConverterCore converter)
			: this(new Ellipsoid(converter.Ellipsoid), converter.FalseEasting, converter.FalseNorthing, new GeoPoint(converter.ProjectionCenter.GetY(), converter.ProjectionCenter.GetX()), converter.SphereType) {
		}
		public MercatorAuxiliarySphereCartesianToGeoConverter(Ellipsoid ellipsoid, double falseEasting, double falseNorthing, GeoPoint projectionCenter, int sphereType)
			: base(ellipsoid, falseEasting, falseNorthing, projectionCenter) {
			SphereType = sphereType;
		}
		public MercatorAuxiliarySphereCartesianToGeoConverter() {
			SphereType = DefaultSphereType;
		}
		protected override CoordinateConverterCore CreateCoreConverter() {
			return new MercatorAuxiliarySphereCartesianToGeoConverterCore(GeoPointFactory.Instance, Ellipsoid.EllipsoidCore, FalseEasting, FalseNorthing, ProjectionCenter, SphereType);
		}
	}
	public enum Hemisphere {
		Northern = 0,
		Southern = 1
	}
}
