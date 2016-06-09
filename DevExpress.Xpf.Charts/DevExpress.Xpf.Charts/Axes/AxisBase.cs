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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum DateTimeMeasurementUnit {
		Millisecond = DateTimeMeasureUnitNative.Millisecond,
		Second = DateTimeMeasureUnitNative.Second,
		Minute = DateTimeMeasureUnitNative.Minute,
		Hour = DateTimeMeasureUnitNative.Hour,
		Day = DateTimeMeasureUnitNative.Day,
		Week = DateTimeMeasureUnitNative.Week,
		Month = DateTimeMeasureUnitNative.Month,
		Quarter = DateTimeMeasureUnitNative.Quarter,
		Year = DateTimeMeasureUnitNative.Year
	}
	public abstract class AxisBase : ChartElement, IAxisElementContainer, IWeakEventListener, IInteractiveElement, IAxisData {
		static readonly DependencyPropertyKey ActualGridLinesLineStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGridLinesLineStyle",
			typeof(LineStyle), typeof(AxisBase), new PropertyMetadata());
		static readonly DependencyPropertyKey ActualGridLinesMinorLineStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGridLinesMinorLineStyle",
			typeof(LineStyle), typeof(AxisBase), new PropertyMetadata());
		public static readonly DependencyProperty ActualGridLinesLineStyleProperty = ActualGridLinesLineStylePropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualGridLinesMinorLineStyleProperty = ActualGridLinesMinorLineStylePropertyKey.DependencyProperty;
		public static readonly DependencyProperty MinorCountProperty = DependencyPropertyManager.Register("MinorCount", typeof(int),
			typeof(AxisBase), new PropertyMetadata(4, ChartElementHelper.UpdateWithClearDiagramCache), new ValidateValueCallback(ValidateMinorCount));
		[Obsolete(ObsoleteMessages.AxisNumericOptionsProperty)]
		public static readonly DependencyProperty NumericOptionsProperty = DependencyPropertyManager.Register("NumericOptions",
			typeof(NumericOptions), typeof(AxisBase), new PropertyMetadata(NumericOptionsPropertyChanged));
		public static readonly DependencyProperty GridLinesVisibleProperty = DependencyPropertyManager.Register("GridLinesVisible",
			typeof(bool), typeof(AxisBase), new PropertyMetadata(ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty GridLinesMinorVisibleProperty = DependencyPropertyManager.Register("GridLinesMinorVisible",
			typeof(bool), typeof(AxisBase), new PropertyMetadata(ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty GridLinesLineStyleProperty = DependencyPropertyManager.Register("GridLinesLineStyle",
			typeof(LineStyle), typeof(AxisBase), new PropertyMetadata(GridLinesLineStylePropertyChanged));
		public static readonly DependencyProperty GridLinesMinorLineStyleProperty = DependencyPropertyManager.Register("GridLinesMinorLineStyle",
			typeof(LineStyle), typeof(AxisBase), new PropertyMetadata(GridLinesMinorLineStylePropertyChanged));
		public static readonly DependencyProperty GridLinesBrushProperty = DependencyPropertyManager.Register("GridLinesBrush",
			typeof(Brush), typeof(AxisBase), new PropertyMetadata(ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty GridLinesMinorBrushProperty = DependencyPropertyManager.Register("GridLinesMinorBrush",
			typeof(Brush), typeof(AxisBase), new PropertyMetadata(ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty InterlacedProperty = DependencyPropertyManager.Register("Interlaced", typeof(bool),
			typeof(AxisBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty InterlacedBrushProperty = DependencyPropertyManager.Register("InterlacedBrush", typeof(Brush),
			typeof(AxisBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty LabelProperty = DependencyPropertyManager.Register("Label", typeof(AxisLabel),
			typeof(AxisBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, LabelPropertyChanged));
		public static readonly DependencyProperty QualitativeScaleComparerProperty = DependencyPropertyManager.Register("QualitativeScaleComparer", typeof(IComparer),
			typeof(AxisBase), new FrameworkPropertyMetadata(null, QualitativeScaleComparerPropertyChanged));
		static bool ValidateMinorCount(object value) {
			int minorCount = (int)value;
			return minorCount > 0 && minorCount < 100;
		}
		static void NumericOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisBase axis = d as AxisBase;
			if (axis != null) {
				axis.numericOptions = (NumericOptions)e.NewValue;
				axis.UpdateLabelTextPattern();
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as NumericOptions, e.NewValue as NumericOptions, axis);
			}
			ChartElementHelper.Update(d, e);
		}
		static void GridLinesLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisBase axis = d as AxisBase;
			if (axis != null) {
				LineStyle newLineStyle = e.NewValue as LineStyle;
				axis.SetValue(ActualGridLinesLineStylePropertyKey, newLineStyle == null ? new LineStyle() : newLineStyle);
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as LineStyle, e.NewValue as LineStyle, axis);
			}
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		static void GridLinesMinorLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisBase axis = d as AxisBase;
			if (axis != null) {
				LineStyle newLineStyle = e.NewValue as LineStyle;
				axis.SetValue(ActualGridLinesMinorLineStylePropertyKey, newLineStyle == null ? new LineStyle() : newLineStyle);
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as LineStyle, e.NewValue as LineStyle, axis);
			}
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		static void LabelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisBase axis = d as AxisBase;
			if (axis != null) {
				AxisLabel label = e.NewValue as AxisLabel;
				if (label == null) {
					axis.actualLabel = new AxisLabel();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, axis.actualLabel);
				}
				else
					axis.actualLabel = label;
				axis.LabelPropertyChanged();
			}
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateXYDiagram2DItems);
		}
		static void QualitativeScaleComparerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisBase axis = d as AxisBase;
			if (axis != null)
				ChartElementHelper.Update(d, new PropertyUpdateInfo(d, e.Property.Name));
		}
		protected static void NumericScaleOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisBase axis = d as AxisBase;
			if (axis != null && e.NewValue != e.OldValue)
				axis.SetActualNumericScaleOptions(d, e);
		}
		protected static void DateTimeScaleOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisBase axis = d as AxisBase;
			if (axis != null)
				axis.SetActualDateTimeScaleOptions(d, e);
		}
		static void MeasureUnitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisBase axis = d as AxisBase;
			if (axis != null)
				axis.ActualDateTimeMeasureUnit = (DateTimeMeasurementUnit)e.NewValue;
			ChartElementHelper.Update(d, e);
		}
		readonly QualitativeScaleOptions qualitativeScaleOptions;
		AxisLabel actualLabel;
		Scale scale;
		[Obsolete]
		AxisRange actualRange;
		SelectionInfo selectionInfo;
		AxisScaleTypeMap scaleMap;
		NumericOptions numericOptions;
		protected Range actualWholeRangeValue;
		NumericScaleOptionsBase actualNumericScaleOptions;
		DateTimeScaleOptionsBase actualDateTimeScaleOptions;
		[Browsable(false), Description("")]
		public Range ActualWholeRange { get { return actualWholeRangeValue; } protected set { actualWholeRangeValue = value; } }
		[
		Browsable(false), 
		Obsolete(ObsoleteMessages.ActualRangeProperty),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public AxisRange ActualRange { get { return actualRange; } protected set { actualRange = value; } }
		public AxisLabel ActualLabel { get { return actualLabel; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public LineStyle ActualGridLinesLineStyle { get { return (LineStyle)GetValue(ActualGridLinesLineStyleProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public LineStyle ActualGridLinesMinorLineStyle { get { return (LineStyle)GetValue(ActualGridLinesMinorLineStyleProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXBrowsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SelectionInfo SelectionInfo { get { return selectionInfo; } }
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int MinorCount {
			get { return (int)GetValue(MinorCountProperty); }
			set { SetValue(MinorCountProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.AxisNumericOptionsProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public NumericOptions NumericOptions {
			get { return (NumericOptions)GetValue(NumericOptionsProperty); }
			set { SetValue(NumericOptionsProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool GridLinesVisible {
			get { return (bool)GetValue(GridLinesVisibleProperty); }
			set { SetValue(GridLinesVisibleProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool GridLinesMinorVisible {
			get { return (bool)GetValue(GridLinesMinorVisibleProperty); }
			set { SetValue(GridLinesMinorVisibleProperty, value); }
		}
		[
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public LineStyle GridLinesLineStyle {
			get { return (LineStyle)GetValue(GridLinesLineStyleProperty); }
			set { SetValue(GridLinesLineStyleProperty, value); }
		}
		[
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public LineStyle GridLinesMinorLineStyle {
			get { return (LineStyle)GetValue(GridLinesMinorLineStyleProperty); }
			set { SetValue(GridLinesMinorLineStyleProperty, value); }
		}
		[
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush GridLinesBrush {
			get { return (Brush)GetValue(GridLinesBrushProperty); }
			set { SetValue(GridLinesBrushProperty, value); }
		}
		[
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush GridLinesMinorBrush {
			get { return (Brush)GetValue(GridLinesMinorBrushProperty); }
			set { SetValue(GridLinesMinorBrushProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Interlaced {
			get { return (bool)GetValue(InterlacedProperty); }
			set { SetValue(InterlacedProperty, value); }
		}
		[
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush InterlacedBrush {
			get { return (Brush)GetValue(InterlacedBrushProperty); }
			set { SetValue(InterlacedBrushProperty, value); }
		}
		[
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public AxisLabel Label {
			get { return (AxisLabel)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}
		[
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public IComparer QualitativeScaleComparer {
			get { return (IComparer)GetValue(QualitativeScaleComparerProperty); }
			set { SetValue(QualitativeScaleComparerProperty, value); }
		}
		XYDiagram2D XYDiagram2D { get { return ((IChartElement)this).Owner as XYDiagram2D; } }
		internal Diagram Diagram { get { return ((IChartElement)this).Owner as Diagram; } }
		internal VisualRangeData VisualRangeData {
			get {
				if (visualRangeData == null)
					CreateVisualRangeData();
				return visualRangeData;
			}
		}
		internal WholeRangeData WholeRangeData {
			get {
				if (wholeRangeData == null)
					CreateWholeRangeData();
				return wholeRangeData;
			}
		}
		internal AxisScaleTypeMap ScaleMap { get { return scaleMap; } }
		internal ScaleType ScaleType { get { return (ScaleType)scale; } }
		internal NumericOptions NumericOptionsInternal { get { return numericOptions; } }
		[Obsolete]
		protected virtual AxisRange ScrollingRangeValue { get { return actualRange; } }
		protected internal abstract bool IsValuesAxis { get; }
		protected internal abstract bool IsVertical { get; }
		protected internal abstract bool IsReversed { get; }
		protected internal abstract DateTimeOptions DateTimeOptionsImpl { get; }
		protected virtual IEnumerable<IStrip> StripsImpl { get { return null; } }
		protected virtual IEnumerable<IConstantLine> ConstantLinesImpl { get { return null; } }
		protected virtual IEnumerable<ICustomAxisLabel> CustomLabelsImpl { get { return null; } }
		protected virtual DateTimeScaleOptionsBase DefaultDateTimeScaleOptions { get { return null; } }
		protected virtual NumericScaleOptionsBase DefaultNumericScaleOptions { get { return null; } }
		protected virtual bool CanShowCustomWithAutoLabels { get { return false; } }
		protected internal virtual double GridSpacingImpl { get { return Double.NaN; } }
		protected abstract int GridSpacingFactor { get; }
		protected internal virtual AxisVisibilityInPanes ActualVisibilityInPanes { get { return null; } }
		protected virtual bool IsFixedRange { get { return false; } }
		protected double ActualGridSpacing {
			get {
				return ((INumericScaleOptions)ActualNumericScaleOptions).GridSpacing;
			}
			set {
				ActualNumericScaleOptions.ResetAutoGrid(value);
				if (ActualDateTimeScaleOptions != null)
					ActualDateTimeScaleOptions.ResetAutoGrid(value);
			}
		}
		protected DateTimeMeasurementUnit ActualDateTimeMeasureUnit {
			get {
				if (ActualDateTimeScaleOptions != null)
					return (DateTimeMeasurementUnit)((IDateTimeScaleOptions)ActualDateTimeScaleOptions).MeasureUnit;
				else
					return DateTimeMeasurementUnit.Day;
			}
			set {
				if (!(actualDateTimeScaleOptions is ManualDateTimeScaleOptions)) {
					actualDateTimeScaleOptions.SetOwner(null);
					actualDateTimeScaleOptions = DefaultDateTimeScaleOptions;
					actualDateTimeScaleOptions.SetOwner(this);
				}
				ManualDateTimeScaleOptions manualOptions = actualDateTimeScaleOptions as ManualDateTimeScaleOptions;
				if (manualOptions != null)
					manualOptions.MeasureUnit = (DateTimeMeasureUnit)value;
				ChartElementHelper.Update((IChartElement)this, new DataAggregationUpdate(this));
			}
		}
		protected DateTimeMeasurementUnit ActualGridAlignment {
			get {
				if (actualDateTimeScaleOptions != null)
					return (DateTimeMeasurementUnit)((IDateTimeScaleOptions)actualDateTimeScaleOptions).GridAlignment;
				else
					return DateTimeMeasurementUnit.Day;
			}
			set {
				if (actualDateTimeScaleOptions == null)
					actualDateTimeScaleOptions = DefaultDateTimeScaleOptions;
				if (actualDateTimeScaleOptions != null)
					actualDateTimeScaleOptions.SetGridAlignment((DateTimeGridAlignment)value);
			}
		}
		internal DateTimeScaleOptionsBase ActualDateTimeScaleOptions { get { return actualDateTimeScaleOptions; } }
		internal NumericScaleOptionsBase ActualNumericScaleOptions { get { return actualNumericScaleOptions; } }
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
		public new double Opacity { get { return base.Opacity; } set { base.Opacity = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush OpacityMask { get { return base.OpacityMask; } set { base.OpacityMask = value; } }
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
		public new Transform LayoutTransform { get { return base.LayoutTransform; } set { base.LayoutTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		#endregion
		public AxisBase() {
			BeginInit();
			selectionInfo = new SelectionInfo();
#pragma warning disable 0612, 0618
			actualRange = new AxisRange();
			actualRange.ChangeOwner(null, this);
#pragma warning restore 0612, 0618
			actualLabel = new AxisLabel();
			actualLabel.ChangeOwner(null, this);
			scale = Scale.Numerical;
			scaleMap = CreateScaleMap(ScaleType, null);
			this.SetValue(ActualGridLinesLineStylePropertyKey, new LineStyle());
			this.SetValue(ActualGridLinesMinorLineStylePropertyKey, new LineStyle());
			actualDateTimeScaleOptions = DefaultDateTimeScaleOptions != null ? DefaultDateTimeScaleOptions : new ManualDateTimeScaleOptions();
			actualDateTimeScaleOptions.SetOwner(this);
			actualNumericScaleOptions = DefaultNumericScaleOptions;
			actualNumericScaleOptions.SetOwner(this);
			qualitativeScaleOptions = new QualitativeScaleOptions(this);
			EndInit();
		}
		#region IAxisElementContainer implementation
		IEnumerable<IScaleBreak> IAxisElementContainer.ScaleBreaks { get { return null; } }
		IEnumerable<IConstantLine> IAxisElementContainer.ConstantLines { get { return ConstantLinesImpl; } }
		IEnumerable<IStrip> IAxisElementContainer.Strips { get { return StripsImpl; } }
		IEnumerable<ICustomAxisLabel> IAxisElementContainer.CustomLabels { get { return CustomLabelsImpl; } }
		#endregion
		#region IAxisData implementation
		AxisVisibilityInPanes IAxisData.AxisVisibilityInPanes { get { return ActualVisibilityInPanes; } }
		AxisScaleTypeMap IAxisData.AxisScaleTypeMap {
			get { return scaleMap; }
			set {
				scaleMap = value;
				ActualLabel.SyncronizeTextPatterWithScaleType();
			}
		}
		void IAxisData.Deserialize() {
			Deserialize();
		}
		void IAxisData.UpdateUserValues() {
			UpdateUserValues();
		}
		bool IAxisData.FixedRange {
			get { return IsFixedRange; }
		}
		IAxisGridMapping IAxisData.GridMapping {
			get {
				if (scaleMap is AxisDateTimeMap)
					return new AxisDateTimeGridMapping((AxisDateTimeMap)scaleMap, ((IDateTimeScaleOptions)ActualDateTimeScaleOptions).GridAlignment, ((IDateTimeScaleOptions)ActualDateTimeScaleOptions).GridOffset);
				else if (scaleMap is AxisNumericalMap)
					return new AxisNumericGridMapping((AxisNumericalMap)scaleMap, ((INumericScaleOptions)ActualNumericScaleOptions).GridAlignment, ((INumericScaleOptions)ActualNumericScaleOptions).GridOffset);
				else if (scaleMap is AxisQualitativeMap)
					return new AxisQualitativeGridMapping((AxisQualitativeMap)scaleMap, qualitativeScaleOptions.GridOffset);
				return null;
			}
		}
		int IAxisData.GridSpacingFactor { get { return GridSpacingFactor; } }
		bool IAxisData.Interlaced { get { return Interlaced; } }
		bool IAxisData.IsArgumentAxis { get { return !IsValuesAxis; } }
		bool IAxisData.IsRadarAxis { get { return this is RadarAxisX2D; } }
		bool IAxisData.IsValueAxis { get { return IsValuesAxis; } }
		bool IAxisData.IsVertical { get { return IsVertical; } }
		bool IAxisData.CanShowCustomWithAutoLabels { get { return CanShowCustomWithAutoLabels; } } 
		IAxisLabel IAxisData.Label { get { return this.ActualLabel; } }
		IAxisLabelFormatterCore IAxisData.LabelFormatter {
			get { return ActualLabel != null ? ActualLabel.Formatter : null; }
			set { ActualLabel.Formatter = (IAxisLabelFormatter)value; }
		}
		int IAxisData.MinorCount { get { return this.MinorCount; } }
		INumericScaleOptions IAxisData.NumericScaleOptions { get { return ActualNumericScaleOptions; } }
		IDateTimeScaleOptions IAxisData.DateTimeScaleOptions { get { return ActualDateTimeScaleOptions; } }
		IScaleOptionsBase IAxisData.QualitativeScaleOptions { get { return qualitativeScaleOptions; } }
		bool IAxisData.Reverse { get { return IsReversed; } }
		bool IAxisData.ShowLabels { get { return ActualLabel.Visible; } }
		bool IAxisData.ShowMajorGridlines { get { return true; } }
		bool IAxisData.ShowMajorTickmarks { get { return true; } }
		bool IAxisData.ShowMinorGridlines { get { return true; } }
		bool IAxisData.ShowMinorTickmarks { get { return true; } }
		IAxisTitle IAxisData.Title { get { return null; } }
		Range visualRange;
		Range wholeRange;
		VisualRangeData visualRangeData;
		WholeRangeData wholeRangeData;
		IAxisRange IAxisData.Range {
			get {
#pragma warning disable 0612
				return actualRange;
#pragma warning restore 0612
			}
		}
		IAxisRange IAxisData.ScrollingRange {
			get {
#pragma warning disable 0612
				return ScrollingRangeValue;
#pragma warning restore 0612
			}
		}
		IVisualAxisRangeData IAxisData.VisualRange { get { return VisualRangeData; } }
		IWholeAxisRangeData IAxisData.WholeRange { get { return WholeRangeData; } }
		void IAxisData.UpdateAutoMeasureUnit() { UpdateAutoMeasureUnit(); }
		IList<IMinMaxValues> IAxisData.CalculateRangeLimitsList(double min, double max) {
			return null;
		}
		RangeValue IAxisData.IncreaseRange(RangeValue range, bool applySideMargins) { return range; }
		GRealRect2D IAxisData.GetLabelBounds(IPane pane) { return this.GetLabelBounds(pane as Pane); }
		bool IAxisData.IsDisposed { get { return false; } }
		#endregion
		#region IInteractiveElement implementation
		bool IInteractiveElement.IsHighlighted {
			get { return selectionInfo.IsHighlighted; }
			set {
				selectionInfo.IsHighlighted = value;
				IsHighlightedChanged();
			}
		}
		bool IInteractiveElement.IsSelected {
			get { return selectionInfo.IsSelected; }
			set {
				selectionInfo.IsSelected = value;
				IsSelectedChanged();
			}
		}
		object IInteractiveElement.Content { get { return this; } }
		#endregion
		void CreateVisualRangeData() {
			visualRangeData = new VisualRangeData(this);
			visualRange = new Range();
			visualRange.SetRangeData(visualRangeData);
			visualRange.ChangeOwner(null, this);
		}
		void CreateWholeRangeData() {
			wholeRangeData = new WholeRangeData(this);
			wholeRange = new Range();
			wholeRange.SetRangeData(wholeRangeData);
			wholeRange.ChangeOwner(null, this);
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return managerType == typeof(PropertyChangedWeakEventManager) && ProcessWeakEvent(sender);
		}
		AxisScaleTypeMap CreateScaleMap(ScaleType scaleType, List<string> values) {
			IAxisData axisData = (IAxisData)this;
			switch (scaleType) {
				case ScaleType.DateTime:
					return new AxisDateTimeMap((DateTimeMeasureUnitNative)axisData.DateTimeScaleOptions.MeasureUnit, axisData.DateTimeScaleOptions.WorkdaysOptions);
				case ScaleType.Qualitative:
					List<object> qualitativeValues = new List<object>();
					foreach (var val in values)
						qualitativeValues.Add(val);
					return new AxisQualitativeMap(qualitativeValues);
				case ScaleType.Numerical:
				default:
					return new AxisNumericalMap();
			}
		}
		MeasureUnitsCalculatorBase GetMeasureUnitsCalculator() {
			if (ScaleMap == null || ScaleMap.ScaleType == ActualScaleType.Qualitative)
				return null;
			if (ScaleMap.ScaleType == ActualScaleType.DateTime)
				return ActualDateTimeScaleOptions.DateTimeMeasureUnitsCalculatorCore;
			return ActualNumericScaleOptions.NumericMeasureUnitsCalculatorCore;
		}
		protected virtual void ScaleTypeUpdated() {
#pragma warning disable 0612
			if (actualRange != null)
				actualRange.UpdateMinMaxValues(this);
#pragma warning restore 0612
		}
		protected virtual void LabelPropertyChanged() {
		}
		protected virtual GRealRect2D GetLabelBounds(Pane pane) {
			return GRealRect2D.Empty;
		}
		protected internal virtual bool GetActualAlwaysShowZeroLevel(ChartNonVisualElement range) {
			return false;
		}
		protected internal virtual void IsHighlightedChanged() {
		}
		protected internal virtual void IsSelectedChanged() {
		}
		protected internal virtual void Deserialize() { }
		protected internal virtual void UpdateUserValues() { }
		protected virtual bool ProcessWeakEvent(object sender) {
			if ((sender is NumericOptions) || (sender is DateTimeOptions)) {
				UpdateLabelTextPattern();
				ChartElementHelper.Update(this);
				return true;
			}
			if (sender is LineStyle) {
				ChartElementHelper.UpdateWithClearDiagramCache(this);
				return true;
			}
			return false;
		}
		protected internal abstract GridLineGeometry CreateGridLineGeometry(Rect axisBounds, IAxisMapping mapping, double axisValue, int thickness);
		protected internal abstract InterlaceGeometry CreateInterlaceGeometry(Rect axisBounds, IAxisMapping mapping, double nearAxisValue, double farAxisValue);
		protected internal abstract IAxisMapping CreateMapping(Rect bounds);
		protected internal void UpdateAutoMeasureUnit() {
			XYDiagram2D diagram = XYDiagram2D;
			if (diagram != null) {
				double axisLength = diagram.GetAxisLength(this);
				if (UpdateAutomaticMeasureUnit(axisLength))
					ChartElementHelper.Update((IChartElement)this, new DataAggregationUpdate(this));
			}
		}
		protected internal bool UpdateAutomaticMeasureUnit(double axisLength) {
			IList<ISeries> seriesList = GetSeries();
			MeasureUnitsCalculatorBase calculator = GetMeasureUnitsCalculator();
			return calculator != null && seriesList != null ? calculator.UpdateAutomaticMeasureUnit(axisLength, seriesList) : false;
		}
		protected internal void UpdateLabelTextPattern() {
			actualLabel.UpdateTextPattern();
		}
		protected internal virtual void CreateMediator() { }
		void SetActualNumericScaleOptions(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			NumericScaleOptionsBase newScaleOptions = e.NewValue as NumericScaleOptionsBase;
			NumericScaleOptionsBase oldScaleOptions = actualNumericScaleOptions;
			if (newScaleOptions == null)
				actualNumericScaleOptions = DefaultNumericScaleOptions;
			else
				actualNumericScaleOptions = newScaleOptions;
			e = new DependencyPropertyChangedEventArgs(e.Property, oldScaleOptions, actualNumericScaleOptions);
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, new DataAggregationUpdate(this));
			if (XYDiagram2D != null)
				XYDiagram2D.RaiseAxisScaleChangedEvent(this, oldScaleOptions, actualNumericScaleOptions);
		}
		void SetActualDateTimeScaleOptions(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DateTimeScaleOptionsBase newScaleOptions = e.NewValue as DateTimeScaleOptionsBase;
			DateTimeScaleOptionsBase oldScaleOptions = actualDateTimeScaleOptions;
			if (newScaleOptions == null) {
				if (DefaultDateTimeScaleOptions != null)
					actualDateTimeScaleOptions = DefaultDateTimeScaleOptions;
				else
					actualDateTimeScaleOptions = new ManualDateTimeScaleOptions();
			}
			else
				actualDateTimeScaleOptions = newScaleOptions;
			e = new DependencyPropertyChangedEventArgs(e.Property, oldScaleOptions, actualDateTimeScaleOptions);
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, new DataAggregationUpdate(this));
			if (XYDiagram2D != null)
				XYDiagram2D.RaiseAxisScaleChangedEvent(this, oldScaleOptions, actualDateTimeScaleOptions);
		}
		internal void UpdateAxisValueContainers() {
			IAxisElementContainer axis = this;
			if (axis.ScaleBreaks != null)
				foreach (IScaleBreak scaleBreak in axis.ScaleBreaks) {
					AxisValueContainerHelper.UpdateAxisValue(scaleBreak.Edge1, scaleMap);
					AxisValueContainerHelper.UpdateAxisValue(scaleBreak.Edge2, scaleMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(scaleBreak.Edge1, scaleMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(scaleBreak.Edge2, scaleMap);
				}
			if (axis.Strips != null)
				foreach (IStrip strip in axis.Strips) {
					AxisValueContainerHelper.UpdateAxisValue(strip.MinLimit, strip.MaxLimit, scaleMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(strip.MinLimit, scaleMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(strip.MaxLimit, scaleMap);
					strip.CorrectLimits();
				}
			if (axis.ConstantLines != null)
				foreach (IConstantLine line in axis.ConstantLines) {
					AxisValueContainerHelper.UpdateAxisValue(line, scaleMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(line, scaleMap);
				}
			if (axis.CustomLabels != null)
				foreach (ICustomAxisLabel label in axis.CustomLabels) {
					AxisValueContainerHelper.UpdateAxisValue(label, scaleMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(label, scaleMap);
				}
		}
		internal string[] GetAvailablePatternPlaceholders() {
			return IsValuesAxis ? new string[1] { ToolTipPatternUtils.ValuePattern } : new string[1] { ToolTipPatternUtils.ArgumentPattern };
		}
		internal IList<ISeries> GetSeries() {
			Diagram diagram = Diagram;
			if (diagram == null)
				return null;
			return diagram.ViewController.GetSeriesByAxis(this);
		}
		public object GetScaleValueFromInternal(double internalValue) {
			return scaleMap.InternalToNative(internalValue);
		}
	}
}
