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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
namespace DevExpress.DemoData.Helpers {
	public static class BackgroundHelper {
		static Dispatcher dispatcher;
		static SynchronizationContext context;
		static TaskScheduler scheduler;
		public static void DoInBackground(Action backgroundAction, Action mainThreadAction) {
			DoInBackground(backgroundAction, mainThreadAction, 30);
		}
		public static void DoInBackground(Action backgroundAction, Action mainThreadAction, int milliseconds) {
			Thread thread = new Thread(delegate() {
				Thread.Sleep(milliseconds);
				if(backgroundAction != null)
					backgroundAction();
				if(mainThreadAction != null)
					Dispatcher.BeginInvoke(mainThreadAction);
			});
			thread.Start();
		}
		public static void DoInMainThread(Action action) {
			if(Dispatcher.CheckAccess()) {
				action();
			} else {
				AutoResetEvent done = new AutoResetEvent(false);
				Dispatcher.BeginInvoke((Action)delegate() {
					action();
					done.Set();
				});
				done.WaitOne();
			}
		}
		public static Dispatcher Dispatcher {
			get {
				if(dispatcher == null)
					dispatcher = DefaultDispatcher;
				return dispatcher;
			}
			set { dispatcher = value; }
		}
		static Dispatcher DefaultDispatcher {
			get {
				return Application.Current.Dispatcher;
			}
		}
		public static SynchronizationContext UIContext {
			get {
				if(context == null) {
					throw new Exception("UIContext hasn't been set yet");
				}
				return context;
			}
			set { context = value; }
		}
		public static TaskScheduler UIScheduler {
			get {
				if(scheduler == null) {
					throw new Exception("UIScheduler hasn't been set yet");
				}
				return scheduler;
			}
			set { scheduler = value; }
		}
	}
}
