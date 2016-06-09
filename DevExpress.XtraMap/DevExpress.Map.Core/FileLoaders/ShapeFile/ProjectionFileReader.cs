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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace DevExpress.Map.Native {
	public class InconsistentPrjException : Exception {
	}
	public class UnrecognizedConverterException : Exception {
	}
	public class ProjectionFileParser {
		CoordObjectFactory geoPointfactory;
		CoordObjectFactory cartesianPointfactory;
		protected CoordObjectFactory GeoPointFactory { get { return geoPointfactory; } }
		protected CoordObjectFactory CartesianPointfactory { get { return cartesianPointfactory; } }
		public ProjectionFileParser(CoordObjectFactory geoObjectFactory, CoordObjectFactory cartesianObjectFactory) {
			this.geoPointfactory = geoObjectFactory;
			this.cartesianPointfactory = cartesianObjectFactory;
		}
		public virtual CoordSystemCore Parse(string content) {
			string csRegex = @"(?<cs>(PROJCS|GEOCS)?)\[(?<content>(.)*?)\][^\[\]]*$";
			Regex regex = new Regex(csRegex, RegexOptions.IgnoreCase);
			Match match = regex.Match(content);
			if(!match.Success)
				throw new InconsistentPrjException();
			if(string.Equals("PROJCS", match.Groups["cs"].Value))
				return CreateCartesianCS(match.Groups["content"].Value);
			return CreateGeoCS(match.Groups["content"].Value);
		}
		protected internal virtual EllipsoidCore ParseEllipsoid(string content) {
			string ellipsoidRegex = @"SPHEROID\[""(?<name>.*?)"",(?<axis>[+-]?(\d+|(\d+[\.,]\d+))?),(?<flattening>[+-]?(\d+|(\d+[\.,]\d+)?))\]";
			Regex regex = new Regex(ellipsoidRegex, RegexOptions.IgnoreCase);
			Match match = regex.Match(content);
			if(!match.Success)
				return EllipsoidCore.WGS84;
			double majorAxis = Convert.ToDouble(match.Groups["axis"].Value, CultureInfo.InvariantCulture);
			double invFlattening = Convert.ToDouble(match.Groups["flattening"].Value, CultureInfo.InvariantCulture);
			return EllipsoidCore.CreateBySemimajorAndInverseFlattering(majorAxis, invFlattening, null);
		}
		protected internal virtual double ParseParameter(string content, string parameter) {
			string parameterRegex = @"PARAMETER\[""" + parameter + @""",(?<value>[+-]?(\d+|(\d+[\.,]\d+))?)\]";
			Regex regex = new Regex(parameterRegex, RegexOptions.IgnoreCase);
			Match match = regex.Match(content);
			return match.Success ? Convert.ToDouble(match.Groups["value"].Value, CultureInfo.InvariantCulture) : 0.0;
		}
		protected internal virtual MeasureUnitCore ParseMeasureUnit(string content) {
			string MeasureUnitCoreRegex = @"UNIT\[""(?<name>.*?)"",(?<value>[+-]?(\d+|(\d+[\.,]\d+))?)\]";
			Regex regex = new Regex(MeasureUnitCoreRegex, RegexOptions.IgnoreCase);
			Match match = regex.Match(content);
			if(!match.Success)
				return MeasureUnitCore.Meter;
			double value = Convert.ToDouble(match.Groups["value"].Value, CultureInfo.InvariantCulture);
			string name = match.Groups["name"].Value;
			string abbr = name.Length > 0 ? new string(name[0], 1).ToLower() : "";
			return new MeasureUnitCore(value, name, abbr);
		}
		protected virtual string ParseProjection(string content) {
			string projectionRegex = @"PROJECTION\[""(?<name>.*?)""\]";
			Regex regex = new Regex(projectionRegex, RegexOptions.IgnoreCase);
			Match match = regex.Match(content);
			return match.Success ? match.Groups["name"].Value : string.Empty;
		}
		protected virtual CartesianCoordSystemCore CreateCartesianCS(string content) {
			string geoCSRegex = @"GEOGCS\[(?>\[(?<DEPTH>)|\](?<-DEPTH>)|.)*\](?(DEPTH)(?!))";
			Regex regex = new Regex(geoCSRegex, RegexOptions.IgnoreCase);
			Match match = regex.Match(content);
			if(!match.Success)
				throw new InconsistentPrjException();
			string geoCSDesc = match.Value;
			EllipsoidCore ellipsoid = ParseEllipsoid(geoCSDesc);
			string projCS = regex.Replace(content, "");
			MeasureUnitCore unit = ParseMeasureUnit(projCS);
			CoordinateConverterCore converter = ParseCoordinateConverter(projCS, ellipsoid, unit);
			return new CartesianCoordSystemCore(CartesianPointfactory) { CoordinateConverter = converter, MeasureUnit = unit };
		}
		internal CoordinateConverterCore ParseCoordinateConverter(string projCS, EllipsoidCore ellipsoid, MeasureUnitCore unit) {
			CoordinateConverterCore converter = null;
			string projection = ParseProjection(projCS);
			if(string.Equals(projection, "Transverse_Mercator") || string.Equals(projection, "Gauss_Kruger"))
				return PopulateTransverseMercatorConverter(projCS, ellipsoid, unit);
			if(string.Equals(projection, "Lambert_Conformal_Conic"))
				return PopulateLambertConformalConicConverter(projCS, ellipsoid, unit);
			if(string.Equals(projection, "Albers"))
				return PopulateAlbersConverter(projCS, ellipsoid, unit);
			if(string.Equals(projection, "Mercator_Auxiliary_Sphere"))
				return PopulateMercatorAuxiliarySphereConverter(projCS, ellipsoid, unit);
			return converter;
		}
		CoordinateConverterCore PopulateAlbersConverter(string projCS, EllipsoidCore ellipsoid, MeasureUnitCore unit) {
			double falseEasting = unit.ToMeters(ParseParameter(projCS, "False_Easting"));
			double falseNorthing = unit.ToMeters(ParseParameter(projCS, "False_Northing"));
			double centralMeridian = ParseParameter(projCS, "Central_Meridian");
			double latitudeOfOrigin = ParseParameter(projCS, "Latitude_Of_Origin");
			double stdParallelSouth = ParseParameter(projCS, "Standard_Parallel_1");
			double stdParallelNorth = ParseParameter(projCS, "Standard_Parallel_2");
			CoordPoint point = GeoPointFactory.CreatePoint(centralMeridian, latitudeOfOrigin);
			return new AlbersCartesianToGeoConverterCore(GeoPointFactory, ellipsoid, falseEasting, falseNorthing, point, stdParallelNorth, stdParallelSouth);
		}
		CoordinateConverterCore PopulateLambertConformalConicConverter(string projCS, EllipsoidCore ellipsoid, MeasureUnitCore unit) {
			double falseEasting = unit.ToMeters(ParseParameter(projCS, "False_Easting"));
			double falseNorthing = unit.ToMeters(ParseParameter(projCS, "False_Northing"));
			double centralMeridian = ParseParameter(projCS, "Central_Meridian");
			double latitudeOfOrigin = ParseParameter(projCS, "Latitude_Of_Origin");
			double stdParallelSouth = ParseParameter(projCS, "Standard_Parallel_1");
			double stdParallelNorth = ParseParameter(projCS, "Standard_Parallel_2");
			CoordPoint point = GeoPointFactory.CreatePoint(centralMeridian, latitudeOfOrigin);
			return new LambertConformalConicCartesianToGeoConverterCore(GeoPointFactory, ellipsoid, falseEasting, falseNorthing, point, stdParallelNorth, stdParallelSouth);
		}
		CoordinateConverterCore PopulateTransverseMercatorConverter(string projCS, EllipsoidCore ellipsoid, MeasureUnitCore unit) {
			double falseEasting = unit.ToMeters(ParseParameter(projCS, "False_Easting"));
			double falseNorthing = unit.ToMeters(ParseParameter(projCS, "False_Northing"));
			double centralMeridian = ParseParameter(projCS, "Central_Meridian");
			double latitudeOfOrigin = ParseParameter(projCS, "Latitude_Of_Origin");
			double scaleFactor = ParseParameter(projCS, "Scale_Factor");
			CoordPoint point = GeoPointFactory.CreatePoint(centralMeridian, latitudeOfOrigin);
			return new TransverseMercatorCartesianToGeoConverterCore(GeoPointFactory, ellipsoid, falseEasting, falseNorthing, point, scaleFactor);
		}
		CoordinateConverterCore PopulateMercatorAuxiliarySphereConverter(string projCS, EllipsoidCore ellipsoid, MeasureUnitCore unit) {
			double falseEasting = unit.ToMeters(ParseParameter(projCS, "False_Easting"));
			double falseNorthing = unit.ToMeters(ParseParameter(projCS, "False_Northing"));
			double centralMeridian = ParseParameter(projCS, "Central_Meridian");
			double stdParallel1 = ParseParameter(projCS, "Standard_Parallel_1");
			int sphereType = (int)ParseParameter(projCS, "Auxiliary_Sphere_Type");
			CoordPoint point = GeoPointFactory.CreatePoint(centralMeridian, stdParallel1);
			return new MercatorAuxiliarySphereCartesianToGeoConverterCore(GeoPointFactory, ellipsoid, falseEasting, falseNorthing, point, sphereType);
		}
		GeoCoordSystemCore CreateGeoCS(string content) {
			return new GeoCoordSystemCore(GeoPointFactory);
		}
	}
}
