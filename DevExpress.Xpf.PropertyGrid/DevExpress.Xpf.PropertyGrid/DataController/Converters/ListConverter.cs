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

using DevExpress.XtraVerticalGrid.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public interface IListItem {
		int Index { get; }
	}
	public class ListConverter : NewInstanceConverter {
		bool CanCreateNewInstance { get; set; }
		public ListConverter(bool canCreateNewInstance) {
			CanCreateNewInstance = canCreateNewInstance;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			if (!CanCreateNewInstance)
				return GetBaseConverter(context).Return(x => x.GetStandardValuesSupported(context), () => false);
			return base.GetStandardValuesSupported(context);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if (!CanCreateNewInstance)
				return GetBaseConverter(context).Return(x => x.GetStandardValues(context), () => null);
			return base.GetStandardValues(context);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			if (!CanCreateNewInstance)
				return GetBaseConverter(context).Return(x => x.GetStandardValuesExclusive(context), () => false);
			return base.GetStandardValuesExclusive(context);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == null)
				throw new ArgumentNullException("destinationType");
			return GetBaseConverter(context).Return(x => x.ConvertTo(context, culture, value, destinationType), () => "(Collection)");
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			IList list = value as IList;
			if (list != null) {
				int length = list.Count;
				Type type = list.GetType();
				Type baseListElementType = GetElementType(type);
				for (int i = 0; i < length; i++) {
					Type elementType = list[i].Return(x => x.GetType(), () => baseListElementType);
					properties.Add(new ListItemPropertyDescriptor(type, elementType, i));
				}
			}
			return new PropertyDescriptorCollection(properties.ToArray());
		}
		public static bool IsNewItemProperty(PropertyDescriptor propertyDescriptor) {
			return propertyDescriptor is NewListItemPropertyDescriptor;
		}
		public static bool IsItemProperty(PropertyDescriptor propertyDescriptor) {
			return propertyDescriptor is ListItemPropertyDescriptor;
		}
		internal static PropertyDescriptor CreateNewListItemPropertyDescriptor(DescriptorContext context) {
			IList list = context.Value as IList;
			if (list == null)
				return null;
			Type type = list.GetType();
			Type baseListElementType = GetElementType(type);
			return new NewListItemPropertyDescriptor(type, baseListElementType);
		}
		static ConstructorInfo GetPublicConstructorInfo(Type elementType) {
			return elementType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
		}
		public static Type GetElementType(Type type) {
			if (type == typeof(object))
				return typeof(object);
			if (type.HasElementType)
				return type.GetElementType();
			if (type.IsGenericType)
				return type.GetGenericArguments()[0];
			return GetElementType(type.BaseType);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return context.Instance != null;
		}
		protected class NewListItemPropertyDescriptor : TypeConverter.SimplePropertyDescriptor {
			TypeConverter converter;
			public NewListItemPropertyDescriptor(Type arrayType, Type elementType)
				: base(arrayType, "New", elementType) {
			}
			public override void SetValue(object instance, object value) {
				IList list = instance as IList;
				list.Add(value);
				this.OnValueChanged(instance, EventArgs.Empty);
			}
			public override object GetValue(object instance) {
				return null;
			}
			public override TypeConverter Converter {
				get {
					if (converter == null)
						converter = CreateConverter();
					return converter;
				}
			}
			TypeConverter CreateConverter() {
				return DataController.NewInstanceConverter;
			}
		}
		protected class ListItemPropertyDescriptor : TypeConverter.SimplePropertyDescriptor, IListItem {
			int index;
			public ListItemPropertyDescriptor(Type listType, Type elementType, int index)
				: base(listType, "[" + index.ToString() + "]", elementType, null) {
				this.index = index;
			}
			public override object GetValue(object instance) {
				if (instance is IList) {
					IList list = (IList)instance;
					if (index < list.Count)
						return list[index];
				}
				return null;
			}
			public override void SetValue(object instance, object value) {
				if (!(instance is IList))
					return;
				IList list = (IList)instance;
				if (index < list.Count)
					list[index] = value;
				this.OnValueChanged(instance, EventArgs.Empty);
			}
			int IListItem.Index { get { return index; } }
		}
	}
}
