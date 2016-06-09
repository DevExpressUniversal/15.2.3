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

using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Interactivity.Internal;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
#if NETFX_CORE
using Windows.UI.Xaml;
using DevExpress.Mvvm.Native;
#endif
namespace DevExpress.Mvvm.UI {
	public class DependencyPropertyBehavior : Behavior<DependencyObject> {
#if !NETFX_CORE
		const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
#endif
		public static readonly DependencyProperty BindingProperty = DependencyProperty.RegisterAttached("Binding", typeof(object), typeof(DependencyPropertyBehavior),
#if !SILVERLIGHT && !NETFX_CORE
			new FrameworkPropertyMetadata(null, (d, e) => ((DependencyPropertyBehavior)d).OnBindingPropertyChanged())  { BindsTwoWayByDefault = true } );
#else
			new PropertyMetadata(null, (d, e) => ((DependencyPropertyBehavior)d).OnBindingPropertyChanged()));
#endif
		EventTriggerEventSubscriber EventHelper;
		public DependencyPropertyBehavior() {
			EventHelper = new EventTriggerEventSubscriber(OnEvent);
		}
		public object Binding {
			get { return GetValue(BindingProperty); }
			set { SetValue(BindingProperty, value); }
		}
		public string PropertyName { get; set; }
		public string EventName { get; set; }
		protected PropertyInfo PropertyInfo {
#if !NETFX_CORE
			get { return PropertyAssociatedObject.GetType().GetProperty(ShortPropertyName, BindingFlags.Instance | BindingFlags.Public); }
#else
			get { return Mvvm.Native.TypeExtensions.GetProperty(PropertyAssociatedObject.GetType(), ShortPropertyName); }
#endif
		}
		protected object PropertyAssociatedObject {
			get { return GetAssociatedObjectForName(PropertyName); }
		}
		protected object EventAssociatedObject {
			get { return GetAssociatedObjectForName(EventName); }
		}
		protected string ShortEventName {
			get { return GetShortName(EventName); }
		}
		protected string ShortPropertyName {
			get { return GetShortName(PropertyName); }
		}
		protected override void OnAttached() {
			if(Binding != null) OnBindingPropertyChanged();
			if(string.IsNullOrEmpty(EventName)) return;
			EventHelper.UnsubscribeFromEvent(EventAssociatedObject, ShortEventName);
			EventHelper.SubscribeToEvent(EventAssociatedObject, ShortEventName);
		}
		protected override void OnDetaching() {
			if(string.IsNullOrEmpty(EventName)) return;
			EventHelper.UnsubscribeFromEvent(EventAssociatedObject, ShortEventName);
		}
		void OnEvent(object sender, object eventArgs) {
			Binding = PropertyInfo.GetValue(PropertyAssociatedObject, null);
		}
		void OnBindingPropertyChanged() {
			if(PropertyAssociatedObject == null)
				return;
			object oldValue = PropertyInfo.GetValue(PropertyAssociatedObject, null);
			if(object.Equals(oldValue, Binding)) return;
			if(PropertyInfo.CanWrite)
				PropertyInfo.SetValue(PropertyAssociatedObject, Convert.ChangeType(Binding, PropertyInfo.PropertyType, null), null);
		}
		protected virtual object GetAssociatedObjectForName(string name) {
			if(AssociatedObject == null)
				return null;
			var namePaths = name.Split('.');
			object currentObject = AssociatedObject;
			foreach(var propertyPath in namePaths.Take(namePaths.Length - 1)) {
#if !NETFX_CORE
				currentObject = currentObject.GetType().GetProperty(propertyPath, bindingFlags).GetValue(currentObject, null);
#else
				currentObject = Mvvm.Native.TypeExtensions.GetProperty(currentObject.GetType(), propertyPath).GetValue(currentObject, null);
#endif
			}
			return currentObject;
		}
		protected virtual string GetShortName(string name) {
			return name.Split('.').Last();
		}
	}
}
