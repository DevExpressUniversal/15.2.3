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

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils.Serializing.Helpers;
using System.Linq;
#if !SILVERLIGHT
using System.ComponentModel;
#else
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SILVERLIGHT
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Core.Serialization {
	public class SerializationProvider {
		protected internal virtual DependencyObject GetSource(DependencyObject dObj) {
			return dObj;
		}
		protected internal virtual DependencyObject GetEventTarget(object obj) {
			return obj as DependencyObject;
		}
		protected internal virtual string GetLayoutVersion(DependencyObject dObj) {
			return DXSerializer.GetLayoutVersion(dObj);
		}
		protected internal virtual string GetSerializationID(DependencyObject dObj) {
			if(DXSerializer.GetIsProcessingSingleObject(dObj))
				return DXSerializer.GetSerializationIDDefault(dObj);
			string id = DXSerializer.GetSerializationID(dObj);
			if(string.IsNullOrEmpty(id))
				id = DXSerializer.GetSerializationIDDefault(dObj);
			return id;
		}
		protected internal virtual void OnStartSerializing(DependencyObject dObj) {
			if(dObj == null) return;
			RaiseEvent(new RoutedEventArgs(DXSerializer.StartSerializingEvent, dObj));
		}
		protected internal virtual void OnEndSerializing(DependencyObject dObj) {
			if(dObj == null) return;
			RaiseEvent(new RoutedEventArgs(DXSerializer.EndSerializingEvent, dObj));
		}
		protected internal virtual void OnStartDeserializing(DependencyObject dObj, LayoutAllowEventArgs ea) {
			if(dObj == null) return;
			if(!RaiseBeforeLoadLayout(dObj, ea.PreviousVersion)) {
				ea.Allow = false;
				return;
			}
			RaiseEvent(new StartDeserializingEventArgs(dObj, ea));
		}
		protected internal virtual void OnEndDeserializing(DependencyObject dObj, string restoredVersion) {
			if(dObj == null) return;
			RaiseEvent(new EndDeserializingEventArgs(dObj, restoredVersion));
			if(restoredVersion != GetLayoutVersion(dObj))
				RaiseLayoutUpgrade(dObj, restoredVersion);
		}
		protected internal virtual void OnCustomGetSerializableProperties(DependencyObject dObj, CustomGetSerializablePropertiesEventArgs e) {
			RaiseEvent(e);
		}
		protected internal virtual void OnCustomGetSerializableChildren(DependencyObject dObj, CustomGetSerializableChildrenEventArgs e) {
			RaiseEvent(e);
		}
		protected internal virtual void OnClearCollection(XtraItemRoutedEventArgs e) {
			RaiseEvent(e);
		}
		protected internal virtual object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			RaiseEvent(e);
			return e.CollectionItem;
		}
		protected internal virtual object OnFindCollectionItem(XtraFindCollectionItemEventArgs e) {
			RaiseEvent(e);
			return e.CollectionItem;
		}
		protected internal virtual bool OnAllowProperty(AllowPropertyEventArgs e) {
			RaiseEvent(e);
			return e.Allow;
		}
		protected internal virtual bool? OnCustomShouldSerializeProperty(CustomShouldSerializePropertyEventArgs e) {
			RaiseEvent(e);
			return e.CustomShouldSerialize;
		}
		protected internal virtual void OnResetProperty(ResetPropertyEventArgs e) {
			RaiseEvent(e);
			if(e.Handled || e.ResetPropertyMode == ResetPropertyMode.None)
				return;
			DependencyObject dObj = e.Source as DependencyObject;
			ResetPropertyMode resetPropertyMode = e.ResetPropertyMode;
			IList list = e.Property.GetValue(e.Source) as IList;
			bool isDependencyProperty = (dObj != null) && (e.DependencyProperty != null);
#if !SILVERLIGHT 
			System.Windows.Data.BindingMode mode = System.Windows.Data.BindingMode.Default;
#endif
			if(isDependencyProperty) {
				System.Windows.Data.BindingExpression be = dObj.ReadLocalValue(e.DependencyProperty) as System.Windows.Data.BindingExpression;
				if(be != null && be.ParentBinding != null) {
#if !SILVERLIGHT 
					mode = be.ParentBinding.Mode;
					if(mode == System.Windows.Data.BindingMode.Default) {
						var metadata = e.DependencyProperty.GetMetadata(dObj) as FrameworkPropertyMetadata;
						mode = (metadata != null) && metadata.BindsTwoWayByDefault ?
							System.Windows.Data.BindingMode.TwoWay : System.Windows.Data.BindingMode.OneWay;
					}
					if(!((mode == System.Windows.Data.BindingMode.TwoWay) || (mode == System.Windows.Data.BindingMode.OneWayToSource)))
						return;
#else
					return;
#endif
				}
			}
			if(resetPropertyMode == ResetPropertyMode.Auto) {
				if(list != null)
					resetPropertyMode = ResetPropertyMode.ClearCollection;
				else if(isDependencyProperty)
					resetPropertyMode = ResetPropertyMode.ClearValue;
				else
					resetPropertyMode = ResetPropertyMode.SetDefaultValue;
			}
			switch(resetPropertyMode) {
				case ResetPropertyMode.ClearValue:
					if(isDependencyProperty) {
#if !SILVERLIGHT 
						if(mode == System.Windows.Data.BindingMode.Default)
							dObj.ClearValue(e.DependencyProperty);
#else
						dObj.ClearValue(e.DependencyProperty);
#endif
					}
					break;
				case ResetPropertyMode.SetDefaultValue:
					object defaultValue = null;
					if(isDependencyProperty) {
#if !SILVERLIGHT 
						if(mode != System.Windows.Data.BindingMode.Default)
							break;
#endif
						defaultValue = e.DependencyProperty.GetMetadata(e.Property.ComponentType).DefaultValue;
					}
					else {
						var defaultValueAttribute = e.Property.Attributes[typeof(System.ComponentModel.DefaultValueAttribute)] as System.ComponentModel.DefaultValueAttribute;
						if(defaultValueAttribute != null)
							defaultValue = defaultValueAttribute.Value;
					}
					e.Property.SetValue(e.Source, defaultValue);
					break;
				case ResetPropertyMode.ClearCollection:
					if(list != null)
						list.Clear();
					break;
			}
		}
		protected internal virtual bool CustomDeserializeProperty(XtraPropertyInfoEventArgs e) {
			RaiseEvent(e);
			return e.Handled;
		}
		protected virtual bool RaiseBeforeLoadLayout(object obj, string restoredVersion) {
			BeforeLoadLayoutEventArgs ea = new BeforeLoadLayoutEventArgs(obj, restoredVersion);
			RaiseEvent(ea);
			return ea.Allow;
		}
		protected virtual void RaiseLayoutUpgrade(object obj, string restoredVersion) {
			RaiseEvent(new LayoutUpgradeEventArgs(obj, restoredVersion));
		}
		protected internal virtual bool OnShouldSerializeCollectionItem(XtraShouldSerailizeCollectionItemEventArgs e) {
			RaiseEvent(e);
			return e.ShouldSerailize;
		}
		protected internal virtual bool OnShouldSerializeProperty(object obj, PropertyDescriptor prop, XtraSerializableProperty xtraSerializableProperty) {
			DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(prop);
			if(dependencyPropertyDescriptor != null)
				return DXSerializationContext.ShouldSerializeDependencyProeprty(dependencyPropertyDescriptor, obj);
			return true;
		}
		protected internal virtual object OnCreateContentPropertyValue(XtraCreateContentPropertyValueEventArgs e) {
			RaiseEvent(e);
			return e.PropertyValue;
		}
		#region static
		static void RaiseEvent(RoutedEventArgs e) {
			UIElement element = e.Source as UIElement;
			if(element != null) {
				element.RaiseEvent(e);
			}
			else {
#if !SL
				FrameworkContentElement frameworkContentElement = e.Source as FrameworkContentElement;
#else
				DependencyObject frameworkContentElement = e.Source as DependencyObject;
#endif
				if(frameworkContentElement != null)
					frameworkContentElement.RaiseEvent(e);
			}
		}
		static readonly SerializationProvider instanceCore = new SerializationProvider();
		public static SerializationProvider Instance {
			get { return instanceCore; }
		}
		#endregion static
	}
}
namespace DevExpress.Xpf.Core.Serialization.Native {
	sealed class SerializationProviderWrapper : IDXSerializable, IXtraSerializable {
		public const string PathSeparator = "$";
		readonly DependencyObject SourceObject;
		readonly SerializationProvider Provider;
		readonly string pathCore;
		public SerializationProviderWrapper(DependencyObject dObj, SerializationProvider provider, IEnumerable<string> parentIDs) {
			SourceObject = dObj;
			Provider = provider;
			IEnumerable<string> pathElements = parentIDs.Concat(new string[] { GetSerializationID() });
			pathCore = string.Join(PathSeparator, pathElements.ToArray());
		}
		string GetSerializationID() {
			return Provider.GetSerializationID(SourceObject);
		}
		DependencyObject IDXSerializable.Source {
			get { return Provider.GetSource(SourceObject); }
		}
		string IDXSerializable.FullPath {
			get { return pathCore; }
		}
		public object EventTarget { get; set; }
		void IXtraSerializable.OnStartSerializing() {
			Provider.OnStartSerializing(Provider.GetEventTarget(EventTarget));
		}
		void IXtraSerializable.OnEndSerializing() {
			Provider.OnEndSerializing(Provider.GetEventTarget(EventTarget));
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs ea) {
			Provider.OnStartDeserializing(Provider.GetEventTarget(EventTarget), ea);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			Provider.OnEndDeserializing(Provider.GetEventTarget(EventTarget), restoredVersion);
		}
		public void OnClearCollection(XtraItemEventArgs e) {
			Provider.OnClearCollection(new XtraClearCollectionEventArgs(Provider.GetEventTarget(EventTarget), e));
		}
		public object OnCreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return Provider.OnCreateCollectionItem(
					new XtraCreateCollectionItemEventArgs(Provider.GetEventTarget(EventTarget), propertyName, e)
				);
		}
		public object OnFindCollectionItem(string propertyName, XtraItemEventArgs e) {
			return Provider.OnFindCollectionItem(
					new XtraFindCollectionItemEventArgs(Provider.GetEventTarget(EventTarget), propertyName, e)
				);
		}
		public bool AllowProperty(object obj, PropertyDescriptor prop, int id, bool isSerializing) {
			return Provider.OnAllowProperty(new AllowPropertyEventArgs(id, isSerializing, prop, obj));
		}
		public bool? CustomShouldSerializeProperty(object obj, PropertyDescriptor prop) {
			return Provider.OnCustomShouldSerializeProperty(new CustomShouldSerializePropertyEventArgs(prop, obj));
		}
		public void ResetProperty(object obj, PropertyDescriptor prop, XtraSerializableProperty attr) {
			XtraResetPropertyAttribute resetAttribute = prop.Attributes[typeof(XtraResetPropertyAttribute)] as XtraResetPropertyAttribute;
			if(resetAttribute == null && attr.Visibility == XtraSerializationVisibility.Visible)
				resetAttribute = XtraResetPropertyAttribute.DefaultInstance;	
			if(resetAttribute != null)
				Provider.OnResetProperty(new ResetPropertyEventArgs(resetAttribute.PropertyResetType, prop, obj));
		}
		public bool CustomDeserializeProperty(object obj, PropertyDescriptor property, XtraPropertyInfo propertyInfo) {
			return Provider.CustomDeserializeProperty(new XtraPropertyInfoEventArgs(DXSerializer.DeserializePropertyEvent, obj, property, propertyInfo));
		}
		public bool OnShouldSerailizaCollectionItem(XtraItemEventArgs e, object item) {
			return Provider.OnShouldSerializeCollectionItem(
					new XtraShouldSerailizeCollectionItemEventArgs(Provider.GetEventTarget(EventTarget), e, item)
				);
		}
		public bool OnShouldSerializeProperty(object obj, PropertyDescriptor prop, XtraSerializableProperty xtraSerializableProperty) {
			return Provider.OnShouldSerializeProperty(obj, prop, xtraSerializableProperty);
		}
		public object OnCreateContentPropertyValue(PropertyDescriptor propertyDescriptor, XtraItemEventArgs e) {
			return Provider.OnCreateContentPropertyValue(
					new XtraCreateContentPropertyValueEventArgs(Provider.GetEventTarget(EventTarget), propertyDescriptor, e)
				);
		}
	}
}
