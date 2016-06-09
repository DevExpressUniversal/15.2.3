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
using System.Windows;
using System.Windows.Threading;
#if SLDESIGN
namespace DevExpress.Xpf.Core.Design.CoreUtils.Commands {
#else
namespace DevExpress.Xpf.Core.Commands {
#endif
	[Obsolete]
   internal static class WeakEventHandlerManager {
		public static void CallWeakReferenceHandlers(object sender, List<WeakReference> handlers) {
			if(handlers != null) {
				EventHandler[] callees = new EventHandler[handlers.Count];
				int count = 0;
				count = CleanupOldHandlers(handlers, callees, count);
				for(int i = 0; i < count; i++) {
					CallHandler(sender, callees[i]);
				}
			}
		}
		private static void CallHandler(object sender, EventHandler eventHandler) {
			DispatcherProxy dispatcher = CreateDispatcherProxy(sender);
			if(eventHandler != null) {
				if(dispatcher != null && !dispatcher.CheckAccess()) {
					dispatcher.BeginInvoke((Action<object, EventHandler>)CallHandler, sender, eventHandler);
				} else {
					eventHandler(sender, EventArgs.Empty);
				}
			}
		}
		private static DispatcherProxy CreateDispatcherProxy(object obj) {
#if !SL || SLDESIGN
			IDispatcherInfo dispatcherInfo = obj as IDispatcherInfo;
			if(dispatcherInfo != null) {
				return DispatcherProxy.CreateDispatcher(dispatcherInfo.Dispatcher);
			}
#endif
			return DispatcherProxy.CreateDispatcher();
		}
		private class DispatcherProxy {
			Dispatcher innerDispatcher;
			private DispatcherProxy(Dispatcher dispatcher) {
				innerDispatcher = dispatcher;
			}
			public static DispatcherProxy CreateDispatcher() {
#if SILVERLIGHT && !SLDESIGN
				return CreateDispatcher(Deployment.Current == null ? null : Deployment.Current.Dispatcher);
#else
				return CreateDispatcher(Dispatcher.FromThread(System.Threading.Thread.CurrentThread));
#endif
			}
			public static DispatcherProxy CreateDispatcher(Dispatcher innerDispatcher) {
				if(innerDispatcher == null)
					return null;
				return new DispatcherProxy(innerDispatcher);
			}
			public bool CheckAccess() {
				return innerDispatcher.CheckAccess();
			}
			public DispatcherOperation BeginInvoke(Delegate method, params Object[] args) {
#if SILVERLIGHT && !SLDESIGN
				return innerDispatcher.BeginInvoke(method, args);
#else
				return innerDispatcher.BeginInvoke(method, DispatcherPriority.Normal, args);
#endif
			}
		}
		private static int CleanupOldHandlers(List<WeakReference> handlers, EventHandler[] callees, int count) {
			for(int i = handlers.Count - 1; i >= 0; i--) {
				WeakReference reference = handlers[i];
				EventHandler handler = reference.Target as EventHandler;
				if(handler == null) {
					handlers.RemoveAt(i);
				} else {
					callees[count] = handler;
					count++;
				}
			}
			return count;
		}
		public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize) {
			if(handlers == null) {
				handlers = (defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>());
			}
			handlers.Add(new WeakReference(handler));
		}
		public static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler) {
			if(handlers != null) {
				for(int i = handlers.Count - 1; i >= 0; i--) {
					WeakReference reference = handlers[i];
					EventHandler existingHandler = reference.Target as EventHandler;
					if((existingHandler == null) || (existingHandler == handler)) {
						handlers.RemoveAt(i);
					}
				}
			}
		}
	}
}
