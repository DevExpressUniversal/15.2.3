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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Security;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWeb.Native {
	public class DashboardSourceTypeConverter : TypeConverter {
		public DashboardSourceTypeConverter() { }
		public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType) {
			return sourceType == typeof(string) || sourceType == typeof(Type);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || destinationType == typeof(object) || destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			return value;
		}
		public static object DashboardSourceFromString(ASPxDashboardViewer viewer, string str, IServiceProvider provider) {
			if(str == null)
				return null;
			else {
				if(IsXmlPath(str))
					return str;
				try {
					 Assembly asm = viewer == null || viewer.Page == null ? null : viewer.Page.GetType().Assembly;
					object type = AsmHelper.GetType(asm, str);
					if(type != null)
						return type;
					if(provider != null)
						return GetTypeFromService(str, provider);
				} catch {
				}
			}
			return str;
		}
		internal static object GetTypeFromService(string str, IServiceProvider provider) {
			if(provider == null)
				return str;
			ITypeDiscoveryService service = provider.GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService;
			if(service != null)
				foreach(Type ntype in service.GetTypes(typeof(Dashboard), false))
					if(ntype.FullName == str)
						return ntype;
			return str;
		}
		public static bool IsXmlPath(string str) {
			return str.Contains("/") || str.Contains("\\") || str.StartsWith("~") || str.EndsWith(".xml");
		}
		[SecuritySafeCritical]
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			Type type = value as Type;
			if(destinationType == typeof(string)) {
				return DashboardSourceToString(value);
			}
			string str = value as string;
			if(str != null && destinationType == typeof(object) && !IsXmlPath(str))
				return Type.GetType(str);
			if(destinationType == typeof(InstanceDescriptor)) {
				if(str != null)
					return new InstanceDescriptor(typeof(string).GetConstructor(new Type[] { typeof(char[]) }), new object[] { str.ToCharArray() });
				if(type != null)
					return new InstanceDescriptor(type.GetType().GetConstructor(new Type[] { }), new object[] { });
				return null;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public static string DashboardSourceToString(object source) {
			if(source == null || object.Equals(string.Empty, source))
				return null;
			string str = source as string;
			if(str != null)
				return str;
			Type type = source as Type;
			if(type != null)
				return type.FullName;
			return source.ToString();
		}
	}
}
