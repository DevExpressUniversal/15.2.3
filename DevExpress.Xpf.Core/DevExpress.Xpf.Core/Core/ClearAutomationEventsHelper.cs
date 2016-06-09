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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Interop;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Internal;
using Expression = System.Linq.Expressions.Expression;
namespace DevExpress.Xpf.Core {
#if !SL
	public static class ClearAutomationEventsHelper {
		[ThreadStatic]
		static ReflectionHelper helper;
		static ReflectionHelper Helper {
			get { return helper ?? (helper = CreateReflectionHelper()); }
		}
		public static bool IsEnabled { get; set; }
		static ClearAutomationEventsHelper() {
			IsEnabled = true;
		}
		static Type clmType;
		static Type ClmType { get { return clmType ?? (clmType = typeof(UIElement).Assembly.GetType("System.Windows.ContextLayoutManager")); } }
		static ReflectionHelper CreateReflectionHelper() {
			return new ReflectionHelper();
		}
		[ThreadStatic]
		static Action clearAutoEventAction;
		static bool HasClearAutoEventAction {get { return clearAutoEventAction != null; }}
		public static void ClearAutomationEvents() {
			if (!IsEnabled || BrowserInteropHelper.IsBrowserHosted || HasClearAutoEventAction)
				return;
			if (Helper.HasContent) {
				ClearAutomationEventsImpl();
				return;
			}
			ResetUpdatePeerCallback();
			ClearAutomationEventsImpl();
		}
		static object FakeUpdatePeerCallback(object args) {
			return null;
		}
		static void ResetUpdatePeerCallback() {
			var updatePeerInfo = typeof(AutomationPeer).GetField("_updatePeer", BindingFlags.Static | BindingFlags.NonPublic);
			updatePeerInfo.SetValue(null, new DispatcherOperationCallback(FakeUpdatePeerCallback));
		}
		static void ClearAutomationEventsImpl() {
			clearAutoEventAction = null;
			object clm = Helper.GetStaticMethodHandler<Func<object, object>>(ClmType, "From", BindingFlags.Static | BindingFlags.NonPublic)(Dispatcher.CurrentDispatcher);
			object autoEventsList = Helper.GetPropertyValue<object>(clm, "AutomationEvents", BindingFlags.Instance | BindingFlags.NonPublic);
			if (Helper.GetPropertyValue<int>(autoEventsList, "Count", BindingFlags.NonPublic | BindingFlags.Instance) == 0)
				return;
			object[] autoEvents = Helper.GetInstanceMethodHandler<Func<object, object[]>>(autoEventsList, "CopyToArray", BindingFlags.Instance | BindingFlags.NonPublic, autoEventsList.GetType())(autoEventsList);
			if (autoEvents == null)
				return;
			foreach (object autoEvent in autoEvents)
				Helper.GetInstanceMethodHandler<Action<object, object>>(autoEventsList, "Remove", BindingFlags.Instance | BindingFlags.NonPublic, autoEventsList.GetType())(autoEventsList, autoEvent);
		}
	}
#endif
}
