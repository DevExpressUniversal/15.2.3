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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpf.Map.Native;
using DevExpress.Map;
namespace DevExpress.Xpf.Map {
	public class MeasureUnit {
		public static List<MeasureUnit> GetPredefinedUnits() {
			var predefined = new List<MeasureUnit>(10);
			predefined.Add(Millimeter);
			predefined.Add(Centimeter);
			predefined.Add(Meter);
			predefined.Add(Kilometer);
			predefined.Add(Inch);
			predefined.Add(Foot);
			predefined.Add(Yard);
			predefined.Add(Furlong);
			predefined.Add(Mile);
			predefined.Add(NauticalMile);
			return predefined;
		}
		public static bool operator ==(MeasureUnit a, MeasureUnit b) {
			if (System.Object.ReferenceEquals(a, b))
				return true;
			if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
				return false;
			return a.MetersInUnit == b.MetersInUnit && a.Name == b.Name && a.Abbreviation == b.Abbreviation;
		}
		public static bool operator !=(MeasureUnit a, MeasureUnit b) {
			return !(a == b);
		}
		public static MeasureUnit Kilometer { get { return MeasureUnitCore.Kilometer; } }
		public static MeasureUnit Meter { get { return MeasureUnitCore.Meter; } }
		public static MeasureUnit Centimeter { get { return MeasureUnitCore.Centimeter; } }
		public static MeasureUnit Millimeter { get { return MeasureUnitCore.Millimeter; } }
		public static MeasureUnit Mile { get { return MeasureUnitCore.Mile; } }
		public static MeasureUnit Furlong { get { return MeasureUnitCore.Furlong; } }
		public static MeasureUnit Yard { get { return MeasureUnitCore.Yard; } }
		public static MeasureUnit Foot { get { return MeasureUnitCore.Foot; } }
		public static MeasureUnit Inch { get { return MeasureUnitCore.Inch; } }
		public static MeasureUnit NauticalMile { get { return MeasureUnitCore.NauticalMile; } }
		public static implicit operator MeasureUnit(MeasureUnitCore unitCore) {
			if (unitCore == null)
				return null;
			return new MeasureUnit(unitCore.MetersInUnit, unitCore.Name, unitCore.Abbreviation);
		}
		public static implicit operator MeasureUnitCore(MeasureUnit unit) {
			return unit == null ? null : unit.measureUnitCore;
		}
		MeasureUnitCore measureUnitCore;
		[Category(Categories.Data)]
		public double MetersInUnit {
			get { return measureUnitCore.MetersInUnit; }
			set { RecreateMeasureUnitCore(value, Name, Abbreviation); }
		}
		[Category(Categories.Data)]
		public string Name {
			get { return measureUnitCore.Name; }
			set { RecreateMeasureUnitCore(MetersInUnit, value, Abbreviation); }
		}
		[Category(Categories.Data)]
		public string Abbreviation {
			get { return measureUnitCore.Abbreviation; }
			set { RecreateMeasureUnitCore(MetersInUnit, Name, value); }
		}
		MeasureUnit(MeasureUnitCore measureUnitCore) {
			this.measureUnitCore = measureUnitCore;
		}
		public MeasureUnit()
			: this(MeasureUnitCore.Meter) {
		}
		public MeasureUnit(double metersInUnit, string name, string abbreviation)
			: this(new MeasureUnitCore(metersInUnit, name, abbreviation)) {
		}
		void RecreateMeasureUnitCore(double metersInUnit, string name, string abbreviation) {
			this.measureUnitCore = new MeasureUnitCore(metersInUnit, name, abbreviation);
		}
		public override string ToString() {
			return Name;
		}
		public override bool Equals(object obj) {
			if (obj == null)
				return false;
			var unit = obj as MeasureUnit;
			return Equals(unit);
		}
		public bool Equals(MeasureUnit unit) {
			if (object.ReferenceEquals(unit, null))
				return false;
			return MetersInUnit == unit.MetersInUnit && Name == unit.Name && Abbreviation == unit.Abbreviation;
		}
		public override int GetHashCode() {
			return MetersInUnit.GetHashCode() ^ Name.GetHashCode() ^ Abbreviation.GetHashCode();
		}
		public double ToMeters(double value) {
			return measureUnitCore.ToMeters(value);
		}
		public double FromMeters(double value) {
			return measureUnitCore.FromMeters(value);
		}
	}
}
