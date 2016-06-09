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
using System.Globalization;
namespace DevExpress.Xpf.Gauges {
	public enum SymbolLengthType {
		Auto,
		Stretch,
		Fixed,
		Proportional
	}
	[TypeConverter(typeof(SymbolLengthConverter))]
	public struct SymbolLength {
		SymbolLengthType type;
		double fixedLength;
		double proportionalLength;
		public SymbolLengthType Type { get { return type; } }
		public double FixedLength { get { return fixedLength; } }
		public double ProportionalLength { get { return proportionalLength; } }
		public SymbolLength(SymbolLengthType type, double length) {
			this.type = type;
			fixedLength = 0.0;
			proportionalLength = 0.0;
			switch(type){
				case SymbolLengthType.Fixed: fixedLength = length; break;
				case SymbolLengthType.Proportional: proportionalLength = length; break;
			}
		}
		public SymbolLength(SymbolLengthType type) :this(type, 0.0) {
		}
	}
	public class SymbolLengthConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string stringValue = value as string;
			if (stringValue != null) {
				if (stringValue == "Auto")
					return new SymbolLength(SymbolLengthType.Auto);
				if (stringValue == "Stretch")
					return new SymbolLength(SymbolLengthType.Stretch);
				if (stringValue == "*")
					return new SymbolLength(SymbolLengthType.Proportional, 1.0);
				if (stringValue.EndsWith("*"))
					return new SymbolLength(SymbolLengthType.Proportional, Convert.ToDouble(stringValue.Substring(0, stringValue.Length - 1), culture));
				return new SymbolLength(SymbolLengthType.Fixed, Convert.ToDouble(stringValue, culture));
			}
			return null;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (value is SymbolLength) {
				SymbolLength symbolLengthValue = (SymbolLength)value;
				if (destinationType == typeof(string)) {
					string stringValue = "";
					switch (symbolLengthValue.Type) {
						case SymbolLengthType.Auto: stringValue = "Auto"; break;
						case SymbolLengthType.Stretch: stringValue = "Stretch"; break;
						case SymbolLengthType.Fixed: stringValue = symbolLengthValue.FixedLength.ToString(culture); break;
						case SymbolLengthType.Proportional: stringValue = symbolLengthValue.ProportionalLength == 1 ? "*" : symbolLengthValue.ProportionalLength.ToString(culture) + "*"; break;
					}
					return stringValue;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
