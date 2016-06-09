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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.RangeControl.Internal;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Editors.RangeControl {
	public class CalendarClient : Control, IRangeControlClient {
		const double DefaultMinItemWidth = 20d;
		const double DefaultMinFontSize = 11;
		const double DefaultMaxItemWidth = 100d;
		const double MaxScaleFactor = 1500d;
		public static readonly DependencyProperty SelectionStartProperty;
		public static readonly DependencyProperty SelectionEndProperty;
		public static readonly DependencyProperty VisibleStartProperty;
		public static readonly DependencyProperty VisibleEndProperty;
		public static readonly DependencyProperty StartProperty;
		public static readonly DependencyProperty EndProperty;
		static readonly DependencyPropertyKey SelectionStartPropertyKey;
		static readonly DependencyPropertyKey SelectionEndPropertyKey;
		static readonly DependencyPropertyKey VisibleStartPropertyKey;
		static readonly DependencyPropertyKey VisibleEndPropertyKey;
		static readonly DependencyPropertyKey StartPropertyKey;
		static readonly DependencyPropertyKey EndPropertyKey;
		public static readonly DependencyProperty IntervalFactoriesProperty;
		public static readonly DependencyProperty ItemIntervalFactoryProperty;
		public static readonly DependencyProperty GroupIntervalFactoryProperty;
		static readonly DependencyPropertyKey ItemIntervalFactoryPropertyKey;
		static readonly DependencyPropertyKey GroupIntervalFactoryPropertyKey;
		public static readonly DependencyProperty AllowGroupingProperty;
		public static readonly DependencyProperty GroupingHeightProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		public static readonly DependencyProperty GroupItemStyleProperty;
		public static readonly DependencyProperty ZoomOutSelectionMarkerTemplateProperty;
		public static readonly DependencyProperty CustomItemIntervalFactoryProperty;
		public static readonly DependencyProperty CustomGroupIntervalFactoryProperty;
		static CalendarClient() {
			Type ownerType = typeof(CalendarClient);
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata((owner, args) => ((CalendarClient)owner).OnItemStyleChanged()));
			GroupItemStyleProperty = DependencyProperty.Register("GroupItemStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata((owner, args) => ((CalendarClient)owner).OnGroupItemStyleChanged()));
			SelectionStartPropertyKey =
				DependencyProperty.RegisterReadOnly("SelectionStart", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, (owner, args) => ((CalendarClient)owner).SelectionStartChanged(args.NewValue)));
			SelectionEndPropertyKey =
				DependencyProperty.RegisterReadOnly("SelectionEnd", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, (owner, args) => ((CalendarClient)owner).SelectionEndChanged(args.NewValue)));
			VisibleStartPropertyKey =
				DependencyProperty.RegisterReadOnly("VisibleStart", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, (owner, args) => ((CalendarClient)owner).VisibleStartChanged(args.NewValue)));
			VisibleEndPropertyKey =
				DependencyProperty.RegisterReadOnly("VisibleEnd", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, (owner, args) => ((CalendarClient)owner).VisibleEndChanged(args.NewValue)));
			StartPropertyKey =
				DependencyProperty.RegisterReadOnly("Start", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, (owner, args) => ((CalendarClient)owner).StartChanged(args.NewValue)));
			EndPropertyKey =
				DependencyProperty.RegisterReadOnly("End", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, (owner, args) => ((CalendarClient)owner).EndChanged(args.NewValue)));
			StartProperty = StartPropertyKey.DependencyProperty;
			EndProperty = EndPropertyKey.DependencyProperty;
			SelectionStartProperty = SelectionStartPropertyKey.DependencyProperty;
			SelectionEndProperty = SelectionEndPropertyKey.DependencyProperty;
			VisibleStartProperty = VisibleStartPropertyKey.DependencyProperty;
			VisibleEndProperty = VisibleEndPropertyKey.DependencyProperty;
			IntervalFactoriesProperty = DependencyProperty.Register("IntervalFactories", typeof(ObservableCollection<IntervalFactory>), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((CalendarClient)d).OnIntervalFactoriesChanged((ObservableCollection<IntervalFactory>)e.OldValue)));
			ItemIntervalFactoryPropertyKey = DependencyProperty.RegisterReadOnly("ItemIntervalFactory", typeof(IntervalFactory), ownerType, new FrameworkPropertyMetadata(null));
			GroupIntervalFactoryPropertyKey = DependencyProperty.RegisterReadOnly("GroupIntervalFactory", typeof(IntervalFactory), ownerType, new FrameworkPropertyMetadata(null));
			ItemIntervalFactoryProperty = ItemIntervalFactoryPropertyKey.DependencyProperty;
			GroupIntervalFactoryProperty = GroupIntervalFactoryPropertyKey.DependencyProperty;
			AllowGroupingProperty = DependencyProperty.Register("AllowGrouping", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange));
			GroupingHeightProperty = DependencyProperty.Register("GroupingHeight", typeof(double), ownerType, new FrameworkPropertyMetadata(30d, FrameworkPropertyMetadataOptions.AffectsArrange));
			ZoomOutSelectionMarkerTemplateProperty = DependencyProperty.Register("ZoomOutSelectionMarkerTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));
			CustomGroupIntervalFactoryProperty = DependencyProperty.Register("CustomGroupIntervalFactory", typeof(IntervalFactory), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((CalendarClient)d).OnCustomGroupIntervalFactoryChanged()));
			CustomItemIntervalFactoryProperty = DependencyProperty.Register("CustomItemIntervalFactory", typeof(IntervalFactory), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((CalendarClient)d).OnCustomItemIntervalFactoryChanged()));
		}
		private void OnCustomItemIntervalFactoryChanged() {
			ItemIntervalFactory = CustomItemIntervalFactory;
			InvalidateArrange();
		}
		private void OnCustomGroupIntervalFactoryChanged() {
			GroupIntervalFactory = CustomGroupIntervalFactory;
			InvalidateArrange();
		}
		public CalendarClient() {
			DefaultStyleKey = typeof(CalendarClient);
			GroupIntervalFactory = new DummyIntervalFactory();
			IntervalFactories = new ObservableCollection<IntervalFactory>();
			IntervalFactories.CollectionChanged += OnIntervalFactoriesCollectionChanged;
			Loaded += OnLoaded;
			SizeChanged += CalendarClientSizeChanged;
		}
		void OnIntervalFactoriesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			UpdateActualIntervalFactories();
		}
		private void UpdateActualIntervalFactories() {
			ActualIntervalFactories = IntervalFactories;
			UpdateIntervalFactories(clientBounds.Width);
			InvalidateArrange();
		}
		public IntervalFactory CustomGroupIntervalFactory {
			get { return (IntervalFactory)GetValue(CustomGroupIntervalFactoryProperty); }
			set { SetValue(CustomGroupIntervalFactoryProperty, value); }
		}
		public IntervalFactory CustomItemIntervalFactory {
			get { return (IntervalFactory)GetValue(CustomItemIntervalFactoryProperty); }
			set { SetValue(CustomItemIntervalFactoryProperty, value); }
		}
		public DataTemplate ZoomOutSelectionMarkerTemplate {
			get { return (DataTemplate)GetValue(ZoomOutSelectionMarkerTemplateProperty); }
			set { SetValue(ZoomOutSelectionMarkerTemplateProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		public Style GroupItemStyle {
			get { return (Style)GetValue(GroupItemStyleProperty); }
			set { SetValue(GroupItemStyleProperty, value); }
		}
		internal LayoutInfo LayoutInfo { get; private set; }
		public double GroupingHeight {
			get { return (double)GetValue(GroupingHeightProperty); }
			set { SetValue(GroupingHeightProperty, value); }
		}
		public bool AllowGrouping {
			get { return (bool)GetValue(AllowGroupingProperty); }
			set { SetValue(AllowGroupingProperty, value); }
		}
		public object Start {
			get { return GetValue(StartProperty); }
			private set { SetValue(StartPropertyKey, value); }
		}
		public object End {
			get { return GetValue(EndProperty); }
			private set { SetValue(EndPropertyKey, value); }
		}
		public object SelectionStart {
			get { return GetValue(SelectionStartProperty); }
			private set { SetValue(SelectionStartPropertyKey, value); }
		}
		public object SelectionEnd {
			get { return GetValue(SelectionEndProperty); }
			private set { SetValue(SelectionEndPropertyKey, value); }
		}
		public object VisibleStart {
			get { return GetValue(VisibleStartProperty); }
			private set { SetValue(VisibleStartPropertyKey, value); }
		}
		public object VisibleEnd {
			get { return GetValue(VisibleEndProperty); }
			private set { SetValue(VisibleEndPropertyKey, value); }
		}
		public ObservableCollection<IntervalFactory> IntervalFactories {
			get { return (ObservableCollection<IntervalFactory>)GetValue(IntervalFactoriesProperty); }
			set { SetValue(IntervalFactoriesProperty, value); }
		}
		ObservableCollection<IntervalFactory> actualIntervalFactories;
		internal ObservableCollection<IntervalFactory> ActualIntervalFactories {
			get { return actualIntervalFactories ?? GetDefaultFactories(); }
			set { actualIntervalFactories = CoerceIntervalFactories(value); }
		}
		private ObservableCollection<IntervalFactory> CoerceIntervalFactories(ObservableCollection<IntervalFactory> value) {
			return (value != null && value.Count > 0) ? value : GetDefaultFactories();
		}
		public IntervalFactory ItemIntervalFactory {
			get { return (IntervalFactory)GetValue(ItemIntervalFactoryProperty); }
			private set { SetValue(ItemIntervalFactoryPropertyKey, value); }
		}
		public IntervalFactory GroupIntervalFactory {
			get { return (IntervalFactory)GetValue(GroupIntervalFactoryProperty); }
			private set { SetValue(GroupIntervalFactoryPropertyKey, value); }
		}
		IValueNormalizer valueValueNormalizer;
		protected IValueNormalizer ValueNormalizer {
			get {
				if (valueValueNormalizer == null) valueValueNormalizer = new DateTimeValueNormalizer();
				return valueValueNormalizer;
			}
		}
		protected IRangeControlClient Instance { get { return this; } }
		protected Rect RenderBounds { get; private set; }
		int MinStepsNumber { get { return 3; } }
		double FontScaleFactor { get { return FontSize / DefaultMinFontSize; } }
		Size ViewPort { get; set; }
		protected object ActualStart { get { return Start ?? GetItemIntervalFactory().Snap(GetDefaultValue()); } }
		protected object ActualEnd { get { return End ?? GetNextValue(ActualStart); } }
		protected object ActualVisibleStart { get { return VisibleStart ?? ActualStart; } }
		protected object ActualVisibleEnd { get { return VisibleEnd ?? ActualEnd; } }
		protected IntervalFactory GetItemIntervalFactory() {
			return ItemIntervalFactory.Return(x => x, () => GetDefaultFactories().First());
		}
		protected virtual void StartChanged(object newValue) { }
		protected virtual void EndChanged(object newValue) { }
		protected virtual void SelectionStartChanged(object newValue) { }
		protected virtual void SelectionEndChanged(object newValue) { }
		protected virtual void VisibleStartChanged(object newValue) { }
		protected virtual void VisibleEndChanged(object newValue) { }
		protected bool IsCompatibleRenderStep(double renderStep) {
			return renderStep < GetMinItemWidth();
		}
		protected virtual void UpdateIntervalFactories(double totalWidth) {
			IntervalFactory groupFactory;
			IntervalFactory itemFactory;
			FindIntervalFactories(totalWidth, out groupFactory, out itemFactory);
			if (CustomGroupIntervalFactory == null)
				GroupIntervalFactory = groupFactory;
			if (CustomItemIntervalFactory == null)
				ItemIntervalFactory = itemFactory;
		}
		protected virtual LayoutInfo CreateLayoutInfo() {
			LayoutInfo info = new LayoutInfo {
				ComparableStart = Instance.GetComparableValue(ActualStart),
				ComparableEnd = Instance.GetComparableValue(ActualEnd),
				ComparableVisibleStart = Instance.GetComparableValue(ActualVisibleStart),
				ComparableVisibleEnd = Instance.GetComparableValue(ActualVisibleEnd)
			};
			DateTime start;
			DateTime end;
			GetVisibleRangeWithOffset(out start, out end);
			info.ComparableRenderVisibleStart = Instance.GetComparableValue(start);
			info.ComparableRenderVisibleEnd = Instance.GetComparableValue(end);
			info.ComparableSelectionStart = ToComparableRange(info.ComparableStart, info.ComparableEnd, Instance.GetComparableValue(SelectionStart));
			info.ComparableSelectionEnd = ToComparableRange(info.ComparableStart, info.ComparableEnd, Instance.GetComparableValue(SelectionEnd));
			return info;
		}
		private void GetVisibleRangeWithOffset(out DateTime start, out DateTime end) {
			start = (DateTime)ActualVisibleStart;
			end = (DateTime)ActualVisibleEnd;
			if (ItemIntervalFactory != null) {
				int factor = 5;
				double startOffset = Instance.GetComparableValue(ActualVisibleStart) - factor * GetComparableStep(ItemIntervalFactory, ActualVisibleStart);
				double endOffset = Instance.GetComparableValue(ActualVisibleEnd) + factor * GetComparableStep(ItemIntervalFactory, ActualVisibleEnd);
				start = (DateTime)Instance.GetRealValue(Math.Max(startOffset, Instance.GetComparableValue(ActualStart)));
				end = (DateTime)Instance.GetRealValue(Math.Min(endOffset, Instance.GetComparableValue(ActualEnd)));
			}
		}
		private void OnIntervalFactoriesChanged(ObservableCollection<IntervalFactory> oldValue) {
			if (oldValue != null) oldValue.CollectionChanged -= OnIntervalFactoriesCollectionChanged;
			if (IntervalFactories != null) IntervalFactories.CollectionChanged += OnIntervalFactoriesCollectionChanged;
			UpdateActualIntervalFactories();
		}
		internal double GetGroupingHeight() {
			return AllowGrouping ? GetGroupingHeight(ActualHeight) : 0d;
		}
		protected double ToComparableRange(double min, double max, double value) {
			double minValue = Math.Max(min, value);
			double maxValue = Math.Min(max, minValue);
			return maxValue;
		}
		protected double RenderToComparable(double renderValue, double totalWidth) {
			double normalValue = renderValue / totalWidth;
			return normalValue * (LayoutInfo.ComparableEnd - LayoutInfo.ComparableStart) + LayoutInfo.ComparableStart;
		}
		protected override Size MeasureOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
		CalendarClientPanel caledarPanel;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			caledarPanel = LayoutHelper.FindElementByType(this, typeof(CalendarClientPanel)) as CalendarClientPanel;
			if (caledarPanel != null)
				caledarPanel.Owner = this;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (caledarPanel == null) return base.ArrangeOverride(finalSize);
			if (double.IsInfinity(finalSize.Height) || double.IsInfinity(finalSize.Width)) return base.ArrangeOverride(finalSize);
			PrepareToRender(CalcUpdateBounds(finalSize));
			caledarPanel.SetLayoutInfo(LayoutInfo, RenderBounds);
			caledarPanel.InvalidateArrange();
			return base.ArrangeOverride(finalSize);
		}
		protected internal void PrepareToRender(Rect renderBounds) {
			RenderBounds = renderBounds;
			UpdateLayoutInfo();
		}
		protected double GetGroupingHeight(double height) {
			return Math.Min(GroupingHeight, height);
		}
		protected void InvalidateRender() {
			InvalidateArrange();
		}
		protected object GetDefaultValue() {
			return DateTime.Today;
		}
		protected virtual ObservableCollection<IntervalFactory> GetDefaultFactories() {
			return new ObservableCollection<IntervalFactory> {
				new YearIntervalFactory(),
				new QuarterIntervalFactory(),
				new MonthIntervalFactory(),
				new DayIntervalFactory(),
				new HourIntervalFactory(),
				new MinuteIntervalFactory(),
				new SecondIntervalFactory(),
			};
		}
		protected virtual void UpdateClientBoundsInternal(ref Rect bounds) {
			RectHelper.Deflate(ref bounds, new Thickness(0, 0, 0, AllowGrouping ? GroupingHeight : 0d));
		}
		protected double GetRenderStep(IntervalFactory factory, double totalWidth) {
			object defaultValue = factory.Snap(GetDefaultValue());
			object nextValue = factory.GetNextValue(defaultValue);
			double step = ValueNormalizer.GetComparableValue(nextValue) - ValueNormalizer.GetComparableValue(defaultValue);
			object start = Start ?? defaultValue;
			object end = End ?? factory.GetNextValue(start);
			double region = ValueNormalizer.GetComparableValue(end) - ValueNormalizer.GetComparableValue(start);
			double normalStep = step / region;
			return normalStep * totalWidth;
		}
		protected bool IsGroupInterval(Point point) {
			return point.X.InRange(0, ActualWidth) && point.Y.InRange(Math.Max(0, ActualHeight - GetGroupingHeight()), ActualHeight);
		}
		protected void OnSetSelectionRange() {
			InvalidateRender();
		}
		private void OnLoaded(object sender, RoutedEventArgs e) {
			SetupActualIntervalFactories();
			UpdateDefaultRanges();
			UpdateIntervalFactories(ActualWidth);
			RaiseClientDataChanged(LayoutChangedType.Layout);
		}
		private void SetupActualIntervalFactories() {
			ActualIntervalFactories = CoerceIntervalFactories(ActualIntervalFactories);
		}
		private void CalendarClientSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateClientBounds();
			UpdateIntervalFactories(clientBounds.Width);
			RaiseClientDataChanged(LayoutChangedType.Layout);
		}
		private void OnItemStyleChanged() {
			Clear();
			InvalidateArrange();
		}
		private void Clear() {
			if (caledarPanel != null)
				caledarPanel.Clear();
		}
		private void OnGroupItemStyleChanged() {
			Clear();
			InvalidateArrange();
		}
		private object GetNextValue(object value) {
			return GetItemIntervalFactory().GetNextValue(value);
		}
		private void UpdateDefaultRanges() {
			UpdateDefaultRange();
			UpdateDefaultVisibleRange();
		}
		private void UpdateDefaultRange() {
			if (Start == null) Start = ItemIntervalFactory != null ? ItemIntervalFactory.Snap(GetDefaultValue()) : ActualIntervalFactories.First().Snap(GetDefaultValue());
			if (End == null) End = ItemIntervalFactory != null ? ItemIntervalFactory.GetNextValue(GetDefaultValue()) : ActualIntervalFactories.First().GetNextValue(GetDefaultValue());
		}
		private void UpdateDefaultVisibleRange() {
			if (VisibleStart == null) VisibleStart = Start;
			if (VisibleEnd == null) VisibleEnd = End;
		}
		void FindIntervalFactories(double totalWidth, out IntervalFactory groupFactory, out IntervalFactory itemFactory) {
			groupFactory = GetFirstFactory();
			itemFactory = groupFactory;
			if (Start == null || End == null) return;
			foreach (IntervalFactory factory in ActualIntervalFactories) {
				double renderStep = GetRenderStep(factory, totalWidth);
				if (IsCompatibleRenderStep(renderStep))
					break;
				groupFactory = itemFactory;
				itemFactory = factory;
			}
		}
		private IntervalFactory GetFirstFactory() {
			return ActualIntervalFactories.If(x => x.Count > 0).Return(x => x.First(), () => GetDefaultFactories().First());
		}
		private double GetDefaultMinItemWidth() {
			return Math.Max(DefaultMinItemWidth, DefaultMinItemWidth * FontScaleFactor);
		}
		private double GetMinItemWidth() {
			return GetDefaultMinItemWidth();
		}
		private double GetMaxItemWidth() {
			return DefaultMaxItemWidth;
		}
		private Rect CalcUpdateBounds(Size size) {
			return ViewPort.IsZero() ? new Rect(new Point(0, 0), size) : CalcRenderRect(size);
		}
		private Rect CalcRenderRect(Size size) {
			double left = ToNormalized(Instance.GetComparableValue(VisibleStart));
			return new Rect(new Point(size.Width * left, 0), ViewPort);
		}
		private double ToNormalized(double value) {
			double comparableStart = Instance.GetComparableValue(Start);
			double comparableEnd = Instance.GetComparableValue(End);
			return (value - comparableStart) / (comparableEnd - comparableStart);
		}
		private void UpdateLayoutInfo() {
			LayoutInfo = CreateLayoutInfo();
		}
		private void UpdateClientBounds() {
			Rect bounds = new Rect(new Point(0, 0), new Size(ActualWidth, ActualHeight));
			UpdateClientBoundsInternal(ref bounds);
			this.clientBounds = bounds;
		}
		private double GetRenderStep(IntervalFactory factory, object visibleStart, object visibleEnd, double viewport) {
			double comparableStart = ValueNormalizer.GetComparableValue(visibleStart);
			double comparableEnd = ValueNormalizer.GetComparableValue(visibleEnd);
			double comparableLength = comparableEnd - comparableStart;
			double steps = comparableLength / GetComparableStep(factory, visibleStart);
			return viewport / steps;
		}
		private double GetComparableStep(IntervalFactory factory, object value) {
			object start = factory.Snap(value);
			object next = factory.GetNextValue(start);
			return ValueNormalizer.GetComparableValue(next) - ValueNormalizer.GetComparableValue(start);
		}
		#region IRangeControlClient
		RangeControlClientHitTestResult IRangeControlClient.HitTest(Point point) {
			if (IsGroupInterval(point)) {
				double value = RenderToComparable(point.X, ActualWidth);
				return new RangeControlClientHitTestResult(
					RangeControlClientRegionType.GroupInterval,
					GroupIntervalFactory.Snap(ValueNormalizer.GetRealValue(value)),
					GroupIntervalFactory.GetNextValue(ValueNormalizer.GetRealValue(value)));
			}
			if (IsItemInterval(point)) {
				double value = RenderToComparable(point.X, ActualWidth);
				return new RangeControlClientHitTestResult(
					RangeControlClientRegionType.ItemInterval,
					ItemIntervalFactory.Snap(ValueNormalizer.GetRealValue(value)),
					ItemIntervalFactory.GetNextValue(ValueNormalizer.GetRealValue(value)));
			}
			return RangeControlClientHitTestResult.Nothing;
		}
		protected bool IsItemInterval(Point point) {
			return point.X.InRange(0, ActualWidth) && point.Y.InRange(0, Math.Max(0, ActualHeight - GetGroupingHeight()));
		}
		bool IRangeControlClient.SetVisibleRange(object visibleStart, object visibleEnd, Size viewportSize) {
			ConstrainByType(ref visibleStart, ref visibleEnd);
			return CorrectVisibleRange(visibleStart, visibleEnd, viewportSize.Width);
		}
		bool CorrectVisibleRange(object visibleStart, object visibleEnd, double width) {
			bool isCorrected = CorrectVisibleRangeByData(ref visibleStart, ref visibleEnd);
			IntervalFactory highFactory = ActualIntervalFactories.If(x => x.Count > 0).Return(x => x.First(), () => GetDefaultFactories().First());
			IntervalFactory lowFactory = ActualIntervalFactories.If(x => x.Count > 0).Return(x => x.Last(), () => GetDefaultFactories().Last());
			double highRenderStep = Math.Min(GetRenderStep(highFactory, visibleStart, visibleEnd, width), width / MinStepsNumber);
			double lowRenderStep = Math.Min(GetRenderStep(lowFactory, visibleStart, visibleEnd, width), width / MinStepsNumber);
			if (highRenderStep < GetMinItemWidth()) {
				CalcFactoryDefinedVisibleRange(highFactory, ref visibleStart, ref visibleEnd, highRenderStep, width);
				isCorrected = true;
			}
			else if (lowRenderStep > GetMaxItemWidth()) {
				CalcFactoryDefinedVisibleRange(lowFactory, ref visibleStart, ref visibleEnd, lowRenderStep, width);
				isCorrected = true;
			}
			if (CalcMaxScaleCorrection(ActualStart, ActualEnd, ref visibleStart, ref visibleEnd, lowFactory, lowRenderStep * MinStepsNumber))
				isCorrected = true;
			VisibleStart = visibleStart;
			VisibleEnd = visibleEnd;
			return isCorrected;
		}
		bool CorrectVisibleRangeByData(ref object visibleStart, ref object visibleEnd) {
			bool corrected = false;
			double comparableVisibleStart = ValueNormalizer.GetComparableValue(visibleStart);
			double comparableVisibleEnd = ValueNormalizer.GetComparableValue(visibleEnd);
			if (comparableVisibleStart > comparableVisibleEnd)
				comparableVisibleEnd = comparableVisibleStart;
			double comparableStart = ValueNormalizer.GetComparableValue(ActualStart);
			double comparableEnd = ValueNormalizer.GetComparableValue(ActualEnd);
			if (!comparableVisibleStart.InRange(comparableStart, comparableEnd)) {
				visibleStart = ValueNormalizer.GetRealValue(comparableVisibleStart.ToRange(comparableStart, comparableEnd));
				corrected = true;
			}
			if (!comparableVisibleEnd.InRange(comparableStart, comparableEnd)) {
				visibleEnd = ValueNormalizer.GetRealValue(comparableVisibleEnd.ToRange(comparableStart, comparableEnd));
				corrected = true;
			}
			return corrected;
		}
		void CalcFactoryDefinedVisibleRange(IntervalFactory factory, ref object visibleStart, ref object visibleEnd, double itemWidth, double viewport) {
			double lastComparableStart = VisibleStart != null ? ValueNormalizer.GetComparableValue(VisibleStart) : 0d;
			double comparableStart = ValueNormalizer.GetComparableValue(visibleStart);
			double comparableEnd = ValueNormalizer.GetComparableValue(visibleEnd);
			double snappedStart = GetSnappedValue(factory, comparableStart, true);
			double snappedNext = GetSnappedValue(factory, comparableStart, false);
			double comparableStep = snappedNext - snappedStart;
			if (!lastComparableStart.AreClose(comparableStart)) {
				comparableStart = comparableEnd - (viewport / itemWidth) * comparableStep;
				visibleStart = ValueNormalizer.GetRealValue(comparableStart);
			}
			else {
				comparableEnd = comparableStart + (viewport / itemWidth) * comparableStep;
				visibleEnd = ValueNormalizer.GetRealValue(comparableEnd);
			}
		}
		bool CalcMaxScaleCorrection(object start, object end, ref object visibleStart, ref object visibleEnd, IntervalFactory factory, double width) {
			double comparableVisibleStart = ValueNormalizer.GetComparableValue(visibleStart);
			double comparableVisibleEnd = ValueNormalizer.GetComparableValue(visibleEnd);
			double comparableStart = ValueNormalizer.GetComparableValue(start);
			double comparableEnd = ValueNormalizer.GetComparableValue(end);
			double scaleFactor = (comparableEnd - comparableStart) / (comparableVisibleEnd - comparableVisibleStart);
			if (Math.Ceiling(scaleFactor) < MaxScaleFactor)
				return false;
			double maxSizeNumber = (comparableEnd - comparableStart) / MaxScaleFactor;
			double visibleEndCandidate = comparableVisibleEnd;
			visibleEndCandidate = visibleEndCandidate > comparableEnd ? comparableEnd : visibleEndCandidate;
			double lastComparableStart = VisibleStart != null ? ValueNormalizer.GetComparableValue(VisibleStart) : 0d;
			if (lastComparableStart != comparableVisibleStart) {
				visibleEnd = ValueNormalizer.GetRealValue(comparableVisibleEnd);
				visibleStart = ValueNormalizer.GetRealValue(Math.Max(comparableVisibleEnd - maxSizeNumber, comparableStart).ToRange(comparableStart, comparableEnd));
			}
			else {
				visibleStart = ValueNormalizer.GetRealValue(comparableVisibleStart);
				visibleEnd = ValueNormalizer.GetRealValue(Math.Min(comparableVisibleStart + maxSizeNumber, comparableEnd).ToRange(comparableStart, comparableEnd));
			}
			double maxItemWidth = ViewPort.Width / MinStepsNumber;
			double comparableItemWidth = maxItemWidth / clientBounds.Width;
			double comparableVisibleRangeWidth = comparableItemWidth * MinStepsNumber;
			if (comparableItemWidth > (comparableVisibleEnd - comparableVisibleStart)) {
				visibleEndCandidate = comparableVisibleStart + comparableVisibleRangeWidth;
				visibleEnd = ValueNormalizer.GetRealValue(visibleEndCandidate);
			}
			return true;
		}
		protected double GetSnappedValue(IntervalFactory factory, double comparableValue, bool isLeft) {
			object realValue = factory.Snap(Instance.GetRealValue(comparableValue));
			object snappedValue = factory.Snap(realValue);
			if (!isLeft)
				snappedValue = factory.GetNextValue(snappedValue);
			return Instance.GetComparableValue(snappedValue);
		}
		bool IRangeControlClient.SetSelectionRange(object selectedMinimum, object selectedMaximum, Size viewportSize, bool isSnapped) {
			ConstrainByType(ref selectedMinimum, ref selectedMaximum);
			SelectionStart = selectedMinimum;
			SelectionEnd = selectedMaximum;
			OnSetSelectionRange();
			return true;
		}
		bool IRangeControlClient.SetRange(object start, object end, Size viewportSize) {
			ConstrainByType(ref start, ref end);
			return SetRangeInternal(start, end, viewportSize);
		}
		bool SetRangeInternal(object start, object end, Size viewportSize) {
			Start = start;
			End = end;
			UpdateVisibleRangeOnSetRange(ActualStart, ActualEnd, viewportSize);
			InvalidateRender();
			return true;
		}
		private void ConstrainByType(ref object start, ref object end) {
			start = start != null && TryGetValue(ref start) ? start : Start;
			end = end != null && TryGetValue(ref end) ? end : End;
		}
		private bool TryGetValue(ref object value) {
			DateTime date;
			bool isSuccessed = DateTime.TryParse(value.ToString(), out date);
			value = date;
			return isSuccessed;
		}
		private static bool IsTypeCorrect(Type type) {
			return type == typeof(DateTime);
		}
		void UpdateVisibleRangeOnSetRange(object start, object end, Size viewportSize) {
			double comparableStart = ValueNormalizer.GetComparableValue(start);
			double comparableEnd = ValueNormalizer.GetComparableValue(end);
			double comparableVisibleStart = ValueNormalizer.GetComparableValue(ActualVisibleStart);
			double comparableVisibleEnd = ValueNormalizer.GetComparableValue(ActualVisibleEnd);
			if (!comparableVisibleStart.InRange(comparableStart, comparableEnd) || !comparableVisibleEnd.InRange(comparableStart, comparableEnd)) {
				comparableVisibleStart = comparableVisibleStart.ToRange(comparableStart, comparableEnd);
				comparableVisibleEnd = comparableVisibleEnd.ToRange(comparableVisibleStart, comparableEnd);
				Instance.SetVisibleRange(ValueNormalizer.GetRealValue(comparableVisibleStart), ValueNormalizer.GetRealValue(comparableVisibleEnd), viewportSize);
			}
		}
		public event EventHandler<LayoutChangedEventArgs> LayoutChanged;
		protected void RaiseClientDataChanged(LayoutChangedType type) {
			var args = CreateChangedEventArgs(type);
			if (LayoutChanged != null)
				LayoutChanged(this, args);
		}
		LayoutChangedEventArgs CreateChangedEventArgs(LayoutChangedType type) {
			return new LayoutChangedEventArgs(type);
		}
		object IRangeControlClient.GetSnappedValue(object value, bool isLeft) {
			return isLeft ? GetItemIntervalFactory().Snap(value) : GetItemIntervalFactory().GetNextValue(value);
		}
		double IRangeControlClient.GetComparableValue(object realValue) {
			return ValueNormalizer.GetComparableValue(realValue);
		}
		object IRangeControlClient.GetRealValue(double comparable) {
			return ValueNormalizer.GetRealValue(comparable);
		}
		void IRangeControlClient.Invalidate(Size viewPort) {
			ViewPort = viewPort;
			InvalidateArrange();
		}
		Rect clientBounds;
		Rect IRangeControlClient.ClientBounds { get { return clientBounds; } }
		bool IRangeControlClient.ConvergeThumbsOnZoomingOut { get { return true; } }
		bool IRangeControlClient.GrayOutNonSelectedRange { get { return false; } }
		bool IRangeControlClient.AllowThumbs { get { return false; } }
		bool IRangeControlClient.SnapSelectionToGrid { get { return true; } }
		string IRangeControlClient.FormatText(object value) {
			return GetItemIntervalFactory().FormatLabelText(value);
		}
		#endregion
	}
}
