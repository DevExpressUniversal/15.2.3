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
using System.Runtime.InteropServices;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	[Serializable]
	public struct DXWebUnit {
		public static readonly DXWebUnit Empty;
		static DXWebUnit() {
			Empty = new DXWebUnit();
		}
		public static bool operator ==(DXWebUnit left, DXWebUnit right) {
			return left.type == right.type && left.value == right.value;
		}
		public static bool operator !=(DXWebUnit left, DXWebUnit right) {
			if(left.type == right.type)
				return (left.value != right.value);
			return true;
		}
		private static string GetStringFromType(DXWebUnitType type) {
			switch(type) {
				case DXWebUnitType.Pixel:
					return "px";
				case DXWebUnitType.Point:
					return "pt";
				case DXWebUnitType.Pica:
					return "pc";
				case DXWebUnitType.Inch:
					return "in";
				case DXWebUnitType.Mm:
					return "mm";
				case DXWebUnitType.Cm:
					return "cm";
				case DXWebUnitType.Percentage:
					return "%";
				case DXWebUnitType.Em:
					return "em";
				case DXWebUnitType.Ex:
					return "ex";
			}
			return string.Empty;
		}
		private static DXWebUnitType GetTypeFromString(string value) {
			if(string.IsNullOrEmpty(value))
				return DXWebUnitType.Pixel;
			if(value.Equals("px"))
				return DXWebUnitType.Pixel;
			if(value.Equals("pt"))
				return DXWebUnitType.Point;
			if(value.Equals("%"))
				return DXWebUnitType.Percentage;
			if(value.Equals("pc"))
				return DXWebUnitType.Pica;
			if(value.Equals("in"))
				return DXWebUnitType.Inch;
			if(value.Equals("mm"))
				return DXWebUnitType.Mm;
			if(value.Equals("cm"))
				return DXWebUnitType.Cm;
			if(value.Equals("em"))
				return DXWebUnitType.Em;
			if(!value.Equals("ex"))
				throw new ArgumentOutOfRangeException("value");
			return DXWebUnitType.Ex;
		}
		public static DXWebUnit Parse(string s) {
			return new DXWebUnit(s, CultureInfo.CurrentCulture);
		}
		public static DXWebUnit Parse(string s, CultureInfo culture) {
			return new DXWebUnit(s, culture);
		}
		public static DXWebUnit Percentage(double n) {
			return new DXWebUnit(n, DXWebUnitType.Percentage);
		}
		public static DXWebUnit Pixel(int n) {
			return new DXWebUnit(n);
		}
		public static DXWebUnit Point(int n) {
			return new DXWebUnit((double)n, DXWebUnitType.Point);
		}
		public static implicit operator DXWebUnit(int n) {
			return Pixel(n);
		}
		internal const int MaxValue = 0x7fff;
		internal const int MinValue = -32768;
		readonly DXWebUnitType type;
		readonly double value;
		public DXWebUnit(int value) {
			if(value < -32768 || value > 0x7fff)
				throw new ArgumentOutOfRangeException("value");
			this.value = value;
			type = DXWebUnitType.Pixel;
		}
		public DXWebUnit(double value) {
			if(value < -32768.0 || value > 32767.0)
				throw new ArgumentOutOfRangeException("value");
			this.value = (int)value;
			type = DXWebUnitType.Pixel;
		}
		public DXWebUnit(double value, DXWebUnitType type) {
			if(value < -32768.0 || value > 32767.0)
				throw new ArgumentOutOfRangeException("value");
			if(type == DXWebUnitType.Pixel)
				this.value = (int)value;
			else
				this.value = value;
			this.type = type;
		}
		public DXWebUnit(string value)
			: this(value, CultureInfo.CurrentCulture, DXWebUnitType.Pixel) {
		}
		public DXWebUnit(string value, CultureInfo culture)
			: this(value, culture, DXWebUnitType.Pixel) {
		}
		internal DXWebUnit(string value, CultureInfo culture, DXWebUnitType defaultType) {
			if(string.IsNullOrEmpty(value)) {
				this.value = 0.0;
				type = (DXWebUnitType)0;
			} else {
				if(culture == null)
					culture = CultureInfo.CurrentCulture;
				string str = value.Trim().ToLowerInvariant();
				int length = str.Length;
				int num2 = -1;
				for(int i = 0; i < length; i++) {
					char ch = str[i];
					if((ch < '0' || ch > '9') && ch != '-' && ch != '.' && ch != ',')
						break;
					num2 = i;
				}
				if(num2 == -1)
					throw new FormatException("UnitParseNoDigits");
				if(num2 < (length - 1))
					type = GetTypeFromString(str.Substring(num2 + 1).Trim());
				else
					type = defaultType;
				string text = str.Substring(0, num2 + 1);
				try {
					this.value = Convert.ToSingle(text, culture);
					if(type == DXWebUnitType.Pixel)
						this.value = (int)this.value;
				} catch {
					throw new FormatException("UnitParseNumericPart");
				}
				if(this.value < -32768.0 || this.value > 32767.0)
					throw new ArgumentOutOfRangeException("value");
			}
		}
		public bool IsEmpty {
			get { return type == (DXWebUnitType)0; }
		}
		public DXWebUnitType Type {
			get {
				if(!IsEmpty)
					return type;
				return DXWebUnitType.Pixel;
			}
		}
		public double Value {
			get { return value; }
		}
		public override int GetHashCode() {
			return DXHashCodeCombiner.CombineHashCodes(type.GetHashCode(), value.GetHashCode());
		}
		public override bool Equals(object obj) {
			if(obj == null || !(obj is DXWebUnit))
				return false;
			DXWebUnit unit = (DXWebUnit)obj;
			return unit.type == type && unit.value == value;
		}
		public override string ToString() {
			return ToString((IFormatProvider)CultureInfo.CurrentCulture);
		}
		public string ToString(CultureInfo culture) {
			return ToString((IFormatProvider)culture);
		}
		public string ToString(IFormatProvider formatProvider) {
			string str;
			if(IsEmpty)
				return string.Empty;
			if(type == DXWebUnitType.Pixel)
				str = ((int)value).ToString(formatProvider);
			else
				str = ((float)value).ToString(formatProvider);
			return str + GetStringFromType(type);
		}
	}
}
