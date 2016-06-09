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
using System.Linq;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.XtraMap;
namespace DevExpress.DashboardCommon.Viewer {
	public class GeoPointMapDashboardItemViewControl : GeoPointMapDashboardItemViewControlBase {
		internal const int MinClusteredBubbleSize = 30;
		internal const int MaxClusteredBubbleSize = 60;
		VectorItemsLayer itemsLayer;
		VectorItemsLayer clusteredBubblesLayer;
		protected override VectorItemsLayer[] DataLayers { get { return new[] { itemsLayer, clusteredBubblesLayer }; } }
		VectorItemsLayer ItemsLayer {
			get {
				if(itemsLayer == null) {
					itemsLayer = new VectorItemsLayer();
					itemsLayer.Data = InitializeDataAdapter();
					itemsLayer.ToolTipPattern = DefaultToolTipPattern;
					MapControl.Layers.Add(itemsLayer);
				}
				return itemsLayer;
			}
		}
		VectorItemsLayer ClusteredBubblesLayer {
			get {
				if(clusteredBubblesLayer == null) {
					clusteredBubblesLayer = new VectorItemsLayer();
					clusteredBubblesLayer.Data = InitializeClusteredBubbleDataAdapter();
					clusteredBubblesLayer.Colorizer = InitializeClusteredBubbleColorizer();
					clusteredBubblesLayer.ToolTipPattern = DefaultToolTipPattern;
					MapControl.Layers.Add(clusteredBubblesLayer);
				}
				return clusteredBubblesLayer;
			}
		}
		ListSourceDataAdapter DataAdapter { get { return (ListSourceDataAdapter)ItemsLayer.Data; } }
		BubbleChartDataAdapter ClusteredBubbleDataAdapter { get { return (BubbleChartDataAdapter)ClusteredBubblesLayer.Data; } }
		ChoroplethColorizer ClusteredBubbleColorizer { get { return (ChoroplethColorizer)ClusteredBubblesLayer.Colorizer; } }
		public GeoPointMapDashboardItemViewControl(IDashboardMapControl mapControl)
			: base(mapControl) {
		}
		public override void FillToolTip(ToolTipControllerShowEventArgs e) {
			if(e.SelectedObject is MapCallout || e.SelectedObject is MapBubble)
				e.SuperTip = GetToolTipItem((MapItem)e.SelectedObject);
			e.Show = e.SuperTip.Items.Count > 0;
		}
		protected override void UpdateData(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data) {
			UpdateGeoPointData((GeoPointMapDashboardItemViewModel)viewModel, (GeoPointMapMultiDimensionalDataSource)data);
		}
		protected override void UpdateLegends(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data) {
		}
		protected override DataSourceAdapterBase GetDataAdapter(VectorItemsLayer layer) {
			if(layer != null) {
				if(layer == itemsLayer)
					return DataAdapter;
				if(layer == clusteredBubblesLayer)
					return ClusteredBubbleDataAdapter;
			}
			return null;
		}
		protected override void ClearDataLayers() {
			if(itemsLayer != null)
				DataAdapter.DataSource = null;
			if(clusteredBubblesLayer != null)
				ClusteredBubbleDataAdapter.DataSource = null;
		}
		ListSourceDataAdapter InitializeDataAdapter() {
			ListSourceDataAdapter dataAdapter = new ListSourceDataAdapter();
			InitializeDataAdapter(dataAdapter);
			dataAdapter.Mappings.Latitude = "Latitude";
			dataAdapter.Mappings.Longitude = "Longitude";
			dataAdapter.Mappings.Text = "DisplayText";
			dataAdapter.Mappings.Type = "MapItemType";
			return dataAdapter;
		}
		BubbleChartDataAdapter InitializeClusteredBubbleDataAdapter() {
			BubbleChartDataAdapter bubbleDataAdapter = new BubbleChartDataAdapter();
			InitializeDataAdapter(bubbleDataAdapter);
			bubbleDataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("ClusteredCount", "ClusteredCount"));
			bubbleDataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("Value", "Value"));
			bubbleDataAdapter.Mappings.Latitude = "Latitude";
			bubbleDataAdapter.Mappings.Longitude = "Longitude";
			bubbleDataAdapter.Mappings.Value = "ClusteredCount";
			bubbleDataAdapter.MeasureRules = new MeasureRules { ValueProvider = new ChartItemValueProvider() };
			return bubbleDataAdapter;
		}
		ChoroplethColorizer InitializeClusteredBubbleColorizer() {
			ChoroplethColorizer colorizer = new ChoroplethColorizer();
			colorizer.ValueProvider = new MapBubbleValueProvider();
			for(int i = 0; i < MapControl.ClusterColors.Count; i++)
				colorizer.ColorItems.Add(new ColorizerColorItem(MapControl.ClusterColors[i]));
			return colorizer;
		}
		void UpdateGeoPointData(GeoPointMapDashboardItemViewModel viewModel, GeoPointMapMultiDimensionalDataSource data) {
			List<GeoPointData> points = new List<GeoPointData>();
			List<GeoPointData> bubbles = new List<GeoPointData>();
			for(int i = 0; i < data.Count; i++) {
				GeoPointData point = new GeoPointData(viewModel, data, i);
				if(point.MapItemType == MapItemType.Bubble)
					bubbles.Add(point);
				else
					points.Add(point);
			}
			DataAdapter.DataSource = points;
			if(viewModel.EnableClustering)
				UpdateClusteredBubbleData(bubbles);
		}
		void UpdateClusteredBubbleData(List<GeoPointData> clusteredBubbles) {
			if(clusteredBubbles.Count == 0)
				return;
			ClusteredBubbleDataAdapter.DataSource = clusteredBubbles;
			ClusteredBubbleDataAdapter.ItemMinSize = MinClusteredBubbleSize;
			ClusteredBubbleDataAdapter.ItemMaxSize = MaxClusteredBubbleSize;
			DoubleCollection weightRangeStops = GetWeightClusterRangeStops(clusteredBubbles.Max(bubble => bubble.ClusteredCount));
			DoubleCollection colorRangeStops = GetColorClusterRangeStops();
			for(int i = 0; i < weightRangeStops.Count - colorRangeStops.Count; i++)
				ClusteredBubbleDataAdapter.ItemMaxSize += 10;
			ClusteredBubbleDataAdapter.MeasureRules.RangeStops.Clear();
			ClusteredBubbleDataAdapter.MeasureRules.RangeStops.AddRange(weightRangeStops);
			ClusteredBubbleColorizer.RangeStops.Clear();
			ClusteredBubbleColorizer.RangeStops.AddRange(colorRangeStops);
		}
	}
}
