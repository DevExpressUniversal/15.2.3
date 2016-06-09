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

using DevExpress.Design.SmartTags;
using DevExpress.Design.UI;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public class BindingPopupViewModel : WpfBindableBase, IBindingEditorControlElementsProvider, IBindingEditorControlDataContextProvider, IBindingEditorControlRelativeSourceProvider, IBindingEditorControlResourcesProvider, IBindingEditorControlBindingSettingsProvider {
		readonly PropertyLineViewModelBase propertyLine;
		readonly Type targetType;
		ICommand closePopupCommand;
		BindingDescription binding;
		public BindingPopupViewModel(PropertyLineViewModelBase propertyLine) {
			this.propertyLine = propertyLine;
			IModelProperty property = ModelPropertyHelper.GetPropertyByName(propertyLine.SelectedItem, propertyLine.PropertyName, propertyLine.PropertyOwnerType ?? propertyLine.SelectedItem.ItemType);
			this.targetType = property.With(x => x.PropertyType);
		}
		public BindingDescription Binding {
			get { return binding; }
			set { SetProperty(ref binding, value, () => Binding, OnBindingChanged); }
		}
		public Func<BindingEditorControlPage> DefaultPage {
			get {
				return () => {
					BindingInfo bindingInfo = propertyLine.BindingInfo;
					if(bindingInfo is ElementNameBindingInfo) return BindingEditorControlPage.ElementNameBinding;
					if(bindingInfo is RelativeSourceSelfBindingInfo) return BindingEditorControlPage.RelativeSourceBinding;
					if(bindingInfo is StaticResourceBindingInfo) return BindingEditorControlPage.ResourceBinding;
					return BindingEditorControlPage.DataContextBinding;
				};
			}
		}
		public ICommand ClosePopupCommand {
			get {
				if(closePopupCommand == null)
					closePopupCommand = new WpfDelegateCommand(ClosePopup, false);
				return closePopupCommand;
			}
		}
		public void ClosePopup() {
			this.propertyLine.IsBindingPopupOpen = false;
		}
		IBindingEditorControlElement IBindingEditorControlElementsProvider.GetRootElement() {
			return new BindingPopupViewModelElement(propertyLine.SelectedItem.Root, null, targetType);
		}
		IBindingEditorControlDataContext IBindingEditorControlDataContextProvider.GetDataContext() {
			return new BindingPopupViewModelDataContext(propertyLine.SelectedItem, targetType);
		}
		IBindingEditorControlRelativeSourceSelf IBindingEditorControlRelativeSourceProvider.GetRelativeSourceSelf() {
			return new BindingPopupViewModelRelativeSourceSelf(propertyLine.SelectedItem, targetType);
		}
		IEnumerable<IBindingEditorControlResource> IBindingEditorControlResourcesProvider.GetResources() {
			return BindingInfoHelper.GetResources(propertyLine.SelectedItem).Select(p => new BindingPopupViewModelResource(p.Key, p.Value, targetType));
		}
		IEnumerable<BindingSettingDescription<BindingMode>> IBindingEditorControlBindingSettingsProvider.GetModes(IBindingEditorControlBindingSource source, string path) {
			if(source == null) yield break;
			DependencyPropertyDescriptor targetProperty = DependencyPropertyDescriptor.FromName(propertyLine.PropertyName, propertyLine.PropertyOwnerType ?? propertyLine.SelectedItem.ItemType, propertyLine.SelectedItem.ItemType);
			FrameworkPropertyMetadata metadata = targetProperty.DependencyProperty.GetMetadata(propertyLine.SelectedItem.ItemType) as FrameworkPropertyMetadata;
			bool bindsTwoWayByDefault = metadata != null && metadata.BindsTwoWayByDefault;
			Type sourceType = ((BindingPopupViewModelBindingSource)source).SourceType;
			if(sourceType != null && BindingInfoHelper.IsReadOnlyProperty(sourceType, path)) {
				if(!bindsTwoWayByDefault)
					yield return new BindingSettingDescription<BindingMode>(BindingMode.Default, BindingMode.OneWay);
				yield return new BindingSettingDescription<BindingMode>(BindingMode.OneWay);
				yield return new BindingSettingDescription<BindingMode>(BindingMode.OneTime);
			} else {
				if(bindsTwoWayByDefault)
					yield return new BindingSettingDescription<BindingMode>(BindingMode.Default, BindingMode.TwoWay);
				else
					yield return new BindingSettingDescription<BindingMode>(BindingMode.Default, BindingMode.OneWay);
				yield return new BindingSettingDescription<BindingMode>(BindingMode.TwoWay);
				yield return new BindingSettingDescription<BindingMode>(BindingMode.OneWay);
				yield return new BindingSettingDescription<BindingMode>(BindingMode.OneTime);
				yield return new BindingSettingDescription<BindingMode>(BindingMode.OneWayToSource);
			}
		}
		IEnumerable<BindingSettingDescription<UpdateSourceTrigger>> IBindingEditorControlBindingSettingsProvider.GetUpdateSourceTriggers() {
			DependencyPropertyDescriptor targetProperty = DependencyPropertyDescriptor.FromName(propertyLine.PropertyName, propertyLine.PropertyOwnerType ?? propertyLine.SelectedItem.ItemType, propertyLine.SelectedItem.ItemType);
			FrameworkPropertyMetadata metadata = targetProperty.DependencyProperty.GetMetadata(propertyLine.SelectedItem.ItemType) as FrameworkPropertyMetadata;
			yield return new BindingSettingDescription<UpdateSourceTrigger>(UpdateSourceTrigger.Default, metadata == null ? UpdateSourceTrigger.PropertyChanged : metadata.DefaultUpdateSourceTrigger);
			yield return new BindingSettingDescription<UpdateSourceTrigger>(UpdateSourceTrigger.Explicit);
			yield return new BindingSettingDescription<UpdateSourceTrigger>(UpdateSourceTrigger.LostFocus);
			yield return new BindingSettingDescription<UpdateSourceTrigger>(UpdateSourceTrigger.PropertyChanged);
		}
		IEnumerable<IBindingEditorControlConverter> IBindingEditorControlBindingSettingsProvider.GetConverters() {
			return BindingInfoHelper.GetResources(propertyLine.SelectedItem).Where(r => IsValueConverter(r.Value)).Select(p => new BindingEditorControlConverter(p.Key));
		}
		void OnBindingChanged() {
			BindingPopupViewModelResource resource = Binding.Source as BindingPopupViewModelResource;
			if(resource != null) {
				BindingInfoHelper.SetStaticResourceBinding(propertyLine.SelectedItem, propertyLine.PropertyName, propertyLine.PropertyOwnerType, Binding.Path, Binding.Converter.With(c => c.Key), Binding.Mode.Value, Binding.UpdateSourceTrigger.Value, resource.Key);
				return;
			}
			BindingPopupViewModelElement element = Binding.Source as BindingPopupViewModelElement;
			if(element != null) {
				BindingInfoHelper.SetElementNameBinding(propertyLine.SelectedItem, propertyLine.PropertyName, propertyLine.PropertyOwnerType, Binding.Path, Binding.Converter.With(c => c.Key), Binding.Mode.Value, Binding.UpdateSourceTrigger.Value, element.Item, element.Item.Name);
				return;
			}
			if(Binding.Source is BindingPopupViewModelRelativeSourceSelf) {
				BindingInfoHelper.SetRelativeSourceSelfBinding(propertyLine.SelectedItem, propertyLine.PropertyName, propertyLine.PropertyOwnerType, Binding.Path, Binding.Converter.With(c => c.Key), Binding.Mode.Value, Binding.UpdateSourceTrigger.Value);
				return;
			}
			DebugHelper.Assert(Binding.Source is BindingPopupViewModelDataContext);
			BindingInfoHelper.SetDataContextBinding(propertyLine.SelectedItem, propertyLine.PropertyName, propertyLine.PropertyOwnerType, Binding.Path, Binding.Converter.With(c => c.Key), Binding.Mode.Value, Binding.UpdateSourceTrigger.Value);
		}
		bool IsValueConverter(Type type) {
			return type.GetInterface("IValueConverter") != null;
		}
	}
	public class BindingEditorControlConverter : IBindingEditorControlConverter {
		readonly string key;
		public BindingEditorControlConverter(string key) {
			this.key = key;
		}
		string IBindingEditorControlConverter.Key { get { return key; } }
	}
	public abstract class BindingPopupViewModelBindingSource : IBindingEditorControlBindingSource {
		public BindingPopupViewModelBindingSource(Type targetType) {
			TargetType = targetType;
		}
		public abstract Type SourceType { get; }
		protected Type TargetType { get; private set; }
		IBindingEditorControlProperty IBindingEditorControlBindingSource.RootProperty {
			get {
				return SourceType.With(t => new BindingPopupViewModelProperty(t, null, null, TargetType));
			}
		}
	}
	public class BindingPopupViewModelDataContext : BindingPopupViewModelBindingSource, IBindingEditorControlDataContext {
		readonly IModelItem item;
		public BindingPopupViewModelDataContext(IModelItem item, Type targetType)
			: base(targetType) {
			this.item = item;
		}
		public override Type SourceType {
			get {
				DesignTimeObject dataContext = BindingInfoHelper.GetDataContext(item);
				if(dataContext == null) return null;
				return dataContext.RuntimeType;
			}
		}
	}
	public class BindingPopupViewModelRelativeSourceSelf : BindingPopupViewModelBindingSource, IBindingEditorControlRelativeSourceSelf {
		readonly IModelItem item;
		public BindingPopupViewModelRelativeSourceSelf(IModelItem item, Type targetType)
			: base(targetType) {
			this.item = item;
		}
		public override Type SourceType { get { return item.ItemType; } }
	}
	public class BindingPopupViewModelElement : BindingPopupViewModelBindingSource, IBindingEditorControlElement {
		readonly BindingPopupViewModelElement parent;
		public BindingPopupViewModelElement(IModelItem item, BindingPopupViewModelElement parent, Type targetType)
			: base(targetType) {
			Guard.ArgumentNotNull(item, "item");
			Item = item;
			this.parent = parent;
		}
		public IModelItem Item { get; private set; }
		public override Type SourceType { get { return Item.ItemType; } }
		IBindingEditorControlElement IMvvmControlTreeNode<IBindingEditorControlElement>.Parent { get { return parent; } }
		string IBindingEditorControlElement.ElementName { get { return Item.Name; } }
		string IBindingEditorControlElement.ElementType { get { return BindingInfoHelper.GetTypeName(Item.ItemType); } }
		IEnumerable<IBindingEditorControlElement> IMvvmControlTreeNode<IBindingEditorControlElement>.GetChildren() {
			return BindingInfoHelper.GetChildElements(Item).Select(e => new BindingPopupViewModelElement(e, this, TargetType));
		}
	}
	public class BindingPopupViewModelResource : BindingPopupViewModelBindingSource, IBindingEditorControlResource {
		readonly string key;
		readonly Type valueType;
		public BindingPopupViewModelResource(string key, Type valueType, Type targetType)
			: base(targetType) {
			this.key = key;
			this.valueType = valueType;
		}
		public string Key { get { return key; } }
		public override Type SourceType { get { return valueType; } }
		string IBindingEditorControlResource.Key { get { return key; } }
	}
	public class BindingPopupViewModelProperty : IBindingEditorControlProperty {
		readonly PropertyDescriptor descriptor;
		readonly IBindingEditorControlProperty parent;
		readonly Type type;
		readonly Type targetType;
		public BindingPopupViewModelProperty(Type type, PropertyDescriptor descriptor, IBindingEditorControlProperty parent, Type targetType) {
			Guard.ArgumentNotNull(type, "type");
			this.type = type;
			this.descriptor = descriptor;
			this.parent = parent;
			this.targetType = targetType;
		}
		IBindingEditorControlProperty Me { get { return this; } }
		IBindingEditorControlProperty IMvvmControlTreeNode<IBindingEditorControlProperty>.Parent { get { return parent; } }
		string IBindingEditorControlProperty.PropertyName { get { return Me.IsClass ? "" : descriptor.Name; } }
		string IBindingEditorControlProperty.PropertyType { get { return BindingInfoHelper.GetTypeName(type); } }
		string IBindingEditorControlProperty.ShortPropertyType { get { return Me.IsPOCO ? BindingInfoHelper.GetTypeName(type.BaseType) : Me.PropertyType; } }
		bool IBindingEditorControlProperty.IsClass { get { return descriptor == null; } }
		bool IBindingEditorControlProperty.IsPOCO { get { return typeof(DevExpress.Mvvm.POCO.IPOCOViewModel).IsAssignableFrom(type); } }
		bool IBindingEditorControlProperty.Highlighted { get { return targetType != null && targetType != typeof(object) && targetType.IsAssignableFrom(type); } }
		IEnumerable<IBindingEditorControlProperty> IMvvmControlTreeNode<IBindingEditorControlProperty>.GetChildren() {
			return BindingInfoHelper.GetProperties(type).Select(p => new BindingPopupViewModelProperty(p.PropertyType, p, this, targetType));
		}
	}
}
