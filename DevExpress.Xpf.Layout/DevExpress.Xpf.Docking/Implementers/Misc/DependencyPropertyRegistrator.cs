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

using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Docking {
	internal class DependencyPropertyRegistrator<OwnerType> where OwnerType : class {
		public void Register<T>(string name, ref DependencyProperty property, T defValue, 
			FrameworkPropertyMetadataOptions options, PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
			property = DependencyPropertyManager.Register(name, typeof(T), typeof(OwnerType),
				new FrameworkPropertyMetadata(defValue, options, changed, coerce));
		}
		public void Register<T>(string name, ref DependencyProperty property,
			T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
			property = DependencyPropertyManager.Register(name, typeof(T), typeof(OwnerType), new PropertyMetadata(defValue, changed, coerce));
		}
		public void RegisterReadonly<T>(string name, ref DependencyPropertyKey propertyKey, ref DependencyProperty property,
			T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
			propertyKey = DependencyPropertyManager.RegisterReadOnly(name, typeof(T), typeof(OwnerType), new PropertyMetadata(defValue, changed, coerce));
			property = propertyKey.DependencyProperty;
		}
		public void RegisterAttached<T>(string name, ref DependencyProperty property,
			T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
			property = DependencyPropertyManager.RegisterAttached(name, typeof(T), typeof(OwnerType), new PropertyMetadata(defValue, changed, coerce));
		}
		public void RegisterAttached<T>(string name, ref DependencyProperty property, FrameworkPropertyMetadataOptions flags,
			T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
			property = DependencyPropertyManager.RegisterAttached(name, typeof(T), typeof(OwnerType), new FrameworkPropertyMetadata(defValue, flags, changed, coerce));
		}
		public void RegisterAttachedReadonly<T>(string name, ref DependencyPropertyKey propertyKey, ref DependencyProperty property,
			T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
			propertyKey = DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(T), typeof(OwnerType), new PropertyMetadata(defValue, changed, coerce));
			property = propertyKey.DependencyProperty;
		}
		public void RegisterAttachedInherited<T>(string name, ref DependencyProperty property,
			T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
			property = DependencyPropertyManager.RegisterAttached(name, typeof(T), typeof(OwnerType), 
#if(!SILVERLIGHT)
				new FrameworkPropertyMetadata(defValue, FrameworkPropertyMetadataOptions.Inherits, changed, coerce));
#else
				new PropertyMetadata(defValue, changed, coerce));
#endif
		}
		public void RegisterAttachedReadonlyInherited<T>(string name, ref DependencyPropertyKey propertyKey, ref DependencyProperty property,
			T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
			propertyKey = DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(T), typeof(OwnerType),
#if(!SILVERLIGHT)
				new FrameworkPropertyMetadata(defValue, FrameworkPropertyMetadataOptions.Inherits, changed, coerce));
#else
				new PropertyMetadata(defValue, changed, coerce));
#endif
			property = propertyKey.DependencyProperty;
		}
		public DependencyProperty AddOwner<T>(string name, DependencyProperty property, T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
#if(!SILVERLIGHT)
			return property.AddOwner(typeof(OwnerType), new PropertyMetadata(defValue, changed, coerce));
#else
			return DependencyPropertyManager.Register(name, typeof(T), typeof(OwnerType), new PropertyMetadata(defValue, changed, coerce));
#endif
		}
		public void OverrideMetadata<T>(DependencyProperty property, T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
#if(!SILVERLIGHT)
			property.OverrideMetadata(typeof(OwnerType), new PropertyMetadata(defValue, changed, coerce));
#endif
		}
		public void OverrideMetadata<T>(DependencyPropertyKey propertyKey, T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
#if(!SILVERLIGHT)
			propertyKey.OverrideMetadata(typeof(OwnerType), new PropertyMetadata(defValue, changed, coerce));
#endif
		}
		public void OverrideDefaultStyleKey(DependencyProperty propertyKey) { 
#if(!SILVERLIGHT)
			propertyKey.OverrideMetadata(typeof(OwnerType), new FrameworkPropertyMetadata(typeof(OwnerType)));
#endif
		}
		public void OverrideUIMetadata<T>(DependencyProperty property, T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
#if(!SILVERLIGHT)
			property.OverrideMetadata(typeof(OwnerType), new UIPropertyMetadata(defValue, changed, coerce));
#endif
		}
		public void RegisterDirectEvent<THandler>(string name, ref RoutedEvent routedEvent) {
			routedEvent = EventManager.RegisterRoutedEvent(name, RoutingStrategy.Direct, typeof(THandler), typeof(OwnerType));
		}
		public void OverrideFrameworkMetadata<T>(DependencyProperty property, T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
#if(!SILVERLIGHT)
			property.OverrideMetadata(typeof(OwnerType), new FrameworkPropertyMetadata(defValue, changed, coerce));
#endif
		}
		public DependencyProperty OverrideFrameworkMetadata<T>(string name, DependencyProperty property, T defValue = default(T), 
			PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
			DependencyProperty internalProperty = property;
#if(!SILVERLIGHT)
			property.OverrideMetadata(typeof(OwnerType), new FrameworkPropertyMetadata(defValue, changed, coerce));
#else
			Register(name + "Internal", ref internalProperty, defValue,
					(dObj, e) => PropertyChanged(dObj, property, e.NewValue, coerce)
				);
#endif
			return internalProperty;
		}
		static int lockCounter = 0;
		static void PropertyChanged(DependencyObject dObj, DependencyProperty property, object value, CoerceValueCallback coerce) {
			if(lockCounter > 0) return;
			lockCounter++;
			object coerceResult = coerce(dObj, value);
			if(!object.Equals(coerceResult, value))
				dObj.SetValue(property, coerceResult);
			lockCounter--;
		}
		public void OverrideMetadataNotDataBindable<T>(DependencyProperty property, T defValue = default(T), PropertyChangedCallback changed = null, CoerceValueCallback coerce = null) {
#if(!SILVERLIGHT)
			property.OverrideMetadata(typeof(OwnerType), new FrameworkPropertyMetadata(defValue, FrameworkPropertyMetadataOptions.NotDataBindable, changed, coerce));
#endif
		}
		public void RegisterClassCommandBinding(ICommand command, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute) {
			CommandManager.RegisterClassCommandBinding(typeof(OwnerType), new CommandBinding(
				command, executed, canExecute));
		}
	}
}
