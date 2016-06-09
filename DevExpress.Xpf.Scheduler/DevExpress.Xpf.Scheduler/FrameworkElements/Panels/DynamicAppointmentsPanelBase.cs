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

using System.Windows.Controls;
using System.Windows;
using System;
using DevExpress.XtraScheduler;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Utils;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface IAppointmentsPanel {
		VisualAppointmentControl CreateVisualAppointment();
		Panel Visual { get; }
	}
	public class AppointmentPanelController : PanelController {
		LoadedUnloadedSubscriber loadSubscriber;
		AppointmentControlObservableCollection appointmentControls;
		bool isOwnerPanelLoaded = false;
		public AppointmentPanelController(DynamicAppointmentsPanelBase ownerPanel)
			: base(ownerPanel) {
			this.loadSubscriber = new LoadedUnloadedSubscriber(ownerPanel, SubscribeEventsOnLoad, UnsubscribeEventsOnUnload);
		}
		public event RequestVisualAppointmentInfoEventHandler RequestVisualAppointmentInfo;
		public new DynamicAppointmentsPanelBase OwnerPanel { get { return (DynamicAppointmentsPanelBase)base.OwnerPanel; } }
		public AppointmentControlObservableCollection AppointmentControls {
			get { return appointmentControls; }
			set {
				if(appointmentControls == value)
					return;
				UnsubscribeAppointmentsEvents();
				appointmentControls = value;
				SubscribeAppointmentsEvents();
			}
		}
		protected SchedulerControl SchedulerControl { get; set; }
		private void SubscribeAppointmentsEvents() {
			if (appointmentControls != null)
				appointmentControls.CollectionChanged += OnAppointmentControlsCollectionChanged;
		}
		void UnsubscribeAppointmentsEvents() {
			if (appointmentControls != null)
				appointmentControls.CollectionChanged -= OnAppointmentControlsCollectionChanged;
		}
		protected virtual void SubscribeEventsOnLoad(FrameworkElement fe) {
			this.isOwnerPanelLoaded = true;
			UnsubscribeSchedulerControlEvent(SchedulerControl);
			SubscribeSchedulerControlEvent(SchedulerControl);
			UnsubscribeAppointmentsEvents();
			SubscribeAppointmentsEvents();
			ForceAppointmentLayout(); 
		}
		protected virtual void ForceAppointmentLayout() {
			RaiseLayoutChanged(AppointmentsPanelChangeActions.RecalculateAppointmentsLayout);
		}
		protected virtual void UnsubscribeEventsOnUnload(FrameworkElement fe) {
			UnsubscribeSchedulerControlEvent(SchedulerControl);
			UnsubscribeAppointmentsEvents();
			this.isOwnerPanelLoaded = false;
		}
		protected override void OnCellContainerChanged(VisualCellContainer oldValue, VisualCellContainer newValue) {
			base.OnCellContainerChanged(oldValue, newValue);
			SetSchedulerControl(oldValue, newValue);
		}
		void SetSchedulerControl(VisualCellContainer oldValue, VisualCellContainer newValue) {
			SchedulerControl oldControl = oldValue != null ? oldValue.View.Control : null;
			UnsubscribeSchedulerControlEvent(oldControl);
			SchedulerControl newControl = newValue != null ? newValue.View.Control : null;
			SubscribeSchedulerControlEvent(newControl);
			SchedulerControl = newControl;
		}
		void SubscribeSchedulerControlEvent(SchedulerControl scheduler) {
			if(scheduler == null)
				return;
			if (this.isOwnerPanelLoaded)
				scheduler.RequestVisualAppointmentInfo += new RequestVisualAppointmentInfoEventHandler(OnRequestVisualAppointmentInfo);
		}
		void UnsubscribeSchedulerControlEvent(SchedulerControl scheduler) {
			if(scheduler == null)
				return;
			scheduler.RequestVisualAppointmentInfo -= new RequestVisualAppointmentInfoEventHandler(OnRequestVisualAppointmentInfo);
		}
		void OnRequestVisualAppointmentInfo(object sender, RequestVisualAppointmentInfoEventArgs e) {
			RaiseRequestVisualAppointmentInfo(sender, e);
		}
		void RaiseRequestVisualAppointmentInfo(object sender, RequestVisualAppointmentInfoEventArgs e) {
			if(RequestVisualAppointmentInfo != null)
				RequestVisualAppointmentInfo(sender, e);
		}
		void OnAppointmentControlsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			ForceAppointmentLayout();
		}
		public void UpdateAppointmentRectStatistics(Rect rect) {
			if (SchedulerControl == null)
				return;
			SchedulerControl.VisualizatoinStatistics.UpdateAppointmentRect(rect);
		}
		public override void UpdateCellsVisualStatistics(CellsRectInfo newCellsRectInfo) {
			if (SchedulerControl == null)
				return;
			int count = newCellsRectInfo.GetCellCount();
			for (int i = 0; i < count; i++)
				SchedulerControl.VisualizatoinStatistics.UpdateCellRect(newCellsRectInfo.Rects[i]);
		}
	}
	public abstract class DynamicAppointmentsPanelBase : SchedulerCellsBasedPanelBase, IAppointmentsPanel {
		static protected readonly int GapBetweenAppointments = 0;
		static protected readonly int LeftAppointmentIndent = 0;
		static protected readonly int RightAppointmentIndent = 0;
		protected DynamicAppointmentsPanelBase() {
			LayoutInfo = LayoutInfo.Empty;
			AppointmentPanelController.RequestVisualAppointmentInfo += OnRequestVisualAppointmentInfo;
		}
		Panel IAppointmentsPanel.Visual { get { return this; } }
		protected LayoutInfo LayoutInfo { get; set; }
		#region CellPadding
		public static readonly DependencyProperty CellPaddingProperty = DependencyProperty.Register("CellPadding", typeof(Thickness), typeof(DynamicAppointmentsPanelBase), new UIPropertyMetadata(new Thickness(0, 0, 0, 0), new PropertyChangedCallback(OnCellPaddingChanged)));
		public Thickness CellPadding {
			get	{ return (Thickness)GetValue(CellPaddingProperty); }
			set	{ SetValue(CellPaddingProperty, value); }
		}
		static void OnCellPaddingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			DynamicAppointmentsPanelBase dynamicAppointmentsPanelBase = o as DynamicAppointmentsPanelBase;
			if (dynamicAppointmentsPanelBase != null)
				dynamicAppointmentsPanelBase.OnCellPaddingChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
		}
		protected virtual void OnCellPaddingChanged(Thickness oldValue, Thickness newValue) {
			DoInvalidateMeasure();
		}
		#endregion
		#region SnapToCells
		public AppointmentSnapToCellsMode SnapToCells {
			get { return (AppointmentSnapToCellsMode)GetValue(SnapToCellsProperty); }
			set { SetValue(SnapToCellsProperty, value); }
		}
		public static readonly DependencyProperty SnapToCellsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DynamicAppointmentsPanelBase, AppointmentSnapToCellsMode>("SnapToCells", AppointmentSnapToCellsMode.Auto, (d, e) => d.OnSnapToCellsChanged(e.OldValue, e.NewValue), null);
		void OnSnapToCellsChanged(AppointmentSnapToCellsMode oldValue, AppointmentSnapToCellsMode newValue) {
			DoInvalidateMeasure();
		}
		#endregion
		protected AppointmentPanelController AppointmentPanelController { get { return (AppointmentPanelController)PanelController; } }
		#region AppointmentControls
		public AppointmentControlObservableCollection AppointmentControls {
			get { return (AppointmentControlObservableCollection)GetValue(AppointmentControlsProperty); }
			set { SetValue(AppointmentControlsProperty, value); }
		}
		public static readonly DependencyProperty AppointmentControlsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DynamicAppointmentsPanelBase, AppointmentControlObservableCollection>("AppointmentControls", null, (d, e) => d.OnAppointmentControlsChanged(e.OldValue, e.NewValue), null);
		void OnAppointmentControlsChanged(AppointmentControlObservableCollection oldValue, AppointmentControlObservableCollection newValue) {
			AppointmentPanelController.AppointmentControls = newValue;
		}
		#endregion
		#region IsDraggedMode
		public bool IsDraggedMode {
			get { return (bool)GetValue(IsDraggedModeProperty); }
			set { SetValue(IsDraggedModeProperty, value); }
		}
		public static readonly DependencyProperty IsDraggedModeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DynamicAppointmentsPanelBase, bool>("IsDraggedMode", false, (d, e) => d.OnIsDraggedModeChanged(e.OldValue, e.NewValue), null);
		protected virtual void OnIsDraggedModeChanged(bool oldValue, bool newValue) {
		}
		#endregion
		#region AppointmentStyle
		public Style AppointmentStyle {
			get { return (Style)GetValue(AppointmentStyleProperty); }
			set { SetValue(AppointmentStyleProperty, value); }
		}
		public static readonly DependencyProperty AppointmentStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DynamicAppointmentsPanelBase, Style>("AppointmentStyle", null);
		#endregion
		public abstract VisualAppointmentControl CreateVisualAppointment();
		void OnRequestVisualAppointmentInfo(object sender, RequestVisualAppointmentInfoEventArgs e) {
			switch(e.DragState) {
				default:
				case AppointmentDragState.Begin:
					OnBeginDrag(e); break;
				case AppointmentDragState.Drag:
					OnDrag(e); break;
				case AppointmentDragState.Cancel:
					OnDragCancel(e); break;
			}
		}
		void OnBeginDrag(RequestVisualAppointmentInfoEventArgs e) {
			List<VisualAppointmentInfo> selectedAppointments = e.AppointmentInfos;
			int count = Children.Count;
			for(int i = 0; i < count; i++) {
				VisualAppointmentControl container = Children[i] as VisualAppointmentControl;
				if(container == null)
					continue;
				Appointment apt = container.GetAppointment();
				if(e.SourceAppointments.Contains(apt)) {
					Size size = container.RenderSize;
					Point topLeft = container.TranslatePoint(new Point(0, 0), this);
					Rect bounds = new Rect(topLeft, size);
					selectedAppointments.Add(new VisualAppointmentInfo(container, bounds, this, this));
				}
			}
		}
		void OnDrag(RequestVisualAppointmentInfoEventArgs e) {
			List<VisualAppointmentInfo> dragInfos = e.AppointmentInfos;
			int count = dragInfos.Count;
			for(int i = 0; i < count; i++) {
				VisualAppointmentInfo dragInfo = dragInfos[i];
				if(dragInfo.Panel != this)
					continue;
				VisualAppointmentControl appointmentControl = dragInfo.Container;
				if(appointmentControl == null)
					continue;
				SetAppointmentVisualState(appointmentControl, !e.Copy);
			}
		}
		void OnDragCancel(RequestVisualAppointmentInfoEventArgs e) {
			List<VisualAppointmentInfo> dragInfos = e.AppointmentInfos;
			int count = dragInfos.Count;
			for(int i = 0; i < count; i++) {
				VisualAppointmentInfo dragInfo = dragInfos[i];
				if(dragInfo.Panel != this)
					continue;
				VisualAppointmentControl appointmentControl = dragInfo.Container;
				if(appointmentControl == null)
					continue;
				SetAppointmentVisualState(appointmentControl, false);
			}
		}
		protected virtual void SetAppointmentVisualState(VisualAppointmentControl appointmentControl, bool dragged) {
			appointmentControl.SetDraggedVisualState(dragged);
		}
		protected override void DoInvalidateMeasure() {
			LayoutInfo = LayoutInfo.Empty;
			base.DoInvalidateMeasure();
		}
		protected internal virtual double GetAppointmentMinWidth() {
			return 12;
		}
		protected override Size DoMeasureOverride(Size availableSize) {
			if(LayoutInfo.CanUsePrevious(availableSize))
				return LayoutInfo.MeasuredSize;
			if(LayoutInfo.CanUsePreviousHeight(availableSize))
				return LayoutInfo.MeasuredSize;
			if(!EnsureCellsRectValid()) {
				LayoutInfo = LayoutInfo.Empty;
				return base.DoMeasureOverride(availableSize);
			}
			if(PanelController.IsLayoutLocked) {
				LayoutInfo = LayoutInfo.Empty;
				return base.DoMeasureOverride(availableSize);
			}
			if (availableSize.Width <= 0)
				return Size.Empty;
			IAppointmentGenerator appointmentGenerator = CreateAppointmentGenerator(AppointmentStyle);
			appointmentGenerator.Begin();
			CalculateAppointmentsLayout(appointmentGenerator, availableSize);
			appointmentGenerator.End();
			LayoutInfo = new LayoutInfo(availableSize);
			LayoutInfo.MeasuredSize = CalculateDesiredSize(availableSize, Children);
			return LayoutInfo.MeasuredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (finalSize.Width <= 0)
				return finalSize;
			if(!EnsureCellsRectValid())
				return finalSize;
			BeginArrangeAppointments();
			foreach(VisualAppointmentControl appointmentControl in Children) {
				if (appointmentControl.Visibility != Visibility.Visible)
					continue;
				if (IsAppointmentControlSizeChanged(appointmentControl)) {
					DoInvalidateMeasure();
					break;
				}				
				Rect rect = CalculateAppointmentRect(appointmentControl);
				UpdateVisualAppointmentStatistics(rect);
				appointmentControl.Arrange(rect);
			}
			EndArrangeAppointments();
			return finalSize;
		}
		protected virtual void UpdateVisualAppointmentStatistics(Rect rect) {
		}
		protected virtual bool IsAppointmentControlSizeChanged(VisualAppointmentControl appointmentControl) {
			return false;
		}
		protected virtual void BeginArrangeAppointments() { 
		}
		protected virtual void EndArrangeAppointments() { 
		}
		public Rect GetAppointmentRect(VisualAppointmentControl visualAppointmentControl) {
			return visualAppointmentControl.CachedRectangle;
		}
		protected abstract void CalculateAppointmentsLayout(IAppointmentGenerator appointmentGenerator, Size availableSize);
		protected abstract Size CalculateDesiredSize(Size availableSize, UIElementCollection children);
		protected abstract Rect CalculateAppointmentRect(VisualAppointmentControl appointmentControl);
		protected abstract IAppointmentGenerator CreateAppointmentGenerator(Style appointmentStyle);		
	}
}
