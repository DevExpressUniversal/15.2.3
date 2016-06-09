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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[TypeConverter(typeof(AxisRangeTypeConverter)),
	Obsolete(ObsoleteMessages.AxisRangeClass)]
	public sealed class AxisRange : ChartNonVisualElement, IAxisRange, ICloneable {
		public static readonly DependencyProperty MinValueProperty = DependencyPropertyManager.Register("MinValue", typeof(object),
			typeof(AxisRange), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, MinValuePropertyChanged));
		public static readonly DependencyProperty MaxValueProperty = DependencyPropertyManager.Register("MaxValue", typeof(object),
			typeof(AxisRange), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, MaxValuePropertyChanged));
		public static readonly DependencyProperty MinValueInternalProperty = DependencyPropertyManager.Register("MinValueInternal",
			typeof(double), typeof(AxisRange), new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, MinInternalValuePropertyChanged));
		public static readonly DependencyProperty MaxValueInternalProperty = DependencyPropertyManager.Register("MaxValueInternal",
			typeof(double), typeof(AxisRange), new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, MaxInternalValuePropertyChanged));
		public static readonly DependencyProperty SideMarginsEnabledProperty = DependencyPropertyManager.Register("SideMarginsEnabled",
			typeof(bool), typeof(AxisRange), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, SideMarginsEnabledPropertyChanged));
		static object CorrectValueProperty(IAxisData axis, object value) {
			if (axis == null || axis.AxisScaleTypeMap == null)
				return value;
			return axis.AxisScaleTypeMap.TryParse(value, CultureInfo.InvariantCulture);
		}
		static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisRange range = d as AxisRange;
			if (range != null && !range.locker.IsLocked)
				ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache);
		}
		static void MinValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisRange range = d as AxisRange;
			if (range != null)
				range.Min = CorrectValueProperty(range.Axis, e.NewValue);
			PropertyChanged(d, e);
		}
		static void MaxValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisRange range = d as AxisRange;
			if (range != null)
				range.Max = CorrectValueProperty(range.Axis, e.NewValue);
			PropertyChanged(d, e);
		}
		static void MinInternalValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisRange range = d as AxisRange;
			if (range != null)
				range.MinInternal = (double)e.NewValue;
			PropertyChanged(d, e);
		}
		static void MaxInternalValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisRange range = d as AxisRange;
			if (range != null)
				range.MaxInternal = (double)e.NewValue;
			PropertyChanged(d, e);
		}
		static void SideMarginsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisRange range = d as AxisRange;
			if (range != null)
				range.SideMargins = (bool)e.NewValue;
			PropertyChanged(d, e);
		}
		internal static void AlwaysShowZeroLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisRange range = d as AxisRange;
			if (range != null)
				range.AlwaysShowZeroLevel = (bool)e.NewValue;
			PropertyChanged(d, e);
		}
		readonly Locker locker = new Locker();
		object actualMinValue;
		object actualMaxValue;
		double actualMinInternalValue = double.NaN;
		double actualMaxInternalValue = double.NaN;
		object Min {
			set {
				actualMinValue = value;
				if (Axis == null || value == null)
					return;
				TryDisableSideMargins();
				if (IsScrollingEnable()) {
					if (IsScrollingRange)
						((WholeRangeData)Axis.WholeRangeData).MinValue = value;
					else
						((VisualRangeData)Axis.VisualRangeData).MinValue = value;
				}
				else {
					((WholeRangeData)Axis.WholeRangeData).MinValue = value;
					((VisualRangeData)Axis.VisualRangeData).MinValue = value;
				}
			}
		}
		object Max {
			set {
				actualMaxValue = value;
				if (Axis == null || value == null)
					return;
				TryDisableSideMargins();
				if (IsScrollingEnable()) {
					if (IsScrollingRange)
						((WholeRangeData)Axis.WholeRangeData).MaxValue = value;
					else
						((VisualRangeData)Axis.VisualRangeData).MaxValue = value;
				}
				else {
					((WholeRangeData)Axis.WholeRangeData).MaxValue = value;
					((VisualRangeData)Axis.VisualRangeData).MaxValue = value;
				}
			}
		}
		double MinInternal {
			set {
				if (Axis == null || double.IsNaN(value))
					return;
				if (NeedSwapMinMax(value, actualMaxInternalValue)) {
					this.SetInternalMinMaxValues(actualMaxInternalValue, value);
					return;
				}
				actualMinInternalValue = value;
				double minValue = CorrectDatetime(value);
				if (IsScrollingEnable()) {
					((VisualRangeData)Axis.VisualRangeData).MinValueInternal = minValue;
				}
				else {
					((WholeRangeData)Axis.WholeRangeData).MinValueInternal = minValue;
					((VisualRangeData)Axis.VisualRangeData).MinValueInternal = minValue;
				}
			}
		}
		double MaxInternal {
			set {
				if (Axis == null || double.IsNaN(value))
					return;
				if (NeedSwapMinMax(actualMinInternalValue, value)) {
					this.SetInternalMinMaxValues(value, actualMinInternalValue);
					return;
				}
				actualMaxInternalValue = value;
				double maxValue = CorrectDatetime(value);
				if (IsScrollingEnable()) {
					((VisualRangeData)Axis.VisualRangeData).MaxValueInternal = maxValue;
				}
				else {
					((WholeRangeData)Axis.WholeRangeData).MaxValueInternal = maxValue;
					((VisualRangeData)Axis.VisualRangeData).MaxValueInternal = maxValue;
				}
			}
		}
		bool SideMargins {
			set {
				if (Axis == null)
					return;
				if (!value)
					Axis.WholeRangeData.SideMarginsValue = 0;
				else {
					Axis.WholeRangeData.AutoSideMargins = SideMarginMode.Enable;
					Axis.VisualRangeData.AutoSideMargins = SideMarginMode.Enable;
				}
			}
		}
		bool AlwaysShowZeroLevel {
			set {
				if (Axis == null)
					return;
				Axis.WholeRangeData.AlwaysShowZeroLevel = value;
			}
		}
		bool IsScrollingRange {
			get {
				Axis2D axis2d = Axis as Axis2D;
				if (axis2d == null)
					return false;
#pragma warning disable 0618
				return axis2d.ScrollingRange == this;
#pragma warning restore 0618
			}
		}
		internal AxisBase Axis { get { return ((IChartElement)this).Owner as AxisBase; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisRangeMinValue"),
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
	DevExpressXpfChartsLocalizedDescription("AxisRangeMaxValue"),
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
	DevExpressXpfChartsLocalizedDescription("AxisRangeMinValueInternal"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double MinValueInternal {
			get { return (double)GetValue(MinValueInternalProperty); }
			set { SetValue(MinValueInternalProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisRangeMaxValueInternal"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double MaxValueInternal {
			get { return (double)GetValue(MaxValueInternalProperty); }
			set { SetValue(MaxValueInternalProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisRangeSideMarginsEnabled"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool SideMarginsEnabled {
			get { return (bool)GetValue(SideMarginsEnabledProperty); }
			set { SetValue(SideMarginsEnabledProperty, value); }
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("AxisRangeActualMinValue")]
#endif
		public object ActualMinValue { get { return actualMinValue; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("AxisRangeActualMaxValue")]
#endif
		public object ActualMaxValue { get { return actualMaxValue; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("AxisRangeActualMinValueInternal")]
#endif
		public double ActualMinValueInternal { get { return actualMinInternalValue; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("AxisRangeActualMaxValueInternal")]
#endif
		public double ActualMaxValueInternal { get { return actualMaxInternalValue; } }
		#region Hidden properties
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool IsEnabled { get { return base.IsEnabled; } set { base.IsEnabled = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool AllowDrop { get { return base.AllowDrop; } set { base.AllowDrop = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BindingGroup BindingGroup { get { return base.BindingGroup; } set { base.BindingGroup = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ContextMenu ContextMenu { get { return base.ContextMenu; } set { base.ContextMenu = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Cursor Cursor { get { return base.Cursor; } set { base.Cursor = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ForceCursor { get { return base.ForceCursor; } set { base.ForceCursor = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new InputScope InputScope { get { return base.InputScope; } set { base.InputScope = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool OverridesDefaultStyle { get { return base.OverridesDefaultStyle; } set { base.OverridesDefaultStyle = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style Style { get { return base.Style; } set { base.Style = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style FocusVisualStyle { get { return base.FocusVisualStyle; } set { base.FocusVisualStyle = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		#endregion
		#region IAxisRange implementation
		object IAxisRange.MinValue { get { return actualMinValue; } }
		object IAxisRange.MaxValue { get { return actualMaxValue; } }
		double IAxisRange.MinValueInternal { get { return MinValueInternal; } }
		double IAxisRange.MaxValueInternal { get { return MaxValueInternal; } }
		void IAxisRange.Assign(IAxisRange source) {
			locker.Lock();
			try {
				MinValue = source.MinValue;
				MaxValue = source.MaxValue;
				MinValueInternal = source.MinValueInternal;
				MaxValueInternal = source.MaxValueInternal;				
			}
			finally {
				locker.Unlock();
			}
		}		
		void IAxisRange.Assign(IAxisRangeData source) {
		}
		void IAxisRange.UpdateRange(object min, object max, double internalMin, double internalMax) {
			actualMaxValue = max;
			actualMinValue = min;
			actualMinInternalValue = internalMin;
			actualMaxInternalValue = internalMax;
		}
		#endregion
		internal void UpdateMinMaxValues(AxisBase axis) {
			if (Axis != null && Axis.WholeRangeData.CorrectionMode != RangeCorrectionMode.InternalValues && Axis.VisualRangeData.CorrectionMode != RangeCorrectionMode.InternalValues) {
				object min = CorrectValueProperty(axis, MinValue);
				if (!double.IsNaN(axis.ScaleMap.NativeToInternal(min)))
					Min = min;
				object max = CorrectValueProperty(axis, MaxValue);
				if (!double.IsNaN(axis.ScaleMap.NativeToInternal(max)))
					Max = max;
			}
			if (Axis != null && (Axis.WholeRangeData.CorrectionMode == RangeCorrectionMode.InternalValues || Axis.VisualRangeData.CorrectionMode == RangeCorrectionMode.InternalValues)) {
				SetMinInternal(MinValueInternal);
				SetMaxInternal(MaxValueInternal);
			}
		}
		void TryDisableSideMargins() {
			if (((IAxisData)Axis).AxisScaleTypeMap.ScaleType == ActualScaleType.Numerical) {
				Axis.WholeRangeData.SideMarginsValue = 0;
				Axis.VisualRangeData.SideMarginsValue = 0;
			}
		}
		void SetMinInternal(double value) {
			if (Axis == null || double.IsNaN(value))
				return;
			actualMinInternalValue = value;
			double minValue = CorrectDatetime(value);
			if (IsScrollingEnable()) {
				((IAxisRangeData)Axis.VisualRangeData).Min = minValue;
			}
			else {
				((IAxisRangeData)Axis.WholeRangeData).Min = minValue;
				((IAxisRangeData)Axis.VisualRangeData).Min = minValue;
			}
		}
		void SetMaxInternal(double value) {
			if (Axis == null || double.IsNaN(value))
				return;
			actualMaxInternalValue = value;
			double maxValue = CorrectDatetime(value);
			if (IsScrollingEnable()) {
				((IAxisRangeData)Axis.VisualRangeData).Max = maxValue;
			}
			else {
				((IAxisRangeData)Axis.WholeRangeData).Max = maxValue;
				((IAxisRangeData)Axis.VisualRangeData).Max = maxValue;
			}
		}
		bool IsScrollingEnable() {
			XYDiagram2D diagram = ((IChartElement)Axis).Owner as XYDiagram2D;
			if (diagram != null && diagram.AxisPaneContainer != null) {
				IList<IPane> panes = diagram.AxisPaneContainer.GetPanes(Axis);
				if (panes != null) {
					foreach (IPane pane in panes) {
						if (Axis.IsValuesAxis && ((Pane)pane).ActualEnableAxisYNavigation)
							return true;
						else if (!Axis.IsValuesAxis && ((Pane)pane).ActualEnableAxisXNavigation)
							return true;
					}
				}
				else {
					if (Axis.IsValuesAxis && diagram.EnableAxisYNavigation)
						return true;
					else if (!Axis.IsValuesAxis && diagram.EnableAxisXNavigation)
						return true;
				}
			}
			return false;
		}
		double CorrectDatetime(double value) {
			if (Axis.ScaleMap.ScaleType == ActualScaleType.DateTime)
				return value + ((AxisDateTimeMap)Axis.ScaleMap).Min;
			return value;
		}
		bool NeedSwapMinMax(double first, double second) {
			if (double.IsNaN(first) || double.IsNaN(second))
				return false;
			return first > second;
		}
		object ICloneable.Clone() {
			AxisRange range = new AxisRange();
#pragma warning disable 0618
			((IAxisRange)range).Assign(Axis.ActualRange);
#pragma warning restore 0618
			return range;
		}
		protected internal override void ChangeOwner(IChartElement oldOwner, IChartElement newOwner) {
			base.ChangeOwner(oldOwner, newOwner);
			object min = actualMinValue;
			object max = actualMaxValue;
			Max = max;
			Min = min;
			SideMargins = SideMarginsEnabled;
		}
		public void SetAuto() {
			ClearValue(MinValueProperty);
			ClearValue(MaxValueProperty);
			ClearValue(MinValueInternalProperty);
			ClearValue(MaxValueInternalProperty);
			Axis.WholeRangeData.CorrectionMode = RangeCorrectionMode.Auto;
			Axis.VisualRangeData.CorrectionMode = RangeCorrectionMode.Auto;
			Axis.WholeRangeData.AutoSideMargins = SideMarginMode.Enable;
			Axis.VisualRangeData.AutoSideMargins = SideMarginMode.Enable;
			PropertyChanged(this, new DependencyPropertyChangedEventArgs());
		}
		public void SetMinMaxValues(object minValue, object maxValue) {
			TryDisableSideMargins();
			if (IsScrollingEnable()) {
				Axis.VisualRangeData.SetRange(minValue, maxValue, double.NaN, double.NaN, true);
			}
			else {
				Axis.WholeRangeData.SetRange(minValue, maxValue, double.NaN, double.NaN, true);
				Axis.VisualRangeData.SetRange(minValue, maxValue, double.NaN, double.NaN, true);
			}
			MinValue = minValue;
			MaxValue = maxValue;
			ClearValue(MinValueInternalProperty);
			ClearValue(MaxValueInternalProperty);
		}
		public void SetInternalMinMaxValues(double min, double max) {
			actualMinInternalValue = min;
			actualMaxInternalValue = max;
			double minValue = CorrectDatetime(min);
			double maxValue = CorrectDatetime(max);
			TryDisableSideMargins();
			if (IsScrollingEnable()) {
				Axis.VisualRangeData.SetRange(null, null, minValue, maxValue, true);
			}
			else {
				Axis.WholeRangeData.SetRange(null, null, minValue, maxValue, true);
				Axis.VisualRangeData.SetRange(null, null, minValue, maxValue, true);
			}
		}
	}
}
