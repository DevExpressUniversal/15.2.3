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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Mvvm.UI.Interactivity;
using System.Windows.Data;
namespace DevExpress.Xpf.Bars { 
	[ContentProperty("Value")]
	public class UpdateAction : BarManagerControllerActionBase {
		static readonly ReflectionHelper rHelper = new ReflectionHelper();
		public static readonly DependencyProperty ValueProperty;
		public static readonly DependencyProperty PropertyProperty;
		public static readonly DependencyProperty PropertyNameProperty;
		public static readonly DependencyProperty ElementNameProperty;
		public static readonly DependencyProperty ElementProperty;
		public DependencyProperty Property {
			get { return (DependencyProperty)GetValue(PropertyProperty); }
			set { SetValue(PropertyProperty, value); }
		}
		public string PropertyName {
			get { return (string)GetValue(PropertyNameProperty); }
			set { SetValue(PropertyNameProperty, value); }
		}
		public object Value {
			get { return (object)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		public BindingBase ValueBinding { get; set; }
		static UpdateAction() {
			ValueProperty = DependencyPropertyManager.Register("Value", typeof(object), typeof(UpdateAction), new FrameworkPropertyMetadata(null));
			PropertyProperty = DependencyPropertyManager.Register("Property", typeof(DependencyProperty), typeof(UpdateAction), new FrameworkPropertyMetadata(null));
			PropertyNameProperty = DependencyPropertyManager.Register("PropertyName", typeof(string), typeof(UpdateAction), new FrameworkPropertyMetadata(null));
			ElementNameProperty = CollectionAction.ElementNameProperty.AddOwner(typeof(UpdateAction));
			ElementProperty = CollectionAction.ElementProperty.AddOwner(typeof(UpdateAction));
		}
		public object Element {
			get { return GetValue(ElementProperty); }
			set { SetValue(ElementProperty, value); }
		}
		public string ElementName {
			get { return (string)GetValue(ElementNameProperty); }
			set { SetValue(ElementNameProperty, value); }
		}
		public override object GetObjectCore() {
			if (Element != null)
				return Element;
			var context = CollectionAction.GetContext(this);
			if (ElementName == null && context != null)
				return context;
			return CollectionActionHelper.Instance.FindElementByName(CollectionAction.GetContext(this), ElementName, Container, ScopeSearchSettings.Local | ScopeSearchSettings.Descendants);
		}
		protected override void ExecuteCore(DependencyObject associatedObject) {
			var element = GetObjectCore();
			if (element == null)
				return;
			var dElement = element as DependencyObject;
			if (ValueBinding != null && Property!=null && dElement!=null) {
				BindingOperations.SetBinding(dElement, Property, ValueBinding);
				return;
			}
			Action<object, object> setter = GetSetter(element);
			if (setter == null)
				return;
			setter(element, Value);
		}
		protected virtual Action<object, object> GetSetter(object element) {
			if (Property != null)
				return (obj, val) => ((DependencyObject)obj).SetValue(Property, val);
			if (PropertyName != null)
				return rHelper.GetInstanceMethodHandler<Action<object, object>>(element, "set_" + PropertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, element.GetType());
			return null;
		}
	}
}
