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
using DevExpress.Map.Localization;
using DevExpress.Compatibility.System;
namespace DevExpress.Map {
	[Serializable]
	public class MeasureUnitCore {
		const double MeterPerInch = 0.0254;
		const double MeterPerFeet = MeterPerInch * 12.0;
		const double MeterPerYards = MeterPerFeet * 3.0;
		const double MeterPerFurlongs = MeterPerYards * 220.0;
		const double MeterPerMile = MeterPerFeet * 5280.0;
		const double MeterPerNauticalMiles = 1852.0;
		const double MeterPerCentimeter = 0.01;
		const double MeterPerKilometer = 1000;
		static readonly MeasureUnitCore kilometer, meter, centimeter, millimeter;
		static readonly MeasureUnitCore mile, furlong, yard, foot, inch, nauticalMile;
		public static MeasureUnitCore Kilometer	{ get { return kilometer; } }
		public static MeasureUnitCore Meter		{ get { return meter; } }
		public static MeasureUnitCore Centimeter   { get { return centimeter; } }
		public static MeasureUnitCore Millimeter   { get { return millimeter; } }
		public static MeasureUnitCore Mile		 { get { return mile; } }
		public static MeasureUnitCore Furlong	  { get { return furlong; } }
		public static MeasureUnitCore Yard		 { get { return yard; } }
		public static MeasureUnitCore Foot		 { get { return foot; } }
		public static MeasureUnitCore Inch		 { get { return inch; } }
		public static MeasureUnitCore NauticalMile { get { return nauticalMile; } }
		static MeasureUnitCore() {
			string kilometrStr = MapLocalizer.GetString(MapStringId.Kilometer);
			string kmStr = MapLocalizer.GetString(MapStringId.KilpmeterAbbr);
			kilometer = new MeasureUnitCore(MeterPerKilometer, kilometrStr, kmStr);
			string meterStr = MapLocalizer.GetString(MapStringId.Meter);
			string mStr = MapLocalizer.GetString(MapStringId.MeterAbbr);
			meter = new MeasureUnitCore(1.0, meterStr, mStr);
			string centimeterStr = MapLocalizer.GetString(MapStringId.Centimeter);
			string cmStr = MapLocalizer.GetString(MapStringId.CentimeterAbbr);
			centimeter = new MeasureUnitCore(MeterPerCentimeter, centimeterStr, cmStr);
			string millimeterStr = MapLocalizer.GetString(MapStringId.Millimeter);
			string mmStr = MapLocalizer.GetString(MapStringId.MillimeterAbbr);
			millimeter = new MeasureUnitCore(0.001, millimeterStr, mmStr);
			string mileStr = MapLocalizer.GetString(MapStringId.Mile);
			string mlStr = MapLocalizer.GetString(MapStringId.MileAbbr);
			mile = new MeasureUnitCore(MeterPerMile, mileStr, mlStr);
			string furlongStr = MapLocalizer.GetString(MapStringId.Furlong);
			string furStr = MapLocalizer.GetString(MapStringId.FurlongAbbr);
			furlong = new MeasureUnitCore(MeterPerFurlongs, furlongStr, furStr);
			string yardStr = MapLocalizer.GetString(MapStringId.Yard);
			string ydStr = MapLocalizer.GetString(MapStringId.YardAbbr);
			yard = new MeasureUnitCore(MeterPerYards, yardStr, ydStr);
			string footStr = MapLocalizer.GetString(MapStringId.Foot);
			string ftStr = MapLocalizer.GetString(MapStringId.FootAbbr);
			foot  = new MeasureUnitCore(MeterPerFeet, footStr, ftStr);
			string inchStr = MapLocalizer.GetString(MapStringId.Inch);
			string inStr = MapLocalizer.GetString(MapStringId.InchAbbr);
			inch = new MeasureUnitCore(MeterPerInch, inchStr, inStr);
			string nauticalMileStr = MapLocalizer.GetString(MapStringId.NauticalMile);
			string NMStr = MapLocalizer.GetString(MapStringId.NauticalMileAbbr);
			nauticalMile = new MeasureUnitCore(MeterPerNauticalMiles, nauticalMileStr, NMStr);
		}
		readonly double metersInUnit;
		readonly string name;
		readonly string abbreviation;
		public double MetersInUnit { get { return metersInUnit; } }
		public string Name		 { get { return name; } }
		public string Abbreviation { get { return abbreviation; } }
		public MeasureUnitCore(double metersInUnit, string name, string abbreviation) {
			this.metersInUnit = metersInUnit;
			this.name = name;
			this.abbreviation = abbreviation;
		}
		public double ToMeters(double value) {
			return value * MetersInUnit;
		}
		public double FromMeters(double value) {
			return value / MetersInUnit;
		}
	}
}
