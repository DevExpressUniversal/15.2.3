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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public class GridViewerDataController {
		readonly GridMultiDimensionalDataSource dataSource;
		readonly MultiDimensionalData data;
		public GridViewerDataController(GridDashboardItemViewModel viewModel, ConditionalFormattingModel cfModel, MultiDimensionalData data) {
			this.data = data;
			dataSource = new GridMultiDimensionalDataSource(data, viewModel, cfModel);
		}
		public GridMultiDimensionalDataSource GetDataSource() {
			return dataSource;
		}
		public decimal NormalizeValue(string columnId, object value) {
			decimal decimalValue;
			try {
				decimalValue = Convert.ToDecimal(value);
			}
			catch {
				decimalValue = decimal.Zero;
			}
			DecimalMinMaxCalculator calculator = dataSource.GetBarCalculator(columnId);
			return calculator.NormalizeValue(decimalValue);
		}
		public decimal GetZeroPosition(string columnId) {
			return dataSource.GetBarCalculator(columnId).AbsoluteZeroPosition;
		}
		public string GetDisplayText(string dataId, int dataIndex) {
			return dataSource.GetDisplayText(dataId, dataIndex);
		}
		public Dictionary<object, string> GetDisplayTexts(string columnId) {
			return dataSource.GetDisplayTexts(columnId);
		}
		public IEnumerable<int> GetDataSourceIndices(IEnumerable<AxisPointTuple> tuples, string targetAxis) {
			IEnumerable<AxisPoint> points = tuples.Select(tuple=>tuple.GetAxisPoint(targetAxis));
			IEnumerable<AxisPoint> lastLevelPoints = GetLastLevelPoints(points); 
			return dataSource.GetRowIndices(lastLevelPoints);
		}
		public string GetValueByColumnId(string measureColumnId) {
			return data.GetValue(data.GetMeasureDescriptorByID(measureColumnId)).DisplayText;
		}
		public string TryGetValueByColumnId(string measureColumnId) {
			MeasureDescriptor measure = data.GetMeasureDescriptorByID(measureColumnId);
			return measure != null ? data.GetValue(measure).DisplayText : null;
		}
		IEnumerable<AxisPoint> GetLastLevelPoints(IEnumerable<AxisPoint> axisPoints) {
			foreach(AxisPoint axisPoint in axisPoints) {
				if(axisPoint.ChildItems.Count == 0)
					yield return axisPoint;
				foreach(AxisPoint childPoint in GetLastLevelPoints(axisPoint.ChildItems))
					yield return childPoint;
			}
		}
		public string Format(object value, string columnId) {
			return FormatDeltaValue(value, columnId);
		}
		public void FillStyleSettings(GridCustomDrawCellEventArgsBase args) {
			dataSource.FillStyleSettings(args);
		}
		public IEnumerable<object> GetUniqueValues(int dataSourceRowIndex, IList<string> columnIds) {
			return columnIds.Select(columnId => dataSource.GetUniqueValue(columnId, dataSourceRowIndex));
		}
		public ValueFormatViewModel GetColumnFormat(string columnId) {
			ValueFormatViewModel valueFormat = null;
			DimensionDescriptor dimension = data.GetDimensionDescriptorByID(columnId);
			if(dimension != null)
				valueFormat = dimension.InternalDescriptor.Format;
			MeasureDescriptor measure = data.GetMeasureDescriptorByID(columnId);
			if(measure != null)
				valueFormat = measure.InternalDescriptor.Format;
			DeltaDescriptor delta = data.GetDeltaDescriptorById(columnId);
			if(delta != null)
				valueFormat = new ValueFormatViewModel(delta.InternalDescriptor.DisplayFormat);
			return valueFormat;
		}
		string FormatDeltaValue(object value, string columnId) {
			DeltaDescriptor deltaDescriptor = data.GetDeltaDescriptorById(columnId);
			if(deltaDescriptor != null) {
				FormatterBase formatter = FormatterBase.CreateFormatter(new ValueFormatViewModel(deltaDescriptor.InternalDescriptor.DisplayFormat));
				return formatter.Format(value);
			}
			DimensionDescriptor dimensionDescriptor = data.GetDimensionDescriptorByID(columnId);
			if(dimensionDescriptor != null)
				return dimensionDescriptor.Format(value);
			MeasureDescriptor measureDescriptor = data.GetMeasureDescriptorByID(columnId);
			if(measureDescriptor != null)
				return measureDescriptor.Format(value);
			return null;
		}
	}
}
