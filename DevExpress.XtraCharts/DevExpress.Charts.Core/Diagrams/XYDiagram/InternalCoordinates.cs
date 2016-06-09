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
namespace DevExpress.Charts.Native {
	public struct AxisValuePair {
		public IAxisData Axis;
		public double Value;
		public object NativeValue {
			get { return null;}
		}
		public AxisValuePair(IAxisData axis, double value) {
			Axis = axis;
			Value = value;
		}
	}
	public class InternalCoordinates {
		readonly IPane pane;
		readonly List<AxisValuePair> axesXValues;
		readonly List<AxisValuePair> axesYValues;
		public IPane Pane { get { return pane; } }
		public List<AxisValuePair> AxesXValues { get { return axesXValues; } }
		public List<AxisValuePair> AxesYValues { get { return axesYValues; } }
		public InternalCoordinates(IPane pane) {
			this.pane = pane;
			axesXValues = new List<AxisValuePair>();
			axesYValues = new List<AxisValuePair>();
		}
		double GetValueByAxis(IAxisData axis, List<AxisValuePair> axesValues) {
			foreach (AxisValuePair pair in axesValues)
				if (pair.Axis == axis)
					return pair.Value;
			return double.NaN;
		}
		public void AddAxisXValue(IAxisData axis, double argument) {
			axesXValues.Add(new AxisValuePair(axis, argument));
		}
		public void AddAxisYValue(IAxisData axis, double value) {
			axesYValues.Add(new AxisValuePair(axis, value));
		}
		public double GetValueByAxisY(IAxisData axis) {
			return GetValueByAxis(axis, axesYValues);
		}
		public double GetArgumentByAxisX(IAxisData axis) {
			return GetValueByAxis(axis, axesXValues);
		}
	}
	public struct PointProjectionOnAxis {
		public IAxisData Axis;
		public double Value;
		public object NativeValue {
			get { return Axis.AxisScaleTypeMap.InternalToNative(Value); }
		}
		public PointProjectionOnAxis(IAxisData axis, double value) {
			Axis = axis;
			Value = value;
		}
	}
	public class PointProjectionsForPane {
		readonly IPane pane;
		readonly List<PointProjectionOnAxis> axesXValues = new List<PointProjectionOnAxis>();
		readonly List<PointProjectionOnAxis> axesYValues = new List<PointProjectionOnAxis>();
		public IPane Pane { get { return pane; } }
		public List<PointProjectionOnAxis> AxesXValues { get { return axesXValues; } }
		public List<PointProjectionOnAxis> AxesYValues { get { return axesYValues; } }
		public PointProjectionsForPane(IPane pane, InternalCoordinates cursorCoordinates) {
			this.pane = pane;
			if (cursorCoordinates != null)
				SetValues(cursorCoordinates.AxesXValues, cursorCoordinates.AxesYValues);
		}
		double GetValueByAxis(IAxisData axis, List<PointProjectionOnAxis> axesValues) {
			foreach (PointProjectionOnAxis pair in axesValues)
				if (pair.Axis == axis)
					return pair.Value;
			return double.NaN;
		}
		void SetValues(List<AxisValuePair> axesXValues, List<AxisValuePair> axesYValues) {
			foreach (AxisValuePair valueX in axesXValues) {
				IAxisData axisData = (IAxisData)valueX.Axis;
				double value = axisData.AxisScaleTypeMap.Transformation.TransformBackward(valueX.Value);
				this.axesXValues.Add(new PointProjectionOnAxis(axisData, value));
			}
			foreach (AxisValuePair valueY in axesYValues) {
				IAxisData axisData = (IAxisData)valueY.Axis;
				double value = axisData.AxisScaleTypeMap.Transformation.TransformBackward(valueY.Value);
				this.axesYValues.Add(new PointProjectionOnAxis(axisData, value));
			}
		}
		public void AddAxisXValue(IAxisData axis, double argument) {
			axesXValues.Add(new PointProjectionOnAxis(axis, argument));
		}
		public void AddAxisYValue(IAxisData axis, double value) {
			axesYValues.Add(new PointProjectionOnAxis(axis, value));
		}
		public double GetValueByAxisY(IAxisData axis) {
			return GetValueByAxis(axis, axesYValues);
		}
		public double GetArgumentByAxisX(IAxisData axis) {
			return GetValueByAxis(axis, axesXValues);
		}
	}
}
