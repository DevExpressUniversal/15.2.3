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
using DevExpress.Utils;
using DevExpress.XtraMap;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class GeoPointMapDashboardItemViewControlBase : MapDashboardItemViewControl, IMapControlEventsListener {
		ColorLegendBase currentColorLegend;
		SizeLegend currentWeightLegend;
		public event EventHandler ClusteredViewportChanged;
		public bool EnableClustering { get { return CurrentGeoPointViewModel.EnableClustering; } }
		internal ColorLegendBase CurrentColorLegend { get { return currentColorLegend; } }
		internal SizeLegend CurrentWeightLegend { get { return currentWeightLegend; } }
		internal GeoPointMapDashboardItemViewModelBase CurrentGeoPointViewModel { get { return (GeoPointMapDashboardItemViewModelBase)CurrentViewModel; } }
		protected abstract VectorItemsLayer[] DataLayers { get; }
		protected GeoPointMapDashboardItemViewControlBase(IDashboardMapControl mapControl)
			: base(mapControl) {
			mapControl.SetExternalListener(this);
		}
		public void Update(GeoPointMapDashboardItemViewModelBase viewModel, MultiDimensionalData data, bool dataChanged) {
			bool viewportChanged = ViewportChanged(viewModel.Viewport);
			bool currentViewModelExisted = CurrentViewModel != null;
			bool enableClusteringChangedInTrue = currentViewModelExisted && viewModel.EnableClustering && !CurrentGeoPointViewModel.EnableClustering;
			bool updateGeometry = viewModel.ShouldUpdateGeometry(CurrentGeoPointViewModel);
			bool updateData = dataChanged || viewModel.ShouldUpdateData(CurrentGeoPointViewModel);
			bool updateLegends = updateData || viewModel.ShouldUpdateLegends(CurrentGeoPointViewModel);
			if(updateGeometry) {
				IList<MapItem> mapItems = PrepareMapItems(viewModel);
				FileLayer.Data = new MapFileDataAdapter(mapItems);
			}
			GeoPointMapMultiDimensionalDataSourceBase dataSource = new GeoPointMapViewerDataController(viewModel, data).GetDataSource();
			if(updateData) {
				ClearDataLayers();
				UpdateData(viewModel, dataSource);
			}
			if(updateLegends)
				UpdateLegends(viewModel, dataSource);
			OnEndUpdate(viewModel);
			if(currentViewModelExisted && (viewportChanged || enableClusteringChangedInTrue))
				ClusteredViewportChanged(this, new EventArgs());
		}
		public bool IsShapeFileLayer(MapItemsLayerBase layer) {
			return layer == FileLayer;
		}
		public override IList GetSelection() {
			List<object> selectedItems = new List<object>();
			foreach(VectorItemsLayer layer in DataLayers) {
				if(layer != null)
					selectedItems.AddRange(layer.SelectedItems);
			}
			return selectedItems.Select(selection => PrepareSelection(selection)).ToArray();
		}
		public override void UpdateSelection(IList selectedValues) {
			ClearSelection();
			if(selectedValues != null && selectedValues.Count > 0) {
				foreach(VectorItemsLayer layer in DataLayers) {
					var dataAdapter = GetDataAdapter(layer);
					if(layer != null && dataAdapter != null && dataAdapter.DataSource != null)
						layer.SelectedItems.AddRange(GetSelectedData(dataAdapter.DataSource, selectedValues));
				}
			}
		}
		protected SuperToolTip GetToolTipItem(MapItem mapItem) {
			SuperToolTip superTip = new SuperToolTip();
			ConfigureTooltipDimesionsInformation(superTip, mapItem, false);
			if(CurrentGeoPointViewModel.EnableClustering && mapItem.Attributes["ClusteredCount"] != null) {
				int count = (int)mapItem.Attributes["ClusteredCount"].Value;
				if(count > 1)
					AddNumberOfPointsToTooltip(superTip, count);
			}
			IList<string> mainTooltip = mapItem.Attributes["MainTooltip"].Value as IList<string>;
			foreach(string text in mainTooltip)
				superTip.Items.Add(new ToolTipItem() { Text = text });
			ConfigTooltipMeasuresInformation(CurrentGeoPointViewModel.TooltipMeasures, superTip, mapItem, false);
			return superTip;
		}
		protected void ConfigureTooltipDimesionsInformation(SuperToolTip superTip, MapItem mapItem, bool isPie) {
			int dimensionCount = CurrentGeoPointViewModel.TooltipDimensions != null ? CurrentGeoPointViewModel.TooltipDimensions.Count : 0;
			IList<string> dimensionValues = isPie ?
				(IList<string>)((IList)mapItem.Attributes["TooltipDimensions"].Value)[0] :
				(IList<string>)mapItem.Attributes["TooltipDimensions"].Value;
			if(dimensionCount > 0 && dimensionValues.Count > 0) {
				for(int i = 0; i < dimensionCount; i++) {
					if(dimensionCount > 1) {
						ToolTipTitleItem titleItem = new ToolTipTitleItem();
						titleItem.Text = CurrentGeoPointViewModel.TooltipDimensions[i].Caption;
						superTip.Items.Add(titleItem);
						ToolTipItem item = new ToolTipItem();
						item.Text = dimensionValues[i];
						superTip.Items.Add(item);
					}
					else {
						ToolTipTitleItem titleItem = new ToolTipTitleItem();
						titleItem.Text = dimensionValues[i];
						superTip.Items.Add(titleItem);
					}
				}
			}
		}
		protected void ConfigTooltipMeasuresInformation(IList<TooltipDataItemViewModel> tooltipMeasuresViewModel, SuperToolTip superTip, MapItem mapItem, bool isPie) {
			IList<object> measureValues = isPie ?
				(IList<object>)((IList)mapItem.Attributes["TooltipMeasures"].Value)[0] :
				(IList<object>)mapItem.Attributes["TooltipMeasures"].Value;
			if(measureValues != null && measureValues.Count > 0) {
				IList<string> tooltipText = new List<string>();
				for(int i = 0; i < measureValues.Count; i++)
					tooltipText.Add(String.Format("{0}: {1}", tooltipMeasuresViewModel[i].Caption, measureValues[i]));
				ToolTipItem item = new ToolTipItem();
				item.Text = String.Join(Environment.NewLine, tooltipText);
				superTip.Items.Add(item);
			}
		}
		protected void AddNumberOfPointsToTooltip(SuperToolTip superTip, int count) {
			ToolTipTitleItem clusteredCountItem = new ToolTipTitleItem();
			clusteredCountItem.Text = String.Format("{0} points", count);
			superTip.Items.Add(clusteredCountItem);
		}
		protected virtual IList PrepareSelection(object selection) {
			IGeoPointSelection geoPointSelection = selection as IGeoPointSelection;
			if(geoPointSelection != null)
				return new[] { geoPointSelection.LatitudeSelection, geoPointSelection.LongitudeSelection };
			return null;
		}
		protected abstract DataSourceAdapterBase GetDataAdapter(VectorItemsLayer layer);
		protected abstract void ClearDataLayers();
		protected abstract void UpdateData(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data);
		protected abstract void UpdateLegends(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data);
		protected void UpdateColorLegend(MapLegendViewModel legendModel, bool allowCreateLegend, MapItemsLayerBase mapLayer) {
			if(legendModel == null)
				return;
			if(currentColorLegend != null) {
				MapControl.Legends.Remove(currentColorLegend);
				currentColorLegend = null;
			}
			if(legendModel.Visible && allowCreateLegend) {
				currentColorLegend = CreateColorLegend(legendModel, mapLayer);
				MapControl.Legends.Add(currentColorLegend);
			}
		}
		protected void UpdateWeightedLegend(WeightedLegendViewModel legendModel, bool allowCreateLegend, MapItemsLayerBase mapLayer) {
			if(legendModel == null)
				return;
			if(currentWeightLegend != null) {
				MapControl.Legends.Remove(currentWeightLegend);
				currentWeightLegend = null;
			}
			if(legendModel.Visible && allowCreateLegend) {
				currentWeightLegend = CreateWeightLegend(legendModel, mapLayer);
				MapControl.Legends.Add(currentWeightLegend);
			}
		}
		protected DoubleCollection GetColorClusterRangeStops() {
			return new DoubleCollection() { 0, 10, 100, 1000 };
		}
		protected DoubleCollection GetWeightClusterRangeStops(double maxValue) {
			DoubleCollection colorRangeStops = GetColorClusterRangeStops();
			while(maxValue > colorRangeStops.Last()) {
				colorRangeStops.Add(colorRangeStops.Last() * 10);
			}
			return colorRangeStops;
		}
		protected DoubleCollection GetRangeStops(double minValue, double maxValue, int count) {
			DoubleCollection rangeStops = new DoubleCollection();
			for(int i = 0; i <= count; i++)
				rangeStops.Add(minValue + (maxValue - minValue) * i / count);
			return rangeStops;
		}
		protected virtual void NotifyLegendItemCreating(LegendItemCreatingEventArgs e) {
		}
		protected void InitializeDataAdapter(DataSourceAdapterBase dataAdapter) {
			dataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("LatitudeSelection", "LatitudeSelection"));
			dataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("LongitudeSelection", "LongitudeSelection"));
			dataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("TooltipDimensions", "TooltipDimensions"));
			dataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("TooltipMeasures", "TooltipMeasures"));
			dataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("MainTooltip", "MainTooltip"));
		}
		List<GeoPointDataBase> GetSelectedData(object dataSource, IList selectedValues) {
			return ((IEnumerable)dataSource)
				.Cast<GeoPointDataBase>()
				.GroupBy(data => new GeoPoint(Helper.ConvertToDouble(data.Latitude), Helper.ConvertToDouble(data.Longitude)))
				.Select(data => data.First())
				.Where(data => selectedValues.Cast<IList>().Any(sel => object.Equals(sel[0], data.LatitudeSelection) && object.Equals(sel[1], data.LongitudeSelection)))
				.ToList();
		}
		void ClearSelection() {
			foreach(VectorItemsLayer layer in DataLayers) {
				if(layer != null)
					layer.SelectedItems.Clear();
			}
		}
		SizeLegend CreateWeightLegend(WeightedLegendViewModel legendModel, MapItemsLayerBase mapLayer) {
			SizeLegend legend = new SizeLegend();
			legend.Type = legendModel.Type == WeightedLegendType.Linear ? SizeLegendType.Inline : SizeLegendType.Nested;
			PrepareLegend(legend, legendModel.Position, mapLayer);
			return legend;
		}
		void IMapControlEventsListener.NotifyLegendItemCreating(LegendItemCreatingEventArgs e) {
			NotifyLegendItemCreating(e);
		}
		IRenderItemStyle IMapControlEventsListener.NotifyDrawMapItem(MapItem item) {
			IMapControlEventsListener mapControlBaseListener = MapControl.BaseListener;
			return mapControlBaseListener != null ? mapControlBaseListener.NotifyDrawMapItem(item) : null;
		}
		void IMapControlEventsListener.NotifyExportMapItem(ExportMapItemEventArgs args) {
			IMapControlEventsListener mapControlBaseListener = MapControl.BaseListener;
			if(mapControlBaseListener != null)
				mapControlBaseListener.NotifyExportMapItem(args);
		}
		void IMapControlEventsListener.NotifyHyperlinkClick(HyperlinkClickEventArgs args) {
			IMapControlEventsListener mapControlBaseListener = MapControl.BaseListener;
			if(mapControlBaseListener != null)
				mapControlBaseListener.NotifyHyperlinkClick(args);
		}
		void IMapControlEventsListener.NotifyMapItemClick(MapItemClickEventArgs args) {
			IMapControlEventsListener mapControlBaseListener = MapControl.BaseListener;
			if(mapControlBaseListener != null)
				mapControlBaseListener.NotifyMapItemClick(args);
		}
		void IMapControlEventsListener.NotifyMapItemDoubleClick(MapItemClickEventArgs args) {
			IMapControlEventsListener mapControlBaseListener = MapControl.BaseListener;
			if(mapControlBaseListener != null)
				mapControlBaseListener.NotifyMapItemDoubleClick(args);
		}
		void IMapControlEventsListener.NotifySelectionChanged() {
			IMapControlEventsListener mapControlBaseListener = MapControl.BaseListener;
			if(mapControlBaseListener != null)
				mapControlBaseListener.NotifySelectionChanged();
		}
		void IMapControlEventsListener.NotifySelectionChanging(MapSelectionChangingEventArgs args) {
			IMapControlEventsListener mapControlBaseListener = MapControl.BaseListener;
			if(mapControlBaseListener != null)
				mapControlBaseListener.NotifySelectionChanging(args);
		}
		void IMapControlEventsListener.NotifyOverlaysArranged(OverlaysArrangedEventArgs args) {
			IMapControlEventsListener mapControlBaseListener = MapControl.BaseListener;
			if(mapControlBaseListener != null)
				mapControlBaseListener.NotifyOverlaysArranged(args);
		}
	}
}
