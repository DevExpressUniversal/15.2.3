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
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.XtraMap;
using DevExpress.XtraMap.Services;
using MapViewport = DevExpress.DashboardCommon.Native.MapViewport;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class MapDashboardItemViewControl {
		protected const string DefaultToolTipPattern = "ToolTip";
		const double ZoomPadding = 0.1;
		const double MinZoomLevel = 0.1;
		public static VectorItemsLayer CreateFileLayer() { return new VectorItemsLayer(); }
		readonly IDashboardMapControl mapControl; 
		VectorItemsLayer fileLayer;
		MapDashboardItemViewModel currentViewModel;
		object currentData;
		MapViewportState clientViewportState;
		protected VectorItemsLayer FileLayer {
			get {
				if(fileLayer == null) {
					fileLayer = CreateFileLayer();
					mapControl.Layers.Add(fileLayer);
				}
				return fileLayer;
			}
		}
		public MapViewportState ClientViewportState { get { return clientViewportState; } set { clientViewportState = value; } }
		protected internal IDashboardMapControl MapControl { get { return mapControl; } }
		protected MapDashboardItemViewModel CurrentViewModel { get { return currentViewModel; } set { currentViewModel = value; } }
		protected object CurrentData { get { return currentData; } set { currentData = value; } }
		protected MapDashboardItemViewControl(IDashboardMapControl mapControl) {
			this.mapControl = mapControl;
			mapControl.NavigationPanelOptionsVisible = false;
			mapControl.MinZoomLevel = MinZoomLevel;
			if(mapControl.ToolTipController != null)
				mapControl.ToolTipController.BeforeShow += OnToolTipControllerBeforeShow;
		}
		public abstract IList GetSelection();
		public abstract void UpdateSelection(IList selectedValues);
		public void ZoomToCurrentRegion() {
			bool navigationEnabled = mapControl.EnableNavigation;
			mapControl.EnableNavigation = true;
			if(clientViewportState != null)
				SetViewportState(clientViewportState);
			else if(currentViewModel != null) {
				MapViewport viewArea = currentViewModel.Viewport;
				if(!viewArea.IsDefault) {
					SetViewportState(new MapViewportState {
						TopLatitude = viewArea.TopLatitude,
						BottomLatitude = viewArea.BottomLatitude,
						LeftLongitude = viewArea.LeftLongitude,
						RightLongitude = viewArea.RightLongitude,
						CenterPointLatitude = viewArea.CenterPointLatitude,
						CenterPointLongitude = viewArea.CenterPointLongitude
					});
					if(viewArea.CreateViewerPaddings)
						mapControl.ZoomLevel -= ZoomPadding;
				}
			}
			mapControl.EnableNavigation = navigationEnabled;
		}
		public void SetViewportState(MapViewportState state) {
			mapControl.ZoomToRegion(
						new GeoPoint(state.TopLatitude, state.LeftLongitude),
						new GeoPoint(state.BottomLatitude, state.RightLongitude),
						new GeoPoint(state.CenterPointLatitude, state.CenterPointLongitude));
		}
		internal GeoPoint GetCenterPoint() {
			return new GeoPoint(currentViewModel.Viewport.CenterPointLatitude, currentViewModel.Viewport.CenterPointLongitude);
		}
		public abstract void FillToolTip(ToolTipControllerShowEventArgs e);
		protected bool ViewportChanged(MapViewport viewport) {
			bool viewportChanged = CurrentViewModel == null || !Object.Equals(viewport, CurrentViewModel.Viewport);
			if(viewportChanged && clientViewportState != null && !ClientViewportStateChanged(viewport))
				return false;
			return viewportChanged;
		}
		bool ClientViewportStateChanged(MapViewport viewport) {
			return viewport.CreateViewerPaddings ||
				viewport.TopLatitude != clientViewportState.TopLatitude || viewport.BottomLatitude != clientViewportState.BottomLatitude ||
				viewport.LeftLongitude != clientViewportState.LeftLongitude || viewport.RightLongitude != clientViewportState.RightLongitude;
		}
		protected void OnEndUpdate(MapDashboardItemViewModel viewModel) {
			bool viewportChanged = ViewportChanged(viewModel.Viewport);
			CurrentViewModel = viewModel;
			if(viewportChanged) {
				clientViewportState = null;
				ZoomToCurrentRegion();
			}
			mapControl.EnableNavigation = !viewModel.LockNavigation;
			if(viewModel.ShapeTitleAttributeName != null) {
				FileLayer.ShapeTitlesVisibility = VisibilityMode.Auto;
				FileLayer.ShapeTitlesPattern = "{" + viewModel.ShapeTitleAttributeName + "}";
			}
			else
				FileLayer.ShapeTitlesVisibility = VisibilityMode.Hidden;
		}
		protected IList<MapItem> PrepareMapItems(MapDashboardItemViewModel viewModel) {
			IList<MapItem> mapItems = new List<MapItem>();
			if(viewModel.MapItems == null)
				return mapItems;
			foreach(MapShapeItem itemViewModel in viewModel.MapItems) {
				MapItem item = null;
				MapShapeDot dotViewModel = itemViewModel as MapShapeDot;
				if(dotViewModel != null)
					item = PrepareMapDot(dotViewModel);
				MapShapePath pathViewModel = itemViewModel as MapShapePath;
				if(pathViewModel != null)
					item = PrepareMapPath(pathViewModel);
				FillAttributes(item, itemViewModel.Attributes);
				mapItems.Add(item);
			}
			return mapItems;
		}
		protected List<Color> PrepareChoroplethColorizerColors(MapColorizerViewModel colorizerViewModel, List<double> rangeStops) {
			List<Color> colors = colorizerViewModel.Colors != null ? colorizerViewModel.Colors.Select(model => Color.FromArgb(model.R, model.G, model.B)).ToList() : MapControl.GradientColors.ToList();
			if(rangeStops != null) {
				int rangesCount = rangeStops.Count;
				if(colors.Count > rangesCount)
					colors.RemoveRange(rangesCount, colors.Count - rangesCount);
			}
			return colors;
		}
		protected List<double> PrepareChoroplethColorizerRangeStops(MapColorizerViewModel colorizerViewModel, double minValue, double maxValue) {
			List<double> rangeStops = colorizerViewModel.RangeStops;
			if(colorizerViewModel.UsePercentRangeStops)
				rangeStops = rangeStops.Select(r => minValue + ((double)r / 100) * (maxValue - minValue)).ToList();
			return rangeStops;		
		}
		protected void PrepareMapColorizer(ChoroplethColorizer colorizer, List<Color> colors, List<double> rangeStops, string attributeName) {
			PrepareMapColorizer(colorizer, colors, rangeStops);
			colorizer.ValueProvider = new ShapeAttributeValueProvider() { AttributeName = attributeName };
		}
		protected void PrepareMapColorizer(ChoroplethColorizer colorizer, List<Color> colors, List<double> rangeStops) {
			colorizer.RangeStops.AddRange(rangeStops);
			colorizer.ColorItems.AddRange(colors.Select(color => new ColorizerColorItem(color)).ToArray());
		}
		protected void ClearMapColorizer(ChoroplethColorizer colorizer) {
			colorizer.ColorItems.Clear();
			colorizer.RangeStops.Clear();
		}
		protected void PrepareLegendsFormatService(XtraMap.MapLegendBase colorLegend, ValueFormatViewModel colorFormatViewModel, XtraMap.MapLegendBase weightLegend, ValueFormatViewModel weightFormatViewModel) {
			Type serviceType = typeof(IColorizerLegendFormatService);
			MapControl.LegendServiceContainer.RemoveService(serviceType);
			if(colorLegend != null || weightLegend != null) {
				LegendsFormatService service = new LegendsFormatService(colorLegend, colorFormatViewModel, weightLegend, weightFormatViewModel);
				MapControl.LegendServiceContainer.AddService(serviceType, service);
			}
		}
		protected ColorLegendBase CreateColorLegend(MapLegendViewModel legendModel, MapItemsLayerBase mapLayer) {
			ColorLegendBase legend = null;
			if(legendModel.Orientation == MapLegendOrientation.Horizontal)
				legend = new ColorScaleLegend();
			else
				legend = new ColorListLegend();
			PrepareLegend(legend, legendModel.Position, mapLayer);
			return legend;
		}
		protected void PrepareLegend(ItemsLayerLegend legend, MapLegendPosition position, MapItemsLayerBase mapLayer) {
			legend.Header = legend.Description = null;
			legend.ItemStyle.Font = new Font(legend.ItemStyle.Font.Name, 8);
			legend.Alignment = ConvertLegendAlignment(position);
			legend.Layer = mapLayer;
		}
		LegendAlignment ConvertLegendAlignment(MapLegendPosition position) {
			switch(position) {
				case MapLegendPosition.TopCenter:
					return LegendAlignment.TopCenter;
				case MapLegendPosition.TopRight:
					return LegendAlignment.TopRight;
				case MapLegendPosition.BottomLeft:
					return LegendAlignment.BottomLeft;
				case MapLegendPosition.BottomCenter:
					return LegendAlignment.BottomCenter;
				case MapLegendPosition.BottomRight:
					return LegendAlignment.BottomRight;
				default:
					return LegendAlignment.TopLeft;
			}
		}
		void OnToolTipControllerBeforeShow(object sender, ToolTipControllerShowEventArgs e) {
			FillToolTip(e);
		}
		MapDot PrepareMapDot(MapShapeDot shapeDot) {
			MapDot dot = new MapDot();
			dot.Location = new GeoPoint(shapeDot.Latitude, shapeDot.Longitude);
			return dot;
		}
		MapPath PrepareMapPath(MapShapePath shapePath) {
			MapPath path = new MapPath();
			foreach(ShapePathSegment shapeSegment in shapePath.Segments) {
				MapPathSegment segment = new MapPathSegment();
				foreach(MapShapePoint point in shapeSegment.Points)
					segment.Points.Add(new GeoPoint(point.Latitude, point.Longitude));
				path.Segments.Add(segment);
			}
			return path;
		}
		void FillAttributes(MapItem item, List<MapShapeItemAttribute> attributes) {
			foreach (IMapItemAttribute attr in attributes)
				  item.Attributes.Add(new MapItemAttribute() { Name = attr.Name, Value = attr.Value });
		}
	}
	public class LegendsFormatService : IColorizerLegendFormatService {
		readonly XtraMap.MapLegendBase colorLegend;
		readonly XtraMap.MapLegendBase weightLegend;
		readonly FormatterBase colorFormatter;
		readonly FormatterBase weightFormatter;
		public LegendsFormatService(XtraMap.MapLegendBase colorLegend, ValueFormatViewModel colorFormatViewModel, XtraMap.MapLegendBase weightLegend, ValueFormatViewModel weightFormatViewModel) {
			this.colorLegend = colorLegend;
			this.weightLegend = weightLegend;
			if(colorFormatViewModel != null)
				colorFormatter = FormatterBase.CreateFormatter(colorFormatViewModel);
			if(weightFormatViewModel != null)
				weightFormatter = FormatterBase.CreateFormatter(weightFormatViewModel);
		}
		public string FormatLegendItem(XtraMap.MapLegendBase legend, MapLegendItemBase legendItem) {
			if(colorLegend != null && colorLegend.Equals(legend))
				return colorFormatter.Format(legendItem.Value);
			if(weightLegend != null && weightLegend.Equals(legend))
				return weightFormatter.Format(legendItem.Value);
			return legendItem.Text;
		}
	}
}
