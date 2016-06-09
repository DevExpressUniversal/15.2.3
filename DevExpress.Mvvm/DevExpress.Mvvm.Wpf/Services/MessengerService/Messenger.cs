#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Reflection;
using System.Threading;
using System.Windows;
using DevExpress.Mvvm.Internal;
using DevExpress.Mvvm.Native;
#if !NETFX_CORE
using System.Windows.Threading;
#else
using Windows.UI.Xaml;
using Windows.UI.Core;
#endif
namespace DevExpress.Mvvm {
	public class Messenger : IMessenger {
		#region Inner Classes
		public struct ActionInvokerTokenPair {
			public readonly IActionInvoker ActionInvoker;
			public readonly object Token;
			public ActionInvokerTokenPair(IActionInvoker actionInvoker, object token) {
				ActionInvoker = actionInvoker;
				Token = token;
			}
		}
		public class ActionInvokerCollection : FuzzyDictionary<Type, List<ActionInvokerTokenPair>> {
			public ActionInvokerCollection()
				: base(TypeInclude) {
			}
			static bool TypeInclude(Type baseType, Type type) {
#if NETFX_CORE
				return type.IsSubclassOf(baseType) || Native.TypeExtensions.IsAssignableFrom(baseType, type);
#else
				return type.IsSubclassOf(baseType) || baseType.IsAssignableFrom(type);
#endif
			}
			public void Register(object token, bool receiveInheritedMessagesToo, Type messageType, IActionInvoker actionInvoker) {
				List<ActionInvokerTokenPair> list;
				if(!TryGetValue(messageType, receiveInheritedMessagesToo, out list)) {
					list = new List<ActionInvokerTokenPair>();
					Add(messageType, list, receiveInheritedMessagesToo);
				}
				list.Add(new ActionInvokerTokenPair(actionInvoker, token));
			}
			public void CleanUp() {
				var removeLists = new List<FuzzyKeyValueVair<Type, List<ActionInvokerTokenPair>>>();
				foreach(var list in this) {
					foreach(ActionInvokerTokenPair removeAction in new List<ActionInvokerTokenPair>(list.Value)) {
						if(removeAction.ActionInvoker.Target != null) continue;
						list.Value.Remove(removeAction);
					}
					if(list.Value.Count == 0)
						removeLists.Add(list);
				}
				foreach(var pair in removeLists) {
					Remove(pair.Key, pair.UseIncludeCondition);
				}
			}
			public void Unregister(object recipient, object token, Delegate action, Type messageType) {
				if(recipient == null) return;
				foreach(List<ActionInvokerTokenPair> list in GetValues(messageType)) {
					foreach(ActionInvokerTokenPair item in list) {
						if(token == null || token.Equals(item.Token)) { 
							item.ActionInvoker.ClearIfMatched(action, recipient);
						}
					}
				}
			}
			public void Send(object message, Type messageTargetType, object token, Type messageType) {
				foreach(List<ActionInvokerTokenPair> list in GetValues(messageType)) {
					foreach(ActionInvokerTokenPair item in list.ToArray()) { 
						if(object.Equals(item.Token, token))
							item.ActionInvoker.ExecuteIfMatched(messageTargetType, message);
					}
				}
			}
		}
#endregion
#if !SILVERLIGHT && !NETFX_CORE
		const DispatcherPriority CleanUpPriority = DispatcherPriority.ApplicationIdle;
#endif
		static readonly object defaultMessengerLock = new object();
		static IMessenger defaultMessenger;
		public static IMessenger Default {
			get {
				if(defaultMessenger == null) {
					lock(defaultMessengerLock) {
						if(defaultMessenger == null)
							defaultMessenger = new Messenger();
					}
				}
				return defaultMessenger;
			}
			set { defaultMessenger = value; }
		}
		bool isMultiThreadSafe;
		ActionInvokerCollection actionInvokers = new ActionInvokerCollection();
		IActionInvokerFactory actionInvokerFactory;
		bool cleanupScheduled;
		public Messenger() :
			this(false) { }
		public Messenger(bool isMultiThreadSafe,
			ActionReferenceType actionReferenceType = ActionReferenceType.WeakReference) :
			this(isMultiThreadSafe, CreateActionInvokerFactory(actionReferenceType)) { }
		public Messenger(bool isMultiThreadSafe, IActionInvokerFactory actionInvokerFactory) {
			this.actionInvokerFactory = actionInvokerFactory;
			this.isMultiThreadSafe = isMultiThreadSafe;
		}
		public virtual void Register<TMessage>(object recipient, object token, bool receiveInheritedMessages, Action<TMessage> action) {
			try {
				if(isMultiThreadSafe)
					Monitor.Enter(actionInvokers);
				IActionInvoker actionInvoker = actionInvokerFactory.CreateActionInvoker<TMessage>(recipient, action);
				RegisterCore(token, receiveInheritedMessages, typeof(TMessage), actionInvoker);
			} finally {
				if(isMultiThreadSafe)
					Monitor.Exit(actionInvokers);
			}
			RequestCleanup();
		}
		protected void RegisterCore(object token, bool receiveInheritedMessages, Type messageType, IActionInvoker actionInvoker) {
			actionInvokers.Register(token, receiveInheritedMessages, messageType, actionInvoker);
		}
		public virtual void Send<TMessage>(TMessage message, Type messageTargetType, object token) {
			try {
				if(isMultiThreadSafe)
					Monitor.Enter(actionInvokers);
				actionInvokers.Send(message, messageTargetType, token, typeof(TMessage)); 
			} finally {
				if(isMultiThreadSafe)
					Monitor.Exit(actionInvokers);
			}
			RequestCleanup();
		}
		public virtual void Unregister(object recipient) {
			UnregisterCore(recipient, null, null, null);
		}
		public virtual void Unregister<TMessage>(object recipient, object token, Action<TMessage> action) {
			UnregisterCore(recipient, token, action, typeof(TMessage));
		}
		protected void UnregisterCore(object recipient, object token, Delegate action, Type messageType) {
			try {
				if(isMultiThreadSafe)
					Monitor.Enter(actionInvokers);
				actionInvokers.Unregister(recipient, token, action, messageType);
			} finally {
				if(isMultiThreadSafe)
					Monitor.Exit(actionInvokers);
			}
			RequestCleanup();
		}
		public void Cleanup() {
			try {
				if(isMultiThreadSafe)
					Monitor.Enter(actionInvokers);
				actionInvokers.CleanUp();
			} finally {
				if(isMultiThreadSafe)
					Monitor.Exit(actionInvokers);
			}
			cleanupScheduled = false;
		}
		public void RequestCleanup() {
			if(cleanupScheduled) return;
			cleanupScheduled = true;
#if SILVERLIGHT               
			Deployment.Current.Dispatcher.BeginInvoke(new Action(Cleanup));
#elif NETFX_CORE
#pragma warning disable 4014
			if(!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
				Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(Cleanup));
#pragma warning restore 4014
#else
			Dispatcher.CurrentDispatcher.BeginInvoke(new Action(Cleanup), CleanUpPriority, null);
#endif
		}
		static IActionInvokerFactory CreateActionInvokerFactory(ActionReferenceType actionReferenceType) {
			return actionReferenceType == ActionReferenceType.WeakReference ?
				(IActionInvokerFactory)new WeakReferenceActionInvokerFactory() :
				(IActionInvokerFactory)new StrongReferenceActionInvokerFactory();
		}
	}
	public enum ActionReferenceType {
		WeakReference,
		StrongReference
	}
}
