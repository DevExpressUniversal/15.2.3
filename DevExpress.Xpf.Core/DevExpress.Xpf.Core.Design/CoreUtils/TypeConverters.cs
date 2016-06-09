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
using System.Reflection;
using System.Collections.Generic;
namespace DevExpress.Xpf.Core.Design {
	public class TypeTypeConverter : TypeConverter {
		internal static Type GetTypeByName(string typeName, Type baseType = null) {
			Type typeByFullName = Type.GetType(typeName);
			if(typeByFullName != null)
				return typeByFullName;
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				Type typByNameWithNamespace = assembly.GetType(typeName);
				if(typByNameWithNamespace != null && IsAppropriateBaseType(typByNameWithNamespace, baseType))
					return typByNameWithNamespace;
			}
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				try {
					foreach(Type type in assembly.GetTypes()) {
						if(type.IsPublic && type.Name == typeName && IsAppropriateBaseType(type, baseType))
							return type;
					}
				} catch {
				}
			}
			return null;
		}
		static bool IsAppropriateBaseType(Type type, Type baseType) {
			return baseType == null || type.BaseType == baseType;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value != null && value is string) {
				return GetTypeByName((string)value);
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public abstract class InheritedTypeTypeConverter : TypeConverter {
		public abstract Type BaseType { get; }
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<Type> result = new List<Type>();
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				foreach(Type type in assembly.GetTypes()) {
					if(IsAppropriateType(type))
						result.Add(type);
				}
			}
			return new TypeConverter.StandardValuesCollection(result);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string) {
				Type type = TypeTypeConverter.GetTypeByName((string)value, BaseType);
				if(type != null && IsAppropriateType(type))
					return Activator.CreateInstance(type);
			}
			return base.ConvertFrom(context, culture, value);
		}
		bool IsAppropriateType(Type type) {
			if(!type.IsAbstract && !type.IsInterface && type.BaseType != null && type.BaseType == BaseType && !type.FullName.Contains("DevExpress")) {
				ConstructorInfo ci = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[0], new ParameterModifier[0]);
				return ci != null;
			}
			return false;
		}
	}
}
