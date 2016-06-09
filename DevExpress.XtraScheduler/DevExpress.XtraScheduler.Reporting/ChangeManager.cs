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
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Reporting.Native {
	#region ReportControlChangeType
	public enum ReportControlChangeType {
		None,
		SchedulerAdapterChanged,
		VisibleResourceCountChanged,
		VisibleIntervalCountChanged,
		ViewStartChanged,
		AppearanceChanged,
		TimelineScalesChanged,
		AllowFakeDataChanged
	}
	#endregion
	#region ReportChangeActionsCalculator
	public static class ReportChangeActionsCalculator {
		internal static Hashtable changeActionsTable = CreateChangeActionsTable();
		internal static Hashtable CreateChangeActionsTable() {
			Hashtable ht = new Hashtable();
			ht[ReportControlChangeType.None] = ReportControlChangeActions.None;
			ht[ReportControlChangeType.VisibleResourceCountChanged] = ReportControlChangeActions.InitializePrintController;
			ht[ReportControlChangeType.VisibleIntervalCountChanged] = ReportControlChangeActions.InitializePrintController | ReportControlChangeActions.UpdateVisibleIntervals;
			ht[ReportControlChangeType.AllowFakeDataChanged] = ReportControlChangeActions.UpdateVisibleIntervals;
			ht[ReportControlChangeType.ViewStartChanged] = ReportControlChangeActions.InitializePrintController | ReportControlChangeActions.UpdateVisibleIntervals;
			ht[ReportControlChangeType.AppearanceChanged] = ReportControlChangeActions.NotifyDependents | ReportControlChangeActions.AppearanceChanges;
			ht[ReportControlChangeType.TimelineScalesChanged] = ReportControlChangeActions.UpdateTimeScales;
			ht[ReportControlChangeType.SchedulerAdapterChanged] = 
				ReportControlChangeActions.InitializePrintController | ReportControlChangeActions.UpdateVisibleIntervals;
			return ht;
		}
		public static ReportControlChangeActions CalculateChangeActions(ReportControlChangeType change) {
			return (ReportControlChangeActions)changeActionsTable[change];
		}
	}
	#endregion
	#region ReportControlChangeActions
	[Flags]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217")]
	public enum ReportControlChangeActions {
		None = 0x00000000,
		UpdateVisibleIntervals = 0x00000001,
		RaiseVisibleIntervalChanged = 0x000100002, 
		InitializePrintController = 0x000100004,
		AppearanceChanges = 0x000100008,
		UpdateTimeScales = 0x000100016,
		NotifyDependents = 0x000100032,
	}
	#endregion
	#region IReportControlChangeTarget
	public interface IReportControlChangeTarget {
		ReportControlChangeActions UpdateVisibleIntervals();
		void NotifyDependents(ReportControlChangeActions actions);
	}
	#endregion
	#region ReportControlChangeManager
	public class ReportControlChangeManager {
		#region Fields
		ReportControlChangeActions actions;
		IReportControlChangeTarget target;
		#endregion
		public ReportControlChangeManager(ReportControlChangeActions actions) {
			this.actions = actions;
		}
		#region Properties
		internal ReportControlChangeActions Actions { get { return actions; } set { actions = value; } }
		#endregion
		internal void SetTarget(IReportControlChangeTarget target) {
			this.target = target;
		}
		public bool IsActionQueued(ReportControlChangeActions action) {
			return (actions & action) != 0;
		}
		public virtual void ApplyChanges(IReportControlChangeTarget target) {
			if (target == null)
				Exceptions.ThrowArgumentException("target", target);
			if (actions == ReportControlChangeActions.None)
				return;
			this.target = target;
			try {
				ApplyChangesCore();
			}
			finally {
				this.target = null;
			}
		}
		protected internal virtual void ApplyChangesCore() {
			if (IsActionQueued(ReportControlChangeActions.UpdateVisibleIntervals))
				UpdateVisibleIntervals();
			if (IsActionQueued(ReportControlChangeActions.UpdateTimeScales))
				UpdateTimelineControls();
			if (IsActionQueued(ReportControlChangeActions.NotifyDependents))
				NotifyDependents();
		}
		private void UpdateTimelineControls() {
		}
		protected internal virtual void UpdateVisibleIntervals() {
			actions |= target.UpdateVisibleIntervals();
		}
		protected internal virtual void NotifyDependents() {
			target.NotifyDependents(actions);
		}
	}
	#endregion
}
