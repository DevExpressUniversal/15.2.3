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
using System.Globalization;
using System.Text;
using DevExpress.Utils.Design;
#if SILVERLIGHT
using DevExpress.Xpf.ComponentModel;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class EncodingConverter : TypeConverter {
		#region static
#if !SILVERLIGHT
		static Encoding DefaultEncoding = Encoding.Default;
#else
		static Encoding DefaultEncoding = Encoding.Unicode;
#endif
		static List<Encoding> GetStandardEncodings() {
			List<Encoding> encodings = new List<Encoding>();
#if !SILVERLIGHT
			encodings.Add(Encoding.Default);
			encodings.Add(Encoding.ASCII);
#endif
			encodings.Add(Encoding.Unicode);
			encodings.Add(Encoding.BigEndianUnicode);
#if !SILVERLIGHT
			encodings.Add(Encoding.UTF7);
#endif
			encodings.Add(Encoding.UTF8);
#if !SILVERLIGHT
			encodings.Add(Encoding.UTF32);
#endif
			return encodings;
		}
		#endregion
		public EncodingConverter() {
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string) {
				foreach(Encoding encoding in GetStandardEncodings()) {
					if(encoding.WebName == (string)value)
						return encoding;
				}
				return DefaultEncoding;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && value is Encoding) {
				return ((Encoding)value).WebName;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(GetStandardEncodings());
		}
	}
}
