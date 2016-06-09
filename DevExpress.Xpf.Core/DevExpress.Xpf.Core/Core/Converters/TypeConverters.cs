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
using System.Windows;
namespace DevExpress.Xpf.Core {
#if SL
	public class DecimalConverter : TypeConverter {
		public DecimalConverter() {
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null)
				return null;
			if(!(value is string))
				throw new ArgumentException("", "value");
			return Convert.ChangeType(value, typeof(decimal), CultureInfo.InvariantCulture);
		}
	}
	public class NullableConverter<T> : TypeConverter {
		public NullableConverter() {
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null)
				return null;
			if(!(value is string))
				throw new ArgumentException("", "value");
			if(typeof(T).IsEnum)
				return Enum.Parse(typeof(T), value as string, false);
			return Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
		}
	}
	public class DateTimeTypeConverter : TypeConverter {
		public DateTimeTypeConverter() {
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (culture == null)
				throw new ArgumentNullException("culture");
			if (value == null)
				throw new ArgumentNullException("value");
			string s = value.ToString();
			if (s.ToLower() == "today") return DateTime.Today;
			if (s.ToLower() == "now") return DateTime.Now;
			var info = (DateTimeFormatInfo)culture.GetFormat(typeof(DateTimeFormatInfo));
			return DateTime.ParseExact(s, info.ShortDatePattern, culture);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (culture == null)
				throw new ArgumentNullException("culture");
			if (destinationType == null)
				throw new ArgumentNullException("destinationType");
			var date = value as DateTime?;
			if (!date.HasValue || destinationType != typeof(string))
				throw new NotSupportedException();
			else {
				var info = (DateTimeFormatInfo)culture.GetFormat(typeof(DateTimeFormatInfo));
				return date.Value.ToString(info.ShortDatePattern, culture);
			}
		}
	}
	public class RectConverter : TypeConverter, DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter {
		public static readonly RectConverter Instance = new RectConverter();
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null)
				throw new ArgumentNullException("value");
			string source = value as string;
			if(source != null)
				return FromString(source, culture);
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null)
				throw new ArgumentNullException("destinationType");
			if(value is Rect) {
				Rect rect = (Rect)value;
				if(destinationType == typeof(string))
					return ToString(rect, null, culture);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		internal static Rect FromString(string source, CultureInfo culture) {
			char separator = GetNumericListSeparator(culture);
			string[] parts = source.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
			if(parts.Length == 4) {
				double x, y, width, height;
				double.TryParse(parts[0], out x);
				double.TryParse(parts[1], out y);
				double.TryParse(parts[2], out width);
				double.TryParse(parts[3], out height);
				return new Rect(x, y, width, height);
			}
			throw new ArgumentException(source);
		}
		internal static string ToString(Rect rect, string format, CultureInfo culture) {
			char separator = GetNumericListSeparator(culture);
			return string.Format(culture,
				"{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}",
				new object[] { separator, rect.X, rect.Y, rect.Width, rect.Height });
		}
		internal static char GetNumericListSeparator(IFormatProvider provider) {
			char separator = ',';
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			if((instance.NumberDecimalSeparator.Length > 0) && (separator == instance.NumberDecimalSeparator[0])) 
				separator = ';';
			return separator;
		}
		#region IOneTypeObjectConverter Members
		object DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.FromString(string str) {
			return FromString(str, CultureInfo.InvariantCulture);
		}
		string DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.ToString(object obj) {
			return ToString((Rect)obj, null, CultureInfo.InvariantCulture);
		}
		Type DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.Type {
			get { return typeof(Rect); }
		}
		#endregion
	}
	public class GridLengthConverter : TypeConverter, DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter {
		public static readonly GridLengthConverter Instance = new GridLengthConverter();
		public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType) {
			switch(Type.GetTypeCode(sourceType)) {
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.String:
					return true;
			}
			return false;
		}
		public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType) {
			if(destinationType != typeof(ComponentModel.Design.Serialization.InstanceDescriptor)) 
				return (destinationType == typeof(string));
			return true;
		}
		public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source) {
			if(source == null)
				throw new ArgumentNullException("source");
			if(source is string)
				return FromString((string)source, cultureInfo);
			double value = Convert.ToDouble(source, cultureInfo);
			GridUnitType auto;
			if(Double.IsNaN(value)) {
				value = 1.0;
				auto = GridUnitType.Auto;
			}
			else auto = GridUnitType.Pixel;
			return new GridLength(value, auto);
		}
		[System.Security.SecurityCritical]
		public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType) {
			if(destinationType == null)
				throw new ArgumentNullException("destinationType");
			if((value != null) && (value is GridLength)) {
				GridLength gridLength = (GridLength)value;
				if(destinationType == typeof(string)) 
					return ToString(gridLength, cultureInfo);
				if(destinationType == typeof(ComponentModel.Design.Serialization.InstanceDescriptor)) {
					return new ComponentModel.Design.Serialization.InstanceDescriptor(
							typeof(GridLength).GetConstructor(new Type[] { typeof(double), typeof(GridUnitType) }),
							new object[] { gridLength.Value, gridLength.GridUnitType }
						);
				}
			}
			throw new NotSupportedException("destinationType");
		}
		internal static System.Windows.GridLength FromString(string s, CultureInfo cultureInfo) {
			GridUnitType type; 
			double value; 
			XamlGridLengthSerializer.FromString(s, cultureInfo, out value, out type);
			return new GridLength(value, type);
		}
		internal static string ToString(GridLength gridLength, CultureInfo cultureInfo) {
			switch(gridLength.GridUnitType) {
				case GridUnitType.Auto:
					return "Auto";
				case GridUnitType.Star:
					if(gridLength.Value == 1.0)
						return "*";
					return (Convert.ToString(gridLength.Value, cultureInfo) + "*");
			}
			return Convert.ToString(gridLength.Value, cultureInfo);
		}
		static class XamlGridLengthSerializer {
			static double[] PixelUnitFactors;
			static string[] PixelUnitStrings;
			static string[] UnitStrings;
			static XamlGridLengthSerializer() {
				UnitStrings = new string[] { "auto", "px", "*" };
				PixelUnitStrings = new string[] { "in", "cm", "pt" };
				PixelUnitFactors = new double[] { 96.0, 37.795275590551178, 1.3333333333333333 };
			}
			internal static void FromString(string s, CultureInfo cultureInfo, out double value, out GridUnitType unit) {
				string str = s.Trim().ToLowerInvariant();
				value = 0.0;
				unit = GridUnitType.Pixel;
				int length = str.Length;
				int num3 = 0;
				double num4 = 1.0;
				int index = 0;
				if(str == UnitStrings[index]) {
					num3 = UnitStrings[index].Length;
					unit = (GridUnitType)index;
				}
				else {
					index = 1;
					while(index < UnitStrings.Length) {
						if(str.EndsWith(UnitStrings[index], StringComparison.Ordinal)) {
							num3 = UnitStrings[index].Length;
							unit = (GridUnitType)index;
							break;
						}
						index++;
					}
				}
				if(index >= UnitStrings.Length) {
					for(index = 0; index < PixelUnitStrings.Length; index++) {
						if(str.EndsWith(PixelUnitStrings[index], StringComparison.Ordinal)) {
							num3 = PixelUnitStrings[index].Length;
							num4 = PixelUnitFactors[index];
							break;
						}
					}
				}
				if((length == num3) && ((unit == GridUnitType.Auto) || (unit == GridUnitType.Star))) {
					value = 1.0;
				}
				else {
					string str2 = str.Substring(0, length - num3);
					value = Convert.ToDouble(str2, cultureInfo) * num4;
				}
			}
		}
		#region IOneTypeObjectConverter Members
		object DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.FromString(string str) {
			return FromString(str, CultureInfo.InvariantCulture);
		}
		string DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.ToString(object obj) {
			return ToString((GridLength)obj, CultureInfo.InvariantCulture);
		}
		Type DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.Type {
			get { return typeof(GridLength); }
		}
		#endregion
	}
	public class SLTypeConverter<T> : TypeConverter {
		public SLTypeConverter() { }
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (!(value is string))
				throw new ArgumentException("", "value");
			if (typeof(T).IsEnum)
				return Enum.Parse(typeof(T), value as string, false);
			return Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
		}
	}
	class ThicknessTypeConverter : TypeConverter, DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter {
		public static readonly ThicknessTypeConverter Instance = new ThicknessTypeConverter();
		public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType) {
			switch(Type.GetTypeCode(sourceType)) {
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.String:
					return true;
			}
			return false;
		}
		public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType) {
			return destinationType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source) {
			if(source == null)
				throw new NullReferenceException();
			if(source is string)
				return FromString((string)source, cultureInfo);
			if(source is double)
				return new Thickness((double)source);
			return new Thickness(Convert.ToDouble(source, cultureInfo));
		}
		public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType) {
			if(!(value is Thickness))
				throw new ArgumentException("UnexpectedParameterType");
			if(destinationType == typeof(string))
				return ToString((Thickness)value, cultureInfo);
			throw new ArgumentException();
		}
		static Thickness FromString(string source, CultureInfo cultureInfo) {
			char separator = ThicknessConverterHelper.GetNumericListSeparator(cultureInfo);
			string[] parts = source.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
			int length = parts.Length;
			if(length <= 4 && length > 0) {
				double[] numArray = new double[parts.Length];
				for(int i = 0; i < length; i++) {
					numArray[i] = ThicknessConverterHelper.FromString(parts[i], cultureInfo);
				}
				switch(length) {
					case 1:
						return new Thickness(numArray[0]);
					case 2:
						return new Thickness(numArray[0], numArray[1], numArray[0], numArray[1]);
					case 4:
						return new Thickness(numArray[0], numArray[1], numArray[2], numArray[3]);
				}
			}
			throw new ArgumentException(source);
		}
		internal static string ToString(Thickness th, CultureInfo cultureInfo) {
			char separator = ThicknessConverterHelper.GetNumericListSeparator(cultureInfo);
			return string.Format(cultureInfo,
				"{1}{0}{2}{0}{3}{0}{4}",
				new object[] { separator, 
					ThicknessConverterHelper.ToString(th.Left, cultureInfo), 
					ThicknessConverterHelper.ToString(th.Top, cultureInfo), 
					ThicknessConverterHelper.ToString(th.Right, cultureInfo), 
					ThicknessConverterHelper.ToString(th.Bottom, cultureInfo)
				});
		}
		#region IOneTypeObjectConverter Members
		object DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.FromString(string str) {
			return FromString(str, CultureInfo.InvariantCulture);
		}
		string DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.ToString(object obj) {
			return ToString((Thickness)obj, CultureInfo.InvariantCulture);
		}
		Type DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.Type {
			get { return typeof(Thickness); }
		}
		#endregion
		internal class ThicknessConverterHelper {
			static double[] PixelUnitFactors = new double[] { 1.0, 96.0, 37.795275590551178, 1.3333333333333333 };
			static string[] PixelUnitStrings = new string[] { "px", "in", "cm", "pt" };
			internal static double FromString(string s, CultureInfo cultureInfo) {
				double result;
				string str = s.Trim();
				string str2 = str.ToLowerInvariant();
				int length = str2.Length;
				int num2 = 0;
				double num3 = 1.0;
				if(str2 == "auto")
					return double.NaN;
				for(int i = 0; i < PixelUnitStrings.Length; i++) {
					if(str2.EndsWith(PixelUnitStrings[i], StringComparison.Ordinal)) {
						num2 = PixelUnitStrings[i].Length;
						num3 = PixelUnitFactors[i];
						break;
					}
				}
				str = str.Substring(0, length - num2);
				try {
					result = Convert.ToDouble(str, cultureInfo) * num3;
				}
				catch(FormatException) {
					throw new FormatException("LengthFormatError");
				}
				return result;
			}
			internal static string ToString(double value, CultureInfo cultureInfo) {
				if(double.IsNaN(value)) return "Auto";
				return Convert.ToString(value, cultureInfo);
			}
			internal static char GetNumericListSeparator(IFormatProvider provider) {
				char ch = ',';
				NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
				if((instance.NumberDecimalSeparator.Length > 0) && (ch == instance.NumberDecimalSeparator[0])) {
					ch = ';';
				}
				return ch;
			}
		}
	}
#else
	class BrushConverter<T> : DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter2 where T : System.Windows.Media.Brush {
		const string BrushValue = "~Xtra#Brush";
		public static readonly DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter2 Instance = new BrushConverter<T>();
		string SerializeToString(object value) {
			using(var stream = new System.IO.MemoryStream()) {
				System.Windows.Markup.XamlWriter.Save(value, stream);
				return BrushValue + Convert.ToBase64String(stream.ToArray());
			}
		}
		object DeserializeFromString(string str) {
			Byte[] res = Convert.FromBase64String(str.Substring(BrushValue.Length));
			using(var stream = new System.IO.MemoryStream(res)) {
				return System.Windows.Markup.XamlReader.Load(stream);
			}
		}
		#region IOneTypeObjectConverter Members
		object DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.FromString(string str) {
			return DeserializeFromString(str);
		}
		string DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.ToString(object obj) {
			return SerializeToString(obj);
		}
		Type DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter.Type {
			get { return typeof(T); }
		}
		bool DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter2.CanConvertFromString(string str) {
			return str.StartsWith(BrushValue);
		}
		#endregion
	}
	class TextDecorationsConverter : DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter {
		public static readonly DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter Instance = new TextDecorationsConverter();
		const string None = "None";
		const string separator = ",";
		class TextDecorationInfo {
			public TextDecoration Decoration { get; private set; }
			public string Name { get; private set; }
			public TextDecorationInfo(TextDecoration decoration, string name) {
				Decoration = decoration;
				Name = name;
			}
		}
		TextDecorationInfo[] predefinedTextDecorations = new TextDecorationInfo[] {
			new TextDecorationInfo(TextDecorations.OverLine[0], "OverLine"),
			new TextDecorationInfo(TextDecorations.Baseline[0], "Baseline"),
			new TextDecorationInfo(TextDecorations.Underline[0], "Underline"),
			new TextDecorationInfo(TextDecorations.Strikethrough[0], "Strikethrough")
		};
		string SerializeToString(TextDecorationCollection collection) {
			int index = 0;
			int unknownDecorationsCount = collection.Count;
			var foundNames = new System.Collections.Generic.List<string>();
			while(index < predefinedTextDecorations.Length && unknownDecorationsCount > 0) {
				TextDecorationInfo info = predefinedTextDecorations[index];
				if(collection.Contains(info.Decoration)) {
					foundNames.Add(info.Name);
					unknownDecorationsCount--;
				}
				index++;
			}
			return foundNames.Count > 0 ? string.Join(separator, foundNames) : None;
		}
		#region IOneTypeObjectConverter Members
		public object FromString(string str) {
			return TextDecorationCollectionConverter.ConvertFromString(str);
		}
		public string ToString(object obj) {
			TextDecorationCollection collection = obj as TextDecorationCollection;
			if(collection == null)
				return None;
			return SerializeToString(collection);
		}
		public Type Type {
			get { return typeof(TextDecorationCollection); }
		}
		#endregion
	}
#endif
}
