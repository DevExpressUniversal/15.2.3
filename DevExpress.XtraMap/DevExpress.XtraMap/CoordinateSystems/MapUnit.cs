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
using System.Globalization;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap {
	public struct MapUnit : IMapUnit {
		public static bool operator ==(MapUnit unit1, MapUnit unit2) {
			return unit1.x == unit2.x && unit1.y == unit2.y;
		}
		public static bool operator !=(MapUnit unit1, MapUnit unit2) {
			return !(unit1 == unit2);
		}
		public static bool Equals(MapUnit unit1, MapUnit unit2) {
			return unit1.x.Equals(unit2.x) && unit1.y.Equals(unit2.y);
		}
		public static MapUnit Normalize(MapUnit mapUnit) {
			return new MapUnit(Math.Max(0, Math.Min(mapUnit.x, 1.0)), Math.Max(0, Math.Min(mapUnit.y, 1.0)));
		}
		public static MapUnit operator *(MapUnit mapUnit, double v) {
			return new MapUnit(mapUnit.x * v, mapUnit.y * v);
		}
		public static MapUnit operator /(MapUnit mapUnit, double v) {
			return new MapUnit(mapUnit.x / v, mapUnit.y / v);
		}
		double x;
		double y;
		public double X {
			get { return x; }
			set { x = value; }
		}
		public double Y {
			get { return y; }
			set { y = value; }
		}
		internal MapUnit(IMapUnit mapUnit) : this(mapUnit.X, mapUnit.Y) { }
		public MapUnit(double x, double y) {
			this.x = x;
			this.y = y;
		}
		public override bool Equals(object o) {
			if (o != null && o is MapUnit)
				return MapUnit.Equals(this, (MapUnit)o);
			return false;
		}
		public override int GetHashCode() {
			return x.GetHashCode() ^ y.GetHashCode();
		}
		public override string ToString() {
			return this.ToString(CultureInfo.CurrentCulture);
		}
		public string ToString(IFormatProvider provider) {
			NumberFormatInfo formatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			string listSeparator = (formatInfo == null || formatInfo.NumberDecimalSeparator != ",") ? "," : ";";
			return this.x.ToString(provider) + listSeparator + this.y.ToString(provider);
		}
	}
}
