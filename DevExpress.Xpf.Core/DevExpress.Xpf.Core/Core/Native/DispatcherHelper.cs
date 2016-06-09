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
using System.Windows.Threading;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Core {
	public static class DispatcherHelper {
#if DEBUGTEST
		static Dispatcher currentDispatcherReference;
		public static event EventHandler ForceUpdateLayout;
		static DispatcherHelper() {
			IncreasePriorityContextIdleMessages();
		}
		static void IncreasePriorityContextIdleMessages() {
			currentDispatcherReference = Dispatcher.CurrentDispatcher; 
			Dispatcher.CurrentDispatcher.Hooks.OperationPosted += (d, e) => {
				if(e.Operation.Priority == DispatcherPriority.ContextIdle)
					e.Operation.Priority = DispatcherPriority.Background;
			};
		}
		public static void ForceIncreasePriorityContextIdleMessages() {
		}
		static void RaiseForceUpdateLayout() {
			ForceUpdateLayout.Do(x => x(null, EventArgs.Empty));
		}
#endif
		static void UpdateLayout() {
#if DEBUGTEST
			ContextLayoutManagerHelper.UpdateLayout();
#endif
		}
		public static void UpdateLayoutAndDoEvents() {
			UpdateLayoutAndDoEvents(null);
		}
		public static void UpdateLayoutAndDoEvents(UIElement element) { UpdateLayoutAndDoEvents(element, DispatcherPriority.Background); }
		public static void UpdateLayoutAndDoEvents(UIElement element, DispatcherPriority priority) {
#if DEBUGTEST
			RaiseForceUpdateLayout();
#endif
			if (element == null)
				UpdateLayout();
			else
				element.UpdateLayout();
			DoEvents(priority);
		}
		public static void DoEvents() {
			DoEvents(DispatcherPriority.Background);
		}
		public static void DoEvents(DispatcherPriority priority) {
			DispatcherFrame frame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(
				priority,
				new DispatcherOperationCallback(ExitFrame),
				frame);
			if(!frame.Dispatcher.HasShutdownFinished)
				Dispatcher.PushFrame(frame);
		}
		static object ExitFrame(object f) {
			((DispatcherFrame)f).Continue = false;
			return null;
		}
	}
}
