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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class Range : ChartNonVisualElement {
		internal class RangeResolver {
			IAxisData axis;
			IAxisRangeData range;
			MediatorResolver mediator;
			double min = double.NaN;
			double max = double.NaN;
			object minValue = null;
			object maxValue = null;
			bool locked = false;
			public object MaxValue {
				set {
					Max = value;
					TryAssign();
				}
			}
			public object MinValue {
				set {
					Min = value;
					TryAssign();
				}
			}
			IScaleMap Map { get { return axis.AxisScaleTypeMap; } }
			object Max {
				set {
					if (locked)
						return;
					maxValue = value;
					if (value == null)
						max = double.NaN;
					else
						max = Map.NativeToInternal(value);
				}
			}
			object Min {
				set {
					if (locked)
						return;
					minValue = value;
					if (value == null)
						min = double.NaN;
					else
						min = Map.NativeToInternal(value);
				}
			}
			double MinInternal { get { return double.IsNaN(min) ? Map.NativeToInternal(range.MinValue) : min; } }
			double MaxInternal { get { return double.IsNaN(max) ? Map.NativeToInternal(range.MaxValue) : max; } }
			bool Resolved { get { return  (mediator == null || mediator.Resolved || IsWholeRange); } }
			bool IsEmpty { get { return minValue == null && maxValue == null; } }
			bool EqualsWithRangeData { get { return IsEqual(minValue, range.MinValue) && IsEqual(maxValue, range.MaxValue); } }
			bool IsWholeRange { get { return range is IWholeAxisRangeData; } }
			public RangeResolver(IAxisData axis, IAxisRangeData range) {
				this.axis = axis;
				this.range = range;
			}
			bool IsEqual(object first, object second) {
				return (first == null && second == null) || (second != null && first != null && first.Equals(second));
			}
			public void SetRange(object min, object max) {
				if (locked || (min == null && max == null))
					return;
				Max = max;
				Min = min;
				TryAssign();
			}
			public bool IsIncluded(RangeResolver range) {
				return (MinInternal <= range.MinInternal && range.MinInternal <= MaxInternal) ||
					(MinInternal <= range.MaxInternal && range.MaxInternal <= MaxInternal);
			}
			public void TryAssign() {
				if (locked || EqualsWithRangeData)
					return;
				TryAssignWithoutThrow();
				if (mediator != null)
					mediator.TryAssignAnotheResolver(this);
			}
			public void TryAssignWithoutThrow() {
				if (!IsEmpty &&  range.CorrectionMode != RangeCorrectionMode.InternalValues) {
					bool setMax = maxValue != null && !maxValue.Equals(range.MaxValue);
					bool setMin = minValue != null && !minValue.Equals(range.MinValue);
					if (setMax && setMin && Map.IsCompatible(minValue) && Map.IsCompatible(maxValue)) {
						((RangeDataBase)range).SetRange(minValue, maxValue, double.NaN, double.NaN, true);
						return;
					}
					if (setMin && Map.IsCompatible(minValue))
						((RangeDataBase)range).MinValue = minValue;
					if (setMax && Map.IsCompatible(maxValue))
						((RangeDataBase)range).MaxValue = maxValue;
				}
			}
			public void SetMediator(MediatorResolver mediator) {
				this.mediator = mediator;
			}
			public void Lock() { locked = true; }
			public void Unlock() { locked = false; }
			public void Reset() {
				minValue = maxValue = null;
			}
		}
		internal class MediatorResolver {
			VisualRangeData visualRangeData;
			RangeResolver visualRange;
			RangeResolver wholeRange;
			public bool Resolved {
				get {
					if (visualRangeData != null && visualRangeData.CorrectionMode == RangeCorrectionMode.Auto)
						return true;
					return wholeRange.IsIncluded(visualRange);
				}
			}
			public MediatorResolver(Range visualRange, Range wholeRange) {
				visualRangeData = (VisualRangeData)visualRange.rangeData;
				this.visualRange = visualRange.rangeRelationResolver;
				this.wholeRange = wholeRange.rangeRelationResolver;
				this.visualRange.SetMediator(this);
				this.wholeRange.SetMediator(this);
				this.wholeRange.TryAssignWithoutThrow();
				this.visualRange.TryAssignWithoutThrow();
			}
			internal void TryAssignAnotheResolver(RangeResolver rangeResolver) {
				if (visualRange == rangeResolver)
					wholeRange.TryAssignWithoutThrow();
				else
					visualRange.TryAssignWithoutThrow();
			}
		}
		public static readonly DependencyProperty MinValueProperty = DependencyPropertyManager.Register("MinValue", typeof(object),
			typeof(Range), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, MinValuePropertyChanged));
		public static readonly DependencyProperty MaxValueProperty = DependencyPropertyManager.Register("MaxValue", typeof(object),
			typeof(Range), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, MaxValuePropertyChanged));
		public static readonly DependencyProperty AutoSideMarginsProperty = DependencyPropertyManager.Register("AutoSideMargins",
			typeof(bool), typeof(Range), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, AutoSideMarginsPropertyChanged));
		public static readonly DependencyProperty SideMarginsValueProperty = DependencyPropertyManager.Register("SideMarginsValue",
			typeof(double), typeof(Range), new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, SideMarginsValuePropertyChanged));
		static object CoerceValueProperty(IAxisData axis, object value) {
			if (axis == null || axis.AxisScaleTypeMap == null)
				return value;
			object nativeValue = axis.AxisScaleTypeMap.ConvertValue(value, CultureInfo.InvariantCulture);
			return axis.AxisScaleTypeMap.IsCompatible(nativeValue) ? nativeValue : null;
		}
		static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Range range = d as Range;
			if (range != null && !range.locker.IsLocked)
				ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache);
		}
		static void MinValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Range range = d as Range;
			if (range != null && range.rangeRelationResolver != null)
				range.rangeRelationResolver.MinValue = CoerceValueProperty(range.Axis, e.NewValue);
			PropertyChanged(d, e);
		}
		static void MaxValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Range range = d as Range;
			if (range != null && range.rangeRelationResolver != null)
				range.rangeRelationResolver.MaxValue = CoerceValueProperty(range.Axis, e.NewValue);
			PropertyChanged(d, e);
		}
		static void AutoSideMarginsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Range range = d as Range;
			if (range != null) {
				if (range.rangeData != null && !range.locker.IsLocked)
					range.rangeData.AutoSideMargins = (bool)e.NewValue ? SideMarginMode.UserEnable : SideMarginMode.UserDisable;
				range.autoSideMargins = (bool)e.NewValue ? SideMarginMode.UserEnable : SideMarginMode.UserDisable;
			}
			PropertyChanged(d, e);
		}
		static void SideMarginsValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Range range = d as Range;
			if (!double.IsNaN((double)e.NewValue))
				range.autoSideMargins = SideMarginMode.UserDisable;
			if (range != null && range.rangeData != null && !range.locker.IsLocked)
				range.rangeData.SideMarginsValue = (double)e.NewValue;
			PropertyChanged(d, e);
		}
		internal static void AlwaysShowZeroLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Range axisRange = d as Range;
			if (axisRange != null) {
				if (axisRange.rangeData != null)
					axisRange.rangeData.AlwaysShowZeroLevel = (bool)e.NewValue;
				else
					axisRange.alwaysShowZeroLevel = (bool)e.NewValue;
			}
			PropertyChanged(d, e);
		}
		readonly Locker locker = new Locker();
		SideMarginMode autoSideMargins = SideMarginMode.Enable;
		RangeDataBase rangeData;
		RangeResolver rangeRelationResolver;
		bool? alwaysShowZeroLevel;
		IAxisData Axis { get { return ((IChartElement)this).Owner as AxisBase; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeMinValue"),
#endif
		Category(Categories.Behavior),
		TypeConverter(typeof(AxisValueTypeConverter)),
		XtraSerializableProperty
		]
		public object MinValue {
			get { return GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeMaxValue"),
#endif
		Category(Categories.Behavior),
		TypeConverter(typeof(AxisValueTypeConverter)),
		XtraSerializableProperty
		]
		public object MaxValue {
			get { return GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAutoSideMargins"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool AutoSideMargins {
			get { return (bool)GetValue(AutoSideMarginsProperty); }
			set { SetValue(AutoSideMarginsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeSideMarginsValue"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double SideMarginsValue {
			get { return (double)GetValue(SideMarginsValueProperty); }
			set { SetValue(SideMarginsValueProperty, value); }
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("RangeActualMinValue")]
#endif
		public object ActualMinValue { get { return rangeData != null ? rangeData.MinValue : null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("RangeActualMaxValue")]
#endif
		public object ActualMaxValue { get { return rangeData != null ? rangeData.MaxValue : null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("RangeActualMinValue")]
#endif
		public double ActualMinValueInternal { get { return rangeData != null ? rangeData.MinValueInternal : double.NaN; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("RangeActualMaxValue")]
#endif
		public double ActualMaxValueInternal { get { return rangeData != null ? rangeData.MaxValueInternal : double.NaN; } }
		internal void SetRangeData(RangeDataBase data) {
			this.rangeData = data;
			rangeRelationResolver = new RangeResolver(data.Axis, data);
			object min = CoerceValueProperty(rangeData.Axis, this.MinValue);
			object max = CoerceValueProperty(rangeData.Axis, this.MaxValue);
			rangeRelationResolver.SetRange(min, max);
			rangeData.SideMarginsValue = this.SideMarginsValue;
			rangeData.AutoSideMargins = this.autoSideMargins;
			if (alwaysShowZeroLevel.HasValue)
				rangeData.AlwaysShowZeroLevel = alwaysShowZeroLevel.Value;
			rangeData.SetUpdateDependencyPropertyDelegate(delegate() { UpdateDependencyProperty(); });
		}
		internal void UpdateDependencyProperty() {
			Lock();
			if (rangeData.SideMarginsValue != 0)
				this.SetCurrentValue(SideMarginsValueProperty, rangeData.SideMarginsValue);
			if (rangeData.AutoSideMargins == SideMarginMode.UserDisable || rangeData.AutoSideMargins == SideMarginMode.UserEnable)
				this.SetCurrentValue(AutoSideMarginsProperty, rangeData.AutoSideMargins == SideMarginMode.UserEnable);
			Unlock();
		}
		internal void UpdateMinMaxValues(AxisBase axis) {
			if (rangeData.CorrectionMode != RangeCorrectionMode.InternalValues) {
				object min = CoerceValueProperty(axis, MinValue);
				object max = CoerceValueProperty(axis, MaxValue);
				rangeRelationResolver.MinValue = min;
				rangeRelationResolver.MaxValue = max;
			}
		}
		internal void UpdateMinMaxValues() {
			if (Axis != null)
				UpdateMinMaxValues((AxisBase)Axis);
		}
		void Lock() {
			locker.Lock();
			rangeRelationResolver.Lock();
		}
		void Unlock() {
			rangeRelationResolver.Unlock();
			locker.Unlock();
		}
		public void SetAuto() {
			Lock();
			if (rangeRelationResolver != null)
				rangeRelationResolver.Reset();
			MinValue = null;
			MaxValue = null;
			Unlock();
			rangeData.CorrectionMode = RangeCorrectionMode.Auto;
			PropertyChanged(this, new DependencyPropertyChangedEventArgs());
		}
		public void SetMinMaxValues(object minValue, object maxValue) {
			if (minValue == null && maxValue == null)
				return;
			Lock();
			MinValue = minValue;
			MaxValue = maxValue;
			Unlock();
			object min = minValue == null ? rangeData.MinValue : minValue;
			object max = maxValue == null ? rangeData.MaxValue : maxValue;
			rangeData.SetRange(min, max, double.NaN, double.NaN, true);
		}
	}
}
