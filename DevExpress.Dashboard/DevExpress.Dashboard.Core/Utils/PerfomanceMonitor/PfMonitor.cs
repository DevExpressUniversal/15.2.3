#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Threading;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.Native.PerfomanceMonitor {
	public class PfMonitor {
		#region static members
		static object syncRoot = new object();
		static Dictionary<int, PfMonitor> monitors = new Dictionary<int, PfMonitor>();
		public static PfMonitor Current {
			get {
				lock(syncRoot) {
					PfMonitor monitor;
					int id = Thread.CurrentThread.ManagedThreadId;
					if(!monitors.TryGetValue(id, out monitor)) {
						monitor = new PfMonitor(id);
						monitors.Add(id, monitor);
					}
					return monitor;
				}
			}
		}
		#endregion
		int stepNum;
		Stack<PfEvent> runEvents = new Stack<PfEvent>();
		PfEventFake fakeEvnt = new PfEventFake();
		PfMonitor(int id) { }
		public IPfEvent Event(string eventName) {
			if(!PfMonitorOptions.IsPfInfoCollected)
				return fakeEvnt;
			PfEvent parent = runEvents.Count == 0 ? null : runEvents.Peek();
			PfEvent newEvent = new PfEvent(parent, eventName, stepNum);
			newEvent.OnStop += EventStop;
			runEvents.Push(newEvent);
			stepNum++;
			return newEvent;
		}
		void EventStop(object sender, EventArgs args) {
			PfEvent currEvent = runEvents.Pop();
			DXContract.Requires(currEvent == sender);
			if(runEvents.Count == 0 && PfMonitorOptions.DefaultWriter != null)
				currEvent.WalkTree(e => PfMonitorOptions.DefaultWriter.WriteEvent(e));
		}
		public static void SaveData() {
			if(PfMonitorOptions.DefaultWriter != null)
				PfMonitorOptions.DefaultWriter.SaveData();
		}
	}
}
