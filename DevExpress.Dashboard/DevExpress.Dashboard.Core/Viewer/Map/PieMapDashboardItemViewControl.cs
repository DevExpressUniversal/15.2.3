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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.XtraMap;
namespace DevExpress.DashboardCommon.Viewer {
	public class PieMapDashboardItemViewControl : GeoPointMapDashboardItemViewControlBase {
		internal const int MinPieSize = 20;
		internal const int MaxPieSize = 60;
		internal const int PieValueRangeStopCount = 3;
		VectorItemsLayer pieLayer;
		protected override VectorItemsLayer[] DataLayers { get { return new[] { pieLayer }; } }
		VectorItemsLayer PieLayer {
			get {
				if(pieLayer == null) {
					pieLayer = new VectorItemsLayer();
					pieLayer.Colorizer = new KeyColorColorizer();
					pieLayer.ToolTipPattern = DefaultToolTipPattern;
					MapControl.Layers.Add(pieLayer);
				}
				return pieLayer;
			}
		}
		KeyColorColorizer PieColorizer { get { return (KeyColorColorizer)PieLayer.Colorizer; } }
		PieChartDataAdapter PieDataAdapter { get { return PieLayer.Data as PieChartDataAdapter; } }
		MapItemStorage PieStorageDataAdapter { get { return PieLayer.Data as MapItemStorage; } }
		public PieMapDashboardItemViewControl(IDashboardMapControl mapControl)
			: base(mapControl) {
		}
		public override void FillToolTip(ToolTipControllerShowEventArgs e) {
			MapPie pie = e.SelectedObject as MapPie;
			if(pie != null)
				e.SuperTip = GetPieToolTip(pie);
			e.Show = e.SuperTip.Items.Count > 0;
		}
		public override void UpdateSelection(IList selectedValues) {
			base.UpdateSelection(selectedValues);
			if(selectedValues != null && selectedValues.Count > 0) {
				if(PieStorageDataAdapter != null && PieStorageDataAdapter.Items.Count > 0)
					PieLayer.SelectedItems.AddRange(PieStorageDataAdapter.Items
						.Select(item => item as MapPie)
						.Where(data => selectedValues.Cast<IList>().Any(sel => object.Equals(sel[0], ((IList)data.Attributes["LatitudeSelection"].Value)[0]) &&
																				object.Equals(sel[1], ((IList)data.Attributes["LongitudeSelection"].Value)[0])))
						.ToList());
			}
		}
		protected override void NotifyLegendItemCreating(LegendItemCreatingEventArgs e) {
			if(PieColorizer.Keys.Count > e.Index)
				e.Item.Text = PieColorizer.Keys[e.Index].Name;
		}
		protected override void UpdateData(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data) {
			UpdatePieData((PieMapDashboardItemViewModel)viewModel, (PieMapMultiDimensionalDataSource)data);
		}
		protected override void UpdateLegends(GeoPointMapDashboardItemViewModelBase viewModel, GeoPointMapMultiDimensionalDataSourceBase data) {
			PieMapDashboardItemViewModel pieModel = (PieMapDashboardItemViewModel)viewModel;
			PieMapMultiDimensionalDataSource pieData = (PieMapMultiDimensionalDataSource)data;
			if(pieLayer != null) {
				bool weightedDataExists = PieDataAdapter != null && PieDataAdapter.DataSource != null;
				bool storageDataExists = PieStorageDataAdapter != null && PieStorageDataAdapter.Items.Count > 0;
				UpdateColorLegend(pieModel.ColorLegend, weightedDataExists || storageDataExists, pieLayer);
				UpdateWeightedLegend(pieModel.WeightedLegend, weightedDataExists, pieLayer);
				ValueFormatViewModel format = pieData.GetValueFormat();
				if(format != null)
					PrepareLegendsFormatService(null, null, CurrentWeightLegend, format);
			}
		}
		protected override DataSourceAdapterBase GetDataAdapter(VectorItemsLayer layer) {
			if(layer != null && layer == pieLayer)
				return PieDataAdapter;
			return null;
		}
		protected override void ClearDataLayers() {
			if(pieLayer != null) {
				if(PieDataAdapter != null)
					PieDataAdapter.DataSource = null;
				if(PieStorageDataAdapter != null)
					PieStorageDataAdapter.Items.Clear();
			}
		}
		protected override IList PrepareSelection(object selection) {
			IList res = base.PrepareSelection(selection);
			if(res != null)
				return res;
			MapPie storagePie = selection as MapPie;
			if(storagePie != null)
				return new object[] { ((IList)storagePie.Attributes["LatitudeSelection"].Value)[0], ((IList)storagePie.Attributes["LongitudeSelection"].Value)[0] };
			return null;
		}
		PieChartDataAdapter InitializePieDataAdapter() {
			PieChartDataAdapter pieDataAdapter = new PieChartDataAdapter();
			InitializeDataAdapter(pieDataAdapter);
			pieDataAdapter.AttributeMappings.Add(new MapItemAttributeMapping("ClusteredCount", "ClusteredCount"));
			pieDataAdapter.Mappings.Latitude = "Latitude";
			pieDataAdapter.Mappings.Longitude = "Longitude";
			pieDataAdapter.Mappings.PieSegment = "Argument";
			pieDataAdapter.Mappings.Value = "Value";
			pieDataAdapter.MeasureRules = new MeasureRules { ValueProvider = new ChartItemValueProvider() };
			pieDataAdapter.ItemMinSize = MinPieSize;
			pieDataAdapter.ItemMaxSize = MaxPieSize;
			pieDataAdapter.PieItemDataMember = "PieItemDataMember";
			return pieDataAdapter;
		}
		SuperToolTip GetPieToolTip(MapPie pie) {
			SuperToolTip superTip = new SuperToolTip();
			ConfigureTooltipDimesionsInformation(superTip, pie, true);
			if(CurrentGeoPointViewModel.EnableClustering) {
				int count = (int)((IList)pie.Attributes["ClusteredCount"].Value)[0];
				if(count > 1)
					AddNumberOfPointsToTooltip(superTip, count);
			}
			IList pieTooltips = (IList)pie.Attributes["MainTooltip"].Value;
			foreach(IList<string> segmentTooltip in pieTooltips) {
				if(segmentTooltip != null) {
					foreach(string text in segmentTooltip)
						superTip.Items.Add(new ToolTipItem() { Text = text });
				}
			}
			ConfigTooltipMeasuresInformation(CurrentGeoPointViewModel.TooltipMeasures, superTip, pie, true);
			return superTip;
		}
		void UpdatePieData(PieMapDashboardItemViewModel viewModel, PieMapMultiDimensionalDataSource data) {
			List<DashboardMapPieSegment> pieSegments = CreatePieSegments(viewModel, data);
			if(pieSegments.Count == 0)
				return;
			if(viewModel.IsWeighted) {
				PieLayer.Data = InitializePieDataAdapter();
				PieDataAdapter.DataSource = pieSegments;
				double minValue = pieSegments.GroupBy(segment => segment.PieItemDataMember).Min(group => group.Sum(segment => segment.Value));
				double maxValue = pieSegments.GroupBy(segment => segment.PieItemDataMember).Max(group => group.Sum(segment => segment.Value));
				DoubleCollection pieRangeStops = GetRangeStops(minValue, maxValue, PieValueRangeStopCount);
				PieDataAdapter.MeasureRules.RangeStops.Clear();
				PieDataAdapter.MeasureRules.RangeStops.AddRange(pieRangeStops);
			}
			else {
				PieLayer.Data = new MapItemStorage();
				IList<MapPie> pies = pieSegments.GroupBy(segment => segment.PieItemDataMember).Select(group => {
					DashboardMapPieSegment firstSegment = group.First();
					MapPie pie = new MapPie() { Location = new GeoPoint(CorrectLatitude(firstSegment.Latitude), Helper.ConvertToDouble(firstSegment.Longitude)) };
					foreach(DashboardMapPieSegment segment in group)
						pie.Segments.Add(new PieSegment() { Argument = segment.Argument, Value = segment.Value });
					pie.Attributes.Add(new MapItemAttribute() { Name = "ClusteredCount", Value = group.Select(segment => segment.ClusteredCount).ToArray() });
					pie.Attributes.Add(new MapItemAttribute() { Name = "TooltipDimensions", Value = group.Select(segment => segment.TooltipDimensions).ToArray() });
					pie.Attributes.Add(new MapItemAttribute() { Name = "TooltipMeasures", Value = group.Select(segment => segment.TooltipMeasures).ToArray() });
					pie.Attributes.Add(new MapItemAttribute() { Name = "MainTooltip", Value = group.Select(segment => segment.MainTooltip).ToArray() });
					pie.Attributes.Add(new MapItemAttribute() { Name = "LatitudeSelection", Value = new[] { firstSegment.LatitudeSelection } });
					pie.Attributes.Add(new MapItemAttribute() { Name = "LongitudeSelection", Value = new[] { firstSegment.LongitudeSelection } });
					pie.Size = 20;
					if(firstSegment.ClusteredCount > 1)
						pie.Size += 10;
					if(group.Count(segment => segment.Value != 0) > 1)
						pie.Size += 10;
					return pie;
				}).ToList();
				foreach(MapPie pie in pies)
					PieStorageDataAdapter.Items.Add(pie);
			}
			PreparePieMapColorizer(pieSegments);
		}
		List<DashboardMapPieSegment> CreatePieSegments(PieMapDashboardItemViewModel viewModel, PieMapMultiDimensionalDataSource data) {
			Dictionary<object, int> argumentsRepository = new Dictionary<object, int>();
			List<DashboardMapPieSegment> pieSegments = new List<DashboardMapPieSegment>();
			int uniqueArgumentIndex = 0;
			for(int i = 0; i < data.Count; i++) {
				if(viewModel.ArgumentDataId != null) {
					DashboardMapPieSegment pieSegment = new DashboardMapPieSegment(viewModel, data, i);
					pieSegment.Fill(viewModel, data, i);
					object argumentValue = pieSegment.Argument.Value;
					int argumentIndex;
					if(argumentsRepository.TryGetValue(argumentValue, out argumentIndex))
						pieSegment.SetPieArgumentIndex(argumentIndex);
					else {
						argumentsRepository.Add(argumentValue, uniqueArgumentIndex);
						pieSegment.SetPieArgumentIndex(uniqueArgumentIndex);
						uniqueArgumentIndex++;
					}
					pieSegments.Add(pieSegment);
				}
				else {
					for(int j = 0; j < viewModel.Values.Count; j++) {
						DashboardMapPieSegment pieSegment = new DashboardMapPieSegment(viewModel, data, i);
						pieSegment.Fill(viewModel.Values[j], data, i, j);
						pieSegments.Add(pieSegment);
					}
				}
			}
			return pieSegments;
		}
		void PreparePieMapColorizer(List<DashboardMapPieSegment> pieSegments) {
			KeyColorColorizer colorizer = new KeyColorColorizer();
			colorizer.ItemKeyProvider = new ArgumentItemKeyProvider();
			colorizer.Keys.Clear();
			List<int> argumentIndexes = new List<int>();
			foreach(DashboardMapPieSegment segment in pieSegments) {
				DashboardMapPieArgument argument = segment.Argument;
				if(!argumentIndexes.Contains(argument.Index)) {
					argumentIndexes.Add(argument.Index);
					colorizer.Keys.Add(new ColorizerKeyItem() { Key = argument, Name = argument.DisplayText });
				}
			}
			int uniqueArgumentsCount = argumentIndexes.Count;
			IList<Color> colors = FillPieLegendColors(MapControl.PaletteColors, uniqueArgumentsCount);
			colorizer.Colors.Clear();
			for(int i = 0; i < uniqueArgumentsCount; i++)
				colorizer.Colors.Add(colors[i]);
			PieLayer.Colorizer = colorizer;
		}
		IList<Color> FillPieLegendColors(ColorCollection colors, int argumentsCount) {
			List<Color> newColors = new List<Color>();
			int index = 0;
			for(int i = 0; i < argumentsCount; i++) {
				if(index >= colors.Count)
					index = 0;
				newColors.Add(colors[index++]);
			}
			return newColors;
		}
		double CorrectLatitude(object value) {
			double res = Helper.ConvertToDouble(value);
			if(res > 90)
				return 90;
			if(res < -90)
				return -90;
			return res;
		}
	}
}
