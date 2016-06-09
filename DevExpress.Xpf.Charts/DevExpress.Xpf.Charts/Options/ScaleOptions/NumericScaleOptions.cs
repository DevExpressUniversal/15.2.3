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
	public abstract class NumericScaleOptionsBase : ScaleOptionsBase, INumericScaleOptions {
		double autoMeasureUnit = double.NaN;
		double autoGridAlignment = 1;
		NumericMeasureUnitsCalculatorCore numericMeasureUnitsCalculatorCore;
		protected override bool GridSpasingAutoImp { get { return (ScaleModeImp == ScaleModeNative.Automatic) || AutoGridImp; } }
		protected abstract double GridAlignmentImp { get; }
		protected abstract double MeasureUnitImp { get; }
		protected virtual bool UseCustomMeasureUnit { get { return false; } }
		internal NumericMeasureUnitsCalculatorCore NumericMeasureUnitsCalculatorCore {
			get {
				if (numericMeasureUnitsCalculatorCore == null)
					numericMeasureUnitsCalculatorCore = new NumericMeasureUnitsCalculatorCore(Axis);
				return numericMeasureUnitsCalculatorCore;
			}
		}
		#region INumericScaleOptions
		double INumericScaleOptions.GridAlignment { get { return GetActualGridAlignment(); } }
		double IScaleOptionsBase<double>.MeasureUnit { get { return GetActualMeasureUnit(); } }
		bool IScaleOptionsBase<double>.UseCustomMeasureUnit { get { return UseCustomMeasureUnit; } }
		bool INumericScaleOptions.UpdateAutomaticUnits(double measureUnit, double gridAlignment) {
			ValueChangeInfo<double> measureUnitChange = null;
			ValueChangeInfo<double> gridAlignmentChange = null;
			bool unitWasUpdated = false;
			double oldAutoMeasureUnit = autoMeasureUnit;
			autoMeasureUnit = measureUnit;
			if (ScaleModeImp == ScaleModeNative.Automatic && oldAutoMeasureUnit != autoMeasureUnit) {
				measureUnitChange = CreateInfo(oldAutoMeasureUnit, autoMeasureUnit);
				unitWasUpdated = true;
			}
			if (GridSpasingAutoImp) {
				gridAlignmentChange = CreateInfo(autoGridAlignment, measureUnit);
				autoGridAlignment = measureUnit;
			}
			if ((measureUnitChange != null && measureUnitChange.IsChanged) || (gridAlignmentChange != null && gridAlignmentChange.IsChanged))
				OnScaleChanged(measureUnitChange, gridAlignmentChange, null, null, null, null);
			return unitWasUpdated;
		}
		double IScaleOptionsBase<double>.CalculateCustomMeasureUnit(IEnumerable<ISeries> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax) {
			return CalculateCustomMeasureUnit(series, axisLength, pixelsPerUnit, visualMin, visualMax, wholeMin, wholeMax);
		}
		#endregion
		protected void OnScaleChanged(ValueChangeInfo<double> measureUnitChange,
								ValueChangeInfo<double> gridAlignmentChange,
								ValueChangeInfo<AggregateFunction> aggregateFunctionChange,
								ValueChangeInfo<double> gridSpacingChange,
								ValueChangeInfo<double> gridOffsetChange,
								ValueChangeInfo<bool> autoGridChange) {
			if (Axis == null || ChartControl == null)
				return;
			NumericScaleChangedEventArgs args = new NumericScaleChangedEventArgs(Axis,
																						CreateInfo((ScaleMode)ScaleModeImp),
																						GetInfo(measureUnitChange, GetActualMeasureUnit()),
																						GetInfo(gridAlignmentChange, GetActualGridAlignment()),
																						GetInfo(gridSpacingChange, GridSpacingImp),
																						GetInfo(aggregateFunctionChange, AggregateFunctionImp),
																						GetInfo(gridOffsetChange, GridOffsetImp),
																						GetInfo(autoGridChange, AutoGridImp));
			ChartControl.RaiseEvent(args);
		}
		internal static bool ValidateDoubleIsGreaterThanZero(object value) {
			return (double)value > 0.0;
		}
		protected virtual double CalculateCustomMeasureUnit(IEnumerable<ISeries> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax) {
			throw new NotImplementedException();
		}
		protected internal virtual void ResetAutoGrid(double customGridSpacing) {
		}
		double GetActualMeasureUnit() {
			return ScaleModeImp != ScaleModeNative.Manual ? autoMeasureUnit : MeasureUnitImp;
		}
		double GetActualGridAlignment() {
			if (AxisData is ILogarithmic && ((ILogarithmic)AxisData).Enabled)
				return 1;
			if (GridSpasingAutoImp)
				return autoGridAlignment;
			return (GridAlignmentImp < MeasureUnitImp) ? MeasureUnitImp : GridAlignmentImp;
		}
	}
	public interface INumericMeasureUnitsCalculator {
		double CalculateMeasureUnit(IEnumerable<Series> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax);
	}
	public class AutomaticNumericScaleOptions : NumericScaleOptionsBase {
		public static readonly DependencyProperty AggregateFunctionProperty = DependencyPropertyManager.Register("AggregateFunction",
			typeof(AggregateFunction), typeof(AutomaticNumericScaleOptions), new PropertyMetadata(AggregateFunction.Average, AggregateFunctionPropertyChanged));
		public static readonly DependencyProperty AutomaticMeasureUnitsCalculatorProperty = DependencyPropertyManager.Register("AutomaticMeasureUnitsCalculator",
			typeof(INumericMeasureUnitsCalculator), typeof(AutomaticNumericScaleOptions), new PropertyMetadata(null, MeasureUnitsCalculatorPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AutomaticNumericScaleOptionsAggregateFunction"),
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
	DevExpressXpfChartsLocalizedDescription("AutomaticNumericScaleOptionsAutomaticMeasureUnitsCalculator"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public INumericMeasureUnitsCalculator AutomaticMeasureUnitsCalculator {
			get { return (INumericMeasureUnitsCalculator)GetValue(AutomaticMeasureUnitsCalculatorProperty); }
			set { SetValue(AutomaticMeasureUnitsCalculatorProperty, value); }
		}
		static void AggregateFunctionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AutomaticNumericScaleOptions obj = d as AutomaticNumericScaleOptions;
			if (obj != null) {
				obj.ActualAggregateFunction = (AggregateFunction)e.NewValue;
				ChartElementHelper.Update(d, new ChartUpdate(new DataAggregationUpdate(obj.AxisData)));
			}
		}
		AggregateFunction aggregateFunction = AggregateFunction.Average;
		double gridAlignment = 1.0;
		double measureUnit = 1.0;
		AggregateFunction ActualAggregateFunction {
			set {
				if (aggregateFunction != value)
					OnScaleChanged(null, null, CreateInfo(aggregateFunction, value), null, null, null);
				aggregateFunction = value;
			}
		}
		protected override AggregateFunction AggregateFunctionImp { get { return aggregateFunction; } }
		protected override ScaleModeNative ScaleModeImp { get { return ScaleModeNative.Automatic; } }
		protected internal override bool AutoGridImp { get { return true; } }
		protected override double GridSpacingImp { get { return Double.NaN; } set { } }
		protected override double GridOffsetImp { get { return 0; } }
		protected override double GridAlignmentImp { get { return gridAlignment; } }
		protected override double MeasureUnitImp { get { return measureUnit; } }
		protected override bool UseCustomMeasureUnit { get { return AutomaticMeasureUnitsCalculator != null; } }
		protected override ChartDependencyObject CreateObject() {
			return new AutomaticNumericScaleOptions();
		}
		protected override double CalculateCustomMeasureUnit(IEnumerable<ISeries> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax) {
			return AutomaticMeasureUnitsCalculator.CalculateMeasureUnit(EnumerateSeries(series), axisLength, pixelsPerUnit, visualMin, visualMax, wholeMin, wholeMax);
		}
	}
	public class ManualNumericScaleOptions : NumericScaleOptionsBase {
		public static readonly DependencyProperty AggregateFunctionProperty = DependencyPropertyManager.Register("AggregateFunction",
			typeof(AggregateFunction), typeof(ManualNumericScaleOptions), new PropertyMetadata(AggregateFunction.Average, AggregateFunctionPropertyChanged));
		public static readonly DependencyProperty MeasureUnitProperty = DependencyPropertyManager.Register("MeasureUnit",
			typeof(double), typeof(ManualNumericScaleOptions), new PropertyMetadata(1.0, MeasureUnitPropertyChanged), NumericScaleOptionsBase.ValidateDoubleIsGreaterThanZero);
		public static readonly DependencyProperty AutoGridProperty = DependencyPropertyManager.Register("AutoGrid",
			typeof(bool), typeof(ManualNumericScaleOptions), new PropertyMetadata(false, AutoGridPropertyChanged));
		public static readonly DependencyProperty GridSpacingProperty = DependencyPropertyManager.Register("GridSpacing",
			typeof(double), typeof(ManualNumericScaleOptions), new PropertyMetadata(1.0, GridSpacingPropertyChanged), ValidateGridSpacing);
		public static readonly DependencyProperty GridOffsetProperty = DependencyPropertyManager.Register("GridOffset",
			typeof(double), typeof(ManualNumericScaleOptions), new PropertyMetadata(0.0, GridOffsetPropertyChanged));
		public static readonly DependencyProperty GridAlignmentProperty = DependencyPropertyManager.Register("GridAlignment",
			typeof(double), typeof(ManualNumericScaleOptions), new PropertyMetadata(1.0, GridAlignmentPropertyChanged), NumericScaleOptionsBase.ValidateDoubleIsGreaterThanZero);
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ManualNumericScaleOptionsAggregateFunction"),
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
	DevExpressXpfChartsLocalizedDescription("ManualNumericScaleOptionsMeasureUnit"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double MeasureUnit {
			get { return (double)GetValue(MeasureUnitProperty); }
			set { SetValue(MeasureUnitProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ManualNumericScaleOptionsAutoGrid"),
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
	DevExpressXpfChartsLocalizedDescription("ManualNumericScaleOptionsGridSpacing"),
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
	DevExpressXpfChartsLocalizedDescription("ManualNumericScaleOptionsGridOffset"),
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
	DevExpressXpfChartsLocalizedDescription("ManualNumericScaleOptionsGridAlignment"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double GridAlignment {
			get { return (double)GetValue(GridAlignmentProperty); }
			set { SetValue(GridAlignmentProperty, value); }
		}
		static bool ValidateGridSpacing(object value) {
			return (double)value > 0;
		}
		static void AggregateFunctionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualNumericScaleOptions obj = d as ManualNumericScaleOptions;
			if (obj != null) {
				obj.ActualAggregateFunction = (AggregateFunction)e.NewValue;
				ChartElementHelper.Update(d, new DataAggregationUpdate(obj.AxisData));
			}
		}
		static void AutoGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualNumericScaleOptions obj = d as ManualNumericScaleOptions;
			if (obj != null) {
				obj.ActualAutoGrid = (bool)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "AutoGrid"));
			}
		}
		static void GridSpacingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualNumericScaleOptions obj = d as ManualNumericScaleOptions;
			if (obj != null) {
				obj.ActualGridSpacing = (double)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "GridSpacing"));
			}
		}
		static void GridOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualNumericScaleOptions obj = d as ManualNumericScaleOptions;
			if (obj != null) {
				obj.ActualGridOffset = (double)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "GridOffset"));
			}
		}
		static void MeasureUnitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualNumericScaleOptions obj = d as ManualNumericScaleOptions;
			if (obj != null) {
				obj.ActualMeasureUnit = (double)e.NewValue;
				ChartElementHelper.Update(d, new DataAggregationUpdate(obj.AxisData));
			}
		}
		static void GridAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManualNumericScaleOptions obj = d as ManualNumericScaleOptions;
			if (obj != null) {
				obj.ActualGridAlignment = (double)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "GridAlignment"));
			}
		}
		bool autoGrid = false;
		double gridSpacing = 1.0;
		double gridOffset = 0;
		double gridAlignment = 1.0;
		double actualMeasureUnit = 1.0;
		double autoGridSpacing = 1.0;
		AggregateFunction aggregateFunction = AggregateFunction.Average;
		AggregateFunction ActualAggregateFunction {
			set {
				if (aggregateFunction != value)
					OnScaleChanged(null, null, CreateInfo(aggregateFunction, value), null, null, null);
				aggregateFunction = value;
			}
		}
		bool ActualAutoGrid {
			set {
				if (autoGrid != value)
					OnScaleChanged(null, null, null, null, null, CreateInfo(autoGrid, value));
				autoGrid = value;
			}
		}
		double ActualGridSpacing {
			set {
				if (gridSpacing != value)
					OnScaleChanged(null, null, null, CreateInfo(gridSpacing, value), null, null);
				gridSpacing = value;
			}
		}
		double ActualGridOffset {
			set {
				if (gridOffset != value)
					OnScaleChanged(null, null, null, null, CreateInfo(gridOffset, value), null);
				gridOffset = value;
			}
		}
		double ActualMeasureUnit {
			set {
				if (actualMeasureUnit != value)
					OnScaleChanged(CreateInfo(actualMeasureUnit, value), null, null, null, null, null);
				actualMeasureUnit = value;
			}
		}
		double ActualGridAlignment {
			set {
				if (gridAlignment != value)
					OnScaleChanged(null, CreateInfo(gridAlignment, value), null, null, null, null);
				gridAlignment = value;
			}
		}
		protected override AggregateFunction AggregateFunctionImp { get { return aggregateFunction; } }
		protected override ScaleModeNative ScaleModeImp { get { return ScaleModeNative.Manual; } }
		protected override double GridSpacingImp {
			get { return AutoGridImp ? autoGridSpacing : gridSpacing; }
			set { autoGridSpacing = value; }
		}
		protected override double GridOffsetImp { get { return gridOffset; } }
		protected override double GridAlignmentImp { get { return gridAlignment; } }
		protected override double MeasureUnitImp { get { return actualMeasureUnit; } }
		protected internal override bool AutoGridImp { get { return autoGrid; } }
		protected override ChartDependencyObject CreateObject() {
			return new ManualNumericScaleOptions();
		}
	}
	public class ContinuousNumericScaleOptions : NumericScaleOptionsBase {
		public static readonly DependencyProperty AutoGridProperty = DependencyPropertyManager.Register("AutoGrid",
			typeof(bool), typeof(ContinuousNumericScaleOptions), new PropertyMetadata(true, AutoGridPropertyChanged));
		public static readonly DependencyProperty GridSpacingProperty = DependencyPropertyManager.Register("GridSpacing",
			typeof(double), typeof(ContinuousNumericScaleOptions), new PropertyMetadata(Double.NaN, GridSpacingPropertyChanged));
		public static readonly DependencyProperty GridOffsetProperty = DependencyPropertyManager.Register("GridOffset",
			typeof(double), typeof(ContinuousNumericScaleOptions), new PropertyMetadata(0.0, GridOffsetPropertyChanged));
		public static readonly DependencyProperty GridAlignmentProperty = DependencyPropertyManager.Register("GridAlignment",
			typeof(double), typeof(ContinuousNumericScaleOptions), new PropertyMetadata(1.0, GridAlignmentPropertyChanged), NumericScaleOptionsBase.ValidateDoubleIsGreaterThanZero);
		static void AutoGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ContinuousNumericScaleOptions obj = d as ContinuousNumericScaleOptions;
			if (obj != null) {
				obj.ActualAutoGrid = (bool)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "AutoGrid"));
			}
		}
		static void GridSpacingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ContinuousNumericScaleOptions obj = d as ContinuousNumericScaleOptions;
			if (obj != null) {
				obj.ActualGridSpacing = (double)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "GridSpacing"));
			}
		}
		static void GridOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ContinuousNumericScaleOptions obj = d as ContinuousNumericScaleOptions;
			if (obj != null) {
				obj.ActualGridOffset = (double)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "GridOffset"));
			}
		}
		static void GridAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ContinuousNumericScaleOptions obj = d as ContinuousNumericScaleOptions;
			if (obj != null) {
				obj.ActualGridAlignment = (double)e.NewValue;
				ChartElementHelper.Update(d, new PropertyUpdateInfo(obj.AxisData, "GridAlignment"));
			}
		}
		bool autoGrid = true;
		double gridSpacing = double.NaN;
		double gridOffset = 0;
		double gridAlignment = 1.0;
		double measureUnit = 1.0;
		double autoGridSpacing = double.NaN;
		AggregateFunction aggregateFunction = AggregateFunction.None;
		bool ActualAutoGrid {
			set {
				if (autoGrid != value)
					OnScaleChanged(null, null, null, null, null, CreateInfo(autoGrid, value));
				autoGrid = value;
			}
		}
		double ActualGridSpacing {
			set {
				if (gridSpacing != value)
					OnScaleChanged(null, null, null, CreateInfo(gridSpacing, value), null, null);
				gridSpacing = value;
			}
		}
		double ActualGridOffset {
			set {
				if (gridOffset != value)
					OnScaleChanged(null, null, null, null, CreateInfo(gridOffset, value), null);
				gridOffset = value;
			}
		}
		double ActualGridAlignment {
			set {
				if (gridAlignment != value)
					OnScaleChanged(null, CreateInfo(gridAlignment, value), null, null, null, null);
				gridAlignment = value;
			}
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ContinuousNumericScaleOptionsAutoGrid"),
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
	DevExpressXpfChartsLocalizedDescription("ContinuousNumericScaleOptionsGridSpacing"),
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
	DevExpressXpfChartsLocalizedDescription("ContinuousNumericScaleOptionsGridOffset"),
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
	DevExpressXpfChartsLocalizedDescription("ContinuousNumericScaleOptionsGridAlignment"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double GridAlignment {
			get { return (double)GetValue(GridAlignmentProperty); }
			set { SetValue(GridAlignmentProperty, value); }
		}
		protected override AggregateFunction AggregateFunctionImp { get { return aggregateFunction; } }
		protected override ScaleModeNative ScaleModeImp { get { return ScaleModeNative.Continuous; } }
		protected override double GridSpacingImp {
			get { return AutoGridImp ? autoGridSpacing : gridSpacing; }
			set { autoGridSpacing = value; }
		}
		protected override double GridOffsetImp { get { return gridOffset; } }
		protected override double GridAlignmentImp { get { return gridAlignment; } }
		protected override double MeasureUnitImp { get { return measureUnit; } }
		protected internal override bool AutoGridImp { get { return autoGrid || double.IsNaN(gridSpacing); } }
		protected internal override void ResetAutoGrid(double customGridSpacing) {
			GridSpacing = customGridSpacing;
			AutoGrid = false;
		}
		protected override ChartDependencyObject CreateObject() {
			return new ContinuousNumericScaleOptions();
		}
	}
}
