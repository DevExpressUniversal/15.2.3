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
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum DateTimeMeasureUnit {
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
	public enum DateTimeGridAlignment {
		Millisecond = DateTimeGridAlignmentNative.Millisecond,
		Second = DateTimeGridAlignmentNative.Second,
		Minute = DateTimeGridAlignmentNative.Minute,
		Hour = DateTimeGridAlignmentNative.Hour,
		Day = DateTimeGridAlignmentNative.Day,
		Week = DateTimeGridAlignmentNative.Week,
		Month = DateTimeGridAlignmentNative.Month,
		Quarter = DateTimeGridAlignmentNative.Quarter,
		Year = DateTimeGridAlignmentNative.Year
	}
	public enum AggregateFunction {
		None = AggregateFunctionNative.None,
		Average = AggregateFunctionNative.Average,
		Minimum = AggregateFunctionNative.Minimum,
		Maximum = AggregateFunctionNative.Maximum,
		Sum = AggregateFunctionNative.Sum,
		Count = AggregateFunctionNative.Count,
		Financial = AggregateFunctionNative.Financial,
	}
	public abstract class DateTimeScaleOptionsBase : ScaleOptionsBase, IDateTimeScaleOptions {
		internal const double LabelLength = 120;
		double autoGridSpacing = DateTimeUnitSelector.DefaultGridSpacing;
		DateTimeMeasureUnitNative autoMeasureUnit = DateTimeUnitSelector.DefaultMeasureUnit;
		DateTimeGridAlignmentNative autoGridAlignment = DateTimeUnitSelector.DefaultGridAlignment;
		DateTimeMeasureUnitsCalculatorCore dateTimeMeasureUnitsCalculatorCore;
		bool NeedAutoGridAlignment { get { return (ScaleModeImp == ScaleModeNative.Automatic) || AutoGridImp; } }
		internal DateTimeMeasureUnitsCalculatorCore DateTimeMeasureUnitsCalculatorCore {
			get {
				if (dateTimeMeasureUnitsCalculatorCore == null && Axis != null)
					dateTimeMeasureUnitsCalculatorCore = new DateTimeMeasureUnitsCalculatorCore(Axis, DateTimeScaleOptionsBase.LabelLength);
				return dateTimeMeasureUnitsCalculatorCore;
			}
		}
		protected double AutoGridSpacing { get { return autoGridSpacing; } }
		protected abstract DateTimeMeasureUnit MeasureUnitImp { get; }
		protected abstract DateTimeGridAlignment GridAlignmentImp { get; }
		protected override bool GridSpasingAutoImp { get { return false; } }
		protected virtual bool UseCustomMeasureUnit { get { return false; } }
		#region IDateTimeScaleOptions
		DateTimeGridAlignmentNative IDateTimeScaleOptions.GridAlignment { get { return (DateTimeGridAlignmentNative)GetActualGridAlignment(); } }
		DateTimeMeasureUnitNative IScaleOptionsBase<DateTimeMeasureUnitNative>.MeasureUnit { get { return (DateTimeMeasureUnitNative)GetActualMeasureUnit(); } }
		IWorkdaysOptions IDateTimeScaleOptions.WorkdaysOptions { get { return null; } }
		DateTimeMeasureUnitsCalculatorCore IDateTimeScaleOptions.Calculator { get { return DateTimeMeasureUnitsCalculatorCore; } }
		bool IScaleOptionsBase<DateTimeMeasureUnitNative>.UseCustomMeasureUnit { get { return UseCustomMeasureUnit; } }
		bool IDateTimeScaleOptions.UpdateAutomaticUnits(DateTimeMeasureUnitNative measureUnit, DateTimeGridAlignmentNative gridAlignment, double spacing) {
			ValueChangeInfo<DateTimeMeasureUnit> measureUnitChange = null;
			ValueChangeInfo<DateTimeGridAlignment> gridAlignmentChange = null;
			if (ScaleModeImp == ScaleModeNative.Automatic) {
				if (autoMeasureUnit != measureUnit) {
					measureUnitChange = CreateInfo((DateTimeMeasureUnit)autoMeasureUnit, (DateTimeMeasureUnit)measureUnit);
					autoMeasureUnit = measureUnit;
				}
			}
			if (NeedAutoGridAlignment) {
				DateTimeMeasureUnit actualMeasureUnit = GetActualMeasureUnit();
				if ((ScaleModeImp == ScaleModeNative.Manual) && ((int)actualMeasureUnit > (int)gridAlignment)) {
					gridAlignment = (DateTimeGridAlignmentNative)actualMeasureUnit;
					spacing = 1;
				}
				if (autoGridAlignment != gridAlignment || autoGridSpacing != spacing) {
					gridAlignmentChange = CreateInfo((DateTimeGridAlignment)autoGridAlignment, (DateTimeGridAlignment)gridAlignment);
					autoGridAlignment = gridAlignment;
					autoGridSpacing = spacing;
				}
			}
			if ((measureUnitChange != null && measureUnitChange.IsChanged) || (gridAlignmentChange != null && gridAlignmentChange.IsChanged))
				OnScaleChanged(measureUnitChange, gridAlignmentChange, null, null, null, null);
			return measureUnitChange != null && measureUnitChange.IsChanged;
		}
		void IDateTimeScaleOptions.UpdateAutomaticGrid(DateTimeGridAlignmentNative gridAlignment, double spacing) {
			ValueChangeInfo<DateTimeGridAlignment> gridAlignmentChange = null;
			if (NeedAutoGridAlignment) {
				DateTimeMeasureUnit actualMeasureUnit = GetActualMeasureUnit();
				if ((ScaleModeImp == ScaleModeNative.Manual) && ((int)actualMeasureUnit > (int)gridAlignment)) {
					gridAlignment = (DateTimeGridAlignmentNative)actualMeasureUnit;
					spacing = 1;
				}
				if (autoGridAlignment != gridAlignment || autoGridSpacing != spacing) {
					gridAlignmentChange = CreateInfo((DateTimeGridAlignment)autoGridAlignment, (DateTimeGridAlignment)gridAlignment);
					autoGridAlignment = gridAlignment;
					autoGridSpacing = spacing;
				}
			}
			if ((gridAlignmentChange != null && gridAlignmentChange.IsChanged))
				OnScaleChanged(null, gridAlignmentChange, null, null, null, null);
		}
		DateTimeMeasureUnitNative IScaleOptionsBase<DateTimeMeasureUnitNative>.CalculateCustomMeasureUnit(IEnumerable<ISeries> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax) {
			return CalculateCustomMeasureUnit(series, axisLength, pixelsPerUnit, visualMin, visualMax, wholeMin, wholeMax);
		}
		#endregion
		protected void OnScaleChanged(ValueChangeInfo<DateTimeMeasureUnit> measureUnitChange,
								 ValueChangeInfo<DateTimeGridAlignment> gridAlignmentChange,
								 ValueChangeInfo<AggregateFunction> aggregateFunctionChange,
								 ValueChangeInfo<double> gridSpacingChange,
								 ValueChangeInfo<double> gridOffsetChange,
								 ValueChangeInfo<bool> autoGridChange) {
			if (Axis == null || ChartControl == null)
				return;
			DateTimeScaleChangedEventArgs args = new DateTimeScaleChangedEventArgs(Axis,
																						CreateInfo((ScaleMode)ScaleModeImp),
																						GetInfo(measureUnitChange, (DateTimeMeasureUnit)autoMeasureUnit),
																						GetInfo(gridAlignmentChange, (DateTimeGridAlignment)autoGridAlignment),
																						GetInfo(gridSpacingChange, autoGridSpacing),
																						GetInfo(aggregateFunctionChange, AggregateFunctionImp),
																						GetInfo(gridOffsetChange, GridOffsetImp),
																						GetInfo(autoGridChange, AutoGridImp));
			ChartControl.RaiseEvent(args);
		}
		DateTimeMeasureUnit GetActualMeasureUnit() {
			return (ScaleModeImp == ScaleModeNative.Automatic) ? (DateTimeMeasureUnit)autoMeasureUnit : MeasureUnitImp;
		}
		DateTimeGridAlignment GetActualGridAlignment() {
			return NeedAutoGridAlignment ? (DateTimeGridAlignment)autoGridAlignment : GridAlignmentImp;
		}
		protected virtual DateTimeMeasureUnitNative CalculateCustomMeasureUnit(IEnumerable<ISeries> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax) {
			throw new NotImplementedException();
		}
		protected internal virtual void ResetAutoGrid(double customGridSpacing) {
		}
		protected internal virtual void SetGridAlignment(DateTimeGridAlignment gridAlignment) {
		}
	}
	public interface IDateTimeMeasureUnitsCalculator {
		DateTimeMeasureUnit CalculateMeasureUnit(IEnumerable<Series> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax);
	}
	public class AutomaticDateTimeScaleOptions : DateTimeScaleOptionsBase {
		public static readonly DependencyProperty AggregateFunctionProperty = DependencyPropertyManager.Register("AggregateFunction",
			typeof(AggregateFunction), typeof(AutomaticDateTimeScaleOptions), new PropertyMetadata(AggregateFunction.Average, AggregateFunctionPropertyChanged));
		public static readonly DependencyProperty AutomaticMeasureUnitsCalculatorProperty = DependencyPropertyManager.Register("AutomaticMeasureUnitsCalculator",
			typeof(IDateTimeMeasureUnitsCalculator), typeof(AutomaticDateTimeScaleOptions), new PropertyMetadata(null, MeasureUnitsCalculatorPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AutomaticDateTimeScaleOptionsAggregateFunction"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public AggregateFunction AggregateFunction {
			get { return (AggregateFunction)GetValue(AggregateFunctionProperty); }
			set { SetValue(AggregateFunctionProperty, value); }
		}
		[
		Browsable(false),
#if !SL
	DevExpressXpfChartsLocalizedDescription("AutomaticDateTimeScaleOptionsAutomaticMeasureUnitsCalculator"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public IDateTimeMeasureUnitsCalculator AutomaticMeasureUnitsCalculator {
			get { return (IDateTimeMeasureUnitsCalculator)GetValue(AutomaticMeasureUnitsCalculatorProperty); }
			set { SetValue(AutomaticMeasureUnitsCalculatorProperty, value); }
		}
		static void AggregateFunctionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AutomaticDateTimeScaleOptions obj = d as AutomaticDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualAggregateFunction = (AggregateFunction)e.NewValue;
				ChartElementHelper.Update(d, new DataAggregationUpdate(obj.AxisData));
			}
		}
		AggregateFunction aggregateFunction = AggregateFunction.Average;
		DateTimeMeasureUnit measureUnit = DateTimeMeasureUnit.Day;
		DateTimeGridAlignment gridAlignment = DateTimeGridAlignment.Day;
		AggregateFunction ActualAggregateFunction {
			set {
				if (aggregateFunction != value)
					OnScaleChanged(null, null, CreateInfo(aggregateFunction, value), null, null, null);
				aggregateFunction = value;
			}
		}
		protected override ScaleModeNative ScaleModeImp { get { return ScaleModeNative.Automatic; } }
		protected override AggregateFunction AggregateFunctionImp { get { return aggregateFunction; } }
		protected override double GridOffsetImp { get { return 0; } }
		protected override double GridSpacingImp { get { return AutoGridSpacing; } set { } }
		protected override DateTimeMeasureUnit MeasureUnitImp { get { return measureUnit; } }
		protected override DateTimeGridAlignment GridAlignmentImp { get { return gridAlignment; } }
		protected override bool UseCustomMeasureUnit { get { return AutomaticMeasureUnitsCalculator != null; } }
		protected internal override bool AutoGridImp { get { return false; } }
		protected override ChartDependencyObject CreateObject() {
			return new AutomaticDateTimeScaleOptions();
		}
		protected override DateTimeMeasureUnitNative CalculateCustomMeasureUnit(IEnumerable<ISeries> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax) {
			return (DateTimeMeasureUnitNative)AutomaticMeasureUnitsCalculator.CalculateMeasureUnit(EnumerateSeries(series), axisLength, pixelsPerUnit, visualMin, visualMax, wholeMin, wholeMax);
		}
	}
	public class ManualDateTimeScaleOptions : DateTimeScaleOptionsBase {
		public static readonly DependencyProperty AggregateFunctionProperty = DependencyPropertyManager.Register("AggregateFunction",
			typeof(AggregateFunction), typeof(ManualDateTimeScaleOptions), new PropertyMetadata(AggregateFunction.Average, AggregateFunctionPropertyChanged));
		public static readonly DependencyProperty MeasureUnitProperty = DependencyPropertyManager.Register("MeasureUnit",
			typeof(DateTimeMeasureUnit), typeof(ManualDateTimeScaleOptions), new PropertyMetadata(DateTimeMeasureUnit.Day, MeasureUnitPropertyChanged));
		public static readonly DependencyProperty AutoGridProperty = DependencyPropertyManager.Register("AutoGrid",
			typeof(bool), typeof(ManualDateTimeScaleOptions), new PropertyMetadata(true, AutoGridPropertyChanged));
		public static readonly DependencyProperty GridSpacingProperty = DependencyPropertyManager.Register("GridSpacing",
			typeof(double), typeof(ManualDateTimeScaleOptions), new PropertyMetadata(1.0, GridSpacingPropertyChanged));
		public static readonly DependencyProperty GridOffsetProperty = DependencyPropertyManager.Register("GridOffset",
			typeof(double), typeof(ManualDateTimeScaleOptions), new PropertyMetadata(0.0, GridOffsetPropertyChanged));
		public static readonly DependencyProperty GridAlignmentProperty = DependencyPropertyManager.Register("GridAlignment",
			typeof(DateTimeGridAlignment), typeof(ManualDateTimeScaleOptions), new PropertyMetadata(DateTimeGridAlignment.Day, GridAlignmentPropertyChanged));
		static void AggregateFunctionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualDateTimeScaleOptions obj = d as ManualDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualAggregateFunction = (AggregateFunction)e.NewValue;
				ChartElementHelper.Update(d, new DataAggregationUpdate(obj.AxisData));
			}
		}
		static void AutoGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualDateTimeScaleOptions obj = d as ManualDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualAutoGrid = (bool)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "AutoGrid"));
			}
		}
		static void GridSpacingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualDateTimeScaleOptions obj = d as ManualDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualGridSpacing = (double)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "GridSpacing"));
			}
		}
		static void GridOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualDateTimeScaleOptions obj = d as ManualDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualGridOffset = (double)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "GridOffset"));
			}
		}
		static void MeasureUnitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualDateTimeScaleOptions obj = d as ManualDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualMeasureUnit = (DateTimeMeasureUnit)e.NewValue;
				ChartElementHelper.Update(d, new DataAggregationUpdate(obj.AxisData));
			}
		}
		static void GridAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualDateTimeScaleOptions obj = d as ManualDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualGridAlignment = (DateTimeGridAlignment)e.NewValue;
				ChartElementHelper.Update(d, new ChartUpdate(new DataAggregationUpdate(obj.AxisData)));
			}
		}
		bool autoGrid = true;
		AggregateFunction aggregateFunction = AggregateFunction.Average;
		DateTimeMeasureUnit measureUnit = DateTimeMeasureUnit.Day;
		DateTimeGridAlignment gridAlignment = DateTimeGridAlignment.Day;
		double gridOffset = 0;
		double gridSpacingImp = 1.0;
		DateTimeMeasureUnit ActualMeasureUnit {
			set {
				if (measureUnit != value)
					OnScaleChanged(CreateInfo(measureUnit, value), null, null, null, null, null);
				measureUnit = value;
			}
		}
		double ActualGridOffset {
			set {
				if (gridOffset != value)
					OnScaleChanged(null, null, null, null, CreateInfo(gridOffset, value), null);
				gridOffset = value;
			}
		}
		double ActualGridSpacing {
			set {
				if (gridSpacingImp != value)
					OnScaleChanged(null, null, null, CreateInfo(gridSpacingImp, value), null, null);
				gridSpacingImp = value;
			}
		}
		bool ActualAutoGrid {
			set {
				if (autoGrid != value)
					OnScaleChanged(null, null, null, null, null, CreateInfo(autoGrid, value));
				autoGrid = value;
			}
		}
		AggregateFunction ActualAggregateFunction {
			set {
				if (aggregateFunction != value)
					OnScaleChanged(null, null, CreateInfo(aggregateFunction, value), null, null, null);
				aggregateFunction = value;
			}
		}
		DateTimeGridAlignment ActualGridAlignment {
			set {
				if (gridAlignment != value)
					OnScaleChanged(null, CreateInfo(gridAlignment, value), null, null, null, null);
				gridAlignment = value;
			}
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ManualDateTimeScaleOptionsAggregateFunction"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public AggregateFunction AggregateFunction {
			get { return (AggregateFunction)GetValue(AggregateFunctionProperty); }
			set { SetValue(AggregateFunctionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ManualDateTimeScaleOptionsMeasureUnit"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public DateTimeMeasureUnit MeasureUnit {
			get { return (DateTimeMeasureUnit)GetValue(MeasureUnitProperty); }
			set { SetValue(MeasureUnitProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ManualDateTimeScaleOptionsAutoGrid"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public bool AutoGrid {
			get { return (bool)GetValue(AutoGridProperty); }
			set { SetValue(AutoGridProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ManualDateTimeScaleOptionsGridSpacing"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double GridSpacing {
			get { return (double)GetValue(GridSpacingProperty); }
			set { SetValue(GridSpacingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ManualDateTimeScaleOptionsGridOffset"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double GridOffset {
			get { return (double)GetValue(GridOffsetProperty); }
			set { SetValue(GridOffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ManualDateTimeScaleOptionsGridAlignment"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public DateTimeGridAlignment GridAlignment {
			get { return (DateTimeGridAlignment)GetValue(GridAlignmentProperty); }
			set { SetValue(GridAlignmentProperty, value); }
		}
		protected override ScaleModeNative ScaleModeImp { get { return ScaleModeNative.Manual; } }
		protected override AggregateFunction AggregateFunctionImp { get { return aggregateFunction; } }
		protected override double GridSpacingImp { get { return AutoGridImp ? AutoGridSpacing : gridSpacingImp; } set { } }
		protected override DateTimeMeasureUnit MeasureUnitImp { get { return measureUnit; } }
		protected override double GridOffsetImp { get { return gridOffset; } }
		protected override DateTimeGridAlignment GridAlignmentImp { get { return gridAlignment; } }
		protected internal override bool AutoGridImp { get { return autoGrid; } }
		protected override ChartDependencyObject CreateObject() {
			return new ManualDateTimeScaleOptions();
		}
		protected internal override void ResetAutoGrid(double customGridSpacing) {
			GridSpacing = customGridSpacing;
			AutoGrid = false;
		}
		protected internal override void SetGridAlignment(DateTimeGridAlignment gridAlignment) {
			GridAlignment = gridAlignment;
			if (AutoGrid)
				AutoGrid = false;
		}
	}
	public class ContinuousDateTimeScaleOptions : DateTimeScaleOptionsBase {
		public static readonly DependencyProperty AutoGridProperty = DependencyPropertyManager.Register("AutoGrid",
			typeof(bool), typeof(ContinuousDateTimeScaleOptions), new PropertyMetadata(true, AutoGridPropertyChanged));
		public static readonly DependencyProperty GridSpacingProperty = DependencyPropertyManager.Register("GridSpacing",
			typeof(double), typeof(ContinuousDateTimeScaleOptions), new PropertyMetadata(1.0, GridSpacingPropertyChanged));
		public static readonly DependencyProperty GridOffsetProperty = DependencyPropertyManager.Register("GridOffset",
			typeof(double), typeof(ContinuousDateTimeScaleOptions), new PropertyMetadata(0.0, GridOffsetPropertyChanged));
		public static readonly DependencyProperty GridAlignmentProperty = DependencyPropertyManager.Register("GridAlignment",
			typeof(DateTimeGridAlignment), typeof(ContinuousDateTimeScaleOptions), new PropertyMetadata(DateTimeGridAlignment.Day, GridAlignmentPropertyChanged));
		static void AutoGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ContinuousDateTimeScaleOptions obj = d as ContinuousDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualAutoGrid = (bool)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "AutoGrid"));
			}
		}
		static void GridSpacingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ContinuousDateTimeScaleOptions obj = d as ContinuousDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualGridSpacing = (double)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "GridSpacing"));
			}
		}
		static void GridOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ContinuousDateTimeScaleOptions obj = d as ContinuousDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualGridOffset = (double)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "GridOffset"));
			}
		}
		static void GridAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ContinuousDateTimeScaleOptions obj = d as ContinuousDateTimeScaleOptions;
			if (obj != null) {
				obj.ActualGridAlignment = (DateTimeGridAlignment)e.NewValue;
				ChartElementHelper.Update(d, new ChartUpdate(new DataAggregationUpdate(obj.AxisData)));
			}
		}
		bool autoGrid = true;
		double gridOffset = 0;
		double gridSpacingImp = 1.0;
		DateTimeGridAlignment gridAlignment = DateTimeGridAlignment.Day;
		bool ActualAutoGrid {
			set {
				if (autoGrid != value)
					OnScaleChanged(null, null, null, null, null, CreateInfo(autoGrid, value));
				autoGrid = value;
			}
		}
		double ActualGridOffset {
			set {
				if (gridOffset != value)
					OnScaleChanged(null, null, null, null, CreateInfo(gridOffset, value), null);
				gridOffset = value;
			}
		}
		double ActualGridSpacing {
			set {
				if (gridSpacingImp != value)
					OnScaleChanged(null, null, null, CreateInfo(gridSpacingImp, value), null, null);
				gridSpacingImp = value;
			}
		}
		DateTimeGridAlignment ActualGridAlignment {
			set {
				if (gridAlignment != value)
					OnScaleChanged(null, CreateInfo(gridAlignment, value), null, null, null, null);
				gridAlignment = value;
			}
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ContinuousDateTimeScaleOptionsAutoGrid"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public bool AutoGrid {
			get { return (bool)GetValue(AutoGridProperty); }
			set { SetValue(AutoGridProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ContinuousDateTimeScaleOptionsGridSpacing"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double GridSpacing {
			get { return (double)GetValue(GridSpacingProperty); }
			set { SetValue(GridSpacingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ContinuousDateTimeScaleOptionsGridOffset"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double GridOffset {
			get { return (double)GetValue(GridOffsetProperty); }
			set { SetValue(GridOffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ContinuousDateTimeScaleOptionsGridAlignment"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public DateTimeGridAlignment GridAlignment {
			get { return (DateTimeGridAlignment)GetValue(GridAlignmentProperty); }
			set { SetValue(GridAlignmentProperty, value); }
		}
		protected override ScaleModeNative ScaleModeImp { get { return ScaleModeNative.Continuous; } }
		protected override AggregateFunction AggregateFunctionImp { get { return AggregateFunction.None; } }
		protected override double GridSpacingImp { get { return AutoGridImp ? AutoGridSpacing : gridSpacingImp; } set { } }
		protected override DateTimeMeasureUnit MeasureUnitImp { get { return DateTimeMeasureUnit.Millisecond; } }
		protected override double GridOffsetImp { get { return gridOffset; } }
		protected override DateTimeGridAlignment GridAlignmentImp { get { return gridAlignment; } }
		protected internal override bool AutoGridImp { get { return autoGrid; } }
		protected override ChartDependencyObject CreateObject() {
			return new ContinuousDateTimeScaleOptions();
		}
		protected internal override void SetGridAlignment(DateTimeGridAlignment gridAlignment) {
			GridAlignment = gridAlignment;
		}
	}
}
