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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.Data;
using DevExpress.Utils;
using System.Collections;
namespace DevExpress.XtraReports.Design {
	public class ParameterValueEditorChangingConverter : TypeConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, attributes);
			List<PropertyDescriptor> newProperties = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor property in properties) {
				newProperties.Add((property.Name == "Value") ?
					CreatePropertyDescriptor(context, property, (IParameter)value) :
					property);
			}
			return new PropertyDescriptorCollection(newProperties.ToArray(), true);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
		static PropertyDescriptor CreatePropertyDescriptor(ITypeDescriptorContext context, PropertyDescriptor oldProperty, IParameter parameter) {
			IDesignerHost host = context.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host != null) {
				IExtensionsProvider extensionProvider = host.RootComponent as IExtensionsProvider;
				if(extensionProvider != null) {
					EditingContext editingContext = new EditingContext(extensionProvider.Extensions[DataEditorService.Guid], extensionProvider);
					if(DataEditorService.GetRepositoryItem(parameter.Type, parameter, editingContext) != null)
						return new ParameterValuePropertyDescriptor2(oldProperty, parameter.Type);
				}
			}
			return new ParameterValuePropertyDescriptor(oldProperty, parameter.Type, IsMultiValue(parameter));
		}
		static bool IsMultiValue(IParameter parameter) { 
			IMultiValueParameter mvParameter = parameter as IMultiValueParameter;
			return mvParameter != null && mvParameter.MultiValue;
		}
	}
	class ParameterValuePropertyDescriptor2 : PropertyDescriptorWrapper {
		class DropDownTypeEditor : UITypeEditor {
			public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
				return UITypeEditorEditStyle.DropDown;
			}
		}
		Type parameterType;
		public ParameterValuePropertyDescriptor2(PropertyDescriptor oldPropertyDescriptor, Type parameterType)
			: base(oldPropertyDescriptor) {
			this.parameterType = parameterType;
		}
		public override Type PropertyType {
			get {
				return parameterType;
			}
		}
		public override object GetEditor(Type editorBaseType) {
			if(editorBaseType == typeof(System.Drawing.Design.UITypeEditor)) {
				return new DropDownTypeEditor();
			}
			return base.GetEditor(editorBaseType);
		}
		public override TypeConverter Converter {
			get {
				return TypeDescriptor.GetConverter(parameterType);
			}
		}
	}
	class ParameterValuePropertyDescriptor : PropertyDescriptorWrapper {
		Type parameterType;
		bool multiValue;
		public ParameterValuePropertyDescriptor(PropertyDescriptor oldPropertyDescriptor, Type parameterType, bool multiValue)
			: base(oldPropertyDescriptor) {
			this.parameterType = parameterType;
			this.multiValue = multiValue;
		}
		public override object GetEditor(Type editorBaseType) {
			if(editorBaseType == typeof(System.Drawing.Design.UITypeEditor)) {
				return multiValue ? null : TypeDescriptor.GetEditor(parameterType, editorBaseType);
			}
			return base.GetEditor(editorBaseType);
		}
		public override TypeConverter Converter {
			get {
				TypeConverter baseConverter = parameterType == typeof(bool) ? 
					new DevExpress.Utils.Design.BooleanTypeConverter() :
					TypeDescriptor.GetConverter(parameterType);
				return multiValue ? new MultiValueConverter(parameterType, baseConverter) : baseConverter;
			}
		}
	}
	public class ParameterTypeConverter : TypeConverter {
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
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && value is Type) {
				string contextName = GetContextName(context);
				return ParameterEditorService.GetDisplayNameByParameterType(contextName, (Type)value);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string) {
				string contextName = GetContextName(context);
				return ParameterEditorService.GetParameterTypeByDisplayName(contextName, (string)value);
			}
			return base.ConvertFrom(context, culture, value);
		}
		protected virtual string GetContextName(ITypeDescriptorContext context) {
			string contextName = null;
			IDesignerHost host = context.GetService<IDesignerHost>();
			if(host != null) {
				IExtensionsProvider extensionsProvider = host.RootComponent as IExtensionsProvider;
				if(extensionsProvider != null)
					contextName = extensionsProvider.Extensions[GetExtensionKey()];
			}
			return contextName;
		}
		protected virtual string GetExtensionKey() {
			return ParameterEditorService.Guid;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context != null) {
				string contextName = GetContextName(context);
				Dictionary<Type, string> dictionary = ParameterEditorService.GetParameterTypes(contextName);
				return new StandardValuesCollection(dictionary.Keys.ToArray());
			}
			return new StandardValuesCollection(new Type[] { });
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
	}
	enum LookUpSettingsType {
		NoLookUp,
		StaticList,
		DynamicList
	}
	class NamedLookUpSettingsType {
		public LookUpSettingsType Type { get; private set; }
		public NamedLookUpSettingsType(LookUpSettingsType type) {
			this.Type = type;
		}
		public override string ToString() {
			return new ParameterLookUpSettingsConverter().ConvertToString(this);
		}
	}
	public class ParameterLookUpSettingsEditor : ObjectPickerEditor {
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			Array enumValues = Enum.GetValues(typeof(LookUpSettingsType));
			List<NamedLookUpSettingsType> values = new List<NamedLookUpSettingsType>();
			foreach(LookUpSettingsType enumValue in enumValues) {
				values.Add(new NamedLookUpSettingsType(enumValue));
			}
			return new PickerFromValuesControl(this, value, values, false);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			object newValue = base.EditValue(context, provider, value);
			if(newValue == null)
				return null;
			LookUpSettings settings = newValue as LookUpSettings;
			if(settings != null)
				return settings;
			NamedLookUpSettingsType type = (NamedLookUpSettingsType)newValue;
			return LookUpSettingsFromType(type.Type);
		}
		LookUpSettings LookUpSettingsFromType(LookUpSettingsType? type) {
			if(type == null)
				return null;
			switch(type) {
				case LookUpSettingsType.NoLookUp: return null;
				case LookUpSettingsType.StaticList: return new StaticListLookUpSettings();
				case LookUpSettingsType.DynamicList: return new DynamicListLookUpSettings();
			}
			throw new NotSupportedException();
		}
	}
	public class ParameterLookUpSettingsConverter : ExpandableObjectConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) {
				NamedLookUpSettingsType type = value as NamedLookUpSettingsType;
				if(value == null || (type != null && type.Type == LookUpSettingsType.NoLookUp))
					return PreviewLocalizer.GetString(PreviewStringId.ParameterLookUpSettingsNoLookUp);
				if(value is StaticListLookUpSettings || (type != null && type.Type == LookUpSettingsType.StaticList))
					return GetDisplayNameByAttribute<StaticListLookUpSettings>();
				if(value is DynamicListLookUpSettings || (type != null && type.Type == LookUpSettingsType.DynamicList))
					return GetDisplayNameByAttribute<DynamicListLookUpSettings>();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		static string GetDisplayNameByAttribute<T>() {
			DXDisplayNameAttribute attribute = TypeDescriptor.GetAttributes(typeof(T))[typeof(DXDisplayNameAttribute)] as DXDisplayNameAttribute;
			return attribute != null ? attribute.DisplayName : null;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			IPropertyFilterService serv = GetService(context, typeof(IPropertyFilterService)) as IPropertyFilterService;
			if(serv == null) return collection;
			Dictionary<string, PropertyDescriptor> properties = new Dictionary<string, PropertyDescriptor>(collection.Count);
			foreach(PropertyDescriptor item in collection)
				properties[item.Name] = item;
			serv.PreFilterProperties(properties, value);
			return new PropertyDescriptorCollection(properties.Values.ToArray<PropertyDescriptor>());
		}
		static object GetService(ITypeDescriptorContext context, Type serviceType) {
			return context != null && context.Container is IServiceProvider ?
				((IServiceProvider)context.Container).GetService(typeof(IPropertyFilterService)) :
				null;
		}
	}
	public interface IPropertyFilterService {
		void PreFilterProperties(IDictionary properties, object component);
		void PreFilterEvents(IDictionary events, object component);
	}
}
