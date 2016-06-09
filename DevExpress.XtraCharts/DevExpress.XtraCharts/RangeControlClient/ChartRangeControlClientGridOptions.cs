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
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum ChartRangeControlClientSnapMode {
		Auto,
		ChartMeasureUnit,
		Manual
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum ChartRangeControlClientGridMode {
		Auto,
		ChartGrid,
		Manual
	}
	[TypeConverter(typeof(ChartRangeControlClientGridOptionsTypeConverter))]
	public abstract class ChartRangeControlClientGridOptions : ChartElement {
		const double DefaultGridSpacing = 1.0;
		const double DefaultSnapSpacing = 1.0;
		const double DefaultSnapOffset = 0.0;
		const double DefaultGridOffset = 0.0;
		const ChartRangeControlClientGridMode DefaultGridMode = ChartRangeControlClientGridMode.Auto;
		const ChartRangeControlClientSnapMode DefaultSnapMode = ChartRangeControlClientSnapMode.Auto;
		double gridSpacing = DefaultGridSpacing;
		double snapSpacing = DefaultSnapSpacing;
		double gridOffset = DefaultGridOffset;
		double snapOffset = DefaultSnapOffset;
		string labelFormat;
		ChartRangeControlClientGridMode gridMode = DefaultGridMode;
		ChartRangeControlClientSnapMode snapMode = DefaultSnapMode;
		RangeControlClientGridMapping clientGridMapping;
		IFormatProvider labelFormatProvider;
		IFormatProvider defaultLabelFormatProvider;
		XYDiagram2D Diagram { get { return (XYDiagram2D)Owner; } }
		protected ISupportRangeControl Container { get { return Owner as ISupportRangeControl; } }
		protected abstract double ChartSnapSpacing { get; }
		protected abstract double ChartGridSpacing { get; }
		protected internal bool IsManualGrid { get { return gridMode == ChartRangeControlClientGridMode.Manual; } }
		protected internal bool IsManualSnap { get { return snapMode == ChartRangeControlClientSnapMode.Manual; } }
		protected internal abstract RangeControlGridUnit GridUnit { get; }
		protected internal abstract RangeControlGridUnit SnapUnit { get; }
		internal AxisBase Axis { get { return Diagram.ActualAxisX; } }
		internal RangeControlClientGridMapping ClientGridMapping {
			get {
				if (clientGridMapping == null)
					clientGridMapping = CreateGridMapping();
				return clientGridMapping;
			}
		}
		internal IFormatProvider ActualLabelFormatProvider { get { return labelFormatProvider != null ? labelFormatProvider : defaultLabelFormatProvider; } }
		internal string ActualLabelFormat { get { return string.IsNullOrWhiteSpace(labelFormat) ? "{0}" : "{0:" + labelFormat + "}"; } }
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartRangeControlClientGridOptions.GridMode"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartRangeControlClientGridOptionsGridMode"),
#endif
		TypeConverter(typeof(EnumTypeConverter)),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public ChartRangeControlClientGridMode GridMode {
			get { return gridMode; }
			set {
				if (gridMode != value) {
					SendNotification(new ElementWillChangeNotification(this));
					gridMode = value;
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartRangeControlClientGridOptions.SnapMode"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartRangeControlClientGridOptionsSnapMode"),
#endif
		TypeConverter(typeof(EnumTypeConverter)),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public ChartRangeControlClientSnapMode SnapMode {
			get { return snapMode; }
			set {
				if (snapMode != value) {
					SendNotification(new ElementWillChangeNotification(this));
					snapMode = value;
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartRangeControlClientGridOptions.GridSpacing"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartRangeControlClientGridOptionsGridSpacing"),
#endif
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public double GridSpacing {
			get { return GetActualGridSpacing(); }
			set {
				if (gridSpacing != value) {
					ValidateSpacing(value, "GridSpacing");
					SendNotification(new ElementWillChangeNotification(this));
					gridSpacing = value;
					DisableAutoGrid();
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartRangeControlClientGridOptions.SnapSpacing"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartRangeControlClientGridOptionsSnapSpacing"),
#endif
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public double SnapSpacing {
			get { return GetActualSnapSpacing(); }
			set {
				if (snapSpacing != value) {
					ValidateSpacing(value, "SnapSpacing");
					SendNotification(new ElementWillChangeNotification(this));
					snapSpacing = value;
					DisableAutoSnap();
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartRangeControlClientGridOptions.GridOffset"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartRangeControlClientGridOptionsGridOffset"),
#endif
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public double GridOffset {
			get { return GetActualGridOffset(); }
			set {
				if (gridOffset != value) {
					ValidateOffset(value, "GridOffset");
					SendNotification(new ElementWillChangeNotification(this));
					gridOffset = value;
					DisableAutoGrid();
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartRangeControlClientGridOptions.SnapOffset"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartRangeControlClientGridOptionsSnapOffset"),
#endif
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public double SnapOffset {
			get { return GetActualSnapOffset(); }
			set {
				if (snapOffset != value) {
					ValidateOffset(value, "SnapOffset");
					SendNotification(new ElementWillChangeNotification(this));
					snapOffset = value;
					DisableAutoSnap();
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartRangeControlClientGridOptions.LabelFormat"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartRangeControlClientGridOptionsLabelFormat"),
#endif
		XtraSerializableProperty
		]
		public string LabelFormat {
			get { return labelFormat; }
			set {
				if (labelFormat != value) {
					SendNotification(new ElementWillChangeNotification(this));
					labelFormat = value;
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartRangeControlClientGridOptions.LabelFormatProvider"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartRangeControlClientGridOptionsLabelFormatProvider"),
#endif
		XtraSerializableProperty
		]
		public IFormatProvider LabelFormatProvider {
			get { return labelFormatProvider; }
			set {
				if (labelFormatProvider != value) {
					SendNotification(new ElementWillChangeNotification(this));
					labelFormatProvider = value;
					RaiseControlChanged();
				}
			}
		}
		public ChartRangeControlClientGridOptions(ChartElement owner) : base(owner) {
			defaultLabelFormatProvider = CreateDefaultLabelFormatProvider();
		}
		#region ShouldSerialize & Reset \ Reset
		bool ShouldSerializeGridSpacing() {
			return IsManualGrid && gridSpacing != DefaultGridSpacing;
		}
		bool ShouldSerializeSnapSpacing() {
			return IsManualSnap && snapSpacing != DefaultSnapSpacing;
		}
		bool ShouldSerializeLabelFormat() {
			return !string.IsNullOrWhiteSpace(labelFormat);
		}
		bool ShouldSerializeGridMode() {
			return gridMode != DefaultGridMode;
		}
		bool ShouldSerializeSnapMode() {
			return snapMode != DefaultSnapMode;
		}
		bool ShouldSerializeLabelFormatProvider() {
			return labelFormatProvider != null;
		}
		bool ShouldSerializeGridOffset() {
			return IsManualGrid && gridOffset != DefaultGridOffset;
		}
		bool ShouldSerializeSnapOffset() {
			return IsManualSnap && snapOffset != DefaultSnapOffset;
		}
		void ResetLabelFormat() {
			LabelFormat = string.Empty;
		}
		void ResetGridSpacing() {
			GridSpacing = DefaultGridSpacing;
		}
		void ResetSnapSpacing() {
			SnapSpacing = DefaultSnapSpacing;
		}
		void ResetGridMode() {
			GridMode = DefaultGridMode;
		}
		void ResetSnapMode() {
			SnapMode = DefaultSnapMode;
		}
		void ResetLabelFormatProvider() {
			LabelFormatProvider = null;
		}
		void ResetGridOffset() {
			GridOffset = DefaultGridOffset;
		}
		void ResetSnapOffset() {
			SnapOffset = DefaultSnapOffset;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeGridMode() || ShouldSerializeSnapMode() || ShouldSerializeGridSpacing() ||
				ShouldSerializeSnapSpacing() || ShouldSerializeLabelFormat() || ShouldSerializeLabelFormatProvider() || ShouldSerializeGridOffset() ||
				ShouldSerializeSnapOffset();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "GridSpacing":
					return ShouldSerializeGridSpacing();
				case "SnapSpacing":
					return ShouldSerializeSnapSpacing();
				case "LabelFormat":
					return ShouldSerializeLabelFormat();
				case "GridMode":
					return ShouldSerializeGridMode();
				case "SnapMode":
					return ShouldSerializeSnapMode();
				case "GridOffset":
					return ShouldSerializeGridOffset();
				case "SnapOffset":
					return ShouldSerializeSnapOffset();
				case "LabelFormatProvider":
					return false;
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		double GetActualGridOffset() {
			if (!IsManualGrid)
				gridOffset = DefaultGridOffset;
			return gridOffset;
		}
		double GetActualSnapOffset() {
			switch (snapMode) {
				case ChartRangeControlClientSnapMode.Auto:
					snapOffset = GridOffset;
					break;
				case ChartRangeControlClientSnapMode.ChartMeasureUnit:
					snapOffset = DefaultSnapOffset;
					break;
				case ChartRangeControlClientSnapMode.Manual:
					break;
				default:
					ChartDebug.Fail("Unknown Snap Mode");
					break;
			}
			return snapOffset;
		}
		double GetActualGridSpacing() {
			if (gridMode == ChartRangeControlClientGridMode.ChartGrid)
				gridSpacing = ChartGridSpacing;
			return gridSpacing;
		}
		double GetActualSnapSpacing() {
			switch (snapMode) {
				case ChartRangeControlClientSnapMode.Auto:
					snapSpacing = GridSpacing;
					break;
				case ChartRangeControlClientSnapMode.ChartMeasureUnit:
					snapSpacing = ChartSnapSpacing;
					break;
				case ChartRangeControlClientSnapMode.Manual:
					break;
				default:
					ChartDebug.Fail("Unknown Snap Mode");
					break;
			}
			return snapSpacing;
		}
		protected internal abstract void UpdateAutoGridSettings(double refinedWholeRange, double scaleLengthInPixels);
		protected abstract RangeControlClientGridMapping CreateGridMapping();
		protected abstract IFormatProvider CreateDefaultLabelFormatProvider();
		protected void DisableAutoGrid() {
			this.gridMode = ChartRangeControlClientGridMode.Manual;
		}
		protected void DisableAutoSnap() {
			this.snapMode = ChartRangeControlClientSnapMode.Manual;
		}
		protected void UpdateAutoSpacing(double value) {
			this.gridSpacing = value;
			this.snapSpacing = SnapSpacing;
		}
		protected virtual void ValidateSpacing(double value, string propertyName) {
			if (value <= 0.0) {
				string pattern = ChartLocalizer.GetString(ChartStringId.MsgIncorrectRangeControlClientSpacing);
				throw new ArgumentException(string.Format(pattern, propertyName));
			}
		}
		protected virtual void ValidateOffset(double value, string propertyName) {
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ChartRangeControlClientGridOptions gridOptions = obj as ChartRangeControlClientGridOptions;
			if (gridOptions != null) {
				gridMode = gridOptions.gridMode;
				snapMode = gridOptions.snapMode;
				gridSpacing = gridOptions.gridSpacing;
				snapSpacing = gridOptions.snapSpacing;
				gridOffset = gridOptions.gridOffset;
				snapOffset = gridOptions.snapOffset;
				labelFormat = gridOptions.labelFormat;
				labelFormatProvider = gridOptions.labelFormatProvider;
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class ChartRangeControlClientQualitativeGridOptions : ChartRangeControlClientGridOptions {
		protected override double ChartGridSpacing {
			get { return 1.0; }
		}
		protected override double ChartSnapSpacing {
			get { return 1.0; }
		}
		protected internal override RangeControlGridUnit GridUnit {
			get { return new RangeControlGridUnit(GridSpacing, GridSpacing, GridOffset); }
		}
		protected internal override RangeControlGridUnit SnapUnit {
			get { return new RangeControlGridUnit(SnapSpacing, SnapSpacing, SnapOffset); }
		}
		public ChartRangeControlClientQualitativeGridOptions(ChartElement owner) : base(owner) {
		}
		protected override RangeControlClientGridMapping CreateGridMapping() {
			return new QualitativeRangeControlClientGridMapping();
		}
		protected override IFormatProvider CreateDefaultLabelFormatProvider() {
			return null;
		}
		protected internal override void UpdateAutoGridSettings(double refinedWholeRange, double scaleLengthInPixels) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new ChartRangeControlClientQualitativeGridOptions(null);
		}
	}
	public partial class GridGenerator {
		public List<object> GenerateGrid(double min, double max, double scaleLength, RangeControlMapping map, ChartRangeControlClientGridOptions options, AxisScaleTypeMap scaleMap) {
			List<object> rangeControlRuler = new List<object>(); 
			double fixedScaleLength = Math.Max(0, scaleLength);
			if (fixedScaleLength > 0.0) {
				if (max <= min)
					return rangeControlRuler;
				if (options.GridMode == ChartRangeControlClientGridMode.Auto) {
					double deltaRange = max - min;
					options.UpdateAutoGridSettings(deltaRange, scaleLength);
				}
				RangeControlGridUnit gridUnit = options.GridUnit;
				if (gridUnit.IsValid) {
					RangeControlClientSnapCalculator snapCalculator = new RangeControlClientSnapCalculator(options, map);
					double currentGridValue = min;
#if DEBUGTEST
					int iterationsCount = 0;
#endif
					while (currentGridValue <= max) {
						double alignedValue = snapCalculator.RoundValue(gridUnit, currentGridValue, false);
						if (map.IsCorrectGridValue(alignedValue))
							rangeControlRuler.Add(scaleMap.RefinedToNative(alignedValue));
						currentGridValue = alignedValue + gridUnit.Step;
#if DEBUGTEST
						iterationsCount++;
						TryTerminateGridGeneration(iterationsCount);
#endif
					}
				}
			}
			return rangeControlRuler;
		}
	}
}
