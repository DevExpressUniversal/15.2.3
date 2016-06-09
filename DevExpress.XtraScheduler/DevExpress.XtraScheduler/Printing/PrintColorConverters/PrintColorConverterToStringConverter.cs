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
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	class PrintColorConverterToStringConverter : TypeConverter {
		const int predefineConverterFlag = 0x100000;
		const int fullColorFlag = 0x010000;
		const int grayScaleColorFlag = 0x020000;
		const int blackAndWhiteColorFlag = 0x040000;
		static int GetDescriptionValue(PrintColorConverter colorConverter) {
			int result = colorConverter.IsPredefinedConverter ? predefineConverterFlag : (int)colorConverter.ApplyFlags;
			if (colorConverter is BlackAndWhitePrintColorConverter)
				return result | blackAndWhiteColorFlag;
			if (colorConverter is GrayScalePrintColorConverter)
				return result | grayScaleColorFlag;
			return result | fullColorFlag;
		}
		static PrintColorConverter CreateFromDescriptionValue(int descriptionValue) {
			if ((descriptionValue & predefineConverterFlag) != 0) {
				if ((descriptionValue & fullColorFlag) != 0)
					return PrintColorConverter.FullColor;
				if ((descriptionValue & grayScaleColorFlag) != 0)
					return PrintColorConverter.GrayScaleColor;
				if ((descriptionValue & blackAndWhiteColorFlag) != 0)
					return PrintColorConverter.BlackAndWhiteColor;
			}
			PrintColorConverter converter = CreateColorConverter(descriptionValue);
			converter.ApplyFlags = (ApplyToFlags)(descriptionValue & (int)ApplyToFlags.All);
			return converter;
		}
		static PrintColorConverter CreateColorConverter(int descriptionValue) {
			if ((descriptionValue & fullColorFlag) != 0)
				return new PrintColorConverter();
			if ((descriptionValue & grayScaleColorFlag) != 0)
				return new GrayScalePrintColorConverter();
			return new BlackAndWhitePrintColorConverter();
		}
		static string ConvertValueToString(object value) {
			if (value == null)
				return null;
			PrintColorConverter colorConverter = value as PrintColorConverter;
			if (colorConverter == null)
				return null;
			return GetDescriptionValue(colorConverter).ToString();
		}
		static object ConvertStringToValue(string str) {
			int val = Convert.ToInt32(str);
			return CreateFromDescriptionValue(val);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType.Equals(typeof(string)))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType.Equals(typeof(string)))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if (destinationType.Equals(typeof(string)))
				return ConvertValueToString(value);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			string stringValue = value as string;
			if (stringValue != null)
				return ConvertStringToValue(stringValue);
			return base.ConvertFrom(context, culture, value);
		}
	}
}
