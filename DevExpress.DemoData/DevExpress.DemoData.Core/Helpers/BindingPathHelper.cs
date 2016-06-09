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
using System.Reflection;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.DemoData.Helpers {
	public static class BindingPathHelper {
		class BindingPathHelperBinding : DisposableBase {
			ValueOwner valueOwner;
			DepPropertyChangedEventHandler onValueChanged;
			public BindingPathHelperBinding(object target, string targetPropertyName, Binding binding) {
				PropertyInfo targetProperty = ReflectionHelper.GetPublicProperty(target.GetType(), targetPropertyName);
				if(targetProperty == null) return;
				valueOwner = new ValueOwner();
				valueOwner.Value = new object();
				onValueChanged = (s, e) => targetProperty.SetValue(target, e.NewValue, null);
				valueOwner.ValueChanged += onValueChanged;
				BindingOperations.SetBinding(valueOwner, ValueOwner.ValueProperty, binding);
			}
			protected override void DisposeManaged() {
				if(valueOwner != null) {
					valueOwner.ValueChanged -= onValueChanged;
					valueOwner.ClearValue(ValueOwner.ValueProperty);
				}
				base.DisposeManaged();
			}
		}
		class ValueOwner : DependencyObject {
			#region Dependency Properties
			public static readonly DependencyProperty ValueProperty;
			static ValueOwner() {
				Type ownerType = typeof(ValueOwner);
				ValueProperty = DependencyProperty.Register("Value", typeof(object), ownerType, new PropertyMetadata(null, RaiseValueChanged));
			}
			object valueValue = null;
			static void RaiseValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
				((ValueOwner)d).valueValue = e.NewValue;
				((ValueOwner)d).RaiseValueChanged(e);
			}
			#endregion
			public object Value { get { return valueValue; } set { SetValue(ValueProperty, value); } }
			public event DepPropertyChangedEventHandler ValueChanged;
			void RaiseValueChanged(DependencyPropertyChangedEventArgs e) {
				if(ValueChanged != null)
					ValueChanged(this, new DepPropertyChangedEventArgs(e));
			}
		}
		public static object GetValue(object source, string bindingPath) {
			if(source == null || bindingPath == null) return null;
			Binding conditionBinding = new Binding(bindingPath) { Source = source };
			ValueOwner valueOwner = new ValueOwner();
			BindingOperations.SetBinding(valueOwner, ValueOwner.ValueProperty, conditionBinding);
			object value = valueOwner.Value;
			valueOwner.SetValue(ValueOwner.ValueProperty, DependencyProperty.UnsetValue);
			return value;
		}
		public static IDisposable BindProperty(object target, string targetPropertyName, Binding binding) {
			return new BindingPathHelperBinding(target, targetPropertyName, binding);
		}
	}
}
