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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Core;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Scheduler.Internal;
using System.Diagnostics;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using System.Collections.Specialized;
#else
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils;
#endif
using DevExpress.Utils;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Scheduler.Drawing.Components;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using System.ComponentModel;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region AppointmentsLayoutInfo
	public class AppointmentsLayoutInfo {
		const bool DefaultValue = true;
		Dictionary<int, bool> appointmentsFitStatusTable = new Dictionary<int, bool>();
		public double ViewPortSize { get; set; }
		public bool GetFitIntoCell(int cellIndex) {
			bool isFit;
			if (this.appointmentsFitStatusTable.TryGetValue(cellIndex, out isFit))
				return isFit;
			return DefaultValue;
		}
		public void Clear() {
			this.appointmentsFitStatusTable.Clear();
		}
		public void SetFitIntoCell(int cellIndex, bool isFit) {
			this.appointmentsFitStatusTable[cellIndex] = isFit;
		}
	}
	#endregion
	#region AppointmentsComponentBase (abstract class)
	public abstract class AppointmentsComponentBase : ItemsBasedComponent<VisualAppointmentControl, AppointmentControl>, IAppointmentsPanel, ICellsComponentChangedListener {
		protected AppointmentsComponentBase(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		#region Properties
		public CellsVisualComponent CellsComponent { get; set; }
		public Thickness CellPadding { get; set; } 
		#region Scheduler
		public SchedulerControl Scheduler {
			get { return (SchedulerControl)GetValue(SchedulerProperty); }
			set { SetValue(SchedulerProperty, value); }
		}
		public static readonly DependencyProperty SchedulerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentsComponentBase, SchedulerControl>("Scheduler", null, (d, e) => d.OnSchedulerChanged(e.OldValue, e.NewValue), null);
		void OnSchedulerChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			if (oldValue != null)
				UnsubscribeFromSchedulerEvents(oldValue);
			if (newValue != null)
				SubscribeToSchedulerEvents(newValue);
		}
		#endregion
		#endregion
		public override void Initialize() {
			base.Initialize();
			Panel.Unloaded += OnUnloaded;
			Panel.Loaded += OnLoaded;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			if (Scheduler != null) {
				UnsubscribeFromSchedulerEvents(Scheduler);
				SubscribeToSchedulerEvents(Scheduler);
			}
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			if (Scheduler != null)
				UnsubscribeFromSchedulerEvents(Scheduler);
		}
		void SubscribeToSchedulerEvents(SchedulerControl scheduler) {
			scheduler.RequestVisualAppointmentInfo += OnRequestVisualAppointmentInfo;
		}
		void UnsubscribeFromSchedulerEvents(SchedulerControl scheduler) {
			scheduler.RequestVisualAppointmentInfo -= OnRequestVisualAppointmentInfo;
		}
		protected override IVisualElementAccessor<VisualAppointmentControl> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<VisualAppointmentControl>(panel, ZIndex);
		}
		protected virtual void OnRequestVisualAppointmentInfo(object sender, RequestVisualAppointmentInfoEventArgs e) {
			switch (e.DragState) {
				default:
				case AppointmentDragState.Begin:
					OnBeginDrag(e);
					break;
				case AppointmentDragState.Drag:
					OnDrag(e);
					break;
				case AppointmentDragState.Cancel:
					OnDragCancel(e);
					break;
			}
		}
		protected virtual void OnBeginDrag(RequestVisualAppointmentInfoEventArgs e) {
			List<VisualAppointmentInfo> selectedAppointments = e.AppointmentInfos;
			int count = VisualItemsAccessor.Count;
			for (int i = 0; i < count; i++) {
				VisualAppointmentControl container = VisualItemsAccessor[i];
				if (!e.SourceAppointments.Contains(container.GetAppointment()))
					continue;
				Size size = container.RenderSize;
				Point topLeft = container.TranslatePoint(new Point(0, 0), (UIElement)Panel);
				Rect bounds = new Rect(topLeft, size);
				Panel panel = ((IConvertible<Panel>)Panel).Convert();
				selectedAppointments.Add(new VisualAppointmentInfo(container, bounds, panel, this));
			}
		}
		protected virtual void OnDrag(RequestVisualAppointmentInfoEventArgs e) {
			List<VisualAppointmentInfo> dragInfos = e.AppointmentInfos;
			int count = dragInfos.Count;
			for (int i = 0; i < count; i++) {
				VisualAppointmentInfo dragInfo = dragInfos[i];
				if (dragInfo.Panel != this)
					continue;
				VisualAppointmentControl appointmentControl = dragInfo.Container as VisualAppointmentControl;
				if (appointmentControl == null)
					continue;
				SetAppointmentVisualState(appointmentControl, !e.Copy);
			}
		}
		protected virtual void OnDragCancel(RequestVisualAppointmentInfoEventArgs e) {
			List<VisualAppointmentInfo> dragInfos = e.AppointmentInfos;
			int count = dragInfos.Count;
			for (int i = 0; i < count; i++) {
				VisualAppointmentInfo dragInfo = dragInfos[i];
				if (dragInfo.Panel != this)
					continue;
				VisualAppointmentControl appointmentControl = dragInfo.Container as VisualAppointmentControl;
				if (appointmentControl == null)
					continue;
				SetAppointmentVisualState(appointmentControl, false);
			}
		}
		protected virtual void SetAppointmentVisualState(VisualAppointmentControl appointmentControl, bool dragged) {
			appointmentControl.SetDraggedVisualState(dragged);
		}
		protected override Size MeasureCore(Size availableSize) {
			if (availableSize.Width <= 0)
				return Size.Empty;
			IVisualElementGenerator<VisualAppointmentControl, AppointmentControl> appointmentGenerator = GetItemsGenerator();
			appointmentGenerator.Reset();
			CalculateAppointmentsLayout(appointmentGenerator, availableSize);
			appointmentGenerator.Release(RemoveUnusedItems);
			return CalculateDesiredSize(availableSize);
		}
		protected override Size ArrangeCore(Rect arrangeBounds) {
			if (arrangeBounds.Width <= 0)
				return arrangeBounds.Size();
			BeginArrangeAppointments();
			int count = VisualItemsAccessor.Count;
			for (int i = 0; i < count; i++) {
				VisualAppointmentControl appointmentControl = VisualItemsAccessor[i];
				if (appointmentControl.Visibility != Visibility.Visible)
					continue;
				if (IsAppointmentControlSizeChanged(appointmentControl)) {
					InvalidateMeasure();
					break;
				}
				Rect rect = CalculateAppointmentRect(appointmentControl);
				RectHelper.Offset(ref rect, arrangeBounds.X, arrangeBounds.Y);
				appointmentControl.Arrange(rect);
			}
			EndArragneAppointments();
			return arrangeBounds.Size();
		}
		protected virtual bool IsAppointmentControlSizeChanged(VisualAppointmentControl appointmentControl) {
			return false;
		}
		protected virtual void BeginArrangeAppointments() {
		}
		protected virtual void EndArragneAppointments() {
		}
		public Rect GetAppointmentRect(VisualAppointmentControl visualAppointmentControl) {
			return visualAppointmentControl.CachedRectangle;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				if (Scheduler != null)
					UnsubscribeFromSchedulerEvents(Scheduler);
			}
		}
		protected abstract void CalculateAppointmentsLayout(IVisualElementGenerator<VisualAppointmentControl, AppointmentControl> appointmentGenerator, Size availableSize);
		protected abstract Size CalculateDesiredSize(Size availableSize);
		protected abstract Rect CalculateAppointmentRect(VisualAppointmentControl appointmentControl);
		public abstract VisualAppointmentControl CreateVisualAppointment();
		#region IAppointmentsPanel Members
		public Panel Visual { get { return ((IConvertible<Panel>)Panel).Convert(); } }
		#endregion
		#region ICellsChangedListener
		void ICellsComponentChangedListener.OnCellsChanged() {
			RaiseComponentChanged();
		}
		#endregion
	}
	#endregion
	#region HorizontalAppointmentsComponentFixedHeightLayoutStrategy
	public class HorizontalAppointmentsComponentFixedHeightLayoutStrategy : AppointmentsLayoutStrategyBase {
		public HorizontalAppointmentsComponentFixedHeightLayoutStrategy(IHorizontalAppointmentLayoutOwner layoutOwner)
			: base(layoutOwner) {
		}
		public IHorizontalAppointmentsComponent AppointmentsComponent { get { return (IHorizontalAppointmentsComponent)LayoutOwner; } }
		protected override MoreButtonCalculatorBase CreateMoreButtonCalculator() {
			return new WeekViewMoreButtonComponentCalculator(AppointmentsComponent.LayoutInfo);
		}
	}
	#endregion
	#region TimelineHorizontalAppointmentsComponentFixedHeightLayoutStrategy
	public class TimelineHorizontalAppointmentsComponentFixedHeightLayoutStrategy : HorizontalAppointmentsComponentFixedHeightLayoutStrategy {
		Size viewportSize;
		public TimelineHorizontalAppointmentsComponentFixedHeightLayoutStrategy(Size viewportSize, IHorizontalAppointmentLayoutOwner layoutOwner)
			: base(layoutOwner) {
			this.viewportSize = viewportSize;
		}
		public override PanelConstraintCalculator CreatePanelConstraintCalculator(bool isResizable, ICellInfoProvider cellsInfo) {
			if (isResizable)
				return new TimelineResizableAppointmentPanelConstraintCalculator(this.viewportSize, cellsInfo);
			return base.CreatePanelConstraintCalculator(isResizable, cellsInfo);
		}
	}
	#endregion
	#region HorizontalAppointmentComponentAutoHeightLayoutStrategy
	public class HorizontalAppointmentComponentAutoHeightLayoutStrategy : AppointmentsAutoHeightLayoutStrategyBase {
		public HorizontalAppointmentComponentAutoHeightLayoutStrategy(IHorizontalAppointmentsComponent appointmentPanel)
			: base(appointmentPanel) {
		}
		public IHorizontalAppointmentsComponent AppointmentsComponent { get { return (IHorizontalAppointmentsComponent)LayoutOwner; } }
		protected override MoreButtonCalculatorBase CreateMoreButtonCalculator() {
			return new WeekViewMoreButtonComponentCalculator(AppointmentsComponent.LayoutInfo);
		}
	}
	#endregion
	public class TimelineHorizontalAppointmentComponentAutoHeightLayoutStrategy : HorizontalAppointmentComponentAutoHeightLayoutStrategy {
		Size viewportSize;
		SchedulerScrollBarVisibility scrollBarVisible;
		public TimelineHorizontalAppointmentComponentAutoHeightLayoutStrategy(Size viewportSize, IHorizontalAppointmentsComponent appointmentPanel, SchedulerScrollBarVisibility scrollBarVisible)
			: base(appointmentPanel) {
			this.scrollBarVisible = scrollBarVisible;
			this.viewportSize = viewportSize;
		}
		public override PanelConstraintCalculator CreatePanelConstraintCalculator(bool isResizable, ICellInfoProvider cellsInfo) {
			if (isResizable)
				return new TimelineResizableAppointmentPanelConstraintCalculator(this.viewportSize, cellsInfo);
			return base.CreatePanelConstraintCalculator(isResizable, cellsInfo);
		}
		protected override double CalculateAvailableAppoimenmentMaxHeight(double position, double appointmentHeight, VisualAppointmentControl visualAppointmentControl) {
			if (this.scrollBarVisible == SchedulerScrollBarVisibility.Never)
				return base.CalculateAvailableAppoimenmentMaxHeight(position, appointmentHeight, visualAppointmentControl);
			return appointmentHeight;
		}
	}
	#region HorizontalAppointmentComponentEqualHeightLayoutStrategy
	public class HorizontalAppointmentComponentEqualHeightLayoutStrategy : AppointmentsEqualHeightLayoutStrategyBase {
		public HorizontalAppointmentComponentEqualHeightLayoutStrategy(IHorizontalAppointmentsComponent appointmentPanel)
			: base(appointmentPanel) {
		}
		public IHorizontalAppointmentsComponent AppointmentsComponent { get { return (IHorizontalAppointmentsComponent)LayoutOwner; } }
		protected override MoreButtonCalculatorBase CreateMoreButtonCalculator() {
			return new WeekViewMoreButtonComponentCalculator(AppointmentsComponent.LayoutInfo);
		}
	}
	#endregion
	#region HorizontalAppointmentsComponentBase (abstract class)
	public abstract class HorizontalAppointmentsComponentBase : AppointmentsComponentBase, IHorizontalAppointmentsComponent {
		protected HorizontalAppointmentsComponentBase(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
			LayoutInfo = new AppointmentsLayoutInfo();
			CreateLayoutStrategy();
		}
		#region Properties
		#region IsResizable
		public bool IsResizable {
			get { return (bool)GetValue(IsResizableProperty); }
			set { SetValue(IsResizableProperty, value); }
		}
		public static readonly DependencyProperty IsResizableProperty = CreateIsResizableProperty();
		static DependencyProperty CreateIsResizableProperty() {
			return DependencyPropertyHelper.RegisterProperty<HorizontalAppointmentsComponentBase, bool>("IsResizable", false, (d, e) => d.OnIsResizableChanged(e.OldValue, e.NewValue), null);
		}
		private void OnIsResizableChanged(bool oldValue, bool newValue) {
			CreateLayoutStrategy();
			InvalidateMeasure();
		}
		#endregion
		protected virtual AppointmentSnapToCellsMode SnapToCells { get { return AppointmentSnapToCellsMode.Always; } }
		protected virtual bool AutoHeight { get { return false; } }
		protected virtual bool IsStatusVisible { get { return false; } }
		protected AppointmentsLayoutStrategyBase LayoutStrategy { get; private set; }
		protected internal AppointmentsLayoutInfo LayoutInfo { get; private set; }
		public override int ZIndex { get { return 150; } }
		#endregion
		public override VisualAppointmentControl CreateVisualAppointment() {
			return new VisualHorizontalAppointmentControl();
		}
		protected void CreateLayoutStrategy() {
			LayoutStrategy = SelectLayoutStrategy();
		}
		protected override void CalculateAppointmentsLayout(IVisualElementGenerator<VisualAppointmentControl, AppointmentControl> appointmentGenerator, Size availableSize) {
			LayoutInfo.Clear();
			LayoutStrategy.CalculateLayout(ViewModelItems, availableSize, appointmentGenerator);
		}
		protected override Rect CalculateAppointmentRect(VisualAppointmentControl appointmentControl) {
			return LayoutStrategy.CalculateAppointmentRect(appointmentControl);
		}
		protected override Size CalculateDesiredSize(Size availableSize) {
			return LayoutStrategy.CalculateDesiredSize(availableSize, VisualItemsAccessor);
		}
		protected override void BeginArrangeAppointments() {
			LayoutStrategy.BeginArrangeAppointments(VisualItemsAccessor);
		}
		protected virtual AppointmentsLayoutStrategyBase SelectLayoutStrategy() {
			if (AutoHeight)
				return new HorizontalAppointmentComponentAutoHeightLayoutStrategy(this);
			return new HorizontalAppointmentsComponentFixedHeightLayoutStrategy(this);
		}
		#region IHorizontalAppointmentLayoutOwner implementation
		ICellInfoProvider IHorizontalAppointmentLayoutOwner.CellsInfo { get { return CellsComponent; } }
		bool IHorizontalAppointmentLayoutOwner.IsStatusVisible { get { return IsStatusVisible; } }
		AppointmentSnapToCellsMode IHorizontalAppointmentLayoutOwner.SnapToCells { get { return SnapToCells; } }
		Thickness IHorizontalAppointmentLayoutOwner.CellPadding { get { return CellPadding; } }
		bool IHorizontalAppointmentLayoutOwner.IsResizable { get { return IsResizable; } }
		AppointmentsLayoutInfo IHorizontalAppointmentsComponent.LayoutInfo { get { return LayoutInfo; } }
		TimeInterval IHorizontalAppointmentLayoutOwner.GetCellIntervalByIndex(int index) {
			VisualTimeCellBase cell = CellsComponent[index];
			XtraSchedulerDebug.Assert(cell != null);
			return cell == null ? TimeInterval.Empty : ((VisualTimeCellBaseContent)cell.Content).GetInterval();
		}
		int IHorizontalAppointmentLayoutOwner.GetDefaultAppointmentHeight() {
			return GetDefaultAppointmentHeight();
		}
		#endregion
		protected virtual int GetDefaultAppointmentHeight() {
			return 22;
		}
	}
	#endregion
	#region HorizontalAppointmentsComponent
	public class HorizontalAppointmentsComponent : HorizontalAppointmentsComponentBase {
		Size viewportSize;
		public HorizontalAppointmentsComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		#region Properties
		public Size ViewportSize {
			get {
				return viewportSize;
			}
			set {
				if (viewportSize == value)
					return;
				viewportSize = value;
				CreateLayoutStrategy();
				InvalidateMeasure();
			}
		}
		protected override AppointmentSnapToCellsMode SnapToCells { get { return Properties.SnapToCells; } }
		protected override bool AutoHeight { get { return Properties.AppointmentAutoHeight; } }
		protected override bool IsStatusVisible { get { return Properties.IsAppointmentStatusVisible; } }
		protected IHorizontalAppointmentComponentProperties Properties { get { return Panel.GetProperties<IHorizontalAppointmentComponentProperties>(); } }
		protected HorizontalVisualAppointmentGenerator2 AppointmentGenerator { get; private set; }
		protected IResourceComponentProperties ResourceComponentProperties { get { return Panel.GetProperties<IResourceComponentProperties>(); } }
		#endregion
		public override void Initialize() {
			base.Initialize();
			AppointmentGenerator = new HorizontalVisualAppointmentGenerator2(this, VisualItemsAccessor);
		}
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			CreateLayoutStrategy();
			if (e.PropertyName == PanelPropertyNames.AppointmentAutoHeightPropertyName)
				VisualItemsAccessor.ForEach(item => item.InvalidateLayout());
			RaiseComponentChanged();
		}
		public override VisualAppointmentControl CreateVisualAppointment() {
			return new VisualHorizontalAppointmentControl();
		}
		protected override IVisualElementGenerator<VisualAppointmentControl, AppointmentControl> GetItemsGenerator() {
			return AppointmentGenerator;
		}
		protected override int GetDefaultAppointmentHeight() {
			if (Properties == null)
				return base.GetDefaultAppointmentHeight();
			int height = Properties.DefaultAppointmentHeight;
			if (height <= 0)
				return base.GetDefaultAppointmentHeight();
			return height;
		}
		protected override AppointmentsLayoutStrategyBase SelectLayoutStrategy() {
			if (!IsResizable)
				return base.SelectLayoutStrategy();
			if (AutoHeight)
				return new TimelineHorizontalAppointmentComponentAutoHeightLayoutStrategy(ViewportSize, this, ResourceComponentProperties.ResourceScrollBarVisible);
			return new TimelineHorizontalAppointmentsComponentFixedHeightLayoutStrategy(ViewportSize, this);
		}
	}
	#endregion
	#region HorizontalDraggedAppointmentsComponent
	public class HorizontalDraggedAppointmentsComponent : HorizontalAppointmentsComponentBase {
		public HorizontalDraggedAppointmentsComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		protected IDraggedAppointmentComponentProperties Properties { get { return Panel.GetProperties<IDraggedAppointmentComponentProperties>(); } }
		protected Style Style { get { return Properties.DraggedAppointmentStyle; } }
		protected override AppointmentsLayoutStrategyBase SelectLayoutStrategy() {
			return new HorizontalAppointmentComponentEqualHeightLayoutStrategy(this);
		}
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			RaiseComponentChanged();
		}
		public override VisualAppointmentControl CreateVisualAppointment() {
			return new VisualDraggedAppointmentControl();
		}
		protected override IVisualElementGenerator<VisualAppointmentControl, AppointmentControl> GetItemsGenerator() {
			return new DraggedVisualAppointmentGenerator(this, VisualItemsAccessor, Style);
		}
	}
	#endregion
	#region VisualAppointmentGenerator (abstract class)
	public abstract class VisualAppointmentGenerator : ControlGenerator<VisualAppointmentControl, AppointmentControl> {
		protected VisualAppointmentGenerator(IVisualComponent owner, IVisualElementAccessor<VisualAppointmentControl> elementAccessor, Style appointmentStyle)
			: base(owner, elementAccessor, appointmentStyle) {
		}
		protected override void PrepareItemOverride(VisualAppointmentControl control, AppointmentControl context) {
			if (control.Style == null)
				control.Style = Style;
			control.Owner = Owner;
			context.ShouldCopyFrom = true;
			((ISupportCopyFrom<AppointmentControl>)control).CopyFrom(context);
			control.IsLayoutValid = false;
		}
	}
	#endregion
	#region HorizontalVisualAppointmentGenerator
	public class HorizontalVisualAppointmentGenerator : VisualAppointmentGenerator {
		public HorizontalVisualAppointmentGenerator(IVisualComponent owner, IVisualElementAccessor<VisualAppointmentControl> elementAccessor, Style appointmentStyle)
			: base(owner, elementAccessor, appointmentStyle) {
		}
		protected internal override VisualAppointmentControl CreateNewItemOverride(AppointmentControl context) {
			return new VisualHorizontalAppointmentControl();
		}
	}
	#endregion
	#region DraggedVisualAppointmentGenerator
	public class DraggedVisualAppointmentGenerator : VisualAppointmentGenerator {
		public DraggedVisualAppointmentGenerator(IVisualComponent owner, IVisualElementAccessor<VisualAppointmentControl> elementAccessor, Style appointmentStyle)
			: base(owner, elementAccessor, appointmentStyle) {
		}
		protected internal override VisualAppointmentControl CreateNewItemOverride(AppointmentControl context) {
			return new VisualDraggedAppointmentControl();
		}
	}
	#endregion
	public abstract class VisualAppointmentGenerator2 : IVisualElementGenerator<VisualAppointmentControl, AppointmentControl> {
		#region Fields
		Dictionary<Appointment, VisualAppointmentControl> usedControlsTable;
		Dictionary<Appointment, VisualAppointmentControl> oldControlsTable;
		Stack<VisualAppointmentControl> cachedControls;
		Stack<VisualAppointmentControl> controlsStack;
		#endregion
		protected VisualAppointmentGenerator2(IVisualComponent owner, IVisualElementAccessor<VisualAppointmentControl> elementAccessor) {
			Owner = owner;
			ElementAccessor = elementAccessor;
			this.usedControlsTable = new Dictionary<Appointment, VisualAppointmentControl>();
			this.cachedControls = new Stack<VisualAppointmentControl>();
			this.controlsStack = new Stack<VisualAppointmentControl>();
		}
		#region Properties
		public IVisualElementAccessor<VisualAppointmentControl> ElementAccessor { get; private set; }
		public IVisualComponent Owner { get; set; }
		#endregion
		public virtual VisualAppointmentControl GenerateNext(AppointmentControl context) {
			Appointment appointment = context.Appointment;
			VisualAppointmentControl control;
			if (oldControlsTable.TryGetValue(appointment, out control))
				oldControlsTable.Remove(appointment);
			else if (cachedControls.Count > 0)
				control = cachedControls.Pop();
			else {
				control = CreateVisualAppointmentControl();
				ElementAccessor.Add(control);
				control.ApplyTemplate();
			}
			usedControlsTable.Add(appointment, control);
			PrepareItemOverride(control, context);
			controlsStack.Push(control);
			return control;
		}
		protected virtual void PrepareItemOverride(VisualAppointmentControl control, AppointmentControl context) {
			if (control.Visibility != Visibility.Visible)
				control.Visibility = Visibility.Visible;
			control.Owner = Owner;
			double oldVersion = control.Version;
			context.ShouldCopyFrom = true;
			((ISupportCopyFrom<AppointmentControl>)control).CopyFrom(context);
			bool isStateValid = oldVersion == control.Version;
			control.IsLayoutValid = control.IsLayoutValid && isStateValid;
		}
		public virtual void Release(bool removeUnusedItems) {
			foreach (VisualAppointmentControl control in oldControlsTable.Values)
				ProcessUnusedItem(control);
			this.oldControlsTable = null;
			this.controlsStack.Clear();
		}
		protected virtual void ProcessUnusedItem(VisualAppointmentControl control) {
			control.Visibility = Visibility.Collapsed;
			control.IsLayoutValid = false;
			cachedControls.Push(control);
		}
		public virtual void Reset() {
			this.oldControlsTable = this.usedControlsTable;
			this.usedControlsTable = new Dictionary<Appointment, VisualAppointmentControl>();
			this.controlsStack.Clear();
		}
		public virtual void MoveBack() {
			if (this.controlsStack.Count == 0)
				return;
			VisualAppointmentControl control = this.controlsStack.Pop();
			this.usedControlsTable.Remove(control.GetAppointment());
			ProcessUnusedItem(control);
		}
		protected abstract VisualHorizontalAppointmentControl CreateVisualAppointmentControl();
	}
	public class HorizontalVisualAppointmentGenerator2 : VisualAppointmentGenerator2 {
		public HorizontalVisualAppointmentGenerator2(IVisualComponent owner, IVisualElementAccessor<VisualAppointmentControl> eleementAccessor)
			: base(owner, eleementAccessor) {
		}
		protected override VisualHorizontalAppointmentControl CreateVisualAppointmentControl() {
			return new VisualHorizontalAppointmentControl();
		}
	}
}
