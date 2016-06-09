#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
namespace DevExpress.DashboardWin.Design {
	public class DashboardSourceConvertor : TypeConverter {
		public const string None = "(none)";
		public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType) {
			return sourceType == typeof(string) || sourceType == typeof(Uri) || sourceType == typeof(Type);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || destinationType == typeof(object) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("DashboardSourceConvertor: destinationType");
			}
			if(destinationType == typeof(string))
				return DashboardSourceToString(value);
			string str = value as string;
			if(str != null && destinationType == typeof(object) && !IsXmlPath(str))
				return Type.GetType(str);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			return value;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return false;
		}
		string DashboardSourceToString(object source) {
			if(source == null || object.Equals(string.Empty, source))
				return None;
			string str = source as string;
			if(str != null)
				return str;
			Uri uri = source as Uri;
			if(uri != null)
				return uri.AbsolutePath;
			Type type = source as Type;
			if(type != null)
				return type.FullName;
			return source.ToString();
		}
		bool IsXmlPath(string str) {
			return str.Contains("/") || str.Contains("\\") || str.EndsWith(".xml");
		}
	}
}
