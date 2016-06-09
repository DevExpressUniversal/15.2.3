#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Utils;
using System.Diagnostics;
using DevExpress.Office.DrawingML;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Drawing;
#if SL
using System.Windows.Controls;
#endif
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region ModifyChartLayoutCommand
	public class ModifyChartLayoutCommand : ModifyChartCommandBase {
		static readonly ChartViewType none = (ChartViewType)(-1);
		static readonly ChartViewType column = (ChartViewType)(-2);
		public static ChartViewType None { get { return none; } }
		public static ChartViewType Column { get { return column; } }
		public ModifyChartLayoutCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public ChartLayoutModifier Modifier { get; set; }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string ImageName {
			get {
				if (Modifier == null)
					return base.ImageName;
				if (!String.IsNullOrEmpty(Modifier.ImageName))
					return "ChartPresets." + Modifier.ImageName;
				else
					return base.ImageName;
			}
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ModifyChartLayout; } }
		#endregion
		protected override Image LoadImage() {
			return LoadImageCore();
		}
		protected override Image LoadLargeImage() {
			return LoadImageCore();
		}
		Image LoadImageCore() {
			return CommandResourceImageLoader.CreateBitmapFromResources(ImageResourcePrefix + "." + ImageName + ".png", ImageResourceAssembly);
		}
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<ChartLayoutModifier> valueBasedState = state as IValueBasedCommandUIState<ChartLayoutModifier>;
			if (valueBasedState == null)
				return;
			this.Modifier = valueBasedState.Value;
			base.ForceExecute(state);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<ChartLayoutModifier>();
		}
		protected override bool IsChecked(Chart chart) {
			return false;
		}
		protected override bool ShouldHideCommand(Chart chart) {
			return false;
		}
		protected override bool CanModifyChart(Chart chart) {
			if (Modifier == null)
				return true; 
			return Modifier.CanModifyChart(chart);
		}
		protected override void ModifyChart(Chart chart) {
			if (Modifier != null)
				Modifier.ModifyChart(chart);
		}
		public ChartPresetCategory CalculateChartPresetCategory() {
			List<Chart> selectedCharts = GetSelectedCharts();
			if (selectedCharts.Count <= 0)
				return ChartPresetCategory.None;
			IChartView initialView = GetChartView(selectedCharts[0]);
			if (initialView == null)
				return ChartPresetCategory.None;
			return CalculateChartPresetCategory(initialView);
		}
		ChartPresetCategory CalculateChartPresetCategory(IChartView view) {
			if (view.ViewType == ChartViewType.Bar || view.ViewType == ChartViewType.Bar3D) {
				BarChartViewBase barView = view as BarChartViewBase;
				Debug.Assert(barView != null);
				if (barView.BarDirection == BarChartDirection.Column) {
					if (barView.Grouping == BarChartGrouping.Clustered || barView.Grouping == BarChartGrouping.Standard)
						return ChartPresetCategory.Column;
					else
						return ChartPresetCategory.ColumnStacked;
				}
				else {
					if (barView.Grouping == BarChartGrouping.Clustered || barView.Grouping == BarChartGrouping.Standard)
						return ChartPresetCategory.Bar;
					else
						return ChartPresetCategory.BarStacked;
				}
			}
			if (view.ViewType == ChartViewType.Line || view.ViewType == ChartViewType.Line3D) {
				if (GetGrouping(view) == ChartGrouping.Standard)
					return ChartPresetCategory.Line;
				else
					return ChartPresetCategory.LineStacked;
			}
			if (view.ViewType == ChartViewType.Pie || view.ViewType == ChartViewType.Pie3D || view.ViewType == ChartViewType.OfPie)
				return ChartPresetCategory.Pie;
			if (view.ViewType == ChartViewType.Doughnut)
				return ChartPresetCategory.Doughnut;
			if (view.ViewType == ChartViewType.Area || view.ViewType == ChartViewType.Area3D) {
				if (GetGrouping(view) == ChartGrouping.Standard)
					return ChartPresetCategory.Area;
				else
					return ChartPresetCategory.AreaStacked;
			}
			if (view.ViewType == ChartViewType.Scatter)
				return ChartPresetCategory.Scatter;
			if (view.ViewType == ChartViewType.Bubble)
				return ChartPresetCategory.Bubble;
			if (view.ViewType == ChartViewType.Radar)
				return ChartPresetCategory.Radar;
			if (view.ViewType == ChartViewType.Stock)
				return ChartPresetCategory.Stock;
			return ChartPresetCategory.None;
		}
		IChartView GetChartView(Chart chart) {
			if (chart.Views.Count <= 0)
				return null;
			return chart.Views[0];
		}
		ChartGrouping GetGrouping(IChartView view) {
			ChartViewWithGroupingAndDropLines groupingView = view as ChartViewWithGroupingAndDropLines;
			if (groupingView != null)
				return groupingView.Grouping;
			return ChartGrouping.Standard;
		}
	}
	#endregion
	#region ChartLayoutPreset
	public class ChartLayoutPreset {
		const LegendPosition legendNone = (LegendPosition)(-1);
		const DataLabelPosition labelsNone = (DataLabelPosition)(-1);
		public static LegendPosition NoLegend { get { return legendNone; } }
		public static DataLabelPosition NoLabels { get { return labelsNone; } }
		public ChartLayoutPreset() {
			LegendPosition = NoLegend;
			DataLabelPosition = NoLabels;
			LastSeriesDataLabelPosition = NoLabels;
			LastDataPointDataLabelPosition = NoLabels;
			HorizontalMajorTickMarks = TickMark.None;
			HorizontalMinorTickMarks = TickMark.None;
			VerticalMajorTickMarks = TickMark.None;
			VerticalMinorTickMarks = TickMark.None;
			DataLabelsShowValue = true;
			ImageName = String.Empty;
		}
		public virtual string ImageName { get; set; }
		public virtual bool HasChartTitle { get; set; }
		public virtual LegendPosition LegendPosition { get; set; }
		public virtual bool IsHorizontalAxisVisible { get; set; }
		public virtual bool IsVerticalAxisVisible { get; set; }
		public virtual bool HorizontalAxisTitle { get; set; }
		public virtual bool VerticalAxisTitle { get; set; }
		public virtual bool HorizontalMajorGridlines { get; set; }
		public virtual bool HorizontalMinorGridlines { get; set; }
		public virtual bool VerticalMajorGridlines { get; set; }
		public virtual bool VerticalMinorGridlines { get; set; }
		public virtual TickMark HorizontalMajorTickMarks { get; set; }
		public virtual TickMark HorizontalMinorTickMarks { get; set; }
		public virtual TickMark VerticalMajorTickMarks { get; set; }
		public virtual TickMark VerticalMinorTickMarks { get; set; }
		public virtual bool IsHorizontalAxisTransparent { get; set; }
		public virtual bool IsVerticalAxisTransparent { get; set; }
		public virtual DataLabelPosition DataLabelPosition { get; set; }
		public virtual DataLabelPosition LastSeriesDataLabelPosition { get; set; }
		public virtual DataLabelPosition LastDataPointDataLabelPosition { get; set; }
		public virtual bool HasDataTable { get; set; }
		public virtual bool LastSeriesDataLabelShowSeriesName { get; set; }
		public virtual bool DataLabelsShowValue { get; set; }
		public virtual bool DataLabelsShowPercentage { get; set; }
		public virtual bool DataLabelsShowCategoryName { get; set; }
		public virtual int GapWidth { get; set; }
		public virtual int Overlap { get; set; }
		public virtual bool ShowSeriesLines { get; set; }
		public virtual bool ShowDropLines { get; set; }
		public virtual bool ShowHighLowLines { get; set; }
		public virtual bool ShowTrendlines { get; set; }
		public virtual bool TrendlineDisplayEquation { get; set; }
		public virtual bool TrendlineDisplayRSquare { get; set; }
	}
	#endregion
	#region ChartLayoutPresetRotated
	public class ChartLayoutPresetRotated : ChartLayoutPreset {
		readonly ChartLayoutPreset preset;
		public ChartLayoutPresetRotated(ChartLayoutPreset preset, string imageName) {
			this.preset = preset;
			this.ImageName = imageName;
		}
		public override bool HasChartTitle { get { return preset.HasChartTitle; } }
		public override LegendPosition LegendPosition { get { return preset.LegendPosition; } }
		public override bool IsHorizontalAxisVisible { get { return preset.IsVerticalAxisVisible; } }
		public override bool IsVerticalAxisVisible { get { return preset.IsHorizontalAxisVisible; } }
		public override bool HorizontalAxisTitle { get { return preset.VerticalAxisTitle; } }
		public override bool VerticalAxisTitle { get { return preset.HorizontalAxisTitle; } }
		public override bool HorizontalMajorGridlines { get { return preset.VerticalMajorGridlines; } }
		public override bool HorizontalMinorGridlines { get { return preset.VerticalMinorGridlines; } }
		public override bool VerticalMajorGridlines { get { return preset.HorizontalMajorGridlines; } }
		public override bool VerticalMinorGridlines { get { return preset.HorizontalMinorGridlines; } }
		public override TickMark HorizontalMajorTickMarks { get { return preset.VerticalMajorTickMarks; } }
		public override TickMark HorizontalMinorTickMarks { get { return preset.VerticalMinorTickMarks; } }
		public override TickMark VerticalMajorTickMarks { get { return preset.HorizontalMajorTickMarks; } }
		public override TickMark VerticalMinorTickMarks { get { return preset.HorizontalMinorTickMarks; } }
		public override bool IsHorizontalAxisTransparent { get { return preset.IsVerticalAxisTransparent; } }
		public override bool IsVerticalAxisTransparent { get { return preset.IsHorizontalAxisTransparent; } }
		public override DataLabelPosition DataLabelPosition { get { return preset.DataLabelPosition; } }
		public override DataLabelPosition LastSeriesDataLabelPosition { get { return preset.LastSeriesDataLabelPosition; } }
		public override DataLabelPosition LastDataPointDataLabelPosition { get { return preset.LastDataPointDataLabelPosition; } }
		public override bool LastSeriesDataLabelShowSeriesName { get { return preset.LastSeriesDataLabelShowSeriesName; } }
		public override bool DataLabelsShowValue { get { return preset.DataLabelsShowValue; } }
		public override bool DataLabelsShowPercentage { get { return preset.DataLabelsShowPercentage; } }
		public override bool DataLabelsShowCategoryName { get { return preset.DataLabelsShowCategoryName; } }
		public override bool HasDataTable { get { return preset.HasDataTable; } }
		public override int GapWidth { get { return preset.GapWidth; } }
		public override int Overlap { get { return preset.Overlap; } }
		public override bool ShowSeriesLines { get { return preset.ShowSeriesLines; } }
		public override bool ShowDropLines { get { return preset.ShowDropLines; } }
		public override bool ShowHighLowLines { get { return preset.ShowHighLowLines; } }
		public override bool ShowTrendlines { get { return preset.ShowTrendlines; } }
		public override bool TrendlineDisplayEquation { get { return preset.TrendlineDisplayEquation; } }
		public override bool TrendlineDisplayRSquare { get { return preset.TrendlineDisplayRSquare; } }
	}
	#endregion
	public abstract class ChartPresets {
		readonly ChartLayoutModifier defaultModifier;
		readonly IList<ChartLayoutModifier> modifiers;
		protected ChartPresets() {
			this.modifiers = new List<ChartLayoutModifier>();
			foreach (ChartLayoutPreset preset in Presets)
				this.modifiers.Add(CreateModifier(preset));
			this.defaultModifier = CreateModifier(DefaultPreset);
		}
		public abstract ChartLayoutPreset DefaultPreset { get; }
		public abstract IList<ChartLayoutPreset> Presets { get; }
		public IList<ChartLayoutModifier> Modifiers { get { return modifiers; } }
		public ChartLayoutModifier DefaultModifier { get { return defaultModifier; } }
		protected abstract ChartLayoutModifier CreateModifier(ChartLayoutPreset preset);
	}
	#region ChartColumnClusteredPresets
	public class ChartColumnClusteredPresets : ChartPresets {
		#region Presets implementation
		#region Preset01
		static readonly ChartLayoutPreset preset01 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered01",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			GapWidth = 150,
		};
		public static ChartLayoutPreset Preset01 { get { return preset01; } }
		#endregion
		#region Preset02
		static readonly ChartLayoutPreset preset02 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered02",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Top,
			IsHorizontalAxisVisible = true,
			DataLabelPosition = DataLabelPosition.OutsideEnd,
			GapWidth = 150,
			Overlap = -25,
		};
		public static ChartLayoutPreset Preset02 { get { return preset02; } }
		#endregion
		#region Preset03
		static readonly ChartLayoutPreset preset03 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered03",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			IsVerticalAxisTransparent = true,
			GapWidth = 75,
			Overlap = -25,
		};
		public static ChartLayoutPreset Preset03 { get { return preset03; } }
		#endregion
		#region Preset04
		static readonly ChartLayoutPreset preset04 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered04",
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			DataLabelPosition = DataLabelPosition.OutsideEnd,
			GapWidth = 75,
		};
		public static ChartLayoutPreset Preset04 { get { return preset04; } }
		#endregion
		#region Preset05
		static readonly ChartLayoutPreset preset05 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered05",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HasDataTable = true,
			GapWidth = 150,
		};
		public static ChartLayoutPreset Preset05 { get { return preset05; } }
		#endregion
		#region Preset06
		static readonly ChartLayoutPreset preset06 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered06",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			LastDataPointDataLabelPosition = DataLabelPosition.OutsideEnd,
			LastSeriesDataLabelShowSeriesName = true,
			GapWidth = 150,
		};
		public static ChartLayoutPreset Preset06 { get { return preset06; } }
		#endregion
		#region Preset07
		static readonly ChartLayoutPreset preset07 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered07",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMinorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			GapWidth = 300,
		};
		public static ChartLayoutPreset Preset07 { get { return preset07; } }
		#endregion
		#region Preset08
		static readonly ChartLayoutPreset preset08 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered08",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorTickMarks = TickMark.Outside,
		};
		public static ChartLayoutPreset Preset08 { get { return preset08; } }
		#endregion
		#region Preset09
		static readonly ChartLayoutPreset preset09 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered09",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			GapWidth = 150,
		};
		public static ChartLayoutPreset Preset09 { get { return preset09; } }
		#endregion
		#region Preset10
		static readonly ChartLayoutPreset preset10 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered10",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			LastSeriesDataLabelPosition = DataLabelPosition.InsideEnd,
			GapWidth = 75,
			Overlap = 40,
		};
		public static ChartLayoutPreset Preset10 { get { return preset10; } }
		#endregion
		#region Preset11
		static readonly ChartLayoutPreset preset11 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnClustered11",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
			GapWidth = 150,
		};
		public static ChartLayoutPreset Preset11 { get { return preset11; } }
		#endregion
		#endregion
		static readonly ChartLayoutPreset[] presets = new ChartLayoutPreset[] { preset01, preset02, preset03, preset04, preset05, preset06, preset07, preset08, preset09, preset10, preset11 };
		static ChartColumnClusteredPresets instance;
		public override ChartLayoutPreset DefaultPreset { get { return preset11; } }
		public override IList<ChartLayoutPreset> Presets { get { return presets; } }
		protected override ChartLayoutModifier CreateModifier(ChartLayoutPreset preset) {
			return new ChartLayoutModifierColumn(preset);
		}
		public static ChartColumnClusteredPresets Instance {
			get {
				if (instance == null)
					instance = new ChartColumnClusteredPresets();
				return instance;
			}
		}
	}
	#endregion
	#region ChartColumnStackedPresets
	public class ChartColumnStackedPresets : ChartPresets {
		#region Presets implementation
		#region Preset01
		static readonly ChartLayoutPreset preset01 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnStacked01",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			GapWidth = 55,
			Overlap = 100,
		};
		public static ChartLayoutPreset Preset01 { get { return preset01; } }
		#endregion
		#region Preset02
		static readonly ChartLayoutPreset preset02 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnStacked02",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Top,
			IsHorizontalAxisVisible = true,
			DataLabelPosition = DataLabelPosition.Center,
			GapWidth = 95,
			Overlap = 100,
		};
		public static ChartLayoutPreset Preset02 { get { return preset02; } }
		#endregion
		#region Preset03
		static readonly ChartLayoutPreset preset03 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnStacked03",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			IsVerticalAxisTransparent = true,
			GapWidth = 75,
			Overlap = 100,
		};
		public static ChartLayoutPreset Preset03 { get { return preset03; } }
		#endregion
		#region Preset04
		static readonly ChartLayoutPreset preset04 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnStacked04",
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			DataLabelPosition = DataLabelPosition.Center,
			GapWidth = 75,
			Overlap = 100,
		};
		public static ChartLayoutPreset Preset04 { get { return preset04; } }
		#endregion
		#region Preset05
		static readonly ChartLayoutPreset preset05 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnStacked05",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HasDataTable = true,
			GapWidth = 95,
			Overlap = 100,
		};
		public static ChartLayoutPreset Preset05 { get { return preset05; } }
		#endregion
		#region Preset06
		static readonly ChartLayoutPreset preset06 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnStacked06",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			LastDataPointDataLabelPosition = DataLabelPosition.Center,
			LastSeriesDataLabelShowSeriesName = true,
			GapWidth = 55,
			Overlap = 100,
		};
		public static ChartLayoutPreset Preset06 { get { return preset06; } }
		#endregion
		#region Preset07
		static readonly ChartLayoutPreset preset07 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnStacked07",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMinorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			GapWidth = 75,
			Overlap = 100,
		};
		public static ChartLayoutPreset Preset07 { get { return preset07; } }
		#endregion
		#region Preset08
		static readonly ChartLayoutPreset preset08 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnStacked08",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			GapWidth = 300,
			Overlap = 100,
			ShowSeriesLines = true,
		};
		public static ChartLayoutPreset Preset08 { get { return preset08; } }
		#endregion
		#region Preset09
		static readonly ChartLayoutPreset preset09 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnStacked09",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			LastSeriesDataLabelPosition = DataLabelPosition.InsideEnd,
			GapWidth = 300,
		};
		public static ChartLayoutPreset Preset09 { get { return preset09; } }
		#endregion
		#region Preset10
		static readonly ChartLayoutPreset preset10 = new ChartLayoutPreset() {
			ImageName = "ChartPresetColumnStacked10",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
			GapWidth = 150,
			Overlap = 100,
		};
		public static ChartLayoutPreset Preset10 { get { return preset10; } }
		#endregion
		#endregion
		static readonly ChartLayoutPreset[] presets = new ChartLayoutPreset[] { preset01, preset02, preset03, preset04, preset05, preset06, preset07, preset08, preset09, preset10 };
		static ChartColumnStackedPresets instance;
		public override ChartLayoutPreset DefaultPreset { get { return preset10; } }
		public override IList<ChartLayoutPreset> Presets { get { return presets; } }
		protected override ChartLayoutModifier CreateModifier(ChartLayoutPreset preset) {
			return new ChartLayoutModifierColumn(preset);
		}
		public static ChartColumnStackedPresets Instance {
			get {
				if (instance == null)
					instance = new ChartColumnStackedPresets();
				return instance;
			}
		}
	}
	#endregion
	#region ChartBarClusteredPresets
	public class ChartBarClusteredPresets : ChartPresets {
		static readonly ChartLayoutPreset preset01 = new ChartLayoutPresetRotated(ChartColumnClusteredPresets.Preset01, "ChartPresetBarClustered01");
		static readonly ChartLayoutPreset preset02 = new ChartLayoutPresetRotated(ChartColumnClusteredPresets.Preset02, "ChartPresetBarClustered02");
		static readonly ChartLayoutPreset preset03 = new ChartLayoutPresetRotated(ChartColumnClusteredPresets.Preset03, "ChartPresetBarClustered03");
		static readonly ChartLayoutPreset preset04 = new ChartLayoutPresetRotated(ChartColumnClusteredPresets.Preset04, "ChartPresetBarClustered04");
		#region Preset05
		static readonly ChartLayoutPreset preset05 = new ChartLayoutPreset() {
			ImageName = "ChartPresetBarClustered05",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			VerticalMajorGridlines = true,
			HasDataTable = true,
			GapWidth = 150,
		};
		public static ChartLayoutPreset Preset05 { get { return preset05; } }
		#endregion
		static readonly ChartLayoutPreset preset06 = new ChartLayoutPresetRotated(ChartColumnClusteredPresets.Preset06, "ChartPresetBarClustered06");
		static readonly ChartLayoutPreset preset07 = new ChartLayoutPresetRotated(ChartColumnClusteredPresets.Preset07, "ChartPresetBarClustered07");
		static readonly ChartLayoutPreset preset08 = new ChartLayoutPresetRotated(ChartColumnClusteredPresets.Preset09, "ChartPresetBarClustered08");
		static readonly ChartLayoutPreset preset09 = new ChartLayoutPresetRotated(ChartColumnClusteredPresets.Preset10, "ChartPresetBarClustered09");
		static readonly ChartLayoutPreset preset10 = new ChartLayoutPresetRotated(ChartColumnClusteredPresets.Preset11, "ChartPresetBarClustered10");
		static readonly ChartLayoutPreset[] presets = new ChartLayoutPreset[] { preset01, preset02, preset03, preset04, preset05, preset06, preset07, preset08, preset09, preset10 };
		static ChartBarClusteredPresets instance;
		public override ChartLayoutPreset DefaultPreset { get { return preset10; } }
		public override IList<ChartLayoutPreset> Presets { get { return presets; } }
		protected override ChartLayoutModifier CreateModifier(ChartLayoutPreset preset) {
			return new ChartLayoutModifierColumn(preset);
		}
		public static ChartBarClusteredPresets Instance {
			get {
				if (instance == null)
					instance = new ChartBarClusteredPresets();
				return instance;
			}
		}
	}
	#endregion
	#region ChartBarStackedPresets
	public class ChartBarStackedPresets : ChartPresets {
		static readonly ChartLayoutPreset preset01 = new ChartLayoutPresetRotated(ChartColumnStackedPresets.Preset01, "ChartPresetBarStacked01");
		static readonly ChartLayoutPreset preset02 = new ChartLayoutPresetRotated(ChartColumnStackedPresets.Preset02, "ChartPresetBarStacked02");
		static readonly ChartLayoutPreset preset03 = new ChartLayoutPresetRotated(ChartColumnStackedPresets.Preset03, "ChartPresetBarStacked03");
		static readonly ChartLayoutPreset preset04 = new ChartLayoutPresetRotated(ChartColumnStackedPresets.Preset04, "ChartPresetBarStacked04");
		#region Preset05
		static readonly ChartLayoutPreset preset05 = new ChartLayoutPreset() {
			ImageName = "ChartPresetBarStacked05",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			VerticalMajorGridlines = true,
			HasDataTable = true,
			GapWidth = 95,
			Overlap = 100,
		};
		public static ChartLayoutPreset Preset05 { get { return preset05; } }
		#endregion
		static readonly ChartLayoutPreset preset06 = new ChartLayoutPresetRotated(ChartColumnStackedPresets.Preset06, "ChartPresetBarStacked06");
		static readonly ChartLayoutPreset preset07 = new ChartLayoutPresetRotated(ChartColumnStackedPresets.Preset07, "ChartPresetBarStacked07");
		static readonly ChartLayoutPreset preset08 = new ChartLayoutPresetRotated(ChartColumnStackedPresets.Preset08, "ChartPresetBarStacked08");
		static readonly ChartLayoutPreset preset09 = new ChartLayoutPresetRotated(ChartColumnStackedPresets.Preset09, "ChartPresetBarStacked09");
		static readonly ChartLayoutPreset preset10 = new ChartLayoutPresetRotated(ChartColumnStackedPresets.Preset10, "ChartPresetBarStacked10");
		static readonly ChartLayoutPreset[] presets = new ChartLayoutPreset[] { preset01, preset02, preset03, preset04, preset05, preset06, preset07, preset08, preset09, preset10 };
		static ChartBarStackedPresets instance;
		public override ChartLayoutPreset DefaultPreset { get { return preset10; } }
		public override IList<ChartLayoutPreset> Presets { get { return presets; } }
		protected override ChartLayoutModifier CreateModifier(ChartLayoutPreset preset) {
			return new ChartLayoutModifierColumn(preset);
		}
		public static ChartBarStackedPresets Instance {
			get {
				if (instance == null)
					instance = new ChartBarStackedPresets();
				return instance;
			}
		}
	}
	#endregion
	#region ChartLinePresets
	public class ChartLinePresets : ChartPresets {
		#region Presets implementation
		#region Preset01
		static readonly ChartLayoutPreset preset01 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine01",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
		};
		public static ChartLayoutPreset Preset01 { get { return preset01; } }
		#endregion
		#region Preset02
		static readonly ChartLayoutPreset preset02 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine02",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Top,
			IsHorizontalAxisVisible = true,
			DataLabelPosition = DataLabelPosition.Right,
		};
		public static ChartLayoutPreset Preset02 { get { return preset02; } }
		#endregion
		#region Preset03
		static readonly ChartLayoutPreset preset03 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine03",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			IsVerticalAxisTransparent = true,
		};
		public static ChartLayoutPreset Preset03 { get { return preset03; } }
		#endregion
		#region Preset04
		static readonly ChartLayoutPreset preset04 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine04",
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
		};
		public static ChartLayoutPreset Preset04 { get { return preset04; } }
		#endregion
		#region Preset05
		static readonly ChartLayoutPreset preset05 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine05",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HasDataTable = true,
		};
		public static ChartLayoutPreset Preset05 { get { return preset05; } }
		#endregion
		#region Preset06
		static readonly ChartLayoutPreset preset06 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine06",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			LastDataPointDataLabelPosition = DataLabelPosition.Right,
			LastSeriesDataLabelShowSeriesName = true,
		};
		public static ChartLayoutPreset Preset06 { get { return preset06; } }
		#endregion
		#region Preset07
		static readonly ChartLayoutPreset preset07 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine07",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			ShowDropLines = true,
		};
		public static ChartLayoutPreset Preset07 { get { return preset07; } }
		#endregion
		#region Preset08
		static readonly ChartLayoutPreset preset08 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine08",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsHorizontalAxisTransparent = true,
			DataLabelPosition = DataLabelPosition.Center,
		};
		public static ChartLayoutPreset Preset08 { get { return preset08; } }
		#endregion
		#region Preset09
		static readonly ChartLayoutPreset preset09 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine09",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			DataLabelPosition = DataLabelPosition.Right,
		};
		public static ChartLayoutPreset Preset09 { get { return preset09; } }
		#endregion
		#region Preset10
		static readonly ChartLayoutPreset preset10 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine10",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			ShowHighLowLines = true,
		};
		public static ChartLayoutPreset Preset10 { get { return preset10; } }
		#endregion
		#region Preset11
		static readonly ChartLayoutPreset preset11 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine11",
		};
		public static ChartLayoutPreset Preset11 { get { return preset11; } }
		#endregion
		#region Preset12
		static readonly ChartLayoutPreset preset12 = new ChartLayoutPreset() {
			ImageName = "ChartPresetLine12",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
		};
		public static ChartLayoutPreset Preset12 { get { return preset12; } }
		#endregion
		#endregion
		static readonly ChartLayoutPreset[] presets = new ChartLayoutPreset[] { preset01, preset02, preset03, preset04, preset05, preset06, preset07, preset08, preset09, preset10, preset11, preset12 };
		static ChartLinePresets instance;
		public override ChartLayoutPreset DefaultPreset { get { return preset12; } }
		public override IList<ChartLayoutPreset> Presets { get { return presets; } }
		protected override ChartLayoutModifier CreateModifier(ChartLayoutPreset preset) {
			return new ChartLayoutModifierLine(preset);
		}
		public static ChartLinePresets Instance {
			get {
				if (instance == null)
					instance = new ChartLinePresets();
				return instance;
			}
		}
	}
	#endregion
	#region ChartPiePresets
	public class ChartPiePresets : ChartPresets {
		#region Presets implementation
		#region Preset01
		static readonly ChartLayoutPreset preset01 = new ChartLayoutPreset() {
			ImageName = "ChartPresetPie01",
			HasChartTitle = true,
			DataLabelsShowValue = false,
			DataLabelsShowCategoryName = true,
			DataLabelsShowPercentage = true,
			DataLabelPosition = DataLabelPosition.BestFit,
		};
		public static ChartLayoutPreset Preset01 { get { return preset01; } }
		#endregion
		#region Preset02
		static readonly ChartLayoutPreset preset02 = new ChartLayoutPreset() {
			ImageName = "ChartPresetPie02",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Top,
			DataLabelsShowValue = false,
			DataLabelsShowPercentage = true,
			DataLabelPosition = DataLabelPosition.BestFit,
		};
		public static ChartLayoutPreset Preset02 { get { return preset02; } }
		#endregion
		#region Preset03
		static readonly ChartLayoutPreset preset03 = new ChartLayoutPreset() {
			ImageName = "ChartPresetPie03",
			LegendPosition = LegendPosition.Bottom,
		};
		public static ChartLayoutPreset Preset03 { get { return preset03; } }
		#endregion
		#region Preset04
		static readonly ChartLayoutPreset preset04 = new ChartLayoutPreset() {
			ImageName = "ChartPresetPie04",
			DataLabelsShowCategoryName = true,
			DataLabelPosition = DataLabelPosition.BestFit,
		};
		public static ChartLayoutPreset Preset04 { get { return preset04; } }
		#endregion
		#region Preset05
		static readonly ChartLayoutPreset preset05 = new ChartLayoutPreset() {
			ImageName = "ChartPresetPie05",
			HasChartTitle = true,
			DataLabelsShowValue = false,
			DataLabelsShowCategoryName = true,
			DataLabelPosition = DataLabelPosition.BestFit,
		};
		public static ChartLayoutPreset Preset05 { get { return preset05; } }
		#endregion
		#region Preset06
		static readonly ChartLayoutPreset preset06 = new ChartLayoutPreset() {
			ImageName = "ChartPresetPie06",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			DataLabelsShowValue = false,
			DataLabelsShowPercentage = true,
			DataLabelPosition = DataLabelPosition.BestFit,
		};
		public static ChartLayoutPreset Preset06 { get { return preset06; } }
		#endregion
		#region Preset07
		static readonly ChartLayoutPreset preset07 = new ChartLayoutPreset() {
			ImageName = "ChartPresetPie07",
			LegendPosition = LegendPosition.Right,
		};
		public static ChartLayoutPreset Preset07 { get { return preset07; } }
		#endregion
		#endregion
		static readonly ChartLayoutPreset[] presets = new ChartLayoutPreset[] { preset01, preset02, preset03, preset04, preset05, preset06, preset07 };
		static ChartPiePresets instance;
		public override ChartLayoutPreset DefaultPreset { get { return preset07; } }
		public override IList<ChartLayoutPreset> Presets { get { return presets; } }
		protected override ChartLayoutModifier CreateModifier(ChartLayoutPreset preset) {
			return new ChartLayoutModifierPie(preset);
		}
		public static ChartPiePresets Instance {
			get {
				if (instance == null)
					instance = new ChartPiePresets();
				return instance;
			}
		}
	}
	#endregion
	#region ChartAreaPresets
	public class ChartAreaPresets : ChartPresets {
		#region Presets implementation
		#region Preset01
		static readonly ChartLayoutPreset preset01 = new ChartLayoutPreset() {
			ImageName = "ChartPresetArea01",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
		};
		public static ChartLayoutPreset Preset01 { get { return preset01; } }
		#endregion
		#region Preset02
		static readonly ChartLayoutPreset preset02 = new ChartLayoutPreset() {
			ImageName = "ChartPresetArea02",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Top,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
		};
		public static ChartLayoutPreset Preset02 { get { return preset02; } }
		#endregion
		#region Preset03
		static readonly ChartLayoutPreset preset03 = new ChartLayoutPreset() {
			ImageName = "ChartPresetArea03",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
		};
		public static ChartLayoutPreset Preset03 { get { return preset03; } }
		#endregion
		#region Preset04
		static readonly ChartLayoutPreset preset04 = new ChartLayoutPreset() {
			ImageName = "ChartPresetArea04",
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			DataLabelPosition = DataLabelPosition.BestFit,
		};
		public static ChartLayoutPreset Preset04 { get { return preset04; } }
		#endregion
		#region Preset05
		static readonly ChartLayoutPreset preset05 = new ChartLayoutPreset() {
			ImageName = "ChartPresetArea05",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			HasDataTable = true,
		};
		public static ChartLayoutPreset Preset05 { get { return preset05; } }
		#endregion
		#region Preset06
		static readonly ChartLayoutPreset preset06 = new ChartLayoutPreset() {
			ImageName = "ChartPresetArea06",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMinorGridlines = true,
		};
		public static ChartLayoutPreset Preset06 { get { return preset06; } }
		#endregion
		#region Preset07
		static readonly ChartLayoutPreset preset07 = new ChartLayoutPreset() {
			ImageName = "ChartPresetArea07",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			ShowDropLines = true,
		};
		public static ChartLayoutPreset Preset07 { get { return preset07; } }
		#endregion
		#region Preset08
		static readonly ChartLayoutPreset preset08 = new ChartLayoutPreset() {
			ImageName = "ChartPresetArea08",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
		};
		public static ChartLayoutPreset Preset08 { get { return preset08; } }
		#endregion
		#endregion
		static readonly ChartLayoutPreset[] presets = new ChartLayoutPreset[] { preset01, preset02, preset03, preset04, preset05, preset06, preset07, preset08 };
		static ChartAreaPresets instance;
		public override ChartLayoutPreset DefaultPreset { get { return preset08; } }
		public override IList<ChartLayoutPreset> Presets { get { return presets; } }
		protected override ChartLayoutModifier CreateModifier(ChartLayoutPreset preset) {
			return new ChartLayoutModifierColumn(preset);
		}
		public static ChartAreaPresets Instance {
			get {
				if (instance == null)
					instance = new ChartAreaPresets();
				return instance;
			}
		}
	}
	#endregion
	#region ChartScatterPresets
	public class ChartScatterPresets : ChartPresets {
		#region Presets implementation
		#region Preset01
		static readonly ChartLayoutPreset preset01 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter01",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
		};
		public static ChartLayoutPreset Preset01 { get { return preset01; } }
		#endregion
		#region Preset02
		static readonly ChartLayoutPreset preset02 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter02",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Top,
			IsHorizontalAxisVisible = true,
			DataLabelPosition = DataLabelPosition.Right,
			DataLabelsShowCategoryName = true,
		};
		public static ChartLayoutPreset Preset02 { get { return preset02; } }
		#endregion
		#region Preset03
		static readonly ChartLayoutPreset preset03 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter03",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMinorGridlines = true,
			VerticalMajorGridlines = true,
			VerticalMinorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
			ShowTrendlines = true,
		};
		public static ChartLayoutPreset Preset03 { get { return preset03; } }
		#endregion
		#region Preset04
		static readonly ChartLayoutPreset preset04 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter04",
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
		};
		public static ChartLayoutPreset Preset04 { get { return preset04; } }
		#endregion
		#region Preset05
		static readonly ChartLayoutPreset preset05 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter05",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
			DataLabelPosition = DataLabelPosition.Right,
			DataLabelsShowCategoryName = true,
		};
		public static ChartLayoutPreset Preset05 { get { return preset05; } }
		#endregion
		#region Preset06
		static readonly ChartLayoutPreset preset06 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter06",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			LastDataPointDataLabelPosition = DataLabelPosition.Right,
			DataLabelsShowCategoryName = true,
		};
		public static ChartLayoutPreset Preset06 { get { return preset06; } }
		#endregion
		#region Preset07
		static readonly ChartLayoutPreset preset07 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter07",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			VerticalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
			DataLabelPosition = DataLabelPosition.Right,
			DataLabelsShowCategoryName = true,
		};
		public static ChartLayoutPreset Preset07 { get { return preset07; } }
		#endregion
		#region Preset08
		static readonly ChartLayoutPreset preset08 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter08",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Bottom,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			VerticalMajorGridlines = true,
		};
		public static ChartLayoutPreset Preset08 { get { return preset08; } }
		#endregion
		#region Preset09
		static readonly ChartLayoutPreset preset09 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter09",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMinorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
			ShowTrendlines = true,
			TrendlineDisplayEquation = true,
			TrendlineDisplayRSquare = true,
		};
		public static ChartLayoutPreset Preset09 { get { return preset09; } }
		#endregion
		#region Preset10
		static readonly ChartLayoutPreset preset10 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter10",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMinorGridlines = true,
			VerticalMajorGridlines = true,
			VerticalMinorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
		};
		public static ChartLayoutPreset Preset10 { get { return preset10; } }
		#endregion
		#region Preset11
		static readonly ChartLayoutPreset preset11 = new ChartLayoutPreset() {
			ImageName = "ChartPresetScatter11",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
		};
		public static ChartLayoutPreset Preset11 { get { return preset11; } }
		#endregion
		#endregion
		static readonly ChartLayoutPreset[] presets = new ChartLayoutPreset[] { preset01, preset02, preset03, preset04, preset05, preset06, preset07, preset08, preset09, preset10, preset11 };
		static ChartScatterPresets instance;
		public override ChartLayoutPreset DefaultPreset { get { return preset11; } }
		public override IList<ChartLayoutPreset> Presets { get { return presets; } }
		protected override ChartLayoutModifier CreateModifier(ChartLayoutPreset preset) {
			return new ChartLayoutModifierScatter(preset);
		}
		public static ChartScatterPresets Instance {
			get {
				if (instance == null)
					instance = new ChartScatterPresets();
				return instance;
			}
		}
	}
	#endregion
	#region ChartRadarPresets
	public class ChartRadarPresets : ChartPresets {
		#region Presets implementation
		#region Preset01
		static readonly ChartLayoutPreset preset01 = new ChartLayoutPreset() {
			ImageName = "ChartPresetRadar01",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			VerticalMajorGridlines = true,
		};
		public static ChartLayoutPreset Preset01 { get { return preset01; } }
		#endregion
		#region Preset02
		static readonly ChartLayoutPreset preset02 = new ChartLayoutPreset() {
			ImageName = "ChartPresetRadar02",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Top,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			VerticalMajorGridlines = true,
			DataLabelPosition = DataLabelPosition.Default,
		};
		public static ChartLayoutPreset Preset02 { get { return preset02; } }
		#endregion
		#region Preset03
		static readonly ChartLayoutPreset preset03 = new ChartLayoutPreset() {
			ImageName = "ChartPresetRadar03",
			HasChartTitle = true,
			LegendPosition = LegendPosition.Top,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			HorizontalMinorGridlines = true,
			VerticalMajorGridlines = true,
			VerticalMinorGridlines = true,
			DataLabelPosition = DataLabelPosition.Default,
		};
		public static ChartLayoutPreset Preset03 { get { return preset03; } }
		#endregion
		#region Preset04
		static readonly ChartLayoutPreset preset04 = new ChartLayoutPreset() {
			ImageName = "ChartPresetRadar04",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			VerticalMajorGridlines = true,
		};
		public static ChartLayoutPreset Preset04 { get { return preset04; } }
		#endregion
		#endregion
		static readonly ChartLayoutPreset[] presets = new ChartLayoutPreset[] { preset01, preset02, preset03, preset04 };
		static ChartRadarPresets instance;
		public override ChartLayoutPreset DefaultPreset { get { return preset04; } }
		public override IList<ChartLayoutPreset> Presets { get { return presets; } }
		protected override ChartLayoutModifier CreateModifier(ChartLayoutPreset preset) {
			return new ChartLayoutModifierLine(preset);
		}
		public static ChartRadarPresets Instance {
			get {
				if (instance == null)
					instance = new ChartRadarPresets();
				return instance;
			}
		}
	}
	#endregion
	#region ChartStockPresets
	public class ChartStockPresets : ChartPresets {
		#region Presets implementation
		#region Preset01
		static readonly ChartLayoutPreset preset01 = new ChartLayoutPreset() {
			ImageName = "ChartPresetStock01",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
		};
		public static ChartLayoutPreset Preset01 { get { return preset01; } }
		#endregion
		#region Preset02
		static readonly ChartLayoutPreset preset02 = new ChartLayoutPreset() {
			ImageName = "ChartPresetStock02",
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			DataLabelPosition = DataLabelPosition.Right
		};
		public static ChartLayoutPreset Preset02 { get { return preset02; } }
		#endregion
		#region Preset03
		static readonly ChartLayoutPreset preset03 = new ChartLayoutPreset() {
			ImageName = "ChartPresetStock03",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			IsVerticalAxisTransparent = true,
		};
		public static ChartLayoutPreset Preset03 { get { return preset03; } }
		#endregion
		#region Preset04
		static readonly ChartLayoutPreset preset04 = new ChartLayoutPreset() {
			ImageName = "ChartPresetStock04",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			HasDataTable = true,
		};
		public static ChartLayoutPreset Preset04 { get { return preset04; } }
		#endregion
		#region Preset05
		static readonly ChartLayoutPreset preset05 = new ChartLayoutPreset() {
			ImageName = "ChartPresetStock05",
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			HorizontalMinorGridlines = true,
		};
		public static ChartLayoutPreset Preset05 { get { return preset05; } }
		#endregion
		#region Preset06
		static readonly ChartLayoutPreset preset06 = new ChartLayoutPreset() {
			ImageName = "ChartPresetStock06",
			HasChartTitle = true,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalAxisTitle = true,
			VerticalAxisTitle = true,
			HorizontalMajorGridlines = true,
			LastDataPointDataLabelPosition = DataLabelPosition.Right,
		};
		public static ChartLayoutPreset Preset06 { get { return preset06; } }
		#endregion
		#region Preset07
		static readonly ChartLayoutPreset preset07 = new ChartLayoutPreset() {
			ImageName = "ChartPresetStock07",
			LegendPosition = LegendPosition.Right,
			IsHorizontalAxisVisible = true,
			IsVerticalAxisVisible = true,
			HorizontalMajorGridlines = true,
			HorizontalMajorTickMarks = TickMark.Outside,
			VerticalMajorTickMarks = TickMark.Outside,
		};
		public static ChartLayoutPreset Preset07 { get { return preset07; } }
		#endregion
		#endregion
		static readonly ChartLayoutPreset[] presets = new ChartLayoutPreset[] { preset01, preset02, preset03, preset04, preset05, preset06, preset07 };
		static ChartStockPresets instance;
		public override ChartLayoutPreset DefaultPreset { get { return preset07; } }
		public override IList<ChartLayoutPreset> Presets { get { return presets; } }
		protected override ChartLayoutModifier CreateModifier(ChartLayoutPreset preset) {
			return new ChartLayoutModifierColumn(preset);
		}
		public static ChartStockPresets Instance {
			get {
				if (instance == null)
					instance = new ChartStockPresets();
				return instance;
			}
		}
	}
	#endregion
	#region ChartPresetCategory
	public enum ChartPresetCategory {
		None,
		Column,
		ColumnStacked,
		Bar,
		BarStacked,
		Line,
		LineStacked,
		Pie,
		Doughnut,
		Area,
		AreaStacked,
		Scatter,
		Bubble,
		Radar,
		Stock,
	}
	#endregion
	#region ChartLayoutModifier (abstract class)
	public abstract class ChartLayoutModifier {
		ChartLayoutPreset preset;
		static readonly Dictionary<ChartPresetCategory, IList<ChartLayoutModifier>> modifierTable = CreateModifierTable();
		protected ChartLayoutModifier(ChartLayoutPreset preset) {
			Guard.ArgumentNotNull(preset, "preset");
			this.preset = preset;
		}
		#region Properties
		public ChartLayoutPreset Preset { get { return preset; } }
		public virtual string ImageName { get { return preset.ImageName; } }
		#endregion
		#region CreateModifierTable
		static Dictionary<ChartPresetCategory, IList<ChartLayoutModifier>> CreateModifierTable() {
			Dictionary<ChartPresetCategory, IList<ChartLayoutModifier>> result = new Dictionary<ChartPresetCategory, IList<ChartLayoutModifier>>();
			result.Add(ChartPresetCategory.Column, ChartColumnClusteredPresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.ColumnStacked, ChartColumnStackedPresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.Bar, ChartBarClusteredPresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.BarStacked, ChartBarStackedPresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.Line, ChartLinePresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.LineStacked, ChartLinePresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.Pie, ChartPiePresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.Doughnut, ChartPiePresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.Area, ChartAreaPresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.AreaStacked, ChartAreaPresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.Scatter, ChartScatterPresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.Bubble, ChartScatterPresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.Radar, ChartRadarPresets.Instance.Modifiers);
			result.Add(ChartPresetCategory.Stock, ChartStockPresets.Instance.Modifiers);
			return result;
		}
		#endregion
		public virtual bool CanModifyChart(Chart chart) {
			return chart.Views.Count > 0; 
		}
		public virtual void ModifyChart(Chart chart) {
			ModifyChartTitle(chart);
			ModifyLegend(chart);
			ModifyAxes(chart);
			ModifyViews(chart);
			ModifyDataTable(chart);
		}
		void ModifyDataTable(Chart chart) {
			DataTableOptions dataTable = chart.DataTable;
			if (Preset.HasDataTable) {
				dataTable.Visible = true;
				dataTable.ShowLegendKeys = true;
				dataTable.ShowVerticalBorder = true;
				dataTable.ShowHorizontalBorder = true;
			}
			else
				dataTable.Visible = false;
		}
		void ModifyChartTitle(Chart chart) {
			if (Preset.HasChartTitle) {
				chart.AutoTitleDeleted = false;
				if (chart.Title.Text.TextType == ChartTextType.None) {
					chart.Title.Text = ChartText.Auto;
					chart.Title.Overlay = false;
				}
			}
			else
				chart.Title.Text = ChartText.Empty;
		}
		void ModifyLegend(Chart chart) {
			if (Preset.LegendPosition == ChartLayoutPreset.NoLegend)
				chart.Legend.Visible = false;
			else {
				Legend legend = chart.Legend;
				legend.Overlay = false;
				legend.Position = Preset.LegendPosition;
				legend.Visible = true;
			}
		}
		protected virtual void ModifyAxes(Chart chart) {
			AxisBase horizontalAxis = ChartModifyPrimaryAxisCommandBase.GetAxis(chart, true);
			if (horizontalAxis != null)
				ModifyHorizontalAxis(horizontalAxis);
			AxisBase verticalAxis = ChartModifyPrimaryAxisCommandBase.GetAxis(chart, false);
			if (verticalAxis != null)
				ModifyVerticalAxis(verticalAxis);
		}
		protected virtual void ModifyHorizontalAxis(AxisBase axis) {
			axis.Delete = !Preset.IsHorizontalAxisVisible;
			axis.ShowMajorGridlines = Preset.VerticalMajorGridlines;
			axis.ShowMinorGridlines = Preset.VerticalMinorGridlines;
			axis.MajorTickMark = Preset.VerticalMajorTickMarks;
			axis.MinorTickMark = Preset.VerticalMinorTickMarks;
			if (Preset.IsHorizontalAxisTransparent)
				axis.ShapeProperties.Outline.Fill = DrawingFill.None;
			else
				axis.ShapeProperties.Outline.Fill = DrawingFill.Automatic;
			if (Preset.HorizontalAxisTitle) {
				if (axis.Title.Text.TextType == ChartTextType.None) {
					axis.Title.Overlay = false;
					axis.Title.Text = ChartText.Auto;
				}
			}
			else
				axis.Title.Text = ChartText.Empty;
		}
		protected virtual void ModifyVerticalAxis(AxisBase axis) {
			axis.Delete = !Preset.IsVerticalAxisVisible;
			axis.ShowMajorGridlines = Preset.HorizontalMajorGridlines;
			axis.ShowMinorGridlines = Preset.HorizontalMinorGridlines;
			axis.MajorTickMark = Preset.HorizontalMajorTickMarks;
			axis.MinorTickMark = Preset.HorizontalMinorTickMarks;
			if (Preset.IsVerticalAxisTransparent)
				axis.ShapeProperties.Outline.Fill = DrawingFill.None;
			else
				axis.ShapeProperties.Outline.Fill = DrawingFill.Automatic;
			if (Preset.VerticalAxisTitle) {
				TitleOptions title = axis.Title;
				if (title.Text.TextType == ChartTextType.None) {
					title.Overlay = false;
					title.Text = ChartText.Auto;
					title.TextProperties.BodyProperties.Rotation = axis.DocumentModel.UnitConverter.AdjAngleToModelUnits(-5400000);
					title.TextProperties.BodyProperties.VerticalText = DrawingTextVerticalTextType.Horizontal;
					DrawingTextParagraph paragraph = new DrawingTextParagraph(axis.DocumentModel);
					paragraph.ApplyParagraphProperties = true;
					paragraph.ParagraphProperties.ApplyDefaultCharacterProperties = true;
					title.TextProperties.Paragraphs.Add(paragraph);
				}
			}
			else
				axis.Title.Text = ChartText.Empty;
		}
		void ModifyViews(Chart chart) {
			if (chart.Views.Count <= 0)
				return;
			ChartViewType viewType = chart.Views[0].ViewType;
			foreach (IChartView chartView in chart.Views)
				if (chartView.ViewType == viewType)
					ModifyView(chartView);
		}
		protected virtual void ModifyView(IChartView chartView) {
			ChartViewWithDataLabels view = chartView as ChartViewWithDataLabels;
			if (view != null)
				ModifyDataLabels(view);
			ISupportsSeriesLines seriesLines = chartView as ISupportsSeriesLines;
			if (seriesLines != null) {
				SeriesLinesCollection lines = seriesLines.SeriesLines;
				lines.Clear();
				if (Preset.ShowSeriesLines)
					lines.Add(new ShapeProperties(view.DocumentModel) { Parent = view.Parent });
			}
		}
		void ModifyDataLabels(ChartViewWithDataLabels view) {
			ModifyViewDataLabels(view.DataLabels, Preset.DataLabelPosition);
			DataLabelPosition seriesDataLabelPosition = Preset.LastSeriesDataLabelPosition;
			for (int i = view.Series.Count - 1; i >= 0; i--) {
				SeriesWithDataLabelsAndPoints series = view.Series[i] as SeriesWithDataLabelsAndPoints;
				if (series != null) {
					ModifyDataLabels(series.DataLabels, seriesDataLabelPosition);
					series.DataLabels.Labels.Clear();
					if (Preset.LastDataPointDataLabelPosition != ChartLayoutPreset.NoLabels) {
						ResetDataLabelContent(series.DataLabels);
						series.DataLabels.Apply = true;
						DataLabel label = new DataLabel(view.Parent, (int)(series.Values.ValuesCount - 1));
						ModifyDataLabelContent(label);
						label.ShowSeriesName = Preset.LastSeriesDataLabelShowSeriesName;
						label.LabelPosition = Preset.LastDataPointDataLabelPosition;
						series.DataLabels.Labels.Add(label);
					}
					seriesDataLabelPosition = ChartLayoutPreset.NoLabels;
				}
			}
		}
		void ModifyViewDataLabels(DataLabels dataLabels, DataLabelPosition position) {
			if (position == ChartLayoutPreset.NoLabels) {
				dataLabels.Labels.Clear();
				ResetDataLabelContent(dataLabels);
			}
			else {
				ModifyDataLabelContent(dataLabels);
				dataLabels.LabelPosition = position;
			}
		}
		void ModifyDataLabels(DataLabels dataLabels, DataLabelPosition position) {
			if (position == ChartLayoutPreset.NoLabels)
				dataLabels.Apply = false;
			else {
				ModifyDataLabelContent(dataLabels);
				dataLabels.LabelPosition = position;
				dataLabels.Apply = true;
			}
		}
		protected virtual void ResetDataLabelContent(DataLabelBase dataLabels) {
			dataLabels.ShowValue = false;
			dataLabels.ShowCategoryName = false;
			dataLabels.ShowPercent = false;
		}
		protected virtual void ModifyDataLabelContent(DataLabelBase dataLabels) {
			dataLabels.ShowValue = Preset.DataLabelsShowValue;
			dataLabels.ShowCategoryName = Preset.DataLabelsShowCategoryName;
			dataLabels.ShowPercent = Preset.DataLabelsShowPercentage;
		}
		public static IList<ChartLayoutModifier> GetModifiers(ChartPresetCategory category) {
			IList<ChartLayoutModifier> result;
			if (modifierTable.TryGetValue(category, out result))
				return result;
			return new List<ChartLayoutModifier>();
		}
	}
	#endregion
	#region ChartLayoutModifierColumn
	public class ChartLayoutModifierColumn : ChartLayoutModifier {
		public ChartLayoutModifierColumn(ChartLayoutPreset preset)
			: base(preset) {
		}
		public override void ModifyChart(Chart chart) {
			base.ModifyChart(chart);
			ModifyGap(chart);
			ModifyOverlap(chart);
		}
		void ModifyGap(Chart chart) {
			int count = chart.Views.Count;
			for (int i = 0; i < count; i++) {
				BarChartViewBase view = chart.Views[i] as BarChartViewBase;
				if (view != null)
					view.GapWidth = Preset.GapWidth;
			}
		}
		void ModifyOverlap(Chart chart) {
			int count = chart.Views.Count;
			for (int i = 0; i < count; i++) {
				BarChartView view = chart.Views[i] as BarChartView;
				if (view != null)
					view.Overlap = Preset.Overlap;
			}
		}
	}
	#endregion
	#region ChartLayoutModifierLine
	public class ChartLayoutModifierLine : ChartLayoutModifier {
		public ChartLayoutModifierLine(ChartLayoutPreset preset)
			: base(preset) {
		}
		protected override void ModifyView(IChartView chartView) {
			base.ModifyView(chartView);
			ISupportsDropLines dropLines = chartView as ISupportsDropLines;
			if (dropLines != null)
				dropLines.ShowDropLines = Preset.ShowDropLines;
			ISupportsHiLowLines highLowLines = chartView as ISupportsHiLowLines;
			if (highLowLines != null)
				highLowLines.ShowHiLowLines = Preset.ShowHighLowLines;
		}
	}
	#endregion
	#region ChartLayoutModifierPie
	public class ChartLayoutModifierPie : ChartLayoutModifier {
		public ChartLayoutModifierPie(ChartLayoutPreset preset)
			: base(preset) {
		}
		protected override void ModifyAxes(Chart chart) {
		}
	}
	#endregion
	#region ChartLayoutModifierScatter
	public class ChartLayoutModifierScatter : ChartLayoutModifier {
		public ChartLayoutModifierScatter(ChartLayoutPreset preset)
			: base(preset) {
		}
		protected override void ModifyView(IChartView chartView) {
			base.ModifyView(chartView);
			ModifySeries(chartView.Series);
		}
		void ModifySeries(SeriesCollection seriesCollection) {
			foreach (ISeries series in seriesCollection)
				ModifySeries(series);
		}
		void ModifySeries(ISeries series) {
			if (Preset.ShowTrendlines) {
				SeriesWithErrorBarsAndTrendlines trendlineSeries = series as SeriesWithErrorBarsAndTrendlines;
				if (trendlineSeries != null) {
					Trendline trendline = new Trendline(trendlineSeries.Parent);
					trendline.DisplayEquation = Preset.TrendlineDisplayEquation;
					trendline.DisplayRSquare = Preset.TrendlineDisplayRSquare;
					trendlineSeries.Trendlines.Add(trendline);
				}
			}
		}
	}
	#endregion
}
