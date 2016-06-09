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
using System.Windows;
using DevExpress.Xpf.Scheduler.Native;
using System.ComponentModel;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualDayViewInterval : VisualInterval {
		#region FirstSimpleInterval
		public static readonly DependencyProperty FirstSimpleIntervalProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewInterval, VisualSimpleResourceInterval>("FirstSimpleInterval", null);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewIntervalFirstSimpleInterval")]
#endif
public VisualSimpleResourceInterval FirstSimpleInterval { get { return (VisualSimpleResourceInterval)GetValue(FirstSimpleIntervalProperty); } set { SetValue(FirstSimpleIntervalProperty, value); } }
		#endregion
		#region IsAlternate
		public static readonly DependencyProperty IsAlternateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewInterval, bool>("IsAlternate", false, (d, e) => d.OnIsAlternateChanged(e.OldValue, e.NewValue));
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewIntervalIsAlternate")]
#endif
public bool IsAlternate { get { return (bool)GetValue(IsAlternateProperty); } set { SetValue(IsAlternateProperty, value); } }
		bool lastSettedIsAlternate;
		protected virtual void OnIsAlternateChanged(bool oldValue, bool newValue) {
			lastSettedIsAlternate = newValue;
		}
		#endregion
		#region TimeIndicatorVisibility
		public static readonly DependencyProperty TimeIndicatorVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewInterval, TimeIndicatorVisibility>("TimeIndicatorVisibility", TimeIndicatorVisibility.TodayView, (o, e) => o.OnTimeIndicatorVisibilityChanged(e.OldValue, e.NewValue));
		public TimeIndicatorVisibility TimeIndicatorVisibility {
			get { return (TimeIndicatorVisibility)GetValue(TimeIndicatorVisibilityProperty); }
			set { SetValue(TimeIndicatorVisibilityProperty, value); }
		}
		void OnTimeIndicatorVisibilityChanged(TimeIndicatorVisibility oldValue, TimeIndicatorVisibility newValue) {
			DevExpress.Xpf.Scheduler.Drawing.Native.TimeIndicatorTimerService.ForceUpdate();
		}
		#endregion
		protected override VisualSimpleResourceInterval CreateVisualSimpleResourceInterval() {
			return new VisualDayViewColumn();
		}
		protected override void CopyFromCore(SingleDayViewInfo source) {
			if (SimpleIntervals == null)
				SimpleIntervals = new VisualSimpleResourceIntervalCollection();			
			CollectionCopyHelper.Copy(SimpleIntervals, source.SingleResourceViewInfoCollection, CreateVisualSimpleResourceInterval);
			FirstSimpleInterval = SimpleIntervals[0];
			UpdateIsAlternateProperty(FirstSimpleInterval.IntervalStart);
			this.View = source.View;
			if (ResourceHeaders == null)
				ResourceHeaders = new VisualResourceHeaderBaseContentCollection();
			CollectionCopyHelper.Copy(ResourceHeaders, source.SingleResourceViewInfoCollection, CreateVisualResourceHeader);
			if (NavigationButtons == null)
				NavigationButtons = new VisualNavigationButtonsCollection();
			CollectionCopyHelper.Copy(NavigationButtons, source.SingleResourceViewInfoCollection, CreateVisualNavigationButtons);
			TimeIndicatorVisibility = source.TimeIndicatorVisibility;
		}
		private void UpdateIsAlternateProperty(DateTime start) {
			if (View == null)
				return;
			bool newIsAlternate = XpfSchedulerUtils.IsTodayDate(View.Control.InnerControl.TimeZoneHelper, start);
			if (lastSettedIsAlternate != newIsAlternate)
				this.IsAlternate = newIsAlternate;
		}
		public override void CopyAppointmentsFrom(SingleDayViewInfo source) {
			if (SimpleIntervals == null)
				return;
			int count = source.SingleResourceViewInfoCollection.Count;
			for (int i = 0; i < count; i++)
				SimpleIntervals[i].CopyAppointmentsFrom(source.SingleResourceViewInfoCollection[i].CellContainers[0]);
		}
		protected virtual VisualResourceHeaderBaseContent CreateVisualResourceHeader() {
			return new VisualResourceHeaderContent();
		}
		protected virtual VisualNavigationButtons CreateVisualNavigationButtons() {
			return new VisualNavigationButtons();
		}
	}
}
