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

using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.XtraCharts.Design {
	public class DateTimeScaleOptionsTypeConverter : ScaleOptionBaseConverter {
		protected override string[] GetAutomaticModeFilter(ScaleOptionsBase options) {
			return new string[] { "GridAlignment", "MeasureUnit" };
		}
		protected override string[] GetManualModeFilter(ScaleOptionsBase options) {
			return CheckIsAxisX(options.Axis) ? new string[] { } : new string[] { "MeasureUnit" };
		}
		protected override string[] GetContinuousModeFilter(ScaleOptionsBase options) {
			return new string[] { "MeasureUnit" };
		}
		protected override string[] GetPolarFilter(ScaleOptionsBase options) {
			return new string[] { "AutoGrid", "GridSpacing", "GridOffset", "GridAlignment" };
		}
		protected override string[] GetAdditionalFilter(ScaleOptionsBase options) {
			if (!CheckIsAxisX(options.Axis)) {
				return new string[] { "WorkdaysOnly", "WorkdaysOptions" };
			}
			return base.GetAdditionalFilter(options);
		}
	}
}
namespace DevExpress.XtraCharts {
	[
	Obsolete("This enum is obsolete now. Use the DateTimeMeasureUnit enum instead."),
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
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
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
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
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
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
	public interface IDateTimeMeasureUnitsCalculator {
		DateTimeMeasureUnit CalculateMeasureUnit(IEnumerable<Series> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax);
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions, "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(DateTimeScaleOptionsTypeConverter))
	]
	public sealed class DateTimeScaleOptions : ScaleOptionsBase, IDateTimeScaleOptions {
		const bool DefaultWorkdaysOnly = false;
		internal const double LabelLength = 100;
		public const DateTimeMeasureUnit DefaultMeasureUnit = DateTimeMeasureUnit.Day;
		public const DateTimeGridAlignment DefaultGridAlignment = DateTimeGridAlignment.Week;
		bool workdaysOnly = DefaultWorkdaysOnly;
		double autoGridSpacing = DateTimeUnitSelector.DefaultGridSpacing;
		WorkdaysOptions workdaysOptions;
		DateTimeMeasureUnit measureUnit;
		DateTimeGridAlignment gridAlignment;
		DateTimeMeasureUnitNative autoMeasureUnit = DateTimeUnitSelector.DefaultMeasureUnit;
		DateTimeGridAlignmentNative autoGridAlignment = DateTimeUnitSelector.DefaultGridAlignment;
		DateTimeMeasureUnitsCalculatorCore dateTimeMeasureUnitsCalculatorCore;
		IDateTimeMeasureUnitsCalculator automaticMeasureUnitsCalculator;
		bool NeedAutoGridAlignment { get { return (ScaleMode == ScaleMode.Automatic) || AutoGrid; } }
		internal DateTimeMeasureUnitsCalculatorCore DateTimeMeasureUnitsCalculatorCore {
			get {
				if (dateTimeMeasureUnitsCalculatorCore == null && Axis != null)
					dateTimeMeasureUnitsCalculatorCore = new DateTimeMeasureUnitsCalculatorCore(Axis, DateTimeScaleOptions.LabelLength);
				return dateTimeMeasureUnitsCalculatorCore;
			}
		}
		protected override ScaleMode DefaultScaleMode { get { return (Axis == null) ? ScaleMode.Manual : Axis.DefaultDateTimeScaleMode; } }
		protected internal override bool IsSmartScale { get { return ScaleMode == ScaleMode.Automatic; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DateTimeScaleOptionsMeasureUnit"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DateTimeScaleOptions.MeasureUnit"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DateTimeMeasureUnit MeasureUnit {
			get {
				return (ScaleMode == ScaleMode.Automatic) ? (DateTimeMeasureUnit)autoMeasureUnit : measureUnit;
			}
			set {
				if (!Loading && (Axis != null) && Axis is AxisYBase)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgMeasureUnitCanNotBeSetForAxisY));
				if (!Loading && (Axis != null) && (Axis.ActualDateTimeScaleMode != ScaleMode.Manual))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDateTimeMeasureUnitPropertyUsing));
				if (value != measureUnit) {
					SendNotification(new ElementWillChangeNotification(this));
					if ((GridAlignment < (DateTimeGridAlignment)value) || (!Loading && (measureUnit == (DateTimeMeasureUnit)GridAlignment))) {
						gridAlignment = (DateTimeGridAlignment)value;
					}
					ValueChangeInfo<DateTimeMeasureUnit> changeInfo = new ValueChangeInfo<DateTimeMeasureUnit>(measureUnit, value);
					measureUnit = value;
					RaiseControlChanged(new DataAggregationUpdate(Axis));
					if (Axis != null)
						Axis.ResetAutoProperty();
					OnScaleChange(null, changeInfo, null, null);
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DateTimeScaleOptionsGridAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DateTimeScaleOptions.GridAlignment"),
		RefreshProperties(RefreshProperties.All),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public DateTimeGridAlignment GridAlignment {
			get { return NeedAutoGridAlignment ? (DateTimeGridAlignment)autoGridAlignment : gridAlignment; }
			set {
				if ((gridAlignment != value) || AutoGrid) {
					ValidateGridAlignment(value);
					SendNotification(new ElementWillChangeNotification(this));
					AutoGridInternal = false;
					ValueChangeInfo<DateTimeGridAlignment> changeInfo = new ValueChangeInfo<DateTimeGridAlignment>(gridAlignment, value);
					gridAlignment = value;
					RaiseControlChanged(new PropertyUpdateInfo(this.Axis, "GridAlignment"));
					OnScaleChange(null, null, changeInfo, null);
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DateTimeScaleOptionsWorkdaysOnly"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DateTimeScaleOptions.WorkdaysOnly"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool WorkdaysOnly {
			get { return workdaysOnly; }
			set {
				if (value != workdaysOnly) {
					SendNotification(new ElementWillChangeNotification(this));
					workdaysOnly = value;
					RaiseControlChanged(new PropertyUpdateInfo(Axis, "WorkdaysOnly"));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DateTimeScaleOptionsWorkdaysOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DateTimeScaleOptions.WorkdaysOptions"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public WorkdaysOptions WorkdaysOptions { get { return workdaysOptions; } }
		[
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("DateTimeScaleOptionsAutomaticMeasureUnitsCalculator"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public IDateTimeMeasureUnitsCalculator AutomaticMeasureUnitsCalculator {
			get { return automaticMeasureUnitsCalculator; }
			set {
				if (value != automaticMeasureUnitsCalculator) {
					SendNotification(new ElementWillChangeNotification(this));
					automaticMeasureUnitsCalculator = value;
					RaiseControlChanged(new DataAggregationUpdate(Axis));
				}
			}
		}
		internal DateTimeScaleOptions(ChartElement owner) : base(owner) {
			workdaysOptions = new WorkdaysOptions(this);
			measureUnit = DefaultMeasureUnit;
			gridAlignment = DefaultGridAlignment;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "MeasureUnit":
					return ShouldSerializeMeasureUnit();
				case "GridAlignment":
					return ShouldSerializeGridAlignment();
				case "WorkdaysOnly":
					return ShouldSerializeWorkdaysOnly();
				case "WorkdaysOptions":
					return ShouldSerializeWorkdaysOptions();
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
		bool ShouldSerializeWorkdaysOnly() {
			return workdaysOnly != DefaultWorkdaysOnly;
		}
		void ResetWorkdaysOnly() {
			WorkdaysOnly = DefaultWorkdaysOnly;
		}
		bool ShouldSerializeWorkdaysOptions() {
			return workdaysOptions.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize()
				|| ShouldSerializeMeasureUnit()
				|| ShouldSerializeGridAlignment()
				|| ShouldSerializeWorkdaysOnly()
				|| ShouldSerializeWorkdaysOptions();
		}
		#endregion
		#region IDateTimeScaleOptions
		DateTimeGridAlignmentNative IDateTimeScaleOptions.GridAlignment { get { return GetActualGridAlignment(); } }
		DateTimeMeasureUnitNative IScaleOptionsBase<DateTimeMeasureUnitNative>.MeasureUnit { get { return GetActualMeasureUnit(); } }
		IWorkdaysOptions IDateTimeScaleOptions.WorkdaysOptions { get { return workdaysOptions; } }
		DateTimeMeasureUnitsCalculatorCore IDateTimeScaleOptions.Calculator { get { return DateTimeMeasureUnitsCalculatorCore; } }
		bool IScaleOptionsBase<DateTimeMeasureUnitNative>.UseCustomMeasureUnit { get { return automaticMeasureUnitsCalculator != null; } }
		bool IDateTimeScaleOptions.UpdateAutomaticUnits(DateTimeMeasureUnitNative desiredUnit, DateTimeGridAlignmentNative desiredAlignment, double spacing) {
			ValueChangeInfo<DateTimeMeasureUnit> measureChangeInfo = null;
			ValueChangeInfo<DateTimeGridAlignment> alignmentChangeInfo = null;
			ValueChangeInfo<double> spacingChangeInfo = null;
			bool unitWasUpdated = false;
			if (ScaleMode == ScaleMode.Automatic) {
				if (autoMeasureUnit != desiredUnit) {
					measureChangeInfo = new ValueChangeInfo<DateTimeMeasureUnit>((DateTimeMeasureUnit)autoMeasureUnit, (DateTimeMeasureUnit)desiredUnit);
					autoMeasureUnit = desiredUnit;
					unitWasUpdated = true;
				}
			}
			if (NeedAutoGridAlignment) {
				if ((ScaleMode == ScaleMode.Manual) && ((int)this.measureUnit > (int)desiredAlignment)) {
					desiredAlignment = (DateTimeGridAlignmentNative)this.measureUnit;
					spacing = 1;
				}
				if (autoGridAlignment != desiredAlignment || autoGridSpacing != spacing) {
					alignmentChangeInfo = new ValueChangeInfo<DateTimeGridAlignment>((DateTimeGridAlignment)autoGridAlignment, (DateTimeGridAlignment)desiredAlignment);
					spacingChangeInfo = new ValueChangeInfo<double>(autoGridSpacing, spacing);
					autoGridAlignment = desiredAlignment;
					autoGridSpacing = spacing;
				}
				if (GridSpacingInternal != autoGridSpacing)
					GridSpacingInternal = autoGridSpacing;
			}
			OnScaleChange(null, measureChangeInfo, alignmentChangeInfo, spacingChangeInfo);
			return unitWasUpdated;
		}
		void IDateTimeScaleOptions.UpdateAutomaticGrid(DateTimeGridAlignmentNative selectedAlignment, double selectedSpacing) {
			DateTimeMeasureUnitNative actualMeasureUnit = GetActualMeasureUnit();
			if ((DateTimeMeasureUnitNative)selectedAlignment < actualMeasureUnit) {
				selectedAlignment = (DateTimeGridAlignmentNative)actualMeasureUnit;
				selectedSpacing = 1;
			}
			if (!NeedAutoGridAlignment || (selectedAlignment == (DateTimeGridAlignmentNative)GridAlignment && selectedSpacing == GridSpacingInternal))
				return;
			double oldGridSpacingValue = GridSpacingInternal;
			autoGridSpacing = selectedSpacing;
			ValueChangeInfo<double> spacingChangeInfo = oldGridSpacingValue != selectedSpacing ? new ValueChangeInfo<double>(oldGridSpacingValue, selectedSpacing) : null;
			DateTimeGridAlignment oldAlignment = GridAlignment;
			autoGridAlignment = selectedAlignment;
			ValueChangeInfo<DateTimeGridAlignment> alignmentChangeInfo = oldAlignment != GridAlignment ? new ValueChangeInfo<DateTimeGridAlignment>(oldAlignment, GridAlignment) : null;
			OnScaleChange(null, null, alignmentChangeInfo, spacingChangeInfo);
			PushAutomaticGridToProperties();
		}
		DateTimeMeasureUnitNative IScaleOptionsBase<DateTimeMeasureUnitNative>.CalculateCustomMeasureUnit(IEnumerable<ISeries> series, double axisLength, int pixelsPerUnit, double visualMin, double visualMax, double wholeMin, double wholeMax) {
			return (DateTimeMeasureUnitNative)automaticMeasureUnitsCalculator.CalculateMeasureUnit(EnumerateSeries(series), axisLength, pixelsPerUnit, visualMin, visualMax, wholeMin, wholeMax);
		}
		#endregion
		void OnScaleChange(ValueChangeInfo<ScaleMode> scaleModeChange, 
						   ValueChangeInfo<DateTimeMeasureUnit> measureUnitChange, 
						   ValueChangeInfo<DateTimeGridAlignment> gridAlignmentChange, 
						   ValueChangeInfo<double> gridSpacingChange) {
			if ((measureUnitChange != null) || (gridAlignmentChange != null) || (gridSpacingChange != null) || (scaleModeChange != null)) {
				AxisBase axis = Owner as AxisBase;
				if (axis != null) {
					IChartEventsProvider eventsProvider = axis.ChartContainer as IChartEventsProvider;
					if (eventsProvider != null)
						eventsProvider.OnAxisScaleChanged(new DateTimeScaleChangedEventArgs(axis,
							(scaleModeChange == null) ? new ValueChangeInfo<ScaleMode>(ScaleMode) : scaleModeChange,
							(measureUnitChange == null) ? new ValueChangeInfo<DateTimeMeasureUnit>((DateTimeMeasureUnit)GetActualMeasureUnit()) : measureUnitChange,
							(gridAlignmentChange == null) ? new ValueChangeInfo<DateTimeGridAlignment>((DateTimeGridAlignment)GetActualGridAlignment()) : gridAlignmentChange,
							(gridSpacingChange == null) ? new ValueChangeInfo<double>(GridSpacing) : gridSpacingChange));
				}
			}
		}
		void ValidateGridAlignment(DateTimeGridAlignment gridAlignment) {
			if (!Loading && (Axis != null)) {
				if (Axis.ActualDateTimeScaleMode == ScaleMode.Automatic)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDateTimeGridAlignmentPropertyUsing));
				if (Axis.ActualDateTimeScaleMode == ScaleMode.Manual && measureUnit > (DateTimeMeasureUnit)gridAlignment)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDateTimeGridAlignment));
			}
		}
		internal DateTimeMeasureUnitNative GetActualMeasureUnit() {
			switch (ScaleMode) {
				case ScaleMode.Automatic:
					return autoMeasureUnit;
				case ScaleMode.Manual:
					return (DateTimeMeasureUnitNative)measureUnit;
				default:
					return DateTimeMeasureUnitNative.Millisecond;
			}
		}
		internal DateTimeGridAlignmentNative GetActualGridAlignment() {
			if (NeedAutoGridAlignment)
				return autoGridAlignment;
			return (DateTimeGridAlignmentNative)gridAlignment;
		}
		internal void SetMeasureUnitsDirect(DateTimeMeasureUnit measureUnit, DateTimeGridAlignment gridAlignment) {
			AutoGridInternal = false;
			ScaleMode = ScaleMode.Manual;
			this.measureUnit = measureUnit;
			this.gridAlignment = gridAlignment;
			OnScaleChange(null, null, null, null);
		}
		protected override void HandleScaleModeChanged(ValueChangeInfo<ScaleMode> changeInfo) {
			if (ScaleMode == ScaleMode.Manual) {
				measureUnit = DefaultMeasureUnit;
				gridAlignment = DefaultGridAlignment;
				OnScaleChange(changeInfo, null, null, null);
			}
		}
		protected override void HandleGridSpacingChange(ValueChangeInfo<double> changeInfo) {
			OnScaleChange(null, null, null, changeInfo);
		}
		protected override void PushAutomaticGridToProperties() {
			gridAlignment = (DateTimeGridAlignment)autoGridAlignment;
			GridSpacingInternal = autoGridSpacing;
		}
		protected override ChartElement CreateObjectForClone() {
			return new DateTimeScaleOptions(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			DateTimeScaleOptions options = obj as DateTimeScaleOptions;
			if (options != null) {
				workdaysOptions.Assign(options.workdaysOptions);
				gridAlignment = options.gridAlignment;
				measureUnit = options.measureUnit;
				workdaysOnly = options.workdaysOnly;
				autoMeasureUnit = options.autoMeasureUnit;
			}
			OnScaleChange(null, null, null, null);
		}
	}
}
