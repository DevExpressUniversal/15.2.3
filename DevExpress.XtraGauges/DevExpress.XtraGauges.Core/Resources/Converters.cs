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
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Localization;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.XtraGauges.Core.Resources {
	public class FactorF2DConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str == null) return base.ConvertFrom(context, culture, value);
			str = str.Trim();
			if(str.Length == 0) return null;
			if(culture == null) {
				culture = CultureInfo.CurrentCulture;
			}
			char separator = culture.TextInfo.ListSeparator[0];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
			string[] strArray = str.Split(new char[] { separator });
			float[] numArray = new float[strArray.Length];
			for(int i = 0; i < numArray.Length; i++) {
				numArray[i] = (float)converter.ConvertFromString(context, culture, strArray[i]);
			}
			if(numArray.Length != 2) {
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgTextParsingError) + str, typeof(FactorF2D).Name));
			}
			return new FactorF2D(numArray[0], numArray[1]);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(culture == null) {
				culture = CultureInfo.CurrentCulture;
			}
			if(value is FactorF2D) {
				if(destinationType == typeof(string)) {
					FactorF2D point = (FactorF2D)value;
					string separator = culture.TextInfo.ListSeparator + " ";
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
					string[] strArray = new string[2];
					int num = 0;
					strArray[num++] = converter.ConvertToString(context, culture, point.XFactor);
					strArray[num++] = converter.ConvertToString(context, culture, point.YFactor);
					return string.Join(separator, strArray);
				}
				if(destinationType == typeof(InstanceDescriptor)) {
					FactorF2D point2 = (FactorF2D)value;
					ConstructorInfo constructor = typeof(FactorF2D).GetConstructor(new Type[] { typeof(float), typeof(float) });
					if(constructor != null) {
						return new InstanceDescriptor(constructor, new object[] { point2.XFactor, point2.YFactor });
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			if(propertyValues == null) {
				throw new ArgumentNullException("propertyValues");
			}
			object obj2 = propertyValues["XFactor"];
			object obj3 = propertyValues["YFactor"];
			if(((obj2 == null) || (obj3 == null)) || (!(obj2 is float) || !(obj3 is float))) {
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgInvalidClassCreationParameters), typeof(FactorF2D).Name));
			}
			return new FactorF2D((float)obj2, (float)obj3);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(typeof(FactorF2D), attributes).Sort(new string[] { "XFactor", "YFactor" });
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class PointF2DConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str == null) return base.ConvertFrom(context, culture, value);
			str = str.Trim();
			if(str.Length == 0) return null;
			if(culture == null) {
				culture = CultureInfo.CurrentCulture;
			}
			char separator = culture.TextInfo.ListSeparator[0];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
			string[] strArray = str.Split(new char[] { separator });
			float[] numArray = new float[strArray.Length];
			for(int i = 0; i < numArray.Length; i++) {
				numArray[i] = (float)converter.ConvertFromString(context, culture, strArray[i]);
			}
			if(numArray.Length != 2) {
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgTextParsingError) + str, typeof(PointF2D).Name));
			}
			return new PointF2D(numArray[0], numArray[1]);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(culture == null) {
				culture = CultureInfo.CurrentCulture;
			}
			if(value is PointF2D) {
				if(destinationType == typeof(string)) {
					PointF2D point = (PointF2D)value;
					string separator = culture.TextInfo.ListSeparator + " ";
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
					string[] strArray = new string[2];
					int num = 0;
					strArray[num++] = converter.ConvertToString(context, culture, point.X);
					strArray[num++] = converter.ConvertToString(context, culture, point.Y);
					return string.Join(separator, strArray);
				}
				if(destinationType == typeof(InstanceDescriptor)) {
					PointF2D point2 = (PointF2D)value;
					ConstructorInfo constructor = typeof(PointF2D).GetConstructor(new Type[] { typeof(float), typeof(float) });
					if(constructor != null) {
						return new InstanceDescriptor(constructor, new object[] { point2.X, point2.Y });
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			if(propertyValues == null) {
				throw new ArgumentNullException("propertyValues");
			}
			object obj2 = propertyValues["X"];
			object obj3 = propertyValues["Y"];
			if(((obj2 == null) || (obj3 == null)) || (!(obj2 is float) || !(obj3 is float))) {
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgInvalidClassCreationParameters), typeof(PointF2D).Name));
			}
			return new PointF2D((float)obj2, (float)obj3);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(typeof(PointF2D), attributes).Sort(new string[] { "X", "Y" });
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class RectangleF2DConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str == null) {
				return base.ConvertFrom(context, culture, value);
			}
			string str2 = str.Trim();
			if(str2.Length == 0) {
				return null;
			}
			if(culture == null) {
				culture = CultureInfo.CurrentCulture;
			}
			char ch = culture.TextInfo.ListSeparator[0];
			string[] strArray = str2.Split(new char[] { ch });
			float[] numArray = new float[strArray.Length];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
			for(int i = 0; i < numArray.Length; i++) {
				numArray[i] = (float)converter.ConvertFromString(context, culture, strArray[i]);
			}
			if(numArray.Length != 4) {
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgTextParsingError) + str2, typeof(RectangleF2D).Name));
			}
			return new RectangleF2D(numArray[0], numArray[1], numArray[2], numArray[3]);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(value is RectangleF2D) {
				if(destinationType == typeof(string)) {
					RectangleF2D rectangle = (RectangleF2D)value;
					if(culture == null) {
						culture = CultureInfo.CurrentCulture;
					}
					string separator = culture.TextInfo.ListSeparator + " ";
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
					string[] strArray = new string[4];
					int num = 0;
					strArray[num++] = converter.ConvertToString(context, culture, rectangle.X);
					strArray[num++] = converter.ConvertToString(context, culture, rectangle.Y);
					strArray[num++] = converter.ConvertToString(context, culture, rectangle.Width);
					strArray[num++] = converter.ConvertToString(context, culture, rectangle.Height);
					return string.Join(separator, strArray);
				}
				if(destinationType == typeof(InstanceDescriptor)) {
					RectangleF2D rectangle2 = (RectangleF2D)value;
					ConstructorInfo constructor = typeof(RectangleF2D).GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float), typeof(float) });
					if(constructor != null) {
						return new InstanceDescriptor(constructor, new object[] { rectangle2.X, rectangle2.Y, rectangle2.Width, rectangle2.Height });
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			if(propertyValues == null) {
				throw new ArgumentNullException("propertyValues");
			}
			object objX = propertyValues["X"];
			object objY = propertyValues["Y"];
			object objWidth = propertyValues["Width"];
			object objHeight = propertyValues["Height"];
			if((((objX == null) || (objY == null)) || ((objWidth == null) || (objHeight == null))) || ((!(objX is float) || !(objY is float)) || (!(objWidth is float) || !(objHeight is float)))) {
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgInvalidClassCreationParameters), typeof(RectangleF2D).Name));
			}
			return new RectangleF2D((float)objX, (float)objY, (float)objWidth, (float)objHeight);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(typeof(RectangleF2D), attributes).Sort(new string[] { "X", "Y", "Width", "Height" });
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class TextSpacingConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str == null) {
				return base.ConvertFrom(context, culture, value);
			}
			string str2 = str.Trim();
			if(str2.Length == 0) {
				return null;
			}
			if(culture == null) {
				culture = CultureInfo.CurrentCulture;
			}
			char ch = culture.TextInfo.ListSeparator[0];
			string[] strArray = str2.Split(new char[] { ch });
			int[] numArray = new int[strArray.Length];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
			for(int i = 0; i < numArray.Length; i++) {
				numArray[i] = (int)converter.ConvertFromString(context, culture, strArray[i]);
			}
			if(numArray.Length != 4) {
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgTextParsingError) + str2, typeof(TextSpacing).Name));
			}
			return new TextSpacing(numArray[0], numArray[1], numArray[2], numArray[3]);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(value is TextSpacing) {
				if(destinationType == typeof(string)) {
					TextSpacing spacing = (TextSpacing)value;
					if(culture == null) {
						culture = CultureInfo.CurrentCulture;
					}
					string separator = culture.TextInfo.ListSeparator + " ";
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
					string[] strArray = new string[4];
					int num = 0;
					strArray[num++] = converter.ConvertToString(context, culture, spacing.Left);
					strArray[num++] = converter.ConvertToString(context, culture, spacing.Top);
					strArray[num++] = converter.ConvertToString(context, culture, spacing.Right);
					strArray[num++] = converter.ConvertToString(context, culture, spacing.Bottom);
					return string.Join(separator, strArray);
				}
				if(destinationType == typeof(InstanceDescriptor)) {
					TextSpacing spacing2 = (TextSpacing)value;
					ConstructorInfo constructor = typeof(TextSpacing).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
					if(constructor != null) {
						return new InstanceDescriptor(constructor, new object[] { spacing2.Left, spacing2.Top, spacing2.Right, spacing2.Bottom });
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			if(propertyValues == null) {
				throw new ArgumentNullException("propertyValues");
			}
			object left = propertyValues["Left"];
			object top = propertyValues["Top"];
			object right = propertyValues["Right"];
			object bottom = propertyValues["Bottom"];
			if((((left == null) || (top == null)) || ((right == null) || (bottom == null))) || ((!(left is int) || !(top is int)) || (!(right is int) || !(bottom is int)))) {
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgInvalidClassCreationParameters), typeof(TextSpacing).Name));
			}
			return new TextSpacing((int)left, (int)top, (int)right, (int)bottom);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(typeof(TextSpacing), attributes).Sort(new string[] { "Left", "Top", "Right", "Bottom" });
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class ThicknessConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str == null) {
				return base.ConvertFrom(context, culture, value);
			}
			string str2 = str.Trim();
			if(str2.Length == 0) {
				return null;
			}
			if(culture == null) {
				culture = CultureInfo.CurrentCulture;
			}
			char ch = culture.TextInfo.ListSeparator[0];
			string[] strArray = str2.Split(new char[] { ch });
			int[] numArray = new int[strArray.Length];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
			for(int i = 0; i < numArray.Length; i++) {
				numArray[i] = (int)converter.ConvertFromString(context, culture, strArray[i]);
			}
			if(numArray.Length != 4) {
				if(numArray.Length == 1)
					return new Thickness(numArray[0]);
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgTextParsingError) + str2, typeof(Thickness).Name));
			}
			return new Thickness(numArray[0], numArray[1], numArray[2], numArray[3]);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(value is Thickness) {
				Thickness thickness = (Thickness)value;
				if(destinationType == typeof(string)) {
					if(culture == null) {
						culture = CultureInfo.CurrentCulture;
					}
					string separator = culture.TextInfo.ListSeparator + " ";
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
					if(thickness.All == -1) {
						string[] strArray = new string[4];
						strArray[0] = converter.ConvertToString(context, culture, thickness.Left);
						strArray[1] = converter.ConvertToString(context, culture, thickness.Top);
						strArray[2] = converter.ConvertToString(context, culture, thickness.Right);
						strArray[3] = converter.ConvertToString(context, culture, thickness.Bottom);
						return string.Join(separator, strArray);
					}
					else return converter.ConvertToString(context, culture, thickness.All);
				}
				if(destinationType == typeof(InstanceDescriptor)) {
					if(thickness.All == -1) {
						ConstructorInfo constructor = typeof(Thickness).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
						if(constructor != null) {
							return new InstanceDescriptor(constructor, new object[] { thickness.Left, thickness.Top, thickness.Right, thickness.Bottom });
						}
					}
					else {
						ConstructorInfo constructor = typeof(Thickness).GetConstructor(new Type[] { typeof(int) });
						if(constructor != null) {
							return new InstanceDescriptor(constructor, new object[] { thickness.All });
						}
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			if(propertyValues == null) {
				throw new ArgumentNullException("propertyValues");
			}
			object left = propertyValues["Left"];
			object top = propertyValues["Top"];
			object right = propertyValues["Right"];
			object bottom = propertyValues["Bottom"];
			if(
				(((left == null) || (top == null)) || ((right == null) || (bottom == null))) ||
				((!(left is int) || !(top is int)) || (!(right is int) || !(bottom is int)))) {
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgInvalidClassCreationParameters), typeof(Thickness).Name));
			}
			return new Thickness((int)left, (int)top, (int)right, (int)bottom);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(typeof(Thickness), attributes).Sort(new string[] { "All", "Left", "Top", "Right", "Bottom" });
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
