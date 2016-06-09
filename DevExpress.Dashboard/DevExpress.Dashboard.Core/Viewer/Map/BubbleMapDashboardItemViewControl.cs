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

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.XtraMap;
namespace DevExpress.DashboardCommon.Viewer {
	public class BubbleMapDashboardItemViewControl : GeoPointMapDashboardItemViewControlBase {
		internal const int MinBubbleSize = 20;
		internal const int MaxBubbleSize = 60;
		internal const int ClusteredBubbleSize = 40;
		internal const int BubbleWeightRangeStopCount = 3;
		VectorItemsLayer bubbleLayer;
		protected override VectorItemsLayer[] DataLayers { get { return new[] { bubbleLayer }; } }
		VectorItemsLayer BubbleLayer {
			get {
				if(bubbleLayer == null) {
					bubbleLayer = new VectorItemsLayer();
					bubbleLayer.Data = InitializeBubbleDataAdapter();
					bubbleLayer.Colorizer = new ChoroplethColorizer { ValueProvider = new ShapeAttributeValueProvider() { AttributeName = "Color" } };
					bubbleLayer.ToolTipPattern = DefaultToolTipPattern;
					MapControl.Layers.Add(bubbleLayer);
				}
				return bubbleLayer;
			}
		}
		BubbleChartDataAdapter BubbleDataAdapter { get { return (BubbleChartDataAdapter)BubbleLayer.Data; } }
		ChoroplethColorizer BubbleColorizer { get { return (ChoroplethColorizer)BubbleLayer.Colorizer; } }
		public BubbleMapDashboardItemViewControl(IDashboardMapControl mapControl)
			: base(mapControl) {
		}
		public override void FillToolTip(ToolTipControllerShowEventArgs e) {
			MapBubble bubble = e.SelectedObject as MapBubble;
			if(bubble != null)
				e.SuperTip = GetToolTipItem(bubble);
			e.Show = e.SuperTip.Items.Count > 0;
		}
		protected override void UpdateData(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data) {
			UpdateBubbleData((BubbleMapDashboardItemViewModel)viewModel, (BubbleMapMultiDimensionalDataSource)data);
		}
		protected override void UpdateLegends(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data) {
			BubbleMapDashboardItemViewModel bubbleModel = (BubbleMapDashboardItemViewModel)viewModel;
			BubbleMapMultiDimensionalDataSource bubbleData = (BubbleMapMultiDimensionalDataSource)data;
			if(bubbleLayer != null) {
				UpdateColorLegend(bubbleModel.ColorLegend, bubbleModel.ColorId != null, bubbleLayer);
				UpdateWeightedLegend(bubbleModel.WeightedLegend, bubbleModel.WeightId != null, bubbleLayer);
				PrepareLegendsFormatService(CurrentColorLegend, bubbleData.GetColorFormat(), CurrentWeightLegend, bubbleData.GetWeightFormat());
			}
		}
		protected override DataSourceAdapterBase GetDataAdapter(VectorItemsLayer layer) {
			if(layer != null && layer == bubbleLayer)
				return BubbleDataAdapter;
			return null;
		}
		protected override void ClearDataLayers() {
			if(bubbleLayer != null)
				BubbleDataAdapter.DataSource = null;
		}
		BubbleChartDataAdapter InitializeBubbleDataAdapter() {
			BubbleChartDataAdapter bubbleDataAdapter = new BubbleChartDataAdapter();
			InitializeDataAdapter(bubbleDataAdapter);
			bubbleDataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("Weight", "Weight"));
			bubbleDataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("Color", "Color"));
			bubbleDataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("ClusteredCount", "ClusteredCount"));
			bubbleDataAdapter.Mappings.Latitude = "Latitude";
			bubbleDataAdapter.Mappings.Longitude = "Longitude";
			bubbleDataAdapter.Mappings.Value = "Weight";
			bubbleDataAdapter.MeasureRules = new MeasureRules { ValueProvider = new ChartItemValueProvider() };
			return bubbleDataAdapter;
		}
		void UpdateBubbleData(BubbleMapDashboardItemViewModel viewModel, BubbleMapMultiDimensionalDataSource data) {
			IList<DashboardMapBubble> bubbles = GetBubbleData(viewModel, data);
			BubbleDataAdapter.DataSource = bubbles;
			if(bubbles.Count == 0)
				return;
			BubbleDataAdapter.ItemMinSize = MinBubbleSize;
			BubbleDataAdapter.ItemMaxSize = viewModel.WeightId != null ? MaxBubbleSize : ClusteredBubbleSize;
			DoubleCollection weightRangeStops = viewModel.WeightId != null ?
				GetRangeStops(bubbles.Min(bubble => bubble.Weight), bubbles.Max(bubble => bubble.Weight), BubbleWeightRangeStopCount) :
				new DoubleCollection { 0, 2 };
			BubbleDataAdapter.MeasureRules.RangeStops.Clear();
			BubbleDataAdapter.MeasureRules.RangeStops.AddRange(weightRangeStops);
			List<double> colorRangeStops = viewModel.ColorId != null ?
				PrepareChoroplethColorizerRangeStops(viewModel.Colorizer, bubbles.Min(bubble => bubble.Color), bubbles.Max(bubble => bubble.Color)) : new List<double> { 0 };
			List<Color> colors = PrepareChoroplethColorizerColors(viewModel.Colorizer, colorRangeStops);
			ClearMapColorizer(BubbleColorizer);
			PrepareMapColorizer(BubbleColorizer, colors, colorRangeStops);
		}
		IList<DashboardMapBubble> GetBubbleData(BubbleMapDashboardItemViewModel viewModel, BubbleMapMultiDimensionalDataSource data) {
			IList<DashboardMapBubble> bubbles = new List<DashboardMapBubble>();
			for(int i = 0; i < data.Count; i++)
				bubbles.Add(new DashboardMapBubble(viewModel, data, i));
			return bubbles;
		}
	}
}
