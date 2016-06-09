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
using System.Windows;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PlatformIndependentDependencyPropertyManager = DevExpress.Xpf.Core.WPFCompatibility.DependencyPropertyManager;
#else
using PlatformIndependentDependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.Core.Native {
	public class DependencyPropertyChangedEventArgs<T> {
		T oldValue;
		T newValue;
		public DependencyPropertyChangedEventArgs(DependencyPropertyChangedEventArgs e) {
			oldValue = (T)e.OldValue;
			newValue = (T)e.NewValue;
		}
		public T OldValue { get { return oldValue; } }
		public T NewValue { get { return newValue; } }
	}
	public delegate void DependencyPropertyChangedCallback<Owner, T>(Owner owner, DependencyPropertyChangedEventArgs<T> e);
	public delegate T CoercePropertyValueCallback<Owner, T>(Owner owner, T baseValue);
	public class DependencyPropertyHelper {
		public static DependencyProperty RegisterProperty<Owner, T>(string name, T defaultValue) where Owner : class {
			return RegisterProperty<Owner, T>(name, defaultValue, null);
		}
		public static DependencyProperty RegisterProperty<Owner, T>(string name, T defaultValue, FrameworkPropertyMetadataOptions flags) where Owner : class {
			return RegisterProperty<Owner, T>(name, defaultValue, flags, null, null);
		}
		public static DependencyProperty RegisterProperty<Owner, T>(string name, T defaultValue, DependencyPropertyChangedCallback<Owner, T> changedCallback) where Owner : class {
			return RegisterProperty<Owner, T>(name, defaultValue, changedCallback, null);
		}
		public static DependencyProperty RegisterProperty<Owner, T>(string name, T defaultValue, FrameworkPropertyMetadataOptions flags, DependencyPropertyChangedCallback<Owner, T> changedCallback) where Owner : class {
			return PlatformIndependentDependencyPropertyManager.Register(name, typeof(T), typeof(Owner), new FrameworkPropertyMetadata(defaultValue, flags,
				changedCallback != null ? new PropertyChangedCallback((dependencyObject, propertyChangedEventArgs) => changedCallback(dependencyObject as Owner, new DependencyPropertyChangedEventArgs<T>(propertyChangedEventArgs))) : null)
				);
		}
		public static DependencyProperty RegisterProperty<Owner, T>(string name, T defaultValue, FrameworkPropertyMetadataOptions flags, DependencyPropertyChangedCallback<Owner, T> changedCallback, CoercePropertyValueCallback<Owner, T> coerceValueCallBack) where Owner : class {
			return PlatformIndependentDependencyPropertyManager.Register(name, typeof(T), typeof(Owner), new FrameworkPropertyMetadata(defaultValue, flags,
				changedCallback != null ? new PropertyChangedCallback((dependencyObject, propertyChangedEventArgs) => changedCallback(dependencyObject as Owner, new DependencyPropertyChangedEventArgs<T>(propertyChangedEventArgs))) : null,
				coerceValueCallBack != null ? new CoerceValueCallback((dependencyObject, baseValue) => coerceValueCallBack(dependencyObject as Owner, (T)baseValue)) : null)
				);
		}
		public static DependencyProperty RegisterProperty<Owner, T>(string name, T defaultValue, DependencyPropertyChangedCallback<Owner, T> changedCallback, CoercePropertyValueCallback<Owner, T> coerceValueCallBack) where Owner : class {
			return PlatformIndependentDependencyPropertyManager.Register(name, typeof(T), typeof(Owner), new FrameworkPropertyMetadata(defaultValue,
				changedCallback != null ? new PropertyChangedCallback((dependencyObject, propertyChangedEventArgs) => changedCallback(dependencyObject as Owner, new DependencyPropertyChangedEventArgs<T>(propertyChangedEventArgs))) : null,
				coerceValueCallBack != null ? new CoerceValueCallback((dependencyObject, baseValue) => coerceValueCallBack(dependencyObject as Owner, (T)baseValue)) : null)
				);
		}
		public static DependencyPropertyKey RegisterReadOnlyProperty<Owner, T>(string name, T defaultValue) where Owner : class {
			return PlatformIndependentDependencyPropertyManager.RegisterReadOnly(name, typeof(T), typeof(Owner), new FrameworkPropertyMetadata(defaultValue));
		}
		public static DependencyPropertyKey RegisterReadOnlyProperty<Owner, T>(string name, T defaultValue, DependencyPropertyChangedCallback<Owner, T> changedCallback) where Owner : class {
			return PlatformIndependentDependencyPropertyManager.RegisterReadOnly(name, typeof(T), typeof(Owner), new FrameworkPropertyMetadata(defaultValue, new PropertyChangedCallback((dependencyObject, propertyChangedEventArgs) => changedCallback(dependencyObject as Owner, new DependencyPropertyChangedEventArgs<T>(propertyChangedEventArgs)))));
		}
		public static DependencyProperty RegisterAttachedProperty<Owner, T>(string name, T defaultValue, FrameworkPropertyMetadataOptions options, DependencyPropertyChangedCallback<Owner, T> changedCallback, CoercePropertyValueCallback<Owner, T> coerceValueCallBack) where Owner : class {
			return PlatformIndependentDependencyPropertyManager.RegisterAttached(name, typeof(T), typeof(Owner), new FrameworkPropertyMetadata(defaultValue, options,
				changedCallback != null ? new PropertyChangedCallback((dependencyObject, propertyChangedEventArgs) => changedCallback(dependencyObject as Owner, new DependencyPropertyChangedEventArgs<T>(propertyChangedEventArgs))) : null,
				coerceValueCallBack != null ? new CoerceValueCallback((dependencyObject, baseValue) => coerceValueCallBack(dependencyObject as Owner, (T)baseValue)) : null)
				);
		}
		public static DependencyProperty RegisterAttachedProperty<Owner, T>(string name, T defaultValue, FrameworkPropertyMetadataOptions options, DependencyPropertyChangedCallback<Owner, T> changedCallback) where Owner : class {
			return RegisterAttachedPropertyCore<Owner, T>( name, defaultValue,  options, 
							changedCallback != null ? new PropertyChangedCallback((dependencyObject, propertyChangedEventArgs) => changedCallback(dependencyObject as Owner, new DependencyPropertyChangedEventArgs<T>(propertyChangedEventArgs))) : null);
		}
		public static DependencyProperty RegisterAttachedPropertyCore<Owner, T>(string name, T defaultValue, FrameworkPropertyMetadataOptions options, PropertyChangedCallback changedCallback) {
			return PlatformIndependentDependencyPropertyManager.RegisterAttached(
				name,
				typeof(T),
				typeof(Owner),
				new FrameworkPropertyMetadata(
					defaultValue,
					options,
					changedCallback
					)
				);
		}
	}
}
