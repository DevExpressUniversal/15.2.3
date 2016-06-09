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
using System.Windows.Media;
using System.Windows.Media.Effects;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class Strip : ChartElement, IStrip, ILegendVisible, IInteractiveElement {
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(Strip), new PropertyMetadata(true, VisiblePropertyChanged));
		public static readonly DependencyProperty CheckedInLegendProperty = DependencyPropertyManager.Register("CheckedInLegend",
			typeof(bool), typeof(Strip), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty CheckableInLegendProperty = DependencyPropertyManager.Register("CheckableInLegend",
			typeof(bool), typeof(Strip), new PropertyMetadata(true));
		public static readonly DependencyProperty MinLimitProperty = DependencyPropertyManager.Register("MinLimit",
			typeof(object), typeof(Strip), new PropertyMetadata(null, MinLimitPropertyChanged));
		public static readonly DependencyProperty MaxLimitProperty = DependencyPropertyManager.Register("MaxLimit",
			typeof(object), typeof(Strip), new PropertyMetadata(null, MaxLimitPropertyChanged));
		public static readonly DependencyProperty BrushProperty = DependencyPropertyManager.Register("Brush",
			typeof(Brush), typeof(Strip));
		public static readonly DependencyProperty AxisLabelTextProperty = DependencyPropertyManager.Register("AxisLabelText", 
			typeof(string), typeof(Strip), new PropertyMetadata(String.Empty, ChartElementHelper.Update));
		public static readonly DependencyProperty LegendTextProperty = DependencyPropertyManager.Register("LegendText", 
			typeof(string), typeof(Strip), new PropertyMetadata(String.Empty, ChartElementHelper.Update));
		public static readonly DependencyProperty BorderColorProperty = DependencyPropertyManager.Register("BorderColor",
			typeof(Color), typeof(Strip));
		public static readonly DependencyProperty LegendMarkerTemplateProperty = DependencyPropertyManager.Register("LegendMarkerTemplate",
			typeof(DataTemplate), typeof(Strip));
		static void MinLimitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Strip strip = d as Strip;
			if (strip != null) {
				strip.UpdateMinLimitAxisValue(e.NewValue);
				ChartElementHelper.Update(d, new AxisElementUpdateInfo(strip, strip.Axis));
			}
		}
		static void MaxLimitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Strip strip = d as Strip;
			if (strip != null) {
				strip.UpdateMaxLimitAxisValue(e.NewValue);
				ChartElementHelper.Update(d, new AxisElementUpdateInfo(strip, strip.Axis));
			}
		}
		static void VisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Strip strip = d as Strip;
			if (strip != null)
				ChartElementHelper.Update(d, new AxisElementUpdateInfo(strip, strip.Axis));
		}
		SelectionInfo selectionInfo;
		readonly StripLimit minLimit;
		readonly StripLimit maxLimit;
		readonly List<StripItem> stripItems = new List<StripItem>();
		internal List<StripItem> StripItems {
			get { return stripItems; } 
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StripVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StripCheckedInLegend"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool CheckedInLegend {
			get { return (bool)GetValue(CheckedInLegendProperty); }
			set { SetValue(CheckedInLegendProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StripCheckableInLegend"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool CheckableInLegend {
			get { return (bool)GetValue(CheckableInLegendProperty); }
			set { SetValue(CheckableInLegendProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StripMinLimit"),
#endif
		Category(Categories.Behavior),
		TypeConverter(typeof(AxisValueTypeConverter)),
		XtraSerializableProperty
		]
		public object MinLimit {
			get { return GetValue(MinLimitProperty); }
			set { SetValue(MinLimitProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StripMaxLimit"),
#endif
		Category(Categories.Behavior),
		TypeConverter(typeof(AxisValueTypeConverter)),
		XtraSerializableProperty
		]
		public object MaxLimit {
			get { return GetValue(MaxLimitProperty); }
			set { SetValue(MaxLimitProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StripBrush"),
#endif
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush Brush {
			get { return (Brush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StripAxisLabelText"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public string AxisLabelText {
			get { return (string)GetValue(AxisLabelTextProperty); }
			set { SetValue(AxisLabelTextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StripLegendText"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public string LegendText {
			get { return (string)GetValue(LegendTextProperty); }
			set { SetValue(LegendTextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StripBorderColor"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public Color BorderColor {
			get { return (Color)GetValue(BorderColorProperty); }
			set { SetValue(BorderColorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StripLegendMarkerTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public DataTemplate LegendMarkerTemplate {
			get { return (DataTemplate)GetValue(LegendMarkerTemplateProperty); }
			set { SetValue(LegendMarkerTemplateProperty, value); }
		}
		internal Axis2D Axis { get { return ((IOwnedElement)this).Owner as Axis2D; } }
		#region Hidden properties
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush Background { get { return base.Background; } set { base.Background = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush BorderBrush { get { return base.BorderBrush; } set { base.BorderBrush = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness BorderThickness { get { return base.BorderThickness; } set { base.BorderThickness = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush Foreground { get { return base.Foreground; } set { base.Foreground = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness Padding { get { return base.Padding; } set { base.Padding = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Effect Effect { get { return base.Effect; } set { base.Effect = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform RenderTransform { get { return base.RenderTransform; } set { base.RenderTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Point RenderTransformOrigin { get { return base.RenderTransformOrigin; } set { base.RenderTransformOrigin = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Visibility Visibility { get { return base.Visibility; } set { base.Visibility = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Geometry Clip { get { return base.Clip; } set { base.Clip = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness Margin { get { return base.Margin; } set { base.Margin = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontFamily FontFamily { get { return base.FontFamily; } set { base.FontFamily = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double FontSize { get { return base.FontSize; } set { base.FontSize = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontStyle FontStyle { get { return base.FontStyle; } set { base.FontStyle = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontWeight FontWeight { get { return base.FontWeight; } set { base.FontWeight = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontStretch FontStretch { get { return base.FontStretch; } set { base.FontStretch = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform LayoutTransform { get { return base.LayoutTransform; } set { base.LayoutTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXBrowsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SelectionInfo SelectionInfo { get { return selectionInfo; } }
		#endregion
		Strip(object minLimitValue, object maxLimitValue) {
			DefaultStyleKey = typeof(Strip);
			minLimit = new StripLimit(this, Double.NegativeInfinity);
			maxLimit = new StripLimit(this, Double.PositiveInfinity);
			if (minLimitValue != null)
				MinLimit = minLimitValue;
			if (maxLimitValue != null)
				MaxLimit = maxLimitValue;
			selectionInfo = new SelectionInfo();
		}
		public Strip() : this(null, null) { 
		}
		public Strip(string minLimit, string maxLimit) : this((object)minLimit, (object)maxLimit) {
		}
		public Strip(DateTime minLimit, DateTime maxLimit) : this((object)minLimit, (object)maxLimit) {
		}
		public Strip(double minLimit, double maxLimit) : this((object)minLimit, (object)maxLimit) {
		}
		object ConvertValueWithAxisScaleMap(object value) {
			if ((value is string) && (string)value == String.Empty)
				return null;
			IAxisData axis = Axis;
			if (axis == null)
				return value;
			AxisScaleTypeMap map = axis.AxisScaleTypeMap;
			return map == null ? value : map.ConvertValue(value, CultureInfo.InvariantCulture);
		}
		void UpdateMinLimitAxisValue(object value) {
			minLimit.UpdateAxisValue(ConvertValueWithAxisScaleMap(value));
		}
		void UpdateMaxLimitAxisValue(object value) {
			maxLimit.UpdateAxisValue(ConvertValueWithAxisScaleMap(value));
		}
		internal void UpdateMinMaxLimits() {
			UpdateMinLimitAxisValue(MinLimit);
			UpdateMaxLimitAxisValue(MaxLimit);
		}
		internal bool GetActualVisible() {
			if (Visible) {
				if (Axis != null && Axis.Diagram2D != null && Axis.Diagram2D.ChartControl != null && Axis.Diagram2D.ChartControl.LegendUseCheckBoxes)
					return !CheckableInLegend || CheckedInLegend;
				return true;
			}
			return false;
		}
		#region IStrip implementation
		IStripLimit IStrip.MinLimit { get { return minLimit; } }
		IStripLimit IStrip.MaxLimit { get { return maxLimit; } }
		void IStrip.CorrectLimits() {}
		bool IStrip.Visible { get { return GetActualVisible(); } }
		#endregion
		#region IInteractiveElement implementation
		bool IInteractiveElement.IsHighlighted {
			get { return selectionInfo.IsHighlighted; }
			set { selectionInfo.IsHighlighted = value; }
		}
		bool IInteractiveElement.IsSelected {
			get { return selectionInfo.IsSelected; }
			set { selectionInfo.IsSelected = value; }
		}
		object IInteractiveElement.Content { get { return this; } }
		#endregion
		#region ILegendVisible implementation
		DataTemplate ILegendVisible.LegendMarkerTemplate { get { return LegendMarkerTemplate; } }
		bool ILegendVisible.CheckedInLegend { get { return CheckedInLegend; } }
		bool ILegendVisible.CheckableInLegend { get { return CheckableInLegend; } }
		#endregion
	}
	public class StripCollection : ChartElementCollection<Strip>, IEnumerable<IStrip>  {
		protected override ChartElementChange Change { get { return base.Change | ChartElementChange.UpdateXYDiagram2DItems ; } }
		IEnumerator<IStrip> IEnumerable<IStrip>.GetEnumerator() {
			foreach (IStrip strip in this)
				yield return strip;
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(System.Collections.Specialized.NotifyCollectionChangedAction action, System.Collections.IList newItems, System.Collections.IList oldItems, int newStartingIndex, int oldStartingIndex) {
			return new AxisElementUpdateInfo(this, (AxisBase)Owner);
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public class StripLimit : IStripLimit {
		readonly Strip strip;
		readonly double infinity;
		object axisValue;
		double value = Double.NaN;
		bool IsEnabled { get { return axisValue != null; } }
		public object AxisValue { get { return axisValue; } }
		public double Value { get { return (IsEnabled && !Double.IsNaN(value)) ? value : infinity; } }
		public StripLimit(Strip strip, double infinity) {
			this.strip = strip;
			this.infinity = infinity;
		}
		public void UpdateAxisValue(object axisValue) {
			this.axisValue = axisValue;
			if (axisValue == null)
				value = Double.NaN;
		}
		#region IAxisValueContainer implementation
		IAxisData IAxisValueContainer.Axis { get { return strip.Axis; } }
		bool IAxisValueContainer.IsEnabled { get { return IsEnabled; } }
		CultureInfo IAxisValueContainer.Culture { get { return CultureInfo.InvariantCulture; } }
		object IAxisValueContainer.GetAxisValue() { return axisValue; }
		void IAxisValueContainer.SetAxisValue(object axisValue) { this.axisValue = axisValue; }
		double IAxisValueContainer.GetValue() { return value; }
		void IAxisValueContainer.SetValue(double value) { this.value = value; }
		#endregion
	}
}
