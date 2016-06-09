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
using System.Threading;
using System.Windows.Controls;
namespace DevExpress.Office.Internal {
	public enum InvokerType {
		MouseMove,
		Refresh,
		Any
	}
	public static class XpfBackgroundWorker {
		static Thread Thread { get; set; }
		static List<Action> Invoker { get; set; }
		static List<InvokerType> InvokerTypes { get; set; }
		static Button control;
		static ManualResetEvent reseter, uiThreadWaiter;
		public static void CheckIsWorkerThread() {
			if (Thread.CurrentThread != Thread)
				throw new AccessViolationException("Invalid thread access!");
		}
		public static void Invoke(Action m, InvokerType type) {
			lock (Invoker) {
				switch (type) {
					case InvokerType.MouseMove:
						while (InvokerTypes.Count > 0 && InvokerTypes[InvokerTypes.Count - 1] == type) {
							InvokerTypes.RemoveAt(InvokerTypes.Count - 1);
							Invoker.RemoveAt(Invoker.Count - 1);
						}
						break;
					case InvokerType.Refresh:
						while (InvokerTypes.Count > 0 && InvokerTypes[InvokerTypes.Count - 1] == type) {
							InvokerTypes.RemoveAt(InvokerTypes.Count - 1);
							Invoker.RemoveAt(Invoker.Count - 1);
						}
						break;
				}
				Invoker.Add(m);
				InvokerTypes.Add(type);
				reseter.Set();
			}
		}
		public static void Invoke(Action m) {
			lock (Invoker) {
				Invoker.Add(m);
				InvokerTypes.Add(InvokerType.Any);
				reseter.Set();
			}
		}
		public static void InvokeInUIThread(Action m, InvokerType type) {
			Invoke(delegate {
				uiThreadWaiter.Reset();
				Action action = delegate {
					m();
					uiThreadWaiter.Set();
				};
				control.Dispatcher.BeginInvoke(action, new object[0]);
				uiThreadWaiter.WaitOne();
			}, type);
		}
		static XpfBackgroundWorker() {
			control = new Button();
			Invoker = new List<Action>();
			InvokerTypes = new List<InvokerType>();
			reseter = new ManualResetEvent(false);
			uiThreadWaiter = new ManualResetEvent(false);
			Thread = new Thread(Worker);
			Thread.IsBackground = true;
			Thread.Start();
		}
		static void Worker() {
			for (; ; ) {
				if (Invoker.Count > 0) {
					Action invoke;
					lock (Invoker) {
						invoke = Invoker[0];
						Invoker.RemoveAt(0);
						InvokerTypes.RemoveAt(0);
						if (Invoker.Count == 0) reseter.Reset();
					}
					invoke();
				} else {
					reseter.WaitOne();
				}
			}
		}
	}
}
