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
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using System;
using System.ComponentModel;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	public class ChartRangeControlClientDateTimeGridOptions : ChartRangeControlClientGridOptions {
		const DateTimeGridAlignment DefaultGridAlignment = DateTimeGridAlignment.Millisecond;
		const DateTimeGridAlignment DefaultSnapAlignment = DateTimeGridAlignment.Millisecond;
		DateTimeGridAlignment gridAlignment = DefaultGridAlignment;
		DateTimeGridAlignment snapAlignment = DefaultSnapAlignment;
		DateTimeScaleOptions ScaleOptions { get { return Axis.DateTimeScaleOptions; } }
		protected override double ChartGridSpacing {
			get { return ScaleOptions.GridSpacing; }
		}
		protected override double ChartSnapSpacing {
			get { return 1.0; }
		}
		protected internal override RangeControlGridUnit GridUnit {
			get { return new AlignedRangeControlGridUnit((DateTimeMeasureUnitNative)GridAlignment, GridSpacing, CalculateGridStep(), GridOffset); }
		}
		protected internal override RangeControlGridUnit SnapUnit {
			get { return new AlignedRangeControlGridUnit((DateTimeMeasureUnitNative)SnapAlignment, SnapSpacing, CalculateSnapStep(), SnapOffset); }
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartRangeControlClientDateTimeGridOptions.GridAlignment"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartRangeControlClientDateTimeGridOptionsGridAlignment"),
#endif
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public DateTimeGridAlignment GridAlignment {
			get { return GetActualGridAlignment(); }
			set {
				if (gridAlignment != value) {
					SendNotification(new ElementWillChangeNotification(this));
					gridAlignment = value;
					DisableAutoGrid();
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartRangeControlClientDateTimeGridOptions.SnapAlignment"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartRangeControlClientDateTimeGridOptionsSnapAlignment"),
#endif
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public DateTimeGridAlignment SnapAlignment {
			get { return GetActualSnapAlignment(); }
			set {
				if (snapAlignment != value) {
					SendNotification(new ElementWillChangeNotification(this));
					snapAlignment = value;
					DisableAutoSnap();
					RaiseControlChanged();
				}
			}
		}
		public ChartRangeControlClientDateTimeGridOptions(ChartElement owner) : base(owner) {
		}
		#region ShouldSerialize & Reset \ Reset
		bool ShouldSerializeGridAlignment() {
			return IsManualGrid && gridAlignment != DefaultGridAlignment;
		}
		bool ShouldSerializeSnapAlignment() {
			return IsManualSnap && snapAlignment != DefaultSnapAlignment;
		}
		void ResetGridAlignment() {
			GridAlignment = DefaultGridAlignment;
		}
		void ResetSnapAlignment() {
			SnapAlignment = DefaultSnapAlignment;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeGridAlignment() || ShouldSerializeSnapAlignment();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "GridAlignment":
					return ShouldSerializeGridAlignment();
				case "SnapAlignment":
					return ShouldSerializeSnapAlignment();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		DateTimeGridAlignment GetActualGridAlignment() {
			if (GridMode == ChartRangeControlClientGridMode.ChartGrid)
				gridAlignment = ScaleOptions.GridAlignment;
			return gridAlignment;
		}
		DateTimeGridAlignment GetActualSnapAlignment() {
			switch (SnapMode) {
				case ChartRangeControlClientSnapMode.Auto:
					snapAlignment = GridAlignment;
					break;
				case ChartRangeControlClientSnapMode.ChartMeasureUnit:
					snapAlignment = (DateTimeGridAlignment)ScaleOptions.MeasureUnit;
					break;
				case ChartRangeControlClientSnapMode.Manual:
					break;
				default:
					ChartDebug.Fail("Unknown Snap Mode");
					break;
			}
			return snapAlignment;
		}
		double CalculateGridStep() {
			return GridSpacing * DateTimeUtilsExt.SizeOfMeasureUnit((DateTimeMeasureUnitNative)GridAlignment);
		}
		double CalculateSnapStep() {
			return SnapSpacing * DateTimeUtilsExt.SizeOfMeasureUnit((DateTimeMeasureUnitNative)SnapAlignment);
		}
		void ValidateDoubleValueAsInteger(double value, string propertyName) {
			if (Convert.ToDouble(Convert.ToInt32(value)) != value) {
				string pattern = ChartLocalizer.GetString(ChartStringId.MsgIncorrectDateTimeRangeControlClientSpacing);
				throw new ArgumentException(string.Format(pattern, propertyName));
			}
		}
		protected internal override void UpdateAutoGridSettings(double refinedWholeRange, double scaleLengthInPixels) {
			double gridAlignmentAuto = MeasureUnitsCalculatorBase.CalculateAutoMeasureUnit(refinedWholeRange, refinedWholeRange, scaleLengthInPixels, Constants.AxisXGridSpacingFactor * 4);
			DateTimeUnitSelector unitSelector = new DateTimeUnitSelector();
			var aligmentUnit = unitSelector.SelectAlignment(gridAlignmentAuto, gridAlignmentAuto);
			this.gridAlignment = (DateTimeGridAlignment)aligmentUnit.Unit;
			this.snapAlignment = SnapAlignment;
			UpdateAutoSpacing(aligmentUnit.Spacing);
		}
		protected override RangeControlClientGridMapping CreateGridMapping() {
			return new DateTimeRangeControlClientGridMapping(ScaleOptions.WorkdaysOptions);
		}
		protected override IFormatProvider CreateDefaultLabelFormatProvider() {
			return new DateTimeLabelFormatter(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new ChartRangeControlClientDateTimeGridOptions(null);
		}
		protected override void ValidateSpacing(double value, string propertyName) {
			base.ValidateSpacing(value, propertyName);
			ValidateDoubleValueAsInteger(value, propertyName);
		}
		protected override void ValidateOffset(double value, string propertyName) {
			base.ValidateOffset(value, propertyName);
			ValidateDoubleValueAsInteger(value, propertyName);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ChartRangeControlClientDateTimeGridOptions gridOptions = obj as ChartRangeControlClientDateTimeGridOptions;
			if (gridOptions != null) {
				gridAlignment = gridOptions.gridAlignment;
				snapAlignment = gridOptions.snapAlignment;
			}
		}
	}
}
