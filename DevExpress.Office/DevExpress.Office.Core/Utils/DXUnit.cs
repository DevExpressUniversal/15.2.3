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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace DevExpress.Office.Utils {
	public enum DXUnitType {
		Pixel = 1,
		Point = 2,
		Pica = 3,
		Inch = 4,
		Mm = 5,
		Cm = 6,
		Percentage = 7,
		Em = 8,
		Ex = 9,
		Deg = 10,
		Fd = 11,
		Emu = 12
	}
	#region StringUnitValueParser
	public class StringUnitValueParser {
		public StringUnitValueParser() {
		}
		public DXUnit GetUnit(string inputValue) {
			return GetUnit(inputValue, -32768.0f, 32767.0f);
		}
		protected internal DXUnit GetUnit(string inputValue, float minValue, float maxValue) {
			ValueInfo valueInfo = StringValueParser.TryParse(inputValue);
			if(valueInfo.IsValidNumber)
				return new DXUnit(valueInfo.Value, GetTypeFromString(valueInfo.Unit), minValue, maxValue);
			else {
				new ArgumentException("UnitValue", inputValue);
				return new DXUnit();
			}
		}
		protected internal DXRotationUnit GetRotationUnitType(string inputValue, float minValue, float maxValue) {
			ValueInfo valueInfo = StringValueParser.TryParse(inputValue);
			if(valueInfo.IsValidNumber)
				return new DXRotationUnit(valueInfo.Value, GetRotationUnitTypeFromString(valueInfo.Unit), minValue, maxValue);
			else {
				new ArgumentException("UnitValue", inputValue);
				return new DXRotationUnit();
			}
		}
		protected internal DXVmlUnit GetVmlUnitType(string inputValue, float minValue, float maxValue) {
			ValueInfo valueInfo = StringValueParser.TryParse(inputValue);
			if(valueInfo.IsValidNumber)
				return new DXVmlUnit(valueInfo.Value, GetVmlUnitTypeFromString(valueInfo.Unit), minValue, maxValue);
			else {
				new ArgumentException("UnitValue", inputValue);
				return new DXVmlUnit();
			}
		}
		private static DXUnitType GetTypeFromString(string value) {
			if(string.IsNullOrEmpty(value))
				return DXUnitType.Pixel;
			switch (value.ToUpperInvariant()) {
				case "PX":
					return DXUnitType.Pixel;
				case "PT":
					return DXUnitType.Point;
				case "%":
					return DXUnitType.Percentage;
				case "PC":
					return DXUnitType.Pica;
				case "IN":
					return DXUnitType.Inch;
				case "MM":
					return DXUnitType.Mm;
				case "CM":
					return DXUnitType.Cm;
				case "EM":
					return DXUnitType.Em;
				case "EX":
					return DXUnitType.Ex;
				default:
					throw new ArgumentException("UnitType", value);
			}
		}
		private static DXUnitType GetRotationUnitTypeFromString(string value) {
			if(string.IsNullOrEmpty(value))
				return DXUnitType.Deg;
			switch(value.ToUpperInvariant()) {
				case "FD":
					return DXUnitType.Fd;
				default:
					throw new ArgumentException("UnitType", value);
			}
		}
		private static DXUnitType GetVmlUnitTypeFromString(string value) {
			if(string.IsNullOrEmpty(value))
				return DXUnitType.Emu;
			switch(value.ToUpperInvariant()) {
				case "PX":
					return DXUnitType.Pixel;
				case "PT":
					return DXUnitType.Point;
				case "PC":
					return DXUnitType.Pica;
				case "IN":
					return DXUnitType.Inch;
				case "MM":
					return DXUnitType.Mm;
				case "CM":
					return DXUnitType.Cm;
				default:
					throw new ArgumentException("UnitType", value);
			}
		}
	}
	#endregion
	#region DXUnitBase
	public abstract class DXUnitBase {
		readonly float value;
		readonly DXUnitType type;
		public float Value { get { return value; } }
		public DXUnitType Type { get { return type; } }
		protected DXUnitBase(float value, DXUnitType type) {
			this.value = ObtainValue(value, type);
			this.type = type;
		}
		protected DXUnitBase(float value, DXUnitType type, float minValue, float maxValue) {
			ValidateValueRange(value, minValue, maxValue);
			this.value = ObtainValue(value, type);
			this.type = type;
		}
		protected DXUnitBase(int value, DXUnitType type, float minValue, float maxValue) {
			ValidateValueRange(value, minValue, maxValue);
			this.value = value;
			this.type = type;
		}
		protected DXUnitBase(string value, float minValue, float maxValue) {
			DXUnitBase unit = Parse(value, minValue, maxValue);
			this.value = unit.value;
			this.type = unit.type;
		}
		protected abstract DXUnitBase Parse(string value, float minValue, float maxValue);
		float ObtainValue(float value, DXUnitType type) {
			if(type == DXUnitType.Pixel || type == DXUnitType.Emu)
				value = (int)value;
			return value;
		}
		void ValidateValueRange(float value, float minValue, float maxValue) {
			if(value < minValue || value > maxValue)
				throw new ArgumentOutOfRangeException("value");
		}
	}
	#endregion
	#region DXUnit
	public class DXUnit : DXUnitBase {
		public DXUnit() 
			: base(0, DXUnitType.Pixel) {
		}
		public DXUnit(float value) 
			: base(value, DXUnitType.Pixel, -32768.0f, 32767.0f) {
		}
		public DXUnit(int value)
			: this(value, -32768, 0x7fff) {
		}
		public DXUnit(int value, float minValue, float maxValue) 
			: base(value, DXUnitType.Pixel, minValue, maxValue) {
		}
		public DXUnit(float value, DXUnitType type)
			: this(value, type, -32768.0f, 32767.0f) {
		}
		public DXUnit(float value, DXUnitType type, float minValue, float maxValue) 
			: base(value, type, minValue, maxValue) {
		}
		public DXUnit(string value)
			: base(value, -32768.0f, 32767.0f) {
		}
		public DXUnit(string value, float minValue, float maxValue)
			: base(value, minValue, maxValue) {
		}
		protected override DXUnitBase Parse(string value, float minValue, float maxValue) {
			StringUnitValueParser parser = new StringUnitValueParser();
			return parser.GetUnit(value, minValue, maxValue);
		}
	}
	#endregion
	#region DXRotationUnit
	public class DXRotationUnit : DXUnitBase {
		public DXRotationUnit() : 
			base(0, DXUnitType.Deg) {
		}
		public DXRotationUnit(string value) 
			: base(value, Int32.MinValue, Int32.MaxValue) {
		}
		public DXRotationUnit(float value, DXUnitType type, float minValue, float maxValue) 
			: base(value, type, minValue, maxValue) {
		}
		protected override DXUnitBase Parse(string value, float minValue, float maxValue) {
			StringUnitValueParser parser = new StringUnitValueParser();
			return parser.GetRotationUnitType(value, minValue, maxValue);
		}
	}
	#endregion
	#region DXVmlUnit
	public class DXVmlUnit : DXUnitBase {
		public DXVmlUnit()
			: base(0, DXUnitType.Emu) {
		}
		public DXVmlUnit(string value)
			: base(value, Int32.MinValue, Int32.MaxValue) {
		}
		public DXVmlUnit(float value, DXUnitType type)
			: base(value, type, Int32.MinValue, Int32.MaxValue) {
		}
		public DXVmlUnit(float value, DXUnitType type, float minValue, float maxValue) 
			: base(value, type, minValue, maxValue) {
		}
		protected override DXUnitBase Parse(string value, float minValue, float maxValue) {
			StringUnitValueParser parser = new StringUnitValueParser();
			return parser.GetVmlUnitType(value, minValue, maxValue);
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			if(Type == DXUnitType.Emu || Type == DXUnitType.Pixel)
				sb.Append((int)Value);
			else
				sb.Append(Value.ToString(CultureInfo.InvariantCulture));
			sb.Append(GetSuffix());
			return sb.ToString();
		}
		string GetSuffix() {
			switch(Type) {
				case DXUnitType.Cm: return "cm";
				case DXUnitType.Mm: return "mm";
				case DXUnitType.Inch: return "in";
				case DXUnitType.Point: return "pt";
				case DXUnitType.Pica: return "pc";
				case DXUnitType.Pixel: return "px";
			}
			return string.Empty;
		}
	}
	#endregion
}
