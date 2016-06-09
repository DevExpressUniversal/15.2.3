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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using DevExpress.Data.Helpers;
using DevExpress.Entity.Model;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using DevExpress.Xpf.Editors.Settings;
namespace DevExpress.Xpf.Editors.Filtering {
	class PropertyDescription {
		public string FieldName { get; set; }
		public string ColumnCaption { get; set; }
		public BaseEditSettings EditSettings { get; set; }
	}
	class StandardColumnsProvider {
		readonly FrameworkElement ownerElement;
		public StandardColumnsProvider(FrameworkElement ownerElement) {
			this.ownerElement = ownerElement;
		}
		public PropertyDescription CurrentColumn { get; set; }
		public PropertyDescription GetStandardColumn(PropertyDescriptor descriptor) {
			try {
				if(descriptor == null)
					return null;
				var propertyInfo = new EdmPropertyInfo(descriptor,
					DataColumnAttributesProvider.GetAttributes(descriptor),
					false);
				var options = GenerateEditorOptions.ForRuntime();
				var context = new RuntimeEditingContext(this);
				var generator = new StandaloneColumnGenerator(context.GetRoot(), ownerElement);
				EditorsSource.GenerateEditor(propertyInfo, generator, null, options.CollectionProperties,
					options.GuessImageProperties, options.GuessDisplayMembers);
				return CurrentColumn;
			} finally {
				CurrentColumn = null;
			}
		}
		public object FindResource(object resourceKey) {
			return ownerElement.FindResource(resourceKey);
		}
	}
	class StandaloneColumnGenerator : EditorsGeneratorBase {
		readonly IModelItem definitionsProvider;
		readonly FrameworkElement ownerElement;
		public StandaloneColumnGenerator(IModelItem definitionsProvider, FrameworkElement ownerElement) {
			this.definitionsProvider = definitionsProvider;
			this.ownerElement = ownerElement;
		}
		public override EditorsGeneratorTarget Target { get { return EditorsGeneratorTarget.Unknown; } }
		protected override EditorsGeneratorMode Mode { get { return EditorsGeneratorMode.EditSettings; } }
		protected override Type GetLookUpEditType() { return null; }
		protected bool GetIsStandardValuesSupported() { return false; }
		protected object GetStandardValues() { throw new InvalidOperationException(); }
		protected bool GetIsStandardValuesExclusive() { throw new InvalidOperationException(); }
		public override void Text(IEdmPropertyInfo property, bool multiline) {
			if(!multiline && GetIsStandardValuesSupported()) {
				GenerateEditor(property, typeof(ComboBoxEditSettings), TextInitializer(property, multiline));
				return;
			}
			base.Text(property, multiline);
		}
		public override void LookUp(IEdmPropertyInfo property, string itemsSource, string displayMember, ForeignKeyInfo foreignKeyInfo) { Object(property); }
		protected internal override Initializer TextInitializer(IEdmPropertyInfo property, bool multiline, int? maxLength = null) {
			if(!multiline && GetIsStandardValuesSupported()) {
				return new Initializer((container, edit) => {
					edit.SetValue(LookUpEditSettingsBase.ItemsSourceProperty, GetStandardValues());
					edit.SetValueIfNotSet(ButtonEditSettings.IsTextEditableProperty, !GetIsStandardValuesExclusive());
					if(property.HasNullableType())
						edit.SetValueIfNotSet(ButtonEditSettings.NullValueButtonPlacementProperty, EditorPlacement.EditBox);
				});
			}
			return base.TextInitializer(property, multiline, maxLength);
		}
		protected internal override Initializer LookUpInitializer(IEdmPropertyInfo property, string itemsSource, string displayMember, ForeignKeyInfo foreignKeyInfo) { return ObjectInitializer(property); }
		void GenerateEditor(IEdmPropertyInfo property, IModelItem filterColumn, IModelItem editSettings, Initializer initializer) {
			AttributesApplier.ApplyBaseAttributesForFilterColumn(property, filterColumn);
			AttributesApplier.ApplyDisplayFormatAttributesForEditSettings(property, () => editSettings);
			if(editSettings != null) {
				initializer.SetEditProperties(filterColumn, editSettings);
				filterColumn.Properties["EditSettings"].SetValue(editSettings);
			}
			definitionsProvider.Properties["CurrentColumn"].SetValue(filterColumn);
		}
		protected override void GenerateEditor(IEdmPropertyInfo property, Type editType, Initializer initializer) {
			var filterColumn = definitionsProvider.Context.CreateItem(typeof(PropertyDescription));
			var editSettings = (editType != null
				? definitionsProvider.Context.CreateItem(editType)
				: filterColumn.Context.CreateItem(typeof(TextEditSettings)));
			GenerateEditor(property, filterColumn, editSettings, initializer);
		}
		public override void GenerateEditorFromResources(IEdmPropertyInfo property, object resourceKey, Initializer initializer) {
			var template = GetResourceTemplate(ownerElement, resourceKey);
			var content = GetResourceContent<PropertyDescription, BaseEditSettings, BaseEdit>(template);
			if(content is BaseEditSettings) {
				var filterColumn = definitionsProvider.Context.CreateItem(typeof(PropertyDescription));
				GenerateEditor(property, filterColumn, new RuntimeEditingContext(content).GetRoot(), initializer);
			} else GenerateEditor(property, null, initializer);
		}
	}
	public class PropertyInfo {
		public static PropertyInfo FromPropertyName(string propertyName) {
			return new PropertyInfo(propertyName);
		}
		public PropertyInfo() { }
		public PropertyInfo(string name, string caption = null, Type type = null) : this() {
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			Name = name;
			Caption = caption ?? SplitStringHelper.SplitPascalCaseString(Name.Replace('.', ' '));
			Type = type;
		}
		public string Name { get; set; }
		public string Caption { get; set; }
		public Type Type { get; set; }
	}
	public class PropertyInfoCollection : List<PropertyInfo> {
		public PropertyInfoCollection() { }
		public PropertyInfoCollection(IEnumerable<PropertyInfo> collection) : base(collection) { }
	}
	public class PropertyNameCollection : List<string> { 
		public PropertyNameCollection() { }
		public PropertyNameCollection(IEnumerable<string> collection) : base(collection) { }
	}
	public class FilterControlColumnsBehavior : Behavior<FilterControl> {
		class CustomPropertyDescriptor : PropertyDescriptor {
			Type type;
			public CustomPropertyDescriptor(Type type) : base("name", new Attribute[0]) {
				this.type = type;
			}
			public override bool CanResetValue(object component) { return false; }
			public override Type ComponentType { get { return typeof(object); } }
			public override object GetValue(object component) { return null; }
			public override bool IsReadOnly { get { return true; } }
			public override Type PropertyType { get { return type; } }
			public override void ResetValue(object component) { }
			public override void SetValue(object component, object value) { }
			public override bool ShouldSerializeValue(object component) { return false; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IEnumerable<string> HiddenProperties {
			get { return (IEnumerable<string>)GetValue(HiddenPropertiesProperty); }
			set { SetValue(HiddenPropertiesProperty, value); }
		}
		public static readonly DependencyProperty HiddenPropertiesProperty =
			DependencyProperty.Register("HiddenProperties", typeof(IEnumerable<string>), typeof(FilterControlColumnsBehavior), new PropertyMetadata(null, (d, e) => ((FilterControlColumnsBehavior)d).UpdateColumns()));
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PropertyInfoCollection AdditionalProperties {
			get { return (PropertyInfoCollection)GetValue(AdditionalPropertiesProperty); }
			set { SetValue(AdditionalPropertiesProperty, value); }
		}
		public static readonly DependencyProperty AdditionalPropertiesProperty =
			DependencyProperty.Register("AdditionalProperties", typeof(PropertyInfoCollection), typeof(FilterControlColumnsBehavior), new PropertyMetadata(null, (d, e) => ((FilterControlColumnsBehavior)d).UpdateColumns()));
		public Type ObjectType {
			get { return (Type)GetValue(ObjectTypeProperty); }
			set { SetValue(ObjectTypeProperty, value); }
		}
		public static readonly DependencyProperty ObjectTypeProperty =
			DependencyProperty.Register("ObjectType", typeof(Type), typeof(FilterControlColumnsBehavior), new PropertyMetadata(null, (d, e) => ((FilterControlColumnsBehavior)d).UpdateColumns()));
		public bool UpperCasePropertyNames {
			get { return (bool)GetValue(UpperCasePropertyNamesProperty); }
			set { SetValue(UpperCasePropertyNamesProperty, value); }
		}
		public static readonly DependencyProperty UpperCasePropertyNamesProperty =
			DependencyProperty.Register("UpperCasePropertyNames", typeof(bool), typeof(FilterControlColumnsBehavior), new PropertyMetadata(false, (d, e) => ((FilterControlColumnsBehavior)d).UpdateColumns()));
		public FilterControlColumnsBehavior() {
			HiddenProperties = new List<string>();
			AdditionalProperties = new PropertyInfoCollection();
		}
		protected override void OnAttached() {
			base.OnAttached();
			UpdateColumns();
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.FilterColumns = null;
		}
		string UpdateColumnCaption(string caption) {
			if(UpperCasePropertyNames)
				return caption.ToUpper();
			return caption;
		}
		protected void UpdateColumns() {
			if(AssociatedObject != null && ObjectType != null) {
			var provider = new StandardColumnsProvider(AssociatedObject);
				var properties = TypeDescriptor.GetProperties(ObjectType)
				.Cast<PropertyDescriptor>()
				.Where(x => {
					if(HiddenProperties != null && HiddenProperties.Contains(x.Name))
						return false;
					if(x.PropertyType == typeof(string))
						return true;
					if(typeof(IEnumerable).IsAssignableFrom(x.PropertyType))
						return false;
					return true;
				})
				.Select(x => {
					var column = provider.GetStandardColumn(x);
					var caption = column.ColumnCaption ?? column.FieldName;
					return CreateFilterColumn(UpdateColumnCaption(caption), column.EditSettings, x.PropertyType, column.FieldName);
				});
				if(AdditionalProperties != null) {
					properties = properties.Concat(AdditionalProperties.Select(
						x => {
							var type = x.Type ?? typeof(string);
							var column = provider.GetStandardColumn(new CustomPropertyDescriptor(type));
							return CreateFilterColumn(
								UpdateColumnCaption(x.Caption ?? x.Name),
								column.EditSettings,
								type,
								x.Name ?? string.Empty);
						}));
				}
				AssociatedObject.FilterColumns = properties;
			}
		}
		protected virtual FilterColumn CreateFilterColumn(string columnCaption, BaseEditSettings editSettings, Type columnType, string fieldName) {
			return new FilterColumn {
				ColumnCaption = columnCaption,
				EditSettings = editSettings,
				ColumnType = columnType,
				FieldName = fieldName
			};
		}
	}
	public static class FilteredComponentHelper {
		public static IEnumerable<FilterColumn> GetColumnsByItemsSource(FrameworkElement ownerElement, object itemsSource, bool upperCasePropertyNames = false) {
			return itemsSource.With(x => x.GetType().GetGenericArguments().FirstOrDefault().With(y => GetColumnsByType(ownerElement, y, upperCasePropertyNames)));
		}
		public static IEnumerable<FilterColumn> GetColumnsByType(FrameworkElement ownerElement, Type type, bool upperCasePropertyNames) {
			var provider = new StandardColumnsProvider(ownerElement);
			IEnumerable<FilterColumn> properties = TypeDescriptor.GetProperties(type)
				.Cast<PropertyDescriptor>()
				.Where(x => {
					if(x.PropertyType == typeof(string))
						return true;
					if(typeof(IEnumerable).IsAssignableFrom(x.PropertyType))
						return false;
					return true;
				})
				.Select(x => {
					var column = provider.GetStandardColumn(x);
					var caption = column.ColumnCaption ?? column.FieldName;
					return new FilterColumn {
						ColumnCaption = UpdateColumnCaption(caption, upperCasePropertyNames),
						EditSettings = column.EditSettings,
						ColumnType = x.PropertyType,
						FieldName = column.FieldName,
					};
				});
			return properties;
		}
		static string UpdateColumnCaption(string caption, bool upperCasePropertyNames) {
			if(upperCasePropertyNames)
				return caption.ToUpper();
			return caption;
		}
	}
}
