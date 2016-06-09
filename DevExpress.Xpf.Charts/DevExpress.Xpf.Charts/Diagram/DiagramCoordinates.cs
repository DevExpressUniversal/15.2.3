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
namespace DevExpress.Xpf.Charts {
	public sealed class DiagramCoordinates {
		readonly Dictionary<Axis2D, AxisValue> axisValueDictionary = new Dictionary<Axis2D, AxisValue>();
		AxisX2D axisX;
		AxisY2D axisY;
		Pane pane;
		ScaleType argumentScaleType;
		ScaleType valueScaleType;
		string qualitativeArgument;
		double numericalArgument;
		DateTime dateTimeArgument;
		double numericalValue;
		DateTime dateTimeValue;
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesAxisX")]
#endif
		public AxisX2D AxisX { get { return axisX; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesAxisY")]
#endif
		public AxisY2D AxisY { get { return axisY; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesPane")]
#endif
		public Pane Pane { get { return pane; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesArgumentScaleType")]
#endif
		public ScaleType ArgumentScaleType { get { return argumentScaleType; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesValueScaleType")]
#endif
		public ScaleType ValueScaleType { get { return valueScaleType; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesQualitativeArgument")]
#endif
		public string QualitativeArgument { get { return qualitativeArgument; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesNumericalArgument")]
#endif
		public double NumericalArgument { get { return numericalArgument; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesDateTimeArgument")]
#endif
		public DateTime DateTimeArgument { get { return dateTimeArgument; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesNumericalValue")]
#endif
		public double NumericalValue { get { return numericalValue; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesDateTimeValue")]
#endif
		public DateTime DateTimeValue { get { return dateTimeValue; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DiagramCoordinatesIsEmpty")]
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
			dateTimeArgument = argument;
			numericalArgument = argumentInternal;
		}
		void SetNumericalValue(double value) {
			valueScaleType = ScaleType.Numerical;
			numericalValue = value;
		}
		void SetDateTimeValue(DateTime value, double valueInternal) {
			valueScaleType = ScaleType.DateTime;
			dateTimeValue = value;
			numericalValue = valueInternal;
		}
		internal void SetAxes(AxisX2D axisX, AxisY2D axisY) {
			this.axisX = axisX;
			this.axisY = axisY;
		}
		internal void SetPane(Pane pane) {
			this.pane = pane;
		}
		internal void SetArgumentAndValue(double argumentInternal, double valueInternal) {
			object value = axisY.ScaleMap.InternalToNative(axisY.ScaleMap.Transformation.TransformBackward(valueInternal));
			object argument = axisX.ScaleMap.InternalToNative(axisX.ScaleMap.Transformation.TransformBackward(argumentInternal));
			switch (axisX.ScaleMap.ScaleType) {
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
			switch (axisY.ScaleMap.ScaleType) {
				case ActualScaleType.DateTime:
					SetDateTimeValue((DateTime)value, valueInternal);
					break;
				default:
					SetNumericalValue((double)value);
					break;
			}
			AddAxisValue(axisX, argumentInternal);
			AddAxisValue(axisY, valueInternal);
		}
		internal void AddAxisValue(Axis2D axis, double valueInternal) {
			if (axis != null) {
				AxisValue axisValue = new AxisValue();
				axisValue.SetValue(axis, valueInternal);
				if (!axisValueDictionary.ContainsKey(axis))
					axisValueDictionary.Add(axis, axisValue);
			}
		}
		public AxisValue GetAxisValue(Axis2D axis) {
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
	[DevExpressXpfChartsLocalizedDescription("AxisValueScaleType")]
#endif
		public ScaleType ScaleType { get { return scaleType; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("AxisValueQualitativeValue")]
#endif
		public string QualitativeValue { get { return qualitativeValue; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("AxisValueNumericalValue")]
#endif
		public double NumericalValue { get { return numericalValue; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("AxisValueDateTimeValue")]
#endif
		public DateTime DateTimeValue { get { return dateTimeValue; } }
		internal AxisValue() {
			scaleType = ScaleType.Numerical;
			qualitativeValue = String.Empty;
		}
		void SetQualitativeValue(string value, double valueInternal) {
			scaleType = ScaleType.Qualitative;
			qualitativeValue = value;
			numericalValue = valueInternal;
		}
		void SetNumericalValue(double value) {
			scaleType = ScaleType.Numerical;
			numericalValue = value;
		}
		void SetDateTimeValue(DateTime value, double valueInternal) {
			scaleType = ScaleType.DateTime;
			dateTimeValue = value;
			numericalValue = valueInternal;
		}
		internal void SetValue(Axis2D axis, double valueInternal) {
			object value = axis.ScaleMap.InternalToNative(valueInternal);
			switch (axis.ScaleMap.ScaleType) {
				case ActualScaleType.Qualitative:
					SetQualitativeValue((string)value, valueInternal);
					break;
				case ActualScaleType.DateTime:
					SetDateTimeValue((DateTime)value, valueInternal);
					break;
				default:
					SetNumericalValue((double)value);
					break;
			}
		}
	}
}
