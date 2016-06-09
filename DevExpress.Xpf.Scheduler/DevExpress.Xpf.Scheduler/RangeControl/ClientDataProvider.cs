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
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.Native {
	public class SchedulerControlClientDataProvider : SchedulerInnerControlClientDataProvider {
		readonly SchedulerControl control;
		bool isDeffered;
		TimeInterval deferredInterval;
		public SchedulerControlClientDataProvider(SchedulerControl control)
			: base(control.InnerControl) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public SchedulerControl SchedulerControl { get { return control; } }
		protected override bool AllowChangeActiveView { get { return SchedulerControl.OptionsRangeControl.AllowChangeActiveView; } }
		protected override bool AutoAdjustMode { get { return SchedulerControl.OptionsRangeControl.AutoAdjustMode; } }
		protected override IScaleBasedRangeControlClientOptions GetOptionsCore() {
		   return SchedulerControl.OptionsRangeControl;
		}
		protected override IDataItemThumbnail CreateDataItemThumbnailItem(Appointment apt) {
			return new DataItemThumbnail(apt);
		}
		protected override ISchedulerControlRangeHelper CreateRangeControlHelper() {
			return new XpfSchedulerControlRangeHelper(SyncSupport.TotalRange);
		}
		protected override TimeInterval GetSchedulerVisibleInterval() {
			if (!this.isDeffered)
				return GetSchedulerVisibleIntervalCore();
			return this.deferredInterval;
		}
		protected TimeInterval GetSchedulerVisibleIntervalCore() {
			return InnerControl.ActiveView.InnerVisibleIntervals.Interval;
		}
		public void BeginDeferredChanges() {
			this.isDeffered = true;
			this.deferredInterval = InnerControl.ActiveView.InnerVisibleIntervals.Interval;
		}
		public void EndDeferredChanges() {
			if (!this.isDeffered) 
				return;
			this.isDeffered = false;
			OnSelectedRangeChangedCore(this.deferredInterval.Start, this.deferredInterval.End);
		}
		protected override void OnSelectedRangeChangedCore(DateTime rangeMinimum, DateTime rangeMaximum) {
			if (this.isDeffered) {
				this.deferredInterval = new TimeInterval(rangeMinimum, rangeMaximum);
				return;
			}
			base.OnSelectedRangeChangedCore(rangeMinimum, rangeMaximum);
		}
		public void AdjustScales() {
			TryAdjustRangeControlScales();
		}
	}
	public class XpfSchedulerControlRangeHelper : SchedulerControlRangeHelper {
		TimeInterval rangeInterval;
		public XpfSchedulerControlRangeHelper(TimeInterval interval) {
			this.rangeInterval = interval;
		}
		public override TimeInterval CalculateAdjustedRange(InnerSchedulerControl innerControl) {
			if (innerControl == null)
				return TimeInterval.Empty;
			TimeInterval visibleInterval = innerControl.ActiveView.GetVisibleIntervals().Interval;
			DateTime newStart = this.rangeInterval.Start;
			DateTime newEnd = this.rangeInterval.End;
			if (newStart > visibleInterval.Start)
				newStart = visibleInterval.Start;
			if (newEnd < visibleInterval.End)
				newEnd = visibleInterval.End;
			if (innerControl.ActiveView.LimitInterval.Start > newStart)
				newStart = innerControl.ActiveView.LimitInterval.Start;
			if (innerControl.ActiveView.LimitInterval.End < newEnd)
				newStart = innerControl.ActiveView.LimitInterval.End;
			return new TimeInterval(newStart, newEnd);
		}
		public override TimeScaleCollection CalculateAdjustedTimeScales(InnerSchedulerControl innerControl) {
			TimeScaleCollection result = new TimeScaleCollection();
			if (innerControl == null)
				return result;
			SchedulerViewType activeViewType = innerControl.ActiveViewType;
			switch (activeViewType) {
				case SchedulerViewType.Month:
					result.Add(new TimeScaleWeek());
					result.Add(new TimeScaleMonth());
					break;
				case SchedulerViewType.Timeline:
				case SchedulerViewType.Gantt:
					result.AddRange(innerControl.TimelineView.ActualScales);
					break;
				default:
					result.LoadDefaults();
					break;
			}
			UpdateScalesFirstDayOfWeek(result, innerControl.FirstDayOfWeek);
			return result;
		}
	}
}
