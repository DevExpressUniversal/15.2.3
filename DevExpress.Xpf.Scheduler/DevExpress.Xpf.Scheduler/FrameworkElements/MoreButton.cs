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
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using System.Windows;
using System;
using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.Utils;
using System.Windows.Input;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using DXContentPresenter = DevExpress.Xpf.Core.DXContentPresenter;
using DevExpress.XtraScheduler.Native;
using System.Collections.Specialized;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region HorizontalWeekMoreButtonPanel
	public class HorizontalWeekMoreButtonPanel : Panel {
		public static readonly DependencyProperty AppointmentsInfoContainerProperty;
		static HorizontalWeekMoreButtonPanel() {
			AppointmentsInfoContainerProperty = DependencyPropertyHelper.RegisterProperty<HorizontalWeekMoreButtonPanel, IAppoinmentsInfoContainer>("AppointmentsInfoContainer", null);
		}
		public IAppoinmentsInfoContainer AppointmentsInfoContainer {
			get { return (IAppoinmentsInfoContainer)GetValue(AppointmentsInfoContainerProperty); }
			set { SetValue(AppointmentsInfoContainerProperty, value); }
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size res = new Size();
			for (int i = 0; i < Children.Count; i++) {
				UIElement elem = Children[i];
				elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				res.Width = Math.Max(res.Width, elem.DesiredSize.Width);
				res.Height = Math.Max(res.Height, elem.DesiredSize.Height);
			}
			if (!double.IsInfinity(availableSize.Width)) res.Width = availableSize.Width;
			if (!double.IsInfinity(availableSize.Height)) res.Height = availableSize.Height;
			return res;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			for (int i = 0; i < Children.Count; i++) {
				ContentPresenter contentPresenter = (ContentPresenter)Children[i];
				if (contentPresenter != null) {
					Rect cellBounds = ((WeekViewMoreButtonInfo)contentPresenter.Content).CellBounds;
					cellBounds = AppointmentsInfoContainer.TranslateRectTo(cellBounds, this);
					contentPresenter.Arrange(cellBounds);
				}
			}
			return finalSize;
		}
	}
	#endregion
	#region MoreButtonInfoComparable
	public class MoreButtonInfoComparable : IComparable<WeekViewMoreButtonInfo> {
		readonly int index;
		public MoreButtonInfoComparable(int index) {
			this.index = index;
		}
		public int CompareTo(WeekViewMoreButtonInfo other) {
			return other.CellIndex - index;
		}
	}
	#endregion
	#region MoreButtonInfoCollection
	public class MoreButtonInfoCollection : ObservableCollection<WeekViewMoreButtonInfo> {  
		public class MoreButtonInfoComparar : IComparable<WeekViewMoreButtonInfo> {
			readonly int index;
			public MoreButtonInfoComparar(int index) {
				this.index = index;
			}
			#region IComparable<WeekViewMoreButtonInfo> Members
			public int CompareTo(WeekViewMoreButtonInfo other) {
				return other.CellIndex - index;
			}
			#endregion
		}
		public bool Contains(int cellIndex) {
			return BinarySearch(cellIndex) >= 0;
		}
		public bool TryGetValue(int cellIndex, out WeekViewMoreButtonInfo buttonInfo) {
			buttonInfo = null;
			int index = BinarySearch(cellIndex);
			if (index >= 0) {
				buttonInfo = this[index];
				return true;
			}
			return false;
		}
		protected internal int BinarySearch(int cellIndex) {
			return DevExpress.Utils.Algorithms.BinarySearch(this, new MoreButtonInfoComparar(cellIndex));
		}
		protected virtual bool CanCopyFrom(IList<WeekViewMoreButtonInfo> source) {
			int sourceCount = source.Count;
			int count = Math.Min(Count, sourceCount);
			for (int i = 0; i < count; i++) {
				if (!this[i].CellBounds.Equals(source[i].CellBounds))
					return false;
			}
			return true;
		}
		public virtual void CopyFrom(IList<WeekViewMoreButtonInfo> source) {
			if (CanCopyFrom(source))
				CopyFromCore(source);
			else {
				Clear();
				int count = source.Count;
				for (int i = 0; i < count; i++)
					Add(source[i]);
			}
		}
		protected virtual void CopyFromCore(IList<WeekViewMoreButtonInfo> source) {
			int sourceCount = source.Count;
			int count = Math.Min(Count, sourceCount);
			for (int i = 0; i < count; i++)
				this[i].CopyFrom(source[i]);
			if (sourceCount < Count) {
				for (int i = Count - 1; i >= sourceCount; i--)
					Remove(this[i]);
			} else if (sourceCount > Count) {
				for (int i = Count; i < sourceCount; i++)
					Add(source[i]);
			}
		}
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	using DevExpress.Xpf.Scheduler.UI;
	using System.Collections.Generic;
	using DevExpress.Xpf.Scheduler.Native;
	using DevExpress.Xpf.Scheduler.Commands;
	#region HorizontalWeekMoreButtonPanel2
	public class HorizontalWeekMoreButtonPanel2 : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			Size res = new Size();
			for (int i = 0; i < Children.Count; i++) {
				ContentPresenter contentPresenter = (ContentPresenter)Children[i];
				contentPresenter.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				res.Width = Math.Max(res.Width, contentPresenter.DesiredSize.Width);
				res.Height = Math.Max(res.Height, contentPresenter.DesiredSize.Height);
			}
			if (!double.IsInfinity(availableSize.Width)) res.Width = availableSize.Width;
			if (!double.IsInfinity(availableSize.Height)) res.Height = availableSize.Height;
			return res;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			for (int i = 0; i < Children.Count; i++) {
				ContentPresenter contentPresenter = (ContentPresenter)Children[i];
				WeekViewMoreButtonInfo info = (WeekViewMoreButtonInfo)contentPresenter.Content;
				Rect cellBounds = info.CellBounds;
				cellBounds = ConvertToActualUnits(cellBounds, finalSize);
				contentPresenter.Arrange(cellBounds);
			}
			return finalSize;
		}
		protected virtual Rect ConvertToActualUnits(Rect bounds, Size availableSize) {
			Rect result = new Rect();
			result.X = bounds.X * availableSize.Width;
			result.Width = bounds.Width * availableSize.Width;
			result.Y = bounds.Y * availableSize.Height;
			result.Height = bounds.Height * availableSize.Height;
			return result;
		}
	}
	#endregion
	#region MoreButtonItemsControl
	public class MoreButtonItemsControl : XPFItemsControl {
		#region View
		public SchedulerViewBase View {
			get { return (SchedulerViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public static readonly DependencyProperty ViewProperty = CreateViewProperty();
		static DependencyProperty CreateViewProperty() {
			return DependencyPropertyHelper.RegisterProperty<MoreButtonItemsControl, SchedulerViewBase>("View", null);
		}
		#endregion
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			WeekViewMoreButtonInfo content = item as WeekViewMoreButtonInfo;
			if (content != null)
				PrepareMoreButtonContent(content);
			base.PrepareContainerForItemOverride(element, item);
		}
		protected virtual void PrepareMoreButtonContent(WeekViewMoreButtonInfo content) {
			content.View = View;
		}
	}
	#endregion
	public abstract class MoreButtonCalculatorBase {
		public virtual void Calculate(AppointmentIntermediateViewInfoCore intermediateViewInfo) {
			for (int i = intermediateViewInfo.FirstCellIndex; i <= intermediateViewInfo.LastCellIndex; i++)
				Calculate(i);
		}
		public abstract void Calculate(int cellIndex);
	}
	public class WeekViewMoreButtonComponentCalculator : MoreButtonCalculatorBase {
		public WeekViewMoreButtonComponentCalculator(AppointmentsLayoutInfo layoutInfo) {
			Guard.ArgumentNotNull(layoutInfo, "layoutInfo");
			LayoutInfo = layoutInfo;
		}
		public AppointmentsLayoutInfo LayoutInfo { get; private set; }
		public override void Calculate(int cellIndex) {
			LayoutInfo.SetFitIntoCell(cellIndex, false);
		}
	}
	#region WeekViewMoreButtonInfoCalculator
	public class WeekViewMoreButtonInfoCalculator : MoreButtonCalculatorBase {
		Dictionary<int, WeekViewMoreButtonInfo> moreButtons;
		readonly ISchedulerTimeCellControl timeCellControl;
		readonly ICellInfoProvider cellsInfo;
		Size cachedTotalSize = Size.Empty;
		public WeekViewMoreButtonInfoCalculator(ISchedulerTimeCellControl timeCellControl, ICellInfoProvider cellsInfo) {
			this.timeCellControl = timeCellControl;
			this.cellsInfo = cellsInfo;
			Reset();
		}
		public ISchedulerTimeCellControl TimeCellControl { get { return timeCellControl; } }
		public ICellInfoProvider CellsInfo { get { return cellsInfo; } }
		public Size TotalSize {
			get {
				if (cachedTotalSize == Size.Empty)
					cachedTotalSize = CalculateTotalCellsSize();
				return cachedTotalSize;
			}
		}
		public override void Calculate(int cellIndex) {
			if (this.moreButtons.ContainsKey(cellIndex))
				return;
			Rect cellBounds = ConvertToRelativeUnits(CellsInfo.GetCellRectByIndex(cellIndex));
			WeekViewMoreButtonInfo info = CreateInfoObject(cellIndex, cellBounds, TimeCellControl.GetCellInterval(cellIndex).Start);
			moreButtons.Add(cellIndex, info);
		}
		protected virtual Rect ConvertToRelativeUnits(Rect bounds) {
			double width = TotalSize.Width;
			double height = TotalSize.Height;
			if (width == 0 || height == 0)
				return bounds;
			return new Rect(bounds.X / width, bounds.Y / height, bounds.Width / width, bounds.Height / height);
		}
		protected virtual WeekViewMoreButtonInfo CreateInfoObject(int cellIndex, Rect cellBounds, DateTime dateTime) {
			return new WeekViewMoreButtonInfo(cellIndex, cellBounds, dateTime);
		}
		public List<WeekViewMoreButtonInfo> GetInfoCollection() {
			return new List<WeekViewMoreButtonInfo>(moreButtons.Values);
		}
		public void Reset() {
			this.moreButtons = new Dictionary<int, WeekViewMoreButtonInfo>();
			this.cachedTotalSize = Size.Empty;
		}
		protected internal virtual Size CalculateTotalCellsSize() {
			Point topLeft = new Point(0, 0);
			Point bottomRight = new Point(0, 0);
			int cellsCount = CellsInfo.GetCellCount();
			for (int i = 0; i < cellsCount; i++) {
				Rect rect = cellsInfo.GetCellRectByIndex(i);
				topLeft.X = Math.Min(topLeft.X, rect.TopLeft().X);
				topLeft.Y = Math.Min(topLeft.Y, rect.TopLeft().Y);
				bottomRight.X = Math.Max(bottomRight.X, rect.BottomRight().X);
				bottomRight.Y = Math.Max(bottomRight.Y, rect.BottomRight().Y);
			}
			Rect resultRect = new Rect(topLeft, bottomRight);
			return resultRect.Size();
		}
	}
	#endregion
	#region WeekViewMoreButtonInfo
	public class WeekViewMoreButtonInfo : DependencyObject, ISupportCopyFrom<WeekViewMoreButtonInfo>, ICommandBasedButtonInfo {
		Rect cellBounds;
		int cellIndex;
		public WeekViewMoreButtonInfo(int cellIndex, Rect cellBounds, DateTime date)
			: base() {
			this.cellIndex = cellIndex;
			this.cellBounds = cellBounds;
			Command = CreateCommand(date);
		}
		#region Properties
		#region View
		public SchedulerViewBase View {
			get { return (SchedulerViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public static readonly DependencyProperty ViewProperty = CreateViewProperty();
		static DependencyProperty CreateViewProperty() {
			return DependencyPropertyHelper.RegisterProperty<WeekViewMoreButtonInfo, SchedulerViewBase>("View", null);
		}
		#endregion
		#region Command
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public static readonly DependencyProperty CommandProperty = CreateCommandProperty();
		static DependencyProperty CreateCommandProperty() {
			return DependencyPropertyHelper.RegisterProperty<WeekViewMoreButtonInfo, ICommand>("Command", null);
		}
		#endregion
		public Rect CellBounds { get { return cellBounds; } }
		public int CellIndex { get { return cellIndex; } }
		#endregion
		#region ICommandBasedButtonInfo Members
		ICommand ICommandBasedButtonInfo.Command { get { return Command; } }
		object ICommandBasedButtonInfo.CommandParameter { get { return View != null ? View.Control : null; } }
		#endregion
		#region ISupportCopyFrom<WeekViewMoreButtonInfo> Members
		public void CopyFrom(WeekViewMoreButtonInfo source) {
			CopyFromCore(source);
		}
		protected virtual void CopyFromCore(WeekViewMoreButtonInfo source) {
			if (View != source.View && source.View != null)
				View = source.View;
			if (Command != source.Command && source.Command != null)
				Command = source.Command;
			cellBounds = source.CellBounds;
			cellIndex = source.CellIndex;
		}
		#endregion
		protected internal virtual ICommand CreateCommand(DateTime date) {
			return new MoreButtonUICommand(date);
		}
	}
	#endregion
	#region VisualDayViewMoreButton
	public class VisualDayViewMoreButton : DependencyObject {
		#region Properties
		#region Visibility
		Visibility lastSetterVisibility = Visibility.Collapsed;
		public Visibility Visibility {
			get { return (Visibility)GetValue(VisibilityProperty); }
			set {
				if (lastSetterVisibility != value)
					SetValue(VisibilityProperty, value);
			}
		}
		public static readonly DependencyProperty VisibilityProperty = CreateVisibilityProperty();
		static DependencyProperty CreateVisibilityProperty() {
			return DependencyPropertyHelper.RegisterProperty<VisualDayViewMoreButton, Visibility>("Visibility", Visibility.Collapsed, (d, e) => d.OnVisibilityChanged(e.NewValue));
		}
		protected internal virtual void OnVisibilityChanged(Visibility visibility) {
			this.lastSetterVisibility = visibility;
		}
		#endregion
		#region ScrollOffset
		double lastSetterScrollOffset = 0;
		public double ScrollOffset {
			get { return (double)GetValue(ScrollOffsetProperty); }
			set {
				if (lastSetterScrollOffset != value)
					SetValue(ScrollOffsetProperty, value);
			}
		}
		public static readonly DependencyProperty ScrollOffsetProperty = CreateScrollOffsetProperty();
		static DependencyProperty CreateScrollOffsetProperty() {
			return DependencyPropertyHelper.RegisterProperty<VisualDayViewMoreButton, double>("ScrollOffset", 0.0, (d, e) => d.OnScrollOffsetChanged(e.NewValue));
		}
		protected internal virtual void OnScrollOffsetChanged(double offset) {
			this.lastSetterScrollOffset = offset;
			Command = new MakeAppointmentVisibleCommand(offset);
		}
		#endregion
		#region Command
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public static readonly DependencyProperty CommandProperty = CreateCommandProperty();
		static DependencyProperty CreateCommandProperty() {
			return DependencyPropertyHelper.RegisterProperty<VisualDayViewMoreButton, ICommand>("Command", null);
		}
		#endregion
		#endregion
	}
	#endregion
	public class MoreButtonViewModel : DependencyObject {
		SchedulerControl scheduler;
		DateTime date;
		Visibility visibility;
		#region Scheduler
		public SchedulerControl Scheduler {
			get { return (SchedulerControl)GetValue(SchedulerProperty); }
			set {
				if (!Object.ReferenceEquals(Scheduler, value))
					SetValue(SchedulerProperty, value);
			}
		}
		public static readonly DependencyProperty SchedulerProperty = DependencyPropertyHelper.RegisterProperty<MoreButtonViewModel, SchedulerControl>("Scheduler", null, (d, e) => d.OnSchedulerChanged(e.OldValue, e.NewValue));
		#endregion
		#region Date
		public DateTime Date {
			get { return (DateTime)GetValue(DateProperty); }
			set {
				if (Date != value)
					SetValue(DateProperty, value);
			}
		}
		public static readonly DependencyProperty DateProperty = DependencyPropertyHelper.RegisterProperty<MoreButtonViewModel, DateTime>("Date", DateTime.MinValue, (d, e) => d.OnDateChanged(e.OldValue, e.NewValue));
		#endregion
		#region Command
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public static readonly DependencyProperty CommandProperty = DependencyPropertyHelper.RegisterProperty<MoreButtonViewModel, ICommand>("Command", null);
		#endregion
		#region Visibility
		public Visibility Visibility {
			get { return (Visibility)GetValue(VisibilityProperty); }
			set {
				if (Visibility != value)
					SetValue(VisibilityProperty, value);
			}
		}
		public static readonly DependencyProperty VisibilityProperty = DependencyPropertyHelper.RegisterProperty<MoreButtonViewModel, Visibility>("Visibility", Visibility.Collapsed, (d, e) => d.OnVisibilityChanged(e.OldValue, e.NewValue));
		#endregion
		void OnSchedulerChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			this.scheduler = newValue;
		}
		void OnVisibilityChanged(Visibility oldValue, Visibility newValue) {
			this.visibility = newValue;
		}
		void OnDateChanged(DateTime oldValue, DateTime newValue) {
			Command = new MoreButtonUICommand(Date);
			this.date = newValue;
		}
	}
	public class MoreButton : SchedulerButton {
		public MoreButton() {
			DefaultStyleKey = typeof(MoreButton);
			LayoutUpdated += OnLayoutUpdated;
		}
		#region ViewModel
		public MoreButtonViewModel ViewModel {
			get { return (MoreButtonViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}
		public static readonly DependencyProperty ViewModelProperty = DependencyPropertyHelper.RegisterProperty<MoreButton, MoreButtonViewModel>("ViewModel", null, (d, e) => d.OnViewModelChanged());
		#endregion
		void OnLayoutUpdated(object sender, EventArgs e) {
			if (Visibility == Visibility.Visible && this.IsNeverMeasured())
				Owner.OnChildrenChanged(this);
		}
		void OnViewModelChanged() {
			if (ViewModel == null)
				return;
			InnerBindingHelper.SetBinding(this, ViewModel, Button.CommandProperty, "Command");
			InnerBindingHelper.SetBinding(this, ViewModel, Button.CommandParameterProperty, "Scheduler");
			InnerBindingHelper.SetBinding(this, ViewModel, Button.VisibilityProperty, "Visibility");
		}
	}
}
