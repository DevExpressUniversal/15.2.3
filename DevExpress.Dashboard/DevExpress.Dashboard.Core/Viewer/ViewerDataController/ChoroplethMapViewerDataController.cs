#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public class ChoroplethMapViewerDataController {
		readonly MultiDimensionalData data;
		readonly Dictionary<object, AxisPoint> axisPointsDict = new Dictionary<object, AxisPoint>();
		public bool DataIsEmpty { get { return data.IsEmpty; } }
		public bool HasRecords { get { return axisPointsDict.Count > 0; } }
		public ChoroplethMapViewerDataController(MultiDimensionalData data, string attributeDimensionId) {
			this.data = data;
			if(!data.IsEmpty) {
				DataAxis axis = data.Axes[DashboardDataAxisNames.DefaultAxis];
				DimensionDescriptor bindingDescriptor = data.GetDimensionDescriptorByID(DashboardDataAxisNames.DefaultAxis, attributeDimensionId);
				axisPointsDict = data.GetAxisPointsByDimension(bindingDescriptor).ToDictionary(axisPoint => axisPoint.Value ?? axisPoint.UniqueValue); 
			}
		}
		public object GetValue(object attribute, string measureName) {
			MeasureValue value = GetMeasureValue(attribute, measureName);
			return value != null ? value.Value : null;
		}
		public string GetDisplayText(object attribute, string measureName) {
			MeasureValue value = GetMeasureValue(attribute, measureName);
			return value != null ? value.DisplayText : null;
		}
		public object GetUniqueValue(object attribute) {
			AxisPoint axisPoint;
			if(axisPointsDict.TryGetValue(attribute, out axisPoint))
				return axisPoint.UniqueValue;
			return null;
		}
		public DeltaValue GetDeltaValue(object attribute, string deltaId) {
			AxisPoint axisPoint;
			if(axisPointsDict.TryGetValue(attribute, out axisPoint)) {
				DeltaDescriptor deltaDescriptor = data.GetDeltaDescriptorById(deltaId);
				return data.GetDeltaValue(axisPoint, deltaDescriptor);
			}
			return null;
		}
		public Tuple<double, double> GetMinMaxValues(string measureName) {
			MeasureDescriptor measureDescriptor = data.GetMeasureDescriptorByID(measureName);
			IList<AxisPoint> axisPoints = axisPointsDict.Values.ToList();
			double min = Helper.ConvertToDouble(data.GetValue(axisPoints[0], measureDescriptor).Value);
			double max = min;
			for(int i = 1; i < axisPoints.Count; i++) {
				double value = Helper.ConvertToDouble(data.GetValue(axisPoints[i], measureDescriptor).Value);
				if(value > max)
					max = value;
				if(value < min)
					min = value;
			}
			return new Tuple<double, double>(min, max);
		}
		public ValueFormatViewModel GetMeasureFormat(string valueId) {
			MeasureDescriptor measureDescriptor = data.GetMeasureDescriptorByID(valueId);
			return measureDescriptor.InternalDescriptor.Format;
		}
		MeasureValue GetMeasureValue(object attribute, string measureName) {
			AxisPoint axisPoint;
			if(axisPointsDict.TryGetValue(attribute, out axisPoint)) {
				MeasureDescriptor measureDescriptor = data.GetMeasureDescriptorByID(measureName);
				return data.GetValue(axisPoint, measureDescriptor);
			}
			return null;
		}
	}
}
