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
using System.Collections;
using System.Dynamic;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class UnitypeDataPropertyDescriptor : PropertyDescriptor {
		Dictionary<Type, PropertyDescriptor> descriptorCache = new Dictionary<Type, PropertyDescriptor>();
		bool isReadOnly;
		public UnitypeDataPropertyDescriptor(PropertyDescriptor propertyDescriptor)
			: this(propertyDescriptor.Name, propertyDescriptor.IsReadOnly) {
				if(propertyDescriptor is DevExpress.Data.Access.ExpandoPropertyDescriptor)
					descriptorCache.Add(typeof(ExpandoObject), propertyDescriptor);
				else if(propertyDescriptor.ComponentType != null && propertyDescriptor.ComponentType != typeof(object))
					descriptorCache.Add(propertyDescriptor.ComponentType, propertyDescriptor);
		}
		public UnitypeDataPropertyDescriptor(string propName, bool isReadOnly)
			: base(propName, null) {
			this.isReadOnly = isReadOnly;
		}
		public override Type PropertyType { get { return typeof(object); } }
		public override bool IsReadOnly { get { return isReadOnly; } }
		public override string Category { get { return string.Empty; } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override bool ShouldSerializeValue(object component) { return false; }
		public override object GetValue(object component) {
			PropertyDescriptor descriptor = GetProperyDescriptor(component);
			if(descriptor != null)
				return descriptor.GetValue(component);
			return null;
		}
		public override void SetValue(object component, object value) {
			PropertyDescriptor descriptor = GetProperyDescriptor(component);
			if(descriptor != null)
				descriptor.SetValue(component, ConvertValue(value, descriptor));
		}
		protected internal virtual PropertyDescriptor GetProperyDescriptor(object component) {
			if(component == null)
				return null;
			Type componentType = component.GetType();
			PropertyDescriptor descriptor = null;
			if(descriptorCache.TryGetValue(componentType, out descriptor))
				return descriptor;
			descriptor = TypeDescriptor.GetProperties(componentType)[Name];
			if(descriptor != null)
				descriptorCache[componentType] = descriptor;
			return descriptor;
		}
		protected Type GetDataType(PropertyDescriptor descriptor) {
			Type dataType = descriptor.PropertyType;
			if(dataType != null) {
				Type nullableUnderlyingType = Nullable.GetUnderlyingType(dataType);
				if(nullableUnderlyingType != null)
					dataType = nullableUnderlyingType;
			}
			return dataType;
		}
		protected virtual object ConvertValue(object val, PropertyDescriptor descriptor) {
			Type dataType = GetDataType(descriptor);
			bool isNull = val == null || DBNull.Value.Equals(val);
			Type valueType = !isNull ? val.GetType() : null;
			if(isNull || dataType == null || dataType.IsAssignableFrom(valueType))
				return val;
			System.ComponentModel.TypeConverter conv = descriptor != null ? descriptor.Converter : null;
			if(conv != null && conv.CanConvertFrom(valueType)) {
				try {
					return conv.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, val);
				} catch { }
			}
			return Convert.ChangeType(val, dataType, System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}
