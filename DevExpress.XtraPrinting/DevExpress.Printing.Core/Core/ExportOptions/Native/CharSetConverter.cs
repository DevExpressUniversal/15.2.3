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
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.ComponentModel;
#if SILVERLIGHT
using DevExpress.Xpf.Collections;
using DevExpress.Xpf.ComponentModel;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class CharSetConverter : TypeConverter {
		private static SortedList charSetList = new SortedList();
		static CharSetConverter() {
			charSetList.Add("Arabic (Windows)", "windows-1256");
			charSetList.Add("Baltic (ISO)", "iso-8859-4");
			charSetList.Add("Baltic (Windows)", "windows-1257");
			charSetList.Add("Central European (ISO)", "iso-8859-2");
			charSetList.Add("Central European (Windows)", "windows-1250");
			charSetList.Add("Cyrillic (ISO)", "iso-8859-5");
			charSetList.Add("Cyrillic (KOI8-R)", "koi8-r");
			charSetList.Add("Cyrillic (Windows)", "windows-1251");
			charSetList.Add("Latin 9 (ISO)", "iso-8859-15");
			charSetList.Add("Unicode (UTF-7)", "utf-7");
			charSetList.Add("Unicode (UTF-8)", "utf-8");
			charSetList.Add("Western European (ISO)", "iso-8859-1");
			charSetList.Add("Western European (Windows)", "windows-1252");
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
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(charSetList.Values);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				if(charSetList.ContainsValue(value))
					return charSetList.GetKey(charSetList.IndexOfValue(value));
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string && charSetList.ContainsKey(value))
				return charSetList[value];
			return base.ConvertFrom(context, culture, value);
		}
	}
}
