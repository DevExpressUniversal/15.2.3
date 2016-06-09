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
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Xpf.Scheduler.Native;
using System.Collections.Generic;
using DevExpress.Utils;
using System.Diagnostics;
using System.Windows.Media;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Scheduler.Drawing.Components;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface ICellInfoProvider {
		Rect GetCellRectByIndex(int index);
		int GetCellCount();
	}
	public interface IHorizontalAppointmentLayoutOwner {
		bool IsStatusVisible { get; }
		AppointmentSnapToCellsMode SnapToCells { get; }
		Thickness CellPadding { get; }
		bool IsResizable { get; }
		int GetDefaultAppointmentHeight();
		TimeInterval GetCellIntervalByIndex(int index);
		ICellInfoProvider CellsInfo { get; }
	}
	public interface IHorizontalAppointmentPanel : IHorizontalAppointmentLayoutOwner {
		ISchedulerTimeCellControl TimeCellControl { get; }
		MoreButtonInfoCollection MoreButtonInfos { get; }
	}
	public interface IHorizontalAppointmentsComponent : IHorizontalAppointmentLayoutOwner {
		AppointmentsLayoutInfo LayoutInfo { get; }
	}
	public class DynamicHorizontalAppointmentsPanel : DynamicAppointmentsPanelBase, IHorizontalAppointmentPanel, IChildAccessor<VisualAppointmentControl> {
		const int DefaultAppointmentHeightConst = 22;
		const int DefaultAppointmentWidth = 23;
		const int DefaultStatusHeight = 5;
		readonly MoreButtonInfoCollection moreButtonInfos;
		public DynamicHorizontalAppointmentsPanel() {
			AppointmentGenerator = new FastHorizontalAppointmentGenerator(this, AppointmentStyle);
			this.moreButtonInfos = new MoreButtonInfoCollection();
			RecreateLayoutStrategy();
		}
		#region Properties
		#region AppointmentsInfoContainer
		public IAppoinmentsInfoContainer AppointmentsInfoContainer {
			get { return (IAppoinmentsInfoContainer)GetValue(AppointmentsInfoContainerProperty); }
			set { SetValue(AppointmentsInfoContainerProperty, value); }
		}
		public static readonly DependencyProperty AppointmentsInfoContainerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DynamicHorizontalAppointmentsPanel, IAppoinmentsInfoContainer>("AppointmentsInfoContainer", null);
		#endregion
		#region IsResizable
		public bool IsResizable {
			get { return (bool)GetValue(IsResizableProperty); }
			set { SetValue(IsResizableProperty, value); }
		}
		public static readonly DependencyProperty IsResizableProperty = CreateIsResizableProperty();
		static DependencyProperty CreateIsResizableProperty() {
			return DependencyPropertyHelper.RegisterProperty<DynamicHorizontalAppointmentsPanel, bool>("IsResizable", false, (d, e) => d.OnIsResizableChanged(e.OldValue, e.NewValue), null);
		}
		private void OnIsResizableChanged(bool oldValue, bool newValue) {
			RecreateLayoutStrategy();
			DoInvalidateMeasure();
		}
		#endregion
		#region AutoHeight
		public bool AutoHeight {
			get { return (bool)GetValue(AutoHeightProperty); }
			set { SetValue(AutoHeightProperty, value); }
		}
		public static readonly DependencyProperty AutoHeightProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DynamicHorizontalAppointmentsPanel, bool>("AutoHeight", default(bool), (d, e) => d.OnAutoHeightChanged(e.OldValue, e.NewValue));
		void OnAutoHeightChanged(bool oldValue, bool newValue) {
			RecreateLayoutStrategy();
			DoInvalidateMeasure();
			foreach (UIElement child in Children) {
				VisualAppointmentControl appointmentControl = child as VisualAppointmentControl;
				if (appointmentControl != null)
					appointmentControl.InvalidateLayout();
			}
		}
		#endregion
		#region IsStatusVisible
		public bool IsStatusVisible {
			get { return (bool)GetValue(IsStatusVisibleProperty); }
			set { SetValue(IsStatusVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsStatusVisibleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DynamicHorizontalAppointmentsPanel, bool>("IsStatusVisible", default(bool), (d, e) => d.OnIsStatusVisibleChanged(e.OldValue, e.NewValue));
		void OnIsStatusVisibleChanged(bool oldValue, bool newValue) {
			DoInvalidateMeasure();
		}
		#endregion
		#region DefaultAppointmentHeight
		public Int32 DefaultAppointmentHeight {
			get { return (Int32)GetValue(DefaultAppointmentHeightProperty); }
			set { SetValue(DefaultAppointmentHeightProperty, value); }
		}
		public static readonly DependencyProperty DefaultAppointmentHeightProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DynamicHorizontalAppointmentsPanel, Int32>("DefaultAppointmentHeight", DefaultAppointmentHeightConst, (d, e) => d.OnDefaultAppointmentHeightChanged(e.OldValue, e.NewValue));
		void OnDefaultAppointmentHeightChanged(Int32 oldValue, Int32 newValue) {
			DoInvalidateMeasure();
		}
		#endregion    
		protected FastHorizontalAppointmentGenerator AppointmentGenerator { get; private set; }
		AppointmentsLayoutStrategyBase LayoutStrategy { get; set; }
		public MoreButtonInfoCollection MoreButtonInfos { get { return moreButtonInfos; } }
		#endregion
		protected override void OnIsDraggedModeChanged(bool oldValue, bool newValue) {
			base.OnIsDraggedModeChanged(oldValue, newValue);
			RecreateLayoutStrategy();
		}
		public override VisualAppointmentControl CreateVisualAppointment() {
			return new VisualHorizontalAppointmentControl();
		}
		void RecreateLayoutStrategy() {
			AppointmentsLayoutStrategyBase newStrategy = null;
			if (IsDraggedMode)
				newStrategy = new HorizontalAppointmentsPanelEqualHeightLayoutStrategy(this);
			else if (AutoHeight)
				newStrategy = new HorizontalAppointmentsPanelAutoHeightLayoutStrategy(this);
			else
				newStrategy = new HorizontalAppointmentsPanelFixedHeightLayoutStrategy(this);
			LayoutStrategy = newStrategy;
		}
		protected override void DoInvalidateMeasure() {
			LayoutInfo = LayoutInfo.Empty;
			if (IsResizable) {
				UIElement parent = VisualTreeHelper.GetParent(this) as UIElement;
				if (parent != null)
					Dispatcher.BeginInvoke(new Action(() => parent.InvalidateMeasure()));
			}
			base.DoInvalidateMeasure();
		}
		protected override Size CalculateDesiredSize(Size availableSize, UIElementCollection children) {
			return LayoutStrategy.CalculateDesiredSize(availableSize, this);
		}
		protected override void CalculateAppointmentsLayout(IAppointmentGenerator appointmentGenerator, Size availableSize) {
			if (AppointmentControls == null)
				return;
			LayoutStrategy.CalculateLayout(AppointmentControls, availableSize, appointmentGenerator);
		}
		protected override void BeginArrangeAppointments() {
			base.BeginArrangeAppointments();
			LayoutStrategy.BeginArrangeAppointments(this);
		}
		protected override Rect CalculateAppointmentRect(VisualAppointmentControl appointmentControl) {
			return LayoutStrategy.CalculateAppointmentRect(appointmentControl);
		}
		protected internal override double GetAppointmentMinWidth() {
			return DefaultAppointmentWidth;
		}
		protected override PanelController CreatePanelController() {
			return new AppointmentPanelController(this);
		}
		protected override IAppointmentGenerator CreateAppointmentGenerator(Style appointmentStyle) {
			if (IsDraggedMode)
				return new DraggedAppointmentGenerator(this, appointmentStyle);
			return AppointmentGenerator;
		}
		protected override void UpdateVisualAppointmentStatistics(Rect rect) {
			AppointmentPanelController.UpdateAppointmentRectStatistics(rect);
			base.UpdateVisualAppointmentStatistics(rect);
		}
		#region IHorizontalAppointmentPanel interface implementation
		ICellInfoProvider IHorizontalAppointmentLayoutOwner.CellsInfo { get { return CellsInfo; } }
		bool IHorizontalAppointmentLayoutOwner.IsStatusVisible { get { return IsStatusVisible; } }
		AppointmentSnapToCellsMode IHorizontalAppointmentLayoutOwner.SnapToCells { get { return SnapToCells; } }
		Thickness IHorizontalAppointmentLayoutOwner.CellPadding { get { return CellPadding; } }
		bool IHorizontalAppointmentLayoutOwner.IsResizable { get { return IsResizable; } }
		ISchedulerTimeCellControl IHorizontalAppointmentPanel.TimeCellControl { get { return SchedulerTimeCellControl; } }
		MoreButtonInfoCollection IHorizontalAppointmentPanel.MoreButtonInfos { get { return MoreButtonInfos; } }
		TimeInterval IHorizontalAppointmentLayoutOwner.GetCellIntervalByIndex(int index) {
			SchedulerItemsControl itemsControl = SchedulerTimeCellControl as SchedulerItemsControl;
			XtraSchedulerDebug.Assert(itemsControl != null);
			if (itemsControl == null)
				return TimeInterval.Empty;
			ItemContainerGenerator generator = itemsControl.ItemContainerGenerator;
			VisualTimeCellBase cell = generator.ContainerFromIndex(index) as VisualTimeCellBase;
			XtraSchedulerDebug.Assert(cell != null);
			return cell == null ? TimeInterval.Empty : ((VisualTimeCellBaseContent)cell.Content).GetInterval();
		}
		#endregion
		#region IChildAccessor<VisualAppointmentControl> implementation
		int IChildAccessor<VisualAppointmentControl>.Count {
			get {
#if SL
				return this.Children.Count;
#else
				return VisualChildrenCount;
#endif
			}
		}
		VisualAppointmentControl IChildAccessor<VisualAppointmentControl>.this[int index] {
			get {
#if SL
				if (index >= Children.Count)
					return null;
				return Children[index] as VisualAppointmentControl;
#else
				if (index >= VisualChildrenCount)
					return null;
				return GetVisualChild(index) as VisualAppointmentControl;
#endif
			}
		}
		#endregion
		int IHorizontalAppointmentLayoutOwner.GetDefaultAppointmentHeight() {
			if (DefaultAppointmentHeight <= 0)
				return DefaultAppointmentHeightConst;
			return DefaultAppointmentHeight; 
		}
	}
	#region OldCode
	#endregion
	public interface IAppointmentGenerator : IVisualElementGeneratorCore<VisualAppointmentControl, AppointmentControl> {
		void Begin();
		void End();
	}
	public class AppointmentGeneratorBase<T> : IAppointmentGenerator where T : VisualAppointmentControl, new() {
		DynamicAppointmentsPanelBase panel;
		int currentIndex = 0;
		readonly Style appointmentStyle;
		public AppointmentGeneratorBase(DynamicAppointmentsPanelBase panel, Style appointmentStyle) {
			this.panel = panel;
			this.appointmentStyle = appointmentStyle;
		}
		public DynamicAppointmentsPanelBase Panel { get { return panel; } }
		public Style AppointmentStyle { get { return appointmentStyle; } }
		public void Begin() {
			this.currentIndex = 0;
		}
		public VisualAppointmentControl GenerateNext(AppointmentControl appointmentControl) {
			bool isNewChildren = this.currentIndex > panel.Children.Count - 1;
			T visualAppointmentControl = isNewChildren ? new T() : (T)Panel.Children[this.currentIndex];
			appointmentControl.ShouldCopyFrom = true;
			if (isNewChildren) {
				Panel.Children.Add(visualAppointmentControl);
				if (AppointmentStyle != null)
					visualAppointmentControl.Style = AppointmentStyle;
				SchedulerLogger.Trace(XpfLoggerTraceLevel.AppointmentPanel, "Create new VisualAppointmentControl: {0}", this.currentIndex);
			}
			else if (visualAppointmentControl.Visibility != Visibility.Visible)
				visualAppointmentControl.Visibility = Visibility.Visible;
			visualAppointmentControl.IsLayoutValid = false;
			((ISupportCopyFrom<AppointmentControl>)visualAppointmentControl).CopyFrom(appointmentControl);
			this.currentIndex++;
			return visualAppointmentControl;
		}
		public void MoveBack() {
			if (this.currentIndex <= 0)
				return;
			this.currentIndex--;
			T appointmentControl = Panel.Children[this.currentIndex] as T;
			appointmentControl.Visibility = Visibility.Collapsed;
		}
		public void End() {
			int redundantChildrenCount = Panel.Children.Count - this.currentIndex;
			if (redundantChildrenCount <= 0)
				return;
			for (int i = Panel.Children.Count - 1; i >= this.currentIndex; i--)
				Panel.Children.RemoveAt(i);
		}
	}
	public abstract class FastAppointmentGeneratorBase<T> : IAppointmentGenerator where T : VisualAppointmentControl, new() {
		class AppointmentKey {
			public AppointmentKey(Appointment appointment, TimeInterval interval) {
				Appointment = appointment;
				Interval = interval;
			}
			public Appointment Appointment { get; private set; }
			public TimeInterval Interval { get; private set; }
		}
		class AppointmentKeyComparer : IEqualityComparer<AppointmentKey> {
			public bool Equals(AppointmentKey x, AppointmentKey y) {
				if (Object.ReferenceEquals(x.Appointment, y.Appointment))
					return x.Interval.Equals(y.Interval);
				return false;
			}
			public int GetHashCode(AppointmentKey obj) {
				return obj.Appointment.GetHashCode();
			}
		}
		class VisualAppointmentControlCache : Dictionary<AppointmentKey, T> {
			public VisualAppointmentControlCache()
				: base(new AppointmentKeyComparer()) {
			}
		}
		#region Fields
		readonly DynamicAppointmentsPanelBase panel;
		readonly Style appointmentStyle;
		VisualAppointmentControlCache usedControlsTable;
		VisualAppointmentControlCache oldControlsTable;
		Stack<T> cachedControls;
		Stack<T> controlsStack;
		#endregion
		protected FastAppointmentGeneratorBase(DynamicAppointmentsPanelBase panel, Style appointmentStyle) {
			this.panel = panel;
			this.appointmentStyle = appointmentStyle;
			this.usedControlsTable = new VisualAppointmentControlCache();
			this.cachedControls = new Stack<T>();
			this.controlsStack = new Stack<T>();
		}
		public virtual VisualAppointmentControl GenerateNext(AppointmentControl context) {
			AppointmentKey key = new AppointmentKey(context.Appointment, context.ViewInfo.Interval);
			T control;
			if (oldControlsTable.TryGetValue(key, out control))
				oldControlsTable.Remove(key);
			else if (cachedControls.Count > 0) {
				control = cachedControls.Pop();
			}
			else {
				control = new T();
				this.panel.Children.Add(control);
				control.ApplyTemplate();
			}
			usedControlsTable.Add(key, control);
			PrepareItemOverride(control, context);
			controlsStack.Push(control);
			return control;
		}
		protected virtual void PrepareItemOverride(T control, AppointmentControl context) {
			if (control.Visibility != Visibility.Visible)
				control.Visibility = Visibility.Visible;
			double oldVersion = control.Version;
			context.ShouldCopyFrom = true;
			((ISupportCopyFrom<AppointmentControl>)control).CopyFrom(context);
			bool isStateValid = oldVersion == control.Version;
			control.IsLayoutValid = control.IsLayoutValid && isStateValid;
		}
		public virtual void Begin() {
			this.oldControlsTable = this.usedControlsTable;
			this.usedControlsTable = new VisualAppointmentControlCache();
			this.controlsStack.Clear();
		}
		public virtual void End() {
			foreach (T control in oldControlsTable.Values)
				ProcessUnusedItem(control);
			this.oldControlsTable = null;
			this.controlsStack.Clear();
		}
		protected virtual void ProcessUnusedItem(T control) {
			control.Visibility = Visibility.Collapsed;
			control.IntermediateLayoutViewInfo = null;
			control.IsLayoutValid = false;
			cachedControls.Push(control);
		}
		public virtual void MoveBack() {
			if (this.controlsStack.Count == 0)
				return;
			T control = this.controlsStack.Pop();
			AppointmentKey key = new AppointmentKey(control.GetAppointment(), control.ViewInfo.GetInterval());
			this.usedControlsTable.Remove(key);
			ProcessUnusedItem(control);
		}
	}
	public class FastHorizontalAppointmentGenerator : FastAppointmentGeneratorBase<VisualHorizontalAppointmentControl> {
		public FastHorizontalAppointmentGenerator(DynamicHorizontalAppointmentsPanel panel, Style appointmentStyle)
			: base(panel, appointmentStyle) {
		}
	}
	#region SequentialBusyIndexCollection
	public class SequentialBusyIndexCollection : DevExpress.Utils.DXCollection<double> {
		protected override void InsertIfNotAlreadyInserted(int index, double obj) {
			if (!UniquenessProvider.LookupObject(obj))
				InsertCore(index, obj);
		}
		public int BinarySearch(double obj) {
			return base.BinarySearch(obj, Comparer<double>.Default);
		}
		public void Sort() {
			base.Sort(Comparer<Double>.Default);
		}
	}
	#endregion
	#region SortedList<T>
	public class SortedList<T> {
		readonly List<T> innerList;
		readonly IComparer<T> comparer;
		public SortedList() {
			this.innerList = new List<T>();
		}
		public SortedList(IComparer<T> comparer)
			: this() {
			Guard.ArgumentNotNull(comparer, "comparer");
			this.comparer = comparer;
		}
		public T this[int index] { get { return innerList[index]; } }
		public int Count { get { return innerList.Count; } }
		#region First
		public T First {
			[DebuggerStepThrough]
			get {
				if (Count <= 0)
					return default(T);
				else
					return innerList[0];
			}
		}
		#endregion
		#region Last
		public T Last {
			[DebuggerStepThrough]
			get {
				if (Count <= 0)
					return default(T);
				else
					return innerList[Count - 1];
			}
		}
		#endregion
		public virtual void Add(T value) {
			int index = BinarySearch(value);
			if (index >= 0)
				return;
			Insert(~index, value);
		}
		public void Sort() {
			this.innerList.Sort(this.comparer);
		}
		public void Insert(int index, T value) {
			this.innerList.Insert(index, value);
		}
		public virtual bool Contains(T value) {
			return BinarySearch(value) >= 0;
		}
		public int BinarySearch(T value) {
			return this.innerList.BinarySearch(value, this.comparer);
		}
		public int BinarySearch(IComparable<T> predicate) {
			return DevExpress.Utils.Algorithms.BinarySearch(this.innerList, predicate);
		}
		public void RemoveAt(int index) {
			this.innerList.RemoveAt(index);
		}
		public virtual void Remove(T value) {
			int index = BinarySearch(value);
			if (index >= 0)
				this.RemoveAt(index);
		}
		public virtual void RemoveFrom(T value) {
			int startIndex = BinarySearch(value);
			if (startIndex < 0)
				startIndex = ~startIndex;
			for (int i = this.innerList.Count - 1; i >= startIndex; i--)
				this.RemoveAt(i);
		}
		public virtual void Clear() {
			this.innerList.Clear();
		}
		public virtual SortedList<T> Clone() {
			SortedList<T> result = CreateEmptyList();
			CopyCore(result);
			return result;
		}
		protected virtual void CopyCore(SortedList<T> destination) {
			destination.innerList.AddRange(innerList);
		}
		protected virtual SortedList<T> CreateEmptyList() {
			return new SortedList<T>();
		}
	}
	#endregion
	public class SequentialBusyIntervals : SortedList<BusyIntervalCollection> {
		class BusyIntervalCollectionComparer : IComparer<BusyIntervalCollection> {
			public int Compare(BusyIntervalCollection x, BusyIntervalCollection y) {
				if (x.DateTime > y.DateTime)
					return 1;
				else if (x.DateTime < y.DateTime)
					return -1;
				return 0;
			}
		}
		public SequentialBusyIntervals()
			: base(new BusyIntervalCollectionComparer()) {
		}
		public int AddCell(DateTime dateTime) {
			BusyIntervalCollection busyIntervalCollection = new BusyIntervalCollection() { DateTime = dateTime };
			int index = BinarySearch(busyIntervalCollection);
			if (index >= 0)
				return index;
			Add(busyIntervalCollection);
			Sort();
			index = BinarySearch(busyIntervalCollection);
			if (index > 0)
				this[index].Assign(this[index - 1]);
			return BinarySearch(busyIntervalCollection);
		}
	}
	public delegate double RealizeAppointmentControl(IAppointmentIntermediateLayoutViewInfo viewInfo);
	public class RelativePositionInfo {
		readonly int from;
		readonly int to;
		readonly double relativePosition;
		double height;
		public RelativePositionInfo(int from, int to, double relativePosition) {
			this.from = from;
			this.to = to;
			this.relativePosition = relativePosition;
		}
		public int From { get { return from; } }
		public int To { get { return to; } }
		public double RelativePosition { get { return relativePosition; } }
		public double Height { get { return height; } set { height = value; } }
	}
	#region SequentialHorizontalAppointmentRelativePositionCalculator
	public class SequentialHorizontalAppointmentRelativePositionCalculator {
		const double DefaultAvailableHeight = double.PositiveInfinity;
		int nextAvailableIndex;
		SequentialBusyIntervals busyIntervals = new SequentialBusyIntervals();
		public SequentialHorizontalAppointmentRelativePositionCalculator() {
			nextAvailableIndex = 1;
		}
		protected internal int NextAvailableIndex { get { return nextAvailableIndex; } set { nextAvailableIndex = value; } }
		public SequentialBusyIntervals BusyIntervals { get { return busyIntervals; } }
		public RelativePositionInfo CalculateRelativePosition(TimeInterval appointmentViewInfoInterval) {
			return CalculateRelativePosition(appointmentViewInfoInterval, 0);
		}
		public RelativePositionInfo CalculateRelativePosition(TimeInterval appointmentViewInfoInterval, double height) {
			int from = GetBusyIndex(appointmentViewInfoInterval.Start);
			int to = GetBusyIndex(appointmentViewInfoInterval.End) - 1;
			double relativePosition = CalculateRelativePositionCore(from, to, 0, height);
			RelativePositionInfo result = new RelativePositionInfo(from, to, relativePosition);
			result.Height = height;
			return result;
		}
		public RelativePositionInfo RecalculateRelativePositionWithKnownHeight(RelativePositionInfo positionInfo, IAppointmentIntermediateLayoutViewInfo viewInfo) {
			int from = positionInfo.From;
			int to = positionInfo.To;
			double relativePosition = CalculateRelativePositionCore(positionInfo.From, positionInfo.To, positionInfo.RelativePosition, positionInfo.Height);
			RelativePositionInfo result = new RelativePositionInfo(from, to, relativePosition);
			result.Height = positionInfo.Height;
			return result;
		}
		protected virtual double CalculateRelativePositionCore(int from, int to, double relativePosition, double height) {
			int i = from;
			while (i <= to) {
				BusyIntervalCollection busyIntervals = BusyIntervals[i];
				BusyInterval interval = FindPossibleIntersectionInterval(busyIntervals, relativePosition);
				if ((interval == BusyInterval.Empty) || (interval.Start >= relativePosition + height))
					i++;
				else {
					relativePosition = interval.End;
					i = from;
				}
			}
			return relativePosition;
		}
		public void MakeIntervalBusy(RelativePositionInfo positionInfo) {
			double top = positionInfo.RelativePosition;
			double bottom = positionInfo.RelativePosition + positionInfo.Height;
			for (int i = positionInfo.From; i <= positionInfo.To; i++)
				AddBusyInterval(busyIntervals[i], new BusyInterval(top, bottom));
		}
		BusyInterval FindPossibleIntersectionInterval(BusyIntervalCollection busyIntervals, double value) {
			for (int i = 0; i < busyIntervals.Count; i++) {
				BusyInterval interval = busyIntervals[i];
				if ((interval.ContainsExcludeEndBound(value)) || (interval.Start >= value))
					return new BusyInterval(interval.Start, interval.End);
			}
			return BusyInterval.Empty;
		}
		protected internal virtual void AddBusyInterval(BusyIntervalCollection busyIntervals, BusyInterval busyInterval) {
			int count = busyIntervals.Count;
			int i = 0;
			while (i < count) {
				if (busyIntervals[i].Start > busyInterval.Start)
					break;
				i++;
			}
			busyIntervals.Insert(i, busyInterval);
		}
		int GetBusyIndex(DateTime dateTime) {
			int index = BusyIntervals.AddCell(dateTime);
			return index;
		}
	}
	#endregion
	public class BusyIntervalInfo {
		public BusyIntervalInfo() {
			BusyInterval = new BusyIntervalCollection();
		}
		public int CellIndex { get; set; }
		DateTime DateTime { get; set; }
		BusyIntervalCollection BusyInterval { get; set; }
	}
	public abstract class PanelConstraintCalculator {
		readonly ICellInfoProvider cellsInfo;
		protected PanelConstraintCalculator(ICellInfoProvider cellsInfo) {
			this.cellsInfo = cellsInfo;
		}
		public ICellInfoProvider CellsInfo { get { return cellsInfo; } }
		public abstract double GetEstimatedCellContentHeight(Size availableSize, int firstCellIndex, int lastCellIndex);
		public abstract double GetMaxCellContentHeight(Size availableSize, int firstCellIndex, int lastCellIndex);
		public abstract CalculateAppointmentPositionResult IsAppointmentFitToCells(double estimatedCellContentHeight, double position, AppointmentControl appointmentControl);
		public virtual double GetMoreButtonAreaHeight(double estimatedCellContentHeight) {
			return estimatedCellContentHeight;
		}
	}
	public class ResizableAppointmentPanelConstraintCalculator : PanelConstraintCalculator {
		public ResizableAppointmentPanelConstraintCalculator(ICellInfoProvider cellsInfo)
			: base(cellsInfo) {
		}
		public override double GetEstimatedCellContentHeight(Size availableSize, int firstCellIndex, int lastCellIndex) {
			return availableSize.Height;
		}
		public override double GetMaxCellContentHeight(Size availableSize, int firstCellIndex, int lastCellIndex) {
			return availableSize.Height;
		}
		public override CalculateAppointmentPositionResult IsAppointmentFitToCells(double estimatedCellContentHeight, double position, AppointmentControl appointmentControl) {
			if (position < estimatedCellContentHeight)
				return CalculateAppointmentPositionResult.AppointmentFitted;
			return CalculateAppointmentPositionResult.AppointmentNotFitted;
		}
	}
	public class TimelineResizableAppointmentPanelConstraintCalculator : ResizableAppointmentPanelConstraintCalculator {
		Size viewportSize;
		public TimelineResizableAppointmentPanelConstraintCalculator(Size viewportSize, ICellInfoProvider cellsInfo)
			: base(cellsInfo) {
				this.viewportSize = viewportSize;
		}
		public override double GetMoreButtonAreaHeight(double estimatedCellContentHeight) {
			return viewportSize.Height;
		}
	}
	public class NotResizableAppointmentPanelConstraintCalculator : PanelConstraintCalculator {
		public NotResizableAppointmentPanelConstraintCalculator(ICellInfoProvider cellsInfo)
			: base(cellsInfo) {
		}
		public override double GetEstimatedCellContentHeight(Size availableSize, int firstCellIndex, int lastCellIndex) {
			Rect firstCellRect = CellsInfo.GetCellRectByIndex(firstCellIndex);
			Rect lastCellRect = CellsInfo.GetCellRectByIndex(lastCellIndex);
			double cellContentHeight = Math.Max(firstCellRect.Height, lastCellRect.Height);
			return cellContentHeight;
		}
		public override double GetMaxCellContentHeight(Size availableSize, int firstCellIndex, int lastCellIndex) {
			Rect firstCellRect = CellsInfo.GetCellRectByIndex(firstCellIndex);
			Rect lastCellRect = CellsInfo.GetCellRectByIndex(lastCellIndex);
			double cellContentHeight = Math.Max(firstCellRect.Height, lastCellRect.Height);
			return cellContentHeight;
		}
		public override CalculateAppointmentPositionResult IsAppointmentFitToCells(double estimatedCellContentHeight, double position, AppointmentControl appointmentControl) {
			int fitCount = 0;
			for (int i = appointmentControl.IntermediateViewInfo.FirstCellIndex; i <= appointmentControl.IntermediateViewInfo.LastCellIndex; i++) {
				double cellHeight = CellsInfo.GetCellRectByIndex(i).Height;
				bool isAptFitted = position <= cellHeight;
				bool prevCellsFitted = fitCount > 0;
				if (!isAptFitted) {
					if (appointmentControl.IntermediateViewInfo.FirstCellIndex < appointmentControl.IntermediateViewInfo.LastCellIndex && i == appointmentControl.IntermediateViewInfo.LastCellIndex && prevCellsFitted)
						return CalculateAppointmentPositionResult.AppointmentPartialFitted;
					return CalculateAppointmentPositionResult.AppointmentNotFitted;
				}
				fitCount++;
			}
			return CalculateAppointmentPositionResult.AppointmentFitted;
		}
	}
	#region VisualAppointmentIntermediateLayout
	public class VisualAppointmentIntermediateLayout {
		public VisualAppointmentIntermediateLayout(HorizontalAppointmentLayoutInfo layoutInfo, VisualAppointmentControl appointment) {
			Guard.ArgumentNotNull(layoutInfo, "layoutInfo");
			Guard.ArgumentNotNull(appointment, "appointment");
			VisualAppointmentControl = appointment;
			LayoutInfo = layoutInfo;
			IsMoreButtonInLastCell = false;
		}
		public VisualAppointmentControl VisualAppointmentControl { get; private set; }
		public HorizontalAppointmentLayoutInfo LayoutInfo { get; private set; }
		public bool IsMoreButtonInLastCell { get; set; }
	}
	#endregion
	public class HorizontalAppointmentsPanelMoreButtonHelper {
		IHorizontalAppointmentPanel appointmentPanel;
		public HorizontalAppointmentsPanelMoreButtonHelper(IHorizontalAppointmentPanel appointmentPanel) {
			this.appointmentPanel = appointmentPanel;
		}
		public MoreButtonCalculatorBase CreateMoreButtonCalculator() {
			return new WeekViewMoreButtonInfoCalculator(this.appointmentPanel.TimeCellControl, this.appointmentPanel.CellsInfo);
		}
		public void ApplyNavigationButtons(MoreButtonCalculatorBase moreButtonCalculator) {
			WeekViewMoreButtonInfoCalculator weekMoreButtonCalculator = moreButtonCalculator as WeekViewMoreButtonInfoCalculator;
			if (weekMoreButtonCalculator != null) {
				IList<WeekViewMoreButtonInfo> moreButtonInfos = weekMoreButtonCalculator.GetInfoCollection();
				this.appointmentPanel.MoreButtonInfos.CopyFrom(moreButtonInfos);
			}
		}
	}
	public class HorizontalAppointmentsPanelFixedHeightLayoutStrategy : AppointmentsLayoutStrategyBase {
		HorizontalAppointmentsPanelMoreButtonHelper moreButtonHelper;
		public HorizontalAppointmentsPanelFixedHeightLayoutStrategy(IHorizontalAppointmentPanel appointmentPanel)
			: base(appointmentPanel) {
			this.moreButtonHelper = new HorizontalAppointmentsPanelMoreButtonHelper(appointmentPanel);
		}
		protected override MoreButtonCalculatorBase CreateMoreButtonCalculator() {
			return this.moreButtonHelper.CreateMoreButtonCalculator();
		}
		protected override void AfterCalculateLayout(SequentialHorizontalAppointmentRelativePositionCalculator relativePositionCalculator, MoreButtonCalculatorBase moreButtonCalculator) {
			this.moreButtonHelper.ApplyNavigationButtons(moreButtonCalculator);
			base.AfterCalculateLayout(relativePositionCalculator, moreButtonCalculator);
		}
	}
	public class HorizontalAppointmentsPanelAutoHeightLayoutStrategy : AppointmentsAutoHeightLayoutStrategyBase {
		HorizontalAppointmentsPanelMoreButtonHelper moreButtonHelper;
		public HorizontalAppointmentsPanelAutoHeightLayoutStrategy(IHorizontalAppointmentPanel appointmentPanel)
			: base(appointmentPanel) {
			this.moreButtonHelper = new HorizontalAppointmentsPanelMoreButtonHelper(appointmentPanel);
		}
		protected override MoreButtonCalculatorBase CreateMoreButtonCalculator() {
			return this.moreButtonHelper.CreateMoreButtonCalculator();
		}
		protected override void AfterCalculateLayout(SequentialHorizontalAppointmentRelativePositionCalculator relativePositionCalculator, MoreButtonCalculatorBase moreButtonCalculator) {
			this.moreButtonHelper.ApplyNavigationButtons(moreButtonCalculator);
			base.AfterCalculateLayout(relativePositionCalculator, moreButtonCalculator);
		}
	}
	public class HorizontalAppointmentsPanelEqualHeightLayoutStrategy : AppointmentsEqualHeightLayoutStrategyBase {
		HorizontalAppointmentsPanelMoreButtonHelper moreButtonHelper;
		public HorizontalAppointmentsPanelEqualHeightLayoutStrategy(IHorizontalAppointmentPanel appointmentPanel)
			: base(appointmentPanel) {
			this.moreButtonHelper = new HorizontalAppointmentsPanelMoreButtonHelper(appointmentPanel);
		}
		protected override MoreButtonCalculatorBase CreateMoreButtonCalculator() {
			return this.moreButtonHelper.CreateMoreButtonCalculator();
		}
		protected override void AfterCalculateLayout(SequentialHorizontalAppointmentRelativePositionCalculator relativePositionCalculator, MoreButtonCalculatorBase moreButtonCalculator) {
			this.moreButtonHelper.ApplyNavigationButtons(moreButtonCalculator);
			base.AfterCalculateLayout(relativePositionCalculator, moreButtonCalculator);
		}
	}
	public abstract class AppointmentsAutoHeightLayoutStrategyBase : AppointmentsLayoutStrategyBase {
		protected AppointmentsAutoHeightLayoutStrategyBase(IHorizontalAppointmentLayoutOwner appointmentPanel)
			: base(appointmentPanel) {
		}
		bool ValidateAppointmentHeight(RelativePositionInfo relativePositionInfo, HorizontalAppointmentLayoutInfo layoutInfo, VisualAppointmentControl visualAppointmentControl) {
			double actualAppointmentHeight = CalculateAvailableAppoimenmentMaxHeight(relativePositionInfo.RelativePosition, layoutInfo.Height, visualAppointmentControl);
			if (layoutInfo.Height != actualAppointmentHeight) {
				visualAppointmentControl.InvalidateLayout();
				relativePositionInfo.Height = layoutInfo.Height = actualAppointmentHeight;
			}
			return actualAppointmentHeight == GetAppointmentMinHeight();
		}
		protected override VisualAppointmentIntermediateLayout CalculateLayoutCore(AppointmentControl appointmentControl, IVisualElementGeneratorCore<VisualAppointmentControl, AppointmentControl> appointmentGenerator, SequentialHorizontalAppointmentRelativePositionCalculator relativePositionCalculator, double estimatedCellContentHeight, RelativePositionInfo relativePositionInfo) {
			VisualAppointmentControl visualAppointmentControl = appointmentGenerator.GenerateNext(appointmentControl);
			ResetAppointmentLayout(visualAppointmentControl);
			HorizontalAppointmentLayoutInfo layoutInfo = CalculateAppointmentLayoutInfo(visualAppointmentControl);
			layoutInfo.RelativePosition = relativePositionInfo.RelativePosition;
			visualAppointmentControl.IntermediateLayoutViewInfo = layoutInfo;
			relativePositionInfo.Height = layoutInfo.Height;
			if (!ValidateAppointmentHeight(relativePositionInfo, layoutInfo, visualAppointmentControl)) {
				relativePositionInfo = relativePositionCalculator.RecalculateRelativePositionWithKnownHeight(relativePositionInfo, appointmentControl.IntermediateViewInfo);
				layoutInfo.RelativePosition = relativePositionInfo.RelativePosition;
				ValidateAppointmentHeight(relativePositionInfo, layoutInfo, visualAppointmentControl);
			}
			CalculateAppointmentPositionResult isFit = ConstraintCalculator.IsAppointmentFitToCells(estimatedCellContentHeight, relativePositionInfo.RelativePosition + relativePositionInfo.Height, appointmentControl);
			if (isFit == CalculateAppointmentPositionResult.AppointmentNotFitted) {
				appointmentGenerator.MoveBack();
				return null;
			}
			VisualAppointmentIntermediateLayout resultLayout = new VisualAppointmentIntermediateLayout(layoutInfo, visualAppointmentControl);
			if (isFit == CalculateAppointmentPositionResult.AppointmentPartialFitted) {
				visualAppointmentControl.LayoutViewInfo.BeginUpdate();
				visualAppointmentControl.LayoutViewInfo.LastCellIndex--;
				visualAppointmentControl.LayoutViewInfo.CancelUpdate();
				if (visualAppointmentControl.ViewInfo.HasRightBorder != false)
					visualAppointmentControl.IsLayoutValid = false;
				visualAppointmentControl.ViewInfo.HasRightBorder = false;
				DateTime endAppointmentViewInfoDate = LayoutOwner.GetCellIntervalByIndex(visualAppointmentControl.LayoutViewInfo.LastCellIndex).End;
				relativePositionInfo = relativePositionCalculator.CalculateRelativePosition(new TimeInterval(appointmentControl.IntermediateViewInfo.Interval.Start, endAppointmentViewInfoDate), layoutInfo.Height);
				resultLayout.IsMoreButtonInLastCell = true;
			}
			relativePositionCalculator.MakeIntervalBusy(relativePositionInfo);
			return resultLayout;
		}
		protected override void MeasureAppointment(VisualAppointmentControl visualAppointment, double availableHeight) {
			double width = GetAppointmentActualWidth(visualAppointment);
			if (visualAppointment.IsLayoutValid )
				return;
			visualAppointment.Measure(new Size(Math.Max(width, GetAppointmentMinWidth()), availableHeight));
			visualAppointment.IsLayoutValid = true;
		}
		void ResetAppointmentLayout(VisualAppointmentControl visualAppointmentControl) {
			if (!visualAppointmentControl.IsLayoutValid) {
				double width = Math.Max(GetAppointmentWidth(visualAppointmentControl), GetAppointmentMinWidth());
				double height = GetAppointmentMinHeight();
				visualAppointmentControl.Measure(new Size(width, height));
				visualAppointmentControl.IsLayoutValid = false;
			}
			visualAppointmentControl.InvalidateLayout();
		}
		protected virtual double CalculateAvailableAppoimenmentMaxHeight(double position, double appointmentHeight, VisualAppointmentControl visualAppointmentControl) {
			double maxCellHeight = CalculateMaxAppointmentHeight(visualAppointmentControl.LayoutViewInfo.FirstCellIndex, visualAppointmentControl.LayoutViewInfo.LastCellIndex);
			double bottomPosition = position + appointmentHeight;
			if (bottomPosition < maxCellHeight)
				return appointmentHeight;
			double heightDelta = bottomPosition - maxCellHeight;
			return Math.Max(appointmentHeight - heightDelta, GetAppointmentMinHeight());
		}
		protected override double GetAppointmentControlHeight(VisualAppointmentControl appointmentControl) {
			if (!appointmentControl.IsLayoutValid) {
				appointmentControl.InvalidateLayout();
				MeasureAppointment(appointmentControl, GetAppointmentMinHeight() + CalculateMaxAppointmentHeight(appointmentControl.LayoutViewInfo.FirstCellIndex, appointmentControl.LayoutViewInfo.LastCellIndex));
			}
			double actualSize = appointmentControl.DesiredSize.Height;
			return (actualSize <= 0) ? GetAppointmentMinHeight() : actualSize;
		}
		double CalculateMaxAppointmentHeight(int firstCellIndx, int lastCellIndx) {
			double maxHeight = 0;
			ICellInfoProvider cellProvider = LayoutOwner.CellsInfo;
			for (int i = firstCellIndx; i <= lastCellIndx; i++)
				maxHeight = Math.Max(cellProvider.GetCellRectByIndex(i).Height, maxHeight);
			return maxHeight;
		}
	}
	public abstract class AppointmentsEqualHeightLayoutStrategyBase : AppointmentsLayoutStrategyBase {
		const double FakeDraggedAppointmentHeight = 1;
		int maxAppointmentLevels = 0;
		double cellHeight = 0;
		protected AppointmentsEqualHeightLayoutStrategyBase(IHorizontalAppointmentLayoutOwner appointmentPanel)
			: base(appointmentPanel) {
		}
		protected override double GetAppointmentControlHeight(VisualAppointmentControl appointmentControl) {
			return FakeDraggedAppointmentHeight;
		}
		protected override void AfterCalculateLayout(SequentialHorizontalAppointmentRelativePositionCalculator relativePositionCalculator, MoreButtonCalculatorBase moreButtonCalculator) {
			base.AfterCalculateLayout(relativePositionCalculator, moreButtonCalculator);
			this.maxAppointmentLevels = 0;
			SequentialBusyIntervals busyIntervals = relativePositionCalculator.BusyIntervals;
			for (int i = 0; i < busyIntervals.Count; i++) {
				BusyIntervalCollection busyInterval = busyIntervals[i];
				this.maxAppointmentLevels = Math.Max(busyInterval.Count, this.maxAppointmentLevels);
			}
		}
		public override void BeginArrangeAppointments(IChildAccessor<VisualAppointmentControl> childAccessor) {
			int firstCellIndex = int.MaxValue;
			int lastCellIndex = int.MinValue;
			int count = childAccessor.Count;
			for (int i = 0; i < count; i++) {
				VisualAppointmentControl appointmentControl = childAccessor[i];
				HorizontalAppointmentLayoutInfo layoutInfo = appointmentControl.IntermediateLayoutViewInfo as HorizontalAppointmentLayoutInfo;
				if (layoutInfo == null)
					continue;
				firstCellIndex = Math.Min(layoutInfo.FirstCellIndex, firstCellIndex);
				lastCellIndex = Math.Max(layoutInfo.LastCellIndex, lastCellIndex);
			}
			double height = double.MaxValue;
			lastCellIndex = Math.Min(lastCellIndex, LayoutOwner.CellsInfo.GetCellCount() - 1);
			for (int i = firstCellIndex; i <= lastCellIndex; i++) {
				height = Math.Min(height, LayoutOwner.CellsInfo.GetCellRectByIndex(i).Height);
			}
			this.cellHeight = height;
		}
		public override Rect CalculateAppointmentRect(VisualAppointmentControl appointmentControl) {
			Rect rect = new Rect();
			double appointmentLeft = GetAppointmentLeft(appointmentControl);
			double appointmentWidth = GetAppointmentRight(appointmentControl) - appointmentLeft;
			rect.X = appointmentLeft;
			double relativePosition = GetRelativePosition(appointmentControl);
			double c = 1.0 / this.maxAppointmentLevels;
			rect.Y = GetContentTop(appointmentControl.LayoutViewInfo.FirstCellIndex) + relativePosition * this.cellHeight * c;
			rect.Width = appointmentWidth;
			rect.Height = this.cellHeight / this.maxAppointmentLevels;
			return rect;
		}
	}
	public abstract class AppointmentsLayoutStrategyBase {
		const int DefaultStatusHeight = 5;
		const int DefaultAppointmentWidth = 23;
		#region Fields
		readonly IHorizontalAppointmentLayoutOwner layoutOwner;
		PanelConstraintCalculator constraintCalculator;
		#endregion
		protected AppointmentsLayoutStrategyBase(IHorizontalAppointmentLayoutOwner layoutOwner) {
			Guard.ArgumentNotNull(layoutOwner, "appointmentPanel");
			this.layoutOwner = layoutOwner;
		}
		#region Properties
		public IHorizontalAppointmentLayoutOwner LayoutOwner { get { return layoutOwner; } }
		public PanelConstraintCalculator ConstraintCalculator {
			get {
				if (constraintCalculator == null)
					constraintCalculator = CreatePanelConstraintCalculator(LayoutOwner.IsResizable, LayoutOwner.CellsInfo);
				return constraintCalculator;
			}
		}
		#endregion
		public virtual void CalculateLayout(IList<AppointmentControl> appointmentControls, Size availableSize, IVisualElementGeneratorCore<VisualAppointmentControl, AppointmentControl> appointmentGenerator) {
			SequentialHorizontalAppointmentRelativePositionCalculator relativePositionCalculator = CreateRelativePositionsCalculator();
			MoreButtonCalculatorBase moreButtonCalculator = CreateMoreButtonCalculator();
			foreach (AppointmentControl appointmentControl in appointmentControls) {
				AppointmentIntermediateViewInfoCore intermediateViewInfo = appointmentControl.IntermediateViewInfo;
				double estimatedCellContentHeight = ConstraintCalculator.GetEstimatedCellContentHeight(availableSize, intermediateViewInfo.FirstCellIndex, intermediateViewInfo.LastCellIndex);
				double moreButtonAreaHeight = ConstraintCalculator.GetMoreButtonAreaHeight(estimatedCellContentHeight);
				if (estimatedCellContentHeight <= 0)
					continue;
				RelativePositionInfo relativePositionInfo = relativePositionCalculator.CalculateRelativePosition(intermediateViewInfo.Interval, GetAppointmentMinHeight());
				if (relativePositionInfo.RelativePosition >= moreButtonAreaHeight) 
					moreButtonCalculator.Calculate(intermediateViewInfo);
				if (relativePositionInfo.RelativePosition >= estimatedCellContentHeight) 
					continue;
				VisualAppointmentIntermediateLayout intermediateLayout = CalculateLayoutCore(appointmentControl, appointmentGenerator, relativePositionCalculator, estimatedCellContentHeight, relativePositionInfo);
				if (intermediateLayout == null) {
					moreButtonCalculator.Calculate(intermediateViewInfo);
					continue;
				}
				if (intermediateLayout.LayoutInfo.RelativePosition + intermediateLayout.LayoutInfo.Height >= moreButtonAreaHeight) {
					moreButtonCalculator.Calculate(intermediateViewInfo);
				}
				if (intermediateLayout.IsMoreButtonInLastCell)
					moreButtonCalculator.Calculate(intermediateViewInfo.LastCellIndex);
				MeasureAppointment(intermediateLayout.VisualAppointmentControl, intermediateLayout.LayoutInfo.Height);
			}
			AfterCalculateLayout(relativePositionCalculator, moreButtonCalculator);
		}
		protected abstract MoreButtonCalculatorBase CreateMoreButtonCalculator();
		protected virtual VisualAppointmentIntermediateLayout CalculateLayoutCore(AppointmentControl appointmentControl, IVisualElementGeneratorCore<VisualAppointmentControl, AppointmentControl> appointmentGenerator, SequentialHorizontalAppointmentRelativePositionCalculator relativePositionCalculator, double estimatedCellContentHeight, RelativePositionInfo relativePositionInfo) {
			CalculateAppointmentPositionResult isFit = ConstraintCalculator.IsAppointmentFitToCells(estimatedCellContentHeight, relativePositionInfo.RelativePosition + relativePositionInfo.Height, appointmentControl);
			if (isFit == CalculateAppointmentPositionResult.AppointmentNotFitted) {
				relativePositionCalculator.MakeIntervalBusy(relativePositionInfo);
				return null;
			}
			VisualAppointmentControl visualAppointmentControl = appointmentGenerator.GenerateNext(appointmentControl);
			HorizontalAppointmentLayoutInfo layoutInfo = CalculateAppointmentLayoutInfo(visualAppointmentControl);
			layoutInfo.RelativePosition = relativePositionInfo.RelativePosition;
			visualAppointmentControl.IntermediateLayoutViewInfo = layoutInfo;
			relativePositionInfo.Height = layoutInfo.Height;
			VisualAppointmentIntermediateLayout resultLayout = new VisualAppointmentIntermediateLayout(layoutInfo, visualAppointmentControl);
			if (isFit == CalculateAppointmentPositionResult.AppointmentPartialFitted) {
				visualAppointmentControl.LayoutViewInfo.LastCellIndex--;
				DateTime endAppointmentViewInfoDate = LayoutOwner.GetCellIntervalByIndex(visualAppointmentControl.LayoutViewInfo.LastCellIndex).End;
				if (visualAppointmentControl.ViewInfo.HasRightBorder != false)
					visualAppointmentControl.IsLayoutValid = false;
				visualAppointmentControl.ViewInfo.HasRightBorder = false;
				relativePositionInfo = relativePositionCalculator.CalculateRelativePosition(new TimeInterval(appointmentControl.IntermediateViewInfo.Interval.Start, endAppointmentViewInfoDate), layoutInfo.Height);
				resultLayout.IsMoreButtonInLastCell = true;
			}
			relativePositionCalculator.MakeIntervalBusy(relativePositionInfo);
			return resultLayout;
		}
		protected virtual void AfterCalculateLayout(SequentialHorizontalAppointmentRelativePositionCalculator relativePositionCalculator, MoreButtonCalculatorBase moreButtonCalculator) {
		}
		public virtual void BeginArrangeAppointments(IChildAccessor<VisualAppointmentControl> childAccessor) {
		}
		public virtual Rect CalculateAppointmentRect(VisualAppointmentControl appointmentControl) {
			Rect rect = new Rect();
			double appointmentLeft = GetAppointmentLeft(appointmentControl);
			double appointmentWidth = GetAppointmentRight(appointmentControl) - appointmentLeft;
			VisualHorizontalAppointmentControl horizontalAppointment = appointmentControl as VisualHorizontalAppointmentControl;
			if (horizontalAppointment == null)
				return rect;
			double appointmentTop = GetAppointmentTop(horizontalAppointment);
			double appointmentHeight = GetAppointmentHeight(horizontalAppointment);
			rect.X = appointmentLeft;
			rect.Y = appointmentTop;
			rect.Height = appointmentHeight;
			rect.Width = appointmentWidth;
			return rect;
		}
		protected internal double GetAppointmentHeight(VisualAppointmentControl appointmentControl) {
			HorizontalAppointmentLayoutInfo layoutInfo = appointmentControl.IntermediateLayoutViewInfo as HorizontalAppointmentLayoutInfo;
			if (layoutInfo == null)
				return 0;
			return layoutInfo.Height;
		}
		protected internal double GetContentTop(int cellIndex) {
			return LayoutOwner.CellsInfo.GetCellRectByIndex(cellIndex).Top ;
		}
		double GetAppointmentTop(VisualHorizontalAppointmentControl appointmentControl) {
			return GetContentTop(appointmentControl.LayoutViewInfo.FirstCellIndex) + GetRelativePosition(appointmentControl);
		}
		protected virtual SequentialHorizontalAppointmentRelativePositionCalculator CreateRelativePositionsCalculator() {
			return new SequentialHorizontalAppointmentRelativePositionCalculator();
		}
		protected virtual double GetAppointmentMinHeight() {
			return LayoutOwner.GetDefaultAppointmentHeight() + (LayoutOwner.IsStatusVisible ? DefaultStatusHeight : 0);
		}
		protected virtual double GetAppointmentMinWidth() {
			return DefaultAppointmentWidth;
		}
		protected HorizontalAppointmentLayoutInfo CalculateAppointmentLayoutInfo(VisualAppointmentControl appointmentControl) {
			HorizontalAppointmentLayoutInfo result = new HorizontalAppointmentLayoutInfo();
			result.Appointment = appointmentControl.GetAppointment();
			result.Height = GetAppointmentControlHeight(appointmentControl);
			result.FirstCellIndex = appointmentControl.LayoutViewInfo.FirstCellIndex;
			result.FirstIndexPosition = appointmentControl.LayoutViewInfo.FirstIndexPosition;
			result.CellIndexes = new AppointmentCellIndexes(result.FirstCellIndex, result.LastCellIndex);
			result.LastIndexPosition = appointmentControl.LayoutViewInfo.LastIndexPosition;
			result.MaxIndexInGroup = appointmentControl.LayoutViewInfo.MaxIndexInGroup;
			result.Visible = true;
			if (ShouldExtendAppointmentBounds(appointmentControl))
				CalculateExtendedAppointmentProperties(result, appointmentControl);
			else {
				result.LastCellIndex = appointmentControl.LayoutViewInfo.LastCellIndex;
				result.Interval = new TimeInterval(appointmentControl.ViewInfo.IntervalStart, appointmentControl.ViewInfo.IntervalEnd);
			}
			return result;
		}
		protected virtual double GetAppointmentControlHeight(VisualAppointmentControl appointmentControl) {
			return GetAppointmentMinHeight();
		}
		bool ShouldExtendAppointmentBounds(VisualAppointmentControl appointmentControl) {
			if (LayoutOwner.SnapToCells == AppointmentSnapToCellsMode.Never)
				return GetAppointmentWidth(appointmentControl) < GetAppointmentMinWidth();
			else
				return false;
		}
		protected internal virtual double GetAppointmentWidth(VisualAppointmentControl appointmentControl) {
			return GetAppointmentRight(appointmentControl) - GetAppointmentLeft(appointmentControl);
		}
		protected virtual double GetAppointmentLeft(VisualAppointmentControl control) {
			Rect cellRect = LayoutOwner.CellsInfo.GetCellRectByIndex(control.LayoutViewInfo.FirstCellIndex);
			if (!IsCellRectValid(cellRect))
				return 0;
			return cellRect.Left + control.LayoutViewInfo.StartRelativeOffset / 100.0 * cellRect.Width + LayoutOwner.CellPadding.Left;
		}
		protected virtual double GetAppointmentRight(VisualAppointmentControl control) {
			Rect cellRect = LayoutOwner.CellsInfo.GetCellRectByIndex(control.LayoutViewInfo.LastCellIndex);
			if (!IsCellRectValid(cellRect))
				return 0;
			return cellRect.Right - control.LayoutViewInfo.EndRelativeOffset / 100.0 * cellRect.Width - LayoutOwner.CellPadding.Right;
		}
		bool IsCellRectValid(Rect cellRect) {
			if (cellRect.Width == 0) 
				return false;
			if (XpfSchedulerUtils.IsZeroRect(cellRect))
				return false;
			return true;
		}
		void CalculateExtendedAppointmentProperties(HorizontalAppointmentLayoutInfo layoutInfo, VisualAppointmentControl appointmentControl) {
			int lastCellIndex = CalculateExtendedAppointmentLastIndex(appointmentControl);
			layoutInfo.LastCellIndex = lastCellIndex;
			appointmentControl.LayoutViewInfo.LastCellIndex = lastCellIndex;
			appointmentControl.LayoutViewInfo.EndRelativeOffset = RecalcEndOffset(appointmentControl);
			layoutInfo.Interval = CalculateExtendedAppointmentInterval(appointmentControl);
		}
		protected internal virtual int CalculateExtendedAppointmentLastIndex(VisualAppointmentControl appointmentControl) {
			Rect lastCellBounds = LayoutOwner.CellsInfo.GetCellRectByIndex(appointmentControl.LayoutViewInfo.LastCellIndex);
			double aptRightBound = GetAppointmentLeft(appointmentControl) + appointmentControl.DesiredSize.Width;
			if (aptRightBound < lastCellBounds.Right)
				return appointmentControl.LayoutViewInfo.LastCellIndex;
			return Math.Min(LayoutOwner.CellsInfo.GetCellCount() - 1, appointmentControl.LayoutViewInfo.LastCellIndex + 1);
		}
		protected internal int RecalcEndOffset(VisualAppointmentControl appointmentControl) {
			Rect cellBounds = LayoutOwner.CellsInfo.GetCellRectByIndex(appointmentControl.LayoutViewInfo.LastCellIndex);
			if (cellBounds.Width == 0)
				return 0;
			else {
				double width = GetAppointmentMinHeight();
				int endOffset = (int)((cellBounds.Right - GetAppointmentLeft(appointmentControl) - width) * 100 / cellBounds.Width);
				return Math.Max(0, endOffset);
			}
		}
		protected internal virtual TimeInterval CalculateExtendedAppointmentInterval(VisualAppointmentControl appointmentControl) {
			DateTime start = appointmentControl.ViewInfo.IntervalStart;
			TimeInterval lastCellInterval = LayoutOwner.GetCellIntervalByIndex(appointmentControl.LayoutViewInfo.LastCellIndex);
			DateTime adjustedEnd = AppointmentTimeScaleHelper.CalculateEndTimeByOffset(lastCellInterval, appointmentControl.LayoutViewInfo.EndRelativeOffset);
			DateTime actualEnd = DevExpress.XtraScheduler.Native.DateTimeHelper.Max(start, adjustedEnd);
			return new TimeInterval(start, actualEnd);
		}
		protected virtual void MeasureAppointment(VisualAppointmentControl visualAppointment, double availableHeight) {
			double width = GetAppointmentActualWidth(visualAppointment);
			if ((visualAppointment.IsLayoutValid && (visualAppointment.ActualWidth == width)) || width == 0 )
				return;
			visualAppointment.Measure(new Size(Math.Max(width, GetAppointmentMinWidth()), availableHeight));
			visualAppointment.IsLayoutValid = true;
		}
		protected internal virtual double GetAppointmentActualWidth(VisualAppointmentControl appointmentControl) {
			double width = GetAppointmentWidth(appointmentControl);
			if (ShouldExtendAppointmentBounds(appointmentControl))
				return GetAppointmentMinWidth();
			return width;
		}
		public double GetRelativePosition(VisualAppointmentControl visualAppointmentControl) {
			HorizontalAppointmentLayoutInfo layoutInfo = visualAppointmentControl.IntermediateLayoutViewInfo as HorizontalAppointmentLayoutInfo;
			if (layoutInfo == null)
				return 0;
			return layoutInfo.RelativePosition;
		}
		public virtual Size CalculateDesiredSize(Size availableSize, IChildAccessor<VisualAppointmentControl> childAccessor) {
			DefaultHorizontalAppointmentPanelDesiredSizeCalculator desiredSizeCalculator = CreateDesiredSizeCalculator(LayoutOwner.IsResizable);
			return desiredSizeCalculator.Calculate(availableSize, childAccessor);
		}
		protected virtual DefaultHorizontalAppointmentPanelDesiredSizeCalculator CreateDesiredSizeCalculator(bool isResizable) {
			if (isResizable)
				return new ResizableHorizontalAppointmentPanelDesiredSizeCalculator();
			return new DefaultHorizontalAppointmentPanelDesiredSizeCalculator();
		}
		public virtual PanelConstraintCalculator CreatePanelConstraintCalculator(bool isResizable, ICellInfoProvider cellsInfo) {
			if (isResizable)
				return new ResizableAppointmentPanelConstraintCalculator(cellsInfo);
			return new NotResizableAppointmentPanelConstraintCalculator(cellsInfo);
		}
	}
	public class DefaultHorizontalAppointmentPanelDesiredSizeCalculator {
		public virtual Size Calculate(Size availableSize, IChildAccessor<VisualAppointmentControl> childAccessor) {
			return new Size(0, 0);
		}
	}
	public class ResizableHorizontalAppointmentPanelDesiredSizeCalculator : DefaultHorizontalAppointmentPanelDesiredSizeCalculator {
		public override Size Calculate(Size availableSize, IChildAccessor<VisualAppointmentControl> childAccessor) {
			double height = 0;
			int count = childAccessor.Count;
			for (int i = 0; i < count; i++) {
				VisualAppointmentControl element = childAccessor[i];
				VisualHorizontalAppointmentControl visualAppointment = element as VisualHorizontalAppointmentControl;
				double position = 0;
				if (visualAppointment != null && visualAppointment.Visibility == Visibility.Visible)
					position = visualAppointment.GetRelativePosition();
				height = Math.Max(height, element.DesiredSize.Height + position);
			}
			double width = (Double.IsInfinity(availableSize.Width)) ? 0 : availableSize.Width;
			return new Size(width, Math.Min(height, availableSize.Height));
		}
	}
}
