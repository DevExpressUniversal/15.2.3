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
using System.ComponentModel;
using System.Windows;
using System.Collections;
using DevExpress.Utils;
using System.Collections.Specialized;
using DevExpress.Xpf.Core;
using System.Reflection;
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Grid.Native {
	public enum CloneDetailMode { Default, Skip, Force }
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public class CloneDetailModeAttribute : Attribute {
		public CloneDetailMode Mode { get; private set; }
		public CloneDetailModeAttribute(CloneDetailMode mode) {
			this.Mode = mode;
		}
	}
	public interface IDetailElement<T> where T : class {
		T CreateNewInstance(params object[] args);
	}
	public interface IDataControlOriginationElement {
		void NotifyPropertyChanged(DataControlBase sourceControl, DependencyProperty property, Func<DataControlBase, DependencyObject> getTarget, Type baseComponentType);
		void NotifyBeginInit(DataControlBase sourceElement, Func<DataControlBase, ISupportInitialize> getTarget);
		void NotifyEndInit(DataControlBase sourceElement, Func<DataControlBase, ISupportInitialize> getTarget);
		void NotifyCollectionChanged(DataControlBase sourceControl, Func<DataControlBase, IList> getCollection, Func<object, object> convertAction, NotifyCollectionChangedEventArgs e);
		DataControlBase GetOriginationControl(DataControlBase sourceControl);
	}
	public class NullDataControlOriginationElement : IDataControlOriginationElement {
		public static readonly IDataControlOriginationElement Instance = new NullDataControlOriginationElement();
		NullDataControlOriginationElement() { }
		void IDataControlOriginationElement.NotifyPropertyChanged(DataControlBase sourceControl, DependencyProperty property, Func<DataControlBase, DependencyObject> getTarget, Type baseComponentType) { }
		void IDataControlOriginationElement.NotifyBeginInit(DataControlBase sourceElement, Func<DataControlBase, ISupportInitialize> getTarget) { }
		void IDataControlOriginationElement.NotifyEndInit(DataControlBase sourceElement, Func<DataControlBase, ISupportInitialize> getTarget) { }
		void IDataControlOriginationElement.NotifyCollectionChanged(DataControlBase sourceControl, Func<DataControlBase, IList> getCollection, Func<object, object> convertAction, NotifyCollectionChangedEventArgs e) { }
		DataControlBase IDataControlOriginationElement.GetOriginationControl(DataControlBase sourceControl) { return sourceControl; }
	}
	public interface IConvertClonePropertyValue {
		object ConvertClonePropertyValue(string propertyName, object sourceValue, DependencyObject destinationObject);
	}
	public static class CloneDetailHelper {
		#region inner classes
		public class ReadonlyDependencyPropertyDescriptor : PropertyDescriptor {
			readonly PropertyDescriptor property;
			readonly DependencyPropertyKey propertyKey;
			public ReadonlyDependencyPropertyDescriptor(PropertyDescriptor property, DependencyPropertyKey propertyKey)
				: base(property) {
				this.property = property;
				this.propertyKey = propertyKey;
			}
			public override bool CanResetValue(object component) { return property.CanResetValue(component); }
			public override Type ComponentType { get { return property.ComponentType; } }
			public override object GetValue(object component) { return property.GetValue(component); }
			public override bool IsReadOnly { get { return false; } }
			public override Type PropertyType { get { return property.PropertyType; } }
			public override void ResetValue(object component) { throw new NotImplementedException(); }
			public override void SetValue(object component, object value) { ((DependencyObject)component).SetValue(propertyKey, value); }
			public override bool ShouldSerializeValue(object component) { throw new NotImplementedException(); }
		}
		#endregion
		static readonly Dictionary<Type, PropertyDescriptorCollection> propertyCache = new Dictionary<Type, PropertyDescriptorCollection>();
		static readonly Dictionary<Type, DependencyPropertyKey[]> propertyKeys = new Dictionary<Type, DependencyPropertyKey[]>();
		static readonly HashSet<DependencyProperty> attachedProperties = new HashSet<DependencyProperty>(); 
		public static void RegisterKnownPropertyKeys(Type ownerType, params DependencyPropertyKey[] knownKeys) {
			propertyKeys.Add(ownerType, knownKeys);
		}
		public static void RegisterKnownAttachedProperty(DependencyProperty property) {
			attachedProperties.Add(property);
		}
		public static bool IsKnownAttachedProperty(DependencyProperty property) {
			return attachedProperties.Contains(property);
		}
		internal static PropertyDescriptorCollection GetCloneProperties(Type componentType, Type baseComponentType) {
			PropertyDescriptorCollection result;
			if(!propertyCache.TryGetValue(componentType, out result)) {
				List<PropertyDescriptor> list = new List<PropertyDescriptor>();
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(componentType);
				foreach(PropertyDescriptor property in properties) {
					PropertyDescriptor propertyToCopy = GetCopyProperty(property, componentType, baseComponentType);
					if(propertyToCopy != null)
						list.Add(propertyToCopy);
				}
				propertyCache[componentType] = result = new PropertyDescriptorCollection(list.ToArray());
			}
			return result;
		}
		static PropertyDescriptor GetCopyProperty(PropertyDescriptor property, Type componentType, Type baseComponentType) {
			CloneDetailModeAttribute cloneDetailModeAttribute = property.Attributes[typeof(CloneDetailModeAttribute)] as CloneDetailModeAttribute;
			if(cloneDetailModeAttribute != null) {
				if(cloneDetailModeAttribute.Mode == CloneDetailMode.Skip)
					return null;
				if(cloneDetailModeAttribute.Mode == CloneDetailMode.Force) {
					DependencyPropertyKey key = FindPropertyKey(componentType, property) ?? FindPropertyKey(baseComponentType, property);
					return new ReadonlyDependencyPropertyDescriptor(property, key);
				}
			}
			if(property.IsReadOnly)
				return null;
			if(property.Name == "Resources")
				return null;
			if(!baseComponentType.IsAssignableFrom(property.ComponentType))
				return null;
			return property;
		}
		static DependencyPropertyKey FindPropertyKey(Type type, PropertyDescriptor property) {
			DependencyPropertyKey key = FindPropertyKeyHelper(type, property);
			if(key == null && type.BaseType != null) 
				return FindPropertyKey(type.BaseType, property);
			return key;
		}
		static DependencyPropertyKey FindPropertyKeyHelper(Type type, PropertyDescriptor property) {
			return propertyKeys.ContainsKey(type) ? propertyKeys[type].FirstOrDefault(key => key.DependencyProperty.GetName() == property.Name) : null;
		}
#if DEBUGTEST
		internal static event EventHandler NewInstanceCreated;
#endif
		public static T CreateElement<T>(T source, object[] args = null) where T : class {
			T destination = ((IDetailElement<T>)source).CreateNewInstance(args);
#if DEBUGTEST
			if(NewInstanceCreated != null)
				NewInstanceCreated(destination, EventArgs.Empty);
#endif
			return destination;
		}
		public static T CloneElement<T>(T source, Action<T> innerCloneAction = null, Func<T, Locker> getCloneDetailLockerAction = null, object[] args = null) where T : DependencyObject {
			T destination = CreateElement(source, args);
			CloneElement<T>(source, destination, innerCloneAction, getCloneDetailLockerAction);
			return destination;
		}
		public static void CloneElement<T>(T source, T destination, Action<T> innerCloneAction = null, Func<T, Locker> getCloneDetailLockerAction = null) where T : DependencyObject {
			Action copyToElementAction = () => CopyToElement<T>(source, destination, innerCloneAction);
			if(getCloneDetailLockerAction != null) {
				getCloneDetailLockerAction(destination).DoLockedAction(copyToElementAction);
			} else {
				copyToElementAction();
			}
		}
		public static void CopyToElement<T>(T source, T destination, Action<T> innerCloneAction = null) where T : DependencyObject {
			ISupportInitialize supportInitialize = destination as ISupportInitialize;
			if(supportInitialize != null)
				supportInitialize.BeginInit();
			PropertyDescriptorCollection properties = CloneDetailHelper.GetCloneProperties(destination.GetType(), typeof(T));
			foreach(PropertyDescriptor property in properties) {
				object currentValue = property.GetValue(destination);
				object newValue = property.GetValue(source);
				if(!object.Equals(currentValue, newValue))
					SetClonePropertyValue(source, property, newValue, destination);
			}
			if(innerCloneAction != null)
				innerCloneAction(destination);
			if(supportInitialize != null)
				supportInitialize.EndInit();
		}
		public static void SetClonePropertyValue(DependencyObject sourceObject, PropertyDescriptor property, object sourceValue, DependencyObject destinationObject) {
			property.SetValue(destinationObject, ConvertClonePropertyValue(sourceObject, property.Name, sourceValue, destinationObject));
		}
		static object ConvertClonePropertyValue(DependencyObject sourceObject, string propertyName, object sourceValue, DependencyObject destinationObject) {
			IConvertClonePropertyValue convertClonePropertyValue = sourceObject as IConvertClonePropertyValue;
			if(convertClonePropertyValue == null)
				return sourceValue;
			return convertClonePropertyValue.ConvertClonePropertyValue(propertyName, sourceValue, destinationObject);
		}
		public static void CloneCollection<T>(IList source, IList destination) where T : DependencyObject {
			CloneCollectionCore<T>(source, destination, item => CloneElement(item));
		}
		public static void CloneSimpleCollection<T>(IList source, IList destination, object[] args = null) where T : class {
			CloneCollectionCore<T>(source, destination, item => CreateElement(item, args));
		}
		static void CloneCollectionCore<T>(IList source, IList destination, Func<T, T> clone) where T : class {
			if(source.Count == 0) return;
			ILockable lockableCollection = destination as ILockable;
			if(lockableCollection != null)
				lockableCollection.BeginUpdate();
			foreach(T item in source) {
				destination.Add(clone(item));
			}
			if(lockableCollection != null)
				lockableCollection.EndUpdate();
		}
		public static void CopyToCollection<T>(IList source, IList destination) where T : DependencyObject {
			if(destination.Count == 0) {
				CloneCollection<T>(source, destination);
				return;
			}
			if(source.Count != destination.Count)
				throw new ArgumentException("source.Count != destination.Count");
			int count = source.Count;
			for(int i = 0; i < count; i++) {
				CopyToElement<T>((T)source[i], (T)destination[i]);
			}
		}
		public static T SafeGetDependentCollectionItem<T>(T item, ISupportGetCachedIndex<T> sourceCollection, IList destinationCollection) where T : DependencyObject {
			int index = sourceCollection.GetCachedIndex(item);
			return index >= 0 ? (T)destinationCollection[index] : null;
		}
	}
	internal static class DataControlOriginationElementHelper {
		public static void EnumerateDependentElemets<T>(DataControlBase sourceControl, Func<DataControlBase, T> getTarget, Action<T> targetInOpenDetailHandler, Action<T> targetInClosedDetailHandler = null) {
			EnumerateDependentElemetsCore<T>(sourceControl, getTarget, targetInOpenDetailHandler, targetInClosedDetailHandler, false, false);
		}
		public static void EnumerateDependentElemetsSkipOriginationControl<T>(DataControlBase sourceControl, Func<DataControlBase, T> getTarget, Action<T> targetInOpenDetailHandler, Action<T> targetInClosedDetailHandler = null) {
			EnumerateDependentElemetsCore<T>(sourceControl, getTarget, targetInOpenDetailHandler, targetInClosedDetailHandler, false, true);
		}
		public static void EnumerateDependentElemetsIncludingSource<T>(DataControlBase sourceControl, Func<DataControlBase, T> getTarget, Action<T> targetInOpenDetailHandler, Action<T> targetInClosedDetailHandler = null) {
			EnumerateDependentElemetsCore<T>(sourceControl, getTarget, targetInOpenDetailHandler, targetInClosedDetailHandler, true, false);
		}
		static void EnumerateDependentElemetsCore<T>(DataControlBase sourceControl, Func<DataControlBase, T> getTarget, Action<T> targetInOpenDetailHandler, Action<T> targetInClosedDetailHandler, bool includeSourceControl, bool skipOriginationControl) {
			DataControlBase dataControl = sourceControl.GetOriginationDataControl();
			T source = getTarget(sourceControl);
			if(!skipOriginationControl && (includeSourceControl || sourceControl != dataControl)) {
				targetInOpenDetailHandler(getTarget(dataControl));
			}
			foreach(DataControlBase clone in dataControl.DetailClones) {
				if(includeSourceControl || clone != sourceControl) {
					if(((RowDetailInfoBase)clone.DataControlParent).IsExpanded)
						targetInOpenDetailHandler(getTarget(clone));
					else if(targetInClosedDetailHandler != null)
						targetInClosedDetailHandler(getTarget(clone));
				}
			}
		}
		public static void UpdateActualTemplateSelector(DependencyObject target, DependencyObject originationObject, DependencyPropertyKey propertyKey, DataTemplateSelector selector, DataTemplate template, Func<DataTemplateSelector, DataTemplate, DataTemplateSelector> createWrapper = null) {
			if(createWrapper == null)
				createWrapper = (s, t) => new ActualTemplateSelectorWrapper(s, t);
			target.SetValue(propertyKey, originationObject != null ? originationObject.GetValue(propertyKey.DependencyProperty) : createWrapper(selector, template));
		}
	}
}
