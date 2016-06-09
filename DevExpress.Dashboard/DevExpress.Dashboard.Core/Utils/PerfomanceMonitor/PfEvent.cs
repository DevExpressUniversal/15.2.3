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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
namespace DevExpress.DashboardCommon.Native.PerfomanceMonitor {
	public class PfEvent : IPfEvent {
		PfEvent parent;
		Stopwatch timer;
		List<PfEvent> children;
		int gc0_prev = -1;
		int gc1_prev = -1;
		int gc2_prev = -1;
		long gcMemory_prev = -1;
		int gc0_curr = -1;
		int gc1_curr = -1;
		int gc2_curr = -1;
		long gcMemory_curr = -1;
		public event EventHandler OnStop;
		public PfEvent(PfEvent parent, string name, int stepNum) {
			this.children = new List<PfEvent>();
			this.ThreadId = Thread.CurrentThread.ManagedThreadId;
			this.parent = parent;
			if(parent != null)
				parent.children.Add(this);
			this.Id = Guid.NewGuid();
			this.StepNumber = stepNum;
			this.Name = name;
			Start();
		}
		public void Dispose() {
			End();
			if(OnStop != null)
				OnStop(this, EventArgs.Empty);
		}
		void Start() {
			gc0_prev = GC.CollectionCount(0);
			gc1_prev = GC.CollectionCount(1);
			gc2_prev = GC.CollectionCount(2);
			gcMemory_prev = GC.GetTotalMemory(false);
			timer = Stopwatch.StartNew();
			StartTime = DateTime.Now;
		}
		void End() {
			timer.Stop();
			gc0_curr = GC.CollectionCount(0);
			gc1_curr = GC.CollectionCount(1);
			gc2_curr = GC.CollectionCount(2);
			gcMemory_curr = GC.GetTotalMemory(false);
		}
		#region IPfEvent implementation
		public int ThreadId { get; set; }
		public Guid Id { get; private set; }
		public Guid? ParentId { get { return parent == null ? (Guid?)null : parent.Id; } }
		public int StepNumber { get; private set; }
		public int Level { get { return parent == null ? 0 : parent.Level + 1; } }
		public string Name { get; private set; }
		public string FullName { get { return parent == null ? Name : parent.Name + " | " + Name; } }
		public DateTime StartTime { get; private set; }
		public long Duration { get { return timer.ElapsedMilliseconds; } }
		public int GC0Count { get { return gc0_curr - gc0_prev; } }
		public int GC1Count { get { return gc1_curr - gc1_prev; } }
		public int GC2Count { get { return gc2_curr - gc2_prev; } }
		public long GCMemory { get { return gcMemory_curr - gcMemory_prev; } }
		public int GC0CountTotal { get { return gc0_curr; } }
		public int GC1CountTotal { get { return gc1_curr; } }
		public int GC2CountTotal { get { return gc2_curr; } }
		public long GCMemoryTotal { get { return gcMemory_curr; } }
		public void WalkTree(Action<IPfEvent> action) {
			foreach(var evnt in children)
				evnt.WalkTree(action);
			action(this);
		}
		#endregion
	}
}
