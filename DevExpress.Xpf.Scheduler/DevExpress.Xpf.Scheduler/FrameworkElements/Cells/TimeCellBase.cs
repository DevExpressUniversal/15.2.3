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

using System.Windows;
using DevExpress.XtraScheduler.Drawing;
using System;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using System.ComponentModel;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public abstract class VisualTimeCellBase : VisualResourceCellBase, ISupportCopyFrom<ITimeCell>, ISupportHitTest {
		static VisualTimeCellBase() {
		}
		protected VisualTimeCellBase() {
			DefaultStyleKey = typeof(VisualTimeCellBase);
		}
		protected virtual SchedulerHitTest HitTest { get { return SchedulerHitTest.None; } }
		#region ISupportCopyFrom<TimeCell> Members
		void ISupportCopyFrom<ITimeCell>.CopyFrom(ITimeCell source) {
			CopyFromCore((TimeCellBase)source);
		}
		#endregion
		#region ISupportHitTest Members
		ISelectableIntervalViewInfo ISupportHitTest.GetSelectableIntervalViewInfo(SchedulerControl control) {
			return GetSelectableIntervalViewInfoCore(control);
		}
		protected virtual ISelectableIntervalViewInfo GetSelectableIntervalViewInfoCore(SchedulerControl control) {
			if (HitTest == SchedulerHitTest.None)
				return null;
			VisualTimeCellBaseContent content = Content as VisualTimeCellBaseContent;
			if (content != null) {
				Resource resource = control.Storage.ResourceStorage.GetResourceById(content.ResourceId);
				TimeInterval contentInterval = new TimeInterval(content.IntervalStart, content.IntervalEnd);
				bool isSelected = control.SelectedInterval.Contains(contentInterval);
				return new VisualSelectableIntervalViewInfo(SchedulerHitTest.Cell, contentInterval, resource, isSelected);
			}
			else
				return null;
		}
		#endregion
		public virtual FrameworkElement GetCellContent() {
			return GetTemplateChild("PART_CONTENT") as FrameworkElement;
		}
	}
	public class VisualTimeCellBaseContent : VisualResourceCellBaseContent, ISupportCopyFrom<ITimeCell> {
		#region IntervalStart
		public static readonly DependencyProperty IntervalStartProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeCellBaseContent, DateTime>("IntervalStart", DateTime.MinValue, (d,e) => d.OnIntervalStartChanged(e.OldValue,e.NewValue));
		public DateTime IntervalStart { get { return (DateTime)GetValue(IntervalStartProperty); } set { SetValue(IntervalStartProperty, value); } }
		#endregion
		#region IntervalEnd
		public static readonly DependencyProperty IntervalEndProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeCellBaseContent, DateTime>("IntervalEnd", DateTime.MinValue, (d, e) => d.OnIntervalEndChanged(e.OldValue, e.NewValue));
		public DateTime IntervalEnd { get { return (DateTime)GetValue(IntervalEndProperty); } set { SetValue(IntervalEndProperty, value); } }
		#endregion
		DateTime lastSettedIntervalStart = DateTime.MinValue;
		DateTime lastSettedIntervalEnd = DateTime.MinValue;
		protected virtual void OnIntervalStartChanged(DateTime oldValue, DateTime newValue) {
			lastSettedIntervalStart = newValue;
		}
		protected virtual void OnIntervalEndChanged(DateTime oldValue, DateTime newValue) {
			lastSettedIntervalEnd = newValue;
		}
		public TimeInterval GetInterval() {
			return new TimeInterval(IntervalStart, IntervalEnd);
		}
		protected internal override bool CopyFromCore(ResourceCellBase source) {
			bool wasChanged = false;
			TimeCellBase timeCellBaseSource = (TimeCellBase)source;
			if (lastSettedIntervalStart != timeCellBaseSource.Interval.Start) {
				IntervalStart = timeCellBaseSource.Interval.Start;
				wasChanged = true;
			}
			if (lastSettedIntervalEnd != timeCellBaseSource.Interval.End) {
				IntervalEnd = timeCellBaseSource.Interval.End;
				wasChanged = true;
			}
			return wasChanged | base.CopyFromCore(source);
		}
		#region ISupportCopyFrom<ITimeCell> Members
		void ISupportCopyFrom<ITimeCell>.CopyFrom(ITimeCell source) {
			CopyFrom((TimeCellBase)source);
		}
		#endregion
	}
}
