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
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.Utils.Design;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public class NumericScaleOptionsTypeConverter : ScaleOptionBaseConverter {
		protected override string[] GetAutomaticModeFilter(ScaleOptionsBase options) {
			return new string[] { "GridAlignment", "MeasureUnit", "CustomGridAlignment", "CustomMeasureUnit" };
		}
		protected override string[] GetManualModeFilter(ScaleOptionsBase options) {
			return CheckIsAxisX(options.Axis) ? new string[] { } : new string[] { "MeasureUnit", "CustomMeasureUnit" };
		}
		protected override string[] GetContinuousModeFilter(ScaleOptionsBase options) {
			return new string[] { "MeasureUnit", "CustomMeasureUnit" };
		}
		protected override string[] GetPolarFilter(ScaleOptionsBase options) {
			return new string[] { "AutoGrid", "GridSpacing", "GridOffset", "GridAlignment", "CustomGridAlignment" };
		}
		protected override string[] GetAdditionalFilter(ScaleOptionsBase options) {
			NumericScaleOptions numericOptions = (NumericScaleOptions)options;
			List<string> filter = new List<string>();
			if (numericOptions.GridAlignment != NumericGridAlignment.Custom)
				filter.Add("CustomGridAlignment");
			if (numericOptions.MeasureUnit != NumericMeasureUnit.Custom)
				filter.Add("CustomMeasureUnit");
			return filter.ToArray();
		}
	}
}
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum NumericMeasureUnit {
		Ones,
		Tens,
		Hundreds,
		Thousands,
		Millions,
		Billions,
		Custom
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum NumericGridAlignment {
		Ones,
		Tens,
		Hundreds,
		Thousands,
		Millions,
		Billions,
		Custom
	}
	public interface INumericMeasureUnitsCalculator {
		double CalculateMeasureUnit(IEnumerable<Series> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax);
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions, "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(NumericScaleOptionsTypeConverter))
	]
	public sealed class NumericScaleOptions : ScaleOptionsBase, INumericScaleOptions {
		static double RoundMeasureUnit(double measureUnit) {
			if (measureUnit == 1)
				return MathUtils.StrongRound(measureUnit);
			else if (measureUnit > 1) {
				int tens = 0;
				while (measureUnit > 10) {
					measureUnit /= 10;
					tens++;
				}
				return MathUtils.StrongRound(measureUnit) * Math.Pow(10, tens);
			} else if (measureUnit > 0) {
				int tens = 0;
				while (measureUnit < 1) {
					measureUnit *= 10;
					tens++;
				}
				return (1.0 / Math.Pow(10, tens)) * MathUtils.StrongRound(measureUnit);
			}
			throw new ArgumentException("Measure unit must be greater than zero.");
		}
		const NumericMeasureUnit DefaultMeasureUnit = NumericMeasureUnit.Ones;
		const NumericGridAlignment DefaultGridAlignment = NumericGridAlignment.Ones;
		const double DefaultCustomGridAlignment = 1;
		const double DefaultCustomMeasureUnit = 100;
		NumericMeasureUnit measureUnit;
		NumericGridAlignment gridAlignment;
		NumericMeasureUnitsCalculatorCore numericMeasureUnitsCalculatorCore;
		INumericMeasureUnitsCalculator automaticMeasureUnitsCalculator;
		bool customGridAlignmentChanged = false;
		double customGridAlignment;
		double customMeasureUnit;
		double autoMeasureUnit = double.NaN;
		double autoGridAlignment = 1;
		double oldGridSpacing = double.NaN;
		internal NumericMeasureUnitsCalculatorCore NumericMeasureUnitsCalculatorCore {
			get {
				if (numericMeasureUnitsCalculatorCore == null && Axis != null)
					numericMeasureUnitsCalculatorCore = new NumericMeasureUnitsCalculatorCore(Axis);
				return numericMeasureUnitsCalculatorCore;
			}
		}
		protected override ScaleMode DefaultScaleMode { get { return ScaleMode.Continuous; } }
		protected internal override bool IsSmartScale { get { return ScaleMode == ScaleMode.Automatic; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("NumericScaleOptionsMeasureUnit"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.NumericScaleOptions.MeasureUnit"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public NumericMeasureUnit MeasureUnit {
			get {
				return measureUnit;
			}
			set {
				if (measureUnit != value) {
					ValidateMeasureUnit(customMeasureUnit);
					SendNotification(new ElementWillChangeNotification(this));
					ValueChangeInfo<double> gridAlignmentChange = null;
					if (ShouldUpdateGridAlignment(value, customMeasureUnit)) {
						double oldGridAlignment = GetActualGridAlignment(gridAlignment, customGridAlignment);
						if (gridAlignment == NumericGridAlignment.Custom)
							customGridAlignment = GetActualMeasureUnit(value, customMeasureUnit);
						else if (value != NumericMeasureUnit.Custom)
							gridAlignment = (NumericGridAlignment)value;
						gridAlignmentChange = new ValueChangeInfo<double>(oldGridAlignment, GetActualGridAlignment(gridAlignment, customGridAlignment));
					}
					ValueChangeInfo<double> measureUnitChange = new ValueChangeInfo<double>(GetActualMeasureUnit(measureUnit, customMeasureUnit), GetActualMeasureUnit(value, customMeasureUnit));
					measureUnit = value;
					RaiseControlChanged(new DataAggregationUpdate(Axis));
					OnScaleChange(null, measureUnitChange, gridAlignmentChange, null);
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("NumericScaleOptionsGridAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.NumericScaleOptions.GridAlignment"),
		RefreshProperties(RefreshProperties.All),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public NumericGridAlignment GridAlignment {
			get {
				return gridAlignment;
			}
			set {
				if (gridAlignment != value || AutoGrid) {
					SendNotification(new ElementWillChangeNotification(this));
					double oldValue = GetActualGridAlignment(gridAlignment, customGridAlignment);
					bool isCustomGridAlignment = value == NumericGridAlignment.Custom;
					if (isCustomGridAlignment && !customGridAlignmentChanged) {
						double gridAlignmentByMeasureUnit = GetActualMeasureUnit(measureUnit, customMeasureUnit);
						if (double.IsNaN(gridAlignmentByMeasureUnit))
							gridAlignmentByMeasureUnit = 1;
						if (!double.IsNaN(oldValue))
							customGridAlignment = Math.Max(oldValue, gridAlignmentByMeasureUnit);
						else
							customGridAlignment = gridAlignmentByMeasureUnit;
					}
					if (!Loading)
						AutoGridInternal = false;
					ValidateGridAlignment(value, customGridAlignment);
					gridAlignment = value;
					customGridAlignmentChanged = isCustomGridAlignment;
					RaiseControlChanged(new PropertyUpdateInfo(Axis, "GridAlignment"));
					OnScaleChange(null, null, new ValueChangeInfo<double>(oldValue, GetActualGridAlignment(gridAlignment, customGridAlignment)), null); 
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("NumericScaleOptionsCustomGridAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.NumericScaleOptions.CustomGridAlignment"),
		RefreshProperties(RefreshProperties.All),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public double CustomGridAlignment {
			get {
				return customGridAlignment;
			}
			set {
				if (customGridAlignment != value) {
					ValidateGridAlignment(gridAlignment, value);
					SendNotification(new ElementWillChangeNotification(this));
					double oldValue = GetActualGridAlignment(gridAlignment, customGridAlignment);
					customGridAlignment = value;
					customGridAlignmentChanged = true;
					RaiseControlChanged(new PropertyUpdateInfo(Axis, "GridAlignment"));
					double newValue = GetActualGridAlignment(gridAlignment, customGridAlignment);
					if (oldValue != newValue)
						OnScaleChange(null, null, new ValueChangeInfo<double>(oldValue, newValue), null);
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("NumericScaleOptionsCustomMeasureUnit"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.NumericScaleOptions.CustomMeasureUnit"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double CustomMeasureUnit {
			get {
				return customMeasureUnit;
			}
			set {
				if (customMeasureUnit != value) {
					ValidateMeasureUnit(value);
					SendNotification(new ElementWillChangeNotification(this));
					ValueChangeInfo<double> gridAlignmentChange = null;
					if (ShouldUpdateGridAlignment(measureUnit, value)) {
						double oldGridAlignment = GetActualGridAlignment(gridAlignment, customGridAlignment);
						gridAlignment = NumericGridAlignment.Custom;
						customGridAlignmentChanged = true;
						customGridAlignment = value;
						gridAlignmentChange = new ValueChangeInfo<double>(oldGridAlignment, GetActualGridAlignment(gridAlignment, customGridAlignment));
					}
					double oldMeasureUnit = GetActualMeasureUnit(measureUnit, customMeasureUnit);
					customMeasureUnit = value;
					RaiseControlChanged(new DataAggregationUpdate(Axis));
					OnScaleChange(null, new ValueChangeInfo<double>(oldMeasureUnit, GetActualMeasureUnit(measureUnit, customMeasureUnit)), gridAlignmentChange, null);
				}
			}
		}
		[
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("DateTimeScaleOptionsAutomaticMeasureUnitsCalculator"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public INumericMeasureUnitsCalculator AutomaticMeasureUnitsCalculator {
			get { return automaticMeasureUnitsCalculator; }
			set {
				if (value != automaticMeasureUnitsCalculator) {
					SendNotification(new ElementWillChangeNotification(this));
					automaticMeasureUnitsCalculator = value;
					RaiseControlChanged(new DataAggregationUpdate(Axis));
				}
			}
		}
		internal NumericScaleOptions(ChartElement owner) : base(owner) {
			measureUnit = DefaultMeasureUnit;
			gridAlignment = DefaultGridAlignment;
			customMeasureUnit = DefaultCustomMeasureUnit;
			customGridAlignment = DefaultCustomGridAlignment;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "CustomMeasureUnit":
					return ShouldSerializeCustomMeasureUnit();
				case "CustomGridAlignment":
					return ShouldSerializeCustomGridAlignment();
				case "GridAlignment":
					return ShouldSerializeGridAlignment();
				case "MeasureUnit":
					return ShouldSerializeMeasureUnit();
				case "AutomaticMeasureUnitsCalculator":
					return false;
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeGridAlignment() {
			return ShouldSerializeAutoGrid() && (gridAlignment != DefaultGridAlignment) && (ScaleMode != ScaleMode.Automatic);
		}
		void ResetGridAlignment() {
			GridAlignment = DefaultGridAlignment;
		}
		bool ShouldSerializeMeasureUnit() {
			return (measureUnit != DefaultMeasureUnit) && (ScaleMode == ScaleMode.Manual);
		}
		void ResetMeasureUnit() {
			MeasureUnit = DefaultMeasureUnit;
		}
		bool ShouldSerializeCustomGridAlignment() {
			return ShouldSerializeGridAlignment() && (gridAlignment == NumericGridAlignment.Custom) && (customGridAlignment != DefaultCustomGridAlignment);
		}
		void ResetCustomGridAlignment() {
			CustomGridAlignment = DefaultCustomGridAlignment;
		}
		bool ShouldSerializeCustomMeasureUnit() {
			return ShouldSerializeMeasureUnit() && (measureUnit == NumericMeasureUnit.Custom) && (customMeasureUnit != DefaultCustomMeasureUnit);
		}
		void ResetCustomMeasureUnit() {
			CustomMeasureUnit = DefaultCustomMeasureUnit;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize()
				|| ShouldSerializeMeasureUnit()
				|| ShouldSerializeGridAlignment()
				|| ShouldSerializeCustomGridAlignment()
				|| ShouldSerializeCustomMeasureUnit();
		}
		#endregion
		#region INumericScaleOptions
		double INumericScaleOptions.GridAlignment { get { return GetActualGridAlignment(); } }
		double IScaleOptionsBase<double>.MeasureUnit { get { return GetActualMeasureUnit(); } }
		bool IScaleOptionsBase<double>.UseCustomMeasureUnit { get { return automaticMeasureUnitsCalculator != null; } }
		bool INumericScaleOptions.UpdateAutomaticUnits(double selectedMeasureUnit, double selectedGridAlignment) {
			ValueChangeInfo<double> measureUnitChange = null;
			ValueChangeInfo<double> gridAlignmentChange = null;
			ValueChangeInfo<double> gridSpacingChange = null;
			bool unitWasUpdated = false;
			double oldAutoMeasureUnit = autoMeasureUnit;
			autoMeasureUnit = selectedMeasureUnit;
			if (ScaleMode == ScaleMode.Automatic && oldAutoMeasureUnit != autoMeasureUnit) {
				measureUnitChange = new ValueChangeInfo<double>(oldAutoMeasureUnit, autoMeasureUnit);
				unitWasUpdated = true;
			}
			if((AutoGrid || ScaleMode == ScaleMode.Automatic) && autoGridAlignment != selectedMeasureUnit) {
				gridAlignmentChange = new ValueChangeInfo<double>(autoGridAlignment, selectedMeasureUnit);
				autoGridAlignment = selectedMeasureUnit;
			}
			if(AutoGrid && oldGridSpacing != GridSpacing) {
				gridSpacingChange = new ValueChangeInfo<double>(oldGridSpacing, GridSpacing);
				oldGridSpacing = GridSpacing;
			}
			OnScaleChange(null, measureUnitChange, gridAlignmentChange, gridSpacingChange);
			return unitWasUpdated;
		}
		double IScaleOptionsBase<double>.CalculateCustomMeasureUnit(IEnumerable<ISeries> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax) {
			return automaticMeasureUnitsCalculator.CalculateMeasureUnit(EnumerateSeries(series), axisLength, pixelsPerUnit, visualMin, visualMax, wholeMin, wholeMax);
		}
		#endregion
		void OnScaleChange(ValueChangeInfo<ScaleMode> scaleModeChange,
						   ValueChangeInfo<double> measureUnitChange,
						   ValueChangeInfo<double> gridAlignmentChange,
						   ValueChangeInfo<double> gridSpacingChange) {
			if ((measureUnitChange != null) || (gridAlignmentChange != null) || (gridSpacingChange != null) || (scaleModeChange != null)) {
				if (Axis != null) {
					IChartEventsProvider eventsProvider = Axis.ChartContainer as IChartEventsProvider;
					if(eventsProvider != null) {
						var args = new NumericScaleChangedEventArgs(Axis,
							(scaleModeChange == null) ? new ValueChangeInfo<ScaleMode>(ScaleMode) : scaleModeChange,
							(measureUnitChange == null) ? new ValueChangeInfo<double>(GetActualMeasureUnit()) : measureUnitChange,
							(gridAlignmentChange == null) ? new ValueChangeInfo<double>(GetActualGridAlignment()) : gridAlignmentChange,
							(gridSpacingChange == null) ? new ValueChangeInfo<double>(GridSpacing) : gridSpacingChange);
						eventsProvider.OnAxisScaleChanged(args);
					}
				}
			}
		}
		double GetActualMeasureUnit() {
			return GetActualMeasureUnit(MeasureUnit, CustomMeasureUnit);
		}
		double GetActualGridAlignment() {
			return GetActualGridAlignment(GridAlignment, CustomGridAlignment);
		}
		double GetActualMeasureUnit(NumericMeasureUnit measureUnit, double customMeasureUnit) {
			if (ScaleMode == XtraCharts.ScaleMode.Manual) {
				switch (measureUnit) {
					case NumericMeasureUnit.Ones:
						return 1d;
					case NumericMeasureUnit.Tens:
						return 10d;
					case NumericMeasureUnit.Hundreds:
						return 100d;
					case NumericMeasureUnit.Thousands:
						return 1000d;
					case NumericMeasureUnit.Millions:
						return 1000000d;
					case NumericMeasureUnit.Billions:
						return 1000000000d;
					case NumericMeasureUnit.Custom:
					default:
						return customMeasureUnit;
				}
			}
			return autoMeasureUnit;
		}
		double GetActualGridAlignment(NumericGridAlignment gridAlignment, double customGridAlignment) {
			if (!AutoGridInternal) {
				switch (gridAlignment) {
					case NumericGridAlignment.Ones:
						return 1d;
					case NumericGridAlignment.Tens:
						return 10d;
					case NumericGridAlignment.Hundreds:
						return 100d;
					case NumericGridAlignment.Thousands:
						return 1000d;
					case NumericGridAlignment.Millions:
						return 1000000d;
					case NumericGridAlignment.Billions:
						return 1000000000d;
					case NumericGridAlignment.Custom:
						return customGridAlignment;
				}
			}
			return 1;
		}
		bool ShouldUpdateGridAlignment(NumericMeasureUnit measureUnit, double customMeasureUnit) {
			double exactGridAlignment = GetActualGridAlignment();
			double exactMeasureUnit = GetActualMeasureUnit(measureUnit, customMeasureUnit);
			return !AutoGrid && ((exactGridAlignment < exactMeasureUnit) || (!Loading && (GetActualMeasureUnit() == exactGridAlignment)));
		}
		void ValidateGridAlignment(NumericGridAlignment gridAlignment, double customGridAlignment) {
			if (customGridAlignment <= 0)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectNumericGridAlignment));
			double exactGridAlignment = GetActualGridAlignment(gridAlignment, customGridAlignment);
			double exactMeasureUnit = GetActualMeasureUnit();
			if (!Loading) {
				if (ScaleMode == ScaleMode.Automatic)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectNumericGridAlignmentPropertyUsing));
				if (ScaleMode == ScaleMode.Manual && !AutoGrid && (exactMeasureUnit > exactGridAlignment))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectNumericGridAlignment));
			}
		}
		void ValidateMeasureUnit(double customMeasureUnit) {
			if (customMeasureUnit <= 0)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectNumericMeasureUnit));
			if (!Loading && (ScaleMode != ScaleMode.Manual))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectNumericMeasureUnitPropertyUsing));
		}
		protected override void HandleGridSpacingChange(ValueChangeInfo<double> changeInfo) {
			OnScaleChange(null, null, null, changeInfo);
		}
		protected override void HandleScaleModeChanged(ValueChangeInfo<ScaleMode> changeInfo) {
			OnScaleChange(changeInfo, null, null, null);
		}
		protected override void PushAutomaticGridToProperties() {
			if (autoGridAlignment == 1d)
				gridAlignment = NumericGridAlignment.Ones;
			else if (autoGridAlignment == 10d)
				gridAlignment = NumericGridAlignment.Tens;
			else if (autoGridAlignment == 100d)
				gridAlignment = NumericGridAlignment.Hundreds;
			else if (autoGridAlignment == 1000d)
				gridAlignment = NumericGridAlignment.Thousands;
			else if (autoGridAlignment == 1000000d)
				gridAlignment = NumericGridAlignment.Millions;
			else if (autoGridAlignment == 1000000000d)
				gridAlignment = NumericGridAlignment.Billions;
			else {
				customGridAlignment = autoGridAlignment;
				gridAlignment = NumericGridAlignment.Custom;
			}
		}
		protected override ChartElement CreateObjectForClone() {
			return new NumericScaleOptions(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			NumericScaleOptions options = obj as NumericScaleOptions;
			if (options != null) {
				gridAlignment = options.gridAlignment;
				measureUnit = options.measureUnit;
				customGridAlignment = options.customGridAlignment;
				customMeasureUnit = options.customMeasureUnit;
			}
		}
	}
}
