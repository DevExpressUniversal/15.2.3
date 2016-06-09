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

using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.XtraMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace DevExpress.DashboardCommon.Viewer {
	public enum DeltaColorType { NoIndication = 0, Bad = 1, Warning = 2, Good = 3 }
	public class ChoroplethMapDashboardItemViewControl : MapDashboardItemViewControl {
		public const string SelectionAttributeName = "Selection";
		public const string DeltaColorAttributeName = "DeltaColor";
		const string ToolTipPattern = "{0}: {1}";
		static List<double> DeltaRangeStops = new List<double>() { 0.5, 1.5, 2.5 };
		ChoroplethColorizer Colorizer {
			get {
				if(FileLayer.Colorizer == null)
					FileLayer.Colorizer = new ChoroplethColorizer();
				return (ChoroplethColorizer)FileLayer.Colorizer;
			}
		}
		ChoroplethMapDashboardItemViewModel CurrentChoroplethViewModel { get { return (ChoroplethMapDashboardItemViewModel)CurrentViewModel; } }
		ChoroplethMapViewerDataController DataController { get; set; }
		public ChoroplethMapDashboardItemViewControl(IDashboardMapControl mapControl)
			: base(mapControl) {
		}
		public void Update(ChoroplethMapDashboardItemViewModel viewModel, MultiDimensionalData data, bool dataChanged) {
			bool updateGeometry = dataChanged && ShouldUpdateGeometry(data) || viewModel.ShouldUpdateGeometry(CurrentChoroplethViewModel);
			bool updateLegend = updateGeometry || viewModel.ShouldUpdateLegend(CurrentChoroplethViewModel);
			if(dataChanged)
				DataController = new ChoroplethMapViewerDataController(data, viewModel.AttributeDimensionId);
			if(updateGeometry) {
				IList<MapItem> mapItems = PrepareMapItems(viewModel);
				UpdateColorizerViewer(viewModel, mapItems);
				UpdateData(mapItems);
				UpdateToolTip();
			}
			if(updateLegend)
				UpdateLegend(viewModel);
			OnEndUpdate(viewModel);
		}
		public bool IsSelectionAllowed(MapItem mapItem) {
			return mapItem.Attributes.Select(attr => attr.Name).Contains(SelectionAttributeName);
		}
		public override void FillToolTip(ToolTipControllerShowEventArgs e) {
			MapItem mapItem = (MapItem)e.SelectedObject;
			MapItemAttribute itemAttribute = mapItem.Attributes.FirstOrDefault(attr => attr.Name == CurrentChoroplethViewModel.ToolTipAttributeName);
			if(itemAttribute != null)
				e.Title = itemAttribute.Value.ToString();
			e.ToolTip = null;
			if(CurrentChoroplethViewModel.ChoroplethColorizer != null) {
				MapItemAttribute bindingAttribute = mapItem.Attributes[CurrentChoroplethViewModel.ChoroplethColorizer.AttributeName];
				if(bindingAttribute != null) {
					List<string> toolTipList = new List<string>();
					MapValueColorizerViewModel valueColorizer = CurrentChoroplethViewModel.ChoroplethColorizer as MapValueColorizerViewModel;
					if(valueColorizer != null) {
						string displayText = DataController.GetDisplayText(bindingAttribute.Value, valueColorizer.ValueId);
						if(displayText != null)
							toolTipList.Add(string.Format(ToolTipPattern, valueColorizer.ValueName, displayText));
					}
					MapDeltaColorizerViewModel deltaColorizer = CurrentChoroplethViewModel.ChoroplethColorizer as MapDeltaColorizerViewModel;
					if(deltaColorizer != null) {
						DeltaValue deltaValue = DataController.GetDeltaValue(bindingAttribute.Value, deltaColorizer.DeltaValueId);
						if(deltaValue != null) {
							toolTipList.Add(string.Format(ToolTipPattern, deltaColorizer.ActualValueName, deltaValue.ActualValue.DisplayText));
							MeasureValue deltaAttributeValue = GetDeltaValue(deltaValue, deltaColorizer.DeltaValueType);
							if(deltaAttributeValue != null)
								toolTipList.Add(string.Format(ToolTipPattern, deltaColorizer.DeltaValueName, deltaAttributeValue.DisplayText));
						}
					}
					FillToolTipMeasures(CurrentChoroplethViewModel.TooltipMeasures, toolTipList, bindingAttribute);
					e.ToolTip = string.Join(Environment.NewLine, toolTipList);
				}
			}
		}
		public override IList GetSelection() {
			return FileLayer.SelectedItems
					.Select(selection => selection as MapItem)
					.Where(item => item.Attributes[SelectionAttributeName] != null)
					.Select(item => new[] { item.Attributes[SelectionAttributeName].Value })
					.ToArray();
		}
		public override void UpdateSelection(IList selectedValues) {
			ClearSelection();
			if(selectedValues != null && selectedValues.Count > 0) {
				if(FileLayer != null) {
					List<MapItem> selectedData = ((MapFileDataAdapter)FileLayer.Data).ViewerMapItems
									.Where(item => item.Attributes[SelectionAttributeName] != null)
									.Where(mapItem => selectedValues.Cast<IList>().Any(sel => object.Equals(sel[0], mapItem.Attributes[SelectionAttributeName].Value)))
									.ToList();
					FileLayer.SelectedItems.AddRange(selectedData);
				}
			}
		}
		bool ShouldUpdateGeometry(MultiDimensionalData data) {
			return DataController == null || !DataController.DataIsEmpty || !data.IsEmpty;
		}
		void FillToolTipMeasures(IList<TooltipDataItemViewModel> tooltipMeasuresViewModel, List<string> toolTipList, MapItemAttribute bindingAttribute) {
			if(tooltipMeasuresViewModel != null) {
				foreach(TooltipDataItemViewModel tooltipViewModel in tooltipMeasuresViewModel) {
					string displayText = DataController.GetDisplayText(bindingAttribute.Value, tooltipViewModel.DataId);
					if(displayText != null)
						toolTipList.Add(string.Format(ToolTipPattern, tooltipViewModel.Caption, displayText));
				}
			}
		}
		void ClearSelection() {
			FileLayer.SelectedItems.Clear();
		}
		void UpdateData(IList<MapItem> mapItems) {
			if(FileLayer.Data != null)
				((MapFileDataAdapter)FileLayer.Data).Dispose();
			FileLayer.Data = new MapFileDataAdapter(mapItems);
		}
		void UpdateColorizerViewer(ChoroplethMapDashboardItemViewModel viewModel, IList<MapItem> mapItems) {
			ClearMapColorizer();
			if(viewModel.ChoroplethColorizer != null && DataController.HasRecords) {
				MapValueColorizerViewModel valueColorizerViewModel = viewModel.ChoroplethColorizer as MapValueColorizerViewModel;
				if(valueColorizerViewModel != null)
					UpdateValueColorizerViewer(valueColorizerViewModel, mapItems);
				MapDeltaColorizerViewModel deltaColorizerViewModel = viewModel.ChoroplethColorizer as MapDeltaColorizerViewModel;
				if(deltaColorizerViewModel != null)
					UpdateDeltaColorizerViewer(deltaColorizerViewModel, mapItems);
			}
		}
		void UpdateLegend(ChoroplethMapDashboardItemViewModel viewModel) {
			MapControl.Legends.Clear();
			MapValueColorizerViewModel colorizerModel = viewModel.ChoroplethColorizer as MapValueColorizerViewModel;
			MapLegendViewModel legendModel = viewModel.Legend;
			if(colorizerModel == null || legendModel == null)
				return;
			ColorLegendBase legend = null;
			if(legendModel.Visible && colorizerModel.Colorizer.RangeStops.Count > 0) {
				legend = CreateColorLegend(legendModel, FileLayer);
				MapControl.Legends.Add(legend);
			}
			PrepareLegendsFormatService(legend, DataController.GetMeasureFormat(colorizerModel.ValueId), null, null);
		}
		void UpdateValueColorizerViewer(MapValueColorizerViewModel colorizerModel, IList<MapItem> mapItems) {
			Tuple<double, double> minMax = DataController.GetMinMaxValues(colorizerModel.ValueId);
			List<double> rangeStops = PrepareChoroplethColorizerRangeStops(colorizerModel.Colorizer, minMax.Item1, minMax.Item2);
			List<Color> colors = PrepareChoroplethColorizerColors(colorizerModel.Colorizer, rangeStops);
			PrepareMapColorizer(colors, rangeStops, colorizerModel.ValueName);
			string attributeName = colorizerModel.AttributeName;
			if(attributeName != null) {
				foreach(MapItem item in mapItems) {
					MapItemAttribute bindingAttribute = item.Attributes[attributeName];
					if(bindingAttribute != null) {
						object selectionAttribute = DataController.GetUniqueValue(bindingAttribute.Value);
						if(selectionAttribute != null) {
							item.Attributes.Add(new MapItemAttribute() { Name = SelectionAttributeName, Value = selectionAttribute });
							item.Attributes.Add(new MapItemAttribute() { Name = colorizerModel.ValueName, Value = DataController.GetValue(bindingAttribute.Value, colorizerModel.ValueId) });
						}
					}
				}
			}
		}
		void UpdateDeltaColorizerViewer(MapDeltaColorizerViewModel colorizerModel, IList<MapItem> mapItems) {
			PrepareMapColorizer(MapControl.DeltaColors.ToList(), DeltaRangeStops, DeltaColorAttributeName);
			string attributeName = colorizerModel.AttributeName;
			if(attributeName != null) {
				foreach(MapItem item in mapItems) {
					MapItemAttribute bindingAttribute = item.Attributes[attributeName];
					if(bindingAttribute != null) {
						object selectionAttribute = DataController.GetUniqueValue(bindingAttribute.Value);
						if(selectionAttribute != null) {
							item.Attributes.Add(new MapItemAttribute() { Name = SelectionAttributeName, Value = DataController.GetUniqueValue(bindingAttribute.Value) });
							DeltaValue deltaValue = DataController.GetDeltaValue(bindingAttribute.Value, colorizerModel.DeltaValueId);
							DeltaColorType deltaType = GetDeltaColorType(deltaValue.IndicatorType, deltaValue.IsGood);
							if(deltaType != DeltaColorType.NoIndication)
								item.Attributes.Add(new MapItemAttribute() { Name = DeltaColorAttributeName, Value = (int)deltaType });
						}
					}
				}
			}
		}
		void PrepareMapColorizer(List<Color> colors, List<double> rangeStops, string attributeName) {
			PrepareMapColorizer(Colorizer, colors, rangeStops, attributeName);
		}
		void ClearMapColorizer() {
			ClearMapColorizer(Colorizer);
			Colorizer.ValueProvider = null;
		}
		void UpdateToolTip() {
			IList<MapItem> mapItems = ((MapFileDataAdapter)FileLayer.Data).ViewerMapItems;
			FileLayer.ToolTipPattern = mapItems.Count > 0 && mapItems.Any(mapItem => mapItem.Attributes.Count > 0) ? DefaultToolTipPattern : null;
		}
		DeltaColorType GetDeltaColorType(IndicatorType indicatorType, bool isGood) {
			switch(indicatorType) {
				case IndicatorType.Warning:
					return DeltaColorType.Warning;
				case IndicatorType.DownArrow:
				case IndicatorType.UpArrow:
					return isGood ? DeltaColorType.Good : DeltaColorType.Bad;
				default:
					return DeltaColorType.NoIndication;
			}
		}
		MeasureValue GetDeltaValue(DeltaValue deltaValue, DeltaValueType deltaValueType) {
			switch(deltaValueType) {
				case DeltaValueType.AbsoluteVariation:
					return deltaValue.AbsoluteVariation;
				case DeltaValueType.PercentVariation:
					return deltaValue.PercentVariation;
				case DeltaValueType.PercentOfTarget:
					return deltaValue.PercentOfTarget;
				case DeltaValueType.ActualValue:
				default:
					return null;
			}
		}
	}
}
