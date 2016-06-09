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
using DevExpress.Compatibility.System.ComponentModel;
#if SILVERLIGHT
using DevExpress.Xpf.ComponentModel;
using DevExpress.Data.Browsing;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
#endif
namespace DevExpress.Utils.Design {
	public class EnumTypeConverter : EnumConverter {
		protected class HashEntry {
			public string[] names;
			public DXDisplayNameAttribute[] displayNames;
			public HashEntry(Type enumType, Type resourceFinder, string resourceFile) {
				names = DevExpress.Data.Utils.Helpers.GetEnumNames(enumType);
				Array values = DevExpress.Data.Utils.Helpers.GetEnumValues(enumType);
				displayNames = new DXDisplayNameAttribute[names.Length];
				for(int i = 0; i < names.Length; i++)
					displayNames[i] = GetDisplayName(resourceFinder, resourceFile, values.GetValue(i), names[i]);
			}
			public string NameToDisplayName(string name) {
				int index = Array.IndexOf(names, name.Trim());
				string displayName = GetDisplayName(index);
				return displayName != null ? displayName.Trim() : null;
			}
			string GetDisplayName(int index) {
				return index >= 0 ? displayNames[index].DisplayName : null;
			}
			public string DisplayNameToName(string displayName) {
				displayName = displayName.Trim();
				for(int i = 0; i < displayNames.Length; i++) {
					string currentDisplayName = GetDisplayName(i);
					if(currentDisplayName != null && string.Equals(currentDisplayName.Trim(), displayName, StringComparison.CurrentCultureIgnoreCase))
						return names[i];
				}
				return null;
			}
			static DXDisplayNameAttribute GetDisplayName(Type resourceFinder, string resourceFile, object enumValue, string enumName) {
				return new DXDisplayNameAttribute(resourceFinder, resourceFile, GetResourceName(enumValue), enumName);
			}
			static string GetResourceName(object enumValue) {
				return string.Concat(enumValue.GetType().FullName, ".", enumValue);
			}
		}
		#region static
		internal static void Refresh() {
			data.Clear();
		}
		protected static Dictionary<Type, HashEntry> data = new Dictionary<Type, HashEntry>();
		static string[] GetDisplayNames(Type enumType, string[] names) {
			List<string> result = new List<string>();
			foreach(string item in names) {
				string s = GetDisplayName(enumType, item);
				if(!string.IsNullOrEmpty(s))
					result.Add(s);
			}
			return result.ToArray();
		}
		static string GetDisplayName(Type enumType, string name) {
			HashEntry entry;
			return data.TryGetValue(enumType, out entry) ? entry.NameToDisplayName(name) : null;
		}
		static string[] GetNames(Type enumType, string[] displayNames) {
			List<string> result = new List<string>();
			foreach(string item in displayNames) {
				string s = GetName(enumType, item);
				if(!string.IsNullOrEmpty(s))
					result.Add(s);
			}
			return result.ToArray();
		}
		static string GetName(Type enumType, string displayName) {
			HashEntry entry;
			return data.TryGetValue(enumType, out entry) ? entry.DisplayNameToName(displayName) : null;
		}
		protected static void Initialize(Type enumType) {
			ResourceFinderAttribute attribute = TypeDescriptor.GetAttributes(enumType)[typeof(ResourceFinderAttribute)] as ResourceFinderAttribute;
			if(attribute != null)
				Initialize(enumType, attribute.ResourceFinder, attribute.ResourceFile);
		}
		protected static void Initialize(Type enumType, Type resourceFinder) {
			Initialize(enumType, resourceFinder, DXDisplayNameAttribute.DefaultResourceFile);
		}
		protected static void Initialize(Type enumType, Type resourceFinder, string resourceFile) {
			if(!data.ContainsKey(enumType))
				data.Add(enumType, new HashEntry(enumType, resourceFinder, resourceFile));
		}
		#endregion
		public EnumTypeConverter(Type type)
			: base(type) {
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string) {
				string name = DisplayNameToName(this.EnumType, (string)value);
				value = name != null ? name : value;
			}
			return base.ConvertFrom(context, culture, value);
		}
		protected string DisplayNameToName(Type enumType, string displayName) {
			displayName = displayName.Trim();
			InitializeInternal(enumType);
			if(IsFlagsDefined(enumType)) {
				string[] names = GetNames(enumType, displayName.Split(','));
				return string.Join(",", names);
			}
			return GetName(enumType, displayName);
		}
		bool IsFlagsDefined(Type type) {
			return type.IsDefined(typeof(FlagsAttribute), false);
		}
		protected virtual void InitializeInternal(Type enumType) {
			Initialize(enumType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(context != null && destinationType == typeof(string)) {
				if(value is string)
					return value;
				string s = (string)base.ConvertTo(context, culture, value, destinationType);
				return NameToDisplayName(this.EnumType, s);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		protected string NameToDisplayName(Type enumType, string name) {
			InitializeInternal(enumType);
			if(IsFlagsDefined(enumType)) {
				string[] displayNames = GetDisplayNames(enumType, name.Split(','));
				return string.Join(",", displayNames);
			}
			return GetDisplayName(enumType, name);
		}
	}
}
