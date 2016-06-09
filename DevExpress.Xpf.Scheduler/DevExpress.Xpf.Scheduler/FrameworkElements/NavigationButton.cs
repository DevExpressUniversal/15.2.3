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
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.UI;
using System.ComponentModel;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using System.Windows.Input;
using DevExpress.XtraScheduler.Native;
using System.Windows.Controls;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Commands;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public enum NavigationDirection {
		Backward,
		Forward
	}
	public class VisualNavigationButton : SchedulerButton, ISelectableIntervalViewInfo {
		public VisualNavigationButton() {
			DefaultStyleKey = typeof(VisualNavigationButton);
		}
		#region NextButtonMargin
		public static readonly DependencyProperty NextNavigationButtonMarginProperty =
			DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedProperty<VisualNavigationButton, Thickness>("NextNavigationButtonMargin", new Thickness(0, 0, 2, 0), FrameworkPropertyMetadataOptions.None, null);
		public static Thickness GetNextNavigationButtonMargin(DependencyObject element) {
			return (Thickness)element.GetValue(NextNavigationButtonMarginProperty);
		}
		public static void SetNextNavigationButtonMargin(DependencyObject element, Thickness value) {
			element.SetValue(NextNavigationButtonMarginProperty, value);
		}
		#endregion
		#region PrevButtonMargin
		public static readonly DependencyProperty PrevNavigationButtonMarginProperty =
			DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedProperty<VisualNavigationButton, Thickness>("PrevNavigationButtonMargin", new Thickness(2, 0, 0, 0), FrameworkPropertyMetadataOptions.None, null);
		public static Thickness GetPrevNavigationButtonMargin(DependencyObject element) {
			return (Thickness)element.GetValue(PrevNavigationButtonMarginProperty);
		}
		public static void SetPrevNavigationButtonMargin(DependencyObject element, Thickness value) {
			element.SetValue(PrevNavigationButtonMarginProperty, value);
		}
		#endregion
		#region Direction
		public static readonly DependencyProperty DirectionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualNavigationButton, NavigationDirection>("Direction", NavigationDirection.Backward, (d, e) => d.OnDirectionChanged(e.OldValue, e.NewValue));
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualNavigationButtonDirection")]
#endif
		public NavigationDirection Direction { get { return (NavigationDirection)GetValue(DirectionProperty); } set { SetValue(DirectionProperty, value); } }
		protected virtual void OnDirectionChanged(NavigationDirection oldValue, NavigationDirection newValue) {
			ChangeVisualState();
		}
		#endregion
		#region Resource
		public Resource Resource { 
			get { return (XtraScheduler.Resource)GetValue(ResourceProperty); }
			set { SetValue(ResourceProperty, value); }
		}
		public static readonly DependencyProperty ResourceProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualNavigationButton, Resource>("Resource", default(Resource), (d, e) => d.OnResourceChanged());
		#endregion
		#region Interval
		public TimeInterval Interval {
			get { return (TimeInterval)GetValue(IntervalProperty); }
			set { SetValue(IntervalProperty, value); }
		}
		public static readonly DependencyProperty IntervalProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualNavigationButton, TimeInterval>("Interval", default(TimeInterval), (d, e) => d.OnIntervalChanged());
		#endregion
		#region ButtonInfo
		public NavigationButtonViewModel ButtonInfo {
			get { return (NavigationButtonViewModel)GetValue(ButtonInfoProperty); }
			set { SetValue(ButtonInfoProperty, value); }
		}
		public static readonly DependencyProperty ButtonInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualNavigationButton, NavigationButtonViewModel>("ButtonInfo", null, OnViewModelChanged);
		static void OnViewModelChanged(VisualNavigationButton navigationButton, DependencyPropertyChangedEventArgs<NavigationButtonViewModel> e) {
			if (e.OldValue != null)
				navigationButton.ClearInnerBinding();
			if (e.NewValue != null)
				navigationButton.SetInnerBindings(e.NewValue);
		}
		#endregion
		#region ISelectableIntervalViewInfo implementation
		XtraScheduler.Drawing.SchedulerHitTest XtraScheduler.Native.ISelectableIntervalViewInfo.HitTestType { get { return DevExpress.XtraScheduler.Drawing.SchedulerHitTest.NavigationButton; } }
		TimeInterval XtraScheduler.Native.ISelectableIntervalViewInfo.Interval { get { return TimeInterval.Empty; } }
		Resource XtraScheduler.Native.ISelectableIntervalViewInfo.Resource { get { return ResourceBase.Empty; } }
		bool XtraScheduler.Native.ISelectableIntervalViewInfo.Selected { get { return true; } }
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ChangeVisualState();
		}
		protected virtual void ChangeVisualState() {
			VisualStateManager.GoToState(this, GetStateName(), false);
		}
		string GetStateName() {
			return Direction == NavigationDirection.Forward ? "Forward" : "Backward";
		}
		void OnResourceChanged() {
			RecreateCommand();
		}
		void OnIntervalChanged() {
			RecreateCommand();
		}
		void RecreateCommand() {
			if (Interval != null && Resource != null)
				Command = CreateCommand();
		}
		void ClearInnerBinding() {
			InnerBindingHelper.ClearBinding(this, VisualNavigationButton.IntervalProperty);
			InnerBindingHelper.ClearBinding(this, VisualNavigationButton.ResourceProperty);
			InnerBindingHelper.ClearBinding(this, VisualNavigationButton.CommandParameterProperty);
			InnerBindingHelper.ClearBinding(this, VisualNavigationButton.VisibilityProperty);
			InnerBindingHelper.ClearBinding(this, VisualNavigationButton.IsEnabledProperty);
		}
		void SetInnerBindings(NavigationButtonViewModel newValue) {
			InnerBindingHelper.SetBinding(this, ButtonInfo, VisualNavigationButton.IntervalProperty, "Interval");
			InnerBindingHelper.SetBinding(this, ButtonInfo, VisualNavigationButton.ResourceProperty, "Resource");
			InnerBindingHelper.SetBinding(this, ButtonInfo, VisualNavigationButton.CommandParameterProperty, "Scheduler");
			InnerBindingHelper.SetBinding(this, ButtonInfo, VisualNavigationButton.VisibilityProperty, "Visibility");
			InnerBindingHelper.SetBinding(this, ButtonInfo, VisualNavigationButton.IsEnabledProperty, "IsEnabled");
		}
		protected virtual ICommand CreateCommand() {
			return new NavigationButtonUICommand(Interval, Resource);
		}
	}
	public class NavigationButtonViewModel : DependencyObject, ISupportCopyFrom<NavigationButtonInfo> {
		#region Resource
		public Resource Resource {
			get { return (XtraScheduler.Resource)GetValue(ResourceProperty); }
			set { SetValue(ResourceProperty, value); }
		}
		public static readonly DependencyProperty ResourceProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonViewModel, XtraScheduler.Resource>("Resource", default(XtraScheduler.Resource));
		#endregion
		#region Interval
		public TimeInterval Interval {
			get { return (TimeInterval)GetValue(IntervalProperty); }
			set { SetValue(IntervalProperty, value); }
		}
		public static readonly DependencyProperty IntervalProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonViewModel, TimeInterval>("Interval", default(TimeInterval));
		#endregion
		#region Scheduler
		public SchedulerControl Scheduler {
			get { return (SchedulerControl)GetValue(SchedulerProperty); }
			set { SetValue(SchedulerProperty, value); }
		}
		public static readonly DependencyProperty SchedulerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonViewModel, SchedulerControl>("Scheduler", default(SchedulerControl));
		#endregion
		#region Visibility
		public Visibility Visibility {
			get { return (Visibility)GetValue(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}
		public static readonly DependencyProperty VisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonViewModel, Visibility>("Visibility", Visibility.Visible);
		#endregion
		#region IsEnabled
		public bool IsEnabled {
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}
		public static readonly DependencyProperty IsEnabledProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonViewModel, bool>("IsEnabled", true);
		#endregion
		public void CopyFrom(NavigationButtonInfo source) {
			if (source != null)
				CopyFromCore(source);
		}
		protected virtual void CopyFromCore(NavigationButtonInfo source) {
			if (!object.ReferenceEquals((object)this.Resource, (object)source.Resource))
				this.Resource = source.Resource;
			if (!Object.ReferenceEquals(Interval, source.Interval))
				Interval = source.Interval;
			if (!Object.ReferenceEquals(Scheduler, source.Control))
				Scheduler = source.Control;
			if (Visibility != source.Visibility)
				Visibility = source.Visibility;
			if (IsEnabled != source.IsEnabled)
				IsEnabled = source.IsEnabled;
		}
	}
	public class NavigationButtonInfo {
		public NavigationButtonInfo(SchedulerControl control, TimeInterval interval, Resource resource) {
			Guard.ArgumentNotNull(resource, "resource");
			Guard.ArgumentNotNull(control, "control");
			Resource = resource;
			Interval = interval;
			Control = control;
			Visibility = Visibility.Collapsed;
			IsEnabled = false;
		}
		public NavigationButtonInfo(SchedulerControl control)
			: this(control, TimeInterval.Empty, ResourceBase.Empty) {
		}
		public TimeInterval Interval { get; set; }
		public Resource Resource { get; set; }
		public SchedulerControl Control { get; set; }
		public Visibility Visibility { get; set; }
		public bool IsEnabled { get; set; }
	}
}
