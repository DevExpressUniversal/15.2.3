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
using System.Windows;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization.Native;
using System.Linq;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using System.IO;
using DevExpress.Mvvm.UI;
using System.Windows.Media;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Core.Serialization {
	public enum StoreLayoutMode {
		All,
		UI,
		None
	}
	public interface IDXSerializable {
		DependencyObject Source { get; }
		string FullPath { get; }
		object EventTarget { get; set; }
	}
	public class AttachedPropertyInfo : DevExpress.Utils.Serializing.XmlXtraSerializer.XmlXtraPropertyInfo {
		public AttachedPropertyInfo(string name, Type propertyType, Type dPropType, Type ownerType, object value, bool isKey)
			: base(name, propertyType, value, isKey) {
			OwnerType = ownerType;
			string[] nameParts = name.Split(new char[] { '.' }, StringSplitOptions.None);
			DependencyPropertyName = nameParts[nameParts.Length - 1];
			DependencyPropertyType = dPropType;
		}
		public AttachedPropertyInfo(string name, Type propertyType, Type dPropType, Type ownerType, object value) : this(name, propertyType, dPropType, ownerType, value, false) { }
		public AttachedPropertyInfo(string name, Type propertyType, object value) : this(name, propertyType, null, null, value) { }
		public Type OwnerType { get; private set; }
		public string DependencyPropertyName { get; private set; }
		public Type DependencyPropertyType { get; private set; }
	}
	public class DXSerializationContext : SerializationContext {
		protected override PropertyDescriptorCollection GetProperties(object obj, IXtraPropertyCollection store) {
			List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>(System.Linq.Enumerable.Cast <PropertyDescriptor>(base.GetProperties(obj, store)));
			if(store != null && obj != null) {
				foreach(XtraPropertyInfo pInfo in store) {
					AttachedPropertyInfo apInfo = pInfo as AttachedPropertyInfo;
					if(apInfo != null) {
						DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromName(
								apInfo.DependencyPropertyName, apInfo.OwnerType, obj.GetType()
							);
						if(descriptor != null) descriptors.Add(descriptor);
					}
				}
			}
			return new PropertyDescriptorCollection((PropertyDescriptor[])descriptors.ToArray(), true);
		}
		protected override void AddSortedProps(DXCollection<SerializablePropertyDescriptorPair> list, List<SerializablePropertyDescriptorPair> pairsList, IComparer<SerializablePropertyDescriptorPair> comparer) {
			list.AddRange(pairsList.OrderBy(x => x, comparer).ToArray());
		}
		protected override void CustomGetSerializableProperties(object obj, List<SerializablePropertyDescriptorPair> pairsList, PropertyDescriptorCollection props) {
			DependencyObject dObj = obj as DependencyObject;
			if(dObj == null) return;
			SerializationProvider provider = DXSerializer.GetSerializationProvider(dObj);
			provider.OnCustomGetSerializableProperties(dObj, new CustomGetSerializablePropertiesEventArgs(obj, pairsList, props));
		}
		protected override void InvokeClearCollection(DeserializeHelper helper, XtraItemEventArgs e) {
			SerializationProviderWrapper wrapper = GetSerializationProviderWrapper(helper, e.Owner);
			if(wrapper != null) wrapper.OnClearCollection(e);
			else base.InvokeClearCollection(helper, e);
		}
		protected override object InvokeFindCollectionItem(DeserializeHelper helper, string propertyName, XtraItemEventArgs e) {
			SerializationProviderWrapper wrapper = GetSerializationProviderWrapper(helper, e.Owner);
			if(wrapper != null) return wrapper.OnFindCollectionItem(propertyName, e);
			return base.InvokeFindCollectionItem(helper, propertyName, e);
		}
		protected override object InvokeCreateCollectionItem(DeserializeHelper helper, string propertyName, XtraItemEventArgs e) {
			SerializationProviderWrapper wrapper = GetSerializationProviderWrapper(helper, e.Owner);
			if(wrapper != null) return wrapper.OnCreateCollectionItem(propertyName, e);
			return base.InvokeCreateCollectionItem(helper, propertyName, e);
		}
		protected override object InvokeCreateContentPropertyValueMethod(DeserializeHelper helper, XtraItemEventArgs e, PropertyDescriptor prop) {
			SerializationProviderWrapper wrapper = GetSerializationProviderWrapper(helper, e.Owner);
			if (wrapper != null) return wrapper.OnCreateContentPropertyValue(prop, e);
			return base.InvokeCreateContentPropertyValueMethod(helper, e, prop);
		}
		protected override bool AllowProperty(SerializeHelperBase helper, object obj, PropertyDescriptor prop, OptionsLayoutBase options, bool isSerializing) {
			SerializationProviderWrapper wrapper = GetSerializationProviderWrapper(helper, obj);
			if(wrapper != null) {
				int id = GetPropertyId(helper, prop);
				return (id == -1) || wrapper.AllowProperty(obj, prop, id, isSerializing);
			}
			return base.AllowProperty(helper, obj, prop, options, isSerializing);
		}
		protected override void ResetProperty(DeserializeHelper helper, object obj, PropertyDescriptor prop, XtraSerializableProperty attr) {
			SerializationProviderWrapper wrapper = GetSerializationProviderWrapper(helper, obj);
			if(wrapper != null)
				wrapper.ResetProperty(obj, prop, attr);
			else
				base.ResetProperty(helper, obj, prop, attr);
		}
		static SerializationProviderWrapper GetSerializationProviderWrapper(SerializeHelperBase helper, object obj) {
			DXSerializer.DXDeserializeHelper dxDeserializeHelper = helper as DXSerializer.DXDeserializeHelper;
			if(dxDeserializeHelper != null)
				return dxDeserializeHelper.GetDXObj(obj) as SerializationProviderWrapper;
			DXSerializer.DXSerializeHelper dxSerializeHelper = helper as DXSerializer.DXSerializeHelper;
			if(dxSerializeHelper != null)
				return dxSerializeHelper.GetDXObj(obj) as SerializationProviderWrapper;
			return null;
		}
		protected override bool CustomDeserializeProperty(DeserializeHelper helper, object obj, PropertyDescriptor property, XtraPropertyInfo propertyInfo) {
			SerializationProviderWrapper wrapper = GetSerializationProviderWrapper(helper, obj);
			if(wrapper != null)
				return wrapper.CustomDeserializeProperty(obj, property, propertyInfo);
			else
				return base.CustomDeserializeProperty(helper, obj, property, propertyInfo);
		}
		protected override bool ShouldSerializeProperty(SerializeHelper helper, object obj, PropertyDescriptor prop, XtraSerializableProperty xtraSerializableProperty) {
			SerializationProviderWrapper wrapper = GetSerializationProviderWrapper(helper, obj);
			if(wrapper != null) {
				bool? result = wrapper.CustomShouldSerializeProperty(obj, prop);
				if(result.HasValue) return result.Value;
				return wrapper.OnShouldSerializeProperty(obj, prop, xtraSerializableProperty);
			}
			DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(prop);
			if(dependencyPropertyDescriptor != null)
				return ShouldSerializeDependencyProeprty(dependencyPropertyDescriptor, obj);
			return base.ShouldSerializeProperty(helper, obj, prop, xtraSerializableProperty);
		}
		internal static bool ShouldSerializeDependencyProeprty(DependencyPropertyDescriptor dependencyPropertyDescriptor, object obj) {
#if SL
			return dependencyPropertyDescriptor.ShouldSerializeValue(obj);
#else
			if(obj is DependencyObject) {
				ValueSource valueSource = System.Windows.DependencyPropertyHelper.GetValueSource((DependencyObject)obj, dependencyPropertyDescriptor.DependencyProperty);
				return valueSource.BaseValueSource >= BaseValueSource.ParentTemplate;
			}
			return false;
#endif
		}
		protected override bool ShouldSerializeCollectionItem(
			SerializeHelper helper, object owner, System.Reflection.MethodInfo mi, XtraPropertyInfo itemProperty, object item, XtraItemEventArgs e) {
			SerializationProviderWrapper wrapper = GetSerializationProviderWrapper(helper, e.Owner);
			if(wrapper != null) return wrapper.OnShouldSerailizaCollectionItem(e, item);
			return base.ShouldSerializeCollectionItem(helper, owner, mi, itemProperty, item, e);
		}
		protected override bool CanDeserializeProperty(object obj, PropertyDescriptor prop) {
			DependencyObject dObj = obj as DependencyObject;
			DependencyPropertyDescriptor dProp = DependencyPropertyDescriptor.FromProperty(prop);
			if(dObj != null && dProp != null) {
				System.Windows.Data.BindingExpression be = dObj.ReadLocalValue(dProp.DependencyProperty) as System.Windows.Data.BindingExpression;
				if(be != null && be.ParentBinding != null) {
					var mode = be.ParentBinding.Mode;
#if !SILVERLIGHT
					if(mode == System.Windows.Data.BindingMode.Default) {
						var metadata = dProp.DependencyProperty.GetMetadata(dObj) as FrameworkPropertyMetadata;
						mode = (metadata != null) && metadata.BindsTwoWayByDefault ?
							System.Windows.Data.BindingMode.TwoWay : System.Windows.Data.BindingMode.OneWay;
					}
					return (mode == System.Windows.Data.BindingMode.TwoWay) || (mode == System.Windows.Data.BindingMode.OneWayToSource);
#else
					return (mode == System.Windows.Data.BindingMode.TwoWay);
#endif
				}
			}
			return base.CanDeserializeProperty(obj, prop);
		}
	}
	public static class DXSerializer {
	#region static
		public static readonly DependencyProperty EnabledProperty;
		public static readonly DependencyProperty SerializationIDProperty;
		public static readonly DependencyProperty SerializationIDDefaultProperty;
		public static readonly DependencyProperty SerializationProviderProperty;
		public static readonly DependencyProperty StoreLayoutModeProperty;
		public static readonly DependencyProperty LayoutVersionProperty;
		static readonly DependencyPropertyKey IsProcessingSingleObjectPropertyKey;
		public static readonly DependencyProperty IsProcessingSingleObjectProperty;
		static readonly DependencyPropertyKey IsCustomSerializableChildPropertyKey;
		public static readonly DependencyProperty IsCustomSerializableChildProperty;
		public static readonly RoutedEvent StartSerializingEvent;
		public static readonly RoutedEvent EndSerializingEvent;
		public static readonly RoutedEvent StartDeserializingEvent;
		public static readonly RoutedEvent EndDeserializingEvent;
		public static readonly RoutedEvent CustomGetSerializableChildrenEvent;
		public static readonly RoutedEvent CustomGetSerializablePropertiesEvent;
		public static readonly RoutedEvent ClearCollectionEvent;
		public static readonly RoutedEvent CreateCollectionItemEvent;
		public static readonly RoutedEvent FindCollectionItemEvent;
		public static readonly RoutedEvent AllowPropertyEvent;
		public static readonly RoutedEvent CustomShouldSerializePropertyEvent;
		public static readonly RoutedEvent ResetPropertyEvent;
		public static readonly RoutedEvent DeserializePropertyEvent;
		public static readonly RoutedEvent CreateContentPropertyValueEvent;
		public static readonly RoutedEvent BeforeLoadLayoutEvent;
		public static readonly RoutedEvent LayoutUpgradeEvent;
		public static readonly RoutedEvent ShouldSerializeCollectionItemEvent;
		static DXSerializer() {
			Type ownerType = typeof(DXSerializer);
			EnabledProperty = DependencyPropertyManager.RegisterAttached(
				"Enabled", typeof(bool), ownerType, new UIPropertyMetadata(true));
			SerializationIDProperty = DependencyPropertyManager.RegisterAttached(
				"SerializationID", typeof(string), ownerType, new UIPropertyMetadata(null, OnSerializationIDChanged));
			SerializationIDDefaultProperty = DependencyPropertyManager.RegisterAttached(
				"SerializationIDDefault", typeof(string), ownerType, new UIPropertyMetadata(null, OnSerializationIDChanged));
			SerializationProviderProperty = DependencyPropertyManager.RegisterAttached(
				"SerializationProvider", typeof(SerializationProvider), ownerType, new UIPropertyMetadata(SerializationProvider.Instance));
			StoreLayoutModeProperty = DependencyPropertyManager.RegisterAttached(
				"StoreLayoutMode", typeof(StoreLayoutMode), ownerType, new UIPropertyMetadata(StoreLayoutMode.UI));
			LayoutVersionProperty = DependencyPropertyManager.RegisterAttached(
				"LayoutVersion", typeof(string), ownerType, new UIPropertyMetadata(null));
			IsProcessingSingleObjectPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly(
				"IsProcessingSingleObject", typeof(bool), ownerType, new UIPropertyMetadata(false));
			IsProcessingSingleObjectProperty = IsProcessingSingleObjectPropertyKey.DependencyProperty;
			IsCustomSerializableChildPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly(
				"IsCustomSerializableChild", typeof(bool), ownerType, new UIPropertyMetadata(false));
			IsCustomSerializableChildProperty = IsCustomSerializableChildPropertyKey.DependencyProperty;
			StartSerializingEvent = EventManager.RegisterRoutedEvent(
				 "StartSerializing", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			EndSerializingEvent = EventManager.RegisterRoutedEvent(
				"EndSerializing", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			StartDeserializingEvent = EventManager.RegisterRoutedEvent(
				 "StartDeserializing", RoutingStrategy.Direct, typeof(StartDeserializingEventHandler), ownerType);
			EndDeserializingEvent = EventManager.RegisterRoutedEvent(
				"EndDeserializing", RoutingStrategy.Direct, typeof(EndDeserializingEventHandler), ownerType);
			CustomGetSerializablePropertiesEvent = EventManager.RegisterRoutedEvent(
				"CustomGetSerializableProperties", RoutingStrategy.Direct, typeof(CustomGetSerializablePropertiesEventHandler), ownerType);
			CustomGetSerializableChildrenEvent = EventManager.RegisterRoutedEvent(
				"CustomGetSerializableChildren", RoutingStrategy.Direct, typeof(CustomGetSerializableChildrenEventHandler), ownerType);
			ClearCollectionEvent = EventManager.RegisterRoutedEvent(
				"ClearCollection", RoutingStrategy.Direct, typeof(XtraItemRoutedEventHandler), ownerType);
			CreateCollectionItemEvent = EventManager.RegisterRoutedEvent(
				"CreateCollectionItem", RoutingStrategy.Direct, typeof(XtraCreateCollectionItemEventHandler), ownerType);
			FindCollectionItemEvent = EventManager.RegisterRoutedEvent(
				"FindCollectionItem", RoutingStrategy.Direct, typeof(XtraFindCollectionItemEventHandler), ownerType);
			AllowPropertyEvent = EventManager.RegisterRoutedEvent(
				"AllowProperty", RoutingStrategy.Direct, typeof(AllowPropertyEventHandler), ownerType);
			CustomShouldSerializePropertyEvent = EventManager.RegisterRoutedEvent(
				"CustomShouldSerializeProperty", RoutingStrategy.Direct, typeof(CustomShouldSerializePropertyEventHandler), ownerType);
			ResetPropertyEvent = EventManager.RegisterRoutedEvent(
				"ResetProperty", RoutingStrategy.Direct, typeof(ResetPropertyEventHandler), ownerType);
			DeserializePropertyEvent = EventManager.RegisterRoutedEvent(
				"DeserializeProperty", RoutingStrategy.Direct, typeof(XtraPropertyInfoEventHandler), ownerType);
			ShouldSerializeCollectionItemEvent = EventManager.RegisterRoutedEvent(
				"ShouldSerializeCollectionItem", RoutingStrategy.Direct, typeof(XtraShouldSerializeCollectionItemEventHandler), ownerType);
			CreateContentPropertyValueEvent = EventManager.RegisterRoutedEvent(
				"CreateContentPropertyValue", RoutingStrategy.Direct, typeof(XtraCreateContentPropertyValueEventHandler), ownerType);
			BeforeLoadLayoutEvent = EventManager.RegisterRoutedEvent("BeforeLoadLayout", RoutingStrategy.Direct, typeof(BeforeLoadLayoutEventHandler), ownerType);
			LayoutUpgradeEvent = EventManager.RegisterRoutedEvent("LayoutUpgrade", RoutingStrategy.Direct, typeof(LayoutUpgradeEventHandler), ownerType);
		}
		static void OnSerializationIDChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs ea) {
			InvalidSerializationIDException.Assert((string)ea.NewValue);
		}
		public static void AddDeserializePropertyHandler(DependencyObject dObj, XtraPropertyInfoEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(DeserializePropertyEvent, handler);
		}
		public static void RemoveDeserializePropertyHandler(DependencyObject dObj, XtraPropertyInfoEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(DeserializePropertyEvent, handler);
		}
		public static void AddShouldSerializeCollectionItemHandler(DependencyObject dObj, XtraShouldSerializeCollectionItemEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(ShouldSerializeCollectionItemEvent, handler);
		}
		public static void RemoveShouldSerializeCollectionItemHandler(DependencyObject dObj, XtraShouldSerializeCollectionItemEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(ShouldSerializeCollectionItemEvent, handler);
		}
		public static void AddBeforeLoadLayoutHandler(DependencyObject dObj, BeforeLoadLayoutEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(BeforeLoadLayoutEvent, handler);
		}
		public static void RemoveBeforeLoadLayoutHandler(DependencyObject dObj, BeforeLoadLayoutEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(BeforeLoadLayoutEvent, handler);
		}
		public static void AddLayoutUpgradeHandler(DependencyObject dObj, LayoutUpgradeEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(LayoutUpgradeEvent, handler);
		}
		public static void RemoveLayoutUpgradeHandler(DependencyObject dObj, LayoutUpgradeEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(LayoutUpgradeEvent, handler);
		}
		public static void AddResetPropertyHandler(DependencyObject dObj, ResetPropertyEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(ResetPropertyEvent, handler);
		}
		public static void RemoveResetPropertyHandler(DependencyObject dObj, ResetPropertyEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(ResetPropertyEvent, handler);
		}
		public static void AddAllowPropertyHandler(DependencyObject dObj, AllowPropertyEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(AllowPropertyEvent, handler);
		}
		public static void RemoveAllowPropertyHandler(DependencyObject dObj, AllowPropertyEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(AllowPropertyEvent, handler);
		}
		public static void AddCustomShouldSerializePropertyHandler(DependencyObject dObj, CustomShouldSerializePropertyEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null)
				uiElement.AddHandler(CustomShouldSerializePropertyEvent, handler);
		}
		public static void RemoveCustomShouldSerializePropertyHandler(DependencyObject dObj, CustomShouldSerializePropertyEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null)
				uiElement.RemoveHandler(CustomShouldSerializePropertyEvent, handler);
		}
		public static void AddCustomGetSerializablePropertiesHandler(DependencyObject dObj, CustomGetSerializablePropertiesEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(CustomGetSerializablePropertiesEvent, handler);
		}
		public static void RemoveCustomGetSerializablePropertiesHandler(DependencyObject dObj, CustomGetSerializablePropertiesEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(CustomGetSerializablePropertiesEvent, handler);
		}
		public static void AddCustomGetSerializableChildrenHandler(DependencyObject dObj, CustomGetSerializableChildrenEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(CustomGetSerializableChildrenEvent, handler);
		}
		public static void RemoveCustomGetSerializableChildrenHandler(DependencyObject dObj, CustomGetSerializableChildrenEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(CustomGetSerializableChildrenEvent, handler);
		}
		public static void AddStartSerializingHandler(DependencyObject dObj, RoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(StartSerializingEvent, handler);
		}
		public static void RemoveStartSerializingHandler(DependencyObject dObj, RoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(StartSerializingEvent, handler);
		}
		public static void AddEndSerializingHandler(DependencyObject dObj, RoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(EndSerializingEvent, handler);
		}
		public static void RemoveEndSerializingHandler(DependencyObject dObj, RoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(EndSerializingEvent, handler);
		}
		public static void AddStartDeserializingHandler(DependencyObject dObj, StartDeserializingEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(StartDeserializingEvent, handler);
		}
		public static void RemoveStartDeserializingHandler(DependencyObject dObj, StartDeserializingEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(StartDeserializingEvent, handler);
		}
		public static void AddEndDeserializingHandler(DependencyObject dObj, EndDeserializingEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(EndDeserializingEvent, handler);
		}
		public static void RemoveEndDeserializingHandler(DependencyObject dObj, EndDeserializingEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(EndDeserializingEvent, handler);
		}
		public static void AddClearCollectionHandler(DependencyObject dObj, XtraItemRoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(ClearCollectionEvent, handler);
		}
		public static void RemoveClearCollectionHandler(DependencyObject dObj, XtraItemRoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(ClearCollectionEvent, handler);
		}
		public static void AddCreateCollectionItemEventHandler(DependencyObject dObj, XtraCreateCollectionItemEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(CreateCollectionItemEvent, handler);
		}
		public static void RemoveCreateCollectionItemEventHandler(DependencyObject dObj, XtraCreateCollectionItemEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(CreateCollectionItemEvent, handler);
		}
		public static void AddFindCollectionItemEventHandler(DependencyObject dObj, XtraFindCollectionItemEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(FindCollectionItemEvent, handler);
		}
		public static void RemoveFindCollectionItemEventHandler(DependencyObject dObj, XtraFindCollectionItemEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(FindCollectionItemEvent, handler);
		}
		public static void AddCreateContentPropertyValueEventHandler(DependencyObject dObj, XtraCreateContentPropertyValueEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if (uiElement != null) uiElement.AddHandler(CreateContentPropertyValueEvent, handler);
		}
		public static void RemoveCreateContentPropertyValueEventHandler(DependencyObject dObj, XtraCreateContentPropertyValueEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if (uiElement != null) uiElement.RemoveHandler(CreateContentPropertyValueEvent, handler);
		}
		public static StoreLayoutMode GetStoreLayoutMode(DependencyObject obj) {
			return (StoreLayoutMode)obj.GetValue(StoreLayoutModeProperty);
		}
		public static void SetEnabled(DependencyObject dObj, bool value) {
			dObj.SetValue(EnabledProperty, value);
		}
		public static bool GetEnabled(DependencyObject dObj) {
			return (bool)dObj.GetValue(EnabledProperty);
		}
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static bool GetIsCustomSerializableChild(DependencyObject dObj) {
			return (bool)dObj.GetValue(IsCustomSerializableChildProperty);
		}
		static void SetIsCustomSerializableChild(DependencyObject dObj, bool value) {
			dObj.SetValue(IsCustomSerializableChildPropertyKey, value);
		}
		public static void SetStoreLayoutMode(DependencyObject obj, StoreLayoutMode value) {
			obj.SetValue(StoreLayoutModeProperty, value);
		}
		public static string GetSerializationIDDefault(DependencyObject obj) {
			return (string)obj.GetValue(SerializationIDDefaultProperty);
		}
		public static void SetSerializationIDDefault(DependencyObject obj, string value) {
			obj.SetValue(SerializationIDDefaultProperty, value);
		}
		public static string GetSerializationID(DependencyObject obj) {
			return (string)obj.GetValue(SerializationIDProperty);
		}
		public static void SetSerializationID(DependencyObject obj, string value) {
			obj.SetValue(SerializationIDProperty, value);
		}
		public static SerializationProvider GetSerializationProvider(DependencyObject obj) {
			return (SerializationProvider)obj.GetValue(SerializationProviderProperty);
		}
		public static void SetSerializationProvider(DependencyObject obj, SerializationProvider value) {
			obj.SetValue(SerializationProviderProperty, value);
		}
		public static string GetLayoutVersion(DependencyObject obj) {
			return (string)obj.GetValue(LayoutVersionProperty);
		}
		public static void SetLayoutVersion(DependencyObject obj, string value) {
			obj.SetValue(LayoutVersionProperty, value);
		}
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static bool GetIsProcessingSingleObject(DependencyObject obj) {
			return (bool)obj.GetValue(IsProcessingSingleObjectProperty);
		}
		static void SetIsProcessingSingleObject(DependencyObject obj, bool value) {
			obj.SetValue(IsProcessingSingleObjectPropertyKey, value);
		}
		#endregion static
		internal static bool IsDXSerializable(DependencyObject dObj) {
			SerializationProvider provider = GetSerializationProvider(dObj);
			return !string.IsNullOrEmpty(provider.GetSerializationID(dObj));
		}
		public static void Serialize(DependencyObject root, object path, string appName, DXOptionsLayout options) {
			AcceptNestedObjects acceptNested = (options == null) ? AcceptNestedObjects.Default : options.AcceptNestedObjects;
			SerializeCore(root, path, appName, options, acceptNested);
		}
		public static void Serialize(DependencyObject root, Stream stream, string appName, DXOptionsLayout options) {
			AcceptNestedObjects acceptNested = (options == null) ? AcceptNestedObjects.Default : options.AcceptNestedObjects;
			SerializeCore(root, stream, appName, options, acceptNested);
		}
		public static void Serialize(DependencyObject[] dObjects, object path, string appName, DXOptionsLayout options) {
			SerializeCore(dObjects, path, appName, options);
		}
		static bool SerializeCore(DependencyObject[] dObjects, object path, string appName, DXOptionsLayout options) {
			return new DXXmlSerializer().SerializeObject(new SerializationStore(dObjects), path, appName, options);
		}
		static bool SerializeCore(DependencyObject root, object path, string appName, DXOptionsLayout options, AcceptNestedObjects acceptNested) {
			return new DXXmlSerializer().SerializeObject(new SerializationStore(root, acceptNested), path, appName, options);
		}
		static bool SerializeCore(DependencyObject root, Stream stream, string appName, DXOptionsLayout options, AcceptNestedObjects acceptNested) {
			return new DXXmlSerializer().SerializeObject(new SerializationStore(root, acceptNested), stream, appName, options);
		}
		public static void Deserialize(DependencyObject root, object path, string appName, DXOptionsLayout options) {
			AcceptNestedObjects acceptNested = (options == null) ? AcceptNestedObjects.Default : options.AcceptNestedObjects;
			DeserializeCore(root, path, appName, options, acceptNested);
		}
		public static void Deserialize(DependencyObject root, Stream stream, string appName, DXOptionsLayout options) {
			AcceptNestedObjects acceptNested = (options == null) ? AcceptNestedObjects.Default : options.AcceptNestedObjects;
			DeserializeCore(root, stream, appName, options, acceptNested);
		}
		public static void Deserialize(DependencyObject[] dObjects, object path, string appName, DXOptionsLayout options) {
			DeserializeCore(dObjects, path, appName, options);
		}
		static void DeserializeCore(DependencyObject[] dObjects, object path, string appName, DXOptionsLayout options) {
			new DXXmlSerializer().DeserializeObject(new SerializationStore(dObjects), path, appName, options);
		}
		static void DeserializeCore(DependencyObject root, object path, string appName, DXOptionsLayout options, AcceptNestedObjects acceptNested) {
			new DXXmlSerializer().DeserializeObject(new SerializationStore(root, acceptNested), path, appName, options);
		}
		static void DeserializeCore(DependencyObject root, Stream stream, string appName, DXOptionsLayout options, AcceptNestedObjects acceptNested) {
			new DXXmlSerializer().DeserializeObject(new SerializationStore(root, acceptNested), stream, appName, options);
		}
		public static void SerializeSingleObject(DependencyObject d, object path, string appName) {
			SetIsProcessingSingleObject(d, true);
			try {
				SerializeCore(d, path, appName, null, AcceptNestedObjects.Nothing);
			} finally {
				SetIsProcessingSingleObject(d, false);
			}
		}
		public static void DeserializeSingleObject(DependencyObject d, object path, string appName) {
			SetIsProcessingSingleObject(d, true);
			try {
				DeserializeCore(d, path, appName, null, AcceptNestedObjects.Nothing);
			} finally {
				SetIsProcessingSingleObject(d, false);
			}
		}
		internal static void Accept(DependencyObject dObj, Action<IDXSerializable> visit) {
			Accept(dObj, AcceptNestedObjects.Default, visit);
		}
		static IList<DependencyObject> acceptedObjects = new List<DependencyObject>();
		internal static void Accept(DependencyObject dObj, AcceptNestedObjects acceptNested, Action<IDXSerializable> visit) {
			try {
				Func<DependencyObject, bool> nestedChilrenPredicate = obj =>
					acceptNested == AcceptNestedObjects.IgnoreChildrenOfDisabledObjects ?
						CanAccept(obj) : true;
				using(VisualTreeEnumerator enumerator = GetEnumerator(dObj, acceptNested, nestedChilrenPredicate)) {
					while(enumerator.MoveNext()) {
						if(!CanAccept(enumerator.Current)) continue;
						var parentIds = GetParentIDs(enumerator.GetVisualParents());
						var behaviors = (BehaviorCollection)enumerator.Current.GetValue(Interaction.BehaviorsProperty);
						if(behaviors != null) {
							foreach(var behavior in behaviors) {
								AcceptCore(behavior, visit, parentIds, nestedChilrenPredicate);
							}
						}
						AcceptCore(enumerator.Current, visit, parentIds, nestedChilrenPredicate);
					}
				}
			}
			finally {
				acceptedObjects.Clear();
				customSerializableChildren.Clear();
			}
		}
		static VisualTreeEnumerator GetEnumerator(DependencyObject dObj, AcceptNestedObjects acceptNested, Func<DependencyObject, bool> nestedChilrenPredicate) {
			VisualTreeEnumerator enumerator;
			switch(acceptNested) {
#if !SL
				case AcceptNestedObjects.IgnoreChildrenOfDisabledObjects:
				case AcceptNestedObjects.AllTree: enumerator = new SerializationEnumerator(dObj, nestedChilrenPredicate); break;
#endif
				case AcceptNestedObjects.Nothing: enumerator = new SingleObjectEnumerator(dObj); break;
				default: enumerator = new VisualTreeEnumerator(dObj); break;
			}
			return enumerator;
		}
		static bool CanAccept(DependencyObject dObj) {
			return GetEnabled(dObj) && !GetIsCustomSerializableChild(dObj) && !customSerializableChildren.Contains(dObj);
		}
		static void AcceptCore(DependencyObject dObj, Action<IDXSerializable> visit, IEnumerable<string> parentIDs, Func<DependencyObject, bool> nestedChilrenPredicate) {
			IDXSerializable dxObj = GetIDXSerializable(dObj, parentIDs);
			if(dxObj != null) {
				visit(dxObj);
				if(!GetIsProcessingSingleObject(dObj)) {
					parentIDs = parentIDs.Concat(GetParentIDs(new DependencyObject[] { dObj }));
					CheckCustomSerializableChildren(dObj, visit, parentIDs, nestedChilrenPredicate);
				}
			}
		}
		static void CheckCustomSerializableChildren(DependencyObject dObj, Action<IDXSerializable> visit, IEnumerable<string> parentIDs, Func<DependencyObject, bool> nestedChilrenPredicate) {
			CustomGetSerializableChildrenEventArgs e = new CustomGetSerializableChildrenEventArgs(dObj);
			SerializationProvider provider = GetSerializationProvider(dObj);
			if(provider != null) {
				provider.OnCustomGetSerializableChildren(dObj, e);
				foreach(DependencyObject child in e.Children.Where(nestedChilrenPredicate)) {
					AcceptChildVisualTree(child, visit, dObj, nestedChilrenPredicate);
				}
			}
		}
		static IList<DependencyObject> customSerializableChildren = new List<DependencyObject>();
		static void AcceptChildVisualTree(DependencyObject child, Action<IDXSerializable> visit, DependencyObject parent, Func<DependencyObject, bool> nestedChilrenPredicate) {
			var enumerator = GetEnumerator(child, AcceptNestedObjects.VisualTreeOnly, d => true);
			while(enumerator.MoveNext()) {
				DependencyObject dObj = enumerator.Current;
				if(!GetEnabled(dObj)) continue;
				IEnumerable<DependencyObject> parents = Enumerable.Concat(
					new DependencyObject[] { parent }, enumerator.GetVisualParents());
				if(IsDXSerializable(dObj))
					customSerializableChildren.Add(dObj);
				AcceptCore(dObj, visit, GetParentIDs(parents), nestedChilrenPredicate);
			}
		}
		static IDXSerializable GetIDXSerializable(DependencyObject dObj, IEnumerable<string> parentIDs) {
			return IsDXSerializable(dObj) ? new SerializationProviderWrapper(dObj, GetSerializationProvider(dObj), parentIDs) : null;
		}
		static IEnumerable<string> GetParentIDs(IEnumerable<DependencyObject> visualParents) {
			IEnumerable<DependencyObject> serializableParents =
			   visualParents.Where((obj) => IsDXSerializable(obj));
			return serializableParents.Select((obj) => GetSerializationProvider(obj).GetSerializationID(obj));
		}
	#region internal classes
		class SerializationStore : IXtraSerializable2 , IXtraSerializableLayout{
			public SerializationStore(DependencyObject[] dObjects) {
				if(dObjects == null || dObjects.Length == 0) return;
				Items = new Dictionary<string, IDXSerializable>();
				try {
					Array.ForEach(dObjects,
						delegate(DependencyObject dObj) {
							IDXSerializable dxObj = GetIDXSerializable(dObj, new string[] { });
							if(dxObj != null)
								CollectDXSerializableItem(dxObj);
						});
				}
				finally { acceptedObjects.Clear(); }
			}
			public SerializationStore(DependencyObject root, AcceptNestedObjects acceptNested) {
				versionCore = GetLayoutVersion(root);
				Items = new Dictionary<string, IDXSerializable>();
				if(root != null)
					Accept(root, acceptNested, CollectDXSerializableItem);
			}
			void CollectDXSerializableItem(IDXSerializable dxObj) {
				if(acceptedObjects.Contains(dxObj.Source)) return;
				acceptedObjects.Add(dxObj.Source);
				IDXSerializable serializableFromCache;
				if(Items.TryGetValue(dxObj.FullPath, out serializableFromCache)) {
					DuplicateSerializationIDException.Assert(dxObj, serializableFromCache);
				}
				else Items.Add(dxObj.FullPath, dxObj);
			}
			string versionCore;
			string IXtraSerializableLayout.LayoutVersion { get { return versionCore; } }
			public Dictionary<string, IDXSerializable> Items { get; private set; }
	#region IXtraSerializable2 Members
			void IXtraSerializable2.Deserialize(IList props) {
				IXtraPropertyCollection collection = props as IXtraPropertyCollection;
				string rootVersion = DXDeserializeHelper.GetLayoutVersion(collection);
				foreach(IDXSerializable dxSerializable in Items.Values) {
					XtraPropertyInfo pInfo;
					if(TryGetXtraPropertyInfo(props as IXtraPropertyCollection, dxSerializable, out pInfo)) {
						new DXDeserializeHelper(dxSerializable, rootVersion).DeserializeObject(dxSerializable.Source, pInfo.ChildProperties, null);
					}
				}
			}
			bool TryGetXtraPropertyInfo(IXtraPropertyCollection collection, IDXSerializable dxSerializable, out XtraPropertyInfo pInfo) {
				string name = XtraPropertyInfo.MakeXtraObjectInfoName(dxSerializable.FullPath);
				pInfo = collection[name];
				if(pInfo == null) {
					name = XtraPropertyInfo.MakeXtraObjectInfoName(DXSerializer.GetSerializationID(dxSerializable.Source));
					if(!string.IsNullOrEmpty(name))
						pInfo = collection[name];
				}
				return pInfo != null;
			}
			XtraPropertyInfo[] IXtraSerializable2.Serialize() {
				List<XtraPropertyInfo> props = new List<XtraPropertyInfo>(Items.Count);
				foreach(IDXSerializable dxSerializable in Items.Values) {
					XtraPropertyInfo pInfo = new XtraPropertyInfo(new XtraObjectInfo(dxSerializable.FullPath, dxSerializable.Source));
					pInfo.ChildProperties.AddRange(new DXSerializeHelper(dxSerializable).SerializeObject(dxSerializable.Source, null));
					props.Add(pInfo);
				}
				return props.ToArray();
			}
			#endregion
		}
		internal class DXSerializeHelper : SerializeHelper {
			protected readonly IDXSerializable DXObject;
			public DXSerializeHelper(IDXSerializable dxObj) {
				DXObject = dxObj;
			}
			protected internal object GetDXObj(object obj) {
				DXObject.EventTarget = obj;
				return (obj != DXObject) ? DXObject : obj;
			}
			protected override void RaiseStartSerializing(object obj) {
				base.RaiseStartSerializing(GetDXObj(obj));
			}
			protected override void RaiseEndSerializing(object obj) {
				base.RaiseEndSerializing(GetDXObj(obj));
			}
			protected override XtraPropertyInfo[] PerformManualSerialization(object obj) {
				return base.PerformManualSerialization(GetDXObj(obj));
			}
			protected override SerializationContext CreateSerializationContext() {
				return new DXSerializationContext();
			}
			protected override XtraPropertyInfo CreateXtraPropertyInfo(PropertyDescriptor prop, object value, bool isKey) {
				DependencyPropertyDescriptor dProp = DependencyPropertyDescriptor.FromProperty(prop);
				if(dProp != null && dProp.IsAttached) {
#if !SL
					return new AttachedPropertyInfo(prop.Name, prop.PropertyType, dProp.DependencyProperty.PropertyType, dProp.DependencyProperty.OwnerType, value, isKey);
#else
					return new AttachedPropertyInfo(prop.Name, prop.PropertyType, value);
#endif
				}
				return base.CreateXtraPropertyInfo(prop, value, isKey);
			}
		}
		internal class DXDeserializeHelper : DeserializeHelper {
			protected readonly IDXSerializable DXObject;
			protected readonly string rootVersion;
			public DXDeserializeHelper(IDXSerializable dxObj, string rootVersion) {
				DXObject = dxObj;
				this.rootVersion = rootVersion;
			}
			protected override string GetRootVersion() {
				return rootVersion;
			}
			protected internal object GetDXObj(object obj) {
				DXObject.EventTarget = obj;
				return (obj != DXObject) ? DXObject : obj;
			}
			protected override bool RaiseStartDeserializing(object obj, string restoredLayoutVersion) {
				return base.RaiseStartDeserializing(GetDXObj(obj), restoredLayoutVersion);
			}
			protected override void RaiseEndDeserializing(object obj, string restoredLayoutVersion) {
				base.RaiseEndDeserializing(GetDXObj(obj), restoredLayoutVersion);
			}
			protected override SerializationContext CreateSerializationContext() {
				return new DXSerializationContext();
			}
		}
		#endregion internal classes
	}
}
