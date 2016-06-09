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

using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Scheduler.Drawing.Components {
	public class VerticalTimeIndicatorContainerComponent : VisualComponent<VerticalTimeIndicatorContainer> {
		Timer timer;
		public VerticalTimeIndicatorContainerComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		protected VerticalTimeIndicatorContainer TimeIndicatorContainer { get; set; }
		public override int ZIndex { get { return 500; } }
		public ResourcesComponent Resources { get; set; }
		#region TimeIndicatorVisibility
		public TimeIndicatorVisibility TimeIndicatorVisibility {
			get { return (TimeIndicatorVisibility)GetValue(TimeIndicatorVisibilityProperty); }
			set { SetValue(TimeIndicatorVisibilityProperty, value); }
		}
		public static readonly DependencyProperty TimeIndicatorVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VerticalTimeIndicatorContainerComponent, TimeIndicatorVisibility>("TimeIndicatorVisibility", TimeIndicatorVisibility.TodayView, (d, e) => d.OnTimeIndicatorVisibilityChanged(e.OldValue, e.NewValue));
		void OnTimeIndicatorVisibilityChanged(TimeIndicatorVisibility oldValue, TimeIndicatorVisibility newValue) {
			if (newValue == TimeIndicatorVisibility.Never)
				StopAndDisposeTimer();
			else
				CreateAndStartTimer();
			CalculateTimeIndicatorContainerVisibility();
		}
		#endregion
		public override void Initialize() {
			base.Initialize();
			TimeIndicatorContainer = CreateVerticalTimeIndicatorContainer();
			VisualItemsAccessor.Add(TimeIndicatorContainer);
			TimeIndicatorContainer.QueryPositionByTime += OnTimeIndicatorContainerQueryPositionByTime;
			TimeIndicatorContainer.Loaded += OnTimeIndicatorContainerLoaded;
			TimeIndicatorContainer.Unloaded += OnTimeIndicatorContainerUnloaded;
		}
		protected override IVisualElementAccessor<VerticalTimeIndicatorContainer> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<VerticalTimeIndicatorContainer>(panel, ZIndex);
		}
		VerticalTimeIndicatorContainer CreateVerticalTimeIndicatorContainer() {
			return new VerticalTimeIndicatorContainer();
		}
		protected override System.Windows.Size MeasureCore(System.Windows.Size availableSize) {
			TimeIndicatorContainer.Measure(availableSize);
			return TimeIndicatorContainer.DesiredSize;
		}
		protected override System.Windows.Size ArrangeCore(System.Windows.Rect arrangeBounds) {
			CalculateTimeIndicatorContainerVisibility();
			TimeIndicatorContainer.Arrange(arrangeBounds);
			return arrangeBounds.Size;
		}
		protected virtual void OnTimeIndicatorContainerQueryPositionByTime(object sender, QueryPositionByTimeEventArgs e) {
			e.Position = Resources.ConvertTimeToPosition(e.DateTime);
		}
		protected virtual void OnTimeIndicatorContainerUnloaded(object sender, RoutedEventArgs e) {
			StopAndDisposeTimer();
		}
		protected virtual void OnTimeIndicatorContainerLoaded(object sender, RoutedEventArgs e) {
			if (TimeIndicatorVisibility != TimeIndicatorVisibility.Never)
				CreateAndStartTimer();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (this.timer == null)
				return;
			this.timer.Dispose();
			this.timer.Tick -= OnTimerTick;
			this.timer = null;
		}
		void CreateAndStartTimer() {
			if (this.timer != null)
				StopAndDisposeTimer();
			this.timer = new Timer();
			this.timer.Interval = new TimeSpan(0, 0, 30);
			this.timer.Tick += OnTimerTick;
			this.timer.Start();
		}
		void StopAndDisposeTimer() {
			if (this.timer == null)
				return;
			this.timer.Tick -= OnTimerTick;
			this.timer.Stop();
			this.timer.Dispose();
			this.timer = null;
		}
		void OnTimerTick(object sender, EventArgs e) {
			TimeIndicatorContainer.InvalidateArrange();
		}
		void CalculateTimeIndicatorContainerVisibility() {
			if (TimeIndicatorVisibility == TimeIndicatorVisibility.Never)
				TimeIndicatorContainer.Visibility = System.Windows.Visibility.Collapsed;
			else
				TimeIndicatorContainer.Visibility = System.Windows.Visibility.Visible;
			TimeIndicatorContainer.InvalidateArrange();
		}
	}
}
