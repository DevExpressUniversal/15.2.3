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
using DevExpress.Mvvm.Native;
#if !NETFX_CORE
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
#else
using Windows.UI.Xaml;
using TypeExtensions = DevExpress.Mvvm.Native.TypeExtensions;
#endif
namespace DevExpress.Mvvm.UI.Interactivity.Internal {
	public static class InteractionHelper {
		public static readonly DependencyProperty BehaviorInDesignModeProperty =
			DependencyProperty.RegisterAttached("BehaviorInDesignMode", typeof(InteractionBehaviorInDesignMode), typeof(InteractionHelper),
			new PropertyMetadata(InteractionBehaviorInDesignMode.Default));
		public static InteractionBehaviorInDesignMode GetBehaviorInDesignMode(DependencyObject d) {
			return (InteractionBehaviorInDesignMode)d.GetValue(BehaviorInDesignModeProperty);
		}
		public static void SetBehaviorInDesignMode(DependencyObject d, InteractionBehaviorInDesignMode behavior) {
			d.SetValue(BehaviorInDesignModeProperty, behavior);
		}
		public static bool IsInDesignMode(DependencyObject obj) {
			bool res = ViewModelBase.IsInDesignMode;
			if(obj is AttachableObjectBase) {
				res = res && !((AttachableObjectBase)obj)._AllowAttachInDesignMode;
				return res;
			}
			if(obj != null) {
				res = res && GetBehaviorInDesignMode(obj) != InteractionBehaviorInDesignMode.AsWellAsNotInDesignMode;
				return res;
			}
			return res;
		}
	}
	public enum InteractionBehaviorInDesignMode { Default, AsWellAsNotInDesignMode }
	class EventTriggerGenericHandler<TSender, TArgs> {
		readonly Action<object, object> action;
		public EventTriggerGenericHandler(Action<object, object> action) {
			this.action = action;
		}
		public void Handler(TSender sender, TArgs args) {
			action(sender, args);
		}
	}
	class EventTriggerEventSubscriber {
		Action<object, object> EventHandler;
		Delegate subscribedEventHandler;
#if NETFX_CORE
		object handlerRegistrationToken;
#endif
		public EventTriggerEventSubscriber(Action<object, object> eventHandler) {
			EventHandler = eventHandler;
		}
		public void SubscribeToEvent(object obj, string eventName) {
			if(obj == null || string.IsNullOrEmpty(eventName)) return;
			Type objType = obj.GetType();
#if NETFX_CORE
			EventInfo eventInfo = TypeExtensions.GetEvent(objType, eventName);
#else
			EventInfo eventInfo = objType.GetEvent(eventName);
#endif
			if(eventInfo == null) {
				return;
			}
			this.subscribedEventHandler = GetEventHandlerToSubscrive(eventInfo.EventHandlerType);
			if(this.subscribedEventHandler == null) return;
#if NETFX_CORE
			this.handlerRegistrationToken = eventInfo.AddEventHandlerEx(obj, this.subscribedEventHandler);
#else
			eventInfo.AddEventHandler(obj, this.subscribedEventHandler);
#endif
		}
#if !SILVERLIGHT && !NETFX_CORE
		public void SubscribeToEvent(object obj, RoutedEvent routedEvent) {
			UIElement eventSource = obj as UIElement;
			if(eventSource == null || routedEvent == null) return;
			this.subscribedEventHandler = GetEventHandlerToSubscrive(routedEvent.HandlerType);
			if(this.subscribedEventHandler == null) return;
			eventSource.AddHandler(routedEvent, this.subscribedEventHandler);
		}
#endif
		public void UnsubscribeFromEvent(object obj, string eventName) {
			if(obj == null || string.IsNullOrEmpty(eventName)) return;
			if(this.subscribedEventHandler == null) return;
			Type type = obj.GetType();
#if NETFX_CORE
			EventInfo info = TypeExtensions.GetEvent(type, eventName);
			if (this.handlerRegistrationToken is Delegate)
				info.RemoveEventHandlerEx(obj, handlerRegistrationToken as Delegate);
			else
				info.RemoveEventHandlerEx(obj, handlerRegistrationToken);
#else
			EventInfo info = type.GetEvent(eventName);
			info.RemoveEventHandler(obj, this.subscribedEventHandler);
#endif
			this.subscribedEventHandler = null;
#if NETFX_CORE
			this.handlerRegistrationToken = null;
#endif
		}
#if !SILVERLIGHT && !NETFX_CORE
		public void UnsubscribeFromEvent(object obj, RoutedEvent routedEvent) {
			UIElement eventSource = obj as UIElement;
			if(eventSource == null || routedEvent == null) return;
			if(this.subscribedEventHandler == null) return;
			eventSource.RemoveHandler(routedEvent, this.subscribedEventHandler);
			this.subscribedEventHandler = null;
		}
#endif
		Delegate GetEventHandlerToSubscrive(Type eventHandlerType) {
			if(!IsEventCorrect(eventHandlerType)) return null;
			ParameterInfo[] parameters = GetParameters(eventHandlerType);
#if NETFX_CORE
			DevExpress.Mvvm.Native.DotNetNativeAssistant.AddTypeElement(typeof(EventTriggerGenericHandler<,>), new Type[] { parameters[0].ParameterType, parameters[1].ParameterType }, activate: DotNetNativePolicy.Public, dynamic: DotNetNativePolicy.RequiredPublic);
#endif
			Type handlerType = typeof(EventTriggerGenericHandler<,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType);
			object instance = Activator.CreateInstance(handlerType, new object[] { EventHandler });
#if !NETFX_CORE
			return Delegate.CreateDelegate(eventHandlerType, instance, instance.GetType().GetMethod("Handler"));
#else
			return TypeExtensions.GetMethod(instance.GetType(), "Handler").CreateDelegate(eventHandlerType, instance);
#endif
		}
		bool IsEventCorrect(Type eventHandlerType) {
#if !NETFX_CORE
			if(!typeof(Delegate).IsAssignableFrom(eventHandlerType)) return false;
#else
			if(!TypeExtensions.IsAssignableFrom(typeof(Delegate), eventHandlerType)) return false;
#endif
			ParameterInfo[] parameters = GetParameters(eventHandlerType);
			if(parameters.Length != 2) return false;
#if !NETFX_CORE
			if(!typeof(object).IsAssignableFrom(parameters[0].ParameterType)) return false;
			if(!typeof(object).IsAssignableFrom(parameters[1].ParameterType)) return false;
#else
			if(!TypeExtensions.IsAssignableFrom(typeof(object), parameters[0].ParameterType)) return false;
			if(!TypeExtensions.IsAssignableFrom(typeof(object), parameters[1].ParameterType)) return false;
#endif
			return true;
		}
		ParameterInfo[] GetParameters(Type eventHandlerType) {
#if !NETFX_CORE
			return eventHandlerType.GetMethod("Invoke").GetParameters();
#else
			return TypeExtensions.GetMethod(eventHandlerType, "Invoke").GetParameters();
#endif
		}
	}
}
