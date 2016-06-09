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

extern alias Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.PropertyEditing;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core.Design {
	public static class AttributeTableBuilderExtensions {
		private const string CompanyToolboxCategoryPath = "DevExpress " + Platform::AssemblyInfo.VSuffixWithoutSeparator + "/";
		private const string CompanyToolboxTabName = "DX." + Platform::AssemblyInfo.VersionShort + ": ";
		public static void AddAttribute(this AttributeTableBuilder builder, Attribute attribute, params Type[] controlTypes) {
			foreach (Type type in controlTypes)
				builder.AddCustomAttributes(type, attribute);
		}
		public static void AddAttribute(this AttributeTableBuilder builder, Attribute attribute, Type controlType, params string[] properties) {
			builder.AddCallback(controlType,
				delegate(AttributeCallbackBuilder b) {
					foreach (string property in properties)
						b.AddCustomAttributes(property, attribute);
				});
		}
		public static void AddAttributes(this AttributeTableBuilder builder, Type controlType, params Attribute[] attributes) {
			builder.AddCallback(controlType, b => b.AddCustomAttributes(attributes));
		}
		public static void HideControls(this AttributeTableBuilder builder, params Type[] controlTypes) {
			builder.AddAttribute(ToolboxBrowsableAttribute.No, controlTypes);
		}
		public static void ShowControls(this AttributeTableBuilder builder, params Type[] controlTypes) {
			builder.AddAttribute(ToolboxBrowsableAttribute.Yes, controlTypes);
		}
		public static void RegisterControls(this AttributeTableBuilder builder, string categoryPath, params Type[] controlTypes) {
			builder.ShowControls(controlTypes);
			builder.AddAttribute(new ToolboxCategoryAttribute(CompanyToolboxCategoryPath + categoryPath), controlTypes);
			builder.AddAttribute(new ToolboxTabNameAttribute(CompanyToolboxTabName + categoryPath), controlTypes);
		}
		public static void HideProperties(this AttributeTableBuilder builder, Type controlType, params string[] properties) {
			builder.AddAttribute(new EditorBrowsableAttribute(EditorBrowsableState.Never), controlType, properties);
			builder.AddAttribute(BrowsableAttribute.No, controlType, properties);
		}
		public static void ShowProperties(this AttributeTableBuilder builder, Type controlType, params string[] properties) {
			builder.AddAttribute(new EditorBrowsableAttribute(EditorBrowsableState.Always), controlType, properties);
			builder.AddAttribute(BrowsableAttribute.Yes, controlType, properties);
		}
		public static void ShowPropertiesAsAdvanced(this AttributeTableBuilder builder, Type controlType, params string[] properties) {
			builder.AddAttribute(new EditorBrowsableAttribute(EditorBrowsableState.Advanced), controlType, properties);
			builder.AddAttribute(BrowsableAttribute.Yes, controlType, properties);
		}
		public static void RegisterAttachedPropertiesForChildren(this AttributeTableBuilder builder, Type controlType, bool includeDescendants,
			params string[] propertyGetters) {
			builder.AddAttribute(new AttachedPropertyBrowsableForChildrenAttribute { IncludeDescendants = includeDescendants },
				controlType, propertyGetters);
			foreach (string property in propertyGetters)
				if (property.StartsWith("Get"))
					builder.AddAttribute(new AttachedPropertyBrowsableForChildrenAttribute { IncludeDescendants = includeDescendants },
						controlType, property.Substring(3));
		}
		public static void RegisterAttachedPropertiesForType(this AttributeTableBuilder builder, Type controlType, Type targetType,
			params string[] propertyGetters) {
			builder.AddAttribute(new AttachedPropertyBrowsableForTypeAttribute(targetType), controlType, propertyGetters);
			foreach (string property in propertyGetters)
				if (property.StartsWith("Get"))
					builder.AddAttribute(new AttachedPropertyBrowsableForTypeAttribute(targetType), controlType, property.Substring(3));
		}
		public static void AddTypeConverter(this AttributeTableBuilder builder, Type converterType, Type controlType, params string[] properties) {
			builder.AddAttribute(new TypeConverterAttribute(converterType), controlType, properties);
		}
		public static void AddStringConverter(this AttributeTableBuilder builder, Type controlType, params string[] properties) {
			builder.AddTypeConverter(typeof(StringConverter), controlType, properties);
		}
		public static void PutPropertiesInCategory(this AttributeTableBuilder builder, string category, Type controlType, params string[] properties) {
			builder.AddAttribute(new CategoryAttribute(category), controlType, properties);
		}
		public static void PutPropertiesInCategory(this AttributeTableBuilder builder, CategoryAttribute category,
			Type controlType, params string[] properties) {
			builder.AddAttribute(category, controlType, properties);
		}
		public static void PositionProperties(this AttributeTableBuilder builder, PropertyOrder order, Type controlType, params string[] properties) {
			builder.AddAttribute(new PropertyOrderAttribute(order), controlType, properties);
		}
	}
	public static class Categories {
		private static CategoryAttribute _CommonProperties;
		public static CategoryAttribute Appearance { get { return CategoryAttribute.Appearance; } }
		public static CategoryAttribute CommonProperties {
			get {
				if (_CommonProperties == null)
					_CommonProperties = new CategoryAttribute("Common Properties");
				return _CommonProperties; 
			} 
		}
		public static CategoryAttribute Layout { get { return CategoryAttribute.Layout; } }
	}
	public static class DesignerHelper {
		public static ModelItem Find(this ModelService modelService, ModelItem startingItem, object platformObject) {
			IEnumerable<ModelItem> items = modelService.Find(startingItem, platformObject.GetType());
			return items.FirstOrDefault<ModelItem>(item => item.GetCurrentValue() == platformObject);
		}
		public static List<ModelItem> Find(this ModelService modelService, ModelItem startingItem, IEnumerable platformObjects) {
			var result = new List<ModelItem>();
			foreach (object platformObject in platformObjects) {
				ModelItem modelItem = modelService.Find(startingItem, platformObject);
				if (modelItem != null)
					result.Add(modelItem);
			}
			return result;
		}
		public static IEnumerable<Platform::System.Windows.FrameworkElement> GetPlatformObjects(this IEnumerable<ModelItem> items) {
			foreach (ModelItem item in items)
				if (item.View != null) {
					var element = item.GetCurrentValue() as Platform::System.Windows.FrameworkElement;
					if (element != null)
						yield return element;
				}
		}
		public static void UpdateProperty(this ModelItem item, string propertyName, Platform::System.Windows.DependencyProperty property,
			Platform::System.Windows.DependencyObject platformObject) {
			ModelProperty modelProperty = item.Properties[propertyName];
			if (platformObject.IsPropertyAssigned(property))
				modelProperty.ComputedValue = platformObject.GetValue(property);
			else
				if (modelProperty.IsSet)
					modelProperty.ClearValue();
		}
		public static bool IsBlend { get { return AppDomain.CurrentDomain.SetupInformation.ApplicationName == "Blend.exe"; } }
	}
	public static class UpdateLayoutHelper {
		public static readonly DependencyProperty UpdateLayoutOnDataContextChangedProperty =
			DependencyProperty.RegisterAttached("UpdateLayoutOnDataContextChanged", typeof(bool), typeof(UpdateLayoutHelper),
			new PropertyMetadata(false, OnUpdateLayoutOnDataContextChanged));
		public static bool GetUpdateLayoutOnDataContextChanged(FrameworkElement element) {
			return (bool)element.GetValue(UpdateLayoutOnDataContextChangedProperty);
		}
		public static void SetUpdateLayoutOnDataContextChanged(FrameworkElement element, bool value) {
			element.SetValue(UpdateLayoutOnDataContextChangedProperty, value);
		}
		static void OnUpdateLayoutOnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			bool updateLayoutOnDataContextChanged = (bool)e.NewValue;
			var elem = d as FrameworkElement;
			if(elem == null)
				return;
			if(updateLayoutOnDataContextChanged)
				elem.DataContextChanged += OnDataContextChanged;
			else
				elem.DataContextChanged -= OnDataContextChanged;
		}
		static void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UpdateLayout((UIElement)sender);
		}
		private static void UpdateLayout(UIElement element) {
			IEnumerable<UIElement> visualTree = GetVisualTree(element);
			visualTree.ToList().ForEach(elem => elem.InvalidateMeasure());
			visualTree.Last().UpdateLayout();
		}
		static IEnumerable<UIElement> GetVisualTree(UIElement from) {
			List<UIElement> elems = new List<UIElement>();
			while(from != null) {
				elems.Add(from);
				from = VisualTreeHelper.GetParent(from) as UIElement;
			}
			return elems;
		}
	}
	public abstract class ConditionalSelectionPolicy : SelectionPolicy {
		protected abstract bool Condition(ModelItem item);
		protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			if (selection.PrimarySelection != null && selection.SelectedObjects.All(item => Condition(item)))
				yield return selection.PrimarySelection;
		}
	}
	public abstract class SameParentTypeSelectionPolicy : ConditionalSelectionPolicy {
		protected override bool Condition(ModelItem item) {
			return item.Parent != null && item.Parent.IsItemOfType(GetParentType());
		}
		protected abstract Type GetParentType();
	}
	public abstract class ContextMenuProviderBase : ContextMenuProvider {
		public ContextMenuProviderBase() {
			UpdateItemStatus +=
				delegate(object sender, MenuActionEventArgs e) {
					if (IsActive(e.Selection))
						OnUpdateItemStatus(e);
					else
						Items.Clear();
				};
		}
		protected virtual bool IsActive(Selection selection) {
			return true;
		}
		protected virtual void OnUpdateItemStatus(MenuActionEventArgs e) {
		}
		protected MenuAction CreateMenuItem(ObservableCollection<MenuBase> items, string displayName, bool checkable,
			EventHandler<MenuActionEventArgs> execute) {
			var result = new MenuAction(displayName);
			result.Checkable = checkable;
			result.Execute += execute;
			items.Add(result);
			return result;
		}
		protected MenuGroup CreateEnumMenuGroup(ObservableCollection<MenuBase> items, string displayName, Type enumType,
			EventHandler<MenuActionEventArgs> execute) {
			return CreateEnumMenuGroup(items, displayName, enumType, true, execute);
		}
		protected MenuGroup CreateEnumMenuGroup(ObservableCollection<MenuBase> items, string displayName, Type enumType, bool checkable,
			EventHandler<MenuActionEventArgs> execute) {
			var result = new MenuGroup(displayName) { HasDropDown = true };
			foreach(string name in Enum.GetNames(enumType))
				CreateMenuItem(result.Items, name, checkable, execute);
			items.Add(result);
			return result;
		}
		protected object GetEnumMenuGroupValue(MenuGroup menuGroup, MenuAction checkedMenuItem) {
			return menuGroup.Items.IndexOf(checkedMenuItem);
		}
		protected void SetEnumMenuGroupValue(MenuGroup menuGroup, object value) {
			if (value == null)
				return;
			for (int i = 0; i < menuGroup.Items.Count; i++)
				((MenuAction)menuGroup.Items[i]).Checked = i == (int)value;
		}
		protected T? GetAttachedProperty<T>(IEnumerable<ModelItem> items, Type declaringType, string propertyName) where T : struct {
			var property = new PropertyIdentifier(declaringType, propertyName);
			return GetProperty<T>(items, item => (T)item.Properties[property].ComputedValue);
		}
		protected void SetAttachedProperty<T>(IEnumerable<ModelItem> items, Type declaringType, string propertyName, T propertyValue) {
			var property = new PropertyIdentifier(declaringType, propertyName);
			SetProperty<T>(items, (item, value) => item.Properties[property].ComputedValue = value, propertyValue);
		}
		protected void SetAttachedProperty<T>(Selection selection, Type declaringType, string propertyName, T propertyValue) {
			using (ModelEditingScope editingScope = selection.PrimarySelection.BeginEdit("Change " + propertyName)) {
				SetAttachedProperty<T>(selection.SelectedObjects, declaringType, propertyName, propertyValue);
				editingScope.Complete();
			}
		}
		protected T? GetProperty<T>(IEnumerable<ModelItem> items, Func<ModelItem, T> getProperty) where T : struct {
			T? result = null;
			foreach (ModelItem item in items)
				if (result == null)
					result = getProperty(item);
				else
					if (!result.Equals(getProperty(item)))
						return null;
			return result;
		}
		protected T? GetProperty<T>(IEnumerable<ModelItem> items, string propertyName) where T : struct {
			return GetProperty<T>(items, item => (T)item.Properties[propertyName].ComputedValue);
		}
		protected void SetProperty<T>(IEnumerable<ModelItem> items, Action<ModelItem, T> setProperty, T propertyValue) {
			foreach (ModelItem item in items)
				setProperty(item, propertyValue);
		}
		protected void SetProperty<T>(IEnumerable<ModelItem> items, string propertyName, T propertyValue) {
			SetProperty<T>(items,
				delegate(ModelItem item, T value) {
					if ((object)value == Platform::System.Windows.DependencyProperty.UnsetValue)
						item.Properties[propertyName].ClearValue();
					else
						item.Properties[propertyName].SetValue(value);
				},
				propertyValue);
		}
		protected void SetProperty<T>(Selection selection, string propertyName, T propertyValue, Action<ModelItem, T> setProperty = null) {
			using (ModelEditingScope editingScope = selection.PrimarySelection.BeginEdit("Change " + propertyName)) {
				if (setProperty != null)
					SetProperty<T>(selection.SelectedObjects, setProperty, propertyValue);
				else
					SetProperty<T>(selection.SelectedObjects, propertyName, propertyValue);
				editingScope.Complete();
			}
		}
	}
	public abstract class ChildContextMenuProvider : ContextMenuProviderBase {
		protected abstract Type GetParentType();
		protected override bool IsActive(Selection selection) {
			return selection.SelectedObjects.All(
				delegate(ModelItem item) {
					var element = item.GetCurrentValue() as Platform::System.Windows.FrameworkElement;
					return element != null && GetParentType().IsInstanceOfType(element.Parent);
				});
		}
	}
}
