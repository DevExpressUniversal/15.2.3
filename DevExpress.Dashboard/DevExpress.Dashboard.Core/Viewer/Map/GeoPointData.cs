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
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraMap;
namespace DevExpress.DashboardCommon.Viewer {
	public interface IGeoPointSelection {
		object LatitudeSelection { get; set; }
		object LongitudeSelection { get; set; }
	}
	public abstract class GeoPointDataBase : IGeoPointSelection {
		public object Latitude { get; set; }
		public object Longitude { get; set; }
		public object LatitudeSelection { get; set; }
		public object LongitudeSelection { get; set; }
		public int ClusteredCount { get; set; }
		public IList<string> TooltipDimensions { get; set; }
		public IList<object> TooltipMeasures { get; set; }
		public abstract IList<string> MainTooltip { get; }
		protected GeoPointDataBase() {
		}
		protected GeoPointDataBase(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			Latitude = GetLatitude(viewModel, data, index);
			Longitude = GetLongitude(viewModel, data, index);
			LatitudeSelection = GetLatitudeSelection(data, index);
			LongitudeSelection = GetLongitudeSelection(data, index);
			TooltipMeasures = GetTooltipMeasureValues(viewModel, data, index);
			if(viewModel.EnableClustering)
				ClusteredCount = GetClusterCount(viewModel, data, index);
			TooltipDimensions = ClusteredCount < 2 ? GetTooltipDimensionValues(viewModel, data, index) : new List<string>();
		}
		protected double GetDoubleValue(string property, GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			return Helper.ConvertToDouble(GetValue(property, data, index));
		}
		protected object GetValue(string property, GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			return data.Properties[property].GetValue(data[index]);
		}
		protected string GetDisplayText(string property, GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			return (string)GetValue(property + GeoPointMapViewerDataController.DisplayTextPostFix, data, index);
		}
		protected string FormatMainTooltipItem(string name, string value) {
			return string.Format("{0}: {1}", name, value);
		}
		object GetLatitude(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			return GetValue(viewModel.LatitudeDataId, data, index);
		}
		object GetLongitude(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			return GetValue(viewModel.LongitudeDataId, data, index);
		}
		object GetLatitudeSelection(GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			return GetValue(GeoPointMapViewerDataController.LatitudeSelection, data, index);
		}
		object GetLongitudeSelection(GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			return GetValue(GeoPointMapViewerDataController.LongitudeSelection, data, index);
		}
		IList<string> GetTooltipDimensionValues(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			int dimCount = viewModel.TooltipDimensions != null ? viewModel.TooltipDimensions.Count : 0;
			IList<string> result = new List<string>();
			for(int dimIndex = 0; dimIndex < dimCount; dimIndex++) {
				TooltipDataItemViewModel itemModel = viewModel.TooltipDimensions[dimIndex];
				IEnumerable<object> values = GetValue(itemModel.DataId, data, index) as IEnumerable<object>;
				if(values != null) 
					result.Add(String.Join(Environment.NewLine, values.Distinct()));
			}
			return result;
		}
		IList<object> GetTooltipMeasureValues(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			IList<object> res = new List<object>();
			if(viewModel.TooltipMeasures != null) {
				foreach(TooltipDataItemViewModel tooltipMeasureViewModel in viewModel.TooltipMeasures)
					res.Add(GetValue(tooltipMeasureViewModel.DataId, data, index));
			}
			return res;
		}
		int GetClusterCount(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data, int index) {
			return Helper.ConvertToInt32(GetValue(viewModel.PointsCountDataId, data, index));
		}
	}
	public class GeoPointData : GeoPointDataBase {
		readonly string valueName;
		public string DisplayText { get; set; }
		public object Value { get; set; }
		public MapItemType MapItemType { get; private set; }
		public override IList<string> MainTooltip { get { return new[] { FormatMainTooltipItem(valueName, DisplayText) }; } }
		public GeoPointData() {
		}
		public GeoPointData(GeoPointMapDashboardItemViewModel viewModel, GeoPointMapMultiDimensionalDataSource data, int index)
			: base(viewModel, data, index) {
			Value = GetValue(viewModel.ValueId, data, index);
			MapItemType = ClusteredCount > 1 ? MapItemType.Bubble : MapItemType.Callout;
			DisplayText = GetDisplayText(viewModel.ValueId, data, index);
			valueName = viewModel.ValueName;
		}
	}
	public class DashboardMapBubble : GeoPointDataBase {
		readonly string weightName;
		readonly string colorName;
		public double Weight { get; set; }
		public double Color { get; set; }
		public string WeightDisplayText { get; set; }
		public string ColorDisplayText { get; set; }
		public override IList<string> MainTooltip {
			get {
				List<string> tooltTip = new List<string>();
				if(WeightDisplayText != null)
					tooltTip.Add(FormatMainTooltipItem(weightName, WeightDisplayText));
				if(ColorDisplayText != null)
					tooltTip.Add(FormatMainTooltipItem(colorName, ColorDisplayText));
				return tooltTip;
			}
		}
		public DashboardMapBubble() {
		}
		public DashboardMapBubble(BubbleMapDashboardItemViewModel viewModel, BubbleMapMultiDimensionalDataSource data, int index)
			: base(viewModel, data, index) {
			Weight = GetWeight(viewModel, data, index);
			Color = GetColor(viewModel, data, index);
			WeightDisplayText = GetWeightDisplayText(viewModel, data, index);
			ColorDisplayText = GetColorDisplayText(viewModel, data, index);
			weightName = viewModel.WeightName;
			colorName = viewModel.ColorName;
		}
		double GetColor(BubbleMapDashboardItemViewModel bubbleViewModel, BubbleMapMultiDimensionalDataSource data, int index) {
			return !string.IsNullOrEmpty(bubbleViewModel.ColorId) ? GetDoubleValue(bubbleViewModel.ColorId, data, index) : 0;
		}
		double GetWeight(BubbleMapDashboardItemViewModel bubbleViewModel, BubbleMapMultiDimensionalDataSource data, int index) {
			return !string.IsNullOrEmpty(bubbleViewModel.WeightId) ? GetDoubleValue(bubbleViewModel.WeightId, data, index) : ClusteredCount;
		}
		string GetColorDisplayText(BubbleMapDashboardItemViewModel bubbleViewModel, BubbleMapMultiDimensionalDataSource data, int index) {
			return !string.IsNullOrEmpty(bubbleViewModel.ColorId) ? GetDisplayText(bubbleViewModel.ColorId, data, index) : null;
		}
		string GetWeightDisplayText(BubbleMapDashboardItemViewModel bubbleViewModel, BubbleMapMultiDimensionalDataSource data, int index) {
			return !string.IsNullOrEmpty(bubbleViewModel.WeightId) ? GetDisplayText(bubbleViewModel.WeightId, data, index) : null;
		}
	}
	public class DashboardMapPieArgument : IComparable {
		readonly object value;
		readonly string displayText;
		int index;
		public object Value { get { return value; } }
		public string DisplayText { get { return displayText; } }
		public int Index { get { return index; } set { index = value; } }
		public DashboardMapPieArgument(object argument, string displayText) {
			this.value = argument;
			this.displayText = displayText;
		}
		public DashboardMapPieArgument(object argument, string displayText, int index)
			: this(argument, displayText) {
			this.index = index;
		}
		public override bool Equals(object obj) {
			DashboardMapPieArgument pieArgument = obj as DashboardMapPieArgument;
			return pieArgument != null && pieArgument.Index == index;
		}
		public override int GetHashCode() {					   
			return index;
		}
		int IComparable.CompareTo(object obj) {
			if(obj == null)
				return 1;
			DashboardMapPieArgument pieArgument = obj as DashboardMapPieArgument;
			if(pieArgument != null) {
				if(index < pieArgument.Index)
					return -1;
				if(index > pieArgument.Index)
					return 1;
				return 0;
			}
			else
				throw new ArgumentException();
		}
	}
	public class DashboardMapPieSegment : GeoPointDataBase {
		public DashboardMapPieArgument Argument { get; private set; }
		public double Value { get; set; }
		public string ValueDisplayText { get; set; }
		public string PieItemDataMember { get; private set; }
		public override IList<string> MainTooltip {
			get {
				return new[] { ValueDisplayText != null ? string.Format("{0}: {1}", Argument.DisplayText, ValueDisplayText) : Argument.DisplayText };
			}
		}
		public DashboardMapPieSegment() {
		}
		public DashboardMapPieSegment(PieMapDashboardItemViewModel viewModel, PieMapMultiDimensionalDataSource data, int index)
			: base(viewModel, data, index) {
			PieItemDataMember = String.Format("{0};{1}", Latitude, Longitude);
		}
		public void SetPieArgumentIndex(int index) {
			Argument.Index = index;
		}
		public void Fill(PieMapDashboardItemViewModel pieViewModel, PieMapMultiDimensionalDataSource data, int index) {
			Argument = new DashboardMapPieArgument(GetArgument(pieViewModel, data, index), GetArgumentDisplayText(pieViewModel, data, index));
			Value = GetArgumentValue(pieViewModel, data, index);
			ValueDisplayText = GetDisplayText(pieViewModel, data, index);
		}
		public void Fill(string valueId, PieMapMultiDimensionalDataSource data, int index, int pieArgumentIndex) {
			Argument = new DashboardMapPieArgument(GetArgument(valueId, data, index), GetArgumentDisplayText(valueId, data, index), pieArgumentIndex);
			Value = GetArgumentValue(valueId, data, index);
			ValueDisplayText = GetDisplayText(valueId, data, index);
		}
		object GetArgument(PieMapDashboardItemViewModel pieViewModel, PieMapMultiDimensionalDataSource data, int index) {
			return GetValue(pieViewModel.ArgumentDataId, data, index);
		}
		object GetArgument(string valueId, PieMapMultiDimensionalDataSource data, int index) {
			return GetValue(valueId + PieMapMultiDimensionalDataSource.ArgumentValuePostFix, data, index);
		}
		string GetArgumentDisplayText(PieMapDashboardItemViewModel pieViewModel, PieMapMultiDimensionalDataSource data, int index) {
			return (string)GetValue(pieViewModel.ArgumentDataId + PieMapMultiDimensionalDataSource.ArgumentDisplayTextPostFix, data, index);
		}
		string GetArgumentDisplayText(string valueId, PieMapMultiDimensionalDataSource data, int index) {
			return (string)GetValue(valueId + PieMapMultiDimensionalDataSource.ArgumentDisplayTextPostFix, data, index);
		}
		double GetArgumentValue(PieMapDashboardItemViewModel pieViewModel, PieMapMultiDimensionalDataSource data, int index) {
			return pieViewModel.Values.Count > 0 ? GetDoubleValue(pieViewModel.Values[0], data, index) : Math.Max(ClusteredCount, 1);
		}
		double GetArgumentValue(string valueId, PieMapMultiDimensionalDataSource data, int index) {
			return GetDoubleValue(valueId, data, index);
		}
		string GetDisplayText(PieMapDashboardItemViewModel pieViewModel, PieMapMultiDimensionalDataSource data, int index) {
			return pieViewModel.Values.Count > 0 ? GetDisplayText(pieViewModel.Values[0], data, index) : null;
		}
	}	
}
