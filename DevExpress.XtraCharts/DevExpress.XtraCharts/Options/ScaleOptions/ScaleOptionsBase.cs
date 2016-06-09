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
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public class ScaleOptionBaseConverter : ExpandableObjectConverter {
		protected bool CheckIsAxisX(AxisBase axis) {
			return axis != null && !axis.IsValuesAxis;
		}
		protected virtual string[] GetAutomaticModeFilter(ScaleOptionsBase options) {
			return new string[] { };
		}
		protected virtual string[] GetManualModeFilter(ScaleOptionsBase options) {
			return new string[] { };
		}
		protected virtual string[] GetContinuousModeFilter(ScaleOptionsBase options) {
			return new string[] { };
		}
		protected virtual string[] GetAdditionalFilter(ScaleOptionsBase options) {
			return new string[] { };
		}
		protected virtual string[] GetPolarFilter(ScaleOptionsBase options) {
			return new string[] { };
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			ScaleOptionsBase options = TypeConverterHelper.GetElement<ScaleOptionsBase>(value);
			List<string> filter = new List<string>();
			if (options != null) {
				if (!CheckIsAxisX(options.Axis)) {
					filter.Add("ScaleMode");
					filter.Add("ProcessMissingPoints");
				}
				if (options.ScaleMode == ScaleMode.Continuous) {
					filter.Add("ProcessMissingPoints");
					filter.Add("AggregateFunction");
				}
				switch (options.ScaleMode) {
					case ScaleMode.Automatic:
						filter.AddRange(GetAutomaticModeFilter(options));
						break;
					case ScaleMode.Manual:
						filter.AddRange(GetManualModeFilter(options));
						break;
					case ScaleMode.Continuous:
						filter.AddRange(GetContinuousModeFilter(options));
						break;
				}
				filter.AddRange(GetAdditionalFilter(options));
				if (options.Axis is PolarAxisX)
					filter.AddRange(GetPolarFilter(options));
			}
			return FilterPropertiesUtils.FilterProperties(collection, filter);
		}
	}
}
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum ScaleMode {
		Automatic = ScaleModeNative.Automatic,
		Manual = ScaleModeNative.Manual,
		Continuous = ScaleModeNative.Continuous
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum AggregateFunction {
		None = AggregateFunctionNative.None,
		Average = AggregateFunctionNative.Average,
		Sum = AggregateFunctionNative.Sum,
		Minimum = AggregateFunctionNative.Minimum,
		Maximum = AggregateFunctionNative.Maximum,
		Count = AggregateFunctionNative.Count,
		Financial = AggregateFunctionNative.Financial
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum ProcessMissingPointsMode {
		Skip = ProcessMissingPointsModeNative.Skip,
		InsertZeroValues = ProcessMissingPointsModeNative.InsertZeroValues,
		InsertEmptyPoints = ProcessMissingPointsModeNative.InsertEmptyPoints
	}
	public abstract class ScaleOptionsBase : ChartElement, IScaleOptionsBase {
		const AggregateFunction DefaultAggregateFunction = AggregateFunction.Average;
		const ProcessMissingPointsMode DefaultProcessMissingPoints = ProcessMissingPointsMode.Skip;
		const bool DefaultAutoGrid = true;
		internal const double DefaultGridSpacing = 1.0;
		internal const double DefaultGridOffset = 0.0;
		ScaleMode scaleMode;
		ProcessMissingPointsMode processMissingPoints = DefaultProcessMissingPoints;
		AggregateFunction aggregateFunction = DefaultAggregateFunction;
		double gridSpacing = DefaultGridSpacing;
		double gridOffset = DefaultGridOffset;
		bool autoGrid = DefaultAutoGrid;
		protected internal AxisBase Axis { get { return base.Owner as AxisBase; } }
		protected abstract ScaleMode DefaultScaleMode { get; }
		protected internal abstract bool IsSmartScale { get; }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleOptionsBaseScaleMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleOptionsBase.ScaleMode"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public ScaleMode ScaleMode {
			get {
				return scaleMode;
			}
			set {
				if (Axis is AxisY)
					throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgAttemptToSetScaleModeForAxisY));
				if (scaleMode != value) {
					SendNotification(new ElementWillChangeNotification(this));
					if (Axis != null && (scaleMode == ScaleMode.Automatic || value == ScaleMode.Automatic))
						Axis.ResetAutoProperty();
					ValueChangeInfo<ScaleMode> changeInfo = new ValueChangeInfo<ScaleMode>(scaleMode, value);
					scaleMode = value;
					RaiseControlChanged(new DataAggregationUpdate(Axis));
					if (!Loading)
						HandleScaleModeChanged(changeInfo);
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleOptionsBaseAggregateFunction"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleOptionsBase.AggregateFunction"),
		RefreshProperties(RefreshProperties.All),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public AggregateFunction AggregateFunction {
			get {
				return aggregateFunction;
			}
			set {
				if (aggregateFunction != value) {
					SendNotification(new ElementWillChangeNotification(this));
					aggregateFunction = value;
					RaiseControlChanged(new DataAggregationUpdate(Axis));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleOptionsBaseProcessMissingPoints"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleOptionsBase.ProcessMissingPoints"),
		RefreshProperties(RefreshProperties.All),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public ProcessMissingPointsMode ProcessMissingPoints {
			get { return processMissingPoints; }
			set {
				if (Axis != null && Axis.IsValuesAxis)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgProcessMissingPointsForValueAxis));
				if (scaleMode == ScaleMode.Continuous)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgProcessMissingPointsForContinuousScale));
				if (processMissingPoints != value) {
					SendNotification(new ElementWillChangeNotification(this));
					processMissingPoints = value;
					RaiseControlChanged(new DataAggregationUpdate(Axis));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleOptionsBaseGridSpacing"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleOptionsBase.GridSpacing"),
		NonTestableProperty(),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double GridSpacing {
			get { return ValidateGridSpacing(gridSpacing); }
			set {
				if (!Axis.IsGridSpacingSupported)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXGridSpacingChanged));
				if (value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectGridSpacing));
				if (value != gridSpacing) {
					SendNotification(new ElementWillChangeNotification(this));
					double oldGridSpacingValue = gridSpacing;
					gridSpacing = value;
					if (!Loading)
						autoGrid = false;
					RaiseControlChanged(new ChartElementUpdateInfo(Axis, ChartElementChange.RangeControlChanged | ChartElementChange.NonSpecific));
					HandleGridSpacingChange(new ValueChangeInfo<double>(oldGridSpacingValue, gridSpacing));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleOptionsBaseAutoGrid"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleOptionsBase.AutoGrid"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NonTestableProperty,
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool AutoGrid {
			get {
				return ValidateAutoGrid(autoGrid);
			}
			set {
				if ((Axis != null) && (!Axis.IsGridSpacingSupported))
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXGridSpacingChanged));
				if (autoGrid != value) {
					SendNotification(new ElementWillChangeNotification(this));
					autoGrid = value;
					if (!autoGrid && !Loading)
						PushAutomaticGridToProperties();
					if (autoGrid && !Loading && (Axis != null))
						Axis.UpdateAutoMeasureUnit(false);
					RaiseControlChanged(new PropertyUpdateInfo(this.Axis, "GridAlignment"));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleOptionsBaseGridOffset"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleOptionsBase.GridOffset"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double GridOffset {
			get { return gridOffset; }
			set {
				if (gridOffset != value) {
					SendNotification(new ElementWillChangeNotification(this));
					gridOffset = value;
					if (!Loading)
						autoGrid = false;
					RaiseControlChanged(new ChartElementUpdateInfo(Axis, ChartElementChange.NonSpecific));
				}
			}
		}
		protected bool AutoGridInternal {
			get {
				return autoGrid;
			}
			set {
				autoGrid = value;
			}
		}
		protected double GridSpacingInternal {
			get { return gridSpacing; }
			set { gridSpacing = value; }
		}
		internal ScaleOptionsBase(ChartElement owner) : base(owner) {
			this.scaleMode = DefaultScaleMode;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "ScaleMode":
					return ShouldSerializeScaleMode();
				case "AggregateFunction":
					return ShouldSerializeAggregateFunction();
				case "AutoGrid":
					return ShouldSerializeAutoGrid();
				case "GridSpacing":
					return ShouldSerializeGridSpacing();
				case "GridOffset":
					return ShouldSerializeGridOffset();
				case "ProcessMissingPoints":
					return ShouldSerializeProcessMissingPoints();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeScaleMode() {
			return scaleMode != DefaultScaleMode;
		}
		void ResetScaleMode() {
			ScaleMode = DefaultScaleMode;
		}
		bool ShouldSerializeAggregateFunction() {
			return (scaleMode != ScaleMode.Continuous) && (aggregateFunction != DefaultAggregateFunction);
		}
		void ResetAggregateFunction() {
			AggregateFunction = DefaultAggregateFunction;
		}
		bool ShouldSerializeGridSpacing() {
			return ShouldSerializeAutoGrid() && gridSpacing != DefaultGridSpacing;
		}
		void ResetGridSpacing() {
			GridSpacing = DefaultGridSpacing;
		}
		bool ShouldSerializeGridOffset() {
			return gridOffset != DefaultGridOffset;
		}
		void ResetGridOffset() {
			GridOffset = DefaultGridOffset;
		}
		bool ShouldSerializeProcessMissingPoints() {
			return (scaleMode != ScaleMode.Continuous) && (processMissingPoints != DefaultProcessMissingPoints);
		}
		void ResetProcessMissingPoints() {
			ProcessMissingPoints = DefaultProcessMissingPoints;
		}
		protected bool ShouldSerializeAutoGrid() {
			return (autoGrid != DefaultAutoGrid) && ((Axis != null) && Axis.IsGridSpacingSupported);
		}
		void ResetAutoGrid() {
			AutoGrid = DefaultAutoGrid;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize()
				|| ShouldSerializeScaleMode()
				|| ShouldSerializeAggregateFunction()
				|| ShouldSerializeAutoGrid()
				|| ShouldSerializeGridSpacing()
				|| ShouldSerializeGridOffset()
				|| ShouldSerializeProcessMissingPoints();
		}
		#endregion
		#region IScaleOptionsBase
		ScaleModeNative IScaleOptionsBase.ScaleMode { get { return (ScaleModeNative)scaleMode; } }
		AggregateFunctionNative IScaleOptionsBase.AggregateFunction { get { return (AggregateFunctionNative)aggregateFunction; } }
		ProcessMissingPointsModeNative IScaleOptionsBase.ProcessMissingPoints { get { return (ProcessMissingPointsModeNative)processMissingPoints; } }
		bool IScaleOptionsBase.GridSpacingAuto { get { return AutoGrid; } }
		double IScaleOptionsBase.GridSpacing {
			get { return GridSpacing; }
			set {
				if (gridSpacing == value)
					return;
				double oldGridSpacingValue = gridSpacing;
				gridSpacing = value;
				HandleGridSpacingChange(new ValueChangeInfo<double>(oldGridSpacingValue, gridSpacing));
			}
		}
		#endregion
		bool ValidateAutoGrid(bool value) {
			return Axis != null ? Axis.GetGridSpacingAutoByUserValue(value) : value;
		}
		double ValidateGridSpacing(double value) {
			return Axis != null ? Axis.GetGridSpacingByUserValue(gridSpacing) : gridSpacing;
		}
		protected IEnumerable<Series> EnumerateSeries(IEnumerable<ISeries> sourceSeries) {
			foreach (ISeries series in sourceSeries) {
				yield return (Series)series;
			}
		}
		protected abstract void HandleGridSpacingChange(ValueChangeInfo<double> changeInfo);
		protected abstract void PushAutomaticGridToProperties();
		protected abstract void HandleScaleModeChanged(ValueChangeInfo<ScaleMode> changeInfo);
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ScaleOptionsBase options = obj as ScaleOptionsBase;
			if (options != null) {
				scaleMode = options.scaleMode;
				aggregateFunction = options.aggregateFunction;
				autoGrid = options.autoGrid;
				gridSpacing = options.gridSpacing;
				gridOffset = options.gridOffset;
				processMissingPoints = options.processMissingPoints;
			}
		}
	}
	public class QualitativeScaleOptions : ScaleOptionsBase {
		protected internal override bool IsSmartScale { get { return false; } }
		protected override ScaleMode DefaultScaleMode { get { return ScaleMode.Continuous; } }
		internal QualitativeScaleOptions(ChartElement owner) : base(owner) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new QualitativeScaleOptions(null);
		}
		protected override void HandleGridSpacingChange(ValueChangeInfo<double> changeInfo) {
		}
		protected override void PushAutomaticGridToProperties() {
		}
		protected override void HandleScaleModeChanged(ValueChangeInfo<ScaleMode> changeInfo) {
		}
	}
}
