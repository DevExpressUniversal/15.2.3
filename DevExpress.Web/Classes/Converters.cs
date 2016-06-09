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
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class DxObjectConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value != null) {
				if(value is string) {
					string stringVal = ((string)value).Trim();
					char[] separator = new char[] { ':' };
					string[] stringArray = stringVal.Split(separator, 2);
					if(stringArray.Length == 2) {
						Type objectType = Type.GetType(stringArray[0]);
						string valueString = stringArray[1];
						if(objectType.FullName == "System.Double" || objectType.FullName == "System.Single")
							valueString = DataUtils.FixFloatingPoint(valueString, culture);
						return TypeDescriptor.GetConverter(objectType).ConvertFrom(context, culture, valueString);
					}
					return null;
				}
				return base.ConvertFrom(context, culture, value);
			}
			return null;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) {
				if(value != null)
					return value.GetType().FullName + ":" + value.ToString();
				else
					return "";
			}
			else
				return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class TypeTypeConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(String);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			Type type = null;
			if(value is String)
				type = Type.GetType(value.ToString());
			if(type == null)
				throw GetConvertFromException(value);
			return type;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(GetSupportedTypes());
		}
		protected virtual Type[] GetSupportedTypes() {
			return new Type[] { };
		}
	}
	public abstract class ComponentIDConverter : TypeConverter {
		public ComponentIDConverter() {
		}
		protected abstract bool IsValidComponentType(IComponent component);
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null)
				return String.Empty;
			if(value.GetType() == typeof(string))
				return (string)value;
			throw GetConvertFromException(value);
		}
		public object[] GetComponentNames(ITypeDescriptorContext context, object ownComponent) {
			object[] names = null;
			if(context != null) {
				ArrayList list = new ArrayList();
				IContainer container = context.Container;
				if(container != null) {
					ComponentCollection components = container.Components;
					foreach(IComponent comp in components) {
						if(ownComponent != null && comp == ownComponent)
							continue;
						if(IsValidComponentType(comp) && !Marshal.IsComObject(comp)) {
							PropertyDescriptor pd = TypeDescriptor.GetProperties(comp)["Modifiers"];
							if(pd != null) {
								MemberAttributes attr = (MemberAttributes)pd.GetValue(comp);
								if((attr & MemberAttributes.AccessMask) == MemberAttributes.Private) {
									continue;
								}
							}
							ISite site = comp.Site;
							if(site != null && site.Name != null) {
								list.Add(site.Name);
							}
						}
					}
				}
				names = list.ToArray();
				Array.Sort(names, Comparer.Default);
			}
			return names;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			object[] names = GetComponentNames(context, context.Instance);
			return new StandardValuesCollection(names);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class ComponentIDConverter<T> : ComponentIDConverter where T : IComponent {
		protected override bool IsValidComponentType(IComponent component) {
			return component is T;
		}
	}
	public class StartDateEditIDConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			object[] names = null;
			DateEditRangeSettings dateRangeSettings = context.Instance as DateEditRangeSettings;
			if (dateRangeSettings != null) {
				object dateRangeSettingsOwner = dateRangeSettings.OwnerProperties.GetOwner();
				if (dateRangeSettingsOwner is ASPxDateEdit)
					names = new ComponentIDConverter<ASPxDateEdit>().GetComponentNames(context, dateRangeSettingsOwner);
				else if (dateRangeSettingsOwner is IDateEditIDResolver)
					names = ((IDateEditIDResolver)dateRangeSettingsOwner).GetPossibleDataItemNames();
			}
			return new StandardValuesCollection(names);
		}
	}
	public class StringListConverter : CollectionConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return ((destinationType == typeof(string[])) || base.CanConvertTo(context, destinationType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object sourceObj) {
			if(sourceObj is string)
				return TrimAllStrings(((string)sourceObj).Split(new char[] { ',' }));
			return base.ConvertFrom(context, culture, sourceObj);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object destinationObj, Type destinationType) {
			if(destinationObj is string[]) {
				string str = string.Join(", ", (string[])destinationObj);
				if(destinationType == typeof(InstanceDescriptor))
					return new string[] { str };
				else if(destinationType == typeof(string))
					return str;
			}
			return base.ConvertTo(context, culture, destinationObj, destinationType);
		}
		private string[] TrimAllStrings(string[] strArray) {
			List<string> ret = new List<string>();
			for(int i = 0; i < strArray.Length; i++)
				ret.Add(strArray[i].Trim());
			return ret.ToArray();
		}
	}
}
