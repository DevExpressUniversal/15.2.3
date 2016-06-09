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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native.Data;
namespace DevExpress.DashboardCommon.Viewer {
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class GridMultiDimensionalDataSource : ReadOnlyTypedList {
		public static string DeltaDescriptorPostFix = "_Delta";
		public static string DisplayTextPostfix = "_DisplayText";
		public static string SparklineStartValue = "_StartValue";
		public static string SparklineEndValue = "_EndValue";
		public static string SparklineStartDisplayText = "_StartDisplayText";
		public static string SparklineEndDisplayText = "_EndDisplayText";
		public static string SparklineMinDisplayText = "_MinDisplayText";
		public static string SparklineMaxDisplayText = "_MaxDisplayText";
		public static string UniqueNamePostFix = "_UniqueName";
		readonly IList<AxisPoint> rows;
		readonly Dictionary<string, DecimalMinMaxCalculator> barCalculators = new Dictionary<string, DecimalMinMaxCalculator>();
		readonly GridFormatConditionalStyleSettingsProvider styleSettingsProvider;
		public GridMultiDimensionalDataSource(MultiDimensionalData data, GridDashboardItemViewModel viewModel, ConditionalFormattingModel cfModel) {
			this.styleSettingsProvider = new GridFormatConditionalStyleSettingsProvider(cfModel, data);
			string columnAxisName = viewModel.ColumnAxisName;
			string sparklineAxisName = viewModel.SparklineAxisName;
			rows = new List<AxisPoint>();
			if(viewModel.HasDimensionColumns) 
				rows = data.GetAxisPoints(columnAxisName);
			else if(columnAxisName != null) 
				rows = new[] { data.GetAxisRoot(columnAxisName) };
			foreach(GridColumnViewModel column in viewModel.Columns) {
				switch(column.ColumnType) {
					case GridColumnType.Dimension:
						Properties.Add(new GridDimensionValueMDDataPropertyDescriptor(column.DataId, column.DataType.ToType()));
						Properties.Add(new GridDimensionDisplayTextMDDataPropertyDescriptor(column.DataId, column.DataId + DisplayTextPostfix));
						Properties.Add(new GridDimensionUniqueNameMDDataPropertyDescriptor(column.DataId, column.DataId + UniqueNamePostFix, column.DataType.ToType()));
						break;
					case GridColumnType.Measure:
						GridMeasureValueMDDataPropertyDescriptor measureDescriptor = new GridMeasureValueMDDataPropertyDescriptor(data, column.DataId, column.DataType, rows);
						if(column.DisplayMode == GridColumnDisplayMode.Bar)
							barCalculators.Add(column.DataId, new DecimalMinMaxCalculator(measureDescriptor, this, column.BarViewModel != null ? column.BarViewModel.AlwaysShowZeroLevel : true));
						Properties.Add(measureDescriptor);
						Properties.Add(new GridMeasureDisplayTextMDDataPropertyDescriptor(data, column.DataId, column.DataId + DisplayTextPostfix));
						break;
					case GridColumnType.Delta:
						ReadOnlyPropertyDescriptor descriptor;
						GridDeltaValueMDDataPropertyDescriptor deltaDescriptor = new GridDeltaValueMDDataPropertyDescriptor(data, column.DataId, column.DeltaValueType, column.DataType, rows);
						if(deltaDescriptor.Initialized) {
							descriptor = deltaDescriptor;
							Properties.Add(new GridDeltaDisplayTextMDDataPropertyDescriptor(data, column.DataId, column.DataId + DisplayTextPostfix, column.DeltaValueType));
							Properties.Add(new GridDeltaMDDataPropertyDescriptor(data, column.DataId, column.DataId + DeltaDescriptorPostFix));
						}
						else {
							descriptor = new GridMeasureValueMDDataPropertyDescriptor(data, column.DataId, column.DataType, rows);
							Properties.Add(new GridMeasureDisplayTextMDDataPropertyDescriptor(data, column.DataId, column.DataId + DisplayTextPostfix));
						}
						Properties.Add(descriptor);
						if(column.DisplayMode == GridColumnDisplayMode.Bar)
							barCalculators.Add(column.DataId, new DecimalMinMaxCalculator(descriptor, this, column.BarViewModel.AlwaysShowZeroLevel));
						break;
					case GridColumnType.Sparkline:
						Properties.Add(new GridSparklineValuesPropertyDescriptor(data, column.DataId, sparklineAxisName));
						Properties.Add(new GridSparklineStartValuePropertyDescriptor(data, column.DataId, column.DataId + SparklineStartValue));
						Properties.Add(new GridSparklineStartDisplayTextPropertyDescriptor(data, column.DataId, column.DataId + SparklineStartDisplayText));
						Properties.Add(new GridSparklineEndValuePropertyDescriptor(data, column.DataId, column.DataId + SparklineEndValue));
						Properties.Add(new GridSparklineEndDisplayTextPropertyDescriptor(data, column.DataId, column.DataId + SparklineEndDisplayText));
						Properties.Add(new GridSparklineMinDisplayTextPropertyDescriptor(data, column.DataId, column.DataId + SparklineMinDisplayText, sparklineAxisName));
						Properties.Add(new GridSparklineMaxDisplayTextPropertyDescriptor(data, column.DataId, column.DataId + SparklineMaxDisplayText, sparklineAxisName));
						break;
				}
			}
		}
		public DecimalMinMaxCalculator GetBarCalculator(string dataId) {
			return barCalculators[dataId];
		}
		public string GetDisplayText(string dataId, int dataIndex) {
			return (string)Properties[dataId + DisplayTextPostfix].GetValue(rows[dataIndex]);
		}
		public object GetUniqueValue(string dataId, int dataIndex) {
			return Properties[dataId + UniqueNamePostFix].GetValue(rows[dataIndex]);
		}
		public IEnumerable<int> GetRowIndices(IEnumerable<AxisPoint> points) {
			return points.Select(point => rows.IndexOf(point));
		}
		public Dictionary<object, string> GetDisplayTexts(string columnId) {
			Dictionary<object, string> res = new Dictionary<object, string>();
			PropertyDescriptor valuePD = Properties[columnId];
			PropertyDescriptor displayTextPD = Properties[columnId + DisplayTextPostfix];
			foreach(AxisPoint point in rows) {
				object value = valuePD.GetValue(point);
				if(value != null && !res.ContainsKey(value)) {
					string displayText = (string)displayTextPD.GetValue(point);
					res.Add(value, displayText);
				}
			}
			return res;
		}
		public void FillStyleSettings(GridCustomDrawCellEventArgsBase args) {
			styleSettingsProvider.FillStyleSettings(null, rows[args.RowIndex], args, args.ColumnId);
		}
		protected override object GetItemValue(int index) {
			return rows[index];
		}
		protected override int GetItemsCount() {
			return rows.Count;
		}
	}
}
