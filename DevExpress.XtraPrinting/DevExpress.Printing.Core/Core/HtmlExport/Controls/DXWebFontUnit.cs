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
using DevExpress.Compatibility.System;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	[Serializable]
	public struct DXWebFontUnit {
		public static implicit operator DXWebFontUnit(int n) {
			return Point(n);
		}
		static DXWebFontUnit() {
			Empty = new DXWebFontUnit();
			Smaller = new DXWebFontUnit(DXWebFontSize.Smaller);
			Larger = new DXWebFontUnit(DXWebFontSize.Larger);
			XXSmall = new DXWebFontUnit(DXWebFontSize.XXSmall);
			XSmall = new DXWebFontUnit(DXWebFontSize.XSmall);
			Small = new DXWebFontUnit(DXWebFontSize.Small);
			Medium = new DXWebFontUnit(DXWebFontSize.Medium);
			Large = new DXWebFontUnit(DXWebFontSize.Large);
			XLarge = new DXWebFontUnit(DXWebFontSize.XLarge);
			XXLarge = new DXWebFontUnit(DXWebFontSize.XXLarge);
		}
		internal static readonly DXWebFontUnit Empty;
		static readonly DXWebFontUnit Smaller;
		static readonly DXWebFontUnit Larger;
		static readonly DXWebFontUnit XXSmall;
		static readonly DXWebFontUnit XSmall;
		static readonly DXWebFontUnit Small;
		static readonly DXWebFontUnit Medium;
		static readonly DXWebFontUnit Large;
		static readonly DXWebFontUnit XLarge;
		static readonly DXWebFontUnit XXLarge;
		readonly DXWebFontSize type;
		readonly DXWebUnit value;
		public bool IsEmpty {
			get { return type == DXWebFontSize.NotSet; }
		}
		public DXWebFontSize Type {
			get { return type; }
		}
		public DXWebUnit Unit {
			get { return value; }
		}
		public DXWebFontUnit(DXWebFontSize type) {
			if(type < DXWebFontSize.NotSet || type > DXWebFontSize.XXLarge)
				throw new ArgumentOutOfRangeException("type");
			this.type = type;
			if(this.type == DXWebFontSize.AsUnit)
				value = DXWebUnit.Point(10);
			else
				value = DXWebUnit.Empty;
		}
		public DXWebFontUnit(DXWebUnit value) {
			type = DXWebFontSize.NotSet;
			if(!value.IsEmpty) {
				type = DXWebFontSize.AsUnit;
				this.value = value;
			} else
				this.value = DXWebUnit.Empty;
		}
		public DXWebFontUnit(int value) {
			type = DXWebFontSize.AsUnit;
			this.value = DXWebUnit.Point(value);
		}
		public DXWebFontUnit(double value)
			: this(new DXWebUnit(value, DXWebUnitType.Point)) {
		}
		public DXWebFontUnit(double value, DXWebUnitType type)
			: this(new DXWebUnit(value, type)) {
		}
		public DXWebFontUnit(string value)
			: this(value, CultureInfo.CurrentCulture) {
		}
		public DXWebFontUnit(string value, CultureInfo culture) {
			type = DXWebFontSize.NotSet;
			this.value = DXWebUnit.Empty;
			if(!string.IsNullOrEmpty(value)) {
				char ch = char.ToLowerInvariant(value[0]);
				switch(ch) {
					case 's':
						if(string.Equals(value, "small", StringComparison.OrdinalIgnoreCase)) {
							type = DXWebFontSize.Small;
							return;
						}
						if(string.Equals(value, "smaller", StringComparison.OrdinalIgnoreCase)) {
							type = DXWebFontSize.Smaller;
							return;
						}
						break;
					case 'l':
						if(string.Equals(value, "large", StringComparison.OrdinalIgnoreCase)) {
							type = DXWebFontSize.Large;
							return;
						}
						if(string.Equals(value, "larger", StringComparison.OrdinalIgnoreCase)) {
							type = DXWebFontSize.Larger;
							return;
						}
						break;
					case 'x':
						if(string.Equals(value, "xx-small", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "xxsmall", StringComparison.OrdinalIgnoreCase)) {
							type = DXWebFontSize.XXSmall;
							return;
						}
						if(string.Equals(value, "x-small", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "xsmall", StringComparison.OrdinalIgnoreCase)) {
							type = DXWebFontSize.XSmall;
							return;
						}
						if(string.Equals(value, "x-large", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "xlarge", StringComparison.OrdinalIgnoreCase)) {
							type = DXWebFontSize.XLarge;
							return;
						}
						if(string.Equals(value, "xx-large", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "xxlarge", StringComparison.OrdinalIgnoreCase)) {
							type = DXWebFontSize.XXLarge;
							return;
						}
						break;
					default:
						if(ch == 'm' && string.Equals(value, "medium", StringComparison.OrdinalIgnoreCase)) {
							type = DXWebFontSize.Medium;
							return;
						}
						break;
				}
				this.value = new DXWebUnit(value, culture, DXWebUnitType.Point);
				type = DXWebFontSize.AsUnit;
			}
		}
		public override int GetHashCode() {
			return DXHashCodeCombiner.CombineHashCodes(type.GetHashCode(), value.GetHashCode());
		}
		public override bool Equals(object obj) {
			if(obj == null || !(obj is DXWebFontUnit))
				return false;
			DXWebFontUnit unit = (DXWebFontUnit)obj;
			return unit.type == type && unit.value == value;
		}
		public static bool operator ==(DXWebFontUnit left, DXWebFontUnit right) {
			return left.type == right.type && left.value == right.value;
		}
		public static bool operator !=(DXWebFontUnit left, DXWebFontUnit right) {
			if(left.type == right.type)
				return left.value != right.value;
			return true;
		}
		public static DXWebFontUnit Parse(string s) {
			return new DXWebFontUnit(s, CultureInfo.InvariantCulture);
		}
		public static DXWebFontUnit Parse(string s, CultureInfo culture) {
			return new DXWebFontUnit(s, culture);
		}
		public static DXWebFontUnit Point(int n) {
			return new DXWebFontUnit(n);
		}
		public override string ToString() {
			return ToString((IFormatProvider)CultureInfo.CurrentCulture);
		}
		public string ToString(CultureInfo culture) {
			return ToString((IFormatProvider)culture);
		}
		public string ToString(IFormatProvider formatProvider) {
			string str = string.Empty;
			if(IsEmpty)
				return str;
			switch(type) {
				case DXWebFontSize.AsUnit:
					return value.ToString(formatProvider);
				case DXWebFontSize.XXSmall:
					return "XX-Small";
				case DXWebFontSize.XSmall:
					return "X-Small";
				case DXWebFontSize.XLarge:
					return "X-Large";
				case DXWebFontSize.XXLarge:
					return "XX-Large";
			}
			return type.ToString().Replace('_', '-');
		}
	}
}
