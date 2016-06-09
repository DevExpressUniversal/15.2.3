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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public sealed class DiagramCoordinates {
		readonly Dictionary<AxisBase, AxisValue> axisValueDictionary = new Dictionary<AxisBase, AxisValue>();
		AxisBase axisX;
		AxisBase axisY;
		XYDiagramPaneBase pane;
		ScaleType argumentScaleType;
		ScaleType valueScaleType;
		string qualitativeArgument;
		double numericalArgument;
		DateTime dateTimeArgument;
		double numericalValue;
		DateTime dateTimeValue;
		internal Dictionary<AxisBase, AxisValue> AxisValueDictionary { get { return axisValueDictionary; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesAxisX")]
#endif
		public AxisBase AxisX { get { return axisX; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesAxisY")]
#endif
		public AxisBase AxisY { get { return axisY; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesPane")]
#endif
		public XYDiagramPaneBase Pane { get { return pane; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesArgumentScaleType")]
#endif
		public ScaleType ArgumentScaleType { get { return argumentScaleType; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesValueScaleType")]
#endif
		public ScaleType ValueScaleType { get { return valueScaleType; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesQualitativeArgument")]
#endif
		public string QualitativeArgument { get { return qualitativeArgument; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesNumericalArgument")]
#endif
		public double NumericalArgument { get { return numericalArgument; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesDateTimeArgument")]
#endif
		public DateTime DateTimeArgument { get { return dateTimeArgument; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesNumericalValue")]
#endif
		public double NumericalValue { get { return numericalValue; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesDateTimeValue")]
#endif
		public DateTime DateTimeValue { get { return dateTimeValue; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DiagramCoordinatesIsEmpty")]
#endif
		public bool IsEmpty {
			get { return argumentScaleType == ScaleType.Qualitative && String.IsNullOrEmpty(qualitativeArgument); }
		}
		internal DiagramCoordinates() {
			argumentScaleType = ScaleType.Qualitative;
			valueScaleType = ScaleType.Numerical;
			qualitativeArgument = String.Empty;
		}
		void SetQualitativeArgument(string argument, double argumentInternal) {
			argumentScaleType = ScaleType.Qualitative;
			qualitativeArgument = argument;
			numericalArgument = argumentInternal;
		}
		void SetNumericalArgument(double argument) {
			argumentScaleType = ScaleType.Numerical;
			numericalArgument = argument;
		}
		void SetDateTimeArgument(DateTime argument, double argumentInternal) {
			argumentScaleType = ScaleType.DateTime;
			dateTimeArgument = axisY.DateTimeScaleOptions.ScaleMode == ScaleMode.Continuous ? argument :
				DateTimeUtils.Round(argument, (DateTimeMeasureUnitNative)axisX.DateTimeScaleOptions.MeasureUnit);
			numericalArgument = argumentInternal;
		}
		void SetNumericalValue(double value) {
			valueScaleType = ScaleType.Numerical;
			numericalValue = value;
		}
		void SetDateTimeValue(DateTime value, double valueInternal) {
			valueScaleType = ScaleType.DateTime;
			dateTimeValue = axisY.DateTimeScaleOptions.ScaleMode == ScaleMode.Continuous ? value :
				DateTimeUtils.Round(value, (DateTimeMeasureUnitNative)axisY.DateTimeScaleOptions.MeasureUnit);
			numericalValue = valueInternal;
		}
		internal void SetAxes(AxisBase axisX, AxisBase axisY) {
			this.axisX = axisX;
			this.axisY = axisY;
		}
		internal void SetPane(XYDiagramPaneBase pane) {
			this.pane = pane;
		}
		internal void SetArgumentAndValue(double argumentInternal, double valueInternal) {
			object value;
			object argument;
			IScaleMap mapX;
			IScaleMap mapY;
			if (RangeHelper.RangeCalculationSwitch && axisY is IAxisData && axisX is IAxisData) {
				mapY = ((IAxisData)axisY).AxisScaleTypeMap;
				mapX = ((IAxisData)axisX).AxisScaleTypeMap;
			}
			else {
				mapY = axisY.ScaleTypeMap;
				mapX = axisX.ScaleTypeMap;
			}
			value = mapY.InternalToNative(mapY.Transformation.TransformBackward(valueInternal));
			argument = mapX.InternalToNative(mapX.Transformation.TransformBackward(argumentInternal));
			switch (axisX.ScaleType) {
				case ActualScaleType.Qualitative:
					SetQualitativeArgument((string)argument, argumentInternal);
					break;
				case ActualScaleType.DateTime:
					SetDateTimeArgument((DateTime)argument, argumentInternal);
					break;
				default:
					SetNumericalArgument((double)argument);
					break;
			}
			switch (axisY.ScaleType) {
				case ActualScaleType.DateTime:
					SetDateTimeValue((DateTime)value, valueInternal);
					break;
				default:
					SetNumericalValue((double)value);
					break;
			}
			SetAxisValue(axisX, argumentInternal);
			SetAxisValue(axisY, valueInternal);
		}
		internal void SetAxisValue(AxisBase axis, double valueInternal) {
			IScaleMap map = ((IAxisData)axis).AxisScaleTypeMap;
			object value = map.InternalToNative(map.Transformation.TransformBackward(valueInternal));
			AxisValue axisValue = new AxisValue();
			switch (axis.ScaleType) {
				case ActualScaleType.Qualitative:
					axisValue.SetQualitativeValue((string)value, valueInternal);
					break;
				case ActualScaleType.DateTime:
					axisValue.SetDateTimeValue((DateTime)value, valueInternal, axis.DateTimeScaleOptions);
					break;
				default:
					axisValue.SetNumericalValue((double)value);
					break;
			}
			AddAxisValue(axis, axisValue);
		}
		internal void AddAxisValue(AxisBase axis, AxisValue axisValue) {
			if (!axisValueDictionary.ContainsKey(axis))
				axisValueDictionary.Add(axis, axisValue);
		}
		internal void AddAxisValue(XYDiagramPaneBase pane, Axis2D axis, int coordinate, int length) {
			if (axis != null) {
				double? internalValue = axis.CalculateInternalValue(pane, coordinate, length);
				if (internalValue.HasValue)
					SetAxisValue(axis, internalValue.Value);
			}
		}
		public AxisValue GetAxisValue(AxisBase axis) {
			AxisValue axisValue;
			if (axisValueDictionary.TryGetValue(axis, out axisValue))
				return axisValue;
			return null;
		}
	}
	public class AxisValue {
		ScaleType scaleType;
		string qualitativeValue;
		double numericalValue;
		DateTime dateTimeValue;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisValueScaleType")]
#endif
		public ScaleType ScaleType { get { return scaleType; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisValueQualitativeValue")]
#endif
		public string QualitativeValue { get { return qualitativeValue; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisValueNumericalValue")]
#endif
		public double NumericalValue { get { return numericalValue; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisValueDateTimeValue")]
#endif
		public DateTime DateTimeValue { get { return dateTimeValue; } }
		internal AxisValue() {
			scaleType = ScaleType.Numerical;
			qualitativeValue = String.Empty;
		}
		internal void SetQualitativeValue(string value, double valueInternal) {
			scaleType = ScaleType.Qualitative;
			qualitativeValue = value;
			numericalValue = valueInternal;
		}
		internal void SetNumericalValue(double value) {
			scaleType = ScaleType.Numerical;
			numericalValue = value;
		}
		internal void SetDateTimeValue(DateTime value, double valueInternal, DateTimeScaleOptions scaleOptions) {
			scaleType = ScaleType.DateTime;
			dateTimeValue = scaleOptions.ScaleMode == ScaleMode.Continuous ? value : DateTimeUtils.Round(value, (DateTimeMeasureUnitNative)scaleOptions.MeasureUnit);
			numericalValue = valueInternal;
		}
	}
}
