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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
namespace DevExpress.Entity.Model.Metadata {
	public class PropertyAccessor {
		object source = null;
		Type sourceType;
		PropertyAccessor(string name) {
			Name = name;
		}
		public PropertyAccessor(object source, string name)
			: this(name) {
			this.source = source;
			if (source != null)
				this.sourceType = source.GetType();
		}
		public PropertyAccessor(string name, Type sourceType)
			: this(name) {
			this.sourceType = sourceType;
		}
		protected PropertyInfo Property { get; set; }
		public string Name { get; private set; }
		public virtual object Value {
			get {
				if (source == null)
					return null;
				if (Property == null)
					Property = sourceType.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
						.FirstOrDefault(x=>x.Name == Name);
				if (Property == null)
					return null;
				MethodInfo getter = Property.GetGetMethod();
				if (getter != null)
					return getter.Invoke(source, null);
				return Property.GetValue(source, new object[0]);
			}
		}
		public override string ToString() {
			try {
				object value = Value;
				if (value == null)
					value = "null";
				return string.Format("{0}: {1}", Name, value.ToString());
			}
			catch {
				return base.ToString();
			}
		}
		public static bool IsComplexPropertyName(string fullName) {
			return !string.IsNullOrEmpty(fullName) && fullName.IndexOf(".") > 0;
		}
		public static object GetValue(object source, string name) {
			if (source == null)
				return null;
			PropertyInfo property = source.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
			if (property == null)
				return null;
			MethodInfo getter = property.GetGetMethod();
			if (getter == null)
				return null;
			return getter.Invoke(source, null);
		}
	}
	public class MethodAccessor {
		object source;
		Type sourceType;
		MethodInfo method;
		MethodAccessor(string name) {
			Name = name;
		}
		public MethodAccessor(object source, string name)
			: this(name) {
			this.source = source;
			if (source != null)
				this.sourceType = source.GetType();
		}
		public MethodAccessor(Type sourceType, string name)
			: this(name) {
			this.sourceType = sourceType;
		}
		public string Name { get; private set; }
		public object Invoke(Func<object[]> argumentsSource = null) {
			return Invoke(this.source, argumentsSource);
		}
		public object Invoke(object target, Func<object[]> argumentsSource = null) {
			if (method == null)
				method = sourceType.GetMethods(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
						.FirstOrDefault(x=>x.Name==Name && !x.IsGenericMethod);
			if (method == null)
				return null;
			if (argumentsSource == null)
				return method.Invoke(target, new object[0]);
			return method.Invoke(target, argumentsSource());
		}
	}
	class NestedPropertyAccessor : PropertyAccessor {
		string fullName;
		public NestedPropertyAccessor(object source, string fullName)
			: base(source, GetCurrentLevelPropertyName(fullName)) {
			this.fullName = fullName;
		}
		public NestedPropertyAccessor(string fullName, Type sourceType)
			: base(GetCurrentLevelPropertyName(fullName), sourceType) {
			this.fullName = fullName;
		}
		bool IsComplex {
			get { return IsComplexPropertyName(fullName); }
		}
		static string GetCurrentLevelPropertyName(string name) {
			int index = name.IndexOf(".");
			if (index > 0)
				return name.Substring(0, index);
			return name;
		}
		string GetNestedPropertyName() {
			int index = fullName.IndexOf(".");
			return fullName.Remove(0, index + 1);
		}
		NestedPropertyAccessor nestedProperty;
		public override object Value {
			get {
				if (IsComplex) {
					object baseValue = base.Value;
					if (baseValue == null)
						return null;
					if (nestedProperty == null)
						nestedProperty = new NestedPropertyAccessor(baseValue, GetNestedPropertyName());
					return nestedProperty.Value;
				}
				return base.Value;
			}
		}
	}
}
