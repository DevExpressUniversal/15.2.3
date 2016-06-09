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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using DevExpress.Data;
using System.Runtime.Serialization;
using System.ComponentModel;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data.Browsing;
using System.Security;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Utils;
using DevExpress.Mvvm.Native;
using System.Text;
using DevExpress.Entity.Model;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
#endif
namespace DevExpress.Mvvm.UI.Native.ViewGenerator {
	[SecuritySafeCritical]
	public static class DataColumnAttributesExtensions {
		#region inner classes
		class AttributeInstanceInitializer : IInstanceInitializer {
			readonly InstanceInitializerAttributeBase[] attributes;
			readonly TypeInfo[] typeInfo;
			public AttributeInstanceInitializer(IEnumerable<InstanceInitializerAttributeBase> attributes) {
				this.attributes = attributes.ToArray();
				this.typeInfo = attributes.OrderBy(x => x.Name).Select(x => new TypeInfo(x.Type, x.Name)).ToArray();
			}
			IEnumerable<TypeInfo> IInstanceInitializer.Types {
				get { return typeInfo; }
			}
			object IInstanceInitializer.CreateInstance(TypeInfo type) {
				InstanceInitializerAttributeBase attribute = attributes.FirstOrDefault(x => x.Name == type.Name && x.Type == type.Type);
				if (attribute == null)
					throw new ArgumentException("type");
				return attribute.CreateInstance();
			}
		}
		#endregion
		public static PropertyValidator CreatePropertyValidator(PropertyDescriptor property, Type ownerType) {
			return PropertyValidator.FromAttributes(DataColumnAttributesProvider.GetAttributesCore(property, ownerType ?? property.ComponentType), property.Name);
		}
#if !SL
		public static TypeConverter GetActualTypeConverter(PropertyDescriptor property, Type ownerType, TypeConverter converter) {
			return DataColumnAttributesProvider.GetAttributes(property, ownerType).ReadAttributeProperty<TypeConverterWrapperAttribute, TypeConverter>(x => x.WrapTypeConverter(converter), converter).Value;
		}
#endif
		public static DataFormGroupAttribute DataFormGroupAttribute(this DataColumnAttributes attribute) {
			return attribute.ReadAttributeProperty<DataFormGroupAttribute, DataFormGroupAttribute>(x => x).Value;
		}
		public static MaskAttributeBase Mask(this DataColumnAttributes attribute) {
			return attribute.ReadAttributeProperty<MaskAttributeBase, MaskAttributeBase>(x => x).Value;
		}
		public static string ToolBarPageName(this DataColumnAttributes attributes) {
			var attr = attributes.ReadAttributeProperty<ToolBarItemAttribute, ToolBarItemAttribute>(x => x);
			return attributes.ReadAttributeProperty(attr, x => x.Page) ?? attributes.GroupName;
		}
		public static bool IsContextMenuItem(this DataColumnAttributes attributes) {
			var attr = attributes.ReadAttributeProperty<ContextMenuItemAttribute, ContextMenuItemAttribute>(x => x);
			return attributes.ReadAttributeProperty(attr, x => true);
		}
		public static IInstanceInitializer InstanceInitializer(this DataColumnAttributes attrib) {
			return attrib.InstanceInitializerCore<InstanceInitializerAttribute>();
		}
		public static IInstanceInitializer NewItemInstanceInitializer(this DataColumnAttributes attrib) {
			return attrib.InstanceInitializerCore<NewItemInstanceInitializerAttribute>();
		}
		static IInstanceInitializer InstanceInitializerCore<TInstanceInitializerAttribute>(this DataColumnAttributes attrib)
			where TInstanceInitializerAttribute : InstanceInitializerAttributeBase {
			TInstanceInitializerAttribute[] attrs = attrib.GetAttributeValues<TInstanceInitializerAttribute, TInstanceInitializerAttribute>(x => x);
			return attrs.Length > 0 ? new AttributeInstanceInitializer(attrs) : null;
		}
		public static string CommandParameterName(this DataColumnAttributes attributes) {
			return attributes.ReadAttributeProperty<CommandParameterAttribute, string>(x => x.CommandParameter).Value;
		}
		public static PropertyDataType PropertyDataType(this DataColumnAttributes attributes) {
			var dxDataTypeValue = attributes.ReadAttributeProperty<DXDataTypeAttribute, PropertyDataType>(x => x.DataType);
			return dxDataTypeValue.Value != default(PropertyDataType) ? dxDataTypeValue.Value : DataAnnotationsAttributeHelper.FromDataType(attributes.DataTypeValue);
		}
		public static int MaxLength(this DataColumnAttributes attribute) {
			var dxMaxLengthValue = attribute.ReadAttributeProperty<DXMaxLengthAttribute, int>(x => x.Length);
			if (dxMaxLengthValue.Value > 0)
				return dxMaxLengthValue.Value;
			if (attribute.MaxLength2Value > 0)
				return attribute.MaxLength2Value;
			return attribute.MaxLengthValue;
		}
		public static bool Required(this DataColumnAttributes attribute) {
			return attribute.RequiredValue || attribute.ReadAttributeProperty<DXRequiredAttribute, bool>(x => true).Value;
		}
		public static bool Hidden(this DataColumnAttributes attribute) {
			return attribute.ReadAttributeProperty<HiddenAttribute, bool>(x => true).Value;
		}
		public static DXImageAttribute DXImage(this DataColumnAttributes attribute) {
			return attribute.ReadAttributeProperty<DXImageAttribute, DXImageAttribute>(x => x).Value;
		}
		public static ImageAttribute Image(this DataColumnAttributes attribute) {
			return attribute.ReadAttributeProperty<ImageAttribute, ImageAttribute>(x => x).Value;
		}
		public static string GetGroupName(this DataColumnAttributes attributes, LayoutType layoutType) {
			var dataFormGroupAttributeValue = attributes.ReadAttributeProperty<DataFormGroupAttribute, DataFormGroupAttribute>(x => x);
			var tableGroupAttributeValue = attributes.ReadAttributeProperty<TableGroupAttribute, TableGroupAttribute>(x => x);
			if (layoutType == LayoutType.DataForm && dataFormGroupAttributeValue.Value != null)
				return attributes.ReadAttributeProperty(dataFormGroupAttributeValue, x => x.GroupName);
			if (layoutType == LayoutType.Table && tableGroupAttributeValue.Value != null)
				return attributes.ReadAttributeProperty(tableGroupAttributeValue, x => x.GroupName);
			return attributes.GroupName;
		}
		public static int? GetOrder(this DataColumnAttributes attributes, LayoutType layoutType) {
			var dataFormGroupAttributeValue = attributes.ReadAttributeProperty<DataFormGroupAttribute, DataFormGroupAttribute>(x => x);
			var tableGroupAttributeValue = attributes.ReadAttributeProperty<TableGroupAttribute, TableGroupAttribute>(x => x);
			var toolBarItemAttributeAttributeValue = attributes.ReadAttributeProperty<ToolBarItemAttribute, ToolBarItemAttribute>(x => x);
			var contextMenuItemAttributeValue = attributes.ReadAttributeProperty<ContextMenuItemAttribute, ContextMenuItemAttribute>(x => x);
			if (layoutType == LayoutType.DataForm && dataFormGroupAttributeValue.Value != null)
				return attributes.ReadAttributeProperty(dataFormGroupAttributeValue, x => x.Order);
			if (layoutType == LayoutType.Table && tableGroupAttributeValue.Value != null)
				return attributes.ReadAttributeProperty(tableGroupAttributeValue, x => x.Order);
			if (layoutType == LayoutType.ToolBar && toolBarItemAttributeAttributeValue.Value != null)
				return attributes.ReadAttributeProperty(toolBarItemAttributeAttributeValue, x => x.GetOrder());
			if (layoutType == LayoutType.ContextMenu && contextMenuItemAttributeValue.Value != null)
				return attributes.ReadAttributeProperty(contextMenuItemAttributeValue, x => x.GetOrder());
			return attributes.Order;
		}
		public static string GetToolBarPageGroupName(this DataColumnAttributes attributes, LayoutType layoutType) {
			var toolBarItemAttributeAttributeValue = attributes.ReadAttributeProperty<ToolBarItemAttribute, ToolBarItemAttribute>(x => x);
			var contextMenuItemAttributeValue = attributes.ReadAttributeProperty<ContextMenuItemAttribute, ContextMenuItemAttribute>(x => x);
			if (layoutType == LayoutType.ToolBar && toolBarItemAttributeAttributeValue.Value != null)
				return attributes.ReadAttributeProperty(toolBarItemAttributeAttributeValue, x => x.PageGroup);
			if (layoutType == LayoutType.ContextMenu && contextMenuItemAttributeValue.Value != null)
				return attributes.ReadAttributeProperty(contextMenuItemAttributeValue, x => x.Group);
			return null;
		}
		public static bool ScaffoldDetailCollection(this DataColumnAttributes attributes) {
			return attributes.ReadAttributeProperty<ScaffoldDetailCollectionAttribute, bool>(x => x.Scaffold, ScaffoldDetailCollectionAttribute.DefaultScaffold).Value;
		}
	}
}
